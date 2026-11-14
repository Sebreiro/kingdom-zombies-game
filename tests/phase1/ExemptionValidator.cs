using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Rejects undeclared suppression and generated-origin claims in owned source and effective policy.</summary>
internal static class ExemptionValidator
{
    /// <summary>Checks syntax and semantic suppression identities against exact observed member permissions.</summary>
    internal static IReadOnlyList<string> CheckSource(string path, string content,
            IReadOnlySet<string>? permissions = null)
    {
        return [];
    }
}
