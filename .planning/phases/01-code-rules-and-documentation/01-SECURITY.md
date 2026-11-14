---
phase: "01"
slug: "code-rules-and-documentation"
status: verified
threats_open: 0
asvs_level: 1
block_on: high
register_authored_at_plan_time: true
threats_total: 31
threats_closed: 31
created: "2026-11-14"
---

# Phase 01 — Security

The required mitigation audit verified all **31 unique plan-time threats** at ASVS level 1. The 18 plans contain 45 rows; repeated T-01-SC entries agree and count once. Every unique threat is high severity with disposition mitigate. There are no open threats, accepted risks or transferred threats in this register. This verdict concerns the declared mitigations; it does not complete Phase 1 or resolve the Linux execution and human-review gaps.

## Trust boundaries

| Boundary | Data crossing | Implemented control |
| --- | --- | --- |
| Declared inputs to owned tooling | Pins, source, project policy, register, documentation and fixture paths | Bounded strict parsing, canonical paths, independent inventories and exact identities |
| Child processes to accepted evidence | Actual command exits, reports, intended diagnostics and artifacts | Fresh invocation identities, exact discovered test, complete result reconciliation and hashed artifacts |
| Acquired dependencies to build | Official archives, NuGet packages, SDK/analyzers and locks | Pinned sources and hashes, inspected provenance and locked restore |
| Candidate state to live state | Text/snapshot values, identifiers, ordering and restore result | Strict bounded validation before immutable-root replacement |
| Worker activity to native engine state | Queued work, cancellation, lifecycle generations and scene callbacks | Captured-thread checks and live-generation fences before engine work |

## Evidence basis

The audit inspected the existing frozen Windows report at `.phase1/temp/16-windows-f4658bb/.phase1/runs/030dd8f9-c5e7-4d52-9b03-505948e8755c/summary.json`, source `f4658bb2872b6578b592a18475e58af04cd064d4`, input `23d1060bfbdb6b082624631c1e32f1cd31a9fe29287745072e364d9f56020071`, tool `f3f2c460d76528ac47f5c5e257f549b5585e76792ab7913d4b0168be4ffe661b`, platform `win32-x64-10.0.19044`.

All 670 unique selected/accepted cases, all 675 zero command exits, all 670 invocation-bound proof/test identities and all 1,356 referenced subject/report hashes reconcile. No skipped tests, failed tests, wrong identities or artifact mismatches were found. At audit time the 200 fingerprinted inputs differ only in `.planning/ROADMAP.md`, `.planning/STATE.md` and `docs/workflow/platform-proof.md`; implementation, tests, enforcement, exemptions, register and provenance inputs are unchanged. The audit ran no new suite and changed no implementation.

**Shared boundary B1:** `tools/phase1/check.cjs` validates independent declarations/selectors before execution; `reconcile` rejects unknown, missing, duplicate, non-pass, stale or wrong nonce/input/tool/platform/diagnostic/prerequisite/evidence results. Evidence paths are contained and nonempty. Actual locked restore/build and exact test discovery precede execution; actual test reports and `ProofProtocol` records must agree. Inputs are recomputed before atomic publication, with exclusive ownership and interruption handling. Existing runner controls exercise missing/deleted/stale/duplicate reports, wrong identities/filters, skipped tests, crash/timeout, missing executables, source/document changes, concurrency, interruption and assertion failures. Their intended rejection diagnostics passed in the frozen report.

## Threat register

Every row is closed at the configured L1 depth. File names and case IDs below identify the actual code and executed evidence, rather than only a planned mitigation.

| Threat | Category | Component | Severity | Disposition | Actual mitigation and proof | Status |
| --- | --- | --- | --- | --- | --- | --- |
| T-01-01 | Tampering | tools/phase1/bootstrap.cjs | high | mitigate | Bounded duplicate-aware manifest parsing, exact declaration reconciliation, official archive/executable and package hashes, observed tool identities; missing/duplicate/conflicting/generator/compiler/engine pin mutations | closed |
| T-01-02 | Spoofing | Case/evidence boundary | high | mitigate | B1; bootstrap separately checks fresh exact native assertion order and nonce; stale/missing/duplicate/identity/prerequisite controls | closed |
| T-01-03 | Tampering | tools/phase1/check.cjs | high | mitigate | Fatal UTF-8, bounded strict JSON, contained paths/symlinks, independent selectors and final input comparison; selection/declaration/required-case and real source/document mutations | closed |
| T-01-04 | Spoofing | Case/evidence boundary | high | mitigate | B1; actual process stale/deleted/duplicate/nonce/crash/timeout/skip/filter controls distinguish intended subject failures from infrastructure errors | closed |
| T-01-05 | Tampering | Donor provenance archive | high | mitigate | RegisterValidator checks pinned ZIP hash before bounded parsing, canonical unique paths and full inventory/revision/headings/rules/exemptions; capture/missing/duplicate/schema/encoding controls | closed |
| T-01-06 | Spoofing | Case/evidence boundary | high | mitigate | B1; distinct exact register diagnostics and fresh subject/test identities for provenance mutations | closed |
| T-01-07 | Tampering | KingdomZombies.slnx | high | mitigate | ProjectEvaluator checks independent exact policy, five solution subjects and evaluated references; allowed/direct/undeclared/duplicate/transitive/type-leak controls | closed |
| T-01-08 | Spoofing | Case/evidence boundary | high | mitigate | B1; exact graph/compiler diagnostics distinguish forbidden dependencies from graph infrastructure errors | closed |
| T-01-09 | Tampering | NamedArgumentAnalyzer | high | mitigate | Actual argument-operation analysis including generated-marked code, SCR0001 and exact source/span origin exceptions; R-38 pair, call-site/generated/adjacent/origin controls | closed |
| T-01-10 | Spoofing | Case/evidence boundary | high | mitigate | B1 and exact Roslyn verifier; R-38 negative matches Error SCR0001 at Subject.cs(4,25)-(4,26), excluding unrelated failures | closed |
| T-01-11 | Tampering | NullForgivingAnalyzer | high | mitigate | Actual suppression-expression/token analysis including generated-marked code, Error SCR0005; R-26 pair, member/null-literal/generated controls | closed |
| T-01-12 | Spoofing | Case/evidence boundary | high | mitigate | B1 and exact diagnostic sets; R-26 negative matches Error SCR0005 at Subject.cs(6,21)-(6,22) | closed |
| T-01-13 | Tampering | ContinuationIndentAnalyzer | high | mitigate | Actual tree/indent computation, SCR0009 and exact generated-span permission; wrapped expressions, chains, nearest-neighbor/generated/adjacent controls | closed |
| T-01-14 | Spoofing | Case/evidence boundary | high | mitigate | B1 and complete expected Roslyn spans; distinct compliant/violating continuation records reconcile | closed |
| T-01-15 | Tampering | EnforcementTests | high | mitigate | BuildFixture requires locked clean restore/build and exact nonzero subject/error/location, excludes NU/MSB/analyzer-loading failures; real consumer, suppression-entry, formatter and wiring controls | closed |
| T-01-16 | Spoofing | Case/evidence boundary | high | mitigate | B1 and real-build location matching; build wiring SCR0006 at Subject.cs(6,25), R-01 CS0168 at Subject.cs(8,13) after clean setup | closed |
| T-01-17 | Tampering | tools/phase1/projects.json | high | mitigate | Bounded schema, canonical paths, independent exact five-project/type/edge inventories; malformed/empty/missing/duplicate/reordered/analyzer metadata controls | closed |
| T-01-18 | Spoofing | Case/evidence boundary | high | mitigate | B1; exact graph-policy outcomes, including SUBJECT_REQUIRED for empty rule subject and order-independent forbidden-edge detection | closed |
| T-01-19 | Tampering | ExampleState | high | mitigate | Validated explicit values and copied read-only collections, bounded unique complete identities/order, nonnegative values, checked elapsed addition and exact invariant integers; state/identity/numeric/ownership/random edge controls | closed |
| T-01-20 | Spoofing | Case/evidence boundary | high | mitigate | B1; state.inputs binds CONTROLLED_ADVANCE to ExampleState:Advance, with distinct ownership/numeric/identity records | closed |
| T-01-21 | Tampering | SnapshotCodec | high | mitigate | SnapshotJson bounds bytes/depth/UTF-8/fields/integers/collections/duplicates; complete candidate validation before ExampleStateStore root exchange; snapshot/invalid/interrupted/cancelled/atomic/mutation controls | closed |
| T-01-22 | Spoofing | Case/evidence boundary | high | mitigate | B1; distinct strict round-trip, accepted-root, interrupted/cancelled/invalid-root and mutation evidence | closed |
| T-01-23 | Tampering | MainThreadGateway | high | mitigate | RequireThread precedes work, Drain checks captured thread and live generation, LifetimeToken closes acceptance before cancellation; native thread/late-work/source/gateway-bypass controls | closed |
| T-01-24 | Spoofing | Case/evidence boundary | high | mitigate | B1 plus exact EngineEvidencePolicy surface/cycle/nonce checks; evidence-integrity/lifecycle/export/generated/handwritten controls, raw results kept separate | closed |
| T-01-25 | Tampering | AGENTS.md | high | mitigate | DocumentationValidator requires canonical root, ordinal case/collision checks, strict UTF-8, nonempty indexes and real links from tracked/explicit authored inventory; navigation/Unicode/anchor/case/input-preservation controls | closed |
| T-01-26 | Spoofing | Case/evidence boundary | high | mitigate | B1; docs.navigation matches DOC_CASE AGENTS.md:2 and docs.portable matches the intended missing-index diagnostic | closed |
| T-01-27 | Tampering | Core project reference page | high | mitigate | DocumentationProjects bounds pages, reachable navigation, one strict declaration and exact actual evaluated references; missing/empty/unreachable/extra-edge/analyzer/ambiguous controls | closed |
| T-01-28 | Spoofing | Case/evidence boundary | high | mitigate | B1; exact DOC_PROJECT_EDGES, DOC_PROJECT_DECLARATION and DOC_PROJECT_REQUIRED outcomes for the five project pages | closed |
| T-01-35 | Tampering | Effective consumer configuration | high | mitigate | ConfigurationTests evaluates all five consumers, strict properties/imports/analyzer metadata/exact packages, five locks and drift rejection; pins.evaluated/configuration-drift/verifier and LOCKED_GRAPH_EXACT including NU1004 | closed |
| T-01-36 | Spoofing | AnalyzerVerifier boundary | high | mitigate | Actual compiler/analyzer diagnostics matched by complete ID/severity/file/span sets, valid subject emission; changed identity/severity/path/start/end/extra/missing diagnostic controls plus B1 | closed |
| T-01-SC | Tampering | Declared dependency acquisition | high | mitigate | Official archive SHA-512/executable and canonical managed-package SHA-256, inspected publisher/repository/payload/signature records, exact central pins/source mapping/five locks and locked-drift proof; no npm/pip/cargo installation | closed |

## Provenance qualification

The four Godot archive nupkgs are unsigned: the retained actual signature logs report NU3004. Their controls use official-archive checksums and exact package-payload hashes with inspected publisher/repository metadata. No successful signature or fabricated package-legitimacy verdict is claimed. Retained signed NuGet records distinguish repository signatures from author signatures. Canonical Windows managed package bytes are common to both OSes; differing raw Linux package metadata is never represented as identical canonical bytes.

The actual Godot inspection/signature records are under `.phase1/runs/da785a9f-d9ef-403b-b56e-29b55c15d69e/`; signed NuGet payload/signature records are under `.phase1/temp/01-04-*` and `test-package-signatures.log`. Package and lock drift rejection are part of the frozen execution evidence.

## Accepted risks

No accepted risks. No transfer or waiver was used to close a threat.

## Security audit trail

| Date | Unique threats | Closed | Open | Reviewer |
| --- | --- | --- | --- | --- |
| 2026-11-14 | 31 | 31 | 0 | gsd-security-auditor; orchestrator recorded the structured read-only result |

## Remaining acceptance conditions

The [partial summary](01-17-SUMMARY.md) and [validation record](01-VALIDATION.md) remain authoritative for the nine required Linux CHILD_TIMEOUT results and fourteen Pending human components. The [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) retains all eight deferred declarations without pass/skip credit. Historical WINDOWS item 1 remains open. These conditions are not closed by this security verdict.

All declared threats have a disposition, there are no accepted risks, `threats_open: 0` is verified, and security status is verified on 2026-11-14. Phase 1, Plan 17 and RULE-01–RULE-07 remain incomplete.

## Subsequent code review — mitigation verdict superseded

The later code review demonstrated active-branch source-policy and native-operation false acceptances (CR-01 and CR-02). The initial 31/31 L1 verdict above is historical and no longer the current sign-off. Five related mitigations are reopened pending repair and focused re-verification: T-01-09, T-01-11 and T-01-13 share the conditional suppression guard, T-01-15 owns enforcement/suppression checks, and T-01-23 owns the native-access source boundary. The compiled suppression reproduction directly exercises SCR0001; the other two analyzer entries are reopened conservatively because the same generic guard protects them. No risk is accepted. At that checkpoint the register had 26 closed and five open high threats. The follow-up below supersedes that intermediate status.

## Repair follow-up — current verified verdict

The independent security follow-up at 7dca9c0 re-verified all five reopened mitigations after fixes 330a427, 8ebbc1d, 3da7114 and 5c52a20. Current status is **31 closed / zero open**, ASVS level 1, high block threshold, with no accepted risks or transfers. No new evidence-backed mitigation regression was found.

ConsumerCompilation evaluates the selected consumer through ResolveReferences and reads complete DefineConstants, LangVersion, Compile and ReferencePath inputs. Every actual repository suppression subject has exactly one evaluated owner; parsing uses those options and unresolved attributes fail closed. The generic exact-permission guard applies to suppressed diagnostic IDs, so T-01-09/11/13 and the T-01-15 source boundary are repaired. The actual compiled conditional-suppression regression exercises SCR0001; no new dedicated SCR0005 or SCR0009 suppression execution is claimed. Native invocation/construction/method/property/field operations are resolved and native method captures are rejected irrespective of capture-time guards, repairing T-01-23. Exact existing adapter identities and invocation-time guards remain.

The follow-up independently inspected the six canonical regression reports in the clean [re-review](01-REVIEW.md): six accepted cases, 36 zero command exits, twelve matching report/subject artifact hashes and matching proof identities. They ran at 5bb1a8d, input 14ee4dbb6a303ab968e343bae54378606d2a951aaead8714b5f0ba42fae23359. All 203 current fingerprinted inputs matched those records at audit time. All 678 original declarations remain unchanged; six additions produce 676 Phase 1 obligations and eight separate Phase 5 cases.

This current security verdict supplies no complete platform or human acceptance. Fresh complete Windows/Linux-VM Phase 1 runs and fourteen actual human outcomes remain Pending.

## Security Audit 2026-11-14

| Metric | Count |
|---|---|
| Threats found | 31 |
| Closed | 31 |
| Open | 0 |
