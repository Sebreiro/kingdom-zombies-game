using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks actual compiled main-building sample projection without a native game window.</summary>
public sealed class BuildingPresentationTests
{
    /// <summary>A paid pending upgrade must retain the completed main tier and all of its functions.</summary>
    [Fact]
    public void Pending_Upgrade_RetainsCompletedTier()
    {
        Type subject = Subject();
        MethodInfo method = subject.GetMethod(name: "FunctionalTier", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException();
        object? actual = method.Invoke(obj: null, parameters: [2, 3, 0.5]);
        actual.ShouldBe(expected: 2);
    }

    /// <summary>Completed work changes capacity and attachments together; intermediate work changes neither.</summary>
    [Theory]
    [InlineData(1, 2, 0.0, 8, 0)]
    [InlineData(1, 2, 0.999, 8, 0)]
    [InlineData(1, 2, 1.0, 12, 0)]
    [InlineData(2, 3, 0.0, 12, 0)]
    [InlineData(2, 3, 0.5, 12, 0)]
    [InlineData(2, 3, 0.999, 12, 0)]
    [InlineData(2, 3, 1.0, 16, 2)]
    [InlineData(3, 3, 0.0, 16, 2)]
    public void Work_ProjectsCapacityAndGuardsTogether(int completed, int requested, double progress,
            int capacity, int guards)
    {
        Type subject = Subject();
        subject.GetMethod(name: "FunctionalCapacity", bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(obj: null, parameters: [completed, requested, progress]).ShouldBe(expected: capacity);
        subject.GetMethod(name: "FunctionalGuards", bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?.Invoke(obj: null, parameters: [completed, requested, progress]).ShouldBe(expected: guards);
    }

    /// <summary>Invalid and nonfinite paid-work states cannot become a functional presentation.</summary>
    [Theory]
    [InlineData(0, 1, 0.5)]
    [InlineData(1, 3, 0.5)]
    [InlineData(3, 2, 0.5)]
    [InlineData(2, 3, -0.1)]
    [InlineData(2, 3, 1.1)]
    [InlineData(2, 3, double.NaN)]
    [InlineData(2, 3, double.PositiveInfinity)]
    public void Work_RejectsInvalidState(int completed, int requested, double progress)
    {
        MethodInfo method = Subject().GetMethod(name: "FunctionalTier", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException();
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () =>
                method.Invoke(obj: null, parameters: [completed, requested, progress]));
        error.InnerException.ShouldBeOfType<ArgumentOutOfRangeException>();
    }

    /// <summary>Definitions are detached from parser lifetime and reject duplicate keys and wrong identities.</summary>
    [Fact]
    public void Definition_DetachesAndRejectsAmbiguousInput()
    {
        MethodInfo method = Subject().GetMethod(name: "ReadDefinition", bindingAttr:
                BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException();
        var actual = (JsonElement)(method.Invoke(obj: null,
                parameters: ["{\"schemaVersion\":1,\"version\":1,\"id\":\"sample\"}", "sample"])
                ?? throw new InvalidOperationException());
        actual.GetProperty(propertyName: "id").GetString().ShouldBe(expected: "sample");
        TargetInvocationException duplicate = Should.Throw<TargetInvocationException>(actual: () =>
                method.Invoke(obj: null, parameters: ["{\"id\":\"a\",\"id\":\"b\"}", "a"]));
        duplicate.InnerException?.Message.ShouldBe(expected: "BUILDING_DUPLICATE_KEY");
        TargetInvocationException wrong = Should.Throw<TargetInvocationException>(actual: () =>
                method.Invoke(obj: null, parameters: ["{\"schemaVersion\":1,\"version\":1,\"id\":\"a\"}", "b"]));
        wrong.InnerException?.Message.ShouldBe(expected: "BUILDING_DEFINITION_ID");
    }

    private static Type Subject()
    {
        string path = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll");
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: path));
        return assembly.GetType(name: "KingdomZombies.Inspection.BuildingPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
