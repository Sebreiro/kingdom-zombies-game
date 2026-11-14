# Qualitative review checklist

Status: Pending. Machine passes establish the linked structural contracts; human judgment remains a separate required review.

| Review obligation | Evidence to inspect |
| --- | --- |
| Names, cohesion, API need, invariants and comments explain intent | [Rule register qualitative components](register.json), changed C# definitions and their positive/negative proofs |
| Retained/adapted/inapplicable source dispositions match the Godot need | [Captured baseline identity](provenance/manifest.json), [register](register.json), [exact exemptions](exemptions.json) |
| Dependency directions and decisions match actual responsibilities | [Architecture decisions](../architecture/README.md), evaluated and compiled proof reports |
| State, identity, time and randomness preserve intended game compatibility without implementing later features | [State and engine decision](../architecture/decisions/0003-state-and-engine.md), Core source and native assertions |
| Focused indexes are concise and lead to the appropriate authority | [Agent guide](../../AGENTS.md), [code index](README.md), [verification reference](../workflow/verification.md) |

The [exact per-rule checklist](review.md) identifies all fourteen qualitative components. Record reviewer, date, changed revision, evidence and explicit outcome in the linked phase-end human-check artifact when review occurs. No qualitative component is marked passed by its existence or by an automated/AI result.
