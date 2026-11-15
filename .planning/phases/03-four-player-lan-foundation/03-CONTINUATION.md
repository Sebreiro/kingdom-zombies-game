# Phase 3 continuation

- Invocation: `$gsd-autonomous --only 3`; stop after Phase 3, without milestone lifecycle or Phase 4 execution.
- Branch: `feature/03-four-player-lan-foundation`, from merged Phase 2 `origin/dev` at `87e0b44`.
- Pull request: [PR #4](https://github.com/Sebreiro/kingdom-zombies-game/pull/4), draft targeting `dev`; reuse it, mark ready only after completion, leave merging to the creator.
- Plans 03-01 through 03-03 are complete. Plan 03-04 implementation/tooling is complete; Task 3 actual desktop/evidence execution remains.
- Whole-phase independent code review is clean; all fifteen findings have committed fixes and independent resolution. Read `03-REVIEW.md` and cumulative `03-REVIEW-FIX.md`.
- Reviewed source/build pin: `d2d5a6106990aaf16b0b02a5f296fc7b11aa0f3e`. Preserve `SourceRevisionId` at this value and set `CustomAfterMicrosoftCommonTargets` to the absolute `tools/phase3/reviewed-source.targets` path through review/gate metadata commits, all strict/inherited builds, the full run and closure validation. Actual execution `revision` still records true HEAD.
- Historical pre-integration non-window proof: 20/20 `.phase3/runs/win32-x64/1be5faf3-4796-4650-8cfa-6316daecff8d/summary.json`; eight affected policies, Phase 2 core 4/4 and runner 3/3 and strict zero-warning build pass in `.phase3/iter3-final-1794770502320/summary.json`. These supply no rendered/input/timing pass.
- Saved full selection: 433 active affected Phase 1 cases + all 16 Phase 2 cases + all 32 Phase 3 cases; 243 active Phase 1 omissions and eight future planned declarations retain explicit status.
- Current policy: Windows development OS only, 1280x720, no VM. Group compatible scenarios; preserve failures, batch repairs, rerun failed/blocked/newly affected checks, then the complete selected set after repairs.
- Historical Phase 1 follow-ups, hosted/Linux, physical mixed-computer LAN, complete campaign, hardware and endurance obligations keep their separate evidence owners.

## Next action

The clean review/fix metadata and refreshed source-bound `03-REVIEW.json` gate are ready for commitment. Then follow `03-04-CHECKPOINT.md` and `docs/workflow/lan-foundation-checks.md`. The exact closure and SourceLink integrations have three passing real GitHub/SDK controls, a zero-warning/error strict build and a passing actual `runner.integrity` case at the reviewed source. The rejected e25 metadata rebuild is retained in `.phase3/task3-prelaunch-mismatch.json`; no full run/window was opened. Retain the exact build pin, strictly rebuild after the metadata commit, then execute `node tools/phase3/check.cjs --all`. First establish actual foreground/native event and background-hold feasibility; collect the grouped selected scenarios, 48 rendered timing rows, 16 stopped comparisons and every required image. Inspect actual images, author/validate `03-EVIDENCE.json`, then write 03-04-SUMMARY and phase verification/transition. No merge.

## First selected run and repair batch

Original full invocation `28bb1d55-5831-4510-b590-9f140e1689b2` remains failed: 464/481 passed, five inherited fixture/matrix failures and twelve Phase 3 desktop driver failures. All sixteen Phase 2 cases, including nine actual desktop cases, passed. Batch `dc523e9` repairs fixture ordering, transitive positive setup, R-37 documentation and DPI/focus handling; all five inherited failed cases now pass focused checks. CR-10 was then found in inherited subject-reference resolution and fixed by `7b140a2` after real-layout RED `8466836`. The reader now validates all 428 retained passing plus five focused passing records, preserving original failures. Strict build and actual `runner.integrity` pass at the new source/build pin. Independent review is clean with fifteen findings resolved.

Next: commit renewed review/gate, rebuild with both pinned properties, run the twelve failed native cases (`--group rendered`) first, then run the required fresh complete 481 selected cases only after focused failures are repaired. Preserve the first failed report and all later focused reports in final history; no image review, timing or final completion is claimed yet.

## Native retry repair batch 2

Focused `72f2d327-0ff1-40bf-83fd-72fbf90a4ab9` remains failed, 0/12 accepted. It verifies physical 1280x720 DPI coordinates, but long unpaced text overflowed the Godot key buffer, empty text did not delete selected content, and a covered challenger could not regain focus/normal Quit; an unretained timing overrun also failed. Source `af8d4f2` paces physical Unicode input with explicit UTF8, physically clears and observes empty fields, raises only owned windows, creates the hidden focus sink before readiness and retains raw timing rows before unchanged rejection. Follow-up `d2d5a6` checks foreground ownership before each paced character. Strict build passes at `d2d5a6106990aaf16b0b02a5f296fc7b11aa0f3e`; independent review remains clean with all fifteen findings resolved. Next run the twelve failed native cases from this focused report, then only after they pass run the entire fresh 481 selection and actual image review. Preserve all prior failures.
