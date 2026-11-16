using System;
using System.Collections;
using System.IO;
using System.Reflection;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises the real portable pack loader independently of a native game window.</summary>
public sealed class ContentInspectorTests
{
    /// <summary>A missing independent shard cannot hide the exact valid bow or independent census.</summary>
    [Fact]
    public void Load_PartialPack_PreservesBowAndCensus()
    {
        Type? type = typeof(ProofValue).Assembly.GetType(name: "KingdomZombies.Core.ContentPack");
        MethodInfo? method = type?.GetMethod(name: "Load");
        Func<string, byte[]?> read = path => File.Exists(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/" + path)) ? File.ReadAllBytes(path: Path.Combine(
                        path1: ProofProtocol.Root, path2: "src/godot-game/" + path)) : null;
        object? pack = method?.Invoke(obj: null, parameters:
        [
            read(arg: "content/pack-index.json"), read(arg: "content/expected-obligations.json"), read,
        ]);
        object? entries = type?.GetProperty(name: "Entries")?.GetValue(obj: pack);
        (entries is IDictionary map && map.Contains(key: "common.bow")).ShouldBeTrue(
                customMessage: "A valid real bow remains inspectable despite 28 missing independent shards");
        object? expected = type?.GetProperty(name: "Expected")?.GetValue(obj: pack);
        (expected is IDictionary census ? census.Count : 0).ShouldBe(expected: 180);
    }
}
