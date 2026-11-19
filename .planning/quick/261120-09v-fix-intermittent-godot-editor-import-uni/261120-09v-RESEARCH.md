# Quick Task 261120-09v: Editor-import thread policy in the Phase 4 runner - Research

**Researched:** 2026-11-20
**Domain:** Node.js harness (`tools/phase4/check.cjs` `command()`), Godot 4.7.2 project settings text format
**Confidence:** HIGH (all findings come from files read in this session; Godot behaviour read from the pinned source clone at `ed1daf0b`)

<user_constraints>
## User Constraints (from CONTEXT.md)

### Locked Decisions
- Root cause is established: a `DirAccessWindows` cwd race during threaded editor import. Setting `editor/import/use_multiple_threads=false` removes it. Do not re-investigate.
- The fix goes in the harness choke point `command()` in `tools/phase4/check.cjs`. The pinned engine is not patched.
- For every command with `--import`, the runner checks that `--path` names a run-owned project under `context.directory` that contains `project.godot`. It then makes sure that file sets `[editor] import/use_multiple_threads=false` before it starts the process.
- `override.cfg` cannot be used because the editor ignores it.
- A project that already sets `import/use_multiple_threads=false` stays byte-identical.
- Any other explicit value (for example `true`) is rejected before spawn with `EDITOR_IMPORT_THREADED`. The runner does not rewrite it.
- An `--import` command whose `--path` is missing, outside the run-owned directory, or has no `project.godot` is rejected before spawn with `EDITOR_IMPORT_PROJECT`.
- Commands without `--import` (`--script`, `--export-*`, `--build-solutions`, `--version`) are not changed.

### Claude's Discretion
- Test: `tools/phase4/editor-import.test.cjs` with `node:test` and a Node child process standing in for Godot, written first so it fails before the fix. **See Pitfall 1 before you keep this file name.**
- `.planning/debug/phase04-import-surrogates.md` is updated to `resolved`.
- `src/godot-game/project.godot` is out of scope (follow-up). Upstream Godot report is out of scope (follow-up).

### Deferred Ideas (OUT OF SCOPE)
- Changing the game project settings; reporting the engine defect upstream.
</user_constraints>

## Summary

All 24 harness editor imports go through `command()`. Nothing under `tools/phase4/` starts Godot any other way. Every `--path` is an absolute directory created under the same `context.directory` that is passed to `command()`. Every one of those directories has a `project.godot` written or copied before the import runs. No project, inline string, or tracked file sets an `[editor]` section or `use_multiple_threads`. No case reads a run-owned `project.godot` back or hashes it after an import. Godot merges repeated section headers, so appending one `[editor]` block is safe.

The one significant planning risk is the test file name. A new tracked file anywhere in the repository breaks the Phase 1 `REPOSITORY_EXACT_TRACKED_UNION` proof unless 1 catalog row is added and 7 derived hashes are recomputed. Put the test inside `check.cjs` as a `--self-test-editor-import` mode. Phase 4 already uses this pattern in `export-profile.cjs` `helperSelfTest`, and it needs no catalog changes.

**Primary recommendation:** Add a synchronous `editorImport(base,resolvedCwd,name,args)` check in `command()` between line 167 (env validation) and line 168 (`environment()` creates directories). It must throw before any record or spawn, and it should reuse `need`, `inside` and `bytes`.

## Findings

### Q1. Hook placement and error style in `command()`
[VERIFIED: tools/phase4/check.cjs:160-192]
- Line 161 checks `COMMAND_TIMEOUT_RANGE`. Lines 162-165 check cwd ownership: `need(resolved===base||resolved.startsWith(base+path.sep),'COMMAND_CWD_OWNERSHIP',name);` then `if(resolved!==base)inside(base,path.relative(base,resolved).replaceAll(path.sep,'/'));`. Lines 166-167 check `COMMAND_ENV_INVALID`.
- Line 168: `const childEnv=env??environment(context.directory);`. `environment()` (lines 143-152) calls `fs.mkdirSync` for `home`, `temp`, `data` and similar. This is the first side effect.
- Line 169 calls `spawn(...)`. Line 177 saves `<name>.exit.json` on exit. Line 188 saves `<name>.command.json`, `.stdout.log` and `.stderr.log`.
- **Insert the policy after line 167 and before line 168.** At that point `base` and `resolved` (the child cwd) already exist. A rejection there leaves no record, no directories, and no process.
- Error style: `function need(condition,code,detail=''){if(!condition)throw new Error(code+' '+ID+' '+detail);}` (line 95), with `const ID='common.bow';` (line 9). New messages will read `EDITOR_IMPORT_PROJECT common.bow <name>`. Call it as `need(cond,'EDITOR_IMPORT_…',name)`, the same way the `COMMAND_*` checks pass `name`. Existing negative tests match `message.startsWith(code+' ')` (export-profile.cjs:304) or a regex such as `/COMMAND_TIMEOUT_RANGE/` (export-profile.cjs:460).
- Reusable helpers: `inside(root,relative)` (lines 96-108) throws `PATH_INVALID`, `PATH_ESCAPE`, `PATH_REPARSE` or `PATH_CASE`. `bytes(root,relative,limit)` (lines 109-113) throws `RESOURCE_MISSING` or `RESOURCE_SIZE`, rejects 0-byte files, and has a 1 MiB default limit. `document()` shows the UTF-8 decode pattern: `new TextDecoder('utf-8',{fatal:true})`. Check that `project.godot` exists with `need(...,'EDITOR_IMPORT_PROJECT')` before you call `bytes()`, so a missing file reports the locked code. To keep one code, wrap the `inside()` call in try/catch and rethrow as `EDITOR_IMPORT_PROJECT` with the original message as the detail.
- `save()` (lines 139-142) writes with `{flag:'wx'}`. Optional evidence such as `<name>.import-policy.json` is safe to save only after the policy passes.
- Module exports (line 540): `module.exports={main,registerCase,cases,runTracer,validate,hash,inside,document,command,environment};`. Cases reach `command()` through `runner.command` (for example export.cjs:3, a lazy `require('./check.cjs').command`). A change inside `command()` therefore covers all callers.

### Q2. Every `--import` call site
Found by grepping `'--import'`, `'--editor'`, `spawn`, `execFile` and `fork` across `tools/phase4/**`, then mapping each site with a script. Godot is never spawned outside `command()`. The other spawns start `taskkill`, `git`, `dotnet` and `node` (check.cjs:156,201; hub.cjs:25,38; data-foundation.cjs:34; integration.cjs:202; content.cjs:548). Phases 1-3 and `tests/` contain no `--import`.

| Site | `--path` | Created as | project.godot source |
|---|---|---|---|
| check.cjs:392 `native-import` | `project` | `inside(context.directory,'project')` | copied from `src/godot-game` (line 390) |
| export.cjs:267 `export-import` | `project` | `inside(source,'src/godot-game')`, `source=inside(context.directory,'source')` | copied from tracked `src` files (lines 238-243). Same dir is then used for `--export-debug` (line 270) |
| animation.cjs:143 | `project` | `inside(context.directory,'animation-native')` | inline (137) |
| audio.cjs:1500 | `project` | `…'native-audio-hub'` | copied (1480) |
| bodies-a.cjs:171 | `project` | `…'native-character'` | copied (162) |
| bodies-a.cjs:250 | `project` | `…'character-matrix'` | inline (245) |
| bodies-b.cjs:161 | `project` | `…'body-matrix'` | inline (156) |
| corpses.cjs:48 | `project` | `…'appearance-native'` | inline (43) |
| defenses.cjs:153 | `project` | `…'native-defense'` | inline (140) |
| export-profile.cjs:216 | `importer` | `runner.inside(context.directory,name+'-importer')` | inline (211). Separate asset-only fixture |
| hub.cjs:412 | `project` | `path.join(context.directory,'native-project')` | copied (386) |
| items.cjs:346 | `project` | `…'native-items'` | inline (342) |
| main-buildings.cjs:240 | `project` | `…'native-building'` | inline (226) |
| media.cjs:96 / :306 | `project` | `…'native-conversion'` / `…'native-scale'` | inline (90 / 300) |
| text-data.cjs:236 / :237 | `controller` / `hub` | `…'native-controller'` / `…'native-hub'` | inline (236) / copied (237) |
| ui-inventory.cjs:150 | `project` | `…'native-inventory'` | inline (146) |
| ui-kit.cjs:19 / :240 / :420 | `native` / `project` / `project` | `…'native-icon'` / `…'native-identity'` / `…'native-ui'` | inline (15 / 235 / 414) |
| world.cjs:243 / :389 / :840 | `project` | `…'native-world'` / `…'native-effects'` / `…'native-inspector'` | inline (235 / 384) / copied (827) |

- In every site, the `context` passed to `command()` is the same object whose `.directory` created the path. All paths are strict descendants, never `context.directory` itself. All are absolute, made with `path.resolve`/`path.join` from `inside()`, so they use backslashes and the drive case of `ROOT`.
- **Nothing bypasses `command()`.** The policy covers all 24 imports.
- No site has an `[editor]` section today. A grep for `\[editor\]|use_multiple_threads` over `tools src tests docs` returns nothing. The inline strings set only `[application]`, `[dotnet]`, `[display]` and `[rendering]`, and all 18 end with `\n`. The copied game file `src/godot-game/project.godot` has sections `[application] [display] [dotnet] [physics] [rendering]`, uses LF (`git ls-files --eol` → `i/lf w/lf attr/text=auto eol=lf`), has no BOM (first bytes `; E`), and `.gitattributes` sets `* text=auto eol=lf`. [VERIFIED: src/godot-game/project.godot, .gitattributes]

### Q3. Is a run-owned project.godot read back or hashed?
None found. Each hashing path either targets the source tree or skips the imported directory:
- `fingerprint(root)` (check.cjs:193-198) hashes `'src/godot-game/project.godot'` under `root`, which is `ROOT` or the `subject` copy. It never hashes `context.directory/project`. `tracerFailures` compares `fingerprint(missingSubject)` (line 490). The subject and the imported `project` are different directories (lines 389 and 440).
- export.cjs: `snapshot()` (lines 11-19) and `inputHashes` (line 285) hash `ROOT` files. The exported PCK's `project.binary` will now contain the setting, but only its presence is checked (`required` list, line 109). Nothing pins its hash.
- export-profile.cjs: lines 245-249 re-hash the `project` settings subject (`PROFILE_SUBJECT_CHANGED`). That directory only runs `--script` (line 229) and is never `--import`ed. The imported directory is the separate `importer` fixture (lines 210-216), which nothing hashes.
- audio.cjs:1569-1570 removes `src/godot-game/project.godot` from a source list on purpose. No other case reads `project.godot` from a run directory, and no strict schema check of `.command.json` keys was found.
- Other files hash `tools/phase4/check.cjs` only as before/after runtime snapshots (provenance.cjs:6-8, data-camp.cjs:79, export.cjs:12, export-layout.cjs:8). Editing `check.cjs` breaks none of them. Its Phase 1 catalog row is `"kind": "outside-validator"` with `"sha256": null` (tools/phase1/required-cases.json:5170-5173).

### Q4. Duplicate `[editor]` headers in Godot 4.7.2
[VERIFIED: godot@ed1daf0b core/config/project_settings.cpp:965-993] `_load_settings_text` keeps a single `section` string. A tag sets only `section = next_tag.name;`, and each assignment calls `set(section + "/" + assign, value)` (or `set(assign, value)` when no section is open). Repeated headers therefore merge, and a later assignment to the same key overwrites an earlier one. Appending `[editor]` at the end of the file is safe. The setting is registered as `GLOBAL_DEF("editor/import/use_multiple_threads", true);` (editor/register_editor_types.cpp:306) and read with `GLOBAL_GET` in `EditorFileSystem` (editor_file_system.cpp:3333). The threaded branch also requires `use_multiple_threads && reimport_files[i].threaded` (line 3346).

So the detector must build the full key the same way Godot does. The following three forms all set the same value:
- `editor/import/use_multiple_threads=…` with no section open
- `import/use_multiple_threads=…` under `[editor]`
- `use_multiple_threads=…` under `[editor/import]`

Accept the project only when every occurrence is exactly `false`. Reject anything else with `EDITOR_IMPORT_THREADED`.

### Q5. Test conventions and commands
- Phase 4 tests live inside modules as `node:test` self-test modes, for example `export-profile.cjs` `helperSelfTest()` (lines 455-464), run with `--self-test-helper` (line 472). Its pre-spawn rejection test is the template to follow. It creates a dir with `fs.mkdtempSync(path.join(ROOT,'.phase4/tdd/04-35'),'helper-red-')`, calls `runner.command({directory},…)`, uses `assert.rejects(…,/COMMAND_TIMEOUT_RANGE/)`, and checks that `<name>.command.json` does not exist. Recorded commands for these modes look like `node --test-reporter=tap tools/phase4/cases/hub.cjs --self-test` (.planning/phases/04-asset-production/tdd/*-red.json). Phase 3 uses standalone `tools/phase3/*.test.cjs`, run with `node --test tools/phase3/revisions.test.cjs`.
- There is no `package.json`. `docs/workflow/verification.md` lists only the `check.cjs` runner commands.
- Do not put the test under `tools/phase4/cases/`. `loadCaseModules` `require`s every file there (case-discovery.cjs:45-48).
- `check.cjs` runs `main()` when `require.main===module` (line 541), and `main` rejects unknown args with `USAGE` (line 527). A self-test flag has to be handled before `main()` is called.
- Stand-in for Godot: `process.execPath` with `['-e',script,'--','--headless','--path',project,'--editor','--import','--quit']`. The `--` is required. Without it, Node exits with `bad option: --headless` (probed with Node v24.22.3). Requiring `check.cjs` has no side effects (cases load lazily in `main`). `command()` does not check the Node pin. Only `engine()` checks it (line 211, pin `24.22.1`). The only Node on this machine is v24.22.3, which is fine for this test. `node --check tools/phase4/check.cjs` passes.

### Q6. Pitfalls checked
- **Path case and separators:** `path.resolve` turns `/` into `\` (probe: forward-slash input → `true`). A lowercase `d:/work/...` path does not match `base+path.sep`, so it is rejected, which fails closed. A sibling path such as `runs/xy` does not match `runs/x\`, and a `..` escape is rejected. Resolve `--path` against the **child cwd** (`path.resolve(resolved,args[i+1])`), not the Node process cwd, because Godot resolves it relative to its own cwd. All current callers pass absolute paths.
- **CRLF, BOM and trailing newline:** Split lines on `/\r?\n/`. Strip a leading `\ufeff` only for parsing, and append raw bytes without rewriting the existing ones. Use `\r\n` for the appended block if the file contains `\r\n`. Add a newline first if the last byte is not `0x0a`, because `config/name="x"[editor]` would not parse. All current fixtures use LF and end with a newline.
- **Multi-line values:** Only match lines whose computed key equals `editor/import/use_multiple_threads`, so continuation lines of a multi-line value are not misread as keys.
- **Editor rewrite:** The Godot editor may re-save `project.godot` during import, and keeps non-default values. A second import of the same directory then finds `false` and leaves the file byte-identical. [ASSUMED]
- **Linux:** `engine()` refuses to run unless `process.platform==='win32'&&process.arch==='x64'` (line 210), and every case resolves `archives['win32-x64']`. There are no Linux Phase 4 imports today. The policy is platform-independent and harmless on Linux. Single-threaded import only removes worker-thread parallelism. [ASSUMED for Linux runtime cost]
- **Timeout budget:** Existing records in `.phase4/runs` show every import passing in under 5 s. Medians range from 3.1 s to 4.5 s, the maximum is 4833 ms (`native-import`, n=201), and there are 0 `CHILD_TIMEOUT` across 1,134 imports. The cap is 60 s. Going single-threaded is very unlikely to approach the cap. Re-measure in verification. [VERIFIED: local run records; serial cost ASSUMED]

## Common Pitfalls

### Pitfall 1: A new tracked file breaks the Phase 1 repository-union proof
**What goes wrong:** `tests/phase1/ExemptionTests.cs:1042-1047` requires the sorted list of checked tracked files to equal `repositoryStages.sources` of case 26 (`ac-08.complete`) exactly (`REPOSITORY_EXACT_TRACKED_UNION`). Unknown file types count as checked (EnforcementTests.cs:627: `SourceFileFormat.IsChecked(path: "assets/new.unknown").ShouldBeTrue();`). Today the union is exact: 471 sorted rows, no undeclared `tools/` files, and `sourceSetSha256` matches (probe script).
**Cost if the separate test file is kept:** Insert row `{"path":"tools/phase4/editor-import.test.cjs","kind":"outside-validator","owners":[],"sha256":null}` at sorted index 465, between `content.cjs` and `expected-obligations.json`. Then recompute 7 derived values:
- `/cases/26/repositoryStages/sourceSetSha256`. This is `digest(JSON.stringify(spec.sources))` (tools/phase1/check.cjs:175).
- `callerStages.repositorySha256` in Phase 1 cases 176, 348, 585, 586 and 589. These are `digest(JSON.stringify(repositoryPlan))` (tools/phase1/check.cjs:276-281,332).
- Phase 2 `core.permissions` `repositorySha256` (tools/phase2/required-cases.json:43).

The 04-23 repair notes treat this exact eight-cell set as a controlled catalog change.
**Recommendation:** Add the test to `check.cjs` as `--self-test-editor-import`, following the `helperSelfTest` precedent. That avoids the catalog changes. If the planner keeps the separate file, it must include the catalog task.

### Pitfall 2: Rejection after side effects
Placing the policy after line 168 creates the `home`, `temp` and other directories. Placing it after the spawn writes `.exit.json` and `.command.json`. Put it before line 168, as described in Q1.

### Pitfall 3: Key matched by section only
Checking only for an `[editor]` header misses the root-key and `[editor/import]` forms (Q4). A `true` value would then slip through, or a duplicate block would be appended.

## Code Examples

Policy sketch. Use `need`, `inside` and `bytes` from check.cjs. The names are illustrative, and the error codes are the locked ones.
```js
const THREADS='editor/import/use_multiple_threads';
function editorImport(base,cwd,name,args){
  if(!args.includes('--import'))return;
  const at=args.indexOf('--path');
  need(at>=0&&args.indexOf('--path',at+1)<0&&typeof args[at+1]==='string'&&args[at+1].length>0,'EDITOR_IMPORT_PROJECT',name);
  const project=path.resolve(cwd,args[at+1]);
  need(project.startsWith(base+path.sep),'EDITOR_IMPORT_PROJECT',name);
  try{inside(base,path.relative(base,project).replaceAll(path.sep,'/'));inside(project,'project.godot');}
  catch(error){need(false,'EDITOR_IMPORT_PROJECT',name+' '+error.message);}
  const file=path.join(project,'project.godot');
  need(fs.existsSync(file)&&fs.lstatSync(file).isFile(),'EDITOR_IMPORT_PROJECT',name);
  const data=bytes(project,'project.godot'),text=new TextDecoder('utf-8',{fatal:true}).decode(data).replace(/^\ufeff/,'');
  let section='';const values=[];
  for(const raw of text.split(/\r?\n/)){const line=raw.trim();if(!line||line.startsWith(';'))continue;
    const header=/^\[([^\]]+)\]$/.exec(line);if(header){section=header[1].trim();continue;}
    const assign=/^([^=]+?)\s*=\s*(.*)$/.exec(line);if(!assign)continue;
    if((section?section+'/'+assign[1]:assign[1])===THREADS)values.push(assign[2].trim());}
  if(values.length){need(values.every(v=>v==='false'),'EDITOR_IMPORT_THREADED',name);return;}
  const eol=text.includes('\r\n')?'\r\n':'\n';
  fs.appendFileSync(file,(data.at(-1)===0x0a?'':eol)+eol+'[editor]'+eol+eol+'import/use_multiple_threads=false'+eol);
}
// in command(), after line 167:  editorImport(base,resolved,name,args);
```

## Validation Architecture

| Property | Value |
|---|---|
| Framework | `node:test` + `node:assert/strict` (built into Node 24) |
| Quick run | `node --test-reporter=tap tools/phase4/check.cjs --self-test-editor-import`. With a separate file: `node --test tools/phase4/editor-import.test.cjs` |
| Scratch dir | `fs.mkdtempSync(path.join(ROOT,'.phase4/tdd/'))`. `.phase4/` is git-ignored (.gitignore:9 `/.phase4/`) |

Required tests. All use the Node stand-in, which prints the bytes of `project.godot` it sees at spawn time:
1. An inline project with no `[editor]` section: the stand-in sees `import/use_multiple_threads=false` and the original bytes are an unchanged prefix. This is RED before the fix.
2. A project that already has `[editor]` and `import/use_multiple_threads=false`: the file is byte-identical after the call (same hash).
3. A project with `true` under `[editor]`, and one with the root-key form: rejects with `EDITOR_IMPORT_THREADED`, leaves no `<name>.command.json`, `.exit.json` or stand-in marker, and does not change the file.
4. Missing `--path`, `--path` equal to `ROOT`, a sibling-prefix directory, and a directory without `project.godot`: each rejects with `EDITOR_IMPORT_PROJECT` and writes no record.
5. A `--script` command on a project without `[editor]`: the file is unchanged.
6. A CRLF file and a file without a trailing newline: the appended block is well-formed (CRLF suffix; a newline is added first).
7. A second import of the same directory: the file is unchanged (idempotent).

Phase gate: run the self-test, then `node --check tools/phase4/check.cjs`. Run one real import case, for example `node tools/phase4/check.cjs --case animation.replacement` or `source.tracer`. These are Windows-only, need the engine, and belong to the executor or verifier, not to research.

## Security Domain
The relevant ASVS controls are V5 (input validation) and V12 (files).
- The policy writes only to `project.godot` inside a run-owned descendant of `context.directory`, after the `inside()` symlink, case and escape checks. It never writes to `ROOT` or `src/`.
- It reads with the 1 MiB `bytes()` limit and strict UTF-8.
- It does not change `shell:false` or the env isolation.
- There are no new dependencies and no network access.

## Package Legitimacy Audit
No external packages. Only Node built-ins are used.

## Assumptions Log
| # | Claim | Risk if wrong |
|---|---|---|
| A1 | The Godot editor keeps the non-default `editor/import/use_multiple_threads=false` when it re-saves project.godot during import | A second import could re-append a block. That is harmless because of the merge semantics, but it would break byte-identity for reused dirs. No current site re-imports the same dir. |
| A2 | Single-threaded import stays far below the 60 s cap | `CHILD_TIMEOUT`. Mitigation: measure duration in the verification run. |
| A3 | Exported `project.binary` (export.cjs) containing the editor setting has no runtime effect | Low. The setting is only read in editor builds (`#ifdef THREADS_ENABLED` path in EditorFileSystem). |

## Open Questions
1. Test placement: `check.cjs --self-test-editor-import` (recommended, no catalog churn), or a separate `editor-import.test.cjs` plus the 8-cell catalog update. The planner decides. The CONTEXT puts this under Claude's discretion.
2. The PR base is locked to `feature/04-asset-production`. `docs/workflow/ai-collaboration.md` normally targets `dev`. CONTEXT overrides that for this stacked fix. The current branch `fix/04-import-cwd-race` has merge-base `07f1b5c`, which is the head of `origin/feature/04-asset-production`.

## Sources
- Read this session: tools/phase4/check.cjs (full), tools/phase4/export.cjs (1-110, 150-295), tools/phase4/cases/export-profile.cjs (195-472), tools/phase1/check.cjs (160-229, 276-290), tests/phase1/ExemptionTests.cs (1030-1085), src/godot-game/project.godot, .gitattributes, .planning/config.json
- Godot source `ed1daf0bf001b61586d9930840f2f1394092c079` (scratchpad clone, read-only): core/config/project_settings.cpp:943-995, editor/file_system/editor_file_system.cpp:3325-3350, editor/register_editor_types.cpp:306
- Read-only probes run with Node v24.22.3: site enumeration, path-case behaviour, argv `--` handling, Phase 1 union/sourceSetSha256, import durations from `.phase4/runs`

**Valid until:** while `tools/phase4/check.cjs` keeps the `command()` structure at lines 160-192.
