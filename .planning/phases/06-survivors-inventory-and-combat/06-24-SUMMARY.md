---
phase: 06-survivors-inventory-and-combat
plan: "24"
subsystem: testing
tags: [godot, csharp, ac-08, bindings, no-window-gate, code-review, gap-closure]
status: complete

requires:
  - phase: 06-23
    provides: "Inspection driver F10 repair; the last source repair before the final bindings"
provides:
  - "ac-08.complete repository stages, the six caller stages and the ExemptionTests theory rows derived from the repaired HEAD (ac08-bindings.cjs --check passes)"
  - "The asset binding cascade, identity closure, packaged field reference and current producer replay rederived to the repaired source"
  - "A green no-window gate on the repaired source (71/71) and on every later fix delta (32/32 reruns)"
  - "A clean independent deep review of all non-planning changes since a9ac95f, recorded in 06-REVIEW.md, reviewed source c2d2456"
  - ".local/temp/phase06-review.json bound to the final HEAD for the 06-25 rendered runner"
affects: [06-25, 06-15]

actuals:
  tokens: 288700  # chars/4 over the realized diff (1,025,036 non-planning + 129,629 planning chars; mostly hash and base64 rebinds)
  tasks: 2
  commits: 43     # git rev-list --count 1e1fe59..e4b556a; the completion docs commit is one more
plan_head_before: 1e1fe591c1c2b0359d6e714f2edd5c865e2e306c
plan_head_after: e4b556a4ee79de0476cf7e4aef15446af081762b

tech-stack:
  added: []
  patterns:
    - "Every source fix is followed by its derivation chain in order: current-replay --write, then ac08-bindings --write (and asset-bindings --write for content closures), each in its own chore commit"
    - "A comment-only fix still changes a bound source hash and therefore needs the full rederivation chain"
    - "Review rounds are recorded as appended sections of 06-REVIEW.md; the front matter describes the latest round and earlier history stays unchanged"

key-files:
  created:
    - .planning/phases/06-survivors-inventory-and-combat/06-24-SUMMARY.md
  modified:
    - src/godot-game/tools/verification/required-cases.json
    - src/godot-game/tools/gameplay-checks/required-cases.json
    - src/godot-game/tools/verification/ac08-bindings.cjs
    - src/godot-game/tools/verification/bindings.test.cjs
    - src/godot-game/tests/verification/ExemptionTests.cs
    - src/godot-game/tests/verification/FoundationPermissionTests.cs
    - src/godot-game/tests/verification/FoundationLanPermissionTests.cs
    - src/godot-game/tests/verification/EngineAccessPolicy.cs
    - src/godot-game/tools/asset-pipeline/required-cases.json
    - src/godot-game/tools/asset-pipeline/content.cjs
    - src/godot-game/tools/game-tests/suites.cjs
    - src/godot-game/tools/README.md
    - src/godot-game/tools/lan-checks/network.cjs
    - src/godot-game/game/foundation/FoundationAutomation.cs
    - src/godot-game/game/foundation/FoundationRoot.cs
    - src/godot-game/game/foundation/FoundationHud.Live.cs
    - src/godot-game/game/foundation/FoundationInventoryWorkspace.cs
    - src/godot-game/tests/scenarios/phase06-headless.cjs
    - src/godot-game/tests/scenarios/phase06-lan.cjs
    - src/godot-game/tests/scenarios/phase06-solo.cjs
    - src/godot-game/tests/scenarios/phase06-rendered.cjs
    - src/godot-game/tests/scenarios/phase06-drivers.test.cjs
    - docs/code/engine-generated-spans.json
    - docs/content/fields.md
    - src/godot-game/game/content/pack-index.json
    - src/godot-game/game/content/packs/*.json (compatibility, delivery, player-identities, text-data, ui-camp, ui-identity, ui-inventory, ui-legacy-stash, ui-map-warnings)
    - src/godot-game/game/content/fixtures/crowds.json
    - .planning/phases/06-survivors-inventory-and-combat/06-REVIEW.md
    - .planning/phases/06-survivors-inventory-and-combat/deferred-items.md

key-decisions:
  - "ac08-bindings.cjs --write must also rebuild each caller's repository stages, overallMs and the ExemptionTests theory rows, because Phase 6 grows the ac-08 source stages from 15 to 19; rehashing the callers alone left CALLER_RESPONSIBILITY_UNION and PRODUCER_DISCOVERY_IDENTITY failures"
  - "The game-tests asset-pipeline group limit is raised from 90 to 150 minutes: a passing content.native-permissions case takes about 65 minutes, so the full 28-case group needs about 92"
  - "The gap-repair review reached clean after five re-reviews by fresh gsd-code-reviewer agents; the final reviewed source is c2d24563389c22628deb301dda4f4d2a8e791997, and only .planning/ records follow it"
  - "phase06-review.json is written after the final planning commit, because both the plan verify and check.cjs --review require revision === HEAD"

patterns-established:
  - "A receipt match binds to the exact sequence the request allocated (GR-04), and only a phase06-action ack reports that sequence (NR-05)"
  - "Frame and draw evidence is read as a gap-free span from a reference frame, or fails with FRAME_SPAN_GAP (GR-03, NR-01)"

requirements-completed: []  # REQUIREMENTS.md is untouched; requirement acceptance waits for the 06-25 rendered evidence

coverage:
  - id: D1
    description: "ac-08 repository stages, caller stages and theory rows are derived from the final repaired source"
    requirement: "RUN-04"
    verification:
      - kind: integration
        ref: "node src/godot-game/tools/verification/ac08-bindings.cjs --check (PASS c2d24563389c rows=612 checked=301 sourceStages=19 callers=6)"
        status: pass
      - kind: integration
        ref: "verification check.cjs --cases ac-08.complete,R-50.positive,rules.godot-callsite-boundary,rules.godot-companion-positive,rules.godot-companion-negative,rules.exemptions (run 3129463a, 6/6 on c2d2456)"
        status: pass
    human_judgment: false
  - id: D2
    description: "Every no-window failed or affected case passes on the repaired source"
    requirement: "RUN-04"
    verification:
      - kind: integration
        ref: "game-tests gate d9f598ec, 71/71 on 0e8b859; reruns dc308851, 7c70e62a and 3406d111, 32/32 each"
        status: pass
    human_judgment: false
  - id: D3
    description: "A clean independent deep review is bound to the final HEAD"
    requirement: "RUN-04"
    verification:
      - kind: review
        ref: "06-REVIEW.md, Gap-repair re-review 5 (status clean, reviewed source c2d2456); plan verify prints 'review bound to'"
        status: pass
    human_judgment: true
    rationale: "Rendered outcomes and the human qualitative judgments belong to 06-25"

metrics:
  duration: 2688min
  completed: 2026-11-27
---

# Phase 6 Plan 24: Final bindings, no-window gate and independent review Summary

**The ac-08 repository bindings, the asset binding cascade and the current producer replay are derived from the repaired source, the 71-case no-window gate passes, and a fresh independent gsd-code-reviewer reached a clean deep review after one full review and five re-reviews. The reviewed source is `c2d24563389c22628deb301dda4f4d2a8e791997`. `.local/temp/phase06-review.json` binds it to the final HEAD for the 06-25 rendered runner.**

## Performance

- **Duration:** about 44.8 hours of wall-clock time (2026-11-25T19:46Z to 2026-11-27T16:34Z). Most of it was spent in no-window gates of about 110 minutes each, reviews and rederivations.
- **Tasks:** 2 of 2
- **Commits:** 43 plan commits (`1e1fe59..e4b556a`) plus the completion docs commit
- **Files:** 35 non-planning files changed (1420 insertions, 368 deletions), plus 06-REVIEW.md and deferred-items.md

## Accomplishments

### Task 1: bindings at the repaired HEAD and the no-window gate

**Bindings.** At `1e1fe59`, `ac08-bindings.cjs --check` reported **127 differences**. The plan had expected 118 at `09e2881`; 06-23 had added more drift since then. `--write` added 78 rows (70 Phase 6 C# sources and 8 outside-validator rows). It also rehashed 40 changed sources and 2 registries and rebuilt 19 source stages (`9062ff0`).

**What the first write missed (`45468d7`).** The Phase 6 sources grow the ac-08 source stages from 15 to 19. `--write` only rehashed the callers, so R-50.positive and rules.exemptions kept 15 source stages (`CALLER_RESPONSIBILITY_UNION`), and the xUnit theories had no rows for `repository-sources-15..18` (`PRODUCER_DISCOVERY_IDENTITY`). `ac08-bindings --write` now rebuilds each caller's contiguous repository run and its `overallMs` from the derived stages, as `check.cjs callerExpected` does. `--check` also reports caller drift and theory-row drift. ExemptionTests gained the 12 missing `[InlineData]` rows, and bindings.test.cjs adds growth, missing-row and extra-row controls.

**Failures the old F3 hash failure had hidden:**

| Commit | Failure | Repair |
| --- | --- | --- |
| `05a81b4` | `asset-bindings.cjs --check` had 4 problems: a stale census.cjs `moduleSha256` (06-18), a pack-index row for FoundationRoot.cs (06-21), `content.validate` 159 of 180, and `SUPPORT_CASE_BINDING` | `--write`, then `modules=423 valid=180` |
| `25679c9` | gameplay-checks.core.permissions, stage foundation-native-event: `CONSUMER_COMPILATION` CS1739. The proof's FoundationView stub still had the pre-06-12 `ProjectRoster` | Both stub signatures declare `double? camera = null, bool ownLiving = true` |
| `292306a`, `391908f` | content.census, export.hub-consumer and export.portability: 158 of 180 source-ready. ui-identity.json still pinned the pre-06-21 FoundationRoot.cs | The identity closure was rebound (13 files, 38 hash lines), and the delivery fixture catalog was rederived |
| `bcab8c2` | content.census cohort dt-0 failed on "documentation decoded hash/size": the packaged gzip copy of fields.md was stale | `text-data.cjs --author-documentation`, with its hash carried to compatibility, delivery and pack-index |
| `0e8b859` | The 90-minute asset-pipeline group cap killed gate `f9e77610` at text.support-native after 27 passes | The group limit is now 150 minutes (see Deviations) |

`30c2679` and `c698395` rederived the ac-08 bindings after these source changes.

**No-window selection.** The selection is the phase-end mode for phase 6 with base `a9ac95f88a3f1b4cfe5b733ba707d6b038dad06d`, plus the run-1 failed and blocked ids, keeping tiers 1 and 2. It is written to `.local/temp/06-24-no-window-selection.json` with each case's reason. 71 cases were kept and 49 tier-3 cases were excluded as needing a game window:

verification.rules, verification.engine, verification.runner, verification.register, gameplay-checks.core.authority, gameplay-checks.core.input, gameplay-checks.core.lifecycle, gameplay-checks.core.permissions, gameplay-checks.runner.integrity, gameplay-checks.runner.processes, gameplay-checks.runner.repeatability, lan-checks.core.authority, lan-checks.network.tracer, lan-checks.core.permissions, lan-checks.core.admission, lan-checks.core.ordering, lan-checks.core.protocol, lan-checks.network.four-peer, lan-checks.core.lifecycle, lan-checks.network.lifecycle, lan-checks.network.idle-poll, lan-checks.adapters.discovery, lan-checks.adapters.profiles, lan-checks.network.discovery-profile, lan-checks.core.input, lan-checks.adapters.substitutions, lan-checks.runner.tracer, lan-checks.runner.timing, lan-checks.runner.integrity, lan-checks.runner.processes, lan-checks.runner.repeatability, asset-pipeline.source.publication, asset-pipeline.layouts.identity, asset-pipeline.layouts.identity-integrity, asset-pipeline.layouts.inventory, asset-pipeline.layouts.legacy-target, asset-pipeline.layouts.camp, asset-pipeline.layouts.map-warnings, asset-pipeline.animation.melee, asset-pipeline.animation.replacement, asset-pipeline.animation.bow, asset-pipeline.animation.work-special, asset-pipeline.appearance.corpses, asset-pipeline.appearance.crowds, asset-pipeline.layouts.application-icon, asset-pipeline.content.source-boundaries, asset-pipeline.content.archive-boundaries, asset-pipeline.content.source-relations, asset-pipeline.content.domain-relations, asset-pipeline.content.census, asset-pipeline.content.native-permissions, asset-pipeline.hub.source, asset-pipeline.source.packaged-expectations, asset-pipeline.text.catalog, asset-pipeline.text.compatibility, asset-pipeline.text.support-catalog, asset-pipeline.text.support-native, asset-pipeline.export.hub-consumer, asset-pipeline.export.portability, phase06.headless.world, phase06.headless.start, phase06.headless.inventory, phase06.headless.combat, phase06.headless.lifecycle, phase06.headless.lan-privacy, basic.strict-build, basic.repository, basic.pure, basic.headless, phase06.packaged, verification.state

All nine cases the plan requires are in the list.

**Result.** Gate `d9f598ec` passed **71/71** in one invocation on `0e8b859`, and F3 run `b3aa3f80` passed 6/6. gameplay-checks.core.permissions passes again. The run directories of these two ids are no longer kept under `.local/runs`, and their durations were not carried over to this summary.

### Task 2: independent review and repair rounds

Each review was run through `/gsd-code-review` at deep depth. Each time it spawned a fresh gsd-code-reviewer agent that had not written the repairs. Each round is an appended section of 06-REVIEW.md. The original 06-14 record and every earlier round are kept unchanged.

| Round (record) | Reviewed source | Findings | Fixes and derivations |
| --- | --- | --- | --- |
| Gap-repair review (`25470d0`) | `0e8b859`, 73 files since `a9ac95f` | 0 P0/P1, 2 P2, 7 P3: **GR-01** LAN pictures bypass the settled-view gate; **GR-02** the clipped-text gate ignored Hint, Roster, SpectatorCard and ControlTransferred; **GR-03** frame and draw spans could silently skip rows; **GR-04** the receipt match was not bound to the submitted sequence; **GR-05** interleaved receipts leaked into the next case; **GR-06** the contested withdrawal decided "stale" from a draw sampled before the pointer moved; **GR-07** world-help visibility had two writers; **GR-08** equipment slot buttons lost their accessible name; **GR-09** a new blank line at end of file | Fixes `cab791f` (GR-09), `760e71f` (GR-01), `f9e61fd` (GR-02), `1ed3aa5` (GR-03 to GR-06), `798ee3e` (GR-07), `27cc231` (GR-08), `af69841` (GR-03 food and empty-ammo spans), `bc1fce6` and `ec42745` (GR-04 LAN permission proof stubs). Derivations `707837a`, `e9fc757`, `3b91270`, `fe7d058`, `c54a3a1`, `860bb93`. Deferred items `1771c2e` |
| Re-review 1 (`f177954`) | `860bb93`, 28 files | All GR resolved, with residuals accepted as D1 and D2. **NR-01** (P3): the solo handover spans still read the rolling draw buffer. **NR-02** (P3): the FoundationAutomation acknowledgement comments misdescribed guest workspace requests | `24d7de9` (NR-01), `d5b2566` (NR-02), derivations `9285731`, `607ce09`. No-window rerun `dc308851` 32/32 on `607ce09` |
| Re-review 2 (`a80595e`) | `607ce09`, 6 files | NR-01 and NR-02 resolved. **NR-03** (P3): the `input.ack` reader comment at network.cjs:82 still stated the old contract | `0ae513a` (comment only). Rerun `ebdc80a0` 21/21 on `0ae513a` |
| Re-review 3 (`5f7ca93`) | `0ae513a`, 1 file | NR-03 resolved. **NR-04** (P3): the `Deliver` summary said it returns the sequence it submitted, which a guest workspace request contradicts | `1e8c124`, derivations `898a85d`, `ad960f3`. No-window rerun `7c70e62a` 32/32 on `ad960f3`; ac-08 run `9a4ce760` 6/6 |
| Re-review 4 (`ab9df14`) | `ad960f3`, 4 files | NR-04 resolved. **NR-05** (P3): the ack contract named the workspace request as the only unreported allocation, but `fixture-attack` and `spectator-proof-lethal` also allocate a sequence that `Deliver` drops | `3470f15` (comments only), derivations `2b2cd34`, `c2d2456`. No-window rerun `3406d111` 32/32 on `c2d2456` (6,637 s); ac-08 run `3129463a` 6/6 |
| Re-review 5 (`e4b556a`) | `c2d2456`, 6 files | **Clean**: no open P0 to P3 finding. NR-05 resolved. `ac08-bindings`, `current-replay` and `asset-bindings --check` all pass | None |

**The eight required checks** are recorded in the first round and re-confirmed for each delta:

1. F1 preparation stays bound to the current source.
2. Receipt observation does not change input routing.
3. F13 receipts are matched exactly, without retries. After GR-03 to GR-05 they are also bound to the sequence and gap-free.
4. ExportDebug adds DEBUG only to ExportDebug, never to Release.
5. The rebound pins were derived, not edited by hand.
6. The census has 252 tuples.
7. The UI fixes are consistent with 06-UI-SPEC, D-08, D-14, D-37, E068, E126 and E128, after the GR-07 and GR-08 fixes.
8. After GR-01, GR-02, GR-03 and NR-01, the predicates reject unsettled, clipped or single-draw evidence.

**Review record.** The 06-REVIEW.md front matter now names `revision: c2d24563389c22628deb301dda4f4d2a8e791997`, `base_revision: a9ac95f…`, `depth: deep` and `status: clean`. The 06-14 values are under `prior_review`.

## Verification

| Command | Result |
| --- | --- |
| `node src/godot-game/tools/verification/ac08-bindings.cjs --check` (re-review 5) | `PASS ac08-bindings c2d24563389c rows=612 checked=301 sourceStages=19 callers=6 repositorySha256=a3390f2a…e90b` |
| `node src/godot-game/tools/verification/current-replay.cjs --check` (re-review 5) | `PASS current-replay 57 historical, 22 changed, 22 added; sourceInputsSha256 3673e23d…b8b2` |
| `node src/godot-game/tools/verification/asset-bindings.cjs --check` (re-review 5) | `PASS asset-bindings modules=423 valid=180` |
| F3 verification cases (`ac-08.complete`, `R-50.positive`, `rules.godot-callsite-boundary`, `rules.godot-companion-positive`, `rules.godot-companion-negative`, `rules.exemptions`) | `b3aa3f80` 6/6 (Task 1); `9a4ce760-ed1a-48ec-ab93-7104b2d258df` 6/6 on `ad960f3`; `3129463a-79a8-4fb2-922f-b3c70d98fe98` 6/6 on `c2d2456` |
| No-window gate | `d9f598ec` 71/71 on `0e8b859` (Task 1); `dc308851-1f4c-4f93-8a3b-2d5d71a817bb` 32/32 on `607ce09` (6,628 s); `ebdc80a0-f8f6-4658-b25f-90bbeb762742` 21/21 on `0ae513a` (434 s); `7c70e62a-5bc3-43e1-be39-7beb91f8fe55` 32/32 on `ad960f3` (6,628 s); `3406d111-4af1-4126-9bd4-b66bb32eef6e` 32/32 on `c2d2456` (6,637 s) |
| `node --test --test-isolation=none src/godot-game/tests/scenarios/phase06-drivers.test.cjs` (re-review 5) | 71 pass, 0 fail |
| `node src/godot-game/tools/verification/check.cjs --group docs` | `3f66cf1f-98d3-4689-a968-9dadb811d3a0` PASS 36/36 |
| Plan verify: the `node -e` review-binding check | Prints `review bound to <HEAD> source c2d24563389c22628deb301dda4f4d2a8e791997` on a clean worktree once the review JSON is written after the final planning commit |

No game window was opened in this plan.

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 3 - Blocking] The asset-pipeline group limit was raised from 90 to 150 minutes**
- **Found during:** Task 1
- **Issue:** A passing content.native-permissions case takes about 65 minutes (3913 to 3944 s in a19c76b1, db6fc539 and ac07550f), so the full 28-case group needs about 92 minutes. The 90-minute cap killed gate `f9e77610` at text.support-native after 27 passes.
- **Fix:** The asset-pipeline child limit in `game-tests/suites.cjs` is now 150 minutes, with a comment that gives the reason.
- **Commit:** `0e8b859`

**2. [Rule 1 - Bug] ac08-bindings --write did not derive the caller stages or the theory rows**
- **Found during:** Task 1
- **Fix:** See Task 1 (`45468d7`). This changes ac08-bindings.cjs, bindings.test.cjs and ExemptionTests.cs, which are outside the plan's file list.

**3. [Rule 1 - Bug] Failures hidden by the F3 hash failure, fixed outside the plan's file list**
- **Found during:** Task 1
- **Fix:** The permission-proof stub (`25679c9`), the identity closure (`292306a`, `391908f`), the packaged field reference (`bcab8c2`) and the asset cascade (`05a81b4`).

**4. [Rule 1 - Bug] The NR-05 fix first failed the strict build, and that commit was reset before any push**
- **Found during:** Task 2 (NR-05)
- **Issue:** The first NR-05 commit, `11cf23d1ca1b6243c6e1fa4753564ba5369689ab`, had a 121-character `///` line. No-window run `94a3bed5-fa45-4f7f-8fb5-48d14d0785e9` failed basic.strict-build with `FoundationAutomation.cs(62,121): error SCR0007` (0/32 in 19 s).
- **Fix:** The local commit was reset before any push. The rewritten comment was committed as `3470f15`, where the longest changed line is 120 characters. It then passed run `3406d111` 32/32. `11cf23d` is not on the branch.

**5. [Rule 3 - Blocking] The rederivation chain after every source fix**
- **Issue:** Every fix to a bound source, including comment-only fixes, changes a pinned hash. The order is fixed:
  1. `current-replay.cjs --write` for game C# sources. It rehashes `engine-generated-spans.json` and its two `registries.spans.sha256` rows.
  2. `asset-bindings.cjs --write`, plus the identity-closure pins and `text-data.cjs --author-documentation`, when FoundationRoot.cs or a content pin changes.
  3. `ac08-bindings.cjs --write` last.
- **Commits:** Task 1: `9062ff0`, `05a81b4`, `30c2679`, `391908f`, `c698395`. GR: `707837a`, `e9fc757`, `3b91270`, `fe7d058`, `c54a3a1`, `860bb93`. NR-02: `9285731`, `607ce09`. NR-04: `898a85d`, `ad960f3`. NR-05: `2b2cd34`, `c2d2456`.
- **Review:** Each re-review compared these as hash-only token diffs against recomputed values.

**6. [Rule 1 - Bug] GR-04 broke the LAN permission proof**
- **Found during:** Task 2
- **Issue:** `Deliver` now returns a `long`, and `return [];` cannot initialize it. lan-checks.core.permissions failed with CS9174 (game-tests `666af558`).
- **Fix:** `bc1fce6` added a nested conditional, which then failed SCR0009 (`9651621c`). `ec42745` turned the stub per return type into a switch expression.

**Total deviations:** 6, all auto-fixed. No architectural change.

## Deferred Issues

These are recorded in deferred-items.md under "Found during 06-24":

- **BuildFixture scratch leak:** `src/godot-game/tests/verification/BuildFixture.cs:27` creates `.local/temp/build-<guid>` (about 55 MB each, about 280 per full gate) and never deletes it. Until its owner fixes this, delete `.local/temp/build-*` after each run finishes, never during one.
- **Stale catalog text:** `phase06.lan.population` at `src/godot-game/tools/game-tests/catalog.json:303` says "move one guest with D", but since 06-22 all three guests move.
- The 06-20 presenter tuple is gone again after GR-07, which removed `KeepHintClosed`. The 06-18 `guardedHostTuples` decision is unchanged.

**Not yet proven on screen (owned by 06-25):**
- **The `clipped` field in a rendered draw.** The clip gate now covers every observed run text layer, but it has only been tested on synthetic drivers-test rows. No rendered draw has yet shown the field set on real clipped text, or clear on fitted text.
- **The cause of the 06-22 guest-1 Ctrl-click.** Run `6d406c5e` showed that the click submitted no quick request. The suspected cause, ordering within one input burst, is inferred and not proven. The withdrawal is now frame-stepped and judged from the resting draw (GR-06), and the rendered conflicts rerun must prove it.
- **The near-successor smooth handover.** It is assumed, not observed, because the territory condition cannot be seen. The spans are now gap-free (NR-01). If the product cuts, `handover` reports `expected smooth, observed cut`.

## Known Stubs

None.

## Threat Flags

None. T-06-44 is mitigated: each review came from a fresh gsd-code-reviewer through the workflow, and the record is bound to the exact clean HEAD by the plan's node check. T-06-45 is mitigated: the bindings were derived only by the `--write` tools from committed HEAD, and each re-review read the resulting changes as hash-only diffs.

## Self-Check: PASSED

- Files present: 06-24-SUMMARY.md, 06-REVIEW.md (front matter `status: clean`, `revision: c2d2456…`), deferred-items.md, verification and gameplay-checks required-cases.json, ac08-bindings.cjs, `.local/temp/06-24-no-window-selection.json` (71 ids).
- Commits on HEAD: `9062ff0`, `45468d7`, `05a81b4`, `30c2679`, `25679c9`, `292306a`, `391908f`, `c698395`, `bcab8c2`, `0e8b859`, `25470d0`, `cab791f`, `760e71f`, `f9e61fd`, `1ed3aa5`, `798ee3e`, `27cc231`, `707837a`, `e9fc757`, `3b91270`, `fe7d058`, `1771c2e`, `af69841`, `bc1fce6`, `c54a3a1`, `ec42745`, `860bb93`, `f177954`, `24d7de9`, `d5b2566`, `9285731`, `607ce09`, `a80595e`, `0ae513a`, `5f7ca93`, `1e8c124`, `898a85d`, `ad960f3`, `ab9df14`, `3470f15`, `2b2cd34`, `c2d2456`, `e4b556a`.
