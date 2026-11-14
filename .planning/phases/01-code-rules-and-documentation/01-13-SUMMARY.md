---
phase: 01-code-rules-and-documentation
plan: "13"
subsystem: documentation
tags: [csharp, documentation, git, utf8, portability, tdd]
requires:
  - phase: 01-12
    provides: Actual native lifecycle, generated-source permissions and trusted inherited proof baseline
provides:
  - Canonical AGENTS.md and focused code, architecture, review and decision navigation
  - Strict ordinal owned-document navigation with 32 independent real controls
  - Exact setup and verification command references preserving interactive preferences
affects: [01-14, 01-15, 01-16, 01-17, later-game-phases]
tech-stack:
  added: []
  patterns: [strict-utf8, ordinal-git-inventory, exact-heading-identities, nonmutating-document-checks]
key-files:
  created:
    - docs/code/README.md
    - docs/architecture/README.md
    - docs/code/review-checklist.md
    - docs/architecture/decisions/0001-enforcement.md
    - docs/architecture/decisions/0002-boundaries.md
    - docs/architecture/decisions/0003-state-and-engine.md
    - docs/workflow/verification.md
    - tests/phase1/DocumentationValidator.cs
    - tests/phase1/DocumentationTests.cs
    - tests/phase1/DocumentationLinks.cs
    - tests/phase1/DocumentationPath.cs
    - tests/phase1/DocumentationAnchors.cs
    - tests/phase1/DocumentationDecisions.cs
    - tests/phase1/DocumentationFixture.cs
  modified:
    - AGENTS.md
    - docs/workflow/ai-collaboration.md
    - docs/workflow/gsd.md
    - docs/workflow/setup.md
    - docs/code/register.json
    - tools/phase1/required-cases.json
key-decisions:
  - Validate owned navigation against exact ordinal Git inventory plus an explicit newly authored list; never stage inputs from the checker.
  - Preserve exact Unicode identity; duplicate implicit heading slugs require explicit unique anchors, with no inferred numeric suffixes.
  - Fingerprint the core authority pages and workflow configuration actually read by documentation proofs; reconcile fixed inputs before metadata updates.
requirements-completed: []
requirements-contributed: [RULE-03, RULE-04]
coverage:
  - id: D1
    description: Canonical guide reaches focused indexes and executable definitions with portable exact navigation
    requirement: RULE-04
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --case docs.navigation
        status: pass
    human_judgment: false
  - id: D2
    description: Real portable documentation mutations and preserved interaction/verification workflow
    requirement: RULE-03
    verification:
      - kind: integration
        ref: node tools/phase1/check.cjs --group docs
        status: pass
      - kind: integration
        ref: .phase1/runs/9ec275b7-832e-429b-be84-8e667c28cbd2/summary.json
        status: pass
    human_judgment: false
  - id: D3
    description: Qualitative rule, architecture-decision and focused-index accuracy review
    verification: []
    human_judgment: true
    rationale: Machine navigation and structure do not establish semantic accuracy; plan 01-14 retains explicit Pending review mapping.
actuals:
  tokens: 21288
  tasks: 2
  commits: 4
commits: 4
plan_head_before: c7468cab60a5601c90e58ab0882077a1e032d0f1
plan_head_after: cec34691bf26316b9d5290d05ddc9c8b36452a2a
duration: 69min
completed: "2026-11-14"
status: complete
---

# Phase 1 Plan 13: Focused documentation and portable exact navigation Summary

**One canonically tracked AGENTS.md now reaches focused executable-rule and architecture references, with 32 distinct C# navigation/workflow proofs and 641 accepted full-suite controls.**

Both tasks are complete on Windows. Phase 1 and all shared RULE requirements remain incomplete. The prepared Linux VM, captured Phase 2 context, immutable donor originals, installed tooling, exact five projects, generated permissions and all inherited severities/exemptions were preserved.

## Final full verification

The single immutable `node tools/phase1/check.cjs --all` run reconciled **641 accepted controls, zero failed controls and 15 IMPLEMENTATION_PENDING obligations out of 656**. Actual exit **1** solely reports required later-owner work; blocked obligations are not passes.

- [Canonical report](../../../.phase1/runs/9ec275b7-832e-429b-be84-8e667c28cbd2/summary.json).
- [Full console output](../../../.phase1/temp/13-all.log) and [actual exit](../../../.phase1/temp/13-all.exit).
- Tested revision: `cec34691bf26316b9d5290d05ddc9c8b36452a2a`.
- Full-run input: `d2279c167eb84f40ef3e2f6b519a4347eee79a87229ff1e7d26b70a168eeed11`.
- Tool identity: `d8f0bda2315abbcd8912095ba100f45f585931fe05cf104328d4e761941ce38c`.
- Actual platform: `win32-x64-10.0.19044`, SDK 10.0.404/runtime 10.0.15/Godot 4.7.2.
- Actual interval: `2026-11-14T10:40:39.0662464Z` to `2026-11-14T11:27:12.8301136Z` (46m34s).
- Group acceptance: docs **28/28**, graph **26/26**, state **33/33**, engine **19/19**. All four distinct R-83/R-84 controls also passed in the full run.

The tested source, docs, specifications, workflow configuration and core authority pages remained fixed through final reconciliation. Git was clean afterward. Subsequent ROADMAP/STATE completion metadata changes have their own focused D-11 documentation validation and identity; they are not represented as the full run's unchanged fingerprint.

Prerequisites and focused evidence:

- Strict whole `KingdomZombies.slnx` build with `--no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q`: **zero warnings/errors**.
- Final [documentation JUnit](../../../.phase1/temp/13-task2-final.xml): **32/32 passed**, zero failed/skipped. Each method has one distinct exact case binding and actual subject diagnostic/location; the canonical runner independently reconciles fresh evidence.
- Canonical tracer: **1/1 accepted**, nonce `df0f499f-39bb-44f6-8103-78c9a10ad224`; post-commit tracer feedback gate **1/1 accepted**, nonce `295294b8-096f-4e39-b73d-54cee053e5b0` before expansion.
- Canonical portable control: **1/1 accepted**, nonce `fe69a7b2-14ab-4bc1-bdcd-adf45cbd2e4e`.
- Focused docs group: **28/28 accepted**, nonce `f81f00e9-251b-42a5-a5b3-3a9dc3facd4d`.
- Independent R-83 positive/negative nonces: `027ce071-adbd-4225-8d3f-210c523d9dcd`, `09846d48-80b7-409d-b25f-4e6fdd6df47c`; R-84: `d5fed2eb-330d-4954-84c9-f69322fdeb70`, `f395a979-49c4-430c-8ba8-0607ab017c14`. Each accepted its one actual intended case.
- [Register/naming/analyzer preflight](../../../.phase1/temp/13-preflight-bindings.xml): **40/40 passed**. [Source/locks/native-companion preflight](../../../.phase1/temp/13-preflight-contracts.xml): **5/5 passed**.
- The documented actual `node tools/phase1/bootstrap.cjs --probe engine` passed, nonce `52ab2747-ba22-4fc0-8c08-5b2752555071`; it remains raw compatibility evidence. The draft command was corrected against the implemented CLI before commit.
- `git diff --check`, canonical Git inventory, exact naming/bindings, unchanged five LF locks, generated-source authenticity and inherited Godot companion controls passed. No test was skipped and no required plan verification command was left unrun.

## Delivered behavior

Tracked `Agents.md` was renamed through unique intermediate `AGENTS.phase1-case-migration.md` to canonical `AGENTS.md`. There is one canonical root entry, with the original concise project description, task-to-document table, collaboration preferences, specifications and planning links preserved. Code and architecture indexes point to actual configuration, analyzers, register, bounded exemptions, structural checks, proof commands and complete live prerequisite review/decision pages. There is no duplicated prose rule authority or consolidated walkthrough.

`DocumentationValidator` reads `git ls-files -z` through the existing bounded strict-UTF-8 Git reader, plus its explicit newly authored list until commit. It validates root AGENTS, docs, specs, core planning authorities and the exact five created-project docs locations. Installed tool/agent/plugin/vendor inputs, generated folders and provenance ZIPs are excluded. The checker never stages or modifies a document or Git index.

Paths are resolved against the ordinal inventory, including every directory segment. Wrong-case targets fail on Windows despite host lookup behavior. Relative/root-relative links, UTF-8 percent escapes, spaces and exact distinct composed/decomposed Unicode identities are supported. Parent traversal outside the repository, malformed escapes, invalid UTF-8, missing targets, missing/ambiguous anchors and duplicate/missing reference declarations produce intended diagnostics. Referenced duplicate implicit slugs require explicit unique anchors; numeric suffixes are not inferred. Permuted multiple links and a true singleton remain valid. Real concurrent repository checks preserve both authored hashes and exact Git index bytes.

The 32 unique controls include all seven formerly pending docs obligations, canonical navigation/portable cases, missing/empty/index checks, broken target/anchor, path-segment and explicit-anchor collisions, ambiguous/missing/full/collapsed references, real invalid filesystem bytes, malformed percent and escaped UTF-8, spaces/Unicode identity, ordering, escaping paths, explicit scope and independent R-83/R-84 machine components. Qualitative review remains separately Pending. The original donor index/decision analog was inspected from the unchanged captured archive.

The AI collaboration page keeps the creator's approved interactive question preferences and exact canonical guide link. `workflow.text_mode` remains `false` and has its own actual proof. Setup owns tool/cache/feed/locked-restore prerequisites. Verification owns actual focused/full commands, D-10 full-suite requirements for code/rule/command/contract changes, D-11 docs-only prose checks, D-12 concise progress/log reporting and D-13 independent-failure/blocked reporting. Clean-checkout platform sequences and fresh-result/coverage obligations explicitly distinguish required later gates from delivered evidence.

## TDD evidence and commits

| Task | RED | GREEN | Intended observed RED |
| --- | --- | --- | --- |
| 1: Canonical navigation tracer | `e295495` | `add9cb7` | Compliant singleton chain passed; the unchanged wrong-case target assertion rejected the scaffold's empty diagnostics. |
| 2: Portable navigation/workflow | `693ff62` | `cec3469` | After strict build success, the heading-only required index was incorrectly accepted and the target DOC_INDEX assertion failed. |

Unchanged supported JUnit reports and semantic records are preserved as `.phase1/temp/13-task{1,2}-red.xml/json`. Both classified **RED_EVIDENCE_OK** before implementation. Setup/compiler/style failures received no RED credit. Ordinary implementation formatting was corrected under the unchanged twelve-analyzer policy. Both final task implementations pass, and the tracer was reverified before expansion.

**TDD Gate Compliance:** two ordered genuine RED/GREEN pairs; no missing test commit, fabricated report, zero-test selection, unrelated failure or self-attested classifier fallback.

The on-disk plan ledger measures **four** source/proof commits from `c7468cab60a5601c90e58ab0882077a1e032d0f1` through `cec34691bf26316b9d5290d05ddc9c8b36452a2a`. The realized net diff is **85150 characters / ceil(chars/4) = 21288 tokens** over **20 files**, on the estimate's required scale. The recorded execution window is **69min** before metadata closure. Planning-metadata commits are additional.

## Deviations from Plan

**[Rule 2 - Missing critical prerequisite] Created complete minimal review and three decision pages.** The required live indexes could not point at absent 01-14-owned destinations. The pages record actual implemented enforcement, boundaries and state/engine decisions, each with Status/Context/Decision/Consequences, working exact links and explicit separately Pending qualitative review. Plan 01-14 retains all five project pages, complete per-rule/project/review mapping and refinement. These prerequisite pages do not claim later-owner closure. They were committed with Task 1.

**[Rule 2 - Input integrity] Included newly read authority/configuration files in the independent fingerprint.** Documentation proofs now read core PROJECT/REQUIREMENTS/ROADMAP/STATE/research authority pages and workflow configuration. Task 2 added those exact inputs so changing them during a run cannot silently reuse evidence. Scoped cohesive C# URL/anchor/decision/fixture helpers and exact manifest/register wiring are within the approved helper allowance. No package, architecture, severity, suppression or exemption change was needed.

No owned implementation is deferred, no authentication gate occurred and no unrun verification or functional stub remains. Empty non-document inventory byte arrays are deliberate target-existence inventory values, not rendered data or unfinished behavior. The final stub scan found no TODO/FIXME/placeholder behavior. No new network endpoint, auth path, production schema or unmodeled trust boundary was introduced.

## Remaining owners and platform handoff

The **15** pending obligations remain required: platform **1**, register **1**, rules **8**, pins **5**. Shared closure belongs to 01-14, measured coverage to 01-15, clean Windows preparation to 01-16 and actual identical-source Linux execution/final reconciliation to 01-17. The five old pin runner-edge IDs retain their declared 01-02 identities and explicit 01-14 closure handoff; WINDOWS entry 9 remains open. Historical bootstrap missing-RED WINDOWS entry 1 remains open. All RULE requirements and qualitative judgments stay Pending.

Plan 01-14 must finish/refine the prerequisite review/decision pages and add each of the five complete project reference pages without broken links. It must retain distinct real proof IDs, exact generated permissions, current source anchors and original donors. The one-shot `.phase1/temp/13-bind.cjs` utility is historical orchestration and must not be reapplied after later-owner edits.

For 01-16/01-17, freeze the **same core authority/configuration inputs** as well as source/docs/specs for both platform runs. Reconcile each full report before updating workflow metadata. ROADMAP/STATE completion updates change the input fingerprint; validate those prose-only updates separately under D-11 and record the new identity, without claiming it equals the full run. Actual Linux and measured 90% line/80% branch coverage evidence remain absent here. No Phase 2 implementation or platform acceptance is claimed.

## Self-Check: PASSED

All fourteen newly created files, canonical renamed guide and this SUMMARY exist. All four source/proof commits are ancestors of HEAD; evaluation-scope resolves the four actual commits, and the persisted ledger measures four. The final immutable report agrees with 641 accepted, zero failed controls and exactly 15 later-owner pending obligations. All 28 documentation, 26 graph, 33 state and 19 engine cases passed. Both original RED reports remain available. No accidental tracked deletion or generated untracked file was left behind.
