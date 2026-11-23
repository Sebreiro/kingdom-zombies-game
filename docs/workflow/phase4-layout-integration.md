# Integrating Phase 4 with the game workspace

[The repository cleanup](https://github.com/Sebreiro/kingdom-zombies-game/pull/6) follows [Phase 4](https://github.com/Sebreiro/kingdom-zombies-game/pull/5) into `dev`. Bring the final `dev` into the cleanup branch and reconcile both sets of changes before merging the cleanup.

## Where the new files go

| Location on the Phase 4 branch | Location after the cleanup |
| --- | --- |
| `src/godot-game/assets/`, `content/`, `inspection/`, `proofs/` and other Godot project files under `src/godot-game/` | Corresponding paths under `src/godot-game/game/`. `res://` stays relative to that resource root. |
| `src/core/` Phase 4 changes | `src/godot-game/core/` |
| `tests/phase1/` Phase 4 tests, including newly added test files | `src/godot-game/tests/verification/` |
| `tools/phase4/` programs, case modules, manifests and proof-build targets | `src/godot-game/tools/asset-pipeline/` |
| `production/phase4/` authoring bundles, recipes and export inputs | `src/godot-game/production/assets/`, outside the Godot resource root |
| `docs/content/` and `docs/workflow/engine-defects.md` | Keep these locations and update links and commands for the relocated files. |

## Reconcile live references

1. Carry over changes from both branches to Core, the test suite, the inherited runners and the exact permission registers. Inspect newly added files that Git merged automatically, since a conflict list alone misses files that can land outside their new directories. Keep both sets of completed quick-task records in `.planning/STATE.md`.
2. Update project references, CommonJS imports and repository-root discovery, the Python extraction entry points, proof-build targets, reviewed-source file sets, case bindings and fixture paths. Include the Windows export profile, input hashes and package prerequisites. Preserve resource IDs and shipped bytes, and keep generated package paths aligned with the portable inspection contract.
3. Update commands and links in the [setup](setup.md), [verification](verification.md), production, Windows inspection and Godot defect guides. Keep the single-threaded Windows editor-import workaround and document its relocated self-test. Keep the two Windows export-layout checks outside the historical Phase 1 selection.
4. Keep `.phase4/` ignored at the repository root, alongside the existing report and cache directories. Keep captured provenance, previous report paths and original revision identities. Rebind the live catalog while preserving the focused Phase 4 handoff and its deferred scenario ownership.

## Verify the integrated checkout

Run the layout tests, list the inherited and asset-pipeline catalogs from a separate working directory, and run the strict locked solution build with analyzers plus affected documentation, boundary and permission checks. Verify the relocated runner can perform its editor-import self-test, build the portable Windows inspection package and launch it through the supported route. Apply the [local testing strategy](local-testing.md) to affected scenarios and report the evidence against the integrated revision. Phase 4 reports continue to describe the revisions they originally tested.
