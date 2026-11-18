using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves compiled audio ownership independently of eventual listening judgments.</summary>
public sealed class AudioPresentationTests
{
    /// <summary>Replacement owners cannot revive an earlier owner with the same local state.</summary>
    [Fact]
    public void ReplacementOwners_ReceiveStrictlyIncreasingEpochs()
    {
        long first = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        long second = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        second.ShouldBeGreaterThan(expected: first);
    }

    /// <summary>The actual generated food source resolves its declared deterministic provenance.</summary>
    [Fact]
    public void Sources_ResolveGeneratedFoodAuthority()
    {
        Source(id: "interface.food-crunch.v1").ShouldContain(expected: "seed190021");
    }

    /// <summary>The derived burst resolves its current shared source without approval extension.</summary>
    [Fact]
    public void Sources_ResolveCurrentSharedAction()
    {
        Source(id: "interface.bloater-burst.v1").ShouldContain(expected: "2790");
    }

    private static readonly Type _subject = Subject();

    private static string Source(string id)
    {
        JsonElement pack = Pack(file: "audio-interface-ambience");
        JsonElement sample = pack.GetProperty(propertyName: "samples").EnumerateArray()
                .Single(predicate: value => StringComparer.Ordinal.Equals(
                        x: value.GetProperty(propertyName: "id").GetString(), y: id));
        try
        {
            return (string)(Call(name: "ResolveSource", arguments: [pack, sample, Pack(file: "audio-actions")])
                    ?? throw new InvalidOperationException());
        }
        catch (TargetInvocationException error) when (error.InnerException is InvalidOperationException
                && error.InnerException.Message is "Sequence contains no matching element")
        {
            return "Production ordinary-source lookup rejected this valid sample";
        }
    }

    private static JsonElement Pack(string file)
    {
        using var document = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(
                path1: ProofProtocol.Root, path2: "src/godot-game/content/packs/" + file + ".json")));
        return document.RootElement.Clone();
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
        return assembly.GetType(name: "KingdomZombies.Inspection.AudioPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
