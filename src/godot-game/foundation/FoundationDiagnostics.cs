using System;
using System.IO;
using System.Text.Json;

namespace KingdomZombies.Foundation;

/// <summary>Writes read-only state observations without submitting input or mutating authority.</summary>
internal sealed class FoundationDiagnostics
{
    internal void Observe(FoundationCoordinator? coordinator, long inputEvents, int direction,
            int nativeActors, bool polledRight, string? feedback = null, long nowMilliseconds = 0,
            FoundationServices? services = null)
    {
        string? output = Environment.GetEnvironmentVariable(variable: "PHASE3_OUTPUT");
        string? nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        if (output is null || nonce is null)
        {
            return;
        }

        string record = JsonSerializer.Serialize(value: new
        {
            nonce, pid = Environment.ProcessId, provenance = "synthetic-native-ParseInputEvent", nowMilliseconds,
            frame = ++_frame, inputEvents, direction, nativeActors, polledRight, ready = coordinator?.Ready ?? false,
            playerId = coordinator?.PlayerId, bindingId = coordinator?.BindingId,
            installedRevision = coordinator?.InstalledRevision, acceptedIntents = coordinator?.AcceptedIntents,
            events = coordinator?.Events, error = coordinator?.Error ?? feedback, snapshot = coordinator?.Snapshot,
            profile = services?.Identity.Selected, profileError = services?.Identity.FailureReason,
            discoveryRows = services?.Browser.Rows, discoveryError = services?.Discovery.FailureReason,
        });
        File.AppendAllText(path: Path.Combine(path1: output, path2: "frames.ndjson"),
                contents: record + Environment.NewLine);
    }

    private long _frame;
}
