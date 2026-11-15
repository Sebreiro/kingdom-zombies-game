using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Composes replaceable transport, detached protocol and pure host authority.</summary>
internal sealed class FoundationCoordinator
{
    internal FoundationCoordinator(string playerId, string label, IConnectionService? connection = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: playerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: label);
        PlayerId = playerId;
        _label = label;
        _connection = connection;
        _generation = connection?.Generation ?? "solo";
    }

    internal string PlayerId { get; }
    internal string BindingId { get; private set; } = "local";
    internal FoundationSnapshot? Snapshot => _session?.Snapshot ?? _remote;
    internal bool Ready => _active && Snapshot?.Participants.Any(predicate: p => p.Ready
            && StringComparer.Ordinal.Equals(x: p.BindingId, y: BindingId)
            && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)) == true;
    internal FoundationActorState? LocalActor => Ready ? Snapshot?.Participants.First(predicate: p =>
            StringComparer.Ordinal.Equals(x: p.BindingId, y: BindingId)).Actor : null;
    internal bool IsHost => _session is not null;
    internal string Error { get; private set; } = "";
    internal long InstalledRevision { get; private set; } = -1;
    internal long AcceptedIntents { get; private set; }
    internal long RejectedPackets { get; private set; }
    internal IReadOnlyList<string> Events => _events;

    internal void Start(bool host, int port = 24680)
    {
        RequireFresh();
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
            return;
        }

        Receive();
        if (_session is null && !_helloSent && _connection?.Connected == true)
        {
            Send(peer: 1, envelope: new FoundationEnvelope(kind: "hello", playerId: PlayerId, label: _label));
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
                sequence: command.Sequence, direction: command.Direction));
        return true;
    }

    internal void Close()
    {
        if (!_active)
        {
            _connection?.Close();
            return;
        }

        _active = false;
        _session?.Close();
        try
        {
            if (_connection?.Connected == true)
            {
                Send(peer: IsHost ? 0 : 1, envelope: new FoundationEnvelope(kind: IsHost ? "end" : "bye",
                        sessionId: Snapshot?.SessionId ?? ""));
            }
        }
        finally
        {
            _connection?.Close();
            RecordEvent(value: "closed");
        }
    }

    private readonly IConnectionService? _connection;
    private readonly string _generation;
    private readonly string _label;
    private readonly Dictionary<int, FoundationPeerBinding> _peers = [];
    private readonly List<string> _events = [];
    private FoundationSession? _session;
    private FoundationSnapshot? _remote;
    private bool _active;
    private bool _helloSent;
    private long _sequence;
    private long _publicationElapsed;
    private long _livenessElapsed;

    private void Receive()
    {
        foreach (ConnectionPacket packet in _connection?.Poll(maximumPackets: 64) ?? [])
        {
            if (!_active || !StringComparer.Ordinal.Equals(x: packet.Generation, y: _generation))
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
                    or InvalidOperationException or KeyNotFoundException or OverflowException)
            {
                RejectedPackets++;
                RecordEvent(value: "packet-rejected:" + error.GetType().Name);
            }
        }
    }

    private void ReceiveHost(int sender, FoundationEnvelope envelope)
    {
        if (envelope.Kind is "hello")
        {
            Admit(sender: sender, envelope: envelope);
            return;
        }

        if (!_peers.TryGetValue(key: sender, value: out FoundationPeerBinding? peer)
                || !StringComparer.Ordinal.Equals(x: envelope.SessionId, y: _session?.Snapshot.SessionId))
        {
            RejectedPackets++;
            return;
        }

        if (envelope.Kind is "ack" && Bound(envelope: envelope, peer: peer)
                && envelope.Revision == peer.BaselineRevision)
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
        if (sender <= 1 || _peers.ContainsKey(key: sender))
        {
            return;
        }

        string binding = Guid.NewGuid().ToString(format: "N");
        string actor = Guid.NewGuid().ToString(format: "N");
        if (_session?.AddParticipant(bindingId: binding, playerId: envelope.PlayerId, actorId: actor,
                label: envelope.Label) != true)
        {
            Send(peer: sender, envelope: new FoundationEnvelope(kind: "reject", label:
                    "This session is full or this profile is already connected. Choose another session or profile."));
            return;
        }

        FoundationSnapshot state = _session.Snapshot;
        _peers.Add(key: sender, value: new FoundationPeerBinding(bindingId: binding, playerId: envelope.PlayerId,
                actorId: actor, baselineRevision: state.Revision));
        Send(peer: sender, envelope: new FoundationEnvelope(kind: "baseline", sessionId: state.SessionId,
                playerId: envelope.PlayerId, actorId: actor, bindingId: binding, revision: state.Revision,
                snapshot: state));
        RecordEvent(value: "baseline-offered:" + binding);
    }

    private void ReceiveGuest(FoundationEnvelope envelope)
    {
        if (envelope.Kind is "reject" or "end")
        {
            Error = envelope.Kind is "end" ? "The host ended the session. Host or join another session."
                    : envelope.Label;
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
                x: p.BindingId, y: BindingId) && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)))
        {
            _remote = candidate;
        }
    }

    private void InstallBaseline(FoundationEnvelope envelope, FoundationSnapshot candidate)
    {
        if (_remote is not null || !StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: PlayerId)
                || !candidate.Participants.Any(predicate: p => StringComparer.Ordinal.Equals(x: p.BindingId,
                        y: envelope.BindingId) && StringComparer.Ordinal.Equals(x: p.Actor.PlayerId, y: PlayerId)
                        && StringComparer.Ordinal.Equals(x: p.Actor.ActorId, y: envelope.ActorId) && !p.Ready))
        {
            throw new InvalidDataException(message: "BASELINE_OWNERSHIP");
        }

        _remote = candidate;
        BindingId = envelope.BindingId;
        InstalledRevision = candidate.Revision;
        RecordEvent(value: "baseline-installed:" + BindingId);
        Send(peer: 1, envelope: new FoundationEnvelope(kind: "ack", sessionId: candidate.SessionId,
                playerId: PlayerId, actorId: envelope.ActorId, bindingId: BindingId, revision: candidate.Revision));
        RecordEvent(value: "baseline-acknowledged:" + BindingId);
    }

    private bool Accept(FoundationMovementCommand command, string binding, string player, string actor)
    {
        bool accepted = _session?.TryAccept(command: command, trustedBinding: binding, trustedPlayer: player,
                trustedActor: actor) == true;
        if (accepted)
        {
            AcceptedIntents++;
        }
        else
        {
            RejectedPackets++;
        }

        return accepted;
    }

    private void BroadcastState()
    {
        if (_session is null)
        {
            return;
        }

        FoundationSnapshot state = _session.Snapshot;
        foreach (int peer in _peers.Keys)
        {
            Send(peer: peer, envelope: new FoundationEnvelope(kind: "state", sessionId: state.SessionId,
                    revision: state.Revision, snapshot: state));
        }
    }

    private void SendLiveness()
    {
        if (IsHost)
        {
            foreach (int peer in _peers.Keys)
            {
                Send(peer: peer, envelope: new FoundationEnvelope(kind: "ping", sessionId: Snapshot?.SessionId ?? ""));
            }
        }
        else
        {
            Send(peer: 1, envelope: new FoundationEnvelope(kind: "ping", sessionId: Snapshot?.SessionId ?? ""));
        }
    }

    private void Send(int peer, FoundationEnvelope envelope)
    {
        _connection?.Send(peer: peer, payload: FoundationProtocol.Encode(envelope: envelope));
    }

    private static bool Bound(FoundationEnvelope envelope, FoundationPeerBinding peer)
    {
        return StringComparer.Ordinal.Equals(x: envelope.BindingId, y: peer.BindingId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: peer.PlayerId)
                && StringComparer.Ordinal.Equals(x: envelope.ActorId, y: peer.ActorId);
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
        if (_active || _session is not null || _helloSent)
        {
            throw new InvalidOperationException(message: "COORDINATOR_ALREADY_USED");
        }
    }
}
