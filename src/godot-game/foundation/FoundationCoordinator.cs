using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Composes replaceable transport, detached protocol and pure host authority.</summary>
internal sealed partial class FoundationCoordinator
{
    internal FoundationCoordinator(string playerId, string label, IConnectionService? connection = null,
            Func<long>? nowMilliseconds = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: playerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: label);
        PlayerId = playerId;
        _label = label;
        _connection = connection;
        _generation = connection?.Generation ?? "solo";
        _nowMilliseconds = nowMilliseconds ?? (() => 0);
    }

    internal string PlayerId { get; }
    internal string BindingId { get; private set; } = "local";
    internal FoundationSnapshot? Snapshot => _session is null ? _remote : HostPublication();
    internal bool Ready => _active && Snapshot?.Participants.Any(predicate: p => p.Ready
            && StringComparer.Ordinal.Equals(x: p.BindingId, y: BindingId)
            && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)) == true;
    internal FoundationActorState? LocalActor => Ready ? Snapshot?.Participants.First(predicate: p =>
            StringComparer.Ordinal.Equals(x: p.BindingId, y: BindingId)).Actor : null;
    internal bool IsHost => _session is not null;
    internal bool WaitingForHost => Ready && !IsHost && _connection?.Connected != true;
    internal string Error { get; private set; } = "";
    internal long InstalledRevision { get; private set; } = -1;
    internal long AcceptedIntents { get; private set; }
    internal FoundationMovementCommand? LastAcceptedIntent { get; private set; }
    internal long LastAcceptedRevision { get; private set; } = -1;
    internal long RejectedPackets { get; private set; }
    internal int AdmissionHistoryCount => _admissions.Count;
    internal IReadOnlyList<string> Events => _events;
    internal bool Closed => _closed;
    internal bool Finished => _used && !_active;

    internal void Start(bool host, int port = 24680)
    {
        RequireFresh();
        _used = true;
        _startedAt = Now();
        if (host)
        {
            (_connection ?? throw new InvalidOperationException(message: "CONNECTION_UNAVAILABLE")).Host(port: port);
        }

        _active = true;
        _session = new FoundationSession(sessionId: Guid.NewGuid().ToString(format: "N"),
                playerId: PlayerId, actorId: Guid.NewGuid().ToString(format: "N"));
        RecordEvent(value: host ? "host-start" : "solo-start");
    }

    internal void Join(string address, int port)
    {
        RequireFresh();
        _used = true;
        _startedAt = Now();
        _lastResponse = _startedAt;
        (_connection ?? throw new InvalidOperationException(message: "CONNECTION_UNAVAILABLE"))
                .Join(address: address, port: port);
        BindingId = "";
        _active = true;
        RecordEvent(value: "join-start");
    }

    internal void Update(long elapsedMilliseconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
        if (!_active)
        {
            DrainEnd();
            return;
        }

        Receive();
        Expire();
        if (!_active)
        {
            return;
        }
        if (_session is null && !_helloSent && _connection?.Connected == true)
        {
            Send(peer: 1, envelope: new FoundationEnvelope(kind: "hello", playerId: PlayerId, label: _label,
                    attemptId: _attempt));
            _helloSent = true;
        }

        _session?.Step(elapsedMilliseconds: elapsedMilliseconds);
        _publicationElapsed = checked(_publicationElapsed + elapsedMilliseconds);
        _livenessElapsed = checked(_livenessElapsed + elapsedMilliseconds);
        if (_session is not null && _publicationElapsed >= 50)
        {
            BroadcastState();
            _publicationElapsed %= 50;
        }

        if (_connection?.Connected == true && _livenessElapsed >= 500)
        {
            SendLiveness();
            _livenessElapsed %= 500;
        }
    }

    internal bool Submit(int direction)
    {
        FoundationActorState? actor = LocalActor;
        if (actor is null || direction is < -1 or > 1)
        {
            return false;
        }

        var command = new FoundationMovementCommand(sessionId: actor.SessionId, playerId: PlayerId,
                actorId: actor.ActorId, direction: direction, bindingId: BindingId, sequence: checked(++_sequence));
        if (_session is not null)
        {
            return Accept(command: command, binding: BindingId, player: PlayerId, actor: actor.ActorId);
        }

        Send(peer: 1, envelope: new FoundationEnvelope(kind: "intent", sessionId: command.SessionId,
                playerId: command.PlayerId, actorId: command.ActorId, bindingId: command.BindingId,
                sequence: command.Sequence, direction: command.Direction, attemptId: _attempt));
        return true;
    }

    internal void Close()
    {
        BeginClose(drain: false);
    }

    private readonly IConnectionService? _connection;
    private readonly string _generation;
    private readonly string _label;
    private readonly string _attempt = Guid.NewGuid().ToString(format: "N");
    private readonly Dictionary<int, FoundationPeerBinding> _peers = [];
    private readonly List<string> _events = [];
    private FoundationSession? _session;
    private FoundationSnapshot? _remote;
    private FoundationSnapshot? _hostSource;
    private FoundationSnapshot? _hostPublished;
    private bool _active;
    private bool _helloSent;
    private long _sequence;
    private long _publicationElapsed;
    private long _livenessElapsed;
    private long _heartbeatSequence;

    private void Receive()
    {
        long lifetime = _lifetime;
        foreach (ConnectionPacket packet in _connection?.Poll(maximumPackets: 64) ?? [])
        {
            if (!_active || lifetime != _lifetime
                    || !StringComparer.Ordinal.Equals(x: packet.Generation, y: _generation)
                    || packet.PeerGeneration.Length == 0 || !StringComparer.Ordinal.Equals(x: packet.PeerGeneration,
                            y: _connection?.PeerGeneration(peer: packet.Sender)))
            {
                continue;
            }

            try
            {
                FoundationEnvelope envelope = FoundationProtocol.Decode(bytes: packet.Payload);
                if (_session is not null)
                {
                    ReceiveHost(sender: packet.Sender, envelope: envelope);
                }
                else if (packet.Sender == 1)
                {
                    ReceiveGuest(envelope: envelope);
                }
            }
            catch (Exception error) when (error is JsonException or InvalidDataException or ArgumentException
                    or InvalidOperationException or KeyNotFoundException or OverflowException or FormatException)
            {
                RejectedPackets++;
                RecordEvent(value: "packet-rejected:" + error.Message);
                if (IsHost && error is InvalidDataException && error.Message is "WIRE_INCOMPATIBLE")
                {
                    Reject(sender: packet.Sender, reason: "Incompatible protocol or content.");
                }
                else if (!IsHost && _remote is null && error is InvalidDataException
                        && error.Message is "WIRE_INCOMPATIBLE")
                {
                    Fail(reason: "Incompatible protocol or content. Choose another session.");
                }
            }
        }
    }

    private bool Accept(FoundationMovementCommand command, string binding, string player, string actor)
    {
        bool accepted = _session?.TryAccept(command: command, trustedBinding: binding, trustedPlayer: player,
                trustedActor: actor) == true;
        if (accepted)
        {
            AcceptedIntents++;
            LastAcceptedIntent = command;
            LastAcceptedRevision = _session?.Snapshot.Revision ?? -1;
        }
        else
        {
            RejectedPackets++;
            RecordEvent(value: "intent-rejected:ownership-readiness-sequence");
        }

        return accepted;
    }

    private void BroadcastState()
    {
        if (_session is null)
        {
            return;
        }

        FoundationSnapshot state = HostPublication();
        foreach ((int sender, FoundationPeerBinding peer) in _peers)
        {
            Send(peer: sender, envelope: new FoundationEnvelope(kind: "state", sessionId: state.SessionId,
                    revision: state.Revision, snapshot: state, attemptId: peer.Baseline.AttemptId));
        }
    }

    private void SendLiveness()
    {
        if (!IsHost && _remote is not null)
        {
            FoundationParticipantState local = _remote.Participants.First(predicate: p =>
                    StringComparer.Ordinal.Equals(x: p.BindingId, y: BindingId));
            Send(peer: 1, envelope: new FoundationEnvelope(kind: "ping", sessionId: _remote.SessionId,
                    bindingId: BindingId, playerId: PlayerId, actorId: local.Actor.ActorId, attemptId: _attempt,
                    sequence: checked(++_heartbeatSequence)));
        }
    }

    private void Send(int peer, FoundationEnvelope envelope)
    {
        if (_connection?.Connected == true)
        {
            _connection.Send(peer: peer, payload: FoundationProtocol.Encode(envelope: envelope));
        }
    }

    private static bool Bound(FoundationEnvelope envelope, FoundationPeerBinding peer)
    {
        return StringComparer.Ordinal.Equals(x: envelope.BindingId, y: peer.BindingId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: peer.PlayerId)
                && StringComparer.Ordinal.Equals(x: envelope.ActorId, y: peer.ActorId)
                && StringComparer.Ordinal.Equals(x: envelope.AttemptId, y: peer.Baseline.AttemptId);
    }

    private void RecordEvent(string value)
    {
        if (_events.Count == 256)
        {
            _events.RemoveAt(index: 0);
        }

        _events.Add(item: value);
    }

    private void RequireFresh()
    {
        if (_used)
        {
            throw new InvalidOperationException(message: "COORDINATOR_ALREADY_USED");
        }
    }

    private FoundationSnapshot HostPublication()
    {
        FoundationSnapshot snapshot = (_session ?? throw new InvalidOperationException(message: "HOST_REQUIRED"))
                .Snapshot;
        if (ReferenceEquals(objA: snapshot, objB: _hostSource) && _hostPublished is not null)
        {
            return _hostPublished;
        }

        _hostSource = snapshot;
        _hostPublished = new FoundationSnapshot(sessionId: snapshot.SessionId, revision: snapshot.Revision,
                participants: snapshot.Participants.Select(selector: p => p.Slot == 1
                        ? new FoundationParticipantState(bindingId: p.BindingId, label: _label, slot: p.Slot,
                                ready: p.Ready, actor: p.Actor) : p));
        return _hostPublished;
    }
}
