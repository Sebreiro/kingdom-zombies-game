# Plan 04-21 technical checkpoint — preserve existing event publication

**Status:** Task 1 complete; Task 2 awaiting a prospective coordinator repair scope. No canonical SUMMARY, Task 2 commit or final 47-case selection exists yet. This is a technical checkpoint, not another request for the creator's source approval.

Task 1 GREEN commit is `3df8209f4bc4a22b505fe1a24ec462199f0ae98b`. Its fresh same-case committed tracer `aa09e941-4cbb-442d-ad2b-af3cadccd72c` passed. R1 used its initial implementation/verification and **0/2** corrective attempts. Original Task 1's historical **3/3** corrections and all failed listening reports remain preserved. Exact creator/user attestation “All is approved” covers only the previously presented forty sources/forty-four version1 adaptations. Personal name, device/player, hearing time and detailed observations remain null; no new or final native-mix approval is inferred.

## Exact remaining issue

Original Task 2 has used **3/3** corrections:

1. Correct Python dictionary ordering that overwrote the short retained source IDs with decoder IDs. Reassembly used the same decoded sources and changed no audio.
2. Correct canonical routing/gain/UI semantics: delivery uses handling rather than implying coin pickup; fire/wind/forest gains are0.3/0.2/0.35; save/content messages useUI22/23/26/27 and outcomes useUI24/26. Independent literal assertions accompany the authored catalog comparison.
3. Apply the prospectively authorized exact canonical64MiB retained-ZIP reader/publication allowance from `04-21-ARCHIVE-AMENDMENT.md` at coordinator commit `8c5295a04e7a0d96ded02ff2600d2286372dc3b4`, preserving ordinary32MiB, member32MiB, expanded128MiB, containment/singlelink/hash/count and all PCM rules.

During correction3, `--publish-interface` successfully validated/copied the exact archive and wrote an immutable `publication.json`, verifying the known partial bank and manifest without changing them. It **then failed** at `write(ROOT,EVENTS,e)` because the shared `content.cjs` writer uses exclusive creation and `src/godot-game/content/audio/events.json` was already correctly authored. Observed diagnostic:

```text
Error: EEXIST: file already exists, open 'D:\Work\Games\KingdomZombies\src\godot-game\content\audio\events.json'
    at write (...\tools\phase4\content.cjs:201:94)
    at publishInterface (...\tools\phase4\cases\audio.cjs:414:3)
```

The helper should preflight/preserve exact existing event bytes and avoid a redundant exclusive write. The archive now also exists, so any resumed helper must preserve/verify its exact known identity rather than delete or blindly overwrite it. **No fourth correction has been made.** A prospective narrow R2 must precede source edits. All four Task 2 media/pack/catalog artifacts must remain byte-for-byte fixed; only the exact helper/preflight logic and necessary tests may change under that scope.

## Concrete verified Task 2 artifacts

All23 isolated donor decoders succeeded. There are27 new version1 variants, including five stereo8second loops with exact352800frame loop lengths, source trim[0,353312) and512frame tail/head overlap. Complete full original WAVs, raw FSB payloads, AudioClip records, locators, tool/source hashes and recipes are retained. The one synthesized food-contact texture follows the explicit AU19 source-catalog gap; no unheard donor was declared unsuitable.

The DT23 catalog contains all30 canonical categories and91 explicit routes, including variants/buses/gain/attenuation/priority/cooldown/repetition, configurable24global/four-per-effect and80/80/60 preferences, current-loop reconciliation, historical-one-shot suppression, visible warning pairs and correct hostile-portal/cosmetic-acid semantics. Existing context-sensitive consumer classification and full index/provenance publication compatibility remain04-23/31-owned.

Every one of these27 new variants remains **Not auditioned**, with null listener/verdict. The actual source-only review is prepared at `.phase4/audio-interface-review-v1/review/index.html`; the same review is retained in the ZIP. No playback, actual hearing, game window, native mix or final32/33 acceptance is claimed.

| Exact file | Bytes | SHA256 at this checkpoint |
| --- | --- | --- |
| tools/phase4/cases/audio.cjs |73514| `efa77debd561599434a2c5991aacada0ef3db240510f121d938568006f32d71f` |
| src/godot-game/assets/audio/interface-ambience.wav |13107808| `e61d445d30ab28d65d163ea89fcc185484b4746f35703cb2ed088f13308ba5b9` |
| src/godot-game/content/packs/audio-interface-ambience.json |115893| `2e107c4ef6eb57262a8f571a1226b694f552a15b806117bcb67e6be7ddfda069` |
| src/godot-game/content/audio/events.json |123935| `375b8199b0b8b0ff168f6cb5894cf5139bcbdb97fc081c4bee0329bcfe948d05` |
| production/phase4/audio-interface-ambience.zip |64820348| `cfa52aeac32a22fcf4898e05fa64a6225016af404ae990632ffd07fb130fb1d2` |

The bank has3276941frames. The ZIP has125members/81440487expanded bytes; its largest member is16228844bytes, `sources/s2125/original.wav`. These are within the exact64MiB compressed/member32MiB/expanded128MiB contracts. The complete exact125member/source/variant inventory is in the retained production manifest and the tracked pack's `bundleMembers`.

## Preserved proof and snapshots

The saved `audio.bank` case **passed** in invocation `36c7cb7b-8b83-4d78-aa2d-b8baee3700da` at actual HEAD `8c5295a04e7a0d96ded02ff2600d2286372dc3b4` with the exact hash-recorded correction3 WIP above. It verifies all125members/full source WAV/raw hashes, all27 exact recipe reproductions, nineteen semantic/PCM negative controls and nineteen restored positives. It also records **eleven** actual reader/publication outcomes, including real64MiB/64MiB+1 and32MiB/32MiB+1 files, wrong-case/lookalike/other-path handling, independently expected wrong-hash rejection, restored positives and destination-selected publication. These zero-filled boundary fixtures establish the reader bounds; the separate full valid125member check establishes actual ZIP validity.

Exact snapshots of all five Task 2 files above are stored under `.phase4/04-21-task2-c3-checkpoint/`, mirroring repository-relative paths. Its `checkpoint.json` records all hashes, original budget, source HEAD and evidence references. Its `evidence/` copies the saved result, frozen inputs, negative controls, archive-reader controls and actual publication record. Original reports remain untouched:

- `.phase4/runs/36c7cb7b-8b83-4d78-aa2d-b8baee3700da/`
- `.phase4/audio-publication/a1993fbb-ad84-41dc-8b09-b555ba2ed69e/publication.json`
- Original decoder/preparation `.phase4/audio-interface/b379502c-aad5-4b74-bb2a-ae697c7bfd9d/`
- Source-ID corrected assembly `.phase4/audio-interface-c1/8e3b10b3-9001-498d-927c-7b637c17b033/`

No Task 2 source edit follows this checkpoint until the prospective R2 scope is committed. No global STATE/ROADMAP/REQUIREMENTS/PROJECT, initial specification, native policy, compiler/cohort source, other agent work or existing source evidence is changed. No package install, branch switch, merge, reset, stash, broad stage or push occurred. The prepared final47selection has not run and supplies no pass credit.
