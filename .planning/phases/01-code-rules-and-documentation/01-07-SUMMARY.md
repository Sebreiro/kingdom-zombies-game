---
phase: 01-code-rules-and-documentation
plan: 07
subsystem: testing
tags: [roslyn, analyzers, continuation, imports, qualification, declarations, godot]
requires:
  - phase: 01-06
    provides: Eight inherited analyzers, exact diagnostic verifier and bounded generated-layout proofs
provides:
  - SCR0009-SCR0012 enforcement with all applicable captured cases and exact Error spans
  - Unique continuation data rows and immediate 7/8/9 offsets with block and string distinctions
  - Exact twelve-analyzer source consumer and two immutable Godot syntax-span permissions
affects: [01-08, 01-12, 01-14, 01-15, 01-16, 01-17]
tech-stack:
  added: []
  patterns: [Distinct manifest data rows, complete-text and precise-span generated permissions]
key-files:
  created: [tools/code-rules/ContinuationIndentAnalyzer.cs, tools/code-rules/UsingOrderAnalyzer.cs, tools/code-rules/ThisQualificationAnalyzer.cs, tools/code-rules/LocalNewDeclarationAnalyzer.cs, tools/code-rules/GeneratedSyntax.cs, tests/phase1/Analyzers/ContinuationIndentAnalyzerTests.cs, tests/phase1/Analyzers/UsingOrderAnalyzerTests.cs, tests/phase1/Analyzers/ThisQualificationAnalyzerTests.cs, tests/phase1/Analyzers/LocalNewDeclarationAnalyzerTests.cs, tests/phase1/GeneratedSyntaxAdapterTests.cs, docs/code/generated-syntax-adaptations.json]
  modified: [tests/phase1/AnalyzerSourceConsumerTests.cs, tests/phase1/ConfigurationTests.cs, tests/phase1/RegisterTests.cs, tools/phase1/required-cases.json, tools/code-rules/AnalyzerReleases.Unshipped.md, docs/code/provenance/analyzer-adoption.json, src/core/ProofValue.cs, src/godot-game/GeneratedCallSiteAdapters.cs, src/godot-game/GeneratedReportAdapter.cs, src/godot-game/proofs/Phase1.cs]
key-decisions:
  - Preserve all twelve donor descriptors and exact inherited syntax exceptions; generated markers grant no permission.
  - Enumerate the twelve registered enabled Error diagnostics explicitly and compile all analyzer source under them.
  - Bound two observed Godot syntax permissions by diagnostic, path, complete Roslyn text hash and precise span; verify raw output and package origin separately.
requirements-completed: [RULE-02, RULE-03]
requirements-status: Pending shared phase acceptance; no RULE requirement is completed by this plan alone
coverage:
  - id: D1
    description: Continuation and import ordering preserve captured semantics and immediate numeric boundaries
    verification:
      - kind: integration
        ref: .phase1/runs/5a29a8aa-54df-4a2b-9add-a7e5cfc3978e/summary.json
        status: pass
    human_judgment: false
  - id: D2
    description: Qualification and local-new declarations enforce exact diagnostic spans and documented exceptions
    verification:
      - kind: integration
        ref: tests/phase1/Analyzers/ThisQualificationAnalyzerTests.cs; tests/phase1/Analyzers/LocalNewDeclarationAnalyzerTests.cs
        status: pass
    human_judgment: false
  - id: D3
    description: Actual twelve-analyzer consumer and exact Godot generated syntax boundaries
    verification:
      - kind: integration
        ref: rules.analyzer-source-consumer; rules.generated-syntax-origin; rules.generated-syntax-boundary
        status: pass
    human_judgment: false
actuals:
  tokens: 87567
  tasks: 2
  commits: 5
plan_head_before: 1eeb2b2e593566a3bb2856614b74c6b9f8ee9fed
plan_head_after: 63557e20e66469648388e06231f5d9dd1a0f58bc
duration: 47min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 7: Continuation, Imports, Qualification and Local Creation Summary

**SCR0009-SCR0012 enforce captured continuation, import, qualification and local-creation rules with exact Error spans, while two observed Godot syntax exceptions require immutable content and precise locations.**

## Performance

- Observed source implementation and verification window: 47min, from the first created proof source at 2026-11-13T20:24:13Z through final reconciliation at 21:10:57Z.
- Tasks: 2 complete; 38 production, proof, configuration and provenance files changed; 11 created.
- Actual tokens are chars/4 of the realized Git diff, rounded upward. Five production/task commits are measured from the persisted ledger; close-out metadata is separate.

## Task Commits

| Task | Name | Commit | Type |
| --- | --- | --- | --- |
| 1 | Continuation and import RED subjects | 4026eb2 | test |
| 1 | Captured continuation/import enforcement and diagnosed prerequisites | 0b11a9c | feat |
| 2 | Qualification and local declaration RED subjects | 00e96c4 | test |
| 2 | Qualification/local creation and explicit twelve-analyzer consumer | a651101 | feat |
| 2 | Actual whole-solution prerequisites and bounded Godot syntax compatibility | 63557e2 | fix |

No tracked file was deleted. No dependency/version was added. Captured donor originals and approved requirements remain unchanged. Adoption details are in `docs/code/provenance/analyzer-adoption.json`.

## Verification

- Final command: `node tools/phase1/check.cjs --all`.
- Final run: `.phase1/runs/5a29a8aa-54df-4a2b-9add-a7e5cfc3978e/summary.json` — **374 passed, 0 failed implemented controls, 184 blocked pending, 558 declared**. Aggregate status remains nonpassing/exit 1 because the future obligations are pending.
- Final input identity: `898a9366cdb3861989721d2fa002d6abd4d6183fe55ecccc613802e1959032eb`; source revision: `63557e20e66469648388e06231f5d9dd1a0f58bc`. Fresh nonce/input/tool/platform/report/evidence reconciliation succeeds for every implemented case, with no duplicate test binding.
- Focused raw suites passed 119/119 continuation, 20/20 import and 11/11 qualification cases. Local declarations initially passed 12/13; the sole unused-local fixture warning was corrected and its exact fresh case passed. Final reconciliation independently executes all 13 local declaration cases and every other implemented case.
- Each captured continuation theory row has a separate stable ID/result. Immediate 7/9-space neighbors complement 8-space controls across all captured continuation forms, including conditional chains, binary/conditional operators, constraints, initializers, base lists and lambda bodies. Four-space collection contents, same-indent brackets/block-lambda braces, string contents and excluded syntax retain their donor distinctions. Existing 59/60/61 body and 119/120/121 UTF-16 boundaries remain active and pass.
- Constructor chaining, `this` values/indexers/extension receivers and C# 14 extension members retain their exceptions. Local declarations include using/await-using, for/using statements, multiple declarators, fields/properties, arguments/returns, collections and top-level statements. Exact ID, Error severity, file/start/end span, intended messages where applicable and zero unexpected compiler diagnostics are required.
- The real analyzer-source consumer requires exactly SCR0001-SCR0012, enabled at Error, then compiles every owned analyzer source under the built analyzers. All twelve release rows are active in `AnalyzerReleases.Unshipped.md`; the captured shipped header remains unchanged.
- `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false` passes all five projects with zero warnings/errors. Native Windows Godot probe `9fde648c-4f3c-4fa4-ad6a-ac854d299a5c` passes construction/ready/exit and runtime identity as raw compatibility evidence. This adds no engine-lifetime, Linux or coverage acceptance claim.
- All six controls blocked in the first full run pass their own fresh reconciliations and the final complete run: `pins.consumer`, `pins.generated-report`, `pins.generated-report-boundary`, `rules.generated-layout-origin`, `rules.generated-layout-boundary`, `rules.godot-callsite-boundary`.
- `git diff <plan_head_before> HEAD --check` passed before the final run. No source changed during final reconciliation.

## Generated Syntax Boundaries

Two observed Godot 4.7.2 generator outputs need compatibility: SCR0009 on the initialization parameter at line 12, column 13, length 6; SCR0011 on `this.@_nonce` in the property getter at line 23, column 81, length 12. The register pins raw BOM-bearing source and package SHA-256 identities separately from Roslyn's BOM-free complete text identity.

Only the exact diagnostic/path/text-hash/span tuple is permitted. Actual origin builds pass; changed content, wrong path, shifted or widened spans fail permission checks. Real handwritten Roslyn subjects at the exact registered coordinates/path still produce the intended diagnostics because their content differs. Actual adjacent handwritten Godot members reject both SCR0009 and SCR0011, and both exact-signature impostors reject with CS0111. These are fresh distinct proofs, not reused results.

The ten existing SCR0001 member scopes, four layout-source records, seven layout-member scopes and original Godot RS0030 scope retain their exact values. No member-wide, type-wide or folder-wide permission was added for the new syntax cases.

## TDD Gate Compliance

All four targets below executed after successful build/reference resolution and failed the intended exact-diagnostic assertion with an empty actual diagnostic set before adoption. Their unchanged JUnit reports and records in `.phase1/red-01-07/` returned `RED_EVIDENCE_OK`; semantic inspection confirms missing planned behavior rather than infrastructure failure.

| Rule | RED target | Intended missing diagnostic |
| --- | --- | --- |
| R-09 | WrappedArgument4Or12SpacesFails4 | SCR0009 on both wrapped argument tokens |
| R-14 | AnalyzeSystemTextBeforeSystemReports | SCR0010 on the out-of-order directive |
| R-28 | AnalyzeFieldAssignmentReports | SCR0011 on the qualified member access |
| R-18 | AnalyzeTypedLocalWithTargetTypedNewReports | SCR0012 on the complete local declaration |

Each RED commit precedes its GREEN commit. Import/build/formatting setup errors are not credited as RED. Historical bootstrap missing-RED ledger entry 1 remains open.

## Deviations from Plan

1. **[Rule 3 — Selector mismatch]** Authored `rules.family7.1/.2` selectors return `SELECTION_EMPTY`. Exact existing rule IDs and distinct captured/data-row IDs supply actual verification; no alias or duplicate passing result was invented. WINDOWS 19 is fixed.
2. **[Rule 3 — Diagnosed handwritten prerequisites]** Active continuation/import rules require compliant layout in proof sources and analyzer source. Strict compilation additionally requires nullable captured return/event contracts and real uses of assigned/declared fixture variables. Whole-solution compilation exposes Core's `this.Value` and Godot adapter/proof continuation indents; these are corrected without changing values or permission targets. Commits: 0b11a9c, a651101, 63557e2; WINDOWS 20 and 22 are fixed.
3. **[Rule 3 — Formatter dependency]** The captured formatter proof uses the SDK-bundled `dotnet format whitespace --folder --verify-no-changes` command instead of adding Workspaces packages. The agreed sample, with strict nullable fixture annotations, remains unchanged by formatting. The command contract follows https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-format. Commit: 0b11a9c; WINDOWS 21 is fixed.
4. **[Rule 2 — Required generated syntax compatibility]** The two precisely registered observed spans above enable the authentic generator while rejecting content/path/span changes, same-coordinate handwritten subjects, adjacent members and colliding definitions. Commit: 63557e2; WINDOWS 22 is fixed.

First full run `b8d2860a-c844-4f46-bd9b-f1221857863e` recorded 366 passes, six failures and 184 pending obligations. Those six failures cascade from the shared Core build prerequisite and are not credited as intended negative proofs. The strict solution then exposed the two generated incompatibilities above. Focused origin/boundary, solution, native Godot and six affected controls passed before the final full run. The final run supersedes that failed run.

## Remaining Phase Work

- WINDOWS analyzer-shell entry 8 is fixed: all twelve source families are real and proven. No current-plan stub, skipped test or unrun actual proof remains.
- Historical bootstrap RED entry 1 and five distinct runner-edge bindings in entry 9 remain open; 01-14 retains closure ownership for the latter.
- Wider enforcement/register activation/exemption validation belongs to 01-08. Graph, state, lifecycle, documentation, coverage and actual Linux proofs retain their planned owners. All RULE requirements remain Pending.
- The prepared local Linux VM was preserved; environment preparation is not Linux acceptance evidence.
- Next plan: **01-08**. Phase progress: **8/18**; Phase 1 remains incomplete.

## Self-Check: PASSED

All eleven created artifacts and this summary exist; all five task/production commits are ancestors of HEAD. No tracked deletion, current-plan stub or unmodeled network/auth/schema boundary was found. Final reconciliation has zero failed implemented obligations.
