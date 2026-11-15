using System;
using System.Collections.Generic;
using KingdomZombies.Foundation;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Queues authenticated provider packets and retains actual production wire replies.</summary>
internal sealed class FoundationHarnessConnection : IConnectionService
{
    public string Generation { get; } = Guid.NewGuid().ToString(format: "N");
    public bool Connected { get; private set; }
    public string PeerGeneration(int peer)
    {
        return PeerGenerations.GetValueOrDefault(key: peer, defaultValue: Generation);
    }
    public void Host(int port) { Connected = true; }
    public void Join(string address, int port) { Connected = true; }
    public void Send(int peer, ReadOnlyMemory<byte> payload)
    {
        Sent.Add(item: (peer, FoundationProtocol.Decode(bytes: payload)));
    }

    public IReadOnlyList<ConnectionPacket> Poll(int maximumPackets)
    {
        var result = new List<ConnectionPacket>();
        while (result.Count < maximumPackets && _incoming.TryDequeue(result: out ConnectionPacket? packet))
        {
            result.Add(item: packet);
        }

        return result;
    }

    public void Close() { Connected = false; CloseCount++; }
    internal int CloseCount { get; private set; }
    internal Dictionary<int, string> PeerGenerations { get; } = [];
    internal List<(int Peer, FoundationEnvelope Envelope)> Sent { get; } = [];
    internal void Enqueue(int sender, FoundationEnvelope envelope, string? generation = null)
    {
        Raw(sender: sender, payload: FoundationProtocol.Encode(envelope: envelope), generation: generation);
    }

    internal void Raw(int sender, ReadOnlyMemory<byte> payload, string? generation = null)
    {
        _incoming.Enqueue(item: new ConnectionPacket(sender: sender, generation: generation ?? Generation,
                payload: payload, peerGeneration: PeerGeneration(peer: sender)));
    }

    private readonly Queue<ConnectionPacket> _incoming = new();
}
