---
task_id: 261116-m0w
status: complete
completed: 2026-11-16
implementation_revision: 4ea92b5a849df34e695da23746ba6f09605096f5
branch: feature/quick-repository-layout
pull_request: https://github.com/Sebreiro/kingdom-zombies-game/pull/6
---

# Game-specific tools organized by purpose

All current tools now live under `src/godot-game/tools`: `verification`, `gameplay-checks`, `lan-checks`, `code-rules`, and the shared .NET/layout helpers. The repository root tools tree and old phase-numbered entry points are removed; no forwarding stubs remain.

The configured analyzer moved too. Its core syntax rules are reusable, but this copy contains exact KingdomZombies/Godot generated-source permissions. Its sixteen implementation files and package lock retain identical bytes, and it still has no game assembly dependency. All five C# projects now inherit compiler/package policy normally within the workspace, so its external-project import overrides were removed.

Rebound module/checkout roots, cross-folder requires, C# child fixtures, SourceLink target roots, actual catalogs, live scenario/selection paths and non-overlapping source fingerprints. Historical protocol/case IDs and cache/report locations remain stable. Current guides and module declarations reflect the new ownership; completed reports and earlier quick-task artifacts remain historical.

## Commits

- `6f8f88f`: follow-up context, focused research and checked plan.
- `0ccf575`: purpose-based game tool folders, root/fixture rebinding and guides/tests.
- `4ea92b5`: include the game-configured analyzer, simplify normal policy inheritance, and remove the root tools tree. This is the final tested implementation revision.

## Verification

Locked restore and strict build passed with zero warnings/errors. The fresh required selection passed 107/107: complete docs (36), compiler-policy (27) and runner (36) groups plus eight actual graph/registry/source/suppression/native checks. All 25 tooling self-tests passed, including alternate-cwd command launch, folder ownership, inherited report execution and real SDK SourceLink/source-stamp controls. Actual headless toolchain/bootstrap passed separately as raw compatibility evidence.

The preservation audit confirms exact bytes in 104 game source/resource/project files, five package locks, sixteen analyzer implementations and three input drivers. Captured provenance is unchanged. [CHECKS](261116-m0w-CHECKS.json) records exact reports, hashes, source/platform inputs and measured durations. [Review](261116-m0w-REVIEW.md) records repairs, including synthetic-fixture directory assumptions, inherited-family path mapping and SourceLink root depth. Failed attempts remain retained and are not relabeled as passes.

No new rendered game, full LAN, Linux execution, measured coverage or human qualitative acceptance is claimed. The initial cleanup report's old paths/fingerprint describe its earlier revision, not the new layout. This follow-up supersedes its provisional tool-placement decision.

## Publication

The same [PR #6](https://github.com/Sebreiro/kingdom-zombies-game/pull/6) targets dev and is ready for review, unmerged. It returned to draft while this follow-up was implemented. The original active Phase 4 checkout is untouched; its asset tools should use `src/godot-game/tools/asset-pipeline` when integrated. Completion records are metadata after the tested source and do not create new game execution evidence.
