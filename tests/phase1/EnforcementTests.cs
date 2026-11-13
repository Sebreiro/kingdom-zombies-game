using System;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual clean and intended-negative builds under the repository policy.</summary>
public sealed class EnforcementTests
{
    /// <summary>A real copied Core consumer rejects precisely the sixty-first body line.</summary>
    [Fact, Trait("Case", "rules.build-wiring")]
    public void RealConsumerRejectsSixtyFirstBodyLine()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "        return 1;",
                newValue: new string(c: '\n', count: 60) + "        return 1;",
                comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) changed = fixture.Build();
        BuildFixture.HasDiagnostic(result: changed, id: "SCR0006", line: 6, column: 25).ShouldBeTrue(
                customMessage: changed.Output);
        ProofProtocol.Record(id: "rules.build-wiring", diagnostic: "REAL_BUILD_SCR0006", location: fixture
                .DirectoryPath + "/src/core/Subject.cs(6,25)");
    }
}
