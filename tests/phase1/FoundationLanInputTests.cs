using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises screen-owned physical holds and local projection selection.</summary>
public sealed class FoundationLanInputTests
{
    /// <summary>Menu and text keys cannot become gameplay holds when a screen opens.</summary>
    [Fact]
    public void ScreenFocusAndLocalProjectionStayIndependent()
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
        ProofProtocol.Record(id: "core.input", diagnostic: "CONTEXT_ALIASES_REPEAT_FOCUS_EXACT_LOCAL_CAMERA_MARKERS");
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
    }
}
