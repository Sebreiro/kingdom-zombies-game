using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves exact documentation navigation with actual compliant and mutated inputs.</summary>
public sealed class DocumentationTests
{
    /// <summary>Rejects a wrong-case target even when the host filesystem accepts the spelling.</summary>
    [Fact, Trait("Case", "docs.navigation")]
    public void Check_WrongCaseLink_ReportsExactTarget()
    {
        var files = new Dictionary<string, byte[]>(comparer: System.StringComparer.Ordinal)
        {
            ["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "# Guide\n[Code](docs/code/README.md)\n"),
            ["docs/code/README.md"] = Encoding.UTF8.GetBytes(s: "# Code\n[Rules](/.editorconfig)\n"),
            [".editorconfig"] = Encoding.UTF8.GetBytes(s: "root = true\n"),
        };
        DocumentationValidator.Check(files: files, required: ["AGENTS.md"]).ShouldBeEmpty();
        files["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "# Guide\n[Code](docs/Code/README.md)\n");
        DocumentationValidator.Check(files: files, required: ["AGENTS.md"]).ShouldContain(
                elementPredicate: item => System.StringComparer.Ordinal.Equals(x: item,
                        y: "DOC_CASE AGENTS.md:2 docs/Code/README.md"));
        ProofProtocol.Record(id: "docs.navigation", diagnostic: "DOC_CASE", location: "AGENTS.md:2");
    }
}
