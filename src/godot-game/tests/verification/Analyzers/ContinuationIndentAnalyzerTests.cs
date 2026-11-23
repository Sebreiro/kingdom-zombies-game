using System;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests.Analyzers;
/// <summary>
/// In-memory proofs of SCR0009 for every continuation kind agreed in plan 01-06: the continuation line passes at
/// 8 spaces past the line it continues and fails at 4 or 12. The block layouts SCR0009 owns (block-lambda braces,
/// collection-expression brackets and elements) pass at their agreed indent and fail elsewhere. Kinds left to
/// IDE0055 or never checked are not reported at any indent. Each sample is a template: <c>~</c> stands for the
/// offset under test and <c>«</c>…<c>»</c> wraps each token that must be reported when the offset is wrong.
/// </summary>
public sealed class ContinuationIndentAnalyzerTests
{
    /// <summary>Wrapped arguments, each on its own line, pass at 8 spaces past the line holding the <c>(</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "R-09.positive")]
    public Task WrappedArgument_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "R-09.positive", diagnostic: "ANALYZER_ACCEPTED", source: Sample(
                template: WrappedArgument, offset: 8, marked: false));
    }

    /// <summary>Wrapped arguments at 4 or 12 spaces fail, each on its first token.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects WrappedArgument at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "R-09.negative")]
    public Task Analyze_WrappedArgument4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "R-09.negative", diagnostic: "SCR0009", source: Sample(
                template: WrappedArgument, offset: 4, marked: true));
    }

    /// <summary>Rejects WrappedArgument at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedArgument_4Or12Spaces_Fails12")]
    public Task Analyze_WrappedArgument12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedArgument_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: WrappedArgument, offset: 12, marked: true));
    }

    /// <summary>A wrapped attribute argument passes at 8 spaces past the attribute's line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.WrappedAttributeArgument_8Spaces_Passes")]
    public Task WrappedAttributeArgument_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedAttributeArgument_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: WrappedAttributeArgument, offset: 8,
                        marked: false));
    }

    /// <summary>A wrapped attribute argument at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects WrappedAttributeArgument at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedAttributeArgument_4Or12Spaces_Fails4")]
    public Task Analyze_WrappedAttributeArgument4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedAttributeArgument_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: WrappedAttributeArgument, offset: 4, marked: true));
    }

    /// <summary>Rejects WrappedAttributeArgument at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedAttributeArgument_4Or12Spaces_Fails12")]
    public Task Analyze_WrappedAttributeArgument12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedAttributeArgument_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: WrappedAttributeArgument, offset: 12, marked: true));
    }

    /// <summary>Wrapped parameters pass at 8 spaces past the declaration's line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.WrappedParameter_8Spaces_Passes")]
    public Task WrappedParameter_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedParameter_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: WrappedParameter, offset: 8, marked: false));
    }

    /// <summary>Wrapped parameters at 4 or 12 spaces fail.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects WrappedParameter at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedParameter_4Or12Spaces_Fails4")]
    public Task Analyze_WrappedParameter4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedParameter_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: WrappedParameter, offset: 4, marked: true));
    }

    /// <summary>Rejects WrappedParameter at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedParameter_4Or12Spaces_Fails12")]
    public Task Analyze_WrappedParameter12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedParameter_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: WrappedParameter, offset: 12, marked: true));
    }

    /// <summary>Fluent-chain lines pass at 8 spaces past the line that starts the chain.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.FluentChain_8Spaces_Passes")]
    public Task FluentChain_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.FluentChain_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: FluentChain, offset: 8, marked: false));
    }

    /// <summary>Fluent-chain lines at 4 or 12 spaces fail, on their <c>.</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects FluentChain at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.FluentChain_4Or12Spaces_Fails4")]
    public Task Analyze_FluentChain4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.FluentChain_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: FluentChain, offset: 4, marked: true));
    }

    /// <summary>Rejects FluentChain at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.FluentChain_4Or12Spaces_Fails12")]
    public Task Analyze_FluentChain12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.FluentChain_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: FluentChain, offset: 12, marked: true));
    }

    /// <summary>A <c>?.</c> chain line passes at 8 spaces past the line that starts the chain.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessChain_8Spaces_Passes")]
    public Task ConditionalAccessChain_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessChain_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ConditionalAccessChain, offset: 8,
                        marked: false));
    }

    /// <summary>A <c>?.</c> chain line at 4 or 12 spaces fails, on its <c>?</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ConditionalAccessChain at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessChain_4Or12Spaces_Fails4")]
    public Task Analyze_ConditionalAccessChain4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessChain_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessChain, offset: 4, marked: true));
    }

    /// <summary>Rejects ConditionalAccessChain at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessChain_4Or12Spaces_Fails12")]
    public Task Analyze_ConditionalAccessChain12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessChain_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessChain, offset: 12, marked: true));
    }

    /// <summary>
    /// Several <c>?.</c> and <c>?[</c> steps, each on its own line, all pass at 8 spaces past the line that starts
    /// the chain, not one more level for each step.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessSteps_8Spaces_Passes")]
    public Task ConditionalAccessSteps_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessSteps_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ConditionalAccessSteps, offset: 8,
                        marked: false));
    }

    /// <summary>Several <c>?.</c> and <c>?[</c> steps at 4 or 12 spaces fail, each on its <c>?</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ConditionalAccessSteps at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessSteps_4Or12Spaces_Fails4")]
    public Task Analyze_ConditionalAccessSteps4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessSteps_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessSteps, offset: 4, marked: true));
    }

    /// <summary>Rejects ConditionalAccessSteps at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessSteps_4Or12Spaces_Fails12")]
    public Task Analyze_ConditionalAccessSteps12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessSteps_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessSteps, offset: 12, marked: true));
    }

    /// <summary>
    /// A <c>.</c> line after a <c>?.</c> line passes at 8 spaces past the line that starts the chain.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.MixedChain_8Spaces_Passes")]
    public Task MixedChain_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.MixedChain_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: MixedChain, offset: 8, marked: false));
    }

    /// <summary>A chain mixing <c>.</c> and <c>?.</c> lines at 4 or 12 spaces fails on every line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects MixedChain at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.MixedChain_4Or12Spaces_Fails4")]
    public Task Analyze_MixedChain4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.MixedChain_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: MixedChain, offset: 4, marked: true));
    }

    /// <summary>Rejects MixedChain at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.MixedChain_4Or12Spaces_Fails12")]
    public Task Analyze_MixedChain12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.MixedChain_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: MixedChain, offset: 12, marked: true));
    }

    /// <summary>A line starting with a binary operator passes at 8 spaces past the left operand's line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.LeadingBinaryOperator_8Spaces_Passes")]
    public Task LeadingBinaryOperator_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LeadingBinaryOperator_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: LeadingBinaryOperator, offset: 8,
                        marked: false));
    }

    /// <summary>A line starting with a binary operator at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects LeadingBinaryOperator at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LeadingBinaryOperator_4Or12Spaces_Fails4")]
    public Task Analyze_LeadingBinaryOperator4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LeadingBinaryOperator_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: LeadingBinaryOperator, offset: 4, marked: true));
    }

    /// <summary>Rejects LeadingBinaryOperator at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LeadingBinaryOperator_4Or12Spaces_Fails12")]
    public Task Analyze_LeadingBinaryOperator12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LeadingBinaryOperator_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: LeadingBinaryOperator, offset: 12, marked: true));
    }

    /// <summary>The right operand after a trailing binary operator passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.TrailingBinaryOperator_8Spaces_Passes")]
    public Task TrailingBinaryOperator_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.TrailingBinaryOperator_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: TrailingBinaryOperator, offset: 8,
                        marked: false));
    }

    /// <summary>The right operand after a trailing binary operator at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects TrailingBinaryOperator at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.TrailingBinaryOperator_4Or12Spaces_Fails4")]
    public Task Analyze_TrailingBinaryOperator4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.TrailingBinaryOperator_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: TrailingBinaryOperator, offset: 4, marked: true));
    }

    /// <summary>Rejects TrailingBinaryOperator at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.TrailingBinaryOperator_4Or12Spaces_Fails12")]
    public Task Analyze_TrailingBinaryOperator12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.TrailingBinaryOperator_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: TrailingBinaryOperator, offset: 12, marked: true));
    }

    /// <summary>A wrapped <c>if (</c> condition passes at 8 spaces past the <c>if</c> line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.WrappedIfCondition_8Spaces_Passes")]
    public Task WrappedIfCondition_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedIfCondition_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: WrappedIfCondition, offset: 8,
                        marked: false));
    }

    /// <summary>A wrapped <c>if (</c> condition at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects WrappedIfCondition at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedIfCondition_4Or12Spaces_Fails4")]
    public Task Analyze_WrappedIfCondition4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedIfCondition_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: WrappedIfCondition, offset: 4, marked: true));
    }

    /// <summary>Rejects WrappedIfCondition at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedIfCondition_4Or12Spaces_Fails12")]
    public Task Analyze_WrappedIfCondition12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedIfCondition_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: WrappedIfCondition, offset: 12, marked: true));
    }

    /// <summary>The <c>?</c> and <c>:</c> lines of a conditional operator pass at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalOperator_8Spaces_Passes")]
    public Task ConditionalOperator_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalOperator_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ConditionalOperator, offset: 8,
                        marked: false));
    }

    /// <summary>The <c>?</c> and <c>:</c> lines at 4 or 12 spaces both fail.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ConditionalOperator at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalOperator_4Or12Spaces_Fails4")]
    public Task Analyze_ConditionalOperator4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalOperator_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ConditionalOperator, offset: 4, marked: true));
    }

    /// <summary>Rejects ConditionalOperator at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalOperator_4Or12Spaces_Fails12")]
    public Task Analyze_ConditionalOperator12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalOperator_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ConditionalOperator, offset: 12, marked: true));
    }

    /// <summary>A constructor initializer passes at 8 spaces past the constructor's first line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ConstructorInitializer_8Spaces_Passes")]
    public Task ConstructorInitializer_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConstructorInitializer_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ConstructorInitializer, offset: 8,
                        marked: false));
    }

    /// <summary>A constructor initializer at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ConstructorInitializer at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConstructorInitializer_4Or12Spaces_Fails4")]
    public Task Analyze_ConstructorInitializer4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConstructorInitializer_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ConstructorInitializer, offset: 4, marked: true));
    }

    /// <summary>Rejects ConstructorInitializer at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConstructorInitializer_4Or12Spaces_Fails12")]
    public Task Analyze_ConstructorInitializer12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConstructorInitializer_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ConstructorInitializer, offset: 12, marked: true));
    }

    /// <summary>A <c>where</c> clause passes at 8 spaces past the declaration's first line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.WhereClause_8Spaces_Passes")]
    public Task WhereClause_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WhereClause_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: WhereClause, offset: 8, marked: false));
    }

    /// <summary>A <c>where</c> clause at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects WhereClause at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WhereClause_4Or12Spaces_Fails4")]
    public Task Analyze_WhereClause4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WhereClause_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: WhereClause, offset: 4, marked: true));
    }

    /// <summary>Rejects WhereClause at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WhereClause_4Or12Spaces_Fails12")]
    public Task Analyze_WhereClause12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WhereClause_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: WhereClause, offset: 12, marked: true));
    }

    /// <summary>A value on the line after <c>=</c> in a declaration passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterEquals_8Spaces_Passes")]
    public Task ValueAfterEquals_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterEquals_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ValueAfterEquals, offset: 8, marked: false));
    }

    /// <summary>A value on the line after <c>=</c> in a declaration at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ValueAfterEquals at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterEquals_4Or12Spaces_Fails4")]
    public Task Analyze_ValueAfterEquals4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterEquals_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterEquals, offset: 4, marked: true));
    }

    /// <summary>Rejects ValueAfterEquals at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterEquals_4Or12Spaces_Fails12")]
    public Task Analyze_ValueAfterEquals12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterEquals_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterEquals, offset: 12, marked: true));
    }

    /// <summary>A value on the line after an assignment's <c>=</c> passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterAssignment_8Spaces_Passes")]
    public Task ValueAfterAssignment_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterAssignment_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ValueAfterAssignment, offset: 8,
                        marked: false));
    }

    /// <summary>A value on the line after an assignment's <c>=</c> at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ValueAfterAssignment at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterAssignment_4Or12Spaces_Fails4")]
    public Task Analyze_ValueAfterAssignment4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterAssignment_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterAssignment, offset: 4, marked: true));
    }

    /// <summary>Rejects ValueAfterAssignment at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterAssignment_4Or12Spaces_Fails12")]
    public Task Analyze_ValueAfterAssignment12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterAssignment_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterAssignment, offset: 12, marked: true));
    }

    /// <summary>A value on the line after <c>return</c> passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterReturn_8Spaces_Passes")]
    public Task ValueAfterReturn_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterReturn_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ValueAfterReturn, offset: 8, marked: false));
    }

    /// <summary>A value on the line after <c>return</c> at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ValueAfterReturn at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterReturn_4Or12Spaces_Fails4")]
    public Task Analyze_ValueAfterReturn4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterReturn_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterReturn, offset: 4, marked: true));
    }

    /// <summary>Rejects ValueAfterReturn at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterReturn_4Or12Spaces_Fails12")]
    public Task Analyze_ValueAfterReturn12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterReturn_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterReturn, offset: 12, marked: true));
    }

    /// <summary>An expression-bodied member's <c>=&gt;</c> on the next line passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.ExpressionBodyArrow_8Spaces_Passes")]
    public Task ExpressionBodyArrow_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ExpressionBodyArrow_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: ExpressionBodyArrow, offset: 8,
                        marked: false));
    }

    /// <summary>An expression-bodied member's <c>=&gt;</c> at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects ExpressionBodyArrow at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ExpressionBodyArrow_4Or12Spaces_Fails4")]
    public Task Analyze_ExpressionBodyArrow4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ExpressionBodyArrow_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: ExpressionBodyArrow, offset: 4, marked: true));
    }

    /// <summary>Rejects ExpressionBodyArrow at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ExpressionBodyArrow_4Or12Spaces_Fails12")]
    public Task Analyze_ExpressionBodyArrow12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ExpressionBodyArrow_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: ExpressionBodyArrow, offset: 12, marked: true));
    }

    /// <summary>A base list on the next line passes at 8 spaces past the type's first line.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.BaseList_8Spaces_Passes")]
    public Task BaseList_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BaseList_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: BaseList, offset: 8, marked: false));
    }

    /// <summary>A base list on the next line at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects BaseList at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BaseList_4Or12Spaces_Fails4")]
    public Task Analyze_BaseList4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BaseList_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: BaseList, offset: 4, marked: true));
    }

    /// <summary>Rejects BaseList at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BaseList_4Or12Spaces_Fails12")]
    public Task Analyze_BaseList12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BaseList_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: BaseList, offset: 12, marked: true));
    }

    /// <summary>A lambda expression body on the line after <c>=&gt;</c> passes at 8 spaces.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.LambdaExpressionBody_8Spaces_Passes")]
    public Task LambdaExpressionBody_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LambdaExpressionBody_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: LambdaExpressionBody, offset: 8,
                        marked: false));
    }

    /// <summary>A lambda expression body on the line after <c>=&gt;</c> at 4 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects LambdaExpressionBody at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LambdaExpressionBody_4Or12Spaces_Fails4")]
    public Task Analyze_LambdaExpressionBody4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LambdaExpressionBody_4Or12Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: LambdaExpressionBody, offset: 4, marked: true));
    }

    /// <summary>Rejects LambdaExpressionBody at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LambdaExpressionBody_4Or12Spaces_Fails12")]
    public Task Analyze_LambdaExpressionBody12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LambdaExpressionBody_4Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: LambdaExpressionBody, offset: 12, marked: true));
    }

    /// <summary>Block-lambda braces pass at the indent of the line where the lambda starts.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.BlockLambdaBraces_AtLambdaLineIndent_Passes")]
    public Task BlockLambdaBraces_AtLambdaLineIndent_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BlockLambdaBraces_AtLambdaLineIndent_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: BlockLambdaBraces, offset: 0, marked: false));
    }

    /// <summary>Block-lambda braces 4 or 8 spaces past the lambda's line fail, both of them.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects BlockLambdaBraces at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BlockLambdaBraces_4Or8Spaces_Fails4")]
    public Task Analyze_BlockLambdaBraces4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BlockLambdaBraces_4Or8Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: BlockLambdaBraces, offset: 4, marked: true));
    }

    /// <summary>Rejects BlockLambdaBraces at offset 8 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BlockLambdaBraces_4Or8Spaces_Fails8")]
    public Task Analyze_BlockLambdaBraces8Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BlockLambdaBraces_4Or8Spaces_Fails8",
                diagnostic: "SCR0009", source: Sample(template: BlockLambdaBraces, offset: 8, marked: true));
    }

    /// <summary>A collection expression's <c>[</c> on the line after <c>=</c> passes at that line's indent.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.CollectionOpenBracket_AtEqualsLineIndent_Passes")]
    public Task CollectionOpenBracket_AtEqualsLineIndent_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionOpenBracket_AtEqualsLineIndent_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: CollectionOpenBracket, offset: 0,
                        marked: false));
    }

    /// <summary>A collection expression's <c>[</c> 4 or 8 spaces past the <c>=</c> line fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects CollectionOpenBracket at offset 4 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.CollectionOpenBracket_4Or8Spaces_Fails4")]
    public Task Analyze_CollectionOpenBracket4Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionOpenBracket_4Or8Spaces_Fails4",
                diagnostic: "SCR0009", source: Sample(template: CollectionOpenBracket, offset: 4, marked: true));
    }

    /// <summary>Rejects CollectionOpenBracket at offset 8 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.CollectionOpenBracket_4Or8Spaces_Fails8")]
    public Task Analyze_CollectionOpenBracket8Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionOpenBracket_4Or8Spaces_Fails8",
                diagnostic: "SCR0009", source: Sample(template: CollectionOpenBracket, offset: 8, marked: true));
    }

    /// <summary>
    /// A collection expression on one line after <c>=</c> has no block layout: it is a value and passes at 8 spaces.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.OneLineCollectionAfterEquals_8Spaces_Passes")]
    public Task OneLineCollectionAfterEquals_8Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OneLineCollectionAfterEquals_8Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: OneLineCollection, offset: 8, marked: false));
    }

    /// <summary>A one-line collection expression after <c>=</c> at 0 or 12 spaces fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects OneLineCollectionAfterEquals at offset 0 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.OneLineCollectionAfterEquals_0Or12Spaces_Fails0")]
    public Task Analyze_OneLineCollectionAfterEquals0Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OneLineCollectionAfterEquals_0Or12Spaces_Fails0",
                diagnostic: "SCR0009", source: Sample(template: OneLineCollection, offset: 0, marked: true));
    }

    /// <summary>Rejects OneLineCollectionAfterEquals at offset 12 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.OneLineCollectionAfterEquals_0Or12Spaces_Fails12")]
    public Task Analyze_OneLineCollectionAfterEquals12Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OneLineCollectionAfterEquals_0Or12Spaces_Fails12",
                diagnostic: "SCR0009", source: Sample(template: OneLineCollection, offset: 12, marked: true));
    }

    /// <summary>Collection-expression elements pass at 4 spaces past the line holding the <c>[</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.CollectionElement_4Spaces_Passes")]
    public Task CollectionElement_4Spaces_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionElement_4Spaces_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: CollectionElement, offset: 4, marked: false));
    }

    /// <summary>A collection-expression element at 0 or 8 spaces past the <c>[</c> line fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    /// <summary>Rejects CollectionElement at offset 0 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.CollectionElement_0Or8Spaces_Fails0")]
    public Task Analyze_CollectionElement0Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionElement_0Or8Spaces_Fails0",
                diagnostic: "SCR0009", source: Sample(template: CollectionElement, offset: 0, marked: true));
    }

    /// <summary>Rejects CollectionElement at offset 8 on its exact token.</summary>
    [Fact, Trait("Case", "rules.continuationindent.CollectionElement_0Or8Spaces_Fails8")]
    public Task Analyze_CollectionElement8Spaces_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionElement_0Or8Spaces_Fails8",
                diagnostic: "SCR0009", source: Sample(template: CollectionElement, offset: 8, marked: true));
    }

    /// <summary>A collection expression's <c>]</c> passes at the indent of the line holding the <c>[</c>.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.CollectionCloseBracket_AtOpenLineIndent_Passes")]
    public Task CollectionCloseBracket_AtOpenLineIndent_Passes()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionCloseBracket_AtOpenLineIndent_Passes",
                diagnostic: "ANALYZER_ACCEPTED", source: Sample(template: CollectionCloseBracket, offset: 0,
                        marked: false));
    }

    /// <summary>A collection expression's <c>]</c> 4 spaces past the <c>[</c> line fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.CollectionCloseBracket_4Spaces_Fails")]
    public Task CollectionCloseBracket_4Spaces_Fails()
    {
        return VerifyAsync(caseId: "rules.continuationindent.CollectionCloseBracket_4Spaces_Fails",
                diagnostic: "SCR0009", source: Sample(template: CollectionCloseBracket, offset: 4, marked: true));
    }

    /// <summary>
    /// Nested continuations are measured from the line they continue: inner arguments sit 8 spaces past the line
    /// holding their own <c>(</c>, and a chain inside a lambda 8 spaces past that chain's first line.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.NestedContinuations_8SpacesPastTheirOwnLine_Pass")]
    public Task NestedContinuations_8SpacesPastTheirOwnLine_Pass()
    {
        return VerifyAsync(caseId: "rules.continuationindent.NestedContinuations_8SpacesPastTheirOwnLine_Pass",
                diagnostic: "ANALYZER_ACCEPTED", source: NestedContinuations);
    }

    /// <summary>An inner wrapped argument at the outer argument's indent fails.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.NestedArgument_AtOuterIndent_Fails")]
    public Task NestedArgument_AtOuterIndent_Fails()
    {
        return VerifyAsync(caseId: "rules.continuationindent.NestedArgument_AtOuterIndent_Fails",
                diagnostic: "SCR0009", source: """
                class C
                {
                    int M(int first, int second)
                    {
                        return M(
                                first: M(
                                {|SCR0009:first|}: 1,
                                {|SCR0009:second|}: 2),
                                second: 3);
                    }
                }
                """);
    }

    /// <summary>Object and collection initializers and anonymous objects are left to IDE0055.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.Initializers_AnyIndent_NotReported")]
    public Task Initializers_AnyIndent_NotReported()
    {
        return VerifyAsync(caseId: "rules.continuationindent.Initializers_AnyIndent_NotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: Initializers);
    }

    /// <summary>Switch-expression braces and arms and query clauses are left to IDE0055.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.SwitchExpressionAndQuery_AnyIndent_NotReported")]
    public Task SwitchExpressionAndQuery_AnyIndent_NotReported()
    {
        return VerifyAsync(caseId: "rules.continuationindent.SwitchExpressionAndQuery_AnyIndent_NotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: SwitchExpressionAndQuery);
    }

    /// <summary>Lines inside multi-line string literals, raw, verbatim and interpolated, are never checked.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.StringLiteralLines_AnyIndent_NotReported")]
    public Task StringLiteralLines_AnyIndent_NotReported()
    {
        return VerifyAsync(caseId: "rules.continuationindent.StringLiteralLines_AnyIndent_NotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: StringLiteralLines);
    }

    /// <summary>
    /// Kinds outside the agreed list are not reported: a closing parenthesis on its own line, a member's value on
    /// the line after its <c>=&gt;</c>, wrapped type arguments and statements.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.OtherKinds_AnyIndent_NotReported")]
    public Task OtherKinds_AnyIndent_NotReported()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OtherKinds_AnyIndent_NotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: OtherKinds);
    }

    /// <summary>
    /// A file holding every agreed kind at its agreed indent is left unchanged by the Roslyn formatter, which is
    /// what IDE0055 checks, and SCR0009 reports nothing in it: the two rules never conflict.
    /// </summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.AgreedSamples_FormatterLeavesUnchanged_AndNotReported")]
    public Task Format_AgreedSamples_PreservesBytesAndHasNoDiagnostic()
    {
        return FormatterAgreesAsync(
                caseId: "rules.continuationindent.AgreedSamples_FormatterLeavesUnchanged_AndNotReported",
                diagnostic: "ANALYZER_ACCEPTED", source: AgreedSamples);
    }

    /// <summary>Generated settings cannot exempt handwritten code.</summary>
    /// <returns>A task that completes when the verification has run.</returns>
    [Fact, Trait("Case", "rules.continuationindent.Analyze_GeneratedCode_IsSkipped")]
    public Task Analyze_GeneratedCode_IsSkipped()
    {
        return VerifyAsync(caseId: "rules.continuationindent.Analyze_GeneratedCode_IsSkipped",
                diagnostic: "SCR0009", source: Sample(template: WrappedArgument, offset: 4, marked: true),
                editorConfig: """
                        root = true

                        [*.cs]
                        generated_code = true
                        """);
    }

    /// <summary>Rejects the immediate 7-space neighbor for WrappedArgument.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedArgumentOffset7Fails")]
    public Task Analyze_WrappedArgumentOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedArgumentOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedArgument, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for WrappedArgument.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedArgumentOffset9Fails")]
    public Task Analyze_WrappedArgumentOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedArgumentOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedArgument, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for WrappedAttributeArgument.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedAttributeArgumentOffset7Fails")]
    public Task Analyze_WrappedAttributeArgumentOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedAttributeArgumentOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedAttributeArgument, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for WrappedAttributeArgument.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedAttributeArgumentOffset9Fails")]
    public Task Analyze_WrappedAttributeArgumentOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedAttributeArgumentOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedAttributeArgument, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for WrappedParameter.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedParameterOffset7Fails")]
    public Task Analyze_WrappedParameterOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedParameterOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedParameter, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for WrappedParameter.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedParameterOffset9Fails")]
    public Task Analyze_WrappedParameterOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedParameterOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedParameter, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for FluentChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.FluentChainOffset7Fails")]
    public Task Analyze_FluentChainOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.FluentChainOffset7Fails", diagnostic: "SCR0009",
                source: Sample(template: FluentChain, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for FluentChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.FluentChainOffset9Fails")]
    public Task Analyze_FluentChainOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.FluentChainOffset9Fails", diagnostic: "SCR0009",
                source: Sample(template: FluentChain, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ConditionalAccessChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessChainOffset7Fails")]
    public Task Analyze_ConditionalAccessChainOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessChainOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessChain, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ConditionalAccessChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessChainOffset9Fails")]
    public Task Analyze_ConditionalAccessChainOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessChainOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessChain, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ConditionalAccessSteps.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessStepsOffset7Fails")]
    public Task Analyze_ConditionalAccessStepsOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessStepsOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessSteps, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ConditionalAccessSteps.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalAccessStepsOffset9Fails")]
    public Task Analyze_ConditionalAccessStepsOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalAccessStepsOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalAccessSteps, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for MixedChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.MixedChainOffset7Fails")]
    public Task Analyze_MixedChainOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.MixedChainOffset7Fails", diagnostic: "SCR0009",
                source: Sample(template: MixedChain, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for MixedChain.</summary>
    [Fact, Trait("Case", "rules.continuationindent.MixedChainOffset9Fails")]
    public Task Analyze_MixedChainOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.MixedChainOffset9Fails", diagnostic: "SCR0009",
                source: Sample(template: MixedChain, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for LeadingBinaryOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LeadingBinaryOperatorOffset7Fails")]
    public Task Analyze_LeadingBinaryOperatorOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LeadingBinaryOperatorOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: LeadingBinaryOperator, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for LeadingBinaryOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LeadingBinaryOperatorOffset9Fails")]
    public Task Analyze_LeadingBinaryOperatorOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LeadingBinaryOperatorOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: LeadingBinaryOperator, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for TrailingBinaryOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.TrailingBinaryOperatorOffset7Fails")]
    public Task Analyze_TrailingBinaryOperatorOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.TrailingBinaryOperatorOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: TrailingBinaryOperator, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for TrailingBinaryOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.TrailingBinaryOperatorOffset9Fails")]
    public Task Analyze_TrailingBinaryOperatorOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.TrailingBinaryOperatorOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: TrailingBinaryOperator, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for WrappedIfCondition.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedIfConditionOffset7Fails")]
    public Task Analyze_WrappedIfConditionOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedIfConditionOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedIfCondition, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for WrappedIfCondition.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WrappedIfConditionOffset9Fails")]
    public Task Analyze_WrappedIfConditionOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WrappedIfConditionOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: WrappedIfCondition, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ConditionalOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalOperatorOffset7Fails")]
    public Task Analyze_ConditionalOperatorOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalOperatorOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalOperator, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ConditionalOperator.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConditionalOperatorOffset9Fails")]
    public Task Analyze_ConditionalOperatorOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConditionalOperatorOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ConditionalOperator, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ConstructorInitializer.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConstructorInitializerOffset7Fails")]
    public Task Analyze_ConstructorInitializerOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConstructorInitializerOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ConstructorInitializer, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ConstructorInitializer.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ConstructorInitializerOffset9Fails")]
    public Task Analyze_ConstructorInitializerOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ConstructorInitializerOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ConstructorInitializer, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for WhereClause.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WhereClauseOffset7Fails")]
    public Task Analyze_WhereClauseOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WhereClauseOffset7Fails", diagnostic: "SCR0009",
                source: Sample(template: WhereClause, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for WhereClause.</summary>
    [Fact, Trait("Case", "rules.continuationindent.WhereClauseOffset9Fails")]
    public Task Analyze_WhereClauseOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.WhereClauseOffset9Fails", diagnostic: "SCR0009",
                source: Sample(template: WhereClause, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ValueAfterEquals.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterEqualsOffset7Fails")]
    public Task Analyze_ValueAfterEqualsOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterEqualsOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterEquals, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ValueAfterEquals.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterEqualsOffset9Fails")]
    public Task Analyze_ValueAfterEqualsOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterEqualsOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterEquals, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ValueAfterAssignment.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterAssignmentOffset7Fails")]
    public Task Analyze_ValueAfterAssignmentOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterAssignmentOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterAssignment, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ValueAfterAssignment.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterAssignmentOffset9Fails")]
    public Task Analyze_ValueAfterAssignmentOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterAssignmentOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterAssignment, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ValueAfterReturn.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterReturnOffset7Fails")]
    public Task Analyze_ValueAfterReturnOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterReturnOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterReturn, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ValueAfterReturn.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ValueAfterReturnOffset9Fails")]
    public Task Analyze_ValueAfterReturnOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ValueAfterReturnOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ValueAfterReturn, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for ExpressionBodyArrow.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ExpressionBodyArrowOffset7Fails")]
    public Task Analyze_ExpressionBodyArrowOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ExpressionBodyArrowOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: ExpressionBodyArrow, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for ExpressionBodyArrow.</summary>
    [Fact, Trait("Case", "rules.continuationindent.ExpressionBodyArrowOffset9Fails")]
    public Task Analyze_ExpressionBodyArrowOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.ExpressionBodyArrowOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: ExpressionBodyArrow, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for BaseList.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BaseListOffset7Fails")]
    public Task Analyze_BaseListOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BaseListOffset7Fails", diagnostic: "SCR0009",
                source: Sample(template: BaseList, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for BaseList.</summary>
    [Fact, Trait("Case", "rules.continuationindent.BaseListOffset9Fails")]
    public Task Analyze_BaseListOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.BaseListOffset9Fails", diagnostic: "SCR0009",
                source: Sample(template: BaseList, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for LambdaExpressionBody.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LambdaExpressionBodyOffset7Fails")]
    public Task Analyze_LambdaExpressionBodyOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LambdaExpressionBodyOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: LambdaExpressionBody, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for LambdaExpressionBody.</summary>
    [Fact, Trait("Case", "rules.continuationindent.LambdaExpressionBodyOffset9Fails")]
    public Task Analyze_LambdaExpressionBodyOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.LambdaExpressionBodyOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: LambdaExpressionBody, offset: 9, marked: true));
    }

    /// <summary>Rejects the immediate 7-space neighbor for OneLineCollection.</summary>
    [Fact, Trait("Case", "rules.continuationindent.OneLineCollectionOffset7Fails")]
    public Task Analyze_OneLineCollectionOffset7_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OneLineCollectionOffset7Fails",
                diagnostic: "SCR0009", source: Sample(template: OneLineCollection, offset: 7, marked: true));
    }

    /// <summary>Rejects the immediate 9-space neighbor for OneLineCollection.</summary>
    [Fact, Trait("Case", "rules.continuationindent.OneLineCollectionOffset9Fails")]
    public Task Analyze_OneLineCollectionOffset9_ReportsDiagnostic()
    {
        return VerifyAsync(caseId: "rules.continuationindent.OneLineCollectionOffset9Fails",
                diagnostic: "SCR0009", source: Sample(template: OneLineCollection, offset: 9, marked: true));
    }

    private const StringComparison Ordinal = StringComparison.Ordinal;
    private const string WrappedArgument = """
        class C
        {
            int M(int first, int second)
            {
                return M(
                ~«first»: 1,
                ~«second»: 2);
            }
        }
        """;
    private const string WrappedAttributeArgument = """
        using System;

        class C
        {
            [Obsolete("Old.",
            ~«true»)]
            void M()
            {
            }
        }
        """;
    private const string WrappedParameter = """
        class C
        {
            void M(
            ~«int» first,
            ~«int» second)
            {
            }
        }
        """;
    private const string FluentChain = """
        using System.Linq;

        class C
        {
            object M(int[] items)
            {
                return items
                ~«.»Where(predicate: i => i > 0)
                ~«.»ToList();
            }
        }
        """;
    private const string ConditionalAccessChain = """
        class C
        {
            string? M(object value)
            {
                return value
                ~«?».ToString();
            }
        }
        """;
    private const string ConditionalAccessSteps = """
        class C
        {
            string? M(string[] items)
            {
                return items
                ~«?»[0]
                ~«?».Trim()
                ~«?».ToUpperInvariant();
            }
        }
        """;
    private const string MixedChain = """
        using System.Linq;

        class C
        {
            string? M(string[] items)
            {
                return items
                ~«.»FirstOrDefault()
                ~«?».Trim()
                ~«.»ToUpperInvariant();
            }
        }
        """;
    private const string LeadingBinaryOperator = """
        class C
        {
            bool M(int a, int b)
            {
                return a > 0
                ~«&&» b > 0;
            }
        }
        """;
    private const string TrailingBinaryOperator = """
        class C
        {
            bool M(int a, int b)
            {
                return a > 0 &&
                ~«b» > 0;
            }
        }
        """;
    private const string WrappedIfCondition = """
        class C
        {
            bool M(int a, int b)
            {
                if (a > 0
                ~«||» b > 0)
                {
                    return true;
                }

                return false;
            }
        }
        """;
    private const string ConditionalOperator = """
        class C
        {
            int M(int a, int b)
            {
                return a > b
                ~«?» a
                ~«:» b;
            }
        }
        """;
    private const string ConstructorInitializer = """
        class B
        {
            public B(int value)
            {
            }
        }

        class C : B
        {
            public C(int value)
            ~«:» base(value: value)
            {
            }
        }
        """;
    private const string WhereClause = """
        class C
        {
            T Make<T>()
            ~«where» T : new()
            {
                return new T();
            }
        }
        """;
    private const string ValueAfterEquals = """
        class C
        {
            int M()
            {
                int value =
                ~«M»();
                return value;
            }
        }
        """;
    private const string ValueAfterAssignment = """
        class C
        {
            int M()
            {
                int value;
                value =
                ~«M»();
                return value;
            }
        }
        """;
    private const string ValueAfterReturn = """
        class C
        {
            int M()
            {
                return
                ~«M»();
            }
        }
        """;
    private const string ExpressionBodyArrow = """
        class C
        {
            int Value
            ~«=>» 1;
        }
        """;
    private const string BaseList = """
        using System;

        class C
        ~«:» IDisposable
        {
            public void Dispose()
            {
            }
        }
        """;
    private const string LambdaExpressionBody = """
        using System;

        class C
        {
            void M()
            {
                Func<int, int> f = v =>
                ~«v» + 1;
            }
        }
        """;
    private const string BlockLambdaBraces = """
        using System;

        class C
        {
            void M()
            {
                Func<int, int> f = v =>
                ~«{»
                ~    return v + 1;
                ~«}»;
            }
        }
        """;
    private const string CollectionOpenBracket = """
        class C
        {
            int[] M()
            {
                int[] values =
                ~«[»
                ~    1,
                ~];
                return values;
            }
        }
        """;
    private const string OneLineCollection = """
        class C
        {
            private static readonly string[] _names =
            ~«[»"a", "b"];
        }
        """;
    private const string CollectionElement = """
        class C
        {
            int[] M()
            {
                int[] values = [
                    1,
                ~«2»,
                ];
                return values;
            }
        }
        """;
    private const string CollectionCloseBracket = """
        class C
        {
            int[] M()
            {
                int[] values = [
                    1,
                ~«]»;
                return values;
            }
        }
        """;
    private const string NestedContinuations = """
        using System;
        using System.Linq;

        class C
        {
            int M(int first, int second)
            {
                return M(
                        first: M(
                                first: 1,
                                second: 2),
                        second: 3);
            }

            object N(int[] items)
            {
                Run(
                        action: () =>
                        {
                            Run(action: () => { });
                        });
                return items
                        .Select(selector: i => i
                                .ToString())
                        .Select(selector: s =>
                        {
                            return s.Length;
                        })
                        .ToList();
            }

            void Run(Action action)
            {
            }
        }
        """;
    private const string Initializers = """
        using System.Collections.Generic;

        class C
        {
            public int Value { get; set; }

            object M()
            {
                var list = new List<int>
                      {
                   1,
                         };
                var anonymous = new
                     {
                   Value = 1,
                        };
                return new C
                           {
                Value =
             2,
                };
            }
        }
        """;
    private const string SwitchExpressionAndQuery = """
        using System.Linq;

        class C
        {
            object M(int value, int[] items)
            {
                int result = value switch
                     {
                   1 => 2,
                          _ => 0,
                 };
                var query = from i in items
                   where i > result
                             select i;
                return query;
            }
        }
        """;
    private const string StringLiteralLines = """"
        class C
        {
            string M(int count)
            {
                string raw = """
                  text
                       more
                  """;
                string verbatim = @"
           verbatim
         ";
                return $"""
            {count}
                {raw
              + verbatim}
            """;
            }
        }
        """";
    private const string OtherKinds = """
        using System.Collections.Generic;

        class C
        {
            int Value =>
                       1;

            int M(int first)
            {
                Dictionary<
              int,
                     string> map = [];
                    return M(
                            first: 1
           );
            }
        }
        """;
    private const string AgreedSamples = """
        using System;
        using System.Linq;

        internal class B
        {
            public B(int value)
            {
            }
        }

        [Obsolete("Old.",
                true)]
        internal sealed class C
                : B, IDisposable
        {
            public C(
                    int value,
                    int other)
                    : base(value: value)
            {
            }

            public int Value
                    => 1;

            public static T Make<T>()
                    where T : new()
            {
                return new T();
            }

            public void Dispose()
            {
            }

            public object M(int[] items, int a, int b)
            {
                int value =
                        Compute(
                                first: a,
                                second: b);
                bool ok = a > 0
                        && b > 0;
                bool also = a > 0 &&
                        b > 0;
                int pick = a > b
                        ? a
                        : b;
                Func<int, int> f = v =>
                        v + 1;
                Func<int, int> g = v =>
                {
                    return v + 1;
                };
                int[] values =
                [
                    value,
                    pick,
                ];
                if (ok
                        || also)
                {
                    value =
                            f(value) + g(pick);
                }

                return items
                        .Where(predicate: i => i > value)
                        .Select(selector: i =>
                        {
                            return i + values.Length;
                        })
                        .ToList();
            }

            public string N(object value)
            {
                return
                        value
                                ?.ToString() ?? string.Empty;
            }

            public string? O(string[] items)
            {
                return items
                        ?[0]
                        ?.Trim()
                        ?.ToUpperInvariant();
            }

            public string? P(string[] items)
            {
                return items
                        .FirstOrDefault()
                        ?.Trim()
                        .ToUpperInvariant();
            }

            private static int Compute(int first, int second)
            {
                return first + second;
            }
        }
        """;
    private static async Task FormatterAgreesAsync(string caseId, string diagnostic, string source)
    {
        string directory = WorkspacePaths.Temp(purpose: "format-"
                + System.Guid.NewGuid().ToString(format: "N"));
        System.IO.Directory.CreateDirectory(path: directory);
        string standalone = source.EndsWith(value: "\n", comparisonType: Ordinal) ? source : source + "\n";
        System.IO.File.WriteAllText(path: System.IO.Path.Combine(path1: directory, path2: "Sample.cs"),
                contents: standalone);
        (int Exit, string Output) result = ConfigurationTests.Dotnet("format", "whitespace", directory,
                "--folder", "--verify-no-changes", "--include", System.IO.Path.Combine(path1: directory,
                        path2: "Sample.cs"));
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        System.IO.File.ReadAllText(path: System.IO.Path.Combine(path1: directory, path2: "Sample.cs"))
                .ShouldBe(expected: standalone);
        await VerifyAsync(caseId: caseId, diagnostic: diagnostic, source: standalone).ConfigureAwait(
                continueOnCapturedContext: false);
    }

    private static string Sample(string template, int offset, bool marked)
    {
        return template.Replace(oldValue: "~", newValue: new string(c: ' ', count: offset),
                comparisonType: Ordinal).Replace(oldValue: "«", newValue: marked ? "{|SCR0009:" : "",
                        comparisonType: Ordinal).Replace(oldValue: "»", newValue: marked ? "|}" : "",
                                comparisonType: Ordinal);
    }

    private static Task VerifyAsync(string caseId, string diagnostic, string source, string? editorConfig = null)
    {
        return InheritedAnalyzerVerifier.VerifyAsync(caseId: caseId, diagnostic: diagnostic,
                analyzerType: "ContinuationIndentAnalyzer", source: source, editorConfig: editorConfig);
    }
}
