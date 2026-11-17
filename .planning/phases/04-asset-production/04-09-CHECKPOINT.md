# Plan 04-09 — Task 1 correction-budget checkpoint

Status: incomplete; zero of two tasks complete. Original Task 1 correction budget is exhausted at three of three. No Task 1 GREEN commit, committed-content tracer, Task 2 implementation, SUMMARY, global progress, ASSET completion or actual acceptance is claimed.

## Current failure and immutable evidence

Canonical invocation `f4e4172c-e764-435a-b9d2-e8f37792c012` in `.phase4/runs/` passes the current strict solution and native harness builds with zero warnings and zero errors. Authenticated official nonconsole Godot 4.7.2 native process PID37728 exits1 during the real production `CharacterPreview.Open`, before the bow matrix or compiled GREEN tests execute:

```text
System.FormatException: path requires exact src/godot-game/ prefix: content/config/items.json
ContentPack.ResourcePath -> CharacterPreview.ReadPresentation:263 -> CharacterPreview.Open
```

The new bow bindings use project-relative paths, while the existing canonical ResourcePath contract requires the exact repository prefix. Existing locomotion bindings and their precise traversal exception contract are unchanged. The proposed bounded next repair is to correct the new bow binding paths and their exact expected set, without weakening ResourcePath or accepting a different exception. No further source correction was performed after this failure.

The tested source is HEAD `0f499f9d71bafeac936e4273313e6f9f45e1bbb4` plus the exact seven WIP files backed up at `.phase4/checkpoints/04-09-f4e4172c-wip/manifest.json`. That manifest records raw SHA256, byte length and Git-filtered blob identity per file. Copies retain the original path beneath that directory. Raw checkout and filtered Git identities are distinct; no later commit is represented as tested.

## TDD history

- `7f9ad2a`: historical semantically invalid RED. Explicit Melee with a selected bow correctly uses the .7-second fist fallback; expecting1.2 there was the wrong action. Its original unchanged JUnit and SDK result remain in `.phase4/04-09-red/`, with the semantic rejection recorded in the approved amendment and corrected evidence. The current test preserves the correct explicit Melee expectation; it is neither wrong nor skipped.
- `651c553`: accepted genuine native RED. Invocation `cdb525f6-8bf7-49de-b456-e22fcce9ea1d` has strict solution/harness0/0, authenticated actual native PID12776/exit0, all704 original melee samples and five source negative controls. Actual selected IT-07 left, IT-02 right and Usable condition expose Action options None/Melee. `Native_UsableBowOffersBowAction` executes and fails `offered=false` versus true; the independent explicit Melee fallback passes. Exact qualified JUnit target receives `RED_EVIDENCE_OK`; semantic assessment is valid. No production bow behavior preceded this RED.
- Initial RED setup failures (unused using; later an IDE0007 declaration) have no behavior credit. The short JUnit target classifier rejection is preserved and was corrected to the exact emitted fully qualified target without altering the original report.

## Original Task 1 correction ledger

1. Initial GREEN strict build: three authored Core formatting diagnostics. Correction1 fixes indentation/comment lengths; the next build reports three named-argument errors (`Math.Cos` and Godot `Image.GetRegion`).
2. Correction2 uses actual pinned GodotSharp XML and .NET reference parameter definitions (`region`, `d`); strict solution build passes0/0.
3. After the complete new native matrix and continuous slider are authored, canonical `29940f01-22c2-46d8-89c2-acaf5d1d53aa` fails strict build with five diagnostics: Project64 lines, slider field CA1001, test line length and wrapping. Correction3 borrows the engine-owned slider through guarded root lookup, moves preset reconciliation into guarded UpdateAim, and fixes test formatting. Canonical `f4e4172c` passes both strict builds but fails the distinct new binding prefix at native Open.

Budget remains3/3 exhausted. No node-repair decomposition or reset is assumed. This checkpoint requires a prospective coordinator repair scope or other explicit continuation decision.

## Preserved implementation and remaining proof

Exactly seven WIP paths are preserved:

| Path | Current responsibility |
| --- | --- |
| `src/core/CharacterPresentation.cs` | Existing sealed identity; additive four-definition constructor, ConfigureBow and TryRelease; detached aim/ammunition/stack/release/fallback fields; original constructor/Configure/Melee compatibility retained |
| `src/godot-game/inspection/CharacterPreview.cs` | Required bow loading/binding validation; Bow action, count/stack/preset and continuous elevation controls; normal torso/head with mechanically cropped actual aim-arm art; fitted gear; spent-arrow visual; explicit retained K15 guard mode |
| `src/godot-game/content/animation/bow.json` | AN07/08 timing/events, source bindings and exact K15 source clip783/frame rectangles/times; current canonical-path error remains unfixed |
| `tests/phase1/CharacterBowTests.cs` | Actual Bow availability; original explicit Melee fallback; zero/one/many ammunition; one-shot release, detached samples and immediate replacement; expected native matrix/boundaries |
| `tools/phase4/cases/bow-work.cjs` | Actual source-bound animation.bow case, source guard authentication, authored448 bow aim/release rows and14 boundary outcomes; these expanded outcomes have not executed successfully |
| `tools/phase4/cases/animation.cjs` | Approved resource staging/fingerprint closure only: bow definition and actual common pack/atlas |
| `tools/phase4/cases/bodies-a.cjs` | Approved FIT_FILES staging/fingerprint closure only: same required resources |

The prospective source/native proof still must establish every new actual grip, source crop, normal head/torso, lower stride, continuous angle, both logical hands/directions, release recovery, zero/one/many/replacement/broken/empty/fallback boundary, actual retained structural guard and no pickup claim. Existing full704/four-jump/five-negative/restored-positive,41 replacement/control and seven UI-state regressions remain required and unchanged. No expanded native pass is inferred from compilation. Further strict definition/event/finite-boundary and generation controls, detached sample assertions, exact native tuple observation and current producer impact need verification before Task1 completion. Guard currently samples retained source frames; required continuous guard aiming must be assessed, not silently inferred.

Task2 has not started. No work-special JSON/PNG/ZIP exists from this execution, and no raster generation was performed. Imagegen and kingdom-zombies-sprites skills were read in preparation; D13 remains authoritative over the older richer-style default. The mandatory fresh same-case committed-byte Task1 tracer must pass before any Task2 expansion.

## Scope and continuation constraints

Prospective amendments `f41ea36` and `0f499f9` authorize the two existing consumer staging/fingerprint lists and the existing Core API only. Plan union10 and phase inventory236 remain unchanged. No Root, generated/native permission, new public identity, new Godot script, tool installation, capacity/deadline or common-media edit was made. Native access remains observation for23; no policy cohort or authorization was granted.

Branch remains `feature/04-asset-production`, existing draft PR5 targeting dev. No branch switch, merge or push. Foreign PROJECT/REQUIREMENTS/ROADMAP/STATE/spec/Phase6/Phase12 edits remain unstaged. All WINDOWS29/30/31, full policy/pack integration, actual Windows1280x720 appearance, physical input, listening, independent judgment, all ASSET and32/33 acceptance remain pending under their original owners.

## Self-check

The seven backup copies and current source hashes were compared at checkpoint creation. Both RED commits are reachable from HEAD. No task was committed complete and no SUMMARY was created.
