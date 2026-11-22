# Plan04-29 R1 exhausted native scene-binding checkpoint

Recorded2026-11-18T12:02:06UTC on `feature/04-asset-production`, HEAD `b908d349fc5efc122f4bb703856137aafb7a5e5f`. Original Task1 remains exhausted3/3. R1 completed its initial verification and both authorized corrective attempts, **2/2 exhausted**. Task2 remains unstarted0/3. Stop before any further source or harness repair; no budget reset or canonical SUMMARY is authorized by this checkpoint.

## Current demonstrated blocker

Current `audio.lifecycle` invocation `8242f818-8c46-44d4-80bc-80a101694286` fails `INVENTORY_NATIVE_MARKER`. Production strict build and native harness build both succeed with0warnings/0errors; the one compiled ownership test succeeds with0skips. The actual authenticated headless native process reaches `Hub()` but Godot cannot instantiate the C# script from the production scene under the separate `InventoryProof` project:

```text
ERROR: Cannot instantiate C# script because the associated class could not be found.
Script: 'res://inspection/InspectionRoot.cs'.
Make sure the script exists and contains a class definition with a name that
matches the filename of the script exactly (it's case-sensitive).
at: can_instantiate (modules/mono/csharp_script.cpp:2358)
```

The exact current Root.cs source resource was staged after helper compilation and its actual production type was constructed from the loaded production assembly. This removes the earlier missing-file error but does not associate the scene script with its class in the helper project's registry. The scene retains static Loading status without running the actual production Root behavior. Existing `--quit-after800` terminates the process with exit0 before a completion report; this is **not** the required naturally completed proof or a clean passing exit. The frame cap was not changed. No stderr was hidden or filtered.

This remains a demonstrated native proof setup/class-binding defect. No saved report establishes individual Audio player, PCM, H19/H25/H26 or backstop passes. No production-domain/player/ownership defect is inferred from this bridge failure. Do not accept missing class, static scene state, exit0 or absent report as negative behavior evidence.

## Immutable verification sequence

| R1 stage | Invocation | Result |
| --- | --- | --- |
| Initial exact line73524→32 | `d6b9a7e4-752c-4de4-8c23-7e6ee8b02b8f` | Exposed adjacent line73632→40 SCR0009; stopped at exact scope boundary;0/2 corrections used |
| Corrective1/2, prospectively authorized byb908d34 | `a5ab71c2-d0f8-4dfe-933a-3c643baa74de` | Both strict builds0/0 and compiled1/1; authenticated nativePID24048 could not open missing copied `res://inspection/InspectionRoot.cs`; no completion report |
| Corrective2/2, audio.cjs-only demonstrated copied-input correction | `8242f818-8c46-44d4-80bc-80a101694286` | Both strict builds0/0 and compiled1/1; exact Root source staged after helper compilation; actual production Root type constructed; scene class binding still fails as above; no completion report |

Current production strict PID31712 exit0 at12:00:44.068UTC. Compiled ownership test PID13876 exit0 at12:00:45.135UTC. Harness build PID5656 exit0 at12:00:49.061UTC. Native PID37276 exit0 at12:01:22.430UTC through the existing incomplete-run frame cap. Reports/logs/exit records remain in the full current invocation directory `.phase4/runs/8242f818-8c46-44d4-80bc-80a101694286/`. The native failure occurs before the helper can create `native-inventory/inventory-results.json`; that file does not exist. The original marker rejection and stderr remain intact.

The current official nonconsole engine is `.phase1/tools/Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe`, version `4.7.2.stable.mono.official.ed1daf0bf`, SHA256 `45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab`. Its authenticated official archive SHA256 is `a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`; exact entry `Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe`. The invocation retains production assembly closure, source input fingerprints, engine/version/origin records and native command nonce. Authentication alone supplies no behavior pass.

## Exact fixed production and current harness

Immutable original five-file snapshot remains `.phase4/audio29-r1-checkpoint-8bef623/`, including verified raw hashes and exact file copies. During R1 only the two prospectively authorized AudioPreview leading-whitespace lines changed; every other production token/byte and Root/test/fixture is unchanged. The only audio.cjs repair stages exact current Root source bytes after helper compilation and constructs its actual loaded production type before the same existing scene assertions. Intended predicate strings, expected values, IDs, order, capacities, deadlines and source approval semantics were not weakened.

| File | Current SHA256 |
| --- | --- |
| `src/godot-game/inspection/AudioPreview.cs` | `c0e39826edf0ec613c5a0bcb51eb77f390b665d7241ebd7f98aa3d324b032bb7` |
| `src/godot-game/inspection/InspectionRoot.cs` | `6e831506dc3ee8b58ad59b2c857d81ad885d630e9f40839c2eafc478b0539e9a` |
| `tests/phase1/AudioPresentationTests.cs` | `3f6f8265a43be327cc5123a0b4a9facbea0317f502470e0dffe1acbd9ee7d01a` |
| `src/godot-game/content/fixtures/audio-states.json` | `4adf9d6c32c089a6afeefa074ba4cf5d6dc4c1e7396fddd32212cc55fcfe732b` |
| `tools/phase4/cases/audio.cjs` | `40690e11bdd945348ea0b3a4eece7f4ed32198db93aec1d727b37d5b92bf514c` |

## Concrete next decision

A prospective bounded continuation is required before another repair. One grounded setup route is the already proven `world.inspector` pattern: run the hub assertions in a separate authenticated owned copy of the **actual KingdomZombies project and main assembly**, with the exact current Root source/scene/production closures, rather than asking `InventoryProof` to resolve the production scene's C# script. The standalone plain AudioPreview controller bridge can remain in its own authenticated harness. Preserve the actual hub loading/navigation/page predicates and their original IDs/order in the combined lifecycle evidence; retain separate process/nonce/closure identities and require both natural completion and clean logs. This is a proposed proof setup adjustment, not an implemented or preapproved repair. Do not change fixed production semantics or take a new native grant to make a proof pass.

After any authorized actual Task1 GREEN, exact scope commit and a fresh committed same-case `audio.lifecycle` tracer must precede original Task2. Task2 still needs genuine RED/GREEN and full independent held-out limits/restore/replacement/backstop work. Final fresh49selection and one current native-policy/producer observation have not run. Canonical SUMMARY/self-check and complete symbol/native tuple inventory remain pending.

No implementation completion, native pass, game window, hearing, loop seam or native mix approval is claimed. Creator source approval remains only the40sources/44presentedversions;27new variants remain Not auditioned. Global planning state, allASSET,49oldconsumer comparisons,WINDOWS29/30/31,index0/180,19public identities and final32/33 boundaries remain unchanged. Parent owns globals/wave/PR/push. All prior checkpoint/RED/original failure reports remain preserved; no source work is rolled back.

## Self-check

All five implementation files exist and were hashed above after the failed current case. `git diff --check` passes. RED commit `d70ef24` remains in ancestry. The current case is explicitly FAIL; neither task is complete and there is no GREEN/tracer or canonical SUMMARY.
