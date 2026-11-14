---
phase: 01-code-rules-and-documentation
plan: "12"
subsystem: native-engine-lifetimes-and-boundaries
tags: [godot, csharp, lifecycle, exports, signals, main-thread, provenance, tdd]
requires:
  - phase: 01-11
    provides: Engine-independent immutable Core state, strict snapshots and atomic restoration
provides:
  - Two actual native scene generations with engine-owned callback ordering and synchronous teardown
  - Real serialized Resource and exported-value validation before use
  - Captured-thread queue delivery and independent closed-generation fencing
  - Fresh complete per-cycle native surface evidence and semantic adapter checks
  - Exact authenticated generated diagnostic spans with handwritten and impostor boundaries
affects: [01-13, 01-14, 01-15, 01-16, 01-17, future-godot-integration]
tech-stack:
  added: []
  patterns: [guarded synchronous engine adapters, frame-driven result dispatch, generation fence before cancellation, exact generated source permissions]
key-files:
  created:
    - src/godot-game/proofs/MainThreadGateway.cs
    - src/godot-game/proofs/LifetimeToken.cs
    - src/godot-game/proofs/LifecycleFixture.cs
    - src/godot-game/proofs/ProofSequence.cs
    - src/godot-game/proofs/ProofResource.cs
    - src/godot-game/proofs/ProofResource.tres
    - src/godot-game/proofs/ValidFixture.tscn
    - src/godot-game/proofs/InvalidFixture.tscn
    - tests/phase1/EngineRun.cs
    - tests/phase1/EngineEvidenceTests.cs
    - tests/phase1/EngineEvidencePolicy.cs
    - tests/phase1/EngineBoundaryTests.cs
    - tests/phase1/EngineCaseTests.cs
    - tests/phase1/EngineGeneratedTests.cs
    - tests/phase1/EngineAccessPolicy.cs
    - tools/code-rules/EngineGeneratedSyntax.cs
    - docs/code/engine-generated-spans.json
  modified:
    - src/godot-game/proofs/Phase1.cs
    - tests/phase1/TypeBoundaryScanner.cs
    - tests/phase1/ExemptionTests.cs
    - tests/phase1/ProofProtocol.cs
    - tools/code-rules/GeneratedSyntax.cs
    - tools/code-rules/NamedArgumentAnalyzer.cs
    - tools/code-rules/LineLengthAnalyzer.cs
    - tools/code-rules/MemberOrderAnalyzer.cs
    - docs/code/exemptions.json
    - docs/code/register.json
    - docs/code/generated-callsite-adaptations.json
    - docs/code/generated-layout-adaptations.json
    - tools/phase1/required-cases.json
key-decisions:
  - Capture engine thread identity at startup and route pure worker results through guarded real-frame dispatch; close the scene-generation fence before cancellation and disconnection.
  - Keep nullable staging until engine injection; check required Resource and nullable value before use while deliberately optional null remains valid.
  - Authorize only exact synchronous adapter methods with an authentic preceding top-level guard; branch, worker, local-function and async continuation contexts cannot borrow a parent-thread guard.
  - Preserve original suppressions and authenticate 170 exact generated diagnostic/member/source/path/span identities; the sealed script needs no added type-level exemption.
requirements-completed: []
requirements-contributed: [RULE-03, RULE-06, RULE-07]
coverage:
  - id: D1
    description: Actual two-instance lifecycle, generated signal binding, teardown and worker lifetime/thread boundaries
    requirement: RULE-06
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case engine.lifecycle
        status: pass
    human_judgment: false
  - id: D2
    description: Actual serialized resource/export loading and required-null rejection before use
    requirement: RULE-06
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case engine.exports
        status: pass
    human_judgment: false
  - id: D3
    description: Complete fresh engine evidence, exact generated/source boundaries and independent Core state
    requirement: RULE-07
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --group engine
        status: pass
      - kind: integration
        ref: node tools/phase1/check.cjs --group state
        status: pass
      - kind: integration
        ref: .phase1/runs/8a7abb86-a8dc-40ba-92c4-02e355c80821/summary.json
        status: pass
    human_judgment: false
actuals:
  tokens: 84771
  tasks: 3
  commits: 7
commits: 7
plan_head_before: 1d5ec9f21aaf42ee3414a266739b75d055b04123
plan_head_after: cd453e6cabcbc41bf8fb5fcf72897e467b8b8d89
duration: 154min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 12: Actual Godot lifetimes, exports, signals and thread access Summary

**Two real Godot scene generations now prove callback order, serialized initialization, exactly-once signals, native teardown and guarded worker delivery, with 609 accepted controls and zero failed controls.**

All three tasks are complete on the actual Windows target. Phase 1 remains incomplete; all shared RULE requirements remain Pending. Identical-source Linux execution, measured coverage, focused documentation and shared closure retain their later plan owners. No gameplay, menu, movement, LAN, additional repository project or Phase 2 implementation was introduced.

## Final verification

The replacement immutable `node tools/phase1/check.cjs --all` run reconciled **609 accepted controls, zero failed controls and 26 IMPLEMENTATION_PENDING obligations out of 635 declared cases**. Actual exit **1** is solely the required later-owner pending work. Pending obligations are not passes. All 19 engine obligations, all 33 state obligations, all 26 graph obligations and every other implemented control were accepted on the final fixed inputs.

- [Canonical full report](../../../.phase1/runs/8a7abb86-a8dc-40ba-92c4-02e355c80821/summary.json).
- [Complete console output](../../../.phase1/temp/12-reconciled-all.log) and [actual exit](../../../.phase1/temp/12-reconciled-all.exit).
- Tested revision: `cd453e6cabcbc41bf8fb5fcf72897e467b8b8d89`.
- Input identity: `6cd46155d8cf7eb13ca89a8f2d651a09f3f3fa2c44c9d9d95de6f83139ebae20`.
- Tool identity: `3bde88b19b3a1929b61e0cfa4fff6908bfbcbbd8f9b20cda9678a6c6d6e2f5fa`.
- Actual platform: `win32-x64-10.0.19044`; pinned SDK 10.0.404, runtime 10.0.15 and Godot 4.7.2.
- Actual command interval: `2026-11-14T09:19:49.018Z` through `2026-11-14T10:05:35.199Z` (45m46s). Fingerprinted source, documentation, specifications and configuration remained fixed. Git was clean after reconciliation.

Prerequisites and focused results:

- Strict whole `KingdomZombies.slnx` build with `--no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`: zero warnings/errors, including after the final fixture repair.
- Canonical lifecycle: 1/1 accepted, nonce `6feba8de-7815-422c-8a30-2c285f4b2508`; tracer rechecked before export expansion. Canonical exports: 1/1 accepted, nonce `f4025432-85cb-40c4-a1d7-27794e25b5c7`.
- Final focused boundaries: 7/7 passed, [JUnit report](../../../.phase1/temp/12-final-boundaries.xml), including authentic-guard positives, branch/worker/await negatives, actual compiler subjects and all generated member/span identities.
- Canonical engine: 19/19 accepted, nonce `944b1529-ea32-4fbc-9700-f5c8ad8180ab`.
- Canonical state: 33/33 accepted, nonce `295391b9-be0d-4831-9943-633d6bb5ac04`.
- Canonical graph: 26/26 accepted, nonce `57a284fb-9692-443e-85af-e0addb30b17a`.
- Original native bootstrap contract passed, nonce `48086386-2b57-49ac-a841-15689f87b8a6`. It remains raw compatibility evidence and does not claim Linux acceptance.
- Register, naming/binding and inherited generated-origin/boundary preflight: 45 passing controls; the two analyzer-source failures identified one new helper indentation issue. After correction, both analyzer-source controls passed 2/2. Exact tracked-source formatting, five LF locks/locked restore and suppressions passed 3/3.
- Final inherited companion repair: [3/3 passed](../../../.phase1/temp/12-inherited-companion-fix.xml), with clean baseline, exact intended compiler diagnostic and unchanged genuine generated identities.
- `git diff --check` passed. No baseline tracked file was deleted and no policy severity, dependency pin, donor input or installed tool was weakened or changed.

## Delivered behavior

The existing `Phase1.tscn` remains the sole launch scene. `Phase1` retains its nonce-tagged construction/ready/exit bootstrap and engine/runtime identity output. Its actual process callback advances `ProofSequence`; the driver loads a `PackedScene`, instantiates the scene, adds it to the real tree, waits for real process frames, removes it and synchronously frees it. It never invokes lifecycle callbacks manually.

Two fresh parent/child scenes each require exactly two genuine parameterless fixture constructions. Native instance IDs must be nonzero and distinct between cycles. Both cycles require the exact observed order `Parent:enter`, `Child:enter`, `Child:ready`, `Parent:ready`, `Child:exit`, `Parent:exit`. A real parent process callback is required. Frame delta is not forwarded as authoritative Core time. Native root validity must be false after `Free`.

`LifecycleFixture.PulseEventHandler` declares a real Godot signal. The fixture explicitly subscribes and unsubscribes its handler, emits through the generated signal name and native `EmitSignal`, and requires exactly one intended handler and the expected live value in each generation. Generated binding is exercised by native signal dispatch and serialized property injection.

`MainThreadGateway` captures startup thread identity, rejects an incorrect thread before invoking a supplied operation and owns a concurrent queue of pure results. A worker operation must return `THREAD_ACCESS` while its operation sentinel remains zero; the proof deliberately performs no unsafe engine API call off-thread. Supported worker results are applied only through the guarded real-frame queue drain.

Each scene has an independent `LifetimeToken`. Its acceptance fence closes before cancellation and explicit signal disconnection. The first pure worker result is queued before teardown and retained behind a controlled barrier. The first scene is unloaded, a distinct second scene becomes live, and only then is the old result released. Both generations' values and handler counts must remain unchanged. The worker completes without depending on cancellation, so cancellation alone cannot satisfy the fence assertion. Live-generation work then applies the expected value on the captured main thread. Cycle waits are bounded by five seconds and each native process by 45 seconds, with process-tree cleanup on timeout.

The real serialized `ProofResource.tres` supplies `Identity = "native-proof"` and `Amount = 13`; `ValidFixture.tscn` assigns that resource and value 37 through exports. Nullable reference staging remains until native injection. The serialized `Variant` value is checked into an explicit nullable `int` staging contract; readiness returns checked non-null reference/value data before use. A deliberately optional resource stays null and is accepted. Genuine native resource construction and checked loaded values are required.

Three distinct bounded native driver modes exercise missing required reference, missing required value and explicit invalid null. The missing-reference scene actually omits its export. The other controlled options clear the valid scene's required staging value/reference before readiness. Each rejection requires native exit 2, `EXPORT_REQUIRED` naming the exact member, the actual scene path and invocation nonce, and `uses:0`. No successful surface set may be published for invalid initialization.

Every successful native run publishes 21 unique nonce-tagged records only after its assertions and cleanup: construction, callbacks, process, signals, teardown, resource, exports, nullable validation and binding for each of two cycles, plus late-work, prohibited-worker and supported-main-thread assertions. Records carry actual instance and thread identities. The bounded evidence policy rejects missing, duplicate, stale or unknown records, inconsistent instance identities, invalid thread identities and exit-zero-without-assertions. Integrity tests remove each actual surface individually and exercise duplicate and stale evidence. Raw native logs are included as hashed result artifacts, independently for each required case.

`EngineAccessPolicy` resolves actual C# engine symbols against the complete handwritten/generated declaration context. It permits only exact adapter file/type/method identities with the authentic `MainThreadGateway.RequireThread` call as a preceding direct top-level statement. A guard in an untaken branch or after the operation cannot authorize access. A parent-method guard cannot authorize engine work in a worker lambda or local function. Async engine operations are rejected, including a top-level guard before `await ... ConfigureAwait(false)`; actual engine continuation uses the guarded frame/queue boundary. This policy is separate from the existing pure-library ConfigureAwait requirement. A real compiled handwritten `Node.QueueFree` subject is rejected at `Subject.cs(7,9)` after both compiler controls succeed.

Pure Core state and snapshot operations execute in the plain .NET proof host while the loaded-assembly inventory contains no `GodotSharp`. Compiled metadata still forbids engine dependencies in Core. Metadata inspection never instantiates an engine object outside native Godot.

## Generated source treatment

Actual pinned compiler diagnostics established 170 precise additional generated permissions. Each records diagnostic, exact member/symbol, producer/package identity, raw and Roslyn-text hashes, source path/suffix, span, reason and positive/negative proofs. The compiled analyzer table matches exact immutable text and span identities; it grants no filename-marker, folder or namespace permission. Existing observed call-site/layout hashes were reconciled for the expanded genuine callback binding.

Every permission has authentic emitted-output and package checks plus changed-source, changed-path and adjacent-span negatives. Actual compiler controls prove the compliant handwritten companion, its exact `LifecycleFixture.cs(6,60)` SCR0001 violation and a fake generated marker's exact `Subject.cs(7,32)` SCR0001 violation. The handwritten partial remains fully analyzed. The existing eighteen exact suppressions are preserved.

An actual sealing experiment corrected an early assumption: the pinned generator changes its signal helper from protected to private for a sealed script. The final fixture is therefore sealed and requires no added CA1852 type exemption. Task 3 deliberately removed the interim `GeneratedLifecycleAdapters.cs` created earlier in this plan; no baseline file was removed. The final generated source, register and exemption metadata agree in the same tested revision. No blanket suppression or null-forgiving workaround was introduced.

The generator APIs were checked against the installed SDK/XML and official [C# signal documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html) and [export documentation](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html). Actual pinned compilation and native execution establish the behavior used here.

## TDD evidence and commits

| Task | RED | GREEN | Actual intended failure |
| --- | --- | --- | --- |
| 1: Native lifecycle tracer | `4647a09` | `ae871dc` | Engine launched and exited zero; the unchanged target rejected missing cycle-1 construction evidence. |
| 2: Serialized export contract | `602132d` | `0157b69` | Strict build and real lifecycle succeeded; the target rejected missing cycle-1 resource evidence. |
| 3: Semantic gateway boundary | `8441dba` | `8d8cc32` | Both copied clean and handwritten engine-call subjects compiled; the target rejected the scaffold's empty diagnostic list for actual `Subject.cs(7,9)` access. |

All three unchanged supported JUnit reports and semantic assessments are retained under `.phase1/temp/12-task{1,2,3}-red.xml/json`, classified `RED_EVIDENCE_OK` before implementation. Setup/compiler/style failures received no RED credit. A first Task 3 subject hit unrelated MA0048; its report was explicitly assessed invalid and preserved as `12-task3-infrastructure.xml/json`, then replaced by a genuine semantic RED run. One redundant tracer invocation correctly rejected concurrent source editing with `SOURCE_CHANGED`; it is not acceptance evidence. Required valid canonical results and the final immutable full run remain independent.

**TDD Gate Compliance:** all three tasks have ordered genuine RED/GREEN commits. The tracer was verified before export expansion. No skipped target, zero-test selection, unrelated fixture failure or classifier self-attestation was used as RED or GREEN evidence.

`cd453e6` is the verified inherited-fixture repair described below. The disk ledger measures **seven** source/proof commits from `1d5ec9f21aaf42ee3414a266739b75d055b04123` through `cd453e6cabcbc41bf8fb5fcf72897e467b8b8d89`. The realized net diff is 339083 characters across 30 files; ceil(chars/4) is **84771** on the estimate's required scale. The measured execution window is **154min** before planning-metadata closure, including both immutable full runs and the scoped repair. Metadata commits are additional.

## Deviations from Plan

**[Rule 1 - Bug] Retargeted the inherited handwritten companion mutation.** Moving immediate shutdown out of `_Ready` made one existing helper's `GetTree().Quit()` text replacement inert. The first immutable full run correctly reported 607 accepted controls, two failed controls (`ac-08.complete`, `rules.godot-companion-negative`) and 26 pending obligations. Its [unchanged diagnostic report](../../../.phase1/runs/2ec51cda-3dc8-4748-9f96-b9ab2ace14c5/summary.json) remains preserved; it is not a successful final run.

After collecting every independent result, `ExemptionTests.VerifyGameCompanion` was adapted to the unique current `WriteEvidence("ready")` statement and the real lowercase `proofs` path. It asserts exactly one anchor, preserves the clean actual compiler baseline and authentic generated hashes, and still requires the intended SCR0001 at the computed source line and column 29. All three affected controls passed before repair commit `cd453e6`. The fresh replacement full run then accepted all 609 implemented controls without source changes. No control was aliased, dropped or weakened.

The cohesive driver, evidence, semantic and generated-source helpers and exact manifest/register wiring are within the approved scoped-helper allowance. No architecture or package change was needed. There are no deferred owned issues, skipped tests or unrun required verification commands.

## Remaining phase obligations

The 26 required later-owner obligations remain: platform 1, register 1, rules 12, documentation 7 and pins 5. Plans 01-13 through 01-17 retain focused documentation, shared register/proof closure, measured coverage, clean checkout and actual Linux execution. The five older pins runner-edge identities remain distinct and retain their 01-14 closure ownership; WINDOWS entry 9 remains open. Historical bootstrap missing-RED WINDOWS entry 1 remains open. All shared RULE requirements remain Pending, and separate qualitative review remains Pending.

The stub scan found no unfinished owned behavior. Empty observation collections, nullable pre-injection staging, teardown nulls and the test host's pre-run strings are exercised initialization states; actual native data and result logs are required before acceptance. No new open broken-window entry is needed. No additional network endpoint, auth path, production storage or unmodeled trust boundary was introduced. All five projects, donor originals, installed tooling, prepared Linux VM and captured Phase 2 context were preserved. No Linux or measured-coverage pass is claimed.

## Next action

Continue Phase 1 with 01-13 focused documentation and exact portable navigation. Keep the Phase 2 context/session annotation intact and leave every shared RULE requirement Pending until the remaining phase owners close their obligations.

## Self-Check: PASSED

All seventeen created artifacts and this SUMMARY exist. All seven source/proof commits are ancestors of HEAD, and the persisted ledger measures seven commits. The final canonical report agrees with 609 accepted controls, zero failed controls and exactly 26 later-owner pending obligations. All required commands were executed and no owned stub or deferred implementation remains. The original diagnostic run and every genuine RED report remain available for inspection.
