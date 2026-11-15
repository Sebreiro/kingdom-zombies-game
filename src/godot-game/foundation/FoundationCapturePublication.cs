using System;
using System.IO;

namespace KingdomZombies.Foundation;

/// <summary>Publishes complete checkpoint metadata with one same-directory atomic acknowledgment.</summary>
internal static class FoundationCapturePublication
{
    internal static void Publish(string file, string record, Action<string>? beforeMove = null)
    {
        string temporary = file + "." + Guid.NewGuid().ToString(format: "N") + ".tmp";
        File.WriteAllText(path: temporary, contents: record);
        beforeMove?.Invoke(obj: temporary);
        File.Move(sourceFileName: temporary, destFileName: file);
    }
}
