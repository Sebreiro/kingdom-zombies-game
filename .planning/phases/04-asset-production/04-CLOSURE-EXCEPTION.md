---
phase: "04"
date: "2026-11-22"
authority: creator_instruction
status: accepted
scope: phase_4_only
---

# Phase 4 — Asset closure exception

The creator closed Phase 4 on the source and headless evidence, without the rendered run, on 2026-11-22:

> I think it is enough. this phase is only about assets.
> They are generated and somehow tested.
> We don't need to test real clicks and so on in this phase.
>
> Consider this steps finished.

Earlier the same day the creator accepted the passing reruns of `world.replacement` after one intermittent engine error (“Omit this fail and finish with phase 4”).

This exception applies to Phase 4 only. It supersedes the [focused Windows handoff](04-HANDOFF.md) steps 4 and 5 and Plans [04-32](04-32-PLAN.md) and [04-33](04-33-PLAN.md). The local testing policy, the saved scenarios and every check, pin and validator remain in force for later phases. No source, case, validator or pin changed for this closure.

## What Phase 4 closes on

The reviewed source is revision `5c637da23437ea09501a9b167449425a824fe1c1` (source `29578ddb…`, media `056324ea…`, tool `081ee3aa…`), bound in `1e0fe27`. The [04-24 summary](04-24-SUMMARY.md) has the full record.

- **Assets.** All 180 resource obligations have source-ready production outputs with provenance, and the inspection hub reaches 180 of 180 entries.
- **Independent review.** The whole-phase review is clean, and all original and renewed findings are resolved.
- **Source and headless checks.** All 776 selected pre-window results pass on the reviewed source:
  - 71 Phase 4 cases and the `acceptance.judgments` control;
  - 678 Phase 1, 7 Phase 2 and 20 Phase 3 inherited checks.
- **Gates.** `runner.prelaunch` and `runner.selection` pass at `1e0fe27`.
- **Portable build.** The Windows inspection package passes `export.portability`. That covers a clean headless launch, close and relaunch, a visible missing-resource diagnostic and continued access to a valid sibling.

## Not performed and not credited

None of the following is recorded as passed. Their saved procedures stay in the [scenario catalog](04-SCENARIOS.md) and the [case ownership register](../../../tools/phase4/handoff.json).

| Item | Scope | New owner |
| --- | --- | --- |
| Rendered Phase 4 cases | 18 IDs in real 1280×720 windows with physical input. G1-hub: `heldout.H01`, `heldout.H12`, `heldout.H13`, `heldout.H15`, `heldout.H21`, `UIH.HUB-NAV.error`, `UIH.HUB-REGISTER.partial`, `UIH.CHARACTER-PREVIEW.populated`, `UIH.WORLD-ITEM-PREVIEW.populated`, `UIH.UI-TEXT-STATUS.long-text`, `UIH.AUDIO-AUDITION.populated`, `UIH.UI-FIXTURE-PREVIEW.populated`, `rendered.hub-navigation`, `rendered.audio-audition`. G2-packaged: `heldout.H27`, `heldout.H28`, `heldout.H31`, `rendered.packaged-relaunch` | Phase 5 |
| Inherited rendered regressions | 9 Phase 2 (G3-solo) and 12 Phase 3 (G3-lan) rendered cases selected as affected | Phase 5 |
| Named judgments | P1–P4 for AC-21 to AC-24: restrained blood and no dismemberment, honest origin and status, KingdomZombies identity, truthful mechanic presentation | Phase 5, on its rendered images |
| Named listening | 95 verdicts: 88 samples, 5 loops, and the live D-31 and D-32 mixes | Phase 9, with audio settings |
| Completion reconciliation | `04-COMPLETION.json`, `--verify-completion` and the final `acceptance.artifacts` pass over rendered results | Phase 5 imports the open IDs; no completion index is written |
| Driver declaration | `04-RUNS.json` with calibrated physical drivers; never authored | Phase 5 |

Each new owner imports its exact IDs and original procedures into its own selection, as the handoff already requires for the 89 later-phase IDs.

## Known issue carried forward

`world.replacement` failed once on `5c637da` (`.phase4/runs/88bdbc40-e7a8-407d-b375-ae018140847b`). The Godot .NET bridge logged `InvalidOperationException: Handle is not initialized` from `ScriptManagerBridge.SwapGCHandleForType` while `ResourceLoader.Load<PackedScene>` loaded an item preview scene at `src/godot-game/inspection/WorldPreview.cs:769`. Five reruns passed.

The suspected cause is a race between the .NET finalizer and the main thread. The managed wrapper for the cached preview scene is a discarded local, so the finalizer can free its handle while the main thread revives the same native object.

Phase 5 owns the follow-up:
- keep both item preview scenes in fields;
- record the defect in `docs/workflow/engine-defects.md`;
- apply the generated-span governance rebind that the change requires.

## Requirements

ASSET-01 to ASSET-05 are complete under this exception, on the source and headless evidence above. Rendered presentation, physical input, listening and the four judgments stay open under the owners above; no later label may treat them as passed in Phase 4.
