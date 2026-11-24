---
phase: 05-ci-and-repeatable-game-testing
plan: "04"
subsystem: phase-verification
requirements-completed: [TEST-01, TEST-02, TEST-03, TEST-04, TEST-05, TEST-06, TEST-07, TEST-08, TEST-09, TEST-10]
---

# Plan 05-04 summary

Independent code review preceded the Windows runs. The first reduced run and the next complete attempt preserve the actual failures, exact source and evidence. The final repairs passed the inherited source, native, selector and teardown checks in focused runs. The shared failure replay passed all five remaining rows, including the one failing member inside the legacy runner group.

Fresh local invocation `62a015a9-6df3-4a66-9a16-039aa7430de0` verified the selected set on clean `b7e419c075bd0c97423142684099c9f5d588f0c9` at 1280×720. The owned hub completed all 15 native checkpoints. Codex accepted P1–P4 after inspecting the fresh images and recorded the exact image identities and readability findings. The source’s basic jobs passed on actual Windows and Ubuntu. [Hosted checks](https://github.com/Sebreiro/kingdom-zombies-game/actions/runs/45331138330)

The measured local pure execution sets a five-second target after preparation. The two-case Critical benchmark used one solo process and one actual host-plus-three-guests group. Its 16.040s rendered time sets a two-minute routine Critical budget. The phase verification record gives setup, grouped and end-to-end timings, original run identities, failure and repair outcomes, and reproduction commands.

See [Phase 5 verification](05-VERIFICATION.md) and [pull request #15](https://github.com/Sebreiro/kingdom-zombies-game/pull/15).
