---
phase: "03"
slug: "four-player-lan-foundation"
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-15"
---

# Phase 03 — Validation Strategy

> Based on the GSD VALIDATION template and 03-RESEARCH.md Validation Architecture. All execution statuses remain pending.

## Test Infrastructure

| Property | Value |
|---|---|
| Framework | Existing xunit.v3 4.0.2 / Shouldly 4.3.0 / existing Phase1.Tests executable; Node 24.22.1; pinned Godot .NET 4.7.2 |
| Config file | `tools/phase3/required-cases.json`, created by 03-01 |
| Quick run command | `node tools/phase3/check.cjs --case core.authority` after 03-01 creates it |
| Non-window groups | `node tools/phase3/check.cjs --group core` / `adapters` / `network` / `runner` |
| Full selected command | `node tools/phase3/check.cjs --all` only after whole-phase review/fixes |
| Completion command | `node tools/phase3/check.cjs --verify-completion .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` |
| Strict build | `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` |
| Estimated runtime | Focused subjects target <60 seconds after build; full groups measured during execution |

New phase3 commands are deliverables. Exact comma-separated phase3 case selection is implemented by 03-01; zero/unknown/duplicate membership fails. Existing phase1/phase2 commands retain their existing selector semantics. Every PLAN automated command has an immediate observable fails_when sibling.

## Sampling Rate

- After each task: focused non-window checks and strict build as appropriate. Headless native diagnostics use the ordinary project/main scene, with no rendered/physical-input credit.
- After each wave: available phase3 non-window groups, affected source/generated/callsite/graph/type/docs and inherited core/runner checks.
- Before any real window: complete the whole phase, review the entire diff and fix findings; save selection and compatible run groups.
- After review: Windows 1280×720 only, no VM. Select every active Critical/new/changed/affected case. Collect failures, continue independent cases, batch repairs, focused failed/blocked/newly affected reruns, then the entire selected set on the repaired build. Initial clean selection needs no duplicate run.
- Feedback target: <=60 seconds per focused subject. Full rendered groups and existing expensive compiler proofs may take longer; preserve scope and report progress.

## Per-Task Verification Map

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---|---|---|---|---|---|---|---|---|---|
| 03-01-01 | 01 | 1 | LAN-01–04 | T-03-01/02/03 | Sender-bound ready authority, bounded wire | unit/headless | strict build; `--case core.authority`; `--case network.tracer` | ❌ W0 | ⬜ pending |
| 03-01-02 | 01 | 1 | LAN-03/04 | T-03-04 | Exact native/public positive and adjacent rejection | scanner/compile | `--case core.permissions`; inherited focused checks | ❌ W0 | ⬜ pending |
| 03-02-01 | 02 | 2 | LAN-01–04 | T-03-05/06/07 | Capacity/readiness/replay/revision fences | unit/headless | `--case core.admission,core.ordering,core.protocol`; `--case network.four-peer` | ❌ W0 | ⬜ pending |
| 03-02-02 | 02 | 2 | LAN-01–04 | T-03-06/07 | Threshold and closed/replaced lifetime safety | unit/headless | `--case core.lifecycle,network.lifecycle,network.idle-poll` | ❌ W0 | ⬜ pending |
| 03-03-01 | 03 | 3 | LAN-01/04 | T-03-08/09/10 | Discovery revalidation, atomic exact profiles | socket/file/headless | `--case adapters.discovery,adapters.profiles,network.discovery-profile` | ❌ W0 | ⬜ pending |
| 03-03-02 | 03 | 3 | LAN-01/02 | T-03-10 | Context-owned input and local camera | unit | `--case core.input`; inherited core group | ❌ W0 | ⬜ pending |
| 03-03-03 | 03 | 3 | LAN-04 | T-03-11 | Individual actual service swaps, exact permissions | composition/scanner | `--case adapters.substitutions,core.permissions` | ❌ W0 | ⬜ pending |
| 03-04-01 | 04 | 4 | LAN-01–04 | T-03-12/13 | Actual observation, provenance, conservative timing | headless/unit | `--case runner.tracer,runner.timing` | ❌ W0 | ⬜ pending |
| 03-04-02 | 04 | 4 | LAN-01–04 | T-03-13/14/15 | Exact membership, real child failures, owned cleanup | runner | `--group runner`; `--list`; docs.navigation | ❌ W0 | ⬜ pending |
| 03-04-03 | 04 | 4 | LAN-01–04 | T-03-01–15 | Review first, fresh full selected-set evidence | rendered/review | strict build; `--all`; `--verify-completion .../03-EVIDENCE.json` | ❌ W0 | ⬜ pending |

All abbreviated phase3 selectors above use `node tools/phase3/check.cjs`. Status: pending/green/red/flaky; no pass is assigned at planning time. Shared T-03-SC preserves the audited dependency baseline; no new package install.

## Wave 0 Requirements

Wave 0 is embedded test-first work inside each feature task, not a separate horizontal plan or real game launch.

- [ ] 03-01: create check.cjs, required-cases.json, network.cjs, FoundationLanAuthorityTests.cs and FoundationLanPermissionTests.cs before consuming them; actual failing assertions precede implementation.
- [ ] 03-02: create FoundationLanLifecycleTests.cs / FoundationLanProtocolTests.cs and register exact named cases before expansion.
- [ ] 03-03: create FoundationLanServicesTests.cs / FoundationLanInputTests.cs with exact linked engine-free Compile inputs; no sixth project or new reference edge.
- [ ] 03-04: create FoundationLanRunnerTests.cs, evidence.cjs, runner-fixture.cjs, rendered.cjs, persistent owned windows-input.ps1, strict catalog and saved scenarios/selection.
- [ ] Implement every new selector/completion command before its consuming task. A missing binding/test/tool is failure/blocker, never empty success.
- [ ] Every new public/native tuple has exact positive and adjacent-negative proof; generated spans come only from actual authenticated strict-build output.
- [ ] No framework/package install. Calibration factor 1, samples 0, confidence low; plan estimates raw/calibrated 58000/40000/50000/55000 tokens.

## Acceptance Coverage

Case IDs are assignments to be registered. Every rendered case declares exact required roles, images, state observations and source AC in the independent catalog.

| Source | Implementation tasks | Cases and required observation | Group |
|---|---|---|---|
| AC-01 | 01-01, 02-02, 03-01/02 | core.input/lifecycle; rendered.entry: normal mouse/keyboard Start/Host/Join/Cancel/Back/Escape, held/repeated activation, one lifetime, bounded Quit | G1/G2/G3 |
| AC-02 | 03-01 | adapters.discovery, network.discovery-profile, rendered.browser: real discovery/manual, empty/Unicode/equal-name/duplicate/reorder/expired/stale-full rows | G2 |
| AC-03 | 02-01 | core.admission, network.four-peer, rendered.admission: 1/2/3/4, fifth Full, last-slot race including pending | G3 |
| AC-04 | 02-02, 03-01 | core.lifecycle/protocol, rendered.join-errors: invalid/unreachable/closed/full/incompatible/repeated/cancelled, exact 30000 ms timeout | G2/G3 |
| AC-05 | 01-01, 02-01 | core.admission/ordering, network.four-peer, rendered.moving-join: current baseline before control while existing actors continue | G3 |
| AC-06 | 02-01, 03-02 | core.authority/input, rendered.walkers: four simultaneous holds, aliases/opposition/repeat, signed 1-second 2.85..3.15u displacement, finite bounds/facing and own cameras | G3 |
| AC-07 | 04-01/03 | runner.timing, rendered.timing: every origin→observer start/reversal/stop <=250ms; every stopped peer <=50 subunits by 500ms, conservative uncertainty | G3 |
| AC-08 | 02-02, 03-02, 04-03 | core.input/lifecycle, rendered.focus: real OS focus stop <=250ms, fresh press on regain/return/join/rehost, independent ownership | G1/G3 |
| AC-09 | 01-01, 02-01 | core.authority/ordering: actual wrong-owner/unready delivery and solo/host/guest common accept/step with no host exemption | G0/G3 |
| AC-10 | 02-01 | core.ordering/network.four-peer: production duplicate/reordered guest and host-local replay; old move after newer stop/reversal | G0/G3 |
| AC-11 | 02-01/02, 03-03 | core.ordering/lifecycle/protocol, adapters.substitutions: old baseline/update/session/adapter/slot callbacks leave replacement unchanged | G0/G3 |
| AC-12 | 02-02 | core.lifecycle, network.lifecycle, rendered.guest-lifecycle: graceful/lost guest removes exact actor, 10000ms boundary, repeated cleanup, fresh replacement | G3 |
| AC-13 | 02-02, 03-02 | network.lifecycle, rendered.host-end/host-loss: distinct visible reason, usable menu, fresh host/join and no old listeners/input/actors | G3/G4 |
| AC-14 | 03-01/03 | adapters.profiles/substitutions, network.discovery-profile, rendered.profiles: real restart, equal-label distinct IDs, missing/invalid/active duplicate refusal | G2/G3 |
| AC-15 | 03-03 | adapters.substitutions: replace discovery/identity/connection individually through actual composition; unchanged Core hashes, unavailable service explicit | G0 |
| AC-16 | 04-02/03 | Saved exact cases/commands/steps/groups; real four-process run; all 16 inherited Phase 2 cases selected | G0–G4 |
| AC-17 | 04-01/02/03 | runner.tracer/timing/integrity/processes/repeatability; completion validator: exact fresh case+role+image set, actual child failures, immutable repair/final reports | G0–G4 |
| AC-18 | 03-01, 04-02/03 | adapters/substitutions, rendered.resources: no account/Internet/donor runtime dependency; owned profiles; honest Windows-only local scope | G0/G1/G2/G3 |

AC-16/17 historical two-local-OS/extra-resolution wording is superseded by current local-testing policy. Routine local Windows 1280×720/no VM applies, while product Windows/Linux support, 1920×1080 layouts and distinct physical mixed-OS release requirements remain intact. No unexecuted environment receives a pass.

## Edge Coverage

| Edge | Resolution | Concrete proof |
|---|---|---|
| LAN-01 boundary | AC-03/04 | 1..4/fifth/racing final slot/default timeout |
| LAN-01 adjacency | AC-02 | Merge duplicate session ads, retain equal-label different sessions |
| LAN-01 empty | AC-02/04 | Empty usable list/manual path, blank/malformed/cancelled attempt retains no slot |
| LAN-01 encoding | AC-02/14 | Unicode labels separate from full opaque IDs |
| LAN-01 ordering | AC-02 | Preserve selected ID across reorder or visibly clear on expiry |
| LAN-01 precision | AC-03/04 | Integer pending-inclusive count; 29999/30000/30001ms |
| LAN-01 idempotency | AC-01/04/12/13 | Repeated entry/hello/cancel/cleanup one lifetime |
| LAN-01 concurrency | AC-03/04/05/12 | Racing hello/cancel/ready/departure conserves slots |
| LAN-02 boundary | AC-06/07 | Inherited bounds; equality passes, strict timing/error overrun fails |
| LAN-02 precision | AC-06/07 | 3000subunits/s, 2850..3150 per second; finite/unrounded records |
| LAN-02 idempotency | AC-06/08/10 | Alias/repeat normalization, duplicate no extra step, fresh-press focus recovery |
| LAN-02 concurrency | AC-05–08/12 | Four movers, moving join, real focus and guest leave preserve other actors/cameras |
| LAN-03 idempotency | AC-09/10 | Common accept/step, duplicate guest/host-local replay rejection |
| LAN-03 concurrency | AC-09–11 | Sender/readiness/session/revision checks before mutation |
| LAN-04 empty | AC-14/15 | New profile one identity; invalid incoming ID/refused service explicit |
| LAN-04 encoding | AC-14 | Ordinal opaque IDs/case variants, no display/address replacement or truncation |
| LAN-04 idempotency | AC-14 | Actual restart and endpoint/peer changes preserve profile |
| LAN-04 concurrency | AC-11–15 | Duplicate cannot steal control; closed/replaced adapter cannot enter current session |
| LAN-03 ordering | AC-10/11 | Older move cannot undo stop; obsolete baseline/update cannot rewind |
| LAN-04 adjacency | AC-14 | Equal names distinct profiles; same active ID never second owner |

## Grouping, Selection and Evidence

G0: non-window build/unit/socket/file/real-headless/runner checks. G1: all nine inherited rendered cases in one ordinary solo process, menu resets, Quit last. G2: reusable host/browser roles for profiles/discovery/manual/errors; profile persistence needs an actual documented restart. G3: host+three actual guests for moving join/controls/camera/timing/focus, then challenger processes for capacity/final-slot/duplicate identity, guest loss/replacement and graceful host end/rehost. G4: unexpected owned-host loss last; reuse intact G3 set where possible. Extra challenger/restart/loss launches have concrete reasons; participants cannot be reduced to obtain one window.

03-SELECTION.json declares every active Critical/new/changed/affected case and every omitted ID/reason before execution. All 16 Phase 2 cases are selected because shared root/input/authority/projection/evidence changed. Inspect the actual Phase 1 required catalog for affected Critical/rules/state/graph/type/runner obligations; unknown impact blocks omission. Planned future cases receive no pass/skip credit, A-01–A-47 identities remain unchanged. This is not Phase 5's general selection/reuse framework.

One invocation owns profile roots, ports, process descendants and immutable `.phase3/runs/<platform>/<nonce>/` reports. Use the existing `.phase2/desktop.lock` to refuse uncontrolled overlap with inherited rendered runs. Validate exact case+role set, distinct PIDs/profiles/actors, session/binding/revision, nonce/input/tool/OS/revision and actual decoded images/state/review hashes. Duplicate guest results cannot replace an absent peer. Keep original/focused/final failures and invocations immutable; a later unresolved failure invalidates acceptance from an older pass.

Real foreground OS input and focus changes are mandatory. Targeted window messages must establish actual native key/held-state behavior and remain labeled as messages. If background holds fail, explicitly synthetic native events may prove only the simultaneous-control subcase. They cannot supply physical-input credit, disable focus clearing or bypass canonical input/authority. Unmet genuine input acceptance is a blocker. Observer remains read-only.

Timing uses one external monotonic clock with request/ack brackets and real completed-draw/image/transform observations. Include sample/clock/scheduling uncertainty conservatively. Startup-relative unrelated clocks, missing visible actor or slow screenshot polling cannot prove 250ms. Every origin/observer pair covers start/reversal/stop; genuine focus-stop is separate. Compare correct stopped actor/session/revision against host within 50 subunits by 500ms. Equality passes; any overrun fails without rounding.

## Multi-Source Coverage Audit

| Source | ID | Feature/constraint | Plan | Status |
|---|---|---|---|---|
| GOAL | Phase 3 | Host+three independently controlled synchronized authoritative walkers | 01–04 | COVERED |
| REQ | LAN-01 | Host/join/discovery/capacity/lifecycle | 01/02/03/04 | COVERED |
| REQ | LAN-02 | Independent walking/cameras/timing/focus | 01/02/03/04 | COVERED |
| REQ | LAN-03 | Common authority and at-most-once intent | 01/02/04 | COVERED |
| REQ | LAN-04 | Stable profiles and three replaceable services | 01/03/04 | COVERED |
| CONTEXT | D-01 | No routine interview | 01–04 | COVERED |
| CONTEXT | D-02 | Agent defaults are not new creator preferences | 01/03 | COVERED |
| CONTEXT | D-03 | English user-facing copy | 01/03/04 | COVERED |
| CONTEXT | D-04 | Carry Phase 2 presentation | 01/03/04 | COVERED |
| RESEARCH | R-01 | Five projects, no packages, exact linked tests/rules/permissions | 01/03/04 | COVERED |
| RESEARCH | R-02 | Raw reliable ENet, trusted sender, bounded drain/idle probe | 01/02 | COVERED |
| RESEARCH | R-03 | Detached full snapshot, revision/readiness ack, moving join | 01/02 | COVERED |
| RESEARCH | R-04 | Pending-inclusive capacity, fifth reason, stable bindings | 02 | COVERED |
| RESEARCH | R-05 | Controlled time, generation/liveness fence and exact thresholds | 02 | COVERED |
| RESEARCH | R-06 | Actual multicast fanout/expiry/source endpoint, stable selection | 03 | COVERED |
| RESEARCH | R-07 | Atomic owned profiles, exact Unicode identity and real restarts | 03 | COVERED |
| RESEARCH | R-08 | Independent contracts, actual per-provider substitutions | 01/03 | COVERED |
| RESEARCH | R-09 | Native focus/copy/actor markers/roster/local camera | 03/04 | COVERED |
| RESEARCH | R-10 | Exact per-peer evidence, actual child-failure controls | 04 | COVERED |
| RESEARCH | R-11 | Persistent owned input driver, provenance, conservative rendered timing | 04 | COVERED |
| RESEARCH | R-12 | Current local policy, saved selection/groups, full final repair pass | 04 | COVERED |
| RESEARCH | R-13 | L1 security, hostile inputs/paths, high-block findings | 01–04 | COVERED |
| RESEARCH | R-14 | Self-contained local increment and honest environment/release scope | 03/04 | COVERED |

No Deferred Ideas were added. Campaign/save/reconnect/succession, broad Phase 5 CI/coverage/reuse, Phase 10 physical mixed-OS/release/hardware/endurance and Phase 11 scheduling retain their existing owners, outside this denominator.

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|---|---|---|---|
| Actual readability, focus/Unicode/overflow/overlap, markers/roster/local cameras and timed frames | LAN-01/02/04 | State cannot establish appearance | Inspect actual required PNGs/frames per role against UI-SPEC; record image hashes and outcomes in 03-VISUAL-REVIEW.md. Agent-operated inspection is allowed. |
| Native input or multicast feasibility limitation | LAN-01/02 | Requires actual local environment observation | Preserve logs and exact unmet condition; report blocker, never a synthetic or manual-path substitute pass. |

## Validation Sign-Off

- [ ] Every task has automated checks and immediate fails_when; no three tasks without automated feedback.
- [ ] Wave 0 creates every new command/test before consumption; no watch-mode or empty execution passes.
- [ ] 18/18 AC and 20/20 edges have executable catalog/scenario mappings.
- [ ] Entire phase reviewed/fixed before first window; selected/omitted IDs and groups saved.
- [ ] Input provenance, conservative timing and exact peer/image coverage validated.
- [ ] Entire final selected set passes after repairs with actual image review and immutable historical failures.
- [ ] Set nyquist_compliant/wave_0_complete only after actual implementation/checks justify them.

**Approval:** pending execution and independent plan verification.
