using System;
using System.IO;
using System.Reflection;
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

    private static Type Subject()
    {
        string path = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll");
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: path));
        return assembly.GetType(name: "KingdomZombies.Inspection.BuildingPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
