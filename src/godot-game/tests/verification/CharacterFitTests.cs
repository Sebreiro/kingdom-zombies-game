using System;
using System.IO;
using System.Reflection;
using System.Text;
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

    /// <summary>The independent native client checks every legal item on every body, pose and facing.</summary>
    [Fact]
    public void Native_Matrix_PreservesEveryFittedItem()
    {
        using JsonDocument report = Native();
        JsonElement matrix = report.RootElement.GetProperty(propertyName: "matrix");
        matrix.GetProperty(propertyName: "tuples").GetInt32().ShouldBe(expected: 2976);
        matrix.GetProperty(propertyName: "failed").GetInt32().ShouldBe(expected: 0);
        report.RootElement.GetProperty(propertyName: "logicalHandsPreserved").GetBoolean().ShouldBeTrue();
        report.RootElement.GetProperty(propertyName: "replacementRetired").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Independent overlap and nearby-body fixtures retain identity and actual opaque layers.</summary>
    [Fact]
    public void Native_H13_OverlapAndNearbyBodiesRemainIndependent()
    {
        using JsonDocument report = Native();
        report.RootElement.GetProperty(propertyName: "h13").GetProperty(propertyName: "passed")
                .GetInt32().ShouldBe(expected: 12);
        report.RootElement.GetProperty(propertyName: "workerRejected").GetBoolean().ShouldBeTrue();
        report.RootElement.GetProperty(propertyName: "detachedRejected").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Unsupported tuples report every exact identity without counting a fallback as fitted.</summary>
    [Theory]
    [InlineData("CH-99", "IT-13", "idle", 1)]
    [InlineData("CH-01", "IT-99", "idle", 1)]
    [InlineData("CH-01", "IT-13", "absent", -1)]
    [InlineData("CH-02", "IT-20", "corpse", 0)]
    public void Unfitted_Tuple_HasExactDiagnostic(string body, string item, string pose, int facing)
    {
        Type subject = Subject();
        using var fits = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/content/animation/fits-a.json")));
        MethodInfo method = subject.GetMethod(name: "FitItem", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException();
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () => method.Invoke(
                obj: null, parameters: [fits.RootElement, body, item, pose, facing]));
        error.InnerException.ShouldBeOfType<InvalidDataException>().Message.ShouldBe(expected:
                $"FIT_INCOMPLETE body={body} item={item} pose={pose} facing={facing}");
    }

    /// <summary>Bounded detached definitions reject ambiguous identities before native resource work.</summary>
    [Theory]
    [InlineData("{\"id\":\"a\",\"id\":\"b\"}", "CHARACTER_DUPLICATE_KEY")]
    [InlineData("{\"schemaVersion\":1,\"version\":1,\"id\":\"other\"}", "CHARACTER_DEFINITION_ID")]
    [InlineData("", "CHARACTER_DEFINITION_SIZE")]
    public void Definition_AmbiguousInput_IsRejected(string json, string diagnostic)
    {
        MethodInfo method = Subject().GetMethod(name: "ReadDefinition", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException();
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () => method.Invoke(
                obj: null, parameters: [Encoding.UTF8.GetBytes(s: json), "fits-a"]));
        error.InnerException.ShouldBeOfType<InvalidDataException>().Message.ShouldBe(expected: diagnostic);
    }

    private static JsonDocument Native()
    {
        string file = Environment.GetEnvironmentVariable(variable: "CHARACTER_NATIVE_REPORT")
                ?? throw new InvalidOperationException(message: "CHARACTER_NATIVE_REPORT_REQUIRED");
        return JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: file));
    }

    private static Type Subject()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        return assembly.GetType(name: "KingdomZombies.Inspection.CharacterPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
