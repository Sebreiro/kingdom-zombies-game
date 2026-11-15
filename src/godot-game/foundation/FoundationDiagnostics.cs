using System;
using System.IO;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Writes read-only state observations without submitting input or mutating authority.</summary>
internal sealed class FoundationDiagnostics
{
    internal void Observe(FoundationCoordinator? coordinator, long inputEvents, int direction,
            int nativeActors, bool polledRight)
    {
        string? output = Environment.GetEnvironmentVariable(variable: "PHASE3_OUTPUT");
        string? nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        if (output is null || nonce is null)
        {
            return;
        }

        string record = JsonSerializer.Serialize(value: new
        {
            nonce, pid = Environment.ProcessId, provenance = "synthetic-native-ParseInputEvent",
            frame = ++_frame, inputEvents, direction, nativeActors, polledRight, ready = coordinator?.Ready ?? false,
            playerId = coordinator?.PlayerId, bindingId = coordinator?.BindingId,
            installedRevision = coordinator?.InstalledRevision, acceptedIntents = coordinator?.AcceptedIntents,
            events = coordinator?.Events, error = coordinator?.Error, snapshot = coordinator?.Snapshot,
        });
        File.AppendAllText(path: Path.Combine(path1: output, path2: "frames.ndjson"),
                contents: record + Environment.NewLine);
    }

    private long _frame;
}
