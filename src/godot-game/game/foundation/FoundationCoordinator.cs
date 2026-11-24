using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    internal (long elapsedMilliseconds, bool insideCamp, bool safeDistance, bool stashReachable)? LocalWorld
    { get; private set; }
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

    internal void Start(bool host, int port = 24680, bool observeWorld = false,
            Func<string, int, byte[]?>? read = null)
    {
        RequireFresh();
        _used = true;
        _startedAt = Now();
        string sessionId = Guid.NewGuid().ToString(format: "N");
        string actorId = Guid.NewGuid().ToString(format: "N");
        FoundationSession session = CreateSession(sessionId: sessionId, actorId: actorId,
                observeWorld: observeWorld && !host, read: read);
        if (host)
        {
            (_connection ?? throw new InvalidOperationException(message: "CONNECTION_UNAVAILABLE")).Host(port: port);
        }

        _session = session;
        _active = true;
        RecordEvent(value: host ? "host-start" : "solo-start");
    }

    internal void Join(string address, int port, string expectedSessionId = "")
    {
        RequireFresh();
        _expectedSession = expectedSessionId;
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
                    attemptId: _attempt, sessionId: _expectedSession));
            _helloSent = true;
        }

        if (_session is not null)
        {
            _session.Step(elapsedMilliseconds: elapsedMilliseconds);
            ReadLocalWorld(session: _session);
            RefreshRunViews();
        }
        _publicationElapsed = checked(_publicationElapsed + elapsedMilliseconds);
        _livenessElapsed = checked(_livenessElapsed + elapsedMilliseconds);
        if (_session is not null && (_pendingPublication || _publicationElapsed >= 16))
        {
            BroadcastState();
            _publicationElapsed %= 16;
            _pendingPublication = false;
        }

        if (_connection?.Connected == true && _livenessElapsed >= 500)
        {
            SendLiveness();
            _livenessElapsed %= 500;
        }
    }

    internal bool Submit(int direction)
    {
        return SubmitRunMotion(direction: direction, sprint: false, jump: false);
    }

    internal bool SubmitRunMotion(int direction, bool sprint, bool jump)
    {
        return direction is >= -1 and <= 1 && SubmitRunAction(kind: "move",
                request: JsonSerializer.SerializeToElement(value: new
                {
                    Direction = direction,
                    Sprint = sprint,
                    Jump = jump
                })) is
        {
            status: "Pending" or "Accepted" or "Overloaded" or "InsufficientStamina"
                    or "Airborne"
        };
    }

    internal (string survivorId, long generation, long deathGeneration)? ReadBoundRunControl(int sender)
    {
        return RunIdentity(sender: sender) is { } identity ? _session?.ReadRunControl(identity: identity) : null;
    }

    internal (FoundationSnapshot roster, JsonElement world)? ReadRunPublic()
    {
        return _session?.ReadRunPublic();
    }

    internal (long revision, ImmutableArray<(string scope, long generation, long token, JsonElement header,
            ImmutableArray<JsonElement> rows)> views)? ReadBoundRunViews(int sender, long? ownGrid = null,
                    long? externalGrid = null, ImmutableArray<long> ownWindows = default,
                    ImmutableArray<long> externalWindows = default)
    {
        return RunIdentity(sender: sender) is { } identity ? _session?.ReadRunViews(identity: identity,
                ownGrid: ownGrid, externalGrid: externalGrid, ownWindows: ownWindows,
                externalWindows: externalWindows) : null;
    }

    internal (string status, bool replayed, long revision) DispatchBoundRunAction(int sender,
            (string survivor, long epoch, long sequence) key, string kind, JsonElement request)
    {
        FoundationSession? session = _session;
        if (session is null || RunIdentity(sender: sender) is not { } identity)
        {
            return ("AccessLost", false, session?.Snapshot.Revision ?? -1);
        }

        if (kind is "respawn")
        {
            var result = DispatchBoundRunRespawns(requests: [(sender, key.epoch, key.sequence)])[0];
            return (result.status, result.replayed, session.Snapshot.Revision);
        }

        return session.DispatchRunAction(identity: identity, key: key, kind: kind, request: request);
    }

    internal ImmutableArray<(string status, bool replayed)> DispatchBoundRunRespawns(
            ImmutableArray<(int sender, long epoch, long sequence)> requests)
    {
        if (_session is not { } session)
        {
            return [.. requests.Select(selector: _ => ("AccessLost", false))];
        }

        return session.DispatchRunRespawns(session: session.Snapshot.SessionId,
                requests: [.. requests.Select(selector: row =>
                {
                    var identity = RunIdentity(sender: row.sender);
                    return (identity?.binding ?? "", identity?.player ?? "", row.epoch, row.sequence);
                })]);
    }

    internal void Close()
    {
        BeginClose(drain: false);
    }

    internal static bool UseObservedWorld(long generation)
    {
        return generation == 1 && HasObservedWorldProof();
    }

#if DEBUG
    internal bool ObserveLethalAttack()
    {
        return HasObservedWorldProof() && _session is { } session && RunIdentity(sender: 0) is { } identity
                && ReadBoundRunControl(sender: 0) is { generation: > 0 } owner
                && session.DispatchRunAction(identity: identity, key: (owner.survivorId, owner.generation,
                        checked(++_sequence)), kind: "proof-lethal", request: JsonSerializer.SerializeToElement(
                                value: new object())).status is "Applied";
    }
#endif

#if DEBUG
    internal static bool ShouldFailEntry(long generation)
    {
        return generation == 1 && Environment.GetEnvironmentVariable(variable: "PHASE2_FAIL_ENTRY_ONCE")
                is string fault && StringComparer.Ordinal.Equals(x: fault,
                        y: Environment.GetEnvironmentVariable(variable: "PHASE2_NONCE"));
    }
#endif

    internal string WorldHint(string instructions)
    {
        if (LocalWorld is not { } state)
        {
            return instructions;
        }

        string stash = state.stashReachable ? "Shared stash in reach." : "Shared stash out of reach.";
        if (!state.safeDistance)
        {
            return instructions + "\n" + stash + " Rest is unsafe with an enemy nearby.";
        }

        string rest = state.insideCamp ? "Safe to rest." : "Rest is unsafe outside camp.";
        return instructions + "\n" + stash + " " + rest;
    }

    internal void ObserveWorldHint(string hint, bool visible)
    {
        if (_worldTransition is not null)
        {
            string visibility = visible ? "visible" : "hidden";
            RecordEvent(value: _worldTransition + "|" + visibility + "|" + hint);
            _worldTransition = null;
        }
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
    private string _expectedSession = "";
    private long _sequence;
    private long _publicationElapsed;
    private bool _pendingPublication;
    private long _livenessElapsed;
    private long _heartbeatSequence;
    private string? _worldTransition;

    private static bool HasObservedWorldProof()
    {
#if DEBUG
        string? seed = Environment.GetEnvironmentVariable(variable: "KZ_RUN_WORLD_PROOF");
        string? nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        return seed is not null && StringComparer.Ordinal.Equals(x: seed, y: nonce)
                && Environment.GetEnvironmentVariable(variable: "PHASE3_ROLE") is "solo"
                && Environment.GetEnvironmentVariable(variable: "PHASE3_INPUT_DRIVER")
                        is "synthetic-native-ParseInputEvent"
                && FoundationObservationScope.Directory() is not null;
#else
        return false;
#endif
    }

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
                else if (packet.Sender == 1 && IsKnownHostGeneration(generation: packet.PeerGeneration))
                {
                    ReceiveGuest(envelope: envelope);
                }
            }
            catch (Exception error) when (error is JsonException or InvalidDataException or ArgumentException
                    or InvalidOperationException or KeyNotFoundException or OverflowException or FormatException)
            {
                RejectedPackets++;
                string reason = error.Message is "WIRE_INCOMPATIBLE" ? "incompatible" : "invalid";
                RecordEvent(value: "packet-rejected:" + reason);
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

        FinishRespawnPoll();
    }

    private void BroadcastState()
    {
        if (_session is null)
        {
            return;
        }

        FoundationSnapshot state = HostPublication();
        var current = _session.ReadRunPublic();
        FoundationRunFrame? model = current is { } publicRun && publicRun.roster.Revision == state.Revision
                ? new FoundationRunFrame { Body = publicRun.world } : null;
        foreach ((int sender, FoundationPeerBinding peer) in _peers)
        {
            SendRunState(sender: sender, peer: peer, state: state, model: model);
        }
    }

    private void SendRunState(int sender, FoundationPeerBinding peer, FoundationSnapshot state,
            FoundationRunFrame? model)
    {
        try
        {
            Send(peer: sender, envelope: new FoundationEnvelope(kind: "state", sessionId: state.SessionId,
                    revision: state.Revision, snapshot: state, attemptId: peer.Baseline.AttemptId, game: model));
        }
        catch (InvalidDataException error) when (model is not null && error.Message is "WIRE_SIZE")
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
            _connection.Send(peer: peer, payload: FoundationProtocol.Encode(envelope: envelope),
                    reliable: envelope.Kind is not "state");
        }
    }

    private static bool Bound(FoundationEnvelope envelope, FoundationPeerBinding peer)
    {
        return StringComparer.Ordinal.Equals(x: envelope.BindingId, y: peer.BindingId)
                && StringComparer.Ordinal.Equals(x: envelope.PlayerId, y: peer.PlayerId)
                && StringComparer.Ordinal.Equals(x: envelope.ActorId, y: peer.ActorId)
                && StringComparer.Ordinal.Equals(x: envelope.AttemptId, y: peer.Baseline.AttemptId);
    }

    private (string session, string binding, string player)? RunIdentity(int sender)
    {
        if (!_active || _session is not { } session)
        {
            return null;
        }

        if (sender == 0)
        {
            return (session.Snapshot.SessionId, BindingId, PlayerId);
        }

        return CurrentReadyPeer(sender: sender) is { } peer
                ? (session.Snapshot.SessionId, peer.BindingId, peer.PlayerId) : null;
    }

    private FoundationSession CreateSession(string sessionId, string actorId, bool observeWorld,
            Func<string, int, byte[]?>? read)
    {
        if (read is not null)
        {
            return FoundationSession.CreateFromResources(sessionId: sessionId, playerId: PlayerId, actorId: actorId,
                    read: read, observeWorld: observeWorld);
        }

#if DEBUG
        if (observeWorld)
        {
            return FoundationSession.CreateObserved(sessionId: sessionId, playerId: PlayerId, actorId: actorId);
        }
#endif
        return new FoundationSession(sessionId: sessionId, playerId: PlayerId, actorId: actorId);
    }

    private void ReadLocalWorld(FoundationSession session)
    {
        var current = session.LocalWorld;
        var previous = LocalWorld;
        LocalWorld = current;
        if (previous?.insideCamp == current.insideCamp && previous?.safeDistance == current.safeDistance
                && previous?.stashReachable == current.stashReachable)
        {
            return;
        }

        string camp = current.insideCamp ? "inside" : "outside";
        string danger = current.safeDistance ? "danger-clear" : "danger-near";
        string stash = current.stashReachable ? "stash-reachable" : "stash-blocked";
        string status = camp + ":" + danger + ":" + stash;
        long revision = session.Snapshot.Revision;
        long position = session.Actor.PositionSubunits;
        _worldTransition = FormattableString.Invariant(formattable:
                $"run-world:{status}:{current.elapsedMilliseconds}:{revision}:{position}");
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
