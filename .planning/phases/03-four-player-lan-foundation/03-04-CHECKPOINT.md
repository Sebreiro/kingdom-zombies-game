# 03-04 implementation ready for review

2026-11-15. Tasks 1 and 2 are complete and locally committed; Task 3 has not started. This is the orchestrator's designed independent-review checkpoint, not a request for additional creator permission. No real game window or VM was opened. Branch remains `feature/03-four-player-lan-foundation`; the parent owns publishing, PR #4 and shared phase state.

## Completed tasks and commits

| Task | RED | GREEN | Result |
| --- | --- | --- | --- |
| 1: actual headless observation, separate native driver and conservative draw timing | `000ec98` | `2eccd01` | Required tracer/timing/permission gate passed 3/3 before Task 2 expansion. |
| 2: exact evidence/child failure controls, grouped procedures and saved selection | `a046c70` | `d40df64` | Final complete no-window selection passed 20/20; inherited core/runner and eight policy checks passed. |

The persisted plan ledger is `.git/gsd-plan-head-before-03-04`, based at `5f4480c`. No tracked deletion, new package, project/reference change, analyzer weakening or generated permission refresh occurred. All 220 generated hashes remain unchanged. Seven actual new native-calling draw-observer tuples use the existing captured gateway and complete compiler-valid positive/adjacent rejection controls; the independent LAN tuple count is now 42.

## Final current non-window evidence

All paths below identify immutable ignored runtime reports, not newly claimed rendered evidence.

| Selection | Result | Report |
| --- | --- | --- |
| All 20 current Phase 3 non-window cases | 20/20 pass, 142506 ms, zero game windows | `.phase3/runs/win32-x64/63922b36-60d0-4954-b9e6-413d9f01ecee/summary.json` |
| Phase 2 core | 4/4 pass | `.phase2/runs/win32-x64/4565ba76-8224-4122-940a-52b74490e167/summary.json` |
| Phase 2 runner | 3/3 pass | `.phase2/runs/win32-x64/a3c0841e-e305-42bd-9276-b68e01de656a/summary.json` |
| `engine.source-policy` | 1/1 pass | `.phase1/runs/3d99e088-a5a8-44f6-b368-23eaff6ffff3/summary.json` |
| `engine.generated-origin` | 1/1 pass | `.phase1/runs/5acb07bd-67b9-4cec-8957-685a009a3d3d/summary.json` |
| `engine.generated-boundary` | 1/1 pass | `.phase1/runs/23fbc1d4-28da-4ae6-874e-01fe5693d579/summary.json` |
| `rules.godot-callsite-boundary` | 1/1 pass | `.phase1/runs/d4360d92-a18f-45eb-90a8-e43f9510443e/summary.json` |
| `graph.visibility-inventory` | 1/1 pass | `.phase1/runs/b7404b83-6d00-48f5-8124-9156e5919552/summary.json` |
| `graph.allowed-composition` | 1/1 pass | `.phase1/runs/4174daa0-bdba-4d0e-88fa-9865de2edcf7/summary.json` |
| `rules.source-format` | 1/1 pass | `.phase1/runs/acf7739b-e315-454e-b73f-0e74660a4b84/summary.json` |
| `docs.navigation` | 1/1 pass | `.phase1/runs/8d10e0c4-4b93-4b12-9f26-3002712594ab/summary.json` |

The strict whole-solution build completed with zero warnings/errors; log `.phase3/03-04-task2-build.log`. The final Phase 3 input fingerprint is `1b88bd437604972e3e10ec01d2c54aaa3cf167d0c69aef6693bc6fc30779e398`; tool fingerprint is `3c3c16d6cb65767d0c613fecf20cd2c2811a7eea7d4a37a22b4e928ef108b7dd`. The report records HEAD `a046c70264d5bc5bc07fb59edc08c959e76a2a45` plus tested working inputs. After GREEN `d40df6452f95f79a9199f70f7877e87f774eabbe`, both fingerprints were recomputed and match exactly. This establishes committed tested bytes; it does not claim a separate post-commit execution. Actual OS: Windows 10 Enterprise LTSC 2021, release `10.0.19044`, x64; OS fingerprint `4a3c7505cb92e1bb2f7412dd3207a435c55d5c78f2e7e8d11213ecfc2e8768db`.

Actual final native subjects took 2274 ms (two-peer tracer), 5403 ms (four-peer/racing admission), 35913 ms (lifecycle/loss/rehost), 3102 ms (idle polling), 3099 ms (discovery/profile restart) and 2350 ms (strict runner tracer). Each remained within its 60-second subject deadline. The longer aggregate includes exact compiler/permission proofs.

## Implementation available for independent review

- `FoundationDrawObserver` subscribes to completed `FramePostDraw`, emits actual projected actor/facing/visibility/camera/control data and PNG/state checkpoints, and cannot submit commands or activate controls. `FoundationDiagnostics` remains separate from `FoundationAutomation`. Two read-only coordinator properties expose the exact already accepted intent and its authoritative revision; they add no alternate action path.
- `tools/phase3/rendered.cjs` owns the persistent PID/window driver, incremental UTF-8 draw polling, physical callback acknowledgements, external `hrtime` brackets, all origin/observer timing samples and stopped-position checks. Physical scan-code SendInput, targeted window messages and synthetic ParseInputEvent remain separate provenance. Timing includes polling/driver/draw/capture scheduling plus a 1 ms clock allowance.
- `evidence.cjs`, `png.cjs`, `processes.cjs` and the independent manifest enforce exact case/role/image membership, actual distinct PID/profile/actor/binding identity, source/tool/OS/revision/nonce, PNG dimensions/content/CRC/hash, review bindings, real child failure controls and bounded owned cleanup. Counterfeit fixture records earn no production execution credit.
- `scenarios.cjs` provides the grouped ordinary native menu/process procedures; `inherited.cjs` executes the saved inherited selection afresh. The shared Phase 2 desktop lock refuses overlap and never removes a changed/foreign owner. Inherited solo profile storage is now invocation-owned. Checkpoint filenames reject drive-relative/traversal names in both observation paths.
- `03-SCENARIOS.md` and `03-SELECTION.json` save 32 Phase 3 cases, all 16 inherited Phase 2 IDs, 124 affected active Phase 1 IDs, 552 individually explained active omissions and eight still-planned Phase 5 declarations. All 18 acceptance criteria and 20 resolved edges have mappings. G0–G4 ordering, resets, extra-process reasons and cleanup are declared before real launch.

## Retained failures and repairs

- Task 1's first expanded permission run failed because two wrapper methods did not actually invoke native APIs. Their unnecessary permission entries were removed; final exact positives/rejections and repository scans pass. Failed report: `.phase3/runs/win32-x64/48c768ee-596a-4fcd-b42b-48b8e65b3477/summary.json`; passing task gate: `.phase3/runs/win32-x64/bc775e86-994d-4b63-839d-08d768e0d490/summary.json`.
- Task 2 build diagnostics for field ordering and one overlong line were corrected without suppressions. Source inspection also added owned mouse cleanup/hit testing, isolated inherited solo profiles, safe checkpoint tokens, incremental polling and accepted-intent/revision timing binding. Final current reports above follow these repairs.
- Actual RED TAP and classifier records remain in `.phase3/tdd/03-04-task1-red.json` and `.phase3/tdd/03-04-task2-red-valid.json`, both `RED_EVIDENCE_OK`. Task 2's initial nested-error TAP was rejected as malformed; `.phase3/tdd/03-04-task2-red.json` is retained and authorized no implementation. Initial sandbox-only Node/build failures were not treated as valid tests. Earlier passing report identities remain historical and are not substituted for the final current selection.

## Remaining Task 3 and exact commands

1. Independently review the complete Phase 3 diff, including all four plans, production/test/tool sources, threats, driver provenance, cleanup, exact permissions and saved coverage. Record findings in `03-REVIEW.md`, fix them and rerun affected no-window proofs/strict build before any game window.
2. After committing the repaired source, pin `$env:SourceRevisionId = (git rev-parse HEAD).Trim()` in PowerShell (POSIX: `export SourceRevisionId="$(git rev-parse HEAD)"`) before the strict build and independent review. Keep this immutable reviewed-source stamp through all later metadata commits/builds; the SDK otherwise embeds each newer HEAD into the DLL and changes its exact tool hash. Obtain the review identity with `node -e "console.log(JSON.stringify(require('./tools/phase3/check.cjs').createIdentity(process.cwd()),null,2))"`. Author `03-REVIEW.json` with `status: complete`, exact `input`, `tool`, `revision`, a named `reviewedBy`, finding dispositions and `report: {path, sha256}` bound to the actual Markdown review. Critical/high findings must be resolved. Commit the Markdown review and gate before execution. The gate remains absent until independent review occurs; see [exact finalization flow](../../../docs/workflow/lan-foundation-checks.md).
3. Retaining that same `SourceRevisionId` pin, run `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`, using the isolated environment, then `node tools/phase3/check.cjs --all`. The runner propagates the pin to inherited builds while recording the true execution HEAD in `revision`; `buildRevision` preserves the reviewed source stamp. The latter executes G0, selected inherited checks/G1, G2, G3 and destructive G4. Do not launch an extra inherited solo run unnecessarily. A focused repair uses `node tools/phase3/check.cjs --failed REPORT`; repairs still require the subsequent fresh whole selected-set pass.
4. First establish actual native event/hold/focus driver feasibility. Collect all required per-role PNGs and 48 start/reversal/stop/focus-stop rows plus 16 stopped comparisons. Inspect every required actual image and record hash-bound appearance outcomes in `03-VISUAL-REVIEW.md`; no state-only or unseen-image review credit.
5. Author `03-EVIDENCE.json` with the final exact report/selection/history/visual-review references and qualified inherited image reviews, then run `node tools/phase3/check.cjs --verify-completion .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` with the original `SourceRevisionId` pin. Only after Task 3 may the final `03-04-SUMMARY.md` be written. Commit permitted closure metadata and validate again from that checkout. If rebuilding, first restore the final report's original `buildRevision` pin; exact binary/tool equality is still required. The parent updates shared STATE/ROADMAP/REQUIREMENTS and the existing PR; no merge.

Actual foreground physical input, sustained simultaneous background window messages, synthetic simultaneous-control fallback and conservative rendered responsiveness remain unproved. These are explicit Task 3 feasibility/evidence risks; an unmet physical or timing obligation is a blocker, never a synthetic pass. No actual image review, hosted/Linux run, VM, physical mixed-computer LAN, complete A-38/A-45 campaign, export, hardware or endurance acceptance is claimed.

## Self-check

Both completed task GREEN commits and both RED commits are ancestors of the current branch. Required implementation/scenario/selection files exist, all final reports above are present, the committed source/tool fingerprints match the final Phase 3 report, generated metadata is unchanged and the working tree was clean after Task 2 GREEN. Shared phase state and PR publishing were left to the parent.

## Task 3 prelaunch blocker — 2026-11-15

At actual metadata HEAD `b5c603f009e5adb3abe5fc6e64621a879b27d6dc`, the mandatory strict rebuild used the explicit isolated SDK environment and `SourceRevisionId=e25a500d12006e256d84b8b241d0cee9c507f278`. It passed with zero warnings/errors, but the subsequent exact identity check failed before `--all` began. No selected-set run or game window occurred; no source, review or gate was changed.

The current source input exactly matches the gate (`a1f23903316f05961192ad385d780360f3e13382d255313a8830082ec5163089`) and the build pin matches. The actual tool is `daa844aa701ba57185b565d2b0e189a1f1131ef724bbf6b6d6cc7542300e5a2e`, differing from the required reviewed `48a74c363799e4cff1c8c87906b220dbc59649953201b436b8189d52bd73c3aa`. The actual proof DLL SHA-256 is `7d32e34a72cbceb407b68893fb48b6dbfc2928c6936aaae6907386c36ee9cd04`; portable PDB SHA-256 is `82be46f826d72c69950ff248afd25c8a87608ef051a73bf3af9cc5839a7515b0`.

Concrete SDK cause: `Microsoft.Build.Tasks.Git.targets` invokes `LocateRepository`, always emits its `Roots` into `SourceRoot`, and conditionally sets the separate `SourceRevisionId` property only when empty. Evaluation of only `InitializeSourceControlInformation` confirms property `SourceRevisionId=e25a500...` while the repository `SourceRoot.RevisionId=b5c603f...`. The GitHub SourceLink initializer consumes that independent item metadata; `_GenerateSourceLinkFile` passes the generated SourceLink document to the compiler. Actual `AssemblyInfo.cs` remains correctly stamped `0.1.0+e25a500...`, but both test/Core `sourcelink.json` files point to GitHub `b5c603f...`. The proof/Core implementation DLLs and PDBs were rewritten; their reference assembly bytes remain unchanged. This is an additional real-repository SourceLink/PDB input, beyond the informational-version pin covered by the earlier isolated fixture.

Immutable diagnostic artifacts: `.phase3/task3-metadata-rebuild.log`, `.phase3/task3-prelaunch-mismatch.json`, `.phase3/task3-scm-metadata.json` and consolidated `.phase3/task3-build-drift-diagnosis.json`. The latter records actual generated contents, all relevant hashes, modification times and evaluated SourceRoot metadata. No repeated build or attempt to normalize/accept the new hash occurred.

Parent continuation: correct the phase-scoped reproducibility mechanism for `SourceRoot.RevisionId`/SourceLink as well as the informational version, preserving exact reviewed DLL bytes where possible. Extend the isolated real SDK fixture with an actual GitHub remote and SourceLink generation to reproduce this gap. Any source/tool change needs its own committed/reviewed pin and gate before the mandatory rebuild/fresh full selected set. Do not rewrite the existing gate tool hash to declare this mismatching build accepted. Task 3 and the final SUMMARY remain incomplete; shared state and PR actions remain with the parent.
