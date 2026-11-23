using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Resolves the ignored workspace root <c>.local/</c> for the verification tests.</summary>
/// <remarks>
/// Mirrors <c>src/godot-game/tools/workspace-paths.cjs</c>. <see cref="Shared"/> resolves <c>KZ_SHARED_LOCAL</c>
/// first, then the parent of the git common dir, then the checkout's own <see cref="Local"/>. Tools, downloads and
/// the NuGet packages, HTTP cache and scratch are shared; temp, runs and the verified feed stay per checkout.
/// </remarks>
internal static class WorkspacePaths
{
    /// <summary>The checkout-relative folder where the verification runner publishes its runs.</summary>
    internal const string VerificationRuns = ".local/runs/verification";
    /// <summary>The checkout-relative lock the verification runner owns while it runs.</summary>
    internal const string CheckLock = ".local/locks/check.lock";

    /// <summary>Gets this checkout's own <c>.local</c> folder.</summary>
    internal static string Local => Path.Combine(path1: ProofProtocol.Root, path2: ".local");

    /// <summary>Gets the repository's shared <c>.local</c> folder.</summary>
    internal static string Shared => _shared.Value;

    /// <summary>Gets the shared NuGet global packages folder.</summary>
    internal static string NuGetPackages => Path.Combine(path1: Shared, path2: "nuget", path3: "packages");

    /// <summary>Gets the shared NuGet HTTP cache folder.</summary>
    internal static string NuGetHttp => Path.Combine(path1: Shared, path2: "nuget", path3: "http");

    /// <summary>Gets the shared NuGet scratch folder, required once the packages folder is shared.</summary>
    internal static string NuGetScratch => Path.Combine(path1: Shared, path2: "nuget", path3: "scratch");

    /// <summary>Gets this checkout's verified Godot package feed.</summary>
    internal static string NuGetFeed => Path.Combine(path1: Local, path2: "nuget", path3: "feed");

    /// <summary>Gets this checkout's .NET CLI home.</summary>
    internal static string CliHome => Temp(purpose: "cli-home");

    /// <summary>Gets this checkout's scratch root, which the runners export as TEMP.</summary>
    internal static string TempFolder => Path.Combine(path1: Local, path2: "temp");

    /// <summary>Gets a per-checkout scratch folder for the named purpose.</summary>
    internal static string Temp(string purpose)
    {
        return Path.Combine(path1: TempFolder, path2: purpose);
    }

    /// <summary>Gets the per-checkout runs folder of the named tool.</summary>
    internal static string Runs(string tool)
    {
        return Path.Combine(path1: Local, path2: "runs", path3: tool);
    }

    /// <summary>Gets the shared tools folder with the given name.</summary>
    internal static string Tools(string name)
    {
        return Path.Combine(path1: Shared, path2: "tools", path3: name);
    }

    /// <summary>Maps a historical workspace identity to its current absolute location; other paths pass
    /// through unchanged.</summary>
    internal static string Physical(string identity)
    {
        string logical = identity.Replace(oldChar: '\\', newChar: '/');
        foreach ((string legacy, bool shared, string current) in _legacy)
        {
            if (StringComparer.Ordinal.Equals(x: logical, y: legacy)
                    || logical.StartsWith(value: legacy + "/", comparisonType: StringComparison.Ordinal))
            {
                return Path.Combine(path1: shared ? Shared : Local, path2: current + logical[legacy.Length..]);
            }
        }

        return identity;
    }

    /// <summary>Accepts only an existing file whose path, without links, lies inside the shared packages
    /// folder.</summary>
    internal static bool IsSharedPackage(string fullPath)
    {
        string root = Path.GetFullPath(path: NuGetPackages);
        string file = Path.GetFullPath(path: fullPath);
        string relative = Path.GetRelativePath(relativeTo: root, path: file);
        if (Path.IsPathRooted(path: relative) || relative is "." || relative.Split(separator: Path
                .DirectorySeparatorChar).Any(predicate: part => part is ".." or ".") || !File.Exists(path: file))
        {
            return false;
        }

        string? current = file;
        while (current is not null && current.Length > root.Length)
        {
            if (File.GetAttributes(path: current).HasFlag(flag: FileAttributes.ReparsePoint))
            {
                return false;
            }

            current = Path.GetDirectoryName(path: current);
        }

        return true;
    }

    /// <summary>Applies the shared NuGet folders, the per-checkout CLI home and the resolved shared root to a
    /// child process.</summary>
    internal static void Isolate(ProcessStartInfo start)
    {
        Directory.CreateDirectory(path: CliHome);
        Directory.CreateDirectory(path: NuGetHttp);
        Directory.CreateDirectory(path: NuGetScratch);
        start.Environment[key: "DOTNET_CLI_HOME"] = CliHome;
        start.Environment[key: "NUGET_PACKAGES"] = NuGetPackages;
        start.Environment[key: "NUGET_HTTP_CACHE_PATH"] = NuGetHttp;
        start.Environment[key: "NUGET_SCRATCH"] = NuGetScratch;
        start.Environment[key: "KZ_SHARED_LOCAL"] = Shared;
    }

    /// <summary>Returns why a <c>KZ_SHARED_LOCAL</c> value is refused, or <see langword="null"/> when it is
    /// accepted.</summary>
    /// <remarks>
    /// The one rule, with the same messages as <c>overrideProblem</c> in <c>workspace-paths.cjs</c> and the
    /// <c>node-setup/shared-root</c> scripts. On Windows the value is drive-qualified or UNC (a root-relative or MSYS
    /// <c>/d/...</c> form is refused, never converted); elsewhere it starts with <c>/</c>. It has no <c>.</c> or
    /// <c>..</c> segment, is not a filesystem or drive root, and is neither the checkout nor inside it, except the
    /// checkout's own <c>.local</c> folder.
    /// </remarks>
    internal static string? OverrideProblem(string value, string checkout, bool windows)
    {
        bool absolute = windows ? Matches(input: value, pattern: @"^([A-Za-z]:[\\/]|[\\/]{2}[^\\/]+[\\/][^\\/]+)")
                : value.StartsWith(value: '/');
        if (!absolute)
        {
            return windows ? "must be a drive-qualified or UNC path, not a root-relative or MSYS /d/... form"
                    : "must be an absolute path";
        }

        string normal = NormalOverride(value: value, windows: windows);
        if (Matches(input: normal, pattern: @"(^|/)\.\.?(/|$)"))
        {
            return "must not contain . or .. segments";
        }

        if (windows ? Matches(input: normal, pattern: "^[A-Za-z]:$|^//[^/]+/[^/]+$") : normal.Length == 0)
        {
            return "must not be a filesystem or drive root";
        }

        string target = (windows ? normal.ToLowerInvariant() : normal) + "/";
        string normalCheckout = NormalOverride(value: checkout, windows: windows);
        string root = (windows ? normalCheckout.ToLowerInvariant() : normalCheckout) + "/";
        if (target.StartsWith(value: root + ".local/", comparisonType: StringComparison.Ordinal))
        {
            return null;
        }

        return target.StartsWith(value: root, comparisonType: StringComparison.Ordinal)
                ? "must not be the checkout or inside it, except its .local folder" : null;
    }

    // Ordered most specific first, so the first matching row is the longest legacy prefix.
    private static readonly (string Legacy, bool Shared, string Current)[] _legacy =
    [
        (".phase1/cache/phase4-exports", true, "downloads/export-templates"),
        (".phase1/cache", true, "downloads/godot"),
        (".phase1/tools/linux-vm", true, "tools/linux-vm"),
        (".phase1/tools/linux-prep", true, "downloads/linux-prep"),
        (".phase1/tools", true, "tools/godot"),
        (".phase1/feed", false, "nuget/feed"),
        (".phase1/packages", true, "nuget/packages"),
        (".phase1/http-cache", true, "nuget/http"),
        (".phase1/cli-home", false, "temp/cli-home"),
        (".phase1/dotnet-home", false, "temp/cli-home"),
        (".phase1/temp", false, "temp"),
        (".phase1/runs", false, "runs/verification"),
        (".phase1/check.lock", false, "locks/check.lock"),
        (".phase1", false, "temp/verification"),
        (".phase2/runs", false, "runs/gameplay-checks"),
        (".phase2/completions", false, "runs/gameplay-checks/completions"),
        (".phase2/desktop.lock", true, "locks/desktop.lock"),
        (".phase2", false, "temp/gameplay-checks"),
        (".phase3/runs", false, "runs/lan-checks"),
        (".phase3", false, "temp/lan-checks"),
        (".phase4/runs", false, "runs/asset-pipeline"),
        (".phase4/acceptance", false, "runs/asset-pipeline/acceptance"),
        (".phase4", false, "temp/asset-pipeline"),
    ];

    private static readonly Lazy<string> _shared = new(valueFactory: ResolveShared);

    // A path for comparison only: on Windows backslashes become '/'; repeated separators collapse except a leading
    // '//'; trailing separators are dropped.
    private static string NormalOverride(string value, bool windows)
    {
        string text = windows ? value.Replace(oldChar: '\\', newChar: '/') : value;
        bool unc = text.StartsWith(value: "//", comparisonType: StringComparison.Ordinal);
        text = Regex.Replace(input: text, pattern: "/{2,}", replacement: "/", options: RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(value: 1));
        return ((unc ? "/" : string.Empty) + text).TrimEnd(trimChar: '/');
    }

    private static bool Matches(string input, string pattern)
    {
        return Regex.IsMatch(input: input, pattern: pattern, options: RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(value: 1));
    }

    private static string ResolveShared()
    {
        string? configured = Environment.GetEnvironmentVariable(variable: "KZ_SHARED_LOCAL");
        if (!string.IsNullOrEmpty(value: configured))
        {
            string? problem = OverrideProblem(value: configured, checkout: ProofProtocol.Root,
                    windows: OperatingSystem.IsWindows());
            return problem is null ? Path.GetFullPath(path: configured)
                    : throw new InvalidOperationException(message: "SHARED_ROOT_INVALID: KZ_SHARED_LOCAL " + problem
                            + ": " + configured);
        }

        string? common = GitCommonDirectory();
        if (common is null)
        {
            return Local;
        }

        return StringComparer.Ordinal.Equals(x: Path.GetFileName(path: common), y: ".git")
                ? Path.Combine(path1: Path.GetDirectoryName(path: common) ?? common, path2: ".local")
                : throw new InvalidOperationException(message: "SHARED_ROOT_UNRESOLVED: git common dir " + common
                        + " is not a .git folder; set KZ_SHARED_LOCAL");
    }

    private static string? GitCommonDirectory()
    {
        string[] arguments = ["-C", ProofProtocol.Root, "rev-parse", "--path-format=absolute", "--git-common-dir"];
        (int exitCode, string output) = RunGit(arguments: arguments);
        string common = output.Trim();
        return exitCode == 0 && common.Length > 0 ? Path.GetFullPath(path: common) : null;
    }

    // Without Git a linked worktree would silently fall back to its own .local; a missing git is a named refusal.
    private static (int ExitCode, string Output) RunGit(string[] arguments)
    {
        var start = new ProcessStartInfo(fileName: "git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (string argument in arguments)
        {
            start.ArgumentList.Add(item: argument);
        }

        Process? started;
        try
        {
            started = Process.Start(startInfo: start);
        }
        catch (Win32Exception exception)
        {
            throw new InvalidOperationException(message: "GIT_MISSING: git could not start, so the shared .local root "
                    + "cannot be resolved; put Git on PATH or set KZ_SHARED_LOCAL", innerException: exception);
        }

        using Process process = started
                ?? throw new InvalidOperationException(message: "SHARED_ROOT_UNRESOLVED: git failed to start");
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(milliseconds: 5000))
        {
            process.Kill(entireProcessTree: true);
            process.WaitForExit();
            throw new TimeoutException(message: "SHARED_ROOT_UNRESOLVED: git exceeded 5 seconds");
        }

        _ = error.GetAwaiter().GetResult();
        return (process.ExitCode, output.GetAwaiter().GetResult());
    }
}
