# KingdomZombies — Version 0.1 specification

**Status:** Scope specified, with a proposed baseline for playtesting.

**Last updated:** 2026-11-13.

**Deliverable:** This single document, specs/0.1/initial-spec.md.

This specification describes the game independently of engine, programming language, networking library, or implementation technology. Confirmed creator decisions define the feature scope. Tables marked **Proposed baseline** supply initial designer-editable values; they are not claims of tested balance. Supporting decisions not individually selected during discovery are identified in section 16.

The intended result is a complete playable game, including its beginning, progression, victory, defeat, saving, multiplayer, interface, art, and sound. Development schedule and learning milestones are outside this specification.

**Navigation:**

- [Scope](#1-product-and-scope), [run lifecycle](#2-run-lifecycle-leadership-and-outcomes), and [world/time](#3-world-time-exploration-and-camp-boundaries).
- [Characters/combat](#4-survivors-movement-recovery-and-combat), [inventory](#5-inventory-equipment-items-and-corpses), and [economy/recruitment](#6-economy-food-and-recruitment).
- [NPCs and armies](#7-npc-roles-equipment-work-and-army-orders), [buildings](#8-main-building-walls-towers-and-paid-work), and [zombies/portals](#9-zombies-loot-portals-and-escalating-attacks).
- [Multiplayer](#10-multiplayer-and-session-behavior), [saving](#11-saving-loading-and-configuration-changes), and [configuration](#12-designer-configuration).
- [Interface/presentation](#13-interface-controls-art-sound-and-translation), [finished criteria](#14-definition-of-a-finished-01-release), [references](#15-supplied-references-and-inspected-material), and [proposed decisions](#16-proposed-supporting-decisions-and-balance-review).

## 1. Product and scope

KingdomZombies is a side-view 2D survival and camp-management game inspired by Kingdom Two Crowns. Players travel left and right, recruit survivors, manage real inventories, build defenses, survive zombie attacks, and destroy the portals threatening one shared camp.

| Subject | Required 0.1 behavior |
| --- | --- |
| Platforms and input | Windows and Linux PCs; keyboard and mouse; one computer per player. |
| Players | Solo or 1–4 cooperative LAN players, including the host. Equal camp-management permissions; no PvP or friendly damage. |
| World | One handcrafted map, with fixed terrain, building sites, and six portals: three on each side of camp. |
| Exploration | Active portals block players and camp NPCs from travelling farther. Destroying a portal opens the route beyond it. Zombies can pass through their own portals. |
| Core progression | Recruit and equip survivors, earn money, clear trees, expand walls, upgrade the main building and defenses, and assault portals. |
| Victory | Destroy all six portals. Victory is awarded immediately, even with zombies still alive. Players may continue or return to the menu. |
| Defeat | Main building destroyed, or no living controlled or eligible recruited survivor remains. |
| Intended run | Approximately 2–3 hours of active play under the default balance; saving and resuming supported. This is a playtest target, not a timer. |
| Scale | Roughly 60–100 mature camp survivors and up to about 300 active zombies. |
| Performance | 30 FPS at 1920×1080 on the creator's PC at the specified four-player population/battle scale. |
| Presentation | Pixel-art side view guided by the supplied references; visible equipped clothing, armor, and weapons. |
| Language | English content in 0.1, prepared for future translations. |
| Audio | Gameplay sound effects and ambience. No music in 0.1. |
| Onboarding | No tutorial. Ordinary labels, tooltips, feedback, and a controls reference are included. |
| Future multiplayer | A replaceable multiplayer-service boundary must allow later Steam and Epic support without rewriting gameplay rules. Their integration is outside 0.1. |

### 1.1 The playable loop

1. During daylight, sell surplus food, withdraw money, recruit, assign jobs, equip survivors, build or repair, and collect loot.
2. Expand toward the next portal by paying for tree clearing and construction at marked sites.
3. Use the player characters and paid warrior squads to attack portals. Attacking can provoke defenders; destroying one provokes retaliation.
4. At night, civilians withdraw into safe camp territory while warriors, towers, and players defend.
5. Recover belongings from casualties, replenish equipment from zombie loot, and repeat at increasing difficulty.
6. Destroy the final portal to win. In continued play, remaining zombies stay dangerous until killed; no portals return.

### 1.2 Explicit exclusions

- Additional islands, travel between maps, procedural terrain, and competing camps.
- Homeless survivors, external recruitment camps, and recruitment encounters outside the main building.
- Controller support, split-screen, online matchmaking, platform accounts, and host migration.
- Firearms, crossbows, shields, parrying, combat rolls, and mounts.
- Hunger, thirst, infection, detailed injuries, survivor aging, and reproduction.
- Crafting, raw material harvesting, clothing-to-rag conversion, cooking, and equipment shops.
- Equipment repair, recoverable fired arrows, and gameplay acid hazards.
- Tutorial sequences, music, and additional translated language content.
- An in-game balance editor and live configuration reload.
- Character classes, experience levels, skill trees, and other progression beyond the specified camp, equipment, and wave systems.

## 2. Run lifecycle, leadership, and outcomes

### 2.1 Starting a run

**Confirmed defaults:** eight recruited survivors total, ten-minute days, five-minute nights, one opening night without scheduled waves, and escalation every three days.

A new run starts at dawn on day 1 with the tier-1 main building, an empty construction landscape at the marked sites, and six living portals. Each starting player takes a different survivor; the rest are unassigned. Starting players do not add survivors or duplicate supplies. Every starting survivor has the same basic outfit described below; further starting supplies are in the shared stash.

**Proposed baseline — initial stash, identical for 1–4 players:**

| Item | Quantity | Initial state |
| --- | ---: | --- |
| Money | 200 | Physical currency, split into legal stacks. |
| Wooden club | 4 | Full durability. |
| Bow | 4 | Full durability. |
| Arrows | 240 | Eight stacks of 30. |
| Small backpack | 4 | Empty. |
| Belt with pouches | 4 | Empty. |
| Berries | 20 | Two stacks of 10. |
| Fruit | 10 | Two stacks of 5. |
| Meat | 10 | Two stacks of 5. |

Starting survivors and purchased recruits wear a basic shirt, trousers, and boots. The shirt and trousers supply small pockets. They carry no personal money, food, weapon, ammunition, armor, or backpack unless equipped from the stash. Purchased recruits do not replenish the initial stash.

### 2.2 Leadership and death

- Each player controls one ordinary recruited survivor who can fight, carry items, and die.
- A cosmetic crown identifies player control alongside normal headgear. It occupies no equipment slot and never becomes loot.
- On death, the character's actual belongings remain on the corpse. Nothing is copied to the successor.
- Control automatically transfers to a random living recruited survivor not controlled by another player. Equipped gear, current health/stamina, location, and carried items are preserved.
- Without a successor, the player spectates while teammates continue. A newly available eligible survivor restores control.
- A character cannot be assigned to two players, including after simultaneous deaths or purchases.
- Unbuilt tower guards, built-in tower archers, wildlife, corpses, and the option to buy a survivor are not eligible successors.
- Succession does not create a recruit, heal someone, restore ammunition, teleport the successor, or remove that survivor's actual gear.

**Proposed supporting rules:** automatic transfer takes 1 second; the camera then follows the successor. The previous NPC job is suspended during control and remembered. Waiting spectators receive available survivors in waiting order, with random selection among eligible survivors for each assignment. A reconnecting player's available previous survivor takes priority for that reconnecting player.

### 2.3 Victory, defeat, and continued play

Destruction of the last portal awards victory immediately. It does not wait for the remaining zombies to die, and it does not remove them. Remaining zombies no longer retreat at dawn and remain until killed. Pending spawns from destroyed portals are canceled; existing enemies and corpses remain.

Main-building destruction immediately defeats the camp. Independently, losing all living controlled and eligible recruited survivors defeats the camp. Money in the stash or unused recruitment capacity does not postpone defeat.

**Proposed supporting rules:**

- A victory overlay offers Continue and Main menu while the world continues running. It also warns that surviving zombies remain dangerous.
- A guest choosing Main menu leaves the session; the host choosing it saves and closes the session for everyone.
- Defeat ends active play and offers loading an earlier save, starting a new run, or returning to the menu. There is no paid revival or resurrection.
- If victory and defeat conditions first become true in the same resolved gameplay moment, defeat takes precedence.
- A victory already awarded is retained in the run record. A subsequent camp defeat during continued play ends that continuation without erasing the earlier victory.
- Loading an earlier save is allowed; this is not an ironman mode.

## 3. World, time, exploration, and camp boundaries

### 3.1 Handcrafted map

The terrain, sites, plants, and portal positions are authored content. Loot, wildlife placement within authored zones, and attack composition vary between runs. The map has no mandatory platforming gaps for NPCs. The player's jump cannot bypass an active portal.

Distances below use **u**, a design unit approximately equal to a standing survivor's body width. This unit does not prescribe a pixel resolution or engine measurement.

**Proposed baseline — mirrored left/right layout:**

| Feature | Distance from the main building on each side |
| --- | --- |
| Map edge | 320 u |
| Portals, nearest to farthest | 100 / 190 / 280 u |
| Wall sites | 25 / 75 / 155 / 245 u |
| Standalone tower sites | 21 / 71 / 151 / 241 u |
| Tree-clearing groups unlocking the outer site pairs | 50 / 125 / 215 u |
| Berry plants | 10 and 50 u |
| Fruit plant | 16 u |
| Wildlife zones | Centered at 45 / 130 / 230 u, each 15 u wide |

This provides eight wall sites and eight standalone tower sites in total. The first pair on each side is available without clearing. Each farther pair requires its associated trees cleared and all intervening portals destroyed. Forest visuals do not obstruct horizontal walking; their clearing gates construction, creates no wood, and does not remove food plants.

A portal is an uncrossable boundary for survivors while alive, regardless of jump height. Zombies can cross active portals belonging to their faction. A destroyed portal leaves a permanently passable site.

### 3.2 Time and daily events

| Setting | Confirmed default |
| --- | ---: |
| Daylight | 600 seconds |
| Night | 300 seconds |
| Opening scheduled-wave-free days | 1 |
| Escalation interval | 3 days |
| Regular waves | One per surviving portal per eligible night |

**Proposed boundary convention:** days 1–3 are stage 0; day 4 begins stage 1; day 7 begins stage 2; day 10 begins stage 3. Stage changes occur at dawn. The first scheduled attack is night 2. Opening grace never suppresses provoked defenders or retaliation.

At dawn, perform each daily event once: advance the day, update the stage, refill plants, introduce the day's wildlife batch, order eligible zombies to retreat, and autosave. Loading or late joining must not repeat those events.

The world never pauses during active play, including solo play, inventory management, save menus, camp menus, and the victory overlay. Startup/loading and a finished defeat are not active gameplay.

### 3.3 Safe territory and retreat — proposed supporting rules

- Each side's outermost completed, living wall defines that side's camp border. A tower alone does not expand the border.
- Without a living wall on a side, the main-building refuge extends 12 u in that direction.
- Losing an outer wall moves that border back to the next surviving inner wall, or to the refuge if none remains. Old walls remain useful.
- A location is safe for civilian work only if it is inside the current borders and no zombie is within 12 u. Being inside a wall is insufficient when enemies have breached it.
- At nightfall, civilian exterior work stops. NPCs retreat toward safe interior positions and may perform safe interior tasks.
- On detecting daytime danger, civilian workers, gatherers, hunters, and loot carriers abandon the task and flee. They fight only if escape is blocked, then resume fleeing when a route opens.
- Warriors retain their combat role, including while hauling during otherwise idle time.
- Retreat does not teleport a character, delete cargo, or make it invulnerable.
- Camp members pass through camp walls; zombies are blocked by them. Allied characters do not obstruct each other's travel or arrows.

### 3.4 Exploration and minimap

Each player has an independent camera following their survivor. The minimap reveals explored areas and displays known portals, defenses, and teammates.

**Proposed supporting rules:** exploration is shared by the camp and revealed within 24 u of any living camp survivor. Previously discovered terrain remains known. The minimap does not expose undiscovered portals or provide live enemy positions through unexplored terrain. Teammates are identifiable by color and player number. Exploration, known portal destruction, and known defenses persist through saves and are included for late joiners.

## 4. Survivors, movement, recovery, and combat

### 4.1 Character attributes — proposed baseline

| Setting | Value / behavior |
| --- | --- |
| Survivor maximum health / stamina | 100 / 100 |
| Walk / sprint speed | 3 / 5 u per second |
| Jump | 1.5 u maximum height; 0.8-second airborne duration on level ground |
| Air movement | 75% of ground walk control |
| Sprint stamina cost | 10 per second |
| Jump stamina cost | 12 per jump |
| Natural stamina regeneration | 14 per second after 0.5 seconds without a stamina-spending action |
| Safe-camp health regeneration | 2 per second after 5 seconds meeting the safe-rest conditions |
| Safe-rest conditions | Inside camp, no enemy within 12 u, and not sprinting, jumping, or attacking; ordinary walking is allowed |
| Food-use cooldown | 2 seconds; food applies its recovery immediately |
| Interaction distance | Main building 4 u; NPCs, corpses, sites, and rally points 3 u |
| Soft / severe carried-load thresholds | 20 / 35 kg |
| Load effect | Between thresholds, walking/sprinting speed declines to 55% and action stamina cost rises to 150% |
| At or above severe load | No sprint or jump; walking remains possible at the minimum speed |
| Empty-corpse removal delay | 180 seconds after the inventory becomes completely empty |

Health does not naturally regenerate outside the safe-camp conditions. Stamina can recover outside camp and during fights; combat alone does not suppress it. A spending action interrupts regeneration for its configured delay. Actions require sufficient stamina; walking never requires stamina.

Weight includes equipped items and every nested item exactly once. Weight never substitutes for grid capacity. Extremely heavy characters can still walk. Health and stamina are clamped to their maxima when food is consumed.

### 4.2 Combat rules

- Melee attacks strike in the facing direction. Bows follow the mouse and fire manually.
- Empty hands permit a fist attack. A single melee weapon produces one weapon attack. With two one-handed weapons, one input swings both together.
- A bow occupies one hand; only an arrow stack may occupy the other hand.
- Players manually refill the arrow-hand stack. Carried reserve arrows do not automatically feed a player's bow.
- Ordinary NPCs and zombies also consume actual arrows. Their automatic handling may transfer reserve stacks into the arrow hand; it creates no ammunition.
- When an ordinary NPC runs out of arrows or breaks a weapon, it uses another usable carried weapon or fists during the immediate fight, then visits the stash to restock when safe.
- Zombies without usable ammunition/weapons fall back to usable carried weapons or unarmed attacks; they cannot use the camp stash.
- Fired arrows are permanently spent. They cannot be recovered from the ground, a target, or a corpse. Tower shots cannot generate loot.
- Friendly attacks cause no allied damage. Friendly arrows pass through allies and camp defenses. Enemy arrows strike and damage the defense blocking their path.
- Spit hits the first survivor or defense in its path and stops. It has no area damage or lingering effect.
- Bloater death acid is cosmetic, including stains on walls and ground. It causes no damage, status effect, slowdown, durability loss, or movement obstruction.

**Proposed supporting rules:** melee hits one eligible target per weapon swing; both weapons may hit the same target. A dual attack uses the slower weapon's interval and the sum of both stamina costs. Armor applies to each hit separately. Arrows and spit stop at solid terrain. Zombies ignore their allies with attacks. At most six melee zombies can strike one structure and three can strike one survivor at the same time; other melee attackers approach or wait for a position.

### 4.3 Attacks and protection — proposed baseline

| Attack / item | Damage | Interval | Reach / projectile | Stamina | Durability |
| --- | ---: | ---: | --- | ---: | --- |
| Survivor fists | 8 | 0.7 s | 1.0 u melee | 3 | Not an item |
| Knife | 10 | 0.5 s | 1.1 u melee | 4 | 120 |
| Wooden club | 18 | 0.9 s | 1.4 u melee | 7 | 150 |
| Machete | 26 | 0.75 s | 1.5 u melee | 8 | 180 |
| Metal bat | 32 | 1.1 s | 1.6 u melee | 10 | 220 |
| Bow | 22 | 1.2 s | Arrow speed 25 u/s; lifetime 2 s | 6 | 160 |
| Spitter natural spit | 12 | 3 s | Range 16 u; speed 8 u/s; lifetime 3 s | 6 | Not an item |

Melee weapons lose one durability per successful weapon strike; bows lose one per shot. A dual attack wears each weapon that successfully hits. No critical hits, headshots, penetration, or bleeding are required in 0.1.

| Armor | Damage reduction while usable | Maximum durability | Wear |
| --- | ---: | ---: | --- |
| Improvised body armor | 20% | 100 | 20% of incoming raw damage |
| Plated body armor | 40% | 200 | 20% of incoming raw damage |
| Protective helmet | 10% | 100 | 10% of incoming raw damage |

Protection combines multiplicatively, capped at 60% total reduction. Armor wear is calculated from the same raw hit, without increasing the damage dealt. Zero-durability weapons cannot make weapon attacks; zero-durability armor provides no protection. Broken items remain real items. Ordinary clothes, bags, and belts do not wear out in this baseline, so losing durability cannot delete a storage grid. Equipment cannot be repaired.

## 5. Inventory, equipment, items, and corpses

### 5.1 Common inventory contract

Players, ordinary recruited NPCs, and zombies use the same item, grid, equipment, weight, and durability rules. Wildlife has loot inventories but no human equipment slots. Built-in tower archers are structural components and are the explicit exception to character inventories.

- Items have persistent identities and rectangular footprints. Compatible items stack and can be split; eligible items rotate 90 degrees.
- Character capacity is finite. Worn clothing pockets, belts, pouches, and backpacks provide storage.
- Filled bags may be placed inside other bags if they fit, up to a configurable nesting depth.
- An item has exactly one owner/location. It cannot be both equipped and stored, nor appear in two inventories.
- A container cannot contain itself directly or indirectly.
- Opening a permitted inventory reveals contents immediately. Transfers and equipping have no search delay or action timer.
- Dragging previews a move; the actual transfer happens only when a valid destination accepts it. Closing a window or losing access cannot discard the dragged item.
- Moving or dropping a storage-bearing item also moves its contents. Unequipping clothing does not leave its pockets behind.
- An invalid placement, split, equipment combination, purchase, or conflicting shared transaction fails clearly without item loss, duplication, or a charge.
- Nearby players may inspect living recruited NPCs, transfer items, and change equipment. This does not freeze the NPC or lock its gear against later automatic upgrades.
- Other player-controlled inventories are private to their controllers. Players share through the stash or intentional ground drops; a separate trade window is not required.
- Dropped items enter a nearby loot pile using the same persistence and transfer rules. They are not automatically destroyed.
- Stash pages, corpse contents, equipped items, and nested storage preserve item identity and current quantities/condition through death and saving.

### 5.2 Slots and storage — proposed baseline

| Location | Rule / capacity |
| --- | --- |
| Headwear | One cap or helmet |
| Face | One face covering |
| Torso clothing | One shirt or coat, including its pocket grid |
| Legs | One pair of trousers, including its pocket grid |
| Body armor | One armor item, separate from clothing |
| Gloves / boots | One pair in each slot |
| Backpack | One backpack, retaining its contents |
| Belt | One belt with pouch storage |
| Left / right hand | One compatible held item each; bow permits only arrows opposite it |
| Quick slots | Four references to food stacks in carried storage; no duplicate storage or independent supply |
| Stash page | 10×10 cells; unlimited pages created as needed |
| Maximum nested containers | Three container levels; a worn container or container directly on a stash page is level 1 |

Pocket grids are provided by the worn items, not by an invisible unlimited personal inventory. With a starter shirt and trousers, a survivor has a 2×1 shirt pocket and a 2×2 trouser pocket. Storage inside an unequipped garment still exists, but the garment must itself have a valid inventory location.

All equipped wearables and weapons must be represented on the player, NPC, or zombie. Normal visual layering can cover underclothes; inventory changes must update the world appearance. The crown remains visible with normal headgear. A bag merely carried inside another bag is not shown as an extra worn backpack.

Quick-slot references follow the actual stack. Removing it from the character, consuming it, or losing it invalidates the reference. Another similar stack is not silently substituted. NPCs use their automatic food rules instead of player quick slots.

### 5.3 Required item roster — proposed baseline

Dimensions are width×height in cells; weights are per individual item. Clothing pairs are single items. Every footprint below may rotate. Identical consumables stack only with matching definition and state; equipment and filled containers do not stack.

| Item | Footprint | Maximum stack | Weight | Purpose / storage |
| --- | --- | ---: | ---: | --- |
| Money | 1×1 | 100 | 0.001 kg | Physical primary currency |
| Arrows | 1×2 | 30 | 0.03 kg | Opposite-hand ammunition stack |
| Knife | 1×2 | 1 | 0.3 kg | One-handed melee |
| Wooden club | 1×3 | 1 | 1.2 kg | One-handed melee |
| Machete | 1×3 | 1 | 0.8 kg | One-handed melee |
| Metal bat | 1×4 | 1 | 2 kg | One-handed melee |
| Bow | 2×4 | 1 | 1.2 kg | Bow-hand item |
| Berries | 1×1 | 10 | 0.1 kg | Food |
| Fruit | 1×1 | 5 | 0.2 kg | Food |
| Meat | 1×2 | 5 | 0.4 kg | Food |
| Basic shirt | 2×2 | 1 | 0.3 kg | Torso; 2×1 pocket |
| Trousers | 2×2 | 1 | 0.6 kg | Legs; 2×2 pocket |
| Coat | 2×3 | 1 | 1 kg | Torso; 2×2 pocket |
| Cap | 2×1 | 1 | 0.1 kg | Headwear; no protection |
| Face covering | 1×1 | 1 | 0.05 kg | Face; cosmetic |
| Gloves | 1×1 | 1 | 0.1 kg | Gloves; cosmetic |
| Boots | 1×2 | 1 | 0.8 kg | Boots; cosmetic |
| Protective helmet | 2×2 | 1 | 1 kg | Headwear; armor profile in section 4 |
| Improvised body armor | 2×3 | 1 | 3 kg | Body armor |
| Plated body armor | 3×3 | 1 | 7 kg | Body armor |
| Belt with pouches | 2×1 | 1 | 0.3 kg | Belt; 4×2 grid, visibly worn pouches |
| Small pouch | 1×2 | 1 | 0.15 kg | Portable container; 2×2 grid |
| Small backpack | 2×3 | 1 | 0.8 kg | Backpack; 4×4 grid |
| Field backpack | 3×4 | 1 | 1.8 kg | Backpack; 5×6 grid |

This is a 24-definition initial item set, plus unarmed attacks. Names, footprints, capacities, weights, appearance assignments, compatibility, and stats belong in configuration. No equipment-selling service is included; the sale menu accepts food only.

### 5.4 Shared stash and access

The main-building stash is shared by every player and ordinary recruited NPC. It has unlimited aggregate space through new pages, independent of building tier. Delivery cannot fail because existing pages are full.

**Proposed supporting rules:**

- Access requires being near the main building. There is no remote stash access.
- The interface supports page navigation, name/type filtering, deliberate placement, and quick transfer with automatic placement.
- Automatic placement first combines compatible stacks, then uses fitting empty spaces, rotating when allowed, then creates a page.
- The shipped page size must fit every supported item; configuration validation rejects definitions that could never enter any page.
- NPCs withdraw actual items and deliver actual cargo. Reservations prevent two actors taking the same item, but do not grant an actor ownership before the transfer succeeds.
- Simultaneous withdrawals, sales, equipping, and corpse looting must produce one consistent result. A losing action receives updated contents and a clear failure message.
- NPCs can equip wearable containers while collecting loot if suitable and available, but cannot ignore capacity or conjure a hauling bag.

### 5.5 Death, hauling, and cleanup

Every ordinary survivor or zombie corpse retains exactly what that individual possessed at death, including worn equipment, money, partly used arrows, food, broken items, and nested contents. Death does not roll fresh loot.

Any non-busy ordinary NPC may haul corpse belongings. Unassigned workers are not the only eligible haulers. A hunter remains a hunter and a warrior remains a warrior after a delivery. Built-in tower guards never haul.

**Proposed hauling rules:**

- Safety and the actor's primary duties take priority. Work outside the walls is daylight-only.
- Reserve items before pickup; remove them from the corpse only when the pickup succeeds.
- Respect grid capacity and load. Return when no more selected loot fits or carried weight reaches 80% of the soft-load threshold.
- Loot priority is money, ammunition, usable combat gear, food, containers, clothing, then broken gear. All are eventually eligible.
- If an item cannot fit, leave it for a better-equipped carrier or player. Do not delete it to complete a task.
- On arrival, cargo enters the shared stash; personal equipment and configured carried reserves remain with the NPC.
- If a carrier dies, its collected cargo remains on its new corpse. Items already removed from the original corpse do not return there.
- Nonempty corpses and loot piles never expire because of age. An empty bag or a broken item still counts as a belonging.
- Once completely empty, a corpse or loot pile expires after 180 seconds. Adding an item cancels that cleanup countdown.
- Cosmetic blood/acid and the physical loot container have separate lifetimes. Visual fading cannot delete loot.

### 5.6 Loading inventories after configuration changes

**Confirmed policy:** load saved layouts as they are and allow items to be taken out. Do not automatically move overflow to the stash, discard belongings, or refuse the entire load merely because size or nesting limits changed.

**Proposed supporting rules:**

- Preserve saved positions, orientation, old display footprints/grid boundaries, and existing containment where needed to make the saved layout readable. Mark incompatible locations as legacy layouts.
- A legacy location permits removal. It does not permit additional invalid placements or deeper invalid nesting.
- Taking an item out uses its current configured footprint and compatibility at the destination. If that destination cannot accept it, the move fails and the item stays at its old location.
- A filled legacy container can be extracted as a unit when the outer placement is legal and does not worsen invalid internal nesting. Its internal legacy placements remain removable.
- An emptied legacy container adopts its current configured grid and restrictions. A stack exceeding a new maximum can be reduced by extracting legal-sized stacks; nothing is silently truncated.
- Existing incompatible hand/slot occupancy is visible and removable. An unusable combination cannot attack or supply protection until made legal.
- Legacy layouts persist through subsequent saves until resolved.
- Missing item definitions or unreadable save data produce an explicit error identifying the missing definition/problem and preserve the original save. The game never substitutes arbitrary items or silently destroys unknown contents.

## 6. Economy, food, and recruitment

### 6.1 Money and spending

Money is a stackable physical item. Corpse money remains on the body; hauled money enters the stash. There is no automatic global treasury balance.

Recruitment, job changes, tree clearing, construction, upgrades, repairs, rebuilding, and rally assaults spend money from the acting player's carried inventory. Stash money must be explicitly withdrawn first. Another player's or NPC's money cannot automatically cover a shortfall.

**Proposed supporting rules:** payments may combine accessible money stacks across the acting player's carried containers. Spend an exact whole-number amount and leave change in its original stacks. A quote displays price and eligibility. Acceptance rechecks funds, capacity, distance, and target state together. An unsuccessful or duplicate action charges nothing. Accepted construction/work orders are not cancelable in 0.1 and do not refund on destruction.

### 6.2 Food — proposed baseline

Food restores both health and stamina and can be used through quick slots. There is no hunger or cooking requirement.

| Food | Health restored | Stamina restored | Sale price each |
| --- | ---: | ---: | ---: |
| Berries | 8 | 15 | 1 money |
| Fruit | 15 | 25 | 2 money |
| Meat | 30 | 40 | 4 money |

The main building has a separate food-sale menu. Only food already in the shared stash can be sold; proceeds are deposited as physical money into that stash. Carried food must be deposited first. Selling does not automatically pay expenses or transfer cash to the seller.

**Proposed supporting rules:** sell a selected quantity at the fixed configured price, with no daily quota. Food consumption, selling, and withdrawal compete for the same real items. NPCs keep three food items when available, eat when health is at or below 60% or stamina at or below 30%, and use a six-second personal eating cooldown. They replenish reserves only when safe. Lack of food causes no starvation damage.

### 6.3 Gathering and hunting — proposed baseline

| Source | Daily availability / behavior |
| --- | --- |
| Each berry plant | Refilled to 8 berries at dawn |
| Each fruit plant | Refilled to 6 fruit at dawn |
| Gathering | 3 seconds per food item; food transferred into the gatherer's inventory |
| Wildlife per authored zone | Up to 2 new rabbits and 1 new deer each dawn |
| Total living wildlife cap | 24; skip excess additions rather than deleting existing animals |
| Rabbit | 15 health; flee speed 4 u/s; corpse contains 2 meat |
| Deer | 40 health; flee speed 4.5 u/s; corpse contains 5 meat |
| Wildlife danger detection | Flee from camp characters within 8 u |
| Hunter operating range | Up to 60 u beyond the current camp border, without crossing an active portal |

Plants refill to their limit, rather than adding that amount to old stock. Remaining wildlife persists; dawn does not duplicate a previously processed batch. A hunter uses actual bow ammunition and collects actual meat from the carcass. Wildlife's meat is its carried loot, not an additional death reroll. Wildlife is not recruitable or eligible for player control.

**Proposed supporting rules:** wildlife does not attack, zombies ignore it, and NPCs do not hunt beyond reachable territory. Food plants are not cleared away with construction trees. Idle non-busy NPCs may haul animal-corpse contents under the common hauling rules.

### 6.4 Main-building recruitment

Survivors can only be bought through the main building. A successful purchase creates one unassigned recruit beside the fire with the basic outfit described in section 2. There are no external survivor camps.

The fireside limit counts **unassigned NPC survivors**, not the entire camp. An unassigned builder or hauler still counts while away working. Assigning a role frees a place even if the assigned NPC is waiting for equipment. Player-controlled survivors do not occupy NPC fireside places.

| Setting | Proposed baseline |
| --- | ---: |
| Recruitment price | 12 money |
| Unassigned capacity at main tier 1 / 2 / 3 | 8 / 12 / 16 |
| Total recruited-survivor cap | 100, including player-controlled characters |
| New-recruit creation | Immediately after the successful purchase |

Buying is blocked at either applicable limit, without charging. Only a completed main-building upgrade increases fireside capacity. Built-in tower archers do not consume population.

**Proposed supporting rules:** voluntarily changing an NPC to unassigned is blocked if it would exceed fireside capacity. An involuntary return to NPC control may temporarily exceed the unassigned limit; the survivor and belongings remain, and additional purchases/unassignments stay blocked until space exists. A waiting player may immediately receive a purchased recruit through normal succession.

## 7. NPC roles, equipment, work, and army orders

### 7.1 Roles — proposed baseline prices and equipment

| Role | Assignment price | Required equipment / primary work |
| --- | ---: | --- |
| Unassigned | 5 to change back from another role | No tool required; paid construction, repair, clearing, and idle hauling |
| Gatherer | 8 | No tool required; gather reachable food and deliver it |
| Hunter | 15 | Usable bow and arrows; hunt wildlife, collect meat, and deliver it |
| Melee warrior | 15 | Usable one-handed melee weapon; defend and join assaults |
| Warrior archer | 15 | Usable bow and arrows; defend and join assaults |

Recruitment initially grants the unassigned role without an additional job charge. Changing to a different role costs that role's configured price. Selecting the current role does nothing and costs nothing. No recurring wages are included.

Only living NPC recruits can be assigned. The people menu cannot reassign another player's controlled survivor. A role change redirects the NPC to the stash when safe; obsolete role equipment is returned there, preserving contents and condition. If essential equipment is missing, display Waiting for equipment rather than creating it. The role remains assigned.

### 7.2 Automatic equipment and supplies

NPCs automatically obtain suitable gear from the stash and switch to better suitable equipment whenever safe, even if the previous equipment was manually chosen by a player. There are no equipment locks in 0.1.

**Proposed supporting rules and baseline:**

- Equipment changes require an actual stash visit. NPCs do not teleport supplies from storage.
- Upgrade decisions occur no more than once every 20 seconds per NPC, only outside immediate danger.
- Melee preference: metal bat, machete, wooden club, knife. Archer/hunter preference: usable bow. A role preference must not equip a bow on a melee-role NPC as its primary weapon.
- Prefer better protection and larger storage when the resulting load remains below the severe threshold. Prefer higher remaining durability between otherwise equivalent items.
- NPCs may dual wield if two suitable one-handed weapons are available, with the common attack/stamina rules.
- Reserve 30 arrows for a hunter and 60 for a warrior archer, including the hand stack, limited by actual carried capacity.
- NPCs/zombies automatically refill an empty arrow-hand stack from their carried reserves. Once no arrows remain, combat fallback applies.
- A fallback needing a different hand item first stows the unusable gear. If no storage placement is legal, the NPC drops that gear into a persistent loot pile before equipping the replacement or using fists; it never deletes equipment to free a hand. Players make their own equivalent inventory changes.
- Replaced items return to the stash; broken items are not erased. Returning storage-bearing gear must preserve or safely transfer its contents.
- A civilian's equipment upgrade does not change its flee-first behavior.
- A fight is over for restocking purposes after five seconds without a hostile target or enemy within 12 u.

### 7.3 Work priorities — proposed baseline

Safety/escape, immediate self-defense, necessary food, and essential restocking override ordinary work.

Unassigned work priority is: paid main-building repair, paid outer-wall repair, other paid repair, paid construction/upgrade, paid tree clearing, corpse hauling, fireside idle. Within a priority, prefer the nearest reachable eligible task. Interrupted tasks retain their actual completed work and items. Releasing a reservation on interruption does not reverse a completed pickup or create another copy of a paid order.

Assigned NPCs perform their primary role before optional hauling. An idle specialist may haul, but does not become a builder or repairer. Warriors defend or follow their current assault order before hauling. Nighttime safety restrictions apply to civilian work regardless of task priority.

Up to three unassigned workers may contribute to one construction/repair/clearing task. Progress is measured in worker-seconds, saved continuously, and resumes after an interruption. Unavailable workers do not magically complete paid orders.

### 7.4 Defense and rally assaults — proposed baseline

Warriors automatically divide between the two sides. They station near that side's outermost intact wall, falling back when it is breached. With no wall, they defend the main-building refuge.

| Setting | Proposed value |
| --- | --- |
| Rebalance interval | 20 seconds, only for warriors not in immediate combat or an assault |
| Side selection | Prefer the side with fewer stationed warriors; break ties by shorter travel |
| Rally location | 5 u inside the current outer wall, or beside the refuge if no wall |
| Assault price | 25 money from the acting player's inventory |
| Squad size | 8 combat-ready warriors |
| Preferred mix | 4 melee and 4 archers; fill shortages with other ready warriors |
| Simultaneous assaults | One per side |
| Recall price | Free |
| Warrior threat-engagement distance | 15 u |
| Archer preferred firing distance | 16 u |

Paying at a side's rally point sends that side's selected squad against its next surviving portal. A shortage of ready warriors, an existing assault on that side, or no remaining portal makes the action unavailable without charging. Players can always attack a reachable portal directly.

Squads remain on their mission through nightfall; they are combatants, not exterior civilian workers. They defend themselves against nearby enemies, then attack the portal. After destroying the target, they automatically return to their camp side. They do not advance to another portal.

Players may recall the squad at its rally point. Recall does not teleport units or refund the launch fee. If another group destroys the target before arrival, the squad returns. Returning warriors resume their job, resupply when safe, and rejoin automatic side allocation.

## 8. Main building, walls, towers, and paid work

### 8.1 Progression and prices — proposed baseline

Prices in the Upgrade price column are incremental payments from the preceding tier. Rebuild prices restore the last completed tier directly. Times are worker-seconds.

| Structure / tier | Required appearance | Maximum health | Build / upgrade price | Work | Rebuild price / work |
| --- | --- | ---: | ---: | ---: | --- |
| Main 1 | Campfire and boxes | 600 | Starting structure | — | No rebuild after defeat |
| Main 2 | Wooden shelter | 1,400 | 200 | 90 | No rebuild after defeat |
| Main 3 | Metal-reinforced building with two guard towers | 3,200 | 500 | 150 | No rebuild after defeat |
| Wall 1 | Wooden | 250 | 20 | 20 | 20 / 20 |
| Wall 2 | Reinforced wood | 900 | 60 | 35 | 80 / 35 |
| Wall 3 | Metal-reinforced | 2,200 | 140 | 50 | 220 / 50 |
| Tower 1 | Wooden platform | 300 | 40 | 30 | 40 / 30 |
| Tower 2 | Reinforced wooden watchtower | 800 | 90 | 45 | 130 / 45 |
| Tower 3 | Metal-reinforced watchtower | 1,600 | 180 | 60 | 310 / 60 |

The completed main-building tier sets the maximum wall/tower tier. A paid incomplete upgrade unlocks nothing. Each main upgrade also raises fireside capacity. Stash capacity remains unlimited at every tier.

| Standalone tower tier | Built-in archers | Range | Arrow damage per archer | Firing interval per archer |
| --- | ---: | ---: | ---: | ---: |
| 1 | 1 | 20 u | 14 | 1.4 s |
| 2 | 2 | 24 u | 20 | 1.4 s |
| 3 | 3 | 28 u | 26 | 1.4 s |

Built-in archers and bows are supplied by the tower's price. They consume neither recruits nor stash equipment. They remain part of the tower, use unlimited arrows, cannot become leaders, and leave no separate inventory, corpse, or loot.

The main building's tier-3 attachments are **two level-1 attack profiles with one archer each**, not two tier-3 standalone towers. They share main-building health and cannot be destroyed, disabled, repaired, or rebuilt separately. They add two archers total.

**Proposed supporting rules:** tower archers fire independently at the nearest zombie in range, prioritizing an enemy attacking their defended structure when distances are equal. They do not attack wildlife or portals. Built-in bows have no separate durability.

### 8.2 Clearing, construction, and upgrades

Tree clearing costs money and worker time, without producing a material resource.

| Clearing group on either side | Proposed price | Proposed work |
| --- | ---: | ---: |
| First | 15 | 30 worker-seconds |
| Second | 25 | 45 worker-seconds |
| Third | 35 | 60 worker-seconds |

**Proposed supporting rules:**

- One paid work order per structure/site at a time. Concurrent player payments cannot create duplicate work or charges.
- A new/rebuilt structure supplies no wall border or tower attacks until complete.
- Its unfinished frame has 25% of the destination tier's health and can be destroyed; damage carries forward when construction completes.
- During an upgrade, the completed old tier remains functional with its current health. Completion adds the increase in maximum health, preserving the absolute amount of damage already sustained.
- If an upgrading defense is destroyed, the upgrade is lost and its previous completed tier remains available for paid rebuilding.
- Main-building upgrades follow the same damage rule, but main destruction always defeats the camp.
- Old walls remain when farther walls are completed. Buildings cannot be freely relocated or demolished for refunds in 0.1.

### 8.3 Repairs and rebuilding

Walls and towers never receive free automatic repairs. A player pays, then unassigned workers perform the repair. Destroyed walls/towers can be rebuilt at their previous completed tier for money and work; they need not restart at tier 1.

**Proposed main-building policy:** it also supports paid repairs while alive, using the same rules. Its destruction cannot be repaired after defeat.

**Proposed baseline:** repair price is the ceiling of 50% of the tier's full build value multiplied by its missing-health fraction, with a minimum price of one. Full build value is the sum of its build/upgrade prices; main tier 1 has a repair valuation of 100. Each worker restores 10 health per second.

A repair purchase authorizes the amount of missing health quoted at purchase. Further damage is not repaired for free beyond that purchased amount. No simultaneous upgrade/repair orders are accepted. Work obeys civilian safety rules, so workers will not stand beside attacking zombies merely to keep a wall repaired.

## 9. Zombies, loot, portals, and escalating attacks

### 9.1 Required zombie roster — proposed baseline stats

Each archetype has configurable appearance, health, movement, attacks, compatible equipment, and spawn availability. All carry real inventories. Their equipment affects combat and remains on their corpse.

| Archetype | Health | Walk speed | Unarmed damage / interval | Equipped-weapon damage multiplier | Defining behavior |
| --- | ---: | ---: | --- | ---: | --- |
| Shambler | 65 | 1.5 u/s | 8 / 1.2 s | 1.0 | Basic slow attacker |
| Runner | 45 | 2.7 u/s | 6 / 0.65 s | 0.9 | Fast, fragile attacker |
| Brute | 240 | 1.0 u/s | 22 / 1.6 s | 1.35 | Tough, strong attacker |
| Armored | 120 | 1.3 u/s | 10 / 1.2 s | 1.0 | Guaranteed usable body armor |
| Spitter | 80 | 1.2 u/s | 6 / 1.4 s | 0.85 | Natural direct-hit spit attack |
| Zombie archer | 65 | 1.4 u/s | 6 / 1.1 s | 1.0 | Guaranteed usable bow and physical arrows |
| Acid bloater | 180 | 1.0 u/s | 12 / 1.5 s | 1.0 | Fat silhouette; cosmetic acid splash on death |

**Additional proposed defaults:** zombie stamina 100, regeneration 12 per second after the common 0.5-second action delay, unarmed stamina cost 3, unarmed reach 1.1 u, and no natural health recovery or food consumption. Weight penalties and actual item durability still apply. Bloater splash radius is 3 u and cosmetic stain lifetime 20 seconds.

Any archetype can use compatible equipped weapons, including bows. Its type name does not exempt it from physical ammunition or armor rules. The default archer loadout guarantees 24–48 arrows; an armored zombie guarantees body armor. Other random loadouts must remain legal.

**Proposed attack priorities:** use a usable melee attack at contact range; otherwise use an equipped bow with arrows; otherwise use the spitter's natural ranged attack if available; otherwise close for melee. A zombie seeks the main building, attacks obstructing defenses, and engages nearby threatening survivors or exposed standalone towers within 12 u. Towers are valid attack targets even though they do not establish a camp border. Portal defenders prioritize nearby attackers while their portal is threatened.

### 9.2 Loot generation and equipment progression

Every zombie starts with money and at least one wearable clothing item. The proposed guaranteed clothing is trousers, which also provide a legal pocket grid for money. Loot exists before death. The generation process must place all mandatory items legally before considering optional items.

**Proposed baseline — equipment quality by stage:**

| Quality | Money per zombie | Fists / club / knife / machete / bat / bow weights | Body-armor chance | Plated share of armor | Helmet chance | Backpack chance | Minimum equipment condition |
| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |
| Q0 | 1–3 | 55 / 25 / 15 / 0 / 0 / 5 | 5% | 0% | 0% | 10% | 60% |
| Q1 | 2–5 | 40 / 25 / 20 / 8 / 2 / 5 | 15% | 10% | 5% | 15% | 65% |
| Q2 | 3–7 | 25 / 20 / 20 / 15 / 10 / 10 | 30% | 30% | 10% | 20% | 70% |
| Q3 | 4–9 | 20 / 15 / 15 / 20 / 15 / 15 | 45% | 60% | 20% | 25% | 80% |
| Q4 | 5–12 | 15 / 10 / 10 / 25 / 20 / 20 | 55% | 80% | 25% | 30% | 90% |

Equipment condition is chosen from the indicated minimum through 100%. Guaranteed archetype gear overrides a conflicting general weapon/armor roll. For example, an archer receives a bow rather than a fist result. Weapon damage uses the actual item and the archetype's modifier, not a separately invented loot weapon.

**Proposed optional loot:**

| Roll | Chance / quantity |
| --- | --- |
| Extra arrows | 35%; 8–20 arrows |
| Food | 25%; 1–2 food items, evenly selected from the three food types |
| Small pouch | 10%; one |
| Spare weapon | 10%; one from the current non-fist weapon pool |
| Spare armor | 5%; one from the current armor pool |
| Shirt or coat | 70%; coat chance within this roll increases 0 / 10 / 25 / 40 / 50% by quality |
| Boots | 30% |
| Cap, gloves, face covering | 10% each |
| Belt with pouches | 15% |
| Backpack type if rolled | Small at Q0–Q1; field-pack share 25 / 50 / 75% at Q2–Q4 |
| Second equipped melee weapon | 0 / 0 / 5 / 10 / 15% by quality, only with a legal one-handed primary |

Optional items are only created if they can legally fit or be equipped. Skipping an optional item for capacity must never omit guaranteed money/clothing or create an invalid inventory. A generated bow loadout receives an arrow-hand stack even on a non-archer archetype. Use 15–30 arrows for these incidental bow users.

The only continuing sources of ordinary equipment are zombie loot and purchased recruits' basic outfit. Initial stash supplies and built-in structural tower equipment are the other explicit exceptions. None of these exceptions introduces an equipment shop or crafting.

### 9.3 Separate zombie-progression configuration

Escalation changes counts, equipment, and variant mix. It does not periodically multiply the health/damage of already living zombies. Difficulty never scales with the number of connected players.

**Proposed baseline — percentages total 100 in every row:**

| Stage | Default days | Regular zombies per portal | Retaliation zombies per portal | Quality | Shambler | Runner | Brute | Armored | Spitter | Archer | Bloater |
| --- | --- | ---: | ---: | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 0 | 1–3 | 6 | 6 | Q0 | 75 | 20 | 0 | 0 | 0 | 5 | 0 |
| 1 | 4–6 | 12 | 10 | Q1 | 55 | 20 | 5 | 5 | 5 | 8 | 2 |
| 2 | 7–9 | 24 | 16 | Q2 | 35 | 20 | 12 | 10 | 8 | 10 | 5 |
| 3 | 10–12 | 40 | 24 | Q3 | 25 | 20 | 15 | 15 | 10 | 10 | 5 |
| 4 | 13 onward | 50 | 32 | Q4 | 20 | 15 | 20 | 20 | 10 | 10 | 5 |

Use the configured interval to derive stages, clamping at the last configured stage. Stage 4 continues until victory or defeat. A designer may add further stage rows without changing the rules.

### 9.4 Scheduled waves and retreat

Each surviving portal receives one regular wave on an eligible night. **Proposed timing:** start at nightfall and distribute its spawns over 90 seconds. Composition is randomized from that stage's weights. Every generated zombie has its own actual inventory.

At dawn, surviving zombies retreat toward a surviving portal. On reaching it, they leave the map with their remaining belongings. This creates no corpse or dropped loot. Future waves create new zombies, not returning survivors.

**Proposed supporting rules:**

- Prefer the nearest surviving portal on the same side, otherwise the nearest remaining portal anywhere on the map.
- A retreating zombie can defend itself or attack an obstructing defense to escape. It never teleports through camp walls.
- When all portals are gone, retreat stops; living zombies stay dangerous until killed.
- Cancel the unspawned remainder of a regular nightly wave at dawn. Living zombies are unaffected by that cancellation except for their retreat order.
- Existing corpses are never cleared at dawn.
- If a portal is destroyed during spawning, cancel its unspawned enemies; already living enemies remain.
- A daytime defensive or retaliation spawn is allowed after dawn and follows the next applicable dawn retreat, unless the portals have all been destroyed.

### 9.5 Portal health, defense, and retaliation — proposed baseline

| Portal position on either side | Health | Defensive wave size | Minimum defensive stage | Maximum active defenders from this portal |
| --- | ---: | ---: | ---: | ---: |
| Nearest | 2,500 | 6 | 0 | 40 |
| Middle | 5,000 | 10 | 1 | 60 |
| Farthest | 8,000 | 14 | 2 | 80 |

Portal defense uses the higher of the current day stage and that portal's minimum defensive stage. This gives distant portals tougher defenders even if attacked early.

A damage event can trigger a defensive wave during day or night, including opening grace. **Proposed timing:** a two-second warning, ten-second spawn duration, and a 30-second cooldown between defensive triggers. Continued damage may trigger another wave after cooldown; mere proximity does not spawn one.

Destroying a portal triggers a warning to all players, then **one additional retaliation wave from every surviving portal**, even during daytime or grace. **Proposed timing:** ten-second warning, then a 60-second release duration per portal.

Retaliation does not replace a regular nightly wave. Multiple portal destructions create distinct retaliation events. Queue overlapping releases at each source portal; do not silently merge them or duplicate the originating event. A source destroyed before release cannot spawn its queued wave.

### 9.6 Population and spawning limits

The proposed active-zombie cap is 300 across regular attackers, defenders, and retaliation together. This is an active living-enemy limit, not a corpse cleanup limit.

**Proposed supporting rules:** if the cap is full, wait to release further queued enemies. Do not delete a living zombie and its possessions to free a slot. Regular queues expire at dawn; defensive/retaliation queues persist until released or their source is destroyed. At a portal, use one release sequence at a time, prioritizing its defensive response, then retaliation, then regular waves. These limits and priorities are configurable.

Spawning, queues, warnings, event identities, already-created inventories, and random progression must survive saving without repeating a wave or rerolling an existing zombie.

## 10. Multiplayer and session behavior

### 10.1 LAN hosting and joining

- One participating player hosts the run. The total limit is four connected players, including the host and any spectators.
- Support both a LAN session list and manual host-address entry.
- Show session name, host, participant count, whether it is in progress, and compatibility/join status.
- A host can start alone and accept guests later. No account or external platform service is required for LAN play.
- Windows and Linux players can share a session.
- Late joining must provide the current time, camp, portals, NPC jobs/orders, inventories, equipment appearances, corpses, exploration, and attack state.
- A late joiner takes a random eligible survivor or spectates. Joining never creates survivors, money, food, ammunition, or another starting kit.
- Gameplay waits for a joining player's world state to be ready before accepting that player's actions. Existing participants continue playing.

### 10.2 Shared authority and simultaneous actions

The host owns one authoritative run. All participants must observe consistent outcomes for item ownership, damage, money, recruitment, job changes, building payments, succession, and victory/defeat.

All players have equal camp-management permissions. Only the host controls session-wide saving, loading, and closing. Guests cannot replace host balance settings.

Concurrent actions must resolve against current shared state. Examples include two players buying the last fireside place, taking the last bow, selecting the same successor, selling food an NPC wants, and paying for the same upgrade. Exactly the successful actions take effect and pay; unsuccessful actions receive an explanation and corrected state.

**Proposed baseline:** connection-loss detection after ten seconds without a response; in-progress join timeout after 30 seconds. These are configurable session values. A late-join snapshot cannot roll the world backward or cause existing participants to replay events.

### 10.3 Leaving, reconnecting, and hosting loss

A departing or disconnected guest's survivor returns to NPC control with all belongings and its remembered role. Its crown disappears. The character stays in the world and remains vulnerable.

Reconnecting restores the same survivor if alive and not controlled by someone else. Otherwise, use normal random succession/spectating. Do not steal a controlled survivor or resurrect a dead one.

**Proposed supporting rule:** maintain a stable local player identity separate from display name, so reconnecting to the same saved run can recognize the player without a platform account. A remembered unassigned role follows the temporary fireside-overflow rule.

Host departure or connection loss ends the session for everyone. There is no host migration. Normal host exit saves before closing; an unexpected loss can only recover from the last successful save. Other participants are informed clearly and returned to the menu.

### 10.4 Replaceable multiplayer-service boundary

LAN-specific discovery, hosting/joining, connection lifecycle, address handling, and player identity must be separate from gameplay rules.

Future Steam/Epic services must be able to supply discovery/invitations, identities, and connections while preserving the same camp ownership, transactions, inventories, succession, save behavior, and combat. Gameplay items and saves must not depend on a platform-specific account identifier as their only identity.

Version 0.1 must demonstrate this separation in its verification: gameplay must not directly depend on LAN addresses or a particular discovery service. This specification does not prescribe interfaces, protocols, libraries, or engine facilities. Platform matchmaking, cross-platform account linking, and platform publication are future work.

## 11. Saving, loading, and configuration changes

### 11.1 Saving contract

The host may save manually at any active-game time. Autosave at dawn and on normal session exit. Support multiple named saved runs and selecting a save to resume.

Saving occurs while play continues and records a coherent state. It must not heal characters, restore wear, refill inventories, complete paid work, change portals, reroll loot, or pause combat.

**Required saved state:**

- Run identity, map content version, day, phase progress, recorded victory/defeat, and exploration.
- Characters, health/stamina, living/dead state, equipment, complete nested inventories, quantities, durability, and legacy layouts.
- Stash pages, ground loot, corpses, and empty-container cleanup progress.
- Buildings, last completed tiers, damage, paid work, cleared trees, and camp boundaries.
- Roles, equipment waiting, task progress, cargo, rally assignments, squads, and recall/return orders.
- Portals, warnings, spawned counts, wave queues, daily-event completion, and randomness needed to avoid rerolling existing events.
- Plant stock, wildlife, and that day's refill/spawn status.
- Player identities and previous-survivor associations needed for reconnection.
- Relevant accepted commitments and compatibility information.

**Proposed retention:** unlimited manual saves while storage permits, plus the latest three autosaves per run. Distinct runs cannot accidentally overwrite each other. A failed save preserves the last valid save and gives the host a clear error. A normal exit with a save error offers retry, return to play, or explicitly leave without saving the latest progress.

A defeated or victorious exit may save that terminal/continued state; earlier retained saves remain selectable. A crash does not promise an impossible last-moment autosave.

### 11.2 Applying changed host settings

Current host configuration applies on new-run start and on loading existing saves. It does not live-reload during a running session. Guests use the host's effective settings.

**Proposed supporting rules:**

- Recalculate character/item behavior from current definitions; clamp current health, stamina, and durability if a new maximum is lower. Raising a maximum does not automatically heal or repair.
- Preserve inventory ownership and legacy placement under section 5.6.
- Keep the saved phase and fractional phase progress when day/night durations change. Loading is not itself a dawn event.
- Recalculate the escalation stage from the saved day and current interval/stage table. Future generated waves use the new progression.
- Preserve already accepted payments, already generated items, paid work completed, and pending wave commitments. Do not charge again, refund price differences automatically, or reroll an existing event.
- Current worker speeds and future prices apply normally; already purchased work retains its remaining work quantity.
- If a new population cap is below the saved population, keep existing characters and block further purchases/spawns as appropriate until below the cap.
- Unknown required definitions or corrupt data produce actionable errors and keep the original save intact.
- Compatibility checks distinguish valid balance changes from missing required content. Joining with incompatible gameplay/content must fail clearly before granting control.

## 12. Designer configuration

### 12.1 Editing workflow

Use separate, documented configuration files. There is no in-game balance editor. This document defines their logical responsibilities without choosing a storage format.

Every designer-tunable value must come from configuration, including all numbers in the proposed baseline tables. Changing balance must not require editing gameplay behavior. Content rosters, references, prices, stage weights, item grids, and map/site positions must also be editable data.

The fixed 0.1 feature contract—such as cooperative play, the supported four-player limit, no item duplication, and no self-containing bags—is distinct from balance. Configuration must not silently enable unsupported features or invalidate ownership rules.

### 12.2 Required configuration domains

| Separate domain | Required editable content |
| --- | --- |
| Run setup | Starting survivors/outfit/stash; total population caps; default map; initial day and phase |
| Time | Day/night duration; grace count; phase-boundary behavior parameters |
| Survivors | Health/stamina, movement/jump, action costs, recovery, safety distances, load thresholds/curves |
| Items | All 24 baseline definitions, labels, weights, footprints, stacks, rotation, storage, compatibility, appearance references |
| Combat | Weapons, unarmed attacks, projectiles, damage modifiers, armor, durability use, melee contact limits |
| Inventory | Stash pages, nested-container limit, quick slots, interaction distances, corpse/loot cleanup |
| Recruitment and jobs | Prices, tiered fireside limits, role needs, equipment preference, reserves, priorities, work/decision timing |
| Food and wildlife | Recovery/sale values, plant yields, harvesting, daily wildlife, prey attributes, corpse contents |
| Map | Terrain layout, sites, trees, plants, wildlife zones, portals, boundaries, minimap reveal |
| Structures | Tier appearance/health, prices, work, repair valuations, prerequisites, rebuild behavior parameters |
| Towers | Per-tier archer count/range/damage/interval, target preferences, main-building attachment profiles |
| Zombie archetypes | All seven variants, attributes, attacks, equipment compatibility, visuals, cosmetic splash |
| Zombie loot | Guarantees, optional rolls, quality equipment pools, condition, quantities, containers |
| Zombie progression | **Its own separate file:** interval, stages, regular/retaliation counts, variant weights, quality progression, caps |
| Portals and assaults | Health, defender profiles, warnings, cooldowns, spawn durations, squad size/mix, prices, recall parameters |
| Sessions and saves | Participant limit within supported scope, timeouts, save retention and feedback |
| Presentation | Camera/minimap parameters, effect lifetimes, sound assignments, UI sizing, feedback thresholds |
| Translation catalog | English names, descriptions, menus, prompts, warnings, errors, and outcomes; future locale entries |

Related domains may refer to stable content identifiers. Zombie progression must remain independently editable from zombie base stats and unrelated settings.

### 12.3 Documentation and validation

Each field must have a name, purpose, unit, default, valid range/choices, and relevant relationships. Each item/role/archetype needs a stable identifier and readable label. The shipped configuration must contain a complete working baseline, not missing entries that rely on hidden balance constants.

Validate before starting/loading:

- Positive durations, valid probabilities, coherent stage ordering, and nonempty weighted selections.
- Positive guaranteed zombie money and at least one valid wearable clothing item.
- Valid item references, legal slots/loadouts, stack limits, rotations, and storage dimensions.
- A stash page able to accept every supported outer item footprint.
- Main-tier restrictions, strictly increasing default fireside capacities, and the agreed tower guard counts.
- Valid map paths/sites and six authored portal locations.
- Sound, image, and translation references for required content.
- Population limits and queue settings that do not require deletion of existing saved belongings.

Report the file, field, offending value, and reason for invalid configuration. Do not silently replace it with a hardcoded balance value. A valid configuration with an old incompatible layout uses the legacy-load policy; that condition is not treated as invalid configuration.

Local display, audio, and key-binding preferences are player settings. They do not override shared host balance.

## 13. Interface, controls, art, sound, and translation

### 13.1 Required screens and feedback

| Surface | Required information and actions |
| --- | --- |
| Main menu | New solo/hosted run, LAN list, manual joining, load saved run, settings, controls, exit |
| Host/lobby | Session name/address information, participants, capacity, start/resume, join/compatibility status |
| HUD | Health, stamina, carried money, weight state, selected/held equipment, arrow count, quick slots, day/time, warnings |
| Main-building people menu | Buy survivor, fireside/total population limits, role counts, individual role assignment, equipment-waiting and activity status |
| Food-sale menu | Stash food quantities, selected sale amount, unit/total price, resulting stash money |
| Inventory | Character silhouette/slots, both hands, grids, rotation/splitting, weight, durability, containers, quick-slot assignment |
| Shared stash | Character and stash views, page controls, filter, quick transfer, exact current shared contents |
| Nearby NPC / corpse / loot pile | Clear owner/target, immediate contents, permitted transfers, equipped gear, distance/access feedback |
| Construction site / structure | Tier, health, requirements, price, repair quote, paid work progress, waiting/danger reason |
| Rally point | Stationed/ready warriors, squad size/mix, next target, price, active order, recall |
| Minimap | Explored terrain, known portals, defenses, teammates, current position |
| Spectator | Follow/cycle living teammates, waiting status, automatic handover when eligible |
| Save/load | Run name, day, result/status, save time, manual/autosave identity, compatibility/error feedback |
| Victory / defeat | Outcome, remaining danger where applicable, continue/load/new-run/menu choices appropriate to that outcome |

**Proposed supporting rules:** interface panels capture their mouse clicks so moving an item cannot fire a weapon or buy something behind the panel. Escape closes the current panel before opening the in-run menu. External inventory access closes when the target is no longer in range or valid. Closing a panel never drops a pending drag. Damage, approaching night, retaliation, and disconnection have visible feedback even when a panel is open.

Spectators cannot buy, move items, assign jobs, or deal damage without a controlled survivor. They may follow living teammates and view shared map/status information, without revealing unexplored territory.

### 13.2 Proposed default controls

All gameplay bindings must be remappable and saved locally. Rebinding cannot make an essential action permanently inaccessible; provide reset-to-default.

| Action | Default |
| --- | --- |
| Move | A / D or left / right arrows |
| Sprint | Hold Shift |
| Jump | Space |
| Attack / manually fire bow | Left mouse button |
| Aim bow | Mouse position |
| Interact with nearby target | E |
| Own inventory | I |
| Rotate a dragged item | R |
| Quick transfer between open inventories | Ctrl + click |
| Split a stack | Shift + drag, then choose quantity |
| Use quick-slot food | 1–4 |
| Expand/minimize map view | Tab |
| Close panel / in-run menu | Escape |

When several targets overlap, show a short target selector rather than silently interacting with an unrelated NPC or structure. Dropping an item requires an explicit drop action/confirmed world-drop target; releasing a drag outside a grid is not destructive.

**Proposed presentation settings:** windowed/fullscreen mode, display resolution, UI scale from 80–150%, independent master/effects/ambience volume, and remappable bindings. Support a readable layout at 1280×720 and at the 1920×1080 target. The default camera shows 80 u horizontally at 16:9, follows with 0.15-second smoothing, and stays inside map bounds; these are presentation configuration values. No graphics option may remove gameplay-relevant equipment information.

### 13.3 Art and animation completeness

Use the supplied references for pixel-art materials, proportions, palette, and atmosphere. The written progression takes precedence over their exact silhouettes.

Required finished content:

- A coherent handcrafted environment with readable foreground, ground, background, day/night lighting, and boundaries.
- Three main-building appearances: campfire/boxes, wooden shelter, metal-reinforced building with two separate attached guard towers.
- Three wall tiers and three standalone tower tiers, including construction, damage, and destroyed-site states.
- Portals with visible damage, defense/retaliation warning, destruction, and permanently destroyed states.
- Visually distinct silhouettes for all seven zombie types, including the fat bloater.
- Survivor and zombie equipment presentation for every applicable slot and all required items.
- Character idle, walking, sprinting where used, jumping/landing where used, melee, bow firing, hit reaction, and death states.
- NPC gathering, building/repairing, carrying, and relevant work feedback; tower firing; spitting and cosmetic bloater death effects.
- Rabbit and deer idle/movement/flee/hit/death presentation.
- A readable icon for every inventory definition, with stack, rotation, durability, and legacy-layout feedback.
- A leadership crown with player identification using both color and number/name. It must not hide actual equipment status.
- Corpse and loot-pile interactions that remain readable when many bodies overlap.

Reused assets are permitted. Placeholder or missing visuals that obscure a required mechanic do not meet completion. Animation layers must work with one-handed pairs, bows/arrows, clothing, armor, and bags across the supported character shapes. Cosmetic effects cannot hide essential targets or delete their loot.

### 13.4 Sound

Required coverage includes footsteps, melee swings/impacts, bow shots/impacts, spit, zombie attack/death cues, player damage/death, construction/repair, tree clearing, recruitment/payment, inventory actions, food use/sale, nightfall, retaliation, portal destruction, victory/defeat, and environmental ambience.

**Proposed baseline:** master/effects/ambience default to 80/80/60%. Limit repeated crowd sounds to 24 concurrent gameplay sounds, prioritizing nearby damage, attacks, and warnings; limit each repeated effect to four overlapping instances. Ambient loops must transition cleanly between day/night.

Music, voice acting, and a narrated tutorial are not required. Critical warnings must also be visible; muted audio must not conceal the start of retaliation or a connection failure.

### 13.5 Translation readiness

Ship all required text in English. Provide translation entries for UI, item/role/archetype names, descriptions, controls, warnings, errors, save/session messages, and outcomes. Translation must not require changing gameplay rules.

Do not embed text necessary for play solely into artwork. Layouts must accommodate longer replacement strings through wrapping, expansion, or scrolling. Verify this using temporary expanded text during testing; shipping a second translated language is not required.

## 14. Definition of a finished 0.1 release

Version 0.1 is finished only when all mandatory systems in this document work together and the acceptance scenarios below pass. A feature's menu, placeholder, isolated demonstration, or configuration entry alone is insufficient.

### 14.1 Release gates

1. **Complete loop:** a fresh solo run and a fresh four-player LAN run can reach victory without development commands, manually injected items, unfinished mandatory steps, or intervention in saved data.
2. **Content complete:** the one map, six portals, seven zombie types, four assigned NPC roles, three main/wall/tower tiers, two wildlife types, 24 baseline item definitions, equipment appearances, interface, and required sound exist.
3. **Outcomes complete:** succession, spectating, both defeat conditions, victory, continued play with remaining enemies, menu exit, and restarting/loading all work.
4. **Multiplayer complete:** Windows/Linux cross-play, discovery, manual joining, late joining, reconnecting, shared actions, and host loss behave as specified for up to four players.
5. **Persistence complete:** manual/dawn/exit saves, multiple runs, full item/corpse/work/wave state, host configuration changes, and legacy inventory extraction work without duplication or silent loss.
6. **Designer-ready:** the baseline is in documented configuration files, including separate zombie progression. Every listed balance domain is editable without changing gameplay rules.
7. **Usable presentation:** no required interaction lacks readable feedback; no equipped item category is invisible; no known incomplete placeholder blocks understanding or play. No tutorial or music is required.
8. **Performance demonstrated:** pass the reference-PC test below with required simulation and equipment visuals enabled.
9. **Stability demonstrated:** no known reproducible crash, save corruption, item/money duplication, persistent multiplayer disagreement, or progression-blocking defect remains in the acceptance scenarios.
10. **Runnable delivery:** the released game can be started on the documented tested Windows and Linux systems without a development environment. Record the tested operating-system versions and Linux distribution.
11. **Default pacing evaluated:** record successful playthrough durations and days, including solo and four-player play. Adjust the proposed balance until representative successful runs support the intended approximately 2–3-hour, 8–12-day experience.

The values in this document are a starting hypothesis. Actual playtests must establish the default economy, ammunition supply, durability pressure, attack difficulty, and completion pacing; the specification itself does not claim these are already proven.

### 14.2 Performance and endurance procedure — proposed supporting rule

Use the creator's PC as the reference machine. Record its CPU, graphics hardware, memory, operating system, resolution, settings, and game/content configuration with the results. No hardware model or completed benchmark is assumed by this document.

Test a four-player session at 1920×1080 with approximately 100 recruited survivors and 300 active zombies. Include a dense battlefront, full visible equipment, working NPCs, nested inventories, shared-stash use, and existing corpses.

**Proposed measurable pass condition:** at least 30 displayed FPS in 95% of one-second samples across a ten-minute representative stress scenario, with no sustained period below 30 FPS longer than five seconds. Record any save/join spikes separately; active play must not freeze for longer than 0.5 seconds during those operations. Do not obtain a pass by turning off required AI, inventory state, equipment visuals, or combat.

For an endurance run, play at least one three-hour saved/resumed session with recurring battles and at least 200 nonempty corpses or loot piles represented over its course. Existing belongings must remain accessible, and the session must not develop a progression block or inconsistent inventories. Intended persistent loot growth is not a reason to delete nonempty corpses.

The reference-PC performance target is separate from verifying that the game functions on both supported operating systems.

### 14.3 Observable acceptance scenarios

| ID | Setup / action | Required result |
| --- | --- | --- |
| A-01 | Start fresh solo and four-player runs | Eight total survivors in either case; one per starting player; correct basic outfits, shared stash, day 1, and six portals; no multiplied starting kit |
| A-02 | Play through first night, then night 2 | First scheduled waves are absent; night 2 receives one wave per surviving portal; provoked spawns remain possible during grace |
| A-03 | Advance through days 3→4, 6→7, and 9→10 | One stage change at each configured boundary; counts, equipment, and variant mix follow the separate progression file |
| A-04 | Kill a controlled survivor while replacements exist | Corpse keeps actual belongings; exactly one random eligible survivor receives control/crown; its health, gear, and location are preserved |
| A-05 | Have two leaders die together with only one successor | Only one receives that survivor; the other spectates; no shared control, duplicate body, or duplicated inventory |
| A-06 | Spectate while a teammate buys a recruit | Eligible waiting player gains control under the waiting order; purchase and character creation occur only once |
| A-07 | Destroy main building; separately kill every eligible camp survivor | Each independently defeats the camp; remaining money or purchase capacity cannot prevent defeat |
| A-08 | Destroy the final portal while zombies remain | Immediate victory; continued play preserves enemies and loot; survivors do not retreat at later dawns |
| A-09 | Trigger victory and defeat together; separately lose camp after victory | Apply the documented simultaneous-result rule; a previously recorded victory is not erased by a later continuation loss |
| A-10 | Fill fireside capacity, assign a job, buy, and upgrade main building | Full capacity blocks buying without charge; assignment frees a place; completed upgrade raises capacity; pending upgrade does not |
| A-11 | Buy a recruit and inspect its belongings | It appears at the fire with only the basic clothing/pockets; no money, food, armor, ammunition, or weapon is created |
| A-12 | Assign all four roles with and without essential equipment | Roles perform their jobs; missing essentials cause visible waiting; unassigned workers alone perform paid construction/repair |
| A-13 | Manually equip an NPC, then provide better suitable stash gear | Safe NPC visits the stash and upgrades; old gear and pocket/container contents remain real; danger delays discretionary upgrades |
| A-14 | Exhaust NPC arrows or break its weapon during combat | It uses carried reserves, a usable fallback, or fists as appropriate, then restocks safely; no infinite ammo or broken-weapon damage |
| A-15 | Threaten daytime civilians and block/unblock escape | They abandon work and flee, fight only if trapped, resume escape when possible, and preserve their cargo |
| A-16 | Enter night while exterior work is underway | Civilians retreat; safe interior work can continue; assault warriors keep their combat order |
| A-17 | Clear trees and build farther walls | Payment and worker time are required; marked sites unlock; borders advance; inner walls stay; no wood resource is produced |
| A-18 | Upgrade main building, walls, and towers through all tiers | Correct visuals, health, prerequisites, fireside limits, and 1/2/3 standalone archers; old tier remains functional during upgrades |
| A-19 | Complete main tier 3 and attack its attachments | Two attached one-archer level-1 attack profiles exist; they share main health and cannot be separately destroyed |
| A-20 | Damage, pay to repair, then destroy/rebuild a defense | No free repairs; workers restore only purchased work; destroyed defense rebuilds at its previous tier for money and work |
| A-21 | Destroy a tower | Built-in guards/bows disappear with the structure and leave no character corpse, inventory, transferable bow, or population change |
| A-22 | Rotate/split/stack/equip, use quick slots, and move filled nested bags | Legal operations preserve identity, state, contents, and exact weight; illegal fit, cycles, over-depth placements, and hand combinations fail safely |
| A-23 | Cross load thresholds and wear equipment to zero | Movement/stamina penalties apply; walking remains possible; severe load prevents sprint/jump; broken weapons/armor are unusable and cannot be repaired |
| A-24 | Open corpse, NPC, and stash inventories during an attack | Contents appear immediately; valid transfers are immediate; the world continues; panel clicks do not attack or trigger background purchases |
| A-25 | Fill existing stash pages and deliver more cargo | Additional pages accept all valid cargo; aggregate capacity does not block NPC deliveries or food-sale proceeds |
| A-26 | Kill a loaded survivor/zombie and a carrier mid-delivery | Corpses contain only actual remaining possessions; spent ammo stays spent; collected cargo is on the carrier's new body, not duplicated at its source |
| A-27 | Leave a corpse nonempty beyond cleanup time, then empty/repopulate it | Nonempty corpse persists; empty timer starts only when empty and cancels if an item is added |
| A-28 | Have players/NPCs compete for one item, food stack, upgrade, or fireside place | Only valid successful actions commit and charge; all participants converge on identical ownership and state |
| A-29 | Sell stash food while NPCs withdraw/eat, with no carried cash | Only actual stash food is sold; proceeds remain in stash; purchases still require explicit cash withdrawal |
| A-30 | Harvest/hunt across dawn and save/load near the transition | Correct finite food/meat; one refill/batch per day; no duplicated daily events or disappearing leftover animals |
| A-31 | Fire character/tower arrows through allies, walls, and enemies | Friendly arrows pass allies/defenses; enemy arrows damage blocking defenses; character arrows are consumed and none are recoverable |
| A-32 | Use all seven zombie types with equipped gear | Distinct behavior/stats and actual weapon/armor effects; spit hits one target; bloater splash causes no gameplay effect; all required loot persists |
| A-33 | Try to pass an active portal, then destroy it | Survivors cannot bypass it by jumping; zombies can pass; destruction permanently opens the route |
| A-34 | Damage a portal by day and destroy it during grace | Defenders can spawn; every surviving portal schedules extra retaliation after warning; regular waves are not replaced |
| A-35 | Destroy portals during pending waves, fill active-enemy cap, and reach dawn | Correct cancellations/queues and retreat; no spawning from destroyed portals, no duplicate event, and no deletion of living enemies to free capacity |
| A-36 | Follow a dawn retreater into a living portal | It leaves with remaining belongings and creates no loot; later waves create new zombies; nearby old corpses remain |
| A-37 | Launch/recall an assault; destroy its target before arrival in another test | Correct payment, side, ready squad, and target; recall/success/obsolete target return survivors without teleporting or automatically targeting another portal |
| A-38 | Join via discovery and manual address on mixed Windows/Linux machines | Both routes work; four participants are supported; incompatible content is explained before control; late state matches the host |
| A-39 | Disconnect/reconnect a guest, including after its former survivor dies/is controlled | Survivor becomes an NPC with gear; reconnect restores it only if available, otherwise normal succession/spectating; no stolen control |
| A-40 | Close host normally; separately lose its connection unexpectedly | Everyone exits the session; normal exit saves; unexpected loss uses the last successful save; there is no host transfer |
| A-41 | Save/resume mid-battle with worn gear, nested bags, work, corpses, warnings, and queues | All relevant state continues without healing, rerolls, double payments, repeated waves, or lost belongings |
| A-42 | Change host values, then load with guests using different local settings | Host values govern the new session; saved commitments remain consistent; guests agree; no live reload occurs mid-session |
| A-43 | Shrink grids/stacks/nesting limits, load an old inventory, and extract items | Old layout remains visible/removable; new placements use current rules; no forced stash transfer, deletion, or rejection solely for layout incompatibility |
| A-44 | Load with an unknown required item or interrupt a save | Actionable error and preserved prior save; no silent item substitution or corrupted replacement of the last good save |
| A-45 | Explore separately, succeed to another character, save, and late join | Independent cameras work; shared explored map and known portal/defense state remain consistent |
| A-46 | Rebind controls, mute sound, scale UI, and substitute longer catalog text | Essential actions remain usable; critical warnings remain visible; translations do not require gameplay changes or obscure controls |
| A-47 | Run complete solo/four-player campaigns and the reference performance/endurance tests | Pass the release gates, record observed pacing and hardware/results, and retain full required systems throughout |

## 15. Supplied references and inspected material

### 15.1 Recovered Kingdom Two Crowns material

Reference root: D:/Work/Games/KindomCrown/recovered.

The supplied recovered source and exported configuration were inspected for wall/tower behavior, main-building progression, camp borders, worker jobs, recruitment, paid work, portals, time-of-day progression, attack direction, and multiplayer joining/state handling.

Inspected areas included Wall, Tower, GuardSlot, Castle, Kingdom, Peasant, BeggarCamp, PayableUpgrade, WorkableBuilding, JobPriorities, Portal, Day, Director, TimesOfDay, and the network connection/router/session code, alongside exported resource/configuration catalogs.

| Observed reference pattern | How this specification uses it |
| --- | --- |
| Outer versus intact defensive borders | Explicit expansion, breach, fallback, and safe-work rules |
| Paid upgrades completed by workers | Separate payment, worker progress, prerequisites, and completed-tier effects |
| Relative job priorities | Configurable work ordering and civilian safety |
| Tower guard slots | Structural 1/2/3 guards instead of ordinary recruited archers |
| Portal defenses and attack scheduling | Explicit defensive/retaliation rules; permanently destroyed portals |
| Daily content and time transitions | Configured cycle, daily events, waves, and stage boundaries |
| Joining and catch-up behavior with two-player assumptions | A consistent four-player requirement; no inherited two-player limit |
| Exported building costs and prerequisites | Reference structure only; this game's proposed prices are independently specified |

The original game was not run as part of this inspection, so these are source observations rather than runtime verification. The exported asset inventory/catalog is not evidence of a Tarkov-style gameplay inventory.

The creator has authorized reuse of the supplied source/assets. Specific assets have not been copied or selected by this specification. This document's rules take precedence over incompatible reference behavior, including recruitment, tower guards, inventory, player count, portal rebuilding, and outcomes.

### 15.2 Visual references

- [Base reference tier 1](../../src/godot-game/references/bases/main-base-tier-1.png): wooden-building style and materials.
- [Base reference tier 2](../../src/godot-game/references/bases/main-base-tier-2.png): reinforced wood and lookout detail.
- [Base reference tier 3](../../src/godot-game/references/bases/main-base-tier-3.png): metal reinforcement and defensive-building atmosphere.
- [Zombie reference](../../src/godot-game/references/zombie/zombie-idle-original-size.png): muted green skin, torn clothing, and side-view pixel-art treatment.

These images were inspected visually. The base images are style references, not the required silhouettes: the specified tier 1 is campfire/boxes, and tier 3 must have two attached towers. Existing zombie animation sheets/strips are possible asset sources, but their timing and production readiness have not been verified here.

## 16. Proposed supporting decisions and balance review

The creator explicitly selected the major gameplay, inventory, recruitment, multiplayer, saving, configuration, platform, and presentation rules. To make the specification usable without leaving routine behavior undefined, the following supporting decisions are **proposals**:

- The map coordinates, exact site/plant/prey counts, distance unit, and shared NPC-assisted exploration.
- Exact item roster, pocket/container layouts, all unconfirmed numerical values, starting stash quantities, and equal starting supply across player counts.
- Health/stamina action delays, armor calculation, dual-attack timing, melee contact limits, and non-wearing ordinary clothes/storage items.
- Fireside counting during temporary control transitions, total population cap, role prices, equipment ranking, and worker/hauling priorities.
- Precise safe-border rules, main-building paid repairs, construction-frame damage, upgrade health handling, no cancellation/refunds, and repair valuation.
- Squad readiness/mix, one simultaneous assault per side, free recall, and return/rebalancing details.
- Exact escalation boundary convention, stage/loot tables, portal health/defender profiles, warning/release durations, caps, and queue handling.
- Simultaneous victory/defeat precedence, remembered roles, spectator order, and post-victory outcome recording.
- Legacy-layout presentation/extraction details, save retention, compatibility handling, and treatment of already accepted commitments after balance edits.
- Default controls, interface settings, sound limits, and the precise performance/endurance sampling procedure.

These proposals do not add an engine requirement or a development deadline. They are explicit starting decisions for implementation and playtesting, not statements that the creator separately approved every number or that a playable balance has already been measured.

**Primary balance checks:** the opening supply must support a viable camp; food and zombie money must fund growth without removing tradeoffs; arrows and equipment durability must create replacement demand without unavoidable ammunition starvation; expanding and destroying portals must reduce ordinary attack sources while retaliation remains threatening; both solo and four-player play must support the intended run length at the same configured difficulty.

Changing a proposed value should update its configuration and this baseline record. Changing a confirmed feature or exclusion is a scope change and should be recorded explicitly rather than silently inherited from a reference game.
