# Phase 1: Code rules and documentation — Specification

**Created:** 2026-11-13 (Europe/Paris)
**Ambiguity score:** 0.06 (gate: ≤ 0.20)
**Requirements:** 7 locked — RULE-01 through RULE-07
**Status:** Requirements specified; implementation and verification remain pending.

## Goal

Turn the starter repository into a reproducible Windows/Linux Godot C# development baseline whose documented rules accept valid engine integration and reject deliberate code, architecture, exemption, and verification failures.

## Background

The inspected game directory contains `project.godot`, its icon/import metadata, and an `.editorconfig` that specifies UTF-8. There are no C# projects, gameplay scripts, scenes, rule analyzers, architecture checks, or executable proof tests. The project settings declare engine/rendering feature tags; they do not establish a validated toolchain.

Initialization supplied project context, requirements, a roadmap, research, and a workflow page. Git currently tracks the root entry point as `Agents.md`; Windows can resolve other casing, so that alone does not prove Linux compatibility. The focused code-rule and architecture documentation still needs to be delivered.

The LangProj reference at `D:/Work/SaaS/LangProj_monorepo` supplies code-style and architecture documents, a rule matrix, scoped exemptions, custom analyzers, and positive/negative proof patterns. Its HTTP/database architecture is not this game's architecture. Its generated-code and threading rules require explicit Godot compatibility decisions.

Scope is owned by [REQUIREMENTS.md](../../REQUIREMENTS.md#development-rules-and-documentation), [ROADMAP.md](../../ROADMAP.md#phase-1-code-rules-and-documentation), and [PROJECT.md](../../PROJECT.md). [Research](../../research/SUMMARY.md) supplies candidates, not certified versions. This specification sharpens the existing seven requirements without adding another phase owner or marking any requirement complete.

## Requirements

1. **RULE-01 — Reproducible toolchain:** A developer can restore, build, and run the same pinned Godot/.NET/compiler/analyzer toolchain and representative integration proof on Windows and Linux from a clean checkout without access to the LangProj donor folder.
   - Current: Starter engine settings exist, but no reproducible C# build or compatibility evidence exists.
   - Target: A coherent, recorded version baseline, repository-owned inputs, documented commands, and actual passing build/rule/scene results from both operating systems. A missing required environment blocks completion.
   - Acceptance: AC-01–AC-04 pass, including deliberately invalid tool selections and failed/interrupted proof runs. Linux instructions or a Windows cross-build alone do not satisfy Linux execution.

2. **RULE-02 — Complete inheritance register:** Every rule in the captured LangProj code-style, code-architecture, and rule-matrix baseline has a unique traceable register entry classified as retained, adapted, or inapplicable, with a reason, explicit scope, and enforcement/proof mapping where applicable.
   - Current: LangProj source references are recorded in PROJECT.md; no local inheritance register exists.
   - Target: A captured donor revision/content identity and exhaustive source-to-local mapping. Adaptations explain the Godot need and their limits; inapplicable web/database rules remain accounted for rather than silently disappearing. Retained/adapted rules identify their local documentation, mechanisms, exemptions, and proofs.
   - Acceptance: AC-05–AC-06 reject missing, duplicate, unclassified, or untraceable entries and accept a complete register independent of row order.

3. **RULE-03 — Proven enforcement:** Documented checks accept compliant code and reject code, architecture, and exemption violations with actionable diagnostics, positive and negative proofs, and no undocumented or blanket suppression.
   - Current: There is no compiler/analyzer enforcement configuration, exemption register, or proof suite.
   - Target: Applicable machine-checkable rules fail the build or a required local check. Each enforcing mechanism has compliant and deliberately violating examples. Qualitative review obligations are explicitly labeled and never represented as automated passes. Rule text, register, configuration, and proofs change together.
   - Acceptance: AC-03–AC-09 demonstrate actual failures from bad inputs and passes from clean controls, including real-build evidence that enforcement is wired into the selected toolchain.

4. **RULE-04 — Focused documentation:** A short root AGENTS.md leads developers and agents to focused documentation indexes whose links, anchors, and filename casing are validated consistently on Windows and Linux.
   - Current: The tracked `Agents.md` is a bootstrap index and `docs/workflow/gsd.md` describes workflow; focused implementation-rule documentation does not exist.
   - Target: One canonically cased root `AGENTS.md` containing the project description and task-to-document index; focused code-style, architecture, rule-matrix, proof-running, and decision documentation. Each created project documents its purpose, boundaries, and allowed dependencies. Readers can load the documents relevant to their task.
   - Acceptance: AC-10–AC-11 pass on both systems and reject broken targets/anchors, missing required documents, ambiguous references, and case collisions.

5. **RULE-05 — Enforced architecture boundaries:** Documented and executable dependency checks separate gameplay rules, Godot integration, and multiplayer services, accepting allowed dependencies and rejecting forbidden or indirect leaks.
   - Current: No solution/project graph or boundary checks exist.
   - Target: A minimal compilable structure with explicit dependency ownership and allowed directions. Gameplay state/rules are independent of Godot objects and concrete discovery/identity/connection services. Engine and service integration connect through the documented boundary. Small fixtures prove the rules without implementing LAN gameplay.
   - Acceptance: AC-12–AC-13 accept the permitted graph and reject direct, transitive, and type-level violations; missing expected subjects cannot make a check pass vacuously.

6. **RULE-06 — Godot lifecycle compatibility:** A representative Godot scene demonstrates construction, scene/resource loading, callbacks, handwritten/generated partials, exports, signals, nullable initialization, main-thread access, and teardown under the enforced rules on Windows and Linux.
   - Current: No runnable C# integration scene or engine-lifecycle proof exists.
   - Target: A small actual engine fixture exercises each listed integration surface under the selected rules. Genuine generated output receives only justified, bounded treatment; handwritten partials remain checked. Lifecycle failures have explicit outcomes.
   - Acceptance: AC-14–AC-17 pass on both systems, including repeated load/unload, missing required exported values, late work after teardown, and prohibited worker-thread engine access.

7. **RULE-07 — Executable state contracts:** Explicit rules and small executable examples prove controlled time and randomness, stable identities preserved through serialization, and saveable authoritative state independent of Godot objects, with engine access confined to the main thread.
   - Current: No time/randomness contracts, stable identity model, serializable state example, or corresponding proofs exist.
   - Target: Minimal examples demonstrate repeatable inputs, valid numeric limits, identity/value preservation, meaningful ordering, and coherent restoration. Invalid or interrupted restoration cannot partially replace live example state. These are architecture proofs, not the full gameplay, networking, or save system.
   - Acceptance: AC-17–AC-22 pass with valid, empty, singleton, multiple-record, invalid, duplicate, reordered, repeated, and interrupted examples.

## Boundaries

**In scope:**

- The reproducible toolchain/build baseline and small proof-running entry points, with executed Windows and Linux evidence.
- The complete LangProj inheritance inventory, local rule matrix, enforcement configuration, scoped compatibility decisions, and positive/negative proofs.
- Minimal compilable gameplay/Godot/service boundaries and executable dependency checks.
- The real Godot integration scene and small time/randomness/identity/serialization examples needed to prove these rules.
- Focused documentation, canonical `AGENTS.md` casing, checked local navigation, and documentation for each created project and architecture decision.
- Required-case/result checks sufficient to trust Phase 1's own proofs, including deliberate failure of the verification path.

**Out of scope:**

- Player-facing menu, movement, production input, camera, or playable foundation — Phase 2 owns these; the integration scene is a proof fixture.
- LAN discovery/joining, host-plus-three-player gameplay, session recovery, and platform-service implementations — Phases 3 and 9 own the relevant behavior; Phase 1 establishes boundaries only.
- Asset production and the complete content pack — Phase 4 owns these; Phase 1 needs only minimal fixture resources.
- GitHub Actions integration, the comprehensive game-runner framework, and the full A-01–A-47 case catalog — Phase 5 owns these. Phase 1's local proofs and actual two-platform execution remain required now.
- Real inventory, combat, economy, world/campaign systems, full save files/migrations, and player save/load UX — later feature phases own these; simple state examples do not count as their delivery.
- Release packaging, complete export acceptance, actual mixed-computer LAN, reference-PC performance, endurance, and balancing — later release work requires its own evidence. Phase 1 does not certify these outcomes.
- Importing LangProj's HTTP/EF/database project topology, test hosts, or registrations — only conventions and proof patterns applicable to this Godot game are inherited.

## Constraints

- The implementation uses Godot and C# and must execute its required proofs on Windows and Linux. An unavailable environment is a reported blocker, never a substitute pass. Headless execution may prove lifecycle/state behavior; it does not claim rendered gameplay or reference-hardware validation.
- Exact engine/SDK/compiler/analyzer/proof-runner versions and the renderer are implementation decisions to be settled by compatibility evidence. Research candidates and starter feature tags are not locked version choices. Once selected, the reproducible baseline has explicit pins and records relevant runtime/platform information.
- The specification locks outcomes, coverage, and exclusions. Project names/layout, interfaces, serialization format, numeric representations, analyzer implementations, and test framework choice belong to discuss-phase/plan-phase, within these outcomes.
- Version selection and rule adaptation must support construction, generated bindings, exports, signals, nullable initialization, engine lifetime, and engine-thread access together. A blanket exemption for Godot integration or handwritten code does not establish compatibility.
- Adopted source and proof inputs must be self-contained in the repository or reproducibly obtained through declared pinned dependencies. Donor folders are provenance sources, not permanent build/runtime dependencies; original donor files are preserved.
- Relevant reference rules, including numeric limits or coverage obligations, must receive an explicit retained/adapted/inapplicable disposition. Deferring GitHub Actions does not silently discard local proof obligations.
- State examples demonstrate the host-authoritative design direction without introducing client-authoritative gameplay or promising deterministic cross-platform Godot physics. Stable identities cannot be replaced by a process-local object address, transient peer number, or LAN address.
- Scope and requirement ownership in the approved roadmap remain unchanged. Creating this document or passing an isolated proof does not complete a gameplay or release requirement.

## Acceptance Criteria

All checkboxes describe implementation acceptance, not work already completed by this specification. A passing evidence set identifies the repository revision, platform, tool versions, invoked command, required cases, actual results, and supporting logs.

- [ ] **AC-01 — Coherent pins (RULE-01):** The selected baseline names the engine, SDK/compiler, analyzer, and proof-runner versions and checks their consistency. A missing required pin, conflicting duplicate declaration, or incompatible selected tool fails with a specific diagnostic. Reordering equivalent declarations cannot change the verdict or choose a different version.
- [ ] **AC-02 — Actual two-platform execution (RULE-01, RULE-06):** A clean checkout restores, builds, runs the rule proofs, and runs the representative Godot scene successfully on Windows and separately on Linux using documented commands. Evidence identifies both environments. Access to the LangProj donor path is unnecessary. Full game export acceptance is not claimed.
- [ ] **AC-03 — Trustworthy failure reporting (RULE-01, RULE-03):** The normal proof entry point returns success only when every currently required Phase 1 case has a fresh, uniquely identified, non-skipped result and its required evidence. Missing tools, zero discovery/empty required subject sets, wrong selection, skipped cases, a deliberately failing case, crash, timeout, missing result/log, and a stale prior report produce non-success with a diagnostic naming the missing or failing obligation. A negative-test case passes only when the expected subject violation is observed; an unrelated infrastructure error cannot stand in for it.
- [ ] **AC-04 — Repetition and interruption (RULE-01, RULE-03, RULE-04, RULE-07):** Repeating a complete proof run on unchanged inputs yields the same verdict. An interrupted run cannot publish a complete pass or lend its artifacts to a later run. Concurrent invocations either isolate their working state/results or explicitly reject the second invocation; they cannot mix evidence, corrupt shared inputs, or manufacture success. Required checks do not rewrite approved rule documents or source inputs to make them pass.
- [ ] **AC-05 — Exhaustive inheritance (RULE-02):** The register identifies the captured donor revision/content and accounts for every source rule heading and matrix rule exactly once, with a valid retained/adapted/inapplicable disposition and reason. Every adaptation identifies its Godot-specific need and scope; every inapplicable rule explains the mismatch. Missing entries, duplicate/conflicting IDs, empty required fields, and an empty inventory are rejected.
- [ ] **AC-06 — Register integrity (RULE-02, RULE-03):** Each retained/adapted rule connects its exact stable ID and local documentation to enforcement, declared scope/exemptions, and real proof references; references resolve back to the intended rule. Unknown IDs, missing mechanisms/proofs, inconsistent mappings, ambiguous references, and undocumented weakening fail. Reordering rows/files leaves the verdict and rule coverage unchanged. IDs and documented reference spellings are not silently case-folded or normalized into another rule. Rule/document/mechanism/proof changes must agree in the same checked revision; any qualitative review component is identified separately.
- [ ] **AC-07 — Positive and negative enforcement (RULE-03):** Every applicable machine-checkable rule has a compliant control and a deliberate violation that exercise its real enforcement mechanism. The violated subject is rejected with the rule and relevant location identified. Representative actual builds prove compiler/analyzer wiring and architecture/exemption failures beyond in-memory rule tests. Duplicate evidence for one case cannot satisfy a different required case, and processing order cannot mask a violation.
- [ ] **AC-08 — Bounded exemptions (RULE-03, RULE-06):** Every exemption identifies the exact rule/diagnostic, applicable file/type/generated surface, reason, and proof of its boundary. Duplicate exemptions or overlapping scopes for the same diagnostic are rejected rather than combined into broader permission. Unregistered suppressions, weakened severity, disguised generated markers on handwritten code, and violations immediately outside an allowed scope fail. Genuine engine-generated output and the compliant handwritten partial both build under their declared treatment.
- [ ] **AC-09 — Rule thresholds and representation (RULE-03):** Every retained/adapted numeric rule declares its unit, limit, inclusive/exclusive comparison, and any rounding or text-length definition. Proofs cover the limit and the nearest representable value on either side, including non-ASCII input where a text rule applies. Invalid text and out-of-range values are diagnosed consistently across platforms; counting/rounding/overflow cannot turn a violation into a pass. Different input ordering cannot alter threshold calculations or exemption matching.
- [ ] **AC-10 — Focused entry point (RULE-04):** Git tracks exactly one root `AGENTS.md` with that casing and no case-colliding alternative. Its content is a project description and task-to-document navigation, with full rules in focused documents. Reachable indexes cover code style, architecture, the rule matrix, proof commands, and decisions. Each created project documents purpose, boundaries, and allowed dependencies, and those lists match the implemented graph.
- [ ] **AC-11 — Portable documentation checks (RULE-04):** Required project-owned documents are nonempty and their local links/anchors resolve to unique intended targets with exact filename casing. A missing document, broken link/anchor, case-colliding path, ambiguous target, or invalid text is rejected on both operating systems. A singleton index is handled correctly and reordering links does not change validity. Spaces and non-ASCII text follow the declared link/encoding conventions without silently changing target identity. Check scope explicitly distinguishes project documentation from installed tool/vendor documentation.
- [ ] **AC-12 — Declared dependency graph (RULE-05):** The implemented minimal structure assigns gameplay, Godot integration, and multiplayer-service responsibilities and records allowed dependency directions. Gameplay rules/state do not depend on Godot object types or concrete discovery/identity/connection providers. The compliant graph and representative permitted boundary use pass the executable checks.
- [ ] **AC-13 — Boundary violation fixtures (RULE-05):** Fixtures that add a forbidden direct reference, a forbidden transitive/type dependency, or an undeclared integration dependency are rejected. Missing expected projects/types and empty rule selectors fail visibly; a legitimately dependency-free component passes when declared as such. Duplicated or reordered references cannot hide a forbidden edge or change the verdict.
- [ ] **AC-14 — Real engine integration (RULE-06):** The selected engine actually constructs and loads the fixture scene/resources, invokes its callbacks, binds handwritten/generated partials, reads exported values, emits/handles signals, applies the nullable-initialization policy, and tears down cleanly. Logs/results identify which assertion covers each surface on each OS. Compilation alone or direct calls that bypass the relevant engine lifecycle do not satisfy this check.
- [ ] **AC-15 — Repeated lifecycle and teardown (RULE-06):** At least two complete load/use/unload cycles obey the selected engine's documented callback order. A signal/action invokes its intended live handler exactly once per expected occurrence; a later cycle inherits no stale subscription or pending work from the earlier instance. Work queued before teardown cannot mutate disposed objects or change live state afterward. Cleanup does not require a blanket rule suppression.
- [ ] **AC-16 — Invalid initialization (RULE-06):** A fixture missing a required exported value/reference, or carrying an invalid required null, fails validation before that value is used and reports what is missing. A deliberately optional null is accepted under its documented contract. The valid fixture loads and runs under the enforced nullable and construction rules.
- [ ] **AC-17 — Engine thread boundary (RULE-06, RULE-07):** Valid engine access executes on the main thread. A deliberately prohibited worker-thread engine access is rejected by the designated check/guard; supported background work can return through the documented boundary without accessing dead objects. The pure state example runs without Godot object dependencies.
- [ ] **AC-18 — Repeatable inputs (RULE-07):** With the same initial example state, controlled time sequence, and random seed/state, independent runs produce the same asserted values and identities on both selected platforms. Restoring the example's saved random state continues the same next draws rather than rerolling. The examples do not read uncontrolled wall-clock time or global randomness through a bypass of the declared contract.
- [ ] **AC-19 — Numeric contract edges (RULE-07):** The example declares time units, valid ranges, precision, and conversion/rounding rules. Zero elapsed time does not advance it. Proofs cover valid minimum/maximum values, invalid values immediately outside the range, and precision-sensitive round trips. Negative elapsed time, overflow, and non-finite input where representable are rejected before state changes; no silent wrap, clamp, or loss of an identity/numeric value is accepted.
- [ ] **AC-20 — Identity, values, and order (RULE-07):** Serializing/restoring the example preserves its stable identities, valid numeric values, and meaningful sequence order exactly according to the declared representation. Reordering records that the contract treats as an unordered collection produces the same semantic state; it cannot select a different winner. Identity comparison/representation is explicit and cannot silently merge distinct valid identities through case/encoding conversion. Saved data contains independent values rather than live Godot references or transient process/peer addresses.
- [ ] **AC-21 — Empty and invalid state (RULE-07):** A valid empty collection, a singleton, and multiple uniquely identified records round-trip. Missing/empty required identity fields, duplicate/conflicting identities, malformed/incomplete data, and unsupported example schema versions are rejected with a specific diagnostic. A valid empty collection is distinguished from an absent or invalid state document. An error leaves the prior live example state intact.
- [ ] **AC-22 — Coherent repeated restoration (RULE-07):** Restoring the same accepted snapshot repeatedly produces the same state without appending records, allocating replacement identities, advancing time/randomness, or replaying an effect. Invalid input or interruption before completion cannot partially apply a replacement. Competing proof activity is isolated or rejected explicitly, and readers observe the complete prior or complete accepted state, never a mixture. This proves the small state contract without requiring the production save/session system.

## Edge Coverage

**Coverage:** 38/38 applicable edges resolved · 0 unresolved
**Verification:** 38 explicit acceptance resolutions · 0 test backstops · 0 dismissals

The canonical edge-probe engine was run over the exact seven requirement statements above. Authored shapes reflect the actual surfaces: tool/configuration collections and I/O; rule/document identifiers; numeric rule thresholds; the dependency graph; lifecycle state; and serialized numeric/identity state. Manual review extended the lifecycle shape to include its exported-value, callback, and subscription collections, adding three rows to the initial 35: adjacency, empty, and ordering for RULE-06. The engine was rerun with that refinement; all 38 rows are retained below. The creator selected explicit outcomes for all three grouped edge questions.

| Category | Requirement | Status | Resolution / Reason |
| --- | --- | --- | --- |
| adjacency | RULE-01 | resolved; verification: explicit | AC-01: conflicting declarations or ambiguous duplicate pins cannot select an accidental toolchain. |
| empty | RULE-01 | resolved; verification: explicit | AC-01–AC-03: missing pins/tools/cases/evidence cannot pass. |
| ordering | RULE-01 | resolved; verification: explicit | AC-01: equivalent pin ordering cannot change selection or verdict. |
| idempotency | RULE-01 | resolved; verification: explicit | AC-04: repeated runs on unchanged inputs have the same verdict. |
| concurrency | RULE-01 | resolved; verification: explicit | AC-04: isolate or explicitly reject competing runs; interrupted work cannot publish success. |
| adjacency | RULE-02 | resolved; verification: explicit | AC-05–AC-06: source/rule IDs are unique and duplicate/conflicting mappings fail. |
| empty | RULE-02 | resolved; verification: explicit | AC-05–AC-06: empty inventory, required fields, and missing enforcement/proofs fail. |
| encoding | RULE-02 | resolved; verification: explicit | AC-06: stable IDs and reference spellings resolve exactly, without silent normalization. |
| ordering | RULE-02 | resolved; verification: explicit | AC-06: reordered register rows preserve coverage and verdict. |
| boundary | RULE-03 | resolved; verification: explicit | AC-08–AC-09: exemption boundaries and numeric limits are explicitly tested. |
| adjacency | RULE-03 | resolved; verification: explicit | AC-07–AC-08: duplicate results and overlapping same-diagnostic exemptions cannot create a pass. |
| empty | RULE-03 | resolved; verification: explicit | AC-03, AC-06: empty subjects/discovery and missing proof references fail. |
| encoding | RULE-03 | resolved; verification: explicit | AC-06, AC-09: exact diagnostic/rule identity and declared text-length/encoding semantics. |
| ordering | RULE-03 | resolved; verification: explicit | AC-06–AC-09: processing order cannot alter coverage, exemptions, thresholds, or verdicts. |
| precision | RULE-03 | resolved; verification: explicit | AC-09: counting, rounding, and overflow cannot misclassify a numeric rule violation. |
| idempotency | RULE-03 | resolved; verification: explicit | AC-04: reruns do not consume stale passes or rewrite inputs. |
| concurrency | RULE-03 | resolved; verification: explicit | AC-04: interrupted/concurrent checks cannot mix results or silently change inputs. |
| adjacency | RULE-04 | resolved; verification: explicit | AC-10–AC-11: one canonical root path and no case-colliding or ambiguous target. |
| empty | RULE-04 | resolved; verification: explicit | AC-10–AC-11: required documents/index entries are present and nonempty. |
| encoding | RULE-04 | resolved; verification: explicit | AC-11: exact casing, spaces, non-ASCII text, and declared encoding/link conventions. |
| ordering | RULE-04 | resolved; verification: explicit | AC-11: reordered links preserve validity and reachability. |
| concurrency | RULE-04 | resolved; verification: explicit | AC-04: checks cannot overwrite approved documents or combine partial evidence. |
| adjacency | RULE-05 | resolved; verification: explicit | AC-13: duplicate references cannot mask a forbidden dependency. |
| empty | RULE-05 | resolved; verification: explicit | AC-13: distinguish a declared dependency-free component from a missing expected subject. |
| ordering | RULE-05 | resolved; verification: explicit | AC-13: graph/reference order cannot change the dependency verdict. |
| adjacency | RULE-06 | resolved; verification: explicit | AC-15: duplicate/stale subscriptions cannot multiply the intended callback. |
| empty | RULE-06 | resolved; verification: explicit | AC-16: distinguish valid optional nulls from missing or invalid required exports. |
| ordering | RULE-06 | resolved; verification: explicit | AC-14–AC-15: actual callbacks obey the selected engine's documented lifecycle order. |
| idempotency | RULE-06 | resolved; verification: explicit | AC-15: repeated load/use/unload has no stale subscription or duplicated handler. |
| concurrency | RULE-06 | resolved; verification: explicit | AC-15–AC-17: late work, invalid initialization, and off-thread access obey explicit lifecycle outcomes. |
| boundary | RULE-07 | resolved; verification: explicit | AC-19: zero, valid limits, and immediately invalid numeric values are tested. |
| adjacency | RULE-07 | resolved; verification: explicit | AC-20–AC-21: distinct identities survive; duplicate/conflicting identities are rejected. |
| empty | RULE-07 | resolved; verification: explicit | AC-21: empty/singleton/multiple valid collections differ from missing or invalid data. |
| encoding | RULE-07 | resolved; verification: explicit | AC-20: identity representation and equality are preserved explicitly. |
| ordering | RULE-07 | resolved; verification: explicit | AC-20: meaningful sequence order survives; unordered record permutations are equivalent. |
| precision | RULE-07 | resolved; verification: explicit | AC-19–AC-20: numeric limits, precision, and identity/value round trips do not lose information. |
| idempotency | RULE-07 | resolved; verification: explicit | AC-18, AC-22: restoration repeats without new identities, rerolls, time advancement, or replayed effects. |
| concurrency | RULE-07 | resolved; verification: explicit | AC-04, AC-21–AC-22: invalid/interrupted replacements cannot partially change live state. |

## Prohibitions (must-NOT)

**Coverage:** 0/0 additional applicable prohibitions · 0 unresolved

The recall/precision review covered RULE-01 through RULE-07 after the edge decisions. No additional bespoke values, privacy, safety, or ethics prohibition was retained for this developer-tooling phase. Candidate concerns about blanket suppression, false/stale proof, changing inputs to obtain a pass, invalid state, donor dependency, and misleading platform completion are concrete engineering/acceptance concerns already covered above. Copying the donor's web architecture or expanding into gameplay is addressed by the explicit boundaries. These concerns are not duplicated as a new prohibition category, and no fictional test descriptors are introduced.

## Ambiguity Report

| Dimension | Score | Min | Status | Notes |
| --- | --- | --- | --- | --- |
| Goal Clarity | 0.95 | 0.75 | Met | A usable, enforced, proven development baseline is the deliverable. |
| Boundary Clarity | 0.95 | 0.70 | Met | Small proofs are included; gameplay, full CI/framework, and release proof stay with their owning phases. |
| Constraint Clarity | 0.90 | 0.65 | Met | Both operating systems must execute; actual version/layout choices remain compatibility-driven implementation decisions. |
| Acceptance Criteria | 0.95 | 0.70 | Met | Explicit positive/negative, lifecycle, state, and failure-evidence checks; no deferred edges. |
| **Ambiguity** | **0.06** | **≤ 0.20** | **Passed** | 1 − (0.35 × 0.95 + 0.25 × 0.95 + 0.20 × 0.90 + 0.20 × 0.95). |

Assessment history:

- Initial: goal 0.85, boundaries 0.85, constraints 0.70, acceptance 0.65; ambiguity 0.22. Acceptance was below its minimum.
- After interview round 1: goal 0.95, boundaries 0.90, constraints 0.90, acceptance 0.90; ambiguity 0.0825 (reported as 0.083). All minimums and the gate passed.
- After edge decisions: goal 0.95, boundaries 0.95, constraints 0.90, acceptance 0.95; ambiguity 0.06. All minimums pass; no requirement assumption remains unresolved. Selecting concrete implementation techniques is the next workflow's task.

## Interview Log

The creator requested interactive ask menus; all recorded answers below came from those menus. No `--auto` choices were substituted for answers.

| Round | Perspective | Question summary | Answer and decision locked |
| --- | --- | --- | --- |
| 1 | Researcher / acceptance | Which platforms must actually pass before Phase 1 closes? | Windows and Linux: execute and pass the build, rules, and representative scene on both. Linux execution is not deferred to Phase 5. |
| 1 | Researcher / scope | Rules only or small executable time/randomness/identity/state examples? | Small executable proofs: repeatable time/random inputs, identities retained through serialization, and state independent of Godot objects. |
| Edge probe | Failure analysis | Missing/skipped/stale evidence; empty/duplicate rules; conflicting pins; overlapping exemptions; disguised generation; casing; thresholds; order? | Specify all as explicit checks with actionable diagnostics and fresh results for repeated/interrupted runs. |
| Edge probe | Failure analysis | Repeated lifecycle, required exports, late work, and worker-thread engine access? | Specify clean repeated cycles, failure before use of missing required values, no post-teardown state change, and main-thread engine access. |
| Edge probe | Failure analysis | Time limits, invalid/duplicate IDs, empty/reordered records, exact round trips, repeated/interrupted restoration? | Specify preservation of valid identities/values/meaningful order and rejection of invalid/incomplete state without partial application. |

---

*Phase: 01-code-rules-and-documentation*
*Spec created: 2026-11-13*
*Next step: `$gsd-discuss-phase 1` — implementation decisions for the locked requirements above.*
