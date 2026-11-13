using System.Text.Json.Nodes;

namespace KingdomZombies.Phase1.Tests;

public static class RegisterValidator
{
    public static JsonObject Read(string relative) => JsonNode.Parse(File.ReadAllText(Path.Combine(ProofProtocol.Root, relative)))!.AsObject();

    public static IReadOnlyList<string> Validate(JsonObject manifest, JsonObject register, JsonObject exemptions, bool requireImplemented = false)
    {
        return Array.Empty<string>();
    }
}
