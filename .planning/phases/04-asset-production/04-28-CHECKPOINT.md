# 04-28 Task1 authored native harness checkpoint

## Latest checkpoint — R2 semantic row lookup resolved; imported transparent RGB differs

Recorded2026-11-17 on `ab72e50010d903af77ed3c5dfca20bd4c6817cfd` plus the exact seven WIP files below. Both tasks remain incomplete0/2. Original Task1 corrections3/3 remain exhausted; R1 used1/2 and resolved its ordinal comparisons; R2 used2/2 and resolved semantic direct-row lookup. A newly exposed pixel defect stops execution under the explicit R2 boundary. No Task1 source commit, committed tracer, Task2 expansion or SUMMARY exists.

Before R2 editing, all seven live files and R1 backups matched the recorded hashes. Only `IDENTITY_HARNESS` in `tools/phase4/cases/ui-kit.cjs` changed. `Row` resolves exactly one direct Control child in the named section by ordinal `resource_id` metadata. Missing and duplicate IDs throw explicit fixture exceptions. Label, binding keycap, player shape, canonical item texture and neighbor accesses use that row; no display-label, sanitized-name, descendant, substring or fallback lookup exists. Four added native controls prove exact ID, ordinal case rejection, duplicate rejection and restored positive identity. All original Check IDs, expectations and predicates remain, including all24 full-RGBA assertions and R1 ordinal edits. The other six implementation files are byte-identical.

R2 attempt1 invocation `1be6e297-a6df-4834-8120-dca229486ef8` passed source replay and strict production0warnings/0errors(11.80s), then failed its newly authored helper at harnessline22 with CS1674: `Godot.Collections.Array<Node>` cannot be used in a `using` statement. Harness0warnings/1error(2.97s); native was not launched. Attempt2 replaced only that enumeration with `GetChildCount`/`GetChild(index)`, consistent with pinned GodotSharp4.7.2 XMLline5316. It creates no owned child-array temporary and never disposes borrowed rows/textures. Existing `GetImage` copies retain their `using` scopes.

### Actual second-attempt result

- Canonical `node tools/phase4/check.cjs --case layouts.identity` invocation `c3eab850-4b70-4bdf-8156-fa7dd318e0cf` is FAIL, started07:06:39.034Z and finished07:06:51.228Z(12.194s). Production strict0warnings/0errors(1.29s), harness strict0warnings/0errors(3.20s).
- Source replay passes27 ZIP members, original master/expanded prompt, exact atlas pixels, seven ICO frames and distinct same-named sources. No source/media regeneration occurred.
- Authenticated official non-console native childPID23056, nonce and production closure match; actual DLL SHA256 `524fce3d172321f3868defbdfb3e5759e1bfe844f3dc6eaa49913d7fd8b910c8`. Native exit1, stderr empty, completion marker FAIL.
- All117 outcomes were reached:93pass/24fail. All original113 outcomes remain, plus four new fixture boundary controls. The four new controls pass. Title/theme/cursor geometry, four counts, four default bindings,18 statuses, four players with shape/color/number/name/state,24 exact atlas/path regions,24 numeric/null condition rows and every duplicate-property/thread/replacement/close/neighbor/detached check pass.
- Exactly `canonical-pixels-IT-01` through `canonical-pixels-IT-24` fail. ExampleIT-01 expected `5e214d68b3f4798bc2d8f5a2a28b478b60356711d838863436f63d5bf24bf3f5`, native `8312c58969e7da5fa42a84d0f1c0990684d03a9d31c20856675f734e068f9248`. These remain assertion failures; the case/task has no pass credit.
- Sandbox Git spawningEPERM preceded equivalent escalated execution and supplies no assertion credit. Historical R1/native/production failures remain immutable.

### Concrete read-only pixel diagnosis

Offline decoding compared each original production PNG crop, its expected manifest RGBA hash, and this exact run's actual imported `.ctex` embedded lossless WebP crop(offset56). For all24 items, expected equals original PNG crop and native observed equals actual imported CTEX crop. Every changed pixel has alpha0 in both images; no changed pixel has positive alpha in either. The saved native `.png.import` files have `process/fix_alpha_border=true`. Thus the fixture compares original source full-RGBA bytes against an importer-processed texture copy whose transparent RGB differs. Region, source path, visible RGB and alpha agree in the observed artifact comparison; no rendering or visual acceptance is inferred. No PNG/JSON/projector/scene/import file was edited, no new native run was launched for this diagnosis and no asserted hash was replaced.

Exact24-row diagnostic: `.phase4/checkpoints/04-28-harness-row/r2-attempt-2/pixel-diagnosis.json`. It records expected/raw/imported/native hashes, crop dimensions and changed/changed-opaque pixel counts. Both attempt manifests, all seven numbered backups, exact before/after boundary maps and emitted harnesses remain under `.phase4/checkpoints/04-28-harness-row/r2-attempt-{1,2}/`. The second emitted harness matches the retained harness exactly; all seven live/backup hashes match after native execution. Canonical run logs and native results remain in their original invocation directories; `identity-native-observation.json` is intentionally absent because the failing child exit prevented that success-only publication.

### Exact latest WIP

| Path | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/assets/ui/identity-symbols.png |2616| d6d263f08544eaa2d53e9e3e93caa5ec1d96b1a297cc8b87c195e897c9d2bc48 |
| src/godot-game/assets/ui/kingdom-zombies.ico |13342| 5ca0170350b621993b7ed6c8d2e501360e677371b3f04df798092e2003b01c21 |
| src/godot-game/content/packs/ui-identity.json |27971| 324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed |
| production/phase4/ui-identity.zip |558233| 718f34b2f8a3b2803b94a9bf6590faa768fb18e3a0b515d43edb5dcd0d92d315 |
| tools/phase4/cases/ui-kit.cjs |80506| 1a1c86f93dbdbb6bcf35958afef90f1f70f4d87774183b93b0540e3a64e660ba |
| src/godot-game/inspection/IdentityKit.cs |19385| 03d3e01a3eedb4251b74f413e639f5cd9d0540c9c097400aae1b1aa7c83898a2 |
| src/godot-game/inspection/IdentityKit.tscn |5093| 9d73d6e1caffe0552cf80492ab55bbd39b7358270725d03975af0e059a392201 |

### Prospective next boundary — not authorized by R2

The smallest next decision is a separately bounded proof-only treatment of source-RGBA versus imported-texture semantics inside `IDENTITY_HARNESS`, preserving exact original raw source hashes, canonical24 identities/regions/paths, all alpha values and all visible RGB. Any proposal must independently reject altered opaque RGB, altered alpha and wrong canonical source/region, retain existing original coverage with explicit mapping and demonstrate restored positives using the actual production adapter. Do not merely replace expected hashes with observed imports, suppress24 assertions, introduce tolerance or change global/production importer/media policy. A source-image decoding proof plus an independently checked imported-texture contract is a candidate requiring its own precise coordinator amendment; no repair or predicted pass is claimed here.

After any separately approved pixel repair, require fresh clean canonicalidentity and real exit0, then bounded Task1 commits and a fresh same-case committed-byte tracer before Task2. Task2's original budget and four-path scope remain unconsumed; its independent remap/identity/condition/icon mutations and fresh affected/full selection remain required. Current16/37 progress, inventory236, full-hub23, icon application31, WINDOWS29/30/31, allASSET and final32/33 appearance/listening/physical-input obligations are unchanged. Foreign global/spec/Phase06/Phase12 WIP stays untouched and unstaged; parent owns progress and PR5.

## Latest checkpoint — R1 ordinal repair resolved; native ID/path defect

Recorded2026-11-17 after canonical invocation `3d42282e-90fb-4bd4-aabc-d7addda792e3` on `c2e249d65f30870d93b3488cada383c85bfdb99b` plus the seven exact WIP files. Both tasks remain incomplete0/2; no task commit, committed tracer or Task2 expansion occurred. Original Task1 corrections remain3/3 exhausted. Prospective ordinal R1 used1/2 and resolved its14 compiler diagnostics; its unused attempt does not authorize fixing the distinct native defect.

All original seven live files and numbered backups were authenticated before editing. Exactly14 diagnosed string equality operators in `IDENTITY_HARNESS` now call local nullable-string `Eq`, implemented by `StringComparer.Ordinal.Equals`. The original empty-string Broken comparison and every nonstring operator/predicate/expected value remain unchanged. Reversing the14 replacements and removing the helper reproduces the original checkpointed case bytes exactly. The actual emitted harness matches the retained repaired harness byte-for-byte. No source/media/JSON/scene/projector change occurred.

Exact map: `.phase4/checkpoints/04-28-harness-ordinal/r1-attempt-1-map.json`. Repaired emitted proof: adjacent `r1-attempt-1-IdentityHarness.cs`. Current seven-file backups and manifest: `.phase4/checkpoints/04-28-harness-ordinal/r1-attempt-1/manifest.json`. Preserve the original manifest/backups and original failed `ae702b6a` invocation independently. The edited case is78972bytes with SHA256 `f95473b5f7079264395d949340d1f5ffa592132f4e45b7598511b33ff5e3467b`. All other six hashes remain exactly those in the original table below.

### Actual verification and failure

- Source replay passes27members, original1254×1254 master, expanded prompt, sampled-RGB alpha/crop/nearest atlas replay,7ICOframes and distinct same-named sources.
- Strict production build passes0warnings/0errors in11.42s; strict native harness build passes0warnings/0errors in3.14s. No suppressions or capacity changes.
- Pinned non-console native childPID27860 runs the real production adapter. PID/nonce/production-DLL closure match; DLL SHA256 `1d2b06efd3ab4ca86ec72897fd49fd6141d4db5c39cdcd9768ab4e7e726b9eee`.
- Native exit1. Twelve outcomes pass: owningALC/type boundary, title, theme, cursor geometry, four section counts and four default bindings. The thirteenth outcome is `fixture-exception`, a `NullReferenceException` after `GetNode` cannot resolve `Scroll/Content/Statuses/status.health/Identity` at emitted harnessline36 through `Label`line22. These partial observations are not a case/task pass.
- Actual logs and results live in `.phase4/runs/3d42282e-90fb-4bd4-aabc-d7addda792e3/`: `result.json`, both strict build logs, `identity-native.command.json`, stdout/stderr and `native-identity/identity-results.json`. Elapsed canonical window06:57:40.048Z–06:58:02.291Z=22.243s. Sandbox Git spawnEPERM preceded the equivalent escalated run and supplies no assertion credit.

### Concrete diagnosis and smallest proposed next scope

Production `IdentityKit.Row`line295 assigns exact stable ID to `Node.Name` and line297 independently retains exact `resource_id` metadata. Status IDs contain periods, beginning `status.health`; the proof `Label` helper concatenates raw ID into a NodePath. Pinned local GodotSharp4.7.2 documentation at `GodotSharp.xml`line5063 explicitly says Node.Name replaces period/colon/at-sign/slash/quote/percent with underscore. Consequently a raw dotted resource ID is not a valid native row-path identity. The native error confirms that raw path lookup fails; the failed fixture did not capture the sanitized row name, so no runtime inspection of that exact resulting name is claimed.

Proposed separately bounded proof repair: resolve a direct row by ordinal exact `resource_id` metadata within its named section, require exactly one match, and then resolve its child control. Preserve production stable IDs and metadata; do not introduce lossy punctuation normalization or a second production implementation. Independently assert all18status metadata IDs and native labels, with same-label/different-ID/reordering controls during Task2. No production/API/media/scene change appears necessary for this observed fixture mismatch. This is a proposal only; no such edit or further run occurred under ordinal-only R1.

After any separately approved repair, rerun unchanged canonical `layouts.identity`, require all actual native outcomes and clean exit, commit Task1 in approved bounded parts, and run a fresh same-case committed-byte tracer before Task2. Still-unreached item pixels/condition/players, replacement/close/neighbor/thread/detached checks receive no pass. No final selected regression, native policy authorization, application-icon adoption, ASSET completion or actual appearance/input/listening credit. Foreign global/spec/Phase06/Phase12 edits remain untouched and unstaged; parent owns progress/inventory236/PR5.

## Original checkpoint retained

Status: blocked before Task1 source gate/commit. No task is complete, no committed tracer exists, and Task2 has not started. Current branch is `feature/04-asset-production`; base/current HEAD before checkpoint metadata is `b8d7666ab64bfcbcaf5b1317f4f45d2dd78b49f0`. The seven implementation files remain exact uncommitted WIP, preserved independently in `.phase4/checkpoints/04-28-harness-ordinal/manifest.json` and its seven numbered backups.

## Observed result and bounded correction history

Canonical `node tools/phase4/check.cjs --case layouts.identity` invocation `ae702b6a-0270-4ba6-9d3c-1f9710e8c7da` is FAIL. Source replay passed:27 exact ZIP members, original generated master, expanded prompt, replayed atlas pixels,7 portable ICO frames, and same-label/different-source-ID controls. Its strict production build passed0warnings/0errors. Its authored IdentityHarness build failed14MA0006 diagnostics for `==` string comparisons. The engine was never launched; there is no native behavior, lifecycle, image-copy or neighboring-instance pass.

All14 exact diagnostic locations in the immutable invocation-local `native-identity/IdentityHarness.cs`, as emitted in `identity-harness-build.stdout.log`:29:24,30:23,34:247,34:326,36:250,36:492,37:371,37:574,37:726,38:260,39:151,41:283,42:35,43:281. The repeated build-summary rows are the same14 diagnostics, not28 distinct defects.

Task1 correction budget3/3 is exhausted. Initial strict production diagnostics were21 authoring analyzer findings (access/member ordering, indentation, line length, explicit null validation and ordinal strings). Correction1 left6, correction2 left2, correction3 produced a clean strict solution build. The subsequent first canonical case exposed the distinct authored-harness issue above. No correction was applied after that failure and no retry attempted. Sandbox Git spawning failedEPERM before the canonical run; equivalent approved escalation produced the actual saved failure. Two direct sandbox managed builds reported failure with0warnings/0errors; equivalent isolated escalation exposed real diagnostics. No policy or analyzer was suppressed.

## Exact retained WIP

| Path | Bytes | SHA256 |
| --- | ---: | --- |
| src/godot-game/assets/ui/identity-symbols.png |2616| d6d263f08544eaa2d53e9e3e93caa5ec1d96b1a297cc8b87c195e897c9d2bc48 |
| src/godot-game/assets/ui/kingdom-zombies.ico |13342| 5ca0170350b621993b7ed6c8d2e501360e677371b3f04df798092e2003b01c21 |
| src/godot-game/content/packs/ui-identity.json |27971| 324f6c05f57112cbdb87a58f1c5975fcdfd7ba4d7cc1d6ea9c55e4280b9202ed |
| production/phase4/ui-identity.zip |558233| 718f34b2f8a3b2803b94a9bf6590faa768fb18e3a0b515d43edb5dcd0d92d315 |
| tools/phase4/cases/ui-kit.cjs |78848| 200609470e4915cd917e642bb995966fbd2d4c4520dab7e89a2a37b4ec155e59 |
| src/godot-game/inspection/IdentityKit.cs |19385| 03d3e01a3eedb4251b74f413e639f5cd9d0540c9c097400aae1b1aa7c83898a2 |
| src/godot-game/inspection/IdentityKit.tscn |5093| 9d73d6e1caffe0552cf80492ab55bbd39b7358270725d03975af0e059a392201 |

The27-member ZIP retains the1254×1254 original built-in imagegen `exec-5b4739c2-e69f-411d-8074-f0b971fd7ac5.png`, exact expanded prompt, alpha-threshold128/crop/centered32px nearest recipe preserving sampled RGB, seven PNG ICO frames16/24/32/48/64/128/256, atlas, source rows/raw bytes/PNGs for common.texture.3/common.s0.7592/common.s0.7989, source recipe and style record. Three `history/palette-normalization/` members preserve the earlier nonselected palette trial. Neither donor crown nor publisher artwork supplies game identity. The saved original alphaBBox is[0,55,1196,1206], rawRGBA494c755433f9382d9bdfc7f06575bbc115450341a32368d327b3ce9486e7a761. The actual256×64 atlas rawRGBA435131d1cff8582b61c8c07f3d3c7219c1e400b8574292d4413c591b6c714194 includes32px emblem/cursor and60×26 blank keycap frame. Essential labels remain native text.

The internal sealed production IdentityKit and scriptless native scene use the exact04-14 Theme/fonts. Snapshot parsing rejects duplicate/prototype keys, non-NFC/control text, excessive bytes/depth/nodes and nonfinite numbers. Production projection validates source-backed24item canonical icons and DT04 condition maxima,4binding IDs,18status IDs,4number/name/shape/color player identities, and24condition rows before replacing native rows. It has a captured-thread guard, detached/closed rejection and close fence. New AtlasTextures are explicitly owned; row children are synchronously removed/freed before owned wrappers are disposed. ResourceLoader base textures and scene/theme resources remain borrowed. These are authored implementation facts, not native execution proof.

## Smallest prospective continuation

Request a separately bounded proof-only R1 authorization, without resetting Task1's3/3 budget: edit only `IDENTITY_HARNESS` inside tools/phase4/cases/ui-kit.cjs, adding an ordinal Eq helper and replacing its14 diagnosed string comparisons. Preserve every predicate, expected value, source/media/JSON/scene/projector byte, source gate, PID/nonce/owning-ALC/DLL/archive authentication, cleanup, image-copy `using` scopes and independently live neighboring instance. No suppression, tolerance/deadline increase, package install, source-policy permission or API change.

After authorization, rerun the exact canonical source case. Any native failure is a newly observed blocker requiring its own bounded decision; the current failure supplies no predicted pass. Task1 must pass, receive its scoped commit and then a mandatory fresh same-case committed-byte tracer before any Task2 expansion. Task2 must still add independent equal-label/different-ID, reordered/remapped/modifier/mouse/unbound/conflict/reset, all4identity cues/states, zero/usable/broken/null-durability and icon/export contract mutations against actual production projection, then affected/fresh selected regressions. Task2 source changes may use only the approved JSON/case/scene/projector paths.

Current project icon is still `res://icon.svg`; no export/application identity claim is made. Full-hub dispatch remains04-23; final actual desktop embedding04-31 and appearance/prohibition/physical-input judgments04-32/33. WINDOWS29/30/31 and all ASSET requirements remain pending. Current new native operations and generated producer impacts still need authentic observation and exact signature handoff to04-23; they are not granted permission here. Required14kit/theme and original hub regressions were not run against this incomplete slice and receive no new pass/skip credit. Foreign planning/specification/Phase06/Phase12 work remains untouched and unstaged. Parent owns global progress, continuation, inventory236 and PR5. No terminal SUMMARY or state advancement is valid yet.
