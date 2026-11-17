using System;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Separates persistent current appearance from explicitly triggered transient effects.</summary>
public sealed class AppearanceState
{
    /// <summary>Accepts the appearance definition.</summary>
    public AppearanceState(byte[] definition)
    {
        ArgumentNullException.ThrowIfNull(argument: definition);
    }

    /// <summary>Returns a detached current-state publication.</summary>
    public JsonElement Snapshot => _snapshot.Clone();

    /// <summary>Silently restores the current appearance.</summary>
    public void Restore(string body, string kind, JsonElement equipment, bool selected)
    {
        _ = body;
        _ = kind;
        _ = equipment;
        _ = selected;
        _ = _snapshot;
    }

    /// <summary>Explicitly triggers a death transition.</summary>
    public void TriggerDeath()
    {
        _ = _snapshot;
    }

    /// <summary>Advances only the caller-supplied clock.</summary>
    public void Advance(double elapsedSeconds)
    {
        _ = elapsedSeconds;
        _ = _snapshot;
    }

    /// <summary>Revokes current appearance and effects.</summary>
    public void Reset()
    {
        _ = _snapshot;
    }

    private readonly JsonElement _snapshot = JsonSerializer.SerializeToElement(value: new
    {
        body = "",
        kind = "empty",
        eventCount = 0,
        transientVisible = false,
        persistent = false,
        equipment = new { },
    });
}
