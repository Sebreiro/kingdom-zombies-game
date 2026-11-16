# Repository layout policy

## Ownership

All authored game runtime code, scenes, shipped assets, reference artwork and C# game/architecture verification tests belong beneath [src/godot-game](../../src/godot-game/README.md). Its `game/` directory is the actual Godot project and resource root. Core, service contracts, tests and build policy are workspace siblings outside that resource tree. Keep distinct project assemblies and the [checked dependency boundaries](boundaries.md).

Game-specific verification, gameplay, LAN and asset-production programs belong under [src/godot-game/tools](../../src/godot-game/tools/README.md), beside their self-tests. Name folders by responsibility: `verification`, `gameplay-checks`, `lan-checks`, and `asset-pipeline` when unfinished Phase 4 tooling integrates. Keep these outside the Godot runtime resource tree. Phase numbers belong to delivery records and historical evidence, not current tool-folder names.

Only reusable, product-independent tools belong under the repository [tools](../../tools/README.md) root. The C# analyzer package stays there because its implementation has no game dependency. Game behavior tests belong in the game workspace. Put explanatory rules, architecture and workflow documents under `docs/`, approved game specifications under `specs/`, and delivery records under `.planning/`.

The root contains the repository map, agent instructions and repository-wide Git/editor configuration. Do not add a root game solution, C# project, banned-symbol list, package manifest or shared .NET build policy. The solution, SDK pin, NuGet sources, local tool manifest and conventional `Directory.*` files belong at the game workspace root; specialized policy belongs in its [build folder](../../src/godot-game/build/README.md).

The [layout regression checks](../../src/godot-game/tools/repository-layout.test.cjs) check solution membership/placement and automated .NET invocation. The [evaluated project policy](../../src/godot-game/tools/verification/projects.json) and [compiled boundary checks](../../src/godot-game/tests/verification/ProjectEvaluator.cs) enforce the same five project identities and allowed references. Changes to physical layout must update these consumers together, without weakening their rules.

## Commands and generated state

Open [game/project.godot](../../src/godot-game/game/project.godot) in Godot and [KingdomZombies.slnx](../../src/godot-game/KingdomZombies.slnx) in the C# IDE. Node runners start from the repository root. Direct .NET commands start from `src/godot-game`; SDK selection begins at the process working directory. The analyzer outside that workspace explicitly imports the shared build and package settings.

Existing ignored `.phase1/`, `.phase2/` and `.phase3/` cache/report locations remain stable. This cleanup moves authored content and configuration, not historical execution evidence. Do not commit generated `bin/`, `obj/`, `.godot/`, caches or local reports. See [setup](../workflow/setup.md) and [verification](../workflow/verification.md).

## Historical paths

Completed phase summaries, review records, captured donor provenance and immutable reports retain their original paths and source identities. Interpret those references using this migration map; do not rewrite an old report to certify relocated code. Live scenario/selection definitions used by runners can rebind source paths while preserving case identities; this does not validate an old report against new inputs.

| Historical location | Current location |
| --- | --- |
| `src/godot-game/project.godot` and its runtime contents | `src/godot-game/game/` |
| `src/core/` | `src/godot-game/core/` |
| `src/multiplayer-abstractions/` | `src/godot-game/multiplayer-abstractions/` |
| `tests/phase1/` | `src/godot-game/tests/verification/` |
| `examples/` | `src/godot-game/references/` |
| Root `KingdomZombies.slnx`, `Directory.Build.*`, `Directory.Packages.props`, `global.json`, `NuGet.config` | Same names under `src/godot-game/` |
| Root `.config/dotnet-tools.json` | `src/godot-game/.config/dotnet-tools.json` |
| Root `BannedSymbols*.txt`, `Directory.Xunit.targets` | Same names under `src/godot-game/build/` |
| `tools/phase1/` | `src/godot-game/tools/verification/` |
| `tools/phase2/` | `src/godot-game/tools/gameplay-checks/` |
| `tools/phase3/` | `src/godot-game/tools/lan-checks/` |
| `tools/dotnet.cjs`, `tools/repository-layout.test.cjs` | Same names under `src/godot-game/tools/` |
| Unfinished Phase 4 asset tools | `src/godot-game/tools/asset-pipeline/` when integrated |

When integrating unfinished work from another feature branch, apply this mapping to new files, project references, runner paths and live navigation. Keep `res://` paths relative to the new `game/` resource root. Revalidate builds, permissions and affected scenarios; a previous branch's execution reports describe its original inputs.
