---
schema_version: 1
open_count: 2
waived_count: 0
fixed_count: 6
total_count: 8
last_updated: 2026-11-13T18:21:54.853Z
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
| 5 | 01 | deviation | tests/phase1/RegisterValidator.cs |  | Strict configuration exposed existing proof-source prerequisites; added explicit imports, ordinal identity/culture handling, bounded named-capture regexes, smaller helpers and useful XML contracts; renamed methods/selectors without changing case IDs. | fixed |  | 2026-11-13T18:20:41.987Z | 2026-11-13T18:21:54.163Z |
| 6 | 01 | deviation | tools/code-rules/packages.lock.json |  | Committed canonical generated dependency locks required by shared analyzer restore ahead of plan 01-18 final lock verification, as explicitly authorized. | fixed |  | 2026-11-13T18:20:42.350Z | 2026-11-13T18:21:54.509Z |
| 7 | 01 | deviation | tools/phase1/bootstrap.cjs |  | Updated the exact engine package closure and mapped feed copy for three new pinned global analyzers; missing, unexpected and wrong-version closures remain rejected. | fixed |  | 2026-11-13T18:20:42.720Z | 2026-11-13T18:21:54.853Z |
| 8 | 01 | stub | tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj |  | Intentional empty custom analyzer assembly establishes tooling-only build wiring; rule source families belong to plans 01-05 through 01-07 and are not credited as enforcement. | open |  | 2026-11-13T18:20:43.090Z |  |

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
  },
  {
    "id": 5,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/RegisterValidator.cs",
    "line": null,
    "description": "Strict configuration exposed existing proof-source prerequisites; added explicit imports, ordinal identity/culture handling, bounded named-capture regexes, smaller helpers and useful XML contracts; renamed methods/selectors without changing case IDs.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T18:20:41.987Z",
    "resolved_at": "2026-11-13T18:21:54.163Z",
    "milestone": "v0.1"
  },
  {
    "id": 6,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/code-rules/packages.lock.json",
    "line": null,
    "description": "Committed canonical generated dependency locks required by shared analyzer restore ahead of plan 01-18 final lock verification, as explicitly authorized.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T18:20:42.350Z",
    "resolved_at": "2026-11-13T18:21:54.509Z",
    "milestone": "v0.1"
  },
  {
    "id": 7,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/bootstrap.cjs",
    "line": null,
    "description": "Updated the exact engine package closure and mapped feed copy for three new pinned global analyzers; missing, unexpected and wrong-version closures remain rejected.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T18:20:42.720Z",
    "resolved_at": "2026-11-13T18:21:54.853Z",
    "milestone": "v0.1"
  },
  {
    "id": 8,
    "kind": "stub",
    "phase": "01",
    "file": "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj",
    "line": null,
    "description": "Intentional empty custom analyzer assembly establishes tooling-only build wiring; rule source families belong to plans 01-05 through 01-07 and are not credited as enforcement.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-11-13T18:20:43.090Z",
    "resolved_at": null,
    "milestone": "v0.1"
  }
]
````
