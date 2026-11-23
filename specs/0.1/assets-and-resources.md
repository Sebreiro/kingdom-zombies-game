# KingdomZombies 0.1 — Asset and resource production plan

**Date:** 2026-11-13.

**Scope authority:** [initial-spec.md](initial-spec.md). Confirmed features and exclusions in that document take precedence. Its proposed content roster and balance are the working production baseline here.

**Purpose:** List the art, animation, sound, interface, authored data, and supporting resources needed for a finished 0.1; identify concrete Kingdom Two Crowns donors; generate or assemble the remaining content with code/Codex. This is a production inventory, not a change to gameplay or a choice of game engine.

**Deliverable status:** Specification and resource inspection complete. The asset packs described here have not been produced or integrated. Inspection decoded 35 representative sprites into temporary previews, inspected 19 animation clips, checked eight audio stream references and four embedded fonts, and searched all 17 exported object inventories. Catalog-only candidates still need extraction and review. No original resources were changed.

## 1. How to use this document

Start with the recovered assets, preserve their useful pixels, animation timing, sound recordings, and palette information, and create only the missing parts. A final resource may combine several donors with generated layers. A reused image still needs a usable export, metadata, and integration.

| Route | Meaning |
| --- | --- |
| **Reuse** | Extract an existing resource; normalize its palette, naming, layout, or portable metadata as needed. No substantial new drawing is expected. |
| **Adapt** | Keep useful donor content, then add or change required parts: equipment layers, metal reinforcement, silhouettes, UI layouts, or event bindings. |
| **Generate** | Create the missing resource with code/Codex. Use the reference palette and reusable components wherever possible. |
| **Reference only** | Useful for timing, construction, materials, or style, but does not satisfy the final asset requirement. |

Evidence labels describe what was actually checked, independently of the production route:

- **V — visual sample:** decoded the named sprite and inspected it. This does not certify every frame in its family.
- **M — decoded metadata:** inspected animation timing/events, embedded font data, or audio metadata and a valid backing-stream range. Audio was not auditioned.
- **C — catalog:** verified the object name, type, owning file, and object identifier in the recovered inventory. Appearance, sound, and full export remain unverified.

Requirement IDs such as `CH-01`, `IT-01`, and `UI-01` identify output obligations. Donor IDs `K01`–`K30` point to section 3. Multiple instances, colors, directions, health states, or tiers do not automatically require independent full illustrations; shared layers and generated variations are preferred.

### 1.1 Fixed content counts

| Content | 0.1 coverage |
| --- | --- |
| World | One handcrafted map; six portals, three on each side; one reusable portal art family is sufficient |
| Survivors | One shared survivor presentation system for players and NPCs; four assigned jobs plus unassigned; eight initial survivors and up to 100 total under the baseline |
| Zombies | Seven recognizable archetypes; all applicable clothing, armor, weapons, and bags must work on each |
| Wildlife | Rabbit and deer, including persistent carcasses |
| Main buildings | Three appearances; tier 3 includes two attached, one-archer towers |
| Defenses | Three wall tiers and three standalone tower tiers; standalone towers show 1 / 2 / 3 archers |
| Authored sites | Baseline: eight wall sites, eight standalone tower sites, six tree-clearing groups, six food plants, six wildlife zones, and two moving rally positions |
| Items | All 24 definitions in specification §5.3; one icon each, plus worn/held representations where applicable |
| Equipment interface | Eleven slots: head, face, torso, legs, body armor, gloves, boots, backpack, belt, left hand, right hand; four food quick-slot references |
| Multiplayer identification | Four player markers distinguishable by number/name as well as color |
| Presentation | Windows/Linux; keyboard/mouse; English catalog; readable at 1280×720 and 1920×1080 and the specified UI scales |

Do not create production assets for extra islands, external recruitment camps, mounts, firearms, shields, crafting, raw resources, music, tutorials, controller prompts, additional locales, Steam/Epic screens, or gameplay acid hazards. Their presence in the recovered game does not add them to 0.1.

## 2. Four-pass coverage review

The requirement inventory was reviewed in four passes. The recovered-resource matching and sample checks then inform the reuse routes recorded throughout this document.

| Pass | Review focus | Obligations surfaced and retained |
| --- | --- | --- |
| 1 — Content census | Specification §§1–9 and §13.3: every actor, item, structure, and map object | Seven zombies, 24 item icons, two wildlife species, all nine building/tier appearances, main-building attachments, marked sites, food sources, crown, corpses, stash, and rally points |
| 2 — Actions and states | Follow each object through movement, fighting, work, damage, death, looting, construction, and restoration | Two-hand poses, bow/arrow-hand states, jumping/landing, seated fireside idle, equipment on corpse poses, depleted plants, tree marking/felling, unfinished frames, upgrade overlays, previous-tier ruins, empty loot, permanent portal ruins, cosmetic-only acid |
| 3 — Complete player experience | Specification §§10–13 plus outcome, configuration, save, and failure paths | LAN discovery/manual entry, reconnect/spectator feedback, shared-operation failures, legacy inventory extraction, save failures, remappable prompts, all English messages, fonts, all sound categories, documented configurations, platform launch resources |
| 4 — Integration and omission check | Specification §14 acceptance scenarios and §16 proposals; verify cross-system asset combinations | Eight humanoid appearance sets with compatible gear, correct visible guard counts, persistent corpses after visual effects expire, UI warnings over open panels, shared minimap discovery, no crown loot, no tower corpses, no recovered-arrow art implying pickup, saved visual states and stress-scene resource coverage |

Section 12 maps the result back to the specification and its acceptance scenarios. These passes check completeness of this inventory; they are not claims that the game or all assets already pass acceptance.

## 3. Recovered Kingdom Two Crowns resource register

### 3.1 Where the usable material lives

Reference root: `D:/Work/Games/KindomCrown/recovered`.

| Locator | Absolute location / meaning |
| --- | --- |
| Resource data root | `D:/Work/Games/KindomCrown/recovered/resources/KingdomTwoCrowns_Data` |
| `R:<id>` | Object `<id>` in `resources.assets` under the resource data root |
| `S0:<id>` | Object `<id>` in `sharedassets0.assets` |
| `S2:<id>` | Object `<id>` in `sharedassets2.assets` |
| `G:<id>` | Object `<id>` in `globalgamemanagers.assets` |
| Object lookup | `D:/Work/Games/KindomCrown/recovered/resources/inventory/<owning-file>.jsonl` |
| Load-path lookup | `D:/Work/Games/KindomCrown/recovered/resources/resources-load-catalog.json` |
| Decoded authored values | `D:/Work/Games/KindomCrown/recovered/resources/gameplay-configs/<owning-file>.jsonl` |
| File identities | `D:/Work/Games/KindomCrown/recovered/resources/manifest.json` |
| Recovered source | `D:/Work/Games/KindomCrown/recovered/source/Assembly-CSharp` |

For example, `R:9758` means the **Sprite** named `wall_2` inside `D:/Work/Games/KindomCrown/recovered/resources/KingdomTwoCrowns_Data/resources.assets`. It is not a loose file named `wall_2.png`. Verify the type and name as well as the ID when extracting. IDs are local to their owning file; names can also repeat within a file.

The inventories contain 206,000 objects: 32,839 Sprites, 508 Texture2D objects, 2,347 AnimationClips, 271 AnimatorControllers, 325 AnimatorOverrideControllers, 3,341 AudioClips, and 17 Fonts, among other types. These totals include unrelated content, alternate biomes, duplicate names, and non-0.1 material; they are not counts of ready-to-use game assets.

Keep `.assets` files with their referenced `.resS` texture streams and `.resource` sound streams. The preservation manifest lists 32 files totaling 2,416,328,557 bytes. Copying whole archives into the finished game is unnecessary; export only selected dependencies.

### 3.2 Art, animation, font, and presentation donors

`Family` names below are search selectors. Only the stated sample IDs are exact object selections; enumerate, disambiguate, and verify all remaining family members during export. Clip numbers refer to AnimationClip objects, not Sprite frames.

| Donor | Exact anchors and family to inspect | Evidence | Planned reuse and remaining work |
| --- | --- | --- | --- |
| K01 — Woodland trees | `R:10674` `trees_trunk_02`; `R:9896` `trees_leaves_02_0`; `R:10716` `trees_stump_01`; families `trees_bg_trunk*`, `trees_bg_leaves*`, `trees_trunk*`, `trees_leaves*`, `trees_stump*` | V trunk/leaves; C remainder | Reuse tree pieces and parallax forest. Reassemble pivots and foliage placement; add the actual map's clearing-group boundaries. Prefer the ordinary oak/birch family for a coherent base set. |
| K02 — Ground and distant land | `R:7982` `backdrop_hills`; `R:11002` `cliff_stones_ground_EXTENDED`; `R:11491` `grass_0`; `R:8949` `ground_object_rocks_01`; families `hills_*`, `ground_object_moss*`, `ground_object_roots*` | V hills; C remainder | Reuse scenery, terrain textures, rocks, roots, and grass. Author the new continuous walkable map and its collision separately. `grass_0` has duplicate names: retain the selected source ID. |
| K03 — Sky | `R:8604` `clouds_01`; `R:10266` `moon_new_recoloured`; cloud/highlight/stratus families; Texture2D `R:411` `star` | V cloud; C remainder | Reuse cloud shapes and celestial details; generate day/night sky colors, placement, transitions, and light settings. Seasons/weather are not required. |
| K04 — Berries | `R:9480` `berrybush`; `R:10311` `berrybush_berries_01`; `berrybush_berries_01` through `_04`; `R:9893` `berrybush_basket` | V bush/one berry layer; C basket/other layers | Reuse base vegetation and detachable berry layers. Harvested/empty/refilled appearances come from layer visibility. Crop/recompose a readable berry item icon from the fruit layer. |
| K05 — Fruit trees | `R:11222` `athena_apple_orchard_back`; `R:9204` `athena_apple_orchard_apples_back`; `R:9468` `athena_apple_orchard_front`; `R:8203` `athena_apple_orchard_apples_front` | V back/apples-back; C front layers | Adapt ordinary foliage and fruit into the two food plants. Remove puzzle/giant-golden-apple associations. The sampled back is 290×104 pixels and depicts several trees; crop/reassemble deliberately instead of treating it as one small plant. |
| K06 — Campfire | `R:7977` `campfire_01`; `R:9734` `campfire`; `R:8356` `fire_mid_burn_0`; `R:10329` `fire_small_burn_0`; eight-frame burn families | V fire bases; C burn frames | Reuse stone/log base and flame animation; compose lighting, seating, and stash boxes for the tier-1 camp. |
| K07 — Chest | `S0:8280` `chest_open_0`; `S0:7551` `chest_empty_1`; clips `R:2451` `chest_close`, `R:2455` `chest_empty`, `R:2459` `chest_open` | V first open frame; C other objects | Adapt into the stash chest/box kit, with closed/open feedback. The chest's appearance must not imply the shared stash has one small fixed capacity. Generate extra rough boxes if needed. |
| K08 — Wooden walls and wreck pieces | `R:8144` `wall_1`; `R:9758` `wall_2`; `R:8847` `wall_0`; `R:8053` `wall_wreck_1_s028_040`; `wall_wreck_1_*`, `wall_wreck_2_*` | V wall 1/2; C marker/fragments | Use `wall_1` as the tier-1 donor and `wall_2` as reinforced-wood tier-2 donor; supplement braces if needed for readability. Reuse fragment approach for damage and ruins. Generate metal layer and matching fragments for tier 3. |
| K09 — Wooden tower pieces | `R:8279` `tower_1a`; `R:7865` `tower_1b`; `R:8711` `tower_2`; `R:8027` `tower_0a`; `R:7808` `tower_0b` | V `tower_1a`, `tower_2`; C remainder | Reuse the wooden platform/supports, particularly the complete wooden `tower_2` sample. Compose this game's three tiers with one/two/three archer positions. Original tier numbers do not equal the new tiers. |
| K10 — Scaffolding | `R:9956` `scaffolds_01`; `R:10804` `scaffolds_fill_01`; families `scaffolds_01`–`_05`, `scaffolds_fill_01`–`_05` | V scaffold 01; C remainder | Reuse/resize modular construction pieces around every required footprint; create metal-specific frame details as necessary. Separate overlays from the old functional tier during upgrades. |
| K11 — Portal | `S0:7865` `portal_back`; `S0:8291` `portal_front`; fragment families `portal_back_s*`, `portal_front_s*`; clips `R:2568` close, `R:2569` closedidle, `R:2570` destroy, `R:2571` idle, `R:2572` open, `R:2573` permanent_destroy | V front/back; M six clips; C fragments | Reuse stone framing, debris, and animation timing. Reconstruct the interior effect, damage composition, warnings, and permanent ruins. Strip reference-game event callbacks and regeneration behavior. One art family serves all six portals. |
| K12 — Deer | `R:8557` `deer_walk_0`; `R:7981` `deer_die_0`; families `deer_walk_*`, `deer_run_*`, `deer_eat_*`, `deer_look_*`, `deer_die_*`; clips `R:2278`, `R:2282`, `R:2286`, `R:2290`, `R:2294`, `R:2298` | V two frames; M die/run/walk clips; C remaining frames/clips | Reuse the deer animation family; recover its palette/material recoloring, add hit feedback, and retain the final carcass pose until loot cleanup permits removal. The sampled death frame is the start of dying, not a finished corpse. |
| K13 — Rabbit | `R:9243` `bunny_stand`; `R:9577` `bunny_dead`; `R:10160` `bunny_walk_0`; clips `R:2193` dead, `R:2197` stand, `R:2201` walk | V stand/dead; M clips; C walk frames | Reuse rabbit poses and movement; tune playback for flee behavior, add hit/death transition if necessary, and keep the corpse interaction. |
| K14 — Human movement | `S0:2361` `peasant_idle_0`; `peasant_idle*`, `peasant_walk*`, `peasant_run*`; clips `R:1204` idle, `R:1207` run, `R:1214` walk | V one body frame; M idle/walk clips; C remainder | Adapt motion and proportions as a survivor template. The sampled frame already contains clothing. Extract/redraw a neutral base and independent equipment layers; this is not a ready-made modular character kit. |
| K15 — Archery | `R:6011` `archer_idle_1`; `R:6056` `archer_walk_0`; `R:6095` `archer_shoot_4`; clips `R:777` idle, `R:780` run, `R:783` shoot, `R:786` prep, `R:804` walk | M idle/shoot/walk clips; C sprites/other clips | Reuse timing and pose references; adapt ordinary survivor/zombie bow poses to visible equipment and mouse aim. Structural tower archers can reuse a simpler composed version of this family. |
| K16 — Worker actions | `S0:4214` `worker_hammer_0`; `S0:4399` `worker_chop_2`; `S0:7986` `worker_work_0`; `S0:6853` `tools_hammer`; clip `R:1245` `worker_hammer` | M hammer clip; C pixels | Reuse hammer/chop motion and tool artwork as work-action props. Rebuild body/equipment layering. These props do not create purchasable hammer/axe items or new tool requirements. |
| K17 — Gathering | `S0:3879` `farmer_forage_work_0`; `S0:2939` `farmer_forage_work_loop_3`; clips `R:903`, `R:906`, `R:912`, `R:915`, `R:918` for forage idle/putaway/walk/work/loop | C | Adapt reach/harvest/put-away poses and any suitable basket detail. Do not import the farming/scythe mechanic or create infinite storage. |
| K18 — Eating and dying | `S0:7060` `beggar_eat_0`; `S0:7246` `beggar_die_0`; clips `R:823` eat and `R:819` die | M death clip; C eating/pixels | Motion reference for food use and ordinary survivor death. Persistent equipped corpse poses and fireside seated idle still need work. Do not import homeless recruitment. |
| K19 — Enemy motion references | `S0:6817` `greed_peasant_idle_0`; `S0:8098` `troll_idle_0`; `R:6066` `greed_knight_idle_0`; `R:6402` `greed_archer_death_0`; associated greed/troll/greed_archer families | V three idle samples; C archer family | Reference only for motion, collapse, and large-body effects. These are fantasy Greed designs, not the required zombies; generate the seven zombie silhouettes and compatible clothing layers. Do not substitute them unchanged. |
| K20 — Money, bow, arrow | `S0:7856` `coin_spin_0` and coin-spin family; `S0:7008` `tools_bow`; `S0:6885` `arrow`; clip `R:2463` coin_spin | V bow/arrow; C coin/clip | Reuse projectile and bow pixels; adapt money, bow, and arrow-bundle inventory icons. Coin money is a proposed presentation choice consistent with physical currency. No coin-purse UI or automatic treasury is inherited. |
| K21 — Crown | `S0:7985` `crown`; `S0:6994` `crown_indicator`; `S0:7295` `crown_glow` | V crown; C extra indicators | Reuse crown shape; generate four number/name/color marker combinations. Crown stays above normal headgear and never becomes a loot item. |
| K22 — Rally and site markers | `R:10173` `banner_1`; `S0:18126` `border_banner_wave_0`; `S0:7705` `trees_mark_01`; `R:8376` `border_portal_icon` | C | Reuse banner motion, marking, and sign components; generate new left/right rally symbols, prices, target labels, and locked/paid state overlays. |
| K23 — Effect textures | Texture2D `R:217` `fire_mid_particle`, `R:292` `fire_small_particle`, `R:269` `sparks`, `R:453` `smoke_small`, `S0:134` `goo_particle`; Sprite `S0:6967` `splash_mid_0` and splash family | C | Reuse texture ingredients for embers, dust, impacts, spit, and cosmetic acid; generate effect definitions. Original particles/materials are not portable finished effects. No damaging fire or lingering acid system is implied. |
| K24 — UI pieces | `S0:7837` wood big normal, `S0:7900` highlight, `S0:6925` pressed; `S0:8173` wood normal, `S0:8269` highlight, `S0:7867` pressed, `S0:8151` disabled | V big normal; C remaining states | Reuse button borders/textures and derive stretchable panel pieces. Generate the new inventory, HUD, people, sale, save, and LAN layouts. Retain pixel clarity at supported UI scales. |
| K25 — Minimap symbols | `S0:6949` `map_icon_portal_active`; `S0:7692` `map_icon_portal_destroyed`; `S0:7907` `map_campfire`; `S0:7540` `map_pointer_0` | V portal pair; C camp/pointer | Reuse/adapt icons. Generate one-dimensional world-map layout, explored mask, defense icons, and four teammate markers. Original island-map backgrounds are not this game's map. |
| K26 — Cursor and input art | Texture2D `G:3` `ui_cursor`; Sprite `S0:7592` `input_glyph_keyboard_sides_0`; keyboard glyph families | C | Reuse suitable cursor/keycap framing; generate missing keyboard/mouse symbols and live binding labels. Do not ship fixed A/D artwork as the only prompt after rebinding. |
| K27 — Fonts | Font `R:6003` `PerfectDOSVGA437`; `R:6004` `LiberationSans`; `S0:558` `Kingdom`; `S0:564` `KingdomMenu` | M | All four contain embedded font bytes: 81,192 / 350,200 / 25,652 / 18,336 respectively. Extract and compare readability. Proposed body font: LiberationSans; pixel/accent font: PerfectDOSVGA437. Reuse available fonts rather than drawing text into UI images. |
| K28 — Save indicator | Loose file `resources/KingdomTwoCrowns_Data/StreamingAssets/SaveIcon.png`, relative to the reference root | C, file exists | Inspect and reuse if suitable as a small save/status indicator; otherwise generate a plain one. This is a loose image, unlike the packed sprite entries. |
| K29 — Palette/composition data | Decoded `BiomeData` `R:73492` `OakAndBirch`; `BiomeSpecificAssets` `R:73486` `OakAndBirch`; `resources/building-upgrades.json`; `resources/gameplay-configs/*.jsonl` | M exported fields | Reuse selected palette values, placement relationships, and assembly references as inputs. Author this game's map, tiers, item data, and balance anew from the specification. |
| K30 — Early castle layers | `R:10583` `castle_02_front`; `R:8704` `castle_02_back`; `R:9819` `castle_02_back_left`; `R:8043` `castle_02_back_right` | V front; C back pieces | Small construction/ground details may be reused. The sampled front is only 97×8 pixels, not a shelter facade. Generate the required main tier 2/3 silhouettes using suitable wood components and supplied style references. |

### 3.3 Findings that prevent incorrect reuse

- **Material mismatch:** visual samples `R:10797` `wall_3` and `R:10608` `tower_3` are stone. This game's third tier is metal-reinforced. Use wooden donors plus generated metal, braces, damage, and ruin pieces.
- **Palette mismatch:** sampled deer pixels are grayscale. `BiomeData` contains deer color gradients, and `SkinRecolor.cs` / `SpriteRendererFX.cs` apply recoloring. Exported pixels alone can look different from the original game. Recover or deliberately replace palette mappings.
- **Layer mismatch:** the peasant sample has baked clothing, and portal/building sprites are separate layers. Neither a character frame nor one facade layer proves a complete modular character/building is available.
- **Animation mismatch:** inspected clips have timing and event data, but a complete portable animation still needs its bindings, referenced frames, transforms, loop rules, and events resolved. Lexically sorting numbered PNGs is insufficient when poses repeat or clip timing differs.
- **Gameplay callbacks:** `portal_permanent_destroy` includes `SpitOutHermits` and `SpitOutGems` events. Reuse the visual sequence only; neither event belongs in 0.1. The hammer clip includes `OnAnimHammerHit`, which must map to this game's paid work feedback and progress rules.
- **Death mismatch:** reference-game fade/disappearance cannot delete 0.1 nonempty corpses. Keep a persistent final pose and loot target even after transient particles fade.
- **No unsupported inventory claim:** the recovered `inventory/*.jsonl` files are asset catalogs. They are not a Tarkov-style gameplay inventory system or its UI art.

### 3.4 Source-code observations used in the production plan

All paths in this table are relative to `D:/Work/Games/KindomCrown/recovered/source/Assembly-CSharp`.

| Inspected source | Resource implication | Required difference for KingdomZombies |
| --- | --- | --- |
| `Wall.cs`, `DebrisGroup.cs` | Wall sprites, wreck pieces, crumble ordering, hit flashes, and rebuild presentation form a set | Paid repairs and three wood/wood/metal tiers; use the specified health/work values |
| `ConstructionBuildingComponent.cs`, `WorkableBuilding.cs` | Scaffolding plus start/work/complete sounds and lighting changes | Old completed tier stays functional during an upgrade; frames, worker limits, and paid progress follow the new spec |
| `GuardSlot.cs`, `Tower.cs` | Guard positions and even substitute guard renderers are separate visual dependencies | Guards are structural 1/2/3 units, no recruited-archer transfer, inventory, successor, or corpse |
| `Archer.cs`, `Peasant.cs` | Idle/move/preparation/attack/flee and equipment-related visuals are state driven | On-foot leaders, physical arrows, both hands, arbitrary permitted gear, and persistent corpses need new bindings |
| `Deer.cs` | Eating, movement, death, recoloring, and fade logic accompany sprite frames | Reuse presentation, replace fade/removal with real wildlife-loot lifetime |
| `WorkableTree.cs` | Trunk, moss, chop marks, leaves, falling direction, and chop sound are separate resources | Clearing provides no wood or money; do not carry over `coinYield` or coin drops |
| `Portal.cs` | Open/close/attack/destroy triggers, crumbling parts, and loop audio | No portal rebuilding, teleportation service, hermit/gem rewards, or final-island behavior |
| `SkinRecolor.cs`, `SpriteRendererFX.cs`, `BiomeSpriteSwapper.cs` | Recolor channels and biome swaps explain why raw sprites can be incomplete visually | One coherent apocalypse palette; no need to ship every biome |
| `AnimationEventBroadcaster.cs`, `AnimationTriggeredVFX.cs`, `AudioAnimationEvents.cs` | Animation events may trigger sound and particles not present in the atlas | Export explicit event metadata; map only approved effects to current gameplay |
| `TimeBasedAudio.cs` | Ambient start/end times, loops, random offsets, and fades are separate from recordings | Day/night ambience only; no music, mandatory seasons, or weather expansion |

These are static source/resource observations. The original game was not launched for this analysis. Reusing a script's concepts or resource references does not mean its original engine-specific component can be copied unchanged.

### 3.5 Repeatable extraction and adaptation procedure

1. Resolve each selected locator against the owning JSONL inventory and preservation manifest. Record name, type, ID, and source-file identity before extraction; select one biome/variant deliberately when names repeat.
2. Decode sprites through their actual sprite/atlas references, including packed rotation, crop, transparency, and external texture streams. Export native RGBA pixels plus the original logical bounds, trim offset, pivot, and source scale. Do not cut arbitrary rectangles out of atlas previews.
3. Resolve selected clips to their actual frames and animated layer/transform bindings. Preserve repeated poses and timing, reconstruct loop behavior, and replace original gameplay callbacks with the approved event list. Export only required controllers/clip metadata as reference inputs, not the entire original actor behavior.
4. Recover color/material information where the image depends on it. Produce portable palette/mask/composition settings or bake a deliberate final palette. Retain the raw donor export so later changes do not require extracting again.
5. Export complete chosen audio payloads from their referenced stream ranges, decode them to a usable format, audition, trim, and define loops/gain. Extract embedded font bytes and verify characters, metrics, and sizes in the actual UI. Metadata presence alone does not finish either step.
6. Compose/adapt the selected pieces into the required output IDs, generating missing components, layers, poses, layouts, and configuration. Preserve source attribution/provenance metadata alongside generated recipes.
7. Validate transparency, registration, scaling, loop boundaries, attachments, source/output identity, and all resource references before adding the output to the integrated content pack.

An existing local decoding example is available at `D:/Work/Games/KindomCrown/tools/export_sprite_samples.py`; it was inspected and uses the recovered resource files with UnityPy 1.23.0. Its hardcoded ten-sample selection and output destination must be adapted for the production manifest. The new game's runtime does not need that extraction tool or the original archives.

## 4. Shared asset production contract

### 4.1 Sprite scale, layers, and metadata

Preserve original pixels during extraction. The checked sprites use 32 source pixels per original world unit, but the specification's `u` is a new design unit and must not silently inherit that scale. Sample dimensions: human frame 32×32, rabbit 16×16, deer walk 35×31, wooden wall 32×43, wooden tower 44×48, crown 6×3, bow 16×18, and arrow 8×4.

Choose a shared logical pixel density using an assembled survivor, zombie, wall, tower, and base test scene before producing the remaining pack. Normalize the large supplied example images to that density; do not make one asset's file resolution determine its in-world size. Keep source resolution, export resolution, and world display dimensions distinct.

Every exported/generated sprite family needs:

- Stable output ID, source donor(s) or generation recipe, content version, and output path.
- Transparent image data, frame rectangles, untrimmed logical canvas, trim offset, pivot/ground line, and intended draw order.
- Loop/non-loop status, duration or frame timing, animation events, and default/fallback pose.
- Hand, arrow-release, head/crown, backpack, and other relevant attachment anchors, including mirrored orientation rules.
- Recolor masks/palettes where used; correct opacity and readable night treatment.
- Separate gameplay shapes/interaction anchors where required. Do not derive a portal barrier, wall blocker, or melee reach blindly from transparent image bounds.

Prefer consistent logical canvases with per-frame offsets, nearest-neighbor scaling, and shared reusable layers. Do not smooth pixel art during extraction. Avoid producing a separate complete atlas for every combination of clothing, bags, armor, and weapon pairs.

### 4.2 Equipment layering and fitting

Provide a neutral body and independent wearable/held layers or equivalent reusable composition data. Equipment must fit the survivor and all seven zombie archetypes. Bodies may share a fitting template, but every supported combination must still work on each silhouette, including the wide brute and bloater.

Layer order must handle rear hand, body/legs, boots, torso, armor, belt, backpack, face/headwear, front hand, and crown without hiding required gear information. Arms may need front/back segments for two-handed drawing. Normal clothing can be covered by armor; unequipping the armor must reveal the correct clothes.

Mirror facing with attachment metadata. Do not swap logical left/right inventory slots when a character turns. Dual weapons need both grips and swing paths visible. A bow needs a nocked-arrow representation only while ammunition is actually present; the opposite-hand stack cannot become an unlimited quiver. Stowed items and bags inside bags do not appear as extra worn items.

Corpse poses use the same remaining-equipment state. Looting a visible weapon, helmet, coat, or backpack updates the corpse composition. The crown is removed at death/control loss. Broken gear remains recognizable; a generated broken/condition overlay is sufficient unless a separate worn state improves clarity.

### 4.3 Code-generated and shared outputs

Generate rectangles, grid lines, clipping masks, progress/health/stamina bars, text labels, stack counts, durability overlays, selection outlines, player numbers, map masks, and basic particles from definitions. Generate item thumbnails from the same master item artwork where legible. Reuse one generic ground-loot container and its interaction UI rather than demanding 24 separate drop animations.

Save generation recipes, source selections, palettes, seeds where applicable, and composition parameters so assets can be reproduced. Generated source images/audio are ordinary production outputs, not hidden hardcoded balance settings. Game-relevant measurements remain in the documented configuration.

## 5. Characters and animation

### 5.1 Required character families

| ID | Required output | Reuse route | Completion requirement |
| --- | --- | --- | --- |
| CH-01 | Shared survivor body, neutral uncovered areas, appearance palette | Adapt K14/K18 and the supplied zombie's general pixel treatment | Works for players, unassigned workers, gatherers, hunters, melee warriors, and warrior archers; roles do not require separate incompatible bodies |
| CH-02 | Shambler | Generate from local zombie reference; use K19 only as motion reference | Readable slow, hunched undead silhouette, with all applicable equipment |
| CH-03 | Runner | Adapt shared zombie layers; generate distinct lean/forward silhouette and faster movement poses | Recognizable without depending on one guaranteed weapon |
| CH-04 | Brute | Generate broad, heavy silhouette and fitting data | Wider body/arms, weighty movement/attack/death; ordinary item appearances fit the body |
| CH-05 | Armored zombie | Adapt zombie base; generate distinct body/posture and armor fit | Guaranteed armor uses the actual armor item layer; removing/breaking it does not reveal a different fictional item |
| CH-06 | Spitter | Generate distinctive head/neck/posture, spit wind-up and release | Direct-hit spit projectile originates from the mouth; no lingering hazard |
| CH-07 | Zombie archer | Adapt zombie layers and K15 pose timing | Readable archer silhouette/posture; actual bow and arrow-hand state, with melee/fist fallback |
| CH-08 | Acid bloater | Generate wide fat silhouette, fitting data, death splash | Splash/stains are cosmetic and do not replace or consume the corpse inventory |
| CH-09 | Structural tower archer | Adapt K15; reuse K20 bow/arrow | Idle/aim/shoot left and right, drawn independently at each guard anchor; no inventory, loot, crown, or separate persistent death body |
| CH-10 | Rabbit | Reuse K13 | Idle, move/flee, hit/death transition, final carcass, loot highlight |
| CH-11 | Deer | Reuse/adapt K12 | Idle/look/eat, walk/run/flee, hit/death, final carcass, recolored appearance, loot highlight |
| CH-12 | Leadership/player identifiers | Reuse K21 + generate labels | Four identities, crown with normal headgear, control-transfer appearance, teammate/minimap version, readable without color alone |
| CH-13 | Persistent ordinary corpses and loot piles | Adapt each character's death endpoint; generate generic pile/interaction marker | Survivor + all seven zombies + both wildlife; no corpse for tower guards; empty/nonempty/selected states remain readable in a crowded scene |

Additional cosmetic human faces, gender variants, or multiple skins per archetype are optional polish, not minimum content. Seven distinct zombie silhouettes remain mandatory even when fitting templates or motion frames are shared.

### 5.2 Action/state matrix

Each cell specifies required presentation, not a requirement to draw an entirely separate sheet. Hit flashes, pose holds, timing changes, shared frames, and procedural attachment motion may satisfy a state if the result is clear.

| ID | State/action | Required on | Reuse / missing work |
| --- | --- | --- | --- |
| AN-01 | Neutral idle, combat-ready idle, turning left/right | CH-01–CH-08 | Adapt K14/K15/K19; add all equipment attachments |
| AN-02 | Walk and faster movement | CH-01–CH-08 where used | Survivor sprint and civilian retreat; zombies use their configured locomotion, including the runner. Speed-adjust shared cycles only when readable |
| AN-03 | Jump takeoff, airborne up/down, landing | CH-01 under player control | Generate missing poses and item/bag movement; no mandatory zombie/NPC jumping mechanic |
| AN-04 | Empty-hand/fist strike | CH-01–CH-08 | Generate/retarget unarmed motion; match facing and hit timing; support a hand made free after fallback |
| AN-05 | One-handed melee | CH-01–CH-08 | Knife thrust/slash, club/bat swing, machete slash; either logical hand; adapt motion but retain actual weapon size and grip |
| AN-06 | Two one-handed weapons together | CH-01–CH-08 when equipped | Composite both swings; mixed pairs and slower-weapon interval; no second attack control or shield animation |
| AN-07 | Bow preparation, mouse/AI aim, release, recovery | CH-01–CH-09 | Adapt K15/K20; support the game's actual aiming range with attachment/pose data rather than limiting gameplay to available atlas angles |
| AN-08 | Empty arrow hand, replacement stack, broken weapon fallback | Ordinary humanoids | Generate readable empty/broken/unequipped visual state. UI transfer is immediate; no mandatory timed reload animation |
| AN-09 | Hit reaction and death, persistent final pose | CH-01–CH-08, CH-10/11 | Adapt K18/K12/K13; generate missing zombie/body poses. Visual reactions do not add stun or invulnerability rules |
| AN-10 | Sit near fire, stand, return to idle | Unassigned CH-01 | Generate seated/standing transitions and equipment fitting; count only actual NPCs in fireside presentation |
| AN-11 | Gather/reach/pick/put away | Gatherer CH-01 | Adapt K17; food appears/disappears according to actual plant/inventory transfers |
| AN-12 | Build, hammer/repair, chop trees | Unassigned CH-01 | Adapt K16; left/right work poses and non-inventory tool props; interrupted work returns to ordinary movement |
| AN-13 | Pick up loot, carry, deliver, stash/equip interaction | Non-busy ordinary NPCs and contextual player interaction | Generate or reuse short reach/carry poses. Keep equipped bags visible; do not show a magical crate that supplies free capacity |
| AN-14 | Eat food / food-use feedback | Survivor characters | Adapt K18 or use a short shared hand-to-mouth pose plus UI feedback. Immediate consumption and cooldown remain as specified |
| AN-15 | Spit wind-up, release, recovery | CH-06 | Generate mouth anchors/pose, projectile, first-hit effect; use AU-08 |
| AN-16 | Bloater burst and corpse settle | CH-08 | Generate cosmetic-only burst; retain readable corpse and contents after stains fade |
| AN-17 | Prey idle, move/flee, hit, death, final carcass | CH-10/11 | K12/K13; no wildlife combat equipment or player-control poses |
| AN-18 | Structural guard idle/aim/fire | CH-09 | Reuse one guard family at 1/2/3 positions; main tier 3 has two one-guard attachments, not six guards |

Food-use, loot, and equip presentation must not delay operations the specification makes immediate. Work and combat visual events must remain synchronized with accepted actions; a purely decorative animation cannot create extra hits, arrows, food, or work.

### 5.3 Local example resources

| Existing local material | Use |
| --- | --- |
| `D:/Work/Games/KingdomZombies/src/godot-game/references/zombie/zombie-idle-original-size.png` and `zombie-idle.png` | Reviewed zombie palette/silhouette reference; adapt into modular body and clothing layers |
| Same directory: `zombie-walk-strip.png`, `zombie-walk-sheet.png`, `walk-frames/walk-01.png` through `walk-08.png` | Existing candidate walk frames; choose one canonical export and verify registration, scale, duplicate frames, and loops before reuse |
| Same directory: `zombie-attack-strip.png`, `zombie-attack-sheet.png`, `attack-frames/attack-01.png` through `attack-08.png` | Existing candidate attack frames; validate against the new hand/weapon action set; not evidence that every required animation exists |
| `D:/Work/Games/KingdomZombies/src/godot-game/references/bases/main-base-tier-1.png`, `main-base-tier-2.png`, `main-base-tier-3.png` | Wood, reinforcement, metal, and mood references. The required tier 1 is fire/boxes, and tier 3 needs two attached towers; do not copy the example silhouettes as the final progression |

The zombie idle image and base tier-3 image were visually reviewed during this audit. Animation candidates were located, not declared production-ready.

## 6. Complete item art and equipment register

Every row requires a legible inventory icon, a localized name/description, definition data, and a reference in the appearance catalog. The 24 icons are separate item identities even where they share donor art. Stack numbers, condition, rotation, weight, and selection are UI overlays, not separately painted icon variants.

Ground drops may use the shared loot-pile presentation; the contents panel must show the correct item icons. Worn/held versions are required only for applicable equipment. A small pouch is a stored container, not an extra equipment slot.

| ID | Item / inventory footprint | Required additional art or resource | Route and donor |
| --- | --- | --- | --- |
| IT-01 | Money, 1×1 | Coin/money icon, stack overlay; optional coin glint in generic loot feedback | Adapt K20 coin-spin frame; one physical currency, no global-wallet artwork |
| IT-02 | Arrows, 1×2 | Bundle icon; opposite-hand stack/nocked-arrow appearance; flying arrow | Adapt K20 arrow; generate bundle/hand representation; projectile shared with towers |
| IT-03 | Knife, 1×2 | Both-hand grips; thrust/slash orientation; corpse-held form | Generate from common metal/wood palette; no suitable dedicated knife donor verified |
| IT-04 | Wooden club, 1×3 | Both-hand grip/swing/corpse form | Generate using K01/K08 wood palette and texture ingredients |
| IT-05 | Machete, 1×3 | Both-hand grip/slash/corpse form | Generate distinctive broad blade; do not substitute a shield/sword icon without adaptation |
| IT-06 | Metal bat, 1×4 | Both-hand grip/swing/corpse form | Generate metal bat using the common metal palette |
| IT-07 | Bow, 2×4 | Held, drawn, released, empty-arrow-hand and corpse forms | Adapt K20 `tools_bow` and K15 motion; icon from the same master artwork |
| IT-08 | Berries, 1×1 | Small food icon; optional hand-to-mouth detail | Adapt K04 berry layer into readable clustered fruit |
| IT-09 | Fruit, 1×1 | Food icon distinct from berries | Adapt K05 ordinary apple layer, redraw/enlarge at the chosen pixel grid if needed |
| IT-10 | Meat, 1×2 | Meat portion icon, food-use detail | Generate; one item definition for both prey species; no cooking/raw-versus-cooked system |
| IT-11 | Basic shirt, 2×2 | Torso/arm/corpse layers; 2×1 pocket diagram/data | Adapt K14 clothing into removable layers, generate missing frames/fits |
| IT-12 | Trousers, 2×2 | Leg/corpse layers; 2×2 pocket data | Adapt K14 and local zombie clothing; support every leg pose and zombie fit |
| IT-13 | Coat, 2×3 | Torso/sleeve/hem/corpse layers; 2×2 pockets | Generate/adapt shirt template and shared palette; do not bake the backpack or armor into it |
| IT-14 | Cap, 2×1 | Head overlay and corpse version | Generate; crown remains visible above it |
| IT-15 | Face covering, 1×1 | Face overlay including changed head poses | Generate; keep mouth/release anchor correct for spitter, without suppressing its attack |
| IT-16 | Gloves, 1×1 | Both-hand layers in all grips/actions | Generate; a pair is one item, fits front/rear hands and wide bodies |
| IT-17 | Boots, 1×2 | Both-foot layers in movement, seated, airborne, and corpse poses | Adapt K14/local reference, generate uncovered poses; one pair is one item |
| IT-18 | Protective helmet, 2×2 | Head armor layer, broken-state feedback, corpse layer | Generate; separate from cosmetic crown; actual protection follows durability |
| IT-19 | Improvised body armor, 2×3 | Readable light improvised armor, all body fits/corpse layer | Generate using shared reinforcement materials; condition overlay can be procedural |
| IT-20 | Plated body armor, 3×3 | Heavier plated silhouette, all body fits/corpse layer | Generate; visually distinct from IT-19 and still compatible with coat/belt/backpack |
| IT-21 | Belt with pouches, 2×1 | Visible belt/pouches; 4×2 storage diagram/data | Generate/adapt shared fabric/leather palette; visible on all relevant bodies |
| IT-22 | Small pouch, 1×2 | Container icon, 2×2 internal grid, open-container header | Generate; reusable UI handles open/closed state; contents do not appear as an extra worn bag |
| IT-23 | Small backpack, 2×3 | Worn bag across poses/directions/corpse; 4×4 internal grid | Generate; front/rear ordering and straps fit every supported humanoid |
| IT-24 | Field backpack, 3×4 | Distinct larger worn bag; 5×6 internal grid | Adapt IT-23 design with larger silhouette; no change to shared container UI behavior |

All icons must remain identifiable at their grid footprint and in the compact HUD/tooltip. Rotating an item rotates its footprint and image consistently. A generated red cross/broken badge may communicate unusable condition without destroying the item's identity. Container art must not obscure quantity, nesting, or the fact that a filled bag contains actual items.

## 7. Buildings, map, and environment

### 7.1 Building and site output register

| ID | Required asset family / instances | Reuse route | States and dependencies |
| --- | --- | --- | --- |
| BL-01 | Main tier 1: campfire, boxes, accessible stash, seating | Adapt K06/K07/K30; generate rough boxes and composition | Intact, damaged, paid repair feedback, defeated remains; 8 baseline fireside places; no wooden-house silhouette |
| BL-02 | Main tier 2: wooden shelter with fire/stash access | Generate silhouette using K08/K09/K10/K30 wood pieces and local base style | Intact, damage, upgrade/repair overlays, defeated remains; 12 fireside places; keeps the common stash access |
| BL-03 | Main tier 3: metal-reinforced shelter and two attached guard towers | Adapt BL-02 + K09; generate metal reinforcement and attachment layout | Intact/damaged/repair/defeat; 16 fireside places; one CH-09 guard on each attachment; shared main-building health |
| BL-04 | Wall tier 1: wood | Reuse/adapt K08 `wall_1` | Unbuilt site, new frame, intact, damage, upgrade overlay, paid repair, destruction, persistent rebuild site |
| BL-05 | Wall tier 2: reinforced wood | Reuse/adapt K08 `wall_2` | Same lifecycle, stronger visible bracing; previous tier remains recognizable during upgrade |
| BL-06 | Wall tier 3: metal-reinforced | Adapt BL-05 + generated metal parts | Same lifecycle, matching metal wreck pieces; original stone `wall_3` is not the final asset |
| BL-07 | Standalone tower tier 1: wooden platform / one archer | Adapt K09 + CH-09 | New frame, intact/damage, upgrade/repair, destruction/ruins; one guard anchor and arrow-release position |
| BL-08 | Standalone tower tier 2: reinforced wood / two archers | Adapt BL-07 + generated wider/braced layout | Two independently visible/firing guard anchors; old one-guard tier stays functional during upgrade |
| BL-09 | Standalone tower tier 3: metal-reinforced / three archers | Adapt BL-08 + generated metal/layout | Three guard anchors, all readable; new-tier archers do not appear before completion |
| BL-10 | Fixed wall/tower site markings and locks | Adapt K08/K09/K22, generate labels/icons | Wall versus tower, available, tree-blocked, portal-blocked, main-tier-blocked, unaffordable, paid/in progress, unsafe/waiting |
| BL-11 | Construction/upgrade/repair component kit | Reuse K10, adapt K08 fragments, generate progress/health indicators | Fits all nine tier appearances; damaged unfinished frame; distinct upgrade and repair status; old completed tier stays visible/functioning |
| BL-12 | Destruction/rebuild kit | Reuse appropriate K08/K11 fragments; generate tower/main metal/wood rubble | Ruins retain previous completed tier for rebuild UI; no guard bodies/bows/crowns appear as loot |
| BL-13 | Stash access marker and chest/boxes | Adapt K07 + generated storage symbol | All three main tiers; opened/closed/selected state; stash can remain open for multiple actors without a physically exclusive lid animation |
| BL-14 | Fireside seating layout | Generate placement data around K06/main layouts | Tiered 8/12/16 capacity; no extra recruit assets for empty places; overflow survivors retain valid world positions |
| BL-15 | Left/right rally point | Adapt K22 banner, generate left/right identity | Moves with the current border; idle/ready/launching/assault active/recalling/returning/no target or insufficient squad feedback |
| BL-16 | Portal set, six placed instances | Reuse/adapt K11 + FX-05 | Intact closed/idle, provoked warning, spawning, taking damage, destroy transition, permanent passable ruins; source/side identifiable in warnings |

Damage and work states may be generated from modular overlays/fragments rather than separate full buildings. Record their thresholds/visibility in presentation data. Do not bake a tier's archers into the building image: each must animate and face its target independently. Attached main-building towers use the tier-1 guard/attack presentation and share the main health indicator.

### 7.2 World and environmental resource register

| ID | Required output | Reuse route | Minimum coverage |
| --- | --- | --- | --- |
| EN-01 | One authored map composition | Generate map data using K01–K05/K22 scenery and BL assets | Fixed positions in the spec baseline, mirrored progression with deliberate visual variation, continuous reachable ground, map edges, six portal barriers, no accidental jump bypass |
| EN-02 | Ground surface, terrain edge/fill, foreground dressing | Reuse/adapt K02 | Continuous readable walking surface, small variation, correct near/far draw order; no required water-crossing or platforming mechanic |
| EN-03 | Near forest, middle forest, distant hills/sky | Reuse K01/K02/K03 | Parallax layers and horizon, legible characters/projectiles at day/night, boundary scenery |
| EN-04 | Day/night lighting and sky treatment | Generate color curves/light settings using K03/K29 | Dawn/day/dusk/night transitions, campfire/structure light, readable darkness, no duplicate day/night atlas for every item |
| EN-05 | Six tree-clearing groups | Adapt K01 + K22 mark + FX-03 | Intact, marked/paid, chopping, falling both directions where appropriate, cleared/stump; opening the build site does not remove food plants |
| EN-06 | Four berry plants | Reuse/adapt K04 | Full/partly harvested/empty/refilled; food count derived from configured stock, not a fixed number of painted berries |
| EN-07 | Two fruit plants | Adapt K05 | Same stock states; fruit and vegetation separable for daily refill |
| EN-08 | Six wildlife zones and prey placements | Generate authored placement data; reuse CH-10/11 | Spawnable/reachable ground, daily batch positions, existing wildlife retained, no mandatory visual zone border |
| EN-09 | Interaction/selection markers in the world | Generate shapes/outlines; reuse K21/K22 where suitable | Nearby character/corpse/site/plant/stash/rally identification, overlapping-target selector, no target ambiguity beneath panels |
| EN-10 | Explored-map representation | Generate from EN-01; reuse K25 symbols | Shared reveal mask, known portal/defense states, current player and teammates, moving camp borders; undiscovered portals remain hidden |
| EN-11 | Environmental prop kit | Reuse small K01/K02/K06 details | Stones, grass, bushes, roots, logs/stumps, firepit details sufficient to finish the one map; variation from a small coherent kit |

Map edge art and portal barriers need separate authored collision/navigation data. Wood, stones, and leaves used decoratively do not become harvestable material items. Rivers, reflections, rain, seasonal variants, towns, cars, and extra apocalypse landmarks are optional art exploration and are not minimum completion requirements.

## 8. Effects, projectiles, and material resources

| ID | Required resource | Route / source | Behavior and coverage |
| --- | --- | --- | --- |
| FX-01 | Arrow projectile and shot/impact feedback | Reuse K20 arrow, adapt K23 particles | Friendly/enemy/tower shots; bow-release socket; wood/metal/ground/flesh/portal impact feedback; arrows never become recoverable ground loot |
| FX-02 | Melee swing and hit feedback | Generate arcs/small impact particles; reuse K23 textures if suitable | Fists, knife, club, machete, bat, dual weapons; flesh/armor/wood/metal/portal contacts; feedback follows successful hits without adding damage |
| FX-03 | Construction, repair, tree cutting/fall, structure collapse | Adapt K10/K08/K23 | Wood chips, small sparks for metal work, dust, falling leaves, ruin settling; no produced wood/resource drops |
| FX-04 | Fire, smoke, embers, local light | Reuse K06/K23; generate light/material settings | Main fire at all tiers and suitable lamps; cosmetic warmth/light, no new burning-status mechanic |
| FX-05 | Portal interior, warning, spawn, hit, destruction | Adapt K11/K23 | Opening/closing/active effect, provocation cue, retaliation cue, spawn origin, hit flash/debris, destroyed inert state; no regrowth effect after destruction |
| FX-06 | Spit projectile and first-hit splash | Adapt K23 goo/splash ingredients; generate motion/material | Readable mouth-to-target projectile, first solid valid hit only, immediate impact; no puddle damage or persistent acid hazard |
| FX-07 | Bloater death splash and stains | Adapt K23/generated shapes | Wall/ground marks, configured radius/lifetime, fading independent of the actual corpse; no collision, status icon, armor damage, or hazard warning |
| FX-08 | Character hit, death, recovery/food feedback | Generate palette flash/small puffs and HUD changes | Survivor/zombies/prey as appropriate, short food/heal/stamina feedback; blood detail optional; do not obscure equipment or corpse selection |
| FX-09 | Selection, valid/invalid placement, ownership feedback | Generate outlines, tint/mask materials, icons | Actor/loot selection; inventory drag footprint; valid/blocked/collision/legacy states; color accompanied by shape or text |
| FX-10 | Crown/control transition and player identity | Adapt K21 + generated short highlight | New leader gains marker once; former NPC/leader loses it correctly; no dropped crown or resurrection effect |
| FX-11 | Condition/damage/palette material profiles | Generate reusable settings/masks based on K29 and source recolor behavior | Day/night tint, actual clothing colors, metal/wood damage, broken-equipment indicator; no need for hand-painted variants for every health number |
| FX-12 | Warning and outcome presentation | Generate overlays/short screen accents, reuse restrained K23 details if helpful | Nightfall, retaliation, portal loss, victory/defeat, connection loss; remains visible above inventory panels; never conceals urgent danger |

Store effect lifetimes, visibility thresholds, sound bindings, and crowd limits in presentation data. At the target scale, reuse a small set of textures and limit decorative emissions; do not meet performance targets by hiding required worn equipment or deleting nonempty corpse visuals/interactions.

## 9. Interface, text, and accessibility resources

### 9.1 Shared UI kit

| ID | Output | Reuse route | Required states |
| --- | --- | --- | --- |
| UI-01 | Buttons, panel borders/backgrounds, tabs, lists, scrollbars, text inputs, sliders, checkboxes, tooltips, modal/toast framing | Adapt K24; generate missing simple geometry | Normal/hover/focus/pressed/disabled/selected, empty/loading/error; scalable borders and clipping at 80–150% UI scale |
| UI-02 | Fonts, typography sizes, color/contrast palette | Reuse K27, generate style data | Readable body/numerals/item counts; headings; missing-glyph fallback; expanded translation text; no gameplay text baked only into art |
| UI-03 | Cursor, aim reticle, interaction cursor, keyboard/mouse prompts | Reuse/adapt K26; generate missing glyphs | Current remapped binding names, mouse buttons, modifier combinations, unbound/conflict/reset feedback; no controller art required |
| UI-04 | Status icon set | Generate, reuse K20/K21/K25 shapes where suitable | Health, stamina, weight, money, ammo, condition/broken, locked, warning, error, waiting, success, busy, food, role/activity, save and connection |
| UI-05 | Item-grid/equipment kit | Generate; use IT-01–IT-24 icons | Empty/occupied cells, eleven slot placeholders, item drag ghost, rotated preview, valid/invalid fit, stack/split dialog, container breadcrumb/depth, durability, weight, quick-slot assignment, tooltips |
| UI-06 | Legacy inventory treatment | Generate using UI-01/UI-05 | Old saved footprint/bounds, incompatible slot/stack/depth badge, removal guidance, current destination preview, extraction failure explanation; contents remain readable and removable |
| UI-07 | Player and multiplayer identity kit | Adapt CH-12/K25 and generate labels | Four colors plus number/name, host/guest/controller/spectator/offline state, teammate map markers; no platform logos/accounts |
| UI-08 | Game identity and launch icon | Generate using the game's name/crown/camp motifs | Title text/art and application icon for the two supported desktop deliveries; no Kingdom Two Crowns or publisher logo reuse |

### 9.2 Required screen/panel resource assemblies

These are distinct layouts/content definitions composed from the shared kit, not a request for one large flattened screenshot per screen.

| ID | Surface | Required content and exceptional states |
| --- | --- | --- |
| UI-09 | Main menu | New solo/host run, LAN list, manual address entry, saved runs, settings, controls, exit; new game title; no mandatory cinematic/menu music |
| UI-10 | Host/lobby and LAN browser | Session name, host/address, participant capacity, in-progress and compatibility state, refresh/join/start/resume; empty list, discovery in progress, full/incompatible/unreachable session |
| UI-11 | Join/loading/reconnect flow | Address validation, progress/waiting, timeout, mismatch detail, host loss, guest return; existing world continues for other players |
| UI-12 | In-run HUD | Health/stamina, carried cash, load band, both hands/held equipment, hand-arrow count, four quick slots, day/time/phase, warnings and teammate indicators |
| UI-13 | Own character inventory | Eleven slots and actual pocket/bag grids, item details/condition/weight, rotation/split/transfer, nested-container views, explicit world-drop target, quick-slot references |
| UI-14 | Shared stash | Character and unlimited paged stash, page/filter controls, name/type filter, quick transfer, actual current contents, empty/result-empty states, losing shared-operation feedback |
| UI-15 | NPC/corpse/ground-loot panel | Target identity and gear, permitted transfers/equipping, actual immediate contents, overlapping-body selector, emptied body/pile, out-of-range/dead target/access closed feedback |
| UI-16 | People/recruitment/job menu | Buy recruit, tiered fireside/total cap, role counts, individual roles, cost, waiting-for-gear, actual activity, population limit/insufficient cash/controlled survivor errors |
| UI-17 | Food sale | Stash food quantities only, selected quantity, per-unit/total price, stash-money result, nothing to sell, concurrent stock change; no equipment/crafting shop tabs |
| UI-18 | Site/building/repair panel | Tier and health, locked reason, prerequisite, personal-cash price, upgrade/build/rebuild/repair action, purchased work/progress, workers absent/unsafe status, destroyed previous-tier state |
| UI-19 | Rally panel | Side, stationed/ready count, desired squad mix, target portal, cost, active/recall/returning state, no target/insufficient squad/already active feedback |
| UI-20 | Minimap and expanded map | Shared explored terrain and known objects, active/destroyed portals, defenses, player/teammates, reveal legend; scale/expand control; no unexplored enemy radar |
| UI-21 | Spectator and succession | Follow/cycle living teammates, waiting for eligible survivor, control-transfer name/marker, disabled actions with reason; no extra character creation |
| UI-22 | Save/load/run list | Run name/day/result, date/time, manual/autosave marker, in-progress/success/failure, compatibility/missing-definition detail, retained older saves, selected run context |
| UI-23 | Exit/save failure and session end | Host retry/return to play/leave without latest save, guest leave, host-ended/lost session explanation; labels must state the scope of the host's exit |
| UI-24 | Victory/defeat | Victory Continue/Main menu plus remaining-danger text; defeat load/new/menu; recorded earlier victory versus later continuation defeat; no claim that live zombies vanished |
| UI-25 | In-run menu/settings/controls | Display mode/resolution, UI scale, master/effects/ambience sliders, remapping/conflict handling/reset, controls reference; active world continues, no Pause label promising frozen play |
| UI-26 | Notifications and transactional errors | Damage/nightfall/retaliation, portal destroyed, order accepted/rejected, missing equipment, insufficient carried cash, inventory full/invalid, target changed, save result, connection failure; readable over every panel |
| UI-27 | Configuration/content error report | File/field/value/reason, missing asset/item/translation ID, incompatible save/content distinction, recoverable old-layout notice; no silent hardcoded fallback message |

### 9.3 English text and translation catalog

Create `TXT-01`, a complete English message catalog for UI-01–UI-27, all 24 item names/descriptions, all seven zombie names, survivor roles, structure/site tiers and actions, food/source names, quick-slot prompts, map symbols, session/save states, outcomes, and error reasons. Include plural/quantity forms and formatted placeholders for day, count, player, item, cost, target, and file/field values.

Create `TXT-02`, the default control labels/help reference and expanded-text test substitutions. They use the current bindings, not hardcoded keys inside images. No tutorial scripts, narrated instructions, or second shipped locale are required.

Muted audio must leave the same critical warning information visible. Color alone must not identify a player, legal inventory move, or danger state. Long strings, large numbers, nested containers, many stash pages, and four player names must fit through wrapping, resizing, or scrolling.

## 10. Sound and ambience register

Each row is a required event category; categories may share suitable recordings. Reuse the recovered recording before synthesizing a replacement. An unfamiliar name is a search lead, not proof of auditory suitability. **No audio in this audit was auditioned or approved as a final mix.** Eight selected clips were checked for duration, sample rate, channels, and valid backing-file byte ranges; these carry M below. Other exact anchors carry C.

| ID | Event coverage | Exact recovered candidates | Route / remaining work |
| --- | --- | --- | --- |
| AU-01 | Survivor footsteps, walking/sprinting, NPC carrying/retreat | `R:3294` `citizen_run01`; `R:4949` `citizen_run02`; `citizen_run*` family, C | Reuse after audition; vary timing with motion and select ground-appropriate variants; do not reuse horse hoofbeats |
| AU-02 | Jump takeoff/landing | AU-01 movement/impact recordings, C | Adapt short footfall/thump; synthesize missing takeoff/landing detail; no requirement for spoken exertion |
| AU-03 | Fist/melee swings, including two weapons | `R:3369` `knight_newattack1`; `R:4872` `knight_newattack2`, C | Adapt appropriate swing portions for knife/machete/club/bat; generate distinct fist/wood/metal detail only where the donors are unsuitable |
| AU-04 | Hits on survivor/zombie flesh and armor | `R:5123` `troll_hits_player`; `R:2880` `Greed_Archer_Take_Damage_Blunt_02`; `R:3700` `Greed_Archer_Take_Damage_Arrow_05`, C | Reuse/adapt impact layers; reduce excessive fantasy creature detail; no friendly-hit cue for attacks that cause no friendly damage |
| AU-05 | Equipment reaches zero condition | `R:4523` `wall_wood_wreck`; `R:3574` `wall_iron_wreck01`, C | Adapt small crack/clink portions or generate a short break cue; do not play a whole building-collapse recording for every worn-out item |
| AU-06 | Bow release, ordinary and tower archers | `R:3288` `arrow_shoot`, M | Reuse; shared event library, no audible claim that tower arrows need replenishment |
| AU-07 | Arrow impact on terrain, wood/metal, prey, portal | `R:5476` `arrow_generic_hit`; `R:3640` `arrow_hit_ground`; `R:4434` `arrow_hit_wall1`; `R:5020` `arrow_hit_wall2`; `R:3357` `arrow_hit_bunny`; `R:4692` `arrow_hits_portal01`, C | Reuse after material audition; one sound for actual first hit, not one per passed ally; no arrow-pickup sound after firing |
| AU-08 | Spit wind-up/release/direct impact | `R:2790` `lizard_spitexplosion_altv`, C | Adapt wet portion only if suitable; generate a short wet release/impact if donor is predominantly fire/explosion. No persistent burning/acid loop |
| AU-09 | Zombie movement, attack, hit, death | `R:2864` `troll_movement14`; `R:3150` `troll_charge01`; `R:3131` `Greed_Archer_Die_01`; `R:2804` `Greed_Knight_Death_02`; `R:3554` `sfx_greed_crusher_die_01`, C | Reuse/adapt a small undead sound bank with pitch/material variants; generate gaps for the seven archetypes. A distinct recording for every type/action is not necessary if the shared bank is readable |
| AU-10 | Survivor/player damage and death; control succession | AU-04 impacts; `R:5736` `player_loses_crown01`, C | Audition the loss cue; adapt or generate a restrained death/control-transfer cue. No dropped crown mechanic or mandatory voiced dialogue |
| AU-11 | Prey flee/hit/death | `R:3944` `deer_jump_v2`; `R:4247` `deer_is_killed`; `R:3811` `sfx_critter_hare_die_01`, C | Reuse/adapt rabbit/deer cues; combine with movement where appropriate |
| AU-12 | Wood/metal building and paid repair | `R:5272` `worker_hammer_wood01`, M; `R:3945` `worker_hammer_wood02`; `R:3804` `worker_hammer_iron01`; `R:5176` `worker_hammer_iron02`, C | Reuse randomized contact variants, driven by actual work; no sound implying free automatic repairs |
| AU-13 | Tree marking/chopping/fall | `R:4611` `payment_choptree01`; `R:3741` `worker_chops_tree01`; `R:3672` `worker_chops_tree02`; AU-15 wood collapse, C | Reuse chop/payment; adapt collapse/foliage tail for felling; no coin-yield or wood-pickup cue |
| AU-14 | Build/upgrade begins and completes | `R:2862` `payment_build`; `R:4543` `wall_completed`; `R:4245` `ironbuilding_complete`, C | Reuse work/complete cues; emit completion once per accepted finished order, including on multiplayer clients |
| AU-15 | Wood/metal defenses hit and collapse | `R:4169` `troll_hitwallwood01`; `R:4041` `troll_hitwalliron01`; `R:4523` `wall_wood_wreck`; `R:5654` `wall_wood_wreck2`; `R:3574` `wall_iron_wreck01`; `R:5154` `wall_iron_wreck02`, C | Reuse material-matched impacts and wreck cues; main-building collapse may layer these with defeat feedback |
| AU-16 | Money pickup/drop, payment, food-sale proceeds | `R:3254` `coin_pickup_v3`, M; `R:4864` `coin_drophitgroundv2`; `R:2862` `payment_build`, C | Reuse; sounds follow committed physical transfers/payments, not a failed or duplicated transaction |
| AU-17 | Recruit appears / job assigned / equipment acquired | `R:3195` `peasant_pickupbow`; `R:3863` `peasant_pickuphammer_v2`; `R:4973` `soldiercloth_paid`, C | Adapt appropriate confirmation/equipment layers and AU-16; no external camp or equipment-store sound sequence |
| AU-18 | Inventory/stash/container open/close, move, split, rotate, equip | `S0:438` `menu_openv3`; `S0:485` `menu_closev2`; `S0:449` `ui_select`, M for select and C others; `R:3755` `chest_open`, C | Reuse short UI sounds; generate subtle material handling if needed. Avoid overwhelming sound on repeated grid movement |
| AU-19 | Food consumed; food sold | AU-16 sale/payment; food-use detail has no verified suitable human donor | Generate short consumption/confirmation; no hunger/cooking loop; do not label animal-eating recordings as ready human sounds |
| AU-20 | Day/night transition, retaliation warning, assault/recall | `S2:68` `ui_daycounter_new_v2`; `R:5586` `horn_blown_reverb`, C | Reuse/adapt short cues, give retaliation a distinguishable warning; pair with UI-26. Use trimmed suitable effects, not the original musical score |
| AU-21 | Active portal ambience/open/spawn | `R:3558` `env_portalnormal`, M; `R:2860` `sfx_ruler_heimdall_special_portal_open`; `R:5582` matching close, C | Reuse ambience; audition/trim open/close as generic effect candidates, removing any teleport implication in behavior |
| AU-22 | Portal destruction | `R:4364` `finalportal_collapsev3`; AU-07 portal impacts, C | Reuse/adapt collapse, sync to the new permanent-destruction presentation; no rebuild/revival sounds |
| AU-23 | Victory and defeat | AU-10/AU-15/AU-22 ingredients; no final standalone outcome sound selected | Generate short non-musical success/failure effect if existing suitable cues cannot be reused; no soundtrack requirement |
| AU-24 | Guest joins/leaves, connection failure | `S0:507` `ui_playerjoins`; `S0:437` `ui_playerleaves`, C | Reuse; generate short failure variant if necessary; always show explanatory text |
| AU-25 | Save success/failure, invalid action/content error | `S0:449` `ui_select`, M; other UI bank candidates | Reuse for success, generate distinguishable negative cue as needed; no loss of error detail when muted |
| AU-26 | UI hover/confirm/cancel | `S0:420` `menu_mouseover01`; `S0:392` `menu_mouseover02`; `S0:449` `ui_select`; `S0:485` `menu_closev2`, M select/C remainder | Reuse; consistent throughout all new screens |
| AU-27 | Day/night ambient beds | `S2:125` `amb_forest_123_day01_q0046`; `S2:138` `amb_forest_123_night01_q0064`, M; `S2:66` meadow day and `S2:47` meadow night, C | Reuse selected forest/meadow beds with clean loops and crossfades; no need to export every biome/season |
| AU-28 | Local fire and environmental wind | `R:4441` `env_homefire`, M; `S2:152` `amblayer_forest_123_wind01_loop_q0003`, C | Reuse positional fire and restrained wind; optional layer, not a weather mechanic |
| AU-29 | Cosmetic bloater splash | AU-08 wet ingredients, C | Adapt or generate wet burst/death layer; no lingering hazard loop after the stain remains |
| AU-30 | Gathering, loot pickup/carry delivery, cloth/bag handling | AU-18 small handling/confirmation; no complete vegetation/bag bank verified | Reuse trimmed suitable handling sounds, generate missing rustle/contact detail; actual item transfer remains the source of the event |

For every chosen recording, export the complete audio payload to a supported production format and record duration, channel count, rate, trim, loop points, gain, and source locator. Audition for unwanted music, creature identity, spoken words, clipping, silence, and loop seams. Keep stereo ambience; choose appropriate positional playback for effects. Recording reuse does not require preserving the original game's mixer.

The checked samples are 44.1 kHz stereo. Examples: `arrow_shoot` approximately 0.068 s, wood hammer 0.327 s, `env_portalnormal` 17.872 s, `env_homefire` 12.935 s, coin pickup 0.750 s, UI select 0.210 s, forest day 92 s, forest night 73.392 s. Byte ranges resolve in `resources.resource`, `sharedassets0.resource`, and `sharedassets2.resource`; this proves availability, not auditory quality or seamless looping.

Create one sound-event catalog with randomized variants, distance/attenuation, priority, volume bus, cooldown/repetition policy, and lifecycle bindings. Apply the specification's proposed 24 concurrent gameplay sounds and four overlapping instances per repeated effect from configuration. Do not download or generate music to fill the excluded music category.

## 11. Authored data and supporting resources

These are production resources alongside sprites and sounds. Their file formats and runtime representation are implementation choices. Separate editable files, stable IDs, validation, and documentation are required; original prices, timers, two-player assumptions, and reference-game rosters are not this game's baseline.

### 11.1 Game configuration packs

| ID | Required separate domain / resource | Contents to author | Recovery use |
| --- | --- | --- | --- |
| DT-01 | Run setup | Eight starting survivors, outfit, initial stash, caps, default map/day/phase | Generate from spec §2; no original starting wallet or mount |
| DT-02 | Time | Day/night 600/300 seconds, first-night grace, transition parameters | Generate from spec §3; original time curves may inform visual transitions only |
| DT-03 | Survivor definitions | Health/stamina, move/jump, costs/recovery, safe distances, load behavior, appearance refs | Generate from spec §4; K14 motion/recolor data are presentation inputs |
| DT-04 | Item definitions | All IT IDs, stable definition IDs, footprints, weight, stack, rotation, slots, container grids, condition, icon/appearance IDs | Generate from spec §5; reference asset inventory is not this data |
| DT-05 | Combat/armor/projectiles | Unarmed/weapons, timing/costs/damage/reach, arrows/spit, armor/durability, hand rules and contact limits | Generate from spec §4; reuse K20 projectile art and selected timing reference only |
| DT-06 | Inventory/access/cleanup | Stash pages, nesting, quick slots, interaction range, corpse/pile cleanup, legacy-layout display rules | Generate from spec §5; includes reusable grid/layout templates |
| DT-07 | Recruitment/jobs/work | Prices, fireside tier limits, role needs/preferences, reserves, safety/work order priorities and timings | Generate from §§6–7; job-source concepts may inform structure, not values |
| DT-08 | Food/plants/wildlife | Food recovery/sale, plant stock/refill, daily prey batches/caps, rabbit/deer attributes and meat inventories | Generate from §6; reuse K04/K05/K12/K13 references |
| DT-09 | Map/site content | Terrain, parallax positions, collision, navigation, 6 portals, 8+8 defense sites, clearing groups, 6 food plants and 6 wildlife zones, boundaries, reveal | Author EN-01 from §3; reuse scenery and assembly details, not original island layouts |
| DT-10 | Structure definitions | Three main/wall/tower tier appearances, prices, HP, work, prerequisites, repair/rebuild valuation, attachment refs | Generate from §8; consult K29 upgrade/config exports only for assembly relationships |
| DT-11 | Tower/guard presentation and attack profiles | 1/2/3 standalone positions, two single-guard main attachments, range/damage/interval/targets | Generate from §8; K09/K15 and source guard slots inform graphics placement |
| DT-12 | Zombie archetypes | Seven CH variants, stats, movement/attacks, equipment compatibility/fitting, appearance/effect refs | Generate from §9; Greed actors are reference only |
| DT-13 | Zombie loot | Guarantees, pools, quality, condition, optional rolls, arrow-hand supplies, legal placement | Generate from §9; loot exists before death, not a death-only visual reward table |
| DT-14 | **Zombie progression, independent file** | Three-day interval, stages, wave counts, variant weights, quality, caps/queue policies | Generate from §9; do not copy Kingdom boss-day or player-count scaling tables |
| DT-15 | Portals/assaults | Portal health/defenders, warnings, cooldowns, wave release, rally price/size/mix, recall and return | Generate from §§7/9; K11 animation/effects reused under permanent-destruction rules |
| DT-16 | Session/save defaults | Up-to-four participants, discovery/join/timeout values, identity/content metadata, retention, UI event refs | Generate from §§10–11; no inherited two-player cap or platform-only IDs |
| DT-17 | Presentation | Camera/minimap, pixel density, UI scale defaults, light/palette curves, effect durations, sound event/bus/limit settings | Adapt K03/K23/K24/K27/K29; all numeric presentation settings documented |
| DT-18 | English translation catalog | TXT-01/02, every required label/message/placeholder and font assignment | Generate new game text; reuse extracted fonts, not the original tutorial/dialogue/localization corpus |

Document every field's meaning, unit, default, allowed values, and dependencies. Include the shipped complete baseline and examples of editing representative item, cost, zombie, progression, sound, and map fields. Validate missing images/sounds/strings, not merely numbers.

### 11.2 Presentation, persistence, packaging, and verification packs

| ID | Resource | Required content |
| --- | --- | --- |
| DT-19 | Asset provenance/export manifest | Output ID/path/version; donor owning file/type/path ID/name; original file identity from preservation manifest; exported dimensions/hash; transformation/generation recipe; evidence/approval state |
| DT-20 | Animation and fitting library | AN-01–AN-18 clips, frames/timing/events/loops; per-body attachment/occlusion/palette data; direction rules, effect/sound bindings, corpse endpoint metadata; no callbacks for excluded mechanics |
| DT-21 | World assembly and interaction metadata | Building layers and damage fragments; three tier footprints, work/repair points, fireside and guard sockets, projectile origins, collision/faction blocking, pickup/stash/rally anchors, navigation and map icon associations |
| DT-22 | UI layouts/themes | UI-01–UI-27 compositions, inventory/page templates, eleven equipment-slot layouts, two-hand combinations, scalable typography, state/error styles, expanded-text fixtures |
| DT-23 | Sound bank definitions | AU-01–AU-30 event routing, selected recordings, loops, mix/priority/repeat rules; default volume preferences; no music bank needed |
| DT-24 | Save/content schema and compatibility resources | Stable content IDs/versions, legacy saved grid dimensions/positions/orientation, visual-state identifiers, map exploration/ruins, phase/work/queue state references, validation errors; no serialized image/audio duplication per save |
| DT-25 | Local preferences/default bindings | Display/resolution/UI scale, master/effects/ambience, action-to-key mapping and reset; independent of host balance |
| DT-26 | Validation fixtures / inspection scenes | All item icons; every body with legal gear; dual mixed weapons and bows; corpse looting; every structure/state; portal/acid effects; day/night; all UI errors/legacy grids; four identities; 100-survivor/300-zombie crowd and persistent corpse scenario |
| DT-27 | Desktop delivery resources | Application icon/title, included selected content/configuration/font/audio files, packaged English catalog and designer field documentation, content version, startup/readme information and tested Windows/Linux record |

Fixtures are development/acceptance resources, not additional maps or gameplay modes. Store generated test inputs separately from shipped default balance. Save/late-join reconstruction must restore persistent appearances without replaying old one-shot death, destruction, payment, recruitment, or warning effects.

### 11.3 Suggested production organization

The following is an organization proposal, not a request to create these folders during this documentation task:

```text
assets/
  characters/       survivor, seven zombies, tower guard, wildlife
  equipment/        shared item masters and per-body fitted layers
  buildings/        main, walls, towers, sites, ruins, scaffolds
  world/            terrain, forest, sky, plants, props, portals
  effects/          textures and reusable presentation definitions
  ui/               icons, themes, fonts, cursors, shared widgets
  audio/            selected effects and ambient recordings
content/
  map/              one authored map and placement/interaction data
  animation/        timing, attachment, fit, event and corpse metadata
  ui/               screens, layouts and messages/state bindings
  config/           separate documented domains DT-01 through DT-18
  manifests/        asset origins, export/generation recipes, versions
validation/         content fixtures and acceptance evidence
```

Choose one stable naming convention and map it to the IDs in this document. Do not use original source path IDs as the only long-term identity of new gameplay items. Keep source/provenance information while assigning independent stable output IDs.

## 12. Completion checks and remaining production work

### 12.1 Specification coverage crosswalk

| Specification coverage | Assets/resources carrying the requirement |
| --- | --- |
| §§1–3: scope, starting run, succession, time, map, outcomes | CH-01/12/13; BL/EN families; UI-09–UI-12, UI-20–UI-25; DT-01/02/09/16; outcome/warning sounds |
| §4: movement, health/stamina, food, hands/combat, condition | AN-01–AN-09/14–AN-18; IT roster; FX-01/02/06–FX-11; UI-12/13; DT-03/05; AU-01–AU-11/19/29 |
| §5: inventories, stash, visible equipment, death, legacy layouts | CH-13; all IT; UI-05/06/13–UI-15; BL-13; DT-04/06/20/24; AU-18/30 |
| §§6–7: recruitment, food economy, jobs, work, assaults | AN-10–AN-14; EN-05–EN-08; BL-14/15; UI-16/17/19; DT-07/08/15; AU-12–AU-20/30 |
| §8: main/wall/tower progression and paid work | BL-01–BL-14; CH-09; FX-03/04; UI-18; DT-10/11/21; AU-12–AU-15 |
| §9: seven zombies, loot, escalation, portals, waves, retreat | CH-02–CH-08; all compatible IT; BL-16; FX-05–FX-07; UI-20/26; DT-12–DT-15; AU-08/09/20–AU-22/29 |
| §§10–11: LAN, simultaneous actions, reconnect, save/load | UI-07/10/11/14/21–UI-23/26/27; DT-16/19/24/25; AU-24/25; stable appearance IDs for shared state |
| §12: all designer-editable configuration | DT-01–DT-18 plus field documentation/validation and asset reference manifests |
| §13: full interface, controls, art, audio, translation | All CH/AN/IT/BL/EN/FX/UI/AU; TXT-01/02; DT-17–DT-23/25 |
| §14: completed game and acceptance | DT-26/27; checks below; selected art/audio must be integrated, not just present on disk |
| §§15–16: reference material and proposed choices | K01–K30, source observations, local example table, proposal labels and independent new configuration values |

### 12.2 All 47 acceptance scenarios checked for resource coverage

| Acceptance IDs | Resources and states that must be present to exercise them |
| --- | --- |
| A-01–A-03 | Initial outfits/stash icons, day/night/environment transitions, six portal presentations, grace/stage configuration, HUD/wave warning cues |
| A-04–A-06 | Equipped corpse, crown removal/new assignment, four identity markers, successor camera target, spectator waiting/transfer UI, recruit appearance |
| A-07–A-09 | Main destruction, defeat/victory overlays and sounds, continued-map visuals, remaining zombie corpses/actors, recorded result/save UI |
| A-10–A-13 | Tiered fireside seats, recruitment/job/limit/waiting labels, all four role action sets, visible equipment upgrades and preserved container views |
| A-14–A-16 | Empty/broken weapon state, fallback fists/gear, restock/carry poses, civilian fleeing, nighttime retreat/work and squad movement |
| A-17–A-21 | Tree mark/chop/fall, every structure tier, frames/damage/repair/ruins, old walls, correct standalone 1/2/3 guards and main 1+1 guards, no separate guard loot |
| A-22–A-25 | All 24 icons, eleven slots/two hands, grid/rotation/split/nested views, four quick slots, weight/condition feedback, immediate open panels, unlimited stash pages |
| A-26–A-27 | Equipped bodies and carried cargo on corpse inventories, updated appearance after looting, nonempty persistence, empty-body/pile cleanup presentation |
| A-28–A-30 | Shared-operation failure/update feedback, people/build/sale panels, physical money/food icons, harvest/depleted/refilled plants, prey/carcass states, dawn/save indicators |
| A-31–A-32 | Friendly/enemy/tower arrows, impact materials, visible ammo/equipment, all seven zombies, spit impact, cosmetic-only bloater splash, ordinary persistent loot |
| A-33–A-36 | Portal barrier/ruin, defender and retaliation warning/spawn, queue/cap data, dawn retreat/disappearance without loot, no portal regeneration |
| A-37 | Left/right rally marker and panel, squad readiness, assault/recall/return status, target-portal icon/state |
| A-38–A-40 | LAN/manual-address/lobby/join/error resources, four-player identity, reconnect/returned-NPC state, host-end/connection-loss messaging and sound |
| A-41–A-44 | Saved corpse/gear/work/map/warning visual states, content/definition IDs, legacy readable/extractable grids, save/config errors, earlier-save list |
| A-45–A-46 | Independent player markers/camera targets, shared explored minimap, live remapped key labels, UI scaling, expanded strings, visible muted-audio warnings |
| A-47 | Integrated full resource pack, desktop launch assets, reference hardware/test record, all-state visual fixture and required crowd/endurance content |

### 12.3 Production acceptance checklist

- [ ] Every CH, AN, IT, BL, EN, FX, UI, AU, TXT, and DT requirement maps to an actual output or explicit shared output. No untracked missing icon, sound binding, state, or reference remains.
- [ ] All selected recovered donors have correct file/type/ID/name, required streams, complete decoded output, origin record, and verified dimensions/alpha. Duplicate names are resolved explicitly.
- [ ] All 24 items are identifiable in inventory and tooltips. Every worn/held item works on CH-01–CH-08 in required states; both hand orientations, mixed dual weapons, bows/arrows, clothing pockets, armor, and both backpacks are checked.
- [ ] All seven zombie silhouettes are recognizable in a crowd; gear reflects actual equipped inventory and wear. No fantasy Greed stand-in remains as a misleading final zombie.
- [ ] All nine main/wall/tower tier appearances, construction/damage/repair/destruction/rebuild states, and portal states exist. Three standalone tiers show 1/2/3 independent guards; main tier 3 shows exactly two single-guard attachments.
- [ ] All corpses and ground-loot piles remain selectable while nonempty, including after particles/stains fade; looting updates visible gear; tower guards never generate bodies or items.
- [ ] The one map has all six portals and baseline content sites, day/night treatment, complete reachable terrain, explored-map representation, food states, wildlife poses, and clearing states.
- [ ] UI covers all normal, empty, waiting, disabled, invalid, conflicting, legacy, lost-access, connection, and save/config failure states. Active-game menus do not promise a pause.
- [ ] Every required sound event has an auditioned recording or generated replacement, correct gain/loop/timing, and appropriate priority. Music remains excluded and critical warnings work when muted.
- [ ] Current English text, fonts, player numbers/names, remapped controls, and long replacement strings work at supported resolutions/UI scales.
- [ ] Every configuration domain is shipped with values and documentation; references validate; zombie progression is separately editable; changed settings do not make saved legacy belongings invisible.
- [ ] Reconstructing a save or late join restores current appearances and loops without replaying old one-shot creation, destruction, purchase, or death events.
- [ ] The complete required visuals and sound remain usable during the specified four-player, approximately 100-survivor/300-zombie test and the persistent-corpse endurance scenario.

### 12.4 Reuse-first production order

1. **Export and normalize the common donor kit.** Prioritize K01–K13, K20–K27, and the AU recording candidates. Preserve native pixels, extract font bytes and sound data, resolve animation bindings, recover palettes, and create the source/output manifest. This prevents redrawing vegetation, wildlife, wood structures, common UI pieces, projectiles, crowns, and effects that already exist.
2. **Prove one complete equipped survivor and zombie.** Use K14–K18 and the local zombie references for motion/style. Finish neutral bodies, independent equipment layers, both hands, bow aim, and equipped corpse behavior before multiplying appearances across all archetypes.
3. **Generate the actual gaps.** Seven final zombie bodies and missing actions/fits; all modern apocalypse weapons/clothing/armor/bags not covered by donors; main tier 2/3 silhouettes; metal wall/tower parts and ruins; 1/2/3 guard layouts; omitted food/item icons. Reuse donor ingredients within these outputs.
4. **Assemble the map, effects, and UI.** Combine the donor kit with generated geometry/text and this game's authored layouts. Complete the eleven-slot inventory, nested/legacy views, jobs/sales/rally screens, LAN/save/outcome flows, minimap, and English messages.
5. **Audition and bind audio, then validate the complete pack.** Reuse suitable effects/ambience, synthesize remaining gaps, document configuration, and run the content/combination/crowd checks. Retire placeholders only after the integrated appearance and behavior satisfy the specification.

The largest unavoidable creation area is the **modular humanoid/equipment system and the new apocalypse building silhouettes**. The largest verified reuse opportunity is **environment art, wildlife, wooden components, portal framing, common symbols/fonts, and the recovered sound library**. This document deliberately does not invent a reuse percentage: only the sampled sprites and metadata have been inspected in detail, and final suitability depends on complete export and integration.

### 12.5 Document audit result

The completed register contains 180 requirement entries: 13 character families, 18 action/state entries, 24 items, 16 building/site entries, 11 world/environment entries, 12 effects, 27 UI entries, 30 audio categories, 27 data/support packs, and two text packs. Shared outputs may satisfy several entries; this is not a count of individual files or hand-drawn frames.

All 24 specification item definitions and all 47 acceptance scenarios have coverage. All 198 distinct numeric recovered-object locators in this document resolve in the supplied catalogs. The 30 donor groups, requirement-ID references, Markdown tables, and specification link were checked for missing entries or broken references. Remaining unchecked production work is explicitly identified through the evidence labels and completion checklist.
