using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves exact placement and stack transactions in the production run.</summary>
[Trait("Tier", "1")]
public sealed class Phase06InventoryTests
{
    /// <summary>Identical adjacent stacks and outfits each own a distinct identity.</summary>
    [Fact]
    public void Phase06_E029_AdjacentInstancesHaveIndependentLocations()
    {
        FoundationSession session = Start();
        RunInventory.Item first = Item(session: session, definitionId: "IT-01");
        RunInventory.Item second = Item(session: session, definitionId: "IT-01", ordinal: 1);
        var left = (RunInventory.StashCell)first.Location;
        var right = (RunInventory.StashCell)second.Location;
        (left.Page, left.X + 1, left.Y).ShouldBe(expected: (right.Page, right.X, right.Y));
        (first.Id != second.Id).ShouldBeTrue();
        long[] shirts = [.. session.Population.Survivors.Select(selector: survivor =>
                session.Inventory.Equipped(survivorId: survivor.Id, slot: "torso-clothing")?.Id ?? 0)];
        shirts.ShouldBe(expected: [1L, 4L, 7L, 10L, 13L, 16L, 19L, 22L]);
        session.Inventory.StashGrid.ShouldBe(expected: new RunDefinitions.Size(width: 10, height: 10));
    }

    /// <summary>All 24 authored configurations resolve and contradictory or oversized rows reject.</summary>
    [Fact]
    public void Phase06_E030_AllAuthoredDefinitionsRejectInvalidExtrema()
    {
        FoundationSession session = Start();
        session.Definitions.Items.Keys.Order(comparer: StringComparer.Ordinal).ShouldBe(expected:
                Enumerable.Range(start: 1, count: 24).Select(selector: number =>
                        FormattableString.Invariant(formattable: $"IT-{number:D2}")));
        foreach (RunDefinitions.Item rule in session.Definitions.Items.Values)
        {
            RunInventory.Item item = Seed(session: session, definitionId: rule.Id);
            ((RunInventory.StashCell)item.Location).Footprint.ShouldBe(expected: rule.Footprint);
            item.StorageGrid.ShouldBe(expected: rule.Storage);
            item.Condition.ShouldBe(expected: rule.MaximumCondition);
            rule.Appearance.CanonicalIcon.ShouldStartWith(expected: rule.Id.Replace(oldChar: '-', newChar: '_'));
        }

        RejectConfiguration(oldValue: "\"maxStack\": 100,", newValue: "\"maxStack\": 0,");
        RejectConfiguration(oldValue: "\"maxStack\": 100,", newValue: "\"maxStack\": 10001,");
        RejectConfiguration(oldValue: "\"maxStack\": 100,", newValue: "\"maxStack\": 2147483648,");
        RejectConfiguration(oldValue: "\"weightKilograms\": 0.001,", newValue: "\"weightKilograms\": 1e50,");
        RejectConfiguration(oldValue: "\"conditionMaximum\": 120,", newValue: "\"conditionMaximum\": -1,");
        RejectConfiguration(oldValue: "\"fitItem\": \"IT-02\"", newValue: "\"fitItem\": \"IT-03\"");
    }

    /// <summary>Two simultaneous moves of the same item publish at most one new location.</summary>
    [Fact]
    public void Phase06_E031_OneInstanceWinsOneConcurrentMove()
    {
        FoundationSession session = Start();
        RunInventory.Item source = Seed(session: session, definitionId: "IT-01");
        var original = (RunInventory.StashCell)source.Location;
        RunInventory.StashCell first = Cell(session: session, item: source, page: original.Page, x: 1);
        RunInventory.StashCell second = Cell(session: session, item: source, page: original.Page, x: 2);
        long revision = session.Snapshot.Revision;
        int success = Race(first: () => Move(session: session, source: source, target: first),
                second: () => Move(session: session, source: source, target: second));
        success.ShouldBe(expected: 1);
        session.Snapshot.Revision.ShouldBe(expected: revision + 1);
        session.Inventory.Items.Count(predicate: item => item.Id == source.Id).ShouldBe(expected: 1);
        RunInventory.Location committed = Find(session: session, id: source.Id).Location;
        (committed == first || committed == second).ShouldBeTrue();
    }

    /// <summary>The eleven slots exist even when the eight unoccupied slots own no items.</summary>
    [Fact]
    public void Phase06_E032_EmptySlotsHaveNoInventoryItems()
    {
        FoundationSession session = Start();
        RunDefinitions.EquipmentSlots.AsEnumerable().ShouldBe(expected:
                ["headwear", "face", "torso-clothing", "legs", "body-armor", "gloves", "boots", "backpack", "belt",
                    "left-hand", "right-hand"]);
        string[] occupied = [.. RunDefinitions.EquipmentSlots.Where(predicate: slot =>
                session.Inventory.Equipped(survivorId: Survivor, slot: slot) is not null)];
        occupied.ShouldBe(expected: ["torso-clothing", "legs", "boots"]);
        session.Inventory.Equipped(survivorId: Survivor, slot: "backpack").ShouldBeNull();
        session.Inventory.Equipped(survivorId: Survivor, slot: "torso-clothing")?.StorageGrid
                .ShouldBe(expected: new RunDefinitions.Size(width: 2, height: 1));
        session.Inventory.Equipped(survivorId: Survivor, slot: "legs")?.StorageGrid
                .ShouldBe(expected: new RunDefinitions.Size(width: 2, height: 2));
    }

    /// <summary>A committed equip cannot be applied again from the old source.</summary>
    [Fact]
    public void Phase06_E033_ReplayingAnEquipKeepsThePublication()
    {
        FoundationSession session = Start();
        RunInventory.Item source = Item(session: session, definitionId: "IT-23");
        RunInventory.Equipment target = Slot(slot: "backpack");
        Move(session: session, source: source, target: target).ShouldBeTrue();
        Reject(session: session, edit: () => Move(session: session, source: source, target: target));
        session.Inventory.Equipped(survivorId: Survivor, slot: "backpack")?.Id.ShouldBe(expected: source.Id);
        session.Inventory.NextItemId.ShouldBe(expected: 57L);
    }

    /// <summary>Enumeration order leaves lookups, total weight, and identities intact.</summary>
    [Fact]
    public void Phase06_E034_ReorderedItemsRetainTheSameOwnersAndWeight()
    {
        FoundationSession session = Start();
        RunInventory.Item[] original = [.. session.Inventory.Items];
        decimal total = Weight(session: session);
        Reverse(session: session);
        session.Inventory.Items.Select(selector: item => item.Id).ShouldBe(expected:
                original.Reverse().Select(selector: item => item.Id));
        session.Inventory.Items.OrderBy(keySelector: item => item.Id).ShouldBe(expected: original);
        Weight(session: session).ShouldBe(expected: total);
        session.Inventory.Equipped(survivorId: Survivor, slot: "boots")?.Id.ShouldBe(expected: 3L);
    }

    /// <summary>Zero and maximum condition are legal and fractional condition survives a move.</summary>
    [Fact]
    public void Phase06_E035_ConditionAndAuthoredWeightKeepExactPrecision()
    {
        FoundationSession session = Configured(oldValue: "\"weightKilograms\": 0.001,",
                newValue: "\"weightKilograms\": 0.001234567891,");
        session.Definitions.Items["IT-01"].WeightKilograms.ShouldBe(expected: 0.001234567891m);
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04", condition: 149.999999999m);
        Move(session: session, source: club, target: Slot(slot: "left-hand")).ShouldBeTrue();
        Find(session: session, id: club.Id).Condition.ShouldBe(expected: 149.999999999m);
        ChangeCondition(session: session, id: club.Id, condition: 0).ShouldBeTrue();
        ChangeCondition(session: session, id: club.Id, condition: 150).ShouldBeTrue();
        Reject(session: session, edit: () => ChangeCondition(session: session, id: club.Id, condition: -0.000000001m));
        Reject(session: session, edit: () => ChangeCondition(session: session, id: club.Id, condition: 150.000000001m));
        Reject(session: session, edit: () => ChangeCondition(session: session, id: club.Id, condition: null));
    }

    /// <summary>Two complete footprints may share their border without sharing a cell.</summary>
    [Fact]
    public void Phase06_E036_EdgeContactAcceptsAndOneCellOverlapRejects()
    {
        FoundationSession session = Start();
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04");
        RunInventory.Item arrows = Item(session: session, definitionId: "IT-02");
        int page = ((RunInventory.StashCell)club.Location).Page;
        Move(session: session, source: arrows, target: Cell(session: session, item: arrows, page: page, x: 1))
                .ShouldBeTrue();
        RunInventory.Item adjacent = Find(session: session, id: arrows.Id);
        Reject(session: session, edit: () => Move(session: session, source: adjacent,
                target: Cell(session: session, item: adjacent, page: page)));
        Find(session: session, id: club.Id).ShouldBeSameAs(expected: club);
    }

    /// <summary>An exact bottom-right fit commits while one more row or column preserves both items.</summary>
    [Fact]
    public void Phase06_E037_ExactFitAndOverflowUseTheCurrentRun()
    {
        FoundationSession session = Start();
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04");
        int page = ((RunInventory.StashCell)club.Location).Page;
        RunInventory.StashCell fit = Cell(session: session, item: club, page: page, x: 9, y: 7);
        Move(session: session, source: club, target: fit).ShouldBeTrue();
        RunInventory.Item fitted = Find(session: session, id: club.Id);
        fitted.Location.ShouldBe(expected: fit);
        Reject(session: session, edit: () => Move(session: session, source: fitted,
                target: Cell(session: session, item: fitted, page: page, x: 9, y: 8)));
        Reject(session: session, edit: () => Move(session: session, source: fitted,
                target: Cell(session: session, item: fitted, page: page, x: 10, y: 7)));
    }

    /// <summary>Contested equipment cannot replace the winner or displace the losing item.</summary>
    [Fact]
    public void Phase06_E038_ContestedEmptyEquipmentSlotAcceptsOneIdentity()
    {
        FoundationSession session = Start();
        RunInventory.Item first = Item(session: session, definitionId: "IT-04");
        RunInventory.Item second = Item(session: session, definitionId: "IT-04", ordinal: 1);
        RunInventory.Equipment target = Slot(slot: "right-hand");
        int success = Race(first: () => Move(session: session, source: first, target: target),
                second: () => Move(session: session, source: second, target: target));
        success.ShouldBe(expected: 1);
        long? winner = session.Inventory.Equipped(survivorId: Survivor, slot: "right-hand")?.Id;
        (winner == first.Id || winner == second.Id).ShouldBeTrue();
        RunInventory.Item loser = winner == first.Id ? second : first;
        Find(session: session, id: loser.Id).ShouldBeSameAs(expected: loser);
    }

    /// <summary>An empty defined slot is usable only by compatible equipment and a real survivor.</summary>
    [Fact]
    public void Phase06_E039_EmptySlotsDoNotCreateExtraOwners()
    {
        FoundationSession session = Start();
        RunInventory.Item pack = Item(session: session, definitionId: "IT-23");
        Reject(session: session, edit: () => Move(session: session, source: pack, target: Slot(slot: "belt")));
        Reject(session: session, edit: () => Move(session: session, source: pack, target: Slot(slot: "carry")));
        Reject(session: session, edit: () => Move(session: session, source: pack,
                target: Slot(slot: "backpack", survivor: "binding-2")));
        Move(session: session, source: pack, target: Slot(slot: "backpack")).ShouldBeTrue();
        session.Inventory.Equipped(survivorId: Survivor, slot: "backpack")?.Id.ShouldBe(expected: pack.Id);
    }

    /// <summary>A quarter-turn changes occupied dimensions and returning restores the same exact item.</summary>
    [Fact]
    public void Phase06_E040_RotationCycleNeverAllocatesAnIdentity()
    {
        FoundationSession session = Start();
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04", condition: 23.125m);
        int page = ((RunInventory.StashCell)club.Location).Page;
        RunInventory.StashCell rotated = Cell(session: session, item: club, page: page, rotated: true);
        long next = session.Inventory.NextItemId;
        Move(session: session, source: club, target: rotated).ShouldBeTrue();
        RunInventory.Item live = Find(session: session, id: club.Id);
        ((RunInventory.StashCell)live.Location).Footprint
                .ShouldBe(expected: new RunDefinitions.Size(width: 3, height: 1));
        Reject(session: session, edit: () => Move(session: session, source: club, target: rotated));
        Move(session: session, source: live, target: club.Location).ShouldBeTrue();
        Find(session: session, id: club.Id).ShouldBe(expected: club);
        session.Inventory.NextItemId.ShouldBe(expected: next);
    }

    /// <summary>Removing the original equipment first makes manual replacement legal.</summary>
    [Fact]
    public void Phase06_E041_ManualUnequipPrecedesReplacementAndBowPairing()
    {
        FoundationSession session = Start();
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04");
        RunInventory.Item bow = Item(session: session, definitionId: "IT-07");
        RunInventory.Item arrows = Item(session: session, definitionId: "IT-02");
        RunInventory.Equipment left = Slot(slot: "left-hand");
        RunInventory.Equipment right = Slot(slot: "right-hand");
        Move(session: session, source: club, target: left).ShouldBeTrue();
        Reject(session: session, edit: () => Move(session: session, source: bow, target: left));
        Reject(session: session, edit: () => Move(session: session, source: bow, target: right));
        Move(session: session, source: Find(session: session, id: club.Id), target: club.Location).ShouldBeTrue();
        Move(session: session, source: bow, target: left).ShouldBeTrue();
        Move(session: session, source: arrows, target: right).ShouldBeTrue();
        session.Inventory.Equipped(survivorId: Survivor, slot: "left-hand")?.Id.ShouldBe(expected: bow.Id);
        session.Inventory.Equipped(survivorId: Survivor, slot: "right-hand")?.Id.ShouldBe(expected: arrows.Id);
    }

    /// <summary>Extreme cells, forged occupied dimensions, and unsupported rotations are atomic failures.</summary>
    [Fact]
    public void Phase06_E042_InvalidGridGeometryCannotWrapOrShrink()
    {
        FoundationSession session = Configured(oldValue: "\"rotatable\": true,", newValue: "\"rotatable\": false,");
        RunInventory.Item club = Seed(session: session, definitionId: "IT-04");
        int page = ((RunInventory.StashCell)club.Location).Page;
        foreach ((int checkedPage, int x, int y, bool rotated) in new (int, int, int, bool)[]
                { (page, int.MaxValue, 0, false), (page, 0, int.MinValue, false), (int.MaxValue, 0, 0, false),
                    (-1, 0, 0, false), (page, 1, 1, true) })
        {
            Reject(session: session, edit: () => Move(session: session, source: club,
                    target: Cell(session: session, item: club, page: checkedPage, x: x, y: y, rotated: rotated)));
        }

        var forged = new RunInventory.StashCell(page: page, x: 9, y: 9, rotated: false,
                footprint: new RunDefinitions.Size(width: 1, height: 1));
        Reject(session: session, edit: () => Move(session: session, source: club, target: forged));
        Find(session: session, id: club.Id).ShouldBeSameAs(expected: club);
    }

    /// <summary>A one-unit split lives beside the original stack with its own stable identity.</summary>
    [Fact]
    public void Phase06_E043_SplitKeepsTheSourceAndAddsOneAdjacentStack()
    {
        FoundationSession session = Start();
        RunInventory.Item money = Seed(session: session, definitionId: "IT-01", quantity: 10);
        int page = ((RunInventory.StashCell)money.Location).Page;
        long fresh = session.Inventory.NextItemId;
        RunInventory.StashCell beside = Cell(session: session, item: money, page: page, x: 1);
        Split(session: session, source: money, quantity: 1, target: beside).ShouldBeTrue();
        Find(session: session, id: money.Id).ShouldBe(expected: money with { Quantity = 9 });
        RunInventory.Item extracted = Find(session: session, id: fresh);
        (extracted.DefinitionId, extracted.Location, extracted.Condition, extracted.Quantity)
                .ShouldBe(expected: ("IT-01", beside, null, 1));
        session.Inventory.NextItemId.ShouldBe(expected: fresh + 1);
    }

    /// <summary>A partial merge fills the exact cap and leaves the original source quantity and location.</summary>
    [Fact]
    public void Phase06_E044_SplitAndPartialMergeConserveTheExactBoundary()
    {
        FoundationSession session = Start();
        RunInventory.Item money = Seed(session: session, definitionId: "IT-01", quantity: 100);
        RunInventory.Item destination = Seed(session: session, definitionId: "IT-01", quantity: 99);
        int page = ((RunInventory.StashCell)money.Location).Page;
        RunInventory.StashCell target = Cell(session: session, item: money, page: page, x: 1);
        long newId = session.Inventory.NextItemId;
        Split(session: session, source: money, quantity: 99, target: target).ShouldBeTrue();
        Find(session: session, id: money.Id).Quantity.ShouldBe(expected: 1);
        RunInventory.Item source = Find(session: session, id: newId);
        Merge(session: session, source: source, target: destination).ShouldBeTrue();
        Find(session: session, id: newId).ShouldBe(expected: source with { Quantity = 98 });
        Find(session: session, id: destination.Id).ShouldBe(expected: destination with { Quantity = 100 });
        session.Inventory.NextItemId.ShouldBe(expected: newId + 1);
    }

    /// <summary>A stale destination stack cannot be merged again after a competing merge fills it.</summary>
    [Fact]
    public void Phase06_E045_CompetingMergesShareTheOneDestinationCapacity()
    {
        FoundationSession session = Start();
        RunInventory.Item destination = Seed(session: session, definitionId: "IT-09", quantity: 4);
        RunInventory.Item first = Item(session: session, definitionId: "IT-09");
        RunInventory.Item second = Item(session: session, definitionId: "IT-09", ordinal: 1);
        int success = Race(first: () => Merge(session: session, source: first, target: destination),
                second: () => Merge(session: session, source: second, target: destination));
        success.ShouldBe(expected: 1);
        Find(session: session, id: destination.Id).Quantity.ShouldBe(expected: 5);
        int[] remaining = [Find(session: session, id: first.Id).Quantity,
            Find(session: session, id: second.Id).Quantity];
        remaining.Order().ShouldBe(expected: [4, 5]);
    }

    /// <summary>A full destination or single-unit source has no legal extra capacity to commit.</summary>
    [Fact]
    public void Phase06_E046_FullMissingAndSelfTargetsRejectUnchanged()
    {
        FoundationSession session = Start();
        RunInventory.Item full = Item(session: session, definitionId: "IT-08");
        RunInventory.Item one = Seed(session: session, definitionId: "IT-08");
        int page = ((RunInventory.StashCell)one.Location).Page;
        Reject(session: session, edit: () => Merge(session: session, source: one, target: full));
        Reject(session: session, edit: () => Merge(session: session, source: one, target: one));
        Reject(session: session, edit: () => Split(session: session, source: one, quantity: 1,
                target: Cell(session: session, item: one, page: page, x: 1)));
        Reject(session: session, edit: () => Merge(session: session, source: full, target: one with { Quantity = 2 }));
    }

    /// <summary>A fully merged source cannot reappear even when its old location is free.</summary>
    [Fact]
    public void Phase06_E047_MergeRetiresTheExactSourceAndRejectsReplay()
    {
        FoundationSession session = Start();
        RunInventory.Item first = Seed(session: session, definitionId: "IT-08", quantity: 2);
        RunInventory.Item second = Seed(session: session, definitionId: "IT-08", quantity: 3);
        long next = session.Inventory.NextItemId;
        Merge(session: session, source: first, target: second).ShouldBeTrue();
        session.Inventory.Find(itemId: first.Id).ShouldBeNull();
        Find(session: session, id: second.Id).ShouldBe(expected: second with { Quantity = 5 });
        Reject(session: session, edit: () => Merge(session: session, source: first, target: second));
        Reject(session: session, edit: () => session.TryEditInventory(edit: (root, _, _) =>
                root.WithItems(items: root.Items.Add(item: first))));
        session.Inventory.NextItemId.ShouldBe(expected: next);
    }

    /// <summary>The proposed source and destination determine the result independently of item order.</summary>
    [Fact]
    public void Phase06_E048_StackMergeHonorsIdentitiesAfterEnumerationChanges()
    {
        FoundationSession forward = Start();
        FoundationSession reverse = Start();
        foreach (FoundationSession session in new[] { forward, reverse })
        {
            RunInventory.Item first = Seed(session: session, definitionId: "IT-08", quantity: 7);
            RunInventory.Item second = Seed(session: session, definitionId: "IT-08", quantity: 8);
            if (session == reverse)
            {
                Reverse(session: session);
            }

            decimal weight = Weight(session: session);
            Merge(session: session, source: first, target: second).ShouldBeTrue();
            Find(session: session, id: first.Id).ShouldBe(expected: first with { Quantity = 5 });
            Find(session: session, id: second.Id).ShouldBe(expected: second with { Quantity = 10 });
            Weight(session: session).ShouldBe(expected: weight);
        }

        forward.Inventory.Items.OrderBy(keySelector: item => item.Id)
                .ShouldBe(expected: reverse.Inventory.Items.OrderBy(keySelector: item => item.Id));
    }

    /// <summary>Invalid amounts and incompatible state leave exact quantities and allocation unchanged.</summary>
    [Fact]
    public void Phase06_E049_SplitRejectsNonIntegralAndInvalidAmountsAtomically()
    {
        FoundationSession session = Start();
        RunInventory.Item money = Seed(session: session, definitionId: "IT-01", quantity: 10);
        RunInventory.Item otherMoney = Seed(session: session, definitionId: "IT-01", quantity: 2);
        RunInventory.Item food = Seed(session: session, definitionId: "IT-08", quantity: 2);
        int page = ((RunInventory.StashCell)money.Location).Page;
        RunInventory.StashCell target = Cell(session: session, item: money, page: page, x: 1);
        foreach (decimal amount in new[] { 0m, -1m, 0.999999999m, 1.000000001m, 10m, 11m, decimal.MaxValue })
        {
            Reject(session: session, edit: () => Split(session: session, source: money, quantity: amount,
                    target: target));
        }

        Reject(session: session, edit: () => Split(session: session, source: money, quantity: 1,
                target: money.Location));
        Reject(session: session, edit: () => Merge(session: session, source: money, target: food));
        Reject(session: session, edit: () => Merge(session: session, source: money with { Condition = 0 },
                target: otherMoney));
        Split(session: session, source: money, quantity: 1, target: target).ShouldBeTrue();
        Find(session: session, id: money.Id).Quantity.ShouldBe(expected: 9);
        CheckConditionCompatibility();
    }

    internal const string Survivor = "survivor-01";

    internal static FoundationSession Start()
    {
        return new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire");
    }

    internal static FoundationSession Configured(string oldValue, string newValue,
            string config = "content/config/items.json")
    {
        return FoundationSession.CreateFromResources(sessionId: "run", playerId: "host", actorId: "host-wire",
                read: (path, _) =>
                {
                    using Stream stream = typeof(FoundationSession).Assembly.GetManifestResourceStream(name: path)
                            ?? throw new InvalidOperationException(message: path);
                    using var reader = new StreamReader(stream: stream);
                    string contents = reader.ReadToEnd();
                    return Encoding.UTF8.GetBytes(s: StringComparer.Ordinal.Equals(x: path, y: config)
                            ? contents.Replace(oldValue: oldValue, newValue: newValue,
                                    comparisonType: StringComparison.Ordinal)
                            : contents);
                });
    }

    internal static RunInventory.Item Item(FoundationSession session, string definitionId, int ordinal = 0)
    {
        return session.Inventory.Items.Where(predicate: item =>
                StringComparer.Ordinal.Equals(x: item.DefinitionId, y: definitionId))
                .OrderBy(keySelector: item => item.Id).ElementAt(index: ordinal);
    }

    internal static RunInventory.Item Find(FoundationSession session, long id)
    {
        return session.Inventory.Find(itemId: id) ?? throw new InvalidOperationException(message: "Missing item.");
    }

    internal static RunInventory.Item Seed(FoundationSession session, string definitionId, int quantity = 1,
            decimal? condition = null)
    {
        RunInventory root = session.Inventory;
        RunDefinitions.Item definition = session.Definitions.Items[definitionId];
        var item = new RunInventory.Item(id: root.NextItemId, definitionId: definitionId, quantity: quantity,
                condition: condition ?? definition.MaximumCondition,
                location: RunItemTransfer.Stash(definition: definition, page: root.StashPageCount, x: 0, y: 0),
                storageGrid: definition.Storage);
        session.TryEditInventory(edit: (current, _, _) => new RunInventory(items: current.Items.Add(item: item),
                stashGrid: current.StashGrid, stashPageCount: current.StashPageCount + 1,
                nextItemId: current.NextItemId + 1)).ShouldBeTrue();
        return item;
    }

    internal static RunInventory.StashCell Cell(FoundationSession session, RunInventory.Item item, int page,
            int x = 0, int y = 0, bool rotated = false)
    {
        return RunItemTransfer.Stash(definition: session.Definitions.Items[item.DefinitionId], page: page,
                x: x, y: y, rotated: rotated);
    }

    internal static RunInventory.Equipment Slot(string slot, string survivor = Survivor)
    {
        return new RunInventory.Equipment(survivorId: survivor, slot: slot);
    }

    internal static bool Move(FoundationSession session, RunInventory.Item source, RunInventory.Location target)
    {
        return session.TryEditInventory(edit: (root, _, _) =>
                RunItemTransfer.Move(inventory: root, expected: source, destination: target));
    }

    internal static bool Split(FoundationSession session, RunInventory.Item source, decimal quantity,
            RunInventory.Location target)
    {
        return session.TryEditInventory(edit: (root, _, _) => RunItemTransfer.Split(inventory: root,
                expected: source, quantity: quantity, destination: target));
    }

    internal static bool Merge(FoundationSession session, RunInventory.Item source, RunInventory.Item target)
    {
        return session.TryEditInventory(edit: (root, definitions, _) => RunItemTransfer.Merge(inventory: root,
                definitions: definitions, expectedSource: source, expectedDestination: target));
    }

    internal static void Reject(FoundationSession session, Func<bool> edit)
    {
        FoundationSnapshot snapshot = session.Snapshot;
        RunInventory inventory = session.Inventory;
        edit().ShouldBeFalse();
        session.Snapshot.ShouldBeSameAs(expected: snapshot);
        session.Inventory.ShouldBeSameAs(expected: inventory);
    }

    internal static int Race(Func<bool> first, Func<bool> second)
    {
        bool firstResult = false;
        bool secondResult = false;
        Parallel.Invoke(actions: [() => firstResult = first(), () => secondResult = second()]);
        return Convert.ToInt32(value: firstResult) + Convert.ToInt32(value: secondResult);
    }

    internal static void Reverse(FoundationSession session)
    {
        session.TryEditInventory(edit: (root, _, _) =>
                root.WithItems(items: [.. root.Items.Reverse()])).ShouldBeTrue();
    }

    private static bool ChangeCondition(FoundationSession session, long id, decimal? condition)
    {
        RunInventory.Item item = Find(session: session, id: id);
        return session.TryEditInventory(edit: (root, _, _) => root.WithItems(items: root.Items.Replace(
                oldValue: item, newValue: item with { Condition = condition })));
    }

    private static void CheckConditionCompatibility()
    {
        RunDefinitions baseline = RunDefinitions.Default;
        RunDefinitions.Item arrows = baseline.Items["IT-02"];
        var conditional = new RunDefinitions.Item(id: arrows.Id, footprint: arrows.Footprint,
                maxStack: arrows.MaxStack, weightKilograms: arrows.WeightKilograms, rotatable: arrows.Rotatable,
                slot: arrows.Slot, storage: arrows.Storage, maximumCondition: 2m, appearance: arrows.Appearance);
        var definitions = new RunDefinitions(start: new RunDefinitions.Start(survivorCount: baseline.SurvivorCount,
                playerLimit: baseline.PlayerLimit, role: baseline.InitialRole, outfit: baseline.StartingOutfit,
                stash: baseline.StartingStash), survivors: new RunDefinitions.SurvivorSettings(
                        maximumHealth: baseline.MaximumHealth, maximumStamina: baseline.MaximumStamina,
                        refugeUnits: 12, dangerUnits: 12),
                inventory: new RunDefinitions.InventorySettings(stashGrid: baseline.StashGrid,
                        maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4),
                items: baseline.Items.SetItem(key: arrows.Id, value: conditional));
        var session = new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire",
                world: RunWorld.Foundation, definitions: definitions);
        RunInventory.Item first = Seed(session: session, definitionId: arrows.Id, quantity: 3, condition: 1.125m);
        RunInventory.Item second = Seed(session: session, definitionId: arrows.Id, quantity: 4, condition: 1.126m);
        Reject(session: session, edit: () => Merge(session: session, source: first, target: second));
        ChangeCondition(session: session, id: second.Id, condition: 1.125m).ShouldBeTrue();
        Merge(session: session, source: first, target: Find(session: session, id: second.Id)).ShouldBeTrue();
        (Find(session: session, id: second.Id).Quantity, Find(session: session, id: second.Id).Condition)
                .ShouldBe(expected: (7, 1.125m));
    }

    private static decimal Weight(FoundationSession session)
    {
        return session.Inventory.Items.Sum(selector: item =>
                item.Quantity * session.Definitions.Items[item.DefinitionId].WeightKilograms);
    }

    private static void RejectConfiguration(string oldValue, string newValue)
    {
        Should.Throw<InvalidDataException>(actual: () => Configured(oldValue: oldValue, newValue: newValue));
    }
}
