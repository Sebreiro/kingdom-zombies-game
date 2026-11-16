# Cleanup code review

Reviewed inline against origin/dev on 2026-11-16. Automated/AI review only; no human qualitative acceptance is inferred.

## Findings repaired before final verification

- Solution project entries must resolve relative to the relocated solution, then compare as checkout-relative inventory paths. Preserve exact five-project checks.
- The reusable analyzer outside the workspace needs explicit early shared props, late targets, central package policy and NuGet source selection. Keep analyzer-only reference metadata and bootstrap exclusion.
- SDK selection follows CLI cwd. .NET launch helpers select workspace policy and root existing subject paths, while non-.NET and policy-free control fixtures retain their own directory.
- Compiler/graph fixtures must create copied policy parents and mirror the new structure; synthetic provider and compiler consumers must still inherit real rules. Preserve negative controls and lock drift behavior.
- Moving policy/tests under src caused overlapping evidence inputs. Consolidate fingerprint scopes without dropping source/policy coverage; use the same scope in publication, inherited validation and test controls.
- The source-impact selection is a live runner contract. Rebind moved test paths without changing its case selection or rewriting original reports/reviews.
- Documentation non-mutation proof must ask Git for its index path to support linked worktrees. Preserve before/after byte comparison.
- API-input tests compare filenames, while file-reading rules use complete paths. Keep the former as filenames and the latter as new workspace paths.
- NuGet local feed metadata now has a different config-relative spelling but resolves to the same checkout-owned feed. Update the independent declaration alongside config.
- Keep .NET-formatting changes outside this task out of unchanged gameplay sources. Game C# and runtime resources remain byte-identical moves.
- A concatenated Node script in the pin permutation test still read the former tool-manifest path. Rebind the script constant, retaining its declaration/permutation assertions.
- The linked-source ownership control stripped a fixed `../../` prefix instead of resolving Include paths from the project directory. Resolve each actual Include to a checkout-relative path and retain the exact declared-owner comparison.

## Required final selection after test-path repairs

Run strict locked restore/whole-solution build, the 36 documentation and 27 compiler-policy cases, actual evaluated/compiled boundaries (`ac-12.complete`, `ac-13.complete`), captured register and current bindings, source format, positive/negative suppression and conditional ownership, native lifecycle and generated origin. These 73 current obligations plus 23 tool self-tests form the final required repository/build selection. The broader pre-repair run remains diagnostic evidence with its original source revision and failures; do not relabel it as current or as a successful complete run. `ac-06.complete` remains a separately declared Phase 5 coverage item outside this task.

## Scope and evidence

Reference images move under workspace references. Reusable analyzer and phase tool paths stay stable. Root/editor/module links and exact permission path selectors are updated; captured provenance, original execution reports, case IDs, package-lock bytes and unfinished Phase 4 work are preserved. Final build, boundary, documentation, catalog, fixture/runner, representative rule and native checks remain required. Ordinary build/repository PR selection applies; no fresh rendered, LAN, other-platform, coverage or human-review credit is claimed.

## Continuation

Branch: `feature/quick-repository-layout`.
Checkout: `C:/Users/User/AppData/Local/Temp/kz-repository-layout`.
Draft PR: https://github.com/Sebreiro/kingdom-zombies-game/pull/6 (base dev).
Original checkout remains on its active Phase 4 branch.
