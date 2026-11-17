# 04-07 — proof-owned image lifetime continuation

Coordinator decision2026-11-17 after checkpoint2eda441. Task1 commits2860b5b/ac5cb91 and mandatory tracer80c67af1 remain valid. Task2 remains incomplete; its original two corrective batches are exhausted, distinct from Task1's two. No budget is reset. Exact eight implementation WIP files plus amendment/backups are in `.phase4/checkpoints/04-07-r2-boundary/manifest.json`.

## Observed failed run

Immutable complete-fitfa05574b-d349-47b0-b520-0a6cb8599966 authenticates PID29872/nonce/DLL, source replay, strict production/harness0warnings/errors and16158 true observations/7936tuples/12H13. The process then exits3221225501/0xC000001D after printing PASS. Canonical status remainsFAIL. Stderr enters`godotsharp_internal_refcounted_disposed`,GodotObject.Dispose/Finalize andGC.RunFinalizers. This establishes the failure boundary, not a proven root cause.

Parent inspected the actual emitted CharacterHarness.cs and composition in bodies-b.cjs. The inherited FIT_HARNESS.ExactPixels in existing`tools/phase4/cases/bodies-a.cjs` creates three proof-owned Images per call: decoded source, cropped region and actual texture copy. None has explicit scoped disposal. B derives this exact helper from A. Fixing that single shared helper avoids a second string-replacement implementation and also tests the original A case honestly.

## Prospective R1 — maximumtwo corrective attempts

This exact amendment expands the already-owned bodies-a.cjs beyond its prior six-path FIT_FILES change **only** for the three Image lifetimes inside FIT_HARNESS.ExactPixels. No additional tracked path:07 union remains12 and phase inventory234. Record the amendment before source edits.

1. Pin root/branch/index and verify every checkpoint live/backup hash. Preserve all original sources, failures, generated originals/prompts and the unexecuted automatic-review rejection. The rejected`added&&` width bypass must never return; unconditional old/new body and pixel assertions stay.
2. Add explicit scoped disposal (`using` or equivalent guaranteed finally) to the helper's decoded source Image, cropped Image and texture.GetImage copy, while the owning engine is active. Check pinned local API ownership documentation if needed. Preserve every decode/region/nearest resize, alpha/nontransparent RGB comparison, early-return result, fixture, native authentication and nonzero-exit check. No Sprite/engine-owned object disposal, assertion weakening, cache bypass, production/fit/resource edit, forced successful exit, warning/finalizer suppression, output/deadline change or new native permission.
3. Emit and inspect the composed actual B/C harness to prove those lifetimes are inherited and all assertions/counts remain. Run focused complete-fit; require all16158 outcomes,7936tuples/12H13, strict builds and a real clean exit0 without fatal stderr. An all-true payload plus nonzero exit still fails. If the crash persists, stop with exact evidence; do not assume Image ownership is the established cause or broaden cleanup to production.
4. On focused success freeze inputs and run the fresh entire justified local selection: b-wide/complete-fit, original bodies.a-art/a-fit (2976tuples/12H13/10compiled0skip and original assertions), discovery, and actual affected hub/native/source-data consumers. Preserve known WINDOWS29/30/31 as separately open final23/31 integration; do not rerun unchanged failures merely to repeat them or claim policy permission from observations. No source edits during accepted runs.

R1 starts0/2 used. Persist exact attempts/source mapping; exhaustion or a distinct production defect stops with checkpoint. Environmental escalation may rerun identical inputs without changing assertion credit. On success commit exact tested Task2 files in bounded owned parts, then immediately write/self-check/commit SUMMARY with complete artifacts/symbols/native tuples and418 retained b+c source members. Parent owns global15→16 progress and PR5; allASSET and actual32/33 appearance/listening/physical-input acceptance remain pending.

## R1 result and diagnosed Nonempty image ownership

R1 attempt1 applied only three ExactPixels using declarations; emitted harness otherwise matchedfa05574b. Canonicalafbfc1d8 still had16158true/7936tuples/12H13/strict0warnings, then0xC000001D. The explicit crash-persistence stop applied; no R1attempt2 or production expansion occurred. Checkpoint8fed30a binds11live/backups. R1 remains1/2 used, unresolved for shutdown; do not relabel it as a fix.

The [diagnosis session](../../debug/phase04-body-shutdown.md) then used all three allowed isolated native probes, no tracked fix. Same productionDLL95e78e67fe15c3f7eb6b99661a968670d3e5a137bc7a277058c24cdd3e0ecc60 and full matrix:

- V1, pre-quit collection: exit0/empty stderr, pending finalizers16355→7. Diagnostic only, no permanent collection workaround.
- V2, only scoped Image disposal in inherited FIT_HARNESS.Nonempty: exit0/empty stderr, no collection/delay.
- V3, exact reversal of only V2: exit0xC000001D/133312stderr bytes. All7936tuples/16158true/12H13 and strict0warnings/errors remain in each probe.

Pinned API ownership and the loop count identify13558 helper-owned Texture.GetImage copies per full run. The scoped treatment and reversal establish high confidence in their causal contribution; precise native/finalizer thread interleaving is inferred, not traced. Other production allocations are not required by this demonstrated fix and are not authorized for change.

## Prospective R2 — shared Nonempty scope, maximum2 corrective attempts

Own only the existing`tools/phase4/cases/bodies-a.cjs` FIT_HARNESS.Nonempty Image local: change`var im = sprite.Texture.GetImage()` to guaranteed scoped disposal such as`using var im`. Retain all three R1ExactPixels scopes, six approved b/c dependency identities, all old/new unconditional pixel/width/geometry/negative assertions and process-exit/fatal-stderr gates. No borrowed sprite/texture/atlas/base-resource disposal, production/resource/other-helper edit, forcedGC/delay/exit workaround, suppression, deadline or permission change. Scope remains07union12/phaseinventory234. Record exact before/after shared and emitted harness hashes; prove no other logic differs.

Verify all checkpoint live/backup hashes before editing, allowing only the subsequent documented metadata changes. Run fresh canonical complete-fit and require clean exit0/empty fatalstderr plus16158true/7936tuples/12H13 and strict production/harness0warnings/errors. Diagnostic copies supply no completion credit. Then freeze inputs and execute the full justified local selection fromR1, including original A art/fit, current b-wide/complete-fit, discovery and actual affected hub/native/data consumers, with all original controls preserved. No edits during accepted runs. Any distinct production/source failure stops for precise scope diagnosis; two corrective attempts maximum, no earlier budget reset.

On success commit exact tested Task2 bytes in bounded declared parts, then immediately write/self-check/commit SUMMARY with original failures, automatic-review rejection, all three diagnostic identities, canonical repaired evidence and source/commit mapping. Parent reconciles15→16 only after terminal completion; all phase integration/ASSET/actualacceptance gates remain pending.

## Prospective R3 — bounded native evidence reader, maximum2 corrective attempts

R2's single using declaration produced an actual clean native exit0/empty stderr in0de88801, retaining16158true/7936tuples/12H13/strict0warnings/errors. R2 remains1/2 used; its distinct-failure stop correctly preserved the laterDOCUMENT_LIMIT failure. Parent inspected the unchanged shared content parser: strict UTF8/BOM/duplicate keys/NFC/finite numbers/forbidden keys/depth24 and100000nodes. The actual immutable native report is1725610bytes,107205nodes,depth5. Increasing global source-document limits is neither needed nor authorized.

Own only existing`tools/phase4/cases/bodies-b.cjs` for a report-specific reader and its independent controls. This adds no path;07union12/phaseinventory234 remain. The reader may accept only the owned native-results evidence, with hard **2097152byte/125000node** bounds, depth no greater than the existing24, and equivalent strict UTF8/BOM/duplicate-key/NFC/finite-number/forbidden-key rejection. Preserve canonical path/reparse/file checks using existing safe primitives. Add explicit root/outcome shape checks so pass values are actual booleans; preserve every original count, original ordered outcome identity, PID/nonce/DLL/ALC and successful-process/fatal-stderr requirement. No rawJSON.parse shortcut that drops the existing trust checks, no report truncation, count-only acceptance, evidence mutation or change to content.cjs.

Independently reject malformed/duplicate/BOM/invalidUTF8/forbidden-key/nonfinite/depth and over-byte/over-node fixtures with their actual diagnostics. Prove the complete immutable107205-node report is accepted without changing its bytes and that the shared default still rejects it; no old failure is relabeled a pass. Record measured capacity rationale and all controls. Keep native assertion/harness/production/resources/other shared helper/dependency logic fixed.

R3 starts0/2. Run focused complete-fit with its genuine native clean exit and complete report validation, then freeze inputs and execute the full relevant selection required above, including originalA regressions, b-wide/complete-fit, discovery and actually affected hub/native/data consumers. No source edits during accepted runs. Two corrective attempts maximum; distinct production/security/functional failures stop for precise diagnosis. Existing R1/R2/original/diagnostic budgets stay unchanged. All historical failures, report-parser failure and checkpoint backups remain immutable.

The previous executor's SDK checkpoint metadata commit returned staging_failed without writing the index; the own REPAIR/CHECKPOINT WIP remains retained. Parent reconciles and commits only those scoped metadata paths through normal guarded Git with hooks. No broad stage, hook bypass or foreign planning change is authorized. On actual local success follow the existing exact Task2→SUMMARY transaction; allASSET/phaseintegration/actualacceptance remain pending.

## R1 execution record — prospective metadata

2026-11-17: root `D:/Work/Games/KingdomZombies`, branch `feature/04-asset-production`, HEAD `62ef4a69ab1ef9521a17c13ba358af218e47ef03`, empty index. Authenticated all nine manifest rows against both live and backup SHA256, and all23 failed-run body-inputs against live bytes before editing. Ordinary Task1/Task2 correction counts remain2/2 each. Shared-helper continuation R1 attempt1 of maximum2 now authorizes exactly three `using` tokens before the existing `var source`, `var im` and `var actual` declarations in FIT_HARNESS.ExactPixels in bodies-a.cjs. All expressions, assertions, original fixtures, production sources/resources, process gates and deadlines remain byte-identical. The emitted B/C harness must equal the retained failed harness after removing exactly these three lifetime tokens. No conclusion about the native crash cause is assumed.

## R1 attempt1 outcome — stopped, canonical FAIL

The helper changed only those three lifetime tokens; actual composed eight-body harness equivalence against fa05574b passed. Live bodies-a.cjs SHA256 is `59d0a7f7fb4fb9dcf1e16ef727308860fb4b4e97bf7087a0341d5fc39eb0d90d`. Initial sandbox invocation failed before creating a case at `spawnSync git EPERM`, supplying no compiler/assertion credit. The identical canonical command with the established isolated .NET environment then ran under approved equivalent escalation, with no source changes or permission-policy changes.

Focused invocation `afbfc1d8-9875-41b0-a6f7-334cf15a72b1` ran 2026-11-17T05:24:56.764Z–05:25:54.406Z (57.642s), tested HEAD62ef4a6 plus the exact WIP. Source b/c replay passed; production build11.83s and harness build3.36s each reported0 warnings/0 errors. Actual native PID9580/nonce authenticated DLL `95e78e67fe15c3f7eb6b99661a968670d3e5a137bc7a277058c24cdd3e0ecc60`. Raw report has16158/16158 true outcomes,7936 tuples/0 failures,12H13, logical hands/replacement/worker/detach true, including the owning-ALC assertion. It printed PASS, then exited3221225501 (`0xC000001D`); therefore the canonical result is FAIL, with no process/gate completion credit.

The new45998-byte fatal stderr begins `Fatal error.`, followed by `csharp_lang && !csharp_lang->script_bindings.is_empty()` at `modules/mono/csharp_script.cpp:1220`, and leaked unsafe references naming ImageTexture, AtlasTexture and Image at `csharp_script.cpp:179`. Unlike the preceding264-byte stderr, this is not evidence that the shared helper alone resolves shutdown. Exact root cause remains undiagnosed. No production/cache/Sprite disposal, forced collection/exit, suppression, assertion change, timeout/output-cap change or additional native permission was attempted.

| Immutable focused artifact | Bytes | SHA256 |
| --- | ---: | --- |
| body-matrix/native-results.json |1725609|6d12ad508480154fb181218294ac283a74346316b22c5fb3fce37db0df530346|
| body-matrix-native.stderr.log |45998|4c651bc3a36b3b5424381f3bbcc511d3351f698643f8687d791ead7e416479c2|
| body-matrix-native.exit.json |100|54c220844adc5ab747bf5fe5dbb2299915581fab14094888a370942a14e0c240|
| body-inputs.json |3359|37fc49754b0fc01ebc090800fd75f84e24d24286dde98e986bc46ff6209bd671|
| body-matrix/CharacterHarness.cs |17850|e196871de3859929d00d32620aa90385c713c2a529852f60cadf01a0c4ec3a0b|
| result.json |1117|ba9cda79aadaa77cced5aa72aa671ba50da56b6f859b8a20f612aec4d3239239|

All23 focused body-inputs still match live files after completion. The previous checkpoint/backups and failed fa05574b payload remain intact. New exact WIP backup manifest is `.phase4/checkpoints/04-07-r1-shared-image/manifest.json`. R1 is1/2 consumed; **the explicit crash-persistence stop applies even with one attempt nominally remaining**. Task2 remains incomplete and uncommitted; no second repair or broader production change is authorized by this record. Full fresh selected regression, complete artifacts/symbol/native tuple inventory and SUMMARY remain outstanding. Parent must diagnose the exact remaining lifetime boundary before a precise new amendment. Foreign WIP remains preserved and unstaged; global progress stays15/37, all ASSET/actual32/33 acceptance pending.

## R2 attempt1 outcome — clean native exit, distinct report-parser blocker

2026-11-17 continuation pinned root `D:/Work/Games/KingdomZombies`, branch `feature/04-asset-production`, HEAD `1ba9dfe85dcd7b9707330ab3bb35646dc11bc1b3`, empty index. All11 prior backups authenticated; all eight implementation files plus AMENDMENTS/CHECKPOINT matched their live hashes. Only REPAIR differed, with the coordinator's already committed subsequent diagnosis/R2 authorization. No earlier budget was reset.

Applied exactly one `using` token in shared FIT_HARNESS.Nonempty. Shared before `59d0a7f7fb4fb9dcf1e16ef727308860fb4b4e97bf7087a0341d5fc39eb0d90d`, after `55b29d8c9b8ea1c61b19f553a7eb1b221bf482715883ddcad45c4a5bee66f433`; emitted eight-body harness before `e196871de3859929d00d32620aa90385c713c2a529852f60cadf01a0c4ec3a0b`, after `14e9a319fb556bdf96a68b4846e69babc39b84c9ab398740b212b58974484fde`. Exact reversal of that token reproduces each original source byte; all assertions, R1 scopes and production/resources remain unchanged. Scope proof is `.phase4/04-07-r2/scope.json`.

Initial sandbox invocation failed at `spawnSync git EPERM` before creating a canonical case, with no assertion credit. Equivalent authorized escalation ran fresh canonical `bodies.complete-fit` invocation `0de88801-e891-4edf-bb64-4f01e26ba02b`, 2026-11-17T05:51:27.987Z–05:52:25.898Z. Both source replays passed. Production build11.66s and harness build3.33s each reported0warnings/0errors. Native PID32100/nonce/current production DLL `bf052cbc1723a7eafc3052d5ba96c248268372847156d16201f724eb38388868` authenticate against the fresh closure. This DLL is a fresh build, not the diagnostic95e78e67 identity.

The actual native process printed its exact PASS marker and exited0 at05:52:25.828Z, with0stderr bytes. Read-only inspection of the immutable raw JSON confirms16158/16158true,7936tuples/0failed,12H13, logical hands/replacement/worker/detach/owningALC alltrue. All23 input hashes still match live bytes. This establishes the clean canonical native contrast, but **the canonical case remains FAIL**: its next step calls the unchanged shared `document(project,'native-results.json')` at bodies-b.cjs:109, which rejects `DOCUMENT_LIMIT native-results.json` at content.cjs:50. The report contains107205 recursively counted nodes, maximum depth5; the shared parser permits100000. Earlier native crashes masked this post-process reader boundary. No report, parser, count, assertion, timeout or output limit was changed to accept it.

| Immutable focused artifact | Bytes | SHA256 |
| --- | ---: | --- |
| body-matrix/native-results.json |1725610|bf03e12dc3176d722368c03babe3037e49fdb21899dcb1a7409cf923a4a2a023|
| body-matrix-native.stderr.log |0|e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855|
| body-matrix-native.exit.json |92|e988f7081f89f5c5ba8849fd43c482c8b6d9d1d4f87f193a429d7f84e3b4058f|
| body-inputs.json |3359|dad904ca2e4cd267f11f20d9eb25b15c832df2cde6f5926bf6a3ccbf21435bda|
| body-matrix/CharacterHarness.cs |17856|14e9a319fb556bdf96a68b4846e69babc39b84c9ab398740b212b58974484fde|
| result.json |1352|03e5ad3ecebcc8013ce48924a530baeacdd03ce8ad2dc78100615aa7bb096f2c|

R2 is1/2 used. The distinct-failure stop applies: no second attempt, parser/source expansion, full selected regression, Task2 commit or SUMMARY. The exact raw inspection is `.phase4/04-07-r2/canonical-result.json`; it is explicitly diagnostic inspection, not substituted canonical acceptance. A precise coordinator amendment is required for the report-local reader boundary before further changes. Proposed scope is only the existing bodies-b.cjs native result-reader call, retaining all outcomes and independent authentication; no global content parser relaxation is authorized. Checkpoint backup is `.phase4/checkpoints/04-07-r2-report-limit/manifest.json`. Foreign WIP, all historical failures and all418 retained b/c members remain preserved; global15/37, WINDOWS29/30/31, allASSET and actual32/33 gates remain pending.
