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

## Latest observation

On 2026-11-19 the creator approved a separate batch of at most 64 fresh imports of the captured animation project from run `0ed41a9d-e6b7-4b15-a180-c68b6beb9dbb`. The batch reached the cap. Each process exited 0 naturally with empty stderr, both pipe EOFs, debugger detachment, and one observed parser entry. None emitted the Unicode failure. Each regenerated the same 44 imported files byte for byte as the original run. The 64 verified archives and raw evidence are indexed in the [Plan 04-23 checkpoint](../phases/04-asset-production/04-23-CHECKPOINT.md).

The malformed buffer, its caller, and ownership remain unobserved. Instrumentation can affect timing and the original environment values and pre-import cache are unavailable. This selection is closed. The original failure remains open and the clean-output gate is still blocked.

## Prior investigation focus

hypothesis: Not established. Varying diagnostic code units may originate from a UTF16 conversion of native data during import; fixed corrupt text, retained-font metadata, resource/assembly lifetime, and native import concurrency remain competing candidates.
bug_class: heisenbug-mandelbug (provisional; unchanged text fixture has dirty and clean executions)
test: STOPPED finalR14 exact64actualimports/noevent and twoNONIMPORTcontrols. Terminalaudit .phase4/04-23-import-debug/r14-terminal-report-a4a81480-b1f0-462e-9d9a-5ae934977a15.json,646602B/SHA3b87eae90fe36499266cc9bab98a837b44d6a71e67276aa1acb1aa0de1925dbe;661indexedrawfiles independentlyhashverified. Elevated supervisor session47292 ended0; no active ownedtarget.
expecting: Rootcause remainsunconfirmed; no causalR11.2fixture repair followsfrom64cleaninstrumentedimports. Actualfirstemittingcaller/buffer/provenance/lifetime evidence isstillmissing. FinalAUTO-RESUME3/3 hardcap prohibits anyfurthernativecommand.
next_action: Parent reviews terminalR14report and precise unresolvedfirst-event requirement; retain investigating/STOPPED, originalR9NOT_GREEN, independentui-inventorynative129gategap and allhistories. No furthernative launch, fourthautomaticcontinuation, archive, sourcefix, Git/PR or acceptance.
candidate_causes:
  - data: malformed retained text or font naming records enter a UTF16 decoder.
  - code: native decoder caller reads invalid lifetime or wrong-length bytes.
  - config/environment: concurrent importer or editor font enumeration exposes a native defect.
and_gate: possible; valid retained data plus concurrent native processing could be jointly required; no contributing cause confirmed.
probe_selection: All selections CLOSED. R14 final54b240f AUTO-RESUME3/3 exhaustedexact64 actualimports(22audio/21world/21text)/twoNONIMPORTcontrols/noUnicodeevent. No additional nativecommand, fourthautomaticcontinuation, sourcefix, acceptance, newtools or globals. Previous oldseven+reserved/R12exact64/R13zeroimports-onefailedsmoke remainseparate preservedhistories.

## Scope and ownership

Diagnosis only. Debugger/session manager owns this file and ignored .phase4/04-23-import-debug/ artifacts. ALL tracked code/data/producers/14archives/Core/tests/ENEXP/currentmodel/Root/index/tracer/registry and foreignPhase07/08 work stayfixed. No commits/push/PR/phaseglobal changes. R11 exactauthority is final04-23-REPAIR.md section. Do not ask creator routine technical questions; symptoms/authorization are already provided. Return causal findings/minimal exact repair responsibility or concrete missing technical evidence. Parent resumes existing executor after diagnosis; no productionfix yet.

## Evidence

- timestamp: 2026-11-19 R14 terminal and independent static audit
  checked: r14-terminal-report-a4a81480-b1f0-462e-9d9a-5ae934977a15.json,646602B/SHA3b87eae90fe36499266cc9bab98a837b44d6a71e67276aa1acb1aa0de1925dbe; immutable supervisor terminal285262B/SHAac52139831038c5154b25ef15d8178741e73afef2b340d07bd8ca504025a699d;661indexedrawfiles rehashed.
  found: FinalR14exact64actualimports=22audio/21world/21text;64naturalexit0/64ready/64realfirstappendentry/64empty stderr/64bothactualEOF/64detach/64heldownedhandleinactive;0Unicodebranch/output/0applicablediagnosticlines/0inputmutations/0boundsfailures. Durations5078–5438ms,maxnative-exitelapsed5282ms,maxstream9438B. Currentfull13fileproductionclosure matchesretainedproductionclosures inallthreesubjects; no rebuild. Cwd78/project95–96 and actualinheritedprofile/AppData/shared.phase1temp/cache fingerprints retained prelaunch. Onlyfirst-entrysamplelengths retained(-1×64), not fullentrycounts/distributions. TwoofatmostfourpreregisteredNONIMPORTsmokes: naturalexactversion/readiness/nativeattribution625ms; intentionalsetupfailure-ownedsuspendedPID22188TaskKill0/250ms,bothEOF+detach250msfromcleanupanchor,notresumed. Allsource/DLL/helperfixedhashesmatch. Originalaudio39/world6Unicodelines/R12/R13reporthashes preserved; noauto-reviewrejection/Git/PR/global/sourcefix/controller/case/ALCacceptance.
  implication: Setup/cleanupreadiness provenforR14 but nativecauseNOTfound. SelectionCLOSEDat64;finalAUTO-RESUME3/3 permitsNOmoreautomaticnativecommands. Preciseunresolvedrequirement is actualemittingappend_utf16caller/thread/stack/registers and originalpointer/signedlength/readableextent/NUL/firstinvalidoffset plusproducerallocation/ownership/release lifetime. Missinghistoricalenv/cache cannotbereconstructed; cleanretry/pathprofiledifference/validsource doNOTselectlength/NUL/lifetime/font/Windows/managed/concurrencyfix. Independentsharednative129Unicodegategap isnecessaryR11.2work, notcausalrepair.

- timestamp: 2026-11-19 R14 midpoint
  checked: First30actualimports under active r14-final-8a37b14a-a6e5-46fc-8039-5d54c6c354a9 selection; actual launch envelopes and native events.
  found: Ten eachaudio/world/text, allnaturalexit0/ready/bothEOF/detach/noUnicodeevent/noinputmutations. Actualcwd78/project95–96/currentUSERPROFILEandAppDatainherited; shared.phase1/temp/cli-home/packages/http-cache used; LANG/LC_ALL currentlyC.UTF-8. Historicalenvironment/cacheprebytes remainunknown. Realappend_utf16firstentry andnativeWriteFile stacks retained, not hypotheticalhookinstallation.
  implication: Intendedcurrenthelperpath/profileboundary isnowobserved, but absentfailure stilldoesnotidentifycause. Fullentrycounts/distributions werenotselected andcannotbereconstructed; onlyemittedfirst-entrysamples exist. Continue onlyactivefinalselection tofirstevent/failure/64.

- timestamp: 2026-11-19 R14 control sequence
  checked: r14-final-8a37b14a-a6e5-46fc-8039-5d54c6c354a9/control-selection.json and fresh smokes PID35616/PID22188; whole supervisor launched require_escalated from outset.
  found: Two of at mostfour preregistered NONIMPORT version smokes executed. Natural PID35616 exact4.7.2 version/monitor-ready/realnativeWriteFile stack/exit0/bothEOF/detach passed625ms. Intentionalnegative suspendedchild PID22188 was notresumed, ownPID/exeSHAverifiedTaskKill plus bothEOF/detach passed562ms total. Zero import/casepasscredit. Separate maximum64actualimports preregistered onlyafterbothcontrols passed; first audio clone PID11980 natural0/5156ms/noevent with unchanged source/assembly closure and cwd78/project95/currentinheritedhelperenvironment.
  implication: R13setup/permissioncleanup defect avoided for this exact new selection. Proceed only active final R14 finite cohort; actual Unicode origin remains unconfirmed and observer timing/ASLR/allocator effects persist.

- timestamp: 2026-11-19T02:22:37Z
  checked: R13terminalreport-e4fcaee1-5d9a-4d3b-8c3b-b1571fbe40ad.json,17401B/SHA10c242c0cf69e286dec33df90b87c1f9ae38e3cbcce6485093763352824ecd0b; static source/syntax and ownedPIDquery.
  found: R13actualimports0,helperbaseline0,cohortnotpreregistered,onefailedseparate--versionsmoke. Fresh PID6608 spawned02:18:21.834Z; CModule rejected undeclaredgum_process_get_current_thread_id at02:18:22.036Z beforemonitor-ready/resume. Taskkill5s commandexit1/Accessdenied02:18:22.060Z; laterpermissions-onlysametaskkill exit0/0.186scommandduration. Smoke closeddetachedafter35313ms, outside8snormalfinalclose; originalcallback lackedreason/timestamp andbothEOFflags, so0/0bytesareNOTstreamEOFproof. PIDabsent inGet-Process02:19:26.009Z andOpenProcesserror87 at02:22:37.078Z. Noauto-reviewrejection. Initialmonitor/smokesource/rawfailure retained; poststopstaticcorrections useexplicitWindowsGetCurrentThreadId symbol/externalstorageaddress semantics/ownedcontrolbounds andretainEOF/8sclose, but onlyNode--check/PythonAST pass, NOcorrectedruntimevalidation. Sixhelper/check/audio/world/text/defenses pluscurrentGame/Core hashes allmatchprior; no trackedimplementation/engine/sourcefix. R12report/boundaryhashesunchanged.
  implication: R13stopcondition isbinding despite0imports. Parent disposition/newprospectiveselection isneeded for anynextnativecommand; first requirevalidatedobserverAPI andsamepermissioncontextownedcleanupwithin5s/8s/bothEOF, thenactualhelperpath/currentenvcapture. Observer failure doesnotdiagnoseoriginalUnicodecause.

- timestamp: 2026-11-19T02:07:06Z
  checked: Completed R12cohortde897d30-da73-4823-bfd7-0563c049a516 and terminalreport19873f91-99df-4343-8f6a-8f9aeb32c240.json.
  found: Exactly64 actualnativeimports/64wrappers (22audio/21world/21text), allnaturalexit0/4859–5219ms/empty stderr/bothEOF/sessionclosed/noinputmutations;64ready+64realliveentryvalidations, zeroUnicodebranches/output. AllownedPIDfinalqueriesinactive. Separate R12history:two--versionsmokes (onepre-resumeobservercontrolfailure/cleanup0, onecleanvalidation), oneacquisitionnetworkrefusal thenapprovedsuccess. Noauto-reviewrejection. Report412304B/SHA c91d96e1dac1512ed9d69619dde87cc4b844175915c198c84b0215b6cfbab9c2 retains everycommand/rawlog/source/DLL/hash/PID/nonce/timing. Original32501BreportstillSHA28335cc10acf5f65c7860b150a65cb2ad5671c26156e164783f19e084bf4feef.
  implication: R12capexhausted;noextraimports. Instrumentation readiness is proven, actualcausalstack/bufferisnot. NoR11.2causalrepair, cleanacceptance ororiginalR9closure followsfromabsence.
- timestamp: 2026-11-19T02:07:05Z
  checked: Read-only reproduction-boundaries-db6cb419-3fb4-413e-9307-46c37bd68193.json,223756B/SHAfe5d532466f367290e6a511f778950337725a530b7a1840a99eb66334d7a4d97.
  found: Originaldirtycwd78chars/project95–96 versusR12cwd168–169/project176–177. Originalhelpers isolated() inheritparenthome/profile/APPDATA/XDG andshare.phase1/temp;R12runner.environment usesfreshownedhome/profile/data/config/cache/temp andstripsruntimeprefixesbeforedeclaredoverrides. Historicalenvvalues/editorcacheprelaunchbyteswerenotrecorded;currentselectedparent/OSlocale(en-US)/editorstatesarelabelledcurrentorpostrunonly. Bothcommandsomitexplicitworker/languageflags;selectedprojectsettingsshownoworker/localeoverride. Eachchildfirstentrylength=-1(64events);fullentrytotals/lengthdistributionswerenotcapturedbeforeexitandremainunknown.Noextraattach/rerunperformed.
  implication: These are uncontrolledreproductionboundaries, not demonstratedcauses. Every-entryJScallbacks also perturb scheduling; faithfulfailing-context/lowertimingimpact capture is a prospectivetechnicalrequirement, not a guessedengine/controllerfix.

- timestamp: 2026-11-19T02:01:13Z
  checked: First17 completed R12imports in cohortde897d30-da73-4823-bfd7-0563c049a516.
  found: Sixaudio/sixworld/fivetext imports naturallyexit0 with empty stderr,4937–5219ms, bothstreamEOF, nofixtureinputmutations. Monitorrecords realappend_utf16 entrypointer/length/caller and safereadablerange in everyrun. No actualUnicodebranch/output yet; allselectedinputs unchanged.
  implication: Continue only predeclared finitefirst-eventselection; absence cannotprove cause/fix. No correctiontoR11.2 helpers justified.

- timestamp: 2026-11-19T01:58:07Z
  checked: Fresh smoke37542533 PID28904 after only observer-owned memory validation correction.
  found: Exactengine4.7.2.stable.mono.official.ed1daf0bf output captured with native WriteFile stack/registers and moduleRVAs; mappedappend_utf16/three diagnostic hooks installed onx64; ownsyntheticUTF16 validator correctlyfindsinvalidunit1. Naturalexit0/172ms/38stdoutbytes/emptystderr/bothEOF/sessionclosed. Original smoke37336 remainsseparate. No nativeimports yet.
  implication: Begin separatelypreregistered at-most64 first-eventimports. Temporaryhook timing is diagnosticobservereffect, never cleanacceptance.

- timestamp: 2026-11-19T01:56:58Z
  checked: R12 official tool acquisition, offline authenticated PE/string/unwind/disassembly map, first smoke0438e9d2 PID37336.
  found: Pinned Frida17.5.1 SHA7efdeb7e95c8e025517e203c2d1121c9eb07c9a3f33a1312ed5960c757e2fe98; pefile2024.8.26 SHA76f8b485dcd3b1bb8166f1128d395fa3d87af26360c2358fb75b80019b957c6f; capstone5.0.6 SHA761c3deae00b22ac697081cdae1383bb90659dd0d79387a09cf5bdbb22b17064. Exact exe diagnostic function RVA0x3efaa00 is String::append_utf16 in pinnedsource, correcting previous parse_utf16 name. Diagnostic branches0x3efaad4/0x3efacb0/0x3efad40. First --version smoke attached but failed own-control check before resume; cleanup taskkill0/closedstreams, no import. No engine input/return altered. Acquisition network refusal then successful approved escalation are separate setup records.
  implication: Native instrumentation works; correct only ignored observer validation, retaining failure/source history before fresh smoke. No causal import finding yet.

- timestamp: 2026-11-19
  checked: R12 prospective authority, original diagnosis report, exact runner implementation and official Frida installation/JavaScript API guidance.
  found: Coordinator approved new finite first-event instrumentation selection. Bundled Python is3.12.14/AMD64. Official Frida supports Python user-space attachment and process/module/register/backtrace/read-range APIs. Configured debugger agent-skills query returned no skills; original report and all histories remain fixed.
  implication: Proceed with task-local authenticated tool acquisition; --version smoke remains separate from native import count and all observer effects must be labelled.

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

root_cause: unconfirmed; finalR14exact64actualimports yieldednoemittingevent despite64ready/realappendentryvalidations at String::append_utf16 RVA0x3efaa00/branches0x3efaad4/0x3efacb0/0x3efad40. Actualfailingcaller/buffer/producerownership notobserved. R13observerAPI/setup/cleanup defects remainseparate fromoriginalUnicodecause; R14boundedcontrols passed.
fix: not applied; diagnosis-only
verification: INVESTIGATION INCONCLUSIVE. FinalR14STOPPEDat64/64actualimports(22audio21world21text),allnatural0/empty stderr/ready/realentry/bothEOF/detach/heldhandleinactive/unchangedinputs,0Unicodediagnostics. TwoNONIMPORTcontrols passed including250msownedsuspendedcleanup/250msbothEOFdetach. Report646602B/SHA3b87eae90fe36499266cc9bab98a837b44d6a71e67276aa1acb1aa0de1925dbe with661rawfileshashverified,fullcurrent/retained13fileassemblyclosuresmatch. PriorR13zeroimports-onefailedsmoke,R12exact64andoldseven+reservedremainseparateCLOSEDhistories. Final3/3NOmoreautomaticnativecommands. Need actualemittingcaller/pointer/signedlength/readableextent/NUL/invalidoffset/producerallocation-and-release evidence; sourcefixunselected. OriginalR9NOT_GREEN/107compiledtestedidentity/independentui-inventorynative129gategapremain. Noarchive/DEBUGCOMPLETE/production/game/hearing/platform/ASSET/Task1acceptance.
files_changed: debugmetadata/ignoredprobesonly
