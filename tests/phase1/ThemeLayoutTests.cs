using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks the real inspection scene's reusable packaged theme binding.</summary>
public sealed class ThemeLayoutTests
{
    /// <summary>The existing hub must use the packaged reusable theme.</summary>
    [Fact]
    public void Scene_AppliesPackagedTheme()
    {
        string scene = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/inspection/Inspection.tscn"));
        scene.ShouldContain(expected: "path=\"res://assets/ui/InspectionTheme.tres\"");
        scene.ShouldContain(expected: "theme = ExtResource(\"3_theme\")");
    }
}
