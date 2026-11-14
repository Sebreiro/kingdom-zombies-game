# Verification reference

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. The complete-other-OS-VM rule continues for later phases.

Use [setup](setup.md) for prerequisites and isolated tool configuration. Run from the checkout root on Windows PowerShell or Linux. The [required-case manifest](../../tools/phase1/required-cases.json) declares obligations independently from discovered tests; the [runner](../../tools/phase1/check.cjs) requires exact fresh results and actual subject evidence.

| Need | Command |
| --- | --- |
| List declared exact IDs and current owners | `node tools/phase1/check.cjs --list` |
| Focused feedback while editing documentation | `node tools/phase1/check.cjs --group docs` |
| Focused code, graph, state or engine feedback | `node tools/phase1/check.cjs --group rules` (or `graph`, `state`, `engine`) |
| Canonical navigation tracer | `node tools/phase1/check.cjs --case docs.navigation` |
| Portable documentation contract | `node tools/phase1/check.cjs --case docs.portable` |
| Complete independently declared Phase 1 obligations | `node tools/phase1/check.cjs --phase 1` |
| Exhaustive manifest, including pending later certification | `node tools/phase1/check.cjs --all` |
| Raw toolchain/native bootstrap compatibility | `node tools/phase1/bootstrap.cjs --probe engine` |

The creator updated platform testing on 2026-11-14: use relevant checks on the development OS during implementation, then run the complete current phase scope on that OS and a local VM using the other supported OS before phase completion. For this Windows checkout, final Phase 1 acceptance requires complete Windows and Linux-VM runs after related repairs stabilize (D-10). Use local hardware acceleration when available. Planning/prose-only changes use focused docs and consistency checks (D-11). Failures, skips, empty selection, stale evidence and missing required results remain failures. Groups are `pins`, `runner`, `register`, `rules`, `graph`, `state`, `engine`, `docs`, `coverage` and `platform`.

`--phase N` selects independently declared cases with `acceptancePhase <= N`; an absent phase means 1. Explicit phases must be positive integers. Selection and deferred ownership use each case declaration, never implementation status or prior results. The older `acceptanceCriteria` allocation table retains its original implementation meaning; current complete two-platform Phase 1 execution belongs to 01-17, while original aggregate certification wrappers retain Phase 5 ownership. The current 684 declarations contain 676 Phase 1 obligations: all 666 original implemented cases, four selector controls and six review regression controls. The eight explicit Phase 5 IDs, phases and owners are listed separately outside the selected denominator with no pass/skip credit. Unresolved selected cases fail. Use `--phase 1 --list` for exact membership. Phase, case, group and exhaustive selectors cannot be combined; `--list` can accompany one selector. `--all` selects all 684 declarations and remains non-success while later certification is pending. Exact `--case` and `--group` retain their pending declarations. The [Phase 5 handoff](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md) defines all eight transferred identities.

After applying the isolation settings in setup, the strict whole-solution build command is identical on both platforms:

```sh
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase1/check.cjs --phase 1
```

The canonical runner performs exact locked restore and proof-project build itself. Whole-solution build additionally checks the game project. Each clean checkout first needs the authenticated engine feed and declared locked solution restore in setup. After source and policy stabilize, run clean restore, strict solution build, native bootstrap and the complete `--phase 1` selection on Windows and the local Linux VM for the same final implementation. Existing analyzer/graph/docs/state/engine and implemented failure-variant cases all remain selected. Inspect actual reports and subject evidence, not exit zero alone. Hardware acceleration is the intended VM mode when available. Coverage, unfinished aggregate cases and advanced certification remain Phase 5 work; no new report framework is needed. Keep tested inputs fixed and retain historical evidence.

Each run publishes an immutable `.phase1/runs/<nonce>/summary.json`, per-case `tests.json`, actual process logs, command identities and nonempty subject logs. The report records tested revision, input/tool/platform hashes and invocation nonce. Inspect the actual exit code and every independent failure. The runner collects independent cases, blocks checks whose prerequisites failed and reports `IMPLEMENTATION_PENDING` separately; blocked and skipped obligations are never passes (D-13). Negative controls pass only after successful setup and the intended subject rejection. Exit zero or raw passing tests alone cannot establish acceptance.

Relay concise completed/pass/fail counts and the active case during long verification, then give the final accepted/failed/blocked counts, exact report identity and links to relevant failure logs (D-12). Preserve diagnostic runs, repair related failures together, verify affected controls and obtain fresh full evidence after code/contract changes. Do not mix records across invocations, manually edit reports, stage files from a checker or silently replace failed controls.

Earlier summaries preserve historical identities and do not certify subsequent changes. The pre-review f4658bb Windows 670-case pass and sixteen Linux outcomes are historical after the four review repairs. Final Plan 01-17 acceptance needs fresh complete Windows and Linux-VM evidence for the current 676-case scope plus separate qualitative review. Module-reference validation is `node tools/phase1/check.cjs --case docs.projects`; current functional linkage is `node tools/phase1/check.cjs --case register.binding-precoverage`. Coverage and human judgment remain separate. All shared RULE requirements stay Pending until actual final platform acceptance and human review pass.

Phase 5 owns fresh measured line/branch coverage from the real analyzer and native-hosted managed production scopes, with unchanged inclusive 90% line / 80% branch targets and no silent exclusions. Collection/instrumentation, exact threshold/report checks and exhaustive platform/runner certification remain required there. There is currently no accepted measured coverage report; do not claim a pass or invent a collection command. The prepared Linux environment likewise supplies no executed baseline result.

The [code index](../code/README.md) defines exact documentation URL/anchor conventions. The documentation checker uses `git ls-files -z` plus its explicit newly authored list, checks strict UTF-8 and ordinal segment identities, and preserves both authored bytes and the Git index. [AI collaboration](ai-collaboration.md) records the platform-testing rule, interactive question preference and `workflow.text_mode=false`.
