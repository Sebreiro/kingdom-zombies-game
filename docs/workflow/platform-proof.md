# Phase 1 platform baseline

The [setup](setup.md) and [verification](verification.md) pages define the ordinary commands. This record distinguishes the complete functional Windows baseline owned by 01-16 from the representative genuine Linux acceptance owned by 01-17. Preparation and exploratory setup/build feedback carry no Linux acceptance credit. Coverage and exhaustive certification remain pending in the [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md).

## Same managed payloads and native platforms

Both platforms use the exact committed five-project locks and the four canonical managed packages authenticated by the official Windows Godot archive. Actual ZIP inspection found 28 inner package entries on each platform; SDK, generator, managed DLL and content entries match. Only package relationship/core metadata differs, so raw Linux archive nupkgs have different hashes and are not substituted. Bootstrap verifies the native OS archive separately and launches its actual native binary. Exact package/generated-source hashes and exemptions remain enforced.

The pinned SDK's assembly-attribute writer emits CRLF on Windows and LF on Linux. Linux preflight rejected the unrecognized exact source with SCR0007 at `(4,121)`. The existing four-source layout registry now additionally records the two observed Linux SDK source hashes and actual Linux producer-target hash; the analyzer permits only those complete identities at the same suffixes and line 4. Origin lookup follows the executing runtime's SDK root. Existing controls exercise both exact newline variants, changed bytes, wrong paths and adjacent source. No generic source normalization, extra member suppression or change to the 170 Godot span permissions is involved.

The actual testing-platform `SelfRegisteredExtensions.cs` has one platform-specific newline after its MSBuild hook call. Both existing exact-member registries retain the Windows identity and additionally record observed Linux SHA-256 `4e0ebb8f0acc3074a07b86d417bfffe8168857353c4c2eec668c3f24f0c0bb2e`, authenticated against the same pinned package and exact member. Other template CRLF bytes remain unchanged. Source validation chooses the actual platform's complete raw hash; it never normalizes a subject. Existing origin and adjacent/collision controls cover this adaptation. The combined exploratory Linux inventory checks 560 source/text/package/producer identities with no mismatches, including all 170 unchanged native generated spans. These setup diagnostics carry no 01-17 acceptance credit.

## Final clean Windows functional baseline

On 2026-11-14, the donor-independent clean checkout `D:/Work/Games/KingdomZombies/.phase1/temp/16-windows-f4658bb` passed locked whole-solution restore, strict build with zero warnings/errors, actual native bootstrap and the complete declared Phase 1 selection. It began without old `bin`, `obj`, `.godot` or proof outputs. Its only seeded dependencies were 41 existing lock-verified NuGet cache payloads and the authenticated official Windows engine archive/checksum file. No donor folder, Downloads path, Linux VM file or private key was copied or required.

| Evidence | Actual result |
| --- | --- |
| `node tools/phase1/bootstrap.cjs --probe engine` | Pass; raw compatibility nonce `59a72fa8-cb62-43ba-aae8-c378f78bf813`, fresh construction/ready/exit and hosted runtime `10.0.15` |
| `dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false` | All five projects restored; committed locks unchanged |
| `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` | Exit 0; zero warnings and errors |
| `node tools/phase1/check.cjs --phase 1` | Exit 0; **670/670 ACCEPTED**, zero failed, blocked, missing or skipped selected obligations |

The [unaltered canonical summary](../../.phase1/temp/16-windows-f4658bb/.phase1/runs/030dd8f9-c5e7-4d52-9b03-505948e8755c/summary.json) retains every case's fresh test report, subject evidence, actual command, exit and identity. Completed groups: runner 36, pins 27, register 46, rules 447, docs 36, graph 26, engine 19 and state 33. All 670 case processes exited zero; no test was skipped. The runner verified unchanged frozen inputs through publication.

| Identity | Actual value |
| --- | --- |
| Tested revision | `f4658bb2872b6578b592a18475e58af04cd064d4` |
| Invocation | `030dd8f9-c5e7-4d52-9b03-505948e8755c` |
| Input | `23d1060bfbdb6b082624631c1e32f1cd31a9fe29287745072e364d9f56020071` |
| Tool | `f3f2c460d76528ac47f5c5e257f549b5585e76792ab7913d4b0168be4ffe661b` |
| Actual platform | `win32-x64-10.0.19044` |
| Actual execution interval | `2026-11-14T14:49:32.789Z`–`2026-11-14T15:39:56.667Z` |
| Observed pins | Node `24.22.1`; SDK `10.0.404`; compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)`; Godot `4.7.2.stable.mono.official.ed1daf0bf`; runtimes `8.0.16` and `10.0.15` |

An earlier invocation on candidate `18fec9f` was stopped shortly after start when exploratory Linux generation revealed the final testing-platform source variant. It has no acceptance credit and was not combined with this report. The complete final invocation above is the only completed full run for the stabilized implementation. Later prose/progress updates receive separate D-11 documentation evidence with their own input identity; this report certifies the frozen source it actually tested.

## Deferred Phase 5 identities

These eight declarations remain pending with `acceptancePhase: 5` and owner `Phase 5`. They are outside the 670 selected denominator and receive no pass or skip result.

| Exact ID | Required later certification |
| --- | --- |
| `ac-02.complete` | Exhaustive clean two-platform execution and reconciliation |
| `ac-06.complete` | Complete bindings including actual measured coverage |
| `ac-07.complete` | Complete enforcement certification including actual measured coverage |
| `ac-09.complete` | All numeric/report thresholds including inclusive 90%/80% coverage |
| `R-49.positive` | Actual analyzer coverage meets both unchanged targets |
| `R-49.negative` | Actual analyzer coverage rejects insufficient/invalid evidence |
| `R-82.positive` | Actual production/native-hosted managed coverage meets both targets |
| `R-82.negative` | Actual production coverage rejects insufficient/invalid evidence |

## Prepared same-source Linux checkout

The genuine Ubuntu 24.04.5 LTS / kernel `6.8.0-147-generic` / x86_64 VM retains clean checkout `/home/phase1/phase1-baseline-16` at the same tested revision `f4658bb2872b6578b592a18475e58af04cd064d4`. Its independently computed 200-file input fingerprint matches the Windows input above. The source bundle is `/home/phase1/phase1-tools/downloads/baseline-16.bundle` (SHA-256 `246720cdcb7cdffad00e6bb3ec4bb497e9f22fede3914c7a0e1312ee90f95f82`). Pinned tools, both authenticated engine archives, the exact four-package feed and 41 lock-verified cache payloads are ready. The source has no old build outputs or acceptance reports. Access and ordinary commands are in the environment record below. This is preparation only; actual clean Linux acceptance remains pending for 01-17.

## Predeclared Linux acceptance selection

This selection is declared before 01-17 execution. Run each exact ID with `node tools/phase1/check.cjs --case <ID>` after the clean native bootstrap, locked whole-solution restore and strict build. Each invocation must publish its own fresh canonical report with one selected and accepted obligation, actual exit zero and no missing, failed, blocked or skipped result. Preserve the common vectors unchanged. No exhaustive Linux run or new wrapper is required.

| Exact case ID | Representative obligation |
| --- | --- |
| `R-38.positive` | Compliant named argument analyzer subject |
| `R-38.negative` | Positional argument rejected with exact SCR0001 evidence |
| `ac-12.complete` | Allowed declared/evaluated five-project graph and compiled types |
| `graph.allowed-and-direct` | Real forbidden Core-to-engine reference rejected |
| `docs.portable` | Actual portable documentation contract and missing-entry rejection |
| `docs.projects` | Five module pages match real evaluated project references |
| `state.inputs` | Controlled time/random inputs and preserved identities |
| `state.identities` | Exact ordinal Unicode identities remain distinct |
| `state.golden-vectors` | Existing unchanged independently observed common seeds/vectors |
| `state.numeric-and-random` | Restored random position continues the exact stream |
| `state.snapshot` | Detached state serializes and round trips exactly |
| `state.restore` | Accepted snapshot replaces one complete root |
| `engine.lifecycle` | Actual construction, callbacks, process, signals, teardown, worker/main-thread and old-generation fences |
| `engine.exports` | Real resource/export/binding values and required-null failure before use |
| `engine.generated-origin` | All 170 actual generated spans and exact producer payload |
| `engine.handwritten-partial` | Real handwritten companion retains exact SCR0001 boundary |

The Linux environment, access, prepared source and pinned prerequisite details are recorded in [01-ENVIRONMENT.md](../../.planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md). The formal Linux results and separate qualitative review remain pending for 01-17.
