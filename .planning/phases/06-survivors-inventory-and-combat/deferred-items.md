# Phase 6 deferred items

Out-of-scope problems found while executing gap plans. Each needs an owner before the phase closes.

## Found during 06-18

### Phase 4 evidence derivation of guarded presenter tuples no longer matches the policy

- **Where:** `src/godot-game/tools/asset-pipeline/evidence.cjs` `guardedHostTuples` (line 782) and its caller in `src/godot-game/tools/asset-pipeline/cases/runner.cjs` `artifactControls` (line 1116, `assert.equal(guarded.length,251)`). Consumers: the asset-pipeline cases `acceptance.artifacts` and `acceptance.artifacts-source`, and `scenarios.cjs` `inventoryExpectation`.
- **Symptom:** `guardedHostTuples` throws `EVIDENCE_NATIVE_HOST_EXTENT` on the current tree.
- **Cause:** the function reads every entry of `EngineAccessPolicy._presenters` and requires the set to equal the `Check_PresenterNative_RequiresExactGuardedMember` InlineData set of `ContentNativePermissionTests`. At `b9aaa2b` both held the same 251 inspection tuples. Phase 6 added 120 foundation presenter tuples to `_presenters` (FoundationHud.Live, FoundationSpectatorPanel, FoundationInventoryArt, FoundationInventoryGrid, FoundationInventoryWorkspace.Layout, FoundationDrawObserver.ObserveAppearance and others) and one inspection tuple (CharacterPreview.DrawComposition). The policy now holds 372 tuples, while the compiled inspection cases hold 252.
- **Why not fixed in 06-18:** rebinding the count to 252 would not repair it, because the derivation compares sets. A repair must decide whether the evidence inventory covers only the inspection presenters (filter by path) or every guarded presenter (and then which proof executes the 120 foundation tuples). That is a proof-design decision outside the 06-18 file list.
- **Not affected:** `content.native-permissions`, which discovers and executes the 252 inspection cases and passes in 06-18.
