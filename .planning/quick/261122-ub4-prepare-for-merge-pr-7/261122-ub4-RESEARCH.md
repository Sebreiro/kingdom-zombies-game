# Quick task 261122-ub4: prepare PR 7 for merge - Research

**Researched:** 2026-11-22, by reading only: PR `f373400`, `origin/dev` `1a14684`. Nothing was run or tested.
**Labels:** a claim with a file:line was read there [VERIFIED by reading]. **[inferred]** means concluded from code, not observed. Dev line numbers unless marked "PR" or "merged".

## User constraints

`261122-ub4-CONTEXT.md` is authoritative. Merge dev without rewriting history, fix the six losses, correct the docs. No tests, builds or probes, no new tests. No edits to catalogs, digests, game code or Phase 1-4 tools. Only the orchestrator deletes, pushes and edits the PR. Never merge the PR.

## Decide before deletion

1. **CONTEXT says two checks will fail. A third input breaks three more:** `.phase4/runs/81c4c6a6-8981-43b7-ae89-0d557dd8a54c` (section 7, row A). The creator's "no exception" decision did not know of it.
2. The saved Phase 2 completion check also loses its inputs (row D).
3. The merge alone fails three Phase 1 cases until a catalog follow-up (section 6).

## 1. Smallest fixes in `tools/workspace-cleanup.cjs`

| Loss | Fix |
| --- | --- |
| 1 | PR:134: `git(p, ['ls-files','-v','-z']).split('\0').some(line => /^[a-zS] /.test(line))`. |
| 2 | PR:81: replace `existsSync(child/.git)` with `keepReason(child)`. At PR:75-77, after the tracked and ignored checks, keep the whole root if it is itself a repository or a registered worktree. Otherwise `.phase4/.git` is just another child. |
| 3 | PR:95: pass each build entry through `keepReason`. `buildOutputs` needs `worktreePaths`. |
| 4 | Last in `worktreeState`: refuse if another registered worktree is inside, or `keepReason` finds a repository or cannot scan. Skip only the worktree's own top-level `.git`. |
| 5 | In `worktreeState`, refuse if `findLock(worktree.path)`. Check again in the `requested` mapping (PR:197), because the scans can take minutes. |
| 6 | PR:204: `findLock(root)` over both fixed paths in section 2. |

**Flags:** git's `git-ls-files.html` gives `H` tracked, `S` "tracked file that is skip-worktree", `M` unmerged, and for `-v` "use lowercase letters for files that are marked as assume unchanged". So `h` is assume-unchanged and `s` is both. Sparse checkouts will be refused, which is safe.

**`keepReason(target, worktreePaths)`:** one helper for losses 2-4.

- A registered worktree at or inside the target keeps it, even without a `.git`. Never test the reverse direction: the main checkout contains every output.
- `lstat` the target. A file or link needs no scan.
- Walk with an explicit stack and `readdirSync(dir, {withFileTypes: true})`. A directory is a repository if it has an entry `.git` of any type, or all of `HEAD`, `objects` and `refs` (bare). Compare names in lower case. Stop at the first hit.
- Skip `isFile()` entries without a stat. `lstat` the rest and descend only into real directories. Do not trust `isDirectory()` alone: libuv types every Windows reparse point as a link, including non-link ones that `rmSync` would enter [CITED: libuv `src/win/fs.c`, `fs__scandir`].
- Any readdir or lstat error means keep or refuse, reason `could not be scanned (<code>)`. Do not use `existsSync` or the error-swallowing `isRealDirectory` here.
- Reason is exactly `contains a Git worktree or repository` when the hit is the target (test 8 needs it). For a deeper hit append ` at <relative path>`.

**Cost [inferred]:** one readdir per directory, no content reads, so 100 GB does not matter. About 1 million files means seconds warm and possibly minutes on cold NTFS. Never scan the seven durable caches in `classifyRoot`. Scan a worktree only after its cheap checks pass. A recursive readdir would hold every entry and lose per-directory error handling.

**`findLock(dir)`:** `lstatSync` each path. Only `ENOENT` and `ENOTDIR` mean absent. A dangling link is a lock and any other error throws.

**Buffer:** PR:21 uses execFileSync's 1 MiB default, and overflow kills the child [CITED: nodejs.org v24 child_process]. `ls-files -v` on dev is about 138 KB (2,734 paths), but `status --untracked-files=all` and the ignored listing pass 1 MiB at roughly 10,000 paths and would stop all cleanup. Set `maxBuffer: 256 * 1024 * 1024` in `git()`.

**Order:** list worktrees, plan and scan artifacts, recheck requested worktrees' locks, check root locks, refuse, and only then delete, prune and remove. PR:204-214 already refuses before the first mutation. Keep every new check above it. A removal that Git itself fails later can still follow artifact deletion (exit 1).

**Side effect:** `tools/phase3/revisions.test.cjs:7,11` creates `.phase3/revision-control-*` repositories and never removes them, so cleanup will always keep them. A repository deep in `.phase1/temp` or `.phase4/runs` keeps that whole child. Report kept paths, do not bypass. `git worktree list` shows no worktree under a phase root today.

## 2. Fixed-path runner locks

| Lock | Taken | Used by |
| --- | --- | --- |
| `.phase1/check.lock` | `tools/phase1/check.cjs:713-715`: `path.join(home, 'check.lock')`, `home` = `path.join(root, '.phase1')` (:711) | Every Phase 1 check |
| `.phase2/desktop.lock` | `tools/phase2/check.cjs:18-21`: `inside(root,'.phase2/desktop.lock')` | Phase 2 native (:103), `tools/phase3/network.cjs:94,136,196,319`, `tools/phase3/scenarios.cjs:330`, `tools/phase4/scenarios.cjs:1550` (kept on purpose after unconfirmed teardown, :1555; released by recovery, :1658-1662) |

There are no others in `tools/`. Do not block on these:

- `publication.lock` (`tools/phase4/content.cjs:514,537`): relative to the publication root, which the cases put in their run folder.
- Copies of either lock in fixtures or nested runs (`tools/phase3/runner-fixture.cjs:99`, `tools/phase4/cases/runner.cjs:341`).
- `observation.owner` (`tools/phase3/check.cjs:70`, per run) and `profiles.lock` (game profile folder).

Bootstrap, builds and most no-window Phase 2-4 checks take no lock, so "never clean while something runs" stays the agent's duty.

## 3. The 14 existing tests, by reading

| # | Test | Agrees, because |
| --- | --- | --- |
| 1 | dry run | durable names skip the scan, three deletes remain |
| 2 | tracked or unignored roots | those checks precede the repository check |
| 3 | build | no repository in the folders |
| 4 | clean pushed | own `.git` skipped, main is not inside the target |
| 5 | dirty, unpushed, current, main | existing checks precede scan and lock |
| 6 | prune | missing directory is decided before scan and lock |
| 7 | arguments | unchanged |
| 8 | nested worktree | only with the exact reason string above |
| 9 | hidden untracked | status runs first |
| 10 | junction | not followed, absent lock is ENOENT |
| 11 | case | lower-cased durable check precedes the scan |
| 12 | lock and refusal | same exit and refusal text |
| 13 | Git failure | `rev-parse` still throws |
| 14 | worktrees-only | no lock or repository in the target |

All agree [inferred]. None covers a new fix.

## 4. Documentation

### Edit

| File:line | Problem | Minimal wording |
| --- | --- | --- |
| `cleanup.md` PR:17 | Delivered checks still read ignored inputs | Add: "Known exceptions: `export.layout`, `export.profile`, the two `rules.export-layout-*` cases and the `ContentNativePermissionTests` model tests read ignored `.phase4` inputs. They fail after cleanup until those inputs are tracked or regenerated." |
| `cleanup.md` PR:28-31 | "Delete" is not absolute | Under the table: "The command keeps and reports a folder that contains a repository or cannot be scanned. After cleanup, rerun the setup isolation step before direct builds; it recreates `.phase1/temp`." |
| `cleanup.md` PR:40 | Removal takes real cache folders too | "Removal deletes everything ignored inside it, including caches that are not links." |
| `cleanup.md` PR:43 | Refusal list incomplete | "...refuses worktrees with uncommitted changes, hidden index flags, unpushed commits, a runner lock, a nested worktree or repository, or a folder it cannot scan." |
| `cleanup.md` PR:67 | False: named removal deletes outside the checkout and deletes tracked files | Start: "Apart from removing a worktree named with `--remove-worktree`, the command never deletes ...". End that list: "a folder with a worktree or repository (`.git` entry or bare layout) at any depth, or a folder it cannot scan." |
| `cleanup.md` PR:69 | One lock | "`--apply` deletes nothing while this checkout has `.phase1/check.lock` or `.phase2/desktop.lock`, or when a requested worktree is refused, including for either lock in that worktree. Inspect a lock before removing it. Other processes take no lock; stop them first." |
| `ai-collaboration.md` 41 (merged 46) | Reports immutable without limit | "Keep original reports, tested revisions and invocation identities immutable until task-end [cleanup](cleanup.md); reuse in later tasks needs durable evidence." |
| `verification.md:13` | "Keep original reports" | "Until task-end [cleanup](cleanup.md), keep original reports, ... Reuse across tasks needs durable evidence." |
| `verification.md:45` | "historical evidence intact" | "Keep tested inputs fixed and never rewrite historical records." |
| `verification.md:5,51` | "retained" baseline | "recorded" in both. |
| `foundation-checks.md:53` | :60, :69, :90, :93 need old runs | Append: "Runs are deleted at task end; run repair, completion and validation commands before [cleanup](cleanup.md)." |
| `lan-foundation-checks.md:24` | Saved verification cannot work | "Verify completion before cleanup; the saved Phase 3 record's local reports are no longer kept." |
| `lan-foundation-checks.md:56` | "Preserve failed reports." | "Preserve failed reports until task end, then record them before [cleanup](cleanup.md)." |
| `docs/content/production.md:55` | Implies lasting run folders | Append: "Run folders are deleted at task end under [workspace cleanup](../workflow/cleanup.md)." |
| `engine-defects.md:67` | "exists only on the machine" | See the evidence table. |
| `platform-proof.md`, after title | Nine lines present local copies as existing | See the evidence table. Do not rewrite the record. |
| `05-CONTEXT.md:189` | "locally retained" | See the evidence table. |
| `.planning/STATE.md:65` | "earlier failed runs remain preserved" | "earlier failed runs remain recorded." |
| `.planning/STATE.md:227` | "Preserve immutable reports" | "Preserve recorded results, reports until task-end cleanup, and distinct identities..." |

`setup.md`, `local-testing.md:56`, `gsd.md` and the AGENTS link are correct after the PR. No test or tool contains the sentences changed above, and no digest of these docs is pinned outside a historical summary.

### Local-run citations in live docs

| Doc | Result committed? | Reproduction committed? | Sentence |
| --- | --- | --- | --- |
| `engine-defects.md:67`, `.phase4/temp/animation-import-unicode/` | Yes, :35-42 and the debug record | Policy self-test only (:53). The trace script and fixture are not. | "Raw traces and logs were local and are deleted by cleanup. The results above and the debug record remain; the original trace cannot be replayed from the repository." |
| `platform-proof.md:15,27,59,65,84,106,115,157,190` | Yes, counts, identities and hashes | Main commands yes (:69-72, :110-113, :152-154). Direct-proof receipts are local only (:25-27). | "This is a historical record. The Windows `.phase1/temp` and `.phase1/runs` copies it names are removed by cleanup; the recorded results and commands remain." |
| `05-CONTEXT.md:189` | Yes, :188 and the timing table | Yes, via `platform-proof.md:72` | "Its local report was at `...`; cleanup removes it, so the figures here are the record." |
| `foundation-checks.md`, `lan-foundation-checks.md`, `production.md`, `verification.md:47` | Generic output paths, no result cited | Yes, commands | As above. `verification.md:47` is fixed by the PR. |
| `docs/code/generated-layout-adaptations.json` (`exportSources`, :230-390) | Hash, size and line only, not the bytes | No: `export.layout` reads the old bytes before it regenerates (`export-layout.cjs:70`) | None. Bound by digest, so follow-up only. |

### Historical records citing local runs (leave alone)

- `04-CLOSURE-EXCEPTION.md:52` (`88bdbc40-...`): result, error and source line are committed, the command is not. The PR can give `node tools/phase4/check.cjs --case world.replacement`.
- `04-24-SUMMARY.md:68-71,94-103,109-112`: results committed. Per-case logs and the local qualification script are lost.
- `.planning/debug/phase04-import-surrogates.md:21,50-52,64,76,171-173` (its reproduction at :52 needs the runs) and `phase04-body-shutdown.md:48-93`.
- Other phase plans, summaries, discussion logs and JSON evidence, `WINDOWS.md`, and dated STATE decisions.
- `production/phase4/*.json` run paths are provenance and digest-bound.

## 5. Merge

Exactly two conflict hunks.

- `.gitignore`: keep `/.phase[0-9]*/`. It covers dev :6-9 and nothing tracked sits under a phase root.
- `.planning/STATE.md`: keep dev's file and both rows after `261114-we2`, with `261119-15c` before `261120-09v`.
- `AGENTS.md` and `verification.md` merge cleanly. Check that the cleanup and engine-defects rows both survive.

Semantic conflicts:

- Section 6.
- Section 4: dev's new engine-defects and production text assumes retained runs.
- `05-CONTEXT.md` D-31 needs evidence that survives cleanup. `cleanup.md` PR:21 already says so, no edit.
- The docs and AGENTS edits change Phase 4's source identity (`tools/phase4/evidence.cjs:26-27,125-129`), so its saved review cannot validate on the merged tree.
- `STATE.md:243` still says "after PR #5 merges". It is merged as `1a14684`. Optional fix.

## 6. Repository-shape follow-up (do not edit here)

**Yes, they fail once the files are tracked** [inferred from code]. `ExemptionTests.cs:1042-1047` requires tracked checked paths to equal the declared list, else `REPOSITORY_EXACT_TRACKED_UNION`. `SourceFileFormat.cs:135-140,172-178` exempts `.planning/` and binary extensions, so exactly three PR files are undeclared:

| New row, kind `outside-validator`, `owners: []`, `sha256: null` | Position in `tools/phase1/required-cases.json` |
| --- | --- |
| `docs/workflow/cleanup.md` | between ai-collaboration (:2145) and engine-defects (:2151) |
| `tools/workspace-cleanup.cjs` | after `tools/phase4/scenarios.cjs` (:5278-5282) |
| `tools/workspace-cleanup.test.cjs` | last |

Failing: `ac-08.complete`, `R-50.positive` (shape stage :8693) and `rules.exemptions` (:16619), the same three that commit `5c637da` repaired for the Python pin files.

Digests to refresh:

| File:line | Field | Current value |
| --- | --- | --- |
| `tools/phase1/required-cases.json:5284` | `sourceSetSha256` (check.cjs:175) | `8038db865fcf719acacdaae060cf545b1b2c5bf4bb147b2a78a4a71c5686c0d1` |
| same file :8676, :11920, :16335, :16453, :16606 (`R-50.positive`, `rules.godot-callsite-boundary`, `rules.godot-companion-positive`, `rules.godot-companion-negative`, `rules.exemptions`) | `repositorySha256` (check.cjs:332) | `69a44bbd1315f6b0738ff0b2cdaf58ee4e802e997268e56e2e71311ab54f3a5f` |
| `tools/phase2/required-cases.json:43` (`core.permissions`) | `repositorySha256` | same value |

Modified files need nothing: their rows have `sha256: null`. No other tracked file pins either catalog's file hash.

## 7. Delivered code that needs existing ignored files

### Works from these files now and fails after deletion [inferred]

| | Needed path | Reader | Fails |
| --- | --- | --- | --- |
| A, **new** | `.phase4/runs/81c4c6a6-8981-43b7-ae89-0d557dd8a54c/{ExportDebug,ExportRelease}/source/...`: the six 200-byte `.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` files named by `exportSources[].surface` | `tests/phase1/GeneratedExportLayoutTests.cs:40,68,153`; `tools/phase4/cases/export-layout.cjs:70` | Phase 1 `rules.export-layout-origin` and `rules.export-layout-boundary` (in `--phase 4` and `--all`), Phase 4 `export.layout` (Critical, selected) |
| B, known | `.phase4/checkpoints/04-02-entry-93386745/manifest.json` and every file it lists | `tools/phase4/cases/export-profile.cjs:14,254-257` | `export.profile` (Critical, selected). Not `export.profile-boundaries`. |
| C, known | `.phase4/04-23-current-model/normalized-index.json` | `tests/phase1/ContentNativePermissionTests.cs:120-121,1988-1989` | The 16 test methods at :118-600 that load the model, in a direct or whole-class run. No cataloged case selects them: `content.native-permissions` runs only `Check_PresenterNative_...` (`census.cjs:738`), which is unaffected. |
| D, **new** | Four `.phase2/runs/win32-x64/<id>/summary.json` (`02-PLATFORM-EVIDENCE.json:9233-9250`) and their images and artifacts | `tools/phase2/check.cjs:60-62`, `evidence.cjs:107-115`, no currency check | `node tools/phase2/check.cjs --verify-completion .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` (historical record, delivered command) |

With section 6, Phase 1 `--phase 4` loses five cases: three from the merge, two from A.

### Needs these files, but already fails on dev [inferred]

| Check | Needed | Why it already fails |
| --- | --- | --- |
| `runner.prelaunch` | `04-REVIEW.json` build report `.phase4/runs/eb05f596-.../review-build.json` and what it references, 18 fix proofs, all 776 `04-SELECTION.json` receipts under `.phase1` to `.phase4` runs, and a scan of `.phase4/runs` (`scenarios.cjs:309,322,669,824-840`) | `scenarios.cjs:300` needs `5c637da` as an ancestor of HEAD. The squash merge `1a14684` does not contain it (git checked). |
| `runner.selection` | The same build report and fix proofs | Same |
| Phase 4 rendered groups (18 own, 21 inherited) | The same plus the portable package in run `b0111601-...` (`scenarios.cjs:1499-1523`) | Same. **Phase 5 imports these IDs and needs a new review build and receipts.** |
| `acceptance.artifacts`, Phase 4 `--verify-completion` | The same | `04-COMPLETION.json` was never written |
| Phase 3 `--verify-completion` of `03-EVIDENCE.json` | 2,691 `.phase*` references, including run `f736803d-...` | Inputs and revision moved (`evidence.cjs:197-198`) |

### Authoring only, no case reaches them

- `ui-kit.cjs:42` `authorKit` reads `.phase4/04-14-font-inspection.json`. :115 `authorIdentity` reads `.phase4/04-28-art.json`. Both are exported with no caller and no CLI. `layouts.kit` and `layouts.identity` do not call them.
- `audio.cjs --publish-interface <dir>` needs a staged `.phase4` folder from `--prepare-interface`, which can be regenerated.
- Recipes retained in tracked zips cannot be replayed as authored: `ui-identity` (`.phase4/04-28-*`), `bodies-a/b/c` (`04-06`, `04-07`), `items-*` (`04-05`), `scale-pilot` (`04-04`), and `main-buildings`, `world` and `work-special` (inputs beside the recipe in its former scratch folder). Their cases read zip members. `world.cjs:100` uses the `.phase4/04-13-authoring` path only as a label and `defenses.cjs:53` executes from memory.

### Not inputs

- `export-layout-origin.json` `capture` and `originalFailedRun`, and the register's `generation` and `compilerAttempt`: no reader. Code compares `exportSources` as JSON and opens only `surface`, which is row A.
- `audio-feasibility.json` `directory` and `export-profile.json` `checkpoint`: not dereferenced. `export.profile` uses its own constant.
- `--failed`, `--complete` and `--record-interruption` take earlier runs as arguments by design.
- `.phase1/feed`, `packages`, `tools` and `cache` (including `cache/phase4-exports`) are read widely and are kept.
- Phase 2/3 and some Phase 4 child environments set TEMP to `.phase1/temp` without creating it (`phase2/check.cjs:143`, `phase3/check.cjs:25`, `hub.cjs:9`). Rerun setup after deletion [inferred].
- Deleting the phase roots themselves, which the script does not do, breaks direct runs of tests that `mkdtemp` in `.phase3` and `audio.cjs --technical` in `.phase4`. The Phase 3 runner recreates its root.

**Limits:** found by searching all tracked text for phase paths, following committed JSON that code opens, and reading script members of the 14 production zips. Paths built entirely at run time from external arguments, and the contents of the ignored folders themselves, were not examined.
