# Phase 1 platform baseline

**Current final-acceptance requirement (creator instruction, 2026-11-14):** Complete the current 676-case Phase 1 selection on Windows and the local Linux VM for the same repaired source. Earlier f4658bb runs below are retained historical evidence, including nine genuine TCG timeouts; they are not final acceptance for the four repairs through 5c52a20. Hardware acceleration setup is requested. Current complete platform reports and separate human judgment remain Pending. See the [repair report](../../.planning/phases/01-code-rules-and-documentation/01-REVIEW-FIX.md) and [verification reference](verification.md).

The [setup](setup.md) and [verification](verification.md) pages define the ordinary commands. This record distinguishes the complete functional Windows baseline owned by 01-16 from actual representative Linux execution owned by 01-17. The Linux baseline remains blocked by real canonical case timeouts on the emulated machine. Preparation, exploratory feedback and separate longer diagnostics carry no acceptance credit. Coverage and exhaustive certification remain pending in the [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md).

## Same managed payloads and native platforms

Both platforms use the exact committed five-project locks and the four canonical managed packages authenticated by the official Windows Godot archive. Actual ZIP inspection found 28 inner package entries on each platform; SDK, generator, managed DLL and content entries match. Only package relationship/core metadata differs, so raw Linux archive nupkgs have different hashes and are not substituted. Bootstrap verifies the native OS archive separately and launches its actual native binary. Exact package/generated-source hashes and exemptions remain enforced.

The pinned SDK's assembly-attribute writer emits CRLF on Windows and LF on Linux. Linux preflight rejected the unrecognized exact source with SCR0007 at `(4,121)`. The existing four-source layout registry now additionally records the two observed Linux SDK source hashes and actual Linux producer-target hash; the analyzer permits only those complete identities at the same suffixes and line 4. Origin lookup follows the executing runtime's SDK root. Existing controls exercise both exact newline variants, changed bytes, wrong paths and adjacent source. No generic source normalization, extra member suppression or change to the 170 Godot span permissions is involved.

The actual testing-platform `SelfRegisteredExtensions.cs` has one platform-specific newline after its MSBuild hook call. Both existing exact-member registries retain the Windows identity and additionally record observed Linux SHA-256 `4e0ebb8f0acc3074a07b86d417bfffe8168857353c4c2eec668c3f24f0c0bb2e`, authenticated against the same pinned package and exact member. Other template CRLF bytes remain unchanged. Source validation chooses the actual platform's complete raw hash; it never normalizes a subject. Existing origin and adjacent/collision controls cover this adaptation. The combined exploratory Linux inventory checks 560 source/text/package/producer identities with no mismatches, including all 170 unchanged native generated spans. These setup diagnostics carry no 01-17 acceptance credit.

## Historical pre-review Windows functional baseline

On 2026-11-14, the donor-independent clean checkout `D:/Work/Games/KingdomZombies/.phase1/temp/16-windows-f4658bb` passed locked whole-solution restore, strict build with zero warnings/errors, actual native bootstrap and the complete declared Phase 1 selection. It began without old `bin`, `obj`, `.godot` or proof outputs. Its only seeded dependencies were 41 existing lock-verified NuGet cache payloads and the authenticated official Windows engine archive/checksum file. No donor folder, Downloads path, Linux VM file or private key was copied or required.

| Evidence | Actual result |
| --- | --- |
| `node tools/phase1/bootstrap.cjs --probe engine` | Pass; raw compatibility nonce `59a72fa8-cb62-43ba-aae8-c378f78bf813`, fresh construction/ready/exit and hosted runtime `10.0.15` |
| `dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false` | All five projects restored; committed locks unchanged |
| `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` | Exit 0; zero warnings and errors |
| `node tools/phase1/check.cjs --phase 1` | Exit 0; **670/670 ACCEPTED**, zero failed, blocked, missing or skipped selected obligations |

The [plan summary](../../.planning/phases/01-code-rules-and-documentation/01-16-SUMMARY.md) records the unaltered canonical report at `.phase1/temp/16-windows-f4658bb/.phase1/runs/030dd8f9-c5e7-4d52-9b03-505948e8755c/summary.json`. That report retains every case's fresh test report, subject evidence, actual command, exit and identity. Completed groups: runner 36, pins 27, register 46, rules 447, docs 36, graph 26, engine 19 and state 33. All 670 case processes exited zero; no test was skipped. The runner verified unchanged frozen inputs through publication.

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

## Clean same-source Linux execution

On 2026-11-14, 01-17 verified the existing genuine Ubuntu 24.04.5 LTS / kernel `6.8.0-147-generic` / x86_64 VM and clean checkout `/home/phase1/phase1-baseline-16` at the same tested revision `f4658bb2872b6578b592a18475e58af04cd064d4`. Git status was clean, and source/tests/tools had no old `bin`, `obj` or `.godot` build outputs. Its independently computed 200-file input fingerprint matches the Windows input above. The source bundle is `/home/phase1/phase1-tools/downloads/baseline-16.bundle` (SHA-256 `246720cdcb7cdffad00e6bb3ec4bb497e9f22fede3914c7a0e1312ee90f95f82`). Only the declared 41 lock-verified package payloads, authenticated official archives/checksum and canonical managed feed were seeded; no old build outputs were copied.

The actual environment is QEMU TCG with four virtual CPUs and 4096 MB RAM. Observed tools are Node `24.22.1`, SDK `10.0.404`, compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)`, application runtime `10.0.15`, editor runtime `8.0.16` and native Linux Godot `4.7.2.stable.mono.official.ed1daf0bf`. All commands used the existing guest environment and checkout-local CLI/package/cache/temp paths from the [environment record](../../.planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md). Linux access works; execution capacity within unchanged case deadlines is the unresolved prerequisite.

| Actual command | Result and retained evidence |
| --- | --- |
| `PHASE1_IGNORE_DOWNLOADS=1 node tools/phase1/bootstrap.cjs --probe engine` (cold attempt) | Exit 1, `CHILD_TIMEOUT` at the existing 60-second game-build limit; nonce `4b95c9b1-ad41-485e-98c9-3ee05ed5f778`. Both restore steps passed. `/home/phase1/17-bootstrap.log` and `.exit` retain the failure. |
| `timeout 900s dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false` | Exit 0; all five projects restored with unchanged locks. `/home/phase1/17-restore.log` and `.exit`. |
| `timeout 900s dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q -m:1` | Exit 0; zero warnings/errors, `00:05:10.06`. Single-node scheduling changes no strict property. `/home/phase1/17-build.log`, `.exit`, `.start` and `.end`. |
| Unchanged native bootstrap command after ordinary build | Exit 0; raw compatibility nonce `0d50ef94-b2e3-4e38-b583-e9345283a7df`, game build `00:00:27.52`, zero warnings/errors. Actual native engine exit 0, runtime `10.0.15`, Linux/headless/`gl_compatibility`. `/home/phase1/17-bootstrap2.log` and `.exit`. |

Both bootstrap invocation directories remain under the frozen checkout's `.phase1/runs/`. Inspection of the successful native `engine.json` finds fresh construction/ready/exit records and 21 actual surface/cycle records: construction, callbacks, process, signals, teardown, resources, exports, nullable validation and binding in two cycles, plus late-work rejection, prohibited worker rejection and main-thread result delivery. The cycles have distinct actual instances `27279754708` and `27581744596` and the single captured managed thread `1`. These are genuine observed native assertions, separately identified from the canonical case outcomes below. They do not turn a timed-out lifecycle/export selector into a pass.

Three bounded environment-only trials of the failed `R-38.positive` selector also timed out at its unchanged 45-second child limit: software intrinsics disabled (`c56c47ad-da0c-4cf3-8813-d20c0d169852`), dynamic PGO disabled (`a6577982-cd19-4b9c-b5cd-ef5dad3d5674`) and single-processor runtime scheduling (`d445c64f-764d-449c-9bf7-ae4fb6fc3c6b`). The first worsened setup speed; all settings were process-local and the original environment was restored. Microsoft's [SIMD controls](https://learn.microsoft.com/en-us/dotnet/standard/simd), [compilation settings](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/compilation) and [processor-count override](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/environment-processorcount-on-windows) document these controls. Source, pins, rule definitions, test selection, assertions and canonical deadlines never changed.

A separate direct diagnostic used an explicit 180-second outer bound and 120-second test limit, receiving **no acceptance credit**. It ran the same named-argument proof under the normal environment, nonce `64865a1c-850f-4f4c-a18a-2b9bf04b5056`, and exited 0 with one passed, zero failed/skipped tests. The selected body took 39.724 seconds; the platform session took 49.08 seconds, exceeding the canonical outer 45 seconds. The [standard platform trace](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-config) showed normal discovery followed by actual evaluated-reference/Roslyn work, with telemetry disabled. `/home/phase1/17-direct-default.*`, `17-direct-diagnostics/` and `.phase1/temp/17-direct-default/` retain its real records. This explains the observed capacity gap without waiving it or claiming a canonical pass.

## Historical predeclared Linux representative selection

This selection was declared before the earlier 01-17 representative execution and is retained to explain its actual outcomes. Each exact ID used `node tools/phase1/check.cjs --case <ID>` after native bootstrap, locked whole-solution restore and strict build. That earlier selection is superseded for final acceptance: run the complete current `node tools/phase1/check.cjs --phase 1` scope on both Windows and the local Linux VM, with the common vectors and deadlines unchanged.

| Exact case ID | Representative obligation | Actual result / exit | Invocation nonce |
| --- | --- | --- | --- |
| `R-38.positive` | Compliant named argument subject | CHILD_TIMEOUT / 1 | `e41c6f73-6238-4467-89cb-c3e886c047a3` |
| `R-38.negative` | Exact positional SCR0001 rejection | CHILD_TIMEOUT / 1 | `6fbbb27c-e605-42b9-85a8-86f1359e980a` |
| `ac-12.complete` | Allowed evaluated/compiled five-project graph | CHILD_TIMEOUT / 1 | `5a047ea9-f0e3-4651-8a26-49c0dd2f1026` |
| `graph.allowed-and-direct` | Actual forbidden Core-to-engine reference | CHILD_TIMEOUT / 1 | `57d0b5bd-0c5e-4d32-9d7f-9d3dfe5c8276` |
| `docs.portable` | Portable contract and missing-entry rejection | ACCEPTED 1/1 / 0 | `06fecf89-9880-47d0-a47e-c809869921e1` |
| `docs.projects` | Five pages match evaluated references | CHILD_TIMEOUT / 1 | `b7c112e0-f5ce-4195-a60c-7bc2749bfbdb` |
| `state.inputs` | Controlled time/random inputs and identities | ACCEPTED 1/1 / 0 | `fe2c487d-87e0-4a25-9519-681c8e5d9ff4` |
| `state.identities` | Exact ordinal Unicode identities | ACCEPTED 1/1 / 0 | `96bd0110-fa21-41ab-befd-75feb5088dbf` |
| `state.golden-vectors` | Unchanged common seeds/vectors | ACCEPTED 1/1 / 0 | `e6ddffc6-107d-477f-8914-16d491f03e9c` |
| `state.numeric-and-random` | Restored exact random continuation | ACCEPTED 1/1 / 0 | `855b3b43-c361-49b0-8bb0-78d41c87a360` |
| `state.snapshot` | Detached exact snapshot round trip | ACCEPTED 1/1 / 0 | `741f8a9f-a96f-4ca7-9950-38df7dbbb681` |
| `state.restore` | One complete accepted root replacement | ACCEPTED 1/1 / 0 | `2c61fd47-7edb-4b31-968a-8f45b2482543` |
| `engine.lifecycle` | Native cycles, callbacks, thread/lifetime fences | CHILD_TIMEOUT / 1 | `39a8ca9a-63e2-416a-84f3-f8935eeebdb1` |
| `engine.exports` | Real resource/export/binding and required-null controls | CHILD_TIMEOUT / 1 | `ae667544-69bb-44ca-864d-06386ca7afb2` |
| `engine.generated-origin` | All 170 actual spans and exact producer | CHILD_TIMEOUT / 1 | `6154f791-a0a1-47ef-a99c-7572b61fd572` |
| `engine.handwritten-partial` | Exact handwritten companion SCR0001 boundary | CHILD_TIMEOUT / 1 | `43327fb4-abd2-48be-8410-79318b474e2e` |

All sixteen original selectors executed sequentially once under the original environment, across `2026-11-14T16:07:13Z`–`2026-11-14T16:43:39Z`, with a clean between-case pause for the bounded diagnostics. Each selected exactly one obligation. **Seven were accepted and nine failed with CHILD_TIMEOUT**; no selector was skipped, unrun or credited from another invocation. All setup command exits were zero; each failed test command reached its unchanged 45-second outer deadline and was terminated with SIGKILL. No functional assertion failure or source change was observed, but these timeouts block the required Linux baseline.

All sixteen reports record revision `f4658bb2872b6578b592a18475e58af04cd064d4`, input `23d1060bfbdb6b082624631c1e32f1cd31a9fe29287745072e364d9f56020071`, tool `7866ee43e38ed4100250b5f2fc452e16be5d35fda7bd5ee33d430a946156e206` and actual platform `linux-x64-6.8.0-147-generic`. Their original canonical paths are `/home/phase1/phase1-baseline-16/.phase1/runs/<nonce>/summary.json`. The seven accepted records each reconcile one passed test, zero skipped tests and matching hashes for all subject evidence and test reports. The nine failures have zero accepted results. Frozen Git status remains clean and all five locks unchanged.

The exact retained archive is `/home/phase1/17-evidence.tar.gz`, copied to `.phase1/temp/17-linux-evidence/17-evidence.tar.gz` on Windows and extracted beside it. SHA-256 on both copies: `662ec21365f10271c1cfc5c06a984c11eac7422876102c663794cbcd218be037`. It contains original canonical reports, actual command/exits/logs, setup receipts and explicitly separate diagnostics; no key, old build outputs or donor content is included. The case exit receipts are `17-case-<ID>.exit`; the between-case pause receipt preserves the terminal child exit before its loop parent stopped. The VM remains available for root closeout and further environment assessment.

The [validation record](../../.planning/phases/01-code-rules-and-documentation/01-VALIDATION.md) retains the resulting acceptance gaps. A sufficiently capable genuine Linux environment must complete the nine failed required selectors under the existing rules/deadlines before Phase 1 can close. The separate [human review](../../.planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md) and RULE-01–RULE-07 remain Pending. No coverage, exhaustive certification, gameplay/export/LAN/rendered/endurance result is claimed.
