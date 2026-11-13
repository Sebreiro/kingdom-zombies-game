---
phase: 01-code-rules-and-documentation
plan: 18
subsystem: testing
tags: [godot, roslyn, configuration, banned-api, locked-restore]
requires:
  - phase: 01-04
    provides: Five strict project declarations and inspected exact package inputs
provides:
  - Permitted Godot product composition and exact compiler diagnostic verifier
  - Evaluated configuration and real banned-API permission/drift controls
  - Five canonical dependency locks with clean locked-restore and drift proof
  - Exact generated Godot report-method adaptation with origin and boundary proofs
affects: [01-05, 01-08, 01-09, 01-14, 01-16, 01-17]
tech-stack:
  added: [Microsoft.CodeAnalysis.CSharp 5.0.0 as a proof-project consumer of the already acquired pinned package]
  patterns: [Exact full-span compiler diagnostic matching, evaluated MSBuild controls, symbol-scoped generated adaptation]
key-files:
  created: [.editorconfig, BannedSymbols.txt, BannedSymbols.Tests.txt, BannedSymbols.Pure.txt, tests/phase1/ConfigurationTests.cs, tests/phase1/Verification/AnalyzerVerifier.cs, src/godot-game/GeneratedReportAdapter.cs, src/godot-game/packages.lock.json, docs/code/engine-adaptations.json]
  modified: [src/godot-game/KingdomZombies.csproj, tests/phase1/KingdomZombies.Phase1.Tests.csproj, tests/phase1/packages.lock.json, Directory.Build.targets, tools/phase1/bootstrap.cjs, tools/phase1/required-cases.json, tests/phase1/PinsTests.cs]
key-decisions:
  - Adapt the captured verifier contract using pinned Roslyn directly and actual evaluated net10 reference assemblies; reject all unexpected diagnostics.
  - Exempt only the authentic pinned Godot generated InitializeFromGameProject method from RS0030; handwritten adjacent code and method impostors remain rejected.
  - Keep five previously pending runner-edge identities blocked with 01-14 assigned their distinct-case closure.
requirements-completed: [RULE-01, RULE-03, RULE-05, RULE-07]
requirements-status: Pending shared phase acceptance; this plan contributes proofs without completing any RULE requirement
coverage:
  - id: D1
    description: Permitted game composition and exact compiler verifier
    verification:
      - kind: integration
        ref: tools/phase1/check.cjs --case pins.consumer; --case pins.verifier; --case pins.composition
        status: pass
    human_judgment: false
  - id: D2
    description: Evaluated policy, banned API scopes, naming permission and exact locks
    verification:
      - kind: integration
        ref: .phase1/runs/9e494b52-a7d8-4640-a775-1dd44e70153b/summary.json; all 13 ConfigurationTests cases passed
        status: pass
    human_judgment: false
  - id: D3
    description: Generated report adapter origin and handwritten boundary
    verification:
      - kind: integration
        ref: tests/phase1/ConfigurationTests.cs#GeneratedReportAdapterHasPinnedOrigin; GeneratedReportPermissionCannotEscapeItsMethod
        status: pass
    human_judgment: false
actuals:
  tokens: 14362
  tasks: 2
  commits: 4
plan_head_before: fc6523b22709cd444e246cc9c78033b82614d52a
plan_head_after: cbd55ef3e7c3736f28567f07bcc36f6666a4f239
duration: 21min
completed: 2026-11-13
status: complete
---

# Phase 1 Plan 18: Compiler and Evaluated Configuration Proofs Summary

**Godot composes Core and service ports under strict evaluated settings, with exact compiler diagnostics, bounded banned-API policies and five canonical locked dependency graphs.**

## Performance

- Observed implementation/evidence window: 2026-11-13T18:27:44Z–18:48:10Z, rounded to 21min; preliminary context loading excluded.
- Tasks: 2/2. Implementation files: 16. Actual tokens: realized code/config diff characters divided by four, rounded up; metadata excluded.
- Four task commits measured from the persisted `.git/gsd-plan-head-before-01-18` ledger. Planning closeout commits follow this measured implementation range.

## Accomplishments

- Actual evaluated game references contain exactly Core, Multiplayer.Abstractions, and the analyzer-only tooling edge. The game builds; an actual Godot-typed consumer emits through the verifier without constructing Nodes in plain .NET.
- The verifier resolves the actual .NET 10 reference pack and matches diagnostic ID, severity, file and complete start/end coordinates. Wrong diagnostic, wrong span/file/severity, missing diagnostics and additional compiler errors are rejected. Godot's net8 assembly reference produces the explicitly expected CS1701 unification warning in raw Roslyn; it is asserted, not suppressed. The actual SDK game build has zero warnings/errors.
- Thirteen unique discovered ConfigurationTests methods execute under the independent required-case manifest. Controls cover all five evaluated projects, exact central global analyzer versions, warnings-as-errors, removed shared imports, runtime/analyzer metadata drift, production ambient-clock/random/service-provider/Console bans, Shouldly, inherited Xunit.Assert ban, severity weakening and permission to omit Async suffixes.
- All five canonical project-directory locks exist and are consumed by a fresh isolated five-project restore with no preexisting assets. The same locks remain byte-identical; changing a dependency fails locked restore with NU1004. Four locks already delivered by 01-04 were retained; the test lock gained the pinned verifier package closure in Task 1, and the game lock moved from ignored cache to its canonical committed path in Task 2. Locks were generated through restore, not authored by hand.
- The observed Godot native-load report adapter is an exact assembly SuppressMessage for one generated method and RS0030. The local adaptation registry records its rule, target, package/source hashes, reason and proof IDs. Authentic generation passes; adjacent handwritten Console emits RS0030; a same-signature handwritten impostor collides with the real generated method with CS0111.

## Task Commits

| Task | Stage | Commit | Result |
| --- | --- | --- | --- |
| 1 | RED | `7bbcbcc` | Actual evaluated game had zero product references; expected two |
| 1 | GREEN | `dc52045` | Permitted composition, exact verifier and fresh manifest-bound cases |
| 2 | RED | `7a27ade` | Actual production AdditionalFiles omitted the required policy input |
| 2 | GREEN | `cbd55ef` | Scoped policies, exact generated adaptation, drift controls and canonical locks |

## Verification

| Command or proof | Result | Evidence |
| --- | --- | --- |
| Task 1 RED C# target with JUnit reporter, then `check tdd-red-evidence` | RED_EVIDENCE_OK; runtime-reference count 0 versus 2 after successful build/discovery | `.phase1/temp/01-18-task1-red.json`, `.xml`, `.log` |
| Task 2 RED C# target with JUnit reporter, then `check tdd-red-evidence` | RED_EVIDENCE_OK; actual AdditionalFiles policy missing after successful build/discovery | `.phase1/temp/01-18-task2-red.json`, `.xml`, `.log` |
| Fresh Task 1 tracer reruns | Three independent 1/1 passes | `.phase1/runs/8f66dcda-3910-422d-add4-57c7f0e2e2d1/`, `66eb6281-fd9f-4ae4-9677-2d4aafbbc14b/`, `be527707-7f95-4208-ae36-d573f8f38d7e/` |
| Task 1 complete implemented inventory | 80 passes, zero failed controls, 214 pending | `.phase1/runs/cd9f22ee-057e-4aa0-8126-09b17b06195f/summary.json` |
| `dotnet build KingdomZombies.slnx --no-restore -p:NuGetAudit=false` | Five projects; zero warnings/errors | `.phase1/temp/01-18-exact-adapter-build.log`; subsequent proof rebuilds |
| `node tools/phase1/check.cjs --group pins` | Expected aggregate exit 1: 22 passes, zero failed controls, five pending, 27 selected | `.phase1/runs/e293ae12-b199-42bc-9012-e2da59a2de53/summary.json` |
| `ConfigurationTests.FiveCanonicalLocksRejectDependencyDrift` | Fresh five-project locked restore passes; five assets generated; original locks unchanged; changed dependency NU1004 | Final `ac-01.complete` case report/log below |
| `node tools/phase1/bootstrap.cjs --probe pins` | Actual pinned tools/packages pass | `.phase1/runs/4ab7bce8-80f3-49fb-b96f-5d3ae2048820/` |
| `node tools/phase1/bootstrap.cjs --probe engine` | Native Windows construction/ready/exit and runtime identities pass | `.phase1/runs/fe6c6074-dbf0-4e93-ace4-a4b71229ec0e/compatibility.json` |
| Final `node tools/phase1/check.cjs --all` | Expected aggregate exit 1: **90 implemented passes, zero failed controls, 213 pending, 303 declared** | `.phase1/runs/9e494b52-a7d8-4640-a775-1dd44e70153b/summary.json`; `.phase1/temp/01-18-final-all.log` |
| `git diff --check` and changed-file stub scan | Pass; no new TODO/placeholder implementation | Pre-closeout checks |

Every final failure record was independently inspected: all 213 are `status: blocked`, `diagnostic: IMPLEMENTATION_PENDING`. No failed implemented test, missing/stale result or source mutation was accepted. Full acceptance, Linux and coverage remain unproven. NuGetAudit=false is a command-scoped offline/local verification choice inherited from the baseline; no persistent audit-disabling project property was introduced. No new package acquisition or substitution occurred: Roslyn inputs already came from the inspected 01-04 cache.

## TDD Gate Compliance

Both tasks have a real C# assertion RED, machine verdict RED_EVIDENCE_OK, semantic inspection and a subsequent GREEN commit. Initial compiler/reporter/root-path setup failures were not counted as RED. Task 1 fails on the actual evaluated reference count; Task 2 fails on the actual missing compiler policy input. No refactor commit was needed. Historical plan 01-01 missing-RED debt remains open; this plan does not fabricate evidence for it.

## Deviations from Plan

1. **[Rule 3 - Blocking] Verifier and dispatcher prerequisites.** Added the already pinned CSharp package to the existing test project and generated its exact lock closure; adapted the captured verifier's matching contract directly to Roslyn instead of acquiring another testing package. Updated independent case bindings and input snapshot scope to include shared configuration and all banned-symbol files. These changes are necessary for real compilation and fresh evidence, not extra product features. Commits: `dc52045`, `cbd55ef`.
2. **[Rule 2 - Critical policy wiring] Missing compiler policy inputs.** Added AdditionalFiles wiring and root diagnostic configuration; a separate pure-boundary input keeps Godot integration legal while banning engine objects in Core/service interfaces. Test process fixtures retain bounded blocking coordination; production retains applicable captured blocking bans. Exact naming-rule permission settings disable only MA0137/MA0156. Broader rule/register and exemption enforcement remains 01-08 ownership. Commit: `cbd55ef`.
3. **[Rule 3 - Observed engine compatibility] Generated native-load report.** Initial strict banned policy, then two attempted generated-tree configuration scopes (exact output path and generator-path suffix) still produced authentic RS0030 in Godot's initializer. Those attempts were removed. After the bounded-fix escalation, the orchestrator explicitly authorized the distinct exact compiler-symbol adaptation with origin/collision/adjacent proofs. The final assembly target is `~M:GodotPlugins.Game.Main.InitializeFromGameProject(System.IntPtr,System.IntPtr,System.IntPtr,System.Int32)` only. No namespace/file-glob severity reduction remains. The initial boundary fixture also applied its injected source to referenced projects; corrected the injection to the actual game only before accepting evidence. Commit: `cbd55ef`.
4. **[Rule 1 - Bug] Bootstrap lock consumer followed the old location/graph.** Relocated its read to the canonical game lock and required the exact six packages plus exactly two project records. Existing permutation control now rejects missing/foreign/wrong-type/project-dependency metadata in addition to package/version drift. Actual engine probe and final full suite pass. Commit: `cbd55ef`.

No library/framework switch, gameplay, service implementation, blanket suppression or limit reduction was added. No tracked deletion, authentication gate or automatic approval rejection occurred. Supplied-root and configured branch guards passed before every task commit; sequential main commits are explicitly configured and authorized.

## Deferred Issues and Aggregate Command Mismatch

The literal tracer/group command selects preexisting pending obligations, including Task 2's then-pending AC-01, so it could not return zero at Task 1's boundary. The orchestrator approved the three fresh actual tracer cases plus complete implemented-suite reconciliation as the bounded Task 1 gate. No aggregate group was called green. Task 2 closes AC-01; five independently declared edge results remain blocked:

| Required identity | Declared owner | Concrete closure owner |
| --- | --- | --- |
| `edge.rule-01.adjacency` | 01-02 | 01-14 distinct-case proof binding |
| `edge.rule-01.empty` | 01-02 | 01-14 distinct-case proof binding |
| `edge.rule-01.ordering` | 01-02 | 01-14 distinct-case proof binding |
| `edge.rule-01.idempotency` | 01-02 | 01-14 distinct-case proof binding |
| `edge.rule-01.concurrency` | 01-02 | 01-14 distinct-case proof binding |

Related earlier controls exist, but no result is reused for another required identity. The pending set is recorded in `deferred-items.md` and WINDOWS entry 9. Future analyzer, graph, state, lifecycle, docs, coverage and Linux cases retain their blockers.

## Known Stubs

No new stub was introduced in this plan's changed files. The preexisting empty custom analyzer assembly remains intentional tooling-only wiring until 01-05–01-07 (WINDOWS entry 8); it is not credited as custom-rule enforcement. No new trust boundary beyond the plan's compiler/configuration/file-proof surface was introduced.

## Next Plan Readiness

Ready for **01-05 (wave 6)**. Phase 1 is 5/18 plans, not complete. All RULE requirements remain Pending: `requirements.ready-ids` returned no ready IDs because shared sibling plans are incomplete. Actual forbidden/direct/transitive/type graph proofs remain 01-09 work. No Linux or coverage pass is claimed. The new observed generated adaptation registry must remain visible to 01-08's broader origin/exemption checks.

No user setup is required. Preexisting untracked `.planning/state.json` and `.planning/tmp/` were preserved and remain outside this plan's commits.

## Self-Check: PASSED

Verified all declared new artifacts exist, all four task commits are ancestors of HEAD, the ledger measures exactly four implementation commits, and the final report contains 90 unique accepted results plus 213 exclusively pending blockers.
