---
phase: "01"
authority: creator_closure_exception
status: open_followups
created: "2026-11-14"
---

# Phase 1 — Closure follow-ups

The creator requested Phase 1 closure after a few important Linux checks without restarting Windows. The [approved scope and exact instruction](01-CLOSURE-EXCEPTION.md) govern this one-time exception. Required focused technical evidence is now satisfied as recorded in the [summary](01-17-SUMMARY.md) and [platform record](../../../docs/workflow/platform-proof.md). The following items remain explicitly unperformed or unaccepted; they are not test or human-review passes.

| Item | Actual status | Follow-up owner / timing |
| --- | --- | --- |
| Complete accelerated Linux Phase 1 run | Not executed. The Windows baseline accepted 676/676 at 59f032c; Linux has focused composed evidence only. Hardware acceleration is not active in the current Windows session. | Platform/testing work when the creator permits a restart; retain Phase 5 full-certification ownership. |
| Original graph.allowed-and-direct Linux case | Failed/unaccepted: its clean fixture build exceeded the unchanged internal 60-second limit. The separately bounded direct proof used the actual validator and real compiled graph mutation and passed; it does not replace the original case's recorded result. | Rerun the original case on an adequately accelerated Linux environment. |
| Fourteen qualitative human components | Individually unreviewed/Pending: R-24, R-34, R-62, R-63, R-65, R-72, R-73, R-74, R-77, R-78, R-79, R-81, R-83 and R-84. No reviewer/date/outcome is fabricated. | Creator/design review through the existing [human checklist](01-HUMAN-CHECK.md), before claiming those individual reviews complete. |
| Eight deferred certification declarations | Pending with Phase 5 ownership: ac-02/06/07/09.complete, R-49 positive/negative and R-82 positive/negative. Inclusive 90% line / 80% branch policy is unchanged; no measured coverage is claimed. | [Phase 5 handoff](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). |
| Historical WINDOWS item 1 | Original initial bootstrap RED evidence was not captured. This is a retained process deviation; later actual functional checks do not manufacture historical RED. | Preserve the record and apply assertion-based RED discipline to future changes. |

The initial Linux attempt remains **5 passed / 2 failed / 0 skipped**. The observed GODOT_LINUXBSD test-name bug was corrected in 70918fd and the changed case passed separately on Windows and Linux. The other original failure is the graph fixture capacity item above. Current assurance is composed baseline plus focused amendments, not a synthetic seven-case or full Linux pass.

No host restart occurred. The owned Linux guest was cleanly shut down at 21:08:59 UTC on 2026-11-14, and its processes, listeners and free disk state were verified. The preserved VM and evidence remain available. The general complete-phase testing rule on the development OS and a local VM using the other OS remains in force for later phases.
