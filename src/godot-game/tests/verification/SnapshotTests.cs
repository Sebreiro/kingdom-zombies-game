using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Executes the real strict snapshot boundary against detached Core states.</summary>
public sealed class SnapshotTests
{
    /// <summary>Restores exact values without modifying the source root.</summary>
    [Fact, Trait("Case", "state.snapshot")]
    public void Codec_DetachedState_RoundTripsExactValues()
    {
        var identity = new StableIdentity(value: "é😀");
        var root = new ExampleState(elapsedMilliseconds: 9007199254740993, ordered: [identity],
                records: new Dictionary<StableIdentity, long> { [identity] = long.MaxValue });
        ExampleState restored = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: root));
        restored.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        restored.Records[identity].ShouldBe(expected: long.MaxValue);
        restored.Ordered[0].Value.ShouldBe(expected: "é😀");
        restored.ShouldNotBeSameAs(expected: root);
        root.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        SnapshotProofData.Equal(actual: restored, expected: root);
        SnapshotProofData.Next(state: restored).ShouldBe(expected: SnapshotProofData.Next(state: root));
        ProofProtocol.Record(id: "state.snapshot", diagnostic: "STRICT_SNAPSHOT_ROUNDTRIP",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode");
    }

    /// <summary>Preserves all fields and continuation in independently allocated roots.</summary>
    [Fact, Trait("Case", "ac-20.complete")]
    public void Acceptance_ExactState_PreservesContinuation()
    {
        foreach (int count in new[] { 0, 1, 7 })
        {
            ExampleState source = SnapshotProofData.State(count: count, total: long.MaxValue, seed: int.MinValue);
            ExampleState actual = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: source));
            SnapshotProofData.Equal(actual: actual, expected: source);
            SnapshotProofData.Next(state: actual).ShouldBe(expected: SnapshotProofData.Next(state: source));
            source.DrawPosition.ShouldBe(expected: 2);
        }

        ProofProtocol.Record(id: "ac-20.complete", diagnostic: "EXACT_SNAPSHOT_CONTINUATION",
                location: "src/godot-game/core/SnapshotCodec.cs:Encode/Decode; src/godot-game/core/ExampleState.cs");
    }

    /// <summary>Distinguishes every missing or null field from valid empty collections.</summary>
    [Fact, Trait("Case", "ac-21.complete")]
    public void Acceptance_RequiredFields_RejectsMissingAndNull()
    {
        string[] names = [.. SnapshotProofData.Document().Select(selector: pair => pair.Key)];
        foreach (string name in names)
        {
            JsonObject absent = SnapshotProofData.Document();
            absent.Remove(propertyName: name).ShouldBeTrue();
            SnapshotProofData.Reject(document: absent.ToJsonString(), diagnostic: "SNAPSHOT_FIELDS");
            JsonObject nil = SnapshotProofData.Document();
            nil[name] = null;
            Should.Throw<Exception>(actual: () => SnapshotProofData.Decode(document: nil));
        }

        foreach (string root in new[] { "", "null", "[]", "{}", "true", "0", "\"text\"" })
        {
            SnapshotProofData.Reject(document: root, diagnostic: "SNAPSHOT_");
        }

        ProofProtocol.Record(id: "ac-21.complete", diagnostic: "REQUIRED_SNAPSHOT_REJECTED",
                location: "src/godot-game/core/SnapshotJson.cs:Fields/Collection/Text/Integer");
    }

    /// <summary>Tests nearest byte, nesting and collection limits through the actual parser/model boundaries.</summary>
    [Fact, Trait("Case", "edge.rule-07.adjacency")]
    public void Bounds_NearestNeighbors_EnforcesAllLimits()
    {
        byte[] small = SnapshotCodec.Encode(state: SnapshotProofData.State(count: 0));
        foreach (int length in new[] { SnapshotCodec.MaxBytes - 1, SnapshotCodec.MaxBytes })
        {
            byte[] padded = [.. small, .. Enumerable.Repeat(element: (byte)' ', count: length - small.Length)];
            SnapshotCodec.Decode(document: padded).Records.ShouldBeEmpty();
        }

        byte[] oversized = [.. small, .. Enumerable.Repeat(element: (byte)' ',
                count: SnapshotCodec.MaxBytes + 1 - small.Length)];
        Should.Throw<FormatException>(actual: () => SnapshotCodec.Decode(document: oversized))
                .Message.ShouldContain(expected: "SNAPSHOT_BYTES");
        AssertDepthNeighbors();
        foreach (int count in new[] { 1023, 1024 })
        {
            ExampleState state = SnapshotProofData.State(count: count);
            SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: state)).Records.Count.ShouldBe(expected: count);
        }

        Should.Throw<ArgumentException>(actual: () => SnapshotProofData.State(count: 1025));
        JsonObject many = SnapshotProofData.Document(count: 1024);
        many["records"]?.AsArray().Add(item: JsonNode.Parse(json: "{\"id\":\"extra\",\"value\":0}"));
        SnapshotProofData.Reject(document: many.ToJsonString(), diagnostic: "SNAPSHOT_COLLECTION");
        JsonObject order = SnapshotProofData.Document(count: 1024);
        order["ordered"]?.AsArray().Add(item: JsonValue.Create(value: "extra"));
        SnapshotProofData.Reject(document: order.ToJsonString(), diagnostic: "SNAPSHOT_COLLECTION");
        ProofProtocol.Record(id: "edge.rule-07.adjacency", diagnostic: "BOUNDED_SNAPSHOT_NEIGHBORS",
                location: "src/godot-game/core/SnapshotJson.cs:Parse/Collection; src/godot-game/core/"
                        + "ExampleState.cs:ValidateRecords");
    }

    /// <summary>Accepts empty/singleton/multiple while rejecting null records, IDs and collections.</summary>
    [Fact, Trait("Case", "edge.rule-07.empty")]
    public void Collections_EmptyAndNull_DistinguishesRequiredState()
    {
        foreach (int count in new[] { 0, 1, 3 })
        {
            SnapshotProofData.Decode(document: SnapshotProofData.Document(count: count)).Records.Count
                    .ShouldBe(expected: count);
        }

        foreach (string replacement in new[] { "null", "[null]", "{}", "[{}]" })
        {
            JsonObject document = SnapshotProofData.Document();
            document["records"] = JsonNode.Parse(json: replacement);
            SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_");
        }

        foreach (string identity in new[] { "null", "\"\"", "1", "[]" })
        {
            JsonObject document = SnapshotProofData.Document(count: 1);
            document["ordered"] = JsonNode.Parse(json: "[" + identity + "]");
            Should.Throw<Exception>(actual: () => SnapshotProofData.Decode(document: document));
        }

        ProofProtocol.Record(id: "edge.rule-07.empty", diagnostic: "EMPTY_DIFFERS_FROM_ABSENT",
                location: "src/godot-game/core/SnapshotCodec.cs:ReadOrder/ReadRecords");
    }

    /// <summary>Rejects invalid UTF-8, syntax and identity text without normalizing valid Unicode.</summary>
    [Fact, Trait("Case", "edge.rule-07.encoding")]
    public void Encoding_ExactUnicode_RejectsMalformedInputs()
    {
        string[] text = ["x", "X", "é", "e\u0301", "😀", " ", " x", "x "];
        StableIdentity[] ids = [.. text.Select(selector: value => new StableIdentity(value: value))];
        var state = new ExampleState(elapsedMilliseconds: 1, ordered: ids,
                records: ids.Select(selector: id => new KeyValuePair<StableIdentity, long>(key: id, value: 7)));
        ExampleState restored = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: state));
        SnapshotProofData.Equal(actual: restored, expected: state);
        foreach (byte[] invalid in new byte[][] { [0xff], [0xc3], [0xed, 0xa0, 0x80], [123, 34, 0xc3, 34, 125] })
        {
            Should.Throw<DecoderFallbackException>(actual: () => SnapshotCodec.Decode(document: invalid));
        }

        foreach (string invalid in new[] { "{", "{}{}", "{\"x\":1,}", "/*x*/{}", "NaN", "Infinity" })
        {
            Should.Throw<JsonException>(actual: () => SnapshotCodec.Decode(
                    document: Encoding.UTF8.GetBytes(s: invalid)));
        }

        AssertInvalidIdentities();
        byte[] complete = SnapshotCodec.Encode(state: state);
        Should.Throw<JsonException>(actual: () => SnapshotCodec.Decode(document: complete.AsMemory()[..^1]));
        ProofProtocol.Record(id: "edge.rule-07.encoding", diagnostic: "STRICT_UTF8_ORDINAL_IDENTITIES",
                location: "src/godot-game/core/SnapshotJson.cs:Parse/Text; src/godot-game/core/"
                        + "StableIdentity.cs:ValidateText");
    }

    /// <summary>Accepts unordered permutations while retaining meaningful order exactly.</summary>
    [Fact, Trait("Case", "edge.rule-07.ordering")]
    public void Ordering_UnorderedPermutation_PreservesSemantics()
    {
        JsonObject document = SnapshotProofData.Document(count: 7);
        ExampleState original = SnapshotProofData.Decode(document: document);
        var shuffle = new Random(Seed: 117);
        for (int run = 0; run < 12; run++)
        {
            JsonArray source = document["records"]?.AsArray()
                    ?? throw new InvalidOperationException(message: "PROOF_RECORDS_REQUIRED");
            JsonNode?[] records = [..source.Select(selector: item => item?.DeepClone())
                    .OrderBy(keySelector: _ => shuffle.Next())];
            document["records"] = new JsonArray(items: records);
            SnapshotProofData.Equal(actual: SnapshotProofData.Decode(document: document), expected: original);
        }

        document["ordered"] = new JsonArray(items: original.Ordered.Reverse().Select(selector: item =>
                JsonValue.Create(value: item.Value) as JsonNode).ToArray());
        ExampleState reversed = SnapshotProofData.Decode(document: document);
        reversed.Ordered[0].Value.ShouldBe(expected: original.Ordered[^1].Value);
        SnapshotProofData.Equal(actual: SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: reversed)),
                expected: reversed);
        ProofProtocol.Record(id: "edge.rule-07.ordering", diagnostic: "ORDERED_AND_UNORDERED_SEMANTICS",
                location: "src/godot-game/core/SnapshotCodec.cs:ReadOrder/ReadRecords");
    }

    /// <summary>Tests exact integer neighbors, seed/position/version and duplicate identity failures.</summary>
    [Fact, Trait("Case", "edge.rule-07.precision")]
    public void Numbers_ExactNeighbors_RejectsLossyTokens()
    {
        foreach (long value in new[] { 0, 1, 9007199254740991, 9007199254740992, 9007199254740993,
            long.MaxValue - 1, long.MaxValue })
        {
            ExampleState root = SnapshotProofData.State(total: value, seed: int.MaxValue);
            SnapshotProofData.Equal(actual: SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: root)),
                    expected: root);
        }

        foreach (string token in new[] { "-1", "1.0", "1e0", "9223372036854775808", "\"1\"", "null" })
        {
            JsonObject document = SnapshotProofData.Document();
            document["elapsedMilliseconds"] = JsonNode.Parse(json: token);
            Should.Throw<Exception>(actual: () => SnapshotProofData.Decode(document: document));
            JsonObject recordDocument = SnapshotProofData.Document();
            JsonNode record = recordDocument["records"]?[0]
                    ?? throw new InvalidOperationException(message: "PROOF_RECORD_REQUIRED");
            record["value"] = JsonNode.Parse(json: token);
            Should.Throw<Exception>(actual: () => SnapshotProofData.Decode(document: recordDocument));
        }

        AssertVersionAndIdentityFailures();
        ProofProtocol.Record(id: "edge.rule-07.precision", diagnostic: "EXACT_INT64_SNAPSHOT",
                location: "src/godot-game/core/SnapshotJson.cs:Integer; src/godot-game/core/"
                        + "ExampleState.cs:ValidateRandom");
    }

    /// <summary>Exercises R-68 positive against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-68.positive")]
    public void CamelCase_AcceptedSchema_ContainsExactNames()
    {
        using var parsed = JsonDocument.Parse(utf8Json: SnapshotCodec.Encode(
                state: SnapshotProofData.State(count: 0)));
        parsed.RootElement.EnumerateObject().Select(selector: item => item.Name).ShouldBe(expected: _schemaFields);
        parsed.RootElement.GetProperty(propertyName: "kind").GetString().ShouldBe(expected: "sample");
        ProofProtocol.Record(id: "R-68.positive", diagnostic: "PINNED_CAMELCASE",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-68 negative against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-68.negative")]
    public void CamelCase_WrongCase_RejectsSchema()
    {
        JsonObject document = SnapshotProofData.Document();
        document["ElapsedMilliseconds"] = document["elapsedMilliseconds"]?.DeepClone();
        document.Remove(propertyName: "elapsedMilliseconds").ShouldBeTrue();
        SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_FIELDS");
        ProofProtocol.Record(id: "R-68.negative", diagnostic: "SNAPSHOT_FIELDS",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-69 positive against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-69.positive")]
    public void Enum_ExactName_AcceptsPinnedValue()
    {
        FieldInfo member = typeof(ProofKind).GetField(name: nameof(ProofKind.Sample))
                ?? throw new InvalidOperationException(message: "PINNED_KIND_MEMBER_REQUIRED");
        member.GetCustomAttribute<JsonStringEnumMemberNameAttribute>()?.Name.ShouldBe(expected: "sample");
        JsonObject document = SnapshotProofData.Document();
        document["kind"] = "sample";
        SnapshotProofData.Decode(document: document).Kind.ShouldBe(expected: ProofKind.Sample);
        ProofProtocol.Record(id: "R-69.positive", diagnostic: "PINNED_ENUM_NAME",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-69 negative against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-69.negative")]
    public void Enum_UnpinnedNames_RejectsEveryVariant()
    {
        foreach (string token in new[] { "\"Sample\"", "\"SAMPLE\"", "\"Unknown\"", "\"unknown\"",
            "\"1\"", "1", "0", "-1", "null", "\" Sample\"", "\"Future\"" })
        {
            JsonObject document = SnapshotProofData.Document();
            document["kind"] = JsonNode.Parse(json: token);
            SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_");
        }
        ProofProtocol.Record(id: "R-69.negative", diagnostic: "SNAPSHOT_KIND",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-70 positive against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-70.positive")]
    public void NullPolicy_EmptyCollections_WritesRequiredValues()
    {
        JsonObject document = SnapshotProofData.Document(count: 0);
        document.Count.ShouldBe(expected: 8);
        document.All(predicate: item => item.Value is not null).ShouldBeTrue();
        document["ordered"]?.AsArray().Count.ShouldBe(expected: 0);
        document["records"]?.AsArray().Count.ShouldBe(expected: 0);
        SnapshotProofData.Decode(document: document).Records.ShouldBeEmpty();
        ProofProtocol.Record(id: "R-70.positive", diagnostic: "STRICT_NULL_POLICY",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-70 negative against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-70.negative")]
    public void NullPolicy_UnknownFields_RejectsAtEveryLevel()
    {
        JsonObject document = SnapshotProofData.Document();
        document["future"] = 1;
        SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_FIELDS");
        document.Remove(propertyName: "future").ShouldBeTrue();
        JsonNode record = document["records"]?[0]
                ?? throw new InvalidOperationException(message: "PROOF_RECORD_REQUIRED");
        record["peerId"] = 1;
        SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_FIELDS");
        record.AsObject().Remove(propertyName: "peerId").ShouldBeTrue();
        record["id"] = null;
        SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_TEXT");
        ProofProtocol.Record(id: "R-70.negative", diagnostic: "SNAPSHOT_FIELDS",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-71 positive against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-71.positive")]
    public void RequiredValues_CompleteCollections_AcceptsAll()
    {
        foreach (int count in new[] { 0, 1, 2 })
        {
            ExampleState expected = SnapshotProofData.State(count: count);
            ExampleState actual = SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: expected));
            SnapshotProofData.Equal(actual: actual, expected: expected);
        }
        ProofProtocol.Record(id: "R-71.positive", diagnostic: "COMPLETE_REQUIRED_VALUES",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-71 negative against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-71.negative")]
    public void RequiredValues_MissingRecordFields_RejectsAll()
    {
        foreach (string name in new[] { "id", "value" })
        {
            JsonObject document = SnapshotProofData.Document();
            JsonObject record = document["records"]?[0]?.AsObject()
                    ?? throw new InvalidOperationException(message: "PROOF_RECORD_REQUIRED");
            record.Remove(propertyName: name).ShouldBeTrue();
            SnapshotProofData.Reject(document: document.ToJsonString(), diagnostic: "SNAPSHOT_FIELDS");
        }
        JsonObject mismatch = SnapshotProofData.Document(count: 1);
        mismatch["ordered"] = new JsonArray();
        Should.Throw<ArgumentException>(actual: () => SnapshotProofData.Decode(document: mismatch));
        ProofProtocol.Record(id: "R-71.negative", diagnostic: "SNAPSHOT_FIELDS",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-72 positive against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-72.positive")]
    public void SharedCodec_IndependentRuns_UsesExactContract()
    {
        ExampleState original = SnapshotProofData.State(seed: int.MinValue, position: 999999);
        byte[] first = SnapshotCodec.Encode(state: original);
        byte[] second = SnapshotCodec.Encode(state: SnapshotCodec.Decode(document: first));
        first.ShouldBe(expected: second);
        SnapshotProofData.Equal(actual: SnapshotCodec.Decode(document: second), expected: original);
        ProofProtocol.Record(id: "R-72.positive", diagnostic: "ONE_STRICT_CODEC",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Exercises R-72 negative against the actual shared strict codec.</summary>
    [Fact, Trait("Case", "R-72.negative")]
    public void SharedCodec_DuplicateProperties_RejectsBeforeConversion()
    {
        string original = Encoding.UTF8.GetString(bytes: SnapshotCodec.Encode(state: SnapshotProofData.State()));
        foreach ((string oldValue, string newValue) in new[] {
            ("\"schemaVersion\":1", "\"schemaVersion\":1,\"schemaVersion\":1"),
            ("\"schemaVersion\":1", "\"schemaVersion\":1,\"schemaVersion\":2"),
            ("\"id\":\"id0\"", "\"id\":\"id0\",\"id\":\"other\""),
            ("\"value\":9", "\"value\":9,\"value\":8"),
            ("\"kind\":\"sample\"", "\"future\":{\"x\":0,\"x\":1},\"kind\":\"sample\"") })
        {
            string duplicate = original.Replace(oldValue: oldValue, newValue: newValue,
                    comparisonType: StringComparison.Ordinal);
            StringComparer.Ordinal.Equals(x: duplicate, y: original).ShouldBeFalse();
            SnapshotProofData.Reject(document: duplicate, diagnostic: "SNAPSHOT_DUPLICATE_PROPERTY");
        }
        ProofProtocol.Record(id: "R-72.negative", diagnostic: "SNAPSHOT_DUPLICATE_PROPERTY",
                location: "src/godot-game/core/SnapshotCodec.cs:Decode; src/godot-game/core/SnapshotJson.cs");
    }

    /// <summary>Rejects reference leaks and mutable fields in actual compiled state/DTO types.</summary>
    [Fact, Trait("Case", "state.snapshot-shape")]
    public void Shape_CompiledState_ContainsIndependentValues()
    {
        SnapshotShapePolicy.Independent(type: typeof(ExampleState)).ShouldBeTrue();
        SnapshotShapePolicy.Independent(type: typeof(SnapshotDto)).ShouldBeTrue();
        SnapshotShapePolicy.Independent(type: typeof(object)).ShouldBeFalse();
        SnapshotShapePolicy.Independent(type: typeof(Dictionary<string, long>)).ShouldBeFalse();
        SnapshotShapePolicy.Independent(type: typeof(Uri)).ShouldBeFalse();
        ProofProtocol.Record(id: "state.snapshot-shape", diagnostic: "COMPILED_INDEPENDENT_STATE",
                location: "src/godot-game/core/ExampleState.cs; src/godot-game/core/"
                        + "SnapshotDto.cs; compiled property/backing-field types");
    }

    private static readonly string[] _schemaFields = ["schemaVersion", "elapsedMilliseconds", "seed", "drawPosition",
        "randomContractVersion", "kind", "ordered", "records"];

    private static void AssertDepthNeighbors()
    {
        foreach (int depth in new[] { 15, 16 })
        {
            byte[] nested = Encoding.UTF8.GetBytes(s: new string(c: '[', count: depth) + "0"
                    + new string(c: ']', count: depth));
            using JsonDocument parsed = SnapshotJson.Parse(document: nested);
            parsed.RootElement.ValueKind.ShouldBe(expected: JsonValueKind.Array);
            Should.Throw<FormatException>(actual: () => SnapshotCodec.Decode(document: nested))
                    .Message.ShouldContain(expected: "SNAPSHOT_OBJECT");
        }

        byte[] excessive = Encoding.UTF8.GetBytes(s: new string(c: '[', count: 17) + "0"
                + new string(c: ']', count: 17));
        Should.Throw<JsonException>(actual: () => SnapshotCodec.Decode(document: excessive));
    }

    private static void AssertInvalidIdentities()
    {
        foreach (int length in new[] { 127, 128 })
        {
            var id = new StableIdentity(value: new string(c: 'é', count: length));
            var state = new ExampleState(elapsedMilliseconds: 0, ordered: [id],
                    records: [new(key: id, value: 0)]);
            SnapshotProofData.Equal(actual: SnapshotCodec.Decode(document: SnapshotCodec.Encode(state: state)),
                    expected: state);
        }

        string original = Encoding.UTF8.GetString(bytes: SnapshotCodec.Encode(
                state: SnapshotProofData.State(count: 1)));
        string tooLong = original.Replace(oldValue: "\"id0\"", newValue: "\"" + new string(c: 'x', count: 129) + "\"",
                comparisonType: StringComparison.Ordinal);
        SnapshotProofData.Reject(document: tooLong, diagnostic: "IDENTITY_LENGTH");
        foreach (string token in new[] { "\"\"", "null", "\"\\u0000\"", "\"\\n\"", "\"\\uD800\"", "\"\\uDC00\"" })
        {
            string invalid = original.Replace(oldValue: "\"id0\"", newValue: token,
                    comparisonType: StringComparison.Ordinal);
            Should.Throw<Exception>(actual: () => SnapshotCodec.Decode(document: Encoding.UTF8.GetBytes(s: invalid)));
        }
    }

    private static void AssertVersionAndIdentityFailures()
    {
        foreach ((string field, string token) in new[] { ("schemaVersion", "0"), ("schemaVersion", "2"),
            ("seed", "2147483648"), ("seed", "-2147483649"), ("drawPosition", "-1"),
            ("drawPosition", "1000001"), ("randomContractVersion", "\"dotnet-10.0.15/next-int64-v2\"") })
        {
            JsonObject document = SnapshotProofData.Document();
            document[field] = JsonNode.Parse(json: token);
            Should.Throw<Exception>(actual: () => SnapshotProofData.Decode(document: document));
        }

        foreach (long position in new[] { 0L, 1, 999999, 1000000 })
        {
            JsonObject document = SnapshotProofData.Document();
            document["drawPosition"] = position;
            SnapshotProofData.Decode(document: document).DrawPosition.ShouldBe(expected: position);
        }

        JsonObject duplicate = SnapshotProofData.Document();
        duplicate["records"]?.AsArray().Add(item: duplicate["records"]?[0]?.DeepClone());
        SnapshotProofData.Reject(document: duplicate.ToJsonString(), diagnostic: "SNAPSHOT_DUPLICATE_ID");
        JsonNode conflict = duplicate["records"]?[2]
                ?? throw new InvalidOperationException(message: "PROOF_CONFLICT_REQUIRED");
        conflict["value"] = 99;
        SnapshotProofData.Reject(document: duplicate.ToJsonString(), diagnostic: "SNAPSHOT_DUPLICATE_ID");
        JsonObject order = SnapshotProofData.Document();
        JsonArray identities = order["ordered"]?.AsArray()
                ?? throw new InvalidOperationException(message: "PROOF_ORDER_REQUIRED");
        identities[1] = "id0";
        Should.Throw<ArgumentException>(actual: () => SnapshotProofData.Decode(document: order));
    }
}
