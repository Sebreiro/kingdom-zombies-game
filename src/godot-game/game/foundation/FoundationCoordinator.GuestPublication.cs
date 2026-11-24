using System;
using System.IO;
using System.Linq;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    private string _localActor = "";
    private string _hostPeerGeneration = "";

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

        if (envelope.Kind is "action-result" or "view-part" or "view-revoke")
        {
            ReceiveGuestRun(envelope: envelope);
            return;
        }

        if (envelope.Kind is "end")
        {
            ReceiveHostEnd(envelope: envelope);
            return;
        }

        if (envelope.Kind is not ("baseline" or "state") || envelope.Snapshot is not FoundationSnapshot candidate)
        {
            return;
        }

        if (_remote is not null && (!StringComparer.Ordinal.Equals(x: candidate.SessionId, y: _remote.SessionId)
                || candidate.Revision <= _remote.Revision || RegressesActorTime(candidate: candidate)))
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "baseline")
        {
            InstallBaseline(envelope: envelope, candidate: candidate);
        }
        else
        {
            InstallState(envelope: envelope, candidate: candidate);
        }
    }

    private bool IsKnownHostGeneration(string generation)
    {
        return _remote is null || StringComparer.Ordinal.Equals(x: _hostPeerGeneration, y: generation);
    }

    private void InstallState(FoundationEnvelope envelope, FoundationSnapshot candidate)
    {
        if (_remote is null || !candidate.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(
                x: p.BindingId, y: BindingId) && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)
                && StringComparer.Ordinal.Equals(x: p.Actor.ActorId, y: _localActor) && (!Ready || p.Ready)))
        {
            return;
        }

        _remote = candidate;
        _remoteWorld = envelope.Game?.Body;
        if (_remoteWorld is { } world)
        {
            _runInbox.ObservePublic(world: world, revision: candidate.Revision, binding: BindingId);
        }

        _lastResponse = Now();
    }

    private void ReceiveHostEnd(FoundationEnvelope envelope)
    {
        if (!IsCurrentHostMessage(envelope: envelope))
        {
            return;
        }

        Send(peer: 1, envelope: new FoundationEnvelope(kind: "end-ack", sessionId: envelope.SessionId,
                bindingId: BindingId, playerId: PlayerId, actorId: _localActor, attemptId: _attempt));
        Fail(reason: "The host ended the session. Host or join another session.");
    }

    private void ReceiveGuestRun(FoundationEnvelope envelope)
    {
        if (_connection?.Connected != true || !IsCurrentHostMessage(envelope: envelope)
                || envelope.Game is not { } game)
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "action-result")
        {
            if (envelope.Sequence <= _sequence && envelope.Sequence >= (RunActionFeedback?.sequence ?? 0))
            {
                RunActionFeedback = (envelope.Sequence, game.Status, game.Replayed);
            }
        }
        else if (!_runInbox.Offer(envelope: envelope, now: Now()))
        {
            RejectedPackets++;
        }
    }

    private bool IsCurrentHostMessage(FoundationEnvelope envelope)
    {
        return _remote is not null && StringComparer.Ordinal.Equals(x: envelope.SessionId, y: _remote.SessionId)
                && StringComparer.Ordinal.Equals(x: envelope.BindingId, y: BindingId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: PlayerId)
                && StringComparer.Ordinal.Equals(x: envelope.ActorId, y: _localActor);
    }

    private bool RegressesActorTime(FoundationSnapshot candidate)
    {
        return _remote?.Participants.Any(predicate: previous => candidate.Participants.Any(predicate: current =>
                StringComparer.Ordinal.Equals(x: previous.Actor.ActorId, y: current.Actor.ActorId)
                        && current.Actor.ElapsedMilliseconds < previous.Actor.ElapsedMilliseconds)) == true;
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
        _hostPeerGeneration = _connection?.PeerGeneration(peer: 1) ?? "";
        InstalledRevision = candidate.Revision;
        _lastResponse = Now();
        RecordEvent(value: "baseline-installed:" + BindingId);
        Send(peer: 1, envelope: new FoundationEnvelope(kind: "ack", sessionId: candidate.SessionId,
                playerId: PlayerId, actorId: envelope.ActorId, bindingId: BindingId, revision: candidate.Revision,
                attemptId: _attempt));
        RecordEvent(value: "baseline-acknowledged:" + BindingId);
    }
}
