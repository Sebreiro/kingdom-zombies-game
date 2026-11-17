---
phase: 04-asset-production
plan: "08"
subsystem: composed-character-animation-and-inspection
status: complete
tags: [Godot, CSharp, locomotion, melee, logical-hands, replacement, strict-evidence]
requires:
  - phase: "04-07"
    provides: "All eight ordinary bodies, immutable source art and complete native fitting"
  - phase: "04-06"
    provides: "Registered body/gear layers and seven-argument static preview compatibility"
  - phase: "04-18"
    provides: "Validated item and combat configuration with accepted intervals/events"
provides:
  - "Independent authored lower locomotion/upper melee, minimal contact and visual-only flinch"
  - "Immediate facing, coherent logical hands, current-generation body/gear/movement/action replacement"
  - "Working preview transport and real selected-character validation diagnostics/recheck recovery"
  - "Reusable source/headless scenarios plus pending actual-input/image checkpoints for seven character UI states"
affects: ["04-09", "04-10", "04-13", "04-23", "04-24", "04-31", "04-32", "04-33"]
tech-stack:
  added: []
  patterns: ["Detached bounded JSON presentation samples", "Explicit manual elapsed time", "Logical hands independent of draw order", "Per-generation contact and callback fencing", "Actual native caller recovery"]
key-files:
  created: ["src/core/CharacterPresentation.cs", "src/godot-game/content/animation/locomotion-melee.json", "tests/phase1/CharacterPresentationTests.cs", "tools/phase4/cases/animation.cjs"]
  modified: ["src/godot-game/inspection/CharacterPreview.cs", "src/godot-game/inspection/InspectionRoot.cs", "docs/code/visibility-exemptions.json", "tests/phase1/ArchitecturePolicy.cs", "tools/phase4/cases/bodies-a.cjs"]
key-decisions:
  - "One minimal sealed public Core identity carries detached engine-independent presentation; no additional public game identity."
  - "Use existing authored pose/grip art and manual explicit elapsed composition without baked equipment combinations or gameplay hit creation."
  - "Catch only documented definition/content failures around actual Open; unexpected native/programming/lifetime failures remain failures."
  - "Keep native operations and generated producer observations as exact handoff facts for04-23, without permission or cohort enrollment."
requirements-addressed: [ASSET-02, ASSET-03]
requirements-completed: []
actuals:
  tokens: 31429
  tasks: 2
  commits: 13
  diff_chars: 125714
  plan_head_before: 0f44c7e4fd9a93b6e8c68ef967a21f4d40f3c262
  plan_head_after: 9da8f8879f9704267311cf99d8dc947ad2b28c92
duration: 89.48min
completed: 2026-11-17
coverage:
  - id: current-character-source-native
    description: "Concurrent melee/locomotion, immediate facing/current replacement and independent native UI/transport/lifetime proofs"
    verification:
      - kind: integration
        ref: "animation.replacement/d82ba67e-09e1-45d6-8739-4846c54e9da6"
        status: pass
      - kind: integration
        ref: "animation.melee/aaa3103e-a75e-475d-a1ab-c413049c5c99"
        status: pass
    human_judgment: false
  - id: actual-character-appearance-input
    description: "Current-build actual input, equipped appearance, motion weight, readability and named visual review"
    verification: []
    human_judgment: true
    rationale: "Whole-phase implementation and independent review/fixes precede04-32/33 Windows1280x720/no-VM actual acceptance."
---

# Phase 04 Plan 08: Concurrent locomotion, melee and coherent logical hands Summary

Authored lower-body locomotion and weighted upper-body melee now compose on all eight actual bodies, with immediate logical-hand-safe replacement, working preview transport and exact selected-character error recovery.

## Accomplishments and task commits

| Task | Result | Commit |
| --- | --- | --- |
| 1 — Compose lower-body locomotion with weighted fists and both-hand melee | Actual authored tracks, strict source bindings, minimal contact, flinch, independent poses/grips and complete original native matrix | `bc7883dfb1097e3bd1ce4318eb1a4ce2f27dfcd5` |
| 2 — Prove immediate facing and mid-action body/gear replacement | H15/H18, stale contact/lifetime fencing, neighbour isolation, native transport, seven distinct UI states and caller error/recheck boundary | `9da8f8879f9704267311cf99d8dc947ad2b28c92` |

Nine implementation paths are the exact amended plan union; distinct Phase04 inventory remains236. Task2 changes exactly Core, CharacterPreview, CharacterPresentationTests, animation.cjs and the prospectively authorized existing InspectionRoot. No pack/index/census/totals, other presenter, generated/native permission or dependency installation was changed. Parent owns global progress, continuation and PR5 updates.

## Verification and exact frozen selection

Frozen source/headless selection: **16 PASS / 0 FAIL / 0 source drift**, 399 inputs. Source freeze is `c83630dc1cb9e010960d378b1ba9d71878714beb` plus the exact five-path Task2 WIP; no source was edited during this selection. `.phase4/04-08-task2-selection/selection.json` records selection and omissions; completion.json records drift and exit0. Every original assertion, case identity, negative control and process gate was retained.

| Case | Invocation | Result |
| --- | --- | --- |
| `animation.replacement` | `d82ba67e-09e1-45d6-8739-4846c54e9da6` | PASS |
| `animation.melee` | `aaa3103e-a75e-475d-a1ab-c413049c5c99` | PASS |
| `bodies.a-art` | `275af0a0-7d10-4bfe-b9d0-7e86caabbe94` | PASS |
| `bodies.a-fit` | `879ac878-507e-41f2-9faa-9389287faa80` | PASS |
| `bodies.b-wide` | `dc019738-c05a-4eb5-91c1-e2804f39344d` | PASS |
| `bodies.complete-fit` | `8d08302e-52f7-4961-9b37-340a1e1dbe26` | PASS |
| `source.case-discovery` | `e24d0a20-47f3-4aef-9d80-03497ac66767` | PASS |
| `data.foundation` | `a318bb0e-ec77-4a1d-bc0c-4d531ad02225` | PASS |
| `data.items-combat` | `6fd1ceef-525e-4270-b2ff-cef4355cf3ac` | PASS |
| `hub.source` | `ed195015-b3d2-4aa1-9f9b-a64b6404a452` | PASS |
| `hub.lifecycle` | `fd5cf989-47c1-4623-94cc-1003b389a8ab` | PASS |
| `items.weapons-food` | `f99fca89-9207-40f6-bc62-cbbeacb8a35b` | PASS |
| `items.wearables` | `06d30c1b-f515-4d9b-a946-5fc00f23e37d` | PASS |
| `layouts.theme` | `5b8535f5-c2c9-4938-9fd4-187cc3275fb3` | PASS |
| `layouts.identity` | `dcb83b35-0637-4247-888f-55fa4bb0765f` | PASS |
| `layouts.identity-integrity` | `49287f1f-a316-4011-8743-ad1a7de2caf3` | PASS |

| Animation case | Invocation | Actual native PID / exit | Melee samples / strict negatives / replacement checks / compiled tests |
| --- | --- | --- | --- |
| `animation.replacement` | `d82ba67e-09e1-45d6-8739-4846c54e9da6` | 32208 / 0 | 704 / 5 / 41 / 35 |
| `animation.melee` | `aaa3103e-a75e-475d-a1ab-c413049c5c99` | 25572 / 0 | 704 / 5 / 41 / 35 |

Both animation cases execute the full shared report contract:35 compiled PASS, zero failures/skips,704 native samples (8 bodies ×2 directions ×11 independently chosen loadouts ×4 stages), four survivor jump stages, five explicit source negatives, independently restored valid binding,41 replacement/control checks and seven required UI states (plus an extra actual overflow-scroll observation). Production and harness strict builds each have0 warnings/0 errors. Native stderr is empty. PID/nonce/archive-authenticated nonconsole engine and exact game/Core DLL SHA256 identities are checked; Core is loaded explicitly before game in the same owning AssemblyLoadContext. Nonzero child exits/fatal stderr fail even if any partial payload looks positive.

Original regressions retain A2976 tuples/5972 native observations/10 compiled/12H13; B-wide and complete7936 tuples/16158 native observations/12H13 each; hub source/lifecycle44 methods/63 expanded compiled subjects each with25/50 actual native observations. Original body art/source, discovery controls, foundation/item-combat data, weapon/food/wearable, theme and identity/integrity controls pass afresh. Other unchanged exporter/audio/media/publication/building/foundation-gameplay behavior cases were explicitly not selected for this bounded slice and receive no pass/skip credit. Final whole-phase Critical/rendered selection remains separate.

Task1 earlier frozen15/15 PASS on399 inputs and mandatory fresh committed-content tracer `9d164474-f9d4-419b-8c1a-239f8dc04bb8` remain immutable history. That tracer authenticated nativePID34880/exit0 and32 compiled PASS before Task2.

## TDD Gate Compliance

Task1 genuine RED commit `9d506ea`, run `0d46cc2c-552a-485f-a5ec-d64317ac5c84`: actual production native static lower segment stayed CH-01.windup.lower after explicit elapsed input. Target Native_StrideContinuesDuringMelee compiled and failed the intended True/False assertion, with native exit0 and exact process/assembly/context identity; SDK RED_EVIDENCE_OK and semantic assessment retained. Task1 GREEN is bc7883d; its committed tracer passed before Task2.

Task2 genuine RED commit `0f26bd74a62482d4753d28f03b0f16f011681415`, run `17aa51bb-4f8f-4945-be97-c29fa0a7990a`: strict builds0/0, actual nativePID38892/exit0 and33 compiled tests =32PASS/one intended Native_InvalidDefinitionKeepsVisibleErrorAndTransport FAIL/zero skips. Actual Selection→Open threw InvalidDataException CHARACTER_DUPLICATE_KEY without exact resource path/cause in Failures, while Recheck stayed usable. SDK RED_EVIDENCE_OK and the unchanged JUnit/report timestamps/semantic assessment are retained. Initial staging EEXIST `f9ae7dd4-52f8-4e82-81c8-142297f42efa` is INVALID_RED; the rejected command-array evidence record is also preserved, not used as authorization. No production GREEN preceded this genuine RED. Final GREEN passes both full shared35-test reports; no optional refactor commit was needed.

## Deviations from Plan and retained failure budgets

Prospective coordinator amendments in04-08-AMENDMENTS/REPAIR added the exact public identity authority, body-proof resource/Core owning-context closure and existing Root selected-character error boundary before those edits. These are bounded integration adjustments, with no obligation pruned.

1. **[Rule2 — missing critical boundary]** The actual caller did not project validated definition failures. Selection now tracks the resource read key, catches only InvalidDataException/JsonException/FormatException around Open, clears the partial current presentation and projects selected ID/resource/actual cause into existing Failures/status. Attach/native/programming/thread/lifetime failures remain outside that catch. Restored original bytes followed by real Recheck clear the duplicate cause and reopen real CH-06 art/controls. No added Root method survives: the first helper changed generator shape, so the final boundary remains inside existing Selection with its existing generated member inventory.
2. **[Rule1 — current replacement/transport correctness]** Movement changes now invalidate old contact generations. Current gear/condition changes end the previous action before clean projection; detach ends the active action and removes owned callbacks/art. Reset now reconciles actual selectors, play/pause/step state and Core state rather than immediately projecting stale selector choices back over the reset.
3. **[Rule3 — proof dependency and fixture correctness]** Explicit owning-context Core load/hash authentication, exact traversal exception/fixture contract, valid scene ownership and real Details expansion retain all expectations instead of supplying fixture/build failures as behavior evidence.

Historical budgets were not reset. Original Task1 corrections:3/3 exhausted (initial18 diagnostics; corrections2,10,then0 compiler diagnostics), then strict/native `79d5851f-0b8a-41ea-89f4-c7524ffaecba` failed due owning ALC dependency load. Decomposition1 passed initially `762687b0-3b70-4176-9d94-e619be11649c` with20 compiled tests,0/2 corrections. Decomposition2 initial `03ad36cd-59b3-4320-8223-a3fd6b6eae94`, corrections `f7f908f6-64b1-470d-9862-aab3ca3616e1` and `973d62c1-f609-4ada-9de6-e59322a3e558` remain failures,2/2 exhausted; the last harness had22 MA0006 and1 MA0002 diagnostics. Ordinal scope1/2 run `3970ac64-8802-4dbf-b23c-c9f8140b7b2e` passed strict builds but nativePID36408/exit1 failed the distinct traversal-negative contract; no expanded/native/compiled accepted pass is inferred. Traversal scope used2/2: `56bb4202-0d06-41f6-b02d-153716393f62` failed observer detach of an unattached preview; `60bbd52b-2480-407b-99f9-0be0de0a270d` passed all704 samples/four jump/five negatives/restored positive/32 compiled. Decomposition3 passed the original15-case selection initially,0/2 corrections.

Task2 initial GREEN `3079c1e0-2b74-4552-99cf-96fa9bb9532d` failed strict build at22 generated-span diagnostics because an added Root helper changed ScriptMethods shape. Correction1 `b9281493-3de3-45af-aa32-3595d860375a` restored existing generated method inventory and passed both strict builds, but native exit1 failed long-text fixture setup (metadata still collapsed). Correction2 `a79e4e0c-4a2d-4061-8435-6a2aab3f4663` opens actual Details before asserting visibility and passes complete35-test/native proof. Task2 correction budget is2/3 used, resolved; the final16-case selection passes without another source repair. Sandbox Git-spawn EPERM setup `a16944a9-75ed-4c8f-a663-d03e074c9e8b` has no behavior credit; equivalent authorized escalation runs normal commands/hooks/gates. All earlier failed reports/checkpoint manifests remain immutable.

The exact five Open negative outcomes remain: InvalidDataException ANIMATION_MISSING content/animation/locomotion-melee.json; InvalidDataException ANIMATION_BINDING_HASH content/config/items.json; InvalidDataException ANIMATION_BINDING_SET; InvalidDataException CHARACTER_DUPLICATE_KEY; and FormatException with exact canonical relative path required: src/godot-game/../escape.json. The traversal fixture now actually reaches that canonical segment rule. Production exception contracts were not normalized or broadly accepted.

## Reusable scenarios and acceptance boundary

The owned animation.cjs exports SCENARIOS with stable IDs, importance/rationale, phase/requirement/dependency tags, setup, ordered actions, expected results, G0 ordering/reset/evidence, and separate pending W04-character actual-input/image checkpoints. Actual checkpoints require the whole phase plus independent review/fixes, Windows1280×720/no VM, real input logs, current-build before/after screenshots and named appearance/readability review. Fixture-only absent/long-identity/partial/error cases explicitly require an independently prepared owned copy; they do not pretend the current pack browser offers those synthetic IDs as ordinary choices. No actual window, physical input, listening, named judgment, ASSET completion or full-phase acceptance is claimed.

| Scenario | Importance | Setup | Actions | Expected |
| --- | --- | --- | --- | --- |
| `04-08.H15-orders` | Critical | Prepare CH-04 walking melee with IT-05 left/IT-06 right at .4 seconds | Facing then IT-03/IT-04; repeat gear then facing; repeat three rapid body/armor/hand/facing permutations | Identical final logical IDs, registered layers and occlusion; facing preserves stride/action phase |
| `04-08.H18-replacement` | Critical | Independently prepare mid-contact CH-04 walk/melee before each change | Replace body CH-07, left IT-03, right IT-04, armor IT-20, movement Faster, action None; restart melee and advance .13 seconds | Every old layer detached immediately; old contact generation rejected; clean concurrent stride/action restart |
| `04-08.lifetime-isolation` | Critical | Create two real previews sharing actual resource bytes | Mutate first; compare second sample/layers/pixels; detach second then emit saved control callback and attempt Tick/old contact | Neighbour unchanged; detached Tick rejected, old callback and contact cannot restore art |
| `04-08.transport` | High | Paused valid walking/melee preview | Single-step; wait native frames; Play each .25/.5/1/2 speed; Pause; Reset through real buttons | Single-step advances one sample; paused clock stable; real Frame scales exact native delta; Reset reconciles displayed selectors and sample |
| `04-08.empty` | Normal | Actual Root with absent CH-no-data selection | Invoke production Selection | No Character body remains; metadata says No validated definition |
| `04-08.loading` | High | Actual Root with previous selection | Invoke Recheck before processing queued resource reads | Loading status and usable cancellation while current validation is pending |
| `04-08.error` | High | Valid actual CH-06 baseline; owned duplicate animation key | Select again; inspect Failures/status/Recheck; restore exact bytes and Recheck | Exact selected ID/resource path/CHARACTER_DUPLICATE_KEY visible; no stale body; actual preview reopens and old cause clears |
| `04-08.populated` | Normal | Actual Root, valid CH-06 | Select and inspect native hierarchy | Authored body and current movement/gear/transport controls present |
| `04-08.partial` | High | Owned copy with profiles a/b present and only fits-c absent | Select real CH-06; inspect diagnostic; restore exact profile and select again | Exact missing profile path/cause visible, old body absent, restored profile reopens preview |
| `04-08.overflow` | Normal | Actual character controls with all rows in 260x320 scroll container | Await native layout, compare content/viewport and scrollbar; scroll to 200 | Content exceeds viewport, real scrollbar has range and offset changes |
| `04-08.long-text` | Normal | Run-owned 40-phrase absent identity through actual Root Selection | Open actual Details and inspect native wrapping ResourceId plus metadata surface | Exact long identity retained, WordSmart wrapping enabled and metadata visible |

Long-text native proof retains the exact unusually long absent identity in the actual wrapping summary and opens actual Details; it does not certify human readability. Partial uses real profiles a/b plus absent c and shows the precise required profile diagnostic; it does not silently substitute fallback art. Empty, pending load, invalid definition, valid body, partial assets, overflowing real controls and long text are distinct native setups/outcomes.

## Artifacts this phase produces

### Production contracts and symbol inventory

Core identity is exactly `src/core/CharacterPresentation.cs|KingdomZombies.Core.CharacterPresentation`, public sealed and engine-independent. Minimal public API remains constructor(animation/items/combat byte arrays), detached JsonElement Snapshot, Configure(body/movement/action/logical hands/facing), Advance(explicit seconds), Flinch and one-shot TryContact(generation). Snapshot fields are body,movement,action,leftItem,rightItem,facing,generation,strideSeconds,actionSeconds,intervalSeconds,lowerPose,upperPose,stage,lowerOffset,upperOffset,leftAngle,rightAngle,flinchOffset,transitionOffset,contactReady,contactVisible,releaseSound,impactSound,gameplayEvents,nextSampleSeconds. Bounded strict UTF8/JSON/depth/count/finite/ordinal selection/timing/track/event validation is retained.

```csharp
public CharacterPresentation(byte[] animation, byte[] items, byte[] combat)
public JsonElement Snapshot
public void Configure(string body, string movement, string action, string leftItem, string rightItem, int facing)
public void Advance(double elapsedSeconds)
public void Flinch()
public bool TryContact(long generation)
private static readonly int[] _zeroOffset = [0, 0];
private readonly JsonElement _definition;
private readonly Dictionary<string, double> _intervals = new(comparer: StringComparer.Ordinal);
private readonly Dictionary<string, JsonElement> _attacks = new(comparer: StringComparer.Ordinal);
private string _body = "CH-01";
private string _movement = "idle";
private string _action = "none";
private string _left = "Empty";
private string _right = "Empty";
private int _facing = 1;
private long _generation;
private double _stride;
private double _actionTime;
private double _flinch;
private double _transition;
private double _contact;
private bool _contactAccepted;
private double ContactFraction => Timing(name: "contactFraction");
private static JsonElement Parse(byte[] bytes, string id)
private static void ValidateValue(JsonElement value, ref int count)
private void ValidateTracks()
private static void ValidateAngles(JsonElement sample)
private void ValidateTiming()
private double Timing(string name)
private string AttackKey()
private static bool Hand(string item)
private double Interval()
private double Angle(string item, JsonElement sample)
private double Length(string clip)
private JsonElement Sample(string clip, double time)
private static string Text(JsonElement row, string name)
private double Remaining(string clip, double time)
```

Internal CharacterPreview retains its seven-argument constructor/static fitting semantics and actual merged profile/body/item/atlas dependencies. Native access uses the captured MainThreadGateway and explicit Tick; only an attached, playing instance receives owned frame callbacks. Actual node identities include Controls/Slots selectors Body,Pose,Facing,Condition,Zoom,all declared fit slots,Movement,Action,PlaybackSpeed; buttons Play,Step,Reset,Flinch; Art/Body_lower,Body_upper,Body_head,registered gear and minimal Contact; Status. Metadata keys body,pose,facing,sample,item,logical_hand,part remain data; dotted part IDs are not Node.Name. Owned Image/GetImage copies use scoped disposal, borrowed resources stay borrowed, no forced GC/delay/exit substitute is used.

```csharp
internal CharacterPreview(Node2D root, MainThreadGateway gateway, JsonElement fits, JsonElement bodies, JsonElement weapons, JsonElement clothing, string body)
internal static JsonElement ReadDefinition(byte[] bytes, string expectedId)
internal static CharacterPreview Open(Node2D root, MainThreadGateway gateway, Func<string, byte[]?> read, string body)
internal static JsonElement FitItem(JsonElement fits, string body, string item, string pose, int facing)
internal void Attach()
internal void Detach()
internal void Tick(double delta)
private readonly Node2D _root;
private static readonly string[] Empty = ["Empty"];
private readonly MainThreadGateway _gateway;
private readonly JsonElement _fits;
private readonly JsonElement _bodies;
private readonly JsonElement[] _packs;
private readonly Dictionary<string, OptionButton> _controls = new(comparer: StringComparer.Ordinal);
private readonly Dictionary<string, Texture2D> _textures = new(comparer: StringComparer.Ordinal);
private string _body;
private bool _attached;
private CharacterPresentation? _presentation;
private SceneTree? _tree;
private bool _playing;
private readonly Dictionary<string, Button> _buttons = new(comparer: StringComparer.Ordinal);
private readonly List<Texture2D> _ownedTextures = [];
private JsonElement _sample;
private string _equipment = "";
private static CharacterPresentation ReadPresentation(Func<string, byte[]?> read)
private void AnimationControls(Node slots)
private Action TransportAction(string id)
private void Play()
private void Step()
private void Reset()
private void PreviewFlinch()
private void Frame()
private void RetireArt()
private static void ValidateFitting(JsonElement root)
private static JsonElement ReadProfile(Func<string, byte[]?> read, string profile)
private static JsonElement MergeFits(JsonElement[] profiles)
private static JsonElement MergeBodies(JsonElement[] profiles)
private int BodyChoice(Node slots)
private static string Text(JsonElement value, string name)
private static void ValidateValue(JsonElement value)
private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)
private void Changed(long index)
private string Selected(string id)
private void Project()
private void EndAction()
private void ReconcileEquipment()
private void Contact(Node2D art, JsonElement pose, int facing)
private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing, string poseId)
private static JsonElement FindPose(JsonElement body, string id)
private Vector2 SegmentOffset(bool lower)
private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)
private string BodyAtlas()
private void AddItem(Node2D art, JsonElement body, JsonElement pose, string slot, string itemId, int facing, List<string> diagnostics)
private void ReconcileHands()
private void AddLayer(Node2D art, JsonElement body, JsonElement pose, JsonElement item, JsonElement layer, string slot, int facing)
private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer, string slot, string anchor, JsonElement anchors, Vector2 at)
private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing, Vector2 at, Vector2 size, float angle, JsonElement pack, JsonElement definition, string key)
private int LayerOrder(JsonElement layer, string slot, int facing)
private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)
private Vector2 Point(JsonElement value)
```

Existing public sealed partial InspectionRoot adds no public identity/member/signature. Only existing `private void Selection()` and `private void Recheck()` change: current character retirement, resource-read-key capture/definition error projection, and clearing stale Failures at a new read. Existing exact guard tuples remain `src/godot-game/inspection/InspectionRoot.cs|KingdomZombies.Inspection|InspectionRoot|Selection` and corresponding Recheck; new native failure-surface accesses are GetNode<TextEdit>(Failures)/Text read-write and GetNode<Label>(Summary/Status)/Text write, plus Recheck's Failures/Text clear. Native construction, installation and cleanup retain their guarded original owners.

Test identity KingdomZombies.Phase1.Tests.CharacterPresentationTests has14 public test methods expanding to35 cases and four private helpers; full current declarations:

```csharp
public void Native_InvalidDefinitionKeepsVisibleErrorAndTransport()
public void Native_ReplacementControlsHaveIndependentObservations()
public void Native_CharacterStatesAreCompleteAndDistinct()
public void Native_StrideContinuesDuringMelee()
public void AllBodies_KeepStrideDuringEveryWeapon(string body)
public void MixedPair_UsesSlowerIntervalAndSingleContact()
public void Flinch_LeavesBothClocksAndGenerationUnchanged()
public void Jump_ExposesAllFourRegisteredStages()
public void Elapsed_RejectsInvalidValues(double elapsed)
public void PublicBoundary_PreservesExactIdentity(string path, string type, bool allowed)
public void PublicBoundary_RejectsExpandedRegistry()
public void Definitions_RejectIndependentMalformedTracks(string defect)
public void Definitions_UseConfiguredTimingAndEvents()
public void Native_ExposesCompleteMeleeMatrix()
private static byte[] Data(string file)
private static string Root()
private static CharacterPresentation Create()
private static JsonDocument Native()
```

Exact visibility registry amendment adds only the CharacterPresentation identity to the original17; ArchitecturePolicy's independent expected set and compiled correct-path/wrong-path/adjacent-type/expanded-registry controls remain. Existing bodies-a FIT_FILES gains only the three actual animation/item/combat definitions plus Core fingerprint; FIT_HARNESS loads exact Core before game and authenticates both owning-context hashes. All original source/fit/helpers/counters/negatives remain intact.

animation.cjs exports INPUTS,files(),fingerprint(),HARNESS,REPLACEMENT_HARNESS,SCENARIOS,native(context,harness),run(context,replacement); prior() retains the inherited isolated environment. Actual registered cases are animation.melee (G0 order50) and animation.replacement (G0 order51). Input closure includes actual Root/scene/preview/Core/test/case and all three body fits/packs/atlases, both equipment packs/atlases, animation, item/combat and toolchain definitions. Native run-owned AnimationHarness:Node members are Invoke(object,string,params object[]),Create(Node2D),Select(Node2D,string,string),Part(Node2D,string),Sample(Node2D),Read(string),Check(bool,string),Gear(Node2D,string),Negatives(),Jump(Node2D,object),Pixels(Sprite2D,JsonElement,string,string),Body(Node2D,string,string,string,int),Grips(Node2D,string,string,string,string,int,double[]),Matrix(Node2D,object),_Ready(),Run(),Observe(bool,string),Layers(Node2D),Presentation(object),Contact(object,long),Button(Node2D,string),Prepare(Node2D,object),Operations(Node2D,object),State(bool,string,string),Replacement(). Its fields are _assembly,_type,_samples,_negatives,_restoredBindingAccepted,_replacementError,_replacementChecks,_states. All new C# string comparisons/dictionaries use explicit ordinal semantics; numeric formatting uses invariant culture.

Native report fields retain pid,nonce,assemblySha256,coreSha256,coreOwningAlc,owningAlc,strideAdvancesDuringMelee,before,after,samples,negatives,restoredBindingAccepted and add replacementError,replacementChecks,states. Each matrix row retains body,left,right,facing,stage,lower,interval,gameplayEvents. Each negative retains defect,rejected,exceptionType,message. replacementError records body,fixture,exceptionType,message,failures,errorVisible,transportUsable,recovered; expected caller catch leaves exceptionType/message empty while the actual visible diagnostic retains the real cause. Check rows record actual id/pass; state rows record id/pass/distinct setup. No passing field is fabricated for a missing scenario.

### Authored animation document, resources and bundles

Actual `content/animation/locomotion-melee.json`: schemaVersion1,id locomotion-melee,version1; obligations AN-01–AN-06 and bodies CH-01–CH-08; generation metadata; timing; registration; clips; exact bindings; pending acceptance. Five clip identities contain13 authored samples: idle1 neutral/idle; walk2 near/far walk-near/walk-far; faster2 drive/return run/walk-far; jump4 takeoff/airborne-up/airborne-down/landing run/walk-far/walk-near/run; melee4 windup/contact/follow-through/settle windup/strike/recover/idle. Sample fields duration,pose,stage,offset and melee angles (fists/knife/club/machete/bat) provide actual authored poses and registered grip rotations. Timing fields unit,meleeDurations,source,dual,contactFraction=.32,flinchSeconds=.12,weightShiftSeconds=.08,contactSeconds=.04,clock=explicit-elapsed-only,gameplayEvents=0 remain configured. Registration fields canvas[48,40],pivot[24,38],filter=nearest,logicalHandsNeverSwap=true,lowerIndependent=true,jumpBody=CH-01,jumpNote retain the registered body/grip contract. Generation fields tool,recipe,donorReferences[K14,K15,K18,K19],retainedSources[common-selected/bodies-a/bodies-b/bodies-c ZIPs],scaleDecision=D-13-pilot-1 and acceptance fields visual,owner,humanApproval,completedObligations remain honest authoring/pending records. Every binding row has exact path/sha256 fields.

Eleven exact bindings authenticate content/config/items.json,content/config/combat.json and a/b/c's content/animation/fits-*.json,content/packs/bodies-*.json,assets/characters/bodies-*.png. Current fists/either-hand knife/club/machete/bat and mixed accepted intervals come from DT04/DT05; AU03/AU04 release/impact remain configured observations, never gameplay hit or listening proof. This plan adds no raster asset, source ZIP/bundle member or Godot subresource ID; it composes the retained exact a/b/c and items-01-10/items-11-24 resources/production bundles from prior plans without modifying them.

### Exact native-policy tuples and producer effects for04-23

One fresh changed-source policy observation `.phase1/runs/c7f92490-bdbe-4575-851c-d752697a3e2a` remains **FAIL**, total414: BuildingPreview.cs=166, CharacterPreview.cs=170, IdentityKit.cs=78. It is expected unresolved integration evidence under existing WINDOWS30, not a Task2 behavior pass or native permission. Current CharacterPreview hosts (19) are `internal void Attach()`, `internal void Detach()`, `private void AnimationControls(Node slots)`, `private void Play()`, `private void Reset()`, `private void Frame()`, `private void RetireArt()`, `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)`, `private string Selected(string id)`, `private void Project()`, `private void Contact(Node2D art, JsonElement pose, int facing)`, `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,`, `private Vector2 SegmentOffset(bool lower)`, `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)`, `private void ReconcileHands()`, `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,`, `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,`, `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)`, `private Vector2 Point(JsonElement value)`. Full source/line/column/resolved member/host inventory is retained in `.phase4/04-08-task2-native-policy-observation.json`; all current CharacterPreview operations are also enumerated below. Root boundary stays within its existing signatures; no permission registry was changed.

| CharacterPreview source line:column | Exact declaring host | Resolved native member |
| --- | --- | --- |
| 104:9 | `internal void Attach()` | `Godot.CanvasItem.TextureFilter` |
| 104:31 | `internal void Attach()` | `Godot.CanvasItem.TextureFilterEnum.Nearest` |
| 105:24 | `internal void Attach()` | `Godot.ScrollContainer.ScrollContainer()` |
| 107:13 | `internal void Attach()` | `Godot.Node.Name` |
| 108:13 | `internal void Attach()` | `Godot.Control.Position` |
| 108:24 | `internal void Attach()` | `Godot.Vector2.Vector2(float, float)` |
| 109:13 | `internal void Attach()` | `Godot.Control.Size` |
| 109:20 | `internal void Attach()` | `Godot.Vector2.Vector2(float, float)` |
| 110:13 | `internal void Attach()` | `Godot.ScrollContainer.FollowFocus` |
| 112:9 | `internal void Attach()` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 113:21 | `internal void Attach()` | `Godot.VBoxContainer.VBoxContainer()` |
| 113:41 | `internal void Attach()` | `Godot.Node.Name` |
| 113:57 | `internal void Attach()` | `Godot.Control.SizeFlagsHorizontal` |
| 113:79 | `internal void Attach()` | `Godot.Control.SizeFlags.ExpandFill` |
| 114:9 | `internal void Attach()` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 135:9 | `internal void Attach()` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 135:30 | `internal void Attach()` | `Godot.Node2D.Node2D()` |
| 135:43 | `internal void Attach()` | `Godot.Node.Name` |
| 135:57 | `internal void Attach()` | `Godot.Node2D.Position` |
| 135:68 | `internal void Attach()` | `Godot.Vector2.Vector2(float, float)` |
| 136:9 | `internal void Attach()` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 136:30 | `internal void Attach()` | `Godot.Label.Label()` |
| 138:13 | `internal void Attach()` | `Godot.Node.Name` |
| 139:13 | `internal void Attach()` | `Godot.Control.Position` |
| 139:24 | `internal void Attach()` | `Godot.Vector2.Vector2(float, float)` |
| 140:13 | `internal void Attach()` | `Godot.Control.Size` |
| 140:20 | `internal void Attach()` | `Godot.Vector2.Vector2(float, float)` |
| 141:13 | `internal void Attach()` | `Godot.Label.AutowrapMode` |
| 141:28 | `internal void Attach()` | `Godot.TextServer.AutowrapMode.WordSmart` |
| 143:9 | `internal void Attach()` | `Godot.OptionButton.Select(int)` |
| 144:9 | `internal void Attach()` | `Godot.OptionButton.Select(int)` |
| 149:21 | `internal void Attach()` | `Godot.Node.GetTree()` |
| 150:13 | `internal void Attach()` | `Godot.SceneTree.ProcessFrame` |
| 164:13 | `internal void Detach()` | `Godot.SceneTree.ProcessFrame` |
| 170:13 | `internal void Detach()` | `Godot.OptionButton.ItemSelected` |
| 175:13 | `internal void Detach()` | `Godot.BaseButton.Pressed` |
| 258:9 | `private void AnimationControls(Node slots)` | `Godot.OptionButton.Select(int)` |
| 262:26 | `private void AnimationControls(Node slots)` | `Godot.Button.Button()` |
| 262:39 | `private void AnimationControls(Node slots)` | `Godot.Node.Name` |
| 262:50 | `private void AnimationControls(Node slots)` | `Godot.Button.Text` |
| 262:63 | `private void AnimationControls(Node slots)` | `Godot.Control.CustomMinimumSize` |
| 262:83 | `private void AnimationControls(Node slots)` | `Godot.Vector2.Vector2(float, float)` |
| 263:13 | `private void AnimationControls(Node slots)` | `Godot.BaseButton.Pressed` |
| 264:13 | `private void AnimationControls(Node slots)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 285:9 | `private void Play()` | `Godot.Button.Text` |
| 286:9 | `private void Play()` | `Godot.BaseButton.Disabled` |
| 302:9 | `private void Reset()` | `Godot.Button.Text` |
| 303:9 | `private void Reset()` | `Godot.BaseButton.Disabled` |
| 306:13 | `private void Reset()` | `Godot.OptionButton.Select(int)` |
| 327:25 | `private void Frame()` | `Godot.Node.GetProcessDeltaTime()` |
| 327:62 | `private void Frame()` | `Godot.OptionButton.Selected` |
| 334:22 | `private void RetireArt()` | `Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)` |
| 335:32 | `private void RetireArt()` | `Godot.Node.GetChildren(bool)` |
| 337:13 | `private void RetireArt()` | `Godot.Node.RemoveChild(Godot.Node)` |
| 338:13 | `private void RetireArt()` | `Godot.Node.QueueFree()` |
| 343:13 | `private void RetireArt()` | `Godot.GodotObject.Dispose()` |
| 500:19 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.VBoxContainer.VBoxContainer()` |
| 500:39 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.Name` |
| 501:9 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 502:9 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 502:28 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Label.Label()` |
| 502:40 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.Name` |
| 502:56 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Label.Text` |
| 503:23 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.OptionButton.OptionButton()` |
| 503:42 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.Name` |
| 503:59 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Control.CustomMinimumSize` |
| 503:79 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Vector2.Vector2(float, float)` |
| 504:9 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 507:13 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.OptionButton.AddItem(string, int)` |
| 510:9 | `private void AddSelector(Node parent, string id, string label, IEnumerable<string> values)` | `Godot.OptionButton.ItemSelected` |
| 530:16 | `private string Selected(string id)` | `Godot.OptionButton.GetItemText(int)` |
| 530:41 | `private string Selected(string id)` | `Godot.OptionButton.Selected` |
| 537:22 | `private void Project()` | `Godot.Node.GetNode<Godot.Node2D>(Godot.NodePath)` |
| 543:25 | `private void Project()` | `Godot.OptionButton.Selected` |
| 544:9 | `private void Project()` | `Godot.Node2D.Scale` |
| 544:21 | `private void Project()` | `Godot.Vector2.Vector2(float, float)` |
| 608:27 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Polygon2D.Polygon2D()` |
| 610:17 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Node.Name` |
| 611:17 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Polygon2D.Polygon` |
| 611:28 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 611:53 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 611:78 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 612:21 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 613:17 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Node2D.Position` |
| 613:28 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 613:57 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.X` |
| 613:69 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.X` |
| 613:78 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Y` |
| 614:57 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Vector2.Vector2(float, float)` |
| 615:17 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Polygon2D.Color` |
| 615:25 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Color.Color(float, float, float, float)` |
| 616:17 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.CanvasItem.ZIndex` |
| 618:13 | `private void Contact(Node2D art, JsonElement pose, int facing)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 641:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.Label.Text` |
| 641:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.Node.GetNode<Godot.Label>(Godot.NodePath)` |
| 642:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 643:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 644:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 645:9 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 648:13 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 649:13 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.Label.Text` |
| 649:13 | `private void ProjectGear(Node2D art, JsonElement body, JsonElement pose, JsonElement lower, int facing,` | `Godot.Node.GetNode<Godot.Label>(Godot.NodePath)` |
| 666:20 | `private Vector2 SegmentOffset(bool lower)` | `Godot.Vector2.Zero` |
| 673:13 | `private Vector2 SegmentOffset(bool lower)` | `Godot.Vector2.X` |
| 674:13 | `private Vector2 SegmentOffset(bool lower)` | `Godot.Vector2.Y` |
| 690:26 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Sprite2D.Sprite2D()` |
| 692:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Node.Name` |
| 693:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Sprite2D.Centered` |
| 694:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Sprite2D.Texture` |
| 695:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Node2D.Position` |
| 695:28 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Vector2.Vector2(float, float)` |
| 696:67 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Vector2.Vector2(float, float)` |
| 697:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Node2D.Scale` |
| 697:25 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Vector2.Vector2(float, float)` |
| 698:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.CanvasItem.ZIndex` |
| 699:17 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.CanvasItem.TextureFilter` |
| 699:33 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.CanvasItem.TextureFilterEnum.Nearest` |
| 701:13 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 702:13 | `private void AddBody(Node2D art, JsonElement pose, int facing, JsonElement lower)` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 756:41 | `private void ReconcileHands()` | `Godot.OptionButton.ItemCount` |
| 758:31 | `private void ReconcileHands()` | `Godot.OptionButton.GetItemText(int)` |
| 761:17 | `private void ReconcileHands()` | `Godot.OptionButton.SetItemDisabled(int, bool)` |
| 762:17 | `private void ReconcileHands()` | `Godot.OptionButton.SetItemTooltip(int, string)` |
| 810:13 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.X` |
| 816:13 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.Y` |
| 816:61 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.Length()` |
| 817:21 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.Angle()` |
| 821:13 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.X` |
| 822:13 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.Y` |
| 823:21 | `private (Vector2 Size, float Angle) FitGeometry(JsonElement body, JsonElement pose, JsonElement layer,` | `Godot.Vector2.Angle()` |
| 839:29 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Texture2D.GetImage()` |
| 840:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Image.Resize(int, int, Godot.Image.Interpolation)` |
| 840:34 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.X` |
| 840:55 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Y` |
| 840:78 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Image.Interpolation.Nearest` |
| 843:19 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Vector2(float, float)` |
| 843:34 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.X` |
| 843:80 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Y` |
| 844:31 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.ImageTexture.CreateFromImage(Godot.Image)` |
| 846:22 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Sprite2D.Sprite2D()` |
| 848:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Node.Name` |
| 849:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Sprite2D.Centered` |
| 850:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Sprite2D.Texture` |
| 851:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Sprite2D.Offset` |
| 851:23 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Round()` |
| 852:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Node2D.Position` |
| 852:24 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Vector2(float, float)` |
| 852:53 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.X` |
| 852:65 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.X` |
| 852:74 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Y` |
| 853:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Node2D.Scale` |
| 853:21 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Vector2.Vector2(float, float)` |
| 854:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Node2D.Rotation` |
| 855:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.CanvasItem.ZIndex` |
| 856:13 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.CanvasItem.TextureFilter` |
| 856:29 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.CanvasItem.TextureFilterEnum.Nearest` |
| 858:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 859:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 860:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 861:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.GodotObject.SetMeta(Godot.StringName, Godot.Variant)` |
| 862:9 | `private void AddFittedSprite(Node2D art, JsonElement item, JsonElement layer, string slot, int facing,` | `Godot.Node.AddChild(Godot.Node, bool, Godot.Node.InternalMode)` |
| 882:23 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.ResourceLoader.Load<Godot.Texture2D>(string, string, Godot.ResourceLoader.CacheMode)` |
| 892:72 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.Texture2D.GetWidth()` |
| 893:33 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.Texture2D.GetHeight()` |
| 898:22 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.AtlasTexture.AtlasTexture()` |
| 900:13 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.AtlasTexture.Atlas` |
| 901:13 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.AtlasTexture.Region` |
| 901:22 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.Rect2.Rect2(float, float, float, float)` |
| 902:13 | `private AtlasTexture Texture(JsonElement pack, JsonElement part, string atlas)` | `Godot.AtlasTexture.FilterClip` |
| 911:16 | `private Vector2 Point(JsonElement value)` | `Godot.Vector2.Vector2(float, float)` |

Producer observation `.phase4/04-08-task2-generated-input-observation.json` records actual current raw/text SHA256/byte lengths/content for49 source inputs and32 emitted surfaces, authenticated Godot.SourceGenerators4.7.2 packageSHA256 `ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e`, plus exact production DLL closure. Relative to Task1, changed source inputs are `src/godot-game/inspection/CharacterPreview.cs`, `src/godot-game/inspection/InspectionRoot.cs`; changed emitted surfaces: 0. There is no enrolled current cohort or generated replay pass; WINDOWS31 remains04-23/31-owned. Task1 observation files are preserved separately. Existing WINDOWS29 current pack/index/provenance remains04-23/31-owned; this bounded Root change does not adopt other presenters or pack outputs.

## Exact tested and committed content

All nine Git-filtered working files equal HEAD blobs. The raw tested checkout identities and committed blob identities are separately recorded below and in `.phase4/checkpoints/04-08-task2-final/manifest.json` with exact backups. CharacterPreview checkout retains CRLF while Git stores LF; equality is filtered-content equality, never a claim of raw byte equality. Original399-input freeze and original Task1 artifacts are retained unchanged except the five declared Task2 paths.

| File | Checkout bytes | Tested raw SHA256 | Committed blob SHA256 |
| --- | --- | --- | --- |
| `src/core/CharacterPresentation.cs` | 18213 | `effb18514498bb6e582e1a587622a4019fdf0a90c2e48d2a6abd20eca2ff2f9e` | `effb18514498bb6e582e1a587622a4019fdf0a90c2e48d2a6abd20eca2ff2f9e` |
| `src/godot-game/inspection/CharacterPreview.cs` | 40690 | `141e35c97bbb1ce60cebc5e21c70c76a4cfef89e4304c14a31220cf903a8a157` | `489353f4f81cdef7039e944a34b650cac433f5cc0495f95fe8dd57b5662383e7` |
| `src/godot-game/content/animation/locomotion-melee.json` | 6318 | `12e5a1cd705fef3d18dae2a99472be2871252409d4ea68debbf241341cb4e7ee` | `12e5a1cd705fef3d18dae2a99472be2871252409d4ea68debbf241341cb4e7ee` |
| `tests/phase1/CharacterPresentationTests.cs` | 18412 | `e172bd4b6ec3c9ecb0fdae5f6b71f8fdf0d68713ccfb35c7c19c7da6a6b9b678` | `e172bd4b6ec3c9ecb0fdae5f6b71f8fdf0d68713ccfb35c7c19c7da6a6b9b678` |
| `tools/phase4/cases/animation.cjs` | 40877 | `372b012ce554c4b89daa93444cb8d4f867d2e432f5087b6bf52ab9c66ccea3de` | `372b012ce554c4b89daa93444cb8d4f867d2e432f5087b6bf52ab9c66ccea3de` |
| `docs/code/visibility-exemptions.json` | 5173 | `dd3d8d88ae4c85e2a178b939b92929286ceb59fd5dab73008da660fe9b51c779` | `dd3d8d88ae4c85e2a178b939b92929286ceb59fd5dab73008da660fe9b51c779` |
| `tests/phase1/ArchitecturePolicy.cs` | 16572 | `3fe9341b1d642e57b6a1574b19618f6ce3291cd2e57300343df4510b1f8f4319` | `3fe9341b1d642e57b6a1574b19618f6ce3291cd2e57300343df4510b1f8f4319` |
| `tools/phase4/cases/bodies-a.cjs` | 39749 | `17bda622ec4504584f14c432bab65d28a561af71de96a2668463b687f2faa9de` | `17bda622ec4504584f14c432bab65d28a561af71de96a2668463b687f2faa9de` |
| `src/godot-game/inspection/InspectionRoot.cs` | 21090 | `fa15e2bbe11269baded4eb0e1cfa5a08005a777a621eda6e89a19997280968ed` | `fa15e2bbe11269baded4eb0e1cfa5a08005a777a621eda6e89a19997280968ed` |

Metrics use realized nine-path source diff characters/4 (125714/4, rounded up), not runtime/model token use. Measured commits come from the persisted plan ledger before `0f44c7e4fd9a93b6e8c68ef967a21f4d40f3c262` through production HEAD `9da8f8879f9704267311cf99d8dc947ad2b28c92`; the later SUMMARY commit is excluded by that recorded interval. Duration 89.48min is the recorded evidence window from first genuine RED08:03:45.627Z through this close-out, including prior checkpoints/failed attempts; it is not a claim of precise original pre-RED work time.

## Known Stubs and remaining acceptance

No behavior-blocking stub, skipped compiled test or unrun automated plan verification remains. Empty collections/defaults are owned runtime stores and guarded no-data states, not mock UI data. Explicit pending actual-input/image/listening/named appearance and ASSET/full-phase acceptance belongs to04-32/33 after whole-phase independent review. No requirement is marked complete here. Existing WINDOWS29/30/31 remain independently open; resolved local fixture/compiler deviations do not erase or close those integration defects.

## Self-Check: PASSED

All nine artifacts exist, match the frozen raw source identities and Git-filtered committed blobs, and have byte-identical durable backups. Task1 bc7883d, both genuine RED commits and Task2 production commit are reachable ancestors. Both final animation native exit records are0 with exact matching PID/nonce/DLL/context identities, both full35-test JUnit reports have zero failures/skips, all16 selected result records pass and source drift is empty. No tracked deletion or foreign path was staged. Foreign global planning/spec/Phase06/Phase12 work is preserved. Global STATE/ROADMAP/requirements/PR updates remain parent-owned after this canonical SUMMARY commit.
