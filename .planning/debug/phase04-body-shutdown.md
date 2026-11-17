---
status: investigating
trigger: Coordinator-observed native shutdown failure during authorized Phase04-only autonomous execution
created: 2026-11-17
updated: 2026-11-17
goal: find_root_cause_only
---

# Phase04 eight-body shutdown

## Current Focus

hypothesis: Remaining production Image/AtlasTexture/ImageTexture wrappers or fixture-owned script bindings survive until native shutdown; shared pixel-helper disposal alone is insufficient.
next_action: Trace actual production and harness ownership, then use bounded isolated controlled probes to distinguish production lifetime from harness teardown.
test: At most three recorded diagnostic native variants, never production/source mutation or completion credit.
expecting: A reproducible contrast with actual process exits and resource/binding evidence, then a precise proposed ownership repair.

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

## Eliminated

- hypothesis: Missing production behavior or a failing tuple explains these two latest failures. All observations are true; actual failure is process finalization.
- hypothesis: Three ExactPixels proof-owned Image using declarations alone resolve shutdown. afbfc1d8 reproduces the crash; preserve its repair and evidence, no rollback or claim of resolution.

## Constraints and authority

Diagnosis only. No tracked production/case/resource/test edit or policy permission. May read all relevant source/evidence and write this session plus ignored `.phase4/04-07-debug/` isolated fixture copies and bounded logs. At most three controlled native diagnostic variants; preserve immutable canonical reports and all assertions in full-matrix probes. Any reduced probe is explicitly diagnostic, never completion evidence. Changes to isolated copies must have exact before/after hashes and one-variable rationale; actual checkout input hashes stay fixed.

No disabling finalizers, accepting nonzero exits, forced success, lost negative controls, arbitrary deadline/output increase, install/update, real game window or unrelated process action. An experiment collecting before shutdown may identify ownership but does not itself authorize a permanent GC/finalizer workaround. Return a root-cause report or exact remaining uncertainty with smallest proposed source/proof files and discriminating follow-up.

Foreign global/spec/Phase06/Phase12 WIP remains untouched/unstaged. Branchfeature/04-asset-production; parent owns commits/PR/global progress. Phase15/37 locally complete;07Task2 remains incomplete; allASSET and WINDOWS29/30/31/actualacceptance pending.

## Resolution

root_cause: pending
fix: not authorized
verification: pending
files_changed: none
