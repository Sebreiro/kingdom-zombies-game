---
phase: "01"
plan: "17"
status: awaiting_restart
phase_complete: false
candidate: 59f032cf23340058eec91f12c5d240f585dfabdc
windows_cases_accepted: 676
linux_current_scope_executed: false
human_review: Pending
---

# Phase 1 — Windows restart checkpoint

The complete current Windows Phase 1 selection passed. The creator enabled Windows Hypervisor Platform, but the final live host query still returned `HypervisorPresent: false`. A creator-controlled Windows restart is the next environment prerequisite. The owned Linux guest is cleanly shut down and its disk, tools, caches and same-source checkout are preserved. No host restart was initiated by an agent.

## Completed evidence

- Final candidate: `59f032cf23340058eec91f12c5d240f585dfabdc`.
- Complete Windows run: **676/676 accepted**, actual exit 0, zero failed/blocked/missing/skipped, eight separately deferred Phase 5 declarations.
- Invocation: `b8b5eacc-c46a-42d7-a898-01ad33c1b0a8`.
- Input: `fe2bf549f20329633fd62cfeab796ca8fb2c5ce02da432346e8ee4c314873426` over 203 files.
- Windows tool: `a23d7bacf589ac13e65eda9e329b0218ebef3a34ba90ad2630f1607349e6aab1`; platform `win32-x64-10.0.19044`.
- Canonical report: `.phase1/temp/17-windows-59f032c/.phase1/runs/b8b5eacc-c46a-42d7-a898-01ad33c1b0a8/summary.json`, SHA-256 `042c7e2b280940860c7e1304ee85bcfb756d4ae58c2d192b51c713dbbd37aa4a`.
- All 676 test reports, 692 subject-evidence files and 681 canonical commands/log hashes were inspected. Clean native bootstrap, locked five-project restore and strict zero-warning/error solution build passed.
- Four review repairs are committed and independently clean; all 31 declared ASVS1 mitigations are closed. These results supply no human judgment.

See the [partial plan summary](01-17-SUMMARY.md), [platform evidence](../../../docs/workflow/platform-proof.md), [clean review](01-REVIEW.md) and [security record](01-SECURITY.md).

## Preserved local VM

The final Linux checkout is `/home/phase1/phase1-final-17`, clean at the identical candidate and input. Its retained bundle `/home/phase1/phase1-tools/downloads/final-17.bundle` hashes to SHA-256 `334110bde21f2a983deb965d0d227e0872ed659309c8d170e3fd10e07f35f54b`. All 41 declared package payloads/lock identities, both Godot archives and retained tool archives were verified. No Linux restore/build/bootstrap or acceptance case has run on this candidate.

Owned shutdown at 2026-11-14T18:43:22Z was verified at 18:43:33Z and reconfirmed at closeout. The identified QEMU PID 21752 and helper PID 23576 exited, SSH/QMP listeners disappeared and the preserved disk allowed exclusive read access. `.phase1/tools/linux-vm/processes.json` retains historical identities with live PIDs cleared and `shutdownConfirmed: true`. No unrelated process was terminated.

## Resume Plan 01-17 only

1. After the creator restarts Windows, verify the live host hypervisor is active. Do not infer activation solely from the Windows Features checkbox.
2. Start only the existing owned VM with `.phase1/tools/linux-vm/start-vm.cjs`, hidden. It defaults to WHPX; verify actual accelerator startup/logs and genuine Linux access. Do not silently fall back to TCG or change proof deadlines. If CPU/startup compatibility needs a concrete runtime correction, preserve the observed failure and adjust only the owned VM configuration.
3. Use the existing full Windows OpenSSH path, runtime key and known-host file recorded in [the environment](01-ENVIRONMENT.md). Source `/home/phase1/phase1-tools/env.sh`; use `/home/phase1/phase1-final-17` and its checkout-local CLI/feed/package/cache/temp paths. Confirm clean candidate/input and absence of old build outputs before setup.
4. Execute actual clean locked restore, strict whole-solution build, native bootstrap and the complete `node tools/phase1/check.cjs --phase 1` selection on Linux. All **676** selected obligations must pass with real reports and subject evidence. Keep the eight Phase 5 declarations separately pending with no pass/skip credit.
5. Preserve original logs/reports. A real source/contract repair requires affected tests and renewed complete evidence invalidated by that change. Metadata-only updates receive focused documentation/consistency checks with distinct identities.
6. Complete the required goal-verification lifecycle and the actual fourteen-component [human review](01-HUMAN-CHECK.md). Only then may Plan 17, Phase 1 and RULE-01–RULE-07 be completed. Stop before Phase 2 under `--only 1`.

## Workflow state

Shared progress remains **17/18 plans**, zero completed milestone phases. Plan 17's summary is deliberately halted and neither task is fully accepted: Task 1 still needs the complete Linux run and Task 2 needs real human judgment. The plan-index helper may return no incomplete plans merely because a halted summary exists; that is not acceptance evidence. Resume this checkpoint rather than treating all 18 plans as complete or repeating the finished Windows run without a new invalidating change.

The historical f4658bb 670-case Windows pass and sixteen-case Linux results retain their original meanings. The new creator rule requires the complete current phase scope on the development OS and a local VM using the other OS. The eight Phase 5 coverage/aggregate identities and unchanged 90%/80% targets retain later ownership. Historical WINDOWS item 1 remains open; WINDOWS 28 awaits complete current Linux acceptance. Preserve captured Phase 2/3 context and unrelated Phase 4 discussion files.

## Post-state documentation check

After checkpoint commit b85a518, the existing actual repository-navigation control `--case ac-10.complete` passed 1/1 selected, actual exit 0, with no failure. Planning consistency independently retained 133 active requirement IDs, all seven RULE checkboxes Pending, 17/18 accepted plans and zero completed milestone phases; captured Phase 4 context is preserved. This is metadata validation, not another complete platform run.

```json
{
  "revision": "b85a5184f6d7037a8356ed7459fcb1ebdfa18b2e",
  "nonce": "5ee9d083-b818-4600-a7b3-4227112af20b",
  "input": "3cc36df574b4ebe667ec4c9c35f9dde8e75ae3d3e9333f1abac9eb2ef47d02b8",
  "tool": "ee1af6264013db4c3439d7970e12f6fdda9a9792c00df06d76cfb5ca53be3158",
  "platform": "win32-x64-10.0.19044",
  "summarySha256": "7b01b028589ff871035b4e356efe63f1290c64b230d111b9cab2f0499a527758"
}
```
