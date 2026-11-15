using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
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
        var menu = new FoundationMenu(root: root, gateway: gateway, services: services, join: () => { },
                back: () => { }, coordinator: () => coordinator, replace: value => { }, input: new FoundationInput(),
                runtime: new FoundationRuntimeState());
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
}
