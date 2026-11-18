using System;
using System.IO;
using System.Reflection;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves compiled audio ownership independently of eventual listening judgments.</summary>
public sealed class AudioPresentationTests
{
    /// <summary>Replacement owners cannot revive an earlier owner with the same local state.</summary>
    [Fact]
    public void ReplacementOwners_ReceiveStrictlyIncreasingEpochs()
    {
        long first = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        long second = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        second.ShouldBeGreaterThan(expected: first);
    }

    private static readonly Type _subject = Subject();

    private static object? Call(string name, object[] arguments)
    {
        MethodInfo method = _subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException(message: name);
        return method.Invoke(obj: null, parameters: arguments);
    }

    private static Type Subject()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        return assembly.GetType(name: "KingdomZombies.Inspection.AudioPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
