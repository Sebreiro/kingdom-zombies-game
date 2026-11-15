using System.Collections.Generic;

namespace KingdomZombies.Foundation;

/// <summary>Holds framework-only runtime feedback and the engine's sampled monotonic clock.</summary>
internal sealed class FoundationRuntimeState
{
    internal bool Hosting { get; set; }
    internal long NowMilliseconds { get; set; }
    internal string Feedback { get; set; } = "";

    internal void Retire(ref FoundationCoordinator? current, ref FoundationCoordinator? closing)
    {
        if (current is null)
        {
            return;
        }

        if (closing is not null && !closing.Closed)
        {
            _retired.Add(item: closing);
        }

        closing = current;
        closing.RequestClose();
        current = null;
    }

    internal void TickRetired()
    {
        for (int index = _retired.Count - 1; index >= 0; index--)
        {
            _retired[index].Update(elapsedMilliseconds: 0);
            if (_retired[index].Closed)
            {
                _retired.RemoveAt(index: index);
            }
        }
    }

    internal void CloseRetired()
    {
        foreach (FoundationCoordinator coordinator in _retired)
        {
            coordinator.Close();
        }

        _retired.Clear();
    }

    private readonly List<FoundationCoordinator> _retired = [];
}
