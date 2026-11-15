# Phase 2 deferred items

## Accepted minor presentation follow-ups

The creator accepted minor gaps on 2026-11-15 and requested phase closure. The advisory UI review found no blockers: preserve Start's accent across hover/pressed states, reconcile the agent-authored outline thickness, and define explicit typography leading in later presentation work. These are recorded follow-ups, not reasons for another game launch or an incomplete Phase 2.

**Resolved 2026-11-15:** The graph fixture was repaired in `7cd3b2b`, retaining production consumers and real forbidden-edge/type proofs. Focused edge/type controls passed, followed by the actual 50/50 GraphTests suite with zero failures/skips (`.phase2/implementation-only-20261115/graph-all.json`). The historical failure below remains preserved. Other-OS testing is not a current local completion gate under the creator's later testing policy; no missing historical execution is relabeled as passed. No unresolved Phase 2 graph item remains.

- Plan 02-02 additional Phase 1 `graph.allowed-and-direct` run `90de883f-0b74-433a-b0aa-2838aa4e9d2e` failed in its mutated positive-control checkout: removing Godot's Core edge leaves the Plan 02-01 FoundationRoot/FoundationView source referring to FoundationSession/FoundationActorState, producing CS0246. The original report and subject log remain under `.phase1/runs/90de883f-0b74-433a-b0aa-2838aa4e9d2e/`. This regression was introduced by Phase 2 and remains required before Phase 2 closure. The parent assigned its positive/forbidden fixture repair and affected both-OS proofs to Plan 02-03; no graph proof pass or repair is claimed by Plan 02-02, and no later-phase or historical closure exception applies.
