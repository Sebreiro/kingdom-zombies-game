using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks captured donor inventory, exact rule mappings, and bounded exemption declarations using
/// detached mutations.</summary>
public sealed class RegisterTests
{
    /// <summary>Checks the complete capture, then requires rejection when R-01 loses a heading relation.</summary>
    [Fact, Trait("Case", "register.capture")]
    public void ValidateRegister_CaptureMissingHeadingRelation_Rejected()
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        (((input.Register["rules"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null"))["headingRefs"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().RemoveAt(index: 0);
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldContain(elementPredicate: x => x.Contains(value: "R-01: HEADING_RELATION",
                        comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: "register.capture", diagnostic: "CAPTURE_EXACT",
                location: "85 IDs, 84 heading occurrences, 24 donor exemptions and missing R-01 relation");
    }

    /// <summary>Rejects a capture that omits an expected donor heading.</summary>
    [Fact, Trait("Case", "register.omit-heading")]
    public void ValidateRegister_CaptureMissingHeading_Rejected()
    {
        RejectCapture(id: "register.omit-heading", mutate: (m, r, e) => (m["headings"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().RemoveAt(index: 0), expected: "CAPTURE: HEADING_INVENTORY");
    }

    /// <summary>Rejects a capture that omits the R-01 matrix row.</summary>
    [Fact, Trait("Case", "register.omit-matrix")]
    public void ValidateRegister_CaptureMissingMatrixRow_Rejected()
    {
        RejectCapture(id: "register.omit-matrix", mutate: (m, r, e) => (m["matrix"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().RemoveAt(index: 0), expected: "R-01: SOURCE_ROW");
    }

    /// <summary>Rejects omission of an original donor exemption disposition.</summary>
    [Fact, Trait("Case", "register.omit-exemption")]
    public void ValidateRegister_CaptureMissingSourceExemption_Rejected()
    {
        RejectCapture(id: "register.omit-exemption", mutate: (m, r, e) => (e["donorExemptions"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().RemoveAt(index: 0), expected: "REGISTER: DONOR_EXEMPTION_INVENTORY");
    }

    /// <summary>Rejects duplicate rule IDs before deduplication could conceal them.</summary>
    [Fact, Trait("Case", "register.duplicate")]
    public void ValidateRegister_CaptureDuplicateId_RejectedBeforeSetConversion()
    {
        RejectCapture(id: "register.duplicate", mutate: (m, r, e) => (r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().Add(item: ((r["rules"] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null"))[0]
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .DeepClone()), expected: "R-01: RULE_ID: DUPLICATE_OR_EMPTY");
    }

    /// <summary>Checks both R-37 source headings and its pending 59/60/61 boundary mappings without claiming
    /// enforcement.</summary>
    [Fact, Trait("Case", "register.r37-trace")]
    public void ValidateRegister_R37Capture_ConnectsBothHeadingsAndProofs()
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        JsonNode row = ((input.Register["rules"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Single(predicate: x => StringComparer
                        .Ordinal.Equals(x: ((x ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["sourceId"]
                                ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null")).GetValue<string>(),
                                y: "R-37")) ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null"));
        (row["headingRefs"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Count.ShouldBe(expected: 2);
        ((row["mechanism"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["path"] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<string>().ShouldBe(
                                expected: "tools/code-rules/BodyLengthAnalyzer.cs");
        ((row["mechanism"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["diagnostics"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().Select(selector: x => (x ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<string>()).ShouldContain(
                                expected: "SCR0006", comparer: StringComparer.Ordinal);
        ((row["mechanism"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["status"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .GetValue<string>().ShouldBe(expected: "pending");
        ((row["numeric"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["boundaries"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().Select(selector: x => (x ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<int>()).ShouldBe(expected: _bodyBoundaries);
        ((row["positiveProof"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["id"] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<string>().ShouldBe(
                                expected: "R-37.positive");
        ((row["negativeProof"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))["id"] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).GetValue<string>().ShouldBe(
                                expected: "R-37.negative");
        ProofProtocol.Record(id: "register.r37-trace", diagnostic: "R37_TRACE_EXACT",
                location: ("R-37 source identity → SCR0006 pending implementation →"
                        + " declared 59/60/61 boundaries; no enforcement pass clai" + "med"));
    }

    /// <summary>Requires an enforcement mechanism for an applicable rule.</summary>
    [Fact, Trait("Case", "register.schema")]
    public void ValidateRegister_SchemaMissingMechanism_Rejected()
    {
        RejectCapture(id: "register.schema", mutate: (m, r, e) => ((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[0]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] = null, expected: "R-01: MECHANISM_REQUIRED");
    }

    /// <summary>Reports the rule ID when its heading relation field is absent.</summary>
    [Fact, Trait("Case", "register.missing-relation-field")]
    public void ValidateRegister_SchemaAbsentRelationField_ReportsActionableFailure()
    {
        RejectCapture(id: "register.missing-relation-field", mutate: (m, r, e) => ((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[0]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsObject().Remove(propertyName: "headingRefs"), expected: "R-01: HEADING_RELATION");
    }

    /// <summary>Checks all 85 rules, 84 headings and 24 exemptions, including the absent EX-22 identity.</summary>
    [Fact, Trait("Case", "ac-05.complete")]
    public void ValidateRegister_CompleteInventory_HasEveryExactDisposition()
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        (input.Manifest["matrix"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Count.ShouldBe(expected: 85);
        (input.Manifest["headings"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Count.ShouldBe(expected: 84);
        (input.Exemptions["donorExemptions"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Count.ShouldBe(expected: 24);
        (input.Exemptions["donorExemptions"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Any(predicate: x => StringComparer.Ordinal
                        .Equals(x: ((x ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["sourceId"]
                                ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null")).GetValue<string>(),
                                y: "EX-22")).ShouldBeFalse();
        ProofProtocol.Record(id: "ac-05.complete", diagnostic: "INVENTORY_EXACT",
                location: ("complete local immutable source inventory and all dispo"
                        + "sitions; enforcement separately pending"));
    }

    /// <summary>Rejects a second R-01 declaration with a conflicting disposition.</summary>
    [Fact, Trait("Case", "edge.rule-02.adjacency")]
    public void ValidateRegister_ConflictingDuplicate_RejectsMaskedOriginal()
    {
        RejectCapture(id: "edge.rule-02.adjacency", mutate: (m, r, e) =>
        {
            JsonNode copy = ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null")).DeepClone();
            copy["disposition"] = "inapplicable";
            (r["rules"] ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .AsArray().Add(item: copy);
        }, expected: "R-01: RULE_ID: DUPLICATE_OR_EMPTY");
    }

    /// <summary>Rejects nine detached mutations that empty inventories or remove required mapping fields.</summary>
    [Fact, Trait("Case", "edge.rule-02.empty")]
    public void ValidateRegister_EmptyRequiredFields_RejectsEveryMissingValue()
    {
        var mutations = new (Action<JsonObject, JsonObject, JsonObject> Apply, string Diagnostic)[]
        {
            ((m, r, e) => (r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsArray().Clear(), "REGISTER: RULE_INVENTORY"),
            ((m, r, e) => (m["headings"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsArray().Clear(), "CAPTURE: HEADING_INVENTORY"),
            ((m, r, e) => (m["files"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsArray().Clear(), "CAPTURE: FILE_INVENTORY"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["reason"] = " ", "R-01: DISPOSITION"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["scope"] = "", "R-01: SCOPE_REQUIRED"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[1] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["adaptationReason"] = null,
                                    "R-02: ADAPTATION_REASON"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["positiveProof"] = null,
                                    "R-01: POSITIVE_PROOF_REQUIRED"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["negativeProof"] = null,
                                    "R-01: NEGATIVE_PROOF_REQUIRED"),
            ((m, r, e) => ((r["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["documentation"] = null,
                                    "R-01: DOCUMENTATION_REFERENCE"),
        };
        foreach ((Action<JsonObject, JsonObject, JsonObject> Apply, string Diagnostic) mutation in mutations)
        {
            (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
            mutation.Apply(input.Manifest, input.Register, input.Exemptions);
            RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                    .Exemptions).ShouldContain(elementPredicate: x => x.Contains(value: mutation.Diagnostic,
                            comparisonType: StringComparison.Ordinal));
        }

        ProofProtocol.Record(id: "edge.rule-02.empty", diagnostic: "REQUIRED_FIELDS_REJECTED",
                location: "nine detached empty/missing inventory and required mapping mutations");
    }

    /// <summary>Rejects identity spelling changes, invalid UTF-8 and duplicate JSON keys.</summary>
    [Fact, Trait("Case", "edge.rule-02.encoding")]
    public void ValidateRegister_AlternateEncoding_PreservesExactIdentity()
    {
        foreach (string? value in new[]
        {
            "r-01",
            "R-001",
            "R-０１",
            "R-01\u0301",
            "R-0\u0131"
        }

        )
        {
            (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
            ((input.Register["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["sourceId"] = value;
            RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                    .Exemptions).ShouldContain(expected: "REGISTER: RULE_INVENTORY", comparer: StringComparer.Ordinal);
        }

        Should.Throw<System.Text.DecoderFallbackException>(actual: () => RegisterValidator.Parse(bytes: [0xC3, 0x28]));
        Should.Throw<InvalidDataException>(actual: () => RegisterValidator.Parse(bytes: System.Text.Encoding
                .UTF8.GetBytes(s: "{\"rules\":[],\"rules\":[]}"))).Message.ShouldContain(expected: "JSON_DUPLICATE");
        ProofProtocol.Record(id: "edge.rule-02.encoding", diagnostic: "IDENTITY_EXACT",
                location: ("case, width, leading zero, combining and dotless charac"
                        + "ters; malformed UTF-8 and duplicate JSON keys"));
    }

    /// <summary>Checks that row and relation permutations preserve coverage while unknown rule IDs still
    /// fail.</summary>
    [Fact, Trait("Case", "edge.rule-02.ordering")]
    public void ValidateRegister_Permutations_PreserveVerdictAndCoverage()
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        IReadOnlyList<string> before = RegisterValidator.Validate(manifest: input.Manifest, register: input
                .Register, exemptions: input.Exemptions);
        before.ShouldBeEmpty();
        foreach (string? name in new[]
        {
            "files",
            "headings",
            "matrix",
            "exemptions"
        }

        )
        {
            Reverse(obj: input.Manifest, name: name);
        }

        Reverse(obj: input.Register, name: "rules");
        Reverse(obj: input.Exemptions, name: "donorExemptions");
        Reverse(obj: input.Exemptions, name: "local");
        foreach (JsonNode? heading in (input.Manifest["headings"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray())
        {
            Reverse(obj: (heading ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsObject(), name: "sourceIds");
        }

        foreach (JsonNode? rule in (input.Register["rules"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray())
        {
            Reverse(obj: (rule ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")).AsObject(), name: "headingRefs");
        }

        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBe(expected: before);
        ((input.Register["rules"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null"))["sourceId"] = "R-unknown";
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldContain(expected: "REGISTER: RULE_INVENTORY", comparer: StringComparer.Ordinal);
        ProofProtocol.Record(id: "edge.rule-02.ordering", diagnostic: "PERMUTATION_EQUAL",
                location: ("row/file/heading/relation permutations preserve full co"
                        + "verage; unknown ID remains rejected"));
    }

    /// <summary>Rejects a mechanism path whose filename casing differs from the declared symbol.</summary>
    [Fact, Trait("Case", "register.reference-case")]
    public void ValidateRegister_SchemaCaseChangedReference_Rejected()
    {
        RejectCapture(id: "register.reference-case", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))[
                                        "path"] = "tools/code-rules/bodyLengthAnalyzer.cs",
                expected: "R-37: MECHANISM_REFERENCE");
    }

    /// <summary>Rejects a mechanism symbol that differs from the exact analyzer name.</summary>
    [Fact, Trait("Case", "register.symbol")]
    public void ValidateRegister_SchemaWrongMechanismSymbol_Rejected()
    {
        RejectCapture(id: "register.symbol", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["symbol"] = "bodyLengthAnalyzer",
                expected: "R-37: MECHANISM_REFERENCE");
    }

    /// <summary>Detects a captured mechanism field changed from the immutable donor row.</summary>
    [Fact, Trait("Case", "register.source-field")]
    public void ValidateRegister_SchemaAlteredCapturedMechanism_Rejected()
    {
        RejectCapture(id: "register.source-field", mutate: (m, r, e) => ((m["matrix"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] = "SCR0007", expected: "R-37: SOURCE_FIELD: mechanism");
    }

    /// <summary>Rejects a proof path that does not match the rule-specific analyzer test.</summary>
    [Fact, Trait("Case", "register.proof-path")]
    public void ValidateRegister_SchemaWrongProofPath_Rejected()
    {
        RejectCapture(id: "register.proof-path", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "positiveProof"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))[
                                        "path"] = "src/godot-game/tests/verification/Analyzers/bodyLengthAnalyzerTests.cs",
                expected: "R-37: PROOF_PATH");
    }

    /// <summary>Keeps composed and decomposed Unicode paths distinct from the required ASCII path.</summary>
    [Fact, Trait("Case", "register.reference-unicode")]
    public void ValidateRegister_SchemaUnicodeReference_Rejected()
    {
        foreach (string? spelling in new[]
        {
            "BodyLéngthAnalyzer.cs",
            "BodyLe\u0301ngthAnalyzer.cs"
        }

        )
        {
            (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
            (((input.Register["rules"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[36] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null"))["mechanism"]
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                            "path"] = "tools/code-rules/" + spelling;
            RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                    .Exemptions).ShouldContain(expected: "R-37: MECHANISM_REFERENCE", comparer: StringComparer.Ordinal);
        }

        ProofProtocol.Record(id: "register.reference-unicode", diagnostic: "R-37: MECHANISM_REFERENCE",
                location: "both composed and decomposed spellings remain distinct from exact ASCII path");
    }

    /// <summary>Rejects evidence mapping borrowed from another rule.</summary>
    [Fact, Trait("Case", "register.proof-reverse")]
    public void ValidateRegister_SchemaProofForDifferentRule_Rejected()
    {
        RejectCapture(id: "register.proof-reverse", mutate: (m, r, e) => ((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[0]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "positiveProof"] = (((r["rules"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))[1]
                                ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null"))["positiveProof"]
                                ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null")).DeepClone(),
                expected: "R-01: PROOF_REVERSE_ID");
    }

    /// <summary>Rejects a rule mechanism whose required error severity is weakened.</summary>
    [Fact, Trait("Case", "register.severity")]
    public void ValidateRegister_SchemaSeverityWeakening_Rejected()
    {
        RejectCapture(id: "register.severity", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["severity"] = "warning",
                expected: "R-37: SEVERITY_WEAKENED");
    }

    /// <summary>Rejects substitution of a different custom diagnostic ID.</summary>
    [Fact, Trait("Case", "register.diagnostic")]
    public void ValidateRegister_SchemaDiagnosticSubstitution_Rejected()
    {
        RejectCapture(id: "register.diagnostic", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "mechanism"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["diagnostics"] = new JsonArray(
                                        "SCR0007"), expected: "R-37: DIAGNOSTIC_IDENTITY");
    }

    /// <summary>Requires the exact rule identity in the documentation reference.</summary>
    [Fact, Trait("Case", "register.documentation")]
    public void ValidateRegister_SchemaWrongDocumentationAnchor_Rejected()
    {
        RejectCapture(id: "register.documentation", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[0]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "documentation"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["symbol"] = "r-01",
                expected: "R-01: DOCUMENTATION_REFERENCE");
    }

    /// <summary>Rejects a body-length limit changed from the captured numeric policy.</summary>
    [Fact, Trait("Case", "register.numeric")]
    public void ValidateRegister_SchemaNumericLimitDrift_Rejected()
    {
        RejectCapture(id: "register.numeric", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "numeric"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["limit"] = 61,
                expected: "R-37: NUMERIC_LIMIT");
    }

    /// <summary>Rejects a line-length counting unit changed to Unicode scalar values.</summary>
    [Fact, Trait("Case", "register.numeric-semantics")]
    public void ValidateRegister_SchemaNumericCountingChange_Rejected()
    {
        RejectCapture(id: "register.numeric-semantics", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[9]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "numeric"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["unit"] = "Unicode scalar values",
                expected: "R-10: NUMERIC_SEMANTICS");
    }

    /// <summary>Rejects duplicate boundary proof IDs instead of counting one case twice.</summary>
    [Fact, Trait("Case", "register.boundary-proof")]
    public void ValidateRegister_SchemaReusedBoundaryEvidence_Rejected()
    {
        RejectCapture(id: "register.boundary-proof", mutate: (m, r, e) => (((((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[36]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "numeric"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["boundaryProofs"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[1]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "id"] = "rules.body.59", expected: "rules.body.59: BOUNDARY_PROOF: DUPLICATE_OR_EMPTY");
    }

    /// <summary>Prevents a pending qualitative review from being represented as an automated pass.</summary>
    [Fact, Trait("Case", "register.review")]
    public void ValidateRegister_SchemaAutomatedHumanJudgment_Rejected()
    {
        RejectCapture(id: "register.review", mutate: (m, r, e) => (((r["rules"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[23]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "qualitativeReview"] ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["status"] = "pass",
                expected: "R-24: HUMAN_REVIEW_PENDING");
    }

    /// <summary>Rejects duplicate local exemption IDs before scopes can be combined.</summary>
    [Fact, Trait("Case", "register.exemption-duplicate")]
    public void ValidateRegister_SchemaDuplicateExemption_Rejected()
    {
        RejectCapture(id: "register.exemption-duplicate", mutate: (m, r, e) => (e["local"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .AsArray().Add(item: ((e["local"] ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null"))[0]
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .DeepClone()), expected: "KZ-EX-LOCK-01: LOCAL_EXEMPTION_ID: DUPLICATE_OR_EMPTY");
    }

    /// <summary>Rejects a differently named exemption overlapping an existing diagnostic scope.</summary>
    [Fact, Trait("Case", "register.exemption-overlap")]
    public void ValidateRegister_SchemaOverlappingExemptions_Rejected()
    {
        RejectCapture(id: "register.exemption-overlap", mutate: (m, r, e) =>
        {
            JsonNode clone = ((e["local"] ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null"))[0] ?? throw new System.InvalidOperationException(
                            message: "Required proof value is null")).DeepClone();
            clone["id"] = "KZ-EX-LOCK-02";
            (e["local"] ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                    .AsArray().Add(item: clone);
        }, expected: "KZ-EX-LOCK-02: EXEMPTION_OVERLAP");
    }

    /// <summary>Rejects replacing an exact lock-file exemption with a directory glob.</summary>
    [Fact, Trait("Case", "register.exemption-broad")]
    public void ValidateRegister_SchemaBroadenedExemptionScope_Rejected()
    {
        RejectCapture(id: "register.exemption-broad", mutate: (m, r, e) => ((e["local"]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[0]
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"))[
                        "path"] = "tests/**/packages.lock.json", expected: "KZ-EX-LOCK-01: EXEMPTION_SCOPE");
    }

    /// <summary>Requires pending mechanisms and reviews to remain incomplete when implementation is required.</summary>
    [Fact, Trait("Case", "register.pending")]
    public void ValidateRegister_FutureMappings_RejectsCompletionCredit()
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        IReadOnlyList<string> strict = RegisterValidator.Validate(manifest: input.Manifest, register: input
                .Register, exemptions: input.Exemptions, requireImplemented: true);
        strict.ShouldNotContain(expected: "R-37: IMPLEMENTATION_PENDING", comparer: StringComparer.Ordinal);
        strict.ShouldContain(expected: "R-49: IMPLEMENTATION_PENDING", comparer: StringComparer.Ordinal);
        strict.ShouldNotContain(expected: "R-37: PROOF_UNIMPLEMENTED", comparer: StringComparer.Ordinal);
        strict.ShouldNotContain(expected: "R-09: PROOF_UNIMPLEMENTED", comparer: StringComparer.Ordinal);
        strict.ShouldContain(expected: "R-24: HUMAN_REVIEW_PENDING", comparer: StringComparer.Ordinal);
        ProofProtocol.Record(id: "register.pending", diagnostic: "IMPLEMENTATION_PENDING",
                location: ("structural capture passes; unbound enforcement and qual"
                        + "itative judgment cannot satisfy full AC-06"));
    }

    /// <summary>Checks R43 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-43.register-resolution")]
    public void ValidateRegister_ResolutionR43_IsExplicit()
    {
        Resolution(id: "R-43");
    }

    /// <summary>Checks R57 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-57.register-resolution")]
    public void ValidateRegister_ResolutionR57_IsExplicit()
    {
        Resolution(id: "R-57");
    }

    /// <summary>Checks R76 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-76.register-resolution")]
    public void ValidateRegister_ResolutionR76_IsExplicit()
    {
        Resolution(id: "R-76");
    }

    /// <summary>Checks R79 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-79.register-resolution")]
    public void ValidateRegister_ResolutionR79_IsExplicit()
    {
        Resolution(id: "R-79");
    }

    /// <summary>Checks R80 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-80.register-resolution")]
    public void ValidateRegister_ResolutionR80_IsExplicit()
    {
        Resolution(id: "R-80");
    }

    /// <summary>Checks R85 has an explicit inapplicable disposition and reason without enforcement mappings.</summary>
    [Fact, Trait("Case", "R-85.register-resolution")]
    public void ValidateRegister_ResolutionR85_IsExplicit()
    {
        Resolution(id: "R-85");
    }

    private static readonly int[] _bodyBoundaries = [59, 60, 61];
    private static (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) Inputs()
    {
        return (RegisterValidator.Read(relative: "docs/code/provenance/manifest.json"), RegisterValidator.Read(
                relative: "docs/code/register.json"), RegisterValidator.Read(relative: "docs/code/exemptions.json"));
    }

    private static void RejectCapture(string id, Action<JsonObject, JsonObject, JsonObject> mutate, string expected)
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        mutate(input.Manifest, input.Register, input.Exemptions);
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldContain(elementPredicate: x => x.Contains(value: expected,
                        comparisonType: StringComparison.Ordinal));
        ProofProtocol.Record(id: id, diagnostic: expected, location: "detached captured inventory mutation");
    }

    private static void Reverse(JsonObject obj, string name)
    {
        JsonNode[] rows = [..(obj[name] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Select(selector: x => (x
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .DeepClone()).Reverse()];
        obj[name] = new JsonArray(items: rows);
    }

    private static void Resolution(string id)
    {
        (JsonObject Manifest, JsonObject Register, JsonObject Exemptions) input = Inputs();
        RegisterValidator.Validate(manifest: input.Manifest, register: input.Register, exemptions: input
                .Exemptions).ShouldBeEmpty();
        JsonNode row = ((input.Register["rules"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).AsArray().Single(predicate: x => StringComparer
                        .Ordinal.Equals(x: ((x ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null"))["sourceId"]
                                ?? throw new System.InvalidOperationException(
                                        message: "Required proof value is null")).GetValue<string>(), y: id))
                ?? throw new System.InvalidOperationException(message: "Required proof value is null"));
        (row["disposition"] ?? throw new System.InvalidOperationException(
                message: "Required proof value is null")).GetValue<string>().ShouldBe(expected: "inapplicable");
        (row["reason"] ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                .GetValue<string>().ShouldNotBeNullOrWhiteSpace();
        row["mechanism"].ShouldBeNull();
        ProofProtocol.Record(id: id + ".register-resolution", diagnostic: "INAPPLICABLE_ACCOUNTED",
                location: id + " HTTP/database-only mechanism explicitly accounted without enforcement credit");
    }
}
