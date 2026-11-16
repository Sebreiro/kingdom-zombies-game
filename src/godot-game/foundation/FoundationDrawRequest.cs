using System;
using System.IO;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Validates and consumes a draw request before its capture can begin.</summary>
internal static class FoundationDrawRequest
{
    internal static string? Consume(string file, string nonce)
    {
        if (!File.Exists(path: file)) { return null; }
        using JsonDocument request = JsonDocument.Parse(json: File.ReadAllText(path: file));
        string? actual = request.RootElement.GetProperty(propertyName: "nonce").GetString();
        string checkpoint = request.RootElement.GetProperty(propertyName: "checkpoint").GetString() ?? "";
        if (!StringComparer.Ordinal.Equals(x: actual, y: nonce)
                || !FoundationObservationScope.ValidCheckpoint(value: checkpoint, maximumLength: 64))
        {
            throw new InvalidDataException(message: "DRAW_REQUEST");
        }

        File.Delete(path: file);
        return checkpoint;
    }
}
