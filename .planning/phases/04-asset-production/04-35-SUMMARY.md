---
phase: 04-asset-production
plan: "35"
subsystem: export-profile
tags: [godot, export, feature-tags, source-only, headless, bounded-processes]
requires:
  - phase: 04-34
    provides: Exact authenticated SDK managed-export layout prerequisite
provides:
  - Permanent asset_inspection feature-qualified Inspection entry with unchanged ordinary Foundation configuration
  - Single existing inspection preset with canonical feature and preserved export options
  - Actual pinned-engine settings/resource observations and independent profile/helper controls
affects: [04-02, 04-31, 04-32, 04-33]
tech-stack:
  added: []
  patterns: [Permanent feature-qualified main scene, Source-only owned native settings probe, Bounded child ownership controls]
key-files:
  created: [src/godot-game/export_presets.cfg, production/phase4/export-profile.json, tools/phase4/cases/export-profile.cjs]
  modified: [src/godot-game/project.godot, tools/phase4/check.cjs]
key-decisions:
  - "Select Inspection only through the named preset asset_inspection feature and permanent qualified setting; ordinary source startup retains Foundation."
  - "Source/headless settings and resource observations provide no packaged Inspection startup credit; unfinished 04-02 Task 1 owns the fresh native consumer proof."
requirements-completed: []
coverage:
  - id: D1
    description: Permanent source profile selects Inspection through explicit canonical features while actual ordinary settings retain Foundation
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: "node tools/phase4/check.cjs --case export.profile"
        status: pass
    human_judgment: false
  - id: D2
    description: Independent wrong-profile/path/ordinary-activation and real bounded helper controls
    requirement: ASSET-05
    verification:
      - kind: integration
        ref: "node tools/phase4/check.cjs --case export.profile-boundaries"
        status: pass
    human_judgment: false
actuals:
  tokens: 10733
  tasks: 2
  commits: 4
plan_head_before: 97f6a86e7d63ae712eb0ee7c95b55669bfce5c81
plan_head_after: deddfa1b59ba278ba420d834a524e42a4c90e0e2
duration: 13min
completed: 2026-11-16
status: complete
---

# Phase 04 Plan 35: Permanent Inspection Export Feature Summary

**The permanent asset_inspection preset feature selects the real Inspection scene while actual ordinary settings retain the exact Foundation main; 61 independent source rejections and bounded native/helper controls protect that contract.**

## Performance and Scope

Two tasks completed; exactly five implementation files changed. The measured persisted-ledger interval contains four RED/GREEN commits, excluding the earlier planning commit and later documentation commits. Actual tokens use 42,929 characters of the realized five-file Git diff divided by four and rounded upward, on the plan estimate scale. The 13-minute duration is the saved RED-to-closeout evidence window beginning 2026-11-16T15:21:28Z; preceding context-reading time was not separately timed.

This completes the source/headless delivery-profile prerequisite only. All ASSET requirements remain Pending. The one preset was transferred from unfinished 04-02; it was not recreated under a new name or duplicated for SDK configurations. No real game window, playback, listening, rendered judgment, native export, Linux run, release proof or whole-pack acceptance was performed.

## Task Commits and TDD Gate Compliance

| Task | Gate | Commit | Result |
| --- | --- | --- | --- |
| 1 | RED | 60965b3ac4a3d1d80f88356505199397ca19526b | Named source assertion fails specifically because the permanent qualified main is absent |
| 1 | GREEN | 5986b76d01a8100bfd47ac122e4c82d64f3b5ea9 | Permanent setting/preset/profile and actual settings/resource probe pass |
| 2 | RED | 5641fb7897ca3f86f353cbffbefa2cb21f0a13bb | Healthy real child accepts timeout60001, so the expected deadline rejection assertion fails |
| 2 | GREEN | deddfa1b59ba278ba420d834a524e42a4c90e0e2 | Deadline/ownership checks plus independent source/native/helper boundaries pass |

Both unchanged TAP records pass the GSD classifier with `RED_EVIDENCE_OK`: `.phase4/tdd/04-35/task1-red.json` and `.phase4/tdd/04-35/task2-red.json`. Their complete reports are `task1.tap` and `task2.tap` in that directory. Semantic review confirms that the exact named test executed and failed on the planned assertion, with no discovery, import, fixture, syntax or unrelated failure supplying RED. Both saved self-tests pass after GREEN. No separate refactor was necessary. The initial unsuccessful process-capture attempt and a hash-query syntax mistake supplied no RED credit.

## Permanent Configuration and Pinned Semantics

`project.godot` retains literal `run/main_scene="res://foundation/Foundation.tscn"` and adds only `run/main_scene.asset_inspection="res://inspection/Inspection.tscn"` in `[application]`. Removing that one added line reproduces original SHA256 `1f7d242e12521a4866512b6017f25f1868cd6160e3c9efe0074ad498008b2a9d`. The existing single `Phase4 Windows inspection` preset changes only `custom_features` from empty to `asset_inspection`; removing the feature reproduces the checkpoint's complete preset hash. Templates, x86_64, external PCK/runtime build-output options, JSON/media/font/docs inclusion and every unrelated option remain byte-identical. The five project declarations and graph are fingerprinted before and after; there is no C# source, project-edge or analyzer change.

The [official feature documentation](https://docs.godotengine.org/en/stable/tutorials/export/feature_tags.html) states that custom preset tags apply to exported projects and remain inactive in ordinary editor runs. The [pinned ProjectSettings implementation](https://raw.githubusercontent.com/godotengine/godot/4.7.2-stable/core/config/project_settings.cpp) registers authored suffixes, lowercases each suffix for explicit-feature membership lookup, and does not lowercase the supplied feature array. The actual-feature getter instead passes the authored feature spelling to `OS.has_feature`. The [pinned startup implementation](https://raw.githubusercontent.com/godotengine/godot/4.7.2-stable/main/main.cpp) selects main_scene through `GLOBAL_GET`. Local pinned GodotSharp XML confirms both actual and explicit-feature bound APIs. Canonical authored spelling is therefore checked before invoking the explicit-feature getter; a normalizing getter cannot legalize an invalid source key.

The exact production settings copy is never rewritten for a probe. A separate owned asset-only importer fixture imports copied PNG/SVG bytes; its actual imported artifacts/remaps are copied into the settings subject. A temporary GDScript SceneTree in that subject calls the actual bound ProjectSettings APIs and loads the actual copied Inspection PackedScene and bow texture. Foundation's source existence and setting are observed; this plan does not claim that its managed scene executes in this small fixture. No production probe script, arbitrary autoload or editor-as-output substitution was added.

## Verification and Immutable Evidence

All run IDs below reside under `.phase4/runs/` and retain their original nonce, invocation revision, source hashes and native command/log identities. They were executed against the final implementation bytes before the Task 2 commit; their recorded revision is 5641fb7, with exact final source fingerprints retained. Committing unchanged bytes does not relabel an earlier invocation's revision.

| Command / scope | Passing invocation |
| --- | --- |
| `node tools/phase4/check.cjs --case export.profile` | 289daddf-78d9-433e-a174-13a877d45b45 |
| `node tools/phase4/check.cjs --case export.profile-boundaries` | 350d9c66-d963-453a-a334-e49a152d6156 |
| `node tools/phase4/check.cjs --case source.tracer` | ff26de07-fd00-4da7-aff0-0858cdb97988 |
| `node tools/phase4/check.cjs --case source.tracer-failures` | f8bc1468-6637-4f50-a1a5-e1593bd6f945 |
| `node tools/phase4/check.cjs --case source.provenance` | 20b6919a-d68c-4108-af28-07d15c839f83 |
| `node tools/phase4/check.cjs --case source.publication` | 9059c6e4-d4d8-4986-8ef2-d3fe56154729 |
| `node tools/phase4/check.cjs --list` | Exit zero; exact eight-case inventory, also saved in the boundary invocation's ordered-cases.json |
| Source profile and helper TAP self-tests | Both 1/1 pass; original RED records retained separately |
| `git diff --check` | Pass |

The post-Task 1 tracer feedback gate passed in f72153c2-15b7-449f-a95c-39bab9d0c5c3 before Task 2 expansion. Earlier positive observations 007524e4-13bd-442b-9d01-f83dccd60f44 and 3848e7b8-5386-40de-b807-602aa31f9c09 are retained as earlier source identities, not substituted for final fingerprints.

Actual pinned engine: `4.7.2.stable.mono.official.ed1daf0bf`, executable SHA256 `2445d009a5e0474fc7064b9767100e9d2c09521890ac5625476bfb81cf03f2d4`. The final positive probe SHA256 is `bf21217b73e4955a56b9d1e641d799042b5b534f96ac266add76408af6be631f`; each nonce changes its probe bytes, and every variant retains its own hash. Actual base, actual-feature, empty-feature, ordinary-feature and uppercase-input-feature lookups return Foundation. The canonical explicit feature returns Inspection; `OS.has_feature(asset_inspection)` is false. The scene loads with 42 visible bow pixels. These are source/configuration/resource observations only.

The boundary case saves 61 independently damaged copies with exact expected/observed diagnostics and per-trial mutation hashes; no variant reuses another subject's pass. It also observes three usable native variants:

| Native variant | Nonce | Actual getter | Canonical explicit getter | Active tag |
| --- | --- | --- | --- | --- |
| Missing override | 83cb07e3-2e0e-41b4-bf53-acd06c8a3960 | Foundation | Foundation | false |
| Uppercase authored suffix | 0b410233-f8f9-41c3-b7c2-889d16aba711 | Foundation | Inspection | false |
| Authored global feature activation | d13d3a54-0d39-4602-8277-0c83e62c46f9 | Inspection | Inspection | true |

Every one of those configurations is rejected by the source validator. The native rows expose why that validator is needed; they supply no acceptance for bad sources. A separately restored/fresh valid subject completes the boundary suite.

The ten helper negatives cover timeout0/-1/1.5/60001, cwd outside the invocation, invalid env type, actual exit7 with separate stdout/stderr, actual ENOENT, actual deadline termination with a spawned descendant confirmed dead, and actual output-limit termination. Default cwd/local isolated environment and explicit owned cwd/environment both succeed. Existing interruption/cleanup semantics also pass through source.tracer-failures. The helper retains shell:false, hidden Windows children, stream closure, output ceilings, descendant cleanup and 60-second maximum; command evidence gains cwd, environment key names without values, and timeout. The committed audio task ac1de1e and its historical results were preserved without rerunning extraction or listening. The unchanged export.layout compiler case was not selected for this source-profile run; no managed/project/analyzer input changed. Required tracer and affected source helper regressions ran freshly.

## Artifacts This Phase Produces

| File | Actual new or adopted responsibility |
| --- | --- |
| src/godot-game/project.godot | Permanent `application/run/main_scene.asset_inspection`; ordinary main and remaining configuration unchanged |
| src/godot-game/export_presets.cfg | Transferred single existing preset; only `custom_features="asset_inspection"` changes relative to checkpoint |
| production/phase4/export-profile.json | Versioned documented source contract and explicit pending native consumer |
| tools/phase4/cases/export-profile.cjs | Positive and independent boundary procedures, generated owned probe/import fixtures and TAP tests |
| tools/phase4/check.cjs | Adopt optional explicit env/cwd from checkpoint, register only profile module, enforce deadline/owned cwd/typed env and retain bounded child evidence |

The profile's actual top-level fields are `schemaVersion`, `feature`, `baseKey`, `overrideKey`, `baseScene`, `inspectionScene`, `preset`, `scope`, `nativeEntryComplete`, `sourceContract`, `resources`, `requiredPreset`, `semantics`, `proofContract`, `documentation`. `sourceContract` contains `ordinaryProjectSha256`, `presetBeforeFeatureSha256`, `projectGraphSha256`, `checkpoint`, `fullRunnerBeforeProfileSha256`. `requiredPreset` names platform, export_filter, include_filter, custom_template/debug, custom_template/release, binary_format/architecture, binary_format/embed_pck, dotnet/include_scripts_content, dotnet/include_debug_symbols and dotnet/embed_build_outputs. `semantics` contains ordinary/explicit/actual/resourceProof/consumer. `proofContract` contains positiveCase/boundaryCase/runGroup/orders/nativeQueries/controls/evidence/nativeEntry.

Module constants: ROOT, PROFILE, PROJECT, PRESET, FEATURE, BASE, QUALIFIED, FOUNDATION, INSPECTION, SOURCE_ONLY, CONTRACT, RESOURCES, PRESET_OPTIONS, PROJECT_FILES and definition. Actual local/exported functions: need, read, write, ini, validate, snapshot, probeScript, observe, profileCase, trialContext, copySubject, mutate, registry, helperControls, boundaries, selfTest, helperSelfTest. Exports are validate/snapshot/probeScript/observe/profileCase/trialContext/copySubject/mutate/registry/helperControls/boundaries/selfTest/helperSelfTest. Generated probe members are reject/_initialize/inspect; its observation fields are nonce/scope/base/qualified/actual/empty/ordinary/inspection/wrongCase/active/foundationExists. Native probe evidence records inputs/probeSha256/engineSha256/version/scope. The adopted command options are env/cwd; new guard locals are childCwd/base/resolved/childEnv and new command-record fields are cwd/environmentKeys/timeout. Trial mutation fields are file/before/after; records retain nonce/status/expected/observed/mutation. Aggregates retain sourceRejected/outcomes/native/helper/restored/orderedCases/scope/nativeEntryComplete. Helper observations contain cwd/home/temp/packages/marker and outcomes contain name/expected/observed.

No asset is created. Consumed production resources are foundation/Foundation.tscn, inspection/Inspection.tscn, assets/common/bow.png, content/pack-index.json, content/packs/tracer.json, icon.svg and docs/README.md, all beneath src/godot-game. All five .csproj files, tools/phase1/projects.json and toolchain.json enter snapshots without modification.

Saved cases are `export.profile` and `export.profile-boundaries`, group export, Critical with rationale, ASSET-05 support, no acceptance ID, needsGameWindow:false, `G0-export-profile` orders350/351. Existing subsets remain source.tracer/source.tracer-failures/source.provenance/source.publication/audio.feasibility/export.layout with their original G0 orders1/2/3/4/6/0. Registry validation allows later legitimate cases while enforcing unique IDs and (runGroup,order); the missing unfinished exporter loader is a current ownership fact, not a permanent prohibition.

Owned fixture identities are profile-project/profile-importer/probe.gd; trials/name/subject; the analogous per-native-variant project/importer; fresh-valid-control/restored-project/restored-importer; explicit-cwd/explicit-env; and fresh helper-red-* self-test directories. Every fixture stays beneath its ignored invocation/TDD directory. Native code never edits production configuration. The 61 source trial names are missing-profile, wrong-profile-feature, missing-profile-feature, duplicate-profile-feature, wrong-profile-scope, wrong-profile-input, missing-feature, wrong-feature, feature-case, duplicate-feature, conflicting-feature, wrong-preset-name, duplicate-preset-name, missing-preset, duplicate-preset-key, preset-section-case, missing-override, setting-case, setting-key-case, duplicate-setting, conflicting-setting-case, base-main, missing-base, inspection-path-case, absolute-scene-path, escaped-scene-path, aliased-scene-path, global-feature, ordinary-config-feature, autoload-shortcut, ordinary-feature-override, missing-inspection, missing-foundation, missing-bow, missing-json, missing-doc, resource-case, resource-escape, resource-absolute, resource-alias, profile-path-escape, profile-path-absolute, profile-path-case, profile-options; missing-option-{include_filter,export_filter,custom_template-debug,custom_template-release,binary_format-architecture,binary_format-embed_pck,dotnet-include_scripts_content,dotnet-include_debug_symbols,dotnet-embed_build_outputs}; missing-filter-{json,png,wav,ogg,ttf,otf,md}; runtime-override. Exact expansion, hashes and diagnostics are saved in source-controls.json and each trial's mutation/result records.

## Source Identities and Preserved Partial Work

| Current committed input | SHA256 |
| --- | --- |
| project.godot | f5378d9a802aee409cd71ddda626ea78be62e431ffbfbac8f8447d69082b09d0 |
| export_presets.cfg | 97bef0a40ca36b6bf564fce9195e875181ca2acfc6cd15753ecd52d90400939e |
| export-profile.json | 570a3ecdd8171ee8b06a52a34231068ee2f8531fb0d402953326ddccacd03029 |
| cases/export-profile.cjs | 1fc611884058d2b4849c836ee6d18ebc2a9b56cd34652fb7f90cd91f49d47f0d |
| check.cjs | 0b0e67e27bf5fe6bb0ae18e57a266ffd07c11271d997b5b73120fecc4985858e |
| projects.json | 717bc7e8898553fe62a632fef80b0bbdc10425ecdc18668f8a39cb5be30a443d |

The immutable `.phase4/checkpoints/04-02-entry-93386745/manifest.json` SHA256 is `791db92afec21652cac33037d6290d1dc0042aa1f61b250af82a32c7d7c33bd2`. Before the first source write, every current partial file and its complete saved copy matched its manifest row. The full saved runner is preserved at SHA256 `2e022d88b462d361bc1807fc54ed031245e93f167edd6c5c7b07837a49d0a021`; its old preset is `7fe9294d4a160582af981a9ec00167f8bf8bb407b6e86ce8b4bbde226bed3b00`. Only the transferred preset and authorized helper/profile runner edits change current bytes. The unfinished `require('./cases/export-feasibility.cjs')` is held out of this plan's committed dependency tree and remains exactly recoverable in the checkpoint for resumed 04-02.

| Protected unfinished 04-02 file | Before and after SHA256 |
| --- | --- |
| production/phase4/export-toolchain.json | 9de0808747f124e7d9748b64f83efe37fc26c3f3e2c4f147262018e5b3fbe469 |
| tools/phase4/export.cjs | 54be82cc09b16b8d92d7f8b25514962c3307e0997a63b6ef0debeebcfbf07a31 |
| tools/phase4/cases/export-feasibility.cjs | dbca749152dd41bacb245fe76c1fceb6c1e167173871c28744aec85a779b2c73 |

Those three files remain intentionally untracked parent-owned partial source and were neither edited nor staged. Completed 04-01/25/34 summaries, source/audio artifacts, audio commit ac1de1e and original failed native entry run93386745-8daf-4965-a300-4ddf4d46c87e remain unchanged. The latter still fails INSPECTION_ENTRY_PENDING. No checkpoint copy or historical run was overwritten.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking fixture] Isolated media import from the exact production settings subject.** Task1 attempts b433e8f5-7b09-4615-8268-882f0a9b980a and 81397c15-b327-4a89-b912-d599c52ffc54 retain actual importer failures: the small fixture lacked FoundationRoot.cs and then the managed editor watcher emitted a timer error even in recovery mode. A separate asset-only owned importer resolved this without rewriting the production/main settings copy or introducing a production script. The final settings/resource process remains genuine pinned-engine observation. Files: cases/export-profile.cjs; commit5986b76.

**2. [Rule 1 - Diagnostic bug] Included the originating Inspection scene in dependency-path rejection.** Task2 run b958026f-9912-429e-98e2-8373c6f43398 correctly rejected a case-mismatched dependency but lacked its referring scene in the diagnostic. The validator now retains the exact bad resource path and referring scene. File: cases/export-profile.cjs; commitdeddfa1.

**3. [Rule 1 - Evidence-reader bug] Bounded the deliberate output-limit record reader separately.** Task2 run6dfdc6a3-4c25-4a86-adee-7ee0b411ff9f completed source/native controls but rejected the >1MiB output-limit command record under the ordinary 1MiB document limit. The helper control now uses an explicit 2MiB maximum reader for that real record; production child output ceilings remain unchanged. File: cases/export-profile.cjs; commitdeddfa1.

All failed attempts remain failed, with immutable logs and current input hashes. No required verify was skipped, no authentication gate occurred and no unresolved implementation stub remains in this plan's source-only scope. The explicitly pending native consumer is an existing 04-02 obligation, not a fabricated passing result. No additional threat surface outside planned T-04-103/104/105/SC was introduced.

## Next Phase Readiness

Parent can resume preserved 04-02 Task1 using this committed permanent profile, restore/register its incomplete export loader when appropriate, and obtain a fresh actual packaged Inspection startup with current binary/PCK/runtime/source identities under natural startup. Foundation's earlier eight frames, separate editor-mounted pack and this settings/resource probe cannot supply that result. Do not redo committed audio Task2. Listening still has no capable named listener/verdict, and remains outside this source task. Branch remains feature/04-asset-production; parent owns existing draft PR #5 targeting dev and continuation/checkpoint updates.

## Self-Check: PASSED

All five implementation files exist; all four listed RED/GREEN commits are ancestors of HEAD. Required evaluation-scope resolves those four commits plus the prior planning commit, rather than treating sandbox-blocked child git as an empty scope. The persisted ledger measures exactly four implementation/test commits from97f6a86 to deddfa1. Only the five authorized implementation files differ across that interval, with no deletions. All automated plan verification commands and the affected source helper regressions passed. All five project declarations, production resources, protected partial files, checkpoint bytes and completed source/audio work remain preserved as stated. No ASSET requirement is completed by this prerequisite summary.
