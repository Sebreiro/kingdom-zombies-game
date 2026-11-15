using System;
using System.IO;

namespace KingdomZombies.Foundation;

/// <summary>Restricts diagnostic files to the invocation's existing nonce-owned directory.</summary>
internal static class FoundationObservationScope
{
    internal static bool ValidCheckpoint(string value, int maximumLength)
    {
        if (value.Length > maximumLength || !(value.StartsWith(value: "draw-", comparisonType:
                StringComparison.Ordinal) || value.StartsWith(value: "capture-", comparisonType:
                        StringComparison.Ordinal))) { return false; }
        foreach (char character in value)
        {
            if (!char.IsAsciiLetterOrDigit(c: character) && character is not '-' and not '_') { return false; }
        }

        return true;
    }

    internal static string? Directory()
    {
        string? output = Environment.GetEnvironmentVariable(variable: "PHASE3_OUTPUT");
        string? nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        string? root = Environment.GetEnvironmentVariable(variable: "PHASE3_RUN_ROOT");
        if (output is null || nonce is null || root is null) { return null; }
        if (!Guid.TryParseExact(input: nonce, format: "D", result: out _))
        {
            throw new InvalidDataException(message: "OBSERVATION_NONCE");
        }

        return Validate(output: output, root: root, nonce: nonce);
    }

    private static string Validate(string output, string root, string nonce)
    {
        string full = Path.GetFullPath(path: output);
        string parent = Path.GetFullPath(path: root);
        if (!StringComparer.Ordinal.Equals(x: Path.GetFileName(path: parent), y: nonce)
                || !full.StartsWith(value: parent + Path.DirectorySeparatorChar, comparisonType:
                        OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)
                || !StringComparer.Ordinal.Equals(x: File.ReadAllText(path: Path.Combine(path1: parent,
                        path2: "observation.owner")), y: nonce))
        {
            throw new InvalidDataException(message: "OBSERVATION_OWNERSHIP");
        }

        CheckLinks(output: full, root: parent);
        return full;
    }

    private static void CheckLinks(string output, string root)
    {
        for (DirectoryInfo? current = new DirectoryInfo(path: output); current is not null; current = current.Parent)
        {
            if (current.Attributes.HasFlag(flag: FileAttributes.ReparsePoint))
            {
                throw new InvalidDataException(message: "OBSERVATION_LINK");
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(x: current.FullName, y: root)) { return; }
        }

        throw new InvalidDataException(message: "OBSERVATION_ROOT");
    }
}
