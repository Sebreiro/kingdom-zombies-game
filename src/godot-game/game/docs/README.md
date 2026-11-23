# Godot integration reference

The existing Godot launch target composes Core and service ports. Its small native proof scene exercises engine-owned construction, resources, exported values, generated partials, signals, main-thread access and repeated teardown. These are development proofs; menus, movement and LAN remain later-phase work.

[Project](../KingdomZombies.csproj) · [Executable project policy](../../tools/verification/projects.json) · [Native fixture](../proofs/Phase1.cs) · [Code index](../../../../docs/code/README.md)

## Resource inspection

The Godot project contains production resources and inspection fixtures. The ordinary project launch opens Foundation for solo and LAN. The Windows inspection export opens the resource hub through the `asset_inspection` feature.

For source inspection, run `inspection/Inspection.tscn` as the current scene. In a portable Windows folder, launch `KingdomZombies.exe` beside `KingdomZombies.pck` and `data_KingdomZombies_windows_x86_64`. Close the window and relaunch the same executable to repeat an inspection.

Use Characters, Items, Buildings/world, UI, Data/text or Audio. Register lists all 180 required IDs. The summary shows the current ID, name and status. Use **Show details** and **Copy details** to capture exact resource information. For a missing resource, record its ID, path, field and reason, open a valid sibling and use **Recheck resources** after restoring the missing bytes.

In Data/text, DT-18 opens the designer fields, DT-19–DT-23 open the production libraries, DT-26 lists the saved fixtures and DT-27 opens the Windows guides. TXT-01 provides the English catalog and TXT-02 provides expanded text for inspection. [Content index](../content/pack-index.json) · [English catalog](../content/text/en.json) · [Delivery crosswalk](../content/packs/delivery.json) · [Designer fields](../content/packs/text-data.json)

The portability receipt records the headless package result. Phase 4's focused Windows review records the final appearance, input, readability and listening evidence. Linux execution follows separate platform verification.

## Reference declaration

The two direct production references implement composition. Godot SDK packages supply engine APIs. The separate analyzer build edge must retain both metadata values.

```project-policy
{"project":"src/godot-game/game/KingdomZombies.csproj","policy":"src/godot-game/tools/verification/projects.json","productionReferences":["src/godot-game/core/KingdomZombies.Core.csproj","src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj"],"inspectionReferences":[],"analyzerReference":{"path":"src/godot-game/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj","OutputItemType":"Analyzer","ReferenceOutputAssembly":"false"}}
```
