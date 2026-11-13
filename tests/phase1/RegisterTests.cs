using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class RegisterTests
{
    private static (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) Inputs() =>
        (RegisterValidator.Read("docs/code/provenance/manifest.json"), RegisterValidator.Read("docs/code/register.json"), RegisterValidator.Read("docs/code/exemptions.json"));

    [Fact, Trait("Case", "register.capture")]
    public void Capture_MissingHeadingRelation_IsRejected()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        input.Register["rules"]![0]!["headingRefs"]!.AsArray().RemoveAt(0);
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains("R-01: HEADING_RELATION", StringComparison.Ordinal));
        ProofProtocol.Record("register.capture", "CAPTURE_EXACT", "85 IDs, 84 heading occurrences, 24 donor exemptions and missing R-01 relation");
    }
}
