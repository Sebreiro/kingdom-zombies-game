namespace KingdomZombies.Multiplayer.Abstractions;

/// <summary>Describes a discoverable session using independent framework values.</summary>
public sealed class SessionAdvertisement
{
    /// <summary>Creates a detached advertisement; receiving adapters supply the sender address.</summary>
    public SessionAdvertisement(string sessionId, string sessionName, string hostLabel, string address,
            int port, int occupied, string availability, int protocol, string content)
    {
        SessionId = sessionId;
        SessionName = sessionName;
        HostLabel = hostLabel;
        Address = address;
        Port = port;
        Occupied = occupied;
        Availability = availability;
        Protocol = protocol;
        Content = content;
    }

    /// <summary>Gets the exact session key, independent of labels and endpoints.</summary>
    public string SessionId { get; }
    /// <summary>Gets the session display name.</summary>
    public string SessionName { get; }
    /// <summary>Gets the optional host display label.</summary>
    public string HostLabel { get; }
    /// <summary>Gets the address derived from the received datagram endpoint.</summary>
    public string Address { get; }
    /// <summary>Gets the transport port.</summary>
    public int Port { get; }
    /// <summary>Gets occupied ready plus pending reservations.</summary>
    public int Occupied { get; }
    /// <summary>Gets Available, Full or Unavailable.</summary>
    public string Availability { get; }
    /// <summary>Gets protocol compatibility.</summary>
    public int Protocol { get; }
    /// <summary>Gets exact fixture content compatibility.</summary>
    public string Content { get; }
}
