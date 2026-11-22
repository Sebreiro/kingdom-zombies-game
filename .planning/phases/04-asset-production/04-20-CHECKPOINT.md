---
phase: 04-asset-production
plan: "20"
status: blocked
gate: blocking-human
type: decision
tasks-authored-and-committed: 2
tasks-accepted: 0
requirements-addressed: [ASSET-02, ASSET-04]
requirements-completed: []
source-head: 8f0eddc1a5ef914f3ccc502f8f2bb4d4a26ebb89
plan_head_before: 524b311af9f078392ec11b91db61703d1ff29c60
plan_head_after: 8f0eddc1a5ef914f3ccc502f8f2bb4d4a26ebb89
implementation-commits: 2
actuals:
  tokens: 553934
  tasks: 0
  commits: 2
actuals-scope: "Measured 2215735 diff characters divided by4, rounded upward; two authored task implementations but zero accepted tasks. Metadata checkpoint commit is separate."
original-correction-budgets:
  task1: "1/3 used"
  task2: "3/3 used; exhausted"
diagnostic-selection:
  cases: 45
  executable-passes: 45
  frozen-inputs: 473
  source-drift: 0
  scheduling-events: 0
  duration-ms: 1521076
  finished-at: "2026-11-18T01:58:52.180Z"
  completion-accepted: false
canonical-summary-created: false
---

# Phase 04 Plan 20: Incomplete authoring checkpoint

**Two task implementations are committed, but319 confirmed authored defects prevent completion. Task2 has exhausted its original three-correction budget. No further repair was performed.**

The canonical `04-20-SUMMARY.md` intentionally remains absent. The parent explicitly authorized this checkpoint path so a SUMMARY existence check cannot release dependent Plan21. Neither task is accepted despite the passing committed tracer and diagnostic CLI outcomes. Parent owns global progress, the next bounded repair scope, wave22 posthooks, push and draft PR5 targeting `dev`.

## Committed implementation and original budgets

| Task | Commit | Authored scope | Current acceptance |
| --- | --- | --- | --- |
| 1: archetypes, loot and independent progression | `767490c3939b4062778de090d986732030acabf8` | First five planned paths;577 fields;484 controls;80 guaranteed-loadout fixtures | Blocked by five incorrect units across zombies/loot and seven wrong death-effect identities |
| 2: portals, sessions, presentation and local preferences | `8f0eddc1a5ef914f3ccc502f8f2bb4d4a26ebb89` | Last four planned documents plus shared case;1278 fields;1031 controls | Blocked by306 incorrect units and authored stain lifetime8 instead of20; original budget exhausted |

Task1 used **1/3** corrections: first failed invocation `5f16c5d5-07fd-448d-9af4-9103fe79c60d` reported `CONTROL_ORACLE` because the negative fixture expected the mutation's leaf path while the production relationship validator reported its enclosing archer guarantee. Corrective change1 specified exact independent diagnostic paths. GREEN `daa3752b-5e73-4243-946d-81fefdf062a0` preceded the task commit. Fresh **same-case committed tracer** `a78d4e2d-ecf4-4e86-abd2-0411abf1db42` passed at `767490c`, before any Task2 implementation. Earlier intermediate GREEN `c66ef33b-9bb3-4b52-b134-d47ea73822fe` also remains preserved.

Task2 used **3/3** corrections, without reset:

1. `1f9a3cd3-6e0a-4b29-b1c3-c2a82baf68c0`: strict production `SIZE` rejected presentation JSON at1,075,481 bytes. Correction1 shortened Task2 field-description prose while preserving all917 entries, defaults, source evidence and dependencies. The inherited1MiB parser limit was not raised.
2. `c01b85f5-84e3-4533-b464-02f264898e22`: `BOUNDS` rejected `/ui/scalePercent/2=150`. Correction2 distinguished the required80–150% UI scale from probability maxima100.
3. `24a0a0ca-20a4-48df-ba1f-9110435fc3b7`: `BOUNDS` rejected `/ui/fontWeights/0=400`. Correction3 limited probability-weight bounds to actual weapon/variant weights, allowing400/700 font weights. That change did **not** repair the separate unit classifier, which still labels font weights as percent.

Task2 GREEN `44b81672-bd45-49ef-8fb3-152afae40af6` passed before commit at `767490c`. Original outcomes are immutable. The initial sandbox `spawnSync git EPERM` happened before a run could be created and was environmental; the equivalent reviewed source-only command succeeded. A failed PowerShell/Node quoting command did not execute its intended authoring script. Neither event is a product pass or an additional corrective attempt. No package installation or authentication gate occurred.

## Confirmed authored defects:319 nonpassing comparisons

### Incorrect unit metadata:311

`.phase4/04-20-final/unit-defects.json` records every file, field ID, JSON pointer, current value/unit and independently required unit. `.phase4/04-20-final/unit-source-rationale.json` adds exact proposed semantic units and source rationale. These comparisons do not call `unit()`.

| Document | Incorrect units | Examples and required semantics |
| --- | ---: | --- |
| zombies.json | 4 | `/common/regenerationPerSecond`: count→stamina/second; `/common/naturalHealthRecovery`: count→health/second; bloater slowdown→ratio and durability loss→durability |
| zombie-loot.json | 1 | `/generation/minimumMoney`: count→money |
| portals-assaults.json | 3 | `/rally/price`, `/rally/recallPrice`, `/rally/failedCharge`: count→money |
| presentation.json | 297 | Font weights percent→font-weight; display/font/spacing/light-radius dimensions count→pixels; RGBA, gains, line-height, contrast and damage thresholds→ratio; lighting time→normalized-day-cycle |
| local-preferences.json | 6 | Display resolution and supported resolution dimensions count→pixels |

`unit(p,type)` is the source of these classifications. `fieldsFor()` authors metadata through it; `validate()`/`remainingSchema()` compare against metadata produced through the same classifier. Therefore their passing metadata equality is **not an independent unit proof**. Scalar baseline defects also compare to the same authored expected structure and do not establish that its semantic unit is correct. The repair needs independent expected units and direct wrong-unit controls that reach the intended validator diagnostic, plus restored positive controls.

The source rationale distinguishes pixels, font-weight codes400/700, dimensionless ratios and normalized lighting time, physical money, stamina rates and disabled health recovery. These expected semantics are proposals for the bounded repair; they have not been written into production metadata or accepted through an amended scope.

### Wrong known effect identities:7

`.phase4/04-20-final/source-contract-defects.json` records actual current/required identities and the concrete producer entry.

| zombies.json pointer | Current | Required actual effect obligation |
| --- | --- | --- |
| `/archetypes/0/deathEffect` shambler | FX-02 | FX-08, `character.death` |
| `/archetypes/1/deathEffect` runner | FX-02 | FX-08, `character.death` |
| `/archetypes/2/deathEffect` brute | FX-02 | FX-08, `character.death` |
| `/archetypes/3/deathEffect` armored | FX-02 | FX-08, `character.death` |
| `/archetypes/4/deathEffect` spitter | FX-02 | FX-08, `character.death` |
| `/archetypes/5/deathEffect` archer | FX-02 | FX-08, `character.death` |
| `/archetypes/6/deathEffect` bloater | FX-11 | FX-07, `bloater.burst`/`bloater.stain` |

The independent obligation catalog identifies FX-02 as melee swing/hit feedback, FX-07 as bloater splash/stains, FX-08 as character hit/death/recovery feedback, and FX-11 as condition/damage/palette material profiles. These authored values are valid known IDs with the **wrong meaning**. Membership validation accepts them. `relationships()` verifies that a separately chosen semantic producer effect exists but does not tie that effect to the authored `deathEffect` field. The literal expected data repeats the wrong IDs. The repair must validate the exact identity-to-actual-effect relationship and reject positive-known-but-wrong FX IDs independently.

### Authored stain lifetime mismatch:1

`presentation.json` `/effects/profiles/31/durationSeconds` is **8**, while initial-spec§9.1 requires cosmetic stain lifetime **20seconds**, already present in `zombies.json` `/bloater/lifetimeSeconds`. `remainingExpected()` copies the pinned existing visual producer duration8 into authored defaults, so both expected data and production document repeat the error. This authored default belongs to the upcoming Plan20 repair, not merely a later integration deferral.

The existing producer's separate duration8 comparison belongs to Plan23, described below. The authored repair must require20 independently while retaining an honest existing-consumer mismatch until that consumer is repaired. No producer was edited during this plan.

## Existing consumer integration:49 separate nonpassing comparisons

These are distinct from319 authored defects. No passing CLI outcome turns them into a successful integration claim.

### Prior18 repair/map comparisons retained

Fresh diagnostic `data.world` run `036ed9d6-e339-4c9c-af0c-51437f55a174` writes `integration-mismatches.json` with the existing18 `integration-pending-04-23` rows: three main repair prices30/60/210→50/150/400; one `BuildingPreview.DefenseWorkPrice` missing-health-ratio fee→fixed50% full valuation; six plant capacity values; six rabbit dawn batches; and two rally offsets+3/-3→+5/-5. Original `04-19-AMENDMENTS.md` and `04-CURRENT-PACK-INTEGRATION-AMENDMENT.md` ownership is preserved. Main pricing, replacement-worker/payment continuity and civilian safety creator decisions were not changed.

### Thirty legacy sound-category comparisons actually executed

`.phase4/04-20-final/effect-alias-comparisons.json` records30 exact current effect rows, their `/effects/{index}/sound` and `/soundObligation` pointers, and companion `/soundEvents/{index}/id` and `/obligation` pointers. Every row is `integration-pending-04-23`. The actual source is `src/godot-game/content/packs/effects-portals.json`,519,562bytes, SHA256 `d2736226850b2c7775fc8042d0220ec0ebf701849f788e3ead826e4904dbc334`, last source commit `12954554884808826780d28f6e896b5257218e8a`.

| Consumer effect group | Current category | Required category |
| --- | --- | --- |
| Three arrow releases | AU-03 | AU-06 |
| Wood/metal/ground/portal arrow contacts | AU-04 | AU-07 |
| Six melee effects | AU-02 | AU-03 |
| Tree chop/fall | AU-12 | AU-13 |
| Wood/metal collapse | AU-12 | AU-15 |
| Fire loop | AU-26 | AU-28 |
| Portal warning/spawn | AU-22 | AU-21 |
| Spit splash | AU-09 | AU-08 |
| Bloater burst | AU-09 | AU-29 |
| Food recovery | AU-07 | AU-19 |
| Crown/control change | AU-18 | AU-10 |
| Nightfall/retaliation warnings | AU-25 | AU-20 |
| Portal-loss warning | AU-25 | AU-22 |
| Victory/defeat | AU-25 | AU-23 |
| Connection-loss warning | AU-25 | AU-24 |

The expected categories come from the independent obligation meanings and the newly declared event contracts; they are not derived from the old aliases. The audit does not classify actor-dependent `character.hit/death` routing (AU-09 versus AU-10), natural health/stamina feedback, generic portal impacts or flesh/armor arrow routing as passes. These need explicit current context before repair.

### One existing stain-duration comparison

`effects-portals.json` `/effects/31/duration=8` for `bloater.stain` independently conflicts with initial-spec§9.1's20seconds. This is one existing consumer comparison, separate from the authored presentation field. `tools/phase4/cases/world.cjs` currently enforces `EFFECT_DURATION<=10` and checks expiry after10seconds, so a producer-only value change cannot honestly satisfy the existing proof contract.

### Proposed exact integration paths; not an edit authorization

Parent must prospectively scope Plan23 before editing:

- `src/godot-game/content/packs/effects-portals.json`: affected `effects[].sound/soundObligation`, `soundEvents[]`, stain duration, and the exact retained `sources.authoringRecipe` fields when needed. Preserve original source bundles and receipts.
- `src/godot-game/content/world/portal-assemblies.json`: exact sourceBindings and genuinely changed derived resources; rebuild real derived resources when required rather than only changing hashes.
- `tools/phase4/cases/world.cjs`: independent event/category obligations,20second stain lifetime and the precise duration/expiry assertions now bounded at10seconds.
- `src/godot-game/inspection/WorldPreview.cs`: only if actual routing or valid20second consumption requires a change, with its existing native boundary, all original assertions and exact permissions retained.
- Actual Plan21 `content/audio/events.json` and audio pack outputs, then exact dependent catalog/scene/pack/hash bindings identified from the changed source. No future file or checksum is invented here.

## Forward-reference availability remains separate

`data.remaining` validates30 declared event/category/bus contracts and rejects unknown events, mismatched categories and unknown buses. Its result deliberately reports `allReferencesResolved:false`, audio resources `pending-04-21`, legacy effect bindings `pending-reconciliation-04-23`, and translation catalog `pending-04-22`. These states do not represent actual audio resource resolution, audition or translated catalog validation. Plan21/22 must produce their physical outputs and Plan23 must reconcile consumers. The source/headless declaration check cannot certify the plan's literal full resource-resolution clause or AC-12/13/14 acceptance.

The new documents are editable authoring definitions and inspection fixtures. They do not implement campaign, session/save, loot gameplay or a live balance editor. Existing runtime/index adoption remains0/180. Actual sound/listening, game input, appearance and final32/33 acceptance remain pending.

## Frozen diagnostic selection and raw evidence

The complete saved selection is `.phase4/04-20-final/selection.json`: **44Phase4 cases plus `graph.visibility-inventory`**, ordered by compatible source/headless run groups. It includes every active executable Critical case, both new cases and the complete prior43 conservative affected selection. All45 executed afresh; no evidence was reused. The prior Plan17 result at `5b7978a` is history, not current pass credit.

The473 frozen raw inputs remained exact. All invocations and the final source HEAD are `8f0eddc1a5ef914f3ccc502f8f2bb4d4a26ebb89`; scheduling events and source drift are empty. Actual case time totals **1,521,076ms**, about25.35minutes; completion timestamp is2026-11-18T01:58:52.180Z. `.phase4/04-20-final/results.json` preserves each actual invocation, source HEAD and saved assembly closures. `completion.json` reports45passes and `fullPhaseSelectedSetPass:false`.

New cases at that revision are `data.zombies` run `540b215e-8343-420a-b38a-fc67a92bc612` and `data.remaining` run `f7ca6ac4-f452-45a9-b0c6-ec02e353bbe2`. Their577/1278 field assertions and484/1031 controls pass as currently implemented; the independent319 authored defects demonstrate the missing semantic coverage. Those passing reports are diagnostic evidence only.

During the run, the parent requested stopping after the active case. The already-running synchronous scheduler had no cancellation-marker check. Parent then explicitly chose to let it finish unchanged rather than kill a scheduler/child, race a process boundary or mutate frozen inputs. This is a scheduling limitation, not a test failure or an extra repair attempt.

### Omissions and unchanged boundaries

`source.provenance` remains selected-but-blocked by historical `adb38f27-592b-4700-880c-3fa1d65d0c15`, `PARTIAL_PACK_REQUIRED` and stale `common.bow`/`Inspection.tscn` binding; it was not rerun or credited. `source.publication` remains source-inferred blocked, not a fresh executed outcome. Related tracer/packaged/export omissions retain original WINDOWS29 and23/31 ownership. Unaffected High `layouts.kit` remains unselected. Other phases, hosted/release procedures and actual rendered scenarios retain their owners.

Current native-policy observation remains historical `08be5ab2`, FAIL2,130 operations (prior1,861 plus269 SessionLayouts), and the57-input/32-surface producer remains unenrolled. No repeated unchanged native-policy/cohort observation or separate full-solution build was justified by these data/CJS-only changes. Selected native/compiled cases ran their normal current proofs and saved their real closures. No native/public identity, C# type, generated surface or policy tuple was added. WINDOWS29/30/31, inventory236/census180/24AC/32D/104H+UI,135requirements and19public identities are unchanged; all ASSET acceptance remains pending.

## Artifacts this plan actually produced

Exactly the original nine implementation paths were created. `.phase4/04-20-final/artifact-inventory.json` contains complete raw SHA256, filtered Git blob, last source commit, dependency identities and function/export inventory. No implementation file was edited after `8f0eddc`.

| Exact path below repository root | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/content/config/zombies.json | 4845 | `1976f94e473c33944aa3ae4281908570db1e1e242198fe834f890b0415cd509f` |
| src/godot-game/content/config/zombie-loot.json | 6284 | `71357249b0dbca27452e5b15db34b7ad28a26bdf0c9e9b44eb5d30678febd8ec` |
| src/godot-game/content/config/zombie-progression.json | 3453 | `68d48bd653326c0c655337b44f3a4bd6755b9cb2fb4c67a518e49a1e1ac776bd` |
| src/godot-game/content/schema/fields-zombies.json | 643352 | `bbb81d0d67b50029800cbbfae52dbcd860c8a72861ea0082f04854d274c565f1` |
| tools/phase4/cases/data-campaign.cjs | 46277 | `ec28fdc51a9058e94894ad74d35aa5ce5ad147fced0613e8c82f5e68f0743acd` |
| src/godot-game/content/config/portals-assaults.json | 99417 | `2b6396a7398aff6ea06164a9502beee18dc9b16d637d9e60dbd264ac739a27fe` |
| src/godot-game/content/config/session-save.json | 193807 | `582380f356c414ff1d7019d223e6deb45c4a12f8c6a939ce076218a7273197f8` |
| src/godot-game/content/config/presentation.json | 1042353 | `db546c1b02bd8880b24b655d61c78c98a9552a9c2415e2f9df42e0fc12cc93b0` |
| src/godot-game/content/config/local-preferences.json | 122083 | `ddb92972f07f39d466fb02fe51b1670c2eb0e238a2c75f0d3d434fb1c242bc80` |

`.phase4/04-20-final/field-id-inventory.json` preserves all1855 unique IDs: `zombies:`158, `zombie-loot:`271, `zombie-progression:`148, `portals-assaults:`86, `session-save:`163, `presentation:`917, `local-preferences:`112. Stable document IDs are DT-12/13/14/15/16/17/25, `fields-zombies`, and inline `fields-portals-assaults`, `fields-session-save`, `fields-presentation`, `fields-local-preferences`. Each field ID is the domain plus its exact JSON pointer, with root `:/`.

New saved case identities are `data.zombies` and `data.remaining`, Critical, group `data`, run group `G0-data`, order44/45. The module introduces constants `ZOMBIES`, `SECTIONS`, `SOURCE_PINS`, `AUDIO_CONTRACTS`, `REMAINING`, `PRODUCER_PINS`; data vectors `roster`, `stats`, `qualities`, `stages`; delayed collaborator `base`; and wrappers/metadata.

Actual function signatures are `sourceText()`, `defaultSection(n)`, `sourceIds(docs)`, `unit(p,type)`, `fieldsFor(name,data,text)` with local `walk(value,p)`, `makeSchema(docs,text)`, `baseline(docs,expected)` with local `walk(a,e,n,p)`, `references()`, `validate(docs,schema,text,expected)`, `relationships(docs)`, `guaranteedFixtures(docs)`, `sourceCheck(docs,text)`, `mutate(docs,name,p,value)`, `inputs(names,extras=[])`, async `zombies(context)`, `remainingExpected()`, `remainingSchema(name,data,text)`, `remainingRelationships(d)`, `remainingSource(d,text)` and async `remaining(context)`. The24 exact exports are recorded in the artifact inventory; neither helper defaultSection nor sourceIds is exported.

No raster/master, ZIP, donor export, new recipe bundle, native resource subresource or C# signature was produced. Existing production bundles/resources remain read-only dependencies. No paid dependency or package was added.

### Source identities

The current initial-spec raw SHA256 is `9d3668d41477e7a70aac4bef7cf83c252470b0e2e96da02543f0c2fefa325260`, last source commit creator `ac9e172569d926c14eb57dd6045ffdad58db3a6d`. Source sections are pinned independently in the authored metadata and case constants. Creator Phase07 content was preserved.

Dependency source commits include foundation data validator `7e3b7e5de3f635cea22abeba397756c401e3eb36`, camp data validator `9f4f44eb0b12eb98c3f7b675ad116f5e78d511c4`, effects/portal assemblies `12954554884808826780d28f6e896b5257218e8a`, world map `5c350248cf534a459f2b2597e76dbec80df2b7ce`, UI kit `ed5c404b64fd2dd2ce2834ff5b84a5dd85c8adfb`, session examples `2d7aa9f7af25098c4935cd12bf1c973dc84317a3` and settings examples `5b7978afdc10a223035876c301911780bb897f78`. The artifact inventory records their exact raw hashes. Historical evidence is not relabeled as the later source HEAD.

## Checkpoint integrity self-check

`.phase4/04-20-final/checkpoint-self-check.json` reports **615/615 checkpoint-integrity checks passed**:473 frozen inputs; nine raw artifact hashes/committed blob identities;45actual reports and saved closure identities; both task commits; the committed Task1 tracer;1855 unique field IDs; retained311unit/8identity-duration/30alias audit rows; and canonical SUMMARY absence. This checks evidence integrity only. Semantic completion is **not accepted**.

No goal-blocking issue was hidden as a stub. Known defects are explicit above. Physical audio/text availability and current-index/native integration remain separate outstanding owners. No new remote endpoint/authentication/schema trust boundary was added; existing strict contained1MiB reads, source hashes, independent diagnostic paths and immutable evidence boundaries remain. The plan's correctness claims remain blocked by the documented authoring/oracle defects.

## Required next action and preserved ownership

Parent must issue a **prospective bounded coherent Plan20 repair scope** covering the319 authored units/identities/lifetime defects and genuinely independent unit/effect/lifetime assertions, including positive-known-but-wrong IDs, exact wrong-hash/rebound/restored controls where applicable, and the unchanged1MiB presentation bound. The source-derived8second authored default needs an independent20second expected obligation while separately preserving the existing producer mismatch. Do not reset or narratively extend original budgets; Task1 remains1/3 and Task2 remains exhausted3/3. Any new repair scope must name its own exact files, acceptance and budget before changes.

After authorized repair, commit the actual fixes, freeze relevant bytes/case contracts anew and execute the required fresh affected selection. The current45-case diagnostic run does not replace that post-repair selection. Only actual repaired acceptance may produce canonical `04-20-SUMMARY.md` or release Plan21.

No PROJECT/STATE/ROADMAP/REQUIREMENTS/initial-spec amendment, branch switch, merge, reset, stash, push, PR transition, policy enrollment, global WINDOWS mutation or recreation of removed `06-DISCUSS-CHECKPOINT.json` occurred. The source branch stays `feature/04-asset-production`; draft PR5 remains draft. Parent retains global52/62 and local27/37 prior progress; this incomplete Plan20 does not advance it.
