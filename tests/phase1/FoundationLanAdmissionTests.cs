using System.Linq;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises reservation capacity and idempotence through actual coordinator wire dispatch.</summary>
public sealed class FoundationLanAdmissionTests
{
    /// <summary>A repeated hello returns its reservation; refusal reasons remain distinct.</summary>
    [Fact]
    public void PendingAdmissionIsIdempotentAndBounded()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        var hello = new FoundationEnvelope(kind: "hello", playerId: "p2", label: "Guest");
        port.Enqueue(sender: 2, envelope: hello);
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope original = port.Sent.Single(predicate: row => row.Envelope.Kind is "baseline").Envelope;
        port.Sent.Clear();
        port.Enqueue(sender: 2, envelope: hello);
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Count(predicate: row => row.Envelope.Kind is "baseline").ShouldBe(expected: 1);
        port.Sent.First(predicate: row => row.Envelope.Kind is "baseline").Envelope.BindingId
                .ShouldBe(expected: original.BindingId);
        port.Enqueue(sender: 3, envelope: new FoundationEnvelope(kind: "hello", playerId: "p2", label: "Other"));
        host.Update(elapsedMilliseconds: 0);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "Duplicate profile");
        foreach (int sender in new[] { 4, 5, 6 })
        {
            port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "hello",
                    playerId: "p" + sender, label: "Guest"));
        }

        host.Update(elapsedMilliseconds: 0);
        (host.Snapshot?.Participants.Length ?? 0).ShouldBe(expected: 4);
        port.Sent.Last().Envelope.Label.ShouldStartWith(expected: "Full");
        ProofProtocol.Record(id: "core.admission", diagnostic: "PENDING_CAPACITY_REPEAT_HELLO_DISTINCT_REASONS");
    }
}
