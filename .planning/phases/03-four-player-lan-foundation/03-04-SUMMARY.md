---
phase: 03-four-player-lan-foundation
plan: "04"
subsystem: testing
tags: [godot, csharp, lan, enet, native-input, rendered-evidence, windows]
status: complete
requires:
  - phase: 03-03
    provides: LAN browser, persistent profiles, session lifetimes and guarded native projection
provides:
  - Exact saved 481-case selection and grouped native LAN proof runner
  - Physical input and completed-draw timing with immutable process/image evidence
  - Complete current visual review, historical failures and validated completion index
affects: [phase-04, phase-05, verification, local-testing]
tech-stack:
  added: []
  patterns: [separate observation and input, owned process cleanup, source-bound evidence, exact visual bindings]
key-files:
  created:
    - tools/phase3/check.cjs
    - tools/phase3/evidence.cjs
    - tools/phase3/required-cases.json
    - tools/phase3/windows-input.ps1
    - src/godot-game/foundation/FoundationDrawObserver.cs
    - .planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md
    - .planning/phases/03-four-player-lan-foundation/03-SELECTION.json
    - .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json
    - .planning/phases/03-four-player-lan-foundation/03-VISUAL-REVIEW.md
  modified:
    - tools/phase2/rendered.cjs
    - tests/phase1/FoundationLanRunnerTests.cs
    - tests/phase1/FoundationLanPermissionTests.cs
    - docs/workflow/lan-foundation-checks.md
    - .planning/phases/03-four-player-lan-foundation/03-04-CHECKPOINT.md
key-decisions:
  - "Use a common external monotonic bracket through complete PNG validation, including all scheduling costs and 1 ms uncertainty."
  - "Keep physical SendInput, controlled window messages and synthetic ParseInputEvent provenance distinct."
  - "Require fresh exact whole-selection execution after reviewed repairs; historical reports never replace current execution."
  - "Reuse an actual visual inspection only for exact hash/byte-identical content with explicit current context bindings."
  - "Pin both assembly SourceRevisionId and repository SourceRoot revision for reproducible GitHub SourceLink."
patterns-established:
  - "Read-only FramePostDraw observations never mutate controls or simulation."
  - "Every case, peer, role, actor, profile, process, image and report has an exact validated identity."
  - "A behavioral pass cannot override a real visual acceptance failure."
requirements-completed: [LAN-01, LAN-02, LAN-03, LAN-04]
duration: 833min
completed: 2026-11-16
actuals:
  tokens: 505622
  tasks: 3
  commits: 83
commits: 83
plan_head_before: 5f4480c5c795834c88d45e90611be6c856abb1fd
plan_head_after: a36cdad2b74550ffb8edaa02339bc0b968a75282
coverage:
  - id: D1
    description: "Exact runner membership, provenance, process failures and guarded observation/input separation"
    requirement: LAN-01
    verification:
      - kind: integration
        ref: "tools/phase3/check.cjs --all; final report 841e10b1-f825-4527-865b-4703382f51eb"
        status: pass
    human_judgment: false
  - id: D2
    description: "Four real peers with physical rendered timing, synchronized stopped positions and session lifecycle"
    requirement: LAN-04
    verification:
      - kind: automated_ui
        ref: "rendered.timing: 48 physical rows and 16 stopped comparisons"
        status: pass
    human_judgment: false
  - id: D3
    description: "Current menu/browser/profile and actor-marker appearance"
    verification:
      - kind: other
        ref: "03-VISUAL-REVIEW.md: 319 explicit current image bindings"
        status: pass
    human_judgment: false
---

# Phase 3 Plan 4: Trustworthy LAN Runner and Native Evidence Summary

**Exact source-bound proof tooling now validates four real LAN peers, physical rendered response, owned cleanup and every required actual image across a fresh 481-case selection.**

## Performance

- Wall-clock span: 833 minutes from first RED commit to measured closure preparation, including independent reviews, failed native attempts, bounded repair cycles and five fresh full attempts.
- Completed: 3/3 tasks; 91 changed files in the measured plan diff.
- Actual token estimate: 505622 = ceiling(2022485 realized diff characters / 4), including production/tests/tools and extensive bound evidence/history metadata. This is the plan's estimate scale, not a harness-token count.
- Commit count: 83, measured from the persisted plan ledger `5f4480c5c795834c88d45e90611be6c856abb1fd` through `a36cdad2b74550ffb8edaa02339bc0b968a75282`. The separate summary commit follows this measured task boundary.

## Accomplishments

- Added the small Node/PowerShell runner with `--case ID[,ID]`, `--group`, `--list`, `--all`, `--failed` and `--verify-completion`. Unknown, duplicate, zero, stale, missing, substitute and forged evidence fails closed. Real crash/timeout controls prove descendant cleanup.
- Kept actual observation separate from input: completed `FramePostDraw` traces and PNG/state checkpoints are read-only; owned physical, targeted window-message and synthetic drivers retain distinct provenance. No focus-clearing behavior was disabled.
- Saved selection and scenarios before launching: all 18 acceptance criteria and 20 resolved edges are mapped, with importance/rationale, setup, actions, results, required evidence, omissions and stable G0–G4 ordering. Current selection contains 433 Phase 1, all 16 Phase 2 and 32 Phase 3 cases.
- Bound source/tool/OS/build revision, actual execution HEAD, nonce, PID/profile/actor/session/binding/revision, required case/role/checkpoint membership, actual logs, PNG dimensions/content/hash and named visual judgments.
- Preserved all failures through repair cycles. The fourth full behavioral pass exposed two actual appearance defects; both were repaired, independently reviewed and checked visually before the fifth full run.

## Task Commits

| Task | RED | GREEN / completion | Outcome |
| --- | --- | --- | --- |
| 1: completed-draw observations and conservative native timing | `000ec98` | `2eccd01` | Tracer/timing/permissions gate passed before expansion. |
| 2: exact failure controls and saved grouped selection | `a046c70` | `d40df64` | Complete non-window tooling and scenario selection implemented. |
| 3: real grouped execution and actual image review | Historical checkpoints retained | `a36cdad` | Fresh 481/481, 48 physical timing rows, 16 stopped comparisons and 319 visual bindings accepted. |

Independent whole-phase review and bounded repairs were committed by the parent between these task boundaries. The complete measured ledger is below.

## Final Verification

Reviewed source/build pin: `c69442148c975de974ebbb1d8ef409380940ecb0`; actual execution HEAD: `6ea600953be031bf2b467a50d35090a94e44f802`.

Exact input `9e98ad7b705fbc417b371da7a48ad3c3713f635fa9672c4de2afe6f7221c5d32`; tool `9e911416e13c7c81e38f0f9883062d1310cfaa085e9b2e571c8b64336b5621b9`. Both `SourceRevisionId` and absolute `CustomAfterMicrosoftCommonTargets=.../tools/phase3/reviewed-source.targets` remained set in the isolated SDK environment for all current runs and children.

Parent's mandatory metadata rebuild passed with zero warnings/errors and exact matching fingerprints before execution. Independent review was clean: 18 findings resolved, scope 123 paths/120 source files. All 220 generated-span hashes, 45 native method permissions and eight public surfaces remained unchanged.

| Current proof | Result | Immutable report |
| --- | --- | --- |
| Early inherited rendered group | 9/9 | `.phase2/runs/win32-x64/be4ab0df-af06-48a4-a766-3b6cdf8dac88/summary.json` |
| Early LAN rendered group | 12/12 | `.phase3/runs/win32-x64/e06596d1-0660-44dd-8488-0c2bbb69e963/summary.json` |
| Early repaired-image gate | Seven actual image bindings pass | `.phase3/task3-batch11-targeted-visual.json` |
| Fresh entire selection | **481/481**, exit 0, 3,363,558 ms | `.phase3/runs/win32-x64/841e10b1-f825-4527-865b-4703382f51eb/summary.json` |
| Inherited Phase 2 core / runner / solo | 4/4, 3/3, 9/9 | `e39eaabd...`, `b4dbdff2...`, `1e75e6b9...` (full references in final report) |
| Actual final visual review | **319/319 bindings pass**, 255 distinct contents | `03-VISUAL-REVIEW.md`, `03-EVIDENCE.json` |
| Original completion reconciliation | PASS, exact tested HEAD | `.phase3/task3-fifth-full-completion.console.log` |

Final report SHA-256: `b7bdf55d74034a5c50527f0e8391e7a7be01d1e46867dd103b9d381e6b2e6bae`.

All 48 origin/observer/action timing rows use actual physical SendInput with conservative upper bounds including delivery, callback acknowledgement, draw observation, capture, PNG read/decode/hash, polling/scheduler costs and 1 ms uncertainty. Maximum upper bound: **197.8586 ms**, below the unchanged 250 ms limit. All 16 stopped comparisons have **0 subunits** host disagreement, maximum observation upper **171.4658 ms**, below 500 ms. Four real timing PIDs were 34924, 13612, 26056 and 28316, with distinct profiles/actors and validated session/authority binding in the report. Controlled window-message simultaneous walking remains separate from physical foreground timing and synthetic controls.

All 319 current images have explicit case/role/checkpoint/report/hash/context review rows. The executor actually opened 221 new or required-current contents; 34 contents match specific prior actual inspections by exact SHA-256 and complete byte equality, with current context reconciled. Repaired browser and both edge-marker images were actually inspected again in the final full run. No sampling or data-only appearance credit was used.

Cleanup: 14 native terminals closed (12 ordinary Quits, two declared losses, zero emergency kills); 17 headless terminals closed (15 normal, two declared losses, zero emergency kills); all 11 native helpers ended; PNG workers ended with runner exit 0; desktop lock absent. All 688 console/stdout/stderr logs had no error/warning/exception/MTU matches. The ignored audit retains every terminal/helper/log hash.

## TDD Gate Compliance

- Task 1 and Task 2 used genuine RED commits before GREEN implementation, with `RED_EVIDENCE_OK` classifier records in `.phase3/tdd/03-04-task1-red.json` and `.phase3/tdd/03-04-task2-red-valid.json`.
- The initial malformed nested-error TAP was rejected and retained; it did not authorize implementation. Sandbox-only process/build failures were not treated as behavioral RED evidence.
- Each later source repair followed the parent-controlled repair/review/new-pin gate before native execution. Full selected execution and actual appearance were required again after source changes.

## Deviations from Plan

### Correctness repairs discovered by independent review and real execution

1. **[Rule 1 — evidence and lifetime bugs]** Whole-phase review found ownership, replay-budget, binding, stopped-reference, terminal and inherited-evidence weaknesses. Parent fixed all 18 review findings and independently rechecked the complete current scope before launch.
2. **[Rule 3 — reproducible build blocker]** Assembly pinning alone did not pin actual GitHub SourceLink `SourceRoot.RevisionId`. A real GitHub five-build RED/GREEN fixture established the missing SDK dependency; the phase-only target now pins the reviewed repository root while preserving real SourceLink. Failed prelaunch evidence remains historical.
3. **[Rule 1 — native procedure and observation bugs]** Real Windows runs exposed DPI/foreground/Unicode typing, profile-popup navigation, scrolled session selection, cancellation retirement and draw-request file sharing issues. Bounded repairs preserved ordinary Godot callbacks, owned process cleanup and exact source identities.
4. **[Rule 1 — physical response and transport bugs]** Serial image validation and delayed/stale snapshot delivery failed genuine 250 ms measurements. Persistent PNG workers, per-observer captures, current-step publication, replaceable ordered full-state snapshots and bounded wire compression repaired causes without subtracting costs or relaxing thresholds.
5. **[Rule 1 — actual visual defects]** The fourth full run passed behavior but failed real appearance: stale Receiving session state in an idle browser and clipped P1 · You at viewport edges. Parent added real/native regression controls, cleared progress on readiness/navigation and clamped visible labels after camera projection. Early actual image review and the fifth fresh full confirmed both fixes.

No package/project/reference additions, analyzer weakening, generated-span refresh, focus-clearing disablement, threshold relaxation or synthetic physical credit were introduced.

## Retained History

`03-EVIDENCE.json` binds 18 historical primary reports (17 Phase 3 plus focused Phase 2) and 1,288 inherited report references. This includes original full `28bb1d55...`, failed full `30ebcea8...` and `f736803d...`, all focused native attempts, browser-only diagnostics `d30c16a4...`/`2a2b232b...`, and fourth full `b0f89396...` with its distinct behavior PASS/visual FAIL.

The prior failed visual Markdown is preserved in Git `446dbd1` and ignored hash archive `.phase3/historical-visual-reviews/acbba4583c95c3e474e193561103b5964df0a05f8a32a86ee9177f9f35f8d40b.md`. Original nonces, revisions, logs, physical inputs, raw timing rows, draw traces, images and cleanup outcomes remain immutable. No historical execution substitutes for the current fresh 481 cases.

## Scope and Handoff

This completes Plan 03-04's **Windows 1280×720 local development** proof, with no VM. It does not claim the later physical mixed-OS release checklist. No unresolved stub or acceptance blocker prevents this plan's foundation/tooling goal; later gameplay remains assigned to later phases.

Parent owns final phase verifier/audits, STATE/ROADMAP/REQUIREMENTS transition and PR/publishing. Those shared files were deliberately not changed by this executor. Final metadata-only closure validation is retained in `.phase3/task3-fifth-full-closure.console.log`.

## Complete Measured Commit Ledger

| Commit | Subject |
| --- | --- |
| `000ec98` | test(03-04): require conservative post-draw timing bounds |
| `2eccd01` | feat(03-04): bind native observations to exact evidence and timing |
| `a046c70` | test(03-04): require exact exhaustive and invalid selection behavior |
| `d40df64` | feat(03-04): prove runner failures and save grouped LAN scenarios |
| `6dd2419` | docs(03-04): record implementation review checkpoint |
| `624f37b` | docs(03): record whole-phase code review findings |
| `a29602d` | fix(03): CR-01 restore browser focus after completing rows |
| `494a197` | fix(03): CR-02 observe existing native controls and absent profiles |
| `10b83d6` | fix(03): CR-03 scope admission replay budgets to connection lifetimes |
| `3a82df1` | fix(03): CR-04 encode opaque actor identities before native projection |
| `7e9664e` | fix(03): CR-05 bind stopped comparisons to native releases and peer draws |
| `ab4cb05` | fix(03): CR-06 reconcile every owned process terminal outcome |
| `df4b90d` | fix(03): WR-01 acknowledge captures only after atomic publication |
| `28ab751` | fix(03): WR-02 traverse the current menu in inherited keyboard checks |
| `51b6718` | fix(03): WR-03 select affected analyzer and generated-register consumers |
| `e9bc36a` | fix(03): WR-04 serialize foreground input and target concurrent joins |
| `bf94821` | fix(03): CR-03 retire disconnected admission histories without recreation |
| `7f9bfd0` | fix(03): CR-05 retain each capture own subject and revision |
| `640e1c3` | test(03): CR-06 fail real peer exits after completed capture |
| `9f72121` | fix(03): WR-03 update exact inherited fixture and ownership consumers |
| `ba56010` | fix(03): CR-07 validate immutable evidence through metadata finalization |
| `3b43287` | fix(03): CR-08 bind selected session identity to join lifetime |
| `6b24056` | fix(03): WR-05 retain inherited failures and continue independent checks |
| `a35bd6f` | fix(03): CR-07 pin reviewed source stamp across metadata rebuilds |
| `166a32a` | fix(03): CR-09 reject actor time regression and protect native camera |
| `1dc1998` | docs(03): close code review and authorize source-bound desktop verification |
| `e25a500` | fix(03): allow exact phase verification metadata at evidence closure |
| `b5c603f` | docs(03): bind clean review gate to final closure integration |
| `a125b97` | docs(03-04): record SourceLink prelaunch blocker |
| `bc6b252` | test(03): reproduce GitHub SourceLink drift across review metadata |
| `cff08eb` | fix(03): pin GitHub SourceLink to reviewed source in phase proof builds |
| `914fc9b` | docs(03): renew clean review gate for reproducible GitHub proof builds |
| `d5d6cf0` | docs(03-04): preserve failed first selected desktop run |
| `dc523e9` | fix(03): repair selected proof fixtures and physical desktop driver |
| `8466836` | test(03): reproduce nested inherited proof reference contract |
| `7b140a2` | fix(03): validate inherited proofs in their exact case directories |
| `ab8c95b` | docs(03): close fifteen review findings before focused desktop retry |
| `6965f6c` | docs(03-04): preserve failed focused native retry checkpoint |
| `af8d4f2` | fix(03): pace native typing and prepare owned focus observations |
| `d2d5a61` | fix(03): recheck owned foreground during paced physical text |
| `024a6d6` | docs(03): bind reviewed native repair batch for focused verification |
| `df55fdf` | docs(03-04): retain batch two native failure evidence |
| `2902f7a` | fix(03): reduce exact PNG validation cost and restore owned focus order |
| `4ea54ca` | fix(03): keep scaled caption focus clear of system-menu controls |
| `032b3e8` | docs(03): record reviewed decoder and owned-window repair |
| `54502b4` | docs(03-04): preserve batch three native failure checkpoint |
| `6666db6` | test(03): require bounded graceful guest departure before transport close |
| `66ee040` | fix(03): drain guest departure and bound native observation delivery |
| `609c752` | test(03): reproduce premature connected browser cancellation close |
| `f32e0bd` | fix(03): retain cancelled connections through Root-owned graceful retirement |
| `6f5c6ab` | docs(03): close cancellation review and bind verified retirement build |
| `629ce51` | docs(03-04): retain batch four rendered failure checkpoint |
| `b9a38e5` | test(03): require actual leftward reversal draw witnesses |
| `4b83280` | fix(03): interact with visible rows and bind first leftward reversal draws |
| `df07eba` | fix(03): bind scrolled row clicks to the current discovered session |
| `9a5fe7a` | docs(03): bind visible interaction and reversal proof review |
| `ad9f220` | docs(03-04): retain batch five native failure checkpoint |
| `ba0b1b7` | fix(03): observe popup highlights and separate peer observation from capture |
| `deedbd9` | docs(03): record clean popup and capture review |
| `472b392` | docs(03-04): preserve batch six timing failure checkpoint |
| `93e8552` | fix(03): validate captured images without blocking peer observations |
| `139abe9` | docs(03): record clean concurrent image validation review |
| `ae0aae6` | docs(03-04): retain focused pass and full stop timing failure |
| `551d7c6` | fix(03): publish changed movement intent in the current host step |
| `9226cd5` | docs(03): record clean bounded publication review |
| `fb506c0` | docs(03-04): preserve third full start timing failure |
| `2f20e72` | fix(03): separate replaceable snapshots from reliable control traffic |
| `a10ad39` | docs(03): record clean snapshot delivery review |
| `59d753a` | docs(03-04): preserve draw request race and MTU warnings |
| `ece9de6` | test(03-04): reproduce draw request sharing lock failures |
| `38c2a0f` | fix(03-04): defer only locked draw requests until the next draw |
| `9c7dae1` | test(03-04): reproduce full snapshot native MTU overflow |
| `c9bb2da` | fix(03-04): compress bounded native frames without changing delivery modes |
| `8a7f3a7` | test(03-04): scope request deletion denial controls to Windows |
| `c9b83d3` | docs(03): record clean request and wire framing review |
| `446dbd1` | docs(03-04): retain full pass and actual visual failures |
| `78e13c6` | test(03-04): reproduce stale ready feedback and clipped edge markers |
| `9ebef7a` | fix(03-04): clear join progress on readiness and fresh browser entry |
| `625f566` | test(03-04): cover native marker bounds and feedback recovery |
| `a38b7df` | fix(03-04): keep visible participant markers inside the logical viewport |
| `c694421` | test(03-04): keep native regression reads inside guarded methods |
| `6ea6009` | docs(03): record clean feedback and marker review |
| `a36cdad` | test(03-04): bind accepted physical LAN and visual evidence |

## Self-Check: PASSED

All required delivered files, final report, visual/evidence bindings and original failed visual archive exist; task RED/GREEN and evidence commits are ancestors of HEAD. Fresh whole-selection and original completion reconciliation passed. No tracked deletion or untracked deliverable remains outside the intentional closure summary. Final closure validation is performed after this summary commit without rerunning game windows.
