# Phase 5 focused repair run

The first focused repair selected the 37 failed and blocked rows from the [initial Windows run](05-INITIAL-RUN.md). It tested clean commit `4b50c81d02424b1322b06585253e5fff5476d930` on Windows x64.

- Command. `node src/godot-game/tools/game-tests/check.cjs --failed .local/runs/game-tests/78becb29-763d-4cf8-93cc-a9abff498b79/summary.json --review .local/temp/phase05-repair-review.json`
- Shared invocation. `b7ed2a51-d6c3-4a4d-aa7f-92c786e9f20d`. Exit 1 with 34 passes and 3 failed catalog rows. Original reports stay under `.local/runs/`.
- Rendered solo passed 9/9 at 1280×720 in one session. The suite invocation `b0f2464f-d9f3-4e6a-924b-dc5699ec1ef6` took 225.766s. Rendered LAN passed 12/12 under `c4dc4220-385f-47ae-83ab-3932f0b16ec7`. All seven selected LAN headless cases passed under `1ca76a2c-d52c-4291-bd9e-d1ce4f4f1c54`.
- The isolated pin proofs passed 2/2 under `946e1947-3a08-4a50-87ea-c3b1cdf14683`. The targeted rules group passed 17/22. Repository authentication exceeded three inherited 45-second stage limits. Both companion checks reported no recognised current producer. The foundation permission case reached the live-producer stage and failed on the same source identity.
- The fixed audio lifecycle, held-out audio and world-map source cases passed. The hub launched separately and stopped before capturing its first category when the input acknowledgement failed. On this desktop the 300% DPI transform made the legacy driver report 426×240 for the physical 1280×720 window. The hub still needs its category captures and P1–P4 review.
- Basic Windows and Ubuntu checks passed on this same revision. [Hosted run](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45290688042)

The next focused attempt uses per-monitor DPI awareness only for the inspection hub's existing owned input driver. Reproduce with the exact hub case and a clean review of the new source. The updated selection distinguishes an exact change to the live FoundationRoot asset stamp from a general content helper change. It retains the actual normalization, publication, and live fixture consumers.
