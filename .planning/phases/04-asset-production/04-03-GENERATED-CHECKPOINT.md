# Plan 04-03 — Frozen native generated-surface checkpoint

Recorded 2026-11-16 on `feature/04-asset-production`. This is a partial execution handoff, not a SUMMARY or completed task. Task 1 remains incomplete; Task 2 has not started. No plan/requirement/roadmap completion is claimed.

## Frozen source and commits

- RED `f23117fb0cb42c67f264059806fa474cab22cbd9`: the actual compiled `Load_PartialPack_PreservesBowAndCensus` test executed and failed the intended inspectable `common.bow` assertion. Its TAP wrapper and unchanged actual record at `tdd/04-03-task1-red.json` passed the current `tdd-red-evidence` classifier with `RED_EVIDENCE_OK`. Initial test discovery was one actual compiled test, failed1; no build/import/fixture error supplied RED credit.
- Partial subject `08035bac4fa6b850143721e3091257fecdffcc33`: coordinator-authorized five-file freeze after bounded Core verification. The commit explicitly records the native compilation blocker; it is not a completed GREEN task or native pass.
- No edits to the analyzer, permission JSONs, source-policy file or any additional implementation path occurred. Source files must remain frozen while the coordinator arranges the exact generated-authentication prerequisite. No game window, listening or judgment acceptance occurred.

| Frozen source path | Bytes | SHA256 |
| --- | ---: | --- |
| `src/core/ContentPack.cs` | 21843 | `d683d91c1e758afdd5294cd08a070e31fa756f4599188d4e378c55b16da63986` |
| `src/godot-game/inspection/InspectionRoot.cs` | 14163 | `8d3e3d1bff10a81a115e49bd2476deb75fdf04b4ead6d25373834b17c874e8a5` |
| `src/godot-game/inspection/Inspection.tscn` | 6678 | `c13d30b08861868268e96cc1ecc120eaec45bcbd0725e737feea8bf783390d4f` |
| `tests/phase1/ContentInspectorTests.cs` | 11411 | `bb40144282a33d645f0b7db3bb5c22d3dfa7d2687706585850159651d9827a4f` |
| `tools/phase4/cases/hub.cjs` | 3017 | `4e88c08ce59f5fd918a32e77f5ac6f485f3ee4ab4cd71257e5defe85343b68be` |

These hashes bind the final native compiler record and successful Core test record to the exact committed partial subject. The native record reports the preceding RED HEAD and dirty source; its input hashes above are the same bytes subsequently committed as `08035ba`.

## Executed bounded verification

Strict `dotnet build tests/phase1/KingdomZombies.Phase1.Tests.csproj --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` passes with zero warnings/errors. This builds actual production Core and its compiled tests, independently of the Godot project.

Latest `node tools/phase4/check.cjs --case hub.source` passes the explicitly bounded pure-loader scope in invocation `1335f567-1ac8-4806-b7cb-e864d75fa5d5`: `.phase4/runs/1335f567-1ac8-4806-b7cb-e864d75fa5d5/result.json`, `compiled.command.json` and `inputs.json`. All26 compiled controls execute, succeeded26, failed0, skipped0. Earlier invocations remain retained. This case currently proves Core only; actual native hub behavior and final case expansion remain necessary before Task 1 completion.

The26 rows cover partial real bow/census; reordered bad/valid shards; six independent expectation mutations including recomputed index digest; four malformed documents; eight path aliases/escapes; missing media/restoration/immutable snapshots/oversized input; equal names with distinct IDs/shared mappings and zero/one/many searches; and four invalid independent output mutations. Missing index expectation reference also preserves the valid bow.

Final actual native compile is retained at `.phase4/runs/3c94852a-37dd-4af5-9439-8025fee9bf76/`:

- `native-compile.command.json`: exact command, timestamps, native compiler exit1, no game window.
- `native-compile.log`: unchanged actual compiler output, zero warnings,177 errors. Every remaining diagnostic belongs to emitted generated source; there is no remaining diagnostic on authored `ContentPack.cs`, `InspectionRoot.cs` or `ContentInspectorTests.cs` in this build.
- `generated-inventory.json`: all177 distinct actual diagnostic path/line/column/reason observations, generated raw/text hashes, producer/package identity and five frozen source hashes.
- `observed-generated/`: exact raw copies of all four failing emitted files, retained before future regeneration. The earlier scratch `.phase4/hub-build.log` is historical/noncanonical; use this immutable invocation.

Producer is `Godot.SourceGenerators/4.7.2`, from `.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg`, package SHA256 `ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e`. Raw file SHA includes the emitted UTF-8 BOM; text SHA uses decoded text without BOM, matching the existing permission contract.

| Emitted file under `src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/` | Raw SHA256 | Text SHA256 | Actual diagnostics |
| --- | --- | --- | --- |
| `Godot.SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs` | `f1237027ad988c11862683fa6d44d17abb55fe2b18276d282e939b5f5c0e1171` | `aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59` | 51; SCR0001, SCR0006, SCR0007, SCR0008 |
| `Godot.SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs` | `6fe2422134efad21dc0848a86c59c91f2daa96388495a1b3b821af4357a2185b` | `d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d` | 60; SCR0001, SCR0007, SCR0008, SCR0011 |
| `Godot.SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs` | `bb7e5c3865b22c22dc7bfaeed0c2e66e7d0c04cedb279f9863e856bf50f8b9b1` | `41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615` | 65; SCR0001, SCR0011 |
| `Godot.SourceGenerators.ScriptPathAttributeGenerator/AssemblyScriptTypes.generated.cs` | `5d5881e57e36f1a6f4c05b1765a3b009281a043d0c47f6c78e7c8db8b58a5010` | `621c1e81abf2b4d2384f78c56e3c865ad780b952b814bab7ca6a56b896009846` | 1; SCR0007; the actual script-type inventory changed |

Generated members requiring exact observed authentication include `GetGodotMethodList`, `InvokeGodotClassMethod` (76-line emitted body), `HasGodotClassMethod`, `SetGodotClassPropertyValue`, `GetGodotClassPropertyValue`, `GetGodotPropertyList`, `SaveGodotObjectData`, `RestoreGodotObjectData`, nested generated `PropertyName`/`MethodName` fields, and `AssemblyHasScriptsAttribute`. Exact spans must be derived from these retained actual files/diagnostics; this checkpoint grants no permission or estimated span credit.

## Concrete bounded-plan blocker

Current Task 2 has five implementation files and the plan has nine. The exact runtime permission mechanism is `tools/code-rules/EngineGeneratedSyntax.cs`, whose baked path/text-hash/line/column/length tuples are used by the analyzers. Merely appending rows to `docs/code/engine-generated-spans.json` cannot satisfy that mechanism or its genuine origin/boundary proofs.

Three existing affected proof consumers also have hardcoded inventory totals: `tests/phase1/EngineGeneratedTests.cs` expects220 local syntax rows, `tests/phase1/GeneratedCallSiteAdapterTests.cs` expects18 total adaptations, and `tests/phase1/GodotCallSiteAdapterTests.cs` expects16 Godot adaptations. Exact new generated permissions must preserve prior baseline and adjacent/impostor/collision controls while extending these actual consumers. These four paths were not edited. A bounded prerequisite/split is required before editing them; the coordinator will arrange planner/checker correction without a new product decision.

The compiled generator output has not been hand-edited. No assembly SuppressMessage attributes or JSON permissions were added to bypass these observations. New handwritten native tuples and exact public API visibility reasons remain unimplemented Task 2 work; no absent 04-26 Input/Shell member is preauthorized.

## Partial artifact contract for continuation

`ContentPack` is the only new Core type. Public constants: `MaxDocumentBytes`, `MaxResourceBytes`, `ExpectedPath`, `ExpectedSha256`. Public properties: `Entries: IReadOnlyDictionary<string,JsonElement>`, `Expected: IReadOnlyDictionary<string,string>`, `Diagnostics: IReadOnlyList<string>`, `Status: string`. Public methods: `ResourcePath(string): string`, `Paths(byte[]?): IReadOnlyList<string>`, `Load(byte[]?,byte[]?,Func<string,byte[]?>): ContentPack`, `Search(string,string,string): IReadOnlyList<string>`, `Category(string): string`, `Mappings(JsonElement): IReadOnlyList<string>`. Private constructor and helpers are source-inventoried in the frozen file; the final SUMMARY must enumerate them fully after actual task completion. Definitions are cloned and dictionaries/lists read-only. Core has no engine/concrete-service import.

`InspectionRoot` remains a sealed partial `Node2D`, with constructor, `_Ready()`, `_Process(double)`, `_ExitTree()`, and private native-invoking methods `Recheck()`, `Cancel()`, `Read(string): byte[]?`, `ChangeCategory(long)`, `ChangeStatus(long)`, `Filter(string)`, `Select(long)`, `Details()`, `Copy()`, `Project()`, `Rows()`, `Selection()`. Every authored native method starts with the captured readonly `_gateway.RequireThread()`. Actual source-policy positives/negative compiler controls are still required before these become authorized tuples.

The production scene retains direct `Bow`, exact PNG path/filter/pivot/offset, `Summary/ResourceId`, `Summary/ResourceName`, `Summary/Status`, `Summary/PackStatus`, plus actual category/register/known-issues, query/status, result list, metadata/copy/recheck/cancel Controls. Native bow projection, cancellation/recheck and scene loading are not yet proven. The scene naturally declares common.bow; model binding is implemented in the frozen root but still awaits native compilation/observation. The native loader reads only validated portable declared paths with file limits, a256MiB per-load read budget, reparse rejection, per-frame reads and generation cancellation. The independent expected summary remains outside collapsed metadata.

No donor/media/source bundle changed. The bow stays one partial master,28 shards pending,zero completed obligations. All ASSET requirements and actual 04-32/04-33 visual/provenance/listening judgments remain pending. Existing source tracer, profile and export/native-observer adaptations retain their explicit 04-31 owner; their old nine-node proof supplies no expanded-hub credit.

## Resume

Reconcile this frozen subject through a bounded independently checked generated-authentication prerequisite. Then resume Task 1's remaining actual native hub/source proof and tracer gate before Task 2’s exact source-policy/visibility controls. Do not redo RED, discard historical failures, advance progress, write a complete SUMMARY, or claim native/task/plan completion from the bounded Core pass.
