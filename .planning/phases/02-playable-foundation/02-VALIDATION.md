---
phase: "02"
slug: "playable-foundation"
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-14"
---

# Phase 02 — Validation Strategy

> Per-phase validation contract. Proposed commands below must be implemented in Wave 0; no Phase 2 execution or pass is claimed.

## Test Infrastructure

| Property | Value |
|---|---|
| Framework | Existing locked C# xUnit executable/Shouldly, strict analyzer suite, new small Node Phase 2 coordinator |
| Config file | Existing project/central package configuration; new independent Phase 2 case catalog required |
| Quick run command | Proposed: `node tools/phase2/check.cjs --group core` |
| Runner controls | Proposed: `node tools/phase2/check.cjs --group runner` |
| Rendered checks | Proposed: `node tools/phase2/check.cjs --group rendered` |
| Full suite command | Proposed: `node tools/phase2/check.cjs --all` |
| Existing affected baseline | Strict solution build plus affected Phase 1 graph/engine/rules/docs cases |
| Estimated runtime | Measure after implementation; focused warm Core target <30 s; native/VM suites may be slower |

## Sampling Rate

- **After every task commit:** Run relevant focused automated command and strict build for C# changes.
- **After every plan wave:** Run implemented Phase 2 groups plus affected boundary/rule/docs checks; do not call missing groups passing.
- **Before `$gsd-verify-work`:** Complete Phase 2 scope on Windows and local Linux VM, actual saved-image review, exact current evidence, no missing obligations.
- **Max feedback latency:** Target <30 seconds for pure/runner focused feedback; measure and document slower native/TCG groups rather than relax correctness or silently skip.

## Per-Task Verification Map

Task IDs align to the final three plans. New Phase 2 interfaces are created within 02-01's tracer before their first verification; no execution or pass is claimed by this map.

| Task ID | Plan | Wave | Requirement | Threat Ref | Secure Behavior | Test Type | Automated Command | File Exists | Status |
|---|---|---|---|---|---|---|---|---|---|
| 02-01-01 | 01 | 1 | BASE-01, BASE-02, BASE-03 | T-02-01, T-02-04, T-02-05 | Production identified command, exact native permissions and owned OS input | tracer/build | `node tools/phase2/check.cjs --case rendered.tracer`; strict solution build; `node tools/phase1/check.cjs --case engine.source-policy` | ❌ Created in tracer | ⬜ pending |
| 02-01-02 | 01 | 1 | BASE-01, BASE-02 | T-02-04 | Reject adjacent/forged native, generated and public API permission | contracts | `node tools/phase2/check.cjs --case core.permissions`; existing exact origin/boundary cases | ❌ New controls | ⬜ pending |
| 02-02-01 | 02 | 2 | BASE-02 | T-02-06 | Validate identities/time/direction and normalize production input | unit/rendered | `node tools/phase2/check.cjs --case core.authority`; `--case core.input`; `--case rendered.distance` | ❌ Planned | ⬜ pending |
| 02-02-02 | 02 | 2 | BASE-01, BASE-02 | T-02-07, T-02-08 | Focus/release barrier, lifetime fence and usable recovery | unit/rendered | `node tools/phase2/check.cjs --case core.lifecycle`; `--group rendered`; strict solution build | ❌ Planned | ⬜ pending |
| 02-03-01 | 03 | 3 | BASE-03 | T-02-02, T-02-03, T-02-09 | Exact evidence, owned processes and concurrent-run refusal | contracts/tracer | `node tools/phase2/check.cjs --group runner`; `--case rendered.tracer` | ❌ Planned | ⬜ pending |
| 02-03-02 | 03 | 3 | BASE-03 | T-02-10 | Complete readable independent case/evidence documentation | docs/catalog | `node tools/phase2/check.cjs --list`; `node tools/phase1/check.cjs --case docs.navigation` | ❌ Planned | ⬜ pending |
| 02-03-03 | 03 | 3 | BASE-01, BASE-02, BASE-03 | T-02-02, T-02-10 | Actual complete Windows/Linux results and current image review | rendered/full | `node tools/phase2/check.cjs --all` on each OS; `--verify-platforms .planning/phases/02-playable-foundation/02-PLATFORM-EVIDENCE.json` | ❌ Planned | ⬜ pending |

*Status: ⬜ pending · ✅ green · ❌ red · ⚠️ flaky*

## Wave 0 Requirements

- [ ] Independent required-case manifest: each AC maps to exact distinct cases and mandatory image checkpoints.
- [ ] Existing C# test assembly extended with pure movement, session/identity, alias/release/focus-latch and command validation cases.
- [ ] New Phase 2 coordinator with exact-result reconciliation and bounded owned processes.
- [ ] Real platform input adapters: Windows native input, guest X11 xdotool; test actual focused game window identity before typing.
- [ ] Read-only observation hook and actual rendered image capture/decode checks.
- [ ] Native root/adapter permission proof, authenticated generated text/span/member/package provenance and adjacent/collision controls.
- [ ] Guest pinned tools/display/renderer/input probes; VM owner handles lifecycle, no host reboot.

## Required Acceptance Coverage

| Acceptance | Required evidence |
|---|---|
| AC-01 | Normal project launch; readable menu; actual mouse and keyboard Start/Quit activation |
| AC-02 | Menu/play images at both 1280×720 and 1920×1080; background, ground, building and one player visible |
| AC-03 | Actual A/D/arrows, both aliases, opposing directions, repeat/release; facing persists at rest |
| AC-04 | Left/right unobstructed one-second elapsed-game-time intervals, displacement inclusive 2.85–3.15 u |
| AC-05 | Ground, building traversal, both clamps/inward resumption and visible bounded camera |
| AC-06 | Stable identified actor and observed canonical command path throughout movement/turning |
| AC-07 | Held/repeated/rapid transitions, zero actors in menu, one in play, fresh spawn/session, old controls absent |
| AC-08 | Real window focus loss/regain with held key, no movement until release and fresh press |
| AC-09 | Ordinary Quit closes the actual game cleanly and no owned process remains |
| AC-10–AC-13 | Exact catalog/result/image matching, deliberate missing/duplicate/empty/stale/crash/timeout/tool/image controls, reorder invariance, fresh rerun, concurrency isolation/refusal |

Use recorded game elapsed time for speed, not host sleep duration. Save current-run images after completed draw; decoded content and declared appearance/readability review are required. Cross-build, headless native callback logs or an empty successful process are insufficient for rendered acceptance.

## Manual-Only Verifications

| Behavior | Requirement | Why Manual | Test Instructions |
|---|---|---|---|
| Appearance/readability/current image review | BASE-01, BASE-02, BASE-03 | Structural/pixel checks alone do not establish readable usable menu or clear facing | Review saved current Windows/Linux menu/play images at both sizes plus facing/bounds/restart checkpoints; record reviewer, case/image IDs, verdict and defects |

Manual review can be performed by the authorized agent reviewing actual rendered artifacts; it is not a new design interview. If any required review cannot be performed, record it as pending and keep completion blocked.

## Validation Sign-Off

- [ ] All final tasks have automated verification or explicit Wave 0 dependencies.
- [ ] Sampling continuity: no 3 consecutive tasks without automated feedback.
- [ ] Wave 0 covers all missing interfaces and test files.
- [ ] No watch-mode flags; processes have explicit deadlines and descendant cleanup.
- [ ] Pure/runner feedback latency measured; native/VM durations recorded.
- [ ] Strict analyzers/architecture/docs pass with exact provenance; no blanket exemptions.
- [ ] Every required case/environment is executed with fresh immutable evidence and images.
- [ ] Negative controls fail for intended causes after successful setup.
- [ ] Current images have recorded appearance/readability review.
- [ ] `nyquist_compliant: true` only after evidence establishes it.

**Approval:** Pending implementation and validation. Phase 1's one-time reduced-Linux closure exception does not waive Phase 2 obligations.
