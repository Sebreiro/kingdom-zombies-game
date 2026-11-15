# Phase 5: CI and repeatable game testing - Context

**Gathered:** 2026-11-14 (Europe/Paris)
**Amended:** 2026-11-15 (Europe/Paris) — permanent local testing policy, immediate reduced selection, scenario importance/run groups and Phase 11 nightly follow-on.
**Status:** Ready for planning; execution depends on the delivered Phases 1–4 foundations.

<domain>
## Phase Boundary

**Current local policy, effective immediately:** Every phase follows [local testing strategy](../../../docs/workflow/local-testing.md): finish implementation, review/fix code, then use development-OS-only 1280×720 shared game runs without a VM. Select every active Critical case plus all new/changed cases and affected regressions. Collect failures, batch repairs, check failed/blocked/newly affected scenarios, then rerun the entire selected set on the repaired build. Every phase saves reusable scenarios with importance and ordered run groups. Phase 5 must not reinstate local VM/extra-resolution catch-up. Hosted and release procedures retain their separate actual-evidence obligations.

Provide traceable, repeatable verification of the delivered code-rule, playable-scene, LAN and content foundations, shared local/Windows/Linux CI entry points, diagnosis/replay, authentic native-hosted coverage and explicit case/environment accounting. Ordinary PRs are a small build/repository selection. Phase-end tests use importance and dependency impact now; manual full execution remains available. Phase 11 owns scheduled nightly full regression testing.

The creator's 2026-11-15 decisions supersede earlier complete-catalog closure, two-local-OS/VM, both-local-resolution and failed-only carry-forward instructions. The current `05-SPEC.md` and decisions below govern selection, execution and evidence. Existing case identities, coverage thresholds and later gameplay/release ownership remain binding; omitted cases receive no pass credit.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**10 requirements are locked: TEST-01 through TEST-10, as amended by this discussion.** Downstream agents MUST read `.planning/phases/05-ci-and-repeatable-game-testing/05-SPEC.md` before planning or implementing. Its current acceptance criteria are AC-01–AC-38; AC-35–AC-38 cover the latest selection, scenario/run-group and repair-cycle policy. TEST-11 belongs to Phase 11. This is planning authority, not execution evidence.

**In scope (from SPEC.md):**

- A complete readable case/evidence catalog for A-01–A-47, inherited rules, foundation/content checks and regressions, using the approved requirement and phase identities.
- Shared local/CI entry points: basic Windows/Linux PR checks targeting about ten minutes per OS, focused feature groups and manually triggered full verification.
- Automated verification of the delivered rule, menu/movement, LAN and content/inspection foundations, with controlled setup, actual production paths and current rendered evidence.
- A real host-plus-three-client coordinator with independent profiles, bounded waits, per-peer diagnostics and owned-resource cleanup.
- Agent instructions and an exercised select/run/inspect/reproduce/fix/rerun workflow, including diagnosis and preserved attempts when a rerun passes unchanged.
- Human-readable summaries, machine-readable results and directly accessible logs, state observations and images, with AI appearance/readability review where required.
- Actual separate analyzer/production coverage and unchanged inclusive 90% line / 80% branch gates, native-hosted callback measurement, report controls and all eight transferred certification cases.
- Development-OS-only routine local verification and actual hosted Windows/Linux full-workflow compatibility, with explicit selected/omitted scope, authentic coverage and certification evidence. No local VM or extra local resolution is required.
- Case activation rules and separate repeatable future physical-LAN/reference-PC/endurance procedures, without giving unexecuted cases acceptance credit.

**Out of scope (from SPEC.md):**

- Implementing unfinished prerequisite phases as part of this specification — Phases 1–4 keep their approved implementation and acceptance ownership.
- Combat, inventories, camp economy, zombies, campaign outcomes, full saves/session recovery and later UI behavior — Phases 6–9 own these; Phase 5 catalogs their future cases.
- Integrating production art into the playable foundation or implementing Phase 4 layout examples as gameplay — their approved integration owners remain unchanged.
- Certifying complete campaigns, actual mixed-computer/mixed-OS LAN, reference-PC performance/endurance or final release exports now — Phase 10 owns the distinct release evidence.
- A dedicated results dashboard/viewer — the creator selected reports with linked artifacts.
- Approved image baselines and automatic visual-difference gates — the creator selected objective automation plus AI image review.
- Game launches, screenshots/AI review, full LAN, coverage certification and exhaustive checker-defect proofs in ordinary PRs; implementing scheduled nightly runs in Phase 5 (Phase 11 owns them); routinely rerunning unaffected non-Critical cases solely because a phase closes. The final selected-set rendered pass after repairs is required.
- A new wholesale rule/runner replacement, weaker enforcement, wider suppressions, lower coverage targets or deleted regressions — the existing delivered foundation and handoff remain binding.
- Selection of a particular new testing library, runner API, report schema or instrumentation implementation — compatibility proofs and concrete mechanisms belong to research/discussion/planning.

</spec_lock>

<decisions>
## Implementation Decisions

### Basic PR checks and focused feature work

- **D-01:** Ordinary PRs run only the strict build with analyzers, current project/code boundary checks, and documentation/catalog validation on Windows and Linux. No game launch or tiny headless game smoke is selected.
- **D-02:** Target **about ten minutes per OS with dependencies cached**. This replaces the offered three-/five-minute targets. It is an optimization target, not a measured capability, a hard inherited proof timeout, or permission to truncate a selected check. Measure and report warm/cold setup, each group and total elapsed time.
- **D-03:** Expensive deliberate-defect proofs, full behavior/rendered/LAN scenarios, coverage certification and AI screenshot review are outside ordinary PRs. Keep focused and manual full entry points. A basic PR pass must not imply broader acceptance.
- **D-04:** The creator's later basic-PR decision supersedes the earlier answers to run whole affected behavior subsystems, or all behavior groups for shared foundations, on PRs. For **feature verification**, retain whole affected groups and include dependents/shared inputs; broad shared changes can still select all groups. Do not put the expensive work back into basic PRs under the earlier answers.
- **D-05:** Use changed folders as selection inputs, with declared consumers, transitive dependencies and shared build/configuration inputs. An unchanged folder alone does not establish unaffected behavior. Unknown impact is a visible selection blocker; do not silently narrow the scope or automatically substitute a full run for that unresolved decision.
- **D-06:** Implement the whole phase without real game windows, using builds/pure/repository checks as useful. Review the whole phase's code and fix findings before rendered tests. Keep group counts and selection reasons with links to exact selected/omitted case lists. The 2026-11-15 repair cycle in D-24–D-29 governs later game runs.

### Phase completion and reuse

- **D-07:** Account for the phase's declared reduced selection and explicit omissions, rather than demanding a fresh complete-catalog run at every closure. Eligible unchanged evidence may support non-rendered checks, but cannot replace the fresh final rendered selected-set pass after repairs. Broad shared changes can select the whole applicable suite.
- **D-08:** Reuse requires matching relevant code/assets, test/case definitions and required membership, transitive dependencies, relevant tools and required environment. Account for every independently declared required case/environment. Folder-only comparison, implementation labels and discovery counts cannot establish reuse.
- **D-09:** Keep original reports, source revisions, fingerprints, artifacts, failed attempts and invocation identities immutable. Completion reports classify **executed now**, **explicitly reused with matching inputs**, and **missing/invalidated**, linking reuse reasons and original evidence. Never present a reused result as newly executed or relabel an old revision.
- **D-10:** Routine local tests use only the development OS, no VM, at 1280×720. Actual hosted Windows/Linux compatibility remains distinct; do not defer a mandatory local VM or additional local resolution to Phase 5. Preserve rendered input/AI review, supported-display, physical mixed-OS LAN, reference-PC and endurance procedures under their hosted/release owners. Reuse cannot supply an unexecuted environment; coverage and visual reuse validate their complete dependencies.
- **D-11:** A regular test execution with zero selected required cases remains non-success. A distinct evidence-validation operation may need zero new executions only when every selected obligation is eligible for reuse; it cannot bypass the final rendered selected-set pass after repairs. An unresolved later failure prevents acceptance through an older pass. Not-selected and future planned cases are explicit, without pass/skip credit.
- **D-12:** Keep and actually prove the manual full Windows/Linux Actions path; configuration alone does not establish compatibility. The amendment removes redundant renewal of valid evidence, not the full-run capability, required cases or genuine environment proof. The current Phase 1 runner does not implement reuse: Phase 5 must build and prove the validator and its rejection controls before reuse supplies acceptance credit.

### Failure reports and reproduction

- **D-13:** Lead with a failure brief: failed case, expected versus observed behavior, key evidence links and replay command. Keep complete case/run results and identity accessible.
- **D-14:** Give each failed attempt one copyable replay command that uses recorded setup, seed, controlled timing inputs and actions. The replay creates fresh scenario state and a fresh report; it does not overwrite or borrow accepted artifacts from the failed attempt.
- **D-15:** Group failures by a **diagnosed** common cause while retaining each affected case's separate outcome and evidence. Grouping is presentation, not deduplication or an assumption that independent failures share a cause.
- **D-16:** Include short relevant log excerpts and a compact expected-versus-observed state comparison in the brief. Link full logs/observations. Preserve unchanged-rerun failures and diagnosis requirements from the specification.

### Screenshot and review handoff

- **D-17:** Group images by scenario and game-run group, keeping before/after states together with explicit peer labels. Routine local captures use 1280×720 only; separate hosted/release display procedures keep their declared sizes.
- **D-18:** Before/after means checkpoints **within one selected scenario on the current build**, such as menu before Start and scene afterward. It does not mean mandatory pre-change and post-change builds/playthroughs for every small edit. Select relevant scenarios during feature work and preserve complete required review accounting at acceptance.
- **D-19:** Use compact labeled previews linked to original full-resolution captures. Preview size does not establish readability; inspect relevant originals when making that judgment.
- **D-20:** Record short structured notes for each scenario: inspected image identities, resolutions/peers, appearance, readability, defects and review outcome. Missing declared review remains unaccepted.
- **D-21:** Lead the review list with failed objective checks or recorded visual defects, then remaining unreviewed scenarios. Keep every required review visible. A dedicated viewer, approved screenshot baseline or image-difference gate remains outside scope.

### Measured cost and optimization direction

- **D-22:** Expose logical groups, selection dependencies, purpose, artifacts and actual duration. The creator wants an understandable breakdown and fast repeatable feedback, not an unqualified assurance that all testing is fast. Treat current measurements separately from proposed budgets.
- **D-23:** Preserve all existing checks, IDs, exact rules, bounded permissions, genuine positive/negative subjects, real engine/production paths, separate analyzer/production coverage and inclusive 90% line / 80% branch targets. Optimize repeated setup and orchestration rather than deleting acceptance obligations. All eight transferred certification IDs remain required Phase 5 work.

### Local execution and repair cycle — 2026-11-15

- **D-24:** Run local tests only on the development OS; do not start a VM. Launch real game windows only after the whole phase implementation and code review/fixes, and only at 1280×720 for routine local verification. Earlier per-task rendered tracer gates are superseded.
- **D-25:** Batch compatible scenarios into one game run whenever possible, using ordinary menu/session resets and exit checks last. If multiple runs or multiplayer processes are necessary, define ordered compatible groups and record why. Keep independent scenario state and per-case outcomes inside shared processes.
- **D-26:** During a game run, collect errors and continue independent scenarios while usable. Fix the collected defects together afterward. Rerun failed/blocked cases and cases newly affected by repairs, then rerun the **entire selected set** on the repaired build once those checks pass. Repeat this cycle on new failures. An initially clean selected-set run already counts as that pass; do not duplicate it without changes.
- **D-27:** Reduced selection starts **immediately**, before Phase 11. Always include every active Critical case, every new/changed case, and every affected regression regardless of importance. Include dependencies, consumers and shared inputs; unknown impact must be resolved before narrowing selection. Do not silently substitute affected-only or Critical-plus-High as the universal floor.
- **D-28:** Give every case an importance of Critical, High, Normal or Low, with a consequence-based rationale. Critical covers startup/progression blockers, crashes, corruption/loss/duplication and essential control/session integrity. High covers major flows; Normal covers ordinary behavior and recoverable boundaries; Low covers lower-impact nonblocking details. Priority is separate from activation and evidence status. Show unselected cases and reasons without claiming a pass.
- **D-29:** Every phase saves AI-readable scenarios alongside implementation, including phases before Phase 5. Include stable ID/source/requirement/owner, importance/rationale, status/dependencies, setup/seed, ordered production actions, expected results, evidence and replay procedure. Each case declares a persistent game-run group and order, ordinary reset/recovery, roles and fresh-process reason when needed; pure cases declare no window. Each group lists shared setup, ordered cases, resets and exits. Phase 5 integrates and validates these definitions for later regression use.
- **D-30:** Add **Phase 11: Nightly full regression testing**, after Phase 10, to schedule the complete active catalog across all importance levels with existing game-run groups and retained results/artifacts. Keep manual full execution. The schedule and execution environment are later planning decisions; reduced local selection does not wait for nightly implementation. The new TEST-11 requirement belongs to Phase 11, while TEST-01–TEST-10 remain Phase 5-owned.

### Agent's Discretion

No new library, report schema, exact CLI spelling, hashing algorithm, instrumentation method, concurrency level or individual future-group budget was selected. Research/planning must choose and prove these mechanisms within the specification. They should investigate building once per OS/revision, verified dependency/engine caches, compatible test batching, immutable within-run preparation and bounded isolated parallelism. These are candidate implementation techniques, not proven speedups or permission to share mutable subjects/focus.

Retain independent case discovery/accounting, isolated defective subjects, intended diagnostics, bounded waits and ownership cleanup. Do not turn the illustrative seconds-to-one-minute edit-loop suggestion or the earlier 3–5-minute PR proposal into approved targets. The selected PR target is approximately ten minutes; future gameplay, content, LAN and coverage timings remain unmeasured.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read the relevant references before planning or implementing.** Paths are relative to the repository root. The original discussion log is for human audit only and is not planning input.

### Current scope and approved amendments

- `.planning/phases/05-ci-and-repeatable-game-testing/05-SPEC.md` — ten locked requirements as amended, AC-01–AC-38, basic PR scope, importance/dependency selection, grouping, repair cycle, reuse validation and certification obligations; MUST read first.
- `docs/workflow/local-testing.md` — latest all-phase local policy, immediate Critical/new/changed/affected selection, persisted scenarios/run groups, and focused repairs followed by a fresh full selected-set pass; overrides older execution wording.
- `.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md` — all eight transferred IDs, real analyzer/native production measurement, exact 90%/80% gates and complete aggregate meanings under the later reuse policy.
- `.planning/PROJECT.md` — complete v0.1 scope, Godot/C#, reuse boundaries and agent-operated verification.
- `.planning/REQUIREMENTS.md` — original 133 requirements plus TEST-11, A-01–A-47 crosswalk, unchanged original ownership and amended verification lifecycle.
- `.planning/ROADMAP.md` — phase goals/dependencies and feature/release owners. Its older fresh-full timing wording is superseded by this approved discussion and current Phase 5 specification.
- `.planning/STATE.md` — live execution/closure position; this capture neither completes Phase 5 nor advances its implementation.
- `specs/0.1/initial-spec.md` — mandatory game behavior, exclusions, inherited display/input baselines, A-01–A-47 and separate release gates.
- `specs/0.1/assets-and-resources.md` — complete resource obligations, provenance and production/inspection evidence.

### Delivered prerequisite contracts

- `.planning/phases/02-playable-foundation/02-SPEC.md` — real menu/input, movement, focus, camera, lifecycle, display and smoke contracts.
- `.planning/phases/02-playable-foundation/02-CONTEXT.md` — basic foundation presentation, routine implementation discretion and English communication.
- `.planning/phases/03-four-player-lan-foundation/03-SPEC.md` — real host/three guests, identity/authority, readiness, capacity, movement, timing and teardown contracts.
- `.planning/phases/03-four-player-lan-foundation/03-CONTEXT.md` — same basic scene and service/gameplay boundaries.
- `.planning/phases/04-asset-production/04-SPEC.md` — census, required inspection scope, content checks and bounded cosmetic exception.
- `.planning/phases/04-asset-production/04-CONTEXT.md` — selected pixel scale/style, motion, equipment, inspection, UI/text and sound treatment to inspect.
- `.planning/phases/01-code-rules-and-documentation/01-SPEC.md` — enforced rules and original production/native/state contracts; interpret closure through its later exception and the current handoff.
- `.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md` — separate no-restart Phase 1 closure scope; this amendment does not fabricate a complete Linux or individual human pass.
- `.planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md` — still-visible original Linux/graph/human follow-ups.

### Existing implementation and operational policy

- `AGENTS.md` — focused documentation entry point.
- `docs/workflow/ai-collaboration.md` — interactive controls, English session context, platform obligations and approved execution/reuse amendment.
- `docs/workflow/verification.md` — current executable commands, exact evidence handling and future Phase 5 policy; no reuse command exists yet.
- `docs/workflow/setup.md` — pinned, isolated tools, authenticated archives/feed and caches; historical VM preparation is not a current local testing requirement.
- `docs/workflow/gsd.md` — planning workflow and document navigation.
- `docs/code/README.md` — executable code rules, permissions and independent qualitative review.
- `docs/architecture/README.md` — five-project graph, engine, state and service policies.
- `docs/architecture/decisions/0002-boundaries.md` — gameplay/engine/service dependency rationale.
- `docs/architecture/decisions/0003-state-and-engine.md` — controlled state, snapshots and native lifetime.
- `tools/phase1/check.cjs` — exact selectors, independent manifest, current sequential per-case execution and immutable reconciliation.
- `tools/phase1/required-cases.json` — independent cases and acceptance ownership; retain existing identities.
- `tools/phase1/toolchain.json` — pinned inputs, not compatibility proof.
- `tools/phase1/projects.json` — current approved project inventory and reference categories.
- `tools/phase2/required-cases.json` — delivered/in-progress foundation catalog to retain and enrich with importance and persistent run groups.
- `tools/phase2/rendered.cjs` — in-progress shared-session ordering, ordinary resets, per-case failures and Quit-last behavior; not a certified Phase 5 framework.
- `tools/phase2/check.cjs` — existing exact/group/failed-case entry points and evidence integration to extend, without claiming an implemented priority selector.
- `tests/phase1/docs/README.md` — test responsibility and allowed inspection/tool edges.
- `tests/phase1/BuildFixture.cs` — real isolated restore/build controls; repeated setup is an optimization candidate, not a disposable proof.
- `tests/phase1/GraphFixture.cs` — clean baseline plus separately compiled graph mutations.
- `tests/phase1/ConsumerCompilation.cs` — consumer-specific symbols/references and source validation.
- `tests/phase1/ProofProtocol.cs` — per-case identity/evidence conventions.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- The current Phase 1 runner/manifest, C# proofs, pinned toolchain, independent required-case accounting and immutable reports are the baseline to extend. Do not replace them wholesale.
- Existing Core identity, explicit elapsed/random inputs, snapshots and atomic state examples supply controlled-fixture patterns. They are not later gameplay features.
- Godot native proof scenes and lifecycle/resource/thread evidence exist. The 2026-11-15 scout also finds the active Phase 2 production menu/movement implementation and an in-progress shared-session runner with nine rendered scenarios, ordinary resets and Quit last. Its exact/failed-case selectors do not yet establish priority/dependency selection or Phase 5 certification. Extend actual delivered Phase 2–4 foundations; LAN, asset-pack, Actions and measured-coverage completion must still be established.
- Focused documentation/indexes already exist. No codebase maps, packaged spike/sketch findings or matching pending todos were found for this discussion.

### Established Patterns and Observed Cost

- `tools/phase1/check.cjs` currently executes cases sequentially, starting a separate test process for each. `BuildFixture.cs` copies real projects and restores/builds isolated consumers; `GraphFixture.cs` also validates a clean game baseline before mutations. Process startup alone is not the whole bottleneck.
- The retained Windows baseline at revision `59f032cf23340058eec91f12c5d240f585dfabdc`, invocation `b8b5eacc-c46a-42d7-a898-01ad33c1b0a8`, accepted 676 cases. Its recorded command window was **51m 36s** (18:40:20.004Z–19:31:55.692Z). This is historical local execution, not current-source, cold-setup or hosted-CI timing.
- Its report is locally retained at `.phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/summary.json`. This ignored diagnostic path is not a clean-checkout dependency. Group times below sum recorded case-process durations and exclude separately performed clean setup/strict solution bootstrap.

| Existing group | Cases | Recorded case-process time |
| --- | ---: | ---: |
| Rules | 449 | 34m 05s |
| Architecture/graph | 27 | 10m 15s |
| Engine | 22 | 1m 51s |
| State | 33 | 1m 12s |
| Register | 46 | 1m 09s |
| Pins | 27 | 1m 05s |
| Documentation | 36 | 1m 00s |
| Runner | 36 | 55s |

These are exhaustive proof-group costs, not a mandate to place those whole groups in basic PRs. No accepted timing exists yet for the future complete gameplay/content/LAN/coverage groups.

### Integration Points

- Add independently declared logical groups and their dependency/input scopes, with basic PR, focused execution, full execution and completion-validation selections clearly distinguished.
- Extend current per-invocation reconciliation with explicit immutable cross-run references for completion only. Preserve exact case/peer/artifact identity, environment, source/tool provenance, and fail closed on invalid reuse. The existing runner has no such reuse capability yet.
- Automate delivered production commands/input and read-only observations, with isolated profiles, bounded real host-plus-three-guest execution, rendered checkpoints and review records. Keep test-only authority controls absent from release builds.
- Add genuine native-hosted coverage and all transferred aggregate controls through acyclic collection/validation/publication. Reconcile complete expected modules and keep analyzer/production counts separate.
- Implement selected report presentation and reproduction over linked human/machine artifacts without requiring a results application.

</code_context>

<specifics>
## Specific Ideas

- The creator clarified: “PR contains only build and other basic checks - to fit in 10 minutes.”
- Folder-based selection was suggested to avoid testing unchanged work. The selected refinement includes dependencies and validates old results rather than treating an unchanged directory as proof.
- The creator said that “sometimes at the end of the phase we don't need a full retest” and selected reuse of unchanged groups after the exact matching conditions were explained.
- The concern about screenshot cost was answered with before/after checkpoints within one current-build scenario, not automatic old/new-build comparisons for every small feature.
- Earlier report/review preferences remain selected; earlier broad PR behavior selections do not.
- The creator's latest answers were: “Use a smaller selection immediately, before nightly testing exists” and “All Critical cases, plus every new or changed case and affected regressions, regardless of importance.” These supersede the suggestion to wait for Phase 11 before reducing scope.
- The requested game loop is implementation → review/fix → grouped run/log errors → batch repair → failed-case checks → entire selected-set regression pass, with as few launches as practical.

</specifics>

<deferred>
## Deferred Ideas

Phase 11 owns scheduled nightly full regression testing (TEST-11); its exact schedule, runner/environment and failure-publication details need its own planning. This follow-on is added to the roadmap, not implemented in Phase 5. Later feature cases remain planned with their original owners. Physical mixed-OS LAN, supported-display, reference-PC rendering/performance, endurance, complete campaigns and release exports retain distinct acceptance requirements.

</deferred>

---

*Phase: 05-ci-and-repeatable-game-testing*
*Context gathered: 2026-11-14; amended: 2026-11-15 (Europe/Paris)*
