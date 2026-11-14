---
phase: 01-code-rules-and-documentation
plan: "16"
subsystem: clean-platform-baseline
tags: [godot, csharp, windows, linux-preparation, exact-provenance]
requires:
  - phase: 01-15
    provides: Independent 670-case Phase 1 selection and eight explicit Phase 5 declarations
provides:
  - Complete clean Windows functional baseline with 670 fresh accepted obligations
  - Same-source genuine Linux checkout, pinned tools, authenticated feed/cache and sixteen predeclared representative cases
  - Narrow authenticated Linux generated-source adaptations preserving exact scopes
affects: [01-17, 05-ci-and-repeatable-game-testing]
tech-stack:
  added: []
  patterns: [canonical-managed-payloads-with-native-platform-engines, exact-observed-platform-source-identities]
key-files:
  created: [docs/workflow/platform-proof.md]
  modified:
    - tools/phase1/bootstrap.cjs
    - tools/phase1/toolchain.json
    - tools/code-rules/GeneratedLayout.cs
    - tests/phase1/GeneratedLayoutAdapterTests.cs
    - tests/phase1/GeneratedCallSiteAdapterTests.cs
    - tests/phase1/ExemptionValidator.cs
    - docs/code/generated-layout-adaptations.json
    - docs/code/generated-callsite-adaptations.json
    - docs/workflow/setup.md
    - .planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md
key-decisions:
  - Preserve common managed NuGet bytes and five locks across operating systems; authenticate each native engine archive separately.
  - Permit only complete observed Linux SDK and testing-platform source identities at unchanged exact path, line, member and producer boundaries.
  - Keep final same-source checkouts fixed; publish completion prose with separate D-11 evidence and leave Linux acceptance and human review to 01-17.
requirements-completed: []
requirements-contributed: [RULE-01, RULE-03, RULE-04, RULE-06]
actuals:
  tokens: 20525
  tasks: 2
  commits: 8
commits: 8
plan_head_before: 9b8091c24b009c433849b5fa5fa5ab888db5fccd
plan_head_after: 605511232928720251f40fb19f2df452b72e28fe
duration: 84min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 16: Clean Windows baseline and Linux preparation Summary

**A clean donor-independent Windows checkout accepts all 670 Phase 1 obligations, and the identical source is prepared on genuine Linux with exact package/generated-source provenance.**

## Completed tasks and commits

| Task / necessary closure | Commit | Result |
| --- | --- | --- |
| Clean setup portability | `a990f30` | Supported Windows ZIP extraction; authenticated canonical managed packages on both native platforms |
| Exact Linux SDK source closure | `4855ba6` | Two observed SDK source hashes, actual producer identity and exact positive/negative boundaries |
| Task 2 — Concrete Linux preparation | `18fec9f` | Pinned environment/access/cache instructions and sixteen exact cases declared before 01-17 |
| Final observed producer closure | `f4658bb` | One exact Linux testing-platform helper identity; unchanged member/package boundaries |
| Task 1 — Final Windows record | `c89f552` | Actual complete passing baseline, canonical identities, explicit deferrals and same-source Linux preparation |

All authoring/commits used explicitly authorized pinned main, the supplied-root guard, configured branch check and normal hooks. Test-only clean checkouts are owned under ignored runtime directories. No repair was authored there; exploratory source updates copied exact changes authored in main. There are no tracked deletions or unrelated staged files.

## Final clean Windows evidence

Frozen source: `f4658bb2872b6578b592a18475e58af04cd064d4`, root `D:/Work/Games/KingdomZombies/.phase1/temp/16-windows-f4658bb`. The independent source snapshot contains 200 inputs and matched main and Linux before execution. The checkout began clean without old build/generated/report outputs, donor folders, Downloads dependence or VM/private-key files. It seeded only 41 existing packages verified against raw archive sidecars and normalized locked NuGet identities, plus the authenticated official engine ZIP/checksum file. The SDK package was resolved from that verified archive feed, not copied from an unrelated warm SDK cache.

The actual ordinary commands all passed:

```sh
node tools/phase1/bootstrap.cjs --probe engine
dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase1/check.cjs --phase 1
```

Bootstrap nonce `59a72fa8-cb62-43ba-aae8-c378f78bf813` records genuine native construction/ready/exit and hosted runtime `10.0.15` (raw compatibility only). Locked solution restore covers all five projects without lock changes. Strict solution build has **zero warnings/errors**, actual exit 0.

The [unaltered canonical full report](../../../.phase1/temp/16-windows-f4658bb/.phase1/runs/030dd8f9-c5e7-4d52-9b03-505948e8755c/summary.json) publishes **670/670 ACCEPTED**, actual exit **0**, **zero failed, blocked, missing or skipped selected obligations**. Independent closeout inspection confirms 670 actual zero-exit case commands and zero skipped/failed test reports. The coordinator verifies nonce, actual evidence and immutable source through publication.

| Identity | Actual value |
| --- | --- |
| Invocation | `030dd8f9-c5e7-4d52-9b03-505948e8755c` |
| Input | `23d1060bfbdb6b082624631c1e32f1cd31a9fe29287745072e364d9f56020071` |
| Tool | `f3f2c460d76528ac47f5c5e257f549b5585e76792ab7913d4b0168be4ffe661b` |
| Actual platform | `win32-x64-10.0.19044` |
| Full command interval | `2026-11-14T14:49:32.789Z`–`2026-11-14T15:39:56.667Z` |
| Observed tools | Node 24.22.1; SDK 10.0.404; compiler 5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf); Godot 4.7.2.stable.mono.official.ed1daf0bf; runtimes 8.0.16 and 10.0.15 |

Accepted groups are runner **36**, pins **27**, register **46**, rules **447**, docs **36**, graph **26**, engine **19** and state **33**. All 666 original implemented controls plus the four selector controls remain required. No existing case, diagnostic, threshold, scoped suppression, lock, project or generated permission was removed or weakened.

## Explicit deferred identities

The report lists these separately with `acceptancePhase: 5`, owner `Phase 5`, outside the selected denominator and without pass/skip credit:

| ID | Required later certification |
| --- | --- |
| `ac-02.complete` | Exhaustive clean two-platform execution/reconciliation |
| `ac-06.complete` | Complete bindings including measured coverage |
| `ac-07.complete` | Complete enforcement certification including measured coverage |
| `ac-09.complete` | All numeric/report thresholds including measured inclusive 90%/80% |
| `R-49.positive` | Actual analyzer coverage meets both unchanged targets |
| `R-49.negative` | Actual analyzer coverage rejects insufficient/invalid evidence |
| `R-82.positive` | Actual production/native-hosted managed coverage meets both targets |
| `R-82.negative` | Actual production coverage rejects insufficient/invalid evidence |

The [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) retains their full obligations. No coverage or exhaustive Linux certification is claimed.

## Concrete Linux preparation

The owned running VM is actual Ubuntu **24.04.5 LTS**, kernel **6.8.0-147-generic**, **x86_64**, QEMU 11.1 TCG with four CPUs/4096 MB. Actual tool identities were observed from authenticated archives: Node 24.22.1, SDK 10.0.404, .NET 8.0.16/10.0.15 and native Godot 4.7.2.stable.mono.official.ed1daf0bf. Guest prerequisite packages come from signed Ubuntu sources. [01-ENVIRONMENT.md](01-ENVIRONMENT.md) records exact archives/hashes, OS prerequisites, process ownership, isolation and ordinary commands.

Access uses `C:/Windows/System32/OpenSSH/ssh.exe`, user `phase1`, `127.0.0.1:23022`, `BatchMode=yes`, `ConnectTimeout=10`, and absolute `UserKnownHostsFile`/key paths under `D:/Work/Games/KingdomZombies/.phase1/tools/linux-vm/`. The PATH `ssh.bat` shadow is not used. No credential contents entered a fixture, source bundle or report.

The final clean checkout `/home/phase1/phase1-baseline-16` is pinned to **the same f4658bb revision and input hash above**, with no `bin`, `obj` or `.godot` outputs under source/tests/tools. Source bundle `/home/phase1/phase1-tools/downloads/baseline-16.bundle` has SHA-256 `246720cdcb7cdffad00e6bb3ec4bb497e9f22fede3914c7a0e1312ee90f95f82`. Both official engine archives are authenticated, native tools extracted, all four canonical managed payloads verified into `.phase1/feed`, and the exact 41-package cache is ready. Source `/home/phase1/phase1-tools/env.sh` before commands; each checkout retains its own CLI home/cache/temp paths. The VM and final clean checkout remain available for 01-17.

The sixteen exact IDs, declared before acceptance, are `R-38.positive`, `R-38.negative`, `ac-12.complete`, `graph.allowed-and-direct`, `docs.portable`, `docs.projects`, `state.inputs`, `state.identities`, `state.golden-vectors`, `state.numeric-and-random`, `state.snapshot`, `state.restore`, `engine.lifecycle`, `engine.exports`, `engine.generated-origin` and `engine.handwritten-partial`. Run the existing setup/bootstrap, locked restore and strict build, then each ordinary `node tools/phase1/check.cjs --case <ID>` command as recorded in the [platform record](../../../docs/workflow/platform-proof.md). Common replay vectors remain unchanged. Preparation is not Linux acceptance; 01-17 must execute every selected case and record actual results.

## Deviations from plan — resolved portability issues

**1. [Rule 3 — Clean setup] Supported extraction and canonical package provenance.** Plan 14 recorded Archive-module autoload failure; fresh 01-16 setup verifies the installed .NET ZipFile alternative now used by bootstrap. Actual inspection found 28 inner entries per OS archive; managed contents match but NuGet relationship/core metadata differs. Linux raw nupkg hashes are not mislabeled as canonical. Both platforms retain the exact existing Windows-archive managed bytes/locks, while actual native engines come from their respective authenticated archives. Missing/alternate managed origins are rejected. Fresh Windows extraction/package hashes, clean bootstrap and the full run pass. Commit `a990f30`; WINDOWS **25 fixed**.

**2. [Rule 1 — Portability] Exact observed SDK layouts.** Exploratory Linux strict build rejected SCR0007 at `(4,121)` on SDK assembly attributes. Two complete Linux source identities and the actual Linux producer-target identity are added to the existing four-source registry at unchanged suffixes/line 4. Actual SDK lookup is portable. Existing tests accept both exact variants and reject changed/wrong-path/adjacent source. Strict Windows build and origin/boundary controls pass; commit `4855ba6`; WINDOWS **26 fixed**. The 170 Godot spans are unchanged.

**3. [Rule 1 — Portability] Exact testing-platform helper source.** Linux emits one LF after the MSBuild hook while retaining all other template bytes; its raw hash differs by one byte from Windows. Existing registries/validator recognize only that observed complete Linux identity at the same authenticated package/member. Origin/adjacent/collision controls pass; commit `f4658bb`; WINDOWS **27 fixed**. The eighteen member suppressions are unchanged.

Exploratory Linux logs remain separate: first cold bootstrap `e8b3cf84-b09b-487d-958f-78bc15092e5c` hit the preserved 60-second restore deadline; the ordinary bounded locked solution restore passed. Initial strict build identified the SDK issue; a subsequent parallel setup build exhausted its bound without new diagnostics. Ordinary strict single-node builds then passed all five projects with zero warnings/errors in **183.63s** and **175.88s**. Combined provenance inventory checked **560** actual source/text/package/producer identities with no mismatch. The actual existing permission-reader/duplicate-boundary control passed **1/1**, and a direct actual native scene exited zero with lifecycle/resource/thread records. These are exploratory setup feedback, not final clean Linux baseline/01-17 acceptance.

The first raw Linux permission invocation omitted `PHASE1_ROOT` and failed on an assembly-relative path; the corrected ordinary invocation passed. Two new Windows origin assertions initially constructed the wrong newline variant; the exact unique MSBuild hook correction passes. A focused boundary publication correctly rejected concurrent prose editing with SOURCE_CHANGED; its frozen rerun passed. These diagnostic reports are preserved without acceptance credit.

The earlier full invocation `126212cb-4409-4402-8c64-9303c484e4ca` on `18fec9f` was stopped shortly after start when the final Linux helper variant was discovered. Only its positively identified coordinator/descendants were stopped; no VM or unrelated process was touched. Its interruption receipt remains in `.phase1/temp/16-preparation/interrupted-candidate.json`; no passing report was fabricated. The stabilized f4658bb invocation is the sole completed final full run.

## Limits and closeout

All seven shared RULE requirements, Phase 1 closure and all fourteen human-review outcomes remain Pending. No actual representative Linux acceptance, measured coverage, exhaustive certification, gameplay/export/LAN/rendered/endurance pass or human judgment is claimed. Exactly five projects, all twelve error analyzers, technical thresholds, five LF locks, authentic xUnit seed, 170 native spans, eighteen member suppressions and the inherited companion repair are retained.

There are no new functional stubs, skipped tests or unrun required plan verifications. Resolved deviations are recorded in WINDOWS 25–27; historical open item 1 remains open. No new network endpoint, auth boundary or schema trust surface was introduced. Acquisition still uses the pinned official archive sources/hashes and declared NuGet feeds.

The recorded implementation/verification window is `2026-11-14T14:18:18.597Z`–`2026-11-14T15:41:26.671Z`, rounded up to **84min**, before summary/shared-metadata closeout. Initial measured actuals cover 42,215 realized diff characters / 4, rounded up to **10,554**, across five commits ending at c89f552, from the persisted plan ledger. These are diff-scale actuals, never harness token counts. Completion-metadata documentation evidence and the final measured range are recorded separately after publication.

## Shared closeout and distinct D-11 evidence

Summary/self-check and fixed WINDOWS 25–27 were committed as `18ff221` before shared progress changed. SDK `roadmap.update-plan-progress 1` first recorded **17 summaries / 18 plans, In Progress**. Only then did STATE advance **16 → 17**, recalculate progress and record **84min / two tasks / eleven owned files**, decisions and session. Metadata commit `49fd6d1` preserves the approved amendment, plan-18 ordering and full Phase 2/3 context annotations. RULE-01–RULE-07 and all fourteen human outcomes remain Pending; no requirement was prematurely marked complete.

The first postmetadata docs invocation `4de5deb9-7119-45db-b2a1-c5bf662c1005` accepted 32 controls and rejected four with the same `DOC_TARGET`: an authored platform page linked directly to an ignored runtime JSON outside the strict Git documentation inventory. Prose-only fix `6055112` links the tracked summary and records the exact runtime path as code. The checker/subject rules were not weakened and no implementation/configuration changed.

The subsequent actual `node tools/phase1/check.cjs --group docs` accepted **36/36**, zero failures, actual exit **0**, with distinct [canonical evidence](../../../.phase1/runs/57d58377-4041-481d-a21e-16331c98d66f/summary.json):

| Identity | Actual value |
| --- | --- |
| Tested revision | `605511232928720251f40fb19f2df452b72e28fe` |
| Invocation | `57d58377-4041-481d-a21e-16331c98d66f` |
| Input | `08e5f9859a85da7075daa9d20760cac21076be468262b435c8ee8cdb2b593925` |
| Tool | `311e21c1f8efe05630bacea96bd512b0bfca556d4b7034d1b4e666cf11335389` |
| Platform | `win32-x64-10.0.19044` |
| Actual last child completion | `2026-11-14T15:49:25.389Z` |

Compared with the frozen full input, current main has exactly three changed fingerprinted prose/progress inputs: `docs/workflow/platform-proof.md`, `.planning/ROADMAP.md` and `.planning/STATE.md`. There are no removed inputs or source/configuration/lock changes. The current snapshot matches the distinct docs report above. The final same-source Windows and Linux checkouts retain their original `23d1060b...6020071` input. This documentation result is not a replacement full run or Linux acceptance.

Final frontmatter actuals are measured from the persisted ledger base `9b8091c24b009c433849b5fa5fa5ab888db5fccd` through `605511232928720251f40fb19f2df452b72e28fe`: **eight commits**, **82,097 realized diff characters**, **20,525 ceil(chars/4) tokens**. The initial historical five-commit measurement above remains explicit. This final summary-only evidence update follows that measured end revision and is outside the fingerprinted input set.

## Self-Check: PASSED

All eleven owned implementation/documentation paths and this summary exist. All five task/closure commits, summary commit `18ff221`, metadata commit `49fd6d1` and prose fix `6055112` are ancestors of HEAD. The final canonical report reconciles 670 accepted results, zero failures/skips and eight separate deferrals; all 670 case command exits are zero, and the frozen input remains unchanged. Exactly five LF locks, the compiler/analyzer policy, declaration manifest, 85-rule register, 170 native span records and scoped exemption registry are unchanged from the plan base. Current main's distinct D-11 docs report accepts 36/36 and matches its current snapshot; shared progress is 17/18, next 01-17, with Phase 2/3 annotations preserved. No tracked deletion or unrelated untracked file remains. WINDOWS 25–27 are fixed; historical item 1 remains open. Shared RULE and human-review outcomes remain Pending.
