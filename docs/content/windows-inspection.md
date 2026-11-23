# Windows inspection and evidence

## Launch the portable package

Copy or extract the entire portable folder to a writable directory. Keep `KingdomZombies.exe`, `KingdomZombies.pck` and `data_KingdomZombies_windows_x86_64` together. The folder supplies the Godot executable, self-contained .NET runtime and the current content pack.

1. Open `KingdomZombies.exe` from that folder. The `asset_inspection` feature selects the Inspection hub. Use 1280×720 for the Phase 4 Windows review session.
2. Wait for **Validated; visual/listening acceptance pending**. The hub shows the current pack count. Select `IT-07` in Items to inspect the Bow.
3. Use the window close button and wait for the process to exit. Open the same executable from the same folder and check the same resource again.

For a headless launch and natural exit, run the following command from the portable folder in PowerShell.

```powershell
.\KingdomZombies.exe --headless --quit-after 360
```

## Build and verify

From the repository root, run the complete export group and the portability case with the pinned Windows tools. The portability case writes the retained folder path, file hashes, fresh launch and relaunch identities, exact missing-resource and valid-sibling observations, and runtime-resolution evidence to `portable-delivery.json` in its run directory. Use that identified folder for the focused review and acceptance. [Development setup](../workflow/setup.md) · [Phase 4 evidence handoff](../../.planning/phases/04-asset-production/04-HANDOFF.md)

```powershell
node src/godot-game/tools/asset-pipeline/check.cjs --group export
node src/godot-game/tools/asset-pipeline/check.cjs --case export.portability
```

The export uses the permanent **Phase4 Windows inspection** preset and the authenticated official Godot 4.7.2 .NET Windows template. Runtime content includes the English catalog, the supporting documentation, fonts, audio, layouts and exact indexed data from the game project. The Windows executable uses the official template metadata. The game application title and icon are part of the project and packaged resources.

For source inspection, open the pinned Godot .NET project and run `inspection/Inspection.tscn` as the current scene. The ordinary project launch opens Foundation. The portability receipt records headless Windows results. Actual Windows input, appearance, readability, listening and judgments follow independent review. Linux execution has a separate platform owner. [Resource inspection index](README.md)
