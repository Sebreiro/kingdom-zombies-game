# Repository layout policy

## Ownership

All authored game runtime code, scenes, shipped assets, reference artwork and C# game/architecture verification tests belong beneath [src/godot-game](../../src/godot-game/README.md). Its `game/` directory is the actual Godot project and resource root. Core, service contracts, tests and build policy are workspace siblings outside that resource tree. Keep distinct project assemblies and the [checked dependency boundaries](boundaries.md).

Game-specific verification, gameplay, LAN and asset-production programs belong under [src/godot-game/tools](../../src/godot-game/tools/README.md), beside their self-tests. Name folders by responsibility: `verification`, `gameplay-checks`, `lan-checks`, and `asset-pipeline` for the Phase 4 asset tooling. Keep these outside the Godot runtime resource tree. Phase numbers belong to delivery records and historical evidence, not current tool-folder names.

All current tools belong in the game workspace. The analyzer's generic rules are reusable, but its configured copy embeds exact KingdomZombies/Godot generated-source permissions and therefore lives in `src/godot-game/tools/code-rules` too. It still has no game assembly references. A future product-independent package extraction would need to remove that configuration coupling before earning a separate repository-level home. Game behavior tests also belong in the workspace. Put explanatory rules, architecture and workflow documents under `docs/`, approved game specifications under `specs/`, and delivery records under `.planning/`.

The root contains the repository map, agent instructions and repository-wide Git/editor configuration. Do not add a root game solution, C# project, banned-symbol list, package manifest or shared .NET build policy. The solution, SDK pin, NuGet sources, local tool manifest and conventional `Directory.*` files belong at the game workspace root; specialized policy belongs in its [build folder](../../src/godot-game/build/README.md).

The [layout regression checks](../../src/godot-game/tools/repository-layout.test.cjs) check solution membership/placement and automated .NET invocation. The [evaluated project policy](../../src/godot-game/tools/verification/projects.json) and [compiled boundary checks](../../src/godot-game/tests/verification/ProjectEvaluator.cs) enforce the same five project identities and allowed references. Changes to physical layout must update these consumers together, without weakening their rules.

## Commands and generated state

Open [game/project.godot](../../src/godot-game/game/project.godot) in Godot and [KingdomZombies.slnx](../../src/godot-game/KingdomZombies.slnx) in the C# IDE. Node runners start from the repository root. Direct .NET commands start from `src/godot-game`; SDK selection begins at the process working directory. All five C# projects inherit the workspace's shared build and package settings.

Generated, regenerable state lives in one ignored folder at the root of every checkout, `.local/`. [workspace-paths.cjs](../../src/godot-game/tools/workspace-paths.cjs) and its C# twin [WorkspacePaths.cs](../../src/godot-game/tests/verification/WorkspacePaths.cs) are the one resolver for its locations; tools do not build these paths themselves.

| Subfolder | Content | Scope |
| --- | --- | --- |
| `tools/` | Node, the Godot editor and the Linux VM | Shared |
| `downloads/` | Pinned archives and checksums, export templates, Node archives and VM preparation archives | Shared |
| `nuget/` | NuGet global packages, HTTP cache and scratch; the verified Godot package feed `nuget/feed/` | Shared; the feed is per checkout |
| `locks/` | The toolchain and desktop locks; the check lock | Shared; the check lock is per checkout |
| `runs/<tool>/` | Reports of `verification`, `gameplay-checks`, `lan-checks` and `asset-pipeline` | Per checkout |
| `temp/` | Temporary files, the CLI home and per-purpose scratch | Per checkout |

Shared folders exist once per repository, in the main checkout's `.local/` (the shared root). A linked worktree finds it through `git rev-parse --path-format=absolute --git-common-dir`, whose parent is the main checkout. The environment variable `KZ_SHARED_LOCAL` overrides it with an absolute folder for CI, extra clones, source copies and bare repositories; a folder that is not a Git checkout uses its own `.local/`. Bootstrap, the register package reads of the C# tests and the asset pipeline's toolchain reads accept a location in the shared root only for their named purposes (the Godot tools and downloads, and NuGet packages) and only after resolving links. Every other path stays inside the checkout. Build and import outputs (`bin/`, `obj/`, `.godot/`) and the GSD-owned `.planning/tmp/`, `.planning/state.json` and `.planning/milestone.lock` stay where their tools expect them.

This layout moves where outputs are written, not historical execution evidence. Do not commit generated `bin/`, `obj/`, `.godot/`, caches or local reports. See [setup](../workflow/setup.md#workspace-folder), [workspace cleanup](../workflow/cleanup.md) and [verification](../workflow/verification.md).

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
| `tools/code-rules/` | `src/godot-game/tools/code-rules/` |
| `tools/dotnet.cjs`, `tools/repository-layout.test.cjs`, `tools/workspace-cleanup.cjs`, `tools/workspace-cleanup.test.cjs` | Same names under `src/godot-game/tools/` |
| `tools/phase4/` | `src/godot-game/tools/asset-pipeline/` |
| `production/phase4/` | `src/godot-game/production/assets/` |
| `.phase1/cache/phase4-exports/` | Shared `.local/downloads/export-templates/` |
| `.phase1/cache/` | Shared `.local/downloads/godot/` |
| `.phase1/tools/linux-vm/` | Shared `.local/tools/linux-vm/`, recreated by the phase that needs a VM |
| `.phase1/tools/linux-prep/` | Shared `.local/downloads/linux-prep/` |
| `.phase1/tools/` | Shared `.local/tools/godot/` |
| `.phase1/feed/` | The checkout's `.local/nuget/feed/` |
| `.phase1/packages/`, `.phase1/http-cache/` | Shared `.local/nuget/packages/`, `.local/nuget/http/` |
| `.phase1/cli-home/`, `.phase1/dotnet-home/` | The checkout's `.local/temp/cli-home/` |
| `.phase1/temp/`, `.phase1/runs/`, `.phase1/check.lock` | The checkout's `.local/temp/`, `.local/runs/verification/`, `.local/locks/check.lock` |
| Other `.phase1/<name>` | The checkout's `.local/temp/verification/<name>` |
| `.phase2/runs/`, `.phase2/completions/` | The checkout's `.local/runs/gameplay-checks/`, `.local/runs/gameplay-checks/completions/` |
| `.phase2/desktop.lock` | Shared `.local/locks/desktop.lock` |
| Other `.phase2/<name>` | The checkout's `.local/temp/gameplay-checks/<name>` |
| `.phase3/runs/`, other `.phase3/<name>` | The checkout's `.local/runs/lan-checks/`, `.local/temp/lan-checks/<name>` |
| `.phase4/runs/`, `.phase4/acceptance/` | The checkout's `.local/runs/asset-pipeline/`, `.local/runs/asset-pipeline/acceptance/` |
| Other `.phase4/<name>` | The checkout's `.local/temp/asset-pipeline/<name>` |

For the legacy `.phaseN` folders the longest matching prefix wins. `workspace-paths.cjs` holds this map as `LEGACY_MAP`, and values compared with pinned or captured bytes, such as register `package` cells, keep their logical `.phaseN` spelling and resolve through it only where a file is opened. Phase 4 inputs that existed only in ignored folders were declared lost on 2026-11-23, so their mapped locations stay missing. [workspace-migrate.cjs](../../src/godot-game/tools/workspace-migrate.cjs) moves a main checkout's legacy caches once; see [setup](../workflow/setup.md#moving-a-checkout-to-the-local-folder).

When integrating unfinished work from another feature branch, apply this mapping to new files, project references, runner paths and live navigation. Keep `res://` paths relative to the new `game/` resource root. Revalidate builds, permissions and affected scenarios; a previous branch's execution reports describe its original inputs.

Phase 4 content and pack records keep paths under `src/godot-game/`, `production/phase4/` and `tools/phase4/` as stable logical identities, so resource IDs, archive members and pinned recipe bytes do not change with the layout. [layout.cjs](../../src/godot-game/tools/asset-pipeline/layout.cjs) is the one map that resolves these identities to the physical folders above and back. Runner bindings, such as case-module paths and hashes, use physical paths.

The [Phase 4 integration guide](../workflow/phase4-layout-integration.md) was applied when the asset-production PR landed.
