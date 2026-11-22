---
phase: 04-asset-production
plan: "36"
subsystem: content-integration
tags: [CommonJS, discovery, independent-expectations, atomic-publication, source-headless]
requires:
  - phase: 04-02
    provides: Existing no-window runner, bounded child helper and actual packaged tracer feasibility
  - phase: 04-25
    provides: Independent 180-row authority and validated immutable-generation publication
provides:
  - Deferred strict deterministic saved-case discovery through the actual CLI
  - Byte-identical authenticated independent expectation register inside the Godot root
  - Register-inclusive atomic publication with independent tamper and restoration controls
affects: [04-03, 04-26, 04-30, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Deferred CommonJS registration, Ordinal discovery, Independently pinned packaged expectations]
key-files:
  created:
    - tools/phase4/case-discovery.cjs
    - tools/phase4/cases/integration.cjs
    - src/godot-game/content/expected-obligations.json
  modified:
    - tools/phase4/check.cjs
    - tools/phase4/content.cjs
    - src/godot-game/content/pack-index.json
    - docs/content/production.md
key-decisions:
  - "Discover modules only at CLI main selection/listing, preserving direct module self-tests and registration cycles."
  - "Authenticate packaged expectations against pinned independent source bytes; a caller register or refreshed index hash cannot redefine required content."
  - "Keep all ASSET requirements pending; this prerequisite supplies source/headless contracts, with actual hub and final acceptance downstream."
requirements-completed: []
requirements-contributed: [ASSET-02, ASSET-05]
coverage:
  - id: saved-case-discovery
    description: Actual CLI discovery, stable catalog, real module fixtures and 35 independent rejection controls
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: .phase4/runs/4823c2e7-b197-4652-9b6c-c785f1f0c6b0/result.json
        status: pass
    human_judgment: false
  - id: independent-packaged-expectations
    description: Exact independent 180-row register authenticated and published with 33 rejection controls and fresh restoration
    requirement: ASSET-02
    verification:
      - kind: integration
        ref: .phase4/runs/128c4bff-8f7a-4f0f-9b9c-ad862c126032/result.json
        status: pass
    human_judgment: false
actuals:
  tokens: 17958
  tasks: 2
  commits: 4
commits: 4
plan_head_before: 065e78b8db83c9e7b83b07f893fcb4e972d7427d
plan_head_after: 8a7dd2888aa9ef1cfb7b01656c2ac7bb8b43ae1f
duration: 12min
completed: 2026-11-16
status: complete
---

# Phase 04 Plan 36: Discoverable saved cases and portable independent expectations Summary

**The real CLI discovers strict saved modules, and the existing atomic publisher carries the exact independent 180-row register into a validated isolated generation without granting any obligation completion.**

## Performance and scope

Two tasks complete; seven implementation paths, three per Task 1 and five per Task 2, with integration.cjs shared sequentially. No extra implementation path or ownership amendment was needed. Execution used the existing main checkout on feature/04-asset-production, without a worktree or branch change. The first explicitly recorded execution timestamp is 2026-11-16T16:53:48Z; checks and completion preparation ran through approximately 17:05Z. Initial context loading preceded that timestamp. Duration is the rounded recorded execution window, not a claim of precise total agent time.

The measured ledger contains four RED/GREEN task commits before this summary. Actual tokens use chars/4 over the realized Git diff from the persisted ledger base through implementation HEAD, including the two RED records; metadata completion commits are outside that snapshot.

## Task commits and TDD evidence

| Task | RED | GREEN | Result |
| --- | --- | --- | --- |
| 1 — strict real saved-case discovery | 8ff9548 | 1b70589 | Actual CLI listing initially omitted integration.cjs; assertion passed after deferred discovery |
| 2 — authenticated packaged expectations | 8479235 | 8a7dd28 | Existing validate.files initially omitted the independent register; assertion passed after package closure implementation |

Both task attributes require TDD even though workflow.tdd_mode is false. The actual TAP records are [Task 1 RED](tdd/04-36-task1-red.json) and [Task 2 RED](tdd/04-36-task2-red.json). The current tdd-red-evidence classifier returned RED_EVIDENCE_OK for each. Semantic inspection confirmed actual target assertion failures, not missing imports, fixture crashes or zero discovery. Both direct tests subsequently pass. No refactor commit was needed.

Task 1's committed tracer feedback gate passed at 1b70589, invocation 5d10f94e-cd36-4642-9c66-c8bd1ff387a7, before Task 2 implementation. The final selected set below then ran against committed implementation 8a7dd2888aa9ef1cfb7b01656c2ac7bb8b43ae1f.

## Verification and exact evidence

| Executed command/case | Final invocation | Observed result |
| --- | --- | --- |
| source.tracer | 297f8380-2a89-4542-a3d3-62cea0b0ab67 | Pass; actual no-window import/native bow pixels, registration and scene observations |
| source.tracer-failures | 8232b3bc-198b-4b85-abd6-2947427aeec4 | Pass; original eleven failed subjects, actual interruption, restored and current production checks retained |
| source.provenance | 3f56d544-34a0-45da-b10d-8b148862cb6e | Pass; independent census, exact source/output identities, partial-state controls |
| source.publication | 9e8a1b64-7419-42f2-87b1-7f50569da8ab | Pass; four actual interrupted worker stages, separate contention, repeated publication and identity controls |
| source.case-discovery | 4823c2e7-b197-4652-9b6c-c785f1f0c6b0 | Pass; actual CLI children, two real canonical fixture modules, reversed enumeration, 35 rejection controls |
| source.packaged-expectations | 128c4bff-8f7a-4f0f-9b9c-ad862c126032 | Pass; 33 expectation rejection controls, actual isolated generation/read, published tamper and restored positives |
| export.profile-boundaries | abec8089-f0a4-41d2-8a71-5f12dbc64d79 | Pass; retained profile/registry subset/order and bounded child env/cwd/cleanup behavior |
| docs.navigation | b9111cf5-d294-4c7e-a1bd-b2039517402c | Pass 1/1 selected documentation obligations |

The six source cases ran through `node tools/phase4/check.cjs --group source`; the affected profile case ran through its existing --case interface. Each Phase 4 record is under .phase4/runs/its-invocation, with actual source revision, inputs, commands, diagnostics and individual result. The docs record is .phase1/runs/b9111cf5-d294-4c7e-a1bd-b2039517402c/summary.json. Both integration direct TAP self-tests and actual --list pass on the final implementation; listing exposes eleven saved cases and grants metadata only. The retained export.layout and export.profile direct --self-test entry points also pass after Task 1.

Selection comprises every new case and the required affected runner/publication/profile regressions; docs.navigation covers the edited production reference. Audio decoding, managed layout compilation, full profile engine probes and full packaged export are not selected for this bounded prerequisite, as declared in the plan: their media/compiler/profile/export behavior was not changed. This supplies no newly executed credit for those cases. Original 04-02 feasibility invocation 2e67e64e-b0d3-4134-9984-ee57d253e2d9 and all historical source/build/evidence identities remain unchanged.

## Artifacts this phase produces

| Path | New APIs, fields, identities and retained contract |
| --- | --- |
| tools/phase4/case-discovery.cjs | Exports createCaseRegistry() -> {cases,registerCase} and loadCaseModules(directory,registry,loadModule=(file)=>require(file)) -> ordered descriptors. Registry method registerCase(id,definition,run) validates metadata and unique IDs/positions. Internal helpers ordinal(a,b), need(ok,code,detail), stable(value), label(value), canonicalDirectory(directory). Each descriptor is {name,path,bytes,sha256,caseIds}. Maximum 128 direct canonical lowercase ASCII hyphenated .cjs files, regular single-link nonreparse paths, ordinal order, source authentication and nonzero registration per module. Optional loader callback is isolated fixture API only. |
| tools/phase4/check.cjs | Replaces the five literal case requires with registry/createCaseRegistry and deferred caseModules/loadCaseModules at actual main selection/listing. Internal ordinal(a,b) and caseOrder(a,b) sort runGroup/order/ID. createRun invocation.json adds caseModules descriptors. Retains exported main, registerCase, cases, runTracer, validate, hash, inside, document, command and environment; existing CLI --case/--group/--list/--export-tracer and all command/env/cwd/deadline/output/cleanup/source-tracer semantics. Module imports register definitions without running scenario bodies. |
| tools/phase4/cases/integration.cjs | Saved source.case-discovery (Critical/source/no-window/G0-integration/order360) and source.packaged-expectations (same metadata, order361), ASSET-02/ASSET-05 and empty prerequisite acceptance arrays. Exports ORIGINAL, definition, cli(context,name,args,expectedCode), discovery(context), packaged(context), selfTest(), packagedSelfTest(). Constants SCRIPT, PACKAGED and EXPECTED_SHA256 identify the runner/register. discovery owns local reject/registry/fixture/body/load helpers; packaged owns snapshot/partial/subject/reject/replace helpers. Direct --self-test and --self-test-packaged execute Node TAP assertions; fixture code and evidence stay in owned ignored run directories. |
| tools/phase4/content.cjs | New exported constants PACKAGED_EXPECTED, EXPECTED_BYTES=25345, EXPECTED_SHA256 and packagedExpected(root,index,register,files). expected(root=ROOT) authenticates pinned authority bytes before its existing census checks. validate(root,index=document(root,INDEX),register=expected(),{requireComplete=false}={}) retains its signature and anchors the caller register and package bytes independently; its returned files includes the register. All prior exported ROOT/INDEX/EXPECTED/hash/need/text/relative/inside/bytes/parse/document/expected/sourceKey/resolveOrigin/validate/merge/destination/write/publish/readPublished/recoverPublication/worker members and legitimate signatures remain. No change to merge/publish/readPublished algorithms; they consume the expanded authenticated closure. |
| src/godot-game/content/expected-obligations.json | New exact 25,345-byte copy of the existing independent source authority, with unchanged schemaVersion/authority/scope/180 ordered id/name/source rows. Runtime resource is res://content/expected-obligations.json. No extra row or acceptance identity. |
| src/godot-game/content/pack-index.json | Only new required schemaVersion1 field expectedObligations:{path,bytes,sha256}; path is src/godot-game/content/expected-obligations.json and bytes/hash bind the exact independent register. All prior index identity/version/status, shards, outputs, provenance/dependencies and evidence fields retained. |
| docs/content/production.md | Source authority and pinned register identity, required reference shape, strict registration/discovery rules, source-only scenario procedures, atomic generation closure and the explicit 04-03 portable Core/native consumption contract. |

Additional canonical evidence files: tdd/04-36-task1-red.json and tdd/04-36-task2-red.json beside this summary. They are planning evidence, not extra implementation ownership.

Original identity/order triples remain exact: source.tracer/G0/1, source.tracer-failures/G0/2, source.provenance/G0/3, source.publication/G0/4, audio.feasibility/G0/6, export.layout/G0/0, export.profile/G0-export-profile/350, export.profile-boundaries/G0-export-profile/351, export.feasibility/G0/5.

## Independent negative-control inventory

Discovery rejects missing/empty directories; uppercase, wrong-extension, double-dot and leading-space filenames; a directory entry; one zero-registration module beside a valid module; throwing module; 129 modules; a real junction ancestor and hardlink file; duplicate case ID and runGroup/order; missing/empty title/rationale/setup/expected/reset/actions/evidence; unsupported importance/window/group/runGroup/order/requirement/acceptance; reserved id/run overrides; nonfunction run; uppercase, doubled separator, NFD and empty IDs. Actual children reject unknown case, unknown group and empty group. Canonical future fixture modules register through the same discovery/strict registry without runner edits; their scenario bodies throw if executed, and loading never executes them. Reversed filesystem enumeration produces identical authenticated descriptors.

Expectation controls independently reject missing/null/extra references, case/alias/escape/absolute paths, wrong bytes/hash/missing hash/negative size, missing register, truncation, unchanged-reference tamper, invalid UTF-8, BOM, duplicate JSON key, wrong register schema/authority/scope, missing/duplicate/unknown/case/NFD rows, changed names/sources, reversed rows, output-derived membership, caller-supplied reduced register, hardlink register, junction parent and tampered published generation. Row/header controls refresh the index reference and prune outputs, yet fail against independent truth. Each outcome retains exact package path/reason; hardlink/junction outcomes also appear in controls.json and final result.

Valid publication/read retains all 180 exact rows and unchanged index shards/outputs, with no tools-root copy inside the generation. A missing-media subject retains all 180 incomplete obligations; a two-output subject with one missing production bundle keeps the other valid output inspectable. The missing-register subject is restored with actual source bytes and validated freshly. The published generation is also restored after its deliberately failed read, followed by a fresh production positive. No production/source authority is mutated by these controls.

## Source identities

| Input/output | SHA256 |
| --- | --- |
| tools/phase4/check.cjs | 3a5d276359095e0927fff173b99bdcc86e1cb87675ac23d6a4ee60806f695842 |
| tools/phase4/case-discovery.cjs | 8effed8f62a7349a692ffe7ced28dfa6011da386c2c142e909c514b2e43e573f |
| tools/phase4/cases/integration.cjs | 16c1a28004669d9e2071cdde8369f48ab5aeae38bb9bd23e351ab21db7c95015 |
| tools/phase4/content.cjs | 096ca3e4af7a1ffea0995569405a03d2203cb99101c0a237ae71e637329a2318 |
| source and packaged expected-obligations.json, each | a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5 |
| src/godot-game/content/pack-index.json | cca59a20691e75e874f7ed4b229e838e43190b683f0885cc2aa5717f6ed92739 |
| docs/content/production.md | f877702ef8c4f668817d1eb653945eb1b8d7e1f69e8d527d664acce1c75ae18c |
| actual bow PNG, unchanged | 08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999 |

## Decisions and deviations

Followed the approved bounded design. No architectural deviation, additional implementation file, dependency install, auth gate or upstream ownership change. Existing case self-test imports remain cycle-safe by deferring discovery until main; expected truth stays independent from caller arguments and produced outputs. Source authoring validation may read the tools authority; delivered native code must consume only packaged bytes under the documented fixed prefix translation.

Initial local Node child spawning was denied EPERM; the authorized bounded checks used escalated process execution. The first RED report used Node's unsupported default text reporter; a genuine TAP rerun was classified and used before implementation. Two initial discovery test runs exposed owned fixture serialization/path-check setup mistakes, corrected within the declared integration.cjs path before Task 1 verification. Their failed run directories 4715bd79-4a8a-4b3c-8c19-4343f621a6df and 62f04329-4288-485c-adae-6f0c1cc620cc remain unchanged; they carry no success credit.

## Remaining acceptance and readiness

The partial index is still incomplete: one usable common.bow output, 28 pending shards, 180 incomplete obligations and zero completed obligations. Existing source/native/listening/judgment statuses are unchanged. All ASSET requirements remain Pending; this summary contributes prerequisites only. Audio remains three exact candidates with null listener, empty verdicts and actual hearing blocked. No rendered game window, VM, Linux execution, human visual/provenance judgment or listening acceptance occurred.

04-03 can now add hub.cjs through actual discovery and consume the packaged independent register, while implementing the full Core/native hub and exact policy controls in its existing ownership. 04-31 still owns current-hub tracer/export/profile/native-observer adaptations and final portable delivery. Actual Windows 1280x720 inspection, listening and named judgments remain 04-32/04-33 duties after whole-phase implementation and independent review/fixes. Original 180 obligations, 24 AC, 32 decisions, 34 H, 70 UI and four prohibition/judgment identities remain unchanged.

No blocking stub or skipped required verification remains in this plan. Stub scan found no TODO/FIXME/placeholder UI data. All new local-file/module/parser surfaces are covered by T-04-106/107/108 and the existing-toolchain threat boundary; no unplanned trust surface was introduced.

## Self-Check: PASSED

All seven declared implementation paths, both actual RED records and this canonical summary exist. Commits 8ff9548, 1b70589, 8479235 and 8a7dd28 are ancestors of implementation HEAD. Final required source/headless results pass on 8a7dd2888aa9ef1cfb7b01656c2ac7bb8b43ae1f; no tracked deletions or generated untracked files remain. Independent source and packaged register bytes/hash agree exactly. The four measured commits match the persisted plan ledger snapshot.
