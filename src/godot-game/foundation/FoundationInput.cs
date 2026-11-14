using System.Collections.Generic;

namespace KingdomZombies.Foundation;

/// <summary>Tracks physical movement holds without owning any simulation state.</summary>
internal sealed class FoundationInput
{
    /// <summary>Gets the normalized direction, treating aliases as one direction.</summary>
    internal int Direction => (_held.Contains(item: 68) || _held.Contains(item: 4194321) ? 1 : 0)
            - (_held.Contains(item: 65) || _held.Contains(item: 4194319) ? 1 : 0);

    /// <summary>Records a fresh physical key transition.</summary>
    internal void Set(int key, bool pressed)
    {
        if (pressed)
        {
            _held.Add(item: key);
        }
        else
        {
            _held.Remove(item: key);
        }
    }

    /// <summary>Closes all held commands at a focus or scene lifetime boundary.</summary>
    internal void Clear()
    {
        _held.Clear();
    }

    private readonly HashSet<int> _held = [];
}
