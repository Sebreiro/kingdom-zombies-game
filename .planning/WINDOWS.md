---
schema_version: 1
open_count: 1
waived_count: 0
fixed_count: 3
total_count: 4
last_updated: 2026-11-13T17:58:29.380Z
---

# Broken Windows Ledger

> Cross-phase defect register. With `workflow.windows_enforce` enabled, `/gsd-ship` blocks while `open_count > 0`.
> Waive with `gsd-tools windows waive <id> "<reason>"` (reason required).
> Mark fixed with `gsd-tools windows fixed <id>`.

| id | phase | kind | file | line | description | status | reason | recorded_at | resolved_at |
|----|-------|------|------|------|-------------|--------|--------|-------------|-------------|
| 1 | 01 | deviation | .planning/phases/01-code-rules-and-documentation/01-01-PLAN.md |  | Task 1 bootstrap TDD annotation has no valid RED commit; actual engine integration and failure controls pass, C# runner proofs belong to 01-02. | open |  | 2026-11-13T17:09:33.726Z |  |
| 2 | 01 | deviation | tools/phase1/bootstrap.cjs |  | Duplicate pin candidate initially returned PINS_MISSING before duplicate detection; fixed ordering and verified intended PINS_CONFLICT in 01-02. | fixed |  | 2026-11-13T17:38:49.703Z | 2026-11-13T17:39:05.630Z |
| 3 | 01 | deviation | tests/phase1/packages.lock.json |  | Added the omitted test dependency lock required for exact locked restore; committed and verified in 01-02. | fixed |  | 2026-11-13T17:39:05.981Z | 2026-11-13T17:39:06.328Z |
| 4 | 01 | deviation | tools/phase1/required-cases.json |  | Plan 01-03 supplied case names were absent from the required inventory; bound actual C# methods and added explicit pending R-37 boundary cases without crediting later enforcement. | fixed |  | 2026-11-13T17:58:02.412Z | 2026-11-13T17:58:29.380Z |

````json
[
  {
    "id": 1,
    "kind": "deviation",
    "phase": "01",
    "file": ".planning/phases/01-code-rules-and-documentation/01-01-PLAN.md",
    "line": null,
    "description": "Task 1 bootstrap TDD annotation has no valid RED commit; actual engine integration and failure controls pass, C# runner proofs belong to 01-02.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-11-13T17:09:33.726Z",
    "resolved_at": null,
    "milestone": "v0.1"
  },
  {
    "id": 2,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/bootstrap.cjs",
    "line": null,
    "description": "Duplicate pin candidate initially returned PINS_MISSING before duplicate detection; fixed ordering and verified intended PINS_CONFLICT in 01-02.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T17:38:49.703Z",
    "resolved_at": "2026-11-13T17:39:05.630Z",
    "milestone": "v0.1"
  },
  {
    "id": 3,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/packages.lock.json",
    "line": null,
    "description": "Added the omitted test dependency lock required for exact locked restore; committed and verified in 01-02.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T17:39:05.981Z",
    "resolved_at": "2026-11-13T17:39:06.328Z",
    "milestone": "v0.1"
  },
  {
    "id": 4,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Plan 01-03 supplied case names were absent from the required inventory; bound actual C# methods and added explicit pending R-37 boundary cases without crediting later enforcement.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T17:58:02.412Z",
    "resolved_at": "2026-11-13T17:58:29.380Z",
    "milestone": "v0.1"
  }
]
````
