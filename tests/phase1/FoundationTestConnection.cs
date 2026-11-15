using System;
using System.Collections.Generic;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Replaces only the independent connection provider for deterministic coordinator checks.</summary>
internal sealed class FoundationTestConnection : IConnectionService
{
    public FoundationTestConnection(int sender)
    {
        Sender = sender;
    }

    public string Generation { get; } = Guid.NewGuid().ToString(format: "N");
    public bool Connected { get; private set; }

    public string PeerGeneration(int peer) { return Generation; }

    public void Host(int port)
    {
        Connected = true;
    }

    public void Join(string address, int port)
    {
        Connected = true;
    }

    public void Send(int peer, ReadOnlyMemory<byte> payload)
    {
        FoundationTestConnection partner = Partner ?? throw new InvalidOperationException(message: "TEST_PARTNER");
        partner._packets.Enqueue(item: new ConnectionPacket(sender: Sender, generation: partner.Generation,
                payload: payload));
    }

    public IReadOnlyList<ConnectionPacket> Poll(int maximumPackets)
    {
        var packets = new List<ConnectionPacket>();
        while (packets.Count < maximumPackets && _packets.TryDequeue(result: out ConnectionPacket? packet))
        {
            packets.Add(item: packet);
        }

        return packets;
    }

    public void Close()
    {
        Connected = false;
        _packets.Clear();
    }

    internal int Sender { get; }
    internal FoundationTestConnection? Partner { get; set; }

    private readonly Queue<ConnectionPacket> _packets = new();
}
