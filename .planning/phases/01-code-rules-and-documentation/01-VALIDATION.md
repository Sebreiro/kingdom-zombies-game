---
phase: "01"
slug: "code-rules-and-documentation"
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-13"
updated: "2026-11-14"
---

# Phase 01 — Validation Strategy

The creator approved simplified Phase 1 completion on 2026-11-14. The revised [specification](01-SPEC.md) and [context](01-CONTEXT.md) govern acceptance. This is a validation plan, not newly executed evidence. Completed-plan reports remain historical; source-allocation coverage in [COVERAGE.md](COVERAGE.md) does not mean measured code coverage.

## Preserve important rules

Keep the pinned toolchain/locks, all twelve analyzers and exact numeric code-rule limits, five-project dependency checks, all 85 rule dispositions, narrow generated/source permissions and every implemented runner/docs/state/native-lifecycle case. Keep intended positive/negative diagnostics, controlled inputs, stable identities, serializable independent state, atomic restore and engine main-thread/lifetime behavior. No working test is removed, skipped or weakened.

## Acceptance timing and commands

| Check | Phase 1 completion | Phase 5 certification |
| --- | --- | --- |
| Windows | Clean donor-independent locked restore, strict solution build, real native bootstrap and one final complete Phase 1 suite | Full certification including coverage and aggregate reports |
| Linux | Actual clean restore/build plus predeclared representative analyzer/graph/docs/state/native-engine checks | Exhaustive complete suite, every failure variant, coverage and cross-platform reconciliation |
| Coverage | Preserve the documented 90% line / 80% branch policy and pending ownership | Actual analyzer and native-hosted production collection, thresholds and report controls |
| Runner | Preserve all implemented failure/freshness/interruption/concurrency cases and truthful selected-set completeness | Remaining aggregate certification and additional exhaustive platform/runner scenarios |
| Human judgment | Short separate review of exact qualitative rule IDs at phase end | Later changes receive their own relevant review |

Existing focused commands: `node tools/phase1/check.cjs --group docs` (or `runner`, `register`, `rules`, `graph`, `state`, `engine`, `pins`) and exact `--case` IDs from the manifest. `node tools/phase1/bootstrap.cjs --probe engine` proves actual native compatibility. The strict whole-solution build and clean restore prerequisites remain in [setup](../../../docs/workflow/setup.md) and [verification](../../../docs/workflow/verification.md).

**Planned completion command:** `node tools/phase1/check.cjs --phase 1`, implemented by 01-15 before use. It selects independently declared Phase 1 obligations and reports explicit deferred Phase 5 IDs separately. It must never filter simply by implementation status or suppress a missing/failed Phase 1 result. Until 01-15 is implemented, this is not an available command. Existing `--all` retains its exhaustive meaning and remains non-success while declared Phase 5 certification is pending.

## Remaining plan ownership

| Plan | Required work | Verification |
| --- | --- | --- |
| 01-14 | Finish only unresolved project/rule mappings and review preparation; reuse already created pages and runner-edge cases | Existing docs.projects, register.binding-precoverage and focused docs/register checks |
| 01-15 | Explicit acceptance-phase metadata, minimal selector, visible Phase 5 ownership and current binding guidance | Focused selection/failure tests; existing register/docs checks |
| 01-16 | One final clean complete Windows Phase 1 run; prepare actual Linux baseline using existing tools/VM | Locked restore, strict solution build, native bootstrap, implemented --phase 1 |
| 01-17 | Execute predeclared representative clean Linux checks, consolidate qualitative review and record truthful closure | Existing exact selectors/native assertions, actual platform logs and separate human review |

Do not repeat a full suite solely to close each intermediate plan or edit planning metadata. Use focused checks while batching related fixes, then the final Windows run after implementation stabilizes. A later implementation/contract change invalidating that evidence requires appropriate renewed verification. Keep tested inputs fixed during each run and retain original reports. Planning/prose-only changes receive docs and consistency checks.

## AC-01 through AC-22

| Criteria | Phase 1 evidence |
| --- | --- |
| AC-01 | Existing exact pin/compiler/configuration/lock checks and clean builds |
| AC-02 | Complete clean functional Windows run and actual representative clean Linux baseline; exhaustive ac-02.complete moves to Phase 5 |
| AC-03–04 | Existing runner failure, fresh-result, repeat/interruption/concurrency/input-integrity checks; no new certification matrix |
| AC-05–06 | Captured 85 IDs / 84 heading relations / 24 source exemption dispositions; real current precoverage binding checks and accurate later ownership |
| AC-07–09 | All implemented analyzer/compiler/build/exemption/numeric tests stay required; the coverage-dependent aggregate cases and measured 90%/80% are Phase 5 |
| AC-10–11 | Canonical guide/indexes, five short project references and existing portable documentation checks |
| AC-12–13 | Existing declared/evaluated/compiled graph and intended violation tests; representative allowed/forbidden cases execute on Linux |
| AC-14–17 | Existing actual engine fixture/assertions for construction/resources/partials/exports/callbacks/signals/teardown and guarded thread/lifetime behavior on both OS baselines |
| AC-18–22 | Existing controlled time/randomness, unchanged common replay vectors, identity/snapshot/restore tests; representative input/round-trip cases execute on Linux |

All already implemented edge cases remain required regressions. The original eight pending aggregate/coverage identities and their full acceptance meaning are retained in the [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). They receive no Phase 1 pass, skip or waiver; 01-15 migrates their executable selection/binding metadata. Phase 1's revised criteria use the existing substantive functional proofs instead of requiring new aggregate wrappers.

## Required separate review

Use the existing [qualitative checklist](../../../docs/code/review.md) and [human-check record](01-HUMAN-CHECK.md). Record actual reviewer/date/outcome for the listed rule components; automated or AI results never fill human fields. Missing concrete Linux access is an explicit baseline blocker. Prepare all independent setup first, then request only the missing access if needed. A VM boot, instructions or Windows cross-build never supplies Linux acceptance.

## Sign-off

- [ ] Important operational rules and every working test are preserved.
- [ ] Explicit Phase 1 selection rejects missing/failed required obligations; Phase 5 work is visible and pending.
- [ ] Clean complete Windows Phase 1 results are current and passing.
- [ ] Actual representative clean Linux checks are current and passing for the intended final implementation.
- [ ] Short project/rule references are accurate and the separate qualitative review is complete.
- [ ] Coverage/exhaustive certification remain required in Phase 5 without false pass credit.
- [ ] Only after these checks may Phase 1 and its RULE requirements be marked complete; this planning update leaves compliance/sign-off flags false.
