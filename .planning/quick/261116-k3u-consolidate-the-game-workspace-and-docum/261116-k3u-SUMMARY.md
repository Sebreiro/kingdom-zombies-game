---
task_id: 261116-k3u
status: complete
completed: 2026-11-16
implementation_revision: de5f40f81e2737e4097c25cd57a17e44643136a0
branch: feature/quick-repository-layout
pull_request: https://github.com/Sebreiro/kingdom-zombies-game/pull/6
---

# Complete game workspace and PR review entry point

The complete authored game workspace now lives in `src/godot-game`: Godot runtime/resources in `game/`, pure state in `core/`, service contracts in `multiplayer-abstractions/`, C# checks in `tests/verification/`, reference artwork in `references/`, and specialized compiler policy in `build/`. The solution, SDK pin, NuGet sources, local tool manifest and conventional shared .NET files are beside these folders.

The five C# project identities, dependency directions, rule meanings, case IDs and package versions remain unchanged. Reusable analyzer implementation and phase-based tool paths remain in `tools/`; the creator requested explanation before considering their reorganization. Runners, isolated fixtures, evaluated solution inventory, source permissions and live navigation resolve the new layout. Root/editor configuration still covers repository-wide formatting. Existing ignored run/cache locations remain stable.

Added repository/workspace/build/tooling guides and `docs/architecture/repository-layout.md`. External PR review starts at `docs/workflow/code-review.md`, linked from AGENTS and the code/architecture indexes. That guide links to actual rules, exceptions, decisions and evidence policy and preserves the separate human qualitative review boundary.

## Delivered commits

- `ef6a2d8`: agreed context, focused research, plan and inline plan check.
- `e81a546`: initial workspace relocation and draft PR lifecycle.
- `6825036`: fixture/enforcement repairs, references, guides and review entry point.
- `b4d20b8`: remaining test-generated path and linked-source ownership resolution fixes.
- `de5f40f`: required continuation formatting correction; final tested implementation revision.

Completion artifacts and the quick-task state row are metadata after the tested implementation. They do not create a new game execution or rewrite prior evidence.

## Verification

- Locked restore and strict whole-solution build passed with zero warnings/errors in 12.25 seconds using existing verified caches.
- Final required selection passed 73/73 in 229.05 seconds: 36 docs, 27 compiler-policy cases, actual evaluated/compiled boundaries, captured/current register bindings, source format, suppression and conditional ownership, native lifecycle and generated origin. All reports share one source fingerprint and revision.
- Layout/tool self-tests passed 23/23 in 29.79 seconds, alongside the selection.
- Confirmed exact bytes in 109 moved runtime/resource/lock/policy files, all sixteen analyzer implementation files and captured provenance.
- The broader diagnostic selection on `6825036` exercised 203 declarations: 200 passes, two test-path failures subsequently repaired, and the existing Phase 5 coverage declaration blocked as pending. It includes all 27 graph, 36 runner and 46 active register checks. This diagnostic run is not reported as a successful complete run or republished against the repaired source.

Machine-readable report identities, hashes, commands, timings, source revision, original failures and scope limits are in [CHECKS](261116-k3u-CHECKS.json). [Review](261116-k3u-REVIEW.md) records the repaired findings. Verification uses development Windows without real game windows. No new Linux, rendered/LAN, measured-coverage or human qualitative pass is claimed.

## Publication and continuation

PR [#6](https://github.com/Sebreiro/kingdom-zombies-game/pull/6), targeting `dev`, was created as a draft at the first implementation push and is now ready for review. It remains unmerged; the creator owns that decision.

Checkout: `C:/Users/User/AppData/Local/Temp/kz-repository-layout`. The original checkout remains on its independently active Phase 4 branch; none of its implementation or planning work was moved into this dev-based branch. Apply the documented migration when integrating that work. Root reference/build/test folders disappear from a fresh checkout of this branch; old immutable reports keep their historical paths and identities.
