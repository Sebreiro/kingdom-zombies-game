using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Owns bounded reusable multicast sockets with sender-derived session endpoints.</summary>
internal sealed class LanDiscoveryService : IDiscoveryService
{
    public string Generation { get; } = Guid.NewGuid().ToString(format: "N");
    public string FailureReason { get; private set; } = "";

    public void Advertise(SessionAdvertisement? advertisement)
    {
        if (!StringComparer.Ordinal.Equals(x: _advertisement?.SessionId, y: advertisement?.SessionId))
        {
            _nextAdvertisement = 0;
        }

        _advertisement = advertisement;
    }

    public void Refresh()
    {
        if (_disposed)
        {
            return;
        }

        _query = true;
        if (FailureReason.Length > 0)
        {
            _socket?.Dispose();
            _socket = null;
            FailureReason = "";
        }
    }

    public IReadOnlyList<SessionAdvertisement> Update(long nowMilliseconds)
    {
        if (_disposed)
        {
            return [];
        }

        try
        {
            Open();
            if (_query)
            {
                Send(bytes: Encoding.UTF8.GetBytes(s: "KZ3?"));
                _query = false;
            }

            Drain(now: nowMilliseconds);
            if (_advertisement is not null && nowMilliseconds >= _nextAdvertisement)
            {
                Send(bytes: DiscoveryCodec.Encode(advertisement: _advertisement));
                _nextAdvertisement = checked(nowMilliseconds + 1000);
            }
        }
        catch (Exception error) when (error is SocketException or NetworkInformationException
                or InvalidOperationException or ArgumentException)
        {
            FailureReason = "LAN discovery is unavailable. Refresh to try again, or enter a host address.";
        }

        foreach (string expired in _sessions.Where(predicate: pair => nowMilliseconds - pair.Value.Seen >= 3500)
                .Select(selector: pair => pair.Key).ToArray())
        {
            _sessions.Remove(key: expired);
        }

        return _sessions.Values.Select(selector: value => value.Advertisement).ToArray();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _socket?.Dispose();
            _socket = null;
            _sessions.Clear();
        }
    }

    private static readonly IPAddress _group = IPAddress.Parse(ipString: "239.255.77.77");
    private readonly Dictionary<string, (SessionAdvertisement Advertisement, long Seen)> _sessions
            = new(comparer: StringComparer.Ordinal);
    private readonly List<IPAddress> _interfaces = [];
    private readonly byte[] _receive = new byte[4097];
    private Socket? _socket;
    private SessionAdvertisement? _advertisement;
    private long _nextAdvertisement;
    private bool _query = true;
    private bool _disposed;

    private void Open()
    {
        if (_socket is not null)
        {
            return;
        }

        var socket = new Socket(addressFamily: AddressFamily.InterNetwork, socketType: SocketType.Dgram,
                protocolType: ProtocolType.Udp);
        try
        {
            socket.ExclusiveAddressUse = false;
            socket.SetSocketOption(optionLevel: SocketOptionLevel.Socket, optionName: SocketOptionName.ReuseAddress,
                    optionValue: true);
            socket.Bind(localEP: new IPEndPoint(address: IPAddress.Any, port: 47777));
            socket.SetSocketOption(optionLevel: SocketOptionLevel.IP, optionName: SocketOptionName.MulticastTimeToLive,
                    optionValue: 1);
            socket.MulticastLoopback = true;
            socket.Blocking = false;
            JoinInterfaces(socket: socket);
            _socket = socket;
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    private void JoinInterfaces(Socket socket)
    {
        _interfaces.Clear();
        foreach (IPAddress address in NetworkInterface.GetAllNetworkInterfaces().Where(predicate: item =>
                item.OperationalStatus == OperationalStatus.Up).SelectMany(selector: item =>
                        item.GetIPProperties().UnicastAddresses).Select(selector: item => item.Address)
                .Where(predicate: address => address.AddressFamily == AddressFamily.InterNetwork)
                .Distinct())
        {
            try
            {
                socket.SetSocketOption(optionLevel: SocketOptionLevel.IP, optionName: SocketOptionName.AddMembership,
                        optionValue: new MulticastOption(group: _group, mcint: address));
                _interfaces.Add(item: address);
            }
            catch (SocketException)
            {
            }
        }

        if (_interfaces.Count == 0)
        {
            throw new InvalidOperationException(message: "DISCOVERY_NO_INTERFACE");
        }
    }

    private void Send(byte[] bytes)
    {
        if (bytes.Length > 4096 || _socket is null)
        {
            return;
        }

        foreach (IPAddress address in _interfaces)
        {
            _socket.SetSocketOption(optionLevel: SocketOptionLevel.IP, optionName: SocketOptionName.MulticastInterface,
                    optionValue: address.GetAddressBytes());
            _socket.SendTo(buffer: bytes, remoteEP: new IPEndPoint(address: _group, port: 47777));
        }
    }

    private void Drain(long now)
    {
        for (int index = 0; index < 32 && _socket?.Poll(microSeconds: 0, mode: SelectMode.SelectRead) == true; index++)
        {
            EndPoint sender = new IPEndPoint(address: IPAddress.Any, port: 0);
            int count;
            try
            {
                count = _socket.ReceiveFrom(buffer: _receive, remoteEP: ref sender);
            }
            catch (SocketException error) when (error.SocketErrorCode is SocketError.MessageSize)
            {
                continue;
            }

            if (count == 4 && Encoding.UTF8.GetString(bytes: _receive, index: 0, count: count) is "KZ3?")
            {
                _nextAdvertisement = 0;
            }
            else if (count <= 4096 && sender is IPEndPoint endpoint
                    && DiscoveryCodec.Decode(bytes: _receive.AsMemory(start: 0, length: count),
                            address: endpoint.Address.ToString()) is SessionAdvertisement advertisement
                    && (_sessions.Count < 256 || _sessions.ContainsKey(key: advertisement.SessionId)))
            {
                _sessions[key: advertisement.SessionId] = (advertisement, now);
            }
        }
    }
}
