---
phase: "07"
slug: "camp-economy-and-development"
status: draft
nyquist_compliant: false
wave_0_complete: false
created: "2026-11-18"
---

# Phase07 validation strategy

Draft execution contract only. No Phase7 implementation,test,review,rendered or release evidence is asserted. The exact24-plan/49-task map below replaces the provisional family map. [07-COVERAGE.md](07-COVERAGE.md) enumerates all22 global requirements,25 local R,48 AC,168 individual edges and18 D decisions with concrete assertions/witnesses. Validation flags stay false until actual execution audit.

## Infrastructure and verified command provenance

| Property | Contract |
| --- | --- |
| Test project | Existing tests/phase1/KingdomZombies.Phase1.Tests.csproj; xUnit/Shouldly; no sixth project or Godot project reference. |
| Existing strict build | `dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q` |
| Existing authority regression | `node tools/phase3/check.cjs --case core.authority`; inherited movement authority only. |
| Existing documentation | `node tools/phase1/check.cjs --group docs` |
| New Phase7 CLI | tools/phase7/check.cjs,produced by07-01-T2 after actual delivered Phase5/6 read. No current unowned --phase7 flag. |
| New flags | --list,--required,--case,--group,--headless,--rendered,--selection; exact dispatch/report contract in07-01. |
| Real production driver | Actual Phase6 normal entry,bindings and codec captured in07-CONTRACTS by07-01-T1. FoundationSession movement/CampLayouts samples are patterns only. |
| Independent enumeration | tools/phase7/required-cases.json is independent of discovered handlers and includes every concrete EID case,tracer/config/review/acceptance ID. |
| No-pass contract | Unavailable prerequisite,zero cases/assertions,missing required handler/peer/result/image,timeout/crash,stale source/review or absent required observation cannot pass. |
| Runtime | Unmeasured; Wave0 measures focused headless shards with bounded deadlines,target below60 seconds without dropping required checks. |

## Exact per-task verification map

Each command assumes repository root. Every runnable task command has an immediately following fails_when in its PLAN. Feature cases are new outputs; before07-01 they are not executable and provide no acceptance credit. Their explicit producer dependency replaces invented existing runner support.

| Plan/task | Wave | Source coverage | Required cases / exact command | Test/evidence | Threat refs | Producer/file state | Status |
| --- | --- | --- | --- | --- | --- | --- | --- |
|07-01-T1|0|All predecessor/shared production contracts|`node tools/phase7/prerequisites.cjs --assert-delivered`|Read-only authentic delivery/source/evidence audit|T-07-01|New audit/07-CONTRACTS produced here|Pending|
|07-01-T2|0|Independent all-source cases and honest evidence|`node tools/phase7/check.cjs --group runner --headless`; `node tools/phase7/check.cjs --list --required`|Known-bad missing/zero/stale/no-peer controls plus real clean assertion control|T-07-02|New CLI/bridge/required-cases/runner-tests/manifest produced here|Pending|
|07-02-T1|1|ECON-03, ECON-05;R01;E001, E002, E003, E004, E165, E166|`node tools/phase7/check.cjs --case camp.time.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-03,T-07-04|New CampTimeTests.cs and time.cjs; CLI producer07-01-T2|Pending|
|07-02-T2|1|ECON-03, ECON-05;R01;E001, E002, E003, E004, E165, E166|`node tools/phase7/check.cjs --group camp.time --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved time.json scenarios|T-07-03,T-07-04|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-03-T1|2|WORLD-05, NPC-06;R02, R13;E005, E006, E007, E008, E009, E010, E011, E082, E083, E084, E085, E086, E087, E088|`node tools/phase7/check.cjs --case camp.territory.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-05,T-07-06|New CampTerritoryTests.cs and territory.cjs; CLI producer07-01-T2|Pending|
|07-03-T2|2|WORLD-05, NPC-06;R02, R13;E005, E006, E007, E008, E009, E010, E011, E082, E083, E084, E085, E086, E087, E088|`node tools/phase7/check.cjs --group camp.territory --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved territory.json scenarios|T-07-05,T-07-06|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-04-T1|3|ECON-01, NPC-01, NPC-02;R03, R08, R09;E012, E013, E014, E015, E016, E017, E018, E047, E048, E049, E050, E051, E052, E053, E054, E055, E056, E057, E058, E059, E060;D-09,D-12|`node tools/phase7/check.cjs --case camp.recruits.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-07,T-07-08|New CampRecruitmentTests.cs and recruits.cjs; CLI producer07-01-T2|Pending|
|07-04-T2|3|ECON-01, NPC-01, NPC-02;R03, R08, R09;E012, E013, E014, E015, E016, E017, E018, E047, E048, E049, E050, E051, E052, E053, E054, E055, E056, E057, E058, E059, E060;D-09,D-12|`node tools/phase7/check.cjs --group camp.recruits --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved recruits.json scenarios|T-07-07,T-07-08|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-05-T1|4|NPC-01, NPC-02, NPC-03, ECON-02, BUILD-02;R24;E153, E154, E155, E156, E168;D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09|`node tools/phase7/check.cjs --case camp.camp-window.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-09,T-07-10|New CampWindowTests.cs and camp-window.cjs; CLI producer07-01-T2|Pending|
|07-05-T2|4|NPC-01, NPC-02, NPC-03, ECON-02, BUILD-02;R24;E153, E154, E155, E156, E168;D-01,D-02,D-03,D-04,D-05,D-06,D-07,D-08,D-09|`node tools/phase7/check.cjs --group camp.camp-window --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved camp-window.json scenarios|T-07-09,T-07-10|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-06-T1|5|NPC-03;R10;E061, E062, E063, E064, E065, E066, E067;D-06,D-07,D-08,D-09,D-12,D-17|`node tools/phase7/check.cjs --case camp.roles.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-11,T-07-12|New NpcRoleTests.cs and roles.cjs; CLI producer07-01-T2|Pending|
|07-06-T2|5|NPC-03;R10;E061, E062, E063, E064, E065, E066, E067;D-06,D-07,D-08,D-09,D-12,D-17|`node tools/phase7/check.cjs --group camp.roles --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved roles.json scenarios|T-07-11,T-07-12|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-07-T1|6|NPC-04;R11;E068, E069, E070, E071, E072, E073, E074|`node tools/phase7/check.cjs --case camp.equipment.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-13,T-07-14|New NpcEquipmentTests.cs and equipment.cjs; CLI producer07-01-T2|Pending|
|07-07-T2|6|NPC-04;R11;E068, E069, E070, E071, E072, E073, E074|`node tools/phase7/check.cjs --group camp.equipment --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved equipment.json scenarios|T-07-13,T-07-14|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-08-T1|7|NPC-05;R12;E075, E076, E077, E078, E079, E080, E081|`node tools/phase7/check.cjs --case camp.supplies.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-15,T-07-16|New NpcSupplyTests.cs and supplies.cjs; CLI producer07-01-T2|Pending|
|07-08-T2|7|NPC-05;R12;E075, E076, E077, E078, E079, E080, E081|`node tools/phase7/check.cjs --group camp.supplies --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved supplies.json scenarios|T-07-15,T-07-16|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-09-T1|8|NPC-07, BUILD-01, BUILD-03, ECON-01;R14, R16;E089, E090, E091, E092, E093, E094, E095, E103, E104, E105, E106, E107, E108, E109;D-12,D-15,D-17,D-18|`node tools/phase7/check.cjs --case camp.clearing.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-17,T-07-18|New CampClearingTests.cs and clearing.cjs; CLI producer07-01-T2|Pending|
|07-09-T2|8|NPC-07, BUILD-01, BUILD-03, ECON-01;R14, R16;E089, E090, E091, E092, E093, E094, E095, E103, E104, E105, E106, E107, E108, E109;D-12,D-15,D-17,D-18|`node tools/phase7/check.cjs --group camp.clearing --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved clearing.json scenarios|T-07-17,T-07-18|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-10-T1|9|BUILD-02, BUILD-03, BUILD-04, NPC-02;R17, R18;E110, E111, E112, E113, E114, E115, E116, E117, E118, E119, E120, E121, E122, E123;D-12,D-15,D-18|`node tools/phase7/check.cjs --case camp.tiers.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-19,T-07-20|New CampConstructionTests.cs and tiers.cjs; CLI producer07-01-T2|Pending|
|07-10-T2|9|BUILD-02, BUILD-03, BUILD-04, NPC-02;R17, R18;E110, E111, E112, E113, E114, E115, E116, E117, E118, E119, E120, E121, E122, E123;D-12,D-15,D-18|`node tools/phase7/check.cjs --group camp.tiers --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved tiers.json scenarios|T-07-19,T-07-20|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-11-T1|10|BUILD-04, BUILD-02, BUILD-03;R19;E124, E125, E126, E127;D-12,D-15,D-18|`node tools/phase7/check.cjs --case camp.upgrades.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-21,T-07-22|New CampUpgradeTests.cs and upgrades.cjs; CLI producer07-01-T2|Pending|
|07-11-T2|10|BUILD-04, BUILD-02, BUILD-03;R19;E124, E125, E126, E127;D-12,D-15,D-18|`node tools/phase7/check.cjs --group camp.upgrades --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved upgrades.json scenarios|T-07-21,T-07-22|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-12-T1|10|BUILD-05, NPC-07, ECON-01;R20;E128, E129, E130, E131, E167;D-12,D-15,D-16,D-18|`node tools/phase7/check.cjs --case camp.repair.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-23,T-07-24|New CampRepairTests.cs and repair.cjs; CLI producer07-01-T2|Pending|
|07-12-T2|10|BUILD-05, NPC-07, ECON-01;R20;E128, E129, E130, E131, E167;D-12,D-15,D-16,D-18|`node tools/phase7/check.cjs --group camp.repair --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved repair.json scenarios|T-07-23,T-07-24|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-13-T1|11|BUILD-06, BUILD-03, BUILD-04, WORLD-05;R21;E132, E133, E134, E135, E136, E137, E138;D-12,D-15,D-18|`node tools/phase7/check.cjs --case camp.rebuild.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-25,T-07-26|New CampRebuildTests.cs and rebuild.cjs; CLI producer07-01-T2|Pending|
|07-13-T2|11|BUILD-06, BUILD-03, BUILD-04, WORLD-05;R21;E132, E133, E134, E135, E136, E137, E138;D-12,D-15,D-18|`node tools/phase7/check.cjs --group camp.rebuild --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved rebuild.json scenarios|T-07-25,T-07-26|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-14-T1|8|ECON-03, NPC-06;R05;E026, E027, E028, E029, E030, E031, E032;D-17|`node tools/phase7/check.cjs --case camp.gathering.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-27,T-07-28|New CampGatheringTests.cs and gathering.cjs; CLI producer07-01-T2|Pending|
|07-14-T2|8|ECON-03, NPC-06;R05;E026, E027, E028, E029, E030, E031, E032;D-17|`node tools/phase7/check.cjs --group camp.gathering --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved gathering.json scenarios|T-07-27,T-07-28|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-15-T1|3|ECON-05, ECON-04;R07;E040, E041, E042, E043, E044, E045, E046|`node tools/phase7/check.cjs --case camp.wildlife.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-29,T-07-30|New CampWildlifeTests.cs and wildlife.cjs; CLI producer07-01-T2|Pending|
|07-15-T2|3|ECON-05, ECON-04;R07;E040, E041, E042, E043, E044, E045, E046|`node tools/phase7/check.cjs --group camp.wildlife --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved wildlife.json scenarios|T-07-29,T-07-30|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-16-T1|8|ECON-04, NPC-05, NPC-06;R06;E033, E034, E035, E036, E037, E038, E039;D-17|`node tools/phase7/check.cjs --case camp.hunting.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-31,T-07-32|New CampHuntingTests.cs and hunting.cjs; CLI producer07-01-T2|Pending|
|07-16-T2|8|ECON-04, NPC-05, NPC-06;R06;E033, E034, E035, E036, E037, E038, E039;D-17|`node tools/phase7/check.cjs --group camp.hunting --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved hunting.json scenarios|T-07-31,T-07-32|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-17-T1|9|NPC-08, NPC-06;R15;E096, E097, E098, E099, E100, E101, E102;D-17|`node tools/phase7/check.cjs --case camp.hauling.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-33,T-07-34|New CampHaulingTests.cs and hauling.cjs; CLI producer07-01-T2|Pending|
|07-17-T2|9|NPC-08, NPC-06;R15;E096, E097, E098, E099, E100, E101, E102;D-17|`node tools/phase7/check.cjs --group camp.hauling --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved hauling.json scenarios|T-07-33,T-07-34|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-18-T1|9|ECON-02, ECON-01;R04;E019, E020, E021, E022, E023, E024, E025;D-10,D-11,D-12,D-13,D-14|`node tools/phase7/check.cjs --case camp.sales.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-35,T-07-36|New CampSaleTests.cs and sales.cjs; CLI producer07-01-T2|Pending|
|07-18-T2|9|ECON-02, ECON-01;R04;E019, E020, E021, E022, E023, E024, E025;D-10,D-11,D-12,D-13,D-14|`node tools/phase7/check.cjs --group camp.sales --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved sales.json scenarios|T-07-35,T-07-36|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-19-T1|12|BUILD-07, BUILD-02;R22;E139, E140, E141, E142, E143, E144, E145|`node tools/phase7/check.cjs --case camp.towers.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-37,T-07-38|New CampTowerTests.cs and towers.cjs; CLI producer07-01-T2|Pending|
|07-19-T2|12|BUILD-07, BUILD-02;R22;E139, E140, E141, E142, E143, E144, E145|`node tools/phase7/check.cjs --group camp.towers --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved towers.json scenarios|T-07-37,T-07-38|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-20-T1|13|BUILD-08, BUILD-04;R23;E146, E147, E148, E149, E150, E151, E152|`node tools/phase7/check.cjs --case camp.attachments.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-39,T-07-40|New MainAttachmentTests.cs and attachments.cjs; CLI producer07-01-T2|Pending|
|07-20-T2|13|BUILD-08, BUILD-04;R23;E146, E147, E148, E149, E150, E151, E152|`node tools/phase7/check.cjs --group camp.attachments --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved attachments.json scenarios|T-07-39,T-07-40|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-21-T1|14|WORLD-05, ECON-01, NPC-08, BUILD-03, BUILD-05;R25;E158, E159, E161, E163|`node tools/phase7/check.cjs --case camp.coherence.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-41,T-07-42|New CampSnapshotTests.cs and coherence.cjs; CLI producer07-01-T2|Pending|
|07-21-T2|14|WORLD-05, ECON-01, NPC-08, BUILD-03, BUILD-05;R25;E158, E159, E161, E163|`node tools/phase7/check.cjs --group camp.coherence --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved coherence.json scenarios|T-07-41,T-07-42|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-22-T1|15|WORLD-05, ECON-01, ECON-03, ECON-05, NPC-03, NPC-04, NPC-05, BUILD-02, BUILD-05, BUILD-07, BUILD-08;AC-42/44/45/46;E157, E160, E162|`node tools/phase7/check.cjs --case camp.configuration.tracer --headless`|Actual production end-to-end solo/host/guest path,capture/install and fresh consequence observation|T-07-43,T-07-44|New CampConfigurationTests.cs and configuration.cjs; CLI producer07-01-T2|Pending|
|07-22-T2|15|WORLD-05, ECON-01, ECON-03, ECON-05, NPC-03, NPC-04, NPC-05, BUILD-02, BUILD-05, BUILD-07, BUILD-08;AC-42/44/45/46;E157, E160, E162|`node tools/phase7/check.cjs --group camp.configuration --headless`|Every individual owned EID assertion; pure/shared operations plus actual headless integration; saved configuration.json scenarios|T-07-43,T-07-44|Feature tests/state/view/scenario produced here; handler fromT1|Pending|
|07-24-T1|16|AC-42/46;D-01,D-10,D-11,D-12,D-13,D-14,D-16,D-17,D-18|`node tools/phase7/check.cjs --case camp.configuration.contracts.tracer --headless`|Actual packaged source/typed validator/English+expanded label through ordinary sale,host commit and solo/host/guest projection|T-07-47,T-07-48|Existing fields-economy/en/expanded; new CampContentContractTests.cs/content-contracts.cjs produced here; CLI producer07-01-T2|Pending|
|07-24-T2|16|AC-42/44/46 field/text bindings; unchanged E157/E160/E162 regression owners|`node tools/phase7/check.cjs --case camp.configuration.contracts --headless`; `node tools/phase7/check.cjs --group camp.content-contracts --headless`; `node tools/phase7/check.cjs --group camp.configuration --headless`; `node tools/phase1/check.cjs --group docs`|Independent complete source-prefix/field/default/range/relationship/schema/docs/actual consumer/text/reference inventory; real observations and known-bad controls|T-07-47,T-07-48|07-24-T1 creates tracer/handler/test; T2 completes world/docs/contract group and reusable scenario|Pending|
|07-23-T1|17|Complete phase correctness/STRIDE/C# and source coverage|`node tools/phase7/check.cjs --case review.prelaunch --headless`; exact strict build above|Independent whole-phase review/all actionable fixes/current source gate before windows|T-07-45,T-07-46|New review JSON/report and acceptance.cjs|Pending|
|07-23-T2|17|All selected requirements/AC/D; E164|`node tools/phase7/check.cjs --case acceptance.selection --headless`; `node tools/phase7/check.cjs --group selected --rendered --selection .planning/phases/07-camp-economy-and-development/07-SELECTION.json`|Windows1280×720/noVM actual solo and host+3guests,input/screenshots/appearance/listening; all selected cases including camp.acceptance.E164|T-07-45,T-07-46|New selection/execution/acceptance scenarios; requireT1 reviewed build|Pending|
|07-23-T3|17|All source rows and local acceptance evidence|`node tools/phase7/check.cjs --case acceptance.final --headless`|Immutable initial/focused/fresh-final lineage; current source/review and complete selected-set reconciliation|T-07-45,T-07-46|New acceptance report; updates validation only after actual audit|Pending|

T-07-SC is reserved in every plan: no package installation is planned; an actual npm/pip/cargo addition requires legitimacy audit and blocking review for ASSUMED/SUS. ASVS L1 and high/critical blocking apply phasewide. Unique numbered threats are T-07-01 through T-07-48; no collisions. Case failure output/nonzero exit and missing fresh positive assertion/evidence are explicit non-pass signals.

## Wave0 producers and blocked prerequisites

- 07-01-T1 owns verifying actual delivered Phase5 catalog/selection/review/evidence and Phase6 ownership/inventory/combat/control/Respawn/safety/input/snapshot APIs plus focused authentic proofs. Implementation halts if absent; no substitute wallet,fixture authority or production acceptance from inspection.
- 07-01-T1 records exact tracked existing paths/signatures and new candidate rebinding in07-CONTRACTS. Any newly discovered shared source edit must be added to exact task ownership and waves recomputed before coding.
- 07-01-T2 owns every new CLI flag,case/group selector,report schema and independent required-case enumeration before downstream command use.
- 07-01-T2 owns negative controls proving missing cases,zero assertions,blocked prerequisites,missing peers/review/evidence/crash/timeout cannot pass. A known clean positive observation must also pass.
- Each featureT1 owns its actual production Core/state/view/codec integration,test and headless handler. Each featureT2 owns its complete EID assertions and reusable scenario. No unimplemented required case earns credit.
- 07-21 owns realistic camp payload measurement,bounded detached parsing/transport controls and coherent root/ownership round trips.
- 07-22 owns typed production data consumers/default/range/relationship rules; dependent07-24 owns transferred schemas/docs/English+expanded text and exact production agreement proof. Both preserve paid work and full-health goals without claiming full Phase9 UX;07-24 must pass before final review.
- 07-24-T1/T2 independently verifies source-prefix/field/message/reference inventory, actual source-to-consumer-to-peer observations and deliberate mismatch/zero-evidence controls, with no rendered acceptance credit.
- 07-23 owns independent final review/fixes and all rendered evidence; every earlier task remains no-window.

## Individual edge commands and witnesses

[07-COVERAGE.md](07-COVERAGE.md#all168-explicit-edge-assertions) contains one row per E001–E168 with exact plan/task,stable case ID,test assertion,source resolution and concrete witness. Each uses `node tools/phase7/check.cjs --case <id> --headless` after its producer. E164 alone uses reviewed final `camp.acceptance.E164 --rendered --selection .planning/phases/07-camp-economy-and-development/07-SELECTION.json` within G7-LAN and cannot block an earlier headless feature group.

## Selection,groups and repair sequence

1. Complete all implementation and required pure/headless feature checks.
2. Independently review the whole phase and fix/re-review actionable findings before any rendered window.
3. Declare every active Critical,new/changed and affected case regardless of importance,including transitive consumers/shared inputs. Resolve uncertain impact; retain omitted IDs/reasons. Preserve original A-ID/scenario identities and later-owned portions.
4. Reuse ordered G7-SOLO and G7-LAN host+three-guests process groups with ordinary production resets,bounded waits and owned cleanup. Windows1280×720,noVM. Explain incompatible additional launches.
5. Record initial failures and continue independent cases; crashes block dependent cases. Batch repairs instead of editing/relaunching after each assertion.
6. Re-run failed/blocked/newly affected scenarios,then the entire selected set fresh on repaired reviewed build. Repeat if final fails. An initially clean entire selection is already the final pass.
7. Preserve immutable tested source/build/input/invocation/report/image/log identity. Explicitly distinguish executed-now,reused,missing/invalidated,not-selected and planned/later-owned evidence. No earlier pass hides a later failure.

Each saved scenario records stable global/local requirement,AC,E,D and original A-ID; importance/rationale/status/tags;seed/setup/preconditions;production actions;expected observations;environment/process roles;evidence/checkpoints;run group/order;ordinary reset/cleanup;bounded waits and any fresh-process justification. Four local processes do not establish physical mixedOS LAN; hosted Windows/Linux,supported displays,hardware/endurance and release balance keep distinct owners.

## Actual appearance and listening checks

| Observation | Owner | Required evidence |
| --- | --- | --- |
| Four-tab/Stash-only grids,grouped People/fixed header,disabled reasons,quantity editor/direct quote,site health/work/wait cues |07-23-T2/T3;D-01–D-18|Actual1280×720 production input and scenario screenshots with explicit readability/appearance verdict. |
| Bag positions/closure,held-input cancellation,Escape priority,nonlethal damage/death/access loss,visible continuous warnings |07-23-T2/T3|Actual native transitions and before/after observations,not sample JSON/log-only UI claims. |
| Work/payment/recruit/sale/shot sound and no replay on publication/restore |07-23-T2/T3|Actual playback/listening observation,reviewer/date/source/event; logs cannot substitute. |
| Structural sockets/art versus separate collision/reachability |07-19/20;final07-23|Headless common combat assertions plus actual final produced appearance review. |

UI-SPEC is absent; deterministic gate is nonblocking (frontend=true,hasFrontendEvidence=false,hasUiSpec=false,block=false,UI hint only). D-01–D-18,produced theme and Phase6 UI contract remain the design authority. No UI-SPEC validation is claimed.

## Audit and sign-off

- [ ] Actual predecessors delivered and exact contracts bound.
- [ ] New CLI controls pass known-bad and clean nonzero-assertion cases.
- [ ] All49 task checks and168 exact edge assertions exist and have authentic results.
- [ ] All22 global IDs,25 local R,48 AC and18 D remain covered.
- [ ] Numbered threat mitigations have actual production controls; no unresolved high/critical issue.
- [ ] Current whole-phase independent review/fixes precede rendered launch.
- [ ] Exact selected/omitted accounting,actual solo/four-process evidence and actual appearance/listening reviews retained.
- [ ] Repair cycle and final complete selected-set pass satisfy current policy.
- [ ] Actual Nyquist audit authorizes compliant flags; until then status draft,nyquist_compliant=false,wave_0_complete=false.

Approval remains pending actual execution/audit. This document provides a complete verification contract,not implementation or acceptance credit.
