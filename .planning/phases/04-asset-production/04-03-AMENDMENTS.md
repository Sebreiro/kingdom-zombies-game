# Plan 04-03 — Bounded execution amendments

## Task 2 exact visibility authority (2026-11-16)

Observed compiled invocation `.phase4/runs/8990ed14-c721-4838-8185-0828abaad777/` ran28 controls:27 passed and `Authorize_PublicContracts_RejectsAdjacentTypesAndPaths` failed with `R-34: invalid or expanded exact visibility registry`. `ArchitecturePolicy.Visibility` independently pins15 path/type identities in C#; the plan's required two JSON reasons cannot authorize actual new public contracts without updating that authority.

The coordinator authorized **only `tests/phase1/ArchitecturePolicy.cs` as one additional Task2 implementation path**, before its edit. Task2 now owns four implementation paths; the plan owns eight. This record is execution metadata. No task, dependency, wave or product decision changes.

Append exactly these two required current identities to the independent C# expected array:

- `src/core/ContentPack.cs|KingdomZombies.Core.ContentPack`
- `src/godot-game/inspection/InspectionRoot.cs|KingdomZombies.Inspection.InspectionRoot`

Preserve all original15 identities, exact case/path/namespace/type matching, schema/shape/proof/reason validation, duplicate and expanded-registry rejection. No prefix/directory grant, generated permission change or absent future04-26 public type is authorized. Compiler-valid positive, wrong-path, sibling/extra-type and expanded-registry controls belong in the already owned `ContentInspectorTests.cs`. Required current source-policy, visibility, strict-build, compiled28 and actual native hub gates must pass before Task2 commit. Retain the27/28 failure and all prior native proof identities.
