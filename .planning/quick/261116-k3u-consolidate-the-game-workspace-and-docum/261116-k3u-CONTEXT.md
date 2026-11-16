# Repository layout - Context

Gathered: 2026-11-16. Status: Ready for planning.

## Task boundary

Consolidate all game projects, assets and game tests under `src/godot-game`, explain build policy and tooling, and provide one external PR review entry point. Implement on a separate feature branch with a PR to dev.

## Locked decisions

- The creator selected a complete game workspace with a `game/` subfolder containing `project.godot`.
- Preserve the separate Core, multiplayer contracts and Godot C# projects and their dependency boundaries.
- Add `docs/workflow/code-review.md` as the external review entry point.
- Explain tooling before reorganizing it. Keep existing tools paths in this cleanup and document their purposes; no approval for a purpose-based tools rename is inferred.
- Base the new branch on latest origin/dev. Preserve the active Phase 4 checkout and all its ongoing changes.
- Keep reusable analyzer implementation in `tools/code-rules`. Its own tests remain part of the game workspace's verification project.

## Implementation discretion

Use a `tests/verification` project folder without renaming its assembly or case IDs. Keep conventional .NET solution/settings filenames at the game workspace root; put banned-symbol and specialized xUnit policy under `build/`. Leave existing ignored run/cache locations in place to preserve evidence and avoid changing active tooling contracts unnecessarily.

## Canonical references

- `AGENTS.md`
- `docs/workflow/ai-collaboration.md`
- `docs/workflow/verification.md`
- `docs/architecture/boundaries.md`
- `docs/code/README.md`

## Workflow execution

Run discussion, focused research, plan checking, implementation, code review and verification sequentially in the current agent. The invoked skill permits inline execution when the creator has not explicitly requested sub-agents. This checkout is isolated from the active feature checkout; it is not a claim of delegated-agent isolation.
