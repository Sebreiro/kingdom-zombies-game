using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies the production mapping from committed run appearances to authored layers.</summary>
[Trait("Tier", "1")]
public sealed class Phase06AppearanceTests
{
    /// <summary>Living and dead gear must be projected through the native composition contract.</summary>
    [Fact]
    public void Phase06_E099_ProjectsWornGearAndCosmeticCrown()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-18", slot: "headwear", condition: 0);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "right-hand", condition: 0);
        FoundationEquipmentProjection.Frame frame = Read(session: session);
        FoundationEquipmentProjection.Subject bound = Owner(frame: frame);
        FoundationEquipmentProjection.Subject neighbor = frame.Actors.Single(predicate: actor =>
                actor.Id is "survivor-02");
        bound.Crowned.ShouldBeTrue();
        neighbor.Crowned.ShouldBeFalse();
        bound.Layers.Select(selector: layer => (layer.Slot, layer.Item, layer.Broken)).ShouldBe(expected:
                [("headwear", "IT-18", true), ("torso", "IT-11", false), ("legs", "IT-12", false),
                    ("boots", "IT-17", false), ("left", "IT-03", false), ("right", "IT-03", true)]);
        JsonElement wear = FoundationEquipmentProjection.Wear(subject: bound);
        wear.GetProperty(propertyName: "headwear").GetString().ShouldBe(expected: "IT-18");
        wear.GetProperty(propertyName: "crown").GetString().ShouldBe(expected: "K21");
        session.Inventory.Items.Any(predicate: item => item.DefinitionId is "K21").ShouldBeFalse();
    }

    /// <summary>Racing complete frames converge on the current authoritative wear for both hands.</summary>
    [Fact]
    public async Task Phase06_E100_CompetingRevisionsConvergeOnCurrentGear()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "right-hand");
        (FoundationSnapshot roster, JsonElement world) before = Public(session: session);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand", condition: 0);
        (FoundationSnapshot roster, JsonElement world) after = Public(session: session);
        var cursor = new FoundationActorNode.AppearanceCursor();
        Task<bool> old = Task.Run(function: () => cursor.Install(revision: before.roster.Revision,
                world: before.world), cancellationToken: TestContext.Current.CancellationToken);
        Task<bool> current = Task.Run(function: () => cursor.Install(revision: after.roster.Revision,
                world: after.world), cancellationToken: TestContext.Current.CancellationToken);
        _ = await Task.WhenAll(tasks: [old, current]);
        (await current).ShouldBeTrue();
        FoundationEquipmentProjection.Frame frame = cursor.Current.ShouldNotBeNull();
        frame.Revision.ShouldBe(expected: after.roster.Revision);
        Owner(frame: frame).Left.Broken.ShouldBeTrue();
        Owner(frame: frame).Right.Item.ShouldBe(expected: "IT-04");
    }

    /// <summary>Zero actual gear stays empty and worn bags omit every nested pouch layer.</summary>
    [Fact]
    public void Phase06_E101_EmptyActorAndNestedPouchDoNotGainVisibleItems()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                [.. inventory.Items.Where(predicate: item => item.Location
                        is not RunInventory.Equipment { SurvivorId: Phase06MeleeTests.Owner })],
                nextItemId: inventory.NextItemId)).ShouldBeTrue();
        FoundationEquipmentProjection.Frame fresh = Read(session: session);
        Owner(frame: fresh).Layers.ShouldBeEmpty();
        fresh.Arrows.ShouldBeEmpty();
        fresh.Corpses.ShouldBeEmpty();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        AddPouch(session: session, bag: bag);
        FoundationEquipmentProjection.Subject worn = Owner(frame: Read(session: session));
        worn.Layers.Single().Slot.ShouldBe(expected: "backpack");
        worn.Layers.Single().Item.ShouldBe(expected: "IT-23");
        FoundationEquipmentProjection.Fits(subject: worn).EnumerateObject().Count().ShouldBe(expected: 1);
    }

    /// <summary>A repeated fatal receipt renders one enduring corpse with its existing broken gear.</summary>
    [Fact]
    public void Phase06_E102_ReplayedDeathKeepsOneUncrownedCorpse()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand", condition: 0);
        Phase06MeleeTests.Enemy(session: session, id: "enemy", x: -7000);
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 5, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();
        (FoundationSnapshot roster, JsonElement world) death = Public(session: session);
        var cursor = new FoundationActorNode.AppearanceCursor();
        cursor.Install(revision: death.roster.Revision, world: death.world).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "enemy", sequence: 1).Replayed.ShouldBeTrue();
        cursor.Install(revision: death.roster.Revision, world: death.world).ShouldBeFalse();
        session.Step(elapsedMilliseconds: 180001);
        FoundationEquipmentProjection.Frame settled = Read(session: session);
        FoundationEquipmentProjection.Subject corpse = settled.Corpses.Single();
        corpse.Left.Broken.ShouldBeTrue();
        corpse.Crowned.ShouldBeFalse();
        corpse.DiedAt.ShouldBe(expected: 0);
        settled.Actors.Any(predicate: actor => actor.Id is Phase06MeleeTests.Owner).ShouldBeFalse();
        FoundationEquipmentProjection.Wear(subject: corpse).TryGetProperty(propertyName: "crown", value: out _)
                .ShouldBeFalse();
    }

    /// <summary>Authored layers preserve logical hands and reject later frames from an older run clock.</summary>
    [Fact]
    public void Phase06_E103_OrderedLayersPreserveLogicalMirroringAndRejectRegressedTime()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        (FoundationSnapshot roster, JsonElement world) original = Public(session: session);
        session.Step(elapsedMilliseconds: 10);
        (FoundationSnapshot roster, JsonElement world) moved = Public(session: session);
        var cursor = new FoundationActorNode.AppearanceCursor();
        cursor.Install(revision: moved.roster.Revision, world: moved.world).ShouldBeTrue();
        cursor.Install(revision: moved.roster.Revision + 1, world: original.world).ShouldBeFalse();
        FoundationEquipmentProjection.Frame installed = cursor.Current.ShouldNotBeNull();
        using var invalid = JsonDocument.Parse(json: moved.world.GetRawText().Replace(
                oldValue: "\"Movement\":\"idle\"", newValue: "\"Movement\":\"unknown\"",
                comparisonType: StringComparison.Ordinal));
        Should.Throw<InvalidDataException>(actual: () => cursor.Install(revision: moved.roster.Revision + 1,
                world: invalid.RootElement));
        cursor.Current.ShouldBeSameAs(expected: installed);
        FoundationEquipmentProjection.Subject actor = Owner(frame: installed);
        actor.Layers.Select(selector: layer => layer.Slot)
                .ShouldBe(expected: ["torso", "legs", "boots", "left", "right"]);
        actor.Left.Item.ShouldBe(expected: "IT-07");
        actor.Right.Item.ShouldBe(expected: "IT-02");
        actor.ArrowReady.ShouldBeTrue();
        (double angle, int facing) left = FoundationEquipmentProjection.BowAngle(dx: -2, dy: 2, facing: 1);
        (double angle, int facing) right = FoundationEquipmentProjection.BowAngle(dx: 2, dy: 2, facing: -1);
        (left.facing, right.facing).ShouldBe(expected: (-1, 1));
        left.angle.ShouldBe(expected: right.angle);
    }

    private static (FoundationSnapshot roster, JsonElement world) Public(FoundationSession session)
    {
        return session.ReadRunPublic() ?? throw new InvalidOperationException(message: "RUN_APPEARANCE_FIXTURE");
    }

    private static FoundationEquipmentProjection.Frame Read(FoundationSession session)
    {
        (FoundationSnapshot roster, JsonElement world) publication = Public(session: session);
        return FoundationEquipmentProjection.Read(revision: publication.roster.Revision, world: publication.world);
    }

    private static FoundationEquipmentProjection.Subject Owner(FoundationEquipmentProjection.Frame frame)
    {
        return frame.Actors.Single(predicate: actor => actor.Id is Phase06MeleeTests.Owner);
    }

    private static void AddPouch(FoundationSession session, RunInventory.Item bag)
    {
        RunDefinitions.Item rule = session.Definitions.Items["IT-22"];
        RunInventory.ContainerCell location = RunItemTransfer.Container(definition: rule, containerId: bag.Id,
                x: 0, y: 0);
        var pouch = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: rule.Id, quantity: 1,
                condition: rule.MaximumCondition, location: location, storageGrid: rule.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items: inventory.Items.Add(item: pouch),
                nextItemId: checked(pouch.Id + 1))).ShouldBeTrue();
    }
}
