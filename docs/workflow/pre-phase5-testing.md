# Testing before Phase 5

The temporary Phases 2–4 policy was superseded by the creator's later 2026-11-15 [local testing strategy](local-testing.md), which now applies to routine local verification in every phase. This path remains a compatibility entry point for existing plans and references.

Use development-OS-only testing without a VM, finish the whole implementation and code review/fixes before game-window tests, use only 1280×720, batch scenarios into shared game runs, collect failures, repair together, check failures, and then rerun the entire selected set after repairs. Reduced selection starts immediately: all active Critical cases plus every new/changed case and affected regression, regardless of importance. Every phase saves reusable scenarios with importance and ordered run groups.

Phase 5 implements the framework and the basic hosted Windows/Linux pull-request checks, without reinstating local VM or extra-resolution catch-up. Phase 11 adds nightly full regression testing and, under TEST-12, the full hosted Windows/Linux runs and the certification moved from Phase 5. Historical reports keep their original identities and scope; this amendment supplies no new execution evidence. Read the canonical strategy for the complete rules and the separate release obligations.
