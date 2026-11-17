# 04-28 Task1 authored native harness checkpoint

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
