using System;
using System.Collections.Generic;
using System.IO;
using KingdomZombies.Phase1.Tests.Models;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves concrete structural contracts without claiming qualitative design review.</summary>
public sealed class ArchitectureRuleTests
{
    /// <summary>Preserves default internal models and the exact declared observed public surfaces.</summary>
    [Fact, Trait("Case", "R-34.positive")]
    public void Rule34_DeclaredVisibility_Accepted()
    {
        ArchitecturePolicy.VisibilityRepository(root: ProofProtocol.Root).ShouldBeEmpty();
        ProofProtocol.Record(id: "R-34.positive", diagnostic: "VISIBILITY_ALLOWED");
    }

    /// <summary>Rejects an immediately adjacent public model under the exact visibility registry.</summary>
    [Fact, Trait("Case", "R-34.negative")]
    public void Rule34_AdjacentPublicModel_Rejected()
    {
        string source = File.ReadAllText(path: ModelPath(model: "BoundaryOptions")).Replace(
                oldValue: "internal sealed class", newValue: "public sealed class",
                comparisonType: StringComparison.Ordinal);
        var fixture = new BuildFixture();
        File.WriteAllText(path: Path.Combine(path1: fixture.DirectoryPath, path2: "src/core/BoundaryOptions.cs"),
                contents: source);
        fixture.Restore();
        (int Exit, string Output) built = fixture.Build();
        built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + built.Output);
        var consumer = ConsumerCompilation.Read(root: fixture.DirectoryPath, project: ProjectEvaluator.Expected[0]);
        ArchitecturePolicy.Visibility(root: ProofProtocol.Root, source: source,
                path: "tests/phase1/Models/BoundaryOptions.cs", options: consumer.Options).ShouldNotBeEmpty();
        ProofProtocol.Record(id: "R-34.negative", diagnostic: "R-34");
    }

    /// <summary>The real production scan rejects an undeclared public type outside the former four-file list.</summary>
    [Fact, Trait("Case", "graph.visibility-inventory")]
    public void Scan_ProductionVisibility_RejectsUnregisteredPublicType()
    {
        var fixture = new BuildFixture(game: true);
        string registry = "docs/code/visibility-exemptions.json";
        string destination = Path.Combine(path1: fixture.DirectoryPath, path2: registry);
        Directory.CreateDirectory(path: Path.GetDirectoryName(path: destination)
                ?? throw new InvalidDataException(message: "Visibility fixture directory required"));
        File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: registry), destFileName: destination);
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        ArchitecturePolicy.VisibilityRepository(root: fixture.DirectoryPath).ShouldBeEmpty();
        const string SourcePath = "src/core/ExampleState.cs";
        string file = Path.Combine(path1: fixture.DirectoryPath, path2: SourcePath);
        string source = File.ReadAllText(path: file).Replace(oldValue: "internal sealed class ExampleState",
                newValue: "#if DEBUG\npublic sealed class ExampleState\n#else\n"
                        + "internal sealed class ExampleState\n#endif",
                comparisonType: StringComparison.Ordinal);
        File.WriteAllText(path: file, contents: source);
        (int Exit, string Output) mutation = fixture.Build();
        mutation.Exit.ShouldBe(expected: 0, customMessage: mutation.Output);
        ArchitecturePolicy.VisibilityRepository(root: fixture.DirectoryPath).ShouldContain(expected:
                "R-34: undeclared public type KingdomZombies.Core.ExampleState in " + SourcePath,
                comparer: StringComparer.Ordinal);
        File.WriteAllText(path: file, contents: source.Replace(oldValue: "#if DEBUG", newValue: "#if !DEBUG",
                comparisonType: StringComparison.Ordinal));
        var consumer = ConsumerCompilation.Read(root: fixture.DirectoryPath, project: ProjectEvaluator.Expected[0]);
        ConsumerCompilation.RequireValid(compilation: consumer.Create());
        ArchitecturePolicy.VisibilityRepository(root: fixture.DirectoryPath).ShouldBeEmpty();
        ProofProtocol.Record(id: "graph.visibility-inventory", diagnostic: "VISIBILITY_INVENTORY_REJECTED",
                location: fixture.DirectoryPath + "/" + SourcePath + "; DEBUG/inverse, three production consumers");
    }

    /// <summary>Exercises R-63 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-63.positive")]
    public void Rule63_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-63.positive", rule: "R-63", negative: false);
    }

    /// <summary>Exercises R-63 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-63.negative")]
    public void Rule63_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-63.negative", rule: "R-63", negative: true);
    }

    /// <summary>Exercises R-64 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-64.positive")]
    public void Rule64_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-64.positive", rule: "R-64", negative: false);
    }

    /// <summary>Exercises R-64 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-64.negative")]
    public void Rule64_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-64.negative", rule: "R-64", negative: true);
    }

    /// <summary>Exercises R-65 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-65.positive")]
    public void Rule65_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-65.positive", rule: "R-65", negative: false);
    }

    /// <summary>Exercises R-65 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-65.negative")]
    public void Rule65_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-65.negative", rule: "R-65", negative: true);
    }

    /// <summary>Exercises R-66 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-66.positive")]
    public void Rule66_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-66.positive", rule: "R-66", negative: false);
    }

    /// <summary>Exercises R-66 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-66.negative")]
    public void Rule66_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-66.negative", rule: "R-66", negative: true);
    }

    /// <summary>Exercises R-74 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-74.positive")]
    public void Rule74_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-74.positive", rule: "R-74", negative: false);
    }

    /// <summary>Exercises R-74 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-74.negative")]
    public void Rule74_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-74.negative", rule: "R-74", negative: true);
    }

    /// <summary>Exercises R-75 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-75.positive")]
    public void Rule75_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-75.positive", rule: "R-75", negative: false);
    }

    /// <summary>Exercises R-75 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-75.negative")]
    public void Rule75_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-75.negative", rule: "R-75", negative: true);
    }

    /// <summary>Exercises R-77 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-77.positive")]
    public void Rule77_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-77.positive", rule: "R-77", negative: false);
    }

    /// <summary>Exercises R-77 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-77.negative")]
    public void Rule77_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-77.negative", rule: "R-77", negative: true);
    }

    /// <summary>Exercises R-81 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-81.positive")]
    public void Rule81_CompliantModel_Accepted()
    {
        VerifyModel(id: "R-81.positive", rule: "R-81", negative: false);
    }

    /// <summary>Exercises R-81 against the actual compiled declared proof model.</summary>
    [Fact, Trait("Case", "R-81.negative")]
    public void Rule81_ViolatingModel_Rejected()
    {
        VerifyModel(id: "R-81.negative", rule: "R-81", negative: true);
    }

    private static readonly string[] _models = ["BoundaryEntity", "BoundarySnapshotDto", "BoundaryOptions",
        "BoundaryMapper", "BoundaryKind"];
    private static void VerifyModel(string id, string rule, bool negative)
    {
        var fixture = new BuildFixture();
        foreach (string model in _models)
        {
            File.Copy(sourceFileName: ModelPath(model: model), destFileName: Path.Combine(path1: fixture.DirectoryPath,
                    path2: "src/core/" + model + ".cs"));
        }

        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN: " + clean.Output);
        string modelName = SelectedModel(rule: rule);
        string source = File.ReadAllText(path: ModelPath(model: modelName));
        if (negative)
        {
            source = Mutate(source: source, rule: rule);
            string target = Path.Combine(path1: fixture.DirectoryPath, path2: "src/core/" + modelName + ".cs");
            if (StringComparer.Ordinal.Equals(x: rule, y: "R-66"))
            {
                target = Path.Combine(path1: fixture.DirectoryPath, path2: ProjectEvaluator.Expected[0]);
                source = File.ReadAllText(path: target).Replace(oldValue: "</Project>", newValue:
                        "<PropertyGroup><Version>0.2.0</Version></PropertyGroup></Project>",
                        comparisonType: StringComparison.Ordinal);
            }

            File.WriteAllText(path: target, contents: source);
            (int Exit, string Output) changed = fixture.Build();
            changed.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + changed.Output);
        }

        IReadOnlyList<string> errors = CheckModel(source: source, model: modelName, rule: rule,
                root: fixture.DirectoryPath);
        if (negative)
        {
            errors.ShouldContain(elementPredicate: error => error.StartsWith(value: rule + ":",
                    comparisonType: StringComparison.Ordinal));
        }
        else
        {
            errors.ShouldBeEmpty();
            var entity = new BoundaryEntity(value: 1, kind: BoundaryKind.Ready);
            entity.Change(value: 2);
            BoundaryMapper.Snapshot(entity: entity).Value.ShouldBe(expected: 2);
            Should.Throw<ArgumentOutOfRangeException>(actual: () => new BoundaryEntity(value: 1,
                    kind: BoundaryKind.Unknown));
        }

        VerifyProduction(rule: rule, negative: negative);
        string production = rule is "R-63" ? "src/core/ExampleState.cs" : rule is "R-74" or "R-75" or "R-77"
                ? "src/core/ProofKind.cs" : "";
        ProofProtocol.Record(id: id, diagnostic: negative ? rule : "STRUCTURE_ALLOWED", location: ModelPath(
                model: modelName) + "\n" + production + "\n" + string.Join(separator: "\n", values: errors));
    }

    private static void VerifyProduction(string rule, bool negative)
    {
        if (rule is not ("R-63" or "R-64" or "R-74" or "R-75" or "R-77"))
        {
            return;
        }

        string type = rule switch { "R-63" => "ExampleState", "R-64" => "SnapshotDto", _ => "ProofKind" };
        string path = "src/core/" + type + ".cs";
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
        if (negative)
        {
            source = rule switch
            {
                "R-63" => source.Replace(oldValue: "{ get; }", newValue: "{ get; set; }",
                        comparisonType: StringComparison.Ordinal),
                "R-64" => source.Replace(oldValue: "{ get; }", newValue: "{ get; set; }",
                        comparisonType: StringComparison.Ordinal),
                "R-74" => source.Replace(oldValue: "Unknown = 0", newValue: "Unknown = -1",
                        comparisonType: StringComparison.Ordinal),
                "R-75" => source.Replace(oldValue: "name: \"sample\"", newValue: "name: \"unknown\"",
                        comparisonType: StringComparison.Ordinal),
                _ => source.Replace(oldValue: "Sample = 1", newValue: "Sample = 2",
                        comparisonType: StringComparison.Ordinal)
            };
            var fixture = new BuildFixture();
            File.WriteAllText(path: Path.Combine(path1: fixture.DirectoryPath, path2: path), contents: source);
            fixture.Restore();
            (int Exit, string Output) built = fixture.Build();
            built.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_PRODUCTION_MUTATION: " + built.Output);
        }

        string selector = "KingdomZombies.Core." + type;
        IReadOnlyList<string> errors = rule is "R-63" or "R-64" ? ArchitecturePolicy.Model(source: source,
                selector: selector, rule: rule) : ArchitecturePolicy.Enum(source: source,
                        selector: selector, rule: rule);
        if (negative)
        {
            errors.ShouldContain(elementPredicate: error => error.StartsWith(value: rule + ":",
                    comparisonType: StringComparison.Ordinal));
        }
        else
        {
            errors.ShouldBeEmpty();
        }
    }

    private static string ModelPath(string model)
    {
        return Path.Combine(path1: ProofProtocol.Root, path2: "tests/phase1/Models/" + model + ".cs");
    }

    private static string SelectedModel(string rule)
    {
        return rule switch
        {
            "R-63" => "BoundaryEntity",
            "R-64" => "BoundarySnapshotDto",
            "R-65" => "BoundaryMapper",
            "R-66" => "BoundaryOptions",
            "R-74" or "R-75" or "R-77" => "BoundaryKind",
            "R-81" => "BoundaryOptions",
            _ => throw new ArgumentException(message: "Unknown structural rule", paramName: nameof(rule))
        };
    }

    private static string Mutate(string source, string rule)
    {
        return rule switch
        {
            "R-63" or "R-64" or "R-81" => source.Replace(oldValue: "private set;", newValue: "set;",
                    comparisonType: StringComparison.Ordinal),
            "R-74" => source.Replace(oldValue: "Unknown = 0", newValue: "Unknown = -1",
                    comparisonType: StringComparison.Ordinal),
            "R-75" => source.Replace(oldValue: "name: \"ready\"", newValue: "name: \"unknown\"",
                    comparisonType: StringComparison.Ordinal),
            "R-77" => source.Replace(oldValue: "Ready = 1", newValue: "Ready = 2",
                    comparisonType: StringComparison.Ordinal),
            "R-65" => IndirectMapper(source: source),
            "R-66" => source,
            _ => throw new ArgumentException(message: "Unknown structural mutation", paramName: nameof(rule))
        };
    }

    private static string IndirectMapper(string source)
    {
        string original = "return new BoundarySnapshotDto(value: entity.Value, kind: \"ready\");";
        string indirect = source.Replace(oldValue: original, newValue: "return Copy(entity: entity);",
                comparisonType: StringComparison.Ordinal);
        int end = indirect.LastIndexOf(value: '}', startIndex: indirect.Length - 1);
        return indirect[..end] + "    private static BoundarySnapshotDto Copy(BoundaryEntity entity)\n"
                + "    {\n        " + original + "\n    }\n}\n";
    }

    private static IReadOnlyList<string> CheckModel(string source, string model, string rule, string root)
    {
        string selector = "KingdomZombies.Phase1.Tests.Models." + model;
        return rule switch
        {
            "R-65" => ArchitecturePolicy.Mapping(source: source, selector: "Snapshot"),
            "R-66" => ArchitecturePolicy.Version(root: root),
            "R-74" or "R-75" or "R-77" => ArchitecturePolicy.Enum(source: source, selector: selector, rule: rule),
            _ => ArchitecturePolicy.Model(source: source, selector: selector, rule: rule)
        };
    }
}

