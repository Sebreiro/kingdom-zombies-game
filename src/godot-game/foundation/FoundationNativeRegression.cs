using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Multiplayer.Abstractions;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Exercises production native handlers headlessly; supplies no rendered or physical-input credit.</summary>
internal static class FoundationNativeRegression
{
    internal static void Browser(Control root, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        var discovery = new FoundationRegressionDiscovery();
        var services = new FoundationServices(identity: new LocalProfileService(directory:
                System.Environment.GetEnvironmentVariable(variable: "PHASE3_PROFILE_ROOT")
                        ?? throw new InvalidDataException(message: "REGRESSION_PROFILE_ROOT")),
                discovery: discovery, connection: () => throw new InvalidOperationException());
        FoundationCoordinator? coordinator = null;
        FoundationMenu menu = SelectionMenu(root: root, gateway: gateway, services: services,
                coordinator: () => coordinator);
        Observe(root: root, gateway: gateway, rows: [Advertisement(id: "hidden", name: "Hidden", occupied: 1)],
                expectedSession: "");
        root.GetNode<Control>(path: "Browser").Visible = true;
        discovery.Rows = [Advertisement(id: "selected", name: "Z camp", occupied: 1)];
        menu.Tick(nowMilliseconds: 1);
        Control rows = root.GetNode<Control>(path: "Browser/Scroll/Rows");
        ((Control)rows.GetChild(idx: 0)).GrabFocus();
        discovery.Rows = [Advertisement(id: "selected", name: "Z camp", occupied: 2),
            Advertisement(id: "inserted", name: "A camp", occupied: 1)];
        menu.Tick(nowMilliseconds: 2);
        Control selected = (Control)rows.GetChild(idx: 1);
        ulong instance = selected.GetInstanceId();
        Require(value: rows.GetChildCount() == 2 && selected.HasFocus()
                && services.Browser.SelectedId is "selected", message: "BROWSER_REORDER_FOCUS");
        menu.Tick(nowMilliseconds: 3);
        Require(value: rows.GetChild(idx: 1).GetInstanceId() == instance && selected.HasFocus(),
                message: "BROWSER_REBUILT_WITHOUT_CHANGE");
        menu.JoinSelected();
        Require(value: menu.RequestedSessionId.Length == 0, message: "SELECTED_REQUEST_NOT_CLEARED");
        root.GetNode<Control>(path: "Browser").Visible = false;
        Observe(root: root, gateway: gateway, rows: [], expectedSession: "selected");
        coordinator = new FoundationCoordinator(playerId: "pending", label: "Pending");
        root.GetNode<Control>(path: "Browser").Visible = true;
        discovery.Rows = [Advertisement(id: "pending-discovery", name: "New host", occupied: 1)];
        menu.Tick(nowMilliseconds: 4);
        Observe(root: root, gateway: gateway, rows: [..discovery.Rows], expectedSession: "selected");
        foreach (Node child in rows.GetChildren()) { rows.RemoveChild(node: child); child.QueueFree(); }
        root.GetNode<Control>(path: "Browser").Visible = false;
        root.GetNode<Control>(path: "Menu/Start").GrabFocus();
        CheckCancellation(root: root, gateway: gateway, services: services);
    }

    internal static void Projection(Control root, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        using PackedScene fixture = GD.Load<PackedScene>(path: "res://foundation/FoundationWorld.tscn");
        Node2D world = fixture.Instantiate<Node2D>();
        world.Name = "RegressionWorld";
        root.AddChild(node: world);
        ulong menu = root.GetNode<Control>(path: "Menu").GetInstanceId();
        foreach (string[] ids in new[] { new[] { "a", "a/../../Menu", ".." }, new[] { "a:b", "a/b", "a?b" },
            new[] { "é", "e\u0301", ":" } })
        {
            FoundationParticipantState[] participants = [Participant(id: "local", slot: 1),
                ..ids.Select(selector: (id, index) => Participant(id: id, slot: index + 2))];
            var snapshot = new FoundationSnapshot(sessionId: "native-regression", revision: 1,
                    participants: participants);
            FoundationView.ProjectRoster(world: world, snapshot: snapshot, bindingId: "local", gateway: gateway);
            FoundationView.ProjectRoster(world: world, snapshot: snapshot, bindingId: "local", gateway: gateway);
            int remoteCount = 0;
            foreach (Node child in world.GetChildren())
            {
                if (child.Name.ToString().StartsWith(value: "Actor_", comparisonType: StringComparison.Ordinal))
                {
                    remoteCount++;
                }
            }
            Require(value: remoteCount == 3, message: "OPAQUE_ACTOR_PROJECTION_COLLISION");
            foreach (string id in ids)
            {
                Require(value: world.GetNode<Node2D>(path: FoundationActorNode.Name(actorId: id)).Position.Y == 540,
                        message: "OPAQUE_ACTOR_PROJECTION");
            }
        }

        Require(value: root.GetNode<Control>(path: "Menu").GetInstanceId() == menu,
                message: "OPAQUE_ACTOR_TRAVERSAL");
        world.RemoveMeta(name: "projection_tick");
        foreach (long elapsed in new[] { long.MaxValue, 0, 0, long.MaxValue - 1, long.MaxValue })
        {
            var actor = new FoundationActorState(sessionId: "native-regression", playerId: "player1",
                    actorId: "local", positionSubunits: 1000, facing: 1, elapsedMilliseconds: elapsed);
            FoundationView.Project(world: world, actor: actor, gateway: gateway);
            Require(value: world.Position.IsFinite() && world.GetNode<Node2D>(path: "Player").Position.IsFinite()
                    && world.Position.X == 624, message: "HOSTILE_TIME_FINITE_CAMERA");
        }

        world.Position = new Vector2(x: float.NaN, y: float.PositiveInfinity);
        FoundationView.Project(world: world, actor: Participant(id: "local", slot: 1).Actor, gateway: gateway);
        Require(value: world.Position.IsFinite() && world.Position.X is >= 320 and <= 960,
                message: "FINITE_CAMERA_RECOVERY");
        root.RemoveChild(node: world);
        world.QueueFree();
    }

    private static void Observe(Control root, MainThreadGateway gateway, SessionAdvertisement[] rows,
            string expectedSession)
    {
        var observer = new FoundationDrawObserver(root: root, gateway: gateway);
        string directory = Path.Combine(path1: FoundationObservationScope.Directory()
                ?? throw new InvalidDataException(message: "REGRESSION_SCOPE"), path2: "corrupt-profile");
        System.IO.Directory.CreateDirectory(path: directory);
        File.WriteAllText(path: Path.Combine(path1: directory, path2: "profiles.json"), contents: "{");
        var identity = new LocalProfileService(directory: directory);
        Require(value: !identity.Load() && identity.Selected is null, message: "PROFILE_FAILURE_REQUIRED");
        foreach (bool pending in new[] { false, true })
        {
            string sample = JsonSerializer.Serialize(value: new { nonce = "regression", profile = identity.Selected,
                profileError = identity.FailureReason, playerId = (string?)null, bindingId = (string?)null,
                snapshot = (object?)null, discoveryRows = rows, ready = false, joining = pending });
            using JsonDocument observation = JsonDocument.Parse(json: observer.InspectControls(sample: sample));
            JsonElement value = observation.RootElement;
            Require(value: value.GetProperty(propertyName: "profile").ValueKind is JsonValueKind.Null
                    && value.GetProperty(propertyName: "input").GetProperty(propertyName: "profileError")
                            .GetString()?.Length > 0, message: "PROFILE_ERROR_OBSERVATION");
            string[] sessions = value.GetProperty(propertyName: "controls").EnumerateArray().Select(
                    selector: control => control.GetProperty(propertyName: "session").GetString() ?? "")
                    .Where(predicate: session => session.Length > 0).ToArray();
            Require(value: expectedSession.Length == 0 ? sessions.Length == 0
                    : sessions.Contains(value: expectedSession, comparer: StringComparer.Ordinal),
                    message: "EXISTING_ROW_IDENTITY");
        }

        Require(value: identity.Create() && identity.Selected is not null, message: "PROFILE_ERROR_RECOVERY");
    }

    private static FoundationParticipantState Participant(string id, int slot)
    {
        return new FoundationParticipantState(bindingId: slot == 1 ? "local" : "binding" + slot,
                label: "Actor", slot: slot, ready: true, actor: new FoundationActorState(
                        sessionId: "native-regression", playerId: "player" + slot, actorId: id,
                        positionSubunits: slot * 100, facing: 1, elapsedMilliseconds: 0));
    }

    private static void CheckCancellation(Control root, MainThreadGateway gateway, FoundationServices services)
    {
        long now = 0;
        var port = new PendingConnection();
        FoundationCoordinator? current = new FoundationCoordinator(playerId: "cancel-profile", label: "Cancel",
                connection: port, nowMilliseconds: () => now);
        current.Join(address: "127.0.0.1", port: 47778);
        current.Update(elapsedMilliseconds: 0);
        FoundationCoordinator? closing = null;
        var runtime = new FoundationRuntimeState();
        var menu = new FoundationMenu(root: root, gateway: gateway, services: services, join: () => { },
                back: () => { }, coordinator: () => current, replace: value =>
                {
                    Require(value: (current is null || current.Closed == false) && port.CloseCount == 0,
                            message: "CANCEL_CLOSED_TRANSPORT_BEFORE_ROOT_OWNER");
                    runtime.Retire(current: ref current, closing: ref closing);
                    current = value;
                }, input: new FoundationInput(), runtime: runtime);
        menu.CancelAttempt();
        menu.CancelAttempt();
        Require(value: current is null && closing?.Closed == false && port.Sent.Last().Kind is "cancel",
                message: "CANCEL_MUST_RETAIN_CONNECTED_ATTEMPT");
        var nextPort = new PendingConnection();
        current = new FoundationCoordinator(playerId: "next-cancel", label: "Next", connection: nextPort,
                nowMilliseconds: () => now);
        current.Join(address: "127.0.0.1", port: 47778);
        current.Update(elapsedMilliseconds: 0);
        menu.CancelAttempt();
        now = 249;
        closing?.Update(elapsedMilliseconds: 0);
        runtime.TickRetired();
        Require(value: port.CloseCount == 0 && nextPort.CloseCount == 0, message: "CANCEL_DRAIN_CLOSED_EARLY");
        now = 250;
        closing?.Update(elapsedMilliseconds: 0);
        runtime.TickRetired();
        Require(value: port.CloseCount == 1 && nextPort.CloseCount == 1, message: "CANCEL_DRAIN_NOT_CLOSED");
        runtime.CloseRetired();
    }

    private static FoundationMenu SelectionMenu(Control root, MainThreadGateway gateway,
            FoundationServices services, Func<FoundationCoordinator?> coordinator)
    {
        FoundationMenu? menu = null;
        menu = new FoundationMenu(root: root, gateway: gateway, services: services, join: () =>
                Require(value: menu?.RequestedSessionId is "selected" && menu.RequestedAddress is "127.0.0.1:47778",
                        message: "SELECTED_ROW_REQUEST_IDENTITY"), back: () => { }, coordinator: coordinator,
                replace: value => { }, input: new FoundationInput(), runtime: new FoundationRuntimeState());
        return menu;
    }

    private static SessionAdvertisement Advertisement(string id, string name, int occupied)
    {
        return new SessionAdvertisement(sessionId: id, sessionName: name, hostLabel: "Host", address: "127.0.0.1",
                port: 47778, occupied: occupied, availability: "Available", protocol: 1,
                content: FoundationProtocol.Content);
    }

    private static void Require(bool value, string message)
    {
        if (!value) { throw new InvalidDataException(message: message); }
    }

    private sealed class PendingConnection : IConnectionService
    {
        public string Generation => "cancel-regression";
        public bool Connected { get; private set; }
        public string PeerGeneration(int peer) => "cancel-peer";
        public void Host(int port) => throw new InvalidOperationException(message: "Guest-only regression");
        public void Join(string address, int port) => Connected = true;
        public IReadOnlyList<ConnectionPacket> Poll(int maximumPackets) => [];
        public void Send(int peer, ReadOnlyMemory<byte> payload, bool reliable = true) => Sent.Add(
                item: FoundationProtocol.Decode(bytes: payload));
        public void Close()
        {
            CloseCount++;
            Connected = false;
        }

        internal int CloseCount { get; private set; }
        internal List<FoundationEnvelope> Sent { get; } = [];
    }
}
