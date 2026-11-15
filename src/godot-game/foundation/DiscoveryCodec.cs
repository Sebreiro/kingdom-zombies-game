using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Maps bounded untrusted discovery bytes into independent immutable values.</summary>
internal static class DiscoveryCodec
{
    internal static byte[] Encode(SessionAdvertisement advertisement)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value: new
        {
            schema = 1, id = advertisement.SessionId, name = advertisement.SessionName,
            host = advertisement.HostLabel, port = advertisement.Port, occupied = advertisement.Occupied,
            availability = advertisement.Availability, protocol = advertisement.Protocol,
            content = advertisement.Content,
        }, options: _options);
    }

    internal static SessionAdvertisement? Decode(ReadOnlyMemory<byte> bytes, string address)
    {
        if (bytes.Length is 0 or > 4096)
        {
            return null;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(utf8Json: bytes,
                    options: new JsonDocumentOptions { MaxDepth = 4 });
            JsonElement root = document.RootElement;
            string id = Text(root: root, property: "id", maximum: 128);
            string name = Text(root: root, property: "name", maximum: 1024);
            string host = Text(root: root, property: "host", maximum: 1024);
            string status = Text(root: root, property: "availability", maximum: 16);
            int port = root.GetProperty(propertyName: "port").GetInt32();
            int occupied = root.GetProperty(propertyName: "occupied").GetInt32();
            if (root.GetProperty(propertyName: "schema").GetInt32() != 1 || string.IsNullOrWhiteSpace(value: id)
                    || string.IsNullOrWhiteSpace(value: name) || port is < 1 or > 65535 || occupied is < 1 or > 4
                    || status is not ("Available" or "Full" or "Unavailable")
                    || !FoundationEndpoint.TryParse(value: address, address: out _, port: out _))
            {
                return null;
            }

            return new SessionAdvertisement(sessionId: id, sessionName: name, hostLabel: host, address: address,
                    port: port, occupied: occupied, availability: status,
                    protocol: root.GetProperty(propertyName: "protocol").GetInt32(),
                    content: Text(root: root, property: "content", maximum: 512));
        }
        catch (Exception error) when (error is JsonException or InvalidOperationException
                or System.Collections.Generic.KeyNotFoundException or FormatException)
        {
            return null;
        }
    }

    private static readonly JsonSerializerOptions _options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private static string Text(JsonElement root, string property, int maximum)
    {
        string text = root.GetProperty(propertyName: property).GetString() ?? "";
        return text.Length <= maximum ? text : throw new JsonException(message: "DISCOVERY_LABEL_BOUND");
    }
}
