using System.Collections.Generic;

namespace KingdomZombies.Foundation;

/// <summary>Tracks physical movement holds without owning any simulation state.</summary>
internal sealed class FoundationInput
{
    /// <summary>Gets the normalized direction, treating aliases as one direction.</summary>
    internal int Direction => _gameplay ? (_held.Contains(item: 68) || _held.Contains(item: 4194321) ? 1 : 0)
            - (_held.Contains(item: 65) || _held.Contains(item: 4194319) ? 1 : 0) : 0;

    /// <summary>Gets whether the last physical transition was a fresh unblocked press.</summary>
    internal bool FreshPress { get; private set; }
    internal bool Gameplay => _gameplay;

    internal static bool WorldAllowed(bool alive, bool focused, bool popup, bool panel, bool guiFocused)
    {
        return alive && focused && !popup && !panel && !guiFocused;
    }

    internal bool Held(int key)
    {
        return _gameplay && _held.Contains(item: key);
    }

    internal void QueueJump()
    {
        _jump |= _gameplay && FreshPress;
    }

    internal bool TakeJump()
    {
        bool jump = _jump;
        _jump = false;
        return jump;
    }

    internal void Mouse(bool pressed)
    {
        Set(key: MouseKey, pressed: pressed);
        _click = pressed && _gameplay && FreshPress;
    }

    internal bool TakeClick(bool world)
    {
        bool click = world && _gameplay && _click;
        _click = false;
        return click;
    }

    internal static (decimal x, decimal y)? Aim(double localX, double localY, long origin, long ground)
    {
        double x = origin + localX * 1000 / 16;
        double y = ground + (540 - localY) * 1000 / 16;
        return double.IsFinite(d: x) && double.IsFinite(d: y) && x is >= -1000000000 and <= 1000000000
                && y is >= -1000000000 and <= 1000000000 ? ((decimal)x, (decimal)y) : null;
    }

    /// <summary>Closes held commands whenever a different screen takes input ownership.</summary>
    internal void SetContext(bool gameplay)
    {
        if (_gameplay != gameplay)
        {
            Clear();
            _gameplay = gameplay;
        }
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
        _jump = false;
        _click = false;
    }

    private const int MouseKey = -1;
    private readonly HashSet<int> _held = [];
    private readonly HashSet<int> _blocked = [];
    private bool _gameplay = true;
    private bool _jump;
    private bool _click;

    internal enum WorkspaceCommand
    {
        Interact,
        Inventory,
        Escape,
    }

    internal interface IWorkspace
    {
        bool CapturesWorld { get; }
        bool Handle(WorkspaceCommand command);
    }
}
