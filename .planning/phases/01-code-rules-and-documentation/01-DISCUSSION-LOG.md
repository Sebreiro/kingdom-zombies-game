# Phase 1: Code rules and documentation - Discussion Log

> **Audit trail only.** Do not use as input to planning, research, or execution agents.
> Decisions are captured in [01-CONTEXT.md](01-CONTEXT.md); this log preserves the alternatives considered.

**Date:** 2026-11-13 (Europe/Paris)
**Phase:** 01-code-rules-and-documentation
**Areas discussed:** Everyday verification workflow; rule documentation and examples; minimal Godot rule examples; tooling stability preferences.

The initial numbered chat prompt was replaced by interactive questions at the creator's request. The language scope and example scope were clarified from free-text answers. Follow-up questions were omitted where those answers had already settled the proposed presentation choices.

---

## Discussion setup

### 1. Which areas would you like to discuss for Phase 1?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | All four areas | Yes |
| 2 | Everyday verification — running checks and understanding failures |  |
| 3 | Rule documentation — explanations, examples, and finding the right fix |  |
| 4 | Godot proof scene — the example and what developers see |  |
| 5 | Tooling stability — stable releases and acceptable prerelease dependencies |  |

**User's choice:** All four areas

### 2. Creator correction to question presentation

Creator-provided instruction; no options were presented.

**User's choice:** as as an interractive question. And this rule should be added to othere AI rules in indexed docs

**Notes:** Implemented docs/workflow/ai-collaboration.md, indexed it from Agents.md and docs/workflow/gsd.md, and set workflow.text_mode=false. Use interactive controls for all remaining discussion questions.

---

## Everyday verification workflow

### 1. While changing code, how should developers and agents run verification? Phase completion will still require the full Windows and Linux evidence already specified.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Focused checks while editing, then the complete local suite before finishing a change (Recommended) | Yes |
| 2 | Run the complete local suite every time verification is requested |  |

**User's choice:** Focused checks while editing, then the complete local suite before finishing a change

### 2. How should a verification run present its results?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Concise progress and a final summary, with failures explained and links to detailed logs (Recommended) | Yes |
| 2 | Show every check and its result as it runs, followed by a final summary |  |

**User's choice:** Concise progress and a final summary, with failures explained and links to detailed logs

### 3. If a verification run finds a failure, should it continue with other checks?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Collect all independent failures; stop checks that depend on a failed prerequisite (Recommended) | Yes |
| 2 | Stop at the first failure so it can be fixed before rerunning |  |

**User's choice:** Collect all independent failures; stop checks that depend on a failed prerequisite

### 4. What verification should be required for documentation-only changes?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Documentation checks for prose-only edits; full suite when rules, commands, or contracts change (Recommended) | Yes |
| 2 | The complete local suite for every documentation change |  |

**User's choice:** Documentation checks for prose-only edits; full suite when rules, commands, or contracts change

### 5. Would you like to discuss verification further or move on? Remaining areas: rule documentation, the Godot proof scene, and tooling stability.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Move on to rule documentation | Yes |
| 2 | Discuss verification further |  |

**User's choice:** Move on to rule documentation

---

## Rule documentation and examples

### 1. How should each coding rule be explained in the documentation?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Precise rule, short rationale, and good/bad examples where the rule is not obvious (Recommended) |  |
| 2 | Compact rule text with links to executable examples and proof tests; minimal explanation in the document |  |

**User's choice:** If it is a link rules - they should be in the lint rule file as as code instead of documented description. Code should be as a rule for most of the times

**Notes:** Creator supplied a code-first alternative: machine-checkable lint/analyzer rules are primarily executable definitions. Documentation links to those definitions rather than duplicating rule prose. The required inheritance/enforcement/proof register remains.

### 2. Where should explanations for non-obvious executable rules live?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Beside the rule code/configuration; separate docs explain architecture decisions, Godot adaptations, and review-only rules (Recommended) | Yes |
| 2 | In a short documentation entry for each rule, linked to its code/configuration |  |

**User's choice:** Beside the rule code/configuration; separate docs explain architecture decisions, Godot adaptations, and review-only rules

### 3. For rules that genuinely require human judgment, how should developers and agents find the review obligations?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | A short, explicit review checklist linked to the relevant code and architecture decisions (Recommended) | Yes |
| 2 | Review notes within the relevant architecture decision documents, without a separate checklist |  |

**User's choice:** A short, explicit review checklist linked to the relevant code and architecture decisions

### 4. How should a developer or AI agent find setup commands, verification commands, and the executable rules?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | One short “Start here” page with commands and task-based links to the authoritative files (Recommended) |  |
| 2 | Focused reference indexes only, with setup and verification commands in their respective documents | Yes |

**User's choice:** Focused reference indexes only, with setup and verification commands in their respective documents

### 5. Would you like to discuss the rule documentation further or move on? Remaining areas: the Godot proof scene and tooling stability.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Move on to the Godot proof scene | Yes |
| 2 | Discuss rule documentation further |  |

**User's choice:** Move on to the Godot proof scene

---

## The Godot proof scene

### 1. What should the small Godot proof scene look like? It will demonstrate the required engine and state contracts without implementing gameplay.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | A minimal technical fixture using plain nodes, simple resources, and explicit check results (Recommended) |  |
| 2 | A tiny game-themed fixture using placeholder survivor/camp objects to illustrate the same contracts |  |

**User's choice:** We already have an empty project. just use it to launch. you can add a few example code files. but don't build the game. it is in the future phases. This pahase should be only about code/architecture/lint rules so AI agents and ppl write similar code  - like small functions. and use only c# ,etc

**Notes:** Use the existing empty Godot project as the launch target. Add only the minimal example code/files/resources needed to demonstrate the required code/architecture rules and engine compatibility. No game-themed showcase, gameplay, or separate presentation feature. The fixed lifecycle/state proof obligations remain small executable examples. Prioritize consistent C# for people and agents, including enforced short functions.

### 2. How broadly should “C# only” apply to project-owned code?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | C# for game code, examples, tests, analyzers, and verification helpers; allow configuration files and minimal launch scripts (Recommended) |  |
| 2 | C# for game code and tests; other languages may be used for development tools | Yes |

**User's choice:** C# for game code and tests; other languages may be used for development tools

**Notes:** C# restriction covers game/example code and tests. Development tooling may use other languages; do not silently extend the C# restriction to every helper.

### 3. Is the scope of the rule examples settled, or would you like to refine it? The final remaining area is tooling stability.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Move on to tooling stability | Yes |
| 2 | Discuss the rule examples further |  |

**User's choice:** Move on to tooling stability

**Notes:** Creator's scope clarification resolved the presentation questions, so no further showcase/UI questions were needed.

---

## Tooling stability preferences

### 1. Should the Phase 1 toolchain permit prerelease dependencies? Exact versions will still need to pass the required compatibility proofs.

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Stable releases only, including test and lint tools (Recommended) |  |
| 2 | Stable engine and runtime; proven prerelease test or lint tools are acceptable when needed | Yes |

**User's choice:** Stable engine and runtime; proven prerelease test or lint tools are acceptable when needed

### 2. If several supported stable toolchain combinations pass all proofs, which should guide the choice?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Prefer an established baseline; upgrade for a concrete compatibility or support benefit (Recommended) | Yes |
| 2 | Prefer the newest supported stable versions that pass all proofs |  |

**User's choice:** Prefer an established baseline; upgrade for a concrete compatibility or support benefit

---

## Discussion completion

### 1. Are these Phase 1 decisions ready to save as the context for research and planning?

| Option | Description | Selected |
| --- | --- | --- |
| 1 | Save the phase context | Yes |
| 2 | Refine the tooling choices |  |
| 3 | Discuss another unclear area |  |

**User's choice:** Save the phase context

**Notes:** The creator approved saving the summarized decisions from all four areas. No planning or implementation was requested in this discussion.

---

## Agent's Discretion

No explicit “you decide” choices were made. Concrete implementation details remain for research and planning within the approved specification and captured decisions; no versions were selected in this discussion.

## Deferred Ideas

None. The creator reiterated that gameplay belongs to later phases; no new scope or backlog item was added.
