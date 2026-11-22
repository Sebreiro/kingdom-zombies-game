---
phase: 04-asset-production
plan: "25"
subsystem: content
tags: [provenance, census, atomic-publication, node, source-headless]
requires:
  - phase: 04-01
    provides: Exact retained bow export, source bundle, shard, scene and extensible case registry
provides:
  - Independent exact 180-obligation expectation and versioned incomplete pack index
  - Strict selected-origin, retained-byte, dependency, recipe and evidence validation
  - Exclusive immutable-generation publication and bounded interrupted-owner recovery
  - Independently observable source H01-H05/H33 controls with actual interrupted workers
affects: [04-02, 04-03, 04-04, 04-23, 04-30, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [exact ordinal identities, immutable generations, atomic manifest pointer, fail-closed dead-owner recovery]
key-files:
  created:
    - tools/phase4/content.cjs
    - tools/phase4/expected-obligations.json
    - tools/phase4/cases/provenance.cjs
    - src/godot-game/content/pack-index.json
    - docs/content/production.md
  modified:
    - tools/phase4/check.cjs
key-decisions:
  - "Publish validated partial content with explicit incomplete status; current bow contributes zero completed obligations."
  - "Retain immutable output identities and generations; replace only the validated current manifest pointer under an exclusive lock."
  - "Keep ASSET requirements pending until 04-33 and native/listening/judgment acceptance pending 04-32."
requirements-completed: []
requirements-contributed: [ASSET-01, ASSET-02, ASSET-05]
coverage:
  - id: exact-provenance-census
    description: Independent expectation, strict output/source identities and truthful partial-pack diagnostics
    requirement: ASSET-01
    verification:
      - kind: integration
        ref: .phase4/runs/a4acb025-ebf0-4c50-ae9e-8a3d14fa121f/result.json
        status: pass
    human_judgment: false
  - id: atomic-publication-controls
    description: Actual interrupted workers, competing live process and stable repeated publication/import
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: .phase4/runs/c5d218a1-701c-4133-8c68-ef72b294b281/result.json
        status: pass
    human_judgment: false
actuals:
  tokens: 21994
  tasks: 2
  commits: 2
plan_head_before: e3da87a90b78eba4ba68a4cc2b38267d2faec9a8
plan_head_after: f411a076d089e6eeeaee7255ab71f4d3684e78d8
duration: 12min
completed: 2026-11-16
status: complete
---

# Phase 04 Plan 25: Exact census/provenance and atomic repeatable publication Summary

**An independent 180-ID register validates exact bow provenance and publishes immutable partial-pack generations, with four actual interrupted workers and live competing-process rejection.**

## Performance and scope

- Two tasks complete; six implementation files, with at most five per task after the approved bounded amendment. Source/headless only; no game window, audio listening, image judgment, compiled source or native policy change.
- Durable implementation window begins at the first supplied-root guard, 2026-11-16T13:07:29Z, and closes around 13:20Z (approximately twelve minutes). Mandatory context loading precedes that window.
- Actuals are `ceil(87976 / 4) = 21994`, measured from the exact UTF-8 decoded `git diff --binary` output over the two task commits. `git rev-list --count e3da87a..f411a07` is exactly 2. Summary/progress metadata commits are excluded by this persisted-ledger measurement boundary.
- Branch remains `feature/04-asset-production`; parent owns existing draft PR #5 to `dev`, further wave coordination, independent review and phase acceptance. No branch, merge, stash, reset or PR action occurred.

## Accomplishments

- Authored a durable independent expectation from the authoritative resource rows and explicit TXT prose, separately from all output enumeration: CH 13, AN 18, IT 24, BL 16, EN 11, FX 12, UI 27, AU 30, DT 27, TXT 2 = exactly 180 canonical unique IDs.
- Created a versioned index containing 29 plan-owned shard paths: one selected tracer and 28 pending shards. The actual bow retains exact bytes/source/recipe/dependencies. It is a partial IT-07 master and contributes zero fully completed obligations; all 180 incomplete obligation diagnostics remain present.
- Validate actual selected files, hashes, owner/type/object identity, retained raw/export bytes, stream ranges, atlas/texture dependencies, recipes and applicable metadata. Shared byte paths require explicit common identity without collapsing distinct output/obligation IDs. V/M/C remain historical input evidence.
- Stage exact selected bytes under an exclusive lock, validate the staged tree, retain immutable generations and atomically replace only the current manifest pointer. Reject lost/altered validated outputs, missing bytes, path escapes/reparse ancestry, false completion and missing/stale evidence. Hard interruption fails closed until explicit dead-owner recovery.
- Store reusable Critical G0 scenarios and individual source outcomes for H01-H05/H33. Actual final native/listening/judgment ownership remains separately pending 04-32/04-33.

## Task Commits

1. Task 1 — Validate exact census, identities and atomic production publication: `34921b93783c6756e3a3655a597a4cb362aa94c4`, `feat(04-25): validate exact content provenance and stage atomic pack publication` (five files).
2. Task 2 — Refine interrupted, competing and repeated production publication: `f411a076d089e6eeeaee7255ab71f4d3684e78d8`, `feat(04-25): prove bounded interruption contention and repeated publication` (three files; content/cases overlap Task 1).

## Verification and exact evidence

Final `node tools/phase4/check.cjs --group source` ran freshly on committed revision `f411a076d089e6eeeaee7255ab71f4d3684e78d8`. All evidence below is ignored invocation-owned local evidence, not newly claimed visual/audio acceptance.

| Case / check | Observed result / exact evidence |
| --- | --- |
| `source.tracer` | PASS; `.phase4/runs/cdfb9d46-77aa-45b9-ba8a-3dea4adba931/result.json`; actual pinned headless native bow load |
| `source.tracer-failures` | PASS; `.phase4/runs/aa642e43-af66-48b2-b2e0-ad3e07156b1b/result.json`; eleven deliberately failed subjects plus fresh restored and production controls; exact donor hashes unchanged |
| `source.provenance` | PASS; `.phase4/runs/a4acb025-ebf0-4c50-ae9e-8a3d14fa121f/result.json`; actual bytes/source invariance, exact census, separate H01-H04 and basic H05/H33 controls |
| `source.publication` | PASS; `.phase4/runs/c5d218a1-701c-4133-8c68-ef72b294b281/result.json`; actual interrupted/competing processes, recovery, path and repeated import controls |
| Committed Task 1 tracer feedback | PASS; `.phase4/runs/bbdd1853-723f-46dd-8b33-0de1de078ef0/result.json`; verified before Task 2 expansion |
| Syntax / whitespace | `node --check` on content/case modules and `git diff --check` pass |
| Evaluation scope | SDK resolves both exact 04-25 task commits as ancestors on this branch |
| Strict compiled solution build | Inapplicable under plan's compiled-input condition; no compiled inputs changed and no build/analyzer pass is claimed |

H01 separately resolves independently authored same-name Sprite/Texture2D selections across `heldout-a.assets`/`heldout-b.assets`, IDs 48/49, and verifies two output identities sharing bow bytes with explicit `shared.bow-pixels`. H02 rejects missing origin/version/recipe/hash/output dependencies/source dependencies/source stream/evidence and falsely verified/stale records. H03 rejects case aliases, decomposed identity, invalid UTF-8, duplicate JSON keys and unsafe paths. H04 compares shuffled outputs/shards/origins with the same exact resolution and coverage. Each bad subject retains explicit failure evidence; suite success means expected rejection was observed.

The final publication invocation records four actual owned workers interrupted after `PUBLICATION_PAUSED <nonce> <stage>` at `locked`, `staged`, `validated` and `before-commit`. Each command records `LOADER_INTERRUPTED`, closed streams and no cleanup error; its own result remains `status: interrupted`, `verified: false`. Exact recovery verifies its dead PID/invocation, preserves the prior current pointer, removes only owned orphan staging/pointer/generation data and releases its lock. A separate competitor fails on the retained lock; `live-competition.command.json` additionally observes a separate competing Node process rejected while its owner remains alive. Live/wrong-owner recovery is rejected. Three fresh publication/import runs preserve the bow's exact stable ID/version/hash without duplicate provenance; an explicit two-output shared composition remains distinct and incomplete.

Original-root denial is checked before writes; no new provenance/publication case needs the external donor root. The affected inherited tracer-failure regression separately reads original donor archives/streams and confirms exact before/after hashes. All new source/index/bundle/media snapshots compare equal before/after the cases. No invocation borrows previous run completion.

## Source and media identities

| Artifact | SHA256 |
| --- | --- |
| `tools/phase4/check.cjs` | `19f649cb864f787a184105fd0aa0de2d6716efdc37ac61bc9aab491f0571733d` |
| `tools/phase4/content.cjs` | `0c2d179c24e867bc9142edc2aa2842bc0e93811f1e355fae54daa9039da062c2` |
| `tools/phase4/cases/provenance.cjs` | `ebdd31f09ac8051426c1d62b10e4ee39d7d4b8136ab2a5b79bd15378af298d99` |
| `tools/phase4/expected-obligations.json` | `a4d9243bda1b512b59c189a3bdc48a7cccc8171a218bbfc5ae16b00274f2fda5` |
| `src/godot-game/content/pack-index.json` | `8628345ce26689b8478cb98ae60adb924919839a1b66ea3e2dddd451e1758c8f` |
| `docs/content/production.md` | `1dbb0a65c0e4f550296088b164da6770b46a367c0c622411a0af7a6d1c595244` |
| Existing unchanged bow PNG | `08b53699e459d71cf73f3e7be84af19d04805d5a7a488121e39a514a81701999` |
| Existing retained source bundle | `03673e2f892369263a54fca6aee26684bc5f3f0b9aee540f3c001d190bd45fac` |

## Artifacts this phase produces

No authored C#, generated permission, native adapter, new media, original export or whole donor archive is introduced. Every actual implementation declaration/field/ID and retained member is inventoried below.

| Exact file | Actual symbols / contract |
| --- | --- |
| `tools/phase4/check.cjs` | One added `require('./cases/provenance.cjs')` after existing exports and before CLI `main`. Existing registry, cases, exports and native scenarios remain intact. |
| `tools/phase4/content.cjs` | Constants/aliases `fs,path,crypto,TextDecoder,equal,spawnSync,ROOT,INDEX,EXPECTED,MAX_FILE,MAX_TOTAL,hash,document,digest`. Functions `need(ok,code,detail='')`, `text(value,label)`, `relative(value)`, `inside(root,name)`, `bytes(root,name)`, `parse(value,label)` with local `walk(item,depth)`, `fileRef(root,ref,files)`, `expected(root=ROOT)`, `sourceKey(source)`, `resolveOrigin(origins,selection)`, `recipe(root,ref,files)`, `validate(root,index=document(root,INDEX),register=expected(),{requireComplete=false}={})`, `merge(base,addition)` with local `key(item)`, `destination(root)`, `write(root,name,value)`, `publish({sourceRoot,index,publicationRoot,invocation=crypto.randomUUID(),observe=()=>{}})`, `readPublished(publicationRoot)`, `recoverPublication({publicationRoot,invocation})`, `worker(args)`. Exact exports: `ROOT,INDEX,EXPECTED,hash,need,text,relative,inside,bytes,parse,document,expected,sourceKey,resolveOrigin,validate,merge,destination,write,publish,readPublished,recoverPublication,worker`. |
| Content return/publication contracts | Validation fields `status,complete,valid,diagnostics,covered,expected,files`; valid output projection `id,version,path,sha256,origins,obligations`; diagnostics `code,id,path,message`. Writer options above, observed stages `locked,staged,validated,before-commit`. Lock `invocation,pid`; current pointer `schemaVersion,invocation,generation,indexSha256,status`; publication result `invocation,generation,status,complete,outputs`; recovery `invocation,status,published,cleanup`. CLI `--publish-worker sourceRoot publicationRoot invocation pauseAt`, modes `none,locked,staged,validated,before-commit,compete`; bounded live nested competitor, observed interruption markers and nonzero failures. |
| `tools/phase4/cases/provenance.cjs` | Aliases `fs,path,equal,content,runner` and destructured content helpers; `definition`. Functions/exports `snapshot()`, `subject(context,name)`, `failure(context,name,code,action)`, `invalid(context,name,code,edit)`, `reference(root,name)`, `sharedSubject(context)`, `provenance(context)`, async `publication(context)`. Registers `source.provenance` and `source.publication`, Critical G0 orders 3/4, with requirements/acceptance, setup/actions/expected/reset/evidence/importance/rationale/no-window metadata. Individual held-out outcome keys H01/H02/H03/H04/H05/H33; `heldout.H05` in interrupted subject results. Fixture-only IDs `heldout.second-bow`, `shared.bow-pixels`, `FX-01` partial mapping and independent source locators described above; no fixture output enters the real index. |
| `tools/phase4/expected-obligations.json` | Schema `schemaVersion,authority,scope,obligations`; each expected row `id,name,source`. Explicit 180 IDs: CH-01 through CH-13; AN-01 through AN-18; IT-01 through IT-24; BL-01 through BL-16; EN-01 through EN-11; FX-01 through FX-12; UI-01 through UI-27; AU-01 through AU-30; DT-01 through DT-27; TXT-01 and TXT-02. Durable names/source-row locators come from the resource authority independently of output manifests. |
| `src/godot-game/content/pack-index.json` | Schema 1, ID `kingdom-zombies.v0.1`, version 1, incomplete, `acceptanceOwner:04-32/04-33`. Top-level `schemaVersion,id,version,status,acceptanceOwner,shards,origins,outputs`. Shard fields `id,ownerPlan,status,path,version,bytes,sha256` (last three selected only). Origin fields `owner,type,objectId,name,ownerSha256,rawSha256,streams,dependencies`; stream `path,offset,bytes,sha256,rangeSha256`; source dependency `owner,type,objectId,name`. Output fields `id,name,version,shard,path,bytes,sha256,route,origins,production,dependencies,metadata,applicability,inspection,evidence,historicalInputEvidence,obligations`; production/file refs `path,bytes,sha256`; dependency additionally `id`; metadata `kind,width,height,rgbaSha256,registration`; applicability `obligations,listening`; inspection `route,dependency`; evidence `source,native,listening,judgment,records`; proof refs additionally `kind,caseId,invocation`; historical `level,credit`; obligation `id,coverage`; explicit shared-output field `sharedFileId` is exercised in fixtures. Sole real output `common.bow` version 1, dependency `inspection.bow`, source K20 `sharedassets0.assets` Sprite 7008 `tools_bow`, atlas 25376 and texture 178, retained exact stream/range hashes and registration; IT-07 partial only. |
| Pending shard inventory | Paths all `src/godot-game/content/packs/<id>.json`: `audio-actions,audio-interface-ambience` (04-21); `bodies-a` (04-06); `bodies-b,bodies-c` (04-07); `camp-access,main-buildings` (04-11); `common,scale-pilot` (04-04); `compatibility,text-data` (04-22); `corpses-wildlife,player-identities` (04-10); `defenses,guards` (04-12); `delivery` (04-31); `effects-portals,world` (04-13); `items-01-10,items-11-24` (04-05); `ui-camp,ui-map-warnings` (04-16); `ui-identity` (04-28); `ui-inventory,ui-legacy-stash` (04-15); `ui-kit` (04-14); `ui-session,ui-settings` (04-17). Existing selected `tracer` belongs to 04-01. Pending rows are planned references, not missing authored stub outputs claimed as completed. |
| Retained original source/recipe members | Existing `production/phase4/tracer-source.json` is referenced and copied exactly, never modified: `bundle.originalExportPngBase64`, `bundle.rawSpriteBase64`, `bundle.rawSpriteTree`, `bundle.textureTree`; `recipe.language,code,sha256,transformations`; source/preservation/registration/image/tools/validation records. The three existing transformations preserve UnityPy atlas decoding, native 16x18 RGBA pixels/lossless PNG and donor pivot via Godot offset. Existing recipe SHA256 `91c15dfa11f14a7489cb91ab961628ad053e02a9b4f502712728ae91ba658c3b`; no new generation prompt, seed, raster or audio bundle member. |
| `docs/content/production.md` | Production/reference page documents independent census, schema/route/evidence/applicability, exact source/shared output contracts, public authoring API, portable bounded paths/UTF-8, staging/pointer/lock/recovery, reusable source scenarios and explicit final evidence limits. |
| Ignored invocation-owned artifacts | `.phase4/runs/<nonce>/inputs.json,inputs-after.json,pack-diagnostics.json`, individual bad-subject JSON outcomes/copies, actual command/stdout/stderr logs, four interrupted subject results, `competing-subject.json,publication-trials.json`; `publication/` and `composition/` own `publication.lock`, `stage-<invocation>`, `current-<invocation>.json`, `current.json`, `generations/<invocation>/` copied exact index/shard/media/source/dependency tree, and `live-competition.json`. Successful generations and original failed reports remain preserved. Cleanup removes only owned unreferenced interruption residue. |

**Exact production native permission tuples introduced: none.** No public C# type, compiled dependency, engine-access path/member, generated source span or visibility exemption changes.

## Decisions and acceptance limits

- Validated publication and complete obligation acceptance are separate results. The real pack remains Incomplete with one partial bow and zero completed obligations. No count, status string, fallback, historical donor sample or interrupted run can mark it complete.
- Existing output identity is immutable within a publication lineage. Future additions compose by exact keys, explicitly selected shard transitions and distinct shared outputs rather than directory order. Output replacement requires a separate version/update contract; this plan does not silently overwrite existing validated output descriptors.
- Clean provenance/publication runs need packaged project content only. Live donor extraction/preservation is an explicit authoring concern; no donor callback is executed by the new modules.
- Leave all ASSET-01/02/05 requirements Pending. Final complete census, actual media families, Windows delivery, whole-phase independent review, native/listening acceptance and four separate judgments retain their scheduled owners. No whole-phase completion is claimed from these source backstops.

## Deviations from Plan

**1. [Rule 3 - Blocking] Narrow registry loading amendment approved before implementation.** The inherited runner exported `registerCase` but never loaded modules. The parent approved adding exactly `tools/phase4/check.cjs` and moving production.md creation to Task 2, keeping six implementation files total and five/three per task. Recorded in PLAN frontmatter before source edits; Task 1 adds only one require. Both affected tracer cases pass freshly.

**2. [Rule 1 - Bug] Preserve actual portable path spelling.** Initial lowercase-only path validation rejected the existing `Inspection.tscn`. Fixed to portable ASCII plus exact ordinal filesystem casing. Case-alias negative control still fails. Failed original invocation `.phase4/runs/f20f4bf2-dd91-421c-96ab-6a030a08c16c` is retained; corrected source gates pass. Task 1 commit contains the correction.

**3. [Rule 1 - Bug] Classify denied publication roots explicitly.** The first expanded trial rejected the donor root as `PATH_INVALID` before reaching the intended publication-root diagnostic. Moved the root allow-list check ahead of relative-path syntax validation; both conditions remain fail-closed before writes. Initial failed invocation `.phase4/runs/719a9dd4-165a-47fa-be30-34d23407955e` remains retained. Task 2 commit contains the correction.

No package install, permission bypass, architectural expansion, intentional tracked deletion, unresolved security finding, authentication gate or unperformed applicable verification remains in this slice. Necessary bounded git/Node/native subprocesses used authorized escalation because ordinary sandbox child operations are restricted; normal commit hooks ran. No automatic approval rejection occurred.

## Self-Check: PASSED

- All five created implementation files and modified runner exist; both exact task commits are ancestors of current HEAD and SDK evaluation scope resolves them.
- Fresh final committed-source selection passes all four cases; original archive/stream and actual tracked source/media snapshots remain unchanged.
- Exact persisted ledger measures two task commits, with before/after revisions and realized-diff scale recorded above.
- No TODO/FIXME/placeholder/coming-soon/not-available stub, skipped test or applicable unrun verification exists in changed implementation files. Missing future content is explicitly pending with later plan ownership, and never counted as produced art or completed pack coverage.
- Threat-surface review found only the planned bounded local parser/file/publication/evidence boundaries; no unplanned network/auth/schema trust boundary or native permission surface was introduced.
