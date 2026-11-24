using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

internal sealed partial class FoundationCoordinator
{
    internal (string survivorId, long generation, long deathGeneration)? LocalRunControl =>
            _session is not null ? ReadBoundRunControl(sender: 0) : _runInbox.Control;
    internal (long sequence, string status, bool replayed)? RunActionFeedback { get; private set; }

    internal (long sequence, string status)? SubmitRunAction(string kind, JsonElement request)
    {
        if (!Ready || LocalActor is not { } actor || LocalRunControl is not { } control)
        {
            return null;
        }

        long epoch = kind is "respawn" ? control.deathGeneration : control.generation;
        if (epoch <= 0)
        {
            return null;
        }

        long sequence = checked(++_sequence);
        string survivor = kind is "respawn" ? "" : control.survivorId;
        FoundationEnvelope envelope = RunActionEnvelope(actor: actor, kind: kind, sequence: sequence,
                survivor: survivor, epoch: epoch, request: request);
        if (_session is not null)
        {
            if (kind is "respawn")
            {
                _respawnPoll.Add(item: (0, envelope));
                return (sequence, "Pending");
            }

            (string status, bool replayed, long revision) outcome = DispatchBoundRunAction(sender: 0,
                    key: (survivor, epoch, sequence), kind: kind, request: request);
            if (kind is "move")
            {
                ObserveRunMovement(binding: BindingId, player: PlayerId, actor: actor.ActorId,
                        sequence: sequence, request: request, status: outcome.status);
            }

            RunActionFeedback = (sequence, outcome.status, outcome.replayed);
            return (sequence, outcome.status);
        }

        Send(peer: 1, envelope: envelope);
        return (sequence, "Pending");
    }

    internal FoundationRunReadback? ReadRunRender()
    {
        if (_session is null)
        {
            return _guestRunReadback;
        }

        FoundationSnapshot? roster = Snapshot;
        if (roster is null || !Ready)
        {
            return null;
        }

        if (ReadRunPublic() is not { } world || ReadBoundRunViews(sender: 0, ownGrid: _localOwnGrid,
                externalGrid: _localExternalGrid) is not { } local || world.roster.Revision != local.revision)
        {
            return null;
        }

        return new FoundationRunReadback(roster: roster, world: world.world,
                views: local.views.ToImmutableDictionary(keySelector: view => view.scope,
                        elementSelector: view => new FoundationRunView(revision: local.revision, epoch:
                                view.generation, token: view.token, header: view.header, rows: view.rows),
                        keyComparer: StringComparer.Ordinal), feedback: RunActionFeedback);
    }

    internal bool RequestRunWindows(long? ownGrid, long? externalGrid)
    {
        if (!Ready || LocalRunControl is not { generation: > 0 } || ownGrid is <= 0 || externalGrid is <= 0)
        {
            return false;
        }

        if (IsHost)
        {
            if (ReadBoundRunViews(sender: 0, ownGrid: ownGrid, externalGrid: externalGrid) is not { } capture
                    || !MatchesWindow(views: capture.views, scope: "own", requested: ownGrid)
                    || !MatchesWindow(views: capture.views, scope: "external", requested: externalGrid))
            {
                return false;
            }

            _localOwnGrid = ownGrid;
            _localExternalGrid = externalGrid;
            return true;
        }

        return SubmitRunAction(kind: "window", request: JsonSerializer.SerializeToElement(
                value: new { OwnGrid = ownGrid, ExternalGrid = externalGrid })).HasValue;
    }

    private readonly FoundationViewInbox _runInbox = new();
    private readonly List<(int sender, FoundationEnvelope envelope)> _respawnPoll = [];
    private JsonElement? _remoteWorld;
    private volatile FoundationRunReadback? _guestRunReadback;
    private long? _localOwnGrid;
    private long? _localExternalGrid;

    private FoundationEnvelope RunActionEnvelope(FoundationActorState actor, string kind, long sequence,
            string survivor, long epoch, JsonElement request)
    {
        return new FoundationEnvelope(kind: "action", sessionId: actor.SessionId,
                playerId: PlayerId, actorId: actor.ActorId, bindingId: BindingId, sequence: sequence,
                attemptId: _attempt, game: new FoundationRunFrame
                {
                    Action = kind,
                    Survivor = survivor,
                    Epoch = epoch,
                    Body = request,
                });
    }

    private void ReceiveRunAction(int sender, FoundationPeerBinding peer, FoundationEnvelope envelope)
    {
        FoundationRunFrame frame = envelope.Game ?? throw new InvalidDataException(message: "WIRE_ACTION");
        if (!peer.Ready || CurrentReadyPeer(sender: sender) is null)
        {
            RejectedPackets++;
            return;
        }

        if (!MatchesActionControl(sender: sender, frame: frame))
        {
            CompleteRunAction(sender: sender, peer: peer, envelope: envelope,
                    status: frame.Action is "respawn" ? "StaleDeath" : "AccessLost", replayed: false,
                    remember: false);
            return;
        }

        if (envelope.Sequence <= peer.RunSequence)
        {
            bool replay = envelope.Sequence == peer.RunSequence && peer.RunStatus.Length != 0
                    && StringComparer.Ordinal.Equals(x: frame.Action, y: peer.RunAction);
            if (envelope.Sequence != peer.RunSequence || peer.RunStatus.Length != 0)
            {
                CompleteRunAction(sender: sender, peer: peer, envelope: envelope,
                        status: replay ? peer.RunStatus : "StaleRequest", replayed: replay, remember: false);
            }

            return;
        }

        if (frame.Action is "respawn")
        {
            RememberRunAction(peer: peer, envelope: envelope, status: "");
            _respawnPoll.Add(item: (sender, envelope));
            return;
        }

        (string status, bool replayed) result = frame.Action is "window"
                ? (ChangeWindows(sender: sender, peer: peer, frame: frame), false)
                : DispatchPeerAction(sender: sender, envelope: envelope, frame: frame);
        CompleteRunAction(sender: sender, peer: peer, envelope: envelope, status: result.status,
                replayed: result.replayed, remember: result.status is not ("InvalidAction" or "StaleRequest"));
    }

    private (string status, bool replayed) DispatchPeerAction(int sender, FoundationEnvelope envelope,
            FoundationRunFrame frame)
    {
        JsonElement request = frame.Body ?? throw new InvalidDataException(message: "WIRE_ACTION");
        (string status, bool replayed, long _) = DispatchBoundRunAction(sender: sender,
                key: (frame.Survivor, frame.Epoch, envelope.Sequence), kind: frame.Action, request: request);
        if (frame.Action is "move")
        {
            ObserveRunMovement(binding: envelope.BindingId, player: envelope.PlayerId, actor: envelope.ActorId,
                    sequence: envelope.Sequence, request: request, status: status);
        }

        return (status, replayed);
    }

    private string ChangeWindows(int sender, FoundationPeerBinding peer, FoundationRunFrame frame)
    {
        JsonElement body = frame.Body ?? throw new InvalidDataException(message: "WIRE_WINDOW");
        if (body.EnumerateObject().Count() != 2
                || !body.TryGetProperty(propertyName: "OwnGrid", value: out JsonElement own)
                || !body.TryGetProperty(propertyName: "ExternalGrid", value: out JsonElement external))
        {
            return "InvalidAction";
        }

        long? ownId = own.ValueKind == JsonValueKind.Null ? null : own.GetInt64();
        long? externalId = external.ValueKind == JsonValueKind.Null ? null : external.GetInt64();
        if (ownId is <= 0 || externalId is <= 0)
        {
            return "InvalidAction";
        }

        if (ReadBoundRunViews(sender: sender, ownGrid: ownId, externalGrid: externalId) is not { } capture
                || !MatchesWindow(views: capture.views, scope: "own", requested: ownId)
                || !MatchesWindow(views: capture.views, scope: "external", requested: externalId))
        {
            return "AccessLost";
        }

        peer.OwnGrid = ownId;
        peer.ExternalGrid = externalId;
        return "Ready";
    }

    private bool MatchesActionControl(int sender, FoundationRunFrame frame)
    {
        return ReadBoundRunControl(sender: sender) is { } control && (frame.Action is "respawn"
                ? control.deathGeneration == frame.Epoch
                : control.generation == frame.Epoch
                        && StringComparer.Ordinal.Equals(x: control.survivorId, y: frame.Survivor));
    }

    private static bool MatchesWindow(
            ImmutableArray<(string scope, long generation, long token, JsonElement header,
                    ImmutableArray<JsonElement> rows)> views, string scope, long? requested)
    {
        return requested is null || views.Any(predicate: view => StringComparer.Ordinal.Equals(
                x: view.scope, y: scope) && view.header.GetProperty(propertyName: "ContainerId") is
                { ValueKind: JsonValueKind.Number } id && id.GetInt64() == requested);
    }

    private void FinishRespawnPoll()
    {
        if (_respawnPoll.Count == 0)
        {
            return;
        }

        ImmutableArray<(int sender, FoundationEnvelope envelope)> allowed = [.. _respawnPoll.Where(
                predicate: row => row.sender == 0 || CurrentReadyPeer(sender: row.sender) is { } peer
                        && Bound(envelope: row.envelope, peer: peer))];
        _respawnPoll.Clear();
        ImmutableArray<(string status, bool replayed)> results = DispatchBoundRunRespawns(
                requests: [.. allowed.Select(selector: row =>
                        (row.sender, row.envelope.Game?.Epoch ?? 0, row.envelope.Sequence))]);
        for (int index = 0; index < results.Length; index++)
        {
            (int sender, FoundationEnvelope envelope) = allowed[index];
            (string status, bool replayed) = results[index];
            if (sender == 0)
            {
                RunActionFeedback = (envelope.Sequence, status, replayed);
            }
            else if (CurrentReadyPeer(sender: sender) is { } peer && Bound(envelope: envelope, peer: peer))
            {
                CompleteRunAction(sender: sender, peer: peer, envelope: envelope, status: status, replayed: replayed);
            }
        }
    }

    private void CompleteRunAction(int sender, FoundationPeerBinding peer, FoundationEnvelope envelope,
            string status, bool replayed, bool remember = true)
    {
        if (remember && envelope.Sequence >= peer.RunSequence)
        {
            RememberRunAction(peer: peer, envelope: envelope, status: status);
        }

        long revision = _session?.Snapshot.Revision ?? 0;
        Send(peer: sender, envelope: new FoundationEnvelope(kind: "action-result", sessionId: envelope.SessionId,
                bindingId: peer.BindingId, playerId: peer.PlayerId, actorId: peer.ActorId,
                attemptId: peer.Baseline.AttemptId, sequence: envelope.Sequence, revision: revision,
                game: new FoundationRunFrame
                {
                    Action = envelope.Game?.Action ?? "",
                    Status = status,
                    Replayed = replayed
                }));
        bool inventoryCorrection = !replayed && envelope.Game?.Action is not ("move" or "attack" or "food"
                or "respawn");
        PublishPeerRunViews(sender: sender, peer: peer, force: inventoryCorrection,
                correlation: envelope.Sequence);
    }

    private void RememberRunAction(FoundationPeerBinding peer, FoundationEnvelope envelope, string status)
    {
        peer.RunSequence = envelope.Sequence;
        peer.RunAction = envelope.Game?.Action ?? "";
        peer.RunStatus = status;
        peer.LastResponse = Now();
    }

    private void ObserveRunMovement(string binding, string player, string actor, long sequence,
            JsonElement request, string status)
    {
        if (status is not ("Accepted" or "Overloaded" or "InsufficientStamina" or "Airborne"))
        {
            RejectedPackets++;
            return;
        }

        AcceptedIntents++;
        LastAcceptedRevision = _session?.Snapshot.Revision ?? -1;
        LastAcceptedIntent = new FoundationMovementCommand(sessionId: _session?.Snapshot.SessionId ?? "",
                playerId: player, actorId: actor, direction: request.GetProperty(propertyName: "Direction").GetInt32(),
                bindingId: binding, sequence: sequence);
        _pendingPublication = true;
    }

    private void RefreshRunViews()
    {
        foreach ((int sender, FoundationPeerBinding peer) in _peers)
        {
            if (CurrentReadyPeer(sender: sender) is not null)
            {
                PublishPeerRunViews(sender: sender, peer: peer, force: false);
            }
        }
    }

    private void PublishPeerRunViews(int sender, FoundationPeerBinding peer, bool force, long correlation = 0)
    {
        if (_session is not { } session || RunIdentity(sender: sender) is not { } identity
                || session.ReadRunViewFence(identity: identity) is not { } fence)
        {
            return;
        }

        bool changed = (fence.epoch, fence.token, fence.inventoryVersion) != peer.RunViewFence;
        long now = Now();
        bool updateItems = force || changed;
        if (!updateItems && correlation == 0 && now - peer.LastRunStatus < 100)
        {
            return;
        }

        if (session.ReadRunViews(identity: identity, ownGrid: peer.OwnGrid, externalGrid: peer.ExternalGrid,
                includeItems: updateItems) is not { } capture)
        {
            return;
        }

        if (updateItems)
        {
            RevokeItemViews(sender: sender, peer: peer, revision: capture.revision, correlation: correlation);
            if (fence.epoch == 0)
            {
                peer.OwnGrid = null;
            }

            if (fence.token == 0)
            {
                peer.ExternalGrid = null;
            }

            peer.RunViewFence = (fence.epoch, fence.token, fence.inventoryVersion);
        }

        long serial = checked(++peer.RunViewSerial);
        foreach (var (scope, generation, token, header, rows) in capture.views)
        {
            var frame = new FoundationRunFrame
            {
                Scope = scope,
                Epoch = generation,
                Token = token,
                View = serial,
                Body = header,
            };
            SendRunView(sender: sender, peer: peer, revision: capture.revision, correlation: correlation,
                    frame: frame, rows: rows);
        }

        peer.LastRunStatus = now;
    }

    private void RevokeItemViews(int sender, FoundationPeerBinding peer, long revision, long correlation)
    {
        if (peer.RunViewSerial == 0)
        {
            return;
        }

        long serial = checked(++peer.RunViewSerial);
        RevokeRunView(sender: sender, peer: peer, revision: revision, serial: serial, correlation: correlation,
                scope: "own");
        RevokeRunView(sender: sender, peer: peer, revision: revision, serial: serial, correlation: correlation,
                scope: "external");
    }

    private void RevokeRunView(int sender, FoundationPeerBinding peer, long revision, long serial,
            long correlation, string scope)
    {
        Send(peer: sender, envelope: RunViewEnvelope(peer: peer, kind: "view-revoke", revision: revision,
                correlation: correlation, frame: new FoundationRunFrame { Scope = scope, View = serial }));
    }

    private void SendRunView(int sender, FoundationPeerBinding peer, long revision, long correlation,
            FoundationRunFrame frame, ImmutableArray<JsonElement> rows)
    {
        if (_connection?.Connected != true)
        {
            return;
        }

        try
        {
            ImmutableArray<byte[]> parts = FoundationProtocol.EncodeView(rows: rows, envelope: part =>
                    RunViewEnvelope(peer: peer, kind: "view-part", revision: revision, correlation: correlation,
                            frame: part), template: frame);
            foreach (byte[] bytes in parts)
            {
                _connection.Send(peer: sender, payload: bytes);
            }
        }
        catch (InvalidDataException)
        {
            RevokeRunView(sender: sender, peer: peer, revision: revision, serial: frame.View,
                    correlation: correlation, scope: frame.Scope);
            RecordEvent(value: "view-rejected:bounds");
        }
    }

    private static FoundationEnvelope RunViewEnvelope(FoundationPeerBinding peer, string kind, long revision,
            long correlation, FoundationRunFrame frame)
    {
        return new FoundationEnvelope(kind: kind, sessionId: peer.Baseline.SessionId, bindingId: peer.BindingId,
                playerId: peer.PlayerId, actorId: peer.ActorId, attemptId: peer.Baseline.AttemptId,
                revision: revision, sequence: correlation, game: frame);
    }
}
