# Verification reference

Use [setup](setup.md) for prerequisites and isolated tool configuration. Run from the checkout root on Windows PowerShell or Linux. The [required-case manifest](../../tools/phase1/required-cases.json) declares obligations independently from discovered tests; the [runner](../../tools/phase1/check.cjs) requires exact fresh results and actual subject evidence.

| Need | Command |
| --- | --- |
| List declared exact IDs and current owners | `node tools/phase1/check.cjs --list` |
| Focused feedback while editing documentation | `node tools/phase1/check.cjs --group docs` |
| Focused code, graph, state or engine feedback | `node tools/phase1/check.cjs --group rules` (or `graph`, `state`, `engine`) |
| Canonical navigation tracer | `node tools/phase1/check.cjs --case docs.navigation` |
| Portable documentation contract | `node tools/phase1/check.cjs --case docs.portable` |
| Full current local suite | `node tools/phase1/check.cjs --all` |
| Raw toolchain/native bootstrap compatibility | `node tools/phase1/bootstrap.cjs --probe engine` |

Focused `--group` or exact `--case` checks provide feedback during edits (D-10). Before finishing any code, executable rule, command or contract change, build the whole solution and run the complete currently implemented local suite with `--all`. Pure prose-only edits require `--group docs` (D-11). Partial feedback does not complete verification. Groups are `pins`, `runner`, `register`, `rules`, `graph`, `state`, `engine`, `docs`, `coverage` and `platform`. A selector must execute its declared nonempty set; no match or missing required case is failure.

After applying the isolation settings in setup, the strict whole-solution build command is identical on both platforms:

```sh
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase1/check.cjs --all
```

The canonical runner performs exact locked restore and proof-project build itself. Whole-solution build additionally checks the existing game project. A clean checkout first needs the verified engine feed and locked solution restore described in setup. The clean-checkout sequence is the same on Windows and Linux: satisfy setup, run native bootstrap, restore the solution locked, strict whole-solution build, then `--all`. Keep sources fixed throughout one full run. Do not reuse a report after changing any fingerprinted source, docs, specs or configuration.

Each run publishes an immutable `.phase1/runs/<nonce>/summary.json`, per-case `tests.json`, actual process logs, command identities and nonempty subject logs. The report records tested revision, input/tool/platform hashes and invocation nonce. Inspect the actual exit code and every independent failure. The runner collects independent cases, blocks checks whose prerequisites failed and reports `IMPLEMENTATION_PENDING` separately; blocked and skipped obligations are never passes (D-13). Negative controls pass only after successful setup and the intended subject rejection. Exit zero or raw passing tests alone cannot establish acceptance.

Relay concise completed/pass/fail counts and the active case during long verification, then give the final accepted/failed/blocked counts, exact report identity and links to relevant failure logs (D-12). Preserve diagnostic runs, repair related failures together, verify affected controls and obtain fresh full evidence after code/contract changes. Do not mix records across invocations, manually edit reports, stage files from a checker or silently replace failed controls.

Plan 01-12's last fixed full run accepted 609 controls with zero failed controls and 26 later-owner pending obligations out of 635; it is historical baseline evidence, not a pass for subsequent documentation changes. Documentation controls are delivered by 01-13. Shared per-rule/project/review closure remains 01-14, measured coverage 01-15, clean Windows preparation 01-16 and actual identical-source Linux execution/final reconciliation 01-17. All shared RULE requirements and qualitative reviews remain Pending until their complete gates pass. Commands document how to obtain platform evidence; Windows execution, instructions or cross-builds cannot substitute for actual Linux execution.

Coverage acceptance requires fresh measured line and branch evidence from the real analyzer and native-hosted managed production scopes, with exact inclusive 90% line / 80% branch gates and no silent exclusions. Plan 01-15 owns the implemented collection/normalization commands and report mutations. There is currently no accepted measured coverage report, so do not report coverage as passing or invent a collection command. The prepared Linux environment is likewise preparation, with actual execution still required.

The [code index](../code/README.md) defines exact documentation URL/anchor conventions. The documentation checker uses `git ls-files -z` plus its explicit newly authored list, checks strict UTF-8 and ordinal segment identities, and preserves both authored bytes and the Git index. [AI collaboration](ai-collaboration.md) retains the interactive question preference and `workflow.text_mode=false`.
