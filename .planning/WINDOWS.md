---
schema_version: 1
open_count: 1
waived_count: 0
fixed_count: 0
total_count: 1
last_updated: 2026-11-13T17:09:33.726Z
---

# Broken Windows Ledger

> Cross-phase defect register. With `workflow.windows_enforce` enabled, `/gsd-ship` blocks while `open_count > 0`.
> Waive with `gsd-tools windows waive <id> "<reason>"` (reason required).
> Mark fixed with `gsd-tools windows fixed <id>`.

| id | phase | kind | file | line | description | status | reason | recorded_at | resolved_at |
|----|-------|------|------|------|-------------|--------|--------|-------------|-------------|
| 1 | 01 | deviation | .planning/phases/01-code-rules-and-documentation/01-01-PLAN.md |  | Task 1 bootstrap TDD annotation has no valid RED commit; actual engine integration and failure controls pass, C# runner proofs belong to 01-02. | open |  | 2026-11-13T17:09:33.726Z |  |

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
  }
]
````
