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
