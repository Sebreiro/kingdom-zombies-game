---
phase: 04-asset-production
plan: "10"
subsystem: persistent-appearance-crowds-and-wildlife
status: complete
tags: [Godot, CSharp, corpses, wildlife, identities, crowds, validation]
requires:
  - phase: "04-09"
    provides: "Current body/gear composition, bow/work state and original fitting/animation proofs"
  - phase: "04-28"
    provides: "Canonical four number/name/shape/color identities"
provides:
  - "Persistent equipped humanoid and wildlife corpse endpoints, current remaining loot and silent restoration"
  - "Actual 600-owner crowd, four redundant identities and retained wildlife cycles"
  - "Constructor validation of authenticated atlas/parts/clips/references/polygons with 80 native controls"
affects: ["04-12", "04-13", "04-23", "04-24", "04-31", "04-32", "04-33"]
tech-stack:
  added: []
  patterns: ["Detached current appearance independent of transient effects", "Guarded control-free composition reuse", "Exact parent-rebound malformed native fixtures", "Frozen source and per-invocation assembly identities"]
key-files:
  created: ["src/core/AppearanceState.cs", "src/godot-game/inspection/CorpsePreview.cs", "src/godot-game/content/packs/corpses-wildlife.json", "tests/phase1/AppearanceStateTests.cs", "tools/phase4/cases/corpses.cjs", "src/godot-game/inspection/CrowdPreview.cs", "src/godot-game/content/fixtures/crowds.json", "src/godot-game/content/packs/player-identities.json", "src/godot-game/assets/characters/identity-wildlife.png"]
  modified: ["docs/code/visibility-exemptions.json", "tests/phase1/ArchitecturePolicy.cs", "src/godot-game/inspection/CharacterPreview.cs"]
key-decisions:
  - "Current gear and persistent endpoint outlive transient effects; restoration stays silent and crown/structural guard restrictions remain explicit."
  - "Reuse actual body/item render helpers without per-owner controls/subscriptions and preserve the original static-fitting scalar field layout."
  - "Use existing K12/K13/K21 source members and the canonical existing UI palette; no new original master or archive."
  - "Validate definitions before native attachment/cropping/animation/polygon use; malformed input is rejected rather than repaired."
requirements-addressed: [ASSET-02, ASSET-03, ASSET-04]
requirements-completed: []
actuals:
  tokens: 120653
  diff_chars: 482611
  tasks: 2
  commits: 10
  plan_head_before: 6b510d5d6ae1d0a0ac81921392672aef49aedd44
  plan_head_after: d2392ff33e682d774e5999728496919f1e9dc62d
  phase_owned_commits: 8
  foreign_commits_excluded_from_phase_attribution: 2
duration: 107.36min
completed: 2026-11-17
coverage:
  - id: persistent-appearance-and-crowds
    description: "Current equipped corpse/loot/wildlife composition and actual crowd/identity resources"
    verification:
      - kind: integration
        ref: ".phase4/04-10-r1-final/completion.json: fresh20/20 frozen source/headless selection"
        status: pass
    human_judgment: false
  - id: strict-native-definition-boundary
    description: "80 actual constructor controls with specific controlled diagnostics and detached invalid owners"
    verification:
      - kind: integration
        ref: ".phase4/runs/26664167-adc1-480b-b82e-8163f654c40c/appearance-observation.json: definitionRows"
        status: pass
    human_judgment: false
  - id: actual-appearance-and-phase-acceptance
    description: "Required rendered readability/appearance, real input/listening and phase acceptance"
    verification: []
    human_judgment: true
    rationale: "Whole-phase production and independent review/fixes precede04-32 Windows1280x720/noVM actual input/listening/named judgments and04-33 reconciliation."
---

# Phase 04 Plan 10: Persistent corpses, identities, wildlife and real crowds Summary

**Current equipped corpse and loot endpoints survive effect expiry; actual 600-owner crowds reuse real body/gear media, four redundant identities and validated retained wildlife cycles.**

## Performance and accounting

Started at persisted ledger base 6b510d5d6ae1d0a0ac81921392672aef49aedd44: 2026-11-17T14:09:57+01:00; completed 2026-11-17T14:57:18.403Z; 107.36 minutes including preserved checkpoint/repair history. Two tasks complete. Twelve exact owned paths leave the distinct Phase04 inventory at236. Realized owned-path Git diff 482611 characters /4 rounded up =120653 estimate-scale tokens, including binary diff notices; this is not harness usage.

The mandatory ledger interval measures 10 commits before this summary metadata commit. It includes two foreign shared-checkout ancestors,4127fd5 Phase6 context andca4b82b STATE session; neither is Phase4 implementation or included in the 8 phase-owned commit attribution/diff. No ref was rewound or foreign edit staged. Parent owns global19→20/current10→12/completed44→45, requirement/inventory tracking and draftPR5 todev. Global STATE/ROADMAP/REQUIREMENTS are deliberately left to that single writer.

## Accomplishments

AppearanceState keeps detached, bounded current body/kind/equipment/selection separate from explicit death/collapse/effect timing and event counts. All eight humanoids keep stable authored corpse poses after effects expire; removing any current bag/coat/headgear/weapon removes only that layer. Repeated restore/reset reconstructs current composition without old one-shots. Crown disappears at death and never becomes loot; structural guards have no character corpse, inventory or loot. Rabbit/deer retain exact final endpoints and optional actual meat carriers. Empty/nonempty/repopulated piles and selected targets use actual item media, steady thin source-derived outlines and modest markers.

CharacterPreview's guarded internal OpenComposition/RestoreComposition/CloseComposition reuses AddBody/AddItem/AddLayer/Texture without per-owner controls, clocks or subscriptions. Its original constructor/Open/static FitItem, eight bodies, immutable borrowed textures, logical hands, bow/work lifecycle and scalar stored layout remain supported by unchanged regression proofs.

CrowdPreview creates100 survivors/300 zombies/100 persistent equipped corpses/100 nonempty piles, with five real current gear slots per sample, quiet shadows, day/night and overlapping-target selection/removal. World/teammate/minimap identities use P1 square#F0C46A,P2 triangle#74BDE0,P3 diamond#DB91B8,P4 cross#A3CC83, exact existing UI vectors and intentional duplicate Alex names. Real K21 crown transfers alongside current headgear. Rabbit/deer idle/move/look/eat/flee/hit/death/corpse choices use retained source clips and endpoints; hit is a small visual flinch with no stun or delay.

Strict constructor validation runs before Attach or unsafe native projection. The atlas path must equal its canonical assets/characters/identity-wildlife.png identity before referenced I/O, SHA256 must authenticate the PNG and declared dimensions must match its actual IHDR. Unique ordinal part/clip names are1..160 characters; bounded arrays use4096, authenticated atlas spans bound offsets/shape points, and the existing0..60 preview clock bounds positive clip stop. Rectangles have exactly four integers, positive sizes and overflow-safe containment. Fractional registration stays exact. Frames are nonempty, finite, nonnegative, start at0, increase strictly and stay through the inclusive stop endpoint; exact death endpoint selection remains authored. Every consumed rabbit/deer action/corpse reference and all four required simple/nondegenerate polygons validate. Nothing sorts, clamps, drops or substitutes malformed source data.

## Task commits and RED/tracer

Task1 genuine compiled RED2d7ece7 executes Death_AfterTransientExpiryPreservesCurrentEquippedCorpse and fails expectedTrue/actualFalse,1test/1failure/0skip/exit2, accepted RED_EVIDENCE_OK. Core dff9ec7, renderer ac75206 and corpse/source-case6796377 commit the approved current-state behavior. Mandatory fresh committed tracer f4d33c1e-7520-4dab-b5ff-9e31d1bea609 passes before Task2 edits; unchanged bodies.a-fit5baa4650 passes10/10 plus2976tuples/5972native/12H13. Original RED and build failures remain immutable under.phase4/04-10-red and04-10-CHECKPOINT.

| Commit | Scope |
| --- | --- |
| 2d7ece77fe6c7ac57f57b9fc4a13779c35b3f7a4 | test(04-10): expose persistent equipped corpse loss after death |
| dff9ec7c4f12d0726f0e64f1a97e69fa37b6dbca | feat(04-10): preserve current gear independently from death effects |
| ac7520691d17e404999fb0bef9364f994f8c6232 | feat(04-10): reuse guarded body and gear composition without controls |
| 67963770dd34b071181b9a11a713be1618f3430f | feat(04-10): compose persistent corpses loot and wildlife endpoints |
| f63d6dae00e79a49f98d3667a231c521a0f5b4b1 | docs(04-10): preserve confirmed task2 validation checkpoint |
| 98f433508c4b736793ba2f2e196018de6c0449c7 | docs(04-10): bound strict atlas clip and marker validation repair |
| b594303078ec21cdcb32ca0b37124e8e73a28785 | feat(04-10): compose validated player wildlife and equipped crowds |
| d2392ff33e682d774e5999728496919f1e9dc62d | test(04-10): prove native crowd resources and malformed definitions |


## Fresh repaired verification and immutable source mappings

Focused crowd d95a6816-0403-47ea-bf00-4c763683ce8f passes strict production/emitted harness0warnings/0errors, actualPID23012/cleanexit0/empty stderr,80 constructor controls and all original counts. Focused corpse f56074ff-be2a-41f6-942c-55d29ec43105 passes actualPID24484/exit0/empty stderr,13compiled/0skip and2189checks/16endpoints/original704.

Fresh entire selected set:20PASS/0FAIL on414 unchanged raw source identities, drift[], 1095069ms. Selection and explicit omissions are frozen at.phase4/04-10-r1-final/selection.json; results.json records each actual invocation HEAD, closure file and hashes rather than relabeling a later commit as its DLL. All newly committed Task2 bytes match these tested inputs. Invocation starts use98f433508c4b736793ba2f2e196018de6c0449c7; exact per-row revisions below govern any shared-checkout ancestry.

| Case | Invocation | Actual HEAD | ms | Result |
| --- | --- | --- | --- | --- |
| appearance.corpses | da36a6a6-c540-4ff9-a10c-7dae0010676d | 98f433508c4b736793ba2f2e196018de6c0449c7 | 25598 | pass |
| appearance.crowds | 26664167-adc1-480b-b82e-8163f654c40c | 98f433508c4b736793ba2f2e196018de6c0449c7 | 31047 | pass |
| animation.work-special | d3fab9b6-09b5-44b0-8d1b-1c00f906463b | 98f433508c4b736793ba2f2e196018de6c0449c7 | 26339 | pass |
| animation.bow | 3c5cd504-9663-44c1-9ee3-7b42109ad69f | 98f433508c4b736793ba2f2e196018de6c0449c7 | 45029 | pass |
| animation.replacement | a7aa967b-ad37-4c0c-be4e-4b045366cef8 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 24279 | pass |
| animation.melee | 5ee954bc-9fa6-4990-bd3d-74244c57b62d | 98f433508c4b736793ba2f2e196018de6c0449c7 | 24021 | pass |
| bodies.a-art | 2d118699-a1fd-47d1-88ea-ae4d37311eb0 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 8022 | pass |
| bodies.a-fit | be5abc34-8949-4fe7-9b68-94e6833c9555 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 26722 | pass |
| bodies.b-wide | f5da9270-f1b5-46a4-a13d-0a1bf172f5a7 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 45903 | pass |
| bodies.complete-fit | 9c0b49d5-c07e-4309-ac79-5da6ffa49aeb | 98f433508c4b736793ba2f2e196018de6c0449c7 | 49034 | pass |
| source.case-discovery | 5a8d4e6a-e251-46b8-ad14-149e527b4fed | 98f433508c4b736793ba2f2e196018de6c0449c7 | 727 | pass |
| data.foundation | baedd8ea-c708-4db6-8b22-73454191050a | 98f433508c4b736793ba2f2e196018de6c0449c7 | 505 | pass |
| data.items-combat | 4e318a6b-a154-48f6-aff0-a0c358061849 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 7350 | pass |
| hub.source | 5cba4f1e-0f53-447a-9749-b951cddb2ed7 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 360676 | pass |
| hub.lifecycle | 7c0725e8-a0be-4785-bfde-7eaafa2a5ba6 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 356884 | pass |
| items.weapons-food | 36f10b22-8c3a-4718-a159-0863e690821f | 98f433508c4b736793ba2f2e196018de6c0449c7 | 7890 | pass |
| items.wearables | 66ef3594-d718-4183-875a-f8d59427137d | 98f433508c4b736793ba2f2e196018de6c0449c7 | 13154 | pass |
| layouts.theme | 4b7bed1f-bb0b-427d-9837-9fb85ac08460 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 14758 | pass |
| layouts.identity | ec9c24a6-4869-41f9-b375-b1163ae10962 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 12751 | pass |
| layouts.identity-integrity | 6ddae4aa-dd3f-4de9-b507-4f2033b0c8a5 | 98f433508c4b736793ba2f2e196018de6c0449c7 | 14380 | pass |


Final crowd actualPID34800/nonce26664167-adc1-480b-b82e-8163f654c40c, same owning Godot Core/game ALC=true/true; actual exit0. Game SHA256d46d05edf18e8e6de2dae5acd32620548b0ff388ef583cd9f2e8cf034f86d391, Core SHA2569e5c252d05a1434c93fbfce953551db42acc1d1f4907546c06490d864179a388. It retains2066 original crowd IDs/order plus80 constructor outcomes,600 actual rows,166 wildlife samples,2189 corpse checks and704 original animation samples. Final corpse actualPID16924/nonceda36a6a6-c540-4ff9-a10c-7dae0010676d, gamed46d05edf18e8e6de2dae5acd32620548b0ff388ef583cd9f2e8cf034f86d391/Core9e5c252d05a1434c93fbfce953551db42acc1d1f4907546c06490d864179a388,13/13compiled/0skip. Official nonconsole engine authentication, explicit Core-before-game loading, actual PID/nonce/hash/ALC and nonzero/fatal-stderr rejection remain canonical; all actual native exit logs are retained per invocation.

Inherited proofs remain: animation35compiled/704samples/four jumps/five exact negatives/restored positive/41replacement controls/sevenUIstates+overflow; A-fit2976/5972native/10compiled/12H13; all-eight7936/16158native/12H13; hub63compiled each and25/50native; bow39compiled/448ordinary/112guard/14boundary/12binding/23extra/128sustained/3authority; work240rows/40controls/71Reset/16actions/17components/56members. Owned native Image/GetImage copies are scoped; borrowed textures/neighbors remain immutable. Array<Node> is not disposed; actual resource metadata identifies sanitized dotted IDs. PNG raw identity is separate from imported exact alpha/visibleRGB; only alpha0 RGB may differ.

Omissions: Unchanged exporter/audio/media/publication/building and foundation gameplay behaviors are outside this presentation slice: no sources/resources/configuration changed there; no acceptance or pass credit. Complete active Critical rendered selection remains final phase scope. Actual game windows, physical input, listening and named readability/appearance judgments remain later32/33-owned. Resource counts supply no FPS, mixed-OS physicalLAN or endurance certification.

## Constructor controls

The earlier diagnostic PID25220/exit0/empty stderr proved five malformed, correctly parent-bound definitions were accepted: out-of-atlas rectangle, duplicate part ID, zero stop, missing frame part and unsorted times. Its separate wrong-parent-hash case correctly rejected CROWD_BINDING_HASH. Those outcomes remain unchanged in.phase4/04-10-validation-diagnostic and the checkpoint manifest; the old canonical20/20/414/1119934ms never accepted modified R1 bytes.

The final80 rows include77 rejections and3 valid baseline/boundary/restored accepts. Each semantic mutation changes only owned temporary identity/fixture bytes and recomputes its temporary parent hash, while the independent wrong-parent-hash control stays unmatched. Every invalid owner remains detached with no unexpected escape-path read, native error or partial attachment. Exact outcomes follow.

| Control | Parent hash matches | Accepted | Detached/path-safe | Actual diagnostic |
| --- | --- | --- | --- | --- |
| valid-baseline | true | true | true/true | valid |
| outside-rect | true | false | true/true | CROWD_PART_RECT |
| duplicate-part | true | false | true/true | CROWD_PART_DUPLICATE |
| zero-stop | true | false | true/true | CROWD_CLIP_STOP |
| missing-frame-part | true | false | true/true | CROWD_FRAME_PART |
| unsorted-times | true | false | true/true | CROWD_FRAME_ORDER |
| wrong-parent-hash | false | false | true/true | CROWD_BINDING_HASH content/packs/player-identities.json |
| dimension-mismatch-width | true | false | true/true | CROWD_ATLAS_DIMENSIONS |
| dimension-fraction-width | true | false | true/true | CROWD_ATLAS_DIMENSIONS |
| dimension-missing-width | true | false | true/true | CROWD_IDENTITY_FIELD width |
| dimension-mismatch-height | true | false | true/true | CROWD_ATLAS_DIMENSIONS |
| dimension-fraction-height | true | false | true/true | CROWD_ATLAS_DIMENSIONS |
| dimension-missing-height | true | false | true/true | CROWD_IDENTITY_FIELD height |
| canonical-path-../escape.png | true | false | true/true | CROWD_ATLAS_PATH |
| canonical-path-assets/characters/../escape.png | true | false | true/true | CROWD_ATLAS_PATH |
| canonical-path-assets/characters/identity-wildlife.png/escape | true | false | true/true | CROWD_ATLAS_PATH |
| canonical-path-/escape.png | true | false | true/true | CROWD_ATLAS_PATH |
| canonical-path-assets\escape.png | true | false | true/true | CROWD_ATLAS_PATH |
| canonical-path-assets/characters/IDENTITY-wildlife.png | true | false | true/true | CROWD_ATLAS_PATH |
| bad-atlas-hash | true | false | true/true | CROWD_ATLAS_HASH |
| invalid-rect-[0,0,0,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[-1,0,1,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,0,1,-1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,108,1,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[2147483647,0,2147483647,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,0,0.5,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,0,"1",1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,0,1] | true | false | true/true | CROWD_PART_RECT |
| invalid-rect-[0,0,1,1,1] | true | false | true/true | CROWD_PART_RECT |
| missing-rect | true | false | true/true | CROWD_IDENTITY_FIELD atlasRect |
| empty-parts | true | false | true/true | CROWD_IDENTITY_COUNT parts |
| bounded-part-count | true | false | true/true | CROWD_IDENTITY_COUNT parts |
| empty-part-id | true | false | true/true | CROWD_IDENTITY_TEXT id |
| long-part-id | true | false | true/true | CROWD_IDENTITY_TEXT id |
| invalid-offset-[0] | true | false | true/true | CROWD_PART_OFFSET |
| invalid-offset-[0,0,0] | true | false | true/true | CROWD_PART_OFFSET |
| invalid-offset-[513,0] | true | false | true/true | CROWD_PART_OFFSET |
| invalid-offset-[0,-513] | true | false | true/true | CROWD_PART_OFFSET |
| invalid-offset-["NaN",0] | true | false | true/true | CROWD_PART_OFFSET |
| invalid-offset-[1E+100,0] | true | false | true/true | CROWD_PART_OFFSET |
| missing-offset | true | false | true/true | CROWD_IDENTITY_FIELD offsetTopLeftExact |
| duplicate-clip | true | false | true/true | CROWD_CLIP_DUPLICATE |
| long-clip-name | true | false | true/true | CROWD_IDENTITY_TEXT name |
| invalid-stop--1 | true | false | true/true | CROWD_CLIP_STOP |
| invalid-stop-60.01 | true | false | true/true | CROWD_CLIP_STOP |
| invalid-stop-1E+100 | true | false | true/true | CROWD_CLIP_STOP |
| nonboolean-loop | true | false | true/true | CROWD_CLIP_LOOP |
| missing-loop | true | false | true/true | CROWD_CLIP_LOOP |
| empty-frames | true | false | true/true | CROWD_IDENTITY_COUNT frames |
| bounded-frame-count | true | false | true/true | CROWD_IDENTITY_COUNT frames |
| invalid-time--1 | true | false | true/true | CROWD_FRAME_TIME |
| invalid-time-2 | true | false | true/true | CROWD_FRAME_TIME |
| invalid-time-1E+100 | true | false | true/true | CROWD_FRAME_TIME |
| missing-first-zero | true | false | true/true | CROWD_FRAME_ORDER |
| equal-times | true | false | true/true | CROWD_FRAME_ORDER |
| unknown-action-reference-rabbitidle | true | false | true/true | CROWD_WILDLIFE_REFERENCE rabbit/idle |
| unknown-action-reference-rabbitdeath | true | false | true/true | CROWD_WILDLIFE_REFERENCE rabbit/death |
| unknown-action-reference-rabbitcorpse | true | false | true/true | CROWD_WILDLIFE_REFERENCE rabbit/corpse |
| missing-action-rabbit | true | false | true/true | CROWD_IDENTITY_FIELD idle |
| unknown-action-reference-deeridle | true | false | true/true | CROWD_WILDLIFE_REFERENCE deer/idle |
| unknown-action-reference-deerdeath | true | false | true/true | CROWD_WILDLIFE_REFERENCE deer/death |
| unknown-action-reference-deercorpse | true | false | true/true | CROWD_WILDLIFE_REFERENCE deer/corpse |
| missing-action-deer | true | false | true/true | CROWD_IDENTITY_FIELD idle |
| missing-polygon-square | true | false | true/true | CROWD_IDENTITY_FIELD square |
| missing-polygon-triangle | true | false | true/true | CROWD_IDENTITY_FIELD triangle |
| missing-polygon-diamond | true | false | true/true | CROWD_IDENTITY_FIELD diamond |
| missing-polygon-cross | true | false | true/true | CROWD_IDENTITY_FIELD cross |
| polygon-too-few | true | false | true/true | CROWD_IDENTITY_COUNT square |
| polygon-too-many | true | false | true/true | CROWD_IDENTITY_COUNT square |
| polygon-point-[0] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-point-[0,0,0] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-point-[513,0] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-point-[0,-513] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-point-["NaN",0] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-point-[1E+100,0] | true | false | true/true | CROWD_POLYGON_POINT square |
| polygon-zero-area | true | false | true/true | CROWD_POLYGON_AREA square |
| polygon-crossing | true | false | true/true | CROWD_POLYGON_SIMPLE square |
| polygon-duplicate | true | false | true/true | CROWD_POLYGON_SIMPLE square |
| valid-boundaries-fractional-registration | true | true | true/true | valid |
| valid-restored | true | true | true/true | valid |


## Deviations from Plan and preserved attempt ledger

Prospective04-10-AMENDMENTS adds only two existing exact Core visibility authorities and the existing renderer composition surface: the registry/independent expected set becomes19 with all18 original identities, compiler-valid positive/wrong-path/adjacent-type/expanded-registry controls retained. Plan union12, phase union236. Reuse exact existing K12/K13/K21 source/archive and canonical28 identity palette; no new original art/master/source bundle, donor/common media, Root, public Godot type, scene, global parser, output capacity, policy permission/cohort or package install is introduced.

Task1 original correction budget3/3 remains exhausted. Setup CA1822/SCR0008 builds were not RED. Initial GREEN had sixCore diagnostics; correction1 left nested indentation, correction2 passed strict then fixture environment omission caused12/13 failures; unchanged established PHASE1_ROOT passed13/13. First harness MA0006 comparison failure was corrected ordinally by correction3 with exact-binding/thread/close proofs. Task1 commits/tracer then passed.

Task2 original correction budget3/3 remains exhausted. Initial dictionary/nested-indent diagnostics; correction1 left two122-character lines; correction2 passed strict then pile logger wrongly assumed part metadata and caused Godot errors/OUTPUT_LIMIT in8ad4100b/PID5708/exit1. Correction3 logs real AtlasTexture source regions and independently verifies all500 canonical pile-icon pixels;17bc954d/PID33136 passes2066crowd/166wildlife/2189corpse/original704. Older selection spans6796377/4127fd5/ca4b82b; exact per-invocation DLL mapping remains.phase4/checkpoints/04-10-task2-validation/invocation-heads-and-assemblies.json. Foreign commits are not reassigned to Phase4.

[Rule2 - Missing critical validation] R1 was prospectively bounded in04-10-REPAIR.md to existing CrowdPreview validation/private helpers and corpses.cjs saved proof only, maximum2 corrective attempts, without resetting either original3/3 budget. All12 live raw/Git-filtered source identities, five raw/Git-filtered backups and six ancestors verified before editing; the other3 Task2 data/PNG bytes remain exact. Initial R1 strict build found five new-helper analyzer diagnostics (two in-body comments, two argument indents and null-forgiving use); one grouped correction resolves them. Focused and full final selection pass; corrective allowance2 remains unused. A sandbox-only initial build failed0diagnostics and NodeGit spawn EPERM was avoided with equivalent authorized native tools; neither supplied behavioral evidence or changed tool pins. Historical outputs/checkpoint/backups remain intact.

No skipped test, unrun selected verify or goal-blocking stub remains in this slice. Empty state fields/collections are deliberate reset/current-state values and do not stand in for required resources. Existing WINDOWS29 stale index/tracer,30 native policy and31 producer integration remain open; no acceptance debt is silently erased.

## Integration observation and pending acceptance

After source stabilized and the fresh20-case selection passed, exactly one current engine.source-policy capture ran at.phase1/runs/9aef8ff9-40ad-4242-b4c8-c9a52199b650. It reportsFAIL with873 actual ungranted operations: src/godot-game/inspection/BuildingPreview.cs=166, src/godot-game/inspection/CharacterPreview.cs=372, src/godot-game/inspection/CorpsePreview.cs=101, src/godot-game/inspection/CrowdPreview.cs=156, src/godot-game/inspection/IdentityKit.cs=78. This supplies observed tuples for04-23, not a permission or compliance pass. Current evaluated producer has53 actual inputs/32 emitted surfaces with authenticated package SHA256ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e; full current source bytes/hashes, generated raw/text hashes and actual production closure are in.phase4/04-10-r1-final-generated-input-observation.json. No cohort enrollment/replay/permission occurs. Historical604/49inputs/32surfaces and all prior failures remain immutable.

Whole production and independent code review/fixes precede04-32 actual Windows1280x720/noVM input/listening/named judgments and04-33 reconciliation. AllASSET remain pending; no actual appearance/input/listening/export/performance/LAN/endurance acceptance is claimed. Parent owns global progress and same draftPR5/dev. Foreign Phase6/Phase12/spec/global work and the removed06-DISCUSS-CHECKPOINT.json are preserved; this executor never recreates it or switches/merges/pushes branches.

## Artifacts this phase produces

### Exact owned file identities

| File | Bytes | Raw SHA256 |
| --- | --- | --- |
| src/core/AppearanceState.cs | 10481 | 0a2990acb35075a31cd7c076444e784d7803d999822e2af1e6425fdaed6b8560 |
| src/godot-game/inspection/CorpsePreview.cs | 18774 | f3739163a2920a65597e719d1021a7ea2c72344a5b415040de076c15ef3624a2 |
| src/godot-game/content/packs/corpses-wildlife.json | 31487 | 67ee6b280269e3d68d72906b9b3be2d621bf4d76cd01fb90b0c9712ecf012f50 |
| tests/phase1/AppearanceStateTests.cs | 12948 | 49ad627e656638f1df8afa4927967d2e5b7b8e671ba4196e27c58a9588bd4efb |
| tools/phase4/cases/corpses.cjs | 49265 | f919c1aa92ed4035613e5bb8fe1f989fa31eafa7a25a1e0b7b60d687bb503d26 |
| src/godot-game/inspection/CrowdPreview.cs | 35942 | 595f601e5bff1c6495e9e89fd3042f0589e47fa107cfeb5187e7b892874c940f |
| src/godot-game/content/fixtures/crowds.json | 207148 | 41bbf06532422683d9784488d39dcb811d66284c19747930b7c7a3b50e8dd840 |
| src/godot-game/content/packs/player-identities.json | 88969 | c482e9a52b796b416b11b7bb73fa49aa6926c3b85bcb15cb47ed8cbccc4f93cf |
| src/godot-game/assets/characters/identity-wildlife.png | 10555 | 9859467281f48e9ea316288d7cad6c691b958e3943ed4129e570556fbc583c69 |
| docs/code/visibility-exemptions.json | 5520 | cacc2e5da3f9de96c584c5f3d9a6418e608d3afc84b6d549461d2c9f068d6084 |
| tests/phase1/ArchitecturePolicy.cs | 16651 | 7736550c57cc626330336272f433540edba0b042376b93869aacd753b6ebe50e |
| src/godot-game/inspection/CharacterPreview.cs | 88843 | 7b20d679ecb39616ed6af668209f460d90df4b96d0854409ae918b565e485712 |


### Actual C# declarations and signatures

#### src/core/AppearanceState.cs

```csharp
public AppearanceState(byte[] definition)
public JsonElement Snapshot
public void Restore(string body, string kind, JsonElement equipment, bool selected)
public void TriggerDeath()
public void Advance(double elapsedSeconds)
public void Reset()
private static readonly string[] _bodies = ["CH-01", "CH-02", "CH-03", "CH-04", "CH-05", "CH-06", "CH-07", "CH-08"];
private static readonly string[] _wildlife = ["rabbit", "deer"];
private readonly JsonElement _definition;
private Dictionary<string, string> _equipment = new(comparer: StringComparer.Ordinal);
private string _body = "";
private string _kind = "empty";
private bool _selected;
private double _deathElapsed = -1;
private double _collapseSeconds;
private double _effectSeconds;
private long _events;
private long _generation;
private void ValidateDefinition()
private bool ValidItem(string slot, string item)
private static bool CanonicalItem(string slot, string item)
private static void ValidateValue(JsonElement value)
```

#### src/godot-game/inspection/CorpsePreview.cs

```csharp
internal CorpsePreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, JsonElement definition, JsonElement common, JsonElement weapons, JsonElement clothing)
internal JsonElement Snapshot
internal static CorpsePreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read)
internal void Restore(string body, string kind, JsonElement equipment, bool selected, int facing = 1)
internal void TriggerDeath()
internal void Advance(double elapsedSeconds)
internal void Reset()
internal void Close()
private static readonly string[] _requiredSources = ["content/packs/common.json", "assets/common/donor-kit.png", "content/animation/fits-a.json", "content/animation/fits-b.json", "content/animation/fits-c.json", "content/packs/bodies-a.json", "content/packs/bodies-b.json", "content/packs/bodies-c.json", "assets/characters/bodies-a.png", "assets/characters/bodies-b.png", "assets/characters/bodies-c.png", "content/packs/items-01-10.json", "content/packs/items-11-24.json", "assets/equipment/items-01-10.png", "assets/equipment/items-11-24.png"];
private readonly Node2D _root;
private readonly MainThreadGateway _gateway;
private readonly Func<string, byte[]?> _read;
private readonly JsonElement _definition;
private readonly JsonElement _common;
private readonly JsonElement[] _packs;
private readonly AppearanceState _state;
private readonly List<Texture2D> _owned = [];
private CharacterPreview? _character;
private bool _closed;
private int _facing = 1;
private static string MissingText()
private static JsonElement ReadPack(Func<string, byte[]?> read, string id)
private static void ValidateCommon(JsonElement value)
private void RequireActive()
private void Retire()
private void Project()
private void Wildlife(Node2D visual, string body, JsonElement sample)
private void Pile(Node2D visual, JsonElement equipment, bool meatOnly)
private Sprite2D CommonSprite(Node2D parent, string id, string name, Vector2 at)
private Sprite2D Sprite(Node2D parent, JsonElement pack, JsonElement part, string name, Vector2 at)
private void Outline(Node2D visual)
```

#### tests/phase1/AppearanceStateTests.cs

```csharp
public void Death_AfterTransientExpiryPreservesCurrentEquippedCorpse()
public void Restore_ReplacesGearSilentlyAndDetachesInputs()
public void Death_AllBodiesPreserveGearAndRemoveCrown(string body)
public void Endpoint_RulesAndResetFencePastEvents()
public void InvalidInput_PreservesCurrentPublication()
public void Visibility_ExactIdentityAndCompilerValidNegativeControls()
private const string Definition = "{\"schemaVersion\":1,\"id\":\"corpses-wildlife\",\"version\":1,"
private static AppearanceState Actual()
private static JsonElement Gear(string? backpack = null)
private static void Compile(string source)
```

#### src/godot-game/inspection/CrowdPreview.cs

```csharp
internal CrowdPreview(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read)
internal JsonElement Snapshot
internal void Attach()
internal void Select(string id)
internal void RestoreRemaining(string id, JsonElement equipment)
internal void TransferControl(string from, string to)
internal void SetNight(bool night)
internal void Wildlife(string body, string action, double elapsedSeconds, int facing, bool selected)
internal void Close()
private readonly Node2D _root;
private readonly MainThreadGateway _gateway;
private readonly Func<string, byte[]?> _read;
private readonly Dictionary<string, byte[]> _bytes = new(comparer: StringComparer.Ordinal);
private readonly JsonElement _fixture;
private readonly JsonElement _identities;
private readonly JsonElement _definition;
private readonly JsonElement _common;
private readonly JsonElement _weapons;
private readonly JsonElement _clothing;
private readonly Dictionary<string, CorpsePreview> _subjects = new(comparer: StringComparer.Ordinal);
private readonly Dictionary<string, JsonElement> _current = new(comparer: StringComparer.Ordinal);
private readonly Dictionary<string, AtlasTexture> _textures = new(comparer: StringComparer.Ordinal);
private bool _attached;
private bool _closed;
private bool _night;
private string _selected = "";
private string _controlled = "survivor-000";
private static string Text(JsonElement row, string name)
private byte[] Cached(Func<string, byte[]?> read, string file)
private JsonElement Definition(string path, string id)
private void Verify(JsonElement bindings)
private void Validate()
private void ValidateIdentityDefinitions()
private static JsonElement Field(JsonElement row, string name, JsonValueKind kind)
private static string IdentityText(JsonElement row, string name)
private static JsonElement Rows(JsonElement row, string name, int minimum)
private static double Number(JsonElement value, double minimum, double maximum, string diagnostic)
private HashSet<string> ValidateParts(uint width, uint height)
private HashSet<string> ValidateClips(HashSet<string> parts)
private void ValidateWildlife(HashSet<string> parts, HashSet<string> clips)
private void ValidatePolygons(double bound)
private static void ValidateGeometry(List<double[]> points, string shape)
private static double Cross(double[] a, double[] b, double[] c)
private static bool OnSegment(double[] a, double[] b, double[] point)
private static bool Intersects(double[] a, double[] b, double[] c, double[] d)
private void RequireActive()
private void Publish()
private void ProjectActor(string id, bool selected)
private void Identity(string id)
private JsonElement Part(string id)
private AtlasTexture Texture(string id)
private void RetireWildlife(Node2D owner)
private void WildlifeOutline(Node2D owner, Sprite2D sprite)
```

#### tests/phase1/ArchitecturePolicy.cs

```csharp
internal static IReadOnlyList<string> Visibility(string root, string source, string path, CSharpParseOptions options)
```

#### src/godot-game/inspection/CharacterPreview.cs

```csharp
internal static CharacterPreview OpenComposition(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string body)
internal void RestoreComposition(string body, string pose, int facing, JsonElement equipment)
internal void CloseComposition()
private bool _composition;
private void AddLayer(Node2D art, JsonElement body, JsonElement pose, JsonElement item, JsonElement layer, string slot, int facing)
```

corpses.cjs exports files/fingerprint/METHODS/harness/native/run/SCENARIOS and CROWD_METHODS/crowdHarness/crowdRun; registers exact appearance.corpses (G0 order54,Critical) and appearance.crowds (G0 order55,High). Saved scenarios04-10.equipped-endpoints,piles,H17,boundary,scale-and-overlap,four-identities-transfer,wildlife-cycles,day-night-readability retain setup/actions/expectations/evidence/reset and laterW04-appearance ordered actual-window checkpoints. New private emitted DefinitionControl/DefinitionControls and definitionRows authenticate malformed constructor outcomes without weakening original counts.

### Resource and authored clip identities

corpses-wildlife.json id/version1 declares all8 bodies,rabbit/deer,actual slot applicability,collapse/effect timing,poses/endpoints/wildlifeClips,exact sourceBindings,selection and pending acceptance. Exact endpoint/clip bindings:

```json
{
  "collapseSeconds": 0.32,
  "effectSeconds": 0.8,
  "poses": [
    "collapse-a",
    "collapse-b",
    "corpse"
  ],
  "endpoints": {
    "rabbit": "common.r.9577",
    "deer": "common.r.10791"
  },
  "wildlifeClips": [
    {
      "id": "common.clip.2193",
      "name": "bunny_dead",
      "stop": 1,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.9577"
            }
          ],
          "endpoint": "common.r.9577"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2197",
      "name": "bunny_stand",
      "stop": 1,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.9243"
            }
          ],
          "endpoint": "common.r.9243"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2201",
      "name": "bunny_walk",
      "stop": 0.625,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.10160"
            },
            {
              "time": 0.0625,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.0625,
              "sprite": "common.r.11517"
            },
            {
              "time": 0.125,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.125,
              "sprite": "common.r.11763"
            },
            {
              "time": 0.1875,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.1875,
              "sprite": "common.r.12003"
            },
            {
              "time": 0.25,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 0.25,
              "sprite": "common.r.12232"
            },
            {
              "time": 0.3125,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 0.3125,
              "sprite": "common.r.12450"
            },
            {
              "time": 0.375,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 0.375,
              "sprite": "common.r.12642"
            },
            {
              "time": 0.4375,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 0.4375,
              "sprite": "common.r.12812"
            },
            {
              "time": 0.5,
              "value": 8,
              "coefficients": [
                0,
                0,
                0,
                8
              ],
              "sourceTime": 0.5,
              "sprite": "common.r.12961"
            },
            {
              "time": 0.5625,
              "value": 9,
              "coefficients": [
                0,
                0,
                0,
                9
              ],
              "sourceTime": 0.5625,
              "sprite": "common.r.13102"
            }
          ],
          "endpoint": "common.r.13102"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2278",
      "name": "deer_die",
      "stop": 0.5,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.7981"
            },
            {
              "time": 0.10000000149011612,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.10000000149011612,
              "sprite": "common.r.9425"
            },
            {
              "time": 0.20000000298023224,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.20000000298023224,
              "sprite": "common.r.8319"
            },
            {
              "time": 0.25,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.25,
              "sprite": "common.r.9751"
            },
            {
              "time": 0.30000001192092896,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 0.30000001192092896,
              "sprite": "common.r.8464"
            },
            {
              "time": 0.3499999940395355,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 0.3499999940395355,
              "sprite": "common.r.7914"
            },
            {
              "time": 0.4000000059604645,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 0.4000000059604645,
              "sprite": "common.r.10791"
            },
            {
              "time": 0.44999998807907104,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 0.44999998807907104,
              "sprite": "common.r.8301"
            }
          ],
          "endpoint": "common.r.8301"
        }
      ],
      "loop": false
    },
    {
      "id": "common.clip.2282",
      "name": "deer_eat",
      "stop": 6.3333330154418945,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.11237"
            },
            {
              "time": 0.1666666716337204,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.1666666716337204,
              "sprite": "common.r.9726"
            },
            {
              "time": 0.3333333432674408,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.3333333432674408,
              "sprite": "common.r.9617"
            },
            {
              "time": 0.5,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.5,
              "sprite": "common.r.9191"
            },
            {
              "time": 0.8333333134651184,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 0.8333333134651184,
              "sprite": "common.r.9140"
            },
            {
              "time": 1.1666666269302368,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 1.1666666269302368,
              "sprite": "common.r.9191"
            },
            {
              "time": 1.5,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 1.5,
              "sprite": "common.r.9140"
            },
            {
              "time": 1.8333333730697632,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 1.8333333730697632,
              "sprite": "common.r.9191"
            },
            {
              "time": 2.1666667461395264,
              "value": 8,
              "coefficients": [
                0,
                0,
                0,
                8
              ],
              "sourceTime": 2.1666667461395264,
              "sprite": "common.r.9140"
            },
            {
              "time": 2.5,
              "value": 9,
              "coefficients": [
                0,
                0,
                0,
                9
              ],
              "sourceTime": 2.5,
              "sprite": "common.r.9191"
            },
            {
              "time": 2.8333332538604736,
              "value": 10,
              "coefficients": [
                0,
                0,
                0,
                10
              ],
              "sourceTime": 2.8333332538604736,
              "sprite": "common.r.9140"
            },
            {
              "time": 3.1666667461395264,
              "value": 11,
              "coefficients": [
                0,
                0,
                0,
                11
              ],
              "sourceTime": 3.1666667461395264,
              "sprite": "common.r.9191"
            },
            {
              "time": 3.5,
              "value": 12,
              "coefficients": [
                0,
                0,
                0,
                12
              ],
              "sourceTime": 3.5,
              "sprite": "common.r.9140"
            },
            {
              "time": 3.8333332538604736,
              "value": 13,
              "coefficients": [
                0,
                0,
                0,
                13
              ],
              "sourceTime": 3.8333332538604736,
              "sprite": "common.r.9191"
            },
            {
              "time": 4.166666507720947,
              "value": 14,
              "coefficients": [
                0,
                0,
                0,
                14
              ],
              "sourceTime": 4.166666507720947,
              "sprite": "common.r.9140"
            },
            {
              "time": 4.5,
              "value": 15,
              "coefficients": [
                0,
                0,
                0,
                15
              ],
              "sourceTime": 4.5,
              "sprite": "common.r.9191"
            },
            {
              "time": 4.833333492279053,
              "value": 16,
              "coefficients": [
                0,
                0,
                0,
                16
              ],
              "sourceTime": 4.833333492279053,
              "sprite": "common.r.9140"
            },
            {
              "time": 5.166666507720947,
              "value": 17,
              "coefficients": [
                0,
                0,
                0,
                17
              ],
              "sourceTime": 5.166666507720947,
              "sprite": "common.r.9191"
            },
            {
              "time": 5.5,
              "value": 18,
              "coefficients": [
                0,
                0,
                0,
                18
              ],
              "sourceTime": 5.5,
              "sprite": "common.r.9140"
            },
            {
              "time": 5.833333492279053,
              "value": 19,
              "coefficients": [
                0,
                0,
                0,
                19
              ],
              "sourceTime": 5.833333492279053,
              "sprite": "common.r.9191"
            },
            {
              "time": 6.166666507720947,
              "value": 20,
              "coefficients": [
                0,
                0,
                0,
                20
              ],
              "sourceTime": 6.166666507720947,
              "sprite": "common.r.9140"
            }
          ],
          "endpoint": "common.r.9140"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2286",
      "name": "deer_eat_short",
      "stop": 3.1666667461395264,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.9726"
            },
            {
              "time": 0.1666666716337204,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.1666666716337204,
              "sprite": "common.r.9617"
            },
            {
              "time": 0.3333333432674408,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.3333333432674408,
              "sprite": "common.r.9191"
            },
            {
              "time": 0.6666666865348816,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.6666666865348816,
              "sprite": "common.r.9140"
            },
            {
              "time": 1,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 1,
              "sprite": "common.r.9191"
            },
            {
              "time": 1.3333333730697632,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 1.3333333730697632,
              "sprite": "common.r.9140"
            },
            {
              "time": 1.6666666269302368,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 1.6666666269302368,
              "sprite": "common.r.9191"
            },
            {
              "time": 2,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 2,
              "sprite": "common.r.9140"
            },
            {
              "time": 2.3333332538604736,
              "value": 8,
              "coefficients": [
                0,
                0,
                0,
                8
              ],
              "sourceTime": 2.3333332538604736,
              "sprite": "common.r.9191"
            },
            {
              "time": 2.6666667461395264,
              "value": 9,
              "coefficients": [
                0,
                0,
                0,
                9
              ],
              "sourceTime": 2.6666667461395264,
              "sprite": "common.r.9140"
            },
            {
              "time": 3,
              "value": 10,
              "coefficients": [
                0,
                0,
                0,
                10
              ],
              "sourceTime": 3,
              "sprite": "common.r.9191"
            }
          ],
          "endpoint": "common.r.9191"
        }
      ],
      "loop": false
    },
    {
      "id": "common.clip.2290",
      "name": "deer_look",
      "stop": 4.25,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.10440"
            },
            {
              "time": 1.75,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 1.75,
              "sprite": "common.r.9143"
            },
            {
              "time": 2,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 2,
              "sprite": "common.r.8695"
            },
            {
              "time": 3.25,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 3.25,
              "sprite": "common.r.9143"
            },
            {
              "time": 4,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 4,
              "sprite": "common.r.10440"
            }
          ],
          "endpoint": "common.r.10440"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2294",
      "name": "deer_run",
      "stop": 0.6800000071525574,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.11040"
            },
            {
              "time": 0.07999999821186066,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.07999999821186066,
              "sprite": "common.r.7963"
            },
            {
              "time": 0.1599999964237213,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.1599999964237213,
              "sprite": "common.r.8337"
            },
            {
              "time": 0.23999999463558197,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.23999999463558197,
              "sprite": "common.r.10574"
            },
            {
              "time": 0.36000001430511475,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 0.36000001430511475,
              "sprite": "common.r.8228"
            },
            {
              "time": 0.4399999976158142,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 0.4399999976158142,
              "sprite": "common.r.8494"
            },
            {
              "time": 0.5600000023841858,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 0.5600000023841858,
              "sprite": "common.r.10769"
            },
            {
              "time": 0.6399999856948853,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 0.6399999856948853,
              "sprite": "common.r.8584"
            }
          ],
          "endpoint": "common.r.8584"
        }
      ],
      "loop": true
    },
    {
      "id": "common.clip.2298",
      "name": "deer_walk",
      "stop": 1.0714285373687744,
      "layers": [
        {
          "binding": {
            "path": 0,
            "attribute": 0,
            "script": {
              "m_FileID": 0,
              "m_PathID": 0
            },
            "typeID": 212,
            "customType": 23,
            "isPPtrCurve": 1,
            "isIntCurve": 0,
            "isSerializeReferenceCurve": 0
          },
          "component": 0,
          "curve": 0,
          "interpolation": "discrete",
          "keys": [
            {
              "time": 0,
              "value": 0,
              "coefficients": [
                0,
                0,
                0,
                0
              ],
              "sourceTime": "initial",
              "sprite": "common.r.8557"
            },
            {
              "time": 0.1428571492433548,
              "value": 1,
              "coefficients": [
                0,
                0,
                0,
                1
              ],
              "sourceTime": 0.1428571492433548,
              "sprite": "common.r.8799"
            },
            {
              "time": 0.2857142984867096,
              "value": 2,
              "coefficients": [
                0,
                0,
                0,
                2
              ],
              "sourceTime": 0.2857142984867096,
              "sprite": "common.r.9040"
            },
            {
              "time": 0.4285714328289032,
              "value": 3,
              "coefficients": [
                0,
                0,
                0,
                3
              ],
              "sourceTime": 0.4285714328289032,
              "sprite": "common.r.8395"
            },
            {
              "time": 0.5714285969734192,
              "value": 4,
              "coefficients": [
                0,
                0,
                0,
                4
              ],
              "sourceTime": 0.5714285969734192,
              "sprite": "common.r.11065"
            },
            {
              "time": 0.7142857313156128,
              "value": 5,
              "coefficients": [
                0,
                0,
                0,
                5
              ],
              "sourceTime": 0.7142857313156128,
              "sprite": "common.r.9884"
            },
            {
              "time": 0.8571428656578064,
              "value": 6,
              "coefficients": [
                0,
                0,
                0,
                6
              ],
              "sourceTime": 0.8571428656578064,
              "sprite": "common.r.11203"
            },
            {
              "time": 1,
              "value": 7,
              "coefficients": [
                0,
                0,
                0,
                7
              ],
              "sourceTime": 1,
              "sprite": "common.r.7943"
            }
          ],
          "endpoint": "common.r.7943"
        }
      ],
      "loop": true
    }
  ]
}
```

crowds.json id/version1 stores600 actual ordered source-bound rows and selected/controlled identities,day/night/overlap metadata. player-identities.json id/version1 stores canonical players/vectors,46 parts,9 authored clips,wildlife mapping,atlas/source archive hashes,retained members and replay recipe. Atlas512x108 SHA2569859467281f48e9ea316288d7cad6c691b958e3943ed4129e570556fbc583c69; raw PNG10555bytes. Source archiveproduction/phase4/common-selected.zip SHA2562f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9 is pre-existing/read-only; no new master/archive is created. Deterministic2pixel padding/512width packing, canonical deerColorA lerp0.35 red-channel multiplication/half-up rounding preserve source alpha;rabbit/K21 are unchanged. Independent Python replay equals atlas/parts/all102retained member hashes and all9clips equal original authored common selections without timing/order edits.

| Part ID | Atlas rectangle | Exact registration |
| --- | --- | --- |
| common.r.10160 | [2,2,16,16] | [-8,0] |
| common.r.10440 | [20,2,30,33] | [-13.973249435424805,15] |
| common.r.10574 | [52,2,33,33] | [-10.98708724975586,10.98708724975586] |
| common.r.10769 | [87,2,32,33] | [-13.966499328613281,15] |
| common.r.10791 | [121,2,31,12] | [-16.923879623413086,36] |
| common.r.11040 | [154,2,32,31] | [-13.973249435424805,17] |
| common.r.11065 | [188,2,35,31] | [-13.973249435424805,17] |
| common.r.11203 | [225,2,35,31] | [-13.973249435424805,17] |
| common.r.11237 | [262,2,34,31] | [-13.973249435424805,17] |
| common.r.11517 | [298,2,16,16] | [-8,0] |
| common.r.11763 | [316,2,16,16] | [-8,0] |
| common.r.12003 | [334,2,16,16] | [-8,0] |
| common.r.12232 | [352,2,16,16] | [-8,0] |
| common.r.12450 | [370,2,16,16] | [-8,0] |
| common.r.12642 | [388,2,16,16] | [-8,0] |
| common.r.12812 | [406,2,16,16] | [-8,0] |
| common.r.12961 | [424,2,16,16] | [-8,0] |
| common.r.13102 | [442,2,16,16] | [-8,0] |
| common.r.7914 | [460,2,30,18] | [-16.923879623413086,30] |
| common.r.7943 | [2,37,36,32] | [-14.934721946716309,16] |
| common.r.7963 | [40,37,33,33] | [-13.973249435424805,15] |
| common.r.7981 | [75,37,33,32] | [-15.973249435424805,16] |
| common.r.8228 | [110,37,33,33] | [-11.98708724975586,11.987087488174438] |
| common.r.8301 | [145,37,30,10] | [-16.923879623413086,38] |
| common.r.8319 | [177,37,31,28] | [-16.973248958587646,20] |
| common.r.8337 | [210,37,33,35] | [-11.98708724975586,11.987087488174438] |
| common.r.8395 | [245,37,36,32] | [-14.934721946716309,16] |
| common.r.8464 | [283,37,30,21] | [-16.923879623413086,27] |
| common.r.8494 | [315,37,33,32] | [-12.966499328613281,13.987087488174438] |
| common.r.8557 | [350,37,35,31] | [-13.973249435424805,17] |
| common.r.8584 | [387,37,32,33] | [-14.966499328613281,15] |
| common.r.8593 | [421,37,30,33] | [-13.973249435424805,15] |
| common.r.8695 | [453,37,31,33] | [-13.973249435424805,15] |
| common.r.8799 | [2,74,35,31] | [-13.963714599609375,17] |
| common.r.9040 | [39,74,35,31] | [-13.973249435424805,17] |
| common.r.9140 | [76,74,36,20] | [-12.973249435424805,28] |
| common.r.9143 | [114,74,33,32] | [-13.973249435424805,16] |
| common.r.9191 | [149,74,37,20] | [-12.973249435424805,28] |
| common.r.9243 | [188,74,16,16] | [-8,0] |
| common.r.9425 | [206,74,33,30] | [-16.973248958587646,18] |
| common.r.9577 | [241,74,16,16] | [-8,0] |
| common.r.9617 | [259,74,37,21] | [-12.973249435424805,27] |
| common.r.9726 | [298,74,36,30] | [-12.973249435424805,18] |
| common.r.9751 | [336,74,30,23] | [-16.973248958587646,25] |
| common.r.9884 | [368,74,35,31] | [-13.963714599609375,17] |
| common.s0.7985 | [405,74,6,3] | [-3,0] |


| Clip | Stop | Loop | Authored frames: time→part |
| --- | --- | --- | --- |
| bunny_dead | 1 | true | 0→common.r.9577 |
| bunny_stand | 1 | true | 0→common.r.9243 |
| bunny_walk | 0.625 | true | 0→common.r.10160; 0.0625→common.r.11517; 0.125→common.r.11763; 0.1875→common.r.12003; 0.25→common.r.12232; 0.3125→common.r.12450; 0.375→common.r.12642; 0.4375→common.r.12812; 0.5→common.r.12961; 0.5625→common.r.13102 |
| deer_die | 0.5 | false | 0→common.r.7981; 0.10000000149011612→common.r.9425; 0.20000000298023224→common.r.8319; 0.25→common.r.9751; 0.30000001192092896→common.r.8464; 0.3499999940395355→common.r.7914; 0.4000000059604645→common.r.10791; 0.44999998807907104→common.r.8301 |
| deer_eat | 6.3333330154418945 | true | 0→common.r.11237; 0.1666666716337204→common.r.9726; 0.3333333432674408→common.r.9617; 0.5→common.r.9191; 0.8333333134651184→common.r.9140; 1.1666666269302368→common.r.9191; 1.5→common.r.9140; 1.8333333730697632→common.r.9191; 2.1666667461395264→common.r.9140; 2.5→common.r.9191; 2.8333332538604736→common.r.9140; 3.1666667461395264→common.r.9191; 3.5→common.r.9140; 3.8333332538604736→common.r.9191; 4.166666507720947→common.r.9140; 4.5→common.r.9191; 4.833333492279053→common.r.9140; 5.166666507720947→common.r.9191; 5.5→common.r.9140; 5.833333492279053→common.r.9191; 6.166666507720947→common.r.9140 |
| deer_eat_short | 3.1666667461395264 | false | 0→common.r.9726; 0.1666666716337204→common.r.9617; 0.3333333432674408→common.r.9191; 0.6666666865348816→common.r.9140; 1→common.r.9191; 1.3333333730697632→common.r.9140; 1.6666666269302368→common.r.9191; 2→common.r.9140; 2.3333332538604736→common.r.9191; 2.6666667461395264→common.r.9140; 3→common.r.9191 |
| deer_look | 4.25 | true | 0→common.r.10440; 1.75→common.r.9143; 2→common.r.8695; 3.25→common.r.9143; 4→common.r.10440 |
| deer_run | 0.6800000071525574 | true | 0→common.r.11040; 0.07999999821186066→common.r.7963; 0.1599999964237213→common.r.8337; 0.23999999463558197→common.r.10574; 0.36000001430511475→common.r.8228; 0.4399999976158142→common.r.8494; 0.5600000023841858→common.r.10769; 0.6399999856948853→common.r.8584 |
| deer_walk | 1.0714285373687744 | true | 0→common.r.8557; 0.1428571492433548→common.r.8799; 0.2857142984867096→common.r.9040; 0.4285714328289032→common.r.8395; 0.5714285969734192→common.r.11065; 0.7142857313156128→common.r.9884; 0.8571428656578064→common.r.11203; 1→common.r.7943 |


Wildlife mapping: {"rabbit":{"idle":"bunny_stand","move":"bunny_walk","flee":"bunny_walk","hit":"bunny_stand","death":"bunny_dead","corpse":"common.r.9577"},"deer":{"idle":"deer_look","look":"deer_look","eat":"deer_eat","move":"deer_walk","flee":"deer_run","hit":"deer_look","death":"deer_die","corpse":"common.r.10791"}}. Exact canonical polygon vectors: {"square":[[-3,-3],[3,-3],[3,3],[-3,3]],"triangle":[[0,-4],[4,3],[-4,3]],"diamond":[[0,-4],[4,0],[0,4],[-4,0]],"cross":[[-1,-4],[1,-4],[1,-1],[4,-1],[4,1],[1,1],[1,4],[-1,4],[-1,1],[-4,1],[-4,-1],[-1,-1]]}.

### Exact retained source members

All102 names and SHA256 values refer to existing common-selected.zip; no source member is fabricated or newly archived.

| Retained member | SHA256 |
| --- | --- |
| images/resources.assets/10160.png | 150f9890f9c54506d2de9877293b9a6e2c24cfe962718e2a52b0286e6114ca8a |
| images/resources.assets/10440.png | 05e96a693af96a3b514c43dbd511a2dd707b9ffbfa024d9961aa98655ffa251e |
| images/resources.assets/10574.png | d7c8bae2ad5314cfdf537b963de89490b0ddcc45fae714f59e291fc18b2c67bc |
| images/resources.assets/10769.png | a995d5f036f934bd38fe560bd1633efc4704f76579e83824bee091091a433b67 |
| images/resources.assets/10791.png | c1330c411236842f959f5cc57775786e32ac2ad1507157bb068b39d27ded58ab |
| images/resources.assets/11040.png | 60d9ff63e0b2b79a5a3a69983c1339fc605c1c7de4dc1b2326fa77961fb5d8be |
| images/resources.assets/11065.png | 7bdbc59bce74314d9f5c427b24d4c8b3002443f4b15524e3185c7350c1f09687 |
| images/resources.assets/11203.png | 09f6976fa05a8f9648e1dc80b8bb8d9b9dda3c91e0f841e957f69d1c7e230d07 |
| images/resources.assets/11237.png | 7dc575e653e1fd5eb72b19acf04293d6c219b22640db3e5e530fea1173c338e5 |
| images/resources.assets/11517.png | e03ac38cbf930b123dc5ff628f389979b11375c3f8523546e9ac867cfa7fa7aa |
| images/resources.assets/11763.png | 12eb7057971d6f086de904b718abe8b57e023e7e11a2fe353b9759c415c116ca |
| images/resources.assets/12003.png | e998cee09082798bc428582145a20bb4b3c205326e0d6a23d31ec5ebbb73fb6f |
| images/resources.assets/12232.png | eaaf3ae16ce2c60ca18dcf62adfdcc85693a568f862fbfece17d54b21aaab40d |
| images/resources.assets/12450.png | ebe6ed205ee0be9202beb5d8a6d9a44714e463b2158997276b86b7168bdee1a2 |
| images/resources.assets/12642.png | 93a187ad350f58de4add248e0ab7a33de7a2a2a098d1aa2d1cef6285fd6c4fa4 |
| images/resources.assets/12812.png | 134d5fbcf015838a112494191e9feb19b6acf3e62147065b9f273043971b245e |
| images/resources.assets/12961.png | 431c3c8efbdc0dd23014f259ce85fa7ca359cf8b05a1f7192b4211b791ca39dd |
| images/resources.assets/13102.png | ba70f4032f6c3ada3091b6c8b7cb73593d27587d0020b08904bc9d353c0d5b42 |
| images/resources.assets/7914.png | 06c78c845007150f773b88803fc5df386eba8b1e6b6eb5ada8ad2d11ee5473f2 |
| images/resources.assets/7943.png | d4e2fa05fd4912f052ab9e017e21e4da1aa18752ebc3c4d7cd61d5b2d0cdef17 |
| images/resources.assets/7963.png | 8d1a41f37375bb3d8df04fe5a4b78a089f76faa8f5d190d6e3ad004dcf99a55c |
| images/resources.assets/7981.png | 9b538828884397e6622484dfebcad847720b9abc41738a7d135c02b5dd33422a |
| images/resources.assets/8228.png | b95d1984acb831b422ab1c0b0899a6d74b25e647ce287949c2f240acc13ba989 |
| images/resources.assets/8301.png | cd6d7e3454106ffc541239bea539845c63a7d12d20e170f52ff2924458aeade4 |
| images/resources.assets/8319.png | da58a0ed7e01467ce24c8e5d8ab436fa4d1fe05c0a02dd1d22b0c64a4a7aaafb |
| images/resources.assets/8337.png | 08ae08949cfc464922aa7fbefbf33b9296819f49deb4e84a029c0799811832d5 |
| images/resources.assets/8395.png | 40b27c8f01fd7c9ff82e63d023168a3d990e6fa75d9a4156be90eb0c759bbda9 |
| images/resources.assets/8464.png | 4d397a6af7d605c0ffb66da596371f1ef55376d98b2b624aab7f5596d3add4a8 |
| images/resources.assets/8494.png | 6b996a9cc61a71aad7ee8b48cd8f14702564be719c2a4532667e3348435d61f9 |
| images/resources.assets/8557.png | 3b0ba9d2b8ac04572b740dd0685274cb378aff088a0378d50d224e86755a51c8 |
| images/resources.assets/8584.png | 3810aaf27ecfde8cade4131d47c8939d5bbc097eebb90fb7feb4808760ca3058 |
| images/resources.assets/8593.png | 05e96a693af96a3b514c43dbd511a2dd707b9ffbfa024d9961aa98655ffa251e |
| images/resources.assets/8695.png | 04b036f2f527a6d378ec3413f135bc155ba7a37d57997d45f9cd0c7d7110da37 |
| images/resources.assets/8799.png | 23953520e3162b93fc375b1c3429dc10c0328cce936b0c80de65c6b28ccf0836 |
| images/resources.assets/9040.png | 3d8a0738c26be6413476e16e409576816c955386615d3943f5c2335b06a2a2b9 |
| images/resources.assets/9140.png | f2f601207b36d604223116ae10bf8595a5a7374ba455276ff12f464b2674bc6b |
| images/resources.assets/9143.png | 59d39bd0e03409d3386271e54424fcd7b1fb2b67ce4bd34f4665db42b9f39c5a |
| images/resources.assets/9191.png | 8129af9d570b35e5d5379ee31412ac902370f3000d4895aa3f7b92530687377f |
| images/resources.assets/9243.png | 580c4ebff6b1958b39fe9cc7a38152281b26e531c1c79910770a17f4575661de |
| images/resources.assets/9425.png | a35e0290ffae47c4773330e3c792cb6f8075834168d009f79b71754e825c1061 |
| images/resources.assets/9577.png | 52c7394a565722c784489a12f81937a1766b4f3238808d325222fb7405056127 |
| images/resources.assets/9617.png | 39acae5389673623c82ceb2950673f0f08e1257fa6d8c06f1457abb51459f742 |
| images/resources.assets/9726.png | ccbab83f6c59ed5418eabb4e8222b1b11e1987288c96c3caa60e4c03ddf917b7 |
| images/resources.assets/9751.png | bd958d8d83c7b1827e0468fdb9ff80bc9c469a23d86d4ccaee370d9c5d0b4f73 |
| images/resources.assets/9884.png | 6f2acb78234efef56355886320f814889136d1eeebbb96736cee565576a3e3b5 |
| images/sharedassets0.assets/7985.png | d4e15ce84bbdb2fbbb9224e66859e48e3cf5543a7d9c0d7ed218f64f55e4e474 |
| palettes/73492.json | d3609c5221308f5be5a2022e9475e08c401d1f7d0aebc40ed1fa5fc634aa2c50 |
| raw/resources.assets/10160.bin | ccfb3af8ada256d237689d5ee9b7bf0666ebce9032e72c47015a3d4d152839bd |
| raw/resources.assets/10440.bin | c82c2841873351ecd4877c5cd9c52ba87e12612ccf2ba98f5b72f480c6f68070 |
| raw/resources.assets/10574.bin | 55fb247f533267e9972880e82a6fdca431cf262b4fd671f1484068e4d5e7a526 |
| raw/resources.assets/10769.bin | abdea560de4ce3eef4fb1346f1bd08c1f2b885619c399b3abe274aa482eff7ea |
| raw/resources.assets/10791.bin | cc7478a729f9538dd9a55f9f01247cc117a401aab97f54a97dec9db530a232b5 |
| raw/resources.assets/11040.bin | bebfc3e406903ed7ee3c028142c2ae0596bb37a0328fff90d3313b56d1a26655 |
| raw/resources.assets/11065.bin | 0aee493c085495520466766260995d3dc3fa46f50d70920d4ef9efba2ff7c187 |
| raw/resources.assets/11203.bin | 607144fdc8ed02b9b8d98602d50ee761fcc72b4b07622157705fb28121b849ab |
| raw/resources.assets/11237.bin | 44149e794c7d8fc9963eb0e9de2a4128e5116fd1741cf64ff08b7d0b071e5d5e |
| raw/resources.assets/11517.bin | fb6608343a2569b218f27c6acb2588d6c18d0973f7b24977f2905ea6934c0c01 |
| raw/resources.assets/11763.bin | a20dfa9ca1a7674445294737450b9e78f01acf3f75583e2c41e4f03efc2fc06c |
| raw/resources.assets/12003.bin | d92c16be1be8848c94eb2bd5892a54af7e1a815ec03e08ddfd19838a971ade2c |
| raw/resources.assets/12232.bin | 177015b14af680495c041d11de4d959cc7780ba56db607461454712c0d6cd295 |
| raw/resources.assets/12450.bin | 1b8d25b98d0c122c78da0806178d64a2f9879ecb6f2dc71353737bfb54f4f757 |
| raw/resources.assets/12642.bin | f413a9322d98a7099a678f85e53eaa2d9cb336b1d1eccf5c839cca729103dd42 |
| raw/resources.assets/12812.bin | f7521519cc28db6b901c27523fe8e07de6e47446cca65db4d58728720f30a867 |
| raw/resources.assets/12961.bin | 89a8e41323862a18e862c063edd1c04ce0e3ac107cd2dddde85b64c5497a5451 |
| raw/resources.assets/13102.bin | 5c88dbf5f45e6529c343b9e4abc90e09ae45fa5ebfbfc4785f5a5f79fac74551 |
| raw/resources.assets/501.bin | 5737c39bbf72813ea0f1f5bcb5644b16206caa8c8c35efae8e26e9f10bc3a825 |
| raw/resources.assets/73430.bin | d2ad709d2b1999292ce531e6b87198032b45a76038b012bcfb7d284204e5dd36 |
| raw/resources.assets/73492.bin | f8d540e45b4732b5462df30bd78af6d92907a0ebb89cc6d72225606495ba1be8 |
| raw/resources.assets/7914.bin | b32f49894b6dd16cead2997f8285ab3f7f35f2dfd65c38b05c15e75d55f40a16 |
| raw/resources.assets/7943.bin | fddef1aaf2dbee6e15478922b66ad27d8e6499d8d1fea914a021800c7fdd225d |
| raw/resources.assets/7963.bin | 8eb3f8ab4a19b340675382ba1210b9caa2c9421b642e23098f8c0dfb60df84e3 |
| raw/resources.assets/7981.bin | 7eff5c324b79c738adbcdb40f0dd98cd7b48d37362ddd610b5af5b006f3750d0 |
| raw/resources.assets/8228.bin | 9a2c86d15ec50df3e3be72e67e890a517becd60f877127b5a1f27b768c3adfeb |
| raw/resources.assets/8301.bin | 54aa17bb81bd9d13aeb7050bdc30fbd01bb192c1ac298e8ab925ee449f97b6c8 |
| raw/resources.assets/8319.bin | 130259e139573799a1d0485f208d7f971f7117ca6d4d103206988c69ec7ebae8 |
| raw/resources.assets/8337.bin | 0d57a9606d407af8d23f40586955754944f7f2b9d27c0d7aef7966a48904f0c4 |
| raw/resources.assets/8395.bin | afd23e9cbc45cf6f2868952c137f7e2a77c27b86373928506e096cbeea41ba5d |
| raw/resources.assets/8464.bin | e01680ddda21fcb7db6eec810defe701b2b9a6ddd267f2fbaf252fcc84c5b247 |
| raw/resources.assets/8494.bin | e4d5a29d185610a7f1849feef377a275f2a9e750c853d2309061d180f20f6c45 |
| raw/resources.assets/8557.bin | 36dbfcbcfbab9ed675c31e60400b9fe12637f031dfc354b2323df74c7d81d52c |
| raw/resources.assets/8584.bin | a7ee7a4cd137736dc21097aa5c5686be7c569c969f6153367af1011e8b01378e |
| raw/resources.assets/8593.bin | 3f5b4d307a37c0b442ac55106aee9e0c4d45e477572b6b08477b6900f6c6829e |
| raw/resources.assets/8695.bin | 09e43bf1a19b77e26a79fa184403e9a4128c7e64329b261eab0913ecab71e3ca |
| raw/resources.assets/8799.bin | 35d3a0084f34ba96cedde379f4ab1acf56c32d931c2a339b040b1f708054c77f |
| raw/resources.assets/9040.bin | 0c75fae90cbf1df6e3dffe58dce098fca608a82dd110ec2d578e5cf3bd204010 |
| raw/resources.assets/9140.bin | 86ca39566e948127cc8cf5961d16b05744ff221179c2a3c572a001d1f08a8539 |
| raw/resources.assets/9143.bin | 01eb99b3f1b64d6c0ff1bcf2cc3316198b08d34c7433142fc17aa14901414b3b |
| raw/resources.assets/9191.bin | 9eee0309c6e76c6a881e1749fbdfaa6f2fefc9203f6f1696bee5671f8cb12b30 |
| raw/resources.assets/9243.bin | ac9c41098fda1dc6bbd837a6d1d481fb2e31f9894f380bb710f58f54943993c3 |
| raw/resources.assets/9425.bin | d8b468829521de1afc4e86364bb099f16c401ddcf9ecc39d4be37ab94191d3c1 |
| raw/resources.assets/9577.bin | f8ddf8bf50c10167ece5dea242869b5410367f2663a0401a66480b6352b1b14b |
| raw/resources.assets/9617.bin | 83f2a2ade336eae80511bb1920387622747d47294ed8d0f1fea601c577c93cb2 |
| raw/resources.assets/9726.bin | e1ab35d376afbbacbc5acce60ee6720a523cee1798dac8dc82a63b66bd150d35 |
| raw/resources.assets/9751.bin | 45694cc7217a44eeb8081769116c31780abfe2a493f64737d9f921b7bdaa6eea |
| raw/resources.assets/9884.bin | 6664af757bde905b445368bd6c140f07830c87fe44bfdfe05109dc9b847333df |
| raw/sharedassets0.assets/171.bin | b2745f7652259b7e13c834ee8b004f8321270926eab309f899fec70bf7f75f8b |
| raw/sharedassets0.assets/25369.bin | 9ddffba61ae7e6da778e288f6744c526bfcf306ae149df95bc4bda6c343415cb |
| raw/sharedassets0.assets/7985.bin | 74868e8af38ee71166cb9f1bac1a81d2119cd88b008de545dcf4618a1612eb59 |
| reference-code/BaseSpriteFX.cs | c456b3c5518cd4a39a9260357cc1e9810ea609ab18efd4764b3f5d4345d752d6 |
| reference-code/Deer.cs | c9a9cfca0b6bc6e337ef5b491ebcae8d28754f5d81fbcc30ed21f40e45c70709 |
| streams/resources.assets.resS/644973680-8388608.bin | 0dd378b313f6c1d10172a46522bea6df25fa1f16a48bba5d933d9f3c880aed0a |
| streams/sharedassets0.assets.resS/146500400-4194304.bin | 25cc89cd719791bdeb3f14692f3f85e107a4a72c21c4454f16a3f8c06a071c55 |


### Actual native-policy tuples for this plan

Each tuple below is actual(path,line,column,member) from the final semantic scanner. Grouping supplies the path and enclosing actual signature; numbers are current source coordinates. CharacterPreview lists only this plan's new/changed composition hosts; its unchanged360-era bow/work/animation hosts plus unchanged BuildingPreview/IdentityKit tuples remain in the full immutable current observation. Every CorpsePreview/CrowdPreview tuple is included. Validation adds framework operations only and grants no native authority.

#### src/godot-game/inspection/CharacterPreview.cs — internal static CharacterPreview OpenComposition(Node2D root, MainThreadGateway gateway,

```text
97:95 Godot.Node.HasNode(Godot.NodePath)
102:9 Godot.CanvasItem.TextureFilter
102:30 Godot.CanvasItem.TextureFilterEnum.Nearest
103:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
103:29 Godot.Node2D.Node2D()
103:42 Godot.Node.Name
```

#### src/godot-game/inspection/CharacterPreview.cs — internal void RestoreComposition(string body, string pose, int facing, JsonElement equipment)

```text
141:22 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
151:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CharacterPreview.cs — internal void CloseComposition()

```text
164:26 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
165:13 Godot.Node.RemoveChild(Godot.Node)
166:13 Godot.Node.QueueFree()
168:13 Godot.GodotObject.RemoveMeta(Godot.StringName)
```

#### src/godot-game/inspection/CorpsePreview.cs — internal void Close()

```text
116:13 Godot.GodotObject.RemoveMeta(Godot.StringName)
```

#### src/godot-game/inspection/CorpsePreview.cs — private void Retire()

```text
219:13 Godot.Node.HasNode(Godot.NodePath)
221:27 Godot.Node.GetNode(Godot.NodePath)
222:13 Godot.Node.RemoveChild(Godot.Node)
223:13 Godot.Node.QueueFree()
228:13 Godot.GodotObject.Dispose()
```

#### src/godot-game/inspection/CorpsePreview.cs — private void Project()

```text
241:22 Godot.Node2D.Node2D()
241:35 Godot.Node.Name
241:52 Godot.CanvasItem.TextureFilter
241:68 Godot.CanvasItem.TextureFilterEnum.Nearest
242:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
246:25 Godot.Node2D.Node2D()
246:38 Godot.Node.Name
247:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
258:91 Godot.Vector2.Vector2(float, float)
273:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
273:35 Godot.Polygon2D.Polygon2D()
275:17 Godot.Node.Name
276:17 Godot.Polygon2D.Polygon
276:28 Godot.Vector2.Vector2(float, float)
276:55 Godot.Vector2.Vector2(float, float)
276:82 Godot.Vector2.Vector2(float, float)
277:21 Godot.Vector2.Vector2(float, float)
278:17 Godot.Polygon2D.Color
278:25 Godot.Color.Color(float, float, float, float)
279:17 Godot.CanvasItem.ZIndex
289:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CorpsePreview.cs — private void Wildlife(Node2D visual, string body, JsonElement sample)

```text
310:86 Godot.Vector2.Vector2(float, float)
316:9 Godot.Node2D.Position
316:28 Godot.Vector2.Vector2(float, float)
317:9 Godot.Node2D.Scale
317:24 Godot.Vector2.Vector2(float, float)
```

#### src/godot-game/inspection/CorpsePreview.cs — private void Pile(Node2D visual, JsonElement equipment, bool meatOnly)

```text
343:25 Godot.Vector2.Vector2(float, float)
344:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
345:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CorpsePreview.cs — private Sprite2D CommonSprite(Node2D parent, string id, string name, Vector2 at)

```text
357:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CorpsePreview.cs — private Sprite2D Sprite(Node2D parent, JsonElement pack, JsonElement part, string name, Vector2 at)

```text
366:27 Godot.ResourceLoader.Load<Godot.Texture2D>(string, string, Godot.ResourceLoader.CacheMode)
369:23 Godot.AtlasTexture.AtlasTexture()
371:13 Godot.AtlasTexture.Atlas
372:13 Godot.AtlasTexture.Region
372:22 Godot.Rect2.Rect2(float, float, float, float)
374:13 Godot.AtlasTexture.FilterClip
377:22 Godot.Sprite2D.Sprite2D()
377:37 Godot.Node.Name
377:50 Godot.Sprite2D.Centered
377:68 Godot.Sprite2D.Texture
377:87 Godot.Node2D.Position
377:102 Godot.CanvasItem.ZIndex
378:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
```

#### src/godot-game/inspection/CorpsePreview.cs — private void Outline(Node2D visual)

```text
385:28 Godot.Image.CreateEmpty(int, int, bool, Godot.Image.Format)
385:96 Godot.Image.Format.Rgba8
386:37 Godot.Node.FindChildren(string, string, bool, bool)
389:34 Godot.Texture2D.GetImage()
389:34 Godot.Sprite2D.Texture
390:33 Godot.Image.GetHeight()
392:37 Godot.Image.GetWidth()
394:34 Godot.Node2D.Transform
394:54 Godot.Vector2.Vector2(float, float)
394:94 Godot.Sprite2D.Offset
395:31 Godot.Vector2.Vector2(float, float)
396:25 Godot.Color.A
396:25 Godot.Image.GetPixel(int, int)
396:62 Godot.Vector2.X
396:86 Godot.Vector2.Y
398:25 Godot.Image.SetPixel(int, int, Godot.Color)
398:47 Godot.Vector2.X
398:61 Godot.Vector2.Y
398:74 Godot.Colors.White
404:31 Godot.Image.CreateEmpty(int, int, bool, Godot.Image.Format)
404:99 Godot.Image.Format.Rgba8
409:21 Godot.Color.A
409:21 Godot.Image.GetPixel(int, int)
409:58 Godot.Color.A
409:58 Godot.Image.GetPixel(int, int)
410:28 Godot.Color.A
410:28 Godot.Image.GetPixel(int, int)
410:67 Godot.Color.A
410:67 Godot.Image.GetPixel(int, int)
411:28 Godot.Color.A
411:28 Godot.Image.GetPixel(int, int)
413:21 Godot.Image.SetPixel(int, int, Godot.Color)
413:57 Godot.Color.Color(string)
418:32 Godot.ImageTexture.CreateFromImage(Godot.Image)
420:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
420:31 Godot.Sprite2D.Sprite2D()
422:13 Godot.Node.Name
422:39 Godot.Sprite2D.Centered
422:57 Godot.Sprite2D.Texture
422:76 Godot.Node2D.Position
422:87 Godot.Vector2.Vector2(float, float)
423:13 Godot.CanvasItem.ZIndex
425:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
425:31 Godot.Polygon2D.Polygon2D()
427:13 Godot.Node.Name
427:38 Godot.Polygon2D.Polygon
427:49 Godot.Vector2.Vector2(float, float)
427:76 Godot.Vector2.Vector2(float, float)
428:17 Godot.Vector2.Vector2(float, float)
428:45 Godot.Polygon2D.Color
428:53 Godot.Color.Color(string)
428:80 Godot.CanvasItem.ZIndex
```

#### src/godot-game/inspection/CrowdPreview.cs — internal void Attach()

```text
57:37 Godot.Node.HasNode(Godot.NodePath)
62:23 Godot.Node2D.Node2D()
62:36 Godot.Node.Name
62:52 Godot.CanvasItem.TextureFilter
62:68 Godot.CanvasItem.TextureFilterEnum.Nearest
63:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
64:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
64:32 Godot.Polygon2D.Polygon2D()
66:13 Godot.Node.Name
66:34 Godot.Polygon2D.Polygon
66:45 Godot.Vector2.Zero
66:59 Godot.Vector2.Vector2(float, float)
67:17 Godot.Vector2.Vector2(float, float)
67:48 Godot.Vector2.Vector2(float, float)
67:77 Godot.CanvasItem.ZIndex
69:25 Godot.Node2D.Node2D()
69:38 Godot.Node.Name
70:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
73:9 Godot.Node.RemoveChild(Godot.Node)
74:9 Godot.Node.QueueFree()
80:25 Godot.Node2D.Node2D()
80:38 Godot.Node.Name
80:49 Godot.Node2D.Position
80:60 Godot.Vector2.Vector2(float, float)
81:13 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
82:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
89:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
89:32 Godot.Node2D.Node2D()
89:45 Godot.Node.Name
89:71 Godot.Node2D.Position
89:82 Godot.Vector2.Vector2(float, float)
90:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
90:32 Godot.Node2D.Node2D()
90:45 Godot.Node.Name
90:69 Godot.Node2D.Position
90:80 Godot.Vector2.Vector2(float, float)
```

#### src/godot-game/inspection/CrowdPreview.cs — internal void SetNight(bool night)

```text
168:9 Godot.CanvasItem.Modulate
168:9 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
168:57 Godot.Color.Color(string)
169:9 Godot.Polygon2D.Color
169:9 Godot.Node.GetNode<Godot.Polygon2D>(Godot.NodePath)
169:68 Godot.Color.Color(string)
```

#### src/godot-game/inspection/CrowdPreview.cs — internal void Wildlife(string body, string action, double elapsedSeconds, int facing, bool selected)

```text
203:24 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
208:18 Godot.Vector2.Vector2(float, float)
211:13 Godot.Vector2.X
214:22 Godot.Sprite2D.Sprite2D()
216:13 Godot.Node.Name
216:30 Godot.Sprite2D.Centered
216:48 Godot.Sprite2D.Texture
216:77 Godot.Node2D.Position
217:13 Godot.Node2D.Scale
217:21 Godot.Vector2.Vector2(float, float)
217:51 Godot.CanvasItem.TextureFilter
217:67 Godot.CanvasItem.TextureFilterEnum.Nearest
219:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
220:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
221:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CrowdPreview.cs — internal void Close()

```text
245:17 Godot.Node.HasNode(Godot.NodePath)
247:32 Godot.Node.GetNode(Godot.NodePath)
250:25 Godot.Node.HasNode(Godot.NodePath)
252:47 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
256:17 Godot.Node.RemoveChild(Godot.Node)
257:17 Godot.Node.QueueFree()
262:17 Godot.GodotObject.Dispose()
266:13 Godot.GodotObject.RemoveMeta(Godot.StringName)
```

#### src/godot-game/inspection/CrowdPreview.cs — private void Publish()

```text
669:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CrowdPreview.cs — private void Identity(string id)

```text
694:25 Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)
695:13 Godot.Node.HasNode(Godot.NodePath)
697:29 Godot.Node.GetNode(Godot.NodePath)
698:13 Godot.Node.RemoveChild(Godot.Node)
699:13 Godot.Node.QueueFree()
702:21 Godot.Node2D.Node2D()
702:34 Godot.Node.Name
702:53 Godot.Node2D.Position
702:64 Godot.Vector2.Vector2(float, float)
703:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
704:21 Godot.Color.Color(string)
707:25 Godot.Vector2.Vector2(float, float)
710:26 Godot.Node2D.Node2D()
710:39 Godot.Node.Name
710:52 Godot.Node2D.Position
710:63 Godot.Vector2.Vector2(float, float)
712:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
713:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
713:35 Godot.Polygon2D.Polygon2D()
713:51 Godot.Node.Name
713:67 Godot.Polygon2D.Polygon
713:85 Godot.Polygon2D.Color
714:25 Godot.Label.Label()
714:37 Godot.Node.Name
714:52 Godot.Control.Position
714:63 Godot.Vector2.Vector2(float, float)
715:17 Godot.Label.Text
716:13 Godot.Control.AddThemeFontSizeOverride(Godot.StringName, int)
717:13 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
720:9 Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)
```

#### src/godot-game/inspection/CrowdPreview.cs — private AtlasTexture Texture(string id)

```text
735:31 Godot.ResourceLoader.Load<Godot.Texture2D>(string, string, Godot.ResourceLoader.CacheMode)
738:23 Godot.AtlasTexture.AtlasTexture()
738:42 Godot.AtlasTexture.Atlas
738:57 Godot.AtlasTexture.Region
738:66 Godot.Rect2.Rect2(float, float, float, float)
740:17 Godot.AtlasTexture.FilterClip
```

#### src/godot-game/inspection/CrowdPreview.cs — private void RetireWildlife(Node2D owner)

```text
750:32 Godot.Node.GetChildren(bool)
752:13 Godot.Node.RemoveChild(Godot.Node)
753:78 Godot.StringName.ToString()
753:78 Godot.Node.Name
756:17 Godot.GodotObject.Dispose()
756:17 Godot.Sprite2D.Texture
759:13 Godot.Node.QueueFree()
```

#### src/godot-game/inspection/CrowdPreview.cs — private void WildlifeOutline(Node2D owner, Sprite2D sprite)

```text
766:30 Godot.Texture2D.GetImage()
766:30 Godot.Sprite2D.Texture
767:30 Godot.Image.CreateEmpty(int, int, bool, Godot.Image.Format)
767:55 Godot.Image.GetWidth()
767:86 Godot.Image.GetHeight()
768:44 Godot.Image.Format.Rgba8
769:29 Godot.Image.GetHeight()
771:33 Godot.Image.GetWidth()
773:21 Godot.Color.A
773:21 Godot.Image.GetPixel(int, int)
775:57 Godot.Vector2I.Left
775:72 Godot.Vector2I.Right
775:88 Godot.Vector2I.Up
775:101 Godot.Vector2I.Down
777:25 Godot.Image.SetPixel(int, int, Godot.Color)
777:52 Godot.Vector2I.X
777:73 Godot.Vector2I.Y
777:90 Godot.Color.Color(string)
783:29 Godot.Image.GetHeight()
785:33 Godot.Image.GetWidth()
787:21 Godot.Color.A
787:21 Godot.Image.GetPixel(int, int)
789:21 Godot.Image.SetPixel(int, int, Godot.Color)
789:64 Godot.Colors.Transparent
794:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
794:30 Godot.Sprite2D.Sprite2D()
796:13 Godot.Node.Name
796:39 Godot.Sprite2D.Centered
796:57 Godot.Sprite2D.Texture
796:67 Godot.ImageTexture.CreateFromImage(Godot.Image)
797:13 Godot.Node2D.Position
797:24 Godot.Node2D.Position
797:42 Godot.Vector2.Vector2(float, float)
797:57 Godot.Vector2.X
797:57 Godot.Node2D.Scale
797:80 Godot.Node2D.Scale
797:88 Godot.Node2D.Scale
797:102 Godot.CanvasItem.ZIndex
799:9 Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)
799:30 Godot.Polygon2D.Polygon2D()
799:46 Godot.Node.Name
799:71 Godot.Polygon2D.Polygon
799:82 Godot.Vector2.Vector2(float, float)
800:13 Godot.Vector2.Vector2(float, float)
800:39 Godot.Vector2.Vector2(float, float)
800:66 Godot.Polygon2D.Color
800:74 Godot.Color.Color(string)
```

Complete native observation SHA256aa2ff4c6c455a23730ce8647f6bb529b4a2457a49812d7bf288b7098a1e9683c. Exact machine-readable artifact/symbol/media/member inventory: .phase4/04-10-r1-final/artifact-inventory.json. No new security surface outside the plan's local-file/parser/native/evidence threat model was introduced.

## Self-Check: PASSED

All12 owned files exist and match their final414-source freeze. Every phase-owned commit above is an ancestor ofd2392ff33e682d774e5999728496919f1e9dc62d; exact Task2 committed Git-filtered bytes match tested raw inputs. All20 selected outcomes pass with drift[],80 definition rows preserve detached/path-safe semantics,actual native exits are0 and strict production/harness builds are0warnings/0errors. Three fixed Task2 data/PNG identities still match the original checkpoint. Original failures/foreign ancestry remain preserved. Parent receives this canonical summary before optional follow-up work and owns global state reconciliation.

