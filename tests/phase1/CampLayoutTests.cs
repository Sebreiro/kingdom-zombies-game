using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

    /// <summary>The generic entry point never inherits the authenticated map dependency allowance.</summary>
    [Fact]
    public void Reader_GenericArraysKeep256Limit()
    {
        byte[] accepted = Encoding.UTF8.GetBytes(s: "[" + string.Join(separator: ",",
                values: Enumerable.Repeat(element: "0", count: 256)) + "]");
        Call(name: "Read", arguments: [accepted]).ShouldNotBeNull();
        byte[] rejected = Encoding.UTF8.GetBytes(s: "[" + string.Join(separator: ",",
                values: Enumerable.Repeat(element: "0", count: 257)) + "]");
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () =>
                Call(name: "Read", arguments: [rejected]));
        error.InnerException.ShouldBeOfType<InvalidDataException>().Message.ShouldBe(expected: "CAMP_ARRAY_COUNT");
    }

    /// <summary>Muting adds status while retaining the entire critical warning.</summary>
    [Fact]
    public void Warning_MutedRetainsFullInformation()
    {
        Call(name: "WarningText", arguments: ["Retaliation: left portal in10 seconds.", true])
                .ShouldBe(expected: "Retaliation: left portal in10 seconds. · Sound muted");
    }

    /// <summary>An undiscovered portal is omitted from every presentation.</summary>
    [Fact]
    public void Map_UnknownPortalIsHidden()
    {
        Call(name: "PortalVisible", arguments: [false]).ShouldBe(expected: false);
    }

    /// <summary>Waiting spectators receive an explicit reason instead of ownership controls.</summary>
    [Fact]
    public void Spectator_UnavailableOwnershipHasReason()
    {
        Call(name: "SpectatorReason", arguments: [false])
                .ShouldBe(expected: "No eligible survivor; request waits without ownership actions.");
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
