---
phase: 04-asset-production
plan: "16"
subsystem: ui
tags: [godot, csharp, camp, map, spectator, warnings, strict-json, source-headless]
requires:
  - phase: 04-15
    provides: "Native inventory/target layouts and shared authenticated UI dependencies"
  - phase: 04-27
    provides: "World inspector, always-fresh bounded reads, validation-only preload cache and merged diagnostics"
provides:
  - "31 camp states across distinct people, stash-sale, site-work and rally compositions"
  - "35 map/spectator/warning states, independent H23 reordering and seven fixture-preview backstops"
  - "Exact authenticated map native.nodes512 allowance with strict generic256 and authored128 limits"
affects: [04-19, 04-23, 04-24, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [exact-hash-bound-json, guarded-native-owners, generation-fenced-deferred-signals]
key-files:
  created:
    - src/godot-game/inspection/CampLayouts.cs
    - src/godot-game/content/ui/camp-panels.json
    - src/godot-game/content/packs/ui-camp.json
    - tests/phase1/CampLayoutTests.cs
    - tools/phase4/cases/ui-camp.cjs
    - src/godot-game/content/ui/map-spectator-warnings.json
    - src/godot-game/content/packs/ui-map-warnings.json
  modified: []
key-decisions:
  - "Authenticate every exact binding before allowing512 only at map.json $.native.nodes; generic256/authored128 remain strict."
  - "Defer native state transitions and reject retired generations before touching the tree."
  - "Retain full muted warning text in an independent CanvasLayer; identity uses canonical number/name/shape/color."
  - "Keep fixed50% repair fees, paid additional-damage coverage and replacement-worker payment/progress continuity."
requirements-completed: []
plan-requirements: [ASSET-02, ASSET-04]
coverage:
  - id: CAMP-PARSER
    description: "Authenticated dependency exception with independent strict boundary and wrong-hash controls"
    verification:
      - kind: integration
        ref: "layouts.camp/32e7b3b3-a011-49f4-950a-40f07d8695fc"
        status: pass
    human_judgment: false
  - id: CAMP-LAYOUTS
    description: "People, sale, site and rally reusable native resources and inspection states"
    verification:
      - kind: automated_ui
        ref: "layouts.camp/32e7b3b3-a011-49f4-950a-40f07d8695fc"
        status: pass
    human_judgment: true
    rationale: "Actual input, readability, appearance and named judgment remain separately gated04-32/33."
  - id: MAP-WARNINGS
    description: "Shared map, canonical identities, spectator states, warning layers, H23 and seven fixture backstops"
    verification:
      - kind: automated_ui
        ref: "layouts.map-warnings/5f60b031-21fd-4a0a-bec3-bcaf4d84ebdf"
        status: pass
    human_judgment: true
    rationale: "Source/headless observations do not certify actual rendered/input/appearance acceptance."
actuals:
  tokens: 53217
  tasks: 2
  commits: 6
plan_head_before: 37e2473e5570923ba6940831dddf31581f210a99
plan_head_after: 1f6a3f58d97601c4965fab5cf4d1cb0a1e67db40
duration: 63min
completed: 2026-11-17
status: complete
---

# Phase 04 Plan 16: Camp, map, succession and warning layouts Summary

**Distinct native camp layouts, shared explored maps, canonical player identities and persistent muted warnings with authenticated parser and lifetime controls.**

## Completion scope

Both original tasks are locally complete. The fresh executable source/headless selection passes39/39 on448 unchanged frozen inputs, zero drift, in1484920ms. This includes the previous37conservative executable groups plus both new cases. Two active Critical integration cases remain selected-but-blocked with original provenance; no complete phase selected-set or ASSET acceptance is claimed. Every result here is freshly executed; no prior pass is reused as a current invocation.

All ASSET requirements remain pending. Inventory236,census180,24AC,32decisions,104H/UI,current-index0/180 remain unchanged. UI-16–21/UI-26 resources declare sampleDataOnly and no gameplayTransactions; completedObligations remain empty and visualReview remains null because actual04-32/33 acceptance has not occurred. These are intentional evidence boundaries, not missing resource implementations.

## Performance and measured actuals

- First preserved original behavioral RED run started2026-11-17T22:07:59.4682740Z; final selection finished2026-11-17T23:10:17.432Z. Duration63min includes original task/repair history through the selected-set run, and excludes later metadata work.
- Fresh39-case executable duration1484920ms (24.75min). Per-case starts/ends/durations, invocation HEAD and actual production closures reside in.phase4/04-16-final/results.json. Compatible groups are ordered explicitly below; each no-window native fixture retains its required owned isolated process rather than inventing a shared game launch.
- Realized seven-source-path Git diff212867characters/4 =53217estimate-scale tokens; measured ledger interval37e2473e5570923ba6940831dddf31581f210a99..1f6a3f58d97601c4965fab5cf4d1cb0a1e67db40 contains6commits. No harness-token count or narrated commit count is substituted. This snapshot excludes the subsequent SUMMARY metadata commit.
- All actual commands ran on Windows. No real game window, VM, physical input, listening, named appearance/readability approval, FPS/LAN/endurance or other-OS evidence was produced.

## Accomplishments

People, sale, site and rally use four separate native trees (PeopleColumns/Roster+Jobs, StashLedger+Quantity, SiteFacts+Work, Squad+Mix), the actual shared theme/fonts and canonical food atlas views.31examples cover counts/caps/cash/controlled and waiting-equipment reasons, stash-only food and money, selected unit/total proceeds, stock revision failure, modal focus, build/upgrade/rebuild/repair paid progress and no-worker/unsafe/ruin cases, and distinct rally side/target/readiness/cash/active/recall/returning conditions. Old completed main capacity remains8while its paid upgrade is unfinished. Low90/high890HP wall2both quote fixed40; paid additional damage is covered to full900health; replacement builders retain payment/progress. The unfinished main upgrade retains price200/work90/capacity8and full1400completion semantics. State controls execute inspection examples, never purchases or AI.

Shared minimap/expanded map builds an explored terrain mask and native known-portal/defense/player markers with a legend and camp border. Unknown portals are omitted from geographic markers, lists, metadata, text, tooltips and focus trees. Four exact canonical identities retain P1Élodie/square/#F0C46A,P2Jérôme/triangle/#74BDE0,P3Alex/diamond/#DB91B8,P4Alex/cross/#A3CC83. Actual Polygon2D vectors/colors and ordinal row IDs are observed, including duplicate Alex names. Spectator follow Previous/Next and the external fixture selector are exercised through actual native signals. Waiting/requested/transfer/unavailable examples keep ownership disabled with reasons and do not imply gameplay succession has been implemented.

All11notification kinds (nightfall,retaliation,damage,portal,order,cash,equipment,inventory,target,save,connection) have separate audible/muted examples. Full text plus Sound muted remains in CanvasLayer40 above ordinary Inventory/People/Sale/Site/Rally context panels. Warning details and map details have independent local scrolling. The complete H23 proof independently reverses definitions, independently reverses sample events, then reverses both; original IDs, event quantities, positions, canonical identities and warning layer/text remain exact. Seven separately named UIH.UI-FIXTURE-PREVIEW.empty/loading/error/populated/partial/overflow/long-text resources execute natively; fixture controls remain outside the viewport.

## Task commits and tracer

- 309da52 test(04-16): specify stash capacity and rally inspection behavior
- a1a556e docs(04-16): preserve exhausted camp parser checkpoint
- bca40a6 docs(04-16): authorize bounded authenticated map parser repair
- 479cdc0 feat(04-16): inspect camp people stash sales paid work and rally states
- 3a5f986 test(04-16): specify muted warnings hidden portals and spectator reasons
- 1f6a3f5 feat(04-16): inspect explored maps succession and persistent warning layers

Task1 exact-five GREEN479cdc0 follows original genuine RED309da52. Fresh committed SAME-CASE tracer3f74c966-cb5d-4ee9-9c14-df7f2da9e495 passes726native+4compiled before Task2 starts. Task2 genuine RED3a5f986 precedes exact-five GREEN1f6a3f5. The final selected set executes both cases again on1f6a3f5: layouts.camp32e7b3b3-a011-49f4-950a-40f07d8695fc passes726native+7compiled0skip; layouts.map-warnings5f60b031-21fd-4a0a-bec3-bcaf4d84ebdf passes2441native+7compiled0skip. Strict production and native harness builds report0warnings/0errors and actual native stderr is clean. Final camp PID32148, map PID15160; each invocation binds its exact compiled production DLL/core hashes, nonce and child PID in its immutable native observation and assembly-closure files.

## TDD Gate Compliance

Original Task1 RED2: three real compiled methods fail intended Sale37versus31,Capacity8versus0 and specific rally reason versus empty assertions. Original unchanged JUnit/timestamps and classifier RED_EVIDENCE_OK remain at.phase4/04-16-task1-red/. Syntax, sandbox and later constructor failures are not RED.

Task2 actual unchanged JUnit.phase4/04-16-task2-red/tests.xml executes7tests:4priorPASS,3new planned assertionFAIL,zero skips/fixture/loaderrors,exit2. Muted warning omitted Sound muted, unknown portal incorrectly returnedtrue, and unavailable spectator incorrectly returnedReady. The exact target Warning_MutedRetainsFullInformation executed and failed its ShouldAssertException assertion; classifier RED_EVIDENCE_OK and semantic assessment authorize GREEN. Supported report freshness and exact command are preserved in evidence.json. Final seven tests pass in both new cases. No optional refactor commit or synthetic RED is claimed.

## Deviations from Plan

### Bounded node repair and auto-fixed issues

The original Task1 corrective budget remains exhausted3/3. Checkpointa1a556e, original native constructor-failed d8f14e5e-9863-445e-aeaa-ebdc48f00267 and exact five-file snapshot manifestSHA256355bbc52ec2b93ca0a2af2c20345e5b96c3218d54a86b82198e07085857ec070 remain immutable. Prospective coordinator authoritybca40a6/04-16-REPAIR.md DECOMPOSES into exact parser R1.1and complete original camp R1.2; no original attempt is reset, no fourth original correction occurs and no assertion is pruned.

R1.1initial bee7c57f failed one SCR0008 member-order error before native execution. Correction1/2 moved the private parser;7f06cc4c passed strict/harness and4compiled, but positive constructors exposed unrelated authored CAMP_POPULATION. Correction2/2 extracted the private real ReadMap boundary used by the constructor: every exact binding authenticates first, then only content/world/map.json $.native.nodes accepts512. Generic256, authored128,1MiB bytes/depth20/nodes30000, Unicode/key/finite-number and domain/hash/ownership limits remain strict.6e4fcc91passes all23independent parser outcomes: actual275;512accept/513reject; sibling/nested/lookalike/child257reject; authored257and129row reject; wronghash before exception; restored current positive; generic256/257; no attachment. The whole run still fails CAMP_POPULATION and receives no camp/teardown credit. R1.1is complete with2/2corrections and no later R1.1correction.

R1.2initial fixes only concrete people.fireside total8to10, preserving8unassigned+2controlled, capacity8and original reason, and rebinds its own exact data hash.9a25b8b0passes all31states/fourcompositions/726native+4compiled0skip and clean native log. R1.2uses0/2corrections. No map/prior dependency is edited.

Task2 initial604b6b06passes strict/harness and7compiled but fails actual native theme setup because the own map pack omitted the existing donor-kit.png dependency. Correction1/3 authenticates it;2a87cebdexecutes2438true observations but remainsFAILED for3actual Godot signal-emitter-Free errors. Correction2/3 adds deferred connections and exact accepting/generation/tree fences, with separate obsolete-generation and post-Close callbacks;07c62b0cpasses2440native and clean log. Correction3/3 preflights succession/fixture/ordinaryPanel/notification text before native replacement; the added malformed-snapshot check requires exact CAMP_TEXT succession while retaining identical Map/Warnings objects.573743a7passes2441native+7compiled and clean log. Task2 uses3/3corrections; no fourth is made. Independent retention file.phase4/04-16-final/map-outcome-retention.json proves all2438original IDs retained in order, zero duplicates, plus3controls. Original failed native logs remain failures.

Rule1/2/3changes above concern only the original owned paths and correctness boundaries. Sandbox spawnSync gitEPERM/REVISION_UNAVAILABLE happened before tests; equivalent authorized ordinary execution supplied real reports and never counted as behavioral RED or a source corrective attempt. No package install, tooling update, public API, dependency edit or architecture change occurred. Deferred signal syntax was checked against the pinned local GodotSharp4.7.2 XML and [official Godot C# signals](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_signals.html) and [Object deferred connection contract](https://docs.godotengine.org/en/stable/classes/class_object.html#enum-object-connectflags); Context7/ctx7were unavailable. Full bounded accounting is in04-16-R1-EXECUTION.md.

## Fresh final ordered selection

Authority.phase4/04-16-final/selection.json independently checks the current Critical registry, freezes448raw input identities, records the previous selection and preserves every omission.39/39executable cases pass; drift[]. The table records immutable invocation IDs, compatible groups/order and measured times.

| Case | Importance | Group / order | Invocation | Duration ms | Result |
| --- | --- | --- | --- | ---: | --- |
| export.layout | Critical | G0 / 0 | d5b1d57e-666a-46a8-80a1-1bce684c8285 | 24666 | PASS |
| audio.feasibility | Critical | G0 / 6 | 879ae469-3135-4eb7-8fd0-5d53eb04cc89 | 9280 | PASS |
| media.conversion | Critical | G0 / 20 | 0ae80c8c-6738-4180-a0ea-ac6689f2031a | 42882 | PASS |
| media.scale | Critical | G0 / 21 | f1380de1-539c-43e9-b642-11ce853a13af | 7960 | PASS |
| items.weapons-food | Critical | G0 / 22 | 70b0665e-a8de-43f8-a38a-da13d9f96b6a | 7712 | PASS |
| items.wearables | Critical | G0 / 23 | c434009c-6763-4232-b9ff-188b75253e77 | 13021 | PASS |
| buildings.main-art | Critical | G0 / 24 | ff8f7b71-acb2-4993-8a74-920360ce05f9 | 4912 | PASS |
| buildings.main-state | Critical | G0 / 25 | 53383ab6-53fe-4d89-bb91-15223e69ae7b | 25390 | PASS |
| bodies.a-art | Critical | G0 / 26 | b847e74f-3f44-47f6-b3af-b87616b817f3 | 7633 | PASS |
| bodies.a-fit | Critical | G0 / 27 | c3daa753-52d5-4580-a894-36dd1a8b4794 | 26286 | PASS |
| bodies.b-wide | Critical | G0 / 28 | b783572c-3269-48b1-9ac8-8d0c1c17aab9 | 44507 | PASS |
| bodies.complete-fit | Critical | G0 / 29 | e15aed2c-ec94-4e2f-b5b0-6df08711c306 | 47298 | PASS |
| layouts.theme | High | G0 / 31 | 808b8dcf-7e7e-4be9-a402-66136969ae76 | 14036 | PASS |
| layouts.identity | High | G0 / 32 | 87268ac4-de6a-472e-9db1-f780b658e058 | 12109 | PASS |
| layouts.identity-integrity | High | G0 / 33 | 62122b33-70a9-4b76-8b9a-deb40c1d7008 | 14221 | PASS |
| layouts.inventory | High | G0 / 34 | 097cfcec-be11-4112-bc49-a003f169c81e | 15744 | PASS |
| layouts.legacy-target | Critical | G0 / 35 | 25728fa2-5de7-4bd2-bc6f-dedc0288539d | 17768 | PASS |
| layouts.camp | High | G0 / 36 | 32e7b3b3-a011-49f4-950a-40f07d8695fc | 14426 | PASS |
| layouts.map-warnings | Critical | G0 / 37 | 5f60b031-21fd-4a0a-bec3-bcaf4d84ebdf | 14540 | PASS |
| animation.melee | Critical | G0 / 50 | e79ad787-6c87-4fc4-8547-7a226b769e65 | 23752 | PASS |
| animation.replacement | Critical | G0 / 51 | 56b71a3a-1ae3-4f9b-86c3-311426b5aecc | 23863 | PASS |
| animation.bow | Critical | G0 / 52 | fa2f1235-392e-42bc-91e5-ae0de984468e | 43911 | PASS |
| animation.work-special | High | G0 / 53 | abc0b44f-4ae0-4fdd-a5a5-1ec36fc0adb8 | 25152 | PASS |
| appearance.corpses | Critical | G0 / 54 | 8b0036f2-10c9-4672-bca7-19da388a1cd9 | 24972 | PASS |
| appearance.crowds | High | G0 / 55 | 3a475080-e923-42f0-ad4b-d0b1e9b48b7e | 30077 | PASS |
| data.foundation | Critical | G0-data / 40 | b93f314f-7fce-42eb-83a2-9ffb42bd18f2 | 471 | PASS |
| data.items-combat | Critical | G0-data / 41 | c6263a8e-92b5-46d1-8383-ad91f6fb6c74 | 6587 | PASS |
| buildings.defense-art | Critical | G0-defenses / 1 | 42b9978e-5409-4bf8-bed5-9d79f9507848 | 2033 | PASS |
| buildings.defense-state | Critical | G0-defenses / 2 | 1110ebee-244c-4af7-99b4-3d1987b688a3 | 10836 | PASS |
| export.profile | Critical | G0-export-profile / 350 | bee24498-90bd-4da4-95f4-cf47a7c562b9 | 8785 | PASS |
| export.profile-boundaries | Critical | G0-export-profile / 351 | 3e885fd5-0e04-47b3-84b4-c8f1fa38c600 | 42979 | PASS |
| hub.source | Critical | G0-hub / 1 | ca3674e6-8cde-4203-a653-136cb7bea115 | 378594 | PASS |
| hub.lifecycle | Critical | G0-hub / 2 | af0d778b-f243-497a-acc3-9c2b6f7abe31 | 388078 | PASS |
| source.case-discovery | Critical | G0-integration / 360 | 726f4835-b4e7-4365-a613-ba250b768861 | 721 | PASS |
| world.map | Critical | G0-world / 1 | 271ff4e8-cd5e-41a5-9a4b-695903917d01 | 6318 | PASS |
| world.effects | Critical | G0-world / 2 | 5c280498-6612-42db-8305-cbbeb5530966 | 20566 | PASS |
| world.inspector | Critical | G0-world / 3 | ee69c325-5b9c-4409-a220-727c4e3bdbc2 | 20646 | PASS |
| world.replacement | Critical | G0-world / 4 | cbbf7241-d904-4ae2-91ea-babc91957504 | 30267 | PASS |
| graph.visibility-inventory | Inherited boundary | Phase1 source / visibility | 50be1713-9409-46a4-a5b6-f8b66b69e365 | 31921 | PASS |

Selected-but-blocked source.provenance retains actual historical adb38f27-592b-4700-880c-3fa1d65d0c15, PARTIAL_PACK_REQUIRED and stale common.bow/Inspection.tscn binding, owner04-23/31. Source.publication is explicitly inferred blocked because its producer calls the same rejecting validation; no direct failure or independent controls are invented. Unchanged source.tracer/source.tracer-failures/source.packaged-expectations/export.feasibility remain omitted with inherited WINDOWS29 provenance, no rerun/pass credit. Current-index0/180 remains unchanged. Source-only layouts.kit remains unaffected with exact shared fonts/theme/media and all actual native consumers selected. Unrelated Phase1/2/3/game/hosted/release procedures retain their prior explicit owners; production strict build and exact Phase1 visibility run here. Actual rendered final selected-set testing remains04-32after completephase+independentreview, and acceptance reconciliation04-33.

## Artifacts this phase produces

All seven original plan paths are real committed source/resources. JSON resources carry complete authored samples and exact existing dependencies; no new archive/master/raw-donor bundle or retained archive member is produced by this UI-only slice. The two packs retain21ordered binding references (13camp+8map, shared duplicates intentional), listed below, rather than editing prior resource bytes. Existing production archives, font files, theme and canonical media remain exact. The native wrappers generate only run-owned ignored proof projects and reports, authenticated against current production closure; they are not shipped resources.

| Exact file | Tested raw bytes | Tested raw SHA256 | Committed filtered Git blob |
| --- | ---: | --- | --- |
| src/godot-game/inspection/CampLayouts.cs | 61237 | 55da347cdeab232c2eee214211a9372ad69bb7121bcc72a82d9b1aa5c669a2ce | 0002a8a538b2858d6f2b6861b3172a465e7e397a |
| src/godot-game/content/ui/camp-panels.json | 23150 | cd6c6da2908f6feb8987e35a325dc06ddf0db343b4f9780b36b2d625eed15230 | a49d4ac91c4e5035bc61d30c14c9f7672919bfac |
| src/godot-game/content/packs/ui-camp.json | 2283 | e05d78d363fc90ea218c6074c0bc278fc75658c415b697fdcdf1487594bc5505 | fcd4d6e1f358802ce30f08ec73575302339ef003 |
| tests/phase1/CampLayoutTests.cs | 4242 | 483c97644d030738bdd6d1fd2510e739000e819c1580829fbb2d3eb956802050 | 5980a7bed4513ccdc3b6a93dffc38520d94630ca |
| tools/phase4/cases/ui-camp.cjs | 46266 | df75bf96c3e11ab6c6e7f928d27052d36b65dbc71e76abce435356900bdd1b72 | b1cd47d1e8f32e348646b2c3709eb3beb6ce3cae |
| src/godot-game/content/ui/map-spectator-warnings.json | 67190 | 2887b8c699daa1d460d67c541876bbcd5a2313d635d2bcf4f580ee2aff3bc109 | 5cbe7136596561fc5f75f2bba9a036b230306945 |
| src/godot-game/content/packs/ui-map-warnings.json | 1603 | e34fdbfccabfc076d055c636284a532678b1b24debdeecf52d0bf57bca1fc00f | 06d42b04f8ebe61faa95999493583942b5bb3769 |

Git clean filters normalize configured line endings; every committed filtered blob was compared to git hash-object --path on the tested working file. Raw tested hashes and committed blob IDs are both retained, with no claim that CRLF and filteredLF bytes are identical. Canonical input manifest.phase4/04-16-final/artifact-inventory.json and frozen selection bind the exact raw tested files.

### Exact production and compiled-test symbols

CampLayouts remains an internal sealed plain class. The constructors, internal behavior/parser/projection/lifetime contracts and all private implementation members are listed with exact signatures. CampLayoutTests is the public proof class; its exact seven Fact methods and reflection helper remain compiled tests, not runtime gameplay. No public production identity is added.

| File and source line | Actual member/signature |
| --- | --- |
| src/godot-game/inspection/CampLayouts.cs:18 | `internal CampLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs)` |
| src/godot-game/inspection/CampLayouts.cs:37 | `internal CampLayouts(Control root, MainThreadGateway gateway, byte[] pack, byte[][] inputs, bool mapWarnings)` |
| src/godot-game/inspection/CampLayouts.cs:57 | `internal static int SaleMoney(int stashStock, int selected, int unitPrice, int stashMoney, int carriedFood)` |
| src/godot-game/inspection/CampLayouts.cs:69 | `internal static int Capacity(int completedTier, int pendingTier)` |
| src/godot-game/inspection/CampLayouts.cs:81 | `internal static string RallyReason(bool hasTarget, bool active, int ready, int personalCash)` |
| src/godot-game/inspection/CampLayouts.cs:91 | `internal static string WarningText(string text, bool muted)` |
| src/godot-game/inspection/CampLayouts.cs:97 | `internal static bool PortalVisible(bool known)` |
| src/godot-game/inspection/CampLayouts.cs:103 | `internal static string SpectatorReason(bool available)` |
| src/godot-game/inspection/CampLayouts.cs:110 | `internal static JsonElement Read(byte[] bytes)` |
| src/godot-game/inspection/CampLayouts.cs:116 | `internal void Open(string id)` |
| src/godot-game/inspection/CampLayouts.cs:124 | `internal void Project(byte[] snapshot, long generation)` |
| src/godot-game/inspection/CampLayouts.cs:160 | `internal void Close()` |
| src/godot-game/inspection/CampLayouts.cs:184 | `private static JsonElement ReadMap(JsonElement manifest, byte[][] inputs)` |
| src/godot-game/inspection/CampLayouts.cs:190 | `private static JsonElement ReadDocument(byte[] bytes, bool mapDependency)` |
| src/godot-game/inspection/CampLayouts.cs:205 | `private static bool Same(string left, string right)` |
| src/godot-game/inspection/CampLayouts.cs:210 | `private static string Text(JsonElement value, string key)` |
| src/godot-game/inspection/CampLayouts.cs:222 | `private static int Number(JsonElement value, string key, int minimum, int maximum)` |
| src/godot-game/inspection/CampLayouts.cs:233 | `private static void Require(bool condition, string code)` |
| src/godot-game/inspection/CampLayouts.cs:241 | `private static void Check(JsonElement value, ref int count, int mapPosition)` |
| src/godot-game/inspection/CampLayouts.cs:282 | `private static void Unique(JsonElement rows)` |
| src/godot-game/inspection/CampLayouts.cs:290 | `private static JsonElement Find(JsonElement rows, string id)` |
| src/godot-game/inspection/CampLayouts.cs:298 | `private static void Bind(JsonElement manifest, byte[][] inputs)` |
| src/godot-game/inspection/CampLayouts.cs:317 | `private static void BindWarnings(JsonElement manifest, byte[][] inputs)` |
| src/godot-game/inspection/CampLayouts.cs:338 | `private void ValidateMapDefinitions(JsonElement map)` |
| src/godot-game/inspection/CampLayouts.cs:386 | `private void ValidateMapState(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:430 | `private void MapView(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:459 | `private void MapTerrain(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:493 | `private void MapMarkers(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:527 | `private void MapPoint(Control viewport, JsonElement item, string kind)` |
| src/godot-game/inspection/CampLayouts.cs:552 | `private void Spectator(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:581 | `private void Cycle(JsonElement state, int offset, long generation)` |
| src/godot-game/inspection/CampLayouts.cs:598 | `private void Notifications(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:623 | `private void MapControls(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:654 | `private void MapSelected(long index, long generation)` |
| src/godot-game/inspection/CampLayouts.cs:665 | `private void ValidateDefinitions(JsonElement items, JsonElement main, JsonElement defenses, JsonElement map)` |
| src/godot-game/inspection/CampLayouts.cs:699 | `private void ValidateFoods(JsonElement items)` |
| src/godot-game/inspection/CampLayouts.cs:715 | `private void ValidateStructures(JsonElement main, JsonElement defenses)` |
| src/godot-game/inspection/CampLayouts.cs:741 | `private void Validate(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:757 | `private void ValidatePeople(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:782 | `private void ValidateSale(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:805 | `private void ValidateSite(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:824 | `private static void ValidateRally(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:840 | `private VBoxContainer Attach(JsonElement state)` |
| src/godot-game/inspection/CampLayouts.cs:859 | `private void People(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:905 | `private void Sale(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:952 | `private void Site(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:1008 | `private void Rally(JsonElement state, VBoxContainer content)` |
| src/godot-game/inspection/CampLayouts.cs:1040 | `private void AddLabel(Node parent, string name, string text, string id = "")` |
| src/godot-game/inspection/CampLayouts.cs:1050 | `private void AddAction(Node parent, string name, string text, string reason, bool available)` |
| src/godot-game/inspection/CampLayouts.cs:1060 | `private void AddIcon(Node parent, string id)` |
| src/godot-game/inspection/CampLayouts.cs:1079 | `private void ValidateIconDefinitions()` |
| src/godot-game/inspection/CampLayouts.cs:1094 | `private void Clear()` |
| src/godot-game/inspection/CampLayouts.cs:1112 | `private void Modal(int amount, int result)` |
| src/godot-game/inspection/CampLayouts.cs:168 | `private static readonly string[] _paths = ["content/ui/camp-panels.json", "content/config/items.json", "content/world/main-assemblies.json", "content/world/defense-assemblies.json", "content/world/map.json", "content/packs/ui-identity.json", "assets/ui/InspectionTheme.tres", "assets/ui/LiberationSans.ttf", "assets/ui/PerfectDOSVGA437.ttf", "assets/ui/Fallback.ttf", "assets/equipment/items-01-10.png", "assets/equipment/items-11-24.png", "assets/common/donor-kit.png"];` |
| src/godot-game/inspection/CampLayouts.cs:173 | `private readonly Control _root;` |
| src/godot-game/inspection/CampLayouts.cs:174 | `private readonly MainThreadGateway _gateway;` |
| src/godot-game/inspection/CampLayouts.cs:175 | `private readonly JsonElement _data;` |
| src/godot-game/inspection/CampLayouts.cs:176 | `private readonly JsonElement _definitions;` |
| src/godot-game/inspection/CampLayouts.cs:177 | `private readonly JsonElement _icons;` |
| src/godot-game/inspection/CampLayouts.cs:178 | `private readonly bool _mapMode;` |
| src/godot-game/inspection/CampLayouts.cs:179 | `private readonly List<Node> _owned = [];` |
| src/godot-game/inspection/CampLayouts.cs:180 | `private readonly List<AtlasTexture> _textures = [];` |
| src/godot-game/inspection/CampLayouts.cs:181 | `private bool _accepting = true;` |
| src/godot-game/inspection/CampLayouts.cs:182 | `private long _generation;` |
| tests/phase1/CampLayoutTests.cs:16 | `public void Sale_UsesStashFoodAndStashMoney()` |
| tests/phase1/CampLayoutTests.cs:24 | `public void Main_PendingUpgradeKeepsCompletedCapacity()` |
| tests/phase1/CampLayoutTests.cs:32 | `public void Rally_NoTargetAndActiveHaveDistinctReasons()` |
| tests/phase1/CampLayoutTests.cs:42 | `public void Reader_GenericArraysKeep256Limit()` |
| tests/phase1/CampLayoutTests.cs:56 | `public void Warning_MutedRetainsFullInformation()` |
| tests/phase1/CampLayoutTests.cs:65 | `public void Map_UnknownPortalIsHidden()` |
| tests/phase1/CampLayoutTests.cs:73 | `public void Spectator_UnavailableOwnershipHasReason()` |
| tests/phase1/CampLayoutTests.cs:80 | `private static object? Call(string name, object[] arguments)` |

The saved CJS module exports PACK,DATA,PATHS,HARNESS,sourceInputs(),isolated(),compiled(context),validateSource(pack,data),camp(context),MAP_PACK,MAP_DATA,MAP_PATHS,MAP_HARNESS,mapWarnings(context). It registers the exact CLI cases layouts.camp and layouts.map-warnings. Authored resources identify heldout.H23 and all seven individually named UIH.UI-FIXTURE-PREVIEW backstops; saved case metadata links their distinct native observations and later rendered checkpoints, without inventing separate delivered heldout CLI cases. HARNESS uses the actual production assembly and exact CampLayouts constructor/Open/Project/Close. Native proof-only InventoryHarness methods are Check(string,bool,object),Eq(string?,string?),Digest(byte[]),Read(string),Invoke(object,string,params object[]),Failure(Action),New(Control,byte[]?,byte[][]?),Label(Node,string),Nodes(Node),_Ready(),Run(),People(Node,JsonElement),Sale(Node,JsonElement,JsonElement),Site(Node,JsonElement),Rally(Node,JsonElement),ParserControls(),ConstructorControls(); MAP_HARNESS retains the common boundary helpers and substitutes its actual map constructor/Run plus Frames(),Reverse(JsonArray),Observe(Control,JsonElement,JsonElement,JsonElement,string),MapConstructorControls(). Exceptions are retained as actual typed/diagnostic failures; a bridge/load error, catch-all success, expected-value substitution or native abort cannot satisfy an observation.

### Exact authenticated retained dependency references

| Own pack | Existing dependency path | Required SHA256 |
| --- | --- | --- |
| ui-camp.json | content/ui/camp-panels.json | cd6c6da2908f6feb8987e35a325dc06ddf0db343b4f9780b36b2d625eed15230 |
| ui-camp.json | content/config/items.json | fa9bb09bfc3a999dd21349274f5010e5a653a868d4a0d0be73a46caaef226720 |
| ui-camp.json | content/world/main-assemblies.json | 1ca169e2bd8818fac84b4ff19d282aa9d6ed8b19b91adb70a830ef100c51ae1e |
| ui-camp.json | content/world/defense-assemblies.json | e8ec43336aa2c79c8b52539bc300d5918c3acc75df8b0506b71caa1f2ef26787 |
| ui-camp.json | content/world/map.json | 5d0dbde0ed6964996f3c5fbab4334c5c50d732c3e08d82515e2fcd89fe0ad3a1 |
| ui-camp.json | content/packs/ui-identity.json | 324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed |
| ui-camp.json | assets/ui/InspectionTheme.tres | b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a |
| ui-camp.json | assets/ui/LiberationSans.ttf | e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49 |
| ui-camp.json | assets/ui/PerfectDOSVGA437.ttf | ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102 |
| ui-camp.json | assets/ui/Fallback.ttf | e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49 |
| ui-camp.json | assets/equipment/items-01-10.png | 3d8b8e3d6aa0a2e94dc3fba8b64cc54573b62031be257db76c79fa2cac53cfe9 |
| ui-camp.json | assets/equipment/items-11-24.png | b5dafe0a20348a9b667e9eda74c4250fdcbf2ce2fffac38ab7193b19059a4219 |
| ui-camp.json | assets/common/donor-kit.png | bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4 |
| ui-map-warnings.json | content/ui/map-spectator-warnings.json | 2887b8c699daa1d460d67c541876bbcd5a2313d635d2bcf4f580ee2aff3bc109 |
| ui-map-warnings.json | content/world/map.json | 5d0dbde0ed6964996f3c5fbab4334c5c50d732c3e08d82515e2fcd89fe0ad3a1 |
| ui-map-warnings.json | content/packs/ui-identity.json | 324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed |
| ui-map-warnings.json | assets/ui/InspectionTheme.tres | b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a |
| ui-map-warnings.json | assets/ui/LiberationSans.ttf | e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49 |
| ui-map-warnings.json | assets/ui/PerfectDOSVGA437.ttf | ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102 |
| ui-map-warnings.json | assets/ui/Fallback.ttf | e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49 |
| ui-map-warnings.json | assets/common/donor-kit.png | bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4 |

### Complete saved sample identities

| Authored sample | Surface / mode | State / individually observable fixture |
| --- | --- | --- |
| people.normal | people | normal |
| people.cap | people | cap |
| people.fireside | people | fireside |
| people.cash | people | cash |
| people.controlled | people | controlled |
| people.remote | people | remote |
| people.waiting | people | waiting |
| people.pending | people | pending |
| sale.normal | sale | normal |
| sale.empty | sale | empty |
| sale.stock-changed | sale | stock-changed |
| sale.quantity | sale | quantity |
| site.repair-low | site | repair-low |
| site.repair-high | site | repair-high |
| site.repair-active | site | repair-active |
| site.no-worker | site | no-worker |
| site.unsafe | site | unsafe |
| site.upgrade | site | upgrade |
| site.build | site | build |
| site.locked | site | locked |
| site.cash | site | cash |
| site.ruin | site | ruin |
| site.complete | site | complete |
| site.destroyed-main | site | destroyed-main |
| rally.ready | rally | ready |
| rally.no-target | rally | no-target |
| rally.insufficient | rally | insufficient |
| rally.active | rally | active |
| rally.returning | rally | returning |
| rally.cash | rally | cash |
| rally.right | rally | right |
| map.minimap | minimap | populated |
| fixture.empty | minimap | empty |
| fixture.loading | minimap | loading |
| fixture.error | minimap | error |
| fixture.partial | minimap | partial |
| fixture.overflow | expanded | overflow |
| fixture.long-text | minimap | long-text |
| map.expanded | expanded | populated |
| spectator.follow | minimap | populated |
| spectator.waiting | minimap | populated |
| spectator.requested | minimap | populated |
| spectator.transfer | minimap | populated |
| spectator.unavailable | minimap | populated |
| warning.nightfall.audible | minimap | populated |
| warning.nightfall.muted | minimap | populated |
| warning.retaliation.audible | minimap | populated |
| warning.retaliation.muted | minimap | populated |
| warning.damage.audible | minimap | populated |
| warning.damage.muted | minimap | populated |
| warning.portal.audible | minimap | populated |
| warning.portal.muted | minimap | populated |
| warning.order.audible | minimap | populated |
| warning.order.muted | minimap | populated |
| warning.cash.audible | minimap | populated |
| warning.cash.muted | minimap | populated |
| warning.equipment.audible | minimap | populated |
| warning.equipment.muted | minimap | populated |
| warning.inventory.audible | minimap | populated |
| warning.inventory.muted | minimap | populated |
| warning.target.audible | minimap | populated |
| warning.target.muted | minimap | populated |
| warning.save.audible | minimap | populated |
| warning.save.muted | minimap | populated |
| warning.connection.audible | minimap | populated |
| warning.connection.muted | minimap | populated |

Backstop bindings: UIH.UI-FIXTURE-PREVIEW.empty→fixture.empty; UIH.UI-FIXTURE-PREVIEW.loading→fixture.loading; UIH.UI-FIXTURE-PREVIEW.error→fixture.error; UIH.UI-FIXTURE-PREVIEW.populated→map.minimap; UIH.UI-FIXTURE-PREVIEW.partial→fixture.partial; UIH.UI-FIXTURE-PREVIEW.overflow→fixture.overflow; UIH.UI-FIXTURE-PREVIEW.long-text→fixture.long-text. H23uses the original map.minimap and fixture.overflow under three independent reorder treatments; expected canonical identities and event quantities come from preserved original input, not observed outputs.

## Final native policy and producer handoff

Exactly one changed final semantic engine.source-policy observation runs after source is final: .phase1/runs/13ce1bf4-488e-457f-9024-f51a844624e1, statusfail,1861ungranted operations (delta291from1570). It remains an explicit04-23integration failure, with no permission/cohort enrollment or compliance claim. New CampLayouts contributes291operations. Every exact observed new path/line/column/member/host tuple is recorded in the appendix; original other-file observations remain in the immutable policy record. Existing WINDOWS30remains open, not duplicated or closed.

| Actual source file | Ungranted operations |
| --- | ---: |
| src/godot-game/inspection/BuildingPreview.cs | 299 |
| src/godot-game/inspection/CampLayouts.cs | 291 |
| src/godot-game/inspection/CharacterPreview.cs | 372 |
| src/godot-game/inspection/CorpsePreview.cs | 101 |
| src/godot-game/inspection/CrowdPreview.cs | 156 |
| src/godot-game/inspection/IdentityKit.cs | 78 |
| src/godot-game/inspection/InventoryLayouts.cs | 183 |
| src/godot-game/inspection/WorldPreview.cs | 381 |

Exactly one current evaluated producer observation recordsGodot.SourceGenerators/4.7.2, authenticated packageSHA256ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e,56currentinputs/32generatedsurfaces. .phase4/04-16-final/generated-input-observation.json retains actual source bytes/raw/text hashes, generated raw/text hashes, package identity and exact production/tested closures. No enrollment/replay or cohort permission occurs; WINDOWS31remains open. No unchanged known-blocked provenance/publication/index run is repeated. WINDOWS29/30/31remain owner23/31 obligations before24independentreview and32/33acceptance.

## Exact repair semantics and integration continuation

This slice uses creator initial-spec§8.3fixed50%valuation fees: main50/150/400,walls10/40/110,towers20/65/155; independent of missingHP, one accepted payment covers additional damage until full health, replacement workers retain payment/progress. Valid build/rebuild/upgrade completion restores full destination health for the original price/work. No gameplay implementation is claimed.

Unchanged src/godot-game/content/world/main-assemblies.json tierDefinitions[0/1/2].repairPrice remains obsolete30/60/210; currentCampdefinitions truthfully use50/150/400. Actual old consumer is BuildingPreview.ShowStatus(int,JsonElement,JsonElement), especially repairPrice read at line595of this final source; source/native proofs live in tools/phase4/cases/main-buildings.cjs and tests/phase1/BuildingPresentationTests.cs. CampLayouts.ValidateStructures and its ui-camp.json exact main/defense bindings are direct affected consumers. Defense lifecycle/current health samples and consumer BuildingPreview plus tools/phase4/cases/defenses.cjs also need current full-health/paid-order semantics checked; they were not edited here. Existing guards.jsonrecipe retains historical inputs and must not be blindly regenerated. Before19/23integration edits, freeze the precise affected path/consumer union prospectively and rebind only changed own manifests; record actual tests and preserve original art/guard recipes. Plan19owns upcoming jobs/economy/worlddata;23owns reconciling prior presentation artifacts before24review. No out-of-scope dependency mismatch is silently accepted as current behavior.

Parent owns global STATE/ROADMAP/REQUIREMENTS/PROJECT/spec progress, wave20, push and existing draftPR5targetingdev. Five foreign unstaged files remain untouched/unstaged; removed06-DISCUSS-CHECKPOINT.jsonis not recreated. Preserve Phase06/07and INV-14Phase12history. No branch switch, merge, reset, stash, broad staging, push or PR lifecycle transition occurs here. This plan hands local completed count24→25/37and next19to the parent without changing global planning or marking ASSET/phase complete. Branch feature/04-asset-production remains current and PR5remainsdraft.

## Known stubs, threats and evidence limits

No TODO/FIXME/placeholder, skipped test or unrun original automated verify remains in the seven outputs. Empty fixture rows, empty completedObligations and null visualReview are intentional authored empty-state and pending-acceptance records with wired production data sources; they are not fabricated pass data. Stub scan and native teardown checks pass. No new remote endpoint/auth/schema/recovered-file access boundary outside the plan threat model is introduced. Fixed exact bindings, strict bounded parsing, pre-native validation, owned cleanup and obsolete-result fences retain T-04-46/47mitigations. T-04-48actual acceptance evidence remains separately pending; no source/headless result supplies it. Existing known integration defect register29/30/31and actual32/33obligations retain their separate scope.

## Self-Check: PASSED

All 485 checks in .phase4/04-16-final/self-check.json pass: the canonical SUMMARY exists with complete local status; all seven files exist and match tested raw SHA256 plus committed filtered blobs; all six ledger commits are ancestors of HEAD; the measured interval contains six commits; the original snapshot manifest and five exact source copies retain their immutable hashes; all 39 executable final results pass with zero drift; all 2,438 original map outcomes remain in order without duplicate IDs; and all 448 frozen raw inputs still match. The realized seven-source-path diff is 212,867 characters, or 53,217 estimate-scale tokens. The subsequent SUMMARY documentation commit lies outside the measured source snapshot. Global planning/specification WIP remains untouched and unstaged, with its progress update handed to the parent.

