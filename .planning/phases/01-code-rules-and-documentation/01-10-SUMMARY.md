---
phase: 01-code-rules-and-documentation
plan: "10"
subsystem: core-state-contracts
tags: [csharp, net10, immutable-state, ordinal-identity, random-replay, tdd]
requires:
  - phase: 01-09
    provides: Evaluated five-project graph, compiled boundaries and structural rule mechanisms
provides:
  - Validated immutable non-game state with explicit integer elapsed and random inputs
  - Exact ordinal identities with bounded valid UTF-16 text
  - Bounded seeded System.Random continuation and independently observed pinned-runtime vectors
  - Real clock bans, exact semantic random-adapter scope and production model/enum proofs
affects: [01-11, 01-14, 01-15, 01-16, 01-17, future-core-models]
tech-stack:
  added: []
  patterns: [checked integer milliseconds, detached immutable collections, ordinal identity equality, versioned bounded replay]
key-files:
  created:
    - src/core/ExampleState.cs
    - src/core/StableIdentity.cs
    - src/core/IElapsedInput.cs
    - src/core/IRandomInput.cs
    - src/core/ReplayRandom.cs
    - src/core/ProofKind.cs
    - tests/phase1/StateInputTests.cs
    - tests/phase1/RandomContractTests.cs
    - tests/phase1/ControlledElapsed.cs
    - tests/phase1/ControlledRandom.cs
    - tests/phase1/ControlledInputPolicy.cs
  modified:
    - BannedSymbols.txt
    - src/core/KingdomZombies.Core.csproj
    - tests/phase1/KingdomZombies.Phase1.Tests.csproj
    - tests/phase1/packages.lock.json
    - tests/phase1/ProjectEvaluator.cs
    - tests/phase1/ArchitecturePolicy.cs
    - tests/phase1/ArchitectureRuleTests.cs
    - tests/phase1/RegisterValidator.cs
    - tools/phase1/projects.json
    - tools/phase1/required-cases.json
    - docs/code/register.json
key-decisions:
  - Keep the proof model internal; expose it only to the existing test assembly through a scoped friend declaration and explicit test-to-Core inspection edge.
  - Replay exactly NextInt64(0,long.MaxValue) under dotnet-10.0.15/next-int64-v1, bounded to 1000000 accepted draws; no runtime-independent sequence guarantee.
  - Preserve exact ordinal UTF-16 identities and checked integer milliseconds without normalization, floating-point conversion or implicit clock input.
requirements-completed: []
requirements-contributed: [RULE-03, RULE-07]
coverage:
  - id: D1
    description: Real controlled Core advance, detached ownership, zero and invalid time behavior, exact Unicode identities
    requirement: RULE-07
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case state.inputs
        status: pass
    human_judgment: false
  - id: D2
    description: Pinned random continuation, common vectors, numeric neighbors and exact uncontrolled-input boundaries
    requirement: RULE-07
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case state.numeric-and-random
        status: pass
      - kind: integration
        ref: .phase1/runs/db2fb6c5-f77d-4bbc-8d2f-e535f25e2d72/summary.json
        status: pass
    human_judgment: false
actuals:
  tokens: 18595
  tasks: 2
  commits: 4
commits: 4
plan_head_before: 3fd7ba9bccc15526ac17fb7171a0364e9eb11af4
plan_head_after: 8ab8f8a13e09390f58bd51811dd1f4f123de1527
duration: 62min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 10: Controlled time, randomness and stable identities Summary

**The real immutable Core proof state now advances through explicit integer time and bounded replay-random inputs while preserving exact identities, verified by 563 accepted fresh controls.**

Both owned tasks are complete. Phase 1 remains incomplete, and every shared RULE requirement remains Pending. This plan contributes to RULE-03/RULE-07; it does not complete the snapshot, lifecycle, documentation, coverage or cross-platform obligations owned by later plans.

## Final verification

The single final immutable `node tools/phase1/check.cjs --all` run reconciled **563 accepted controls, zero failed controls and 55 IMPLEMENTATION_PENDING obligations of 618 declared cases**. The actual exit was **1 solely because the later-owner obligations remain pending**. Pending obligations never count as passes. All 26 graph-group cases and all implemented state cases passed within this full run.

- Canonical report: `.phase1/runs/db2fb6c5-f77d-4bbc-8d2f-e535f25e2d72/summary.json`.
- Console and actual exit: `.phase1/temp/10-final-all.log` and `.phase1/temp/10-final-all.exit`.
- Tested revision: `8ab8f8a13e09390f58bd51811dd1f4f123de1527`.
- Input identity: `682ecbac022ac3d718c2b708c9ff8f7441b3f601d56bec6451ebfb5bab1088aa`.
- Tool identity: `eae3c54579676fe56720bea9b8c86b55f0fe98cc23a952595b408e6f6246e321`.
- Actual platform: `win32-x64-10.0.19044`.
- Run interval: `2026-11-14T05:23:25.318Z` to `2026-11-14T06:05:47.784Z`. Fingerprinted source, documentation, specifications and configuration stayed fixed. The checkout was clean after reconciliation.

Prerequisites completed before that run:

- Strict whole `KingdomZombies.slnx` build with `--no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`: zero warnings and zero errors.
- Task 1 focused C# report: 5/5 passed, `.phase1/temp/10-task1-green.xml`.
- Task 2 focused C# report: 13/13 passed, `.phase1/temp/10-task2-green.xml`.
- Exact task CLI cases: `state.inputs` accepted 1/1 in `45f91b57-bd80-48d0-ba61-bca6f288c5e9`; `state.numeric-and-random` accepted 1/1 in `5cc6bbec-43a0-4728-abae-614abd64f44e`.
- Unique naming/bindings and register mutations: 37/37 passed, `.phase1/temp/10-preflight.xml`.
- Source format, unchanged five canonical LF locks across locked restore, scoped banned inputs and all twelve analyzer source controls: 4/4 passed, `.phase1/temp/10-final-prerequisites.xml`.
- Affected actual production model/enum mutations and allowed/forbidden graph: 9/9 passed, `.phase1/temp/10-production-structure.xml`.
- Actual pinned Godot native-host compatibility probe passed, nonce `17a7b1ae-a007-483b-b724-6fffb8e53abe`; this remains raw Windows compatibility evidence, not a Linux or full lifecycle pass.
- `git diff --check` passed. No tracked file was deleted.

The final full run independently covers all final implementations and bindings, including the last added enum removal/order controls and real production subject locations.

## Delivered behavior

`ExampleState` is a small non-game immutable model. It contains nonnegative signed 64-bit elapsed milliseconds, a meaningful stable-ID order and an unordered identity/value map. Construction validates the complete candidate, rejects duplicate or mismatched identities, then takes detached collection copies behind read-only views. Nonzero advance checks the elapsed delta and checked total before drawing; it replaces the first ordered record's value with the explicit draw and retains the same identity objects. Zero delta returns the existing root, consumes no random draw and has no effect. Invalid time, overflow and exhausted random input cannot replace the prior root.

`StableIdentity` uses ordinal equality and hashing over exact supplied text. It accepts 1..128 UTF-16 units, rejects control characters and unpaired surrogates, and preserves case, permitted whitespace, composed/decomposed forms and supplementary characters. Proofs cover lengths 127/128/129, surrogate-pair boundaries, combining text, duplicate exact keys, independent runs and mutable-input detachment.

`ReplayRandom` persists explicit seed, exact runtime-contract name and position 0..1000000. It reconstructs the same sequence by replaying exactly `NextInt64(0,long.MaxValue)` and advances the accepted position once per draw. The upper limit rejects the next draw before state replacement. Common five-draw vectors for seeds 0, 1, int.MinValue and int.MaxValue were observed directly with `System.Random` before implementing the adapter and retained in `.phase1/temp/10-independent-vectors.json`. Assertions contain hardcoded expected observations; they do not calculate expectations with the adapter under test. Independent and restored streams compare the same next draws. Linux must execute these same constants later.

The contract intentionally pins runtime 10.0.15; Microsoft documents that random implementations can change between runtime versions ([System.Random documentation](https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-random)). No runtime-independent PRNG stability is claimed.

The actual integer parsing boundary rejects negative, fractional, nonfinite, exponent, malformed and overflow text without a floating-point model. Exact values above 2^53 and through long.MaxValue survive parsing and checked operations. `ProofKind` retains explicit `Unknown = 0` and `Sample = 1` with pinned names; construction rejects Unknown and undefined values. Existing structural rules now also inspect and mutate the real production state/enum. Enum evolution checks separately reject removal, renaming, renumbering, reordering and duplicate values while allowing additive evolution. Qualitative evolution review remains Pending.

Production banned APIs reject DateTime, DateTimeOffset, TickCount/TickCount64, Stopwatch, Random.Shared and parameterless Random through real RS0030 consumer builds. A resolved-symbol policy restricts seeded Random use to the exact `ReplayRandom` file/type/constructor, readonly field and draw surface; alias, adjacent-type/file and target-typed construction negatives exercise that mechanism. The existing trusted test policy remains separate, so runner clocks and bounded process coordination remain operational. No suppression, severity or generated permission was broadened.

## TDD evidence and task commits

| Task | RED | GREEN | Evidence |
| --- | --- | --- | --- |
| 1: Real controlled advance | `386f9a5` | `e080089` | `10-task1-red.xml/json`: strict build succeeded; actual Advance target executed once and failed the intended elapsed assertion, expected 12 but observed 7. Classifier returned RED_EVIDENCE_OK. |
| 2: Seeded replay continuation | `a4b1b4c` | `8ab8f8a` | `10-task2-red.xml/json`: strict build and independent raw observation succeeded; actual restored adapter target executed once and failed its required positive-draw assertion, observing zero. Classifier returned RED_EVIDENCE_OK. Final GREEN asserts the independently observed exact third draw and subsequent state value. |

Both unchanged supported JUnit reports and semantic assessments remain under `.phase1/temp/`. Infrastructure/style/restore failures never supplied RED credit. The initial standalone observation-file build did not satisfy the strict infrastructure policy and was not credited; the successful independent observation ran through the trusted pinned C# test host. The tracer was reverified end-to-end after task 1 GREEN and before task 2 expansion. No TDD gate violation or additional refactor commit occurred.

The persisted ledger measures 4 commits from `3fd7ba9bccc15526ac17fb7171a0364e9eb11af4` through `8ab8f8a13e09390f58bd51811dd1f4f123de1527`. Actual tokens are realized committed diff characters/4, rounded up (18595), not model token usage. The observed execution window is 62 minutes rounded up from the first RED run at `05:04:41.347Z` through final reconciliation at `06:05:47.881Z`; initial context reading and metadata closure are additional.

## Deviations from Plan

### Auto-fixed issues

**1. [Rule 3 - Blocking] Wired the actual internal Core proof into the existing test project.**

- **Found during:** Task 1.
- **Issue:** The prior test project had no Core compile reference, and a new project reference changes its canonical NuGet lock graph.
- **Fix:** Added the explicit test-to-Core inspection edge, a scoped friend assembly, the same independent expected graph permission, and regenerated only the existing pinned test lock. Restored canonical LF bytes and proved locked restore preserves all five lock boundaries.
- **Files modified:** Core/test csproj files, `ProjectEvaluator.cs`, `projects.json`, test `packages.lock.json`.
- **Commits:** `386f9a5`, `e080089`; final full acceptance covers the final graph.
- **Ledger:** WINDOWS entry 24 records this adjustment as fixed. Historical entries 1 and 9 remain unchanged and open.

Necessary small helper, manifest, register and production-model wiring stayed within the approved proof scope. No sixth repository project, gameplay behavior, package substitution, immutable donor edit or installed-tool mutation was introduced. No authentication or package-legitimacy gate occurred.

## Remaining phase obligations

The 55 later-owner obligations remain required: platform 1, register 1, rules 22, documentation 7, engine 9, state 10 and pins 5. State obligations requiring snapshot serialization, strict candidate parsing and atomic restoration remain explicitly Pending under 01-11; current identity examples do not claim those round trips. Plans 01-12/13/14/15/16/17 retain lifecycle, documentation, shared binding closure, measured coverage, clean-checkout and actual Linux ownership. The five older pin-runner-edge identities remain distinct for 01-14. The prepared Linux VM and captured Phase 2 context were preserved.

No owned goal-blocking stub, skipped test or unrun verification remains. Empty collections in tests are intentional valid boundary inputs. No new network endpoint, auth path, production file-access boundary or persisted schema was introduced. All RULE requirements remain Pending until their remaining sibling obligations are complete.

## Next action

Continue Phase 1 with 01-11 strict snapshots and atomic restoration. Reuse the internal validated model and pinned random continuation without adding gameplay or changing the approved later-phase ownership.

## Self-Check: PASSED

All 11 created implementation/proof artifacts and this SUMMARY exist. All four RED/GREEN commits are ancestors of HEAD. The measured ledger range is nonzero, all source changes are committed, and final canonical reconciliation accepts all 563 implemented controls with zero failed controls. Both genuine RED reports, the independent vector observation and the immutable full report remain available under `.phase1/`.
