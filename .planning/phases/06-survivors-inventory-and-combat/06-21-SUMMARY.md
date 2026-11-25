---
phase: 06-survivors-inventory-and-combat
plan: "21"
subsystem: testing
tags: [godot, csharp, rendered-driver, native-input, escape-order, settled-view, gap-closure]

requires:
  - phase: 06-20
    provides: controls[].clipped in draws.ndjson and the Space-while-browsing fix
provides:
  - "ordinary-cleanup.json is written only after every role's terminal record proves an ordinary Quit with exit 0"
  - "The solo world case walks with run.moveUntil (native D) instead of the undefined dash helper"
  - "settledView(draw) in phase06-rendered.cjs, used by every solo picture and ready for 06-22"
  - "A frame-stepped native drag: press, one drawn frame, the 4 px start threshold, one drawn frame, destination, held step, release"
  - "FoundationRoot._Input acknowledges every delivered key before workspace routing"
  - "menu() follows the D-14 Escape order, confirms each effect and reports the failing step"
affects: [06-22, 06-24, 06-25, 06-15]

actuals:
  tokens: 28100    # chars/4 over the realized diff (112398 chars, including the derived spans registry rows)
  tasks: 3
  commits: 4
plan_head_before: 6c472193b7d67fa409677381604a7ff011e9f503
plan_head_after: 54de9a1c37eb8a72f6d70d8885d96e4ef06a200d

tech-stack:
  added: []
  patterns:
    - "Rendered drivers take images only from settled, unclipped views (settledView)"
    - "A native gesture waits for a drawn frame between press, threshold crossing and destination"
    - "A reset names the D-14 step for each Escape and fails with that step when its effect is missing"
    - "tap() always delivers the release, so a failed press cannot turn the next press into an echo"

key-files:
  created:
    - src/godot-game/tests/scenarios/phase06-drivers.test.cjs
  modified:
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/scenarios/phase06-solo.cjs
    - src/godot-game/tests/scenarios/phase06-rendered.cjs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/tools/asset-pipeline/content.cjs
    - docs/code/engine-generated-spans.json
    - src/godot-game/tools/verification/required-cases.json

key-decisions:
  - "The split failure was on the driver side: the product opens QuantityToMove at the first motion at least 4 px from a Shift press, and run 1 never began any drag, so FoundationInventoryWorkspace.Input.cs is unchanged"
  - "The rendered close already wrote ordinary-cleanup.json after success (since 97cacc6); run 1's ENOENT followed from the failed Escape close. ended() now validates every terminal record itself instead of relying on caller order"
  - "settledView treats 'Opening inventory…' and 'Opening stash page…' as pending copy and requires every visible RunHud and InventoryWorkspace control to report clipped: false"
  - "Carried-board presence is checked through the installed own.* view and the drawn Item_ controls under CarriedStorage, because renamed empty boards (@Control@N) are not observed"
  - "The Escape receipt is recorded before routing; handled keys still return before _input, world actions and the menu Escape"

patterns-established:
  - "Any game C# change re-derives current-replay.cjs --write and rebinds the two spans registry hashes in a separate chore commit"

requirements-completed: [CHAR-02, CHAR-03, CHAR-06, INV-03, INV-04, INV-05, INV-06, INV-07, INV-10, COMBAT-03, COMBAT-05, COMBAT-06, INV-12, INV-13, RUN-01, RUN-02, RUN-03]

coverage:
  - id: D1
    description: "Shared evidence exists after an ordinary close and is absent after a failed one; the solo world case walks through run.moveUntil"
    requirement: "RUN-01"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (Evidence_RenderedClose_ListsOnlyWrittenFiles, SoloDriver_AllCases_ReferenceDefinedHelpers)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case phase06.headless.world,phase06.headless.lifecycle,phase06.headless.lan-privacy (run 045430ad, PASS 3/3)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Solo pictures wait for settled, unclipped views; the Shift-drag opens the split field while held"
    requirement: "INV-04"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (SettledView_PendingOrClipped_Rejected)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,phase06.headless.inventory (run 72190e6f, PASS 2/2)"
        status: pass
    human_judgment: true
    rationale: "The rendered solo cases are rerun on the real desktop in 06-25; the split and settled captures are not yet proven in a rendered run"
  - id: D3
    description: "Every delivered key is acknowledged; resets follow the D-14 Escape order"
    requirement: "RUN-02"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs src/godot-game/tools/game-tests/catalog.test.cjs (39/39, includes Menu_EscapeEffects_FollowD14Order)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case basic.strict-build,phase06.headless.inventory,phase06.headless.lifecycle,gameplay-checks.core.input,lan-checks.core.input,asset-pipeline.source.tracer,asset-pipeline.source.tracer-failures,asset-pipeline.content.census,asset-pipeline.text.compatibility (run f1eb2b3a, PASS 9/9)"
        status: pass
    human_judgment: false

duration: 41min
completed: 2026-11-25
status: complete
---

# Phase 6 Plan 21: Solo rendered driver, cleanup evidence and Escape receipts Summary

**The solo driver walks with native D, takes images only from settled and unclipped views, Shift-drags in frame-separated steps, and resets through the D-14 Escape order. FoundationRoot now acknowledges every key, including keys the inventory workspace consumes, and the cleanup record is written only after every role's ordinary Quit with exit 0.**

## Performance

- **Duration:** about 41 min
- **Started:** 2026-11-25T17:32:34Z
- **Completed:** 2026-11-25T18:13Z (summary written)
- **Tasks:** 3 of 3
- **Files modified:** 8 (1 created)

## Accomplishments

- `ended()` reads each peer's `terminal.json` and runs `terminal.validate` before it writes `ordinary-cleanup.json` with `{flag: 'wx'}`. A missing or non-zero exit, a missing ordinary Quit or an expected loss leaves the record absent. The headless and rendered closes both write through it.
- The solo world case uses `run.moveUntil(state, peer, key.right, X > start + 500)` in place of `dash`.
- `settledView(draw)` is exported from phase06-rendered.cjs. Every solo image goes through the `shot` wrapper, so its predicate includes `settledView`.
- `run.drag` waits for drawn frames between the press, the 4 px start-threshold crossing and the destination. It runs a held step before release. The split gesture waits for `QuantityToMove` = floor(6 / 2) = `3` while the button is held. The rotation gesture waits for the upright and then the rotated `PlacementPreview`.
- `FoundationRoot._Input` counts and records every `InputEventKey` right after `_gateway.RequireThread()`. The `handled` return is unchanged and still comes before `_input`, the world input and the menu Escape. The guarded method keeps its signature. No `EngineAccessPolicy` member was added.
- `menu()` picks the D-14 step from the current draw for each Escape. The order is: cancel the drag or split preview, close the item menu, leave the nearby target list, close the topmost bag window, close the inventory, then return to the menu. It waits for the acknowledged key and then for that step's effect, and fails with the step name. It returns the steps taken. `tap()` always sends the release.

## Task Commits

1. **Task 1: Shared evidence exists after close, and the solo world case walks with native input** - `6e6e65a` (fix)
2. **Task 2: Solo inventory captures settled grids, and a physical Shift-drag opens the split field** - `289f58d` (fix)
3. **Task 3: Every delivered key is acknowledged, and resets follow the D-14 Escape order** - `8c1f249` (fix)
4. **Derived producer replay for the FoundationRoot change** - `54de9a1` (chore)

## Run-1 diagnosis (re-confirmed from the recorded draws)

Sources: solo `.local/runs/game-tests/ee17949e-…/62f18c32-928d-407a-b1e3-6e9d1680f8e1/` (`solo/draws.ndjson`, `native-input-15896.json`) and LAN `6d406c5e-0500-4c50-9b1e-d800ddc95f38/` (`native-input-8572.json`, guest `draws.ndjson`).

**World (`dash is not defined`).** The world case at the old line 26 called `dash`, which no module declares. With the old source restored, `SoloDriver_AllCases_ReferenceDefinedHelpers` fails with `actual: [ 'dash' ]`. It passes on the repaired file.

**Equipped capture (draw 914).** The inventory opened at draw 911, with key 73 at inputEvents 21. Draws 911–921 show three visible `CarriedStorage/Rows/@Label@57/59/61` rows reading "Opening inventory…". The installed `own.*` views for containers 1, 2 and 58 were already present at 914, so only the pending copy tells the frames apart. Evaluated on the recorded draws, with `clipped` defaulted to false because run 1 predates 06-20, `settledView` rejects 911, 914 and 921 and first accepts draw 922. It also accepts 927, 937 and 940.

**Split (draws 938–940). Proven side: the driver.**
- Product rule (`FoundationInventoryWorkspace.Input.cs`): a left press on an item button sets `_press` with `split: mouse.ShiftPressed` (`ItemInput`). The first `InputEventMouseMotion` at `DistanceTo(press.At) >= 4` calls `_model.Begin(split)`. Begin sets `ChoosingAmount` when `MaxStack > 1 && Quantity > 1`, and `SplitMenu` opens at once, while the button is held. No destination is needed to open the field. `PointerRelease` clears `_press`, so a release handled before the motion prevents any drag.
- Driver gesture (records 40–45): Shift down was acknowledged at draw 938. Then came left press at (420, 387), which is `Item_64`, the berries with quantity 6, centered at (420, 387) and 32×32. Then moves to (428, 387) and (430, 387) by `SetCursorPos` and the release at (430, 387). Shift up was acknowledged at draw 940. The whole gesture fell between two draws. The release point was still on the source's own occupied cell (0, 0), not on a legal empty cell.
- Frame record: no draw from 938 to the end of the run contains `QuantityToMove`, `PlacementPreview` or an `ItemMenu` after 936. Not even a non-split drag preview appeared, so `Begin` never ran. At draw 1000 the hover tooltip `Item_64/@PopupPanel@154` shows the pointer still over the stack. The press therefore hit the stack, but no motion was handled between the press and the release. The 10 px distance passes the threshold rule, so the remaining cause is ordering. The cursor moves come from `SetCursorPos`, which Windows turns into synthesized `WM_MOUSEMOVE` messages, while the button events come from `SendInput`. In a burst that spans one frame, the game handled the release before any motion. This ordering is inferred, since the record does not timestamp individual Windows messages. That the driver is the wrong side is proven: the product never got a press-then-motion sequence.
- Repair: the driver Shift-presses the stack and waits for a draw at least two frames later. It moves 8 px (twice the 4 px threshold) toward the destination and waits for a draw. It then moves onto the empty pouch cell (1, 0), waits for `QuantityToMove` = `3` while the button is held, and releases. The product is unchanged, so Half = floor(quantity / 2) and the source-preserving cancellation (AC-16) are untouched.

**Escape resets (solo).** The first reset Escape (record 47) went to the open workspace. No drag was active, because none had begun, so `Escape()` closed the topmost bag window: `CloseBag_63` disappeared at draw 1656 while inputEvents stayed 24. `_Input` returned on `handled` before `_inputEvents++`/`RecordInput`, so `nativeKey` timed out with `RENDERED_DEADLINE native event solo/27/true`. The aborted tap never sent its release. The close's next press (record 49) therefore reached the game as an echo at draw 2376 (`{key: 4194305, pressed: true, echo: true}`, inputEvents 25). The workspace ignores echoes, so it was acknowledged but did nothing. The release followed at 2378. The next press (record 51) was consumed again, closing `CloseBag_58` at draw 2380 without a receipt. That deadline produced `TERMINAL_NORMAL_QUIT solo`. The plan text said the first Escape cancelled an active preview. In run 1 it closed a bag window instead, by the same consumed-key mechanism.

**Escape resets (LAN guests).** For guest-3, the workspace was open at draw 1000. Press record 90 closed it at draw 1094 with inputEvents unchanged at 72, and no receipt came. The repeated press (record 92) arrived as an echo at draw 1782, the release at 1784, and the next press reached the menu at 1786. For guest-2, press record 98 closed the workspace at draw 1906 with inputEvents still 72. It was never acknowledged, which produced `RENDERED_DEADLINE native event guest-2/27/true` in the cleanup. This is the same pattern.

**Cleanup ENOENT (F12).** The rendered close has called `native.ended(state)` after a successful close since `97cacc6`. In run 1 the LAN close failed on the guest Escape receipts above, so the record was never written and `phase06.lan.population` failed its post-close evidence recheck with ENOENT. Its in-process work had passed and its captures were taken. The plan's statement that only headless groups write the record does not match the source. The missing record was the truthful result of the failed close, and Task 3 removes that cause. Task 1 makes the writer check the terminal records itself.

## Files Created/Modified

- `phase06-drivers.test.cjs` (new): four named tests and 19 subtests. They use real `lan-checks/rendered.cjs` waits over scratch `draws.ndjson` files, a fake input driver and a fake `network.cleanup` that writes real-shaped terminal records.
- `phase06-headless.cjs`: `ended()` validates terminal records first.
- `phase06-solo.cjs`: `moveUntil` walk, `shot` settled wrapper, split and rotate gestures through `run.drag`.
- `phase06-rendered.cjs`: `settledView`, frame-stepped `drag`, release-safe `tap`, D-14 `menu`.
- `FoundationRoot.cs`: key receipt before routing (23601 bytes, SHA-256 `bab43117ad732b8ef7d685330b9a0e90115a2465d35a0262904f14550e935f21`).
- `content.cjs`: only the FoundationRoot `SUPPORT_SOURCE_IDENTITY` row. `git diff 09e2881..HEAD` shows 1 line changed, and `supportStampOnly(HEAD^, current)` returns true.
- `engine-generated-spans.json`, `required-cases.json`: re-derived replay. The registry hash went from `e0e3161b…8348` to `ebd9052b69db85faae4bdcdf879d0bd1f1e5b68431462971310f07d1945dfb94`.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] The plan's F12 premise did not match the source**
- **Found during:** Task 1
- **Issue:** The rendered close already wrote `ordinary-cleanup.json` after success. The ENOENT came from the failed close.
- **Fix:** No duplicate writer was added. `ended()` now validates each role's terminal record (ordinary Quit, exit 0, matching pid and role, no expected loss), so a failed close cannot write the record whatever the calling order.
- **Files modified:** phase06-headless.cjs
- **Commit:** 6e6e65a

**2. [Rule 1 - Bug] The rotation gesture had the same burst race as the split**
- **Found during:** Task 2
- **Issue:** The ammunition rotation pressed and moved in one burst before tapping R, the pattern that defeated the split.
- **Fix:** It now uses the frame-stepped `run.drag` and waits for the upright, then the rotated, `PlacementPreview` around the R tap.
- **Files modified:** phase06-solo.cjs
- **Commit:** 289f58d

**3. [Rule 1 - Bug] An unacknowledged press left the key held**
- **Found during:** Task 3 diagnosis (the echo at draw 2376)
- **Fix:** `tap()` always delivers the release and reports the first failure.
- **Files modified:** phase06-rendered.cjs
- **Commit:** 8c1f249

**4. [Rule 3 - Blocking] Stale current producer replay after the FoundationRoot change**
- **Fix:** Ran `current-replay.cjs --write` (57 historical, 22 changed, 22 added; sourceInputsSha256 `d6bffe30…f489`) and rebound the two registry hashes.
- **Verification:** `verification.engine` PASS 22/22 (verification run 8a3e9c2a) within game-tests run 48226d1f.
- **Commit:** 54de9a1

**Total deviations:** 4 auto-fixed (3 bugs, 1 blocking). None changes product behavior beyond the planned key receipt.

## Verification runs

| Purpose | Command | Run | Result |
| --- | --- | --- | --- |
| Task 1 RED (old solo source) | `node --test …/phase06-drivers.test.cjs` with HEAD `phase06-solo.cjs` restored temporarily | local | FAIL `[ 'dash' ]` |
| Task 1 verify | `node --test …/phase06-drivers.test.cjs` | local | 10/10 pass |
| Task 1 verify | `check.cjs --case phase06.headless.world,phase06.headless.lifecycle,phase06.headless.lan-privacy` | game-tests 045430ad-23ad-4e3a-a4dd-cc37739d3ab3 | PASS 3/3; `ordinary-cleanup.json` present in group 95e14f81 |
| Task 2 verify | `node --test …/phase06-drivers.test.cjs` | local | 17/17 pass |
| Task 2 verify | `check.cjs --case basic.strict-build,phase06.headless.inventory` | game-tests 72190e6f-fc44-4f35-9f69-da87b2d24e0f | PASS 2/2 |
| Task 3 verify | `check.cjs --case basic.strict-build,phase06.headless.inventory,phase06.headless.lifecycle,gameplay-checks.core.input,lan-checks.core.input,asset-pipeline.source.tracer,asset-pipeline.source.tracer-failures,asset-pipeline.content.census,asset-pipeline.text.compatibility` | game-tests f1eb2b3a-ecb9-4d7d-ac1f-75fe7044e0df (gameplay-checks 7d260cdd, lan-checks 0b517f5e, asset-pipeline ba4e47f1, 91a9050c, 195fd155, f006dfc2) | PASS 9/9; no `SUPPORT_SOURCE_IDENTITY` in the four asset-pipeline logs |
| Task 3 verify | `node --test …/phase06-drivers.test.cjs …/game-tests/catalog.test.cjs` | local | 39/39 pass |
| Replay and regression | `check.cjs --case verification.engine,basic.repository,basic.headless,lan-checks.core.permissions` | game-tests 48226d1f-7cd1-41a3-8f6d-68822978adf0 (verification 8a3e9c2a) | PASS 4/4 (run on the tree of 54de9a1 before it was committed) |

Every file in `src/godot-game/tests/scenarios` loads with `require` (six drivers and the new test).

No rendered run was made. Per the plan, the six solo rendered cases are rerun on the real desktop in 06-25. `controls[].clipped` has still not run in a rendered draw. If real clipping exists, `settledView` will make the affected solo pictures time out with their labels there.

## Known Stubs

None.

## Deferred Issues

- `gameplay-checks.core.permissions` (CharacterPresentation.cs) is still deferred from 06-20 and is untouched here.
- The rendered proof of the split field, the settled captures and the D-14 resets belongs to 06-25. The LAN driver's own waits (F12 `RENDERED_DEADLINE guest-1 one authority withdrawal`) belong to 06-22, which can reuse `settledView` and the D-14 `menu`.

## Next Phase Readiness

- 06-22 can import `run.settledView` and rely on `run.menu` and `run.drag` (with its `held` step).
- 06-25 must rerun the solo group. Expected D-14 reset steps for the inventory case: close bag window (pouch), close bag window (backpack), close inventory window, return to ordinary menu.

## Self-Check: PASSED

- Commits on the branch: 6e6e65a, 289f58d, 8c1f249, 54de9a1 (`git rev-list --count 6c47219..HEAD` = 4).
- Files present: phase06-drivers.test.cjs, phase06-rendered.cjs (`settledView` exported), FoundationRoot.cs, content.cjs stamp row.

---
*Phase: 06-survivors-inventory-and-combat*
*Completed: 2026-11-25*
