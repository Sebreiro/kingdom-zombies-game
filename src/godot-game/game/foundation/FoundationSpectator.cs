using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Owns only presentation rights, explicit spectator choices and verified camera transitions.</summary>
internal sealed class FoundationSpectator
{
    internal bool CanDrive { get; private set; }
    internal bool Visible { get; private set; }
    internal bool CanRespawn => Visible && _deathEpoch > 0 && _deathStatus is "Ready" && _pending is null;
    internal bool Pending => _pending.HasValue;
    internal string Status => Pending && _deathStatus is not "Waiting" ? "Requesting survivor…"
            : _deathStatus switch
            {
                "Early" => "Respawn available shortly",
                "Waiting" or "Defeated" => "Waiting for a survivor",
                _ => "",
            };
    internal long DeathEpoch => _deathEpoch;
    internal double? Camera { get; private set; }
    internal bool Smoothing => _handover.HasValue;
    internal string? RetiredSurvivor { get; private set; }
    internal ImmutableArray<Teammate> Teammates { get; private set; } = [];
    internal Teammate? Followed { get; private set; }
    internal string FollowNotice { get; private set; } = "";
    internal string TransferNotice { get; private set; } = "";

    internal void Observe(FoundationRunReadback? readback, string binding, long now, double center, double span)
    {
        if (readback is null)
        {
            HoldUnavailable(center: center);
            return;
        }

        if (!StringComparer.Ordinal.Equals(x: _session, y: readback.Roster.SessionId))
        {
            Clear();
            _session = readback.Roster.SessionId;
        }

        if (!readback.Views.TryGetValue(key: "status", value: out FoundationRunView? current))
        {
            HoldUnavailable(center: center);
            return;
        }

        if (current.Revision < _revision)
        {
            return;
        }

        _revision = current.Revision;
        JsonElement data = current.Header;
        if (readback.World is { } world)
        {
            _origin = world.GetProperty(propertyName: "OriginX").GetInt64();
        }

        if (current.Epoch > 0)
        {
            ObserveLive(readback: readback, view: current, binding: binding, now: now, center: center, span: span);
        }
        else
        {
            ObserveDead(readback: readback, data: data, binding: binding, center: center);
        }
    }

    internal void Clear()
    {
        _session = "";
        _revision = -1;
        _origin = 0;
        _generation = 0;
        _survivor = "";
        _deathEpoch = 0;
        _deathStatus = "";
        _held = 0;
        _pending = null;
        _handover = null;
        _transferUntil = 0;
        CanDrive = false;
        Visible = false;
        RetiredSurvivor = null;
        Camera = null;
        Teammates = [];
        Followed = null;
        FollowNotice = "";
        TransferNotice = "";
        _known.Clear();
    }

    internal bool BeginRequest(long epoch, long sequence)
    {
        if (!CanRespawn || sequence <= 0 || epoch != _deathEpoch)
        {
            return false;
        }

        _pending = sequence;
        return true;
    }

    internal bool Cycle(bool previous)
    {
        if (!Visible || Teammates.IsEmpty)
        {
            return false;
        }

        int old = Followed is null ? -1 : Teammates.IndexOf(item: Followed);
        int index = old < 0 ? (previous ? Teammates.Length - 1 : 0)
                : (old + (previous ? -1 : 1) + Teammates.Length) % Teammates.Length;
        return Follow(choice: Teammates[index]);
    }

    internal bool Follow(Teammate choice)
    {
        if (!Visible || !Teammates.Contains(item: choice))
        {
            return false;
        }

        Followed = choice;
        FollowNotice = "";
        return true;
    }

    internal void RememberVisible(double center, double span)
    {
        if (!CanDrive && !Visible || span <= 0)
        {
            return;
        }

        double left = center - span / 2;
        double right = center + span / 2;
        foreach ((double a, double b) in _known.ToArray())
        {
            if (right >= a && left <= b)
            {
                left = Math.Min(val1: left, val2: a);
                right = Math.Max(val1: right, val2: b);
                _ = _known.Remove(item: (a, b));
            }
        }

        _known.Add(item: (left, right));
    }

    private readonly List<(double left, double right)> _known = [];
    private string _session = "";
    private long _revision = -1;
    private long _origin;
    private long _generation;
    private string _survivor = "";
    private long _deathEpoch;
    private string _deathStatus = "";
    private double _held;
    private long? _pending;
    private (double from, double to, long at)? _handover;
    private long _transferUntil;

    private void ObserveLive(FoundationRunReadback readback, FoundationRunView view, string binding, long now,
            double center, double span)
    {
        string survivor = Text(value: view.Header, field: "SurvivorId");
        if (survivor.Length == 0 || view.Epoch < _generation)
        {
            HoldUnavailable(center: center);
            return;
        }

        Camera = null;
        if (view.Epoch != _generation && _deathEpoch > 0)
        {
            if (Pose(world: readback.World, binding: binding, survivor: survivor) is not { } target)
            {
                HoldUnavailable(center: center);
                return;
            }

            Transfer(target: target, now: now, center: center, span: span);
        }

        _generation = view.Epoch;
        _survivor = survivor;
        _deathEpoch = 0;
        _pending = null;
        CanDrive = true;
        Visible = false;
        Teammates = [];
        Followed = null;
        FollowNotice = "";
        TransferNotice = now < _transferUntil ? "Control transferred · " + survivor : "";
        AdvanceHandover(now: now);
    }

    private void ObserveDead(FoundationRunReadback readback, JsonElement data, string binding, double center)
    {
        long death = data.GetProperty(propertyName: "DeathGeneration").GetInt64();
        if (_deathStatus.Length == 0 || death != _deathEpoch)
        {
            StartDeath(data: data, epoch: death, center: center);
        }

        CanDrive = false;
        Visible = true;
        _deathStatus = Text(value: data, field: "Status");
        if (_deathStatus is "Ready" && (Number(data: data, field: "OpensAt") is not { } opens
                || data.GetProperty(propertyName: "RootTime").GetInt64() < opens))
        {
            _deathStatus = "Early";
        }

        Teammates = ReadTeammates(data: data, roster: readback.Roster, binding: binding);
        UpdateFollow(world: readback.World);
        if (_pending is { } requested && readback.Feedback is { } receipt && receipt.sequence == requested
                && !receipt.replayed && receipt.status is not ("Assigned" or "Waiting"))
        {
            _pending = null;
        }
    }

    private void StartDeath(JsonElement data, long epoch, double center)
    {
        _deathEpoch = epoch;
        _pending = null;
        _handover = null;
        _transferUntil = 0;
        TransferNotice = "";
        Followed = null;
        FollowNotice = "";
        RetiredSurvivor = _survivor.Length == 0 ? RetiredSurvivor : _survivor;
        _held = Number(data: data, field: "DeathX") is { } death ? Clamp(value: Pixels(contact: death))
                : Clamp(value: center);
    }

    private void UpdateFollow(JsonElement? world)
    {
        if (Followed is { } followed)
        {
            Teammate? current = Teammates.FirstOrDefault(predicate: choice => choice.Same(other: followed));
            if (current is null)
            {
                FollowNotice = followed.Label + " is no longer available to follow. Choose a living teammate.";
                Followed = null;
            }
            else
            {
                Followed = current;
                if (Pose(world: world, binding: current.Binding, survivor: current.Survivor) is { } contact)
                {
                    _held = Clamp(value: contact);
                }
            }
        }

        Camera = _held;
    }

    private void Transfer(double target, long now, double center, double span)
    {
        double from = Clamp(value: center);
        double to = Clamp(value: target);
        double left = Math.Min(val1: from, val2: to) - span / 2;
        double right = Math.Max(val1: from, val2: to) + span / 2;
        bool nearby = Math.Abs(value: target - from) <= span / 4;
        if (nearby && _known.Any(predicate: range => range.left <= left && range.right >= right) && from != to)
        {
            _handover = (from, to, now);
        }
        else
        {
            Camera = to;
        }

        _transferUntil = now + 2000;
    }

    private void AdvanceHandover(long now)
    {
        if (_handover is not { } motion)
        {
            return;
        }

        double t = Math.Clamp(value: (now - motion.at) / 250d, min: 0, max: 1);
        Camera = motion.from + (motion.to - motion.from) * t * t * (3 - 2 * t);
        if (t >= 1)
        {
            _handover = null;
        }
    }

    private void HoldUnavailable(double center)
    {
        CanDrive = false;
        Visible = false;
        Camera = Clamp(value: center);
        Teammates = [];
        Followed = null;
    }

    private double? Pose(JsonElement? world, string binding, string survivor)
    {
        if (world is not { } data)
        {
            return null;
        }

        foreach (JsonElement actor in data.GetProperty(propertyName: "Actors").EnumerateArray())
        {
            if (StringComparer.Ordinal.Equals(x: Text(value: actor, field: "Id"), y: survivor)
                    && StringComparer.Ordinal.Equals(x: Text(value: actor, field: "BindingId"), y: binding))
            {
                return Pixels(contact: actor.GetProperty(propertyName: "X").GetDouble());
            }
        }

        return null;
    }

    private double Pixels(double contact)
    {
        return (contact - _origin) * 16 / 1000;
    }

    private static ImmutableArray<Teammate> ReadTeammates(JsonElement data, FoundationSnapshot roster, string binding)
    {
        ImmutableArray<Teammate>.Builder rows = ImmutableArray.CreateBuilder<Teammate>();
        foreach (JsonElement row in data.GetProperty(propertyName: nameof(Teammates)).EnumerateArray())
        {
            string key = Text(value: row, field: "BindingId");
            int slot = row.GetProperty(propertyName: "Slot").GetInt32();
            FoundationParticipantState? ready = roster.Participants.FirstOrDefault(predicate: player =>
                    player.Ready && player.Slot == slot && StringComparer.Ordinal.Equals(x: player.BindingId, y: key));
            if (ready is not null && !StringComparer.Ordinal.Equals(x: key, y: binding))
            {
                rows.Add(item: new Teammate(binding: key, survivor: Text(value: row, field: "SurvivorId"),
                        slot: slot, label: ready.Label));
            }
        }

        return [.. rows.OrderBy(keySelector: row => row.Slot)];
    }

    private static double Clamp(double value)
    {
        return Math.Clamp(value: value, min: -320, max: 320);
    }

    private static double? Number(JsonElement data, string field)
    {
        JsonElement property = data.GetProperty(propertyName: field);
        return property.ValueKind == JsonValueKind.Null ? null : property.GetDouble();
    }

    private static string Text(JsonElement value, string field)
    {
        return value.GetProperty(propertyName: field).GetString() ?? "";
    }

    internal sealed record Teammate
    {
        internal Teammate(string binding, string survivor, int slot, string label)
        {
            Binding = binding;
            Survivor = survivor;
            Slot = slot;
            Label = label;
        }

        internal string Binding { get; }
        internal string Survivor { get; }
        internal int Slot { get; }
        internal string Label { get; }

        internal bool Same(Teammate other)
        {
            return StringComparer.Ordinal.Equals(x: Binding, y: other.Binding)
                    && StringComparer.Ordinal.Equals(x: Survivor, y: other.Survivor);
        }
    }
}
