# Phase 8: Zombie campaign and outcomes - Discussion Log

> **Audit trail only.** Use [08-CONTEXT.md](08-CONTEXT.md) and [08-SPEC.md](08-SPEC.md) for planning. This log includes alternatives and superseded choices.

**Dates** 2026-11-18 to 2026-11-21 (Europe/Paris)

**Areas discussed** Campaign map and clock, retaliation warnings, warrior rally panel, victory and defeat screens

The creator selected all four areas and approved saving the phase context, discussion log and amended specification.

## Campaign map and clock

### 1. What should the compact minimap show during play? Both options keep unexplored locations hidden and use the same shared discoveries.

| Option | Description | Selected |
| --- | --- | --- |
| Fixed full map (recommended) | keep camp, both sides, and explored landmarks in stable positions so players can orient themselves at a glance | Yes |
| Local scrolling map | follow the player at a larger scale for clearer nearby landmarks, with the expanded map showing the full world |  |

**Creator response**

> Fixed full map (recommended) — keep camp, both sides, and explored landmarks in stable positions so players can orient themselves at a glance

### 2. How should players open and close the expanded map?

| Option | Description | Selected |
| --- | --- | --- |
| Press M to toggle (recommended) | the map stays open until the player presses M again or closes it with Escape | Yes |
| Hold M for a glance | the map stays visible only while M is held and closes on release |  |

**Creator response**

> Press M to toggle (recommended) — the map stays open until the player presses M again or closes it with Escape

### 3. Should players be able to move while the expanded map is open? The world keeps running either way.

| Option | Description | Selected |
| --- | --- | --- |
| Block movement (recommended) | give the map the same input behavior as inventory and camp panels, including blocking attacks and quick-food keys | Yes |
| Allow movement | let players walk, sprint, and jump while viewing the map, while blocking attacks, quick-food keys, and world interactions |  |

**Creator response**

> Block movement (recommended) — give the map the same input behavior as inventory and camp panels, including blocking attacks and quick-food keys

### 4. How should the campaign clock show time until dawn or dusk? Both options show the current day, whether it is daylight or night, and the campaign stage.

| Option | Description | Selected |
| --- | --- | --- |
| Progress bar (recommended) | show how far the current phase has progressed at a glance |  |
| Exact countdown | show minutes and seconds until the next dawn or dusk |  |

**Creator response**

> No calldown - just sun on the background moving from left to righ from dusk to dawn

### 5. When should the sun move left to right in the game background, and what should happen at night?

| Option | Description | Selected |
| --- | --- | --- |
| Sun by day, moon by night | the sun crosses from dawn to dusk, then the moon crosses from dusk to dawn | Yes |
| Sun by day only | the sun crosses from dawn to dusk, with no moving sky marker at night |  |
| Sun at night | the sun crosses from dusk to dawn, as written |  |

**Creator response**

> Sun by day, moon by night — the sun crosses from dawn to dusk, then the moon crosses from dusk to dawn

### 6. Would you like to discuss anything else about the map and clock, or move on to retaliation warnings? The rally panel and outcome screens are still ahead.

| Option | Description | Selected |
| --- | --- | --- |
| Next area | discuss retaliation warnings | Yes |
| More on map and clock | revisit a choice or add a detail |  |

**Creator response**

> Next area — discuss retaliation warnings

---

## Retaliation warnings

### 1. When two portals fall together, where should players see the two retaliation warnings? Both choices keep each warning visible above open panels without taking input focus.

| Option | Description | Selected |
| --- | --- | --- |
| Separate top banners (recommended) | stack one prominent banner per destruction at the top center | Yes |
| Compact alert panel | show one row per destruction in a fixed panel beside the minimap |  |

**Creator response**

> Separate top banners (recommended) — stack one prominent banner per destruction at the top center

Superseded by the creator's later instruction to remove retaliation warnings entirely.

### 2. How long should a retaliation banner stay prominent? The warning lead is 10 seconds by default, and a busy portal may release the wave later.

| Option | Description | Selected |
| --- | --- | --- |
| Full warning lead (recommended) | keep each banner visible until its warning period ends, then dismiss it automatically |  |
| Brief banner, then compact alert | reduce each banner to a smaller visible alert after a few seconds and keep it there until the warning period ends |  |

**Creator response**

> I don't want a warning at all

### 3. With retaliation warnings removed, when should the wave become eligible to spawn? A portal that is already releasing another wave still finishes it first.

| Option | Description | Selected |
| --- | --- | --- |
| Keep the 10-second delay | the wave becomes eligible after the existing lead time, without a retaliation banner or sound |  |
| Make it eligible immediately | the wave can start as soon as portal destruction is resolved and the source is free | Yes |

**Creator response**

> Make it eligible immediately — the wave can start as soon as portal destruction is resolved and the source is free

### 4. What feedback should players get when a portal is destroyed, separate from any retaliation?

| Option | Description | Selected |
| --- | --- | --- |
| World feedback only | show the destruction and permanent ruins in the world, and update the map for players who already know the site | Yes |
| Add a local notice | also show a brief destruction message to players near the portal |  |
| Add a camp-wide notice | also show a brief destruction message to everyone, without revealing an unknown location |  |

**Creator response**

> World feedback only — show the destruction and permanent ruins in the world, and update the map for players who already know the site

### 5. Would you like to add anything about retaliation or portal destruction, or move on to the warrior rally panel? The victory and defeat screens are still ahead.

| Option | Description | Selected |
| --- | --- | --- |
| Next area | discuss the warrior rally panel | Yes |
| More on retaliation | revisit timing or destruction feedback |  |

**Creator response**

> Next area — discuss the warrior rally panel

---

## Warrior rally panel

### 1. How should the panel look when a player interacts with a side’s rally point?

| Option | Description | Selected |
| --- | --- | --- |
| Compact command panel (recommended) | keep most of the battlefield visible while showing the squad, target, price, and available action |  |
| Large camp-style panel | use a centered, semi-transparent window with more room for squad and order details |  |

**Creator response**

> what is it about? which panel???

Clarified the physical side rally, its price, and its send/recall actions.

### 2. How much of the screen should that rally panel take up when you interact with the rally point?

| Option | Description | Selected |
| --- | --- | --- |
| Compact window (recommended) | show the squad status and controls while keeping most of the battlefield visible |  |
| Larger camp-style window | give the squad and order details more room in a semi-transparent panel |  |

**Creator response**

> Could we make as a single button to rally squad and than to send squad to portal?
> Or do we need more menus?

Creator proposed a contextual rally button in place of a separate panel. The follow-up selected one press to select and dispatch the squad, with no assembly stage.

### 3. Should the rally button send the squad in one press or give players a preview first? Both options use the same button at the rally point.

| Option | Description | Selected |
| --- | --- | --- |
| One press to send (recommended) | show the squad and price when the player gets in range, then select and send the squad on one press |  |
| Two presses on the same button | the first press opens an inline preview, and the second presses Send. The squad stays on duty until Send succeeds. |  |

**Creator response**

> continue

Resumed the discussion and repeated this unresolved choice.

### 4. At the rally point, how many presses should it take to send the squad? Both choices use one contextual button.

| Option | Description | Selected |
| --- | --- | --- |
| One press | show the squad and price as you approach, then send on press | Yes |
| Two presses | preview the squad on the first press, then send on the second |  |

**Creator response**

> One press — show the squad and price as you approach, then send on press

### 5. How much squad detail should the rally prompt show before launch and while warriors are returning?

| Option | Description | Selected |
| --- | --- | --- |
| Counts and roles (recommended) | show stationed and ready counts, the planned melee and archer mix, and how many are returning | Yes |
| Include names | show the same counts and roles, plus the names of the warriors selected or returning |  |

**Creator response**

> Counts and roles (recommended) — show stationed and ready counts, the planned melee and archer mix, and how many are returning

### 6. When both rally points are in range near the refuge, how should the player choose which side to command?

| Option | Description | Selected |
| --- | --- | --- |
| Walk toward the side (recommended) | the prompt names the nearer rally, and an exact tie asks the player to step toward one side before sending |  |
| Choose left or right in the prompt | show a side selector only when both rallies are in range, then use the same Send button |  |

**Creator response**

> I though rally responsible for attacking portal from it's side. I don't understand why I need to choose this? Explain to me

Both rallies target their own side. Use direct interaction with the intended physical rally and identify it in the prompt. No extra side selection was chosen. Safe resolution of overlap remains a planning detail.

### 7. Is there anything else you want to discuss about the rally interaction, or should we move on to victory and defeat screens?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | discuss victory and defeat screens |  |
| More on rally | revisit the button or squad status | Yes |

**Creator response**

> More on rally — revisit the button or squad status

### 8. What would you like to revisit about the rally? You can also describe the interaction you have in mind.

| Option | Description | Selected |
| --- | --- | --- |
| Send and recall | when the button changes and what pressing it does | Yes |
| Squad readiness | what the prompt shows when the squad can’t launch |  |
| Returning squad | what players see while warriors make their way back |  |

**Creator response**

> Send and recall — when the button changes and what pressing it does

### 9. How should the Recall squad button work while the squad is out?

| Option | Description | Selected |
| --- | --- | --- |
| One press to recall (recommended) | press the same button once to start the return | Yes |
| Confirm recall | press once to see a confirmation in the prompt, then press again to start the return |  |

**Creator response**

> One press to recall (recommended) — press the same button once to start the return

### 10. What should the rally button show while the squad is returning?

| Option | Description | Selected |
| --- | --- | --- |
| Returning status (recommended) | replace the action with a disabled Returning label and the number still on the way | Yes |
| Disabled Send button | keep the price visible and explain that the returning squad is still occupying the rally |  |

**Creator response**

> Returning status (recommended) — replace the action with a disabled Returning label and the number still on the way

### 11. When the squad can’t launch, when should the rally prompt explain why? For example, only seven warriors are ready or the player is short on carried money.

| Option | Description | Selected |
| --- | --- | --- |
| Show the reason on approach (recommended) | display it beside the disabled Send action | Yes |
| Show the reason on press | keep the prompt shorter and explain the issue when the player tries to send |  |

**Creator response**

> Show the reason on approach (recommended) — display it beside the disabled Send action

### 12. Are you ready to move on to victory and defeat screens, or is there another rally detail you want to discuss?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | discuss victory and defeat screens | Yes |
| More on rally | revisit another detail |  |

**Creator response**

> Next area — discuss victory and defeat screens

---

## Victory and defeat screens

### 1. When the final portal falls and enemies are still alive, should the victory overlay let the player keep moving and fighting? The world keeps running either way.

| Option | Description | Selected |
| --- | --- | --- |
| Keep controls available (recommended) | show a victory card with Continue and Main menu, and let the player keep fighting outside the card | Yes |
| Give the overlay input focus | block the player’s movement and attacks until they press Continue, while the world keeps running |  |

**Creator response**

> Keep controls available (recommended) — show a victory card with Continue and Main menu, and let the player keep fighting outside the card

### 2. In LAN play, who should dismiss the victory card with Continue? The victory itself is recorded for the whole camp as soon as the last portal falls.

| Option | Description | Selected |
| --- | --- | --- |
| Each player dismisses their own card (recommended) | teammates can keep it open or close it independently |  |
| Host dismisses it for everyone | the host’s Continue closes the card on every screen | Yes |

**Creator response**

> Host dismisses it for everyone — the host’s Continue closes the card on every screen

### 3. What should the defeat screen tell players about why the run ended? If the camp already earned a victory, both options show that earlier result.

| Option | Description | Selected |
| --- | --- | --- |
| Show the cause (recommended) | show the day and whether the main building fell, no eligible survivors remained, or both happened together | Yes |
| Keep it brief | show Defeat and the day the run ended |  |

**Creator response**

> Show the cause (recommended) — show the day and whether the main building fell, no eligible survivors remained, or both happened together

### 4. If victory arrives while a player has inventory, the map, or the camp panel open, what should happen to that panel?

| Option | Description | Selected |
| --- | --- | --- |
| Keep the panel open (recommended) | show the victory card above it and leave the player’s current panel in place | Yes |
| Close the panel | bring the player back to the battlefield with the victory card visible |  |

**Creator response**

> Keep the panel open (recommended) — show the victory card above it and leave the player’s current panel in place

### 5. Are you ready for me to save the Phase 8 context and align its specification with your decisions?

| Option | Description | Selected |
| --- | --- | --- |
| Save the decisions (recommended) | write the phase context and discussion log and update the Phase 8 spec | Yes |
| More on outcomes | revisit the victory card or defeat screen |  |
| Explore another Phase 8 topic | discuss another decision before saving |  |

**Creator response**

> Save the decisions (recommended) — write the phase context and discussion log and update the Phase 8 spec

---

## Planning Discretion

Exact layout, localized copy, selection handling for overlapping physical interactions, and the guest treatment of host-only Continue are left to planning. The approved context records the required behavior.

## Deferred Ideas

No new backlog ideas were added. Phase 9 keeps its existing save, load, late-join and settings scope.
