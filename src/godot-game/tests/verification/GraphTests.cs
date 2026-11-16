using System.Collections.Generic;
using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves evaluated allowed and forbidden dependency graphs using real compiled subjects.</summary>
public sealed class GraphTests
{
    /// <summary>Requires production output policy to reject a compilable unintended executable.</summary>
    [Fact, Trait("Case", "R-52.negative")]
    public void Evaluate_CoreExecutable_ReportsOutputKind()
    {
        var fixture = new GraphFixture();
        string core = Path.Combine(path1: fixture.Root, path2: ProjectEvaluator.Expected[0]);
        File.WriteAllText(path: core, contents: File.ReadAllText(path: core).Replace(oldValue: "</Project>",
                newValue: "<PropertyGroup><OutputType>Exe</OutputType></PropertyGroup></Project>",
                comparisonType: System.StringComparison.Ordinal));
        string source = "namespace KingdomZombies.Core;\n\ninternal static class Entry\n{\n"
                + "    internal static int Main()\n    {\n        return 0;\n    }\n}\n";
        File.WriteAllText(path: Path.Combine(path1: fixture.Root, path2: "src/godot-game/core/Entry.cs"), contents: source);
        (int Exit, string Output) built = fixture.Run("build", core, "--no-restore", "--disable-build-servers",
                "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        ProjectEvaluator.Validate(root: fixture.Root).ShouldContain(elementPredicate: error => error.Contains(
                value: "GRAPH_OUTPUT_KIND", comparisonType: System.StringComparison.Ordinal));
        ProofProtocol.Record(id: "R-52.negative", diagnostic: "GRAPH_OUTPUT_KIND", location: core);
    }

    /// <summary>Requires an intended graph failure after a clean and a violating subject both compile.</summary>
    [Fact, Trait("Case", "graph.allowed-and-direct")]
    public void Evaluate_CoreEngineReference_ReportsForbiddenEdge()
    {
        ProjectEvaluator.Validate(root: ProofProtocol.Root).ShouldBeEmpty();
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        TypeBoundaryScanner.RequireType(assembly: Path.Combine(path1: fixture.Root,
                path2: "src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll"),
                expectedType: "KingdomZombies.Foundation.FoundationRoot").ShouldBeEmpty();
        IReadOnlyList<string> errors = ProjectEvaluator.Validate(root: fixture.Root);
        errors.ShouldContain(elementPredicate: error => IsDirectViolation(error: error));
        ProofProtocol.Record(id: "graph.allowed-and-direct", diagnostic: "GRAPH_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    /// <summary>Requires the compiled type mechanism to reject a real engine type in Core.</summary>
    [Fact, Trait("Case", "ac-13.complete")]
    public void Inspect_CoreEngineType_ReportsCompiledLeak()
    {
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        string source = "namespace KingdomZombies.Core;\n"
                + "/// <summary>Compiled type boundary probe.</summary>\ninternal static class Subject\n{\n"
                + "    internal static string Read()\n    {\n"
                + "        return typeof(KingdomZombies.Proofs.Phase1).FullName ?? string.Empty;\n    }\n}\n";
        File.WriteAllText(path: Path.Combine(path1: fixture.Root, path2: "src/godot-game/core/Subject.cs"), contents: source);
        (int Exit, string Output) built = fixture.Run("build", ProjectEvaluator.Expected[0], "--no-restore",
                "--disable-build-servers", "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        IReadOnlyList<string> errors = TypeBoundaryScanner.Validate(assembly: Path.Combine(path1: fixture.Root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                expectedType: "KingdomZombies.Core.ProofValue", boundary: "core");
        errors.ShouldContain(elementPredicate: error => error.Contains(value: "TYPE_FORBIDDEN",
                comparisonType: System.StringComparison.Ordinal));
        ProofProtocol.Record(id: "ac-13.complete", diagnostic: "TYPE_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    /// <summary>Requires the real props edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.props")]
    public void Evaluate_PropsReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.props", mutation: "props");
    }

    /// <summary>Requires the real targets edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.targets")]
    public void Evaluate_TargetsReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.targets", mutation: "targets");
    }

    /// <summary>Requires the real wildcard edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.wildcard")]
    public void Evaluate_WildcardReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.wildcard", mutation: "wildcard");
    }

    /// <summary>Requires the real semicolon edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.semicolon")]
    public void Evaluate_SemicolonReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.semicolon", mutation: "semicolon");
    }

    /// <summary>Requires the real transitive edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.transitive")]
    public void Evaluate_TransitiveReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.transitive", mutation: "transitive");
    }

    /// <summary>Requires the real undeclared edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.undeclared")]
    public void Evaluate_UndeclaredReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.undeclared", mutation: "undeclared");
    }

    /// <summary>Requires the real duplicate edge to compile before its evaluated graph failure.</summary>
    [Fact, Trait("Case", "graph.duplicate")]
    public void Evaluate_DuplicateReference_ReportsBoundary()
    {
        VerifyGraphMutation(id: "graph.duplicate", mutation: "duplicate");
    }

    /// <summary>Rejects an actual compiled base dependency token.</summary>
    [Fact, Trait("Case", "graph.type-base")]
    public void Inspect_BaseLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-base", family: "base");
    }

    /// <summary>Rejects an actual compiled interface dependency token.</summary>
    [Fact, Trait("Case", "graph.type-interface")]
    public void Inspect_InterfaceLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-interface", family: "interface");
    }

    /// <summary>Rejects an actual compiled generic dependency token.</summary>
    [Fact, Trait("Case", "graph.type-generic")]
    public void Inspect_GenericLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-generic", family: "generic");
    }

    /// <summary>Rejects an actual compiled attribute dependency token.</summary>
    [Fact, Trait("Case", "graph.type-attribute")]
    public void Inspect_AttributeLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-attribute", family: "attribute");
    }

    /// <summary>Rejects an actual compiled signature dependency token.</summary>
    [Fact, Trait("Case", "graph.type-signature")]
    public void Inspect_SignatureLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-signature", family: "signature");
    }

    /// <summary>Rejects an actual compiled field dependency token.</summary>
    [Fact, Trait("Case", "graph.type-field")]
    public void Inspect_FieldLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-field", family: "field");
    }

    /// <summary>Rejects an actual compiled body dependency token.</summary>
    [Fact, Trait("Case", "graph.type-body")]
    public void Inspect_BodyLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-body", family: "body");
    }

    /// <summary>Rejects an actual compiled lambda dependency token.</summary>
    [Fact, Trait("Case", "graph.type-lambda")]
    public void Inspect_LambdaLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-lambda", family: "lambda");
    }

    /// <summary>Rejects an actual compiled async dependency token.</summary>
    [Fact, Trait("Case", "graph.type-async")]
    public void Inspect_AsyncLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-async", family: "async");
    }

    /// <summary>Rejects an actual compiled iterator dependency token.</summary>
    [Fact, Trait("Case", "graph.type-iterator")]
    public void Inspect_IteratorLeak_ReportsCompiledBoundary()
    {
        VerifyTypeMutation(id: "graph.type-iterator", family: "iterator");
    }

    /// <summary>Requires all five independent expected project and compiled type identities.</summary>
    [Fact, Trait("Case", "ac-12.complete")]
    public void Evaluate_DeclaredProjectsAndTypes_Accepted()
    {
        ProjectEvaluator.Validate(root: ProofProtocol.Root).ShouldBeEmpty();
        TypeBoundaryScanner.ValidateRepository(root: ProofProtocol.Root).ShouldBeEmpty();
        ProofProtocol.Record(id: "ac-12.complete", diagnostic: "GRAPH_ALLOWED");
    }

    /// <summary>Compiles real game composition through independent Core and service-port contracts.</summary>
    [Fact, Trait("Case", "graph.allowed-composition")]
    public void Build_ExplicitCoreAndPortComposition_Accepted()
    {
        var fixture = new GraphFixture();
        fixture.Write(relative: "src/godot-game/game/CompositionProbe.cs", content: "using KingdomZombies.Core;\n"
                + "using KingdomZombies.Multiplayer.Abstractions;\n\nnamespace Boundary;\n\n"
                + "internal static class CompositionProbe\n{\n"
                + "    internal static ProofValue Read(IProofService service, int value)\n    {\n"
                + "        return new ProofValue(value: service.Echo(value: value));\n    }\n}\n");
        fixture.Compile(project: ProjectEvaluator.Expected[2]);
        ProjectEvaluator.Validate(root: fixture.Root).ShouldBeEmpty();
        ProofProtocol.Record(id: "graph.allowed-composition", diagnostic: "GRAPH_ALLOWED", location: fixture.Root);
    }

    /// <summary>Rejects absent and duplicated type selectors before any set conversion.</summary>
    [Fact, Trait("Case", "edge.rule-05.empty")]
    public void Inspect_AbsentAndDuplicateSelectors_ReportsMissingSubject()
    {
        string assembly = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll");
        foreach (IReadOnlyList<string>? selectors in new IReadOnlyList<string>?[]
        {
            null, [], [""], ["A", "A"]
        })
        {
            TypeBoundaryScanner.Validate(assembly: assembly, expectedTypes: selectors, boundary: "core")
                    .ShouldContain(elementPredicate: error => error.StartsWith(value: "TYPE_SELECTOR",
                            comparisonType: System.StringComparison.Ordinal));
        }

        TypeBoundaryScanner.Validate(assembly: assembly, expectedType: "Absent.Subject", boundary: "core")
                .ShouldContain(elementPredicate: error => error.StartsWith(value: "TYPE_MISSING",
                        comparisonType: System.StringComparison.Ordinal));
        ArchitecturePolicy.Model(source: "namespace Proof;", selector: "Proof.Entity", rule: "R-63")
                .ShouldContain(elementPredicate: error => error.StartsWith(value: "MODEL_MISSING",
                        comparisonType: System.StringComparison.Ordinal));
        ProofProtocol.Record(id: "edge.rule-05.empty", diagnostic: "SUBJECT_REQUIRED");
    }

    /// <summary>Exercises the real R-51 compliant boundary.</summary>
    [Fact, Trait("Case", "R-51.positive")]
    public void Rule51_GraphControl_Accepted()
    {
        VerifyAllowed(id: "R-51.positive");
    }

    /// <summary>Exercises the real R-51 violating boundary.</summary>
    [Fact, Trait("Case", "R-51.negative")]
    public void Rule51_GraphViolation_Rejected()
    {
        VerifyGraphMutation(id: "R-51.negative", mutation: "undeclared");
    }

    /// <summary>Exercises the real R-52 compliant boundary.</summary>
    [Fact, Trait("Case", "R-52.positive")]
    public void Rule52_OutputControl_Accepted()
    {
        VerifyAllowed(id: "R-52.positive");
    }

    /// <summary>Exercises the real R-53 compliant boundary.</summary>
    [Fact, Trait("Case", "R-53.positive")]
    public void Rule53_CompilerControl_Accepted()
    {
        VerifyCompiler(id: "R-53.positive", negative: false);
    }

    /// <summary>Exercises the real R-53 violating boundary.</summary>
    [Fact, Trait("Case", "R-53.negative")]
    public void Rule53_CompilerViolation_Rejected()
    {
        VerifyCompiler(id: "R-53.negative", negative: true);
    }

    /// <summary>Proves a real game reference cannot supply its undeclared engine dependency to Core.</summary>
    [Fact, Trait("Case", "graph.transitive-compiler")]
    public void Build_UndeclaredTransitiveEngineType_ReportsCs0012()
    {
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        string core = Path.Combine(path1: fixture.Root, path2: ProjectEvaluator.Expected[0]);
        fixture.Write(relative: ProjectEvaluator.Expected[0], content: File.ReadAllText(path: core).Replace(
                oldValue: "<ProjectReference Include=\"../godot-game/KingdomZombies.csproj\"/>",
                newValue: "<Reference Include=\"KingdomZombies\"><HintPath>../godot-game/.godot/mono/temp/bin/Debug/"
                        + "KingdomZombies.dll</HintPath></Reference>",
                comparisonType: System.StringComparison.Ordinal));
        fixture.Compile(project: ProjectEvaluator.Expected[0], restore: true);
        fixture.Write(relative: "src/godot-game/core/Subject.cs", content: "namespace KingdomZombies.Core;\n\n"
                + "internal static class Subject\n{\n"
                + "    internal static int Read(KingdomZombies.Proofs.Phase1 value)\n    {\n"
                + "        return value.Name.ToString().Length;\n    }\n}\n");
        (int Exit, string Output) built = fixture.Run("build", ProjectEvaluator.Expected[0], "--no-restore",
                "--disable-build-servers", "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        built.Exit.ShouldNotBe(expected: 0);
        built.Output.ShouldContain(expected: "Subject.cs(7,22): error CS0012", customMessage: built.Output);
        built.Output.ShouldContain(expected: "'GodotSharp, Version=4.7.2.0");
        built.Output.ShouldNotContain(expected: "error MSB");
        built.Output.ShouldNotContain(expected: "error NU");
        ProofProtocol.Record(id: "graph.transitive-compiler", diagnostic: "CS0012", location: built.Output);
    }

    /// <summary>Exercises the real R-54 compliant boundary.</summary>
    [Fact, Trait("Case", "R-54.positive")]
    public void Rule54_AnalyzerControl_Accepted()
    {
        VerifyAllowed(id: "R-54.positive");
    }

    /// <summary>Exercises the real R-54 violating boundary.</summary>
    [Fact, Trait("Case", "R-54.negative")]
    public void Rule54_AnalyzerViolation_Rejected()
    {
        VerifyAnalyzerMetadata(id: "R-54.negative", field: "OutputItemType", value: "None");
    }

    /// <summary>Exercises the real R-55 compliant boundary.</summary>
    [Fact, Trait("Case", "R-55.positive")]
    public void Rule55_PortsControl_Accepted()
    {
        VerifyAllowed(id: "R-55.positive");
    }

    /// <summary>Exercises the real R-55 violating boundary.</summary>
    [Fact, Trait("Case", "R-55.negative")]
    public void Rule55_PortsViolation_Rejected()
    {
        BoundaryFixture.Port(id: "R-55.negative", dependency: "core");
    }

    /// <summary>Exercises the real R-56 compliant boundary.</summary>
    [Fact, Trait("Case", "R-56.positive")]
    public void Rule56_ContractControl_Accepted()
    {
        VerifyAllowed(id: "R-56.positive");
    }

    /// <summary>Exercises the real R-56 violating boundary.</summary>
    [Fact, Trait("Case", "R-56.negative")]
    public void Rule56_ContractViolation_Rejected()
    {
        BoundaryFixture.Port(id: "R-56.negative", dependency: "engine");
    }

    /// <summary>Exercises the real R-58 compliant boundary.</summary>
    [Fact, Trait("Case", "R-58.positive")]
    public void Rule58_PurityControl_Accepted()
    {
        VerifyAllowed(id: "R-58.positive");
    }

    /// <summary>Exercises the real R-58 violating boundary.</summary>
    [Fact, Trait("Case", "R-58.negative")]
    public void Rule58_PurityViolation_Rejected()
    {
        VerifyTypeMutation(id: "R-58.negative", family: "body");
    }

    /// <summary>Exercises the real R-59 compliant boundary.</summary>
    [Fact, Trait("Case", "R-59.positive")]
    public void Rule59_InfrastructureControl_Accepted()
    {
        VerifyAllowed(id: "R-59.positive");
    }

    /// <summary>Exercises the real R-59 violating boundary.</summary>
    [Fact, Trait("Case", "R-59.negative")]
    public void Rule59_InfrastructureViolation_Rejected()
    {
        BoundaryFixture.CoreProvider(id: "R-59.negative");
    }

    /// <summary>Exercises the real R-60 compliant boundary.</summary>
    [Fact, Trait("Case", "R-60.positive")]
    public void Rule60_LocatorControl_Accepted()
    {
        BoundaryFixture.Locator(id: "R-60.positive", negative: false);
    }

    /// <summary>Exercises the real R-60 violating boundary.</summary>
    [Fact, Trait("Case", "R-60.negative")]
    public void Rule60_LocatorViolation_Rejected()
    {
        BoundaryFixture.Locator(id: "R-60.negative", negative: true);
    }

    /// <summary>Exercises the real R-61 compliant boundary.</summary>
    [Fact, Trait("Case", "R-61.positive")]
    public void Rule61_ProviderControl_Accepted()
    {
        BoundaryFixture.Provider(id: "R-61.positive", negative: false);
    }

    /// <summary>Exercises the real R-61 violating boundary.</summary>
    [Fact, Trait("Case", "R-61.negative")]
    public void Rule61_ProviderViolation_Rejected()
    {
        BoundaryFixture.Provider(id: "R-61.negative", negative: true);
    }

    /// <summary>Exercises the real R-62 compliant boundary.</summary>
    [Fact, Trait("Case", "R-62.positive")]
    public void Rule62_ReusableControl_Accepted()
    {
        BoundaryFixture.Provider(id: "R-62.positive", negative: false, reusable: true);
    }

    /// <summary>Exercises the real R-62 violating boundary.</summary>
    [Fact, Trait("Case", "R-62.negative")]
    public void Rule62_ReusableViolation_Rejected()
    {
        BoundaryFixture.Provider(id: "R-62.negative", negative: true, reusable: true);
    }

    /// <summary>Exercises the real R-78 compliant boundary.</summary>
    [Fact, Trait("Case", "R-78.positive")]
    public void Rule78_EnumBoundaryControl_Accepted()
    {
        VerifyAllowed(id: "R-78.positive");
    }

    /// <summary>Exercises the real R-78 violating boundary.</summary>
    [Fact, Trait("Case", "R-78.negative")]
    public void Rule78_EnumBoundaryViolation_Rejected()
    {
        BoundaryFixture.Port(id: "R-78.negative", dependency: "enum");
    }

    /// <summary>Proves the adjacent analyzer edge needs both metadata fields.</summary>
    [Fact, Trait("Case", "edge.rule-05.adjacency")]
    public void Evaluate_AnalyzerAssemblyReference_ReportsForbidden()
    {
        VerifyAnalyzerMetadata(id: "edge.rule-05.adjacency", field: "ReferenceOutputAssembly", value: "true");
    }

    /// <summary>Requires missing policy subjects and selector mutations to fail before graph assertions.</summary>
    [Fact, Trait("Case", "graph.policy-selectors")]
    public void Evaluate_MalformedPolicySelectors_ReportsRequired()
    {
        var fixture = new GraphFixture();
        string path = Path.Combine(path1: fixture.Root, path2: "tools/phase1/projects.json");
        string original = File.ReadAllText(path: path);
        foreach (string mutation in new[] { "missing", "null", "empty", "duplicate" })
        {
            System.Text.Json.Nodes.JsonObject policy = System.Text.Json.Nodes.JsonNode.Parse(json: original)?.AsObject()
                    ?? throw new System.IO.InvalidDataException(message: "Missing policy");
            System.Text.Json.Nodes.JsonArray projects = policy["projects"]?.AsArray()
                    ?? throw new System.IO.InvalidDataException(message: "Missing projects");
            System.Text.Json.Nodes.JsonNode first = projects[0]
                    ?? throw new System.IO.InvalidDataException(message: "Missing first project");
            if (mutation is "missing")
            {
                first.AsObject().Remove(propertyName: "allowed");
            }
            else if (mutation is "null")
            {
                first["expectedTypes"] = null;
            }
            else if (mutation is "empty")
            {
                first["expectedTypes"] = new System.Text.Json.Nodes.JsonArray();
            }
            else
            {
                first["expectedTypes"] = new System.Text.Json.Nodes.JsonArray("A", "A");
            }

            File.WriteAllText(path: path, contents: policy.ToJsonString());
            ProjectEvaluator.Validate(root: fixture.Root).ShouldNotBeEmpty();
        }

        File.WriteAllText(path: path, contents: original);
        string core = Path.Combine(path1: fixture.Root, path2: ProjectEvaluator.Expected[0]);
        File.Move(sourceFileName: core, destFileName: core + ".missing");
        ProjectEvaluator.Validate(root: fixture.Root).ShouldContain(elementPredicate: error => error.StartsWith(
                value: "GRAPH_MISSING", comparisonType: System.StringComparison.Ordinal));

        ProofProtocol.Record(id: "graph.policy-selectors", diagnostic: "SUBJECT_REQUIRED", location: path);
    }

    /// <summary>Permutes policy and reference order while retaining the complete evaluated verdict.</summary>
    [Fact, Trait("Case", "edge.rule-05.ordering")]
    public void Evaluate_PermutedReferences_PreservesVerdict()
    {
        var fixture = new GraphFixture();
        fixture.AddBoundaryMutation(mutation: "semicolon");
        IReadOnlyList<string> before = ProjectEvaluator.Validate(root: fixture.Root);
        string core = Path.Combine(path1: fixture.Root, path2: ProjectEvaluator.Expected[0]);
        string game = "../godot-game/KingdomZombies.csproj";
        string ports = "../multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj";
        File.WriteAllText(path: core, contents: File.ReadAllText(path: core).Replace(oldValue: game + ";" + ports,
                newValue: ports + ";" + game, comparisonType: System.StringComparison.Ordinal));
        string path = Path.Combine(path1: fixture.Root, path2: "tools/phase1/projects.json");
        System.Text.Json.Nodes.JsonObject policy = System.Text.Json.Nodes.JsonNode.Parse(json:
                File.ReadAllText(path: path))?.AsObject()
                ?? throw new InvalidDataException(message: "Missing policy");
        System.Text.Json.Nodes.JsonArray projects = policy["projects"]?.AsArray()
                ?? throw new InvalidDataException(message: "Missing projects");
        var reversed = new System.Text.Json.Nodes.JsonArray();
        foreach (System.Text.Json.Nodes.JsonNode? project in System.Linq.Enumerable.Reverse(source: projects))
        {
            reversed.Add(item: project?.DeepClone());
        }

        policy["projects"] = reversed;
        File.WriteAllText(path: path, contents: policy.ToJsonString());
        fixture.Compile(project: ProjectEvaluator.Expected[0]);
        ProjectEvaluator.Validate(root: fixture.Root).ShouldBe(expected: before);
        string assembly = Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll");
        TypeBoundaryScanner.Validate(assembly: assembly, expectedTypes: ["Absent.B", "Absent.A"], boundary: "core")
                .ShouldBe(expected: TypeBoundaryScanner.Validate(assembly: assembly,
                        expectedTypes: ["Absent.A", "Absent.B"], boundary: "core"));
        ProofProtocol.Record(id: "edge.rule-05.ordering", diagnostic: "ORDER_INDEPENDENT",
                location: string.Join(separator: "\n", values: before));
    }

    private static void VerifyGraphMutation(string id, string mutation)
    {
        var fixture = new GraphFixture();
        fixture.AddBoundaryMutation(mutation: mutation);
        IReadOnlyList<string> errors = ProjectEvaluator.Validate(root: fixture.Root);
        errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "GRAPH_FORBIDDEN",
                comparisonType: System.StringComparison.Ordinal));
        errors.ShouldContain(elementPredicate: error => error.Contains(value: "defined by",
                comparisonType: System.StringComparison.Ordinal));
        if (mutation is "duplicate")
        {
            errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "GRAPH_DUPLICATE",
                    comparisonType: System.StringComparison.Ordinal));
        }

        ProofProtocol.Record(id: id, diagnostic: "GRAPH_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    private static void VerifyTypeMutation(string id, string family)
    {
        var fixture = new GraphFixture();
        fixture.AddDirectEngineEdge();
        fixture.Write(relative: "src/godot-game/game/LeakBase.cs", content: "namespace Boundary;\n"
                + "/// <summary>External base probe.</summary>\npublic abstract class LeakBase\n{\n}\n");
        fixture.Write(relative: "src/godot-game/game/ILeak.cs", content: "namespace Boundary;\n"
                + "/// <summary>External interface probe.</summary>\npublic interface ILeak\n{\n"
                + "    /// <summary>Returns a probe value.</summary>\n    int Read();\n}\n");
        fixture.Write(relative: "src/godot-game/game/LeakAttribute.cs", content: "using System;\n\nnamespace Boundary;\n"
                + "/// <summary>External attribute probe.</summary>\n"
                + "[AttributeUsage(validOn: AttributeTargets.Class)]\n"
                + "public sealed class LeakAttribute : Attribute\n{\n}\n");
        fixture.Compile(project: ProjectEvaluator.Expected[2]);
        fixture.Write(relative: "src/godot-game/core/Subject.cs", content: TypeSubject(family: family));
        fixture.Compile(project: ProjectEvaluator.Expected[0]);
        string assembly = Path.Combine(path1: fixture.Root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll");
        IReadOnlyList<string> errors = TypeBoundaryScanner.Validate(assembly: assembly,
                expectedType: "KingdomZombies.Core.ProofValue", boundary: "core");
        errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "TYPE_FORBIDDEN",
                comparisonType: System.StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: "TYPE_FORBIDDEN", location: family + "\n"
                + string.Join(separator: "\n", values: errors));
    }

    private static string TypeSubject(string family)
    {
        string type = "KingdomZombies.Proofs.Phase1";
        string content = family switch
        {
            "base" => "internal sealed class Subject : Boundary.LeakBase\n{\n}\n",
            "interface" => "internal sealed class Subject : Boundary.ILeak\n{\n"
                    + "    /// <summary>Implements the external contract.</summary>\n"
                    + "    public int Read()\n    {\n        return 1;\n    }\n}\n",
            "field" => "internal sealed class Subject\n{\n    internal Subject(" + type + "? value)\n"
                    + "    {\n        _value = value;\n    }\n    internal " + type + "? Value => _value;\n"
                    + "    private readonly " + type + "? _value;\n}\n",
            "generic" => StaticSubject(member: "internal static System.Collections.Generic.List<" + type
                    + "> Read()\n    {\n        return [];\n    }"),
            "signature" => StaticSubject(member: "internal static " + type + "? Read()\n"
                    + "    {\n        return null;\n    }"),
            "attribute" => "[Boundary.Leak]\ninternal static class Subject\n{\n}\n",
            "body" => StaticSubject(member: "internal static System.Type Read()\n    {\n        return typeof("
                    + type + ");\n    }"),
            "lambda" => StaticSubject(member: "internal static System.Func<System.Type> Read()\n"
                    + "    {\n        return () => typeof(" + type + ");\n    }"),
            "async" => StaticSubject(member: "internal static async System.Threading.Tasks.Task<System.Type> Read()\n"
                    + "    {\n        await System.Threading.Tasks.Task.CompletedTask.ConfigureAwait(\n"
                    + "                continueOnCapturedContext: false);\n"
                    + "        return typeof(" + type + ");\n    }"),
            "iterator" => StaticSubject(member: "internal static System.Collections.Generic.IEnumerable<System.Type> "
                    + "Read()\n    {\n        yield return typeof(" + type + ");\n    }"),
            _ => throw new System.ArgumentException(message: "Unknown compiled leak family", paramName: nameof(family))
        };
        return "namespace KingdomZombies.Core;\n\n/// <summary>Compiled boundary probe.</summary>\n" + content;
    }

    private static string StaticSubject(string member)
    {
        return "internal static class Subject\n{\n    " + member + "\n}\n";
    }

    private static void VerifyAllowed(string id)
    {
        ProjectEvaluator.Validate(root: ProofProtocol.Root).ShouldBeEmpty();
        TypeBoundaryScanner.ValidateRepository(root: ProofProtocol.Root).ShouldBeEmpty();
        ProofProtocol.Record(id: id, diagnostic: "BOUNDARY_ALLOWED",
                location: "Exact five projects and compiled subjects");
    }

    private static void VerifyCompiler(string id, bool negative)
    {
        var fixture = new BuildFixture();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN: " + clean.Output);
        if (negative)
        {
            fixture.Write(source: BuildFixture.CleanSource.Replace(oldValue: "return 1;",
                    newValue: "return typeof(Godot.Vector2).Name.Length;",
                    comparisonType: System.StringComparison.Ordinal));
            (int Exit, string Output) changed = fixture.Build();
            BuildFixture.HasDiagnostic(result: changed, id: "CS0246", line: 8, column: 23)
                    .ShouldBeTrue(customMessage: changed.Output);
        }

        ProofProtocol.Record(id: id, diagnostic: negative ? "CS0246" : "COMPILER_ALLOWED",
                location: fixture.DirectoryPath + "/src/godot-game/core/Subject.cs");
    }

    private static void VerifyAnalyzerMetadata(string id, string field, string value)
    {
        var fixture = new GraphFixture();
        string path = Path.Combine(path1: fixture.Root, path2: "src/godot-game/Directory.Build.targets");
        string expected = field is "OutputItemType" ? "Analyzer" : "false";
        File.WriteAllText(path: path, contents: File.ReadAllText(path: path).Replace(oldValue:
                field + "=\"" + expected + "\"", newValue: field + "=\"" + value + "\"",
                comparisonType: System.StringComparison.Ordinal));
        fixture.Compile(project: ProjectEvaluator.Expected[0]);
        IReadOnlyList<string> errors = ProjectEvaluator.Validate(root: fixture.Root);
        errors.ShouldContain(elementPredicate: error => error.StartsWith(value: "GRAPH_FORBIDDEN",
                comparisonType: System.StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: "GRAPH_FORBIDDEN",
                location: string.Join(separator: "\n", values: errors));
    }

    private static bool IsDirectViolation(string error)
    {
        bool rule = error.Contains(value: "GRAPH_FORBIDDEN", comparisonType: System.StringComparison.Ordinal);
        bool subject = error.Contains(value: "src/godot-game/core", comparisonType: System.StringComparison.Ordinal);
        bool dependency = error.Contains(value: "godot-game", comparisonType: System.StringComparison.Ordinal);
        return rule && subject && dependency;
    }
}
