# Phase 4: Asset production — Specification

**Created:** 2026-11-14
**Ambiguity score:** 0.016 (gate: ≤ 0.20)
**Requirements:** 5 locked

## Goal

Deliver all 180 required resource obligations as a traceable, self-contained Godot content pack and Windows inspection build, including reusable procedural character animation, live character-combination inspection, and the explicitly bounded cosmetic-defect exception below.

## Background

The repository contains the Godot/C# development foundation and native proof scenes, reference images under examples, and candidate zombie idle/walk/attack artwork. The Phase 2 and Phase 3 specifications deliberately permit basic placeholders; they are not completed production art. No complete game-asset production manifest, fitted modular character library, full resource gallery, or validated Windows inspection export exists at specification time. Code-rule donor provenance is not a game-content manifest.

The supplied Kingdom Two Crowns recovery root is available at D:/Work/Games/KindomCrown/recovered. Its catalog, decoded metadata, and sampled visual evidence identify candidates, not completed exports or an auditioned sound bank. Selected resources still need extraction, fitting, conversion, assembly, generation of gaps, and production evidence. Preserve the recovered originals.

Scope authority is the [project definition](../../PROJECT.md), [approved requirements](../../REQUIREMENTS.md), [roadmap](../../ROADMAP.md), [game specification](../../../specs/0.1/initial-spec.md), and [asset/resource plan](../../../specs/0.1/assets-and-resources.md). The asset plan supplies the complete CH/AN/IT/BL/EN/FX/UI/AU/DT/TXT register and donor locators. The creator's phase-specific decisions recorded here govern the delivery boundary: static UI layouts/state examples, assembled-map inspection, a Windows inspection build with focused checks, and documented deferral of only minor cosmetic flaws. Full game delivery still targets Windows and Linux.

## Requirements

1. **ASSET-01 — Traceable source and production identity:** Every selected recovered asset or reused logic unit is traceable to a preserved original and a versioned, stable project output through its exact source identity, transformations or Codex/GPT generation recipe, and recorded validation evidence.
   - Current: The asset plan identifies donor groups, exact sampled locators, source preservation identities, and V/M/C evidence levels. Local examples exist, but there is no complete production manifest tying all selected game resources to final outputs.
   - Target: A production manifest records each output's independent stable ID, project-relative path, version and content identity; exact donor owning file/type/object ID/name and preserved-original identity where applicable; selected dependencies; export/transformation or generation recipe; dimensions/alpha or audio/animation metadata as applicable; and actual validation/approval status. Reused logic retains its origin and Godot adaptation record. Generated sources and recipes are retained with the project.
   - Acceptance: AC-02, AC-03 and AC-22 pass; the ASSET-01 held-out backstops in Edge Coverage are executed and reported. A reviewer can follow an output back to the specific preserved source or Codex/GPT generation record without treating duplicate donor names as sufficient identity.

2. **ASSET-02 — Complete reuse-first resource pack:** All 180 CH/AN/IT/BL/EN/FX/UI/AU/DT/TXT obligations map to actual complete or explicitly shared outputs, using every suitable Kingdom Two Crowns donor first and Codex/GPT generation for remaining gaps, subject only to the recorded minor-cosmetic deferral policy.
   - Current: The 180-obligation production register and the example/candidate art exist. The complete exported, adapted, generated, and imported resource set does not.
   - Target: Every obligation in the census below has actual usable outputs or an explicitly declared shared output. Reuse all suitable Kingdom Two Crowns material; adapt useful donor parts to the required materials, silhouettes and states. Generate remaining gaps with Codex/GPT, including code-generated resources where appropriate. Optional extra skins, weather, landmarks or other polish do not become mandatory obligations.
   - Acceptance: AC-01, AC-05, AC-09, AC-18, AC-21, AC-22 and AC-23 pass; all ASSET-02 held-out backstops pass. Missing required resource families, missing files, or unreadable required equipment/actions cannot use the minor-cosmetic exception.

3. **ASSET-03 — Composable presentation and live inspection:** The Godot inspection scenes demonstrate every required character, equipment, action, corpse, structure, portal, and environment state, with live body/gear/action controls and procedural composition that keeps walking active during melee, bow aiming, and firing.
   - Current: Existing zombie frames are candidates, and the Godot scenes are foundation proofs. There is no validated body/equipment fitting library, composed movement/combat presentation, complete structure/state gallery, or live character inspector.
   - Target: Required movement, combat, work, equipment and corpse appearances are reusable components composed procedurally in Godot. Walking continues during melee attacks, bow aiming and firing. The live inspector switches bodies, equipment, movement and attacks, applies changes immediately, and clears replaced visuals; a clean animation restart is acceptable. Inspect all required body shapes, directions, hand combinations, state transitions, structures and environmental resources without generating a full animation sheet for every equipment/action combination.
   - Acceptance: AC-04 through AC-09, AC-14, AC-18, AC-21 and AC-24 pass; all ASSET-03 held-out backstops pass. A fixed sequence of flattering presets alone does not satisfy the live character-inspection requirement.

4. **ASSET-04 — Inspectable UI, text, data and sound:** All required UI layouts and state examples, English text, authored data and compatibility metadata, and auditioned sound/ambience event resources are inspectable without implementing their later gameplay interactions.
   - Current: Required screens, texts, configuration domains, sounds and bindings are described in the specifications. Source fonts/audio are candidates or decoded metadata; no complete inspected and auditioned pack is present.
   - Target: UI-01–UI-27 are reusable visual layouts and normal/exceptional state examples, not flattened screenshots that lose layout/state information. TXT-01/02, all DT-01–DT-27 packs, field documentation, stable content/visual-state metadata and AU-01–AU-30 sound/ambience resources are complete. Audio is actually auditionable through documented event bindings. UI interactions, inventory transactions and other real game rules are implemented in their later gameplay phases.
   - Acceptance: AC-10 through AC-14 and AC-24 pass; all ASSET-04 held-out backstops pass. Inspection demonstrates required visual states, longer text, local preferences, legacy representations and sound routing without claiming the corresponding gameplay systems are implemented.

5. **ASSET-05 — Portable content and focused Windows delivery:** A self-contained Godot project and Windows inspection build load and expose the complete resource pack without external recovered folders or absolute development paths, report exact resource failures while leaving unaffected entries inspectable, and pass focused Windows content smoke checks.
   - Current: The project launches Phase 1 proof scenes; the complete resource gallery and Windows inspection build do not exist. The external recovery folder is an authoring input.
   - Target: The repository contains the selected production inputs, content, metadata, documentation and runnable inspection scenes, plus a Windows inspection build. Runtime/inspection resource resolution uses packaged project content. Missing or invalid resources identify the exact problem, mark the pack incomplete, and leave unaffected entries inspectable. Obtain focused Windows import/export/launch and content evidence; broad build matrices and Linux execution are later-phase work.
   - Acceptance: AC-15 through AC-17 pass and all ASSET-05 held-out backstops pass. Provenance may name an absolute original-source location, but that location cannot be a runtime or clean-content-check dependency.

### Complete resource census

These are obligation counts, not counts of files, frames, atlases, recordings, or unique illustrations. Shared outputs must be explicit.

| Register | Required IDs | Obligations |
| --- | --- | ---: |
| Character families | CH-01–CH-13 | 13 |
| Actions/states | AN-01–AN-18 | 18 |
| Items | IT-01–IT-24 | 24 |
| Buildings/sites | BL-01–BL-16 | 16 |
| Environment/world | EN-01–EN-11 | 11 |
| Effects/materials/projectiles | FX-01–FX-12 | 12 |
| UI resources/layouts | UI-01–UI-27 | 27 |
| Audio categories | AU-01–AU-30 | 30 |
| Authored data/support packs | DT-01–DT-27 | 27 |
| Text packs | TXT-01–TXT-02 | 2 |
| **Total** | **All registered obligations** | **180** |

The required roster remains one reusable survivor presentation, seven distinct zombie silhouettes with all applicable equipment, structural tower guards, rabbit/deer, four player identities, ordinary equipped corpses and loot piles. Additional survivor faces/skins are optional under the asset plan; the creator answered “all required” rather than requesting an additional cosmetic roster.

## Boundaries

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

## Constraints

- **Approved content and baselines:** The game specification controls behavior and exclusions. Its proposed counts/values remain the working configurable baseline, not proven balance. The asset plan controls the resource register and required combinations. Preserve the correct wood/reinforced-wood/metal materials and this game's silhouettes rather than importing incompatible donor tier numbers, fantasy actors or mechanics.
- **Reuse and generation:** Reuse every suitable Kingdom Two Crowns donor, including appropriate parts of an adapted output. Record why a relevant inspected candidate is unsuitable when a gap must be generated. Preserve original recovered files. Codex/GPT produces the remaining content; shared geometry, masks, text, particles and synthesized gaps may be generated with code.
- **Animation composition:** Required locomotion and attacks combine in Godot; no forced stop while walking and performing melee, aiming or firing. Maintain logical hand identities across facing changes, real equipment and ammunition state, correct layering, native pixel clarity and authored timing. The specific rig/composition features, add-on, file formats, layout and implementation remain discuss/plan decisions.
- **Optional paid dependency:** A paid animation add-on may be proposed with its cost and compatibility/inclusion implications. No add-on is selected by this specification and no purchase is authorized. Any selected solution must work within the established Godot/C# project and self-contained Windows inspection delivery.
- **UI and map boundary:** UI resource completeness means layouts/state examples; it does not require interactive inventory/game screens in Phase 4. Map completeness means the assembled authored map and its resources/metadata; it does not require playable traversal. The character inspector is expressly live.
- **Windows-focused evidence:** Produce the Windows inspection build and bounded import, launch, content, visual and audio checks. Use meaningful held-out cases without requiring an exhaustive cartesian combination run, a broad build matrix or release-performance/endurance certification. Full game Windows/Linux support remains required in later delivery.
- **Cosmetic exception:** Only minor cosmetic flaws in documented uncommon combinations may be deferred. Required gear, actions and critical states must remain readable. Missing resources, absent required content, detached/hidden equipment that prevents reading the state, or a must-NOT violation are not permitted cosmetic deferrals. Each issue names the first relevant integration owner: Phase 6 for characters/inventory/combat, Phase 7 for camp/buildings, Phase 8 for zombies/campaign. Deferral is not permission to leave the issue through release.
- **Presentation/data:** Preserve the specified 1280×720 and 1920×1080 layouts and 80–150% UI range, four identities readable without color alone, English catalog and longer-string accommodation, eleven slots/four quick references, independently editable zombie progression and documented units/defaults/references. Configure audio limits from the baseline rather than hardcoding hidden balance.
- **Persistent appearances:** Corpses and belongings have separate lifetimes from blood/acid/particles. Content reconstruction restores current gear, ruins, work and loop state without replaying past death/destruction/payment/recruitment/warning effects. Preview controls may intentionally play an effect; restoring a state is not such a request.
- **Project conventions:** Follow the [architecture index](../../../docs/architecture/README.md), [code index](../../../docs/code/README.md) and [verification reference](../../../docs/workflow/verification.md) when later implementing the pack and checks. This specification does not certify completion of any implementation phase.

## Acceptance Criteria

- [ ] **AC-01 — Complete census (ASSET-02):** Every one of the 180 registered IDs maps to actual inspectable outputs or explicitly declared shared outputs. All required resource families exist; file/atlas/recording counts do not substitute for obligation coverage.
- [ ] **AC-02 — Exact origins (ASSET-01):** Each selected recovered output/logic unit identifies its owning source file, type, object ID/name or original code unit, preserved-original identity and selected dependencies. Original recovered files remain unchanged.
- [ ] **AC-03 — Reusable production records (ASSET-01):** Stable output IDs, relative paths, versions/content identities, transformations or Codex/GPT generation sources/recipes and applicable image/audio/animation metadata are recorded. Actual production validation is distinguished from original V/M/C inspection evidence.
- [ ] **AC-04 — Live composed animation (ASSET-03):** In the Godot character inspector, the reviewer switches bodies, equipment, movement and attacks live. Walking continues during melee, bow aim and firing. Changes appear immediately, clear old visuals and may restart the current animation cleanly. Required combinations do not depend on separately baked full-combination sheets.
- [ ] **AC-05 — Required body/equipment coverage (ASSET-02/03):** The survivor and all seven recognizable zombie silhouettes support every applicable item, direction and required AN action, including wide bodies, clothing, both armor types, belt, gloves, boots and both backpacks. All 24 item definitions have identifiable icons and applicable worn/held forms. Only AC-18's bounded cosmetic exceptions may remain.
- [ ] **AC-06 — Hands, ammunition and condition (ASSET-03):** Fixtures show fists, either-hand one-handed weapons, mixed dual attacks, bow/arrow states, empty/changed arrow hands and broken/fallback gear. Facing changes preserve logical inventory hand identity; visible equipment/ammunition reflects the selected sample state.
- [ ] **AC-07 — Corpses and loot (ASSET-03):** Ordinary survivor/zombie and rabbit/deer corpse endpoints remain inspectable/selectable after effects fade. Changing the sample remaining equipment updates the corpse appearance, crown loss is represented, and nonempty/empty/selected states are distinguishable. Structural guards produce no character corpse or item loot.
- [ ] **AC-08 — Buildings and portals (ASSET-03):** Fixtures show all three main/wall/tower tiers and required site, frame, upgrade, damage, repair, destruction, previous-tier/rebuild and permanent portal-ruin states. Standalone tiers show 1/2/3 independently animated guards; main tier 3 has exactly two one-guard attachments. Upgrades preserve the visible old functional tier until completion.
- [ ] **AC-09 — World and crowd resources (ASSET-02/03):** The assembled map contains the baseline six portals, eight wall/eight standalone-tower sites, clearing groups, six food plants, wildlife zones, terrain and interaction/navigation/collision metadata. Day/night, plant/clearing/prey/effect states are inspectable. Short crowd/persistent-corpse fixtures use the real presentation resources for approximately 100 survivors/300 zombies and the later ≥200 nonempty-corpse/pile scenario; no FPS or three-hour endurance pass is inferred.
- [ ] **AC-10 — UI state resources (ASSET-04):** UI-01–UI-27 and their required normal, empty, waiting, disabled, invalid, conflicting, legacy, lost-access and failure examples are present as reusable layouts/state resources. These examples can be inspected without implementing their future gameplay interaction logic.
- [ ] **AC-11 — Text, scale and identity (ASSET-04):** TXT-01/02 cover all specified labels/descriptions/prompts/errors and preserve formatting placeholders/current-binding references. Layout examples demonstrate supported resolutions/scales, longer replacement text, readable names/numbers/counts and four identities distinguishable beyond color. Held-out display/text cases are carried by AC-19.
- [ ] **AC-12 — Auditioned sound (ASSET-04):** AU-01–AU-30 map to auditioned recordings or generated replacements with documented event routing, duration/channel/rate/trim/loop/gain and mix/priority/repetition settings. Day/night and local ambience can be auditioned, baseline 24/four limits are configurable, and critical warnings have visible examples when muted.
- [ ] **AC-13 — Complete data/support (ASSET-04):** DT-01–DT-27 are present with the complete working baseline, documented meanings/units/defaults/allowed values/dependencies and representative editing examples. Item/content/image/sound/text references validate, zombie progression is independently editable, and local preferences are separated from host balance.
- [ ] **AC-14 — Compatibility presentation (ASSET-03/04):** Stable IDs/versions and sample saved-layout/appearance metadata support inspectable old grid dimensions/placements/orientation, incompatible slots/stacks/depth, current-destination previews and explicit missing-definition errors. Fixture restoration shows persistent gear/corpses/ruins/loops without replaying past one-shot effects; actual gameplay save/load and extraction remain later integration.
- [ ] **AC-15 — Windows content delivery (ASSET-05):** The Godot project, inspection scenes and Windows inspection build load their selected content, fonts, text, data and audio without the external recovery folder or absolute development-path dependencies. Source-locator text in provenance does not create a runtime dependency.
- [ ] **AC-16 — Visible incomplete state (ASSET-05):** Deliberately absent/invalid required resources identify the exact resource ID/path and problem, mark the pack incomplete and leave unaffected gallery entries inspectable. A fallback display cannot turn a missing required output into completed coverage.
- [ ] **AC-17 — Focused current evidence (ASSET-05):** Documented focused Windows import/export/launch, content, visual and audio checks provide current results and required diagnostic evidence. Missing evidence is recorded as such. Linux execution, broad build matrices, full gameplay and release performance/endurance are not claimed from this inspection run.
- [ ] **AC-18 — Bounded cosmetic carryover (ASSET-02/03):** Every remaining known issue is a reproducible minor cosmetic flaw in an exactly documented uncommon combination; all required gear/actions/critical states remain readable. The register records expected/actual results and a Phase 6/7/8 owner as specified above. A missing resource or non-cosmetic required-state failure blocks Phase 4 completion.
- [ ] **AC-19 — Held-out edges (ASSET-01–05):** Planning carries all 34 Edge Coverage rows into non-inferable must-have checks, and Phase 4 obtains held-out evidence for them. Inputs/combinations are chosen independently of authored happy-path examples. Cases may share setup/evidence when their individual obligations remain identifiable; these are not 34 separate build runs. Any unsupported backstop remains a visible verification gap.
- [ ] **AC-20 — Inspection boundary (ASSET-03–05):** The delivered resource index and inspection entry points expose the live character controls, assembled map, UI state examples, data and audio described here. Resource inspection is clearly identified; the pack's completion record does not claim implementation of the later gameplay interactions or a complete playable campaign.
- [ ] **AC-21 — Blood limit (ASSET-02/03, prohibition):** Hit/death presentation uses restrained blood effects and MUST NOT include dismemberment. Verify by recorded visual judgment across the relevant effect/death families.
- [ ] **AC-22 — Honest provenance/status (ASSET-01/02, prohibition):** Production records MUST NOT present catalog entries/metadata/unfinished exports as completed assets or label generated/adapted material as unchanged donor reuse. Verify through recorded source/output and evidence-status review.
- [ ] **AC-23 — Game identity (ASSET-02/04/05, prohibition):** Title, icon and UI identify KingdomZombies and MUST NOT use Kingdom Two Crowns or publisher logos as the game's identity. Verify by recorded visual review.
- [ ] **AC-24 — Truthful mechanic presentation (ASSET-03/04, prohibition):** Art/UI MUST NOT imply recoverable fired arrows, lootable dropped crowns, structural-guard loot, damaging bloater acid or an active-game menu that pauses play. Donor visuals follow KingdomZombies' specified mechanics. Verify by recorded visual/content review.

## Edge Coverage

**Coverage:** 34/34 applicable edges resolved · 0 unresolved.
**Verification:** 0 explicit edge resolutions · 34 backstops. This records verification obligations, not already-passing tests.

The compiled edge-probe engine ran over the five requirement statements above and proposed 32 rows, with zero unclassified requirements. Authored shapes were ASSET-01: collection/text/io; ASSET-02: numeric-range/collection/io; ASSET-03: numeric-range/collection/stateful; ASSET-04: all five shapes; ASSET-05: collection/text/stateful/io. Manual review added repeat-export/coverage idempotency for ASSET-01 and ASSET-02. Every engine row is retained. The creator selected held-out tests for all three grouped probes.

Every row has status resolved and verification backstop. AC-19 requires plan-phase to preserve the row as a held-out, non-inferable check; a missing test/evidence later is a verification gap. Table intents guide test selection without claiming that every unarticulated detail is already an explicit requirement. Existing source requirements and interview decisions still apply.

| Category | Requirement | Status | Verification | Held-out backstop intent / acceptance link |
| --- | --- | --- | --- | --- |
| adjacency | ASSET-01 | resolved | backstop | H01: Same-named donors in different owning files/types and explicitly shared outputs retain the right source/output identities; AC-02/03/19. |
| empty | ASSET-01 | resolved | backstop | H02: A selected output with missing origin, identity, recipe or required evidence cannot obtain completed provenance; AC-03/19. |
| encoding | ASSET-01 | resolved | backstop | H03: Case/encoding variants cannot silently alias stable IDs/source locators or portable paths; AC-02/03/19. |
| ordering | ASSET-01 | resolved | backstop | H04: Reordering donor/manifest enumeration preserves exact output-to-origin resolution; AC-02/03/19. |
| concurrency | ASSET-01 | resolved | backstop | H05: Interrupted/competing production work preserves originals and cannot publish unvalidated output as verified; AC-02/03/19. |
| boundary | ASSET-02 | resolved | backstop | H06: A missing unique obligation remains a failure even when duplicate rows inflate the apparent total to 180; AC-01/19. |
| adjacency | ASSET-02 | resolved | backstop | H07: Explicitly shared files may satisfy several obligations without collapsing distinct item/resource identities; AC-01/19. |
| empty | ASSET-02 | resolved | backstop | H08: Empty/partial packs and absent or empty required output dependencies cannot count as completed coverage; AC-01/19. |
| ordering | ASSET-02 | resolved | backstop | H09: Shuffling resource/mapping order preserves the actual covered-ID set and completeness result; AC-01/19. |
| precision | ASSET-02 | resolved | backstop | H10: Unique integer obligation coverage is exact; percentages, duplicate files or rounding cannot certify an incomplete pack; AC-01/19. |
| concurrency | ASSET-02 | resolved | backstop | H11: Output generation/import interrupted partway through remains visibly incomplete rather than gaining metadata-only coverage; AC-01/16/19. |
| boundary | ASSET-03 | resolved | backstop | H12: Empty/last-arrow and broken/usable transitions, required tier/guard counts and persistent-corpse boundaries preserve the intended presentation; AC-06/07/08/19. |
| adjacency | ASSET-03 | resolved | backstop | H13: Overlapping coat/armor/belt/bag/hand layers and nearby bodies keep required gear and target identity readable; AC-05/07/18/19. |
| empty | ASSET-03 | resolved | backstop | H14: Empty hands/ammunition/slots, empty versus nonempty corpses and the structural-guard exception do not invent equipment or loot; AC-06/07/19. |
| ordering | ASSET-03 | resolved | backstop | H15: Facing/action/gear order changes retain correct final equipment, occlusion and logical hand identity; AC-04/05/06/19. |
| precision | ASSET-03 | resolved | backstop | H16: Donor pixel registration, offsets, nonuniform frame timing, held/repeated frames and loop/endpoints survive conversion/composition; AC-03/04/05/19. |
| idempotency | ASSET-03 | resolved | backstop | H17: Repeated preview reset/state restoration leaves one current composition/loop/corpse state without replaying past one-shots; AC-04/07/14/19. |
| concurrency | ASSET-03 | resolved | backstop | H18: Mid-action body/gear changes immediately clear replaced visuals, permit a clean restart and retain concurrent walking/combat presentation; AC-04/19. |
| boundary | ASSET-04 | resolved | backstop | H19: Supported resolution/UI-scale endpoints, expanded text and configured sound limits remain inspectable; AC-11/12/19. |
| adjacency | ASSET-04 | resolved | backstop | H20: Equal labels/values and touching/overlapping UI or legacy-grid regions do not merge identities or hide critical warning/state information; AC-10/11/14/19. |
| empty | ASSET-04 | resolved | backstop | H21: Empty/zero-result examples and missing/invalid text, audio or data refs show the right state/diagnostic without a false resource pass; AC-10/11/12/13/19. |
| encoding | ASSET-04 | resolved | backstop | H22: Catalog encoding, exact IDs/placeholders, accented names/font fallback and longer strings remain readable without corrupting text identity; AC-11/19. |
| ordering | ASSET-04 | resolved | backstop | H23: Reordered data/definitions retain bindings and quantities; warning layers and four-player identities remain correct across UI state examples; AC-10/11/13/19. |
| precision | ASSET-04 | resolved | backstop | H24: Display scaling, formatting and media conversion do not silently change authored units/defaults, quantities, timing or sound metadata; AC-03/11/12/13/19. |
| idempotency | ASSET-04 | resolved | backstop | H25: Repeated UI/audio/current-state example restoration does not accumulate ambience loops or replay old transactional/death/warning cues; AC-12/14/19. |
| concurrency | ASSET-04 | resolved | backstop | H26: Rapid view/audition/state changes leave correct bindings/current loops and visible muted warnings, without requiring later gameplay interactions; AC-10/12/14/19. |
| adjacency | ASSET-05 | resolved | backstop | H27: Similar/conflicting resource paths are diagnosed accurately; one bad entry does not conceal independent valid entries; AC-15/16/19. |
| empty | ASSET-05 | resolved | backstop | H28: An empty or partially missing pack/gallery cannot obtain a successful content result through zero discovery or fallback art; AC-16/17/19. |
| encoding | ASSET-05 | resolved | backstop | H29: Packaged path/ID spelling and readable exact diagnostics do not depend on development-path aliases or encoding loss; AC-15/16/19. |
| ordering | ASSET-05 | resolved | backstop | H30: Validation/discovery order cannot suppress known resource failures or prevent inspection of unaffected entries; AC-16/19. |
| idempotency | ASSET-05 | resolved | backstop | H31: Repeated clean launch/close/relaunch uses the same packaged content and does not require hidden donor/editor caches; AC-15/17/19. |
| concurrency | ASSET-05 | resolved | backstop | H32: Partial imports, interrupted checks or failed launch attempts cannot inherit an earlier successful result; unaffected content remains inspectable where the build can run; AC-16/17/19. |
| idempotency | ASSET-01 | resolved | backstop | H33 (manual): Re-export/re-import preserves stable selected-output identities without duplicate provenance records; AC-03/19. |
| idempotency | ASSET-02 | resolved | backstop | H34 (manual): Recomputing or regenerating coverage cannot count one obligation twice or change completeness solely through repetition; AC-01/19. |

## Prohibitions (must-NOT)

**Coverage:** 4/4 applicable prohibitions resolved · 0 unresolved.
**Verification:** 0 test-tier · 4 judgment-tier.

The recall/precision pass considered all five requirements. Routine completeness, path/reference correctness and stale-state candidates are owned by the edge checks. The retained bespoke constraints concern truthful origins/status, this game's identity, truthful mechanic presentation and the creator's blood-content limit. These require recorded provenance/visual judgment; no mechanical check descriptor or fixture path is fabricated.

| Prohibition (must-NOT statement) | Requirement | Status | Verification / Reason |
| --- | --- | --- | --- |
| MUST NOT present catalog entries/metadata/unfinished exports as completed assets, or label generated/adapted material as unchanged donor reuse. | ASSET-01, ASSET-02 | resolved | verification: judgment; AC-22; creator accepted provenance/status review in the prohibition probe. |
| MUST NOT use Kingdom Two Crowns or publisher logos as the title/icon/UI identity of KingdomZombies. | ASSET-02, ASSET-04, ASSET-05 | resolved | verification: judgment; AC-23; creator accepted source/identity limits. |
| MUST NOT imply recoverable fired arrows, lootable dropped crowns, structural-guard loot, damaging bloater acid or an active-game menu that pauses play. | ASSET-03, ASSET-04 | resolved | verification: judgment; AC-24; creator accepted mechanic-presentation review. |
| MUST NOT include dismemberment; blood presentation remains restrained. | ASSET-02, ASSET-03 | resolved | verification: judgment; AC-21; already resolved by the creator in interview round 1. |

## Ambiguity Report

| Dimension | Score | Min | Status | Notes |
| --- | ---: | ---: | --- | --- |
| Goal Clarity | 0.98 | 0.75 | ✓ | Complete census, reusable composition and inspection deliverables are fixed. |
| Boundary Clarity | 0.99 | 0.70 | ✓ | Static UI/map inspection, live character controls, Windows build and later gameplay integration are explicit. |
| Constraint Clarity | 0.99 | 0.65 | ✓ | Reuse/generation priority, animation overlap, paid-proposal boundary and cosmetic limits are fixed. |
| Acceptance Criteria | 0.98 | 0.70 | ✓ | Pass/fail checks, 34 held-out backstops and four judgment prohibitions have defined owners/intent. |
| **Ambiguity** | **0.016** | **≤0.20** | **✓** | Exact calculation: 1 − (0.35×0.98 + 0.25×0.99 + 0.20×0.99 + 0.20×0.98) = 0.0155; displayed to three decimals. |

No dimension is below minimum. The edge backstops represent deliberately delegated test detail, not unresolved scope or already-executed proofs.

### Assessment history

| Assessment | Goal | Boundaries | Constraints | Acceptance | Ambiguity | Outcome |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Initial roadmap/requirements | 0.90 | 0.85 | 0.70 | 0.65 | 0.203 | Acceptance below minimum; interview required. |
| After round 1 | 0.95 | 0.92 | 0.85 | 0.88 | 0.092 | Gate passed; creator requested another round. |
| After round 2 | 0.97 | 0.96 | 0.94 | 0.95 | 0.043 | Gate passed; creator requested another round. |
| After round 3 | 0.98 | 0.99 | 0.97 | 0.96 | 0.024 | Gate passed; creator requested failure analysis. |
| After round 4 | 0.98 | 0.91 | 0.96 | 0.66 | 0.106 | Acceptance below minimum because “uncommon defects” was not yet bounded. |
| After round 5 | 0.98 | 0.99 | 0.97 | 0.98 | 0.020 | Gate passed; creator requested the final round. |
| After round 6 | 0.98 | 0.99 | 0.99 | 0.98 | 0.016 | Gate passed; creator explicitly selected edge checks and writing SPEC.md. |

## Interview Log

| Round | Perspective | Question summary | Decision locked |
| --- | --- | --- | --- |
| 1 | Researcher | Required finish, character variety and blood detail? | Complete all described art. Use Godot procedural/composable animations to avoid generating every equipment/action combination. Cover all required characters; no additional optional cosmetic roster requested. Restrained blood, no dismemberment. |
| 2 | Researcher / Simplifier | Movement during attacks, live inspector and foundation replacement? | Walking continues during melee, bow aiming and firing. Switch bodies/gear/movement/attacks live. Deliver the pack/inspection scenes; integrate into actual gameplay later. Reuse all suitable Kingdom Two Crowns material and generate the rest with Codex/GPT. |
| 3 | Boundary Keeper | UI interaction, map traversal and platform delivery? | UI layouts/state examples only; interactive behavior later. Inspect the assembled map/resources; traversal later. Windows inspection build with limited build testing; broader/Linux checks later. |
| 4 | Failure Analyst | Missing resources, combination defects and mid-action changes? | Exact resource diagnostics mark the pack incomplete while unaffected entries remain inspectable. Documented uncommon visual defects may carry forward. Inspector changes apply immediately, clear old visuals and may restart animation cleanly. |
| 5 | Seed Closer | What defects may carry, and until when? | Narrow round 4's exception to minor cosmetic flaws only; gear/actions/critical states remain readable. Fix in the first relevant Phase 6/7/8 integration, with exact affected combinations and reproduction. |
| 6 | Seed Closer | Add-on budget and final additions? | Paid animation add-ons may be proposed with cost; no purchase authorization. No additional scope. Explicit approval to proceed through edge checks and write SPEC.md. |
| Edge probe | Boundary / failure completeness | Resource, animation/state and UI/audio/data edges? | Creator selected held-out tests for details not fully specified in all three groups. Retain all 32 engine rows plus two manual repeat-operation rows as backstops, with no unresolved rows. |
| Prohibition probe | Must-NOT review | Source/status/identity and misleading mechanic presentation? | Keep all source/identity and mechanic-presentation limits as review criteria. Blood limit was already explicit in round 1. Four judgment-tier prohibitions resolved. |

---

*Phase: 04-asset-production*
*Spec created: 2026-11-14*
*Next step: $gsd-discuss-phase 4 — implementation decisions for the locked requirements above, including procedural animation and the reuse/generation approach.*

