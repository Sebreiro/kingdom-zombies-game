---
phase: 06-survivors-inventory-and-combat
plan: "22"
subsystem: testing
tags: [godot, csharp, lan-driver, receipts, f12, f13, succession, roster, gap-closure]

requires:
  - phase: 06-21
    provides: "settledView, the frame-stepped native drag, release-safe tap and the D-14 Escape reset"
provides:
  - "Role-named profiles (Host, Guest 1-3, Survivor) passed to network.launch as the retained profile root"
  - "A population case that walks the three guests apart with native D and requires roster text, spread bodies, crowns and settledView"
  - "runAction observations carrying the receipt kind and replay flag, and RunActionFeedback carrying the kind"
  - "receipt(): exact matching on kind, not replayed and a later sequence, read from every frame since submission"
  - "A rendered conflicts case with exact quick receipts, a frame-stepped Ctrl click and per-role committed money"
  - "Succession predicates that judge holds over distinct committed draws, the Respawn gate, the slot order and the handover rule"
affects: [06-23, 06-24, 06-25, 06-15]

actuals:
  tokens: 57500    # chars/4 over the realized diff (230058 chars); 13100 of it is hand-written source, the rest the derived spans registry rows
  tasks: 3
  commits: 5
plan_head_before: 1e1f8f94c5eaad55d674d7c9305e5eed957db679
plan_head_after: 1e897baf67bcc5d574169a8aff9fe3b69d5e766d

tech-stack:
  added: []
  patterns:
    - "A driver accepts only the receipt of the request it submitted (same kind, not replayed, later sequence) and reads every frame since submission"
    - "Receipts seen while waiting are recorded in the case evidence as interleaved, before or after the accepted one"
    - "A camera hold is judged over at least two distinct committed draws while a teammate actually travels"
    - "Recorder: a span longer than the shared 256-row reader window is kept by polling it into a map"

key-files:
  created:
    - .planning/phases/06-survivors-inventory-and-combat/06-22-SUMMARY.md
  modified:
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/scenarios/phase06-lan.cjs
    - src/godot-game/tests/scenarios/phase06-drivers.test.cjs
    - src/godot-game/tests/scenarios/phase06-rendered.cjs
    - src/godot-game/game/foundation/FoundationDiagnostics.cs
    - src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs
    - src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs
    - src/godot-game/game/foundation/FoundationCoordinator.cs
    - src/godot-game/tests/verification/Phase06PublicationTests.cs
    - docs/code/engine-generated-spans.json
    - src/godot-game/tools/verification/required-cases.json

key-decisions:
  - "F13 is mechanism (a), a driver correlation defect: the watcher's place receipt (Applied) is replaced 47-100 ms later by the workspace's own window request receipt (Ready), and the old action() took the first later sequence from the latest frame. The product emits exactly one non-replayed result per request, so GuestPublication and RunWire publication are unchanged"
  - "RunActionFeedback carries the receipt kind; FoundationRunReadback keeps its three-field Feedback through a private RunReceipt projection, so no consumer changes"
  - "The roster check follows the product text 'P{slot} (You) · {label}': the marker follows the player number, as FoundationHud.Project draws it"
  - "Body width is one u (16 px at 1000 subunits), the spec 3.1 definition, because draws record actor positions but no actor rectangles"
  - "Run 1's conflicts deadline was not a receipt race: guest-1's Ctrl click submitted no quick request at all, so the withdrawal is now frame-stepped like the 06-21 drag"

patterns-established:
  - "Any game C# change re-derives current-replay.cjs --write and rebinds the two spans registry hashes in a separate chore commit"
  - "No tracked file may change state while a check.cjs run is in progress; the runner fails every pass with 'Source changed during execution.'"

requirements-completed: [CHAR-01, INV-08, INV-10, INV-11, RUN-01, RUN-02, RUN-03, RUN-04]

coverage:
  - id: D1
    description: "Four distinct profile names reach every snapshot; the population case requires roster text, four separated crowned bodies and settled views"
    requirement: "CHAR-01"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (Roster_DistinctNames_MatchSnapshotSlots)"
        status: pass
      - kind: integration
        ref: "node src/godot-game/tools/game-tests/check.cjs --case phase06.headless.lan-privacy,phase06.headless.start (run 2ee83538, PASS 2/2)"
        status: pass
    human_judgment: true
    rationale: "The rendered population case reruns on the real desktop in 06-25"
  - id: D2
    description: "Each accepted receipt belongs to the submitted request; F13 is closed at its proven cause"
    requirement: "INV-11"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (Receipt_ExactKind_IgnoresInterleaved)"
        status: pass
      - kind: integration
        ref: "check.cjs --case phase06.headless.lan-privacy,phase06.headless.inventory,lan-checks.core.protocol,lan-checks.core.ordering (run c7dbd58c, PASS 4/4); 20 consecutive lan-privacy invocations on 1e897ba, 20/20 pass"
        status: pass
    human_judgment: false
  - id: D3
    description: "Succession holds, follow order, Respawn gate and handover rule are judged over distinct committed draws and frames"
    requirement: "RUN-03"
    verification:
      - kind: unit
        ref: "node --test src/godot-game/tests/scenarios/phase06-drivers.test.cjs (Succession_CameraHold_RequiresDistinctDraws, Succession_RespawnGateAndHandover_FollowTheirRules)"
        status: pass
      - kind: integration
        ref: "check.cjs --case phase06.headless.lifecycle (run bab6f15b, PASS 1/1)"
        status: pass
    human_judgment: true
    rationale: "phase06.lan.succession has never run; it reruns on four real peers in 06-25"

duration: 64min
completed: 2026-11-25
status: complete
---

# Phase 6 Plan 22: Four-role LAN driver and receipt race Summary

**F13 is proven to be a driver correlation defect: the watcher's place receipt stays latest for only 47-100 ms before the workspace's own window request replaces it. The drivers now accept only the receipt of the request they submitted, read from every frame since submission, and 20 consecutive fresh invocations pass. The LAN roles carry distinct names, the guests walk apart before the population pictures, and the succession case judges every hold over distinct committed draws.**

## Performance

- **Duration:** about 64 min
- **Started:** 2026-11-25T18:12Z
- **Completed:** 2026-11-25T19:16Z
- **Tasks:** 3 of 3
- **Files modified:** 11 (one created: this summary)

## Accomplishments

**Names and population (Task 1).**
- `launch` in phase06-headless.cjs writes `<run>/profiles/<role>/profiles.json`: schema 1, one profile with a fresh 32-hex id, ordinal 1, and the name `Host`, `Guest 1`, `Guest 2`, `Guest 3` or `Survivor`. It passes that root to `network.launch` as the retained profile root. `lan-checks/network.cjs` is unchanged (`git diff 09e2881..HEAD` is empty).
- `privacy()` now requires that each player's participant in every snapshot carries that player's own name.
- The population case focuses each guest and walks it right with `run.moveUntil` (native D) by at least 1500, 3000 and 4500 subunits. The host stays. Every receiver's picture must show:
  - 8 visible actor bodies, 4 crowns and the local body in camera;
  - the four controlled bodies at pairwise drawn distances of at least one body width;
  - roster rows `Roster/Player1`-`Player4` equal to `P{slot}[ (You)] · {label}`, with exactly one `(You)` on the receiver's own binding;
  - each player's own profile name;
  - `run.settledView(draw)`.
- On the run-1 population draws, `rosterMatches` rejects all four rosters (four equal "Player 1" labels) and accepts them once the labels are renamed. `spread` rejects them, because the bodies stood 9.6 px apart.

**Receipts (Task 2).**
- `RunActionFeedback` is now `(sequence, kind, status, replayed)`. It is set from `kind` (local), `game.Action` (guest result envelopes), `"respawn"` (the local respawn poll) and `action` (fixture actions), with its replacement rule unchanged. `runAction` in frames.ndjson exposes `kind` and `replayed`. `FoundationRunReadback.Feedback` stays three-field through a private `RunReceipt` projection. No `EngineAccessPolicy` member was added.
- `receipt(state, peer, kind, before, label)` reads every frame written after `before`. It accepts the first receipt whose kind matches, whose `replayed` is false and whose sequence is greater than the one observed before submission. Every other receipt seen while waiting is recorded in the case evidence under `interleaved`, as `before` or `after` the accepted one, and is never accepted. `action()` has no resubmission loop. The native food, cooldown and empty-ammo receipts use the same matching.
- The rendered conflicts case:
  - waits for the exact `quick` receipts;
  - requires the first withdrawal to be `Applied`;
  - requires the second to be `AccessLost` or `StaleItem` when its cell was still visible, or a visible correction with no request when it was not;
  - checks every role's carried money against its own original total plus its own applied withdrawal.
  The Ctrl click is frame-stepped: Ctrl is acknowledged, the pointer rests on the cell for a drawn frame, and the click is handled while Ctrl is still held for another drawn frame. The final pictures no longer require an external view on roles that have none.

**Succession (Task 3).** Each predicate was checked against `spectator.Camera`, `Followed`, `DeathEpoch`, `TransferNotice`, `Smoothing`, `survivor` and `generation`, and against scenario row 3.
- **Death hold (D-33).** `cameraHold` needs at least two distinct committed draws after death with one camera pose (`spectator.Camera` equal and `cameraX` within 0.02), none of them followed, while the teammate travels at least 1150 subunits between the first and the last.
- **Follow loss (D-34).** The loss picture keeps the last followed pose with the selection cleared and the notice shown. The later hold is judged by `cameraHold` while a different teammate travels.
- **Teammate order.** `eligible()` takes the living ready teammates from the status view in slot order. Next must follow the first and Previous the last. The `Teammate_{slot}` buttons must list exactly those slots.
- **Respawn gate (AC-02).** `respawnGate` reads every frame from the death picture to the enabled button. The button must be disabled while `RootTime < OpensAt` and enabled after it, and `OpensAt − corpse.At = 1000`. The death picture requires the button drawn disabled, and the chosen picture requires it enabled.
- **Transfer.** The successor must be an actor with no binding in the host's view after the rejoin. The player and binding must be unchanged, and `ControlTransferred` must read `Control transferred · {survivor}`. `handover` expects a smooth handover within 320 px (a quarter of 1280) of the held center, and a cut otherwise.

## Task Commits

1. **Task 1: Four distinctly named, spread and crowned survivors** - `971db72` (feat)
2. **Task 2: Prove and remove the receipt race behind F13 and the conflicts deadline** - `a59e976` (fix)
3. **Task 3: The succession case observes a real camera hold and an exclusive transfer** - `de7a186` (fix)
4. **Record the receipts that replace an accepted receipt** - `77e27a3` (fix)
5. **Derived producer replay for the receipt-kind change** - `1e897ba` (chore)

## F13 diagnosis

**Reproduction.** With the `kind`/`replayed` observation in place and the old `action()`, the bounded loop (up to 30) reproduced the failure on invocation 1: game-tests `069f23d0-cdb4-4150-9a71-5a45f0622a15`, `place 'Ready' !== 'Applied'`. Reproduction count: 1 of 1.

**Failing role.** The failing role was guest-3, the watcher, at its `place` in `checkLooseItems`, not guest 1 or 2. Guest-1's frames show only its sequence-1 move receipt, because the select→place loop never started.

**Receipt stream (guest-3 frames, group 79ae264f):**

| Frame | Sequence | Kind | Status | Replayed | Latest for |
| --- | --- | --- | --- | --- | --- |
| 701 | 15 | external-items-page | Ready | false | 53 frames |
| 754 | 16 | place | Applied | false | 3 frames, 47 ms |
| 757 | 17 | workspace | Ready | false | 7 frames |

**Mechanism (a), proven.**
- Moving the off-page bag to carried equipment changes the desired windows. `FoundationInventoryWorkspace.RefreshWindows` then submits `RequestRunWorkspace`, which is `SubmitRunAction("workspace")` with the next sequence, and its `Ready` receipt replaces the place receipt.
- The old `action()` read only the last frame every 25 ms and took the first sequence above `previous`. A poll that missed the window took sequence 17 as the place receipt.
- The window measured on nine runs, from the place receipt to the next receipt: 47, 49, 51, 48, 48, 58, 50, 50 and 100 ms (`069f23d0`, `a8fc27da`, `1be360e3`, `bb9182f5`, `e97593f1`, `66910e9b`, `f10ace15`, `8c60d0dc`, `aa7239c1`).
- Run 1 (`b27d4a6a`, guest-3) shows the same pattern: sequence 16 Applied at frame 597, then sequence 17 Ready at frame 599, 45 ms later.

**Not (b).**
- Every sequence appears once with `replayed: false`, and the replacing receipt has a different kind.
- The host's `CompleteRunAction` sends one result per request. A replay keeps `Replayed = true` (`Phase06PublicationTests.CheckStashCorrection`, now `(sequence, "place", "Applied", true)`).
- `lan-checks.core.protocol` and `.ordering` pass.

**After the repair.** In 4 of the 20 stability invocations, the replacing workspace receipt had already arrived at the poll that accepted the place receipt, and it is recorded as `{awaiting: "place", relation: "after", sequence: 17, kind: "workspace", status: "Ready"}`. The old driver would have failed those runs with F13. The four invocations are `ac60dc59`, `888a3e4f`, `db9424e4` and `f651aa37`.

## Run-1 conflicts finding (F12)

Rendered run 1 (`6d406c5e`, guest-1 frames) contains no `quick` receipt at all. The latest receipt stayed sequence 3, the stash `Ready` at frame 470, for 38 s, until the Escape reset. The Ctrl press was acknowledged at draw 511 and the release at draw 513. The click (native record 88, at (840, 260)) landed on `Item_25`, centered at (840, 260) in every draw from 509 to 516.

So the deadline was not a receipt race: the click submitted no request. Frames do not record mouse events, so the exact cause of the dropped submission is inferred, not proven. The click was delivered between the Ctrl press and release within one burst, the same ordering class that defeated the 06-21 split drag. The withdrawal is now frame-stepped. It must be proven in the rendered rerun in 06-25.

## Verification runs

| Purpose | Command | Run | Result |
| --- | --- | --- | --- |
| Task 1 unit | `node --test …/phase06-drivers.test.cjs` | local | 31/31 pass, includes `Roster_DistinctNames_MatchSnapshotSlots` |
| Task 1 verify | `check.cjs --case phase06.headless.lan-privacy,phase06.headless.start` | game-tests 2ee83538-2001-4dac-a868-b9c2dc553d0e | PASS 2/2; the participants' labels are `Host`, `Guest 1`, `Guest 2`, `Guest 3` in all four agreement rows |
| Diagnosis, kind observed | `check.cjs --case phase06.headless.lan-privacy` | a8fc27da-2c6c-430d-9824-bbb11905f7cc | PASS; stream shows place Applied for 49 ms, then workspace Ready |
| Reproduction loop | the same case, at most 30 times, stopping at the first failure | 069f23d0-cdb4-4150-9a71-5a45f0622a15 | FAIL `place 'Ready' !== 'Applied'` (1 of 1) |
| Task 2 verify | `check.cjs --case phase06.headless.lan-privacy,phase06.headless.inventory,lan-checks.core.protocol,lan-checks.core.ordering` | c7dbd58c-7b2b-4bb6-9eee-df31139b5c52 (lan-checks a5e32cf2) | PASS 4/4, no `!==` |
| Task 3 verify | `node --test …/phase06-drivers.test.cjs`; `check.cjs --case phase06.headless.lifecycle` | local; bab6f15b-b774-46c1-9f20-353eac1e531a | 47/47 pass; PASS 1/1 |
| Replay and regression | `check.cjs --case verification.engine,basic.repository,basic.headless,lan-checks.core.permissions` | 580f64cc-2009-47af-bd06-ec934199ce2e (verification 2a600fa6) | PASS 4/4; `verification.engine` 22/22, run on the tree of 1e897ba before it was committed |
| Stability (acceptance) | 20 consecutive `check.cjs --case phase06.headless.lan-privacy` on clean 1e897ba | see below | 20/20 pass |
| Headless regression | `check.cjs --case phase06.headless.world,phase06.headless.start,phase06.headless.inventory,phase06.headless.combat,phase06.headless.lifecycle` | 6eaf2ca1-6535-4376-ae0d-911c2aa9306f | PASS 5/5 |
| Unit and catalog | `node --test …/phase06-drivers.test.cjs …/game-tests/catalog.test.cjs` | local | 63/63 pass |

**The 20 consecutive stability invocations** ran on the clean tree of `1e897ba`, each a fresh process set:
- `ac60dc59-029d-4031-8ec5-73a6a697acc7`
- `e1ec36a4-3137-447d-a5b7-858523238e13`
- `888a3e4f-f6bd-48ac-a8d7-d43c3c43aa89`
- `0958aee3-30ee-45d2-8294-dd137e7fa6df`
- `34a1b0df-b8ba-46c5-b7b8-8e325c56c6a7`
- `e66935dd-18d3-4a30-8967-81aa0f28d91c`
- `db9424e4-c904-4185-9cd7-9ef5445a8f3f`
- `fa2e76bb-9e85-4183-b836-b5acf0f431d2`
- `0b6a1f90-9b86-4c2c-81c0-f890d0a95e52`
- `f651aa37-666c-4d52-9b64-e59184ebaca8`
- `5160d278-c323-4000-8021-c99365ecf22b`
- `30b81b38-6c55-4a07-b8e6-990481e9daa9`
- `8b10fbe4-cd5e-4f46-9868-0986e0c4ac6d`
- `f45f9238-0fa2-4862-8cfe-facb76c3ac2a`
- `5b4f08c0-03c1-40aa-b787-159264062319`
- `c0f11e88-5ddd-4c0b-9e63-902bef2442e4`
- `0870582b-c880-43df-86f8-e593f2a62a04`
- `5481cdee-b3f6-43e9-a649-0a348d062e00`
- `9919c361-7f64-4b2b-8c70-8afb4a73a329`
- `fc73c50c-f45b-4afb-8278-71c52f96d570`

**An earlier, discarded stability attempt** ran on `a59e976` and does not count. It held:
- 10 passes: `1be360e3`, `bb9182f5`, `e97593f1`, `66910e9b`, `f10ace15`, `8c60d0dc`, `aa7239c1`, `3b483cfa`, `bedf9409`, `72336c02`;
- two failures with `Source changed during execution.` (`a3acc15c`, `4806f0a0`), caused by my own edits to tracked files while it ran;
- one invocation (`406755da`) that I stopped during its build.

No rendered run was made. Per the plan, the three LAN rendered cases rerun on the real desktop in 06-25.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Premise] The failing F13 `place` was the watcher's, not the guests 1-2 loop**
- **Found during:** Task 2 diagnosis
- **Issue:** The plan pointed at the guests' select→place loop. The reproduced failure and run 1 are both guest-3's `place` in `checkLooseItems`, and guests 1-2 had not started.
- **Fix:** The exact matching applies to every `action()`, so both paths are covered.
- **Commit:** a59e976

**2. [Rule 3 - Blocking] The receipt kind needed two more files**
- **Issue:** The tuple change also reaches `FoundationCoordinator.cs` (the fixture action setter) and `Phase06PublicationTests.cs`, whose full-tuple assertion gains `"place"`.
- **Fix:** Both were updated. `FoundationRunReadback` is unchanged through `RunReceipt`.
- **Commit:** a59e976

**3. [Rule 1 - Bug] The run-1 conflicts deadline was a missing submission, not a receipt race**
- **Fix:** The Ctrl click is frame-stepped (see the F12 finding). The final conflicts pictures no longer require an external view on the host and guest-3, which have none. Before, `undefined?.Rows.every()` made that predicate always false.
- **Commit:** a59e976

**4. [Rule 2 - Missing] Exact matching for native food and attack receipts**
- **Issue:** The inventory case observed `sequence > assigned` after closing the workspace, which itself submits a receipt. The combat case had the same weakness.
- **Fix:** Both use `receipt()` with `food` and `attack`.
- **Commit:** a59e976

**5. [Rule 2 - Evidence] Replacing receipts are recorded as well as preceding ones**
- **Fix:** `receipt()` records every other receipt seen while waiting, with its relation to the accepted one, so the F13 replacement is visible in the case evidence.
- **Commit:** 77e27a3

**6. [Rule 3 - Blocking] Stale current producer replay after the C# change**
- **Fix:** Ran `current-replay.cjs --write` (57 historical, 22 changed, 22 added; sourceInputsSha256 `482fcebf…f8e8`) and rebound the two registry hashes, from `ebd9052b…fb94` to `adbb0c0b…734e`.
- **Verification:** `verification.engine` PASS 22/22 (verification run 2a600fa6).
- **Commit:** 1e897ba

**7. [Clarification] Two measures use the product's actual form**
- **Roster text:** the marker follows the player number (`P2 (You) · Guest 1`), as `FoundationHud.Project` draws it. The plan's `P{slot} · {name}` plus `(You)` reads either way.
- **Body width:** one u. Draws carry actor positions but no actor rectangles. Spec 3.1 defines u as about a standing survivor's body width, and the world draws it as 16 px. `FoundationDrawObserver` is outside the plan's files.

**Total deviations:** 7: 3 bugs or premises, 2 missing pieces, 1 blocking and 1 clarification. Product behavior changes only by the extra `kind` field in the read-only observation.

## Known Stubs

None.

## Threat Flags

None. The new observation fields carry only the local receipt kind and replay flag (T-06-40). `run.privacy`, the disjoint own rows and the remote-scope revocation are unchanged, and the privacy check now also requires the distinct names.

## Deferred Issues

- `gameplay-checks.core.permissions` (CharacterPresentation.cs) is still deferred from 06-20 and is untouched here.
- The catalog's `phase06.lan.population` action text still says "move one guest with D", while all three guests now walk apart. The catalog was not edited, to avoid moving catalog-bound hashes outside this plan's files. The owner is 06-24 (final bindings).
- The rendered proof of the population, conflicts and succession predicates belongs to 06-25. Two parts are inferred, not proven: the Ctrl-click ordering and the assumption that a near successor's handover is smooth (the territory condition is not observable). If known territory does not cover the interpolation, the product cuts and `handover` reports `expected smooth, observed cut`.

## Next Phase Readiness

- 06-25 can rerun `phase06.lan.population`, `.conflicts` and `.succession` on the real desktop. Expected population roster: `P1 · Host`, `P2 · Guest 1`, `P3 · Guest 2`, `P4 · Guest 3`, with `(You)` after each receiver's own number.
- Any later driver can import `receipt` and `receiptsAfter` from the headless helpers.

## Self-Check: PASSED

- Commits on the branch: 971db72, a59e976, de7a186, 77e27a3, 1e897ba (`git rev-list --count 1e1f8f9..HEAD` = 5).
- Files present: phase06-headless.cjs (`receipt`, `receiptsAfter`, `profileNames` exported), phase06-lan.cjs (`rosterMatches`, `spread`, `cameraHold`, `respawnGate`, `handover` exported), phase06-drivers.test.cjs (the four new tests), FoundationDiagnostics.cs (`runAction.kind`, `runAction.replayed`).

---
*Phase: 06-survivors-inventory-and-combat*
*Completed: 2026-11-25*
