using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace KingdomZombies.Foundation;

/// <summary>Parses the explicit manual IP-address and optional port grammar.</summary>
internal static class FoundationEndpoint
{
    internal static bool TryParse(string value, out string address, out int port)
    {
        address = "";
        port = 47778;
        if (string.IsNullOrWhiteSpace(value: value) || value.Length > 256)
        {
            return false;
        }

        string[] parts = value.Split(separator: ':');
        if (parts.Length > 2 || parts.Length == 2 && (!int.TryParse(s: parts[1], style: NumberStyles.None,
                provider: CultureInfo.InvariantCulture, result: out port) || port is < 1 or > 65535)
                || parts[0].Split(separator: '.').Length != 4
                || !IPAddress.TryParse(ipString: parts[0], address: out IPAddress? parsed)
                || parsed.AddressFamily != AddressFamily.InterNetwork
                || !StringComparer.Ordinal.Equals(x: parts[0], y: parsed.ToString()))
        {
            return false;
        }

        address = parsed.ToString();
        return true;
    }
}
