# Phase 5: CI and repeatable game testing — Specification

**Created:** 2026-11-14 (Europe/Paris)
**Ambiguity score:** 0.038 (gate: ≤ 0.20)
**Requirements:** 10 locked — TEST-01 through TEST-10
**Status:** Discussion captured; ready for planning. Execution depends on completion of Phases 1–4.

**Scope amendment, approved by the creator on 2026-11-23:** Phase 5 is reduced to what Phases 6–10 use: one shared runner and catalog, three test tiers, basic Windows/Linux PR checks and the existing solo/LAN rendered scenarios running through the shared runner. Coverage certification with the eight transferred IDs, full hosted Windows/Linux runs, evidence reuse and exhaustive runner certification move to Phase 11 under the new TEST-12; none of it is recorded as passed. Five rules change for Phases 6–10: Critical becomes a budgeted smoke set, Phase 1 rule-suite groups run only when their inputs change, edge cases are unit tests carrying their IDs, no plan reruns the whole selection, and evidence is the tested commit with its run report and artifacts. TEST-01–TEST-10 keep their IDs and their [requirement statements](../../REQUIREMENTS.md); only this specification's expansions change. The [disposition table](#acceptance-disposition) accounts for every criterion, and [05-CONTEXT.md](05-CONTEXT.md) D-35–D-46 record the decisions. This amendment governs where it conflicts with the 2026-11-14 and 2026-11-15 text below.

**Current amendment, 2026-11-15:** The creator's [local testing strategy](../../../docs/workflow/local-testing.md) applies immediately to every phase: complete implementation and code review/fixes before real game windows; development OS only, no VM, 1280×720; grouped scenarios and collected failures; batch repairs, check failed/blocked/newly affected cases, then rerun the entire selected set on the repaired build. Selection always includes every active Critical case plus all new/changed cases and affected regressions, regardless of importance. Every phase saves reusable scenarios with importance and ordered game-run groups. This supersedes older complete-catalog phase-end, local-VM, extra-local-resolution and failed-only carry-forward wording. Phase 11 owns scheduled nightly full tests under TEST-11; TEST-01–TEST-10 and all eight coverage/certification IDs remain here. Hosted Windows/Linux compatibility and separate release evidence remain distinct. AC-35–AC-38 make the new policy testable; original interview/edge scores remain historical.

**Discussion amendment, 2026-11-14:** The creator selected build/repository-only PR checks targeting about ten minutes per OS with cached dependencies, folder/dependency-aware feature selection, and validated reuse of unchanged passing groups at phase completion. These decisions supersede the earlier broad behavior-on-PR and repeat-everything closure policy. Complete required case/environment coverage, actual full-workflow compatibility, immutable evidence, all eight handoff IDs and 90%/80% targets remain binding. See [05-CONTEXT.md](05-CONTEXT.md). This document is amended authority; original interview answers below remain historical.

## Goal

Developers and AI agents can run every delivered check through one shared runner and catalog, in three tiers sized for the edit loop, the plan and the phase end, with basic Windows/Linux pull-request checks and readable failure reports. Later phases add tests and scenario rows, not test infrastructure.

## Background

At the initial specification scout the repository contained the Godot/C# toolchain, twelve inherited analyzers, Core/state/snapshot and native lifecycle fixtures, an independent case manifest and a runner with exact selection, isolated processes and fresh evidence reconciliation. The manifest declared 684 cases: 676 Phase 1 obligations and eight pending Phase 5 identities. The retained complete Windows run accepted 676/676. Current Phase 1 closure and unperformed Linux/human follow-ups are recorded separately in STATE.md and its closure exception; neither that closure nor this amendment supplies Phase 5 certification.

Phases 2–4 have specifications and captured implementation context, but their playable menu/movement, LAN scenario and complete content pack/inspection delivery do not yet exist. No .github workflow, complete game acceptance-case catalog, four-process gameplay runner or accepted measured analyzer/production coverage report exists. Phase 5 extends the working verification foundation once those earlier increments are delivered; it neither assumes they passed nor replaces them with test-only demonstrations.

**Measured basis for the 2026-11-23 amendment:** The Phase 1 rule suite takes 51.6 minutes for 676 cases at one process per case. It supplied 433 of the 481 results in the Phase 3 closing run and 678 of the 776 in Phase 4. Phase 3 marks 31 of 32 cases Critical and Phase 4 marks 62 of 75, so the Critical-plus-affected selection selects almost everything. Phase 4 reran a growing whole selection after each plan, from 10 minutes to 97. The Phase 6–8 specifications declare 464 edge cases. The amendment removes these repeated costs while Phases 6–10 are built; Phase 11 restores full certification on the finished game.

Authority: [project definition](../../PROJECT.md), [approved requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md), [current state](../../STATE.md), [game specification](../../../specs/0.1/initial-spec.md), and the mandatory [Phase 1 certification handoff](05-PHASE1-HANDOFF.md). The [playable foundation](../02-playable-foundation/02-SPEC.md), [LAN foundation](../03-four-player-lan-foundation/03-SPEC.md) and [asset production](../04-asset-production/04-SPEC.md) retain their behavior/content contracts. The current [collaboration policy](../../../docs/workflow/ai-collaboration.md) and [verification reference](../../../docs/workflow/verification.md) govern execution and evidence.

## Requirements

1. **TEST-01 — Traceable complete case catalog:** A repository-stored catalog maps all unchanged source acceptance IDs A-01 through A-47 and foundation, content and regression cases to approved requirements, owning phases, required or planned status, runnable checks or repeatable evidence procedures, environments and required artifacts.
   - Current: tools/phase1/required-cases.json independently declares 684 rule/foundation obligations: 676 Phase 1 cases and eight pending Phase 5 identities. The game specification and REQUIREMENTS.md link A-01–A-47 to requirements, but there is no complete executable/evidence catalog for the game and content foundations.
   - Target: One readable repository catalog lists scenarios: all 47 source scenarios without renumbering, the delivered solo, LAN and inspection scenarios, one entry per existing Phase 1–4 suite group, and later feature and release cases as planned. Each entry names its requirements, owning phase, required or planned status, importance with a one-line reason, tags, tier, run group and order, and the test or procedure that proves it. Unit tests, including edge-case tests, are discovered from the test project and are not catalog rows. The existing Phase 1–4 manifests under src/godot-game/tools/ stay as they are and are referenced as suites.
   - Acceptance: AC-01–AC-03, AC-30, AC-35, AC-38, AC-39, AC-43 and AC-45 pass, including missing, duplicate and invalid importance or group metadata controls.

2. **TEST-02 — Local and CI execution with explicit scope:** One documented entry point runs every tier locally, and Windows/Linux pull requests run the basic checks through the same entry point. Ordinary PRs target about ten minutes per OS with dependencies cached.
   - Current: The Phase 1 runner already supports exact case/group/phase selection and isolated evidence. No .github workflow exists at scout time. The recorded Windows 676-case result is Phase 1 evidence; Linux completion is still pending. No gameplay CI execution has occurred.
   - Target: PRs run the strict build with analyzers, project/code boundary checks, documentation/catalog validation, the pure tests and one headless game scenario, with no game window. The local phase-end selection is the Critical smoke set plus all new/changed cases and affected regressions; Phase 1 rule-suite groups join it only when their declared inputs changed. Complete implementation and review/fixes before grouped 1280×720 development-OS game tests, without a VM. A manual full local run executes every active case. Full hosted Windows/Linux runs of rendered, LAN and coverage checks belong to Phase 11.
   - Acceptance: AC-03, AC-04, AC-06, AC-36, AC-37, AC-39–AC-42 and AC-44 pass. PR and phase-end results identify the selection, explicit omissions and measured timing; a reduced pass never claims complete-catalog certification.

3. **TEST-03 — Agent-operated diagnosis and recovery:** An AI agent can follow a repository-documented select, run, inspect, reproduce, fix and rerun procedure using recorded scenario inputs and current-run evidence, while preserving failed attempts and requiring diagnosis plus a clean rerun before acceptance.
   - Current: docs/workflow/verification.md describes Phase 1 selection, immutable reports and failure handling. It does not yet provide the complete game/menu/LAN/content select-to-reproduction workflow.
   - Target: The repository procedure covers select, run, inspect, reproduce, fix and rerun under the whole-phase/review/grouped-run workflow. Every failed case reports expected and observed behavior, its evidence links and one replay command. Failed runs are kept; a later pass never overwrites them. Failures met during Phase 5's own runs follow the procedure. The staged failure exercise belongs to Phase 11.
   - Acceptance: AC-08 and AC-37 pass, and each failed case in a Phase 5 run has a failure brief and replay command.

4. **TEST-04 — Controlled checks of real behavior:** Gameplay-rule checks execute the delivered production behavior with controlled setup, explicit time and random inputs where applicable, and repeatable assertions of authoritative state.
   - Current: Core example state, snapshot, elapsed-input and replay-random implementations and extensive Phase 1 proofs exist. The menu/movement, LAN and content production described by Phases 2–4 are not implemented at this scout.
   - Target: Automated fixtures exercise the delivered production rules, with controlled initial state and explicit time/randomness where applicable. Scenario setup is separated from production actions and read-only observations. Inputs and expected logical outcomes are recorded well enough to replay; no future combat, inventory or campaign behavior is invented for this phase.
   - Acceptance: AC-09, AC-10 and AC-13 pass, including relevant empty/single/multiple fixtures and controlled-input boundaries.

5. **TEST-05 — Rendered production-input and visual evidence:** Rendered menu, keyboard, mouse, focus and scene checks exercise production input paths, using only 1280×720 for routine local verification, automate objective assertions and provide current-run images for AI appearance and readability review without requiring approved image baselines.
   - Current: Native Godot construction/lifetime proof scenes exist. There is no playable menu/input scene, content inspection pack, game visual runner or approved screenshot baseline in the current implementation.
   - Target: The delivered Phase 2 solo rendered scenarios run through the shared runner at 1280×720 with the real menu, keyboard/mouse, focus, movement and scene transitions, objective assertions and current-run images. One inspection-hub smoke scenario visits each category, captures one image per category and records the AI's appearance/readability notes and the four Phase 4 judgments P1–P4. Approved image baselines and automatic image comparison are not required. The other 17 Phase 4 rendered cases belong to Phase 11.
   - Acceptance: AC-11–AC-13 and AC-37 pass; headless results do not establish a rendered or visual-review pass. Supported-display acceptance retains separate hosted/release procedures.

6. **TEST-06 — Four real isolated LAN processes:** One real host and three real guest game processes complete the delivered LAN foundation scenarios with independent profiles, production commands, bounded waits, per-peer results and logs, and reliable cleanup of owned processes.
   - Current: The project has multiplayer abstraction boundaries and a locked Phase 3 specification, but no implemented host/join/movement scenario or four-process test coordinator.
   - Target: The delivered Phase 3 LAN scenarios run through the shared runner: a real host and three guest processes with independent profiles and the actual LAN/authority path. Each controls its own actor and observes synchronized state. Compatible scenarios share one owned process set, with bounded waits, per-peer results/logs and cleanup. Routine local execution uses the development OS only. Isolation of overlapping simultaneous runs and hosted full runs belong to Phase 11.
   - Acceptance: AC-14–AC-16 pass, including missing/duplicate peers, capacity and repeat-run controls.

7. **TEST-07 — Inspectable reports and evidence:** Each execution publishes fresh nonempty results, logs, observations and required images bound to the exact case, peer, tested commit, platform and run identity.
   - Current: Phase 1 publishes per-invocation summaries, per-case results, real subject/process logs and source/tool/platform fingerprints. There is no integrated game/image/peer report or accepted measured analyzer/production coverage report.
   - Target: One concise human and machine report per run links cases/peers to actual logs, observations and decoded images, and records the tested commit, whether the tree was clean, the selection with omissions and reasons, and timing per group. Reports and linked artifacts suffice; no dedicated viewer is required. Reports are never rewritten. Coverage counts, per-file fingerprints, completion aggregates and reuse records belong to Phase 11.
   - Acceptance: AC-17, AC-18 and AC-32 pass. Missing, stale or mixed per-run evidence cannot pass.

8. **TEST-08 — Visible failure for incomplete verification:** Verification reports a visible non-success outcome for missing dependencies, zero required-case execution, skipped required cases, crashes, timeouts, missing peers and missing required evidence.
   - Current: Existing runner/architecture/documentation/native tests reject many deliberate failures. The game/peer/image, coverage-report and transferred aggregate certification controls remain incomplete.
   - Target: Every selected required obligation is accounted for against an independent declaration. A missing prerequisite, zero discovery, a skipped case, an actual failure or crash, a timeout, an absent peer result and a missing required artifact each yield non-success with visible diagnostics, proven by one deliberate control each on the shared runner. Blocked obligations remain separate from passed cases; unrelated failures remain inspectable.
   - Acceptance: AC-03, AC-15, AC-18 and AC-20 pass with actual deliberate defective subjects and separately verified clean controls.

9. **TEST-09 — Rejected deliberate failures and separated planned cases:** Deliberate positive and negative trials prove that the runner rejects defective subjects and that a future planned case can neither report a pass nor hide a currently required case.
   - Current: Twelve analyzers and native/Core proofs exist, but real measured 90%/80% coverage and all eight transferred aggregate/coverage identities are pending. The handoff explicitly preserves those IDs and their original meaning.
   - Target: The seven controls of TEST-08 pass against deliberately defective subjects with matching clean controls, and planned cases stay outside the required denominator without pass or skip credit. Measured 90% line / 80% branch coverage and all eight transferred certification identities belong to Phase 11 under TEST-12 with unchanged thresholds and meanings; Phase 5 re-owns their declarations without claiming them.
   - Acceptance: AC-20, AC-30 and AC-45 pass.

10. **TEST-10 — Feature activation and distinct evidence obligations:** Feature delivery activates its owning planned cases as required, preserves earlier regressions and records rendered input, real mixed-OS LAN, reference-PC and endurance evidence as separate obligations with repeatable procedures and accurate completion status.
   - Current: Phase ownership and future-case lifecycle rules are documented; the Phase 1 selector separates its eight later declarations. There is no complete game-case activation procedure or release-environment evidence catalog.
   - Target: The full release catalog exists now, with unimplemented feature cases visibly planned and owned. Feature delivery activates its cases and retains earlier regressions. Rendered input, four local processes, physical mixed-OS LAN, reference-PC performance and endurance are separate evidence kinds in the catalog; each owning phase writes its detailed procedure. Phase 5 supplies the catalog without claiming later gameplay or release runs have passed.
   - Acceptance: AC-30, AC-31 and AC-38 pass; planned or partial evidence cannot conceal a currently required obligation.

## Boundaries

**In scope:**

- One readable catalog for A-01–A-47, the delivered solo, LAN and inspection scenarios, the existing Phase 1–4 suite groups and planned later cases, using the approved requirement and phase identities.
- One shared runner and entry point with three tiers: pure tests, headless scenarios, and rendered solo and LAN groups.
- Windows and Linux pull-request checks targeting about ten minutes per OS: strict build with analyzers, boundary and documentation checks, pure tests and one headless scenario.
- The delivered Phase 2 solo and Phase 3 LAN rendered scenarios running through the shared runner, and one inspection-hub smoke scenario with AI image review and the four Phase 4 judgments.
- The reduced phase-end selection: a budgeted Critical smoke set, new and changed cases, affected regressions and input-triggered rule-suite groups.
- A check that every specification edge ID has a unit test, for use from Phase 6.
- A documented select, run, inspect, reproduce, fix and rerun procedure, with a failure brief and replay command for each failed case.
- One report per run with the tested commit, selection, outcomes, timing and linked logs, observations and images.
- Seven deliberate non-success controls on the shared runner.
- The `world.replacement` engine-race fix and its engine-defect record from the Phase 4 closure exception.
- The Phase 4 checks that Phase 5 runs work in the tree relocated by PR #6, including the pinned recipes that assume the original checkout's `.phase4` folder (CONTEXT D-46).

**Out of scope:**

- Measured 90% line / 80% branch coverage, native-hosted coverage collection and the eight transferred certification IDs — Phase 11 owns them under TEST-12 with unchanged thresholds and meanings.
- Full hosted Windows/Linux runs of rendered, LAN and coverage checks — Phase 11.
- Evidence reuse validation, completion aggregates, per-file fingerprints and artifact hashes — Phase 11.
- Exhaustive runner certification: permutation, repeat, overlapping-run, interruption and Unicode-identity trials and the staged failure exercise — Phase 11.
- The other 17 Phase 4 rendered cases with the physical driver declaration, and the Phase 1 follow-ups (complete Linux suite, graph capacity case, fourteen human reviews) — Phase 11.
- Renewing the Phase 4 review-bound gates (`runner.prelaunch`, `runner.selection`, `acceptance.judgments`, `acceptance.artifacts` and `--verify-completion`) against the relocated tree — Phase 11 (CONTEXT D-46).
- A new runner, manifest or negative-control suite in each later phase, and a catalog row for each edge case.
- Combat, inventories, camp economy, zombies, campaign outcomes, full saves/session recovery and later UI behavior — Phases 6–9 own these; Phase 5 catalogs their future cases.
- Certifying complete campaigns, actual mixed-computer/mixed-OS LAN, reference-PC performance/endurance or final release exports — Phase 10 owns the distinct release evidence.
- A dedicated results dashboard/viewer, approved image baselines and automatic visual-difference gates.
- Scheduled nightly runs — Phase 11 owns them under TEST-11.
- Rewriting the existing Phase 1–4 runners, weaker rule enforcement, wider suppressions or deleted regressions — the delivered foundation stays as it is.

## Constraints

- Preserve Godot/C#, the existing project/engine/service boundaries, compatible proven pins/locks, all twelve analyzers, exact rule thresholds, bounded generated/suppression permissions and existing proofs. A candidate test adapter is not proven by package availability or successful compilation.
- Prove actual discovery, subject execution, process exits and deliberate failure behavior for the shared runner. An incompatible candidate may be replaced by a bounded repository-owned harness while retaining every Phase 5 acceptance obligation.
- Test drivers use production input/commands; initialization and read-only observation are distinct. Test-only authority controls are excluded from release builds. Shared fixture setup must not become substitute gameplay.
- During implementation use checks without a real game window: tier 1 per task and the touched tier 2 groups per plan. Do not rerun the whole selection per plan. After whole-phase review and fixes, run the reduced selection once on the development OS at 1280×720 with no VM, then follow the repair cycle in AC-37.
- Preserve the Phase 2/3 display, input, authority, movement, timing and lifecycle contracts and the Phase 4 census, presentation decisions and bounded cosmetic exception.
- Every scenario and process has explicit bounded waits and owned cleanup. Ordinary cached PRs target about ten minutes per OS; this is a target, not permission to drop a selected check or change an inherited proof deadline. Budgets for tier 1 and the smoke set are set from measured runs.
- Cases are selected from independent declarations and phase ownership, not from discovery, implementation labels or prior outcomes. Future planned work is never passed, skipped for credit or allowed to hide a currently required case.
- Evidence for Phases 5–10 is the tested commit, the run report and its logs, observations and images. Reports keep failure history and are never rewritten. A run cannot present another run's artifacts as its own. Unreviewed required visuals cannot be relabeled accepted.
- Deferral is not acceptance. Nothing moved to Phase 11 is recorded as passed, and the 90% line / 80% branch targets and the eight transferred identities keep their original meaning there.
- This is a planning-only change. It does not advance STATE.md, complete a requirement, run the game or establish CI acceptance.

## Acceptance Criteria

- [ ] **AC-01:** The catalog accounts for exactly the source identities A-01 through A-47, with no missing, duplicated or renumbered source ID. It also lists the delivered solo, LAN and inspection scenarios and one entry for each existing Phase 1–4 suite group. Unit tests are discovered from the test project and are not catalog rows. Additional scenarios use their own identities rather than altering the 47 source IDs.

- [ ] **AC-02:** Every catalog entry has a readable title and valid, nonblank requirement, owning-phase, status, importance, tag, tier, run-group and check-or-procedure fields under AC-35. Missing, duplicate or dangling values are rejected. A source umbrella scenario may link several checks.

- [ ] **AC-03:** Selection exposes the independently declared expected set. Document the exact case and group selections, the pull-request selection, the reduced phase-end selection and the manual full selection. Preserve the existing exhaustive phase-selector semantics of the Phase 1 runner: positive integer phases include declarations with acceptancePhase at or below that phase. The reduced selection is a distinct declared operation under AC-36. Invalid or conflicting filters and a run with zero selected required cases are non-success.

- [ ] **AC-04:** Windows and Linux pull requests run the strict build with analyzers on actual code, project and code boundary checks, documentation and catalog validation, the pure tests (tier 1) and the one headless scenario in AC-44. They open no game window and run no rendered review, four-process LAN, rule-suite defect proofs or coverage gate. Target about ten minutes per OS with dependencies cached and report the measured time. If the headless scenario pushes a run past the target, move it to a separate job that does not block merging and say so. Reports name the selected checks.

- [ ] **AC-05:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Manual full GitHub Actions and local entry points can execute the same complete declared required scope, including inherited rules/native proofs, controlled fixtures, rendered input, content checks, real four-process LAN and coverage/certification. Prove actual successful full Windows/Linux Actions execution, not configuration alone. Routine PR scope is separate. Later closure may retain unchanged group evidence from those actual executions under AC-06/AC-33 rather than repeat everything.

- [ ] **AC-06:** Before Phase 5 completion, reconcile its declared local selection on the development OS at 1280×720 and one actual successful pull-request workflow run on Windows and on Linux for the final source. Do not require or launch a local other-OS VM. Record the actual OS, tools and tested commit. Cross-builds and prepared environments supply no acceptance. Full hosted runs of rendered, LAN and coverage checks belong to Phase 11.

- [ ] **AC-07:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* An agent follows the repository procedure through one actual controlled current-run failure: find its declaration, select/run it, locate the exact report/log/state/image evidence, reproduce the recorded setup/seed/action sequence, diagnose and repair the cause, and obtain the required fresh reruns. Missing reproduction information or required artifacts leaves the exercise unaccepted.

- [ ] **AC-08:** A failed run's report and artifacts are kept and never overwritten. A later pass has its own run identity and cannot replace, relabel or hide the failure. The runner does not retry a failed case into a pass. If a failure passes on an unchanged rerun, record a diagnosis before acceptance.

- [ ] **AC-09:** Controlled fixtures execute actual delivered production behavior and assert authoritative outcomes for relevant empty, single and multiple-state inputs, equal identities, repeated actions and meaningful event ordering. Initial scenario setup may be controlled, but gameplay actions follow the production path and observations do not mutate it. Independent scenarios do not share mutable clocks, random streams or setup.

- [ ] **AC-10:** Elapsed/random-input fixture boundaries follow their actual units and range contracts, including zero, nearest valid/invalid neighbors, insufficient/negative values where applicable and overflow/fraction rejection. Repeated setup/seed/time/action inputs produce the specified logical state. This does not require bit-identical pixels, process timings or network scheduling.

- [ ] **AC-11:** Rendered checks exercise real menu/keyboard/mouse routes and assert scene entry/exit/reentry, movement/facing and focus handling under Phase 2/3 contracts. Routine local captures use only 1280×720; preserve separate supported-display hosted/release procedures. Before/after checkpoints are within selected current-build scenarios. Group compatible checks in shared runs under AC-37. Input cannot leak through focused controls; reentry cannot duplicate actions. Missing/undecodable images or absent objective assertions cannot pass.

- [ ] **AC-12:** One inspection-hub smoke scenario launches the hub, visits every category and captures one current-run image per category with objective resource and state results. The AI records which images it inspected, its appearance and readability findings and any defects. The four Phase 4 judgments P1–P4 are recorded on these images under their original procedure. No screenshot baseline or image-diff gate is required. A missing declared review stays unaccepted; headless execution cannot supply it. The other 17 Phase 4 rendered cases and the Phase 4 driver declaration are executed in Phase 11 under their original procedures.

- [ ] **AC-13:** Test drivers exercise the production input/command path; read-only observations cannot write the expected gameplay outcome. Deliberate invalid-command tests may address the real command boundary, but do not certify keyboard/mouse rendering by themselves. Test-only authority mutation controls are absent from release builds.

- [ ] **AC-14:** The full LAN scenario launches one real host and three real guests with distinct profiles and actual network/authority behavior. Every peer has its own required results and logs; each moves only its actor and observes the coherent production baseline/movement/facing. Preserve all Phase 3 discovery/manual-join, readiness, identity, capacity, timing/tolerance, disconnect and teardown acceptance contracts; mocks or synthetic peer records cannot satisfy this scenario.

- [ ] **AC-15:** Capacity and runner controls distinguish legal one-to-four-player production sessions from the required four-process scenario. A missing host/guest, duplicate profile/actor claim, fifth admitted participant, absent peer result or expired wait prevents the corresponding acceptance. Deadlines are explicit, positive, bounded and checked without numeric coercion or overflow; missing confirmed completion by the deadline is non-success. Equal display names do not merge opaque identities.

- [ ] **AC-16:** A repeated run starts from fresh profiles, user data, session endpoints, logs and artifacts, with no stale actors, slots, callbacks or peer results. Cleanup verifies ownership and releases the run's own processes and descendants without terminating unrelated games. Isolation of overlapping simultaneous runs belongs to Phase 11.

- [ ] **AC-17:** Each run publishes one report with a run identity, the tested commit and whether the working tree was clean, the operating system and tool versions, the selected scope, commands and exit codes, per-case outcomes and links to its logs, observations and images. Required artifacts are fresh, nonempty and valid; artifacts from another run cannot establish this run's result. Published reports are never edited. Per-file input fingerprints, artifact hashes and completion aggregates belong to Phase 11.

- [ ] **AC-18:** Each run reconciles the expected cases, peers and required artifacts against its actual discovery and results, and rejects missing, duplicate, unexpected or skipped records. Failed, blocked, planned and not-selected cases stay distinct and receive no pass credit.

- [ ] **AC-19:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Catalog/report/reproduction metadata is valid UTF-8. Stable case/profile/actor/peer/module identities and portable path segments retain exact spelling, case and Unicode identity; display names do not substitute for them. Valid Unicode workspace/profile paths work. Malformed encoding, wrong-case or silently normalized identity references fail explicitly.

- [ ] **AC-20:** Seven deliberate trials on the shared runner are visibly rejected: a missing dependency, zero discovered cases, a skipped required case, an assertion failure or crash, a timeout, a missing peer result, and missing required evidence such as an absent log or image. Each trial establishes its prerequisites and reaches the intended defective subject; a setup failure alone is not the expected rejection. A matching clean control passes. The existing Phase 1–4 runner controls stay in their suites. Trials for bad filters, cancellation, stale, duplicate or unknown results and wrong identities belong to Phase 11.

- [ ] **AC-21:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Collect genuine separate analyzer and handwritten production line/branch evidence. The production scope includes applicable Core, multiplayer ports and native-hosted Godot managed code; measure an actual engine callback and nonzero branch behavior. Inventory expected modules explicitly. A missing expected handwritten module is a failure; modules with no executable sequence points and legitimate generated exclusions are classified with reasons, not silently removed.

- [ ] **AC-22:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Each of the analyzer and production scopes independently meets inclusive 90% line coverage and 80% branch coverage. Compare exact covered/total counts with overflow-safe integer arithmetic, not displayed rounded percentages. Exact-threshold inputs pass; the nearest representable below-threshold inputs fail. Validate nonnegative integral counts, covered <= total and legitimate denominators; an expected executable scope cannot pass with zero/absent measurement.

- [ ] **AC-23:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Coverage gates reject absent, empty, malformed, duplicate, stale, wrong-source/scope/module and below-limit reports, invalid/fractional/overflowing counts and unexplained omissions/exclusions. Valid module/report-row permutations preserve exact totals and verdicts. The analyzer and production scopes cannot compensate for each other.

- [ ] **AC-24:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Functional execution, coverage collection, validation and publication form an acyclic process; collection/certification checks do not recursively invoke the full suite. Retain source identity and original production binaries; isolated copied assemblies/PDBs may be instrumented if needed to measure native hosting. Do not change production behavior or omit unmeasured engine code to raise coverage. Fresh certification retains all failed and successful measurement attempts.

- [ ] **AC-25:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Activate and obtain actual passing evidence for ac-02.complete, ac-06.complete, ac-07.complete, ac-09.complete, R-49.positive, R-49.negative, R-82.positive and R-82.negative with their original exhaustive meanings. Retain existing implemented case identities and all applicable prior regressions. The current 684/676 counts are a scout snapshot, not a cap on later required declarations; new cases are declared before acceptance.

- [ ] **AC-26:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* The aggregate register, enforcement and numeric/report certifications reconcile real functional mechanisms and the actual measured coverage evidence, including native/state evidence. A precoverage binding, existing Phase 1 pass, empty wrapper or successful subprocess exit cannot substitute for the transferred aggregate acceptance.

- [ ] **AC-27:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Permuting catalog entries, semantically unordered fixture inputs, peer/result arrival order or coverage/module rows does not alter set membership, exact totals or acceptance. Duplicate identity ties are rejected. Scenario command/focus/readiness/revision ordering that affects actual production behavior remains explicit and is not permuted away.

- [ ] **AC-28:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Repeating a scenario from its recorded setup, seed, controlled time and action sequence reproduces its specified logical outcomes and inherited tolerances, with a fresh invocation and clean scenario state. Repeats do not overwrite reports, reuse accepted artifacts, create extra actors or change the reference behavior. Pixel bytes, timestamps and process identities need not match.

- [ ] **AC-29:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Concurrent execution, interruption, crash, timeout and cancellation cannot mix cases, peers, profiles, resources or artifacts across invocations. Preserve available diagnostics and a non-success interrupted outcome. Partial publication or an input/catalog change during execution cannot produce an accepted final report. Owned resources are cleaned up; historical reports remain immutable.

- [ ] **AC-30:** The catalog distinguishes required cases from future planned cases with explicit owners and acceptance phases. Feature delivery activates corresponding checks/procedures alongside the feature and retains earlier regressions. Repeated activation does not duplicate cases, change source IDs or lose history. Planned cases remain outside the current required denominator without pass/skip credit; implementation labels cannot hide a declaration already required by the selected scope.

- [ ] **AC-31:** The catalog records separate evidence kinds for headless state, rendered input with AI review, four local processes, physical mixed-computer and mixed-OS LAN, reference-PC rendering performance and saved/resumed endurance. Later feature and release entries stay planned under their approved owners until executed, and each owning phase writes its detailed procedure. Local and hosted automation does not certify the physical-LAN, hardware or endurance obligations.

- [ ] **AC-32:** Run summaries name the run kind: pull request, reduced phase-end or manual full. They show selected, failed, blocked, not-selected and planned cases, importance, run group and order, process counts, selection and omission reasons, review gaps and measured time per group. A selected case with a missing tool, evidence or review is non-success. No dedicated viewer or screenshot baseline is required. Phase 11 owns nightly full execution; no nightly success is presumed before it exists.

- [ ] **AC-33:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Reuse validation accepts eligible unchanged passing group evidence in its required environment and rejects changed assets/code or transitive dependencies outside the group's folder, changed test/case definitions or selected membership, incompatible tools/environment, missing/altered artifacts, wrong case/peer identity, unresolved later failures and unknown impact. Coverage validates complete module/scope inputs; eligible visual reuse includes inspected images/review, but cannot replace AC-37's fresh final rendered selected-set pass after repairs. Validate consumers and shared inputs, not only directories. Exercise genuine positive/negative controls, an eligible validation needing zero new executions, a repaired rendered selection that still requires a fresh final pass, and missing/invalidated obligations an old pass cannot hide. Never relabel revisions/invocations or credit unselected cases.

- [ ] **AC-34:** *Moved to Phase 11 under TEST-12; not a Phase 5 gate.* Publish the logical groups and their selection dependencies, purpose, expected artifacts and measured duration by OS/environment. Record cached and cold setup separately from execution, and total PR wall time. Optimize redundant setup/build/process launches and use only proven isolated bounded concurrency; retain all case assertions, deliberate-defect meanings and native/production paths. Ten minutes is the ordinary cached PR design target, not a fabricated result, hard gameplay deadline or justification to truncate scope. Full behavior/coverage/visual durations remain unmeasured until those implementations run.

- [ ] **AC-35:** Integrate every phase's repository-stored scenarios into the one catalog. Each scenario has a stable ID, source or requirement, owning phase, required or planned status, Critical, High, Normal or Low importance with a one-line reason, tags, tier, setup, ordered production actions, expected results, run group and order, reset, process roles and any fresh-process reason. Pure checks declare that they need no window. Invalid importance, a missing or duplicate order and a dangling group reference fail validation, with a clean control.

- [ ] **AC-36:** The reduced phase-end selection contains the Critical smoke set under AC-41, every new or changed case and every affected regression regardless of importance. Affected cases are found from the phase branch's changed files against `dev` through declared tags and suite inputs, including the rule-suite trigger in AC-42; unknown impact cannot silently narrow the selection. Prove that a changed Low case, an unchanged smoke case, an affected consumer and a planned Critical case select correctly. Reports list every omission and its reason without pass credit; a reduced pass does not claim that the complete catalog passed.

- [ ] **AC-37:** Prove the operational sequence. During implementation each task runs its own tier 1 tests and each plan runs the tier 2 groups it touched; no plan reruns the whole selection. After whole-phase implementation and code review with fixes, run the selected tier 2 groups and the tier 3 rendered groups on the development OS at 1280×720 with no VM. Compatible cases share processes using ordinary resets; multiplayer keeps its required peer roles. Record failures and continue independent cases. Batch repairs, rerun failed, blocked and newly affected cases, then obtain a fresh clean pass of the entire selected set on the repaired build. Preserve the initial, repair and final reports. An initial clean selected-set run needs no duplicate launch.

- [ ] **AC-38:** Each phase saves or updates its scenarios in the one catalog and adds its unit tests to the test project. Phase 5 imports the existing Phase 2–4 definitions and documents how later phases contribute, select and repair. The manual full local run includes all active importance levels with the same groups. Catalog TEST-11 and TEST-12 as planned Phase 11 work; implement no scheduled run in Phase 5 and give no unexecuted nightly, certification or future-feature pass credit.

- [ ] **AC-39:** One documented entry point runs all three tiers from the one catalog. The delivered Phase 2 solo and Phase 3 LAN rendered scenarios run through it. A later phase adds test files and catalog rows only; it adds no runner, manifest or negative-control suite of its own. The existing Phase 1–4 runners under src/godot-game/tools/ remain callable as suites and are not rewritten.

- [ ] **AC-40:** The three tiers exist with measured durations. Tier 1 runs the pure tests in one test process. Tier 2 runs headless game scenarios with one launch per group. Tier 3 runs the rendered solo group and the rendered host-plus-three-guests group. Record the measured time of each tier on the development OS and set the tier 1 and smoke-set budgets from those measurements.

- [ ] **AC-41:** Critical is a fixed, listed smoke set: launch, menu, start, movement and host with three guests now, and later the flows whose failure stops play or corrupts, loses or duplicates state. The set has a rendered time budget set from measurement, proposed at ten minutes. Adding a Critical case keeps the set inside the budget or replaces another. Re-rank the existing Phase 2–4 cases once under this definition; a case that leaves the smoke set becomes High, Normal or Low and stays in the catalog.

- [ ] **AC-42:** Each Phase 1 suite group declares its input paths. A group is selected at phase end only when the phase branch changed one of its inputs, such as the analyzers, build properties, exemption and permit registers, the test project or the runner. Otherwise the strict build, which applies every analyzer to all real code, is the regression check. Prove both directions: an analyzer change selects the rule group, and a gameplay-only change selects none of it.

- [ ] **AC-43:** From Phase 6, each specification edge case is a unit test that carries its edge ID. One check compares a phase's specification edge IDs with the test IDs and fails on a missing or duplicate ID. Edge cases have no catalog row, per-case command or separate evidence file. Phase 5 delivers the check and proves it with one matching and one missing ID.

- [ ] **AC-44:** One headless scenario launches the game, enters the scene, moves a character and quits, on Windows and on Linux in the pull-request workflow. It exists so that Linux game code runs before release validation.

- [ ] **AC-45:** The eight transferred certification declarations (`ac-02.complete`, `ac-06.complete`, `ac-07.complete`, `ac-09.complete`, `R-49.positive`, `R-49.negative`, `R-82.positive`, `R-82.negative`) are re-owned to Phase 11 in the Phase 1 manifest and stay pending without pass or skip credit. The existing selector controls are updated to match, and a Phase 5 selection no longer requires them.

## Acceptance Disposition

Approved by the creator on 2026-11-23. Moved criteria keep their identifiers and wording above and become Phase 11 gates under TEST-12.

| Criteria | Disposition |
| --- | --- |
| AC-09, AC-10, AC-11, AC-13, AC-14, AC-15, AC-30 | Phase 5, unchanged |
| AC-01, AC-02, AC-03, AC-04, AC-08, AC-12, AC-17, AC-18, AC-31, AC-32, AC-35, AC-36, AC-37, AC-38 | Phase 5, narrowed in place |
| AC-06, AC-16, AC-20 | Split: the stated part stays in Phase 5 and the remainder moves to Phase 11 |
| AC-05, AC-07, AC-19, AC-21–AC-29, AC-33, AC-34 | Phase 11 under TEST-12 |
| AC-39–AC-45 | New in Phase 5 |

## Pre-Amendment Wording

The 2026-11-15 wording of each criterion narrowed or split on 2026-11-23. Phase 11 takes the parts that left Phase 5 from here.

- **AC-01 (2026-11-15):** The catalog accounts for exactly the source identities A-01 through A-47, with no missing, duplicated or renumbered source ID, and also includes the existing rule checks plus BASE, LAN, ASSET, TEST and regression coverage. Additional cases use their own identities rather than altering the 47 source IDs.
- **AC-02 (2026-11-15):** Every case has a readable scenario and valid, nonblank requirement/owner/acceptance-phase/status/check-or-procedure/environment/artifact references, plus the importance, dependency and run-group metadata in AC-35. Invalid, duplicate or dangling metadata is rejected. A source umbrella scenario may link multiple checks with explicit ownership and evidence.
- **AC-03 (2026-11-15):** Selection exposes the independently declared expected set. Document exact case/group, basic PR, reduced phase-end and manual full/current-phase selections. Preserve existing exhaustive phase-selector semantics: positive integer phases include declarations with acceptancePhase at or below that phase, irrespective of implementation status. The reduced selection is a distinct declared operation under AC-36, not a silent narrowing of an existing exhaustive command. Invalid/conflicting filters and regular execution with zero selected required cases are non-success. A distinct validation may execute no new cases only for eligible matching evidence under AC-33; it cannot bypass AC-37's fresh final rendered pass after repairs.
- **AC-04 (2026-11-15):** Ordinary Windows/Linux PRs run strict build/analyzers on actual code, current project/code boundary checks and documentation/catalog validation, with no game launch, rendered review, four-process LAN, exhaustive deliberate-defect proofs or coverage certification. Target about ten minutes per OS with dependencies cached; record measured group/total time and overruns without dropping selected checks. Feature selections use changed folders plus transitive consumers/shared inputs; unknown impact is a visible blocker. Reports name selected cases, omissions and reasons.
- **AC-06 (2026-11-15):** Before Phase 5 completion, reconcile its declared local selection on the development OS plus actual hosted Windows/Linux full-workflow compatibility and the coverage/certification obligations. Do not require or launch a local other-OS VM. Routine local rendered execution is 1280×720 only; separately catalog supported-display/release evidence. Validate eligible reuse under AC-33 and honor AC-37's fresh final rendered selected-set pass after repairs. Preserve actual OS/kernel/architecture, tools, revisions, fingerprints and original invocation/artifact identities. Clean execution is donor/cache/prior-output independent. Prepared environments, cross-builds and relabeled reports supply no acceptance.
- **AC-08 (2026-11-15):** If a failure passes on an unchanged rerun, retain both attempts and a diagnosis before acceptance. A later successful attempt has a fresh invocation identity and the appropriate clean required results; it cannot replace, relabel or hide the original failure. An automatic retry by itself cannot turn the failed attempt into a pass.
- **AC-12 (2026-11-15):** Delivered content/inspection fixtures produce objective resource/state results and the required current-run images. The AI records which images it inspected, appearance/readability findings and any defects, linked to case/run/source identity. Preserve the Phase 4 resource census, visual decisions and explicit minor-cosmetic exception; no screenshot baseline or image-diff gate is required. Missing declared visual review stays unaccepted; headless execution cannot supply it.
- **AC-16 (2026-11-15):** Repeated and overlapping runs preserve isolated profiles, user data, session endpoints, logs and artifacts. Conflicting access to input focus is handled explicitly rather than sending actions to another window. Cleanup verifies ownership and releases the run's processes/descendants/resources, without terminating unrelated games, another run or the preserved local VM. A rerun has no stale actors, slots, callbacks or peer results.
- **AC-17 (2026-11-15):** Each execution publishes immutable identity, revision/input fingerprint, actual tool/platform identities, selected scope, commands/exits, outcomes and artifact references/hashes. Its required logs/observations/images are fresh, nonempty and valid; source changes or artifacts from another invocation cannot establish that run's acceptance. A distinct completion aggregate may reference original prior reports only through explicit validated reuse under AC-33, never by copying or rewriting them as fresh.
- **AC-18 (2026-11-15):** Each execution reconciles exact expected cases/peers/artifacts against its actual fresh discovery/results, rejecting missing, duplicate, unexpected, skipped, stale or mismatched records. Shared artifacts are declared and cannot replace distinct results. Completion aggregates additionally validate exact required membership and every reused reference under AC-33. Independent failures, blocked obligations, planned work and missing/invalidated evidence remain distinct and receive no pass credit.
- **AC-20 (2026-11-15):** Deliberate trials visibly reject missing/incompatible tools, a bad runner, zero discovery, a wrong filter, skipped required cases, actual assertion failures, crash, timeout, cancellation, missing peer completion, missing/empty logs or state observations or rendered images, stale/duplicate/unknown results and wrong run/source/tool/platform identities. Each negative control establishes its prerequisites and reaches the intended defective subject; setup failure alone is not the expected rejection. Matching clean controls pass.
- **AC-31 (2026-11-15):** Store separate repeatable procedures, environments, expected artifacts and completion records for headless/native state, rendered input and AI review, four local processes, actual mixed-computer/mixed-OS LAN, reference-PC rendering performance and saved/resumed endurance. Later feature/release records remain planned under their approved owners until actually executed. Local/hosted automation does not certify the distinct physical-LAN, hardware or endurance obligations.
- **AC-32 (2026-11-15):** Summaries distinguish basic PR, reduced phase-end, manual full execution and certification/evidence validation. Show executed-now, eligible reuse, selected missing/invalidated/failed, not-selected and planned cases with original provenance. Include importance, run-group/order, launch/process counts, selection/omission reasons, exact lists, review/environment gaps and timing. Selected missing tools/evidence/review or invalid reuse is non-success. No dedicated viewer or screenshot baseline is required. Phase 11 owns nightly full execution; no nightly success is presumed before it exists.
- **AC-35 (2026-11-15):** Integrate every phase's repository-stored AI-readable scenarios. Validate stable ID/source/requirement/owner, activation, Critical/High/Normal/Low importance with consequence-based rationale, dependency/impact tags, preconditions/setup/seed, ordered production actions, expected results, evidence/replay procedure, persistent game-run group/order, reset/recovery, process roles and any fresh-process reason. Pure checks explicitly need no window. Groups list shared setup, ordered cases, resets and exits. Invalid importance, missing/duplicate order, dangling group references and unexplained extra launches fail metadata validation, with meaningful clean controls.
- **AC-36 (2026-11-15):** The reduced phase-end selection applies immediately and contains every active Critical case, every new/changed case and all affected regressions regardless of importance. Account for consumers, transitive dependencies and shared inputs; unknown impact cannot silently narrow selection. Prove changed Low cases, unchanged Critical cases, affected consumer cases and planned Critical cases select correctly. Preserve catalog membership and activation independently from run selection. Reports list every omission and reason without pass/skip credit; reduced success does not claim the complete catalog passed.
- **AC-37 (2026-11-15):** Prove the operational sequence: whole-phase implementation with no real game windows, code review/fixes, then development-OS-only 1280×720 grouped game testing with no VM. Compatible cases share processes using ordinary resets; multiplayer retains required peer roles and fresh-process needs have reasons. During a usable run, record failures and continue independent cases. Batch repairs, rerun failed/blocked/newly affected cases, then obtain a fresh clean pass of the entire selected set on the repaired build in one run or explicit compatible groups. Repeat after new failures. Preserve initial/repair/final reports; stale passes cannot replace this final pass. An initial clean selected-set run needs no duplicate launch.
- **AC-38 (2026-11-15):** Each phase, including those before Phase 5, saves or updates reusable scenario definitions with importance and ordered run groups, linked from its context/verification record. Phase 5 imports existing definitions and documents the contribution/selection/repair procedure for later phases. Manual full execution includes all active importance levels with the same groups and authentic evidence rules. Catalog TEST-11 as Phase 11-owned planned nightly full-regression work; implement no scheduled nightly requirement in Phase 5 and give no unexecuted nightly or future-feature pass credit.

## Edge Coverage

**Original specification coverage:** 67/67 applicable edges resolved. The later discussion amendment adds explicit execution/reuse controls in AC-33–AC-34; no new automated edge-probe result or ambiguity score is claimed.
**Verification:** 67 explicit · 0 backstop · 0 dismissed

The original compiled edge probe ran over the ten pre-amendment requirement statements with authored data/behavior shapes. Its deterministic floor proposed 63 edges, all resolved explicitly. Manual review added phase-selector boundaries, reproduction text/paths, rendered-size boundaries and LAN identity encoding. The creator accepted those grouped proposals. The table preserves that original record; AC-33–AC-34 add discussion-era reuse/runtime controls without claiming a new probe execution.

**2026-11-23 amendment note:** The table records the original resolutions. A row that cites a criterion moved to Phase 11 is verified there with that criterion.

| Category | Requirement | Status | Resolution / Reason |
|----------|-------------|--------|---------------------|
| adjacency | TEST-01 | resolved; verification: explicit | AC-01–AC-02: duplicate case/source IDs are rejected; distinct source, foundation and regression IDs remain distinct. |
| empty | TEST-01 | resolved; verification: explicit | AC-01–AC-03: missing source scenarios, blank required metadata and an empty required selection cannot pass. |
| encoding | TEST-01 | resolved; verification: explicit | AC-02, AC-19: metadata is valid UTF-8; case IDs and portable references retain exact spelling and Unicode identity. |
| ordering | TEST-01 | resolved; verification: explicit | AC-01, AC-27: catalog enumeration order changes neither source coverage nor the selected case set. |
| adjacency | TEST-02 | resolved; verification: explicit | AC-04–AC-06, AC-32: fast, full, local, VM and hosted results keep separate scope and environment identities. |
| empty | TEST-02 | resolved; verification: explicit | AC-03–AC-06: a fast/full selection with no required executable checks or missing platform results cannot certify success. |
| ordering | TEST-02 | resolved; verification: explicit | AC-04, AC-27: changed-path, catalog and result ordering cannot alter applicable selection or its verdict. |
| idempotency | TEST-02 | resolved; verification: explicit | AC-05–AC-06, AC-28: repeated commands execute fresh checks against recorded inputs rather than reusing an earlier pass. |
| concurrency | TEST-02 | resolved; verification: explicit | AC-16, AC-29: overlapping local/CI runs have isolated resources and evidence; interruption cannot certify completion. |
| adjacency | TEST-03 | resolved; verification: explicit | AC-07–AC-08: failures and retries of the same case remain distinct attempts with a linked diagnosis. |
| empty | TEST-03 | resolved; verification: explicit | AC-07–AC-08: absent reproduction inputs, failure artifacts or diagnosis leaves verification unaccepted. |
| ordering | TEST-03 | resolved; verification: explicit | AC-07–AC-08: inspect and diagnose the failure before accepting the fresh rerun; required verification follows the repair. |
| idempotency | TEST-03 | resolved; verification: explicit | AC-08, AC-28: replay uses the recorded logical setup and obtains a new run identity without replacing the failed attempt. |
| concurrency | TEST-03 | resolved; verification: explicit | AC-16, AC-29: reproducing one failure neither changes another active scenario nor overwrites its evidence. |
| boundary | TEST-04 | resolved; verification: explicit | AC-09–AC-10: controlled elapsed/random inputs exercise accepted boundaries, nearest invalid neighbors and zero as defined by their actual production contracts. |
| adjacency | TEST-04 | resolved; verification: explicit | AC-09: equal identities or repeated actions retain the actual production rejection/idempotency behavior; fixtures do not merge them to manufacture the expected state. |
| empty | TEST-04 | resolved; verification: explicit | AC-09: relevant empty, single and multiple-state fixtures assert actual behavior; an empty discovered test set cannot pass. |
| ordering | TEST-04 | resolved; verification: explicit | AC-09, AC-28: the recorded command/event order is replayed; semantically unordered inputs may be permuted without changing the expected logical verdict. |
| precision | TEST-04 | resolved; verification: explicit | AC-10: units, integer ranges and overflow rejection match the controlled-input contracts; rounding cannot turn invalid values into valid inputs. |
| idempotency | TEST-04 | resolved; verification: explicit | AC-09, AC-28: identical setup, time, seed and logical action sequence reproduce the expected authoritative state. |
| concurrency | TEST-04 | resolved; verification: explicit | AC-09, AC-29: simultaneous fixtures do not share mutable setup, clocks or random streams; observation does not mutate authority. |
| adjacency | TEST-05 | resolved; verification: explicit | AC-11, AC-28: repeated input and scene reentry cannot duplicate actions or retained handlers; evidence remains attached to its actual scene/window. |
| empty | TEST-05 | resolved; verification: explicit | AC-11–AC-12: absent or undecodable images, empty scene results and missing required review evidence do not establish a rendered pass. |
| ordering | TEST-05 | resolved; verification: explicit | AC-11–AC-12: assert the intended focus/scene state before input and capture the resulting state after the action. |
| idempotency | TEST-05 | resolved; verification: explicit | AC-11, AC-28: menu/scene exit and reentry work repeatedly with the inherited input/focus behavior. |
| concurrency | TEST-05 | resolved; verification: explicit | AC-16, AC-29: competing windows cannot silently receive each other's input; conflicting access is isolated or reported as non-success. |
| boundary | TEST-06 | resolved; verification: explicit | AC-14–AC-15: require exactly the declared four-process scenario, retain the one-to-four-player production capacity and reject a fifth participant under the inherited contract. |
| adjacency | TEST-06 | resolved; verification: explicit | AC-14–AC-15: equal display names do not merge distinct profiles; duplicate profile/actor/peer claims cannot satisfy distinct participant obligations. |
| empty | TEST-06 | resolved; verification: explicit | AC-14–AC-15: absent host, missing guest or empty peer evidence makes the four-process scenario non-success. |
| ordering | TEST-06 | resolved; verification: explicit | AC-14, AC-27: joining or receiving reports in a different order cannot omit a peer; readiness and revision order still follow the production contract. |
| precision | TEST-06 | resolved; verification: explicit | AC-14–AC-15: inherited movement tolerances, capacity and configured deadlines are checked without percentage/time rounding or overflow. |
| idempotency | TEST-06 | resolved; verification: explicit | AC-16, AC-28: rerunning starts from fresh isolated session state, without extra actors, occupied stale slots or reused peer reports. |
| concurrency | TEST-06 | resolved; verification: explicit | AC-16, AC-29: simultaneous sessions isolate profiles, endpoints, logs and process ownership; cleanup cannot terminate another session or the preserved VM. |
| boundary | TEST-07 | resolved; verification: explicit | AC-17–AC-18, AC-21: every declared case/peer/artifact and expected handwritten coverage module is accounted for; one missing obligation prevents acceptance. |
| adjacency | TEST-07 | resolved; verification: explicit | AC-17–AC-18: duplicate result identities are rejected; shared artifacts must be explicitly declared and cannot substitute for distinct case/peer results. |
| empty | TEST-07 | resolved; verification: explicit | AC-17–AC-18, AC-21–AC-23: absent/empty logs, images or expected measured modules fail; genuinely non-executable modules are explicitly classified. |
| encoding | TEST-07 | resolved; verification: explicit | AC-19: exact UTF-8 identities and path segments survive reporting; malformed or silently normalized identity evidence is rejected. |
| ordering | TEST-07 | resolved; verification: explicit | AC-18, AC-27: result and artifact arrival order changes neither completeness nor verdict; duplicate ties are rejected. |
| precision | TEST-07 | resolved; verification: explicit | AC-22–AC-23: covered/total values are exact valid integers, with covered not exceeding total and separate line/branch denominators. |
| idempotency | TEST-07 | resolved; verification: explicit | AC-17, AC-28: a later run publishes new immutable evidence; it cannot overwrite or inherit an earlier run's accepted artifacts. |
| concurrency | TEST-07 | resolved; verification: explicit | AC-17, AC-29: incomplete publication and cross-run artifact mixtures cannot satisfy a finished run. |
| boundary | TEST-08 | resolved; verification: explicit | AC-03, AC-15, AC-20: zero required execution, one missing peer/case, invalid phase/deadline values and expired deadlines are non-success. |
| adjacency | TEST-08 | resolved; verification: explicit | AC-18, AC-20: duplicate, unknown or mismatched case/peer/evidence claims are rejected instead of deduplicated into success. |
| empty | TEST-08 | resolved; verification: explicit | AC-20: missing tools, discovery, results, state observations, required images or expected coverage are explicit failures or blockers. |
| encoding | TEST-08 | resolved; verification: explicit | AC-19–AC-20: malformed metadata and wrong-case/changed identity references fail visibly. |
| ordering | TEST-08 | resolved; verification: explicit | AC-18, AC-20, AC-27: deliberate failures remain failures under result-order permutations; independent failures remain visible. |
| precision | TEST-08 | resolved; verification: explicit | AC-10, AC-15, AC-23: fractions, negative/overflowed counts, invalid durations and invalid report denominators cannot evade validation. |
| idempotency | TEST-08 | resolved; verification: explicit | AC-08, AC-20, AC-28: the same injected defect still fails on repeat; a later clean attempt does not erase it. |
| concurrency | TEST-08 | resolved; verification: explicit | AC-20, AC-29: crash, cancellation, missing peer completion and interrupted publication cannot become a successful run. |
| boundary | TEST-09 | resolved; verification: explicit | AC-21–AC-23: exactly 90% lines and 80% branches pass each scope; the nearest representable below-threshold counts fail. |
| adjacency | TEST-09 | resolved; verification: explicit | AC-23, AC-25–AC-26: duplicate coverage identities or aggregate results cannot conceal an absent module or transferred case. |
| empty | TEST-09 | resolved; verification: explicit | AC-21–AC-23: missing/empty expected scopes or callback evidence fail; zero executable modules require explicit classification. |
| encoding | TEST-09 | resolved; verification: explicit | AC-19, AC-23: changed source/module identities or malformed metadata invalidate measured evidence. |
| ordering | TEST-09 | resolved; verification: explicit | AC-23, AC-27: permutations of valid coverage/module/result rows retain exact totals and the same verdict. |
| precision | TEST-09 | resolved; verification: explicit | AC-22–AC-23: overflow-safe exact integer comparisons enforce inclusive targets; rounded percentages never decide acceptance. |
| idempotency | TEST-09 | resolved; verification: explicit | AC-24, AC-28: repeated certification collects fresh same-source evidence and preserves original binaries and prior reports. |
| concurrency | TEST-09 | resolved; verification: explicit | AC-24, AC-29: collection/validation/publication do not recursively launch full certification or combine concurrent-run outputs. |
| adjacency | TEST-10 | resolved; verification: explicit | AC-30–AC-32: planned, required, fast, full and distinct environment/evidence categories cannot be merged into a broader completion claim. |
| empty | TEST-10 | resolved; verification: explicit | AC-02, AC-30–AC-31: missing feature ownership, activation evidence or a required environment procedure remains visible and unaccepted. |
| encoding | TEST-10 | resolved; verification: explicit | AC-19, AC-30: activation preserves exact source and regression IDs; no renumbering, case folding or display-name substitution. |
| ordering | TEST-10 | resolved; verification: explicit | AC-27, AC-30: catalog/activation enumeration order cannot change ownership or the required set. |
| idempotency | TEST-10 | resolved; verification: explicit | AC-30: repeated activation produces one required declaration per case, preserving earlier regressions and evidence history. |
| concurrency | TEST-10 | resolved; verification: explicit | AC-17, AC-29–AC-30: catalog/input changes during a run invalidate acceptance; the report cannot silently switch its required set. |
| boundary | TEST-02 | resolved; verification: explicit | AC-03, AC-25, AC-30: phase selectors accept only positive integer phases and include all declarations owned at or before that phase, independently of implementation status. Added by manual review. |
| encoding | TEST-03 | resolved; verification: explicit | AC-07, AC-19, AC-28: reproduction records preserve exact scenario IDs and valid Unicode paths/arguments without normalizing meaningful identity. Added by manual review. |
| boundary | TEST-05 | resolved; verification: explicit | AC-11: inherited 1280x720 and 1920x1080 rendered checks use actual captures at the required sizes, with focus/input assertions and readable required controls. Added by manual review. |
| encoding | TEST-06 | resolved; verification: explicit | AC-14, AC-19: opaque profile/actor/peer identities compare exactly and remain distinct under equal display names; valid Unicode profile paths are supported. Added by manual review. |

## Prohibitions (must-NOT)

**Coverage:** 0/0 additional bespoke prohibitions applicable · 0 unresolved

The two-stage adversarial recall and precision pass covered all ten requirements after edge resolution. Candidate concerns included fabricated completion, hidden failures/planned work, removed checks, weakened thresholds, state-setting substitutes, missing visual review, intrusive cleanup, overwritten runs and misleading evidence substitution. These are technical correctness/evidence obligations with explicit negative acceptance criteria above, rather than additional values/safety/ethics prohibitions; they were not duplicated into a new prohibition register.

General credential handling, path/injection protection and artifact privacy are canonical security-review concerns, owned by the security workflow; no duplicate canon prohibition is minted here. No additional non-canonical values/safety/ethics candidate survived precision filtering.

| Prohibition (must-NOT statement) | Requirement | Status | Verification / Reason |
|----------------------------------|-------------|--------|------------------------|
| None additional | TEST-01–TEST-10 | No applicable bespoke prohibition | Technical must-NOT behavior remains binding in AC-03, AC-08, AC-13, AC-16–AC-26 and AC-29–AC-32; no prohibition is deferred. |

## Ambiguity Report

| Dimension | Score | Min | Status | Notes |
|-----------|-------|-----|--------|-------|
| Goal Clarity | 0.97 | 0.75 | Met | Complete traceable foundation verification and inherited certification, without claiming future gameplay. |
| Boundary Clarity | 0.97 | 0.70 | Met | Fast/manual/full split, reports without a viewer, real hosted evidence, and later release environments have explicit owners. |
| Constraint Clarity | 0.94 | 0.65 | Met | Exact coverage scopes/thresholds, controlled input, native execution, two local OSes plus hosted CI, bounded isolation and inherited contracts. |
| Acceptance Criteria | 0.96 | 0.70 | Met | Original 32 criteria/67 resolved edges; discussion adds AC-33–AC-34 for validated reuse and measured runtime. Original scores are historical. |
| **Ambiguity** | **0.038** | **≤ 0.20** | **Met** | 1 − (0.35×0.97 + 0.25×0.97 + 0.20×0.94 + 0.20×0.96). |

The scores record specification clarity, not implementation progress or a measured probability of correctness.

- Initial assessment from roadmap/requirements: goal 0.90, boundaries 0.85, constraints 0.72, acceptance 0.75; ambiguity 0.1785, displayed as 0.18.
- After round 1: goal 0.93, boundaries 0.92, constraints 0.85, acceptance 0.88; ambiguity 0.0985, displayed as 0.10. The creator requested another round.
- After round 2: goal 0.95, boundaries 0.94, constraints 0.90, acceptance 0.93; ambiguity 0.0665, displayed as 0.07. The creator requested another round.
- After round 3: goal 0.97, boundaries 0.97, constraints 0.94, acceptance 0.96; ambiguity 0.038, displayed as 0.04. The creator approved proceeding, then explicitly accepted all proposed edge rules.

## Interview Log

The original requirement interview below is retained as history. The later discussion amendment and current requirements/criteria above supersede its earlier PR breadth and mandatory fresh-full closure timing.

| Round | Perspective | Question summary | Answer / decision locked |
|-------|-------------|------------------|--------------------------|
| 1 | Researcher | When should full Windows/Linux verification run? | Fast checks per PR; full suite on manual request and before phase completion. No nightly requirement selected. |
| 1 | Researcher | How should visual verification combine automation and judgment? | Automate objective checks and have the AI inspect current-run images for appearance/readability. No approved-image baseline required. |
| Gate 1 | Clarity gate | Write now or continue? | One more requirements round. |
| 2 | Researcher / Simplifier | What is the irreducible fast-PR scope? | Strict build, rules/architecture/docs checks and focused checks for affected behavior. |
| 2 | Researcher / Simplifier | Can an unchanged passing rerun erase a failure? | Keep the failure visible; require diagnosis and a clean rerun before acceptance. |
| Gate 2 | Clarity gate | Write now or continue? | One more requirements round. |
| 3 | Boundary Keeper | Is a dedicated visual results viewer required? | Reports, machine-readable results and linked logs/images are sufficient. |
| 3 | Boundary Keeper | What must prove hosted CI works? | An actual successful full Windows/Linux GitHub Actions run on the final source, in addition to required local/VM runs. |
| Gate 3 | Clarity gate | Finish checks and write? | Yes — finish the checks and write SPEC.md. |
| Edge review | Failure / boundary review | Missing/duplicate evidence, exact identities/Unicode, ordering, numeric boundaries, reruns, concurrency and cleanup? | Specify all proposed rules as explicit pass/fail requirements and write SPEC.md. |
| Prohibition review | Adversarial precision pass | Any additional bespoke values/safety/ethics prohibition beyond technical acceptance and canonical security concerns? | No additional applicable candidate; no unresolved prohibition. This is the probe result, not an extra creator answer. |

---

*Phase: 05-ci-and-repeatable-game-testing*

*Spec created: 2026-11-14 (Europe/Paris)*

*Next step: $gsd-plan-phase 5 — use the amended specification and 05-CONTEXT.md. This discussion implements no runner, CI or evidence-reuse capability.*
