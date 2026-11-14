# AI Collaboration

This document records the creator's preferences for AI agents working on KingdomZombies. The root [agent guide](../../AGENTS.md) indexes these instructions.

## Interactive Questions

- Use the available interactive question controls when asking the creator for information, preferences, or decisions. Do not substitute a numbered chat list when interactive controls are available.
- Offer clear, concise choices when useful, and allow a free-text answer. If the control supports only one selection, allow the creator to name several choices in the free-text answer when the question permits multiple selections.
- Use ordinary chat questions when interactive controls are unavailable or the creator explicitly requests a text response.
- Keep GSD's `workflow.text_mode` set to `false` for this preference. A workflow's text-mode default does not override the creator's explicit request for interactive questions.

## Development and Phase-End Testing

- During implementation, run relevant checks on the operating system where the agent is developing.
- Before finishing a phase, test its complete required scope on that development OS and in a local VM running the other supported OS: Linux when developing on Windows, or Windows when developing on Linux.
- Use hardware acceleration for the local VM when available. Record actual execution and results; a prepared VM or cross-build does not establish a pass.
- Preserve explicitly deferred later-phase work and its ownership. A complete phase run selects the current phase's independently declared required scope, without crediting pending later-phase cases.
- Batch related repairs with focused feedback, then run the complete phase scope after the implementation stabilizes. Repeat affected checks after defects and renew the complete evidence when a later implementation or contract change invalidates it.

The creator established this platform rule on 2026-11-14. It supersedes Phase 1's earlier representative-only Linux completion boundary. See [verification](verification.md) for the current commands and evidence requirements.

See [GSD workflow](gsd.md) for phase commands and planning artifacts.
