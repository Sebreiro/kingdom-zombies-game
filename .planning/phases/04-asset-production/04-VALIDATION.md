---
phase: "04"
slug: asset-production
status: draft
nyquist_compliant: false
wave_0_complete: false
created: 2026-11-16
---

# Phase 04 — Validation Strategy Draft

This is a planning draft derived from 04-SPEC.md, 04-UI-SPEC.md, 04-RESEARCH.md and the current local-testing policy. No Phase 4 acceptance check, hidden backstop, audio listening or judgment is reported passed. Extracted donor feasibility evidence is separate and supplies no content-completion credit. [CITED: 04-SPEC.md AC-17/19/22; docs/workflow/local-testing.md]

## Execution boundary

1. Complete the whole phase using source-only, pure, repository, headless import/native and export checks.
2. Review the complete phase code independently and fix findings before any real game window.
3. Declare the current active Critical + new + changed + affected regression selection, explicit omissions and ordered game-run groups.
4. Run the grouped selection on Windows with every routine real window at 1280×720, no VM.
5. Collect failures while continuing independent cases, batch repairs, execute focused failed/blocked/newly affected checks, then rerun the entire selected set on the repaired build.
6. Perform actual appearance/provenance judgments and actual sample-specific audio listening. Preserve all initial/focused/final reports, tested revisions, hashes and images. An initially clean selected-set run needs no duplicate run.

[CITED: docs/workflow/local-testing.md; docs/workflow/verification.md]

1920×1080 and UI-scale endpoints are authored fixture canvases inspected/calculated within the 1280×720 hub. Those results are not real 1920×1080-window, Linux, reference-PC performance, physical LAN or three-hour endurance evidence. [CITED: 04-UI-SPEC.md Display, Text and Accessibility Fixtures]

## Existing runnable feedback

| Existing command | What it establishes | Window policy |
|---|---|---|
| `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` | Strict whole solution compilation/analyzers after isolated setup/restore | No window |
| `node tools/phase1/check.cjs --group docs` | Existing navigation/portable docs checks | No window |
| `node tools/phase1/check.cjs --group rules` | Current code rules | No window |
| `node tools/phase1/check.cjs --group graph` | Current project/type policy | No window |
| `node tools/phase1/check.cjs --case engine.source-policy` | Current exact native access policy | No window |
| `node tools/phase3/check.cjs --list` | Existing exact catalog listing | No execution |
| `node tools/phase3/check.cjs --group core` | Existing foundation pure regressions | No window |
| `node tools/phase3/check.cjs --group adapters` | Existing foundation socket/file regressions | No window |
| `node tools/phase3/check.cjs --group network` | Existing foundation native headless subjects | No window |
| `node tools/phase3/check.cjs --group runner` | Existing runner integrity | No window |
| `node tools/phase3/check.cjs --group rendered` | Existing grouped foundation native cases only when current review/selection identities validate | After whole-phase review only |

These commands are documented in the existing workflow references. None currently implements Phase 4 acceptance. Phase 3's historical review/fingerprint cannot approve changed Phase 4 code; plan current source-bound review/selection support without weakening or editing historical results. [CITED: docs/workflow/verification.md; docs/workflow/lan-foundation-checks.md]

## Required new validation tooling

Proposed Phase 4 content/composition/layout/export/rendered/completion interfaces in 04-RESEARCH.md are design recommendations and do not exist yet. Plans must implement them before listing them as runnable proof. No priority selector, reuse certification or Phase 5 framework is implied. [CITED: 04-RESEARCH.md Validation Architecture; docs/workflow/local-testing.md]

| New check family | Required subject | Automation versus judgment |
|---|---|---|
| Census/provenance | Independent exact 180 expected IDs; usable files/dependencies; origin/recipe/output/hash/evidence; empty/duplicate/order/interruption/re-export | Pure/file checks plus actual provenance judgment |
| Media extraction | Alpha/native logical registration, palettes, font bytes/glyphs, runtime clip tracks/events, full audio sample/rate/length/hash | Source decode/headless; appearance/listening later |
| Composition | All body/action/gear applicability, actual item identity, left/right/facing, dual/bow/ammo/condition, wide fits, corpse endpoints | Pure/headless plus live controls and actual images |
| Structures/map | Every lifecycle/tier, guards and main 1+1, old tier during upgrades, permanent portal ruins, baseline map sites/metadata | Data/native resources plus rendered assembled-state review |
| UI/text/data | All layouts/state resources, all fields/defaults/units/references, English placeholders, legacy samples and endpoint canvas scales | Pure/layout/native plus readability judgments |
| Audio | All 30 event categories, variants/routing/buses/limits, trim/loops/gain and owned current loops | Native controls/metadata plus actual named listener verdicts |
| Portability | Matching templates, real .NET Windows export, packaged files and clean content loader, accurate isolated bad entries | Headless export plus real packaged launch/relaunch |
| Evidence integrity | Exact case/peer/image/audio/source/tool/revision identities, failures, current independent review, no stale success | Runner negative controls and immutable reports |

## Acceptance criteria — preserved verbatim

The following checkbox statements are copied from the specification and remain unchecked. They are obligations, not research results. [CITED: 04-SPEC.md Acceptance Criteria]

<!-- DATA_d7c5a412_START -->

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
<!-- DATA_d7c5a412_END -->

## Domain held-out backstops — all 34 preserved

Choose concrete inputs/combinations independently of happy-path production presets. Each H identity retains its own outcome/evidence. Shared setup does not combine cases into an omnibus pass. Source status "resolved/backstop" is an obligation classification, not execution status. [CITED: 04-SPEC.md AC-19]

<!-- DATA_e3f6b904_START -->
| Category | Requirement | Source status | Verification | Held-out intent / acceptance link |
|---|---|---|---|---|
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
<!-- DATA_e3f6b904_END -->

**Execution state for H01–H34:** Not executed.

## UI held-out backstops — all 70 identities

Stable proposed IDs preserve the exact source surface/category pair. The test inputs remain held out from authored happy paths; all current outcomes are Not executed. The intent column is copied verbatim from the UI contract. [CITED: 04-UI-SPEC.md UI Considerations]

<!-- DATA_f29d631a_START -->
| Proposed stable ID | Surface | Category | Current outcome | Original held-out intent |
|---|---|---|---|---|
| UIH.HUB-NAV.loading | HUB-NAV | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.HUB-NAV.error | HUB-NAV | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.HUB-NAV.overflow | HUB-NAV | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.HUB-NAV.long-text | HUB-NAV | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.HUB-REGISTER.empty | HUB-REGISTER | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.HUB-REGISTER.loading | HUB-REGISTER | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.HUB-REGISTER.error | HUB-REGISTER | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.HUB-REGISTER.populated | HUB-REGISTER | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.HUB-REGISTER.partial | HUB-REGISTER | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.HUB-REGISTER.overflow | HUB-REGISTER | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.HUB-REGISTER.zero-one-many | HUB-REGISTER | zero-one-many | Not executed | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UIH.HUB-REGISTER.long-text | HUB-REGISTER | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.CHARACTER-PREVIEW.empty | CHARACTER-PREVIEW | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.CHARACTER-PREVIEW.loading | CHARACTER-PREVIEW | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.CHARACTER-PREVIEW.error | CHARACTER-PREVIEW | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.CHARACTER-PREVIEW.populated | CHARACTER-PREVIEW | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.CHARACTER-PREVIEW.partial | CHARACTER-PREVIEW | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.CHARACTER-PREVIEW.overflow | CHARACTER-PREVIEW | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.CHARACTER-PREVIEW.long-text | CHARACTER-PREVIEW | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.WORLD-ITEM-PREVIEW.empty | WORLD-ITEM-PREVIEW | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.WORLD-ITEM-PREVIEW.loading | WORLD-ITEM-PREVIEW | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.WORLD-ITEM-PREVIEW.error | WORLD-ITEM-PREVIEW | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.WORLD-ITEM-PREVIEW.populated | WORLD-ITEM-PREVIEW | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.WORLD-ITEM-PREVIEW.overflow | WORLD-ITEM-PREVIEW | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.WORLD-ITEM-PREVIEW.long-text | WORLD-ITEM-PREVIEW | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.UI-COLLECTIONS.empty | UI-COLLECTIONS | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.UI-COLLECTIONS.loading | UI-COLLECTIONS | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.UI-COLLECTIONS.error | UI-COLLECTIONS | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.UI-COLLECTIONS.populated | UI-COLLECTIONS | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.UI-COLLECTIONS.partial | UI-COLLECTIONS | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.UI-COLLECTIONS.overflow | UI-COLLECTIONS | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.UI-COLLECTIONS.zero-one-many | UI-COLLECTIONS | zero-one-many | Not executed | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UIH.UI-COLLECTIONS.long-text | UI-COLLECTIONS | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.UI-FORMS-CONTROLS.empty | UI-FORMS-CONTROLS | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.UI-FORMS-CONTROLS.loading | UI-FORMS-CONTROLS | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.UI-FORMS-CONTROLS.error | UI-FORMS-CONTROLS | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.UI-FORMS-CONTROLS.partial | UI-FORMS-CONTROLS | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.UI-FORMS-CONTROLS.overflow | UI-FORMS-CONTROLS | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.UI-FORMS-CONTROLS.long-text | UI-FORMS-CONTROLS | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.UI-TEXT-STATUS.empty | UI-TEXT-STATUS | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.UI-TEXT-STATUS.loading | UI-TEXT-STATUS | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.UI-TEXT-STATUS.error | UI-TEXT-STATUS | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.UI-TEXT-STATUS.populated | UI-TEXT-STATUS | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.UI-TEXT-STATUS.partial | UI-TEXT-STATUS | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.UI-TEXT-STATUS.overflow | UI-TEXT-STATUS | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.UI-TEXT-STATUS.zero-one-many | UI-TEXT-STATUS | zero-one-many | Not executed | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UIH.UI-TEXT-STATUS.long-text | UI-TEXT-STATUS | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.DATA-TEXT-INSPECTOR.empty | DATA-TEXT-INSPECTOR | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.DATA-TEXT-INSPECTOR.loading | DATA-TEXT-INSPECTOR | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.DATA-TEXT-INSPECTOR.error | DATA-TEXT-INSPECTOR | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.DATA-TEXT-INSPECTOR.populated | DATA-TEXT-INSPECTOR | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.DATA-TEXT-INSPECTOR.partial | DATA-TEXT-INSPECTOR | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.DATA-TEXT-INSPECTOR.overflow | DATA-TEXT-INSPECTOR | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.DATA-TEXT-INSPECTOR.zero-one-many | DATA-TEXT-INSPECTOR | zero-one-many | Not executed | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UIH.DATA-TEXT-INSPECTOR.long-text | DATA-TEXT-INSPECTOR | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.AUDIO-AUDITION.empty | AUDIO-AUDITION | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.AUDIO-AUDITION.loading | AUDIO-AUDITION | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.AUDIO-AUDITION.error | AUDIO-AUDITION | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.AUDIO-AUDITION.populated | AUDIO-AUDITION | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.AUDIO-AUDITION.partial | AUDIO-AUDITION | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.AUDIO-AUDITION.overflow | AUDIO-AUDITION | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.AUDIO-AUDITION.zero-one-many | AUDIO-AUDITION | zero-one-many | Not executed | How does the layout read at zero, one, and many items (singular vs plural copy, spacing)? |
| UIH.AUDIO-AUDITION.long-text | AUDIO-AUDITION | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
| UIH.UI-FIXTURE-PREVIEW.empty | UI-FIXTURE-PREVIEW | empty | Not executed | What is shown when there is no data — zero items, an unfilled form, or absent media? |
| UIH.UI-FIXTURE-PREVIEW.loading | UI-FIXTURE-PREVIEW | loading | Not executed | What is shown while data or content is still loading (skeleton, spinner, progressive reveal)? |
| UIH.UI-FIXTURE-PREVIEW.error | UI-FIXTURE-PREVIEW | error | Not executed | What is shown when the load or submit fails (message, retry affordance, partial fallback)? |
| UIH.UI-FIXTURE-PREVIEW.populated | UI-FIXTURE-PREVIEW | populated | Not executed | What does the normal populated (happy-path) state look like at a typical volume of content? |
| UIH.UI-FIXTURE-PREVIEW.partial | UI-FIXTURE-PREVIEW | partial | Not executed | What is shown for partial or incomplete data — some fields or rows present, others missing? |
| UIH.UI-FIXTURE-PREVIEW.overflow | UI-FIXTURE-PREVIEW | overflow | Not executed | What happens when content exceeds its container — scroll, clip, wrap, or truncate? |
| UIH.UI-FIXTURE-PREVIEW.long-text | UI-FIXTURE-PREVIEW | long-text | Not executed | What happens with unusually long text — truncation, wrapping, ellipsis, or reflow? |
<!-- DATA_f29d631a_END -->

## Four distinct judgment reviews

| Judgment | Source | Required evidence | Status |
|---|---|---|---|
| Restrained blood, no dismemberment | AC-21 | Relevant hit/death/effect-family images reviewed by named reviewer with exact content identity | Not executed |
| Honest provenance/status | AC-22 | Selected source/output/recipe comparisons; catalog versus production evidence; adapted/generated route review | Not executed |
| KingdomZombies identity | AC-23 | Actual title/icon/UI/export inspection; no source-game/publisher logo substitution | Not executed |
| Truthful mechanics | AC-24 | No recoverable fired arrows, dropped crown loot, structural-guard loot, damaging cosmetic acid or active-game Pause promise | Not executed |

[CITED: 04-SPEC.md Prohibitions; tests cannot replace the specified actual judgments]

## Proposed ordered game-run groups

| Group | Compatible scenarios | Reset / extra-process rationale |
|---|---|---|
| G0 source/headless | Strict build, existing affected rules/boundaries/docs/pure/native, all new media/catalog/baseline/composition/layout/runner and export checks | No game window; bounded owned subprocesses |
| G1 inspection hub | Register/failures, characters/items/actions/corpses, structures/map/day-night/crowd, UI/text/data/legacy, audio controls | One hub process; ordinary category/state/reset controls; actual listening/visual checkpoints per case |
| G2 packaged inspection | Fresh owned export launch, complete packaged census/views, close/relaunch, donor unavailable, isolated invalid content variant | Actual executable distinct from editor; extra disposable bad-content instance only when needed |
| G3 inherited solo/LAN regression | All active Critical and affected foundation scenarios selected after impact analysis | Reuse existing ordinary process groups; retain required peer counts; destructive exit checks last |

These are grouping proposals. The planner/executor must name every selected/omitted exact case and justify any extra incompatible process or native launch. Current policy does not automatically require the full historical Phase 3 suite, nor permit omitting an active Critical/affected case. [CITED: docs/workflow/local-testing.md]

## Wave 0 and feasibility gates

- [ ] Exact expected 180-ID registry and producer output/evidence schema.
- [ ] Source-bound content/baseline/media validator and negative controls.
- [ ] Exact rig/fitting/pose/occlusion/event metadata and native policy support.
- [ ] Every required surface/state/canvas and all 70 UI backstop identities in saved case catalog.
- [ ] Every AC/H identity and four judgment procedures retained in saved scenarios.
- [ ] Matching authenticated .NET export templates, correct hub entry mode and packaged JSON/text/media inclusion.
- [ ] Actual audio audition/listener route, with exact sample/version verdict recording.
- [ ] Current independent whole-phase review and prelaunch source/selection fingerprints.
- [ ] New scenario importance/rationale, dependency tags, reproducible actions/results and ordered persistent groups.
- [ ] Image/draw/input identity capture and immutable reports, plus audio/review links.
- [ ] Final complete selected-set repair-cycle accounting.

No rendered acceptance or listening is claimed here. Export template acquisition/self-contained compatibility and listener availability remain explicit feasibility work; see 04-RESEARCH.md Resolved production routes and pending execution evidence. No production-art completeness may be inferred from these drafts. [CITED: 04-RESEARCH.md; 04-SPEC.md AC-12/17/22]


## Executable task mapping — planned, all unchecked

Revision1 contains35 canonical plans,70 tasks and29 waves; each plan has fewer than10 files (maximum9), each task at most5. Planning/source checks are not production or acceptance evidence. Whole actual tracer stays04-01 Task1 before expansion04-25. Research routes are RESOLVED; official matching archive acquisition is observed, but installation/net10 export/launch and actual named listening/judgments remain unperformed.

| Plan | Wave | Tasks | Files | Production capability | Status |
| --- | --- | --- | --- | --- | --- |
| 04-01 | 1 | 2 | 5 | Production donor-to-inspector tracer and exact provenance | [ ] Not executed |
| 04-02 | 5 | 2 | 8 | Early portable-export and audio feasibility | [ ] Not executed |
| 04-03 | 6 | 2 | 9 | Live inspection shell, validated loader and exact native permissions | [ ] Not executed |
| 04-04 | 8 | 2 | 9 | Reusable donor kit, exact animation conversion and assembled scale proof | [ ] Not executed |
| 04-05 | 9 | 2 | 7 | All 24 item identities and reusable wearable/held masters | [ ] Not executed |
| 04-06 | 10 | 2 | 8 | Survivor and first three zombie body fits | [ ] Not executed |
| 04-07 | 11 | 2 | 9 | Complete armored, wide and specialist zombie silhouettes/fits | [ ] Not executed |
| 04-08 | 12 | 2 | 5 | Concurrent locomotion, melee and coherent logical hands | [ ] Not executed |
| 04-09 | 13 | 2 | 8 | Bow, work, food and specialist action composition | [ ] Not executed |
| 04-10 | 14 | 2 | 9 | Persistent corpses, wildlife, identities and real crowd resources | [ ] Not executed |
| 04-11 | 9 | 2 | 8 | Three actual main-building tiers and shared camp resources | [ ] Not executed |
| 04-12 | 15 | 2 | 8 | Walls, independent guards and complete defense lifecycle | [ ] Not executed |
| 04-13 | 16 | 2 | 9 | Complete authored world, portal and environmental effect inspection | [ ] Not executed |
| 04-14 | 10 | 2 | 8 | Native UI kit, packaged fonts and reusable theme | [ ] Not executed |
| 04-15 | 15 | 2 | 7 | Reusable inventory, legacy, stash and target layout states | [ ] Not executed |
| 04-16 | 18 | 2 | 7 | People, economy, construction, map, rally and warning layouts | [ ] Not executed |
| 04-17 | 19 | 2 | 7 | Session, save, outcome, settings and error layout resources | [ ] Not executed |
| 04-18 | 10 | 2 | 9 | Complete run, time, survivor, item, combat and inventory baseline | [ ] Not executed |
| 04-19 | 18 | 2 | 8 | Complete economy, food, map, structure and tower baseline | [ ] Not executed |
| 04-20 | 20 | 2 | 9 | Complete zombies, progression, portals, sessions and preferences | [ ] Not executed |
| 04-21 | 21 | 2 | 8 | All 30 real sound categories and complete event-bank resources | [ ] Not executed |
| 04-22 | 23 | 2 | 9 | Complete English text, compatibility and data/text inspection | [ ] Not executed |
| 04-23 | 24 | 2 | 5 | Complete source census and final exact native-policy reconciliation | [ ] Not executed |
| 04-24 | 27 | 2 | 5 | Independent whole-phase review and exact prelaunch selection | [ ] Not executed |
| 04-25 | 2 | 2 | 6 | Exact census/provenance and atomic repeatable publication | [ ] Not executed |
| 04-26 | 7 | 2 | 6 | Inspection input/lifetime expansion with immediate exact native permission proofs | [ ] Not executed |
| 04-27 | 17 | 2 | 5 | World/item navigation, replacement and held-out preview states | [ ] Not executed |
| 04-28 | 11 | 2 | 5 | Canonical status/input/player symbols and retained desktop identity art | [ ] Not executed |
| 04-29 | 22 | 2 | 5 | Native audio audition, exact current-loop ownership and backstops | [ ] Not executed |
| 04-30 | 25 | 2 | 5 | Saved scenario/evidence runner and prelaunch/acceptance validation code | [ ] Not executed |
| 04-31 | 26 | 2 | 6 | Complete portable Windows inspection export and clean failure/relaunch proof | [ ] Not executed |
| 04-32 | 28 | 2 | 5 | Actual selected native/listening run and separate named judgments | [ ] Not executed |
| 04-33 | 29 | 2 | 5 | Exact acceptance/artifact/evidence completion reconciliation | [ ] Not executed |
| 04-34 | 3 | 2 | 7 | Exact SDK export-layout prerequisite | [ ] Not executed |
| 04-35 | 4 | 2 | 5 | Permanent inspection export-feature profile | [ ] Not executed |

Exact180 source/resource ownership: [obligations](04-OBLIGATION-COVERAGE.md). Full24AC/32D/34H/70UI/4prohibition source statements/owners: [acceptance map](04-ACCEPTANCE-COVERAGE.md) and [coverage JSON](04-PLAN-COVERAGE.json). Wave/command creation: [graph](04-PLAN-GRAPH.md). [Source audit](04-SOURCE-AUDIT.md) has no missing in-scope obligation. Every actual file/symbol/retained bundle member enters [artifact inventory](04-ARTIFACTS.md). Original checker report stays [04-PLAN-CHECK.md](04-PLAN-CHECK.md); dispositions are in [revision1](04-REVISION-1.md).

### Native permission and review ordering

04-03 creates and exactly authorizes the actual root/loader with compiled positive/negative controls before engine.source-policy.04-26 then creates Input/Shell, updates actual exact path/namespace/type/member permissions and compiled guard/near-miss tests, and only then runs its source-policy gate. Tuple metadata and blanket grants never approve native access.04-23 covers the remaining completed native inventory and source-policy gate.04-30 implements all runner/prelaunch/acceptance code and negative controls before04-24 parent-supplied actual independent whole-phase review/fixes and data-only selection binding.04-31 actual complete export precedes review;04-32 actual native/listening/judgments follows it;04-33 reconciles actual completion. Source repairs require renewed independent review before windows.

### Independent domain backstop map

Each H remains verification: backstop with its own outcome; shared invocations/setup do not merge identities.

| ID | Source/check owner | Stable saved case | Actual final owner | Outcome |
| --- | --- | --- | --- | --- |
| H01 | 04-25 Task1 | heldout.H01 | 04-32 Task1 | [ ] Not executed |
| H02 | 04-25 Task1 | heldout.H02 | 04-32 Task1 | [ ] Not executed |
| H03 | 04-25 Task1 | heldout.H03 | 04-32 Task1 | [ ] Not executed |
| H04 | 04-25 Task1 | heldout.H04 | 04-32 Task1 | [ ] Not executed |
| H05 | 04-25 Task2 | heldout.H05 | 04-32 Task1 | [ ] Not executed |
| H06 | 04-23 Task1 | heldout.H06 | 04-32 Task1 | [ ] Not executed |
| H07 | 04-23 Task1 | heldout.H07 | 04-32 Task1 | [ ] Not executed |
| H08 | 04-23 Task1 | heldout.H08 | 04-32 Task1 | [ ] Not executed |
| H09 | 04-23 Task1 | heldout.H09 | 04-32 Task1 | [ ] Not executed |
| H10 | 04-23 Task1 | heldout.H10 | 04-32 Task1 | [ ] Not executed |
| H11 | 04-23 Task1 | heldout.H11 | 04-32 Task1 | [ ] Not executed |
| H12 | 04-09 Task1 | heldout.H12 | 04-32 Task1 | [ ] Not executed |
| H13 | 04-06 Task2 | heldout.H13 | 04-32 Task1 | [ ] Not executed |
| H14 | 04-09 Task1 | heldout.H14 | 04-32 Task1 | [ ] Not executed |
| H15 | 04-08 Task2 | heldout.H15 | 04-32 Task1 | [ ] Not executed |
| H16 | 04-04 Task1 | heldout.H16 | 04-32 Task1 | [ ] Not executed |
| H17 | 04-10 Task1 | heldout.H17 | 04-32 Task1 | [ ] Not executed |
| H18 | 04-08 Task2 | heldout.H18 | 04-32 Task1 | [ ] Not executed |
| H19 | 04-17 Task2 | heldout.H19 | 04-32 Task1 | [ ] Not executed |
| H20 | 04-15 Task2 | heldout.H20 | 04-32 Task1 | [ ] Not executed |
| H21 | 04-17 Task1 | heldout.H21 | 04-32 Task1 | [ ] Not executed |
| H22 | 04-17 Task2 | heldout.H22 | 04-32 Task1 | [ ] Not executed |
| H23 | 04-16 Task2 | heldout.H23 | 04-32 Task1 | [ ] Not executed |
| H24 | 04-18 Task2 | heldout.H24 | 04-32 Task1 | [ ] Not executed |
| H25 | 04-29 Task2 | heldout.H25 | 04-32 Task1 | [ ] Not executed |
| H26 | 04-29 Task2 | heldout.H26 | 04-32 Task1 | [ ] Not executed |
| H27 | 04-23 Task1 | heldout.H27 | 04-32 Task1 | [ ] Not executed |
| H28 | 04-23 Task1 | heldout.H28 | 04-32 Task1 | [ ] Not executed |
| H29 | 04-23 Task1 | heldout.H29 | 04-32 Task1 | [ ] Not executed |
| H30 | 04-23 Task1 | heldout.H30 | 04-32 Task1 | [ ] Not executed |
| H31 | 04-31 Task2 | heldout.H31 | 04-32 Task1 | [ ] Not executed |
| H32 | 04-23 Task1 | heldout.H32 | 04-32 Task1 | [ ] Not executed |
| H33 | 04-25 Task2 | heldout.H33 | 04-32 Task1 | [ ] Not executed |
| H34 | 04-23 Task1 | heldout.H34 | 04-32 Task1 | [ ] Not executed |

H12/H14 also cover exact guard/tier/corpse/empty boundaries04-10–12; H19 sound limits04-29 Task2; H24 actual media precision04-04/21 and layout formatting04-17; H27–H32 actual packaged/source variants04-31 Task2. Every individual H retains all linked observations.

### Independent UI backstop map

| Exact saved identity | Source/check owner | Actual final owner | Outcome |
| --- | --- | --- | --- |
| UIH.HUB-NAV.loading | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-NAV.error | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-NAV.overflow | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-NAV.long-text | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.empty | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.loading | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.error | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.populated | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.partial | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.overflow | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.zero-one-many | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.HUB-REGISTER.long-text | 04-26 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.empty | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.loading | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.error | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.populated | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.partial | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.overflow | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.CHARACTER-PREVIEW.long-text | 04-08 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.empty | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.loading | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.error | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.populated | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.overflow | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.WORLD-ITEM-PREVIEW.long-text | 04-27 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.empty | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.loading | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.error | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.populated | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.partial | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.overflow | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.zero-one-many | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-COLLECTIONS.long-text | 04-15 Task1 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.empty | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.loading | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.error | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.partial | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.overflow | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FORMS-CONTROLS.long-text | 04-17 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.empty | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.loading | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.error | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.populated | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.partial | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.overflow | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.zero-one-many | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-TEXT-STATUS.long-text | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.empty | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.loading | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.error | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.populated | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.partial | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.overflow | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.zero-one-many | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.DATA-TEXT-INSPECTOR.long-text | 04-22 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.empty | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.loading | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.error | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.populated | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.partial | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.overflow | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.zero-one-many | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.AUDIO-AUDITION.long-text | 04-29 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.empty | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.loading | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.error | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.populated | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.partial | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.overflow | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |
| UIH.UI-FIXTURE-PREVIEW.long-text | 04-16 Task2 | 04-32 Task1 | [ ] Not executed |

### Actual evidence and command gates

- Four judgment-tier prohibitions are separate projectProhibitions projections under04-32 must_haves.prohibitions, statement/status/verification only. Named actual file/source/media-specific judgments are04-32 Task2; no mechanical descriptor or inferred verdict is invented.
- Every new command/case remains pending its exact creating task in PLAN command_status and graph. Every automated PLAN command has an immediate observable fails_when. Existing commands remain actual inherited subjects only.
- Complete production/source/headless work, actual independent review/fixes, then exact activeCritical/new/changed/affected selection, omissions/rationale, importance/dependencies and ordered G1/G2/G3 groups before Windows1280x720/noVM windows.
- Native scenarios collect failures while independent cases continue; batch repairs, focused failed/blocked/newly affected checks, then fresh whole selected-set pass on repaired reviewed source. An initially clean full selection needs no duplicate pass. Preserve original failed/current/prior identities and records.
- Actual capable named reviewer listens to exact actual audio files/versions and records sample/loop/mix verdicts. No named listener, capability availability or hearing is presumed by research. Playback/waveform/metadata is insufficient; absent actual review blocks AC-12.
- Completion distinguishes executed-now/eligible reused/missing-invalid. Missing resource/backstop/listening/judgment, noncosmetic required-state failure or unresolved high-security finding blocks. All boxes remain unchecked until actual linked evidence exists.


## Observed export-layout prerequisite — 04-34

Existing ASSET-05 export is blocked by the retained actual ExportDebug SDK SCR0007 failure in runa9a37eb6-93a2-4891-b3f2-d442ea7b7a8b.04-34(wave3,depends25) supplies2 bounded source-only tasks before unfinished04-02 Task1(now wave5, after permanent-profile35). New rules.export-layout-origin/rules.export-layout-boundary and Phase4 export.layout commands are pending their creating tasks; exact actual source/SDK/config/path/hash/line and negative spans must be proved. Normal Debug and original layout/source IDs stay enforced. Neither Linux nor native release export/content acceptance is supplied. See04-EXPORT-PREREQUISITE.md for exact observations and protected partial work. Already committed02 audioTask2 is preserved, not repeated; completed01/25 summaries/results remain immutable. Original180/24AC/32D/34H/70UI/4prohibition scope is unchanged. Planning alone checks no execution box.


## Permanent inspection export profile — 04-35

35 depends completed34/wave4; unfinished02Task1 depends35/wave5. Permanent source configuration adds asset_inspection on the existing single named inspection preset and its qualified Inspection main setting, preserving literal ordinary Foundation and five project edges/options. Preset ownership transfers from02 to35; check.cjs adopts only the checkpointed compatible env/cwd helper plus new profile loader, holding incomplete export-module activation for02. Two new source cases use G0-export-profile orders350/351, without renumbering existing cases or adding AC/H/UI scope. Actual settings/resource/default/explicit-feature observations and independent profile/case/path/ordinary-activation failures supply source evidence only. Fresh actual packaged Inspection entry remains02; prior Foundation8frames/editor PCK/config supporting proofs do not substitute. Checkpoint and five partial hashes are retained under .phase4/checkpoints/04-02-entry-93386745/manifest.json; completed01/25/34 and committed audio stay immutable. See04-INSPECTION-EXPORT-PROFILE.md. No source or STATE implementation occurs during planning.
