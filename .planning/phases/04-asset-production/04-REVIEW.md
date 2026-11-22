---
phase: 04-asset-production
reviewed: 2026-11-22T15:28:02.000Z
reviewer: Claude Phase 4 Independent Reviewer
revision: 5c637da23437ea09501a9b167449425a824fe1c1
source: 29578ddb9b6fe12f4a37188cb008faf5dda68988ebb32298534f9f9c317b7ae9
media: 056324ea9e8255b2e1afe7b7596ac21906ef25364424d4057c2e70547e0e59fd
tool: 081ee3aa048f6f5e8e6b0bea902fe1911d9255a9c3f1721fc3444fb17fdcae6e
depth: standard
files_reviewed: 7
files_reviewed_list:
  - tools/phase1/required-cases.json
  - tools/phase2/required-cases.json
  - tools/phase1/check.cjs
  - tests/phase1/ExemptionTests.cs
  - tests/phase1/SourceFileFormat.cs
  - tools/phase4/scenarios.cjs
  - tools/phase4/evidence.cjs
findings:
  critical: 0
  warning: 0
  info: 0
  total: 0
status: clean
---

# Phase 4 independent code, content and evidence review (third renewal)

## Summary

The Claude Phase 4 Independent Reviewer reviewed `5c637da23437ea09501a9b167449425a824fe1c1`, which is HEAD. I did not implement the change. Read-only `identifySource(root, 5c637da)` and `identifyTool(root)` reproduce exactly the source `29578ddb…`, media `056324ea…` and tool `081ee3aa…` above.

The range `8889f5a..5c637da` is one commit. It changes only `tools/phase1/required-cases.json` and `tools/phase2/required-cases.json`. No code, test, product source, media or Phase 4 tool file changed, and no tracked file was added or removed by this commit.

I ran no `check.cjs`, dotnet or Godot, and I did not treat the running batch as evidence.

### Disposition of the Phase 1 source-set repair: correct and complete

**The failure.** `RepositoryShape` (`tests/phase1/ExemptionTests.cs:1037-1057`) requires the ordinally sorted declared `repositoryStages.sources[].path` to equal exactly the tracked paths that `SourceFileFormat.IsChecked` keeps. That predicate drops `.claude/`, `.codex/`, `.agents/` and `.planning/`, plus the listed binary extensions. I replicated it read-only over `git ls-tree` at both revisions:

| Revision | Tracked | Declared | Result |
| --- | --- | --- | --- |
| `fee2a0e` | 480 | 480 | Equal |
| `5c637da` | 482 | 482 | Equal, with no missing or extra path |

The working-tree index also yields 482.

The only tracked files added since `fee2a0e` outside `.planning` are `tools/phase4/python.cjs` and `tools/phase4/python.json`, both from the IN-01 repair. They are exactly the two rows inserted.

**The classification.** Both rows are `outside-validator` with `owners: []` and `sha256: null`. That matches all 46 other `tools/phase4/*` rows and every Phase 1–3 tool script. Neither file is compiled or consumed by the five C# projects, so neither is a `source` or `configuration` row. `repositoryPlan` hashes and assigns owners only to applicable rows.

**The source-set digest.** The rows sit in ordinal position. Every other `repositoryStages` field is unchanged from `8889f5a`. `sourceSetSha256` recomputes to `8038db86…` as `sha256(JSON.stringify(sources))`, which is exactly what `repositoryPlan` checks. `tools/phase1/check.cjs` `repositoryPlan` accepts the plan.

**The repository rebind.** `callerStages.repositorySha256` is checked by `callerPlan` (`tools/phase1/check.cjs:332`) against `sha256(JSON.stringify(callerRepository(root)))`, and that function returns the `ac-08.complete` `repositoryStages`. The value recomputes to `69a44bbd…`; at `8889f5a` it was `7496b779…`. All six caller declarations carry the new value:

- R-50.positive;
- rules.godot-callsite-boundary;
- rules.godot-companion-positive;
- rules.godot-companion-negative;
- rules.exemptions;
- Phase 2 core.permissions.

`ExemptionTests.cs:632` reads the same catalog field.

**Stale digests.** A `git grep` of the whole tree at `5c637da` finds no remaining occurrence of the old `427afda9…` or `7496b779…`. The Phase 3 catalog has no caller stages. No Phase 4 data or the pack index pins either catalog's bytes; `export-layout.cjs` and `census.cjs` read them live.

**Targeted reruns.** The three reruns `d5edb66e`, `e4674233` and `341550ac` passed. Their input fingerprints equal the committed snapshot of `5c637da`, so they tested exactly the committed fix. They recorded HEAD `8889f5a` because they ran before the commit. Phase 4 would therefore correctly refuse them as receipts (`COMPLETION_INHERITED_INPUT`). The final selection needs the fresh `5c637da` Phase 1 receipts, which shows the WR-05 binding working as intended. The failing full run `f32bd493` is correctly bound to `8889f5a`.

**Should the IN-01 repair have caught this?** Yes, in principle. Adding a tracked, checked file requires a matching Phase 1 source-set row, and the repair could have run the three repository-shape cases before committing. The omission was nevertheless caught by the required full Windows Phase 1 run before any binding. Plan 24 already requires that run, and the gate and impact rules made it necessary. This is a process miss that was contained, not a tooling defect, so I record no finding.

### Earlier findings still resolved at this revision

No code changed in this range, so each repair below is byte-identical to the one I accepted at `8889f5a`.

| ID | Confirmation |
| --- | --- |
| CR-01 | The Phase 2 `validateResources` role map is unchanged. |
| CR-02 | The H27/H28 derived PCKs and the two positive lifetimes are unchanged. |
| CR-03 | The staged rendered contracts and the offline verifier are unchanged. |
| CR-04 | The build, prepared-source, runtime and portable-closure authentication is unchanged. |
| CR-05 | The `prelaunch` review, fix-proof and receipt gate is unchanged. |
| CR-06 | `completionSourceReceipts` still excludes `acceptance.artifacts`. |
| CR-07 | The audition and listening bindings are unchanged. |
| CR-08 | The Inspection C# is unchanged. |
| CR-09 | The publication claim protocol is unchanged. |
| CR-10 | `impactClosure` is unchanged. On the `5c637da` diff it marks 678/678 Phase 1, 16/16 Phase 2 and 32/32 Phase 3 rows affected, and `loadCatalog` gives 75 cases and 18 active rendered cases. |
| WR-01 | The campaign start and terminal receipts, signals and budgets are complete through WR-03. |
| WR-02 | `CharacterPreview` is unchanged. |
| WR-03 | The campaign scope and ancestry rule are unchanged. |
| WR-04 | `rendered-process.cjs` identifies processes by PID plus creation time and kills exactly; it is unchanged. |
| WR-05 | The committed-input binding is unchanged and verified again on retained receipts. |
| WR-06 | No inherited tool comparison exists, and `inheritedBinding` has only `input`. |
| IN-01 | The `python.cjs`/`python.json` pin, the `command()` verification and `identifyTool` are unchanged. The pin files are now also declared in the Phase 1 source set. |
| IN-02 | `recordInterruption` and `attemptRows` are unchanged and resolve rows before any side effect. |

The interim RR-01 to RR-05 repairs are untouched.

### Observations carried forward (no finding)

- The real `attemptRows` derivation is covered only by code tracing; the controls always pass fixture rows.
- Phase 2/3 runners record the Node version but do not enforce the pin themselves.
- Phase 4 Python scripts run without `-I`, so `PYTHONPATH`/`PYTHONHOME` would pass through. Neither variable is set on this workstation.
- The `pwsh` used for `content.native-permissions` discovery is unpinned. Its output is cross-checked by executing all 251 discovered tests.
- The source changed after `fee2a0e`, so Plan 24 needs a fresh portable export, a `review-build.json` and the complete selected pre-window receipts at this revision before binding.
