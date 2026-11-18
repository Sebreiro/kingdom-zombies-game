# Plan 04-22 Task 2 technical checkpoint

Recorded 2026-11-18. Branch `feature/04-asset-production`. Task 1 is committed as `f7f735e` and its fresh committed tracer passed. Task 2 is incomplete and its source is uncommitted. No phase, requirement, current-policy, producer or actual acceptance completion is claimed.

## Completed Task 1

Five files committed: `src/godot-game/content/text/en.json`, `src/godot-game/content/text/expanded.json`, `src/godot-game/content/packs/text-data.json`, `tools/phase4/cases/text-data.cjs`, `docs/content/fields.md`.

- Initial source-only catalog pass: `.phase4/runs/db8edcf8-92d2-489e-81e2-d919f247830e/`.
- Final Task 1 GREEN: `.phase4/runs/a5354093-bebc-40a2-a940-86e0e2f65f40/`.
- Mandatory fresh same-case committed tracer: `.phase4/runs/207a22ce-409b-478d-babe-ec87dec91180/`, `text.catalog`, PASS, source `f7f735e`.
- Field union is 4,603 unique identities across 18 config domains with zero duplicate/omitted pointers. Inline documentation metadata is explicitly excluded when comparing the four inline root defaults. Every actual child field/default is independently checked.
- English includes all current source labels, 24 exact item name/description keys, typed count/day/player/item/cost/target/file/field placeholders, zero/one/other forms and 17 current binding action references. Expanded replacement rows preserve IDs/types and at least double label lengths.
- Full designer documentation is approximately 2.65MiB. The pack retains its full gzip/base64 bytes, decoded length and SHA256 within the existing 1MiB JSON limit. No Root/read cap changed.

## Task 2 current work

Only its five planned implementation files were edited/created:

- `src/godot-game/content/compatibility/appearance-layout.json`: DT-24 metadata-only saved/current/missing fixtures and 16 distinctly named text/data state backstops. No image/audio payload or save transaction.
- `src/godot-game/content/packs/compatibility.json`: current path/hash bindings, pending acceptance and empty completed-obligation list.
- `src/godot-game/inspection/DataTextPreview.cs`: new internal guarded owner. Authenticates current text/data bindings and decoded documentation; exposes native read-only fields/text, state/expanded selectors and current-binding/count examples. DT-24 reuses actual AppearanceState, WorldPreview, BuildingPreview and muted AudioPreview.Restore owners, with saved footprint/orientation geometry beside current destination. Native semantics remain unverified.
- `src/godot-game/inspection/InspectionRoot.cs`: new data-owner field/lifecycle/Rows/Selection wiring through existing method bodies. No authored Root method/signature was added. Exact Root signatures and final generated surface must still be independently inventoried.
- `tools/phase4/cases/text-data.cjs`: Task 2 fixture authoring and constants added. The `text.compatibility` case and actual native proof are not implemented yet.

## Exhausted original Task 2 correction budget

All strict builds used the existing command, normal analyzers and no bypass:

`dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`

| Attempt | Actual result | Correction applied afterward |
| --- | --- | --- |
| Initial implementation build | 9 errors: CS0029 signal Action delegate conversion; Root SCR0006/MA0051 body 66/67 lines; CA1861 repeated count array; 4 SCR0005 null-forgiving uses; CA1001 owner not disposable. 0 warnings. | Correction 1: exact Godot delegate type; IDisposable; static counts; explicit native owner checks; condense only added Root wiring into existing-body helper call. |
| Correction 1 build | 2 errors: Root Selection SCR0006 62 lines / MA0051 63 lines. 0 warnings. | Correction 2: retain closed prior data owner until synchronous replacement and compact existing copy expressions without changing signatures or copy. |
| Correction 2 build | 2 errors: Root lines475/481 SCR0007, 125/123 characters. 0 warnings. | Correction 3: preserve exact copy through DataTextPreview.Unknown/Shared constants, leaving Root methods/signatures unchanged. |
| Correction 3 build | 1 error: `DataTextPreview.cs(23,17): SCR0008: 'Dispose' (public method) must come before 'Shared' (internal constant)`. 0 warnings. | None. Original three corrections exhausted. |

## Minimal proposed next scope

Before further source changes, prospectively authorize a bounded repair scope for declaration order: move the `Unknown`/`Shared` internal constants below the public Dispose method in the same DataTextPreview file, preserving values and behavior, then run the unchanged strict build. This does not grant native permissions, generated producer enrollment, public identities, reader-cap changes or unowned Root refactors.

After that repair succeeds, Task 2 still needs substantive implementation review and its actual production-native proof, independently defined backstops, rejection/restored controls, current compatibility loops/state observations, scoped task commit, fresh complete selected set, exact final policy/producer observation, artifact/native tuple inventory and canonical summary/self-check. Preserve all existing historical reports and the original exhausted budget. A repair scope is separate and must not reset or hide it.

## Evidence limitations and unchanged ownership

No real game window opened. No sound listening, actual readability, final mix, Windows acceptance or ASSET completion is asserted. No index adoption, policy allowance or generated-cohort enrollment was performed. Parent retains globals, PR5/push, wave25 synchronization and final integration23/31. Existing WINDOWS29/30/31 and all original failures remain unchanged.
