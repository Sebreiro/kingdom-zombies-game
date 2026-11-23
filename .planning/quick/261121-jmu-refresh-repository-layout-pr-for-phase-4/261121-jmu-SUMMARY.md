---
mode: quick-full
task_id: 261121-jmu
status: complete
---

# Repository layout PR refresh

- Updated the layout policy and game tools guide. The [Phase 4 integration guide](../../../docs/workflow/phase4-layout-integration.md) maps Phase 4 runtime files, tests, production inputs and tools to their workspace locations and identifies the live references and checks that must move with them.
- Reviewed Phase 4 through `23db20a` and checked `dev` at `5428da8`. The simulated merge still reports conflicts in 30 files across planning state, permission registers, Core, tests and Node runners. Its resulting tree also retains 44 tool files, 20 production inputs and 120 Godot files at their old paths. Reconcile and verify the combined checkout after Phase 4 lands.
- The separate open PR #7 also overlaps in planning state and the placement of its two repository-maintenance scripts. If that PR lands first, reconcile those files as part of the merge from `dev`.
- On documentation commit `8832a16`, the Windows layout suite passed 6/6 and the documentation group passed 36/36 with pinned Node 24.22.1. The initial documentation run lacked the verified local Godot feed and began before the new guide was tracked. The successful rerun used the same verified four Godot packages from the existing checkout. Its report is `.phase1/runs/60b25cd6-3087-45af-8ae7-3f1b0bfd13e7/summary.json` in the cleanup checkout.
- The successful checks cover the cleanup branch. The combined source build and portable Windows inspection checks remain part of integration.
