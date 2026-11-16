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
        CheckRequest(directory: directory);
    }

    private static void CheckRequest(string directory)
    {
        string file = Path.Combine(path1: directory, path2: "draw.request");
        const string Record = "{\"nonce\":\"owned\",\"checkpoint\":\"draw-one\"}";
        File.WriteAllText(path: file, contents: Record);
        CheckWindowsLocks(file: file, record: Record);
        File.ReadAllText(path: file).ShouldBe(expected: Record);
        FoundationDrawRequest.Consume(file: file, nonce: "owned").ShouldBe(expected: "draw-one");
        File.Exists(path: file).ShouldBeFalse();
        FoundationDrawRequest.Consume(file: file, nonce: "owned").ShouldBeNull();
        CheckRejectedRequest(file: file);
    }

    private static void CheckWindowsLocks(string file, string record)
    {
        if (!OperatingSystem.IsWindows()) { return; }
        using (var locked = new FileStream(path: file, mode: FileMode.Open, access: FileAccess.ReadWrite,
                share: FileShare.None))
        {
            Should.NotThrow(action: () => FoundationDrawRequest.Consume(file: file, nonce: "owned"))
                    .ShouldBeNull();
        }

        File.ReadAllText(path: file).ShouldBe(expected: record);
        using (var locked = new FileStream(path: file, mode: FileMode.Open, access: FileAccess.Read,
                share: FileShare.Read))
        {
            Should.NotThrow(action: () => FoundationDrawRequest.Consume(file: file, nonce: "owned"))
                    .ShouldBeNull();
            File.ReadAllText(path: file).ShouldBe(expected: record);
        }
    }

    private static void CheckRejectedRequest(string file)
    {
        string[] invalid = ["{\"nonce\":\"foreign\",\"checkpoint\":\"draw-one\"}",
            "{\"nonce\":\"owned\",\"checkpoint\":\"draw-../escape\"}"];
        foreach (string record in invalid)
        {
            File.WriteAllText(path: file, contents: record);
            Should.Throw<InvalidDataException>(actual: () => FoundationDrawRequest.Consume(file: file,
                    nonce: "owned"));
            File.ReadAllText(path: file).ShouldBe(expected: record);
        }

        File.WriteAllText(path: file, contents: "{");
        Should.Throw<JsonException>(actual: () => FoundationDrawRequest.Consume(file: file, nonce: "owned"));
        File.ReadAllText(path: file).ShouldBe(expected: "{");
        File.Delete(path: file);
        File.WriteAllText(path: file, contents: "{\"nonce\":\"owned\",\"checkpoint\":\"draw-one\"}");
        if (OperatingSystem.IsWindows())
        {
            File.SetAttributes(path: file, fileAttributes: FileAttributes.ReadOnly);
            try
            {
                Should.Throw<UnauthorizedAccessException>(actual: () => FoundationDrawRequest.Consume(file: file,
                        nonce: "owned"));
                File.Exists(path: file).ShouldBeTrue();
            }
            finally { File.SetAttributes(path: file, fileAttributes: FileAttributes.Normal); }
        }

        File.Delete(path: file);
    }
}
