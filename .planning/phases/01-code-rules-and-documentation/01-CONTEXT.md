# Phase 1: Code rules and documentation - Context

**Gathered:** 2026-11-13 (Europe/Paris)
**Status:** Ready for planning

**Creator-approved amendments, 2026-11-14:** Preserve all working rules, fixtures and tests. Measured coverage, advanced platform/report reconciliation and unfinished aggregate certification remain [Phase 5](../05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) work. The creator's later platform instruction supersedes the earlier representative-only Linux boundary: use the development OS for implementation feedback, then execute the complete current phase scope on that OS and a local VM running the other OS before phase completion. For Phase 1 this requires complete Windows and Linux-VM `--phase 1` runs for the final implementation, plus separate qualitative review. Use local hardware acceleration when available. See [01-SPEC.md](01-SPEC.md) and [AI collaboration](../../../docs/workflow/ai-collaboration.md).

<domain>
## Phase Boundary

Establish enforced code, architecture, and lint rules so people and AI agents write consistent C# for KingdomZombies. Use the existing empty Godot project to launch the minimal examples needed to prove those rules and engine compatibility. This phase delivers the development baseline, its proof checks, and focused documentation. Gameplay belongs to the later phases.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**7 requirements are locked: RULE-01 through RULE-07.** See [01-SPEC.md](01-SPEC.md) for full requirements, boundaries, and acceptance criteria.

Downstream agents MUST read `.planning/phases/01-code-rules-and-documentation/01-SPEC.md` before planning or implementing. Requirements are not duplicated here. The creator's request to keep the examples minimal clarifies their form; it does not remove the required engine lifecycle/state proofs or actual Windows and Linux execution.

**In scope (from SPEC.md):**

- The reproducible toolchain/build baseline and small proof-running entry points, with executed Windows and Linux evidence.
- The complete LangProj inheritance inventory, local rule matrix, enforcement configuration, scoped compatibility decisions, and positive/negative proofs.
- Minimal compilable gameplay/Godot/service boundaries and executable dependency checks.
- The real Godot integration scene and small time/randomness/identity/serialization examples needed to prove these rules.
- Focused documentation, canonical `AGENTS.md` casing, checked local navigation, and documentation for each created project and architecture decision.
- Required-case/result checks sufficient to trust Phase 1's own proofs, including deliberate failure of the verification path.

**Out of scope (from SPEC.md):**

- Player-facing menu, movement, production input, camera, or playable foundation — Phase 2 owns these; the integration scene is a proof fixture.
- LAN discovery/joining, host-plus-three-player gameplay, session recovery, and platform-service implementations — Phases 3 and 9 own the relevant behavior; Phase 1 establishes boundaries only.
- Asset production and the complete content pack — Phase 4 owns these; Phase 1 needs only minimal fixture resources.
- GitHub Actions integration, the comprehensive game-runner framework, and the full A-01–A-47 case catalog — Phase 5 owns these. Phase 1's local proofs and actual two-platform execution remain required now.
- Real inventory, combat, economy, world/campaign systems, full save files/migrations, and player save/load UX — later feature phases own these; simple state examples do not count as their delivery.
- Release packaging, complete export acceptance, actual mixed-computer LAN, reference-PC performance, endurance, and balancing — later release work requires its own evidence. Phase 1 does not certify these outcomes.
- Importing LangProj's HTTP/EF/database project topology, test hosts, or registrations — only conventions and proof patterns applicable to this Godot game are inherited.

</spec_lock>

<decisions>
## Implementation Decisions

### Executable rules and focused documentation

- **D-01:** Make machine-checkable rules primarily executable lint/analyzer/configuration/architecture-check definitions. Documentation indexes and links to these definitions instead of maintaining duplicate prose descriptions as a competing source of truth. The locked requirements and approved rule dispositions still constrain what that code must enforce.
- **D-02:** Explain non-obvious rules beside their code or configuration where the format permits it, consistent with the adopted comment conventions. Separate documents explain architecture decisions, Godot adaptations, and review-only rules.
- **D-03:** Preserve the required inheritance and rule register: stable IDs, retained/adapted/inapplicable dispositions, reasons, exact code/configuration references, scopes/exemptions, and positive/negative proof mappings. Making the rules executable does not remove this traceability.
- **D-04:** Put genuinely qualitative obligations in a short, explicit review checklist linked to the relevant code and architecture decisions. Identify them as human-review obligations; do not present them as automated passes.
- **D-05:** Use focused reference indexes only. Setup commands and verification commands live in their respective documents; do not add a consolidated “Start here” walkthrough. Keep the root agent guide short.
- **D-06:** Consistent code for both people and agents is the purpose of the rules. Small functions are an explicit preference to enforce. Resolve exact thresholds through the LangProj inheritance register and required proofs; the discussion did not select a new numeric limit.

### Minimal examples and language boundary

- **D-07:** Reuse `src/godot-game/project.godot` as the launch target. Add only the minimal example code/files/resources needed to exercise the specified code/architecture rules, Godot lifecycle surfaces, and state contracts.
- **D-08:** Do not build gameplay, a game-themed demonstration, or a presentation feature in Phase 1. Minimal proof fixtures must not become a second game project or a substitute playable foundation. Launching the empty project alone does not replace the specified executable assertions.
- **D-09:** Game code, example game/engine code, and tests use C#. Development tools may use other languages. Do not interpret “C# only” as a prohibition on all non-C# tooling or ordinary configuration formats.

### Everyday verification workflow

- **D-10 (amended twice, 2026-11-14):** Use focused checks on the development OS while batching related closure changes. After implementation stabilizes, execute the complete current Phase 1 scope on Windows and in the local Linux VM for the same final implementation. Use hardware acceleration when available. Re-run affected checks after a defect; repeat a complete run when a subsequent source/contract change invalidates it. The eight declared Phase 5 coverage/aggregate cases remain pending; advanced certification stays there. Existing required checks still fail on real failures, skips, missing results or stale evidence.
- **D-11 (amended 2026-11-14):** Planning-only and prose-only documentation edits use documentation and planning-consistency checks. They do not execute planned commands or claim new implementation evidence. Changes to executable rules, runner selection or code receive focused behavioral checks plus the final D-10 Phase 1 run. Retain every working enforcement rule and test.
- **D-12:** Show concise progress and a final summary, with clear failure explanations and links to detailed logs.
- **D-13:** Collect all independent failures in one run. Stop checks whose prerequisites failed and report those blocked obligations explicitly; they do not count as passes. Retain the specification's fresh-result, non-skipping, and trustworthy-failure requirements.

### Tooling stability

- **D-14:** Use stable engine and runtime releases. Prerelease test or lint tools are acceptable when needed and proven compatible; a prerelease is not an automatic default.
- **D-15:** When multiple supported stable combinations pass the proofs, prefer an established baseline. Upgrade for a concrete compatibility or support benefit rather than merely because a newer version exists.
- **D-16:** Exact engine/SDK/compiler/analyzer/test-tool versions remain research and compatibility-proof decisions. Pin the selected baseline as required by SPEC.md. Existing starter tags and research candidates were not approved as the final versions during this discussion.

### AI collaboration

- **D-17:** Ask questions through available interactive question controls, including free-text answers. Use ordinary chat questions only when those controls are unavailable or the creator requests text. The creator explicitly requested that this preference be recorded in indexed AI instructions.
- **D-18:** This preference is already recorded in `docs/workflow/ai-collaboration.md`, indexed from `Agents.md` and `docs/workflow/gsd.md`; `.planning/config.json` now sets `workflow.text_mode` to `false`. Preserve these links when Phase 1 establishes canonical root `AGENTS.md` casing.

### Agent's Discretion

No “you decide” answers delegated additional product choices. Research and planning still select concrete versions, project layout, enforcement implementations, state representations, and proof tooling within the locked specification and the decisions above. Choose the smallest implementation that genuinely proves the requirements; do not invent gameplay work or new documentation layers.

**D-19 — Preserve completed work:** Keep all already implemented checks, including runner-edge cases delivered during 01-14, generated-source permissions, strict snapshots and native lifecycle fixtures. The simplification changes remaining acceptance timing; it does not delete tests, relax diagnostics, widen exemptions or rewrite working infrastructure. The eight still-pending aggregate/coverage case IDs are explicitly assigned to Phase 5 by 01-15, with no skipped or fabricated pass.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.** Paths below are relative to the repository root, including external donor paths beginning with `../../`.

### Phase and project authority

- `.planning/phases/01-code-rules-and-documentation/01-SPEC.md` — locked requirements; MUST read before planning.
- `.planning/REQUIREMENTS.md` — RULE-01 through RULE-07 and their primary phase ownership.
- `.planning/ROADMAP.md` — Phase 1 goal, completion criteria, and boundaries with later phases.
- `.planning/PROJECT.md` — creator-approved scope, LangProj inheritance, technology, and verification constraints.
- `.planning/research/SUMMARY.md` — implementation candidates and unresolved proofs; recommendations are not validated version choices.
- `specs/0.1/initial-spec.md` — game authority; particularly the multiplayer-service boundary and state/persistence context. Use it to preserve architectural compatibility, not to pull gameplay into Phase 1.
- `specs/0.1/assets-and-resources.md` — content ownership and minimal fixture-resource context; the production asset pack belongs to Phase 4.

### Indexed project instructions

- `Agents.md` — current tracked root entry point; Phase 1 still owns its canonical `AGENTS.md` casing.
- `docs/workflow/ai-collaboration.md` — creator-requested interactive question rule.
- `docs/workflow/gsd.md` — workflow commands and planning-artifact navigation.

### LangProj provenance sources

These external files are inventory/provenance inputs, not permanent build, runtime, test, or documentation-resolution dependencies. Capture the adopted baseline and reusable inputs inside KingdomZombies as SPEC.md requires. The final local rule docs and register must stand on their own.

- `../../SaaS/LangProj_monorepo/src/backend/docs/CodeRules/code-style.md` — source C# conventions, including function/member size and formatting rules.
- `../../SaaS/LangProj_monorepo/src/backend/docs/CodeRules/code-architecture.md` — dependency and type conventions; classify web/database-specific topology instead of copying it.
- `../../SaaS/LangProj_monorepo/src/backend/docs/rule-matrix.md` — source rule identities, enforcement, scoped exemptions, and proof mappings.
- `../../SaaS/LangProj_monorepo/src/backend/docs/decisions/0003-enforcement-approach.md` — source enforcement rationale. This is the verified path; the abbreviated path in PROJECT.md omits `src/backend/`.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- `src/godot-game/project.godot` is the existing starter launch target. It has project/assembly settings and an icon, but no C# project, scripts, scenes, analyzers, architecture checks, or proof runner were present during the scout.
- `src/godot-game/.editorconfig` currently specifies UTF-8 only; it is not the implemented coding standard.
- The LangProj references identify existing analyzer and architecture-proof patterns that may be adapted. Copy only the applicable, proven inputs into this repository; preserve donor originals.

### Established Patterns

- The root guide already uses a task-to-document table. Continue its focused navigation pattern.
- `docs/workflow/gsd.md` already holds workflow commands; `docs/workflow/ai-collaboration.md` now holds interaction preferences.
- There are no gameplay patterns or C# test framework choices to preserve. The starter's engine/rendering feature tags are declarations, not compatibility evidence.
- No prior-phase context, codebase maps, or matching pending todos existed for this first phase.

### Integration Points

- Put the minimal Godot examples under the existing game project and connect them to executable rule/lifecycle checks.
- Establish the minimal compilable gameplay/Godot/service boundaries required by SPEC.md without implementing the future systems.
- Connect rule IDs to their executable definitions, inheritance dispositions, scoped exceptions, proof fixtures, and documentation indexes.
- Resolve the tracked `Agents.md` casing during Phase 1 and update affected links together, including the link from the AI collaboration document.
- Keep the Phase 1 local proof entry points small and compatible with later Phase 5 verification work; do not implement the comprehensive game-runner framework now.

</code_context>

<specifics>
## Specific Ideas

- Creator emphasis: “Code should be as a rule for most of the times.” Executable definitions should carry most rules.
- Creator emphasis: “We already have an empty project. just use it to launch. you can add a few example code files. but don't build the game.”
- The purpose is for “AI agents and ppl [to] write similar code,” including small functions.
- The creator chose focused reference indexes over a “Start here” page.
- The creator narrowed the language rule to C# for game code and tests, explicitly allowing other languages for development tools.
- All four discussion areas were selected, and the creator explicitly chose “Save the phase context” after reviewing the summary.

</specifics>

<deferred>
## Deferred Ideas

None — discussion stayed within phase scope. Gameplay and a playable foundation remain in their existing later phases; no new backlog item or phase was proposed.

</deferred>

---

*Phase: 01-code-rules-and-documentation*
*Context gathered: 2026-11-13*
