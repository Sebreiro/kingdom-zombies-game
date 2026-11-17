---
status: diagnosed
trigger: Coordinator-observed native shutdown failure during authorized Phase04-only autonomous execution
created: 2026-11-17
updated: 2026-11-17
goal: find_root_cause_only
---

# Phase04 eight-body shutdown

## Current Focus

hypothesis: Confirmed causal lifetime defect in FIT_HARNESS.Nonempty: the proof owns each fresh GetImage copy but never disposes it. Deferred GodotObject finalizers overlap native C# binding shutdown in the pinned engine; helper-only deterministic disposal removes the observed failure and exact reversal restores it.
next_action: Parent reviews the diagnosed one-line Nonempty Image scope amendment, then separately authorizes and verifies the tracked repair with the unchanged canonical bodies.complete-fit gate. Diagnosis is complete; no additional native diagnostic or source edit is authorized by this session.
test: Completed bounded full-matrix controlled contrast: canonical failure -> diagnostic GC clean exit; helper using-only clean exit -> exact helper reversal native fatal.
expecting: Parent may authorize a separate precise amendment for Nonempty Image scope; a fresh canonical production gate must verify the tracked repair. Diagnostic copies do not supply completion credit.
bug_class: concurrency (leading; finalizer versus native script-binding shutdown order; reproducible with the eight-body workload)
native_diagnostic_budget: 3/3 native diagnostics complete; no further native diagnostics; production resource retirement variant cancelled before preparation/execution
candidate_causes:
  - code: CharacterPreview creates unscoped AtlasTexture, Image, ImageTexture on every Project; only nodes/signals are retired.
  - code: FIT_HARNESS.Nonempty creates unscoped GetImage results; async Run retains preview, neighbor and node locals through its last await.
  - environment: pinned Godot4.7.2 CSharpLanguage.finalize iterates script_bindings while finalizers may dispose refcounted wrappers.
  - data: eight-body matrix produces more wrappers than the historical six-body workload and changes GC pressure.
and_gate: yes, confirmed helper-owned deferred resources plus pinned engine native/finalizer shutdown ordering; exact thread interleaving is inferred from direct finalizer/native stack and pinned source rather than a native debugger trace
reasoning_checkpoint:
  hypothesis: Nonempty leaves13558fresh proof-owned Image copies/run for GC; finalizers can run during CSharpLanguage.finalize after native binding infrastructure begins retiring, causing the observed0xC000001D.
  confirming_evidence:
    - canonicalafbfc1d8 and exact fresh reversalv3 have identicalharness/DLL/assertions and both fatal after16158alltrue
    - v2 changes only Nonempty using and exits0/stderr0bytes with canonicalDLL/fullmatrix
    - v1 drains finalizers while native engine is alive and exits0/stderr0; memory-info FinalizationPendingCount snapshots16355to7
    - originalfa05574b native stack directly shows GodotObject.Finalize -> Dispose -> godotsharp_internal_refcounted_disposed; reversal fatal names script_bindings nonempty
  falsification_test: A same-input tracked helper repair that retains the fatal shutdown would falsify sufficiency/stability; v3 was the available reversal control and restored the failure. Repeated stability/cross-environment verification remains outside this three-run diagnosis budget.
  fix_rationale: Add using only to Nonempty's GetImage copy, so both the early true return and final false return deterministically dispose helper-owned Image before shutdown; no Sprite/Atlas/base texture is disposed.
  blind_spots: No native debugger thread trace or repeated multi-environment stability sample; production AddFittedSprite/Texture still contains independently observed unscoped allocations, and its broader ownership correctness was not tested or changed.
  candidate_causes:
    - code: missing deterministic disposal of proof-owned Image in Nonempty (confirmed causal contrast)
    - code: production-owned Image/AtlasTexture/ImageTexture lifetime (observed risk, not necessary for this contrast, not production repair authorization)
    - environment: pinnedGodot4.7.2/native binding finalization overlap with managed finalizers (supported stack/source and GC contrast)
    - data: eight-body workload amplifies allocation/GC pressure (volume known; exact triggering threshold not established)
  and_gate: yes helper-lifetime plus native finalization timing; do not label pure deterministic geometry failure or claim engine race scheduling was directly captured

## Symptoms

- expected: Complete all7936 body/pose/facing/equipment tuples and16158 assertions, retire previews/resources and exit the authenticated headless native process0 without fatal stderr.
- actual: All assertions and strict builds pass, then process exits3221225501/0xC000001D after writing results/PASS marker. Canonical gates correctly fail.
- error: Originalfa05574b stack enters godotsharp_internal_refcounted_disposed,GodotObject.Dispose/Finalize andGC.RunFinalizers. After three shared-helper using declarations,afbfc1d8 still fails; stderr starts csharp_lang script_bindings nonempty and leaked ImageTexture/AtlasTexture/Image unsafe references.
- timeline: Task1 six-body5952tuples/12123assertions tracer80c67af1 passed. Eight-bodyfa05574b and shared-helperafbfc1d8 fail at shutdown. Earlier Task1 also retained an abnormal exit after failing proof assertions; it was never called a demonstrated shutdown fix.
- repro: Pinned installed Godot4.7.2 mono Windows, no real window. `node tools/phase4/check.cjs --case bodies.complete-fit` with the frozen checkpointWIP, equivalent escalated process execution for environment restrictions. Current HEAD8fed30a. Sources/backups in `.phase4/checkpoints/04-07-r1-shared-image/manifest.json`; all11live/backup hashes authenticate.

## Evidence

- timestamp: 2026-11-17; source: `.phase4/runs/fa05574b-d349-47b0-b520-0a6cb8599966`; result: PID29872,16158true/7936tuples/12H13,production+harness0warnings/errors,then0xC000001D. Sourcecheckpoint2eda441.
- timestamp: 2026-11-17; source: `.phase4/runs/afbfc1d8-9875-41b0-a6f7-334cf15a72b1`; result: PID9580,samealltrue/strict0/0,then0xC000001D;45998fatalstderr bytes. Emitted harness differs from prior only by three using declarations in shared ExactPixels. Checkpoint8fed30a.
- timestamp: 2026-11-17; source: CharacterPreview.AddFittedSprite/Texture/Detach; result: source.GetImage resized without scoped disposal, new ImageTexture per fitted layer, new AtlasTexture per region; Detach disconnects signals/closes acceptance but has no explicit resource ownership release. Source inspection alone does not establish causation or authorize disposing shared ResourceLoader resources.
- timestamp: 2026-11-17; checked: Phase0 recall and bootstrap; found: No registered MemPalace tool/CLI and no knowledge-base.md; fallback has no entries. Configured debugger agent_skills is empty. SBFL skipped because the observed native shutdown failure has no per-test coverage spectrum; assertions all pass. No fixture/production change yet.
- timestamp: 2026-11-17; checked: complete CharacterPreview.cs and shared/full harness bodies-a.cjs,bodies-b.cjs; found: CharacterPreview.Project removes old children synchronously and queues their node deletion, but every AddBody creates new AtlasTexture and every AddFittedSprite creates AtlasTexture + GetImage + ImageTexture without explicit disposal; Texture caches ResourceLoader base textures. FIT_HARNESS.Nonempty also leaves GetImage unscoped. Run Detach/QueueFree executes and awaits a ProcessFrame before returning; _Ready quits immediately after serializing PASS. Implication: production resource ownership and proof-owned Image lifetime are distinct candidates; shared base textures must not be blindly disposed.
- timestamp: 2026-11-17; checked: canonical stderr and authoritative pinned engine sources ed1daf0bf001b61586d9930840f2f1394092c079; found: afbfc1d8 starts with _instance_binding_free_callback fatal while script_bindings nonempty, then unsafe resource leaks. CSharpLanguage.finalize calls DisposablesTracker_OnGodotShuttingDown, sets finalizing, iterates bindings/free_instance_binding and later clears them. GodotObject finalizer calls Dispose(false), which enters godotsharp_internal_refcounted_disposed for owned refcounted instances. URLs: https://raw.githubusercontent.com/godotengine/godot/ed1daf0bf001b61586d9930840f2f1394092c079/modules/mono/csharp_script.cpp and https://raw.githubusercontent.com/godotengine/godot/ed1daf0bf001b61586d9930840f2f1394092c079/modules/mono/glue/GodotSharp/GodotSharp/Core/GodotObject.base.cs . Implication: exact native source matches the observed shutdown/finalizer path; concurrent binding mutation needs controlled evidence.
- timestamp: 2026-11-17; checked: variant1 prepared provenance; found: .phase4/04-07-debug/v1-gc-before-quit/provenance.json records39 copied source/input files; sole source difference is CharacterHarness.cs pre-quit frame+GC drain. Canonical harness e196871de3859929d00d32620aa90385c713c2a529852f60cadf01a0c4ec3a0b; variant56c1a9a6904f305d3022d0cdb287d63d77fe32954027c5a73d42cdc4342e17ed. Production DLL95e78e67fe15c3f7eb6b99661a968670d3e5a137bc7a277058c24cdd3e0ecc60 and all11 live/checkpoint identities authenticated. Nonce73581930-ed72-4e0e-ad5e-df97cc528d09. Assertion matrix and native Quit exit code remain unchanged; diagnostic collection is not a proposed permanent repair.
- timestamp: 2026-11-17T05:35:53Z; checked: variant1 initial sandbox invocation; found: dotnet spawn EPERM synchronously before a build/native PID exists; diagnostic-result.json records stage build, no observed native result, checkpointUnchanged true. Implication: environment failure supplies no hypothesis evidence; exact same prepared variant requires per-command escalation, with separate immutable retry result.
- timestamp: 2026-11-17T05:36:30Z; checked: variant1 escalated build; found: exit1 NU1101 for Meziantou.Analyzer,Roslynator.Analyzers,Microsoft.CodeAnalysis.BannedApiAnalyzers from inherited central GlobalPackageReference; initial isolated package copy only included four Godot packages. No native process executed. Implication: complete the isolated copy with these existing pinned caches; do not relax build policy or change test source. Preserve harness-build logs and diagnostic-result-escalated.json.
- timestamp: 2026-11-17T05:38:03Z; checked: variant1 local-cache-retry native; found: PID30580/nonce73581930-ed72-4e0e-ad5e-df97cc528d09/canonical DLL95e78e67 authenticated;7936tuples/16158alltrue/12H13; native exit0/stderr0bytes; build0warnings/0errors and import0. Diagnostic GC pending finalizers16355 before,7 after. All11 checkpoint identities unchanged. Canonical stderr has178ImageTexture+96AtlasTexture+64Image leaked-reference entries. Implication: retiring deferred wrappers before native teardown prevents this observed crash; does not isolate production versus proof ownership or authorize permanent GC workaround. Result: .phase4/04-07-debug/v1-gc-before-quit/diagnostic-result-local-cache-retry.json.
- timestamp: 2026-11-17; checked: variant2 prepared; found: only Nonempty Image lease changed to using. Full harness canonical e196871de3859929d00d32620aa90385c713c2a529852f60cadf01a0c4ec3a0b to14e9a319fb556bdf96a68b4846e69babc39b84c9ab398740b212b58974484fde; nonce02016150-8ba4-4f34-b7be-af458feb8f74; canonical production DLL95e78e67 and11checkpoint identities unchanged. Exact original/prepared39input hashes in .phase4/04-07-debug/v2-nonempty-scope/provenance.json.
- timestamp: 2026-11-17T05:40:42Z; checked: variant2 Nonempty scope only; found: PID5256/nonce02016150-8ba4-4f34-b7be-af458feb8f74/canonical DLL95e78e67; all7936tuples/16158assertions/12H13 pass, actual native exit0, stderr0bytes, fixture strict build0warnings/0errors, import0; all11checkpoint hashes unchanged. Implication: production-resource retirement is not necessary for the observed clean contrast; cannot infer production ownership correctness or guaranteed stability from one pass. Result .phase4/04-07-debug/v2-nonempty-scope/diagnostic-result.json.
- timestamp: 2026-11-17; checked: Nonempty lease volume and pinned local API; found: full canonical results contain13312 fitted sprite Nonempty calls and246 H13 Nonempty calls, total13558 fresh Image leases per full run. Pinned GodotSharp.xml lines253696-253701 specifies Texture2D.GetImage returns a newly created Image copy each call. No base texture or AtlasTexture ownership transfers to Nonempty; the copied Image is solely proof-owned. Implication: using can dispose the copy on early success/false return without affecting native sprite/base texture or independent neighbor.
- timestamp: 2026-11-17; checked: final variant3 prepared provenance; found: manager and parent explicitly authorized exact reversal after unexpected variant2 success. Fresh39copied input files are byte-identical to canonical afbfc1d8, harness e196871de3859929d00d32620aa90385c713c2a529852f60cadf01a0c4ec3a0b (reverse of v2 hash14e9a319), productionDLL95e78e67, nonce239bc06f-cac4-4a72-ad02-2cb5bf79cfff. All11checkpoint identities unchanged; R1 ExactPixels using scopes remain intact. Manifest .phase4/04-07-debug/v3-nonempty-reversal/provenance.json records exact reversal and every input hash. No GC/delay or assertion differences.
- timestamp: 2026-11-17T05:43:34Z; checked: final variant3 exact helper reversal; found: PID27972/nonce239bc06f-cac4-4a72-ad02-2cb5bf79cfff/canonicalDLL95e78e67; all7936tuples/16158true/12H13, actual exit3221225501(0xC000001D),stderr133312bytes. First error is script_bindings nonempty in _instance_binding_free_callback; leaked AtlasTexture/ImageTexture/Image references follow. Strict fixturebuild0warnings/0errors/import0. Implication: reversal reproduces the shutdown failure after unchanged successful functionality; together with v2 it causally localizes the demonstrated repair to Nonempty Image lifetime, rather than a required production change. Result .phase4/04-07-debug/v3-nonempty-reversal/diagnostic-result.json.
- timestamp: 2026-11-17; checked: final preservation and negative-control review; found: all3variants retain all original16158assertions including geometry, mouth, body pixels, invalid widths/rotations/tuples, actual-owning-ALC, logical hands, selected coat/replacement, worker thread rejection, detached callback rejection and12H13neighbor checks. Final11live+backup checkpoint hashes are exact; index empty. No tracked production/case/resource/test/policy edit, commit, archive, source weakening or task/phase completion. Implication: proposed using scope is ownership-local and preserves neighbor/base texture behavior; canonical gate still pending a separately authorized tracked amendment.

## Eliminated

- hypothesis: Missing production behavior or a failing tuple explains these two latest failures. All observations are true; actual failure is process finalization.
- hypothesis: Three ExactPixels proof-owned Image using declarations alone resolve shutdown. afbfc1d8 reproduces the crash; preserve its repair and evidence, no rollback or claim of resolution.
- hypothesis: A production-owned resource change is required before any full-matrix clean shutdown is possible. Variant2 scopes only the proof-owned Nonempty Image, retaining production DLL95e78e67 and all behavior/assertions, and exits0 with empty stderr. This refutes necessity, not the presence of separate unscoped production-resource risks.

## Constraints and authority

Diagnosis only. No tracked production/case/resource/test edit or policy permission. May read all relevant source/evidence and write this session plus ignored `.phase4/04-07-debug/` isolated fixture copies and bounded logs. At most three controlled native diagnostic variants; preserve immutable canonical reports and all assertions in full-matrix probes. Any reduced probe is explicitly diagnostic, never completion evidence. Changes to isolated copies must have exact before/after hashes and one-variable rationale; actual checkout input hashes stay fixed.

No disabling finalizers, accepting nonzero exits, forced success, lost negative controls, arbitrary deadline/output increase, install/update, real game window or unrelated process action. An experiment collecting before shutdown may identify ownership but does not itself authorize a permanent GC/finalizer workaround. Return a root-cause report or exact remaining uncertainty with smallest proposed source/proof files and discriminating follow-up.

Foreign global/spec/Phase06/Phase12 WIP remains untouched/unstaged. Branchfeature/04-asset-production; parent owns commits/PR/global progress. Phase15/37 locally complete;07Task2 remains incomplete; allASSET and WINDOWS29/30/31/actualacceptance pending.

## Resolution

root_cause: FIT_HARNESS.Nonempty in tools/phase4/cases/bodies-a.cjs leaves each helper-owned Texture2D.GetImage copy undisposed (13558calls/fullrun). Deferred GodotObject finalization can overlap pinned native C# script-binding teardown, causing0xC000001D after all assertions pass. Scoped helper-only disposal produces clean exit; exact reversal restores fatal shutdown with identical production DLL and matrix.
confidence: high causal localization via clean helper-only contrast and exact fatal reversal; native interleaving mechanism inferred from observed stack plus pinned engine source, not live thread trace
fix: Not applied (diagnosis-only). Smallest proposed amendment is `using var im=sprite.Texture.GetImage();` inside shared FIT_HARNESS.Nonempty; retain R1ExactPixels scopes. Do not dispose borrowed sprite textures or shared ResourceLoader base textures; no permanent GC workaround.
verification: Diagnostic only. v1GC:7936tuples/16158true/12H13,PID30580,exit0,stderr0; v2using-only:same fullproof,PID5256,exit0,stderr0; v3exactreversal:same fullproof,PID27972,exit0xC000001D,stderr133312bytes. All3fixtures strict0warnings/0errors/import0. Native budget3/3used. Production repair verification/stability and task completion remain pending.
proposed_files: [tools/phase4/cases/bodies-a.cjs]
proposed_regression: Keep existing full bodies.complete-fit matrix and native process exit/stderr gate; preserve all geometry/body/mouth/neighbor/lifetime/negative assertions. Scoped copy disposal must cover both early true and final false return paths; no coverage relaxation.
oracle_type: specified (GetImage creates a fresh Image; process must exit0 without fatal stderr; existing exact pixel/geometry and lifecycle contracts remain)
files_changed: [.planning/debug/phase04-body-shutdown.md]
diagnostic_artifacts: .phase4/04-07-debug/ (ignored isolated copies, manifests, source driver and immutable logs only)
cycles: 3 investigation native variants; 0 tracked fix cycles
specialist_review: none; general maps to engineering:debug, which is absent from the installed skill catalog and filesystem skill inventory
prevention: why not caught before the native gate: no deterministic-disposal check for helper-owned GetImage copies; guard: retained full bodies.complete-fit matrix plus actual native exit and fatal-stderr gate, which correctly caught this failure

## Specialist Review

The debugger returned specialist_hint `general`, which maps to `engineering:debug`. No matching installed skill exists in the available catalog or filesystem skill inventory, so no specialist invocation was possible. The causal source contrast, exact reversal, API ownership evidence, and proposed helper-only repair are preserved for parent review. No production resource retirement or permanent collection workaround is proposed.

## Manager Disposition

Diagnosis-only objective achieved. The parent explicitly retains ownership of tracked repair authorization, commits, archive, and global phase progress. This session is intentionally left at its active path with status `diagnosed` for parent reconciliation; no commit, archive, task completion, or phase completion is claimed. The native diagnostic budget is exhausted at 3/3.
