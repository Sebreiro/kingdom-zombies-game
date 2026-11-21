using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Separates persistent current appearance from explicitly triggered transient effects.</summary>
public sealed class AppearanceState
{
    /// <summary>Validates and detaches the bounded framework-only appearance definition.</summary>
    public AppearanceState(byte[] definition)
    {
        ArgumentNullException.ThrowIfNull(argument: definition);
        if (definition.Length is 0 or > 1048576 || definition[0] == 239)
        {
            throw new InvalidDataException(message: "APPEARANCE_DEFINITION_SIZE");
        }

        _ = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: definition);
        using var document = JsonDocument.Parse(utf8Json: definition,
                options: new JsonDocumentOptions { MaxDepth = 24 });
        ValidateValue(value: document.RootElement);
        _definition = document.RootElement.Clone();
        ValidateDefinition();
    }

    /// <summary>Returns a detached current-state publication with no native or gameplay references.</summary>
    public JsonElement Snapshot => JsonSerializer.SerializeToElement(value: new
    {
        body = _body,
        kind = _kind,
        selected = _selected,
        equipment = _equipment,
        generation = _generation,
        eventCount = _events,
        transientVisible = _deathElapsed >= 0 && _deathElapsed < _effectSeconds,
        persistent = _kind is "corpse" or "wildlife" || _kind is "pile" && _equipment.Count > 0,
        selectable = _kind is "alive" or "corpse" or "wildlife-alive" or "wildlife"
                || _kind is "pile" && _equipment.Count > 0,
        pose = _deathElapsed >= 0 && _deathElapsed < _collapseSeconds
                ? _deathElapsed < _collapseSeconds / 2 ? "collapse-a" : "collapse-b"
                : _kind is "corpse" or "wildlife" ? "corpse" : "idle",
        crownLoot = false,
        currentLoops = 0,
        gameplayEvents = 0,
    });

    /// <summary>Installs validated current gear atomically and silently without replaying past events.</summary>
    public void Restore(string body, string kind, JsonElement equipment, bool selected)
    {
        bool humanoid = _bodies.Contains(value: body, comparer: StringComparer.Ordinal);
        bool wildlife = body is "rabbit" or "deer";
        if (!(humanoid && kind is ("alive" or "corpse") || wildlife && kind is ("wildlife-alive" or "wildlife")
                || body is "pile" && kind is "pile" || body is "guard" && kind is "structural")
                || equipment.ValueKind is not JsonValueKind.Object || equipment.GetRawText().Length > 4096)
        {
            throw new InvalidDataException(message: "APPEARANCE_SELECTION");
        }

        var candidate = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach (JsonProperty entry in equipment.EnumerateObject())
        {
            string item = entry.Value.ValueKind is JsonValueKind.String ? entry.Value.GetString() ?? "" : "";
            if (candidate.Count >= 16 || !candidate.TryAdd(key: entry.Name, value: item)
                    || !ValidItem(slot: entry.Name, item: item) || kind is "structural"
                    || wildlife && entry.Name is not "meat" || !wildlife && entry.Name is "meat" && body is not "pile")
            {
                throw new InvalidDataException(message: "APPEARANCE_EQUIPMENT " + entry.Name);
            }
        }

        if (kind is "corpse" or "pile")
        {
            _ = candidate.Remove(key: "crown");
        }

        _body = body;
        _kind = kind;
        _equipment = candidate;
        _selected = selected && kind is not "structural";
        _deathElapsed = -1;
        _generation = checked(_generation + 1);
    }

    /// <summary>Triggers one current death transition; duplicate or structural triggers create no loot.</summary>
    public void TriggerDeath()
    {
        if (_kind is "structural")
        {
            Reset();
        }
        else if (_kind is "alive" or "wildlife-alive")
        {
            _kind = _kind is "alive" ? "corpse" : "wildlife";
            _ = _equipment.Remove(key: "crown");
            _deathElapsed = 0;
            _events = checked(_events + 1);
            _generation = checked(_generation + 1);
        }
    }

    /// <summary>Advances only finite bounded caller time; stable gear never expires with an effect.</summary>
    public void Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(d: elapsedSeconds) || elapsedSeconds is < 0 or > 60)
        {
            throw new InvalidDataException(message: "APPEARANCE_CLOCK");
        }

        if (_deathElapsed >= 0)
        {
            _deathElapsed = Math.Min(val1: _effectSeconds, val2: _deathElapsed + elapsedSeconds);
        }
    }

    /// <summary>Closes the current generation and clears effects, gear, selection and event counters.</summary>
    public void Reset()
    {
        _body = "";
        _kind = "empty";
        _selected = false;
        _equipment = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        _deathElapsed = -1;
        _events = 0;
        _generation = checked(_generation + 1);
    }

    private static readonly string[] _bodies = ["CH-01", "CH-02", "CH-03", "CH-04", "CH-05", "CH-06",
        "CH-07", "CH-08"];
    private static readonly string[] _wildlife = ["rabbit", "deer"];
    private readonly JsonElement _definition;
    private Dictionary<string, string> _equipment = new(comparer: StringComparer.Ordinal);
    private string _body = "";
    private string _kind = "empty";
    private bool _selected;
    private double _deathElapsed = -1;
    private double _collapseSeconds;
    private double _effectSeconds;
    private long _events;
    private long _generation;

    private void ValidateDefinition()
    {
        if (_definition.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || _definition.GetProperty(propertyName: "version").GetInt32() != 1
                || _definition.GetProperty(propertyName: "id").GetString() is not "corpses-wildlife"
                || !_definition.GetProperty(propertyName: "bodies").EnumerateArray().Select(
                        selector: value => value.GetString()).SequenceEqual(second: _bodies,
                                comparer: StringComparer.Ordinal)
                || !_definition.GetProperty(propertyName: "wildlife").EnumerateArray().Select(
                        selector: value => value.GetString()).SequenceEqual(second: _wildlife,
                                comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "APPEARANCE_DEFINITION_ID");
        }

        _collapseSeconds = _definition.GetProperty(propertyName: "collapseSeconds").GetDouble();
        _effectSeconds = _definition.GetProperty(propertyName: "effectSeconds").GetDouble();
        if (_collapseSeconds is <= 0 or > 2 || _effectSeconds < _collapseSeconds || _effectSeconds > 10)
        {
            throw new InvalidDataException(message: "APPEARANCE_TIMING");
        }

        foreach (JsonProperty slot in _definition.GetProperty(propertyName: "slots").EnumerateObject())
        {
            if (slot.Value.ValueKind is not JsonValueKind.Array || slot.Value.GetArrayLength() is 0 or > 12
                    || slot.Value.EnumerateArray().Any(predicate: value => !CanonicalItem(slot: slot.Name,
                            item: value.GetString() ?? "")) || slot.Value.EnumerateArray().Select(
                                    selector: value => value.GetString()).Distinct(comparer: StringComparer.Ordinal)
                                    .Count() != slot.Value.GetArrayLength())
            {
                throw new InvalidDataException(message: "APPEARANCE_SLOT");
            }
        }
    }

    private bool ValidItem(string slot, string item)
    {
        return CanonicalItem(slot: slot, item: item) && (_definition.GetProperty(propertyName: "slots")
                .TryGetProperty(propertyName: slot, value: out JsonElement values)
                && values.EnumerateArray().Any(predicate: value => StringComparer.Ordinal.Equals(
                        x: value.GetString(), y: item)));
    }

    private static bool CanonicalItem(string slot, string item)
    {
        return slot switch
        {
            "headwear" => item is "IT-14" or "IT-18",
            "face" => item is "IT-15",
            "torso" => item is "IT-11" or "IT-13",
            "legs" => item is "IT-12",
            "armor" => item is "IT-19" or "IT-20",
            "gloves" => item is "IT-16",
            "boots" => item is "IT-17",
            "backpack" => item is "IT-23" or "IT-24",
            "belt" => item is "IT-21",
            "left" or "right" => item is "IT-02" or "IT-03" or "IT-04" or "IT-05" or "IT-06"
                    or "IT-07" or "IT-08" or "IT-09" or "IT-10",
            "crown" => item is "K21",
            "meat" => item is "IT-10",
            _ => false,
        };
    }

    private static void ValidateValue(JsonElement value)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (names.Count >= 128 || !names.Add(item: property.Name))
                {
                    throw new InvalidDataException(message: "APPEARANCE_DUPLICATE_OR_COUNT");
                }

                ValidateValue(value: property.Value);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            if (value.GetArrayLength() > 512)
            {
                throw new InvalidDataException(message: "APPEARANCE_ARRAY");
            }

            foreach (JsonElement element in value.EnumerateArray())
            {
                ValidateValue(value: element);
            }
        }
        else if (value.ValueKind is JsonValueKind.Number && !double.IsFinite(d: value.GetDouble())
                || value.ValueKind is JsonValueKind.String && (value.GetString() ?? "").Length > 4096)
        {
            throw new InvalidDataException(message: "APPEARANCE_VALUE");
        }
    }
}
