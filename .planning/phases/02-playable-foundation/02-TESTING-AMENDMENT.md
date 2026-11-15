# Phase 2 Testing Amendment

Creator instruction on 2026-11-15 supersedes the older testing clauses in 02-SPEC, 02-CONTEXT, 02-UI-SPEC, 02-VALIDATION and plans 02-01–03. Canonical policy: [testing before Phase 5](../../../docs/workflow/pre-phase5-testing.md).

- Finish all Phase 2 implementation without opening a real game window. Non-rendered builds/tests remain allowed. Earlier per-task rendered tracer gates no longer apply.
- Review the complete phase code, then fix findings before rendered execution.
- Execute only on Windows, the development OS; do not run a VM. Rendered acceptance uses only 1280×720.
- Retain all gameplay scenarios, production input, scene identity/lifecycle, movement, bounds/focus, evidence integrity and failure controls. Share one game process where practical, use ordinary menu reset, and test Quit last.
- Log scenario errors and continue independent scenarios. Batch fixes, then rerun failed or unexecuted scenarios only. Combine their results with original passes by explicit report references; no full-group repetition solely for changed fingerprints.
- Linux/other-OS and 1920×1080 acceptance deferred here remain Phase 5 work, with no pass credit. Preserve actual earlier captures/results with their original scope; do not relabel them as new execution.

The 2026-11-15 pause is revoked by the creator's explicit instruction to continue under these rules. Plan 02-01 is complete, 02-02 Task 1 is committed, and Task 2 implementation is preserved. Finish the runner and the Phase 2 graph-fixture regression before code review. Phase and PR stay incomplete/draft until the amended completion scope passes.
