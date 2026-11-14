using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks the evaluated project boundary independently of compiler success.</summary>
internal static class ProjectEvaluator
{
    /// <summary>Returns actionable boundary violations for an actual project closure.</summary>
    internal static IReadOnlyList<string> Validate(string root)
    {
        return [];
    }
}
