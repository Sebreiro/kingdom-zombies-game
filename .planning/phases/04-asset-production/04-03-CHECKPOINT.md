# Plan 04-03 technical integration checkpoint

Recorded 2026-11-16 after 04-02 completion, on `feature/04-asset-production` at parent handoff `60e573c`.

## Executor state

The initial 04-03 executor returned a terminal checkpoint before editing any file: zero of two tasks complete, no implementation commits, no game window. Resume Task 1 after the independently checked graph amendment; do not infer completion from this checkpoint.

## Concrete gaps

1. `tools/phase4/check.cjs` registers existing case modules with explicit imports. The planned new `tools/phase4/cases/hub.cjs` cannot expose mandatory `hub.source` without a runner edit. That would exceed Task 1's five-file and the plan's nine-file implementation bounds.
2. The independent 180-obligation expected register currently lives at `tools/phase4/expected-obligations.json`, outside the exported Godot root. The runtime hub needs an explicit packaged input preserving all independent expectations; produced outputs cannot define what is expected.

## Parent decision

Resolve these implementation integration gaps through a narrowly bounded prerequisite or split, with explicit ownership and current positive/negative proof. The typed planner is revising the graph; an independent typed plan checker must approve the revision before execution resumes. This is a technical plan correction within the authorized Phase 4 scope and requires no new product decision.

Preserve all completed work, locked decisions, 180 obligations, 24 acceptance criteria, domain/UI backstops and judgment prohibitions. Plans remain bounded to nine implementation files, tasks to five. All source/headless implementation and independent whole-phase review/fixes precede real game windows. The single-phase boundary, draft PR #5, pending ASSET requirements and actual listening blocker remain in force.

## Resume

Use the fresh phase-plan-index dependency order after the amendment. Execute any new prerequisite first, then resume 04-03 at Task 1 with its revised exact contracts and normal task commits. The original executor remains available; no reset, restoration or duplicate implementation is needed.

## First independent integration review

The planner produced 04-36 (two tasks, seven files) for strict case discovery and the authenticated packaged register, with 04-03 consuming its explicit contracts. Mechanical checks passed for 36 plans / 72 tasks / 30 waves while preserving all approved obligation and acceptance identities.

The independent [integration review](04-PLAN-CHECK-INTEGRATION-PREREQUISITE.md) found one additional compatibility blocker: the current `tools/phase4/native-observer.cjs` requires the exact original nine-node tracer tree, while 04-03 expands that production scene. The required 04-31 export group includes this observer's case, but the graph had no unfinished owner for adapting the actual observer or preserving a compatible subject. Direct `Bow` and `Summary/ResourceId` bindings also have existing source/PCK consumers.

The parent routed this concrete finding back to the planner for a bounded ownership/verification correction before execution. The discovery/register design itself passed. Keep actual natural packaged hub startup, resource/status bindings and positive/negative controls required; relaxing the old node count alone is insufficient. No production implementation or acceptance was performed during this review.

## Approved resolution

The independent [revision review](04-PLAN-CHECK-INTEGRATION-PREREQUISITE-REVISION-1.md) passes with zero blockers or warnings. Plan 04-36 owns the seven-file/two-task discovery and packaged-register prerequisite. Plan 04-03 preserves the existing bow/summary bindings while implementing the complete hub. Plan 04-31 Task 1 owns the current tracer, exporter, native observer, profile fixtures and portability case; it requires current C# dependency closure and fresh natural packaged-hub positive/negative observations. Task 2 owns final delivery changes and runs the entire export group afterward. Plan 04-30 registers the required `export.hub-consumer` metadata.

All 36 schema/structure checks, 32/32 decision coverage and the fresh revision consistency/identity probes pass. Counts remain 36 plans / 72 tasks / 30 waves; completed implementation remains 5/36. Execute 04-36, then resume 04-03 Task 1. Historical failed review and all original evidence remain unchanged.
