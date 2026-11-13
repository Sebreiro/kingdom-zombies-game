---
schema_version: 1
open_count: 3
waived_count: 0
fixed_count: 20
total_count: 23
last_updated: 2026-11-13T21:25:19.535Z
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
| 8 | 01 | stub | tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj |  | Four real custom analyzers are implemented and proven in 01-05; eight remaining source families belong to 01-06 and 01-07 and are not credited as enforcement. | fixed |  | 2026-11-13T18:20:43.090Z | 2026-11-13T21:12:32.380Z |
| 9 | 01 | unmet-truth | tools/phase1/required-cases.json |  | Five pins runner-edge identities declared to 01-02 remain pending; 01-14 must bind distinct actual cases before full phase acceptance. | open |  | 2026-11-13T18:47:31.762Z |  |
| 10 | 01 | deviation | tools/phase1/required-cases.json |  | Plan 01-05 authored family selectors are absent from the independent inventory; exact required rule IDs and distinct captured edge IDs supply the proof selections. | fixed |  | 2026-11-13T19:21:03.747Z | 2026-11-13T19:21:22.539Z |
| 11 | 01 | deviation | tools/code-rules/NamedArgumentAnalyzer.cs |  | Inherited named-argument analyzer reported compiler-synthesized collection construction arguments; ignore implicit operations while explicit collection arguments remain checked by the real Roslyn regression. | fixed |  | 2026-11-13T19:22:07.026Z | 2026-11-13T19:22:19.339Z |
| 12 | 01 | deviation | tests/phase1/ConfigurationTests.cs |  | Active inherited analyzers exposed handwritten call/comment and verifier construction prerequisites; sources and throwaway fixture subjects now comply, and the compile-only test owns a minimal explicit entry point. | fixed |  | 2026-11-13T19:22:19.538Z | 2026-11-13T19:23:02.368Z |
| 13 | 01 | deviation | docs/code/generated-callsite-adaptations.json |  | Observed pinned xUnit/Microsoft Testing Platform and Godot generated members require exact SCR0001 symbol permissions; all ten are registered with source/package origin and adjacent/collision controls, with no blanket generated exemption. | fixed |  | 2026-11-13T19:22:19.745Z | 2026-11-13T19:23:02.564Z |
| 14 | 01 | deviation | tools/phase1/required-cases.json |  | Independent input identity omitted the previously empty analyzer project; owned analyzer sources are now included and isolated copied-source mutation rejects old evidence with INPUT_MISMATCH while cache output remains excluded. | fixed |  | 2026-11-13T19:22:19.942Z | 2026-11-13T19:23:02.760Z |
| 15 | 01 | deviation | tools/phase1/required-cases.json |  | Plan01-06 authored family6 selectors are absent; exact independently bound rule and captured-case IDs supply real verification, with no fabricated selector pass. | fixed |  | 2026-11-13T20:02:17.681Z | 2026-11-13T20:03:53.296Z |
| 16 | 01 | deviation | tests/phase1/ConfigurationTests.cs |  | New null and layout enforcement required explicit handwritten null checks, compliant wrapped source and member ordering, and formatting of compile-only fixture subjects; severities and approved requirements remain unchanged. | fixed |  | 2026-11-13T20:02:17.873Z | 2026-11-13T20:04:20.561Z |
| 17 | 01 | deviation | docs/code/generated-layout-adaptations.json |  | Observed pinned generated layout requires four exact immutable source/path/line entries and seven exact diagnostic/member permissions; actual origin, modified-content, wrong-path, adjacent and existing collision proofs bound these permissions. | fixed |  | 2026-11-13T20:02:18.059Z | 2026-11-13T20:06:31.089Z |
| 18 | 01 | deviation | tests/phase1/Verification/InheritedAnalyzerVerifier.cs |  | Actual same-process consumer builds exposed Windows DLL locks; loading analyzer bytes preserves actual diagnostic behavior without holding the output file open. | fixed |  | 2026-11-13T20:02:18.253Z | 2026-11-13T20:04:20.744Z |
| 19 | 01 | deviation | tools/phase1/required-cases.json |  | Plan01-07 authored family7 selectors are absent; exact distinct required rule and captured-case IDs provide real reconciliation, without fabricated selector passes. | fixed |  | 2026-11-13T20:36:42.706Z | 2026-11-13T21:12:32.744Z |
| 20 | 01 | deviation | tests/phase1/ConfigurationTests.cs |  | SCR0009/SCR0010 enforcement exposes handwritten continuation and import prerequisites; corrected source layout, nullable fixture contracts and unused fixture variables while preserving literal values and diagnostic scopes. | fixed |  | 2026-11-13T20:36:42.897Z | 2026-11-13T21:12:33.089Z |
| 21 | 01 | deviation | tests/phase1/Analyzers/ContinuationIndentAnalyzerTests.cs |  | Captured formatter compatibility proof uses the SDK-bundled dotnet format whitespace folder verifier instead of adding a Workspaces package; original agreed sample remains unchanged by formatting. | fixed |  | 2026-11-13T20:36:43.087Z | 2026-11-13T21:12:33.448Z |
| 22 | 01 | deviation | docs/code/generated-syntax-adaptations.json |  | Whole-solution build requires one Core qualification fix, handwritten Godot continuation layout fixes, and two exact Godot syntax permissions bounded by diagnostic, full content hash, path and span with actual origin, mutation, adjacency and impostor proofs. | fixed |  | 2026-11-13T20:56:19.761Z | 2026-11-13T21:12:33.814Z |
| 23 | 01 | unrun-verify | tests/phase1/EnforcementTests.cs |  | 01-08 wider enforcement and exemptions remain unimplemented after three bounded prerequisite repair attempts; full suite not rerun under failed activation; safe tracer remains committed. | open |  | 2026-11-13T21:25:19.535Z |  |

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
    "description": "Four real custom analyzers are implemented and proven in 01-05; eight remaining source families belong to 01-06 and 01-07 and are not credited as enforcement.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T18:20:43.090Z",
    "resolved_at": "2026-11-13T21:12:32.380Z",
    "milestone": "v0.1"
  },
  {
    "id": 9,
    "kind": "unmet-truth",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Five pins runner-edge identities declared to 01-02 remain pending; 01-14 must bind distinct actual cases before full phase acceptance.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-11-13T18:47:31.762Z",
    "resolved_at": null,
    "milestone": "v0.1"
  },
  {
    "id": 10,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Plan 01-05 authored family selectors are absent from the independent inventory; exact required rule IDs and distinct captured edge IDs supply the proof selections.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T19:21:03.747Z",
    "resolved_at": "2026-11-13T19:21:22.539Z",
    "milestone": "v0.1"
  },
  {
    "id": 11,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/code-rules/NamedArgumentAnalyzer.cs",
    "line": null,
    "description": "Inherited named-argument analyzer reported compiler-synthesized collection construction arguments; ignore implicit operations while explicit collection arguments remain checked by the real Roslyn regression.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T19:22:07.026Z",
    "resolved_at": "2026-11-13T19:22:19.339Z",
    "milestone": "v0.1"
  },
  {
    "id": 12,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/ConfigurationTests.cs",
    "line": null,
    "description": "Active inherited analyzers exposed handwritten call/comment and verifier construction prerequisites; sources and throwaway fixture subjects now comply, and the compile-only test owns a minimal explicit entry point.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T19:22:19.538Z",
    "resolved_at": "2026-11-13T19:23:02.368Z",
    "milestone": "v0.1"
  },
  {
    "id": 13,
    "kind": "deviation",
    "phase": "01",
    "file": "docs/code/generated-callsite-adaptations.json",
    "line": null,
    "description": "Observed pinned xUnit/Microsoft Testing Platform and Godot generated members require exact SCR0001 symbol permissions; all ten are registered with source/package origin and adjacent/collision controls, with no blanket generated exemption.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T19:22:19.745Z",
    "resolved_at": "2026-11-13T19:23:02.564Z",
    "milestone": "v0.1"
  },
  {
    "id": 14,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Independent input identity omitted the previously empty analyzer project; owned analyzer sources are now included and isolated copied-source mutation rejects old evidence with INPUT_MISMATCH while cache output remains excluded.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T19:22:19.942Z",
    "resolved_at": "2026-11-13T19:23:02.760Z",
    "milestone": "v0.1"
  },
  {
    "id": 15,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Plan01-06 authored family6 selectors are absent; exact independently bound rule and captured-case IDs supply real verification, with no fabricated selector pass.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:02:17.681Z",
    "resolved_at": "2026-11-13T20:03:53.296Z",
    "milestone": "v0.1"
  },
  {
    "id": 16,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/ConfigurationTests.cs",
    "line": null,
    "description": "New null and layout enforcement required explicit handwritten null checks, compliant wrapped source and member ordering, and formatting of compile-only fixture subjects; severities and approved requirements remain unchanged.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:02:17.873Z",
    "resolved_at": "2026-11-13T20:04:20.561Z",
    "milestone": "v0.1"
  },
  {
    "id": 17,
    "kind": "deviation",
    "phase": "01",
    "file": "docs/code/generated-layout-adaptations.json",
    "line": null,
    "description": "Observed pinned generated layout requires four exact immutable source/path/line entries and seven exact diagnostic/member permissions; actual origin, modified-content, wrong-path, adjacent and existing collision proofs bound these permissions.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:02:18.059Z",
    "resolved_at": "2026-11-13T20:06:31.089Z",
    "milestone": "v0.1"
  },
  {
    "id": 18,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/Verification/InheritedAnalyzerVerifier.cs",
    "line": null,
    "description": "Actual same-process consumer builds exposed Windows DLL locks; loading analyzer bytes preserves actual diagnostic behavior without holding the output file open.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:02:18.253Z",
    "resolved_at": "2026-11-13T20:04:20.744Z",
    "milestone": "v0.1"
  },
  {
    "id": 19,
    "kind": "deviation",
    "phase": "01",
    "file": "tools/phase1/required-cases.json",
    "line": null,
    "description": "Plan01-07 authored family7 selectors are absent; exact distinct required rule and captured-case IDs provide real reconciliation, without fabricated selector passes.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:36:42.706Z",
    "resolved_at": "2026-11-13T21:12:32.744Z",
    "milestone": "v0.1"
  },
  {
    "id": 20,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/ConfigurationTests.cs",
    "line": null,
    "description": "SCR0009/SCR0010 enforcement exposes handwritten continuation and import prerequisites; corrected source layout, nullable fixture contracts and unused fixture variables while preserving literal values and diagnostic scopes.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:36:42.897Z",
    "resolved_at": "2026-11-13T21:12:33.089Z",
    "milestone": "v0.1"
  },
  {
    "id": 21,
    "kind": "deviation",
    "phase": "01",
    "file": "tests/phase1/Analyzers/ContinuationIndentAnalyzerTests.cs",
    "line": null,
    "description": "Captured formatter compatibility proof uses the SDK-bundled dotnet format whitespace folder verifier instead of adding a Workspaces package; original agreed sample remains unchanged by formatting.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:36:43.087Z",
    "resolved_at": "2026-11-13T21:12:33.448Z",
    "milestone": "v0.1"
  },
  {
    "id": 22,
    "kind": "deviation",
    "phase": "01",
    "file": "docs/code/generated-syntax-adaptations.json",
    "line": null,
    "description": "Whole-solution build requires one Core qualification fix, handwritten Godot continuation layout fixes, and two exact Godot syntax permissions bounded by diagnostic, full content hash, path and span with actual origin, mutation, adjacency and impostor proofs.",
    "status": "fixed",
    "reason": "",
    "recorded_at": "2026-11-13T20:56:19.761Z",
    "resolved_at": "2026-11-13T21:12:33.814Z",
    "milestone": "v0.1"
  },
  {
    "id": 23,
    "kind": "unrun-verify",
    "phase": "01",
    "file": "tests/phase1/EnforcementTests.cs",
    "line": null,
    "description": "01-08 wider enforcement and exemptions remain incomplete. The preserved inherited policy/source activation is verified: strict five-project solution has zero warnings/errors; all three earlier activation regressions pass. Full immutable-source run 9ab3b049-7ae7-4ef5-9e84-9e4587ad491b finished with 375 passed, zero failed controls, 184 pending, 559 declared, exit 1 solely for pending obligations. Native bootstrap and exact generated syntax origin/boundary controls pass. Task 1 expansion and Task 2 remain incomplete; 08-expand.cjs is an UNRUN draft. Exact evidence is recorded in 01-08-CHECKPOINT.md.",
    "status": "open",
    "reason": "",
    "recorded_at": "2026-11-13T21:25:19.535Z",
    "resolved_at": null,
    "milestone": "v0.1"
  }
]
````
