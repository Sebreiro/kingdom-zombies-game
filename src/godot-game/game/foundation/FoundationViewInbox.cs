using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Stages bounded recipient parts until one complete authorized window can replace the current view.</summary>
internal sealed class FoundationViewInbox
{
    internal ImmutableDictionary<string, FoundationRunView> Views { get; private set; } =
            ImmutableDictionary.Create<string, FoundationRunView>(keyComparer: StringComparer.Ordinal);

    internal (string survivorId, long generation, long deathGeneration)? Control => Views.TryGetValue(
            key: "status", value: out FoundationRunView? status) ? (Text(value: status.Header, name: "SurvivorId"),
                    status.Epoch, status.Header.GetProperty(propertyName: "DeathGeneration").GetInt64()) : null;

    internal bool Offer(FoundationEnvelope envelope, long now)
    {
        if (envelope.Game is not { } frame || frame.View <= Floor(scope: frame.Scope)
                || envelope.Revision < _controlFence)
        {
            return false;
        }

        if (envelope.Kind is "view-revoke")
        {
            if (_pending.TryGetValue(key: frame.Scope, value: out Stage? staged) && staged.Serial > frame.View)
            {
                return false;
            }

            ClearScope(scope: frame.Scope, serial: frame.View);
            return true;
        }

        if (frame.Body is not { } header || frame.Scope is not "status" && Control?.generation != frame.Epoch)
        {
            return false;
        }

        Stage? stage = SelectStage(scope: frame.Scope, serial: frame.View, envelope: envelope, now: now);
        if (stage is null)
        {
            return false;
        }

        if (!stage.Match(envelope: envelope, frame: frame, header: header)
                || !stage.Add(index: frame.Part, rows: frame.Rows))
        {
            ClearScope(scope: frame.Scope, serial: frame.View);
            return false;
        }

        if (stage.Complete)
        {
            Install(scope: frame.Scope, stage: stage);
        }

        return true;
    }

    internal void ObservePublic(JsonElement world, long revision, string binding)
    {
        if (!Views.TryGetValue(key: "status", value: out FoundationRunView? status) || revision < status.Revision)
        {
            return;
        }

        string actual = world.GetProperty(propertyName: "Actors").EnumerateArray().Where(predicate: actor =>
                StringComparer.Ordinal.Equals(x: Text(value: actor, name: "BindingId"), y: binding))
                .Select(selector: actor => Text(value: actor, name: "Id")).FirstOrDefault() ?? "";
        string expected = Text(value: status.Header, name: "SurvivorId");
        if (!StringComparer.Ordinal.Equals(x: actual, y: expected))
        {
            _controlFence = Math.Max(val1: _controlFence, val2: revision);
            ClearScope(scope: "status", serial: Floor(scope: "status"));
            ClearScope(scope: "own", serial: Floor(scope: "own"));
            ClearScope(scope: "external", serial: Floor(scope: "external"));
        }
    }

    internal void Expire(long now)
    {
        foreach ((string scope, Stage stage) in _pending.ToArray())
        {
            if (now - stage.StartedAt >= PartTimeout)
            {
                _pending.Remove(key: scope);
                _watermarks[scope] = stage.Serial;
            }
        }
    }

    internal void Clear()
    {
        Views = Views.Clear();
        _pending.Clear();
        _watermarks.Clear();
        _controlFence = 0;
    }

    internal void Disconnect(long revision)
    {
        _controlFence = Math.Max(val1: _controlFence, val2: revision);
        ClearScope(scope: "status", serial: Floor(scope: "status"));
        ClearScope(scope: "own", serial: Floor(scope: "own"));
        ClearScope(scope: "external", serial: Floor(scope: "external"));
    }

    private const long PartTimeout = 5000;
    private readonly Dictionary<string, Stage> _pending = new(comparer: StringComparer.Ordinal);
    private readonly Dictionary<string, long> _watermarks = new(comparer: StringComparer.Ordinal);
    private long _controlFence;

    private long Floor(string scope)
    {
        return _watermarks.GetValueOrDefault(key: scope);
    }

    private Stage? SelectStage(string scope, long serial, FoundationEnvelope envelope, long now)
    {
        bool previous = _pending.TryGetValue(key: scope, value: out Stage? stage);
        if (stage is not null && serial < stage.Serial)
        {
            return null;
        }

        if (stage is not null && serial == stage.Serial)
        {
            return now - stage.StartedAt < PartTimeout ? stage : null;
        }

        if (previous && stage is not null)
        {
            _watermarks[scope] = stage.Serial;
        }

        stage = new Stage(envelope: envelope, now: now);
        _pending[scope] = stage;
        return stage;
    }

    private void Install(string scope, Stage stage)
    {
        _pending.Remove(key: scope);
        _watermarks[scope] = stage.Serial;
        if (Views.TryGetValue(key: scope, value: out FoundationRunView? previous) && previous.Revision > stage.Revision)
        {
            return;
        }

        if (scope is "status" && Control?.generation != stage.Epoch)
        {
            ClearScope(scope: "own", serial: Floor(scope: "own"));
            ClearScope(scope: "external", serial: Floor(scope: "external"));
        }

        Views = Views.SetItem(key: scope, value: stage.Build());
    }

    private void ClearScope(string scope, long serial)
    {
        if (_pending.Remove(key: scope, value: out Stage? stage))
        {
            serial = Math.Max(val1: serial, val2: stage.Serial);
        }

        _watermarks[scope] = Math.Max(val1: serial, val2: Floor(scope: scope));
        Views = Views.Remove(key: scope);
    }

    private static string Text(JsonElement value, string name)
    {
        return value.GetProperty(propertyName: name).GetString() ?? "";
    }

    private sealed class Stage
    {
        internal Stage(FoundationEnvelope envelope, long now)
        {
            FoundationRunFrame frame = envelope.Game ?? throw new InvalidOperationException(message: "RUN_PART");
            Serial = frame.View;
            Revision = envelope.Revision;
            Epoch = frame.Epoch;
            Token = frame.Token;
            _correlation = envelope.Sequence;
            _header = frame.Body ?? throw new InvalidOperationException(message: "RUN_HEADER");
            _parts = new ImmutableArray<JsonElement>[frame.Parts];
            StartedAt = now;
        }

        internal long Serial { get; }
        internal long Revision { get; }
        internal long Epoch { get; }
        internal long Token { get; }
        internal long StartedAt { get; }
        internal bool Complete => _received == _parts.Length;

        internal bool Match(FoundationEnvelope envelope, FoundationRunFrame frame, JsonElement header)
        {
            return frame.Parts == _parts.Length && frame.Epoch == Epoch && frame.Token == Token
                    && envelope.Revision == Revision && envelope.Sequence == _correlation
                    && JsonElement.DeepEquals(element1: _header, element2: header);
        }

        internal bool Add(int index, ImmutableArray<JsonElement> rows)
        {
            if (!_parts[index].IsDefault)
            {
                return _parts[index].Length == rows.Length && _parts[index].Zip(second: rows,
                        resultSelector: (first, second) => JsonElement.DeepEquals(element1: first, element2: second))
                        .All(predicate: equal => equal);
            }

            if (_rowCount + rows.Length > FoundationProtocol.MaximumViewRows)
            {
                return false;
            }

            if (rows.Any(predicate: row => !_identities.Add(item: row.GetProperty(propertyName: "Id").GetInt64())))
            {
                return false;
            }

            _parts[index] = rows;
            _rowCount += rows.Length;
            _received++;
            return true;
        }

        internal FoundationRunView Build()
        {
            return new FoundationRunView(revision: Revision, epoch: Epoch, token: Token,
                    header: _header, rows: [.. _parts.SelectMany(selector: rows => rows)]);
        }

        private readonly ImmutableArray<JsonElement>[] _parts;
        private readonly JsonElement _header;
        private readonly long _correlation;
        private readonly HashSet<long> _identities = [];
        private int _received;
        private int _rowCount;
    }
}
