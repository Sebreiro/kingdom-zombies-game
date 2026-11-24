using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the actual fresh run's recruited population and exact inventory.</summary>
[Trait("Tier", "1")]
public sealed class Phase06PopulationTests
{
    /// <summary>The native session authority owns the persistent run roster.</summary>
    [Fact]
    public void Phase06_E007_FreshRunCreatesEightIndependentOutfits()
    {
        FoundationSession session = Start();
        session.Population.Survivors.Select(selector: survivor => survivor.Id).ShouldBe(expected:
                ["survivor-01", "survivor-02", "survivor-03", "survivor-04", "survivor-05", "survivor-06",
                    "survivor-07", "survivor-08"]);
        session.Population.Survivors.Select(selector: survivor =>
                (survivor.Health, survivor.Stamina, survivor.Role)).Distinct()
                .ShouldBe(expected: [(100m, 100m, "unassigned")]);
        long next = 1;
        foreach (RunPopulation.Survivor survivor in session.Population.Survivors)
        {
            RunInventory.Item[] items = Outfit(session: session, survivorId: survivor.Id);
            items.Select(selector: item => item.Id).ShouldBe(expected: [next, next + 1, next + 2]);
            items.Select(selector: item => item.DefinitionId).ShouldBe(expected: ["IT-11", "IT-12", "IT-17"]);
            items.Select(selector: item => ((RunInventory.Equipment)item.Location).Slot)
                    .ShouldBe(expected: ["torso-clothing", "legs", "boots"]);
            next += 3;
        }

        session.Inventory.Items.Select(selector: item => item.Id)
                .ShouldBe(expected: Enumerable.Range(start: 1, count: 56).Select(selector: id => (long)id));
    }

    /// <summary>Each supported controller count has the same eight survivors and one exact kit.</summary>
    [Fact]
    public void Phase06_E008_OneThroughFourControllersShareTheOneRunKit()
    {
        for (int players = 1; players <= 4; players++)
        {
            var session = FoundationSession.CreateFromResources(sessionId: "run", playerId: "host",
                    actorId: "host-wire", read: ReadFixture);
            RunPopulation.Survivor[] survivors = [.. session.Population.Survivors];
            RunInventory inventory = session.Inventory;
            for (int player = 2; player <= players; player++)
            {
                Add(session: session, player: player).ShouldBeTrue();
                session.MarkReady(bindingId: Numbered(prefix: "binding", number: player)).ShouldBeTrue();
            }

            session.Snapshot.Participants.Length.ShouldBe(expected: players);
            session.Population.Survivors.AsEnumerable().ShouldBe(expected: survivors);
            session.Population.Leases.Select(selector: lease => lease.SurvivorId)
                    .ShouldBe(expected: survivors.Take(count: players).Select(selector: survivor => survivor.Id));
            session.Inventory.ShouldBeSameAs(expected: inventory);
            CheckKit(session: session);
            if (players == 4)
            {
                Add(session: session, player: 5).ShouldBeFalse();
            }
        }
    }

    /// <summary>A departing guest leaves the actual moved recruit and outfit available for another binding.</summary>
    [Fact]
    public void Phase06_E009_ReservationsReleaseTheSameRecruitAndRejectOldInput()
    {
        FoundationSession session = Start();
        RunInventory inventory = session.Inventory;
        long[] outfit = [.. Outfit(session: session, survivorId: "survivor-02").Select(selector: item => item.Id)];
        Add(session: session, player: 2).ShouldBeTrue();
        RunPopulation.Lease before = Lease(session: session, bindingId: "binding-2");
        (before.SessionId, before.PlayerId, before.Slot, before.SurvivorId, before.Generation)
                .ShouldBe(expected: ("run", "player-2", 2, "survivor-02", 2L));
        var command = new FoundationMovementCommand(sessionId: "run", playerId: "player-2", actorId: "wire-2",
                direction: 1, bindingId: "binding-2");
        session.TryAccept(command: command, trustedBinding: "binding-2", trustedPlayer: "player-2",
                trustedActor: "wire-2").ShouldBeFalse();
        session.MarkReady(bindingId: "binding-2").ShouldBeTrue();
        session.TryAccept(command: command, trustedBinding: "binding-2", trustedPlayer: "player-2",
                trustedActor: "wire-2").ShouldBeTrue();
        session.Step(elapsedMilliseconds: 100);
        RunPopulation.Survivor moved = session.Population.ForLease(lease: before);
        session.RemoveParticipant(bindingId: "binding-2").ShouldBeTrue();

        session.Population.Survivors[1].PositionSubunits.ShouldBe(expected: -7700);
        session.Population.Survivors[1].ShouldBeSameAs(expected: moved);
        session.Population.FindLease(bindingId: "binding-2").ShouldBeNull();
        session.MarkReady(bindingId: "binding-2").ShouldBeFalse();
        Add(session: session, player: 2).ShouldBeFalse();
        Add(session: session, player: 3).ShouldBeTrue();
        RunPopulation.Lease replacement = Lease(session: session, bindingId: "binding-3");
        (replacement.SurvivorId, replacement.Slot, replacement.Generation)
                .ShouldBe(expected: (before.SurvivorId, before.Slot, before.Generation + 1));
        session.Population.ForLease(lease: replacement).ShouldBeSameAs(expected: moved);
        session.Snapshot.Participants[1].Actor.ActorId.ShouldBe(expected: "wire-3");
        session.Snapshot.Participants[1].Actor.PositionSubunits.ShouldBe(expected: moved.PositionSubunits);
        session.MarkReady(bindingId: "binding-3").ShouldBeTrue();
        session.TryAccept(command: command, trustedBinding: "binding-2", trustedPlayer: "player-2",
                trustedActor: "wire-2").ShouldBeFalse();
        session.TryAccept(command: command, trustedBinding: "binding-3", trustedPlayer: "player-3",
                trustedActor: "wire-3").ShouldBeFalse();
        session.Step(elapsedMilliseconds: 100);
        session.Population.ForLease(lease: replacement).ShouldBeSameAs(expected: moved);
        session.Inventory.ShouldBeSameAs(expected: inventory);
        Outfit(session: session, survivorId: "survivor-02").Select(selector: item => item.Id)
                .ShouldBe(expected: outfit);
        session.Close();
        session.TryAccept(command: command with { PlayerId = "player-3", ActorId = "wire-3", BindingId = "binding-3" },
                trustedBinding: "binding-3", trustedPlayer: "player-3", trustedActor: "wire-3").ShouldBeFalse();
        CheckConcurrentAdmission();
    }

    /// <summary>Every outfit owns only its occupied equipment locations and its empty authored pockets.</summary>
    [Fact]
    public void Phase06_E010_NewSurvivorsAndStorageHaveNoPersonalSupplies()
    {
        FoundationSession session = Start();
        RunDefinitions.EquipmentSlots.AsEnumerable().ShouldBe(expected:
                ["headwear", "face", "torso-clothing", "legs", "body-armor", "gloves", "boots", "backpack", "belt",
                    "left-hand", "right-hand"]);
        foreach (RunPopulation.Survivor survivor in session.Population.Survivors)
        {
            RunInventory.Item[] items = Outfit(session: session, survivorId: survivor.Id);
            items.Select(selector: item => (item.Quantity, item.Condition)).Distinct().ShouldBe(expected:
                    [(1, (decimal?)null)]);
            items.Select(selector: item => session.Definitions.Items[item.DefinitionId].Storage).ShouldBe(expected:
                    [new RunDefinitions.Size(width: 2, height: 1), new RunDefinitions.Size(width: 2, height: 2), null]);
        }

        session.Inventory.Items.Count(predicate: item => item.Location is RunInventory.Equipment)
                .ShouldBe(expected: 24);
        session.Inventory.Items.All(predicate: item => item.Location is RunInventory.Equipment or
                RunInventory.StashCell).ShouldBeTrue();
    }

    /// <summary>Duplicate reservations and readiness acknowledgments leave one published inventory.</summary>
    [Fact]
    public void Phase06_E011_RepeatedAdmissionDoesNotMultiplyBelongings()
    {
        FoundationSession session = Start();
        Add(session: session, player: 2).ShouldBeTrue();
        FoundationSnapshot reserved = session.Snapshot;
        RunInventory inventory = session.Inventory;
        Add(session: session, player: 2).ShouldBeFalse();
        session.Snapshot.ShouldBeSameAs(expected: reserved);
        session.MarkReady(bindingId: "binding-2").ShouldBeTrue();
        FoundationSnapshot ready = session.Snapshot;
        session.MarkReady(bindingId: "binding-2").ShouldBeFalse();
        session.Snapshot.ShouldBeSameAs(expected: ready);
        session.Inventory.ShouldBeSameAs(expected: inventory);
        CheckKit(session: session);
    }

    /// <summary>Joining and marking readiness in a different order leaves the same roster and stash order.</summary>
    [Fact]
    public void Phase06_E012_AdmissionOrderingPreservesPublishedIdentityAndKitOrder()
    {
        FoundationSession first = Start();
        FoundationSession second = Start();
        foreach (int player in new[] { 2, 3, 4 })
        {
            Add(session: first, player: player).ShouldBeTrue();
            first.MarkReady(bindingId: Numbered(prefix: "binding", number: player)).ShouldBeTrue();
        }

        foreach (int player in new[] { 4, 2, 3 })
        {
            Add(session: second, player: player).ShouldBeTrue();
        }

        foreach (int player in new[] { 3, 2, 4 })
        {
            second.MarkReady(bindingId: Numbered(prefix: "binding", number: player)).ShouldBeTrue();
        }

        first.Population.Survivors.AsEnumerable().ShouldBe(expected: second.Population.Survivors.AsEnumerable());
        first.Inventory.Items.AsEnumerable().ShouldBe(expected: second.Inventory.Items.AsEnumerable());
        first.Inventory.StashPageCount.ShouldBe(expected: second.Inventory.StashPageCount);
    }

    /// <summary>The kit retains exact authored condition, quantities, geometry and precision.</summary>
    [Fact]
    public void Phase06_E013_ConfiguredStacksAndConditionAreExact()
    {
        FoundationSession session = Start();
        CheckKit(session: session);
        session.Definitions.Items.Count.ShouldBe(expected: 24);
        session.Definitions.Items["IT-01"].WeightKilograms.ShouldBe(expected: 0.001m);
        session.Definitions.Items["IT-02"].WeightKilograms.ShouldBe(expected: 0.03m);
        session.Definitions.Items["IT-21"].Storage.ShouldBe(expected: new RunDefinitions.Size(width: 4, height: 2));
        session.Definitions.Items["IT-23"].Storage.ShouldBe(expected: new RunDefinitions.Size(width: 4, height: 4));
        session.Inventory.StashPageCount.ShouldBe(expected: 2);
        session.Inventory.NextItemId.ShouldBe(expected: 57);
        var occupied = new HashSet<(int page, int x, int y)>();
        foreach (RunInventory.Item item in session.Inventory.Items.Where(predicate: item =>
                item.Location is RunInventory.StashCell))
        {
            var cell = (RunInventory.StashCell)item.Location;
            RunDefinitions.Size footprint = session.Definitions.Items[item.DefinitionId].Footprint;
            cell.Footprint.ShouldBe(expected: cell.Rotated
                    ? new RunDefinitions.Size(width: footprint.Height, height: footprint.Width) : footprint);
            cell.Page.ShouldBeInRange(from: 0, to: 1);
            for (int y = cell.Y; y < cell.Y + cell.Footprint.Height; y++)
            {
                for (int x = cell.X; x < cell.X + cell.Footprint.Width; x++)
                {
                    x.ShouldBeInRange(from: 0, to: 9);
                    y.ShouldBeInRange(from: 0, to: 9);
                    occupied.Add(item: (cell.Page, x, y)).ShouldBeTrue();
                }
            }
        }

        occupied.Count.ShouldBe(expected: 102);

        var configured = FoundationSession.CreateFromResources(sessionId: "configured", playerId: "host",
                actorId: "host-wire", read: ReadVariation);
        (configured.Population.Survivors[0].Health, configured.Population.Survivors[0].Stamina)
                .ShouldBe(expected: (99.75m, 100.125m));
        configured.Inventory.Items.Where(predicate: item => item.DefinitionId is "IT-01")
                .Sum(selector: item => item.Quantity).ShouldBe(expected: 180);
        configured.Inventory.Items.Where(predicate: item => item.DefinitionId is "IT-07")
                .All(predicate: item => item.Location is RunInventory.StashCell { Rotated: true }).ShouldBeTrue();
        configured.Inventory.StashPageCount.ShouldBeGreaterThan(expected: 2);
    }

    /// <summary>Missing, malformed, oversized and contradictory authored bytes fail with the source path.</summary>
    [Fact]
    public void RejectsInvalidRunDefinitionsBeforeBuildingInventory()
    {
        string path = "content/config/run.json";
        InvalidDataException missing = Should.Throw<InvalidDataException>(actual: () => RunDefinitions.Load(
                read: (candidate, limit) => StringComparer.Ordinal.Equals(x: candidate, y: path)
                        ? null : ReadFixture(path: candidate, limit: limit)));
        missing.Message.ShouldContain(expected: "RUN_DEFINITION_INVALID " + path);
        RejectChange(path: path, original: "\"startingSurvivors\": 8", changed: "\"startingSurvivors\": 9");
        RejectChange(path: path, original: "\"quantity\": 240", changed: "\"quantity\": 239");
        RejectChange(path: path, original: "\"startingSurvivors\": 8",
                changed: "\"startingSurvivors\": 8, \"startingSurvivors\": 8");
        RejectChange(path: "content/config/items.json", original: "\"slot\": \"torso-clothing\"",
                changed: "\"slot\": \"legs\"");
        RejectChange(path: "content/config/items.json", original: "\"conditionMaximum\": 150",
                changed: "\"conditionMaximum\": 0");
        RejectChange(path: "content/config/combat.json", original: "\"maximumDurability\": 200",
                changed: "\"maximumDurability\": 201");
        RejectChange(path: "content/config/inventory.json", original: "\"rootContainerLevel\": 1",
                changed: "\"rootContainerLevel\": 2");
        InvalidDataException oversized = Should.Throw<InvalidDataException>(actual: () => RunDefinitions.Load(
                read: (candidate, limit) => StringComparer.Ordinal.Equals(x: candidate, y: path) ? new byte[limit + 1]
                        : ReadFixture(path: candidate, limit: limit)));
        oversized.Message.ShouldContain(expected: "RUN_DEFINITION_INVALID " + path);
    }

    /// <summary>The coordinator reads authored settings before it opens a host port or starts a solo run.</summary>
    [Fact]
    public void ReadsSuppliedResourcesBeforeOpeningTransport()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        InvalidDataException error = Should.Throw<InvalidDataException>(actual: () => host.Start(host: true,
                read: (path, limit) => path is "content/config/items.json" ? null
                        : ReadFixture(path: path, limit: limit)));
        error.Message.ShouldContain(expected: "RUN_DEFINITION_INVALID content/config/items.json");
        host.IsHost.ShouldBeFalse();
        host.Snapshot.ShouldBeNull();
        port.Connected.ShouldBeFalse();

        var solo = new FoundationCoordinator(playerId: "host", label: "Host");
        solo.Start(host: false, read: ReadVariation);
        solo.Update(elapsedMilliseconds: 0);
        (solo.LocalWorld?.stashReachable ?? false).ShouldBeTrue();
        solo.Close();
    }

    private static FoundationSession Start()
    {
        return new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire");
    }

    private static bool Add(FoundationSession session, int player)
    {
        return session.AddParticipant(bindingId: Numbered(prefix: "binding", number: player),
                playerId: Numbered(prefix: "player", number: player), actorId: Numbered(prefix: "wire", number: player),
                label: Numbered(prefix: "Guest", number: player));
    }

    private static string Numbered(string prefix, int number)
    {
        return FormattableString.Invariant(formattable: $"{prefix}-{number}");
    }

    private static RunPopulation.Lease Lease(FoundationSession session, string bindingId)
    {
        return session.Population.FindLease(bindingId: bindingId)
                ?? throw new InvalidOperationException(message: "MISSING_TEST_LEASE");
    }

    private static void CheckConcurrentAdmission()
    {
        FoundationSession session = Start();
        Add(session: session, player: 2).ShouldBeTrue();
        Add(session: session, player: 3).ShouldBeTrue();
        RunInventory inventory = session.Inventory;
        bool[] results = new bool[2];
        Parallel.Invoke(actions: [() => results[0] = Add(session: session, player: 4),
            () => results[1] = Add(session: session, player: 5)]);
        results.Count(predicate: accepted => accepted).ShouldBe(expected: 1);
        session.Population.Leases.Select(selector: lease => lease.SurvivorId).ShouldBe(expected:
                ["survivor-01", "survivor-02", "survivor-03", "survivor-04"]);
        session.Population.Leases.Select(selector: lease => lease.Slot).ShouldBe(expected: [1, 2, 3, 4]);
        session.Population.Leases.Select(selector: lease => lease.Generation).ShouldBe(expected: [1L, 2L, 3L, 4L]);
        session.Inventory.ShouldBeSameAs(expected: inventory);
    }

    private static RunInventory.Item[] Outfit(FoundationSession session, string survivorId)
    {
        return [.. session.Inventory.Items.Where(predicate: item =>
                item.Location is RunInventory.Equipment location && StringComparer.Ordinal.Equals(
                        x: location.SurvivorId, y: survivorId))];
    }

    private static void CheckKit(FoundationSession session)
    {
        RunInventory.Item[] stash = [.. session.Inventory.Items.Where(predicate: item =>
                item.Location is RunInventory.StashCell)];
        (string id, int quantity, int stacks, decimal? condition)[] expected =
        [
            ("IT-01", 100, 2, null), ("IT-04", 1, 4, 150m), ("IT-07", 1, 4, 160m), ("IT-02", 30, 8, null),
            ("IT-23", 1, 4, null), ("IT-21", 1, 4, null), ("IT-08", 10, 2, null), ("IT-09", 5, 2, null),
            ("IT-10", 5, 2, null),
        ];
        stash.GroupBy(keySelector: item => (item.DefinitionId, item.Quantity, item.Condition))
                .Select(selector: group => (group.Key.DefinitionId, group.Key.Quantity, group.Count(),
                        group.Key.Condition)).ShouldBe(expected: expected);
    }

    private static void RejectChange(string path, string original, string changed)
    {
        InvalidDataException error = Should.Throw<InvalidDataException>(actual: () => RunDefinitions.Load(
                read: (candidate, limit) => StringComparer.Ordinal.Equals(x: candidate, y: path)
                        ? Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(bytes: ReadFixture(path: candidate,
                                limit: limit)).Replace(oldValue: original, newValue: changed,
                                        comparisonType: StringComparison.Ordinal))
                        : ReadFixture(path: candidate, limit: limit)));
        error.Message.ShouldContain(expected: "RUN_DEFINITION_INVALID " + path);
    }

    private static byte[] ReadFixture(string path, int limit)
    {
        using Stream stream = typeof(FoundationSession).Assembly.GetManifestResourceStream(name: path)
                ?? throw new InvalidOperationException(message: path);
        stream.Length.ShouldBeLessThanOrEqualTo(expected: limit);
        byte[] bytes = new byte[checked((int)stream.Length)];
        stream.ReadExactly(buffer: bytes);
        return bytes;
    }

    private static byte[] ReadVariation(string path, int limit)
    {
        string source = Encoding.UTF8.GetString(bytes: ReadFixture(path: path, limit: limit));
        string changed = path switch
        {
            "content/config/inventory.json" => source
                    .Replace(oldValue: "      10,", newValue: "      4,", comparisonType: StringComparison.Ordinal)
                    .Replace(oldValue: "      10", newValue: "      3", comparisonType: StringComparison.Ordinal)
                    .Replace(oldValue: "\"mainBuildingUnits\": 4", newValue: "\"mainBuildingUnits\": 8",
                            comparisonType: StringComparison.Ordinal),
            "content/config/run.json" => source.Replace(oldValue: "\"quantity\": 200, \"stackQuantity\": 100",
                    newValue: "\"quantity\": 180, \"stackQuantity\": 90", comparisonType: StringComparison.Ordinal),
            "content/config/survivors.json" => source
                    .Replace(oldValue: "\"maximumHealth\": 100", newValue: "\"maximumHealth\": 99.75",
                            comparisonType: StringComparison.Ordinal)
                    .Replace(oldValue: "\"maximumStamina\": 100", newValue: "\"maximumStamina\": 100.125",
                            comparisonType: StringComparison.Ordinal),
            _ => source,
        };
        return Encoding.UTF8.GetBytes(s: changed);
    }
}
