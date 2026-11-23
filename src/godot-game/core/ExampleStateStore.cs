using System;
using System.Threading;

namespace KingdomZombies.Core;

/// <summary>Owns one immutable live proof root.</summary>
internal sealed class ExampleStateStore
{
    /// <summary>Initializes the live immutable root.</summary>
    internal ExampleStateStore(ExampleState initial)
    {
        ArgumentNullException.ThrowIfNull(argument: initial);
        _root = initial;
    }

    /// <summary>Acquires one complete root per read.</summary>
    internal ExampleState Read()
    {
        return Volatile.Read(location: ref _root);
    }

    /// <summary>Restores a fully prepared candidate with a controlled precommit seam.</summary>
    internal ExampleState Restore(ReadOnlyMemory<byte> document, Action<ExampleState>? beforeCommit = null,
            CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(location1: ref _writer, value: 1, comparand: 0) != 0)
        {
            throw new InvalidOperationException(message: "RESTORE_BUSY: another writer owns preparation");
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            ExampleState candidate = SnapshotCodec.Decode(document: document);
            beforeCommit?.Invoke(obj: candidate);
            cancellationToken.ThrowIfCancellationRequested();
            Interlocked.Exchange(location1: ref _root, value: candidate);
            return candidate;
        }
        finally
        {
            Volatile.Write(location: ref _writer, value: 0);
        }
    }

    private ExampleState _root;
    private int _writer;
}
