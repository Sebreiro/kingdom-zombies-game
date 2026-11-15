using System;
using System.IO;
using System.Text.Json;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Writes read-only state observations without submitting input or mutating authority.</summary>
internal sealed class FoundationDiagnostics
{
    internal void Attach(Godot.Control root, MainThreadGateway gateway)
    {
        _draw = new FoundationDrawObserver(root: root, gateway: gateway);
        _draw.Start();
    }

    internal void Detach()
    {
        _draw?.Stop();
    }

    internal void RecordInput(int key, bool pressed, bool echo)
    {
        _lastInput = new { key, pressed, echo };
    }

    internal void LostFocus()
    {
        _focusLosses++;
    }

    internal void Quitting()
    {
        string? directory = FoundationObservationScope.Directory();
        if (directory is null) { return; }
        string file = Path.Combine(path1: directory, path2: "quit.ack.json");
        string record = JsonSerializer.Serialize(value: new { action = "ordinary-Quit", pid = Environment.ProcessId,
            nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE"),
            role = Environment.GetEnvironmentVariable(variable: "PHASE3_ROLE"), frame = _frame });
        File.WriteAllText(path: file + ".tmp", contents: record);
        File.Move(sourceFileName: file + ".tmp", destFileName: file);
    }

    internal void Observe(FoundationCoordinator? coordinator, long inputEvents, int direction,
            int nativeActors, bool polledRight, string? feedback = null, long nowMilliseconds = 0,
            FoundationServices? services = null)
    {
        string? output = FoundationObservationScope.Directory();
        string? nonce = Environment.GetEnvironmentVariable(variable: "PHASE3_NONCE");
        if (output is null || nonce is null)
        {
            return;
        }

        string record = JsonSerializer.Serialize(value: new
        {
            nonce, pid = Environment.ProcessId, provenance = "read-only-state-observation", nowMilliseconds,
            inputDriver = Environment.GetEnvironmentVariable(variable: "PHASE3_INPUT_DRIVER"),
            frame = ++_frame, inputEvents, direction, nativeActors, polledRight, lastInput = _lastInput,
            focusLosses = _focusLosses, observationKind = "process-state-not-rendered",
            ready = coordinator?.Ready ?? false,
            playerId = coordinator?.PlayerId, bindingId = coordinator?.BindingId,
            installedRevision = coordinator?.InstalledRevision, acceptedIntents = coordinator?.AcceptedIntents,
            lastAcceptedIntent = coordinator?.LastAcceptedIntent,
            lastAcceptedRevision = coordinator?.LastAcceptedRevision,
            events = coordinator?.Events, error = coordinator?.Error ?? feedback, snapshot = coordinator?.Snapshot,
            profile = services?.Identity.Selected, profileError = services?.Identity.FailureReason,
            discoveryRows = services?.Browser.Rows, discoveryError = services?.Discovery.FailureReason,
        });
        File.AppendAllText(path: Path.Combine(path1: output, path2: "frames.ndjson"),
                contents: record + Environment.NewLine);
        if (_draw is not null) { _draw.Sample = record; }
    }

    private long _frame;
    private long _focusLosses;
    private object? _lastInput;
    private FoundationDrawObserver? _draw;
}
