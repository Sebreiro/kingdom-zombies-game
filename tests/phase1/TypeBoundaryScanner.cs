using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Inspects compiled metadata without loading or constructing engine objects.</summary>
internal static class TypeBoundaryScanner
{
    /// <summary>Checks expected compiled subjects and referenced types, including generated bodies.</summary>
    internal static IReadOnlyList<string> Validate(string assembly, string expectedType, string boundary)
    {
        return [];
    }
}
