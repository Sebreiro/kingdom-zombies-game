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

The repository [ignore rules](../../.gitignore) cover every top-level `.phaseN` folder. Do not write task outputs to other ignored locations. A new output location needs a `.phaseN` subfolder, or an update to the ignore rules, this page and the cleanup command together.

## Worktrees

- Create a task worktree outside the checkout, in the system temp folder, named `kz-<task-id>-<short-description>`, on the task's branch. Record its path in the continuation record.
- At task end, once its commits are pushed and nothing uncommitted remains, remove it from another checkout with `node tools/workspace-cleanup.cjs --apply --worktrees-only --remove-worktree <path>`. Removal also deletes the ignored outputs inside it; `--worktrees-only` leaves the other checkout's own outputs alone. The branch and pull request remain.
- Remove only worktrees created for the task. A dry run lists every linked worktree; ask the creator before removing any other, because another session may be using it.
- Delete a local task branch only after its pull request is merged or closed.
- Do not use `git worktree remove --force` or delete a worktree folder by hand without the creator's approval. The command refuses worktrees with uncommitted changes or unpushed commits.

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
| `--apply` | Deletes the listed outputs and prunes records of worktrees whose folders are gone. |
| `--build` | Also lists ignored `bin/`, `obj/` and `.godot/` folders for deletion. |
| `--worktrees-only` | Skips this checkout's outputs; only lists, prunes and removes worktrees. |
| `--remove-worktree <path>` | Removes that linked worktree if it is clean and pushed; repeatable. |

The command never deletes outside the checkout, a tracked file, a root that contains tracked files, a root Git does not ignore or a folder that contains a Git worktree or repository. It unlinks junctions and symbolic links without following them. A worktree counts as clean only with no tracked changes, no untracked files (whatever the Git configuration hides), no assume-unchanged or skip-worktree files and no rebase, merge, cherry-pick, revert or bisect in progress.

`--apply` deletes nothing while `.phase1/check.lock` exists, because a checker run is active or stopped uncleanly; inspect the lock first. It also deletes nothing when a requested worktree is refused. The command exits with 1 when it refuses, when a deletion or removal fails or when Git fails, and with 2 for invalid arguments. In the completion report, state what was deleted and what was kept and why. The command's tests run with `node --test tools/workspace-cleanup.test.cjs`.
