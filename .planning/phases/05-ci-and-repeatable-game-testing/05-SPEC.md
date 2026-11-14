# Phase 5: CI and repeatable game testing — Specification

**Created:** 2026-11-14 (Europe/Paris)
**Ambiguity score:** 0.038 (gate: ≤ 0.20)
**Requirements:** 10 locked — TEST-01 through TEST-10
**Status:** Ready for implementation discussion; execution depends on completion of Phases 1–4.

## Goal

Developers and AI agents can select, execute, diagnose and rerun traceable checks for the delivered foundations locally and in Windows/Linux GitHub Actions, with full final-source certification of the required cases, native-hosted coverage and distinct environment evidence.

## Background

The repository already contains the Godot/C# toolchain, twelve inherited analyzers, Core/state/snapshot and native lifecycle fixtures, an independent case manifest and a runner with exact selection, isolated process execution and fresh evidence reconciliation. At scout time the manifest has 684 declarations: 676 Phase 1 obligations and eight explicitly pending Phase 5 certification cases. The latest recorded complete Windows Phase 1 run accepted 676/676; matching Linux completion and separate human review remain pending in STATE.md. This specification is not new platform acceptance evidence.

Phases 2–4 have specifications and captured implementation context, but their playable menu/movement, LAN scenario and complete content pack/inspection delivery do not yet exist. No .github workflow, complete game acceptance-case catalog, four-process gameplay runner or accepted measured analyzer/production coverage report exists. Phase 5 extends the working verification foundation once those earlier increments are delivered; it neither assumes they passed nor replaces them with test-only demonstrations.

Authority: [project definition](../../PROJECT.md), [approved requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md), [current state](../../STATE.md), [game specification](../../../specs/0.1/initial-spec.md), and the mandatory [Phase 1 certification handoff](05-PHASE1-HANDOFF.md). The [playable foundation](../02-playable-foundation/02-SPEC.md), [LAN foundation](../03-four-player-lan-foundation/03-SPEC.md) and [asset production](../04-asset-production/04-SPEC.md) retain their behavior/content contracts. The current [collaboration policy](../../../docs/workflow/ai-collaboration.md) and [verification reference](../../../docs/workflow/verification.md) govern execution and evidence.

## Requirements

1. **TEST-01 — Traceable complete case catalog:** A repository-stored catalog maps all unchanged source acceptance IDs A-01 through A-47 and foundation, content and regression cases to approved requirements, owning phases, required or planned status, runnable checks or repeatable evidence procedures, environments and required artifacts.
   - Current: tools/phase1/required-cases.json independently declares 684 rule/foundation obligations: 676 Phase 1 cases and eight pending Phase 5 identities. The game specification and REQUIREMENTS.md link A-01–A-47 to requirements, but there is no complete executable/evidence catalog for the game and content foundations.
   - Target: One readable repository catalog covers all 47 source scenarios without renumbering, the existing rule cases, BASE/LAN/ASSET/TEST checks and additional regressions. Each declaration names its approved requirements, owner and acceptance phase, required/planned state, executable check or repeatable evidence procedure, environment and required artifacts. Multiple checks may implement one source scenario; that relationship is explicit.
   - Acceptance: AC-01–AC-03, AC-19, AC-27 and AC-30 pass, including missing/duplicate metadata controls and order-independent catalog coverage.

2. **TEST-02 — Local and CI execution with explicit scope:** Documented local and GitHub Actions entry points run the currently required checks on Windows and Linux, with fast pull-request checks for the strict build, rules, architecture, documentation and affected behavior, full checks on manual request and before phase completion, and actual same-source local, other-OS VM and hosted CI completion evidence.
   - Current: The Phase 1 runner already supports exact case/group/phase selection and isolated evidence. No .github workflow exists at scout time. The recorded Windows 676-case result is Phase 1 evidence; Linux completion is still pending. No gameplay CI execution has occurred.
   - Target: The same documented check entry points serve local use and Windows/Linux GitHub Actions. Pull requests run the strict build, rules, architecture, documentation and affected-behavior checks. Full runs are manually available and mandatory before phase completion. Phase 5 completion includes actual final-source clean local/other-OS VM runs and a real passing full Windows/Linux Actions run; configuration or prepared environments alone are insufficient.
   - Acceptance: AC-03–AC-06, AC-25 and AC-32 pass; the fast result identifies exactly what ran and the full result accounts for the complete required scope.

3. **TEST-03 — Agent-operated diagnosis and recovery:** An AI agent can follow a repository-documented select, run, inspect, reproduce, fix and rerun procedure using recorded scenario inputs and current-run evidence, while preserving failed attempts and requiring diagnosis plus a clean rerun before acceptance.
   - Current: docs/workflow/verification.md describes Phase 1 selection, immutable reports and failure handling. It does not yet provide the complete game/menu/LAN/content select-to-reproduction workflow.
   - Target: An agent can select relevant checks, execute them, inspect the current report and its logs/state/images, reproduce recorded inputs, diagnose and repair a failure, rerun affected checks, and obtain full verification when required. A pass after an unchanged rerun does not erase the first failure or remove the diagnosis requirement.
   - Acceptance: AC-07–AC-08, AC-19 and AC-28–AC-29 pass in an actual current-run failure/reproduction exercise.

4. **TEST-04 — Controlled checks of real behavior:** Gameplay-rule checks execute the delivered production behavior with controlled setup, explicit time and random inputs where applicable, and repeatable assertions of authoritative state.
   - Current: Core example state, snapshot, elapsed-input and replay-random implementations and extensive Phase 1 proofs exist. The menu/movement, LAN and content production described by Phases 2–4 are not implemented at this scout.
   - Target: Automated fixtures exercise the delivered production rules, with controlled initial state and explicit time/randomness where applicable. Scenario setup is separated from production actions and read-only observations. Inputs and expected logical outcomes are recorded well enough to replay; no future combat, inventory or campaign behavior is invented for this phase.
   - Acceptance: AC-09–AC-10, AC-13 and AC-28–AC-29 pass, including relevant empty/single/multiple fixtures and controlled-input boundaries.

5. **TEST-05 — Rendered production-input and visual evidence:** Rendered menu, keyboard, mouse, focus and scene checks exercise production input paths at the inherited supported resolutions, automate objective assertions and provide current-run images for AI appearance and readability review without requiring approved image baselines.
   - Current: Native Godot construction/lifetime proof scenes exist. There is no playable menu/input scene, content inspection pack, game visual runner or approved screenshot baseline in the current implementation.
   - Target: After Phases 2–4 deliver their foundations, repeatable rendered checks exercise the real menu, keyboard/mouse, focus, movement/scene transitions and relevant inspection fixtures. Objective state/input/resource checks are automated. The AI inspects current-run images for appearance and readability and records its conclusions and defects. A visual pass requires that review where declared; approved image baselines and automatic image comparison are not required.
   - Acceptance: AC-11–AC-13, AC-16 and AC-28–AC-29 pass at the inherited required display sizes; headless results do not establish a rendered or visual-review pass.

6. **TEST-06 — Four real isolated LAN processes:** One real host and three real guest game processes complete the delivered LAN foundation scenarios with independent profiles, production commands, bounded waits, per-peer results and logs, and reliable cleanup of owned processes.
   - Current: The project has multiplayer abstraction boundaries and a locked Phase 3 specification, but no implemented host/join/movement scenario or four-process test coordinator.
   - Target: A real host and three guest processes use independent profiles and the actual delivered LAN/authority path. Each controls its own actor and observes the inherited synchronized state. Scenarios have bounded readiness/action/termination waits, per-peer results and logs, and cleanup limited to owned resources. The full scenario runs on each required local/VM/CI operating system.
   - Acceptance: AC-14–AC-16, AC-19 and AC-27–AC-29 pass, including missing/duplicate peers, capacity, repeat-run and overlapping-run controls.

7. **TEST-07 — Fresh inspectable reports and coverage evidence:** Each run publishes nonempty results, process logs, state observations and required rendered images reconciled by exact case, peer, source, tool, platform and invocation identity, together with separate real analyzer and production coverage counts.
   - Current: Phase 1 publishes per-invocation summaries, per-case results, real subject/process logs and source/tool/platform fingerprints. There is no integrated game/image/peer report or accepted measured analyzer/production coverage report.
   - Target: Concise human-readable summaries and machine-readable results link exact cases and peers to fresh logs, observations, decoded rendered images and measured coverage. Invocation, source revision/input fingerprint, actual tools/platform and artifact identities are reconciled. Analyzer and production scopes retain separate covered/total line and branch counts. Reports and linked artifacts are sufficient; a dedicated results viewer is outside scope.
   - Acceptance: AC-17–AC-19, AC-21–AC-24 and AC-27–AC-29 pass; missing expected modules and mixed/stale evidence cannot pass.

8. **TEST-08 — Visible failure for incomplete verification:** Verification reports a visible non-success outcome for missing dependencies, bad runner discovery or filters, zero required-case execution, skipped required cases, crashes, timeouts, missing peers, stale or missing evidence and invalid measured-coverage reports.
   - Current: Existing runner/architecture/documentation/native tests reject many deliberate failures. The game/peer/image, coverage-report and transferred aggregate certification controls remain incomplete.
   - Target: Every selected required obligation is accounted for against an independent declaration. Missing prerequisites, invalid selection/discovery, skipped cases, actual failures, crash, timeout, absent peer completion, bad/missing/stale artifacts and invalid coverage yield a non-success result with visible diagnostics. Blocked obligations remain separate from passed cases; unrelated failures remain inspectable.
   - Acceptance: AC-03, AC-15, AC-18–AC-20, AC-23 and AC-29 pass with actual deliberate defective subjects and separately verified clean controls.

9. **TEST-09 — Measured coverage and completed inherited certification:** Deliberate positive and negative trials prove that runner and report checks and the inclusive 90% line and 80% branch coverage gates accept genuine sufficient evidence and reject insufficient or invalid evidence for the separate real analyzer and applicable handwritten production scopes, while completing all eight inherited Phase 5 aggregate and coverage identities without weakening existing checks.
   - Current: Twelve analyzers and native/Core proofs exist, but real measured 90%/80% coverage and all eight transferred aggregate/coverage identities are pending. The handoff explicitly preserves those IDs and their original meaning.
   - Target: Real analyzer code and applicable handwritten Core, ports and Godot managed production code separately meet inclusive 90% line and 80% branch coverage. A native-hosted engine callback and nonzero branch evidence are actually measured. Exact denominator/exclusion/report validation, meaningful positive/negative controls and all eight transferred declarations are completed without lowering existing rules or replacing working proofs.
   - Acceptance: AC-20–AC-29 pass, including exact-threshold neighbors, authentic native-hosted collection, all transferred IDs and fresh complete same-source Windows/Linux execution.

10. **TEST-10 — Feature activation and distinct evidence obligations:** Feature delivery activates its owning planned cases as required, preserves earlier regressions and records rendered input, real mixed-OS LAN, reference-PC and endurance evidence as separate obligations with repeatable procedures and accurate completion status.
   - Current: Phase ownership and future-case lifecycle rules are documented; the Phase 1 selector separates its eight later declarations. There is no complete game-case activation procedure or release-environment evidence catalog.
   - Target: The full release catalog exists now, with unimplemented feature cases visibly planned and owned. Feature delivery activates its cases and retains earlier regressions. Rendered input, four local processes, physical mixed-OS LAN, reference-PC performance and endurance have distinct procedures, environments, artifacts and completion records. Phase 5 supplies the procedures/catalog without claiming later gameplay or release runs have passed.
   - Acceptance: AC-01–AC-03, AC-19, AC-25 and AC-27–AC-32 pass; planned or partial evidence cannot conceal a currently required obligation.

## Boundaries

**In scope:**

- A complete readable case/evidence catalog for A-01–A-47, inherited rules, foundation/content checks and regressions, using the approved requirement and phase identities.
- Documented shared local/CI selection and execution entry points, fast Windows/Linux PR workflows, and manually triggered full Windows/Linux verification.
- Automated verification of the delivered rule, menu/movement, LAN and content/inspection foundations, with controlled setup, actual production paths and current rendered evidence.
- A real host-plus-three-client coordinator with independent profiles, bounded waits, per-peer diagnostics and owned-resource cleanup.
- Agent instructions and an exercised select/run/inspect/reproduce/fix/rerun workflow, including diagnosis and preserved attempts when a rerun passes unchanged.
- Human-readable summaries, machine-readable results and directly accessible logs, state observations and images, with AI appearance/readability review where required.
- Actual separate analyzer/production coverage and unchanged inclusive 90% line / 80% branch gates, native-hosted callback measurement, report controls and all eight transferred certification cases.
- Fresh complete final-source local development-OS, other-OS local VM and hosted Windows/Linux CI evidence for the Phase 5 required scope.
- Case activation rules and separate repeatable future physical-LAN/reference-PC/endurance procedures, without giving unexecuted cases acceptance credit.

**Out of scope:**

- Implementing unfinished prerequisite phases as part of this specification — Phases 1–4 keep their approved implementation and acceptance ownership.
- Combat, inventories, camp economy, zombies, campaign outcomes, full saves/session recovery and later UI behavior — Phases 6–9 own these; Phase 5 catalogs their future cases.
- Integrating production art into the playable foundation or implementing Phase 4 layout examples as gameplay — their approved integration owners remain unchanged.
- Certifying complete campaigns, actual mixed-computer/mixed-OS LAN, reference-PC performance/endurance or final release exports now — Phase 10 owns the distinct release evidence.
- A dedicated results dashboard/viewer — the creator selected reports with linked artifacts.
- Approved image baselines and automatic visual-difference gates — the creator selected objective automation plus AI image review.
- Mandatory full-suite execution on every PR/push or on a nightly schedule — fast PR checks and manual/pre-completion full execution were selected; no scheduled full run was requested.
- A new wholesale rule/runner replacement, weaker enforcement, wider suppressions, lower coverage targets or deleted regressions — the existing delivered foundation and handoff remain binding.
- Selection of a particular new testing library, runner API, report schema or instrumentation implementation — compatibility proofs and concrete mechanisms belong to research/discussion/planning.

## Constraints

- Preserve Godot/C#, the existing project/engine/service boundaries, compatible proven pins/locks, all twelve analyzers, exact rule thresholds, bounded generated/suppression permissions and existing proofs. A candidate test adapter is not proven by package availability or successful compilation.
- Prove actual discovery, subject execution, process exits, evidence and deliberate failure behavior for the selected runner. An incompatible candidate may be replaced by a bounded repository-owned harness while retaining every acceptance obligation.
- Test drivers use production input/commands; initialization and read-only observation are distinct. Test-only authority controls are excluded from release builds. Shared fixture setup must not become substitute gameplay.
- Use relevant focused checks during development. At phase completion run the complete required scope on the development OS and a local VM with the other supported OS, with hardware acceleration when available. In addition, Phase 5 requires an actual passing full Windows/Linux Actions run on the final source. All required environments remain visible if blocked.
- Preserve the Phase 2/3 display, input, authority, movement, timing and lifecycle contracts and the Phase 4 census, presentation decisions and bounded cosmetic exception. Phase 5 adds cross-platform verification of the delivered foundations; it does not convert future asset/gameplay bindings into passed behavior.
- Coverage scopes are separate: real analyzer code, and applicable handwritten Core/ports/native-hosted managed Godot production code. Classify genuinely non-executable modules and legitimate generated exclusions explicitly; never omit expected unmeasured handwritten modules.
- Use exact inclusive threshold arithmetic and authenticated same-source evidence. Collection may instrument isolated copied assemblies/PDBs if necessary, preserving originals. Functional execution, collection, validation and publication must be acyclic.
- Every scenario/process has explicit bounded waits and owned cleanup. No fixed PR runtime or new performance/latency target was selected. Preserve inherited numeric limits rather than inventing new gameplay balance or a weaker test-only tolerance.
- Cases are selected from independent declarations and phase ownership, not from discovery, implementation labels or prior outcomes. Future planned work is never passed, skipped for credit or allowed to hide a currently required case.
- Reports retain failure history, source/run identity and the precise selected scope. Technical failures and unreviewed required visuals cannot be relabeled as accepted.
- This is a planning-only change. It does not advance STATE.md, complete a requirement, start the VM, run the game or establish CI/coverage acceptance.

## Acceptance Criteria

- [ ] **AC-01:** The catalog accounts for exactly the source identities A-01 through A-47, with no missing, duplicated or renumbered source ID, and also includes the existing rule checks plus BASE, LAN, ASSET, TEST and regression coverage. Additional cases use their own identities rather than altering the 47 source IDs.

- [ ] **AC-02:** Every case has a readable scenario and valid, nonblank requirement/owner/acceptance-phase/status/check-or-procedure/environment/artifact references. Invalid, duplicate or dangling metadata is rejected. A source umbrella scenario may link multiple checks with explicit ownership and evidence.

- [ ] **AC-03:** Selection lists the independently declared expected set before acceptance. Exact case, group, fast and full/current-phase selections are documented. Phase selectors accept positive integers and include every declaration with acceptancePhase at or below the selected phase, irrespective of implementation status. Unknown, conflicting or wrong filters, invalid phases and zero required-case execution cannot return a successful verification result.

- [ ] **AC-04:** Windows/Linux pull-request workflows execute a strict build and the applicable rule, architecture and documentation checks plus focused checks for affected delivered behavior. The affected-case selection rule is documented and auditable; uncertain impact selects a sufficient broader set or reports a blocker. The report names its selected cases and omissions. Fast means this defined scope; no fixed runtime budget was selected.

- [ ] **AC-05:** A manual full GitHub Actions run and the documented local full entry point execute the same declared currently required scope on Windows and Linux, including inherited rules/native proofs, controlled fixtures, rendered production-input checks, delivered content checks, four real LAN processes and coverage/certification. CI configuration alone does not satisfy execution.

- [ ] **AC-06:** Before Phase 5 completion, preserve clean full results for the same final implementation from the development OS, a local VM running the other supported OS, and an actual full Windows/Linux GitHub Actions run. Record actual platform/kernel/architecture, tools, revision and input identity. Use VM hardware acceleration when available and record the actual mode. A source/contract repair renews affected checks and final complete evidence on the final source. No donor folders, Downloads cache or previous build/proof output is required for clean acceptance.

- [ ] **AC-07:** An agent follows the repository procedure through one actual controlled current-run failure: find its declaration, select/run it, locate the exact report/log/state/image evidence, reproduce the recorded setup/seed/action sequence, diagnose and repair the cause, and obtain the required fresh reruns. Missing reproduction information or required artifacts leaves the exercise unaccepted.

- [ ] **AC-08:** If a failure passes on an unchanged rerun, retain both attempts and a diagnosis before acceptance. A later successful attempt has a fresh invocation identity and the appropriate clean required results; it cannot replace, relabel or hide the original failure. An automatic retry by itself cannot turn the failed attempt into a pass.

- [ ] **AC-09:** Controlled fixtures execute actual delivered production behavior and assert authoritative outcomes for relevant empty, single and multiple-state inputs, equal identities, repeated actions and meaningful event ordering. Initial scenario setup may be controlled, but gameplay actions follow the production path and observations do not mutate it. Independent scenarios do not share mutable clocks, random streams or setup.

- [ ] **AC-10:** Elapsed/random-input fixture boundaries follow their actual units and range contracts, including zero, nearest valid/invalid neighbors, insufficient/negative values where applicable and overflow/fraction rejection. Repeated setup/seed/time/action inputs produce the specified logical state. This does not require bit-identical pixels, process timings or network scheduling.

- [ ] **AC-11:** Rendered checks use the real menu, keyboard and mouse routes and verify scene entry/exit/reentry, movement/facing and focus handling against the inherited Phase 2/3 contracts. Obtain actual before/after captures at 1280x720 and 1920x1080 for the required cases. Text-entry/control focus does not leak gameplay input, and repeated entry does not add duplicate handlers/actions. Missing or undecodable images and absent objective assertions cannot pass.

- [ ] **AC-12:** Delivered content/inspection fixtures produce objective resource/state results and the required current-run images. The AI records which images it inspected, appearance/readability findings and any defects, linked to case/run/source identity. Preserve the Phase 4 resource census, visual decisions and explicit minor-cosmetic exception; no screenshot baseline or image-diff gate is required. Missing declared visual review stays unaccepted; headless execution cannot supply it.

- [ ] **AC-13:** Test drivers exercise the production input/command path; read-only observations cannot write the expected gameplay outcome. Deliberate invalid-command tests may address the real command boundary, but do not certify keyboard/mouse rendering by themselves. Test-only authority mutation controls are absent from release builds.

- [ ] **AC-14:** The full LAN scenario launches one real host and three real guests with distinct profiles and actual network/authority behavior. Every peer has its own required results and logs; each moves only its actor and observes the coherent production baseline/movement/facing. Preserve all Phase 3 discovery/manual-join, readiness, identity, capacity, timing/tolerance, disconnect and teardown acceptance contracts; mocks or synthetic peer records cannot satisfy this scenario.

- [ ] **AC-15:** Capacity and runner controls distinguish legal one-to-four-player production sessions from the required four-process scenario. A missing host/guest, duplicate profile/actor claim, fifth admitted participant, absent peer result or expired wait prevents the corresponding acceptance. Deadlines are explicit, positive, bounded and checked without numeric coercion or overflow; missing confirmed completion by the deadline is non-success. Equal display names do not merge opaque identities.

- [ ] **AC-16:** Repeated and overlapping runs preserve isolated profiles, user data, session endpoints, logs and artifacts. Conflicting access to input focus is handled explicitly rather than sending actions to another window. Cleanup verifies ownership and releases the run's processes/descendants/resources, without terminating unrelated games, another run or the preserved local VM. A rerun has no stale actors, slots, callbacks or peer results.

- [ ] **AC-17:** Every invocation publishes an immutable report identity and records its revision, input fingerprint, actual tool/platform identities, selected scope, commands/exits, case/peer outcomes and artifact references/hashes. Required logs/observations/images are current and nonempty; images decode and demonstrate the declared capture. Changed tested inputs, stale artifacts and evidence from another invocation cannot establish acceptance.

- [ ] **AC-18:** Reconcile exact expected case, peer and artifact identities against actual fresh discovery/execution/results, rather than accepting exit zero or discovered counts alone. Reject missing, duplicate, unexpected, skipped, stale or mismatched results. Shared artifacts are declared explicitly and never replace distinct case/peer results. Report independent failures and blocked obligations separately; blocked/skipped/pending cases receive no pass credit.

- [ ] **AC-19:** Catalog/report/reproduction metadata is valid UTF-8. Stable case/profile/actor/peer/module identities and portable path segments retain exact spelling, case and Unicode identity; display names do not substitute for them. Valid Unicode workspace/profile paths work. Malformed encoding, wrong-case or silently normalized identity references fail explicitly.

- [ ] **AC-20:** Deliberate trials visibly reject missing/incompatible tools, a bad runner, zero discovery, a wrong filter, skipped required cases, actual assertion failures, crash, timeout, cancellation, missing peer completion, missing/empty logs or state observations or rendered images, stale/duplicate/unknown results and wrong run/source/tool/platform identities. Each negative control establishes its prerequisites and reaches the intended defective subject; setup failure alone is not the expected rejection. Matching clean controls pass.

- [ ] **AC-21:** Collect genuine separate analyzer and handwritten production line/branch evidence. The production scope includes applicable Core, multiplayer ports and native-hosted Godot managed code; measure an actual engine callback and nonzero branch behavior. Inventory expected modules explicitly. A missing expected handwritten module is a failure; modules with no executable sequence points and legitimate generated exclusions are classified with reasons, not silently removed.

- [ ] **AC-22:** Each of the analyzer and production scopes independently meets inclusive 90% line coverage and 80% branch coverage. Compare exact covered/total counts with overflow-safe integer arithmetic, not displayed rounded percentages. Exact-threshold inputs pass; the nearest representable below-threshold inputs fail. Validate nonnegative integral counts, covered <= total and legitimate denominators; an expected executable scope cannot pass with zero/absent measurement.

- [ ] **AC-23:** Coverage gates reject absent, empty, malformed, duplicate, stale, wrong-source/scope/module and below-limit reports, invalid/fractional/overflowing counts and unexplained omissions/exclusions. Valid module/report-row permutations preserve exact totals and verdicts. The analyzer and production scopes cannot compensate for each other.

- [ ] **AC-24:** Functional execution, coverage collection, validation and publication form an acyclic process; collection/certification checks do not recursively invoke the full suite. Retain source identity and original production binaries; isolated copied assemblies/PDBs may be instrumented if needed to measure native hosting. Do not change production behavior or omit unmeasured engine code to raise coverage. Fresh certification retains all failed and successful measurement attempts.

- [ ] **AC-25:** Activate and obtain actual passing evidence for ac-02.complete, ac-06.complete, ac-07.complete, ac-09.complete, R-49.positive, R-49.negative, R-82.positive and R-82.negative with their original exhaustive meanings. Retain existing implemented case identities and all applicable prior regressions. The current 684/676 counts are a scout snapshot, not a cap on later required declarations; new cases are declared before acceptance.

- [ ] **AC-26:** The aggregate register, enforcement and numeric/report certifications reconcile real functional mechanisms and the actual measured coverage evidence, including native/state evidence. A precoverage binding, existing Phase 1 pass, empty wrapper or successful subprocess exit cannot substitute for the transferred aggregate acceptance.

- [ ] **AC-27:** Permuting catalog entries, semantically unordered fixture inputs, peer/result arrival order or coverage/module rows does not alter set membership, exact totals or acceptance. Duplicate identity ties are rejected. Scenario command/focus/readiness/revision ordering that affects actual production behavior remains explicit and is not permuted away.

- [ ] **AC-28:** Repeating a scenario from its recorded setup, seed, controlled time and action sequence reproduces its specified logical outcomes and inherited tolerances, with a fresh invocation and clean scenario state. Repeats do not overwrite reports, reuse accepted artifacts, create extra actors or change the reference behavior. Pixel bytes, timestamps and process identities need not match.

- [ ] **AC-29:** Concurrent execution, interruption, crash, timeout and cancellation cannot mix cases, peers, profiles, resources or artifacts across invocations. Preserve available diagnostics and a non-success interrupted outcome. Partial publication or an input/catalog change during execution cannot produce an accepted final report. Owned resources are cleaned up; historical reports remain immutable.

- [ ] **AC-30:** The catalog distinguishes required cases from future planned cases with explicit owners and acceptance phases. Feature delivery activates corresponding checks/procedures alongside the feature and retains earlier regressions. Repeated activation does not duplicate cases, change source IDs or lose history. Planned cases remain outside the current required denominator without pass/skip credit; implementation labels cannot hide a declaration already required by the selected scope.

- [ ] **AC-31:** Store separate repeatable procedures, environments, expected artifacts and completion records for headless/native state, rendered input and AI review, four local processes, actual mixed-computer/mixed-OS LAN, reference-PC rendering performance and saved/resumed endurance. Later feature/release records remain planned under their approved owners until actually executed. Local/hosted automation does not certify the distinct physical-LAN, hardware or endurance obligations.

- [ ] **AC-32:** Summaries distinguish a passing fast selection from a complete full run and identify deferred/planned cases and still-missing environments/reviews. Missing required tools, environments, artifacts or review yields explicit non-success/blocker status, never a broader completion claim. A dedicated results viewer, mandatory nightly full runs and approved screenshot baselines are not required.

## Edge Coverage

**Coverage:** 67/67 applicable edges resolved · 0 unresolved
**Verification:** 67 explicit · 0 backstop · 0 dismissed

The compiled edge probe ran over the exact ten requirement statements above with authored data/behavior shapes. Its deterministic floor proposed 63 edges, all resolved with explicit criteria. Manual review added four missed categories: phase-selector boundaries, reproduction text/paths, rendered-size boundaries and LAN identity encoding. The creator accepted the complete grouped edge proposals after the requirements gate. The table records every engine-proposed pair plus those four additions; no proposed edge was silently dropped.

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
| Acceptance Criteria | 0.96 | 0.70 | Met | 32 pass/fail criteria and 67 explicitly resolved edges; no unresolved dimensions or planner assumptions. |
| **Ambiguity** | **0.038** | **≤ 0.20** | **Met** | 1 − (0.35×0.97 + 0.25×0.97 + 0.20×0.94 + 0.20×0.96). |

The scores record specification clarity, not implementation progress or a measured probability of correctness.

- Initial assessment from roadmap/requirements: goal 0.90, boundaries 0.85, constraints 0.72, acceptance 0.75; ambiguity 0.1785, displayed as 0.18.
- After round 1: goal 0.93, boundaries 0.92, constraints 0.85, acceptance 0.88; ambiguity 0.0985, displayed as 0.10. The creator requested another round.
- After round 2: goal 0.95, boundaries 0.94, constraints 0.90, acceptance 0.93; ambiguity 0.0665, displayed as 0.07. The creator requested another round.
- After round 3: goal 0.97, boundaries 0.97, constraints 0.94, acceptance 0.96; ambiguity 0.038, displayed as 0.04. The creator approved proceeding, then explicitly accepted all proposed edge rules.

## Interview Log

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

*Next step: $gsd-discuss-phase 5 — implementation decisions for the locked scope above.*
