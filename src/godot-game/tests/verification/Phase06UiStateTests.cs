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
    public void Hud_ReservesPresent_ReadsOnlyCarriedFoodAndReadyArrows()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        RunInventory.Item berries = Carried(session: session, definitionId: "IT-08", bag: bag.Id, x: 0, quantity: 4);
        _ = Carried(session: session, definitionId: "IT-02", bag: bag.Id, x: 1, quantity: 9);
        _ = Carried(session: session, definitionId: "IT-01", bag: bag.Id, x: 3, quantity: 7);
        Live(session: session).Ammo.ShouldBeNull();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-07", slot: "left-hand");
        FoundationHudState unloaded = Live(session: session);
        unloaded.Ammo.ShouldBe(expected: 0);
        unloaded.QuietEmptyHand(left: false).ShouldBeTrue();
        unloaded.QuietEmptyHand(left: true).ShouldBeFalse();
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

    /// <summary>Zero, one and several authorized peers preserve an explicit, stable follow choice.</summary>
    [Fact]
    public void Phase06_E126_SpectatorFollowsOnlyExplicitLivingTeammates()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        var state = new FoundationSpectator();
        state.Observe(readback: Readback(session: session), binding: "local", now: 0, center: -128, span: 1280);
        KillPlayer(session: session);
        state.Observe(readback: Readback(session: session), binding: "local", now: 1, center: -128, span: 1280);
        state.Visible.ShouldBeTrue();
        state.CanDrive.ShouldBeFalse();
        state.Camera.ShouldBe(expected: -128);
        state.Teammates.ShouldBeEmpty();
        state.Cycle(previous: false).ShouldBeFalse();
        Join(session: session, ordinal: 2);
        state.Observe(readback: Readback(session: session), binding: "local", now: 2, center: -128, span: 1280);
        state.Followed.ShouldBeNull();
        state.Cycle(previous: false).ShouldBeTrue();
        state.Cycle(previous: false).ShouldBeTrue();
        state.Followed?.Slot.ShouldBe(expected: 2);
        Join(session: session, ordinal: 3);
        state.Observe(readback: Readback(session: session), binding: "local", now: 3, center: -128, span: 1280);
        state.Teammates.Select(selector: option => option.Slot).ShouldBe(expected: [2, 3]);
        state.Cycle(previous: false).ShouldBeTrue();
        state.Followed?.Slot.ShouldBe(expected: 3);
        FoundationSpectator.Teammate departed = state.Followed.ShouldNotBeNull();
        session.RemoveParticipant(bindingId: departed.Binding).ShouldBeTrue();
        state.Observe(readback: Readback(session: session), binding: "local", now: 4, center: -128, span: 1280);
        state.Followed.ShouldBeNull();
        state.Camera.ShouldBe(expected: -128);
        state.FollowNotice.ShouldContain(expected: "no longer available");
        state.Follow(choice: departed).ShouldBeFalse();
        state.Cycle(previous: true).ShouldBeTrue();
        state.Followed?.Slot.ShouldBe(expected: 2);
    }

    /// <summary>The exact Respawn interval and pending receipt gate order one current transfer.</summary>
    [Fact]
    public void Phase06_E128_SpectatorWaitsForExplicitCurrentTransfer()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        var state = new FoundationSpectator();
        FoundationRunReadback old = Readback(session: session);
        state.Observe(readback: old, binding: "local", now: 0, center: -128, span: 1280);
        KillPlayer(session: session);
        session.Step(elapsedMilliseconds: 999);
        state.Observe(readback: Readback(session: session), binding: "local", now: 999, center: -128, span: 1280);
        state.CanRespawn.ShouldBeFalse();
        state.BeginRequest(epoch: 1, sequence: 1).ShouldBeFalse();
        session.Step(elapsedMilliseconds: 1);
        state.Observe(readback: Readback(session: session), binding: "local", now: 1000, center: -128, span: 1280);
        state.CanRespawn.ShouldBeTrue();
        session.Population.FindLease(bindingId: "local").ShouldBeNull();
        state.BeginRequest(epoch: 1, sequence: 2).ShouldBeTrue();
        state.BeginRequest(epoch: 1, sequence: 2).ShouldBeFalse();
        state.Observe(readback: Readback(session: session, receipt: (1, "Early", false)), binding: "local", now: 1001,
                center: -128, span: 1280);
        state.Pending.ShouldBeTrue();
        state.Status.ShouldBe(expected: "Requesting survivor…");
        state.Observe(readback: old, binding: "local", now: 1002, center: -128, span: 1280);
        state.CanDrive.ShouldBeFalse();
        state.Pending.ShouldBeTrue();
        RunSuccession.Outcome assigned = Respawn(session: session, sequence: 2);
        assigned.Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        state.Observe(readback: Readback(session: session, receipt: (2, "Assigned", false)), binding: "local",
                now: 1003, center: -128, span: 1280);
        state.CanDrive.ShouldBeTrue();
        state.Visible.ShouldBeFalse();
        state.Pending.ShouldBeFalse();
        state.TransferNotice.ShouldContain(expected: assigned.View.ShouldNotBeNull().SurvivorId.ShouldNotBeNull());
        state.RetiredSurvivor.ShouldBe(expected: Phase06MeleeTests.Owner);
        state.Observe(readback: Readback(session: session), binding: "local", now: 3003, center: -128, span: 1280);
        state.TransferNotice.ShouldBeEmpty();
    }

    /// <summary>A queued request completes once when a genuine candidate is released.</summary>
    [Fact]
    public void Spectator_QueuedRequest_CompletesWithoutRepeating()
    {
        FoundationSession session = Phase06SuccessionTests.Start(count: 2);
        Join(session: session, ordinal: 2);
        var state = new FoundationSpectator();
        state.Observe(readback: Readback(session: session), binding: "local", now: 0, center: -128, span: 1280);
        KillPlayer(session: session);
        session.Step(elapsedMilliseconds: 1000);
        state.Observe(readback: Readback(session: session), binding: "local", now: 1000, center: -128, span: 1280);
        state.BeginRequest(epoch: 1, sequence: 1).ShouldBeTrue();
        Respawn(session: session, sequence: 1).Status.ShouldBe(expected: RunSuccession.Status.Waiting);
        state.Observe(readback: Readback(session: session, receipt: (1, "Waiting", false)), binding: "local",
                now: 1001, center: -128, span: 1280);
        state.Status.ShouldBe(expected: "Waiting for a survivor");
        state.CanRespawn.ShouldBeFalse();
        state.BeginRequest(epoch: 1, sequence: 2).ShouldBeFalse();
        state.Cycle(previous: true).ShouldBeTrue();
        FoundationSpectator.Teammate teammate = state.Followed.ShouldNotBeNull();
        session.RemoveParticipant(bindingId: teammate.Binding).ShouldBeTrue();
        state.Observe(readback: Readback(session: session), binding: "local", now: 1002, center: -128, span: 1280);
        state.CanDrive.ShouldBeTrue();
        state.Visible.ShouldBeFalse();
        state.TransferNotice.ShouldContain(expected: teammate.Survivor);
    }

    /// <summary>Only a nearby handover through previously shown bounds uses the quarter-second tween.</summary>
    [Theory]
    [InlineData(2000, true, true)]
    [InlineData(2000, false, false)]
    [InlineData(20000, true, false)]
    public void SpectatorCamera_UnknownOrDistantView_Cuts(int elapsed, bool known, bool smooth)
    {
        FoundationSession session = Phase06MeleeTests.Start();
        var state = new FoundationSpectator();
        state.Observe(readback: Readback(session: session), binding: "local", now: 0, center: -128, span: 1280);
        if (known)
        {
            state.RememberVisible(center: -128, span: 1280);
        }

        (bool accepted, _) = session.TryAcceptMotion(controller: Phase06MeleeTests.Control(), sequence: 1,
                input: new RunLocomotion.Input(right: true));
        accepted.ShouldBeTrue();
        session.Step(elapsedMilliseconds: elapsed);
        KillPlayer(session: session);
        state.Observe(readback: Readback(session: session), binding: "local", now: 1, center: -128, span: 1280);
        double from = state.Camera.ShouldNotBeNull();
        state.RememberVisible(center: from, span: 1280);
        session.Step(elapsedMilliseconds: 1000);
        state.Observe(readback: Readback(session: session), binding: "local", now: 2, center: from, span: 1280);
        state.BeginRequest(epoch: 1, sequence: 1).ShouldBeTrue();
        Respawn(session: session, sequence: 1).Status.ShouldBe(expected: RunSuccession.Status.Assigned);
        state.Observe(readback: Readback(session: session), binding: "local", now: 3, center: from, span: 1280);
        state.Smoothing.ShouldBe(expected: smooth);
        state.Camera.ShouldBe(expected: smooth ? from : -128);
        if (smooth)
        {
            state.Observe(readback: Readback(session: session), binding: "local", now: 128, center: from, span: 1280);
            state.Camera.ShouldBe(expected: (from - 128) / 2);
            state.Observe(readback: Readback(session: session), binding: "local", now: 253, center: from, span: 1280);
            state.Camera.ShouldBe(expected: -128);
            state.Smoothing.ShouldBeFalse();
        }
    }

    private static FoundationRunReadback Readback(FoundationSession session,
            (long sequence, string status, bool replayed)? receipt = null)
    {
        (FoundationSnapshot roster, JsonElement world) = session.ReadRunPublic().ShouldNotBeNull();
        return new FoundationRunReadback(roster: roster, world: world,
                views: ImmutableDictionary<string, FoundationRunView>.Empty.Add(key: "status",
                        value: Scope(session: session, scope: "status")), feedback: receipt);
    }

    private static RunSuccession.Outcome Respawn(FoundationSession session, long sequence)
    {
        return session.RequestRespawns(requests: [new RunSuccession.Request(sessionId: session.Snapshot.SessionId,
                bindingId: "local", deathGeneration: 1, sequence: sequence)]).Single();
    }

    private static void Join(FoundationSession session, int ordinal)
    {
        string binding = FormattableString.Invariant(formattable: $"guest-{ordinal}");
        session.AddParticipant(bindingId: binding, playerId: binding, actorId: binding, label: "Guest").ShouldBeTrue();
        session.MarkReady(bindingId: binding).ShouldBeTrue();
    }

    private static void KillPlayer(FoundationSession session)
    {
        decimal position = Phase06MeleeTests.Survivor(session: session).PositionSubunitsExact;
        int facing = position < 58350 ? -1 : 1;
        session.TryAddHostile(id: "spectator-attacker", position: position - 650 * facing,
                facing: facing, health: 100).ShouldBeTrue();
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "left-hand", owner: "spectator-attacker");
        session.TryUpdateVitals(survivorId: Phase06MeleeTests.Owner, health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "spectator-attacker", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();
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
