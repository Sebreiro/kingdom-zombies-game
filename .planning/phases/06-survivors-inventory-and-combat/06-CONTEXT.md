# Phase 6: Survivors, inventory, and combat - Context

**Gathered:** 2026-11-16–2026-11-17 (Europe/Paris)
**Status:** Ready for planning; prerequisites remain required before implementation

<domain>
## Phase Boundary

Deliver the solo/four-player LAN survivor, real-inventory, combat, corpse/loot and control-succession slice under the amended Phase 6 specification. Include usable production interfaces, coherent saved state and the stationary-hostile proof fixture. Phase 5 must complete before implementation; this discussion does not advance Phase 4 execution or claim asset/framework/gameplay acceptance.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**22 requirements covering 29 Phase 6 source IDs are locked.** Downstream agents MUST read [06-SPEC.md](06-SPEC.md) before research, planning or implementation. Requirements are not repeated as new decisions here.

Two explicit creator amendments have already been reconciled with the specification, source game spec, requirements, roadmap, project and state notes:

- **2026-11-16:** Stash/inventory filtering is deferred to Phase 12 (INV-14). Phase 6 retains unfiltered paged stash access, navigation and transfers under INV-10.
- **2026-11-17:** One second after death enables Respawn; an explicit request is required before assignment or queuing. Pending requests later complete in order. Spectating follows living teammates.

**In scope:**

- The complete CHAR/INV/COMBAT/RUN source set above, in normal solo and four-player LAN gameplay.
- Real elapsed-time, safety/danger and reachable local-access rules, proven through representative camp/defense/enemy fixtures before the rest of the phase is integrated.
- Exactly eight persistent ordinary survivors and one configured fresh-run kit; existing survivors are used for joining/control, and guest departure does not delete a survivor or possessions.
- All 24 item definitions, eleven slots, actual pockets, filled/nested containers, stacks, rotation, load, condition and ground loot.
- Immediate permitted inventory interactions, private controlled inventories, local unlimited paged stash, page navigation, quick transfer, four food references and corrected conflict feedback.
- Fists, usable single/dual melee, manually refilled mouse-aimed bows, faction/obstruction rules, armor/wear and retained broken equipment.
- Living/corpse equipped appearances and crowns using the required production content.
- The actual health/stamina/equipment/arrow/load/money HUD, inventory/stash/loot panels, quick-slot interaction and spectator/succession surfaces, including empty/error/access-loss states.
- Coherent snapshot round trips of the new state and complete saved-layout dimensions/placements/footprints/metadata.
- Stationary hostile targets with triggered real attacks, representative defenses/terrain and repeatable death/succession/loot scenarios; fixture-only supplies remain separate from normal startup.
- Stored scenarios, phase-owned acceptance activation, pure/build checks, review and grouped post-review solo/four-process evidence under the current testing policy.

**Out of scope:**

- Stash and inventory filtering, including filter controls, name/type matching, normalization and filtered-result states — Phase 12 owns INV-14 under the creator's 2026-11-16 amendment.
- Autonomous recruitment, job/equipment/restocking AI, food sale, hunting/gathering, hauling, paid construction/repair/upgrades and the full safe-work economy — Phase 7 owns those player flows. Phase 6 supplies their shared item/safety/combat foundations.
- Seven-archetype zombie behavior, enemy pursuit, waves, portals, progression, retaliation, army orders and complete victory/defeat — Phase 8. Stationary targets suffice here; zero successor candidates can remain a fixture spectator/waiting state without claiming campaign defeat acceptance.
- Complete cross-archetype automatic arrow refill/fallback, natural spit and cosmetic bloater acid acceptance — Phase 8 owns COMBAT-04/07. Shared equipment/ammo rules are established here without requiring those autonomous consumers.
- Full handcrafted-map/day-night/minimap integration — later world/campaign phases. Representative camp borders, danger, local reachability and elapsed time are required now rather than constant test substitutes.
- Manual/dawn/exit saves, multi-run selection, atomic file recovery, complete configuration-change application and interactive legacy-layout viewing/extraction — Phase 9. Saved-layout preservation is required here; legacy extraction is not claimed.
- Complete session recovery/reconnection/host-loss saves and the remaining session metadata/permissions experience — Phase 9. Basic joins/leaves must already conserve the Phase 6 survivor/item state.
- Complete preferences/remapping/UI-scale/translation/audio polish across the full game — Phase 9. This phase's input, feedback and equipment presentation must already work; later UX ownership does not permit missing phase interfaces.
- Physical mixed-OS LAN certification, other supported display configurations, reference-PC performance and real-duration endurance/release validation — their declared release procedures/Phase 10; routine local checks use development Windows at 1280×720 without a VM.
- Scheduled nightly full regression execution — Phase 11. Critical/new/changed/affected local selection and saved scenario metadata apply immediately.
- Additional combat/progression systems, friendly damage, equipment repair and other v0.1 exclusions — the canonical game scope remains unchanged.

</spec_lock>

<decisions>
## Implementation Decisions

The 48 decisions below reflect the creator’s selected answers, including custom responses. Offered alternatives and recommendations that were not selected are not approvals.

### Inventory layout and nested bags

- **D-01:** Arrange the main inventory with the character silhouette and eleven equipment slots on the left, and labeled item-provided pocket/bag grids on the right. Keep equipment and storage visible together at 1280×720.
- **D-02:** Open each nested container in its own movable window, allowing several actual grids to be arranged side by side.
- **D-03:** While dragging, show the full cell footprint, distinguish legal/illegal placement and highlight compatible equipment slots. Explain a rejected drop using the chosen failure-feedback surface.
- **D-04:** Assign carried food to quick slots by dragging onto a slot or using an Assign to quick slot item-menu action. These remain references to actual carried stacks.
- **D-05:** Provide deliberate world dropping through the item-menu Drop action. Releasing a drag outside a valid grid cancels it; there is no dedicated world-drop zone selected.
- **D-06:** Shift-drag opens an exact quantity field with a Half shortcut, confirm/cancel and a preview of extracted/remaining quantities. Preserve the specification’s exact split identities and integer limits.
- **D-07:** Closing an outer container window also closes windows for its nested descendants. This changes views only and never moves their belongings.
- **D-08:** Open inventory panels take over movement input: close them before walking, sprinting or jumping again. The live world, other actors, recovery, physics and cleanup continue under authoritative time; the input rule must not freeze the simulation.
- **D-09:** Put detailed item information, including weight, exact condition, stack limit and storage size, in hover tooltips rather than a fixed selected-item inspector.
- **D-10:** Show a slim condition bar on equipment icons, exact values in the tooltip and a clear broken marker. Keep one canonical inventory icon per item definition.
- **D-11:** On first opening a bag window, place it beside the source grid/window, keeping it on screen where possible. The player can move it afterward; later openings use D-15’s remembered position.
- **D-12:** Equipping is drag-only. Do not add the offered Equip item-menu shortcut. Occupied slots still require manual unequipping first; no automatic swap is introduced.
- **D-13:** Use almost the full screen for the inventory workspace, with a semi-transparent background so the active world remains visible. Preserve readable text, icons, cell boundaries and damage/status cues.
- **D-14:** Escape first cancels an active drag and leaves browsing open. A subsequent Escape closes the active inventory window, before any enclosing in-run menu action.
- **D-15:** Within the same play session, remember bag-window positions but keep bag windows closed when inventory reopens. Reopen only the main inventory; manually reopened bags return to their remembered positions. Access must still be valid.
- **D-16:** Containers open through the item-menu Open action only. Ordinary clicks, double-clicks and drags do not open them; do not add the offered dedicated Open button.

### Stash and nearby looting

- **D-17:** Use a fixed side-by-side workspace for external access: own equipment on the far left, carried storage in the middle and the stash/NPC/corpse/pile inventory on the right. Nested bags retain their movable windows and the semi-transparent treatment.
- **D-18:** Creator-approved scope amendment: defer all stash and inventory filtering to Phase 12 under INV-14. Phase 6 has no filter controls or filtering acceptance; retain unlimited pages, navigation, local access, deliberate placement, quick transfers and stack-first auto-placement under INV-10. No offered filter-results layout was selected.
- **D-19:** For overlapping permitted nearby targets, show the selector inside the external inventory panel on the right. Selecting an entry loads that target’s current contents there.
- **D-20:** Show failed-transfer explanations in the affected panel, with corrected authoritative contents. If access closes that panel, use a brief HUD notice instead. No dismissal dialog was selected.
- **D-21:** Ctrl-click withdrawals use any carried storage: merge compatible carried stacks first, then find legal space in pockets or bags. This is storage placement, not automatic equipping or arrow-hand refilling; maintain actual capacity, nesting and ownership rules.
- **D-22:** Browse stash pages with Previous/Next arrows, current-page/total information and direct page-number entry. Scrollable page tabs and vertically stacked page grids were not selected.
- **D-23:** After an automatic deposit affects another stash page, switch to the first affected page, highlight changed cells and indicate any additional affected pages. Present the committed result without moving items for display.
- **D-24:** If an external target becomes inaccessible while other permitted targets remain, close its contents and related bag windows safely, keep own inventory open and return the external panel to the nearby-target list with the loss-of-access reason.

### Combat HUD and feedback

- **D-25:** Place health/stamina and carried money/load at the top left; put hands, ready arrows and four food slots along the bottom action bar.
- **D-26:** Damage to the controlled survivor uses a restrained screen-edge tint and health-bar pulse, visible over panels, alongside the established character flinch. The world continues running.
- **D-27:** Explain blocked attacks briefly beside the action bar near the hands/ammunition display, emphasizing the relevant readout. D-43 explicitly overrides this for empty ammunition: do not show an empty-ammo or failed-empty-shot explanation.
- **D-28:** Show attack and food cooldowns as shaded sweeps over the relevant icons, clearing as readiness returns. Food references display the same personal food cooldown, not independent slot timers.

### Death, handover and spectating

- **D-29:** Require the player’s Respawn request before post-death succession. On actual assignment, move the camera smoothly for a nearby successor and cut directly for a distant successor; preserve that survivor’s real location and state.
- **D-30:** Enable Respawn one second after death. Time passing alone does not assign a survivor; clicking an enabled button assigns an available eligible survivor immediately, subject to earlier queued requests. The click does not start an additional one-second delay.
- **D-31:** If no eligible successor is available, clicking Respawn records a waiting request. That request completes automatically when a survivor becomes available in request order, retaining player-slot ties for equal authoritative request times. No second confirmation is required after queuing. Preserve exclusive assignment, random eligible selection, actual belongings and duplicate-command safety.
- **D-32:** Spectating follows other players’ living survivors, using Previous/Next buttons plus a compact teammate list and visible Respawn/request status. Do not provide a freely moving spectator camera or substitute ordinary NPC following. Preserve the specification’s spectator permissions and no-unexplored-territory rules.
- **D-33:** Immediately after the controlled survivor dies, hold a fixed camera view at the death location until the player chooses a living teammate to follow or Respawn completes. Do not automatically attach to an available teammate.
- **D-34:** If a followed teammate dies, hold the camera at its current view and let the spectator select another teammate. Do not automatically cycle to another living teammate. Zero living teammates remains an explicit waiting/no-follow-target state.
- **D-35:** For a queued request, show a simple Waiting for a survivor status. Do not display the offered queue position or full waiting-player list.
- **D-36:** On successful handover, show a brief Control transferred banner as the camera, crown and HUD update to the newly controlled survivor’s actual state.

### Actions while inventory is open

- **D-37:** Any open inventory, stash or loot panel blocks attacks until panels close, in addition to the movement-input block in D-08. Semi-transparent panels never pass clicks through to world actions.
- **D-38:** Block quick-food keys 1–4 while inventory panels are open. Number input in inventory cannot also consume quick-slot food.
- **D-39:** Add a Consume action to the item menu for actual carried food. Use that exact action name rather than the offered Eat label. It consumes one real carried unit under the existing health/stamina/full-stat/cooldown rules. Food in external inventories must first be transferred to the controlled survivor.
- **D-40:** Nonlethal damage keeps inventory panels open and preserves the current safe preview/interaction while showing the chosen damage cues. The player decides when to close the panels; death/access-loss cleanup still follows the specification.

### Bow aiming and ammunition cues

- **D-41:** Use a small crosshair at the cursor while using the bow. Do not add the offered short aim line.
- **D-42:** The action-bar ammunition readout shows only arrows ready in the opposite hand. Inspect carried reserves through inventory; do not add a reserve total to the HUD or its ammunition tooltip.
- **D-43:** Specific creator override: empty ammunition shows only a zero count and no visible arrow on the bow. Add no extra empty-hand marker, refill hint or blocked-empty-shot explanation. This is an exception to D-27, not approval of the offered zero-count-plus-explanation option. Empty shots still reject safely and do not queue or create ammunition.
- **D-44:** An empty bow continues aiming and following the cursor, visibly unloaded. Do not lower it into the offered carry pose merely because the arrow hand is empty.

### Recovery and load feedback

- **D-45:** Represent safe health recovery with short Unsafe / Resting / Recovering status labels, without a qualification progress display. The authoritative safety conditions, continuous five-second qualification and recovery rate remain those in the specification.
- **D-46:** Show carried weight, Normal/Heavy/Overloaded state, current movement-speed modifier and current stamina-cost modifier directly in the HUD. Exact load penalties are not restricted to a tooltip; the configured thresholds/curve remain unchanged.
- **D-47:** Use the stamina bar alone to show spending and refilling. Do not add a separate recovery-delay or active-regeneration indicator.
- **D-48:** For a sprint or jump blocked by load or insufficient stamina, show a short reason beside the vitals and emphasize the relevant stamina/load readout rather than using the action-bar notice area.

### Agent's Discretion

The creator did not select a general “you decide” option. Research/planning may resolve routine implementation choices within the decisions and locked specification: exact panel dimensions/transparency, contrast and font sizing; window placement/clamping/scrolling; valid Half rounding and quantity widgets; labels/localization keys; short cue durations; the distance threshold for smooth-versus-cut camera handover; numeric display precision; stable authoritative command, item/state and snapshot representations; and the required scenario wiring. These are implementation choices, not additional creator-approved gameplay requirements.

Preserve the existing project boundaries, configurable baseline, manual arrow refill, private/local access, continuous simulation and all quantity/identity/timing invariants. Do not interpret input blocking as pausing the world, auto-equip quick transfers, substitute another food stack, or use generic failure text for the explicitly quiet empty-ammo case. No new library, project, paid tool, protocol or architecture exemption was selected. Full preferences/remapping/scale, legacy extraction and complete campaign/economy behavior retain their specified later owners.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.** All paths are repository-relative and complete.

### Scope and accepted amendments

- `.planning/phases/06-survivors-inventory-and-combat/06-SPEC.md` — Locked 22 requirements, 29 source IDs and acceptance criteria; includes the filtering and explicit Respawn amendments. MUST read before planning.
- `.planning/ROADMAP.md` — Phase 6 delivery/dependency boundary and Phase 12 filtering ownership.
- `.planning/REQUIREMENTS.md` — Primary ownership and current 135-requirement map; INV-10 remains Phase 6, INV-14 belongs to Phase 12, RUN-02/RUN-04 reflect explicit Respawn.
- `.planning/PROJECT.md` — Project direction and accepted creator amendments.
- `.planning/STATE.md` — Current execution position and outstanding prerequisites; Phase 4 continues independently at capture time.
- `specs/0.1/initial-spec.md` — Sections 2, 4, 5, 13 and acceptance scenarios; apply the dated filtering and Respawn amendments, not superseded automatic-handover wording.
- `specs/0.1/assets-and-resources.md` — Required production content and evidence boundaries.

### Carried-forward decisions

- `.planning/phases/04-asset-production/04-CONTEXT.md` — Pixel scale, wooden frames/dark interiors, fonts, restrained hit effects, authored flinch/death, logical hand identity and equipped/corpse composition.
- `.planning/phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md` — Build-only basic PRs, dependency-aware selection, evidence reuse limits and the current local repair cycle.
- `.planning/phases/03-four-player-lan-foundation/03-CONTEXT.md` — Inherited LAN foundation and user communication preferences; its older implementation-status/platform narration is historical.

### Architecture, code and verification

- `docs/architecture/README.md` — Canonical project/type/engine/state boundaries and their executable definitions.
- `docs/architecture/decisions/0002-boundaries.md` — Gameplay, engine and service dependency separation.
- `docs/architecture/decisions/0003-state-and-engine.md` — Controlled state, strict snapshots, atomic publication and engine lifetime.
- `docs/code/README.md` — Applicable C# rules, exemptions, review and portable documentation conventions.
- `docs/workflow/ai-collaboration.md` — Phase implementation branch/draft-PR/ready rules, interactive controls and current testing policy.
- `docs/workflow/local-testing.md` — Finish implementation and code review before grouped rendered tests; select Critical/new/changed/affected cases on development Windows at 1280×720.
- `docs/workflow/verification.md` — Delivered check commands and the distinction between current execution, reused evidence, blocked checks and later release environments.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- `src/core/FoundationSession.cs`, `src/core/FoundationActorState.cs` and `src/core/FoundationSnapshot.cs` provide the existing authoritative movement/participant publication. They do not provide a persistent eight-survivor population: admission currently creates a temporary actor and departure removes it.
- `src/godot-game/foundation/FoundationInput.cs` tracks physical holds, fresh presses and clear/block behavior at input-context boundaries. Extend those protections to the selected panel input rules without replaying held movement/attacks when panels close.
- `src/godot-game/foundation/FoundationHud.cs` and `src/godot-game/foundation/FoundationView.cs` provide roster projection and a locally assigned camera. They are foundations for the actual vitals/action-bar/panel/spectator surfaces, not evidence those surfaces are implemented.
- `src/core/SnapshotDto.cs`, `src/core/SnapshotCodec.cs` and `src/core/ExampleStateStore.cs` demonstrate detached state, explicit time/randomness and strict atomic restore patterns. They are examples rather than the complete new survivor/item/request graph.
- `src/core/ContentPack.cs` provides authenticated content definitions, stable resource references and independent pack diagnostics. `src/core/CharacterPresentation.cs` composes authored elapsed-time tracks, including evolving bow/item presentation. Phase 4 remains in progress; these classes and inspection previews do not certify accepted production content or gameplay ammo/condition ownership.

### Established Patterns

- Keep gameplay independent of Godot and discovery/connection details. Use stable identity, one authoritative committed state, explicit elapsed time/randomness, validation before mutation, coherent publications and detached snapshots.
- The initial scout found no production health/stamina, item ownership graph, equipment/storage, combat, corpse cleanup, Respawn queue or spectator subsystem. Re-scout the completed prerequisites before implementation; do not treat appearance/content examples as those systems.
- Carry Phase 4’s practical pixel-art composition, wooden framing/dark quiet interiors, clear body text, restrained visual impact accents, authored flinch/death and actual worn/held/corpse equipment. Exact new layout choices are recorded above.
- Existing documentation indexes and focused rules remain canonical. No codebase maps, packaged spike/sketch findings or matching pending todos were found for this discussion.

### Integration Points

- Extend the existing solo/LAN production action path into persistent survivor control and all item/combat/Respawn transactions, rather than adding a separate gameplay authority for the fixture or host-local UI.
- Revalidate access, owner, state, death and control at commit. Connect UI cancellation/window closure to preview lifetime, not implicit world drops; keep another controlled survivor’s inventory private.
- Extend publication/catch-up and snapshot state with ownership/layout/condition/timers, Respawn availability and pending request order, preserving old-layout representation without claiming Phase 9 extraction.
- Feed actual inventory/equipment/ammunition and authoritative actions into the accepted character presentation. The zero-arrow bow has no visible arrow even while it continues aiming.
- Use the completed Phase 5 framework and saved scenarios under the current grouped development-OS testing policy. Preserve later campaign/economy/release portions and exact evidence environments.

</code_context>

<specifics>
## Specific Ideas

- The creator selected almost full-screen inventory and added a semi-transparent background to see the world behind it.
- “let's move filtering to the phase 12. No filtering for stash and inventory now” is an explicit scope/timing amendment, not a filter-layout selection.
- “wait for user to press "respawn" button and than first option” selected explicit confirmation plus smooth-nearby/cut-distant camera movement; the next answers resolved one-second button enablement and click-to-queue.
- Spectating is for other living players, rather than a free camera. Subsequent answers explicitly retain a fixed view until the player chooses another teammate.
- The food item-menu action is named **Consume**, not Eat.
- “Just zero cound and no arrow in the bow. No additional hint of explanation” is a custom exception to general blocked-attack messages, not the offered zero-count-plus-explanation choice.
- All seven areas were selected explicitly (the original four, then all three additional areas). The creator approved creation of the final context and discussion log after reviewing all 48 answers.

</specifics>

<deferred>
## Deferred Ideas

**Phase 12: Stash and inventory filtering — INV-14.** The roadmap phase exists at `.planning/phases/12-stash-and-inventory-filtering/`. No filtering is delivered in Phases 6–11. Phase 12 owns the detailed filter interface, matching/empty/text-normalization behavior, permissions and unchanged-layout/transfer proof; no offered results list, page-dimming or combined layout was selected here. This remains final scope and requires actual implementation/evidence in its owner phase.

No other new capability was deferred from the discussion. The existing Phase 7 economy/AI, Phase 8 campaign, Phase 9 persistence/preferences/legacy extraction, Phase 10 release and Phase 11 nightly responsibilities retain their amended boundaries in the locked specification/roadmap.

</deferred>

---

*Phase: 06-survivors-inventory-and-combat*
*Context gathered: 2026-11-16–2026-11-17 (Europe/Paris)*
