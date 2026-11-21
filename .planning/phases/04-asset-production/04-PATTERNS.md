# Phase 04: Asset production - Pattern Map

**Mapped:** 2026-11-16
**Scope:** 16 proposed file families / integration surfaces; filenames inside new directories remain planner decisions. Research specifies responsibilities and directories, not a final exhaustive filename list.
**Coverage:** 5 exact integration surfaces, 7 role matches, 4 families without a production analog.

## File Classification

| New/Modified File or family | Role | Data Flow | Closest tracked analog | Match |
|---|---|---|---|---|
| `src/core/` catalog IDs, detached definitions and validators | model | transform | `src/core/FoundationSnapshot.cs` | role |
| `src/core/` composition/current-state/preview clock values | model | event-driven | `src/core/FoundationSession.cs` | role |
| `src/godot-game/content/` expected IDs, output manifest, baseline domains, map, poses/fits, messages | config | transform | None for content schema | none |
| `src/godot-game/assets/` PNG/font/WAV/theme/effect resources | component | file-I/O | None for production media | none |
| `production/phase4/` selected exports, generated masters, prompts, recipes, provenance | config | batch | None for asset production | none |
| `tools/phase4/` extraction/conversion/export tools | service | file-I/O | `src/godot-game/foundation/LocalProfileStorage.cs` for publication only | role |
| `src/godot-game/inspection/` hub scene/root and preview adapters | component | event-driven | `src/godot-game/foundation/FoundationView.cs` | role |
| `src/godot-game/inspection/` query/selection/input state | store | event-driven | `src/godot-game/foundation/FoundationInput.cs` | role |
| `src/godot-game/inspection/` audio audition/current-state restoration | service | event-driven | No audio bank/player analog | none |
| `tests/phase1/EngineAccessPolicy.cs` | config | transform | Existing exact policy | exact |
| `docs/code/` visibility and generated-source permission registers | config | transform | Existing exact registers below | exact |
| `tools/phase4/check.cjs`, catalog and saved required cases | controller | batch | `tools/phase3/check.cjs`, `tools/phase3/catalog.cjs` | role |
| `tools/phase4/` scenario/evidence/completion modules | service | batch | `tools/phase3/scenarios.cjs`, `tools/phase3/evidence.cjs` | role |
| `tests/phase1/` content/composition/layout and negative controls | test | transform | Existing compiled proof and policy suite | exact integration |
| `docs/content/` field, production/export and known-issue pages; navigation entries | config | transform | `src/godot-game/docs/README.md`, `docs/code/README.md` | exact integration |
| Godot project/export configuration and approved project declarations | config | file-I/O | Existing `src/godot-game/project.godot`, project policy | exact integration |

## Pattern Assignments

### Detached definitions, validation and controlled state

**Analogs:** `src/core/FoundationSnapshot.cs:34-56`; `src/core/FoundationSession.cs:73-100,168-179`; `src/core/SnapshotWriter.cs:1-34`.

```csharp
// FoundationSession.cs:93-100
public void Step(long elapsedMilliseconds)
{
    RequireActive();
    ArgumentOutOfRangeException.ThrowIfNegative(value: elapsedMilliseconds);
    FoundationParticipantState[] candidate = [.._snapshot.Participants.Select(selector: participant =>
            AdvanceParticipant(participant: participant, elapsedMilliseconds: elapsedMilliseconds))];
    _snapshot = new FoundationSnapshot(sessionId: Actor.SessionId, revision: checked(_snapshot.Revision + 1),
            participants: candidate);
}
```

Copy framework-only imports, named arguments, ordinal stable identity checks, candidate validation and complete immutable publication. Snapshot validation rejects duplicates with `InvalidDataException`; explicit encoding in `SnapshotWriter` writes required schema fields and enforces byte limits. Content needs its own independent expected-ID set, schema versions, finite timing/geometry checks and references. Do not reuse session/player constraints as content rules. Manual preview stepping receives elapsed time; restoration installs current appearance and loops without replaying transient events.

### Filesystem staging and production/export tools

**Analog:** `src/godot-game/foundation/LocalProfileStorage.cs:12-42,52-77,82-112,125-130`.

```csharp
// LocalProfileStorage.cs:100-107
using (var stream = new FileStream(path: temporary, mode: FileMode.CreateNew, access: FileAccess.Write,
        share: FileShare.None))
{
    stream.Write(buffer: bytes);
    stream.Flush(flushToDisk: true);
}

_publish(arg1: temporary, arg2: _path);
```

The existing adapter normalizes its root, rejects reparse-point ancestry, takes an exclusive lock, bounds input size/depth and deletes temporary files in `finally`. Copy these responsibilities into the chosen offline language rather than adding production code to profile storage. Its single-file replacement is only a partial analog: a content pack needs validated staged outputs plus an atomic manifest/publication strategy, contention controls and preserved originals. Archive selection, streamed animation conversion, isolated audio decoding, template authentication and packaged dependency inspection have no existing production implementation. Use research for those algorithms.

### Guarded native view, selection and lifetime

**Analogs:** `src/godot-game/foundation/FoundationView.cs:1-20,21-48,64-81`; `src/godot-game/foundation/FoundationInput.cs:15-46`.

```csharp
// FoundationView.cs:64-72
internal static void Project(Node2D world, FoundationActorState actor, MainThreadGateway gateway)
{
    gateway.RequireThread();
    Node2D player = world.GetNode<Node2D>(path: "Player");
    float x = actor.PositionSubunits * 16f / 1000;
    x = float.IsFinite(f: x) ? x : 0;
    player.Position = new Vector2(x: x, y: 540);
    player.GetNode<Polygon2D>(path: "Facing").Scale = new Vector2(x: actor.Facing, y: 1);
```

ProjectRoster removes obsolete nodes before adding absent ones: copy reconciliation by stable identity, avoiding accumulated equipment layers/players. Copy import grouping (System, Godot, project namespaces) and small guarded synchronous adapter methods. Replace primitive polygons, hardcoded camera geometry and presentation wall-clock smoothing with the Phase 4 authored media, attachment metadata and explicit preview clock. The inspection hub is a separate scene, not a replacement for existing session authority.

`FoundationInput.SetContext` clears held commands at ownership changes; `Clear` blocks previously held keys until release. Reuse this behavior for focus loss, modal/selector changes and scene recreation; introduce current binding labels rather than copying hardcoded movement keys.

**Lifetime sources:** `src/godot-game/proofs/MainThreadGateway.cs:10-23,32-48`; `src/godot-game/proofs/LifetimeToken.cs:30-35`.

```csharp
// LifetimeToken.cs:31-35
internal void Close()
{
    IsOpen = false;
    Cancellation.Cancel();
}
```

Gateway drainage checks `item.Lifetime.IsOpen` before applying queued results on the captured thread. Preserve the acceptance fence before cancellation/disconnection; queue detached decode results, never worker access to native objects. Explicitly disconnect signals, stop/free audio and retire old layers during teardown. This is a lifetime pattern, not an existing audio implementation.

### Exact native and generated permissions

**Authority:** `tests/phase1/EngineAccessPolicy.cs:34-61,85-100,111-139,142-170`.

```csharp
// EngineAccessPolicy.cs:117-122
bool production = StringComparer.Ordinal.Equals(x: path, y: "src/godot-game/foundation/" + type + ".cs")
        && StringComparer.Ordinal.Equals(x: node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                .FirstOrDefault()?.Name.ToString(), y: "KingdomZombies.Foundation")
        && node.Ancestors().OfType<TypeDeclarationSyntax>().Count() == 1
        && _production.TryGetValue(key: type, value: out string[]? productionMethods)
        && productionMethods.Contains(value: name, comparer: StringComparer.Ordinal);
```

**Critical planning difference:** adding names to `_production` alone cannot authorize `inspection/` files: path and namespace are currently pinned to Foundation. Add an equally exact inspection path/namespace/type/method inventory and negative sibling/path/unguarded controls. Policy includes constructors, property/field/event access and method references; lambdas/local functions/async bodies are not granted native access. Guard recognition accepts the captured readonly `_gateway` or an unassigned parameter named `gateway` (`142-170`), not an arbitrary helper named similarly.

`docs/code/visibility-exemptions.json:5-9` supplies the exact shape:

```json
{
  "path": "src/godot-game/foundation/FoundationRoot.cs",
  "type": "KingdomZombies.Foundation.FoundationRoot",
  "reason": "The ordinary authored main scene requires this exact Godot engine-facing root and its authenticated generated partial."
}
```

For new public Core contracts or public Godot scripts, justify each actual path/type separately. `docs/code/engine-generated-spans.json:5-22` binds observed diagnostics to producer/package/source hashes, exact span, symbol and positive/negative proofs. `docs/code/generated-callsite-adaptations.json`, `docs/code/generated-layout-adaptations.json` and `docs/code/generated-syntax-adaptations.json` are exact authenticated registers, not blanket suppressions. Extend only for actually observed generated surfaces after compiling the chosen scripts. Never edit generated output or treat runtime/generated paths embedded in register data as source analogs.

### Current runner, scenarios and evidence

**Analogs:** `tools/phase3/catalog.cjs:1-44`; `tools/phase3/check.cjs:8-16,67-92`; `tools/phase3/scenarios.cjs:329-352`; `tools/phase3/evidence.cjs:1-19`.

```javascript
// evidence.cjs:16-20
function exact(expected,actual,label){
  requireTrue(Array.isArray(expected)&&expected.length>0&&Array.isArray(actual),'EXACT_EMPTY '+label);
  requireTrue(new Set(expected).size===expected.length&&new Set(actual).size===actual.length,'EXACT_DUPLICATE '+label);
  requireTrue(expected.length===actual.length&&actual.every(id=>expected.includes(id)),'EXACT_MEMBERSHIP '+label);
}
```

Copy CommonJS/node built-in imports, checked relative/canonical paths (`inside`, lines 8-14), source hashes, exclusive evidence writes and exact selected-result reconciliation. Never derive the independent 180-obligation expectation from the produced manifest itself. Generalize expected-set helpers deliberately if an empty set is valid; this helper rejects it.

Catalog rows require title/importance/rationale/setup/reset/runGroup, ordered actions/expected/evidence/requirements/acceptance/tags, stable case IDs and order (`catalog:9-15`). `prelaunch:29-44` checks Windows, current review identity, resolved findings and immutable review copies. The runner records failures per case and continues collecting outcomes (`check:73-85`); source drift invalidates results (`87`). Scenario groups sort by persistent order, reuse compatible processes, record individual failures and clean up in `finally` (`scenarios:329-352`).

Use new Phase 4 paths/groups/IDs and source inputs; remove LAN-specific participant/timing/role assumptions. Preserve all active Critical cases and new/changed/affected regressions with omission reasons. Save source-only vs native/rendered vs listening/judgment/export evidence separately. Add fresh completion-index reconciliation; Phase 3 success does not establish a Phase 4 completion command. Preserve H01-H34, separate UI backstops and four judgment reviews as identifiable obligations.

### Documentation navigation and project/export configuration

**Analogs:** `src/godot-game/docs/README.md:1-12`; `docs/code/README.md`; validator `tests/phase1/DocumentationProjects.cs:13-23,33-53`.

```markdown
# Godot integration reference

[Project](../KingdomZombies.csproj) · [Executable project policy](../../../tools/phase1/projects.json) · [Native fixture](../proofs/Phase1.cs) · [Code index](../../../docs/code/README.md)
```

Copy concise reference pages and relative links from a reachable index. Add `docs/content/README.md` and link its field/production/export/known-issue pages into authored navigation. Preserve exact Git path case and UTF-8; new docs must enter the documentation checker's authored scope. Do not duplicate or create a sixth `project-policy` declaration: existing five module references remain the authority. Existing native README introductory phase wording is stale; copy its navigation structure, not its claim that menus/LAN are future work.

Godot project/config changes must keep the ordinary launch target and approved project edges. Export presets/template acquisition and JSON/non-resource inclusion checks need new Phase 4 behavior; project existence alone is not a packaging analog or proof. Runtime resources belong under the Godot project root, production masters and recipes outside it.

## Shared Patterns

- **Validation/errors:** immutable candidates, ordinal IDs, finite numeric inputs and bounded strict documents; machine-readable diagnostic codes plus contextual ID/file/field/value for hub recovery. Do not turn fallback art/text into a passing resource status.
- **Native access:** captured gateway guard at each exact authorized synchronous member; lifetime fence before cancellation and teardown. No web authentication applies.
- **Standards:** named C# arguments, grouped imports, small methods (60-line body ceiling), no blanket analyzer suppression. Preserve existing five-project topology.
- **Evidence:** source identities and exact expected/observed set equality; observed native draws, actual listening and independent judgments retain distinct credit. Full implementation and review/fixes precede grouped real Windows 1280×720 runs.
- **Provenance/publication:** originals stay immutable; validate staged outputs and hashes before publication. A file's presence does not establish exported completeness, audition or visual approval.

## No Analog Found

No current production analog exists for the content schema/census, finished raster/audio production, donor/runtime-curve conversion, composed animation/fitting/occlusion, audio routing/audition, or isolated Windows export validation. Research and UI-SPEC define these contracts. Existing state, filesystem and view patterns cover their boundaries only. Do not add donor research or gameplay transactions to fill these gaps.

## Metadata

Analog scope: tracked Core, Godot Foundation/proof adapters, Phase 3 tooling, Phase 1 permission/documentation controls and code registers. Every named existing analog was checked with `git ls-files`; embedded generated output locators are evidence data only. New directory families are proposals. No source, branch or commit changes were made. Only this pattern map was written.
