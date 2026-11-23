# Repository layout - Research

## Existing integration points

The five-project solution, evaluated/compiled boundary policy, compiler fixtures, generated-source permissions, case manifests and Node runners contain repository-relative paths. A directory move alone breaks checks. Preserve project names, package locks, case IDs, source/generated semantics and five-project dependency edges while rebinding live paths.

The Godot project currently occupies `src/godot-game`; moving it into `game/` keeps sibling projects and tests outside its resource and default C# source scope. Godot `res://` paths stay unchanged because complete resource contents move together.

The reusable analyzer stays outside the game workspace. It needs explicit shared props/targets and central package policy imports after root build files move. Verify its evaluated imports rather than relying on a successful game-only build.

## .NET discovery constraints

[MSBuild directory customization](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory) documents upward props/targets lookup and explicit path properties. [Central package management](https://learn.microsoft.com/en-us/nuget/consume-packages/central-package-management) documents closest-file package discovery. [SDK selection](https://learn.microsoft.com/en-us/dotnet/core/versions/selection) starts global.json lookup at the CLI working directory. Therefore direct developer commands must run in the workspace; automated .NET launches must select that working directory and preserve rooted subject paths.

NuGet's verified local archive feed must remain tied to the checkout, not accidentally become relative to the new configuration folder. Scratch fixtures must copy policy into the same new relative layout, create destination directories and retain isolated mutation behavior. Solution inventory must resolve project entries relative to the moved solution.

## Documentation and historical evidence

Rebase local Markdown links by resolving their original target and mapping both document and target to their new locations. Update active source/policy manifests and indexes. Do not rewrite captured donor provenance, completed phase summaries, test reports or source identities to resemble new execution. Add a migration map for historical path interpretation. Planning files that are current navigation authorities can have links updated without changing historical claims.

## Verification selection

This is a repository/build change, not a gameplay feature. Follow the ordinary PR selection in the verification reference: strict whole-solution build/analyzers, actual project/code boundaries, documentation and catalogs. Add focused regression checks for changed fixture and runner path semantics, including actual native headless import/launch. Preserve originals and failed attempts. Do not claim rendered/LAN, other-platform or qualitative human acceptance from headless checks.

## Risks

Root-relative strings also occur in synthetic fixtures and inherited analyzer provenance. Change live paths, then inspect relative XML references, solution parsing, mutations and generated allowlist hashes individually. A broad source edit is not permission to alter a captured original document or exempt newly invalid code. Reconcile unfinished Phase 4 paths separately when its branch integrates; this branch deliberately forks from dev.
