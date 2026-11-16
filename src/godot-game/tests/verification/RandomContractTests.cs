using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KingdomZombies.Core;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires actual seeded replay and strict numeric input boundaries.</summary>
public sealed class RandomContractTests
{
    /// <summary>Matches independent raw observations captured before implementation, also required on Linux.</summary>
    [Fact, Trait("Case", "state.golden-vectors")]
    public void Runtime_CommonSeeds_MatchesIndependentVectors()
    {
        Environment.Version.ToString().ShouldBe(expected: "10.0.15");
        int[] seeds = [0, 1, int.MinValue, int.MaxValue];
        for (int index = 0; index < seeds.Length; index++)
        {
            var random = new Random(Seed: seeds[index]);
            var replay = new ReplayRandom(seed: seeds[index], runtimeContract: ReplayRandom.PinnedRuntimeContract,
                    drawPosition: 0);
            long[] expected = index == 1 ? _seedOne : _seedZero;
            for (int draw = 0; draw < 5; draw++)
            {
                random.NextInt64(minValue: 0, maxValue: long.MaxValue).ShouldBe(expected: expected[draw]);
                replay.Draw().ShouldBe(expected: expected[draw]);
                var restored = new ReplayRandom(seed: seeds[index], runtimeContract: ReplayRandom.PinnedRuntimeContract,
                        drawPosition: draw);
                restored.Draw().ShouldBe(expected: expected[draw]);
                restored.DrawPosition.ShouldBe(expected: draw + 1);
            }
        }

        ProofProtocol.Record(id: "state.golden-vectors", diagnostic: "PINNED_GOLDEN_VECTORS",
                location: "src/godot-game/core/ReplayRandom.cs:Draw; runtime=10.0.15; seeds=0,1,int.MinValue,int.MaxValue");
    }

    /// <summary>Restoring an explicit saved position continues the observed runtime stream.</summary>
    [Fact, Trait("Case", "state.numeric-and-random")]
    public void Replay_RestoredPosition_ContinuesExactStream()
    {
        var restored = new ReplayRandom(seed: 0, runtimeContract: "dotnet-10.0.15/next-int64-v1", drawPosition: 2);
        restored.Draw().ShouldBe(expected: 9016307100457696812);
        restored.DrawPosition.ShouldBe(expected: 3);
        var identity = new StableIdentity(value: "preserved");
        var root = new ExampleState(elapsedMilliseconds: 9007199254740993, ordered: [identity],
                records: [new(key: identity, value: long.MaxValue)]);
        ExampleState advanced = root.Advance(elapsed: new ControlledElapsed(milliseconds: 1), random: restored);
        advanced.ElapsedMilliseconds.ShouldBe(expected: 9007199254740994);
        advanced.Records[identity].ShouldBe(expected: 4310211293818176047);
        advanced.Ordered[0].ShouldBeSameAs(expected: identity);
        restored.DrawPosition.ShouldBe(expected: 4);
        root.Records[identity].ShouldBe(expected: long.MaxValue);
        ProofProtocol.Record(id: "state.numeric-and-random", diagnostic: "BOUNDED_REPLAY",
                location: "src/godot-game/core/ReplayRandom.cs:Draw");
    }

    /// <summary>Checks lower/upper positions, exact exhaustion, contract rejection and old-root preservation.</summary>
    [Fact, Trait("Case", "state.random-limits")]
    public void Replay_PositionNeighbors_RejectsBeforeReplacement()
    {
        Should.Throw<ArgumentOutOfRangeException>(actual: () => Create(position: -1));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => Create(position: 1_000_001));
        foreach (string contract in new[] { "", "dotnet-10.0.15/next-int64-v2", "DOTNET-10.0.15/next-int64-v1",
            " dotnet-10.0.15/next-int64-v1", "dotnet-10.0.14/next-int64-v1" })
        {
            Should.Throw<ArgumentException>(actual: () => new ReplayRandom(seed: 0, runtimeContract: contract,
                    drawPosition: 0));
        }

        ReplayRandom last = Create(position: 999_999);
        ReplayRandom exhausted = Create(position: 1_000_000);
        long value = last.Draw();
        value.ShouldBeGreaterThanOrEqualTo(expected: 0);
        value.ShouldBeLessThan(expected: long.MaxValue);
        last.DrawPosition.ShouldBe(expected: 1_000_000);
        Should.Throw<InvalidOperationException>(actual: () => last.Draw());
        Should.Throw<InvalidOperationException>(actual: () => exhausted.Draw());
        exhausted.DrawPosition.ShouldBe(expected: 1_000_000);
        var root = new ExampleState(elapsedMilliseconds: 5, ordered: [], records: []);
        root.Advance(elapsed: new ControlledElapsed(milliseconds: 0), random: exhausted).ShouldBeSameAs(expected: root);
        Should.Throw<InvalidOperationException>(actual: () => root.Advance(
                elapsed: new ControlledElapsed(milliseconds: 1), random: exhausted));
        root.ElapsedMilliseconds.ShouldBe(expected: 5);
        exhausted.DrawPosition.ShouldBe(expected: 1_000_000);
        ProofProtocol.Record(id: "state.random-limits", diagnostic: "REPLAY_RANGE_REJECTED",
                location: "src/godot-game/core/ReplayRandom.cs:ReplayRandom/Draw");
    }

    /// <summary>Rejects fractional/nonfinite/overflow text at the actual integer-only parsing boundary.</summary>
    [Fact, Trait("Case", "state.numeric-text")]
    public void Parse_IntegerNeighbors_PreservesExactPrecision()
    {
        foreach (long valid in new[] { 0L, 1L, 9007199254740993L, long.MaxValue })
        {
            ExampleState.ParseMilliseconds(text: valid.ToString(provider: CultureInfo.InvariantCulture))
                    .ShouldBe(expected: valid);
        }

        foreach (string invalid in new[] { "", "-1", "9223372036854775808", "1.0", "0.5", "NaN", "Infinity",
            "-Infinity", "1e3", "+1", " 1", "1 ", "１", "١" })
        {
            Should.Throw<FormatException>(actual: () => ExampleState.ParseMilliseconds(text: invalid));
        }

        var root = new ExampleState(elapsedMilliseconds: 9007199254740993, ordered: [], records: []);
        var random = new ControlledRandom(value: 1);
        Should.Throw<FormatException>(actual: () => root.Advance(elapsed: new ControlledElapsed(milliseconds:
                ExampleState.ParseMilliseconds(text: "9223372036854775808")), random: random));
        root.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        random.DrawPosition.ShouldBe(expected: 0);
        ProofProtocol.Record(id: "state.numeric-text", diagnostic: "INTEGER_TEXT_BOUNDARY",
                location: "src/godot-game/core/ExampleState.cs:ParseMilliseconds");
    }

    /// <summary>Rejects Unknown and undefined kinds while retaining the exact stable enum identities.</summary>
    [Fact, Trait("Case", "state.kind")]
    public void Construct_UnknownKind_RejectsSavedSentinel()
    {
        ((int)ProofKind.Unknown).ShouldBe(expected: 0);
        ((int)ProofKind.Sample).ShouldBe(expected: 1);
        Enum.GetNames<ProofKind>().ShouldBe(expected: ["Unknown", "Sample"]);
        foreach (ProofKind invalid in new[] { ProofKind.Unknown, (ProofKind)(-1), (ProofKind)2 })
        {
            Should.Throw<ArgumentOutOfRangeException>(actual: () => new ExampleState(elapsedMilliseconds: 0,
                    ordered: [], records: [], kind: invalid));
        }

        new ExampleState(elapsedMilliseconds: 0, ordered: [], records: [], kind: ProofKind.Sample)
                .Kind.ShouldBe(expected: ProofKind.Sample);
        ProofProtocol.Record(id: "state.kind", diagnostic: "EXPLICIT_KIND_SENTINEL",
                location: "src/godot-game/core/ProofKind.cs; src/godot-game/core/ExampleState.cs:ExampleState");
    }

    /// <summary>Requires the real production adapter boundary and resolved adjacent/alias negatives.</summary>
    [Fact, Trait("Case", "state.random-scope")]
    public void Symbols_ExactAdapterBoundary_RejectsAdjacentUses()
    {
        var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: ProjectEvaluator.Expected[0]);
        CSharpCompilation actual = consumer.Create();
        actual.GetDiagnostics(cancellationToken: TestContext.Current.CancellationToken)
                .Where(predicate: diagnostic => diagnostic.Severity is DiagnosticSeverity.Error)
                .ShouldBeEmpty();
        ControlledInputPolicy.Validate(compilation: actual).ShouldBeEmpty();
        VerifyProductUses();
        foreach ((string Path, string Source) subject in ScopeMutations())
        {
            CSharpCompilation compilation = Compilation(trees: [consumer.Parse(source: subject.Source,
                    path: subject.Path)]);
            compilation.GetDiagnostics(cancellationToken: TestContext.Current.CancellationToken)
                    .Where(predicate: diagnostic => diagnostic.Severity is DiagnosticSeverity.Error).ShouldBeEmpty();
            ControlledInputPolicy.Validate(compilation: compilation).ShouldNotBeEmpty(customMessage: subject.Path);
        }

        ProofProtocol.Record(id: "state.random-scope", diagnostic: "RANDOM_SCOPE",
                location: "src/godot-game/core/ReplayRandom.cs; adjacent resolved Subject.cs mutations");
    }

    /// <summary>Requires actual banned-API build diagnostics for all uncontrolled clock/global random forms.</summary>
    [Fact, Trait("Case", "state.uncontrolled")]
    public void Build_UncontrolledSources_ReportsBannedApi()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        string[] expressions = ["System.DateTime.Now.Ticks", "System.DateTimeOffset.UtcNow.Ticks",
            "System.Environment.TickCount", "System.Environment.TickCount64",
            "System.Diagnostics.Stopwatch.GetTimestamp()", "System.Random.Shared.NextInt64()",
            "new System.Random().NextInt64()"];
        var evidence = new List<string>();
        foreach (string expression in expressions)
        {
            fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "int Read()", newValue: "long Read()",
                    comparisonType: StringComparison.Ordinal).Replace(oldValue: "return 1;",
                            newValue: "return " + expression + ";", comparisonType: StringComparison.Ordinal));
            (int Exit, string Output) result = fixture.Build();
            BuildFixture.HasDiagnostic(result: result, id: "RS0030", line: 8, column: 16)
                    .ShouldBeTrue(customMessage: result.Output);
            evidence.Add(item: expression + " => " + fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(8,16)");
        }

        ProofProtocol.Record(id: "state.uncontrolled", diagnostic: "RS0030",
                location: string.Join(separator: "\n", values: evidence));
    }

    /// <summary>Proves controlled integer time without a wall-clock dependency.</summary>
    [Fact, Trait("Case", "R-73.positive")]
    public void Rule73_ElapsedIntegers_PreservesExactTime()
    {
        var root = new ExampleState(elapsedMilliseconds: 9007199254740993, ordered: [], records: []);
        ExampleState next = root.Advance(elapsed: new ControlledElapsed(milliseconds: 1),
                random: new ControlledRandom(value: 1));
        next.ElapsedMilliseconds.ShouldBe(expected: 9007199254740994);
        root.ElapsedMilliseconds.ShouldBe(expected: 9007199254740993);
        ProofProtocol.Record(id: "R-73.positive", diagnostic: "CONTROLLED_INTEGER_TIME",
                location: "src/godot-game/core/ExampleState.cs:Advance");
    }

    /// <summary>Rejects a resolved clock alias in a real otherwise compliant production consumer.</summary>
    [Fact, Trait("Case", "R-73.negative")]
    public void Rule73_UncontrolledTime_ReportsBannedApi()
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        fixture.Build().Exit.ShouldBe(expected: 0);
        fixture.Write(source: "using Clock = System.DateTime;\n\n" + BuildFixture.CleanSource.Replace(
                oldValue: "return 1;", newValue: "return Clock.Now.Millisecond;",
                comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) result = fixture.Build();
        BuildFixture.HasDiagnostic(result: result, id: "RS0030", line: 10, column: 16)
                .ShouldBeTrue(customMessage: result.Output);
        ProofProtocol.Record(id: "R-73.negative", diagnostic: "RS0030",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs(10,16)");
    }

    /// <summary>Preserves the controlled continuation and exact IDs in independent and restored operations.</summary>
    [Fact, Trait("Case", "ac-18.complete")]
    public void Acceptance_ControlledReplay_PreservesExactContinuation()
    {
        var id = new StableIdentity(value: "e\u0301 😀 ");
        var root = new ExampleState(elapsedMilliseconds: 0, ordered: [id], records: [new(key: id, value: 0)]);
        ReplayRandom first = Create(position: 0);
        ReplayRandom independent = Create(position: 0);
        ExampleState left = root.Advance(elapsed: new ControlledElapsed(milliseconds: 1), random: first);
        ExampleState right = root.Advance(elapsed: new ControlledElapsed(milliseconds: 1), random: independent);
        left.Records[id].ShouldBe(expected: 7083764782846131554);
        right.Records[id].ShouldBe(expected: left.Records[id]);
        left.Ordered[0].ShouldBeSameAs(expected: id);
        right.Ordered[0].ShouldBeSameAs(expected: id);
        var restored = new ReplayRandom(seed: first.Seed, runtimeContract: first.RuntimeContract,
                drawPosition: first.DrawPosition);
        restored.Draw().ShouldBe(expected: 5154802594370149492);
        independent.Draw().ShouldBe(expected: 5154802594370149492);
        first.Draw().ShouldBe(expected: 5154802594370149492);
        ProofProtocol.Record(id: "ac-18.complete", diagnostic: "CONTROLLED_CONTINUATION",
                location: "src/godot-game/core/ExampleState.cs:Advance; src/godot-game/core/ReplayRandom.cs:Draw");
    }

    /// <summary>Uses integer-only input and checked addition with no precision loss or partial replacement.</summary>
    [Fact, Trait("Case", "ac-19.complete")]
    public void Acceptance_IntegerTime_RejectsInvalidNeighbors()
    {
        long maximum = ExampleState.ParseMilliseconds(text: "9223372036854775807");
        maximum.ShouldBe(expected: long.MaxValue);
        var root = new ExampleState(elapsedMilliseconds: maximum, ordered: [], records: []);
        var random = new ControlledRandom(value: 0);
        root.Advance(elapsed: new ControlledElapsed(milliseconds: 0), random: random).ShouldBeSameAs(expected: root);
        Should.Throw<OverflowException>(actual: () => root.Advance(
                elapsed: new ControlledElapsed(milliseconds: 1), random: random));
        Should.Throw<FormatException>(actual: () => ExampleState.ParseMilliseconds(text: "9223372036854775808"));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => root.Advance(
                elapsed: new ControlledElapsed(milliseconds: -1), random: random));
        random.DrawPosition.ShouldBe(expected: 0);
        root.ElapsedMilliseconds.ShouldBe(expected: maximum);
        ProofProtocol.Record(id: "ac-19.complete", diagnostic: "INTEGER_TIME_ACCEPTANCE",
                location: "src/godot-game/core/ExampleState.cs:ParseMilliseconds/Advance");
    }

    /// <summary>Checks the inclusive integer time/random bound and their immediately invalid neighbors.</summary>
    [Fact, Trait("Case", "edge.rule-07.boundary")]
    public void Boundary_TimeAndPosition_RejectsNearestInvalid()
    {
        ExampleState.ParseMilliseconds(text: "0").ShouldBe(expected: 0);
        ExampleState.ParseMilliseconds(text: "1").ShouldBe(expected: 1);
        ExampleState.ParseMilliseconds(text: "9223372036854775807").ShouldBe(expected: long.MaxValue);
        Should.Throw<FormatException>(actual: () => ExampleState.ParseMilliseconds(text: "-1"));
        Should.Throw<FormatException>(actual: () => ExampleState.ParseMilliseconds(text: "9223372036854775808"));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => Create(position: -1));
        Should.Throw<ArgumentOutOfRangeException>(actual: () => Create(position: ReplayRandom.MaxDrawPosition + 1));
        ReplayRandom limit = Create(position: ReplayRandom.MaxDrawPosition);
        Should.Throw<InvalidOperationException>(actual: () => limit.Draw());
        limit.DrawPosition.ShouldBe(expected: ReplayRandom.MaxDrawPosition);
        ProofProtocol.Record(id: "edge.rule-07.boundary", diagnostic: "STATE_INTEGER_BOUNDARIES",
                location: "src/godot-game/core/ExampleState.cs:ParseMilliseconds; src/godot-game/core/ReplayRandom.cs:ReplayRandom/Draw");
    }

    /// <summary>Rejects incompatible names, numbers and order under the pinned enum map.</summary>
    [Fact, Trait("Case", "state.enum-evolution")]
    public void Enum_StableMap_RejectsIncompatibleEvolution()
    {
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/core/ProofKind.cs"));
        const string Selector = "KingdomZombies.Core.ProofKind";
        ArchitecturePolicy.Enum(source: source, selector: Selector, rule: "R-77").ShouldBeEmpty();
        const string Header = "using System.Text.Json.Serialization; namespace KingdomZombies.Core; ";
        const string Removed = Header + "internal enum ProofKind {"
                + " [JsonStringEnumMemberName(name: \"unknown\")] Unknown = 0 }";
        const string Reordered = Header + "internal enum ProofKind {"
                + " [JsonStringEnumMemberName(name: \"sample\")] Sample = 1,"
                + " [JsonStringEnumMemberName(name: \"unknown\")] Unknown = 0 }";
        ArchitecturePolicy.Enum(source: Removed, selector: Selector, rule: "R-77").ShouldNotBeEmpty();
        ArchitecturePolicy.Enum(source: Reordered, selector: Selector, rule: "R-77").ShouldNotBeEmpty();
        string[] mutations = [source.Replace(oldValue: "Sample = 1", newValue: "Renamed = 1",
                comparisonType: StringComparison.Ordinal), source.Replace(oldValue: "Sample = 1",
                        newValue: "Sample = 2", comparisonType: StringComparison.Ordinal),
            source.Replace(oldValue: "name: \"sample\"", newValue: "name: \"renamed\"",
                    comparisonType: StringComparison.Ordinal), source.Replace(oldValue: "Unknown = 0",
                            newValue: "Unknown = 1", comparisonType: StringComparison.Ordinal),
            source.Replace(oldValue: "Unknown = 0", newValue: "Sample = 0",
                    comparisonType: StringComparison.Ordinal).Replace(oldValue: "Sample = 1",
                            newValue: "Unknown = 1", comparisonType: StringComparison.Ordinal)];
        foreach (string mutation in mutations)
        {
            ArchitecturePolicy.Enum(source: mutation, selector: Selector, rule: "R-77").ShouldNotBeEmpty();
        }

        string additive = source.Replace(oldValue: "Sample = 1", newValue: "Sample = 1,\n"
                + "    [JsonStringEnumMemberName(name: \"additional\")] Additional = 2",
                comparisonType: StringComparison.Ordinal);
        ArchitecturePolicy.Enum(source: additive, selector: Selector, rule: "R-77").ShouldBeEmpty();
        ProofProtocol.Record(id: "state.enum-evolution", diagnostic: "PINNED_ENUM_EVOLUTION",
                location: "src/godot-game/core/ProofKind.cs; src/godot-game/tests/verification/ArchitecturePolicy.cs:ValidEvolution");
    }

    private static readonly long[] _seedZero = [7083764782846131554, 5154802594370149492,
        9016307100457696812, 4310211293818176047, 9058748489721462462];
    private static readonly long[] _seedOne = [4307412542716114199, 3991716777541808379,
        934021439863608487, 2287661736829161214, 6291475812869357143];
    private static ReplayRandom Create(long position)
    {
        return new ReplayRandom(seed: 0, runtimeContract: ReplayRandom.PinnedRuntimeContract, drawPosition: position);
    }

    private static CSharpCompilation Compilation(SyntaxTree[] trees)
    {
        return CSharpCompilation.Create(assemblyName: "RandomBoundary", syntaxTrees: trees,
                references: AnalyzerVerifier.References(), options: new CSharpCompilationOptions(
                        outputKind: OutputKind.DynamicallyLinkedLibrary,
                        nullableContextOptions: NullableContextOptions.Enable));
    }

    private static IEnumerable<(string Path, string Source)> ScopeMutations()
    {
        const string Prefix = "using R = System.Random; namespace KingdomZombies.Core; ";
        const string Adjacent = Prefix + "internal static class Adjacent { internal static long Read()"
                + " { return new R(Seed: 0).NextInt64(minValue: 0, maxValue: long.MaxValue); } }";
        yield return ("src/godot-game/core/ReplayRandom.cs", Adjacent);
        yield return ("src/godot-game/core/Adjacent.cs", Adjacent);
        yield return ("src/godot-game/core/ReplayRandom.cs", Prefix + "internal sealed class ReplayRandom {"
                + " internal ReplayRandom() { R random = new(Seed: 0); } }");
        yield return ("src/godot-game/core/ReplayRandom.cs", Prefix + "internal static class ReplayRandom {"
                + " internal static R Draw() { return R.Shared; } }");
        yield return ("src/godot-game/core/Other.cs", Prefix + "internal sealed class ReplayRandom {"
                + " internal ReplayRandom(int seed) { R random = new(Seed: seed); } }");
    }

    private static void VerifyProductUses()
    {
        foreach (string project in ProjectEvaluator.Expected.Take(count: 3))
        {
            var consumer = ConsumerCompilation.Read(root: ProofProtocol.Root, project: project);
            CSharpCompilation compilation = consumer.Create();
            ConsumerCompilation.RequireValid(compilation: compilation);
            ControlledInputPolicy.Validate(compilation: compilation).ShouldBeEmpty();
        }
    }
}
