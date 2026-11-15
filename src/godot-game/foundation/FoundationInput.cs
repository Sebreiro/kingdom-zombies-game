using System.Collections.Generic;

namespace KingdomZombies.Foundation;

/// <summary>Tracks physical movement holds without owning any simulation state.</summary>
internal sealed class FoundationInput
{
    /// <summary>Gets the normalized direction, treating aliases as one direction.</summary>
    internal int Direction => (_held.Contains(item: 68) || _held.Contains(item: 4194321) ? 1 : 0)
            - (_held.Contains(item: 65) || _held.Contains(item: 4194319) ? 1 : 0);

    /// <summary>Gets whether the last physical transition was a fresh unblocked press.</summary>
    internal bool FreshPress { get; private set; }

    /// <summary>Defines the owning input screen without changing the RED subject's existing holds.</summary>
    internal void SetContext(bool gameplay)
    {
        _ = gameplay && _held.Count > 0;
    }

    /// <summary>Records a fresh physical key transition.</summary>
    internal void Set(int key, bool pressed)
    {
        FreshPress = false;
        if (pressed)
        {
            FreshPress = !_blocked.Contains(item: key) && _held.Add(item: key);
        }
        else
        {
            _held.Remove(item: key);
            _blocked.Remove(item: key);
        }
    }

    /// <summary>Closes all held commands at a focus or scene lifetime boundary.</summary>
    internal void Clear()
    {
        _blocked.UnionWith(other: _held);
        _held.Clear();
        FreshPress = false;
    }

    private readonly HashSet<int> _held = [];
    private readonly HashSet<int> _blocked = [];
}
