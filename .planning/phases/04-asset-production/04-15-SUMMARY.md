---
phase: 04-asset-production
plan: "15"
subsystem: ui
tags: [godot, csharp, inventory, legacy-layout, stash, targets, native-proof]
status: complete
requires:
  - phase: 04-28
    provides: Canonical item identity resources, recovered theme and packaged fonts
  - phase: 04-10
    provides: Persistent equipped corpse and exact nearby-target appearance contracts
provides:
  - Native eleven-slot inventory, real four-food references and rotated grid/drag layouts
  - Saved legacy geometry, separate current destination and unchanged failed-extraction resources
  - Paged stash/context/conflict and exact NPC/corpse/pile access examples with independent H20
affects: [04-13, 04-16, 04-17, 04-22, 04-23, 04-31, 04-32, 04-33, 06, 12]
tech-stack:
  added: []
  patterns: [guarded internal native composition, detached strict bounded JSON, exact resource_id rows, source and imported visible pixel authentication]
key-files:
  created:
    - src/godot-game/inspection/InventoryLayouts.cs
    - src/godot-game/content/ui/inventory.json
    - src/godot-game/content/packs/ui-inventory.json
    - tests/phase1/InventoryLayoutTests.cs
    - tools/phase4/cases/ui-inventory.cjs
    - src/godot-game/content/ui/legacy-stash-targets.json
    - src/godot-game/content/packs/ui-legacy-stash.json
  modified:
    - tools/phase4/cases/export-profile.cjs
key-decisions:
  - "Preserve saved geometry and contents independently of current destination rules and failed extraction."
  - "Keep layout-owned ghost wrapper separate from its actually rotated icon child."
  - "Use exact stable resource IDs and canonical native pixels; labels and Node.Name never substitute for identity."
  - "Stash filter/no-match examples remain inspection data; gameplay filtering and INV-14 belong to Phase12."
  - "Known WINDOWS29/30/31 integration remains later-owner work; local passes never certify full phase acceptance."
  - "Only the exact canonical InspectionTheme reader accepts at most2MiB; other resource paths retain1MiB and current scene/runtime closure stays authentic."
requirements-completed: []
requirements-touched: [ASSET-02, ASSET-03, ASSET-04]
actuals:
  tokens: 336773
  tasks: 2
  commits: 13
commits: 13
plan_head_before: 6faa5e291e4117271dc445c9793ea65c6a2ec9ad
plan_head_after: 14968cc064263dff24e31ad94edc0a343d846ae9
duration: 123.88min
completed: 2026-11-17
coverage:
  - id: inventory-native
    description: Eleven slots, exact food references, canonical icons and rotated native grids/actions
    verification:
      - kind: integration
        ref: "layouts.inventory 4d451ef8-6a9a-4ea3-b1de-24f9b08715fb:859 native outcomes,5 compiled tests0skip"
        status: pass
    human_judgment: false
  - id: legacy-target-native
    description: Saved geometry, stash conflicts, nearby target access and independent H20 layers
    verification:
      - kind: integration
        ref: "layouts.legacy-target f86514c5-470b-4622-ae60-2c6b4ff8bd62:1452 native outcomes,5 compiled tests0skip"
        status: pass
    human_judgment: false
  - id: export-profile-current-closure
    description: Bounded exact theme reader and authentic current scene, script, font and compiled runtime profile fixtures
    verification:
      - kind: integration
        ref: "export.profile 0f0e679c-ed19-45ad-bdf3-baf3460b43c1"
        status: pass
      - kind: integration
        ref: "export.profile-boundaries e5f5d8d5-eb5d-46ae-8ed7-76ce0f9344e3"
        status: pass
    human_judgment: false
  - id: actual-layout-readability-and-integration
    description: Full hub/index dispatch, current native permissions/export and actual appearance/input acceptance
    verification: []
    human_judgment: true
    rationale: "Known WINDOWS29/30/31 remain04-23/31-owned. Whole phase and independent review/fixes precede04-32 actual Windows1280x720/noVM review and04-33 reconciliation."
---

# Phase 04 Plan 15: Native inventory, legacy, stash and exact targets Summary

**Eleven native slots and real quick references retain canonical items; old saved bounds and belongings remain distinct from current destinations, stash conflicts and exact nearby target identities.**

## Completion and evidence boundary

Both local implementation tasks are complete. The fresh executable local affected selection passes 33/33 on 430 unchanged raw input identities, zero drift; it comprises32 Phase04 cases and exact Phase1 visibility. Two active Critical integration cases remain explicitly selected-but-blocked: source.provenance and source.publication. This is not a full phase selected-set or ASSET acceptance pass. Each executable case ran freshly in this frozen-source selection; none was supplied by old evidence reuse.

Current-source tests ran at HEAD468641d3f715f705fc135d3ebf6dfb3b6bc92b5c plus the exact final Task2 and export-profile WIP; tested bytes were committed unchanged as14968cc064263dff24e31ad94edc0a343d846ae9. The mandatory Task1 committed-content tracer ran earlier at e7ccc86, not the later Task2 revision. Every original report, failed command, source-input hash, native nonce/PID, copied DLL and original/revised selection remains distinct. Global STATE/ROADMAP/REQUIREMENTS,21→22,current15→13,completed46→47,wave17 hooks,inventory236 and PR5 are parent-owned. No global state, policy/cohort permission, index/Root integration or PR change was made here. The prospectively amended plan union is exactly eight paths, Task2 union six; the phase inventory remains236.

## Accomplishments

InventoryLayouts is an internal sealed non-GodotObject composition owner. Seventeen current inventory/HUD states project eleven logical slots, separately named hands with actual bow/ammunition, four references to real food stacks, shirt/trouser/belt/bag and nested grids, stack split/count, footprint/rotated icon, numeric current/max and Broken, weight, breadcrumbs/depth and full tooltips. A zero-rotation layout-owned wrapper preserves the drag icon's actual90-degree transform through native container layout and replacement/restoration. Owned nodes, atlas views, style boxes and proof Image copies are deterministically retired; one neighbor survives another owner's Close. Thread/generation/detached/closed fences remain effective.

Twenty-two additional examples preserve saved8×6 bounds, old1×3 rotated knife, quantity31 berry stack, incompatible slot/depth and missing-definition states; current2×1 destination has a separate title/control tree. Failed extraction keeps original contents/source bytes unchanged. Stash shows unlimited paging, character context, empty/no-match/full/new-page/stock-changed/losing-operation examples. NPC/corpse/pile targets named Alex retain npc-alex/corpse-alex/pile-alex identities, all eleven gear rows, actual contents and permitted/disabled/emptied/range/dead/closed feedback. Active-world and inspection-only statements remain truthful.

Independent held-out H20 supplies three equal-label/value targets with touching and overlapping native regions, changed saved7×5 geometry, changed2×3 saved footprint and actual canonical textures. It verifies distinct IDs, selected-only modest one-pixel outline/marker and a visible wrapping warning above ordinary inventory. Native observations establish controls, text, texture identity and geometry, not actual-window readability or input judgment. Filtering remains visual inspection data only; Phase6 stays unfiltered and Phase12 owns INV-14. No game transaction, inventory ownership operation or filtering implementation is claimed.

## TDD Gate Compliance

Task1 RED18d4a8c precedes Task1 GREENe7ccc86. The actual authenticated JUnit report at .phase4/04-15-red-authenticated/evidence.json executed3 intended production-assembly assertion failures (slots/rotation/quick reference), zero fixture errors/skips, exit2, SDK RED_EVIDENCE_OK and semantic acceptance. Observed start16:22:46.843Z/report16:22:47.513Z on2026-11-17 governs freshness. Original short-target INVALID_RED and the earlier lower-bound timing estimate remain historical and are not promoted to observed launch times.

Fresh SAME-CASE committed Task1 tracer fe31b29a-d72e-4484-9084-d8adffc4a30e passes before Task2. It discovers/runs the then-current3 tests and859 native outcomes at e7ccc86. Task2 REDca9e586 then executes five tests: old three pass; saved-geometry and equal-name target-identity assertions fail intentionally; zero errors/skips, exit2, strict0/0, SDK RED_EVIDENCE_OK plus semantic acceptance. Its actual observed timing is start16:56:39.7324119Z/report16:56:40.4067256Z in .phase4/04-15-task2-red-authenticated. The earlier missing-PHASE1_ROOT run had five assembly-load failures: semantic-rejection.json rejects it despite mechanical RED_EVIDENCE_OK, and it supplies no GREEN authorization. Final Task2 GREEN54f7d77a92caab40e94465ff310fc54290264c0d descends from ca9e586. No behavior-changing refactor followed final verification.

## Original failures, prospective repairs and budgets

| Stage / preserved identity | Actual result / accounting |
| --- | --- |
| Task1 original3/3 | Two test/analyzer convention batches before genuine RED; first GREEN formatting/static batch; final SCR0007 line521123>120. Checkpointb68ced9; manifest18c3b564... retains original inputs. |
| R1 authority7d61595; used1/2 | Exact whitespace wrap; ordinal harness comparisons only. Canonical1578397a:846native/844pass2fail; production ghost transform and zero-grid observation unresolved; one CanvasItem/two ObjectDB leaks preserved. Manifest314a2f8b...; no clean teardown claim. |
| R2 authority72386b6; used1/2 | Baseline64ff4293 actual ghost0deg after two layout frames. Proof-only15369b99 frees exact duplicate-property Control141129943168 on SAME DLL5db6c136..., removing leak stderr. Actual production wrapper repair follows. |
| R2 initial9c850ee4 | Original846 IDs/order pass; two appended canonical expectations use IT-02 instead of source knifeIT-03 and fail. No expected values copied from observation. |
| R2 corrective1 / fd0c230f | Source-defined IT-03 yields859pass, strict production/harness0/0,3compiled0skip, natural exit0/empty stderr. Original occupied-zero-grid source order requires exact ITEM_BOUNDS; separately isolated empty nested2×2 grid reaches GRID_DOMAIN, wrong-hash and restored-positive retained. |
| Task1 commit e7ccc86 / tracerfe31b29a | 859 IDs preserve original846 order plus13 controls. Task1 genuine behavior and mandatory fresh committed tracer complete. |
| Task2 original3/3 | Test conventions1; conservative missing-PHASE1_ROOT environment correction2; GREEN line wrapping3. Genuine REDca9e586. Final89c2fdab strict0warnings/1SCR0009 line253indent28vs20; Task2 native had not run. Checkpointce8f127/manifeste6b53fd3... preserves seven files and251 evidence identities. |
| R3 authority2904995 / initial | All seven live/backup raw SHA256 and raw/filtered blobs plus251 evidence identities and RED/checkpoint/amendment ancestry verified before exact conditional whitespace. Other five files remain byte-identical to checkpoint. |
| R3 sandbox environment | Git spawnSync EPERM before invocation; preserved environment-failure.json; equivalent authorized ordinary subprocess execution follows. Not a source correction. |
| R3 initialadc91f09 | Strict solution0warnings0errors; harness LEGACY_CLEAN_BUILD regex lost literal-parenthesis/newline escapes. Native/compiled bodies not yet reached; failure retained. |
| R3 corrective1 /7f7c294d | Only existing legacy build-output regex restores exact0Warning(s)/0Error(s) observation. Focused PASS22states/5compiled0skip/1452native/PID30928/naturalexit0/empty stderr. R3used1/2; all earlier budgets unchanged. |
| Scheduling history | Initial .phase4/04-15-r3-final selection stopped only its authenticated owned coordinator24916/tree before provenance; five completed passes retained as interrupted history. Corrected accepted selection later stopped only coordinator35700/tree before publication, retained ten completed groups, and resumed remaining groups on same428inputs. No production/data/test edits or additional repair attempt. Original/revised scheduling records remain immutable. |

No fourth original correction or reset of an exhausted budget occurred. R3 presenter diff against latest backup is solely eight leading spaces removed at253; case diff is solely escaped build-output regex. Both original inventory JSON/pack, both legacy JSON/pack and InventoryLayoutTests.cs remain byte-identical to the latest checkpoint. No parser/domain/order/geometry/ownership behavior, API/path/identity, policy/cohort, analyzer, deadline or capacity changed during R3. The1,022,146-byte legacy document passes the unchanged1,048,576-byte bound; no limit was raised.

Authoritative preserved manifest identities: .phase4/checkpoints/04-15-original-budget/manifest.json SHA25618c3b56400d3b07436c453065694b4719ac2c4f878cdd498aa5f6e56b75a26af; .phase4/checkpoints/04-15-r1-production-boundary/manifest.json SHA256314a2f8b67b4753d870f8f422dbc5b2626116f1a43a4bc6ef5a50d704a50ade6; .phase4/checkpoints/04-15-task2-original-budget/manifest.json SHA256e6b53fd3d0f0013ebd0718a5b8907afb80f5f63e5a56164e8246158f97d60739. Each retains exact live/backup byte and raw/filtered Git mappings at its own checkpoint; final inventory below records final tested bytes, never relabeling changed files as old sources.

## Fresh local verification and mappings

Final selection authority: .phase4/04-15-r4-final/selection.json. Its results.json retains all actual per-case starts/ends/durations/revisions and copied assembly closures; completion.json records zero drift. All430raw current input identities remain frozen through the fresh33-group run. The earlier failed R3 selection and interrupted/revised scheduling histories remain immutable and receive no final-pass substitution. Total measured executable-case time1372699ms (22.88min), separate from interrupted attempts, setup, authored work and total ledger window.

| Case | Fresh invocation | ms | Result / actual HEAD |
| --- | --- | --- | --- |
| `layouts.inventory` | `4d451ef8-6a9a-4ea3-b1de-24f9b08715fb` | 22586 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `layouts.legacy-target` | `f86514c5-470b-4622-ae60-2c6b4ff8bd62` | 18377 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `buildings.defense-art` | `9967506b-14d9-464e-9140-459da7104676` | 2164 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `buildings.defense-state` | `312b0e57-412f-4c24-aca6-ee48280f2545` | 11548 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `buildings.main-art` | `79a15b66-163f-47cb-8cfb-e6c312da83d7` | 5094 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `buildings.main-state` | `6cad5812-2f56-4a9c-a593-1ea310e5e959` | 14905 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `media.conversion` | `579d4d89-37f2-423c-ab36-dfbaf0cf5527` | 45290 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `media.scale` | `63cfa25b-978c-406a-91a7-3e2e75c0af98` | 8559 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `appearance.corpses` | `ddda8eab-43b2-4f70-a44c-4cf5cc53de52` | 26783 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `appearance.crowds` | `4076d571-d168-439e-a8d7-8cdc3fd8dc3b` | 32860 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `animation.work-special` | `b5ede4f5-cf0b-4791-8559-1b2195b57832` | 27614 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `animation.bow` | `bcfc320c-65a3-44f0-be7c-43df29275f54` | 47533 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `animation.replacement` | `088508a4-9413-4200-9f83-a1e5ba8fc7fb` | 25594 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `animation.melee` | `3a7108fa-de26-4f85-8996-7d3806925c14` | 25568 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `bodies.a-art` | `ca369704-a84f-45a7-9970-bf3d149c7a90` | 8517 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `bodies.a-fit` | `86574efb-068f-4121-8f2c-327becec5a55` | 28303 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `bodies.b-wide` | `fbcc9489-de22-4ad3-8fe6-1c7d4240381c` | 48008 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `bodies.complete-fit` | `a507167a-cc87-4c09-b4a7-2861fcfb59da` | 49277 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `source.case-discovery` | `08d39b21-d69d-4264-8bb1-271e75319c76` | 667 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `data.foundation` | `3151b6f7-9ff9-4e6e-99c0-431f3084325c` | 479 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `data.items-combat` | `8f6c6254-99a8-4d38-855e-67c996eb29a8` | 7287 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `hub.source` | `995570d2-3d79-4c7d-bae8-6801929fd8e9` | 382505 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `hub.lifecycle` | `e12f70bd-f885-4c6c-8032-bed978cf4c39` | 355817 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `items.weapons-food` | `c3e7240d-52b5-4c84-87f2-cb2e83197bdc` | 7800 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `items.wearables` | `6ff11b0d-5881-4233-b6ec-7425291146be` | 12976 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `layouts.theme` | `0aac7298-d0eb-4117-9a4c-0d20a71fe1b4` | 14086 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `layouts.identity` | `42cc4427-81d0-4eac-8db9-239655d9e153` | 12130 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `layouts.identity-integrity` | `9980aafc-996f-4044-a7c5-aca72420d5cb` | 14234 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `export.layout` | `eec2273c-84b2-4b3f-8c74-159997668a9f` | 23950 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `export.profile` | `0f0e679c-ed19-45ad-bdf3-baf3460b43c1` | 9079 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `export.profile-boundaries` | `e5f5d8d5-eb5d-46ae-8ed7-76ce0f9344e3` | 43374 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `audio.feasibility` | `3f766bf0-e13c-41f0-afcb-4b9d8b86e1bb` | 9277 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |
| `graph.visibility-inventory` | `d18f7ce3-2b5c-4c66-8b74-d93a91e2de71` | 30458 | pass / `468641d3f715f705fc135d3ebf6dfb3b6bc92b5c` |

Both inventory cases execute the strict whole-solution build and emitted harness with0warnings/0errors, discover and run all5 actual InventoryLayoutTests with0skips, authenticate actual Core/game load-context ownership, canonical pixels/source hashes, exact copied production DLL closure and official nonconsole engine archive/entry/version, and require natural exit0/empty native stderr. Inventory outcome ID/order comparison against the committed Task1 tracer is exact859/859; legacy has1452positive/negative/native outcomes. Retained constructor controls rebound owned parent hashes to semantic faults, reject before native attachment, independently reject wrong hashes and restore valid owners. Fatal/import/leak output is never suppressed.

| Case/run | Actual PID / exit | Game DLL SHA256 | Core DLL SHA256 |
| --- | --- | --- | --- |
| `layouts.inventory` / `4d451ef8-6a9a-4ea3-b1de-24f9b08715fb` | 38216 / 0 | `949a731d38f1774997053cebade02b4cc10e4be78c645e65199da737f455cb21` | `440880ab3838de3086ddfc3b9360ba886aac440394b02bd19e65489d9e342b9f` |
| `layouts.legacy-target` / `f86514c5-470b-4622-ae60-2c6b4ff8bd62` | 22292 / 0 | `949a731d38f1774997053cebade02b4cc10e4be78c645e65199da737f455cb21` | `440880ab3838de3086ddfc3b9360ba886aac440394b02bd19e65489d9e342b9f` |

Engine version4.7.2.stable.mono.official.ed1daf0bf; engineSHA25645336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab; archiveSHA256a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0; exact archive entryGodot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe. Each run keeps engine origin command, source-input manifests, full production assembly/deps/runtime closure, generated harness/project, command/stdout/stderr/exit records and nonce-bound native results. Historical Task1 tracer gameSHA256bb6c2989e761654a613f80e3eba63f5ec0a3fcca44f8ffb93bc404184bb7dffa/Core27de3269c415d3dbd40c0908d9e9fb359d5b1f251cef061e226b563372a4dc43 remain separate; no later DLL is relabeled as that tracer.

## Selected but blocked phase obligations and explicit omissions

source.provenance: actual historical FAILadb38f27-592b-4700-880c-3fa1d65d0c15 at1ffe807, PARTIAL_PACK_REQUIRED from zero-valid stale common.bow→Inspection.tscn FILE_HASH_MISMATCH. No new invocation/pass credit. Exact historical/current comparison is in artifact-inventory-final.json:8of9 relevant hashes match; Inspection.tscn changed since history, while current index/tracer retain their unchanged old binding. This source-based binding diagnosis is documented by04-CURRENT-PACK-INTEGRATION-AMENDMENT.md; it is not asserted that all historical inputs remain identical.

source.publication: selected-but-blocked by source-based inference, not an executed failure/pass. provenance.cjs104 baseline publish(ROOT,index) calls content.cjs211 validate and rejects OUTPUT_INVALID as PUBLICATION_INVALID_OUTPUT for that same current invalid binding; independent publication controls remain unexecuted until04-23/31 repair. Original25 publication passes remain historical. Both Critical obligations remain required and receive no skip/pass or phase-completion credit.

Other explicit omissions: unchanged current tracer/tracer-failures/packaged-expectations/export.feasibility remain WINDOWS29 integration-owned. engine.source-policy/current generated cohort remain WINDOWS30/31 and are observed once below without grants. Source-only High layouts.kit has unchanged exact media/fonts/wood and selected native consumers. Other Phase1/2/3 cases have no changed Core/foundation/LAN/project/analyzer/policy edge; strict solution, five production reflection tests and visibility cover this local adapter delta. Actual windows, physical input, named appearance/readability/provenance judgments, audio listening, FPS, LAN, endurance, hosted/Linux and release validation remain their separate owners. Unknown or blocked required phase work is never counted as a local pass.

## R4 scoped current export consumer repair

Prospective authority468641d,04-15-EXPORT-REPAIR.md follows checkpoint9dc2f3e. The earlier R3 final selection FAILED31/33 executable groups on428 unchanged inputs in1348931ms: export.profile1fea9fd2 and export.profile-boundaries2434abdf stopped at PROFILE_RESOURCE_SIZE. The unchanged canonical theme is1058195bytes,9619over the old reader bound; its exact SHA256b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a and three retained embedded fonts remain intact. No resource/font/media/Root/index/JSON parser/policy bytes were rewritten. All seven live/backup rawSHA256/raw-filtered mappings, all2756 retained evidence identities, manifestSHA25611cd67a03c3302ad5a9e239e7d84cde58f113cd4114a98a4cf76a4541e6cbad6 and ten historical ancestors passed before the first R4 edit.

R4 substep1 initial source proof .phase4/04-15-r4-source-initial/result.json passes61 preserved source rejection controls plus10 independent reader controls;0/2 corrective attempts. read(root,file) authenticates exact portable spelling, checks file type/nonzero/size before allocation, reads the bounded opened descriptor and rejects length drift. Only exact src/godot-game/assets/ui/InspectionTheme.tres permits at most2097152bytes; every other resource remains1048576bytes. sceneClosure(root,seeds) follows actual scene/theme/source/font references, authenticates every dependency and bounds128files,16depth,16777216aggregate bytes, with cycle-safe visited identities. snapshot/copySubject/observe now share this exact closure; no stripped script/theme or substitute scene. The actual static closure contains14files. Independent closure-controls.json adds9 passing owned-copy checks: cycle-safe, depth-over, count128/129, aggregate16/17MiB, nested-missing/nested-case and script-hash-change. No production edit or extra corrective attempt was needed.

R4 substep2 initial syntax verification failed because the new local expected collided with the existing observe expected parameter; CASE_MODULE_LOAD asserted nothing and launched no native process. Corrective1/2 renamed that local to expectations. The sandbox Git EPERM before invocation is separately recorded as an environment failure; equivalent authorized normal Git/.NET/authenticated headless execution follows. Focused export.profiled55f496c-7105-4f59-a00b-b0377ad2e0a8 and export.profile-boundaries70fb883c-c314-4a03-beff-a0a6531feb0a PASS. No second corrective attempt. Actual strict production builds0warnings/0errors, copied current game/Core/ports/deps/runtime artifacts, exact source/config/media hashes, three font imports and authentic current InspectionRoot.cs produce genuine native settings/scene/bow observations. The original feature/ordinary/empty/wrong-case/global/missing-override semantics, all61 source controls, original helper bounds/descendant teardown, original registry IDs/orders and fresh restored native positive remain passing. The actual source/native script and PID mapping is checked, with natural clean exit0/empty stderr. Native probe releases its proof-owned Image reference and frees the real scene. No packaged-startup, final current-pack integration or actual-window acceptance is granted.

R4 substep3 freezes430inputs/eight intended paths and executes all33 executable groups afresh after focused profile passes.0/2 corrective attempts; no source edits during accepted execution. The original two selected-but-blocked Criticals and every original integration/actual omission remain explicit. This is the complete executable local pass and does not claim full-phase selected-set success. Attempts/history live in .phase4/04-15-r4-final/attempts.json; final selection/mappings are in that same directory. Exactly one final changed policy/producer observation follows the whole run; no permission or enrollment is granted.

### Export reader, closure and observation contracts

| Actual function signature | Source line |
| --- | --- |
| `function need(value,code,detail)` | 25 |
| `function read(root,file)` | 26 |
| `function sceneClosure(root,seeds=RESOURCES)` | 35 |
| `function write(root,file,value)` | 55 |
| `function ini(root,file)` | 57 |
| `function validate(root)` | 71 |
| `function snapshot(root)` | 95 |
| `function productionSources()` | 99 |
| `function buildEnvironment(context)` | 110 |
| `function copyRuntime(project)` | 113 |
| `function probeScript(nonce)` | 128 |
| `async function observe(root,context,name='profile',expected={qualified:INSPECTION,inspection:INSPECTION,active:false,actual:FOUNDATION})` | 178 |
| `async function profileCase(context)` | 247 |
| `function trialContext(context,name)` | 264 |
| `function copySubject(context)` | 269 |
| `function mutate(root,file,edit)` | 271 |
| `function registry()` | 272 |
| `async function helperControls(context)` | 283 |
| `function sourceControls(context,before=snapshot(ROOT))` | 323 |
| `async function boundaries(context)` | 412 |
| `function selfTest()` | 437 |
| `function helperSelfTest()` | 449 |

New bounded constants: THEME,RESOURCE_LIMIT,THEME_LIMIT,CLOSURE_COUNT,CLOSURE_DEPTH,CLOSURE_BYTES. Nested sceneClosure.visit(file,depth) and productionSources.walk(directory) retain bounded/authenticated dependency and production-source traversal. New sourceControls(context,before=snapshot(ROOT)) preserves original controls; productionSources(),buildEnvironment(context),copyRuntime(project) authenticate the real compiled runtime. CommonJS exports: `read`, `sceneClosure`, `sourceControls`, `validate`, `snapshot`, `probeScript`, `observe`, `profileCase`, `trialContext`, `copySubject`, `mutate`, `registry`, `helperControls`, `boundaries`, `selfTest`, `helperSelfTest`. No new case identity or registry order is added. GDScript reject(reason),_initialize(),inspect() retain original settings/image predicates and add real script identity,actual process identity and deterministic image/scene retirement.

| Independent reader control | Exact path / byte size | Expected / actual |
| --- | --- | --- |
| `theme-exact-2mib` | `src/godot-game/assets/ui/InspectionTheme.tres` / 2097152 | `accepted` / `accepted` |
| `theme-over-2mib` | `src/godot-game/assets/ui/InspectionTheme.tres` / 2097153 | `PROFILE_RESOURCE_SIZE` / `PROFILE_RESOURCE_SIZE src/godot-game/assets/ui/InspectionTheme.tres` |
| `ordinary-exact-1mib` | `src/godot-game/assets/common/bow.png` / 1048576 | `accepted` / `accepted` |
| `ordinary-over-1mib` | `src/godot-game/assets/common/bow.png` / 1048577 | `PROFILE_RESOURCE_SIZE` / `PROFILE_RESOURCE_SIZE src/godot-game/assets/common/bow.png` |
| `theme-empty` | `src/godot-game/assets/ui/InspectionTheme.tres` / 0 | `PROFILE_RESOURCE_SIZE` / `PROFILE_RESOURCE_SIZE src/godot-game/assets/ui/InspectionTheme.tres` |
| `theme-missing` | `src/godot-game/assets/ui/InspectionTheme.tres` / null | `PROFILE_RESOURCE_MISSING` / `PROFILE_RESOURCE_MISSING src/godot-game/assets/ui/InspectionTheme.tres` |
| `theme-case` | `src/godot-game/assets/ui/inspectiontheme.tres` / null | `PATH_CASE` / `PATH_CASE common.bow src/godot-game/assets/ui/inspectiontheme.tres` |
| `theme-alias` | `uid://InspectionTheme` / null | `PATH_INVALID` / `PATH_INVALID common.bow uid://InspectionTheme` |
| `theme-traversal` | `src/godot-game/assets/ui/../ui/InspectionTheme.tres` / null | `PATH_INVALID` / `PATH_INVALID common.bow src/godot-game/assets/ui/../ui/InspectionTheme.tres` |
| `theme-sibling` | `src/godot-game/assets/ui/OtherTheme.tres` / 2097152 | `PROFILE_RESOURCE_SIZE` / `PROFILE_RESOURCE_SIZE src/godot-game/assets/ui/OtherTheme.tres` |

Exact final profile run `0f0e679c-ed19-45ad-bdf3-baf3460b43c1`; boundaries `e5f5d8d5-eb5d-46ae-8ed7-76ce0f9344e3`. Real subject PID37464 equals runner PID37464; executable `D:/Work/Games/KingdomZombies/.phase1/tools/Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe`, SHA256`45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab`, console wrapperSHA256`2445d009a5e0474fc7064b9767100e9d2c09521890ac5625476bfb81cf03f2d4`, official archiveSHA256`a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`. The engine reports `4.7.2.stable.mono.official.ed1daf0bf`. All ordinary,empty and wrong-case values resolve Foundation; explicit canonical feature resolves Inspection. Original native missing override,case normalization and global feature controls plus restored positive pass in their distinct owned projects. The bow remains16x18 with42 nonzero-alpha pixels. Current Root/index failure obligations are preserved and unmodified; these immediate scene/configuration observations do not certify eventual whole-pack validation.

| Exact current resource/source input | SHA256 |
| --- | --- |
| `src/godot-game/project.godot` | `f5378d9a802aee409cd71ddda626ea78be62e431ffbfbac8f8447d69082b09d0` |
| `src/godot-game/KingdomZombies.csproj` | `3ca7b050cd7cd3642771e295e4229ea57da3813119d5ef7921f711859df3871b` |
| `src/godot-game/content/expected-obligations.json` | `a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5` |
| `src/godot-game/foundation/Foundation.tscn` | `d4ed284d8bf53503fe560060527fbfc85d998c607750555fc905008177bf346e` |
| `src/godot-game/foundation/FoundationRoot.cs` | `530adae080b9d0811c9f48af7d1c92f3006f6e2bbabf01bd8303ec328040f8f8` |
| `src/godot-game/inspection/Inspection.tscn` | `6eacdca2448238b0234f61ae14ea3049db0f320aff3ecce7c330c31f70f448b3` |
| `src/godot-game/assets/common/bow.png` | `08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999` |
| `src/godot-game/inspection/InspectionRoot.cs` | `fa15e2bbe11269baded4eb0e1cfa5a08005a777a621eda6e89a19997280968ed` |
| `src/godot-game/assets/ui/InspectionTheme.tres` | `b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a` |
| `src/godot-game/assets/common/donor-kit.png` | `bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4` |
| `src/godot-game/assets/ui/Fallback.ttf` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` |
| `src/godot-game/assets/ui/LiberationSans.ttf` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` |
| `src/godot-game/assets/ui/PerfectDOSVGA437.ttf` | `ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102` |
| `src/godot-game/content/pack-index.json` | `cca59a20691e75e874f7ed4b229e838e43190b683f0885cc2aa5717f6ed92739` |
| `src/godot-game/content/packs/tracer.json` | `e21c933b36bacd5f81206b33f7a9680bedd89bd5e580b12f5004ab212895d1f4` |
| `src/godot-game/icon.svg` | `6c80384360a5b269d1054bfb27241258154e2cc8167c522016fdc1820e84e0f8` |
| `src/godot-game/docs/README.md` | `4d229df85a61e2bd64439c19a871983ee225988fe78624586a8faba69626edd0` |

The profile records108 current production-source/config hashes. Each strict build checks identical sources before/after; copied binary closure is bounded32files/16MiB each/64MiB aggregate. Importer consumes the exact closure png/svg/ttf/otf inputs, keeps the settings subject production project.godot untouched, and hashes generated .godot/imported outputs plus source import metadata. Each command/version/import/source/assembly/observed PID has its own immutable record; console wrapper and actual nonconsole subject are distinguished.

| Copied compiled runtime file | Bytes | SHA256 |
| --- | --- | --- |
| `src/godot-game/.godot/mono/temp/bin/Debug/GodotSharp.dll` | 5928960 | `2afe2686aacfefac36da6601b59aeb05e560f06e4970cd5036100acd2ca4249d` |
| `src/godot-game/.godot/mono/temp/bin/Debug/GodotSharpEditor.dll` | 303104 | `195c81c1ccb7c05559fc79b4acbb9025154a74ad99ecb9bb3747b5fbfc2ed1d1` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Core.dll` | 97280 | `440880ab3838de3086ddfc3b9360ba886aac440394b02bd19e65489d9e342b9f` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Core.pdb` | 32980 | `bd732931f6292f0748cd211d0969a448f71b6223663d81522c54af9d93900a0a` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Core.xml` | 28772 | `35dd6d282c11bf5f14bc76ded97865044324d5f07b5047543698ff643d5e12dd` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Multiplayer.Abstractions.dll` | 8192 | `c5dd5ec1897b62594a91fe960f0748cf2ef0771110532bf821d65bd44133e80a` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Multiplayer.Abstractions.pdb` | 12628 | `3dd437c729d9a02a1e7831afba6b626d1a839f4a345f55b8c708c03f8dea4575` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.Multiplayer.Abstractions.xml` | 10341 | `8653c78e02e3ff6db52e2cb7bff366305498f8d3467570b4b575948214fc6e3e` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.deps.json` | 2482 | `2697218d683aee1fe41a750aa66ab615b7f07dcf206648c78fcf8046c25c4861` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll` | 391168 | `949a731d38f1774997053cebade02b4cc10e4be78c645e65199da737f455cb21` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.pdb` | 151364 | `8fb1c33097619d3db931528ccfa1e6cfe1353a69272099a3d4bb348832e214a6` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.runtimeconfig.json` | 305 | `4ce55550bffe2e763ef3d418b5566fc16d1290811d9a8fd98f5a6a6cbd378f2b` |
| `src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.xml` | 52522 | `565fc5293fcdbc4d152a5fa0cbb928691c3a5f7ebdbcd067d078b18ff6aae4ce` |


## Final changed native-policy and producer observations

After final source stabilized, one actual engine.source-policy observation at.phase1/runs/8d330747-06fe-4ece-a7a8-2b021e5bcab7 reportsFAIL with1189 ungranted operations (183 more than historical1006): src/godot-game/inspection/BuildingPreview.cs=299, src/godot-game/inspection/CharacterPreview.cs=372, src/godot-game/inspection/CorpsePreview.cs=101, src/godot-game/inspection/CrowdPreview.cs=156, src/godot-game/inspection/IdentityKit.cs=78, src/godot-game/inspection/InventoryLayouts.cs=183. Exact path/line/column/member/host rows live in native-policy-observation.json. This is evidence for04-23, not native authorization or a compliance pass; WINDOWS30 remains open.

Actual evaluated producerGodot.SourceGenerators/4.7.2, authenticated packageSHA256ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e, has54 current inputs/32 emitted surfaces. generated-input-observation.json retains actual source bytes/raw/text hashes, generated raw/text hashes, package and exact production closure, plus changed-input identities relative to04-12. No producer enrollment/replay/permission occurs; WINDOWS31 remains open. WINDOWS29 and all ASSET acceptance remain pending. The known ledger entries29/30/31 remain open and are not duplicated or closed here.

## Artifacts this phase produces

The exact eight production/test/tool paths and current raw identities follow. No tracked new raster/font/archive/master/prompt was created. Existing selected donor/item/theme/font source bundles remain with04-04/05/14/28; this plan retains their exact packaged bindings below. Complete context/ID occurrences, original859 and legacy1452outcome IDs, constructor/H20 results, source declarations and historical input comparison are machine-enumerated in .phase4/04-15-r4-final/artifact-inventory-final.json (SHA256cb9c9bfafd3bc82c9a763eb497bdcea5258355991a43463a042648a093ca512c).

| Exact file | Bytes | Raw SHA256 | Git blob identities |
| --- | --- | --- | --- |
| `src/godot-game/inspection/InventoryLayouts.cs` | 61455 | `dbe540c0eaa06f8ee1a3e5f3b9f0c861383d5e0089220af3498f3de119c99f7a` | `7ee98d807cd457b40089e6447fa9eea37e5a3ee5` (raw=filtered) |
| `src/godot-game/content/ui/inventory.json` | 129595 | `a5feffc50318f4789b665fc40a1c3ea2182e99e666aa095a6432043ffd888524` | `d67280278e588bd863aa4fad331bde9a2fdd8a3d` (raw=filtered) |
| `src/godot-game/content/packs/ui-inventory.json` | 2060 | `7452f12311b7bcf07c9d1ebda80870a2aa28ead0d6217f1331231c47ce983df0` | `03eb772111ac5a9f42d995c9fbb2699860435546` (raw=filtered) |
| `tests/phase1/InventoryLayoutTests.cs` | 4622 | `2c60b6973ee82b63a9e5265fccfd1ed98a6413a2f51802ae11e5ede70b75b3cd` | `1c7607fa06053c131b9a52f1e2928a01d142baba` (raw=filtered) |
| `tools/phase4/cases/ui-inventory.cjs` | 57452 | `f3e2600d352ab3e3f019eeeb82153052e86885a83944f8619c69ef4062301246` | `3d76c10a4b2e65c2b345fde362f1fb3b97e1e4c4` (raw=filtered) |
| `src/godot-game/content/ui/legacy-stash-targets.json` | 1022146 | `30022521793976a185dae6b0b2140dadcc48b3a1c7b57327495444073a7cedc1` | `565db29c6ce5db5f7e3a8dfd8f069bd6e2779acd` (raw=filtered) |
| `src/godot-game/content/packs/ui-legacy-stash.json` | 2074 | `69943745397fe33982ea0f94cb752733911ae7178c0d5e489a0ae8f909b23058` | `15d8d4151bcc4a6b4c8b905740d309f65d137869` (raw=filtered) |
| `tools/phase4/cases/export-profile.cjs` | 42699 | `de2c3448e692050a36b2fb4ab28dd1f11508ec40e027836c7bf48c028e0947bd` | `1aca680c7bbd475658ad9c752abc428be0e010a2` (raw=filtered) |

### `src/godot-game/inspection/InventoryLayouts.cs`

Type: `internal sealed KingdomZombies.Inspection.InventoryLayouts`. Every authored member declaration is listed; array/object initializers are implementation state.

| Line | Exact declaration/signature |
| --- | --- |
| 17 | `internal InventoryLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs) {` |
| 32 | `internal static string[] SlotIds() {` |
| 39 | `internal static int[] Footprint(int width, int height, bool rotated) {` |
| 50 | `internal static JsonElement SavedLayout(JsonElement sample) {` |
| 56 | `internal static string[] TargetIds(JsonElement sample) {` |
| 63 | `internal static JsonElement? ResolveQuick(JsonElement state, int index) {` |
| 89 | `internal static JsonElement Read(byte[] bytes) {` |
| 105 | `internal void Open(string id) {` |
| 113 | `internal void Project(byte[] snapshot, long generation) {` |
| 145 | `internal void Close() {` |
| 153 | `private static readonly string[] _paths = ["content/ui/inventory.json", "content/config/items.json", "content/packs/ui-identity.json", "content/packs/items-01-10.json", "content/packs/items-11-24.json", "assets/equipment/items-01-10.png", "assets/equipment/items-11-24.png", "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf", "assets/ui/Fallback.ttf", "assets/common/donor-kit.png"];` |
| 158 | `private static readonly string[] _statuses = ["normal", "empty", "broken", "unbound", "reference-lost", "full", "invalid-fit", "over-depth", "world-drop", "transfer-success", "transfer-failure", "loading", "error", "partial", "overflow", "one", "long-text"];` |
| 161 | `private readonly bool _legacy;` |
| 162 | `private readonly Control _root;` |
| 163 | `private readonly MainThreadGateway _gateway;` |
| 164 | `private readonly JsonElement _pack;` |
| 165 | `private readonly JsonElement _data;` |
| 166 | `private readonly JsonElement _definitions;` |
| 167 | `private readonly JsonElement _icons;` |
| 168 | `private readonly List<Node> _owned = [];` |
| 169 | `private readonly List<AtlasTexture> _textures = [];` |
| 170 | `private readonly List<StyleBoxFlat> _styles = [];` |
| 171 | `private bool _accepting = true;` |
| 172 | `private long _generation;` |
| 174 | `private static bool Same(string left, string right) {` |
| 179 | `private static string Text(JsonElement value, string key) {` |
| 191 | `private static void CheckValue(JsonElement value, ref int count) {` |
| 238 | `private void Bind(byte[][] inputs) {` |
| 261 | `private static JsonElement Find(JsonElement rows, string id) {` |
| 273 | `private static void Unique(JsonElement rows) {` |
| 282 | `private void ValidateIcons(JsonElement weapons, JsonElement clothing) {` |
| 319 | `private static void ValidateDefinition(JsonElement definition) {` |
| 337 | `private static void ValidateRegion(JsonElement icon, JsonElement pack, bool first) {` |
| 352 | `private void ValidateData() {` |
| 379 | `private void ValidateState(JsonElement state) {` |
| 410 | `private void ValidateItem(JsonElement item, JsonElement grids) {` |
| 443 | `private void ValidateGrid(JsonElement grid, JsonElement items) {` |
| 479 | `private void ValidateSlots(JsonElement state) {` |
| 531 | `private static void ValidateHud(JsonElement hud) {` |
| 552 | `private static void ValidateOwnership(JsonElement state) {` |
| 594 | `private static void ValidateActions(JsonElement state) {` |
| 627 | `private int[] ItemSize(JsonElement item) {` |
| 635 | `private bool Fits(JsonElement state) {` |
| 673 | `private void Attach(JsonElement state) {` |
| 695 | `private void ProjectInventory(JsonElement state) {` |
| 726 | `private void ProjectSlots(JsonElement state, Node content) {` |
| 766 | `private void ProjectGrids(JsonElement state, Node content) {` |
| 800 | `private void AddCells(Control parent, int columns, int rows) {` |
| 815 | `private void AddPlaced(Control parent, JsonElement item) {` |
| 829 | `private void AddIcon(Node parent, JsonElement item, bool rotated) {` |
| 848 | `private string Details(JsonElement item) {` |
| 869 | `private void ProjectActions(JsonElement state, Node content) {` |
| 908 | `private void AddLabel(Node parent, string name, string text) {` |
| 916 | `private void ValidateLegacyData() {` |
| 937 | `private void ValidateLegacyState(JsonElement state) {` |
| 968 | `private void ValidateHistoric(JsonElement grid, bool missing) {` |
| 1011 | `private void ValidateCurrentGrid(JsonElement grid) {` |
| 1035 | `private void ValidateStash(JsonElement stash) {` |
| 1061 | `private void ValidateTargets(JsonElement state) {` |
| 1112 | `private void ProjectLegacy(JsonElement state) {` |
| 1135 | `private void ProjectHistoric(Node parent, JsonElement grid, string name, bool current) {` |
| 1160 | `private void ProjectHistoricItem(Control parent, JsonElement item, bool current) {` |
| 1188 | `private void ProjectStash(JsonElement state, Node content) {` |
| 1207 | `private void ProjectTargets(JsonElement state, Node content) {` |
| 1235 | `private void ProjectTargetRegion(Control parent, JsonElement target, string selected) {` |
| 1255 | `private void Clear() {` |

### `tests/phase1/InventoryLayoutTests.cs`

Type: `public sealed KingdomZombies.Phase1.Tests.InventoryLayoutTests`. Every authored member declaration is listed; array/object initializers are implementation state.

| Line | Exact declaration/signature |
| --- | --- |
| 16 | `public void Slots_PreserveElevenNamedIdentities() {` |
| 25 | `public void Rotation_ChangesFootprint() {` |
| 36 | `public void QuickSlot_UsesExactCurrentInstance() {` |
| 52 | `public void Legacy_PreservesSavedGeometryAfterFailedExtraction() {` |
| 66 | `public void Targets_PreserveSameNameDistinctIds() {` |
| 77 | `private static readonly string[] _expectedSlots = ["headwear", "face", "torso", "legs", "body-armor", "gloves", "boots", "backpack", "belt", "left-hand", "right-hand"];` |
| 79 | `private static readonly int[] _rotated = [2, 1];` |
| 80 | `private static readonly int[] _unrotated = [1, 2];` |
| 82 | `private static object? Call(string name, object[] arguments) {` |

### JavaScript contracts and proof resources

| Function | Parameters |
| --- | --- |
| `isolated` | `` |
| `sourceInputs` | `` |
| `validateSource` | `p,d` |
| `compiled` | `context` |
| `native` | `context,options={}` |
| `inventory` | `context` |
| `legacy` | `context` |

Exact CommonJS export sets: `SLOTS,PATHS,BACKSTOPS,HARNESS,sourceInputs,validateSource,compiled,native,inventory`; `LEGACY_PACK,LEGACY_DATA,LEGACY_HARNESS,legacy`. Case IDs layouts.inventory and layouts.legacy-target are saved source/headless G0 scenarios with importance/rationale/setup/ordered actions/expectations/reset/evidence/requirements/AC/backstop identities. Embedded HARNESS and LEGACY_HARNESS are proof-only native hosts, each invoking the actual compiled internal InventoryLayouts through reflection; they do not implement a test-owned inventory model. Their actual emitted source/project/hash lives in every run. InventoryProof.csproj, InventoryHarness.cs, probe.gd, subjects and JSON observation files are run-owned generated proof resources, not additional tracked production files/public identity. Neither adds an application scene or new Godot-generated production script.

Runtime node identity paths include Warning and Inventory/Scroll/Content; Title, Scope, HUD, Slots, Quick, Grids, Collection, Split, DragGhost/Fit, DragGhost/Footprint/Icon, Compatibility/Saved, Compatibility/CurrentDestination, Guidance, Extraction, Badges, Stash/Context/Pages/Filters/Feedback and each exact page ID; Targets/Selector/Regions and each exact target row/Identity/Transfer/Feedback/Slots/Contents. Native rows use ordinal resource_id metadata; dots sanitized by Node.Name are not identifiers. AtlasTexture views retain canonical_icon metadata, exact texture path/region, raw cropSHA256 and imported alpha/nonzero-alpha RGB. No alpha-zero RGB tolerance is applied before diagnosis/authentication.

### `src/godot-game/content/ui/inventory.json` state and data identities

Root `inventory-layouts`, version 1; fields `schemaVersion`, `id`, `version`, `sampleDataOnly`, `backstops`, `samples`. 571 exact context/ID occurrences (all JSON pointers retained in artifact-inventory-final.json), 75 distinct textual IDs. Repeated slot/item IDs belong to separately serialized samples; they are never merged by label.

| Sample ID | Status | Kind / exact selected target |
| --- | --- | --- |
| `inventory.normal` | `normal` | `inventory` |
| `inventory.empty` | `empty` | `inventory` |
| `inventory.broken` | `broken` | `inventory` |
| `inventory.unbound` | `unbound` | `inventory` |
| `inventory.reference-lost` | `reference-lost` | `inventory` |
| `inventory.full` | `full` | `inventory` |
| `inventory.invalid-fit` | `invalid-fit` | `inventory` |
| `inventory.over-depth` | `over-depth` | `inventory` |
| `inventory.world-drop` | `world-drop` | `inventory` |
| `inventory.transfer-success` | `transfer-success` | `inventory` |
| `inventory.transfer-failure` | `transfer-failure` | `inventory` |
| `inventory.loading` | `loading` | `inventory` |
| `inventory.error` | `error` | `inventory` |
| `inventory.partial` | `partial` | `inventory` |
| `inventory.overflow` | `overflow` | `inventory` |
| `inventory.one` | `one` | `inventory` |
| `inventory.long-text` | `long-text` | `inventory` |

All distinct authored IDs: `inventory-layouts`, `inventory.normal`, `headwear`, `face`, `torso`, `legs`, `body-armor`, `gloves`, `boots`, `backpack`, `belt`, `left-hand`, `right-hand`, `equipped-headwear`, `equipped-face`, `equipped-torso`, `equipped-legs`, `equipped-body-armor`, `equipped-gloves`, `equipped-boots`, `equipped-backpack`, `equipped-belt`, `equipped-left-hand`, `equipped-right-hand`, `berries-a`, `berries-b`, `fruit`, `meat`, `money`, `pouch`, `knife`, `shirt`, `trousers`, `bag`, `nested`, `inventory.empty`, `inventory.broken`, `inventory.unbound`, `inventory.reference-lost`, `inventory.full`, `full-0-0`, `full-1-0`, `full-2-0`, `full-3-0`, `full-0-1`, `full-1-1`, `full-2-1`, `full-3-1`, `full-0-2`, `full-1-2`, `full-2-2`, `full-3-2`, `full-0-3`, `full-1-3`, `full-2-3`, `full-3-3`, `inventory.invalid-fit`, `inventory.over-depth`, `inventory.world-drop`, `inventory.transfer-success`, `inventory.transfer-failure`, `inventory.loading`, `inventory.error`, `inventory.partial`, `inventory.overflow`, `overflow-0-0`, `overflow-1-0`, `overflow-0-1`, `overflow-1-1`, `overflow-0-2`, `overflow-1-2`, `overflow-0-3`, `overflow-1-3`, `inventory.one`, `inventory.long-text`.

Backstops: `UI-COLLECTIONS/empty`, `UI-COLLECTIONS/loading`, `UI-COLLECTIONS/error`, `UI-COLLECTIONS/populated`, `UI-COLLECTIONS/partial`, `UI-COLLECTIONS/overflow`, `UI-COLLECTIONS/zero-one-many`, `UI-COLLECTIONS/long-text`.

### `src/godot-game/content/ui/legacy-stash-targets.json` state and data identities

Root `legacy-stash-target-layouts`, version 1; fields `id`, `version`, `sampleDataOnly`, `gameplayTransactions`, `filtering`, `backstops`, `samples`. 3530 exact context/ID occurrences (all JSON pointers retained in artifact-inventory-final.json), 84 distinct textual IDs. Repeated slot/item IDs belong to separately serialized samples; they are never merged by label.

| Sample ID | Status | Kind / exact selected target |
| --- | --- | --- |
| `legacy.shrink` | `shrink` | `legacy` / `corpse-alex` |
| `legacy.slot` | `incompatible-slot` | `legacy` / `corpse-alex` |
| `legacy.stack` | `oversized-stack` | `legacy` / `corpse-alex` |
| `legacy.depth` | `over-depth` | `legacy` / `corpse-alex` |
| `legacy.extraction-failure` | `extraction-failure` | `legacy` / `corpse-alex` |
| `legacy.empty-current` | `empty-adopt-current` | `legacy` / `corpse-alex` |
| `legacy.missing-definition` | `missing-definition` | `legacy` / `corpse-alex` |
| `stash.normal` | `normal` | `stash` / `corpse-alex` |
| `stash.empty` | `empty` | `stash` / `corpse-alex` |
| `stash.no-match` | `no-match` | `stash` / `corpse-alex` |
| `stash.stock-changed` | `stock-changed` | `stash` / `corpse-alex` |
| `stash.losing-operation` | `losing-operation` | `stash` / `corpse-alex` |
| `stash.full-page` | `full-page-new-page` | `stash` / `corpse-alex` |
| `target.npc` | `npc` | `target` / `npc-alex` |
| `target.corpse` | `corpse` | `target` / `corpse-alex` |
| `target.ground` | `ground` | `target` / `pile-alex` |
| `target.emptied` | `emptied` | `target` / `corpse-alex` |
| `target.out-of-range` | `out-of-range` | `target` / `corpse-alex` |
| `target.dead` | `dead` | `target` / `corpse-alex` |
| `target.closed` | `access-closed` | `target` / `corpse-alex` |
| `target.overlap` | `overlap` | `target` / `corpse-alex` |
| `target.empty-pile` | `empty-pile` | `target` / `pile-alex` |

All distinct authored IDs: `legacy-stash-target-layouts`, `legacy.shrink`, `inventory.normal`, `headwear`, `face`, `torso`, `legs`, `body-armor`, `gloves`, `boots`, `backpack`, `belt`, `left-hand`, `right-hand`, `equipped-headwear`, `equipped-face`, `equipped-torso`, `equipped-legs`, `equipped-body-armor`, `equipped-gloves`, `equipped-boots`, `equipped-backpack`, `equipped-belt`, `equipped-left-hand`, `equipped-right-hand`, `berries-a`, `berries-b`, `fruit`, `meat`, `money`, `pouch`, `knife`, `shirt`, `trousers`, `bag`, `nested`, `saved-old`, `old-knife`, `old-berries`, `old-bag`, `destination-shirt`, `page-123456`, `stash-berries`, `stash-knife`, `npc-alex`, `npc-alex.gear`, `npc-alex.contents`, `npc-alex.berries`, `npc-alex.knife`, `corpse-alex`, `corpse-alex.gear`, `corpse-alex.contents`, `corpse-alex.berries`, `corpse-alex.knife`, `pile-alex`, `pile-alex.gear`, `pile-alex.contents`, `pile-alex.berries`, `pile-alex.knife`, `legacy.slot`, `legacy.stack`, `legacy.depth`, `legacy.extraction-failure`, `legacy.empty-current`, `emptied-adopted`, `legacy.missing-definition`, `stash.normal`, `stash.empty`, `stash.no-match`, `stash.stock-changed`, `stash.losing-operation`, `stash.full-page`, `full-page`, `full-field-bag`, `next-empty-page`, `target.npc`, `target.corpse`, `target.ground`, `target.emptied`, `target.out-of-range`, `target.dead`, `target.closed`, `target.overlap`, `target.empty-pile`.

Backstops: `H20`.

#### `public sealed partial KingdomZombies.InventoryProof.InventoryHarness : Node (run-owned only)`

Exact source `.phase4/runs/4d451ef8-6a9a-4ea3-b1de-24f9b08715fb/native-inventory/InventoryHarness.cs` SHA256`df72abf08e033dbd9bd71de2d1b82d2884d7a730af27fe0428c857db34201f43`.

| Line | Actual proof-only method signature |
| --- | --- |
| 17 | `private void Check(string id,bool pass,object observed)` |
| 18 | `private static bool Eq(string? x,string? y)=>StringComparer.Ordinal.Equals(x,y)` |
| 19 | `private static string Digest(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant()` |
| 20 | `private static byte[] Read(string file)=>System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://"+file))` |
| 21 | `private static object? Invoke(object subject,string name,params object[] args)=>subject.GetType().GetMethod(name,BindingFlags.Instance\|BindingFlags.NonPublic)!.Invoke(subject,args)` |
| 22 | `private static Exception? Failure(Action action)` |
| 23 | `private object New(Control root,byte[]? pack=null,byte[][]? inputs=null)` |
| 24 | `private static Control Row(Node parent,string id)` |
| 25 | `private static string Label(Node root,string path)=>root.GetNode<Label>(path)` |
| 26 | `public override async void _Ready()` |
| 27 | `private async Task Run()` |
| 54 | `private async Task GhostControls()` |
| 65 | `private void EmptyGridControls()` |
| 71 | `private void Canonical(string id,TextureRect image,JsonElement expected)` |
| 72 | `private void Controls(Control live)` |

#### `public sealed partial KingdomZombies.InventoryProof.InventoryHarness : Node (run-owned only)`

Exact source `.phase4/runs/f86514c5-470b-4622-ae60-2c6b4ff8bd62/native-inventory/InventoryHarness.cs` SHA256`cc2494596a3b8d0bdb59fb6b7fcb9a9bc9bb3f70405c3be0e932128e1187c873`.

| Line | Actual proof-only method signature |
| --- | --- |
| 17 | `private void Check(string id,bool pass,object observed)` |
| 18 | `private static bool Eq(string? x,string? y)=>StringComparer.Ordinal.Equals(x,y)` |
| 19 | `private static string Digest(byte[] b)=>Convert.ToHexString(SHA256.HashData(b)).ToLowerInvariant()` |
| 20 | `private static byte[] Read(string file)=>System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://"+file))` |
| 21 | `private static object? Invoke(object subject,string name,params object[] args)=>subject.GetType().GetMethod(name,BindingFlags.Instance\|BindingFlags.NonPublic)!.Invoke(subject,args)` |
| 22 | `private static Exception? Failure(Action action)` |
| 23 | `private object New(Control root,byte[]? pack=null,byte[][]? inputs=null)` |
| 24 | `private static Control Row(Node parent,string id)` |
| 25 | `private static string Label(Node root,string path)=>root.GetNode<Label>(path)` |
| 26 | `public override async void _Ready()` |
| 28 | `private async Task Run()` |
| 43 | `private async Task Frames()` |
| 44 | `private void Inspect(Control root,JsonElement state,string id,JsonElement icons)` |
| 50 | `private void Grid(Node section,JsonElement grid,string id,bool current,JsonElement icons)` |
| 53 | `private void ConstructorControls()` |
| 59 | `private void Canonical(string id,TextureRect image,JsonElement expected)` |

Actual authored native Name/name literals: `Inventory`, `Scroll`, `Content`, `Title`, `Scope`, `Warning`, `HUD`, `Weight`, `Teammates`, `Slots`, `resource_id`, `Location`, `Empty`, `Item`, `Quick`, `Grids`, `Cells`, `Details`, `Collection`, `Cell`, `Count`, `Icon`, `canonical_icon`, `DragGhost`, `Footprint`, `Fit`, `Split`, `Transfer`, `WorldDrop`, `Actions`, `CharacterContext`, `Compatibility`, `Notice`, `Badges`, `Saved`, `CurrentDestination`, `Guidance`, `Extraction`, `Missing`, `Identity`, `Stash`, `Context`, `Pages`, `Filters`, `Feedback`, `Targets`, `Selector`, `Regions`, `Contents`, `panel`, `Marker`. Dynamic stable IDs come from the separately inventoried data identities.

### Exact packaged source/resource bindings

Both pack IDs are version1, schemaVersion1, sampleDataOnly=true/gameplayTransactions=false, presenter KingdomZombies.Inspection.InventoryLayouts, entry Open(string id). ui-inventory owns UI-05/UI-12/UI-13 and ui-legacy-stash owns UI-06/UI-14/UI-15. completedObligations=[] and visualReview=null intentionally retain later acceptance status; they are not missing projection stubs or acceptance passes. Each pack has exactly twelve ordered dependencies:

| Exact packaged dependency | ui-inventory SHA256 | ui-legacy-stash SHA256 |
| --- | --- | --- |
| `content/ui/inventory.json` / `content/ui/legacy-stash-targets.json` | `a5feffc50318f4789b665fc40a1c3ea2182e99e666aa095a6432043ffd888524` | `30022521793976a185dae6b0b2140dadcc48b3a1c7b57327495444073a7cedc1` |
| `content/config/items.json` | `fa9bb09bfc3a999dd21349274f5010e5a653a868d4a0d0be73a46caaef226720` | `fa9bb09bfc3a999dd21349274f5010e5a653a868d4a0d0be73a46caaef226720` |
| `content/packs/ui-identity.json` | `324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed` | `324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed` |
| `content/packs/items-01-10.json` | `9402295cc6457d42d53a7b4a8ada19bc06711d6db8f600386f49b05a238686b7` | `9402295cc6457d42d53a7b4a8ada19bc06711d6db8f600386f49b05a238686b7` |
| `content/packs/items-11-24.json` | `78761a40c0e0b7a22ef9885ee847338fd7315ce69014756608a0bc965fe0a826` | `78761a40c0e0b7a22ef9885ee847338fd7315ce69014756608a0bc965fe0a826` |
| `assets/equipment/items-01-10.png` | `3d8b8e3d6aa0a2e94dc3fba8b64cc54573b62031be257db76c79fa2cac53cfe9` | `3d8b8e3d6aa0a2e94dc3fba8b64cc54573b62031be257db76c79fa2cac53cfe9` |
| `assets/equipment/items-11-24.png` | `b5dafe0a20348a9b667e9eda74c4250fdcbf2ce2fffac38ab7193b19059a4219` | `b5dafe0a20348a9b667e9eda74c4250fdcbf2ce2fffac38ab7193b19059a4219` |
| `assets/ui/InspectionTheme.tres` | `b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a` | `b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a` |
| `assets/ui/LiberationSans.ttf` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` |
| `assets/ui/PerfectDOSVGA437.ttf` | `ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102` | `ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102` |
| `assets/ui/Fallback.ttf` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` | `e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49` |
| `assets/common/donor-kit.png` | `bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4` | `bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4` |

Retained new bundle members: none. All borrowed canonical item forms, font licenses, original donor parts, original prompts and recipes remain in their existing source bundles and are not newly authored/recounted here. No donor-root write or runtime donor-folder dependency was introduced.

### Exact new native-policy tuples

All rows below have exact production path src/godot-game/inspection/InventoryLayouts.cs, namespace KingdomZombies.Inspection, sealed internal type InventoryLayouts. These183 operation sites are current observed missing-policy tuples grouped by exact synchronous host; they remain ungranted. All current cross-file operations are retained in the full policy JSON above. Captured _gateway.RequireThread guards are retained at each native boundary; observation data is not a class-wide permission.

| Exact synchronous host signature | Observed native operation tuples (line:column member) |
| --- | --- |
| `internal void Project(byte[] snapshot, long generation) {` | `126:58 Godot.Node.IsInsideTree()` |
| `private void Attach(JsonElement state) {` | `676:21 Godot.PanelContainer.PanelContainer()`; `676:42 Godot.Node.Name`; `676:62 Godot.Control.Position`; `676:73 Godot.Vector2.Vector2(float, float)`; `677:17 Godot.Control.Size`; `677:24 Godot.Vector2.Vector2(float, float)`; `677:54 Godot.Control.Theme`; `677:62 Godot.ResourceLoader.Load<Godot.Theme>(string, string, Godot.ResourceLoader.CacheMode)`; `679:22 Godot.ScrollContainer.ScrollContainer()`; `679:44 Godot.Node.Name`; `680:23 Godot.VBoxContainer.VBoxContainer()`; `680:43 Godot.Node.Name`; `680:61 Godot.Control.SizeFlagsHorizontal`; `680:83 Godot.Control.SizeFlags.ExpandFill`; `681:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `683:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `684:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `688:23 Godot.Label.Label()`; `688:35 Godot.Node.Name`; `688:53 Godot.Label.Text`; `689:17 Godot.CanvasItem.ZIndex`; `689:31 Godot.Label.AutowrapMode`; `689:46 Godot.TextServer.AutowrapMode.WordSmart`; `690:17 Godot.Control.Size`; `690:24 Godot.Vector2.Vector2(float, float)`; `690:53 Godot.Control.Theme`; `690:61 Godot.Control.Theme`; `691:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectInventory(JsonElement state) {` | `698:24 Godot.Node.GetNode(Godot.NodePath)` |
| `private void ProjectSlots(JsonElement state, Node content) {` | `729:21 Godot.VBoxContainer.VBoxContainer()`; `729:41 Godot.Node.Name`; `730:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `734:23 Godot.HBoxContainer.HBoxContainer()`; `734:43 Godot.Node.Name`; `735:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `736:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `752:21 Godot.VBoxContainer.VBoxContainer()`; `752:41 Godot.Node.Name`; `753:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectGrids(JsonElement state, Node content) {` | `769:21 Godot.VBoxContainer.VBoxContainer()`; `769:41 Godot.Node.Name`; `770:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `773:27 Godot.VBoxContainer.VBoxContainer()`; `773:47 Godot.Node.Name`; `774:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `775:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `782:27 Godot.Control.Control()`; `782:41 Godot.Node.Name`; `783:21 Godot.Control.CustomMinimumSize`; `783:41 Godot.Vector2.Vector2(float, float)`; `784:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void AddCells(Control parent, int columns, int rows) {` | `807:17 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `807:39 Godot.ColorRect.ColorRect()`; `807:55 Godot.Node.Name`; `809:25 Godot.Control.Position`; `809:36 Godot.Vector2.Vector2(float, float)`; `809:71 Godot.Control.Size`; `809:78 Godot.Vector2.Vector2(float, float)`; `810:25 Godot.ColorRect.Color`; `810:33 Godot.Color.FromHtml(System.ReadOnlySpan<char>)`; `810:66 Godot.Control.MouseFilter`; `810:80 Godot.Control.MouseFilterEnum.Ignore` |
| `private void AddPlaced(Control parent, JsonElement item) {` | `819:19 Godot.Control.Control()`; `819:33 Godot.Node.Name`; `820:17 Godot.Control.Position`; `820:28 Godot.Vector2.Vector2(float, float)`; `822:17 Godot.Control.Size`; `822:24 Godot.Vector2.Vector2(float, float)`; `822:71 Godot.Control.TooltipText`; `822:106 Godot.CanvasItem.ZIndex`; `823:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `824:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| `private void AddIcon(Node parent, JsonElement item, bool rotated) {` | `835:23 Godot.AtlasTexture.AtlasTexture()`; `835:42 Godot.AtlasTexture.Atlas`; `835:50 Godot.ResourceLoader.Load<Godot.Texture2D>(string, string, Godot.ResourceLoader.CacheMode)`; `836:55 Godot.AtlasTexture.Region`; `836:64 Godot.Rect2.Rect2(float, float, float, float)`; `837:43 Godot.AtlasTexture.FilterClip`; `839:21 Godot.TextureRect.TextureRect()`; `839:39 Godot.Node.Name`; `839:54 Godot.TextureRect.Texture`; `839:73 Godot.Control.Size`; `839:80 Godot.Vector2.Vector2(float, float)`; `840:17 Godot.Control.CustomMinimumSize`; `840:37 Godot.Vector2.Vector2(float, float)`; `840:64 Godot.TextureRect.ExpandMode`; `840:77 Godot.TextureRect.ExpandModeEnum.IgnoreSize`; `841:17 Godot.TextureRect.StretchMode`; `841:31 Godot.TextureRect.StretchModeEnum.KeepAspectCentered`; `842:17 Godot.CanvasItem.TextureFilter`; `842:33 Godot.CanvasItem.TextureFilterEnum.Nearest`; `842:71 Godot.Control.RotationDegrees`; `843:17 Godot.Control.PivotOffset`; `843:31 Godot.Vector2.Vector2(float, float)`; `843:58 Godot.Control.MouseFilter`; `843:72 Godot.Control.MouseFilterEnum.Ignore`; `844:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `845:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectActions(JsonElement state, Node content) {` | `876:25 Godot.HBoxContainer.HBoxContainer()`; `876:45 Godot.Node.Name`; `877:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `882:26 Godot.Control.Control()`; `882:40 Godot.Node.Name`; `882:60 Godot.Control.CustomMinimumSize`; `882:80 Godot.Vector2.Vector2(float, float)`; `883:39 Godot.Control.SizeFlagsVertical`; `883:59 Godot.Control.SizeFlags.ShrinkCenter`; `884:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `885:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `887:13 Godot.Control.Position`; `887:13 Godot.Node.GetNode<Godot.TextureRect>(Godot.NodePath)`; `887:66 Godot.Vector2.Vector2(float, float)` |
| `private void AddLabel(Node parent, string name, string text) {` | `911:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `911:31 Godot.Label.Label()`; `911:43 Godot.Node.Name`; `911:56 Godot.Label.Text`; `911:69 Godot.Label.AutowrapMode`; `911:84 Godot.TextServer.AutowrapMode.WordSmart`; `912:17 Godot.Control.SizeFlagsHorizontal`; `912:39 Godot.Control.SizeFlags.ExpandFill` |
| `private void ProjectLegacy(JsonElement state) {` | `1115:24 Godot.Node.GetNode(Godot.NodePath)`; `1119:22 Godot.VBoxContainer.VBoxContainer()`; `1119:42 Godot.Node.Name`; `1120:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectHistoric(Node parent, JsonElement grid, string name, bool current) {` | `1138:23 Godot.VBoxContainer.VBoxContainer()`; `1138:43 Godot.Node.Name`; `1139:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1140:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `1145:23 Godot.Control.Control()`; `1145:37 Godot.Node.Name`; `1145:53 Godot.Control.ClipContents`; `1145:74 Godot.Control.CustomMinimumSize`; `1145:94 Godot.Vector2.Vector2(float, float)`; `1148:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectHistoricItem(Control parent, JsonElement item, bool current) {` | `1167:19 Godot.Control.Control()`; `1167:33 Godot.Node.Name`; `1167:70 Godot.CanvasItem.ZIndex`; `1168:17 Godot.Control.Position`; `1168:28 Godot.Vector2.Vector2(float, float)`; `1170:17 Godot.Control.Size`; `1170:24 Godot.Vector2.Vector2(float, float)`; `1171:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1172:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `1180:13 Godot.Control.TooltipText` |
| `private void ProjectStash(JsonElement state, Node content) {` | `1192:21 Godot.VBoxContainer.VBoxContainer()`; `1192:41 Godot.Node.Name`; `1193:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| `private void ProjectTargets(JsonElement state, Node content) {` | `1210:23 Godot.VBoxContainer.VBoxContainer()`; `1210:43 Godot.Node.Name`; `1211:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1214:19 Godot.Control.Control()`; `1214:33 Godot.Node.Name`; `1214:51 Godot.Control.CustomMinimumSize`; `1214:71 Godot.Vector2.Vector2(float, float)`; `1215:17 Godot.Control.ClipContents`; `1216:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1220:23 Godot.VBoxContainer.VBoxContainer()`; `1220:43 Godot.Node.Name`; `1221:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1222:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| `private void ProjectTargetRegion(Control parent, JsonElement target, string selected) {` | `1239:19 Godot.Panel.Panel()`; `1239:31 Godot.Node.Name`; `1240:17 Godot.Control.Position`; `1240:28 Godot.Vector2.Vector2(float, float)`; `1241:17 Godot.Control.Size`; `1241:24 Godot.Vector2.Vector2(float, float)`; `1242:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)`; `1243:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)`; `1246:27 Godot.StyleBoxFlat.StyleBoxFlat()`; `1246:46 Godot.StyleBoxFlat.BgColor`; `1246:56 Godot.Color.Color(float, float, float, float)`; `1247:21 Godot.StyleBoxFlat.BorderColor`; `1247:35 Godot.Color.FromHtml(System.ReadOnlySpan<char>)`; `1247:68 Godot.StyleBoxFlat.BorderWidthLeft`; `1247:89 Godot.StyleBoxFlat.BorderWidthRight`; `1248:21 Godot.StyleBoxFlat.BorderWidthTop`; `1248:41 Godot.StyleBoxFlat.BorderWidthBottom`; `1250:13 Godot.Control.AddThemeStyleboxOverride(Godot.StringName, Godot.StyleBox)` |
| `private void Clear() {` | `1260:13 Godot.Node.RemoveChild(Godot.Node)`; `1261:13 Godot.GodotObject.Free()`; `1267:13 Godot.GodotObject.Dispose()`; `1273:13 Godot.GodotObject.Dispose()` |

## Deviations from Plan

The original two-task contract is preserved; the committed prospective export repair adds only the existing export-profile.cjs, making eight paths. Prospective technical repair authorities R1/R2/R3 are explicit additions documented above: Rule1 production ghost transform repair only underR2; Rule3 pinned observation/strict formatting/proof-owner repairs under exact scopes. The R4 authority below separately permits only the named exact theme-reader exception and authentic consumer closure. No other gameplay/public API/policy/parser/deadline/output-limit change is granted. No authentication gate, package install, architectural substitution or source-security surface outside the plan was introduced. Byte-exact files and accepted tests meet current AGENTS.md conventions; strict analyzers remain enabled and no hook was bypassed.

Stub scan: no TODO/FIXME/unwired gameplay substitute prevents either local plan goal. Empty collections are real empty/reference-lost sample states or owner tracking; null visual review and empty completion arrays intentionally preserve pending actual acceptance. No selected required test is skipped. All three planned automated verification commands execute successfully through the canonical inventory cases; broader blocked integration and actual acceptance remain explicitly visible above and in WINDOWS29/30/31.

## Task commits and measured cost

- `18d4a8c test(04-15): expose missing inventory identities rotation and references`
- `b68ced9 docs(04-15): preserve original inventory correction checkpoint`
- `7d61595 docs(04-15): scope inventory tracer verification repair`
- `72386b6 docs(04-15): preserve native checkpoint and scope ghost repair`
- `e7ccc86 feat(04-15): project native inventory grids and exact food references`
- `ca9e586 test(04-15): expose lost legacy geometry and same-name target identities`
- `ce8f127 docs(04-15): preserve task2 original correction checkpoint`
- `2904995 docs(04-15): scope legacy layout verification repair`
- `9dc2f3e docs(04-15): preserve R3 export profile size checkpoint`
- `468641d docs(04-15): scope current export resource fixture repair`
- `9832318 feat(04-15): preserve legacy stash and target sample identities`
- `54f7d77 feat(04-15): preserve legacy layout and target native state`
- `14968cc fix(04-15): authenticate current export profile closure`

The persisted plan ledger measures13commits before the SUMMARY metadata commit, from6faa5e291e4117271dc445c9793ea65c6a2ec9ad to14968cc064263dff24e31ad94edc0a343d846ae9. Realized exact eight-path Git diff is1347091characters, chars/4 rounded up=336773estimate-scale tokens versus the45000estimate; this is not harness token usage. Ledger window2026-11-17T17:15:01+01:00 through2026-11-17T18:18:53.542Z is123.88minutes and includes original/repair/checkpoint history; it is not only final verification duration. No tracked file deletion occurred. SUMMARY/self-check commit follows production commits immediately; parent performs global completion hooks afterward.

## Next owner handoff

Owner23 wires actual InventoryLayouts and both packs into complete Root/index/dispatch, reconciles exact native tuples without weakening controls, repairs WINDOWS29 and reruns all blocked provenance/publication/current resource cases. Owner31 preserves this scoped profile consumer repair, handles its prospectively approved later application identity/config changes without rewriting historical ordinary-config evidence, adopts/export-packages actual current resources and authenticates the final producer/delivery. Preserve original846/859 identities, thirteen appended ghost/grid/owner controls, all22 legacy states, independentH20, strict bounded pre-native rejection, canonical pixels, actual item/target identity and deterministic cleanup. Owner32 provides whole-phase post-review actual Windows1280x720/noVM appearance/input/listening/named judgments;33 performs final reconciliation. Filtering gameplay remains12-owned. Parent continues13/wave18, keeps concurrent Phase06/spec/Phase12 history intact and never recreates the removed06-DISCUSS-CHECKPOINT.json.

## Self-Check: PASSED

All eight intended implementation paths exist and match the final authenticated raw SHA256 inventory. All 13 measured pre-summary commits exist and are ancestors of HEAD14968cc; both genuine RED commits precede their GREEN implementations. The complete fresh executable selection passed33/33 on430 frozen inputs with zero drift, and the inventory retains all859 original outcome IDs in exact order. No tracked file deletion occurred. Foreign global planning and specification edits remain unstaged and untouched. The SUMMARY metadata commit follows these verified production commits; global completion state remains coordinator-owned.
