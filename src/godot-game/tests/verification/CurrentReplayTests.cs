using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves the current producer replay keeps every historical input and only appends new ones.</summary>
public sealed class CurrentReplayTests
{
    /// <summary>Added inputs follow the historical ones and are carried with every changed historical input.</summary>
    [Fact]
    public void ReplaySnapshotPaths_AddedInputsAfterHistorical_CarriesChangedAndAdded()
    {
        EngineGeneratedTests.ReplaySnapshotPaths(recorded: [("a.cs", "1"), ("b.cs", "9"), ("c.cs", "3"),
            ("d.cs", "4"), ("e.cs", "5")], historical: _historical).ShouldBe(expected: ["b.cs", "d.cs", "e.cs"]);
        EngineGeneratedTests.ReplaySnapshotPaths(recorded: _historical, historical: _historical).ShouldBeEmpty();
    }

    /// <summary>Dropping any historical input fails, wherever it was and whatever is added instead.</summary>
    [Fact]
    public void ReplaySnapshotPaths_MissingHistoricalInput_Rejects()
    {
        foreach ((string Path, string Sha256)[] recorded in new (string Path, string Sha256)[][]
        {
            [("a.cs", "1"), ("c.cs", "3")],
            [("a.cs", "1"), ("b.cs", "2")],
            [("b.cs", "2"), ("c.cs", "3"), ("d.cs", "4")],
            [("a.cs", "1"), ("c.cs", "3"), ("d.cs", "4")],
        })
        {
            Should.Throw<ShouldAssertException>(actual: () => EngineGeneratedTests.ReplaySnapshotPaths(
                    recorded: recorded, historical: _historical)).Message.ShouldContain(
                            expected: "REPLAY_HISTORICAL_INPUTS");
        }
    }

    /// <summary>Reordering historical inputs, or moving one among the added inputs, fails.</summary>
    [Fact]
    public void ReplaySnapshotPaths_ReorderedHistoricalInputs_Rejects()
    {
        foreach ((string Path, string Sha256)[] recorded in new (string Path, string Sha256)[][]
        {
            [("b.cs", "2"), ("a.cs", "1"), ("c.cs", "3")],
            [("a.cs", "1"), ("c.cs", "3"), ("b.cs", "2")],
            [("a.cs", "1"), ("c.cs", "3"), ("d.cs", "4"), ("b.cs", "2")],
        })
        {
            Should.Throw<ShouldAssertException>(actual: () => EngineGeneratedTests.ReplaySnapshotPaths(
                    recorded: recorded, historical: _historical)).Message.ShouldContain(
                            expected: "REPLAY_HISTORICAL_INPUTS");
        }
    }

    /// <summary>Added inputs may not repeat a historical path, repeat each other or appear out of order.</summary>
    [Fact]
    public void ReplaySnapshotPaths_InvalidAddedInputs_Rejects()
    {
        foreach ((string Path, string Sha256)[] added in new (string Path, string Sha256)[][]
        {
            [("b.cs", "2")],
            [("d.cs", "4"), ("d.cs", "4")],
            [("e.cs", "5"), ("d.cs", "4")],
        })
        {
            Should.Throw<ShouldAssertException>(actual: () => EngineGeneratedTests.ReplaySnapshotPaths(
                    recorded: [.. _historical, .. added], historical: _historical)).Message.ShouldContain(
                            expected: "REPLAY_ADDED_INPUTS");
        }
    }

    private static readonly (string Path, string Sha256)[] _historical = [("a.cs", "1"), ("b.cs", "2"),
        ("c.cs", "3")];
}
