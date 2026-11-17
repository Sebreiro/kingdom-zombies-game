using System;
using System.IO;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Asserts observations of the actual compiled native character renderer and hub controls.</summary>
public sealed class CharacterFitTests
{
    /// <summary>Removing armor must expose the coat selected through the real character controls.</summary>
    [Fact]
    public void Selected_Coat_ReturnsAfterArmorRemoval()
    {
        string file = Environment.GetEnvironmentVariable(variable: "CHARACTER_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "CHARACTER_NATIVE_REPORT_REQUIRED");
        using var report = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: file));
        report.RootElement.GetProperty(propertyName: "coatAfterArmorRemoval").GetString()
                .ShouldBe(expected: "IT-13");
    }
}
