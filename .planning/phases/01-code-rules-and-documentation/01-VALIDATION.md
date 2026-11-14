---
phase: "01"
slug: "code-rules-and-documentation"
status: validated
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-13"
updated: "2026-11-14"
---

# Phase 01 — Validation Strategy

**Latest acceptance policy, 2026-11-14:** The creator now requires complete current phase scope on the development OS and a local VM running the other OS. Four review repairs and six new regressions require fresh full Windows/Linux-VM evidence for 676 selected obligations. Historical 670-case Windows and sixteen-case Linux records below retain their actual outcomes; they do not certify the repaired implementation. Independent repair/security review and actual human judgment remain Pending.

## Validation audit — 2026-11-14

The required post-execution audit is **partial**: it identified no new missing-test behavior, but escalated nine existing automated Linux execution gaps with zero resolved. The fourteen qualitative human components remain separately Pending. `status: validated` records that this audit occurred; `nyquist_compliant: false` and the incomplete phase status remain unchanged.

The independent audit reconciled all 670 unique Windows selected/accepted cases, all 675 zero command exits and all 1,356 recorded subject/report artifact hashes. The fourteen artifacts belonging to the seven accepted Linux cases also match. It confirmed the original Linux archive SHA-256 and retained the nine actual CHILD_TIMEOUT results without acceptance credit. Existing tests cover each RULE-01–RULE-07 behavior; no duplicate test or additional full run was needed. Phase 5 declarations retain their explicit pending ownership.

The nine escalated machine-check gaps remain the exact failed selectors in the platform record. They require successful actual execution; they are not reclassified as manual-only checks. The human-review record remains the sole owner of human reviewer/date/outcome fields. See the [partial execution summary](01-17-SUMMARY.md) and [platform record](../../../docs/workflow/platform-proof.md).

The creator approved simplified Phase 1 completion on 2026-11-14. The revised [specification](01-SPEC.md) and [context](01-CONTEXT.md) govern acceptance. This record combines the validation strategy with actual 01-17 partial outcomes below. Phase 1 remains incomplete: nine required representative Linux cases timed out, and human review is Pending. Completed-plan reports remain historical; source-allocation coverage in [COVERAGE.md](COVERAGE.md) does not mean measured code coverage.

## Preserve important rules

Keep the pinned toolchain/locks, all twelve analyzers and exact numeric code-rule limits, five-project dependency checks, all 85 rule dispositions, narrow generated/source permissions and every implemented runner/docs/state/native-lifecycle case. Keep intended positive/negative diagnostics, controlled inputs, stable identities, serializable independent state, atomic restore and engine main-thread/lifetime behavior. No working test is removed, skipped or weakened.

## Acceptance timing and commands

| Check | Phase 1 completion | Phase 5 certification |
| --- | --- | --- |
| Windows | Clean donor-independent locked restore, strict solution build, real native bootstrap and one final complete Phase 1 suite | Full certification including coverage and aggregate reports |
| Linux | Clean local VM restore/build/native bootstrap and the complete current Phase 1 selection, including implemented failure variants | Complete Phase 5 scope including later coverage/aggregate cases and advanced reconciliation |
| Coverage | Preserve the documented 90% line / 80% branch policy and pending ownership | Actual analyzer and native-hosted production collection, thresholds and report controls |
| Runner | Preserve all implemented failure/freshness/interruption/concurrency cases and truthful selected-set completeness | Remaining aggregate certification and additional exhaustive platform/runner scenarios |
| Human judgment | Short separate review of exact qualitative rule IDs at phase end | Later changes receive their own relevant review |

Existing focused commands: `node tools/phase1/check.cjs --group docs` (or `runner`, `register`, `rules`, `graph`, `state`, `engine`, `pins`) and exact `--case` IDs from the manifest. `node tools/phase1/bootstrap.cjs --probe engine` proves actual native compatibility. The strict whole-solution build and clean restore prerequisites remain in [setup](../../../docs/workflow/setup.md) and [verification](../../../docs/workflow/verification.md).

**Implemented completion command:** `node tools/phase1/check.cjs --phase 1` selects independently declared Phase 1 obligations and reports the eight explicit deferred Phase 5 IDs separately. It never filters simply by implementation status or suppresses a missing/failed Phase 1 result. Plan 01-16's frozen Windows invocation accepted all 670 selected obligations. Existing `--all` retains its exhaustive meaning and remains non-success while declared Phase 5 certification is pending.

## Remaining plan ownership

| Plan | Required work | Verification |
| --- | --- | --- |
| 01-14 | Finish only unresolved project/rule mappings and review preparation; reuse already created pages and runner-edge cases | Existing docs.projects, register.binding-precoverage and focused docs/register checks |
| 01-15 | Explicit acceptance-phase metadata, minimal selector, visible Phase 5 ownership and current binding guidance | Focused selection/failure tests; existing register/docs checks |
| 01-16 | One final clean complete Windows Phase 1 run; prepare actual Linux baseline using existing tools/VM | Locked restore, strict solution build, native bootstrap, implemented --phase 1 |
| 01-17 | Repair confirmed findings, independently review them, execute complete final Windows/Linux-VM phase scope and obtain actual qualitative review | Clean setup, existing complete --phase 1 selection, original platform reports and separate human judgment |

Do not repeat a full suite solely to close each intermediate plan or edit planning metadata. Use focused checks while batching related fixes, then the final Windows run after implementation stabilizes. A later implementation/contract change invalidating that evidence requires appropriate renewed verification. Keep tested inputs fixed during each run and retain original reports. Planning/prose-only changes receive docs and consistency checks.

## AC-01 through AC-22

The frozen Windows report `030dd8f9-c5e7-4d52-9b03-505948e8755c` accepts 670/670 on `f4658bb`. The [platform record](../../../docs/workflow/platform-proof.md) gives the sixteen exact Linux nonces, unchanged common input, real tool/platform identities and retained logs: seven accepted, nine CHILD_TIMEOUT, zero skipped/unrun. The table records those outcomes; it does not mark all ACs or RULE requirements complete.

| Criterion | Actual functional evidence and remaining condition |
| --- | --- |
| AC-01 | Windows pins 27/27 and strict clean build accepted. Genuine Linux observes the same pinned tools, locked five-project restore, zero-warning/error strict build and native bootstrap. |
| AC-02 | **Blocked.** Complete clean Windows 670/670 passed; Linux clean setup passed but nine of sixteen required representative selectors timed out under unchanged limits. `ac-02.complete` separately remains pending Phase 5. |
| AC-03 | Windows runner 36/36 and implemented pin/failure controls accepted. Actual Linux timeouts produce non-success and zero accepted results for each affected selector; a longer diagnostic supplies no acceptance credit. |
| AC-04 | Existing Windows repetition/interruption/concurrency/input controls accepted and preserved. Linux case inputs stayed frozen; the outer sequence paused only between terminal cases, with the actual child exit retained. No mixed result was credited. |
| AC-05 | Current focused register controls accept all 85 rule IDs, 84 unique headings and 24 source exemption dispositions, including missing/duplicate/reordered/empty/encoding rejection. |
| AC-06 | 77 implemented bindings accepted by `register.binding-precoverage`; six individual inapplicable resolutions accepted. R-49/R-82 and `ac-06.complete` remain pending Phase 5. Qualitative mapping/adaptation judgment remains **Pending**. |
| AC-07 | All existing Windows functional compliant/violating checks accepted, including 447 rules-group controls and all twelve SCR Error analyzers. Required Linux R-38 pair timed out, so the representative baseline is **blocked**. Coverage-dependent `ac-07.complete` remains pending Phase 5. |
| AC-08 | Windows exact suppression/generated-origin/adjacency/collision controls accepted; focused register overlap/broadening controls accepted. Linux strict build retains every permission, but generated-origin/handwritten-partial selectors timed out; no Linux canonical pass is claimed. |
| AC-09 | Windows exact rule units/inclusive limits/nearest-neighbor/non-ASCII/overflow/order controls accepted; Linux `state.numeric-and-random` accepted. Inclusive 90%/80% policy remains unchanged; measurement and `ac-09.complete` remain pending Phase 5. |
| AC-10 | Frozen Windows docs 36/36 and five project-reference pages accepted. Material review confirms the five declarations and focused indexes. Linux `docs.projects` timed out; human usefulness/accuracy review remains **Pending**. |
| AC-11 | Linux `docs.portable` accepted with actual portable-contract/missing-entry evidence; Windows docs include the full existing casing/anchor/Unicode/ordering failure controls. Later prose receives its own focused D-11 report. |
| AC-12 | Windows graph 26/26 accepted. Linux strict five-project build passed, but required evaluated/compiled `ac-12.complete` timed out: representative graph acceptance **blocked**. |
| AC-13 | Windows direct/transitive/type/missing/empty/duplicate/order graph controls accepted. Required Linux `graph.allowed-and-direct` timed out and receives no intended-violation credit. |
| AC-14 | Windows engine 19/19 accepted. Linux raw native bootstrap actually published all 21 fresh surface/cycle assertions, but the lifecycle/export/generated/partial canonical selectors timed out; Linux representative acceptance **blocked**. |
| AC-15 | Raw Linux native output observes two distinct instances, exact callbacks/signals/teardown and old-generation fences. Windows full lifecycle controls accepted; timed-out Linux `engine.lifecycle` is not promoted by raw output. |
| AC-16 | Windows actual required-reference/value/invalid-null failures-before-use and valid optional-null controls accepted. Linux valid raw scene passed; canonical `engine.exports` timed out, leaving its required invalid-initialization acceptance unresolved. |
| AC-17 | Windows guarded/worker/continuation/lifetime controls accepted. Actual Linux raw output observes worker rejection and one captured main thread. Canonical lifecycle timeout remains blocking; raw output is identified separately. |
| AC-18 | Linux `state.inputs`, unchanged `state.golden-vectors` and `state.numeric-and-random` each accepted fresh 1/1, alongside all 33 Windows state controls. No vectors or random contract were changed. |
| AC-19 | Existing Windows numeric contract edge/overflow/zero-time/precision controls accepted. Linux representative controlled-input and random-continuation cases accepted; no exhaustive Linux edge matrix is claimed. |
| AC-20 | Linux `state.identities` and `state.snapshot` accepted, preserving ordinal Unicode identities, independent values, meaningful order and detached round trips. Windows retains all existing order/representation failure controls. |
| AC-21 | Windows empty/singleton/multiple/invalid-schema/duplicate/incomplete state controls accepted. Linux `state.snapshot` and `state.restore` accepted; invalid replacements remain covered by preserved Windows cases. |
| AC-22 | Linux complete-root `state.restore` accepted. Windows repeated/concurrent/cancelled/invalid restore controls accepted, including atomic publication without partial live-state replacement. |

All already implemented edge cases remain required regressions. The original eight pending aggregate/coverage identities and their full acceptance meaning are retained in the [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). They receive no Phase 1 pass, skip or waiver; 01-15 migrates their executable selection/binding metadata. Phase 1's revised criteria use the existing substantive functional proofs instead of requiring new aggregate wrappers.

## Required separate review

Use the existing [qualitative checklist](../../../docs/code/review.md) and [human-check record](01-HUMAN-CHECK.md). Record actual reviewer/date/outcome for the listed rule components; automated or AI results never fill human fields. Missing concrete Linux access is an explicit baseline blocker. Prepare all independent setup first, then request only the missing access if needed. A VM boot, instructions or Windows cross-build never supplies Linux acceptance.

## Material review executed by 01-17

On 2026-11-14, AI inspection reviewed the five existing module pages against `tools/phase1/projects.json`, all 85 rule dispositions, current mechanism/proof bindings and the narrow exemption registries. There are 48 retained, 31 adapted and six inapplicable rules; 77 implemented bindings and two pending coverage bindings (R-49/R-82, Phase 5). The 84 unique source headings and 24 donor exemption dispositions remain accounted for. The six inapplicable IDs are R-43, R-57, R-76, R-79, R-80 and R-85, each with its own subject-specific reason and resolution case. This inspection is separate from required human judgment.

The actual focused register invocation `cae2f0ca-f2bf-490f-897b-7ca40513a421` on `21597fdeaed675c89bab5e0dd7aabf29c0266597` accepted all 46 implemented controls, including `register.binding-precoverage`, capture, missing/duplicate/reordered relations, exact links/symbols, severities, numeric semantics and exemption overlap/broadening rejection. Actual overall exit **1** and status **fail** are retained: `--group register` also selects the single pending Phase 5 `ac-06.complete`, reported **blocked / IMPLEMENTATION_PENDING**. There were zero failed implemented controls, and the pending aggregate received no pass or skip credit. Canonical evidence is `.phase1/runs/cae2f0ca-f2bf-490f-897b-7ca40513a421/summary.json`; input `08e5f9859a85da7075daa9d20760cac21076be468262b435c8ee8cdb2b593925`, tool `8221b70af7d3679f06ad0714ac6cbcba02ca12b9f357dadb4c0ffc031371216c`, platform `win32-x64-10.0.19044`.

No implementation/rule/test/configuration diff exists between that reviewed source and the frozen Windows/Linux implementation `f4658bb`; subsequent Plan 17 edits are proof/review prose only. All five projects, twelve Error analyzers, exact numeric limits, five LF locks, authentic xUnit logical seed, 170 authenticated native span permissions and eighteen precise member suppressions remain unchanged. Historical WINDOWS item 1 stays open; resolved items 25–27 retain their actual fixes. The [platform record](../../../docs/workflow/platform-proof.md) retains actual setup/case outcomes, including failed attempts. The fourteen qualitative components remain Pending in the existing human-check record.

## Sign-off

- [x] Important operational rules and every working test are preserved.
- [x] Explicit Phase 1 selection rejects missing/failed required obligations; Phase 5 work is visible and pending.
- [x] Clean complete Windows Phase 1 results are current and passing for frozen implementation f4658bb.
- [ ] Complete current Windows and local Linux-VM Phase 1 runs pass for the repaired implementation. Earlier seven accepted/nine timed-out Linux cases remain historical; hardware capacity and fresh complete evidence are still required.
- [ ] Short project/rule references are accurate and the separate qualitative review is complete.
- [x] Coverage/exhaustive certification remain required in Phase 5 without false pass credit.
- [ ] Only after these checks may Phase 1 and its RULE requirements be marked complete; this planning update leaves compliance/sign-off flags false.

## Validation Audit 2026-11-14

| Metric | Count |
|---|---|
| Gaps found | 9 |
| Resolved | 0 |
| Escalated | 9 |
