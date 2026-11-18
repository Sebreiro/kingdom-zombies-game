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

## R1 continuation: declaration order passed, proof setup budget exhausted

Prospective scope: coordinator commit `63cba90`, [04-22-REPAIR.md](04-22-REPAIR.md). The original Task 2 budget remains 3/3 exhausted.

Before editing, all nine planned files were copied byte-for-byte to `.phase4/04-22-r1-before/`; its `inputs.json` records raw file hashes and all five starting Task 2 hashes matched the prospective repair record.

The four original process log files were not saved and their original paths/hashes are unavailable. Available exact tool output blocks were recovered from the executor's actual session transcript and saved as explicitly transcript-derived records, without claiming they are original process logs. Source transcript: `C:/Users/User/.codex/sessions/2026/11/18/rollout-2026-11-18T15-53-49-01a1ef26-5449-7e12-8d7d-15af0ba18aea.jsonl`.

| Attempt | Transcript line | Saved record under `.phase4/04-22-r1-before/` | SHA256 |
| --- | --- | --- | --- |
| Original initial | 293 | original-build-transcript-0.json | 724b866bdfa0a4736a779ad2c4855cc18b02fc189bd72887af5581e8d1ac21f7 |
| Original correction 1 | 303 | original-build-transcript-1.json | 47d2110d9e5ab8a263cc09cf1d1355fedf826168aec46b500a224e5ff0faaa8d |
| Original correction 2 | 321 | original-build-transcript-2.json | cd1fe8cf825299b1fecb1c17bc3a80f33b5aba09438ce8aed3a56d94dab362dd |
| Original correction 3 | 329 | original-build-transcript-3.json | 873328566a93fd692c47c41f72da7d7174ca5f996d092d7e615f51d0a59070a5 |

`original-build-evidence.json` additionally records exact tool call IDs and the original-log limitation.

### R1.1 result

Only the existing Unknown/Shared declarations moved below public Dispose and before the internal constructor. An exact token/byte comparison after removing that moved block passed; all other eight files stayed fixed. The unchanged strict solution build passed with zero warnings/errors in 13.40 seconds. **R1.1 correction count: 0/2.** Raw logs: `.phase4/04-22-r1-before/r1-1-initial.stdout.txt` and `r1-1-initial.stderr.txt`; their hashes and all nine resulting file hashes are in `r1-1-result.json`.

### R1.2 result

Only `tools/phase4/cases/text-data.cjs` changed. It now contains the first controller/actual-hub proof implementation and distinctly named 16 native state predicates, but none has executed natively yet. Every invocation passed the strict production build with zero warnings/errors before failing test-controller compilation. No setup failure is treated as semantic rejection.

| Attempt | Invocation | Actual outcome |
| --- | --- | --- |
| Initial | d9c5e7f9-6d51-4cd9-8cf1-f91360874e5a | Test controller build: 28 errors, comprising five nullable reflection casts and ordinal string analyzer requirements. No native launch. |
| Correction 1 | c5bc8f4f-267b-43b3-acb0-0bbda07204e9 | Editing script failed before writing because of an invalid Python replacement escape. The same shell erroneously continued into an unchanged proof/build. This attempt is preserved and counted; no source correction or native outcome occurred. |
| Correction 2 | de62f716-fa1d-4bd3-b992-fd92403bb1c8 | Checked reflection getters and explicit ordinal equality/prefix/suffix comparisons corrected the earlier setup errors. Three MA0002 errors remain on the loop-set comparison's two Order calls and SequenceEqual. No native launch. |

**R1.2 correction count: 2/2 exhausted.** All eight fixed production/output files still exactly match `r1-1-result.json`. Raw commands/stdout/stderr and result records remain in each immutable `.phase4/runs/{invocation}/` directory. Their precise paths/hashes, attempt identities and current proof-source hash are recorded in `.phase4/04-22-r1-before/r1-2-result.json`; the current case source is also preserved as `r1-2-final-text-data.cjs`.

The exact remaining expression in TEXT_HARNESS is `loops.Split(',').Order().SequenceEqual(expected.Order())`. The minimal proposed next proof-only correction adds `StringComparer.Ordinal` to both Order calls and SequenceEqual, preserving the exact expected loop set and predicate. This needs a new prospective bounded coordinator scope before further edits. Fixed DataTextPreview, Root, compatibility resources and Task 1 outputs remain out of scope for that proof-only correction. Any later real production semantic defect requires its own precise scope.

Task 2 remains uncommitted and incomplete. No canonical complete SUMMARY, final selected51 pass or final changed policy/producer observation exists. Task 1's committed tracer remains unchanged. The unrelated concurrent Phase 7 discussion checkpoint was observed and left untouched.
