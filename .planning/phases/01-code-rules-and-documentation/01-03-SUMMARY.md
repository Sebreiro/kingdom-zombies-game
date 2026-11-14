---
phase: 01-code-rules-and-documentation
plan: "03"
subsystem: testing
tags: [csharp, provenance, rule-register, exact-identity, exemptions]
requires:
  - phase: 01-02
    provides: Trusted C# proof runner and independent required-case reconciliation
provides:
  - Immutable local capture of 151 original donor files at the researched revision
  - Exact 85-rule, 84-heading-occurrence and 24-exemption inheritance inventory
  - Strict register, reference, numeric metadata and exemption corruption controls
affects: [01-04, 01-05, 01-06, 01-07, 01-08, 01-14, phase1-verification]
actuals:
  tokens: 102159
  tasks: 2
  commits: 4
plan_head_before: 14f580d433a160360f4a660df13f2408fc4e487f
plan_head_after: 36a66a6c834d85df0499f8cd6794d3ebb9709418
tech-stack:
  added: []
  patterns: [immutable Git-blob capture, ordinal many-to-many rule identity, separate structural and implemented validation, fail-closed scoped exemptions]
key-files:
  created: [docs/code/provenance/langproj-baseline.zip, docs/code/provenance/manifest.json, docs/code/register.json, docs/code/exemptions.json, tests/phase1/RegisterValidator.cs, tests/phase1/RegisterTests.cs]
  modified: [tools/phase1/required-cases.json]
key-decisions:
  - "Capture original blobs at the known revision; donor paths are never runtime, build or documentation-resolution dependencies."
  - "Structural register acceptance cannot satisfy implemented enforcement or human review; AC-06 and real rule proofs remain blocked."
  - "Retain only the observed exact NuGet lock-file candidate exemption; future analyzer, generated and coverage scopes require local evidence before activation."
requirements-completed: []
requirements-addressed: [RULE-02, RULE-03]
coverage:
  - id: D1
    description: Exact captured donor inventory and classified source relations
    requirement: RULE-02
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case register.capture
        status: pass
      - kind: integration
        ref: ac-05.complete and all 36 implemented register cases in final --all report
        status: pass
    human_judgment: false
  - id: D2
    description: Strict mutation rejection and explicit pending enforcement/review gate
    requirement: RULE-03
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case register.schema
        status: pass
      - kind: integration
        ref: register.pending rejects acceptance as implemented evidence
        status: pass
    human_judgment: false
duration: 17min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 03: Captured donor baseline and executable register Summary

**A repository-owned 151-file donor capture now anchors all 85 source rules, 84 heading occurrences and 24 exemptions, with C# proofs rejecting missing, ambiguous, altered or weakened mappings.**

## Performance

- Observed execution window: 2026-11-13T17:41:50Z through approximately 17:58:02Z; 17 minutes rounded up. Context loading preceded this window.
- Tasks: 2/2. Six new implementation/provenance artifacts and one existing required-case manifest changed.
- Actual estimate scale: 408,633 realized Git text-diff characters / 4, rounded up to 102,159. The 348,027-byte binary ZIP is represented by Git's binary-change marker in this text metric; its complete per-file identities and original rule rows are additionally present in the manifest. The measured four implementation commits exclude this summary and subsequent planning-state commits.

## Accomplishments

- Captured clean donor HEAD `6836a9718ff5c939e89abf2b97763760607de192` using `git archive --format=zip` at that exact revision. Archive SHA-256: `19c3766fddf638b02a2b78d969e222448ecd98a22b79f70e8b28effd858dfcca`. All four research-hashed documents match their observed hashes. Original Git paths and bytes are retained without modifications.
- The capture includes the source rule documents and decision record, root/backend indexes, shared/root editorconfigs, SDK/build/package pins, both banned-symbol files, all twelve analyzer implementations, MemberBodies, release manifests, their tests/verifier, and the architecture/documentation/process/real-build proof helpers. All 151 file paths, sizes, roles and SHA-256 values are recorded. No LICENSE/NOTICE file was found in the donor tree; the manifest records that fact and the creator-provided source provenance without inventing a license grant.
- Preserved all 49 style and 35 architecture heading occurrences with original exact path, line, title and ID relations. Shared Banned APIs and XML headings and repeated subjects remain many-to-many relationships. The register has exactly one researched retained/adapted/inapplicable disposition per source ID, with reasons, scopes, local documentation selectors, precise planned mechanism/proof paths, diagnostics, review components and numeric semantics.
- Classified all 24 donor exemption rows individually, preserving the EX-21 to EX-23 gap. EF/web/font permissions were not imported. The sole local candidate is the observed exact `tests/phase1/packages.lock.json` final-newline exception, with paired future boundary proof IDs and pending status. The candidate grants no completed-proof credit.
- The C# validator compares the manifest with immutable original ZIP entries, original heading declarations and matrix/exemption rows. It rejects duplicate IDs before set conversion, unknown identities, missing relations/fields, captured-source field drift, invalid JSON/UTF-8, case/Unicode substitutions, wrong mechanism symbols/paths, reverse proof mismatches, reused proof identities, diagnostic substitutions, severity weakening, numeric-counting changes, and duplicate/overlapping/broadened exemptions. Permutations of rows, files, headings and relations preserve the verdict.
- R-37 traces both original headings to SCR0006 and the precise planned BodyLength analyzer/test paths. Separate `rules.body.59`, `.60` and `.61` obligations are declared pending for 01-06. Structural capture validation passes while `requireImplemented: true` reports missing real mechanisms/proofs and outstanding human judgment. No future analyzer enforcement is represented as passing.

## Task Commits

1. **Task 1: Validate one captured rule through complete inventory identity** — `68df5ac` (validated RED), `948aa38` (GREEN).
2. **Task 2: Reject register, reference and exemption weakening** — `127b617` (validated RED), `36a66a6` (GREEN).

No tracked file was deleted. Existing `.planning/state.json` and `.planning/tmp/` were preserved and excluded from staging. Capture/binding development scripts and runtime reports live in ignored `.phase1/` output; they are not build dependencies.

## Verification and Evidence

| Command or control | Actual result | Evidence |
| --- | --- | --- |
| `node tools/phase1/check.cjs --case register.capture` | Pass; baseline accepted and omitted R-01 heading relation rejected | [focused capture report](../../../.phase1/runs/b14a0d66-6aaa-4f42-9d66-bc0747d3b438/summary.json) |
| Task 1 complete implemented suite, `--all` | 47 passes, zero failed controls, 222 explicit later blockers; 269 selected | [Task 1 full report](../../../.phase1/runs/92ed4a80-bb44-401f-bc64-d2c613ff6f45/summary.json) |
| `node tools/phase1/check.cjs --case register.schema` | Pass; missing mechanism rejected as `R-01: MECHANISM_REQUIRED` | [focused schema report](../../../.phase1/runs/7c8f3b03-0ba9-4fe7-bc13-6970514e6bd6/summary.json) |
| Intermediate `--group register` | 30 implemented passes; full AC-06 explicitly blocked | [register report](../../../.phase1/runs/c1426ded-f4a6-47bf-94ef-7eff93b095b4/summary.json) |
| Final `node tools/phase1/check.cjs --all` on the final committed implementation bytes | Expected exit 1: **77 implemented passes, zero failed controls, 214 explicit future blockers; 291 selected**. Includes all 36 new register controls plus all 41 existing implemented controls | [final complete-inventory report](../../../.phase1/runs/6493f882-daf9-47b2-bfab-1c79dbda7bd7/summary.json), [console log](../../../.phase1/temp/01-03-final-all.log) |
| `git diff --check`, artifact and commit ancestry checks | Pass; no deletion, stub marker or donor-runtime-path dependency found in authored implementation | Actual command exits and final self-check |

The final report runs on source bytes later committed in `36a66a6`; its input snapshot records those bytes even though its Git revision precedes that commit. The full inventory remains nonpassing by design until future implementation, coverage, human review and actual Linux obligations are satisfied. No required task verification was skipped or substituted by an infrastructure error.

## TDD Gate Compliance

- **Task 1 RED:** `Capture_MissingHeadingRelation_IsRejected` compiled and executed, then failed its Shouldly predicate because the initial validator returned no R-01 missing-relation diagnostic. The unchanged JUnit report received `RED_EVIDENCE_OK` after selecting its actual fully qualified emitted test identity. [RED record](../../../.phase1/tdd/01-03-task1/red-evidence.json), [original report](../../../.phase1/tdd/01-03-task1/red.xml). Semantic inspection confirmed the intended assertion, successful build and one executed test.
- **Task 2 RED:** `Schema_MissingMechanism_IsRejected` compiled and executed, then failed its intended assertion because the capture-only validator accepted a missing mechanism. Its unchanged JUnit report likewise received `RED_EVIDENCE_OK` using the emitted fully qualified identity. [RED record](../../../.phase1/tdd/01-03-task2/red-evidence.json), [original report](../../../.phase1/tdd/01-03-task2/red.xml). Semantic inspection confirmed the missing-mapping assertion rather than setup failure.
- The classifier initially rejected each simple method-name selector as `no_target_test_failure`; only the evidence selector was corrected to match the actual report. Neither production implementation proceeded before classifier approval. Both task RED commits precede their own GREEN commits. No separate refactor commit was necessary.
- The Task 1 tracer was rerun within the fresh complete suite before Task 2 expansion. Its captured source identity passes; actual analyzer enforcement intentionally remains owned by later plans.

## Decisions Made

- Keep captured donor bytes as provenance, with current local C# validation and later operational rule definitions providing the executable authority. No command reaches the external donor after capture.
- Preserve all explicit original PR-review components, including serialization, enum, configuration and inapplicable health-check judgments. Structural verification never signs off those judgments.
- Fail closed for candidate exemptions and future mechanism/proof statuses. Precise planned references support subsequent implementation without manufacturing completed AC-06 or rule-family evidence.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 2 - Critical correctness] Bind the plan's real cases into the independent obligation manifest.**
- The dependency manifest had no `register.capture` or `register.schema` entries and no concrete bindings for this plan's register cases. The task commands otherwise selected zero obligations.
- Added exact actual C# method/trait/diagnostic bindings, enabled the implemented AC-05/edge/inapplicable-resolution controls, and declared the three separate future R-37 boundary obligations pending.
- File: `tools/phase1/required-cases.json`; verified through the trusted normal entry point and the final full run. Commits: `68df5ac`, `948aa38`, `127b617`, `36a66a6`.
- Recorded as fixed entry 4 in `WINDOWS.md`. Historical open entry 1 remains unchanged.

No package acquisition, authentication gate, branch change, donor mutation or automatic approval rejection occurred. Existing packages and tool versions were reused.

## Known Stubs and Threat Review

No placeholder implementation, TODO/FIXME marker or skipped current-plan test remains. Pending register/proof/exemption fields are explicit later-plan declarations and are rejected as complete evidence; they are not passing substitutes. Full AC-06, machine-rule enforcement, exemption boundary builds, human judgments, coverage and actual Linux execution remain required.

No new network endpoint, authentication, schema or external runtime-file surface was introduced. ZIP integrity is pinned before parsing; archive and JSON reads are bounded, canonical paths and exact IDs are checked, duplicate JSON keys fail, UTF-8 is strict, and the existing fresh-input/evidence reconciliation covers every executed proof. ZIP entries are inspected without extracting files.

## Remaining Phase Work

All RULE-01 through RULE-07 requirements remain Pending. This increment completes plan 01-03 only. The next action is plan 01-04; progress is 3/18 plans in Phase 1. Phase 2 is not started. Actual rule enforcement and precise observed generator/analyzer exemptions belong to subsequent plans; the current capture does not certify them.

## Self-Check: PASSED

All seven implementation/provenance paths exist. All four implementation commits are ancestors of HEAD. The final full report contains 77 actual passes and zero failed controls, with all 214 remaining selected obligations explicitly blocked. No tracked deletion occurred; no donor path is required at build/test/runtime. This summary is written to the canonical plan path before planning progress advances.
