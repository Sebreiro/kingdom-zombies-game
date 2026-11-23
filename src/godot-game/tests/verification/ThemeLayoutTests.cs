using System.IO;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks the real inspection scene's reusable packaged theme binding.</summary>
public sealed class ThemeLayoutTests
{
    /// <summary>The existing hub must use the packaged reusable theme.</summary>
    [Fact]
    public void Scene_PackagedTheme_IsApplied()
    {
        string scene = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/inspection/Inspection.tscn"));
        scene.ShouldContain(expected: "path=\"res://assets/ui/InspectionTheme.tres\"");
        scene.ShouldContain(expected: "theme = ExtResource(\"3_theme\")");
    }

    /// <summary>Both text roles declare explicit packaged fallback and bounded Unicode coverage.</summary>
    [Fact]
    public void Fonts_PackagedFontSet_DeclaresFallbackAndMissingDiagnostic()
    {
        using JsonDocument pack = ReadPack();
        JsonElement fonts = pack.RootElement.GetProperty(propertyName: "fonts");
        fonts.GetArrayLength().ShouldBe(expected: 3);
        fonts[0].GetProperty(propertyName: "fallbacks")[0].GetString()
                .ShouldBe(expected: "assets/ui/Fallback.ttf");
        fonts[1].GetProperty(propertyName: "fallbacks")[0].GetString()
                .ShouldBe(expected: "assets/ui/Fallback.ttf");
        fonts[2].GetProperty(propertyName: "fallbacks").GetArrayLength().ShouldBe(expected: 0);
        pack.RootElement.GetProperty(propertyName: "fontPolicy").GetProperty(propertyName: "unsupported")[0]
                .GetInt32().ShouldBe(expected: 0x10ffff);
    }

    /// <summary>Authored scales remain UI values and never change the registered world sprite scale.</summary>
    [Fact]
    public void Scales_UiScale_KeepsWorldZoomIndependent()
    {
        using JsonDocument pack = ReadPack();
        JsonElement scales = pack.RootElement.GetProperty(propertyName: "scales");
        scales.GetArrayLength().ShouldBe(expected: 3);
        scales[0].GetDouble().ShouldBe(expected: 0.8);
        scales[1].GetDouble().ShouldBe(expected: 1.0);
        scales[2].GetDouble().ShouldBe(expected: 1.5);
        string scene = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/inspection/Inspection.tscn"));
        scene.ShouldContain(expected: "metadata/scale_policy = \"UI canvas once; world zoom independent\"");
        scene.ShouldContain(expected: "offset = Vector2(-6, -9)");
    }

    private static JsonDocument ReadPack()
    {
        return JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/content/packs/ui-kit.json")));
    }
}
