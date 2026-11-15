---
phase: "02"
slug: "playable-foundation"
status: approved
shadcn_initialized: false
preset: none
created: "2026-11-14"
reviewed_at: "2026-11-14T22:52:52Z"
---

# Phase 02 — UI Design Contract

**Testing amended 2026-11-15:** Follow [02-TESTING-AMENDMENT.md](02-TESTING-AMENDMENT.md). The current rendered acceptance scope is Windows 1280×720 after all implementation and code review/fixes; no VM or per-task game-window gates. Older extra-resolution/platform verification text is deferred to Phase 5. Visual/interaction design remains unchanged.

This contract supplies the smallest usable presentation for BASE-01–BASE-03. Locked behavior comes from [02-SPEC.md](02-SPEC.md); [02-CONTEXT.md](02-CONTEXT.md) D-01 authorizes routine choices without another interview. Palette, layout, typography and placeholder dimensions below are agent defaults under D-02, not creator-approved aesthetic decisions. User-facing copy is English (D-03). No Phase 2 technical research existed when this contract was authored.

## Design System

| Property | Value |
|----------|-------|
| Tool | none |
| Preset | not applicable |
| Component library | Godot native Controls and scene primitives; no external design system |
| Icon library | none; draw the facing marker as a triangle |
| Font | Godot bundled fallback font; repository-defined theme overrides, no OS font dependency |
| Baseline | Existing Godot/C# project and pinned Phase 1 toolchain |

Scout: `rg --files src/godot-game` found project/proof resources, with no production menu, theme, font assets or playable fixture. `rg --files -g components.json -g package.json -g '*theme*' -g '*.tres' -g '*.ttf' -g '*.otf'` found only the Phase 1 proof resource. This is a Godot project; the React/shadcn initialization and installed-component inventory gates do not apply. Native controls named below describe implementation choices, not a package export inventory.

## Spacing Scale

All UI measurements are logical pixels at the 1280×720 reference viewport.

| Token | Value | Usage |
|-------|-------|-------|
| xs | 4px | Focus outline inset and marker separation |
| sm | 8px | Compact text gaps |
| md | 16px | Button separation and horizontal padding |
| lg | 24px | Screen-edge safe margin |
| xl | 32px | Title-to-actions gap |
| 2xl | 48px | Button minimum height |
| 3xl | 64px | Reserved major layout spacing |

Exceptions: none. Button minimum width is 240px; menu content maximum width is 480px. Outlines are 4px. World dimensions are game measurements, not additional UI spacing tokens.

## Typography

Exactly three sizes and two weights: 16/24/32px and regular 400/bold 700. Derive bold from the bundled font; do not introduce external font downloads.

| Role | Size | Weight | Line Height |
|------|------|--------|-------------|
| Body / controls hint / error | 16px | 400 | 1.5 |
| Label / Start and Quit | 24px | 400 | 1.5 |
| Heading / KingdomZombies | 32px | 700 | 1.2 |

Use these sizes consistently. Center the menu title and action labels; left-align the in-scene hint. Keep text fully visible at both required resolutions.

## Color

The 60/30/10 split describes visual emphasis rather than an exact screenshot pixel quota; the sparse fixture uses more background area.

| Role | Value | Usage |
|------|-------|-------|
| Dominant (60%) | `#18252B` | Menu background and scene sky |
| Secondary (30%) | `#35433F` | Button surfaces, ground and building body |
| Accent (10% maximum) | `#F0C46A` | Start action border, focused action outline, player body and facing triangle |
| Text | `#F4F2E8` | Title, action labels, controls hint, error copy and player outline |
| Destructive | none | Quit has no saved progress to destroy in this phase |

Accent reserved for: Start border, keyboard focus outline on the active button, player body and directional marker. Quit uses the secondary surface except its focus outline. Separate shapes with the light text-colored outline where secondary fills meet. Hover brightens the existing surface; pressed darkens it; disabled reduces saturation and ignores activation. Keyboard focus always has an explicit 4px outline, including while hovered; it must not rely only on a color change. Keep body-text contrast at least 4.5:1 against its surface and essential shape/control boundaries at least 3:1.

## Layout and Interaction

### Main menu

Visual anchor: Start is the menu's primary focal point through its accent border, initial keyboard focus and position above Quit.

Use one full-viewport Control with a centered vertical container. Display `KingdomZombies`, leave 32px, then show `Start` and `Quit` as 240×48px minimum Buttons separated by 16px. At launch and after returning from play, focus Start. Tab/Shift+Tab and Up/Down cycle between the two buttons; Enter/Space and a primary mouse click activate the focused/pointed action. Maintain natural mouse hover and keyboard focus simultaneously. Do not add audio, artwork, settings, save slots or a pause overlay.

Each discrete press causes at most one transition. Lock both actions while committing a transition, and require release before another activation. Escape in play returns directly to this menu; Escape in the menu has no action. Quit exits immediately with no confirmation because this phase has no persistent run data. Menu input is consumed before any gameplay command route sees it. Return removes scene input/callbacks; a new Start creates exactly one fresh player with cleared movement input.

### Playable fixture

Visual anchor: the controlled player is the scene's primary focal point through the accent body, light outline, visible facing marker and camera following; the building remains secondary scenery.

Use flat repository-owned primitives: colored sky, continuous ground, a small building body with triangular roof and door, and one outlined static player with a side-facing triangle. The building is scenery drawn behind the player, with no interaction affordance or blocking collision. The player silhouette and triangle remain distinguishable while crossing the building. No generated image or donor asset is required.

Default geometry: 1u = 16 engine/logical pixels; player width 1u and height 2u; building width 6u and total height 4u; scene extends from −60u to +60u; player-center limits are −59u and +59u; spawn is −8u, facing right, with feet on the ground. These are temporary fixture dimensions, independent of recovered source-pixel density. The gameplay model owns units and bounds; presentation converts them rather than becoming a second movement authority. Walking remains 3u/s per the specification.

Use a 1280×720 logical viewport and proportional canvas scaling to 1920×1080. Preserve the existing canvas-items stretch approach; define the reference size explicitly. The camera shows 80u horizontally at 16:9, follows the active local player, and clamps to the scene. Apply the source baseline of 0.15-second following smoothing; snap to the initial camera position on entry and clamp the final camera position so smoothing cannot expose outside the scene. Keep ground at 75% of viewport height with continuous fill below it. Keep the complete player visible at both endpoints. The 80u framing is a presentation baseline from specification §13.2, not a source-asset scale.

Place the controls hint 24px from the top-left in a viewport-anchored overlay, independent of camera movement. Keep it above world geometry and inside the safe margin; it captures no mouse or movement input. Mirror only the visual facing marker when direction changes; retain the same actor identity. The latest nonzero direction persists after stopping. Losing window focus clears movement, and restoring focus requires a fresh press before movement resumes.

## Copywriting Contract

| Element | Copy |
|---------|------|
| Menu heading | KingdomZombies |
| Primary CTA | Start |
| Secondary action | Quit |
| In-scene controls hint | Move: A / D or Left / Right arrows. Esc: Main menu. |
| Empty state heading | Not applicable: no data-driven list, form or selectable saved run exists. |
| Empty state body | Not applicable: the fixture is locally authored and starts with exactly one player. |
| Error state | Could not start the scene. Try Start again. |
| Destructive confirmation | None: Quit closes this temporary session immediately. |

`Start` is the locked short label from BASE-01; do not replace it to satisfy a generic verb-plus-noun convention. If scene entry reports a recoverable failure, retain/restore the menu, display the error copy below the actions with 16px spacing, re-enable Start and Quit, and focus Start. The message wraps within 480px; retry clears the previous error. Log diagnostic detail for developers, outside player copy. A fatal engine/process failure remains a failing smoke result, not a claimed recoverable state.

## UI Considerations

Post-approval probe executed on 2026-11-14 using the installed `.codex/gsd-core/bin/lib/ui-consideration-probe.cjs` module through Node `proposeElements(elements)` and `analyzeCoverage(elements, resolutions)`. Autonomous kind confirmation retained these authored classifications: E1 menu actions = nav + interactive-control; E2 title/hint/error labels = static-content; E3 fixture visuals = media. Each returned category was resolved explicitly against this contract and SPEC AC-01, AC-02, AC-07, AC-08 and AC-11–AC-12. This is specification coverage, not executed game acceptance evidence.

| Category | Element(s) | Status | Resolution / Reason |
|----------|------------|--------|---------------------|
| loading | E1 menu actions | ✅ covered | Local scene transitions lock activation until success or failure; held input cannot repeat activation. |
| error | E1 menu actions | ✅ covered | Recoverable entry failure restores the menu, displays the contracted error, focuses Start and permits retry or Quit. |
| overflow | E1 menu actions | ✅ covered | Menu content stays within 24px screen margins at both supported resolutions without clipping or overlapping actions. |
| long-text | E1 menu actions | ✅ covered | Start and Quit fit their minimum widths; essential action labels are never ellipsized. |
| overflow | E2 title/hint/error labels | ✅ covered | Title, hint and error remain inside 24px margins; body text wraps without hiding actions. |
| long-text | E2 title/hint/error labels | ✅ covered | Hint and error text wrap and grow vertically; all essential English instructions remain visible at both supported resolutions. |
| empty | E3 fixture visuals | ✅ covered | Missing ground, building or player fails rendered acceptance; an empty fixture cannot pass. |
| loading | E3 fixture visuals | ✅ covered | All authored primitives are ready before play input is enabled. |
| error | E3 fixture visuals | ✅ covered | Recoverable construction failure returns to the menu error path; fatal or invisible-scene failures fail the smoke result. |
| populated | E3 fixture visuals | ✅ covered | Play shows exactly one player, one decorative main building, continuous ground and colored background with readable facing. |

Probe result: 10 applicable, 10 resolved with explicit verification, 0 backstop, 0 unresolved, 0 unclassified. Form, collection, partial-data and zero-one-many data states do not apply because there are no forms or variable-length collections. The fixed two-action menu is navigation, not a data collection.

## Registry Safety

| Registry | Blocks Used | Safety Gate |
|----------|-------------|-------------|
| none | none | Not applicable: native Godot controls and authored primitives only; no shadcn or third-party registry. |

## Verification Contract

Follow BASE-03 and AC-01–AC-13 without substituting headless results for rendered production input. On both Windows and Linux, retain current-run images of launch menu, initial fixture, left-facing movement, right-facing movement and returned menu. Exercise both 1280×720 and 1920×1080; inspection checks readability, keyboard focus, full player/marker visibility, building traversal and screen-edge bounds. Record actual visual review separately from image capture. No passing result is implied by writing this contract.

## Checker Sign-Off

- [x] Dimension 1 Copywriting: PASS
- [x] Dimension 2 Visuals: FLAG — approved with nonblocking anchor recommendation; explicit menu/scene visual anchors added above.
- [x] Dimension 3 Color: PASS
- [x] Dimension 4 Typography: PASS
- [x] Dimension 5 Spacing: PASS
- [x] Dimension 6 Registry Safety: PASS
- [x] Dimension 7 Inventory Provenance: PASS

**Approval:** approved 2026-11-14; checker returned six PASS dimensions and one nonblocking Visuals FLAG. The recommendation is addressed by the explicit anchors; no repeat checker pass is claimed.
