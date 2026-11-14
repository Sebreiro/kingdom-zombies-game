using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Owns the single strict schema-1 detached proof snapshot contract.</summary>
internal static class SnapshotCodec
{
    /// <summary>Limits complete UTF-8 input bytes, inclusive.</summary>
    internal const int MaxBytes = 1_048_576;
    /// <summary>Limits JSON container nesting, inclusive.</summary>
    internal const int MaxDepth = 16;
    /// <summary>Limits each identity collection, inclusive.</summary>
    internal const int MaxRecords = 1024;

    /// <summary>Encodes independent values using exact camelCase schema names.</summary>
    internal static byte[] Encode(ExampleState state)
    {
        ArgumentNullException.ThrowIfNull(argument: state);
        return SnapshotWriter.Encode(snapshot: new SnapshotDto(state: state));
    }

    /// <summary>Validates the entire detached candidate before returning any state.</summary>
    internal static ExampleState Decode(ReadOnlyMemory<byte> document)
    {
        using JsonDocument parsed = SnapshotJson.Parse(document: document);
        JsonElement root = parsed.RootElement;
        SnapshotJson.Fields(element: root, names: ["schemaVersion", "elapsedMilliseconds", "seed", "drawPosition",
            "randomContractVersion", "kind", "ordered", "records"]);
        if (SnapshotJson.Integer(element: root.GetProperty(propertyName: "schemaVersion")) != 1)
        {
            throw new FormatException(message: "SNAPSHOT_SCHEMA: schemaVersion must be 1");
        }

        ProofKind kind = ReadKind(element: root.GetProperty(propertyName: "kind"));
        StableIdentity[] ordered = ReadOrder(element: root.GetProperty(propertyName: "ordered"));
        KeyValuePair<StableIdentity, long>[] records = ReadRecords(element: root.GetProperty(propertyName: "records"));
        return new ExampleState(elapsedMilliseconds: SnapshotJson.Integer(element: root.GetProperty(
                propertyName: "elapsedMilliseconds")), ordered: ordered, records: records, kind: kind,
                seed: checked((int)SnapshotJson.Integer(element: root.GetProperty(propertyName: "seed"))),
                drawPosition: SnapshotJson.Integer(element: root.GetProperty(propertyName: "drawPosition")),
                randomContractVersion: SnapshotJson.Text(element: root.GetProperty(
                        propertyName: "randomContractVersion")));
    }

    private static ProofKind ReadKind(JsonElement element)
    {
        if (!StringComparer.Ordinal.Equals(x: SnapshotJson.Text(element: element), y: "Sample"))
        {
            throw new FormatException(message: "SNAPSHOT_KIND: exact pinned Sample name required");
        }

        return ProofKind.Sample;
    }

    private static StableIdentity[] ReadOrder(JsonElement element)
    {
        SnapshotJson.Collection(element: element);
        return [.. element.EnumerateArray().Select(selector: item =>
                new StableIdentity(value: SnapshotJson.Text(element: item)))];
    }

    private static KeyValuePair<StableIdentity, long>[] ReadRecords(JsonElement element)
    {
        SnapshotJson.Collection(element: element);
        var records = new List<KeyValuePair<StableIdentity, long>>();
        var identities = new HashSet<StableIdentity>();
        foreach (JsonElement record in element.EnumerateArray())
        {
            SnapshotJson.Fields(element: record, names: ["id", "value"]);
            var identity = new StableIdentity(value: SnapshotJson.Text(element: record.GetProperty(
                    propertyName: "id")));
            long value = SnapshotJson.Integer(element: record.GetProperty(propertyName: "value"));
            ArgumentOutOfRangeException.ThrowIfNegative(value: value, paramName: nameof(element));
            if (!identities.Add(item: identity))
            {
                throw new FormatException(message: "SNAPSHOT_DUPLICATE_ID: duplicate or conflicting identity");
            }

            records.Add(item: new(key: identity, value: value));
        }

        return [.. records];
    }
}
