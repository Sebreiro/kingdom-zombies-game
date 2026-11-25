---
phase: 06-survivors-inventory-and-combat
plan: "18"
subsystem: build-and-permission-proofs
tags: [build-configuration, export, permissions, census, roslyn, gap-closure]
status: complete

requires:
  - phase: 06-survivors-inventory-and-combat
    provides: "06-17 R-48 renames; run 1 listed F6, F8 and F9 with their failure logs"
provides:
  - "Core defines DEBUG in ExportDebug, as the game's Godot SDK does, so ExportDebug builds and exports compile (F8)"
  - "The Phase 3 LAN namespace control clones every authored partial of the checked type (F6)"
  - "The native permission census selects exactly the 252 current guarded inspection tuples (F9)"
affects: [06-24, 06-25, phase-11-certification]

actuals:
  tokens: 2422
  tasks: 3
  commits: 4
plan_head_before: 1592daba5688b0e7d5bdd9097def7bdd3e329a01
plan_head_after: fb301ca8d65e1c9424b45f6e15a6a883f72b7a36

tech-stack:
  added: []
  patterns:
    - "A Microsoft.NET.Sdk library referenced by the Godot game mirrors the Godot SDK's ExportDebug DEBUG rule in its own project file"
    - "Semantic namespace controls clone every authored partial found through the type symbol's DeclaringSyntaxReferences, filtered to evaluated Compile inputs"

key-files:
  created:
    - .planning/phases/06-survivors-inventory-and-combat/deferred-items.md
  modified:
    - src/godot-game/core/KingdomZombies.Core.csproj
    - src/godot-game/tests/verification/ConsumerCompilation.cs
    - src/godot-game/tests/verification/FoundationLanPermissionTests.cs
    - src/godot-game/tools/asset-pipeline/cases/census.cjs

key-decisions:
  - "F8 is repaired in the Core project file only, with a DefineConstants condition for ExportDebug. Multiplayer.Abstractions has no #if DEBUG code, so the shared props stay unchanged."
  - "F6 is repaired in the proof. Production FoundationHud stays a two-file partial; the namespace control now moves both partials."
  - "F9 raises the exact census count from 251 to 252 and keeps equality. The Phase 4 evidence derivation in evidence.cjs and runner.cjs is a set mismatch (372 policy tuples against 252 compiled cases), not a count, and is deferred."

patterns-established:
  - "A proof that clones a type into another namespace must clone all of its authored partials"

requirements-completed: []

coverage:
  - id: D1
    description: "The game compiles and exports in ExportDebug; Release keeps DEBUG-only fixture code out"
    requirement: "COMBAT-03"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build (run a92f4920, PASS 1/1, 0 warnings, 0 errors)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.export.hub-consumer,asset-pipeline.export.portability,phase06.packaged (run bc38d740 before commit and tracer rerun 3b69b35d on 30e28d1, PASS 3/3 each)"
        status: pass
    human_judgment: false
  - id: D2
    description: "The LAN permission proof analyzes the complete FoundationHud and keeps 45 tuples with every negative control"
    requirement: "CHAR-04"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case lan-checks.core.permissions,gameplay-checks.core.authority (run 18e43f5c, PASS 2/2, EXACT_LAN_PERMISSIONS:45_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES)"
        status: pass
    human_judgment: false
  - id: D3
    description: "The native permission census selects and passes exactly 252 guarded inspection tuples"
    requirement: "COMBAT-08"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case asset-pipeline.content.native-permissions (run db6fc539 on clean fb301ca, asset-pipeline 114a4421, PASS 1/1, 252 selected, 252 passed, 0 skipped, 0 accepted)"
        status: pass
    human_judgment: false

duration: 175min
completed: 2026-11-25
---

# Phase 6 Plan 18: ExportDebug build, LAN partial-type proof and native census Summary

**Core now defines DEBUG in ExportDebug like the game, so the common bow ExportDebug export compiles again while ExportRelease still excludes fixture code. The LAN namespace control clones both FoundationHud partials and passes with 45 tuples. The census selects the 252 current guarded inspection tuples, including the new CharacterPreview.DrawComposition.**

## Performance

- **Started:** 2026-11-25T12:26Z
- **Completed:** 2026-11-25T15:20Z
- **Tasks:** 3 of 3
- **Files modified:** 4 source files, plus deferred-items.md

## F8: ExportDebug build (Task 1, tracer)

**Pre-repair.** The plan's direct command (`dotnet build game/KingdomZombies.csproj -c ExportDebug --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`) reported 0 warnings and 2 errors:

- `game/foundation/FoundationCoordinator.cs(481,38): error CS0117: 'FoundationSession' does not contain a definition for 'CreateObserved'`
- `game/.godot/mono/temp/obj/ExportDebug/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs(4,121): error SCR0007` (line length)

**Diagnosis.** `dotnet msbuild … -getProperty:DefineConstants -p:Configuration=…` before the repair:

| Configuration | Game | Core |
| --- | --- | --- |
| ExportDebug | `…;TRACE;EXPORTDEBUG;DEBUG` | `TRACE;EXPORTDEBUG` |
| ExportRelease | `…;TRACE;EXPORTRELEASE` | `TRACE;EXPORTRELEASE` |
| Debug | `…;TRACE;DEBUG;TOOLS` | `TRACE;DEBUG` |
| Release | `…;TRACE;RELEASE` | `TRACE;RELEASE` |

The Godot SDK's `Sdk.targets` adds DEBUG for ExportDebug ("ExportDebug also defines DEBUG like Debug does"). Core uses Microsoft.NET.Sdk, which only knows Debug and Release, so every Core member under `#if DEBUG` (FoundationSession.CreateObserved and the RunCombatFixture, RunProjections, RunWorld regions) was missing while the game's `#if DEBUG` callers compiled. The hypothesis held.

The SCR0007 error is an artifact of the direct command, not of the export. Without a runtime identifier the build writes `obj/ExportDebug/` directly, while the analyzer's GeneratedLayout list (and the export's `-r win-x64` publish) uses `obj/ExportDebug/win-x64/`.

**Repair.** `core/KingdomZombies.Core.csproj` adds `DEBUG` to DefineConstants only when Configuration is ExportDebug. After the repair:

| Configuration | Core DefineConstants |
| --- | --- |
| ExportDebug | `TRACE;DEBUG;EXPORTDEBUG` |
| ExportRelease | `TRACE;EXPORTRELEASE` |
| Release | `TRACE;RELEASE` |
| Debug | `TRACE;DEBUG` |

The game's values are unchanged. ExportRelease and Release lack DEBUG in both projects (AC-35, T-06-30).

**Post-repair.** The plan's direct command now reports 0 warnings and 1 error, the SCR0007 artifact above; CS0117 is gone. The RID-faithful ExportDebug build is the export's own `dotnet publish -c ExportDebug -r win-x64` in its source copy. It exited 0 with no `error` lines in both export cases. A direct `-r win-x64` build in the worktree would need a RID restore that rewrites the tracked `game/packages.lock.json`, so it was not run there.

## F6: LAN permission consumer compilation (Task 2)

**Pre-repair.** `lan-checks.core.permissions` (game-tests run `472e8572`, lan-checks run `022154fe`) failed with `CHILD_EXIT 2` after 4 min 44 s overall (test duration 259.5 s), with `CONSUMER_COMPILATION src/godot-game/game/foundation/FoundationHud.cs(12,47): error CS0535: 'FoundationHud' does not implement interface member 'IDisposable.Dispose()'`. The stack runs from `CheckNativeTuples` through `CheckNamespaceAndAsync` to `ConsumerCompilation.RequireValid`.

**Mechanism.** `ConsumerCompilation.Read` and `Create` were correct: the evaluated Compile items include `FoundationHud.Live.cs`. The defect was in `FoundationLanPermissionTests.CheckNamespaceAndAsync`. It cloned only the checked file into `namespace KingdomZombies.Foundation.Adjacent` and prepended one `using` line, which moves the class declaration from line 11 to line 12. That created a second, one-file `Adjacent.FoundationHud` without the `Dispose` declared in the other partial. FoundationHud is the only multi-file type among the native tuples, and its `Project` is the 45th and last tuple. The 4-minute duration was therefore the normal cost of the first 44 tuples, not a separate slowdown. The passing run took 260.5 s.

**Repair.** `ConsumerCompilation.Partials(compilation, type)` returns the other trees that declare the same type symbol, filtered to the evaluated authored Compile inputs. `CheckNamespaceAndAsync` clones the checked file and every such partial into the adjacent namespace. A new `Adjacent` helper fails closed with `ADJACENT_NAMESPACE` if a partial lacks the expected file-scoped namespace. The forged, guard-removed, wrong-path, adjacent-method and async variants still replace only their own file. The 45-tuple count, the inherited inventory and every negative control are unchanged. No test method was added, so R-48 is unaffected.

`git diff 09e2881..HEAD -- src/godot-game/game/foundation/FoundationHud.cs src/godot-game/game/foundation/FoundationHud.Live.cs` is empty.

## F9: native permission census (Task 3)

**Pre-repair.** `asset-pipeline.content.native-permissions` (game-tests run `d19ce4ca`, asset-pipeline run `735dbcc7`) failed with `NATIVE_PERMISSIONS_DISCOVERY_SELECTION`. Discovery returned 252 tests with 252 unique `uid` and 252 unique `testUid` values.

**Diff against b9aaa2b.** The `Check_PresenterNative_RequiresExactGuardedMember` InlineData set went from 251 to 252:

- Added: `src/godot-game/game/inspection/CharacterPreview.cs|KingdomZombies.Inspection.CharacterPreview|DrawComposition(JsonElement,JsonElement,JsonElement,int,JsonElement)`. Its first statement is `_gateway.RequireThread();`.
- Changed signatures of existing members, one for one: `OpenComposition(…,string)` became `OpenComposition(Node2D,MainThreadGateway,Func<string, byte[]?>,string,bool)` (first statement `gateway.RequireThread();`), and `RestoreComposition(string,string,int,JsonElement)` became `RestoreComposition(string,string,int,JsonElement,string?,JsonElement,IReadOnlySet<string>?)` (first statement `_gateway.RequireThread();`).

CharacterPreview.cs and ContentNativePermissionTests.cs are both in the 06-REVIEW reviewed file list. No unexpected tuple was found.

**Repair.** census.cjs requires exactly 252 tests, 252 unique uids and 252 unique testUids. Equality is kept. `grep -c "tests.length===252" src/godot-game/tools/asset-pipeline/cases/census.cjs` returns 1.

**Post-repair.** On the clean committed tree `fb301ca` (game-tests `db6fc539`, asset-pipeline `114a4421`) the case passed: discovery selected 252 cases, all 252 executed and passed in their own xUnit child, 0 were skipped and 0 obligations were accepted. The log contains no NATIVE_PERMISSIONS_DISCOVERY_SELECTION and no skipped cohort. The run took 65.8 min, which is about 15 s per child.

## Task Commits

1. **Task 1 (tracer): Core defines DEBUG in ExportDebug.** `30e28d1` (fix)
2. **Task 2: the LAN namespace control clones every authored partial.** `c017bc6` (test)
3. **Task 3: the census selects the 252 current tuples.** `f183b4a` (test)
4. **Deferred item record.** `fb301ca` (docs), committed before the clean census rerun so that the tested tree held it

## Verification runs

| Case | Run | Outcome |
| --- | --- | --- |
| Direct ExportDebug build, before repair | stdout only | exit 1: CS0117 (FoundationCoordinator.cs 481,38) and SCR0007 (RID-less AssemblyAttributes path) |
| Direct ExportDebug build, after repair | stdout only | exit 1: SCR0007 only (RID-less path artifact); CS0117 gone |
| basic.strict-build | game-tests `a92f4920` | PASS 1/1, 0 warnings, 0 errors |
| export.hub-consumer, export.portability, phase06.packaged | game-tests `bc38d740` (asset-pipeline `bb506da5`, `a5f3bf42`) | PASS 3/3; both ExportDebug `-r win-x64` publishes exit 0 with no errors; phase06.packaged ExportRelease publish exit 0 |
| Same three, tracer gate on committed `30e28d1` | game-tests `3b69b35d` (asset-pipeline `97b71955`, `914f36af`) | PASS 3/3 |
| lan-checks.core.permissions, before repair | game-tests `472e8572`, lan-checks `022154fe` | FAIL CHILD_EXIT 2, CONSUMER_COMPILATION CS0535 |
| lan-checks.core.permissions, gameplay-checks.core.authority | game-tests `18e43f5c` (lan-checks `2befb218`, gameplay-checks `bf7cbf8a`) | PASS 2/2, EXACT_LAN_PERMISSIONS:45_TUPLES_8_PUBLIC_TYPES_WITH_VALID_NEGATIVES |
| content.native-permissions, before repair, no pwsh on PATH | game-tests `6d96d086` | FAIL `spawn pwsh ENOENT` (environment) |
| content.native-permissions, before repair | game-tests `d19ce4ca`, asset-pipeline `735dbcc7` | FAIL NATIVE_PERMISSIONS_DISCOVERY_SELECTION, 252 discovered |
| content.native-permissions, after repair | game-tests `a19c76b1` (asset-pipeline `4c3baeee`) | case PASS, 252/252, 0 skipped, then the runner failed it with "Source changed during execution" because the SUMMARY draft and deferred-items.md were written during the run (see deviation 4) |
| content.native-permissions, after repair, clean tree | game-tests `db6fc539` on `fb301ca`, clean (asset-pipeline `114a4421`) | PASS 1/1, 252 selected, 252 passed, 0 skipped, 0 accepted, 65.8 min |

The export runs before the Task 1 commit record revision `1592dab` with `clean: false`; the tracer rerun `3b69b35d` tested the committed `30e28d1`. Task 2's run tested the uncommitted change that `c017bc6` commits byte for byte.

## Deviations from Plan

### Environment

**1. [Rule 3 - Blocking] `pwsh` was not on the Git Bash PATH**
- **Found during:** Task 3 reproduction.
- **Issue:** the discovery step spawns `pwsh` and failed with `spawn pwsh ENOENT`. PowerShell 7 is not installed system-wide on this host.
- **Fix:** no install. The existing PowerShell 7.6.9 binary at `C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe` (Authenticode Valid, signed by Microsoft Corporation, the version setup.md lists as verified) was prepended to PATH for the census runs only. No repository file changed.

### Scope notes

**2. Direct ExportDebug build criterion.** The plan expected the direct command to report 0 warnings and 0 errors. After the repair it reports one SCR0007 error on the RID-less `obj/ExportDebug/` AssemblyAttributes path, which the analyzer's GeneratedLayout list does not cover and the export never produces. The export's RID build is clean. Adding that path to the analyzer allowlist would change the Phase 1 rule suite's inputs and is not needed for any shipped build, so it was not done.

**3. Sibling pins of the guarded tuple set not rebound (deferred).** `evidence.cjs guardedHostTuples` (`tuples.length === 251`) and `cases/runner.cjs artifactControls` (`assert.equal(guarded.length,251)`) derive from `EngineAccessPolicy._presenters`, which Phase 6 grew from 251 to 372 tuples (120 foundation presenters, 1 new and 2 re-signatured inspection presenters). They compare that whole set with the 252 compiled inspection cases, so rebinding to 252 would still fail. A trial edit was reverted. Recorded in [deferred-items.md](deferred-items.md); it affects `asset-pipeline.acceptance.artifacts` and `acceptance.artifacts-source`, which no 06-16..06-25 plan currently lists.

### Process

**4. One invalidated census run.** The first post-repair run (`a19c76b1`) passed the case 252/252, but I wrote the SUMMARY draft and deferred-items.md into the worktree while it ran. The runner's source check (git status plus untracked files) then failed the run with "Source changed during execution", as designed. Task 3 and the deferred record were committed, the draft moved out of the worktree, and the case was rerun on the clean tree (`db6fc539`, pass). Nothing was retried or masked: the invalidated run stays as a failure in `.local/runs`.

## Issues Encountered

None beyond the deviations above.

## Known Stubs

None.

## Next Phase Readiness

F6, F8 and F9 are closed. The deferred evidence derivation needs an owner before phase verification. The 06-24 items (the `ac-08 derivation` binding test and the F3 REPOSITORY_SOURCE_HASH cases) are unchanged.

## Self-Check: PASSED

All four modified source files, deferred-items.md and this summary exist. Commits 30e28d1, c017bc6, f183b4a and fb301ca are ancestors of HEAD. `grep -c "tests.length===252"` on census.cjs returns 1.
