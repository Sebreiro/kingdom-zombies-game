# Pull request code review

Use this document as the entry point for an external PR review tool or a contributor reviewing KingdomZombies. Review the PR against its target branch, normally `dev`, and the actual changed revision. [AI collaboration](ai-collaboration.md) governs feature branches, draft/ready transitions and merging.

## Read the applicable authorities

| Review subject | Authority |
| --- | --- |
| Required game behavior and exclusions | [Version 0.1 specification](../../specs/0.1/initial-spec.md), [asset/resource plan](../../specs/0.1/assets-and-resources.md), relevant accepted requirement and phase context |
| C# formatting, naming, APIs and executable rules | [Code index](../code/README.md), [rule register](../code/register.json), [current mechanism/proof matrix](../code/rule-matrix.md) |
| Compiler/analyzer policy and banned APIs | [Workspace build policy](../../src/godot-game/build/README.md), [shared properties](../../src/godot-game/Directory.Build.props), [targets](../../src/godot-game/Directory.Build.targets), [editor configuration](../../.editorconfig) |
| Exact permitted exceptions | [Exemptions](../code/exemptions.json), generated/engine permission references selected by the [code index](../code/README.md) |
| Dependencies, state, services and engine ownership | [Architecture index](../architecture/README.md), [boundaries](../architecture/boundaries.md), [layout policy](../architecture/repository-layout.md), applicable architecture decisions |
| Judgment about design and meaning | [Qualitative checklist](../code/review-checklist.md), [fourteen exact obligations](../code/review.md) |
| Test selection, execution and evidence limits | [Verification](verification.md), [local testing strategy](local-testing.md), [setup](setup.md) |

The linked executable definitions and exact exception registers control their rules. Do not substitute personal style preferences for them, duplicate them into a divergent checklist, or relax them simply because a checker reports a violation. Captured provenance explains inherited decisions; current bindings identify their implemented mechanisms.

## Review the change

1. Read the PR description, acceptance criteria and applicable decisions. Inspect the full diff, then follow affected callers, consumers, transitive dependencies and shared inputs. Check final behavior rather than commit narration.
2. Look for correctness defects: invalid state transitions, lost data, races, cancellation/lifetime errors, unsafe parsing, ownership mistakes, protocol incompatibility and unhandled failure paths. For game or LAN changes, connect findings to an observable player/session consequence.
3. Check dependency direction, engine lifetime/main-thread access, explicit time/random inputs, snapshots and API necessity against the linked rules. Review analyzer, test, runner, manifest and exemption changes for bypasses or false-positive passes.
4. Assess names, cohesion, invariants and comments using the qualitative obligations. Check that documentation and module declarations describe actual implementation and that new files follow the layout policy.
5. Inspect relevant tests and executed evidence. Follow the documented verification selection; report unexecuted, failed, blocked, stale or missing checks accurately. An exit code, configured workflow, synthetic input or headless run cannot supply a missing rendered/LAN/platform/human pass.

Review implementation and fix findings before launching rendered scenarios. Ordinary repository/build PRs use strict solution build/analyzers, project/code boundaries and documentation/catalog validation; exhaustive negative-control suites, complete LAN runs and coverage certification have their own selections. Do not invent a new blanket full-suite requirement or infer that an unchanged folder is unaffected.

## Report actionable findings

Give each finding a concise severity/title, a precise changed file/line, the triggering conditions, the concrete consequence and the applicable requirement or rule link. Explain a plausible defect using evidence from the code; distinguish confirmed behavior from an assumption that still needs proof. Suggest a correction when useful, without prescribing unnecessary redesign.

Use P0 for an immediate critical issue, P1 for a high-impact defect requiring prompt correction, P2 for a normal actionable defect, and P3 for a minor actionable issue. Keep uncertain questions and optional design suggestions separate from defect findings. Avoid reporting formatting preferences already settled by project policy or repeating the same underlying defect at many call sites.

Conclude the review with findings, checks actually executed and material gaps. If there are no actionable findings, say so and still state verification limits. Automated/AI review never marks separately required human qualitative components as passed, fills in a human reviewer identity, authorizes merging or claims unperformed platform acceptance. Preserve historical failures and report identities.
