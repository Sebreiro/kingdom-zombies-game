# 04-37 Task 2 — bounded node repair

Status: authorized, not yet implemented. Recorded after checkpoint `6883fdd` under `$gsd-autonomous --only 4`.

## Decision and retained state

`DECOMPOSE`: Task 2 has coupled restoration, completeness and producer-lifetime concerns. Apply the configured node-repair procedure with two repair attempts per subtask. This is an in-memory decomposition of existing Task 2; do not modify PLAN.md or add a phase/plan/task to the graph. Keep 37 plans / 74 tasks / 31 waves, six completed plans and 04-37 at 1/2 completed tasks until all required gates pass.

Task 1 commits `daf2512` and `66c1980` are preserved. The five pending Task 2 files and all immutable failed/passing records in [the checkpoint](04-37-CHECKPOINT.md) are retained. The unrelated concurrent Phase 6 specification commit `affc858` is preserved and is not this task's work. Never reset, revert, rebase, stash or discard these inputs.

The selected suite completed 17/18. `rules.godot-callsite-boundary` failed before its collision batch with stale injected `AdjacentCallSite` output and CS0103. The final restored native and solution builds pass, and the frozen 26 Core controls pass. None of those positives completes the failed collision or newly identified completeness/lifetime obligations.

## Subtasks and bounded ownership

| Subtask | Required outcome | Implementation scope | Initial repair budget |
| --- | --- | --- | ---: |
| R1 — current producer restoration | Regenerate actual current output before semantic cohort validation, execute compiler-valid intended collisions, and finish with a fresh restored positive. Preserve `RequireValid` and every original/current negative. | `tests/phase1/GodotCallSiteAdapterTests.cs`; an already planned helper consumer only if necessary. Maximum five paths. | 2 |
| R2 — complete durable producer proof | Enforce both observation-to-permission and permission-to-observation coverage; reject extra rows and missing observations; admit later independently observed cohorts without applying frozen 177-specific expectations to all cohorts; reproduce historical producer inputs without forever rehashing nonproducer test/hub/scene subjects against the live checkout. | `tests/phase1/EngineGeneratedTests.cs`, `tests/phase1/ExemptionValidator.cs`, `tests/phase1/GeneratedCallSiteAdapterTests.cs`, `docs/code/engine-generated-spans.json`, `docs/code/generated-callsite-adaptations.json`. Maximum five paths; edit only those actually needed. | 2 |
| R3 — current complete verification | After focused failed/affected/new controls pass, execute all 18 selected cases freshly, followed by restored native/solution builds, all 26 Core controls and exact frozen-input checks. Preserve every earlier failure. | Evidence and supported metadata only; a new code failure routes back to its specific repair subtask with that budget decremented. | 2 |

This inline repair may reopen consumers already owned by Task 1 and therefore exceed Task 2's original five-path union. Each repair subtask stays within five implementation paths and the entire plan remains within its original nine. This is the exact bounded coordinator amendment before editing those reopened paths; it grants no new implementation file or broader diagnostic permission.

## Invariants

- Preserve all five frozen 04-03 source files byte-for-byte throughout 37 and this repair. Its execution freeze is a checkpoint precondition, distinct from persistent Godot generator input identity.
- Identify actual generator inputs from the real producer/compilation, preserve their authenticated bytes/recipe when historical reproduction requires them, and retain observation-subject identities separately. Test/hub/scene hashes must not become permanent live generator-input gates merely because they were frozen for 37.
- Retain immutable original 220 syntax / 18 callsite / 16 Godot / 26 permission baselines and the frozen 177-observation descriptor. Later cohorts require their own independently authenticated observations, source/package/input identities and exact complete coverage. No registry-derived expected inventory, arbitrary count changes, broad suppression, wildcard producer or absent-member approval.
- Reject compiler-invalid fixtures before accepting any intended collision/negative. Regenerate current producer state after injected cases; stale outputs cannot supply success or rejection credit.
- Keep the existing 66 syntax spans and eight member permissions only where supported by actual observations. Required new negative fixtures must execute and fail for the intended reason before the fixes are accepted.
- All source/headless implementation and independent whole-phase review/fixes precede real game windows. No window, listening or judgment credit; all ASSET requirements remain pending.

## Retry accounting and closeout

The prior three direct corrections are preserved history. This is the first structured node repair; budgets above must not be silently reset. Record each concrete failed attempt and remaining budget here. If a subtask exhausts its budget, return an honest checkpoint with the attempted changes, exact remaining blocker and source/evidence identities.

Do not create a completed SUMMARY, commit Task 2 as complete or advance progress until the entire required gate passes. The eventual SUMMARY must disclose `[Node Repair - DECOMPOSE] Task 2` and its subtask outcomes, the existing bounded fifth-file amendment and all retained failures. Preserve concurrent edits and stage only this plan's owned changes. Parent owns draft PR #5 and the continuation document.
