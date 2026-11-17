# 04-06 Task 1 — bounded art-verification checkpoint

Recorded2026-11-17. Plan04-06 remains **0/2 tasks complete**. No production commit or SUMMARY exists. Task2, its compiled RED and native fitting behavior have not started. The parent retains global progress/continuation/PR ownership. AllASSET and actual04-32/33 acceptance remain pending.

## Pinned checkout and preserved work

- Root `D:/Work/Games/KingdomZombies`, branch `feature/04-asset-production`, starting/current production HEAD `91144f763b9d8781e950f75f7c6e06032d7e12ea`.
- Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE, Phase6 SPEC/discussion checkpoint, initial-spec and untracked Phase12 directory remain untouched and unstaged.
- Index was empty at dispatch and at this checkpoint. No branch switch, install, stash/reset/rebase/merge/push or unrelated process termination occurred.
- No real game window, listening, physical input or appearance acceptance was performed. Actual listener remains null.

## Current observed blocker

Canonical `node tools/phase4/check.cjs --case bodies.a-art` fails in immutable invocation `.phase4/runs/72c88b32-77b4-4d4e-aad1-12227eeb4712/` with `BODY_ATLAS_PIXELS`.

Read-only inspection identifies exactly one out-of-bounds part: `CH-02.local-head`, atlas rectangle `[0,480,48,40]`, atlas size `[768,480]`. The existing canvas holds192 cells; the adapted head is the193rd. Proposed bounded continuation: change only the recipe canvas to768×520, regenerate/publish the same four implementation paths, run the canonical gate, and, only after a successful Task1 commit, rerun its mandatory tracer before any Task2 work. This proposal is **not authority to edit**; the executor stopped after its correction budget.

## Retained correction/failure history

1. First CLI attempt failed before creating an invocation: `CASE_MODULE_EMPTY .../items.cjs`. The earlier alphabetical bodies module eagerly imported the items case and registered it before the discovery loader reached it. Correction1 made item helpers lazy; no discovery semantics or other module was edited.
2. Next attempt failed before invocation creation with `REVISION_UNAVAILABLE common.bow spawnSync git EPERM`. This is an environment failure, not an assertion or RED. The authorized verifier was subsequently escalated with child-process access.
3. Invocation `e38bde03-e8c0-4e4b-b0fc-9df39aabc376` failed `COMMAND_TIMEOUT_RANGE common.bow bodies-art` before Python launch. Correction2 removed the unsupported120000ms child option, retained the existing60000ms cap, and cached exact image/transform reproduction to fit that bound. No timeout/capacity rule was widened.
4. Invocation `7991ed40-de79-4547-8804-5070ccdd0731` failed `BODY_PIXELS`: six head partitions were empty. Source-side inspection also found detached neighboring-cell pixels in three runner samples. Correction3 selected the actual upper head pixel row and retained the largest8-connected source component before nearest resizing; this preserves authored pixels and removes neighboring-cell fragments. The exact recipe and independent replay checker record those operations.
5. Invocation `72c88b32-77b4-4d4e-aad1-12227eeb4712` failed `BODY_ATLAS_PIXELS` as described above. No further correction was applied.

The default three correction batches are exhausted. A new prospective bounded authority must name its exact edit/verification scope and attempt count; previous failures/budgets must remain intact. No Task1 pass or source-side acceptance is inferred from partial earlier assertions.

An optional read-only key-inspection command initially had one extra closing parenthesis and failed Node syntax parsing; the corrected read-only command confirmed the existing `pngSha256` donor-record field. Neither command supplies verification credit.

## Generated originals and source decisions

Built-in image_gen only; no API key, CLI fallback or installation. All prompts, generated originals and source/adaptation recipes are retained in the current ZIP. D-13's simpler low-resolution direction overrides older sprite-skill richer examples. The source-side production choice is density1, with30/28/30 standing pixels for CH-01/02/03; inspection enlargement is separate.

- Initial CH-01 prompt was rejected at output by the safety system: request `7d7bd875-d6da-4f0d-b787-2b0a29e3128e`, `moderation_blocked`, category `sexual`. Exact original prompt/error identity is retained in `history/rejected-safety.json`; no image was returned.
- Safe covered-base retry `exec-f927d70c-6320-40d3-ba87-9d73b77da190.png` was rejected as resembling baked selectable clothing. It remains separate in `history/rejected-covered-survivor.png` with its expanded prompt.
- Selected CH-01 featureless tan artist-mannequin original `exec-7049c737-a451-4364-87f1-475543e6b056.png` SHA256 `91c129c5f6975b096608e22c684b11af2aa7907f2fe8f145add79181ef687963`; expanded-prompt hash `91e835604d62404c21eefce75f0697ceb202abac7bb5fbdc4a2932a0f7097c13`.
- Selected CH-02 hunched grey-green mannequin original `exec-150d36ae-c41e-4d9f-854d-21ec9d37d447.png` SHA256 `ae89005fec1683d9ffec84a5b29cdd5b3284abf2c455f900a239e46373b4e2c2`; expanded-prompt hash `e09dff640a1a6a4647401b3dfd20931c110ee95628e748980b74fadce26e192f`.
- Initial CH-03 `exec-190acc74-9b64-45dd-bac6-3a34b6341d93.png` was rejected because its first head touched/clipped the canvas top. Original and initial prompt remain separate in history.
- Selected targeted CH-03 repair `exec-556feeac-a216-4745-bba1-83c52eee88c2.png` SHA256 `46a7a5542ad7d9f90c9da2ce594da1235dc674b1b8ef2aad0f7c74df2e3ac822`; repair-prompt hash `3711c91fb32cc34314c149d293bef1cb7ab3f9d14ba7906abb97095382ade95b`.

All actual originals remain in `C:/Users/User/.codex/generated_images/01a1e749-8703-7933-80c2-dc6e3a3ba15d/` and are copied into the bundle or retained rejected history. Selected sources each contain16 poses in four rows. The recipe thresholds alpha at240, removes observed red matte using `r>180 && r>2*g && r>2*b`, removes disconnected neighboring-cell pixels, crops exact source rectangles, uses one body-wide nearest scale and places on48×40 registered canvases. Retained interior RGB values are unchanged. These are explicit adaptation transforms, not unchanged donor reuse.

Exact K14/K18 references are `common.s0.2361`, `7052`, `10365`, `7060`, `7246`, `10669`, with raw bytes, original PNGs and records. Local zombie idle/walk/attack strips and identities are retained; baked clothes are excluded from neutral body pixels. The uncovered idle head crop `[560,260,815,540]` is adapted to5×6 with nearest sampling as `CH-02.local-head`; its actual renderer binding is still Task2 work and is not claimed complete.

## Frozen own WIP and backup

| Implementation path | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/assets/characters/bodies-a.png | 47929 | 5eecacec87394adf7faa791070d19bddbf89913daf49702a6a05af3093ea0efc |
| src/godot-game/content/packs/bodies-a.json | 341525 | 931e5ed9e08bd7c2f34d203f54870f2bca3f79a1dc1291c663388f5c5d766c9d |
| production/phase4/bodies-a.zip | 5822523 | 8d5776a0690f31994be16ea57e4479d98f88fedc644a7243e093fcf1f3c4acb9 |
| tools/phase4/cases/bodies-a.cjs | 11255 | 7ed0821584f99ad09d7c951c28e63099bf8ef37f4406fd054a92b883a0169c84 |

Exact copies plus recipe and machine manifest are in `.phase4/checkpoints/04-06-task1-bounded/`. Latest authoring stage is `.phase4/04-06/stage-1794882712934897700/`; authoring recipe `.phase4/04-06/build-bodies.py`; selected-stage pointer `.phase4/04-06/latest-stage.txt`. Original stages and invocation directories remain untouched.

The current bundle has232 members: three selected originals, expanded prompts, retained rejected attempts, six exact donor raw/PNG/record triples, three local source references, scale-pilot metadata,48 full body poses,144 partitioned components, the adapted local head master/atlas component, recipe, canonical PNG/JSON and complete `members.json` hash inventory. No final artifact/symbol/native tuple inventory or valid SUMMARY is claimed until both tasks actually pass. There is no C# implementation or new native-policy tuple in this checkpoint.

## Required continuation order

1. Obtain bounded authority for the precise canvas correction and its verification budget; do not silently reset original budgets.
2. Verify frozen own WIP hashes and preserve all foreign changes; root/branch/index pin before any edit/commit.
3. Apply only authorized correction, regenerate in a fresh stage, publish PNG/ZIP then JSON and run actual `bodies.a-art`. Any new failure stops according to the new bounded authority.
4. Commit verified Task1 scoped files with normal hooks, then rerun postcommit tracer before Task2 expansion.
5. Execute Task2 with genuine compiled behavior RED, production renderer and independent native assertions, strict production/harness0warnings/errors and exact diagnostics. This remains unimplemented.
6. Only after both tasks finish, write/self-check/commit canonical SUMMARY immediately. Parent handles global progress and continuation; no ASSET completion.

## Superseding Task2 checkpoint — 2026-11-17

Current plan progress is **1/2 tasks complete**. The earlier Task1 checkpoint and every original failed stage/invocation remain historical. Task1 is now committed; Task2 has a genuine committed RED and five backed-up implementation WIP files, but no complete GREEN, Task2 implementation commit or SUMMARY. Phase progress remains12/37. Shared global planning/spec/Phase06/Phase12 changes remain untouched and unstaged.

### Completed Task1 and mandatory feedback

- R2 moved only the existing per-body original/prompt `BODY_GENERATION` assertion ahead of part decoding/replay. The PNG/JSON/ZIP stayed byte-identical to R1. Fresh art `b2aa3f0b-1a33-49c2-8cc3-131cd01942a7` and discovery `70d4fd0b-73d6-443d-b4f3-cc153b5ae804` pass. All193 parts reproduce and are contained, all48 poses and three distinct alpha silhouettes remain, and all nine original controls produce their exact diagnostics. The actual768×520 atlas was inspected at native scale.
- Task1 commit `2c54e537df089e486a777dc131e5f6130c1cc691` contains only the original four owned files. Mandatory postcommit tracer `a4ca9e61-d718-453c-b916-4e21971d48a7` passes on those committed bytes before Task2 expansion.
- Original Task1 corrections3/3 and R1 retry1/1 remain exhausted; R2's one attempt resolved its exact ordering defect. Initial unsupported repeated `--case` usage and sandbox git EPERM supplied no assertion credit; equivalent elevated single-case commands passed.

### Task2 RED and observed partial behavior

Native invocation `169d3a1b-9d74-43b8-a214-dbb252ffb872` builds the actual production solution0warnings/0errors and drives the real CSharp hub/category/resource controls. NativePID37828, nonce and productionDLL `ac019f9673f9d09520f997b97f4be3e0eafbc5c28df5690bd78f00d518a4f862` authenticate the baseline: no character controls and `coatAfterArmorRemoval:null`. The compiled `CharacterFitTests.Selected_Coat_ReturnsAfterArmorRemoval` executes one test, fails one assertion expectingIT-13, and skips none. The supported TAP replay and semantically assessed `RED_EVIDENCE_OK` record are `.phase4/04-06-task2-red.json`; its actual command is `node --test-reporter=tap .phase4/04-06-red.cjs`. RED commit `765c45d0f182840dc39defd099f4f99d64df702c` contains only CharacterFitTests and the saved case/native client.

Partial GREEN `61d6a6ce-9fd6-471c-ad7c-b320b5ba28da` subsequently proves the original coat assertion on the actual native hub, with production0warnings/0errors and nativePID18664. This partial gate did not yet include the complete matrix and is not Task2 completion. The latest full invocation below again proves actual exposedIT-13, but blocks before executing the matrix or expanded compiled controls.

### Exhausted Task2 corrections and exact blocker

The ordinary Task2 three correction batches are **3/3 exhausted**:

1. Initial implementation strict build reported ten authored analyzer errors: null-forgiving, repeated constant array, nonordinal equality and concrete texture return type. Batch1 corrected them without suppressions; partial GREEN61d6a6ce passed.
2. Expanded full invocation `1ab19852-f699-4e6d-9534-ebb0908d1c23` reported four formatting errors. Batch2 corrected the reported binary indentation and overlong source/test lines. Subsequent `cbfd68aa-a0bf-4fa8-95b2-5af712ebd866` retained one nested-lambda SCR0009.
3. Batch3 changed that exact remaining lambda-body indentation from24 to32 spaces. Fresh full `d89bb321-2e50-40bb-8a12-848471825d5b` now builds production0warnings/0errors and observes actual hubIT-13/PID24984/DLL `8855bc2d619b33a4febeb87556cfcec1d36c45710b186013dddb8574de28fcc2`. It stops at **character-harness-build:22errors/0warnings**. The authored embedded C# harness has MA0006 string equality/inequality errors, MA0002 missing ordinal dictionary comparer, and MA0011 missing invariant formatting on two integer `ToString` calls. Exact original diagnostics, arguments, PID and stderr/stdout remain in that invocation's `character-harness-build.command.json`.

No additional repair was applied. There is no result for the2976 native legal item/body/pose/facing tuples,12 independentH13 fixtures, current expanded ten compiled tests, final discovery/art/fit selection or full current source/native-policy regression. The source-side14 semantic fit controls completed before the latest strict build, but do not replace those blocked obligations. All real windows, appearance, listening, physical input, final hub/index adoption and ASSET completion remain pending.

### Exact five-file frozen WIP

Complete copies and the machine manifest, including the22 exact diagnostics and history, are in `.phase4/checkpoints/04-06-task2-exhausted/manifest.json`. Snapshot revision is765c45d; the index is empty. The ledger base remainsfbb7dbc and must not be reset.

| Path | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/content/animation/fits-a.json | 90821 | 66f5e551bcff45ec4832c408743ca13c0c5a19310dd0d90f002a5e21784b0a0c |
| src/godot-game/inspection/CharacterPreview.cs | 19949 | 90a609b0eb2bcc48d327bfdbfbdd9f891d57aeb9ff78ace37765d5f3a0c76a35 |
| src/godot-game/inspection/InspectionRoot.cs | 20173 | c58ef31d929582782c26b05c46da21d1f40d5fd185688cfb373eb44efecb8fce |
| tests/phase1/CharacterFitTests.cs | 5151 | 4cc71323c97aeb0d7c163818ef91f69c678893e44538b57b702650bfc70c6801 |
| tools/phase4/cases/bodies-a.cjs | 37859 | 9cc8a5b9e42f73e193134da9383869c4564acc7e1a1f40e9e6850eef66af609f |

Authored fit data covers48 pose-anchor sets, eleven logical slots and22 distinct legal visible items (31 slot/item choices), both directions, actual retained atlas/component bindings and exact incomplete tuples. The internal presenter/root wiring compiles and the live selected-coat behavior is observed. Complete native tuple/pixel/occlusion/replacement/nearby-body/guard/lifetime proof remains unexecuted behind the harness compile blocker; no complete fitting or visual quality is claimed. No new public Godot script, policy permission, package install or tracked implementation path was introduced.

The precise prospective repair is confined to existing `FIT_HARNESS` text in `tools/phase4/cases/bodies-a.cjs`: ordinal string comparisons, an ordinal dictionary comparer and invariant integer formatting for the reported22 diagnostics. A new explicit bounded authority is needed before applying it. It must retain every2976-tuple/pixel/empty/replacement/H13/guard/diagnostic assertion, all original art controls and strict0warning/error production AND harness gates. It cannot grant a timeout increase, new path, count-only pass, permission expansion, budget reset or a rewritten historical result. After a successful bounded full gate, finish the remaining original Task2 obligations, commit its exact five files, then write/self-check/commit the canonical SUMMARY immediately. Parent continues to own global progress and draftPR5.
