using System;
using System.IO;
using System.Text.Json;
using KingdomZombies.Foundation;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks the actual producer's atomic publication boundary before an acknowledgment can exist.</summary>
internal static class FoundationCapturePublicationTests
{
    internal static void Check()
    {
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase3/controls/" + Guid.NewGuid()
                .ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        string file = Path.Combine(path1: directory, path2: "checkpoint.json");
        const string Record = "{\"complete\":true}";
        FoundationCapturePublication.Publish(file: file, record: Record, beforeMove: temporary =>
        {
            File.Exists(path: file).ShouldBeFalse();
            File.ReadAllText(path: temporary).ShouldBe(expected: Record);
            using var sample = JsonDocument.Parse(json: File.ReadAllText(path: temporary));
            sample.RootElement.GetProperty(propertyName: "complete").GetBoolean().ShouldBeTrue();
        });
        File.ReadAllText(path: file).ShouldBe(expected: Record);
        Should.Throw<IOException>(actual: () => FoundationCapturePublication.Publish(file: file, record: "{}"));
        File.ReadAllText(path: file).ShouldBe(expected: Record);
    }
}
