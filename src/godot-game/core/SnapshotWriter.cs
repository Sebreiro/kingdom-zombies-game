using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Writes the single exact schema without serializer defaults or permissive enum converters.</summary>
internal static class SnapshotWriter
{
    /// <summary>Emits every required schema field including empty collections.</summary>
    internal static byte[] Encode(SnapshotDto snapshot)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(utf8Json: stream))
        {
            writer.WriteStartObject();
            writer.WriteNumber(propertyName: "schemaVersion", value: 1);
            writer.WriteNumber(propertyName: "elapsedMilliseconds", value: snapshot.ElapsedMilliseconds);
            writer.WriteNumber(propertyName: "seed", value: snapshot.Seed);
            writer.WriteNumber(propertyName: "drawPosition", value: snapshot.DrawPosition);
            writer.WriteString(propertyName: "randomContractVersion", value: snapshot.RandomContractVersion);
            writer.WriteString(propertyName: "kind", value: snapshot.Kind);
            WriteCollections(writer: writer, snapshot: snapshot);
            writer.WriteEndObject();
        }

        byte[] document = stream.ToArray();
        if (document.Length > SnapshotCodec.MaxBytes)
        {
            throw new FormatException(message: "SNAPSHOT_BYTES: encoded document exceeds 1048576 UTF-8 bytes");
        }

        return document;
    }

    private static void WriteCollections(Utf8JsonWriter writer, SnapshotDto snapshot)
    {
        writer.WriteStartArray(propertyName: "ordered");
        foreach (string identity in snapshot.Ordered)
        {
            writer.WriteStringValue(value: identity);
        }

        writer.WriteEndArray();
        writer.WriteStartArray(propertyName: "records");
        foreach (KeyValuePair<string, long> record in snapshot.Records)
        {
            writer.WriteStartObject();
            writer.WriteString(propertyName: "id", value: record.Key);
            writer.WriteNumber(propertyName: "value", value: record.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
