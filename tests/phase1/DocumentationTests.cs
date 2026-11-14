using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves exact documentation navigation with actual compliant and mutated inputs.</summary>
public sealed class DocumentationTests
{
    /// <summary>Distinguishes an existing heading-only index from a usable required navigation index.</summary>
    [Fact, Trait("Case", "docs.portable")]
    public void Check_EmptyIndex_RejectsMissingEntries()
    {
        var files = new Dictionary<string, byte[]>(comparer: System.StringComparer.Ordinal)
        {
            ["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "# Guide\n"),
        };
        DocumentationValidator.Check(files: files, required: ["AGENTS.md"]).ShouldContain(
                elementPredicate: item => System.StringComparer.Ordinal.Equals(x: item, y: "DOC_INDEX AGENTS.md"));
        ProofProtocol.Record(id: "docs.portable", diagnostic: "DOC_INDEX", location: "AGENTS.md:1");
    }

    /// <summary>Rejects a wrong-case target even when the host filesystem accepts the spelling.</summary>
    [Fact, Trait("Case", "docs.navigation")]
    public void Check_WrongCaseLink_ReportsExactTarget()
    {
        CheckRepository();
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

    /// <summary>Traverses real canonical indexes into executable definitions on the current checkout.</summary>
    [Fact, Trait("Case", "ac-10.complete")]
    public void Check_RepositoryIndex_ResolvesAuthority()
    {
        CheckRepository();
        ProofProtocol.Record(id: "ac-10.complete", diagnostic: "DOC_NAVIGATION", location: "AGENTS.md:12");
    }

    /// <summary>Checks real on-disk bytes with a wrong-case link independently of host path resolution.</summary>
    [Fact, Trait("Case", "ac-11.complete")]
    public void Check_FilesystemSubject_RejectsWrongCase()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        string root = DocumentationFixture.Write(files: files);
        files["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "[Subject](docs/Subject.md)\n");
        File.WriteAllBytes(path: Path.Combine(path1: root, path2: "AGENTS.md"), bytes: files["AGENTS.md"]);
        var actual = files.Keys.ToDictionary(keySelector: path => path,
                elementSelector: path => File.ReadAllBytes(path: Path.Combine(path1: root, path2: path)),
                comparer: StringComparer.Ordinal);
        Reject(files: actual, id: "ac-11.complete", diagnostic: "DOC_CASE", location: "AGENTS.md:1");
    }

    /// <summary>Rejects directory-segment collisions even when the leaf filenames differ.</summary>
    [Fact, Trait("Case", "edge.rule-04.adjacency")]
    public void Check_CollidingSegments_RejectsAmbiguity()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files.Add(key: "docs/Subject.md", value: Encoding.UTF8.GetBytes(s: "# Other\n"));
        Reject(files: files, id: "edge.rule-04.adjacency", diagnostic: "DOC_COLLISION", location: "docs/subject.md");
    }

    /// <summary>Rejects a required document removed from the exact inventory.</summary>
    [Fact, Trait("Case", "edge.rule-04.empty")]
    public void Check_MissingDocument_ReportsRequired()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files.Remove(key: "docs/subject.md");
        Reject(files: files, id: "edge.rule-04.empty", diagnostic: "DOC_REQUIRED", location: "docs/subject.md");
    }

    /// <summary>Preserves composed and decomposed Unicode as two distinct valid target identities.</summary>
    [Fact, Trait("Case", "edge.rule-04.encoding")]
    public void Check_UnicodeSpaces_PreservesExactIdentity()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create(link: "/docs/%C3%A9%20name.md#café");
        files.Add(key: "docs/é name.md", value: Encoding.UTF8.GetBytes(s: "# Café\n"));
        files.Add(key: "docs/e\u0301 name.md", value: Encoding.UTF8.GetBytes(s: "# Other\n"));
        Accept(files: files, id: "edge.rule-04.encoding", location: "docs/é name.md:1");
    }

    /// <summary>Permuting multiple links preserves validity without changing either input collection.</summary>
    [Fact, Trait("Case", "edge.rule-04.ordering")]
    public void Check_PermutedLinks_PreservesVerdict()
    {
        Dictionary<string, byte[]> first = DocumentationFixture.Create();
        first["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "[A](docs/subject.md)\n[B](docs/subject.md#subject)\n");
        Dictionary<string, byte[]> second = DocumentationFixture.Create();
        second["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "[B](docs/subject.md#subject)\n[A](docs/subject.md)\n");
        string before = DocumentationFixture.Hash(files: first) + DocumentationFixture.Hash(files: second);
        DocumentationValidator.Check(files: first, required: ["AGENTS.md"]).ShouldBeEmpty();
        DocumentationValidator.Check(files: second, required: ["AGENTS.md"]).ShouldBeEmpty();
        (DocumentationFixture.Hash(files: first) + DocumentationFixture.Hash(files: second)).ShouldBe(
                expected: before);
        ProofProtocol.Record(id: "edge.rule-04.ordering", diagnostic: "DOC_NAVIGATION", location: "AGENTS.md:1-2");
    }

    /// <summary>Concurrent checks preserve every actual authored byte and the actual Git index bytes.</summary>
    [Fact, Trait("Case", "edge.rule-04.concurrency")]
    public async Task Check_ConcurrentReads_PreservesInputs()
    {
        byte[] index = DocumentationFixture.IndexBytes();
        string before = DocumentationFixture.Hash(files: DocumentationValidator.ReadRepository());
        Task[] checks =
        [
            Task.Run(action: CheckRepository, cancellationToken: TestContext.Current.CancellationToken),
            Task.Run(action: CheckRepository, cancellationToken: TestContext.Current.CancellationToken)
        ];
        await Task.WhenAll(tasks: checks);
        DocumentationFixture.IndexBytes().ShouldBe(expected: index);
        DocumentationFixture.Hash(files: DocumentationValidator.ReadRepository()).ShouldBe(expected: before);
        ProofProtocol.Record(id: "edge.rule-04.concurrency", diagnostic: "DOC_UNCHANGED", location: "AGENTS.md:1");
    }

    /// <summary>Rejects a present required document containing only whitespace.</summary>
    [Fact, Trait("Case", "docs.empty")]
    public void Check_WhitespaceDocument_ReportsEmpty()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["docs/subject.md"] = Encoding.UTF8.GetBytes(s: " \n");
        Reject(files: files, id: "docs.empty", diagnostic: "DOC_EMPTY", location: "docs/subject.md");
    }

    /// <summary>Rejects a nonexistent target while keeping the required source and target document valid.</summary>
    [Fact, Trait("Case", "docs.target")]
    public void Check_BrokenTarget_ReportsMissing()
    {
        Reject(files: DocumentationFixture.Create(link: "docs/missing.md"), id: "docs.target",
                diagnostic: "DOC_TARGET", location: "AGENTS.md:2");
    }

    /// <summary>Rejects a nonexistent exact heading rather than accepting the existing file alone.</summary>
    [Fact, Trait("Case", "docs.anchor")]
    public void Check_BrokenAnchor_ReportsMissing()
    {
        Reject(files: DocumentationFixture.Create(link: "docs/subject.md#absent"), id: "docs.anchor",
                diagnostic: "DOC_ANCHOR", location: "AGENTS.md:2");
    }

    /// <summary>Rejects duplicate implicit heading slugs instead of choosing the first declaration.</summary>
    [Fact, Trait("Case", "docs.ambiguous-anchor")]
    public void Check_DuplicateHeadings_RejectsAmbiguity()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["docs/subject.md"] = Encoding.UTF8.GetBytes(s: "# Subject\n## Subject\n");
        Reject(files: files, id: "docs.ambiguous-anchor", diagnostic: "DOC_ANCHOR", location: "AGENTS.md:2");
    }

    /// <summary>Allows duplicate visible headings through an explicit unique declared anchor.</summary>
    [Fact, Trait("Case", "docs.explicit-anchor")]
    public void Check_ExplicitDisambiguation_AcceptsUniqueAnchor()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create(link: "docs/subject.md#second");
        files["docs/subject.md"] = Encoding.UTF8.GetBytes(s: "# Subject\n<a id=\"second\"></a>\n## Subject\n");
        Accept(files: files, id: "docs.explicit-anchor", location: "docs/subject.md:2");
    }

    /// <summary>Rejects duplicate reference definitions instead of letting document order pick a target.</summary>
    [Fact, Trait("Case", "docs.ambiguous-reference")]
    public void Check_DuplicateReferences_RejectsAmbiguity()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["AGENTS.md"] = Encoding.UTF8.GetBytes(s:
                "[Subject][ref]\n[ref]: docs/subject.md\n[ref]: docs/subject.md#subject\n");
        Reject(files: files, id: "docs.ambiguous-reference", diagnostic: "DOC_REFERENCE", location: "AGENTS.md:3");
    }

    /// <summary>Resolves one exact collapsed reference to its declared UTF-8 target.</summary>
    [Fact, Trait("Case", "docs.reference")]
    public void Check_CollapsedReference_AcceptsUniqueDefinition()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "[Subject][]\n[Subject]: docs/subject.md\n");
        Accept(files: files, id: "docs.reference", location: "AGENTS.md:1");
    }

    /// <summary>Rejects an undefined full reference rather than ignoring it as plain text.</summary>
    [Fact, Trait("Case", "docs.missing-reference")]
    public void Check_MissingReference_ReportsDefinition()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "[Subject][absent]\n");
        Reject(files: files, id: "docs.missing-reference", diagnostic: "DOC_REFERENCE", location: "AGENTS.md:1");
    }

    /// <summary>Rejects actual invalid UTF-8 bytes loaded from an isolated filesystem subject.</summary>
    [Fact, Trait("Case", "docs.invalid-utf8")]
    public void Check_InvalidUtf8_ReportsEncoding()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files["docs/subject.md"] = [0xc3, 0x28];
        string root = DocumentationFixture.Write(files: files);
        files["docs/subject.md"] = File.ReadAllBytes(path: Path.Combine(path1: root, path2: "docs/subject.md"));
        Reject(files: files, id: "docs.invalid-utf8", diagnostic: "DOC_UTF8", location: "docs/subject.md");
    }

    /// <summary>Rejects a malformed percent escape before resolving any host path.</summary>
    [Fact, Trait("Case", "docs.percent")]
    public void Check_MalformedPercent_ReportsEncoding()
    {
        Reject(files: DocumentationFixture.Create(link: "docs/%ZZ.md"), id: "docs.percent",
                diagnostic: "DOC_ENCODING", location: "AGENTS.md:2");
    }

    /// <summary>Rejects a syntactically hex escape whose bytes are not valid UTF-8.</summary>
    [Fact, Trait("Case", "docs.percent-utf8")]
    public void Check_InvalidEscapedUtf8_ReportsEncoding()
    {
        Reject(files: DocumentationFixture.Create(link: "docs/%FF.md"), id: "docs.percent-utf8",
                diagnostic: "DOC_ENCODING", location: "AGENTS.md:2");
    }

    /// <summary>Rejects an encoded parent traversal escaping the repository before file access.</summary>
    [Fact, Trait("Case", "docs.escape")]
    public void Check_EscapingLocalLink_ReportsBoundary()
    {
        Reject(files: DocumentationFixture.Create(link: "%2e%2e/outside.md"), id: "docs.escape",
                diagnostic: "DOC_ESCAPE", location: "AGENTS.md:2");
    }

    /// <summary>Requires a genuine single link and single target, without vacuous empty discovery.</summary>
    [Fact, Trait("Case", "docs.singleton")]
    public void Check_SingletonIndex_AcceptsSingleTarget()
    {
        Accept(files: DocumentationFixture.Create(), id: "docs.singleton", location: "AGENTS.md:2");
    }

    /// <summary>Excludes installed and provenance documents even when their bytes and paths are invalid.</summary>
    [Fact, Trait("Case", "docs.scope")]
    public void Check_InstalledDocuments_ExcludesExternalScope()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files.Add(key: ".codex/Bad.md", value: [0xff]);
        files.Add(key: ".codex/bad.md", value: [0xff]);
        files.Add(key: "docs/code/provenance/invalid.md", value: [0xff]);
        files.Add(key: "src/core/obj/docs/invalid.md", value: [0xff]);
        Accept(files: files, id: "docs.scope", location: "AGENTS.md:2");
    }

    /// <summary>Requires real focused indexes to reference nonempty executable definitions.</summary>
    [Fact, Trait("Case", "R-83.positive")]
    public void Check_RealRuleIndexes_AcceptsOperationalLinks()
    {
        CheckRepository();
        ProofProtocol.Record(id: "R-83.positive", diagnostic: "DOC_NAVIGATION", location: "docs/code/README.md:7");
    }

    /// <summary>Rejects removal of the actual focused code index from an otherwise complete inventory.</summary>
    [Fact, Trait("Case", "R-83.negative")]
    public void Check_RemovedRuleIndex_RejectsRequiredAuthority()
    {
        var files = new Dictionary<string, byte[]>(dictionary: DocumentationValidator.ReadRepository(),
                comparer: StringComparer.Ordinal);
        files.Remove(key: "docs/code/README.md");
        Reject(files: files, id: "R-83.negative", diagnostic: "DOC_REQUIRED", location: "docs/code/README.md",
                required: DocumentationValidator.Authored);
    }

    /// <summary>Requires all real decision pages to be complete and linked through the focused index.</summary>
    [Fact, Trait("Case", "R-84.positive")]
    public void Check_RealDecisions_AcceptsCompleteStructure()
    {
        DocumentationValidator.CheckDecisions(files: DocumentationValidator.ReadRepository()).ShouldBeEmpty();
        ProofProtocol.Record(id: "R-84.positive", diagnostic: "DOC_DECISIONS",
                location: "docs/architecture/decisions/0001-enforcement.md:1");
    }

    /// <summary>Rejects the actual decision when its consequences section is removed.</summary>
    [Fact, Trait("Case", "R-84.negative")]
    public void Check_IncompleteDecision_ReportsMissingSection()
    {
        var files = new Dictionary<string, byte[]>(dictionary: DocumentationValidator.ReadRepository(),
                comparer: StringComparer.Ordinal);
        const string Path = "docs/architecture/decisions/0001-enforcement.md";
        files[Path] = Encoding.UTF8.GetBytes(s: Encoding.UTF8.GetString(bytes: files[Path]).Replace(
                oldValue: "## Consequences", newValue: "## Omitted", comparisonType: StringComparison.Ordinal));
        DocumentationValidator.CheckDecisions(files: files).ShouldContain(elementPredicate: error => error.StartsWith(
                value: "DOC_DECISION " + Path + ":1", comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: "R-84.negative", diagnostic: "DOC_DECISION", location: Path + ":1");
    }

    /// <summary>Preserves the approved interactive preference and its exact canonical indexed link.</summary>
    [Fact, Trait("Case", "docs.workflow")]
    public void Check_InteractionPreference_PreservesApprovedContract()
    {
        RegisterValidator.Read(relative: ".planning/config.json")["workflow"]?["text_mode"]?.GetValue<bool>()
                .ShouldBe(expected: false);
        string source = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "docs/workflow/ai-collaboration.md"));
        source.ShouldContain(expected: "[agent guide](../../AGENTS.md)");
        source.ShouldContain(expected: "Use the available interactive question controls");
        source.ShouldContain(expected: "Keep GSD's `workflow.text_mode` set to `false`");
        ProofProtocol.Record(id: "docs.workflow", diagnostic: "DOC_WORKFLOW",
                location: "docs/workflow/ai-collaboration.md:7");
    }

    /// <summary>Rejects case-colliding directory segments whose leaf names are independently unique.</summary>
    [Fact, Trait("Case", "docs.directory-collision")]
    public void Check_DirectoryCollision_RejectsDistinctLeaves()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create();
        files.Add(key: "Docs/other.md", value: Encoding.UTF8.GetBytes(s: "# Other\n"));
        Reject(files: files, id: "docs.directory-collision", diagnostic: "DOC_COLLISION", location: "Docs | docs");
    }

    /// <summary>Rejects duplicate explicit IDs even when their visible heading text differs.</summary>
    [Fact, Trait("Case", "docs.anchor-collision")]
    public void Check_DuplicateExplicitIds_RejectsAmbiguity()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create(link: "docs/subject.md#named");
        files["docs/subject.md"] = Encoding.UTF8.GetBytes(s: "<a id=\"named\"></a>\n<a id=\"named\"></a>\n");
        Reject(files: files, id: "docs.anchor-collision", diagnostic: "DOC_ANCHOR", location: "AGENTS.md:2");
    }

    /// <summary>Does not invent numeric suffixes for duplicate implicit headings under the declared policy.</summary>
    [Fact, Trait("Case", "docs.heading-suffix")]
    public void Check_ImplicitNumericSuffix_RequiresExplicitAnchor()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create(link: "docs/subject.md#subject-1");
        files["docs/subject.md"] = Encoding.UTF8.GetBytes(s: "# Subject\n## Subject\n");
        Reject(files: files, id: "docs.heading-suffix", diagnostic: "DOC_ANCHOR", location: "AGENTS.md:2");
    }

    /// <summary>Does not normalize a composed link onto a distinct decomposed filename.</summary>
    [Fact, Trait("Case", "docs.unicode-distinct")]
    public void Check_DistinctUnicodeTarget_RejectsNormalization()
    {
        Dictionary<string, byte[]> files = DocumentationFixture.Create(link: "docs/%C3%A9.md");
        files.Add(key: "docs/e\u0301.md", value: Encoding.UTF8.GetBytes(s: "# Other\n"));
        Reject(files: files, id: "docs.unicode-distinct", diagnostic: "DOC_TARGET", location: "AGENTS.md:2");
    }

    private static void Accept(IReadOnlyDictionary<string, byte[]> files, string id, string location)
    {
        DocumentationValidator.Check(files: files, required: ["AGENTS.md", "docs/subject.md"]).ShouldBeEmpty();
        ProofProtocol.Record(id: id, diagnostic: "DOC_NAVIGATION", location: location);
    }

    private static void Reject(IReadOnlyDictionary<string, byte[]> files, string id, string diagnostic,
            string location, IReadOnlyList<string>? required = null)
    {
        DocumentationValidator.Check(files: files, required: required ?? ["AGENTS.md", "docs/subject.md"])
                .ShouldContain(elementPredicate: error => error.StartsWith(value: diagnostic + " ",
                        comparisonType: StringComparison.Ordinal) && error.Contains(value: location,
                                comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: diagnostic, location: location);
    }

    private static void CheckRepository()
    {
        Dictionary<string, byte[]> files = DocumentationValidator.ReadRepository();
        DocumentationValidator.Check(files: files, required: DocumentationValidator.Authored).ShouldBeEmpty();
        string guide = Encoding.UTF8.GetString(bytes: files["AGENTS.md"]);
        guide.ShouldContain(expected: "[Code index](docs/code/README.md)");
        Encoding.UTF8.GetString(bytes: files["docs/code/README.md"]).ShouldContain(
                expected: "[.editorconfig](../../.editorconfig)");
        files.ContainsKey(key: "Agents.md").ShouldBeFalse();
    }
}
