---
phase: 04-asset-production
plan: "06"
subsystem: character-art-and-equipment-fitting
tags: [Godot, CSharp, pixel-art, retained-provenance, native-proof, TDD]
status: complete
requires:
  - phase: 04-05
    provides: Retained ordinary item masters, parts and authenticated equipment atlases
  - phase: 04-26
    provides: Guarded native hub, owned input, transport and preview lifetimes
provides:
  - Three neutral body families with48 authored poses,193 reusable parts and232 retained archive members
  - Internal character presenter with eleven logical slots,48 anchor sets and both-facing ordinary equipment composition
  - Complete source/headless fit proof over2976 tuples,12 independent H13 fixtures and ten compiled subjects
affects: [04-07, 04-14, 04-18, 04-23, 04-24, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [source-reproducible neutral poses, per-body anchors, logical hand identity, guarded native ownership, immutable separate observation reports]
key-files:
  created:
    - src/godot-game/assets/characters/bodies-a.png
    - src/godot-game/content/packs/bodies-a.json
    - production/phase4/bodies-a.zip
    - tools/phase4/cases/bodies-a.cjs
    - src/godot-game/content/animation/fits-a.json
    - src/godot-game/inspection/CharacterPreview.cs
    - tests/phase1/CharacterFitTests.cs
    - .planning/phases/04-asset-production/04-06-ARTIFACTS.json
  modified:
    - src/godot-game/inspection/InspectionRoot.cs
key-decisions:
  - "Use density1 registered48x40 poses and retain separate generated originals, source references, exact transforms and rejected attempts."
  - "Keep logical hands independent of front/back rendering; removing armor reveals the selected torso and replacement retires old layers."
  - "Preserve raw hub/matrix reports and publish their combined fit report under its own exclusive filename."
  - "Close only local04-06 production; WINDOWS29/30/31 block phase completion and independent review until04-23/31 repair and fresh proof."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-03]
coverage:
  - id: neutral-body-source
    description: Three source-reproducible neutral body families and retained provenance
    verification:
      - kind: integration
        ref: "bodies.a-art /6593f525-185f-463a-8f92-f4e9ecd20ed0"
        status: pass
    human_judgment: false
  - id: complete-ordinary-fitting-headless
    description: Native composition, actual coat restoration, logical hands, replacement and independent H13 data
    verification:
      - kind: integration
        ref: "bodies.a-fit /7172b858-5354-4988-979c-d6aba2c92282"
        status: pass
    human_judgment: false
  - id: final-phase-policy-integration
    description: Current exact native permissions and generated-producer cohort
    verification:
      - kind: integration
        ref: "engine.source-policy /cc21a87b-1617-4935-9610-41bb9ff120cb"
        status: fail
      - kind: integration
        ref: "engine.generated-boundary /ae38be89-41cf-4de9-b64d-42862bca3789"
        status: fail
    human_judgment: true
    rationale: "Open04-23 integration work requires actual source/producer repair and fresh automated proof before independent review; human approval cannot substitute for it."
  - id: final-character-appearance
    description: Actual rendered silhouette, fit, overlap and nearby-body readability
    verification: []
    human_judgment: true
    rationale: "Requires complete phase implementation, independent review/fixes and separately owned04-32/33 actual Windows1280x720/no-VM acceptance."
actuals:
  tokens: 131033
  tasks: 2
  commits: 9
  diff_chars: 524129
  scale: "ceil(chars/4) over ordinary git diff --no-ext-diff for the eight implementation paths; binary notices counted, not archive bytes or harness tokens"
commits: 9
plan_head_before: fbb7dbca850472ad35e30f6fc5eb97daece23453
plan_head_after: 531573e690fe90d81e2ef745437a75b41261acdd
duration: 69min
duration_scope: "Observed retained verification/repair window02:29:39.946Z to03:38:46.499Z; initial authoring/generation prelude unmeasured"
completed: 2026-11-17
---

# Phase 04 Plan 06: Survivor and first zombie body fits Summary

**Three neutral survivor/shambler/runner pose families with source-reproducible components and real layered ordinary equipment fitting, proved through the actual production DLL.**

Local production is complete at2/2 tasks. The fresh thirteen-case source/headless selection is **11PASS/2FAIL**. This summary supplies no full-selected-set pass, final native-policy compliance, independent review, phase completion or ASSET acceptance. The [policy integration amendment](04-POLICY-INTEGRATION-AMENDMENT.md) explicitly assigns the two open integration failures to04-23 before04-24 independent review and04-32 acceptance. Original tests, five producer inventories, controls and failed reports remain intact.

## Performance and cost

The earliest retained failing source verification began2026-11-17T02:29:39.946Z; closeout measurement was2026-11-17T03:38:46.499Z. That observed window is69min6.553sec and includes repairs/checkpoints. Initial art authoring, generation and context-loading time is unmeasured and is not reconstructed.

Actuals are524129 realized unified-diff characters /131033 estimated tokens on the plan's chars/4 scale, against38000 estimated tokens. The eight implementation paths are enumerated below. Binary notices are counted on the same scale as prior summaries; compressed ZIP/PNG byte sizes are separately inventoried. Scoped summary/inventory metadata is excluded from implementation diff cost.

Nine commits are measured by `git rev-list --count fbb7dbca850472ad35e30f6fc5eb97daece23453..531573e690fe90d81e2ef745437a75b41261acdd` using the persisted `.git/gsd-plan-head-before-04-06` ledger. This interval includes the checkpoint/coordinator metadata commits listed below; it does not mean nine implementation commits. This summary commit is outside that recorded before/after interval. The closeout did not reset the ledger.

## Accomplishments

- CH-01 shared survivor, CH-02 hunched shambler and CH-03 lean forward runner have48 separately inspectable authored poses,144 partitioned components and one adapted local head,193 parts total. Three selected generated originals, exact prompts, references, transformations and rejected attempts remain in the232-member bundle.
- Actual Characters-page controls compose all22 legal ordinary items across31 slot/item choices, three bodies,16 poses and two facings:2976 legal tuples. Armor removal restores the selected IT-13 coat; logical hands retain item IDs; replacement retires old layers; broken and empty states remain explicit.
- The real production DLL and isolated authored harness build with zero warnings/errors. Fresh native hubPID16660 and matrixPID37660 authenticate the invocation/assembly and produce5972 true observations,12 independent H13 fixtures and ten compiled passing subjects with zero skips.
- Discovery, both inherited hub suites, visibility, generated origin and four native/Core baseline regressions pass. Current native-policy and producer replay remain failed, recorded and assigned before final review.

## Task commits and source identity

1. **Task1 neutral body art:** `2c54e537df089e486a777dc131e5f6130c1cc691` — original four owned implementation paths only.
2. **Task2 genuine compiled RED:** `765c45d0f182840dc39defd099f4f99d64df702c` — CharacterFitTests plus saved case/native client.
3. **Task2 complete ordinary fitting:** `531573e690fe90d81e2ef745437a75b41261acdd` — exact five authenticated Task2 paths, normal hooks, no source edit during closeout.

The same measured interval also includes `cc23bfc` (strict harness checkpoint), `8c27909` (bounded R3 authority), `c4fbb75` (R3 result/publication checkpoint), `b7890d1` (bounded R4 authority), `a2acc93` (fresh selection checkpoint), and `6016525` (final integration ownership). Earlier `2c29ff0`/`ac0da21` checkpoints and the ledger-base `fbb7dbc` preserve Task1/R1/R2 history outside this interval. Foreign `affc858` is not attributed to this executor.

Mandatory post-Task1 committed-byte tracer `a4ca9e61-d718-453c-b916-4e21971d48a7` passed before Task2 expansion. The fresh final local observations were executed on **`b7890d1dcda3ce5ca71873fc35102aa0bbfb5724` plus the exact five WIP files below**, not on the later production commit. Only metadata commits followed that tested HEAD before this byte-preserving source commit. No test was rerun solely for the documentation/ownership decision.

| Authenticated Task2 path | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/content/animation/fits-a.json | 90821 | 66f5e551bcff45ec4832c408743ca13c0c5a19310dd0d90f002a5e21784b0a0c |
| src/godot-game/inspection/CharacterPreview.cs | 19949 | 90a609b0eb2bcc48d327bfdbfbdd9f891d57aeb9ff78ace37765d5f3a0c76a35 |
| src/godot-game/inspection/InspectionRoot.cs | 20173 | c58ef31d929582782c26b05c46da21d1f40d5fd185688cfb373eb44efecb8fce |
| tests/phase1/CharacterFitTests.cs | 5151 | 4cc71323c97aeb0d7c163818ef91f69c678893e44538b57b702650bfc70c6801 |
| tools/phase4/cases/bodies-a.cjs | 38734 | eee7bc9c56d713064926235721d8867a9b30f8ef4b7e8057a70059df953b7274 |

All five live files and all five backups authenticated against `.phase4/checkpoints/04-06-r4-boundaries/manifest.json` before staging. Source315 freeze `ce30099bee51ec442ac79e7b590c25429df1bd0ea7711eb048f12a7540e90c21` and expanded508 freeze `5b84b7385c9bee2d097da4e2f6aa7808442b472d2104f200e2a1e316cf55f6a4` matched at selection end. The315 snapshot still matches at closeout. The expanded snapshot now differs only in later04-06-CHECKPOINT/04-06-REPAIR/deferred-items metadata; it was taken during the selection as an additional freeze, not represented as the original pre-run snapshot.

## Artifacts this phase produces

The complete machine inventory is [04-06-ARTIFACTS.json](04-06-ARTIFACTS.json). It enumerates **every232 retained ZIP member** by exact path, uncompressed size and SHA256; all193 part/resource identities and atlas rectangles; selected original/prompt/source/recipe records; every actual C# declaration with source line/signature; all case/Python/GDScript/harness declarations; the complete fit schema/anchors/layers; actual saved controls/backstops; and every290 observed native operation with exact source path/line/column/target. The appendix is scoped metadata generated from the actual bytes, not a new implementation path or native permission.

| Exact implementation file | Actual output |
| --- | --- |
| src/godot-game/assets/characters/bodies-a.png | Actual768x520 transparent nearest-sampled registered body atlas; `res://assets/characters/bodies-a.png`; SHA256002148ddb40d3cc5a4c2695e45e3ebca765e79c06f53e0f7d120d22cdf9f5141 |
| src/godot-game/content/packs/bodies-a.json | Version1/schema1 `bodies-a`, source-side density1/D-13-pilot-1; three neutral body identities,16 poses each,193 part bindings and57 source-suitability rows |
| production/phase4/bodies-a.zip | Complete232-member retained production bundle; exact archive SHA256 and every member identity in the appendix |
| tools/phase4/cases/bodies-a.cjs | Saved Critical `bodies.a-art`/`bodies.a-fit` scenarios, G0 orders26/27, no game window; source replay, native hub client, strict isolated matrix harness and compiled gate |
| src/godot-game/content/animation/fits-a.json | Version1/schema1 `fits-a`, eleven slots,48 body/pose anchor sets,22 items, both facings, exact source bindings, layer/replacement rules and H13 review ownership |
| src/godot-game/inspection/CharacterPreview.cs | New internal sealed CharacterPreview, constructor and guarded synchronous composition/definition/fitting members; no new public Godot Node script |
| src/godot-game/inspection/InspectionRoot.cs | Existing type gains private CharacterPreview? _character; modified _ExitTree(), Recheck(), Cancel(), ChangeCategory(long), Select(long), Selection() retire/recreate the owned presenter |
| tests/phase1/CharacterFitTests.cs | New public sealed test type with five test method declarations /ten expanded subjects and private Native()/Subject() helpers |

Plan-associated metadata within the measured history is `.planning/WINDOWS.md`, `04-06-CHECKPOINT.md`, `04-06-REPAIR.md`, `04-POLICY-INTEGRATION-AMENDMENT.md` and `deferred-items.md`. This closeout adds only this SUMMARY and `04-06-ARTIFACTS.json`. Those documents retain actual ownership/history; shared PROJECT/REQUIREMENTS/ROADMAP/STATE, initial-spec, Phase06 and Phase12 WIP are foreign and remain unstaged.

### Actual members and resources

CharacterPreview's complete declarations, including constructor parameters, return types and private fields, are in `sourceDeclarations` of the appendix. Its callable inventory is CharacterPreview(Node2D,MainThreadGateway,JsonElement,JsonElement,JsonElement,JsonElement,string), ReadDefinition(byte[],string), Open(Node2D,MainThreadGateway,Func<string,byte[]?>,string), FitItem(JsonElement,string,string,string,int), Attach(), Detach(), Text(JsonElement,string), ValidateValue(JsonElement), AddSelector(Node,string,string,IEnumerable<string>), Changed(long), Selected(string), Project(), AddBody(Node2D,JsonElement,int), AddItem(Node2D,JsonElement,JsonElement,string,string,int,string), ReconcileHands(), AddLayer(Node2D,JsonElement,JsonElement,JsonElement,JsonElement,string,int), AddFittedSprite(Node2D,JsonElement,JsonElement,string,int,Vector2,Vector2,float,JsonElement,JsonElement,string), LayerOrder(JsonElement,string,int), Texture(JsonElement,JsonElement,string), Point(JsonElement). Exact source signatures in the appendix are authoritative.

CharacterFitTests declares Selected_Coat_ReturnsAfterArmorRemoval(), Native_Matrix_PreservesEveryFittedItem(), Native_H13_OverlapAndNearbyBodiesRemainIndependent(), Unfitted_Tuple_HasExactDiagnostic(string,string,string,int), Definition_RejectsAmbiguousInput(string,string), Native(), Subject(). The four unfitted subjects retain exact body/item/pose/facing diagnostics and three malformed definitions retain duplicate-key, definition-ID and size rejection.

The case module exports ART_FILES, VERIFY, fingerprint, art, isolated, DRIVER, nativeFit, fit, FIT_FILES, FIT_HARNESS, nativeMatrix, validateFits and fitControls. Local engine() and SLOT_ITEMS/PYTHON constants are also inventoried. VERIFY has largest/replay/verify; DRIVER has choose/_initialize/run. The isolated CharacterHarness type has actual fields and Invoke/Read/Create/Check/Select/Item/Gear/Signature/Nonempty/ExactPixels/_Ready/Run members, all enumerated exactly in the appendix. This harness is an invocation-local proof program, not a new production script.

New runtime resources are the atlas, body pack and fit JSON above. Body part keys, three master IDs, all48 full-pose IDs,144 component IDs and CH-02.local-head are enumerated in `art.parts`. No production scene/subresource file or new public Node script is introduced. Dynamically constructed controls use Controls/Slots with Body, Pose, Facing, Condition, Zoom and the eleven slot names, plus Art, Status and owned layer sprites.

### Native operation inventory and final-policy boundary

The observation contains124 CharacterPreview operations in eleven exact methods and166 inherited BuildingPreview operations in seventeen methods. Both sets retain exact target signatures and source coordinates under `nativePolicyObserved`; inherited BuildingPreview rows are distinguished by their original source path, not attributed as new04-06 implementation.

CharacterPreview's eleven native-operation owners are Attach(), Detach(), AddSelector(...), Selected(string), Project(), AddBody(...), ReconcileHands(), AddLayer(...), AddFittedSprite(...), Texture(...) and Point(...). The existing guarded InspectionRoot.Selection() also constructs Node2D and sets Name at line427. These are observations for final04-23 exact-policy reconciliation with independent guard controls, **not granted tuples**. Native worker/detached rejection passed in the current fitting harness; that does not replace complete neighboring/wrong-path/deferred/thread/lifetime permission proof.

Observed current generated sources are retained under `.phase4/observed26/70993187039f49129c73ad527a489bf1/`: ScriptMethods43, ScriptProperties37, ScriptSerialization57 diagnostics,137 total. Their exact raw hashes are in the appendix. No current-producer cohort was enrolled by this plan and no historical inventory or generated permission was changed.

### Retained generation, sources and recipes

The bundle's selected masters are generated CH-01 featureless tan artist mannequin, CH-02 hunched grey-green mannequin and repaired CH-03 lean runner. Exact image_gen originals are exec-7049c737-a451-4364-87f1-475543e6b056.png, exec-150d36ae-c41e-4d9f-854d-21ec9d37d447.png and exec-556feeac-a216-4745-bba1-83c52eee88c2.png. Each selected master and expanded prompt hash is retained in the pack/appendix and bundle.

History retains the initial safety rejection7d7bd875-d6da-4f0d-b787-2b0a29e3128e (moderation_blocked; no returned image), the covered survivor retry exec-f927d70c-6320-40d3-ba87-9d73b77da190.png rejected for resembling baked clothing, and runner exec-190acc74-9b64-45dd-bac6-3a34b6341d93.png rejected for a clipped head, with the original/repair expanded prompts. None is silently counted as selected suitable art.

Exact six K14/K18 references are common.s0.2361/7052/10365/7060/7246/10669, each with retained raw/PNG/record triple. Local idle/walk/attack PNG references and scale-pilot metadata remain separate. All selected generated original frames have source-suitability records. Baked donor clothes are excluded from neutral pixels. CH-02 idle uses the adapted uncovered local head from source crop[560,260,815,540], nearest5x6.

The retained build-bodies.py recipe thresholds alpha at240, removes observed red matte using r>180 and r>2*g and r>2*b, preserves the largest8-connected selected component, crops exact recorded rectangles, applies one body-wide nearest scale and places on48x40 registered canvases. Kept interior RGB values are unchanged. Logical standing heights are30/28/30, with source-side density1; inspection zoom is separate. Recipe functions sha/js/png/clean/rows/largest/main and the canonical PNG/JSON are retained and fully inventoried. No package install, paid service or new dependency occurred.

## Fit contract, controls and saved scenarios

The fit definition binds exact hashes of bodies-a/items-01-10/items-11-24. Slots are headwear(IT-14/18), face(IT-15), torso(IT-11/13), legs(IT-12), armor(IT-19/20), gloves(IT-16), boots(IT-17), backpack(IT-23/24), belt(IT-21), left(IT-02–10) and right(IT-02–10). Each slot offers Empty. Condition is Usable/Broken, zoom1/2/4/8, facing right/left, with body/pose controls.

Each body uses idle, walk-near, walk-far, run, windup, strike, recover, aim, hit, collapse-a, collapse-b, corpse, sit, gather, eat, carry. Each pose has head/face/torso/hips/backpack/leftShoulder/rightShoulder/leftHand/rightHand/leftFoot/rightFoot anchors and actual retained full/head/upper/lower components. The corpse endpoint persists with selected equipment. Exact48 anchor sets, item layers, pivots, dimensions, front/rear/both surfaces, occlusion masks and z ordering are in the appendix.

All2976 legal tuples =3 bodies x16 poses x2 facings x31 slot/item choices retain actual pixels and explicit layering. The independent harness also checks empty slots, replacement, logical hands, condition, worker/detached rejection and twelve H13 overlap/nearby-body fixtures. H13 independently cycles body IDs, idle/run/strike/corpse, both facings, coat/shirt, both armor choices, belt, backpack, gloves, boots, headwear and held equipment; another body's signature remains unchanged. These are source/headless composition observations. Appearance/readability judgment remains04-32.

All nine art controls pass exact diagnostics: missing-pose/baked-equipment→BODY_POSES, wrong-density→BODY_ID_SCALE, false-acceptance→BODY_FALSE_ACCEPTANCE, missing-master→BODY_GENERATION, missing-component→BODY_COMPONENTS, recolored-silhouette→BODY_ROSTER, altered-transform→BODY_MASK, wrong-source-hash→BODY_DONOR_IDENTITY.

All fourteen semantic fit controls pass: density, false-acceptance, slot-identity, missing-source, logical-hand-swap, default-shirt, sway, linear-filter, missing-pose, invalid-anchor, missing-item, empty-fit, wrong-part, missing-H13. Their exact FIT_* diagnostics are saved unchanged in the appendix and invocation.

The two Critical saved cases declare setup/actions/expected results, requirements, acceptance IDs, evidence, fresh owned headless reset, needsGameWindow:false and G0 order26/27. Inherited lifecycle regression also executes all twelve saved headless HUB-NAV loading/error/overflow/long-text and HUB-REGISTER empty/loading/error/populated/partial/overflow/zero-one-many/long-text backstops. Their rendered checkpoints remain pending, not inferred passes.

## Verification and immutable reports

All following reports authenticate tested HEADb7890d1 plus the five exact WIP identities. Original report hashes and paths are recorded in `evidenceAuthentication` of the appendix.

| Exact case | Immutable invocation | Actual outcome |
| --- | --- | --- |
| bodies.a-art | 6593f525-185f-463a-8f92-f4e9ecd20ed0 | PASS;3 bodies,48 poses,193 reproduced contained parts,232 members,nine controls;8102ms |
| bodies.a-fit | 7172b858-5354-4988-979c-d6aba2c92282 | PASS; strict production/harness0warnings/errors,hubPID16660,matrixPID37660,2976 tuples,12H13,5972 observations,10compiled/0skips,14 semantic controls;26567ms |
| source.case-discovery | d484648f-add3-4495-a64a-7544f0a5f75e | PASS;35 rejected discovery faults,four actual CLI children,zero case bodies during import;502ms |
| hub.source | 67c6484c-f8c2-492a-bd89-ae1dc7ac650e | PASS;44 original methods/63compiled/25native observations,PID30292;263343ms |
| hub.lifecycle | f93e9d78-3ced-4169-99fd-0ac4964a6958 | PASS;63compiled/0skips,50native observations,PID24396,12headless backstops;262519ms |
| graph.visibility-inventory | 0bbf32b7-a321-4a08-a158-8eb1b45c8564 | PASS |
| engine.source-policy | cc21a87b-1617-4935-9610-41bb9ff120cb | FAIL;290 exact unauthorized operations:166 BuildingPreview/17methods and124 CharacterPreview/11methods; WINDOWS30 |
| engine.generated-origin | 64fb9418-43b8-407f-a03f-8861107dc806 | PASS; original authenticated origin contracts |
| engine.generated-boundary | ae38be89-41cf-4de9-b64d-42862bca3789 | FAIL;~102sec,not timeout; no exact current source-path/hash cohort at CurrentProducer:751 via VerifyReproduction:698; WINDOWS31 |
| engine.lifecycle | 47b8e40f-7be7-4a66-91f8-85ceeb414167 | PASS; original actual two-native-cycle proof |
| engine.exports | da6fa3b8-3607-44d0-a344-1a2646167b8a | PASS; serialized/null/export proof |
| engine.evidence-integrity | 22439721-acba-4989-a2eb-deb6a652e62a | PASS; missing/stale/duplicate controls |
| engine.core-independence | 295548ae-2350-4ba8-abee-fd4b1e5e819f | PASS; actual Core operations/compiled metadata/assembly inventory |

Production strict buildPID35748 exits0 with0warnings/errors; authored harness buildPID25508 exits0 with0warnings/errors. The authenticated production assembly SHA256 for the final native hub is e282c16fac3828a2a173647c44567f8f3fa8a7a5f2c91fd49e8ffe354788ab4b. Raw native-observation.json, matrix-observation.json and combined fit-observation.json are three distinct immutable files. No overwrite/deletion was used.

Focused complete fit62f06c54-842a-4321-aba5-935ad0e2a42e passed before the fresh selection, with hub8780/matrix31892 and the same2976/12/5972/ten-compiled/fourteen-controls obligations. It is supporting history, not a substitute for the fresh7172b858 result. The shell's final exit0 does not turn the two scenario failures into aggregate acceptance.

Selection is declared in `.phase4/04-06-r4-selection.json`: new/changed body cases, discovery, affected Root hub/lifecycle/native/producer/visibility regressions and native/Core baselines. Explicitly omitted source.provenance has the unchanged inherited WINDOWS29 stale Inspection.tscn binding, owned04-23/31; it receives no pass/skip credit. Unaffected LAN/audio/export/item-art/building-art wrappers and oversized historical whole-class wrappers were not selected. Rendered/listening/physical-input acceptance is separately pending. No complete-catalog or other-OS credit is claimed.

## TDD gate compliance

Genuine native RED169d3a1b-9d74-43b8-a214-dbb252ffb872 built production0warnings/errors, authenticated CSharp hubPID37828/nonce/assembly ac019f9673f9d09520f997b97f4be3e0eafbc5c28df5690bd78f00d518a4f862 and observed no character controls /coatAfterArmorRemoval:null. The compiled Selected_Coat_ReturnsAfterArmorRemoval test ran1/failed1/skipped0, expecting IT-13. TAP `.phase4/04-06-task2-red.json` validates RED_EVIDENCE_OK; the closeout reran the read-only evidence validator and semantically confirmed assertion failure, not build/import failure.

RED was committed765c45d before implementation. Partial GREEN61d6a6ce and later incomplete matrix attempts remain partial history. Complete GREEN is62f06c54 and fresh7172b858, including all expanded compiled subjects with0skips. No separate refactor commit was needed. No RED was fabricated from environment EPERM, compiler errors or an unexecuted target.

## Deviations from Plan and retained repair history

All original failures remain immutable. The [checkpoint](04-06-CHECKPOINT.md) and [bounded repair record](04-06-REPAIR.md) retain exact stages, diagnostics, source versions, native identities and prospective authorization. This closeout changes no implementation bytes.

- **Task1 original3/3:** lazy item-helper import fixed premature case registration; existing60000ms child bound was retained after unsupported120000ms failure; selected head row/largest8-connected component fixed empty/neighbor fragments. Original e38bde03 COMMAND_TIMEOUT_RANGE,7991ed40 BODY_PIXELS and72c88b32 BODY_ATLAS_PIXELS remain failures.
- **R1 atlas1/1:** authoring canvas768x480→768x520 fits193rd local-head component; fresh5b421ecf reaches a distinct missing-master decode failure. No atlas retry reset.
- **R2 hash ordering1/1 resolved:** move the existing exact original/prompt identity check ahead of untrusted image decoding. All nine expected diagnostics and every pixel assertion remain. Artb2aa3f0b/discovery70d4fd0b and committed tracera4ca9e61 pass; Task1 commit2c54e53.
- **Task2 original3/3:** correct ten authored analyzer issues, four formatting issues and the last nested-lambda indentation without suppressions. Partial61d6a6ce passes coat restoration;1ab19852/cbfd68aa/d89bb321 retain formatting/22-error strict-harness failures. d89bb321's production0/0 and hubPID24984 are partial, not full fitting proof.
- **R3 harness2/2 exhausted:** first correct19 ordinal comparisons, one comparer and two invariant formatting sites.7fd2238f production/harness0/0 and hubPID24832 pass, but matrixPID32084 reaches OUTPUT_LIMIT with1050301 stderr bytes and no matrix result. Remaining authorized attempt uses raw FileAccess.GetFileAsBytes plus fresh Image.LoadPngFromBuffer/Error.Ok, retaining identical atlas/rectangle/nearest/per-pixel assertions.0c284ff5 hubPID26116/matrixPID7984 authenticates DLL37aef4b5810e767f713437ecfbb07b7c8b2c353c617f904c032f031298445026 and passes2976/12/5972, then overall fails EEXIST before compiled subjects. Those matrix observations remain partial history.
- **R4 report1/2 resolved:** only three report/orchestration references change, publishing combined fit-observation.json separately and selecting it through CHARACTER_NATIVE_REPORT. Raw hub/matrix reports, exclusive writes, assertions, harness, data and tests remain unchanged. Focused62f06c54 and fresh7172b858 complete the canonical gate.
- **Integration ownership disposition:** canonical04-06 assigns later native adapters to04-23. The parent amendment also assigns final current-producer enrollment to that plan, which now has a ten-path union/four-path Task2. WINDOWS30/31 remain open. No tuple authorization, expectation relaxation, cohort rewrite or acceptance waiver is granted by observed operations.

These are scoped correctness/blocker fixes and explicit bounded continuations, not additional formal tasks or a reset of previous budgets. R4 retains1/2 used; no new retry was consumed during closeout.

## Issues encountered and deferred integration

WINDOWS29 remains open for current index/source-provenance adoption under04-23/31. WINDOWS30 must authenticate the final exact native signatures and independent positive/negative guard controls. WINDOWS31 must enroll one authentic final current producer cohort with retained producerSources, historical replay and original negative controls. All three must be resolved before04-24 independent review and04-32 actual acceptance. Repairs require fresh affected proof and an entire justified source/headless selection; passing generated-origin/current capture/native controls cannot replace the currently failed replay.

Pre-invocation sandbox gitEPERM and closeout Git index permission denial were environment restrictions, not assertion failures or RED. Equivalent approved elevated commands preserved the same inputs and normal hooks. No timeout/output-limit increase, package install, blanket policy, warning suppression, unrelated kill, branch switch, stash/reset/merge/rebase or push occurred.

## Known stubs and threat surface

No placeholder production component, skipped compiled subject or stub preventing this local production goal was found. Intentional null acceptance/listener fields and empty completedObligations preserve pending acceptance; empty equipment/absent layer states are functional choices, not fabricated content. Unsupported fit tuples reject explicitly rather than supplying a default fit.

The new local JSON/native composition surface is already covered by T-04-16/T-04-17/T-04-18 and T-04-SC: bounded definitions/depth/count/finite values, exact source identity, guarded synchronous ownership, immutable evidence and no new package. No additional network endpoint, authentication path, external service or schema trust boundary was introduced. Full final permission/producer compliance remains failed as stated above.

## Decisions made

Keep low-resolution donor-like proportions and source density1, with D-13 taking precedence over richer historical sprite examples. Keep clothing/belts/bags fixed to their anchors without secondary sway, use authored collapse/corpse poses, preserve actual selected torso/equipment and related distinct undead silhouettes. Final readable appearance is still a separate judgment.

Complete local04-06 production under the explicit parent integration disposition while retaining every failed selected result and pending acceptance. Keep exact observations separate from prospective policy permissions. Preserve foreign global planning/spec work for coordinator reconciliation.

## User setup required

None. No external service configuration or additional installation is required.

## Next phase readiness

Committing this canonical summary permits the parent to reconcile **12→13 of37 local Phase4 plans**. Remaining ready wave12 plans are04-14 and04-18; this executor starts neither. Formal37plans/74tasks/31waves and distinct phase source inventory234 are unchanged.

The parent owns STATE/ROADMAP/progress reconciliation, continuation, push and existing draftPR5 on feature/04-asset-production targeting dev. This closeout deliberately leaves foreign shared planning/spec WIP untouched and unstaged.

ASSET-01/02/03 contributions are local production only; all ASSET requirements remain pending. Entire phase source/headless implementation, resolution of WINDOWS29/30/31, independent code review/fixes and then actual Windows1280x720/no-VM04-32/33 appearance/listening/physical-input acceptance remain required. Listener is null. No game window, actual listening, physical input or final human judgment was performed by this closeout.

## Self-Check: PASSED

All eight implementation files, the scoped artifact inventory and canonical summary exist. Task1, genuine RED and Task2 source commits are ancestors of HEAD. All five live and backup hashes still match the frozen manifest after the Task2 commit; their working-tree diff is empty. The inventory parses with232 exact archive members and290 observed native operations. Persisted-ledger measurement is9 commits through531573e. No tracked file deletion occurred. Original proof reports authenticate the recorded11PASS/2FAIL selection and testedb7890d1 revision; this self-check supplies no new execution, final policy or appearance credit.
