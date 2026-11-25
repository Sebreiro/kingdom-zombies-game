# Roadmap: KingdomZombies

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. Historical evidence limits remain explicit; the current local strategy below supersedes the earlier VM rule.

## Overview

Deliver the complete specified version 0.1: players build, equip, and defend one shared survivor camp through a solo or four-player LAN campaign, preserving their survivors, belongings, and progress. The creator approved the original 133 requirements, working-increment direction and ten-phase roadmap on 2026-11-13; TEST-11 extended this to 134 requirements and eleven phases. On 2026-11-16 the creator deferred stash/inventory filtering to Phase 12, splitting it from INV-10 into INV-14 for a current total of 135 requirements and twelve phases. Phases 1–3 are complete, including the playable menu/walking scene and four-player LAN foundation. Phase 1 retains its focused closure exception and disclosed follow-ups.

**Planning mode:** `mvp` means working, verifiable increments toward the full approved game; it does not reduce scope.
**Granularity:** Standard configuration, with creator-requested Phase 11 on 2026-11-15 and Phase 12 on 2026-11-16 extending the original ten-phase direction.
**Scope authority:** [PROJECT.md](PROJECT.md), [REQUIREMENTS.md](REQUIREMENTS.md), the [game specification](../specs/0.1/initial-spec.md), and the [asset/resource plan](../specs/0.1/assets-and-resources.md). [Research](research/SUMMARY.md) informs proofs and implementation choices; it retains initial candidate history; Phase 1 records the now-pinned and proven development baseline.
**Approval:** Requirements, working-increment direction, and detailed roadmap approved by the creator on 2026-11-13.

**Test-scope amendment, approved 2026-11-23:** Phase 5 is reduced to one shared runner and catalog, three test tiers, basic Windows/Linux PR checks and the existing solo/LAN rendered scenarios on the shared runner. Measured coverage with the eight transferred certification IDs, full hosted Windows/Linux runs, evidence reuse validation, exhaustive runner trials, the other 17 Phase 4 rendered cases and the Phase 1 follow-ups move to Phase 11 under the new TEST-12, with no pass credit and unchanged thresholds. Five rules change in the [local testing strategy](../docs/workflow/local-testing.md) for Phases 6–10: a budgeted Critical smoke set, input-triggered Phase 1 rule-suite groups, edge cases as unit tests carrying their IDs, no whole-selection rerun per plan, and commit-based evidence. TEST-12 raises the scope to 136 requirements; TEST-01–TEST-10 keep their IDs, statements and Phase 5 ownership. Phase 10 release gates count the cases active in Phases 1–10; TEST-11 and TEST-12 cases stay pending until Phase 11, as INV-14 does until Phase 12. The Phase 7 plans predate this amendment and need a revision pass before execution. See the amended [Phase 5 specification](phases/05-ci-and-repeatable-game-testing/05-SPEC.md) and [context](phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md). This paragraph supersedes conflicting Phase 5 ownership wording elsewhere in this roadmap.

**Current local testing amendment, approved 2026-11-15:** Apply [local testing strategy](../docs/workflow/local-testing.md) in every phase immediately: implement the whole phase, review/fix code, then grouped development-OS-only 1280×720 game runs without a VM. Always select active Critical cases plus new/changed cases and affected regressions. Collect failures, batch repairs, check failures, then rerun the entire selected set on the repaired build. Every phase saves reusable scenarios with importance and ordered run groups. Phase 11 adds nightly full tests under TEST-11; original requirement ownership, hosted compatibility and distinct release evidence remain explicit.

**Scope timing amendment, approved 2026-11-14:** Keep Phase 1's working rules, tests and clean two-platform baseline. Phase 5 now owns measured coverage, exhaustive platform execution/evidence reconciliation and unfinished aggregate runner certification. The ten-phase order, all 133 requirement IDs and their primary owners are unchanged; the [Phase 5 handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) records the required certification work.

## Milestones

- 🚧 **v0.1 Complete playable LAN campaign** — Phases 1–3 complete; Phase 4 in progress; Phases 5–12 pending. Phase 11 added 2026-11-15; Phase 12 filtering added 2026-11-16.

## v0.1 — Complete playable LAN campaign

**Milestone goal:** Deliver the approved solo/four-player campaign and release evidence, establish sustainable nightly full regression testing, then complete the deferred stash/inventory filtering across twelve increments.

## Phases

Integer phases execute in order. Decimal phases are reserved for explicitly inserted work.

- [x] **Phase 1: Code rules and documentation** - Developers can use strict, proven Godot-compatible C# rules and focused documentation. (completed 2026-11-14)
- [x] **Phase 2: Playable foundation** - Players launch a menu, enter a small scene, and move one character left and right. (completed 2026-11-15)
- [x] **Phase 3: Four-player LAN foundation** - A host and three guests independently control synchronized characters in the same scene. (completed 2026-11-16)
- [ ] **Phase 4: Asset production** - The complete required resource pack is reusable, self-contained, and inspectable in fixtures.
- [x] **Phase 5: CI and repeatable game testing** - Developers and AI agents run stored cases locally and in CI with trustworthy evidence.
- [ ] **Phase 6: Survivors, inventory, and combat** - Players manage real belongings, fight, die, and continue through ordinary-survivor succession.
- [ ] **Phase 7: Camp economy and development** - Players recruit, equip, feed, employ, and expand a functioning camp.
- [ ] **Phase 8: Zombie campaign and outcomes** - Players face the full zombie campaign and assault six portals through victory or defeat.
- [ ] **Phase 9: Persistence and complete player experience** - Players save, resume, reconnect, configure, and use the complete game reliably.
- [ ] **Phase 10: Release validation and balancing** - Complete campaigns and measured desktop, LAN, performance, endurance, and pacing evidence establish release readiness.
- [ ] **Phase 11: Nightly full regression testing**
- [ ] **Phase 12: Stash and inventory filtering** - Add the filtering explicitly deferred from earlier gameplay increments.
- [ ] **Phase 12: Stash and inventory filtering**

## Delivery and Ownership Rules

Each requirement has exactly one primary owner in a phase's **Requirements** field and in the requirements traceability table. That owner delivers and verifies its complete approved behavior. Prerequisite work in earlier phases and regression/integration reruns in later phases do not create additional owners or mark a requirement complete early. Nineteen of the current 136 approved requirements are complete; the traceability table records each owner and status. PLAT-01 and PLAT-02 are outside this milestone.

**Filtering timing amendment, creator instruction 2026-11-16:** No stash or inventory filtering is delivered in Phases 6–11. Phase 12 owns INV-14; INV-10 continues to require unlimited pages, local access, navigation, placement and transfers in Phase 6. This overrides earlier complete-interface/release wording only for filtering. Retain planned filtering cases without pass credit until Phase 12; its integration and affected release regressions must pass before final filtering/full-scope acceptance.

Every feature includes its stable identities, host-authoritative validation and atomic ownership changes, configuration and field documentation, versioned saveable state and relevant legacy metadata, usable input/UI/feedback, and regression evidence. Snapshot capture/restore proofs accompany new state before the full player-facing save experience. Solo, host-local, and guest actions follow the same authority path. A failed or duplicated operation cannot change possessions, payments, or commitments.

| Boundary | Required timing |
| --- | --- |
| Rule and foundation checks | Each phase uses development-OS checks during implementation and complete current phase-scope testing on that OS and a local VM running the other OS at phase end. Phase 1 runs the complete declared selection on Windows and Linux. Under the 2026-11-23 amendment Phase 11 owns measured 90%/80% coverage, advanced platform/report reconciliation and the eight unfinished aggregate/coverage declarations. Phases 2 and 3 retain their launch, input, rendered-scene and connection checks before the comprehensive framework. |
| Resource production versus integration | Phase 4 finishes all 180 resource obligations and content/combination fixtures, including data, text, audio bindings, and presentation metadata. Phases 6–9 connect those outputs to real gameplay, session, and save behavior; Phase 10 proves the integrated workload. |
| Full catalog versus implemented cases | Phase 5 records A-01–A-47 and additional rule/foundation/content/regression cases, and requires cases for implemented foundations. Future feature cases are explicitly planned with owners, then activated during implementation. Planned cases never count as passed. |
| Early world services | At the start of Phase 6, authoritative elapsed time, safety/danger, and reachability support recovery and access. At the start of Phase 7, day/night transitions, once-only resource-event identities, authored work areas/portal barriers, and live border/fallback rules support actual camp work. Full campaign scheduling remains Phase 8. |
| Feature UI and exploration | Each increment includes its own usable UI, focus handling, feedback, and applicable content/audio. Independent cameras start with controlled actors; shared exploration/minimap works with the authored campaign in Phase 8. Phase 9 completes every interface and verifies map preservation through real save/load/late join. |
| Persistence, sessions, and configuration | Stable player/run/content/item identities begin in the foundations. Feature snapshots, commitments, validation, and legacy inventory representation develop in Phases 6–8. Phase 9 owns full SESSION, SAVE, DATA, and UX acceptance plus complete dawn/save/join and continuous-play integration. |
| Evidence environments | Rule/headless checks, rendered production-input checks, four local processes, actual mixed-OS LAN, creator-PC rendering, and real-duration endurance prove different things. Local process tests or hosted CI cannot replace physical LAN or reference-hardware evidence. |

## Phase Details

### Phase 1: Code rules and documentation

**Goal:** Developers can build and extend the Godot game under strict, documented C# rules that permit correct engine integration and demonstrably reject violations.
**Mode:** mvp
**Depends on:** Nothing (first phase)
**Requirements:** RULE-01, RULE-02, RULE-03, RULE-04, RULE-05, RULE-06, RULE-07
**Success Criteria** (what must be TRUE):

1. A developer can reproduce a pinned engine/SDK/compiler/analyzer build and run a representative Godot integration scene through construction, loading, callbacks, and teardown. The recorded proof covers handwritten/generated partials, exports, signals, nullability, and main-thread access.
2. A developer can find every inherited LangProj rule classified as retained, adapted, or inapplicable, with a reason and narrowly scoped compatibility decision; normal Godot code passes without blanket suppression.
3. Running the documented checks accepts compliant examples and rejects deliberate code, architecture, and exemption violations with actionable diagnostics.
4. A developer or agent can navigate a short root `AGENTS.md` and focused indexes with valid, consistently cased links, and locate enforced gameplay/Godot/service boundaries plus rules for time, randomness, identities, and saveable authoritative state.

**Earlier completion boundary (superseded for Phase 1 by the focused closure exception above):** Retain every working compiler/analyzer, boundary, exemption, runner, documentation, state and lifecycle check. Use focused development-OS feedback while repairing related findings, then clean setup and the complete declared Phase 1 scope on Windows and the local Linux VM for the same final implementation. Use hardware acceleration when available. Measured coverage, advanced certification/report reconciliation and the eight explicit pending declarations retain Phase 5 ownership and receive no pass/skip credit now. Separate actual qualitative review remains required. The revised [specification](phases/01-code-rules-and-documentation/01-SPEC.md) governs the final plan.

**Plans:** 18/18 plans complete; current amended goal verified and phase closed on 2026-11-14.
**Wave 1**
- [x] 01-01-PLAN.md — Pinned native-host toolchain proof

**Wave 2** *(blocked on Wave 1 completion)*
- [x] 01-02-PLAN.md — Trustworthy required-case results

**Wave 3** *(blocked on Wave 2 completion)*
- [x] 01-03-PLAN.md — Complete donor capture and rule register

**Wave 4** *(blocked on Wave 3 completion)*
- [x] 01-04-PLAN.md — Minimal projects and strict configuration

**Wave 5** *(blocked on Wave 4 completion)*
- [x] 01-18-PLAN.md — Compiler verifier and evaluated configuration proofs

**Wave 6** *(blocked on Wave 5 completion)*
- [x] 01-05-PLAN.md — Construction and call-site analyzers

**Wave 7** *(blocked on Wave 6 completion)*
- [x] 01-06-PLAN.md — Null, size, length, and member-order analyzers

**Wave 8** *(blocked on Wave 7 completion)*
- [x] 01-07-PLAN.md — Continuation and declaration analyzers

**Wave 9** *(blocked on Wave 8 completion)*
- [x] 01-08-PLAN.md — Actual build and exemption proofs

**Wave 10** *(blocked on Wave 9 completion)*
- [x] 01-09-PLAN.md — Evaluated dependency and type boundaries

**Wave 11** *(blocked on Wave 10 completion)*
- [x] 01-10-PLAN.md — Controlled time, randomness, and identity

**Wave 12** *(blocked on Wave 11 completion)*
- [x] 01-11-PLAN.md — Strict snapshots and atomic restoration

**Wave 13** *(blocked on Wave 12 completion)*
- [x] 01-12-PLAN.md — Real engine lifecycle and thread boundary

**Wave 14** *(blocked on Wave 13 completion)*
- [x] 01-13-PLAN.md — Canonical guide and documentation checking

**Wave 15** *(blocked on Wave 14 completion)*
- [x] 01-14-PLAN.md — Project references and rule bindings

**Wave 16** *(blocked on Wave 15 completion)*
- [x] 01-15-PLAN.md — Explicit Phase 1 selection and Phase 5 certification handoff

**Wave 17** *(blocked on Wave 16 completion)*
- [x] 01-16-PLAN.md — Final clean Windows baseline and Linux preparation

**Wave 18** *(blocked on Wave 17 completion)*
- [x] 01-17-PLAN.md — Representative clean Linux checks and Phase 1 review

**Cross-cutting constraints:** <!-- gsd:generated -->
- A developer receives actionable inherited analyzer failures while compliant code is accepted (AC-07).
- Applicable numeric and syntax boundaries preserve the donor's exact semantics (AC-09).

**UI hint:** no

**Delivery boundary:** Preserve the adopted LangProj conventions, analyzers, exact numeric semantics, bounded Godot permissions and five-project boundaries. Keep the selected toolchain pins and canonical `AGENTS.md`; no working checks are removed or weakened. Plans 01-15–17 separate explicit Phase 5 certification obligations from Phase 1's functional evidence. Final acceptance uses the retained Windows baseline plus actual focused Linux evidence and the targeted both-OS amendment under the creator exception; the original failed graph case, full Linux suite and individual human reviews remain explicit follow-ups. Reuse completed project/reference/decision pages rather than creating parallel documentation. Coverage and exhaustive certification are required in the [Phase 5 handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md).

### Phase 2: Playable foundation

**Goal:** As a player, I want to start a small scene from a visible menu and move my character left and right using normal input, so that I can explore the game's playable foundation.
**Mode:** mvp
**Depends on:** Phase 1
**Requirements:** BASE-01, BASE-02, BASE-03
**Success Criteria** (what must be TRUE):

1. Launching the game displays a usable main menu, and choosing Start enters the small playable scene.
2. The player sees representative base assets and a character responding to left/right input through the production controls and camera.
3. A developer can repeat launch, menu selection, movement, and rendered-scene smoke checks, inspect current results, and see failure when an expected behavior or required image is missing.

**Plans:** 3/3 plans complete; verified and closed 2026-11-15.
**UI hint:** yes

**Delivery boundary:** Keep this scene as the Phase 3 LAN foundation. Establish actor identity, input/focus routing, canonical local commands, and engine lifecycle checks here. Use a few representative resources; full pack acceptance stays in Phase 4. These small checks exist before Phase 5 automation and do not imply a complete campaign.

### Phase 3: Four-player LAN foundation

**Goal:** As a LAN player, I want to host or join a foundation scene for one host and three guests and independently control my character, so that all four players observe consistent authoritative movement.
**Mode:** mvp
**Depends on:** Phase 2
**Requirements:** LAN-01, LAN-02, LAN-03, LAN-04
**Success Criteria** (what must be TRUE):

1. A host and up to three guest game processes connect to the same scene with distinct player profiles and distinct controlled characters.
2. Each participant's normal movement input affects only their own character, while every participant observes consistent resulting positions through their own camera.
3. Solo, host-local, and guest actions produce one authoritative result per accepted action; repeated input/command delivery cannot execute the host action twice.
4. A developer can repeat the connection/movement smoke checks and substitute the discovery, identity, or connection adapter without changing gameplay rules; player identity survives independently of a LAN address or transient peer number.

**Plans:** 4/4 plans complete
**UI hint:** yes

Plans:
**Wave 1**
- [x] 03-01-PLAN.md — Prove the production host/manual-join ENet tracer and exact permissions without game windows.

**Wave 2** *(blocked on Wave 1 completion)*
- [x] 03-02-PLAN.md — Complete four-participant admission, ordering, timeout and session-lifetime behavior.

**Wave 3** *(blocked on Wave 2 completion)*
- [x] 03-03-PLAN.md — Complete discovery, stable profiles, native controls, independent cameras and service substitutions.

**Wave 4** *(blocked on Wave 3 completion)*
- [x] 03-04-PLAN.md — Deliver strict per-peer evidence, review/fix the whole phase and run grouped selected scenarios.

**Delivery boundary:** Provide the minimal host/join controls and feedback needed for the increment. Establish revision/baseline, readiness, disconnect cleanup, and session lifetime contracts, while full discovery metadata, late-join recovery, reconnection, and host-loss/save behavior remain owned by Phase 9. Four local processes are a repeatable network proof; mixed-OS LAN on separate computers remains required release evidence.

### Phase 4: Asset production

Include the rendered case IDs assigned to Phase ### Phase 4: Asset production in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

**Goal:** Developers and players can inspect the complete required game content in a self-contained Godot resource pack, with reusable recovered material prioritized and every output traceable.
**Mode:** mvp
**Depends on:** Phase 3
**Requirements:** ASSET-01, ASSET-02, ASSET-03, ASSET-04, ASSET-05
**Success Criteria** (what must be TRUE):

1. The production manifest maps all 180 CH/AN/IT/BL/EN/FX/UI/AU/DT/TXT obligations to actual outputs or explicit shared outputs. Selected donors and reused logic retain preserved-original identities, transformations, versions, stable output IDs, and validation evidence.
2. Content fixtures show the survivor and all seven zombie families with required equipment, both hands/directions/actions, armor and backpack fits, bow/arrow states, and persistent corpse appearances; all 24 item definitions are identifiable.
3. Fixtures show all main/wall/tower tiers and lifecycle states, standalone guard counts of 1/2/3, exactly two one-guard main attachments, permanent portal states, and the authored map/environment/wildlife/effect resources.
4. A developer can inspect every required UI/text/data resource and error/legacy layout, test longer strings and supported scales, and audition all required sound/ambience categories through documented event bindings.
5. The packaged content and inspection scenes import and run without the recovered-material folder or other absolute development paths; missing references and unfitted combinations are visible failures.

**Plans:** 35/37 plans executed; 04-32 and 04-33 waived under the [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md)

Phase 4 is complete under the creator's [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md) of 2026-11-22. All 180 obligations are source-ready, the reviewed source `5c637da` has a clean independent review, and all 776 selected pre-window results pass on it ([04-24 summary](phases/04-asset-production/04-24-SUMMARY.md)). The rendered run, the four named judgments and named listening were not performed; Phase 5 owns the rendered cases and judgments and Phase 9 owns listening.

Plans:
**Wave 1**
- [x] 04-01-PLAN.md — Production donor-to-inspector tracer and exact provenance

**Wave 2** *(blocked on Wave 1 completion)*
- [x] 04-25-PLAN.md — Exact census/provenance and atomic repeatable publication

**Wave 3** *(blocked on Wave 2 completion)*
- [x] 04-34-PLAN.md — Exact SDK export-layout prerequisite

**Wave 4** *(blocked on Wave 3 completion)*
- [x] 04-35-PLAN.md — Permanent inspection export-feature profile

**Wave 5** *(blocked on Wave 4 completion)*
- [x] 04-02-PLAN.md — Early portable-export and audio feasibility

**Wave 6** *(blocked on Wave 5 completion)*
- [x] 04-36-PLAN.md — Discoverable saved cases and portable independent expectations

**Wave 7** *(blocked on Wave 6 completion)*
- [x] 04-37-PLAN.md — Exact frozen inspection generated-authentication prerequisite

**Wave 8** *(blocked on Wave 7 completion)*
- [x] 04-03-PLAN.md — Live inspection shell, validated loader and exact native permissions

**Wave 9** *(blocked on Wave 8 completion)*
- [x] 04-26-PLAN.md — Inspection input/lifetime expansion with immediate exact native permission proofs

**Wave 10** *(blocked on Wave 9 completion)*
- [x] 04-04-PLAN.md — Reusable donor kit, exact animation conversion and assembled scale proof

**Wave 11** *(blocked on Wave 10 completion)*
- [x] 04-05-PLAN.md — All 24 item identities and reusable wearable/held masters
- [x] 04-11-PLAN.md — Three actual main-building tiers and shared camp resources

**Wave 12** *(blocked on Wave 11 completion)*
- [x] 04-06-PLAN.md — Survivor and first three zombie body fits
- [x] 04-14-PLAN.md — Native UI kit, packaged fonts and reusable theme
- [x] 04-18-PLAN.md — Complete run, time, survivor, item, combat and inventory baseline

**Wave 13** *(blocked on Wave 12 completion)*
- [x] 04-07-PLAN.md — Complete armored, wide and specialist zombie silhouettes/fits
- [x] 04-28-PLAN.md — Canonical status/input/player symbols and retained desktop identity art

**Wave 14** *(blocked on Wave 13 completion)*
- [x] 04-08-PLAN.md — Concurrent locomotion, melee and coherent logical hands

**Wave 15** *(blocked on Wave 14 completion)*
- [x] 04-09-PLAN.md — Bow, work, food and specialist action composition

**Wave 16** *(blocked on Wave 15 completion)*
- [x] 04-10-PLAN.md — Persistent corpses, wildlife, identities and real crowd resources

**Wave 17** *(blocked on Wave 16 completion)*
- [x] 04-12-PLAN.md — Walls, independent guards and complete defense lifecycle
- [x] 04-15-PLAN.md — Reusable inventory, legacy, stash and target layout states

**Wave 18** *(blocked on Wave 17 completion)*
- [x] 04-13-PLAN.md — Complete authored world, portal and environmental effect inspection

**Wave 19** *(blocked on Wave 18 completion)*
- [x] 04-27-PLAN.md — World/item navigation, replacement and held-out preview states

**Wave 20** *(blocked on Wave 19 completion)*
- [x] 04-16-PLAN.md — People, economy, construction, map, rally and warning layouts
- [x] 04-19-PLAN.md — Complete economy, food, map, structure and tower baseline

**Wave 21** *(blocked on Wave 20 completion)*
- [x] 04-17-PLAN.md — Session, save, outcome, settings and error layout resources

**Wave 22** *(blocked on Wave 21 completion)*
- [x] 04-20-PLAN.md — Complete zombies, progression, portals, sessions and preferences

**Wave 23** *(blocked on Wave 22 completion)*
- [x] 04-21-PLAN.md — All 30 real sound categories and complete event-bank resources

**Wave 24** *(blocked on Wave 23 completion)*
- [x] 04-29-PLAN.md — Native audio audition, exact current-loop ownership and backstops

**Wave 25** *(blocked on Wave 24 completion)*
- [x] 04-22-PLAN.md — Complete English text, compatibility and data/text inspection

**Wave 26** *(blocked on Wave 25 completion)*
- [x] 04-23-PLAN.md — Complete source census and final exact native-policy reconciliation

**Wave 27** *(blocked on Wave 26 completion)*
- [x] 04-30-PLAN.md — Saved scenario/evidence runner and prelaunch/acceptance validation code

**Wave 28** *(blocked on Wave 27 completion)*
- [x] 04-31-PLAN.md — Complete portable Windows inspection export and clean failure/relaunch proof

**Wave 29** *(blocked on Wave 28 completion)*
- [x] 04-24-PLAN.md — Independent whole-phase review and exact prelaunch selection

**Wave 30** *(blocked on Wave 29 completion)*
- [x] 04-32-PLAN.md — Actual selected native/listening run and separate named judgments (waived, not executed)

**Wave 31** *(blocked on Wave 30 completion)*
- [x] 04-33-PLAN.md — Exact acceptance/artifact/evidence completion reconciliation (waived, not executed)

**Cross-cutting constraints:** <!-- gsd:generated -->
- AC-02 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-03 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-16 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-20 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-08: Use Godot's built-in animation tools and project code only, with no paid animation dependency. Research and planning should solve composition within that boundary rather than schedule a paid-add-on comparison or purchase.
- D-25: Use one Windows inspection hub organized by category, with common navigation for characters, items, buildings/world, UI, data/text and audio. The complete obligation register and exact resource failures remain accessible under the specification.
- D-28: Keep each preview's resource ID, name and status visible in a compact summary. Expand provenance, dimensions, timing and other metadata when needed. Missing/invalid-resource errors remain visible rather than being hidden in collapsed details.
- AC-12 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-15 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-17 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-31: Sound effects are punchier and more game-like: sharper swings, stronger impacts and more noticeable confirmations. This is an explicit selection over the proposed restrained-audio recommendation; the minimal visual-effects preference does not make the audio equally restrained. Suitable recovered recordings still come first and require audition/adaptation.
- D-32: Keep forest, wind and fire ambience subdued so action sounds and important cues stand out. Include the required day/night/local ambience and audition controls; exact gains and mix parameters remain configurable production choices.
- AC-04 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-05 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-08 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-13: Favor the recovered sprites' simpler, low-resolution logical detail. Simplify the larger zombie/base examples to match, rather than raising the whole pack to their richer detail. Exact pixel density and dimensions remain a research/production choice established through the required assembled survivor/zombie/wall/tower/base scale test.
- D-14: Use natural days and atmospheric nights: earthy scenery and warm wood in daylight, cooler/darker nights and restrained warm firelight. Required equipment, identities and states remain readable after dark.
- D-15: Keep character heads, hands and equipment close to natural proportions. Use pose and contrast for readability rather than noticeably enlarged heads/hands/items.
- D-16: Buildings and defenses look like practical camp construction: rough timber, straightforward bracing and functional reinforcement, with clear tier silhouettes. Preserve the specified wood/reinforced-wood/metal progression.
- D-17: Backgrounds are layered but unobtrusive. Distant scenery and parallax provide depth while quiet space keeps characters, equipment and sites clear.
- D-18: For normal unselected characters, strengthen dark outer pixel edges selectively where needed while keeping donor-style shading. Do not apply a uniform outline by default.
- D-21: Zombie bodies share related muted undead skin tones, with subtle color differences supporting their required distinct silhouettes. Equipped items retain their actual appearance.
- D-22: Start portal colors from suitable recovered donor colors. Spit and cosmetic acid use separate restrained sickly colors. No exact hue or numeric palette was selected; cosmetic acid remains non-damaging.
- D-23: Use small, quiet pixel contact-shadow shapes beneath characters and small objects to establish their contact with the ground.
- AC-01 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-06 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-03: Clothing, belts and bags follow their attachment points closely, with little independent sway or bounce. This selects mostly fixed gear rather than the proposed secondary-motion treatment.
- D-11: Clothing, armor and bags have a practical scavenged fit: slightly loose, utilitarian and modestly bulky, fitted to each required body. Slightly loose shapes do not imply the independent sway/bounce rejected in D-03.
- D-20: Provide one canonical inventory icon per item, without separate durability-level icon variants. The creator did not choose one of the proposed icon-art treatments and does not want that detail expanded now. Exact durability indicators remain a planning detail within the required layouts/state examples; this does not remove item identities or required broken/condition presentation, nor the separate worn/held/corpse forms.
- AC-18 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-01: Use deliberate pixel-art poses for walking and attacks, with flexible arm/weapon movement where aiming needs it. Keep crisp pixel edges and the required concurrent walking/combat presentation; do not make full baked sheets for every equipment/action combination.
- D-06: Death uses a short authored collapse suited to each body, ending in a stable, readable equipped corpse pose. A variable physical-collapse treatment was not selected.
- D-02: Melee attacks have clear weight and follow-through. Shoulders and torso support the strike; heavier bodies and weapons feel weighty, while quick attacks remain sharp. Walking continues.
- AC-19 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-04: Melee effects are very minimal. Let the swing and target reaction carry the action; use only the small contact effects needed for clarity. The restrained-blood/no-dismemberment limit remains binding.
- D-07: Hits cause a small, clear shoulder/torso flinch. This is visual feedback only and adds no stun, knockback or action delay.
- D-09: Direction changes switch facing immediately and continue the current stride/action coherently. Preserve logical left/right hand identity and actual equipment.
- D-27: Include play/pause, playback-speed and single-step controls to examine grips, layering, poses and timing. These operate the inspection preview; they do not change the active-game menu's no-pause rule.
- AC-24 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-12: Gathering, building, carrying and fireside rest use restrained, recognizable task poses with only the body movement each activity needs.
- AC-07 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-09 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-14 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-24: Selected corpses, loot piles and other inspectable objects use a steady thin contrasting pixel outline and modest marker. This defines selected-state presentation; it does not implement later gameplay interactions.
- D-19: Standing damaged structures show localized material damage: cracks, splintered timber, bent reinforcement and small damaged areas, increasing with severity. Keep tier, material and functional state clear.
- AC-21 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-10 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-11 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- D-29: Use suitable recovered wooden buttons/borders as frames around simple dark, quiet panel interiors. Keep text, numbers and item grids clear.
- D-30: Use a clear sans-serif face for most body text and numbers, with pixel-style type for selected headings/accents. Exact font files, sizes and fallbacks remain subject to extraction/comparison and the specified readability/scaling/expanded-text checks.
- AC-13 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-23 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.
- AC-22 is implemented by this slice and retains explicit source/headless versus final actual evidence ownership.

**UI hint:** yes

**Delivery boundary:** Maximize suitable reuse from `D:/Work/Games/KindomCrown/recovered`, preserve originals, and adapt/generate remaining gaps. Catalog-only or decoded-metadata evidence is not production completion. Include the full baseline data, English text, provenance, save/content compatibility metadata, crowd/persistent-corpse fixtures, and desktop delivery resources required by the asset plan. Resource/state fixtures complete production acceptance here; later phases must verify actual gameplay event bindings, ownership-driven appearances, save/join reconstruction, and measured release performance. The 180 obligations are not a count of separate files.

### Phase 5: CI and repeatable game testing

Include the rendered case IDs assigned to Phase 5 in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

The Phase 4 [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md) assigned this phase the 18 rendered Phase 4 IDs due at the asset handoff, the 21 affected inherited rendered regressions, the four named judgments P1–P4, the physical driver declaration and the `world.replacement` engine-race follow-up. Under the 2026-11-23 amendment Phase 5 keeps the 21 inherited regressions, one inspection-hub smoke scenario with the four judgments, and the engine-race follow-up. The other 17 rendered IDs and the driver declaration move to Phase 11.

**Goal:** Developers and AI agents can run every delivered check through one shared runner and catalog, in three tiers sized for the edit loop, the plan and the phase end, with basic Windows/Linux pull-request checks and readable failure reports. Later phases add tests and scenario rows, not test infrastructure.
**Mode:** mvp
**Depends on:** Phase 4
**Requirements:** TEST-01, TEST-02, TEST-03, TEST-04, TEST-05, TEST-06, TEST-07, TEST-08, TEST-09, TEST-10
**Success Criteria** (what must be TRUE):

1. An agent can find one readable repository catalog covering A-01–A-47, the delivered solo, LAN and inspection scenarios, the existing Phase 1–4 suites and planned later cases, each linked to requirements, owning phase, importance, tags, tier and ordered game-run group. Existing phase scenarios are integrated; future cases remain planned.
2. One documented entry point runs three tiers: pure tests in one process, headless scenarios with one launch per group, and rendered solo and host-plus-three-guests groups with independent profiles, bounded waits, isolated logs and cleanup. The delivered Phase 2 and Phase 3 rendered scenarios pass through it at 1280×720 on the development OS, and a later phase adds tests and catalog rows without adding a runner.
3. Windows and Linux pull requests run the strict build with analyzers, boundary and documentation checks, the pure tests and one headless game scenario in about ten minutes per OS, shown by an actual run on each.
4. An agent can follow the documented select/run/inspect/reproduce/fix/rerun procedure using each run's report, failure brief, replay command, logs, state observations and required rendered images. Deliberate missing-dependency, zero-discovery, skipped-required-case, crash, timeout, missing-peer and missing-evidence trials fail visibly; a planned feature case cannot manufacture a pass or hide a currently required case.
5. The reduced phase-end selection always includes the budgeted Critical smoke set, all new/changed cases and affected regressions, with explicit omissions and no false full-suite claim. Phase 1 rule-suite groups are selected only when their declared inputs change, and a check confirms that every specification edge ID has a unit test. The implementation → review/fix → grouped run/log → batch repair → failed-case checks → entire-selected-set pass is proven on the development OS at 1280×720 without a VM.

**Plans:** 4 plans

- [x] [05-01](phases/05-ci-and-repeatable-game-testing/05-01-PLAN.md) Shared catalog, selection and report
- [x] [05-02](phases/05-ci-and-repeatable-game-testing/05-02-PLAN.md) Pure tests, headless smoke and basic CI
- [x] [05-03](phases/05-ci-and-repeatable-game-testing/05-03-PLAN.md) Inherited rendered groups and asset checks
- [x] [05-04](phases/05-ci-and-repeatable-game-testing/05-04-PLAN.md) Review, measured runs and verification

Completed on independently reviewed `b7e419c` with 92/92 selected Windows checks, P1–P4 and hosted Windows and Ubuntu. [Phase 5 verification](phases/05-ci-and-repeatable-game-testing/05-VERIFICATION.md)
**UI hint:** yes

**Delivery boundary:** Build one shared runner over the existing Phase 2–4 scenario definitions and keep the Phase 1–4 runners (`verification`, `gameplay-checks`, `lan-checks` and `asset-pipeline` under `src/godot-game/tools/`) callable as suites instead of rewriting them. Prove exact discovery, execution, exit status and report reconciliation for the shared runner; use a bounded repository-owned scene harness if a candidate test adapter fails compatibility. Automate the delivered rules/menu/LAN/content foundations now and catalog later feature and release cases as planned. Test drivers exercise production input/commands, with observation separated from mutation and test-only authority controls excluded from release builds. Measured coverage, the eight transferred certification IDs, full hosted Windows/Linux runs, evidence reuse, exhaustive runner certification, the other 17 Phase 4 rendered cases and the Phase 1 follow-ups belong to Phase 11 and receive no pass credit here.

**Required planning input:** Read the amended [05-SPEC.md](phases/05-ci-and-repeatable-game-testing/05-SPEC.md) and [05-CONTEXT.md](phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md) before planning Phase 5. The [05-PHASE1-HANDOFF.md](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) is Phase 11 planning input under the 2026-11-23 amendment; Phase 5 re-owns its eight declarations to Phase 11 without claiming them. Phase 1 completion does not satisfy or waive those obligations.

### Phase 6: Survivors, inventory, and combat

Include the rendered case IDs assigned to Phase 6 in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

**Goal:** As a solo or LAN player, I want to control an ordinary survivor, manage real belongings, fight with actual equipment, and continue after a player's death without losing or duplicating survivors or items.
**Mode:** mvp
**Depends on:** Phase 5
**Requirements:** CHAR-01, CHAR-02, CHAR-03, CHAR-04, CHAR-05, CHAR-06, INV-01, INV-02, INV-03, INV-04, INV-05, INV-06, INV-07, INV-08, INV-09, INV-10, INV-11, INV-12, INV-13, COMBAT-01, COMBAT-02, COMBAT-03, COMBAT-05, COMBAT-06, COMBAT-08, RUN-01, RUN-02, RUN-03, RUN-04
**Success Criteria** (what must be TRUE):

1. A run has eight total survivors and one configured starting kit regardless of participant count. Players control unique crowned survivors, walk/sprint/jump with airborne control, and observe configured stamina, safe-camp health recovery, carried-food recovery/cooldown, and nested-load penalties.
2. Players can use all 24 item definitions, eleven equipment slots, actual pockets/containers, rotation, exact stack splitting/merging, legal nesting, filled-container movement, and four quick food references. Invalid fits/cycles/depth fail safely, and living/corpse appearances track equipped belongings.
3. Players can inspect permitted nearby NPC/corpse/loot contents and use the local unlimited paged shared stash with page navigation while other controlled-player inventories remain private. No stash/inventory filtering is delivered here; Phase 12 owns INV-14. Drag cancel, panel closure, lost access, and competing transfers preserve each item exactly once and provide corrected state and failure feedback.
4. Players can fight with fists, one or two one-handed weapons, or a mouse-aimed bow consuming real opposite-hand arrows with manual refill. Costs/timing, armor, retained broken gear, no friendly damage, friendly arrow passthrough, and blocking-defense hits match the specification.
5. Death leaves exactly the character's remaining belongings; nonempty corpses/piles persist and only empty ones expire. Respawn enables one second after death and requires an explicit request before assignment or waiting. Random succession preserves the eligible survivor's state/location/gear, concurrent requests never share a successor, and queued requests automatically receive newly available survivors in request order without gaining loot from crowns. Spectating follows living teammates through Previous/Next and a teammate list. Camera handover is smooth nearby and cuts when distant.

**Plans:** 19/25 plans executed (06-16–06-25 close the gaps in [06-VERIFICATION](phases/06-survivors-inventory-and-combat/06-VERIFICATION.md))
**Wave 1**
- [x] 06-01 — [Authoritative world time, camp safety and access](phases/06-survivors-inventory-and-combat/06-01-PLAN.md)

**Wave 2** *(blocked on Wave 1 completion)*
- [x] 06-02 — [Configured survivor population and starting belongings](phases/06-survivors-inventory-and-combat/06-02-PLAN.md)

**Wave 3** *(blocked on Wave 2 completion)*
- [x] 06-03 — [Equipment, grids, stacks and nested containers](phases/06-survivors-inventory-and-combat/06-03-PLAN.md)

**Wave 4** *(blocked on Wave 3 completion)*
- [x] 06-04 — [Local access, shared stash and transfers](phases/06-survivors-inventory-and-combat/06-04-PLAN.md)
- [x] 06-05 — [Movement, load, recovery and actual food](phases/06-survivors-inventory-and-combat/06-05-PLAN.md)

**Wave 5** *(blocked on Wave 4 completion)*
- [x] 06-06 — [Melee, bows, armor and projectile rules](phases/06-survivors-inventory-and-combat/06-06-PLAN.md)

**Wave 6** *(blocked on Wave 5 completion)*
- [x] 06-07 — [Corpse belongings, cleanup and survivor succession](phases/06-survivors-inventory-and-combat/06-07-PLAN.md)

**Wave 7** *(blocked on Wave 6 completion)*
- [x] 06-08 — [Coherent snapshots and saved inventory layouts](phases/06-survivors-inventory-and-combat/06-08-PLAN.md)

**Wave 8** *(blocked on Wave 7 completion)*
- [x] 06-09 — [LAN action authority and private publications](phases/06-survivors-inventory-and-combat/06-09-PLAN.md)

**Wave 9** *(blocked on Wave 8 completion)*
- [x] 06-10 — [Live equipment, character and corpse presentation](phases/06-survivors-inventory-and-combat/06-10-PLAN.md)

**Wave 10** *(blocked on Wave 9 completion)*
- [x] 06-11 — [Inventory, stash and loot controls](phases/06-survivors-inventory-and-combat/06-11-PLAN.md)

**Wave 11** *(blocked on Wave 10 completion)*
- [x] 06-12 — [HUD, input priority and spectating](phases/06-survivors-inventory-and-combat/06-12-PLAN.md)

**Wave 12** *(blocked on Wave 11 completion)*
- [x] 06-13 — [Playable combat fixture and shared scenarios](phases/06-survivors-inventory-and-combat/06-13-PLAN.md)

**Wave 13** *(blocked on Wave 12 completion)*
- [x] 06-14 — [Asset integration and whole-phase code review](phases/06-survivors-inventory-and-combat/06-14-PLAN.md)

**Wave 14** *(blocked on Wave 13 completion; gap closure)*
- [x] 06-16 — [Rule-suite fixture inventory and MTP producer pin](phases/06-survivors-inventory-and-combat/06-16-PLAN.md)

**Wave 15** *(blocked on Wave 14 completion; gap closure)*
- [x] 06-17 — [Test-name rule conformance](phases/06-survivors-inventory-and-combat/06-17-PLAN.md)

**Wave 16** *(blocked on Wave 15 completion; gap closure)*
- [x] 06-18 — [Debug export, LAN permission consumer and native census](phases/06-survivors-inventory-and-combat/06-18-PLAN.md)

**Wave 17** *(blocked on Wave 16 completion; gap closure)*
- [x] 06-19 — [Inherited rendered runner import preparation and run reporting](phases/06-survivors-inventory-and-combat/06-19-PLAN.md)

**Wave 18** *(blocked on Wave 17 completion; gap closure)*
- [x] 06-20 — [Inventory workspace and HUD contract repairs](phases/06-survivors-inventory-and-combat/06-20-PLAN.md)

**Wave 19** *(blocked on Wave 18 completion; gap closure)*
- [ ] 06-21 — [Solo rendered driver, shared evidence and key receipts](phases/06-survivors-inventory-and-combat/06-21-PLAN.md)

**Wave 20** *(blocked on Wave 19 completion; gap closure)*
- [ ] 06-22 — [Four-role LAN driver and receipt race](phases/06-survivors-inventory-and-combat/06-22-PLAN.md)

**Wave 21** *(blocked on Wave 20 completion; gap closure)*
- [ ] 06-23 — [Original inspection handoff driver](phases/06-survivors-inventory-and-combat/06-23-PLAN.md)

**Wave 22** *(blocked on Wave 21 completion; gap closure)*
- [ ] 06-24 — [Final bindings, no-window gate and independent review](phases/06-survivors-inventory-and-combat/06-24-PLAN.md)

**Wave 23** *(blocked on Wave 22 completion; gap closure)*
- [ ] 06-25 — [Focused repair rerun and evidence record](phases/06-survivors-inventory-and-combat/06-25-PLAN.md)

**Wave 24** *(blocked on Wave 23 completion)*
- [ ] 06-15 — [Grouped gameplay verification and phase closeout](phases/06-survivors-inventory-and-combat/06-15-PLAN.md)

**UI hint:** yes

**Delivery boundary:** Begin with real authoritative elapsed-time, safety/danger, and reachable-access services needed for recovery, cooldowns, cleanup, and interaction; prove these with representative camp/defense/enemy fixtures before integrating the rest of the phase. Include the actual inventory/stash/loot/equipment/health/stamina/quick-slot/succession interfaces and feedback now. Snapshot round trips cover identity, ownership graphs, quantities, condition, cleanup, control eligibility, and saved layout dimensions/placements; legacy representation begins here. NPC/zombie ammunition consumers use the shared combat contract as they arrive, with complete cross-archetype automatic refill/fallback and spit/cosmetic-acid acceptance owned by Phase 8. Autonomous NPC economy and full world campaign remain later increments.

### Phase 7: Camp economy and development

Include the rendered case IDs assigned to Phase ### Phase 7: Camp economy and development in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

**Goal:** Players can sustain and expand a working camp through physical-money transactions, recruitment, NPC jobs, food, hauling, and paid development that respects changing safe territory.
**Mode:** mvp
**Depends on:** Phase 6
**Requirements:** WORLD-05, ECON-01, ECON-02, ECON-03, ECON-04, ECON-05, NPC-01, NPC-02, NPC-03, NPC-04, NPC-05, NPC-06, NPC-07, NPC-08, BUILD-01, BUILD-02, BUILD-03, BUILD-04, BUILD-05, BUILD-06, BUILD-07, BUILD-08
**Success Criteria** (what must be TRUE):

1. Players pay using accessible carried physical money, receive no charge for failed/duplicate actions, and sell only real shared-stash food into physical stash proceeds. Gatherers deplete/refill plants and hunters spend real arrows to recover rabbits' and deer's existing meat; daily wildlife identities/caps preserve animals from prior days.
2. Players recruit only at the main building within completed-tier fireside/population limits, with the basic outfit and no extra starting supplies. They can assign the four paid roles, see equipment waiting, and exercise temporary-control overflow without deleting or forcibly changing survivors.
3. NPCs acquire/upgrade/restock actual stash gear, food, and ammunition without losing replaced gear or nested contents. Eligible ordinary NPCs reserve and haul within capacity while retaining their role; interruptions, danger, and carrier death retain cargo in the correct inventory/corpse.
4. Players purchase tree clearing, three-tier main/wall/tower construction/upgrades, one flat-fee repair to full health, and rebuilding at the last completed tier. Only unassigned workers perform paid work; replacements and interruptions preserve payment/progress, unfinished frames give no completed benefit, and old tiers function during upgrades. Valid building/rebuild/upgrade completion restores full health at the original price/time; an active repair covers further damage without extra charges and closes at full health. Destruction does not refund or revive the structure.
5. Living completed outer walls define borders and fall back to inner walls/refuge after destruction; civilians flee unsafe work with cargo, fight only when escape is blocked, and stop exterior work at night. Standalone towers field 1/2/3 structural archers and the tier-three main has exactly two one-archer attachments sharing its health, without ordinary survivor population, inventory, or loot.

**Plans:** 24 plans

Plans:
**Wave 0 — prerequisite contracts and proof entry**
- [ ] 07-01-PLAN.md — Verify predecessor contracts and produce Phase7 proof entry

**Wave 1** *(blocked on Wave 0 completion)*
- [ ] 07-02-PLAN.md — Authoritative time and retained dawn events

**Wave 2** *(blocked on Wave 1 completion)*
- [ ] 07-03-PLAN.md — Live territory and civilian retreat

**Wave 3** *(blocked on Wave 2 completion)*
- [ ] 07-04-PLAN.md — Physical-payment recruitment and population
- [ ] 07-15-PLAN.md — Persistent daily wildlife admissions

**Wave 4** *(blocked on Wave 3 completion)*
- [ ] 07-05-PLAN.md — Four-tab camp window and People workspace

**Wave 5** *(blocked on Wave 4 completion)*
- [ ] 07-06-PLAN.md — Paid roles, cargo return and basic guards

**Wave 6** *(blocked on Wave 5 completion)*
- [ ] 07-07-PLAN.md — Safe physical NPC equipment upgrades

**Wave 7** *(blocked on Wave 6 completion)*
- [ ] 07-08-PLAN.md — NPC physical food, ammunition and fallback

**Wave 8** *(blocked on Wave 7 completion)*
- [ ] 07-09-PLAN.md — Paid clearing and autonomous shared workers
- [ ] 07-14-PLAN.md — Finite plant gathering and stash delivery
- [ ] 07-16-PLAN.md — Real hunter combat and existing-meat recovery

**Wave 9** *(blocked on Wave 8 completion)*
- [ ] 07-10-PLAN.md — Three-tier construction and damageable frames
- [ ] 07-17-PLAN.md — Reserved physical hauling and carrier-death cargo
- [ ] 07-18-PLAN.md — Direct quoted food sales into physical stash money

**Wave 10** *(blocked on Wave 9 completion)*
- [ ] 07-11-PLAN.md — Functional old tiers and full-health upgrades
- [ ] 07-12-PLAN.md — One flat-fee paid repair to full health

**Wave 11** *(blocked on Wave 10 completion)*
- [ ] 07-13-PLAN.md — Last-completed-tier defense rebuilding

**Wave 12** *(blocked on Wave 11 completion)*
- [ ] 07-19-PLAN.md — Independent structural tower archers

**Wave 13** *(blocked on Wave 12 completion)*
- [ ] 07-20-PLAN.md — Tier-three main-building structural attachments

**Wave 14** *(blocked on Wave 13 completion)*
- [ ] 07-21-PLAN.md — Whole-camp snapshot, configuration and authority hardening

**Wave 15** *(blocked on Wave 14 completion)*
- [ ] 07-22-PLAN.md — Validated camp configuration and production consumers

**Wave 16** *(blocked on Wave 15 completion)*
- [ ] 07-24-PLAN.md — Packaged fields, text and production contract agreement

**Wave 17** *(blocked on Wave 16 completion)*
- [ ] 07-23-PLAN.md — Independently review and complete grouped rendered acceptance

**Cross-cutting constraints:** <!-- gsd:generated -->
- D-09: Fixed People header keeps Buy recruit/carried price/fireside and total counts/blocking reason visible while scrolling.
- D-12: Recruitment,role,clearing,construction,upgrade,repair,rebuild and sale actions submit directly from explicit priced buttons with current-state/no-charge validation.
- D-06: People groups the scrollable roster by role with headings/counts and current activity/equipment waiting.
- D-07: Roster remains visible alongside selected status/roles/prices/gear requirements.
- D-08: Away/controlled targets retain visible prices/gear but disabled choices with specific live reasons.
- D-17: Nearby danger/night interruption briefly signals its cause before retreat animation, while People retains current activity/reason.
- D-15: Nearby paid sites show compact actual work/wait feedback; site panel retains full paid progress/state.
- D-18: Nearby wait markers remain compact and the site panel displays the specific no-worker/danger/nightfall reason.

**UI hint:** yes

**Delivery boundary:** Establish authoritative day/night transitions and once-only plant/wildlife daily-event identities, authored work areas, portal-aware reachability, live border fallback, and danger checks before their dependent work systems. These prerequisites cannot wait for Phase 8. Implement the people/jobs/recruitment/sale/building interfaces and relevant sounds/feedback here. Snapshot and concurrency cases include payments, accepted work quantities, old-tier state, reservations, cargo, roles, and daily resource commitments. Basic warrior combat/equipment operates here; full allocation, paid rally assaults, and recall return behavior belong to Phase 8. Full dawn orchestration across actual saving and late joining is accepted in Phase 9.

### Phase 8: Zombie campaign and outcomes

Include the rendered case IDs assigned to Phase ### Phase 8: Zombie campaign and outcomes in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

**Goal:** Players can defend the developed camp against the complete zombie campaign and assault six permanent portals through correctly resolved victory, continued play, or defeat.
**Mode:** mvp
**Depends on:** Phase 7
**Requirements:** COMBAT-04, COMBAT-07, RUN-05, RUN-06, RUN-07, WORLD-01, WORLD-02, WORLD-03, ZOMB-01, ZOMB-02, ZOMB-03, ZOMB-04, ZOMB-05, ZOMB-06, ZOMB-07, ZOMB-08, ARMY-01, ARMY-02, ARMY-03
**Success Criteria** (what must be TRUE):

1. Players traverse the complete authored map with six portals and every baseline site/clearing/plant/wildlife zone. Active portals block survivors even when jumping, allow zombies through, and become permanently passable when destroyed; default time uses 600-second days, 300-second nights, opening scheduled-wave grace, and three-day escalation.
2. All seven zombie archetypes use their distinct behavior and legal real equipment/loot generated before death. NPCs and zombies spend real ammunition, refill from carried reserves, and fall back to usable gear/fists; spit stops at its first eligible hit and bloater acid remains cosmetic.
3. Every surviving portal supplies eligible scheduled waves, provoked defenders obey warnings/cooldowns/stage/caps even during grace, and each destruction adds distinct retaliation at surviving sources. Queue priority/caps/dawn expiration/source cancellation preserve living enemies and their belongings; independently editable progression changes future generation without player-count scaling or inflating living enemies.
4. Warriors defend allocated sides and fall back with borders; players pay a valid side rally squad to attack its next surviving portal, with invalid/duplicate requests uncharged. Free recall, success, or an obsolete target returns survivors without teleportation, refunds, or an automatic next assault.
5. Either main destruction or loss of all living controlled/eligible survivors defeats the camp. Final-portal destruction immediately awards victory while zombies/possessions persist and further dawn retreat stops; simultaneous first victory/defeat resolves as defeat, and an earlier victory remains recorded after a later continuation defeat.

**Plans:** TBD
**UI hint:** yes

**Delivery boundary:** Integrate produced map/roster/equipment/building resources, shared exploration/minimap, campaign time/HUD, rally controls, warnings, and outcome overlays into actual play now. Full persisted and late-join exploration acceptance remains Phase 9. Carry snapshot proofs for queues, generated loadouts/randomness, warnings, accepted assault orders, once-only event identities, portal destruction, exploration, and outcome precedence. Preserve the documented A-08 interpretation: remaining zombies stop dawn retreat after the final portal; civilian retreat rules remain unchanged. Proposed balance stays configurable and unproven until playtesting.

### Phase 9: Persistence and complete player experience

The Phase 4 [closure exception](phases/04-asset-production/04-CLOSURE-EXCEPTION.md) assigns this phase the 95 named listening verdicts: 88 samples, 5 loops and the live D-31 and D-32 mixes.

Include the rendered case IDs assigned to Phase ### Phase 9: Persistence and complete player experience in the [asset handoff](phases/04-asset-production/04-HANDOFF.md) when selecting and verifying this phase.

**Goal:** Players can reliably save, resume, join, reconnect, and use the complete campaign with safe configuration changes, consistent exploration, and finished presentation.
**Mode:** mvp
**Depends on:** Phase 8
**Requirements:** WORLD-04, WORLD-06, WORLD-07, SESSION-01, SESSION-02, SESSION-03, SESSION-04, SESSION-05, SESSION-06, SAVE-01, SAVE-02, SAVE-03, SAVE-04, SAVE-05, SAVE-06, SAVE-07, SAVE-08, DATA-01, DATA-02, DATA-03, DATA-04, UX-01, UX-02, UX-03, UX-04, UX-05, UX-06, UX-07
**Success Criteria** (what must be TRUE):

1. The host can manually save, autosave at dawn, save on normal exit, and select multiple named runs with the specified retention. Coherent saves preserve all run/character/item/corpse/work/random/event/player/exploration state without healing, rerolls, or repeated dawn events after load/join. Active inventories/menus/saving/victory overlays keep play running; failure/corruption/interruption preserves prior/original data and exit failure offers retry, return, or explicit unsaved departure.
2. Designers can edit the complete separately documented configuration baseline, including independent zombie progression, with stable references and field/unit/default/range/relationship documentation. Invalid values/content fail before start/load with file/field/value/reason; valid host changes apply only at start/load, preserving commitments, fractional time, possessions, and existing populations. Players can see and extract legacy-layout contents under current destination rules without forced relocation or silent loss.
3. Windows/Linux players can discover sessions or use a manual address with metadata/capacity/compatibility feedback, join a continuing host without extra survivors/supplies, and gain control only after coherent catch-up. Equal camp permissions and host-only session controls hold; guest departures restore NPC roles/belongings, reconnection respects survivor availability, and host loss ends the session clearly with last-successful-save recovery.
4. Players have independent cameras and shared explored minimap knowledge, preserved through actual save/load/late join, with identifiable teammates and no unexplored-threat leakage. Every required menu/HUD/people/sale/inventory/stash/loot/building/rally/map/spectator/save/outcome interaction is usable; panel priority, overlapping targets, access loss, and focus prevent background attacks/purchases, while spectators can follow and inspect permitted status without mutating the camp.
5. Each player retains independent display/audio/binding preferences, can remap essential actions and restore defaults, and can use readable 1280x720/1920x1080 layouts and UI scales. Required effects/ambience work in play, critical warnings remain visible when muted, and the English catalog/layouts pass longer-string checks without gameplay text trapped in artwork.

**Plans:** TBD
**UI hint:** yes

**Delivery boundary:** Consolidate the state, configuration, UI, audio, and session foundations already delivered; do not introduce stable identity, serialization, item ownership, or input usability for the first time. Test coherent capture/catch-up during concurrent gameplay, once-only dawn orchestration, both-platform save replacement/recovery, legacy extraction, and no replay of old one-shot audiovisual events. Full SESSION/SAVE/DATA/UX requirements and WORLD-04/WORLD-06/WORLD-07 reach complete acceptance here; earlier partial deliveries are prerequisites only.

### Phase 10: Release validation and balancing

**Goal:** Players receive a complete, runnable Windows/Linux v0.1 whose unaided campaigns, LAN behavior, default pacing, and measured performance/endurance satisfy the release gates.
**Mode:** mvp
**Depends on:** Phase 9
**Requirements:** REL-01, REL-02, REL-03, REL-04, REL-05, REL-06
**Success Criteria** (what must be TRUE):

1. Fresh solo and four-player campaigns reach victory without development commands, injected supplies, or unfinished mandatory steps; recorded runs and tuning support approximately 2–3 hours and 8–12 in-game days at the default baseline.
2. Packaged Windows and Linux builds launch without a development environment, and recorded actual mixed-OS LAN play on separate computers demonstrates the complete campaign/session behavior and tested systems.
3. All required acceptance cases and environment-specific evidence are current and passing, including A-01–A-47 and the additional foundation/regression cases, with no known reproducible crash, corruption, duplication, persistent disagreement, or progression blocker.
4. On the recorded creator reference PC at 1920x1080 with four players, approximately 100 survivors and 300 active zombies, ten minutes of full-behavior stress achieves at least 30 FPS in 95% of one-second samples, no sub-30 stretch over five seconds, and no active-play save/join freeze over 0.5 seconds.
5. A real three-hour saved/resumed endurance run with recurring battles and at least 200 nonempty corpses/loot piles represented over its course preserves accessible belongings and consistent inventories without a progression block.

**Plans:** TBD
**UI hint:** yes

**Delivery boundary:** Record actual CPU/GPU/RAM, OS, build/configuration, sampling method, raw measurements, and run evidence; hardware is not yet specified and no result is presumed. Preserve full AI, inventory, visible equipment, combat, persistent loot, and save/join behavior during performance work. Accelerated setups, headless throughput, and four loopback processes are useful diagnostics, not replacements for required real-duration, rendered-hardware, or physical LAN proofs. Tuning may revise proposed baselines with recorded evidence; mandatory behavior and exclusions remain binding.

### Phase 11: Nightly full regression testing

**Goal:** Developers receive scheduled full-regression results for the complete active catalog and the deferred certification of the test system on the finished game, while routine phase-end testing continues to use the reduced selection.
**Mode:** mvp
**Depends on:** Phase 10
**Requirements:** TEST-11, TEST-12
**Success Criteria** (what must be TRUE):

1. A scheduled nightly entry point executes the complete active case catalog across Critical, High, Normal and Low importance, using the saved scenarios and ordered compatible game-run groups; planned future cases receive no pass credit.
2. The nightly and manual full entry points share the actual scenario definitions, environment declarations and pass/fail/evidence rules. Required builds and multiplayer roles are preserved, compatible scenarios share game runs and necessary extra launches have recorded reasons.
3. Real scheduled execution publishes inspectable summaries, logs, state/image/review artifacts as declared, selected case/environment coverage and measured group/launch timings. Missing cases, crash, timeout, unavailable prerequisites and incomplete evidence produce visible non-success.
4. An agent can identify the tested revision, reproduce failed scenarios, batch fixes and perform focused repair checks followed by the full selected-set regression pass, preserving original failures and attempts.
5. Repository instructions distinguish reduced phase-end testing, nightly full regression and distinct release procedures. The configured schedule/environment and actual execution are recorded; scheduling configuration alone is not a passing nightly run.
6. Real analyzer code and applicable handwritten production code separately meet inclusive 90% line and 80% branch coverage, with a native-hosted engine callback actually measured, and all eight transferred certification identities pass with their original exhaustive meanings.
7. The complete declared scope, including rendered input, four-process LAN and coverage, runs successfully on the actual Windows and Linux environments chosen in this phase's planning; configuration alone is not a pass. The Phase 1 follow-ups (complete Linux suite, graph capacity case, fourteen human reviews) are executed or explicitly closed by the creator.
8. The remaining runner certification passes: evidence reuse validation, permutation, repeat, overlapping-run, interruption and Unicode-identity trials, the staged failure exercise, strict per-file provenance for nightly and release evidence, the renewed Phase 4 review-bound gates on the relocated tree, and the 17 deferred Phase 4 rendered cases with their driver declaration.

**Plans:** TBD
**UI hint:** no

**Delivery boundary:** Add scheduled full testing and its operational handoff on top of Phase 5's framework. Do not postpone immediate reduced selection, saved scenarios, importance metadata or run grouping until this phase. Exact schedule, runner availability/environment and publication details are decided during Phase 11 planning. Preserve the development-OS-only/no-VM routine local policy, manual full execution and separately required physical LAN, display, hardware and endurance release evidence. Under the 2026-11-23 amendment this phase also completes the certification deferred from Phase 5: take the moved and split criteria from the [Phase 5 specification](phases/05-ci-and-repeatable-game-testing/05-SPEC.md) and the coverage packages and eight identities from the [Phase 1 handoff](phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md), with unchanged thresholds and meanings.

### Phase 12: Stash and inventory filtering

**Goal:** Players can find items through stash and inventory filtering while existing locations, quantities, access permissions and transfer behavior remain intact.
**Mode:** mvp
**Depends on:** Phase 11
**Requirements:** INV-14
**Success Criteria** (what must be TRUE):

1. Name/type filtering is usable on stash and inventory surfaces; the detailed results layout is decided in this phase rather than assumed from the declined Phase 6 options.
2. Empty filters reveal the unfiltered view, unmatched filters show a clear empty result, and normalized case-insensitive name matching behaves consistently on Windows/Linux.
3. Filtering never moves items, changes their page/cell/owner or bypasses current access/privacy rules. Transfers still validate the current authoritative state, including stale or concurrently changed results.
4. Stored scenarios and the active regression catalog cover matching, no matches, text normalization, ownership, permission and transfer interactions in solo and LAN play. Follow the current local testing policy and the established nightly/manual framework; retain earlier failures and distinct environment evidence.

**Plans:** TBD
**UI hint:** yes

**Delivery boundary:** Creator-requested on 2026-11-16 during Phase 6 discussion: “let's move filtering to the phase 12. No filtering for stash and inventory now”. Earlier phases retain pages/navigation and normal inventory transfers without filtering. Final filtering acceptance requires actual implementation and evidence here, including affected integration/release checks; earlier full-interface or release labels do not supply acceptance for this deferred feature. No result-list, grid-dimming or other filter layout was selected during Phase 6.

## Coverage

| Phase | Primary requirements | Current status |
| --- | ---: | --- |
| 1. Code rules and documentation | 7 | Complete under focused closure exception |
| 2. Playable foundation | 3 | Complete |
| 3. Four-player LAN foundation | 4 | Complete |
| 4. Asset production | 5 | Complete under closure exception |
| 5. CI and repeatable game testing | 10 | Complete |
| 6. Survivors, inventory, and combat | 29 | Pending |
| 7. Camp economy and development | 22 | Pending |
| 8. Zombie campaign and outcomes | 19 | Pending |
| 9. Persistence and complete player experience | 28 | Pending |
| 10. Release validation and balancing | 6 | Pending |
| 11. Nightly full regression testing | 2 | Pending |
| 12. Stash and inventory filtering | 1 | Pending |
| **Total** | **136** | **29 complete / 107 pending** |

Every approved version 0.1 ID is assigned exactly once; no unknown IDs, duplicate owners, or orphan requirements are allowed. The [requirements traceability table](REQUIREMENTS.md#traceability) is the per-ID ownership record. Its existing A-01–A-47 source crosswalk stays unchanged; Phase 5 expands it into executable/evidence cases and activation states.

## Progress

Execution order: 1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10 → 11 → 12. Plans are created during phase planning; no plan count is invented here.

| Phase | Plans Complete | Status | Completed |
| --- | --- | --- | --- |
| 1. Code rules and documentation | 18/18 | Complete    | 2026-11-14 |
| 2. Playable foundation | 3/3 | Complete    | 2026-11-15 |
| 3. Four-player LAN foundation | 4/4 | Complete    | 2026-11-16 |
| 4. Asset production | 35/37, 2 waived | Complete under closure exception | 2026-11-22 |
| 5. CI and repeatable game testing | 4/4 | Complete | 2026-11-24 |
| 6. Survivors, inventory, and combat | 19/25 | In Progress | - |
| 7. Camp economy and development | 0/TBD | Not started | - |
| 8. Zombie campaign and outcomes | 0/TBD | Not started | - |
| 9. Persistence and complete player experience | 0/TBD | Not started | - |
| 10. Release validation and balancing | 0/TBD | Not started | - |
| 11. Nightly full regression testing | 0/TBD | Not started | - |
| 12. Stash and inventory filtering | 0/TBD | Not started | - |

---
*Detailed roadmap approved: 2026-11-13; local testing policy and Phase 11/TEST-11 added: 2026-11-15; creator-requested filtering deferral and Phase 12/INV-14 added: 2026-11-16; test-scope amendment and TEST-12 added: 2026-11-23 (Europe/Paris). Phases 1–4 are complete.*

## Backlog

### Phase 999.1: Rename phase-named identifiers (BACKLOG)

**Goal:** [Captured for future planning]
Scope: rename the identifiers that still carry delivery-phase numbers after the folder move to `.local/`: `Phase1.cs`/`Phase1.tscn`, the `KingdomZombies.Phase1.Tests` project, the `PHASE1_*`, `PHASE2_*`, `PHASE3_*` and `KZ_PHASE4_*` environment variables, the `phase1`/`phase2`/`phase3` case families, the `Phase4 Windows inspection` export preset and the Linux VM user. Renaming changes test fully qualified names and case IDs across the registers, so it needs its own rebinding plan. Captured by quick task 261123-ltj on 2026-11-23.
**Requirements:** TBD
**Plans:** 0 plans

Plans:
- [ ] TBD (promote with /gsd-review-backlog when ready)

### Phase 999.2: Derive the producer registry and pin its cohort identity once (BACKLOG)

**Goal:** [Captured for future planning]
Scope: (a) a script that regenerates `docs/code/engine-generated-spans.json` from current sources, extending what `src/godot-game/tools/verification/current-replay.cjs` does for `currentProducerReplay`. (b) Keep the Godot producer cohort id and its hashes in one pinned place instead of the ~37 copies in `ExemptionValidator.cs`, `ExemptionTests.cs` and `required-cases.json`. Changing the pin must stay a separate, reviewed edit, and tests must never read their expected id from the file they check. Captured by the creator during Phase 6 plan 06-16 on 2026-11-25. Not to be implemented in Phase 6.
**Requirements:** TBD
**Plans:** 0 plans

Plans:
- [ ] TBD (promote with /gsd-review-backlog when ready)
