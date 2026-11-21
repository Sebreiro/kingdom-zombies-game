# Phase 7 — Camp economy and development context

**Gathered** 2026-11-18 (Europe/Paris)

**Status** Ready for planning. Phase 6 implementation is required before Phase 7 implementation.

<domain>
## Phase Boundary

Deliver a working solo and LAN camp on both sides of the main building. Players recruit survivors, assign roles, sell stored food, and pay for development from carried money. NPCs gather, hunt, haul, and perform shared work as territory, danger, and time change. This discussion settles how players navigate, give those orders, and see the results.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**25 requirements are locked, including the 22 requirements assigned to Phase 7.** Read [07-SPEC.md](07-SPEC.md) before research, planning, or implementation. It defines the complete requirements, constraints, acceptance checks, and source scenario boundaries.

**In scope**

- A playable working camp on both sides, authored work, food and wildlife areas, and representative active and destroyed portal barriers.
- Authoritative day and night transitions, finite resources, once-per-day events, and live camp borders with refuge fallback before dependent jobs.
- The 22 requirements assigned to Phase 7: WORLD-05, ECON-01–05, NPC-01–08, and BUILD-01–08.
- Physical payments, food sales, recruitment limits, autonomous roles, equipment, food and ammunition, safety, hauling, and shared paid work.
- Three tiers for the main building, walls, and towers. Damageable frames, full-health build, rebuild and upgrade completion, and one flat-fee repair order to full health.
- Basic nearest-wall or refuge guarding and structural tower attacks using the shared combat and item rules.
- Working camp panels, produced art and sound, feature configuration, coherent snapshots, and repeatable scenarios with solo, host, and guest evidence.

**Out of scope**

- Phase 8 owns the full six-portal map and campaign, shared exploration and minimap, the full zombie roster and scheduled waves, full warrior allocation and rally/recall, and campaign outcomes.
- Phase 9 owns full saves and player-facing load, reconnection, late join, and complete changed-configuration behavior. Phase 12 owns stash and inventory filtering.
- The specification excludes direct NPC task and target commands, new economy systems such as crafting and equipment shops, alternate repair billing, and runtime dependence on recovered source folders.
- Hosted and release procedures own physical mixed-OS LAN, supported-display, hardware, endurance, release balance, and other distinct evidence.

</spec_lock>

<decisions>
## Implementation Decisions

### Main-building navigation

- **D-01:** Use one tabbed camp window with **Stash**, **People**, **Food sales**, and **Main building**. Open on Stash every time. The Stash tab incorporates the Phase 6 equipment, carried storage, and shared stash workspace with its existing paging, item access, and transfer rules.
- **D-02:** Make the camp window almost full screen and semi-transparent. Carry forward the wooden frames and dark interiors. Keep the active world and warning cues visible. Each selected tab gets the entire workspace, so the equipment and storage grids appear only on Stash.
- **D-03:** Use the inventory input rules for all camp tabs. Block movement, attacks, quick-food keys, and world clicks while the window is open. Time, work, and threats continue, and Escape handles the active panel before the run menu.
- **D-04:** Leaving Stash closes open bag windows and remembers their positions for this session. Returning to the tab leaves them closed until manually reopened. Apply normal inventory cancellation and access rules to any unfinished interaction.
- **D-05:** Keep the camp window open on nonlethal damage and show the damage cues over it. Death or loss of main-building access closes the window.

### People and role assignment

- **D-06:** Use one scrollable roster grouped by current job, with a heading and count for each role. Include the survivor's current job and activity, including equipment waiting.
- **D-07:** Keep the roster visible while the selected survivor's status, role choices, prices, and gear requirements appear to its right.
- **D-08:** For a survivor who is away or under player control, show the job choices disabled with the specific reason. Keep prices and gear requirements visible and update availability as the survivor's state changes.
- **D-09:** Put **Buy recruit**, its carried-money price, and both the fireside and total population counts in a fixed header above the roster. Keep these visible while scrolling or inspecting a survivor. Show which limit or other eligibility rule is blocking recruitment.

### Purchases and food sales

- **D-10:** Let players type an exact whole-number sale quantity or adjust it with +/−. **Max** selects the current available quantity of the chosen food.
- **D-11:** Show current stash stock, selected quantity, unit price, total proceeds, and resulting stash money before **Sell**. The button submits the visible quote directly, including a quantity selected with Max.
- **D-12:** Submit recruitment, paid role changes, clearing, construction, upgrades, repairs, and rebuilding directly from their explicitly priced buttons. The visible price and action are the purchase step. Use the existing current-state eligibility and no-charge rules.
- **D-13:** Refresh the food-sale view and quote as current stock and prices arrive. If the available stock falls below the selected amount before Sell is pressed, reduce the amount to the available stock and update the displayed proceeds. At zero stock, the sale action is unavailable. A sale still needs an explicit Sell click.
- **D-14:** Evaluate the submitted sale against the quantity and price shown when the player pressed Sell. When the host rejects a changed quote or otherwise stale action, show the current values and explain the rejection. Preserve food and money on the rejected request and require a new click for any revised sale.

### Work and safety feedback

- **D-15:** As a player approaches a site with a paid order, show a small work bar or waiting marker in the world. The site panel shows the full paid progress and current state.
- **D-16:** Mark the nearby repair bar as paid repair and show current health against full health. The bar can move down after further damage during the paid order and back up as workers restore health.
- **D-17:** When a nearby civilian stops work because of danger or nightfall, show a brief marker identifying that cause, then let the retreat animation carry the action. Keep the current activity and reason in the People tab.
- **D-18:** Keep the nearby site waiting marker compact. Put the specific waiting reason in the site panel, such as no eligible worker, danger, or nightfall.

### Planning discretion

Research and planning can choose exact panel dimensions, responsive layout and scroll behavior, roster ordering within each role, cue range and duration, marker art, short labels, and display precision. Preserve the selected transitions, visibility, interaction, and live-update behavior. Use authoritative committed state for costs, ownership, progress, and feedback. Existing architecture, content identities, and verification procedures apply.

</decisions>

<canonical_refs>
## Canonical References

Read the applicable references before planning or implementing. These are complete repository-relative paths.

### Scope and prerequisites

- `.planning/phases/07-camp-economy-and-development/07-SPEC.md` — Locked requirements, boundaries, and acceptance criteria. Read first.
- `.planning/ROADMAP.md` — Phase 7 delivery boundary, Phase 6 dependency, and later owners.
- `.planning/REQUIREMENTS.md` — Requirement ownership and source scenario crosswalk.
- `.planning/PROJECT.md` — Project scope and creator-approved amendments.
- `.planning/STATE.md` — Active implementation position and prerequisite status.
- `specs/0.1/initial-spec.md` — Camp, jobs, economy, buildings, time, interface and source acceptance rules.
- `specs/0.1/assets-and-resources.md` — Required art, sound, UI, and configuration resources and evidence.
- `.planning/phases/06-survivors-inventory-and-combat/06-SPEC.md` — Shared item ownership, access, combat, Respawn, and snapshot contracts.
- `.planning/phases/06-survivors-inventory-and-combat/06-CONTEXT.md` — Stash layout, paging, bag windows, input and damage cues, and authoritative failure feedback.
- `.planning/phases/04-asset-production/04-CONTEXT.md` — Pixel-art style, interface and sound treatment, and inspection scope.
- `.planning/phases/05-ci-and-repeatable-game-testing/05-CONTEXT.md` — Case selection, evidence, and scenario grouping.

### Architecture and workflow

- `docs/architecture/README.md` — Project boundaries and index of state and engine rules.
- `docs/architecture/decisions/0003-state-and-engine.md` — Authority, state reconstruction, and engine lifetimes.
- `docs/workflow/ai-collaboration.md` — Phase implementation branch and draft PR lifecycle, and verification policy.
- `docs/workflow/local-testing.md` — Whole-phase review before rendered checks, required case selection, and repair cycle.

### Existing code and resources

- `src/core/FoundationSession.cs` — Authoritative participant identity, movement, and immutable publication patterns.
- `src/godot-game/inspection/CampLayouts.cs` — Inspection compositions for people, sales, and sites.
- `src/godot-game/content/ui/camp-panels.json` — Camp appearance and example states for availability, price, stock, paid progress, and interruption.
- `src/godot-game/content/config/time.json` — Configured day/night lengths and ordered dawn events.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- Reuse the Phase 4 UI theme, fonts, icons, and authored camp samples where they fit the approved layouts. The inspection People page already puts the roster beside role details. The sale examples show three food types, stock, quotes, and proceeds. Site examples cover health, tier, order state, and worker availability.
- The packaged main-building, defense, wildlife, and work presentations give the production state views a starting point. DT-02 provides the time and dawn data.

### Established Patterns

- The current foundation session owns participant movement and publishes one immutable view. Use the production Phase 6 inventory, combat, control and snapshot paths once available for Phase 7 integration.
- `CampLayouts` and `camp-panels.json` represent inspection samples. Use the approved production decisions for behavior where samples differ, including the sale sample's confirmation and site repair progress.

### Integration Points

- Share the Phase 6 stash workspace and access checks inside the Stash tab. Keep focus, selection, and open bag windows as view state and use ordinary inventory operations to move belongings.
- Feed People, Food sales, and site views from current authoritative state. Submit the displayed action from one intentional click and render corrected contents or reasons on rejection.
- Use actual paid order state and job transitions to drive the nearby bar, marker, and one-shot interruption cue. Keep detailed reasons in the corresponding panel and retain visible warnings while panels are open.

</code_context>

<specifics>
## Specific Ideas

- The creator's later choice to open on the Stash tab supersedes the earlier separate Open stash button and its close behavior.
- The sale example is seven selected berries and a stock update to four. Before the click, show four and its revised proceeds. If the clicked sale loses the race, show an error with the reason and the current quote.
- People and site panels carry the full explanations. The world shows short cues that let the player keep following the action.

</specifics>

<deferred>
## Deferred Ideas

No new backlog items came out of the discussion. Later-phase owners are listed in the specification boundary.

</deferred>

---

*Phase directory* `07-camp-economy-and-development`

*Context gathered* 2026-11-18 (Europe/Paris)
