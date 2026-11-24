using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Godot;
using KingdomZombies.Core;
using KingdomZombies.Inspection;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Owns one persistent survivor or corpse's authored native composition.</summary>
internal sealed class FoundationActorArt : IDisposable
{
    public void Dispose()
    {
        _gateway.RequireThread();
        _composition.CloseComposition();
        _root.GetParent().RemoveChild(node: _root);
        _root.QueueFree();
    }

    internal FoundationActorArt(Node2D root, MainThreadGateway gateway, FoundationAppearanceAssets assets,
            string body, bool hostile)
    {
        _root = root;
        _gateway = gateway;
        _gateway.RequireThread();
        _state = assets.State();
        _motion = assets.Presentation();
        _collapse = assets.CollapseMilliseconds;
        _composition = CharacterPreview.OpenComposition(root: root, gateway: gateway, read: assets.Read,
                body: body, animated: true);
        if (hostile)
        {
            root.Modulate = new Color(r: 0.78f, g: 0.9f, b: 0.73f);
        }

        _crown = new Sprite2D
        {
            Name = "Crown",
            Texture = assets.Crown,
            Centered = false,
            Position = new Vector2(x: 24, y: 5),
            ZIndex = 40,
            Visible = false,
        };
        root.AddChild(node: _crown);
    }

    internal void Apply(FoundationEquipmentProjection.Subject subject, long elapsed, Vector2 at,
            (double x, double y)? cursor)
    {
        _gateway.RequireThread();
        _root.Position = at - new Vector2(x: 24, y: 48);
        bool wear = _layers.IsDefault || !Enumerable.SequenceEqual(first: subject.Layers, second: _layers)
                || _crown.Visible != subject.Crowned;
        if (wear)
        {
            _layers = subject.Layers;
            _state.Restore(body: subject.Body, kind: subject.DiedAt.HasValue ? "corpse" : "alive",
                    equipment: FoundationEquipmentProjection.Wear(subject: subject), selected: false);
            _crown.Visible = _state.Snapshot.GetProperty(propertyName: "equipment")
                    .TryGetProperty(propertyName: "crown", value: out _);
            _wear = FoundationEquipmentProjection.Fits(subject: subject);
            _broken = subject.Layers.Where(predicate: layer => layer.Broken)
                    .Select(selector: layer => layer.Slot).ToImmutableHashSet(equalityComparer: StringComparer.Ordinal);
        }

        if (subject.DiedAt is { } died)
        {
            string pose = CorpsePose(age: elapsed - died);
            Draw(subject: subject, upper: pose, lower: pose, facing: subject.Facing, track: default, wear: wear);
        }
        else
        {
            DrawLiving(subject: subject, elapsed: elapsed, cursor: cursor, wear: wear);
        }
    }

    private readonly Node2D _root;
    private readonly MainThreadGateway _gateway;
    private readonly CharacterPreview _composition;
    private readonly AppearanceState _state;
    private readonly CharacterPresentation _motion;
    private readonly Sprite2D _crown;
    private readonly double _collapse;
    private ImmutableArray<FoundationEquipmentProjection.Layer> _layers;
    private ImmutableHashSet<string> _broken = [];
    private JsonElement _wear;
    private long _lastElapsed = -1;
    private double? _lastStrike;
    private string _lastPose = "";

    private string CorpsePose(double age)
    {
        return age >= _collapse ? "corpse" : age < _collapse / 2 ? "collapse-a" : "collapse-b";
    }

    private void DrawLiving(FoundationEquipmentProjection.Subject subject, long elapsed,
            (double x, double y)? cursor, bool wear)
    {
        string movement = subject.Movement is "jump" && subject.Body is not "CH-01" ? "idle" : subject.Movement;
        FoundationEquipmentProjection.Layer left = subject.Left;
        FoundationEquipmentProjection.Layer right = subject.Right;
        (double x, double y)? target = subject.Aim ?? cursor;
        bool bow = target.HasValue && FoundationEquipmentProjection.HasBow(subject: subject);
        string action = bow ? "bow" : subject.StrikeAt.HasValue && !subject.Aim.HasValue ? "melee" : "none";
        (double angle, int facing) aim = target is { } desired && bow ? FoundationEquipmentProjection.BowAngle(
                dx: desired.x - subject.At.x, dy: subject.At.y - desired.y, facing: subject.Facing)
                : (0, subject.Facing);
        bool freshStrike = subject.StrikeAt.HasValue && subject.StrikeAt != _lastStrike;
        if (freshStrike)
        {
            _motion.Configure(body: subject.Body, movement: movement, action: "none", leftItem: left.Item,
                    rightItem: right.Item, facing: aim.facing);
        }

        _motion.Configure(body: subject.Body, movement: movement, action: action,
                leftItem: action is "melee" && left.Broken ? "Empty" : left.Item,
                rightItem: action is "melee" && right.Broken ? "Empty" : right.Item, facing: aim.facing);
        if (bow)
        {
            bool ready = subject.ArrowReady && !subject.Aim.HasValue;
            _motion.ConfigureBow(aimRadians: aim.angle, leftArrows: ready && left.Item is "IT-02" ? 1 : 0,
                    rightArrows: ready && right.Item is "IT-02" ? 1 : 0, leftUsable: !left.Broken,
                    rightUsable: !right.Broken, stackIdentity: "committed-hand", intrinsicAmmunition: false,
                    holdUnloaded: true);
        }

        double delta = freshStrike && subject.StrikeAt is { } strike ? elapsed - strike
                : _lastElapsed < 0 ? 0 : elapsed - _lastElapsed;
        _motion.Advance(elapsedSeconds: Math.Clamp(value: delta / 1000, min: 0, max: 60));
        _lastElapsed = elapsed;
        _lastStrike = subject.StrikeAt;
        JsonElement sample = _motion.Snapshot;
        bool flinch = subject.FlinchAt is { } at && elapsed - at is >= 0 and < 250;
        string upper = flinch ? "hit" : bow ? "aim" : Text(sample: sample, field: "upperPose");
        Draw(subject: subject, upper: upper, lower: Text(sample: sample, field: "lowerPose"),
                facing: aim.facing, track: sample, wear: wear);
    }

    private void Draw(FoundationEquipmentProjection.Subject subject, string upper, string lower, int facing,
            JsonElement track, bool wear)
    {
        _gateway.RequireThread();
        string pose = upper + "|" + lower + "|" + facing.ToString(provider: CultureInfo.InvariantCulture)
                + "|" + TrackKey(sample: track);
        if (!wear && StringComparer.Ordinal.Equals(x: pose, y: _lastPose))
        {
            return;
        }

        _composition.RestoreComposition(body: subject.Body, pose: upper, facing: facing, equipment: _wear,
                lowerPose: lower, sample: track, brokenSlots: _broken);
        _lastPose = pose;
    }

    private static string TrackKey(JsonElement sample)
    {
        if (sample.ValueKind != JsonValueKind.Object)
        {
            return "";
        }

        string[] fields = ["lowerOffset", "upperOffset", "leftAngle", "rightAngle", "flinchOffset",
            "transitionOffset", "nocked", "bowStage", "aimRadians"];
        string hold = Text(sample: sample, field: "action") is "bow"
                ? Math.Floor(d: sample.GetProperty(propertyName: "actionSeconds").GetDouble() * 30)
                        .ToString(provider: CultureInfo.InvariantCulture) : "";
        return string.Join(separator: "|", values: fields.Select(selector: field =>
                sample.GetProperty(propertyName: field).GetRawText())) + "|" + hold;
    }

    private static string Text(JsonElement sample, string field)
    {
        return sample.GetProperty(propertyName: field).GetString() ?? "idle";
    }
}
