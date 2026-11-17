using System;
using System.IO;
using System.Text.Json;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual ammunition, bow timing and detached composed samples.</summary>
public sealed class CharacterBowTests
{
    /// <summary>Explicit melee preserves its fist fallback when a bow is selected.</summary>
    [Fact]
    public void Melee_WithBowPreservesFistFallback()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-01", movement: "walk", action: "melee", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 0.7);
        subject.Snapshot.GetProperty(propertyName: "releaseSound").GetString().ShouldBe(expected: "AU-03");
    }

    /// <summary>The actual live usable bow and arrow preview offers its required bow action.</summary>
    [Fact]
    public void Native_UsableBowOffersBowAction()
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

    private static CharacterPresentation Create()
    {
        string root = Environment.GetEnvironmentVariable(variable: "PHASE1_ROOT")
                ?? throw new InvalidOperationException(message: "PHASE1_ROOT_REQUIRED");
        string content = Path.Combine(path1: root, path2: "src/godot-game/content");
        return new CharacterPresentation(animation: File.ReadAllBytes(path: Path.Combine(path1: content,
                path2: "animation/locomotion-melee.json")), items: File.ReadAllBytes(path: Path.Combine(path1: content,
                        path2: "config/items.json")), combat: File.ReadAllBytes(path: Path.Combine(path1: content,
                                path2: "config/combat.json")));
    }
}
