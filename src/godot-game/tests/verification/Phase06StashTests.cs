using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the production stash's placement, navigation and transfer outcomes.</summary>
[Trait("Tier", "1")]
public sealed class Phase06StashTests
{
    /// <summary>Fresh setup chooses an earlier page that can fit the item legally rotated.</summary>
    [Fact]
    public void Phase06_E070_StartupUsesTheEarliestFittingOrientation()
    {
        FoundationSession session = Start(stacks:
                [Stack(id: "IT-01", quantity: 4, count: 9), Stack(id: "IT-07"),
                    Stack(id: "IT-04")]);
        RunInventory.Item sword = session.Inventory.Items.Single(predicate: item => item.DefinitionId is "IT-04");
        var cell = (RunInventory.StashCell)sword.Location;
        (cell.Page, cell.X, cell.Y, cell.Rotated).ShouldBe(expected: (0, 1, 2, true));
        RunInventory.Item[] stacks = [.. session.Inventory.Items.Where(predicate: item => item.DefinitionId
                is "IT-01")];
        stacks.Select(selector: item => item.Id).ShouldBe(expected: Enumerable.Range(start: 25, count: 9)
                .Select(selector: id => (long)id));
        stacks.All(predicate: item => item.Quantity == 4).ShouldBeTrue();
        session.Inventory.StashPageCount.ShouldBe(expected: 2);
        RunInventoryRules.IsValid(inventory: session.Inventory, definitions: session.Definitions,
                population: session.Population).ShouldBeTrue();
    }

    /// <summary>Depositing into a full stash publishes one new page and the exact affected cells.</summary>
    [Fact]
    public void Phase06_E071_FullExistingPagesCreateOnePageOnCommit()
    {
        FoundationSession session = Start(stacks: [Stack(id: "IT-01", quantity: 100, count: 64)]);
        RunInventory.ItemPath source = Seed(session: session, id: "IT-01", quantity: 7,
                location: Pocket(session: session, id: "IT-01"));
        Open(session: session).StashPageCount.ShouldBe(expected: 4);
        long revision = session.Snapshot.Revision;
        RunItemActions.Outcome result = Quick(session: session, source: source, sequence: 2);
        result.Status.ShouldBe(expected: RunItemActions.Status.Applied);
        RunItemActions.View corrected = result.View.ShouldNotBeNull();
        corrected.StashPageCount.ShouldBe(expected: 5);
        corrected.StashPageNumber.ShouldBe(expected: 5);
        corrected.Revision.ShouldBe(expected: revision + 1);
        corrected.External.Single().Quantity.ShouldBe(expected: 7);
        corrected.External.Single().Id.ShouldBe(expected: source.Item.Id);
        RunStash.PageEffect effect = result.StashEffect.ShouldNotBeNull();
        effect.PageNumber.ShouldBe(expected: 5);
        effect.ChangedCells.AsEnumerable().ShouldBe(expected: [(0, 0)]);
        effect.MorePages.ShouldBe(expected: 0);
        session.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 0);
    }

    /// <summary>Two authorized depositors reuse the same new page without overlaps or copies.</summary>
    [Fact]
    public async Task Phase06_E072_CompetingDepositsShareOneNewPage()
    {
        FoundationSession session = Start(stacks: [Stack(id: "IT-01", quantity: 100, count: 16)]);
        session.AddParticipant(bindingId: "guest", playerId: "guest-player", actorId: "guest-actor", label: "Guest")
                .ShouldBeTrue();
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        RunInventory.ItemPath first = Seed(session: session, id: "IT-01", quantity: 100,
                location: Pocket(session: session, id: "IT-01"));
        RunInventory.ItemPath second = Seed(session: session, id: "IT-01", quantity: 100,
                location: Pocket(session: session, id: "IT-01", owner: "survivor-02"));
        Open(session: session);
        Open(session: session, binding: "guest");
        long revision = session.Snapshot.Revision;
        Task<RunItemActions.Outcome> host = Task.Run(function: () => Quick(session: session, source: first,
                sequence: 2));
        Task<RunItemActions.Outcome> guest = Task.Run(function: () => Quick(session: session, source: second,
                sequence: 2, binding: "guest"));
        RunItemActions.Outcome[] results = await Task.WhenAll(tasks: [host, guest]);
        results.All(predicate: result => result.Status == RunItemActions.Status.Applied).ShouldBeTrue();
        session.Snapshot.Revision.ShouldBe(expected: revision + 2);
        session.Inventory.StashPageCount.ShouldBe(expected: 2);
        RunInventory.Item[] deposits = [.. session.Inventory.Items.Where(predicate: item =>
                item.Location is RunInventory.StashCell { Page: 1 }).OrderBy(keySelector: item =>
                        ((RunInventory.StashCell)item.Location).X)];
        deposits.Select(selector: item => ((RunInventory.StashCell)item.Location).X).ShouldBe(expected: [0, 1]);
        deposits.Select(selector: item => item.Id).Order().ShouldBe(expected: [first.Item.Id, second.Item.Id]);
        deposits.Sum(selector: item => item.Quantity).ShouldBe(expected: 200);
    }

    /// <summary>An empty stash has one usable page and browsing cannot allocate an absent page.</summary>
    [Fact]
    public void Phase06_E073_EmptyStashAndMissingPagesAreDistinct()
    {
        FoundationSession session = Start(stacks: []);
        RunItemActions.View empty = Open(session: session);
        empty.StashPageCount.ShouldBe(expected: 1);
        empty.StashPageNumber.ShouldBe(expected: 1);
        empty.External.ShouldBeEmpty();
        FoundationSnapshot snapshot = session.Snapshot;
        RunItemActions.Outcome absent = Submit(session: session, sequence: 2,
                request: new RunItemActions.Browse(token: 1, pageNumber: 2));
        absent.Status.ShouldBe(expected: RunItemActions.Status.InvalidPage);
        absent.View.ShouldNotBeNull().StashPageNumber.ShouldBe(expected: 1);
        session.Snapshot.ShouldBeSameAs(expected: snapshot);
        session.Inventory.StashPageCount.ShouldBe(expected: 1);
    }

    /// <summary>Quick withdrawal uses real storage and retries cannot spend or refill a second stack.</summary>
    [Fact]
    public void Phase06_E075_QuickWithdrawalUsesPocketsAndCannotReplay()
    {
        FoundationSession session = Start(stacks: [Stack(id: "IT-02", quantity: 5, count: 2)]);
        RunInventory.ItemPath hand = Seed(session: session, id: "IT-02", quantity: 1,
                location: new RunInventory.Equipment(survivorId: "survivor-01", slot: "left-hand"));
        RunInventory.ItemPath carried = Seed(session: session, id: "IT-02", quantity: 27,
                location: Pocket(session: session, id: "IT-02", slot: "legs"));
        RunItemActions.View stash = Open(session: session);
        RunInventory.ItemPath first = Path(session: session, id: stash.External[0].Id);
        RunInventory.ItemPath second = Path(session: session, id: stash.External[1].Id);
        RunItemActions.Outcome withdrawal = Quick(session: session, source: first, sequence: 2,
                direction: RunItemActions.QuickDirection.Withdraw);
        withdrawal.Status.ShouldBe(expected: RunItemActions.Status.Applied);
        RunInventory.Item moved = session.Inventory.Find(itemId: first.Item.Id).ShouldNotBeNull();
        moved.Quantity.ShouldBe(expected: 2);
        moved.Location.ShouldBe(expected: Pocket(session: session, id: "IT-02", rotated: true));
        session.Inventory.Find(itemId: carried.Item.Id)?.Quantity.ShouldBe(expected: 30);
        session.Inventory.Find(itemId: hand.Item.Id).ShouldBeSameAs(expected: hand.Item);
        FoundationSnapshot snapshot = session.Snapshot;
        Quick(session: session, source: second, sequence: 2, direction: RunItemActions.QuickDirection.Withdraw)
                .Replayed.ShouldBeTrue();
        session.Snapshot.ShouldBeSameAs(expected: snapshot);
        session.Inventory.Find(itemId: second.Item.Id).ShouldBeSameAs(expected: second.Item);
    }

    /// <summary>Stack-first placement uses stable page order and reports every affected page.</summary>
    [Fact]
    public void Phase06_E076_CompatibleStacksWinBeforeFreeCellsAndUseStablePageOrder()
    {
        FoundationSession session = Start(stacks: []);
        RunInventory.ItemPath second = Seed(session: session, id: "IT-01", quantity: 98,
                location: Stash(session: session, id: "IT-01", page: 1), pageCount: 2);
        RunInventory.ItemPath first = Seed(session: session, id: "IT-01", quantity: 99,
                location: Stash(session: session, id: "IT-01", x: 1));
        RunInventory.ItemPath source = Seed(session: session, id: "IT-01", quantity: 2,
                location: Pocket(session: session, id: "IT-01"));
        Open(session: session);
        RunItemActions.Outcome browse = Submit(session: session, sequence: 2,
                request: new RunItemActions.Browse(token: 1, pageNumber: 2));
        browse.View.ShouldNotBeNull().StashPageNumber.ShouldBe(expected: 2);
        RunItemActions.Outcome result = Quick(session: session, source: source, sequence: 3, token: 2);
        result.Status.ShouldBe(expected: RunItemActions.Status.Applied);
        result.View.ShouldNotBeNull().StashPageNumber.ShouldBe(expected: 1);
        session.Inventory.Find(itemId: source.Item.Id).ShouldBeNull();
        session.Inventory.Find(itemId: first.Item.Id)?.Quantity.ShouldBe(expected: 100);
        session.Inventory.Find(itemId: second.Item.Id)?.Quantity.ShouldBe(expected: 99);
        RunStash.PageEffect effect = result.StashEffect.ShouldNotBeNull();
        effect.PageNumber.ShouldBe(expected: 1);
        effect.ChangedCells.AsEnumerable().ShouldBe(expected: [(1, 0)]);
        effect.MorePages.ShouldBe(expected: 1);
        session.Inventory.StashPageCount.ShouldBe(expected: 2);
    }

    /// <summary>Integral page and quantity bounds are checked before any split or allocation.</summary>
    [Fact]
    public void Phase06_E077_PageAndQuantityBoundsDoNotWrapOrTruncate()
    {
        FoundationSession session = Start(stacks: []);
        RunInventory.ItemPath source = Seed(session: session, id: "IT-01", quantity: 3,
                location: Pocket(session: session, id: "IT-01"), pageCount: int.MaxValue);
        Open(session: session);
        FoundationSnapshot before = session.Snapshot;
        long sequence = 1;
        foreach (decimal badPage in new[] { -1m, 0m, 1.5m, int.MaxValue + 1m, decimal.MaxValue })
        {
            Submit(session: session, sequence: ++sequence, request: new RunItemActions.Browse(token: 1,
                    pageNumber: badPage)).Status.ShouldBe(expected: RunItemActions.Status.InvalidPage);
        }

        RunItemActions.View maximum = Submit(session: session, sequence: ++sequence,
                request: new RunItemActions.Browse(token: 1, pageNumber: int.MaxValue)).View.ShouldNotBeNull();
        maximum.StashPageNumber.ShouldBe(expected: int.MaxValue);
        maximum.External.ShouldBeEmpty();
        long token = sequence;
        RunInventory.StashCell destination = Stash(session: session, id: "IT-01", page: int.MaxValue - 1);
        foreach (decimal badAmount in new[] { -1m, 0m, 1.5m, 3m, decimal.MaxValue })
        {
            Submit(session: session, sequence: ++sequence, request: new RunItemActions.Place(expected: source,
                    destination: destination, token: token, amount: badAmount)).Status
                    .ShouldBe(expected: RunItemActions.Status.Incompatible);
        }

        session.Snapshot.ShouldBeSameAs(expected: before);
        Submit(session: session, sequence: ++sequence, request: new RunItemActions.Place(expected: source,
                destination: destination, token: token, amount: 1)).Status
                .ShouldBe(expected: RunItemActions.Status.Applied);
        session.Inventory.StashPageCount.ShouldBe(expected: int.MaxValue);
        session.Inventory.Find(itemId: source.Item.Id)?.Quantity.ShouldBe(expected: 2);
        session.Inventory.Items.Single(predicate: item => item.Location == destination).Quantity
                .ShouldBe(expected: 1);
    }

    /// <summary>A partial merge remains detached when the remainder cannot fit any carried grid.</summary>
    [Fact]
    public void QuickWithdrawal_NoRoom_RejectsWithoutPartialMerge()
    {
        FoundationSession session = Start(stacks: [Stack(id: "IT-02", quantity: 5)]);
        RunInventory.ItemPath arrows = Seed(session: session, id: "IT-02", quantity: 29,
                location: Pocket(session: session, id: "IT-02", rotated: true));
        _ = Seed(session: session, id: "IT-18", quantity: 1,
                location: Pocket(session: session, id: "IT-18", slot: "legs"));
        RunInventory.ItemPath source = Path(session: session, id: Open(session: session).External.Single().Id);
        FoundationSnapshot snapshot = session.Snapshot;
        RunItemActions.Outcome rejected = Quick(session: session, source: source, sequence: 2,
                direction: RunItemActions.QuickDirection.Withdraw);
        rejected.Status.ShouldBe(expected: RunItemActions.Status.Capacity);
        session.Snapshot.ShouldBeSameAs(expected: snapshot);
        session.Inventory.Find(itemId: arrows.Item.Id).ShouldBeSameAs(expected: arrows.Item);
        session.Inventory.Find(itemId: source.Item.Id).ShouldBeSameAs(expected: source.Item);
        session.Inventory.Equipped(survivorId: "survivor-01", slot: "right-hand").ShouldBeNull();
    }

    /// <summary>A contested last stash item appears only in the winner's personal inventory.</summary>
    [Fact]
    public async Task QuickWithdrawal_ContestedLastStashItem_PublishesOneOwner()
    {
        FoundationSession session = Start(stacks: [Stack(id: "IT-01", quantity: 11)]);
        session.AddParticipant(bindingId: "guest", playerId: "guest-player", actorId: "guest-actor", label: "Guest")
                .ShouldBeTrue();
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        RunInventory.ItemPath source = Path(session: session, id: Open(session: session).External.Single().Id);
        Open(session: session, binding: "guest");
        long revision = session.Snapshot.Revision;
        Task<RunItemActions.Outcome> host = Task.Run(function: () => Quick(session: session, source: source,
                sequence: 2, direction: RunItemActions.QuickDirection.Withdraw));
        Task<RunItemActions.Outcome> guest = Task.Run(function: () => Quick(session: session, source: source,
                sequence: 2, binding: "guest", direction: RunItemActions.QuickDirection.Withdraw));
        RunItemActions.Outcome[] results = await Task.WhenAll(tasks: [host, guest]);
        RunItemActions.View winner = results.Single(predicate: result => result.Status == RunItemActions.Status.Applied)
                .View.ShouldNotBeNull();
        RunItemActions.View loser = results.Single(predicate: result => result.Status
                == RunItemActions.Status.StaleItem).View.ShouldNotBeNull();
        winner.Personal.Single(predicate: item => item.Id == source.Item.Id).Quantity.ShouldBe(expected: 11);
        loser.Personal.Any(predicate: item => item.Id == source.Item.Id).ShouldBeFalse();
        winner.External.ShouldBeEmpty();
        loser.External.ShouldBeEmpty();
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
    }

    private static FoundationSession Start(ImmutableArray<RunDefinitions.StartingStack> stacks)
    {
        RunDefinitions baseline = RunDefinitions.Default;
        var settings = new RunDefinitions.InventorySettings(stashGrid: new RunDefinitions.Size(width: 4, height: 4),
                maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4);
        var start = new RunDefinitions.Start(survivorCount: 8, playerLimit: 4, role: "unassigned",
                outfit: baseline.StartingOutfit, stash: stacks);
        var survivors = new RunDefinitions.SurvivorSettings(maximumHealth: 100, maximumStamina: 100,
                refugeUnits: 12, dangerUnits: 12);
        var definitions = new RunDefinitions(start: start, survivors: survivors, inventory: settings,
                items: baseline.Items);
        var world = new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: RunWorld.DefaultRanges, defenses: [], blockers: [], hostiles: [],
                anchors: [new RunWorld.Anchor(id: RunWorld.StashId, contact: new RunWorld.Point(x: -4000, y: 0),
                        kind: RunWorld.Access.Stash)]);
        return new FoundationSession(sessionId: "stash-session", playerId: "host", actorId: "actor",
                world: world, definitions: definitions);
    }

    private static RunItemActions.View Open(FoundationSession session, string binding = "local")
    {
        var target = new RunExternalAccess.Target(kind: RunWorld.Access.Stash, id: RunWorld.StashId);
        RunItemActions.Outcome result = Submit(session: session, sequence: 1,
                request: new RunItemActions.Select(target: target), binding: binding);
        result.Status.ShouldBe(expected: RunItemActions.Status.Ready);
        return result.View.ShouldNotBeNull();
    }

    private static RunItemActions.Outcome Submit(FoundationSession session, long sequence,
            RunItemActions.Request request, string binding = "local")
    {
        RunPopulation.Lease lease = session.Population.FindLease(bindingId: binding).ShouldNotBeNull();
        var controller = new RunItemActions.Controller(sessionId: lease.SessionId, bindingId: binding,
                generation: lease.Generation);
        return session.ItemActions.Submit(controller: controller, sequence: sequence, request: request);
    }

    private static RunItemActions.Outcome Quick(FoundationSession session, RunInventory.ItemPath source,
            long sequence, long token = 1, string binding = "local",
            RunItemActions.QuickDirection direction = RunItemActions.QuickDirection.Deposit)
    {
        return Submit(session: session, sequence: sequence, request: new RunItemActions.QuickTransfer(
                expected: source, token: token, direction: direction), binding: binding);
    }

    private static RunInventory.ItemPath Seed(FoundationSession session, string id, int quantity,
            RunInventory.Location location, int? pageCount = null)
    {
        long itemId = session.Inventory.NextItemId;
        RunDefinitions.Item rule = session.Definitions.Items[id];
        var item = new RunInventory.Item(id: itemId, definitionId: id, quantity: quantity,
                condition: rule.MaximumCondition, location: location, storageGrid: rule.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => new RunInventory(items: inventory.Items.Add(item: item),
                stashGrid: inventory.StashGrid, stashPageCount: pageCount ?? inventory.StashPageCount,
                nextItemId: itemId + 1, quickReferences: inventory.QuickReferences)).ShouldBeTrue();
        return Path(session: session, id: itemId);
    }

    private static RunInventory.ItemPath Path(FoundationSession session, long id)
    {
        return session.Inventory.PathTo(itemId: id).ShouldNotBeNull();
    }

    private static RunInventory.ContainerCell Pocket(FoundationSession session, string id,
            string owner = "survivor-01", string slot = "torso-clothing", bool rotated = false)
    {
        RunInventory.Item container = session.Inventory.Equipped(survivorId: owner, slot: slot).ShouldNotBeNull();
        return RunItemTransfer.Container(definition: session.Definitions.Items[id], containerId: container.Id,
                x: 0, y: 0, rotated: rotated);
    }

    private static RunInventory.StashCell Stash(FoundationSession session, string id, int page = 0, int x = 0)
    {
        return RunItemTransfer.Stash(definition: session.Definitions.Items[id], page: page, x: x, y: 0);
    }

    private static RunDefinitions.StartingStack Stack(string id, int quantity = 1, int count = 1)
    {
        return new RunDefinitions.StartingStack(itemId: id, quantity: quantity, count: count);
    }
}
