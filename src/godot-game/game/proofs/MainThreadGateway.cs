using System;
using System.Collections.Concurrent;
using System.Threading;

namespace KingdomZombies.Proofs;
/// <summary>Owns startup-thread access and a queue of pure results with generation fences.</summary>
internal sealed class MainThreadGateway
{
    /// <summary>Rejects access before any supplied engine operation can execute.</summary>
    internal void RequireThread()
    {
        if (Environment.CurrentManagedThreadId != _thread)
        {
            throw new InvalidOperationException(message: "THREAD_ACCESS");
        }
    }

    /// <summary>Runs an engine operation only on the captured startup thread.</summary>
    internal void Run(Action operation)
    {
        RequireThread();
        operation();
    }

    /// <summary>Accepts a pure result from any producer without invoking engine APIs.</summary>
    internal void Enqueue(LifetimeToken lifetime, int value, Action<int> apply, ManualResetEventSlim? barrier = null)
    {
        _queue.Enqueue(item: (lifetime, value, apply, barrier));
    }

    /// <summary>Drains only live generations after the optional controlled release barrier opens.</summary>
    internal void Drain()
    {
        RequireThread();
        int count = _queue.Count;
        for (int index = 0; index < count && _queue.TryDequeue(result: out var item); index++)
        {
            if (item.Barrier is not null && !item.Barrier.IsSet)
            {
                _queue.Enqueue(item: item);
                continue;
            }

            if (item.Lifetime.IsOpen)
            {
                item.Apply(obj: item.Value);
            }
        }
    }

    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly ConcurrentQueue<(LifetimeToken Lifetime, int Value, Action<int> Apply,
            ManualResetEventSlim? Barrier)> _queue = new();
}
