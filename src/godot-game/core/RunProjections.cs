using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KingdomZombies.Core;

/// <summary>Builds detached public and private values at the current host authority boundary.</summary>
internal static class RunProjections
{
    internal static (string survivorId, long generation, long deathGeneration)? Control(
            FoundationSession.RunPublication? root, (string session, string binding, string player) identity)
    {
        if (root is null || !StringComparer.Ordinal.Equals(x: root.Snapshot.SessionId, y: identity.session)
                || !root.Snapshot.Participants.Any(predicate: participant => participant.Ready
                        && StringComparer.Ordinal.Equals(x: participant.BindingId, y: identity.binding)
                        && StringComparer.Ordinal.Equals(x: participant.Actor.PlayerId, y: identity.player)))
        {
            return null;
        }

        RunSuccession.View? view = RunSuccession.Read(current: root, binding: identity.binding);
        return (view?.SurvivorId ?? "", view?.Generation ?? 0, view?.DeathGeneration ?? 0);
    }

    internal static (FoundationSnapshot roster, JsonElement world)? Public(FoundationSession.RunPublication? root)
    {
        if (root is null)
        {
            return null;
        }

        RunWorld world = root.World;
        RunInventory.Item[] gear = [.. root.Inventory.Items.Where(predicate: item =>
                item.Location is RunInventory.Equipment or RunInventory.CorpseEquipment)];
        var data = new
        {
            Elapsed = root.ElapsedMilliseconds,
            OriginX = world.Origin.X,
            Ground = world.Origin.Y + world.GroundContactSubunits,
            CampLeft = world.CampBorders.left,
            CampRight = world.CampBorders.right,
            Actors = root.Population.Survivors.Where(predicate: actor => actor.Health > 0)
                    .Select(selector: actor => Actor(root: root, actor: actor, gear: gear)),
            Corpses = root.Loot.Deaths.Values.Where(predicate: death => root.Loot.Targets.ContainsKey(
                    key: death.CorpseId)).Select(selector: death => Corpse(root: root, death: death, gear: gear)),
            Defenses = world.Defenses.Select(selector: defense => new
            {
                defense.Id,
                defense.Contact.X,
                defense.Contact.Y,
                defense.Living,
                defense.Completed,
                defense.IsWall,
            }),
            Loot = root.Loot.Targets.Values.Where(predicate: target => world.Anchors.Contains(item: target.Anchor))
                    .Select(selector: target => new
                    {
                        target.Anchor.Id,
                        target.Anchor.Contact.X,
                        target.Anchor.Contact.Y,
                        Kind = target.Anchor.Kind.ToString(),
                    }),
            Arrows = root.Combat.Arrows.Select(selector: arrow =>
            {
                (decimal x, decimal y) = arrow.At(now: root.ElapsedMilliseconds);
                return new { arrow.Id, X = x, Y = y, VelocityX = arrow.Velocity.x, VelocityY = arrow.Velocity.y };
            }),
        };
        return (root.Snapshot, JsonSerializer.SerializeToElement(value: data));
    }

    internal static (long revision, ImmutableArray<(string scope, long generation, long token, JsonElement header,
            ImmutableArray<JsonElement> rows)> views)? Private(FoundationSession session,
                    FoundationSession.RunPublication? root, (string session, string binding, string player) identity,
                    long? ownGrid, long? externalGrid, bool includeItems)
    {
        if (root is null || Control(root: root, identity: identity) is not { } control)
        {
            return null;
        }

        RunSuccession.View? succession = RunSuccession.Read(current: root, binding: identity.binding);
        RunPopulation.Survivor? owner = root.Population.Survivors.FirstOrDefault(predicate: actor =>
                StringComparer.Ordinal.Equals(x: actor.Id, y: control.survivorId));
        JsonElement status = JsonSerializer.SerializeToElement(value: new
        {
            SurvivorId = control.survivorId,
            Generation = control.generation,
            DeathGeneration = control.deathGeneration,
            Status = succession?.Status.ToString(),
            OpensAt = succession?.OpensAt,
            DeathX = succession?.DeathContact?.X,
            DeathY = succession?.DeathContact?.Y,
            Health = owner?.Health,
            Stamina = owner?.Stamina,
            root.Definitions.MaximumHealth,
            root.Definitions.MaximumStamina,
            RootTime = root.ElapsedMilliseconds,
            Teammates = succession?.Teammates.Select(selector: teammate =>
                    new { teammate.BindingId, teammate.Slot, teammate.SurvivorId }),
        });
        if (control.generation == 0 || !includeItems)
        {
            return (root.Snapshot.Revision, [("status", control.generation, 0, status, [])]);
        }

        var controller = new RunItemActions.Controller(sessionId: identity.session, bindingId: identity.binding,
                generation: control.generation);
        RunItemActions.View? view = session.ItemActions.Read(controller: controller);
        if (view is null)
        {
            return null;
        }

        (JsonElement header, ImmutableArray<JsonElement> rows) own = Own(root: root, view: view, requested: ownGrid);
        (JsonElement header, ImmutableArray<JsonElement> rows)? external = External(root: root, view: view,
                requested: externalGrid);
        return (root.Snapshot.Revision, external is { } selected
                ? [("status", control.generation, 0, status, []),
                    ("own", control.generation, 0, own.header, own.rows),
                    ("external", control.generation, view.Selection?.Token ?? 0, selected.header, selected.rows)]
                : [("status", control.generation, 0, status, []),
                    ("own", control.generation, 0, own.header, own.rows)]);
    }

    internal static (string status, bool replayed) Dispatch(FoundationSession session,
            FoundationSession.RunPublication root, RunItemActions.Controller controller, long sequence,
            string kind, JsonElement request)
    {
        if (sequence <= 0)
        {
            return ("StaleRequest", false);
        }

        try
        {
            return kind switch
            {
                "move" => Motion(session: session, controller: controller, sequence: sequence,
                        input: Read<MotionData>(request: request)),
                "attack" => Attack(session: session, controller: controller, sequence: sequence,
                        input: Read<AttackData>(request: request)),
                "food" => Food(session: session, root: root, controller: controller, sequence: sequence,
                        input: Read<FoodData>(request: request)),
                _ => ItemAction(session: session, controller: controller, sequence: sequence,
                        input: ReadItemAction(root: root, kind: kind, request: request)),
            };
        }
        catch (Exception failure) when (failure is JsonException or InvalidDataException)
        {
            return ("InvalidAction", false);
        }
    }

    private static readonly JsonSerializerOptions _strict = new()
    {
        AllowDuplicateProperties = false,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    private static JsonElement Actor(FoundationSession.RunPublication root, RunPopulation.Survivor actor,
            RunInventory.Item[] gear)
    {
        (decimal x, decimal y) = RunCombat.Contact(world: root.World, survivor: actor);
        string? binding = root.Population.Leases.FirstOrDefault(predicate: lease =>
                StringComparer.Ordinal.Equals(x: lease.SurvivorId, y: actor.Id))?.BindingId;
        RunInventory.Item[] worn = [.. gear.Where(predicate: item => item.Location is RunInventory.Equipment slot
                && StringComparer.Ordinal.Equals(x: slot.SurvivorId, y: actor.Id))];
        RunInventory.Item? left = AtHand(items: worn, hand: "left-hand");
        RunInventory.Item? right = AtHand(items: worn, hand: "right-hand");
        bool striking = actor.StrikeAt.HasValue && actor.AttackAvailableAt > root.ElapsedMilliseconds;
        return JsonSerializer.SerializeToElement(value: new
        {
            actor.Id,
            X = x,
            Y = y,
            actor.Facing,
            actor.Hostile,
            BindingId = binding,
            Movement = actor.JumpStartedAt.HasValue ? "jump" : actor.MovementPose,
            StrikeAt = striking ? actor.StrikeAt : null,
            StrikeUntil = striking ? actor.AttackAvailableAt : null,
            AimX = striking ? actor.StrikeAim?.X : null,
            AimY = striking ? actor.StrikeAim?.Y : null,
            actor.FlinchAt,
            ArrowReady = left is { DefinitionId: "IT-07" } && right is { DefinitionId: "IT-02" }
                    || right is { DefinitionId: "IT-07" } && left is { DefinitionId: "IT-02" },
            Equipment = Equipment(items: worn),
        });
    }

    private static JsonElement Corpse(FoundationSession.RunPublication root, RunDeathAndLoot.Death death,
            RunInventory.Item[] gear)
    {
        RunPopulation.Survivor actor = root.Population.Survivors.First(predicate: survivor =>
                StringComparer.Ordinal.Equals(x: survivor.Id, y: death.SurvivorId));
        return JsonSerializer.SerializeToElement(value: new
        {
            Id = death.CorpseId,
            death.SurvivorId,
            death.Contact.X,
            death.Contact.Y,
            actor.Facing,
            actor.Hostile,
            death.At,
            Equipment = Equipment(items: gear.Where(predicate: item => item.Location is
                    RunInventory.CorpseEquipment slot && StringComparer.Ordinal.Equals(x: slot.CorpseId,
                            y: death.CorpseId))),
        });
    }

    private static RunInventory.Item? AtHand(RunInventory.Item[] items, string hand)
    {
        return items.FirstOrDefault(predicate: item => item.Location is RunInventory.Equipment slot
                && StringComparer.Ordinal.Equals(x: slot.Slot, y: hand));
    }

    private static JsonElement Equipment(IEnumerable<RunInventory.Item> items)
    {
        return JsonSerializer.SerializeToElement(value: items.Select(selector: item => new
        {
            Slot = item.Location switch
            {
                RunInventory.Equipment worn => worn.Slot,
                RunInventory.CorpseEquipment body => body.Slot,
                _ => throw new InvalidDataException(message: "RUN_VISIBLE_EQUIPMENT"),
            },
            item.DefinitionId,
            Broken = item.Condition == 0,
        }));
    }

    private static (JsonElement header, ImmutableArray<JsonElement> rows) Own(
            FoundationSession.RunPublication root, RunItemActions.View view, long? requested)
    {
        RunInventory.Item? container = Container(items: view.Personal, requested: requested);
        RunDefinitions.Size? bounds = container?.StorageGrid;
        var info = new
        {
            view.SurvivorId,
            ContainerId = container?.Id,
            Width = bounds?.Width,
            Height = bounds?.Height,
            Nearby = view.Nearby.Select(selector: target => new { Kind = target.Kind.ToString(), target.Id }),
            Quick = view.QuickReferences.Select(selector: row => new
            {
                Slot = row.slot,
                ItemId = row.itemId,
                Stamp = root.Inventory.PathTo(itemId: row.itemId) is { } path
                        && path.IsCarriedBy(survivorId: view.SurvivorId) ? Stamp(path: path.Items) : null,
            }),
            Availability = view.Availability.ToString(),
        };
        return (JsonSerializer.SerializeToElement(value: info), Rows(root: root, items: view.Personal,
                scope: item => container is null ? item.Location is RunInventory.Equipment
                        : item.Location is RunInventory.ContainerCell cell && cell.ContainerId == container.Id));
    }

    private static (JsonElement header, ImmutableArray<JsonElement> rows)? External(
            FoundationSession.RunPublication root, RunItemActions.View view, long? requested)
    {
        if (view.Selection is not { } selection)
        {
            return null;
        }

        RunInventory.Item? container = Container(items: view.External, requested: requested);
        RunDefinitions.Size? bounds = container?.StorageGrid
                ?? (selection.Target.Kind == RunWorld.Access.Stash ? root.Inventory.StashGrid : null);
        var info = new
        {
            Kind = selection.Target.Kind.ToString(),
            TargetId = selection.Target.Id,
            Page = selection.Target.Kind == RunWorld.Access.Stash ? selection.PageIndex + 1 : (int?)null,
            PageCount = view.StashPageCount,
            ContainerId = container?.Id,
            Width = bounds?.Width,
            Height = bounds?.Height,
        };
        return (JsonSerializer.SerializeToElement(value: info), Rows(root: root, items: view.External,
                scope: item => container is null ? RunExternalAccess.Contains(selection: selection,
                        location: item.Location)
                        : item.Location is RunInventory.ContainerCell cell && cell.ContainerId == container.Id));
    }

    private static RunInventory.Item? Container(ImmutableArray<RunInventory.Item> items, long? requested)
    {
        return requested is { } id ? items.FirstOrDefault(predicate: item => item.Id == id
                && item.StorageGrid.HasValue) : null;
    }

    private static ImmutableArray<JsonElement> Rows(FoundationSession.RunPublication root,
            ImmutableArray<RunInventory.Item> items, Func<RunInventory.Item, bool> scope)
    {
        var lookup = items.ToDictionary(keySelector: item => item.Id);
        return [.. items.Where(predicate: scope).OrderBy(keySelector: item => item.Id)
                .Select(selector: item => PrivateItem(root: root, item: item, lookup: lookup))];
    }

    private static JsonElement PrivateItem(FoundationSession.RunPublication root, RunInventory.Item item,
            Dictionary<long, RunInventory.Item> lookup)
    {
        RunDefinitions.Item rule = root.Definitions.Items[item.DefinitionId];
        bool legacy = root.Inventory.SavedLayout?.Items.TryGetValue(key: item.Id,
                value: out (RunDefinitions.Item definition, bool legacy) original) == true && original.legacy;
        RunDefinitions.Item display = root.Inventory.SavedLayout?.RuleFor(item: item, current: rule) ?? rule;
        return JsonSerializer.SerializeToElement(value: new
        {
            item.Id,
            item.DefinitionId,
            item.Quantity,
            item.Condition,
            StorageWidth = item.StorageGrid?.Width,
            StorageHeight = item.StorageGrid?.Height,
            FootprintWidth = display.Footprint.Width,
            FootprintHeight = display.Footprint.Height,
            display.Appearance.CanonicalIcon,
            display.Rotatable,
            display.MaxStack,
            display.MaximumCondition,
            Legacy = legacy,
            Location = Location(item: item),
            Stamp = Stamp(path: VisiblePath(item: item, lookup: lookup)),
        });
    }

    private static ImmutableArray<RunInventory.Item> VisiblePath(RunInventory.Item item,
            Dictionary<long, RunInventory.Item> lookup)
    {
        ImmutableArray<RunInventory.Item>.Builder path = ImmutableArray.CreateBuilder<RunInventory.Item>();
        path.Add(item: item);
        while (item.Location is RunInventory.ContainerCell cell
                && lookup.TryGetValue(key: cell.ContainerId, value: out RunInventory.Item? parent))
        {
            path.Add(item: parent);
            item = parent;
        }

        return path.ToImmutable();
    }

    private static JsonElement Location(RunInventory.Item item)
    {
        return item.Location switch
        {
            RunInventory.Equipment slot => JsonSerializer.SerializeToElement(value:
                    new { Kind = "equipment", TargetId = slot.SurvivorId, slot.Slot }),
            RunInventory.CorpseEquipment slot => JsonSerializer.SerializeToElement(value:
                    new { Kind = "corpse", TargetId = slot.CorpseId, slot.Slot }),
            RunInventory.Ground pile => JsonSerializer.SerializeToElement(value:
                    new { Kind = "ground", TargetId = pile.PileId }),
            RunInventory.GridCell cell => JsonSerializer.SerializeToElement(value: new
            {
                Kind = cell is RunInventory.StashCell ? "stash" : "container",
                GridId = cell.Grid.id,
                cell.X,
                cell.Y,
                cell.Rotated,
                Width = cell.Footprint.Width,
                Height = cell.Footprint.Height,
            }),
            _ => throw new InvalidDataException(message: "RUN_VIEW_LOCATION"),
        };
    }

    private static string Stamp(ImmutableArray<RunInventory.Item> path)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value: path.Select(selector: item => new
        {
            item.Id,
            item.DefinitionId,
            item.Quantity,
            item.Condition,
            StorageWidth = item.StorageGrid?.Width,
            StorageHeight = item.StorageGrid?.Height,
            Location = Location(item: item),
        }));
        return Convert.ToHexString(inArray: SHA256.HashData(source: bytes));
    }

    private static (string status, bool replayed) Motion(FoundationSession session,
            RunItemActions.Controller controller, long sequence, MotionData input)
    {
        if (input.Direction is < -1 or > 1)
        {
            return ("InvalidAction", false);
        }

        var movement = new RunLocomotion.Input(left: input.Direction < 0, right: input.Direction > 0,
                sprint: input.Sprint, jump: input.Jump);
        (bool accepted, RunLocomotion.Block reason) = session.TryAcceptMotion(controller: controller,
                sequence: sequence, input: movement);
        return (accepted ? (reason == RunLocomotion.Block.None ? "Accepted" : reason.ToString()) : "StaleRequest",
                false);
    }

    private static (string status, bool replayed) Attack(FoundationSession session,
            RunItemActions.Controller controller, long sequence, AttackData input)
    {
        if (input.AimX.HasValue != input.AimY.HasValue)
        {
            return ("InvalidAction", false);
        }

        RunCombat.Aim? aim = input.AimX is { } x && input.AimY is { } y ? new RunCombat.Aim(x: x, y: y) : null;
        RunCombat.Outcome outcome = session.Attack(controller: controller, sequence: sequence, aim: aim);
        return (outcome.Status.ToString(), outcome.Replayed);
    }

    private static (string status, bool replayed) Food(FoundationSession session,
            FoundationSession.RunPublication root, RunItemActions.Controller controller, long sequence, FoodData input)
    {
        var request = new RunFood.Request(expected: Expected(inventory: root.Inventory, item: input.Item),
                quickSlot: input.QuickSlot);
        RunFood.Result outcome = session.ConsumeFood(controller: controller, sequence: sequence, request: request);
        return (outcome.Status.ToString(), outcome.Replayed);
    }

    private static (string status, bool replayed) ItemAction(FoundationSession session,
            RunItemActions.Controller controller, long sequence, RunItemActions.Request input)
    {
        RunItemActions.Outcome outcome = session.ItemActions.Submit(controller: controller, sequence: sequence,
                request: input);
        return (outcome.Status.ToString(), outcome.Replayed);
    }

    private static RunItemActions.Request ReadItemAction(FoundationSession.RunPublication root, string kind,
            JsonElement request)
    {
        return kind switch
        {
            "select" => Select(input: Read<SelectData>(request: request)),
            "browse" => Browse(input: Read<BrowseData>(request: request)),
            "close" => new RunItemActions.Close(token: Read<CloseData>(request: request).Token),
            "drop" => Drop(root: root, input: Read<DropData>(request: request)),
            "quick" => Quick(root: root, input: Read<QuickData>(request: request)),
            "place" => Place(root: root, input: Read<PlaceData>(request: request)),
            "merge" => Merge(root: root, input: Read<MergeData>(request: request)),
            _ => throw new InvalidDataException(message: "RUN_ACTION_KIND"),
        };
    }

    private static RunItemActions.Select Select(SelectData input)
    {
        if (input.TargetId.Length is < 1 or > 128 || !Enum.TryParse(value: input.TargetKind, ignoreCase: false,
                result: out RunWorld.Access kind) || !Enum.IsDefined(value: kind)
                || !StringComparer.Ordinal.Equals(x: kind.ToString(), y: input.TargetKind))
        {
            throw new InvalidDataException(message: "RUN_ACTION_TARGET");
        }

        return new RunItemActions.Select(target: new RunExternalAccess.Target(kind: kind, id: input.TargetId));
    }

    private static RunItemActions.Browse Browse(BrowseData input)
    {
        return new RunItemActions.Browse(token: input.Token, pageNumber: input.Page);
    }

    private static RunItemActions.Drop Drop(FoundationSession.RunPublication root, DropData input)
    {
        if (input.TargetId is { Length: < 1 or > 128 })
        {
            throw new InvalidDataException(message: "RUN_ACTION_TARGET");
        }

        return new RunItemActions.Drop(expected: Expected(inventory: root.Inventory, item: input.Item),
                pileId: input.TargetId);
    }

    private static RunItemActions.QuickTransfer Quick(FoundationSession.RunPublication root, QuickData input)
    {
        return new RunItemActions.QuickTransfer(expected: Expected(inventory: root.Inventory, item: input.Item),
                token: input.Token, direction: input.Deposit ? RunItemActions.QuickDirection.Deposit
                        : RunItemActions.QuickDirection.Withdraw);
    }

    private static RunItemActions.Place Place(FoundationSession.RunPublication root, PlaceData input)
    {
        RunInventory.ItemPath expected = Expected(inventory: root.Inventory, item: input.Item);
        RunInventory.Location destination = Destination(root: root, expected: expected, input: input.Destination);
        RunInventory.ItemPath? container = input.Container is { } other
                ? Expected(inventory: root.Inventory, item: other) : null;
        return new RunItemActions.Place(expected: expected, token: input.Token, destination: destination,
                amount: input.Amount, container: container);
    }

    private static RunItemActions.Merge Merge(FoundationSession.RunPublication root, MergeData input)
    {
        return new RunItemActions.Merge(expected: Expected(inventory: root.Inventory, item: input.Item),
                token: input.Token, target: Expected(inventory: root.Inventory, item: input.Target));
    }

    private static RunInventory.Location Destination(FoundationSession.RunPublication root,
            RunInventory.ItemPath expected, DestinationData input)
    {
        if (input.Kind is "equipment" or "corpse" && input.TargetId is { Length: >= 1 and <= 128 } target
                && input.Slot is { Length: >= 1 and <= 32 } slot && input.GridId is null && input.X is null
                && input.Y is null && input.Rotated is null)
        {
            return input.Kind is "equipment" ? new RunInventory.Equipment(survivorId: target, slot: slot)
                    : new RunInventory.CorpseEquipment(corpseId: target, slot: slot);
        }

        if (input.TargetId is not null || input.Slot is not null || input.GridId is not { } id
                || input.X is not { } x || input.Y is not { } y || input.Rotated is not { } rotated)
        {
            throw new InvalidDataException(message: "RUN_ACTION_DESTINATION");
        }

        RunDefinitions.Size size = expected.Items.IsEmpty ? new RunDefinitions.Size(width: 1, height: 1)
                : RunInventoryRules.Footprint(definition: root.Definitions.Items[expected.Item.DefinitionId],
                        rotated: rotated);
        return input.Kind switch
        {
            "stash" when id is >= 0 and <= int.MaxValue => new RunInventory.StashCell(page: (int)id, x: x, y: y,
                    rotated: rotated, footprint: size),
            "container" when id > 0 => new RunInventory.ContainerCell(containerId: id, x: x, y: y,
                    rotated: rotated, footprint: size),
            _ => throw new InvalidDataException(message: "RUN_ACTION_DESTINATION"),
        };
    }

    // A deliberately stale expectation lets the ordinary resolver check the current scope first.
    private static RunInventory.ItemPath Expected(RunInventory inventory, ItemData item)
    {
        if (item.Id <= 0 || item.Stamp.Length != 64)
        {
            throw new InvalidDataException(message: "RUN_ACTION_ITEM");
        }

        RunInventory.ItemPath? current = inventory.PathTo(itemId: item.Id);
        if (current is null)
        {
            return new RunInventory.ItemPath(items: []);
        }

        if (StringComparer.Ordinal.Equals(x: item.Stamp, y: Stamp(path: current.Items)))
        {
            return current;
        }

        return new RunInventory.ItemPath(items: current.Items.SetItem(index: 0, item: current.Item with
        {
            Quantity = 0,
        }));
    }

    private static T Read<T>(JsonElement request)
            where T : class
    {
        if (request.ValueKind != JsonValueKind.Object)
        {
            throw new InvalidDataException(message: "RUN_ACTION_OBJECT");
        }

        return request.Deserialize<T>(options: _strict)
                ?? throw new InvalidDataException(message: "RUN_ACTION_OBJECT");
    }

    private sealed class MotionData
    {
        public required int Direction { get; init; }
        public required bool Sprint { get; init; }
        public required bool Jump { get; init; }
    }

    private sealed class AttackData
    {
        public required double? AimX { get; init; }
        public required double? AimY { get; init; }
    }

    private sealed class ItemData
    {
        public required long Id { get; init; }
        public required string Stamp { get; init; }
    }

    private sealed class FoodData
    {
        public required ItemData Item { get; init; }
        public required int? QuickSlot { get; init; }
    }

    private sealed class SelectData
    {
        public required string TargetKind { get; init; }
        public required string TargetId { get; init; }
    }

    private sealed class BrowseData
    {
        public required long Token { get; init; }
        public required int Page { get; init; }
    }

    private sealed class CloseData
    {
        public required long Token { get; init; }
    }

    private sealed class DropData
    {
        public required ItemData Item { get; init; }
        public required string? TargetId { get; init; }
    }

    private sealed class QuickData
    {
        public required ItemData Item { get; init; }
        public required long Token { get; init; }
        public required bool Deposit { get; init; }
    }

    private sealed class PlaceData
    {
        public required ItemData Item { get; init; }
        public required long? Token { get; init; }
        public required DestinationData Destination { get; init; }
        public required ItemData? Container { get; init; }
        public required decimal? Amount { get; init; }
    }

    private sealed class MergeData
    {
        public required ItemData Item { get; init; }
        public required long? Token { get; init; }
        public required ItemData Target { get; init; }
    }

    private sealed class DestinationData
    {
        public required string Kind { get; init; }
        public required string? TargetId { get; init; }
        public required string? Slot { get; init; }
        public required long? GridId { get; init; }
        public required int? X { get; init; }
        public required int? Y { get; init; }
        public required bool? Rotated { get; init; }
    }
}
