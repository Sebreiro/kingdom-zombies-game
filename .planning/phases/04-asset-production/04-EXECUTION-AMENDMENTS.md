# Phase 04 execution amendments

## Preparation 01 — ignored runtime output

Before 04-01 execution, add `/.phase4/` to the existing root ignore list. The checked plans already require owned temporary invocation/evidence output there, but the inherited list covers only Phases 1–3. This one-file preparation follows the existing convention and prevents generated caches/reports from being accidentally staged. It changes no resource, acceptance criterion, planned task ownership or runtime behavior. The 33 checked plans and 66 planned tasks remain the production contract.

Owner: parent orchestrator. Modified production/configuration file: `.gitignore` only. Verification: `git check-ignore .phase4/ignore-probe.txt` must print that exact path and exit zero; existing Phase 1–3 ignored paths must still be ignored. No game window or content pass is supplied by this preparation.
