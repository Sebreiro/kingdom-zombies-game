using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual composed native character observations independently of presentation metadata.</summary>
public sealed class CharacterPresentationTests
{
    /// <summary>A malformed selected character retains its exact error while the real hub stays usable.</summary>
    [Fact]
    public void Native_InvalidDefinition_KeepsVisibleErrorAndTransport()
    {
        using JsonDocument report = Native();
        JsonElement observed = report.RootElement.GetProperty(propertyName: "replacementError");
        observed.GetProperty(propertyName: "errorVisible").GetBoolean().ShouldBeTrue();
        observed.GetProperty(propertyName: "transportUsable").GetBoolean().ShouldBeTrue();
        observed.GetProperty(propertyName: "recovered").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Both canonical cases execute actual replacement, lifecycle, isolation and transport controls.</summary>
    [Fact]
    public void Native_ReplacementControls_HaveIndependentObservations()
    {
        using JsonDocument report = Native();
        JsonElement checks = report.RootElement.GetProperty(propertyName: "replacementChecks");
        foreach (string id in new[] { "H15-order-facing-gear", "H15-order-gear-facing",
            "H15-immediate-facing-phase", "H18-retired-Body", "H18-retired-left", "H18-retired-right",
            "H18-retired-armor", "H18-retired-Movement", "H18-retired-Action", "shared-neighbour-isolation",
            "transport-single-step", "transport-paused", "transport-speed-0.25×", "transport-speed-0.5×",
            "transport-speed-1×", "transport-speed-2×", "transport-reset", "detached-late-callback-contact",
            "actual-root-recheck-recovery", "partial-restored" })
        {
            checks.EnumerateArray().Single(predicate: check => StringComparer.Ordinal.Equals(
                    x: check.GetProperty(propertyName: "id").GetString(), y: id))
                    .GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
        }
    }

    /// <summary>Seven independent native character surfaces retain distinct setup and actual observations.</summary>
    [Fact]
    public void Native_CharacterStates_AreCompleteAndDistinct()
    {
        using JsonDocument report = Native();
        JsonElement states = report.RootElement.GetProperty(propertyName: "states");
        foreach (string id in new[] { "empty", "loading", "error", "populated", "partial", "overflow", "long-text" })
        {
            JsonElement state = states.EnumerateArray().Single(predicate: row => StringComparer.Ordinal.Equals(
                    x: row.GetProperty(propertyName: "id").GetString(), y: id));
            state.GetProperty(propertyName: "pass").GetBoolean().ShouldBeTrue();
            state.GetProperty(propertyName: "setup").GetString().ShouldNotBeNullOrWhiteSpace();
        }
    }

    /// <summary>Moving lower segments must advance while the same upper strike remains active.</summary>
    [Fact]
    public void Native_Melee_KeepsStride()
    {
        using JsonDocument report = Native();
        report.RootElement.GetProperty(propertyName: "strideAdvancesDuringMelee").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Every ordinary body composes all accepted melee intervals with continuous stride.</summary>
    [Theory]
    [InlineData("CH-01")]
    [InlineData("CH-02")]
    [InlineData("CH-03")]
    [InlineData("CH-04")]
    [InlineData("CH-05")]
    [InlineData("CH-06")]
    [InlineData("CH-07")]
    [InlineData("CH-08")]
    public void Stride_AllBodiesAndWeapons_Continues(string body)
    {
        foreach ((string item, double interval) in new[] { ("Empty", 0.7), ("IT-03", 0.5), ("IT-04", 0.9),
            ("IT-05", 0.75), ("IT-06", 1.1) })
        {
            foreach (int facing in new[] { -1, 1 })
            {
                CharacterPresentation subject = Create();
                subject.Configure(body: body, movement: "walk", action: "melee", leftItem: item,
                        rightItem: "Empty", facing: facing);
                JsonElement before = subject.Snapshot;
                subject.Advance(elapsedSeconds: 0.13);
                JsonElement after = subject.Snapshot;
                after.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: interval);
                StringComparer.Ordinal.Equals(x: after.GetProperty(propertyName: "lowerPose").GetString(),
                        y: before.GetProperty(propertyName: "lowerPose").GetString()).ShouldBeFalse();
                after.GetProperty(propertyName: "upperPose").GetString().ShouldBe(expected: "windup");
                after.GetProperty(propertyName: "leftItem").GetString().ShouldBe(expected: item);
                after.GetProperty(propertyName: "facing").GetInt32().ShouldBe(expected: facing);
            }
        }
    }

    /// <summary>Mixed actions use the slower interval and one contact per generation.</summary>
    [Fact]
    public void MixedPair_TwoWeaponTimings_UsesSlowerIntervalAndSingleContact()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-04", movement: "faster", action: "melee", leftItem: "IT-03",
                rightItem: "IT-04", facing: -1);
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 0.9);
        long generation = subject.Snapshot.GetProperty(propertyName: "generation").GetInt64();
        subject.TryContact(generation: generation).ShouldBeFalse();
        subject.Advance(elapsedSeconds: 0.30);
        subject.TryContact(generation: generation).ShouldBeTrue();
        subject.TryContact(generation: generation).ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "gameplayEvents").GetInt32().ShouldBe(expected: 0);
        subject.Advance(elapsedSeconds: 0.9);
        subject.TryContact(generation: generation).ShouldBeFalse();
    }

    /// <summary>Flinch never delays movement or accepted attack timing, and its small offset expires.</summary>
    [Fact]
    public void Flinch_ActivePresentation_LeavesClocksAndGenerationUnchanged()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-08", movement: "walk", action: "melee", leftItem: "IT-06",
                rightItem: "IT-03", facing: 1);
        subject.Advance(elapsedSeconds: 0.1);
        JsonElement before = subject.Snapshot;
        subject.Flinch();
        foreach (string field in new[] { "strideSeconds", "actionSeconds", "intervalSeconds", "generation" })
        {
            subject.Snapshot.GetProperty(propertyName: field).GetRawText().ShouldBe(expected:
                    before.GetProperty(propertyName: field).GetRawText());
        }

        subject.Snapshot.GetProperty(propertyName: "flinchOffset").GetDouble().ShouldBe(expected: -1);
        subject.Advance(elapsedSeconds: 0.13);
        subject.Snapshot.GetProperty(propertyName: "flinchOffset").GetDouble().ShouldBe(expected: 0);
    }

    /// <summary>The survivor has four distinct authored takeoff, up, down and landing samples.</summary>
    [Fact]
    public void Jump_AuthoredTrack_ExposesAllFourRegisteredStages()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-01", movement: "jump", action: "melee", leftItem: "IT-05",
                rightItem: "Empty", facing: 1);
        foreach (string stage in new[] { "takeoff", "airborne-up", "airborne-down", "landing" })
        {
            JsonElement sample = subject.Snapshot;
            sample.GetProperty(propertyName: "stage").GetString().ShouldBe(expected: stage);
            double elapsed = sample.GetProperty(propertyName: "nextSampleSeconds").GetDouble();
            while (elapsed < 0.0001)
            {
                subject.Advance(elapsedSeconds: 0.0001);
                elapsed = subject.Snapshot.GetProperty(propertyName: "nextSampleSeconds").GetDouble();
            }

            if (stage is not "landing")
            {
                double duration = stage is "takeoff" ? 0.081 : 0.16;
                subject.Advance(elapsedSeconds: duration);
            }
        }
    }

    /// <summary>Invalid elapsed values cannot introduce hidden clocks or poison the current sample.</summary>
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(-0.001)]
    [InlineData(60.001)]
    public void Elapsed_InvalidValues_AreRejected(double elapsed)
    {
        CharacterPresentation subject = Create();
        string before = subject.Snapshot.GetRawText();
        Should.Throw<ArgumentOutOfRangeException>(actual: () => subject.Advance(elapsedSeconds: elapsed));
        subject.Snapshot.GetRawText().ShouldBe(expected: before);
    }

    /// <summary>The exact compiler-valid public identity never authorizes adjacent types.</summary>
    [Theory]
    [InlineData("src/godot-game/core/CharacterPresentation.cs", "CharacterPresentation", true)]
    [InlineData("src/godot-game/core/Neighbor.cs", "CharacterPresentation", false)]
    [InlineData("src/godot-game/core/CharacterPresentation.cs", "CharacterPresentationNeighbor", false)]
    public void PublicBoundary_ExpectedDeclaration_PreservesExactIdentity(string path, string type, bool allowed)
    {
        string source = "namespace KingdomZombies.Core; public sealed class " + type + " {}";
        var options = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp14);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(text: source, options: options,
                cancellationToken: TestContext.Current.CancellationToken);
        var compilation = CSharpCompilation.Create(assemblyName: "VisibilityProbe",
                syntaxTrees: [tree], references: [MetadataReference.CreateFromFile(path: typeof(object).Assembly
                        .Location)], options: new CSharpCompilationOptions(
                                outputKind: OutputKind.DynamicallyLinkedLibrary));
        compilation.GetDiagnostics(cancellationToken: TestContext.Current.CancellationToken)
                .Where(predicate: diagnostic => diagnostic.Severity is DiagnosticSeverity.Error)
                .ShouldBeEmpty();
        ArchitecturePolicy.Visibility(root: Root(), source: source, path: path, options: options).Count
                .ShouldBe(expected: allowed ? 0 : 1);
        typeof(CharacterPresentation).IsPublic.ShouldBeTrue();
        typeof(CharacterPresentation).IsSealed.ShouldBeTrue();
    }

    /// <summary>Expanding the independent registry is rejected even with a compiler-valid new type.</summary>
    [Fact]
    public void PublicBoundary_ExpandedRegistry_IsRejected()
    {
        string directory = Path.Combine(path1: Root(), path2: ".phase1/temp/animation-visibility-"
                + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: Path.Combine(path1: directory, path2: "docs/code"));
        JsonNode registry = JsonNode.Parse(json: File.ReadAllText(path: Path.Combine(path1: Root(),
                path2: "docs/code/visibility-exemptions.json"))) ?? throw new InvalidDataException();
        (registry["permissions"] ?? throw new InvalidDataException()).AsArray().Add(item: new JsonObject
        {
            ["path"] = "src/godot-game/core/Neighbor.cs",
            ["type"] = "KingdomZombies.Core.Neighbor",
            ["reason"] = "Not authorized"
        });
        File.WriteAllText(path: Path.Combine(path1: directory, path2: "docs/code/visibility-exemptions.json"),
                contents: registry.ToJsonString());
        ArchitecturePolicy.Visibility(root: directory, source: "namespace KingdomZombies.Core;"
                + " public sealed class Neighbor {}", path: "src/godot-game/core/Neighbor.cs",
                options: new CSharpParseOptions(languageVersion: LanguageVersion.CSharp14))
                .Any(predicate: value => StringComparer.Ordinal.Equals(x: value,
                        y: "R-34: invalid or expanded exact visibility registry")).ShouldBeTrue();
    }

    /// <summary>Malformed authored data fails independently before producing a presentation sample.</summary>
    [Theory]
    [InlineData("body")]
    [InlineData("clip")]
    [InlineData("pose")]
    [InlineData("offset")]
    [InlineData("duration")]
    [InlineData("angle")]
    [InlineData("profile")]
    [InlineData("contact")]
    [InlineData("clock")]
    [InlineData("events")]
    public void Definitions_IndependentMalformedTracks_AreRejected(string defect)
    {
        JsonNode animation = JsonNode.Parse(utf8Json: Data(file: "animation/locomotion-melee.json"))
                ?? throw new InvalidDataException();
        JsonObject clips = (animation["clips"] ?? throw new InvalidDataException()).AsObject();
        JsonNode sample = clips["melee"]?[0] ?? throw new InvalidDataException();
        switch (defect)
        {
            case "body": animation["bodies"]?[0] = "CH-99"; break;
            case "clip": clips["extra"] = new JsonArray(); break;
            case "pose": sample["pose"] = "not-registered"; break;
            case "offset": sample["offset"]?[0] = 9; break;
            case "duration": sample["duration"] = 0; break;
            case "angle": sample["angles"]?["knife"] = 4; break;
            case "profile": sample["angles"]?["extra"] = 0; break;
            case "contact": animation["timing"]?["contactFraction"] = -1; break;
            case "clock": animation["timing"]?["clock"] = "wall-clock"; break;
            case "events": animation["timing"]?["gameplayEvents"] = 1; break;
            default: throw new InvalidDataException();
        }

        Should.Throw<InvalidDataException>(actual: () => new CharacterPresentation(
                animation: JsonSerializer.SerializeToUtf8Bytes(value: animation),
                items: Data(file: "config/items.json"), combat: Data(file: "config/combat.json")));
    }

    /// <summary>Timing, contact and sound bindings come from validated definitions, including legal edits.</summary>
    [Fact]
    public void Definitions_ConfiguredTracks_UseTimingAndEvents()
    {
        JsonNode animation = JsonNode.Parse(utf8Json: Data(file: "animation/locomotion-melee.json"))
                ?? throw new InvalidDataException();
        animation["timing"]?["contactFraction"] = 0.25;
        animation["clips"]?["melee"]?[0]?["duration"] = 0.25;
        animation["clips"]?["melee"]?[3]?["duration"] = 0.35;
        animation["timing"]?["flinchSeconds"] = 0.2;
        JsonNode combat = JsonNode.Parse(utf8Json: Data(file: "config/combat.json"))
                ?? throw new InvalidDataException();
        JsonNode club = (combat["attacks"] ?? throw new InvalidDataException()).AsArray()
                .Single(predicate: value => value?["itemRef"]?.GetValue<string>() is "IT-04")
                ?? throw new InvalidDataException();
        club["intervalSeconds"] = 1.25;
        club["releaseSound"] = "AU-04";
        var subject = new CharacterPresentation(animation: JsonSerializer.SerializeToUtf8Bytes(value: animation),
                items: Data(file: "config/items.json"), combat: JsonSerializer.SerializeToUtf8Bytes(value: combat));
        subject.Configure(body: "CH-08", movement: "walk", action: "melee", leftItem: "IT-03",
                rightItem: "IT-04", facing: 1);
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 1.25);
        subject.Snapshot.GetProperty(propertyName: "releaseSound").GetString().ShouldBe(expected: "AU-04");
        subject.Advance(elapsedSeconds: 0.3);
        long generation = subject.Snapshot.GetProperty(propertyName: "generation").GetInt64();
        subject.TryContact(generation: generation).ShouldBeFalse();
        subject.Advance(elapsedSeconds: 0.02);
        subject.TryContact(generation: generation).ShouldBeTrue();
        subject.Flinch();
        subject.Advance(elapsedSeconds: 0.13);
        subject.Snapshot.GetProperty(propertyName: "flinchOffset").GetDouble().ShouldBe(expected: -1);
        subject.Advance(elapsedSeconds: 0.08);
        subject.Snapshot.GetProperty(propertyName: "flinchOffset").GetDouble().ShouldBe(expected: 0);
    }

    /// <summary>All native body, direction, hand and stage combinations have actual independent observations.</summary>
    [Fact]
    public void Native_MeleeMatrix_ExposesEveryCombination()
    {
        using JsonDocument report = Native();
        JsonElement samples = report.RootElement.GetProperty(propertyName: "samples");
        samples.GetArrayLength().ShouldBe(expected: 704);
        foreach (string body in Enumerable.Range(start: 1, count: 8).Select(selector: value => "CH-"
                + value.ToString(format: "00", provider: System.Globalization.CultureInfo.InvariantCulture)))
        {
            samples.EnumerateArray().Count(predicate: row => StringComparer.Ordinal.Equals(
                    x: row.GetProperty(propertyName: "body").GetString(), y: body)).ShouldBe(expected: 88);
        }
    }

    private static byte[] Data(string file)
    {
        return File.ReadAllBytes(path: Path.Combine(path1: Root(), path2: "src/godot-game/game/content", path3: file));
    }

    private static string Root()
    {
        return Environment.GetEnvironmentVariable(variable: "PHASE1_ROOT")
                ?? throw new InvalidOperationException(message: "PHASE1_ROOT_REQUIRED");
    }

    private static CharacterPresentation Create()
    {
        string game = Path.Combine(path1: Root(), path2: "src/godot-game/game/content");
        return new CharacterPresentation(animation: File.ReadAllBytes(path: Path.Combine(path1: game,
                path2: "animation/locomotion-melee.json")), items: File.ReadAllBytes(path: Path.Combine(path1: game,
                        path2: "config/items.json")), combat: File.ReadAllBytes(path: Path.Combine(path1: game,
                                path2: "config/combat.json")));
    }

    private static JsonDocument Native()
    {
        string file = Environment.GetEnvironmentVariable(variable: "ANIMATION_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "ANIMATION_NATIVE_REPORT_REQUIRED");
        return JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: file));
    }
}
