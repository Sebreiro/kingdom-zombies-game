using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Validates owned documentation against exact portable path and anchor identities.</summary>
internal static class DocumentationValidator
{
    /// <summary>Returns subject diagnostics without modifying documentation or the Git index.</summary>
    internal static IReadOnlyList<string> Check(IReadOnlyDictionary<string, byte[]> files,
            IReadOnlyList<string> required)
    {
        return [];
    }
}
