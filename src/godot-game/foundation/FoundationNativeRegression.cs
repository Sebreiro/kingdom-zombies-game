using System;
using System.IO;
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
        var menu = new FoundationMenu(root: root, gateway: gateway, services: services, join: () => { },
                back: () => { }, coordinator: () => null, replace: value => { }, input: new FoundationInput(),
                runtime: new FoundationRuntimeState());
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
        foreach (Node child in rows.GetChildren()) { rows.RemoveChild(node: child); child.QueueFree(); }
        root.GetNode<Control>(path: "Browser").Visible = false;
        root.GetNode<Control>(path: "Menu/Start").GrabFocus();
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
