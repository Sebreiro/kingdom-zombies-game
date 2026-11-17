using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises stable corpse state independently from transient event lifetime.</summary>
public sealed class AppearanceStateTests
{
    /// <summary>Explicit death fades its effect while current corpse and gear remain.</summary>
    [Fact]
    public void Death_AfterTransientExpiryPreservesCurrentEquippedCorpse()
    {
        var subject = new AppearanceState(definition: Encoding.UTF8.GetBytes(s: Definition));
        JsonElement equipment = JsonSerializer.SerializeToElement(value: new { backpack = "IT-23" });
        subject.Restore(body: "CH-01", kind: "alive", equipment: equipment, selected: true);
        subject.TriggerDeath();
        subject.Advance(elapsedSeconds: 5);
        subject.Snapshot.GetProperty(propertyName: "persistent").GetBoolean().ShouldBeTrue();
        subject.Snapshot.GetProperty(propertyName: "kind").GetString().ShouldBe(expected: "corpse");
        subject.Snapshot.GetProperty(propertyName: "equipment").GetProperty(propertyName: "backpack")
                .GetString().ShouldBe(expected: "IT-23");
        subject.Snapshot.GetProperty(propertyName: "transientVisible").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Current gear replaces all previous gear and silent restore never replays death events.</summary>
    [Fact]
    public void Restore_ReplacesGearSilentlyAndDetachesInputs()
    {
        AppearanceState subject = Actual();
        using var equipment = JsonDocument.Parse(json:
                "{\"backpack\":\"IT-24\",\"torso\":\"IT-13\",\"crown\":\"K21\"}");
        subject.Restore(body: "CH-08", kind: "alive", equipment: equipment.RootElement, selected: true);
        JsonElement before = subject.Snapshot;
        subject.TriggerDeath();
        long events = subject.Snapshot.GetProperty(propertyName: "eventCount").GetInt64();
        for (int index = 0; index < 12; index++)
        {
            subject.Restore(body: "CH-08", kind: "corpse", equipment: Gear(), selected: false);
            subject.Advance(elapsedSeconds: 60);
            subject.Snapshot.GetProperty(propertyName: "equipment").EnumerateObject().Count().ShouldBe(expected: 0);
            subject.Snapshot.GetProperty(propertyName: "eventCount").GetInt64().ShouldBe(expected: events);
            subject.Snapshot.GetProperty(propertyName: "transientVisible").GetBoolean().ShouldBeFalse();
        }

        before.GetProperty(propertyName: "equipment").GetProperty(propertyName: "crown").GetString()
                .ShouldBe(expected: "K21");
        subject.Restore(body: "CH-08", kind: "corpse", equipment: Gear(backpack: "IT-23"), selected: true);
        subject.Snapshot.GetProperty(propertyName: "equipment").GetProperty(propertyName: "backpack").GetString()
                .ShouldBe(expected: "IT-23");
        subject.TriggerDeath();
        subject.Snapshot.GetProperty(propertyName: "eventCount").GetInt64().ShouldBe(expected: events);
    }

    /// <summary>Every body uses the authored two-stage collapse then a stable equipped endpoint.</summary>
    [Theory]
    [InlineData("CH-01")]
    [InlineData("CH-02")]
    [InlineData("CH-03")]
    [InlineData("CH-04")]
    [InlineData("CH-05")]
    [InlineData("CH-06")]
    [InlineData("CH-07")]
    [InlineData("CH-08")]
    public void Death_AllBodiesPreserveGearAndRemoveCrown(string body)
    {
        AppearanceState subject = Actual();
        subject.Restore(body: body, kind: "alive", equipment: JsonSerializer.SerializeToElement(
                value: new { crown = "K21", backpack = "IT-24", torso = "IT-13", headwear = "IT-18", left = "IT-03" }),
                selected: true);
        subject.TriggerDeath();
        subject.Snapshot.GetProperty(propertyName: "pose").GetString().ShouldBe(expected: "collapse-a");
        subject.Advance(elapsedSeconds: 0.17);
        subject.Snapshot.GetProperty(propertyName: "pose").GetString().ShouldBe(expected: "collapse-b");
        subject.Advance(elapsedSeconds: 0.16);
        subject.Snapshot.GetProperty(propertyName: "pose").GetString().ShouldBe(expected: "corpse");
        subject.Advance(elapsedSeconds: 20);
        subject.Snapshot.GetProperty(propertyName: "equipment").TryGetProperty(propertyName: "crown", value: out _)
                .ShouldBeFalse();
        subject.Snapshot.GetProperty(propertyName: "equipment").EnumerateObject().Count().ShouldBe(expected: 4);
        subject.Snapshot.GetProperty(propertyName: "eventCount").GetInt64().ShouldBe(expected: 1);
        subject.Snapshot.GetProperty(propertyName: "persistent").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Empty and repopulated piles, wildlife meat and structural exclusions remain independent.</summary>
    [Fact]
    public void Endpoint_RulesAndResetFencePastEvents()
    {
        AppearanceState subject = Actual();
        subject.Restore(body: "pile", kind: "pile", equipment: Gear(), selected: true);
        subject.Snapshot.GetProperty(propertyName: "selectable").GetBoolean().ShouldBeFalse();
        subject.Restore(body: "pile", kind: "pile", equipment: Gear(backpack: "IT-23"), selected: true);
        subject.Snapshot.GetProperty(propertyName: "selectable").GetBoolean().ShouldBeTrue();
        foreach (string body in new[] { "rabbit", "deer" })
        {
            subject.Restore(body: body, kind: "wildlife-alive", equipment: JsonSerializer.SerializeToElement(
                    value: new { meat = "IT-10" }), selected: true);
            subject.TriggerDeath();
            subject.Advance(elapsedSeconds: 4);
            subject.Snapshot.GetProperty(propertyName: "persistent").GetBoolean().ShouldBeTrue();
            subject.Snapshot.GetProperty(propertyName: "equipment").GetProperty(propertyName: "meat").GetString()
                    .ShouldBe(expected: "IT-10");
            Should.Throw<InvalidDataException>(actual: () => subject.Restore(body: body, kind: "wildlife",
                    equipment: Gear(backpack: "IT-24"), selected: false));
        }

        Should.Throw<InvalidDataException>(actual: () => subject.Restore(body: "guard", kind: "structural",
                equipment: Gear(backpack: "IT-24"), selected: false));
        subject.Restore(body: "guard", kind: "structural", equipment: Gear(), selected: true);
        subject.TriggerDeath();
        subject.Advance(elapsedSeconds: 1);
        subject.Snapshot.GetProperty(propertyName: "kind").GetString().ShouldBe(expected: "empty");
        subject.Snapshot.GetProperty(propertyName: "eventCount").GetInt64().ShouldBe(expected: 0);
        subject.Snapshot.GetProperty(propertyName: "persistent").GetBoolean().ShouldBeFalse();
        subject.Reset();
        subject.Advance(elapsedSeconds: 60);
        subject.Snapshot.GetProperty(propertyName: "selected").GetBoolean().ShouldBeFalse();
    }

    /// <summary>Malformed clocks, identities and ambiguous or mismatched gear fail before state replacement.</summary>
    [Fact]
    public void InvalidInput_PreservesCurrentPublication()
    {
        AppearanceState subject = Actual();
        subject.Restore(body: "CH-01", kind: "corpse", equipment: Gear(backpack: "IT-24"), selected: true);
        string before = subject.Snapshot.GetRawText();
        foreach (double delta in new[] { double.NaN, double.PositiveInfinity, -0.1, 61 })
        {
            Should.Throw<InvalidDataException>(actual: () => subject.Advance(elapsedSeconds: delta));
        }

        foreach (string invalid in new[] { "{\"backpack\":\"IT-23\",\"backpack\":\"IT-24\"}",
                "{\"headwear\":\"IT-24\"}", "{\"Backpack\":\"IT-24\"}", "{\"backpack\":2}" })
        {
            using var gear = JsonDocument.Parse(json: invalid);
            Should.Throw<InvalidDataException>(actual: () => subject.Restore(body: "CH-01", kind: "corpse",
                    equipment: gear.RootElement, selected: true));
        }

        Should.Throw<InvalidDataException>(actual: () => subject.Restore(body: "ch-01", kind: "corpse",
                equipment: Gear(), selected: false));
        subject.Snapshot.GetRawText().ShouldBe(expected: before);
        Should.Throw<InvalidDataException>(actual: () => new AppearanceState(definition: []));
        Should.Throw<InvalidDataException>(actual: () => new AppearanceState(definition: new byte[1048577]));
        Should.Throw<InvalidDataException>(actual: () => new AppearanceState(definition:
                Encoding.UTF8.GetBytes(s: Definition.Replace(oldValue: "0.8", newValue: "0.1",
                        comparisonType: StringComparison.Ordinal))));
    }

    /// <summary>Only the exact new identity is public; compiler-valid neighboring declarations remain denied.</summary>
    [Fact]
    public void Visibility_ExactIdentityAndCompilerValidNegativeControls()
    {
        const string Identity = "namespace KingdomZombies.Core; public sealed class AppearanceState { }";
        string registry = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "docs/code/visibility-exemptions.json"));
        string temporary = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/appearance-" + Guid.NewGuid()
                .ToString(format: "N"));
        string directory = Path.Combine(path1: temporary, path2: "docs/code");
        _ = Directory.CreateDirectory(path: directory);
        string file = Path.Combine(path1: directory, path2: "visibility-exemptions.json");
        File.WriteAllText(path: file, contents: registry);
        try
        {
            var options = new CSharpParseOptions(languageVersion: LanguageVersion.CSharp14);
            Compile(source: Identity);
            ArchitecturePolicy.Visibility(root: temporary, source: Identity, path: "src/core/AppearanceState.cs",
                    options: options).ShouldBeEmpty();
            ArchitecturePolicy.Visibility(root: temporary, source: Identity, path: "src/core/appearanceState.cs",
                    options: options).Count.ShouldBe(expected: 1);
            const string Adjacent = Identity + " public sealed class Adjacent { }";
            Compile(source: Adjacent);
            ArchitecturePolicy.Visibility(root: temporary, source: Adjacent, path: "src/core/AppearanceState.cs",
                    options: options).Count.ShouldBe(expected: 1);
            JsonObject expanded = (JsonNode.Parse(json: registry) ?? throw new InvalidDataException()).AsObject();
            JsonArray rows = (expanded["permissions"] ?? throw new InvalidDataException()).AsArray();
            rows.Count.ShouldBe(expected: 19);
            rows.Add(item: new JsonObject
            {
                ["path"] = "src/core/AppearanceState.cs",
                ["type"] = "KingdomZombies.Core.Adjacent",
                ["reason"] = "unapproved adjacent identity"
            });
            File.WriteAllText(path: file, contents: expanded.ToJsonString());
            ArchitecturePolicy.Visibility(root: temporary, source: Adjacent, path: "src/core/AppearanceState.cs",
                    options: options).Single().ShouldBe(
                            expected: "R-34: invalid or expanded exact visibility registry");
        }
        finally
        {
            Directory.Delete(path: temporary, recursive: true);
        }
    }

    private const string Definition = "{\"schemaVersion\":1,\"id\":\"corpses-wildlife\",\"version\":1,"
            + "\"bodies\":[\"CH-01\",\"CH-02\",\"CH-03\",\"CH-04\",\"CH-05\",\"CH-06\",\"CH-07\",\"CH-08\"],"
            + "\"wildlife\":[\"rabbit\",\"deer\"],\"slots\":{\"backpack\":[\"IT-23\",\"IT-24\"]},"
            + "\"collapseSeconds\":0.32,\"effectSeconds\":0.8}";

    private static AppearanceState Actual()
    {
        return new AppearanceState(definition: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/content/packs/corpses-wildlife.json")));
    }

    private static JsonElement Gear(string? backpack = null)
    {
        return backpack is null ? JsonSerializer.SerializeToElement(value: new { })
                : JsonSerializer.SerializeToElement(value: new { backpack });
    }

    private static void Compile(string source)
    {
        var compilation = CSharpCompilation.Create(assemblyName: "AppearanceVisibility",
                syntaxTrees: [CSharpSyntaxTree.ParseText(text: source)], references: [MetadataReference.CreateFromFile(
                        path: typeof(object).Assembly.Location)], options: new CSharpCompilationOptions(
                                outputKind: OutputKind.DynamicallyLinkedLibrary));
        using var stream = new MemoryStream();
        compilation.Emit(peStream: stream).Success.ShouldBeTrue();
    }

}
