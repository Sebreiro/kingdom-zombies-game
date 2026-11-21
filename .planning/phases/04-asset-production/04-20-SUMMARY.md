---
phase: 04-asset-production
plan: "20"
subsystem: content-data
tags: [json, nodejs, independent-semantic-validation, source-headless]
requires:
  - phase: 04-19
    provides: Current economy/world authoring defaults, source crosswalks and explicit integration comparisons
  - phase: 04-17
    provides: Session/save/settings inspection resources and authenticated headless regressions
  - phase: 04-18
    provides: Bounded strict parser, canonical item/combat/inventory definitions and source-validation helpers
  - phase: 04-13
    provides: Actual map, portal/effect resources and guarded world inspection dependencies
provides:
  - Seven-archetype zombie, complete quality/optional loot and independently editable progression definitions
  - Portal/assault, session/save, presentation and separate local-preference defaults
  - 1855 documented field identities with independently verified semantic units and exact source/default controls
  - Two saved data cases,319 independently closed authored findings and fresh45-case source/headless evidence
affects: [04-21, 04-22, 04-23, 04-24, 04-29, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [explicit-field-inventory, independent-literal-unit-catalog, source-bound-default-validation, preserved-old-fail-new-pass-evidence]
key-files:
  created:
    - src/godot-game/content/config/zombies.json
    - src/godot-game/content/config/zombie-loot.json
    - src/godot-game/content/config/zombie-progression.json
    - src/godot-game/content/schema/fields-zombies.json
    - tools/phase4/cases/data-campaign.cjs
    - src/godot-game/content/config/portals-assaults.json
    - src/godot-game/content/config/session-save.json
    - src/godot-game/content/config/presentation.json
    - src/godot-game/content/config/local-preferences.json
  modified: []
key-decisions:
  - Validate all1855 units against an explicit source-reviewed field-ID catalog independent of authoring metadata and unit classification.
  - Bind known death-effect IDs to actual semantic effect obligations, rejecting valid IDs with the wrong meaning.
  - Author the required20second cosmetic stain lifetime independently of the existing8second producer and retain its separate nonpassing integration comparison.
  - Validate declared AU/event contracts while explicitly preserving physical audio21/text22 and consumer23 availability limits.
requirements-addressed: [ASSET-02, ASSET-04]
requirements-completed: []
actuals:
  tokens: 575023
  tasks: 2
  commits: 6
commits: 6
plan_head_before: 524b311af9f078392ec11b91db61703d1ff29c60
plan_head_after: ed8a79c78a986c18580a4f26ef8075ee967e7bff
duration: "78.17min"
duration_scope: "First saved product failure01:25:36.466Z through repaired selection02:43:46.932Z, including checkpoint/coordinator interval; earlier authoring and subsequent metadata closeout excluded"
completed: 2026-11-18
status: complete
---

# Phase 04 Plan 20: Zombie, campaign, session and preference authoring summary

**Seven zombie archetypes, complete loot/progression and remaining campaign/session/presentation defaults now have1855 documented fields, independent semantic validation and a fresh45-case source/headless pass.**

This completes the approved authoring slice after the prospectively scoped R1 repair. The unchanged historical `04-20-CHECKPOINT.md` is resolved by this canonical summary; its original failures, budgets and raw evidence remain preserved. It was a technical authoring checkpoint, reclassified by `04-20-REPAIR.md` at coordinator commit `ca6613d`, not a missing human credential or appearance judgment.

These are editable definitions and inspection fixtures. Campaign, loot generation, session/save gameplay and a live balance editor are not delivered by these files. AC-12/13/14 source responsibilities are supplied here; physical sound/text, actual integration and full-phase acceptance retain their separate owners. No ASSET requirement is marked complete.

## Delivered authoring data

- `zombies.json` contains all seven source stat/attack/multiplier rows, common stamina/regeneration/reach/targets, actual equipment/ammunition rules, body/effect references and the non-damaging radius3/lifetime20 bloater definition. Normal death uses FX-08/`character.death`; bloater uses FX-07/`bloater.burst` and its stain family.
- `zombie-loot.json` contains every Q0–Q4 money/weapon/armor/plated/helmet/backpack/condition row, every optional chance/quantity/coat/second-weapon/backpack-share rule, guaranteed money/trousers/bow/arrows/armor and legal-capacity policy. Eighty actual guaranteed-loadout fixtures cover archetypes, quality and range endpoints. All original fixture contents are preserved exactly.
- `zombie-progression.json` remains separate:3day intervals, all five day/count/retaliation/quality/variant-weight rows totaling100, cap300, queue priority/cancellation, nightfall release and dawn retreat/persistence rules. Living stats and player count do not drive escalation.
- `portals-assaults.json` contains2500/5000/8000health,6/10/14defenders, stages0/1/2, caps40/60/80, warning/release/cooldown rules and rally25money, size8/mix4+4, one-per-side/free-recall/distance/return policies.
- `session-save.json` contains four participants,10second response/30second join defaults, stable identity and service separation, host authority, coherent saved state and compatibility, latest3autosaves/unlimited manual saves, failed-save choices and changed-host-settings commitments.
- `presentation.json` contains80u/0.15second camera, minimap/UI parameters, complete current effect/color/light profiles, source-required20second stain, thresholds, preview controls,30declared sound contracts, buses and24/four limits. Palette/gain choices remain configurable production defaults rather than creator-selected exact hues.
- `local-preferences.json` separates80/80/60audio, display/UI scale80–150%, complete remappable bindings and reset behavior from host balance.

The schema documents every field's identity, meaning, unit, default, allowed values, source and dependencies. The three zombie domains use `fields-zombies.json`; the other four retain structured inline field documentation. Presentation is **1,045,063bytes**,3,513bytes below the unchanged1MiB limit. No byte/depth/node/count/string/encoding bound was raised.

## Task commits and measured cost

| Outcome | Commit | Verification |
| --- | --- | --- |
| Original Task1 authoring | `767490c` | Original GREEN and fresh committed same-case tracer retained |
| Original Task2 authoring | `8f0eddc` | Original GREEN and diagnostic45 retained as history |
| Durable incomplete checkpoint | `d42bf89` |615checkpoint-integrity checks retained |
| Prospective coherent repair scope | `ca6613d` | Original budgets preserved; exact R1.1/R1.2/R1.3 scope |
| R1.1 repaired units/effect bindings and independent catalog | `745ea75` | GREEN `081bcd00`; committed tracer `fe9bcd80` before R1.2 |
| R1.2 repaired presentation/local/portal units and lifetime | `ed8a79c` | Initial GREEN `bc9c9485`; exact319closure audit; fresh45 selection |

The persisted plan ledger measures **six ancestry commits**, from `524b311af9f078392ec11b91db61703d1ff29c60` to `ed8a79c78a986c18580a4f26ef8075ee967e7bff`, including the checkpoint and coordinator scope commits. This summary's metadata commit is separate. `actuals.tokens` is the measured2,300,092character realized nine-file diff divided by4, rounded upward; it is not a harness token count. Exact metrics are in `.phase4/04-20-r1-final/metrics.json` and `completion-metrics.json`.

## Independent repair and preserved attempts

The original implementation passed mirrored checks but failed independent review. The immutable audits identify **319 authored findings**:311unit labels, seven known-but-wrong FX IDs and one authored stain duration. All319 are now independently closed on actual repaired bytes.

The new `SEMANTIC_UNIT_GROUPS` literal enumerates all1855 exact field identities; `SEMANTIC_UNITS` is its lookup. It was constructed from raw data node kinds and source-reviewed numeric semantics before production repair, without taking unit expectations from metadata or the authoring classifier. The scope comparison proved exactly311 authorized differences. All1544 previously correct labels remain unchanged, including quantities, probabilities, time, distance, booleans, IDs, structures and ordinal day counts. The explicit catalog includes ratios, pixels, font-weight codes, normalized lighting time, money and regeneration rates.

`semanticEffects()` independently binds each archetype to the required actual effect obligation. Valid-but-wrong FX-02/FX-11 and other known inappropriate identities fail `EFFECT_SEMANTICS`; mere membership in the known obligation set is insufficient. `semanticStain()` requires20seconds, independently of the old producer. Source§9.1 and the stain field's updated rationale document that obligation and the existing8second mismatch.

Before correcting production metadata, the strengthened functions actually rejected preserved old inputs at intended diagnostics:

- R1.1 old unit: `UNIT_SEMANTICS`, zombies `/common/regenerationPerSecond`; old FX: `EFFECT_SEMANTICS`, `/archetypes/0/deathEffect`. Records: `.phase4/04-20-r1/r1-1-red.json`.
- R1.2 old money unit: `UNIT_SEMANTICS`, portals `/rally/price`; old duration: `STAIN_LIFETIME`, presentation `/effects/profiles/31/durationSeconds`. Records: `r1-2-red.json`.
- At final source `ed8a79c`, invocation **`b7e3fd03-e26c-467a-b067-acdfa4b297b7`** substituted every one of the319 preserved original bad values into immutable copies, required exact file/field/reason rejection, then accepted restored actual production values. Full hashes, HEAD, timestamps and diagnostics are in `.phase4/04-20-r1-final/old-fail-new-pass.json`.

Every1855field identity has a wrong-unit rejection control; seven known-valid wrong effects and8second stain rejection have independent controls and restored positives. All **1515 original control identities** remain:484Task1 and1031Task2. The original `metadata-unit` control now reaches the stronger direct `UNIT_SEMANTICS` diagnostic. Current cases record1069/2311 controls, including restoration positives. `original-controls-preserved.json` confirms zero missing original names and exact preservation of all80guaranteed-loadout contents.

### Budgets remain separate

| Scope | Corrections used | Preserved outcome |
| --- | --- | --- |
| Original Task1 |1/3 | Exact diagnostic-path oracle correction; no reset |
| Original Task2 |3/3, exhausted | Metadata size, UI-scale bounds, font-weight bounds; no fourth original correction |
| R1.1 |1/2 | Initial catalog construction incorrectly proposed elapsed days for ordinal `originDay`; scope assertion caught it before production edits, and the original count classification was retained |
| R1.2 |0/2 | Initial repaired verification passed |
| R1.3 |0/2 | Initial exact closure, fresh whole selection and final integrity checks passed |

Original failure invocations are preserved: Task1 `5f16c5d5` (`CONTROL_ORACLE`); Task2 `1f9a3cd3` (`SIZE`), `c01b85f5` (UI-scale `BOUNDS`), `24a0a0ca` (font-weight `BOUNDS`). Original GREEN/tracer and diagnostic evidence remain unchanged. Sandbox `spawnSync git EPERM` occurred before run creation and was environmental. An unevaluated shell quoting syntax error executed no intended authoring statement. Neither is a product pass or a silently reset repair budget. No package/authentication gate occurred. Initial and terminal repair accounting live under `.phase4/04-20-r1/`.

## Fresh entire selected-set evidence

`.phase4/04-20-r1-final/selection.json` froze **473 relevant raw inputs**, all active executable Critical cases, both new/changed data cases and the complete conservative affected selection. Every selected executable case ran afresh. Original45diagnostic passes at `8f0eddc`,473inputs/1,521,076ms, are history and supply no repaired pass credit.

The repaired selection completed **45/45 passes** at exact source `ed8a79c78a986c18580a4f26ef8075ee967e7bff`, with **1,588,583ms** case time (about26.48minutes), finishing2026-11-18T02:43:46.932Z. Source drift and scheduling events are empty; safe-stop was not triggered. `results.json` retains per-case actual invocation IDs, HEADs and saved assembly closures. `completion.json` retains the complete outcome; `fullPhaseSelectedSetPass:false` correctly distinguishes this completed authoring slice from unfinished full-phase integration/actual acceptance.

| Ordered compatible group | Exact ordered case IDs |
| --- | --- |
| G0 | export.layout, audio.feasibility, media.conversion, media.scale, items.weapons-food, items.wearables, buildings.main-art, buildings.main-state, bodies.a-art, bodies.a-fit, bodies.b-wide, bodies.complete-fit, layouts.theme, layouts.identity, layouts.identity-integrity, layouts.inventory, layouts.legacy-target, layouts.camp, layouts.map-warnings, layouts.session, layouts.endpoints, animation.melee, animation.replacement, animation.bow, animation.work-special, appearance.corpses, appearance.crowds |
| G0-data | data.foundation, data.items-combat, data.economy, data.world, data.zombies, data.remaining |
| G0-defenses | buildings.defense-art, buildings.defense-state |
| G0-export-profile | export.profile, export.profile-boundaries |
| G0-hub | hub.source, hub.lifecycle |
| G0-integration | source.case-discovery |
| G0-world | world.map, world.effects, world.inspector, world.replacement |
| Phase1 final group | graph.visibility-inventory |

Fresh repaired `data.zombies` is **`9729c4d7-8cb6-48da-bf41-d844f7001846`**:577fields/source assertions,1069recorded controls and80legal loadouts. Fresh `data.remaining` is **`79045085-deab-4f5e-873c-13ae251ab894`**:1278fields/source assertions,2311recorded controls and30declared sound contracts. These are source/headless outcomes, not gameplay/audio/appearance judgments.

Both tracer requirements are retained. Original committed tracer `a78d4e2d-ecf4-4e86-abd2-0411abf1db42` passed at `767490c` before original Task2; repaired committed **same-case** tracer `fe9bcd80-c1a6-4abd-b0bd-aa3825ca41e9` passed at `745ea75` before R1.2. The ignored repaired scheduler gained only a pre-run safe case-boundary stop check; selected IDs, assertions and timeouts were unchanged. No active child was killed.

### Explicit omissions and evidence limits

`source.provenance` remains selected-but-blocked by historical `adb38f27-592b-4700-880c-3fa1d65d0c15`, `PARTIAL_PACK_REQUIRED` and stale `common.bow`/`Inspection.tscn`; it was not rerun or credited. `source.publication` remains source-inferred blocked, not an executed current pass/failure. Related tracer/packaged/export omissions preserve original WINDOWS29/23/31 ownership. Unaffected High `layouts.kit` remains unselected. Actual game windows/input/listening/appearance, other-phase cases and hosted/release procedures retain their owners.

Current native-policy observation is still historical **`08be5ab2`, FAIL2130 operations**, and the57-input/32-surface producer remains unenrolled. These data/CJS-only edits justified no repeated unchanged policy/cohort observation or separate solution build; selected native/compiled cases executed their normal current proofs and retained real closures. No native/public type, source-policy tuple or permission was added. Existing Root fresh-read/publication/cache/diagnostic boundaries, generic256/authored128/exact-map512 controls and theme-only2MiB/other1MiB/export14-file boundaries remain unchanged.

## Existing49 consumer comparisons remain nonpassing

These are separate from the319 closed authored findings:

- **18 prior repair/map comparisons:** main repair prices3, plant capacities6, rabbit batches6, rally offsets2 and `DefenseWorkPrice`1. Fresh `data.world` run `7ac76d33-d328-4184-b022-e9c541d10204` retains them as `integration-pending-04-23`.
- **30 effect-category comparisons:** exact original JSON pointers/current/required categories are preserved in `.phase4/04-20-final/effect-alias-comparisons.json`, source pack SHA256 `d2736226850b2c7775fc8042d0220ec0ebf701849f788e3ead826e4904dbc334`. The producer bytes remain exact. These historical nonpassing comparisons are not newly executed repaired passes.
- **One producer stain-duration comparison:** current `effects-portals.json` `/effects/31/duration=8` versus required20. Fresh `data.remaining` writes `existing-stain-integration.json`; the correct authored20second default does not conceal this actual old consumer.

The30category rows cover bow03→06; four material arrow-contact04→07; six melee02→03; trees12→13; collapses12→15; fire26→28; portal warning/spawn22→21; spit splash09→08; bloater09→29; food recovery07→19; crown18→10; night/retaliation25→20; portal-loss25→22; outcomes25→23; connection25→24. Actor-dependent character hit/death, natural regeneration feedback, generic portal impacts and flesh/armor contacts remain unclassified until actual context is inspected; none is assumed correct.

The prospectively committed `04-CURRENT-PACK-INTEGRATION-AMENDMENT.md` assigns exact Plan23 effects/portal-assembly/world-case/conditional-WorldPreview/shared-data-case and current presentation dependency metadata ownership. It includes actual20second producer lifetime, old `EFFECT_DURATION<=10`/expiry-after10 proof assumptions, real derived resources/recipes and verified binding updates. Prior repair/map scopes remain separate and intact. No existing consumer/dependency was edited during Plan20 repair.

## Physical audio/text and actual acceptance

Thirty AU/event/category/bus declarations are validated against explicit obligations, including unknown-event, mismatched-category and unknown-bus rejection. The result deliberately retains `allReferencesResolved:false`, audio `pending-04-21`, text catalog `pending-04-22` and consumer reconciliation `pending-04-23`. No future file/hash, audition or physical sound/text resolution is invented. The approved repair scope preserves this sequencing; the declaration proof does not certify the literal full-resource or actual-acceptance clauses ahead of their owners.

All ASSET requirements, AC-12/13/14 full-phase acceptance, WINDOWS29/30/31, current index0/180 and actual32/33 remain pending. Inventory236/census180/24AC/32D/104H+UI/135requirements and19public identities are unchanged.

## Artifacts this phase produces

Exactly the original nine implementation paths exist. `.phase4/04-20-r1-final/artifact-inventory.json` contains full raw/Git blob/old-new/source-commit inventories; `field-id-inventory.json` stores all complete metadata entries. Original nine raw/blob identities, audits and615-check checkpoint integrity remain preserved under `.phase4/04-20-final/`, including terminal checkpoint SHA256 `c359e94033cb2cdfc219b03eb2f828479c0fb4552c0ee419a89cc763a22e1a37`.

| Exact file | Bytes | SHA256 | Last source commit |
| --- | ---: | --- | --- |
| src/godot-game/content/config/zombies.json |4845| `b60b1aab2a307936153596871ff95dcdbdc2a89dba01f385dbfff6abcfabd889` |745ea75|
| src/godot-game/content/config/zombie-loot.json |6284| `71357249b0dbca27452e5b15db34b7ad28a26bdf0c9e9b44eb5d30678febd8ec` |767490c|
| src/godot-game/content/config/zombie-progression.json |3453| `68d48bd653326c0c655337b44f3a4bd6755b9cb2fb4c67a518e49a1e1ac776bd` |767490c|
| src/godot-game/content/schema/fields-zombies.json |643374| `5c3fc218cfcdc5eb083db11d80603cb2353c58f9d563a17ba448dfe1d321b03c` |745ea75|
| tools/phase4/cases/data-campaign.cjs |127842| `1bcf5b1f4787da9a0be488fc5f5190d3ddb6c19195252a41a6c99f03ec28fd6b` |ed8a79c|
| src/godot-game/content/config/portals-assaults.json |99417| `7b51db1b76ce5cd00e1155068265aea67326df20f7d69c2c756f6ec660ebeeee` |ed8a79c|
| src/godot-game/content/config/session-save.json |193807| `582380f356c414ff1d7019d223e6deb45c4a12f8c6a939ce076218a7273197f8` |8f0eddc|
| src/godot-game/content/config/presentation.json |1045063| `1c708e3d84970f2d61044db228e438aa8dcb79f7c91df09ee3818576f2ec5750` |ed8a79c|
| src/godot-game/content/config/local-preferences.json |122089| `f45aeb48d4f988efd8c30811d5cb1afb9f9e3939c994f8f9009a01ba4a3da112` |ed8a79c|

Stable document IDs are DT-12/13/14/15/16/17/25, `fields-zombies` and inline `fields-portals-assaults`, `fields-session-save`, `fields-presentation`, `fields-local-preferences`. Exact field IDs are domain plus JSON pointer, root `:/`: zombies158; zombie-loot271; zombie-progression148; portals-assaults86; session-save163; presentation917; local-preferences112. All1855 unique IDs are preserved.

The source module defines `ZOMBIES`, `SECTIONS`, `SOURCE_PINS`, `AUDIO_CONTRACTS`, `REMAINING`, `PRODUCER_PINS`, `SEMANTIC_UNIT_GROUPS`, `SEMANTIC_UNITS`, `DEATH_EFFECTS`, data vectors `roster/stats/qualities/stages`, delayed collaborator wrappers and case metadata. Its26 actual function declarations include local walkers. Top-level signatures are `sourceText()`, `defaultSection(n)`, `sourceIds(docs)`, `presentationUnit(p)`, `unit(p,type,name)`, `fieldsFor(name,data,text)`, `makeSchema(docs,text)`, `baseline(docs,expected)`, `references()`, `validate(docs,schema,text,expected)`, `relationships(docs)`, `guaranteedFixtures(docs)`, `sourceCheck(docs,text)`, `mutate(docs,name,p,value)`, `inputs(names,extras=[])`, async `zombies(context)`, `remainingExpected()`, `remainingSchema(name,data,text)`, `remainingRelationships(d)`, `remainingSource(d,text)`, async `remaining(context)`, `semanticUnits(files)`, `semanticEffects(docs)` and `semanticStain(p)`. All30 exact exports are recorded in the artifact inventory; the two saved cases remain Critical, group `data`, run group `G0-data`, orders44/45.

No native C# type/member/signature, public identity, source-policy tuple, raster/master, ZIP/source bundle or native subresource was added. Existing media, bundles and recipes remain read-only dependencies. No package/toolchain installation occurred.

### Source commits and identities

Current initial-spec SHA256 remains `9d3668d41477e7a70aac4bef7cf83c252470b0e2e96da02543f0c2fefa325260`, creator source commit `ac9e172569d926c14eb57dd6045ffdad58db3a6d`. Relevant source sections are independently pinned. Dependency commits remain foundation validator `7e3b7e5d`, camp validator `9f4f44eb`, effects/portal assemblies `12954554`, world map `5c350248`, UI kit `ed5c404b`, session examples `2d7aa9f7` and settings examples `5b7978af`. Full hashes/commits are in the inventory. Creator Phase07 and other agents' work were preserved; historical reports were not relabeled as later HEAD evidence.

## Known stubs, threats and parent handoff

No goal-blocking authoring stub or TODO/FIXME/placeholder/coming-soon marker remains in the nine artifacts. Intentional zero charges/non-damaging effects and explicit future resource availability are documented semantics, not simulated gameplay. No new endpoint, auth path, schema trust boundary or permission was introduced. Strict contained reads, finite/count/depth checks, source pins and exact diagnostic controls remain; the independent oracle fixes strengthen the existing boundary.

Parent owns global STATE/ROADMAP/REQUIREMENTS/PROJECT updates, local27→28/global52→53 progress, wave22 posthooks, PR5/push and next Plan21/wave23. No global planning/spec mutation, branch switch, merge, reset, stash, push, PR transition, WINDOWS mutation or recreation of removed `06-DISCUSS-CHECKPOINT.json` occurred. Branch remains `feature/04-asset-production`; draft PR5 targeting `dev` remains draft.

## Self-Check: PASSED

All **633** checks in `.phase4/04-20-r1-final/self-check.json` passed:45actual fresh outcomes and saved closures;473frozen raw inputs; nine repaired raw/Git identities and original committed blobs; six task/coordinator ancestors; both committed tracers; original terminal checksum and615-check record;319exact old-fail/new-pass closures;1855unique independent unit identities; original control names and80unchanged loadout fixtures; and explicit existing49integration ownership. After writing this summary, `summary-self-check.json` separately verified its existence, the six ancestors and unchanged473inputs. `budgets-terminal.json` records original1/3+3/3 and R1.1=1/2,R1.2=0/2,R1.3=0/2 without reset. These checks certify the repaired authoring/source slice; physical resources, native/index integration and actual acceptance remain pending as stated above.
