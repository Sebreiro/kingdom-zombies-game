---
phase: 01-code-rules-and-documentation
plan: "17"
subsystem: linux-baseline-and-qualitative-review
tags: [godot, csharp, linux, actual-evidence, blocking-human]
requires:
  - phase: 01-16
    provides: Frozen f4658bb Windows 670-case pass and same-source genuine Linux checkout/tools/cache
provides:
  - Actual clean Linux locked restore, strict zero-warning/error build and raw native bootstrap
  - Sixteen actual canonical Linux outcomes, seven accepted and nine capacity timeouts
  - Reviewed 85-rule mappings, explicit AC-01 through AC-22 outcomes and fourteen pending human components
affects: [01-phase-verification, 02-playable-foundation, 05-ci-and-repeatable-game-testing]
tech-stack:
  added: []
  patterns: [retained-canonical-deadlines, separate-diagnostic-and-acceptance-evidence]
key-files:
  created: [.planning/phases/01-code-rules-and-documentation/01-17-SUMMARY.md]
  modified:
    - docs/workflow/platform-proof.md
    - .planning/phases/01-code-rules-and-documentation/01-VALIDATION.md
    - .planning/phases/01-code-rules-and-documentation/01-HUMAN-CHECK.md
    - .planning/WINDOWS.md
key-decisions:
  - Preserve canonical source, checks and deadlines; genuine Linux timeouts remain blocking despite a longer diagnostic pass.
  - Keep all human outcomes and RULE requirements Pending; independent material review does not supply human approval.
requirements-completed: []
actuals:
  tokens: 9404
  tasks: 0
  commits: 3
plan_head_before: 21597fdeaed675c89bab5e0dd7aabf29c0266597
plan_head_after: 3613a3750049af18d87e036e34c7d950fde6b7cb
duration: 56min recorded execution window
completed: 2026-11-14
status: halted
---

# Phase 1 Plan 17: Linux baseline and qualitative review Summary

**Clean genuine Linux setup and seven representative proofs pass; nine canonical timeouts and actual human judgment still block Phase 1.**

## Status and recorded window

This is a **partial, halted** summary. Neither task meets its full acceptance criterion: Task 1 has nine required Linux case failures; Task 2's independent review is prepared but its human component has not occurred. Completed tasks: **0/2**. All sixteen predeclared selectors actually ran; none is skipped or unrun. No requirement, plan or phase completion is claimed.

The logged execution window is `2026-11-14T15:54:44Z` (first clean Linux bootstrap) through `2026-11-14T16:50:02.551Z` (last focused docs child), rounded up to **56min**, excluding earlier context reads and this summary closeout. The [platform record](../../../docs/workflow/platform-proof.md) owns exact commands, environment/source identities, all sixteen case nonces, raw native assertion details and limits. The [validation record](01-VALIDATION.md) records actual AC-01–AC-22 outcomes. The [human-check record](01-HUMAN-CHECK.md) retains all fourteen reviewer/date/outcome fields as Pending.

## Task 1 — Actual Linux execution, acceptance blocked

Used the existing Ubuntu **24.04.5 LTS**, kernel **6.8.0-147-generic**, **x86_64** VM, QEMU TCG with four virtual CPUs and 4096 MB RAM. No duplicate VM or host installation was created. The clean `/home/phase1/phase1-baseline-16` checkout stayed at `f4658bb2872b6578b592a18475e58af04cd064d4`, matching the completed Windows source. It initially had no source build outputs; only the declared verified package/archive/feed cache was seeded. No old build outputs or donor files were copied.

Observed Node **24.22.1**, SDK **10.0.404**, compiler **5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)**, runtimes **8.0.16 / 10.0.15**, and native Linux Godot **4.7.2.stable.mono.official.ed1daf0bf**.

| Executed verification | Actual outcome |
| --- | --- |
| Existing native bootstrap, cold attempt | Exit 1, CHILD_TIMEOUT at unchanged 60-second game build; nonce `4b95c9b1-ad41-485e-98c9-3ee05ed5f778`; both restore steps passed. Failure retained. |
| Ordinary bounded locked five-project restore | Exit 0, all five projects restored, locks unchanged. |
| Strict whole-solution build, single node, original strict flags, 900-second outer bound | Exit 0; zero warnings/errors; `00:05:10.06`. |
| Unchanged native bootstrap after ordinary build | Exit 0, nonce `0d50ef94-b2e3-4e38-b583-e9345283a7df`; real Linux engine/runtime and construction/ready/exit evidence. |
| Sixteen exact existing `--case` invocations | **7 ACCEPTED, 9 CHILD_TIMEOUT**; each selected exactly one obligation. No skip/unrun/merged result. |

The seven accepted IDs are `docs.portable`, `state.inputs`, `state.identities`, `state.golden-vectors`, `state.numeric-and-random`, `state.snapshot` and `state.restore`. Their actual CTRF reports each contain one passed test and zero skipped tests; all referenced evidence/report hashes were rechecked after copying the original records.

The nine failed IDs are `R-38.positive`, `R-38.negative`, `ac-12.complete`, `graph.allowed-and-direct`, `docs.projects`, `engine.lifecycle`, `engine.exports`, `engine.generated-origin` and `engine.handwritten-partial`. Each has zero accepted results and an actual test-process SIGKILL at the unchanged outer **45-second** limit. Every canonical setup command exited zero. No source change or functional assertion failure was observed; failure to finish within the required deadline still blocks acceptance. Access works: the issue is execution capacity, not absent Linux access.

All sixteen reports have:

| Identity | Actual value |
| --- | --- |
| Revision | `f4658bb2872b6578b592a18475e58af04cd064d4` |
| Input | `23d1060bfbdb6b082624631c1e32f1cd31a9fe29287745072e364d9f56020071` |
| Tool | `7866ee43e38ed4100250b5f2fc452e16be5d35fda7bd5ee33d430a946156e206` |
| Platform | `linux-x64-6.8.0-147-generic` |
| Selector window | `2026-11-14T16:07:13Z`–`2026-11-14T16:43:39Z`, including a between-case pause for diagnostics |

Original reports remain `/home/phase1/phase1-baseline-16/.phase1/runs/<nonce>/summary.json`. The exact evidence archive `/home/phase1/17-evidence.tar.gz` is copied and extracted at `.phase1/temp/17-linux-evidence/` on Windows. Both archive hashes are SHA-256 **662ec21365f10271c1cfc5c06a984c11eac7422876102c663794cbcd218be037**. It retains actual logs/exits, command identities, original reports, setup receipts and explicitly separate diagnostics; it contains no credential or donor. The first loop was stopped only after its active `state.identities` child exited 0; `/home/phase1/17-loop.pause` preserves the terminal process status. The remaining eight selectors then ran once sequentially. No canonical invocation was interrupted or silently credited.

The successful raw native `engine.json` actually emits 21 fresh surface/cycle assertions: two distinct native instances, construction/resources/exports/bindings/null validation, callbacks/process/signals/teardown, prohibited-worker rejection, old-generation fences and captured main-thread delivery. They were inspected, rather than inferred from exit zero. Raw bootstrap evidence does **not** promote any timed-out canonical native selector.

## Bounded environment diagnostics — no acceptance credit

Three process-local trials of the failed named-argument selector retained the canonical deadlines and also failed: disabled hardware intrinsics (`c56c47ad-da0c-4cf3-8813-d20c0d169852`), disabled dynamic PGO (`a6577982-cd19-4b9c-b5cd-ef5dad3d5674`) and single-processor runtime scheduling (`d445c64f-764d-449c-9bf7-ae4fb6fc3c6b`). The first worsened setup performance. All were reverted by ending their shells; the normal environment was used for the original sixteen outcomes. No source, binary input, pin, rule, threshold, exemption, test selector or canonical deadline was changed.

One explicitly separate direct diagnostic used outer **180 seconds** and test **120 seconds**, normal runtime settings, nonce `64865a1c-850f-4f4c-a18a-2b9bf04b5056`. It exited 0 with one passed, zero failed/skipped tests. The selected body took **39.724s**, the session **49.08s** and total approximately **53s**, exceeding the canonical outer 45 seconds. Standard MTP trace showed normal discovery and actual selected evaluated-reference/Roslyn work, with telemetry disabled. This is diagnostic evidence of the capacity gap; it is not canonical acceptance. Exact logs and the documented Microsoft controls are linked in the platform record. No further environment-trial loop was run.

## Task 2 — Independent material review prepared, human review pending

Reviewed all five current project pages against executable policy, all **85** dispositions (**48 retained / 31 adapted / 6 inapplicable**), **84** unique source headings, **24** donor exemption dispositions, **77** implemented bindings and the two explicit Phase 5 coverage bindings. The six inapplicable rules retain their individual reasons/resolution cases. All five projects, twelve Error analyzers, numeric limits, five LF locks, authentic xUnit logical seed, 170 native span permissions and eighteen precise member suppressions are preserved. No working source/test/rule/configuration change was made.

Focused `--group register` on `21597fd`, nonce `cae2f0ca-f2bf-490f-897b-7ca40513a421`, accepted **46** implemented controls with zero failed implemented controls. Its actual overall exit is **1** and report status **fail** because the exhaustive group also selects pending Phase 5 `ac-06.complete`, reported blocked/IMPLEMENTATION_PENDING. That aggregate received no pass or skip credit. Input `08e5f9859a85da7075daa9d20760cac21076be468262b435c8ee8cdb2b593925`; tool `8221b70af7d3679f06ad0714ac6cbcba02ca12b9f357dadb4c0ffc031371216c`; platform `win32-x64-10.0.19044`.

The current record maps all **AC-01–AC-22** to actual evidence and remaining conditions. The fourteen human components are R-24, R-34, R-62, R-63, R-65, R-72, R-73, R-74, R-77, R-78, R-79, R-81, R-83 and R-84. Each now names its concrete evidence and review question. Reviewer/date/outcome remain **Pending** for every row and overall. Plan 01-17 explicitly requires: “Keep it pending until the human review actually occurs.” AI inspection, automation and automatic approval cannot substitute for that judgment.

The completed immutable Windows report remains **670/670 accepted**, nonce `030dd8f9-c5e7-4d52-9b03-505948e8755c`, on the same frozen implementation. The eight Phase 5 declarations remain pending without pass/skip credit: `ac-02.complete`, `ac-06.complete`, `ac-07.complete`, `ac-09.complete`, R-49 positive/negative and R-82 positive/negative. Inclusive 90%/80% targets and the [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) are unchanged.

## Distinct D-11 documentation evidence

After the proof/review prose commits, actual `node tools/phase1/check.cjs --group docs` passed **36/36**, exit **0**, zero failures/skips or nonzero command exits. Canonical report: `.phase1/runs/e8ec1335-b79e-4a7b-92cf-bb39f57658b0/summary.json`.

| Identity | Actual value |
| --- | --- |
| Revision | `3613a3750049af18d87e036e34c7d950fde6b7cb` |
| Invocation | `e8ec1335-b79e-4a7b-92cf-bb39f57658b0` |
| Input | `e07e91f9b6d86a09dc8dfe21b0e44d5b2dfcec3eac034ce7fe69cfb991732eee` |
| Tool | `210785254e50a052df0021e9143f930dfd2820e62adbd17ecd8d51ce780561b9` |
| Platform | `win32-x64-10.0.19044` |
| Last child completion | `2026-11-14T16:50:02.551Z` |

This report validates changed prose on main; it is not another full Windows run or a Linux pass. The summary is outside the fingerprinted input set and records these identities without rewriting historical reports.

## Partial commits and measured actuals

- `1faad99` — `docs(01-17): retain actual Linux baseline capacity gap` (platform record and WINDOWS 28).
- `3613a37` — `docs(01-17): prepare acceptance and pending human review` (validation and human-check records).

The persisted ledger base is `21597fdeaed675c89bab5e0dd7aabf29c0266597`. `git rev-list --count` through `3613a3750049af18d87e036e34c7d950fde6b7cb` measures **3** commits: the two owned commits plus concurrent unrelated `41429aa` (Phase 4 specification), which was preserved. The **37,616** realized diff characters over the four owned changed paths yield **9,404 chars/4 tokens**. The unrelated specification is excluded from that token measure. This summary metadata commit is after the stated measured range; no harness token count is used.

## Deferred issues and checkpoint

- **WINDOWS 28 open:** nine required canonical Linux cases exceed current TCG capacity; sufficient genuine Linux execution capacity must produce passing evidence under existing checks/deadlines. Access already works. No source defect was established by the bounded diagnostics.
- Actual human review remains required for all fourteen components. No reviewer/date/outcome was invented.
- Historical WINDOWS **1** remains open; items **25–27** remain fixed. No retrospective RED, waiver or resolution was invented.

No functional stub, skipped test, unrun selector, new network endpoint/auth/schema surface or source deletion was introduced. No Phase 2/3/5 execution occurred. Captured Phase 2/3 context and the concurrent Phase 4 specification are preserved. Coverage, exhaustive platform certification, gameplay/exports/rendered/LAN/performance/endurance acceptance remain outside this work.

Shared STATE/ROADMAP/REQUIREMENTS are intentionally **not advanced** for this halted checkpoint: Plan 17 and Phase 1 are incomplete, progress remains 17/18, and RULE-01–RULE-07 remain Pending. Root owns remaining environment assessment and phase code/security/goal verification before consolidating the actual human review. The existing VM remains running for root closeout; no unrelated process was stopped.

## Self-Check: PASSED

All four owned changed paths, this summary and the retained evidence archive exist. Both owned proof/review commits are ancestors of HEAD; neither deletes tracked files. The sixteen original reports reconcile the exact same source/input/tool/platform, seven accepted test reports and subject-evidence hashes, nine actual canonical timeouts and zero skipped/unrun selectors. Archive hashes match across Linux and Windows. The main snapshot still exactly matches the distinct D-11 report input `e07e91f9b6d86a09dc8dfe21b0e44d5b2dfcec3eac034ce7fe69cfb991732eee` after summary creation. All fourteen human outcomes and seven RULE requirements are verified Pending. Summary status is halted, not complete; shared progress has not advanced. The only new tracked artifact is this intentional summary; generated/runtime evidence remains ignored. The Linux capacity gap remains open as WINDOWS 28, historical item 1 remains open, and items 25–27 remain fixed.
