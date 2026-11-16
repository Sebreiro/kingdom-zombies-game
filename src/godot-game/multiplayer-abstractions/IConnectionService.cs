using System;
using System.Collections.Generic;

namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Provides replaceable packet transport with explicit delivery semantics and framework values only.</summary>
public interface IConnectionService
{
    /// <summary>Gets the unique provider lifetime for trusted callback fencing.</summary>
    string Generation { get; }
    /// <summary>Gets whether the connection can send packets.</summary>
    bool Connected { get; }
    /// <summary>Gets the current trusted connection lifetime, or empty after this peer disconnects.</summary>
    string PeerGeneration(int peer);
    /// <summary>Starts a host with bounded transport capacity, separate from admission.</summary>
    void Host(int port);
    /// <summary>Starts one asynchronous connection to an explicit endpoint.</summary>
    void Join(string address, int port);
    /// <summary>Sends a bounded packet reliably by default, or as replaceable ordered state when false.</summary>
    void Send(int peer, ReadOnlyMemory<byte> payload, bool reliable = true);
    /// <summary>Polls without waiting and receives at most the requested bounded count.</summary>
    IReadOnlyList<ConnectionPacket> Poll(int maximumPackets);
    /// <summary>Closes this lifetime idempotently before disposal or replacement.</summary>
    void Close();
}
