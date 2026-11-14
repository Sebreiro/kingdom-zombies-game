---
phase: 01-code-rules-and-documentation
fixed_at: 2026-11-14T18:06:07Z
review_path: .planning/phases/01-code-rules-and-documentation/01-REVIEW.md
iteration: 1
fix_scope: critical_warning
findings_in_scope: 4
fixed: 4
skipped: 0
status: all_fixed
---

# Phase 1: Code Review Fix Report

**Fixed at:** 2026-11-14T18:06:07Z
**Source review:** `.planning/phases/01-code-rules-and-documentation/01-REVIEW.md`
**Iteration:** 1, single initial repair pass

**Summary:**

- Findings in scope: 4
- Fixed: 4, each committed separately with normal hooks
- Skipped: 0
- New required cases: 6; 684 total declarations, 676 Phase 1 obligations, unchanged eight pending Phase 5 identities

`all_fixed` records applied source repairs and focused verification. These findings concern enforcement/proof logic, so each is marked **fixed: requires human verification** under the fixer contract. Independent review, final platform evidence and the existing human judgments remain separate. No review disposition, security disposition, requirement or phase-completion flag was advanced by this fixer.

## Fixed Issues

### CR-01: Active consumer preprocessor branches bypass engine and suppression policies

**Status:** fixed: requires human verification
**Files modified:** `tests/phase1/ConsumerCompilation.cs` (new), `tests/phase1/ConsumerPolicyTests.cs` (new), `tests/phase1/EngineAccessPolicy.cs`, `tests/phase1/ExemptionValidator.cs`, `tests/phase1/EngineBoundaryTests.cs`, `tests/phase1/RandomContractTests.cs`, `tests/phase1/BoundaryFixture.cs`, `tools/phase1/required-cases.json`
**Commit:** `330a427`
**Applied fix:** Evaluate the selected Debug consumer's complete `DefineConstants`, `LangVersion`, actual `Compile` inventory and resolved `ReferencePath` inputs. A scan reuses its explicit evaluated context while parsing fresh source bytes; there is no global cache that can hide changed fixtures or imports. Engine handwritten, emitted companion and replacement trees use the same parse options and actual references, including required editor references. Engine subjects fail closed on compiler errors. Suppression scans select actual owners, reuse evaluation per consumer, and reject unresolved attribute identities. The random and service-locator semantic callers now use selected consumer options as well.

The existing guarded/spoofed engine snippets now include the real generated-companion prerequisites or use independent type/path identities. Compiler failures are not accepted as engine-policy rejections. Guard/path/type/method boundaries are preserved.

New controls cover actual evaluated `DEBUG`, `TOOLS` and the platform symbol, their inverse inactive branches, an unresolved semantic subject, and an active unregistered assembly suppression that silences the actual NamedArgumentAnalyzer. The inactive/no-suppression controls retain its real diagnostic.

**Focused evidence:** `engine.consumer-branches` passed invocation `2168e7f6-d380-4fb2-b075-d35b51f9d716`; `rules.suppression-conditional` passed `6becbd56-f2b6-462e-a774-f39528519da6`. Existing `engine.source-policy`, `R-67.positive`, `state.random-scope`, suppression-alias and suppression-duplicate controls passed. CR-01's original false-acceptance demonstrations remain in REVIEW.md; no historical initial RED was invented for these new controls.

### CR-02: Method groups and implicit native property references bypass the engine operation scan

**Status:** fixed: requires human verification
**Files modified:** `tests/phase1/EngineAccessPolicy.cs`, `tests/phase1/EngineOperationTests.cs` (new), `tools/phase1/required-cases.json`
**Commit:** `8ebbc1d`
**Applied fix:** Inspect resolved invocation, object-creation, method-reference, property-reference and field-reference operations, deduplicated by operation kind and syntax span. Native method references are rejected even inside a guarded adapter because capture-time checking cannot protect later invocation. Existing precise guarded direct-call checks remain authoritative; authored deferred wrappers perform their guard when invoked.

**Actual RED:** Build-valid `engine.method-escape` invocation `a99062f7-eb9d-4615-8065-0f871701d1f7` and `engine.inherited-members` invocation `b19ba2b6-c66c-4630-834a-7789e2bfa597` failed against the old scanner because no engine violation was reported. Both subject compilations had succeeded before the policy assertion.

**Actual GREEN:** The same required cases passed `2d227eee-230d-4f25-95a8-5cdee0fe58cc` and `5163a8c1-0b40-4bba-9f50-61716d43f0ec`. Controls cover captured and directly supplied `Task.Run` native method groups, an actual guarded Phase1 capture, a valid guarded authored wrapper, valid guarded direct calls, a bare inherited `Name` property and `NotificationReady` field. Existing engine guard-boundary proof passed `ac1d2bb9-d559-43e5-b293-f48112d5a427`.

### WR-01: The visibility proof checks four fixed files instead of the production subject inventory

**Status:** fixed: requires human verification
**Files modified:** `tests/phase1/ArchitecturePolicy.cs`, `tests/phase1/ArchitectureRuleTests.cs`, `tests/phase1/RuleDefinitions.cs`, `tools/phase1/required-cases.json`, `docs/architecture/boundaries.md`, `docs/code/register.json`, `docs/code/rule-matrix.md`
**Commit:** `3da7114`
**Applied fix:** R-34's actual positive proof now invokes `VisibilityRepository`, which evaluates all authored Compile inputs in Core, service ports and Godot. Every parsed source receives its selected consumer's explicit parse options; the single-source visibility helper no longer has a default-parser path. The exact three-type registry is unchanged. Tooling/test public surfaces and emitted generator outputs retain separate scopes. The current binding, its independent definition and the rule matrix point to the repository mechanism.

**Focused evidence:** `graph.visibility-inventory` passed `c7ce300f-e58d-4643-92ff-57207fef60b4`. It builds a copied actual game consumer, verifies the clean repository, changes the existing `ExampleState.cs` outside the former list to a public class under active `DEBUG`, builds it successfully, and requires the repository scan's exact R-34 rejection. The inverse inactive branch compiles semantically and is accepted. The existing synthetic negative remains and passed; the actual positive passed. Binding validation passed `083e2850-cdf0-4eb0-9982-3843d49bd85b`. Necessity/minimality judgment remains human Pending. REVIEW.md supplies the original false-acceptance evidence; no new pre-fix RED is claimed for a scanner that did not previously exist.

### WR-02: The implicit-usings negative proof succeeds for an unrelated missing type

**Status:** fixed: requires human verification
**Files modified:** `tests/phase1/EnforcementTests.cs`, `tests/phase1/RunnerContractTests.cs`, `tools/phase1/required-cases.json`
**Commit:** `5c52a20`
**Applied fix:** Replace the undefined `Missing` subject with an unqualified existing `Uri`. The existing R-16 negative retains its exact `Subject.cs(6,21)` CS0246 requirement. The positive uses explicitly qualified `System.Uri`. A new sensitivity case establishes the same actual Core consumer with a declared subject-only Compile inventory before its baseline, then changes only that consumer's `ImplicitUsings` property. Identical subject bytes fail with disable, compile with enable, and fail again after restoring disable. Analyzer bootstrap and all severity settings remain enabled and unchanged. Exact phase-membership assertions now account for the six added cases.

**Actual RED:** `d478fc2f-29db-470c-8da6-ab67c531254c` successfully prepared the isolated consumer but rejected the old subject even with implicit usings enabled, reporting only CS0246 for `Missing` at the intended declaration. An earlier command-line-property attempt (`fdadb209-fd10-4ff3-8a2c-7add07585324`) instead propagated implicit usings to analyzer sources and produced unrelated IDE0005 errors; that retained attempt is not behavior RED or acceptance evidence.

**Actual GREEN:** `rules.implicit-usings-control` passed `10dbbe3d-bd5e-4851-8479-2ef0f5d5a439`; R-16 positive passed `b71bb155-baf6-4bfe-ae49-b968fbb07b91`; negative passed `7ea9ba26-bf05-4405-bfd9-83ac42ca25b4`; exact phase membership passed `458785d4-bf97-444c-8a32-98f26880ff62`.

## Verification

All editing, commits and verification ran in the **main Windows checkout** at `D:/Work/Games/KingdomZombies`, as explicitly configured by `workflow.use_worktrees=false`. No worktree, temporary branch or recovery sentinel was created. Root identity was checked before every mutation/commit. The actual source sections were reread after edits; strict whole-solution builds passed before each atomic commit. Normal hooks were used.

The final strict build at `5c52a20c9fdabe9fad085e97333ca65d1ec8b219` exited zero with **0 warnings / 0 errors** using:

```text
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
```

The same command with MSBuild file logging retained `.phase1/temp/review-fix-strict-5c52a20.log`. Workspace-local CLI home and package paths were used. No full suite was run between fixes.

The following final focused invocations all used revision `5c52a20c9fdabe9fad085e97333ca65d1ec8b219` and input hash `a7d747999a07c9b8834ff813a47bcf11ebaa5f0aba30a2d9061229584d118d10`:

| Selector | Accepted | Invocation nonce |
| --- | --- | --- |
| `--group docs` | 36/36 | `ce2be889-4bc6-495a-97ee-32abec53dc0c` |
| `--case register.binding-precoverage` | 1/1 | `9d45bd96-371d-46bd-aa38-8d4ae695a731` |
| `--case R-67.negative` | 1/1 | `2c306e32-7866-4f04-bf32-361ee5914681` |
| `--case engine.gateway-bypass` | 1/1 | `dd817562-f582-494b-8247-1233ec97f66d` |
| `--case R-60.positive` | 1/1 | `f24e150a-7f55-47ee-a6aa-dcd55d73a581` |
| `--case R-60.negative` | 1/1 | `e222caa6-378a-4ba5-b520-928011f52ba8` |

Canonical evidence is retained under `.phase1/runs/<nonce>/summary.json` with actual test/process/subject records. Development build and incomplete-fixture failures remain retained; they were corrected without treating infrastructure or unrelated compiler errors as intended policy rejection. Proofs listed earlier are per-repair focused records, not a claim that earlier inputs certify later source revisions.

An exact object comparison against the starting `0276ea8` manifest confirmed **all 678 original declarations unchanged**, including IDs, test methods, commands, diagnostic expectations and deferred ownership. The six additional identities are:

- `engine.consumer-branches`
- `rules.suppression-conditional`
- `engine.method-escape`
- `engine.inherited-members`
- `graph.visibility-inventory`
- `rules.implicit-usings-control`

The eight unchanged pending Phase 5 identities are `ac-02.complete`, `ac-06.complete`, `ac-07.complete`, `ac-09.complete`, R-49 positive/negative and R-82 positive/negative. Twelve Error analyzers, inclusive body-60/line-120 limits, five locks, 170 engine span permissions and eighteen member suppressions were not broadened or weakened. Historical reports were not edited.

## Remaining acceptance work

The parent workflow owns independent review, disposition/security reconciliation, policy-document updates, and fresh complete Phase 1 verification on Windows and the local Linux VM under the creator's latest platform requirement. This fixer changed no host/VM settings. The old frozen Windows/Linux records are historical after these repairs and are not a final baseline for the new source. The existing fourteen human components and seven RULE requirements remain Pending; coverage and the eight declared certification cases remain Phase 5 work.

This report is intentionally **uncommitted** for the parent workflow's documentation commit. Source working state was clean after the four commits; this report is the sole fixer-owned uncommitted artifact.

---

_Fixed: 2026-11-14T18:06:07Z_
_Fixer: the agent (gsd-code-fixer)_
_Iteration: 1_
