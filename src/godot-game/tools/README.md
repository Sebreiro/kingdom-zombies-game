# Game development tools

These programs prepare, inspect and test KingdomZombies. They are game-specific, so they live in the game workspace beside its code and tests. They stay outside `game/`, the Godot runtime resource root.

## Tools by purpose

| Location | Purpose |
| --- | --- |
| [game-tests](game-tests/check.cjs) | Run the shared three-tier catalog and report the selected test results |
| [verification](verification/check.cjs) | Code-rule, architecture, state, engine and documentation checks against this game's declared inventory and evidence contracts |
| [verification/bootstrap.cjs](verification/bootstrap.cjs) | Verify pinned tool identities, prepare authentic Godot packages and run headless compatibility proofs |
| [gameplay-checks](gameplay-checks/check.cjs) | Menu, movement and local-game scenarios, input drivers, screenshots and report validation |
| [lan-checks](lan-checks/check.cjs) | Multiplayer scenarios, process coordination, timing, input and image evidence, and inherited-check orchestration |
| [dotnet.cjs](dotnet.cjs) | Select the workspace SDK configuration while preserving explicit subject paths; an internal launch helper |
| [repository-layout.test.cjs](repository-layout.test.cjs) | Check workspace/tool ownership, project placement and command root discovery |
| [code-rules](code-rules/docs/README.md) | Twelve C# syntax/style analyzers plus this repository's exact Godot/KingdomZombies generated-code permissions |
| [workspace-paths.cjs](workspace-paths.cjs) | The one resolver for the ignored `.local/` workspace root: shared and per-checkout locations, child-process isolation variables and the legacy `.phaseN` map |
| [workspace-cleanup.cjs](workspace-cleanup.cjs) | Task-end deletion of ignored outputs and finished worktrees; see [workspace cleanup](../../../docs/workflow/cleanup.md) |
| [workspace-migrate.cjs](workspace-migrate.cjs) | One-time, dry-run-first move of a main checkout's legacy `.phase1/` caches into `.local/`; see [setup](../../../docs/workflow/setup.md#moving-a-checkout-to-the-local-folder) |
| [node-setup](node-setup/setup.sh) | Pinned Node download, verification and shared install, and the agent SessionStart PATH hooks; see [setup](../../../docs/workflow/setup.md#shared-node) |
| [verification/ac08-bindings.cjs](verification/ac08-bindings.cjs), [verification/asset-bindings.cjs](verification/asset-bindings.cjs) | Regenerate and check the ac-08 repository stages and caller hashes, and the asset catalog, delivery and pack-index bindings |
| [verification/rebind-gates.cjs](verification/rebind-gates.cjs) | Fail-closed gates for a repository-wide move: unchanged history, no new machine paths, legacy-literal residue, case lists, tools tests, roots, the strict build, bindings and the docs group |
| [verification/linux-vm/start-vm.cjs](verification/linux-vm/start-vm.cjs) | Launcher for the owned Linux VM in the shared `.local/tools/linux-vm/` |

The analyzer's core rules are reusable, but this repository's configured copy includes game-specific generated-source permissions. It therefore lives here too, without introducing any game assembly dependency into the analyzer. Game C# tests live in [tests/verification](../tests/verification/docs/README.md); Node self-tests stay beside their tools here. No current tool folder remains at the repository root.

## Commands

Run these from the checkout root. The three check entry points also resolve their catalogs correctly when launched by absolute path from another directory.

```sh
node src/godot-game/tools/verification/check.cjs --list
node src/godot-game/tools/verification/check.cjs --group docs
node src/godot-game/tools/gameplay-checks/check.cjs --list
node src/godot-game/tools/lan-checks/check.cjs --list
node --test src/godot-game/tools/repository-layout.test.cjs src/godot-game/tools/lan-checks/*.test.cjs
```

Use [setup](../../../docs/workflow/setup.md) for dependencies, [verification](../../../docs/workflow/verification.md) for selecting cases and interpreting reports, and [LAN checks](../../../docs/workflow/lan-foundation-checks.md) for multiplayer commands. Direct .NET commands still run from `src/godot-game`.

## Historical identities and output

Purpose names replace the old `tools/phase1`, `tools/phase2` and `tools/phase3` command paths. No compatibility wrappers remain. Existing protocol and case names keep their identities so old evidence stays traceable. A name inside an original report does not describe a current tool directory.

Every tool writes its output under the ignored `.local/` root through [workspace-paths.cjs](workspace-paths.cjs): runs under `.local/runs/<tool>/` (`verification`, `gameplay-checks`, `lan-checks`, `asset-pipeline`), scratch under `.local/temp/<purpose>/`, both in the checkout that ran them. Toolchains, downloads and NuGet caches come from the repository's shared root. Original reports name the legacy `.phase1/`–`.phase4/` folders; the [historical path map](../../../docs/architecture/repository-layout.md#historical-paths) resolves them.

The ac-08 and asset binding tools regenerate bindings from the current bytes and check them; they are not replays of the one-shot refactoring or binding utilities that must not be reapplied. Run their `--check` mode after any change to a hashed file, and their `--write` mode only for a deliberate rebind, ac-08 last. Both `--write` modes refuse a tree with uncommitted changes other than their own output files and the `.planning`, `.claude`, `.codex` and `.agents` folders, so commit the edited code first, then write and commit the bindings alone.

The Phase 4 asset pipeline handles source extraction, content validation and publication, saved inspection scenarios, and portable Windows exports. It lives in `asset-pipeline/` beside the other game tools. Its runs go to `.local/runs/asset-pipeline/` (acceptance records in `acceptance/` there) and its scratch to `.local/temp/asset-pipeline/`. Pinned recipes that assert the original checkout's `.phase4/` folder stop with `CAPTURED_ROOT_REQUIRED` before launch: the export tracer, audio `--prepare`, `--approve-actions` and `--prepare-interface`, the media conversion controls and the audio-feasibility extraction. Phase 5 makes the Phase 4 checks it runs work in the relocated tree (05-CONTEXT D-46). Keep original reports intact and execute new checks after changing inputs. [Phase 4 integration guide](../../../docs/workflow/phase4-layout-integration.md) · [Historical paths](../../../docs/architecture/repository-layout.md#historical-paths)
