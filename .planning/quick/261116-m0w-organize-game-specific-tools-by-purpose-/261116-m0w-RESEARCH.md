# Game tools - Focused research

The phase-named runners are coupled to this game's project inventory, case catalogs, native scenes and evidence contracts. They are game-specific. Deeper inspection also found exact KingdomZombies/Godot generated-source permissions embedded in two analyzer helpers. Its core rules have no game assembly dependency, but the configured package is game-specific and should move into the workspace too. Its shared compiler/package policy can then use normal directory inheritance instead of external-project import overrides.

Moving runners two levels deeper changes `__dirname`-based checkout-root discovery, cross-folder require paths, native driver locations, C# child-process fixtures and test-generated programs. Some fixtures infer protocol families from directory basenames; decouple that inference while retaining original report family names. Treat `path.resolve(__dirname, ...)` separately from root-relative data paths and module-relative require paths.

After moving tools under src, current fingerprints that include both src and explicit tool directories overlap. Keep complete source coverage while removing duplicate nested inputs in every producer, reconciler and synthetic fixture. Test fixture input placeholders still need actual moved catalogs and helper files.

Rebase Markdown links by resolving original document and target locations. Update current policy/module declarations and saved live scenario/selection definitions, leaving immutable provenance/review/run records and earlier quick-task reports intact. Add a migration entry for old command paths.

Use strict whole-solution build, complete docs/pins/runner groups, current project/type/registry/suppression checks, all tool self-tests and actual headless bootstrap. No gameplay source, package version or rule meaning changes; ordinary repository/build PR selection applies.
