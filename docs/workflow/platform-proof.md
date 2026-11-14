# Phase 1 platform baseline

**Latest creator authority (2026-11-14):** The [one-time Phase 1 closure exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) requests important actual Linux checks without a Windows restart, retaining the complete Windows **676/676** baseline. The authorized composed evidence is now complete: strict Linux setup/native bootstrap, five unchanged case passes, the corrected branch case passing on both OSes, and a separately passing direct graph-mechanism smoke. The original graph case remains **failed/unaccepted**, full Linux execution is unrun, and fourteen individual human outcomes remain unreviewed follow-ups. No new full Windows/Linux run or component-human pass is claimed. Later phases retain the complete development-OS/other-OS-VM rule.

## Focused amendment and direct graph-mechanism proof

Actual SDK evaluation from the original attempt established `GODOT_LINUXBSD`, target `linuxbsd`, with `DEBUG` and `TOOLS`. Narrow fix `70918fdb77cff309527b407418083e52db89378e` adds only that observed spelling to `ConsumerPolicyTests.cs`'s platform selection; its active/inverse and unresolved-subject assertions, production policy, helpers, manifests, pins and deadlines are unchanged. The clean guest checkout advanced through a verified tracked-source Git bundle (SHA-256 `0dc445999b42dc5a07c0539b5d01d9080e100ce5b75a552455e42400cd013ca8`), with no source authored in the guest. The new 203-file input is `2e16dd414d42f53cba37707e629c9110fe64164ddb8e96636c878c014419c15f`.

| Targeted verification | Actual result / identity |
| --- | --- |
| Windows canonical `--case engine.consumer-branches` | **1/1 ACCEPTED**, exit 0; nonce `66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9`, tool `301718a41128e6df235378cea6b54c123baef1343a283ade9c21cbfaf870269a` |
| Linux strict incremental test-project build | Exit 0, zero warnings/errors, `00:04:07.43`; original strict flags and `-m:1` |
| Linux direct corrected branch case, 8m MTP / 10m outer | **1 passed / 0 failed / 0 skipped**, exit 0, `CONSUMER_BRANCHES_CHECKED`; nonce `b4ccec5f-255f-466f-bea2-f807b260ccf8`, tool `b8578556d768b15bb41bfc8a2b5b67e5b63a1be7b0dc2b278edcf658217b6f75` |

Both changed-case runs have the same actual input above. The Windows report retains its genuine precommit workspace revision `266a6d87c72477d77dee8beaa8283ae0bce4d441`; postcommit inspection confirmed its fingerprint equals the committed `70918fd` snapshot. Its original report is `.phase1/runs/66a74d57-ecc3-4d79-b1b8-0f27bf41e6d9/summary.json`. The Linux amendment ran `20:49:51.631Z`–`20:55:53.044Z`, including build/tool observation. Its source is `70918fd`, platform `linux-x64-6.8.0-147-generic`, and exact proof location is `actual evaluated DEBUG/TOOLS/GODOT_LINUXBSD; inverse branches and unresolved subject`. Original report, subject evidence, prerequisite/build commands and fresh executable/compiler/test-assembly identities are in `.phase1/amendment/b4ccec5f-255f-466f-bea2-f807b260ccf8/` in the guest. Receipt SHA-256: `95d57c2c9f183e530382dcf8e64f255f32e483423e16473ae502f5a45f4e6733`. The already-verified native baseline is explicitly reused because no production/native code changed; no new bootstrap is invented after the test-only correction.

The separately labeled **direct graph-mechanism smoke**, nonce `6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365`, ran `20:57:02.986Z`–`21:05:58.060Z`. It verified **72** copied fixture-source hashes against the candidate, rejected unexpected source files and checked the clean subject against the actual compiled `BuildFixture.CleanSource` constant. No build output was copied. A one-off ignored reflection host, compiled with the pinned SDK/reference pack without a new project/package, invoked the **existing compiled** `ProjectEvaluator.Validate` and existing `GraphTests.IsDirectViolation`; it did not rewrite the validator.

The compliant fixture built strictly with zero warnings/errors in `00:03:27.44`, then the existing validator returned **zero errors**. The exact `AddDirectEngineEdge` XML delta removed game-to-Core and added Core-to-game. Its original-sequence force-evaluate restore, locked restore and single changed-Core strict build all exited zero; the changed build had zero warnings/errors in `00:01:56.88`. The existing validator then returned exactly:

```text
GRAPH_FORBIDDEN: src/core/KingdomZombies.Core.csproj -> src/godot-game/KingdomZombies.csproj defined by src/core/KingdomZombies.Core.csproj
```

The existing predicate also returned true. All eight direct-proof commands exited zero without timeout/signal. Ordinary setup/build bounds were 1200 seconds and validator-host bounds 600 seconds; all internal source query/compiler limits remain unchanged. Validator assembly SHA-256 is `de05b6a138780912c7c103c69aea3f53442088215d54db48fbc6656f455de83f`, matching the amended Linux test assembly. The direct receipt, actual mutations, strict build logs, source inventory, existing evaluator's raw query logs, host source/binary/compiler identities and returned results are under `.phase1/direct-graph/6eebf5c4-80f6-4fb0-9bbb-da7cbcfc6365/`; receipt SHA-256 `3c512d37162931e4f14666b130eaa21534046f592f256e60674a6e26899dfaaf`. Its explicit `originalCaseCredited: false` preserves the original `graph.allowed-and-direct` failure as follow-up.

New original artifacts are archived at `/home/phase1/17-focused-amendment-evidence.tar.gz` and copied/extracted under `.phase1/temp/17-linux-smoke/amendment-evidence/` on Windows. Matching archive SHA-256: `ceeeb9e22e5dfb83d4752f178bd7f8eb385028a3b2d00b1115156adc6506c765`. `.phase1/temp/17-linux-smoke/amendment-inspection.json` independently verifies the corrected exact report/proof/evidence, all command-log hashes and exits, 72 source hashes, actual XML before/after bytes, both real compiled graph results, and unchanged source input. Compared with the complete Windows baseline, only `ConsumerPolicyTests.cs` changes C# bytes; the other five input differences are explicit phase policy/proof prose. All source/pin/lock/manifests outside that one test literal remain unchanged.

After evidence transfer and inspection, only the positively identified owned TCG guest received `sync; sudo -n poweroff` at `2026-11-14T21:08:59Z`, actual SSH exit 0. At `21:09:10.1604557Z`, QEMU 31028 and launcher 28040 had exited, owned SSH/QMP listeners were absent, and the preserved disk allowed an exclusive read-open. Original/new process receipts are retained; the ignored live record preserves executable/start/requested TCG and last PIDs while marking confirmed shutdown and clearing live PIDs. The host still reports no active hypervisor; **Windows was not restarted**. Disk/base image/firmware/caches/tools/keys and all historical evidence remain preserved. The [environment record](../../.planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md) names runtime details. Root owns formal Phase 1 closure and follow-up records under the creator instruction.

## Original focused Linux smoke attempt — preserved failures

The creator's exception predeclared seven existing cases, unchanged exact method filters/assertions, ordinary bounded strict setup, and direct software-emulated smoke execution with **8-minute MTP / 10-minute outer** case limits. The canonical runner's 45-second case limit and every internal source deadline remain unchanged. This is focused Linux smoke evidence, not canonical complete `--phase 1` acceptance.

The existing owned Ubuntu 24.04.5 LTS / kernel `6.8.0-147-generic` / x86_64 guest was started explicitly with TCG after the creator declined a host restart. The clean source remained `59f032cf23340058eec91f12c5d240f585dfabdc`, matching the final Windows source and 203-file input `fe2bf549f20329633fd62cfeab796ca8fb2c5ce02da432346e8ee4c314873426`. Only authenticated official archive contents were extracted into the local tools/feed; four canonical managed payload hashes matched. No old build output was copied.

| Actual setup | Outcome |
| --- | --- |
| Locked five-project restore, ordinary 1200-second outer bound | Exit 0; all five projects restored |
| Strict whole-solution build with original flags and `-m:1`, 1200-second outer bound | Exit 0; **zero warnings/errors**, `00:06:07.66` |
| Unchanged `node tools/phase1/bootstrap.cjs --probe engine` after ordinary build | Exit 0; nonce `c9059bcc-b8fa-499d-82e3-6612c20dfbe7`; original 60-second child limits unchanged |

Fresh native evidence contains construction/ready/exit and 21 surface/cycle records across distinct instances `27279754708` / `27581744596` on captured managed thread `1`: callbacks, process, signals, teardown, resources, exports, null validation, binding, late-work fencing, prohibited-worker rejection and main-thread delivery were inspected. Observed tools match Node `24.22.1`, SDK `10.0.404`, compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)`, Godot `4.7.2.stable.mono.official.ed1daf0bf`, and runtimes `8.0.16` / `10.0.15`. Actual executable/compiler/test-assembly hashes are retained in `tools.json` and the original receipt.

| Predeclared case | Actual outcome | Process duration |
| --- | --- | --- |
| `R-38.positive` | PASS; exact 1 passed / 0 failed / 0 skipped, `ANALYZER_ACCEPTED`, exit 0 | 55.524s |
| `R-38.negative` | PASS; exact 1 passed / 0 failed / 0 skipped, `SCR0001`, exit 0 | 53.774s |
| `graph.allowed-and-direct` | FAIL; exact 1 failed / 0 passed / 0 skipped, exit 2 | 169.153s |
| `state.snapshot` | PASS; exact 1 passed / 0 failed / 0 skipped, `STRICT_SNAPSHOT_ROUNDTRIP`, exit 0 | 15.160s |
| `state.restore` | PASS; exact 1 passed / 0 failed / 0 skipped, `ATOMIC_ACCEPTED_ROOT`, exit 0 | 16.440s |
| `engine.consumer-branches` | FAIL; exact 1 failed / 0 passed / 0 skipped, exit 2 | 44.025s |
| `rules.suppression-conditional` | PASS; exact 1 passed / 0 failed / 0 skipped, `CONDITIONAL_SUPPRESSION_REJECTED`, exit 0 | 50.744s |

The graph failure is `System.TimeoutException: Isolated compiler exceeded 60 seconds` in `BuildFixture.Run` line 184, through `GraphFixture` line 19. Its fresh clean fixture build failed the unchanged infrastructure prerequisite before the forbidden-edge subject. No deadline, shared-compiler setting or assertion was relaxed; no fixture build outputs were seeded. The branch regression failed with `Sequence contains no matching element` at `ConsumerPolicyTests.cs:23`: its `Single` assumes `GODOT_WINDOWS` or `GODOT_LINUX`. A bounded actual SDK evaluation confirms **`GODOT_LINUXBSD`**, `GodotTargetPlatform: linuxbsd`, `OS: Unix`, with `DEBUG` and `TOOLS`; the consumer supplied the genuine platform symbol. No technical source repair or blind rerun was made in this attempt.

The original focused invocation is `55960e73-29ec-4d63-ad88-028f588e5e6d`, platform `linux-x64-6.8.0-147-generic`, observed tool hash `1b607aea3c49c937008cfeca23d8e9a7c8082516cd844155c5c2291ced86c8cc`. It ran `2026-11-14T20:21:29.529Z`–`20:38:23.523Z` including setup. Its receipt is `/home/phase1/phase1-final-17/.phase1/smoke/55960e73-29ec-4d63-ad88-028f588e5e6d/smoke-receipt.json`, SHA-256 `419cfed0bd80d579415317eef1407057098cad2cced33565c89716f2e4ffc5a8`. All seven original exact test reports were inspected: five passes have valid nonce/input/tool/platform-bound proofs and evidence hashes, while both failures remain failures. All 16 original command-log hashes match; only the two failed case processes have nonzero exits, neither outer process timed out. Git status and the post-run 203-file input remain unchanged.

Original evidence is preserved in `/home/phase1/17-final-smoke-evidence.tar.gz` and copied/extracted under `.phase1/temp/17-linux-smoke/` on Windows. Matching SHA-256: `ed71610d06a12d0aafa5d0c292b9bb20c5d14af9886098fad4c3b6696e76ada9`. It includes original reports/proofs/subject logs/commands, native bootstrap records, the actual SDK-symbol diagnostic and failed graph-fixture logs/inventory. `.phase1/temp/17-linux-smoke/original-inspection.json` records independent inspection. No credential or old build output is included. The [plan summary](../../.planning/phases/01-code-rules-and-documentation/01-17-SUMMARY.md) retains actual outcomes and limits. This first attempt remains **5 passed / 2 failed**, rather than receiving retroactive credit from the separate amendment/mechanism proof above.

The [setup](setup.md) and [verification](verification.md) pages define the ordinary commands. The earlier complete-Linux/restart boundary below is historical for Phase 1 under the creator's later exception. Full accelerated Linux execution and the original failed graph case remain visible follow-ups. Coverage and the eight explicit later-phase declarations retain their ownership in the [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md); no deferred case or unreviewed human component receives pass credit.

## Final repaired-source Windows execution

On 2026-11-14, the clean frozen validation checkout `.phase1/temp/17-windows-59f032c` executed the complete current Phase 1 scope after the four review repairs stabilized. It began with no old build/proof outputs. Only 41 exact lock-verified NuGet payloads and the authenticated official Windows Godot archive/checksum were seeded. Source was never repaired inside this validation checkout; donor files, VM files, private keys and old build outputs were excluded.

| Actual command | Actual result |
| --- | --- |
| `node tools/phase1/bootstrap.cjs --probe engine` | Exit 0; fresh native Windows engine, raw compatibility nonce `27a26937-02f2-4085-b9d2-254b5700f25c` |
| `dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false` | Exit 0; all five projects restored with unchanged committed locks |
| `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` | Exit 0; zero warnings/errors, `00:00:06.15` |
| `node tools/phase1/check.cjs --phase 1` | Exit 0; **676/676 ACCEPTED**, zero failed, blocked, missing or skipped selected obligations; eight separate Phase 5 deferrals |

| Identity | Actual value |
| --- | --- |
| Tested revision | `59f032cf23340058eec91f12c5d240f585dfabdc` |
| Invocation | `b8b5eacc-c46a-42d7-a898-01ad33c1b0a8` |
| Input / file count | `fe2bf549f20329633fd62cfeab796ca8fb2c5ce02da432346e8ee4c314873426` / 203 |
| Tool | `a23d7bacf589ac13e65eda9e329b0218ebef3a34ba90ad2630f1607349e6aab1` |
| Platform | `win32-x64-10.0.19044` |
| Canonical execution interval | `2026-11-14T18:40:20.004Z`–`2026-11-14T19:31:55.692Z` |
| Summary SHA-256 | `042c7e2b280940860c7e1304ee85bcfb756d4ae58c2d192b51c713dbbd37aa4a` |

The original canonical report is `.phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/summary.json`, recorded with setup/log receipts in the [partial plan summary](../../.planning/phases/01-code-rules-and-documentation/01-17-SUMMARY.md). Inspection at `2026-11-14T19:32:38.400Z` reverified all 676 exact discovered test reports, their hashes, 692 subject-evidence files, all 681 recorded command outcomes/log hashes and unchanged source fingerprint. Every selected case command exited zero without timeout/signal; no selected case was skipped or credited from another invocation. Frozen Git status and all five locks remain unchanged. Groups accepted: rules 449, graph 27, engine 22, runner 36, pins 27, register 46, docs 36 and state 33.

Observed tools are Node `24.22.1`, SDK `10.0.404`, compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)`, native Godot `4.7.2.stable.mono.official.ed1daf0bf`, editor runtime `8.0.16` and hosted application runtime `10.0.15`. The fresh raw native log includes construction/ready/exit and 21 surface/cycle records for two distinct instances, with callbacks, resources/exports/binding/null validation, teardown, late-work fencing, prohibited-worker rejection and captured main-thread delivery. Canonical engine cases separately passed within the complete run. No coverage or human judgment is inferred.

## Historical complete-Linux preparation and restart prerequisite

The new clean guest checkout `/home/phase1/phase1-final-17` is pinned to the exact revision and 203-file input above. Its tracked-source bundle `/home/phase1/phase1-tools/downloads/final-17.bundle` has matching Windows/Linux SHA-256 `334110bde21f2a983deb965d0d227e0872ed659309c8d170e3fd10e07f35f54b`. Guest inspection reverified all 41 package payloads against their raw hashes and current lock identities, both official Godot archives, all four retained tool archives and absence of old `bin`/`obj`/`.godot` outputs. Native archive identity and canonical common managed archive identity remain separate. No Linux restore, build, bootstrap or acceptance case has run on this candidate.

After positive live process/executable/parent/disk/known-host verification, only the owned old TCG guest received `sync; sudo -n poweroff` at `2026-11-14T18:43:22Z`. At `18:43:33.5437334Z`, QEMU 21752 and launcher 23576 were exited, loopback SSH/QMP listeners were absent, and the preserved disk allowed an exclusive read-open. The ignored process record now marks confirmed shutdown and clears live PIDs while retaining the old identities. The [environment record](../../.planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md) names the original receipts and exact prepared paths. No unrelated process, host setting or disk was changed.

The host still reported `HypervisorPresent: false`; Windows was not restarted by the agent. The creator has enabled Windows Hypervisor Platform and must complete the host restart. After restart, verify the active hypervisor, start only this owned guest with the prepared WHPX-default launcher, verify actual WHPX startup and genuine Linux access, then execute native bootstrap, clean locked five-project restore, strict zero-warning/error build and the complete `node tools/phase1/check.cjs --phase 1` selection on this frozen candidate. Unchanged deadlines apply. Keep WINDOWS 28 open until complete current Linux acceptance actually passes. Plan 01-17 remains halted at **0/2 accepted tasks**; all fourteen human components and RULE-01–RULE-07 remain Pending.

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

These eight declarations remain pending with `acceptancePhase: 5` and owner `Phase 5`. They are outside the current 676 selected denominator (670 in the historical pre-review run) and receive no pass or skip result.

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
