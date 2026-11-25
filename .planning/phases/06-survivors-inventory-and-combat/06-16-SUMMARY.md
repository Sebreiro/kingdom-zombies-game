---
phase: 06-survivors-inventory-and-combat
plan: "16"
subsystem: testing
tags: [verification, rule-suite, roslyn, godot-source-generators, producer-replay, msbuild]
status: complete

requires:
  - phase: 06-survivors-inventory-and-combat
    provides: "06-14 reviewed Phase 6 source and the failed initial phase-end run (F2, F4, F5 R-67, F7)"
provides:
  - "Isolated rule and state fixtures compile the exact linked foundation set, including FoundationPublicWorld.cs"
  - "MTP producer pin bound to the current proof csproj with unchanged generated surface"
  - "Suppression scan binds each file under its consuming project's real assembly name"
  - "Current Godot producer replay that may append new inputs, derived by a committed tool"
  - "Concurrent historical Godot producer replay builds for the repository scan"
affects: [06-17, 06-24, 06-25, 06-15, phase-11-certification]

actuals:
  tokens: 200145
  tasks: 2
  commits: 7
plan_head_before: 599a012113cb8fda748e881542f5bd5620751342
plan_head_after: a7d2a2a3ac1f043771c888ff234dca419bf4dd33

tech-stack:
  added: []
  patterns:
    - "Derived pins are rebound only through their derivation: canonical-JSON sourceInputsSha256, registry file SHA-256, current-replay.cjs --write"
    - "A current producer replay lists the historical cohort inputs first in exact order, then ordinal-ascending added inputs, each changed or added input carried as a snapshot"
    - "Fixture preparation and restore stay sequential; only independent fixture builds run concurrently"

key-files:
  created:
    - src/godot-game/tests/verification/CurrentReplayTests.cs
    - src/godot-game/tools/verification/current-replay.cjs
  modified:
    - src/godot-game/tests/verification/FoundationLinkedSources.cs
    - src/godot-game/tests/verification/RestoreMutationProof.cs
    - src/godot-game/tests/verification/ConsumerCompilation.cs
    - src/godot-game/tests/verification/ExemptionValidator.cs
    - src/godot-game/tests/verification/EngineGeneratedTests.cs
    - docs/code/generated-callsite-adaptations.json
    - docs/code/engine-generated-spans.json
    - src/godot-game/tools/verification/required-cases.json
    - src/godot-game/tools/README.md

key-decisions:
  - "F2 and state.restore-mutation share one cause: fixture copies lacked FoundationPublicWorld.cs (CS2001)"
  - "F7 is the same stale MTP csproj pin as F4; the generated SelfRegisteredExtensions.cs surface is unchanged"
  - "Creator decision (option b): the suppression scan compiles under the consuming project's real assembly name"
  - "Creator decision (option A): the current producer replay may append new inputs after the exact ordered historical inputs; the Phase 5 cohort and its hard-coded authorities stay unchanged"
  - "Creator decision (option 1): historical Godot replay fixture builds run concurrently; restores stay sequential and childTimeoutMs stays 60000"

patterns-established:
  - "current-replay.cjs --check/--write is the derivation for currentProducerReplay; never hand-edit its hashes"

requirements-completed: []

coverage:
  - id: D1
    description: "Rule and state fixture copies compile the exact linked foundation set (F2, R-67 linked ownership, restore-mutation)"
    requirement: "INV-11"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/verification/check.cjs --cases R-46.positive,R-46.negative,R-47.positive,R-47.negative,R-67.positive,R-67.negative,state.restore-mutation,rules.generated-callsite-boundary,rules.xunit-producer-relocation,rules.xunit-producer-seed,rules.xunit-producer-bypass,rules.xunit-producer-shape,rules.xunit-producer-hash (run 9d9d8ce2, 13/13)"
        status: pass
    human_judgment: false
  - id: D2
    description: "MTP producer pin and the current Godot producer replay follow the current sources (F4, F7, boundary-reproduction-live)"
    requirement: "COMBAT-08"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/verification/check.cjs --cases engine.generated-origin,engine.generated-boundary,edge.rule-03.adjacency,rules.generated-callsite-origin,rules.suppression-duplicate (run 34416f1a, 5/5)"
        status: pass
      - kind: unit
        ref: "src/godot-game/tests/verification/CurrentReplayTests.cs (4/4, direct dotnet run)"
        status: pass
    human_judgment: false
  - id: D3
    description: "Suppression scan resolves internal-constant attributes and still rejects every planted suppression"
    requirement: "RUN-04"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/verification/check.cjs --cases R-67.negative,R-50.negative,rules.suppression-pragma,rules.suppression-semantic,rules.suppression-duplicate,rules.suppression-configuration,rules.suppression-effective,rules.suppression-conditional,rules.generated-claims,rules.exemption-executable-positive,rules.exemption-executable-negative,edge.rule-03.adjacency,edge.rule-03.empty,rules.xunit-producer-root,engine.consumer-branches,engine.fake-generated (run fcf21005, 16/16)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/verification/check.cjs --group docs (run 9d82078d, 36/36)"
        status: pass
    human_judgment: false

duration: 689min
completed: 2026-11-25
---

# Phase 6 Plan 16: Rule-suite fixture inventory and MTP producer pin Summary

**The Phase 1 rule, state and engine proofs fail run-1's F2, F4, F7 and R-67 causes no longer. Fixtures now copy FoundationPublicWorld.cs, the MTP pin names the current proof csproj, the suppression scan sees friend-assembly internals, the Godot producer replay is derived from Phase 6 sources by a committed tool, and replay fixture builds run concurrently so R-67 fits its 60 s limit.**

## Performance

- **Duration:** 689 min wall-clock, including three creator decision round trips
- **Started:** 2026-11-25T00:46:05Z
- **Completed:** 2026-11-25T12:15:11Z
- **Tasks:** 2 planned tasks plus three creator-approved scope extensions
- **Files modified:** 11 (2 created)

## Accomplishments

- **F2 and state.restore-mutation.** One cause: CS2001 for `FoundationPublicWorld.cs`, which the fixture copies omitted. `FoundationLinkedSources.Paths` now equals the proof project's linked Compile set.
- **F4 and F7.** One stale pin. `phase1-baseline-mtp` named the csproj before Phase 6. The rebind came only after the generated surface was shown to be unchanged.
- **R-67.positive.** Repaired at three successive depths:
  - the linked set
  - the MTP pin
  - the suppression scan's assembly identity
  - It now passes in 41.6 s, under its unchanged 60 s limit.
- **engine.generated-boundary.** `boundary-reproduction-live` passes. The current replay now covers all 79 Phase 6 game inputs, while the Phase 5 cohort stays untouched.

## Task Commits

1. **Task 1: Fixture copies compile the current linked foundation sources.** `5ccc886` (fix)
2. **Task 2: Prove F7 shares the stale MTP pin, then bind it.** `8c74160` (fix)
3. **Extension 1: Suppression scan uses the consumer's assembly name.** `10d26d3` (fix)
4. **Extension 2a: Replay may append new inputs, with the CurrentReplayTests negatives.** `1929332` (feat)
5. **Extension 2b: Derivation tool current-replay.cjs.** `37ce78e` (feat)
6. **Extension 2c: Replay derived from Phase 6 sources.** `0a96455` (fix)
7. **Extension 3: Concurrent historical replay builds.** `a7d2a2a` (perf)

## Diagnosis evidence (before repair)

**Restore-mutation, run 7b0f665b.** The new `INFRASTRUCTURE_CLEAN_BUILD:` customMessage showed:
`CSC : error CS2001: Source file '…\src\godot-game\tests\verification\../../game/foundation/FoundationPublicWorld.cs' could not be found.`
This is the same CS2001 for FoundationPublicWorld.cs as R-46 (run 2dd526e8, which also failed R-67.positive on the linked set).

**F7, runs 64f9682f and run-1 6dc691a3.** Three checks failed on the same assertion:
- edge.rule-03.adjacency
- `engine.generated-origin` stage `origin-fixed`
- `engine.generated-boundary` stage `boundary-calls-cohort-0`

The assertion was `Hash(... sourceInputs path)`, should be `6a0cf431df8b4e8b3a250a7c8d4de07e435b65a9f019c960e0c54c05ab1ea271` but was `5ced3273a80cf7152602e8393fc6a8250585622ff907df1a5163c8cab74afab4`.

**KZ-EX-MTP-CALLS-01 surfaces were compared unchanged before the rebind:**
- Since 18e77bc, the csproj diff is 10 added linked `<Compile>` items and nothing else.
- `obj/Debug/net10.0/SelfRegisteredExtensions.cs`, built from the current csproj and without a BOM, hashes to `e076067d5288932df9dd49714222151b9209d952053047cba3eaca7b6b2ebf05`. That equals the recorded `sha256` and `textSha256`.

**Godot replay, run b823ae13.** `boundary-reproduction-live` failed with `CURRENT_PRODUCER_INPUTS`:
- Phase 6 changed 18 of the 57 recorded game inputs and added 22 game C# files.
- A fresh `dotnet build src/godot-game/game/KingdomZombies.csproj` rewrote all 32 Godot.SourceGenerators outputs. All 32 equal the cohort's recorded `sha256` and `textSha256`.

**R-67.positive time.** It was 61.5-62.3 s, against a 60 s limit.
- The control with the old `"SuppressionScan"` name was just as slow: 62.3 s.
- About 48 s went to four sequential historical replay fixture builds of about 12 s each.
- Phase 6 grew Core from 21 files / 240,620 bytes to 48 files / 637,046 bytes. Every fixture recompiles Core.

## Rebound pins and derived data

| Pin | Old | New | Derivation |
| --- | --- | --- | --- |
| `phase1-baseline-mtp` `sourceInputs[0].sha256` | `6a0cf431…ea271` | `5ced3273…afab4` | SHA-256 of the git blob of the csproj |
| `phase1-baseline-mtp` `sourceInputsSha256` (registry and ac-08 cohort row) | `8c78e497…c4b3` | `571639791a2dc0e006d1e29c313b5c5efc45548599cab64d99599dce8ca17648` | Canonical compact JSON; reproduces the old value from the old input |
| generated-callsite-adaptations.json file hash (two producer-stage bindings, callerAuthority) | `43078b67…8763` | `a2bd41043b0990a61d4210192e0a4a8fb1a3307cfa36af74e2d2287a2bb9ebd0` | File SHA-256 |
| `currentProducerReplay` `sourceInputsSha256` | `dac57229…cc71e` (57 inputs, 7 snapshots) | `1f501db5cb53557f36bd1a15cfd0ac8876ffa09fccf1fd15e27da3cd9099ce03` (57 historical + 22 added, 44 snapshots: 22 changed + 22 added) | `node src/godot-game/tools/verification/current-replay.cjs --write` |
| engine-generated-spans.json file hash (two producer-stage bindings) | `054e71e0…f976` | `cf8a25f63d5845065cf4a07974e15e230ca21b04399b86f80ba66764d13b3795` | File SHA-256 |

**Unchanged:**
- The ac-08 `repositoryStages.bindings` rows for both registries still hold the old hashes. 06-24's `ac08-bindings.cjs --write` owns them.
- Cohort `inspection23-final-5943a86f6eb2` is unchanged: its 78 rows, its observations, and the hard-coded IDs and hashes in ExemptionValidator.cs, ExemptionTests.cs and required-cases.json.
- In the spans registry, only `currentProducerReplay` changed. `schemaVersion`, `scope`, `local`, `producerInventories` and `producerSources` are byte-identical.

**Added replay inputs (22):** FoundationActorArt, FoundationAppearance, FoundationAppearanceAssets, FoundationCoordinator.RunWire, FoundationEquipmentProjection, FoundationHud.Live, FoundationHudState, FoundationInventoryArt, FoundationInventoryGrid, FoundationInventoryInteraction, FoundationInventoryWorkspace(.External, .Input, .Layout), FoundationPublicWorld, FoundationRunFrame, FoundationRunReadback, FoundationRunView, FoundationSpectator, FoundationSpectatorPanel, FoundationViewInbox, FoundationWorldInput. All are under `src/godot-game/foundation/`, recorded as logical paths.

**Tool check:** `current-replay.cjs`, run on an export of the Phase 5 tree b9aaa2b, reproduces the committed hand-made Phase 5 replay exactly: 57 inputs, 7 changed, 0 added.

## Files Created/Modified

- `src/godot-game/tests/verification/FoundationLinkedSources.cs`: adds FoundationPublicWorld.cs.
- `src/godot-game/tests/verification/RestoreMutationProof.cs`: the clean fixture build reports its output.
- `src/godot-game/tests/verification/ConsumerCompilation.cs`: exposes the evaluated `AssemblyName`.
- `src/godot-game/tests/verification/ExemptionValidator.cs`: scans under the consumer's assembly name, and prepares producer roots before reading permissions.
- `src/godot-game/tests/verification/EngineGeneratedTests.cs`:
  - adds `ReplaySnapshotPaths`
  - adds `PrepareProducerRoots`
  - splits `ReplayProducer` into `PrepareReplay` and `FinishReplay`
  - extracts `MatchesRoot` and `ReplaySources`
  - adds a lock over the replay-root cache and the progress receipts
- `src/godot-game/tests/verification/CurrentReplayTests.cs` (new): tests that a missing original, reordered originals, invalid added inputs, and the positive case each behave correctly.
- `src/godot-game/tools/verification/current-replay.cjs` (new) and its row in `src/godot-game/tools/README.md`.
- `docs/code/generated-callsite-adaptations.json`, `docs/code/engine-generated-spans.json` and `src/godot-game/tools/verification/required-cases.json`: the pins and data in the table above.

## Decisions Made

See key-decisions. All three scope extensions follow decisions the creator made in chat, relayed by the orchestrator.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] Derived pins of the MTP registry had to be rebound with the csproj hash**
- **Found during:** Task 2
- **Issue:** Changing `sourceInputs[0].sha256` changes the canonical `sourceInputsSha256`, which the registry and the ac-08 cohort row mirror, and the registry's file hash, which is pinned in two producer-stage bindings and `callerAuthority`. Without these the runner rejected the registry: `PRODUCER_REGISTRY_BINDING — calls` (run 720ff871).
- **Fix:** Rebound each through its existing derivation (table above). The ac-08 bindings row is left to 06-24.
- **Files modified:** docs/code/generated-callsite-adaptations.json, src/godot-game/tools/verification/required-cases.json
- **Committed in:** 8c74160

### Creator-approved scope extensions

**2. [Scope extension, creator decision: option b] SUPPRESSION_ATTRIBUTE_UNRESOLVED in R-67.positive**
- **Found during:** Task 1. This failure surfaced only after the F4 pin repair.
- **Issue:** `ExemptionValidator.CheckSource` bound each file under the fixed assembly name `"SuppressionScan"`. `[InlineData("Stash", RunWorld.StashId)]` in Phase06ExternalInteractionTests.cs uses an internal Core constant, and Core's InternalsVisibleTo names only the proof assembly, so the attribute did not resolve.
- **Fix:** Compile under `consumer.AssemblyName`. The rule, the R-67 assertion, every exemption and the test file are unchanged.
- **Verification:**
  - R-67.positive passes (run 9d9d8ce2).
  - R-67.negative and the 15 other scanner cases pass (run fcf21005).
- **Committed in:** 10d26d3

**3. [Scope extension, creator decision: option A] Stale current Godot producer replay**
- **Found during:** Task 2. This failure surfaced only after the F7 pin repair.
- **Issue:** The replay format required the cohort's exact 57-path list, so it could not represent the 22 Phase 6 inputs. No regeneration tool existed.
- **Fix:**
  - `VerifyCurrentReplay` now requires the historical inputs first, with exact paths and order (`REPLAY_HISTORICAL_INPUTS`), followed only by ordinal-ascending added paths that never repeat an original (`REPLAY_ADDED_INPUTS`). Snapshots must be the changed historical inputs followed by every added input.
  - New minimal derivation tool `current-replay.cjs` (`--check` / `--write`). It refuses uncommitted sources and never edits the cohort.
  - The replay was regenerated with `--write`; `--check` then passes.
- **Verification:**
  - CurrentReplayTests 4/4: a missing original is rejected in 4 variants and reordered originals in 3; invalid added inputs are rejected; the positive case passes.
  - engine.generated-boundary passes (run 34416f1a).
- **Committed in:** 1929332, 37ce78e, 0a96455

**4. [Scope extension, creator decision: option 1, "Investigate, no raise"] R-67.positive over its 60 s limit**
- **Found during:** extension 2. R-67.positive passed in a direct run but took about 62 s.
- **Issue:** Four historical replay fixture builds ran one after another. Each recompiles the current Core, which Phase 6 grew 2.6×.
- **Fix:** `PrepareProducerRoots` runs in `ReadPermissions`.
  - Each needed replay fixture is still written and restored one at a time, using the existing `--disable-parallel` restore.
  - Only the four builds run concurrently.
  - The replay-root cache and progress receipts are locked.
  - Fixtures, inputs, builds and assertions are the same. The direct run still creates exactly four fixtures, and childTimeoutMs stays 60000.
- **Verification:** The direct run took 41.4 s. In the runner, R-67.positive passed in 41.6 s (run 9d9d8ce2). No flakiness was observed across the runs.
- **Committed in:** a7d2a2a

---

**Total deviations:** 1 auto-fixed (Rule 3) and 3 creator-approved scope extensions.
**Impact on plan:**
- Every extension fixes a failure that run 1 had hidden behind F4 or F7.
- No rule, exemption, assertion, count or timeout was weakened or raised.
- The plan's files_modified list grew to include the files the creator approved.

## Issues Encountered

- **Known tools-test failure, owned by 06-24.**
  - The test: `src/godot-game/tools/verification/bindings.test.cjs`, "ac-08 derivation reproduces both committed catalogs byte for byte at the last catalog commit". 24/25 pass.
  - Cause: this plan's required-cases.json edits moved the "last catalog commit" to a revision whose Phase 6 sources the ac-08 block does not yet bind. `ac08-bindings.cjs --check` already reported 118 differences at 09e2881.
  - Resolution: 06-24's planned `ac08-bindings.cjs --write` regenerates the ac-08 block and both catalogs, which restores this test.
- **R-48.positive still lists exactly the 43 run-1 violations.** 06-17 owns those. The four new CurrentReplayTests method names are not among them.

## Verification (final, fresh invocations at a7d2a2a)

| Invocation | Result | Run |
| --- | --- | --- |
| Task 1: 13 cases (R-46±, R-47±, R-67±, state.restore-mutation, rules.generated-callsite-boundary, five rules.xunit-producer-*) | PASS 13/13; R-67.positive 41.6 s | 9d9d8ce2-2601-4e0e-9142-5973fe23f334 |
| Task 2: engine.generated-origin, engine.generated-boundary, edge.rule-03.adjacency, rules.generated-callsite-origin, rules.suppression-duplicate | PASS 5/5 | 34416f1a-3567-4742-85fa-da9af0f96a27 |
| 16 other scanner cases | PASS 16/16 | fcf21005-9d71-4b6d-ad12-d8e7dd3ee0f9 |
| `check.cjs --group docs` | PASS 36/36 | 9d82078d-96bf-45af-a5e2-dbac9275cc8b |
| CurrentReplayTests, direct dotnet run | 4/4 | none |
| `current-replay.cjs --check` | PASS: 57 historical, 22 changed, 22 added | none |

Acceptance greps:
- FoundationPublicWorld.cs appears once in FoundationLinkedSources.cs.
- `6a0cf431…` appears 0 times in generated-callsite-adaptations.json and `5ced3273…` once.
- `git diff --stat 09e2881..HEAD -- src/godot-game/tests/verification` also lists the creator-approved extension files. Task 1 alone touched only FoundationLinkedSources.cs and RestoreMutationProof.cs.

Run-1 reports and `.local/temp/phase06-review.json` are untouched. No game window was opened.

## How to reproduce

From the worktree root, with pinned Node on PATH:

1. `node src/godot-game/tools/verification/current-replay.cjs --check`
2. `node src/godot-game/tools/verification/check.cjs --cases R-46.positive,R-46.negative,R-47.positive,R-47.negative,R-67.positive,R-67.negative,state.restore-mutation,rules.generated-callsite-boundary,rules.xunit-producer-relocation,rules.xunit-producer-seed,rules.xunit-producer-bypass,rules.xunit-producer-shape,rules.xunit-producer-hash`
3. `node src/godot-game/tools/verification/check.cjs --cases engine.generated-origin,engine.generated-boundary,edge.rule-03.adjacency,rules.generated-callsite-origin,rules.suppression-duplicate`
4. `node src/godot-game/tools/verification/check.cjs --cases R-67.negative,R-50.negative,rules.suppression-pragma,rules.suppression-semantic,rules.suppression-duplicate,rules.suppression-configuration,rules.suppression-effective,rules.suppression-conditional,rules.generated-claims,rules.exemption-executable-positive,rules.exemption-executable-negative,edge.rule-03.adjacency,edge.rule-03.empty,rules.xunit-producer-root,engine.consumer-branches,engine.fake-generated`
5. `node src/godot-game/tools/verification/check.cjs --group docs`

Each must print PASS with every selected obligation.

## User Setup Required

None. No external service configuration is required.

## Next Phase Readiness

- 06-17 (R-48 renames) can proceed.
- 06-24 must rebind both registries' ac-08 binding rows through `ac08-bindings.cjs --write`. That also restores the bindings tools test.
- The 06-24 independent review should include:
  - the replay rule change
  - `current-replay.cjs`
  - the regenerated replay
  - the concurrent replay builds

---
*Phase: 06-survivors-inventory-and-combat*
*Completed: 2026-11-25*

## Self-Check: PASSED

- FOUND: src/godot-game/tests/verification/CurrentReplayTests.cs, src/godot-game/tools/verification/current-replay.cjs
- FOUND commits: 5ccc886, 8c74160, 10d26d3, 1929332, 37ce78e, 0a96455, a7d2a2a
