# Phase 1 platform baseline

The [setup](setup.md) and [verification](verification.md) pages define the ordinary commands. This record distinguishes the complete functional Windows baseline owned by 01-16 from the representative genuine Linux acceptance owned by 01-17. Preparation and exploratory setup/build feedback carry no Linux acceptance credit. Coverage and exhaustive certification remain pending in the [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md).

## Same managed payloads and native platforms

Both platforms use the exact committed five-project locks and the four canonical managed packages authenticated by the official Windows Godot archive. Actual ZIP inspection found 28 inner package entries on each platform; SDK, generator, managed DLL and content entries match. Only package relationship/core metadata differs, so raw Linux archive nupkgs have different hashes and are not substituted. Bootstrap verifies the native OS archive separately and launches its actual native binary. Exact package/generated-source hashes and exemptions remain enforced.

The pinned SDK's assembly-attribute writer emits CRLF on Windows and LF on Linux. Linux preflight rejected the unrecognized exact source with SCR0007 at `(4,121)`. The existing four-source layout registry now additionally records the two observed Linux SDK source hashes and actual Linux producer-target hash; the analyzer permits only those complete identities at the same suffixes and line 4. Origin lookup follows the executing runtime's SDK root. Existing controls exercise both exact newline variants, changed bytes, wrong paths and adjacent source. No generic source normalization, extra member suppression or change to the 170 Godot span permissions is involved.

The actual testing-platform `SelfRegisteredExtensions.cs` has one platform-specific newline after its MSBuild hook call. Both existing exact-member registries retain the Windows identity and additionally record observed Linux SHA-256 `4e0ebb8f0acc3074a07b86d417bfffe8168857353c4c2eec668c3f24f0c0bb2e`, authenticated against the same pinned package and exact member. Other template CRLF bytes remain unchanged. Source validation chooses the actual platform's complete raw hash; it never normalizes a subject. Existing origin and adjacent/collision controls cover this adaptation. The combined exploratory Linux inventory checks 560 source/text/package/producer identities with no mismatches, including all 170 unchanged native generated spans. These setup diagnostics carry no 01-17 acceptance credit.

## Predeclared Linux acceptance selection

This selection is declared before 01-17 execution. Run each exact ID with `node tools/phase1/check.cjs --case <ID>` after the clean native bootstrap, locked whole-solution restore and strict build. Each invocation must publish its own fresh canonical report with one selected and accepted obligation, actual exit zero and no missing, failed, blocked or skipped result. Preserve the common vectors unchanged. No exhaustive Linux run or new wrapper is required.

| Exact case ID | Representative obligation |
| --- | --- |
| `R-38.positive` | Compliant named argument analyzer subject |
| `R-38.negative` | Positional argument rejected with exact SCR0001 evidence |
| `ac-12.complete` | Allowed declared/evaluated five-project graph and compiled types |
| `graph.allowed-and-direct` | Real forbidden Core-to-engine reference rejected |
| `docs.portable` | Actual portable documentation contract and missing-entry rejection |
| `docs.projects` | Five module pages match real evaluated project references |
| `state.inputs` | Controlled time/random inputs and preserved identities |
| `state.identities` | Exact ordinal Unicode identities remain distinct |
| `state.golden-vectors` | Existing unchanged independently observed common seeds/vectors |
| `state.numeric-and-random` | Restored random position continues the exact stream |
| `state.snapshot` | Detached state serializes and round trips exactly |
| `state.restore` | Accepted snapshot replaces one complete root |
| `engine.lifecycle` | Actual construction, callbacks, process, signals, teardown, worker/main-thread and old-generation fences |
| `engine.exports` | Real resource/export/binding values and required-null failure before use |
| `engine.generated-origin` | All 170 actual generated spans and exact producer payload |
| `engine.handwritten-partial` | Real handwritten companion retains exact SCR0001 boundary |

The Linux environment, access, prepared source and pinned prerequisite details are recorded in [01-ENVIRONMENT.md](../../.planning/phases/01-code-rules-and-documentation/01-ENVIRONMENT.md). The formal Linux results and separate qualitative review remain pending for 01-17.
