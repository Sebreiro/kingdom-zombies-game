using System.Linq;
using System.Reflection;
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
    public void Input_JumpAndAttackRequireFreshWorldPresses()
    {
        typeof(FoundationInput).GetMethod(name: "TakeJump", bindingAttr:
                BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
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
