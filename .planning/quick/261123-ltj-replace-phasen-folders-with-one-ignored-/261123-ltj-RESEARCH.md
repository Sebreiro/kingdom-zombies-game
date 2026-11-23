# Quick Task 261123-ltj: one ignored `.local/` root - Research (completion)

**Researched:** 2026-11-23, read-only, against the task worktree at `88953aa` (origin/dev). Node 24.22.1 (session-only) ran repository tools in read-only mode: `--list`, `content.normalizeCurrent`, `content.validate`, and an ac-08 row probe. Nothing was written to the repository or to the main checkout.
**Locations:** "main checkout" is the primary working tree. "Task worktree" is this linked worktree. "Shared root" (S) is the main checkout's `.local/`. "Local root" (L) is a checkout's own `.local/`, so in the main checkout S = L.
**Scope:** This file holds new findings only. The inventory, hash cascade, NuGet sharing risks and proposed layout are in [PRE-RESEARCH](261123-ltj-PRE-RESEARCH.md). The creator's locked decisions are in [CONTEXT](261123-ltj-CONTEXT.md).
**Confidence:** HIGH for repository facts (files read, probes run). MEDIUM for Claude Code and Codex behaviour (official docs fetched this session). Items tagged `[ASSUMED]` need confirmation.

## Summary

1. **Claude Code:** there is no `.claude/settings.json`. `.claude/settings.local.json` is **tracked** (not ignored) and holds the GSD-installed hooks, including a machine-specific Node path. Add a new tracked `.claude/settings.json` with a single SessionStart hook. Hooks merge across settings files. A tested POSIX `sh` script appends one `export PATH=…` line to `CLAUDE_ENV_FILE`; it is idempotent and fail-open, and works from any working directory.
2. **Codex cannot set PATH for a session.** Hooks can only add context, `shell_environment_policy.set` takes literal strings, and project config needs trust. The closest option is a SessionStart hook in `.codex/hooks.json` that prints the exact PATH line as developer context. GSD's pruning of `hooks.json` removes only GSD-managed commands, so the extra entry survives a GSD reinstall.
3. **Backlog:** the project has never used it: no `## Backlog`, no `999.x` folder and no `.planning/todos/`. The GSD convention is `/gsd-capture --backlog`. It writes a `## Backlog` → `### Phase 999.1: … (BACKLOG)` entry in ROADMAP and creates `.planning/phases/999.1-rename-phase-named-identifiers/.gitkeep`. The number and slug were verified with `gsd-tools`. A quick task may write it.
4. **Rebinding tools:** the fsu `acRows` rule, extracted byte-exact from the fsu PLAN (sha256 `daa62c2f…`, which matches the fsu SUMMARY), **reproduces HEAD's ac-08 block exactly**. All 498 rows match, the 230 checked rows fall in 15 chunks of 16, and `repositorySha256` is `4a795aab`. The asset cascade is also byte-reproducible at HEAD: `fixtureCatalog` is gzip level 9, `pack-index.json` equals `normalizeCurrent()`, and `validate` returns 180. Track three tools; drop the `merge`, `moves`, `state` and `records` gates.
5. **New hazard (VM):** the main checkout's `system.qcow2` stores an **absolute** backing-file path into the old `.phase1/tools/linux-vm/downloads/`. Moving the folder breaks the disk chain unless the header is first rebased to a relative path (`qemu-img rebase -u`; `qemu-img.exe` ships in the VM folder).
6. **New hazard (register package reads):** C# tests read register `package` and `surface` values physically from the checkout root. These are `.phase1/feed/…nupkg` and `.phase1/packages/…`. `ExemptionValidator.CheckHash` rejects any path that leaves the root. A per-checkout feed keeps feed reads inside the root. Shared-package reads need the same narrow exception as bootstrap.
7. **Seeding:** all 41 locked packages are present in the main checkout's `.phase1/packages` (47 id/version folders, 576 MB), and the 4 feed nupkgs match their `toolchain.json` pins. The Node pins in CONTEXT match the official `SHASUMS256.txt` already stored in the main checkout's `linux-prep`. The session Node zip matches the win-x64 pin.
8. **`info/exclude` is shared:** `git rev-parse --git-path info/exclude` from the task worktree resolves to the common dir. A probe showed that the common file's existing `.claude/worktrees/` line ignores paths in this worktree. Add the line `/.local/`.
9. **PR #11 review:** 5 contradictions with live text, all fixable without changing history (§7). The renewal bullet in STATE still says the `.phase4` recipes "run only there until Phase 5 renews them". After this task they fail fast everywhere.
10. **PRE-RESEARCH counts still hold at `88953aa`:** src 74 files / 317 lines, docs 15 / 575, 282 of 423 module rows. STATE.md went from 1 to 2 lines, and the main checkout's `.phase1/` now holds only the 7 durable folders (§9).

**Primary recommendation:** add the central path helpers first. Then rebuild the three binding tools as tracked tools and prove them against HEAD bytes **before** editing any hashed file. Only after that, do the literal replacement and the cascade.

<user_constraints>
## User Constraints

Authoritative and locked: [261123-ltj-CONTEXT.md](261123-ltj-CONTEXT.md). It is not duplicated here, at the orchestrator's instruction. This research relies on these decisions: the `.local/` name and subfolders; tools, downloads and nuget shared through `--git-common-dir` with an environment override; runs and temp per worktree; no machine paths; history keeps its bytes; no weakening; the main checkout's `.phase1` is copied, never moved, during this task; the migration is dry-run first and the creator runs it after merge; the Claude and Codex session hooks; the identifier rename recorded as a backlog item; the rebinding tools tracked under `src/godot-game/tools/verification/`; Node from nodejs.org is the only download.
</user_constraints>

## 1. Claude Code SessionStart hook

| Fact | Evidence |
|---|---|
| `.claude/settings.json` | **Absent** in the tree (`git ls-files .claude` lists no such file). |
| `.claude/settings.local.json` | **Tracked** (since `9416252`, PR #1) and **not ignored** (`git check-ignore` prints nothing). It holds every GSD hook (SessionStart: `gsd-check-update.js`, `gsd-path-export.js`, `gsd-session-state.sh`; plus PreToolUse, PostToolUse and others), `"worktree": {"baseRef": "head"}` and permissions. Each Node hook command embeds a machine-specific fallback Node path, a pre-existing issue outside this task. [VERIFIED: file read] |
| Who writes it | The GSD local install writes hooks to the local-scope file: `"settingsFileByScope": { "local": "settings.local.json", "global": "settings.json" }`. [VERIFIED: `.claude/gsd-core/bin/lib/capability-registry.cjs:620-623`] So do not add project hooks there; a GSD reinstall rewrites it. |
| How an existing hook is registered | An entry under `hooks.SessionStart[].hooks[]` with `"type": "command"`. The command is a shell string run by Git Bash on Windows and `sh -c` elsewhere, and it refers to `"$CLAUDE_PROJECT_DIR"/.claude/hooks/<file>`. `gsd-path-export.js` already uses `CLAUDE_ENV_FILE` to append `export PATH='<dir>':"$PATH"`, converting `C:\…` to `/c/…` for Git Bash. [VERIFIED: `.claude/hooks/gsd-path-export.js`] |
| Merge across files | "Hook entries merge across settings levels rather than replacing each other." Matching hooks run **in parallel**. [CITED: code.claude.com/docs/en/hooks] So the new hook cannot put Node on PATH for the GSD hooks that start at the same time, and it does not need to. |
| `CLAUDE_ENV_FILE` scope | Available to SessionStart, Setup, CwdChanged and FileChanged hooks. Bash commands later in the session run the file as shell code. On Windows, **PowerShell tool commands do not receive a variable whose value contains `/` or `\`**, and they receive none at all if any line is not a plain assignment. The GSD line already contains `"$PATH"`. [CITED: code.claude.com/docs/en/hooks] So the hook helps Bash tool calls only. Document the PowerShell form separately (the setup script prints it). |
| Matchers | `startup`, `resume`, `clear`, `compact`, `fork`. Omit the matcher so every start source runs it; the script deduplicates. [CITED: same] |

**Exact new file `.claude/settings.json`** (tracked, project scope, separate from GSD):

```json
{
  "hooks": {
    "SessionStart": [
      {
        "hooks": [
          {
            "type": "command",
            "command": "sh \"$CLAUDE_PROJECT_DIR\"/src/godot-game/tools/node-setup/session-path.sh",
            "timeout": 10
          }
        ]
      }
    ]
  }
}
```

Use `sh <file>` so the script needs no exec bit on Linux. The folder name `node-setup/` is a recommendation under the "specific names" discretion. It keeps the PowerShell and sh setup scripts and the two session scripts together.

**Script `src/godot-game/tools/node-setup/session-path.sh`.** It was tested in scratch: the default resolution with no shared Node is a silent no-op, two runs append one line, it works from an unrelated working directory and with a backslash `CLAUDE_PROJECT_DIR`, a folder name with spaces resolves, and sourcing the line finds `node.exe`.

```sh
#!/bin/sh
# SessionStart hook: put the repository's shared Node on PATH for this agent session.
# Claude Code: appends one export line to $CLAUDE_ENV_FILE. Codex (--codex): prints it as session context.
# Fail-open: any problem exits 0 and changes nothing.
mode=${1:-claude}
top=$(git -C "${CLAUDE_PROJECT_DIR:-.}" rev-parse --show-toplevel 2>/dev/null) || exit 0
version=$(sed -n 's/^ *"node": *"\([0-9.]*\)".*/\1/p' "$top/src/godot-game/tools/verification/toolchain.json" 2>/dev/null | head -n 1)
[ -n "$version" ] || exit 0
if [ -n "${KZ_SHARED_LOCAL:-}" ]; then
  shared=$KZ_SHARED_LOCAL
else
  common=$(git -C "$top" rev-parse --path-format=absolute --git-common-dir 2>/dev/null) || exit 0
  shared=$(dirname "$common")/.local
fi
case $(uname -s) in
  MINGW*|MSYS*|CYGWIN*) dir=$shared/tools/node/node-v$version-win-x64 exe=node.exe ;;
  Linux) dir=$shared/tools/node/node-v$version-linux-x64/bin exe=node ;;
  *) exit 0 ;;
esac
[ -f "$dir/$exe" ] || exit 0
if command -v cygpath >/dev/null 2>&1; then dir=$(cygpath -u "$dir"); fi
quoted=$(printf '%s' "$dir" | sed "s/'/'\\\\''/g")
line="export PATH='$quoted':\"\$PATH\""
if [ "$mode" = --codex ]; then
  printf 'Shared Node %s for this repository. In each shell command run first: %s\n' "$version" "$line"
  exit 0
fi
[ -n "${CLAUDE_ENV_FILE:-}" ] || exit 0
grep -qxF "$line" "$CLAUDE_ENV_FILE" 2>/dev/null || printf '%s\n' "$line" >> "$CLAUDE_ENV_FILE"
exit 0
```

- The version comes from `toolchain.json`: `"node": "24.22.1",` [VERIFIED: `src/godot-game/tools/verification/toolchain.json:9`], so the hook never duplicates the pin. If `toolchain.json` gains a nested `node` key, keep the top-level `"node"` first or adjust the `sed`.
- The archive layout `node-v24.22.1-win-x64/node.exe` is [VERIFIED: session zip listing]. The Linux `node-v24.22.1-linux-x64/bin/node` is [ASSUMED] (the standard nodejs.org layout); the setup script should assert it.
- `KZ_SHARED_LOCAL` is the recommended name for the shared-root override (discretion). The same variable must be honoured by `workspace-paths.cjs`, the C# helper and both setup scripts.
- When a checkout lives inside a bare repository, `dirname(common)` is not a checkout, so the override is required. Document it with CI.

## 2. Codex equivalent

| Fact | Evidence |
|---|---|
| Tracked files | `.codex/config.toml` (GSD-managed: `[features] hooks = true`, `[agents] max_depth = 1`) and `.codex/hooks.json` (one GSD SessionStart entry whose `command` and `commandWindows` embed a machine path, a pre-existing issue). [VERIFIED: files read] |
| Events | `SessionStart`, `SubagentStart`, `SessionEnd`, `PreToolUse`, `PermissionRequest`, `PostToolUse`, `PreCompact`, `PostCompact`, `UserPromptSubmit`, `SubagentStop`, `Stop`, `PreModelRequest`, `PostModelRequest`, `Interrupt`. [CITED: learn.chatgpt.com/docs/hooks] |
| Can a hook set PATH? | **No.** SessionStart output is plain stdout or `additionalContext` added as developer context; the docs name no `env` output. Hooks run with the session's working directory. [CITED: same] |
| `shell_environment_policy` | `inherit = all\|core\|none`, `set` (`map<string,string>`, "explicit environment values"), filters, and `experimental_use_profile`. The docs do not describe `$PATH` expansion in `set` [CITED: learn.chatgpt.com/docs/config-file/config-reference]; treating it as literal is [ASSUMED]. A literal PATH would need a machine path in a tracked file and would replace the whole PATH, so reject it. `experimental_use_profile` would need a persistent profile edit, which cleanup.md forbids. |
| Trust | Project `.codex/` layers (config, hooks) load only in a **trusted** project, and each non-managed hook must be reviewed and trusted by hash (`/hooks`). [CITED: same] |
| Both formats in one layer | Codex loads `hooks.json` and `[hooks]` in `config.toml` together and warns. Keep everything in `hooks.json`. [CITED: same] |
| GSD reinstall | GSD's Codex migration prunes only commands that `isManagedHookCommand` recognises and keeps other entries. [VERIFIED: `.codex/gsd-core/bin/lib/installer-migrations/002-codex-legacy-hooks-json.cjs:22-60`] |

**Closest supported option:** add a second SessionStart group to `.codex/hooks.json`, leaving the GSD group byte-identical. Its stdout tells the agent the exact PATH line to prefix:

```json
      {
        "hooks": [
          {
            "type": "command",
            "command": "sh \"$(git rev-parse --show-toplevel)/src/godot-game/tools/node-setup/session-path.sh\" --codex",
            "commandWindows": "powershell -NoProfile -ExecutionPolicy Bypass -Command \"& (Join-Path (git rev-parse --show-toplevel) 'src/godot-game/tools/node-setup/session-path.ps1')\"",
            "timeout": 10
          }
        ]
      }
```

Which shell Codex uses to run `commandWindows` is [ASSUMED] (cmd or PowerShell). The quoting above works under either, but it was not run under Codex, which is not installed here. `session-path.ps1` was tested in scratch with Windows PowerShell 5.1: it prints the line when the shared Node exists and nothing otherwise.

```powershell
# Codex SessionStart hook on Windows: prints the shared Node folder as session context. Fail-open.
$ErrorActionPreference = 'SilentlyContinue'
$top = git rev-parse --show-toplevel 2>$null
if (-not $top) { exit 0 }
$version = (Get-Content -Raw (Join-Path $top 'src/godot-game/tools/verification/toolchain.json') | ConvertFrom-Json).node
if (-not $version) { exit 0 }
$shared = $env:KZ_SHARED_LOCAL
if (-not $shared) {
  $common = git -C $top rev-parse --path-format=absolute --git-common-dir 2>$null
  if (-not $common) { exit 0 }
  $shared = Join-Path (Split-Path -Parent $common) '.local'
}
$dir = Join-Path $shared "tools/node/node-v$version-win-x64"
if (-not (Test-Path -LiteralPath (Join-Path $dir 'node.exe') -PathType Leaf)) { exit 0 }
$dir = [System.IO.Path]::GetFullPath($dir)
Write-Output "Shared Node $version for this repository. In each PowerShell command run first: `$env:Path = '$($dir.Replace("'", "''"))' + [IO.Path]::PathSeparator + `$env:Path"
exit 0
```

Also document the zero-config alternative: start `codex` from a shell that already evaluated the setup script's PATH line. Codex passes PATH to its commands (`core` and `all` both include PATH), but the default `inherit` value differs between sources [CITED: web search, MEDIUM]. Docs must say that the Codex hook is advisory context, not a PATH change, and that it needs a one-time `/hooks` trust.

## 3. Backlog convention

| Finding | Evidence |
|---|---|
| ROADMAP has no `## Backlog` and no `999.x` heading; `.planning/phases/` holds `01`–`08`, `11` and `12`; there is no `.planning/todos/` | `grep -n -i "backlog\|999" ROADMAP.md` matches nothing; `ls .planning` |
| STATE "Pending Todos" is prose about Phase 1 and 3 follow-ups. "Deferred Items" is a project table (`Category \| Item \| Status \| Deferred At \| Milestone`) for scope deferrals | `.planning/STATE.md:195-199`, `:234-244` |
| `/gsd-capture --backlog <text>` → `add-backlog.md`: write the ROADMAP entry **before** creating the folder; append `## Backlog` at the end of ROADMAP if missing; then `### Phase {NEXT}: {description} (BACKLOG)` with `**Goal:** [Captured for future planning]`, `**Requirements:** TBD`, `**Plans:** 0 plans` and `Plans:` / `- [ ] TBD (promote with /gsd-review-backlog when ready)`; then `.planning/phases/{NEXT}-{slug}/.gitkeep`; then commit | [VERIFIED: `.claude/gsd-core/workflows/add-backlog.md`] |
| `NEXT` = `999.1`, slug = `rename-phase-named-identifiers` | [VERIFIED: `gsd-tools query phase.next-decimal 999 --raw` and `generate-slug`] |
| GSD keeps 999.x out of phase counts and progress | sentinel handling in `roadmap.cjs:226,261` and `roadmap-command-router.cjs:55` (`SENTINELS = new Set([0, 999])`) [VERIFIED: read] |
| No repository code enumerates `.planning/phases/*`; `.planning/**` is skipped by the ac-08 rule. ROADMAP **is** a DocumentationValidator-owned document, so the docs group must pass after the edit | `DocumentationValidator.cs:87-96` |

**Recommendation:** the quick task writes the entry itself, following `add-backlog.md` without invoking the separate commit step. The entry goes in one docs commit on the task branch, with these parts:
- `## Backlog` at the end of ROADMAP, below the footer, as the workflow prescribes.
- `### Phase 999.1: Rename phase-named identifiers (BACKLOG)`. The goal names `Phase1.cs`/`Phase1.tscn`, the `KingdomZombies.Phase1.Tests` project, `PHASE1_*`/`PHASE2_*`/`PHASE3_*`/`KZ_PHASE4_*` variables, the `phase1`/`phase2`/`phase3` case families, the `Phase4 Windows inspection` preset and the VM user `/home/phase1`. It notes that renaming changes test FQNs and case IDs across the registers.
- `.planning/phases/999.1-rename-phase-named-identifiers/.gitkeep`.
- Optionally, one STATE "Deferred Items" row linking it. That table is where this project records deferrals; adding the row is a planner choice, not a creator decision.

A quick task may do all of this; nothing restricts backlog capture to phase work.

## 4. Rebinding tools to track

**Probe at `88953aa`** (scratch script, read-only):

| Check | Result |
|---|---|
| fsu gate script extracted from the PLAN markers | sha256 `daa62c2f34409450785496ee65ce8eb0c1ab1adbe257ef837fa60ca1f63e0892`, equal to the fsu SUMMARY line 83 |
| `acRows('HEAD', ac08)`: path set, kind by extension, owners, HEAD-blob sha256 | **0 bad rows** of 498 |
| `sha256(JSON.stringify(sources)) === sourceSetSha256`; `sha256(JSON.stringify(repositoryStages))` | true; `4a795aab…` = all 5 `callerStages.repositorySha256` in verification plus `core.permissions` in gameplay-checks |
| `repository-sources-*` stages = checked rows in order, chunks of 16 | true (230 rows → 15 stages, 25 stages in total) |
| `bindings[]` (7 registers) against file bytes | all current |
| Asset catalog `moduleSha256` | 423 rows, 30 modules, 0 stale; 19 modules naming `.phaseN` carry exactly **282** rows |
| `delivery.json fixtureCatalog` | `sha256`/`uncompressedBytes` match the catalog; `gzipBase64` equals `zlib.gzipSync(bytes,{level:9})` (level 6 does not) |
| `pack-index.json` | byte-equal to `JSON.stringify(content.normalizeCurrent(),null,2)+'\n'`; `content.validate(ROOT).valid.length` = 180 |
| Formatting | all three `required-cases.json`, `delivery.json` and `pack-index.json` are `JSON.stringify(x,null,2)+'\n'`, LF |
| `--list` from an unrelated working directory | verification 686, gameplay-checks 16, lan-checks 32, asset-pipeline 75 (unchanged from fsu) |

**What to build** (names follow the "specific names" discretion):

| Tracked tool | Modes | Reuses | Notes |
|---|---|---|---|
| `src/godot-game/tools/verification/ac08-bindings.cjs` | `--check` (default, read-only), `--write` | fsu `acRows`, `blobs`, `tree`; research §3c | `--write` rebuilds `sources`, `sourceSetSha256`, the `repository-sources-*` stages (other stages are kept in order), `projects` (tracked `*.csproj` under `src/godot-game/`) and `bindings` (register bytes) from **HEAD blobs**. It refuses a working tree that differs from HEAD in any hashed row. Then it rewrites `callerStages.repositorySha256` in both catalogs. It never derives `keys` (42, produced by C# `ExemptionValidator.ReadPermissions`) or `cohorts`; `--check` asserts `keys.length === 42` and that cohorts are unchanged. It writes with `JSON.stringify(x,null,2)+'\n'`. |
| `src/godot-game/tools/verification/asset-bindings.cjs` | `--check`, `--write` | fsu `bindings` gate (module walk, `phys`); `content.cjs` exports | Order: every `{module, moduleSha256}` → `delivery.json fixtureCatalog` (sha256, bytes, gzip level 9 base64) → `pack-index.json = normalizeCurrent()` → `validate` = 180 → `text-data.validateSupport()`. `projectGraphSha256` (export-profile JSON and module) is checked, not rewritten, unless `projects.json` changes. |
| `src/godot-game/tools/verification/rebind-gates.cjs` | gates `history`, `newhost`, `lists`, `layout`, `build`, `bindings`, `docs`, each with `--base <rev>` | fsu gates, generalised | `history`: paths matching the history pattern (`.planning/phases/0[1-4]-`, `.planning/quick/` except the current task, `docs/code/provenance/`, plus `--keep <pathspec>` for this task's 5 registers, 5 production JSONs and `platform-proof.md`) equal their `--base` blobs. `newhost`: no new machine-path line compared with base; generalise the `Users[/\\]User` term to any user folder. `lists`: counts equal the catalog lengths. `layout`: `node --test` exits 0 with 0 failures (do not pin 6, because tests will be added). `build`: locked restore with no `GET https?:` and 0/0 warnings and errors, using the **new** isolation environment from `workspace-paths.cjs`. `bindings`: calls the two tools in `--check` mode, plus `producerPlan` staged = 9 and the hashed registries. `docs`: `check.cjs --group docs` passes N/N. |

**Merge-specific parts to drop:** `merge` (PR × DEV merge topology), `moves` (206 mapped files), `state` (it pins the old "Phase 5" renewal bullet, §7), `records` (fsu record shape), the `MAP`/`RETIRED` move tables, and the `DEV`-relative comparisons of zip blobs, `extract-*.py` and `fields.md`. The last group becomes `history --keep`.

**How to test** (`src/godot-game/tools/verification/bindings.test.cjs`, `node --test`):
1. **Reproduction:** `ac08-bindings --write` against a temporary copy of the catalogs, rebuilt from HEAD blobs, produces bytes identical to the committed `required-cases.json` files. The same holds for `asset-bindings` with `delivery.json` and `pack-index.json`. That reproduces dev's current blocks byte for byte without depending on a fixed commit, so it also works in shallow CI clones.
2. **Negative controls:** in memory, flip one source sha, add a phantom path, drop a checked row, change one module byte, and use gzip level 6. Each one makes `--check` exit 1 with the named row.
3. **Dirty-tree refusal:** `--write` refuses when a hashed row's working-tree bytes differ from HEAD.

Run these tests **before** the literal replacement, so the tools are proven on unchanged bytes.

The STATE `[Process history]` concern says "One-shot refactoring/binding utilities must not be reapplied." The tools README must say these tools regenerate bindings from current bytes and are not replays of one-shot utilities. Phase 5 D-43 drops per-file **evidence** bindings for Phases 5–10, but the runners still enforce catalog `moduleSha256` and ac-08, so the tools stay needed.

## 5. Migration command

**Main checkout, read-only listing:** `.phase2/`, `.phase3/` and `.phase4/` are empty. `.phase1/` holds exactly `cache cli-home dotnet-home feed http-cache packages tools`. No links or nested `.git` were found.

| Legacy (main checkout) | Size | Target | Notes |
|---|---|---|---|
| `.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip` | 112 MB | `.local/downloads/` | Verify SHA-512 and SHA-256 against `toolchain.json` after the move |
| `.phase1/cache/SHA512-SUMS.txt` | 5.6 KB | `.local/downloads/` | |
| `.phase1/cache/phase4-exports/` (`.tpz` + `SHA512-SUMS.txt`) | 1.2 GB | `.local/downloads/export-templates/` | Must equal the path map entry for the pinned logical `.phase1/cache/phase4-exports/…` (`export-toolchain.json:8,11`) |
| `.phase1/cache/game.packages.lock.json` | 1.8 KB | not migrated (stray) | left to legacy cleanup |
| `.phase1/tools/Godot_v4.7.2-stable_mono_win64/` | 258 MB | `.local/tools/godot/Godot_v4.7.2-stable_mono_win64/` | Verify `executableSha256` |
| `.phase1/tools/linux-vm/` | 8.3 GB | `.local/tools/linux-vm/` | **Rebase first** (below). Contains `id_ed25519`, a private key: never copy it to a tracked location and keep its file mode. |
| `.phase1/tools/linux-prep/` | 398 MB | `.local/tools/linux-prep/` | It already holds `node-v24.22.1-linux-x64.tar.xz` and the official `node-SHASUMS256.txt` |
| `.phase1/feed/` | 3.4 MB | `.local/feed/` (the main checkout's own feed) | Verify the 4 pins |
| `.phase1/packages/` | 576 MB | `.local/nuget/packages/` | |
| `.phase1/http-cache/` | 60 MB | `.local/nuget/http/` | |
| `.phase1/cli-home/` | 37 MB | `.local/cli-home/` (per checkout) | |
| `.phase1/dotnet-home/` | 0 | — | Remove only if empty |

**VM disk chain:** the `system.qcow2` header holds `backing_file_offset = 0x210` and `size = 0x56`. That field is an **absolute** path ending in `.phase1/tools/linux-vm/downloads/ubuntu-24.04-minimal.img`, and the backing image is qcow2 (magic `QFI\xfb`). [VERIFIED: header bytes read] Before the rename, run in the VM folder `qemu/qemu-img.exe rebase -u -F qcow2 -b downloads/ubuntu-24.04-minimal.img system.qcow2`, which rewrites the header only, and check `qemu-img info --backing-chain system.qcow2`. Then rename the folder. A relative backing path resolves against the overlay's own folder [ASSUMED: QEMU documents this; not run]. The launcher `start-vm.cjs` builds every path from `__dirname` and survives the move; its tracked copy goes under `src/godot-game/tools/` per CONTEXT.

**Design:** a separate script, `src/godot-game/tools/workspace-migrate.cjs`, with a test, rather than another mode of `workspace-cleanup.cjs`. Cleanup only deletes and has a 14-test safety model; migration moves, rewrites a VM header and has different refusals. Export the safe helpers from `workspace-cleanup.cjs`, which today exports only `parseArgs, parseWorktrees, planArtifacts, planWorktrees, run, UsageError` [VERIFIED: `:345`]: `keepReason`, `isIgnored`, `hasTrackedFiles` and `isRealDirectory`. Resolve S through `workspace-paths.cjs`.

| Behaviour | Rule |
|---|---|
| Default | Dry run: print each row as `move`, `skip (already migrated)`, `skip (stray)` or `refuse (<reason>)`, with sizes. Exit 0 only if there would be no refusal. |
| `--apply` | One same-volume `rename` per row, which is all-or-nothing per item. Stop at the first failure and report what moved. A rerun is idempotent: a missing source with an existing target means "already migrated". |
| Refuse: target exists | Never merge into or overwrite an existing target. |
| Refuse: locks | Legacy `.phase1/check.lock` or `.phase2/desktop.lock`, any `S/locks/*`, or any `L/locks/*` in any listed worktree. |
| Refuse: running process | Any process whose executable or working directory is under the main checkout's `.phase1/` (Windows `Win32_Process.ExecutablePath`; Linux `/proc/*/exe` and `cwd`), including `qemu-system-x86_64` and the PIDs in `linux-vm/processes.json`. Advise `dotnet build-server shutdown` first: build servers hold analyzer DLLs inside `packages/` without being detectable by image path, and a failed rename is the backstop. |
| Refuse: unsafe source | A source that is not a real directory or file (links), contains tracked files, is not ignored, or contains a repository (reuse `keepReason`). |
| Refuse: wrong code | The main checkout's HEAD lacks `src/godot-game/tools/workspace-paths.cjs` (`git cat-file -e HEAD:…`), meaning its branch still uses `.phase1`. Also refuse if `.local/` is not ignored there (`git check-ignore -q .local/`). |
| Refuse: override | `KZ_SHARED_LOCAL` is set to anything other than the main checkout's `.local`. |
| VM | Rebase and check the chain before the rename. Refuse if `qemu-img` is missing or reports an error. |

Run order for the creator after the merge: switch the main checkout to `dev`. Its current branch `feature/05-ci-and-repeatable-game-testing` (`ecb423d`) is **tree-identical** to `88953aa` (`git diff ecb423d 88953aa` is empty), so nothing is lost. It is squash-merged, so deleting the local branch needs `-D`. Then run the dry run, `--apply`, and the cleanup dry run.

## 6. Seeding this task's build

The build and docs group run a locked restore and a strict build of `KingdomZombies.slnx`. `check.cjs --group docs` runs `dotnet restore … --locked-mode` and then `dotnet build` with `NUGET_PACKAGES` from the environment [VERIFIED: `verification/check.cjs:125-129,752-755`]. No Godot binary, archive or VM is needed. The layout tests need nothing.

| Need | Source in the main checkout | Status |
|---|---|---|
| 41 distinct locked packages (5 `packages.lock.json`) | `.phase1/packages/` (45 ids, 47 id/version folders) | **all 41 present** with `.nupkg.metadata` and `.nupkg.sha512` [VERIFIED: probe] |
| `godot.net.sdk/4.7.2` (MSBuild SDK, not in the lock files) | `.phase1/packages/godot.net.sdk/4.7.2` | present |
| Verified feed (4 nupkgs; `NuGet.config` source `godot-verified-archive`) | `.phase1/feed/` | present; sha256 equals all 4 `godotPackages` pins [VERIFIED] |
| http-cache, cli-home | — | not needed; created empty |

Commands (orchestrator, task worktree root, after §8):

```sh
MAIN=$(dirname "$(git rev-parse --path-format=absolute --git-common-dir)")
S="$MAIN/.local" L="$(git rev-parse --show-toplevel)/.local"
git -C "$MAIN" check-ignore -q .local/x || { echo 'add /.local/ to info/exclude first'; exit 1; }
mkdir -p "$S/nuget/packages" "$S/nuget/http" "$S/nuget/scratch" "$L/feed" "$L/cli-home" "$L/temp"
cp -R -n "$MAIN/.phase1/packages/." "$S/nuget/packages/"
for f in "$MAIN"/.phase1/feed/*.nupkg; do cp -n "$f" "$L/feed/"; cmp "$f" "$L/feed/${f##*/}" || exit 1; done
diff -rq "$MAIN/.phase1/packages" "$S/nuget/packages" && test "$(ls "$L/feed"/*.nupkg | wc -l)" = 4
```

Copy only, never move. Git Bash `cp` is GNU coreutils 8.32, where `-n` means no-clobber. Then build with the new isolation: `NUGET_PACKAGES=$S/nuget/packages`, `NUGET_HTTP_CACHE_PATH=$S/nuget/http`, `NUGET_SCRATCH=$S/nuget/scratch` (required once packages are shared; see PRE-RESEARCH §4), `DOTNET_CLI_HOME=$L/cli-home`, `TEMP`/`TMP`/`TMPDIR=$L/temp`. Pass `--disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false`, so that no server keeps handles inside the shared packages folder.

**Feed placement (discretion), recommended per checkout `L/feed/`.** `NuGet.config` needs a relative value (`../../.local/feed`), and `bootstrap.cjs:176` requires `toolchain.json` `feeds` to match it. `ExemptionValidator.CheckHash` also requires the package path to stay inside the root (§9a). Bootstrap re-creates the feed from the shared Godot archive with hash checks. Add `feed` and `cli-home` to the cleanup keep list; they are cheap, but losing the feed breaks a direct restore that needs an uncached Godot package.

## 7. Phase 5 consistency review (PR #11, squash `88953aa`)

`88953aa` changes 9 planning and docs files and no code. Contradictions with PR #6, the fsu records or this rename:

| # | Where | Contradiction | Fix in this PR |
|---|---|---|---|
| C1 | `.planning/STATE.md:213` `[PR #6 relocation]` | It says renewal of the gates is owned by Phase 11, but also that the recipes asserting `.phase4` "run only there until Phase 5 **renews** them". After this task the recipes fail fast with a named error in **every** checkout, the main checkout included. "Renews" also collides with the Phase 11 term. | Reword: the pinned recipes (export tracer, audio assembly) fail fast with `CAPTURED_ROOT_REQUIRED`, naming 05-CONTEXT D-46, until Phase 5 makes the Phase 4 checks it runs work in the relocated tree. The review-bound gates stay with Phase 11. |
| C2 | The fsu `state` gate pattern `^- \[PR #6 relocation\].*runner\.prelaunch.*Phase 5` | It encodes the superseded "Phase 5 renews" handoff. | Do not carry it into the tracked tools (§4). The fsu records keep their bytes; the ltj SUMMARY notes the supersession. |
| C3 | `docs/workflow/verification.md:57` (section "Existing Phase 1 Commands and Evidence", **outside** the amended section) | "Phase 5 owns genuine measured line/branch coverage…"; "Reuse requires the validated Phase 5 mechanism". PR #11 moved both to Phase 11 under TEST-12, and its precedence note covers only the "Phase 5 Execution and Reuse Policy" section. | Change the owner to Phase 11 (TEST-12) in that paragraph; one sentence. |
| C4 | `docs/workflow/cleanup.md:23` | "…which the Phase 5 reuse mechanism must define…". Evidence reuse validation moved to Phase 11 (05-CONTEXT D-36). | This task rewrites cleanup.md anyway; say "Phase 11 (TEST-12)". |
| C5 | `.planning/STATE.md` Session Continuity (`:248-254`) | "PR #5 … is ready for review; marking it ready does not authorize the merge" sits next to "PR #5 is merged as `1a14684`". "The amendment merges to `dev` through its own planning PR" is now done (`88953aa`). `Last session: 2026-11-22T22:30` / "Stopped at: Phase 4 closed" is stale. | This task updates STATE (quick-task row, last activity) anyway; drop the PR #5 "ready" sentence and state that PR #11 merged as `88953aa`. |
| C6 | `docs/architecture/repository-layout.md:19` | "Existing ignored `.phase1/`–`.phase3/` … remain stable. Phase 4 adds `.phase4/`." | Already in scope: rewrite it for `.local/` and add the `.phaseN → .local/…` rows to the historical map. |

Not contradictions (leave as written):
- 05-CONTEXT D-46 and 05-SPEC:94 describe what the pinned recipes **assume** ("the original checkout's `.phase4` folder"). That stays true, because their bytes are kept.
- The `.phase4` sizes (05-CONTEXT:225, DISCUSSION-LOG:264,270) and the `.phase1/temp/…` report path (05-CONTEXT:210) are historical measurements.
- 05-CONTEXT still names 14 retired pre-PR #6 paths, such as `tools/phase4/check.cjs` at line 187. The repository-layout historical map resolves them; refresh them only if the planner wants to.
- `ai-collaboration.md:51` ("Phase 5 must implement … reuse validation") is covered by PR #11's precedence note at line 40. Fix it in passing, since this task edits that file for the worktree rule.
- Pre-existing ROADMAP glitches not touched by PR #11: Phase 4 is still `- [ ]` in the phase list, the Phase 12 bullet appears twice (lines 43–44), and the Phase 11 bullet has no description. Optional fix; it is outside this task's subject.

## 8. Main-checkout untracked noise

- [VERIFIED] `git rev-parse --git-path info/exclude` from the task worktree returns the common dir's `info/exclude`. `git check-ignore -v --no-index .claude/worktrees/x` in the task worktree reports `<common-dir>/info/exclude:7:.claude/worktrees/`. The common exclude file therefore applies to every worktree. Today `.local/x` is not ignored in either checkout.
- **Exact line:** `/.local/`. Add it idempotently from any checkout:

```sh
EX="$(git rev-parse --path-format=absolute --git-common-dir)/info/exclude"
grep -qxF '/.local/' "$EX" || printf '%s\n' '/.local/' >> "$EX"
git -C "$(dirname "$EX")/../.." check-ignore -v .local/x   # expect: …info/exclude:N:/.local/
```

- Interaction with cleanup: the worktree-removal check counts only `.gitignore` files as marking files disposable (`workspace-cleanup.cjs:207-209`). In linked worktrees `.local/` is per-worktree only on branches that carry the new `.gitignore`, so removal still works. The main checkout is never removed. Once the main checkout's branch carries the new ignore rule, the exclude line is harmless; it may stay.

## 9. Corrections to PRE-RESEARCH

| PRE-RESEARCH claim | Status at `88953aa` |
|---|---|
| "Node is not available, so no repository tool ran" | Superseded. Node 24.22.1 ran the read-only probes in §4 and §6. |
| Live totals: src 74 files / 317 lines, docs 15 / 575 (same regex) | **Hold** at both `18e77bc` and `88953aa`. The docs split is 10 Markdown files (44 lines) and 5 JSON files (531 lines). |
| The regex misses computed patterns | Add `.gitignore:6` `/.phase[0-9]*/`, `workspace-cleanup.cjs:163` `/^\.phase\d+(\/\|$)/`, `:174` `/^\.phase\d+$/`, `cleanup.md:31,39` (`.phaseN`). |
| STATE.md history count | Now **2** live lines (`:193`, `:213`); PR #11 added `:193`. Live planning for Phases 5 and 6 names `.phaseN` 8 times (05-CONTEXT 3, 05-DISCUSSION-LOG 2, 05-SPEC 1, 06-DISCUSSION-LOG 2), all historical or descriptive. |
| §1: `.phase1/tools/phase4-*.json`, `.phase1/quick-*`, `phase4-pr-body.md`, `runs/`, `temp/`, `check.lock` exist in the main checkout | **No longer true.** The main checkout's `.phase1/` holds only the 7 durable folders and `tools/` holds only Godot, `linux-prep` and `linux-vm`. |
| §1 VM row: "includes an untracked launcher" | True, and also: `system.qcow2` has an **absolute** backing path (§5), and the folder holds a private SSH key. |
| §3: "5 `docs/code` registers … keep their bytes; treat `.phase1/feed/X.nupkg` as a logical package identity" | Correct for the bytes, but **incomplete**: tests also read these values **physically** (9a). |
| D8(b) `.claude/settings.json` [ASSUMED] | Now verified. That file is absent; `settings.local.json` is tracked and owned by GSD (§1). |
| D1 recommended `.tmp`; `KZ_SHARED_TMP` | Superseded by the creator's `.local`. Use `KZ_SHARED_LOCAL`. |
| 282 of 423 module rows; 25 C# files; 230 of 498 checked ac-08 rows | **Confirmed exactly** (§4). |

### 9a. Physical reads of register paths (new)

| Read site | Value read | Problem after the move |
|---|---|---|
| `EngineGeneratedTests.cs:321` (`Local(Text(row,"package"))`), `:766` (`SafePath(Root, package)`) | `.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg` | Must map to `L/feed/…`, which stays inside the root |
| `ExemptionValidator.cs:540-542`: `string package = engine ? ".phase1/feed/Godot.SourceGenerators.4.7.2.nupkg" : Text(row: row, name: "package"); CheckHash(relative: package, …)` | feed **and** `.phase1/packages/…nupkg` rows (`generated-callsite-adaptations.json:13,27,688,711`) | `CheckHash` (`:665-671`) rejects rooted or `..` paths (`SUPPRESSION_ORIGIN_PATH`). Mapping to the shared packages folder, which is outside a worktree's root, needs the narrow shared-root exception. Which rows reach `:542` is MEDIUM confidence (call path not traced); run `rules.exemptions` to confirm. |
| `GeneratedLayoutAdapterTests.cs:44`, `GeneratedSyntaxAdapterTests.cs:29` (`Hash(path) → Local(path)`); `GeneratedExportLayoutTests.cs:189` `Local` | `.phase1/packages/xunit.v3.core.mtp-v2/4.0.2/…`, `.phase1/feed/…` | Same mapping. The `Local` helpers are `Path.Combine(ProofProtocol.Root, path)` (`EngineGeneratedTests.cs:1312-1315`). |
| `EngineGeneratedTests.cs:259-265` `_producerRecipePaths` includes `src/godot-game/NuGet.config` and `toolchain.json` | file bytes | **No problem.** It is only an in-memory cache key (`ProducerKey`, `:698-711`), never a pinned value, so editing those files is safe. |

The C# `WorkspacePaths` helper therefore needs `Physical(logical)` for `.phase1/feed/` → `L/feed/` and `.phase1/packages/` → `S/nuget/packages/`. Every read site above calls it, and `CheckHash` gets the shared-root exception. This is in addition to the literals PRE-RESEARCH counted.

## Architectural Responsibility Map

| Capability | Owner | Secondary | Rationale |
|---|---|---|---|
| Location resolution (S, L, override) | `src/godot-game/tools/workspace-paths.cjs` + C# `WorkspacePaths` | runner env (`KZ_*`) passes it to C# children and source copies | One place for every `.local` path |
| Session PATH | `.claude/settings.json` + `node-setup/session-path.sh`; `.codex/hooks.json` + `session-path.ps1` / `--codex` | setup scripts print the per-shell line | The agent harness owns the session environment; nothing persistent |
| Toolchain acquisition | `bootstrap.cjs` (Godot, feed), `node-setup/setup.{sh,ps1}` (Node) | `S/locks/toolchain.lock` | Node cannot install itself |
| Binding regeneration | `verification/ac08-bindings.cjs`, `asset-bindings.cjs` | `rebind-gates.cjs` | Tracked and tested against HEAD bytes |
| One-time move | `workspace-migrate.cjs` | the safety helpers exported by `workspace-cleanup.cjs` | Moving and deleting are different safety models |

## Validation Architecture

| Property | Value |
|---|---|
| Framework | `node --test` (Node 24.22.1) for JS; xUnit v3 / MTP through `verification/check.cjs` for C# and docs |
| Quick run | `node --test src/godot-game/tools/repository-layout.test.cjs src/godot-game/tools/workspace-cleanup.test.cjs` plus the new test files |
| Full set | strict locked build + `node src/godot-game/tools/verification/check.cjs --group docs` + `rebind-gates.cjs history newhost lists layout build bindings docs --base 88953aa` |

| Requirement | Test | Command | Exists |
|---|---|---|---|
| Shared root resolves from the task worktree, the main checkout and an unrelated cwd; the override wins | unit tests on `workspace-paths.cjs` with temporary git repos and worktrees | `node --test src/godot-game/tools/workspace-paths.test.cjs` | ❌ Wave 0 |
| Session hook: idempotent, fail-open, override, spaces, Windows `CLAUDE_PROJECT_DIR` | test that runs `sh session-path.sh` with a fake shared root and a temporary `CLAUDE_ENV_FILE` | `node --test src/godot-game/tools/node-setup/session-path.test.cjs` | ❌ Wave 0 |
| Node setup matches the pin | run the setup once, then sha256 the archive against `toolchain.json` | the setup script itself plus a hash assertion | ❌ |
| Bindings regenerate byte-identically and detect drift | §4 tests | `node --test src/godot-game/tools/verification/bindings.test.cjs` | ❌ Wave 0 |
| Cleanup keep/delete table for `.local/`, shared locks, legacy `.phaseN` | extend the existing 14 tests | `node --test src/godot-game/tools/workspace-cleanup.test.cjs` | ✅ (extend) |
| Migration dry run, refusals, idempotence, no VM touch without `--apply` | temporary fake `.phase1` trees; VM rebase stubbed | `node --test src/godot-game/tools/workspace-migrate.test.cjs` | ❌ Wave 0 |
| Layout: case lists unchanged (686/16/32/75) | existing test 6 | `node --test src/godot-game/tools/repository-layout.test.cjs` | ✅ |

Sampling: quick run per commit; full set before the ac-08 commit and again at the end. Export, editor import, headless launch, game windows and the full case selection are not run (CONTEXT).

## Security Domain

| Concern | Control |
|---|---|
| Path traversal through the shared-root exception | Allow exactly S (resolved, real path, no links) for the named purposes; keep `PATH_OUTSIDE_WORKSPACE` and `SUPPRESSION_ORIGIN_PATH` for everything else. Test both an allowed and a rejected path. |
| Download integrity (Node) | SHA-256 pin in `toolchain.json`, checked before extraction. Download to `.part` with a nonce, then rename. Both CONTEXT pins match the official `SHASUMS256.txt` already in the main checkout's `linux-prep`. |
| Hook command injection | The hook quotes paths in single quotes with `'` escaped, reads no untrusted input and is fail-open. Codex hooks are trusted per hash. |
| Secrets | The VM folder holds `id_ed25519`. Migration renames it and never copies it. It must never move to a tracked location with the launcher. |
| Shared lock scope | Make the desktop lock shared (S). Cleanup in any checkout should refuse while `S/locks/desktop.lock` exists. Lock files record only `{pid, nonce, at}` and no checkout (`gameplay-checks/check.cjs:23`), so ownership cannot be attributed. |

No external packages are installed. The Package Legitimacy Audit does not apply; Node comes from nodejs.org with a pinned hash.

## Environment Availability

| Dependency | Available | Version | Note |
|---|---|---|---|
| git | ✓ | 2.56.1.windows.1 | `worktrees.root` is set in **global** config; `core.longpaths` is unset (exit 1) |
| dotnet | ✓ | 10.0.404 | matches the pin |
| node | session-only | 24.22.1 | not on PATH; that is what this task fixes |
| Windows PowerShell | ✓ | 5.1 | no `pwsh` |
| Git Bash `sh`, `cygpath`, GNU `cp` 8.32 | ✓ | — | |
| `qemu-img` | ✓ (main checkout, inside the VM folder) | — | needed only by the creator's migration |
| codex, claude CLIs | not on PATH | — | the Codex hook cannot be run here; the creator checks it once with `/hooks` |

## Assumptions Log

| # | Claim | Section | Risk if wrong |
|---|---|---|---|
| A1 | The Linux Node archive unpacks to `node-v24.22.1-linux-x64/bin/node` | §1 | The Linux hook silently does nothing; the setup script should assert the layout |
| A2 | Codex runs `commandWindows` under cmd or PowerShell, and the quoting works | §2 | The Codex context message is missing; it is advisory only |
| A3 | Codex `shell_environment_policy.set` does not expand `$PATH` | §2 | None; the option is rejected either way |
| A4 | A relative qcow2 backing path resolves against the overlay's folder, and `rebase -u` rewrites only the header | §5 | The VM does not boot after migration; reversible by rebasing back |
| A5 | The rows reaching `ExemptionValidator.cs:542` include the `.phase1/packages/…` packages | §9a | If not, the exception is needed only for the adapter tests |
| A6 | Claude Code asks the user to accept new project hooks before running them | §1 | First-session friction only |

## Decisions the creator still has to make

1. **Codex:** accept that Codex gets an advisory context line instead of a real PATH change, which is the platform's limit. If not, drop the Codex hook and document launching `codex` from a prepared shell instead.
2. **VM migration:** approve the migration rewriting the `system.qcow2` header (`qemu-img rebase -u`, reversible). If not, leave `linux-vm/` out of the migration and keep `.phase1/tools/linux-vm/` as a documented legacy exception.
3. **Main checkout branch:** after the merge, switch the main checkout from `feature/05-ci-and-repeatable-game-testing` (content-identical to `88953aa`) to `dev` before running the migration.

Planner discretion, no creator decision needed: per-checkout `L/feed/` and `L/cli-home/` (outside CONTEXT's subfolder list, justified in §6 and §9a); the `KZ_SHARED_LOCAL` name; the STATE "Deferred Items" row for 999.1; the optional ROADMAP glitch fixes.

## Sources

- Claude Code hooks reference (SessionStart, `CLAUDE_ENV_FILE`, merging, parallelism, Windows PowerShell limits): https://code.claude.com/docs/en/hooks (MEDIUM, official)
- Codex hooks (events, trust, `commandWindows`, stdout as context, both formats in one layer): https://learn.chatgpt.com/docs/hooks (MEDIUM, official)
- Codex configuration reference and advanced config (`shell_environment_policy`, project trust, ignored keys): https://learn.chatgpt.com/docs/config-file/config-reference.md, https://learn.chatgpt.com/codex/config-advanced (MEDIUM, official)
- Repository at `88953aa`: `.claude/settings.local.json`, `.claude/hooks/gsd-path-export.js`, `capability-registry.cjs`, `.codex/config.toml`, `.codex/hooks.json`, the Codex installer migration 002, `add-backlog.md`, `gsd-capture.md`, the fsu PLAN/RESEARCH/SUMMARY, `toolchain.json`, `bootstrap.cjs`, `verification/check.cjs`, `workspace-cleanup.cjs`, `ExemptionValidator.cs`, `EngineGeneratedTests.cs`, `DocumentationValidator.cs`, `git show 88953aa`, plus the read-only probes described above (HIGH)
- Main checkout, listed and hashed read-only: `.phase1/*` sizes, feed sha256, the qcow2 header, `linux-prep/node-SHASUMS256.txt`, `.git/info/exclude` (HIGH)
