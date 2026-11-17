using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Composes detached authored presentation tracks using only caller-supplied elapsed seconds.</summary>
public sealed class CharacterPresentation
{
    /// <summary>Validates and detaches bounded animation, item and accepted combat configuration bytes.</summary>
    public CharacterPresentation(byte[] animation, byte[] items, byte[] combat)
    {
        _definition = Parse(bytes: animation, id: "locomotion-melee");
        JsonElement itemData = Parse(bytes: items, id: "DT-04");
        JsonElement combatData = Parse(bytes: combat, id: "DT-05");
        ValidateTracks();
        ValidateTiming();
        foreach (JsonElement attack in combatData.GetProperty(propertyName: "attacks").EnumerateArray())
        {
            string key = attack.GetProperty(propertyName: "itemRef").GetString() ?? "Empty";
            if (key is "Empty" && Text(row: attack, name: "attackId") is not "survivor.fists")
            {
                continue;
            }

            double interval = attack.GetProperty(propertyName: "intervalSeconds").GetDouble();
            if (interval is <= 0 or > 60 || !double.IsFinite(d: interval)
                    || !_intervals.TryAdd(key: key, value: interval))
            {
                throw new InvalidDataException(message: "ANIMATION_INTERVAL " + key);
            }

            foreach (string sound in new[] { "releaseSound", "impactSound" })
            {
                string value = Text(row: attack, name: sound);
                if (value is not ("AU-03" or "AU-04" or "AU-06" or "AU-07" or "AU-08"))
                {
                    throw new InvalidDataException(message: "ANIMATION_SOUND " + key);
                }
            }

            _attacks.Add(key: key, value: attack);
        }

        string[] required = ["IT-03", "IT-04", "IT-05", "IT-06"];
        foreach (string id in required)
        {
            if (!_intervals.ContainsKey(key: id) || !itemData.GetProperty(propertyName: "definitions")
                    .EnumerateArray().Any(predicate: item => StringComparer.Ordinal.Equals(
                            x: Text(row: item, name: "id"), y: id)))
            {
                throw new InvalidDataException(message: "ANIMATION_ITEM " + id);
            }
        }

        if (!_intervals.ContainsKey(key: "Empty") || Text(row: combatData.GetProperty(propertyName: "handRules"),
                name: "dualInterval") is not "maximum-of-both")
        {
            throw new InvalidDataException(message: "ANIMATION_HAND_RULE");
        }
    }

    /// <summary>Returns a detached sample with logical hand IDs independent of facing and draw order.</summary>
    public JsonElement Snapshot
    {
        get
        {
            JsonElement lower = Sample(clip: _movement, time: _stride);
            JsonElement upper = Sample(clip: "melee", time: _actionTime / Interval());
            double flinch = _flinch > 0 ? -1 : 0;
            return JsonSerializer.SerializeToElement(value: new
            {
                body = _body,
                movement = _movement,
                action = _action,
                leftItem = _left,
                rightItem = _right,
                facing = _facing,
                generation = _generation,
                strideSeconds = _stride,
                actionSeconds = _actionTime,
                intervalSeconds = Interval(),
                lowerPose = Text(row: lower, name: "pose"),
                upperPose = _action is "melee" ? Text(row: upper, name: "pose") : Text(row: lower, name: "pose"),
                stage = Text(row: lower, name: "stage"),
                lowerOffset = lower.GetProperty(propertyName: "offset"),
                upperOffset = _action is "melee" ? upper.GetProperty(propertyName: "offset")
                        : JsonSerializer.SerializeToElement(value: _zeroOffset),
                leftAngle = Angle(item: _left, sample: upper),
                rightAngle = Angle(item: _right, sample: upper),
                flinchOffset = flinch,
                transitionOffset = _transition > 0 ? 1 : 0,
                contactReady = _action is "melee" && _actionTime / Interval() >= ContactFraction
                        && !_contactAccepted,
                contactVisible = _contact > 0,
                releaseSound = Text(row: _attacks[AttackKey()], name: "releaseSound"),
                impactSound = Text(row: _attacks[AttackKey()], name: "impactSound"),
                gameplayEvents = 0,
                nextSampleSeconds = Math.Min(val1: Remaining(clip: _movement, time: _stride),
                        val2: _action is "melee" ? Remaining(clip: "melee", time: _actionTime / Interval())
                                * Interval() : 60),
            });
        }
    }

    /// <summary>Applies one current selection; facing changes preserve current track phases.</summary>
    public void Configure(string body, string movement, string action, string leftItem, string rightItem, int facing)
    {
        if (!_definition.GetProperty(propertyName: "bodies").EnumerateArray().Any(predicate: value =>
                StringComparer.Ordinal.Equals(x: value.GetString(), y: body))
                || movement is not ("idle" or "walk" or "faster" or "jump")
                || action is not ("none" or "melee") || facing is not (-1 or 1)
                || movement is "jump" && body is not "CH-01" || !Hand(item: leftItem) || !Hand(item: rightItem))
        {
            throw new InvalidDataException(message: "ANIMATION_SELECTION");
        }

        bool replacement = !StringComparer.Ordinal.Equals(x: body, y: _body)
                || !StringComparer.Ordinal.Equals(x: movement, y: _movement)
                || !StringComparer.Ordinal.Equals(x: action, y: _action)
                || !StringComparer.Ordinal.Equals(x: leftItem, y: _left)
                || !StringComparer.Ordinal.Equals(x: rightItem, y: _right);
        if (!StringComparer.Ordinal.Equals(x: movement, y: _movement))
        {
            _stride = 0;
            _transition = Timing(name: "weightShiftSeconds");
        }

        if (replacement)
        {
            _generation = checked(_generation + 1);
            _actionTime = 0;
            _contact = 0;
            _contactAccepted = false;
        }

        _body = body;
        _movement = movement;
        _action = action;
        _left = leftItem;
        _right = rightItem;
        _facing = facing;
    }

    /// <summary>Advances independent layers without movement, damage or hidden clocks.</summary>
    public void Advance(double elapsedSeconds)
    {
        if (!double.IsFinite(d: elapsedSeconds) || elapsedSeconds is < 0 or > 60)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(elapsedSeconds));
        }

        double length = Length(clip: _movement);
        _stride = _movement is "jump" ? Math.Min(val1: length - 0.0000001, val2: _stride + elapsedSeconds)
                : (_stride + elapsedSeconds) % length;
        _flinch = Math.Max(val1: 0, val2: _flinch - elapsedSeconds);
        _transition = Math.Max(val1: 0, val2: _transition - elapsedSeconds);
        _contact = Math.Max(val1: 0, val2: _contact - elapsedSeconds);
        if (_action is "melee")
        {
            double next = _actionTime + elapsedSeconds;
            if (next >= Interval())
            {
                _generation = checked(_generation + (long)Math.Floor(d: next / Interval()));
                _contactAccepted = false;
            }

            _actionTime = next % Interval();
        }
    }

    /// <summary>Starts a small visual-only overlay without modifying either track's time or generation.</summary>
    public void Flinch()
    {
        _flinch = Timing(name: "flinchSeconds");
    }

    /// <summary>Accepts one current-generation preview contact; never creates a gameplay attack or hit.</summary>
    public bool TryContact(long generation)
    {
        if (generation != _generation || _action is not "melee" || _contactAccepted
                || _actionTime / Interval() < ContactFraction)
        {
            return false;
        }

        _contactAccepted = true;
        _contact = Timing(name: "contactSeconds");
        return true;
    }

    private static readonly int[] _zeroOffset = [0, 0];
    private readonly JsonElement _definition;
    private readonly Dictionary<string, double> _intervals = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, JsonElement> _attacks = new(comparer: StringComparer.Ordinal);
    private string _body = "CH-01";
    private string _movement = "idle";
    private string _action = "none";
    private string _left = "Empty";
    private string _right = "Empty";
    private int _facing = 1;
    private long _generation;
    private double _stride;
    private double _actionTime;
    private double _flinch;
    private double _transition;
    private double _contact;
    private bool _contactAccepted;

    private double ContactFraction => Timing(name: "contactFraction");

    private static JsonElement Parse(byte[] bytes, string id)
    {
        if (bytes.Length is 0 or > 1048576 || bytes[0] == 239)
        {
            throw new InvalidDataException(message: "ANIMATION_DOCUMENT_SIZE");
        }

        string text = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
                .GetString(bytes: bytes);
        using var document = JsonDocument.Parse(json: text, options: new JsonDocumentOptions { MaxDepth = 24 });
        int count = 0;
        ValidateValue(value: document.RootElement, count: ref count);
        JsonElement root = document.RootElement;
        if (Text(row: root, name: "id") is not { } actual || !StringComparer.Ordinal.Equals(x: actual, y: id)
                || root.GetProperty(propertyName: "version").GetInt32() != 1)
        {
            throw new InvalidDataException(message: "ANIMATION_DOCUMENT_ID");
        }

        return root.Clone();
    }

    private static void ValidateValue(JsonElement value, ref int count)
    {
        if (++count > 100000)
        {
            throw new InvalidDataException(message: "ANIMATION_DOCUMENT_COUNT");
        }

        if (value.ValueKind is JsonValueKind.Object)
        {
            var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                if (!keys.Add(item: property.Name) || property.Name is "__proto__" or "constructor" or "prototype")
                {
                    throw new InvalidDataException(message: "ANIMATION_DUPLICATE_KEY");
                }

                ValidateValue(value: property.Value, count: ref count);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            foreach (JsonElement item in value.EnumerateArray())
            {
                ValidateValue(value: item, count: ref count);
            }
        }
        else if (value.ValueKind is JsonValueKind.Number && !double.IsFinite(d: value.GetDouble())
                || value.ValueKind is JsonValueKind.String && value.GetString() is { } text
                        && (text.Length > 4096 || !text.IsNormalized()))
        {
            throw new InvalidDataException(message: "ANIMATION_VALUE");
        }
    }

    private void ValidateTracks()
    {
        string[] bodies = ["CH-01", "CH-02", "CH-03", "CH-04", "CH-05", "CH-06", "CH-07", "CH-08"];
        if (!_definition.GetProperty(propertyName: "bodies").EnumerateArray().Select(selector: value =>
                value.GetString()).SequenceEqual(second: bodies, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "ANIMATION_BODIES");
        }

        string[] clips = ["idle", "walk", "faster", "jump", "melee"];
        if (_definition.GetProperty(propertyName: "schemaVersion").GetInt32() != 1
                || !_definition.GetProperty(propertyName: "clips").EnumerateObject().Select(
                        selector: value => value.Name).SequenceEqual(second: clips, comparer: StringComparer.Ordinal))
        {
            throw new InvalidDataException(message: "ANIMATION_CLIPS");
        }

        foreach (string clip in clips)
        {
            JsonElement samples = _definition.GetProperty(propertyName: "clips").GetProperty(propertyName: clip);
            if (samples.GetArrayLength() is < 1 or > 16)
            {
                throw new InvalidDataException(message: "ANIMATION_SAMPLES");
            }

            foreach (JsonElement sample in samples.EnumerateArray())
            {
                double duration = sample.GetProperty(propertyName: "duration").GetDouble();
                JsonElement offset = sample.GetProperty(propertyName: "offset");
                if (duration is <= 0 or > 5 || Text(row: sample, name: "pose") is not ("idle" or "walk-near"
                        or "walk-far" or "run" or "windup" or "strike" or "recover")
                        || Text(row: sample, name: "stage").Length is 0 or > 32
                        || offset.GetArrayLength() != 2 || offset.EnumerateArray().Any(predicate: number =>
                                number.GetInt32() is < -8 or > 8))
                {
                    throw new InvalidDataException(message: "ANIMATION_SAMPLE");
                }

                if (clip is "melee")
                {
                    ValidateAngles(sample: sample);
                }
            }
        }
    }

    private static void ValidateAngles(JsonElement sample)
    {
        string[] profiles = ["fists", "knife", "club", "machete", "bat"];
        JsonElement angles = sample.GetProperty(propertyName: "angles");
        if (!angles.EnumerateObject().Select(selector: value => value.Name).SequenceEqual(second: profiles,
                comparer: StringComparer.Ordinal) || angles.EnumerateObject().Any(predicate: value =>
                        Math.Abs(value: value.Value.GetDouble()) > Math.PI))
        {
            throw new InvalidDataException(message: "ANIMATION_ANGLES");
        }
    }

    private void ValidateTiming()
    {
        foreach (string key in new[] { "contactFraction", "flinchSeconds", "weightShiftSeconds", "contactSeconds" })
        {
            if (Timing(name: key) is <= 0 or > 0.5)
            {
                throw new InvalidDataException(message: "ANIMATION_TIMING " + key);
            }
        }

        JsonElement timing = _definition.GetProperty(propertyName: "timing");
        if (Text(row: timing, name: "clock") is not "explicit-elapsed-only"
                || timing.GetProperty(propertyName: "gameplayEvents").GetInt32() != 0
                || Math.Abs(value: Length(clip: "melee") - 1) > 0.00000001
                || Math.Abs(value: _definition.GetProperty(propertyName: "clips").GetProperty(
                        propertyName: "melee")[0].GetProperty(propertyName: "duration").GetDouble()
                        - ContactFraction) > 0.00000001)
        {
            throw new InvalidDataException(message: "ANIMATION_TIMING_BINDING");
        }
    }

    private double Timing(string name)
    {
        return _definition.GetProperty(propertyName: "timing").GetProperty(propertyName: name).GetDouble();
    }

    private string AttackKey()
    {
        return _intervals.TryGetValue(key: _left, value: out double left) && left == Interval() ? _left
                : _intervals.TryGetValue(key: _right, value: out double right) && right == Interval()
                        ? _right : "Empty";
    }

    private static bool Hand(string item)
    {
        return item is "Empty" or "IT-02" or "IT-03" or "IT-04" or "IT-05" or "IT-06" or "IT-07"
                or "IT-08" or "IT-09" or "IT-10";
    }

    private double Interval()
    {
        double left = _left is "IT-03" or "IT-04" or "IT-05" or "IT-06" ? _intervals[_left] : 0;
        double right = _right is "IT-03" or "IT-04" or "IT-05" or "IT-06" ? _intervals[_right] : 0;
        return left + right > 0 ? Math.Max(val1: left, val2: right) : _intervals["Empty"];
    }

    private double Angle(string item, JsonElement sample)
    {
        string profile = item is "IT-03" ? "knife" : item is "IT-04" ? "club"
                : item is "IT-05" ? "machete" : item is "IT-06" ? "bat" : "fists";
        return _action is "melee" ? sample.GetProperty(propertyName: "angles").GetProperty(propertyName: profile)
                .GetDouble() : 0;
    }

    private double Length(string clip)
    {
        return _definition.GetProperty(propertyName: "clips").GetProperty(propertyName: clip).EnumerateArray()
                .Sum(selector: value => value.GetProperty(propertyName: "duration").GetDouble());
    }

    private JsonElement Sample(string clip, double time)
    {
        JsonElement samples = _definition.GetProperty(propertyName: "clips").GetProperty(propertyName: clip);
        foreach (JsonElement sample in samples.EnumerateArray())
        {
            time -= sample.GetProperty(propertyName: "duration").GetDouble();
            if (time < 0)
            {
                return sample;
            }
        }

        return samples[samples.GetArrayLength() - 1];
    }

    private static string Text(JsonElement row, string name)
    {
        return row.GetProperty(propertyName: name).GetString()
                ?? throw new InvalidDataException(message: "ANIMATION_TEXT " + name);
    }

    private double Remaining(string clip, double time)
    {
        foreach (JsonElement sample in _definition.GetProperty(propertyName: "clips")
                .GetProperty(propertyName: clip).EnumerateArray())
        {
            double duration = sample.GetProperty(propertyName: "duration").GetDouble();
            if (time < duration - 0.00000001)
            {
                return duration - time;
            }

            time -= duration;
        }

        return 0.0000001;
    }
}
