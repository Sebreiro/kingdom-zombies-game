---
phase: 04-asset-production
plan: "14"
subsystem: native-ui-kit-and-typography
tags: [Godot, CSharp, native-theme, packaged-fonts, retained-provenance, measured-layout]
status: complete
requires:
  - phase: 04-05
    provides: Actual ordinary resource identities and preview resources
  - phase: 04-26
    provides: Guarded inspection shell, input, transport and owned native lifetimes
provides:
  - Four authenticated recovered wood frame states and three packaged font resources
  - Reusable native Theme with explicit glyph fallback, control states and independent focus/selection
  - Source, compiled and native headless evidence over twelve logical canvas/scale/text combinations
affects: [04-15, 04-16, 04-17, 04-22, 04-23, 04-24, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [packaged FontFile fallback, tiled pixel corners, measured native text wrapping, full viewport containment, frozen input selection]
key-files:
  created:
    - src/godot-game/assets/ui/LiberationSans.ttf
    - src/godot-game/assets/ui/PerfectDOSVGA437.ttf
    - src/godot-game/assets/ui/Fallback.ttf
    - src/godot-game/content/packs/ui-kit.json
    - tools/phase4/cases/ui-kit.cjs
    - src/godot-game/assets/ui/InspectionTheme.tres
    - tests/phase1/ThemeLayoutTests.cs
  modified:
    - src/godot-game/inspection/Inspection.tscn
    - tools/phase4/cases/hub.cjs
key-decisions:
  - "Use exact inspected Liberation Sans for body and packaged fallback, with Perfect DOS VGA437 for short display accents; do not imply universal Unicode coverage."
  - "Measure native unwrapped paragraph widths and actual Label line counts before requiring wrapping; require full two-axis containment after legal scrolling."
  - "Apply the same acceptance predicates to native deliberate tiny-font, clipping and invalid/double-scale controls and restored positives."
  - "Close only the local UI production slice; current pack integration, final native permissions/producer proof and actual appearance/input acceptance retain their existing owners."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-04, ASSET-05]
coverage:
  - id: exact-ui-kit-source
    description: Authenticated packaged font roles and recovered wood pixels with independent negative controls
    verification:
      - kind: integration
        ref: "layouts.kit /4ada4b59-3f9e-4eed-992a-0cfc90d051ed"
        status: pass
    human_judgment: false
  - id: native-theme-layout
    description: Actual Theme, fonts, states, focus, wrapping, local scrolling and one UI scale
    verification:
      - kind: integration
        ref: "layouts.theme /707d6fc3-dd07-4917-b920-108c92e9fc21"
        status: pass
    human_judgment: false
  - id: original-hub-regressions
    description: Original compiled loader controls and native source/lifecycle observations with exact UI closure
    verification:
      - kind: integration
        ref: "hub.source /641f4412-4303-4f04-b690-7f7a2b340531"
        status: pass
      - kind: integration
        ref: "hub.lifecycle /5911a871-7c21-4ebc-8031-62e3c4ae3c17"
        status: pass
    human_judgment: false
  - id: actual-ui-readability-and-input
    description: Final appearance, readability, supported displays and physical input
    verification: []
    human_judgment: true
    rationale: "Requires complete phase implementation, independent review/fixes and actual04-32/33 Windows1280x720/no-VM acceptance; headless logical canvases do not supply this evidence."
actuals:
  tokens: 286629
  tasks: 2
  commits: 5
  diff_chars: 1146515
  scale: "ceil(chars/4) over ordinary git diff --no-ext-diff for the nine implementation paths; binary notices counted, not font file bytes or harness tokens"
commits: 5
plan_head_before: 04764197c538b5c0f5d27d8a7bdb36ff4de0e851
plan_head_after: a05fff36f484cd86a74b3387f4d80bfffc0b5ce8
duration: 27min
duration_scope: "Observed retained evidence03:50:54.641Z to Task2 commit04:17:38.663Z; initial font authoring prelude unmeasured"
completed: 2026-11-17
---

# Phase 04 Plan 14: Native UI kit, packaged fonts and reusable theme Summary

**Recovered wooden native-control frames, exact packaged sans/pixel fonts, explicit fallback and a reusable Theme, proved with real shaped text and native geometry.**

Both local tasks are complete. The fresh justified five-case source/headless selection passes on frozen inputs. This contributes UI-01/UI-02 and AC-10/AC-11 implementation evidence; it does not complete the remaining UI layouts, English catalog, all ASSET requirements, final full-hub integration, review or actual04-32/33 acceptance.

## Performance and task commits

1. Task1: `ed5c404` — package exact wood kit and inspected font roles. Initial source invocation `a0669219-2511-416d-9006-d5d75a3d949d` passes; mandatory postcommit tracer `6d064e3a-0a55-4757-a676-819115b52b92` passes on `ed5c404`.
2. Genuine compiled RED: `6a8cfaf` — one executed Scene_AppliesPackagedTheme assertion fails, zero skips, because the real scene lacks the packaged theme. Exact report: `.phase4/04-14-red/tests.json`. The preceding comment-length build failure remains invalid RED history.
3. Task2: `a05fff3` — real reusable native Theme, actual scene bindings, three compiled tests, complete native layout proof and approved original hub closure amendment.

Checkpoint `d378755` and prospective repair authority `72d9087` are included in the ledger-measured five commits from `0476419` through Task2. The SUMMARY commit follows that measured interval. No tracked files were deleted. Nine distinct implementation paths match the eight canonical paths plus the preapproved existing hub.cjs amendment; Task2 has exactly five paths. Distinct phase implementation inventory remains234.

## Verification and exact source identity

All invocations below live at `.phase4/runs/{nonce}/`, with immutable invocation/result/command/exit/stdout/stderr records and distinct evidence filenames. They were freshly executed sequentially with one canonical `--case` per command. Tested HEAD is `72d9087a325cb47f2ff03abb56c36f8a8b2ed745` plus exact WIP, subsequently committed unchanged as `a05fff3`; they are not executions on that later commit.

| Order | Canonical case | Invocation | Result and actual observations | Duration |
| --- | --- | --- | --- | --- |
| 1 | layouts.kit | 4ada4b59-3f9e-4eed-992a-0cfc90d051ed | PASS; three font replays, four exact RGBA wood regions,14 controls/nine states,11 semantic/malformed-font rejections | 393ms |
| 2 | layouts.theme | 707d6fc3-dd07-4917-b920-108c92e9fc21 | PASS; strict production/harness0warnings/0errors;3 discovered/3 compiled passes,0skips;172/172 native assertions, PID9876 | 14493ms |
| 3 | source.case-discovery | de0350c1-d4c3-4f3f-acd0-a87e9edaf8c6 | PASS;23 catalog cases,35 rejection controls,four real CLI children,zero case bodies executed on import | 494ms |
| 4 | hub.source | 641f4412-4303-4f04-b690-7f7a2b340531 | PASS; original44 methods/63 compiled passes,0skips;25/25 native assertions, PID32188 | 265425ms |
| 5 | hub.lifecycle | 5911a871-7c21-4ebc-8031-62e3c4ae3c17 | PASS; original44 methods/63 compiled passes,0skips;50/50 native assertions, PID21052;12 saved backstops retained | 264809ms |

Entire selected elapsed window is583901ms (9.732min),04:07:15.279Z–04:16:59.180Z. Separate launches preserve the canonical compiled and isolated native subjects; all use source/headless processes and zero real game windows. The focused repaired theme invocation `94356ba1-23c8-4146-992c-8e72be8b9617` also passes172native/3compiled, PID36120, before this complete selection. Its strict production build took11.750s and harness3.407s; final selected theme builds took1.507s and3.533s. No independently established cold-cache measurement is claimed.

The final freeze authentication at04:17:14.804Z found **zero drift across2462 exact files** throughout the selection. Freeze and aggregate: `.phase4/checkpoints/04-14-native-layout/r1-frozen-inputs.json` and `r1-final-selection.json`. Metadata updates after that terminal authentication are separate from accepted-run inputs.

| Task2 implementation path | Accepted SHA256, identical in Task2 commit |
| --- | --- |
| src/godot-game/assets/ui/InspectionTheme.tres | b29205230d3293caa8c3c2c74c4e63348c42a28eb1bed0c1ea51d52c60ab089a |
| src/godot-game/inspection/Inspection.tscn | 6eacdca2448238b0234f61ae14ea3049db0f320aff3ecce7c330c31f70f448b3 |
| tests/phase1/ThemeLayoutTests.cs | fdb259c1ae61a7f91e0fc67756c8d750e25589e9134b2d2cf93ee29048ca0470 |
| tools/phase4/cases/ui-kit.cjs | c965b42c0f69da13e979220260d32a31bef16c1e78e5198e72d6059849b789e2 |
| tools/phase4/cases/hub.cjs | 8774b455ac8e496363483f08fe016b366ea125db7db2e5bca6d6f8532c937c84 |

Native theme proof authenticates the pinned official archive and its actual non-console executable entry, child PID/nonce, exact production DLL and owning AssemblyLoadContext. Accepted DLL SHA256 is `99f166e3a217615a046159199e9f661e1e61be733f72b92d8bdd8a947ca22687`. Actual source and lifecycle subjects preserve original archive/native PID/DLL checks and independently compare theme, three fonts and common atlas in source and copied subject before/after. Registration remains lazy and side-effect-free, as the original discovery controls prove.

## Measured layout and independent controls

Every original165 outcome ID is preserved as the exact ordered prefix of the172 native outcomes. All twelve original logical canvas/scale/text combinations remain:1280x720 and1920x1080, each at0.8/1/1.5, with normal and doubled text. These are authored logical headless projections, not physical-display evidence.

| Logical canvas | Scale | Available label width | Normal lines | Doubled lines |
| --- | --- | --- | --- | --- |
| 1280x720 | 0.8 | 1488 | 1 | 2 |
| 1280x720 | 1 | 1168 | 2 | 3 |
| 1280x720 | 1.5 | 741 | 2 | 4 |
| 1920x1080 | 0.8 | 2320 | 1 | 2 |
| 1920x1080 | 1 | 1808 | 1 | 2 |
| 1920x1080 | 1.5 | 1168 | 2 | 3 |

Native full-string shaping reports1426px normal and2855px doubled at font16. Native line heights are19/43/67/91 for one/two/three/four lines, with actual minimum-height and visible-line checks. Explicit two-paragraph newline text at width1168 yields four actual visible lines/91px. Local pinned GodotSharp4.7.2 XML signatures/documentation were inspected for Font.GetStringSize and Label.GetLineCount/GetVisibleLineCount/GetLineHeight; no version-dependent API was guessed.

At1920/0.8 the normal content height1238 fits the actual scroll size1286; MaxValue/Page1238/1238 gives legal range0 and scroll0. The final target is wholly contained. Other overflowing layouts retain legal positive scrolling and full target containment in both axes with the original1px geometry tolerance. No artificial overflow or increased tolerance was introduced.

TextAccepts, Reachable and ScaleAccepts serve both original positives and actual native deliberate defects. Font8 is rejected by TextAccepts then font16 restoration passes; moving the actual Toast outside X then Y is rejected by Reachable then restored containment passes; actual scale2.25 requested1.5 and unsupported1.2 are rejected by ScaleAccepts then1.5 restoration passes. World Sprite2D scale remains1. Original font/style/fallback/focus/selection/corner/hash assertions remain intact. Focus uses a transparent outer4px accent border; selection uses a separate inset4px marker without layout change. Text contrast ratios against background/secondary/hover/pressed/disabled are13.98194/9.23622/7.03991/12.29773/11.41890, all above4.5; essential accent boundary contrast also passes its3 threshold and deliberate low-contrast rejection.

## Artifacts this phase produces

| Exact implementation file | Actual responsibility/inventory |
| --- | --- |
| src/godot-game/assets/ui/LiberationSans.ttf | Exact350200-byte resources.assets Font6004, body role,2294 mapped codepoints; SHA256 e5b0af421ea2bfbc1ac8d251d647268087ae82786234c57f757d1f0b90fa8b49 |
| src/godot-game/assets/ui/PerfectDOSVGA437.ttf | Exact81192-byte resources.assets Font6003, display accent role,256 mapped codepoints; SHA256 ab5f7798ce9d05aa09380fce2142b11690ba3216a73208954f87c6c63a035102 |
| src/godot-game/assets/ui/Fallback.ttf | Independently packaged identical inspected Font6004 bytes/hash/coverage; explicit fallback for both roles; no system fallback or universal Unicode claim |
| src/godot-game/content/packs/ui-kit.json | ui-kit/version1/sampleDataOnly, UI-01/UI-02, exact fonts/four inspected candidates/wood origins/atlas/bundle, palette/spacing/typography/control/state/focus/fixtures/scale contract, null approval and empty completedObligations; SHA256 cdfbbf4880fdcc14d200dcb3df0b4ac173fec4a30c65d15a6736a7d30591341a |
| src/godot-game/assets/ui/InspectionTheme.tres | Actual1058195-byte native Theme;15 subresources and one existing atlas external resource; no script/native permission introduced |
| src/godot-game/inspection/Inspection.tscn | Exact external Theme3_theme binding on Summary, Shell and reusable hidden UiKit fixture subtree; original foundation/world bow registration retained |
| tests/phase1/ThemeLayoutTests.cs | Public sealed ThemeLayoutTests; public void Scene_AppliesPackagedTheme(), Fonts_DeclarePackagedFallbackAndMissingDiagnostic(), Scales_KeepWorldZoomIndependent(); private static JsonDocument ReadPack() |
| tools/phase4/cases/ui-kit.cjs | Saved layouts.kit/layouts.theme cases, bounded font parser/source replay, theme/fixture recipe, strict compiled/native evidence and measured positive/negative predicates |
| tools/phase4/cases/hub.cjs | Approved existing native(context,lifecycle=false) resource closure refinement: copy assets/ui and authenticate Theme/three fonts/common atlas, preserving original compiled/source/lifecycle controls |

Selected fonts come from the already retained `production/phase4/common-selected.zip`, SHA256 `2f66618305221d32d503bba04be755ae38e606408a7c5d9d8f65a1db19d9e6d9`; all2395 existing members remain intact, with no new archive/install/download. Selected members are `fonts/resources.assets/6004.ttf`, `fonts/resources.assets/6003.ttf` and their `raw/resources.assets/6004.bin`, `raw/resources.assets/6003.bin` origins. Exact resource owner hash is `878915bf9ed642977abf5712bb86f804c8b59c1e8651d888816813c89bace91c`; raw6004/6003 hashes are `0f7348e1bd02b92c30482b56976163235ec1880cf57022ddaf54df35cf5b1738` / `c070d545443d85fe927c1773e8c8cf0bb9aa1617e83cdbbfc82cb40d4494fa92`.

All four inspected candidates' embedded name records and cmap identities are retained in ui-kit.json. Liberation Sans2.00.1 records Google2010/Red Hat2012 copyright, Ascender/Steve Matteson attribution and SIL Open Font License1.1. Perfect DOS VGA437 records2003/1.0, zeh and fatorcaos.com.br attribution/URL, without an embedded license statement; extraction does not imply permission or invented terms. Unselected Kingdom Font558 has254 mapped codepoints/seven fixture gaps; KingdomMenu564 has198/25 gaps; their embedded noio/FontStruct/Creative Commons Attribution records remain candidate evidence. Liberation/pixel/fallback coverage is2294/256/2294; explicit native packaged fallback routes U+0100, and unsupported U+10FFFF produces the exact missing-glyph diagnostic. Same-byte Liberation fallback does not expand its own repertoire.

Existing atlas `assets/common/donor-kit.png` is2048x2763, SHA256 `bf3b0b91be9b772184f034d8e681e4a8b21210cfa5197aac2c03fc773c4b88d4`. All selected wood sources are Sprite records in sharedassets0.assets, owner hash `3648658d475170661517c4907e50977a4ab932441b404a681cd0ce485592f3ff`:

| State | Stable ID/name | Atlas rectangle | Retained selected members |
| --- | --- | --- | --- |
| normal | common.s0.7837 /menu_button_wood_big_normal | 136,1159,62,25 | images/sharedassets0.assets/7837.png; raw/sharedassets0.assets/7837.bin |
| hover | common.s0.7900 /menu_button_wood_big_highlight | 311,1159,62,25 | images/sharedassets0.assets/7900.png; raw/sharedassets0.assets/7900.bin |
| pressed | common.s0.6925 /menu_button_wood_big_pressed | 1212,1089,62,25 | images/sharedassets0.assets/6925.png; raw/sharedassets0.assets/6925.bin |
| disabled | common.s0.8151 /menu_button_wood_disabled | 852,1159,28,25 | images/sharedassets0.assets/8151.png; raw/sharedassets0.assets/8151.bin |

Exact source/raw/PNG/RGBA hashes and recipes remain in ui-kit.json/common.json and the retained archive. All four frames use8px corners, tiled edges, no drawn texture center and16x8 content margins; quiet native dark backgrounds keep essential text out of textures.

Theme external ID: `wood` (existing common atlas). FontFile subresources: `Fallback`, `Body`, `Pixel`; FontVariation: `Bold` (Body +0.5 embolden,700 role); StyleBoxTexture: `wood_normal`, `wood_hover`, `wood_pressed`, `wood_disabled`; StyleBoxFlat: `quiet`, `secondary`, `hover`, `pressed`, `disabled`, `focus`, `selected`. Theme variations are Body16/400/1.5, ControlLabel20/400/1.5, Heading24/700/1.2, Display32/400/1.2 and SelectedButton. Spacing is4/8/16/24/32/48/64. Native control inventory: Button, PanelContainer, TabBar, ItemList, HScrollBar, VScrollBar, LineEdit, TextEdit, HSlider, VSlider, CheckBox, TooltipPanel, PopupPanel, ToastPanel. Saved states: normal, hover, focus, pressed, disabled, selected, empty, loading, error.

New scriptless node paths under `Inspection/UiKit`: Backdrop, Scroll, Scroll/Content; under Content: Display, Heading, Body, Normal, Selected, Disabled, DisabledReason, Tabs, List, Input, Check, Slider, VerticalSlider, Empty, Loading, Error, LongText, LongId, Tooltip/Text, Modal/Text, Toast/Text. UiKit is a reusable fixture, not a claim that final current-pack preview dispatch is complete;04-23 owns that integration.

Authored CommonJS symbols/signatures: fingerprint(files), fontInfo(data), diagnose(text,fonts), authorKit(), validateKit(pack,read), async kit(context), color(hex,alpha=1), authorTheme(), kitScene(), isolated(), async compiled(context), async nativeTheme(context), contrast(a,b), async theme(context). Constants: PYTHON, PACK, FONT_FILES, KIT_FILES, FIXTURE, SOURCE_VERIFY, THEME, SCENE, THEME_FILES, HARNESS. Local helpers: fontInfo.range(offset,count)/u16(offset)/u32(offset), kitScene.add(name,type,props), contrast.luminance(x); anonymous parser/authoring/control callbacks are local expressions. Exports: fontInfo, diagnose, FIXTURE, FONT_FILES, KIT_FILES, fingerprint, authorKit, validateKit, kit, THEME, SCENE, THEME_FILES, authorTheme, kitScene, isolated, HARNESS, compiled, nativeTheme, contrast, theme. color remains internal. Python SOURCE_VERIFY is retained as a string recipe and emitted separately per invocation; it replays selected font bytes and exact frame pixels from the retained bundle without the recovered root.

HARNESS emits invocation-local `KingdomZombies.ThemeProof.ThemeHarness : Node`, public sealed partial; fields private readonly List<object> _outcomes and private bool _failed. Members: private void Check(string id,bool pass,object observed); private static string Digest(byte[] data); private static bool TextAccepts(Label label,Control viewport,out object observed); private static bool Reachable(ScrollContainer scroll,Control content,Control last,out object observed); private static bool ScaleAccepts(Control kit,Sprite2D world,float scale,out object observed); public override async void _Ready(); private async Task Run(). This is an isolated proof subject, not production engine authorization. **New production native-policy tuples: none**; no production C# native member, exemption, generated register, permission or public API was added by this plan.

## Saved scenarios, backstops and selection limits

layouts.kit and layouts.theme are High: missing glyph/control states, clipping or unreadable identities break the reusable inspection flow. Saved setup/actions/expected/evidence/reset/importance/rationale live in their actual registrations, G0 order30/31, no game window. The final source order is the five-case table above. Original Critical hub.source/hub.lifecycle keep G0-hub order1/2, their complete original setup and boundary controls, owned cleanup and12 lifecycle backstops. Discovery proves module import is side-effect-free. UI inputs select both original hub regressions through shared Theme/scene/font/atlas dependencies.

Separately tagged headless backstops remain H19 (six canvas/scale combinations), H22 (expanded text/wrapping), H24 (glyph diagnostic), UI-FORMS-CONTROLS/long-text (final control reachability), UI-TEXT-STATUS/error (full error identity). Their actual rendered checkpoints remain pending04-32/33. All original hub backstops retain headless=true/rendered=pending04-32 in final evidence. No image review, actual listening, listener identity, physical input, supported-display certification or real-window readability judgment is supplied; listener remains null. Existing unrelated scenarios were not selected for this bounded proof repair and receive no pass/skip credit. Whole-phase implementation/review/fixes precede the later saved Windows1280x720/no-VM game-run selection.

## Deviations from Plan and retained failures

1. **[Rule3 — approved blocking resource closure]** Existing hub.cjs initially copied only assets/common. The prospective04-14-AMENDMENTS authorized only its assets/ui copy and exact Theme/three-font/common-atlas identity checks before the edit. No original method/control/native observation was removed and no deadline or permission changed.
2. **[Rule1 — proof predicate correction, prospective R1]** Three normal strings were incorrectly required to occupy two lines despite fitting their actual native widths; two1920/0.8 targets were incorrectly required to overflow despite already being contained.04-14-REPAIR prospectively authorized proof-only correction, maximum two attempts. First R1 repair passes; no second correction was needed. Production Theme/scene/fonts/kit/tests/hub were not edited during R1. Same-predicate native deliberate defects and restorations supplement all165 original IDs.

Ordinary Task2 correction3/3 was already exhausted before R1. Preserve initial invalid comment-length RED build; genuine compiled RED6a8cfaf; failed GREEN `1a114845-d8e4-4f38-85dd-eeff8b481e07` (JSON overload/comment); failed `38bcd431-2cd0-4e9b-8bd8-0bbd585dda3b` (five authored-harness analyzer errors); failed `81f976e0-614b-4bb9-9f41-b2d339b2f176` (production/harness0warnings/errors,3compiled0skips,160/165native). No historical failure was overwritten or relabeled. Five preserved WIP/backups were authenticated before R1 against `.phase4/checkpoints/04-14-native-layout/manifest.json`; the repaired proof hash is explicitly different. Environmental `e4c4012a-7e9a-4c3a-af64-d90808bcac1e` records Git spawn EPERM; equivalent escalated execution changed no source/expectation and supplies its own actual proof. No install, new package, scope expansion, tolerance increase, suppression, budget reset or real game window occurred.

## Known stubs and open integration gates

No unfinished stub prevents this local kit/theme slice. LineEdit placeholder_text is actual input guidance; empty collections in proof code are runtime accumulators; sample empty/loading/error states are intentional Phase4 resource fixtures. Later gameplay transactions are outside this resource-only slice.

Existing WINDOWS29 (stale tracer scene/source.provenance and current pack adoption), WINDOWS30 (290 unapproved native operations in BuildingPreview/CharacterPreview), and WINDOWS31 (missing authentic final current producer cohort) remain OPEN. Their exact failed evidence and04-23/31 ownership remain in the current-pack/policy integration amendments and WINDOWS.md. These unchanged failures were not rerun or granted exemptions here. Current index still has180 expected identities, one partial bow,28 pending rows and zero completed obligations. No ASSET requirement is marked complete. Complete root dispatch/index adoption, final permission/producer proofs, independent04-24 review/fixes and04-32/33 actual acceptance remain required.

Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE, initial-spec, Phase06 discussion/SPEC and untracked Phase12 work remain preserved and unstaged. The coordinator owns global13→14 progress reconciliation, session continuation and draftPR5 updates. This executor changes no global state pointer or requirement status.

## Self-Check: PASSED

All nine implementation files and canonical SUMMARY exist. All five ledger-window commits are reachable from current HEAD. Every accepted implementation hash matches both disk and Task2 commit; no accepted source was changed after execution. Actual five-case aggregate passes, original165 ordered outcome IDs/twelve combinations remain, and the measured ledger count is5. Stub/threat scan found no unfinished local goal blocker or new trust-boundary surface; existing separately owned integration failures remain explicit above.
