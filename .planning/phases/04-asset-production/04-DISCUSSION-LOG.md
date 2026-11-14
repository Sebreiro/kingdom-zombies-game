# Phase 4: Asset production - Discussion Log

> **Audit trail only.** Do not use as input to planning, research or execution agents. Decisions are captured in [04-CONTEXT.md](04-CONTEXT.md); this log preserves the alternatives considered.

**Date:** 2026-11-14 (Europe/Paris)
**Phase:** 04-asset-production
**Areas discussed:** Animation and equipment motion; Pixel scale and visual consistency; Inspection and review workflow; Interface and sound treatment
**Implementation questions:** 32

---

## Animation and equipment motion

### Q01 - Motion style

What should the composed character animation look like? All options must support walking while attacking or using a bow, with crisp pixel art.

| Option | Description | Selected |
| --- | --- | --- |
| Pixel poses with flexible aiming (Recommended) | Walking and attacks have deliberate pixel-art poses; arms and weapons can move smoothly where aiming needs it. | ✓ |
| Stepped motion throughout | Bodies, limbs and weapons visibly move between discrete poses, including directional aiming. |  |
| Smooth limb motion | Body parts and equipment move and rotate continuously, giving the characters a fluid cutout-animation appearance. |  |

**User's choice:** Pixel poses with flexible aiming (Recommended) — Walking and attacks have deliberate pixel-art poses; arms and weapons can move smoothly where aiming needs it.

**Decision:** D-01 in the context.

### Q02 - Attack weight

When a character attacks while walking, how expressive should the body motion be? The walking cycle continues in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Clear weight and follow-through (Recommended) | Shoulders and torso join the strike; heavy bodies and weapons feel weighty, while quick attacks stay sharp. | ✓ |
| Compact upper-body motion | Keep the torso fairly steady and concentrate the action in the arms and weapon for a restrained look. |  |
| Exaggerated action | Use strong twists, leans and recoil so attacks read dramatically even in a crowd. |  |

**User's choice:** Clear weight and follow-through (Recommended) — Shoulders and torso join the strike; heavy bodies and weapons feel weighty, while quick attacks stay sharp.

**Decision:** D-02 in the context.

### Q03 - Clothing and bag movement

How much independent movement should clothing and equipment have during walking and attacks?

| Option | Description | Selected |
| --- | --- | --- |
| Subtle secondary motion (Recommended) | Small backpack bounce, coat-hem sway and belt movement; held weapons stay firmly gripped and equipment remains readable. |  |
| Mostly fixed to the body | Clothing and bags follow their attachment points closely, with little extra sway or bounce. | ✓ |
| Pronounced sway and bounce | Give coats, bags and pouches noticeable follow-through for a more lively, stylized appearance. |  |

**User's choice:** Mostly fixed to the body — Clothing and bags follow their attachment points closely, with little extra sway or bounce.

**Decision:** D-03 in the context.

### Q04 - Melee effects

How much should visual effects emphasize melee swings and impacts? Blood remains restrained, with no dismemberment.

| Option | Description | Selected |
| --- | --- | --- |
| Restrained impact accents (Recommended) | Let the body and weapon motion carry the attack, supported by small contact flashes, dust or brief subtle trails where useful. |  |
| Very minimal accents | Rely mainly on the swing and target reaction, using only the small effects needed to show contact clearly. | ✓ |
| Strong attack accents | Use more noticeable short trails and impact bursts so strikes stand out clearly in crowded scenes. |  |

**User's choice:** Very minimal accents — Rely mainly on the swing and target reaction, using only the small effects needed to show contact clearly.

**Decision:** D-04 in the context.

### Q05 - Bow aiming posture

When aiming a bow up or down, how much should the character’s body follow the aim direction?

| Option | Description | Selected |
| --- | --- | --- |
| Head and upper body follow (Recommended) | Turn the head and adjust the shoulders and torso moderately, while the legs keep their walking rhythm. |  |
| Arms do most of the work | Keep the head and torso close to their normal pose; the arms and bow carry most of the aiming motion. | ✓ |
| Strong aiming poses | Use pronounced head, shoulder and torso changes so upward and downward shots have distinctly different silhouettes. |  |

**User's choice:** Arms do most of the work — Keep the head and torso close to their normal pose; the arms and bow carry most of the aiming motion.

**Decision:** D-05 in the context.

### Q06 - Death transition

What style should the death-to-corpse transition use? Corpses keep their actual remaining equipment and remain inspectable in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Short authored collapse (Recommended) | Use deliberate falling poses suited to each body, ending in a stable, readable corpse pose. | ✓ |
| More physical collapse | Let the body bend and settle with some variation, giving falls a less uniform appearance. |  |
| Quick, simple collapse | Keep the transition brief and spend most of the detail on the final equipped corpse pose. |  |

**User's choice:** Short authored collapse (Recommended) — Use deliberate falling poses suited to each body, ending in a stable, readable corpse pose.

**Decision:** D-06 in the context.

### Q07 - Hit reaction

How pronounced should the visual reaction to a hit be? This changes the animation only; it does not add stun, knockback or action delays.

| Option | Description | Selected |
| --- | --- | --- |
| Small, clear flinch (Recommended) | A brief shoulder or torso recoil makes contact visible while movement and accepted actions continue. | ✓ |
| Very restrained reaction | Use a tiny pose change and a brief color cue, keeping the current movement almost undisturbed. |  |
| Strong visible recoil | Use a larger body reaction so hits read clearly in crowds, while preserving the existing gameplay timing. |  |

**User's choice:** Small, clear flinch (Recommended) — A brief shoulder or torso recoil makes contact visible while movement and accepted actions continue.

**Decision:** D-07 in the context.

### Q08 - Animation tooling

How should planning handle optional animation add-ons? Any paid option would need its price and compatibility explained before a purchase.

| Option | Description | Selected |
| --- | --- | --- |
| Investigate built-in Godot tools first (Recommended) | Research an approach using the existing engine tools; propose an add-on if it offers a concrete advantage. |  |
| Compare add-ons upfront | Require a comparison of built-in tools and suitable add-ons, including price, before choosing the animation approach. |  |
| Use built-in tools only | Keep the animation solution within Godot’s own tools and project code, with no paid animation dependency. | ✓ |

**User's choice:** Use built-in tools only — Keep the animation solution within Godot’s own tools and project code, with no paid animation dependency.

**Decision:** D-08 in the context.

### Q09 - Changing direction

How should a character look when changing direction? Movement responds immediately, and logical left/right hand identities stay correct in both options.

| Option | Description | Selected |
| --- | --- | --- |
| Immediate facing change (Recommended) | Face the new direction at once and continue the current stride or action coherently. | ✓ |
| Brief turning pose | Show a short transition pose as the character turns, without delaying movement or accepted actions. |  |

**User's choice:** Immediate facing change (Recommended) — Face the new direction at once and continue the current stride or action coherently.

**Decision:** D-09 in the context.

### Q10 - Starting and stopping

How should the animation move between standing and walking? Actual movement starts and stops immediately in both options.

| Option | Description | Selected |
| --- | --- | --- |
| Quick weight shift and settle (Recommended) | Use a brief pose adjustment into the walk and a short settling pose when stopping. | ✓ |
| Direct idle/walk switches | Switch straight between the idle pose and walking cycle, keeping transitions minimal. |  |

**User's choice:** Quick weight shift and settle (Recommended) — Use a brief pose adjustment into the walk and a short settling pose when stopping.

**Decision:** D-10 in the context.

### Q11 - Equipment fit

What visual fit should clothing, armor and bags have across the survivor and zombie bodies? Every required item must fit and remain readable.

| Option | Description | Selected |
| --- | --- | --- |
| Practical scavenged fit (Recommended) | Fit gear to each body with a slightly loose, utilitarian appearance and modest bulk. | ✓ |
| Close, tidy fit | Keep clothing and armor closely fitted, aligned and relatively compact. |  |
| Chunky layered fit | Emphasize overlapping garments, armor and bags with a bulkier silhouette, while preserving each body’s identity. |  |

**User's choice:** Practical scavenged fit (Recommended) — Fit gear to each body with a slightly loose, utilitarian appearance and modest bulk.

**Decision:** D-11 in the context.

### Q12 - Work and rest poses

How animated should gathering, building, carrying and fireside rest look?

| Option | Description | Selected |
| --- | --- | --- |
| Restrained, task-specific poses (Recommended) | Use clear reaching, hammering, carrying and seated poses, with only as much body movement as the activity needs. | ✓ |
| Mostly arms and props | Keep the torso and legs fairly steady during routine work; arm and tool motion carries most of the activity. |  |
| Lively full-body activity | Use larger bends, weight shifts and idle movements to make everyday camp activity more animated. |  |

**User's choice:** Restrained, task-specific poses (Recommended) — Use clear reaching, hammering, carrying and seated poses, with only as much body movement as the activity needs.

**Decision:** D-12 in the context.

## Pixel scale and visual consistency

### Q13 - Logical pixel detail

What level of pixel detail should we target when matching the existing examples with recovered art? The asset plan requires an assembled scale test before full production.

| Option | Description | Selected |
| --- | --- | --- |
| Choose after an assembled comparison (Recommended) | Show a survivor, zombie, wall, tower and base at two candidate pixel densities before locking the production scale. |  |
| Favor the recovered sprites’ simpler detail | Keep a low-resolution look close to the donor art and simplify the larger examples to match. | ✓ |
| Favor the examples’ richer detail | Preserve more of the zombie and building examples’ detail, adapting recovered parts to the same visual density. |  |

**User's choice:** Favor the recovered sprites’ simpler detail — Keep a low-resolution look close to the donor art and simplify the larger examples to match.

**Decision:** D-13 in the context.

**Notes:** Exact logical pixel density and sprite dimensions remain to be selected through the asset plan's assembled scale test; no numeric density was chosen.

### Q14 - Day and night mood

What overall color and lighting mood should the game art use? Nighttime characters, equipment and important states must remain readable.

| Option | Description | Selected |
| --- | --- | --- |
| Natural days, atmospheric nights (Recommended) | Use earthy scenery and warm wood in daylight, with cooler, darker nights and restrained warm firelight. | ✓ |
| Muted and bleak throughout | Keep even daylight more desaturated and subdued, with a colder survival-horror mood. |  |
| Brighter and more colorful | Use clearer, stronger colors by day and more colorful night contrasts. |  |

**User's choice:** Natural days, atmospheric nights (Recommended) — Use earthy scenery and warm wood in daylight, with cooler, darker nights and restrained warm firelight.

**Decision:** D-14 in the context.

### Q15 - Character proportions

At the simpler pixel scale, how should character proportions balance a natural look with readable faces, hands and equipment?

| Option | Description | Selected |
| --- | --- | --- |
| Slightly emphasized details (Recommended) | Keep broadly natural proportions, with small exaggerations where faces, grips or key equipment need clarity. |  |
| Close to natural proportions | Keep heads, hands and equipment sizes restrained, using pose and contrast to make them readable. | ✓ |
| Chunkier, more stylized proportions | Use noticeably larger heads, hands and equipment details for a bolder pixel-art look. |  |

**User's choice:** Close to natural proportions — Keep heads, hands and equipment sizes restrained, using pose and contrast to make them readable.

**Decision:** D-15 in the context.

### Q16 - Structure construction style

What construction style should buildings and defenses have? The required wood, reinforced-wood and metal tier progression still applies.

| Option | Description | Selected |
| --- | --- | --- |
| Practical camp construction (Recommended) | Rough timber, straightforward bracing and functional reinforcement, with clear tier silhouettes. | ✓ |
| Neater, deliberately built structures | More regular planks, aligned supports and orderly reinforcement. |  |
| Heavily patched scavenged structures | More irregular salvaged parts and visibly pieced-together construction. |  |

**User's choice:** Practical camp construction (Recommended) — Rough timber, straightforward bracing and functional reinforcement, with clear tier silhouettes.

**Decision:** D-16 in the context.

### Q17 - Background density

How dense should the decorative background scenery be, using the required environment art?

| Option | Description | Selected |
| --- | --- | --- |
| Layered but unobtrusive (Recommended) | Use distant scenery and parallax for depth, with enough quiet space to keep characters, equipment and sites clear. | ✓ |
| Rich and dense | Favor fuller, overlapping scenery and a more enclosed forest atmosphere. |  |
| Sparse and open | Use fewer decorative layers and larger quiet areas for a simpler, more exposed landscape. |  |

**User's choice:** Layered but unobtrusive (Recommended) — Use distant scenery and parallax for depth, with enough quiet space to keep characters, equipment and sites clear.

**Decision:** D-17 in the context.

### Q18 - Normal character edges

How should normal, unselected characters separate visually from the scenery? Selection highlights are a separate required state.

| Option | Description | Selected |
| --- | --- | --- |
| Subtle dark edges where needed (Recommended) | Strengthen some outer pixel edges for clarity while keeping the donor-style shading. | ✓ |
| Color and brightness separation | Rely on palette, lighting and quieter backgrounds rather than adding outlines. |  |
| Consistent thin outline | Give foreground characters a uniform thin dark contour for stronger separation. |  |

**User's choice:** Subtle dark edges where needed (Recommended) — Strengthen some outer pixel edges for clarity while keeping the donor-style shading.

**Decision:** D-18 in the context.

### Q19 - Structure damage

How should standing, damaged buildings and defenses show deterioration? Their material, tier and functional state must stay clear.

| Option | Description | Selected |
| --- | --- | --- |
| Localized material damage (Recommended) | Use cracks, splintered timber, bent reinforcement and small damaged areas, with severity increasing visibly. | ✓ |
| Mostly surface wear | Use restrained texture and color changes, keeping the overall outline nearly unchanged. |  |
| Stronger silhouette damage | Show more broken or missing small sections before destruction, while keeping the structure’s function and tier readable. |  |

**User's choice:** Localized material damage (Recommended) — Use cracks, splintered timber, bent reinforcement and small damaged areas, with severity increasing visibly.

**Decision:** D-19 in the context.

### Q20 - Inventory icons

How should inventory icons relate to the worn, held or world appearance of each item? All 24 items must remain identifiable.

| Option | Description | Selected |
| --- | --- | --- |
| Same artwork, adjusted for readability (Recommended) | Derive icons from the master item art where it reads well, strengthening or simplifying details when needed. |  |
| Consistently simplified icons | Give every item a reduced icon version focused on its strongest shapes and identifying features. |  |
| More detailed close-up icons | Show richer material detail in inventory while keeping colors and identity consistent with the world art. |  |
| Free-text response | Creator supplied their own instruction below. | ✓ |

**User's choice:** Don't make multiple icons for the inventory. There will be durability numbers or something. so not care right now

**Decision:** D-20 in the context.

**Notes:** Reflected back as one canonical inventory icon per item, with no separate durability icon variants. No exact durability indicator was selected; its form remains a planning detail. This does not remove required condition/broken-state presentation or any of the 24 item identities.

### Q21 - Zombie body colors

How much should zombie archetypes differ in body color? Their equipped items retain their actual appearance.

| Option | Description | Selected |
| --- | --- | --- |
| Shared palette with subtle differences (Recommended) | Use related, muted undead skin tones, with small color differences supporting each silhouette. | ✓ |
| Clearly distinct body palettes | Give each archetype a more recognizable body-color treatment, within the subdued overall style. |  |
| Nearly identical body colors | Use a very consistent undead palette and rely mainly on body shape, posture and motion to distinguish types. |  |

**User's choice:** Shared palette with subtle differences (Recommended) — Use related, muted undead skin tones, with small color differences supporting each silhouette.

**Decision:** D-21 in the context.

### Q22 - Portal and creature-effect colors

What color direction should portals and creature effects use?

| Option | Description | Selected |
| --- | --- | --- |
| Donor-inspired portal colors (Recommended) | Keep suitable recovered portal colors as the starting point, with separate, restrained sickly colors for spit and cosmetic acid. | ✓ |
| Blue/violet portals, green creature effects | Use an explicit cool supernatural portal palette contrasted with muted green creature effects. |  |
| Green-led supernatural palette | Make subdued green the main accent across portals and creature effects, distinguishing them through shape and motion. |  |

**User's choice:** Donor-inspired portal colors (Recommended) — Keep suitable recovered portal colors as the starting point, with separate, restrained sickly colors for spit and cosmetic acid.

**Decision:** D-22 in the context.

**Notes:** No exact hue or numeric palette was chosen; this selects the donor-inspired route. Cosmetic acid remains non-damaging under the locked specification.

### Q23 - Ground contact

What grounding treatment should characters and small world objects use?

| Option | Description | Selected |
| --- | --- | --- |
| Small pixel contact shadows (Recommended) | Use a quiet, simple dark pixel shape beneath them to establish contact with the ground. | ✓ |
| Grounding mainly in the artwork | Rely on the feet, object bases and nearby ground shading, without separate shadow shapes. |  |
| Broader pixel shadows | Use more noticeable flat pixel shadows to give objects a stronger sense of weight. |  |

**User's choice:** Small pixel contact shadows (Recommended) — Use a quiet, simple dark pixel shape beneath them to establish contact with the ground.

**Decision:** D-23 in the context.

### Q24 - Selected-object treatment

What visual treatment should selected corpses, loot piles and other inspectable objects use? This defines the selected-state artwork, not later gameplay interactions.

| Option | Description | Selected |
| --- | --- | --- |
| Thin outline and small marker (Recommended) | Use a steady contrasting pixel outline with a modest marker, keeping the object itself clear. | ✓ |
| Ground brackets and label | Mark the object’s ground position with pixel brackets and identify it with a nearby label. |  |
| Mild tint and small marker | Slightly brighten or tint the object and add a modest marker, with no outline. |  |

**User's choice:** Thin outline and small marker (Recommended) — Use a steady contrasting pixel outline with a modest marker, keeping the object itself clear.

**Decision:** D-24 in the context.

## Inspection and review workflow

### Q25 - Inspection navigation

How would you like to navigate the Windows inspection build? It must expose the complete resource pack and its inspection scenes.

| Option | Description | Selected |
| --- | --- | --- |
| One hub organized by category (Recommended) | Browse characters, items, buildings/world, UI, data/text and audio from a common navigation panel. | ✓ |
| Resource-register browser first | Start from the 180 obligation IDs and their status, then open the corresponding previews. |  |
| Separate focused inspection scenes | Use a simple launcher to open each inspection area as its own scene. |  |

**User's choice:** One hub organized by category (Recommended) — Browse characters, items, buildings/world, UI, data/text and audio from a common navigation panel.

**Decision:** D-25 in the context.

### Q26 - Character preview layout

How should the live character inspection page be arranged? Body, equipment, movement and attack controls remain available in every option.

| Option | Description | Selected |
| --- | --- | --- |
| One large live preview (Recommended) | Give one character most of the space, with its controls alongside for close inspection. | ✓ |
| Two previews side by side | Compare two bodies, equipment combinations or action states while editing the selected preview. |  |
| Grid of live previews | Show several combinations at once, with controls for the selected character. |  |

**User's choice:** One large live preview (Recommended) — Give one character most of the space, with its controls alongside for close inspection.

**Decision:** D-26 in the context.

### Q27 - Playback controls

What animation playback controls should the character inspector provide?

| Option | Description | Selected |
| --- | --- | --- |
| Pause, speed and single-step controls (Recommended) | Play or pause, slow the animation down, and advance in small steps to inspect grips, layering and timing. | ✓ |
| Pause and speed controls | Inspect animations at different speeds without single-step controls. |  |
| Basic play and restart | Keep playback simple, using action and state selectors to inspect the required appearances. |  |

**User's choice:** Pause, speed and single-step controls (Recommended) — Play or pause, slow the animation down, and advance in small steps to inspect grips, layering and timing.

**Decision:** D-27 in the context.

### Q28 - Resource details

How much resource information should appear beside a preview? Missing or invalid resource errors must stay visible in every option.

| Option | Description | Selected |
| --- | --- | --- |
| Compact summary, expandable details (Recommended) | Keep the resource ID, name and status visible; expand provenance, dimensions, timings and other metadata when needed. | ✓ |
| Full details always visible | Show the production and source information alongside the preview throughout inspection. |  |
| Artwork first, details in a separate view | Keep only the resource ID and status near the artwork, with a dedicated details view for the full record. |  |

**User's choice:** Compact summary, expandable details (Recommended) — Keep the resource ID, name and status visible; expand provenance, dimensions, timings and other metadata when needed.

**Decision:** D-28 in the context.

## Interface and sound treatment

### Q29 - UI materials

What visual treatment should the game’s menus and panels use? These are the reusable UI layouts and state examples for the asset pack.

| Option | Description | Selected |
| --- | --- | --- |
| Wood frames, quiet panel interiors (Recommended) | Reuse suitable wooden buttons and borders, with simple dark interiors that keep text and item grids clear. | ✓ |
| Rustic wood throughout | Use more visible wood texture and material detail across the panels themselves. |  |
| Minimal dark panels with wood accents | Keep most surfaces plain and use recovered wood details sparingly on key controls and edges. |  |

**User's choice:** Wood frames, quiet panel interiors (Recommended) — Reuse suitable wooden buttons and borders, with simple dark interiors that keep text and item grids clear.

**Decision:** D-29 in the context.

### Q30 - Typography direction

What typography style should the UI use? Exact fonts must still pass the required readability, scaling and expanded-text checks.

| Option | Description | Selected |
| --- | --- | --- |
| Readable body text, pixel-style accents (Recommended) | Use a clear sans-serif face for most text and numbers, with pixel-style type for selected headings or accents. | ✓ |
| Pixel-style type throughout | Use suitable pixel fonts across headings, labels and body text, with readability taking priority. |  |
| Clean sans-serif throughout | Use clear, simple typography for headings and body text, leaving the pixel-art character to the surrounding UI artwork. |  |

**User's choice:** Readable body text, pixel-style accents (Recommended) — Use a clear sans-serif face for most text and numbers, with pixel-style type for selected headings or accents.

**Decision:** D-30 in the context.

**Notes:** A typography direction was selected, not a specific extracted font, size or fallback implementation.

### Q31 - Sound-effects character

What overall character should the sound effects have? Suitable recovered recordings are still the first choice, followed by audition and adaptation.

| Option | Description | Selected |
| --- | --- | --- |
| Grounded and restrained (Recommended) | Natural footfalls, clear wood/metal contacts and subdued creature sounds, with short, controlled effects. |  |
| Punchier and more game-like | Sharper swings, stronger impacts and more noticeable confirmations. | ✓ |
| Harsher survival-horror tone | Rougher creature textures and more unsettling impacts, while keeping important actions distinguishable. |  |

**User's choice:** Punchier and more game-like — Sharper swings, stronger impacts and more noticeable confirmations.

**Decision:** D-31 in the context.

**Notes:** The creator selected punchier audio despite choosing very minimal visual impact effects. Do not replace this with the unselected restrained-audio recommendation.

### Q32 - Ambience balance

How prominent should day/night and local ambience be beside the punchier sound effects?

| Option | Description | Selected |
| --- | --- | --- |
| Quiet ambience, clear actions (Recommended) | Keep forest, wind and fire beds subdued so actions and important cues stand out. | ✓ |
| Balanced atmosphere and action | Give ambience a clearly audible presence while keeping actions and warnings distinct. |  |
| Prominent atmosphere between actions | Let environmental sounds fill quiet moments more strongly, with action sounds taking the foreground when needed. |  |

**User's choice:** Quiet ambience, clear actions (Recommended) — Keep forest, wind and fire beds subdued so actions and important cues stand out.

**Decision:** D-32 in the context.

## Discussion flow and final readiness

Routing prompts are summarized here; implementation questions and their presented alternatives are recorded above.

| Step | Area | Prompt summary | Options | Reply |
| --- | --- | --- | --- | --- |
| 1 | Phase selection | Which Phase 4 choices would you like to discuss? | Animation and equipment motion; Pixel scale and visual consistency; Inspection and review workflow; Interface and sound treatment | all |
| 2 | Animation and equipment motion | More animation questions, or move to pixel scale? Remaining: pixel scale, inspection workflow, interface and sound. | Next area; More animation questions | More animation questions |
| 3 | Animation and equipment motion | More animation discussion, or move to pixel scale? Remaining: pixel scale, inspection workflow, interface and sound. | Next area; More animation questions | More animation questions |
| 4 | Animation and equipment motion | Which animation detail would you like to explore next? | Movement, turns and transitions; Body and equipment composition; Work, idle and special actions | all |
| 5 | Animation and equipment motion | Continue animation discussion, or move to pixel scale and visual consistency? Remaining: pixel scale, inspection workflow, interface and sound. | Next area; More animation questions | Next area — Save the animation decisions and continue to matching characters, buildings and recovered sprites. |
| 6 | Pixel scale and visual consistency | More discussion of pixel scale and visual consistency, or move to inspection workflow? Interface and sound follows. | Next area; More visual questions | More visual questions |
| 7 | Pixel scale and visual consistency | More visual discussion, or move to inspection workflow? Interface and sound is the final area after that. | Next area; More visual questions | More visual questions |
| 8 | Pixel scale and visual consistency | Which visual details would you like to explore next? | Character and zombie color identity; Portal and effect colors; Grounding and object emphasis | all |
| 9 | Pixel scale and visual consistency | More visual discussion, or move to inspection workflow? Interface and sound remains after that. | Next area; More visual questions | Next area — Save the visual decisions and continue to browsing and inspecting the pack. |
| 10 | Inspection and review workflow | More inspection-workflow discussion, or move to interface and sound treatment, the final selected area? | Next area; More inspection questions | Next area — Save the inspection decisions and continue to the game’s UI and sound presentation. |
| 11 | Interface and sound treatment | We’ve discussed animation, visual consistency, inspection workflow, and interface/sound treatment. Are you ready for the Phase 4 context, or is there more to clarify? | I’m ready for context (Recommended) — Save the decisions in Phase 4’s context and discussion log for research and planning.; More interface and sound questions — Explore another UI or audio detail before finishing.; Explore another gray area — Revisit another topic or name a remaining concern. | I’m ready for context (Recommended) — Save the decisions in Phase 4’s context and discussion log for research and planning. |

## Agent's Discretion

The exact durability indicator remains a planning detail; "numbers or something" was not a final design selection. No other answer explicitly said "you decide." Exact density, native rig/composition implementation, fitting metadata, file formats/layout, source selections, actual fonts, sound variants and mix values remain research/planning choices within the locked specification and the recorded decisions.

## Deferred Ideas

None. No new capability or backlog item was requested. Required condition presentation and all later gameplay/release obligations retain their existing scope and owners.

---

*Creator explicitly selected saving the context and discussion log after all four areas were discussed.*
