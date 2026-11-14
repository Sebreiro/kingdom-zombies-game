# Verification reference

Use [setup](setup.md) for prerequisites and isolated tool configuration. Run from the checkout root on Windows PowerShell or Linux. The [required-case manifest](../../tools/phase1/required-cases.json) declares obligations independently from discovered tests; the [runner](../../tools/phase1/check.cjs) requires exact fresh results and actual subject evidence.

| Need | Command |
| --- | --- |
| List declared exact IDs and current owners | `node tools/phase1/check.cjs --list` |
| Focused feedback while editing documentation | `node tools/phase1/check.cjs --group docs` |
| Focused code, graph, state or engine feedback | `node tools/phase1/check.cjs --group rules` (or `graph`, `state`, `engine`) |
| Canonical navigation tracer | `node tools/phase1/check.cjs --case docs.navigation` |
| Portable documentation contract | `node tools/phase1/check.cjs --case docs.portable` |
| Exhaustive manifest, including pending later certification | `node tools/phase1/check.cjs --all` |
| Raw toolchain/native bootstrap compatibility | `node tools/phase1/bootstrap.cjs --probe engine` |

The creator amended Phase 1 completion on 2026-11-14: preserve every working rule/test, use focused `--group` or exact `--case` feedback while batching closure work, then obtain one final complete Phase 1 Windows run and actual representative clean Linux checks (D-10). Planning/prose-only changes use docs and planning-consistency checks (D-11). Real failures, skips, empty selection and missing required results remain failures. Groups are `pins`, `runner`, `register`, `rules`, `graph`, `state`, `engine`, `docs`, `coverage` and `platform`.

**Planned by 01-15, unavailable until implemented:** `node tools/phase1/check.cjs --phase 1` selects the independently declared Phase 1 obligations. It must keep every existing implemented case, reject unresolved selected cases, and separately list the explicit pending Phase 5 certification IDs/owners. Existing `--all` keeps its exhaustive meaning; it does not become an implementation-only selector and remains non-success while later certification is pending. The [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) defines the transferred work.

After applying the isolation settings in setup, the strict whole-solution build command is identical on both platforms:

```sh
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase1/check.cjs --all
```

The canonical runner performs exact locked restore and proof-project build itself. Whole-solution build additionally checks the existing game project. A clean checkout first needs the verified engine feed and locked solution restore described in setup. On Windows, run native bootstrap, locked restore, strict solution build and the complete Phase 1 selection once after closure changes stabilize. On Linux, run actual clean restore/build/bootstrap plus the predeclared representative analyzer/graph/docs/state/engine checks in 01-16/17. Complete Linux failure variants, coverage and exhaustive cross-platform reconciliation are Phase 5 work. The `--all` example above remains the existing exhaustive command, not the amended Phase 1 completion command. Keep tested inputs fixed during each run; a changed fingerprint requires fresh evidence rather than reuse of a prior report.

Each run publishes an immutable `.phase1/runs/<nonce>/summary.json`, per-case `tests.json`, actual process logs, command identities and nonempty subject logs. The report records tested revision, input/tool/platform hashes and invocation nonce. Inspect the actual exit code and every independent failure. The runner collects independent cases, blocks checks whose prerequisites failed and reports `IMPLEMENTATION_PENDING` separately; blocked and skipped obligations are never passes (D-13). Negative controls pass only after successful setup and the intended subject rejection. Exit zero or raw passing tests alone cannot establish acceptance.

Relay concise completed/pass/fail counts and the active case during long verification, then give the final accepted/failed/blocked counts, exact report identity and links to relevant failure logs (D-12). Preserve diagnostic runs, repair related failures together, verify affected controls and obtain fresh full evidence after code/contract changes. Do not mix records across invocations, manually edit reports, stage files from a checker or silently replace failed controls.

Earlier summaries preserve historical full-run identities; they do not certify subsequent changes. Module-reference validation is `node tools/phase1/check.cjs --case docs.projects`; current functional linkage is `node tools/phase1/check.cjs --case register.binding-precoverage`. The latter checks real mechanisms and distinct proofs while keeping coverage and human judgment separate. Plan 01-15 now migrates explicit certification ownership and implements phase selection; 01-16 owns the final clean Windows baseline and 01-17 the representative actual Linux baseline and separate review. The original coverage-dependent AC-06/07/09 aggregate cases remain pending for Phase 5. All shared RULE requirements stay Pending until the revised Phase 1 baseline and qualitative review actually pass. Windows execution, instructions or cross-builds never substitute for actual Linux execution.

Phase 5 owns fresh measured line/branch coverage from the real analyzer and native-hosted managed production scopes, with unchanged inclusive 90% line / 80% branch targets and no silent exclusions. Collection/instrumentation, exact threshold/report checks and exhaustive platform/runner certification remain required there. There is currently no accepted measured coverage report; do not claim a pass or invent a collection command. The prepared Linux environment likewise supplies no executed baseline result.

The [code index](../code/README.md) defines exact documentation URL/anchor conventions. The documentation checker uses `git ls-files -z` plus its explicit newly authored list, checks strict UTF-8 and ordinal segment identities, and preserves both authored bytes and the Git index. [AI collaboration](ai-collaboration.md) retains the interactive question preference and `workflow.text_mode=false`.
