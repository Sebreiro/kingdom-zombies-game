using System;
using System.IO;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual ammunition, bow timing and detached composed samples.</summary>
public sealed class CharacterBowTests
{
    /// <summary>A usable bow with its actual opposite arrow hand uses the accepted bow interval.</summary>
    [Fact]
    public void Bow_WithOppositeArrowHandUsesBowInterval()
    {
        CharacterPresentation subject = Create();
        subject.Configure(body: "CH-01", movement: "walk", action: "melee", leftItem: "IT-07",
                rightItem: "IT-02", facing: 1);
        subject.Snapshot.GetProperty(propertyName: "intervalSeconds").GetDouble().ShouldBe(expected: 1.2);
        subject.Snapshot.GetProperty(propertyName: "releaseSound").GetString().ShouldBe(expected: "AU-06");
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
