using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Assembles complete public worlds independently of the recipient's private control and items.</summary>
internal sealed class FoundationPublicWorld
{
    internal const int MaximumParts = 2048;
    internal const int MaximumRows = 262144;

    internal static ImmutableArray<byte[]> Encode(FoundationSnapshot roster, JsonElement world, string attempt)
    {
        JsonElement header = Header(world: world);
        var frame = new FoundationRunFrame { Body = header };
        FoundationEnvelope Envelope(FoundationRunFrame part) => new(kind: "world-part", sessionId: roster.SessionId,
                revision: roster.Revision, snapshot: roster, attemptId: attempt, game: part);
        ImmutableArray<JsonElement> rows = [.. _collections.SelectMany(selector: name => world
                .GetProperty(propertyName: name).EnumerateArray().Select(selector: row =>
                        JsonSerializer.SerializeToElement(value: new { Collection = name, Row = row })))];
        return FoundationProtocol.EncodeView(rows: rows, envelope: Envelope, template: frame,
                maximumParts: MaximumParts, maximumRows: MaximumRows);
    }

    internal (FoundationSnapshot roster, JsonElement world)? Offer(FoundationEnvelope envelope)
    {
        FoundationSnapshot roster = envelope.Snapshot ?? throw new InvalidDataException(message: "PUBLIC_ROSTER");
        FoundationRunFrame frame = envelope.Game ?? throw new InvalidDataException(message: "PUBLIC_PART");
        if (roster.Revision < _revision || roster.Revision == _revision && _parts is null)
        {
            return null;
        }

        if (roster.Revision > _revision)
        {
            Begin(roster: roster, frame: frame);
        }

        if (_parts is not { } parts || _roster is not { } current || parts.Length != frame.Parts
                || !StringComparer.Ordinal.Equals(x: _header.GetRawText(), y: frame.Body?.GetRawText())
                || !StringComparer.Ordinal.Equals(x: _rosterBytes, y: JsonSerializer.Serialize(value: roster))
                || !Add(parts: parts, frame: frame))
        {
            Discard(revision: roster.Revision);
            throw new InvalidDataException(message: "PUBLIC_PART_MISMATCH");
        }

        if (_received != parts.Length)
        {
            return null;
        }

        JsonElement world = Complete(header: _header, parts: parts);
        Discard(revision: roster.Revision);
        return (current, world);
    }

    internal void Discard(long revision)
    {
        if (revision >= _revision)
        {
            _revision = revision;
            _parts = null;
            _roster = null;
            _rosterBytes = "";
            _received = 0;
            _count = 0;
        }
    }

    internal void Clear()
    {
        Discard(revision: _revision);
        _revision = -1;
    }

    private const string Invalid = "PUBLIC_PART";
    private static readonly string[] _collections = ["Actors", "Corpses", "Defenses", "Loot", "Arrows"];
    private FoundationSnapshot? _roster;
    private string _rosterBytes = "";
    private JsonElement _header;
    private ImmutableArray<JsonElement>[]? _parts;
    private long _revision = -1;
    private int _received;
    private int _count;

    private void Begin(FoundationSnapshot roster, FoundationRunFrame frame)
    {
        _revision = roster.Revision;
        _roster = roster;
        _rosterBytes = JsonSerializer.Serialize(value: roster);
        _header = frame.Body ?? throw new InvalidDataException(message: Invalid);
        _parts = new ImmutableArray<JsonElement>[frame.Parts];
        _received = 0;
        _count = 0;
    }

    private bool Add(ImmutableArray<JsonElement>[] parts, FoundationRunFrame frame)
    {
        if (!parts[frame.Part].IsDefault)
        {
            return parts[frame.Part].Select(selector: row => row.GetRawText()).SequenceEqual(
                    second: frame.Rows.Select(selector: row => row.GetRawText()), comparer: StringComparer.Ordinal);
        }
        if (frame.Rows.Length > MaximumRows - _count)
        {
            return false;
        }

        parts[frame.Part] = frame.Rows;
        _count += frame.Rows.Length;
        _received++;
        return true;
    }

    private static JsonElement Header(JsonElement world)
    {
        return JsonSerializer.SerializeToElement(value: new
        {
            Elapsed = world.GetProperty(propertyName: "Elapsed"),
            OriginX = world.GetProperty(propertyName: "OriginX"),
            Ground = world.GetProperty(propertyName: "Ground"),
            CampLeft = world.GetProperty(propertyName: "CampLeft"),
            CampRight = world.GetProperty(propertyName: "CampRight"),
        });
    }

    private static JsonElement Complete(JsonElement header, ImmutableArray<JsonElement>[] parts)
    {
        Dictionary<string, List<JsonElement>> fields = _collections.ToDictionary(keySelector: name => name,
                elementSelector: _ => new List<JsonElement>(), comparer: StringComparer.Ordinal);
        foreach (JsonElement row in parts.SelectMany(selector: part => part))
        {
            string collection = row.GetProperty(propertyName: "Collection").GetString() ?? "";
            fields[collection].Add(item: row.GetProperty(propertyName: "Row"));
        }

        var world = new Dictionary<string, object>(comparer: StringComparer.Ordinal);
        foreach (JsonProperty property in header.EnumerateObject())
        {
            world.Add(key: property.Name, value: property.Value);
        }

        foreach ((string name, List<JsonElement> rows) in fields)
        {
            world.Add(key: name, value: rows);
        }

        JsonElement combined = JsonSerializer.SerializeToElement(value: world);
        FoundationProtocol.ValidateWorld(body: combined);
        return combined;
    }
}
