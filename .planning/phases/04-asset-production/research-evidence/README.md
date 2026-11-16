# Phase 04 research evidence

These files establish selected donor-format feasibility only. They do not certify production content, fitting, audio audition, Windows export, any AC/H/UI backstop, or the four judgment-tier prohibitions.

## Retained observations

- `donor-probe.json`: fresh original-file identities (32 matching files), six exported sprite records, four embedded font payload identities, seven decoded clip metadata objects, initial multi-owner audio results and errors.
- `audio-isolation-probe.json`: five fresh bounded decoder subprocess results, each loading only its owning archive. All five decoded to PCM WAV metadata successfully. This corrects the inference that the earlier FMOD errors meant those source recordings were unsupported. It does not establish the root cause of the original batch failures.
- `format-probe.json`: actual runtime streamed keys and resolved pointer mappings for seven clips, plus an independent constrained PCM16 sample-count/hash check for three effects. The streamed terminal infinity is represented by a descriptive sentinel string so this report remains strict JSON.
- `clip-*.json`: original decoded runtime clip trees. These retain pointer bindings, streamed/dense/constant data, authored times/loops and original events as source evidence. Excluded donor callbacks are not approved production events.
- `*.png` and `*-sprite.json`: native selected sprite exports and decoded source metadata. Logical rectangles and exported trimmed sizes differ; these are not finished building or neutral-body resources.
- `research-plan-input.json`: documentation questions submitted to the GSD research-plan seam. The seam requested unavailable Context7; official web fallback and cached digests are documented in `04-RESEARCH.md`.

## Repeat probes

Use the explicit existing bundled Python interpreter with `-B` to avoid writing bytecode in the donor dependency tree. From the repository root:

```powershell
& C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe -B .planning/phases/04-asset-production/research-evidence/probe_donors.py
& C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe -B .planning/phases/04-asset-production/research-evidence/probe_formats.py
& C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe -B .planning/phases/04-asset-production/research-evidence/probe_audio_isolation.py
```

The first probe must precede the format probe because it creates selected raw FSB payloads used by the second. Audio isolation runs independently. Both raw FSB and temporary PCM-WAV research payloads were removed after gathering the reports; the scripts recreate them. Do not treat generated probe samples as selected final assets or an auditioned bank.

The scripts read the preserved recovery root and installed decoder dependencies. They write only their own containing evidence directory; do not run the original donor exporter unchanged, because it writes to its own repository. The final game's runtime and clean content checks must use this project's packaged selected exports and never require the recovery root.
