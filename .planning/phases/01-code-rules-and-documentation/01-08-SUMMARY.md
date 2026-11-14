---
phase: "01-code-rules-and-documentation"
plan: "08"
subsystem: testing
tags: [csharp, roslyn, godot, xunit, enforcement, provenance]
requires:
  - phase: "01-07"
    provides: "Twelve inherited analyzers, exact spans and generated-origin boundaries"
provides:
  - "Strict inherited policy with real compiler, IDE, third-party, custom and banned-API consumer proofs"
  - "Exact tracked-source format, actual formatter selection and retained test naming enforcement"
  - "Bounded suppression, authentic generation, donor-disposition and numeric-edge checks"
  - "Checkout-independent pinned xUnit generation with producer integrity receipts"
affects: ["01-09", "01-14", "01-15", "01-16", "01-17"]
tech-stack:
  added: []
  patterns: ["Clean real consumer before intended negative", "Exact producer/path/hash/member/span permissions", "Immutable complete-suite reconciliation"]
key-files:
  created:
    - Directory.Xunit.targets
    - tests/phase1/BuildFixture.cs
    - tests/phase1/EnforcementTests.cs
    - tests/phase1/SourceFileFormat.cs
    - tests/phase1/TestSourceIndex.cs
    - tests/phase1/TestMethodNameTests.cs
    - tests/phase1/ExemptionValidator.cs
    - tests/phase1/ExemptionTests.cs
    - tests/phase1/ThresholdTests.cs
    - tests/phase1/GeneratedSurfaceScanner.cs
    - docs/code/provenance/xunit-entrypoint-path-adaptation.json
  modified:
    - .editorconfig
    - Directory.Build.targets
    - docs/code/register.json
    - docs/code/exemptions.json
    - tools/phase1/required-cases.json
    - tests/phase1/RunnerContractTests.cs
    - tests/phase1/AnalyzerSourceConsumerTests.cs
key-decisions:
  - "Require actual intended diagnostic/file/start coordinates after valid restore and clean compilation; unrelated failure never satisfies a negative."
  - "Keep exact generated producer permissions and reject source/configuration/effective-property suppression escapes."
  - "Use the same pinned authentic xUnit task with one stable logical project seed and verified late target override."
  - "Preserve stable case IDs, captured samples, all 85 source rules, 84 headings and 24 exemption IDs including gap 22."
requirements-completed: []
requirements-contributed: [RULE-02, RULE-03, RULE-06]
coverage:
  - id: D1
    description: "Real compiler/analyzer/banned-API and permission wiring"
    requirement: RULE-03
    verification:
      - kind: integration
        ref: "node tools/phase1/check.cjs --case rules.build-wiring"
        status: pass
    human_judgment: false
  - id: D2
    description: "Required source guard rejects a real compiler-accepted suppression bypass"
    requirement: RULE-03
    verification:
      - kind: integration
        ref: "node tools/phase1/check.cjs --case rules.exemptions"
        status: pass
    human_judgment: false
  - id: D3
    description: "Independent genuine Godot generation and handwritten companion boundary"
    requirement: RULE-06
    verification:
      - kind: integration
        ref: "node tools/phase1/check.cjs --case ac-08.complete"
        status: pass
    human_judgment: false
actuals:
  tokens: 262410
  tasks: 2
  commits: 34
commits: 34
plan_head_before: aa038a407893ddf2f822dbb34477d7b4cdb7d210
plan_head_after: cc964c2ca3154d064429ee74c92351855cb39d8c
duration: 289min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 08: Actual build and bounded-exemption proofs Summary

**Strict inherited policy, exact source/generated permissions and real consumer failures are verified by 477 fresh passing controls.**

Both owned tasks are complete. Phase 1 remains incomplete: the declared suite has 107 later-owner obligations, and all shared RULE requirement statuses remain Pending. This summary contributes to RULE-02/RULE-03/RULE-06 without claiming their cross-plan completion.

## Verification

The complete immutable-source `node tools/phase1/check.cjs --all` run finished with **477 passed controls, zero failed controls, 107 IMPLEMENTATION_PENDING obligations and 584 declared cases**. Actual exit was **1**, solely because the later obligations remain pending. No pending obligation is counted as a pass.

- Canonical report: `.phase1/runs/ad285936-7755-4d08-a242-f882d2495ea9/summary.json`.
- Console/actual exit: `.phase1/temp/08-final-all.log` / `08-final-all.exit`.
- Tested revision: `cc964c2ca3154d064429ee74c92351855cb39d8c` (source increment `114f2c9`).
- Input identity: `7cde7c9c72a422541f89379dd2df4d2d05988174fed650aaa53db6838c65a35f`.
- Tool identity: `90cd81fe40fe8cb37e1d195006e6a1be4fc86570962fd9f2d654bb861238237c`.
- Actual platform: `win32-x64-10.0.19044`.
- Full run: `2026-11-14T01:41:17.549Z` to `2026-11-14T02:05:49.240Z`; inputs stayed unchanged.

Before the full run, the strict five-project solution built with zero warnings/errors (`08-final-preflight-build.log`). Native bootstrap passed `d70acb20-9f33-40d4-8055-53ff70ea8b14` as raw compatibility evidence. The actual twelve-analyzer source consumer, tracked source format, all unique current test bindings, exact SCR0006 tracer and 36/36 register mutation checks passed. Git status was clean before/after the complete run; `git diff --check` passed. No extra full/preflight rerun was performed after success.

## Delivered behavior

Task 1 activates the inherited policy without weakening severity and exercises actual compiler/IDE/Meziantou/Roslynator/custom/banned-API/test-project builds. Negatives require their intended diagnostic, subject and coordinates after a successful locked restore and clean control. MA0137 and MA0156 suffix permissions use independent configuration drift; logging requires actual CA1848 `(10,9)` and CA1873 `(10,83)`. Source checks enforce strict UTF-8 without BOM, LF, final newline, trailing whitespace and tabs. Only five exact observed lock paths permit an absent final LF; locked restore preserves their LF bytes. Real formatter controls require nonzero file selection and exact WHITESPACE for 3/5 indentation. The retained Roslyn name scanner discovers all actual Fact/Theory variants and validates every unique implemented binding. Its 380 meaningful declaration/binding renames preserve stable case IDs and all analyzer/captured sample literals.

Task 2 checks source syntax/semantic suppression, exact member permissions, configuration weakening, duplicate/overlap/fake-generation claims and actual effective MSBuild properties. The SDK's observed NoWarn identifiers are exactly 1701 and 1702; extra/duplicate identifiers and WarningsNotAsErrors/analyzer disablement fail. Eighteen existing exact member suppressions retain source/package provenance, four immutable layout-source records and two exact Godot syntax spans remain bounded. No whole Godot/project/directory exemption was added.

All 24 donor exemption rows remain present exactly once, including missing ID 22. Ten active exact local records cover five locks plus superseded MA0051, engine/pure-library continuation policy, test naming, the exact xUnit executable and analyzer bootstrap edge. Other donor rows have specific inapplicability reasons. Removing scopes cannot pass by clearing their backreferences. Fresh actual Godot projects build with the compliant companion; changing only `_Ready` retains handwritten SCR0001 at `Phase1.cs(32,29)` while eight generated-source hashes remain unchanged. AC-08 performs its own independent fresh build/mutation; `rules.exemptions` independently demonstrates a compiler-accepted pragma bypass rejected by the required source guard at `(1,1)`.

Numeric controls execute actual 119/120/121 UTF-16 neighbors for ASCII/Cyrillic/supplementary/combining text under LF/CRLF, body lines 59/60/61 and continuation offsets 7/8/9. Exact numeric metadata rejects overflow, fractions and wrong types without losing the rule identity. Invalid UTF-8 and byte-probe neighbors preserve declared byte units. Independent ordering, real rerun/input immutability and real concurrency/evidence isolation controls pass. Coverage metadata validation supplies no coverage-measurement credit.

## Task commits

Each task was built through verified atomic increments; genuine RED commits precede their GREEN implementation.

| Task/increment | Commits |
| --- | --- |
| Task 1 real SCR0006 tracer | `b139948` RED → `3b45318` GREEN |
| Inherited policy and real build expansion | `dd59a77`, `f4d4d79` |
| Source encoding/lock scope | `6241e33` RED → `d95e99e` GREEN |
| Actual formatter/tab selection | `4c608a5` |
| Portable authentic xUnit producer | `f93cef3` RED → `6df071f` GREEN |
| Logging | `df231b8` |
| Retained naming and exact bindings | `ddb3b80` RED → `2602d4e` GREEN |
| Task 2 source/effective suppression | `2a066f1` RED → `3b6b683` GREEN |
| Numeric/generated claims | `b971d01` RED → `673a0fc` GREEN |
| Exact local dispositions | `8afd176` |
| Fresh Godot companion and async stream permission | `16b12e5` |
| Required active inventory and final independent edges | `f78bd25` RED → `114f2c9` GREEN |

Checkpoint/evidence metadata commits: `05e36f8`, `7f58ecb`, `559b0c0`, `c5bac24`, `03c432d`, `a1920b4`, `c654fdb`, `de0a8af`, `a5c8f65`, `1f20709`, `38b2af4`, `cc964c2`.

The persisted ledger measures 34 commits through `cc964c2`: 32 owned plan increments plus the two preserved independent Phase 2 documentation commits `eea1373`/`e42e58f`. Those Phase 2 files are excluded from the realized owned diff: 1,049,640 raw diff characters / 4 = 262,410 estimate-scale tokens across 55 owned files. This is not a harness token count. The 289-minute observed wall window begins at the first tracer RED commit (`2026-11-13T21:19:35Z`); earlier editing is not measured. Final completion metadata is separate from this measured pre-summary ledger.

## TDD evidence

All listed evidence uses an actually executed C# target after strict harness compilation, an unchanged supported JUnit report and `RED_EVIDENCE_OK` semantic assessment. Infrastructure/setup/style failures provide no RED credit.

| Cycle | Actual intended RED | Evidence under `.phase1/temp/` |
| --- | --- | --- |
| Real build tracer | Planned recognizer assertion fails despite actual SCR0006 at `(6,25)` | `08-red.json` / `08-red.xml` |
| Source format | Invalid UTF-8 expected one diagnostic; stub returned zero | `08-source-red.json` / `08-source-red.xml` |
| Producer portability | Restored genuine relocated consumer cannot clean-build with path-bound generated identity | `08-producer-red.json` / `08-producer-red.xml` |
| Retained names | `GetAll` incorrectly accepted by stub | `08-names-red.json` / `08-names-red.xml` |
| Suppression | Real pragma expected one diagnostic; stub returned zero | `08-suppression-red.json` / `08-suppression-red.xml` |
| Numeric identity | Generic structure error omitted required `R-07: NUMERIC_VALUE` | `08-numeric-red.json` / `08-numeric-red.xml` |
| Generated claims | `.g.i.cs` origin claim returned zero instead of one | `08-claims-red.json` / `08-claims-red.xml` |
| Required scopes | Removed inventory and cleared references incorrectly returned no errors | `08-empty-red.json` / `08-empty-red.xml` |

The numeric evidence description was corrected to the actual generic diagnostic observed in JUnit; no escaped FormatException was claimed afterward. Refactoring extracts bounded helpers and changes identifiers/layout without changing captured sample bytes. One-shot utilities recorded in the checkpoint have already run and must not be reapplied.

## Deviations from plan

1. **[Rule 1 — Bug] Activation prerequisites.** Clean async fixture used a disallowed expression body; generated tuple declaration conflicted with formatter continuation; standalone formatter emission lacked final LF. Compliant baseline/tuple alias and emitted-file-only LF handling fixed these without changing intended negatives, captured `AgreedSamples` or any of twelve rules (`dd59a77`).
2. **[Rule 1 — Bug] Formatter selected zero files.** Absolute project/include paths falsely returned zero success. Relative paths plus a nonzero-selection assertion prove actual subject selection (`4c608a5`).
3. **[Rule 3 — Blocking issue] Authentic xUnit output depended on checkout path.** A bounded late MSBuild override invokes the same pinned/hash-verified task with a stable logical project seed. Evaluated last-target/task arguments, receipts, cache identity and tamper/bypass controls are required; root and two relocated Windows projects pass. Original records/raw bytes are retained in the adaptation provenance (`6df071f`). This follows supported [MSBuild override ordering](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory), without disabled generation, handwritten Main or an absolute seed.
4. **[Rule 2 — Missing critical checks] Naming, suppression, numeric identity and empty inventories.** Actual syntax discovery/current bindings, semantic suppression/effective policy and exact active scope inventory prevent silent omission or broadening. Extra proof helpers/cases are necessary enforcement wiring, not gameplay or a new product subsystem.
5. The three-minute analyzer-class exploratory invocation ended after 215 passes. Focused missing/incomplete class runs completed all 274 unique analyzer tests, and the final independent complete run passes all implemented cases. The aborted invocation alone supplies no complete-suite or RED acceptance.
6. The ledger SDK detected pre-existing table/JSON drift only in WINDOWS23. Its official renderer restored that table row from unchanged canonical JSON, then `windows fixed 23` closed the verified obligation. Other entries remain preserved.

## Remaining phase obligations and risks

No implemented-scope stubs or skipped required cases remain. The 107 pending manifest obligations belong to later work: graph/default visibility and type boundaries (09), state/lifecycle/doc mechanisms, distinct runner-edge closure (14), real 90%/80% measurement (15), and actual Linux/clean-checkout acceptance (16/17). All seven RULE requirements remain Pending. Historical bootstrap RED/WINDOWS entry 1 remains honest and unchanged; plan-08 unfinished-verification entry 23 is fixed with this final report.

The stable xUnit override is deliberately version/target/task pinned and fails visibly on producer shape changes. Two Windows relocation paths are executed evidence; Linux execution is not claimed. The live ignored Linux VM, original donor archive, Phase 2 context/discussion files and independent Phase 2 STATE annotation remain preserved. No auth gate, gameplay/UI capability, extra dependency install or external publication was introduced.

## Self-Check: PASSED

All key created artifacts exist; all 34 measured commits are ancestors of the recorded plan head. The final report has exactly 477 passing results and 107 IMPLEMENTATION_PENDING failures, with no failed implemented control. Source was clean after verification and diff check passed.
