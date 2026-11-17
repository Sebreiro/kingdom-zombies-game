using System;
using System.IO;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual composed native character observations independently of presentation metadata.</summary>
public sealed class CharacterPresentationTests
{
    /// <summary>Moving lower segments must advance while the same upper strike remains active.</summary>
    [Fact]
    public void Native_StrideContinuesDuringMelee()
    {
        using JsonDocument report = Native();
        report.RootElement.GetProperty(propertyName: "strideAdvancesDuringMelee").GetBoolean().ShouldBeTrue();
    }

    private static JsonDocument Native()
    {
        string file = Environment.GetEnvironmentVariable(variable: "ANIMATION_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "ANIMATION_NATIVE_REPORT_REQUIRED");
        return JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: file));
    }
}
