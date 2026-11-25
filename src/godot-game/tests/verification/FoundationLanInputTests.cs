using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises screen-owned physical holds and local projection selection.</summary>
[Trait("Tier", "1")]
public sealed class FoundationLanInputTests
{
    /// <summary>Menu and text keys cannot become gameplay holds when a screen opens.</summary>
    [Fact]
    public void Input_ScreenFocusAndLocalProjection_StayIndependent()
    {
        var input = new FoundationInput();
        input.SetContext(gameplay: false);
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 0);
        input.SetContext(gameplay: true);
        input.Direction.ShouldBe(expected: 0);
        input.Set(key: 68, pressed: false);
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 1);
        input.Set(key: 4194321, pressed: true);
        input.Set(key: 4194321, pressed: true);
        input.FreshPress.ShouldBeFalse();
        input.Direction.ShouldBe(expected: 1);
        input.Set(key: 65, pressed: true);
        input.Direction.ShouldBe(expected: 0);
        input.Clear();
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 0);
        CheckProjection();
        CheckMarkerBounds();
        ProofProtocol.Record(id: "core.input", diagnostic: "CONTEXT_ALIASES_REPEAT_FOCUS_EXACT_LOCAL_CAMERA_MARKERS");
    }

    /// <summary>Native jump input requires a one-shot, focus-fenced consumer.</summary>
    [Fact]
    public void Input_JumpAndAttack_RequireFreshWorldPresses()
    {
        var input = new FoundationInput();
        input.Set(key: 32, pressed: true);
        input.QueueJump();
        input.TakeJump().ShouldBeTrue();
        input.TakeJump().ShouldBeFalse();
        input.Set(key: 32, pressed: true);
        input.QueueJump();
        input.TakeJump().ShouldBeFalse();
        input.Mouse(pressed: true);
        input.TakeClick(world: false).ShouldBeFalse();
        input.TakeClick(world: true).ShouldBeFalse();
        input.Mouse(pressed: true);
        input.TakeClick(world: true).ShouldBeFalse();
        input.Mouse(pressed: false);
        input.Mouse(pressed: true);
        input.TakeClick(world: true).ShouldBeTrue();
        input.TakeClick(world: true).ShouldBeFalse();
    }

    /// <summary>Opening or leaving a panel fences held modifiers, old clicks and pending discrete actions.</summary>
    [Fact]
    public void Input_WorkspaceTransition_ClearsMovementAndPendingPresses()
    {
        var input = new FoundationInput();
        const int Shift = 4194325;
        input.Set(key: 68, pressed: true);
        input.Set(key: Shift, pressed: true);
        input.Held(key: Shift).ShouldBeTrue();
        input.Direction.ShouldBe(expected: 1);
        input.Set(key: 32, pressed: true);
        input.QueueJump();
        input.Mouse(pressed: true);
        input.SetContext(gameplay: false);
        input.Direction.ShouldBe(expected: 0);
        input.Held(key: Shift).ShouldBeFalse();
        input.TakeJump().ShouldBeFalse();
        input.TakeClick(world: true).ShouldBeFalse();
        input.SetContext(gameplay: true);
        input.Set(key: 68, pressed: true);
        input.Set(key: Shift, pressed: true);
        input.Mouse(pressed: true);
        input.Direction.ShouldBe(expected: 0);
        input.Held(key: Shift).ShouldBeFalse();
        input.TakeClick(world: true).ShouldBeFalse();
        input.Set(key: Shift, pressed: false);
        input.Set(key: Shift, pressed: true);
        input.Held(key: Shift).ShouldBeTrue();
        input.Mouse(pressed: false);
        input.Mouse(pressed: true);
        input.TakeClick(world: true).ShouldBeTrue();
    }

    /// <summary>Cursor resolution honors current world origin, invert-Y ground and finite action bounds.</summary>
    [Fact]
    public void Input_CursorCoordinates_ResolveToBoundedHostAim()
    {
        FoundationInput.Aim(localX: 64, localY: 492, origin: -300, ground: 700)
                .ShouldBe(expected: (3700m, 3700m));
        FoundationInput.Aim(localX: -8, localY: 548, origin: 100, ground: 200)
                .ShouldBe(expected: (-400m, -300m));
        FoundationInput.Aim(localX: double.NaN, localY: 540, origin: 0, ground: 0).ShouldBeNull();
        FoundationInput.Aim(localX: 16000001, localY: 540, origin: 0, ground: 0).ShouldBeNull();
    }

    /// <summary>An actual held bow stays toward the cursor with zero nocked arrows or synthetic release.</summary>
    [Fact]
    public void Input_UnloadedBow_RetainsAimAndCannotRelease()
    {
        string content = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game/game/content");
        var presentation = new CharacterPresentation(
                animation: File.ReadAllBytes(path: Path.Combine(path1: content,
                        path2: "animation/locomotion-melee.json")),
                items: File.ReadAllBytes(path: Path.Combine(path1: content, path2: "config/items.json")),
                combat: File.ReadAllBytes(path: Path.Combine(path1: content, path2: "config/combat.json")),
                bow: File.ReadAllBytes(path: Path.Combine(path1: content, path2: "animation/bow.json")));
        presentation.Configure(body: "CH-01", movement: "idle", action: "bow", leftItem: "IT-07",
                rightItem: "Empty", facing: -1);
        presentation.ConfigureBow(aimRadians: 0.5, leftArrows: 0, rightArrows: 0, leftUsable: true,
                rightUsable: true, stackIdentity: "unloaded", intrinsicAmmunition: false, holdUnloaded: true);
        presentation.Advance(elapsedSeconds: 0.8);
        JsonElement sample = presentation.Snapshot;
        sample.GetProperty(propertyName: "aimRadians").GetDouble().ShouldBe(expected: 0.5);
        sample.GetProperty(propertyName: "bowStage").GetString().ShouldBe(expected: "aim");
        sample.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        sample.GetProperty(propertyName: "fallback").GetString().ShouldBe(expected: "none");
        presentation.TryRelease(generation: sample.GetProperty(propertyName: "generation").GetInt64())
                .ShouldBeFalse();
    }

    /// <summary>A numbered food press uses current recipient authority without exposing its nested window.</summary>
    [Fact]
    public void Input_QuickFood_UsesPermittedReferenceWithoutOpenGrid()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        var berries = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: "IT-08", quantity: 2,
                condition: null, location: RunItemTransfer.Container(definition: session.Definitions.Items["IT-08"],
                        containerId: bag.Id, x: 0, y: 0), storageGrid: null);
        const string Owner = Phase06MeleeTests.Owner;
        session.TryEditInventory(edit: (inventory, definitions, _) => RunQuickReferences.Assign(inventory:
                inventory.WithItems(items: inventory.Items.Add(item: berries), nextItemId: berries.Id + 1),
                definitions: definitions, survivorId: Owner, slot: 1, itemId: berries.Id)).ShouldBeTrue();
        const string Fixture = "QUICK_FOOD_VIEW";
        (string session, string binding, string player) identity = (session.Snapshot.SessionId, "local", "host");
        (_, ImmutableArray<(string scope, long generation, long token, JsonElement header,
                ImmutableArray<JsonElement> rows)> views) = session.ReadRunViews(identity: identity)
                        ?? throw new System.InvalidOperationException(message: Fixture);
        (_, _, _, JsonElement header, ImmutableArray<JsonElement> rows) = views.Single(predicate: view =>
                view.scope is "own");
        rows.Any(predicate: row => row.GetProperty(propertyName: "Id").GetInt64() == berries.Id).ShouldBeFalse();
        JsonElement reference = header.GetProperty(propertyName: "Quick")[0];
        string stamp = reference.GetProperty(propertyName: "Stamp").GetString().ShouldNotBeNull();
        stamp.Length.ShouldBe(expected: 64);
        (string status, _, _) = session.DispatchRunAction(identity: identity, key: (Owner, 1, 1), kind: "food",
                request: JsonSerializer.SerializeToElement(value: new
                {
                    Item = new { Id = reference.GetProperty(propertyName: "ItemId").GetInt64(), Stamp = stamp },
                    QuickSlot = reference.GetProperty(propertyName: "Slot").GetInt32(),
                }));
        status.ShouldBe(expected: "Consumed");
        session.Inventory.Find(itemId: berries.Id)?.Quantity.ShouldBe(expected: 1);
    }

    private static void CheckProjection()
    {
        var session = new FoundationSession(sessionId: "session", playerId: "host", actorId: "host-actor");
        session.AddParticipant(bindingId: "guest-binding", playerId: "guest", actorId: "guest-actor",
                label: "Equal").ShouldBeTrue();
        FoundationPresentation.Local(snapshot: session.Snapshot, bindingId: "guest-binding").ShouldBeNull();
        session.MarkReady(bindingId: "guest-binding").ShouldBeTrue();
        FoundationParticipantState local = FoundationPresentation.Local(snapshot: session.Snapshot,
                bindingId: "guest-binding") ?? throw new System.InvalidOperationException(message: "LOCAL");
        local.Actor.ActorId.ShouldBe(expected: "guest-actor");
        FoundationPresentation.Local(snapshot: session.Snapshot, bindingId: "old-binding").ShouldBeNull();
        FoundationPresentation.LabelOffset(snapshot: session.Snapshot, participant: local).ShouldBe(expected: 28);
        session.RemoveParticipant(bindingId: "guest-binding").ShouldBeTrue();
        session.AddParticipant(bindingId: "replacement", playerId: "new", actorId: "new-actor", label: "Equal")
                .ShouldBeTrue();
        session.MarkReady(bindingId: "replacement").ShouldBeTrue();
        FoundationPresentation.Local(snapshot: session.Snapshot, bindingId: "guest-binding").ShouldBeNull();
        FoundationPresentation.Local(snapshot: session.Snapshot, bindingId: "replacement")?.Actor.ActorId
                .ShouldBe(expected: "new-actor");
        CheckChainedOverlap();
    }

    private static void CheckChainedOverlap()
    {
        FoundationParticipantState[] actors = [.. Enumerable.Range(start: 1, count: 3).Select(
                selector: slot => new FoundationParticipantState(bindingId: slot == 1 ? "local" : "binding" + slot,
                        label: "Player", slot: slot, ready: true, actor: new FoundationActorState(sessionId: "s",
                                playerId: "player" + slot, actorId: "actor" + slot,
                                positionSubunits: slot * 5000, facing: 1, elapsedMilliseconds: 0)))];
        var snapshot = new FoundationSnapshot(sessionId: "s", revision: 1, participants: actors);
        FoundationPresentation.LabelOffset(snapshot: snapshot, participant: actors[0]).ShouldBe(expected: 0);
        FoundationPresentation.LabelOffset(snapshot: snapshot, participant: actors[1]).ShouldBe(expected: 28);
        FoundationPresentation.LabelOffset(snapshot: snapshot, participant: actors[2]).ShouldBe(expected: 0);
    }

    private static void CheckMarkerBounds()
    {
        foreach (float viewport in new[] { 640f, 1280f, 1920f })
        {
            foreach (float width in new[] { 48f, 96f, 120f })
            {
                foreach (float actor in new[] { 0, 16, viewport / 2, viewport - 16, viewport })
                {
                    float offset = FoundationPresentation.MarkerOffset(actorScreenX: actor,
                            markerWidth: width, viewportWidth: viewport);
                    (actor + offset).ShouldBeGreaterThanOrEqualTo(expected: 0);
                    (actor + offset + width).ShouldBeLessThanOrEqualTo(expected: viewport);
                    if (actor == viewport / 2) { offset.ShouldBe(expected: -width / 2); }
                }

                foreach (float outside in new[] { -width, viewport + width })
                {
                    FoundationPresentation.MarkerOffset(actorScreenX: outside, markerWidth: width,
                            viewportWidth: viewport).ShouldBe(expected: -width / 2);
                }
            }
        }
    }
}
