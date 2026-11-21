---
phase: "04"
slug: "asset-production"
status: draft
shadcn_initialized: false
preset: none
created: "2026-11-16"
---

# Phase 04 — UI Design Contract

Visual and interaction contract for the separate KingdomZombies resource inspection hub and the 27 reusable game UI resource obligations. Implementation and rendered acceptance are pending. Checker approval is pending.

## Authority and Delivery Boundary

Use [04-CONTEXT.md](04-CONTEXT.md) D-01–D-32, [04-SPEC.md](04-SPEC.md) ASSET-01–ASSET-05, AC-01–AC-24 and H01–H34, and [the resource register](../../../specs/0.1/assets-and-resources.md), especially §§9–11. These remain authoritative; the defaults below resolve routine presentation details without changing their scope. All 34 held-out obligations and four judgment-tier prohibitions remain independently identifiable in planning and verification.

Deliver one Windows inspection hub in the existing Godot/C# project. Characters, Items, Buildings/world, UI, Data/text and Audio share navigation, resource identity, status and diagnostics. Inspect all 180 obligations or their explicitly shared outputs. A file count, percentage, polished preset or placeholder cannot stand in for a missing obligation. Reuse suitable recovered resources first and retain exact donor/generated/adapted provenance.

UI-01–UI-27 are reusable native layouts and authored visual states, not flattened screenshots. Their fixture state selectors work; future inventory transactions, purchases, saving, networking, jobs and combat logic are outside this phase. Preserve the foundation gameplay entry point and placeholders. Character controls and audio audition controls are live. Clearly label the hub and UI fixtures as resource inspection so sample state does not imply implemented gameplay.

## Design System

| Property | Value |
|----------|-------|
| Tool | none |
| Preset | not applicable |
| Component library | Project-authored Godot native Controls, Theme resources and reusable scene/resource assemblies |
| Icon library | Selected recovered K20/K21/K25/K26 symbols plus generated missing pixel symbols, each mapped to a stable output ID |
| Font | Package extracted K27 LiberationSans for body text/numbers and PerfectDOSVGA437 for selected headings; package glyph fallback; production extraction/readability remains to be proved |
| Engine | Existing pinned Godot 4.7.2 mono/C# toolchain; no new UI framework or paid dependency |
| Source treatment | D-29 wooden frames with quiet dark interiors; D-30 clear sans-serif with pixel accents |

Scout evidence: `rg --files src/godot-game -g '*.tscn' -g '*Theme*' -g '*theme*' -g '*.ttf' -g '*.otf' -g 'components.json'` and inspection of `src/godot-game/project.godot`, `foundation/Foundation.tscn`, `FoundationWorld.tscn`, `FoundationRoot.cs` and the Phase 3 UI contract. The current foundation uses native Controls, local StyleBoxFlat states, a bundled fallback font and FontVariation, with 1280×720 canvas-items/expand configuration. There is no shadcn configuration, React/Next/Vite stack, installed external design-system package or production font asset in that inspected set. shadcn initialization and package inventory gates are inapplicable; Component Inventory is intentionally omitted for Tool: none. `query agent-skills gsd-ui-researcher --skills-only` returned no configured skills.

Reuse existing focus/input ownership conventions. Create a phase-local production Theme for the hub and resource layouts without rewriting foundation visuals. Native Button, Label, LineEdit, OptionButton, slider, scrolling and container patterns are implementation candidates evidenced by existing scenes, not a claimed exhaustive engine inventory. Use containers and content-driven sizing for production layouts rather than copying the foundation's fixed offsets.

K24 wood states are donor candidates, not already verified exports. Keep pixel corner regions intact and stretch/tile only appropriate edge/interior regions. No text, quantity, binding or essential state may exist solely inside a texture. Resolve exact packaged font/output paths in the manifest. If a donor font lacks a required glyph, route to a packaged fallback and record its source; a temporary engine font must not falsely complete the font obligation.

## Spacing Scale

All values are logical UI pixels at 100% user scale. Apply the selected UI scale once, independently of world sprite zoom and fixture inspection zoom. Snap pixel-art edges to rendered pixel boundaries. Do not resize the world's logical art density to make UI text fit.

| Token | Value | Usage |
|-------|-------|-------|
| xs | 4px | Icon/text separation, compact badge inset, focus stroke |
| sm | 8px | Label/field gap, row inset, related control gap |
| md | 16px | Content gutters, controls-to-preview gap, group separation |
| lg | 24px | Normal screen safe margin and section padding |
| xl | 32px | Major content separation and compact icon artwork box |
| 2xl | 48px | Minimum standalone button/input height and fixture toolbar row |
| 3xl | 64px | Large row/preview spacing where content warrants it |

Exceptions: none for spacing. A 48px control at 80% scale renders approximately 38px high; this is a keyboard/mouse desktop contract, not a touch-target claim. Borders and sprite pixel units are drawing geometry rather than additional spacing tokens. Hover/selected/focus decoration must not change control layout or obscure text.

## Typography

Exactly four size tokens and two weights: 16/20/24/32px and regular 400/bold 700. These are production defaults within D-30, not a claim that the donor font comparison is complete. Use the existing local font-variation approach for the bold face if the selected extracted family lacks a separate bold file; package the base font and record the derivation.

| Role | Size | Weight | Line Height |
|------|------|--------|-------------|
| Body, metadata, numeric counts, field captions, diagnostics | 16px | 400 | 1.5 |
| Label, button text, selected row name | 20px | 400 | 1.5 |
| Heading, category and section title | 24px | 700 | 1.2 |
| Display, KingdomZombies hub title and sparse outcome title | 32px | 400 | 1.2 |

Use LiberationSans for the first three roles. Use PerfectDOSVGA437 only for short display accents; use the sans-serif fallback for unsupported glyphs and full sentences. Do not use pixel type for file paths, item counts, errors or long descriptions. Keep numbers aligned within their own columns. Preserve authored values, units and precision; no display rounding may disguise a different quantity/default.

Wrap body text, warning text, descriptions and error reasons; grow rows vertically. Never shrink typography to fit. Single-line list names may ellipsize only when the selected detail exposes the complete name and stable ID; tooltip alone is insufficient. Editable fields scroll their contents. Exact IDs/paths remain copyable, wrap at safe separators when possible and permit horizontal scrolling for a single unbreakable token. An ellipsis must never replace the only available diagnostic identity.

## Color

The 60/30/10 split controls approximate visual emphasis across the hub, not world-art or item palettes. Reuse the established dark/cream/gold family while selecting donor wooden frames under D-29. Maintain at least 4.5:1 contrast for ordinary text and 3:1 for essential boundaries, markers and focus against their immediate backing.

| Role | Value | Usage |
|------|-------|-------|
| Dominant (60%) | `#18252B` | App background, preview surround, quiet dark panel interiors |
| Secondary (30%) | `#35433F` | Row/control fills, inset information groups; recovered warm timber frames provide material texture |
| Accent (10% maximum) | `#F0C46A` | Current category marker, selected resource marker, keyboard focus, the current primary Inspect resource action and current playback position |
| Text | `#F4F2E8` | Main text, quantities, labels, status words and critical warning outlines |
| Hover / pressed / disabled fill | `#44554F` / `#252F2C` / `#2F3331` | Supporting control states; disabled state also includes a reason |
| Destructive | `#B84740` | Destructive fixture action border/icon only; use cream text on a dark backing |

Accent reserved for: selected category's inset marker; selected resource's inset marker; outer focus outline; the single primary Inspect resource button; current animation timeline position. Ordinary links/buttons remain secondary until focused or selected. Do not recolor real equipped items to encode body identity. A thin steady cream outline plus modest marker identifies selected corpses/loot/inspectable objects under D-24. Avoid pulsing selected outlines.

Use written status plus a distinct glyph: check + Ready, clock + Loading, exclamation + Incomplete/Error, dash + Unavailable. Do not rely on green/red. Four player markers include P1/P2/P3/P4, the player name and a distinct simple shape, with four required secondary identity colors alongside those cues. Dark-mode world fixtures must preserve equipment, identities and critical state readability under D-14; local backgrounds must not drown them in panel decoration.

## Layout and Interaction

### Hub shell and responsive composition

At 1280×720/100%, use 24px outer margins, a 48px title/status row and 16px gutters. A 176px category rail sits beside the remaining workspace. Keep the resource ID, complete selected name and explicit production status in a compact wrapping summary above the preview. The pack status remains visible in the shell. Put Register and Known issues under the common navigation without adding extra content categories. Display unique covered obligations as `{covered}/180 obligations`; use Incomplete until every required obligation and its required evidence is satisfied.

On the character page, give one character the main preview area and put controls in a 320px scrolling side panel, separated by 16px. At the reference size this leaves approximately 624px preview width inside the workspace. Body/gear selection uses labeled selectors rather than a second character grid. Other categories use a searchable resource list and selected-resource detail; a list can occupy the side panel or its own browse state, but must not compress the character into a thumbnail. Expand provenance/dimensions/timing into a detail drawer; failures are always visible outside that drawer.

When available layout width after user scale falls below 1040 logical pixels, replace the category rail with a labeled category selector and put the controls in a vertically scrollable region below the preview. Keep the resource summary and preview transport together at the top of that region. Switch browse/detail as explicit views when necessary; Back to list restores the filter, selected ID and scroll position. Essential controls may require scrolling at 150% scale but may not be clipped, hidden behind another panel or reduced in font size. Scrolling is local to the owning region; avoid a single giant horizontal canvas for ordinary menus.

Use 1920×1080's extra space for preview/detail breathing room, not more required characters or extra features. Large world/map or legacy-grid fixtures intentionally permit pan/zoom inside a labeled clipped viewport; shell navigation and diagnostics remain outside it. View transforms are inspection state and cannot modify the authored map/layout/data.

### Input, control states and focus

Every interactive hub control has normal, hover, keyboard focus, pressed, disabled and selected states where applicable. Focus is a 4px outer gold outline; selection is an inset marker, so both remain distinguishable. Tab/Shift+Tab follow visual order, native selectors own their arrow keys, Enter/Space activate the focused button and text editing keeps its own keys. Hidden/disabled controls cannot keep focus. Put a visible reason next to disabled actions and make that reason available in a focused help/detail region.

Consume inspector/panel input so it cannot also trigger world movement, firing or another preview action. Do not attach a global Space-to-play action while a text field or native popup owns that key. Escape closes the innermost popup, then the metadata drawer, then returns from detail to its category; it must not silently discard an operation or unexpectedly exit the app. Restore focus to the opening control when closing a popup. Category changes retain the category's last selected resource where available and cancel obsolete loads/auditions. Late results cannot replace a newer selection.

### Live characters, world and items

Expose body (survivor and seven zombie bodies), equipment by the eleven logical slots, left/right hands, facing, movement, action, aim, ammunition and condition with explicit labels. Include valid-empty and broken/fallback examples. Opposite facing preserves logical left/right inventory identity. Illegal new combinations are unavailable with reasons; the separate compatibility fixtures can deliberately display historical invalid occupancy without silently legalizing it.

Transport is explicit: Play preview / Pause preview, speed selector `0.25×`, `0.5×`, `1×`, `2×`, Step frame and Reset preview. Default speed is 1×. Step frame is enabled while paused and advances one authored presentation sample; show clip/sample index and authored time so nonuniform durations and held frames remain inspectable. Preview speed affects inspection playback, not configuration values or game timing. Reset reconstructs one current state; triggering an attack/effect is a separate deliberate action. Repeated reset must not add render layers, event subscriptions, ambience loops or replay historical one-shots.

Body/gear/action changes apply immediately, remove the replaced visuals and may restart the selected action cleanly. Walking continues during melee, bow aiming and firing. Preserve D-01–D-12 pose/motion constraints and D-13–D-24 scale/material/state choices. Show actual equipped items and ammunition. A stable corpse remains selectable after transient blood/acid fades; changing its remaining equipment updates its appearance. Structural guards have no corpse/loot example. Fired arrows and removed crowns have no recoverable loot treatment; cosmetic acid has no danger/damage badge.

Buildings/world selectors expose each main/wall/tower tier and lifecycle state, 1/2/3 standalone guards, two one-guard final-main attachments, six portal placements and permanent ruins. Include day/night, damage, work/repair, previous functional tier, food/clearing/wildlife/effects, assembled-map and short crowd/persistent-corpse previews. State restoration must reconstruct current appearances without replaying destruction, death, payment, recruitment or warnings. A separate Trigger effect action may intentionally audition such a cue.

### Register, failures and data/text

The register retains all 180 expected IDs even with an empty/partially imported pack. Filter by ID/name/category/status; show `0 results`, `1 result` and plural results without collapsing rows that share labels or output files. Clearing the filter restores the complete expected register. Show explicitly shared output mappings individually; do not count a duplicate obligation twice. Keep source-catalog evidence separate from current production validation.

A missing/invalid entry shows its exact obligation/output ID, project-relative path, affected field/reference where applicable, and concrete reason. The shell changes to Pack incomplete. Independent entries remain usable; do not replace the whole hub with a blocking failure modal. A diagnostic placeholder says Unavailable and does not count as completed art. Provide Copy details and Recheck resources; preserve selection/filter and return fresh results rather than inheriting an earlier successful status. Provenance may display an original absolute locator as text, but runtime and validation resolve packaged project paths.

Data/text detail exposes field meaning, unit, shipped default, allowed values, dependencies and exact referenced IDs. Keep host balance and local preferences visibly separate. Include the independently editable zombie-progression resource, English placeholder/current-binding examples, missing-definition diagnostics and sample content-version metadata. This read-only inspection need not become a configuration authoring tool.

### Audio audition

List all AU-01–AU-30 with their actual event binding and selected source/version. Show duration, channel/rate/trim/loop/gain, bus, priority, attenuation, variants, cooldown/repetition and configured 24 concurrent/four-per-repeated-effect limits. Use Play sound, Stop sound and an explicit Loop preview toggle; show Playing/Stopped/Muted and elapsed time. Start audition deliberately; selecting a list entry must not emit a loud sound automatically. Changing category or Stop all audio cancels owned auditions/loops. Rapid changes and repeated state restoration leave only the current intended loop set.

Use the D-31 punchier action/confirmation treatment and D-32 subdued forest/wind/fire ambience. Include day/night/local ambience samples and configurable master/effects/ambience controls. Muting leaves visible critical warning examples and the Muted label. No music or voice bank is added. A file with metadata alone is not an auditioned recording; retain a distinct Not auditioned status until actual listening evidence exists.

## Reusable UI Resource Obligations

Every row maps to editable native scene/theme/state resources and stable manifest IDs. Shared kit resources may serve several rows. Phase 4 selectors load these examples; the game operations represented by the examples remain later-phase work.

| ID | Required resource and state examples |
|----|-------------------------------------|
| UI-01 | Wood/dark shared kit: buttons, panels, tabs, lists, scrollbars, inputs, sliders, checkboxes, tooltips, modal/toast frames; normal/hover/focus/pressed/disabled/selected and empty/loading/error |
| UI-02 | Four-size/two-weight typography, readable numerals/counts, pixel accent, contrast palette, packaged glyph fallback and expanded text |
| UI-03 | Cursor/reticle/interaction cursor, keyboard/mouse/modifiers, current remapped labels, unbound/conflict/reset; no fixed binding artwork as the only cue |
| UI-04 | Distinct glyph-plus-label status kit for health, stamina, load, cash, ammo, condition/broken, locked, warning/error/waiting/success/busy, food, role/activity, save/connection |
| UI-05 | Item grids; eleven slots; drag ghost; rotated valid/invalid fit; quantity/split; breadcrumb/depth; durability/weight; four quick references; complete tooltips |
| UI-06 | Saved old bounds/footprints/orientation, incompatible slot/stack/depth badges, readable contents, removal guidance, current destination preview and extraction failure |
| UI-07 | Four numbered/named/distinct-shaped player identities, host/guest/controller/spectator/offline labels and teammate markers |
| UI-08 | KingdomZombies title and desktop icon; no donor/publisher identity |
| UI-09 | Main menu with solo/host, LAN/manual address, saves, settings, controls and exit |
| UI-10 | Host/lobby and LAN browser resource examples: session/host/address/capacity/progress/compatibility; empty/discovering/full/incompatible/unreachable |
| UI-11 | Join/loading/reconnect: validation, waiting/progress, timeout, mismatch, host loss and guest return; no promise that other players' world stops |
| UI-12 | HUD: health/stamina, carried cash, load, both hands, hand arrows, four quick references, day/time/phase, warnings and teammates |
| UI-13 | Own inventory: headwear, face, torso, legs, body armor, gloves, boots, backpack, belt, left hand and right hand; actual pocket/bag grids, nesting, rotate/split/transfer/world-drop/reference states |
| UI-14 | Unlimited paged shared stash with character context, page/name/type filters, current contents, empty/no-match and losing shared-operation examples |
| UI-15 | NPC/corpse/ground-loot target identity/current gear, overlap selector, allowed/disabled transfer states, emptied target, lost range/dead target/access closed |
| UI-16 | Recruit/job/people: price, fireside/total cap, counts, individual role/activity, waiting for gear, population/cash/controlled-survivor failures |
| UI-17 | Stash-food-only sale: actual stock, quantity, unit/total price, stash-money result, nothing to sell and concurrent stock change |
| UI-18 | Site/building: tier/HP, prerequisite/lock reason, personal cash, build/upgrade/rebuild/repair price, paid work/progress, no workers/unsafe, previous tier/ruin |
| UI-19 | Rally: side, stationed/ready count, desired mix, portal/cost, active/recall/returning, no target/insufficient squad/already active |
| UI-20 | Minimap/expanded map: shared explored mask, known portals/defenses, active/destroyed states, current player/teammates, camp border and legend; hide unexplored portals |
| UI-21 | Spectator/succession: follow/cycle teammate, waiting for eligible survivor, transfer name/marker, unavailable actions/reason |
| UI-22 | Save/load/run list: name/day/result/date/time, manual/auto, saving/success/failure, compatibility/missing definitions, older saves and selected context |
| UI-23 | Exit/session end: host retry/return/leave without latest save, guest leave, host-ended/lost connection, explicit scope of host exit |
| UI-24 | Victory Continue/Main menu with remaining-danger text; defeat Load save/New run/Main menu; earlier victory versus later continuation defeat |
| UI-25 | Active-world menu/settings/controls: display/resolution, UI scale, master/effects/ambience, rebind/conflict/reset and current reference; explicit World continues |
| UI-26 | Panel-independent visible nightfall/retaliation/damage/portal and order/cash/equipment/inventory/target/save/connection notifications and errors; muted-warning example |
| UI-27 | Configuration/content report: exact file/field/value/reason and missing asset/item/text IDs, incompatible save/content distinction, recoverable legacy notice |

Inventory defaults: one canonical icon per IT-01–IT-24 under D-20. Show numeric current/maximum condition plus a Broken glyph/word at zero, rather than making durability icon variants. Ordinary clothing/bags/belts do not acquire invented wear. Show stack count and weight as separate labeled values. Quick slots are four references to real food stacks, not duplicate supplies. Distinguish two hands by label, not screen-side position alone. A legacy grid preserves original saved geometry and contents; put the current destination in a separately titled preview, never overlay the two sets of bounds ambiguously.

Keep critical warnings above inventory and other ordinary panels in a dedicated nonblocking notification layer. Their complete text wraps; it cannot be cut off by a tooltip or modal. Error/report details scroll independently. Z-order must preserve the selected target/identity and current interaction context rather than merging nearby panels or equal labels.

## Copywriting Contract

All copy is English catalog data with plural forms and typed placeholders. Preserve stable message/definition IDs and current-binding references; TXT-02 supplies longer replacement strings without shipping a second locale. Rows below prescribe important hub copy and game-state examples; the complete catalog still covers every UI row, item/zombie/role/site, prompt and error in TXT-01/02.

| Element | Copy |
|---------|------|
| Hub title/subtitle | KingdomZombies / Resource inspection |
| Primary CTA | Inspect resource |
| Category labels | Characters; Items; Buildings/world; UI; Data/text; Audio |
| Resource summary | `{id} · {name} · {status}` |
| Coverage | `{covered}/180 obligations · Pack incomplete` until complete current coverage/evidence permits Pack complete |
| Empty state heading | No resources match |
| Empty state body | Clear the filter or choose another category. |
| Missing pack | No packaged resources found. Check the content report and restore the listed files. |
| Loading | Loading `{id}`… |
| Error state | Cannot load `{id}` at `{path}`: `{reason}`. Restore or correct this resource, then choose Recheck resources. |
| Invalid reference | `{id}`: `{file}`, field `{field}`, value `{value}` — `{reason}`. |
| Diagnostic actions | Copy details; Recheck resources |
| Preview transport | Play preview; Pause preview; Step frame; Reset preview; Playback speed |
| Metadata disclosure | Show details; Hide details |
| UI fixture label | Layout example — gameplay actions are not active. |
| Audio actions/status | Play sound; Stop sound; Stop all audio; Loop preview; Playing; Stopped; Muted; Not auditioned |
| Game active-menu statement | World continues while this menu is open. |
| Legacy layout notice | Saved layout differs from current rules. Existing contents are shown in their saved positions. |
| Unavailable action | `{action}` unavailable: `{reason}`. |
| Shared-operation conflict example | Contents changed before the action completed. Review the current items and try again. |
| Victory continuation example | Victory recorded. Living zombies can still threaten the camp. |
| Guest leave example | Leave session? Other players can continue. / Leave session / Stay in session |
| Host exit example | End session for everyone? / Save and end session / Return to play |
| Host save-failure example | The latest save failed: `{reason}`. Retry saving, return to play, or end the session without the latest save. / Retry save / Return to play / End without latest save |
| Destructive confirmation | Hub inspection has no destructive content editing. UI-23's End without latest save requires an explicit fixture confirmation: End the session without the latest save? Progress since the last successful save will be lost. / End without latest save / Return to play |
| Control reset example | Reset controls to defaults? / Reset controls / Keep current controls |
| Muted critical warning | `{warning}` remains visible; append Sound muted when the example demonstrates muted playback |

Do not use vague OK for recovery, delete a save implicitly, promise active-world pause, imply acid damage or suggest that fired arrows/crowns/structural guards create loot. Do not insert donor names as normal game identity. Provenance detail may name the donor accurately.

## Display, Text and Accessibility Fixtures

Author explicit fixture canvas sizes of 1280×720 and 1920×1080 at 80%, 100% and 150% user UI scale. Test layout calculation at every combination. Provide expanded-text replacements at least twice normal label length, multiline errors, long unbroken IDs/paths, duplicate display names with distinct IDs, accented names and unsupported-glyph fallback. Preserve all format placeholders and current remapped binding labels. Include one/zero/many rows, empty hands/ammo, large quantities, all eleven slots, nested/legacy grids and four coincident player/target identities.

The UI preview selector independently controls Fixture resolution, UI scale, Text length and State; show those selected values beside the preview. Fit-to-window is a labeled inspection zoom, not a different authored resolution. Allow 1:1 pan to inspect small glyphs. Keep hub controls outside the fixture so increasing fixture scale does not trap its own selector. Missing glyph/reference examples remain explicit diagnostics and cannot gain a passing resource status through fallback art/text.

Follow [current local testing](../../../docs/workflow/local-testing.md): finish the whole phase and code review/fixes before opening real game windows; all routine Windows windows use 1280×720 without a VM. Group compatible inspection scenarios in as few processes as practical. Save stable scenario IDs, importance/rationale, dependency tags, ordered run groups, resets and individual evidence links. Select all active Critical cases plus new/changed/affected regressions; batch repairs, then rerun the entire selected set on the repaired build. Do not introduce per-plan/per-small-fix rendered gates.

Authored 1920×1080 and endpoint canvases can be inspected within the 1280×720 hub and checked with layout/resource checks. That does not claim a real 1920×1080 window, reference-PC stress, Linux or endurance pass. Record any unperformed native display/release procedure under its separate owner. H19/H22/H24 still require identifiable held-out endpoint/text/format checks. Fixtures and this document are obligations, not completed visual evidence.

## UI Considerations

Compiled probe executed after independent checker approval on 2026-11-16. The ten authored surface descriptions explicitly union applicable element kinds. Raw input, raw report, automatic conservative resolutions and resolved report are preserved in the adjacent 04-ui-probe-*.json files. These are verification obligations, not executed UI evidence.

| Element | Category | Status | Required held-out check |
| --- | --- | --- | --- |
| HUB-NAV | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| HUB-NAV | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| HUB-NAV | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| HUB-NAV | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| HUB-REGISTER | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| HUB-REGISTER | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| HUB-REGISTER | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| HUB-REGISTER | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| HUB-REGISTER | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| HUB-REGISTER | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| HUB-REGISTER | zero-one-many | resolved; verification: backstop | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| HUB-REGISTER | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| CHARACTER-PREVIEW | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| CHARACTER-PREVIEW | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| CHARACTER-PREVIEW | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| CHARACTER-PREVIEW | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| CHARACTER-PREVIEW | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| CHARACTER-PREVIEW | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| CHARACTER-PREVIEW | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| WORLD-ITEM-PREVIEW | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| WORLD-ITEM-PREVIEW | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| WORLD-ITEM-PREVIEW | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| WORLD-ITEM-PREVIEW | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| WORLD-ITEM-PREVIEW | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| WORLD-ITEM-PREVIEW | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UI-COLLECTIONS | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UI-COLLECTIONS | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UI-COLLECTIONS | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UI-COLLECTIONS | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UI-COLLECTIONS | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UI-COLLECTIONS | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UI-COLLECTIONS | zero-one-many | resolved; verification: backstop | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UI-COLLECTIONS | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UI-FORMS-CONTROLS | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UI-FORMS-CONTROLS | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UI-FORMS-CONTROLS | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UI-FORMS-CONTROLS | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UI-FORMS-CONTROLS | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UI-FORMS-CONTROLS | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UI-TEXT-STATUS | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UI-TEXT-STATUS | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UI-TEXT-STATUS | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UI-TEXT-STATUS | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UI-TEXT-STATUS | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UI-TEXT-STATUS | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UI-TEXT-STATUS | zero-one-many | resolved; verification: backstop | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UI-TEXT-STATUS | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| DATA-TEXT-INSPECTOR | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| DATA-TEXT-INSPECTOR | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| DATA-TEXT-INSPECTOR | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| DATA-TEXT-INSPECTOR | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| DATA-TEXT-INSPECTOR | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| DATA-TEXT-INSPECTOR | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| DATA-TEXT-INSPECTOR | zero-one-many | resolved; verification: backstop | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| DATA-TEXT-INSPECTOR | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| AUDIO-AUDITION | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| AUDIO-AUDITION | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| AUDIO-AUDITION | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| AUDIO-AUDITION | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| AUDIO-AUDITION | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| AUDIO-AUDITION | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| AUDIO-AUDITION | zero-one-many | resolved; verification: backstop | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| AUDIO-AUDITION | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UI-FIXTURE-PREVIEW | empty | resolved; verification: backstop | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UI-FIXTURE-PREVIEW | loading | resolved; verification: backstop | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UI-FIXTURE-PREVIEW | error | resolved; verification: backstop | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UI-FIXTURE-PREVIEW | populated | resolved; verification: backstop | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UI-FIXTURE-PREVIEW | partial | resolved; verification: backstop | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UI-FIXTURE-PREVIEW | overflow | resolved; verification: backstop | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UI-FIXTURE-PREVIEW | long-text | resolved; verification: backstop | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |

Every resolved backstop must be lifted as a separately identifiable held-out check by the planner. Absent implementation/evidence remains a verification gap. Domain constraints H01–H34 and the four judgment reviews remain independently required.
## Registry Safety

| Registry | Blocks Used | Safety Gate |
|----------|-------------|-------------|
| none | none | Not applicable: no shadcn, remote component blocks, third-party registry or paid animation dependency is selected. |

Selected recovered/generated media follow ASSET-01 provenance, dependency and production validation. An original catalog locator is source evidence, not proof of finished exported content. No purchase or external runtime recovery-folder dependency is introduced.

## Checker Sign-Off

- [x] Dimension 1 Copywriting: approved with FLAG; specific guest-leave CTA corrected
- [x] Dimension 2 Visuals: PASS
- [x] Dimension 3 Color: approved with FLAG; required four identity colors corrected
- [x] Dimension 4 Typography: PASS
- [x] Dimension 5 Spacing: PASS
- [x] Dimension 6 Registry Safety: PASS
- [x] Dimension 7 Inventory Provenance: PASS

**Approval:** APPROVED by independent gsd-ui-checker on 2026-11-16; two nonblocking recommendations applied.

Pre-populated from all 32 context decisions, the five locked requirements, the 27 UI obligation rows and current Godot conventions. Numeric spacing/type/layout/playback defaults are researcher decisions within that authority. This contract does not certify implementation, audition, resource completeness, held-out checks or final visual judgment.
