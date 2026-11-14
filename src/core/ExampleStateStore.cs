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
        cancellationToken.ThrowIfCancellationRequested();
        ExampleState candidate = SnapshotCodec.Decode(document: document);
        beforeCommit?.Invoke(obj: candidate);
        return Read();
    }

    private ExampleState _root;
}
