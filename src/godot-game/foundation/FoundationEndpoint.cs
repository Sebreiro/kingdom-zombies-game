using System;
using System.Globalization;
using System.Net;

namespace KingdomZombies.Foundation;

/// <summary>Parses the explicit manual IP-address and optional port grammar.</summary>
internal static class FoundationEndpoint
{
    internal static bool TryParse(string value, out string address, out int port)
    {
        address = "";
        port = 24680;
        if (string.IsNullOrWhiteSpace(value: value) || value.Length > 256)
        {
            return false;
        }

        string[] parts = value.Split(separator: ':');
        if (parts.Length > 2 || parts.Length == 2 && (!int.TryParse(s: parts[1], style: NumberStyles.None,
                provider: CultureInfo.InvariantCulture, result: out port) || port is < 1024 or > 65535)
                || !IPAddress.TryParse(ipString: parts[0], address: out IPAddress? parsed))
        {
            return false;
        }

        address = parsed.ToString();
        return true;
    }
}
