using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Resolves one inventory request and its scoped correction on the host's authority lock.</summary>
internal sealed class RunItemActions
{
    internal RunItemActions(FoundationSession session)
    {
        _session = session;
    }

    internal View? Read(Controller controller)
    {
        return _session.UnderInventoryAuthority(action: current =>
        {
            Access? access = Authenticate(current: current, controller: controller);
            return access is null ? null : Project(access: access, inventory: access.Root.Inventory,
                    revision: access.Root.Snapshot.Revision);
        });
    }

    internal Outcome Submit(Controller controller, long sequence, Request request)
    {
        return _session.UnderInventoryAuthority(action: current =>
        {
            Access? access = Authenticate(current: current, controller: controller);
            return access is null ? new Outcome(status: Status.AccessLost, view: null)
                    : Resolve(access: access, sequence: sequence, request: request);
        });
    }

    private readonly FoundationSession _session;
    private readonly Dictionary<string, Progress> _bindings = new(comparer: StringComparer.Ordinal);

    private Access? Authenticate(FoundationSession.RunPublication? current, Controller controller)
    {
        if (current is null || !StringComparer.Ordinal.Equals(x: current.Snapshot.SessionId, y: controller.SessionId)
                || current.Population.FindLivingControl(bindingId: controller.BindingId) is not { } lease
                || lease.Generation != controller.Generation
                || RunExternalAccess.Controller(current: current, lease: lease) is not { } participant)
        {
            return null;
        }

        if (!_bindings.TryGetValue(key: lease.BindingId, value: out Progress? progress)
                || progress.Generation != lease.Generation)
        {
            progress = new Progress(generation: lease.Generation);
            _bindings[lease.BindingId] = progress;
        }

        if (progress.Selection is { } selected && !RunExternalAccess.Allows(current: current,
                participant: participant, target: selected.Target))
        {
            progress.Selection = null;
            progress.SelectionLost = true;
        }

        return new Access(current: current, participant: participant, lease: lease, progress: progress);
    }

    private Outcome Resolve(Access access, long sequence, Request request)
    {
        Progress progress = access.Progress;
        RunInventory inventory = access.Root.Inventory;
        long revision = access.Root.Snapshot.Revision;
        if (sequence <= 0 || sequence < progress.Sequence)
        {
            return new Outcome(status: Status.StaleRequest,
                    view: Project(access: access, inventory: inventory, revision: revision));
        }

        if (sequence == progress.Sequence)
        {
            return new Outcome(status: progress.Status, replayed: true,
                    view: Project(access: access, inventory: inventory, revision: revision));
        }

        Status status = request switch
        {
            Select select => Choose(access: access, target: select.Target, sequence: sequence),
            Close close => CloseSelection(progress: progress, token: close.Token),
            Transfer transfer => Commit(access: access, transfer: transfer, inventory: ref inventory,
                    revision: ref revision),
            _ => Status.Incompatible,
        };
        progress.Sequence = sequence;
        progress.Status = status;
        return new Outcome(status: status,
                view: Project(access: access, inventory: inventory, revision: revision));
    }

    private static Status Choose(Access access, RunExternalAccess.Target target, long sequence)
    {
        if (!RunExternalAccess.Allows(current: access.Root, participant: access.Participant, target: target))
        {
            return Status.AccessLost;
        }

        access.Progress.Selection = new RunExternalAccess.Selection(target: target, token: sequence);
        access.Progress.SelectionLost = false;
        return Status.Ready;
    }

    private static Status CloseSelection(Progress progress, long token)
    {
        if (progress.Selection is not { } selected || token != selected.Token)
        {
            return Status.AccessLost;
        }

        progress.Selection = null;
        progress.SelectionLost = false;
        return Status.Ready;
    }

    private Status Commit(Access access, Transfer transfer, ref RunInventory inventory, ref long revision)
    {
        Status failure = Prepare(access: access, transfer: transfer, candidate: out RunInventory? candidate);
        if (candidate is null)
        {
            return failure;
        }

        if (!_session.TryEditInventory(edit: (current, _, _) => ReferenceEquals(objA: current,
                objB: access.Root.Inventory) ? candidate : null, change: out RunItemTransfer.Change? change)
                || change is null)
        {
            return Status.Incompatible;
        }

        inventory = change.After;
        revision = _session.Snapshot.Revision;
        return Status.Applied;
    }

    private static Status Prepare(Access access, Transfer transfer, out RunInventory? candidate)
    {
        candidate = null;
        RunInventory inventory = access.Root.Inventory;
        if (transfer.Expected.Items.IsDefaultOrEmpty || !Permits(access: access, token: transfer.Token,
                root: transfer.Expected.Root))
        {
            return Status.AccessLost;
        }

        RunInventory.Item? source = RunItemTransfer.Current(inventory: inventory, expected: transfer.Expected);
        if (source is null)
        {
            return Status.StaleItem;
        }

        return transfer switch
        {
            Drop drop => DropOnGround(access: access, drop: drop, candidate: out candidate),
            Merge merge => MergeStacks(access: access, merge: merge, source: source, candidate: out candidate),
            Place place => PlaceItem(access: access, place: place, source: source, candidate: out candidate),
            _ => Status.Incompatible,
        };
    }

    private static Status PlaceItem(Access access, Place place, RunInventory.Item source,
            out RunInventory? candidate)
    {
        candidate = null;
        RunInventory inventory = access.Root.Inventory;
        if (place.Destination is RunInventory.Ground)
        {
            return Status.Incompatible;
        }

        RunInventory.Location root = place.Destination;
        if (place.Destination is RunInventory.ContainerCell cell)
        {
            if (place.Container is not { Items.IsDefaultOrEmpty: false } container
                    || container.Item.Id != cell.ContainerId
                    || !Permits(access: access, token: place.Token, root: container.Root))
            {
                return Status.AccessLost;
            }

            if (RunItemTransfer.Current(inventory: inventory, expected: container) is null)
            {
                return Status.StaleItem;
            }

            root = container.Root;
        }

        if (!Permits(access: access, token: place.Token, root: root))
        {
            return Status.AccessLost;
        }

        Status placement = RunItemTransfer.CheckPlacement(inventory: inventory, definition:
                access.Root.Definitions.Items[source.DefinitionId], source: source,
                destination: place.Destination, split: place.Amount.HasValue);
        if (placement != Status.Ready)
        {
            return placement;
        }

        candidate = place.Amount is { } amount
                ? RunItemTransfer.Split(inventory: inventory, expected: place.Expected, quantity: amount,
                        destination: place.Destination)
                : RunItemTransfer.Move(inventory: inventory, expected: place.Expected,
                        destination: place.Destination);
        return Status.Incompatible;
    }

    private static Status MergeStacks(Access access, Merge merge, RunInventory.Item source,
            out RunInventory? candidate)
    {
        candidate = null;
        if (merge.Target.Items.IsDefaultOrEmpty || !Permits(access: access, token: merge.Token,
                root: merge.Target.Root))
        {
            return Status.AccessLost;
        }

        RunInventory inventory = access.Root.Inventory;
        RunInventory.Item? target = RunItemTransfer.Current(inventory: inventory, expected: merge.Target);
        if (target is null)
        {
            return Status.StaleItem;
        }

        RunDefinitions definitions = access.Root.Definitions;
        if (source.Id == target.Id || !StringComparer.Ordinal.Equals(x: source.DefinitionId, y: target.DefinitionId)
                || source.Condition != target.Condition
                || target.Location is RunInventory.Equipment || target.StorageGrid.HasValue)
        {
            return Status.Incompatible;
        }

        if (target.Quantity >= definitions.Items[target.DefinitionId].MaxStack)
        {
            return Status.Capacity;
        }

        candidate = RunItemTransfer.Merge(inventory: inventory, definitions: definitions,
                expectedSource: merge.Expected, expectedDestination: merge.Target);
        return Status.Incompatible;
    }

    private static Status DropOnGround(Access access, Drop drop, out RunInventory? candidate)
    {
        candidate = null;
        var target = new RunExternalAccess.Target(kind: RunWorld.Access.Pile, id: drop.PileId);
        if (!drop.Expected.IsCarriedBy(survivorId: access.Lease.SurvivorId)
                || !RunExternalAccess.Allows(current: access.Root, participant: access.Participant, target: target))
        {
            return Status.AccessLost;
        }

        candidate = RunItemTransfer.Drop(inventory: access.Root.Inventory, expected: drop.Expected,
                pileId: drop.PileId);
        return Status.Incompatible;
    }

    private static bool Permits(Access access, long? token, RunInventory.Location root)
    {
        return root is RunInventory.Equipment equipment
                && StringComparer.Ordinal.Equals(x: equipment.SurvivorId, y: access.Lease.SurvivorId)
                || access.Progress.Selection is { } current && token == current.Token
                        && RunExternalAccess.Contains(selection: current, location: root);
    }

    private static View Project(Access access, RunInventory inventory, long revision)
    {
        string owner = access.Lease.SurvivorId;
        RunExternalAccess.Selection? selected = access.Progress.Selection;
        ImmutableArray<RunExternalAccess.Target> nearby = RunExternalAccess.Nearby(current: access.Root,
                participant: access.Participant);
        Status availability = access.Progress.SelectionLost ? Status.AccessLost
                : nearby.IsEmpty ? Status.NoTargets : Status.Ready;
        return new View(revision: revision, survivorId: owner, nearby: nearby, selection: selected,
                personal: inventory.CarriedBy(survivorId: owner), external: selected is null ? []
                        : RunExternalAccess.Contents(inventory: inventory, selection: selected),
                quickReferences: [.. inventory.QuickReferences.Where(predicate: reference =>
                        StringComparer.Ordinal.Equals(x: reference.Key.survivor, y: owner))
                        .OrderBy(keySelector: reference => reference.Key.slot)
                        .Select(selector: reference => (reference.Key.slot, reference.Value))],
                availability: availability, stashPageCount: selected?.Target.Kind == RunWorld.Access.Stash
                        ? inventory.StashPageCount : null);
    }

    internal enum Status
    {
        Ready,
        NoTargets,
        Applied,
        AccessLost,
        StaleRequest,
        StaleItem,
        Incompatible,
        Capacity,
    }

    internal sealed record Controller
    {
        internal Controller(string sessionId, string bindingId, long generation)
        {
            SessionId = sessionId;
            BindingId = bindingId;
            Generation = generation;
        }

        internal string SessionId { get; }
        internal string BindingId { get; }
        internal long Generation { get; }
    }

    internal abstract record Request;

    internal sealed record Select : Request
    {
        internal Select(RunExternalAccess.Target target)
        {
            Target = target;
        }

        internal RunExternalAccess.Target Target { get; }
    }

    internal sealed record Close : Request
    {
        internal Close(long token)
        {
            Token = token;
        }

        internal long Token { get; }
    }

    internal abstract record Transfer : Request
    {
        internal RunInventory.ItemPath Expected { get; }
        internal long? Token { get; }

        protected Transfer(RunInventory.ItemPath expected, long? token)
        {
            Expected = expected;
            Token = token;
        }

    }

    internal sealed record Place : Transfer
    {
        internal Place(RunInventory.ItemPath expected, RunInventory.Location destination, long? token = null,
                RunInventory.ItemPath? container = null, decimal? amount = null)
                : base(expected: expected, token: token)
        {
            Destination = destination;
            Container = container;
            Amount = amount;
        }

        internal RunInventory.Location Destination { get; }
        internal RunInventory.ItemPath? Container { get; }
        internal decimal? Amount { get; }
    }

    internal sealed record Merge : Transfer
    {
        internal Merge(RunInventory.ItemPath expected, RunInventory.ItemPath target, long? token = null)
                : base(expected: expected, token: token)
        {
            Target = target;
        }

        internal RunInventory.ItemPath Target { get; }
    }

    internal sealed record Drop : Transfer
    {
        internal Drop(RunInventory.ItemPath expected, string pileId) : base(expected: expected, token: null)
        {
            PileId = pileId;
        }

        internal string PileId { get; }
    }

    internal sealed class View
    {
        internal View(long revision, string survivorId, ImmutableArray<RunExternalAccess.Target> nearby,
                RunExternalAccess.Selection? selection, ImmutableArray<RunInventory.Item> personal,
                ImmutableArray<RunInventory.Item> external, ImmutableArray<(int slot, long itemId)> quickReferences,
                Status availability, int? stashPageCount)
        {
            Revision = revision;
            SurvivorId = survivorId;
            Nearby = nearby;
            Selection = selection;
            Personal = personal;
            External = external;
            QuickReferences = quickReferences;
            Availability = availability;
            StashPageCount = stashPageCount;
        }

        internal long Revision { get; }
        internal string SurvivorId { get; }
        internal ImmutableArray<RunExternalAccess.Target> Nearby { get; }
        internal RunExternalAccess.Selection? Selection { get; }
        internal ImmutableArray<RunInventory.Item> Personal { get; }
        internal ImmutableArray<RunInventory.Item> External { get; }
        internal ImmutableArray<(int slot, long itemId)> QuickReferences { get; }
        internal Status Availability { get; }
        internal int? StashPageCount { get; }
    }

    internal sealed class Outcome
    {
        internal Outcome(Status status, View? view, bool replayed = false)
        {
            Status = status;
            View = view;
            Replayed = replayed;
        }

        internal Status Status { get; }
        internal View? View { get; }
        internal bool Replayed { get; }
    }

    private sealed class Progress
    {
        internal Progress(long generation)
        {
            Generation = generation;
        }

        internal long Generation { get; }
        internal long Sequence { get; set; }
        internal Status Status { get; set; }
        internal RunExternalAccess.Selection? Selection { get; set; }
        internal bool SelectionLost { get; set; }
    }

    private sealed class Access
    {
        internal Access(FoundationSession.RunPublication current, FoundationParticipantState participant,
                RunPopulation.Lease lease, Progress progress)
        {
            Root = current;
            Participant = participant;
            Lease = lease;
            Progress = progress;
        }

        internal FoundationSession.RunPublication Root { get; }
        internal FoundationParticipantState Participant { get; }
        internal RunPopulation.Lease Lease { get; }
        internal Progress Progress { get; }
    }
}
