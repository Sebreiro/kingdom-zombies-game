using System;
using System.IO;
using System.Reflection;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Tests actual compiled inspection accounting rather than a test-owned game model.</summary>
public sealed class CampLayoutTests
{
    /// <summary>Sale proceeds belong to stash and carried food cannot increase its selected stock.</summary>
    [Fact]
    public void Sale_UsesStashFoodAndStashMoney()
    {
        Call(name: "SaleMoney", arguments: [7, 3, 2, 31, 99]).ShouldBe(expected: 37);
        Call(name: "SaleMoney", arguments: [0, 0, 4, 31, 99]).ShouldBe(expected: 31);
    }

    /// <summary>A paid but unfinished upgrade leaves the old fireside capacity available.</summary>
    [Fact]
    public void Main_PendingUpgradeKeepsCompletedCapacity()
    {
        Call(name: "Capacity", arguments: [1, 2]).ShouldBe(expected: 8);
        Call(name: "Capacity", arguments: [2, 3]).ShouldBe(expected: 12);
    }

    /// <summary>No target and an existing assault are separate actionable explanations.</summary>
    [Fact]
    public void Rally_NoTargetAndActiveHaveDistinctReasons()
    {
        Call(name: "RallyReason", arguments: [false, false, 8, 25])
                .ShouldBe(expected: "No surviving portal on this side.");
        Call(name: "RallyReason", arguments: [true, true, 8, 25])
                .ShouldBe(expected: "An assault is already active on this side.");
    }

    private static object? Call(string name, object[] arguments)
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        Type subject = assembly.GetType(name: "KingdomZombies.Inspection.CampLayouts", throwOnError: true)
                ?? throw new InvalidOperationException();
        MethodInfo method = subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException();
        return method.Invoke(obj: null, parameters: arguments);
    }
}
