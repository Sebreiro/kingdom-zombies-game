---
phase: "06"
slug: "survivors-inventory-and-combat"
status: approved
reviewed_at: '2026-11-24T10:36:53Z'
shadcn_initialized: false
preset: none
created: "2026-11-24"
---

# Phase 06 — UI Design Contract

## Authority and delivery boundary

Use [06-SPEC.md](06-SPEC.md) R01–R22 and AC-01–AC-41, [06-CONTEXT.md](06-CONTEXT.md) D-01–D-48, [game specification sections 2, 4, 5 and 13](../../../specs/0.1/initial-spec.md) and the [Phase 4 visual contract](../04-asset-production/04-UI-SPEC.md). The controls and surfaces below are part of the production solo and four-player LAN game. Phase 12 owns filtering. Phase 9 owns interactive legacy-layout extraction, full session recovery and the complete preferences experience.

All displayed belongings, permissions, eligibility and outcomes come from the latest applicable committed gameplay state. One command path serves solo, host-local and guest play. A pending drag, amount field, page change, camera move or tooltip is presentation state. Keep the active world running while these views are open. [06-SPEC.md R11–R13, R19, R21](06-SPEC.md#requirements)

## Design System

| Property | Value |
| --- | --- |
| Tool | none |
| Preset | Inapplicable for the native Godot project |
| Component library | Existing Godot/C# Controls, Themes and content resources |
| Icon library | Packaged UI-03/04/05/07 symbols and the canonical IT-01–IT-24 item atlases |
| Font | Packaged Liberation Sans, with its existing bold variation and glyph fallback. Perfect DOS VGA 437 for the short display heading only |
| Appearance | Phase 4 wooden frames, quiet dark interiors, pixel-art items and established player markers |

Use [InspectionTheme.tres](../../../src/godot-game/game/assets/ui/InspectionTheme.tres) as the visual source for the game theme, including its wood states, font roles, focus border and quiet surfaces. Use the packaged [UI kit](../../../src/godot-game/game/content/packs/ui-kit.json), [identity and icon mappings](../../../src/godot-game/game/content/packs/ui-identity.json) and [asset register](../../../specs/0.1/assets-and-resources.md) for symbols, atlas regions and resource IDs. Preserve nearest-neighbor art, crisp integer drawing and existing world art density. Use native text for all labels, measures, bindings and reasons.

Extend the live [FoundationHud](../../../src/godot-game/game/foundation/FoundationHud.cs), [FoundationView](../../../src/godot-game/game/foundation/FoundationView.cs) and [FoundationInput](../../../src/godot-game/game/foundation/FoundationInput.cs) integration. Reuse visuals and projection conventions from [InventoryLayouts](../../../src/godot-game/game/inspection/InventoryLayouts.cs). Its sample data stays in inspection; production projections use live survivor, item and access publications. The [Godot project](../../../src/godot-game/game/project.godot) already targets a 1280×720 canvas with expand stretch. A repository search for `components.json`, Tailwind and PostCSS configuration found no web UI stack or external component package. The shadcn and package inventory gates are inapplicable.

## Spacing Scale

| Token | Value | Usage |
| --- | --- | --- |
| xs | 4px | Badge inset, icon/text separation, focus outlines |
| sm | 8px | Related controls, nested grid labels, compact rows |
| md | 16px | Panel padding and column gutters |
| lg | 24px | Screen safe margins |
| xl | 32px | Inventory cell pitch and compact title rows |
| 2xl | 48px | Main controls, slot buttons and action-bar tiles |
| 3xl | 64px | Large silhouette or sparse-state spacing |

Use these logical pixels at the reference scale. Combine tokens for layout dimensions. Border thickness, reticle strokes and authored atlas pixels are drawing geometry. Use 48px for standalone buttons and 32px for grid cells given the desktop mouse-and-keyboard target. Preserve full-footprint hit testing even where the art has transparent pixels.

## Typography

| Role | Size | Weight | Line Height |
| --- | --- | --- | --- |
| Body, HUD values, counts, tooltip and feedback | 16px | 400 | 1.5 |
| Control label, button and selected target name | 20px | 400 | 1.5 |
| Panel heading | 24px | 700 | 1.2 |
| Spectator heading | 32px | 400 | 1.2 |

Use the theme’s corresponding type roles. Wrap help, errors and item details on a stable backplate. Use local scrolling for overflow. A single-line item or player name may ellipsize in a compact row if hover and focus reveal the full name and the selected view shows its identity. Use the player slot or target type and distance to distinguish equal-looking names. Preserve full integer counts, page numbers and quantities with no abbreviation. For ordinary HUD values use up to two decimal places with trailing zeroes omitted. Use the committed band, eligibility and bar fraction, independent of the rounded text. Add precision near a threshold if the ordinary display would imply the wrong band, and display applicable exact condition and weight in item tooltips without concealing fractional wear.

## Color

The approximate 60/30/10 distribution applies to the interface surfaces. World art, player identity and functional readings have their own palettes.

| Role | Value | Usage |
| --- | --- | --- |
| Dominant (60%) | `#18252B` | Quiet panel interiors and HUD backplates |
| Secondary (30%) | `#35433F` | Cells, inset rows and supporting controls under timber framing |
| Accent (10% maximum) | `#F0C46A` | Enabled Respawn, keyboard focus outline, selected target or teammate inset, committed stash-cell highlight |
| Text and essential markers | `#F4F2E8` | Text, counts, icons, compatible destination marks and candidate outline |
| Hover, pressed, disabled fill | `#44554F` / `#252F2C` / `#2F3331` | Supporting states |
| Destructive | `#B84740` | Fill for the explicit Drop marker, with a cream glyph and label |
| Functional health / stamina | `#C86C64` / `#91BBA4` | Labeled vitals and their fill levels |

Accent reserved for enabled Respawn, the 4px outer keyboard focus outline, inset markers on the selected nearby target or followed teammate, and committed stash-cell highlights. Use solid outline and check for legal fit, dashed outline and cross for illegal fit, and an explicit reason on rejection. Pair the restrained damage tint with a health pulse and character flinch. Broken gear includes a crossed-condition glyph and the text **Broken**. Preserve the four Phase 4 player identity markers with player number, name and distinct shape alongside color.

Use an 88% opaque dominant fill for the main workspace and nested window interiors. Keep controls, text, tooltips, status backplates and critical state markers opaque. Check ordinary text contrast of at least 4.5:1 and essential boundaries or focus of at least 3:1 over both the brightest and darkest reference gameplay backgrounds. Keep the selected marker inset and the focus outline outside it.

## Copywriting Contract

| Element | Copy |
| --- | --- |
| Primary live HUD action | **Open inventory** (I) |
| Inventory actions | **Open**, **Assign to quick slot**, **Consume**, **Drop**, **Rotate item** (R), **Close inventory**, **Close bag** |
| Split field and actions | **Split stack**, **Quantity to move**, **Half**, **Move quantity**, **Cancel split**, **Moving {amount} · Remaining {remainder}** |
| Split error | **Enter a whole number from 1 to {maximum}.** |
| Storage empty | **No storage equipped.** / **Equip clothing, a belt or a backpack with storage.** |
| Grid, stash page and quick slot empty | **Empty container** / **Empty stash page** / **Slot {number} empty**. In the workspace, **Drag carried food here to assign a slot.** |
| External panel with no target | **No nearby inventories.** / **Move closer to a reachable stash, survivor or loot.** |
| Empty NPC, corpse or pile | **No items to take.** Keep the target and its valid inventory locations visible. |
| Access closed | **{target} is out of reach. Move closer to reopen.** / **{target} is no longer available. Choose another nearby target.** On no remaining targets, use **Move closer to a reachable inventory.** |
| Stale item or transfer | **That item moved. Choose an item from the updated inventory.** |
| Other transfer failures | **Item does not fit. Rotate it or choose another grid.** / **Unequip {item} before replacing it.** / **No room in carried storage. Make space in a pocket or bag.** / **That stack is full. Choose a different stack or empty cells.** / **A container cannot hold itself. Choose another container.** / **Container nesting limit reached. Choose a shallower grid.** / **This item does not fit {slot}. Choose a compatible slot.** |
| Stash paging | **Previous page**, **Next page**, **Page {current} of {total}**, **Go to page**. Invalid input displays **Enter a page number from 1 to {total}.** Multi-page result displays **Items also placed on {count} other pages.** |
| Pending and update failure | **Moving item…**, **Opening inventory…**, **Consuming food…**, **Requesting survivor…**. On a recoverable load failure use **Inventory unavailable. Reopen it to try again.** Keep the established connection-loss feedback if the host is lost. |
| Recovery and load | **Unsafe**, **Resting**, **Recovering**. **Load {weight} kg · {Normal/Heavy/Overloaded}**, **Move speed {percent}% · Stamina cost {percent}%**. |
| Blocked sprint/jump | **Too much weight to {sprint/jump}.** / **Not enough stamina to {sprint/jump}.** |
| Blocked attack and food | **Attack cooling down.** / **Not enough stamina to attack.** / **Weapon broken. Free a hand or change weapons.** / **Hands incompatible. Check your equipped items.** / **Food cooling down.** / **Food stack unavailable. Assign carried food.** For an empty arrow hand, show only the zero count and unloaded bow. |
| Spectator | **Spectating**, **Respawn**, **Respawn available shortly**, **Waiting for a survivor**, **No living teammates to follow**, **{player} is no longer available to follow. Choose a living teammate.**, **Previous teammate**, **Next teammate**, **Following {player}**, **Control transferred** |
| Destructive confirmation | **Drop** is a deliberate item-menu action labeled for the selected item and quantity in its menu context. Activating it explicitly commits the drop request. Outside release and Escape cancel the preview. No second dialog is needed for this deliberate transfer to recoverable world loot. |

Keep the creator-selected **Respawn**, **Open**, **Consume** and **Drop** labels in context. Include object names in accessible descriptions, such as **Consume one {item}**. Keep disabled status reasons visible and independent of tint.

## Layout and Interaction

### Live world and HUD

At 1280×720 use a 24px safe margin. The live survivor is the gameplay focal point. Anchor the vital bars and their labels at top left, with carried money, load and its movement/stamina modifiers in a compact group beside them. Put **Open inventory** next to the action bar within its safe band. Show current and maximum health/stamina and use the stamina bar alone for its spending and regeneration. **Unsafe** reflects loss of qualifying safety/rest conditions. **Resting** indicates qualification or safe rest at full health. **Recovering** indicates active health gain. Display these from authoritative state without a progress display.

Anchor the bottom action bar in a 48px-high row with left hand, right hand, ready arrows, and four numbered food references. Show the canonical held icons, occupancy and condition in both hand tiles. Ready arrows are only the actual opposite-hand count for the bow, with a zero count when empty, including when reserves are carried. With no bow equipped, keep the ready-arrow tile inactive. While aiming with the bow, keep a small crosshair at the cursor and the bow posed toward it with no arrow layer when empty. Use no aim line, reserve tally or empty-shot message. Use shaded cooldown sweeps across the relevant hand action and each valid assigned food tile, with the same authoritative personal food timing for every reference. Use one icon per item definition consistently across storage, equipment and HUD.

Place blocked attacks by the hand/ammunition group, and blocked sprint/jump by the vitals. Emphasize the specific blocked readout. Use a two-second default for transient notices, coalescing identical repeated input instead of stacking messages. Put the health pulse and restrained screen-edge damage tint above the workspace on a draw-only layer. Use a 250ms default for the pulse and tint, with authored world flinch. Nonlethal damage preserves an otherwise valid preview, menu and input focus. Keep the existing LAN identity/connection feedback in a separate safe area, clear of vital and panel controls.

### Inventory workspace

At the reference size put the workspace between y=112 and y=640 with 24px side margins and 16px gutters. This leaves a top band for vitals and a bottom band for the action bar. Capture gameplay input across the viewport while any inventory surface is open, including gaps and transparent regions.

In own inventory, use a 320px equipment column on the left and a flexible carried-storage column on the right. The equipped survivor silhouette is the left visual anchor. Place labeled headwear, face, torso clothing, legs, body armor, gloves, boots, backpack, belt, left hand and right hand in two slot columns around it. At the reference width, reserve 96px per slot column and 80px for the centered silhouette, with 8px between columns. Wrap a long slot label within its control. Keep all eleven slot controls visible with distinct empty/occupied markers. Use a 4px slim condition bar for applicable equipment with an exact tooltip reading and broken marker. Show labeled, actual worn item grids beside the equipment view. Lay out multiple pocket and bag grids with independent labels in a scrollable flow. Unequipping clothing moves its actual storage and removes the prior owner's corresponding grid immediately on commit.

Use full-size 32px cells with fixed content geometry. Draw an item across its actual oriented footprint, with a readable integer quantity badge on stackables. Hover and focus expose tooltip detail for name, weight, current/max condition when applicable, quantity/stack limit and container dimensions when applicable. Focus/hover details use the same data, dismiss on view changes, and clamp within the viewport.

Open a nested grid only via the selected item's **Open** menu action. Display it in a movable in-canvas window above the workspace, with a title, source breadcrumb, actual depth and close action. Open the first view beside its source on the side with usable room. Raise the active window and keep its title bar and close action inside the workspace bounds. Local horizontal/vertical scrolling handles grids larger than the visible window. Keep several real grids open simultaneously. Remember positions by instance ID for the current play session and clamp them on reopen. Closing a window closes descendants; reopening inventory recreates only the main workspace. When a moved item's access or ancestry becomes invalid for an open view, close the affected branch and retain valid siblings. These view changes preserve committed contents.

### Stash and nearby loot

For external access at 1280×720, use fixed columns of 320px equipment, 432px carried storage and 448px external contents. The external grid and its target title are the workspace focal point. Put the authorized target selector inside the right column. When several targets are available, open their compact list on interaction and preserve the selected target identity until it changes or becomes invalid. Show NPC/corpse equipment and its actual storage in that column, using local scroll. Show a pile's actual contents with its target heading. Never display another controlled player's private contents. Use a steady selection outline and a distinct marker on the selected world target, consistent with Phase 4.

For a stash, keep its current page title, previous/next controls, editable page number and page total in a fixed row next to the grid. Keep the full default 10×10 page visible at the reference size and use local grid scroll for larger configured layouts. Commit valid direct navigation on Enter or **Go to page**. At boundaries, disable unavailable arrows. Changing pages cancels a preview anchored to the departed page. Empty pages remain usable and navigation creates no storage. Automatic deposits use the committed first affected page and highlight changed cells for two seconds. Display the additional affected-page count if applicable. Apply that navigation only to the depositing player's active matching stash view, without disrupting another viewer's selection or a newer local action.

Validate access at open and at commit. When the selected target becomes inaccessible, remove its projected contents, close its related bag windows and safely clear affected previews. Keep own inventory open. With other targets available, restore the nearby list and show the reason in that panel. With none available, show the reason in a brief HUD notice when the external panel closes. If an empty corpse or pile later disappears under the authoritative cleanup rule, use the same access-loss route. Panel failures stay with the affected panel until a new interaction or closing. Corrected state replaces stale contents coherently.

### Controls, previews and authoritative transactions

Preserve the section 13.2 default keys for movement, Shift sprint, Space jump, left-click attack, E interaction, I inventory, R drag rotation, Ctrl-click quick transfer, Shift-drag splitting, 1–4 food and Escape. In inventory, right-click an item or use the keyboard context-menu action to open its item menu. Use active-panel focus, then the enclosing workspace, then world input. Tab and Shift+Tab traverse visible controls in layout order. Enter and Space activate the focused control where appropriate. Use accessible names for icon-only controls and return focus to a valid opener when a child closes. Clear gameplay holds and pending actions on focus loss, inventory opening, death and disconnection. Require a fresh press after world input resumes.

Drag keeps the current source committed until a legal destination is accepted. Show the full cell footprint, legal/illegal state and compatible equipment slots. Highlighting compatibility does not make an occupied slot available. Drop on a compatible free equipment slot to equip and move gear out deliberately before replacement. Rotation previews in 90-degree steps. Outside release cancels. Escape first cancels the drag and any owned split UI while leaving browsing open. Escape next dismisses the active popup or window before the workspace and the enclosing in-run menu. Purely decorative HUD layers leave input untouched. While browsing, the workspace captures movement, sprint, jump, attacks, world interaction and quick-food keys. Page-entry digits act only on the field.

Shift-drag opens the exact integer split field for a stack with at least two units. Set **Half** to floor(source quantity / 2). Confirming the amount continues the extracted-stack preview. A valid placement commits split and move together. The source remainder retains its identity, and cancellation before commit preserves the original stack. Display the extracted and remaining amounts while the field is open. For compatible merges, fill the destination up to capacity and leave an exact remainder in the source where needed. Invalid or stale previews refresh from the authoritative result and use the affected-panel failure copy.

Ctrl-click in an authorized external view withdraws to actual carried storage, merging compatible carried stacks before using legal pocket or bag space. The open local stash accepts Ctrl-click deposits from carried storage. Allow deliberate drag transfers for any permitted location and preserve storage descendants. Quick withdrawals never equip or refill the arrow hand. Food may be referenced by dragging the actual carried stack onto a numbered tile or using **Assign to quick slot** with a numbered selection. A reference changes no quantity. On exhaustion, full merge, removal or death, clear invalid references with no replacement stack. Choose **Consume** for carried food while browsing. It uses the same authority, cooldown and one-unit effect as the quick-slot key, even at full vitals.

While an action is in flight retain the last permitted committed projection, mark the pending operation, and guard duplicate submissions of the same intent. Remove authority-gated contents immediately on known access loss. Ignore a late result for a superseded target, item context or survivor view while keeping the newest applicable committed state. Preserve the source and display a concrete reason on rejection.

### Spectator and succession

Use a compact anchored card with **Respawn** as the focal action and the camera view visible. Immediately remove the dead survivor's gameplay input, live action bar and inventory panels, clear their previews, and hold the camera at the death view. Display the disabled Respawn button and its short status until the authoritative one-second boundary. Enabling the button leaves control unassigned. An enabled request resolves immediately or enters the waiting queue. Queued requests complete automatically when a survivor becomes eligible, in authoritative request order with player slot breaking ties. During submission and while queued, block duplicate requests and show the corresponding copy without queue position.

Under the button, provide Previous/Next and a compact teammate selector using live eligible player slots in stable order. Follow only after a deliberate selection. With no selection, Previous chooses the last eligible teammate and Next chooses the first. With a selection, cycle the eligible teammates in the chosen direction. With zero teammates, keep the empty-state text and disable follow controls. With one, allow selection while still cycling to that same target. On loss of a followed teammate, hold the last view, clear the selection and wait for another deliberate choice. Use only the permitted teammate view and previously available information. Spectators have no inventory, combat or free-camera input.

On actual assignment, update crown, control and the HUD together to the same successor publication, preserve the successor's real state and show **Control transferred** with the new survivor identity for two seconds. For the camera, use a 0.25-second smooth handover when that successor lies within one quarter of the current horizontal camera span from the held camera center. Cut directly for a greater displacement or where interpolating would expose unexplored territory. Apply the existing map bounds, then resume normal follow. These are presentation defaults within D-29.

### Production appearance

Compose actual clothes, armor and equipped hands using the established authored character, action and item layers. Preserve left/right logical hand identities through facing changes. Crowns remain visible with headwear and identify control. Nested bags never add a worn layer. Broken items stay represented while equipped or contained. Equipped possessions and their layering update after committed changes on both living entities and lootable corpses. Transient damage and death cues do not remove the corpse, inventory or visible item layers. Loot projection uses the exact remaining contents of the authoritative corpse or pile. [06-SPEC.md AC-24–AC-27, AC-40](06-SPEC.md#acceptance-criteria)

## UI Considerations

Applicable state considerations resolved: 17 covered, 0 backstop, 0 unresolved in this authored contract. The rows specify expected behavior for planning and review.

| Category | Element(s) | Status | Resolution |
| --- | --- | --- | --- |
| empty | Personal inventory and worn grids | ✅ covered | With no storage-bearing equipment, show the Storage empty copy and all eleven slots. A worn empty grid keeps its legal cells and drop targets. |
| empty | External access | ✅ covered | No targets, an available empty corpse/pile and an available empty stash page use their corresponding Copywriting Contract states. |
| empty | Food references and active bow | ✅ covered | Missing food renders an empty numbered tile. An active unloaded bow follows the cursor with the zero count and no visible arrow or extra warning. |
| empty | Spectator teammate selector | ✅ covered | Zero living teammates retains the fixed camera, follow controls disabled and the independent Respawn or waiting status. |
| loading | Target contents and session updates | ✅ covered | The external title updates to the requested target and shows the pending copy without reusing the previous target's contents. Apply only current permitted responses. |
| loading | Transfers, consume and Respawn request | ✅ covered | Retain the last permitted projection, mark the pending intent and block its duplicate submission until authoritative resolution. |
| error | Inventory transaction | ✅ covered | Rejected transactions show the applicable Copywriting Contract reason, correct stale contents and keep ownership consistent. |
| error | External access and spectator target | ✅ covered | An invalid view closes or clears its contents, shows the applicable copy and leaves permitted sibling controls available. |
| error | Session/load failure | ✅ covered | A recoverable view failure offers reopening through the Copywriting Contract. Host loss retains connection feedback and clears actionable session views. |
| populated | Own inventory and external grid | ✅ covered | At reference size all eleven slots are visible, the selected grid shows actual footprints/counts and the default stash page is reachable with its navigation. |
| populated | Living and corpse appearance | ✅ covered | Equipped layers and crown, where eligible, update from committed state for equal-looking items, gear removal, orientation switches and death. |
| partial | Equipment and pockets | ✅ covered | Partially filled grids and empty equipment slots retain stable positions, with capacity only from real storage. Broken gear remains visible and marked. |
| partial | Food and spectator state | ✅ covered | A subset of valid food references stays usable under one cooldown. Surviving teammates remain selectable when another teammate dies or leaves. |
| overflow | Grids, nested windows and targets | ✅ covered | Oversized collections scroll locally without obscuring navigation, window close controls, vitals or action bar. |
| zero-one-many | Pages, targets and teammates | ✅ covered | An empty page remains usable. A single target or teammate is selectable by identity, and multiple entries retain stable ordering without collapsing duplicate names. |
| long-text | Item details and player/target names | ✅ covered | Long names wrap in detail and remain available to hover and focus when a compact row truncates. |
| long-text | Amounts, page numbers and reasons | ✅ covered | Integer fields and values preserve full digits. Error reasons wrap with the related action and its focus indicator still reachable. |

## Verification handoff

At the reference 1280×720 layout, exercise own inventory, external stash and loot access, valid/invalid drag and split, visual condition and worn layers, damage over inventory, action feedback, quiet empty bow, and the full death/request/follow/handover surface. Include the access and ownership races in AC-15/16/19/34/41 and the unchanged live simulation during browsing. Verify the interface on the actual solo and host-plus-three-guests runtime through production actions, with distinct viewport evidence and controlled identities. Exercise layout states as part of the existing [local testing strategy](../../../docs/workflow/local-testing.md) after whole-phase implementation and code review.

Import the exact Phase 6 entries in the current [asset handoff register](../../../src/godot-game/tools/asset-pipeline/handoff.json) and their procedures in [04-SCENARIOS.md](../04-asset-production/04-SCENARIOS.md). Use the original inspection fixture for checks specific to that tool. Shipped gameplay acceptance requires separate production observations under [06-SPEC.md AC-33–AC-41](06-SPEC.md#acceptance-criteria). The retained Phase 4 IDs and the Phase 6 state rows above are verification obligations.

## Registry Safety

| Registry | Blocks Used | Safety Gate |
| --- | --- | --- |
| none | none | Inapplicable. The contract uses project-authored Godot controls and packaged Phase 4 resources. |

## Checker Sign-Off

- [ ] Dimension 1 Copywriting
- [ ] Dimension 2 Visuals
- [ ] Dimension 3 Color
- [ ] Dimension 4 Typography
- [ ] Dimension 5 Spacing
- [ ] Dimension 6 Registry Safety
- [ ] Dimension 7 Inventory Provenance

**Approval** pending independent review.

The 48 decisions in 06-CONTEXT and the locked 06-SPEC govern this contract. Layout measurements, opacity, copy for unspecified states, context-menu activation, Half rounding, display precision and camera/cue timing are researcher defaults within the discretion recorded in 06-CONTEXT. Phase 4 supplies the visual assets and theme. Phase 5 supplies the test framework.
