using System;
using System.Collections.Generic;
using System.Text;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Follows actual module links and rejects independent declaration and reachability mutations.</summary>
public sealed class DocumentationProjectTests
{
    /// <summary>Traverses Core's policy link and reconciles five declarations with evaluated MSBuild items.</summary>
    [Fact, Trait("Case", "docs.projects")]
    public void Check_ActualProjectPages_MatchesEvaluatedReferences()
    {
        Dictionary<string, byte[]> files = Current();
        DocumentationValidator.Check(files: files, required: DocumentationValidator.Authored).ShouldBeEmpty();
        DocumentationProjects.Check(files: files, root: ProofProtocol.Root).ShouldBeEmpty();
        ProjectEvaluator.Validate(root: ProofProtocol.Root).ShouldBeEmpty();
        ProofProtocol.Record(id: "docs.projects", diagnostic: "DOC_PROJECTS",
                location: Core + " -> tools/phase1/projects.json -> five evaluated MSBuild projects");
    }

    /// <summary>Rejects an absent module page even with all project/configuration inputs present.</summary>
    [Fact, Trait("Case", "docs.projects.missing")]
    public void Check_RemovedCorePage_RejectsMissingDeclaration()
    {
        Dictionary<string, byte[]> files = Current();
        files.Remove(key: Core);
        Reject(files: files, id: "docs.projects.missing", diagnostic: "DOC_PROJECT_REQUIRED");
    }

    /// <summary>Rejects a whitespace-only module page as an absent machine-readable declaration.</summary>
    [Fact, Trait("Case", "docs.projects.empty")]
    public void Check_EmptyCorePage_RejectsEmptyDeclaration()
    {
        Dictionary<string, byte[]> files = Current();
        files[Core] = Encoding.UTF8.GetBytes(s: " \n");
        Reject(files: files, id: "docs.projects.empty", diagnostic: "DOC_PROJECT_DECLARATION");
    }

    /// <summary>Rejects a complete page whose actual canonical-index link has been removed.</summary>
    [Fact, Trait("Case", "docs.projects.unreachable")]
    public void Check_UnlinkedCorePage_RejectsUnreachableDeclaration()
    {
        Dictionary<string, byte[]> files = Current();
        Change(files: files, path: "docs/architecture/README.md", before: "[Core](../../src/core/docs/README.md)",
                after: "Core");
        Reject(files: files, id: "docs.projects.unreachable", diagnostic: "DOC_PROJECT_UNREACHABLE");
    }

    /// <summary>Rejects a changed production edge in an otherwise complete linked declaration.</summary>
    [Fact, Trait("Case", "docs.projects.edges")]
    public void Check_ExtraCoreEdge_RejectsPolicyMismatch()
    {
        Dictionary<string, byte[]> files = Current();
        Change(files: files, path: Core, before: "\"productionReferences\":[]",
                after: "\"productionReferences\":[\"src/godot-game/KingdomZombies.csproj\"]");
        Reject(files: files, id: "docs.projects.edges", diagnostic: "DOC_PROJECT_EDGES");
    }

    /// <summary>Rejects a runtime assembly-reference claim for the analyzer OutputItemType field.</summary>
    [Fact, Trait("Case", "docs.projects.output-item")]
    public void Check_AnalyzerOutputType_RejectsMetadataMismatch()
    {
        Dictionary<string, byte[]> files = Current();
        Change(files: files, path: Core, before: "\"OutputItemType\":\"Analyzer\"",
                after: "\"OutputItemType\":\"Reference\"");
        Reject(files: files, id: "docs.projects.output-item", diagnostic: "DOC_PROJECT_ANALYZER");
    }

    /// <summary>Rejects a changed ReferenceOutputAssembly field independently from OutputItemType.</summary>
    [Fact, Trait("Case", "docs.projects.reference-output")]
    public void Check_AnalyzerAssemblyFlag_RejectsMetadataMismatch()
    {
        Dictionary<string, byte[]> files = Current();
        Change(files: files, path: Core, before: "\"ReferenceOutputAssembly\":\"false\"",
                after: "\"ReferenceOutputAssembly\":\"true\"");
        Reject(files: files, id: "docs.projects.reference-output", diagnostic: "DOC_PROJECT_ANALYZER");
    }

    /// <summary>Rejects a duplicated policy block instead of silently selecting its first occurrence.</summary>
    [Fact, Trait("Case", "docs.projects.ambiguous")]
    public void Check_DuplicateDeclaration_RejectsAmbiguousPolicy()
    {
        Dictionary<string, byte[]> files = Current();
        string text = Encoding.UTF8.GetString(bytes: files[Core]);
        files[Core] = Encoding.UTF8.GetBytes(s: text + text[text.IndexOf(value: "```project-policy",
                comparisonType: StringComparison.Ordinal)..]);
        Reject(files: files, id: "docs.projects.ambiguous", diagnostic: "DOC_PROJECT_DECLARATION");
    }

    private const string Core = "src/core/docs/README.md";
    private static Dictionary<string, byte[]> Current()
    {
        return DocumentationValidator.ReadRepository();
    }

    private static void Change(Dictionary<string, byte[]> files, string path, string before, string after)
    {
        string text = Encoding.UTF8.GetString(bytes: files[path]);
        text.ShouldContain(expected: before);
        files[path] = Encoding.UTF8.GetBytes(s: text.Replace(oldValue: before, newValue: after,
                comparisonType: StringComparison.Ordinal));
    }

    private static void Reject(Dictionary<string, byte[]> files, string id, string diagnostic)
    {
        DocumentationProjects.Check(files: Current(), root: ProofProtocol.Root).ShouldBeEmpty();
        DocumentationProjects.Check(files: files, root: ProofProtocol.Root).ShouldContain(
                elementPredicate: error => error.StartsWith(value: diagnostic + " " + Core,
                        comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: diagnostic, location: Core);
    }
}
