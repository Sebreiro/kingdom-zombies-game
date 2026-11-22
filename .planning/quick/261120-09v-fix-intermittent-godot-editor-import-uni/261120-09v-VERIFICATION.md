---
phase: quick-261120-09v-fix-intermittent-godot-editor-import-uni
verified: 2026-11-20T00:30:00Z
status: passed
score: 7/7 must-haves verified
covered_files:
  - ".planning/debug/phase04-import-surrogates.md"
  - ".planning/phases/04-asset-production/04-23-CHECKPOINT.md"
  - ".planning/quick/261120-09v-fix-intermittent-godot-editor-import-uni/261120-09v-PLAN.md"
  - ".planning/quick/261120-09v-fix-intermittent-godot-editor-import-uni/261120-09v-RED.json"
  - ".planning/quick/261120-09v-fix-intermittent-godot-editor-import-uni/261120-09v-SUMMARY.md"
  - "tools/phase4/check.cjs"
covered_digest: "v4:sha256:2b8d7e72b815570c98be494127383ddbbdbd51b6ef285f648c4d1872a9ca717a"
behavior_unverified: 0
overrides_applied: 0
---

# Quick Task 261120-09v: Fix intermittent Godot editor-import Unicode parsing errors (cwd race) - Verification Report

**Goal:** Remove the intermittent Godot 4.7.2 editor-import `Unicode parsing error` lines. The cause is a Windows working-directory race during threaded editor import. The fix is a runner policy in `tools/phase4/check.cjs` `command()` that forces single-threaded editor import for every run-owned `--import` project.
**Verified:** 2026-11-20
**Status:** passed
**Re-verification:** No (initial verification)

## Goal Achievement

### Observable Truths

| # | Truth | Status | Evidence |
|---|-------|--------|----------|
| 1 | Every runner command whose args contain `--import` starts its child only after the run-owned project.godot sets `editor/import/use_multiple_threads=false`, so harness editor imports run on one thread and the cwd race cannot occur | VERIFIED | **Code.** `editorImportPolicy(base,resolved,name,args);` is at check.cjs:201. It runs after the `COMMAND_ENV_INVALID` check and before `const childEnv=` (line 202) and `spawn`. **Call sites.** All `--import` calls in the 16 caller modules (15 case files plus export.cjs) go through `runner.command`, and none spawns Godot directly. **Self-test.** T1 (absolute and relative `--path`) shows the child reads base+block. **Real runs.** Four runs at revision 4728211 (tools/ is identical at HEAD): animation.replacement, audio.heldout, world.inspector and text.support-native all have `status: pass`. Their six `*import*.stderr.log` files are 0 bytes, no Unicode error appears, and every imported project.godot ends with the block (one occurrence). **Mechanism.** The single-thread mechanism comes from the pre-planning Frida trace (CONTEXT). This verification did not re-derive it. |
| 2 | A run-owned project.godot without the setting gets exactly one appended `[editor]` block (CRLF if CRLF, leading EOL if no final newline); the original bytes stay a prefix | VERIFIED | The code is at check.cjs:189-192. `appendFileSync` adds `(data.at(-1)===0x0a?'':eol)+eol+block`. Self-test T1, T3 (idempotent second import) and T7 (CRLF, no-final-newline) pass, and each compares the child's view and the disk bytes. |
| 3 | A project that already sets the key to false stays byte-identical (plan wording); approved post-review contract: only a file that already ends with the canonical block is unchanged, every other file gets the block appended last, and Godot's effective value is false | VERIFIED (deliberate, documented deviation) | `if(text===block\|\|text.endsWith(eol+block))return;` (line 191). Self-test T2 covers several cases. `editor` and `editor-crlf` stay byte-identical. `root`, `section`, `comment` and `override` false spellings get the block appended. A probe of mine also left a BOM+canonical file unchanged. The change is recorded in the REVIEW.md Disposition (WR-01), the SUMMARY Tasks table, the debug-record Resolution, and the policy comment at line 166. It fixes a real defect: a misread `false` could skip the append while Godot still read `true`. See Deliberate Deviations. |
| 4 | Any other explicit value is rejected with `EDITOR_IMPORT_THREADED` before environment directories, records or a process exist; the file is not rewritten | VERIFIED | `need(values.every(value=>value==='false'),'EDITOR_IMPORT_THREADED',name)` runs before any write (line 188). Feature-tag keys (`.windows`, `.editor`) are matched (line 187). Self-test T5 passes for 9 scenarios: editor/root/section true, mixed, quoted key, spaced key, header with comment, override-windows and override-editor. For each, the file stays byte-identical and `absent()` finds no `.command.json`, no `.exit.json` and no `home`. My probe also rejected `False` and `"false"`. |
| 5 | An `--import` command whose `--path` is missing, valueless, outside `context.directory`, or names a directory without project.godot is rejected with `EDITOR_IMPORT_PROJECT` before side effects | VERIFIED | Lines 170-179. Self-test T6 passes for: no-path, path-last, outside, sibling-prefix, no-project, plus the hardening cases (project.binary, hard link, --main-pack, positional project file). Each leaves no records and no home directory. My probe also rejected `--path` equal to the run directory itself. |
| 6 | Commands without `--import` (`--script`, `--export-debug`, `--version`) leave project.godot untouched | VERIFIED | Line 169 is `if(!args.includes('--import'))return;`. Self-test T4 passes for script, export, script-true and version. The only real `--export-debug` call (export.cjs:270) runs on the same project right after its `--import`, so it also reads the single-thread setting. No call site uses `--editor` without `--import`. |
| 7 | The debug record is resolved with root cause, evidence, fix and verification, and the 04-23 checkpoint points at it without claiming 04-23 acceptance | VERIFIED | The debug record has `status: resolved` and `updated: 2026-11-20`. Its top `## Resolution` covers root cause, evidence, fix, verification and follow-ups. History from `## Symptoms` onward is byte-identical to 07f1b5c (diff empty). The checkpoint's original bytes are an unchanged prefix. The new note links `../../debug/phase04-import-surrogates.md` and says it "claims no 04-23 Task 1 GREEN, clean-output qualification, plan completion or acceptance". |

**Score:** 7/7 truths verified (0 present, behavior-unverified)

### Deliberate Deviations (not gaps)

The plan must_have #3 and the CONTEXT "Policy strictness" item say an existing explicit `false` stays byte-identical. After code review (WR-01, WR-02), the orchestrator narrowed this. Only a file that already ends with the canonical block is left unchanged. Any other file gets the block appended last, because Godot keeps the last value it reads, so the appended block wins over any earlier spelling Godot might read differently from the JS parser. The intent is still met: Godot's effective value is false for every harness import, and threaded values are still rejected without rewriting. Commits 8cd2f36 (RED, 3 of 7 fail, reproduced here) and 4728211 (GREEN, 7/7) carry this change. No override entry was added, because the deviation meets the plan's intent and is documented in REVIEW.md, SUMMARY.md and the debug record. A human reviewer should still know that the CONTEXT decision text no longer describes the runtime behaviour exactly.

### Required Artifacts

| Artifact | Expected | Status | Details |
|----------|----------|--------|---------|
| `tools/phase4/check.cjs` | editorImportPolicy guard in command() plus `--self-test-editor-import` mode; contains `EDITOR_IMPORT_THREADED` | VERIFIED | The policy is at lines 160-192 and the self-test at lines 574-628. `node --check` passes. |
| `.planning/quick/.../261120-09v-RED.json` | Failing run before the policy (5 fail / 2 pass); contains `failingTests` | VERIFIED | exitCode 1, summary {7,2,5}, 5 failing and 2 passing names, TAP output contains `not ok`, semanticAssessment present. |
| `.planning/debug/phase04-import-surrogates.md` | Resolved, top Resolution; `status: resolved` | VERIFIED | As described in truth 7. |
| `.planning/phases/04-asset-production/04-23-CHECKPOINT.md` | Dated note pointing at the debug record | VERIFIED | As described in truth 7. |

### Key Link Verification

| From | To | Via | Status | Details |
|------|----|-----|--------|---------|
| check.cjs `command()` | `editorImportPolicy(base,resolved,name,args)` | Synchronous call before `const childEnv=` | WIRED | Line 201 is the call and line 202 is `childEnv`. |
| `require.main===module` dispatch | `editorImportSelfTest()` | Exact argv `['--self-test-editor-import']` before `main()` | WIRED | Line 630. Any other argv goes to `main()`, and `--list` exits 0. |
| 04-23-CHECKPOINT.md | debug/phase04-import-surrogates.md | Relative link `../../debug/phase04-import-surrogates.md` | WIRED | The target exists. |

### Data-Flow Trace (Level 4)

| Artifact | Data Variable | Source | Produces Real Data | Status |
|----------|---------------|--------|--------------------|--------|
| editorImportPolicy | `text` / `values` | `bytes(project,'project.godot')` with a strict UTF-8 decode | Yes. The real-run project.godot files contain the appended block. | FLOWING |

### Behavioral Spot-Checks

| Behavior | Command | Result | Status |
|----------|---------|--------|--------|
| Syntax | `node --check tools/phase4/check.cjs` | exit 0 | PASS |
| GREEN self-test at HEAD | `node --test-reporter=tap tools/phase4/check.cjs --self-test-editor-import` | 7 tests, 7 pass, 0 fail. Scratch `.phase4/tdd/editor-import` is empty afterwards. | PASS |
| Original RED reproduced | check.cjs at 6279cf9 run from the scratchpad | 7 tests, 2 pass (T2, T4), 5 fail (T1, T3, T5, T6, T7), all `ERR_ASSERTION` | PASS (matches RED.json) |
| Review RED reproduced | check.cjs at 8cd2f36 run from the scratchpad | 7 tests, 4 pass, 3 fail (tests 2, 5, 6) | PASS (matches the Disposition) |
| Existing self-tests | `export-profile.cjs --self-test-helper` and `--self-test` | 1/1 each | PASS |
| Main dispatch intact | `check.cjs --list` | exit 0 | PASS |
| Edge probe (scratchpad, exported `command()`) | BOM+canonical, `False`, `"false"`, `--path`=run dir, override-false then canonical | Unchanged / THREADED / THREADED / PROJECT / unchanged | PASS |
| Real Godot evidence (read-only) | Read result.json, invocation.json and `*import*` records for the 4 runs | All pass at revision 4728211, six imports with 0-byte stderr, block present | PASS |

### Probe Execution

Not applicable. No `scripts/*/tests/probe-*.sh` is declared or relevant. Godot cases were not run, as instructed. The existing real-run records were read instead.

### Requirements Coverage

| Requirement | Source Plan | Description | Status | Evidence |
|-------------|-------------|-------------|--------|----------|
| 261120-09v | PLAN | Quick-task goal (not a ROADMAP requirement; 04-23 acceptance stays pending) | SATISFIED | Truths 1-7 |

### Scope Checks

- The only tracked file changed outside `.planning/` between 07f1b5c and HEAD is `tools/phase4/check.cjs`. No new tracked file was added outside `.planning/`.
- `src/godot-game/project.godot` is unchanged. ROADMAP.md, REQUIREMENTS.md and STATE.md are unchanged.
- `tools/phase1/required-cases.json` lists check.cjs as `outside-validator` with `sha256: null`, so no pinned-hash proof breaks.
- No fixture in any `--import` call site contains `[editor]`, `use_multiple_threads`, a `project.binary` or a hard-linked project.godot. Every project path is a subdirectory of `context.directory`, so the new rejections do not affect current callers.

### Anti-Patterns Found

| File | Line | Pattern | Severity | Impact |
|------|------|---------|----------|--------|
| (added lines in all modified files) | - | TBD/FIXME/XXX/TODO/HACK/placeholder | - | None found |
| tools/phase4/check.cjs | 183-187 | The line-based parser does not model multi-line values. A string value that spans lines could hide a `[section]` or an override key from the JS scan. | Info | Contrived and not present in any fixture. A hidden base key is still overridden by the block appended last. Only a hidden feature-tag override would escape. |
| tools/phase4/check.cjs | 191 | `eol` comes from the whole text, so a mixed-EOL file that already ends with an LF block gets a redundant CRLF block | Info | Harmless. The redundant block is also `false`. |
| .phase4/tdd/ | - | Leftover ignored scratch files (`q09v-*`, `*.tap`) and an empty `editor-import` parent | Info | git-ignored. IN-06 records it as a known partial. |

### Human Verification Required

None for this quick task's goal. The 04-23 whole-selection clean-output run under the new runner is still owed. The debug record, the checkpoint note and the SUMMARY all record it as an open follow-up. It is outside this task's goal and is not claimed here.

### Gaps Summary

No gaps. The policy is at the single runner choke point that every `--import` call uses. It runs before any side effect, fails closed on threaded values and unowned projects, and always leaves the canonical single-thread block last in the project.godot the child reads. RED-before-GREEN is reproducible from history. Real pinned-Godot runs of the four cases that originally failed show clean import stderr and the appended setting. The narrowed "byte-identical" rule is a deliberate, documented post-review change that strengthens the guarantee.

---

_Verified: 2026-11-20_
_Verifier: Claude (gsd-verifier)_
