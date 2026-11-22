# Workspace cleanup

AI agents must leave the checkout and the machine as they found them, apart from the committed work. Without cleanup, phase runs, probes and worktrees pile up: one checkout held about 74 GB of ignored Phase 4 run output. These rules apply to every phase plan, quick task and ad-hoc task. The [agent guide](../../AGENTS.md) indexes them.

## When to clean up

Clean up at task end: when a phase plan, quick task or other requested piece of work is finished and its commits are pushed. Do not wait for phase completion or for the pull request to merge.

When pausing an unfinished task for another session, keep the outputs the continuation needs and name them in the continuation record. The agent that finishes the task cleans up. Never delete outputs while a game run, test runner or build from the same checkout is still running.

## Record evidence before deleting it

Ignored outputs are deleted at task end, including reports that a record cites. Before cleanup:

- Put every result the task relies on into its committed record (summary, verification record or pull request): the command, tested revision, exit status, pass/fail/blocked counts, failures and the key identities the report printed. A report path in a record names local evidence that no longer exists after cleanup.
- Write reproduction steps: the exact commands from the checkout root, required setup, run groups, manual steps and expected results, so the creator can rerun every check.
- Move anything a later task needs as input, such as approved assets or scenario definitions, into a tracked location. Ignored folders are not storage between tasks.

Known exceptions: Phase 4 `export.layout` and `export.profile`, Phase 1 `rules.export-layout-origin` and `rules.export-layout-boundary`, the `ContentNativePermissionTests` model tests and the Phase 2 completion check `node tools/phase2/check.cjs --verify-completion .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` read ignored `.phase4` or `.phase2` inputs. They fail after cleanup until those inputs are tracked or regenerated. The Phase 3 completion check of `03-EVIDENCE.json` and the Phase 4 `runner.prelaunch`, `runner.selection`, rendered groups, `acceptance.artifacts` and completion check also need deleted runs, although they already fail on `dev` for other reasons.

Every pull request description has a **How to reproduce** section with those steps.

Within one task, keep reports immutable and reuse them as [verification](verification.md) allows. Reports do not survive into later tasks, so evidence reuse across tasks needs tracked or otherwise durable evidence storage, which the Phase 5 reuse mechanism must define before relying on older reports.

## What to delete and keep

| Location | Content | At task end |
| --- | --- | --- |
| `.phase1/` folders `cache`, `cli-home`, `dotnet-home`, `feed`, `http-cache`, `packages`, `tools` | Pinned toolchain archives, verified feed, package caches, Godot and the Linux VM | Keep |
| Everything else in `.phase1/` | Runs, temporary files, checker scratch and drafts such as pull request bodies | Delete |
| `.phase2/` and every later `.phaseN/` folder | Runs, controls, builds, checkpoints, probes and generated media | Delete |
| `.planning/tmp/` | GSD workflow probes | Delete |
| `bin/`, `obj/`, `.godot/` | Build and import caches | Optional; delete them to reclaim space |
| Files the task wrote outside the checkout, such as scripts or downloads in the system temp folder | Task scratch | Delete |
| Tracked files, `.planning/state.json`, `.gsd/` and files the task did not create | Project and tool state | Keep |

The command keeps and reports a folder that contains a repository or cannot be scanned. After cleanup, rerun the [setup](setup.md) isolation step before direct builds; it recreates `.phase1/temp`.

The repository [ignore rules](../../.gitignore) cover every top-level `.phaseN` folder. Do not write task outputs to other ignored locations. A new output location needs a `.phaseN` subfolder, or an update to the ignore rules, this page and the cleanup command together.

## Worktrees

- Create a task worktree outside the checkout, in the system temp folder, named `kz-<task-id>-<short-description>`, on the task's branch. Record its path in the continuation record.
- At task end, once its commits are pushed and nothing uncommitted remains, remove it from another checkout with `node tools/workspace-cleanup.cjs --apply --worktrees-only --remove-worktree <path>`. Removal deletes everything ignored inside it, including caches that are not links; `--worktrees-only` leaves the other checkout's own outputs alone. The branch and pull request remain.
- Remove only worktrees created for the task. A dry run lists every linked worktree; ask the creator before removing any other, because another session may be using it.
- Delete a local task branch only after its pull request is merged or closed.
- Do not use `git worktree remove --force` or delete a worktree folder by hand without the creator's approval. The command refuses worktrees with uncommitted changes, hidden index flags, unpushed commits, a runner lock, a nested worktree or repository, or a folder it cannot scan.

## Processes and environment

Before finishing, stop the game instances, servers, build servers and helper processes the task started. The owned Linux VM follows [its setup rules](setup.md#local-vm-hardware-acceleration). Do not leave persistent user or machine environment changes, such as `PATH` entries that point at task folders.

## Cleanup command

From the repository root, after `git fetch --prune` so the pushed state is current:

```sh
node tools/workspace-cleanup.cjs
node tools/workspace-cleanup.cjs --apply
```

The first command is a dry run that lists each output as `delete` or `keep` with a reason, and each linked worktree as `removable`, `prune` or `keep`. Inspect it before applying.

| Option | Effect |
| --- | --- |
| `--apply` | Deletes the listed outputs and runs `git worktree prune`, which removes the records of every unlocked worktree whose folder is gone, including other sessions'. |
| `--build` | Also lists ignored `bin/`, `obj/` and `.godot/` folders for deletion. |
| `--worktrees-only` | Skips this checkout's outputs; only lists, prunes and removes worktrees. |
| `--remove-worktree <path>` | Removes that linked worktree if it is clean and pushed; repeatable. |

Apart from removing a worktree named with `--remove-worktree`, the command never deletes outside the checkout, a tracked file, a root that contains tracked files, a root Git does not ignore, a folder with a worktree or repository (`.git` entry or bare layout) at any depth, or a folder it cannot scan. It unlinks junctions and symbolic links without following them. A worktree counts as clean only with no tracked changes, no untracked files (whatever the Git configuration hides; only the repository's own `.gitignore` files mark files as disposable, not `core.excludesFile` or `info/exclude`), no assume-unchanged or skip-worktree files and no rebase, merge, cherry-pick, revert or bisect in progress.

`--apply` deletes nothing while this checkout has `.phase1/check.lock` or `.phase2/desktop.lock`, or when a requested worktree is refused, including for either lock in that worktree. Inspect a lock before removing it. Other processes take no lock; stop them first. The command exits with 1 when it refuses, when a deletion or removal fails or when Git fails, and with 2 for invalid arguments. In the completion report, state what was deleted and what was kept and why. The command's tests run with `node --test tools/workspace-cleanup.test.cjs`. They passed 14 of 14 at f373400, before the skip-worktree, hidden-untracked, nested-repository, build-folder, worktree-scan, runner-lock and output-buffer fixes, and have not been rerun on the current script; none of them covers those fixes.
