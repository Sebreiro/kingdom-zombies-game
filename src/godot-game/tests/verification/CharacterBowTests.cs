using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual ammunition, bow timing and detached composed samples.</summary>
public sealed class CharacterBowTests
{
    /// <summary>Explicit melee preserves its fist fallback when a bow is selected.</summary>
    [Fact]
    public void Melee_WithBow_PreservesFistFallback()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-01", movement: "walk", action: "melee", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 0.7);
        subject.Snapshot.GetProperty(propertyName: "releaseSound").GetString().ShouldBe(expected: "AU-03");
    }

    /// <summary>The actual live usable bow and arrow preview offers its required bow action.</summary>
    [Fact]
    public void Native_UsableBow_OffersBowAction()
    {
        string path = Environment.GetEnvironmentVariable(variable: "BOW_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "BOW_NATIVE_REPORT_REQUIRED");
        using var report = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: path));
        JsonElement bow = report.RootElement.GetProperty(propertyName: "bow");
        bow.GetProperty(propertyName: "leftItem").GetString().ShouldBe(expected: "IT-07");
        bow.GetProperty(propertyName: "rightItem").GetString().ShouldBe(expected: "IT-02");
        bow.GetProperty(propertyName: "condition").GetString().ShouldBe(expected: "Usable");
        bow.GetProperty(propertyName: "offered").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Zero, one and many selected arrows independently control nocking in either logical hand.</summary>
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(12, true)]
    public void Nock_ActualAmmunition_FollowsQuantityAndCondition(int count, bool expected)
    {
        foreach (int facing in new[] { -1, 1 })
        {
            foreach (bool left in new[] { false, true })
            {
                CharacterPresentation subject = Create(bow: true);
                subject.Configure(body: "CH-08", movement: "walk", action: "bow",
                        leftItem: left ? "IT-07" : "IT-02", rightItem: left ? "IT-02" : "IT-07", facing: facing);
                subject.ConfigureBow(aimRadians: 0.237, leftArrows: left ? 0 : count,
                        rightArrows: left ? count : 0, leftUsable: true, rightUsable: true, stackIdentity: "actual-a");
                subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBe(expected: expected);
                subject.Snapshot.GetProperty(propertyName: "bowHand").GetString().ShouldBe(
                        expected: left ? "left" : "right");
            }
        }
    }

    /// <summary>Release is one shot per generation, last arrows vanish and replacement is immediate.</summary>
    [Fact]
    public void LastArrow_ReleaseAndReplacement_PreserveDetachedSamples()
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.ConfigureBow(aimRadians: -0.37, leftArrows: 0, rightArrows: 1, leftUsable: true,
                rightUsable: true, stackIdentity: "actual-a");
        JsonElement before = subject.Snapshot;
        long generation = before.GetProperty(propertyName: "generation").GetInt64();
        subject.TryRelease(generation: generation).ShouldBeFalse();
        subject.Advance(elapsedSeconds: 0.6);
        subject.TryRelease(generation: generation).ShouldBeTrue();
        subject.TryRelease(generation: generation).ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "pickupCreated").GetBoolean().ShouldBeFalse();
        before.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
        subject.ConfigureBow(aimRadians: 0.37, leftArrows: 0, rightArrows: 12, leftUsable: true,
                rightUsable: true, stackIdentity: "actual-b");
        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
        subject.Snapshot.GetProperty(propertyName: "actionSeconds").GetDouble().ShouldBe(expected: 0);
        subject.TryRelease(generation: generation).ShouldBeFalse();
        subject.ConfigureBow(aimRadians: 0.37, leftArrows: 0, rightArrows: 12, leftUsable: false,
                rightUsable: true, stackIdentity: "actual-b");
        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 0.7);
    }

    /// <summary>Actual authored stack bounds and finite angle bounds reject impossible presentation inputs.</summary>
    [Fact]
    public void BowSelection_ActualStack_UsesDefinitionAndFiniteAngles()
    {
        CharacterPresentation subject = Create(bow: true);
        foreach (double angle in new[] { double.NaN, double.PositiveInfinity, -2, 2 })
        {
            Should.Throw<InvalidDataException>(actual: () => subject.ConfigureBow(aimRadians: angle,
                    leftArrows: 0, rightArrows: 1, leftUsable: true, rightUsable: true, stackIdentity: "a"));
        }

        foreach (int count in new[] { -1, 31 })
        {
            Should.Throw<InvalidDataException>(actual: () => subject.ConfigureBow(aimRadians: 0,
                    leftArrows: count, rightArrows: 0, leftUsable: true, rightUsable: true, stackIdentity: "a"));
            Should.Throw<InvalidDataException>(actual: () => subject.ConfigureBow(aimRadians: 0,
                    leftArrows: 0, rightArrows: count, leftUsable: true, rightUsable: true, stackIdentity: "a"));
        }

        subject = Create(bow: true, alterItems: SetArrowMaximum);
        subject.ConfigureBow(aimRadians: Math.PI / 2, leftArrows: 2, rightArrows: 0, leftUsable: true,
                rightUsable: true, stackIdentity: "actual");
        Should.Throw<InvalidDataException>(actual: () => subject.ConfigureBow(aimRadians: 0,
                leftArrows: 3, rightArrows: 0, leftUsable: true, rightUsable: true, stackIdentity: "actual"));
    }

    /// <summary>Empty or broken bow actions fall back only to actual usable melee gear or fists.</summary>
    [Theory]
    [InlineData("IT-07", "Empty", false, true, 0.7)]
    [InlineData("Empty", "IT-07", true, false, 0.7)]
    [InlineData("IT-03", "Empty", true, true, 0.5)]
    [InlineData("IT-03", "Empty", false, true, 0.7)]
    [InlineData("Empty", "IT-06", true, true, 1.1)]
    [InlineData("Empty", "IT-06", true, false, 0.7)]
    public void BowFallback_EmptyOrUnavailableBow_UsesUsableMeleeOrFists(string left, string right, bool leftUsable,
            bool rightUsable, double interval)
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: left,
                rightItem: right, facing: -1);
        subject.ConfigureBow(aimRadians: 0.2, leftArrows: 0, rightArrows: 0, leftUsable: leftUsable,
                rightUsable: rightUsable, stackIdentity: "fallback");
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: interval);
        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "pickupCreated").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "gameplayEvents").GetInt32().ShouldBe(expected: 0);
    }

    /// <summary>Native samples cover independently chosen aim angles, both hands, facing and every body.</summary>
    [Fact]
    public void Native_BowMatrix_RespectsIndependentBoundaries()
    {
        string path = Environment.GetEnvironmentVariable(variable: "BOW_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "BOW_NATIVE_REPORT_REQUIRED");
        using var report = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: path));
        report.RootElement.GetProperty(propertyName: "bowSamples").GetArrayLength().ShouldBe(expected: 448);
        JsonElement boundaries = report.RootElement.GetProperty(propertyName: "bowBoundaries");
        boundaries.GetArrayLength().ShouldBe(expected: 17);
        boundaries.EnumerateArray().Any(predicate: row => row.GetProperty(propertyName: "pass").GetBoolean()
                && row.GetProperty(propertyName: "id").GetString() is "static-retires-native-sample").ShouldBeTrue();
        var keys = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement row in report.RootElement.GetProperty(propertyName: "bowSamples").EnumerateArray())
        {
            string key = row.GetProperty(propertyName: "body").GetString() + ":"
                    + row.GetProperty(propertyName: "facing").GetRawText() + ":"
                    + row.GetProperty(propertyName: "hand").GetString() + ":"
                    + row.GetProperty(propertyName: "aim").GetRawText() + ":"
                    + row.GetProperty(propertyName: "stage").GetString();
            keys.Add(item: key).ShouldBeTrue();
            JsonElement sample = row.GetProperty(propertyName: "sample");
            sample.GetProperty(propertyName: "gameplayEvents").GetInt32().ShouldBe(expected: 0);
            sample.GetProperty(propertyName: "pickupCreated").GetBoolean().ShouldBeFalse();
            sample.GetProperty(propertyName: "bowHand").GetString().ShouldBe(
                    expected: row.GetProperty(propertyName: "hand").GetString());
        }

        report.RootElement.GetProperty(propertyName: "guardSamples").GetArrayLength().ShouldBe(expected: 112);
        foreach (JsonElement row in report.RootElement.GetProperty(propertyName: "guardSamples").EnumerateArray())
        {
            row.GetProperty(propertyName: "pixels").GetBoolean().ShouldBeTrue();
            row.GetProperty(propertyName: "grip").GetBoolean().ShouldBeTrue();
            row.GetProperty(propertyName: "retiredPixelsRemoved").GetBoolean().ShouldBeTrue();
        }
    }

    /// <summary>Authored timing, events, ranges and pickup flags must remain valid before use.</summary>
    [Theory]
    [InlineData("schemaVersion", "2")]
    [InlineData("prepareFraction", "0")]
    [InlineData("prepareFraction", "0.3")]
    [InlineData("releaseFraction", "0.1")]
    [InlineData("releaseFraction", "0.8")]
    [InlineData("releaseSound", "\"AU-03\"")]
    [InlineData("impactSound", "\"AU-04\"")]
    [InlineData("clock", "\"wall-clock\"")]
    [InlineData("structuralAmmunition", "\"stack-name-sentinel\"")]
    [InlineData("fallback", "\"invented-bow\"")]
    [InlineData("recoverable", "true")]
    [InlineData("aimRangeRadians", "[-1,1]")]
    public void BowDefinition_InvalidInput_IsRejected(string field, string value)
    {
        Should.Throw<InvalidDataException>(actual: () => Create(bow: true,
                alterBow: document => document[field] = JsonNode.Parse(json: value)));
    }

    /// <summary>Aim preserves time and generations; replacement invalidates a release even after it was due.</summary>
    [Fact]
    public void AimAndRelease_LateGeneration_PreservesDetachedAuthority()
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.ConfigureBow(aimRadians: 0, leftArrows: 0, rightArrows: 30, leftUsable: true,
                rightUsable: true, stackIdentity: "a");
        subject.Advance(elapsedSeconds: 0.6);
        JsonElement before = subject.Snapshot;
        long old = before.GetProperty(propertyName: "generation").GetInt64();
        subject.ConfigureBow(aimRadians: -0.237, leftArrows: 0, rightArrows: 30, leftUsable: true,
                rightUsable: true, stackIdentity: "a");
        subject.Snapshot.GetProperty(propertyName: "generation").GetInt64().ShouldBe(expected: old);
        subject.Snapshot.GetProperty(propertyName: "actionSeconds").GetDouble().ShouldBe(expected: 0.6);
        before.GetProperty(propertyName: "aimRadians").GetDouble().ShouldBe(expected: 0);
        subject.ConfigureBow(aimRadians: -0.237, leftArrows: 0, rightArrows: 1, leftUsable: true,
                rightUsable: true, stackIdentity: "b");
        subject.TryRelease(generation: old).ShouldBeFalse();
        subject.Advance(elapsedSeconds: 0.6);
        long current = subject.Snapshot.GetProperty(propertyName: "generation").GetInt64();
        subject.TryRelease(generation: current).ShouldBeTrue();
        subject.TryRelease(generation: current).ShouldBeFalse();
        before.GetProperty(propertyName: "released").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Explicit structural ammunition survives many cycles without an inventory stack.</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void IntrinsicAmmunition_StructuralBow_SustainsConfiguredCycles(int facing)
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: "IT-07",
                rightItem: "Empty", facing: facing);
        subject.ConfigureBow(aimRadians: 0.237, leftArrows: 0, rightArrows: 0, leftUsable: false,
                rightUsable: false, stackIdentity: "actual", intrinsicAmmunition: true);
        JsonElement detached = subject.Snapshot;
        long previous = -1;
        for (int cycle = 0; cycle < 64; cycle++)
        {
            JsonElement before = subject.Snapshot;
            before.GetProperty(propertyName: "intrinsicAmmunition").GetBoolean().ShouldBeTrue();
            before.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
            before.GetProperty(propertyName: "leftArrows").GetInt32().ShouldBe(expected: 0);
            before.GetProperty(propertyName: "rightArrows").GetInt32().ShouldBe(expected: 0);
            before.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 1.2);
            before.GetProperty(propertyName: "releaseSound").GetString().ShouldBe(expected: "AU-06");
            before.GetProperty(propertyName: "impactSound").GetString().ShouldBe(expected: "AU-07");
            long generation = before.GetProperty(propertyName: "generation").GetInt64();
            generation.ShouldBeGreaterThan(expected: previous);
            subject.TryRelease(generation: previous).ShouldBeFalse();
            subject.TryRelease(generation: generation).ShouldBeFalse();
            subject.Advance(elapsedSeconds: 0.601);
            subject.TryRelease(generation: generation).ShouldBeTrue();
            subject.TryRelease(generation: generation).ShouldBeFalse();
            subject.Snapshot.GetProperty(propertyName: "spentVisualArrows").GetInt32().ShouldBe(
                    expected: cycle + 1);
            subject.Snapshot.GetProperty(propertyName: "pickupCreated").GetBoolean().ShouldBeFalse();
            subject.Snapshot.GetProperty(propertyName: "gameplayEvents").GetInt32().ShouldBe(expected: 0);
            subject.Advance(elapsedSeconds: 0.599);
            previous = generation;
        }

        detached.GetProperty(propertyName: "spentVisualArrows").GetInt32().ShouldBe(expected: 0);
        detached.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Ordinary calls, even guard-looking stack names, never grant structural authority.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(30)]
    public void OrdinaryAmmunition_FiniteStack_IsConsumed(int count)
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.ConfigureBow(aimRadians: 0, leftArrows: 0, rightArrows: count, leftUsable: true,
                rightUsable: true, stackIdentity: "structural-guard-intrinsic");
        for (int cycle = 0; cycle < count; cycle++)
        {
            subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
            subject.Snapshot.GetProperty(propertyName: "intrinsicAmmunition").GetBoolean().ShouldBeFalse();
            subject.Advance(elapsedSeconds: 0.601);
            subject.TryRelease(generation: subject.Snapshot.GetProperty(propertyName: "generation")
                    .GetInt64()).ShouldBeTrue();
            subject.Advance(elapsedSeconds: 0.599);
        }

        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "fallback").GetString().ShouldBe(expected: "melee-or-fists");
        subject.Advance(elapsedSeconds: 0.601);
        subject.TryRelease(generation: subject.Snapshot.GetProperty(propertyName: "generation")
                .GetInt64()).ShouldBeFalse();
    }

    /// <summary>Leaving structural mode revokes late releases and detached samples retain their old state.</summary>
    [Theory]
    [InlineData("ordinary")]
    [InlineData("none")]
    [InlineData("melee")]
    public void IntrinsicAmmunition_InvalidatedSelection_RevokesAuthority(string mode)
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: "bow", leftItem: "IT-07",
                rightItem: "Empty", facing: 1);
        subject.ConfigureBow(aimRadians: 0, leftArrows: 0, rightArrows: 0, leftUsable: false,
                rightUsable: false, stackIdentity: "actual", intrinsicAmmunition: true);
        subject.Advance(elapsedSeconds: 0.601);
        JsonElement detached = subject.Snapshot;
        long generation = detached.GetProperty(propertyName: "generation").GetInt64();
        if (mode is "ordinary")
        {
            subject.ConfigureBow(aimRadians: 0, leftArrows: 0, rightArrows: 0, leftUsable: false,
                    rightUsable: false, stackIdentity: "actual");
        }
        else
        {
            subject.Configure(body: "CH-01", movement: "walk", action: mode, leftItem: "IT-07",
                    rightItem: "Empty", facing: 1);
        }

        subject.TryRelease(generation: generation).ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "intrinsicAmmunition").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 0.7);
        detached.GetProperty(propertyName: "intrinsicAmmunition").GetBoolean().ShouldBeTrue();
        detached.GetProperty(propertyName: "nocked").GetBoolean().ShouldBeTrue();
    }

    /// <summary>The actual native guard completes 64 consecutive cycles in each facing and revokes authority.</summary>
    [Fact]
    public void Native_IntrinsicAmmunition_SustainsAndRevokes()
    {
        string path = Environment.GetEnvironmentVariable(variable: "BOW_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "BOW_NATIVE_REPORT_REQUIRED");
        using var report = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: path));
        JsonElement rows = report.RootElement.GetProperty(propertyName: "guardSustained");
        rows.GetArrayLength().ShouldBe(expected: 128);
        var identities = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonElement row in rows.EnumerateArray())
        {
            identities.Add(item: row.GetProperty(propertyName: "facing").GetRawText() + ":"
                    + row.GetProperty(propertyName: "cycle").GetRawText()).ShouldBeTrue();
            row.GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
            row.GetProperty(propertyName: "before").GetProperty(propertyName: "intrinsicAmmunition")
                    .GetBoolean().ShouldBeTrue();
            row.GetProperty(propertyName: "released").GetProperty(propertyName: "released")
                    .GetBoolean().ShouldBeTrue();
            row.GetProperty(propertyName: "next").GetProperty(propertyName: "nocked")
                    .GetBoolean().ShouldBeTrue();
        }

        JsonElement controls = report.RootElement.GetProperty(propertyName: "guardAuthority");
        controls.GetArrayLength().ShouldBe(expected: 3);
        foreach (JsonElement control in controls.EnumerateArray())
        {
            control.GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
            control.GetProperty(propertyName: "sample").GetProperty(propertyName: "intrinsicAmmunition")
                    .GetBoolean().ShouldBeFalse();
        }
    }

    /// <summary>Structural authority rejects inventory stacks, counts and inactive action selections.</summary>
    [Theory]
    [InlineData("bow", "IT-02", 0)]
    [InlineData("bow", "Empty", 1)]
    [InlineData("none", "Empty", 0)]
    public void IntrinsicAmmunition_InvalidSetup_RejectsInventedInventory(string action, string opposite, int count)
    {
        CharacterPresentation subject = Create(bow: true);
        subject.Configure(body: "CH-01", movement: "walk", action: action, leftItem: "IT-07",
                rightItem: opposite, facing: 1);
        Should.Throw<InvalidDataException>(actual: () => subject.ConfigureBow(aimRadians: 0,
                leftArrows: 0, rightArrows: count, leftUsable: true, rightUsable: true,
                stackIdentity: "actual", intrinsicAmmunition: true));
    }

    private static void SetArrowMaximum(JsonNode root)
    {
        JsonNode definitions = root["definitions"] ?? throw new InvalidDataException(message: "definitions");
        JsonNode arrows = definitions[1] ?? throw new InvalidDataException(message: "arrows");
        arrows["maxStack"] = 2;
    }

    private static CharacterPresentation Create(bool bow = false, Action<JsonNode>? alterItems = null,
            Action<JsonNode>? alterBow = null)
    {
        string root = Environment.GetEnvironmentVariable(variable: "PHASE1_ROOT")
                ?? throw new InvalidOperationException(message: "PHASE1_ROOT_REQUIRED");
        string content = Path.Combine(path1: root, path2: "src/godot-game/game/content");
        byte[] animation = File.ReadAllBytes(path: Path.Combine(path1: content,
                path2: "animation/locomotion-melee.json"));
        byte[] items = File.ReadAllBytes(path: Path.Combine(path1: content, path2: "config/items.json"));
        if (alterItems is not null)
        {
            JsonNode node = JsonNode.Parse(utf8Json: items) ?? throw new InvalidDataException(message: "items");
            alterItems(obj: node);
            items = JsonSerializer.SerializeToUtf8Bytes(value: node);
        }
        byte[] combat = File.ReadAllBytes(path: Path.Combine(path1: content, path2: "config/combat.json"));
        if (bow)
        {
            byte[] definition = File.ReadAllBytes(path: Path.Combine(path1: content, path2: "animation/bow.json"));
            if (alterBow is not null)
            {
                JsonNode node = JsonNode.Parse(utf8Json: definition)
                        ?? throw new InvalidDataException(message: "bow");
                alterBow(obj: node);
                definition = JsonSerializer.SerializeToUtf8Bytes(value: node);
            }

            return new CharacterPresentation(animation: animation, items: items, combat: combat,
                    bow: definition);
        }

        return new CharacterPresentation(animation: animation, items: items, combat: combat);
    }
}
