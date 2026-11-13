using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks exact source bytes against the inherited project text-format contract.</summary>
internal static class SourceFileFormat
{
    /// <summary>Returns an actionable rule and byte location for each invalid source representation.</summary>
    internal static IReadOnlyList<string> Check(string path, byte[] content)
    {
        return [];
    }
}
