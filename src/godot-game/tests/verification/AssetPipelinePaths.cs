using System;
using System.Linq;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves recorded repository paths to the physical files of the current layout.</summary>
/// <remarks>
/// <see cref="Physical"/> mirrors <c>src/godot-game/tools/asset-pipeline/layout.cjs</c>: Phase 4 content and pack
/// records keep their logical <c>src/godot-game/...</c>, <c>production/phase4/...</c> and <c>tools/phase4/...</c>
/// identities. <see cref="Current"/> additionally applies the layout policy's historical-path map to captured
/// producer provenance, whose recorded paths keep their original bytes.
/// </remarks>
internal static class AssetPipelinePaths
{
    /// <summary>Maps a logical record path to its physical repository-relative path; idempotent.</summary>
    internal static string Physical(string path)
    {
        foreach ((string logical, string physical) in _folders)
        {
            if (Within(path: path, folder: logical))
            {
                return physical + path[logical.Length..];
            }
        }

        return path.StartsWith(value: Workspace, comparisonType: StringComparison.Ordinal)
                && IsGameEntry(relative: path[Workspace.Length..]) ? Game + path[Workspace.Length..] : path;
    }

    /// <summary>Maps a captured historical repository path to its current physical path; idempotent.</summary>
    internal static string Current(string path)
    {
        string physical = Physical(path: path);
        if (!StringComparer.Ordinal.Equals(x: physical, y: path))
        {
            return physical;
        }

        foreach ((string historical, string current) in _historical)
        {
            if (Within(path: path, folder: historical))
            {
                return current + path[historical.Length..];
            }
        }

        if (_workspaceFiles.Contains(value: path, comparer: StringComparer.Ordinal))
        {
            return Workspace + path;
        }

        if (_buildFiles.Contains(value: path, comparer: StringComparer.Ordinal))
        {
            return Workspace + "build/" + path;
        }

        return path.StartsWith(value: Workspace, comparisonType: StringComparison.Ordinal)
                && !path[Workspace.Length..].Contains(value: '/') && path.EndsWith(value: ".cs",
                        comparisonType: StringComparison.Ordinal) ? Game + path[Workspace.Length..] : path;
    }

    private const string Workspace = "src/godot-game/";
    private const string Game = Workspace + "game/";

    private static readonly (string Logical, string Physical)[] _folders =
    [
        ("tools/phase4", Workspace + "tools/asset-pipeline"),
        ("production/phase4", Workspace + "production/assets"),
    ];

    private static readonly string[] _gameFolders = ["assets", "content", "inspection", "foundation", "proofs", "docs"];

    private static readonly string[] _gameFiles = ["project.godot", "export_presets.cfg", "KingdomZombies.csproj",
        "override.cfg"];

    private static readonly (string Historical, string Current)[] _historical =
    [
        ("src/core", Workspace + "core"),
        ("src/multiplayer-abstractions", Workspace + "multiplayer-abstractions"),
        ("tests/phase1", Workspace + "tests/verification"),
        ("tools/phase1", Workspace + "tools/verification"),
        ("tools/phase2", Workspace + "tools/gameplay-checks"),
        ("tools/phase3", Workspace + "tools/lan-checks"),
        ("tools/code-rules", Workspace + "tools/code-rules"),
        (Workspace + ".godot", Game + ".godot"),
    ];

    private static readonly string[] _workspaceFiles = ["KingdomZombies.slnx", "Directory.Build.props",
        "Directory.Build.targets", "Directory.Packages.props", "global.json", "NuGet.config"];

    private static readonly string[] _buildFiles = ["BannedSymbols.txt", "BannedSymbols.Tests.txt",
        "BannedSymbols.Pure.txt", "Directory.Xunit.targets"];

    private static bool Within(string path, string folder)
    {
        return StringComparer.Ordinal.Equals(x: path, y: folder)
                || path.StartsWith(value: folder + "/", comparisonType: StringComparison.Ordinal);
    }

    private static bool IsGameEntry(string relative)
    {
        return _gameFolders.Any(predicate: folder => Within(path: relative, folder: folder))
                || _gameFiles.Contains(value: relative, comparer: StringComparer.Ordinal);
    }
}
