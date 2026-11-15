# Testing before Phase 5

Creator-approved on 2026-11-15. This temporary policy governs Phases 2–4, including the resumed Phase 2 run. It supersedes conflicting platform, resolution, per-task rendered gates and rerun instructions in existing phase specifications, UI contracts and plans. Required gameplay behavior remains unchanged.

1. Execute tests only on the development operating system. Do not run a VM. Linux/other-OS and additional-resolution acceptance deferred by this policy remains explicit Phase 5 follow-up, without pass credit.
2. Implement the whole phase before launching a real game window. Builds, analyzers, pure tests, repository checks and other checks without a real game window may run during implementation. Do not use rendered tracer gates between tasks or plans.
3. After implementation, review the phase's code and fix the findings before rendered testing.
4. Run rendered scenarios at 1280×720 only. Exercise as many scenarios as practical in one game process, resetting through ordinary menu controls between scenarios. Run Quit last. A necessary fresh-process scenario must have a recorded reason; do not launch a process per scenario by default.
5. Collect and log scenario errors, continue independent scenarios when the process remains usable, then batch fixes. A crash or unusable process may block remaining scenarios; it must not count as a pass.
6. After repairs, rerun only failed or previously unexecuted scenarios. When those pass, consider the selected scope tested together with the earlier passing scenarios. Do not repeat the full rendered group solely because a repair changed a source fingerprint.
7. Preserve original reports, image identities, revisions, failures and logs. A completion record links the first pass and each focused repair run, showing which scenarios passed in each. This explicitly authorized temporary approach does not claim Phase 5's dependency-aware reuse certification.

The phase branch and draft-to-ready PR rules remain in force. Phase completion still needs delivered behavior, code review/fixes, and every currently selected scenario accounted for under this policy. Phase 5 owns the comprehensive framework and the deferred platform/resolution obligations; nothing here creates a Linux or 1920×1080 pass.
