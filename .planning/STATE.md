---
gsd_state_version: "1.0"
milestone: v0.1
milestone_name: Complete playable LAN campaign
current_phase: 06
current_phase_name: Survivors, inventory, and combat
current_plan: 6
status: executing
stopped_at: "Plan 06-05 passed. Execution continues on 06-06."
last_updated: "2026-11-24T13:02:34Z"
last_activity: 2026-11-24
last_activity_desc: Plan 06-05 verified movement, carried load, safe recovery and food consumption
state_head: c158ac7fa3c2d63565c57db051581592e29c1f92
progress:
  total_phases: 12
  completed_phases: 5
  total_plans: 105
  completed_plans: 69
  percent: 42
---

# Project State

## Project Reference

See: [PROJECT.md](PROJECT.md) (updated 2026-11-14).
Scope: [REQUIREMENTS.md](REQUIREMENTS.md); implementation context: [research/SUMMARY.md](research/SUMMARY.md); delivery: [ROADMAP.md](ROADMAP.md).

**Core value:** Players can build, equip, and defend a shared survivor camp through a complete solo or four-player LAN campaign, with consistent ownership of their survivors, items, and progress.
**Phase 4 closure (November 22).** Phase 4 is complete under the creator's [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md). Its rendered cases, four named judgments and the engine-race follow-up move to Phase 5; named listening moves to Phase 9.

Phase 4 closes on the reviewed source `5c637da`: a clean independent review and all 776 selected pre-window results. See the [04-24 summary](phases/04-asset-production/04-24-SUMMARY.md).

## Current Position

Phase: 06 (Survivors, inventory, and combat) — EXECUTING
Current Plan: 6
Total Plans in Phase: 15
Status: Executing Phase 06
Last activity: 2026-11-24 — Plan 06-05 verified movement, carried load, safe recovery and food consumption

Progress: [████░░░░░░] 42%

Milestone progress: 5 of 12 phases, 69 of 105 planned plans complete with two Phase 4 plans waived, and 29 of 136 requirements complete.

## Performance Metrics

**Velocity:**
- Total plans completed: 64. Two Phase 4 plans were waived.
- Phase 1 recorded windows: 1352min, 75min average (rounded); historical per-plan scopes remain below.
- Phase 3 recorded plan windows: 944min total, 236min average. Plan 03-04's 833min includes reviews, failed attempts and five fresh full attempts; it is not the final selected-run duration.
- Final Phase 3 selected execution: 3,363,558ms / 56.06min, 481/481. No overall execution-time total is inferred for plans without recorded duration metadata.
- Final Phase 5 Windows selection: 11,748,289ms, 92/92 with all four recorded hub judgments.

**By Phase:**

| Phase | Plans | Total | Avg/Plan |
| --- | --- | --- | --- |
| 1 | 18 | 1352min | 75min |
| 2 | 3 | Partial duration metadata | - |
| 3 | 4 | 944min | 236min |
| 3 | 4 | - | - |

**Recent Trend:**
- Latest completed phase: 03-01 (39min), 03-02 (29min), 03-03 (43min), 03-04 (833min).
- Trend: Genuine failures drove bounded repair batches, focused acceptance and a fresh entire selection. The final full pass and image review retain their original execution identities; earlier failed runs remain preserved.

**Per-Plan Metrics:**

| Plan | Duration | Tasks | Files |
|------|----------|-------|-------|
| Phase 01 P01 | 9min | 2 tasks | 10 files |
| Phase 01 P02 | 21min | 2 tasks | 9 files |
| Phase 01 P03 | 17min | 2 tasks | 7 files |
| Phase 01 P04 | 17min | 2 tasks | 25 files |
| Phase 01 P18 | 21min | 2 tasks | 16 files |
| Phase 01 P05 | 25min | 2 tasks | 30 files |
| Phase 01 P06 | 44min | 2 tasks | 41 files |
| Phase 01 P07 | 47min | 2 tasks | 38 files |
| Phase 01 P08 | 289min | 2 tasks | 55 files |
| Phase 01 P09 | 158min | 2 tasks | 18 files |
| Phase 01 P10 | 62min | 2 tasks | 22 files |
| Phase 01 P11 | 68min | 2 tasks | 15 files |
| Phase 01 P12 | 154min | 3 tasks | 30 files |
| Phase 01 P13 | 69min | 2 tasks | 20 files |
| Phase 01 P14 | 101min | 2 tasks | 31 files |
| Phase 01 P15 | 11min | 2 tasks | 10 files |
| Phase 01 P16 | 84min | 2 tasks | 11 files |
| Phase 01 P17 | 155min | 2 tasks | 9 files |
| Phase 04 P01 | 10min | 2 tasks | 5 files |
| Phase 04 P25 | 12min | 2 tasks | 6 files |
| Phase 04 P34 | 20min | 2 tasks | 7 files |
| Phase 04 P35 | 13min | 2 tasks | 5 files |
| Phase 04 P02 | 157min | 2 tasks | 9 files |
| Phase 04 P36 | 12min | 2 tasks | 7 files |
| Phase 04 P37 | 99min | 2 tasks | 12 files |
| Phase 04 P03 | 20min | 2 tasks | 8 files |
| Phase 04 P26 | 130min | 2 tasks | 14 files |

## Accumulated Context

### Decisions

- [Phase 07 planning, 2026-11-18]: Camp economy and development is planned in 24 plans / 49 tasks / 18 waves (0–17). Research, pattern mapping and the revised independent [plan check](phases/07-camp-economy-and-development/07-PLAN-CHECK.md) are complete with no remaining findings. The [coverage ledger](phases/07-camp-economy-and-development/07-COVERAGE.md) assigns all 22 global requirements, 25 local requirements, 48 acceptance criteria, 168 edges and 18 decisions. Implementation remains blocked on actual Phase 5/6 delivery and the 07-01 contract/evidence audit; validation flags remain draft/pending. Phase 04 stays the active implementation/continuation on its existing branch and draft PR. This future-phase planning record grants no feature/test acceptance or phase advancement.

- [Phase 07 specification, 2026-11-18]: [07-SPEC.md](phases/07-camp-economy-and-development/07-SPEC.md) locks 25 local requirements for the same 22 Phase 7 project IDs, with 48 future acceptance checks, 168 explicit edge resolutions and ambiguity 0.026. Reviewed civilian danger remains at or within 12 u. Next preparatory step is discuss-phase 7; current Phase 04 implementation/continuation and all feature acceptance statuses remain unchanged.

- [Phase 07 specification, creator decisions 2026-11-17]: All civilian jobs may work in safe reachable exterior daylight territory; role changes require the NPC returned to the main building and cargo/obsolete gear deposit before new duties. Basic warriors guard the nearest completed wall/refuge, and shared paid orders dispatch automatically. Construction/rebuild/upgrade completion restores full health at original price/time. Repair uses one fixed 50%-valuation fee independent of missing HP to restore full health, covering further damage during the active order without extra charges; it closes at full health. Replacement builders preserve payment/progress regardless of buyer death/disconnection. Interim extra-charge/funding/cumulative-billing ideas are superseded. Phase 04 implementation and its continuation remain current; no feature acceptance or phase advancement is claimed.

Full project decisions are in [PROJECT.md](PROJECT.md); detailed Phase 1 implementation history and immutable report identities remain in its eighteen plan summaries.

- The approved v0.1 scope now contains 134 requirements and eleven sequential phases. RULE-01–RULE-07, BASE-01–BASE-03 and LAN-01–LAN-04 are complete; 120 requirements remain Pending.
- Phase 1 establishes the pinned Godot/C# five-project baseline, twelve inherited analyzers, bounded generated permissions, controlled state/snapshots and real native lifecycle/thread proofs. Phases 2–3 add the playable menu/walking scene and four-player LAN foundation.
- Accept Phase 1 through the creator-authorized Windows 676 baseline plus actual focused Linux strict/native/rule/state/graph evidence and one platform-name test correction checked on both OSes. No host restart occurred.
- Keep the complete Linux suite, original graph-case timeout and fourteen unreviewed human components as explicit follow-ups. Never convert them into passed cases or reviews.
- The latest 2026-11-15 local policy supersedes earlier other-OS VM requirements: routine phase closure uses development Windows at 1280×720 without a VM. Phase 5 retains hosted Windows/Linux compatibility, measured 90%/80% coverage and eight pending certification declarations.
- Phase implementation uses `feature/<task-id>-<short-description>` branches and draft PRs to `dev` at the first implementation push; the same PR becomes ready for review after phase completion and required verification. Follow the [phase branch and PR rules](../docs/workflow/ai-collaboration.md#phase-branches-and-pull-requests).
- [Phase 05]: Latest 2026-11-15 testing policy supersedes earlier VM and full-catalog phase-closure bullets: use development OS only, no VM, 1280x720, finish implementation and review/fixes before game windows. Select all active Critical cases plus every new/changed case and affected regression immediately. Save reusable scenarios with importance and ordered run groups; collect failures, batch repairs, check failures, then rerun the whole selected set. Phase 11 owns TEST-11 nightly full regression testing; hosted and distinct release evidence remains separate.
- [Phase 11]: TEST-11 adds nightly full regression testing after Phase 10; original requirement IDs/owners remain preserved.
- [Phase 03]: Independent verification passed 21/21 must-haves, all four roadmap criteria and LAN-01–LAN-04. Source/build remains `c69442148c975de974ebbb1d8ef409380940ecb0`; original full execution HEAD is `6ea600953be031bf2b467a50d35090a94e44f802`. Metadata closure must retain both the assembly revision and reviewed SourceLink target.
- [Phase 03]: Final full `841e10b1-f825-4527-865b-4703382f51eb` passed 481/481 in 56.06min: 433 Phase 1, sixteen Phase 2 and 32 Phase 3. All 48 physical timing rows meet 250ms (maximum 197.8586ms); all sixteen stopped comparisons agree with host. Current visual review accepts 319 bindings / 255 distinct contents. This is local Windows proof, with separate physical LAN, supported-display, hardware and endurance release owners.
- [Phase 04]: 04-01 provides common.bow source/headless tracer only; ASSET-01/02/05 remain pending complete census, portable export and actual 04-32/04-33 acceptance.
- [Phase 04]: 04-25 validates exact180 expectations and atomic partial-pack publication; one partial bow and zero completed obligations; all ASSET requirements and final native/listening/judgment acceptance remain pending 04-32/04-33.
- [Phase 04]: Publish validated partial content with explicit incomplete status; current bow contributes zero completed obligations.
- [Phase 04]: Retain immutable output identities and generations; replace only the validated current manifest pointer under an exclusive lock.
- [Phase 04]: 04-34 permits only six authenticated Windows SDK export path/hash/line tuples; original Debug/source/member permissions and IDs remain unchanged.
- [Phase 04]: 04-34 completes only the ASSET-05 managed export layout prerequisite; all ASSET requirements remain pending and preserved 04-02 Task1 resumes without repeating committed audio Task2.
- [Phase 04]: 04-35 permanently selects Inspection through asset_inspection export feature and qualified main; ordinary Foundation configuration remains exact.
- [Phase 04]: 04-35 source/headless configuration/resource proof alone supplies no packaged Inspection startup credit. Completed 04-02 subsequently supplies that actual native feasibility proof without repeating its committed audio task; all ASSET requirements remain pending.
- [Phase 04]: 04-02 proves actual natural packaged Inspection startup and local self-contained runtime closure through read-only native observations; complete content and rendered acceptance remain pending.
- [Phase 04]: 04-02 retains three exact decoded audio candidates with listening blocked, null listener and empty verdicts; AC-12 and all ASSET requirements remain pending 04-32/04-33.
- [Phase 04]: 04-36 discovers strict saved modules at CLI main; published expectations retain the independently pinned 180-row source authority and zero completion credit.
- [Phase 04]: Preserve original220/18/16/26 generated contracts; authenticate exact current observations in both directions and retain raw producer snapshots for historical regeneration.
- [Phase 04]: Only engine.generated-boundary receives validated120000ms case capacity with a122000ms outer deadline; all other defaults and original case identities remain unchanged.
- [Phase 04]: 04-37 completes generated proof only. All ASSET requirements stay pending, and04-03 resumes actual native hub/source-policy/visibility before later current-hub delivery and04-32/33 acceptance.
- [Phase 04]: 04-03 proves actual compiled CSharp native hub semantics and exact16-signature authored policy; rendering, physical input, clipboard round-trip, final packaging and allASSET acceptance remain separate pending work.
- [Phase 04]: 04-03 bounded visibility amendment appends only ContentPack and InspectionRoot to independent original15 public identities; graph-next04-26 owns future input/lifetime expansion.
- [Phase 04]: 04-26 proves source/headless owned input/transport/lifetimes and exact32 authored signatures with fresh complete29-operation proof plus tracer; all ASSET and04-32/33 acceptance remain pending.
- [Phase 04]: Retain exact reviewed per-case capacity: ac08 75000 and companion-negative60000 alongside existing boundary120000; positive/default45s,686 IDs and original220/18/16/26 contracts unchanged.
- [Phase 04]: Creator closed Phase 4 on 2026-11-22 on source/headless evidence: "this phase is only about assets ... We don't need to test real clicks and so on in this phase." ASSET-01–05 complete under the [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md); 04-32/04-33 waived and not executed. Phase 5 owns the 18 rendered Phase 4 IDs, 21 inherited rendered regressions, judgments P1–P4, the driver declaration and the world.replacement engine-race follow-up; Phase 9 owns the 95 listening verdicts.
- [Phase 06]: Creator defers all stash/inventory filtering to Phase 12 (INV-14); no filtering UI or acceptance in Phases 6-11. INV-10/R12/AC-18/AC-33 retain pages, navigation and transfers without filtering. This supersedes older filtering delivery wording and older eleven-phase/134-requirement totals; current scope is 12 phases and 135 requirements.

- [Phase 04]: 04-04 retains725 sprites/35 clips/2434 ZIP members and source-side density1; conversion/scale/discovery pass, final appearance/listening pending. WINDOWS29 tracks inherited current-hub tracer/index binding for04-23/31 before review.

- [Phase 04]: 04-05 retains24 identities/20 generated originals/88 art parts/166 component inputs and185 ZIP members. Two new scriptless preview scenes raise actual phase inventory234; all-zoom native reachability repaired and final item/discovery gates pass. Full-hub/appearance acceptance remains pending.

- [Phase 04]: 04-11 delivers three main tiers/47 parts/153 retained members; shared stash,8/12/16 seats and two independent single-guard attachments. Final strict production and harness0warnings/errors,17 compiled tests/260 native observations pass. Original failures and consumed repair budgets remain preserved. Inventory234 unchanged; actual appearance/full-hub acceptance remains pending.

- [Phase 04]: 04-06 commits2c54e53/531573e and SUMMARY96979df retain3 bodies/48 poses/193 parts/232 source members; complete fits2976 tuples/12 H13/5972 observations/10 compiled0skip with strict production/harness0warnings. Fresh broader selection11PASS/2FAIL is preserved. WINDOWS30/31 remain required04-23 integration; no policy pass or ASSET acceptance. Inventory234 unchanged.

- [Phase 04]: 04-14 fonts/frame kit and native theme committeded5c404/a05fff3, SUMMARYa889513;5-case fresh selection passes583.901s with2462 unchanged inputs. Native172 includes original165 IDs/12 logical canvas-scale-text combinations; strict production/harness0warnings,3compiled0skip,original hub25/50native+63compiled each. Inventory234 unchanged,actualappearance pending.

- [Phase 04]: 04-18 six editable baseline documents/984fields committed02ae884/7e3b7e5,SUMMARYc25f4be. Fresh foundation390fc7b3/items4d5d4ed7/discoverycd5a840d pass:384directsource assertions/52data negatives/5legal loadouts,35discovery negatives. Nine accepted hashes frozen; no gameplay/native permission claim. Inventory234 unchanged; wave12 complete.

- [Phase 04]: 04-07 adds5 distinct neutral bodies,80poses,320parts and418 retained b/c members. All8 choices/7936tuples/16158native/12H13 pass;12-case final local selection passes383 unchanged inputs. Shared helper Image ownership fixes confirmed by scoped treatment/reversal; strict report-only2MiB/125k reader retains14 negative controls and global100k limit. Source5eb6b7f/5d0f5e8,SUMMARYbbfc57e. Current policy291operations remains OPEN23-owned;inventory234 unchanged.

- [Phase04]: 04-28 resources/production projector committedaa0bf15/e2dea70/be65d18,SUMMARYb7186e7. Final11casePASS404unchanged inputs; identity127/integrity402 exact4shape vectors/native theme172+3compiled0skip; originalhub63compiled each/native25/50. Two new source/scene paths raise inventory234→236. Separate native-policyFAIL369 and unenrolled49input producer remain23-owned. Current icon.svg replacement/export embedding remains31; allactualacceptance pending.

- [Phase04]: 04-08 Task 1 bc7883d, Task 2 9da8f88 and canonical SUMMARY 4d1c1c1 complete. Final 16/16 PASS, 399 unchanged inputs; both animation cases retain 704 samples, five strict negatives, 41 replacement/control checks, seven UI states and 35 compiled passes with zero skips. Original body/hub/item/data/theme/identity regressions pass. Nine existing/planned paths leave inventory 236. Native policy c7f92490 remains FAIL 414 and producer 49 inputs/32 surfaces remains unenrolled for 23/31. All actual acceptance remains pending.
- [Phase 06]: Creator amendment 2026-11-17: Respawn enables one second after death; an explicit button request assigns an available eligible survivor or joins the waiting queue. Pending requests complete automatically in request order, with player-slot ties; no automatic transfer without requesting. Spectating follows living teammates using Previous/Next and a teammate list. Camera smooth nearby/cuts distant; survivor state and possessions stay unchanged.

- [Phase04]: 04-09 Task 1 dbf792f/642affe/60da583, Task 2 d88cdd2/caf830d and canonical SUMMARY a64c69f complete. Final 18/18 PASS, 405 unchanged inputs; bow 39 compiled/128 sustained cycles, work 240 rows/40 controls/71 Reset checks, source 56 members/17 components/16 actions. Original body/hub/item/data/theme/identity controls pass; scalar captured origin preserves 10 pure reflection tests without GodotSharp. Ten amended paths leave inventory 236. Native policy dcdc37a2 remains FAIL 604 and producer 49 inputs/32 surfaces remains unenrolled for 23/31. All actual acceptance remains pending.

- [Phase04]: 04-10 Task 1 dff9ec7/ac75206/6796377, Task 2 b594303/d2392ff and canonical SUMMARY 92017af complete. Final 20/20 PASS, 414 unchanged inputs; actual 600 owners, 2066 original crowd checks/166 wildlife samples/2189 corpse checks plus 80 definition controls. Twelve amended paths leave inventory 236; public identity authority is exactly 19. Native policy 9aef8ff9 remains FAIL 873 and evaluated producer 53 inputs/32 surfaces remains unenrolled for 23/31. Foreign 4127fd5/ca4b82b planning commits and their deleted checkpoint remain preserved. All actual acceptance remains pending.

- [Phase04]: Completed [04-12](phases/04-asset-production/04-12-SUMMARY.md): twenty-one of 37 local plans; wave 17 remains in progress with 04-15 next. Art02fe08e and Task2 5d593db/9a92af2, SUMMARY1a12a5a, supply six wall/tower tiers,46 lifecycle parts,nine footprint frames,124 retained ZIP members and three generated masters. Structural guards use11 parts/five split poses with independent1/2/3 sockets,22 samples,quiet previous-tier restoration and separate collision/work/release owners. Fresh27/27 selected cases pass on421 unchanged inputs in1,225,385ms;935 defense outcomes retain all929 original IDs/order plus six pixel controls, while original main17compiled/260native pass. Independent diagnosis proves exact alpha/visible RGB; only alpha-zero RGB differs on import, with exact raw source hashes retained. Original Task2 3/3 budget remains exhausted; R1 used1/2 corrective attempts. Eight planned paths leave inventory236; no public identity added. Latest policy e7b45d2f remains FAIL1006 operations (+133 Building),53 evaluated producer inputs/32 surfaces unenrolled. WINDOWS29/30/31,all ASSET and actual acceptance remain pending.

- [Phase04]: Completed04-15 source e7ccc86/9832318/54f7d77/14968cc and SUMMARYb9e65c5;33/33 executable cases on430 unchanged inputs,with two blocked Critical integration cases preserved separately. Inventory236,policy1189/producer54-input32-surface pending23/31. Wave17 complete,next13/wave18. All actual acceptance remains pending.

- [Phase04]: Completed04-13 source5c35024/1295455/726e6b6 and SUMMARY5572cd0. Fresh35/35 executable groups pass on439 unchanged inputs in1403558ms;two selected Critical provenance/publication cases remain blocked separately,with no full-phase pass. Map796/23,effects1393/31;WorldPreview adds239 observed ungranted operations,total1428;producer55inputs32surfaces pending23/31. Inventory236;wave18 complete,next27/wave19. All actual acceptance pending.

- [Phase04]: Completed04-27 Task1 5aa0a1e/tracer9c74a702,actual REDa2eb2b7,Task2/root447d0bd and SUMMARY94edbca. Final37/37 executable groups pass on441 unchanged inputs in1411546ms;two Critical integration cases remain blocked separately. Inspector531/replacement578+10compiled preserveoriginal IDs. Fresh bounded runtime reads/successful-only publication and merged pack/read diagnostics restore four unchanged regressions. Inventory236;policy1570/producer55-input32-surface pending23/31. Wave19 complete,next16then19/wave20;all actual acceptance pending.

- [Phase04]: Completed04-16 Task1 GREEN479cdc0/tracer3f74c966,Task2 genuineRED3a5f986/GREEN1f6a3f5,SUMMARY442915a. Final39/39 executable cases pass on448 unchanged inputs in1484920ms;two selected Critical integration cases remain blocked separately. Camp726/map2441native plus7compiled0skip. Exact authenticated mapnodes512 allowance retains generic256/authored128;deferred generation-fenced signals cleanly retire controls. Inventory236;policy1861/producer56inputs32surfaces pending23/31. Wave20 remains1/2 complete,next19;actual acceptance pending.

- [Phase04]: Completed04-19 Task1 015ee55/fresh committed tracer09592eb9,Task2 9f4f44e,SUMMARYffbe8d2. Final41/41 executable cases pass at ac9e172 on457 unchanged inputs in1502900ms;two selected Critical integration cases remain blocked separately. Delivered1764field IDs/eight paths;economy369fields324controls,world1395fields1089controls+five relationships. Eighteen old/current comparisons remain nonpassing23-owned. Foreign Phase07commit caused preserved2-pass scheduling interruption,then complete fresh-r1 passed. Inventory236;policy1861/producer56inputs32surfaces unchanged. Wave20 complete,next17wave21;actual acceptance pending.

- [Phase04]: Completed04-17 genuineREDs ec6102e/914676b/085c81f,Task1 GREEN2d7aa9f/fresh committed tracer1b0b110d,Task2 GREEN5b7978a,SUMMARY47ed5a9. Final43/43 executable cases pass at5b7978a on464 unchanged inputs in1517750ms;two selected Critical integration cases remain blocked separately. Session699/endpoints1708native plus4compiled0skip;944selfchecks pass. Original budgets and R1/R2/R3 checkpoints remain preserved. Inventory236;policy2130/producer57inputs32surfaces pending23/31. Wave21 complete,next20wave22;actual acceptance pending.

- [Phase04]: Completed04-20 original767490c/8f0eddc,checkpointd42bf89,prospectiveca6613d,repairs745ea75/ed8a79c,SUMMARY5c7a47b. Repaired committed tracerfe9bcd80 precedesR1.2. Fresh45/45 cases ated8a79c on473 unchanged inputs pass in1588583ms;633selfchecks and319exactold-fail/new-pass controls pass. All1855IDs,1515originalcontrolnames and80legalfixtures preserved. Old45diagnostic at8f0eddc remains nonacceptinghistory. Existing49comparisons remain23-owned. Inventory236;recorded2130policy/current57inputs32surfaces unchanged. Wave22 complete,next21wave23;actualacceptance pending.

- [Phase04]: Historical04-21 pre-approval checkpoint was incomplete at source318f288 after preservedab6826b. Forty donor sources/44candidate variants coverAU01-15 in56.655828seconds;all remain Not auditioned. Fresh committed audio.actions9a5b2af8-40af-4047-a676-f46a9755eb91 passes exact PCM/source/bundle reproduction,13negative controls and12restored positives,then fails onlyAUDIO_LISTENING_REQUIRED. Review: [source-only listening checkpoint](phases/04-asset-production/04-21-LISTENING-CHECKPOINT.md),with exact portableZIP and local prepared page. OriginalTask1 corrections3/3 preserved,no technical error remains;noGREEN/tracer/Task2/canonicalSUMMARY or category/ASSET acceptance. Wave23 remains incomplete;local28/37/global53/62 stay fixed. Real listener source/version/device/verdict is required;finalnative mixing/actual32/33 remain later. Inventory236,49existingconsumer comparisons,WINDOWS29/30/31,policy2130/current57-input32-surface producer and index0/180 remain pending.

- [Phase04]: Completed04-21 Task1GREEN3df8209/freshtraceraa09e941,Task2GREEN54059b7,SUMMARY8dae14a. Creatorverbatimapproval for40sources/44versions retained with null unspecifieddetails;27newversions remainNotauditioned. Fresh47/47cases at54059b7 on481 unchangedinputs pass in1628532ms(case)/1639745mswall;641closeoutchecks andsummaryselfcheck pass. Originalbudgets3/3+3/3,R1initial0/2,R2initial0/2 preserved; exactTask2publication nowpreservesexistingEVENTS/ZIP identities. Inventory236;wave23complete,next29wave24. RetainedZIP64MiB localexception requiresexact23/31consumer repair;49existingcomparisons/policy2130/current57inputs32surfaces/index/actualacceptance remainpending.

- [Phase04]: Completed04-29 originalTask1GREEN4f0583a/freshcommittedtracer716d7c95,originalTask2GREENf72e3be,partialstoprepair8d318f4,Rootstatusrepair ecc20cd,SUMMARY7dc1f83. Fresh49/49executablecases at ecc20cd on484unchangedinputs pass in1749413mscase/1769167mswall;687/807native+22compiled0skip,strict0/0. All oldbudgets,stopped20,failed47/2 and unattributedhistorical4ObjectDBwarnings preserved. R5initial+C1/2,R6initial0/2. Exactlyonefinalpolicy db114983 FAIL2259=historical2130+129Audio tuples; currentproducer58inputs32surfaces unenrolled. Wave24complete,30/37local,next22wave25;inventory236 and sourceintegration/49consumercomparisons/actualacceptance remainpending.

- [Phase04]: Completed04-22 Task1f7f735e/fresh committed tracer207a22ce,Task2 tested727faf5,SUMMARYb1bcae9. Fresh51/51 executable cases at727faf5 pass on492 unchanged inputs in1823596ms case/1844575ms wall;363messages,3615bindings,4603fields,82controller+11Hub outcomes and16backstops. OriginalTask2=3/3,R1.1initial0/2,R1.2=2/2,R2initial0/2,R3.1C1/2,R3.2initial0/2,R3.3C1/2 preserved with actual failed native evidence. Single finalpolicy749fcabe FAIL2349 adds90DataText tuples; currentproducer59inputs32surfaces unenrolled. Wave25complete,31/37local,next23wave26;inventory236 and all source/native/consumer/actualacceptance obligations remain pending.

- [Phase04 partial checkpoint,2026-11-19]: All180 source-ready/native-reached+4worldviews, currentpolicy2654/251, authenticated57input32emission producer, freshgovernance28 and current-binary358 pass. CURRENT67:67assertionPASS/66clean-qualified/1animationimportUnicodeFAIL.69localimplementationfiles copied immutably, uncommitted; formal04-23 remains0/2 andPhase04 remains31/37. No budget/debug reset, acceptance or phase advance.
- [Phase 08]: Context gathered on 2026-11-21 and ready for planning. See [08-CONTEXT.md](phases/08-zombie-campaign-and-outcomes/08-CONTEXT.md) and the [amended specification](phases/08-zombie-campaign-and-outcomes/08-SPEC.md). — Capture the creator-approved campaign interfaces and immediate retaliation without interrupting the current implementation session.
- [Phase 05]: Approved shared test preparation, focused checks and automatic scratch cleanup after durable evidence is saved. Exact builds are kept only for diagnosis or handoff. See [05-CONTEXT.md](phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md). — Reduce the disk footprint and repeated setup measured in Phase 4 while retaining required verification.
- [Phase 05, 2026-11-23]: The creator approved the test-scope amendment. Phase 5 is reduced to a shared runner and catalog, three tiers, basic PR checks and the existing solo/LAN scenarios. Coverage certification with the eight IDs, full hosted runs, evidence reuse and exhaustive runner trials move to Phase 11 under the new TEST-12, and five cheaper testing rules apply to Phases 6–10. See the amended [05-SPEC.md](phases/05-ci-and-repeatable-game-testing/05-SPEC.md) and [05-CONTEXT.md](phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md) D-35–D-46. Plan Phase 5 from the amended specification and context.
- [Phase 05, 2026-11-23]: After PR #6 moved the tools, the creator moved the renewal of the Phase 4 review-bound gates (`runner.prelaunch`, `runner.selection`, `acceptance.judgments`, `acceptance.artifacts`, `--verify-completion`) to Phase 11 under TEST-12. They stay expected-blocked without pass credit until then. Phase 5 still makes the Phase 4 checks it runs work in the relocated tree, including the recipes that assume the original `.phase4` folder. See [05-CONTEXT.md](phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md) D-46.

### Pending Todos

No remaining Phase 1 closure gate under the current creator exception. Follow-up work is recorded in the [closure register](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md); it remains unperformed or unaccepted. The old restart checkpoint is historical and is not the next action.

Phase 3 has no blocking acceptance item. Its independent [UI audit](phases/03-four-player-lan-foundation/03-UI-REVIEW.md) retains eight advisory warnings (19/24, zero blockers), including focus styling, roster typography and full-text disclosure. No exact UI-contract compliance is claimed.

### Roadmap Evolution

- 2026-11-14 — Creator approved Phase 1 simplification. Revised remaining plans 01-14–17 and Phase 1/5 acceptance boundaries. Coverage targets remain 90% lines / 80% branches; measured coverage, exhaustive platform/report reconciliation and unfinished aggregate certification have explicit Phase 5 ownership. Ten phases, 133 requirement IDs, primary owners and all working enforcement/tests are preserved. This is a plan change, not a passed implementation or platform check.
- Phase 02 changed: Phase 5 discussion amended verification: basic build/repository-only PRs target about ten minutes per OS; dependency-aware focused groups and validated unchanged-group reuse replace mandatory full closure reruns. Complete case/environment evidence, eight handoff IDs and 90/80 coverage targets remain required. See phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md and amended 05-SPEC.md.
- Phase 11 added: Nightly full regression testing (TEST-11) after Phase 10; immediate reduced local testing and saved scenario/run-group metadata are not deferred.
- Phase 05 edited: Creator local testing amendment: no VM, development OS/1280x720 only after whole implementation and review; Critical/new/changed/affected selection now; grouped repair checks followed by the full selected-set pass.
- Phase 12 added: Stash and inventory filtering deferred from Phase 6 by creator on 2026-11-16; INV-14 owns filtering, while INV-10 retains pages/access/transfers. Current scope: 135 requirements across 12 phases, 14 complete and 121 pending.
- Phase 05 and Phase 11 edited: Creator-approved test-scope amendment on 2026-11-23. Phase 5 delivers one shared runner and catalog, three tiers, basic PR checks and the existing solo/LAN scenarios. Measured coverage with the eight transferred IDs, full hosted Windows/Linux runs, evidence reuse validation, exhaustive runner trials, the other 17 Phase 4 rendered cases and the Phase 1 follow-ups move to Phase 11 under the new TEST-12. Five cheaper local testing rules apply to Phases 6–10. Current scope: 136 requirements across 12 phases, 19 complete and 117 pending. The Phase 7 plans need a revision pass before execution.

### Blockers/Concerns

- [Cleanup 2026-11-22] The Phase 1-4 ignored artifacts were deleted (quick task 261122-ub4). Until their inputs are tracked or regenerated, `export.profile`, `export.layout`, the `ContentNativePermissionTests` model tests and Phase 2 `--verify-completion` fail. The Phase 1 catalog also needs rows for the PR 7 files. The creator declared those Phase 4 inputs (the checkpoint manifest, the current model and the three JSON reports) lost on 2026-11-23; the checks stay blocked under the owners the roadmap assigns. See the [task summary](quick/261122-ub4-prepare-for-merge-pr-7/261122-ub4-SUMMARY.md). Phase 5 regenerated the exact current sources for both Windows SDK export-layout rule proofs.
- [PR #6 relocation] Phase 5 repaired its selected relocated asset checks, including the retained export tracer and audio recipes. The historical Phase 4 review-bound gates retain their existing evidence and are renewed in Phase 11 under TEST-12. [Relocation summary](quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-SUMMARY.md)
- [Phase 1 follow-ups] Full Linux execution and the original graph case remain unaccepted; fourteen individual human reviews remain Pending. See the [closure register](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md) for actual evidence limits and timing. These are disclosed follow-ups under the creator exception, not current closure gates.
- [Local VM] The guest is cleanly stopped and preserved. Hardware acceleration is not active in the current Windows session; a creator-chosen restart is needed before attempting the accelerated follow-up. No restart is required for this completed run. After the `.local/` folder change merges, the migration deletes the legacy VM folder, and the phase that next needs a VM recreates it under the shared `.local/tools/linux-vm/`.
- [Phase 11] Measured analyzer/production coverage, advanced reconciliation and eight certification cases remain pending at the unchanged inclusive 90% line / 80% branch targets. The 2026-11-23 amendment moved them from Phase 5 to Phase 11 under TEST-12. The executable catalog, register and rule matrix now assign the eight pending declarations to Phase 11. They are excluded by `--phase 5` and included by `--phase 11`.
- [Process history] WINDOWS item 1 retains the missing original bootstrap RED record; later successful evidence does not recreate it. All original plan reports and failed attempts remain historical records. One-shot refactoring/binding utilities must not be reapplied.
- [Release] Reference-PC hardware and required real mixed-OS LAN/endurance environments need actual recording and measurements before release acceptance.
- [Phase 3 diagnostics] The completion CLI reports six supposed missing file references in 03-01-SUMMARY: four are executable command strings, and two are phase-relative `tdd/03-01-task*-red.json` paths. Both actual phase-local RED files exist. Independent verification checked the real artifacts and source links; the generic summary-path diagnostic remains disclosed without changing immutable summary metadata.

### Quick Tasks Completed

| # | Description | Date | Commit | Status | Directory |
| --- | --- | --- | --- | --- | --- |
| 261114-we2 | Require phase feature branches and draft-to-ready PRs targeting dev | 2026-11-14 | 9a96240 | — | [261114-we2-define-phase-feature-branches-and-draft-](./quick/261114-we2-define-phase-feature-branches-and-draft-/261114-we2-SUMMARY.md) |
| 261116-k3u | Consolidate the game workspace; explain tooling and add the PR review entry point | 2026-11-16 | de5f40f | Verified | [261116-k3u-consolidate-the-game-workspace-and-docum](quick/261116-k3u-consolidate-the-game-workspace-and-docum/261116-k3u-SUMMARY.md) |
| 261116-m0w | Move all game-configured tools into purpose-based workspace folders | 2026-11-16 | 4ea92b5 | Verified | [261116-m0w-organize-game-specific-tools-by-purpose-](./quick/261116-m0w-organize-game-specific-tools-by-purpose-/261116-m0w-SUMMARY.md) |
| 261119-15c | Require task-end cleanup of ignored outputs and worktrees, with a dry-run-first cleanup command | 2026-11-20 | 855b177 | — | [261119-15c-agent-cleanup-rules](./quick/261119-15c-agent-cleanup-rules/261119-15c-SUMMARY.md) |
| 261120-09v | Fix intermittent Godot editor-import Unicode parsing errors (cwd race) | 2026-11-20 | 87c0b80 | — | [261120-09v-fix-intermittent-godot-editor-import-uni](./quick/261120-09v-fix-intermittent-godot-editor-import-uni/261120-09v-SUMMARY.md) |
| 261121-jmu | Refresh repository layout PR for Phase 4 | 2026-11-21 | 8832a16 | Verified | [261121-jmu-refresh-repository-layout-pr-for-phase-4](./quick/261121-jmu-refresh-repository-layout-pr-for-phase-4/261121-jmu-SUMMARY.md) |
| 261122-ub4 | Prepare PR 7 for merge: merge dev, fix cleanup data-loss paths, align docs; delete Phase 1-4 artifacts | 2026-11-22 | 6421770 | — | [261122-ub4-prepare-for-merge-pr-7](./quick/261122-ub4-prepare-for-merge-pr-7/261122-ub4-SUMMARY.md) |
| 261123-fsu | Resolve PR #6 conflicts: merge dev (Phase 4 and PR #7) into the relocated layout and rebind what the move breaks | 2026-11-23 | e7ec53d | Verified | [261123-fsu-resolve-conflict-https-github-com-sebrei](./quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-SUMMARY.md) |
| 261123-ltj | Replace the ignored `.phaseN` folders with one `.local/` folder, share Node 24 and NuGet per repository, and fix the PR #11 contradictions | 2026-11-23 | cfbbb5f | Verified | [261123-ltj-replace-phasen-folders-with-one-ignored-](./quick/261123-ltj-replace-phasen-folders-with-one-ignored-/261123-ltj-SUMMARY.md) |

## Deferred Items

The [Phase 5 handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) retains measured analyzer/production coverage, advanced reconciliation and eight aggregate/coverage cases. The current manifest contains 686 declarations: 676 Phase 1 cases, two Phase 4 Windows export-layout proofs and eight separately pending Phase 11 cases. The original 684-declaration baseline and its Windows 676/focused both-OS evidence remain immutable and distinct from these new Phase 4 proofs and the limited composed Linux evidence. Under the 2026-11-23 amendment Phase 11 owns that handoff through TEST-12; the manifest now assigns the eight declarations to Phase 11 under AC-45.

Phase 1 is closed under its explicit [exception](phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md). Its [follow-ups](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md) remain unperformed/unaccepted. Current routine local verification follows the development-OS-only, 1280×720, no-VM policy. Preserve immutable reports and distinct identities for changed code and metadata.

| Category | Item | Status | Deferred At | Milestone |
| --- | --- | --- | --- | --- |
| Certification (TEST-12) | Measured 90%/80% coverage, advanced platform reconciliation and eight pending aggregate/coverage cases | Required; implementation/acceptance pending | Creator-approved 2026-11-14 amendments; moved from Phase 5 by the creator-approved 2026-11-23 amendment | v0.1 Phase 11 |
| Phase 1 closure follow-ups | Full Linux suite, original graph-case capacity and fourteen individual human reviews | Unperformed/unaccepted; not passed | Creator no-restart exception, 2026-11-14 | v0.1; see closure register |
| Future platform work | PLAT-01 and PLAT-02 Steam/Epic implementations | Outside v0.1; no next-release commitment | Initial scope | v0.1 |
| Backlog 999.1 | Rename the phase-named identifiers (`Phase1.cs`/`Phase1.tscn`, `KingdomZombies.Phase1.Tests`, `PHASE1_*`-style variables, `phase1`–`phase3` case families) | Captured; not scheduled | Quick task 261123-ltj, 2026-11-23 | [ROADMAP backlog](ROADMAP.md#backlog) |

## Session Continuity

Last session: 2026-11-24T13:02:34Z
Stopped at: [Plan 06-05](phases/06-survivors-inventory-and-combat/06-05-SUMMARY.md) passed on `b7b0f04`.
Resume file: [06-06-PLAN.md](phases/06-survivors-inventory-and-combat/06-06-PLAN.md)

Phase 4 closed on the reviewed source `5c637da` (bound in `1e0fe27`, gates passing). The [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md) records what was not performed and its new owners.

[Phase 5 pull request #15](https://github.com/Sebreiro/kingdom-zombies-game/pull/15) merged into `dev` as `b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6`. Phase 6 work continues on `feature/06-survivors-inventory-and-combat` in `D:/GitWorktrees/KingdomZombies/kz-06-survivors-inventory-and-combat`.

Phase 6 implementation is published in [draft pull request #16](https://github.com/Sebreiro/kingdom-zombies-game/pull/16), targeting `dev`. Its first implementation commit is `6bd6750a1fc923bd15634574aee5e4bb1aa28803`.

Phase 6 baseline on Windows used the pinned Node and `node src/godot-game/tools/game-tests/check.cjs --pr` from the linked checkout. Invocation `87cbb73a-e93f-412c-b053-fa41b043f8df` exited 0 on `b9aaa2b951e6f5dc128a17c7d58e0d17ffe10cb6` with four passes, zero failures and zero blocked cases. Strict build, repository validation, pure tests and headless solo gameplay passed.

Repository layout continuation: [PR #6](https://github.com/Sebreiro/kingdom-zombies-game/pull/6) (`feature/quick-repository-layout`, quick tasks 261116-k3u, 261116-m0w and 261121-jmu) integrated `dev` (Phase 4 and PR #7) by quick task 261123-fsu, with every Phase 4 and PR #7 file at its [current layout location](../docs/architecture/repository-layout.md#historical-paths).
