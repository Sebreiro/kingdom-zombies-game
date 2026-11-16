# Game tools - Context

Gathered 2026-11-16. Follow-up to quick task 261116-k3u, on its existing feature branch and PR #6.

## Creator decisions

The creator now explicitly requests purpose-based tool-folder names and placement of game-only tools beside the game code and other tools. This supersedes the earlier decision to explain tooling before renaming it; the earlier completion records remain historical.

Move game-specific programs and their self-tests under `src/godot-game/tools`. Use `verification` for code/architecture/documentation/toolchain checks, `gameplay-checks` for menu/movement/local-game checks, and `lan-checks` for multiplayer/process/evidence checks. Place shared .NET launch and layout helpers at that tools root. Keep the reusable, game-independent analyzer package at root `tools/code-rules`.

Keep existing case/protocol identities and immutable report/cache locations. Do not rename namespaces or evidence IDs solely because they contain historical phase names. Continue in `feature/quick-repository-layout` and the same PR #6 to dev, returning it to draft during this work. Preserve the separate active Phase 4 checkout. Its eventual asset tooling should use `src/godot-game/tools/asset-pipeline` when integrated.

## Discretion and references

Classification amendment after source inspection: `GeneratedSyntax.cs` and `EngineGeneratedSyntax.cs` embed exact KingdomZombies/Godot generated-source permissions. The core analyzer rules are reusable, but this configured copy is game-specific. Under the creator's ownership rule, it also moves to `src/godot-game/tools/code-rules`; no current root tools folder remains. This supersedes the provisional analyzer exception above, without changing its assembly identity or runtime dependencies.

Resolve module/entry-point roots, fixture program paths, fingerprint scopes, live declarations and documentation together. Do not leave forwarding scripts at the old phase-named paths. Preserve original reports and completed phase/quick-task artifacts rather than rewriting their evidence. See AGENTS.md, docs/workflow/ai-collaboration.md, docs/workflow/verification.md, and docs/architecture/repository-layout.md.
