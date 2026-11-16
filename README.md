# KingdomZombies

A Godot/C# side-view survival and camp-management game for solo or up to four cooperative LAN players on Windows and Linux. The [version 0.1 specification](specs/0.1/initial-spec.md) defines required behavior.

## Start here

- Open the game in Godot: [project.godot](src/godot-game/game/project.godot).
- Open the complete C# solution: [KingdomZombies.slnx](src/godot-game/KingdomZombies.slnx).
- Set up pinned dependencies: [development setup](docs/workflow/setup.md).
- Run checks and interpret evidence: [verification](docs/workflow/verification.md).
- Review a pull request: [code review entry point](docs/workflow/code-review.md).
- Work as an AI contributor: [agent guide](AGENTS.md).

## Repository map

| Location | Purpose |
| --- | --- |
| [src/godot-game](src/godot-game/README.md) | Complete game workspace: runtime projects, scenes, assets, references, game tests, solution and .NET configuration |
| [tools](tools/README.md) | Build-time code analyzers and programs that prepare dependencies, run checks and collect evidence |
| [docs](docs/architecture/README.md) | Code rules, architecture, development and review guidance |
| [specs](specs/0.1/initial-spec.md) | Approved game behavior and resource requirements |
| [.planning](.planning/STATE.md) | Delivery plans, decisions, progress and historical evidence references |

The root keeps repository-wide editor, Git and agent configuration. Generated build output and ignored verification caches/reports stay out of source control. The [layout policy](docs/architecture/repository-layout.md) defines where new files belong and maps historical paths to the current layout.

Run Node check commands from the repository root. Run direct .NET commands from `src/godot-game` so its SDK pin and NuGet configuration apply:

```sh
cd src/godot-game
dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false
```

First complete the isolated environment and verified engine-feed preparation in [setup](docs/workflow/setup.md). A fresh checkout does not yet contain that feed.
