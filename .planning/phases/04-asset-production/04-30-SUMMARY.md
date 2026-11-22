---
phase: 04-asset-production
plan: "30"
subsystem: scenario-evidence-and-acceptance
tags: [Windows, Godot, headless, CommonJS, evidence, selection]
status: complete
task_commits:
  task_1_red: c93fca515d2a10f7014b8eca60f65bcaf8dac7db
  task_1_green: 22cb0fed389e4e6307706040a59325afb1289f97
  task_2_red: f57fb41e8b6c7f6ed2c01de8e9ca3afb29de3c19
  task_2_green: b7e5cab9883b17fd53af8696c8d8791481dca560
  validator_repair: a00e3e4dbca03db4cf703315f95eecf1edb86c41
requires:
  - phase: 04-23
    provides: Current 180-resource census, source support catalog, and exact native permission proof
provides:
  - Review-bound Phase 4 selection, immutable evidence, and completion validation
  - A saved catalog of 71 source checks and 107 rendered procedures with an exact Phase 4 handoff
affects: [04-31, 04-24, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Committed input fingerprints, exclusive evidence records, current review adapter, exact scenario ownership]
key-files:
  created:
    - tools/phase4/evidence.cjs
    - tools/phase4/scenarios.cjs
    - tools/phase4/cases/runner.cjs
  modified:
    - tools/phase4/check.cjs
    - tools/phase4/required-cases.json
    - .planning/phases/04-asset-production/04-SCENARIOS.md
    - src/godot-game/content/packs/delivery.json
    - src/godot-game/content/pack-index.json
key-decisions:
  - "Bind actual attempts to the independent current review and the exact saved driver declaration."
  - "Keep the creator's 18 active rendered procedures and exact later ownership of 89 deferred procedures."
  - "Preserve historical source passes and interrupted runs with their original revisions and scope."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03, ASSET-04, ASSET-05]
coverage:
  - id: current-saved-runner
    description: Focused validator controls, exact catalog ownership and inherited source gates
    verification:
      - kind: integration
        ref: "runner group on 0138c1d and focused-ownership.json"
        status: pass
    human_judgment: false
  - id: actual-windows-and-named-reviews
    description: Phase 4 rendered input, named listening, and named prohibition judgments
    verification: []
    human_judgment: true
    rationale: "Plans 31, 24, 32 and 33 provide the reviewed package, actual Windows runs and named observations."
actuals:
  tokens: 232036
  tasks: 2
  commits: 8
commits: 8
plan_head_before: 8a5d9fd8ed658bf07223a8de293efa64ba7a7c05
plan_head_after: 0138c1d4b2df47b5c17d54eda778dfab4f40daf7
duration: "208 min"
completed: 2026-11-21
---

# Phase 04 Plan 30: Saved scenarios and evidence validation summary

The Phase 4 runner validates current review, selection, inputs and evidence, with 18 due rendered procedures and 89 assigned to Phases 5–9. All 180 asset entries, 24 acceptance criteria, 32 decisions, 34 held-out intents, 70 UI backstops and four distinct judgment procedures retain their original identities. The acceptance count is zero.

## Task commits

| Task | Commit | Result |
| --- | --- | --- |
| 1 RED | `c93fca5` | The actual completion CLI rejected the flag at usage dispatch instead of evaluating the missing evidence path. |
| 1 GREEN | `22cb0fe` | The runner checked exact source diagnostics, review bindings, run evidence and saved scenario membership. The fresh committed tracer passed in `0e7bab08-ec7c-4764-b5d5-ff2760cb5f0a`. |
| 2 RED | `f57fb41` | The named-evidence validator accepted arbitrary text bytes stored with a WAV extension and a matching digest. |
| 2 GREEN | `b7e5cab` | The four focused validators passed with genuine WAV parsing, immutable run history and an exact artifact index. |
| Validator repair | `a00e3e4` | The validators compare the 251 exact guarded-host tuples and their compiled evidence, supersede older passes when a later invocation on the same inputs fails, and require exact saved drive steps plus both packaged launch lifetimes. |

The plan ledger spans eight commits, including five from these tasks and three intervening creator commits. The scoped implementation patch contains 928,143 characters, or 232,036 tokens on the plan estimate scale. The creator's two Phase 08 commits and the focused Phase 04 handoff commit remain separate in the inventory.

## Current evidence

The November 21 handoff governs this plan. Its active set contains eight held-out cases, seven UI backstops, and the navigation, packaged relaunch and audio procedures. Deferred owners 5 through 9 account for 25, 18, 6, 11 and 29 procedures respectively. The ownership audit verified exact membership across all 107 procedures, the new completion accounting and nine rejection controls. It also matched 513 current source input files to their committed bytes. [Focused Windows handoff](04-HANDOFF.md)

| Check | Fresh outcome on `0138c1d` | Retained record |
| --- | --- | --- |
| Exact four validator cases | 4 passed with pinned Node 24.22.1 | `8a89cbfc-cf4c-43ca-a24a-66b65df8d1a2`, `c3ac7b67-38d0-4d58-aeb5-0eb63fc7565f`, `f63ce124-1d4a-4b83-b6ae-36fee9239ac3`, `ecee9b0a-d14a-4e62-94d5-2037e51d8a02` |
| Runner group | 4 passed | `f0efa8a6-a3b6-4431-90bf-7ec18c271129`, `232d2ad6-3cac-4d5a-9030-f68b402b2a55`, `89789e66-68ff-433d-b26c-4a9b6b6d50bd`, `41311574-16eb-46f6-a5d9-a8f8553a245c` |
| Case discovery and DT-26 support catalog | Both passed | `c2d3b3bc-ad2e-4f50-b650-12a9eb25541e`, `12ba87ef-d05f-4445-9a93-ca1310fc9d74` |
| Inherited visibility and engine source policy | Both passed with their original IDs | `f76691b3-68c0-48e7-bcbb-d035a67b59b5`, `270dff42-e3f3-413b-b986-a89deaccd018` |
| Strict solution build | SDK 10.0.404, zero warnings, zero errors | `.phase4/04-30-handoff-build-0f607cd7-a84e-405e-bf23-f3297dd3d8a4.log`, SHA-256 `6c188436b785e3136bd187972572fd7de0f814da3d6fed433cf487527a224ed6` |
| Ownership, active coverage and false-pass controls | 18 active, 89 deferred, nine deliberate rejections, zero accepted | `.phase4/04-30-focused-handoff-71a4433b-c646-4b74-b4c2-1fa6616f162c/focused-ownership.json`, SHA-256 `7da37bbb7c0019c293d26bb0cabce5a0ed0cfb39f0eed059c1683942ed5e0e31` |

The current source fingerprint is `f2e84d3cb3f662990540fd81786a6cc4a156a94ded489178325b0d270f4d4165`. The current media fingerprint is `1d100d502e7a125e885264db5ec34e477872c021216bdf6e5d03809172a64420`. The creator's `tools/phase4/handoff.json` matches committed SHA-256 `fea135bfa2d785d476f20e6e4d2d60a2df06bb52aa293cb25833bac93402a2a6`.

## Preserved execution history

- Both TDD regressions passed on the current head. Their original semantic failures passed the GSD RED classifier before each GREEN commit. The retained receipts are `.phase4/tdd/04-30-task1-red-98892b84-e635-4fa3-894d-1ce73efa54f3.json` and `.phase4/tdd/04-30-task2-red-3bb0a2db-0554-4291-bd75-09aeb38d31a1.json`.
- The earlier complete `b7e5cab` source and headless selection passed 72/72 on 1,390 verified committed inputs. It audited 148 clean positive native commands, 51 single-threaded positive imports, three deliberate native negative commands and zero unexpected native issues. The 251 guarded-member checks passed individually. The receipt is `.phase4/04-30-final-source-d7c52fb7-0671-45ee-88d1-4ca175411b18/completion.json`, SHA-256 `35a709f5fc0bfd91f0285401c0da344da8b8a3c4605d1ae96cd1251badfa60b2`.
- The repair passed 11/11 focused checks, including case discovery, provenance, publication, census, packaged expectations, and source and native DT-26 support. The record is `.phase4/04-30-repair-focused-f8ed83a4-2638-49b0-bb5b-8625a91a84a1.log`. The repaired proof reader also checked the 251 genuine compiled CTRF artifacts from the earlier run and rejected a modified execution-record digest. This verifies the reader against historical data.
- The repaired full selection started on `a00e3e4`. It recorded five qualified top-level passes before concurrent creator commits changed source and planning inputs. All 251 nested tests emitted passes, then their top-level snapshot assertion failed on drift. The enclosing run stopped with exit 1. `.phase4/04-30-repair-final-source-460c3de2-3685-45c2-8b77-94d9a1e1dfe8/case-006.json` has SHA-256 `8fe5345be8b2ccb1a88d25b57397781483ce220836f8af175cb01c2aa0146895`. The corresponding `.phase4/runs/73581524-171c-474c-a911-7bdc393ffcb6/result.json` has SHA-256 `996c3f3edebd10be10c67537c1c954cf97eb771c7e601afe16f8ab2a39617074`. The attempt carries no whole-selection result.
- The current handoff changes the runner's tracked source inputs. The earlier 72-case pass and repair sweep remain historical. The current table credits only fresh runs. Windows rendered evidence, named listening, the four actual review verdicts, and final delivery acceptance remain pending.

## Deviations from the original plan

- The existing checker required a narrow dispatch addition for exact comma-separated IDs, the composition and rendered groups, and `--verify-completion`. The dynamic case loader, original commands, and single-threaded import policy retain their original paths.
- The DT-26 fixture embeds the authored catalog. Its payload, extent and hash were regenerated, and the stock normalizer updated only the corresponding catalog and delivery references. Both derived dependencies were authorized and documented before editing.
- The first runner integration check found that the existing `content.native-permissions` procedure lacked a saved heading. The scenario documentation now includes it. Original failed invocation `397b0ebd-6522-4d0c-87c4-f797e941ec37` remains intact.
- The scoped repair addressed three demonstrated omissions together. Detached fixtures established that an older passing result could mask a later failure, a single launch could satisfy a two-launch record, and the inventory omitted all 251 guarded-host tuples. The repaired controls reject those cases and accept matching current evidence and proven unrelated invocations.
- The creator's November 21 amendment replaced the wider rendered matrix with the focused handoff during the repaired full source run. The current runner, affected discovery and catalog checks, inherited visibility and source policy checks, strict build and independent handoff controls passed on the amended revision. The Phase 08 work and creator-owned handoff remain separately attributed.

## Artifacts this phase produces

| Current file | Bytes | SHA-256 | Responsibility |
| --- | ---: | --- | --- |
| `tools/phase4/cases/runner.cjs` | 31,839 | `2eb8df093c57e3d8e6430bad69f14802a9d8deb3b89261c99728d3da089bd728` | Four exact registered validators and detached source test controls |
| `tools/phase4/check.cjs` | 51,263 | `c44c6ac446be6b00322c08fc3da2f5c7423251cab92fbad46da3575b153177f4` | Dynamic dispatch integration in `async main(args)` |
| `tools/phase4/evidence.cjs` | 30,855 | `f15ef506788daf71935f4a1f459063998a27a8e2e75ff3a0f9a1a53a648cb879` | Evidence, source identity, outcome, judgment and artifact validators |
| `tools/phase4/scenarios.cjs` | 48,265 | `7438084430f915294eb1b79e3de4a1815150b1b048799e622af6564eafc36af3` | Source and rendered dispatch, exact driver obligations, review binding and completion reconciliation |
| `tools/phase4/required-cases.json` | 668,897 | `01326ff9d8c15763f6ce32a1b560e6db948b32f1894ac1c339f771d7568b4187` | Stable catalog, 71 source case IDs, 107 rendered IDs and two pending export cases |
| `.planning/phases/04-asset-production/04-SCENARIOS.md` | 264,584 | `25adb3a215d0c83d2a0ecd6150774b37795fa4a70efd359555ea4cb5737e9548` | Saved procedures and declared evidence contract |
| `src/godot-game/content/packs/delivery.json` | 97,371 | `9fe0fc4a9fb8218a9e67c14145cdbb84413aa57b01ff5ae3eced205b89688bb3` | Derived `fixtureCatalog` UTF-8/gzip payload, uncompressed extent and digest |
| `src/godot-game/content/pack-index.json` | 218,006 | `fd20e67680691dbc7aabeb3391f1f2059a17f4646e6ffefe759396822ebf766f` | Exact corresponding catalog and delivery extents and digests |

The complete inventory includes 106 JavaScript function signatures and source line numbers, all case and fixture IDs, the exact current derived entries, and the 20 concurrent creator paths separately attributed. It is retained at `.phase4/04-30-focused-handoff-71a4433b-c646-4b74-b4c2-1fa6616f162c/current-artifacts.json`, SHA-256 `d0c532abbcf83fc388465b6dab57b3efe6fc90b93f4ad7841099965e00a7cb9e`. The case exports are `runner.prelaunch`, `runner.selection`, `acceptance.judgments` and `acceptance.artifacts`. The embedded Windows capture helper declares `Phase4Capture`, its `Point` and `Rect` structures, and exact desktop calls for foreground-window, process ID, client rectangle, client-to-screen, DPI context and compositor completion. There are zero added retained bundle members and zero added native permission tuples. The current validator independently derives the 251 existing guarded-host tuples.

`export.hub-consumer` remains pending for Plan 31 as Critical in `export`, run group `G0-export-hub`, order 310, under ASSET-05 and AC-15/16/17. `export.portability` remains Plan 31 work. Later phase data supplies the genuine independent review, exact portable Windows package, real input and rendered evidence, and named verdicts before final acceptance.

## Self-Check: PASSED

All eight scoped files match the recorded lengths and SHA-256 digests. All five implementation commits are ancestors of the tested revision. The measured ledger contains eight commits through `0138c1d`.
