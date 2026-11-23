using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    internal void RequestClose()
    {
        BeginClose(drain: true);
    }

    private const long JoinTimeoutMilliseconds = 30000;
    private const long ResponseTimeoutMilliseconds = 10000;
    private readonly Func<long> _nowMilliseconds;
    private bool _used;
    private bool _closed;
    private bool _draining;
    private bool _drainingGuest;
    private long _lifetime;
    private long _startedAt;
    private long _lastResponse;
    private long _closingAt;
    private long _lastNow;

    private long Now()
    {
        long now = _nowMilliseconds();
        if (now < _lastNow)
        {
            throw new InvalidOperationException(message: "MONOTONIC_CLOCK_REQUIRED");
        }

        _lastNow = now;
        return now;
    }

    private void Expire()
    {
        long now = Now();
        ExpireAdmissions(now: now);
        if (_session is not null)
        {
            foreach ((int sender, FoundationPeerBinding peer) in _peers.ToArray())
            {
                if (peer.Ready ? now - peer.LastResponse >= ResponseTimeoutMilliseconds
                        : now - peer.StartedAt >= JoinTimeoutMilliseconds)
                {
                    RemovePeer(sender: sender, peer: peer, reason: "response-timeout");
                }
            }
        }
        else if (_active && (Ready ? now - _lastResponse >= ResponseTimeoutMilliseconds
                : now - _startedAt >= JoinTimeoutMilliseconds))
        {
            Fail(reason: Ready ? "Connection lost: the host stopped responding. Host or join another session."
                    : "Join timed out after 30 seconds. Check the address and try again.");
        }
    }

    private void RemovePeer(int sender, FoundationPeerBinding peer, string reason)
    {
        if (!_peers.TryGetValue(key: sender, value: out FoundationPeerBinding? current)
                || !ReferenceEquals(objA: current, objB: peer))
        {
            return;
        }

        _peers.Remove(key: sender);
        if (_admissions.TryGetValue(key: sender, value: out FoundationAdmissionHistory? history)
                && !history.Expired && StringComparer.Ordinal.Equals(x: history.Generation,
                        y: peer.ConnectionGeneration)
                && StringComparer.Ordinal.Equals(x: history.Generation, y: _connection?.PeerGeneration(peer: sender)))
        {
            history.Outcomes[key: peer.Baseline.AttemptId] = new FoundationEnvelope(kind: "reject",
                    label: "This admission attempt is closed. Join again.", attemptId: peer.Baseline.AttemptId);
        }
        _session?.RemoveParticipant(bindingId: peer.BindingId);
        RecordEvent(value: "departed:" + peer.BindingId + ":" + reason);
        BroadcastState();
    }

    private void Fail(string reason)
    {
        Error = reason;
        BeginClose(drain: false);
    }

    private void BeginClose(bool drain)
    {
        if (_closed || _draining)
        {
            if (!drain && _draining)
            {
                FinishClose();
            }

            return;
        }

        _active = false;
        _lifetime = checked(_lifetime + 1);
        _session?.Close();
        SendDeparture();
        _expectedSession = "";
        _drainingGuest = drain && !IsHost && _connection?.Connected == true;
        _draining = _drainingGuest || (drain && IsHost && _peers.Count > 0);
        _closingAt = Now();
        _session = null;
        _remote = null;
        BindingId = "";
        _localActor = "";
        _sequence = 0;
        if (!_draining)
        {
            FinishClose();
        }
    }

    private void SendDeparture()
    {
        if (IsHost)
        {
            foreach ((int sender, FoundationPeerBinding peer) in _peers)
            {
                Send(peer: sender, envelope: PeerMessage(kind: "end", peer: peer));
            }
        }
        else
        {
            Send(peer: 1, envelope: new FoundationEnvelope(kind: _remote is null ? "cancel" : "bye",
                    sessionId: _remote?.SessionId ?? "", playerId: PlayerId, actorId: _localActor,
                    bindingId: BindingId, attemptId: _attempt));
        }
    }

    private void DrainEnd()
    {
        if (!_draining)
        {
            return;
        }

        foreach (ConnectionPacket packet in _connection?.Poll(maximumPackets: 64) ?? [])
        {
            if (!StringComparer.Ordinal.Equals(x: packet.Generation, y: _generation))
            {
                continue;
            }

            try
            {
                FoundationEnvelope envelope = FoundationProtocol.Decode(bytes: packet.Payload);
                if (_peers.TryGetValue(key: packet.Sender, value: out FoundationPeerBinding? peer)
                        && envelope.Kind is "end-ack" && Bound(envelope: envelope, peer: peer)
                        && StringComparer.Ordinal.Equals(x: envelope.SessionId, y: peer.Baseline.SessionId))
                {
                    _peers.Remove(key: packet.Sender);
                }
            }
            catch (Exception error) when (error is InvalidDataException or JsonException
                    or InvalidOperationException or FormatException or KeyNotFoundException)
            {
                RejectedPackets++;
            }
        }

        if ((!_drainingGuest && _peers.Count == 0) || Now() - _closingAt >= 250)
        {
            FinishClose();
        }
    }

    private static FoundationEnvelope PeerMessage(string kind, FoundationPeerBinding peer)
    {
        return new FoundationEnvelope(kind: kind, sessionId: peer.Baseline.SessionId, playerId: peer.PlayerId,
                actorId: peer.ActorId, bindingId: peer.BindingId, attemptId: peer.Baseline.AttemptId);
    }

    private void FinishClose()
    {
        if (_closed)
        {
            return;
        }

        _closed = true;
        _draining = false;
        _drainingGuest = false;
        _peers.Clear();
        _admissions.Clear();
        _connection?.Close();
        RecordEvent(value: "closed");
    }
}
