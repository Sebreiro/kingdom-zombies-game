# Local testing strategy

Creator-approved on 2026-11-15 (Europe/Paris), including the Phase 5 discussion. This is the current policy for routine local verification in every phase, starting immediately with the active Phase 2. It supersedes conflicting older platform, resolution, per-task game-launch and phase-end rerun instructions. Required game behavior and separate release acceptance remain defined by the specifications.

## Phase workflow

1. Implement the whole phase before opening a real game window. Builds, analyzers, pure tests, repository checks and other checks without a real game window may run during implementation. Do not add rendered gates between small fixes, tasks or plans.
2. Review the complete phase code and fix the findings before rendered testing.
3. Declare the selected scenarios and their game-run groups before execution. Run only on the development operating system, without starting a VM. Every routine local game window uses **1280×720**.
4. Run as many compatible scenarios as practical in one game process. Use ordinary menu/session controls to reset state between scenarios, preserve scenario independence and run destructive exit checks last. For multiplayer, one group may require a host and several guests; reuse that process set for compatible scenarios. Do not reduce required participants to achieve one window.
5. Collect scenario failures and logs, then continue independent scenarios while the process remains usable. Do not stop to edit code or relaunch after each assertion failure. A crash or unusable state blocks remaining dependent scenarios and is recorded separately from a pass.
6. Fix the collected problems together. Rerun failed or blocked scenarios, grouped into as few launches as practical. Include newly affected scenarios if a repair changes their behavior. Repeat the repair cycle until these pass.
7. After the focused repair checks pass, rerun the **entire selected scenario set** on the repaired build. Prefer one game run; otherwise use the declared compatible groups. If this final pass finds another problem, repeat the repair cycle and final selected-set pass. The final regression pass is required after repairs; combining earlier passes with focused repair results alone does not replace it.

When the initial selected-set run passes without repairs, it already supplies the final selected-set pass; do not launch it again merely to duplicate the result. Keep builds and prerequisite checks separate from game-window runs.

## Selection and importance

Use a smaller phase-end selection **immediately**, before nightly testing exists. Always select:

- Every active **Critical** case, including regressions from earlier phases.
- Every new or changed case in the phase, regardless of importance.
- Every regression affected by the implementation or its repairs, regardless of importance, including consumers, transitive dependencies and shared inputs.

These obligations are cumulative. An unchanged folder alone does not prove a case unaffected. Resolve uncertain impact before narrowing the selection. A broad shared change can select the full applicable suite. Priority never removes a case from the catalog, changes its required/planned lifecycle or supplies a pass for an unexecuted case.

| Importance | Meaning | Routine selection |
| --- | --- | --- |
| Critical | A failure prevents starting or progressing, crashes the game, corrupts or loses possessions/saves, duplicates authority/state, or breaks essential control/session integrity. | Always, once the feature is active. |
| High | A major supported player flow or important gameplay behavior fails. | Whenever new, changed or affected. |
| Normal | Standard behavior, boundaries and recoverable failure paths. | Whenever new, changed or affected. |
| Low | Lower-impact presentation or rare, nonblocking details. | Whenever new, changed or affected. |

Assign importance from the consequence of failure, with a short rationale. A boundary case that can corrupt data is Critical; do not rank all edge cases Low. An unimplemented future case stays planned even when its importance is Critical.

Record the selected and omitted IDs with reasons before testing. Report unselected cases as **not selected for this phase run**, without pass/skip credit. Distinguish them from selected failures, blockers, invalid evidence and future planned work. This reduced selection does not certify the complete catalog. Before Phase 11, lower-priority unaffected cases can remain unexecuted under this explicit policy; no nightly result is presumed.

## Scenarios saved by every phase

Every phase must add or update repository-stored, AI-readable test scenarios alongside its feature, including phases before Phase 5. Preserve stable case IDs and link each scenario to the existing requirement/source acceptance ID and owning phase. Do not renumber A-01–A-47.

Each scenario records:

- Its title, importance and rationale, required/planned status, and feature/dependency tags used for impact selection.
- Preconditions, required environment/process roles, reproducible setup or seed, ordered production input/actions, and explicit expected results.
- Required observations, logs and image/review checkpoints, plus the command or repeatable procedure an agent follows.
- A persistent game-run group ID, its order within that group, and how to reset or recover through ordinary controls before the next scenario.
- Any need for a fresh process or incompatible setup, with a concrete reason, and bounded waits/owned cleanup.

Each game-run group lists its ordered scenarios, shared launch/setup, process roles, resets and exit behavior. A pure/build case declares that it needs no game window. Store definitions in the owning phase's tracked scenario document or catalog and link them from its context/verification record. Later phases consume these definitions and add regressions instead of relying on chat memory. Phase 5 integrates the existing phase catalogs, validates metadata and supplies shared selection/grouping/report mechanisms; writing scenarios does not wait for that framework.

Keep group identity distinct from an execution's invocation identity. Shared process/logs are allowed, but every scenario retains its own outcome and evidence references. Use fresh scenario state inside a reused process. Never silently add a launch per case; record why additional groups/processes are necessary.

## Evidence and broader verification

Preserve original reports, tested revisions, invocation IDs, logs, images, failures and repair attempts until the task ends. Then record their results and reproduction steps and delete the local files, as [workspace cleanup](cleanup.md) requires. Link initial, focused repair and final selected-set runs. The report shows selection reasons, importance, run-group membership, launches/process roles, results, review status and omissions. Log collection does not turn an assertion failure into success.

Phase 5 still implements dependency-aware selection and evidence validation. Earlier passing evidence may remain useful history or support explicitly eligible unchanged checks, but it cannot replace the fresh final rendered selected-set pass after repairs. Existing commands are only those documented for delivered tooling; this policy does not invent a priority selector or certify an unfinished reuse mechanism.

Hosted Windows/Linux CI remains separate from routine local testing. Basic PR checks still target about ten minutes per OS with cached dependencies and perform build/analyzer/boundary/documentation/catalog checks without launching the game. Actual hosted full-workflow compatibility remains Phase 5 work; a local other-OS VM is no longer a phase-completion requirement. Other-OS execution can be established by its real hosted/native environment, never by a Windows cross-build.

The supported Windows/Linux product, required display support, actual mixed-computer/mixed-OS LAN, reference-PC 1920×1080 stress and real-duration endurance/release procedures retain their distinct owners. Catalog outstanding evidence under those hosted or release procedures; do not require a local VM or an extra routine local resolution as Phase 5 catch-up. Historical missing evidence remains unperformed until its own procedure is executed.

## Phase 11: nightly full regression testing

Phase 11 introduces scheduled full-suite execution across all active importance levels, with the complete catalog, existing run groups and inspectable results/artifacts. Planned future cases receive no pass credit. Nightly full runs complement the reduced phase-end selection above, which starts now. Preserve manual full execution and distinct release procedures. The schedule, execution environment and runner availability are Phase 11 planning decisions; no nightly automation is created by this policy discussion.
