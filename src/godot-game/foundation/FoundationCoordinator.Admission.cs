using System;
using System.Linq;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    private void ReceiveHost(int sender, FoundationEnvelope envelope)
    {
        if (envelope.Kind is "hello")
        {
            Admit(sender: sender, envelope: envelope);
            return;
        }

        if (!_peers.TryGetValue(key: sender, value: out FoundationPeerBinding? peer)
                || !StringComparer.Ordinal.Equals(x: envelope.SessionId, y: _session?.Snapshot.SessionId)
                || !Bound(envelope: envelope, peer: peer))
        {
            RejectedPackets++;
            RecordEvent(value: "packet-rejected:binding-session-attempt");
            return;
        }

        if (envelope.Kind is "ack" && envelope.Revision == peer.BaselineRevision)
        {
            _session?.MarkReady(bindingId: peer.BindingId);
            RecordEvent(value: "ready:" + peer.BindingId);
            BroadcastState();
        }
        else if (envelope.Kind is "intent")
        {
            var command = new FoundationMovementCommand(sessionId: envelope.SessionId, playerId: envelope.PlayerId,
                    actorId: envelope.ActorId, direction: envelope.Direction, bindingId: envelope.BindingId,
                    sequence: envelope.Sequence);
            Accept(command: command, binding: peer.BindingId, player: peer.PlayerId, actor: peer.ActorId);
        }
    }

    private void Admit(int sender, FoundationEnvelope envelope)
    {
        if (sender <= 1 || _session is null)
        {
            return;
        }

        if (_peers.TryGetValue(key: sender, value: out FoundationPeerBinding? previous))
        {
            if (StringComparer.Ordinal.Equals(x: previous.PlayerId, y: envelope.PlayerId)
                    && StringComparer.Ordinal.Equals(x: previous.Baseline.AttemptId, y: envelope.AttemptId))
            {
                Send(peer: sender, envelope: previous.Baseline);
            }
            else
            {
                Reject(sender: sender, reason: "Duplicate active connection attempt.");
            }

            return;
        }

        if (_session.Snapshot.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(
                x: p.Actor.PlayerId, y: envelope.PlayerId)))
        {
            Reject(sender: sender, reason: "Duplicate profile is already connected.");
            return;
        }

        Reserve(sender: sender, envelope: envelope);
    }

    private void Reserve(int sender, FoundationEnvelope envelope)
    {
        string binding = Guid.NewGuid().ToString(format: "N");
        string actor = Guid.NewGuid().ToString(format: "N");
        if (_session?.AddParticipant(bindingId: binding, playerId: envelope.PlayerId, actorId: actor,
                label: envelope.Label) != true)
        {
            Reject(sender: sender, reason: "Full: this session already has four participants or reservations.");
            return;
        }

        FoundationSnapshot state = _session.Snapshot;
        var baseline = new FoundationEnvelope(kind: "baseline", sessionId: state.SessionId,
                playerId: envelope.PlayerId, actorId: actor, bindingId: binding, revision: state.Revision,
                snapshot: state, attemptId: envelope.AttemptId);
        _peers.Add(key: sender, value: new FoundationPeerBinding(bindingId: binding, playerId: envelope.PlayerId,
                actorId: actor, baselineRevision: state.Revision, baseline: baseline));
        Send(peer: sender, envelope: baseline);
        RecordEvent(value: "baseline-offered:" + binding);
    }

    private void Reject(int sender, string reason)
    {
        Send(peer: sender, envelope: new FoundationEnvelope(kind: "reject", label: reason));
        RecordEvent(value: "admission-rejected:" + reason);
    }
}
