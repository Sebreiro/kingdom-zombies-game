using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Builds independent exact proof inputs and compares complete state fingerprints.</summary>
internal static class SnapshotProofData
{
    /// <summary>Creates coherent non-game roots with independently varying fields.</summary>
    internal static ExampleState State(int count = 2, long total = 9, int seed = 1, long position = 2)
    {
        StableIdentity[] ordered = [..Enumerable.Range(start: 0, count: count).Select(selector: index =>
                new StableIdentity(value: "id" + index.ToString(provider: CultureInfo.InvariantCulture)))];
        return new ExampleState(elapsedMilliseconds: total, ordered: ordered,
                records: ordered.Select(selector: identity => new KeyValuePair<StableIdentity, long>(
                        key: identity, value: total)), seed: seed, drawPosition: position);
    }

    /// <summary>Reads a fresh JSON tree for a single deliberate input mutation.</summary>
    internal static JsonObject Document(int count = 2)
    {
        return JsonNode.Parse(utf8Json: SnapshotCodec.Encode(state: State(count: count)))?.AsObject()
                ?? throw new InvalidOperationException(message: "PROOF_DOCUMENT_REQUIRED");
    }

    /// <summary>Runs the real codec over a mutated tree.</summary>
    internal static ExampleState Decode(JsonObject document)
    {
        return SnapshotCodec.Decode(document: Encoding.UTF8.GetBytes(s: document.ToJsonString()));
    }

    /// <summary>Requires the actual strict boundary to reject an input with a subject diagnostic.</summary>
    internal static Exception Reject(string document, string diagnostic)
    {
        Exception error = Should.Throw<Exception>(actual: () => SnapshotCodec.Decode(
                document: Encoding.UTF8.GetBytes(s: document)));
        error.Message.ShouldContain(expected: diagnostic);
        return error;
    }

    /// <summary>Compares all root fields, exact order and unordered semantic values.</summary>
    internal static void Equal(ExampleState actual, ExampleState expected)
    {
        actual.ElapsedMilliseconds.ShouldBe(expected: expected.ElapsedMilliseconds);
        actual.Kind.ShouldBe(expected: expected.Kind);
        actual.Seed.ShouldBe(expected: expected.Seed);
        actual.DrawPosition.ShouldBe(expected: expected.DrawPosition);
        actual.RandomContractVersion.ShouldBe(expected: expected.RandomContractVersion);
        actual.Ordered.Select(selector: item => item.Value).ShouldBe(expected:
                expected.Ordered.Select(selector: item => item.Value));
        actual.Records.Count.ShouldBe(expected: expected.Records.Count);
        foreach (KeyValuePair<StableIdentity, long> record in expected.Records)
        {
            actual.Records[record.Key].ShouldBe(expected: record.Value);
        }
    }

    /// <summary>Reconstructs the explicit next random draw without advancing the persisted root.</summary>
    internal static long Next(ExampleState state)
    {
        var random = new ReplayRandom(seed: state.Seed, runtimeContract: state.RandomContractVersion,
                drawPosition: state.DrawPosition);
        return random.Draw();
    }
}
