---
status: investigating
trigger: "Phase 04 Plan 23: intermittent Unicode surrogate diagnostics during official Godot fixture import prevent clean verification"
created: 2026-11-19
updated: 2026-11-19
mode: diagnose-only
---

## Symptoms

- Expected: existing official Godot 4.7.2 headless fixture imports complete naturally with clean positive logs, followed by unchanged native predicates and proper cleanup.
- Actual: frozen whole62 verification executed61assertedpasses/oneworldimportfailure; raw99positiveenginecommands contain TWOdirtyimports. Current107compiledcasespass. No wholeR9/Task1/ASSET acceptance.
- Errors: audio.heldout6edd3316 emitted39varying UTF16 unpaired-surrogate diagnostics at inventory-native-import(exit0), then792controller+15Hubassertionspassed. world.inspector958dd583 emitted6surrogate diagnostics, correctlyfailedWORLD_INSPECTOR_IMPORT_CLEAN beforeprobe. Exacterrors/inputs/commands/rawlogs in r9-native-log-audit-62.json and r9-final-checkpoint.json under .phase4/04-23-preflight/.
- Timeline: during currentrepairedfrozenR9selection, after earlierfocusedtext.support-native06e6d4a8 emitted3similar diagnostics; oneunchangedfreshsupportae6f7b6apassedclean, originalfailurepreserved. Other97positivecommands inwhole62auditclean. Earliertest-fixtureFoundationclosure/rawImagewarnings/audio-retirement defects separatelyresolved; do not conflate those with currentUnicodefailure.
- Reproduction: inspect saved failedproject/command/inventory-native-import.stderr.log in .phase4/runs/6edd3316-3e54-44b7-9ebe-340bb8a17f5b and inspector-import logs/project in .phase4/runs/958dd583-8b52-4bc8-be2e-c82e87717822. Use controlledfreshdisposableclones/currentauthenticengine and recordexactdifferences; no blindretryuntilclean or alteredtoolchain/sourceassets.

## Current Focus

hypothesis: Not established. Varying diagnostic code units may originate from a UTF16 conversion of native data during import; fixed corrupt text, retained-font metadata, resource/assembly lifetime, and native import concurrency remain competing candidates.
bug_class: heisenbug-mandelbug (provisional; unchanged text fixture has dirty and clean executions)
test: Inventory retained dirty and clean fixture inputs, engine identity, helper isolation and native source of exact diagnostic before selecting bounded probes.
expecting: Fixed malformed input predicts a matching malformed UTF16 or name record; runtime/native defect predicts valid identical input with varying diagnostic units or a native conversion path.
next_action: Coordinator reviews diagnosis-report.json and scopes native failing-caller/buffer attribution (or an explicitly prospective further diagnostic selection). No more native launch in this selection; no source repair or clean-import closure is justified yet.
candidate_causes:
  - data: malformed retained text or font naming records enter a UTF16 decoder.
  - code: native decoder caller reads invalid lifetime or wrong-length bytes.
  - config/environment: concurrent importer or editor font enumeration exposes a native defect.
and_gate: possible; valid retained data plus concurrent native processing could be jointly required; no contributing cause confirmed.
probe_selection: Approved finite eight-clone observation only (not acceptance or a corrective retry). Subject is retained dirty audio native-inventory input; tracked current sources are never built or altered. Copy all retained candidate input files except postrun proof results, generated .uid/.import and generated .godot cache; preserve .godot/mono prebuilt data. Record complete source/DLL closure and original-source provenance per clone. Observable variable is --verbose plus stream chunk timestamp instrumentation. Default import parallelism unchanged. Use real runner.command 60000ms/1048576-character-per-stream/5000ms-taskkill/8000ms-final-close bounds. Per-clone environment uses real runner.environment with existing .phase1 package/home caches read paths and owned temp/profile directories; this differs from historical inherited values, so it is not an exact environment replay. No import/assets/scripts skipped. Stop after8 regardless of results; stop earlier on setup/child/output/cleanup failure. No native debugger available in PATH or standard Windows Kits path; no install authorized. Negative result yields precise caller-stack requirement, not causal closure.

## Scope and ownership

Diagnosis only. Debugger/session manager owns this file and ignored .phase4/04-23-import-debug/ artifacts. ALL tracked code/data/producers/14archives/Core/tests/ENEXP/currentmodel/Root/index/tracer/registry and foreignPhase07/08 work stayfixed. No commits/push/PR/phaseglobal changes. R11 exactauthority is final04-23-REPAIR.md section. Do not ask creator routine technical questions; symptoms/authorization are already provided. Return causal findings/minimal exact repair responsibility or concrete missing technical evidence. Parent resumes existing executor after diagnosis; no productionfix yet.

## Evidence

- timestamp: 2026-11-19T01:44:52Z
  checked: Completed finite observation cohorts eda13efa-6349-437d-816b-0ba7103dad4a and ce962c1d-0b74-4005-a0cb-cf9fde6f3096.
  found: Seven attributable official imports PIDs33608/29064/11240/10664/28412/24108/16328 exited0 with empty stderr,4740–4835ms commanddurations, unchanged selected source/DLL bytes, real runner.command bounds preserved. Actual7native+2setup failures=9wrapperattempts; no-spawn recovery counted separately. First maskedsetup conservatively reserves1possible native slot, so7attributable+1reserved=8absolute maximum. All observations and original failures retained. No diagnostic event occurred; no controlled bad/restored subject or controller/ALC assertion was executed.
  implication: Observation cap reached; clean absence does not identify cause or fix. Stop further launches.
- timestamp: 2026-11-19
  checked: Final durable diagnosis-report.json,32501bytes,SHA28335cc10acf5f65c7860b150a65cb2ad5671c26156e164783f19e084bf4feef.
  found: Native cause remains unconfirmed. Retained/current Game DLL011777e98c7c6a653554fc28d119f67a9828851c7de8cd2632e7ac70a970f754 and CoreDLLd900ff3e9fe45e72e80e537bd3f983e5f57b872f9cca34a0f400f7f006c402d1 match; no freshcompilecredit. Endhashes for check.cjs/ui-inventory.cjs/audio.cjs match starting exactR11subjects. Existing .phase1 PDBs found are managed; native debugger absent from checked PATH/standard location. Exact source links, raw evidence fingerprints, source/DLL closures, PIDs/nonces/times, setup histories and limits are in the report.
  implication: Missing evidence is first failing native parse_utf16 caller/module/thread plus actual pointer/length/extent/NUL/firstinvalidoffset and buffer ownership/lifetime. Prospective debugger/symbol/instrumentation or broadermatrix scope is required before outside-path/toolchain/newmeaning actions. No creator judgment gate was invented.

- timestamp: 2026-11-19T01:43:50Z
  checked: Same-command permission-only escalation, observations-eda13efa-6349-437d-816b-0ba7103dad4a/probe-1-a7deca8c-8a6f-438b-9013-1f4ae576c10d.
  found: Official engine import PID33608 ran4831ms, exit0, empty stderr, unchanged selected source/DLL hashes. This is first attributable native observation. No controller assertions or ALC observations were run. Two earlier setup failures remain separate, first conservatively reserved because original exception/PID was masked.
  implication: Native execution is available through permitted escalation; this clean import provides no causal conclusion. Remaining finite observation cap is6, yielding7 attributable imports plus1reserved <=8.

- timestamp: 2026-11-19
  checked: Instrumented setup recovery observations-a824e38f-8ecb-4f7a-9ff1-4d30db0fbe35/probe-1-9e91876b-43b2-4640-a89c-3318a1603b5a.
  found: spawn-exception.json records synchronous ChildProcess.spawn EPERM before return/PID; zero native launch for this measured attempt, zero events and no source mutations. Original uninstrumented attempt remains conservatively unclassified. Both setup attempts are retained separately; zero completed native observations.
  implication: Permission boundary, not game or font behavior, prevented observation. Same command equivalent escalation is authorized; do not count this as an import failure or clean result.

- timestamp: 2026-11-19T01:42:45Z
  checked: First observation setup and static no-spawn boundary recovery.
  found: First ignored cohort observations-45be1873-d659-4936-8503-44b955b3ea6d/probe-1-ca9d7e8b-b0f1-4ed0-9695-ecbacffd37ca contains prelaunch closure and empty events only. A catch attempted to read missing import.stderr.log and masked original exception; source error-recording correction retained. setup-recovery.json uses a spawn stub that cannot launch a process and shows same context/env passes helper validation to check.cjs169 spawn boundary. No new attributable completed engine command exists; original launch status is unproved and conservatively consumes one possible launch slot. No Godot process remained at process inventory.
  implication: Recover exact spawn/setup error with immediate durable instrumentation before resuming already-selected observations; cannot count first setup as clean/native proof.

- timestamp: 2026-11-19T01:38:00Z
  checked: static-inspect.py and static-input-comparison.json in ignored diag-20261119-utf16 directory.
  found: Retained text-dirty and text-clean have125 identical non-cache candidate inputs; only postrun native-mix-retirement.json differs. Five fixtures have zero invalid UTF8/JSON-surrogate findings. All3 retained TTF name tables strict-decode as valid UTF16 (30/30/26 records) with identical font hashes across dirty and clean cases. Exe45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab matches authenticated ZIP entry; ZIPa2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0 matches pinned manifest.
  implication: Fixed malformed source text/JSON/font naming records do not explain variable diagnostics in checked fixture inputs. This does not exclude native use of valid font/resource data.
- timestamp: 2026-11-19
  checked: Official Godot source at ed1daf0bf001b61586d9930840f2f1394092c079, core/string/ustring.cpp and Windows/text/import callers.
  found: Exact pair-context messages originate in String::parse_utf16 first pass; String::utf16 later emits the plain Unpaired surrogate form. Audio preserves13 diagnostic units over three blocks; world2 units over three blocks; text1 unit over three blocks. Original logs do not expose caller or shared stream timing. Direct shell source download failed with network refusal; official browser source remains readable. No engine/source was changed.
  implication: Native caller attribution, rather than source text encoding conversion, is the next discriminating evidence.

- timestamp: 2026-11-19
  checked: Bootstrap and knowledge-base availability.
  found: No configured gsd-debugger agent skills were returned; no debug knowledge-base file or MemPalace MCP tool is available. KB fallback has no candidate. SBFL is inapplicable because this intermittent native import has no per-test native coverage.
  implication: Continue direct native diagnostic tracing; do not treat prior R10 lifetime evidence as a Unicode diagnosis.
- timestamp: 2026-11-19
  checked: Retained command envelopes for audio/world/text imports.
  found: All use authentic declared Godot_v4.7.2-stable_mono_win64.exe with --headless --path <fixture> --editor --import --quit and 60000ms timeout. Dirty inventory PID37004 took4268ms; dirty inspector PID34336 took4545ms; dirty text PID8092 took4364ms. Same audio case's later Hub import PID34280 was clean. Logs expose separate stdout/stderr without event timestamps, so original error timing within import cannot be recovered from line order across streams.
  implication: Neither exit0 nor successful later controller assertions explain errors. Need event-attributed fresh evidence if timing becomes diagnostic.

- R9 final checkpoint: .phase4/04-23-preflight/r9-final-checkpoint.json,57681B,SHA9863dcb4094b4b303f96211c89fa18f3e75bfff843bcd05de68ee019d028a172.
- Final raw audit: .phase4/04-23-preflight/r9-native-log-audit-62.json,6858B,SHA9c4b356f12c015a61eb842d3efff56e59258739fbbebbb767b92f0fcf870713f.
- Shared import discovery: .phase4/04-23-preflight/r9-import-scope-discovery.json,4352B,SHA5a2c7ad4805454151f6619dfc020fc836921a6599834aa88dc10e9343027b9fa.
- Shared helper: tools/phase4/cases/ui-inventory.cjs57452B,SHAf3e2600d352ab3e3f019eeeb82153052e86885a83944f8619c69ef4062301246,native()129. Existinggate onlyERROR/SCRIPTERROR missesUnicode diagnostics; addinggate alone does not repair actualcause. Audio callerread-only5528877886c4063f837f6fbe99df7d987a4feb7d66598c57a818ea2f890070c4.

## Eliminated

- hypothesis: Fixed malformed UTF8/JSON or retained TTF name-table UTF16 bytes directly cause the intermittent diagnostics.
  evidence: Five retained fixtures strict-decode clean; dirty/clean text candidate input hashes match125files and all TTF name records match/validate. Reported malformed units vary across failures.
  timestamp: 2026-11-19

## Resolution

root_cause: unconfirmed; source of varying bad native UTF16 buffer not attributed
fix: not applied; diagnosis-only
verification: Inconclusive. Seven boundedclean observations do not repair or clear original audio/world/text failures. Gategap independently confirmed but not causalexplanation. OriginalR9NOT_GREEN remains.
files_changed: debugmetadata/ignoredprobesonly
