using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks the compiled world inspector fence and independent actual native observations.</summary>
public sealed class WorldInspectorTests
{
    /// <summary>Returning to the same resource in a replacement owner cannot admit its old completion.</summary>
    [Fact]
    public void ReplacementOwner_RejectsLateSameResourceCompletion()
    {
        long oldEpoch = Next(previous: 0);
        long replacementEpoch = Next(previous: 0);
        Accepts(completion: oldEpoch, current: replacementEpoch, oldId: "world.assembled",
                currentId: "world.assembled", accepting: true).ShouldBeFalse();
    }

    /// <summary>Late A cannot replace B and revoked owners cannot publish even a matching result.</summary>
    [Fact]
    public void Completion_RejectsOlderDifferentAndRevokedSelection()
    {
        long first = Next(previous: 0), second = Next(previous: first);
        Accepts(completion: first, current: second, oldId: "IT-07", currentId: "world.assembled",
                accepting: true).ShouldBeFalse();
        Accepts(completion: second, current: second, oldId: "IT-07", currentId: "IT-07",
                accepting: false).ShouldBeFalse();
        Accepts(completion: second, current: second, oldId: "it-07", currentId: "IT-07",
                accepting: true).ShouldBeFalse();
        Accepts(completion: second, current: second, oldId: "IT-07", currentId: "IT-07",
                accepting: true).ShouldBeTrue();
    }

    /// <summary>Every new owner in a rapid replacement sequence receives a distinct fence.</summary>
    [Fact]
    public void RapidReplacement_DoesNotReuseOwnerEpochs()
    {
        long[] epochs = [.. Enumerable.Range(start: 0, count: 64).Select(selector: _ => Next(previous: 0))];
        epochs.Distinct().Count().ShouldBe(expected: 64);
    }

    /// <summary>The two authorized scenes retain ordinal paths and reject aliases independently.</summary>
    [Fact]
    public void SceneBoundary_RejectsAliasWithoutWeakeningCanonicalPath()
    {
        Call(name: "InspectorResourcePath", arguments: ["inspection/ItemsWeaponsPreview.tscn"])
                .ShouldBe(expected: "inspection/ItemsWeaponsPreview.tscn");
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () =>
                Call(name: "InspectorResourcePath", arguments: ["inspection/./ItemsWeaponsPreview.tscn"]));
        error.InnerException?.Message.ShouldBe(expected:
                "WORLD_INSPECTOR_SCENE_PATH inspection/./ItemsWeaponsPreview.tscn");
    }

    /// <summary>Six held-out UI states have separate successful observations from the real native hub.</summary>
    [Fact]
    public void Native_BackstopsHaveSixIndependentOutcomes()
    {
        using JsonDocument report = Native();
        JsonElement outcomes = report.RootElement.GetProperty(propertyName: "outcomes");
        foreach (string id in new[] { "empty", "loading", "error", "populated", "overflow", "long-text" })
        {
            JsonElement[] rows = [.. outcomes.EnumerateArray().Where(predicate: row => StringComparer.Ordinal.Equals(
                    x: row.GetProperty(propertyName: "id").GetString(), y: "WORLD-ITEM-PREVIEW." + id))];
            rows.Length.ShouldBe(expected: 1);
            rows[0].GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
        }
    }

    /// <summary>Native transforms and partial failures retain independent positive controls.</summary>
    [Fact]
    public void Native_RestoresValidNeighborsAndAuthoredPositions()
    {
        using JsonDocument report = Native();
        JsonElement[] rows = [.. report.RootElement.GetProperty(propertyName: "outcomes").EnumerateArray()];
        foreach (string id in new[] { "partial-valid-world", "restored-positive", "reader-restored-world",
                "persisted-world.assembled-reselected-authored-positions", "persisted-world.effects-reset-quiet",
                "persisted-IT-07-reset-art-only", "persisted-IT-15-reselected-art-only",
                "persisted-BL-01-reset-nested-controls", "persisted-BL-04-reselected-nested-controls" })
        {
            rows.Single(predicate: row => StringComparer.Ordinal.Equals(
                    x: row.GetProperty(propertyName: "id").GetString(), y: id))
                    .GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
        }
    }

    private static readonly Type _subject = Subject();

    private static long Next(long previous)
    {
        return (long)(Call(name: "NextInspectorEpoch", arguments: [previous])
                ?? throw new InvalidOperationException());
    }

    private static bool Accepts(long completion, long current, string oldId, string currentId, bool accepting)
    {
        return (bool)(Call(name: "InspectorAccepts", arguments: [completion, current, oldId, currentId, accepting])
                ?? throw new InvalidOperationException());
    }

    private static object? Call(string name, object[] arguments)
    {
        MethodInfo method = _subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException(message: name);
        return method.Invoke(obj: null, parameters: arguments);
    }

    private static Type Subject()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        return assembly.GetType(name: "KingdomZombies.Inspection.WorldPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }

    private static JsonDocument Native()
    {
        return JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Environment.GetEnvironmentVariable(
                variable: "WORLD_INSPECTOR_REPORT") ?? throw new InvalidOperationException(
                        message: "WORLD_INSPECTOR_REPORT_REQUIRED")));
    }
}
