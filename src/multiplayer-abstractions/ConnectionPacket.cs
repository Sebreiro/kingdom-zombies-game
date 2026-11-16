using System;

namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Delivers detached bytes and sender identity supplied by the transport provider.</summary>
public sealed class ConnectionPacket
{
    /// <summary>Copies packet bytes while preserving the actual sender and adapter lifetime.</summary>
    public ConnectionPacket(int sender, string generation, ReadOnlyMemory<byte> payload, string? peerGeneration = null)
    {
        Sender = sender;
        Generation = generation;
        PeerGeneration = peerGeneration ?? generation;
        Payload = payload.ToArray();
    }

    /// <summary>Gets the actual transient sender, never a payload ownership claim.</summary>
    public int Sender { get; }
    /// <summary>Gets the trusted transport lifetime.</summary>
    public string Generation { get; }
    /// <summary>Gets the sender connection lifetime captured before consuming this packet.</summary>
    public string PeerGeneration { get; }
    /// <summary>Gets the detached packet bytes.</summary>
    public ReadOnlyMemory<byte> Payload { get; }
}
