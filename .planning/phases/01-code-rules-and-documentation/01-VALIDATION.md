---
phase: "01"
slug: "code-rules-and-documentation"
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-13"
---

# Phase 01 — Validation Strategy

This is a validation contract, not a claim of executed evidence. The locked [specification](01-SPEC.md) and the complete acceptance map in [research](01-RESEARCH.md#validation-architecture) govern the required cases.

## Test Infrastructure

| Property | Value |
| --- | --- |
| Framework | Pinned C# proof project and actual Godot processes; exact package pairing proven during bootstrap |
| Config file | Repository toolchain manifest and required-case manifest, created in Wave 0 |
| Quick run command | `node tools/phase1/check.cjs --group state` |
| Full suite command | `node tools/phase1/check.cjs --all` |
| Platform gate | Complete fresh suite on Windows and Linux against the same source identity |
| Estimated runtime | Measure during bootstrap; all processes have explicit timeouts |

## Sampling Rate

- After each task: run its relevant automated group; prerequisite failures block dependent checks.
- After each implementation wave and before phase verification: run the complete available local suite.
- Prose-only changes: run `node tools/phase1/check.cjs --group docs`.
- Target quick feedback below 30 seconds after build; bounded build/engine/coverage checks remain required even when slower.
- Missing, skipped, duplicated, stale, or interrupted results cannot pass. Collect independent failures and report blocked obligations.

## Per-Task Verification Map

Exact ownership below refers to the18 checked plans (37tasks). All commands remain planned outputs until implemented; this document claims no executed compliance. Every negative proof must observe its intended subject diagnostic, not a restore/tool failure. Full source/decision/85-rule/38-edge allocation is in [COVERAGE.md](COVERAGE.md).

| ACs | Requirements | Automated command | Coverage obligation | File exists | Status |
| --- | --- | --- | --- | --- | --- |
| AC-01 | RULE-01 | `node tools/phase1/check.cjs --group pins` | Exact coherent pins; absent/conflicting/mismatched tools and reordered declarations | No — Wave 0 | pending |
| AC-02 | RULE-01, RULE-06 | `node tools/phase1/check.cjs --all` on each OS | Clean donor-independent restore/build/rules/real scene; source and platform identity | No — Wave 0 | pending |
| AC-03, AC-04 | RULE-01, RULE-03, RULE-04, RULE-07 | `node tools/phase1/check.cjs --group runner` | Real failure/zero discovery/filter/skip/crash/timeout/artifact/stale/duplicate/interruption/concurrency/source-mutation checks | No — Wave 0 | pending |
| AC-05, AC-06 | RULE-02, RULE-03 | `node tools/phase1/check.cjs --group register` | All 85 donor IDs, 84 headings, exact relations/dispositions; invalid/empty/duplicate/missing/reordered cases | No — Wave 0 | pending |
| AC-07, AC-08, AC-09 | RULE-03, RULE-06 | `node tools/phase1/check.cjs --group rules` | Real compliant/violation proofs for every applicable machine rule; actual builds; bounded exemptions; numeric neighbors and coverage thresholds | No — Wave 0 | pending |
| AC-10, AC-11 | RULE-04 | `node tools/phase1/check.cjs --group docs` | Canonical tracked AGENTS.md; exact links/anchors/encoding/scope; invalid fixtures and project docs | No — Wave 0 | pending |
| AC-12, AC-13 | RULE-05 | `node tools/phase1/check.cjs --group graph` | Exact evaluated graph and type boundaries; positive and forbidden direct/transitive/undeclared/missing/empty/reordered cases | No — Wave 0 | pending |
| AC-14, AC-15, AC-16, AC-17 | RULE-06, RULE-07 | `node tools/phase1/check.cjs --group engine` | Engine-created resources/exports/partials/callbacks/signals; two cycles; invalid initialization; no late work; thread guard | No — Wave 0 | pending |
| AC-18, AC-19, AC-20, AC-21, AC-22 | RULE-07 | `node tools/phase1/check.cjs --group state` | Controlled time/randomness; numeric edges; exact IDs/order; strict empty/invalid/version parsing; atomic repeated restore | No — Wave 0 | pending |

## Exact Plan/Task Ownership

| Acceptance criterion | Exact owners (plan:task) | Required command group | Proof obligation |
| --- | --- | --- | --- |
| AC-01 | 01-01:1–2;01-02:2;01-18:2 | `node tools/phase1/check.cjs --group pins` | Exact pins, missing/conflict/mismatch, equivalent permutations, evaluated central pins and five generated locks |
| AC-02 | 01-16:1;01-17:1–2 | `node tools/phase1/check.cjs --group platform` | Actual clean donor-independent same-source Windows and Linux full suites |
| AC-03 | 01-02:1–2 | `node tools/phase1/check.cjs --group runner` | Real failure, missing tool/discovery/filter/skip/crash/timeout/log/result/stale/duplicate |
| AC-04 | 01-02:2;01-17:2 | `node tools/phase1/check.cjs --group runner` | Repeat/interruption/concurrency/source-document mutation |
| AC-05 | 01-03:1–2;01-14:2 | `node tools/phase1/check.cjs --group register` | 85IDs/84headings/24exemptions, exact disposition and identity mutations |
| AC-06 | 01-03:2;01-14:2 | `node tools/phase1/check.cjs --group register` | Bidirectional mechanism/proof/reference integrity and permutation |
| AC-07 | 01-18:1–2;01-05–01-09;01-14:2 | `node tools/phase1/check.cjs --group pins`; `node tools/phase1/check.cjs --group rules` | Exact compiler verifier/configuration controls, every applicable machine mechanism clean/negative plus actual-build categories |
| AC-08 | 01-08:2;01-12:3 | `node tools/phase1/check.cjs --group rules` | Exact exemptions, overlap/suppression/fake-generation/adjacent boundaries |
| AC-09 | 01-06;01-07:1;01-08:2;01-15 | `node tools/phase1/check.cjs --group rules` | Exact numeric units and below/equal/above; real coverage90/80 |
| AC-10 | 01-13:1;01-14:1 | `node tools/phase1/check.cjs --group docs` | Canonical AGENTS, focused indexes and allfive project docs |
| AC-11 | 01-13:1–2 | `node tools/phase1/check.cjs --group docs` | Portable exact paths/anchors/case/space/Unicode/encoding/scope |
| AC-12 | 01-04:1–2;01-18:1;01-09:1 | `node tools/phase1/bootstrap.cjs --probe boundaries`; `node tools/phase1/check.cjs --group pins`; `node tools/phase1/check.cjs --group graph` | Exact expected evaluated graph, owned declarations, allowed real Godot/Core/service boundary use |
| AC-13 | 01-09:1–2 | `node tools/phase1/check.cjs --group graph` | Direct/imported/transitive/type/generated/undeclared/missing/empty/permutation |
| AC-14 | 01-01:1;01-12:1–3 | `node tools/phase1/check.cjs --group engine` | Actual engine/resource/callback/export/partial/signal/nullability surface assertions |
| AC-15 | 01-12:1 | `node tools/phase1/check.cjs --group engine` | Two distinct cycles, callback order, exactly-once handlers, latework rejection |
| AC-16 | 01-12:2 | `node tools/phase1/check.cjs --group engine` | Required missing/invalid exports fail before use, optionalnull passes |
| AC-17 | 01-09;01-10;01-12:1,3 | `node tools/phase1/check.cjs --group engine` | Main-thread guard and semantic bypass checks; pure worker/Core |
| AC-18 | 01-10:1–2;01-17 | `node tools/phase1/check.cjs --group state` | Controlled repeatability and saved random continuation common vectors |
| AC-19 | 01-10:1–2;01-11:1 | `node tools/phase1/check.cjs --group state` | Integer units/range/precision/zero/overflow/invalid neighbors |
| AC-20 | 01-10;01-11:1 | `node tools/phase1/check.cjs --group state` | Exact ordinal identities/values, meaningful ordering, unordered semantics |
| AC-21 | 01-11:1–2 | `node tools/phase1/check.cjs --group state` | Empty/single/many; absent/null/duplicate/malformed/version rejection |
| AC-22 | 01-11:2 | `node tools/phase1/check.cjs --group state` | Idempotent restore, interruption, competing writers, atomic reader visibility |

Platform orchestration is deliberately outside `check.cjs --all` to prevent recursion: `node tools/phase1/platform.cjs --windows-clean` (01-16:1), `--prepare-linux` (01-16:2, preparation only), actual Linux-native `--linux-clean` (01-17:1), and `--reconcile` (01-17:2). Every child is bounded and every command needs exact fresh required IDs, not only exit0. Check group pins/runner is ready in01-02; capture/schema checks in01-03; project/shared configuration declarations in01-04; permitted consumer references, exact compiler verifier/configuration controls and all five generated locks in01-18; each analyzer family in01-05–07; real builds/exemptions01-08; graph01-09; state01-10/11; actual engine01-12; docs01-13/14; coverage01-15. Until later required groups exist, `--all` stays non-success rather than omitting them.

Register checks use staged exact obligations: `--case register.capture` and `--case register.schema` in01-03 validate the captured baseline and mapping schema; `--case register.binding-precoverage` in01-14 resolves all actual created mechanisms/proofs except the explicitly owned coverage cases. Full `register.binding` and complete-suite success require the implemented coverage bindings in01-15. Stage-specific feedback does not claim full inheritance enforcement before its mechanisms exist.

Unique tooling-integrity threats are T-01-01 through T-01-36, allocated consecutively two per numeric plan ID;01-18 adds T-01-35/36 and executes in wave5 after01-04. T-01-SC is the reserved repeated supply-chain row. Each task's actual mutation proofs verify its relevant input/evidence boundary. Exact source/edge/rule allocations remain planning commitments, never passed status.

## Wave 0 Requirements

- [ ] Workspace-local writable tool/cache/run directories, exact pinned inputs, dependency locks and actual Godot integration compatibility.
- [ ] Minimal compilable core/service/Godot/enforcement/proof boundaries; independent expected project inventory.
- [ ] Captured donor originals, content/revision identity, heading-to-rule relations and all adopted enforcement inputs.
- [ ] Required-case manifest and fresh-result reconciliation before treating any suite as complete.
- [ ] Actual compiler/analyzer/architecture violations and exact diagnostic checks.
- [ ] Real engine lifecycle fixtures and pure state examples, with managed coverage collection retaining adopted 90% line / 80% branch gates.
- [ ] Genuine Linux execution path, including required SDK/runtime and engine; Windows-only results do not close the phase.

## Security Verification

Plans must allocate unique phase threat IDs and map their tests into these task rows: strict paths and process arguments, bounded parsing, exact pins and provenance, fake generation/suppression rejection, fresh proof identity and atomic publication. ASVS L1 is scoped guidance, not a certification claim. See research security applicability.

## Manual-Only Verifications

| Behavior | Requirement | Why manual | Instructions |
| --- | --- | --- | --- |
| Qualitative rule obligations | RULE-02, RULE-03 | Human judgment cannot honestly be reported as automated enforcement | Keep a short checklist with exact rule IDs and record review separately. |
| Linux environment provisioning if no usable runtime is available | RULE-01, RULE-06 | Environment access may require the creator | Prepare reproducible commands and source identity; request only concrete missing environment access. Actual Linux checks must still execute. |

## Validation Sign-Off

- [ ] Every task has an automated check or an explicit Wave 0 dependency.
- [ ] No three consecutive tasks lack automated verification.
- [ ] Wave 0 covers every missing reference.
- [ ] No watch-mode checks or unbounded child processes.
- [ ] Fresh complete Windows and Linux evidence exists for the same source identity.
- [ ] `nyquist_compliant: true` only after actual validation.

Approval: pending implementation and executed validation.
