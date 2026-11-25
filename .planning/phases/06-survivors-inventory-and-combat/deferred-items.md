# Phase 6 deferred items

Out-of-scope problems found while executing gap plans. Each needs an owner before the phase closes.

## Found during 06-18

### Phase 4 evidence derivation of guarded presenter tuples no longer matches the policy

- **Where:** `src/godot-game/tools/asset-pipeline/evidence.cjs` `guardedHostTuples` (line 782) and its caller in `src/godot-game/tools/asset-pipeline/cases/runner.cjs` `artifactControls` (line 1116, `assert.equal(guarded.length,251)`). Consumers: the asset-pipeline cases `acceptance.artifacts` and `acceptance.artifacts-source`, and `scenarios.cjs` `inventoryExpectation`.
- **Symptom:** `guardedHostTuples` throws `EVIDENCE_NATIVE_HOST_EXTENT` on the current tree.
- **Cause:** the function reads every entry of `EngineAccessPolicy._presenters` and requires the set to equal the `Check_PresenterNative_RequiresExactGuardedMember` InlineData set of `ContentNativePermissionTests`. At `b9aaa2b` both held the same 251 inspection tuples. Phase 6 added 120 foundation presenter tuples to `_presenters` (FoundationHud.Live, FoundationSpectatorPanel, FoundationInventoryArt, FoundationInventoryGrid, FoundationInventoryWorkspace.Layout, FoundationDrawObserver.ObserveAppearance and others) and one inspection tuple (CharacterPreview.DrawComposition). The policy now holds 372 tuples, while the compiled inspection cases hold 252.
- **Why not fixed in 06-18:** rebinding the count to 252 would not repair it, because the derivation compares sets. A repair must decide whether the evidence inventory covers only the inspection presenters (filter by path) or every guarded presenter (and then which proof executes the 120 foundation tuples). That is a proof-design decision outside the 06-18 file list.
- **Not affected:** `content.native-permissions`, which discovers and executes the 252 inspection cases and passes in 06-18.

## Found during 06-20

### gameplay-checks.core.permissions rejects src/godot-game/core/CharacterPresentation.cs

- **Where:** `node src/godot-game/tools/game-tests/check.cjs --case gameplay-checks.core.permissions`, which runs `src/godot-game/tools/gameplay-checks/check.cjs --case core.permissions` (staged caller plan in `csharp()`).
- **Symptom:** `FAIL core.permissions: src/godot-game/core/CharacterPresentation.cs`. The result file carries only that path as its message.
- **Not caused by 06-20:** the same failure is recorded on 2026-11-24 (gameplay-checks run `f897841c`), before this plan, and 06-20 changes no Core file. Rerun in 06-20 as gameplay-checks run `998e8bf5`.
- **Why not fixed in 06-20:** the staged permission inventory for Core sources is outside the 06-20 file list and needs its owner to decide whether the file is a permitted Core input.

### One more foundation presenter tuple for the 06-18 evidence derivation

- 06-20 adds the guarded presenter `FoundationHud.KeepHintClosed()` to `EngineAccessPolicy._presenters`. The 06-18 item above (`guardedHostTuples`) now sees 373 policy tuples. Its repair decision is unchanged.
