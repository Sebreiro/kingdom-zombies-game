---
phase: 04-asset-production
plan: "21"
subsystem: audio-resources
tags: [pcm, donor-provenance, exact-frame-regions, creator-attestation, source-headless]
status: complete
requires:
  - phase: 04-20
    provides: Independent canonical AU contracts and presentation/preferences
  - phase: 04-02
    provides: Pinned isolated decoder and truthful listening route
  - phase: 04-06
    provides: Character inspection and ownership boundaries
  - phase: 04-26
    provides: Existing guarded hub controls
  - phase: 04-27
    provides: World inspection and lifecycle contracts
provides:
  - Forty action sources and44 unchanged creator-approved version1 adaptations
  - Twenty-three additional full sources and27 actual new variants
  - Five stereo loops and complete30-category91-route DT23 catalog
  - Two saved Critical audio cases and fresh47-case selected-set pass
affects: [04-23, 04-24, 04-29, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Owner-isolated decoding, Integer PCM regions, Hash-scoped attestation, Preserve-or-create publication]
key-files:
  created:
    - src/godot-game/assets/audio/actions.wav
    - src/godot-game/content/packs/audio-actions.json
    - production/phase4/audio-actions.zip
    - tools/phase4/cases/audio.cjs
    - src/godot-game/assets/audio/interface-ambience.wav
    - src/godot-game/content/packs/audio-interface-ambience.json
    - src/godot-game/content/audio/events.json
    - production/phase4/audio-interface-ambience.zip
  modified: []
key-decisions:
  - Bind the literal creator attestation only to exact presented audio/category versions and preserve unknown listener details.
  - Retain complete WAV/raw sources and recipes rather than discarding content to fit a generic reader.
  - Permit64MiB only for the exact canonical tools archive path and leave global/native publisher compatibility pending.
  - Keep27 new variants Not auditioned and native mix/loop hearing with04-32.
requirements-contributed: [ASSET-01, ASSET-02, ASSET-04]
requirements-completed: []
actuals:
  tokens: 135739
  tasks: 2
  commits: 8
  diff_chars: 542954
  scale: "ceil(chars/4) over the exact eight-path realized Git diff, including binary notices; not harness tokens"
commits: 8
plan_head_before: e34e60d0fa22346a0fbc38786e582ec79723fcfb
plan_head_after: 54059b73503acec1e30a95f6704ac97f82249e2b
duration: "503.1074min elapsed"
duration_scope: "First preserved decoder command through final selection, including creator-approval and coordinator waits"
selected_case_duration_ms: 1628532
selected_wall_duration_ms: 1639745
completed: 2026-11-18
---

# Phase 04 Plan 21: Recorded sound banks and event routing summary

**Two actual PCM banks provide all30 canonical sound categories,63 recovered full sources,71 sample resources and91 event routes, with a fresh47/47 executable source/headless selected-set pass.**

This completes the approved local resource-production contract. All ASSET requirements remain pending. Task1's presented40sources/44version1 adaptations have creator source/category approval. All27 new Task2 variants remain **Not auditioned**; native mixing, actual loop seams and04-32/33 judgments remain pending.

## Delivered resources

The action bank contains2498522 stereo frames at44100Hz PCM16, duration56.65582766439909seconds. All40 original full-source WAVs,44 adaptations, IDs/versions/regions/frame/channel/rate/trim/gain/PCM/WAV identities stayed unchanged through approval and repairs. Five mono originals stay mono in the retained full WAVs; only their documented bank adaptations duplicate mono to stereo.

The interface/ambience bank contains3276941 stereo frames at44100Hz PCM16, duration74.30705215419501seconds. It supplies the required transfers/recruitment/equipment/inventory/food/warnings/portal/outcomes/session/save/UI/forest/fire/wind/bloater/handling recordings and explicitly shares existing action resources where declared. Full sources, raw AudioClip objects, compressed payloads, locators, preservation/tool identities and exact recipes remain in tracked ZIPs.

Five stereo loops cover active portal, fire, forest day, forest night and wind:352800 output frames each, local loop[0,352800), source trim[0,353312),512-frame tail/head overlap. Complete original ambience WAVs are retained. Every new trim/gain/channel/loop construction reproduces exactly. Mathematical nonzero/peak/frame checks establish PCM integrity, not perceived silence, clipping, suitability or seamless looping.

The sole synthesized source is the AU19 non-tonal food-contact/crunch texture, seed190021. The approved asset register explicitly identifies no verified suitable human-food donor; animal-eating recordings were not relabeled as human sounds. The complete generated WAV and expanded deterministic recipe are retained. No unheard donor was declared unsuitable, and no music/dialogue bank was authored.

DT23 uses the independent canonical30 event IDs from04-20/spec, without adapting them to stale consumer aliases. Its91 operations specify variants/bus/gain/attenuation/priority/cooldown/repetition/lifecycle. Defaults remain configurable24global/four-per-effect and80/80/60 master/effects/ambience. Forest/wind/fire gains are0.35/0.2/0.3. Current loops reconcile by generation/entity; historical one-shots never replay. Warning/error/outcome routes retain visible UI pairs when muted. Portal behavior is hostile-active/spawn/permanent destruction without player teleport; bloater audio is a single cosmetic death burst without persistent damage/burning/stain loop. Context-sensitive existing consumer classification remains04-23-owned.

## Actual approval and listening truth

The creator replied exactly **“All is approved”** to the presented AU01-15 review. The coordinator observed the reply at2026-11-18T10:25:36UTC; this is not an invented hearing timestamp.

The retained `creator-au01-15-v1` creator/user chat attestation binds that literal statement to presented manifest SHA256 `826ba6acae7cf2a3b07c18903a0f18756e22893cf16748292513051f048daf69`, original packet `f37896dadfd5154a2f1c34d47f16f5714fc5bc2b349641159a893eb5eacc73c8`, unchanged bank, complete44version/hash matrix and fifteen source/category uses. Personal name, output device, player, hearing timestamp and detailed observations remain null. The assistant did not hear these sounds. Approval does not extend to changed/new audio, AU16-30 uses or final native mix.

Original Not auditioned review material and genuine pre-approval `AUDIO_LISTENING_REQUIRED` reports are preserved at their original revisions. The current action ZIP retains that original review, full presented manifest, exact attestation and actual coordinator record. Runtime/case validation does not read .planning or ignored folders.

Every27 new variant and five auditory seam verdicts remain Not auditioned. A deliberate source-only review is prepared at `.phase4/audio-interface-review-v1/review/index.html` and retained in the ZIP. No playback, machine hearing, real game window, physical input or final mix/appearance/readability judgment occurred.

## Task commits and original evidence

| Task | GREEN commit | Required proof |
| --- | --- | --- |
| 1: action/impact/actor/work | `3df8209f4bc4a22b505fe1a24ec462199f0ae98b` | Initial6d36f764 GREEN; fresh SAME-CASE committed tracer `aa09e941-4cbb-442d-ad2b-af3cadccd72c` passed before Task2 |
| 2: interface/ambience/events | `54059b73503acec1e30a95f6704ac97f82249e2b` | Actual authoring continuation succeeds; saved bank `07b6db9c-abfa-4d07-b108-87921652b27c` and final committed bank `12b64a19-bb14-4f75-a36e-9865c6a62422` pass |

[Listening checkpoint](04-21-LISTENING-CHECKPOINT.md), [actual approval/resume](04-21-APPROVAL-AND-RESUME.md), [archive amendment](04-21-ARCHIVE-AMENDMENT.md), [technical checkpoint](04-21-TECHNICAL-CHECKPOINT.md) and [publication repair](04-21-PUBLICATION-REPAIR.md) remain preserved. No historical failed or incomplete invocation is relabeled.

The persisted ledger measures8 commits through tested code HEAD, including checkpoint/coordinator commits; it excludes the later SUMMARY metadata commit:

```text
ab6826b wip(04-21): preserve donor candidate review awaiting real audition
318f288 fix(04-21): preserve exact LF candidate manifest identities
e207413 docs(04): record required audio audition checkpoint without progress credit
04d85c0 docs(04-21): record creator source approval and bounded resume
3df8209 feat(04-21): bind exact action recordings to creator source approval
8c5295a docs(04-21): bound retained interface archive exception and consumer repair
54b7028 docs(04-21): preserve helper checkpoint and bound exact-output publication repair
54059b7 feat(04-21): publish interface ambience and complete event bank
```

## Deviations and bounded repairs

- **Original Task1:3/3.** Correct the copied recipe root after AUDIO_OUTPUT_ROOT; preserve five actual mono clips after the inherited stereo-only AUDIO_FORMAT failure; then emit explicit LF after Git exposed normalized working/blob differences. Source audio stayed fixed. Committed pre-approval `9a5b2af8-40af-4047-a676-f46a9755eb91` correctly failed the real listening prerequisite.
- **Prospective R1: initial PASS,0/2 corrections.** The actual creator reply resolved source approval. Promote only exact unchanged metadata and retain independently validated scope/statement/hash/version evidence. Original3/3 was not reset.
- **Original Task2:3/3.** Fix source-ID dictionary ordering; correct delivery/gain/UI-pair semantics with independent literal assertions; then apply the prospective exact archive boundary allowance. The required archive is64820348bytes/125members/81440487expanded bytes, largest member16228844bytes. Complete full WAV/raw sources were retained.
- **Exact C3 allowance.** Only ordinal `production/phase4/audio-interface-ambience.zip` receives64MiB compressed tools read/publication. Other paths remain32MiB, including actions.zip/wrongcase/lookalikes/otherfolders. Member32MiB/expanded128MiB/count/containment/reparse/singlelink/full-read/hash/source/PCM rules stay unchanged. Real64/64+1 and32/32+1 reader fixtures, wrong-hash/restored controls and destination-selected publication prove the narrow boundary. Reader fixture acceptance is separate from valid125-member ZIP verification.
- **Checkpoint before another correction.** Saved bank `36c7cb7b-8b83-4d78-aa2d-b8baee3700da` passed, but authoring failed EEXIST after verified archive publication when exclusively creating already-correct EVENTS. The technical checkpoint SHA256 `d89aee41a722da839386c19797e5879b76266ba82a85b77dc22c56cb9e442d20`, exact ignored five-file snapshot/checkpoint SHA256 `6d4e43743a1cbbe2d0a98b487e8ef676999c7dbc4effde47d92decd886da86ad`, original reports and archive-only receipt are intact. No fourth original correction occurred.
- **Prospective R2: initial PASS,0/2 corrections.** Only audio.cjs changed. Preflight exact EVENTS before publication; preserve identical existing bytes or create absent bytes exclusively after verified archive; reject conflicts and recheck afterward. Existing canonical archives preserve only exact independently expected staged/destination length/hash/bytes; ordinary duplicate guards remain. All four Task2 artifact bytes and Task1 approved bytes stayed fixed.

Clean actual receipt: `.phase4/audio-publication/cebb9130-5aff-437e-b099-8a469f1602d4/authoring-complete.json`, written after archive/EVENTS identities are confirmed. Separate controls prove existing/missing/conflicting EVENTS, conflicting ZIP, ordinary duplicate rejection, mutation/exclusive-creation fences and restored positive. The earlier archive-only failed-command receipt remains historical.

Original action controls retain thirteen negative identities/twelve restored positives, plus six approval controls:19negative/18restored. Task2 retains19semantic/source/PCM negatives/19restored, exactly **eleven** reader/publication outcomes, plus eight separate R2 controls. No fabricated reviewer, unsupported approval, unapproved version/hash/region/source or unsupported donor rejection is accepted.

Initial sandbox EPERM used equivalent approved bounded execution. One redundant ZIP overwrite met Windows' mapped-file restriction; hashes proved the destination already exact, requiring no deletion/process termination. One incidental Node command syntax typo executed no production statement and was corrected. Closeout's read-only Git child similarly required equivalent escalation after EPERM. No package install, paid dependency, auth gate, deadline bypass, hidden stderr, branch switch, merge, reset, stash, broad stage or push occurred.

## Fresh final selection

**47/47 PASS**, one actual source HEAD `54059b73503acec1e30a95f6704ac97f82249e2b`,481unchanged frozen inputs, zero stops/drift/scheduling revisions. The exact selection is prior45 executable Critical/new/affected cases plus both audio cases, with compatible ordered groups and safe between-case STOP. No active proof child was interrupted; individual proof deadlines/assertions stayed unchanged.

Case execution: **1628532ms (27.142min)**; wall: **1639745ms**. Start2026-11-18T10:55:16.638Z; finish2026-11-18T11:22:36.383Z. Whole-plan elapsed503.1074min includes creator/coordinator waits from first preserved command2026-11-18T02:59:29.939Z, not continuous execution.

Evidence: `.phase4/04-21-final/selection.json`, `results.json`, `completion.json`. Each result preserves actual invocation/finished HEAD;26 raw assembly closure records retain exact identities/hashes. Inherited assembly reuse is not mislabeled as new compilation. Prior45 ated8a79c/473inputs/1588583ms is history. No standalone conditional solution build or unchanged policy/cohort observation was added for media/CJS changes.

| Case | Invocation | Duration ms | Result |
| --- | --- | --- | --- |
| export.layout | `995f1227-934e-4a93-8b54-ba43278fad35` | 25483 | PASS |
| audio.feasibility | `54d3a42b-0494-422c-b09f-7fde3e3e0bbc` | 9454 | PASS |
| media.conversion | `ff8c50dc-3c9c-47ac-a1e6-9b799c9e744f` | 43406 | PASS |
| media.scale | `2f80c927-e3d7-4716-a2cb-0f9970277450` | 8144 | PASS |
| items.weapons-food | `945f72cf-8af5-4b74-bda6-9a5c6580cf40` | 7909 | PASS |
| items.wearables | `7fb2b654-87eb-4507-b890-a6bd7e51b581` | 13212 | PASS |
| buildings.main-art | `ea6ac441-3e0e-4b8e-ab48-208e15432b83` | 5077 | PASS |
| buildings.main-state | `27c3222b-b4ab-4b56-a53c-bf049dff8e9f` | 26259 | PASS |
| bodies.a-art | `770221e0-e784-4989-853b-90fb8a69f528` | 8003 | PASS |
| bodies.a-fit | `7fdd1b67-bf7a-4908-afe2-d1635bd4093f` | 27443 | PASS |
| bodies.b-wide | `5aa80cd0-8b47-4957-ae04-60270783eb38` | 45833 | PASS |
| bodies.complete-fit | `9c25cbd7-1ea6-4fec-bd0f-9b9d7b109d14` | 48564 | PASS |
| layouts.theme | `abfb910a-ba20-403f-8d1d-9e509ad51a5a` | 14295 | PASS |
| layouts.identity | `6acab436-89b7-497a-8813-2d5ad2b81341` | 12542 | PASS |
| layouts.identity-integrity | `0c3394f0-079f-42b3-a7fe-d159e131d5c5` | 14745 | PASS |
| layouts.inventory | `5eb623c1-75aa-40de-a9c7-68bfec4b705d` | 16105 | PASS |
| layouts.legacy-target | `11950918-e912-456f-8a6a-22c91c664cb2` | 18469 | PASS |
| layouts.camp | `778fedb4-67a0-4c36-8904-360dbe8dfb3f` | 14755 | PASS |
| layouts.map-warnings | `0ae8fde5-3144-49cf-95e1-428005b23d9a` | 14922 | PASS |
| layouts.session | `db2e1b45-4c4a-4a69-bad5-0d084be6e574` | 14642 | PASS |
| layouts.endpoints | `ec42aff7-e6a9-4acd-80c5-e6c665b153b3` | 19164 | PASS |
| animation.melee | `c3b8f10b-3923-4616-abc8-1f0efb3f5cab` | 24802 | PASS |
| animation.replacement | `1053987c-13ed-441b-ba03-a89e3d4d377d` | 24726 | PASS |
| animation.bow | `2a50ea2f-293e-4c4a-a081-9915d81806c2` | 44767 | PASS |
| animation.work-special | `eb7fe7d9-5e0b-400e-92b2-bfa3069a606f` | 25926 | PASS |
| appearance.corpses | `cb4adc47-412d-4b35-82f6-d8fcf953a183` | 25850 | PASS |
| appearance.crowds | `56e9081d-2b39-4443-8139-57c1635965ca` | 30877 | PASS |
| audio.actions | `90648f2d-3d7c-46a6-a17c-6a5b97570415` | 2800 | PASS |
| audio.bank | `12b64a19-bb14-4f75-a36e-9865c6a62422` | 5942 | PASS |
| data.foundation | `d727a23d-cfc9-4483-81a7-703247a00342` | 493 | PASS |
| data.items-combat | `9f601d24-3755-4235-9801-9c29a5e8dc05` | 7022 | PASS |
| data.economy | `8e2db674-35a4-4b02-895a-69b385a38e3e` | 1032 | PASS |
| data.world | `bf68fd27-c54b-433c-84b4-af2070dde0bb` | 3069 | PASS |
| data.zombies | `d5f6c9ca-d1dd-44d4-8368-97d0f74dcb99` | 64588 | PASS |
| data.remaining | `23ba2614-e297-44bf-9463-931901bd840c` | 3478 | PASS |
| buildings.defense-art | `f1510695-de77-486e-8395-00845f85a937` | 2078 | PASS |
| buildings.defense-state | `625ba80b-f3f8-4af2-98c4-8580f058312e` | 11169 | PASS |
| export.profile | `047b32fb-5f52-4728-a09c-98bb1a1f6c1a` | 9001 | PASS |
| export.profile-boundaries | `29514c01-e065-4ade-ad33-b8157c92a2d1` | 44223 | PASS |
| hub.source | `be009d91-9f45-4615-9b01-094008643daf` | 386272 | PASS |
| hub.lifecycle | `9daef183-9ff9-4d2e-b1fb-6525692fcd52` | 388071 | PASS |
| source.case-discovery | `f2d362ca-f80c-4c98-9ef6-0f3c1809bacc` | 738 | PASS |
| world.map | `efd16132-7004-4293-91db-7666741fd9db` | 6238 | PASS |
| world.effects | `e8964683-2968-4ec8-8ddc-46a1d5d0cac1` | 20862 | PASS |
| world.inspector | `8cb0bec2-adc2-4d8a-85a2-03e061e986f8` | 21950 | PASS |
| world.replacement | `ff3bb632-7a3e-4896-8199-2788ca34d317` | 31882 | PASS |
| graph.visibility-inventory | `4cfd304f-000f-4dcd-8ec2-79fb8476b58c` | 32250 | PASS |

## Pending acceptance and integration

The entire justified executable source/headless selection passed; `fullPhaseSelectedSetPass:false` correctly preserves missing whole-phase actual acceptance.

`source.provenance` remains the unchanged historical adb38f27 failure; `source.publication` remains **source-inferred blocked**, without a fabricated directly executed failure. Their unexecuted independent controls remain04-23/31-owned. Known source.tracer/tracer-failures/packaged-expectations/export.feasibility integrations and unaffected layouts.kit retain the selection's explicit omissions.

Existing49 consumer comparisons/context-sensitive hit/death/regen/impact routes remain04-23-owned. This64MiB exception closes only the local tools producer/case boundary. Global content.cjs/fileRef/validate/publish/readPublished/provenance consumers still use32MiB and need their prospectively assigned04-23/31 reconciliation. No full publication/native/runtime compatibility is claimed.

WINDOWS29/30/31, index0/180, recorded08be5ab2 policyFAIL2130 and57-input32-surface unenrolled producer cohort remain pending. New audio hearing/seams/mix, whole-phase independent review/fixes, Windows1280x720/no-VM physical input/draw/judgments and04-32/33 reconciliation remain required. No whole-game, physical LAN, FPS/hardware/endurance, supported-display or hosted other-OS credit is claimed.

No production stub/TODO/FIXME, skipped test or applicable unrun source/headless verify remains. Null unprovided creator details, null one-shot loops and explicit Not auditioned are intentional truthful states with owners. They supply no false AC12/ASSET/native-mixer acceptance. No unmodeled network/auth/schema trust surface was introduced; local source/archive boundaries and the exact64MiB exception are covered by the plan/amendments. New native-policy/public C# tuples: **none**.

Parent owns global STATE/ROADMAP/REQUIREMENTS/PROJECT updates,28→29local/53→54global,next04-29,wave23post and draftPR5/push. This executor changed none of those globals, the initial spec, removed06-DISCUSS-CHECKPOINT.json or other agents' work. No ASSET or other-phase advancement occurs here.

## Artifacts this phase produces

All eight paths exist; raw working bytes equal committed blobs. Four fixed Task2 artifacts equal their preserved checkpoint identities. All selected full source/raw/variant/recipe bytes are tracked in the ZIPs, not cache-only.

| Exact file | Bytes | SHA256 |
| --- | --- | --- |
| [src/godot-game/assets/audio/actions.wav](../../../src/godot-game/assets/audio/actions.wav) | 9994132 | `fb173a9863cbc2ba1989cea83afddea3497d22bc760ba28803b9d5e8a545e144` |
| [src/godot-game/content/packs/audio-actions.json](../../../src/godot-game/content/packs/audio-actions.json) | 207376 | `252eb90d37440a7176b10afcb1408920a6bf9f19ae466185c9fe3d624d6c9154` |
| [production/phase4/audio-actions.zip](../../../production/phase4/audio-actions.zip) | 15897100 | `7febdb1994bfde067f83545b751bc9f23763869609f7ab8e9273e54f4992da23` |
| [tools/phase4/cases/audio.cjs](../../../tools/phase4/cases/audio.cjs) | 79549 | `28c8b227d0dfae7f70fca8472727970b0dbcf917c307311b493197f8f4bb8c2d` |
| [src/godot-game/assets/audio/interface-ambience.wav](../../../src/godot-game/assets/audio/interface-ambience.wav) | 13107808 | `e61d445d30ab28d65d163ea89fcc185484b4746f35703cb2ed088f13308ba5b9` |
| [src/godot-game/content/packs/audio-interface-ambience.json](../../../src/godot-game/content/packs/audio-interface-ambience.json) | 115893 | `2e107c4ef6eb57262a8f571a1226b694f552a15b806117bcb67e6be7ddfda069` |
| [src/godot-game/content/audio/events.json](../../../src/godot-game/content/audio/events.json) | 123935 | `375b8199b0b8b0ff168f6cb5894cf5139bcbdb97fc081c4bee0329bcfe948d05` |
| [production/phase4/audio-interface-ambience.zip](../../../production/phase4/audio-interface-ambience.zip) | 64820348 | `cfa52aeac32a22fcf4898e05fa64a6225016af404ae990632ffd07fb130fb1d2` |

Named JavaScript signatures: decoderRecipe(); promote(); validateApproval(pack); inputs(); prepare(); validate(pack,bank); technical(context); actions(context); archiveLimit(relative); archiveFile(file,destinationIdentity,expectedHash); archiveBytes(root,relative,expectedHash); publishArchive(sourceRoot,sourceRelative,destinationRoot,destinationRelative,expectedHash); preflightEvents(root,expected); finishEvents(root,expected,state); publishInterface(directory); archiveControls(context); publicationControls(context); interfaceDecoder(); eventCatalog(interfacePack); prepareInterface(); validateInterface(pack,bank,events); bank(context). Arrow helpers: `approvedListening()`, `ids()`, `sourceChannels(id)`, `ownerOf(key)`, `serializeEvents(value)`. Exact CommonJS exports: `CANDIDATES, COVERAGE, ASSEMBLE, PROMOTE, APPROVAL, decoderRecipe, prepare, promote, validate, technical, actions, INTERFACE_CANDIDATES, INTERFACE_ASSEMBLE, interfaceDecoder, eventCatalog, prepareInterface, publishInterface, validateInterface, archiveBytes, publishArchive, bank`. Embedded retained Python declarations include `sha(data)`, `file_hash(file)`, `owned(value)`, `decode(identity,destination)`, `wav(pcm)` and `add(identity,data,source,trim,gain,loop,transform)`; full signatures/source remain in the recipe members.

Authoring modes: `--prepare`, `--approve-actions`, `--prepare-interface`, `--publish-interface OWNED_STAGE_DIRECTORY`, `--technical`. Cases: `audio.actions` and `audio.bank`, Critical/source-headless/audio/G0/orders121,122. No new C# collaborator, native/public signature or permission tuple.

All71 sample resource IDs/versions/exact integer bank regions are below. Full source tuples and WAV/PCM/frame/channel/rate/trim/gain/loop/preservation/listening identities remain in the two tracked pack/retained production manifests.

| Resource ID | Version | Frames | Bank region [start,end) |
| --- | --- | --- | --- |
| action.3294.full | 1 | 11025 | [0,11025) |
| action.3294.jump-trim | 1 | 5292 | [11025,16317) |
| action.4949.full | 1 | 11025 | [18522,29547) |
| action.4949.jump-trim | 1 | 5292 | [29547,34839) |
| action.3369.full | 1 | 19293 | [37044,56337) |
| action.4872.full | 1 | 17847 | [58542,76389) |
| action.5123.full | 1 | 87537 | [78594,166131) |
| action.2880.full | 1 | 14969 | [168336,183305) |
| action.3700.full | 1 | 11785 | [185510,197295) |
| action.4523.full | 1 | 120931 | [199500,320431) |
| action.4523.break-trim | 1 | 6174 | [320431,326605) |
| action.3574.full | 1 | 121010 | [328810,449820) |
| action.3574.break-trim | 1 | 6174 | [449820,455994) |
| action.3288.full | 1 | 3016 | [458199,461215) |
| action.5476.full | 1 | 16537 | [463420,479957) |
| action.3640.full | 1 | 4249 | [482162,486411) |
| action.4434.full | 1 | 6109 | [488616,494725) |
| action.5020.full | 1 | 6109 | [496930,503039) |
| action.3357.full | 1 | 3596 | [505244,508840) |
| action.4692.full | 1 | 13498 | [511045,524543) |
| action.2790.full | 1 | 223785 | [526748,750533) |
| action.2864.full | 1 | 71486 | [752738,824224) |
| action.3150.full | 1 | 22050 | [826429,848479) |
| action.3131.full | 1 | 43862 | [850684,894546) |
| action.2804.full | 1 | 154350 | [896751,1051101) |
| action.3554.full | 1 | 63210 | [1053306,1116516) |
| action.5736.full | 1 | 196608 | [1118721,1315329) |
| action.3944.full | 1 | 27633 | [1317534,1345167) |
| action.4247.full | 1 | 33075 | [1347372,1380447) |
| action.3811.full | 1 | 19110 | [1382652,1401762) |
| action.5272.full | 1 | 14401 | [1403967,1418368) |
| action.3945.full | 1 | 12862 | [1420573,1433435) |
| action.3804.full | 1 | 6878 | [1435640,1442518) |
| action.5176.full | 1 | 9510 | [1444723,1454233) |
| action.4611.full | 1 | 31237 | [1456438,1487675) |
| action.3741.full | 1 | 209475 | [1489880,1699355) |
| action.3672.full | 1 | 159450 | [1701560,1861010) |
| action.2862.full | 1 | 132300 | [1863215,1995515) |
| action.4543.full | 1 | 81799 | [1997720,2079519) |
| action.4245.full | 1 | 95986 | [2081724,2177710) |
| action.4169.full | 1 | 33923 | [2179915,2213838) |
| action.4041.full | 1 | 33923 | [2216043,2249966) |
| action.5654.full | 1 | 120931 | [2252171,2373102) |
| action.5154.full | 1 | 121010 | [2375307,2496317) |
| interface.r3254.full | 1 | 33075 | [0,33075) |
| interface.r4864.full | 1 | 11323 | [35280,46603) |
| interface.r3195.full | 1 | 23221 | [48808,72029) |
| interface.r3863.full | 1 | 12721 | [74234,86955) |
| interface.r4973.full | 1 | 65536 | [89160,154696) |
| interface.r3755.full | 1 | 84640 | [156901,241541) |
| interface.handling.v1 | 1 | 6615 | [243746,250361) |
| interface.r5586.full | 1 | 198450 | [252566,451016) |
| interface.r3558.loop | 1 | 352800 | [453221,806021) |
| interface.r2860.full | 1 | 115762 | [808226,923988) |
| interface.r5582.full | 1 | 40632 | [926193,966825) |
| interface.r4364.full | 1 | 230821 | [969030,1199851) |
| interface.r4441.loop | 1 | 352800 | [1202056,1554856) |
| interface.s0438.full | 1 | 24738 | [1557061,1581799) |
| interface.s0485.full | 1 | 28780 | [1584004,1612784) |
| interface.s0485.negative | 1 | 15435 | [1614989,1630424) |
| interface.s0449.full | 1 | 9242 | [1632629,1641871) |
| interface.s0507.full | 1 | 192937 | [1644076,1837013) |
| interface.s0437.full | 1 | 115762 | [1839218,1954980) |
| interface.s0420.full | 1 | 7973 | [1957185,1965158) |
| interface.s0392.full | 1 | 8268 | [1967363,1975631) |
| interface.s268.full | 1 | 203220 | [1977836,2181056) |
| interface.s2125.loop | 1 | 352800 | [2183261,2536061) |
| interface.s2138.loop | 1 | 352800 | [2538266,2891066) |
| interface.s2152.loop | 1 | 352800 | [2893271,3246071) |
| interface.food-crunch.v1 | 1 | 11025 | [3248276,3259301) |
| interface.bloater-burst.v1 | 1 | 13230 | [3261506,3274736) |

### Exact action archive inventory (215 members)

```text
approval/coordinator-record.md
approval/creator-attestation.json
approval/presented-manifest.json
recipes/assemble.py
recipes/decode.py
recipes/promote.py
recipes/selection.json
review/README.txt
review/index.html
review/manifest.json
sources/2790/audioclip.bin
sources/2790/original.wav
sources/2790/provenance.json
sources/2790/stream.fsb
sources/2804/audioclip.bin
sources/2804/original.wav
sources/2804/provenance.json
sources/2804/stream.fsb
sources/2862/audioclip.bin
sources/2862/original.wav
sources/2862/provenance.json
sources/2862/stream.fsb
sources/2864/audioclip.bin
sources/2864/original.wav
sources/2864/provenance.json
sources/2864/stream.fsb
sources/2880/audioclip.bin
sources/2880/original.wav
sources/2880/provenance.json
sources/2880/stream.fsb
sources/3131/audioclip.bin
sources/3131/original.wav
sources/3131/provenance.json
sources/3131/stream.fsb
sources/3150/audioclip.bin
sources/3150/original.wav
sources/3150/provenance.json
sources/3150/stream.fsb
sources/3288/audioclip.bin
sources/3288/original.wav
sources/3288/provenance.json
sources/3288/stream.fsb
sources/3294/audioclip.bin
sources/3294/original.wav
sources/3294/provenance.json
sources/3294/stream.fsb
sources/3357/audioclip.bin
sources/3357/original.wav
sources/3357/provenance.json
sources/3357/stream.fsb
sources/3369/audioclip.bin
sources/3369/original.wav
sources/3369/provenance.json
sources/3369/stream.fsb
sources/3554/audioclip.bin
sources/3554/original.wav
sources/3554/provenance.json
sources/3554/stream.fsb
sources/3574/audioclip.bin
sources/3574/original.wav
sources/3574/provenance.json
sources/3574/stream.fsb
sources/3640/audioclip.bin
sources/3640/original.wav
sources/3640/provenance.json
sources/3640/stream.fsb
sources/3672/audioclip.bin
sources/3672/original.wav
sources/3672/provenance.json
sources/3672/stream.fsb
sources/3700/audioclip.bin
sources/3700/original.wav
sources/3700/provenance.json
sources/3700/stream.fsb
sources/3741/audioclip.bin
sources/3741/original.wav
sources/3741/provenance.json
sources/3741/stream.fsb
sources/3804/audioclip.bin
sources/3804/original.wav
sources/3804/provenance.json
sources/3804/stream.fsb
sources/3811/audioclip.bin
sources/3811/original.wav
sources/3811/provenance.json
sources/3811/stream.fsb
sources/3944/audioclip.bin
sources/3944/original.wav
sources/3944/provenance.json
sources/3944/stream.fsb
sources/3945/audioclip.bin
sources/3945/original.wav
sources/3945/provenance.json
sources/3945/stream.fsb
sources/4041/audioclip.bin
sources/4041/original.wav
sources/4041/provenance.json
sources/4041/stream.fsb
sources/4169/audioclip.bin
sources/4169/original.wav
sources/4169/provenance.json
sources/4169/stream.fsb
sources/4245/audioclip.bin
sources/4245/original.wav
sources/4245/provenance.json
sources/4245/stream.fsb
sources/4247/audioclip.bin
sources/4247/original.wav
sources/4247/provenance.json
sources/4247/stream.fsb
sources/4434/audioclip.bin
sources/4434/original.wav
sources/4434/provenance.json
sources/4434/stream.fsb
sources/4523/audioclip.bin
sources/4523/original.wav
sources/4523/provenance.json
sources/4523/stream.fsb
sources/4543/audioclip.bin
sources/4543/original.wav
sources/4543/provenance.json
sources/4543/stream.fsb
sources/4611/audioclip.bin
sources/4611/original.wav
sources/4611/provenance.json
sources/4611/stream.fsb
sources/4692/audioclip.bin
sources/4692/original.wav
sources/4692/provenance.json
sources/4692/stream.fsb
sources/4872/audioclip.bin
sources/4872/original.wav
sources/4872/provenance.json
sources/4872/stream.fsb
sources/4949/audioclip.bin
sources/4949/original.wav
sources/4949/provenance.json
sources/4949/stream.fsb
sources/5020/audioclip.bin
sources/5020/original.wav
sources/5020/provenance.json
sources/5020/stream.fsb
sources/5123/audioclip.bin
sources/5123/original.wav
sources/5123/provenance.json
sources/5123/stream.fsb
sources/5154/audioclip.bin
sources/5154/original.wav
sources/5154/provenance.json
sources/5154/stream.fsb
sources/5176/audioclip.bin
sources/5176/original.wav
sources/5176/provenance.json
sources/5176/stream.fsb
sources/5272/audioclip.bin
sources/5272/original.wav
sources/5272/provenance.json
sources/5272/stream.fsb
sources/5476/audioclip.bin
sources/5476/original.wav
sources/5476/provenance.json
sources/5476/stream.fsb
sources/5654/audioclip.bin
sources/5654/original.wav
sources/5654/provenance.json
sources/5654/stream.fsb
sources/5736/audioclip.bin
sources/5736/original.wav
sources/5736/provenance.json
sources/5736/stream.fsb
variants/action.2790.full.wav
variants/action.2804.full.wav
variants/action.2862.full.wav
variants/action.2864.full.wav
variants/action.2880.full.wav
variants/action.3131.full.wav
variants/action.3150.full.wav
variants/action.3288.full.wav
variants/action.3294.full.wav
variants/action.3294.jump-trim.wav
variants/action.3357.full.wav
variants/action.3369.full.wav
variants/action.3554.full.wav
variants/action.3574.break-trim.wav
variants/action.3574.full.wav
variants/action.3640.full.wav
variants/action.3672.full.wav
variants/action.3700.full.wav
variants/action.3741.full.wav
variants/action.3804.full.wav
variants/action.3811.full.wav
variants/action.3944.full.wav
variants/action.3945.full.wav
variants/action.4041.full.wav
variants/action.4169.full.wav
variants/action.4245.full.wav
variants/action.4247.full.wav
variants/action.4434.full.wav
variants/action.4523.break-trim.wav
variants/action.4523.full.wav
variants/action.4543.full.wav
variants/action.4611.full.wav
variants/action.4692.full.wav
variants/action.4872.full.wav
variants/action.4949.full.wav
variants/action.4949.jump-trim.wav
variants/action.5020.full.wav
variants/action.5123.full.wav
variants/action.5154.full.wav
variants/action.5176.full.wav
variants/action.5272.full.wav
variants/action.5476.full.wav
variants/action.5654.full.wav
variants/action.5736.full.wav
production/manifest.json
```

### Exact interface archive inventory (125 members)

```text
recipes/assemble-interface.py
recipes/decode-interface.py
recipes/selection.json
review/index.html
review/questions.txt
sources/r2860/audioclip.bin
sources/r2860/original.wav
sources/r2860/provenance.json
sources/r2860/stream.fsb
sources/r3195/audioclip.bin
sources/r3195/original.wav
sources/r3195/provenance.json
sources/r3195/stream.fsb
sources/r3254/audioclip.bin
sources/r3254/original.wav
sources/r3254/provenance.json
sources/r3254/stream.fsb
sources/r3558/audioclip.bin
sources/r3558/original.wav
sources/r3558/provenance.json
sources/r3558/stream.fsb
sources/r3755/audioclip.bin
sources/r3755/original.wav
sources/r3755/provenance.json
sources/r3755/stream.fsb
sources/r3863/audioclip.bin
sources/r3863/original.wav
sources/r3863/provenance.json
sources/r3863/stream.fsb
sources/r4364/audioclip.bin
sources/r4364/original.wav
sources/r4364/provenance.json
sources/r4364/stream.fsb
sources/r4441/audioclip.bin
sources/r4441/original.wav
sources/r4441/provenance.json
sources/r4441/stream.fsb
sources/r4864/audioclip.bin
sources/r4864/original.wav
sources/r4864/provenance.json
sources/r4864/stream.fsb
sources/r4973/audioclip.bin
sources/r4973/original.wav
sources/r4973/provenance.json
sources/r4973/stream.fsb
sources/r5582/audioclip.bin
sources/r5582/original.wav
sources/r5582/provenance.json
sources/r5582/stream.fsb
sources/r5586/audioclip.bin
sources/r5586/original.wav
sources/r5586/provenance.json
sources/r5586/stream.fsb
sources/s0392/audioclip.bin
sources/s0392/original.wav
sources/s0392/provenance.json
sources/s0392/stream.fsb
sources/s0420/audioclip.bin
sources/s0420/original.wav
sources/s0420/provenance.json
sources/s0420/stream.fsb
sources/s0437/audioclip.bin
sources/s0437/original.wav
sources/s0437/provenance.json
sources/s0437/stream.fsb
sources/s0438/audioclip.bin
sources/s0438/original.wav
sources/s0438/provenance.json
sources/s0438/stream.fsb
sources/s0449/audioclip.bin
sources/s0449/original.wav
sources/s0449/provenance.json
sources/s0449/stream.fsb
sources/s0485/audioclip.bin
sources/s0485/original.wav
sources/s0485/provenance.json
sources/s0485/stream.fsb
sources/s0507/audioclip.bin
sources/s0507/original.wav
sources/s0507/provenance.json
sources/s0507/stream.fsb
sources/s2125/audioclip.bin
sources/s2125/original.wav
sources/s2125/provenance.json
sources/s2125/stream.fsb
sources/s2138/audioclip.bin
sources/s2138/original.wav
sources/s2138/provenance.json
sources/s2138/stream.fsb
sources/s2152/audioclip.bin
sources/s2152/original.wav
sources/s2152/provenance.json
sources/s2152/stream.fsb
sources/s268/audioclip.bin
sources/s268/original.wav
sources/s268/provenance.json
sources/s268/stream.fsb
variants/interface.bloater-burst.v1.wav
variants/interface.food-crunch.v1.wav
variants/interface.handling.v1.wav
variants/interface.r2860.full.wav
variants/interface.r3195.full.wav
variants/interface.r3254.full.wav
variants/interface.r3558.loop.wav
variants/interface.r3755.full.wav
variants/interface.r3863.full.wav
variants/interface.r4364.full.wav
variants/interface.r4441.loop.wav
variants/interface.r4864.full.wav
variants/interface.r4973.full.wav
variants/interface.r5582.full.wav
variants/interface.r5586.full.wav
variants/interface.s0392.full.wav
variants/interface.s0420.full.wav
variants/interface.s0437.full.wav
variants/interface.s0438.full.wav
variants/interface.s0449.full.wav
variants/interface.s0485.full.wav
variants/interface.s0485.negative.wav
variants/interface.s0507.full.wav
variants/interface.s2125.loop.wav
variants/interface.s2138.loop.wav
variants/interface.s2152.loop.wav
variants/interface.s268.full.wav
production/manifest.json
```

Original action review metadata is historical; approved current metadata uses production/manifest.json. Member inventories retain exact sizes/hashes and exclude their own self-hash cycle. Final independent inventory/budgets/stub/threat/metrics records are `.phase4/04-21-final/artifact-inventory.json`, `budgets-terminal.json`, `stub-threat-scan.json` and `completion-metrics.json`.

## Self-Check: PASSED

All641 closeout checks passed:47actual current-source outcomes,26raw assembly-record identities,481frozen inputs,eight raw working/Git artifact identities,Task1approved andTask2fixed bytes,preserved checkpoints/budgets,required committed tracer and real completed authoring receipt. After writing this canonical file,summary-self-check.json separately verified its required frontmatter,eight files,two task ancestors,481unchanged inputs,all71sample rows and340retained member references. No missing required artifact,unexpected tracked deletion,source drift or applicable unperformed source/headless verification remains. These checks supply no new hearing/native-mix/full-phase acceptance credit.

