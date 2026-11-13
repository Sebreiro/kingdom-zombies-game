---
phase: "01"
slug: "code-rules-and-documentation"
status: approved
reviewed_at: "2026-11-13T16:27:07Z"
shadcn_initialized: false
preset: none
created: "2026-11-13"
---

# Phase 01 — UI Design Contract

## Scope and Authority

Phase 1 has no frontend or player-facing UI work. This contract records that boundary and preserves the existing developer-output obligations; it introduces no presentation feature.

Sources: [01-SPEC.md](01-SPEC.md) Boundaries and AC-03–AC-04; [01-CONTEXT.md](01-CONTEXT.md) D-05, D-07–D-08 and D-12–D-13; [ROADMAP.md](../../ROADMAP.md#phase-1-code-rules-and-documentation) Phase 1; [REQUIREMENTS.md](../../REQUIREMENTS.md#development-rules-and-documentation) RULE-01–RULE-07; [01-RESEARCH.md](01-RESEARCH.md) Summary and Architectural Responsibility Map.

The roadmap explicitly marks Phase 1 as having no UI. The automated gate's frontend match came from that negative UI hint; it reported no frontend evidence. This token match does not override the locked scope.

Use the existing `src/godot-game/project.godot` only for the minimal executable integration proof. Do not add a menu, HUD, playable scene, game-themed demonstration, or visual presentation. Phase 2 owns the playable foundation. Actual engine execution and Windows/Linux evidence remain required by the specification.

## Design System

| Property | Value |
| --- | --- |
| Tool | none |
| Preset | not applicable |
| Component library | none for this phase |
| Icon library | not applicable |
| Font | not applicable; use the developer's terminal and Markdown viewer defaults |

The project is Godot/C#, not a React/Next.js/Vite frontend. Repository scouting found the existing Godot project and no frontend `components.json`, application `package.json`, TSX components, or CSS design system. The shadcn initialization gate is not applicable. No component inventory is required with `Tool: none`.

## Spacing Scale

Not applicable: no rendered UI layout is delivered. Do not introduce spacing tokens or phase-specific layout metrics. Focused Markdown documentation retains its existing navigation patterns.

## Typography

Not applicable: this phase declares no font sizes, font weights, line heights, or typography tokens. Terminal and documentation rendering remain controlled by the developer's tools.

## Color

Not applicable: no palette, surface split, accent allocation, destructive color, or color tokens are introduced. Developer proof results must identify their outcome in text; color is not required to interpret them.

## Copywriting Contract

There are no player-facing controls or UI states. Developer output follows the already approved requirements below; exact command names and diagnostic wording belong to implementation planning.

| Element | Contract | Source |
| --- | --- | --- |
| Primary CTA | Not applicable; no graphical action control | SPEC Boundaries; CONTEXT D-08 |
| Empty state heading/body | Not applicable as UI copy; missing required cases or empty discovery must produce a non-success diagnostic naming the missing obligation | SPEC AC-03 |
| Progress | Show concise progress while checks execute | CONTEXT D-12 |
| Final summary | Show the run outcome with clear failure explanations and links to detailed logs | CONTEXT D-12 |
| Error diagnostics | Identify the failing or missing obligation; rule violations identify the rule and relevant location | SPEC AC-03, AC-07 |
| Independent failures | Collect all independent failures in one run | CONTEXT D-13 |
| Blocked checks | Stop checks whose prerequisites failed and explicitly report the blocked obligations; do not count them as passes | CONTEXT D-13 |
| Destructive confirmation | Not applicable; no destructive UI action is introduced | SPEC Boundaries |

Preserve fresh, uniquely identified results and required evidence. An interrupted or incomplete run cannot report a complete pass or reuse stale artifacts as current evidence. These are proof-runner behavior obligations from SPEC AC-03–AC-04, not new UI requirements.

## UI Considerations

Applicable frontend state considerations resolved: none applicable; no frontend elements exist in the phase scope. Developer proof output is governed by the Copywriting Contract and the specification's executable acceptance criteria.

| Category | Element(s) | Status | Resolution / Reason |
| --- | --- | --- | --- |
| empty / loading / error / populated / partial | None | dismissed | No forms, collections, media, navigation, or graphical controls are delivered. Execution progress and failure reporting are terminal obligations already captured above. |
| overflow / zero-one-many / long-text | None | dismissed | No phase-owned visual containers or UI collections exist. No frontend wrapping, truncation, or responsive layout contract is needed. |

Responsive layout, visual assets, animation, touch targets, graphical focus behavior, and frontend accessibility patterns are not applicable because Phase 1 introduces no frontend. This exemption does not remove documentation navigation, actionable diagnostics, or trustworthy proof results.

## Registry Safety

Not applicable: no shadcn initialization, component registry, third-party block, or UI package is introduced. No registry vetting is required.

## Checker Sign-Off

Pending checker review. Applicability is recorded below; no approval is claimed by this draft.

| Dimension | Applicability |
| --- | --- |
| 1 Copywriting | Developer-output contract only; graphical UI copy not applicable |
| 2 Visuals | Not applicable; presentation work excluded |
| 3 Color | Not applicable; no UI palette |
| 4 Typography | Not applicable; no UI typography |
| 5 Spacing | Not applicable; no UI layout |
| 6 Registry Safety | Not applicable; no registry use |
| 7 Inventory Provenance | Not applicable; Tool: none |

**Approval:** pending
