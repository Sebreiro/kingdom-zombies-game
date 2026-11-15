using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using KingdomZombies.Multiplayer.Abstractions;
using KingdomZombies.Proofs;

namespace KingdomZombies.Foundation;

/// <summary>Owns one manually polled reliable ENet peer on the authentic native thread.</summary>
internal sealed class EnetConnectionService : IConnectionService, IDisposable
{
    public EnetConnectionService(MainThreadGateway gateway)
    {
        _gateway = gateway;
    }

    public string Generation { get; } = Guid.NewGuid().ToString(format: "N");
    public bool Connected { get; private set; }

    public void Host(int port)
    {
        _gateway.RequireThread();
        RequireUnused(port: port);
        _peer = new ENetMultiplayerPeer();
        Check(error: _peer.CreateServer(port: port, maxClients: 16, maxChannels: 1));
        _peer.TransferMode = MultiplayerPeer.TransferModeEnum.Reliable;
        _peer.PeerConnected += ConnectedPeer;
        _peer.PeerDisconnected += DisconnectedPeer;
        Connected = true;
    }

    public void Join(string address, int port)
    {
        _gateway.RequireThread();
        RequireUnused(port: port);
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: address);
        _peer = new ENetMultiplayerPeer();
        Check(error: _peer.CreateClient(address: address, port: port, channelCount: 1));
        _peer.TransferMode = MultiplayerPeer.TransferModeEnum.Reliable;
        _peer.PeerConnected += ConnectedPeer;
        _peer.PeerDisconnected += DisconnectedPeer;
    }

    public IReadOnlyList<ConnectionPacket> Poll(int maximumPackets)
    {
        _gateway.RequireThread();
        ArgumentOutOfRangeException.ThrowIfLessThan(value: maximumPackets, other: 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value: maximumPackets, other: 64);
        var packets = new List<ConnectionPacket>();
        if (_peer is null || _closed || _transportLost)
        {
            return packets;
        }

        _peer.Poll();
        Connected = _peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Connected;
        _transportLost = _peer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected;
        for (int index = 0; index < maximumPackets && _peer.GetAvailablePacketCount() > 0; index++)
        {
            int sender = _peer.GetPacketPeer();
            byte[] bytes = _peer.GetPacket();
            Check(error: _peer.GetPacketError());
            if (bytes.Length <= FoundationProtocol.MaximumBytes)
            {
                packets.Add(item: new ConnectionPacket(sender: sender, generation: Generation, payload: bytes));
            }
        }

        return packets;
    }

    public void Send(int peer, ReadOnlyMemory<byte> payload)
    {
        _gateway.RequireThread();
        if (_closed || _peer is null || !Connected)
        {
            throw new InvalidOperationException(message: "TRANSPORT_NOT_CONNECTED");
        }

        if (payload.Length is < 2 or > FoundationProtocol.MaximumBytes || peer < 0)
        {
            throw new InvalidDataException(message: "TRANSPORT_PACKET");
        }

        if (peer > 1 && !_connectedPeers.Contains(item: peer))
        {
            return;
        }

        _peer.SetTargetPeer(id: peer);
        Check(error: _peer.PutPacket(buffer: payload.ToArray()));
        using ENetConnection host = _peer.GetHost();
        host.Flush();
    }

    public void Close()
    {
        _gateway.RequireThread();
        if (_closed)
        {
            return;
        }

        _closed = true;
        Connected = false;
        if (_peer is not null)
        {
            _peer.PeerConnected -= ConnectedPeer;
            _peer.PeerDisconnected -= DisconnectedPeer;
        }

        _connectedPeers.Clear();
        _peer?.Close();
        _peer?.Dispose();
        _peer = null;
    }

    public void Dispose()
    {
        Close();
    }

    private readonly MainThreadGateway _gateway;
    private readonly HashSet<long> _connectedPeers = [];
    private ENetMultiplayerPeer? _peer;
    private bool _closed;
    private bool _transportLost;

    private void ConnectedPeer(long peer)
    {
        if (!_closed)
        {
            _connectedPeers.Add(item: peer);
        }
    }

    private void DisconnectedPeer(long peer)
    {
        _connectedPeers.Remove(item: peer);
    }

    private void RequireUnused(int port)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value: port, other: 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value: port, other: 65535);
        if (_closed || _peer is not null)
        {
            throw new InvalidOperationException(message: "TRANSPORT_ALREADY_USED");
        }
    }

    private void Check(Error error)
    {
        _gateway.RequireThread();
        if (error != Error.Ok)
        {
            throw new InvalidOperationException(message: "ENET_ERROR:" + error);
        }
    }
}
