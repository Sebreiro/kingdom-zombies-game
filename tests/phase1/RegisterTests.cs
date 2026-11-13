using System.Text.Json.Nodes;
using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Checks captured donor inventory, exact rule mappings, and bounded exemption declarations using detached mutations.</summary>
public sealed class RegisterTests
{
    private static readonly int[] BodyBoundaries = [59, 60, 61];
    private static (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) Inputs() =>
        (RegisterValidator.Read("docs/code/provenance/manifest.json"), RegisterValidator.Read("docs/code/register.json"), RegisterValidator.Read("docs/code/exemptions.json"));

    /// <summary>Checks the complete capture, then requires rejection when R-01 loses a heading relation.</summary>
    [Fact, Trait("Case", "register.capture")]
    public void CaptureMissingHeadingRelationIsRejected()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        input.Register["rules"]![0]!["headingRefs"]!.AsArray().RemoveAt(0);
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains("R-01: HEADING_RELATION", StringComparison.Ordinal));
        ProofProtocol.Record("register.capture", "CAPTURE_EXACT", "85 IDs, 84 heading occurrences, 24 donor exemptions and missing R-01 relation");
    }

    private static void RejectCapture(string id, Action<JsonObject, JsonObject, JsonObject> mutate, string expected)
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        mutate(input.Manifest, input.Register, input.Exemptions);
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains(expected, StringComparison.Ordinal));
        ProofProtocol.Record(id, expected, "detached captured inventory mutation");
    }

    /// <summary>Rejects a capture that omits an expected donor heading.</summary>
    [Fact, Trait("Case", "register.omit-heading")]
    public void CaptureMissingHeadingIsRejected() => RejectCapture("register.omit-heading", (m, r, e) => m["headings"]!.AsArray().RemoveAt(0), "CAPTURE: HEADING_INVENTORY");

    /// <summary>Rejects a capture that omits the R-01 matrix row.</summary>
    [Fact, Trait("Case", "register.omit-matrix")]
    public void CaptureMissingMatrixRowIsRejected() => RejectCapture("register.omit-matrix", (m, r, e) => m["matrix"]!.AsArray().RemoveAt(0), "R-01: SOURCE_ROW");

    /// <summary>Rejects omission of an original donor exemption disposition.</summary>
    [Fact, Trait("Case", "register.omit-exemption")]
    public void CaptureMissingSourceExemptionIsRejected() => RejectCapture("register.omit-exemption", (m, r, e) => e["donorExemptions"]!.AsArray().RemoveAt(0), "REGISTER: DONOR_EXEMPTION_INVENTORY");

    /// <summary>Rejects duplicate rule IDs before deduplication could conceal them.</summary>
    [Fact, Trait("Case", "register.duplicate")]
    public void CaptureDuplicateIdIsRejectedBeforeSetConversion() => RejectCapture("register.duplicate", (m, r, e) => r["rules"]!.AsArray().Add(r["rules"]![0]!.DeepClone()), "R-01: RULE_ID: DUPLICATE_OR_EMPTY");

    /// <summary>Checks both R-37 source headings and its pending 59/60/61 boundary mappings without claiming enforcement.</summary>
    [Fact, Trait("Case", "register.r37-trace")]
    public void CaptureR37ConnectsBothHeadingsAndPlannedBoundaryProofs()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var row = input.Register["rules"]!.AsArray().Single(x => string.Equals(x!["sourceId"]!.GetValue<string>(), "R-37", StringComparison.Ordinal))!;
        row["headingRefs"]!.AsArray().Count.ShouldBe(2);
        row["mechanism"]!["path"]!.GetValue<string>().ShouldBe("tools/code-rules/BodyLengthAnalyzer.cs");
        row["mechanism"]!["diagnostics"]!.AsArray().Select(x => x!.GetValue<string>()).ShouldContain("SCR0006", StringComparer.Ordinal);
        row["mechanism"]!["status"]!.GetValue<string>().ShouldBe("pending");
        row["numeric"]!["boundaries"]!.AsArray().Select(x => x!.GetValue<int>()).ShouldBe(BodyBoundaries);
        row["positiveProof"]!["id"]!.GetValue<string>().ShouldBe("R-37.positive");
        row["negativeProof"]!["id"]!.GetValue<string>().ShouldBe("R-37.negative");
        ProofProtocol.Record("register.r37-trace", "R37_TRACE_EXACT", "R-37 source identity → SCR0006 pending implementation → declared 59/60/61 boundaries; no enforcement pass claimed");
    }

    /// <summary>Requires an enforcement mechanism for an applicable rule.</summary>
    [Fact, Trait("Case", "register.schema")]
    public void SchemaMissingMechanismIsRejected() => RejectCapture("register.schema", (m, r, e) => r["rules"]![0]!["mechanism"] = null, "R-01: MECHANISM_REQUIRED");

    /// <summary>Reports the rule ID when its heading relation field is absent.</summary>
    [Fact, Trait("Case", "register.missing-relation-field")]
    public void SchemaAbsentRelationFieldIsActionable() => RejectCapture("register.missing-relation-field", (m, r, e) => r["rules"]![0]!.AsObject().Remove("headingRefs"), "R-01: HEADING_RELATION");

    /// <summary>Checks all 85 rules, 84 headings and 24 exemptions, including the absent EX-22 identity.</summary>
    [Fact, Trait("Case", "ac-05.complete")]
    public void InventoryCompleteBaselineHasEveryExactDisposition()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        input.Manifest["matrix"]!.AsArray().Count.ShouldBe(85);
        input.Manifest["headings"]!.AsArray().Count.ShouldBe(84);
        input.Exemptions["donorExemptions"]!.AsArray().Count.ShouldBe(24);
        input.Exemptions["donorExemptions"]!.AsArray().Any(x => string.Equals(x!["sourceId"]!.GetValue<string>(), "EX-22", StringComparison.Ordinal)).ShouldBeFalse();
        ProofProtocol.Record("ac-05.complete", "INVENTORY_EXACT", "complete local immutable source inventory and all dispositions; enforcement separately pending");
    }

    /// <summary>Rejects a second R-01 declaration with a conflicting disposition.</summary>
    [Fact, Trait("Case", "edge.rule-02.adjacency")]
    public void SchemaConflictingDuplicateCannotMaskOriginal() => RejectCapture("edge.rule-02.adjacency", (m, r, e) =>
    {
        var copy = r["rules"]![0]!.DeepClone(); copy["disposition"] = "inapplicable"; r["rules"]!.AsArray().Add(copy);
    }, "R-01: RULE_ID: DUPLICATE_OR_EMPTY");

    /// <summary>Rejects nine detached mutations that empty inventories or remove required mapping fields.</summary>
    [Fact, Trait("Case", "edge.rule-02.empty")]
    public void SchemaEmptyRequiredFieldsAlwaysFail()
    {
        var mutations = new (Action<JsonObject, JsonObject, JsonObject> Apply, string Diagnostic)[]
        {
            ((m, r, e) => r["rules"]!.AsArray().Clear(), "REGISTER: RULE_INVENTORY"),
            ((m, r, e) => m["headings"]!.AsArray().Clear(), "CAPTURE: HEADING_INVENTORY"),
            ((m, r, e) => m["files"]!.AsArray().Clear(), "CAPTURE: FILE_INVENTORY"),
            ((m, r, e) => r["rules"]![0]!["reason"] = " ", "R-01: DISPOSITION"),
            ((m, r, e) => r["rules"]![0]!["scope"] = "", "R-01: SCOPE_REQUIRED"),
            ((m, r, e) => r["rules"]![1]!["adaptationReason"] = null, "R-02: ADAPTATION_REASON"),
            ((m, r, e) => r["rules"]![0]!["positiveProof"] = null, "R-01: POSITIVE_PROOF_REQUIRED"),
            ((m, r, e) => r["rules"]![0]!["negativeProof"] = null, "R-01: NEGATIVE_PROOF_REQUIRED"),
            ((m, r, e) => r["rules"]![0]!["documentation"] = null, "R-01: DOCUMENTATION_REFERENCE"),
        };
        foreach (var mutation in mutations)
        {
            var input = Inputs(); mutation.Apply(input.Manifest, input.Register, input.Exemptions);
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain(x => x.Contains(mutation.Diagnostic, StringComparison.Ordinal));
        }
        ProofProtocol.Record("edge.rule-02.empty", "REQUIRED_FIELDS_REJECTED", "nine detached empty/missing inventory and required mapping mutations");
    }

    /// <summary>Rejects identity spelling changes, invalid UTF-8 and duplicate JSON keys.</summary>
    [Fact, Trait("Case", "edge.rule-02.encoding")]
    public void SchemaExactEncodingNeverNormalizesIdentity()
    {
        foreach (var value in new[] { "r-01", "R-001", "R-０１", "R-01\u0301", "R-0\u0131" })
        {
            var input = Inputs(); input.Register["rules"]![0]!["sourceId"] = value;
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("REGISTER: RULE_INVENTORY", StringComparer.Ordinal);
        }
        Should.Throw<System.Text.DecoderFallbackException>(() => RegisterValidator.Parse(new byte[] { 0xC3, 0x28 }));
        Should.Throw<InvalidDataException>(() => RegisterValidator.Parse(System.Text.Encoding.UTF8.GetBytes("{\"rules\":[],\"rules\":[]}"))).Message.ShouldContain("JSON_DUPLICATE");
        ProofProtocol.Record("edge.rule-02.encoding", "IDENTITY_EXACT", "case, width, leading zero, combining and dotless characters; malformed UTF-8 and duplicate JSON keys");
    }

    private static void Reverse(JsonObject obj, string name)
    {
        var rows = obj[name]!.AsArray().Select(x => x!.DeepClone()).Reverse().ToArray();
        obj[name] = new JsonArray(rows);
    }

    /// <summary>Checks that row and relation permutations preserve coverage while unknown rule IDs still fail.</summary>
    [Fact, Trait("Case", "edge.rule-02.ordering")]
    public void SchemaPermutationsPreserveVerdictAndExactCoverage()
    {
        var input = Inputs();
        var before = RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions);
        before.ShouldBeEmpty();
        foreach (var name in new[] { "files", "headings", "matrix", "exemptions" }) Reverse(input.Manifest, name);
        Reverse(input.Register, "rules"); Reverse(input.Exemptions, "donorExemptions"); Reverse(input.Exemptions, "local");
        foreach (var heading in input.Manifest["headings"]!.AsArray()) Reverse(heading!.AsObject(), "sourceIds");
        foreach (var rule in input.Register["rules"]!.AsArray()) Reverse(rule!.AsObject(), "headingRefs");
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBe(before);
        input.Register["rules"]![0]!["sourceId"] = "R-unknown";
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("REGISTER: RULE_INVENTORY", StringComparer.Ordinal);
        ProofProtocol.Record("edge.rule-02.ordering", "PERMUTATION_EQUAL", "row/file/heading/relation permutations preserve full coverage; unknown ID remains rejected");
    }

    /// <summary>Rejects a mechanism path whose filename casing differs from the declared symbol.</summary>
    [Fact, Trait("Case", "register.reference-case")]
    public void SchemaCaseChangedReferenceIsRejected() => RejectCapture("register.reference-case", (m, r, e) => r["rules"]![36]!["mechanism"]!["path"] = "tools/code-rules/bodyLengthAnalyzer.cs", "R-37: MECHANISM_REFERENCE");

    /// <summary>Rejects a mechanism symbol that differs from the exact analyzer name.</summary>
    [Fact, Trait("Case", "register.symbol")]
    public void SchemaWrongMechanismSymbolIsRejected() => RejectCapture("register.symbol", (m, r, e) => r["rules"]![36]!["mechanism"]!["symbol"] = "bodyLengthAnalyzer", "R-37: MECHANISM_REFERENCE");

    /// <summary>Detects a captured mechanism field changed from the immutable donor row.</summary>
    [Fact, Trait("Case", "register.source-field")]
    public void SchemaAlteredCapturedMechanismIsRejected() => RejectCapture("register.source-field", (m, r, e) => m["matrix"]![36]!["mechanism"] = "SCR0007", "R-37: SOURCE_FIELD: mechanism");

    /// <summary>Rejects a proof path that does not match the rule-specific analyzer test.</summary>
    [Fact, Trait("Case", "register.proof-path")]
    public void SchemaWrongProofPathIsRejected() => RejectCapture("register.proof-path", (m, r, e) => r["rules"]![36]!["positiveProof"]!["path"] = "tests/phase1/Analyzers/bodyLengthAnalyzerTests.cs", "R-37: PROOF_PATH");

    /// <summary>Keeps composed and decomposed Unicode paths distinct from the required ASCII path.</summary>
    [Fact, Trait("Case", "register.reference-unicode")]
    public void SchemaUnicodeReferenceIsRejected()
    {
        foreach (var spelling in new[] { "BodyLéngthAnalyzer.cs", "BodyLe\u0301ngthAnalyzer.cs" })
        {
            var input = Inputs(); input.Register["rules"]![36]!["mechanism"]!["path"] = "tools/code-rules/" + spelling;
            RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldContain("R-37: MECHANISM_REFERENCE", StringComparer.Ordinal);
        }
        ProofProtocol.Record("register.reference-unicode", "R-37: MECHANISM_REFERENCE", "both composed and decomposed spellings remain distinct from exact ASCII path");
    }

    /// <summary>Rejects evidence mapping borrowed from another rule.</summary>
    [Fact, Trait("Case", "register.proof-reverse")]
    public void SchemaProofForDifferentRuleIsRejected() => RejectCapture("register.proof-reverse", (m, r, e) => r["rules"]![0]!["positiveProof"] = r["rules"]![1]!["positiveProof"]!.DeepClone(), "R-01: PROOF_REVERSE_ID");

    /// <summary>Rejects a rule mechanism whose required error severity is weakened.</summary>
    [Fact, Trait("Case", "register.severity")]
    public void SchemaSeverityWeakeningIsRejected() => RejectCapture("register.severity", (m, r, e) => r["rules"]![36]!["mechanism"]!["severity"] = "warning", "R-37: SEVERITY_WEAKENED");

    /// <summary>Rejects substitution of a different custom diagnostic ID.</summary>
    [Fact, Trait("Case", "register.diagnostic")]
    public void SchemaDiagnosticSubstitutionIsRejected() => RejectCapture("register.diagnostic", (m, r, e) => r["rules"]![36]!["mechanism"]!["diagnostics"] = new JsonArray("SCR0007"), "R-37: DIAGNOSTIC_IDENTITY");

    /// <summary>Requires the exact rule identity in the documentation reference.</summary>
    [Fact, Trait("Case", "register.documentation")]
    public void SchemaWrongDocumentationAnchorIsRejected() => RejectCapture("register.documentation", (m, r, e) => r["rules"]![0]!["documentation"]!["symbol"] = "r-01", "R-01: DOCUMENTATION_REFERENCE");

    /// <summary>Rejects a body-length limit changed from the captured numeric policy.</summary>
    [Fact, Trait("Case", "register.numeric")]
    public void SchemaNumericLimitDriftIsRejected() => RejectCapture("register.numeric", (m, r, e) => r["rules"]![36]!["numeric"]!["limit"] = 61, "R-37: NUMERIC_LIMIT");

    /// <summary>Rejects a line-length counting unit changed to Unicode scalar values.</summary>
    [Fact, Trait("Case", "register.numeric-semantics")]
    public void SchemaNumericCountingChangeIsRejected() => RejectCapture("register.numeric-semantics", (m, r, e) => r["rules"]![9]!["numeric"]!["unit"] = "Unicode scalar values", "R-10: NUMERIC_SEMANTICS");

    /// <summary>Rejects duplicate boundary proof IDs instead of counting one case twice.</summary>
    [Fact, Trait("Case", "register.boundary-proof")]
    public void SchemaReusedBoundaryEvidenceIsRejected() => RejectCapture("register.boundary-proof", (m, r, e) => r["rules"]![36]!["numeric"]!["boundaryProofs"]![1]!["id"] = "rules.body.59", "rules.body.59: BOUNDARY_PROOF: DUPLICATE_OR_EMPTY");

    /// <summary>Prevents a pending qualitative review from being represented as an automated pass.</summary>
    [Fact, Trait("Case", "register.review")]
    public void SchemaAutomatedHumanJudgmentIsRejected() => RejectCapture("register.review", (m, r, e) => r["rules"]![23]!["qualitativeReview"]!["status"] = "pass", "R-24: HUMAN_REVIEW_PENDING");

    /// <summary>Rejects duplicate local exemption IDs before scopes can be combined.</summary>
    [Fact, Trait("Case", "register.exemption-duplicate")]
    public void SchemaDuplicateExemptionIsRejected() => RejectCapture("register.exemption-duplicate", (m, r, e) => e["local"]!.AsArray().Add(e["local"]![0]!.DeepClone()), "KZ-EX-LOCK-01: LOCAL_EXEMPTION_ID: DUPLICATE_OR_EMPTY");

    /// <summary>Rejects a differently named exemption overlapping an existing diagnostic scope.</summary>
    [Fact, Trait("Case", "register.exemption-overlap")]
    public void SchemaOverlappingExemptionsAreRejected() => RejectCapture("register.exemption-overlap", (m, r, e) =>
    {
        var clone = e["local"]![0]!.DeepClone(); clone["id"] = "KZ-EX-LOCK-02"; e["local"]!.AsArray().Add(clone);
    }, "KZ-EX-LOCK-02: EXEMPTION_OVERLAP");

    /// <summary>Rejects replacing an exact lock-file exemption with a directory glob.</summary>
    [Fact, Trait("Case", "register.exemption-broad")]
    public void SchemaBroadenedExemptionScopeIsRejected() => RejectCapture("register.exemption-broad", (m, r, e) => e["local"]![0]!["path"] = "tests/**/packages.lock.json", "KZ-EX-LOCK-01: EXEMPTION_SCOPE");

    /// <summary>Requires pending mechanisms and reviews to remain incomplete when implementation is required.</summary>
    [Fact, Trait("Case", "register.pending")]
    public void SchemaFutureMappingsCannotCountAsCompletedEvidence()
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var strict = RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions, requireImplemented: true);
        strict.ShouldContain("R-37: IMPLEMENTATION_PENDING", StringComparer.Ordinal);
        strict.ShouldContain("R-37: PROOF_UNIMPLEMENTED", StringComparer.Ordinal);
        strict.ShouldContain("R-24: HUMAN_REVIEW_PENDING", StringComparer.Ordinal);
        ProofProtocol.Record("register.pending", "IMPLEMENTATION_PENDING", "structural capture passes; unbound enforcement and qualitative judgment cannot satisfy full AC-06");
    }

    private static void Resolution(string id)
    {
        var input = Inputs();
        RegisterValidator.Validate(input.Manifest, input.Register, input.Exemptions).ShouldBeEmpty();
        var row = input.Register["rules"]!.AsArray().Single(x => string.Equals(x!["sourceId"]!.GetValue<string>(), id, StringComparison.Ordinal))!;
        row["disposition"]!.GetValue<string>().ShouldBe("inapplicable");
        row["reason"]!.GetValue<string>().ShouldNotBeNullOrWhiteSpace();
        row["mechanism"].ShouldBeNull();
        ProofProtocol.Record(id + ".register-resolution", "INAPPLICABLE_ACCOUNTED", id + " HTTP/database-only mechanism explicitly accounted without enforcement credit");
    }

    /// <summary>Checks R43 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-43.register-resolution")]
    public void ResolutionR43IsExplicit() => Resolution("R-43");
    /// <summary>Checks R57 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-57.register-resolution")]
    public void ResolutionR57IsExplicit() => Resolution("R-57");
    /// <summary>Checks R76 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-76.register-resolution")]
    public void ResolutionR76IsExplicit() => Resolution("R-76");
    /// <summary>Checks R79 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-79.register-resolution")]
    public void ResolutionR79IsExplicit() => Resolution("R-79");
    /// <summary>Checks R80 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-80.register-resolution")]
    public void ResolutionR80IsExplicit() => Resolution("R-80");
    /// <summary>Checks R85 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-85.register-resolution")]
    public void ResolutionR85IsExplicit() => Resolution("R-85");
}
