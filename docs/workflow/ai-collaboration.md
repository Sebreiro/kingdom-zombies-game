# AI Collaboration

This document records the creator's preferences for AI agents working on KingdomZombies. The root [agent guide](../../AGENTS.md) indexes these instructions.

## Interactive Questions

- Use the available interactive question controls when asking the creator for information, preferences, or decisions. Do not substitute a numbered chat list when interactive controls are available.
- Offer clear, concise choices when useful, and allow a free-text answer. If the control supports only one selection, allow the creator to name several choices in the free-text answer when the question permits multiple selections.
- Use ordinary chat questions when interactive controls are unavailable or the creator explicitly requests a text response.
- Keep GSD's `workflow.text_mode` set to `false` for this preference. A workflow's text-mode default does not override the creator's explicit request for interactive questions.

## Development and Phase-End Testing

- During implementation, run relevant checks on the operating system where the agent is developing.
- Before finishing a phase, account for its complete required scope on that development OS and in a local VM running the other supported OS: Linux when developing on Windows, or Windows when developing on Linux.
- Use hardware acceleration for the local VM when available. Record actual execution and results; a prepared VM or cross-build does not establish a pass.
- Preserve explicitly deferred later-phase work and its ownership. A complete phase run selects the current phase's independently declared required scope, without crediting pending later-phase cases.
- Batch related repairs with focused feedback. Under the Phase 5 reuse policy below, rerun missing/invalidated groups and their dependents after implementation stabilizes; retain valid unchanged evidence. Broad shared changes can require the full scope again.

The creator established these platform obligations on 2026-11-14 and subsequently approved the execution/reuse amendment below. Required environments remain distinct. See [verification](verification.md) for current commands and evidence requirements.

## Phase 5 Execution and Evidence Reuse Amendment

- Ordinary Windows/Linux PRs target about ten minutes per OS with dependencies cached. Run the strict build with analyzers, current project/code boundary checks and documentation/catalog validation. Do not launch the game or include exhaustive deliberate-defect proofs, full LAN scenarios, coverage certification or AI screenshot review in this basic selection.
- During feature work, select focused groups from changed folders plus their consumers, transitive dependencies and shared configuration/build inputs. An unchanged folder alone does not establish unaffected behavior. Uncertain impact is an explicit selection blocker.
- At phase completion, reuse passing group evidence only when relevant code/assets, dependencies, test/case definitions, tools and required environment still match. Rerun missing/invalidated groups and dependents. Every required case/environment remains accounted for; planned cases receive no pass credit.
- Keep original reports, tested revisions and invocation identities immutable. Completion reports distinguish executed-now, explicitly reused and missing/invalidated evidence. Reuse validates an earlier result; it never supplies a newly executed pass or cross-platform substitution. An unresolved later failure prevents acceptance through an older pass.
- Before/after screenshots are checkpoints within a selected scenario on the current build, not obligatory runs of old/new builds for every edit. Keep real input, fresh-run evidence and declared appearance/readability review.
- Phase 5 must implement and prove dependency-aware selection and reuse validation. Until then, the current Phase 1 runner still executes selected cases afresh. This policy invents no reuse command or retroactive certification. Actual full/manual workflow compatibility still needs execution evidence.

See [Phase 5 context](../../.planning/phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md) and its [amended specification](../../.planning/phases/05-ci-and-repeatable-game-testing/05-SPEC.md). Ten minutes is an optimization target, not permission to omit selected checks. Report measured warm/cold, per-group and total times.

**One-time Phase 1 exception, later on 2026-11-14:** The creator requested no Windows restart, a few important actual Linux tests, then Phase 1 closure. The [closure exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) predeclares the focused checks and evidence limits. Preserve the full Windows result; keep the unexecuted complete Linux suite and unreviewed detailed human checklist visible as follow-ups. This exception does not change the rule for later phases or fabricate test/review passes.

See [GSD workflow](gsd.md) for phase commands and planning artifacts.
