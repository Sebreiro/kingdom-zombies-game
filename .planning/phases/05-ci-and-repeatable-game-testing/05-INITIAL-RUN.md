# Phase 5 initial and repair evidence

The first reduced run on Windows x64 used pinned Node 24.22.1, .NET 10.0.404, and Godot 4.7.2. It tested `cbf306492d61d0d705dfd274746e889e3c331f13` at 1280×720.

- Command. `node src/godot-game/tools/game-tests/check.cjs --phase-end --review .local/temp/phase05-rendered-review.json`
- Shared invocation. `78becb29-763d-4cf8-93cc-a9abff498b79`. Exit 1, 30 passed, 17 failed, 20 blocked out of 67 catalog rows, 60m 52s.
- All four basic checks passed. Windows local pure tests took 1.370s, headless solo took 1.049s, and preparation with a fresh strict build took 15.907s. Cached hosted jobs for the same commit passed in 2m 41s on Windows and 1m 57s on Ubuntu. [Hosted run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45280820850)
- Phase 1 rules ran 451 checks, with 429 passes. Runner and register independently passed 36/36 and 46/46. Tools and pins passed 25/27. The rules failures include mixed checkout line endings, missing historical export fixture bytes, outdated relocated paths and selectors, and Windows output collisions. The original nested results remain under `.local/runs/verification/`.
- An inherited proof build removed the active Debug game assembly. Solo could open the window but could not instantiate FoundationRoot. LAN game processes did not produce a ready scene, so the rendered multiplayer cases stayed blocked. The inspection hub stopped before opening a window on source-byte drift. The original rendered groups receive no pass credit.
- The relocated source tracer, its negative controls, audio actions and bank, world effects, inspection, and world replacement passed. Audio lifecycle and held-out cases stopped at the relocated hub copy after their controller assertions passed. World-map source replay stopped when its retained recipe tried to load the old extractor path.

The repair uses LF checkout bytes, isolates generated proof builds, regenerates the six export inputs from the pinned SDK, and repairs known relocated paths. A fresh Debug build is required and checked again after any suite that changes runtime files. The Windows ExportRelease probe excludes test-only input and fault injection while retaining the read-only observer. Plain Release encountered the existing SDK-generated line-length rule. An isolated retry with only that SDK attribute disabled validated its assembly boundary.

Reproduce the repair and then the full reduced run using [the shared game test commands](../../../docs/workflow/shared-game-tests.md). Supply a clean review for the exact repaired commit. The original report stays intact. This record does not close Phase 5.
