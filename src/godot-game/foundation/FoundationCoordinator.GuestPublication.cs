using System;
using System.IO;
using System.Linq;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    private string _localActor = "";

    private void ReceiveGuest(FoundationEnvelope envelope)
    {
        if (envelope.AttemptId.Length > 0 && !StringComparer.Ordinal.Equals(x: envelope.AttemptId, y: _attempt))
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "reject")
        {
            if (_remote is null)
            {
                Fail(reason: envelope.Label);
            }

            return;
        }

        if (!StringComparer.Ordinal.Equals(x: envelope.AttemptId, y: _attempt))
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "end" && _remote is not null
                && StringComparer.Ordinal.Equals(x: envelope.SessionId, y: _remote.SessionId)
                && StringComparer.Ordinal.Equals(x: envelope.BindingId, y: BindingId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: PlayerId)
                && StringComparer.Ordinal.Equals(x: envelope.ActorId, y: _localActor))
        {
            Send(peer: 1, envelope: new FoundationEnvelope(kind: "end-ack", sessionId: _remote.SessionId,
                    bindingId: BindingId, playerId: PlayerId, actorId: _localActor, attemptId: _attempt));
            Fail(reason: "The host ended the session. Host or join another session.");
            return;
        }

        if (envelope.Kind is not ("baseline" or "state") || envelope.Snapshot is not FoundationSnapshot candidate)
        {
            return;
        }

        if (_remote is not null && (!StringComparer.Ordinal.Equals(x: candidate.SessionId, y: _remote.SessionId)
                || candidate.Revision <= _remote.Revision))
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "baseline")
        {
            InstallBaseline(envelope: envelope, candidate: candidate);
        }
        else if (_remote is not null && candidate.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(
                x: p.BindingId, y: BindingId) && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)
                && StringComparer.Ordinal.Equals(x: p.Actor.ActorId, y: _localActor) && (!Ready || p.Ready)))
        {
            _remote = candidate;
            _lastResponse = Now();
        }
    }

    private void InstallBaseline(FoundationEnvelope envelope, FoundationSnapshot candidate)
    {
        if (_expectedSession.Length > 0 && !StringComparer.Ordinal.Equals(x: _expectedSession,
                y: candidate.SessionId))
        {
            Fail(reason: "The selected session is no longer available. Refresh the list or enter a host address.");
            return;
        }

        if (_remote is not null || !StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: PlayerId)
                || !candidate.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(x: p.BindingId,
                        y: envelope.BindingId) && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)
                        && StringComparer.Ordinal.Equals(x: p.Actor.ActorId, y: envelope.ActorId) && !p.Ready))
        {
            throw new InvalidDataException(message: "BASELINE_OWNERSHIP");
        }

        _remote = candidate;
        BindingId = envelope.BindingId;
        _localActor = envelope.ActorId;
        InstalledRevision = candidate.Revision;
        _lastResponse = Now();
        RecordEvent(value: "baseline-installed:" + BindingId);
        Send(peer: 1, envelope: new FoundationEnvelope(kind: "ack", sessionId: candidate.SessionId,
                playerId: PlayerId, actorId: envelope.ActorId, bindingId: BindingId, revision: candidate.Revision,
                attemptId: _attempt));
        RecordEvent(value: "baseline-acknowledged:" + BindingId);
    }
}
