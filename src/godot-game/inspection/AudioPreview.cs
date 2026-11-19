using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using Godot;
using KingdomZombies.Proofs;

namespace KingdomZombies.Inspection;

/// <summary>Owns explicit audio auditions and the current state's native loop set.</summary>
internal sealed class AudioPreview : IDisposable
{
    /// <summary>Closes acceptance and releases the native players owned by this preview.</summary>
    public void Dispose()
    {
        Close();
    }

    internal AudioPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string category)
    {
        _root = root;
        _gateway = gateway;
        _gateway.RequireThread();
        _epoch = NextEpoch();
        Load(read: read, category: category);
        Build();
    }

    internal long Epoch => _epoch;

    internal string Status => !_visible.Any(predicate: id => _routes.ContainsKey(key: id))
            ? "Unavailable — audio data; choose Recheck resources"
            : "Partial — audio audition ready; listening/mix acceptance pending";

    internal static long NextEpoch()
    {
        return Interlocked.Increment(location: ref _nextEpoch);
    }

    internal static bool Accepts(long completion, long current, bool accepting)
    {
        return accepting && completion == current;
    }

    internal static bool IsMuted(bool muted, double master, double channel)
    {
        return muted || master == 0 || channel == 0;
    }

    internal static int VariantIndex(int count, int previous, uint draw)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value: count);
        if (count == 1)
        {
            return 0;
        }

        int selected = (int)(draw % (uint)(previous >= 0 ? count - 1 : count));
        return previous >= 0 && selected >= previous ? selected + 1 : selected;
    }

    internal static string[] Ids(string category)
    {
        return category is "Audio" or "" ? [.. Enumerable.Range(start: 1, count: 30).Select(selector: number =>
                "AU-" + number.ToString(format: "00", provider: CultureInfo.InvariantCulture))] : [];
    }

    internal static bool Handles(string id)
    {
        return Ids(category: "Audio").Contains(value: id, comparer: StringComparer.Ordinal);
    }

    internal static AudioPreview Open(InspectionShell? shell, MainThreadGateway gateway,
            Func<string, byte[]?> read, string category)
    {
        gateway.RequireThread();
        var root = new Node2D { Name = "AudioPreview" };
        var preview = new AudioPreview(root: root, gateway: gateway, read: read, category: category);
        shell?.Install(preview: root);
        return preview;
    }

    internal static byte[] ExtractPcm(byte[] bytes, JsonElement bank, JsonElement sample)
    {
        Require(valid: bytes.Length is >= 44 and <= 16_777_216, file: Text(value: bank, key: "path"),
                field: "bytes", value: bytes.Length.ToString(provider: CultureInfo.InvariantCulture),
                reason: "PCM bank must fit16MiB");
        string file = Text(value: bank, key: "path");
        Require(valid: Same(left: Hash(bytes: bytes), right: Text(value: bank, key: "sha256")),
                file: file, field: "sha256",
                value: Text(value: bank, key: "sha256"), reason: "bank hash mismatch");
        ReadOnlySpan<byte> span = bytes;
        Require(valid: span[..4].SequenceEqual(other: "RIFF"u8) && span.Slice(start: 8, length: 8)
                .SequenceEqual(other: "WAVEfmt "u8) && span.Slice(start: 36, length: 4).SequenceEqual(other: "data"u8)
                && BinaryPrimitives.ReadInt32LittleEndian(source: span.Slice(start: 4, length: 4)) == bytes.Length - 8
                && BinaryPrimitives.ReadInt32LittleEndian(source: span.Slice(start: 16, length: 4)) == 16
                && BinaryPrimitives.ReadInt16LittleEndian(source: span.Slice(start: 20, length: 2)) == 1
                && BinaryPrimitives.ReadInt16LittleEndian(source: span.Slice(start: 22, length: 2)) == 2
                && BinaryPrimitives.ReadInt32LittleEndian(source: span.Slice(start: 24, length: 4)) == 44100
                && BinaryPrimitives.ReadInt32LittleEndian(source: span.Slice(start: 28, length: 4)) == 176400
                && BinaryPrimitives.ReadInt16LittleEndian(source: span.Slice(start: 32, length: 2)) == 4
                && BinaryPrimitives.ReadInt16LittleEndian(source: span.Slice(start: 34, length: 2)) == 16
                && BinaryPrimitives.ReadInt32LittleEndian(source: span.Slice(start: 40, length: 4))
                        == bytes.Length - 44,
                file: file, field: "format", value: "WAV", reason: "expected canonical44100Hz stereoPCM16");
        int first = sample.GetProperty(propertyName: "regionFrames")[0].GetInt32();
        int last = sample.GetProperty(propertyName: "regionFrames")[1].GetInt32();
        Require(valid: Number(value: bank, key: "frames") == (bytes.Length - 44) / 4
                && Number(value: bank, key: "channels") == 2 && Number(value: bank, key: "rate") == 44100
                && Number(value: bank, key: "sampleWidth") == 2 && first >= 0 && last > first
                && last <= (bytes.Length - 44) / 4 && last - first == Number(value: sample, key: "frames")
                && Number(value: sample, key: "channels") == 2 && Number(value: sample, key: "rate") == 44100
                && Number(value: sample, key: "sampleWidth") == 2, file: file, field: "regionFrames",
                value: first.ToString(provider: CultureInfo.InvariantCulture) + ":" + last.ToString(
                        provider: CultureInfo.InvariantCulture), reason: "invalid integer PCM region or format");
        byte[] pcm = span.Slice(start: 44 + (first * 4), length: (last - first) * 4).ToArray();
        Require(valid: Same(left: Hash(bytes: pcm), right: Text(value: sample, key: "pcmSha256")),
                file: file, field: "pcmSha256",
                value: Text(value: sample, key: "pcmSha256"), reason: "sample PCM hash mismatch");
        return pcm;
    }

    internal static string ResolveSource(JsonElement pack, JsonElement sample, JsonElement actions)
    {
        string id = SourceText(owner: sample, field: "id", id: "sample");
        if (sample.TryGetProperty(propertyName: "source", value: out JsonElement original))
        {
            ValidateOriginal(source: original, id: id);
            return original.GetRawText();
        }

        string reference = SourceText(owner: sample, field: "sourceId", id: id);
        return reference switch
        {
            "generated-food-crunch" => GeneratedSource(pack: pack, sample: sample, id: id),
            "action.2790.full" => SharedSource(pack: pack, sample: sample, actions: actions, id: id),
            _ => DonorSource(pack: pack, sample: sample, id: id, reference: reference),
        };
    }

    internal void Tick(double delta)
    {
        _gateway.RequireThread();
        if (_closed)
        {
            return;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(value: delta);
        _time += double.IsFinite(d: delta) ? delta : 0;
        var completed = new List<long>();
        foreach ((long id, var player) in _players)
        {
            if (!player.Player.Playing)
            {
                completed.Add(item: id);
            }
        }
        foreach (long id in completed)
        {
            Remove(id: id);
        }

        Refresh();
    }

    internal void Close()
    {
        _gateway.RequireThread();
        _closed = true;
        _epoch = NextEpoch();
        StopAll();
        _routes.Clear();
        _samples.Clear();
        _banks.Clear();
        if (_panel is not null)
        {
            Node scroll = _panel.GetParent();
            _root.RemoveChild(node: scroll);
            scroll.Free();
        }

        _panel = null;
        _selector = null;
        _status = null;
        _details = null;
        _loop = null;
        _mute = null;
    }

    internal bool Trigger(string routeId, string eventId, long epoch, bool loop, bool restoring)
    {
        _gateway.RequireThread();
        if (!Accepts(completion: epoch, current: _epoch, accepting: !_closed)
                || !_routes.TryGetValue(key: routeId, value: out JsonElement route))
        {
            return false;
        }

        if (restoring && Text(value: route.GetProperty(propertyName: "lifecycle"), key: "mode")
                is not "current-state-loop")
        {
            return false;
        }

        if (!restoring && (_committed.Contains(item: eventId) || _last.TryGetValue(key: routeId,
                value: out double last) && (_time - last) * 1000 < Number(value: route, key: "cooldownMilliseconds")))
        {
            return false;
        }

        if (!Admit(routeId: routeId, route: route))
        {
            return false;
        }

        string[] variants = [.. route.GetProperty(propertyName: "variants").EnumerateArray()
                .Select(selector: value => value.GetString() ?? "")];
        int previous = _variants.GetValueOrDefault(key: routeId, defaultValue: -1);
        int choices = previous >= 0 && variants.Length > 1 ? variants.Length - 1 : variants.Length;
        int next = VariantIndex(count: variants.Length, previous: previous,
                draw: DrawVariant(choices: choices));
        string sample = variants[next];
        _variants[routeId] = next;
        Start(routeId: routeId, sampleId: sample, route: route, loop: loop, restoring: restoring);
        if (!restoring)
        {
            _last[routeId] = _time;
            _committed.Add(item: eventId);
            _oneShots++;
        }

        Refresh();
        return true;
    }

    internal void Restore(string[] routes)
    {
        _gateway.RequireThread();
        if (_closed)
        {
            return;
        }

        string[] wanted = [.. routes.Distinct(comparer: StringComparer.Ordinal).Where(predicate: id =>
                _routes.TryGetValue(key: id, value: out JsonElement route) && Text(value: route.GetProperty(
                        propertyName: "lifecycle"), key: "mode") is "current-state-loop")];
        foreach (long id in _players.Where(predicate: pair => !pair.Value.State
                || !wanted.Contains(value: pair.Value.Route, comparer: StringComparer.Ordinal))
                .Select(selector: pair => pair.Key).ToArray())
        {
            Remove(id: id);
        }

        foreach (string id in wanted)
        {
            if (!_players.Values.Any(predicate: value => value.State && Same(left: value.Route, right: id)))
            {
                Trigger(routeId: id, eventId: "", epoch: _epoch, loop: true, restoring: true);
            }
        }

        Refresh();
    }

    private const string Fixture = "content/fixtures/audio-states.json";
    private const string Events = "content/audio/events.json";
    private const string SourceFile = "content/packs/audio-interface-ambience.json";
    private static long _nextEpoch;
    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly Dictionary<string, JsonElement> _routes = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, (JsonElement Sample, JsonElement Bank, string Source)> _samples = new(
            comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, byte[]> _banks = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<long, (AudioStreamPlayer Player, AudioStreamWav Stream, string Route,
        string Sample, int Priority, bool State)> _players = [];
    private readonly Dictionary<string, double> _last = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, int> _variants = new(comparer: StringComparer.Ordinal);
    private readonly HashSet<string> _committed = new(comparer: StringComparer.Ordinal);
    private readonly List<string> _errors = [];
    private readonly List<string> _visible = [];
    private JsonElement _definition;
    private JsonElement _catalog;
    private VBoxContainer? _panel;
    private OptionButton? _selector;
    private Label? _status;
    private TextEdit? _details;
    private CheckButton? _loop;
    private CheckButton? _mute;
    private long _epoch;
    private long _serial;
    private int _oneShots;
    private double _time;
    private double _master = 80;
    private double _effects = 80;
    private double _ambience = 60;
    private bool _closed;
    private uint _variantState = 290029;

    private static string Text(JsonElement value, string key)
    {
        return value.GetProperty(propertyName: key).GetString() ?? throw new InvalidDataException(message: key);
    }

    private static bool Same(string left, string right)
    {
        return StringComparer.Ordinal.Equals(x: left, y: right);
    }

    private static JsonElement SourceNode(JsonElement owner, string field, string id)
    {
        Require(valid: owner.ValueKind is JsonValueKind.Object && owner.TryGetProperty(
                propertyName: field, value: out _), file: SourceFile, field: field, value: id,
                reason: "required source authority field missing");
        return owner.GetProperty(propertyName: field);
    }

    private static string SourceText(JsonElement owner, string field, string id)
    {
        JsonElement value = SourceNode(owner: owner, field: field, id: id);
        Require(valid: value.ValueKind is JsonValueKind.String && value.GetString()?.Length is > 0 and <= 4096,
                file: SourceFile, field: field, value: id, reason: "invalid source authority text");
        return value.GetString() ?? throw new InvalidDataException(message: field);
    }

    private static JsonElement SourceMatch(JsonElement owner, string field, string id)
    {
        JsonElement array = SourceNode(owner: owner, field: field, id: id);
        Require(valid: array.ValueKind is JsonValueKind.Array && array.GetArrayLength() <= 512,
                file: SourceFile, field: field, value: id, reason: "invalid bounded source authority array");
        JsonElement[] matches = [.. array.EnumerateArray().Where(predicate: value => Same(
                left: SourceText(owner: value, field: "id", id: id), right: id))];
        Require(valid: matches.Length == 1, file: SourceFile, field: field, value: id,
                reason: "source reference must match exactly one authority");
        return matches[0];
    }

    private static void ValidateOriginal(JsonElement source, string id)
    {
        string owner = SourceText(owner: source, field: "owner", id: id);
        string digest = SourceText(owner: source, field: "rawObjectSha256", id: id);
        Require(valid: owner is "resources.assets" or "sharedassets0.assets" or "sharedassets2.assets"
                && Number(value: source, key: "objectId") > 0 && digest.Length == 64
                && digest.All(predicate: value => value is >= '0' and <= '9' or >= 'a' and <= 'f'),
                file: SourceFile, field: "source", value: id, reason: "invalid original source identity");
    }

    private static string DonorSource(JsonElement pack, JsonElement sample, string id, string reference)
    {
        JsonElement record = SourceMatch(owner: pack, field: "sources", id: reference);
        Require(valid: Same(left: id, right: "interface." + reference + ".full")
                || Same(left: id, right: "interface." + reference + ".loop")
                || id is "interface.handling.v1" && reference is "r3755"
                || id is "interface.s0485.negative" && reference is "s0485", file: SourceFile,
                field: "sourceId", value: id, reason: "donor reference disagrees with variant identity");
        ValidateOriginal(source: SourceNode(owner: record, field: "source", id: id), id: id);
        JsonElement output = SourceNode(owner: record, field: "output", id: id);
        Require(valid: Number(value: output, key: "rate") == Number(value: sample, key: "rate")
                && Number(value: output, key: "sampleWidth") == Number(value: sample, key: "sampleWidth")
                && Number(value: output, key: "frames") >= sample.GetProperty(propertyName: "trimFrames")[1]
                        .GetInt32(), file: SourceFile, field: "output", value: id,
                reason: "donor format or trim disagrees with current variant");
        return record.GetRawText();
    }

    private static void DerivedSource(JsonElement sample, string id, string expected, int frames, double gain)
    {
        Require(valid: Same(left: id, right: expected) && Number(value: sample, key: "version") == 1
                && Number(value: sample, key: "frames") == frames
                && Number(value: sample, key: "rate") == 44100 && Number(value: sample, key: "channels") == 2
                && Number(value: sample, key: "sampleWidth") == 2
                && sample.GetProperty(propertyName: "trimFrames")[0].GetInt32() == 0
                && sample.GetProperty(propertyName: "trimFrames")[1].GetInt32() == frames
                && sample.GetProperty(propertyName: "gainLinear").GetDouble() == gain
                && Text(value: sample.GetProperty(propertyName: "listening"), key: "status") is "Not auditioned",
                file: SourceFile, field: "derivedSource", value: id,
                reason: "derived identity/trim/gain/format or listening scope mismatch");
    }

    private static string GeneratedSource(JsonElement pack, JsonElement sample, string id)
    {
        DerivedSource(sample: sample, id: id, expected: "interface.food-crunch.v1", frames: 11025, gain: 1);
        JsonElement gap = SourceNode(owner: pack, field: "donorGap", id: id);
        Require(valid: SourceText(owner: gap, field: "category", id: id) is "AU-19"
                && SourceText(owner: gap, field: "authority", id: id)
                        is "specs/0.1/assets-and-resources.md section10 AU-19"
                && SourceText(owner: gap, field: "synthesis", id: id)
                        is "Deterministic non-tonal contact/crunch texture seed190021; needs real audition",
                file: SourceFile, field: "donorGap", value: id, reason: "generated source authority mismatch");
        JsonElement recipes = SourceNode(owner: pack, field: "recipes", id: id);
        Require(valid: SourceText(owner: recipes, field: "assemblerMember", id: id)
                        is "recipes/assemble-interface.py"
                && SourceText(owner: recipes, field: "assemblerSha256", id: id)
                        is "4d1b537ffe933e13c11955e9437422bddc5532a85ae5596684940ff4e2868b37"
                && SourceText(owner: recipes, field: "decoderMember", id: id) is "recipes/decode-interface.py"
                && SourceText(owner: recipes, field: "decoderSha256", id: id)
                        is "df674987d2c9e0d45b8da0e5b55a9342ac4b8dbcc45221763d2754907e5df88b",
                file: SourceFile, field: "recipes", value: id, reason: "generated retained recipe identity mismatch");
        return JsonSerializer.Serialize(value: new { kind = "generated", sourceId = "generated-food-crunch",
            authority = gap, recipes, derivedListening = sample.GetProperty(propertyName: "listening") });
    }

    private static string SharedSource(JsonElement pack, JsonElement sample, JsonElement actions, string id)
    {
        DerivedSource(sample: sample, id: id, expected: "interface.bloater-burst.v1", frames: 13230, gain: 0.65);
        JsonElement shared = SourceMatch(owner: pack, field: "sharedSources", id: "action.2790.full");
        Require(valid: SourceText(owner: shared, field: "pack", id: id) is "packs/audio-actions.json"
                && SourceText(owner: actions, field: "id", id: id) is "audio-actions"
                && Number(value: actions, key: "version") == 1, file: SourceFile, field: "sharedSources.pack",
                value: id, reason: "shared source must bind the current canonical action pack");
        JsonElement current = SourceMatch(owner: actions, field: "samples", id: "action.2790.full");
        string digest = SourceText(owner: shared, field: "wavSha256", id: id);
        Require(valid: Same(left: digest, right: SourceText(owner: current, field: "sha256", id: id))
                && digest is "9b29b74a94784a96307282820866e3be9731670dc32e9baa2ddd3bdbcb53f4cb"
                && Number(value: current, key: "frames") >= 13230, file: SourceFile,
                field: "sharedSources.wavSha256", value: id, reason: "shared current sample WAV hash mismatch");
        ValidateOriginal(source: SourceNode(owner: current, field: "source", id: id), id: id);
        return JsonSerializer.Serialize(value: new { kind = "shared", sourceId = "action.2790.full", shared,
            currentSourceSample = current, approvalScope = "Source only; derived trim remains Not auditioned",
            derivedListening = sample.GetProperty(propertyName: "listening") });
    }

    private static int Number(JsonElement value, string key)
    {
        return value.GetProperty(propertyName: key).GetInt32();
    }

    private static string Hash(byte[] bytes)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: bytes));
    }

    private static void Require(bool valid, string file, string field, string value, string reason)
    {
        if (!valid)
        {
            throw new InvalidDataException(message: file + ", field " + field + ", value " + value + " — " + reason);
        }
    }

    private static JsonElement Parse(byte[]? bytes, string file)
    {
        Require(valid: bytes is { Length: > 0 and <= 1_048_576 }, file: file, field: "bytes", value: "read",
                reason: "missing or oversized document");
        using JsonDocument document = JsonDocument.Parse(utf8Json: bytes ?? [], options: new JsonDocumentOptions
        {
            MaxDepth = 32,
        });
        Unique(value: document.RootElement, file: file);
        return document.RootElement.Clone();
    }

    private static void Unique(JsonElement value, string file)
    {
        if (value.ValueKind is JsonValueKind.Object)
        {
            var names = new HashSet<string>(comparer: StringComparer.Ordinal);
            foreach (JsonProperty property in value.EnumerateObject())
            {
                Require(valid: names.Add(item: property.Name), file: file, field: property.Name,
                        value: property.Value.GetRawText(), reason: "duplicate field");
                Unique(value: property.Value, file: file);
            }
        }
        else if (value.ValueKind is JsonValueKind.Array)
        {
            Require(valid: value.GetArrayLength() <= 512, file: file, field: "array", value: "count",
                    reason: "array exceeds512");
            foreach (JsonElement child in value.EnumerateArray())
            {
                Unique(value: child, file: file);
            }
        }
    }

    private void Load(Func<string, byte[]?> read, string category)
    {
        try
        {
            _definition = Parse(bytes: read(arg: Fixture), file: Fixture);
            _variantState = _definition.GetProperty(propertyName: "variantSeed").GetUInt32();
            Require(valid: _variantState > 0, file: Fixture, field: "variantSeed", value: "0",
                    reason: "owned replayable variant seed must be nonzero");
            _catalog = Bound(read: read, file: Events);
            foreach (string pack in new[] { "content/packs/audio-actions.json",
                    "content/packs/audio-interface-ambience.json" })
            {
                LoadPack(read: read, file: pack);
            }

            foreach (JsonElement entry in _catalog.GetProperty(propertyName: "events").EnumerateArray())
            {
                foreach (JsonElement route in entry.GetProperty(propertyName: "routes").EnumerateArray())
                {
                    LoadRoute(route: route);
                    if (Same(left: Text(value: entry, key: "category"), right: category))
                    {
                        _visible.Add(item: Text(value: route, key: "id"));
                    }
                }
            }
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or FormatException
                or InvalidOperationException or KeyNotFoundException or ArgumentException)
        {
            _errors.Add(item: error.Message);
        }
    }

    private JsonElement Bound(Func<string, byte[]?> read, string file)
    {
        byte[]? bytes = read(arg: file);
        JsonElement binding = _definition.GetProperty(propertyName: "bindings").EnumerateArray()
                .Single(predicate: value => Same(left: Text(value: value, key: "file"), right: file));
        Require(valid: bytes is not null && Same(left: Hash(bytes: bytes), right: Text(value: binding, key: "sha256")),
                file: file,
                field: "sha256", value: Text(value: binding, key: "sha256"), reason: "current binding hash mismatch");
        return Parse(bytes: bytes, file: file);
    }

    private void LoadPack(Func<string, byte[]?> read, string file)
    {
        try
        {
            JsonElement pack = Bound(read: read, file: file);
            JsonElement bank = pack.GetProperty(propertyName: "bank");
            string path = Text(value: bank, key: "path");
            Require(valid: path is "assets/audio/actions.wav" or "assets/audio/interface-ambience.wav", file: file,
                    field: "bank.path", value: path, reason: "noncanonical audio bank");
            byte[]? bytes = read(arg: path);
            Require(valid: bytes is not null, file: path, field: "bytes", value: "read", reason: "missing PCM bank");
            foreach (JsonElement sample in pack.GetProperty(propertyName: "samples").EnumerateArray())
            {
                try
                {
                    ExtractPcm(bytes: bytes ?? [], bank: bank, sample: sample);
                    string id = Text(value: sample, key: "id");
                    bool shared = sample.TryGetProperty(propertyName: "sourceId", value: out JsonElement reference)
                            && reference.GetString() is "action.2790.full";
                    JsonElement actions = shared ? Bound(read: read, file: "content/packs/audio-actions.json")
                            : default;
                    string source = ResolveSource(pack: pack, sample: sample, actions: actions);
                    _samples.Add(key: id, value: (sample, bank, source));
                    _banks[path] = bytes ?? [];
                }
                catch (Exception error) when (error is InvalidDataException or InvalidOperationException
                        or KeyNotFoundException or ArgumentException)
                {
                    _errors.Add(item: file + ", field samples, value " + sample.GetRawText() + " — " + error.Message);
                }
            }
        }
        catch (Exception error) when (error is InvalidDataException or JsonException or InvalidOperationException
                or KeyNotFoundException or ArgumentException)
        {
            _errors.Add(item: file + ", field pack, value read — " + error.Message);
        }
    }

    private void LoadRoute(JsonElement route)
    {
        string id = Text(value: route, key: "id");
        try
        {
            JsonElement variants = route.GetProperty(propertyName: "variants");
            Require(valid: variants.GetArrayLength() is > 0 and <= 64 && variants.EnumerateArray().All(
                    predicate: variant => _samples.ContainsKey(key: variant.GetString() ?? "")), file: Events,
                    field: "variants", value: id, reason: "missing or invalid sample binding");
            double gain = route.GetProperty(propertyName: "gainLinear").GetDouble();
            Require(valid: double.IsFinite(d: gain) && gain is >= 0 and <= 1
                    && Number(value: route, key: "priority") is >= 0 and <= 100
                    && Number(value: route, key: "cooldownMilliseconds") is >= 0 and <= 60000
                    && Text(value: route, key: "bus") is "Effects" or "Ambience", file: Events, field: "mix",
                    value: id, reason: "invalid gain/bus/priority/cooldown");
            _routes.Add(key: id, value: route);
        }
        catch (Exception error) when (error is InvalidDataException or InvalidOperationException
                or KeyNotFoundException or ArgumentException)
        {
            _errors.Add(item: Events + ", field routes, value " + id + " — " + error.Message);
        }
    }

    private void Build()
    {
        _gateway.RequireThread();
        var scroll = new ScrollContainer { Name = "Audio", CustomMinimumSize = new Vector2(x: 650, y: 500),
            FollowFocus = true };
        _root.AddChild(node: scroll);
        _panel = new VBoxContainer { Name = "Content", CustomMinimumSize = new Vector2(x: 620, y: 900) };
        scroll.AddChild(node: _panel);
        _selector = new OptionButton { Name = "Event" };
        _panel.AddChild(node: _selector);
        foreach (string id in _visible.Where(predicate: id => _routes.ContainsKey(key: id))
                .Order(comparer: StringComparer.Ordinal))
        {
            _selector.AddItem(label: id);
        }

        _selector.ItemSelected += Select;
        _loop = new CheckButton { Name = "Loop", Text = "Loop explicitly" };
        _panel.AddChild(node: _loop);
        _mute = new CheckButton { Name = "Mute", Text = "Mute" };
        _panel.AddChild(node: _mute);
        _mute.Toggled += Muted;
        Button(text: "Play", action: Play);
        Button(text: "Stop", action: StopAudition);
        Button(text: "Stop all", action: StopAll);
        _status = new Label { Name = "Status" };
        _panel.AddChild(node: _status);
        _details = new TextEdit { Name = "Details", Editable = false, WrapMode = TextEdit.LineWrappingMode.Boundary,
            CustomMinimumSize = new Vector2(x: 600, y: 320) };
        _panel.AddChild(node: _details);
        Volume(name: "Master", value: _master);
        Volume(name: "Effects", value: _effects);
        Volume(name: "Ambience", value: _ambience);
        Limits();
        var states = new OptionButton { Name = "CurrentState" };
        _panel.AddChild(node: states);
        IEnumerable<JsonElement> examples = _definition.ValueKind is JsonValueKind.Object
                ? _definition.GetProperty(propertyName: "states").EnumerateArray() : [];
        foreach (JsonElement state in examples)
        {
            states.AddItem(label: Text(value: state, key: "id"));
        }

        states.ItemSelected += RestoreSelection;
        Refresh();
    }

    private void Button(string text, Action action)
    {
        _gateway.RequireThread();
        var button = new Button { Name = text.Replace(oldValue: " ", newValue: ""), Text = text,
            Disabled = text is "Play" && !_visible.Any(predicate: id => _routes.ContainsKey(key: id)) };
        _panel?.AddChild(node: button);
        button.Pressed += action;
    }

    private void Volume(string name, double value)
    {
        _gateway.RequireThread();
        var label = new Label { Text = name + " volume (%)" };
        _panel?.AddChild(node: label);
        var slider = new HSlider { Name = name, MinValue = 0, MaxValue = 100, Step = 1, Value = value };
        _panel?.AddChild(node: slider);
        slider.ValueChanged += amount => SetVolume(name: name, value: amount);
    }

    private void Limits()
    {
        _gateway.RequireThread();
        foreach (string name in new[] { "gameplayConcurrentLimit", "repeatedEffectLimit" })
        {
            var label = new Label { Text = name };
            _panel?.AddChild(node: label);
            var input = new SpinBox { Name = name, MinValue = 1, MaxValue = name is "gameplayConcurrentLimit"
                    ? 128 : 24, Step = 1, Value = name is "gameplayConcurrentLimit" ? 24 : 4 };
            _panel?.AddChild(node: input);
            input.ValueChanged += _ => ReconcileLimits();
        }
    }

    private void SetVolume(string name, double value)
    {
        _gateway.RequireThread();
        if (_closed)
        {
            return;
        }

        if (name is "Master")
        {
            _master = value;
        }
        else if (name is "Effects")
        {
            _effects = value;
        }
        else
        {
            _ambience = value;
        }

        Refresh();
    }

    private void Muted(bool muted)
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            Refresh();
        }
    }

    private void Select(long index)
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            StopAudition();
            Refresh();
        }
    }

    private void Play()
    {
        _gateway.RequireThread();
        if (_closed || _selector is null || _selector.ItemCount == 0)
        {
            return;
        }

        StopAudition();
        Trigger(routeId: _selector.GetItemText(idx: _selector.Selected), eventId: "audition."
                + (++_serial).ToString(provider: CultureInfo.InvariantCulture), epoch: _epoch,
                loop: _loop?.ButtonPressed ?? false, restoring: false);
    }

    private void RestoreSelection(long index)
    {
        _gateway.RequireThread();
        if (!_closed)
        {
            Restore(routes: [.. _definition.GetProperty(propertyName: "states")[(int)index]
                    .GetProperty(propertyName: "loops").EnumerateArray().Select(selector: value => value.GetString()
                            ?? "")]);
        }
    }

    private bool Admit(string routeId, JsonElement route)
    {
        _gateway.RequireThread();
        int global = (int)(_panel?.GetNode<SpinBox>(path: "gameplayConcurrentLimit").Value ?? 24);
        int effect = (int)(_panel?.GetNode<SpinBox>(path: "repeatedEffectLimit").Value ?? 4);
        effect = Math.Min(val1: effect, val2: Number(value: route.GetProperty(propertyName: "repetition"),
                key: "maximumOverlap"));
        while (_players.Count >= global || _players.Values.Count(predicate: value => Same(
                left: value.Route, right: routeId)) >= effect)
        {
            bool repeated = _players.Values.Count(predicate: value => Same(left: value.Route,
                    right: routeId)) >= effect;
            KeyValuePair<long, (AudioStreamPlayer Player, AudioStreamWav Stream, string Route,
                string Sample, int Priority, bool State)>[] candidates = [.. _players.Where(predicate: pair =>
                        (!repeated || Same(left: pair.Value.Route, right: routeId))
                                && pair.Value.Priority <= Number(value: route, key: "priority"))
                        .OrderBy(keySelector: pair => pair.Value.Priority).ThenBy(keySelector: pair => pair.Key)];
            if (candidates.Length == 0)
            {
                return false;
            }

            Remove(id: candidates[0].Key);
        }

        return true;
    }

    private void ReconcileLimits()
    {
        _gateway.RequireThread();
        if (_closed || _panel is null)
        {
            return;
        }

        int global = (int)_panel.GetNode<SpinBox>(path: "gameplayConcurrentLimit").Value;
        int effect = (int)_panel.GetNode<SpinBox>(path: "repeatedEffectLimit").Value;
        foreach (long id in _players.OrderByDescending(keySelector: pair => pair.Value.Priority)
                .ThenByDescending(keySelector: pair => pair.Key).Skip(count: global)
                .Select(selector: pair => pair.Key).ToArray())
        {
            Remove(id: id);
        }

        foreach (string route in _players.Values.Select(selector: value => value.Route).Distinct(
                comparer: StringComparer.Ordinal).ToArray())
        {
            foreach (long id in _players.Where(predicate: pair => Same(left: pair.Value.Route, right: route))
                    .OrderByDescending(keySelector: pair => pair.Key).Skip(count: effect)
                    .Select(selector: pair => pair.Key).ToArray())
            {
                Remove(id: id);
            }
        }

        Refresh();
    }

    private void Start(string routeId, string sampleId, JsonElement route, bool loop, bool restoring)
    {
        _gateway.RequireThread();
        (JsonElement sample, JsonElement bank, _) = _samples[sampleId];
        byte[] pcm = ExtractPcm(bytes: _banks[Text(value: bank, key: "path")], bank: bank, sample: sample);
        var stream = new AudioStreamWav { Data = pcm, Format = AudioStreamWav.FormatEnum.Format16Bits,
            MixRate = 44100, Stereo = true, LoopMode = loop ? AudioStreamWav.LoopModeEnum.Forward
                    : AudioStreamWav.LoopModeEnum.Disabled, LoopBegin = 0,
            LoopEnd = Number(value: sample, key: "frames") };
        var player = new AudioStreamPlayer { Name = "Player" + (++_serial).ToString(
                provider: CultureInfo.InvariantCulture), Stream = stream, MaxPolyphony = 1 };
        player.SetMeta(name: "route", value: routeId);
        player.SetMeta(name: "sample", value: sampleId);
        player.SetMeta(name: "epoch", value: _epoch);
        player.SetMeta(name: "state_loop", value: restoring);
        _root.AddChild(node: player);
        _players.Add(key: _serial, value: (player, stream, routeId, sampleId,
                Number(value: route, key: "priority"), restoring));
        ApplyVolume(player: player, route: route);
        player.Play(fromPosition: 0);
    }

    private uint DrawVariant(int choices)
    {
        uint maximum = uint.MaxValue - (uint.MaxValue % (uint)choices);
        do
        {
            _variantState ^= _variantState << 13;
            _variantState ^= _variantState >> 17;
            _variantState ^= _variantState << 5;
        }
        while (_variantState > maximum);
        return (_variantState - 1) % (uint)choices;
    }

    private void ApplyVolume(AudioStreamPlayer player, JsonElement route)
    {
        _gateway.RequireThread();
        double volume = (_mute?.ButtonPressed ?? false) ? 0 : _master / 100 * (Text(value: route, key: "bus")
                is "Ambience" ? _ambience : _effects) / 100 * route.GetProperty(propertyName: "gainLinear").GetDouble();
        player.VolumeDb = volume <= 0 ? -80 : (float)(20 * Math.Log10(d: volume));
    }

    private void Remove(long id)
    {
        _gateway.RequireThread();
        (AudioStreamPlayer player, AudioStreamWav stream, _, _, _, _) = _players[id];
        _players.Remove(key: id);
        player.Stop();
        player.Stream = null;
        _root.RemoveChild(node: player);
        player.Free();
        stream.Dispose();
    }

    private void StopAudition()
    {
        _gateway.RequireThread();
        foreach (long id in _players.Where(predicate: pair => !pair.Value.State)
                .Select(selector: pair => pair.Key).ToArray())
        {
            Remove(id: id);
        }

        if (!_closed)
        {
            Refresh();
        }
    }

    private void StopAll()
    {
        _gateway.RequireThread();
        foreach (long id in _players.Keys.ToArray())
        {
            Remove(id: id);
        }

        if (!_closed)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        _gateway.RequireThread();
        if (_closed || _panel is null || _status is null || _details is null || _selector is null)
        {
            return;
        }

        foreach ((AudioStreamPlayer player, _, string route, _, _, _) in _players.Values)
        {
            ApplyVolume(player: player, route: _routes[route]);
        }

        bool muted = CurrentMuted();
        _status.Text = (muted ? "Muted · Sound muted" : _players.Count > 0 ? "Playing" : "Stopped")
                + " · time " + _time.ToString(format: "F2", provider: CultureInfo.InvariantCulture) + "s";
        _panel.SetMeta(name: "epoch", value: _epoch);
        _panel.SetMeta(name: "one_shots", value: _oneShots);
        _panel.SetMeta(name: "players", value: _players.Count);
        _panel.SetMeta(name: "loops", value: string.Join(separator: ",", values: _players.Values
                .Where(predicate: value => value.State).Select(selector: value => value.Route)
                .Order(comparer: StringComparer.Ordinal)));
        string details = string.Join(separator: "\n", values: _errors);
        if (_selector.ItemCount > 0)
        {
            JsonElement route = _routes[_selector.GetItemText(idx: _selector.Selected)];
            details += "\n" + route.GetRawText();
            foreach (JsonElement variant in route.GetProperty(propertyName: "variants").EnumerateArray())
            {
                (JsonElement sample, JsonElement bank, string source) = _samples[variant.GetString() ?? ""];
                details += "\n" + bank.GetRawText() + "\n" + sample.GetRawText() + "\nSource " + source;
            }

            if (route.GetProperty(propertyName: "warningsVisibleWhenMuted").GetBoolean())
            {
                string id = Text(value: route, key: "id");
                string warning = _definition.GetProperty(propertyName: "warnings").TryGetProperty(
                        propertyName: id, value: out JsonElement message) ? message.GetString() ?? id : id;
                _status.Text += "\nCritical visual warning: " + warning + " · " + string.Join(
                        separator: "; ", values: route
                                .GetProperty(propertyName: "visualPairs").EnumerateArray().Select(
                                        selector: value => value.GetString() ?? ""));
            }
        }

        _details.Text = details + "\nSource/headless playback and lifetime only. New27 variants Not auditioned; "
                + "listening, loop seams and final mix remain04-32/33.";
    }

    private bool CurrentMuted()
    {
        _gateway.RequireThread();
        double channel = _selector is not null && _selector.ItemCount > 0
                && Text(value: _routes[_selector.GetItemText(idx: _selector.Selected)], key: "bus") is "Ambience"
                ? _ambience : _effects;
        return IsMuted(muted: _mute?.ButtonPressed ?? false, master: _master, channel: channel);
    }
}
