---
phase: 01-code-rules-and-documentation
plan: "11"
subsystem: strict-snapshots-and-atomic-restoration
tags: [csharp, net10, strict-json, immutable-state, atomic-restore, tdd]
requires:
  - phase: 01-10
    provides: Real immutable state, exact ordinal identities and pinned bounded random continuation
provides:
  - Bounded schema-1 strict UTF-8 snapshots of the actual detached Core model
  - Exact pinned enum wire names, required values and rejection before semantic record conversion
  - Nonblocking writer ownership and one atomic immutable-root publication
  - Actual compiled early-record mutation proof and controlled concurrent-reader/cancellation proofs
affects: [01-12, 01-14, 01-15, 01-16, 01-17, future-state-contracts]
tech-stack:
  added: []
  patterns: [strict System.Text.Json parsing, detached immutable DTOs, ordinal wire names, atomic root exchange]
key-files:
  created:
    - src/core/SnapshotCodec.cs
    - src/core/SnapshotDto.cs
    - src/core/SnapshotJson.cs
    - src/core/SnapshotWriter.cs
    - src/core/ExampleStateStore.cs
    - tests/phase1/SnapshotTests.cs
    - tests/phase1/SnapshotProofData.cs
    - tests/phase1/SnapshotShapePolicy.cs
    - tests/phase1/RestoreAtomicityTests.cs
    - tests/phase1/RestoreConcurrencyProof.cs
    - tests/phase1/RestoreMutationProof.cs
  modified:
    - src/core/ExampleState.cs
    - tests/phase1/ArchitectureRuleTests.cs
    - tools/phase1/required-cases.json
    - docs/code/register.json
key-decisions:
  - Use the existing JsonStringEnumMemberName("sample") as the exact ordinal wire contract; reject Sample, Unknown and all numeric enum input.
  - Bound the shared strict parser to 1048576 UTF-8 bytes, depth 16 and 1024 records/ordered identities, with inclusive limits and no normalization.
  - Reject competing restores with RESTORE_BUSY; publish one complete immutable root only after detached validation and the precommit cancellation fence.
requirements-completed: []
requirements-contributed: [RULE-03, RULE-07]
coverage:
  - id: D1
    description: Exact detached round trips, required schema, integer/identity/order fidelity and strict boundary neighbors
    requirement: RULE-07
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case state.snapshot
        status: pass
    human_judgment: false
  - id: D2
    description: Atomic accepted roots, exact unchanged roots on failure, explicit competition and actual early-record mutation detection
    requirement: RULE-07
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --group state
        status: pass
      - kind: integration
        ref: .phase1/runs/8d339b5a-3d33-4b9f-aed8-a36c59ff2cd8/summary.json
        status: pass
    human_judgment: false
actuals:
  tokens: 22714
  tasks: 2
  commits: 4
commits: 4
plan_head_before: 1dd026afc6ff1765d399be5e960ada188f11632b
plan_head_after: 75df62b69becb98be9742f27a498430f9a127ab8
duration: 68min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 11: Strict snapshots and atomic restoration Summary

**The real Core proof state now round-trips through a bounded strict JSON contract and restores through one atomic immutable-root exchange, with 590 accepted fresh controls and no failed controls.**

Both tasks are complete. Phase 1 remains incomplete and all shared RULE requirements remain Pending. This internal non-game example contributes to RULE-03/RULE-07; it does not implement production save files, inventory, simulation, networking or later-phase gameplay.

## Final verification

The single immutable final `node tools/phase1/check.cjs --all` run reconciled **590 accepted controls, zero failed controls and 35 IMPLEMENTATION_PENDING obligations out of 625 declared cases**. Actual exit **1** is solely the required later-owner pending work. Pending cases are not passes. All 33 state-group obligations, all ten R-68 through R-72 polarity controls, the actual production DTO/model checks and all inherited implemented controls were accepted in this run.

- Canonical report: `.phase1/runs/8d339b5a-3d33-4b9f-aed8-a36c59ff2cd8/summary.json`.
- Console/actual exit: `.phase1/temp/11-final-all.log` and `.phase1/temp/11-final-all.exit`.
- Tested revision: `75df62b69becb98be9742f27a498430f9a127ab8`.
- Input identity: `e97a0b4d6e5e0c12a6c13ff540a966c4ac346a5eb78cdc11705ea093a7e50b93`.
- Tool identity: `e7a582942e23009be3c14c8eb97d8a1f4f5fd7b9ebc8378494d8fe978f510aaa`.
- Actual platform: `win32-x64-10.0.19044`; pinned SDK 10.0.404, runtime 10.0.15, Godot 4.7.2.
- Actual command interval: `2026-11-14T06:40:02.487Z` through `2026-11-14T07:23:23.919Z` (43m21s). Fingerprinted source/docs/spec/config bytes remained fixed. Checkout was clean after reconciliation.

Prerequisites and focused evidence:

- Strict whole `KingdomZombies.slnx` build with `--no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`: zero warnings/errors.
- Final focused snapshots: 19/19 passed, `.phase1/temp/11-task1-final.xml`; focused restoration: 8/8 passed, `.phase1/temp/11-task2-green.xml`.
- Canonical tracer accepted 1/1 twice, including post-GREEN recheck `f621fc39-545f-45ce-ab13-3de87274299e` before task 2 expansion. Final full run independently rechecks the final pinned lowercase wire contract.
- Final `--group state`: 33/33 accepted, nonce `1938d12f-b810-487d-b3a6-ebe86eebcbea`; no empty selection or pending state case remains.
- Unique test names/bindings and register mutations: 37/37 passed, `.phase1/temp/11-preflight.xml`.
- Final register mutations, actual production DTO mutation, scoped banned inputs and all twelve analyzer source controls: 40/40 passed, `.phase1/temp/11-final-prerequisites.xml`.
- Exact five canonical LF locks/locked restore, source formatting, actual compiled graph and production model/enum mutation checks: 12/12 passed, `.phase1/temp/11-architecture-locks.xml`.
- Actual pinned native Godot compatibility probe passed, nonce `b28359cf-203b-457f-a1f1-427cec935ea2`; this is Windows raw compatibility evidence, not complete lifecycle or Linux acceptance.
- `git diff --check` passed; no tracked file was deleted. No policy, suppression, diagnostic severity, dependency pin or generated permission was weakened.

## Delivered behavior

`SnapshotCodec` owns one exact camelCase schema: `schemaVersion`, `elapsedMilliseconds`, `seed`, `drawPosition`, `randomContractVersion`, `kind`, `ordered`, and `records`; each record requires exactly `id` and `value`. Schema version is 1. The codec returns an actual detached `ExampleState`, preserving all fields, exact Int64 values including values above 2^53 and long.MaxValue, ordered sequence, unordered identity/value semantics and next random continuation. The source root never changes during a round trip.

The existing `ProofKind.Sample` member and numeric value remain unchanged. Its existing `[JsonStringEnumMemberName("sample")]` attribute pins the wire name **sample**. A compiled-member attribute assertion verifies the actual pin; positive and negative wire-name proofs accept only that exact ordinal string and reject Sample, other case variants, surrounding whitespace, Unknown, numeric strings, numeric tokens, null and future names. The initial implementation used the C# identifier; final implementation, register reasons and accepted full evidence consistently use the existing attribute contract.

`SnapshotJson` validates actual UTF-8 before parsing. It rejects malformed/truncated encodings and JSON, comments/trailing commas, absent/null/non-object roots, missing/null required fields, unknown properties and duplicate property names at all object depths. It bounds documents to **1048576 UTF-8 bytes inclusive**, parser nesting to **16 containers inclusive**, and each identity collection to **1024 entries inclusive**. Tests cover 1048575/1048576/1048577 bytes, parser depths 15/16/17, and parser/model collection counts 1023/1024/1025. Nested syntactically valid arrays within the depth cap still fail the required root schema. The complete schema is never relaxed to exercise the parser boundary.

The reader validates exact integer tokens without floating-point conversion, then validates identity text, required record fields, values, duplicates and version/range rules before any candidate can return. Duplicate or conflicting IDs fail explicitly rather than choosing a hidden winner. Exact ordinal IDs preserve case, whitespace, composed/decomposed Unicode and supplementary characters; empty/control/unpaired-surrogate and length-overflow identities fail. Tests exercise lengths 127/128/129 through the real codec boundary. Both collections are required even when empty. Unordered randomized record permutations retain semantic equality; reversed meaningful identity order stays reversed through another round trip.

`SnapshotDto` contains detached immutable primitive/value collections. Compiled property/backing-field checks require the actual Core state, identity and DTO shapes and reject arbitrary references or mutable collection shapes. R-64's existing real-build mutation proof now also compiles and inspects the actual production DTO. Engine references, transient peer IDs and network addresses have no accepted schema representation.

`SnapshotWriter` and the shared strict parser are small cohesive helpers behind the single codec. System.Text.Json's explicit depth and integer APIs were checked against Microsoft documentation; actual pinned-runtime tests establish the behavior used here ([MaxDepth](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsondocumentoptions.maxdepth?view=net-10.0), [Int64 JSON access](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.jsonelement.getint64?view=net-10.0)). No additional package was acquired.

`ExampleStateStore` holds one immutable root. Each read acquires one `Volatile.Read` reference. A nonblocking compare/exchange gate rejects a second writer with **RESTORE_BUSY** while preparation is owned. Restore parses/validates a complete detached candidate, runs the controlled precommit seam, checks cancellation immediately before publication, then performs one `Interlocked.Exchange`. The gate releases in `finally`, including invalid, cancelled and interrupted preparation. The atomic operation is the documented reference exchange ([Interlocked.Exchange](https://learn.microsoft.com/en-us/dotnet/api/system.threading.interlocked.exchange?view=net-10.0)).

Restore never advances time/randomness or invokes domain creation/effect operations. Repeating accepted state preserves exact IDs, elapsed time, seed, position, contract and next draw without append/replay/reroll. Invalid-after-valid input, pre-cancelled input, cancellation at the precommit seam and interrupted preparation preserve the exact former root reference and all values. Controlled barriers run three isolated stores with four concurrent readers each; complete cross-field fingerprints require every observed root to be entirely old or entirely new. Busy writers are rejected while the first writer is paused, and readers retain complete root references.

The mutation proof is an actual isolated implementation mutation: a copied `ExampleStateStore.Restore` publishes a root containing the first accepted record's value while retaining the old elapsed/seed/position/order fields **before** the precommit seam. The copied project must restore/build and the unchanged target must pass first. The mutated project must also compile cleanly. Re-running the unchanged interrupted-preparation test must then fail its **ATOMIC_ROOT_UNCHANGED** assertion in a real fresh one-test JUnit report, with no error/skipped substitute. The full runner accepted this mutation detector. Child proof environment variables are removed, so its reports cannot satisfy or overwrite another invocation's evidence.

## TDD evidence and commits

| Task | RED | GREEN | Actual evidence |
| --- | --- | --- | --- |
| 1: Strict detached round trip | `556c585` | `1bed129` | `.phase1/temp/11-task1-red.xml/json`: strict whole build succeeded; exact target executed once and failed expected elapsed 9007199254740993 versus zero from the decoder scaffold. RED_EVIDENCE_OK. |
| 2: Accepted atomic publication | `c46ba08` | `75df62b` | `.phase1/temp/11-task2-red.xml/json`: strict whole build succeeded; exact target executed once and failed expected candidate elapsed 91 versus old root 7. RED_EVIDENCE_OK. |

Both unchanged supported JUnit reports and semantic assessments are retained. Compiler/style/cancellation-analyzer setup failures received no RED credit. One initial focused mutation invocation lacked the explicit `PHASE1_ROOT` required by the existing fixture host; rerunning with that invocation environment passed. These infrastructure events were not counted as subject failure or acceptance. Task 1's tracer was verified after GREEN and before task 2 expansion. Both tasks have genuine ordered RED/GREEN commits; no TDD gate violation or extra refactor commit occurred.

The persisted ledger measures four production/proof commits from `1dd026afc6ff1765d399be5e960ada188f11632b` through `75df62b69becb98be9742f27a498430f9a127ab8`. Realized diff length is 90853 characters; actual estimate-scale cost is ceil(chars/4) = **22714**, across 15 changed files. Recorded duration **68min** rounds up the observed first RED execution through final reconciliation; initial reading and metadata closure are additional.

## Deviations from Plan

None outside the approved scope. Cohesive parser/writer/test helpers, exact case-manifest bindings and actual production DTO checks use the explicitly permitted helper/manifest/register wiring. R-70 is deliberately adapted as required: authoritative internal snapshots reject unknown/null/absent required data to prevent silent loss, unlike the captured donor's permissive HTTP defaults. R-68/R-69 record the actual enum attribute contract. Qualitative review remains separately Pending; machine execution does not complete human review or shared RULE requirements.

## Remaining phase obligations

The 35 required later-owner obligations remain: platform 1, register 1, rules 12, documentation 7, engine 9, pins 5. Plans 01-12/13/14/15/16/17 retain lifecycle, focused documentation, shared binding closure, measured coverage, clean checkout and actual Linux execution. The five older pins runner-edge identities retain their 01-14 closure ownership; WINDOWS entry 9 remains open. Historical bootstrap missing-RED WINDOWS entry 1 remains open. No new owned stub, skipped test or unrun verification was left behind, so no new broken-window entry is required.

All five repository projects, immutable donor inputs, installed `.codex`, the prepared Linux VM and the captured Phase 2 context were preserved. No actual Linux or measured-coverage pass is claimed. The random contract remains pinned to runtime 10.0.15 and requires later actual Linux execution of the same independent vectors. This introduces only the already planned in-memory snapshot parsing boundary; no network endpoint, auth path, production file storage or new unmodeled trust surface was added.

## Next action

Continue Phase 1 with 01-12 actual Godot lifecycle/export/signal/thread proofs. Keep Phase 2 unexecuted and all shared RULE requirements Pending until their remaining phase obligations close.

## Self-Check: PASSED

All eleven created implementation/proof files and this SUMMARY exist. All four RED/GREEN commits are ancestors of HEAD. The measured ledger range contains four commits and all source changes are committed. Final immutable reconciliation accepts all 590 implemented controls with zero failed controls; all 35 non-success obligations are explicitly IMPLEMENTATION_PENDING. Both genuine RED reports, focused/final reports and the actual isolated mutation reports remain available under `.phase1/`.
