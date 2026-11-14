# Phase 5: CI and repeatable game testing - Discussion Log

> **Audit trail only.** Do not use as planning/research/execution input. Current decisions are in CONTEXT.md; earlier selections below may be superseded.

**Date:** 2026-11-14 (Europe/Paris)
**Phase:** 05-ci-and-repeatable-game-testing
**Areas discussed:** Failure reports/reproduction; PR selection; visual-review handoff; test groups/feedback time.

## Failure reports and reproduction

### Question 1

When a check fails, what should appear first in its report? Every option still includes the required results, logs, images, and run identity.

| Option | Description | Selected |
| --- | --- | --- |
| Failure brief (Recommended) | Show the failed case, expected versus observed behavior, key evidence links, and the replay command first. | Yes |
| Run overview | Start with totals and the complete case table, then open individual failures. |  |
| Requirement overview | Start with affected requirements and scenarios, then show their failed checks. |  |

**User’s choice:** Failure brief — Show the failed case, expected versus observed behavior, key evidence links, and the replay command first.

### Question 2

How should an agent normally reproduce a failed case? Each replay uses the recorded setup, seed, timing inputs, and actions, and creates a fresh report.

| Option | Description | Selected |
| --- | --- | --- |
| One replay command (Recommended) | A command linked to the failed attempt restores the scenario inputs and runs that case again. | Yes |
| Step-by-step commands | The report lists separate commands for setup, launch, actions, and inspection so the agent can work through them. |  |

**User’s choice:** One replay command — A command linked to the failed attempt restores the scenario inputs and runs that case again.

### Question 3

When several checks fail for the same diagnosed cause, how should the report present them? Every affected case keeps its own result and evidence.

| Option | Description | Selected |
| --- | --- | --- |
| Group by cause (Recommended) | Show one failure brief for the shared cause, with all affected cases and their evidence listed beneath it. | Yes |
| List every case separately | Give each failed case its own brief, even when the diagnosis is repeated. |  |

**User’s choice:** Group by cause — Show one failure brief for the shared cause, with all affected cases and their evidence listed beneath it.

### Question 4

How much diagnostic evidence should appear directly in a failure brief? Full logs and observations remain linked in either layout.

| Option | Description | Selected |
| --- | --- | --- |
| Short excerpts (Recommended) | Include the relevant log lines and a compact expected-versus-observed state comparison. | Yes |
| Links only | Keep the brief very short and open the linked files for diagnostic detail. |  |

**User’s choice:** Short excerpts — Include the relevant log lines and a compact expected-versus-observed state comparison.

## Pull-request check selection

### Question 1

For a behavior change with known related checks, how broadly should the fast PR run test it? The strict build, rules, architecture, and documentation checks always run.

| Option | Description | Selected |
| --- | --- | --- |
| Whole affected subsystem (Recommended) | Run its related behavior checks together, such as the LAN group for a networking change. | Yes |
| Only directly mapped cases | Run the specific cases linked to the change and their declared dependencies; this is narrower but requires more detailed mapping. |  |

**User’s choice:** Whole affected subsystem — Run its related behavior checks together, such as the LAN group for a networking change.

### Question 2

If the affected behavior cannot be identified confidently, what should the PR workflow do? It must not silently accept a narrow selection.

| Option | Description | Selected |
| --- | --- | --- |
| Run the full required suite (Recommended) | Expand verification automatically and report why the broader run was selected. |  |
| Report a selection blocker | Stop with an explicit explanation until the affected scope is identified. | Yes |

**User’s choice:** Report a selection blocker — Stop with an explicit explanation until the affected scope is identified.

### Question 3

For changes to shared input, state, or scene-lifetime code, how broadly should behavior checks run when its consumers are known?

| Option | Description | Selected |
| --- | --- | --- |
| All delivered behavior groups (Recommended) | Check the playable scene, LAN, and content fixtures together because they share the foundation. | Yes |
| Known consuming groups | Run the complete checks for each documented consumer, leaving unrelated groups out. |  |

**User’s choice:** All delivered behavior groups — Check the playable scene, LAN, and content fixtures together because they share the foundation.

### Question 4

How should the PR summary explain which checks were selected and omitted? The exact case lists and selection reasons are recorded in either option.

| Option | Description | Selected |
| --- | --- | --- |
| Compact summary with links (Recommended) | Show subsystem counts and reasons, linking to the complete selected and omitted case lists. | Yes |
| Complete lists inline | Include every selected and omitted case directly in the summary, even when the report is long. |  |

**User’s choice:** Compact summary with links — Show subsystem counts and reasons, linking to the complete selected and omitted case lists.

**Later amendment:** Whole affected-subsystem/all shared-foundation behavior selections for PRs were superseded by the creator’s basic-PR-only instruction and final explicit no-game-launch choice in Test groups and feedback time. Unknown-impact blocking and compact selection summaries remain selected.

## Visual-review handoff

### Question 1

How should the report organize screenshots for appearance and readability review? Original images remain directly accessible in either layout.

| Option | Description | Selected |
| --- | --- | --- |
| By scenario (Recommended) | Keep each case’s before/after images together, with both required resolutions and clear peer labels. | Yes |
| By screen or asset category | Group menu, gameplay, characters, buildings, and other images across cases for visual comparison. |  |

**User’s choice:** By scenario — Keep each case’s before/after images together, with both required resolutions and clear peer labels.

### Question 2

How should screenshot groups appear in the report? Reviewers can open the original full-resolution captures in either option.

| Option | Description | Selected |
| --- | --- | --- |
| Compact previews with links (Recommended) | Show labeled before/after previews and link each image to its original for close inspection. | Yes |
| Full-size images inline | Place the original-size captures directly in the report, making it longer to scroll. |  |

**User’s choice:** Compact previews with links — Show labeled before/after previews and link each image to its original for close inspection.

### Question 3

How should the AI record its visual-review conclusions? Both options must identify the inspected images, findings, defects, and review outcome.

| Option | Description | Selected |
| --- | --- | --- |
| Short structured notes (Recommended) | Give each scenario a compact record of appearance, readability, inspected resolutions/peers, defects, and outcome. | Yes |
| Free-form review notes | Write a narrative assessment for each scenario, linking the inspected images and recording the same required facts. |  |

**User’s choice:** Short structured notes — Give each scenario a compact record of appearance, readability, inspected resolutions/peers, defects, and outcome.

### Question 4

When many screenshot groups need review, which should appear first? Every required review remains listed until completed.

| Option | Description | Selected |
| --- | --- | --- |
| Problems first (Recommended) | Lead with failed objective checks or recorded visual defects, then show the remaining unreviewed scenarios. | Yes |
| Stable scenario order | Keep the catalog’s scenario order so repeated reviews follow a consistent sequence. |  |

**User’s choice:** Problems first — Lead with failed objective checks or recorded visual defects, then show the remaining unreviewed scenarios.

## Test groups and feedback time

### Question 1

What time target should we design for an ordinary PR run on each OS with dependencies cached?

| Option | Description | Selected |
| --- | --- | --- |
| About 5 minutes (Recommended) | Aim for 3–5 minutes for normal PRs, with heavier tooling changes and full certification identified separately. |  |
| About 3 minutes | Prioritize a very small fast selection and make runner optimization a stronger constraint. |  |
| About 10 minutes | Allow broader PR checking with less pressure to reduce setup and execution costs. | Yes |

**User’s choice:** About 10 minutes — Allow broader PR checking with less pressure to reduce setup and execution costs.

### Question 2

When should the expensive proofs that deliberately break sample projects and the test runner execute?

| Option | Description | Selected |
| --- | --- | --- |
| When tooling changes, plus full verification (Recommended) | Run the relevant proof groups for analyzer/build/test-tool changes; run every required proof before phase completion and in manual full runs. |  |
| Every PR | Include all deliberate-defect proofs in ordinary PRs too, and optimize toward the 10-minute target. |  |

**User’s choice:** PR contains only build and other basic checks - to fit in 10 minutes. Full test could be run at the end of phase. Also, we could split by folders (if content of the folder was not changed - don't need to test) or something. Also, sometimes at the end of the phase we don't need a full retest.

### Question 3

At phase completion, should we reuse passing evidence for unchanged test groups and rerun only changed groups plus their dependents? Reuse would require matching code/assets, test definitions, relevant tools, and required environments; uncertain impact remains a blocker.

| Option | Description | Selected |
| --- | --- | --- |
| Reuse unchanged groups (Recommended) | Assemble complete phase evidence from valid earlier results and fresh affected-group results. Broad shared changes can still require a full run. | Yes |
| Fresh complete run | Rerun the whole required scope at phase completion, unless a complete passing run already covers the final unchanged inputs. |  |

**User’s choice:** Reuse unchanged groups — Assemble complete phase evidence from valid earlier results and fresh affected-group results. Broad shared changes can still require a full run.

### Question 4

Which checks should an ordinary PR include within the 10-minute target?

| Option | Description | Selected |
| --- | --- | --- |
| Build and repository checks (Recommended) | Strict build with analyzers, current project/code boundary checks, and documentation/catalog validation; no game launch. | Yes |
| Add a tiny game smoke check | The same basic checks plus one short headless startup/action check, without screenshots or four game processes. |  |

**User’s choice:** Build and repository checks — Strict build with analyzers, current project/code boundary checks, and documentation/catalog validation; no game launch.

## Flow and clarification record

| Step | Choices / response | Outcome |
| --- | --- | --- |
| Initial areas | Failure reports; PR selection; visual-review handoff; All three areas | All three areas selected |
| Failure area check | Next area / More questions | Next area |
| PR area check | Next area / More questions | Next area |
| Initial finish check | Ready for context / Explore more | Creator instead requested test-group descriptions/timings, short CI and an explanation of before/after screenshots |
| Final finish check | Ready for context, including amended requirements / Explore more | Ready: write context/log and align conflicting Phase 5 verification requirements |

The creator’s free-text request at the initial finish check was: “I want to clarify each tests group and it's approximate time. CI github action tests should not take a lot of time. IF we have before after screenshots, does it mean AI should run game before and after changes? If we would run on each small feature - it would take ages to finish. Let's think about the way to make tests as fast as possible”.

The agent explained that before/after captures are scenario checkpoints in one current-build run. It inspected the retained 676-case Windows report rather than running a new exhaustive suite, reported the 51m36s historical command window and group costs, and identified sequential per-case processes and repeated isolated builds. It proposed approximately 3–5-minute PR feedback; the creator selected about ten minutes instead. No proposed future timing or optimization was represented as measured.

## Superseded decisions and authority amendments

- Later explicit basic PR scope replaces the earlier whole affected-behavior/all shared-foundation PR selections. Ordinary PRs perform strict build/analyzers, current boundaries and docs/catalog validation, with no game launch.
- The explicit unchanged-group reuse selection replaces mandatory repetition of all phase-end tests. Required case/environment membership, actual full-workflow compatibility, coverage thresholds, eight transferred IDs and immutable original provenance remain binding.
- Current requirements/specification and indexed verification policy are amended; historical reports, case IDs and original interview answers are not rewritten as new evidence.
- The ten-minute figure is the chosen approximate warm PR target per OS. Exact future-group budgets, cold setup time, concurrency and implementation details were not selected.

## Agent’s Discretion

No “you decide” product preference was selected. Research/planning chooses exact test tooling, grouping/dependency and reuse mechanisms, report/CLI schemas and compatible optimization techniques within the amended contract. Candidate caching/build/batching/parallelism advice is not an approved tool choice or measured speedup.

## Deferred Ideas

None added; existing later feature and release-environment ownership is preserved.
