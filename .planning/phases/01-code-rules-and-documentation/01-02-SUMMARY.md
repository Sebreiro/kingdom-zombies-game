---
phase: 01-code-rules-and-documentation
plan: "02"
subsystem: testing
tags: [csharp, xunit, mtp, proof-runner, evidence, process-cleanup]
requires:
  - phase: 01-01
    provides: Pinned Windows native-engine and compiler bootstrap
provides:
  - Independent inventory of 22 acceptance criteria, 38 edge rows and all R-01 through R-85 obligations
  - Fresh exact C# test, input, tool, platform and evidence reconciliation
  - Real failure, interruption, concurrency, source mutation and process-tree cleanup controls
affects: [01-03, phase1-proof-groups, verification]
actuals:
  tokens: 50332
  tasks: 2
  commits: 4
plan_head_before: 1d5d3e7e018c360f188811274f57d3202ad86ca7
plan_head_after: c036f56ce05b3fb5c85108de2cf68d945e198bfb
tech-stack:
  added: [xunit.v3 4.0.2, Shouldly 4.3.0, Microsoft.Testing.Platform 2.5.1]
  patterns: [independent obligation manifest, isolated C# process mutations, atomic run summary, exclusive repository check lock]
key-files:
  created: [tools/phase1/check.cjs, tools/phase1/required-cases.json, tests/phase1/KingdomZombies.Phase1.Tests.csproj, tests/phase1/packages.lock.json, tests/phase1/ProofProtocol.cs, tests/phase1/RunnerContractTests.cs, tests/phase1/RunnerProcessFixture.cs, tests/phase1/PinsTests.cs]
  modified: [tools/phase1/bootstrap.cjs]
key-decisions:
  - "Use the actual 4.0.2 MTP executable switches and exact-method filtering; reconcile both its CTRF result and independently tagged C# proof evidence."
  - "Reject all duplicate identities and declarations, including identical duplicates; never select an accidental winner."
  - "Keep unimplemented obligations explicit and blocked in --all; no RULE requirement or phase completion follows from this runner slice."
requirements-completed: []
requirements-addressed: [RULE-01, RULE-03, RULE-04, RULE-07]
coverage:
  - id: D1
    description: Exact fresh result reconciliation and real process integrity controls
    requirement: RULE-03
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --group runner
        status: pass
    human_judgment: false
  - id: D2
    description: Pin uniqueness, permutation and deliberately wrong tool identities
    requirement: RULE-01
    verification:
      - kind: integration
        ref: 9 implemented pins cases in final --all run; other pins obligations remain blocked
        status: pass
    human_judgment: false
duration: 21min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 02: Trustworthy C# proof results Summary

**A locked C# proof executable now supplies fresh, independently reconciled evidence, with real process failures and tampering rejected before an atomic success summary can be published.**

## Performance

- Observed execution window: 2026-11-13T17:15:18Z through approximately 17:36:26Z, 21 minutes; initial context loading precedes this window.
- Tasks: 2/2; implementation files: 9.
- Actual estimate scale: 201,325 realized diff characters / 4, rounded up to 50,332. The explicit obligation inventory accounts for most of the diff. Four commits are measured from the persisted plan ledger through `plan_head_after`; subsequent documentation commits are outside that measured implementation boundary.

## Accomplishments

- `check.cjs` implements `--group pins|runner|register|rules|graph|state|engine|docs|coverage|platform`, `--all`, `--list` and exact `--case`. Unknown/conflicting options, empty selections and unknown IDs fail. A fresh cryptographic nonce, exclusive repository lock and isolated run directory separate every invocation.
- The independently authored manifest contains all 22 AC identities, all 38 SPEC edge identities, all 85 donor rule identities, and positive/negative or register-resolution obligations. Proposed dispositions remain subject to the later register validation; qualitative and inapplicable rows do not receive fabricated runtime passes. Pending subjects remain in selection and block `--all`.
- Exact locked restore/build precedes C# execution. Each selected method must match its actual MTP report, trait ID and pass count, and must also emit one fresh proof result with matching nonce/input/tool/platform, successful infrastructure prerequisites, subject diagnostic/location and nonempty evidence. Missing, unknown, duplicate, skipped, blocked, stale, escaping and unrelated-diagnostic results fail.
- Source snapshots include approved new untracked implementation inputs. Input and evidence hashes, executable/compiler identities, OS/revision, commands, child exit status and process log hashes are retained. Approved-input changes invalidate publication. The lock is held through same-directory temporary-write/atomic-rename publication; interruptions cannot publish a complete pass.
- C# process controls run isolated copies of the normal entry point and an actual built C# executable. Every mutation has a separate registered case ID. Missing executable, zero discovery, wrong selection, skip, assertion failure, crash, timeout, deleted log/result, duplicate result, wrong nonce, stale prior pass, interruption, concurrency and source/document changes all reach their intended diagnostics. The timeout fixture requires a live descendant readiness handshake before proving both PIDs exit. Competing `--all` is rejected while the first successful run remains intact.
- Independent failures are retained together, blocked prerequisites are distinguished from passes, and diagnostic output links to relative process logs. Controlled fixture coordinators have five-second bounds; normal enclosing proof cases have 45-second bounds.

## Task Commits

1. **Task 1: Run one C# proof through independent obligation reconciliation** — `5c5c185` (validated RED), `afab7e8` (GREEN).
2. **Task 2: Exercise real failure, cleanup and publication controls** — `7c97a4f` (validated RED), `c036f56` (GREEN).

No tracked file deletion was included in these commits. Preexisting `.planning/state.json` and `.planning/tmp/` were preserved and excluded from staging.

## Verification and Evidence

| Command or proof | Actual result | Evidence |
| --- | --- | --- |
| `node tools/phase1/check.cjs --group runner` | 32/32 pass; includes actual process controls | [runner summary](../../../.phase1/runs/68752f55-3fb4-4db7-bf25-f633131997f3/summary.json) |
| `node tools/phase1/check.cjs --case runner.process.timeout` after live-descendant hardening | 1/1 pass; actual descendant readiness and PID exit required | [timeout summary](../../../.phase1/runs/688b3cc2-db12-4fb6-b9e7-7d7f45c60b52/summary.json) |
| `node tools/phase1/check.cjs --all` on final implementation bytes | Expected exit 1: **41 implemented passes, zero failed controls, 222 explicitly blocked future obligations**, 263 selected | [final complete-inventory report](../../../.phase1/runs/75a2eb51-215e-4106-98b4-8b086cae1b3e/summary.json), [complete console output](../../../.phase1/temp/verified-all.log) |
| Deliberate genuine C# assertion through isolated normal entry point | Nonzero `TEST_FAILED`, intended assertion and location retained | [initial failure-control workspace parent](../../../.phase1/temp/), later registered `runner.process.assertion` evidence inside final report |
| `node tools/phase1/bootstrap.cjs --probe pins` | Pass; all existing raw pin controls and actual tool identities | [pin controls](../../../.phase1/runs/63468580-0605-476f-a7bb-4d57124eb100/pins-controls.json) |
| `node tools/phase1/bootstrap.cjs --probe engine` | Pass; actual locked restore/build/native Godot construction, ready and exit | [compatibility identity](../../../.phase1/runs/0fb38718-1999-4df4-b88f-a539947f0bf3/compatibility.json), [engine log](../../../.phase1/runs/0fb38718-1999-4df4-b88f-a539947f0bf3/engine.log) |
| Exact locked NuGet restore and actual pinned executable help | Pass; 4.0.2 uses `--report-xunit-ctrf`/`--report-xunit-junit`, not older documented spellings | Restore/build command logs in final report; [test dependency lock](../../../tests/phase1/packages.lock.json) |
| `dotnet nuget verify --all` for all 22 restored non-Godot test nupkgs | Exit 0; author/repository signatures verified | [package verification log](../../../.phase1/temp/test-package-signatures.log) |
| `node --check` for both development scripts and `git diff --check` | Pass | Actual command exit statuses observed |

The final full report reruns all 32 runner cases and all nine implemented pin cases after the last timeout change. Existing raw bootstrap probes remain compatibility evidence; they do not substitute for the still-pending complete engine/rules/platform acceptance groups. The broad `pins` group intentionally remains non-success until its declared future evaluated-configuration and edge obligations are implemented.

## TDD Gate Compliance

- **Task 1 RED:** `MissingRequiredResultCannotPass` executed and failed on `result.Exit.ShouldBe(1)` because the initial acceptance function returned zero for missing required evidence. The genuine fresh JUnit report returned `RED_EVIDENCE_OK`, then was semantically inspected. [RED record](../../../.phase1/tdd/01-02-task1/red-evidence.json), [unchanged report](../../../.phase1/tdd/01-02-task1/red-valid.xml). GREEN adds mandatory independent completeness and identity checks.
- **Task 2 RED:** `RealAssertionFailureIsActionable` executed an actual isolated C# assertion failure and observed `TEST_FAILED`; its assertion failed solely because the diagnostic lacked a clickable log link. The corrected fresh report returned `RED_EVIDENCE_OK`, with semantic inspection confirming the intended assertion. [RED record](../../../.phase1/tdd/01-02-task2/red-evidence.json), [unchanged report](../../../.phase1/tdd/01-02-task2/red-valid.xml). GREEN adds the direct log link and completes the real process controls.
- Earlier setup attempts used a relative fixture root/current directory incorrectly. Those reports are preserved but were rejected semantically as infrastructure failures; they did not authorize GREEN. Fixture setup and explicit root handling were corrected before the valid RED reruns. The automatic classifier's syntactic acceptance of the first Task 2 setup failure was insufficient; semantic inspection rejected it.
- Both tasks have RED commits preceding their own GREEN commits. No separate refactor commit was necessary. The historical Plan 01-01 missing-RED ledger entry remains open: these new controls prove intended failure behavior, without inventing earlier evidence.

## Decisions Made

- Keep the project-local MTP runner setting; do not change the repository-wide test runner. Inspect the exact installed executable rather than assume older report/filter switches.
- Reject identical duplicates through an explicit uniqueness policy. Equivalent ordering preserves selection, while duplicate candidates and conflicting declarations cannot choose a winner.
- Fail closed on a lock left by abrupt termination. The fixture proves a subsequent run is rejected, verifies dead PIDs, and explicitly removes only its own stale lock before a fresh successful run. Live-checkout verification does not steal or silently delete locks.
- Keep all unimplemented Phase 1 obligations visible. This is a small Phase 1 runner, with no gameplay, CI, comprehensive game catalog or later-phase implementation.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] Detect duplicate pin candidates before package-count validation.**
- Found during Task 2's genuine C# duplicate-candidate proof.
- Appending an identical fifth candidate originally reported `PINS_MISSING` before checking duplicate identity. Duplicate detection now runs before exact cardinality validation and reports `PINS_CONFLICT`.
- File: `tools/phase1/bootstrap.cjs`; verified by focused case and the final complete implemented suite; commit `c036f56`.

**2. [Rule 2 - Critical correctness] Commit the test dependency lock.**
- The plan's project path list omitted its generated lock file. Exact repeatable restore requires `tests/phase1/packages.lock.json`; it is committed and normal execution uses `--locked-mode`.
- File: `tests/phase1/packages.lock.json`; introduced in `5c5c185`, verified by each normal execution.

The existing task-local workspace CLI/cache conventions and scoped authorized execution context were retained for child processes, network restore and Git metadata. No package install failed, no alternative package was substituted, no automatic approval review rejection occurred, and no authentication gate occurred.

## Known Stubs and Threat Review

No TODO/FIXME/placeholder implementation or skipped required proof remains. Explicit crash/skip/hang fixture methods are intentional negative subjects selected only inside isolated controls; the registered enclosing controls execute and pass. The 222 pending manifest obligations are honest later-plan declarations, not acceptance passes or hidden runtime stubs.

No new security surface outside the declared input/process/evidence and dependency-acquisition boundaries was introduced. Strict bounded JSON parsing, fatal UTF-8 decoding, exact identities, path containment, source snapshots, bounded children and process-tree cleanup cover this slice. Actual Linux execution remains unproved.

## Remaining Phase Work

All RULE-01 through RULE-07 requirements remain Pending. Plans 01-03 onward still own register completion, actual rule enforcement, boundary/state/lifecycle expansion, documentation, coverage and full Windows/Linux execution. This summary completes Plan 01-02 only and supports progress of 2/18 plans. Phase 2 is not started.

## Self-Check: PASSED

All nine implementation artifacts and this summary exist. All four implementation commits are ancestors of HEAD. No tracked deletion was included. Final full-inventory reconciliation confirms 41 implemented passes and zero failed controls, with every remaining selected obligation explicitly blocked. Runtime fixture outputs are ignored; preexisting planning runtime files are preserved.
