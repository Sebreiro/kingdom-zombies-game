# Development setup

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. Current routine local testing uses the development OS only, without a VM; see [local testing strategy](local-testing.md).

The exact baseline is [toolchain.json](../../src/godot-game/tools/verification/toolchain.json). The existing [Godot project](../../src/godot-game/game/project.godot) hosts a minimal C# proof scene. These setup references introduce no gameplay. Preserve the implemented runner and rules. The original Phase 1 full-baseline procedure below is historical; its closure exception and the current local testing strategy govern what is required now. Measured coverage and the eight explicit later-phase declarations are Phase 11 obligations under TEST-12. The catalog still declares those eight with `acceptancePhase: 5` until the Phase 5 implementation reassigns them, so `--phase 5` still selects them. [Verification](verification.md) owns current commands and evidence interpretation; VM setup below is optional historical preparation, not a routine phase-completion gate.

| Prerequisite | Exact version | Source |
| --- | --- | --- |
| Node.js, x64 | 24.22.1 | [Official release](https://nodejs.org/download/release/v24.22.1/) and its `SHASUMS256.txt`; the [Node setup](#shared-node) installs it once per repository |
| .NET SDK, x64 | 10.0.404 | [Microsoft .NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0); use the platform archive/installer and the published checksum |
| .NET application runtime, x64 | 10.0.15 | Same Microsoft download page; supplied by the pinned SDK |
| .NET editor runtime, x64 | 8.0.16 | [Microsoft .NET 8 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0); required alongside SDK 10 for Godot editor tools targeting `net8.0` |
| Godot .NET | 4.7.2 stable | Platform archives below; retain the entire `GodotSharp` directory |
| C# / target | 14.0 / net10.0 | Explicit project declarations; compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)` comes from SDK 10.0.404 |
| Local coverage tool | 18.12.0 | [Microsoft dotnet-coverage](https://www.nuget.org/packages/dotnet-coverage/18.12.0), pinned in the repository tool manifest; collection is proved in a later plan |
| PowerShell, x64 | 7 (`pwsh` on `PATH`; 7.6.9 verified) | [Microsoft PowerShell releases](https://github.com/PowerShell/PowerShell/releases); the Phase 4 `content.native-permissions` check runs its test discovery through `pwsh` and fails with `spawn pwsh ENOENT` without it |
| CPython, x64 | 3.12.14 | Runs the Phase 4 media, archive and pixel scripts. [python.json](../../src/godot-game/tools/asset-pipeline/python.json) pins the executable, `python312.dll` and Pillow 13.0.0, and every launch verifies them. Set `KZ_PHASE4_PYTHON` when the same build is installed elsewhere. UnityPy 1.23.0 loads from the donor resource folder and each script checks its version |

The installed SDK is a prerequisite. [global.json](../../src/godot-game/global.json) disables SDK roll-forward and prerelease selection. The bootstrap checks actual Node, SDK, compiler, engine and managed runtime identities instead of treating declarations as proof. Roslyn NuGet development packages have separate version numbers; they are not the SDK compiler version.

| OS / architecture | Exact official archive | SHA-512 |
| --- | --- | --- |
| Windows x64 | [Godot_v4.7.2-stable_mono_win64.zip](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/Godot_v4.7.2-stable_mono_win64.zip) | `79229fd112b0c9cbeab82363a4ef7be18ea70f1caf86bf912789335b136fbe7e01db0053a33461438c5da1c680c17bbb10040bd09bedc51221cd4423d0367757` |
| Linux x64 | [Godot_v4.7.2-stable_mono_linux_x86_64.zip](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/Godot_v4.7.2-stable_mono_linux_x86_64.zip) | `1855960b27ee3ef5e66e5e228cced69d55637b24334a7411162687dcd077d8f9f645348cdb8eae984bec8135d49ed855a1e3a16476786b8bce60774fd8402d13` |

Both SHA-512 values come from the [official release checksum file](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/SHA512-SUMS.txt). Windows also has pinned SHA-256 `a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`. Linux archive availability is preparation; instructions and cross-builds do not count as a Linux execution result.

## Workspace folder

Every checkout keeps ignored, regenerable state in one root folder, `.local/`. [workspace-paths.cjs](../../src/godot-game/tools/workspace-paths.cjs) resolves every location in it; the [layout policy](../architecture/repository-layout.md#commands-and-generated-state) lists the subfolders. `tools/`, `downloads/` and `nuget/` live once per repository, in the main checkout's `.local/` (the shared root), which every linked worktree finds through `git rev-parse --path-format=absolute --git-common-dir`. Runs, temp, CLI homes, the verified package feed and the check lock stay in each checkout's own `.local/`.

Set the environment variable `KZ_SHARED_LOCAL` to an absolute folder to choose the shared root explicitly. CI, extra clones that should not use another checkout's caches, source copies and checkouts whose Git common folder is not a `.git` folder (a bare repository or a submodule) need it; the helper stops with `SHARED_ROOT_UNRESOLVED` in the last case. A folder that is not a Git checkout uses its own `.local/`. Git must be on `PATH`: without it, the Node setup, bootstrap, the runners and the C# tests stop with `GIT_MISSING`, and none of them falls back to the checkout's own `.local/`.

One rule applies to `KZ_SHARED_LOCAL` everywhere: in `workspace-paths.cjs`, the Node setup scripts, both session hooks and the C# tests.

- On Windows the value must be drive-qualified (`<drive>:/kz-shared` or `<drive>:\kz-shared`) or a UNC path (`\\<server>\<share>\kz-shared`). A root-relative value (`\kz-shared`, `/kz-shared`) and the Git Bash form `/<drive>/kz-shared` are refused, not converted; in Git Bash, write `<drive>:/kz-shared`. Elsewhere the value must start with `/`.
- It must not contain a `.` or `..` segment and must not be a filesystem or drive root.
- It must not be the checkout or a folder inside it, except the checkout's own `.local/` and the folders below it.

Setup, bootstrap and the runners stop with `SHARED_ROOT_INVALID` and the reason. The session hooks treat an invalid value as no shared Node: they write nothing and never fail the session.

Nothing writes the shared root unless Git ignores it in the checkout that holds it. The Node setup, bootstrap, the runners, `--print-env` and the C# tests first run `git check-ignore` there and otherwise stop with `SHARED_ROOT_NOT_IGNORED`. Branches from before the `.local/` layout do not ignore it. While the main checkout is on such a branch, add this line to its repository-local exclude file, `.git/info/exclude`; no tool adds it for you:

```text
/.local/
```

On Windows, allow long paths in this repository once, because worktree and build paths can exceed the Windows path limit and removal then fails with "Filename too long":

```sh
git config core.longpaths true
```

## Shared Node

[toolchain.json](../../src/godot-game/tools/verification/toolchain.json) pins the Node archives by SHA-256 under `nodeArchives`. The [Node setup scripts](../../src/godot-game/tools/node-setup/setup.sh) download the pinned archive into the shared root's `.local/downloads/node/`, verify it before extraction and unpack it into `.local/tools/node/<archive folder>/`. They hold the shared `.local/locks/toolchain.lock` while writing and download nothing when Node is already installed. Each prints one PATH line for its shell on stdout and nothing else; messages go to stderr. They make no persistent user or machine `PATH` change.

| Shell | Install and put Node on `PATH` for this shell |
| --- | --- |
| Git Bash, Linux | `eval "$(sh src/godot-game/tools/node-setup/setup.sh)"` |
| PowerShell | `powershell -NoProfile -ExecutionPolicy Bypass -File src/godot-game/tools/node-setup/setup.ps1 \| Out-String \| Invoke-Expression` |

Both scripts take `--check` to verify the archive and the installed Node without writing, and `--archive <file>` to seed the shared downloads from a local copy that matches the pin. `setup.sh` also takes `--platform linux-x64`, which verifies and unpacks the Linux archive without running it, for VM preparation. On Git Bash, `setup.sh` verifies an existing install itself and otherwise delegates the installation to `setup.ps1`.

Agent sessions get the shared Node without a manual step:

- Claude Code runs the SessionStart hook in the tracked `.claude/settings.json`, [session-path.sh](../../src/godot-game/tools/node-setup/session-path.sh). It appends the `export PATH=...` line to the session environment file, which changes `PATH` for later Bash tool calls. PowerShell tool calls do not receive it; run the PowerShell line above there. Claude Code asks once to accept new project hooks. The GSD-owned `.claude/settings.local.json` keeps its own hooks.
- Codex cannot change `PATH` for a session. Its SessionStart hook in the tracked `.codex/hooks.json` prints the exact PATH line as session context, and the agent prefixes its shell commands with it. Trust the hook once with `/hooks`. Alternatively, start `codex` from a shell that has already evaluated the setup script's line.

Both hooks do nothing until the shared Node is installed, and never fail a session start.

Confirmed defects in this engine version, with the workaround each one has in this repository, are listed in [engine defects](engine-defects.md). Check that page before you upgrade the engine or change how a harness starts it.

Bootstrap acquisition happens as part of the later proof commands. It checks the shared root's `.local/downloads/godot/` first, may seed it from the matching archive in Downloads, then downloads the exact official release if absent. It extracts the engine into the shared `.local/tools/godot/` and holds the shared `.local/locks/toolchain.lock` while it acquires. Downloads and the LangProj/recovered donor directories are never required. To require official acquisition, set `PHASE1_IGNORE_DOWNLOADS=1` and `PHASE1_FORCE_DOWNLOAD=1` for that invocation. An optional `PHASE1_ARCHIVE` names a local archive cache seed; the same pinned hashes apply. Node HTTPS uses a task-local direct connection when inherited proxy settings are broken. No machine-wide proxy or NuGet configuration changes are made.

All child processes receive the isolation variables from `workspace-paths.cjs`: `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH` and `NUGET_SCRATCH` in the shared root's `.local/nuget/`, `DOTNET_CLI_HOME` in the checkout's `.local/temp/cli-home/`, `TEMP`, `TMP` and `TMPDIR` in the checkout's `.local/temp/`, and `KZ_SHARED_LOCAL` set to the resolved shared root. NuGet needs the shared scratch folder for its cross-process locks once the package folder is shared. The native engine comes from the actual OS archive. The common managed feed uses the exact four packages from the verified Windows archive on both operating systems, explicitly selected by `managedPackageArchive` in the toolchain manifest. Linux therefore caches both official archives. Their inner SDK, generator and managed DLL contents match, but package relationship/core metadata and raw nupkg hashes differ. Linux archive packages must not replace the canonical bytes pinned by the five common locks and generated-source provenance. Bootstrap verifies both archive identities and the canonical package hashes before copying to the checkout's `.local/nuget/feed/`; it never executes the Windows binary on Linux. [NuGet.config](../../src/godot-game/NuGet.config) maps those exact IDs to that feed and other pinned dependencies to NuGet.org. The archive packages are unsigned. Their provenance is the official archive checksum, package payload hashes and inspected publisher/repository metadata; no successful NuGet signature verdict is claimed. Each controlled child has a 60-second deadline and descendant cleanup. Windows extraction uses the installed .NET `ZipFile` implementation through PowerShell, avoiding a dependency on Archive-module autoload.

For direct CLI setup commands outside the bootstrap, first establish the same isolation from the checkout root, with Node on `PATH`. `--print-env` creates the folders and prints the assignments for one shell. PowerShell:

```powershell
node src/godot-game/tools/workspace-paths.cjs --print-env powershell | Out-String | Invoke-Expression
Push-Location -LiteralPath 'src/godot-game'
dotnet tool restore --configfile NuGet.config
Pop-Location
```

Git Bash or Linux (Linux requires `unzip` and the x64 prerequisites):

```sh
eval "$(node src/godot-game/tools/workspace-paths.cjs --print-env sh)"
(cd src/godot-game && dotnet tool restore --configfile NuGet.config)
```

`node src/godot-game/tools/workspace-paths.cjs --print-roots` prints the resolved checkout, local and shared roots.

Tool restore uses only the declared official NuGet source. The bootstrap's initial engine slice needs only the four archive packages; additional phase tools are restored when their owning plans establish their proofs. After the native bootstrap command in the [verification reference](verification.md) has populated the verified engine feed, restore exactly the five committed project lock files from the checkout root on either platform:

```sh
cd src/godot-game
dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
```

Use the same source checkout and pinned prerequisites on each operating system. A clean checkout must not need LangProj, recovered donor folders or a Downloads cache. Keep each checkout's CLI home, feed and temp in its own `.local/`, share the package, HTTP and scratch folders through the shared root, and retain the entire platform Godot archive directory. Acquisition failure, missing prerequisites or failed locked restore blocks the dependent build/proofs. Do not substitute an unverified package name or silently update a lock to bypass failure. Bootstrap native subprocesses have bounded deadlines; the full case coordinator records each case's own deadline and cleanup. The prepared workspace Linux VM belongs to later platform execution and must not be terminated by unrelated proof cleanup.

Setup alone does not certify execution, coverage, exports or rendered gameplay. Under the creator's latest [platform rule](ai-collaboration.md#development-and-phase-end-testing), use the development OS for implementation checks, then execute the complete current phase scope on that OS and in a local VM running the other supported OS at phase end. Plan 01-17 owns fresh complete Phase 1 Windows/Linux-VM evidence after the review repairs; earlier 01-16/01-17 reports remain historical. Measured 90%/80% coverage, eight explicitly pending declarations (still `acceptancePhase: 5` in the catalog until the Phase 5 implementation reassigns them) and advanced certification belong to Phase 11 under TEST-12, through the [Phase 1 certification handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). Actual commands and evidence rules are in [verification](verification.md).

## Local VM hardware acceleration

For the existing Windows-hosted Linux VM, QEMU's [WHPX backend](https://www.qemu.org/docs/master/system/whpx.html) uses Windows Hypervisor Platform. Enable **Windows Hypervisor Platform** in **Turn Windows features on or off**, then complete any requested Windows restart. The creator confirmed enabling this feature on 2026-11-14; live inspection still reported no active hypervisor before restart. No accelerated execution is inferred from the checkbox alone.

The owned runtime was the ignored `.phase1/tools/linux-vm/` folder. The [migration](#moving-a-checkout-to-the-local-folder) deletes it, and the phase that next needs a VM recreates it under the shared root's `.local/tools/linux-vm/`. The tracked launcher [start-vm.cjs](../../src/godot-game/tools/verification/linux-vm/start-vm.cjs) runs it from there and defaults to `whpx` and records the requested accelerator/CPU with the owned process identity; software `tcg` requires explicit selection. After restart, first confirm that the host hypervisor is active, start only the one owned VM, verify actual WHPX startup and genuine Linux access, then run the complete Phase 1 selection with unchanged deadlines. Preserve the existing disk, verified tools and caches; do not start a second process on the same live disk. Shut down the owned guest cleanly before a host restart. Hardware preparation supplies no test acceptance by itself.

## Moving a checkout to the .local folder

Checkouts made before the `.local/` layout keep their toolchain caches in the legacy ignored `.phase1/` folder. [workspace-migrate.cjs](../../src/godot-game/tools/workspace-migrate.cjs) moves them into the main checkout's `.local/` once. It always works on the main checkout, found through the Git common folder from any working directory. Run it after the pull request that introduced `.local/` has merged:

1. Switch the main checkout to `dev` and update it: `git switch dev` and `git pull`. A squash-merged branch is not an ancestor of `dev`, so deleting its local copy needs `git branch -D <branch>`. The migration refuses until the main checkout's HEAD contains `src/godot-game/tools/workspace-paths.cjs` and its `.gitignore` ignores `/.local/`, which holds only after the switch.
2. Stop running games, runners and the Linux VM, then run `dotnet build-server shutdown`. Build servers can hold files inside the package cache.
3. Dry run: `node src/godot-game/tools/workspace-migrate.cjs`. It writes nothing and lists one row per legacy item as `<action> <legacy path> -> <target> (<size>)`. The first row is the deletion of the legacy Linux VM folder with its size; the VM is not moved, and the phase that needs one recreates it. Before it lists a move, the dry run verifies the source against its pin: the Godot archives and checksum file, the export templates, the Godot editor and the feed. The VM preparation archives move unchanged. It also checks that every target's parent folders can be created. The feed lands in the main checkout's own `.local/nuget/feed/`. Package and HTTP caches move, or are deleted as duplicates when the shared copy already exists. `cli-home`, `dotnet-home` and stray files are deleted. Lines starting with `refuse:` name what blocks the move: a lock file, a process running from the legacy folder or the VM, a linked, tracked, unignored or repository-holding source, a source or existing target that fails its pin, a target parent that is not a folder, or a `KZ_SHARED_LOCAL` other than the main checkout's `.local/`. `--apply` runs exactly these checks first, so a dry run that exits 0 shows that `--apply` will not stop on a pin or target check.
4. Apply: `node src/godot-game/tools/workspace-migrate.cjs --apply`. After every check passes, it makes all moves (each one rename on the same volume), then all deletions, and deletes the legacy VM folder last. It holds the shared `.local/locks/toolchain.lock` while it changes anything, so no bootstrap or Node setup writes the same folders. If it is interrupted, for example by a file in use, rerun it: it skips the rows already migrated or deleted and continues. A process that was killed leaves the lock behind; inspect it, then delete it before the rerun.
5. Run the cleanup dry run, `node src/godot-game/tools/workspace-cleanup.cjs`, and inspect it before `--apply`; see [workspace cleanup](cleanup.md).
