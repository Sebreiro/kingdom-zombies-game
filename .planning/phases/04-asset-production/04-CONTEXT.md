# Phase 4: Asset production - Context

**Gathered:** 2026-11-14 (Europe/Paris)
**Status:** Ready for planning; implementation depends on completion of Phases 1–3.

<domain>
## Phase Boundary

Produce the complete, traceable 180-obligation game resource pack in the existing Godot/C# project, with reusable procedural character composition, live inspection, assembled-map and structure/state inspection, UI/text/data resources, auditionable sound and a self-contained Windows inspection build. Reuse suitable recovered Kingdom Two Crowns material first, adapt it to KingdomZombies, and generate the gaps. Later phases integrate these resources into actual gameplay.

</domain>

<spec_lock>
## Requirements (locked via SPEC.md)

**5 requirements are locked: ASSET-01 through ASSET-05.** See [04-SPEC.md](04-SPEC.md) for the full requirements, constraints and acceptance criteria.

Downstream agents MUST read the Phase 4 specification before planning or implementing. Its requirements are not duplicated as implementation decisions below.

**In scope:**

- All outputs and explicitly shared outputs for the complete 180-obligation register, including the 24 item definitions, complete baseline data, English text, fonts, sound, ambience, desktop identity and documentation.
- Reuse/adaptation of suitable recovered material, Codex/GPT generation of gaps, preserved-source identities, selected dependency export and production provenance.
- Godot procedural/composable animation, attachment/fitting/occlusion metadata, reusable body/equipment layers, action timing, sound/effect bindings and persistent corpse endpoints.
- A live character inspector for bodies, gear, movement and attacks; immediate selection changes with correct current visuals and clean restarts where needed.
- Inspection of all main/wall/tower tiers and lifecycle states, six portal placements/states, the assembled authored map, environment, plants, wildlife and effects.
- UI visual layouts and state examples, including error/legacy/conflicting states, scaling and expanded-text inspection; audio audition controls and documented event routing.
- Content/compatibility metadata and fixture-based reconstruction of current appearances without old one-shot effects.
- A self-contained Godot project, Windows inspection build and focused Windows content checks, including the held-out backstops below.
- A visible register of permitted minor cosmetic issues, with exact affected combinations, reproduction steps, expected/actual presentation and the first relevant gameplay phase as owner.

**Out of scope:**

- Replacing the Phase 2/3 solo/LAN foundation placeholders with production art — the creator selected separate pack/inspection delivery and later gameplay integration.
- Implementing the 27 UI surfaces' gameplay interaction logic, inventory transactions, buying, saving, networking, jobs or combat rules — this phase produces their resources and visual state examples.
- Playable traversal of the authored campaign map, autonomous NPC/zombie behavior, waves, campaign scheduling and outcomes — inspect the assembled resources here; gameplay arrives in later increments.
- Full gameplay save/load/late-join reconstruction and event delivery — current-state content fixtures and metadata are included, while real session/ownership/save behavior is integrated later.
- Linux inspection builds/execution, exhaustive build/platform certification, complete CI/E2E infrastructure, release exports, reference-PC FPS certification and real-duration endurance — the creator requested a Windows build and limited build testing here; Phases 5 and 10 and the relevant feature phases own their later evidence.
- A separate baked animation/atlas for every possible full-body/equipment/action combination — reusable composition is a required constraint.
- Extra cosmetic rosters, new maps/biomes, procedural terrain, excluded weapons/systems, music, voice acting, tutorials, controller prompts, additional shipped locales or platform-service screens — not required by the approved 0.1 register.
- Purchasing an animation add-on — a priced proposal may be considered during implementation discussion/planning; the creator has not authorized a purchase.

**Acceptance carry-forward:** Preserve all AC-01–AC-24, all 34 individually identifiable held-out backstops H01–H34 and all four judgment-tier prohibitions. They are obligations, not passing evidence. The full 180-obligation census remains required; file counts or metadata-only records cannot replace actual content. Only the specification's exactly documented minor cosmetic defects may carry to their first relevant Phase 6/7/8 integration owner.

**Discussion refinement:** The creator selected built-in Godot animation tools and project code only (D-08). The optional paid-add-on route mentioned in the specification is not selected.

</spec_lock>

<decisions>
## Implementation Decisions

### Animation and equipment motion

- **D-01:** Use deliberate pixel-art poses for walking and attacks, with flexible arm/weapon movement where aiming needs it. Keep crisp pixel edges and the required concurrent walking/combat presentation; do not make full baked sheets for every equipment/action combination.
- **D-02:** Melee attacks have clear weight and follow-through. Shoulders and torso support the strike; heavier bodies and weapons feel weighty, while quick attacks remain sharp. Walking continues.
- **D-03:** Clothing, belts and bags follow their attachment points closely, with little independent sway or bounce. This selects mostly fixed gear rather than the proposed secondary-motion treatment.
- **D-04:** Melee effects are very minimal. Let the swing and target reaction carry the action; use only the small contact effects needed for clarity. The restrained-blood/no-dismemberment limit remains binding.
- **D-05:** Bow aiming is led mainly by the arms and bow. Keep the head and torso close to their normal pose while aiming up/down, rather than adding pronounced torso/head tracking.
- **D-06:** Death uses a short authored collapse suited to each body, ending in a stable, readable equipped corpse pose. A variable physical-collapse treatment was not selected.
- **D-07:** Hits cause a small, clear shoulder/torso flinch. This is visual feedback only and adds no stun, knockback or action delay.
- **D-08:** Use Godot's built-in animation tools and project code only, with no paid animation dependency. Research and planning should solve composition within that boundary rather than schedule a paid-add-on comparison or purchase.
- **D-09:** Direction changes switch facing immediately and continue the current stride/action coherently. Preserve logical left/right hand identity and actual equipment.
- **D-10:** Starting to walk uses a quick visual weight shift; stopping uses a short settle. Actual movement starts and stops immediately.
- **D-11:** Clothing, armor and bags have a practical scavenged fit: slightly loose, utilitarian and modestly bulky, fitted to each required body. Slightly loose shapes do not imply the independent sway/bounce rejected in D-03.
- **D-12:** Gathering, building, carrying and fireside rest use restrained, recognizable task poses with only the body movement each activity needs.

### Pixel scale and visual consistency

- **D-13:** Favor the recovered sprites' simpler, low-resolution logical detail. Simplify the larger zombie/base examples to match, rather than raising the whole pack to their richer detail. Exact pixel density and dimensions remain a research/production choice established through the required assembled survivor/zombie/wall/tower/base scale test.
- **D-14:** Use natural days and atmospheric nights: earthy scenery and warm wood in daylight, cooler/darker nights and restrained warm firelight. Required equipment, identities and states remain readable after dark.
- **D-15:** Keep character heads, hands and equipment close to natural proportions. Use pose and contrast for readability rather than noticeably enlarged heads/hands/items.
- **D-16:** Buildings and defenses look like practical camp construction: rough timber, straightforward bracing and functional reinforcement, with clear tier silhouettes. Preserve the specified wood/reinforced-wood/metal progression.
- **D-17:** Backgrounds are layered but unobtrusive. Distant scenery and parallax provide depth while quiet space keeps characters, equipment and sites clear.
- **D-18:** For normal unselected characters, strengthen dark outer pixel edges selectively where needed while keeping donor-style shading. Do not apply a uniform outline by default.
- **D-19:** Standing damaged structures show localized material damage: cracks, splintered timber, bent reinforcement and small damaged areas, increasing with severity. Keep tier, material and functional state clear.
- **D-20:** Provide one canonical inventory icon per item, without separate durability-level icon variants. The creator did not choose one of the proposed icon-art treatments and does not want that detail expanded now. Exact durability indicators remain a planning detail within the required layouts/state examples; this does not remove item identities or required broken/condition presentation, nor the separate worn/held/corpse forms.
- **D-21:** Zombie bodies share related muted undead skin tones, with subtle color differences supporting their required distinct silhouettes. Equipped items retain their actual appearance.
- **D-22:** Start portal colors from suitable recovered donor colors. Spit and cosmetic acid use separate restrained sickly colors. No exact hue or numeric palette was selected; cosmetic acid remains non-damaging.
- **D-23:** Use small, quiet pixel contact-shadow shapes beneath characters and small objects to establish their contact with the ground.
- **D-24:** Selected corpses, loot piles and other inspectable objects use a steady thin contrasting pixel outline and modest marker. This defines selected-state presentation; it does not implement later gameplay interactions.

### Inspection and review workflow

- **D-25:** Use one Windows inspection hub organized by category, with common navigation for characters, items, buildings/world, UI, data/text and audio. The complete obligation register and exact resource failures remain accessible under the specification.
- **D-26:** The live character page gives one character most of the space, with body, equipment, movement and attack controls alongside. Side-by-side or multi-character comparison layouts were not selected.
- **D-27:** Include play/pause, playback-speed and single-step controls to examine grips, layering, poses and timing. These operate the inspection preview; they do not change the active-game menu's no-pause rule.
- **D-28:** Keep each preview's resource ID, name and status visible in a compact summary. Expand provenance, dimensions, timing and other metadata when needed. Missing/invalid-resource errors remain visible rather than being hidden in collapsed details.

### Interface and sound treatment

- **D-29:** Use suitable recovered wooden buttons/borders as frames around simple dark, quiet panel interiors. Keep text, numbers and item grids clear.
- **D-30:** Use a clear sans-serif face for most body text and numbers, with pixel-style type for selected headings/accents. Exact font files, sizes and fallbacks remain subject to extraction/comparison and the specified readability/scaling/expanded-text checks.
- **D-31:** Sound effects are punchier and more game-like: sharper swings, stronger impacts and more noticeable confirmations. This is an explicit selection over the proposed restrained-audio recommendation; the minimal visual-effects preference does not make the audio equally restrained. Suitable recovered recordings still come first and require audition/adaptation.
- **D-32:** Keep forest, wind and fire ambience subdued so action sounds and important cues stand out. Include the required day/night/local ambience and audition controls; exact gains and mix parameters remain configurable production choices.

### Agent's Discretion

The creator left the exact durability indicator to planning ("numbers or something" was not a selected display design). No other answer explicitly delegated a product preference as "you decide."

Research/planning still choose the exact logical density through the required assembled scale test, native Godot composition/rig features, pose and attachment data, per-body fitting, timings, resource formats/folder layout, extraction/transformation tooling and validation implementation. These choices must preserve donor pixel registration, authored timing/events, logical hand identity, current equipment/ammunition, occlusion and persistent endpoints.

Select actual donor outputs, normalized palettes, font files/sizes/fallbacks and auditioned sound variants/mix values within the directions above. Retain the required stable IDs, versions, generation sources/recipes and portable dependencies. No particular new library, paid dependency, numeric density, playback increment, font or media format was selected here.

Routine inspection navigation/control details may be resolved within D-25–D-28. No extra comparison grid, additional cosmetic roster, new gameplay interaction or other capability was requested. Retain the approved baseline data and sound limits rather than inventing new balance from these presentation choices.

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read the relevant references before planning or implementing.** Every path below is relative to the repository root.

### Phase and game authority

- `.planning/phases/04-asset-production/04-SPEC.md` — Five locked requirements ASSET-01–ASSET-05; MUST read before planning. Owns the complete census, AC-01–AC-24, H01–H34, four judgment prohibitions, Windows inspection boundary and minor-cosmetic exception.
- `.planning/REQUIREMENTS.md` — Approved requirement identities, primary phase ownership and acceptance traceability.
- `.planning/ROADMAP.md` — Phase 4 goal and dependency on Phase 3; later gameplay integration, testing and release owners.
- `.planning/PROJECT.md` — Complete version 0.1 direction, reuse priority, preserved sources, Godot/C# and self-contained delivery.
- `specs/0.1/initial-spec.md` — Game behavior, equipment and inventory semantics, authored map, configurable baselines, exclusions and A-01–A-47.
- `specs/0.1/assets-and-resources.md` — Complete 180-obligation register; exact donor locators/evidence levels; scale test, composition/fitting, UI/text/audio/data and production checks. Particularly §§3–5 and §§9–12.
- `.planning/research/SUMMARY.md` — Research candidates and unresolved proofs; suggestions are not implemented or verified content.

### Prior context and indexed project instructions

- `.planning/phases/01-code-rules-and-documentation/01-CONTEXT.md` — Executable rules, focused indexes, C# game code/tests, scoped verification and concise reporting. Read the current specification and indexed workflow for subsequent amendments.
- `.planning/phases/02-playable-foundation/02-CONTEXT.md` — Basic foundation visuals, production-art ownership in Phase 4 and English communication.
- `.planning/phases/03-four-player-lan-foundation/03-CONTEXT.md` — Same basic LAN fixture, gameplay/service boundaries and later production-content integration.
- `.planning/STATE.md` — Actual execution position and outstanding prerequisites; this discussion does not advance implementation phases.
- `AGENTS.md` — Canonical documentation entry point.
- `docs/workflow/ai-collaboration.md` — Interactive question preference and current collaboration/testing policy.
- `docs/code/README.md` — Executable C# rules, scoped compatibility and qualitative review references.
- `docs/architecture/README.md` — Project, type, engine, state and service boundaries.
- `docs/architecture/decisions/0002-boundaries.md` — Gameplay/engine/service dependency rationale.
- `docs/architecture/decisions/0003-state-and-engine.md` — Controlled state, reconstruction and engine lifetime.
- `docs/workflow/setup.md` — Current pinned, isolated development prerequisites.
- `docs/workflow/verification.md` — Selecting focused documentation/content checks and interpreting actual evidence.
- `docs/workflow/gsd.md` — Workflow commands and planning-artifact navigation.
- `tools/phase1/toolchain.json` — Pinned toolchain inputs; compatibility evidence remains separate.
- `tools/phase1/projects.json` — Approved project inventory and dependency categories.
- `src/godot-game/project.godot` — Existing Godot launch target; deliver the pack/inspection within this project.

### Local visual references and candidate art

- `examples/zombie/zombie-idle-original-size.png` — Reviewed shambler palette/posture reference; simplify to the selected donor-like logical detail.
- `examples/zombie/zombie-idle.png` — Companion local zombie reference; file resolution does not set world scale.
- `examples/zombie/zombie-walk-strip.png` — Candidate walking artwork, requiring registration, timing and loop checks.
- `examples/zombie/zombie-walk-sheet.png` — Alternative candidate walking export; choose a canonical production representation.
- `examples/zombie/zombie-attack-strip.png` — Candidate attack artwork; not complete equipment/action coverage.
- `examples/zombie/zombie-attack-sheet.png` — Alternative candidate attack export; validate actual required hand/action use.
- `examples/bases/main-base-tier-1.png` — Material/mood reference; the specified first tier remains fire/boxes.
- `examples/bases/main-base-tier-2.png` — Wood/reinforcement reference, requiring the game's own tier silhouette.
- `examples/bases/main-base-tier-3.png` — Reviewed wood/metal mood reference; the final tier still needs the specified two one-guard attachments.

The external recovery root identified in the specification/resource plan is an authoring/provenance input. Runtime inspection and clean content checks must resolve packaged project content rather than require that folder. Preserve the original recovered files.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets

- The existing Godot project and C# boundaries are the delivery target. At scout time, its scenes/resources and scripts were Phase 1 proofs rather than a production character renderer, content manifest or gallery.
- The local zombie idle/walk/attack artwork and three base examples are reference/candidate inputs. The zombie idle and base tier-3 example were visually inspected during discussion; that is not production approval of every candidate frame or the final required tier silhouettes.
- The resource plan identifies suitable recovered scenery, building pieces, wildlife, motion references, UI pieces, fonts and audio candidates. Its V/M/C evidence levels do not establish finished exports or auditioned sound.
- The existing native examples in src/godot-game/proofs/ProofResource.cs, LifecycleFixture.cs and LifetimeToken.cs demonstrate serialized-resource validation, real engine callbacks and closing result acceptance before cancellation/teardown. Reuse the applicable conventions without treating these fixtures as content/gameplay implementations.

### Established Patterns

- Strict C# rules, project/type boundaries, main-thread engine access, scoped compatibility and focused documentation already constrain later content/inspection code. Respect the approved project inventory; this discussion does not authorize a new project topology or blanket exemptions.
- Preserve pixels during extraction and use nearest-neighbor presentation. The donor's source pixels/original world units, the new game's design units and final display dimensions are separate; D-13 does not silently inherit the donor's 32-source-pixel world-unit convention.
- Build reusable body/equipment/action composition with actual state and stable metadata. One icon per inventory item is not permission to replace worn/held/corpse appearances with icons.
- No codebase maps, packaged spike/sketch findings or matching pending todos were found for this discussion.
- Phase 1 remains in progress at capture time. Phase 2/3 context exists, but proof fixtures and planning documents do not establish their implemented production behavior.

### Integration Points

- Add the content pack and inspection entry points within the existing game project, independently accessible from the minimal proof scenes. The creator selected separate resource/inspection delivery; replacing the Phase 2/3 placeholders is later integration.
- Connect the complete obligation-to-output manifest to actual packaged images, composition data, fonts, text, audio, authored baseline data and inspection entries. Expose exact failures while leaving independent valid entries inspectable.
- Route the live character controls through one current body/equipment/action composition, clearing replaced visuals immediately and permitting the clean animation restarts already allowed by the specification.
- Use the same production resources in the structure/world/map, UI/legacy/current-state, audio and bounded crowd/persistent-corpse fixtures. Current-state reconstruction does not replay old one-shot events.
- Retain selected source inputs and reproducible recipes locally. Export only selected dependencies, preserve origins and distinguish actual production validation from source inspection.
- Supply the focused Windows import/export/launch/content/visual/audio evidence required by the phase specification. Later gameplay integration, Linux/release certification, reference-PC performance and real-duration endurance keep their existing owners.

</code_context>

<specifics>
## Specific Ideas

- The creator chose all four discussion areas, requested two additional animation rounds and two additional visual rounds, then explicitly chose to save the phase context and discussion log.
- "Use built-in tools only" is the animation-tool boundary.
- "Arms do most of the work" is the bow-aiming preference; melee still uses shoulders/torso for weight.
- "Don't make multiple icons for the inventory. There will be durability numbers or something. so not care right now" limits inventory-icon variant work without selecting a specific condition indicator.
- Preserve the deliberately different emphasis of very minimal visual impact accents and punchier sound effects.

</specifics>

<deferred>
## Deferred Ideas

None — no new capability or backlog item was proposed. Exact durability-indicator design is a planning detail within the existing required UI, not a deferred feature. The specification's pre-existing bounded cosmetic-defect policy and later gameplay/release responsibilities remain unchanged.

</deferred>

---

*Phase: 04-asset-production*
*Context gathered: 2026-11-14 (Europe/Paris)*
