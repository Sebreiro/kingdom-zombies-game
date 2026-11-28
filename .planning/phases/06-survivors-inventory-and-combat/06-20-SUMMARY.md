---
phase: 06-survivors-inventory-and-combat
plan: "20"
subsystem: ui
tags: [godot, csharp, hud, inventory-workspace, focus, draw-observer, gap-closure]

requires:
  - phase: 06-19
    provides: rendered runner on one prepared imported copy and measured per-case timing
provides:
  - Default workspace focus on the selected nearby control or equipment_headwear, never Close inventory
  - Headless inventory proof that Space while browsing keeps the workspace open and the survivor grounded
  - Empty action-bar tiles that read "Slot {n}" over an unclipped "empty"
  - World help on a stable dark backplate, hidden while any inventory surface is open
  - Equipment slots with a wrapped `Label` region and a separate `Icon` region
  - `controls[].clipped` in draws.ndjson for the rendered capture predicates of 06-21 and 06-22
affects: [06-21, 06-22, 06-24, 06-25, 06-15]

actuals:
  tokens: 71280    # chars/4 over the realized diff: 26114 source chars + 259007 derived spans-registry chars (base64 snapshots)
  tasks: 3
  commits: 4
plan_head_before: 91b96a6fd2dbed435b23274df205376099d26265
plan_head_after: bf003d1

tech-stack:
  added: []
  patterns:
    - "A HUD label is added to its parent before it is sized, so its minimum size uses the HUD theme font"
    - "A slot control is a label region over a fixed 34 px icon region; a wrapped label adds one 24 px line"
    - "The root shows the world help every drivable frame; the HUD re-hides it from VisibilityChanged while browsing"

key-files:
  created: []
  modified:
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.Layout.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.External.cs
    - src/godot-game/game/foundation/FoundationInventoryGrid.cs
    - src/godot-game/game/foundation/FoundationHud.Live.cs
    - src/godot-game/game/foundation/FoundationDrawObserver.cs
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - docs/code/engine-generated-spans.json
    - src/godot-game/tools/verification/required-cases.json

key-decisions:
  - "World help hiding lives in the HUD: FoundationRoot sets Hint.Visible after the HUD observes, so the new guarded presenter FoundationHud.KeepHintClosed() re-hides it from VisibilityChanged while browsing"
  - "The world help uses the HUD body font (Liberation Sans 16, line spacing 5) at (636, 7, 290, 97); the longest WorldHint needs four lines, so the backplate rises into the top 24 px margin instead of scrolling or clipping"
  - "Quick-food slots moved to y 464 under the equipment column and workspace feedback to x 336 under carried storage, so eleven labeled slots fit between y 44 and 448"
  - "clipped is computed inside the existing ObserveControl member, so the 45-tuple LAN permission inventory is unchanged"

patterns-established:
  - "Engine-access presenters: every new Godot-touching member is added to EngineAccessPolicy._presenters in the same commit"
  - "After any game C# change, re-derive currentProducerReplay with current-replay.cjs --write and rebind the spans registry file hash"

requirements-completed: [INV-02, INV-07, INV-09, CHAR-04, CHAR-06]

coverage:
  - id: D1
    description: "Space while browsing keeps the inventory open and the survivor grounded (default focus never on Close inventory)"
    requirement: "INV-02"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case phase06.headless.inventory,phase06.headless.world (run fc7602ca, PASS 2/2; RED c73a0a6e)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Empty action-bar tiles read 'Slot {n}' over an unclipped 'empty'; world help on a stable backplate, hidden while browsing"
    requirement: "INV-09"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,phase06.headless.world,phase06.headless.inventory (run 1ca8f30a, PASS 3/3)"
        status: pass
    human_judgment: true
    rationale: "Headless runs build and exercise the HUD but cannot show legibility or the help being hidden; rendered judgment belongs to 06-25 and 06-15"
  - id: D3
    description: "Equipment slots with separate Label and Icon regions; draws.ndjson reports clipped text"
    requirement: "INV-07"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,phase06.headless.inventory,phase06.headless.start,lan-checks.core.permissions (run 78656c23, PASS 4/4)"
        status: pass
    human_judgment: true
    rationale: "No rendered run executed the clipped measurement in this plan; 06-21 and 06-22 consume it and 06-25/06-15 judge the slot layout"

duration: 90min
completed: 2026-11-25
status: complete
---

# Phase 6 Plan 20: Inventory focus, HUD text and labeled slots Summary

**Space no longer closes the inventory, empty action-bar tiles and world help render in full on the HUD body font, equipment slots carry a wrapped Label region above a separate Icon region, and draws.ndjson reports clipped text per control.**

## Performance

- **Duration:** about 90 min across two sessions (Task 1 by an earlier executor, Tasks 2 and 3 here)
- **Started:** 2026-11-25T15:51:23Z (first recorded run of the interrupted session)
- **Completed:** 2026-11-25T17:21:22Z
- **Tasks:** 3 of 3
- **Files modified:** 10

## Accomplishments

- Opening the workspace focuses the selected nearby control or `equipment_headwear`. Space while browsing neither closes it nor jumps. The headless inventory case now proves this (E068, E128, D-08, D-37).
- Empty hand and food tiles read the 16 px title over a full-width `empty`, with tooltip `Slot {n} empty` or `{Hand} empty`. The quiet bow-ammo tile stays blank (D-43).
- The world help sits on a stable dark backplate inside its band. It is hidden while any inventory surface captures the world, and the RestState label keeps the rest state visible (D-45).
- Each equipment slot keeps its node name and hit target. It has a `Label` child (16 px, word-smart wrap) and an `Icon` child below. Empty slots show an `Empty` marker in the icon region. The condition bar and `× Broken` marker also sit in the icon region.
- `ObserveControl` adds `clipped` to every observed control record.

## Task Commits

1. **Task 1: The jump key while browsing keeps the inventory open and the survivor grounded** - `731d839` (fix), committed by the earlier executor and verified here.
2. **Task 2: Empty action-bar tiles and world help text render in full** - `96a54db` (fix)
3. **Task 3: Labeled equipment slots and a clipped-text observation** - `2abf677` (fix)
4. **Derived pin re-derivation for the 06-20 sources** - `bf003d1` (chore)

## Task 1 verification and evidence

`git show 731d839` implements the whole action. `Draw` passes `_nearbyFocus ?? headwear ?? close` to `RestoreFocus`, and `DrawEquipment` returns the first slot button, `equipment_headwear`. The inventory case calls `jumpWhileBrowsing` right after `native equip view`. It taps `key.jump`, waits for a committed frame at least 250 ms of world time later, and requires every committed frame after the tap to be open, at `Ground`, and not `jump`. FoundationRoot.cs and FoundationInput.cs are untouched. No completion commit was needed.

Reports left by the interrupted session:

| Run | Revision | Result | Use |
| --- | --- | --- | --- |
| `e5f0b1b2` | `91b96a6`, dirty tree | inventory FAIL `JUMP_KEY_CLOSED_INVENTORY` (`false !== true`) | Not cited as RED. The dirty tree is not recoverable, so the test version it ran is unknown. |
| `94e8ef58` | `0acaac1`, a commit object not on the branch: the same Task 1 commit with a member-body comment | both cases BLOCKED: strict build `SCR0002` | None |
| `b094d300` | `efb4713`, a commit object not on the branch: the fix with an earlier `jumpWhileBrowsing` that chose frames by world `Elapsed` | world PASS, inventory FAIL `JUMP_KEY_CLOSED_INVENTORY` | The fix was present and the assertion still failed. Rows from the earlier world session in the same process can match an elapsed-time filter, which is probably why 731d839 filters by frame number. This run is not RED evidence. |
| `6dbdaf6d` | `731d839` | no summary.json; interrupted | None |

Because no genuine RED could be told apart, it was reproduced once. Only `FoundationInventoryWorkspace.cs` was restored from `731d839^`, keeping the new assertion. After the run the file was restored to HEAD and `git status` was clean before any further edit.

- **RED:** game-tests run `c73a0a6e-5273-4b13-b508-da58cb55747d` (revision 731d839, `clean: false` from the temporary revert): `FAIL phase06.headless.inventory JUMP_KEY_CLOSED_INVENTORY false !== true`. The inventory closed after Space.
- **GREEN:** game-tests run `fc7602ca-9962-48d8-bd59-b8d9139c663b` (revision 731d839, clean): `PASS 2/2` for `phase06.headless.inventory,phase06.headless.world`. Its `browsing` evidence frame 81 has `inventoryOpen: true`, actor `survivor-01` at `Y 0` with `Ground 0`, and `Movement: "idle"`.

## Final geometry at 1280×720

**Action bar** (row y 648–696, the 48 px band anchored 24 px above the bottom). Every tile ends before Open inventory at x 660.

| Tile | x range | Width | Title / empty text width at 16 px |
| --- | --- | --- | --- |
| Left hand | 24–130 | 106 | 67 / 44 in 98 |
| Right hand | 138–244 | 106 | 77 / 44 in 98 |
| Ready arrows | 252–360 | 108 | 98 in 100 |
| Slot 1–4 | 368–436, 440–508, 512–580, 584–652 | 68 | 41 / 44 in 60 |
| Open inventory | 660–830 | 170 | unchanged |

Inside a tile: Title (4, 2, w−8, 19). Empty Value (4, 23, w−8, 22), no trimming. Item Value (34, 21, w−39, 22), ellipsis, with the 27×24 icon at (5, 21). The item value keeps 67 px in a hand tile for `× Broken` (64 px).

**World help** (Hint): (636, 7, 290, 97), that is x 636–926, y 7–104. Backplate `#18252B`, content margins 4/3, inner width 282. In Liberation Sans 16 with line spacing 5 (24 px lines), the longest WorldHint ("…End session and return to main menu.\nShared stash out of reach. Rest is unsafe with an enemy nearby.") wraps to four lines, 91 px. A Godot 4.7.2 label measured 4 lines, 4 visible, minimum height 97. Bands: the load and modifier group now ends at x 628 (Money 452–628, Load and LoadModifiers 304–628; the widest modifier text measured 321 px). The roster starts at x 936. The bottom edge is y 104. Vitals end at x 288 and the action bar starts at y 648.

**Torso clothing slot**: workspace-content frame (16, 172, 96, 84), screen (40, 284, 96, 84). Label region (4, 2, 88, 46): two lines, 43 px. Icon region (4, 50, 88, 24). Condition bar (8, 76, 80, 4). The label region ends at y 48, before the icon region at 50. A one-line slot is 60 px tall: label (4, 2, 88, 22), icon at y 26, bar at y 52.

**Own equipment column**: left slots stack from content y 44 to 448 (screen 156–560), right slots from 44 to 360. The silhouette is unchanged at (120, 78, 80, 280). Quick-food slots are at content y 464–510 (screen 576–622). Feedback is at (336, 464) under carried storage. Nearby gear slots are 124×60 at a 64 px pitch.

## Files Created/Modified

- `FoundationInventoryWorkspace.cs` - headwear default focus (Task 1); DrawSlot `Label`/`Icon`/`Empty` children; measured wrap per slot; quick row and feedback placement
- `FoundationInventoryWorkspace.Layout.cs` - slot constants and `EquipmentFrame`
- `FoundationInventoryWorkspace.External.cs` - nearby gear frames 124×60
- `FoundationInventoryGrid.cs` - equipment icon and `× Broken` marker in the icon region
- `FoundationHud.Live.cs` - tile geometry and empty copy, Hint backplate and browse hiding, label add-before-size
- `FoundationDrawObserver.cs` - `clipped` in `ObserveControl`
- `phase06-headless.cjs` - Space-while-browsing assertion (Task 1)
- `EngineAccessPolicy.cs` - presenter tuple `FoundationHud.KeepHintClosed()`
- `docs/code/engine-generated-spans.json`, `required-cases.json` - re-derived replay and registry hash

## Decisions Made

See `key-decisions`. In short:
- Hiding the help is done in the HUD because the pinned FoundationRoot re-shows it after the HUD observes.
- A four-line help backplate was chosen over scrolling, because a scrolling label would itself be reported as `clipped`.
- Quick slots and feedback were moved so that labeled slots fit.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] FoundationRoot re-shows the Hint every frame after the HUD observes**
- **Found during:** Task 2
- **Issue:** `FoundationRoot.Observe` runs `_hud.Observe(...)` and then `hint.Visible = CanDrive`, so setting Visible in the HUD alone is overwritten, and FoundationRoot.cs must not change.
- **Fix:** `UpdateActions` hides the help while browsing, and the new presenter `KeepHintClosed()` handles `VisibilityChanged` to hide it again after the root shows it. `ObserveWorldHint` therefore reads the real hidden state. The presenter is registered in `EngineAccessPolicy._presenters`, a test file outside the task list.
- **Files modified:** FoundationHud.Live.cs, EngineAccessPolicy.cs
- **Verification:** run 1ca8f30a PASS 3/3; lan-checks.core.permissions and the engine group pass (runs 097b8658, 1a0a6e6c).
- **Committed in:** 96a54db

**2. [Rule 1 - Bug] The world help could not fit three lines in its band**
- **Found during:** Task 2
- **Issue:** In its original Open Sans 16, the longest WorldHint needs four or more lines at any width the band allows. Three lines need more than 400 px.
- **Fix:** The help uses the HUD theme (Liberation Sans 16, 24 px lines). The load and modifier group was narrowed to its measured text, giving the help x 636–926, and the backplate rises to y 7. **Trade-off:** y 7–24 is inside the UI-SPEC 24 px screen margin. The roster already starts at y 4 there.
- **Files modified:** FoundationHud.Live.cs
- **Committed in:** 96a54db

**3. [Rule 1 - Bug] Labels sized before entering the tree kept the fallback font's minimum**
- **Found during:** Task 3 geometry check
- **Issue:** Out of the tree, a Label's minimum size uses the default Open Sans (23 px lines). A wrapped "Torso clothing" label would have grown to 49 px, into the icon region. HUD titles grew from 19 to 23 px.
- **Fix:** Slot labels, the `Empty` marker and HUD `Text()` labels are now added to their parent before being positioned and sized.
- **Files modified:** FoundationInventoryWorkspace.cs, FoundationHud.Live.cs
- **Committed in:** 2abf677

**4. [Rule 3 - Blocking] Eleven labeled slots did not fit above the quick row**
- **Found during:** Task 3
- **Issue:** With 16 px labels over a 24 px icon, the slots need about 404 px. Only 356 px were free above the quick row at y 408.
- **Fix:** Quick-food slots moved to y 464, and workspace feedback moved to x 336 under carried storage. The broken marker and icon placement in FoundationInventoryGrid.cs (not in the file list) moved into the icon region. Nearby gear slots grew to 124×60.
- **Files modified:** FoundationInventoryWorkspace.cs, FoundationInventoryWorkspace.External.cs, FoundationInventoryGrid.cs
- **Committed in:** 2abf677

**5. [Rule 3 - Blocking] Stale current producer replay after the 06-20 source changes**
- **Found during:** Task 2 regression run
- **Issue:** `verification.engine` failed `engine.generated-boundary` at `boundary-reproduction-live` with `CURRENT_PRODUCER_INPUTS`. The replay hashes every game C# source, and 731d839 had already changed one.
- **Fix:** After the source commits, ran `current-replay.cjs --write` (57 historical, 22 changed, 22 added; sourceInputsSha256 `70f00cee…0576`). Rebound the two producer-stage registry hashes in required-cases.json from `cf8a25f6…3795` to `e0e3161b72dbdac4c55f328a933b9ff5c5c3744f8d669c2f496bc85481c88348`. The ac-08 bindings remain with 06-24.
- **Verification:** verification run 843036c7 PASS 22/22 within game-tests run 1a0a6e6c.
- **Committed in:** bf003d1

---

**Total deviations:** 5 auto-fixed (2 bugs, 3 blocking). **Impact:** Every deviation is needed to meet the contract within the pinned FoundationRoot. Two changes go beyond the plan: the moved quick row and feedback, and the help's upper edge at y 7.

## Issues Encountered

- Run 7a84ab32 failed the strict build on `SCR0009` (wrapped-argument indentation in FoundationInventoryGrid.cs). This was fixed before the commit.
- Run 85c3a819 reported "Source changed during execution" because deferred-items.md was edited while it ran. It was rerun untouched as 78656c23.
- `gameplay-checks.core.permissions` fails with `src/godot-game/core/CharacterPresentation.cs`. The same failure was recorded on 2026-11-24, and 06-20 changes no Core file. It is logged in deferred-items.md.

## Verification runs

| Purpose | Run | Result |
| --- | --- | --- |
| Task 1 RED (pre-fix workspace, new assertion) | c73a0a6e | FAIL JUMP_KEY_CLOSED_INVENTORY |
| Task 1 GREEN | fc7602ca | PASS 2/2 |
| Task 2 verify | 1ca8f30a | PASS 3/3 |
| Task 2 extras | 097b8658 | lan-checks.core.permissions PASS, basic.headless PASS, gameplay-checks.core.permissions FAIL (pre-existing), verification.engine 21/22 (stale replay, fixed in bf003d1) |
| Task 3 verify | 78656c23 | PASS 4/4 (lan-checks 3bc34a59) |
| Final regression at bf003d1 | 1a0a6e6c | PASS 6/6: verification.engine 22/22 (843036c7), basic.repository, basic.headless, phase06.headless.world, .inventory, .start |

No rendered run was made. Per the plan, rendered layout and image judgment belong to 06-25 and 06-15. The `clipped` field has not yet run in a rendered draw.

## Known Stubs

None.

## User Setup Required

None - no external service configuration required.

## Next Phase Readiness

- 06-21 and 06-22 can consume `controls[].clipped`. They are the first to exercise it in a rendered run and should confirm it is false for the slot labels, tile values and Hint.
- Later plans that change game C# must re-derive the current producer replay again (`current-replay.cjs --write`, then rebind the two registry hashes).

## Self-Check: PASSED

- Commits on the branch: 731d839, 96a54db, 2abf677, bf003d1 (`git log 91b96a6..HEAD`).
- Files present: FoundationInventoryWorkspace.Layout.cs, FoundationDrawObserver.cs (`clipped` present), FoundationHud.Live.cs (`"Empty"` count 0), 06-20-SUMMARY.md.

---
*Phase: 06-survivors-inventory-and-combat*
*Completed: 2026-11-25*
