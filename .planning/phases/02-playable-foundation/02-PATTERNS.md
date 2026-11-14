# Phase 02 — Existing Patterns

Research map for planning; proposed additions are not existing implementations. Sources were opened during this research.

| New responsibility | Closest existing analog | Reuse / adaptation |
|---|---|---|
| Pure identified actor state | `src/core/StableIdentity.cs`, `ExampleStateStore.cs`, `IElapsedInput.cs` | Ordinal identity, immutable state/read boundary, explicit elapsed input; keep proof types internal and create minimal deliberate gameplay API |
| Production composition root | `src/godot-game/proofs/Phase1.cs` | One engine-owned script; retain proof fixture independently; no proof nonce requirement in ordinary game startup |
| Guarded native methods | `tests/phase1/EngineAccessPolicy.cs`, `proofs/MainThreadGateway.cs` | Extend exact path/namespace/type/method inventory for production adapters; synchronous preceding guard; no native captures or broad permission |
| Scene teardown | `proofs/LifetimeToken.cs`, `LifecycleFixture.cs` | Close acceptance before cancellation/disconnection; old callbacks cannot mutate new scene |
| Exact generated permissions | `tools/code-rules/EngineGeneratedSyntax.cs`, `tests/phase1/EngineGeneratedTests.cs` | Actual package/text/span identities; account for assembly-wide script metadata; retain adjacent/impostor/collision rejection |
| Public gameplay contract permission | `docs/code/visibility-exemptions.json` | Every new public production type needs exact justified entry and proof; do not broaden all Core types |
| Test organization | `tests/phase1/KingdomZombies.Phase1.Tests.csproj` | Add C# tests to existing host; no sixth project |
| Required-case reconciliation | `tools/phase1/check.cjs` | Independent catalog, unique exact IDs, nonce/input/tool/platform binding, immutable evidence, reject zero/missing/duplicate/stale |
| Process negative controls | `tests/phase1/RunnerContractTests.cs`, `RunnerProcessFixture.cs` | Real missing tools/crashes/timeouts and owned cleanup; setup must succeed before intended rejection is credited |
| Native launch | `tests/phase1/EngineRun.cs` | Pinned native executable and isolated process; replace headless mode for required Phase 2 rendered scenarios |
| Documentation | `docs/workflow/verification.md`, module indexes | Focused linked reference, honest pending evidence, exact portable links, distinct environment obligations |

## Gaps to Implement

- No existing gameplay model, production menu/input/camera, or foundation smoke flow is claimed reusable.
- One root script plus ordinary helper classes minimizes generated permissions; native operations in helpers still require exact guard-policy entries.
- A real OS input driver and display/rendering fixture are required; existing headless engine proof is insufficient.
- Phase 2 small case catalog and fail controls precede acceptance; the comprehensive Phase 5 framework and reuse selection remain later work.

See [research](02-RESEARCH.md) for claim provenance and [validation](02-VALIDATION.md) for proposed verification commands and complete acceptance coverage.
