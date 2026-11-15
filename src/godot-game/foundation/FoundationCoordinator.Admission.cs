using System;
using System.Collections.Generic;
using System.Linq;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    private readonly Dictionary<int, FoundationAdmissionHistory> _admissions = [];

    private void ReceiveHost(int sender, FoundationEnvelope envelope)
    {
        RetireReplacedConnection(sender: sender);
        if (envelope.Kind is "hello")
        {
            Admit(sender: sender, envelope: envelope);
            return;
        }

        if (envelope.Kind is "cancel" && _peers.TryGetValue(key: sender,
                value: out FoundationPeerBinding? pending) && !pending.Ready
                && StringComparer.Ordinal.Equals(x: envelope.AttemptId, y: pending.Baseline.AttemptId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: pending.PlayerId))
        {
            RemovePeer(sender: sender, peer: pending, reason: "cancelled");
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
            if (!peer.Ready)
            {
                peer.Ready = true;
                peer.LastResponse = Now();
            }

            _session?.MarkReady(bindingId: peer.BindingId);
            RecordEvent(value: "ready:" + peer.BindingId);
            BroadcastState();
        }
        else if (envelope.Kind is "intent")
        {
            var command = new FoundationMovementCommand(sessionId: envelope.SessionId, playerId: envelope.PlayerId,
                    actorId: envelope.ActorId, direction: envelope.Direction, bindingId: envelope.BindingId,
                    sequence: envelope.Sequence);
            if (Accept(command: command, binding: peer.BindingId, player: peer.PlayerId, actor: peer.ActorId))
            {
                peer.LastResponse = Now();
            }
        }
        else if (envelope.Kind is "bye")
        {
            RemovePeer(sender: sender, peer: peer, reason: "left");
        }
        else if (envelope.Kind is "ping" && peer.Ready && envelope.Sequence > peer.HeartbeatSequence)
        {
            peer.HeartbeatSequence = envelope.Sequence;
            peer.LastResponse = Now();
        }
    }

    private void RetireReplacedConnection(int sender)
    {
        if (_peers.TryGetValue(key: sender, value: out FoundationPeerBinding? obsolete)
                && !StringComparer.Ordinal.Equals(x: obsolete.ConnectionGeneration,
                        y: _connection?.PeerGeneration(peer: sender)))
        {
            RemovePeer(sender: sender, peer: obsolete, reason: "connection-replaced");
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

        FoundationAdmissionHistory history = AdmissionHistory(sender: sender);
        if (!history.Expired && history.Outcomes.TryGetValue(key: envelope.AttemptId,
                value: out FoundationEnvelope? outcome))
        {
            Send(peer: sender, envelope: outcome);
            return;
        }

        if (history.Expired || history.Outcomes.Count >= 32)
        {
            Reject(sender: sender, reason: "This connection's admission window is closed. Reconnect to join again.");
            return;
        }

        if (_session.Snapshot.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(
                x: p.Actor.PlayerId, y: envelope.PlayerId)))
        {
            Refuse(sender: sender, hello: envelope, reason: "Duplicate profile is already connected.");
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
            Refuse(sender: sender, hello: envelope,
                    reason: "Full: this session already has four participants or reservations.");
            return;
        }

        FoundationSnapshot state = HostPublication();
        var baseline = new FoundationEnvelope(kind: "baseline", sessionId: state.SessionId,
                playerId: envelope.PlayerId, actorId: actor, bindingId: binding, revision: state.Revision,
                snapshot: state, attemptId: envelope.AttemptId);
        _peers.Add(key: sender, value: new FoundationPeerBinding(bindingId: binding, playerId: envelope.PlayerId,
                actorId: actor, baselineRevision: state.Revision, baseline: baseline, startedAt: Now(),
                connectionGeneration: _connection?.PeerGeneration(peer: sender) ?? ""));
        AdmissionHistory(sender: sender).Outcomes.Add(key: envelope.AttemptId, value: baseline);
        Send(peer: sender, envelope: baseline);
        RecordEvent(value: "baseline-offered:" + binding);
    }

    private void Reject(int sender, string reason)
    {
        Send(peer: sender, envelope: new FoundationEnvelope(kind: "reject", label: reason));
        RecordEvent(value: "admission-rejected:" + reason);
    }

    private void Refuse(int sender, FoundationEnvelope hello, string reason)
    {
        var outcome = new FoundationEnvelope(kind: "reject", label: reason, attemptId: hello.AttemptId,
                playerId: hello.PlayerId);
        AdmissionHistory(sender: sender).Outcomes.Add(key: hello.AttemptId, value: outcome);
        Send(peer: sender, envelope: outcome);
        RecordEvent(value: "admission-rejected:" + reason);
    }

    private FoundationAdmissionHistory AdmissionHistory(int sender)
    {
        string generation = _connection?.PeerGeneration(peer: sender) ?? "";
        if (!_admissions.TryGetValue(key: sender, value: out FoundationAdmissionHistory? history)
                || !StringComparer.Ordinal.Equals(x: generation, y: history.Generation))
        {
            history = new FoundationAdmissionHistory(generation: generation, startedAt: Now());
            _admissions[key: sender] = history;
        }

        if (Now() - history.StartedAt >= 30000)
        {
            history.Outcomes.Clear();
            history.Expired = true;
        }

        return history;
    }

    private void ExpireAdmissions(long now)
    {
        foreach ((int sender, FoundationAdmissionHistory history) in _admissions.ToArray())
        {
            if (!StringComparer.Ordinal.Equals(x: history.Generation, y: _connection?.PeerGeneration(peer: sender)))
            {
                _admissions.Remove(key: sender);
            }
            else if (now - history.StartedAt >= 30000)
            {
                history.Outcomes.Clear();
                history.Expired = true;
            }
        }
    }
}
