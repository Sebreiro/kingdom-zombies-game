# Plan 04-08 — Bounded execution amendments

## Task 1 exact cross-assembly presentation authority (2026-11-17)

Inspection of the actual assembly boundary confirms that Core only exposes its internal types to Phase1.Tests. The real CharacterPreview therefore needs one public, engine-independent presentation identity. Before implementation, the coordinator authorizes two additional existing paths for this plan:

- `docs/code/visibility-exemptions.json`
- `tests/phase1/ArchitecturePolicy.cs`

Append only the exact identity `src/core/CharacterPresentation.cs|KingdomZombies.Core.CharacterPresentation` to the reasoned registry and independent C# expected set. Preserve all seventeen current identities, the registry shape, ordinal path/type matching, duplicate and expanded-registry rejection, and all existing positive and negative controls. Add compiler-valid controls for the actual new identity, wrong path, adjacent public type and expanded registry in the already owned CharacterPresentationTests.cs. No other public Core type, public Godot script, native access or generated-source permission is authorized.

The permitted minimal public contract is the sealed CharacterPresentation class, its constructor accepting animation/items/combat definition bytes, a detached JsonElement Snapshot, Configure(body, movement, action, leftItem, rightItem, facing), Advance(elapsedSeconds), Flinch(), and TryContact(generation). Implement strict validated definitions and explicit elapsed presentation time. Contact is a one-shot preview observation; it never creates gameplay hits. Retain logical hand identity and immediate facing, independent lower/upper presentation, and stale-generation rejection.

Task 1 may publish the two policy paths as a separate bounded part alongside its original five production/proof paths. Task 2 retains its original four paths. The plan union becomes seven existing/planned paths; the distinct phase union remains236 because both authority files are already inventoried. No task, dependency, wave, product decision or correction budget changes. The parent owns global progress and inventory reconciliation after terminal SUMMARY.

The contained CharacterPreview transport may subscribe to guarded native frame delivery, accept explicit Tick(delta), and expose working play/pause, speed, step and reset controls within the already owned preview. Preserve the existing seven-argument constructor and static fitting behavior for legacy consumers; no InspectionRoot or InspectionShell edit is authorized. Each added native access and producer-input impact must be recorded for04-23 reconciliation. Source/headless proofs must exercise the actual implementation. The compiled native-stride RED may establish the missing behavior on the current preview before adding the new contract; missing API, fixture or discovery failures do not qualify. Actual windows and judgments remain gated on complete production and independent review.

## Task 1 affected body-proof resource closure (2026-11-17)

The actual bodies-a native hub and bodies-b consumer call CharacterPreview.Open. Their existing FIT_FILES list also supplies the native staging closure. Once Open consumes real animation definitions, these proofs require those actual resources. Before editing, the coordinator adds only existing `tools/phase4/cases/bodies-a.cjs` ownership for appending `src/godot-game/content/animation/locomotion-melee.json`, `src/godot-game/content/config/items.json`, and `src/godot-game/content/config/combat.json` to that closure, and `src/core/CharacterPresentation.cs` to its source fingerprint inputs. Preserve all current helper algorithms, assertions, IDs, counters, limits, authentication, resource-lifetime fixes and negative controls. Do not substitute fallback animation when a required production definition is missing.

This is one bounded proof-dependency part; the plan union is now eight paths and the distinct phase union remains236. No budgets reset. The first GREEN build's eighteen Core diagnostics remain part of the original three-attempt correction history. Re-run affected original body/native/hub consumers after the production change; record the exact frozen current inputs and every outcome.
