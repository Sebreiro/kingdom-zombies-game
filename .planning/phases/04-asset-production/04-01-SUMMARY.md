---
phase: 04-asset-production
plan: "01"
subsystem: content-production
tags: [Godot, UnityPy, provenance, headless, PNG, negative-controls]
requires:
  - phase: 03-four-player-lan-foundation
    provides: Pinned Godot project, engine toolchain and isolated proof conventions
provides:
  - Exact K20 bow export with retained original export, sprite bytes and reproduction recipe
  - Declarative native inspection scene loading the production bow
  - Source-bound headless tracer and eleven real negative controls
affects: [04-25, 04-02, 04-03, 04-04, 04-23, 04-30, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [read-only donor selection, retained production bundle, isolated headless scene probe, immutable invocation evidence]
key-files:
  created:
    - tools/phase4/check.cjs
    - src/godot-game/assets/common/bow.png
    - src/godot-game/content/packs/tracer.json
    - src/godot-game/inspection/Inspection.tscn
    - production/phase4/tracer-source.json
  modified: []
key-decisions:
  - "The tracer uses a declarative scene and an invocation-owned headless probe; no production C# native-policy permission is added."
  - "Validate original RGBA bytes and all visible imported pixels separately because Godot processes fully transparent edge RGB."
  - "The runtime scene and source.tracer need project content only; authoring export and original-preservation controls explicitly use the read-only donor root."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-05]
coverage:
  - id: bow-native-tracer
    description: Exact recovered bow, retained source identity and real native scene load
    requirement: ASSET-01
    verification:
      - kind: integration
        ref: .phase4/runs/50133c93-3e47-4f3e-8afc-8e331a8f0cdd/result.json
        status: pass
    human_judgment: false
  - id: tracer-failure-controls
    description: Eleven fresh failed subjects, restored and unchanged-production native controls, original hash preservation
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: .phase4/runs/21c5877b-a8a3-433a-b6e1-b262ce9123ee/result.json
        status: pass
    human_judgment: false
actuals:
  tokens: 13056
  tasks: 2
  commits: 2
plan_head_before: 44dbb0975e4d40ff873a781a19c88aecd2611a2a
plan_head_after: f5097b8573223f3c48a0383a9b342ff429c53898
duration: 10min
completed: 2026-11-16
status: complete
---

# Phase 04 Plan 01: Production donor-to-inspector tracer Summary

**K20 Sprite 7008 exports to a preserved 16×18 bow PNG and loads through the pinned headless Godot scene, with eleven actual rejection controls and fresh restored passes.**

## Performance

- Tasks: 2/2; five new implementation files; no compiled source changes.
- Measured production/evidence window: first owned invocation directory at 2026-11-16T12:51:26Z through close-out around 13:01Z, approximately ten minutes. Initial context reading precedes this durable window and is not included in this duration.
- Actual tokens use `ceil(52223 / 4) = 13056`, measured over `git diff --binary 44dbb097..f5097b8`. The two measured commits exclude subsequent summary/progress documentation commits.
- Branch: `feature/04-asset-production`; existing draft PR #5 targets `dev`. Parent retains PR updates and all phase acceptance/merge decisions.

## Accomplishments

- Exported actual native bow pixels through installed UnityPy 1.23.0/Pillow 13.0.0/Python 3.12.14. No package installation or generated replacement art was used.
- Recorded owning file, Sprite/atlas/texture identities, original archive/stream hashes, selected stream range, logical registration and exact image hashes. Retained the original export, raw Sprite bytes/type tree, texture tree and complete extraction recipe in the production bundle.
- Added a separate declarative inspection scene and compact identity/name/status labels. Ordinary foundation launch remains unchanged.
- Added stable source/headless cases, a bounded case registry, isolated process homes/temp, authenticated engine/version checks, current-input snapshots, output limits, timeouts, owned descendant cleanup and exclusive per-invocation evidence writes.
- Observed eleven genuine failed subjects plus fresh restored and production native passes; donor archive/stream and production inputs remained unchanged.

## Task Commits

1. Task 1 — Inspect one real recovered bow from exact source to native resource: `88d5fc2ff8a0773d542321bedb63806650440ed2` — `feat(04-01): trace recovered bow through pinned headless inspection`.
2. Task 2 — Prove the production tracer fails honestly on bad content: `f5097b8573223f3c48a0383a9b342ff429c53898` — `test(04-01): reject bad tracer content and interrupted native loads`.

## Verification and Evidence

The final `node tools/phase4/check.cjs --group source` executes the two plan commands on committed revision `f5097b8573223f3c48a0383a9b342ff429c53898`. All paths below are repository-relative, ignored local evidence; source and media remain tracked.

| Subject | Actual result / immutable invocation |
| --- | --- |
| `node tools/phase4/check.cjs --case source.tracer` | PASS; `.phase4/runs/50133c93-3e47-4f3e-8afc-8e331a8f0cdd/result.json` |
| `node tools/phase4/check.cjs --case source.tracer-failures` | PASS; `.phase4/runs/21c5877b-a8a3-433a-b6e1-b262ce9123ee/result.json`; eleven expected failed children, fresh restored and production native controls |
| Post-Task-1 tracer feedback gate | PASS; `.phase4/runs/1dc056cc-5285-4e2d-bd7a-64a878cdd980/result.json`; expansion proceeded after committed tracer verification |
| Restored disposable media control | PASS; `.phase4/runs/a8c71ec9-194f-4c53-82fa-6856f30df666/result.json` |
| Unchanged real production control | PASS; `.phase4/runs/1e46a162-0f8f-415f-bf5f-8d4ff09b3df6/result.json` |
| Original donor preservation | Exact before/after equality for the selected 58,725,896-byte archive and 303,786,800-byte texture stream; `originals-before.json` and `originals-after.json` in final suite invocation |
| Syntax / whitespace | `node --check tools/phase4/check.cjs` and `git diff --check` passed |
| Inherited strict solution build | Inapplicable: no compiled inputs changed. No build or analyzer pass is claimed. |
| Evaluation scope | `gsd-tools.cjs check evaluation-scope --plan 04-01 --commits-only --raw` resolved both task commits on this branch |

Every successful native check creates a fresh owned miniature project containing the exact tracked scene/texture/shard bytes. The pinned engine imports those bytes, instantiates `res://inspection/Inspection.tscn`, decodes the original PNG, checks its original RGBA hash and every visible imported pixel/alpha, verifies registration and labels, frees the scene, and emits the current nonce's observed completion. The probe is headless and does not require a rendered prelaunch review. No actual game window opened.

| Deliberate subject | Expected observed failure | Final failed child nonce |
| --- | --- | --- |
| Missing bow PNG | `RESOURCE_MISSING common.bow src/godot-game/assets/common/bow.png` | `a255ec45-a01b-4a7e-a7b9-7658fa57b245` |
| Corrupt bow PNG | `MEDIA_HASH_MISMATCH` with exact bow path | `23814e1d-0cf9-4ce7-8329-d3709652924e` |
| Missing manifest | `RESOURCE_MISSING` with exact shard path | `0ec2c0c8-d192-4cb0-9fbe-f8ae31a2638e` |
| Corrupt manifest | `JSON_INVALID` with exact shard path | `89d5ae90-e0b5-48c4-8322-17eed438d890` |
| Duplicate manifest key | `JSON_DUPLICATE_KEY` with exact shard path/key | `0557697e-686c-413d-b9c9-c0044eac5aea` |
| Changed source object ID | `SOURCE_IDENTITY_MISMATCH` with exact shard path | `729c444f-f7d0-441d-81dc-2d6aa854f66c` |
| Changed preserved-original hash | `SOURCE_IDENTITY_MISMATCH common.bow originals`; mutation record identifies `production/phase4/tracer-source.json` | `378b0552-a25b-4786-9815-67102a4ffa17` |
| Missing scene | `RESOURCE_MISSING` with exact scene path | `aaec4920-9ae7-4715-8703-8e696bb313d4` |
| Corrupt scene | Actual `NATIVE_IMPORT_ERROR` with exact scene path | `d67510db-ffa3-46b3-934f-5448926cddeb` |
| Scene references absent native texture | Actual `NATIVE_LOAD_ERROR` with scene path; engine stderr names `absent-bow.png` | `137f61d1-c045-4723-ae67-e2a2edc10142` |
| Loader interrupted after observed load | `LOADER_INTERRUPTED`; current `TRACER_LOADED` present, `TRACER_COMPLETE` absent, streams closed and no cleanup error | `61219ca6-d722-467c-ae77-de9af630c39a` |

Each failed child retains its own failed result, mutation description/payload where present, input/command evidence where reached and original invocation nonce. The suite records expected rejection separately from subject failure. Restoring the owned missing-media copy does not rewrite its original failure record or reuse old success.

## Source and Media Identities

| Identity | Exact value |
| --- | --- |
| Output / version | `common.bow` / `1`; shard `tracer` / `1`; partial master for `IT-07` |
| Source | K20, `sharedassets0.assets`, type `Sprite`, object `7008`, name `tools_bow` |
| Atlas | Same owning file, `SpriteAtlas` object `25376`, name `objects` |
| Texture | Same owning file, `Texture2D` object `178`, name `sactx-0-1024x1024-Uncompressed-objects-07654c7b` |
| Stream | `sharedassets0.assets.resS`, offset `280718128`, size `4194304`, SHA256 `620d3a6fd3f441ea4e02f07bef8479f118d451076e29917d292797babb3bad53` |
| Archive SHA256 | `3648658d475170661517c4907e50977a4ab932441b404a681cd0ce485592f3ff` |
| Whole stream SHA256 | `2efccae88002a02ca85f050a6d6dc2e5fe894c32de80e36f30cd12f8c7ffb5c2` |
| Preservation manifest SHA256 | `38a914a53ba9e3a618e6b97942ee3ae382ade702d6ce1f04485e583b4923d31d` |
| Raw Sprite bytes SHA256 | `12a7303fa0815fb8cd0ac10630e934dd91b87777c041838ada7cfa8259fa4023` |
| PNG | 135 bytes, 16×18 RGBA, 42 visible pixels, alpha bbox `[6,1,10,17]`, SHA256 `08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999` |
| Decoded RGBA SHA256 | `88b8e7062f0eb9cfb788ef3658028ba58a6f62d408d3e39ae2e1228bfa6c8905` |
| Registration | Full 16×18 logical rectangle; bottom-left normalized pivot `(0.375,0.5)`; source offset `(-2,0)`; trim `(0,0)`; 32 original pixels/unit; packed settings `67`; atlas rect `(578,526,16,18)`; atlas offset `(576,526)`; no separate alpha texture |
| Godot registration | Nearest filtering, scale `(1,1)`, uncentered, top-left offset `(-6,-9)`; production world density remains owned by the later assembled scale proof |
| Engine | `4.7.2.stable.mono.official.ed1daf0bf`; executable SHA256 `2445d009a5e0474fc7064b9767100e9d2c09521890ac5625476bfb81cf03f2d4` |
| Final CLI SHA256 | `f60ab67e5ed06034ae2cc6ca92c5fc6491a800cc703124214292d3e4957f9f58` |
| Scene SHA256 | `66702efb9bf9390f6454316ce0ec5bc7808cb5f439addf0430d46abccb589739` |

## Artifacts this phase produces

All five planned implementation files were created. No additional tracked implementation file or native permission file was added.

| Exact file | Actual declarations, resources and retained members |
| --- | --- |
| `tools/phase4/check.cjs` | CommonJS constants `ROOT`, `PYTHON`, `DONOR`, `ID`, `FILES`, `ORIGINALS`, `EXPECTED`, `IMAGE`, `REGISTRATION`, `RECIPE`; registry `cases`. Functions `hash(value)`, `need(condition,code,detail='')`, `inside(root,relative)`, `bytes(root,relative,limit=1048576)`, `document(root,relative)` with local recursive `walk(value,depth)`, `save(root,relative,value)`, `environment(directory)`, `terminate(child)`, async `command(context,name,executable,args,{timeout=60000,interruptMarker}={})` with local `stop(code)`, `fingerprint(root)`, `createRun(caseId)`, `engine()`, `validate(root)`, `probeScript(nonce,image,interrupt)`, async `runTracer(root,context,{interrupt=false}={})`, async `exportTracer()`, async `originalIdentities()`, `copySubject(directory)`, async `tracerFailures(context)`, `registerCase(id,definition,run)`, async `main(args)`. Module exports exactly `main,registerCase,cases,runTracer,validate,hash,inside,document,command,environment`. Node built-in aliases `fs,path,crypto,spawn,spawnSync,TextDecoder,equal`. |
| CLI/case contracts in `check.cjs` | `--case ID`, `--group GROUP`, `--list`, authoring `--export-tracer`. Installed group `source`; stable cases `source.tracer`, `source.tracer-failures`; G0 orders 1/2, High importance, source/headless-only scenario metadata, independent per-case outcomes. Future names fail `CASE_PENDING_OR_UNKNOWN`. Authoring export is for absent destinations; repeatable multi-output publication remains 04-25 work. |
| Invocation-owned scripts emitted by `check.cjs` | `export.py` contains retained `RECIPE`; Python helpers `sha` and `portable` are lambdas. `project/probe.gd` extends `SceneTree` with `_initialize()`, `reject(reason)`, `inspect()`. It instantiates the actual tracked scene and checks native pixels/registration/labels. These are ignored evidence artifacts, not production C# adapters. |
| `src/godot-game/assets/common/bow.png` | Actual unchanged native-pixel bow export; independent stable ID `common.bow`, version 1, 16×18 RGBA. |
| `src/godot-game/content/packs/tracer.json` | Schema 1, shard ID `tracer`, version 1; sole resource `common.bow`; path `assets/common/bow.png`; scene `inspection/Inspection.tscn`; exact source/image/registration records; explicit `IT-07` partial-master status and incomplete phase pack. No `pack-index.json` is created early; 04-25 owns census/index expansion. |
| `src/godot-game/inspection/Inspection.tscn` | External resource `1_bow`, type `Texture2D`, `res://assets/common/bow.png`; no subresources/scripts. Nodes `Inspection:Node2D`, `Bow:Sprite2D`, `Summary:VBoxContainer`, `Summary/Title:Label`, `Summary/ResourceId:Label`, `Summary/ResourceName:Label`, `Summary/Status:Label`, `Summary/PackStatus:Label`. |
| `production/phase4/tracer-source.json` | Stable output/source/preservation/registration/image/tool/validation records. Retained bundle members `bundle.originalExportPngBase64`, `bundle.rawSpriteBase64`, `bundle.rawSpriteTree`, `bundle.textureTree`; `recipe.language`, full `recipe.code`, `recipe.sha256`, and all three transformation descriptions. Exact archive/stream identities and source object dependencies retained as provenance; no whole donor archive copied. |
| `.phase4/runs/<nonce>/` | Ignored invocation-owned `invocation.json`, `inputs.json`, `engine.json`, `result.json`, optional `setup-failure.json`, `export.py`, staged `bow.png`/`source.json`, process `.command.json`/`.stdout.log`/`.stderr.log`, `mutation.json`/`mutation.bin`, originals before/after files, `subject/` copies, `project/project.godot`, `project/probe.gd`, copied actual scene/shard/PNG and engine-derived import files; isolated `home,temp,data,config,cache,packages,http-cache` directories. All named final child nonces above remain preserved. |

**Exact production native-policy tuples introduced:** none. No authored C# file, public script type, generated-source permission, visibility exemption, or engine-access policy entry changes in this plan. Native resources are declarative; ephemeral headless verification uses engine-owned GDScript. Later root/loader/input/shell permissions remain assigned to their checked plans.

## Decisions Made

- Keep extraction and live donor-preservation proof explicit authoring operations; the normal native scene and clean `source.tracer` resolve project content and retained provenance only.
- Use a fresh isolated native project per invocation to avoid developer caches, foundation startup or compiled gameplay dependencies. Copy the exact scene bytes and require current observed completion.
- Keep original decoded RGBA hash distinct from imported texture equality: default Godot alpha-border processing can change fully transparent RGB, while all alpha and visible colors must remain exact.
- Leave `ASSET-01`, `ASSET-02` and `ASSET-05` pending at requirement level. This plan contributes a single production tracer; complete census, hub, portable export and actual acceptance belong to later phase plans. Marking these whole requirements complete here would overstate delivered scope.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Compared identity objects structurally instead of insertion order**
- Found during Task 1 export verification. Python's stream-key order differed from the independent JavaScript expectation.
- Changed exact object comparisons to `isDeepStrictEqual`, preserving strict values/ordinal strings without treating field order as identity.
- File: `tools/phase4/check.cjs`; commit `88d5fc2`.

**2. [Rule 1 - Bug] Accounted for native transparent-edge import processing**
- Found during Task 1 real loader. An exact imported raw RGBA hash failed because Godot changes invisible edge RGB.
- The loader now verifies the original PNG's complete decoded hash, then checks every imported alpha and every visible pixel color against that original. It does not weaken visible-pixel preservation.
- File: `tools/phase4/check.cjs`; commit `88d5fc2`.

**3. [Rule 1 - Bug] Distinguished import failure from runtime scene-load failure**
- Found during Task 2's actual missing native texture control: editor import completed, then the real scene loader rejected it.
- Added exact `NATIVE_LOAD_ERROR` scene context while preserving the actual command/error logs and failed invocation.
- File: `tools/phase4/check.cjs`; commit `f5097b8`.

These corrections stayed within the five authorized files and the two planned tasks. Strict duplicate-key validation and pinned registration implement the plan's parser/provenance threat mitigations. No additional tracked implementation file or architecture was introduced.

## Issues Encountered

- Ordinary sandbox Node child spawning returned `spawnSync git EPERM`. Authorized bounded commands succeeded through escalation; this was an environment permission boundary, with no approval-review rejection and no hook bypass. The initial empty invocation directory `1196db74-8ade-418a-a263-f6c4d48382d3` remains historical; subsequent failed setup handling now writes `setup-failure.json`.
- Preserved failed authoring comparison `23821a53-5734-4889-8b7f-0e2c5e65b630`, failed original imported-pixel attempt `15eb16d6-9610-45db-b848-7df08b3b1497`, and failed initial negative-suite classification `aac4c3db-d86c-49b6-beb4-fb6289158ef5`. They supply no current success credit.
- Successful original export is `.phase4/runs/e1b18629-e219-4750-8b30-f7e3b427ca1a/`; original recipe/output hashes are retained in tracked production data.

## Acceptance Limits and Next Readiness

- This completes the planned source/headless tracer slice. The scene identifies resource inspection and incomplete pack status. It does not implement the complete pack inspector early.
- No full resource family, 180-obligation coverage, H/UI backstop, rendered appearance, listening, independent judgment, Windows export, gameplay integration or Phase 4 acceptance is passed by this result. No ASSET requirement is completed here.
- Actual Windows 1280×720 review/run/listening and separate named judgments remain 04-32/04-33 responsibilities after whole implementation and independent review/fixes. Full policy, census, atomic repeatable publication and artifact reconciliation retain their assigned plan owners.
- No known stub blocks this slice; no skipped test or applicable unrun verification remains. The build applicability condition is false. No new threat surface outside the plan's local parser/media/native/evidence boundaries was introduced.

## Self-Check: PASSED

- All five produced implementation files exist; PNG/scene/source/shard identities are recorded and exercised by the final committed-source cases.
- Both task commits exist and are ancestors of current phase HEAD; evaluation scope resolves exactly these two task subjects before summary close-out.
- Both plan verification cases pass freshly on `f5097b8`; original and production invariance checks pass; eleven negative child results remain failures.
- Syntax/whitespace checks pass; stub-pattern scan found no `TODO`, `FIXME`, `placeholder`, `coming soon` or `not available` content in the five produced files. Ordinary foundation entry configuration is unchanged.
