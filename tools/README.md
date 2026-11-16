# Reusable development tools

This directory holds product-independent tooling. The [code-rules package](code-rules/docs/README.md) implements twelve Roslyn C# syntax/style analyzers and has no game assembly dependencies. Its build explicitly imports this repository's shared compiler/package settings; the analyzer implementations remain reusable outside KingdomZombies.

Game-specific tools belong with the game code in [src/godot-game/tools](../src/godot-game/tools/README.md). That guide covers verification, local gameplay checks, LAN checks and shared launch helpers. Their self-tests live beside them. Do not add game-only programs or phase-numbered tool folders at this root.

See the [repository layout policy](../docs/architecture/repository-layout.md) for ownership and migration, and [PR review](../docs/workflow/code-review.md) for rules and evidence requirements.
