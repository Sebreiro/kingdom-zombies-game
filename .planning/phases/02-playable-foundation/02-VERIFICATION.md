---
phase: 02-playable-foundation
verified: 2026-11-15T12:35:02Z
status: passed
score: 14/14 must-haves verified
covered_files: [".gitignore", ".planning/phases/02-playable-foundation/02-01-PLAN.md", ".planning/phases/02-playable-foundation/02-01-SUMMARY.md", ".planning/phases/02-playable-foundation/02-02-PLAN.md", ".planning/phases/02-playable-foundation/02-02-SUMMARY.md", ".planning/phases/02-playable-foundation/02-03-PLAN.md", ".planning/phases/02-playable-foundation/02-03-SUMMARY.md", ".planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json", ".planning/phases/02-playable-foundation/02-REVIEW-FIX.md", ".planning/phases/02-playable-foundation/02-REVIEW.md", ".planning/phases/02-playable-foundation/02-SCENARIOS.md", ".planning/phases/02-playable-foundation/02-TESTING-AMENDMENT.md", ".planning/phases/02-playable-foundation/02-VALIDATION.md", ".planning/phases/02-playable-foundation/02-VISUAL-REVIEW.md", "docs/code/engine-generated-spans.json", "docs/code/exemptions.json", "docs/code/generated-callsite-adaptations.json", "docs/code/register.json", "docs/code/visibility-exemptions.json", "docs/workflow/ai-collaboration.md", "docs/workflow/foundation-checks.md", "docs/workflow/local-testing.md", "docs/workflow/pre-phase5-testing.md", "docs/workflow/setup.md", "docs/workflow/verification.md", "src/core/FoundationActorState.cs", "src/core/FoundationMovementCommand.cs", "src/core/FoundationSession.cs", "src/godot-game/GeneratedCallSiteAdapters.cs", "src/godot-game/foundation/Foundation.tscn", "src/godot-game/foundation/FoundationInput.cs", "src/godot-game/foundation/FoundationRoot.cs", "src/godot-game/foundation/FoundationView.cs", "src/godot-game/foundation/FoundationWorld.tscn", "src/godot-game/project.godot", "tests/phase1/ArchitecturePolicy.cs", "tests/phase1/BuildFixture.cs", "tests/phase1/ConsumerCompilation.cs", "tests/phase1/EngineAccessPolicy.cs", "tests/phase1/EngineGeneratedTests.cs", "tests/phase1/ExemptionValidator.cs", "tests/phase1/FoundationLifecycleTests.cs", "tests/phase1/FoundationMovementTests.cs", "tests/phase1/FoundationPermissionTests.cs", "tests/phase1/FoundationRunnerTests.cs", "tests/phase1/GeneratedCallSiteAdapterTests.cs", "tests/phase1/GodotCallSiteAdapterTests.cs", "tests/phase1/GraphFixture.cs", "tests/phase1/GraphTests.cs", "tests/phase1/KingdomZombies.Phase1.Tests.csproj", "tests/phase1/SuppressionTests.cs", "tools/code-rules/EngineGeneratedSyntax.cs", "tools/phase2/check.cjs", "tools/phase2/evidence.cjs", "tools/phase2/linux-input.sh", "tools/phase2/rendered.cjs", "tools/phase2/required-cases.json", "tools/phase2/runner-fixture.cjs", "tools/phase2/windows-input.ps1"]
covered_digest: "v4:sha256:b922ed87fe668c626c1100062629303645af1a7582f3972f1ec59266cfbcdca7"
behavior_unverified: 0
overrides_applied: 0
human_verification: []
decision_coverage:
  honored: 3
  total: 3
  not_honored: []
---

# Phase 2: Playable foundation Verification Report

**Phase Goal:** As a player, I want to start a small scene from a visible menu and move my character left and right using normal input, so that I can explore the game's playable foundation.

**Verified:** 2026-11-15T12:35:02Z
**Status:** passed
**Re-verification:** No — initial canonical verification.

The creator's [testing amendment](02-TESTING-AMENDMENT.md) and current [local testing strategy](../../../docs/workflow/local-testing.md) govern the contract: whole implementation and code review/fixes before one grouped development-OS 1280×720 rendered selection. All 16 new active cases were selected; none was omitted. The initial nine-case rendered selection passed without repairs, so it also supplies the final selected-set pass. The separate compiler-proof timeout repair preceded that rendered run. No additional game window, VM, build or passing test group was run for this verification.

## User Flow Coverage

| Step | Expected | Evidence | Status |
| --- | --- | --- | --- |
| Launch | Ordinary main scene displays usable Start/Quit menu | `project.godot` points to `Foundation.tscn`; actual launch-menu observations/images in native run below | VERIFIED |
| Start | Mouse or keyboard activation enters one fresh playable scene | `FoundationRoot.Start`, actual menu/retry and lifecycle scenarios; real actor-group counts | VERIFIED |
| Walk and explore | A/D and arrows move the identified player through scenery and camera | Core authority, production input/projection, signed measured walks, building crossing and endpoint observations | VERIFIED |
| Return and restart | Escape removes play; new Start has fresh identity and cleared input | Root closes authority/removes world; actual lifecycle/focus scenarios | VERIFIED |
| Quit | Ordinary menu action exits cleanly | Last scenario used keyboard Space; owned PID 17240 exited zero | VERIFIED |
| Outcome | Player can explore the basic playable foundation | Complete nine-scenario native selection, required images and recorded appearance review | VERIFIED |

## Goal Achievement

### Observable Truths

Roadmap criteria are retained first. The equivalent plan launch-menu truth is deduplicated; other plan truths add the details below. Environment wording follows the explicit amendment rather than obsolete VM/two-resolution clauses.

| # | Truth | Status | Evidence |
| --- | --- | --- | --- |
| 1 | Launch displays a usable menu and Start enters the small scene | VERIFIED | Ordinary launch configuration, authored menu, production Start; `rendered.menu` and `rendered.tracer` pass |
| 2 | Representative assets and a character respond through production controls and camera | VERIFIED | Authored world, sole Core state, read-only projection; directions/distance/world pass and actual appearance receipt |
| 3 | Repeatable checks expose current results and fail for missing behavior/images | VERIFIED | Independent 16-case catalog, coordinator, reconciliation and runner integrity/process/repeatability passes |
| 4 | Real Start, movement, Escape and Quit traverse production composition | VERIFIED | Native input acknowledgements and nonce-bound observations; shared native run passes 9/9, clean final Quit |
| 5 | Position changes through identified Core commands; view projects that state | VERIFIED | `FoundationSession.Advance` validates ownership/time/direction; exact authority assertions and native identity observations |
| 6 | Strict permissions continue rejecting adjacent/forged/native/generated misuse | VERIFIED | `FoundationPermissionTests` invokes real source guards, visibility, authenticated origin, generated boundaries and member-collision checks; focused repaired result passes |
| 7 | A/D/arrows use one actor at 3u/s; aliases/repeats/opposites/releases behave correctly | VERIFIED | Production `FoundationInput.Direction`; named pure authority/input proofs and actual eight-combination simulation-time intervals |
| 8 | Ground, scenery crossing, endpoints and camera keep the actor visible | VERIFIED | Core clamps ±59000, fixed ground, 16px/u view and clamped smoothing; actual world observations and reviewed endpoint/overlap PNGs |
| 9 | Escape/repeat/focus/reentry cannot retain old actors, commands or held input | VERIFIED | Root closes old session and removes world; input release barrier; named lifecycle proof plus native lifecycle/focus cases |
| 10 | Native menu, English copy, focus and recoverable entry feedback are usable | VERIFIED | Actual mouse/keyboard navigation, pressed/focus captures, scoped entry failure/retry and reviewed 1280×720 images |
| 11 | Exact current result/image sets reject zero/missing/duplicate/stale/interrupted evidence | VERIFIED | Reconciliation checks identities, exact membership, artifact hashes, decoded PNGs and state bindings; integrity proof passes |
| 12 | Missing tools/crashes/nonzero exits/deadlines fail with owned cleanup | VERIFIED | Shell-free process helper/deadline tree cleanup; real failing-child and descendant fixtures pass intended rejection checks |
| 13 | Fresh evidence, result order and desktop conflict handling preserve honest outcomes | VERIFIED | Fresh nonce/directories, latest-result aggregation and explicit repair linkage; repeatability proof covers order, immutable originals, concurrent lock refusal and cleanup outcomes |
| 14 | Complete amended Phase 2 selection executes with actual image review | VERIFIED | Seven accepted pure/compiled cases plus nine native cases; independent completion validation accepted original report/hash and 50 required review bindings |

**Score:** 14/14 truths verified; 0 present-but-behavior-unverified. No prohibition items or accepted must-have overrides were declared.

### Required Artifacts

| Artifact | Expected implementation and wiring | Status |
| --- | --- | --- |
| `src/core/FoundationSession.cs`, command and actor records | Validated immutable identified state; sole checked movement authority consumed by Root | VERIFIED |
| `src/godot-game/foundation/FoundationRoot.cs` | Guarded real callbacks, menu/session lifecycle, command submission and observation-only capture | VERIFIED |
| `Foundation.tscn`, `FoundationWorld.tscn`, `FoundationView.cs` | Real ordinary main scene, repository-authored primitives, actor projection and camera | VERIFIED |
| `FoundationInput.cs` | Actual physical holds, normalized aliases and fresh-press boundary, called by Root | VERIFIED |
| `tests/phase1/FoundationMovementTests.cs`, `FoundationLifecycleTests.cs`, `FoundationPermissionTests.cs` | Value/state-transition and authentic positive/negative boundary assertions | VERIFIED |
| `tools/phase2/check.cjs`, `rendered.cjs`, `evidence.cjs`, native input drivers | Catalog selection, real owned OS input, result reconciliation, bounded cleanup and completion validation | VERIFIED |
| `tests/phase1/FoundationRunnerTests.cs`, `tools/phase2/runner-fixture.cjs` | Invoked deliberate integrity/process/order/conflict and production cleanup controls | VERIFIED |
| `docs/workflow/foundation-checks.md`, `02-SCENARIOS.md` | Repeatable commands and saved selected case priorities, actions, expected outcomes and ordered groups | VERIFIED |
| `02-PLATFORM-EVIDENCE.json`, `02-VISUAL-REVIEW.md` | Explicit original report/hash references and actual associated image judgments | VERIFIED |

### Key Link Verification

| From | To | Actual connection | Status |
| --- | --- | --- | --- |
| `project.godot` | `Foundation.tscn` | `run/main_scene="res://foundation/Foundation.tscn"`; scene attaches Root script | WIRED |
| Root native events | Input → Core | Physical events populate input; each elapsed update creates one addressed command and calls `Advance` | WIRED |
| Root focus/return/exit | Input and session lifetime | Clears holds, closes old authority, removes world, disconnects callbacks | WIRED |
| Root | World and View | Loads/instantiates authored world; projects session Actor each update | WIRED |
| Native runner | Ordinary game Root | Owned process keyboard/mouse inputs; nonce-gated read-only post-draw observations, never movement setters | WIRED |
| Catalog/coordinator | Test host and evidence | Exact named test filters and native selection; hashes/identities and image checkpoints reconciled | WIRED |
| Completion index | Original reports and image reviews | Validator opens original paths and checks report/artifact/image/state hashes plus review bindings | WIRED |

### Data-Flow Trace (Level 4)

| Rendered value | Real source and route | Status |
| --- | --- | --- |
| Player position/facing | OS physical input → Root/Input → addressed command → Core Actor → View player position/marker | FLOWING |
| Camera | Core Actor X/elapsed time → View smoothing/clamp → World offset | FLOWING |
| Menu/error/actor count | Actual Start/return/error handlers and live tree nodes → visibility/focus and post-draw observations | FLOWING |

Static primitive geometry and English copy are the intended authored content, not disconnected dynamic data. Resources declare repository ownership and validate their dependency closure and byte-identical isolated copies.

### Behavioral Evidence and Spot-Checks

Retained executed reports were inspected and validated rather than rerun. Their CTRF reports record one named test for each pure/compiled case. The three named Core behaviors are `IdentifiedRightwardCommandAdvancesThreeUnits`, `PhysicalAliasesCancelAndRelease` and `ClosedAuthorityAndHeldKeysCannotCrossLifetimes`; permission acceptance uses `ProductionEventAccessRequiresAuthenticPrecedingGuard`.

| Selection | Original invocation / evidence | Result |
| --- | --- | --- |
| Core baseline | `.phase2/runs/win32-x64/269d8969-90cf-481d-a3fd-a88eb500e49c/summary.json` | Authority/input/lifecycle pass; permission compiler proof timeout retained as failed |
| Runner controls | `.phase2/runs/win32-x64/41e1747f-29ad-4426-ae34-d42faff92a47/summary.json` | 3/3 pass |
| Failed permission repair | `.phase2/runs/win32-x64/6bec74eb-439b-48bc-ad3e-6e503bd27252/summary.json` | 1/1 pass after strict build-flag alignment; same 30-second inner deadline |
| Entire rendered selection | `.phase2/runs/win32-x64/d4f78d36-22b3-4296-9d69-c544afc2486d/summary.json` | 9/9 pass, one PID 17240, 253.862s, Quit last, exit zero |
| Appearance | `.phase2/reviews/d4f78d36-22b3-4296-9d69-c544afc2486d-appearance.json` | Actual inspection covers 50 required references/22 distinct PNG hashes plus four supplementary captures |
| Strict build | `.phase2/review-fixes/strict-build.log` | Build succeeded, zero warnings/errors |
| Affected graph baseline | `.phase2/implementation-only-20261115/graph-all.json` | Actual CTRF: 50 passed, zero failed/skipped, 998.362s |
| Read-only check executed by verifier | `node tools/phase2/check.cjs --verify-completion .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` | Exit 0; PASS amended development-OS completion |

The native log identifies Godot 4.7.2, OpenGL compatibility on AMD Radeon RX 6600M and the expected recoverable entry fixture; no native error/warning was recorded. The completion validator preserves the failed baseline and accepts the explicit focused permission repair. It does not relabel either report as a fresh full run or certify Phase 5 dependency-aware reuse.

### Probe Execution

No shell probe is declared for this gameplay phase. The required executable completion gate was independently executed as recorded above. No server, VM or extra rendered process was started.

### Requirements Coverage

| Requirement | Source plans | Description | Status | Evidence |
| --- | --- | --- | --- | --- |
| BASE-01 | 02-01–03 | Visible menu, Start, direct Escape, fresh lifecycle and Quit | SATISFIED | Root lifecycle plus native menu/tracer/lifecycle/focus/quit outcomes |
| BASE-02 | 02-01–03 | Representative scene and identified grounded left/right controls | SATISFIED | Core/input/view implementation, value assertions and native direction/distance/world evidence |
| BASE-03 | 02-01–03 | Repeatable smoke checks and honest current evidence | SATISFIED | 16-case catalog/scenarios, runner controls, immutable reports, actual image review and independent completion validation |

No additional Phase 2 requirement IDs are orphaned. AC-01–AC-13 are represented by the catalog/scenario mappings under the amended environment scope.

### Test Quality Audit

| Test source | Linked requirements | Active executed cases | Assertion strength | Assessment |
| --- | --- | --- | --- | --- |
| Foundation movement/lifecycle proofs | BASE-01/02 | 3 | Exact distances/identities, rejected inputs and immutable state after closure | Adequate |
| Foundation permission proof | BASE-02/03 | 1 repaired pass | Authenticated clean controls plus intended neighboring rejection diagnostics | Adequate |
| Foundation runner proofs | BASE-03 | 3 | Exact rejection behavior, real child failures/cleanup, order/conflict and preserved outcomes | Adequate |
| Native scenarios | BASE-01–03 | 9 | Production OS inputs, simulation-time intervals, actual node counts and saved images | Adequate |

No accepted case is disabled/skipped. Synthetic images/results in runner fixtures deliberately test evidence mechanics; they are not presented as game behavior or appearance evidence. Gameplay expectations derive from the explicit movement/lifecycle contract, while visual judgment has a separate actual-inspection receipt. A resource copy check alone would be insufficient runtime proof; the same selected live scene supplies it here.

### Decision Coverage

The read-only decision-coverage gate found **3/3** trackable decisions honored, no missing decisions: “All trackable CONTEXT.md decisions are honored by shipped artifacts.”

### Anti-Patterns and Review

No blocking stub, disconnected gameplay state or unresolved phase finding was found in the inspected source. Whole-phase review and repair history are preserved in [02-REVIEW.md](02-REVIEW.md) and [02-REVIEW-FIX.md](02-REVIEW-FIX.md): all four findings and the intermediate cleanup regression were fixed and re-reviewed clean through `d087f15`. The later test-helper build-flag alignment is covered by the actual repaired permission pass.

The disconfirmation pass checked three plausible false passes: hidden actors instead of actual teardown, an observer bypassing production input, and fixture-only images supplying gameplay acceptance. Actual world removal/node counts, real OS input acknowledgements and independently hash-bound native images resolve those concerns. Cleanup/error behavior additionally has production-boundary pure controls; green happy-path screenshots alone were not used to establish it.

### Human Verification Required

None outstanding. The planner-deferred appearance/readability check has already been performed by actual saved-image inspection and recorded against the required image hashes. The creator subsequently accepted completion and directed phase closure. No new review or unperformed environment is represented as passed.

### Scope Limits and Closure Handoff

No current Phase 2 goal gap remains. Routine-local completion is Windows 1280×720 under explicit creator authority. Other-environment, supported-display, hosted workflow, mixed-computer LAN, reference-hardware and endurance/release procedures retain their separate later owners; this report supplies no synthetic Linux or 1920×1080 pass. Historical Phase 1 exceptions/follow-ups remain unchanged. No Phase 3 work was started.

Final documentation navigation validation after staging this new report is the orchestrator's remaining repository closure step; this report does not claim that future check passed. The verifier changed only this report and did not commit or alter shared phase state.

---

_Verified: 2026-11-15T12:35:02Z_
_Verifier: the agent (gsd-verifier)_
