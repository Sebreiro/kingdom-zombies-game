using System;
using System.IO;
using System.Text.Json;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Injects nonce-scoped synthetic native events, independently from read-only observations.</summary>
internal static class FoundationAutomation
{
    internal static void Tick(Control root, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        string? nonce = System.Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        string? directory = FoundationObservationScope.Directory();
        if (nonce is null || directory is null)
        {
            return;
        }

        string file = Path.Combine(path1: directory, path2: "input.request");
        if (!File.Exists(path: file))
        {
            return;
        }

        string? json = ReadRequest(file: file);
        if (json is null)
        {
            return;
        }

        using JsonDocument document = JsonDocument.Parse(json: json);
        JsonElement request = document.RootElement;
        if (!StringComparer.Ordinal.Equals(x: request.GetProperty(propertyName: "nonce").GetString(), y: nonce))
        {
            throw new InvalidOperationException(message: "DRIVER_NONCE");
        }

        Deliver(root: root, request: request, gateway: gateway);
        File.Delete(path: file);
        File.WriteAllText(path: Path.Combine(path1: directory, path2: "input.ack"), contents:
                request.GetProperty(propertyName: "sequence").GetInt64().ToString(
                        provider: System.Globalization.CultureInfo.InvariantCulture));
    }

    private static string? ReadRequest(string file)
    {
        try
        {
            return File.ReadAllText(path: file);
        }
        catch (IOException)
        {
            return null;
        }
    }

    private static void Deliver(Control root, JsonElement request, MainThreadGateway gateway)
    {
        gateway.RequireThread();
        string action = request.GetProperty(propertyName: "action").GetString() ?? "";
        if (action is "focus")
        {
            root.GetNode<Control>(path: request.GetProperty(propertyName: "control").GetString() ?? "").GrabFocus();
        }
        else if (action is "address")
        {
            root.GetNode<LineEdit>(path: "Browser/Address").Text = request.GetProperty(propertyName: "address")
                    .GetString() ?? "";
        }
        else if (action is "regression-browser")
        {
            FoundationNativeRegression.Browser(root: root, gateway: gateway);
        }
        else if (action is "key")
        {
            Key code = (Key)request.GetProperty(propertyName: "key").GetInt64();
            using var key = new InputEventKey { PhysicalKeycode = code, Keycode = code,
                Pressed = request.GetProperty(propertyName: "pressed").GetBoolean() };
            Input.ParseInputEvent(@event: key);
        }
        else
        {
            throw new InvalidOperationException(message: "DRIVER_ACTION");
        }
    }
}
