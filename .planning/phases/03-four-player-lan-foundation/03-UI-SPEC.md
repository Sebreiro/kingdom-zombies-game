---
phase: "03"
slug: "four-player-lan-foundation"
status: approved
shadcn_initialized: false
preset: none
created: "2026-11-15"
---

# Phase 03 — UI Design Contract

This contract extends the completed Phase 2 foundation for LAN-01–LAN-04. Binding behavior comes from [03-SPEC.md](03-SPEC.md), especially AC-01–AC-08 and AC-12–AC-15. [03-CONTEXT.md](03-CONTEXT.md) D-01 delegates routine choices without another interview; D-02 makes the choices below agent defaults, not additional creator-approved requirements. D-03 requires English copy; D-04 carries forward the Phase 2 presentation. No Phase 3 RESEARCH.md existed during the scout.

**Current testing authority:** [local-testing.md](../../../docs/workflow/local-testing.md) and [ai-collaboration.md](../../../docs/workflow/ai-collaboration.md) supersede older local two-OS/VM and extra-resolution testing text in phase artifacts. Implement and review the complete phase before rendered tests; routine local windows use Windows at 1280×720 with no VM. Product layout still supports 1280×720 and 1920×1080. Separate hosted/native Linux and release procedures retain their own honest evidence scope.

## Design System

| Property | Value |
|----------|-------|
| Tool | none |
| Preset | not applicable |
| Component library | Existing Godot native Controls, local Theme/StyleBoxFlat resources and authored scene primitives |
| Icon library | none; retain the drawn directional triangle |
| Font | Godot bundled fallback; existing FontVariation bold treatment; no OS font dependency |
| Baseline | Phase 2 Foundation.tscn, FoundationWorld.tscn, FoundationRoot.cs, FoundationInput.cs and FoundationView.cs |

Scout commands: `rg --files src/godot-game` and `rg --files -g components.json -g package.json -g '*theme*' -g '*.ttf' -g '*.otf' src .planning/phases/03-four-player-lan-foundation`. The production entry point is `res://foundation/Foundation.tscn`. Its embedded native Theme already defines normal, focus, hover, pressed and disabled button states. There is no React stack, shadcn configuration, external design-system package or installed font asset; shadcn initialization, package enumeration and the Component Inventory section are inapplicable. Native control choices below are implementation guidance, not an exhaustive package inventory. The configured-agent-skills query returned no additional researcher skills.

Reuse the current theme rather than introducing another visual system. Use Button, Label, LineEdit, OptionButton and native scrolling/container controls for the new surfaces, with the same explicit focus styles. Use no remote assets or third-party UI blocks. This contract specifies presentation; service/protocol choices remain technical research work.

## Spacing Scale

All UI measurements are logical pixels at the 1280×720 reference viewport. Retain proportional canvas scaling to 1920×1080.

| Token | Value | Usage |
|-------|-------|-------|
| xs | 4px | Focus outlines and short marker separation |
| sm | 8px | Label-to-field gaps, compact row text gaps |
| md | 16px | Control gaps, row padding and major container separation |
| lg | 24px | Screen safe margin, body-text line box |
| xl | 32px | Main title-to-content gap |
| 2xl | 48px | Minimum button/field height and title line box |
| 3xl | 64px | Browser row height and reserved major spacing |

Exceptions: none for UI spacing. Buttons and fields have a 48px minimum interactive height. Main action buttons are at least 240px wide; utility buttons are at least 144px wide. Focus outlines are 4px and remain within the safe margin. Existing 2px world strokes and unit-based fixture geometry are inherited drawing measurements, not new UI spacing tokens.

## Typography

Exactly three sizes and two weights, inherited from Phase 2: 16/24/32px; regular 400 and bold 700. Reuse the bundled font and existing local bold derivation.

| Role | Size | Weight | Line Height |
|------|------|--------|-------------|
| Body / field label / row details / feedback / player marker | 16px | 400 | 1.5 |
| Label / action / editable value / session name | 24px | 400 | 1.5 |
| Heading / screen title | 32px | 700 | 1.2 |

Keep main-menu title/actions centered. Left-align form labels, browser rows, status and errors. Do not shrink text to fit. Essential instructions, action labels, counts and status words remain fully visible; ordinary body copy wraps. A LineEdit scrolls its editable text horizontally. Display-only names/endpoints may ellipsize as specified below without changing stored values or selection identity.

## Color

Retain the Phase 2 palette. The 60/30/10 split defines emphasis, not exact screenshot area; the sparse world remains predominantly sky.

| Role | Value | Usage |
|------|-------|-------|
| Dominant (60%) | `#18252B` | Menu/browser backgrounds, world sky and dark inset fields |
| Secondary (30%) | `#35433F` | Buttons, browser rows, compact status surfaces, ground and building |
| Accent (10% maximum) | `#F0C46A` | Start border, active keyboard-focus outline, selected-session inset marker, local actor body/facing marker and You marker |
| Text / essential boundary | `#F4F2E8` | Labels, feedback, outlines, participant numbers and remote actor bodies |
| Destructive | none | Direct Escape and Quit retain the temporary-session behavior; no persistent data deletion exists |

Accent reserved for: Start border; the currently focused control; the selected session's 4px inset marker; the locally controlled actor's body, facing triangle and You marker. Host/Join controls use the secondary surface until focused. Errors use explicit text and an outline, not a new warning color. Remote actors use the text-colored body/triangle with dark inset detailing and a light outline; participant numbers distinguish them independently of color.

Reuse existing hover `#44554F`, pressed `#252F2C` and disabled `#2F3331` surfaces. A selected row retains its inset marker when another control takes focus. Focus has an outer 4px outline, so focus and selection are independently visible. Body text must retain at least 4.5:1 contrast; essential shapes/control boundaries at least 3:1. No pulsing, animated spinner or color-only success/error signal is required.

## Layout and Interaction

### Shared input and transition rules

Preserve Phase 2's consumed menu input, explicit keyboard focus, single transition per fresh press and release-before-reactivation rules. Tab/Shift+Tab move through enabled controls in visual order. Enter/Space activate a focused button. Space in a text field inserts a space; Enter in the address field submits that address only; Enter in the display-name field commits that edit only. Arrow keys edit text or navigate an open selector/list in their own context. They cannot also reach movement. Native option popups receive their own keys; Escape closes the popup before acting on the enclosing screen.

Do not retain the existing two-button assumption that any focused control other than Start means Quit. Route activation to the actual focused control. Disabled controls cannot activate, and focus must never remain on a hidden/disabled control. Focus changes and page transitions clear held gameplay input; reopening play requires a fresh movement press. Mouse hover and keyboard focus coexist.

One owned transition/admission may be active per process. Lock other session-entry actions until it resolves; Cancel join remains usable while joining. A late callback after Cancel join/Back, menu return or a newer attempt cannot change the current screen, show an old error, restore old actors or move focus. Join progress does not pause existing participants. No ready/waiting lobby is added.

### Main menu and local profiles

Retain the centered KingdomZombies title and Start focal point. Expand the existing menu to a maximum width of 640px inside 24px safe margins. Use this vertical order: title; 32px gap; Local profile selector; Display name field with New profile utility button; 16px gap; Start, Host session, Join LAN, Quit. Main actions remain a centered 240px-wide stack with 48px height and 16px gaps. The compact profile section uses 16px labels, 48px inputs and 8px label gaps. Reserve two wrapping feedback lines below the actions with an 8px gap; fit the entire menu inside the 672px available height without hiding Quit. At launch and ordinary play return, focus Start.

Create and select the first profile automatically when no profile exists, with default display name Player 1. New profile creates/selects another persistent profile with default Player N, where N is a local presentation ordinal. It receives its own stable identity. The selector shows `Profile N · {display name}`; duplicate display names remain separate entries through Profile N. Ordinals are presentation labels, never authority IDs. Do not expose opaque identity strings, transient peer numbers or storage paths as normal player copy.

Editing a display name commits on Enter, focus loss or a session-entry action; preserve Unicode and do not silently shorten the stored name. Empty/whitespace-only edits show the specified validation message and block entry until repaired. New profile remains usable to recover from an unavailable selected profile. Persistence failure is visible and prevents an apparently successful profile switch/entry. Do not silently replace an unreadable existing identity. A selector with many profiles scrolls, keeps the selected item visible and preserves selection by profile identity. Editing a name leaves that identity unchanged. No profile deletion or management screen is introduced.

Host session starts the existing foundation immediately with the selected profile. Default session label is `{display name}'s camp`; it is display text, never a session key. No extra naming modal is required. Start retains the ordinary solo flow. Join LAN opens the browser and focuses Refresh. Quit closes the process directly from the menu. The field/profile UI remains outside the playable fixture.

### Basic LAN browser

Use one centered browser panel with maximum width 1120px, available height 672px and 24px screen margins. Its vertical sections are: a single 48px heading row with LAN sessions at left and a compact Playing as summary at right; a 48px session-list heading row with Refresh followed by 8px and the scrollable rows; Host address label and LineEdit with Join address; feedback; footer with Join session and Back. Keep 16px between these five sections and 8px between labels and their content. The list flexes into remaining height with a 192px minimum showing three 64px rows. Feedback reserves up to three 24px body lines. This minimum layout consumes 660px, including the 64px total inter-section gaps. Footer controls remain visible. Shorten only the summary's display name if needed, with full focus/hover detail. At 1920×1080, proportional canvas scaling preserves the same layout rather than shrinking typography.

Each 64px row has two lines with 16px horizontal padding. First line: session name at 24px and a reserved right-hand participant count `{N}/4`. Second line: host label or endpoint at 16px, with availability word Available, Full or Unavailable. The count includes occupied and pending reservations as reported by the host. Keep count/status visible even with long session names; ellipsize the name and endpoint within their own columns. Hover and keyboard-focused row detail show the full original name and endpoint in a wrapping tooltip/detail surface within safe margins. Do not rely on hover alone. Missing optional host label falls back to the endpoint. Missing required session identity/endpoint yields no joinable row.

Refresh starts an immediate scan; the browser also refreshes periodically while open. Exact scan cadence belongs to the connection implementation. Show discovery feedback separately from connection feedback. Retain valid rows while scanning; keep manual entry and Back available even when discovery fails. A successful initial scan with no results shows the contracted empty heading/body. One result uses the same row layout as many; use `1 session` and `{N} sessions`. Overflow scrolls vertically by mouse wheel and keyboard with the focused row kept visible; never introduce horizontal list scrolling.

Click or keyboard navigation selects a row. Selection is keyed by session identity across refresh, reordering and duplicate announcements. Equal labels/endpoints do not merge different sessions. If the selected session disappears, clear selection, disable Join session and show the selected-session-vanished message without moving focus to a different host. Full rows stay visible/selectable for inspection but Join session is disabled. Refresh cannot steal focus from an address edit. An unselected/empty list leaves Join session disabled and manual joining available.

Joining always revalidates the actual host; a stale Available row never supplies readiness or capacity. Join session submits the selected session only. Join address submits the address field only, so changing row selection cannot redirect manual submission. Use a visible host-address example placeholder `192.168.1.20`; the actual accepted address/optional-port grammar must match the production connection adapter. Invalid entry remains editable with focus/caret preserved. No pasted credentials, account or Internet service are requested.

### Connecting, cancellation and recovery

After either Join action, replace the browser's feedback with Connecting to {host}… and show Cancel join in place of the Join controls. Lock profile/endpoint/row changes for that attempt. Retain enough browser context to show the chosen host; keep Cancel join as the initial focus. Once transport admission succeeds but state is not yet installed, show Receiving session state… instead. Do not present an actor, ready status or movement affordance before the coherent baseline is installed. There is no fabricated percentage/progress bar.

Successful readiness enters the foundation exactly once and clears join feedback and held movement. Cancel join or Escape during joining closes that attempt and restores the browser, retaining its field/selection if still valid and focusing the originating Join action, or the address field if invalid/unavailable. Back/Escape from the idle browser returns to the main menu and focuses Join LAN. At the default 30-second join deadline, display the timeout copy and restore usable join controls. Repeated clicks/held activation cannot reserve extra slots or create multiple actors.

Recoverable errors remain in the current screen's feedback area until another attempt, edit or explicit navigation clears them. Errors wrap without covering inputs/actions and name the problem plus a next step. Address validation focuses its field; a duplicate-profile rejection focuses Back so the user can select/create another profile; other join failures restore focus to the originating action when usable. Diagnostics/protocol IDs go to developer logs, outside player copy.

### Multiplayer fixture and session lifetime

Retain the Phase 2 sky, ground, nonblocking decorative building, player proportions, direction triangle, 16 logical px/u conversion, 3u/s baseline, scene limits and 0.15-second local camera smoothing/clamping. Each process follows only its locally assigned actor, snapped on entry; it need not keep every remote actor on screen. Camera technique cannot become another movement authority.

Give each ready participant a stable-in-session presentation marker P1–P4. Host is P1; guest markers remain attached to their actor for that actor's lifetime. They are labels, not actor/player/session identifiers. Render each marker above its actor; append You for the local actor. Keep labels at 16px on a dominant-surface backing with 4px inset. The local body/facing marker retain gold; remote bodies/markers use the neutral scheme. When actors overlap, stack/offset the screen-space labels enough to read every P-number; this changes labels only, never simulation positions. Remove departed actors and their labels together.

Keep the inherited controls hint 24px from top-left in a mouse-ignoring overlay, maximum width 720px. At top-right, within 24px, use a 320px-wide compact text block for session role/count and participant names. The block has a one-line header, then at most four 24px participant lines with `P{N} · {display name}` and local `(You)`. Long display names ellipsize within their column and expose full content through the same focus/hover detail approach when the block is inspected; P-number and You never truncate. The HUD does not take gameplay focus or consume movement keys; full names are also available in browser/profile text before play. Separate upper-left and upper-right regions so no hint/roster text overlaps at 1280×720. Solo may retain its minimal Phase 2 hint with no roster.

The host header reads Hosting · {N}/4 players; guests read Connected · {N}/4 players. For one participant use Hosting · 1/4 player. Do not label an unready participant as controllable. Remote join/leave updates the count/roster without resetting remaining actors/cameras. A guest departure removes exactly its placeholder; a replacement can acquire the released slot without inheriting stale control.

Escape in play returns directly to the main menu, as locked by AC-01. Host Escape ends the shared temporary session; guest Escape leaves only that guest. No confirmation modal interrupts this inherited transition. The host hint explicitly says it ends the session, so the action is understandable before pressing it. Quit remains a main-menu action with no save/deletion confirmation. Graceful host departure returns guests to a usable main menu with the host-ended message. Unexpected host loss does so at the default 10-second loss condition with the connection-lost message. Focus Start and retain the explanation until the next entry/navigation action. If a connection adapter reports a pending loss before the threshold, show Waiting for host… as textual HUD feedback; do not invent a second timeout or claim recovery before a current response. Session return closes old acceptance, clears input/roster/actors and permits fresh Host/Join without old messages or callbacks.

## Copywriting Contract

| Element | Copy |
|---------|------|
| Menu heading | KingdomZombies |
| Inherited primary CTA | Start |
| Host CTA | Host session |
| Browser CTA | Join LAN |
| Quit action | Quit |
| Profile label / create action | Local profile / New profile |
| Name label / validation | Display name / Enter a display name to continue. |
| Default profile display name | Player {N} |
| Profile summary | Playing as {display name} · Profile {N} |
| Profile persistence error | Could not save this profile. Try again or choose another profile. |
| Profile load error | Could not load this profile. Choose another profile or create a new profile. |
| Browser heading / refresh action | LAN sessions / Refresh |
| Browser list heading | Available sessions |
| Empty state heading | No LAN sessions found |
| Empty state body | Refresh to search again, or enter a host address below. |
| Discovery loading | Searching for LAN sessions… |
| Discovery service error | LAN discovery is unavailable. Refresh to try again, or enter a host address. |
| List count | 1 session / {N} sessions |
| Row availability | Available / Full / Unavailable |
| Selected host disappeared | This session is no longer available. Refresh or enter a host address. |
| Address label / placeholder | Host address / 192.168.1.20 |
| Join CTAs | Join session / Join address |
| Navigation / cancellation | Back / Cancel join |
| Address validation error | Enter a valid host address, then choose Join address. |
| Connecting state | Connecting to {host}… |
| Baseline-installation state | Receiving session state… |
| Cancelled attempt | Join cancelled. Choose a session or enter a host address. |
| Join timeout | Joining timed out. Check the host address and try again. |
| Unreachable/closed host | Could not connect to the host. Check the address and that the host is running, then try again. |
| Full session refusal | This session is full (4/4). Choose another session or try again later. |
| Incompatible session | This session uses an incompatible game version. Use the same game build as the host, then try again. |
| Duplicate profile refusal | This profile is already connected. Go Back and choose or create another local profile. |
| Invalid identity refusal | This profile could not join. Go Back and choose or create another local profile. |
| Connection service unavailable | LAN connection is unavailable. Try again after checking your network. |
| Host creation error | Could not host a session. Check your network and try Host session again. |
| Solo entry error | Could not start the scene. Try Start again. |
| Guest scene entry error | Could not open the session scene. Try joining again. |
| Solo / guest controls hint | Move: A / D or Left / Right arrows. Esc: Main menu. |
| Host controls hint | Move: A / D or Left / Right arrows. Esc: End session and return to main menu. |
| Host HUD header | Hosting · {N}/4 players |
| Guest HUD header | Connected · {N}/4 players |
| Local participant marker | P{N} · You |
| Temporary host-response feedback | Waiting for host… |
| Graceful host departure | The host ended the session. Choose Start, Host session or Join LAN to play again. |
| Unexpected host loss | Connection to the host was lost. Choose Join LAN to try again, or start a new session. |
| Destructive confirmation | None. Escape leaves/ends this temporary session directly; Quit closes the menu process. No persistent data is deleted. |

Start remains the locked short label; do not rename it to satisfy a generic verb-plus-noun rule. Host session and Join session supply explicit verb/noun LAN CTAs. Use the contracted singular form for one player. Interpolate names as plain display text, never markup or authority identifiers. Store full Unicode text independently of visual shortening. Copy specifies distinct failure classes; implementation must map actual service errors to these reasons rather than promise a retry will succeed.

## UI Considerations

Authored elements: E1 main/browser navigation and buttons = nav + interactive-control; E2 local profile selector/name form = form + list-collection + interactive-control; E3 LAN session rows = list-collection + interactive-control; E4 host-address form = form + interactive-control; E5 headings/hints/feedback = static-content; E6 fixture actors/markers = media; E7 participant roster = list-collection + static-content. These classifications include variable profiles, network rows and participant collections rather than treating them as fixed navigation.

The installed `.codex/gsd-core/bin/lib/ui-consideration-probe.cjs` was run on 2026-11-15 using `proposeElements(elements)` and `analyzeCoverage(elements, resolutions)`, with the authored classifications above and resolutions parsed from this table. Result: 39 applicable, 39 explicitly resolved, 0 backstop, 0 unresolved, 0 unclassified. The rows below reference the copy table instead of duplicating messages. These are design obligations, not game-test passes.

| Category | Element(s) | Status | Resolution / Reason |
|----------|------------|--------|---------------------|
| loading | E1 navigation/actions | ✅ covered | Entry locks competing actions; joining leaves Cancel join enabled; completion/failure restores the correct focus and cannot double-enter. |
| error | E1 navigation/actions | ✅ covered | Recoverable Start/Host/Join errors use the contracted reason and restore usable actions on the owning page. |
| overflow | E1 navigation/actions | ✅ covered | Panels remain within 24px safe margins with footer actions visible; no horizontal navigation scrolling is needed. |
| long-text | E1 navigation/actions | ✅ covered | Essential action labels remain complete at 24px; descriptive feedback wraps independently. |
| empty | E2 profile form/selector | ✅ covered | First launch creates/selects Player 1; blank name edits show validation and cannot enter a session. |
| loading | E2 profile form/selector | ✅ covered | Profile load/save completes before entry; pending profile work cannot race an entry or create apparent identity success. |
| error | E2 profile form/selector | ✅ covered | Load/save failures stay visible with choose/create/try-again recovery; an unreadable identity is not silently regenerated. |
| populated | E2 profile form/selector | ✅ covered | Selected Profile N and its editable display name are visible; equal names retain separate identity-backed entries. |
| partial | E2 profile form/selector | ✅ covered | An incomplete name edit remains editable and blocks only session entry; New profile provides a recovery route. |
| overflow | E2 profile form/selector | ✅ covered | Many profiles scroll inside the selector; selected entry remains visible without expanding the main menu off-screen. |
| zero-one-many | E2 profile form/selector | ✅ covered | Zero initializes one profile; one is selected; many are independently selectable by persistent identity. |
| long-text | E2 profile form/selector | ✅ covered | Name entry scrolls horizontally without changing Unicode; selector display can ellipsize with full focus/hover detail. |
| empty | E3 session rows | ✅ covered | The contracted empty state appears after a successful empty scan; address entry remains available. |
| loading | E3 session rows | ✅ covered | Search feedback retains valid existing rows and manual joining; refresh does not steal typing focus. |
| error | E3 session rows | ✅ covered | Discovery failure, vanished selection and stale/full join rejection have distinct recovery feedback. |
| populated | E3 session rows | ✅ covered | Each row shows name, host/endpoint, N/4 and availability; selection is by session identity. |
| partial | E3 session rows | ✅ covered | Missing optional host label falls back to endpoint; missing required identity/endpoint cannot produce a joinable row. |
| overflow | E3 session rows | ✅ covered | More than three visible rows scroll vertically; keyboard navigation reveals the focused row; counts/status retain space. |
| zero-one-many | E3 session rows | ✅ covered | Empty/one/many scans use correct count wording and one consistent row layout; duplicate advertisements merge by identity only. |
| long-text | E3 session rows | ✅ covered | Names/endpoints ellipsize within columns with full focus/hover detail; Unicode and identity remain unchanged. |
| empty | E4 address form | ✅ covered | Empty submission produces field validation rather than opening a connection or slot. |
| loading | E4 address form | ✅ covered | Connecting and baseline installation use distinct feedback; endpoint changes are locked until Cancel join/failure. |
| error | E4 address form | ✅ covered | Invalid address, unreachable/full/incompatible host, timeout and identity failure map to distinct contracted copy/recovery. |
| partial | E4 address form | ✅ covered | Incomplete input stays in the field; invalid submission preserves caret/value and creates no actor. |
| long-text | E4 address form | ✅ covered | The editable address scrolls horizontally; validation/status wraps without hiding Cancel join or join actions. |
| overflow | E5 headings/hints/feedback | ✅ covered | Labels stay inside page/HUD regions; feedback grows within its reserved region without covering footer controls. |
| long-text | E5 headings/hints/feedback | ✅ covered | Essential hints/errors wrap at 16px; interpolated labels are plain text; no instructions are ellipsized. |
| empty | E6 fixture actors/markers | ✅ covered | No local actor is shown before readiness; a ready scene requires its assigned local actor and all currently ready visible peers. |
| loading | E6 fixture actors/markers | ✅ covered | Coherent baseline installation precedes actor/control publication; existing participants remain active. |
| error | E6 fixture actors/markers | ✅ covered | Recoverable scene construction returns to owning feedback; invisible/missing ready actors fail acceptance. |
| populated | E6 fixture actors/markers | ✅ covered | One through four ready actors use distinct P-markers; local You, facing and independent camera remain readable. |
| empty | E7 roster | ✅ covered | Solo can omit the roster; active LAN always includes at least the host after readiness; teardown clears old rows. |
| loading | E7 roster | ✅ covered | Roster is published from accepted current state; pending admission never claims controllable readiness. |
| error | E7 roster | ✅ covered | Host loss returns to the menu explanation; departed actors/rows are removed without resetting remaining control. |
| populated | E7 roster | ✅ covered | At most four P-number/name lines identify ready actors, local You and session count. |
| partial | E7 roster | ✅ covered | Join/install keeps the incoming actor unready; labels never substitute for missing ownership/baseline state. |
| overflow | E7 roster | ✅ covered | Four 24px rows fit the 320px HUD block; it remains separate from the left controls hint. |
| zero-one-many | E7 roster | ✅ covered | Host-only uses singular wording, two through four use plural, and ended sessions show no stale participant rows. |
| long-text | E7 roster | ✅ covered | Display names shorten in their column; P-number/You remain complete; full names remain available before play. |

## Registry Safety

| Registry | Blocks Used | Safety Gate |
|----------|-------------|-------------|
| none | none | Not applicable: native Godot resources only; no shadcn or third-party registry |

## Verification Contract

Save reusable Phase 3 scenarios with source ACs, importance/rationale, setup, production actions, explicit expected results, evidence checkpoints and ordered run groups. Select all active Critical cases plus every new/changed/affected case, including inherited Start/Quit, facing/camera, focus-loss and Escape regressions. Implement the entire phase and fix code-review findings before opening game windows. This spec creates no rendered evidence.

Routine rendered scope is Windows 1280×720 without a VM. Reuse compatible host/guest processes across scenarios while preserving fresh scenario state through normal menu controls. Four-player checks still require host plus three guests; capacity rejection can require a fifth attempt, declared as an additional process/role rather than silently reducing participants. End/quit checks run last where practical. After repairs, run focused failed/affected checks, then the complete selected set on the repaired build. Record omissions and separate hosted/Linux, both-supported-display and physical mixed-computer release evidence honestly; old artifacts cannot reinstate routine VM or extra-resolution requirements.

Required visual checkpoints cover main/profile controls; empty/populated/overflowing browser; long Unicode/equal-name rows with stable selection after refresh; manual input validation; joining, baseline-installation and Cancel join; full/incompatible/duplicate-profile/timeout recovery; four ready P-markers/roster/You; independent walking/facing/cameras; focus loss; guest departure/slot reuse; host-ended/lost messages and a fresh rehost/rejoin. Inspect actual appearance and keyboard focus in addition to saving screenshots. Thresholds remain the locked 30-second join timeout, 10-second response-loss timeout and inclusive synchronization limits; use controlled-time checks for boundary timing, not a UI animation as timing evidence.

## Checker Sign-Off

- [ ] Dimension 1 Copywriting: PASS
- [ ] Dimension 2 Visuals: PASS
- [ ] Dimension 3 Color: PASS
- [ ] Dimension 4 Typography: PASS
- [ ] Dimension 5 Spacing: PASS
- [ ] Dimension 6 Registry Safety: PASS
- [ ] Dimension 7 Inventory Provenance: PASS

**Approval:** pending; researcher draft, no checker or rendered acceptance pass claimed.
