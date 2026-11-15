# LAN foundation checks

The [Phase 3 scenarios](../../.planning/phases/03-four-player-lan-foundation/03-SCENARIOS.md), [saved selection](../../.planning/phases/03-four-player-lan-foundation/03-SELECTION.json) and [independent catalog](../../tools/phase3/required-cases.json) define exact obligations. Follow the [local testing strategy](local-testing.md): implement and independently review/fix the whole phase before any game window. Routine local runs use Windows at 1280×720 without a VM. Selection includes every new/changed/affected case, all 16 inherited Phase 2 cases and the explicitly enumerated active Phase 1 integrity cases. Omitted source IDs have individual reasons and earn no execution credit.

| Need | Command |
| --- | --- |
| Validate/list exact required cases and saved coverage | `node tools/phase3/check.cjs --list` |
| Focused no-window proof | `node tools/phase3/check.cjs --case runner.tracer,runner.timing` |
| Pure authority or exact permission proofs | `node tools/phase3/check.cjs --group core` |
| Real socket/file provider checks | `node tools/phase3/check.cjs --group adapters` |
| Ordinary main-scene headless native subjects | `node tools/phase3/check.cjs --group network` |
| Malformed evidence, child failure and ownership controls | `node tools/phase3/check.cjs --group runner` |
| Reviewed grouped Windows native scenarios | `node tools/phase3/check.cjs --group rendered` |
| Full fresh saved selection, including inherited checks | `node tools/phase3/check.cjs --all` |
| Fresh focused repair preserving original report | `node tools/phase3/check.cjs --failed .phase3/runs/PLATFORM/NONCE/summary.json` |
| Verify exact final completion and actual image review | `node tools/phase3/check.cjs --verify-completion .planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` |

Build first using the isolated environment from [setup](setup.md):

```sh
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
```

`--case` accepts exact comma-separated IDs and rejects zero, unknown or duplicate IDs. Groups are core, adapters, network, runner and rendered. Selectors cannot be combined. No-window groups never launch a game window. A rendered/full command fails closed before launch unless `.planning/phases/03-four-player-lan-foundation/03-REVIEW.json` records complete independent whole-phase review, the current source/tool/revision fingerprints, a named reviewer, resolved findings and the hash of its actual Markdown review. The runner also validates and hashes the saved selection before launch. Task 3 authors the actual review record after review/fixes; implementation does not preapprove itself.

The small phase-owned runner is not Phase 5's general catalog/reuse framework. G0 runs build/pure/file/socket/real-headless/runner checks. G1 runs all nine inherited rendered scenarios in one solo process with ordinary resets and Quit last. G2 reuses real host/browser processes; profile restart needs a different PID with the same owned profile root, and extra equally named hosts provide browser overflow. G3 uses host plus three guests, adding only capacity/race/identity challengers and a replacement after deliberate guest loss. G4 unexpectedly terminates the owned host last, reusing the intact G3 set when available. Every extra launch has a recorded reason. A focused G4 execution needs its own four-peer setup.

Each invocation creates exclusive immutable `.phase3/runs/<platform>/<nonce>/` outputs: input snapshot, selected result records, actual process/launch logs, native-input acknowledgements, read-only process/draw streams and PNG/state checkpoints. Case-plus-peer membership uses exact sets. A duplicate guest cannot replace an absent fourth peer; a headless result cannot satisfy rendered or physical-input obligations. Required PNGs must decode as nonempty 1280×720 images with valid chunk CRCs, immutable image/state hashes and matching process/profile/session/actor/binding/revision/draw identity. G4 deliberately binds the terminated host's pre-loss image and each guest's post-loss menu image; no post-loss image is claimed for a dead process.

The persistent Windows helper pins actual PIDs, process start identities and native window handles. `physical-SendInput` means scan-code events delivered through the foreground OS input stream; it is distinct from targeted `window-message` delivery and `synthetic-native-ParseInputEvent`. A helper delivery acknowledgement alone never proves that Godot accepted the event: native callbacks, held polling and authoritative changes are separately observed. Background messages must demonstrate sustained holds before use. An explicit synthetic fallback is allowed only for simultaneous-control/moving-join diagnostics, without physical-input credit. Native focus clearing and canonical authority stay active.

Timing uses one external `process.hrtime.bigint()` coordinator clock. The conservative interval starts before delivery and ends after completed-draw PNG acknowledgement, including all polling/scheduling/capture delays and a 1 ms clock allowance. No startup-relative peer clocks are subtracted. Exact 250 ms and 50-subunit/500 ms equality passes; the smallest represented overrun fails without rounding. Every required origin/observer start, reversal, stop and real focus-stop needs a visible in-camera actor and bound image/transform evidence. Actual driver feasibility and these rendered limits remain unproved until Task 3 executes them after review.

The shared `.phase2/desktop.lock` coordinates inherited and new native runs. Its release verifies the recorded owner and file identity; changed/foreign locks remain intact. Profiles, ports and output paths are invocation-owned, bounded waits stop failing subjects, and cleanup targets only owned children/descendants. Never kill Godot globally. Actual nonzero, crash and timeout controls execute real children; counterfeit malformed records prove rejection only and cannot supply successful production execution.

Completion requires one fresh entire-selected-set final report on the repaired source, original/focused history references, current source/tool/OS/revision and the saved selection/review hashes. `.planning/phases/03-four-player-lan-foundation/03-EVIDENCE.json` references that final report, immutable history, selection, actual visual-review artifact and per-image review rows containing exact case, role, checkpoint, image hash, report hash, reviewer, verdict and notes. Inherited results use their original qualified phase/source IDs and real report/process references. Actual readability, Unicode/focus/overflow, actor markers/roster and camera appearance require inspection; decoded pixels or state cannot replace judgment. Any selected failure, missing peer/image, stale identity, unreviewed image or unresolved later failure blocks acceptance.

Preserve failed reports. Continue independent scenarios while usable, batch repairs, run failed/blocked/newly affected checks, then rerun the full selected set on the repaired build. An initially clean full selected run already supplies that pass. These local results give no hosted/Linux, physical mixed-computer LAN, complete A-38/A-45 campaign, packaging, reference hardware or endurance certification; those obligations retain their separate owners.
