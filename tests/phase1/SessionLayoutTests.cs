using System;
using System.IO;
using System.Reflection;
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
