# Foundation checks

The small Phase 2 runner checks the ordinary menu and playable foundation. The [temporary testing policy](pre-phase5-testing.md) governs Phases 2–4: implement the whole phase, review and fix its code, then run rendered cases on the development OS at 1280×720. Other-OS and 1920×1080 acceptance and dependency-aware reuse certification remain Phase 5 work. Prepared tools and historical reports supply no new pass.

Use the pinned tools and workspace-local environment in [setup](setup.md). Build the existing solution before pure checks; no new project or package is required:

```sh
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase2/check.cjs --list
node tools/phase2/check.cjs --group core
node tools/phase2/check.cjs --group runner
```

These commands never launch the game, including native version or renderer probes. Pure C# checks use the existing test executable with exact method filters, one passing executed test and a nonce-bound proof result. The runner controls use real Node children for missing executable, nonzero exit, crash, deadline and descendant cleanup tests; they do not send desktop input.

After the phase code review and its fixes, execute the final rendered selection:

```sh
node tools/phase2/check.cjs --group rendered
```

All nine cases share one native game process. Menu runs first to exercise the scoped one-shot entry error; each successful case returns through Escape, and the next starts through ordinary menu controls. Quit runs last, using Tab then Space; the report records that activation method. Start exercises mouse and keyboard activation. A separate mouse Quit exit is not executed in this selection. Resource isolation validates the declared resource closure and byte-identical repository copies, using the shared live scene for runtime behavior rather than starting another game. Keyboard and mouse actions go through verified owned foreground windows. On Windows the input helper confirms the owned PID/HWND; the focus case uses its own temporary focus sink. The retained Linux helper is portable preparation and is not required to run under the temporary policy.

`--case ID` runs one exact catalog case. `--all` runs the seven pure cases and the nine rendered cases, with one shared game session. Use it only after implementation and review. For ordinary manual play, launch the pinned native Godot executable with `--path src/godot-game --rendering-method gl_compatibility --resolution 1280x720`; the main menu is the entry point.

## Required case catalog

Required membership comes from [required-cases.json](../../tools/phase2/required-cases.json), independently of discovered/passing tests. No pending catalog entry earns credit.

| Case | Required behavior / specification criteria |
| --- | --- |
| core.authority | Identified 3u/s commands, validation, ground and bounds; AC-04–AC-06 |
| core.input | Aliases, opposing input and release; AC-03 |
| core.lifecycle | Closed authority and held-input lifetime barriers; AC-07–AC-08 |
| core.permissions | Exact guarded native access and rejecting neighbors; AC-06 |
| rendered.tracer | Launch, Start, walking, facing, Escape; AC-01–AC-03, AC-06 |
| rendered.menu | Focus navigation, mouse hover/press, recoverable entry and keyboard retry; AC-01, AC-07 |
| rendered.directions | Both key families, aliases, opposing input, release and repeat; AC-03 |
| rendered.distance | One-second game-time intervals and inclusive 2.85–3.15u displacement; AC-04 |
| rendered.world | Building traversal, ground, both clamps and inward recovery; AC-05–AC-06 |
| rendered.lifecycle | Held/repeated activation, fresh actor/session and rapid restart; AC-07 |
| rendered.focus | Actual focus transfer, held aliases and fresh press; AC-08 |
| rendered.resources | Repository resource closure, isolated copy hashes and shared runtime; AC-10 |
| rendered.quit | Tab-focused keyboard Quit and clean process exit; AC-09 |
| runner.integrity | Empty/missing/duplicate/stale/image/path rejection; AC-12 |
| runner.processes | Actual child missing executable/nonzero/crash/deadline and cleanup; AC-12 |
| runner.repeatability | Ordering, fresh repair identity, immutable original, concurrent lock refusal; AC-13 |

All rendered cases require launch menu, initial scene, right-facing, left-facing and returned menu images. Menu also requires the recoverable error; world requires building overlap and both bounds; lifecycle requires restart. Additional checkpoint images are retained. Image decoding is a structural check; appearance review remains separate under AC-11.

## Evidence and focused repairs

Every run creates a new `.phase2/runs/<platform>/<nonce>/summary.json`, input snapshot, case results and logs. A shared native `session/` contains unique capture IDs, state JSON, PNGs, request audit, native input acknowledgements and game log. Reports record the tested Git revision, source fingerprint, tool identity, nonce, platform, timing and each selected case's pass/fail/blocked status. Never edit a run, replace an image or overwrite a failure. Legacy schema-1 runs remain historical and are not accepted as schema-2 repair baselines.

Scenario assertions are collected. When safe, the runner releases owned held keys and returns through Escape before continuing independent cases. A process crash or unsuccessful reset explicitly blocks remaining cases. Native input and observations each have a 12-second deadline; Quit has eight seconds; bound traversal has a 65-second wall-clock budget per side. Pure cases have 180 seconds. Timed-out owned child trees are terminated. The native game is cleaned up at the end, without terminating unrelated processes. A desktop lock refuses simultaneous rendered invocations; pure checks have separate invocation directories. After an interrupted runner leaves a lock, inspect its recorded PID and the owned game before removing that exact stale lock.

After collecting errors, batch repairs and rerun only failed or blocked cases from the printed report path:

```sh
node tools/phase2/check.cjs --failed .phase2/runs/win32-x64/BASELINE-NONCE/summary.json
```

A repair run keeps its own revision, fingerprint, nonce and images and binds the original report path/hash/revision in `repairOf`. If a repair still fails, use its report as the next `--failed` baseline. Earlier passes remain tied to their original reports even when repairs changed source fingerprints; this is the creator-authorized temporary policy, without Phase 5 reuse certification.

Link the pure reports, initial rendered report and each focused repair in chronological order:

```sh
node tools/phase2/check.cjs --complete CORE-REPORT RUNNER-REPORT RENDERED-REPORT REPAIR-REPORT
```

The command writes a new `.phase2/completions/<nonce>.json` with immutable report references, hashes, latest per-case outcomes and missing cases. It requires all 16 distinct catalog cases for `coverage-complete`. A later failure supersedes an earlier pass; reordered/duplicate reports, missing repair baselines or wrong repair selections are rejected. Source fingerprints are preserved but are not required to match between authorized repair runs. This coverage record does not imply appearance approval.

Copy the generated completion JSON into a new review record, preserve its `reports`, and add a `reviews` entry for each catalog-required image in the accepted case results. Additional observation images remain available for diagnosis:

```json
{
  "reportSha256": "exact original report hash",
  "id": "rendered.menu",
  "checkpoint": "launch-menu",
  "imageSha256": "exact PNG hash",
  "reviewer": "reviewer identity",
  "verdict": "pass",
  "notes": "Actual appearance and readability judgment for this image"
}
```

Review actual saved images for readable English labels/hint/error, focus and hover distinction, complete player and facing marker over the building and at bounds, camera/ground continuity and clipping. Record defects and repair affected cases before accepting their images. Do not invent a reviewer or approve from PNG decoding alone. Validate the new completion/review record:

```sh
node tools/phase2/check.cjs --verify-completion PATH-TO-REVIEW-RECORD.json
```

Validation reopens the original reports and images, verifies hashes, case/environment coverage and exact image review bindings. Missing evidence, nonzero/crash/deadline, stale identities, duplicate substitutions, corrupt images, unreviewed appearance and incomplete scope produce non-success. `--verify-platforms` is a compatibility alias for this temporary development-OS gate; it does not certify Linux or additional resolutions. Keep those deferred obligations explicit in the phase's completion record and draft PR.

The fixture uses repository-authored primitive content IDs `foundation.fixture.v1`, `foundation.ground.v1`, `foundation.building.v1` and `foundation.player.v1`. No donor input or external reference folder is used. Phase 1's historical closure exception and follow-ups remain separately recorded; this runner does not change them.
