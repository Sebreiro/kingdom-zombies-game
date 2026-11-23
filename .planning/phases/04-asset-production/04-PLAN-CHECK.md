# Phase 04 pre-execution plan check

## ISSUES FOUND

Checked 24 plans / 54 tasks / 19 waves. This is plan review, not implementation evidence. No application, export, listening or rendered acceptance was executed.

The plans preserve actual media production, reusable composition, complete baseline authoring, independent backstops, honest listening/judgment, portable export and the complete-phase review-before-window boundary. All 24 structure queries returned valid, with 2–3 tasks per plan. Both supplied deterministic command probes contain 131 rows, zero blockers/warnings and null readError; pending creation is not treated as a defect. The sampled calibrated estimate check returned a 100,000-token budget and low confidence (zero completed actual samples); plan estimates range 26,000–47,000.

### Structured issues

```yaml
issues:
  - plan: null
    dimension: research_resolution
    severity: blocker
    required_property: "Phase research carries no unresolved open question before execution."
    description: "04-RESEARCH.md has an unqualified '## Open Questions' with four entries and no RESOLVED markers: template acquisition/export, actual listening, logical density, and donor dependency closure. Its template-acquisition uncertainty is also stale against research-evidence/export-template-acquisition.json. Plans 02/04 schedule production proofs, but the research does not distinguish settled implementation decisions from remaining execution evidence."
    fix_hint: "Reconcile research with the actual acquisition record and settled production routes; distinguish unperformed acceptance proofs from research questions and resolve any genuinely undecided route without claiming those proofs passed."
  - plan: "04-03"
    task: 3
    dimension: dependency_correctness
    severity: blocker
    required_property: "Each exact native-source-policy gate has current permissions for the actual native methods it examines."
    description: "Task 2 authorizes actual observed inspection members. Task 3 subsequently creates InspectionInput.cs and InspectionShell.cs and changes InspectionRoot.cs, while explicitly recording required tuples for 04-23 reconciliation. The plan-level verification nevertheless runs engine.source-policy after Task 3. Exact path/type/member authorization cannot be supplied by tuple metadata alone; the policy update must precede that gate."
    fix_hint: "Provide a bounded exact-permission update and its negative controls after the Task 3 native subjects exist and before the plan-level source-policy gate, preserving exact ownership and avoiding blanket authorization."
  - plan: null
    plans: ["04-23", "04-24"]
    dimension: scope_sanity
    severity: blocker
    required_property: "Each execution plan remains below the checker’s 15-file blocker threshold."
    description: "Both 04-23 and 04-24 declare 15 distinct files_modified. Their three tasks and five-file task limits satisfy task bounds but do not satisfy the separate per-plan file bound."
    fix_hint: "Split final census/policy, runner/export, and review/evidence reconciliation into bounded dependent slices while retaining all obligations and the review-before-window order."
  - plan: null
    plans: ["04-01", "04-03", "04-13", "04-14", "04-21"]
    dimension: scope_sanity
    severity: warning
    required_property: "Plans above the 10-file warning threshold receive scope refinement."
    description: "files_modified counts are respectively 10, 12, 11, 12 and 12. These exceed the checker’s normal 5–8-file target and meet its warning threshold despite bounded individual tasks."
    fix_hint: "Separate independently verifiable production or policy/theme refinements into dependent slices where this reduces execution context without reducing delivery."
```

Three blocker entries and one warning entry require revision. The remediation examples are non-binding; satisfying the stated properties by another valid mechanism addresses the findings.
