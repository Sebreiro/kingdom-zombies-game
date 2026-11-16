# Tool reorganization review

Reviewed inline, 2026-11-16. No separate human qualitative acceptance is claimed.

## Repairs and checks

- Moved 43 game-specific tool/helper files without compatibility stubs; reusable analyzer sources remain outside the game workspace.
- Rebased entry-point __dirname roots, cross-folder requires, driver/catalog paths and C# fixture program locations. Added command tests launched from an unrelated working directory.
- Decoupled historical evidence family IDs from current folder basenames in inherited execution and validation; retained fail-closed family selection.
- Removed overlapping source fingerprint prefixes after all game tools moved under src. Seeded actual toolchain metadata and fixture directories explicitly instead of depending on incidental fingerprint placeholders.
- Updated the imported LAN SourceLink target's repository-root calculation from two to four parent directories. Existing real SDK tests verify reviewed source stamps independently of metadata commits.
- Scanned the compiled test assembly for old tool paths, catching two concatenated script constants that text-only path replacement missed.
- Updated root/workspace guides, live module policy links, command examples and scenario/selection definitions. Historical completed quick/phase reports remain unchanged; their prior fingerprints can correctly become stale.

## Final selection

Strict solution build and locked restore; 25 tool self-tests including folder ownership and alternate-cwd entry points; 36 documentation, 27 compiler-policy and 36 runner cases; eight current graph/registry/source/suppression/native cases (107 required declarations total); actual headless toolchain bootstrap. The ordinary repository/build selection applies. No rendered/LAN, Linux, coverage-certification or human-review pass is inferred.

## Publication

Continue `feature/quick-repository-layout` and draft PR #6 to dev. Return that PR to ready after fresh verification. Preserve the separate active Phase 4 checkout; document its future `asset-pipeline` location without importing its unfinished source.
