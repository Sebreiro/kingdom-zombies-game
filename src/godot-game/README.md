# Game workspace

All game runtime projects, resources, reference artwork and game verification tests belong in this workspace. Open [KingdomZombies.slnx](KingdomZombies.slnx) for the five-project C# solution, or [game/project.godot](game/project.godot) in Godot.

| Folder | Responsibility |
| --- | --- |
| [game](game/docs/README.md) | Godot project, engine integration, scenes, runtime assets and native proof scenes; `res://` starts here |
| [core](core/docs/README.md) | Engine-independent state, identity, time/random inputs and snapshot contracts |
| [multiplayer-abstractions](multiplayer-abstractions/docs/README.md) | Replaceable service contracts without concrete engine or network implementations |
| [tests/verification](tests/verification/docs/README.md) | C# rule, architecture, state, engine and game integration tests; the historical assembly name and case IDs remain stable |
| references | Art direction examples, outside the runtime resource tree; written specifications control final behavior and silhouettes |
| [build](build/README.md) | Banned API lists and the specialized generated xUnit entry-point policy |
| [tools](tools/README.md) | Game-specific verification, local gameplay and LAN programs, shared launch helpers and their self-tests |

The separate assemblies keep their existing dependency boundaries. Moving them beneath one workspace does not permit Core or service contracts to depend on Godot. The reusable analyzer is the fifth solution project and remains in [tools/code-rules](../../tools/code-rules/docs/README.md).

## Build configuration

| File | Purpose |
| --- | --- |
| [global.json](global.json) | Exact .NET SDK selection |
| [NuGet.config](NuGet.config) | Approved package sources and verified local Godot feed |
| [Directory.Build.props](Directory.Build.props) | Shared language, nullable, warning, analysis and version settings |
| [Directory.Build.targets](Directory.Build.targets) | Analyzer build edge and scoped banned API files |
| [Directory.Packages.props](Directory.Packages.props) | Central exact package versions |
| [.config/dotnet-tools.json](.config/dotnet-tools.json) | Pinned local coverage tool |

Run direct .NET commands from this directory. The repository's automated launch helpers select it while keeping explicit project and fixture paths. See [setup](../../docs/workflow/setup.md) and [verification](../../docs/workflow/verification.md) for commands and evidence requirements. The [repository layout policy](../../docs/architecture/repository-layout.md) governs new files.
