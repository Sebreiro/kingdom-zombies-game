# Game development tools

These programs prepare, inspect and test KingdomZombies. They are game-specific, so they live in the game workspace beside its code and tests. They stay outside `game/`, the Godot runtime resource root.

## Tools by purpose

| Location | Purpose |
| --- | --- |
| [verification](verification/check.cjs) | Code-rule, architecture, state, engine and documentation checks against this game's declared inventory and evidence contracts |
| [verification/bootstrap.cjs](verification/bootstrap.cjs) | Verify pinned tool identities, prepare authentic Godot packages and run headless compatibility proofs |
| [gameplay-checks](gameplay-checks/check.cjs) | Menu, movement and local-game scenarios, input drivers, screenshots and report validation |
| [lan-checks](lan-checks/check.cjs) | Multiplayer scenarios, process coordination, timing, input and image evidence, and inherited-check orchestration |
| [dotnet.cjs](dotnet.cjs) | Select the workspace SDK configuration while preserving explicit subject paths; an internal launch helper |
| [repository-layout.test.cjs](repository-layout.test.cjs) | Check workspace/tool ownership, project placement and command root discovery |
| [code-rules](code-rules/docs/README.md) | Twelve C# syntax/style analyzers plus this repository's exact Godot/KingdomZombies generated-code permissions |

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

Purpose names replace the old `tools/phase1`, `tools/phase2` and `tools/phase3` command paths. No compatibility wrappers remain. Existing protocol/case names and ignored `.phase1/`, `.phase2/`, `.phase3/` cache/report locations keep their identities so old evidence stays traceable. A name inside an original report does not describe a current tool directory.

The Phase 4 asset pipeline handles source extraction, content validation and publication, saved inspection scenarios, and portable Windows exports. Integrate it as `asset-pipeline/` beside the other game tools. Its report and cache location is `.phase4/` at the checkout root. Keep original reports intact and execute new checks after changing inputs. [Phase 4 integration guide](../../../docs/workflow/phase4-layout-integration.md) · [Historical paths](../../../docs/architecture/repository-layout.md#historical-paths)
