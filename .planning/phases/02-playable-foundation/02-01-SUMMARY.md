---
phase: 02-playable-foundation
plan: "01"
subsystem: gameplay-foundation
tags: [godot, csharp, native-input, generated-provenance, fixed-point]
status: complete
requires:
  - phase: 01-code-rules-and-documentation
    provides: Pinned five-project build, exact source/generated permissions and proof transport
provides:
  - Ordinary Start/Quit main scene with a real identified rightward walking tracer
  - Immutable movement/state contracts and sole pure fixed-point movement authority
  - Exact production native/public/generated permission controls
affects: [02-02, 02-03, 03-four-player-lan-foundation]
tech-stack:
  added: []
  patterns: [identified-command-authority, guarded-native-root, post-draw-read-only-observer, owned-os-input]
key-files:
  created:
    - src/core/FoundationMovementCommand.cs
    - src/core/FoundationActorState.cs
    - src/core/FoundationSession.cs
    - src/godot-game/foundation/FoundationRoot.cs
    - src/godot-game/foundation/FoundationInput.cs
    - src/godot-game/foundation/FoundationView.cs
    - src/godot-game/foundation/Foundation.tscn
    - src/godot-game/foundation/FoundationWorld.tscn
    - tests/phase1/FoundationMovementTests.cs
    - tests/phase1/FoundationPermissionTests.cs
    - tools/phase2/check.cjs
    - tools/phase2/rendered.cjs
    - tools/phase2/windows-input.ps1
    - tools/phase2/linux-input.sh
    - tools/phase2/required-cases.json
  modified:
    - tests/phase1/EngineAccessPolicy.cs
    - tests/phase1/ArchitecturePolicy.cs
    - tools/code-rules/EngineGeneratedSyntax.cs
    - docs/code/engine-generated-spans.json
    - docs/code/generated-callsite-adaptations.json
    - src/godot-game/GeneratedCallSiteAdapters.cs
key-decisions:
  - Keep the existing five-project graph and pinned packages; use authored primitive scenes.
  - Instantiate and remove an authored world scene; count actual actor-group nodes in smoke observations.
  - Compile sixteen generated-member collision subjects together while checking every exact type/member diagnostic and retaining the inherited deadline.
requirements-completed: [BASE-01, BASE-02, BASE-03]
requirement-scope: Plan-local tracer and permission delivery only; complete Phase 2 requirement acceptance remains with plans 02-02 and 02-03.
duration: 30min
completed: 2026-11-14
actuals:
  tokens: 36469
  tasks: 2
  commits: 6
plan_head_before: a61bcd3c7e7db911a999390be4102ea61fca450e
plan_head_after: 3a52e6660945553914803dd36940ed70872ab799
tdd_classifier: not-executed-before-green
---

# Phase 2 Plan 1: Native playable tracer and exact permissions Summary

**The normal Godot project now launches a native Start/Quit menu, creates one identified Core actor, walks it through real OS input, removes its world on Escape, and exits cleanly through Quit.**

## Performance and scope

The measured source-execution window starts at the first authored test file's filesystem creation time, 2026-11-14T23:08:45Z, and ends at approximately 23:39Z: 30 minutes, excluding earlier context reading. Two tasks are delivered. Thirty implementation files changed; the measured six-commit ledger interval also includes the orchestrator's separate continuation-document commit `d17e1c9`. The diff estimate is `ceil(realized non-planning diff characters / 4)`, not a harness token count. The summary commit is outside the recorded `plan_head_after` interval.

This plan establishes the selected Windows tracer and exact permissions. It does not certify the complete Phase 2 catalog, Linux, 1920×1080, expanded input/lifecycle behavior, or Phase 1 historical human judgments.

## Accomplishments and contracts

- `FoundationMovementCommand(string sessionId, string playerId, string actorId, int direction)` is an immutable record addressed to the active ownership tuple.
- `FoundationActorState` publishes schema version 1, exact session/player/actor identities, authored fixture/ground/building/player content identities, fixed-point X, ground feet 0, facing and elapsed milliseconds.
- `FoundationSession(string sessionId, string playerId, string actorId)` owns `Actor`; its sole mutation method is `Advance(FoundationMovementCommand command, long elapsedMilliseconds)`. It rejects wrong ownership, invalid direction and negative time, checks elapsed/distance arithmetic, uses 1000 subunits/u and 3000 subunits/s, spawns at −8000 and clamps to −59000..59000. Zero direction preserves position/facing.
- `FoundationRoot` captures the authentic main-thread gateway, uses native buttons/events, carries fractional elapsed conversion remainder, submits the identified command and delegates read-only projection at 16 logical pixels/u. The ordinary project main scene requires no proof nonce or alternate scene argument.
- `FoundationWorld.tscn` supplies flat authored ground/building/player primitives. Start instantiates the world; Escape closes command acceptance and removes/frees it. The smoke observer counts actual `foundation-actor` nodes, proving zero → one → zero actors in the tree.
- The observer is enabled only by explicit `PHASE2_NONCE`/`PHASE2_OUTPUT`, consumes observation-only checkpoint files, waits for completed draw, captures PNGs and atomically publishes state. It cannot start sessions or issue movement commands.
- The coordinator independently declares all sixteen final IDs and supports `--case`, `--group`, `--all`, `--list`. Three are implemented here; selecting an unimplemented declaration fails. Runs use fresh nonce/input/tool/platform identities and immutable directories under ignored `.phase2/runs/<platform>/<nonce>/`.
- Windows input checks the exact game PID/window and acknowledged focus before sending native mouse and physical scan-code keyboard events. The archive's window-owning executable is used directly; the console launcher owns a different process. The Linux xdotool adapter is supplied for later execution, without claiming a Linux pass.

## Exact generated and public inventory

The actual pinned generator remains `Godot.SourceGenerators/4.7.2`, canonical package SHA-256 `ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e`.

- Generated syntax permissions increase from 170 to **204**, with **34 new exact diagnostic/path/full-Roslyn-text-hash/span/member rows**. Existing proof rows remain; the assembly-wide `AssemblyScriptTypes` row changes only to match the actually emitted script list.
- Eight new generated callsite members on `KingdomZombies.Foundation.FoundationRoot` increase the callsite registry from 10 to **18** and the Godot collision inventory from 8 to **16**. The additions are `GetGodotMethodList`, `InvokeGodotClassMethod`, `HasGodotClassMethod`, `SetGodotClassPropertyValue`, `GetGodotClassPropertyValue`, `GetGodotPropertyList`, `SaveGodotObjectData` and `RestoreGodotObjectData`.
- Actual raw generated bytes, Roslyn text, exact spans/member identities and canonical package hashes are authenticated. There are **26** registered member suppression permissions in total, retaining the prior eighteen. Analyzer severities, donor records and banned symbols are unchanged.
- R-34 remains an exact seven-type registry: the previous three plus the four necessary Foundation public types. The actual enforcement inventory in `ArchitecturePolicy` matches those additions; an adjacent public type is rejected.
- Native permission controls cover every production method containing native operations, authentic preceding guards, absent/late/branch-only guards, native method-group capture, worker/local/async use, wrong paths, adjacent types/methods and forged namespaces. `LoseFocus` performs only pure input clearing; it is not falsely treated as a native-operation subject.

The Windows GUI executable SHA-256 is `45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab`. It was independently read from the exact official archive after verifying the archive's pinned SHA-512, and the coordinator checks that executable hash in addition to the pinned console executable and engine version.

## Final executed verification

All rows below passed. The three Phase 2 runs share final input digest `4299a6f05d24a1701f70bc8b52b6d047c1ede028bda5323360ed154f340acc89`.

| Command / selection | Invocation | Result |
|---|---|---|
| Strict whole-solution build, unchanged prescribed flags | Final build before stabilized selection | Zero warnings, zero errors; 5.63s |
| `node tools/phase2/check.cjs --case core.permissions` | `3f84711d-b495-43aa-a1b6-cb4977315509` | 1/1; 31.240s |
| `node tools/phase2/check.cjs --case core.authority` | `176de08b-5c36-4b8c-896d-533339fe6573` | 1/1; 1.546s |
| `node tools/phase2/check.cjs --case rendered.tracer` | `d316a367-0195-4851-b010-33992cdaccc4` | 1/1; 7.247s; clean native log and exit |
| `node tools/phase1/check.cjs --case engine.source-policy` | `3d062dc0-a9ba-4767-ae15-d2f29efc0aed` | 1/1 |
| `node tools/phase1/check.cjs --case engine.generated-origin` | `7fc7a843-34ff-4fd0-889b-e5bf310f5f68` | 1/1 |
| `node tools/phase1/check.cjs --case engine.generated-boundary` | `6cb3cbb9-8892-4202-b475-d5c6b2371454` | 1/1 |
| `node tools/phase1/check.cjs --case rules.godot-callsite-boundary` | `adccd41a-8a47-4b2b-9c50-7fa044ca68b0` | 1/1 within original deadline |

The tracer's four actual 1280×720 images were decoded and visually reviewed: readable launch/returned menus with focused Start, sky, continuous ground, decorative outlined building, one outlined accent player, readable hint and visible right-facing marker. Review: `.phase2/reviews/d316a367-0195-4851-b010-33992cdaccc4.executor-review.json`, bound to each PNG hash. No Linux or other-resolution visual credit is claimed.

## Task commits

1. `1a28a7b` — test: identified rightward authority behavior.
2. `cefca0a` — feat: identified fixed-point movement authority; immediately handed to orchestrator for draft PR.
3. `c55b84f` — feat: native menu, authored fixture, exact generated permissions and real-input tracer. Tracer feedback gate passed before permission expansion.
4. `bdf29d5` — test: production native event-access guard regression.
5. `3a52e66` — fix: complete production permission controls, actual scene lifetime and bounded collision proof.

The separate orchestrator-owned `d17e1c9` continuation commit is included in the measured ledger count, not attributed as an implementation task.

## TDD evidence and process limitation

- Task 1 genuine RED: `.phase1/foundation-red/red.json`; exactly one named xUnit test executed and failed on expected X −5000 versus actual −8000 before movement was implemented. The subsequent strict build and focused GREEN passed. The RED test commit precedes the implementation commit.
- Task 2 genuine RED: `.phase1/foundation-permissions-red-valid/red.json`; the guarded positive control passed, then the named test failed because the scanner accepted unguarded native event access. Adding `IEventReferenceOperation` inspection fixes the planned rejection. An earlier `.phase1/foundation-permissions-red/red.json` attempt lacked `PHASE1_ROOT` and is explicitly excluded as infrastructure failure, not RED evidence.
- Exact original CTRF reports, commands and semantic assessments are preserved in each valid RED directory's `evidence.json`; original reports were not rewritten.
- **The GSD `tdd-red-evidence` classifier was not executed before GREEN.** Its documented supported reporter list did not include the repository's xUnit CTRF format. No machine-classifier approval, retrospective gate approval or fabricated report conversion is claimed. This is a disclosed workflow limitation, distinct from the actual planned-assertion failures and current functional passes.
- Attempting the prescribed optional cross-phase ledger append was refused because existing `.planning/WINDOWS.md` rendered-table/JSON content disagrees for row 28. No unrelated ledger content was repaired or overwritten; the process limitation remains explicit here for the orchestrator.

## Deviations and retained diagnostic runs

1. **[Rule 2 — Missing critical functionality] Native event references.** The existing scanner did not inspect `IEventReferenceOperation`; a genuine failing production control exposed the gap. The scanner now resolves event symbols with the same exact guarded tuple policy.
2. **[Rule 3 — Blocking integration] Exact visibility enforcement inventory.** `ArchitecturePolicy` contained a hardcoded three-type list. It now names exactly the four new necessary types alongside the prior three, matching the registry and its actual repository scan.
3. **[Rule 1 — Lifetime correctness] Actual world removal.** The initial composition hid authored world nodes on return. Extracting `FoundationWorld.tscn` allows native instantiation/removal and actual actor-node counts. Final tracer proves clean 0/1/0 lifetime.
4. **[Rule 1 — Native tooling correctness] Owned GUI PID, physical scan codes, atomic files and conditional draw disconnection.** Initial runs exposed console-wrapper window ownership, missing physical scan codes, file publication races and an unnecessary signal disconnect. The final native run has clean logs and captures actual movement.
5. **[Rule 3 — Verification capacity] Collision batching.** Sixteen sequential real collision builds exceeded the inherited case deadline. One combined consumer still proves each exact type/member's `CS0111` diagnostic from generated source, preserving every obligation and the deadline.

Material diagnostic invocations are retained unchanged: native `03c8378a-ec4d-4c04-a3d4-f9be8c2865c3` (console-wrapper window missing), `69400b04-d8be-4990-8fb0-86dd66cde69d` (physical movement absent), permission `3867d0d3-f8b5-4684-984b-6d49e21108db` (assertions passed but SOURCE_CHANGED correctly rejected edits during verification), permission `083b4ffe-c6d7-404b-9368-0111d3e3f2aa` (test incorrectly expected pure LoseFocus to perform native work), and inherited `fc9a36f5-44c2-4192-ad1a-cc0a2b2ab39b` (sequential-collision CHILD_TIMEOUT). Later stable passes above supersede acceptance; earlier records remain historical.

## Remaining Phase 2 obligations and continuation

Thirteen independently declared catalog cases intentionally remain pending for plans 02-02/02-03: expanded pure input/lifecycle; rendered menu, directions, measured distance, world, lifecycle, focus, quit and resources; runner integrity, processes and repeatability. Selecting pending cases remains non-success. Release barriers, camera smoothing, full UI states and broader rejection/process controls are later-plan work; no unwired UI or blocking stub is claimed delivered here. Stub scan found no TODO/FIXME/empty-placeholder flow in the delivered slice.

Continue on `feature/02-playable-foundation`, same draft PR **https://github.com/Sebreiro/kingdom-zombies-game/pull/3**, targeting `dev`. The orchestrator owns pushes/PR state, shared STATE/ROADMAP/REQUIREMENTS updates and VM lifecycle. Linux and complete two-platform Phase 2 acceptance remain required; Phase 1 historical closure follow-ups and qualitative reviews remain unchanged. Plan 02-02 may build directly on these contracts and the same native scenario/driver.

## Self-Check: PASSED

Verified all fifteen listed created implementation artifacts exist; verified all five task commits are ancestors of current HEAD. Final `git diff --check` passed, and the task commit contains no tracked-file deletions. Only two pre-existing orchestrator research-cache files remain untracked; no generated task output is left untracked. Shared planning state is intentionally left to the orchestrator under the dispatch ownership contract.
