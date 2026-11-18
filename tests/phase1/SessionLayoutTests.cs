using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the actual compiled session presenter behavior.</summary>
public sealed class SessionLayoutTests
{
    /// <summary>Guest departure must explicitly preserve the other players' running world.</summary>
    [Fact]
    public void Departure_GuestSaysOthersContinue()
    {
        Call(name: "Departure", arguments: [false]).ShouldBe(expected:
                "Leave session? Other players can continue.");
    }

    /// <summary>Host departure explicitly affects every participant.</summary>
    [Fact]
    public void Departure_HostEndsForEveryone()
    {
        Call(name: "Departure", arguments: [true]).ShouldBe(expected: "End session for everyone?");
    }

    /// <summary>The exact current packaged theme is accepted by the real compiled binding validator.</summary>
    [Fact]
    public void Bind_CurrentThemeAccepted()
    {
        string project = Path.Combine(path1: ProofProtocol.Root, path2: "src/godot-game");
        using var pack = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(path1: project,
                path2: "content/packs/ui-session.json")));
        byte[][] inputs = [..pack.RootElement.GetProperty(propertyName: "bindings").EnumerateArray()
                .Select(selector: row => File.ReadAllBytes(path: Path.Combine(path1: project,
                        path2: row.GetProperty(propertyName: "path").GetString()
                                ?? throw new InvalidOperationException())))];
        Exception? rejection = null;
        try
        {
            Call(name: "Bind", arguments: [pack.RootElement, inputs]);
        }
        catch (TargetInvocationException error)
        {
            rejection = error.InnerException ?? error;
        }
        rejection.ShouldBeNull();
    }

    /// <summary>Active-world settings support the largest UI scale and actual remapped references.</summary>
    [Fact]
    public void Validate_Accepts150PercentActiveWorldSettings()
    {
        using var state = JsonDocument.Parse(json: """
                {"id":"settings.held-out","surface":"settings","title":"Élodie's settings",
                "status":"ready","backstop":"none","world":"World continues",
                "preferences":{"resolution":"1280x720","scale":150,"fullscreen":false,
                "master":75,"effects":65,"ambience":35},
                "bindings":[{"action":"Inspect","current":"J","default":"E","state":"bound"}],
                "placeholder":"Press {binding} to inspect {itemId} for {playerName}.",
                "expanded":"Jérôme retains the complete current reference.","diagnostics":[]}
                """);
        Exception? rejection = null;
        try
        {
            Call(name: "Validate", arguments: [state.RootElement]);
        }
        catch (TargetInvocationException error)
        {
            rejection = error.InnerException ?? error;
        }
        rejection.ShouldBeNull();
    }

    private static object? Call(string name, object[] arguments)
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        Type subject = assembly.GetType(name: "KingdomZombies.Inspection.SessionLayouts", throwOnError: true)
                ?? throw new InvalidOperationException();
        MethodInfo method = subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException();
        return method.Invoke(obj: null, parameters: arguments);
    }
}
