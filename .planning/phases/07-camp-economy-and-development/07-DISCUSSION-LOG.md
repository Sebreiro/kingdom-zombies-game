# Phase 7 — Camp economy and development discussion log

> **Discussion record.** Use [07-CONTEXT.md](07-CONTEXT.md) as the planning input. This log preserves the offered alternatives and responses.

**Date** 2026-11-18 (Europe/Paris)

**Phase** 07-camp-economy-and-development

**Areas discussed** Main-building navigation, People and role assignment, Purchases and food sales, Work and safety feedback

The creator selected all four areas and later chose **Save Phase 7 context** after reviewing the decisions.

## Main-building navigation

### At the main building, how should players move between recruitment/jobs, food sales, and the main building’s own upgrades/repairs?

| Option | Description | Selected |
| --- | --- | --- |
| Tabbed camp window (Recommended) | People, Food sales, and Main building tabs, plus an Open stash button. | ✓ |
| Small action menu | choose an action, then open its separate panel; return to the menu to switch. |  |
| Single overview | put people, sales, and building sections on one scrollable screen. |  |

**Creator’s answer** Tabbed camp window (Recommended) — People, Food sales, and Main building tabs, plus an Open stash button.

The tabbed layout was selected. The later Stash-tab decision replaces the separate Open stash button.

### How much of the screen should the tabbed camp window occupy?

| Option | Description | Selected |
| --- | --- | --- |
| Wide centered window (Recommended) | enough room for the roster and details, with the world visible around its edges. |  |
| Side panel | leave most of the world visible and use a narrower, scrollable camp interface. |  |
| Almost full screen | use a semi-transparent workspace similar to inventory, with more room for camp information. | ✓ |

**Creator’s answer** Almost full screen — use a semi-transparent workspace similar to inventory, with more room for camp information.

### While the camp window is open, should it use the same control rules as inventory?

| Option | Description | Selected |
| --- | --- | --- |
| Same controls as inventory (Recommended) | close the camp window before moving, attacking, or using quick-food keys. | ✓ |
| Allow movement | keep movement keys active when no field is being edited; block attacks and quick-food keys, and close the window if main-building access is lost. |  |

**Creator’s answer** Same controls as inventory (Recommended) — close the camp window before moving, attacking, or using quick-food keys.

### When the player clicks Open stash from the camp window, what should happen after they close the stash?

| Option | Description | Selected |
| --- | --- | --- |
| Return to the camp tab (Recommended) | replace the camp view with the stash, then return to the same camp tab if main-building access is still valid. |  |
| Return to the game | opening the stash closes the camp window; closing the stash leaves the player back in the world. | ✓ |

**Creator’s answer** Return to the game — opening the stash closes the camp window; closing the stash leaves the player back in the world.

This was the choice for the separate-window flow at the time. The later Stash-tab decision superseded that flow.

### Would you like to discuss main-building navigation further, or move to People and role assignment?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | People and role assignment. |  |
| More questions | explore main-building navigation further. | ✓ |

**Creator’s answer** More questions — explore main-building navigation further.

### When the player reopens the camp window, which tab should appear?

| Option | Description | Selected |
| --- | --- | --- |
| People every time (Recommended) | use a predictable starting tab for recruitment and jobs. |  |
| Last-used tab | remember People, Food sales, or Main building within the current play session. |  |
| Custom response | See the creator's answer below. | ✓ |

**Creator’s answer** Stash every time. I think whole stash should be just as a separate tab instead of open above winodw

The custom response makes Stash the fourth tab and the initial tab on every opening. There is one camp window.

### When switching from Stash to People, Food sales, or Main building, should the player’s own equipment and carried storage stay visible?

| Option | Description | Selected |
| --- | --- | --- |
| Show only the selected tab (Recommended) | other camp tabs replace the inventory workspace and use the full window. | ✓ |
| Keep personal inventory visible | equipment and carried storage stay on the left, while the selected camp tab replaces the stash area on the right. |  |

**Creator’s answer** Show only the selected tab (Recommended) — other camp tabs replace the inventory workspace and use the full window.

### If nested bag windows are open in the Stash tab, what should happen when the player switches to another camp tab?

| Option | Description | Selected |
| --- | --- | --- |
| Close bag windows (Recommended) | remember their positions, but reopen them manually when returning to Stash, matching inventory’s existing reopening behavior. | ✓ |
| Hide and restore them | keep the bag views for this camp visit and show them again when returning to Stash, provided access is still valid. |  |

**Creator’s answer** Close bag windows (Recommended) — remember their positions, but reopen them manually when returning to Stash, matching inventory’s existing reopening behavior.

### If the controlled survivor takes nonlethal damage while any camp tab is open, should the camp window stay open?

| Option | Description | Selected |
| --- | --- | --- |
| Keep it open (Recommended) | show the damage cues over the window and let the player close it, matching inventory behavior. | ✓ |
| Close it immediately | return to the world on a nonlethal hit so movement and combat controls become available. |  |

**Creator’s answer** Keep it open (Recommended) — show the damage cues over the window and let the player close it, matching inventory behavior.

### Does that settle main-building navigation, or are there more details you want to discuss?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | People and role assignment | ✓ |
| More questions | stay with main-building navigation |  |

**Creator’s answer** Next area — People and role assignment

## People and role assignment

### How should the People tab organize the roster as the camp grows?

| Option | Description | Selected |
| --- | --- | --- |
| Group by job (Recommended) | show everyone in one scrollable list with a heading and count for each role. | ✓ |
| One list by name | keep survivors in one alphabetical list, with their role and activity on each row. |  |
| Group by availability | show survivors eligible for role changes first, then those away from the main building or controlled by a player. |  |

**Creator’s answer** Group by job (Recommended) — show everyone in one scrollable list with a heading and count for each role.

### After selecting a survivor in the roster, where should players see the available jobs, prices, and equipment requirements?

| Option | Description | Selected |
| --- | --- | --- |
| Details beside the roster (Recommended) | keep the list visible and show the selected survivor’s status and job choices on the right. | ✓ |
| Expand the selected row | show job choices and prices directly below that survivor. |  |
| Open a separate job dialog | show the selected survivor’s details and job choices over the roster. |  |

**Creator’s answer** Details beside the roster (Recommended) — keep the list visible and show the selected survivor’s status and job choices on the right.

### If the selected survivor is away from the main building or controlled by another player, what should the job panel show?

| Option | Description | Selected |
| --- | --- | --- |
| Show disabled job choices (Recommended) | keep prices and equipment requirements visible and explain why the survivor can’t change jobs right now. | ✓ |
| Show status only | explain why the survivor can’t change jobs and reveal the job choices once they’re eligible. |  |

**Creator’s answer** Show disabled job choices (Recommended) — keep prices and equipment requirements visible and explain why the survivor can’t change jobs right now.

### Where should Buy recruit appear in the People tab?

| Option | Description | Selected |
| --- | --- | --- |
| Fixed header above the roster (Recommended) | keep Buy recruit, its price, and both counts visible while scrolling or inspecting a survivor. | ✓ |
| Recruit card in the Unassigned group | put the price and recruitment limits where new recruits will appear. |  |
| Shared details area | show recruitment when no survivor is selected and job choices when one is selected. |  |

**Creator’s answer** Fixed header above the roster (Recommended) — keep Buy recruit, its price, and both counts visible while scrolling or inspecting a survivor.

### Would you like to refine the People tab further, or move to Purchases and food sales?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | Purchases and food sales | ✓ |
| More questions | stay with People and role assignment |  |

**Creator’s answer** Next area — Purchases and food sales

## Purchases and food sales

### How should players choose how much food to sell?

| Option | Description | Selected |
| --- | --- | --- |
| Exact amount with Max (Recommended) | type a whole number or use +/−, with Max to select all of that food. | ✓ |
| Exact amount only | type a whole number or use +/− without a shortcut to sell the whole stock. |  |
| Quick quantities | offer 1, Half, All, and a Custom option to enter another amount. |  |

**Creator’s answer** Exact amount with Max (Recommended) — type a whole number or use +/−, with Max to select all of that food.

### Once the Food sales tab shows the selected amount, proceeds, and resulting stash balance, should clicking Sell complete the sale or open a second confirmation?

| Option | Description | Selected |
| --- | --- | --- |
| Sell from the visible quote (Recommended) | one Sell click submits the displayed sale, including when the amount was set with Max. | ✓ |
| Confirm every sale | open a confirmation with the food, amount, and proceeds before submitting. |  |
| Confirm sales of all stock | ask for confirmation only when the sale would empty that food’s stock. |  |

**Creator’s answer** Sell from the visible quote (Recommended) — one Sell click submits the displayed sale, including when the amount was set with Max.

### Which purchases should ask for a second confirmation?

| Option | Description | Selected |
| --- | --- | --- |
| Confirm paid site work (Recommended) | ask before clearing, building, upgrading, repairing, or rebuilding. Recruit and change jobs directly from their priced buttons. |  |
| Confirm every purchase | ask before recruiting, changing jobs, or starting paid site work. |  |
| Purchase from the visible price | submit any purchase directly from its priced button. | ✓ |

**Creator’s answer** Purchase from the visible price — submit any purchase directly from its priced button.

### Suppose you select 7 berries, but a teammate takes some and only 4 remain. The game shows the updated stock and sells nothing. What should the Food sales tab do with your selected quantity?

| Option | Description | Selected |
| --- | --- | --- |
| Keep 7 visible and explain (Recommended) | disable Sell until you enter a valid amount or press Max again. |  |
| Adjust to 4 and explain | update the visible quote to the current maximum and require a new Sell click. |  |
| Clear the quantity and explain | let the player enter the amount again from scratch. |  |
| Custom response | See the creator's answer below. | ✓ |

**Creator’s answer** Update in real time. Always seeing acutal price and quantity. If it changed afteer button pressed - show error with explanation

Show live stock and prices. If a clicked sale fails because conditions changed, explain why and show the updated values.

### If the update arrives before Sell is pressed and only 4 of the 7 selected berries remain, what should the quantity field show?

| Option | Description | Selected |
| --- | --- | --- |
| Keep 7 as an invalid entry (Recommended) | show that only 4 are available and disable Sell until the player adjusts the amount. |  |
| Reduce it to 4 | update the quantity and proceeds on screen so a later Sell click submits 4. | ✓ |

**Creator’s answer** Reduce it to 4 — update the quantity and proceeds on screen so a later Sell click submits 4.

Apply the reduction when the new stock is visible before the click. The sale already submitted in a race still gets checked against the clicked quote.

### Would you like to discuss purchases and food sales further, or move to the final area, Work and safety feedback?

| Option | Description | Selected |
| --- | --- | --- |
| Next area | Work and safety feedback | ✓ |
| More questions | stay with Purchases and food sales |  |

**Creator’s answer** Next area — Work and safety feedback

## Work and safety feedback

### What should players see above a site with a paid order while they’re walking through camp?

| Option | Description | Selected |
| --- | --- | --- |
| Compact cue near the site (Recommended) | show a small work bar or waiting marker as the player approaches. | ✓ |
| Cue only on the selected site | show progress when the player targets the site, before opening its panel. |  |
| World changes only | show workers, frames, and damaged structures in the world. Keep progress and waiting markers inside the site panel. |  |

**Creator’s answer** Compact cue near the site (Recommended) — show a small work bar or waiting marker as the player approaches.

### What should the nearby cue show during a paid repair?

| Option | Description | Selected |
| --- | --- | --- |
| Show live health (Recommended) | mark the bar as a paid repair and let it rise or fall with actual health until the structure is full. | ✓ |
| Show a repair icon | use an active or waiting marker in the world and keep the exact health and full-health goal in the site panel. |  |

**Creator’s answer** Show live health (Recommended) — mark the bar as a paid repair and let it rise or fall with actual health until the structure is full.

### When a nearby civilian stops working because of danger or nightfall, should the world display a cue on that survivor?

| Option | Description | Selected |
| --- | --- | --- |
| Brief cue when work stops (Recommended) | show a short danger or night marker, then let the retreat animation carry the action. | ✓ |
| Cue only when inspecting the survivor | show the current activity and reason while the survivor is targeted. |  |
| Animations and People tab | show the retreat in the world and keep text or markers in the roster. |  |

**Creator’s answer** Brief cue when work stops (Recommended) — show a short danger or night marker, then let the retreat animation carry the action.

### How much should the nearby site marker tell the player when a paid order is waiting?

| Option | Description | Selected |
| --- | --- | --- |
| Marker only (Recommended) | keep the world clear and reveal the reason when the player opens the site panel. | ✓ |
| Marker with a short reason | show a label such as Waiting for worker, Unsafe, or Nightfall as the player approaches. |  |

**Creator’s answer** Marker only (Recommended) — keep the world clear and reveal the reason when the player opens the site panel.

### Would you like to explore work and safety feedback further, or review the decisions from all four areas?

| Option | Description | Selected |
| --- | --- | --- |
| Review decisions | finish this area and review everything we discussed | ✓ |
| More questions | stay with work and safety feedback |  |

**Creator’s answer** Review decisions — finish this area and review everything we discussed

## Planning discretion

The creator selected the product behavior above. The context leaves layout dimensions, ordering within each role, marker art, and similar details to planning.

## Deferred Ideas

The discussion added no backlog items.
