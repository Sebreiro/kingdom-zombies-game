using System;
using System.Collections.Generic;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Replaces transport alone while retaining production coordinator packet validation and ownership.</summary>
internal sealed class FoundationConnectionReplacement : IConnectionService
{
    public string Generation => Subject.Generation;
    public bool Connected => Subject.Connected;
    public string PeerGeneration(int peer) { return Subject.PeerGeneration(peer: peer); }
    public void Host(int port) { Subject.Host(port: port); }
    public void Join(string address, int port) { Subject.Join(address: address, port: port); }
    public void Send(int peer, ReadOnlyMemory<byte> payload) { Subject.Send(peer: peer, payload: payload); }
    public IReadOnlyList<ConnectionPacket> Poll(int maximumPackets)
    {
        return Subject.Poll(maximumPackets: maximumPackets);
    }

    public void Close() { Subject.Close(); }
    internal FoundationHarnessConnection Subject { get; } = new();
}
