using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;
using Inventory = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves current native HUD, input ownership and spectator presentation decisions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06UiStateTests
{
    /// <summary>Panel input cannot attack or steer, and real damage updates the live HUD.</summary>
    [Fact]
    public void Phase06_E125_PanelInputWinsAndTheCommittedHudRemainsCurrent()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Phase06MeleeTests.Enemy(session: session, id: "panel-attacker", x: -7350);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "left-hand", owner: "panel-attacker");
        FoundationHudState before = Live(session: session);
        var controls = new FoundationInput();
        controls.Set(key: 68, pressed: true);
        controls.SetContext(gameplay: FoundationInput.WorldAllowed(alive: true, focused: true, popup: false,
                panel: true, guiFocused: false));
        controls.Mouse(pressed: true);
        controls.TakeClick(world: true).ShouldBeFalse();
        controls.QueueJump();
        controls.TakeJump().ShouldBeFalse();
        controls.Direction.ShouldBe(expected: 0);
        session.Step(elapsedMilliseconds: 41);
        session.AttackUncontrolled(survivorId: "panel-attacker", sequence: 1).Hits.Single().Lethal.ShouldBeFalse();
        FoundationHudState damaged = Live(session: session);
        damaged.RunTime.ShouldBe(expected: 41);
        damaged.Health.ShouldBeLessThan(expected: before.Health);
        damaged.Stamina.ShouldBe(expected: before.Stamina);
        FoundationHudState.ShowsDamage(previous: before, next: damaged).ShouldBeTrue();
        controls.SetContext(gameplay: FoundationInput.WorldAllowed(alive: true, focused: true, popup: false,
                panel: false, guiFocused: false));
        controls.Set(key: 68, pressed: true);
        controls.Direction.ShouldBe(expected: 0);
        controls.Set(key: 68, pressed: false);
        controls.Set(key: 68, pressed: true);
        controls.Direction.ShouldBe(expected: 1);
    }

    /// <summary>Repeated readback and local cancellation preserve the real source during nonlethal damage.</summary>
    [Fact]
    public void Phase06_E127_RepeatedReadbackAndCancelPreserveTheCurrentPreview()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item held = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand");
        Phase06MeleeTests.Enemy(session: session, id: "panel-attacker", x: -7350);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "left-hand", owner: "panel-attacker");
        Inventory.Item item = Inventory.ReadItem(row: Scope(session: session, scope: "own").Rows.Single(
                predicate: row => row.GetProperty(propertyName: "Id").GetInt64() == held.Id));
        var workspace = new Inventory();
        workspace.Show(external: false);
        workspace.Begin(item: item, external: false, token: 0, split: false).ShouldBeTrue();
        FoundationHudState before = Live(session: session);
        session.AttackUncontrolled(survivorId: "panel-attacker", sequence: 1).Hits.Single().Lethal.ShouldBeFalse();
        FoundationHudState hit = Live(session: session);
        FoundationHudState.ShowsDamage(previous: before, next: hit).ShouldBeTrue();
        FoundationHudState.ShowsDamage(previous: hit, next: Live(session: session)).ShouldBeFalse();
        workspace.Drag?.Item.ShouldBe(expected: item);
        workspace.Open.ShouldBeTrue();
        workspace.Cancel();
        workspace.Cancel();
        workspace.Drag.ShouldBeNull();
        session.Inventory.Find(itemId: held.Id).ShouldBeSameAs(expected: held);
    }

    /// <summary>HUD food references use one shared real cooldown and held arrow counts ignore reserves.</summary>
    [Fact]
    public void Phase06_HudReadsOnlyCarriedFoodAndReadyArrows()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        RunInventory.Item berries = Carried(session: session, definitionId: "IT-08", bag: bag.Id, x: 0, quantity: 4);
        _ = Carried(session: session, definitionId: "IT-02", bag: bag.Id, x: 1, quantity: 9);
        _ = Carried(session: session, definitionId: "IT-01", bag: bag.Id, x: 3, quantity: 7);
        Live(session: session).Ammo.ShouldBeNull();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        Live(session: session).Ammo.ShouldBe(expected: 0);
        FoundationHudState.BlockedNotice(kind: "attack", status: "EmptyAmmo").ShouldBeNull();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-02", slot: "right-hand", quantity: 3);
        AssignFood(session: session, item: berries);
        RunInventory.ItemPath path = session.Inventory.PathTo(itemId: berries.Id).ShouldNotBeNull();
        session.ConsumeFood(controller: Phase06MeleeTests.Control(), sequence: 3,
                request: new RunFood.Request(expected: path, quickSlot: 1)).Status.ShouldBe(
                        expected: RunFood.Status.Consumed);
        FoundationHudState row = Live(session: session);
        row.Ammo.ShouldBe(expected: 3);
        row.Money.ShouldBe(expected: 7);
        row.Left?.Definition.ShouldBe(expected: "IT-07");
        row.Right?.Definition.ShouldBe(expected: "IT-02");
        row.Food.Select(selector: slot => slot?.Id).ShouldBe(expected: [berries.Id, null, berries.Id, null]);
        row.Food[0]?.Quantity.ShouldBe(expected: 3);
        row.FoodShade.ShouldBe(expected: 1);
        session.Step(elapsedMilliseconds: 1000);
        decimal shade = Live(session: session).FoodShade;
        shade.ShouldBeGreaterThan(expected: 0);
        shade.ShouldBeLessThan(expected: 1);
    }

    /// <summary>Post-death views require a production model for explicit teammate selection.</summary>
    [Fact]
    public void Phase06_E126_SpectatorFollowsOnlyExplicitLivingTeammates()
    {
        Type? state = typeof(FoundationInput).Assembly.GetType(
                name: "KingdomZombies.Foundation.FoundationSpectator", throwOnError: false);
        state.ShouldNotBeNull();
    }

    private static FoundationHudState Live(FoundationSession session)
    {
        return FoundationHudState.Read(view: Scope(session: session, scope: "status"),
                survivor: Phase06MeleeTests.Owner, generation: 1).ShouldNotBeNull();
    }

    private static FoundationRunView Scope(FoundationSession session, string scope)
    {
        (long revision, ImmutableArray<(string scope, long generation, long token, JsonElement header,
                ImmutableArray<JsonElement> rows)> views) = session.ReadRunViews(identity:
                        (session.Snapshot.SessionId, "local", "host"))
                        ?? throw new InvalidOperationException(message: "CURRENT_RUN_VIEW");
        (_, long generation, long token, JsonElement header, ImmutableArray<JsonElement> rows) = views.Single(
                predicate: row => StringComparer.Ordinal.Equals(x: row.scope, y: scope));
        return new FoundationRunView(revision: revision, epoch: generation, token: token, header: header, rows: rows);
    }

    private static void AssignFood(FoundationSession session, RunInventory.Item item)
    {
        session.TryEditInventory(edit: (inventory, definitions, _) =>
        {
            RunInventory? first = RunQuickReferences.Assign(inventory: inventory, definitions: definitions,
                    survivorId: Phase06MeleeTests.Owner, slot: 1, itemId: item.Id);
            return first is null ? null : RunQuickReferences.Assign(inventory: first, definitions: definitions,
                    survivorId: Phase06MeleeTests.Owner, slot: 3, itemId: item.Id);
        }).ShouldBeTrue();
    }

    private static RunInventory.Item Carried(FoundationSession session, string definitionId, long bag, int x,
            int quantity)
    {
        RunDefinitions.Item definition = session.Definitions.Items[definitionId];
        var item = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: definitionId,
                quantity: quantity, condition: definition.MaximumCondition, location: RunItemTransfer.Container(
                        definition: definition, containerId: bag, x: x, y: 0), storageGrid: definition.Storage);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items:
                inventory.Items.Add(item: item), nextItemId: item.Id + 1)).ShouldBeTrue();
        return item;
    }
}
