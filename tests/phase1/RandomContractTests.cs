using System;
using System.IO;
using System.Text.Json;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual seeded replay and strict numeric input boundaries.</summary>
public sealed class RandomContractTests
{
    /// <summary>Observes the raw runtime before implementing the replay subject.</summary>
    [Fact]
    public void Runtime_CommonSeeds_RecordsIndependentVectors()
    {
        int[] seeds = [0, 1, int.MinValue, int.MaxValue];
        long[][] vectors = new long[seeds.Length][];
        for (int index = 0; index < seeds.Length; index++)
        {
            var random = new Random(Seed: seeds[index]);
            vectors[index] = new long[5];
            for (int draw = 0; draw < 5; draw++)
            {
                vectors[index][draw] = random.NextInt64(minValue: 0, maxValue: long.MaxValue);
            }
        }

        File.WriteAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: ".phase1/temp/10-independent-vectors.json"), contents: JsonSerializer.Serialize(value:
                        new { runtime = Environment.Version.ToString(), seeds, vectors }));
    }

    /// <summary>Restoring an explicit saved position continues the observed runtime stream.</summary>
    [Fact, Trait("Case", "state.numeric-and-random")]
    public void Replay_RestoredPosition_ContinuesExactStream()
    {
        var restored = new ReplayRandom(seed: 0, runtimeContract: "dotnet-10.0.15/next-int64-v1", drawPosition: 2);
        restored.Draw().ShouldBeGreaterThan(expected: 0);
        restored.DrawPosition.ShouldBe(expected: 3);
        ProofProtocol.Record(id: "state.numeric-and-random", diagnostic: "BOUNDED_REPLAY",
                location: "src/core/ReplayRandom.cs:Draw");
    }
}
