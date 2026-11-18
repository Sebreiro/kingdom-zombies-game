---
phase: 04-asset-production
plan: "21"
status: blocked-listening
gate: blocking-human
tasks-completed: 0
canonical-summary-created: false
source-head-before: e34e60d0fa22346a0fbc38786e582ec79723fcfb
recorded-at: "2026-11-18T03:05:00Z"
---

# Phase 04 Plan 21 — Real source audition required

Task 1 has a concrete, technically checked candidate review. It is **incomplete** because no capable listener has actually heard these exact recordings. All 40 donor recordings and 44 adapted variants are **Not auditioned**. No donor has been declared unsuitable, no replacement has been synthesized, and no AU category or ASSET requirement is accepted. Task 2 has not started. The canonical `04-21-SUMMARY.md` remains absent so dependent work is not falsely released.

The exact plan prerequisite is Task 1's acceptance criterion: **“missing real audition blocks completion.”** Its action requires actual candidate audition and sample-version verdicts. Source-only listening is permitted without a game window. This preparation requests only that source audition; final native mixing, loop/current-state behavior and 04-32/04-33 judgments remain later work. The available session has no supported audio listening/evaluation tool. Decoder success, PCM statistics, a playhead or a playback command supplies no hearing verdict.

## Prepared review

The agent has extracted the tracked [audio-actions.zip](../../../production/phase4/audio-actions.zip) into this local, disposable review folder:

`D:/Work/Games/KingdomZombies/.phase4/audio-review-04-21-v1/`

Open [the prepared review page](../../../.phase4/audio-review-04-21-v1/review/index.html). It has deliberate Play controls for the **complete source** and the **adapted variant**, exact category questions, source owner/object/name, version, trim/gain, integer bank regions and the full adapted WAV SHA256. Nothing autoplays. The human need not extract, install, run commands or open a game. The same page is retained as `review/index.html` inside the ZIP, alongside `review/README.txt` and the complete `review/manifest.json`.

The stereo [candidate bank](../../../src/godot-game/assets/audio/actions.wav) is also directly playable: 2,498,522 frames, 44,100 Hz, PCM16, 56.65582766439909 seconds, 9,994,132 bytes. Its final 50ms separator is outside all sample regions. Individual sources and variants are preferable for final sample-specific verdicts.

The [tracked candidate manifest](../../../src/godot-game/content/packs/audio-actions.json) is the complete 44-row source/version/region/hash matrix. It records original frame count/channel/rate/PCM and WAV identities, exact owner/stream byte offsets and hashes, raw AudioClip identity, tool identities, before/after original preservation, adapted integer regions, trim, gain, channel transform, loop=null and explicit null listener/verdict. All source objects are `resources.assets` / `AudioClip`; donor root is read-only `D:/Work/Games/KindomCrown/recovered/resources`. The five mono sources are 2880, 3700, 3131, 3554 and 3811; original WAVs stay mono. Their candidate bank variants duplicate each mono channel to stereo without resampling. Every other source retains stereo.

## Review questions

For each unique full source and variant, listen to the entire sample and report music, speech, unwanted source identity, silence, audible clipping/distortion, onset/tail and suitability. Assess D-31 sharper swing/stronger impact/noticeable confirmation **by hearing**. The headroom-only normalization is not a claim of punch or final mix balance. Shared recordings need only one verdict per exact version; category suitability still needs the relevant classification below.

| Category | Exact donor object candidates | Question |
| --- | --- | --- |
| AU-01 footsteps / carrying / retreat | 3294, 4949 | Human footfalls rather than hooves; suitable walk/sprint cadence? |
| AU-02 jump | 3294, 4949, 120ms trims | Do the short trims read as takeoff/landing without voice? |
| AU-03 melee swings | 3369, 4872 | Which actual swing portions fit knife/machete/club/bat/fist; which material detail is missing? |
| AU-04 flesh / armor hits | 5123, 2880, 3700 | Classify actual flesh/armor contact; any inappropriate creature detail? |
| AU-05 worn-out equipment | 4523, 3574, 140ms trims | Does each trim read as a small crack/clink rather than a building collapse? |
| AU-06 bow release | 3288 | Is the release short and sharp without unrelated handling? |
| AU-07 arrow first contacts | 5476, 3640, 4434, 5020, 3357, 4692 | Classify actual terrain/wood/metal/prey/portal contact. |
| AU-08 spit | 2790 | Is any wet portion usable without fire/explosion identity? Give exact source seconds, or reject with actual heard reason. |
| AU-09 zombie actions | 2864, 3150, 3131, 2804, 3554, 2880, 3700 | Which movement/attack/hit/death portions suit a shared undead bank; which retain inappropriate creature identity? |
| AU-10 survivor damage / death / succession | 5123, 5736 | Any restrained suitable death/succession portion without music or crown-loss implication? |
| AU-11 prey | 3944, 4247, 3811, 3357 | Confirm deer/rabbit identity and flee/hit/death suitability. |
| AU-12 paid building / repair | 5272, 3945, 3804, 5176 | Classify wood/metal work contacts and variant usefulness. |
| AU-13 tree mark / chop / fall | 4611, 3741, 3672, 4523, 5654 | Which mark/chop/fall portions fit without implying coin yield or item pickup? |
| AU-14 build / upgrade start and completion | 2862, 4543, 4245 | Suitable noticeable single start/completion cues; any unwanted musical content? |
| AU-15 defense impacts / collapse | 4169, 4041, 4523, 5654, 3574, 5154 | Classify wood/metal impact and collapse tails. |

Particularly uncertain audition locations in the playable bank (seconds are display aids; the exact integer frame regions in the manifest are authoritative):

| Sample version | Exact region [start,end) | Bank seconds | Full adapted WAV SHA256 |
| --- | --- | --- | --- |
| action.3294.jump-trim v1 | [11025,16317) | 0.250000–0.370000 | 6dab2930a03d460622ccfc798c26a9f42703431bf745275f36e2d9e0352ce529 |
| action.4949.jump-trim v1 | [29547,34839) | 0.670000–0.790000 | e24b68cebc164da8becff97bf84934a04b37a7a7db630ada6dd210b358e47fdd |
| action.4523.break-trim v1 | [320431,326605) | 7.266009–7.406009 | c950c877b886f310a4abda508d73a7ab667234c175902fe9e14ef54721d3be9c |
| action.3574.break-trim v1 | [449820,455994) | 10.200000–10.340000 | 30c62d0b4042b6d0027174f4a1ff911f761b00273601b0b629ae9b5cda6bfc56 |
| action.2790.full v1 | [526748,750533) | 11.944399–17.018889 | 9b29b74a94784a96307282820866e3be9731670dc32e9baa2ddd3bdbcb53f4cb |
| action.5736.full v1 | [1118721,1315329) | 25.367823–29.826054 | 70146316c11af62a9cd9b676f111127c9e1dd62917593a45663b4cfed60b3b17 |

Record actual listener name, UTC time, audible output device/player, sample ID/version/WAV hash, entire-source/variant hearing, observations and `accepted`, `changes-required` or `blocked` verdict. For proposed edits, give exact source seconds and the heard reason. The coordinator can transcribe supplied real observations into immutable sample-version evidence. Missing source coverage remains unknown rather than a rejection. Synthesis is allowed only for a gap supported by actual relevant donor review. These source verdicts do not approve a future game mix.

## Executed technical evidence

- Corrected preparation: `.phase4/audio-candidates/02a9625c-3c1c-4590-9208-b01fb2153d96/`. Forty independently owned decoder children succeeded, preserving each command/exit/stdout/stderr and rehashing the exact donor owner/stream originals before and after. Every full source WAV, compressed FSB stream payload, raw AudioClip object and source/tool/locator record is retained in the ZIP, not only in this ignored folder.
- `node tools/phase4/cases/audio.cjs --technical`: PASS. Forty distinct full source PCM identities, 44 adapted variants, all 15 exact canonical AU category contracts, integer non-overlapping frame regions, precise channels/rate/duration/trim/gain, no truncated payload, and no full-scale output PCM samples. This establishes mathematical integrity, not absence of audible distortion, silence suitability or audible identity.
- `node tools/phase4/check.cjs --case audio.actions`: **FAIL, expected unresolved human prerequisite**, at original HEAD `e34e60d0fa22346a0fbc38786e582ec79723fcfb`, invocation `f2a0d153-bf3c-4674-acc3-889fdfb28d13`, started `2026-11-18T03:03:57.641Z`. Its `technical.json` passes all integrity checks first; `result.json` then reports exactly `AUDIO_LISTENING_REQUIRED AU01-15 exact source and adapted candidate versions are Not auditioned; Task1 remains incomplete`. No pass credit, GREEN commit or committed tracer is claimed.
- Retained ZIP independently checks 210 exact members, safe bounded paths/counts/expanded bytes, full member hashes, raw source/object hashes, complete WAV/PCM lengths, original metadata, retained manifest rebound equality and exact reproduction of every adapted trim/gain/channel transform.
- Thirteen deliberate controls reject with exact intended diagnostics: wrong category (`AUDIO_CATEGORY_IDS`), known-valid wrong source (`AUDIO_SOURCE_IDENTITY`), wrong bank hash (`AUDIO_BANK_HASH`), region rebound (`AUDIO_REGION_HASH`), fractional frame (`AUDIO_REGION`), wrong rate/channel (`AUDIO_SAMPLE_FORMAT`), fabricated listener (`AUDIO_FALSE_AUDITION`), unsupported donor rejection (`AUDIO_FALSE_DONOR_REJECTION`), wrong category binding (`AUDIO_CATEGORY_BINDING`), original mutation (`AUDIO_ORIGINAL_MUTATION`), false peak (`AUDIO_TECHNICAL_METADATA`), truncated bank (`WAV_HEADER`). Twelve restored positive validations pass after the first twelve mutations. Original production bytes remain unchanged.

| Exact tracked artifact | Bytes | SHA256 |
| --- | --- | --- |
| tools/phase4/cases/audio.cjs | 26686 | 057782f69c697862ad34ca1ad4fe5ebb2a956ebb877ded28ee39b5fd553f187c |
| src/godot-game/content/packs/audio-actions.json | 207612 | c22236ff9c79fa9224bf15e725cb254f633fc1a8108012b9e299441e5cde442a |
| src/godot-game/assets/audio/actions.wav | 9994132 | fb173a9863cbc2ba1989cea83afddea3497d22bc760ba28803b9d5e8a545e144 |
| production/phase4/audio-actions.zip | 15837100 | e205665e8cf3bb386e2d4743c87b63db35fcd54e041f0c997baf43b2f0cded85 |

The decoder recipe hash is `5e0b103e4ecf2591f00744b5c7f0c53a3fa576c807f9def10805e90197a33189`; assembly recipe hash is `72b6d91ffdd0d52d8fcf5407d578e1d6001e67948e818753324ec2d2b34416c4`. The unchanged inherited extraction template is `tools/phase4/extract-audio.py` SHA256 `2f0a2e9a243bc8f240b9231f459e626e96f33c7532808b80c2d87809fce5169f`.

## Artifacts and continuation inventory

Only the original four Task 1 implementation paths and this coordinator-authorized planning checkpoint are changed. No native/C# source, policy, other pack, global planning document, initial specification, recovered input or existing evidence was edited.

`audio.cjs` adds constants `PYTHON`, `PACK`, `BANK`, `ZIP`, independent `CANDIDATES`/`COVERAGE`, embedded `ASSEMBLE`/`ZIP_CHECK`, and functions `ids()`, `sourceChannels(id)`, `decoderRecipe()`, `inputs()`, `prepare()`, `validate(pack,bank)`, `technical(context)`, `actions(context)`. CommonJS exports are `CANDIDATES,COVERAGE,decoderRecipe,prepare,validate,technical,actions`. Standalone commands are `--prepare` and `--technical`; registered case is `audio.actions`, Critical, source/headless group `audio`, G0 order121. Exact native-policy tuples introduced: **none**.

The ZIP contains `sources/<objectId>/original.wav`, `stream.fsb`, `audioclip.bin`, `provenance.json` for each of the 40 exact object IDs in the category matrix; `variants/action.<objectId>.full.wav` for all forty; jump trims for 3294/4949 and break trims for 4523/3574; `recipes/decode.py`, `recipes/assemble.py`, `recipes/selection.json`; `review/README.txt`, `review/index.html`, `review/manifest.json`. Every exact member path/hash/byte count is enumerated in manifest `bundleMembers` (the retained manifest is the final additional member, avoiding a self-hash cycle). All 44 version1 sample resource/region IDs are enumerated in manifest `samples`. No generated prompt, layered sound, selected loop, music or dialogue bank is claimed.

The candidate validator deliberately accepts only this unauditioned preparation contract and makes the missing listener fail. A continuation must consume genuine supplied hearing observations, validate their exact source/sample versions and adapt/classify or synthesize actual supported gaps before implementing the approved-production contract. Do not change this gate to pass by recording an invented reviewer or treating technical statistics as hearing.

## Original repair budget and preserved history

Two of the original maximum three Task 1 correction attempts have been consumed; the budget is **not reset** at this checkpoint. No third correction was needed after the technical pass.

1. First genuine preparation `c8cce052-40ae-4b53-a30e-1e2973723366` failed `AUDIO_OUTPUT_ROOT` before decode: the copied recipe's file-relative root resolved under its ignored run directory. Correction1 pinned the actual authorized workspace root while retaining all containment/reparse checks.
2. Preparation `98259d44-b136-4203-a4fc-908d8d3cc069` reached donor2880 and failed the inherited stereo-only `AUDIO_FORMAT` assumption. A read-only actual AudioClip metadata inspection established exactly five mono originals and all forty 44100Hz PCM16 source records. Correction2 now verifies the exact expected native source channels, source duration within one frame, original PCM preservation and explicitly reproduced mono duplication for the stereo bank. The corrected complete preparation is `02a9625c-3c1c-4590-9208-b01fb2153d96`.

Initial sandbox child creation returned `spawn EPERM`; approved equivalent elevated decoder/check commands succeeded. This environment restriction is not RED or a correctness correction. No approval rejection, authentication gate, install, deadline/output-bound increase, hidden stderr, native game window, playback or actual hearing occurred.

No C# changed, so the plan's conditional solution build is inapplicable. The final full selected source/headless set has **not** run because both tasks are incomplete. Historical prior45 evidence is not new 04-21 credit. Actual `audio.bank`/Task2 files remain absent. All known 23/31 integration/policy issues and final32/33 acceptance remain with their existing owners.

Resume only after actual source audition input. Complete Task1 and its exact GREEN commit/fresh same-case committed tracer before beginning Task2. Preserve the original correction budget and stop before a fourth technical correction unless prospectively amended. The parent owns global state/progress, continuation, existing draftPR5/push and all subsequent integration; this checkpoint advances none of them.
