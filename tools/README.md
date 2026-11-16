# Development tools

This directory is the repository's workshop. These programs help build, inspect and test the game; they are outside the Godot runtime resource tree and are not gameplay services.

For example, the compiler loads the code-rule analyzer while building C#. A verification runner can then start a test process, drive a game session, collect assertions/screenshots, and reject stale or incomplete reports. The game does not need that runner to handle player movement or networking.

## Existing tools

| Location | Purpose | Typical use |
| --- | --- | --- |
| [code-rules](code-rules/docs/README.md) | Twelve reusable Roslyn syntax/style analyzers. No game assembly dependencies. | Runs automatically during consumer builds; implementation changes require its own proof tests. |
| [phase1/bootstrap.cjs](phase1/bootstrap.cjs) | Verify pinned tool identities, prepare authentic engine packages and run native bootstrap proofs. | Prepare a clean checkout or diagnose toolchain compatibility. |
| [phase1/check.cjs](phase1/check.cjs) | Run exact declared code-rule, architecture, state, engine and documentation checks and validate actual evidence. | Check a focused group or named case while changing the repository. |
| [phase2/check.cjs](phase2/check.cjs) | Test the playable menu/movement foundation. Companion scripts drive input and reconcile screenshots/reports. | Run foundation cases selected for the current change. |
| [phase3/check.cjs](phase3/check.cjs) | Test LAN behavior, coordinate processes and validate timing, input and image evidence. | Run selected multiplayer cases or validate completion evidence. |
| [dotnet.cjs](dotnet.cjs) | Select the game workspace's SDK configuration while preserving explicit subject paths. | Used internally by runners; it is not a separate build command. |
| [repository-layout.test.cjs](repository-layout.test.cjs) | Guard solution placement and automated SDK working-directory behavior. | `node --test tools/repository-layout.test.cjs` |

The `phase1`, `phase2` and `phase3` names record their delivery origin. They remain stable command paths in this cleanup because existing tooling and evidence refer to them. A future rename should be discussed separately. Asset-production tools on the unfinished Phase 4 branch will need the [layout migration](../docs/architecture/repository-layout.md#historical-paths) when that branch integrates; this dev-based branch does not claim to include that work.

## Tests and output

Game C# tests live under [src/godot-game/tests/verification](../src/godot-game/tests/verification/docs/README.md). Node `*.test.cjs` files beside a runner test that runner itself, such as selection, revision binding or process cleanup. They are development-tool tests.

Ignored `.phase1/`, `.phase2/` and `.phase3/` directories at the repository root hold caches, logs and immutable run reports. They are generated local state. Keep old reports and identities intact; do not move or rewrite them to suggest fresh execution. Normal `bin/`, `obj/` and `.godot/` output is ignored too.

Run commands from the repository root. Start with [verification](../docs/workflow/verification.md) for case selection and report interpretation, [setup](../docs/workflow/setup.md) for dependencies, and [code review](../docs/workflow/code-review.md) when assessing a PR. A checker exit code alone does not establish game acceptance.
