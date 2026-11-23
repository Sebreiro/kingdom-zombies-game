# Quick Task 261123-ltj: Replace the .phaseN folders with one ignored .local folder - Context

**Gathered:** 2026-11-23
**Status:** Ready for planning

<domain>
## Task Boundary

> Let's change naming. I don't like folder names ".phase1" or similar. If this folder needs to be used for the future items - like cache, etc - let's change name to something specific. Also update docs. Also let's make Node24, and other binaries per repository not per worktree. But these binaries should not be committed.

> I think we need a single folder for files that are gitignored and could be remade - something like ".tmp" with specific subfolders. Even required binaries go there but to the specific folder. If a phase needs to create some files for git - most likely, they have to live in the game folder - there is already a tools folder. If we need a project root folder that is committed to git - it is OK but the name should be specific.

All live code, live docs and ignore rules that use `/.phase[0-9]*/` move to one ignored root folder, `.local/`. The task branch is `feature/261123-ltj-local-folder`, cut from `origin/dev` at `88953aa`, which already contains PR #6 (`18e77bc`) and the Phase 5 planning PR #11.

Pre-discussion research is in `261123-ltj-PRE-RESEARCH.md` beside this file.

</domain>

<decisions>
## Implementation Decisions

### Folder name and layout (creator)
- The single git-ignored, regenerable root folder is **`.local/`**. It replaces `/.phase[0-9]*/` in `.gitignore`, live code and live docs.
- Subfolders:
  - `.local/tools/`: extracted binaries (Node 24.22.1, Godot 4.7.2 editor, Linux VM files).
  - `.local/downloads/`: pinned archives and checksums, export templates.
  - `.local/nuget/`: NuGet global packages, HTTP cache, scratch.
  - `.local/locks/`: cross-run locks.
  - `.local/runs/<tool-purpose>/`: `verification`, `gameplay-checks`, `lan-checks`, `asset-pipeline`.
  - `.local/temp/`: per-checkout scratch.
- The cleanup command keeps `tools`, `downloads` and `nuget` and deletes runs, temp and other scratch. Everything else in its keep/delete table changes to match.

### Sharing (creator)
- `tools`, `downloads` and `nuget` live once per repository, in the **main checkout's** `.local/`. Every worktree finds it through `git rev-parse --path-format=absolute --git-common-dir`, whose parent is the main checkout. An environment variable overrides it for CI, extra clones and copies.
- Runs and temp stay per worktree.
- The research recommends what else to share. Follow it unless the plan finds a reason not to. That means NuGet scratch and the machine-wide desktop lock are shared, and CLI homes are per checkout. NuGet requires a shared scratch folder when packages are shared.
- The bootstrap's "path must stay inside the checkout" rule, and the export trace check that rejects the old folder, get an explicit, narrow exception for the shared root.

### What stays where it is
- `bin/`, `obj/` and `.godot/` build and import outputs stay in place. Godot C# expects `.godot/mono/temp`, and registers and the analyzer depend on it.
- GSD-owned `.planning/tmp/`, `.planning/state.json` and `.planning/milestone.lock` stay in place. The vendored GSD tool hard-codes them.
- Tracked root folders already have specific names (`.claude`, `.codex`, `.planning`, `docs`, `specs`, `src`). No change.

### Durable data in ignored folders
- **Code tracked, data regenerable:** anything that is really code but lives in an ignored folder, such as the VM launcher, moves to a tracked location under `src/godot-game/tools/`. That follows the creator's rule that files git needs live in the game folder.
- **Phase 4 inputs declared lost (creator):** the checkpoint manifest, the current model and the three JSON reports are gone from the main checkout, so they are declared lost. The checks that read them stay blocked, owned by the phase the current roadmap assigns. After PR #11, Phase 5 makes the Phase 4 checks it runs work in the relocated tree, and Phase 11 renews the review-bound gates.
- **Captured recipes keep their bytes:** pinned recipes that assert the old `.phase4` folder fail fast with a clear, named error that points at the renewal owner. Captured strings and pinned data keep their bytes.

### Node 24 (creator)
- Add a SHA-256 pin to `toolchain.json`:
  - win-x64 `node-v24.22.1-win-x64.zip` = `d2046d28c2c3dcd211ac5145c0bf30e2047ef9dfb306693c8c6e2eccd8b95fa5`
  - linux-x64 `node-v24.22.1-linux-x64.tar.xz` = `6c172a9ab6718f7b801fb5a7bd486af2ee5e32758086a04b84fcce2aa77095e6`
  - Both hashes come from the official `SHASUMS256.txt`.
- A tracked setup script (PowerShell and sh, since Node cannot install itself) downloads the official archive from nodejs.org into the shared `.local/downloads/`. It verifies the archive against the pin, unpacks it into the shared `.local/tools/`, and is idempotent.
- Run it once during this task.

### Node on PATH (creator)
- The setup script prints a per-shell PATH line. The project also gets a **SessionStart hook for Claude Code and the equivalent for Codex**, so each agent session gets the shared Node on PATH.
- No persistent user or machine PATH change.

### Phase-named identifiers (creator)
- Out of scope. This task changes folders only.
- Record the identifier rename as a **backlog task**, using the project's GSD backlog convention. It covers `Phase1.cs`, the `KingdomZombies.Phase1.Tests` project, `PHASE1_*` environment variables and the `phase1`/`phase2`/`phase3` case families.

### Extras (creator)
- **Worktree location rule:** create task worktrees at `<git config --get worktrees.root>/<main checkout folder name>/kz-<task-id>-<short-description>`, falling back to the system temp folder when the value is unset.
  - Worktree creation moves out of `docs/workflow/cleanup.md` into `docs/workflow/ai-collaboration.md`.
  - `cleanup.md` keeps only cleanup.
  - `worktrees.root` is a per-user git value, so the repository holds no machine path.
- **Rebinding tools tracked:** the ac-08 regenerator and the gate checks from quick task 261123-fsu were deleted with that task's scratch. Rebuild them as tracked tools under `src/godot-game/tools/verification/`, using the committed fsu PLAN and RESEARCH, so future moves do not start from scratch.
- **Long paths:** `docs/workflow/setup.md` tells people to set `git config core.longpaths true` in this repository. A recent worktree removal failed with "Filename too long".
- **Publishing:** after the checks pass and the review is clean, push the branch and open a PR to `dev`. Do not merge it. The orchestrator pushes and opens the PR; subagents never push or change GitHub.

### Phase 5 planning changes (creator)
- The creator asked for the Phase 5 commits to be reviewed and included. They are already on `dev` through PR #11 (squash `88953aa`), so this branch includes them.
- Review that content for contradictions with this change and with the earlier quick-task records, and fix them in this PR. The known example is renewal ownership: the fsu STATE bullet versus Phase 5 CONTEXT D-46 and Phase 11.

### Verification (carried from the creator's last choice)
- Strict locked build with analyzers, layout tests and the documentation checks.
- Hash bindings rebound and checked by the tracked tools.
- The shared root resolves correctly from the task worktree, from the main checkout and from an unrelated working directory.
- The Node setup script ran once and its result matches the pin.
- Not run, reported as not run: export, editor import, headless launch, game windows, the full case selection.

### Claude's Discretion
- Exact names of the environment variables (the shared-root override and the `KZ_*` isolation variables) and of the new helpers, provided they are specific.
- Whether the verified Godot package feed is shared or copied per checkout.
- How to seed the shared NuGet folder for this task's build. Prefer copying from the main checkout's existing `.phase1` caches. Never move them.
- Commit granularity.

### Decisions after research (creator, 2026-11-23)
- **Codex:** add an advisory SessionStart hook in `.codex/hooks.json` that prints the exact PATH line as session context. Codex cannot change PATH for a session. Each user trusts the hook once. Claude gets a real PATH change through a new tracked `.claude/settings.json` SessionStart hook.
- **Linux VM:** the migration command does not move the VM. It deletes the legacy `linux-vm` folder, and the next phase recreates the VM under `.local/tools/` if it needs one. The VM disk header therefore needs no rewrite.
  - The dry run lists the deletion and its size first.
  - The creator runs the migration after this PR merges.
  - Docs say that the VM is recreated by the phase that needs it.
  - Launcher code currently in the ignored VM folder moves to a tracked location under `src/godot-game/tools/` only if it is generic and carries no secret or machine path. Otherwise the docs describe its role.
- **Main checkout before migration:** after the merge the main checkout is still on the squash-merged `feature/05-ci-and-repeatable-game-testing`. The migration refuses while the main checkout's branch still uses `.phase1`. Switching the main checkout to `dev` is the creator's step and is documented.

</decisions>

<specifics>
## Specific Ideas

- **One central path helper.** Add one JS helper and one C# helper that compute every `.local` location. Do not scatter new literals across about 74 files.
- **No weakening.** Do not weaken, skip or broaden a validator, pin, exemption or expected count. Rebind only what the move requires, in cascade order: code, registers, catalog, delivery, pack index, then ac-08.
- **History keeps its bytes.** Phase records, quick-task records and captured provenance keep their bytes. The layout policy's historical-path map gets `.phaseN → .local/…` rows that interpret them.
- **No machine paths.** No new line may contain a machine-specific path.
- **Do not migrate the main checkout's existing toolchain.** Leave its `.phase1` caches (Godot, about 8 GB of VM files, feed, packages) in place during this task, because other sessions may use them. Provide a tracked, dry-run-first migration command and document it. The creator runs it once this PR merges. Copying for this task's own build is allowed.
- **Keep the main checkout's untracked status clean.** Until the main checkout's branch carries the new ignore rule, writing to its `.local/` would show up as untracked files. The orchestrator handles this before the first install, using the repository-local exclude file.
- **Downloads.** Node from nodejs.org is approved. No other downloads or installs.
- **Commits.** Commit messages end with `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`.
- **Locations in records.** Quick-task records name locations symbolically: main checkout, task worktree, shared root.

</specifics>

<canonical_refs>
## Canonical References

- `261123-ltj-PRE-RESEARCH.md`: inventory, path construction, hash cascade, sharing risks and proposed layout
- `docs/workflow/cleanup.md`, `docs/workflow/setup.md`, `docs/workflow/ai-collaboration.md`, `docs/workflow/local-testing.md`, `docs/workflow/verification.md`
- `docs/architecture/repository-layout.md`: layout policy and historical-path map
- `src/godot-game/tools/verification/bootstrap.cjs`, `src/godot-game/tools/verification/toolchain.json`, `src/godot-game/tools/workspace-cleanup.cjs`
- `.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/`: rebinding method, gates and ac-08 rule
- `.planning/phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md` (D-46) and `05-SPEC.md`: renewal ownership after PR #11

</canonical_refs>
