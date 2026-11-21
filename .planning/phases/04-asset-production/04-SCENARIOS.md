# Phase04 saved source fixtures and acceptance scenarios

**Current execution scope (November 21).** The [focused Windows handoff](04-HANDOFF.md) sets the required rendered selection and assigns deferred IDs to Phases 5–9. It supersedes earlier full-matrix execution instructions below.

This fixed crosswalk binds current fixture resources and saved case definitions. Source and headless results leave appearance, listening and final acceptance pending. Plan 31 owns final packaged delivery. Plans 32 and 33 own actual acceptance and named judgments.

## Plan 04-30 integration ownership

The existing dispatcher accepts one case ID and has no completion route. Plan 04-30 owns the narrow dispatch integration in `tools/phase4/check.cjs` for exact comma-separated IDs, the declared composition and rendered groups, and the completion evidence path. The dynamic loader and editor-import workaround retain their current behavior.

The native DT-26 source view embeds the exact `tools/phase4/required-cases.json` bytes. Updating the catalog therefore requires regenerating only the existing compressed payload, uncompressed length and SHA-256 in `src/godot-game/content/packs/delivery.json`. The stock normalizer supplies the corresponding catalog and delivery byte and digest fields in `src/godot-game/content/pack-index.json`. The existing source fixture records, resource definitions, accepted counts, native permissions and delivery and export status retain their prior values.

## Running the saved procedures

`node tools/phase4/check.cjs --list` lists executable source checks. `--case ID[,ID...]` takes exact registered identities and rejects duplicate and unknown IDs. The source groups include `content`, `composition`, `layouts`, `audio`, `export` and `runner`. The separate `renderedCases` array in the saved catalog identifies actual-only procedures.

### export.hub-consumer

Critical source/headless case for ASSET-05 and AC-15/16/17. Use the pinned Windows export toolchain and a fresh run-owned directory. Build and export the current project, check the bundled runtime and PCK members, then start the exported executable headlessly in its own directory and profile. Inspect the settled hub with read-only loopback scene tree and object queries. Require the current bow texture and registration, visible resource and status labels, the independent 180-count and exact Items rows, and the real navigation and preview controls. Exercise each required rejection on a separate copy of the captured data. A separate editor-mounted probe checks the Bow pixels and exact Register membership using the same PCK and exported managed assemblies. Retain the run, child, packet and package receipts. This group requires zero game windows.

### export.portability

Critical source/headless case for ASSET-05 and AC-15/16/17/19. Use the same pinned export toolchain and keep the positive portable directory fixed. Launch and naturally close the package in a fresh profile. Exercise missing item data, filename lookalikes, case and encoded aliases, changed bytes, reordered definitions, empty discovery, invalid expected-register bytes, an interrupted import and an independently failed launch. Use the exact PCK and exported managed assemblies for the mounted item probe. Observe the precise IT-11 failure and working IT-07 animation. Relaunch the original executable in a second profile and compare every packaged file hash. Save both child lifetimes and all variant receipts. This case supplies headless delivery evidence for H27–H32. The exact rendered procedures follow the focused ownership register.

`--group rendered` first reads the current review, fixes and selection files. Their hashes and the Phase 4 reviewed-source targets must match committed bytes. The source revision must still match every tracked source input byte. The review's build report identifies the exact toolchain, source, media, engine and packaged executable. Select and justify every disposition in `04-SELECTION.json`. Groups record process roles, case order, reset, cleanup and a bounded duration. Runtime files and shared build inputs determine affected inherited consumers. A source change requires a renewed review and selection.

`--case runner.prelaunch-source` runs the detached review and selection controls before actual review data exists. `--case runner.selection` validates the actual committed review and selection. `--case runner.prelaunch` and `--group rendered` additionally require `04-SELECTION.json` to provide `sourceResults` with one `id` and hash-bound `result` reference for every selected pre-window source or headless case. Both entry points verify review and selection directly. The named judgments and final artifact verification run after the rendered campaign.

The `04-RUNS.json` input binds the current identity, review and selection SHA-256 hashes, declaration time and invocation campaign UUID. It declares an `initial`, `focused` or `final` attempt. Repairs link the previous immutable report. A focused attempt supplies the reason and exact failed, blocked and newly affected case IDs. An actual driver supplies the saved case ID, independently selected stimulus, and ordered checkpoint steps with physical key or mouse input and exact inspected node properties. The runner copies the driver declaration into the invocation before starting the owned processes. It records input acknowledgments, raw read-only native observations and composited client PNGs after `DwmFlush`. Each recorded checkpoint must match the saved input and observed property. The packaged relaunch procedure requires two distinct lifetimes of the same executable. The runner retains per-case failures and continues while the owned process remains usable. The inherited solo and LAN sets use their existing observers under the current Phase 4 review binding.

`--verify-completion PATH` checks the whole current selection. The index links the initial, repair and final campaign reports, source/headless results, current export, named listening and judgments, and the changed-file and source audit inventories. A focused pass never replaces the fresh full selected run after repairs. Later same-input source failures, blockers and missing outcomes invalidate earlier passes. Proven unrelated source revisions remain independent. The permission inventory matches all 251 exact guarded host tuples against the current policy, compiled case declarations and the linked per-test execution reports. Every required audio variant and loop retains its exact WAV hash and version. The two mix decisions require their own named records. The four prohibition judgments retain their individual statements and exact case images. The independent humans must provide the actual listening and image verdicts and their linked evidence.

Use `--case acceptance.artifacts-source` for detached artifact and receipt-history qualification. Include its independent passing receipt in both the pre-window source results and the completion index. Use `--case acceptance.artifacts` with the actual `04-COMPLETION.json`, or `--verify-completion PATH` to name the index. Both run the full completion verification. The completion index lists independent source results and the verification publishes its own result after those checks.

## fixtures

- **item-icons-and-all-24-definitions** — src/godot-game/content/packs/items-01-10.json, src/godot-game/content/packs/items-11-24.json
- **eight-bodies-and-all-legal-equipment-fits** — src/godot-game/content/packs/bodies-a.json, src/godot-game/content/packs/bodies-b.json, src/godot-game/content/packs/bodies-c.json, src/godot-game/content/animation/fits-a.json, src/godot-game/content/animation/fits-b.json, src/godot-game/content/animation/fits-c.json
- **concurrent-locomotion-mixed-dual-and-bow** — src/godot-game/content/animation/locomotion-melee.json, src/godot-game/content/animation/bow.json, src/godot-game/content/animation/work-special.json
- **persistent-equipped-corpses-and-loot** — src/godot-game/content/packs/corpses-wildlife.json, src/godot-game/content/fixtures/crowds.json
- **all-structure-tiers-and-states** — src/godot-game/content/world/main-assemblies.json, src/godot-game/content/world/defense-assemblies.json, src/godot-game/content/packs/guards.json
- **six-portals-cosmetic-acid-day-night-world** — src/godot-game/content/world/map.json, src/godot-game/content/world/portal-assemblies.json, src/godot-game/content/packs/effects-portals.json
- **all-ui-errors-legacy-grids-and-four-identities** — src/godot-game/content/ui/inventory.json, src/godot-game/content/ui/legacy-stash-targets.json, src/godot-game/content/ui/camp-panels.json, src/godot-game/content/ui/map-spectator-warnings.json, src/godot-game/content/ui/session-save-outcomes.json, src/godot-game/content/ui/settings-errors.json, src/godot-game/content/packs/ui-identity.json
- **100-survivors-300-zombies-100-corpses-100-piles** — src/godot-game/content/fixtures/crowds.json
- **current-loop-and-quiet-compatibility** — src/godot-game/content/fixtures/audio-states.json, src/godot-game/content/compatibility/appearance-layout.json

## heldout

- **H01** — H01: Same-named donors in different owning files/types and explicitly shared outputs retain the right source/output identities; AC-02/03/19.
- **H02** — H02: A selected output with missing origin, identity, recipe or required evidence cannot obtain completed provenance; AC-03/19.
- **H03** — H03: Case/encoding variants cannot silently alias stable IDs/source locators or portable paths; AC-02/03/19.
- **H04** — H04: Reordering donor/manifest enumeration preserves exact output-to-origin resolution; AC-02/03/19.
- **H05** — H05: Interrupted/competing production work preserves originals and cannot publish unvalidated output as verified; AC-02/03/19.
- **H06** — H06: A missing unique obligation remains a failure even when duplicate rows inflate the apparent total to 180; AC-01/19.
- **H07** — H07: Explicitly shared files may satisfy several obligations without collapsing distinct item/resource identities; AC-01/19.
- **H08** — H08: Empty/partial packs and absent or empty required output dependencies cannot count as completed coverage; AC-01/19.
- **H09** — H09: Shuffling resource/mapping order preserves the actual covered-ID set and completeness result; AC-01/19.
- **H10** — H10: Unique integer obligation coverage is exact; percentages, duplicate files or rounding cannot certify an incomplete pack; AC-01/19.
- **H11** — H11: Output generation/import interrupted partway through remains visibly incomplete rather than gaining metadata-only coverage; AC-01/16/19.
- **H12** — H12: Empty/last-arrow and broken/usable transitions, required tier/guard counts and persistent-corpse boundaries preserve the intended presentation; AC-06/07/08/19.
- **H13** — H13: Overlapping coat/armor/belt/bag/hand layers and nearby bodies keep required gear and target identity readable; AC-05/07/18/19.
- **H14** — H14: Empty hands/ammunition/slots, empty versus nonempty corpses and the structural-guard exception do not invent equipment or loot; AC-06/07/19.
- **H15** — H15: Facing/action/gear order changes retain correct final equipment, occlusion and logical hand identity; AC-04/05/06/19.
- **H16** — H16: Donor pixel registration, offsets, nonuniform frame timing, held/repeated frames and loop/endpoints survive conversion/composition; AC-03/04/05/19.
- **H17** — H17: Repeated preview reset/state restoration leaves one current composition/loop/corpse state without replaying past one-shots; AC-04/07/14/19.
- **H18** — H18: Mid-action body/gear changes immediately clear replaced visuals, permit a clean restart and retain concurrent walking/combat presentation; AC-04/19.
- **H19** — H19: Supported resolution/UI-scale endpoints, expanded text and configured sound limits remain inspectable; AC-11/12/19.
- **H20** — H20: Equal labels/values and touching/overlapping UI or legacy-grid regions do not merge identities or hide critical warning/state information; AC-10/11/14/19.
- **H21** — H21: Empty/zero-result examples and missing/invalid text, audio or data refs show the right state/diagnostic without a false resource pass; AC-10/11/12/13/19.
- **H22** — H22: Catalog encoding, exact IDs/placeholders, accented names/font fallback and longer strings remain readable without corrupting text identity; AC-11/19.
- **H23** — H23: Reordered data/definitions retain bindings and quantities; warning layers and four-player identities remain correct across UI state examples; AC-10/11/13/19.
- **H24** — H24: Display scaling, formatting and media conversion do not silently change authored units/defaults, quantities, timing or sound metadata; AC-03/11/12/13/19.
- **H25** — H25: Repeated UI/audio/current-state example restoration does not accumulate ambience loops or replay old transactional/death/warning cues; AC-12/14/19.
- **H26** — H26: Rapid view/audition/state changes leave correct bindings/current loops and visible muted warnings, without requiring later gameplay interactions; AC-10/12/14/19.
- **H27** — H27: Similar/conflicting resource paths are diagnosed accurately; one bad entry does not conceal independent valid entries; AC-15/16/19.
- **H28** — H28: An empty or partially missing pack/gallery cannot obtain a successful content result through zero discovery or fallback art; AC-16/17/19.
- **H29** — H29: Packaged path/ID spelling and readable exact diagnostics do not depend on development-path aliases or encoding loss; AC-15/16/19.
- **H30** — H30: Validation/discovery order cannot suppress known resource failures or prevent inspection of unaffected entries; AC-16/19.
- **H31** — H31: Repeated clean launch/close/relaunch uses the same packaged content and does not require hidden donor/editor caches; AC-15/17/19.
- **H32** — H32: Partial imports, interrupted checks or failed launch attempts cannot inherit an earlier successful result; unaffected content remains inspectable where the build can run; AC-16/17/19.
- **H33** — H33 (manual): Re-export/re-import preserves stable selected-output identities without duplicate provenance records; AC-03/19.
- **H34** — H34 (manual): Recomputing or regenerating coverage cannot count one obligation twice or change completeness solely through repetition; AC-01/19.

## backstops

- **UIH.HUB-NAV.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.HUB-NAV.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.HUB-NAV.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.HUB-NAV.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.HUB-REGISTER.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.HUB-REGISTER.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.HUB-REGISTER.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.HUB-REGISTER.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.HUB-REGISTER.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.HUB-REGISTER.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.HUB-REGISTER.zero-one-many** — How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?
- **UIH.HUB-REGISTER.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.CHARACTER-PREVIEW.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.CHARACTER-PREVIEW.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.CHARACTER-PREVIEW.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.CHARACTER-PREVIEW.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.CHARACTER-PREVIEW.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.CHARACTER-PREVIEW.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.CHARACTER-PREVIEW.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.WORLD-ITEM-PREVIEW.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.WORLD-ITEM-PREVIEW.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.WORLD-ITEM-PREVIEW.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.WORLD-ITEM-PREVIEW.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.WORLD-ITEM-PREVIEW.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.WORLD-ITEM-PREVIEW.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.UI-COLLECTIONS.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.UI-COLLECTIONS.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.UI-COLLECTIONS.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.UI-COLLECTIONS.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.UI-COLLECTIONS.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.UI-COLLECTIONS.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.UI-COLLECTIONS.zero-one-many** — How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?
- **UIH.UI-COLLECTIONS.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.UI-FORMS-CONTROLS.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.UI-FORMS-CONTROLS.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.UI-FORMS-CONTROLS.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.UI-FORMS-CONTROLS.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.UI-FORMS-CONTROLS.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.UI-FORMS-CONTROLS.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.UI-TEXT-STATUS.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.UI-TEXT-STATUS.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.UI-TEXT-STATUS.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.UI-TEXT-STATUS.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.UI-TEXT-STATUS.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.UI-TEXT-STATUS.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.UI-TEXT-STATUS.zero-one-many** — How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?
- **UIH.UI-TEXT-STATUS.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.DATA-TEXT-INSPECTOR.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.DATA-TEXT-INSPECTOR.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.DATA-TEXT-INSPECTOR.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.DATA-TEXT-INSPECTOR.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.DATA-TEXT-INSPECTOR.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.DATA-TEXT-INSPECTOR.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.DATA-TEXT-INSPECTOR.zero-one-many** — How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?
- **UIH.DATA-TEXT-INSPECTOR.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.AUDIO-AUDITION.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.AUDIO-AUDITION.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.AUDIO-AUDITION.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.AUDIO-AUDITION.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.AUDIO-AUDITION.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.AUDIO-AUDITION.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.AUDIO-AUDITION.zero-one-many** — How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?
- **UIH.AUDIO-AUDITION.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?
- **UIH.UI-FIXTURE-PREVIEW.empty** — What is shown when there is no data — zero items, an unfilled form, or absent media?
- **UIH.UI-FIXTURE-PREVIEW.loading** — What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?
- **UIH.UI-FIXTURE-PREVIEW.error** — What is shown when the load or submit fails (message, retry affordance, partial fallback)?
- **UIH.UI-FIXTURE-PREVIEW.populated** — What does the normal populated (happy-path) state look like at a typical volume of content?
- **UIH.UI-FIXTURE-PREVIEW.partial** — What is shown for partial or incomplete data — some fields or rows present, others missing?
- **UIH.UI-FIXTURE-PREVIEW.overflow** — What happens when content exceeds its container — scroll, clip, wrap, or truncate?
- **UIH.UI-FIXTURE-PREVIEW.long-text** — What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

## acceptance

- **AC-01** — **AC-01 — Complete census (ASSET-02):** Every one of the 180 registered IDs maps to actual inspectable outputs or explicitly declared shared outputs. All required resource families exist; file/atlas/recording counts do not substitute for obligation coverage.
- **AC-02** — **AC-02 — Exact origins (ASSET-01):** Each selected recovered output/logic unit identifies its owning source file, type, object ID/name or original code unit, preserved-original identity and selected dependencies. Original recovered files remain unchanged.
- **AC-03** — **AC-03 — Reusable production records (ASSET-01):** Stable output IDs, relative paths, versions/content identities, transformations or Codex/GPT generation sources/recipes and applicable image/audio/animation metadata are recorded. Actual production validation is distinguished from original V/M/C inspection evidence.
- **AC-04** — **AC-04 — Live composed animation (ASSET-03):** In the Godot character inspector, the reviewer switches bodies, equipment, movement and attacks live. Walking continues during melee, bow aim and firing. Changes appear immediately, clear old visuals and may restart the current animation cleanly. Required combinations do not depend on separately baked full-combination sheets.
- **AC-05** — **AC-05 — Required body/equipment coverage (ASSET-02/03):** The survivor and all seven recognizable zombie silhouettes support every applicable item, direction and required AN action, including wide bodies, clothing, both armor types, belt, gloves, boots and both backpacks. All 24 item definitions have identifiable icons and applicable worn/held forms. Only AC-18's bounded cosmetic exceptions may remain.
- **AC-06** — **AC-06 — Hands, ammunition and condition (ASSET-03):** Fixtures show fists, either-hand one-handed weapons, mixed dual attacks, bow/arrow states, empty/changed arrow hands and broken/fallback gear. Facing changes preserve logical inventory hand identity; visible equipment/ammunition reflects the selected sample state.
- **AC-07** — **AC-07 — Corpses and loot (ASSET-03):** Ordinary survivor/zombie and rabbit/deer corpse endpoints remain inspectable/selectable after effects fade. Changing the sample remaining equipment updates the corpse appearance, crown loss is represented, and nonempty/empty/selected states are distinguishable. Structural guards produce no character corpse or item loot.
- **AC-08** — **AC-08 — Buildings and portals (ASSET-03):** Fixtures show all three main/wall/tower tiers and required site, frame, upgrade, damage, repair, destruction, previous-tier/rebuild and permanent portal-ruin states. Standalone tiers show 1/2/3 independently animated guards; main tier 3 has exactly two one-guard attachments. Upgrades preserve the visible old functional tier until completion.
- **AC-09** — **AC-09 — World and crowd resources (ASSET-02/03):** The assembled map contains the baseline six portals, eight wall/eight standalone-tower sites, clearing groups, six food plants, wildlife zones, terrain and interaction/navigation/collision metadata. Day/night, plant/clearing/prey/effect states are inspectable. Short crowd/persistent-corpse fixtures use the real presentation resources for approximately 100 survivors/300 zombies and the later ≥200 nonempty-corpse/pile scenario; no FPS or three-hour endurance pass is inferred.
- **AC-10** — **AC-10 — UI state resources (ASSET-04):** UI-01–UI-27 and their required normal, empty, waiting, disabled, invalid, conflicting, legacy, lost-access and failure examples are present as reusable layouts/state resources. These examples can be inspected without implementing their future gameplay interaction logic.
- **AC-11** — **AC-11 — Text, scale and identity (ASSET-04):** TXT-01/02 cover all specified labels/descriptions/prompts/errors and preserve formatting placeholders/current-binding references. Layout examples demonstrate supported resolutions/scales, longer replacement text, readable names/numbers/counts and four identities distinguishable beyond color. Held-out display/text cases are carried by AC-19.
- **AC-12** — **AC-12 — Auditioned sound (ASSET-04):** AU-01–AU-30 map to auditioned recordings or generated replacements with documented event routing, duration/channel/rate/trim/loop/gain and mix/priority/repetition settings. Day/night and local ambience can be auditioned, baseline 24/four limits are configurable, and critical warnings have visible examples when muted.
- **AC-13** — **AC-13 — Complete data/support (ASSET-04):** DT-01–DT-27 are present with the complete working baseline, documented meanings/units/defaults/allowed values/dependencies and representative editing examples. Item/content/image/sound/text references validate, zombie progression is independently editable, and local preferences are separated from host balance.
- **AC-14** — **AC-14 — Compatibility presentation (ASSET-03/04):** Stable IDs/versions and sample saved-layout/appearance metadata support inspectable old grid dimensions/placements/orientation, incompatible slots/stacks/depth, current-destination previews and explicit missing-definition errors. Fixture restoration shows persistent gear/corpses/ruins/loops without replaying past one-shot effects; actual gameplay save/load and extraction remain later integration.
- **AC-15** — **AC-15 — Windows content delivery (ASSET-05):** The Godot project, inspection scenes and Windows inspection build load their selected content, fonts, text, data and audio without the external recovery folder or absolute development-path dependencies. Source-locator text in provenance does not create a runtime dependency.
- **AC-16** — **AC-16 — Visible incomplete state (ASSET-05):** Deliberately absent/invalid required resources identify the exact resource ID/path and problem, mark the pack incomplete and leave unaffected gallery entries inspectable. A fallback display cannot turn a missing required output into completed coverage.
- **AC-17** — **AC-17 — Focused current evidence (ASSET-05):** Documented focused Windows import/export/launch, content, visual and audio checks provide current results and required diagnostic evidence. Missing evidence is recorded as such. Linux execution, broad build matrices, full gameplay and release performance/endurance are not claimed from this inspection run.
- **AC-18** — **AC-18 — Bounded cosmetic carryover (ASSET-02/03):** Every remaining known issue is a reproducible minor cosmetic flaw in an exactly documented uncommon combination; all required gear/actions/critical states remain readable. The register records expected/actual results and a Phase 6/7/8 owner as specified above. A missing resource or non-cosmetic required-state failure blocks Phase 4 completion.
- **AC-19** — **AC-19 — Held-out edges (ASSET-01–05):** Planning carries all 34 Edge Coverage rows into non-inferable must-have checks, and Phase 4 obtains held-out evidence for them. Inputs/combinations are chosen independently of authored happy-path examples. Cases may share setup/evidence when their individual obligations remain identifiable; these are not 34 separate build runs. Any unsupported backstop remains a visible verification gap.
- **AC-20** — **AC-20 — Inspection boundary (ASSET-03–05):** The delivered resource index and inspection entry points expose the live character controls, assembled map, UI state examples, data and audio described here. Resource inspection is clearly identified; the pack's completion record does not claim implementation of the later gameplay interactions or a complete playable campaign.
- **AC-21** — **AC-21 — Blood limit (ASSET-02/03, prohibition):** Hit/death presentation uses restrained blood effects and MUST NOT include dismemberment. Verify by recorded visual judgment across the relevant effect/death families.
- **AC-22** — **AC-22 — Honest provenance/status (ASSET-01/02, prohibition):** Production records MUST NOT present catalog entries/metadata/unfinished exports as completed assets or label generated/adapted material as unchanged donor reuse. Verify through recorded source/output and evidence-status review.
- **AC-23** — **AC-23 — Game identity (ASSET-02/04/05, prohibition):** Title, icon and UI identify KingdomZombies and MUST NOT use Kingdom Two Crowns or publisher logos as the game's identity. Verify by recorded visual review.
- **AC-24** — **AC-24 — Truthful mechanic presentation (ASSET-03/04, prohibition):** Art/UI MUST NOT imply recoverable fired arrows, lootable dropped crowns, structural-guard loot, damaging bloater acid or an active-game menu that pauses play. Donor visuals follow KingdomZombies' specified mechanics. Verify by recorded visual/content review.

## decisions

- **D-01** — **D-01:** Use deliberate pixel-art poses for walking and attacks, with flexible arm/weapon movement where aiming needs it. Keep crisp pixel edges and the required concurrent walking/combat presentation; do not make full baked sheets for every equipment/action combination.
- **D-02** — **D-02:** Melee attacks have clear weight and follow-through. Shoulders and torso support the strike; heavier bodies and weapons feel weighty, while quick attacks remain sharp. Walking continues.
- **D-03** — **D-03:** Clothing, belts and bags follow their attachment points closely, with little independent sway or bounce. This selects mostly fixed gear rather than the proposed secondary-motion treatment.
- **D-04** — **D-04:** Melee effects are very minimal. Let the swing and target reaction carry the action; use only the small contact effects needed for clarity. The restrained-blood/no-dismemberment limit remains binding.
- **D-05** — **D-05:** Bow aiming is led mainly by the arms and bow. Keep the head and torso close to their normal pose while aiming up/down, rather than adding pronounced torso/head tracking.
- **D-06** — **D-06:** Death uses a short authored collapse suited to each body, ending in a stable, readable equipped corpse pose. A variable physical-collapse treatment was not selected.
- **D-07** — **D-07:** Hits cause a small, clear shoulder/torso flinch. This is visual feedback only and adds no stun, knockback or action delay.
- **D-08** — **D-08:** Use Godot's built-in animation tools and project code only, with no paid animation dependency. Research and planning should solve composition within that boundary rather than schedule a paid-add-on comparison or purchase.
- **D-09** — **D-09:** Direction changes switch facing immediately and continue the current stride/action coherently. Preserve logical left/right hand identity and actual equipment.
- **D-10** — **D-10:** Starting to walk uses a quick visual weight shift; stopping uses a short settle. Actual movement starts and stops immediately.
- **D-11** — **D-11:** Clothing, armor and bags have a practical scavenged fit: slightly loose, utilitarian and modestly bulky, fitted to each required body. Slightly loose shapes do not imply the independent sway/bounce rejected in D-03.
- **D-12** — **D-12:** Gathering, building, carrying and fireside rest use restrained, recognizable task poses with only the body movement each activity needs.
- **D-13** — **D-13:** Favor the recovered sprites' simpler, low-resolution logical detail. Simplify the larger zombie/base examples to match, rather than raising the whole pack to their richer detail. Exact pixel density and dimensions remain a research/production choice established through the required assembled survivor/zombie/wall/tower/base scale test.
- **D-14** — **D-14:** Use natural days and atmospheric nights: earthy scenery and warm wood in daylight, cooler/darker nights and restrained warm firelight. Required equipment, identities and states remain readable after dark.
- **D-15** — **D-15:** Keep character heads, hands and equipment close to natural proportions. Use pose and contrast for readability rather than noticeably enlarged heads/hands/items.
- **D-16** — **D-16:** Buildings and defenses look like practical camp construction: rough timber, straightforward bracing and functional reinforcement, with clear tier silhouettes. Preserve the specified wood/reinforced-wood/metal progression.
- **D-17** — **D-17:** Backgrounds are layered but unobtrusive. Distant scenery and parallax provide depth while quiet space keeps characters, equipment and sites clear.
- **D-18** — **D-18:** For normal unselected characters, strengthen dark outer pixel edges selectively where needed while keeping donor-style shading. Do not apply a uniform outline by default.
- **D-19** — **D-19:** Standing damaged structures show localized material damage: cracks, splintered timber, bent reinforcement and small damaged areas, increasing with severity. Keep tier, material and functional state clear.
- **D-20** — **D-20:** Provide one canonical inventory icon per item, without separate durability-level icon variants. The creator did not choose one of the proposed icon-art treatments and does not want that detail expanded now. Exact durability indicators remain a planning detail within the required layouts/state examples; this does not remove item identities or required broken/condition presentation, nor the separate worn/held/corpse forms.
- **D-21** — **D-21:** Zombie bodies share related muted undead skin tones, with subtle color differences supporting their required distinct silhouettes. Equipped items retain their actual appearance.
- **D-22** — **D-22:** Start portal colors from suitable recovered donor colors. Spit and cosmetic acid use separate restrained sickly colors. No exact hue or numeric palette was selected; cosmetic acid remains non-damaging.
- **D-23** — **D-23:** Use small, quiet pixel contact-shadow shapes beneath characters and small objects to establish their contact with the ground.
- **D-24** — **D-24:** Selected corpses, loot piles and other inspectable objects use a steady thin contrasting pixel outline and modest marker. This defines selected-state presentation; it does not implement later gameplay interactions.
- **D-25** — **D-25:** Use one Windows inspection hub organized by category, with common navigation for characters, items, buildings/world, UI, data/text and audio. The complete obligation register and exact resource failures remain accessible under the specification.
- **D-26** — **D-26:** The live character page gives one character most of the space, with body, equipment, movement and attack controls alongside. Side-by-side or multi-character comparison layouts were not selected.
- **D-27** — **D-27:** Include play/pause, playback-speed and single-step controls to examine grips, layering, poses and timing. These operate the inspection preview; they do not change the active-game menu's no-pause rule.
- **D-28** — **D-28:** Keep each preview's resource ID, name and status visible in a compact summary. Expand provenance, dimensions, timing and other metadata when needed. Missing/invalid-resource errors remain visible rather than being hidden in collapsed details.
- **D-29** — **D-29:** Use suitable recovered wooden buttons/borders as frames around simple dark, quiet panel interiors. Keep text, numbers and item grids clear.
- **D-30** — **D-30:** Use a clear sans-serif face for most body text and numbers, with pixel-style type for selected headings/accents. Exact font files, sizes and fallbacks remain subject to extraction/comparison and the specified readability/scaling/expanded-text checks.
- **D-31** — **D-31:** Sound effects are punchier and more game-like: sharper swings, stronger impacts and more noticeable confirmations. This is an explicit selection over the proposed restrained-audio recommendation; the minimal visual-effects preference does not make the audio equally restrained. Suitable recovered recordings still come first and require audition/adaptation.
- **D-32** — **D-32:** Keep forest, wind and fire ambience subdued so action sounds and important cues stand out. Include the required day/night/local ambience and audition controls; exact gains and mix parameters remain configurable production choices.

## judgments

- **P1** — MUST NOT present catalog entries/metadata/unfinished exports as completed assets, or label generated/adapted material as unchanged donor reuse.
- **P2** — MUST NOT use Kingdom Two Crowns or publisher logos as the title/icon/UI identity of KingdomZombies.
- **P3** — MUST NOT imply recoverable fired arrows, lootable dropped crowns, structural-guard loot, damaging bloater acid or an active-game menu that pauses play.
- **P4** — MUST NOT include dismemberment; blood presentation remains restrained.

## Executable case procedures

### export.layout

Importance: Critical. The actual exporter cannot resume until both real SDK configurations compile without weakened analyzers

Setup: Authenticated Windows x64 SDK 10.0.404, six observed immutable sources, pinned local Godot feed/cache; no new packages

Actions: Authenticate producer and retained exact source bytes → Copy tracked current source into separate owned configurations → Require enabled analyzer and warnings-as-errors properties plus exact tooling edge → Publish genuine self-contained net10.0/win-x64 ExportDebug and ExportRelease → Authenticate fresh generated sources and managed runtime dependencies; preserve all compiler evidence

Expected: Both genuine managed export configurations pass with original analyzers; no native export/content/Linux acceptance

Evidence: inputs.json, sdk-version.command.json, producer.targets, retained-sources.json, ExportDebug-policy.command.json, ExportRelease-policy.command.json, ExportDebug-publish.command.json, ExportRelease-publish.command.json, configurations.json, result.json. Group G0, order 0. Reset: Fresh owned copies and local home/temp/cache per invocation; each child bounded to 60 seconds with descendant cleanup; preserve logs

### source.tracer

Importance: High. Incorrect media/origin/native binding invalidates the production slice

Setup: Tracked bow, source record, shard and scene; authenticated pinned Windows engine; no donor folder required

Actions: Validate exact retained identity/media/recipe → Copy production subject to fresh owned project → Import and instantiate the exact native scene → Observe decoded pixels, registration, labels and unique completion

Expected: Current nonempty source-bound texture loads; native errors/timeout/missing completion fail

Evidence: inputs.json, engine.json, native-import.command.json, native-loader.command.json, result.json. Group G0, order 1. Reset: Fresh owned no-window project per invocation; bounded descendant cleanup

### source.tracer-failures

Importance: High. Missing/corrupt/origin-mismatched/interrupted content must never inherit success

Setup: Passing source.tracer prerequisites plus read-only preserved donor archive/stream for before/after identity observation

Actions: Hash original donor and production inputs → Mutate owned media/manifest/origin/scene copies independently → Run the same production validator/importer/loader and require exact failed invocations → Interrupt only after native texture load is observed → Restore the owned missing-media subject and load it afresh → Load unchanged production and rehash originals

Expected: Eleven fresh deliberate failures; fresh restored and production passes; original bytes unchanged

Evidence: inputs.json, originals-before.json, originals-after.json, mutation.json, mutation.bin, native-loader.command.json, result.json. Group G0, order 2. Reset: Copies are run-owned; restore only missing media in its disposable copy; preserve all failure records; bounded descendant cleanup

### source.provenance

Importance: Critical. False identity/coverage or publication can corrupt production provenance

Setup: Tracked partial bow shard/source and independent 180-ID register; fresh owned evidence directory; no donor root

Actions: Validate actual selected bytes, dependencies and exact origins → Run independently selected duplicate-owner/type and shared-output controls → Reject missing provenance/evidence and ordinal/UTF-8/path errors → Shuffle enumeration → Interrupt and compete before commit → Repeat publication without duplicate stable identities

Expected: 180 expected, zero completed obligations, one usable partial output; every deliberate bad subject fails; unaffected output remains inspectable

Evidence: inputs.json, pack-diagnostics.json, h01-wrong-owner.json, h05-interrupted-before-validation.json, h05-competing-lock.json, inputs-after.json, result.json. Group G0, order 3. Reset: Run-owned copies and generations only; no game/audio; preserve failure evidence and originals

### source.publication

Importance: Critical. False identity/coverage or publication can corrupt production provenance

Setup: Tracked partial bow shard/source and independent 180-ID register; fresh owned evidence directory; no donor root

Actions: Publish actual tracked bow to a validated immutable generation → Interrupt owned worker at four observed stages → Run competing separate process against its exclusive lock → Recover only dead exact invocation ownership → Reject regressed, stale, missing-byte, reparse and escaped publications → Repeat export/import and compose explicit shared identities

Expected: Four actual interrupted failed workers and actual locked competitor preserve current validated output; fresh repeated runs preserve one stable selected identity; no whole-pack completion

Evidence: inputs.json, interrupted-locked.command.json, interrupted-staged.command.json, interrupted-validated.command.json, interrupted-before-commit.command.json, competitor.command.json, publication-trials.json, inputs-after.json, result.json. Group G0, order 4. Reset: Run-owned copies and generations only; no game/audio; preserve failure evidence and originals

### export.feasibility

Importance: Critical. Authoring-path or shared-runtime dependence blocks portable Windows delivery

Setup: Authenticated official 4.7.2 mono templates, pinned editor/SDK and real existing Godot project

Actions: Authenticate selected template archive and exact Windows members → Publish actual existing managed projects self-contained → Export real tracer/resources through matching template → Inspect complete runtime dependency closure → Run clean exported application headlessly with development/runtime overrides removed

Expected: Genuine template binary/PCK, complete net10 runtime/deps and actual packaged inspection entry; foundation startup/editor-mounted PCK are supporting evidence only

Evidence: inputs.json, project-inputs.json, templates.json, managed-publish.command.json, native-export.command.json, closure.json, portable-headless.command.json, portable-headless-host.log, portable-headless-debugger.json, portable-headless-observation.json, observer-controls.json, packaged-resource.command.json, negative-controls.json, result.json. Group G0, order 5. Reset: Fresh owned source/export/portable directories and bounded descendant cleanup; preserve all attempts

### audio.feasibility

Importance: Critical. Truncated PCM, decoder cross-contamination or fabricated listening invalidates sound production

Setup: Explicit installed Python 3.12.14, UnityPy/Pillow/pyfmodex; read-only preserved exact donor owners/streams

Actions: Validate retained candidate source/WAV/PCM bundles → Reproduce one action, confirmation and ambience in separate bounded native decoder children → Rehash owning originals before/after → Reject missing children, truncated PCM, bad source/metadata and fabricated listening

Expected: Three complete independent PCM reproductions; exact hashes; listening visibly blocked pending capable human verdicts

Evidence: inputs.json, feasibility.action.command.json, feasibility.confirmation.command.json, feasibility.ambience.command.json, negative-controls.json, result.json. Group G0, order 6. Reset: One fresh owned child output per exact selection; preserve all command/output evidence; no playback/window

### media.conversion

Importance: Critical. Lost layers, trim registration, held frames or executable donor callbacks corrupt every later resource consumer

Setup: Tracked selected ZIP, pack and atlas; pinned installed Python/Pillow; no donor root is read by this case; headless Godot4.7.2

Actions: Validate every retained member/source/stream/image/registration identity and all35 runtime clips → Exercise independent H16 nonuniform/repeated/held, dense, constant, loop/endpoint and event oracles → Reject semantic corruptions after rebuilding outer ZIP hashes → Import actual atlas and construct native Animation tracks with exact key times

Expected: Complete portable kit and all native tracks/keys retained;13 deliberate corruptions rejected; actual rendered approval remains pending

Evidence: inputs.json, portable-common.command.json, conversion-controls.command.json, controls/held-out.json, controls/negative-controls.json, engine.json, native-conversion.command.json, result.json. Group G0, order 20. Reset: Fresh owned immutable invocation, bounded children; no real game window

### media.scale

Importance: Critical. Incorrect density, registration, alpha or missing original generation sources invalidates every later body/building fit

Setup: Tracked39-member pilot ZIP, actual atlas/scene/pack and common donor ZIP; installed pinned Python/Pillow and headless Godot4.7.2

Actions: Reproduce alpha cleanup/crop/nearest exports from all4 original generated masters and exact expanded prompts → Validate actual donor wall/tower/base pixels and inspected reference identities → Reject missing masters/prompts/parts, density/ground/alpha/sampling/source faults → Import/instantiate actual day/night scene and observe18 sprites, exact anchors/layers/nearest resources/contact shadows → Corrupt actual native scene filtering and observe native rejection

Expected: Reproducible common logical density with8 actual art parts,4 original masters and7 references;13 faults rejected; visual approval remains pending04-32

Evidence: inputs.json, portable-scale.command.json, negative-controls.json, scale-native-import.command.json, scale-native.command.json, scale-native-linear-negative.command.json, native-negative.json, result.json. Group G0, order 21. Reset: Fresh owned project and evidence; bounded children; no game window, user judgment or donor access

### items.weapons-food

Importance: Critical. Wrong grips, ammo, identity or missing sources invalidate required composition and inventory inputs

Setup: Tracked retained item ZIP/atlas/pack/scriptless scene; pinned installed Python/Pillow and headless Godot4.7.2; no donor access

Actions: Reproduce every component from retained donor/generated masters and bounded crop/nearest recipes → Validate exact independent IT-01â€“10 roster/grips/canonical icons/conditional overlays and distinct bundle/projectile/food identities → Reject17 semantic faults beyond outer archive hashes → Emit real selection/state/zoom button signals for each item twice; inspect exclusive groups, actual texture pixels and compact identity/status → Reach every component through native scroll ranges at1/2/4/8x; reject malformed/current and retained original-overflow scenes

Expected: Ten complete source identities and components,17 semantic rejections, all native button/zoom transitions and two native rejections; final acceptance remains pending

Evidence: inputs.json, negative-controls.json, engine.json, items-portable.command.json, items-native.command.json, items-native-negative.command.json, result.json. Group G0, order 22. Reset: Fresh owned immutable invocation and bounded headless children; no real game window

### items.wearables

Importance: Critical. Baked gear, missing limb/strap/corpse inputs or incorrect container semantics block legal equipment composition

Setup: Tracked retained wearable ZIP/atlas/pack/scriptless scene, prior ten-item pack, pinned Python/Pillow and headless Godot4.7.2; no donor access

Actions: Validate independent exact24-item union and one canonical icon per item → Reproduce every cropped torso/sleeve/leg/head/hand/foot/strap/corpse input from14 original built-in masters → Validate seven exact storage diagrams and11-slot membership; pouch stored-only and bags hidden when nested → Reject23 semantic missing-source/component/slot/baked/crown/mouth/condition/operation-bound faults → Exercise every native item/zoom and exclusive removable states twice, including current identity/status and previous-group retirement → Reach every component through native scroll ranges at1/2/4/8x; reject malformed/current and retained original-overflow scenes

Expected: Fourteen distinct masters and all modular inputs;23 semantic rejections; exact native item/storage/pixel/zoom outcomes; rendered/body-fit/hub acceptance remains pending

Evidence: inputs.json, negative-controls.json, engine.json, items-portable.command.json, items-native.command.json, items-native-negative.command.json, result.json. Group G0, order 23. Reset: Fresh owned immutable invocation and bounded headless children; no real window

### buildings.main-art

Importance: Critical. Wrong main silhouettes, missing lifecycle material or false source lineage invalidates camp resource production

Setup: Tracked portable main-building ZIP/atlas/shard, installed pinned Python/Pillow; recovered roots are unnecessary

Actions: Authenticate exact bundle/member/project bytes → Reproduce47 actual material/lifecycle/guard inputs from retained selected/generated art → Independently reconstruct three main silhouettes and12 lifecycle compositions → Reject12 independent semantic faults

Expected: Three exact materials, no baked guards, binary alpha, localized increasing damage, reusable repair/upgrade/ruin inputs and12 rejected controls

Evidence: inputs.json, buildings-art.command.json, negative-controls.json, result.json. Group G0, order 24. Reset: Fresh owned immutable source invocation; no real game window

### buildings.main-state

Importance: Critical. Premature upgrade functions, independent attachment ownership or replayed defeat corrupt required building presentation

Setup: Strict current production assembly,17 compiled controls, retained art definitions and owned native proof project using actual production DLL

Actions: Validate exact baseline/resources and14 semantic negative controls → Discover and run all17 BuildingPresentationTests with zero skips → Load the byte-identical actual production presenter in native Godot and drive36 tier/sample selections → Verify old functional tiers, seats, shared concurrent stash, quiet restore, actual guard pixels and independent attack/arrow clocks → Exercise1/2/4/8x native scroll reachability, two independent owners, captured thread guard and teardown

Expected: All declared compiled/native observations pass; no projected labels or substitute implementation; final current-hub adoption and appearance remain pending

Evidence: inputs.json, negative-controls.json, buildings-strict-build.command.json, buildings-discovery.command.json, buildings-compiled.command.json, production-assemblies.json, native-observation.json, result.json. Group G0, order 25. Reset: Fresh owned headless project and bounded child processes; no real game window

### bodies.a-art

Importance: Critical. Missing or baked body poses prevent reusable character/equipment coverage

Setup: Retained built-in generated originals, exact K14/K18/source head references, complete production atlas and strict bounded ZIP inventory

Actions: Reproduce193 parts from exact retained originals and masks/crops/nearest transforms → Assert all48 neutral poses, disjoint reusable component coverage and three distinct alpha silhouettes → Reject nine source/pose/baked/density/component/acceptance/transform faults

Expected: Three actual neutral body families with true alpha and exact source identity; final appearance judgement remains04-32/33

Evidence: inputs.json, bodies-art.command.json, art-observation.json, result.json. Group G0, order 26. Reset: Fresh immutable source-side invocation; no game window

### bodies.a-fit

Importance: Critical. Incorrect replacement loses selected equipment and character identity

Setup: Strict compiled production hub, real body/item atlases and saved fit definitions

Actions: Select CH-01 through actual category/resource controls → Select coat, cover with armor and remove armor → Assert actual exposed torso is the selected IT-13 coat

Expected: Current composition retains selected equipment and clears replaced layers; actual appearance remains04-32/33

Evidence: inputs.json, native-observation.json, matrix-observation.json, fit-observation.json, character-strict-build.command.json, character-compiled.command.json, result.json. Group G0, order 27. Reset: Fresh owned headless native hub and report; no game window

### bodies.b-wide

Importance: Critical. Missing fits hides required equipped bodies and persistent corpse contents

Setup: Pinned strict production DLL, source-replayed neutral atlas components and actual item definitions

Actions: Replay every retained body component and alpha/partition/source binding → Select actual bodies,16poses,bothfacings and every31 legal slot/item choice through production Controls → Remove and replace gear; retain logical hand identity and equipped corpse endpoints

Expected: Every declared tuple has actual current item pixels; no recolored substitute or baked armor; final readability judgment remains04-32

Evidence: body-inputs.json, body-matrix-observation.json, body-production-assemblies.json, body-strict-build.command.json, body-harness-build.command.json, result.json. Group G0, order 28. Reset: Fresh owned authenticated headless process, immutable per-invocation reports; no game window

### bodies.complete-fit

Importance: Critical. Missing fits hides required equipped bodies and persistent corpse contents

Setup: Pinned strict production DLL, source-replayed neutral atlas components and actual item definitions

Actions: Replay every retained body component and alpha/partition/source binding → Select actual bodies,16poses,bothfacings and every31 legal slot/item choice through production Controls → Remove and replace gear; retain logical hand identity and equipped corpse endpoints

Expected: Every declared tuple has actual current item pixels; no recolored substitute or baked armor; final readability judgment remains04-32

Evidence: body-inputs.json, body-matrix-observation.json, body-production-assemblies.json, body-strict-build.command.json, body-harness-build.command.json, result.json. Group G0, order 29. Reset: Fresh owned authenticated headless process, immutable per-invocation reports; no game window

### layouts.kit

Importance: High. Missing native-control states or packaged glyphs make the reusable game UI unusable

Setup: Exact selected common bundle and atlas, four inspected font candidates, three packaged font resources; no donor root or system font

Actions: Authenticate font/source/name/license/cmap records and replay font bytes → Compare all four wood atlas regions with retained original RGBA pixels → Require type/spacing/native-control/state/focus contract and reject malformed fonts and independent semantic faults → Diagnose unsupported U+10FFFF explicitly

Expected: Source-bound UI-01/UI-02 kit passes; physical input and appearance/readability remain pending04-32/33

Evidence: inputs.json, negative-controls.json, ui-source.command.json, result.json. Group G0, order 30. Reset: Fresh immutable source invocation; no game window

### layouts.theme

Importance: High. Clipped controls, lost identities or unreadable text break the required reusable UI inspection flow

Setup: Strict production and authored native harness, exact theme/fonts/common atlas and actual scriptless UiKit subtree from Inspection.tscn

Actions: Compile and discover all three ThemeLayoutTests with zero skips → Authenticate the official native binary, child PID/nonce/production DLL and owning ALC → Load actual Theme/FontFile/FontVariation/StyleBoxTexture resources and native controls → Observe font/fallback/state/focus contracts plus six authored canvas/scale combinations with normal and doubled text → Check local scrolling, copyable unbroken identity, one/zero/many resources, contrast and deliberately tiny text rejection

Expected: All source/compiled/headless assertions pass; H19/H22/H24 and UI text/control backstops retain separate final rendered review

Evidence: inputs.json, contrast.json, ui-strict-build.command.json, ui-discovery.command.json, ui-compiled.command.json, ui-harness-build.command.json, production-assemblies.json, engine.json, native-observation.json, result.json. Group G0, order 31. Reset: Fresh owned native project; bounded processes and retired native resources; zero real game windows

### layouts.identity

Importance: High. Lost current input or player/item identity makes essential UI inspection unusable

Setup: Exact04-14 theme/fonts/wood, actual24 item definitions and canonical atlas regions; source-backed foundation defaults; actual scriptless IdentityKit and production internal projector

Actions: Replay original generated master/prompt/crop/nearest-alpha recipes and exact donor cursor/keycap bytes → Parse all7 ICO frames and bind portable bytes to original artwork without claiming current export adoption → Build strict production and native proof with zero warnings/errors → Authenticate native binary/archive/PID/nonce/production DLL/owning ALC and project actual24items/18statuses/4bindings/4players → Reject duplicate keys, wrong thread, closed and detached state; replace rows and close one instance while an independent neighbor retains shared resources

Expected: Exact source/native identities pass with native editable labels and current numeric condition; final icon application and visual/prohibition judgment remain31/32/33

Evidence: identity-inputs.json, identity-source-replay.command.json, identity-strict-build.command.json, identity-harness-build.command.json, identity-engine.json, identity-production-assemblies.json, identity-native-observation.json, result.json. Group G0, order 32. Reset: Fresh owned headless project; close projector fence before owned rows/AtlasTextures, never dispose borrowed bases; zero real game windows

### layouts.identity-integrity

Importance: High. Collapsed IDs, stale bindings or incorrect condition cues can misidentify the current player, action or item

Setup: Committed source-backed identity case plus actual production IdentityKit, DT04 definitions, all24 canonical item icons and an independent live neighboring instance

Actions: Reject independent altered canonical icon/region/source hash/max/ID/player cue/master/bundle/application-scope fixtures and restore originals → Project equal-label IDs and four equal-name player cues in reversed order; prove native exact metadata and current states → Project keyboard/modifier and mouse remaps, unbound/conflict/context/reset snapshots through actual production Project → Check zero/usable/maximum numeric condition and exact single canonical icon for all24 DT04 items; reject illegal range and invented null-item wear → Verify replaced/closed owned rows and AtlasTextures retire while neighbor and shared borrowed atlas remain usable

Expected: Every original identity assertion remains and all added production-driven mutations/restorations pass with strict0warnings/errors and native exit0; no gameplay remapping, final appearance or export embedding credit

Evidence: identity-inputs.json, identity-integrity-controls.json, identity-source-replay.command.json, identity-strict-build.command.json, identity-harness-build.command.json, identity-engine.json, identity-production-assemblies.json, identity-native-observation.json, result.json. Group G0, order 33. Reset: Every fixture restores source defaults through Project; owned headless child/project only, no real window; close acceptance before row/owned texture retirement

### layouts.inventory

Importance: High. Lost identities, false condition or hidden fit failures make inventory resources misleading

Setup: Actual DT04 item definitions, canonical single item masters and native packaged theme; strict production assembly and authenticated Godot process; no gameplay transactions

Actions: Authenticate all exact dependency paths and SHA256 bindings → Discover and execute three compiled production behavior tests with zero skips → Project all17 independent state samples through the actual internal native presenter → Observe eleven named slots, both hands/ammo, four current food references and actual worn grids → Validate rotated image/footprint, counts, tooltips, weight, numeric condition/Broken and local scroll structure → Reject constructor-rebound malformed semantic data before native attachment; independent wrong hash and path controls → Check generation/thread/close/neighbor ownership and all eight UI collection backstop identities

Expected: All source/compiled/native assertions pass with real reusable Controls and text; final actual04-32 readability/focus/input review remains pending

Evidence: inventory-inputs.json, inventory-strict-build.command.json, inventory-discovery.command.json, inventory-compiled.command.json, inventory-harness-build.command.json, inventory-engine.json, inventory-production-assemblies.json, inventory-native-observation.json, result.json. Group G0, order 34. Reset: Fresh owned headless subject; each named sample replaces its rows; close acceptance before owned nodes/textures; zero real game windows

### layouts.legacy-target

Importance: Critical. Silent geometry replacement or merged target identities can misrepresent preserved belongings and failed operations

Setup: Bound source definitions, canonical icons and packaged theme; independent22 states and actual native internal presenter; no transactions or gameplay filtering

Actions: Execute5 compiled behavior tests retaining original3 and two genuine Task2 RED assertions → Inspect saved old bounds, footprints, orientation, contents and separate current destination → Observe incompatible slot, oversize stack, over-depth, missing-definition and failed-extraction resources → Inspect unlimited paged stash, character context, visual name/type/no-match and losing-operation feedback → Inspect exact NPC/corpse/ground identities, gear, contents, transfer permissions and lost access → Project independent H20 equal labels/values with touching/overlapping regions and selected warning layer → Reject independently rebound malformed constructor inputs before attachment; restore positive and close exact native owners

Expected: Every actual source/compiled/native predicate passes; saved contents remain unchanged and all target/warning identities remain exact

Evidence: legacy-inputs.json, legacy-strict-build.command.json, inventory-compiled.command.json, inventory-native-observation.json, result.json. Group G0, order 35. Reset: Fresh owned headless process; each sample replaces exact controls, closes acceptance and retires all textures/styles; final actual32-owned review remains pending

### layouts.camp

Importance: High. False money population or paid-work presentation misrepresents essential camp actions

Setup: Current creator-spec baselines, actual canonical definitions/theme/icons and internal compiled CampLayouts; no gameplay transactions

Actions: Authenticate exact bounded bindings and independent source values → Discover and run all actual compiled CampLayoutTests with zero skips → Project31 saved native states across four genuinely distinct compositions → Observe actual stash stock/proceeds, old completed capacity, fixed low/highHP repair quote and active paid progress → Observe distinct rally target/active/readiness/cash/recall reasons → Reject parent-rebound domain faults before native attachment; separate wrong hash/path and restored positive → Check synchronous thread, stale generation, detached/closed owner, replacement texture disposal and unaffected neighbor

Expected: All source/compiled/native predicates pass; real input/readability/appearance and gameplay remain separate later evidence

Evidence: camp-inputs.json, camp-strict-build.command.json, camp-discovery.command.json, camp-compiled.command.json, inventory-engine.json, inventory-production-assemblies.json, inventory-native-observation.json, result.json. Group G0, order 36. Reset: Fresh authenticated headless process; replace owned tree per sample, revoke acceptance and free exact owned Controls/atlas views; preserve neighbor

### layouts.map-warnings

Importance: Critical. Unexplored information leaks hidden warnings or false ownership misrepresent current shared state

Setup: Exact source-bound map and canonical four-player identities, packaged theme and actual internal CampLayouts; inspection only

Actions: Authenticate exact pack dependencies and all35 source examples → Execute all7 real compiled behavior tests with zero skips → Project shared minimap/expanded mask known active/destroyed portals defenses and canonical numbered named shaped colored players → Prove unknown portals absent from every native layer text tooltip and focus tree → Observe all11 notifications audible and muted with full text above ordinary panels → Independently reverse all definitions and events and then both under H23, retaining IDs quantities layers and identity → Individually execute all7 UI-FIXTURE-PREVIEW backstops with controls outside viewport → Activate native follow Previous/Next and fixture selector; reject rebound bad definitions/events plus stale thread and closed owners

Expected: All current source/compiled/native observations pass without appearance gameplay ownership or actual32/33 acceptance credit

Evidence: map-warning-inputs.json, map-warning-strict-build.command.json, camp-discovery.command.json, camp-compiled.command.json, inventory-native-observation.json, result.json. Group G0, order 37. Reset: Fresh authenticated owned headless process; replace exact Controls and CanvasLayer per sample; revoke acceptance before teardown; preserve neighbor

### layouts.session

Importance: Critical. Misleading exit and save scope risks lost progress and wrong host authority

Setup: Exact41 authored samples, six authenticated theme/data dependencies and actual compiled internal presenter

Actions: Execute compiled departure assertions → Observe distinct main solo host manual browser lobby join save and outcome compositions → Observe exact server/run IDs address capacity progress compatibility and failure distinctions → Exercise explicit host save-failure confirmation and obsolete callback fence → Observe H21 missing and invalid text audio data with exact diagnostics and no resource pass → Reject independent wrong hash and rebound invalid-domain inputs before any attachment; restore positive

Expected: Every independently named source compiled and native observation passes; actual review remains separately pending

Evidence: session-inputs.json, session-strict-build.command.json, session-compiled.command.json, inventory-native-observation.json, result.json. Group G0, order 38. Reset: Fresh owned headless native process; close exact owners; preserve independent neighbor; no session or save gameplay runs

### layouts.endpoints

Importance: Critical. Trapped controls, false pause or hidden diagnostics obscure authority and failure recovery

Setup: Twelve purpose-built settings/diagnostic state resources, exact packaged theme/fonts and actual compiled internal presenter; all six logical canvas/UI-scale pairs inside existing1280x720 headless hub

Actions: Project every independent normal/remapped/conflict/reset/form/error state → Inspect independently chosen1280x720/1920x1080 at80/100/150percent without changing world zoom → Exercise external native canvas/scale/fit/1to1pan selectors → Observe exact accented/long/unbroken/placeholder/current-reference text and packaged glyph coverage → Inspect complete file/field/value/reason diagnostics and local vertical overflow → Reject invalid endpoints and rebound invalid domains without attachment; wronghash and restored positive controls → Close exact owner and reject stale/wrongthread/closed calls while independent neighbor survives

Expected: All strict compiled/source and actual native observations pass, no clipped/trapped controls or lost identity; actual readability/input/appearance and sound-limit/listening evidence remain later-owned

Evidence: endpoints-inputs.json, endpoints-strict-build.command.json, session-compiled.command.json, inventory-production-assemblies.json, inventory-native-observation.json, result.json. Group G0, order 39. Reset: Fresh owned headless native process; all examples local and nonpersistent; exact generation owners close, no real game window

### animation.melee

Importance: Critical. Stopping stride or misidentifying equipment breaks required concurrent presentation

Setup: Strict current production assemblies and exact eight-body/fits/item/configuration bytes in an authenticated owned headless engine

Actions: Select actual logical club and knife with walking and melee → Advance explicit deterministic elapsed time → Inspect actual lower and upper segments then execute compiled assertions

Expected: Stride changes beneath the same weighted windup without swapping logical hands; no gameplay actions or damage are created

Evidence: animation-inputs.json, animation-assemblies.json, animation-observation.json, animation-strict-build.command.json, animation-harness-build.command.json, animation-native-run.command.json, compiled/tests.xml, result.json. Group G0, order 50. Reset: Detach current preview and retire owned process; no game window, sound or real input

### animation.replacement

Importance: Critical. Obsolete selections or invisible required-resource failures break live inspection integrity

Setup: Exact current production assemblies, full owned resource copy and actual native inspection root using its unchanged scene children

Actions: Prove actual CH-06 controls load before the negative fixture → Duplicate a required animation key only in the owned native project → Invoke actual production selection and inspect visible exact diagnostics and recheck transport → Restore original valid definition bytes and retain authenticated native and compiled outcomes

Expected: The current character failure remains visible with exact resource identity and cause; unaffected controls remain usable

Evidence: animation-inputs.json, animation-observation.json, animation-native-run.command.json, compiled/tests.xml, result.json. Group G0, order 51. Reset: Restore the exact owned definition in finally and close actual root and native process; no game window

### animation.bow

Importance: Critical. Invented ammunition or wrong logical hands breaks required combat presentation

Setup: Actual production preview and exact current bodies, equipment and configuration in authenticated owned headless engine

Actions: Equip actual usable bow and opposite arrow hand → Observe real live action selectors and composed output → Run 64 consecutive structural cycles in both facings with empty inventory, zero arrows and Broken ordinary condition → Reject duplicate and stale releases, then revoke structural authority through ordinary zero-ammo, non-bow and detach → Execute compiled finite-stack, legacy six-argument and explicit intrinsic behavior assertions

Expected: Finite ordinary ammunition and explicit unlimited structural ammunition preserve configured 1.2-second timing, real source arrows, logical hands and generation-bound releases without loot or gameplay events

Evidence: bow-inputs.json, animation-assemblies.json, animation-observation.json, compiled/tests.xml, result.json. Group G0, order 52. Reset: Detach owned preview and close owned headless process; no actual game window

### animation.work-special

Importance: High. Missing role/specialist actions or invented inventory/hazards misrepresent required game mechanics

Setup: Actual production preview with current fitted sources, retained generated effects, exact DT05 projectile configuration and authenticated owning Core/game assemblies

Actions: Select16 applicable work/special actions with live facing, logical tool hand and real fitted bags/clothing → Sample authored boundaries with silent restoration and explicit transient events → Change actual transfer fixture and verify real food artwork without free capacity → Release mouth spit through covering, stop at first contact and expire lifetime → Expire cosmetic stain while keeping equipped corpse → Interrupt to live locomotion and reject malformed actual work definitions

Expected: 240 independent native pose/source/grip outcomes plus distinct boundary/negative controls; original704 melee/five controls remain intact; clean authenticated exit

Evidence: work-inputs.json, work-source.json, animation-assemblies.json, animation-observation.json, result.json. Group G0, order 53. Reset: Clear work and ordinary gear through live selectors; detach owned preview; no actual game window

### appearance.corpses

Importance: Critical. Losing current corpse/loot or replaying past events breaks inspection integrity

Setup: Exact actual body/fits/item/common sources and owning Core/game assemblies

Actions: Inspect all8 collapse and persistent endpoints in both facings → Remove/repopulate current gear and selected pile/wildlife meat → Repeat silent resets/restores and enforce lifetime/resource isolation

Expected: Actual source layers remain after transient expiry; no default coat/crown/structural corpse or past effects

Evidence: appearance-inputs.json, appearance-observation.json, appearance-assemblies.json, compiled/tests.xml, result.json. Group G0, order 54. Reset: Reset and close owned native previews; no window/input/listening

### appearance.crowds

Importance: High. Metadata counts and color-only identities cannot establish actual resource production

Setup: Actual600 native equipped owners, retained wildlife/crown atlas and exact existing28 four-player palette

Actions: Instantiate100survivors300zombies and200nonempty equipped corpses/piles → Select overlapping target and change remaining gear without hiding neighbours → Transfer realK21 crown with headgear and inspect all four name/number/shape/color forms → Sample every retained wildlife cycle/final endpoint and day/night

Expected: Real source-bound actors/gear/identity/wildlife resources, no FPS/LAN/endurance inference

Evidence: crowd-inputs.json, identity-replay.json, appearance-observation.json, appearance-assemblies.json, result.json. Group G0, order 55. Reset: Close owned native crowd, preserving immutable borrowed resources; no actual game window

### audio.actions

Importance: Critical. False audio identity, truncated samples or invented hearing invalidates required action content

Setup: Tracked exact candidate bank, manifest and ZIP; existing pinned Python; actual capable source-only listener required for completion

Actions: Verify exact AU category contracts and forty distinct donor recordings → Verify integer regions, source/variant hashes and reproduce every trim/gain → Check bounded retained ZIP and thirteen exact deliberate failures plus restored positives → Require actual listener evidence; Not auditioned blocks completion

Expected: Exact immutable source/sample matrix and literal creator attestation validate; invented or extended approvals reject

Evidence: inputs.json, retained-bundle.command.json, negative-controls.json, technical.json, result.json. Group G0, order 121. Reset: Fresh owned evidence directory only; no game window, playback, donor writes or changes to production bytes

### audio.bank

Importance: Critical. Missing sound resources, invalid loops or false approval invalidate required production content

Setup: Exact tracked action/interface banks and DT23 catalog; existing pinned Python; no game or automatic audition

Actions: Validate all canonical AU categories and explicit semantic routes → Check complete source/WAV/raw bundles and exact PCM region/loop metadata → Reject false approval, wrong source/frames/loops/limits/preferences/category/portal/acid/restoration semantics → Retain Not auditioned state for every new recording

Expected: Thirty categories resolve actual recordings, configurable24/4 and80/80/60 defaults, owned current loops and truthful pending hearing

Evidence: inputs.json, retained-interface.command.json, negative-controls.json, archive-reader-controls.json, r2-publication-controls.json, result.json. Group G0, order 122. Reset: Fresh invocation-owned evidence; no source mutation, playback, game window or native mix judgment

### layouts.application-icon

Importance: High. Incorrect application identity or borrowed export credit violates source and game identity

Setup: Current exact project icon path and retained original256PNG; authenticated existing headless Godot

Actions: Authenticate exact ZIP member and current project bytes → Reject missing changed old and wrongcase icon subjects → Decode/import actual256PNG and compare alpha/visibleRGB natively

Expected: Exact source and current native PNG pass; all invalid subjects reject; no Windows executable or visual judgment claimed

Evidence: inputs.json, controls.json, engine.json, native-observation.json, result.json. Group G0-application-icon, order 1. Reset: Dispose only owned native images and keep retained archive/ICO unchanged

### audio.lifecycle

Importance: Critical. Stale native players or historical cue replay breaks current inspection state

Setup: Fresh strict production assemblies and authenticated headless native engine

Actions: Inspect all30categories and91routes through actual players → Compare exact native PCM and integer loop boundaries → Restore current loops19times without historical events → Retire A then reject its stale callback in replacement A → Restore valid day-fire loops beside missing action bank,activate reachable Stopall and verify exact native resource retirement → Observe all eight independently named AUDIO-AUDITION backstops and real hub loading/navigation

Expected: Every independent native assertion passes; hearing and final native mix remain04-32/33

Evidence: audio-inputs.json, audio-compiled/tests.xml, inventory-production-assemblies.json, inventory-engine.json, inventory-native-observation.json, result.json. Group G0-audio-native, order 3. Reset: Close acceptance then stop/free all owned players and streams; retire native controls and copied project

### audio.heldout

Importance: Critical. Wrong limits or stale audio ownership misrepresents current state and muted warnings

Setup: Current strict production/controller and actual-project hub in two authenticated owned headless processes

Actions: Assert independent1/7/128global and1/2/24repeated input endpoints with actual admission/priority and source overlap ceilings → Exercise actual24variant auditions,cooldown and committed-event deduplication → Restore three independently chosen loopsets without historical cues → Replace eleven owners and actual hub categories,then reject stale callbacks → Mute each configured bus and retain full critical warning → Independently assert all eight audio backstops and reject wrong current hash/rebound invalid gain before restored positive

Expected: Every required ordered source/compiled/native observation passes; source/headless supplies no hearing or final mix judgment

Evidence: audio-inputs.json, audio-compiled/tests.xml, inventory-native-observation.json, audio-hub-observation.json, audio-native-observation.json, result.json. Group G0-audio-native, order 4. Reset: Close every owner and free all players/streams/controls; native groups report naturally before exit

### data.foundation

Importance: Critical. Invalid shared baseline or silent fallback would prevent a coherent run and lose source identity

Setup: Pinned committed specification, actual item/body packs, authored separate configs and field inventory; no game window

Actions: Read strict bounded UTF-8 documents with exact ordinal paths → Walk every field and its meaning unit default range dependencies and source → Compare complete numerical source rows and prose-derived relationships → Reject independent malformed documents missing fields references bounds and incoherent relationships → Accept independent designer edits without a hidden fallback

Expected: Exact baseline/source crosswalk and strict validation pass; actual gameplay and final resource inspection remain pending

Evidence: inputs.json, source-spec.md, source-crosswalk.json, fields.json, negative-controls.json, valid-edit.json, result.json. Group G0-data, order 40. Reset: Fresh immutable source invocation; isolated in-memory edits only; no process or window

### data.items-combat

Importance: Critical. Invalid shared baseline or silent fallback would prevent a coherent run and lose source identity

Setup: Pinned committed specification, actual item/body packs, authored separate configs and field inventory; no game window

Actions: Compare every24-item source row and actual icon/name/appearance/fit binding → Compare attack armor projectile contact storage timing and load baselines directly with source → Validate legal hand pairs stash geometry armor condition and reference relationships → Reject independent missing corrupt nonfinite ordinal duplicate bounds and precision-loss fixtures → Accept designer edits while keeping source defaults separately authenticated

Expected: Exact baseline/source crosswalk and strict validation pass; actual gameplay and final resource inspection remain pending

Evidence: inputs.json, source-spec.md, source-crosswalk.json, fields.json, H24-precision.json, negative-controls.json, legal-loadouts.json, valid-edit.json, result.json. Group G0-data, order 41. Reset: Fresh immutable source invocation; isolated in-memory edits only; no process or window

### data.economy

Importance: Critical. Invalid shared work and economy defaults can lose paid orders or possessions

Setup: Current creator baseline section bytes, actual production dependencies, exhaustive field inventory; no game window

Actions: Read bounded strict documents → Validate every field and source rule → Check actual production references → Reject independently altered source defaults and missing metadata

Expected: Complete exact source baseline with no hidden fallback

Evidence: inputs.json, source-spec.md, source-crosswalk.json, fields.json, negative-controls.json, result.json. Group G0-data, order 42. Reset: Independent immutable source invocation; no game process

### data.world

Importance: Critical. Invalid shared work and economy defaults can lose paid orders or possessions

Setup: Current creator baseline section bytes, actual production dependencies, exhaustive field inventory; no game window

Actions: Read bounded strict documents → Validate every field and source rule → Check actual production references → Reject independently altered source defaults and missing metadata

Expected: Complete exact source baseline with no hidden fallback

Evidence: inputs.json, source-spec.md, source-crosswalk.json, fields.json, negative-controls.json, result.json, integration-mismatches.json. Group G0-data, order 43. Reset: Independent immutable source invocation; no game process

### data.zombies

Importance: Critical. Invalid guaranteed inventories or progression can lose belongings and invalidate a saved run

Setup: Current exact source sections and production resource dependencies; no game window

Actions: Strict bounded parse → Verify exhaustive fields and exact tables → Validate actual references and guaranteed placements → Reject independent scalar and relationship defects

Expected: Complete editable zombie baseline without hidden constants or invalid guaranteed loadouts

Evidence: inputs.json, source-spec.md, fields.json, source-crosswalk.json, negative-controls.json, guaranteed-loadouts.json, result.json. Group G0-data, order 44. Reset: Fresh immutable source invocation, no gameplay process

### data.remaining

Importance: Critical. Invalid guaranteed inventories or progression can lose belongings and invalidate a saved run

Setup: Current exact source sections and production resource dependencies; no game window

Actions: Strict bounded parse → Verify exhaustive fields and exact tables → Validate actual references and guaranteed placements → Reject independent scalar and relationship defects

Expected: Complete editable zombie baseline without hidden constants or invalid guaranteed loadouts

Evidence: inputs.json, source-spec.md, fields.json, source-crosswalk.json, negative-controls.json, reference-status.json, result.json. Group G0-data, order 45. Reset: Fresh immutable source invocation, no gameplay process

### buildings.defense-art

Importance: Critical. Missing defense materials, previous tiers or traceable lifecycle art invalidates required production

Setup: Tracked defenses ZIP/atlas/shard and pinned existing Python/Pillow; no donor folder or game window

Actions: Authenticate complete retained members and selected donor lineage → Replay46 actual art parts from retained3 generated masters and selected source pixels → Independently validate six tiers, localized damage, matched ruins and nine exact frames → Reject16 material/lifecycle/source/alpha/frame/acceptance faults

Expected: Six actual wood/reinforced-wood/metal tiers and BL10–12 components pass exact source replay and all16 independent controls

Evidence: inputs.json, defenses-art.command.json, negative-controls.json, result.json. Group G0-defenses, order 1. Reset: Fresh immutable owned source invocation; no real game window

### buildings.defense-state

Importance: Critical. Premature guards, wrong rebuild tier, invented structural loot or unsafe definitions invalidate required production

Setup: Current strict production DLL, exact portable defense/guard definitions and authenticated official headless Godot; no real game window

Actions: Reconstruct five exact donor guard poses from disjoint torso/arm-bow layers and unchanged arrow → Build actual production and emitted native harness without warnings → Drive132 independent native tier/site/work/damage/ruin selections and compare imported actual pixels → Aim/fire three guards independently, retain captured arrows and exercise128 sustained per-facing intrinsic cycles → Independently exercise H12 tier/guard/rebuild and H14 empty/no-loot transitions → Reject35 semantic constructor faults separately from6 missing/hash/dimension media faults; verify owner/thread/late fences

Expected: All actual native/semantic/source outcomes pass; original main two-guard profile remains separately verified; actual appearance/input/named judgment remain pending

Evidence: inputs.json, defenses-guard-source.command.json, defenses-strict-build.command.json, production-assemblies.json, engine.json, native-observation.json, result.json. Group G0-defenses, order 2. Reset: Fresh owned headless project; ResetDefense cancels current arrows, ordinary selectors reset samples, Detach closes acceptance before late results

### export.profile

Importance: Critical. Wrong delivery entry or accidental ordinary activation blocks inspection delivery and normal startup

Setup: Permanent production profile and single existing preset; authenticated pinned Windows engine; fresh owned source copy

Actions: Validate exact canonical keys, preset feature/options, portable resources and unchanged ordinary configuration → Import owned copy with authenticated pinned engine → Read genuine base/actual/explicit-feature settings and load Inspection texture

Expected: Ordinary and wrong-case query resolve Foundation; canonical explicit feature resolves Inspection and loads 42 visible bow pixels; no native export credit

Evidence: inputs.json, profile-inputs.json, profile-import.command.json, profile-settings.command.json, profile-observations.json, result.json. Group G0-export-profile, order 350. Reset: Fresh owned disposable project; no production mutation or autoload; bounded hidden children with descendant cleanup; immutable evidence retained

### export.profile-boundaries

Importance: Critical. Wrong delivery entry or accidental ordinary activation blocks inspection delivery and normal startup

Setup: Permanent production profile and single existing preset; authenticated pinned Windows engine; fresh owned source copy

Actions: Mutate independent owned profile/config/preset/resource copies and require exact fresh diagnostics → Observe native missing override, case normalization and global activation on separate usable copies → Exercise real child default/explicit env/cwd, deadline/output/nonzero/spawn rejection and descendant cleanup → Validate original case subsets and global ID/order uniqueness; finish with fresh valid native control

Expected: Every deliberate damaged source is rejected independently; native API semantics match pinned behavior; helper bounds and original registry identities survive

Evidence: inputs.json, ordered-cases.json, source-controls.json, native-controls.json, helper-controls.json, trials/*/result.json, result.json. Group G0-export-profile, order 351. Reset: Fresh owned disposable project; no production mutation or autoload; bounded hidden children with descendant cleanup; immutable evidence retained

### hub.source

Importance: Critical. One invalid resource must not hide valid content or erase required obligations

Setup: Strict actual C# production build, compiled loader controls and isolated byte-identical native hub with real assembly bindings

Actions: Run compiled loader boundary controls → Drive actual native category/search/filter/details/copy signals → Observe frame-driven loading/cancel/recheck, missing register/media and restored native bow

Expected: Nonzero compiled discovery and actual native observations; valid bow survives independent failures with complete180 census

Evidence: compiled.command.json, inputs.json, native-build.command.json, native-assemblies.json, native-hub.command.json, native-observation.json, result.json. Group G0-hub, order 1. Reset: Owned isolated resources; restore every mutated file and destroy the scene; no game window

### hub.lifecycle

Importance: Critical. Stale callbacks or duplicated input must not replace the selected preview or accumulate native owners

Setup: Actual compiled production CSharp scene and native Controls in an isolated headless project

Actions: Inspect explicit preview transport → Drive owned keys, category restoration and repeated reset → Observe late generation rejection and teardown

Expected: One current preview; text and popups own input; paused stepping uses actual authored samples; obsolete results are rejected

Evidence: native-observation.json, native-build.command.json, native-assemblies.json, native-hub.command.json, result.json. Group G0-hub, order 2. Reset: Close acceptance, cancel work, disconnect handlers and retire every owned preview; no game window

### source.case-discovery

Importance: Critical. Missing or ambiguous executable scenarios invalidate verification selection

Setup: Tracked canonical CommonJS cases and fresh owned real fixture modules; no game window

Actions: List the actual CLI catalog and reject unknown selections → Load real fixtures without running case bodies → Reverse directory enumeration → Reject each independent invalid module and registration

Expected: Stable complete catalog; canonical future module discovered; exact invalid inputs fail visibly

Evidence: inputs.json, cli-list.command.json, unknown-case.command.json, unknown-group.command.json, controls.json, result.json. Group G0-integration, order 360. Reset: Fresh run-owned fixture directories and isolated registries; no production fixture edits

### source.packaged-expectations

Importance: Critical. A missing or output-derived expectation set can conceal missing required content

Setup: Tracked canonical CommonJS cases and fresh owned real fixture modules; no game window

Actions: Authenticate all independent source and package bytes → Reject each isolated reference/header/row/alias/tamper control → Publish and read an isolated generation with all 180 exact rows → Keep valid independent output and all missing obligations visible → Revalidate unchanged production

Expected: Byte-identical 180 rows; one partial bow, 28 pending shards, zero completed obligations; every invalid expectation fails

Evidence: inputs.json, controls.json, publication.json, result.json. Group G0-integration, order 361. Reset: Fresh run-owned fixture directories and isolated registries; no production fixture edits

### export.application-icon

Importance: High. Missing or aliased application art invalidates current delivery identity

Setup: Current exact icon/project/profile bytes and original historical ordinary-config contract

Actions: Validate current source icon and bounded resource closure → Independently remove corrupt and alias icon/config in owned copies → Restore each positive and preserve original sourceContract/support scope

Expected: Current exact source icon passes; every changed/missing/aliased subject rejects; final native export remains31-owned

Evidence: icon-controls.json, result.json. Group G0-source-icon-profile, order 1. Reset: Restore only disposable subject files; never mutate current source or retained archive

### text.catalog

Importance: High. Missing or mutated text/field defaults makes required content unusable or silently changes authoring authority

Setup: Actual authored definitions and exact English/expanded replacement resources; no game window

Actions: Compare every current source label and required copy with stable catalog messages → Check typed placeholders plurals current-binding actions and twice-length replacements → Authenticate full packaged field documentation and reject independent corrupt controls

Expected: Every required current text/source binding field default and packaged documentation passes with explicit negative controls

Evidence: inputs.json, negative-controls.json, fields.json, result.json. Group G0-text, order 1. Reset: Read-only source checks; discard detached corrupt subjects

### text.compatibility

Importance: Critical. Lost saved geometry, obsolete loops or replayed historical events violates persistent state and content identity

Setup: Current strict actual production DLL, authenticated official headless Godot, isolated controller and separate actual KingdomZombies hub process

Actions: Inspect actual current fields defaults typed messages plurals current binding and packaged documentation → Independently exercise all16 native UI-TEXT-STATUS and DATA-TEXT-INSPECTOR states → Restore current gear corpse ruin work and loop owners silently with exact old geometry and separate destination → Reject exact missing malformed and corrupt subjects then restore actual valid definitions → Select actual production hub Data/text entries through native navigation signals

Expected: All individual actual production-native assertions pass, exact failures remain visible and independent resources remain usable

Evidence: inputs.json, engine.json, production-assemblies.json, text-controller-native-observation.json, text-hub-native-observation.json, native-summary.json, result.json. Group G0-text, order 2. Reset: Close only owned preview lifetimes and retire native controls after immutable observations; no save or gameplay transactions

### text.support-catalog

Importance: Critical. Missing fixture or falsely accepted delivery prevents honest inspection and final package verification

Setup: Current fixed fixture corpus, exact coverage matrix, authored source cases and delivery guides

Actions: Authenticate every actual fixture and current case module → Compare all34H/70backstops/24AC/32D/four procedures with independent matrix → Corrupt coverage/bindings/docs/acceptance independently and restore → Prove dynamic publication values cannot cause documentation cycles while independent byte validation rejects tampering

Expected: Complete source crosswalk with exact immutable resources and honest pending judgments

Evidence: controls.json, result.json. Group G0-text, order 3. Reset: Detached negative controls; authored production bytes stay unchanged

### text.support-native

Importance: Critical. Unreachable or unauthenticated support views hide missing runtime resources

Setup: Current strict actual production DLL and authenticated official headless engine; isolated controller and actual hub

Actions: Read actual bounded DT26 fixture catalog and DT27 startup guides through current native controls → Reject missing delivery and invalid catalog/fixture/guide/schema/acceptance independently → Restore actual current support and select both views in the real production hub

Expected: All independent observations pass with natural completion, exact assembly/PID/nonce identity and clean logs

Evidence: inputs.json, engine.json, production-assemblies.json, text-controller-native-observation.json, text-hub-native-observation.json, native-summary.json, result.json. Group G0-text, order 4. Reset: Dispose owned previews and controls after materializing immutable observations

### world.map

Importance: Critical. Wrong authored geometry,missing food,portal bypass or false exploration invalidates the entire world pack

Setup: Tracked world/common bundles and atlas; pinned existing Python/Pillow and authenticated headless Godot; no donor directory

Actions: Replay42 source-bound art parts and212 retained members → Independently compare every mirrored baseline site and separate geometry/references → Reject23 deliberate semantic faults → Instantiate13 real PackedScenes with275 declared composition nodes and inspect native pixels,stock,clearing,food,reveal,identity and moving rally resources

Expected: All exact world resources and independent malformed controls pass; real game windows and named judgment remain04-32/33

Evidence: inputs.json, negative-controls.json, world-map-source.command.json, engine.json, native-observation.json, result.json. Group G0-world, order 1. Reset: Fresh owned immutable invocation; each native owner removed and freed before next sample

### world.effects

Importance: Critical. Active ruins,misleading loot/hazard presentation or obsolete effects break required mechanic and lifetime integrity

Setup: Source-bound world/effects/common resources,current strict production DLL and authenticated headless Godot;no donor or game window

Actions: Independently authenticate176 exact donor parts,six clip timings and51 required effect IDs → Restore24 current world/portal/stock/clearing/loop samples silently → Trigger each effect explicitly and verify limits,expiry,first contact,Reset/epoch/thread/neighbor/closed boundaries → Reject31 parent-rebound malformed semantic subjects before native attachment,plus separate wrong hash and restored positive → Authenticate native imported alpha/nonzero RGB with deterministically disposed proof-owned Images

Expected: Every source/native resource and rejection control passes; actual visual/blood/mechanic/listening judgments remain04-32/33

Evidence: inputs.json, world-effects-source.command.json, world-effects-strict.command.json, production-assemblies.json, engine.json, native-observation.json, result.json. Group G0-world, order 2. Reset: Close each owned presenter;retire nodes/resources and reject old generations synchronously

### world.inspector

Importance: Critical. Unreachable authored content or one bad item hiding valid world breaks the inspection pack

Setup: Current strict production assembly, actual CSharp hub and authenticated headless Godot in owned isolated project

Actions: Select all24items and9structure tiers through real hub signals → Select every world/effect current state,pan edges and zoom without changing authored positions → Observe six independently named WORLD-ITEM-PREVIEW backstops → Restore damaged media and close owned native scene

Expected: Actual production resource identities and every individual outcome pass; actual input/draw/readability remain04-32

Evidence: inputs.json, production-assemblies.json, engine.json, native-observation.json, result.json. Group G0-world, order 3. Reset: Revoke and retire all previews; restore owned copied bytes only

### world.replacement

Importance: Critical. A stale owner or failed entry must not replace valid current authored content

Setup: Current strict production assembly, real native hub and six compiled production fence/backstop tests

Actions: Submit late A after selecting B through actual root acceptance fence → Rapidly select/reset effects,buildings and authored map → Independently observe six wearable/building replacement backstops → Restore valid media and retire owned native scene

Expected: All native and compiled outcomes pass independently,with stable authored positions and no historical events

Evidence: inputs.json, production-assemblies.json, native-observation.json, compiled/tests.xml, compiled/tests.json, result.json. Group G0-world, order 4. Reset: Revoke and retire every preview;restore owned copied media and definitions


## Additional implemented source census procedures

These four actual source checks add coverage records. Catalog registration supplies no execution, acceptance, final package or judgment verdict.

### content.source-boundaries

Importance: Critical. A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity

Owner: 04-23 Task1. Requirements: ASSET-01, ASSET-02, ASSET-04, ASSET-05. Source acceptance coverage: AC-13, AC-15. Actual acceptance remains pending.

Setup: Actual retained canonical interface archive and independently constructed boundary subjects; all other source caps remain32MiB

Actions: Authenticate actual canonical source bytes and exact recorded size/hash → Exercise exact64MiB and64MiB+1 canonical reads independently → Reject wrongcase/lookalike/default-path oversize and wronghash source identities → Restore the original real archive and preserve every production byte

Expected: Only the exact actual source identity receives the bounded64MiB exception; all invalid subjects fail visibly

Evidence: inputs.json, boundaries.json, result.json. Group G0-content, order 60. Reset: Independent invocation-owned scratch subjects, never mutate retained production archive

Source module: tools/phase4/cases/census.cjs; SHA256 ad6ccfd557d998437b347af2cd31ff84990e0797bc8b3e477e0bf3be6b69fd7e.

### content.archive-boundaries

Importance: Critical. A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity

Owner: 04-23 Task1. Requirements: ASSET-01, ASSET-02, ASSET-04, ASSET-05. Source acceptance coverage: AC-13, AC-15. Actual acceptance remains pending.

Setup: Actual retained common and13other archives, immutable pinned container/member identities and independent small ZIP subjects

Actions: Authenticate complete source file before parsing/inflation → Stream all real members through the production cap/CRC/hash validator → Target the same production bound enforcer with exact/over and unauthorized-member subjects → Reject actual corrupt CRC/data, unsafe/duplicate/case aliases, fullhash tamper and lookalike containers then restore

Expected: Only the exact actual source identity receives the bounded64MiB exception; all invalid subjects fail visibly

Evidence: archive-controls.json, archive-members.json, other-archives.json, result.json. Group G0-content, order 61. Reset: Independent invocation-owned scratch subjects, never mutate retained production archive

Source module: tools/phase4/cases/census.cjs; SHA256 ad6ccfd557d998437b347af2cd31ff84990e0797bc8b3e477e0bf3be6b69fd7e.

### content.source-relations

Importance: Critical. A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity

Owner: 04-23 Task1. Requirements: ASSET-01, ASSET-02, ASSET-04, ASSET-05. Source acceptance coverage: AC-13, AC-15. Actual acceptance remains pending.

Setup: Current actual133file model,14retained production archives and independent mutations of authenticated source/part/recipe relationships

Actions: Authenticate and stream every retained archive through existing bounded CRC/SHA reader → Match actual retained primary JSON/atlas/recipe/generated-master/prompt/member inventories and exact source owner/raw/stream identities → Reject other valid source/member/recipe/part roles and unsafe/duplicate/callback/range mutations at the same production guards → Restore actual inputs and independently repeat complete retained source relation validation

Expected: Real retained bytes and relationships pass; each named independent defect fails for its specific source/field reason, never a fabricated acceptance receipt

Evidence: retained-positive.json, retained-controls.json, retained-restored.json, result.json. Group G0-content, order 62. Reset: Only detached in-memory metadata mutations; every tracked producer/archive byte stays fixed

Source module: tools/phase4/cases/census.cjs; SHA256 ad6ccfd557d998437b347af2cd31ff84990e0797bc8b3e477e0bf3be6b69fd7e.

### content.domain-relations

Importance: Critical. A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity

Owner: 04-23 Task1. Requirements: ASSET-01, ASSET-02, ASSET-04, ASSET-05. Source acceptance coverage: AC-13, AC-15. Actual acceptance remains pending.

Setup: Three actual packaged donor fonts and mixed generated/donor work master, immutable common/work archives and current runtime resources

Actions: Link explicit font roles and candidates to authenticated original donor member bytes, retaining shared body/fallback identity → Check actual generated master/cell/crop/recipe/prompt and donor hammer relationships → Reject other valid sources/roles, wrong regions/references, duplicate identities and forbidden callbacks/gameplay mutation → Restore actual source/runtime bytes after each independent guard

Expected: Exact real domain relationships and all independent failures/restored positives pass with0accepted

Evidence: inputs.json, domain-positive.json, domain-controls.json, result.json. Group G0-content, order 63. Reset: Detached metadata and bounded byte callbacks only, no tracked source/resource mutation

Source module: tools/phase4/cases/census.cjs; SHA256 ad6ccfd557d998437b347af2cd31ff84990e0797bc8b3e477e0bf3be6b69fd7e.


### content.census

Importance: Critical. A source archive exception applied to an alias or unchecked bytes breaks bounded publication identity

Owner: 04-23 Task1. Requirements: ASSET-01, ASSET-02, ASSET-04, ASSET-05. Actual appearance/listening/platform/acceptance remains pending.

Setup: All actual current production shards/resources and independently fixed180 authority; bounded retained source archives, completed Core decoder and actual current Godot Root

Actions: Decode and authenticate actual distinct resources/dependencies/shared mappings against independent180 IDs → Reject duplicate-inflated totals,missing unique IDs,partial/empty dependencies,precision and conflicting paths → Preserve valid sibling visibility and order-independent identities without accepted judgment credit → Publish actual full retained source generations atomically and prove interruption/recovery/restored identity → Reach each actual current resource through the genuine compiled native Root and current source/assembly/ALC/PID/nonce closure

Expected: All180 actual source resources and exact independent controls pass; accepted remains0 and later judgment owners stay pending

Evidence: census-inputs.json, census-controls.json, census-source-proof.json, result.json. Group G0-content, order 64. Reset: Owned disposable subjects only; no production mutation; original legacy fixtures preserved

Source module: tools/phase4/cases/census.cjs; SHA256 ad6ccfd557d998437b347af2cd31ff84990e0797bc8b3e477e0bf3be6b69fd7e.

## Current runner and actual evidence procedures

All actual runs require the independent current review. Each case keeps its own outcome.

### content.native-permissions

Exact final native guarded permissions and held-out content boundaries. Critical. Every permitted native operation must belong to an actual exact guarded member, and invalid content must stay visible.

**Setup and input** Current authored source, pinned SDK and xUnit assembly, exact completed package and independent 180-obligation authority. Current exact resource and source identities from the case setup.

**Actions** Authenticate actual independently defined obligations and all retained current sources → Exercise independent shared, conflicting, encoded and reordered packaged bindings and interrupted publication → Build the current solution, discover every exact guarded-member test UID and execute each one with the existing limits → Require authentic passing guarded positives and rejected wrong-path, sibling, unguarded, deferred and worker callsites.

**Expected** Every exact current native permission case and held-out content control passes with zero skips and zero accepted obligations.

**Evidence** census-controls.json · census-source-proof.json · native-permissions-packaged-controls.json · native-permissions-inputs.json · native-permissions-selection.json · native-permissions-execution.json · result.json.

**Run and reset** G0-content / 65 / owned-source-or-headless-processes. Existing exact case/child deadlines; no extension implied. Use independent invocation-owned directories and leave current source and executable evidence fixed. Use independent invocation-owned directories and leave current source and executable evidence fixed.

### runner.prelaunch

Current Phase 4 review and prelaunch gate. Critical. Running an unreviewed build invalidates all rendered acceptance.

**Setup and input** Current independent review, exact saved selection and selected no-window receipts.

**Actions** Authenticate the committed independent review and each finding disposition → Verify every selected pre-window source receipt and later outcome.

**Expected** Current independent review, selected groups and eligible no-window prerequisites authorize the prelaunch gate.

**Evidence** result.json.

**Run and reset** G0-runner / 300 / owned-source-or-headless-processes. Validate the actual committed records without opening a game window. Preserve prior invocations.

### runner.selection

Exact current and inherited selected cases. Critical. Omitting a Critical, new, changed or affected check invalidates the planned evidence.

**Setup and input** Current independent review and exact committed selection.

**Actions** Derive phase-local impact independently from changed source, shared inputs and inherited consumers → Check exact selected and omitted cases, roles and ordering.

**Expected** Every required current or inherited case remains selected with an exact process and cleanup declaration.

**Evidence** result.json.

**Run and reset** G0-runner / 301 / owned-source-or-headless-processes. Validate the actual committed records without opening a game window. Preserve prior invocations.

### runner.prelaunch-source

Detached review, selection and runner evidence controls. Critical. Missing, misbound or superseded evidence must fail before a game window opens.

**Setup and input** Fixed current source and invocation-owned negative fixtures.

**Actions** Exercise detached review, selection and actual CLI dispatch failure controls → Reject invalid results, process failures and rewritten history → Exercise current inherited project resource roles without a game window.

**Expected** Every invalid subject fails explicitly and bounded source controls establish zero actual acceptance.

**Evidence** immutable-control.json · runner-controls.json · selection-controls.json · inherited-resource-control.log · result.json.

**Run and reset** G0-runner / 304 / owned-source-or-headless-processes. Bound each nested CLI to 15 seconds. Preserve all prior records.

### acceptance.judgments

Named current media and separate prohibition judgments. Critical. A missing or stale listening or real named verdict cannot support actual acceptance.

**Setup and input** Fixed current source and invocation-owned negative fixtures. Current exact resource and source identities from the case setup.

**Actions** Authenticate actual PCM WAV format, exact sample and version, and independent named verdict receipts → Check one distinct named result for each prohibition and exact current case images.

**Expected** Every missing, stale, forged or failed named result rejects without mechanical listening or visual credit.

**Evidence** judgments-controls.json · result.json.

**Run and reset** G0-runner / 302 / owned-source-or-headless-processes. 15 seconds for the nested CLI; source-only fixture evaluation. Keep original fixtures, review records and every prior invocation immutable. Keep original fixtures, review records and every prior invocation immutable.

### acceptance.artifacts

Current artifact, scope and exact completion inventory. Critical. An incomplete or stale artifact inventory can hide unverified production or selected failures.

**Setup and input** Current completion index, eligible source receipts and the actual rendered and named outcomes.

**Actions** Compare exact tracked artifacts and reviewed symbols, case exports, native tuples and retained archive members → Reconcile the complete current run, earlier failure history, and required coverage.

**Expected** Missing, stale or unexpected artifacts, signatures, cases and final evidence are rejected.

**Evidence** result.json.

**Run and reset** G0-runner / 303 / owned-source-or-headless-processes. Verify current completion data without opening a game window. Preserve prior invocations.

### acceptance.artifacts-source

Detached artifact inventory and receipt-history controls. Critical. Source qualification must expose invalid evidence independently of the final closure result.

**Setup and input** Fixed current source and invocation-owned negative fixtures.

**Actions** Validate detached artifact, source signature and native permission controls → Require independent prerequisites while a completion check is in progress.

**Expected** An active completion check does not require its own receipt and every independent missing or failed receipt blocks closure.

**Evidence** artifacts-controls.json · result.json.

**Run and reset** G0-runner / 305 / owned-source-or-headless-processes. Verify isolated fixtures without opening a game window. Preserve prior invocations.

### heldout.H01

H01: Same-named donors in different owning files/types and explicitly shared outputs retain the right source/output identities; AC-02/03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H01 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H01: Same-named donors in different owning files/types and explicitly shared outputs retain the right source/output identities; AC-02/03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1000 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H02

H02: A selected output with missing origin, identity, recipe or required evidence cannot obtain completed provenance; AC-03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H02 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H02: A selected output with missing origin, identity, recipe or required evidence cannot obtain completed provenance; AC-03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1001 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H03

H03: Case/encoding variants cannot silently alias stable IDs/source locators or portable paths; AC-02/03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H03 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H03: Case/encoding variants cannot silently alias stable IDs/source locators or portable paths; AC-02/03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1002 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H04

H04: Reordering donor/manifest enumeration preserves exact output-to-origin resolution; AC-02/03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H04 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H04: Reordering donor/manifest enumeration preserves exact output-to-origin resolution; AC-02/03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1003 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H05

H05: Interrupted/competing production work preserves originals and cannot publish unvalidated output as verified; AC-02/03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H05 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H05: Interrupted/competing production work preserves originals and cannot publish unvalidated output as verified; AC-02/03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1004 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H06

H06: A missing unique obligation remains a failure even when duplicate rows inflate the apparent total to 180; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H06 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H06: A missing unique obligation remains a failure even when duplicate rows inflate the apparent total to 180; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1005 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H07

H07: Explicitly shared files may satisfy several obligations without collapsing distinct item/resource identities; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H07 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H07: Explicitly shared files may satisfy several obligations without collapsing distinct item/resource identities; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1006 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H08

H08: Empty/partial packs and absent or empty required output dependencies cannot count as completed coverage; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H08 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H08: Empty/partial packs and absent or empty required output dependencies cannot count as completed coverage; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1007 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H09

H09: Shuffling resource/mapping order preserves the actual covered-ID set and completeness result; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H09 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H09: Shuffling resource/mapping order preserves the actual covered-ID set and completeness result; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1008 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H10

H10: Unique integer obligation coverage is exact; percentages, duplicate files or rounding cannot certify an incomplete pack; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H10 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H10: Unique integer obligation coverage is exact; percentages, duplicate files or rounding cannot certify an incomplete pack; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1009 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H11

H11: Output generation/import interrupted partway through remains visibly incomplete rather than gaining metadata-only coverage; AC-01/16/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H11 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H11: Output generation/import interrupted partway through remains visibly incomplete rather than gaining metadata-only coverage; AC-01/16/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1010 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H12

H12: Empty/last-arrow and broken/usable transitions, required tier/guard counts and persistent-corpse boundaries preserve the intended presentation; AC-06/07/08/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H12 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H12: Empty/last-arrow and broken/usable transitions, required tier/guard counts and persistent-corpse boundaries preserve the intended presentation; AC-06/07/08/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1011 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H13

H13: Overlapping coat/armor/belt/bag/hand layers and nearby bodies keep required gear and target identity readable; AC-05/07/18/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H13 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H13: Overlapping coat/armor/belt/bag/hand layers and nearby bodies keep required gear and target identity readable; AC-05/07/18/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1012 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H14

H14: Empty hands/ammunition/slots, empty versus nonempty corpses and the structural-guard exception do not invent equipment or loot; AC-06/07/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H14 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H14: Empty hands/ammunition/slots, empty versus nonempty corpses and the structural-guard exception do not invent equipment or loot; AC-06/07/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1013 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H15

H15: Facing/action/gear order changes retain correct final equipment, occlusion and logical hand identity; AC-04/05/06/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H15 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H15: Facing/action/gear order changes retain correct final equipment, occlusion and logical hand identity; AC-04/05/06/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1014 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H16

H16: Donor pixel registration, offsets, nonuniform frame timing, held/repeated frames and loop/endpoints survive conversion/composition; AC-03/04/05/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H16 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H16: Donor pixel registration, offsets, nonuniform frame timing, held/repeated frames and loop/endpoints survive conversion/composition; AC-03/04/05/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1015 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H17

H17: Repeated preview reset/state restoration leaves one current composition/loop/corpse state without replaying past one-shots; AC-04/07/14/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H17 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H17: Repeated preview reset/state restoration leaves one current composition/loop/corpse state without replaying past one-shots; AC-04/07/14/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1016 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H18

H18: Mid-action body/gear changes immediately clear replaced visuals, permit a clean restart and retain concurrent walking/combat presentation; AC-04/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H18 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H18: Mid-action body/gear changes immediately clear replaced visuals, permit a clean restart and retain concurrent walking/combat presentation; AC-04/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1017 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H19

H19: Supported resolution/UI-scale endpoints, expanded text and configured sound limits remain inspectable; AC-11/12/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H19 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H19: Supported resolution/UI-scale endpoints, expanded text and configured sound limits remain inspectable; AC-11/12/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1018 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H20

H20: Equal labels/values and touching/overlapping UI or legacy-grid regions do not merge identities or hide critical warning/state information; AC-10/11/14/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H20 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H20: Equal labels/values and touching/overlapping UI or legacy-grid regions do not merge identities or hide critical warning/state information; AC-10/11/14/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1019 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H21

H21: Empty/zero-result examples and missing/invalid text, audio or data refs show the right state/diagnostic without a false resource pass; AC-10/11/12/13/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H21 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H21: Empty/zero-result examples and missing/invalid text, audio or data refs show the right state/diagnostic without a false resource pass; AC-10/11/12/13/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1020 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H22

H22: Catalog encoding, exact IDs/placeholders, accented names/font fallback and longer strings remain readable without corrupting text identity; AC-11/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H22 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H22: Catalog encoding, exact IDs/placeholders, accented names/font fallback and longer strings remain readable without corrupting text identity; AC-11/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1021 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H23

H23: Reordered data/definitions retain bindings and quantities; warning layers and four-player identities remain correct across UI state examples; AC-10/11/13/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H23 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H23: Reordered data/definitions retain bindings and quantities; warning layers and four-player identities remain correct across UI state examples; AC-10/11/13/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1022 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H24

H24: Display scaling, formatting and media conversion do not silently change authored units/defaults, quantities, timing or sound metadata; AC-03/11/12/13/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H24 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H24: Display scaling, formatting and media conversion do not silently change authored units/defaults, quantities, timing or sound metadata; AC-03/11/12/13/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1023 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H25

H25: Repeated UI/audio/current-state example restoration does not accumulate ambience loops or replay old transactional/death/warning cues; AC-12/14/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H25 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H25: Repeated UI/audio/current-state example restoration does not accumulate ambience loops or replay old transactional/death/warning cues; AC-12/14/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1024 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H26

H26: Rapid view/audition/state changes leave correct bindings/current loops and visible muted warnings, without requiring later gameplay interactions; AC-10/12/14/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H26 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H26: Rapid view/audition/state changes leave correct bindings/current loops and visible muted warnings, without requiring later gameplay interactions; AC-10/12/14/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1025 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H33

H33 (manual): Re-export/re-import preserves stable selected-output identities without duplicate provenance records; AC-03/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H33 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H33 (manual): Re-export/re-import preserves stable selected-output identities without duplicate provenance records; AC-03/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1026 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H34

H34 (manual): Recomputing or regenerating coverage cannot count one obligation twice or change completeness solely through repetition; AC-01/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H34 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H34 (manual): Recomputing or regenerating coverage cannot count one obligation twice or change completeness solely through repetition; AC-01/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1027 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-NAV.loading

HUB-NAV loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-NAV / loading · Current native preview for the named surface.

**Actions** Navigate to HUB-NAV using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1028 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-NAV.error

HUB-NAV error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-NAV / error · Current native preview for the named surface.

**Actions** Navigate to HUB-NAV using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1029 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-NAV.overflow

HUB-NAV overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-NAV / overflow · Current native preview for the named surface.

**Actions** Navigate to HUB-NAV using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1030 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-NAV.long-text

HUB-NAV long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-NAV / long-text · Current native preview for the named surface.

**Actions** Navigate to HUB-NAV using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1031 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.empty

HUB-REGISTER empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / empty · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1032 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.loading

HUB-REGISTER loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / loading · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1033 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.error

HUB-REGISTER error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / error · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1034 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.populated

HUB-REGISTER populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / populated · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1035 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.partial

HUB-REGISTER partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / partial · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1036 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.overflow

HUB-REGISTER overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / overflow · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1037 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.zero-one-many

HUB-REGISTER zero-one-many backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / zero-one-many · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared zero-one-many state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1038 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.HUB-REGISTER.long-text

HUB-REGISTER long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. HUB-REGISTER / long-text · Current native preview for the named surface.

**Actions** Navigate to HUB-REGISTER using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1039 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.empty

CHARACTER-PREVIEW empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / empty · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1040 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.loading

CHARACTER-PREVIEW loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / loading · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1041 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.error

CHARACTER-PREVIEW error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / error · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1042 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.populated

CHARACTER-PREVIEW populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / populated · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1043 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.partial

CHARACTER-PREVIEW partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / partial · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1044 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.overflow

CHARACTER-PREVIEW overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / overflow · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1045 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.CHARACTER-PREVIEW.long-text

CHARACTER-PREVIEW long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. CHARACTER-PREVIEW / long-text · Current native preview for the named surface.

**Actions** Navigate to CHARACTER-PREVIEW using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1046 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.empty

WORLD-ITEM-PREVIEW empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / empty · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1047 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.loading

WORLD-ITEM-PREVIEW loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / loading · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1048 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.error

WORLD-ITEM-PREVIEW error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / error · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1049 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.populated

WORLD-ITEM-PREVIEW populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / populated · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1050 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.overflow

WORLD-ITEM-PREVIEW overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / overflow · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1051 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.WORLD-ITEM-PREVIEW.long-text

WORLD-ITEM-PREVIEW long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. WORLD-ITEM-PREVIEW / long-text · Current native preview for the named surface.

**Actions** Navigate to WORLD-ITEM-PREVIEW using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1052 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.empty

UI-COLLECTIONS empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / empty · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1053 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.loading

UI-COLLECTIONS loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / loading · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1054 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.error

UI-COLLECTIONS error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / error · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1055 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.populated

UI-COLLECTIONS populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / populated · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1056 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.partial

UI-COLLECTIONS partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / partial · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1057 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.overflow

UI-COLLECTIONS overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / overflow · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1058 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.zero-one-many

UI-COLLECTIONS zero-one-many backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / zero-one-many · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared zero-one-many state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1059 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-COLLECTIONS.long-text

UI-COLLECTIONS long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-COLLECTIONS / long-text · Current native preview for the named surface.

**Actions** Navigate to UI-COLLECTIONS using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1060 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.empty

UI-FORMS-CONTROLS empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / empty · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1061 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.loading

UI-FORMS-CONTROLS loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / loading · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1062 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.error

UI-FORMS-CONTROLS error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / error · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1063 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.partial

UI-FORMS-CONTROLS partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / partial · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1064 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.overflow

UI-FORMS-CONTROLS overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / overflow · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1065 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FORMS-CONTROLS.long-text

UI-FORMS-CONTROLS long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FORMS-CONTROLS / long-text · Current native preview for the named surface.

**Actions** Navigate to UI-FORMS-CONTROLS using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1066 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.empty

UI-TEXT-STATUS empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / empty · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1067 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.loading

UI-TEXT-STATUS loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / loading · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1068 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.error

UI-TEXT-STATUS error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / error · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1069 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.populated

UI-TEXT-STATUS populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / populated · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1070 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.partial

UI-TEXT-STATUS partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / partial · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1071 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.overflow

UI-TEXT-STATUS overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / overflow · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1072 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.zero-one-many

UI-TEXT-STATUS zero-one-many backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / zero-one-many · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared zero-one-many state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1073 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-TEXT-STATUS.long-text

UI-TEXT-STATUS long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-TEXT-STATUS / long-text · Current native preview for the named surface.

**Actions** Navigate to UI-TEXT-STATUS using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1074 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.empty

DATA-TEXT-INSPECTOR empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / empty · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1075 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.loading

DATA-TEXT-INSPECTOR loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / loading · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1076 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.error

DATA-TEXT-INSPECTOR error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / error · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1077 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.populated

DATA-TEXT-INSPECTOR populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / populated · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1078 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.partial

DATA-TEXT-INSPECTOR partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / partial · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1079 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.overflow

DATA-TEXT-INSPECTOR overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / overflow · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1080 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.zero-one-many

DATA-TEXT-INSPECTOR zero-one-many backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / zero-one-many · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared zero-one-many state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1081 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.DATA-TEXT-INSPECTOR.long-text

DATA-TEXT-INSPECTOR long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. DATA-TEXT-INSPECTOR / long-text · Current native preview for the named surface.

**Actions** Navigate to DATA-TEXT-INSPECTOR using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1082 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.empty

AUDIO-AUDITION empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / empty · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1083 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.loading

AUDIO-AUDITION loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / loading · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1084 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.error

AUDIO-AUDITION error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / error · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1085 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.populated

AUDIO-AUDITION populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / populated · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1086 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.partial

AUDIO-AUDITION partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / partial · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1087 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.overflow

AUDIO-AUDITION overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / overflow · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1088 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.zero-one-many

AUDIO-AUDITION zero-one-many backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / zero-one-many · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared zero-one-many state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** How does the layout read at zero, one, and many items (singular vs plural copy, spacing)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1089 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.AUDIO-AUDITION.long-text

AUDIO-AUDITION long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. AUDIO-AUDITION / long-text · Current native preview for the named surface.

**Actions** Navigate to AUDIO-AUDITION using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1090 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.empty

UI-FIXTURE-PREVIEW empty backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / empty · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared empty state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when there is no data — zero items, an unfilled form, or absent media?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1091 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.loading

UI-FIXTURE-PREVIEW loading backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / loading · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared loading state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown while data or content is still loading (skeleton, spinner, progressive reveal)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1092 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.error

UI-FIXTURE-PREVIEW error backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / error · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared error state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown when the load or submit fails (message, retry affordance, partial fallback)?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1093 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.populated

UI-FIXTURE-PREVIEW populated backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / populated · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared populated state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What does the normal populated (happy-path) state look like at a typical volume of content?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1094 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.partial

UI-FIXTURE-PREVIEW partial backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / partial · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared partial state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What is shown for partial or incomplete data — some fields or rows present, others missing?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1095 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.overflow

UI-FIXTURE-PREVIEW overflow backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / overflow · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared overflow state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens when content exceeds its container — scroll, clip, wrap, or truncate?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1096 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### UIH.UI-FIXTURE-PREVIEW.long-text

UI-FIXTURE-PREVIEW long-text backstop. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. UI-FIXTURE-PREVIEW / long-text · Current native preview for the named surface.

**Actions** Navigate to UI-FIXTURE-PREVIEW using physical keyboard or mouse → Exercise its declared long-text state → Capture the exact state and its independent readability or diagnostic observation.

**Expected** What happens with unusually long text — truncation, wrapping, ellipsis, or reflow?

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1097 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### rendered.hub-navigation

Current hub controls and exact resource failures. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. Characters, items, buildings/world, UI, data/text and audio category names · Current resource IDs and visible error cases.

**Actions** Exercise the input-driven category nav, explicit resource selection and compact/expanded metadata → Capture the corresponding selected resource and its exact visible diagnostic.

**Expected** Every selected category and exact resource failure remains reachable through current native controls.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1098 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### rendered.audio-audition

Current audio controls and listening handoff. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. Exact selected AU category, sample, source version, loops and mix · Named listener and retained stream/version reference.

**Actions** Navigate to Audio with physical input and exercise ordinary auditions and restoration → Capture current native selection, input and visible controls → Have the named listener record the exact WAV/version and heard verdict.

**Expected** The exact selected cue, loop and mix inputs remain audible and reviewable with independent named file/version verdicts.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G1-hub / 1099 / inspection-hub. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H27

H27: Similar/conflicting resource paths are diagnosed accurately; one bad entry does not conceal independent valid entries; AC-15/16/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H27 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H27: Similar/conflicting resource paths are diagnosed accurately; one bad entry does not conceal independent valid entries; AC-15/16/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1000 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H28

H28: An empty or partially missing pack/gallery cannot obtain a successful content result through zero discovery or fallback art; AC-16/17/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H28 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H28: An empty or partially missing pack/gallery cannot obtain a successful content result through zero discovery or fallback art; AC-16/17/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1001 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H29

H29: Packaged path/ID spelling and readable exact diagnostics do not depend on development-path aliases or encoding loss; AC-15/16/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H29 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H29: Packaged path/ID spelling and readable exact diagnostics do not depend on development-path aliases or encoding loss; AC-15/16/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1002 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H30

H30: Validation/discovery order cannot suppress known resource failures or prevent inspection of unaffected entries; AC-16/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H30 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H30: Validation/discovery order cannot suppress known resource failures or prevent inspection of unaffected entries; AC-16/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1003 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H31

H31: Repeated clean launch/close/relaunch uses the same packaged content and does not require hidden donor/editor caches; AC-15/17/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H31 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H31: Repeated clean launch/close/relaunch uses the same packaged content and does not require hidden donor/editor caches; AC-15/17/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1004 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### heldout.H32

H32: Partial imports, interrupted checks or failed launch attempts cannot inherit an earlier successful result; unaffected content remains inspectable where the build can run; AC-16/17/19. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. An independently selected case-specific input outside the happy-path presets · Exact observed combination and current file/resource IDs.

**Actions** Declare the independent H32 stimulus and expected checkpoints in the selected run → Apply the exact case stimulus and exercise the relevant authored native control → Capture the individual observed outcome against this held-out intent.

**Expected** H32: Partial imports, interrupted checks or failed launch attempts cannot inherit an earlier successful result; unaffected content remains inspectable where the build can run; AC-16/17/19.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1005 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.

### rendered.packaged-relaunch

Actual Windows package relaunch and clean exit. High. Missing or misleading resource evidence prevents completion of the corresponding required scenario.

**Setup and input** Complete current Phase 4 implementation and independent review. Use the saved Windows 1280×720 selection on the reviewed build. Exact current export identity and bounded run-owned package location · Donor-free and editor-cache-free environment.

**Actions** Launch and inspect current packaged content → Close through the ordinary window control → Launch the same package again, inspect current resources and close.

**Expected** Both ordinary launches run the exact current exported product with reachable resources, exit normally and leave no children.

**Evidence** Invocation-bound input acknowledgement · Read-only native post-draw observation · Decoded 1280×720 PNG with same observation identity · Individual case outcome with expected and observed details.

**Run and reset** G2-packaged / 1006 / packaged-inspection. 12 seconds per input or observation; 15 seconds to quit normally; compatible group uses the exact predeclared bound. Return through the ordinary navigation or transport controls; restore the authored baseline before the next independent scenario. Release held input, request normal close, and require exited owned game and helper processes.
