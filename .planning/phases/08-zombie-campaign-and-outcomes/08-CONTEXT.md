# Phase 8: Zombie campaign and outcomes - Context

**Gathered:** 2026-11-18 to 2026-11-21 (Europe/Paris)
**Status:** Ready for planning

<domain>
## Phase Boundary

Deliver the complete authored zombie campaign in solo and four-player LAN play. Players explore the shared map, defend the camp, destroy the six portals directly or through warrior assaults, and reach victory, continued play or defeat with consistent shared state. Use real equipment and inventories through combat and outcomes. Deliver live campaign interfaces and a coherent state proof for the Phase 9 persistence and join work.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**23 requirements and 57 acceptance checks are locked.** Read `08-SPEC.md` before planning or implementation. Its 2026-11-21 amendment incorporates D-08 through D-12 from this discussion and takes precedence over the older warning, timing and presentation descriptions in referenced sources.

**In scope**

- The complete authored map, live portals and site gates, independent cameras, shared exploration and a minimap.
- Host campaign time, all seven archetypes, real combat and inventory ownership, scheduled and provoked waves, retaliation, and dawn retreat.
- Warrior side allocation, paid assaults and ordinary recall and return, including the nearby rally action.
- Live clock and sky, map, campaign feedback and result controls, with solo and LAN verification and a coherent campaign state handoff.

**Out of scope**

- Durable saves, load and late-join acceptance, complete settings and presentation, and their Phase 9 integration.
- Procedural maps, new currencies, revival, equipment shops, crafting, additional factions and other networking platforms.
- Supported-display, mixed-OS, reference-PC, endurance and complete pacing release sign-off.

</spec_lock>

<decisions>
## Implementation Decisions

### Campaign map and clock

- **D-10:** Frame the complete authored axis in the compact minimap so camp, both sides and explored landmarks stay in stable positions. Use the same shared discoveries in the expanded full-world map. Preserve the Phase 6 spectator camera rules and the specification's exploration limits.
- **D-10:** Use M to toggle the expanded map and Escape to close it. The map follows the inventory and camp input rules while open. It consumes movement, attacks, quick-food keys and world interactions while authoritative time, threats and other actors continue. Close or change focus without passing old input through to the world.
- **D-10:** Show the current day, daylight or night, and campaign stage in the HUD. Convey progress through the game background. The sun moves left to right from dawn to dusk and the moon moves left to right from dusk to dawn. Both follow authoritative campaign time. Use the sky motion in place of a countdown or progress bar.

### Retaliation and portal destruction

- **D-08:** For each permanent portal destruction, retaliation is ready immediately once that gameplay moment is resolved. Use the selected closest surviving portal on each nonempty side and the locked per-source release rules. An active release finishes before another starts. Keep same-moment destructions distinct, the trigger-time generation inputs, the normal release window and destroyed-source cancellation. Remove the old ten-second lead and dedicated retaliation warning, banner, countdown, cue and sound.
- **D-09:** Show portal destruction through its world effect and permanent ruins and update the known map marker. The destruction emits no separate message to nearby players or the camp. Keep unknown sites hidden. The last portal still triggers the normal campaign outcome.
- The separately specified two-second defender delay remains silent. Other required critical feedback retains its own display rules.

### Contextual rally control

- **D-11:** Give each physical rally a compact nearby prompt with one contextual action. The player approaches the desired rally and commands that rally's side. Show the side being acted on. Use one fresh activation to select and send the ready squad at the displayed carried-money price. Revalidate access, target, price, readiness and side slot when the host accepts the order.
- **D-11:** Show stationed and ready counts, the planned melee/archer composition with applicable fill-ins, the next known portal and the price. Use counts and roles in place of a named roster. Display the specific reason beside an unavailable Send action on approach.
- **D-11:** For an active assault, the same action becomes **Recall squad**. One press recalls for free. While members return, show a disabled **Returning** status and how many warriors are still on the way. A new send becomes available after the return is complete and the normal requirements are met. Existing squad, payment, and ordinary travel rules apply.

### Victory and defeat

- **D-12:** Show a compact victory card with the result, the surviving-enemy warning and the established actions. Movement and combat remain usable outside the card when gameplay owns input. Card clicks apply only to the card. If map, inventory or a camp panel is open, place the card above it and retain that panel and its input ownership.
- **D-12:** Solo Continue closes the card locally. In LAN, the host's Continue dismisses it for everyone without pausing or restarting the run. Main menu follows the existing host and guest session behavior. Keep the award and the card's dismissal distinct so restoration or repeated updates preserve the current presentation.
- **D-12:** Defeat ends active play and presents the day, whether the main building fell, whether no living controlled or eligible recruits remained, and both causes when both apply in the same resolved moment. Show any earlier victory. Preserve the existing menu and new-run flow and Phase 9's ownership of loading earlier saves.

### Planning discretion

Choose the exact placement, dimensions, visual treatment and localized wording for the map, sky, rally prompt and outcome card within the selected behavior. Resolve overlapping interaction ranges through the normal targeted interaction rules, keeping the prompt and committed action bound to the same physical rally. The creator's choice specifies direct per-rally interaction. The exact tie handling was left to planning. Use safe focus transitions and an unambiguous way to show the host-only Continue action to guests.

</decisions>

<canonical_refs>
## Canonical References

Read the relevant sections of these references before planning or implementation.

### Scope and prerequisites

- `.planning/phases/08-zombie-campaign-and-outcomes/08-SPEC.md` — Locked requirements, amendments and acceptance criteria. Read before planning.
- `.planning/PROJECT.md`, `.planning/REQUIREMENTS.md` and `.planning/ROADMAP.md` — Project ownership, Phase 8 boundary and Phase 9 handoff. The Phase 8 specification governs the later changes to older wave, cap, warning and interface descriptions.
- `specs/0.1/initial-spec.md` — §§2.3, 3.2–3.4, 7.4, 9.4–9.7 and the linked acceptance scenarios.
- `specs/0.1/assets-and-resources.md` — Produced world, K03 sky, K25 minimap, actor, effect, audio and interface resources.
- `.planning/phases/06-survivors-inventory-and-combat/06-SPEC.md` and `.planning/phases/06-survivors-inventory-and-combat/06-CONTEXT.md` — Inventory ownership, combat, foreground panels, succession and spectating.
- `.planning/phases/07-camp-economy-and-development/07-SPEC.md` and `.planning/phases/07-camp-economy-and-development/07-CONTEXT.md` — Current camp sides, roles, physical transactions, ordinary movement and camp input.

### Architecture and verification

- `docs/architecture/decisions/0003-state-and-engine.md` — Authoritative controlled state and native engine ownership.
- `docs/workflow/ai-collaboration.md` and `docs/workflow/local-testing.md` — Implementation branch lifecycle, scope for phase evidence, grouped normal-input scenarios and repair cycle.
- `docs/workflow/engine-defects.md` — Known Godot defects relevant to later engine and content verification.

### Code and authored resources

- `src/core/FoundationSession.cs` — Existing authoritative session and movement entry point.
- `src/godot-game/foundation/FoundationInput.cs`, `src/godot-game/foundation/FoundationView.cs` and `src/godot-game/foundation/FoundationHud.cs` — Existing input ownership and live view/HUD boundaries.
- `src/godot-game/content/world/map.json` and `src/godot-game/content/config/time.json` — Authored axis, sky layers and starting time data.
- `src/godot-game/content/config/zombie-progression.json` and `src/godot-game/content/config/portals-assaults.json` — Progression and historical wave, portal and rally defaults to reconcile with the Phase 8 spec.
- `src/godot-game/content/ui/map-spectator-warnings.json`, `src/godot-game/content/ui/camp-panels.json` and `src/godot-game/content/ui/session-save-outcomes.json` — Map and peer markers, rally information fields and result fixtures. Apply the approved live interaction and feedback decisions when integrating them.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- Reuse the authored world axis, visible destruction effects, known active/destroyed portal markers, teammate markers and sky art in the live game. Read the Phase 4 resources as authored assets and sample state.
- Reuse camp prompt and outcome styling, localized messages and the existing menu and session paths. The rally and outcome fixtures identify useful presentation fields and states.

### Established Patterns

- The foundation separates host state from engine views. Input contexts close held actions when foreground ownership changes. Apply that pattern to the full map and preserve the holder when the passive victory card appears above it.
- Phase 6 preserves the world behind semi-transparent panels while blocking their gameplay input. Phase 7 applies the same rule to camp tabs and revalidates priced actions against current access and resources.

### Integration Points

- Drive both maps from shared exploration and live state. Project sun and moon from authoritative phase time alongside the current day/phase/stage HUD.
- Update portal world and known map state on permanent destruction. Commit distinct same-moment retaliation after source selection from the resolved surviving set, then arbitrate each source's ready queue.
- Bind each contextual rally action to its current physical target and side. Publish current acceptance, active and returning squad status through the state that supplies the prompt.
- Publish defeat causes and earlier victory with the resolved outcome. Propagate host Continue to LAN participants and retain the current card state in the campaign handoff proof.

</code_context>

<specifics>
## Specific Ideas

- The creator chose the moving sky cue instead of the offered bar or countdown and clarified the sun's daylight path and the moon's night path.
- The creator initially chose stacked banners and then removed retaliation warnings entirely during the duration question. The immediate readiness and world-only destruction choices supersede the banner proposal and original warning lead.
- The creator asked for one rally button instead of another menu, then chose one press to send and one to recall. Each rally commands its own side.
- The host dismisses victory for all LAN participants. The card can be visible over the current panel while the run continues.

</specifics>

<deferred>
## Deferred Ideas

No new backlog ideas were added. Durable saves, load, late join and complete settings remain under Phase 9.

</deferred>

---

*Phase: 08-zombie-campaign-and-outcomes*
*Context approved: 2026-11-21*
