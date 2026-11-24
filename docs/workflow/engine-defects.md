# Known Godot engine defects

This page lists confirmed defects in the pinned Godot engine that affect this project. For each one it gives the symptom, the cause, the workaround in this repository and what must be true before the workaround can be removed. Read it before you change how a harness starts Godot, change import settings or upgrade the engine.

Add an entry only when engine source or traced evidence confirms the defect. Keep the full investigation in its `.planning` debug record and link that record from the entry.

| Defect | Engine | Workaround | Status |
| --- | --- | --- | --- |
| [Windows editor import working-directory race](#windows-editor-import-working-directory-race) | Godot 4.7.2 .NET, Windows | The Phase 4 harness forces single-threaded editor import | Worked around in the Phase 4 harness. Still present in the engine and in developer editor sessions. |
| [Item preview managed handle failure](#item-preview-managed-handle-failure) | Godot 4.7.2 .NET, Windows | The inspection hub retains the two managed PackedScene wrappers across resource selections | Workaround implemented in Phase 5 |

## Windows editor import working-directory race

**Symptom.** An editor import, such as a headless `--import`, sometimes prints blocks of lines like these on stderr and still exits 0:

```text
Unicode parsing error: Unpaired trail surrogate (4897 [lead?] df01)
Unicode parsing error: Unpaired surrogate (df01)
```

The values change from run to run, and most imports print nothing. A batch of 64 threaded imports of one failing fixture printed nothing at all. In Phase 4 the errors failed the clean-import checks of `animation.replacement`, `audio.heldout`, `world.inspector` and `text.support-native`.

**Affected.**

- The pinned engine in [toolchain.json](../../src/godot-game/tools/verification/toolchain.json): Godot 4.7.2 .NET, official Windows x64 build, source commit `ed1daf0bf001b61586d9930840f2f1394092c079`.
- Windows only. The defect is in `DirAccessWindows`. Linux was not checked.
- Imports that use worker threads. The project setting `editor/import/use_multiple_threads` defaults to `true`, and fonts import on worker threads because `ResourceImporterDynamicFont::can_import_threaded()` returns true.

**Cause.**

1. The `DirAccessWindows` constructor in `drivers/windows/dir_access_windows.cpp` reads the process working directory without a lock, in two calls. `GetCurrentDirectoryW(0)` returns the size, the buffer is resized without being initialized, and `GetCurrentDirectoryW(n)` fills it. The result of the second call is not checked.
2. `DirAccessWindows::change_dir()` changes the process working directory for a moment under `GLOBAL_LOCK_FUNCTION`. The constructor does not take that lock.
3. If an import worker thread makes the working directory longer between the two calls, the second call fails and writes nothing. The constructor then decodes the uninitialized heap bytes as UTF-16 in `String::utf16()`, which prints the first block of errors. Its own `change_dir(".")` converts the string twice more, which prints two more blocks.
4. In every failing run the garbage bytes were the tail of a `.fontdata` file that the same import had just written.

**Evidence.** A Frida trace recorded every `GetCurrentDirectoryW` and `SetCurrentDirectoryW` call by thread on the original failing animation fixture. It only observed calls and did not change timing, arguments or results. Each mode ran four times.

| Import mode | Threads touching the working directory | Failed `GetCurrentDirectoryW` fills per run |
| --- | --- | --- |
| Threaded (engine default) | 13 | 4, 4, 3, 0 |
| `use_multiple_threads=false` | 1 | 0, 0, 0, 0 |

Both modes made the same 6912 calls and 1142 size/fill pairs. The 44 imported files were byte-identical across threaded runs, single-threaded runs and the original failing run. Identical output does not make the errors harmless, because the affected `DirAccess` works from a corrupt directory string.

**Workaround.** In [src/godot-game/tools/asset-pipeline/check.cjs](../../src/godot-game/tools/asset-pipeline/check.cjs), `command()` starts every Phase 4 harness process. For each command that contains `--import`, it calls `editorImportPolicy()` before it creates the environment or spawns anything:

- `--path` must name one run-owned project below the run directory that contains `project.godot`. These are rejected with `EDITOR_IMPORT_PROJECT`: a missing, unowned or file-less `--path`, a `project.binary` (Godot would load it instead of `project.godot`), a hard-linked `project.godot`, `--main-pack` and a positional project file.
- It appends `[editor]` with `import/use_multiple_threads=false` as the last block of that `project.godot`. Godot keeps the last value it reads, so this block wins. A file that already ends with the block is left unchanged, so importing the same project twice changes nothing.
- It reads keys the way Godot's `VariantParser` does. Any assignment of the setting that is not `false` is rejected with `EDITOR_IMPORT_THREADED`, including feature-tag overrides such as `.windows` and `.editor`, which beat the base key.
- Commands without `--import` are not changed.

`override.cfg` cannot carry the setting because the editor ignores it. The engine binary stays official and unpatched.

Run `node src/godot-game/tools/asset-pipeline/check.cjs --self-test-editor-import` to test the policy. It uses a Node stand-in for Godot and does not need the engine. With the policy in place, all four affected cases passed with empty import stderr.

**Not covered.**

- Developer editor sessions on the game project still use threaded import and can still print these errors. Changing [project.godot](../../src/godot-game/game/project.godot) needs separate approval.
- Any other harness that starts a Godot import must apply the same setting itself. Today only the Phase 4 harness runs `--import`.
- The defect has not been reported to Godot.

**Removing the workaround.** Keep the policy until an engine upgrade changes the `DirAccessWindows` constructor so that it takes the working-directory lock or checks the result of `GetCurrentDirectoryW`. Confirm the change in the new engine's source. Clean runs alone cannot prove the fix, because the race is intermittent and 64 threaded imports in a row passed while the defect was present.

**Records.**

- Investigation and evidence: [debug record](../../.planning/debug/phase04-import-surrogates.md).
- Fix plan, review and verification: [quick task 261120-09v](../../.planning/quick/261120-09v-fix-intermittent-godot-editor-import-uni/261120-09v-SUMMARY.md).
- Raw traces and logs were local, in the ignored `.phase4/temp/animation-import-unicode/` folder, and are deleted by [workspace cleanup](cleanup.md). The results above and the debug record remain; the original trace cannot be replayed from the repository.

## Item preview managed handle failure

**Symptom.** The Phase 4 `world.replacement` run on `5c637da` emitted `InvalidOperationException: Handle is not initialized` from `ScriptManagerBridge.SwapGCHandleForType` while `ResourceLoader.Load<PackedScene>` loaded an item preview scene. The Windows runner recorded the diagnostic on stderr even though the native assertions passed. Five fresh reruns passed with empty stderr.

**Working diagnosis.** The exception appeared while Godot was reusing a native scene across item selections. The former local variable let its managed wrapper become eligible for finalization before the next selection. The stack trace is consistent with a race between finalization of the wrapper and reuse of the native scene. The order of those operations was not traced.

**Workaround.** [InspectionRoot](../../src/godot-game/game/inspection/InspectionRoot.cs) retains the weapons and wearables [scene wrappers](../../src/godot-game/game/inspection/WorldPreview.cs) for the lifetime of its inspection hub. Individual selections still authenticate the scene before instantiating it. Replacing or resetting a preview retires its child nodes while the hub keeps the two scene wrappers alive.

**Verification.** Run `node src/godot-game/tools/asset-pipeline/check.cjs --case world.replacement` with the pinned Windows toolchain. The existing case exercises rapid replacement, error and recovery through the native hub and requires clean stderr. Keep the workaround until engine source or a targeted trace establishes safe wrapper reuse. A passing rerun alone cannot resolve the intermittent failure.

The original invocation and the five reruns are recorded in the [Phase 4 review summary](../../.planning/phases/04-asset-production/04-24-SUMMARY.md) and the [closure exception](../../.planning/phases/04-asset-production/04-CLOSURE-EXCEPTION.md).
