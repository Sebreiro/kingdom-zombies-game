using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies detached authoritative run snapshots and their continuation.</summary>
[Trait("Tier", "1")]
public sealed class Phase06SnapshotTests
{
    /// <summary>Neighboring equal stacks keep their individual identities and clock revision.</summary>
    [Fact]
    public void Phase06_E118_CapturesOneDetachedRunWithExactElapsedTime()
    {
        FoundationSession session = Phase06InventoryTests.Start();
        session.Step(elapsedMilliseconds: 123);
        RunInventory.Item left = Phase06InventoryTests.Item(session: session, definitionId: "IT-01");
        RunInventory.Item right = Phase06InventoryTests.Item(session: session, definitionId: "IT-01", ordinal: 1);
        byte[] data = session.CaptureRunSnapshot();
        session.Step(elapsedMilliseconds: 1);
        FoundationSession restored = Load(data: data);
        restored.LocalWorld.elapsedMilliseconds.ShouldBe(expected: 123);
        Phase06InventoryTests.Find(session: restored, id: left.Id).ShouldBe(expected: left);
        Phase06InventoryTests.Find(session: restored, id: right.Id).ShouldBe(expected: right);
        restored.CaptureRunSnapshot().ShouldBe(expected: data);
    }

    /// <summary>Saved grid extents keep an outlying placement after the configured grid narrows.</summary>
    [Fact]
    public void Phase06_E119_RestorePreservesSavedExtentsAndLegalExtrema()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item sourceItem = Phase06InventoryTests.Seed(session: source, definitionId: "IT-01");
        var cell = (RunInventory.StashCell)sourceItem.Location;
        RunInventory.StashCell outer = Phase06InventoryTests.Cell(session: source, item: sourceItem, page: cell.Page,
                x: 9, y: 9);
        Phase06InventoryTests.Move(session: source, source: sourceItem, target: outer).ShouldBeTrue();
        RunInventory.Item bow = Phase06InventoryTests.Seed(session: source, definitionId: "IT-07");
        var bowCell = (RunInventory.StashCell)bow.Location;
        RunInventory.StashCell rotated = Phase06InventoryTests.Cell(session: source, item: bow,
                page: bowCell.Page, x: 6, y: 8, rotated: true);
        Phase06InventoryTests.Move(session: source, source: bow, target: rotated).ShouldBeTrue();
        source.TryAddHostile(id: "limit", position: 59000, facing: -1, health: 100).ShouldBeTrue();
        FoundationSession current = StartWithChangedGeometry();
        current.Definitions.StashGrid.Width.ShouldBe(expected: 7);
        current.TryRestoreRunSnapshot(data: source.CaptureRunSnapshot(), error: out string? error)
                .ShouldBeTrue(customMessage: error);
        current.Inventory.StashGrid.ShouldBe(expected: new RunDefinitions.Size(width: 10, height: 10));
        RunSavedLayout layout = current.Inventory.SavedLayout.ShouldNotBeNull();
        layout.LegacyStash.ShouldBeTrue();
        layout.Items[bow.Id].legacy.ShouldBeTrue();
        Phase06InventoryTests.Find(session: current, id: bow.Id).Location.ShouldBe(expected: rotated);
        Phase06InventoryTests.Find(session: current, id: sourceItem.Id).Location.ShouldBe(expected: outer);
        current.Population.Survivors.Single(predicate: survivor => survivor.Hostile).PositionSubunitsExact
                .ShouldBe(expected: 59000);
        Load(data: current.CaptureRunSnapshot()).Inventory.StashGrid.ShouldBe(expected: current.Inventory.StashGrid);
    }

    /// <summary>Empty and single-item inventories keep their exact records and next identity.</summary>
    [Fact]
    public void Phase06_E121_RestoresEmptyAndSingleCollections()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item item = Phase06InventoryTests.Item(session: source, definitionId: "IT-01");
        long next = source.Inventory.NextItemId;
        source.TryEditInventory(edit: (current, _, _) => current.WithItems(items: [item])).ShouldBeTrue();
        FoundationSession single = Load(data: source.CaptureRunSnapshot());
        single.Inventory.Items.Single().ShouldBe(expected: item);
        single.Inventory.NextItemId.ShouldBe(expected: next);
        source.TryEditInventory(edit: (current, _, _) => current.WithItems(items: [])).ShouldBeTrue();
        FoundationSession empty = Load(data: source.CaptureRunSnapshot());
        empty.Inventory.Items.IsEmpty.ShouldBeTrue();
        empty.Inventory.NextItemId.ShouldBe(expected: next);
        empty.UnderInventoryAuthority(action: root => root?.Combat.Arrows.IsEmpty).ShouldBe(expected: true);
    }

    /// <summary>A contested transfer and capture publish matching coordinates and host revisions.</summary>
    [Fact]
    public void Phase06_E120_ConcurrentCaptureMatchesExactlyOneCommittedTransferRevision()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item item = Phase06InventoryTests.Seed(session: source, definitionId: "IT-01");
        var before = (RunInventory.StashCell)item.Location;
        RunInventory.StashCell after = Phase06InventoryTests.Cell(session: source, item: item, page: before.Page, x: 2);
        long revision = source.Snapshot.Revision;
        byte[]? captured = null;
        Phase06InventoryTests.Race(first: () =>
        {
            captured = source.CaptureRunSnapshot();
            return true;
        }, second: () => Phase06InventoryTests.Move(session: source, source: item, target: after))
                .ShouldBe(expected: 2);
        FoundationSession restored = Load(data: captured.ShouldNotBeNull());
        restored.Snapshot.Revision.ShouldBeInRange(from: revision, to: revision + 1);
        Phase06InventoryTests.Find(session: restored, id: item.Id).Location.ShouldBe(
                expected: restored.Snapshot.Revision == revision ? before : after);
        restored.Inventory.Items.Count(predicate: row => row.Id == item.Id).ShouldBe(expected: 1);
    }

    /// <summary>Duplicate fields cannot replace the active root, even with the same value.</summary>
    [Fact]
    public void Phase06_E122_RejectsAmbiguousRecordsWithoutChangingTheRoot()
    {
        FoundationSession session = Phase06InventoryTests.Start();
        byte[] baseline = session.CaptureRunSnapshot();
        byte[] duplicate = Encoding.UTF8.GetBytes(s:
                Encoding.UTF8.GetString(bytes: baseline).Insert(startIndex: 1, value: "\"Version\":1,"));
        AssertRejected(session: session, data: duplicate, baseline: baseline);
        CheckMalformedCandidates(session: session, baseline: baseline);
        CheckProjectileAndFoodReplay();
    }

    /// <summary>Stored order keys, receipts and random progress continue across reordered records.</summary>
    [Fact]
    public void Phase06_E123_ShuffledRecordsKeepPendingSlotsAndRandomContinuation()
    {
        CheckPendingOrder();
        CheckRandomContinuation();
    }

    /// <summary>Precise state and whole quantities survive through detached bytes.</summary>
    [Fact]
    public void Phase06_E124_KeepsFractionalConditionAndVitalsWithIntegralCounts()
    {
        FoundationSession source = Phase06InventoryTests.Start();
        RunInventory.Item weapon = Phase06InventoryTests.Seed(session: source, definitionId: "IT-07",
                condition: 17.123456789m);
        RunInventory.Item coins = Phase06InventoryTests.Item(session: source, definitionId: "IT-01");
        source.TryUpdateVitals(survivorId: Phase06InventoryTests.Survivor, health: 88.123456789m,
                stamina: 62.123456789m).ShouldBeTrue();
        FoundationSession restored = Load(data: source.CaptureRunSnapshot());
        Phase06InventoryTests.Find(session: restored, id: weapon.Id).Condition.ShouldBe(expected: 17.123456789m);
        Phase06InventoryTests.Find(session: restored, id: coins.Id).Quantity.ShouldBe(expected: coins.Quantity);
        restored.Population.Survivors[0].Health.ShouldBe(expected: 88.123456789m);
        restored.Population.Survivors[0].Stamina.ShouldBe(expected: 62.123456789m);
    }

    /// <summary>Active sprint, bow recovery and flinch restore into the exact same public appearance.</summary>
    [Fact]
    public void RunSnapshot_ActiveCues_PreserveAppearanceWithoutReplay()
    {
        FoundationSession source = Phase06MeleeTests.Start();
        RunItemActions.Controller controller = Phase06MeleeTests.Control();
        source.TryAcceptMotion(controller: controller, sequence: 1, input: new RunLocomotion.Input(
                right: true, sprint: true)).accepted.ShouldBeTrue();
        source.Step(elapsedMilliseconds: 100);
        Phase06MeleeTests.Enemy(session: source, id: "flinch", x: -7000);
        source.AttackUncontrolled(survivorId: "flinch", sequence: 1).Hits.Single().TargetId
                .ShouldBe(expected: Phase06MeleeTests.Owner);
        _ = Phase06MeleeTests.Equip(session: source, id: "IT-07", slot: "left-hand");
        _ = Phase06MeleeTests.Equip(session: source, id: "IT-02", slot: "right-hand");
        var aim = new RunCombat.Aim(x: 2000.25, y: 321.5);
        source.Attack(controller: controller, sequence: 2, aim: aim).Status
                .ShouldBe(expected: RunCombat.Status.Accepted);
        JsonElement world = source.ReadRunPublic().ShouldNotBeNull().world;
        JsonElement pose = world.GetProperty(propertyName: "Actors")[0];
        pose.GetProperty(propertyName: "Movement").GetString().ShouldBe(expected: "faster");
        pose.GetProperty(propertyName: "StrikeAt").GetDecimal().ShouldBe(expected: 100);
        pose.GetProperty(propertyName: "AimX").GetDouble().ShouldBe(expected: aim.X);
        pose.GetProperty(propertyName: "FlinchAt").GetDecimal().ShouldBe(expected: 100);

        byte[] captured = source.CaptureRunSnapshot();
        FoundationSession restored = Load(data: captured);
        AssertSameProjection(expected: world, actual: restored.ReadRunPublic().ShouldNotBeNull().world);
        restored.Attack(controller: controller, sequence: 2, aim: aim).Replayed.ShouldBeTrue();
        restored.CaptureRunSnapshot().ShouldBe(expected: captured);
        RejectEdit(session: restored, baseline: captured, edit: root =>
                Member(root: Member(root: root, key: "Population"), key: "Survivors")[0]
                        .ShouldNotBeNull()["FlinchAt"] = 101);
    }

    private static void AssertSameProjection(JsonElement expected, JsonElement actual)
    {
        foreach (JsonProperty field in expected.EnumerateObject())
        {
            JsonElement restored = actual.GetProperty(propertyName: field.Name);
            if (field.Value.ValueKind == JsonValueKind.Array)
            {
                string[] RecordSet(JsonElement records)
                {
                    return [.. records.EnumerateArray().Select(selector: row =>
                            row.GetRawText()).Order(comparer: StringComparer.Ordinal)];
                }

                RecordSet(records: restored).ShouldBe(expected: RecordSet(records: field.Value));
            }
            else
            {
                JsonElement.DeepEquals(element1: restored, element2: field.Value).ShouldBeTrue();
            }
        }
    }

    private static FoundationSession Load(byte[] data)
    {
        FoundationSession restored = Phase06InventoryTests.Start();
        restored.TryRestoreRunSnapshot(data: data, error: out string? error).ShouldBeTrue(customMessage: error);
        return restored;
    }

    private static void CheckMalformedCandidates(FoundationSession session, byte[] baseline)
    {
        RejectEdit(session: session, baseline: baseline, edit: root => root["Version"] = 2);
        RejectEdit(session: session, baseline: baseline, edit: root => root["Elapsed"] = -1);
        RejectEdit(session: session, baseline: baseline, edit: root => root["Extra"] = 1);
        RejectEdit(session: session, baseline: baseline, edit: root => _ = root.Remove(propertyName: "Succession"));
        RejectEdit(session: session, baseline: baseline, edit: root => root["World"] = null);
        RejectEdit(session: session, baseline: baseline, edit: root => Items(root: root).Add(item: null));
        RejectEdit(session: session, baseline: baseline, edit: root =>
                Items(root: root).Add(item: Items(root: root)[0].ShouldNotBeNull().DeepClone()));
        RejectEdit(session: session, baseline: baseline, edit: root =>
                Items(root: root)[0].ShouldNotBeNull()["DefinitionId"] = "missing");
        RejectEdit(session: session, baseline: baseline, edit: root =>
                Items(root: root)[0].ShouldNotBeNull()["Quantity"] = 1.5m);
        RejectEdit(session: session, baseline: baseline, edit: SetContainerCycle);
        RejectEdit(session: session, baseline: baseline, edit: root =>
                Member(root: Member(root: root, key: "Population"), key: "Leases")[0]
                        .ShouldNotBeNull()["SurvivorId"] = "absent");
        RejectEdit(session: session, baseline: baseline, edit: root =>
                Member(root: root, key: "Succession")["DrawPosition"] = 1_000_001L);
    }

    private static void SetContainerCycle(JsonObject root)
    {
        JsonNode item = Items(root: root).First(predicate: row =>
                Member(root: row.ShouldNotBeNull(), key: "DefinitionId").GetValue<string>() is "IT-12")
                .ShouldNotBeNull();
        JsonNode location = Member(root: item, key: "Location");
        location["Kind"] = 4;
        location["TargetId"] = null;
        location["Slot"] = null;
        location["GridId"] = Member(root: item, key: "Id").DeepClone();
        location["X"] = 0;
        location["Y"] = 0;
        location["Rotated"] = false;
        location["Footprint"] = Member(root: Member(root: item, key: "Layout"), key: "Footprint").DeepClone();
    }

    private static void RejectEdit(FoundationSession session, byte[] baseline, Action<JsonObject> edit)
    {
        JsonObject changes = JsonNode.Parse(utf8Json: baseline).ShouldNotBeNull().AsObject();
        edit(obj: changes);
        AssertRejected(session: session, data: Encoding.UTF8.GetBytes(s: changes.ToJsonString()), baseline: baseline);
    }

    private static void AssertRejected(FoundationSession session, byte[] data, byte[] baseline)
    {
        byte[] source = [.. data];
        FoundationSession.RunPublication? before = session.UnderInventoryAuthority(action: root => root);
        session.TryRestoreRunSnapshot(data: data, error: out string? error).ShouldBeFalse();
        error.ShouldNotBeNullOrEmpty();
        session.UnderInventoryAuthority(action: root => root).ShouldBeSameAs(expected: before);
        session.CaptureRunSnapshot().ShouldBe(expected: baseline);
        data.ShouldBe(expected: source);
    }

    private static void CheckProjectileAndFoodReplay()
    {
        FoundationSession source = Phase06MeleeTests.Start();
        RunItemActions.Controller controller = Phase06MeleeTests.Control();
        RunFood.Request food = ConsumeOne(source: source, controller: controller);
        Phase06MeleeTests.Enemy(session: source, id: "arrow-target", x: 1000.123m, health: 1);
        RunInventory.Item bow = Phase06MeleeTests.Equip(session: source, id: "IT-07", slot: "left-hand");
        RunInventory.Item ammo = Phase06MeleeTests.Equip(session: source, id: "IT-02", slot: "right-hand");
        var aim = new RunCombat.Aim(x: 2000, y: 0);
        source.Attack(controller: controller, sequence: 8, aim: aim).Status
                .ShouldBe(expected: RunCombat.Status.Accepted);
        byte[] flight = source.CaptureRunSnapshot();
        FoundationSession restored = Load(data: flight);
        restored.Attack(controller: controller, sequence: 8, aim: aim).Replayed.ShouldBeTrue();
        restored.ConsumeFood(controller: controller, sequence: 7, request: food).Replayed.ShouldBeTrue();
        restored.Inventory.Find(itemId: ammo.Id).ShouldBeNull();
        restored.Inventory.Find(itemId: bow.Id)?.Condition.ShouldBe(expected: bow.Condition - 1);
        restored.CaptureRunSnapshot().ShouldBe(expected: flight);
        source.Step(elapsedMilliseconds: 1000);
        restored.Step(elapsedMilliseconds: 1000);
        byte[] impact = restored.CaptureRunSnapshot();
        impact.ShouldBe(expected: source.CaptureRunSnapshot());
        RunDeathAndLoot.State loot = Phase06DeathTests.Loot(session: restored);
        loot.Deaths.Count.ShouldBe(expected: 1);
        RunDeathAndLoot.Death death = loot.Deaths["arrow-target"];
        (death.At != decimal.Truncate(d: death.At)).ShouldBeTrue();
        loot.Targets[death.CorpseId].EmptySince.ShouldBe(expected: death.At);
        restored.TryRestoreRunSnapshot(data: impact, error: out string? error).ShouldBeTrue(customMessage: error);
        long remaining = decimal.ToInt64(d: decimal.Ceiling(d: death.At + restored.Definitions.LootEmptyDelay))
                - restored.LocalWorld.elapsedMilliseconds;
        restored.Step(elapsedMilliseconds: remaining);
        Phase06DeathTests.Loot(session: restored).Targets.ContainsKey(key: death.CorpseId).ShouldBeFalse();
        Phase06DeathTests.Loot(session: restored).Deaths.Count.ShouldBe(expected: 1);
        byte[] cleaned = restored.CaptureRunSnapshot();
        Load(data: cleaned).CaptureRunSnapshot().ShouldBe(expected: cleaned);
    }

    private static RunFood.Request ConsumeOne(FoundationSession source, RunItemActions.Controller controller)
    {
        RunInventory.Item pocket = source.Inventory.Equipped(survivorId: Phase06MeleeTests.Owner, slot: "legs")
                .ShouldNotBeNull();
        RunInventory.Item food = Phase06InventoryTests.Seed(session: source, definitionId: "IT-10", quantity: 2);
        RunInventory.ContainerCell cell = RunItemTransfer.Container(definition: source.Definitions.Items["IT-10"],
                containerId: pocket.Id, x: 0, y: 0);
        Phase06InventoryTests.Move(session: source, source: food, target: cell).ShouldBeTrue();
        var request = new RunFood.Request(expected: source.Inventory.PathTo(itemId: food.Id).ShouldNotBeNull());
        source.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 50, stamina: 100).ShouldBeTrue();
        source.ConsumeFood(controller: controller, sequence: 7, request: request).Status
                .ShouldBe(expected: RunFood.Status.Consumed);
        return request;
    }

    private static void CheckPendingOrder()
    {
        FoundationSession source = StartPopulation(count: 4);
        _ = Phase06DeathTests.Join(session: source);
        _ = Phase06DeathTests.Join(session: source, binding: "leave");
        _ = Phase06DeathTests.Join(session: source, binding: "final");
        AddKiller(session: source);
        _ = Phase06MeleeTests.Equip(session: source, id: "IT-03", slot: "left-hand", owner: "killer");
        _ = Phase06MeleeTests.Equip(session: source, id: "IT-04", slot: "right-hand", owner: "killer");
        source.TryUpdateVitals(survivorId: "survivor-02", health: 1, stamina: 100).ShouldBeTrue();
        Kill(session: source, survivor: "survivor-01", sequence: 1);
        RunSuccession.Request guest = Respawn(binding: "guest", epoch: 2);
        RunSuccession.Request host = Respawn(binding: "local", epoch: 1);
        source.RequestRespawns(requests: [guest, host]).Select(selector: result => result.Status)
                .ShouldBe(expected: [RunSuccession.Status.Waiting, RunSuccession.Status.Waiting]);
        byte[] saved = source.CaptureRunSnapshot();
        byte[] shuffled = Shuffle(data: saved);
        FoundationSession restored = Load(data: shuffled);
        restored.CaptureRunSnapshot().ShouldBe(expected: saved);
        restored.RequestRespawns(requests: [guest, host]).All(predicate: result => result.Replayed).ShouldBeTrue();
        restored.RemoveParticipant(bindingId: "leave").ShouldBeTrue();
        restored.ReadSuccession(bindingId: "local")?.SurvivorId.ShouldBe(expected: "survivor-03");
        restored.ReadSuccession(bindingId: "guest")?.Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        restored.RemoveParticipant(bindingId: "final").ShouldBeTrue();
        restored.ReadSuccession(bindingId: "guest")?.SurvivorId.ShouldBe(expected: "survivor-04");
    }

    private static void CheckRandomContinuation()
    {
        FoundationSession source = StartPopulation(count: 5);
        AddKiller(session: source);
        Kill(session: source, survivor: "survivor-01", sequence: 1);
        source.RequestRespawns(requests: [Respawn(binding: "local", epoch: 1)]).Single().Status
                .ShouldBe(expected: RunSuccession.Status.Assigned);
        RunSuccession.View first = source.ReadSuccession(bindingId: "local").ShouldNotBeNull();
        long draw = source.UnderInventoryAuthority(action: root => root?.Succession.DrawPosition)
                .ShouldNotBeNull();
        draw.ShouldBeGreaterThan(expected: 0);
        byte[] snapshot = source.CaptureRunSnapshot();
        FoundationSession restored = Load(data: Shuffle(data: snapshot));
        restored.TryRestoreRunSnapshot(data: snapshot, error: out string? error).ShouldBeTrue(customMessage: error);
        restored.CaptureRunSnapshot().ShouldBe(expected: snapshot);
        string next = first.SurvivorId.ShouldNotBeNull();
        long generation = first.Generation.ShouldNotBeNull();
        PutSuccessorInFront(session: source, generation: generation);
        PutSuccessorInFront(session: restored, generation: generation);
        RunSuccession.Request request = Respawn(binding: "local", epoch: generation);
        Kill(session: source, survivor: next, sequence: 2);
        Kill(session: restored, survivor: next, sequence: 2);
        source.RequestRespawns(requests: [request]).Single().Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        restored.RequestRespawns(requests: [request]).Single().Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        restored.CaptureRunSnapshot().ShouldBe(expected: source.CaptureRunSnapshot());
    }

    private static FoundationSession StartPopulation(int count)
    {
        RunDefinitions baseline = RunDefinitions.Default;
        var start = new RunDefinitions.Start(survivorCount: count, playerLimit: 4, role: "woodcutter",
                outfit: baseline.StartingOutfit, stash: []);
        var vitals = new RunDefinitions.SurvivorSettings(maximumHealth: 100, maximumStamina: 100,
                refugeUnits: 12, dangerUnits: 12);
        var inventory = new RunDefinitions.InventorySettings(stashGrid: baseline.StashGrid,
                maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4);
        var definitions = new RunDefinitions(start: start, survivors: vitals, inventory: inventory,
                items: baseline.Items, food: baseline.Food, combat: baseline.Combat);
        return new FoundationSession(sessionId: "combat", playerId: "host", actorId: "actor",
                world: RunWorld.Foundation, definitions: definitions);
    }

    private static FoundationSession StartWithChangedGeometry()
    {
        RunDefinitions baseline = RunDefinitions.Default;
        var start = new RunDefinitions.Start(survivorCount: baseline.SurvivorCount, playerLimit: 4,
                role: baseline.InitialRole, outfit: baseline.StartingOutfit, stash: []);
        var vitals = new RunDefinitions.SurvivorSettings(maximumHealth: baseline.MaximumHealth,
                maximumStamina: baseline.MaximumStamina, refugeUnits: baseline.Ranges.RefugeUnits,
                dangerUnits: baseline.Ranges.DangerUnits);
        var inventory = new RunDefinitions.InventorySettings(stashGrid: new RunDefinitions.Size(width: 7, height: 10),
                maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4);
        RunDefinitions.Item old = baseline.Items["IT-07"];
        var bow = new RunDefinitions.Item(id: old.Id, footprint: new RunDefinitions.Size(width: 1, height: 3),
                maxStack: old.MaxStack, weightKilograms: old.WeightKilograms, rotatable: old.Rotatable,
                slot: old.Slot, storage: old.Storage, maximumCondition: old.MaximumCondition,
                appearance: old.Appearance);
        var definitions = new RunDefinitions(start: start, survivors: vitals, inventory: inventory,
                items: baseline.Items.SetItem(key: bow.Id, value: bow), food: baseline.Food, combat: baseline.Combat);
        return new FoundationSession(sessionId: "run", playerId: "host", actorId: "host-wire",
                world: RunWorld.Foundation, definitions: definitions);
    }

    private static void PutSuccessorInFront(FoundationSession session, long generation)
    {
        var control = new RunItemActions.Controller(sessionId: "combat", bindingId: "local", generation: generation);
        session.TryAcceptMotion(controller: control, sequence: 1, input: new RunLocomotion.Input(right: true))
                .accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: 100);
    }

    private static void AddKiller(FoundationSession session)
    {
        Phase06MeleeTests.Enemy(session: session, id: "killer", x: -7500);
    }

    private static void Kill(FoundationSession session, string survivor, long sequence)
    {
        session.TryUpdateVitals(survivorId: survivor, health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "killer", sequence: sequence).Hits.Any(predicate: hit =>
                hit.Lethal && StringComparer.Ordinal.Equals(x: hit.TargetId, y: survivor)).ShouldBeTrue();
        session.Step(elapsedMilliseconds: 1000);
    }

    private static RunSuccession.Request Respawn(string binding, long epoch)
    {
        return new RunSuccession.Request(sessionId: "combat", bindingId: binding, deathGeneration: epoch, sequence: 1);
    }

    private static byte[] Shuffle(byte[] data)
    {
        JsonObject root = JsonNode.Parse(utf8Json: data).ShouldNotBeNull().AsObject();
        foreach (JsonNode parent in new[] { root, Member(root: root, key: "Inventory"),
                Member(root: root, key: "Population"), Member(root: root, key: "Loot"),
                Member(root: root, key: "Succession") })
        {
            foreach (JsonArray rows in parent.AsObject().Select(selector: property => property.Value)
                    .OfType<JsonArray>())
            {
                JsonNode?[] reverse = [.. rows.Reverse().Select(selector: row => row?.DeepClone())];
                rows.Clear();
                foreach (JsonNode? row in reverse)
                {
                    rows.Add(item: row);
                }
            }
        }

        return Encoding.UTF8.GetBytes(s: root.ToJsonString());
    }

    private static JsonArray Items(JsonObject root)
    {
        return Member(root: Member(root: root, key: "Inventory"), key: "Items").AsArray();
    }

    private static JsonNode Member(JsonNode root, string key)
    {
        return root[key].ShouldNotBeNull();
    }
}
