using System;

namespace KingdomZombies.Core;

/// <summary>Defines the strict detached proof snapshot boundary.</summary>
internal static class SnapshotCodec
{
    /// <summary>Encodes an independent state.</summary>
    internal static byte[] Encode(ExampleState state)
    {
        ArgumentNullException.ThrowIfNull(argument: state);
        return [123, 125];
    }

    /// <summary>Prepares an independent candidate.</summary>
    internal static ExampleState Decode(ReadOnlyMemory<byte> document)
    {
        ArgumentOutOfRangeException.ThrowIfZero(value: document.Length, paramName: nameof(document));
        return new ExampleState(elapsedMilliseconds: 0, ordered: [], records: []);
    }
}
