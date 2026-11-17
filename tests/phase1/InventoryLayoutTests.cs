using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the real compiled inventory presenter contracts without native initialization.</summary>
public sealed class InventoryLayoutTests
{
    /// <summary>All eleven equipment locations preserve their exact distinct logical identities.</summary>
    [Fact]
    public void Slots_PreserveElevenNamedIdentities()
    {
        string[] actual = (string[])(Call(name: "SlotIds", arguments: []) ?? throw new InvalidOperationException());
        actual.ShouldBe(expected: _expectedSlots);
        actual.Distinct(comparer: StringComparer.Ordinal).Count().ShouldBe(expected: 11);
    }

    /// <summary>Rotation changes real cell geometry rather than only rotating an image.</summary>
    [Fact]
    public void Rotation_ChangesFootprint()
    {
        int[] actual = (int[])(Call(name: "Footprint", arguments: [1, 2, true])
                ?? throw new InvalidOperationException());
        actual.ShouldBe(expected: _rotated);
        ((int[])(Call(name: "Footprint", arguments: [1, 2, false])
                ?? throw new InvalidOperationException())).ShouldBe(expected: _unrotated);
    }

    /// <summary>Equal food labels never substitute for a lost exact shortcut reference or copy its quantity.</summary>
    [Fact]
    public void QuickSlot_UsesExactCurrentInstance()
    {
        using var state = JsonDocument.Parse(json: """
                {"quick":["berries-b",null,"missing","berries-a"],"items":[
                {"id":"berries-a","definition":"IT-08","label":"Berries","quantity":2},
                {"id":"berries-b","definition":"IT-08","label":"Berries","quantity":7}]}
                """);
        var actual = (JsonElement?)Call(name: "ResolveQuick", arguments: [state.RootElement, 0]);
        actual.HasValue.ShouldBeTrue();
        actual.Value.GetProperty(propertyName: "id").GetString().ShouldBe(expected: "berries-b");
        actual.Value.GetProperty(propertyName: "quantity").GetInt32().ShouldBe(expected: 7);
        Call(name: "ResolveQuick", arguments: [state.RootElement, 2]).ShouldBeNull();
    }

    private static readonly string[] _expectedSlots = ["headwear", "face", "torso", "legs", "body-armor", "gloves",
        "boots", "backpack", "belt", "left-hand", "right-hand"];
    private static readonly int[] _rotated = [2, 1];
    private static readonly int[] _unrotated = [1, 2];

    private static object? Call(string name, object[] arguments)
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        Type subject = assembly.GetType(name: "KingdomZombies.Inspection.InventoryLayouts", throwOnError: true)
                ?? throw new InvalidOperationException();
        MethodInfo method = subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException();
        return method.Invoke(obj: null, parameters: arguments);
    }
}
