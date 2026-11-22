# Phase 6: Survivors, inventory, and combat - Discussion Log

> **Audit trail only.** Do not use as input to planning, research or execution agents.
> Decisions are captured in [06-CONTEXT.md](06-CONTEXT.md); this log preserves all alternatives and custom answers.

**Date:** 2026-11-16–2026-11-17 (Europe/Paris)
**Phase:** 06-survivors-inventory-and-combat
**Areas discussed:** Inventory layout and nested bags; Stash and nearby looting; Combat HUD and feedback; Death, handover and spectating; Actions while inventory is open; Bow aiming and ammunition cues; Recovery and load feedback
**Substantive questions:** 48

## Discussion Flow

- Initially selected inventory, then explicitly changed selection to all four offered areas.
- Inventory received four rounds of four questions; the creator requested more after rounds 1–3, then moved on.
- Stash/looting received two rounds; filtering was explicitly moved to Phase 12 during its second question.
- Combat HUD received one round; death/spectating received two rounds, including explicit Respawn timing, queued requests and teammate-follow clarification.
- At the first complete review (36 answers), the creator chose Explore more gray areas. All three newly offered areas were selected with “all of this”. Each received one four-question round.
- After the expanded 48-answer review, the creator selected “I’m ready for context — create the final decision record for Phase 6 research and planning.”

## Inventory layout and nested bags

### D-01 — Question 1

**Question:** How should your own inventory be arranged at 1280×720? All options include the character silhouette, eleven equipment slots, both hands, and the actual storage supplied by worn items.

| Option | Description | Selected |
| --- | --- | --- |
| Equipment left, storage right (Recommended) | silhouette and equipped items on the left; labeled pocket and bag grids on the right, visible together. | ✓ |
| Equipment in the center | slots around the silhouette, with pocket and bag grids arranged on either side. |  |
| Equipment and storage tabs | a compact panel with separate views, leaving more of the world visible but requiring tab switching. |  |

**User’s answer:** Equipment left, storage right — silhouette and equipped items on the left; labeled pocket and bag grids on the right, visible together.

### D-02 — Question 2

**Question:** When you open a bag inside another container, how should its contents appear?

| Option | Description | Selected |
| --- | --- | --- |
| Expand in the storage column (Recommended) | reveal a labeled child grid below its parent; keep other open grids available and scroll when needed. |  |
| Navigate into the bag | replace the current storage view with its contents, with a breadcrumb path to return to parent containers. |  |
| Open a movable window | give each opened container its own window for arranging several grids side by side. | ✓ |

**User’s answer:** Open a movable window — give each opened container its own window for arranging several grids side by side.

### D-03 — Question 3

**Question:** What placement guidance should appear while you drag an item between equipment slots and storage grids?

| Option | Description | Selected |
| --- | --- | --- |
| Clear fit preview (Recommended) | show the item’s cell footprint, mark valid/invalid placement, and highlight compatible equipment slots; explain a rejected drop. | ✓ |
| Live explanations | show the fit preview and compatible slots, plus a short reason while hovering an invalid destination, such as “Occupied” or “Too deep.” |  |
| Minimal preview | show the item outline while dragging and explain any failure when you release it. |  |

**User’s answer:** Clear fit preview — show the item’s cell footprint, mark valid/invalid placement, and highlight compatible equipment slots; explain a rejected drop.

### D-04 — Question 4

**Question:** How should you assign a carried food stack to quick slots 1–4? The slot will always reference the actual stack, as required by the specification.

| Option | Description | Selected |
| --- | --- | --- |
| Drag and item menu (Recommended) | drag food onto a quick slot, or use an “Assign to quick slot” item-menu action. | ✓ |
| Drag only | drag food from an open storage grid onto one of the four quick slots. |  |
| Choose from the quick slot | click a quick slot, then select a carried food stack from a compact list. |  |

**User’s answer:** Drag and item menu — drag food onto a quick slot, or use an “Assign to quick slot” item-menu action.

### D-05 — Question 5

**Question:** How should you deliberately drop an item onto the ground? Dropping a filled bag or garment keeps all its contents together.

| Option | Description | Selected |
| --- | --- | --- |
| Item-menu Drop action (Recommended) | choose “Drop” from the item’s menu; dragging outside a grid simply cancels. | ✓ |
| Labeled Drop target | drag the item onto a clearly marked drop area within the inventory interface. |  |
| Both methods | provide the item-menu action and the labeled drop target. |  |

**User’s answer:** Item-menu Drop action — choose “Drop” from the item’s menu; dragging outside a grid simply cancels.

### D-06 — Question 6

**Question:** How should the stack-split quantity chooser work after Shift-drag? It will show how many units are taken and how many remain.

| Option | Description | Selected |
| --- | --- | --- |
| Number field with Half shortcut (Recommended) | type an exact amount or choose half, then confirm. | ✓ |
| Slider and number field | drag to an approximate amount, adjust the exact number if needed, then confirm. |  |
| Number field only | a compact exact-quantity prompt with confirm and cancel. |  |

**User’s answer:** Number field with Half shortcut — type an exact amount or choose half, then confirm.

### D-07 — Question 7

**Question:** If a bag and a bag inside it are both open, what should happen when you close the outer bag’s window? This only changes the view; items stay where they are.

| Option | Description | Selected |
| --- | --- | --- |
| Keep child windows open (Recommended) | leave the inner bag open, with its owner and container path visible in the title. |  |
| Close child windows too | closing the outer bag also closes windows for everything nested inside it. | ✓ |
| Minimize the outer bag | collapse it to a small title bar while keeping its child windows open. |  |

**User’s answer:** Close child windows too — closing the outer bag also closes windows for everything nested inside it.

### D-08 — Question 8

**Question:** Should movement controls still work while inventory panels are open?

| Option | Description | Selected |
| --- | --- | --- |
| Keep movement available (Recommended) | walk, sprint and jump while browsing; text fields and inventory gestures take priority over the inputs they use. |  |
| Stop movement until panels close | inventory takes over your controls, so you must close it before moving; enemies and the rest of the world continue. | ✓ |

**User’s answer:** Stop movement until panels close — inventory takes over your controls, so you must close it before moving; enemies and the rest of the world continue.

### D-09 — Question 9

**Question:** Where should detailed item information—such as weight, condition, stack limit and storage size—appear?

| Option | Description | Selected |
| --- | --- | --- |
| Hover tooltip (Recommended) | point at an item to see its details beside the cursor, keeping the grids compact. | ✓ |
| Fixed details panel | select an item to show its information in a dedicated area of the inventory. |  |
| Both | show a quick tooltip on hover and keep selected-item details in a dedicated panel. |  |

**User’s answer:** Hover tooltip — point at an item to see its details beside the cursor, keeping the grids compact.

### D-10 — Question 10

**Question:** How should equipment condition appear in inventory grids without hovering? Each item keeps one canonical icon; broken equipment will always have a clear marker.

| Option | Description | Selected |
| --- | --- | --- |
| Slim condition bar (Recommended) | a small bar shows remaining condition; the tooltip gives exact values. | ✓ |
| Numeric condition | show current and maximum condition as compact numbers on the icon. |  |
| Broken marker only | show a marker once unusable; use the tooltip to inspect condition before it breaks. |  |

**User’s answer:** Slim condition bar — a small bar shows remaining condition; the tooltip gives exact values.

### D-11 — Question 11

**Question:** Where should a bag window appear when you first open it? You can move it afterward in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Beside its source window (Recommended) | open next to the grid you clicked, keeping the whole window on screen where possible. | ✓ |
| Cascade from one corner | open each new window slightly offset from the previous one, making the stack easy to find. |  |
| Centered on screen | open in a predictable central position, then arrange it manually. |  |

**User’s answer:** Beside its source window — open next to the grid you clicked, keeping the whole window on screen where possible.

### D-12 — Question 12

**Question:** Should equipping support an item-menu action as well as dragging onto a slot? Occupied slots still require manual unequipping first.

| Option | Description | Selected |
| --- | --- | --- |
| Drag and Equip menu (Recommended) | drag onto a slot, or choose “Equip”; choose left/right when either hand is valid. |  |
| Drag only | equip items by dragging them onto the intended equipment slot. | ✓ |

**User’s answer:** Drag only — equip items by dragging them onto the intended equipment slot.

### D-13 — Question 13

**Question:** How much screen space should the main inventory panel occupy at 1280×720? The world continues running, and damage/status feedback remains visible in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Large centered panel (Recommended) | give equipment and storage ample room while leaving the world visible around the edges. |  |
| Compact side panel | leave more of the world visible, accepting more scrolling within storage. |  |
| Almost full screen | maximize space for equipment, grids and bag windows, with damage/status cues kept visible over the interface. | Selected with clarification |

**User’s answer:** 3rd option - almost full screen, but we can make background semi transparent to see what going on in the world

**Notes:** Selected with added semi-transparent background.

### D-14 — Question 14

**Question:** When you press Escape while dragging an item, what should happen first? The item stays at its source in both options.

| Option | Description | Selected |
| --- | --- | --- |
| Cancel the drag only (Recommended) | return to browsing; another Escape closes the active inventory window. | ✓ |
| Cancel and close the active window | one Escape cancels the drag and closes the window you were using. |  |

**User’s answer:** Cancel the drag only — return to browsing; another Escape closes the active inventory window.

### D-15 — Question 15

**Question:** When you close and reopen inventory during the same play session, how should bag windows return? Only containers you still have permission to access can reopen.

| Option | Description | Selected |
| --- | --- | --- |
| Restore open windows and positions (Recommended) | return to the same accessible bags where you left them. |  |
| Remember positions, keep bags closed | reopen only the main inventory; bags return to their previous positions when opened manually. | ✓ |
| Start with a clean layout | reopen the main inventory with all extra bag windows closed and reset their positions. |  |

**User’s answer:** Remember positions, keep bags closed — reopen only the main inventory; bags return to their previous positions when opened manually.

### D-16 — Question 16

**Question:** How should you open a bag or other container from an inventory grid or equipment slot?

| Option | Description | Selected |
| --- | --- | --- |
| Double-click or item-menu Open (Recommended) | double-click for speed, with an explicit “Open” action also available in the item menu. |  |
| Item-menu Open only | choose “Open” from the item’s menu, keeping ordinary clicks and drags free of opening actions. | ✓ |
| Small Open button | show a dedicated button on container items for opening their contents. |  |

**User’s answer:** Item-menu Open only — choose “Open” from the item’s menu, keeping ordinary clicks and drags free of opening actions.

## Stash and nearby looting

### D-17 — Question 1

**Question:** When you open the stash, an NPC inventory or a corpse, how should it sit alongside your own inventory? The almost full-screen, semi-transparent treatment carries forward.

| Option | Description | Selected |
| --- | --- | --- |
| Fixed side-by-side layout (Recommended) | your equipment on the far left, carried storage in the middle, and the external inventory on the right; nested bags still use movable windows. | ✓ |
| Movable external window | keep your own equipment-left/storage-right layout, and open the stash or loot target in a separate movable window. |  |

**User’s answer:** Fixed side-by-side layout — your equipment on the far left, carried storage in the middle, and the external inventory on the right; nested bags still use movable windows.

### D-18 — Question 2

**Question:** When you filter the shared stash by name or type, how should matching items be presented? Filtering will never move items from their actual cells.

| Option | Description | Selected |
| --- | --- | --- |
| Results list across all pages (Recommended) | show matching items with their page numbers; selecting one opens its original page and highlights it. |  |
| Filtered page grids | keep the normal grid view, dim nonmatching items, and navigate between pages containing matches. |  |
| Results list beside the page grid | keep matches across all pages and the selected item’s actual grid visible together, using more panel space. |  |

**User’s answer:** let's move filtering to the phase 12. No filtering for stash and inventory now

**Notes:** Explicit creator scope amendment supersedes locked filtering delivery. No offered filter layout was selected. Phase 12 added with INV-14; INV-10 and Phase 6 R12/AC-18/AC-33 retain unfiltered pages/navigation/transfers. Historical E074 remains visible as deferred. Roadmap, requirements, project, source-spec timing and state notes updated. Creator explicitly defers filtering to Phase 12 instead of selecting a filter-results layout.

### D-19 — Question 3

**Question:** When several nearby NPCs, corpses or loot piles overlap, where should their target selector appear? Choosing a target then opens its contents in the external inventory panel.

| Option | Description | Selected |
| --- | --- | --- |
| Small list beside your survivor (Recommended) | show target names/types and distances near the character, highlighting each target as you point at its row. |  |
| List inside the external inventory panel | open the inventory layout with a target list on the right, then select whose contents to view. | ✓ |

**User’s answer:** List inside the external inventory panel — open the inventory layout with a target list on the right, then select whose contents to view.

### D-20 — Question 4

**Question:** How should failed transfers and lost inventory access be explained? The view will always refresh to the current authoritative contents.

| Option | Description | Selected |
| --- | --- | --- |
| Message in the affected panel (Recommended) | show a short explanation there; if access closes the panel, show a brief HUD notice instead. | ✓ |
| HUD notices only | show short messages over the game without adding an error area inside inventory panels. |  |
| Dialog requiring dismissal | explain the failure in a dialog you must close before continuing; the world still runs. |  |

**User’s answer:** Message in the affected panel — show a short explanation there; if access closes the panel, show a brief HUD notice instead.

### D-21 — Question 5

**Question:** When you Ctrl-click an external item to take it, which carried storage should quick transfer use? All placements must still obey capacity, nesting and equipment rules.

| Option | Description | Selected |
| --- | --- | --- |
| Any carried storage, stack first (Recommended) | fill compatible carried stacks, then use a legal space in your pockets or bags. | ✓ |
| Selected container only | put it in the carried container you have selected; explain if that container cannot accept it. |  |
| Ask for a destination each time | choose the receiving pocket or bag for every quick transfer. |  |

**User’s answer:** Any carried storage, stack first — fill compatible carried stacks, then use a legal space in your pockets or bags.

### D-22 — Question 6

**Question:** How should you browse the stash’s unlimited pages?

| Option | Description | Selected |
| --- | --- | --- |
| Previous/Next and page-number jump (Recommended) | show the current page and total, with arrows and a field for jumping directly to a page. | ✓ |
| Scrollable page tabs | click numbered tabs above the current grid and scroll the tab strip when there are many pages. |  |
| Stacked page grids | scroll vertically through labeled pages, keeping each item in its actual grid. |  |

**User’s answer:** Previous/Next and page-number jump — show the current page and total, with arrows and a field for jumping directly to a page.

### D-23 — Question 7

**Question:** If a quick deposit places items on another stash page, should the view follow the deposit?

| Option | Description | Selected |
| --- | --- | --- |
| Keep the current page (Recommended) | show a brief deposit confirmation with the affected page numbers and a control to go there. |  |
| Follow the deposit | switch to the first affected page and highlight the changed cells; indicate any other affected pages. | ✓ |

**User’s answer:** Follow the deposit — switch to the first affected page and highlight the changed cells; indicate any other affected pages.

### D-24 — Question 8

**Question:** If the selected NPC, corpse or loot pile becomes inaccessible while other permitted targets are still nearby, what should the interface do? Its contents and related bag windows will close safely.

| Option | Description | Selected |
| --- | --- | --- |
| Return to the target list (Recommended) | keep your own inventory open and show the remaining nearby choices, with the reason access was lost. | ✓ |
| Close the external panel | keep your own inventory open; use Interact again when you want to choose another target. |  |

**User’s answer:** Return to the target list — keep your own inventory open and show the remaining nearby choices, with the reason access was lost.

## Combat HUD and feedback

### D-25 — Question 1

**Question:** How should the combat HUD arrange health, stamina, carried money/load, hands, arrows and quick food slots?

| Option | Description | Selected |
| --- | --- | --- |
| Top-left vitals, bottom action bar (Recommended) | health/stamina and money/load at the top left; hands, arrows and food slots along the bottom. | ✓ |
| Single bottom strip | group all of these readouts in one strip across the bottom of the screen. |  |
| Lower-corner clusters | health/stamina and money/load at the bottom left; hands, arrows and food slots at the bottom right. |  |

**User’s answer:** Top-left vitals, bottom action bar — health/stamina and money/load at the top left; hands, arrows and food slots along the bottom.

### D-26 — Question 2

**Question:** What visual cue should accompany damage to your survivor? The existing character flinch carries forward in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Brief edge tint and health-bar pulse (Recommended) | a restrained screen-edge cue and emphasized health loss remain noticeable over open panels. | ✓ |
| HUD warning and health-bar pulse | show a short damage warning near the vitals and emphasize the health loss. |  |
| Health-bar pulse only | keep the visual cue concentrated on the health readout, alongside the character’s flinch. |  |

**User’s answer:** Brief edge tint and health-bar pulse — a restrained screen-edge cue and emphasized health loss remain noticeable over open panels.

### D-27 — Question 3

**Question:** Where should the explanation for a blocked attack appear? Failed attack presses remain discarded rather than queued, as already specified.

| Option | Description | Selected |
| --- | --- | --- |
| Beside the action bar (Recommended) | show a short reason near the hands/ammunition display and emphasize the relevant readout. | ✓ |
| Brief central HUD notice | show the reason in a consistent notice area nearer the center of the screen. |  |
| Beside the survivor | show a small temporary reason near your character in the world. |  |

**User’s answer:** Beside the action bar — show a short reason near the hands/ammunition display and emphasize the relevant readout.

### D-28 — Question 4

**Question:** How should attack and food cooldowns appear in the action bar? Food slots will show the same shared survivor cooldown, rather than separate timers.

| Option | Description | Selected |
| --- | --- | --- |
| Shaded sweep over the relevant icons (Recommended) | an overlay clears as the action becomes ready. | ✓ |
| Small progress strip under the icons | show readiness with a compact bar below each relevant readout. |  |
| Numeric countdown | show the remaining seconds on the relevant icons until ready. |  |

**User’s answer:** Shaded sweep over the relevant icons — an overlay clears as the action becomes ready.

## Death, handover and spectating

### D-29 — Question 1

**Question:** At the one-second handover after death, how should the camera move to the randomly assigned survivor’s actual location?

| Option | Description | Selected |
| --- | --- | --- |
| Smooth nearby, cut when distant (Recommended) | use a short smooth transition for a nearby successor and a direct cut for a large jump. | Selected with clarification |
| Always cut immediately | switch directly to the successor when control transfers. |  |
| Brief fade between locations | use a short fade to bridge the move to the successor. |  |

**User’s answer:** wait for user to press "respawn" button and than first option

**Notes:** Creator selects smooth-nearby/cut-distant camera and explicitly changes the automatic succession rule to require a Respawn button. Follow-up questions below resolve timing and queued requests. Selected camera option with explicit Respawn amendment; timing/queue resolved in the next answers.

### D-30 — Question 2

**Question:** How should the one-second succession delay work with the Respawn button?

| Option | Description | Selected |
| --- | --- | --- |
| Enable after one second (Recommended) | after death, wait one second, then enable Respawn; clicking it transfers control immediately if a survivor is available. | ✓ |
| Start the delay on click | Respawn is available immediately, and clicking it starts the one-second wait before control transfers. |  |

**User’s answer:** Enable after one second — after death, wait one second, then enable Respawn; clicking it transfers control immediately if a survivor is available.

### D-31 — Question 3

**Question:** If no eligible survivor is available after that second, how should Respawn work?

| Option | Description | Selected |
| --- | --- | --- |
| Click to join the waiting queue (Recommended) | pressing Respawn records your request; you can spectate, and control transfers automatically when a survivor becomes available in queue order. | ✓ |
| Wait for availability, then click | keep Respawn disabled until a survivor is available for your turn, then require your click before handing over control. |  |

**User’s answer:** Click to join the waiting queue — pressing Respawn records your request; you can spectate, and control transfers automatically when a survivor becomes available in queue order.

**Notes:** Creator separately clarifies: 'wiat. you can spectate for othere players' and 'instead of free camera spectating'. Capture teammate-follow spectating, not a free camera.

### D-32 — Question 4

**Question:** How should you switch between living teammates while spectating? The camera follows the selected teammate, and your Respawn/waiting status stays visible.

| Option | Description | Selected |
| --- | --- | --- |
| Previous/Next plus teammate list (Recommended) | cycle with buttons or choose a particular teammate from a compact list. | ✓ |
| Clickable teammate roster | select the player you want to follow directly from the roster. |  |
| Previous/Next buttons only | cycle through living teammates, with the followed player’s name shown. |  |

**User’s answer:** Previous/Next plus teammate list — cycle with buttons or choose a particular teammate from a compact list.

### D-33 — Question 5

**Question:** After your survivor dies, what should the camera show before you press Respawn, if a living teammate is available?

| Option | Description | Selected |
| --- | --- | --- |
| Follow a teammate automatically (Recommended) | enter teammate-follow spectating with Respawn and switching controls visible. |  |
| Stay at the death location | keep a fixed view there until you choose a teammate to follow or press Respawn. | ✓ |

**User’s answer:** Stay at the death location — keep a fixed view there until you choose a teammate to follow or press Respawn.

### D-34 — Question 6

**Question:** If the teammate you are following dies and another living teammate is available, what should spectating do?

| Option | Description | Selected |
| --- | --- | --- |
| Follow the next living teammate (Recommended) | switch automatically and show the newly followed player’s name. |  |
| Hold the current view | keep the camera fixed there and let you choose another teammate from the controls. | ✓ |

**User’s answer:** Hold the current view — keep the camera fixed there and let you choose another teammate from the controls.

### D-35 — Question 7

**Question:** What waiting information should appear after you request Respawn and no survivor is available?

| Option | Description | Selected |
| --- | --- | --- |
| Your queue position and status (Recommended) | show “Waiting for a survivor” and your current place in the queue. |  |
| Waiting status only | keep a simple “Waiting for a survivor” message. | ✓ |
| Full waiting-player list | show the queued players in order, with your entry highlighted. |  |

**User’s answer:** Waiting status only — keep a simple “Waiting for a survivor” message.

### D-36 — Question 8

**Question:** When Respawn completes, how should the game make the new controlled survivor clear? The camera, crown and HUD will always update to that survivor’s actual state.

| Option | Description | Selected |
| --- | --- | --- |
| Brief handover banner (Recommended) | show a short “Control transferred” message as the camera and HUD change. | ✓ |
| Camera, crown and HUD only | let those updates identify the handover without an extra message. |  |
| Brief world marker | emphasize the new controlled survivor with a short marker as the camera and HUD change. |  |

**User’s answer:** Brief handover banner — show a short “Control transferred” message as the camera and HUD change.

## Actions while inventory is open

### D-37 — Question 1

**Question:** Should you be able to attack while any inventory, stash or loot panel is open? Panel clicks and item drags must always take priority over world actions.

| Option | Description | Selected |
| --- | --- | --- |
| Block attacks until panels close (Recommended) | inventory takes over combat input as well as movement. | ✓ |
| Allow attacks in uncovered world areas | clicking outside every panel can attack when no drag or menu interaction is active; the semi-transparent panels never pass clicks through. |  |

**User’s answer:** Block attacks until panels close — inventory takes over combat input as well as movement.

### D-38 — Question 2

**Question:** Should quick-food keys 1–4 work while inventory panels are open? Food still uses the actual carried stack and the shared personal cooldown.

| Option | Description | Selected |
| --- | --- | --- |
| Allow them except while typing (Recommended) | you can eat while browsing; quantity fields and other text input take priority over number keys. |  |
| Block them until panels close | keep quick-food keys inactive during inventory management. | ✓ |

**User’s answer:** Block them until panels close — keep quick-food keys inactive during inventory management.

### D-39 — Question 3

**Question:** Should your carried food have an Eat action in its item menu, allowing deliberate use while inventory is open? Food in the stash or another character’s inventory must be transferred to you first.

| Option | Description | Selected |
| --- | --- | --- |
| Add Eat to the item menu (Recommended) | consume one carried unit through the menu under the normal food and cooldown rules. | Selected with clarification |
| Use food through quick slots only | assign food while inventory is open, then close the panels to use it. |  |

**User’s answer:** add "consume" action

**Notes:** Creator accepts carried-food menu consumption and explicitly names the action Consume. Do not retain the offered Eat label. Existing actual-stack, full-stat consumption and personal cooldown rules apply. Selected menu consumption with action renamed Consume.

### D-40 — Question 4

**Question:** How should inventory panels react when your survivor takes nonlethal damage?

| Option | Description | Selected |
| --- | --- | --- |
| Keep panels open (Recommended) | show the agreed damage cues and preserve your current inventory interaction so you decide when to close it. | ✓ |
| Close inventory automatically | cancel any drag safely at its source, close the panels, and return control to gameplay. |  |

**User’s answer:** Keep panels open — show the agreed damage cues and preserve your current inventory interaction so you decide when to close it.

## Bow aiming and ammunition cues

### D-41 — Question 1

**Question:** What aiming indicator should appear while you use a bow?

| Option | Description | Selected |
| --- | --- | --- |
| Small crosshair at the cursor (Recommended) | a clear aiming point that keeps the world view uncluttered. | ✓ |
| Crosshair and short aim line | show the cursor crosshair plus a short line from the bow indicating its aim direction. |  |
| Bow pose and normal cursor only | let the drawn bow and regular cursor communicate the aim. |  |

**User’s answer:** Small crosshair at the cursor — a clear aiming point that keeps the world view uncluttered.

### D-42 — Question 2

**Question:** What should the bow’s ammunition readout show? Arrows still require manual refilling of the opposite hand.

| Option | Description | Selected |
| --- | --- | --- |
| Hand count and carried reserves (Recommended) | show arrows ready in the hand and the separate reserve total carried in pockets or bags. |  |
| Hand count only | show arrows ready to fire; inspect carried reserves in inventory. | ✓ |
| Hand count with reserves in a tooltip | keep the main readout compact and reveal the reserve total on hover. |  |

**User’s answer:** Hand count only — show arrows ready to fire; inspect carried reserves in inventory.

### D-43 — Question 3

**Question:** What should happen in the HUD when the arrow hand becomes empty?

| Option | Description | Selected |
| --- | --- | --- |
| Empty-hand marker and refill hint (Recommended) | mark the hand as empty and briefly show “Refill arrow hand” beside the action bar. |  |
| Empty-hand marker only | keep a clear empty marker alongside the zero arrow count. |  |
| Zero count, then explain blocked shots | show zero arrows and use the agreed action-bar explanation if you try to fire. |  |

**User’s answer:** Just zero cound and no arrow in the bow. No additional hint of explanation

**Notes:** Explicit custom answer: show only zero ammunition and an unloaded bow with no visible arrow. Add no extra empty-ammo marker, hint or blocked-shot explanation. This is a specific exception to general blocked-attack action-bar messages; do not reinterpret the answer as selecting the third offered option. Other attack failure feedback remains as previously chosen. Custom response; none of the offered options was selected as written. No empty-ammo hints/explanations.

### D-44 — Question 4

**Question:** When the arrow hand is empty, should the survivor keep aiming the empty bow or lower it? Both options show no arrow and add no messages.

| Option | Description | Selected |
| --- | --- | --- |
| Keep aiming the empty bow (Recommended) | the bow continues following the cursor, visibly unloaded. | ✓ |
| Lower it to a carry pose | return to the aiming pose when real arrows are loaded into the opposite hand. |  |

**User’s answer:** Keep aiming the empty bow — the bow continues following the cursor, visibly unloaded.

## Recovery and load feedback

### D-45 — Question 1

**Question:** How should the HUD show the five-second safe-rest qualification before natural health recovery starts? The recovery conditions and timing are already fixed.

| Option | Description | Selected |
| --- | --- | --- |
| Small progress indicator near health (Recommended) | show qualification building up, then change to an active-recovery indicator. |  |
| Short status label | show whether you are unsafe, resting or recovering, without a progress display. | ✓ |
| Health bar only | let the rising health bar show recovery once it starts, with detailed rules available in a tooltip. |  |

**User’s answer:** Short status label — show whether you are unsafe, resting or recovering, without a progress display.

### D-46 — Question 2

**Question:** How much carried-load information should be visible in the HUD?

| Option | Description | Selected |
| --- | --- | --- |
| Weight and a short state (Recommended) | show current weight and Normal/Heavy/Overloaded; put exact movement and stamina penalties in a tooltip. |  |
| Weight, state and exact penalties | also show the current movement-speed and stamina-cost modifiers directly in the HUD. | ✓ |

**User’s answer:** Weight, state and exact penalties — also show the current movement-speed and stamina-cost modifiers directly in the HUD.

### D-47 — Question 3

**Question:** Should stamina’s recovery delay and regeneration have a separate visual cue? The half-second delay and recovery rate remain as specified.

| Option | Description | Selected |
| --- | --- | --- |
| Stamina bar only (Recommended) | show spending and refilling through the bar itself. | ✓ |
| Small stamina-status indicator | add a compact cue beside the bar for waiting to recover and actively regenerating. |  |

**User’s answer:** Stamina bar only — show spending and refilling through the bar itself.

### D-48 — Question 4

**Question:** How should the HUD explain a sprint or jump blocked by load or insufficient stamina?

| Option | Description | Selected |
| --- | --- | --- |
| Short reason beside the vitals (Recommended) | show the reason near the stamina/load readout and emphasize the relevant value. | ✓ |
| Use the action-bar notice area | put the reason in the same place as other blocked-action explanations. |  |
| Emphasize the readout only | draw attention to the relevant stamina or load value without an extra text message. |  |

**User’s answer:** Short reason beside the vitals — show the reason near the stamina/load readout and emphasize the relevant value.

## Agent's Discretion

No general “you decide” answer was selected. CONTEXT.md identifies the remaining routine implementation details without presenting unselected recommendations as creator approvals.

## Deferred Ideas

Stash/inventory filtering is explicitly deferred to Phase 12 under INV-14. Earlier unfiltered pages and transfers remain required. No filter-results layout was selected.

## Record Integrity

The log retains all 48 substantive questions, their complete offered options and the actual selected/custom responses. The empty-ammo answer is not silently matched to an option that adds an explanation. The explicit Respawn amendment and the no-free-camera clarification are retained. This is a discussion record, not implementation or acceptance evidence.

## Record Verification

- All 48 context decision IDs and audit question IDs are sequential and present, with 48 user answers, seven completed discussion areas and all required context sections.
- The final records decode as strict UTF-8; all 17 canonical files and authored artifact links resolve. The roadmap validates without warnings, and the 135 requirement IDs retain unique primary ownership (INV-10 in Phase 6; INV-14 in Phase 12).
- The repository documentation group passed 36/36 selected obligations in `.phase1/runs/a188e36f-6c88-42b9-98dc-6e01a9684965/summary.json`. Separate record checks cover the newly authored context/log before tracking. This does not claim Phase 6 gameplay, rendered or LAN acceptance.
- Earlier attempts `.phase1/runs/b8a57528-cec2-4c47-aab6-c22410d9a553/summary.json` and `.phase1/runs/ef4251b5-65eb-4e53-af75-15d9d1370223/summary.json` were blocked at dependency restore. Their original records remain retained; the passing run is a distinct execution after dependency access was available.
