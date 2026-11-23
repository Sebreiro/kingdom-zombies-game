using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks exact source bytes against the inherited project text-format contract.</summary>
internal static class SourceFileFormat
{
    /// <summary>Counts raw bytes in the Git text probe; only indices below this limit affect classification.</summary>
    internal const int TextProbeLength = 8000;
    /// <summary>Names each observed lock permission independently; no filename-wide exception exists.</summary>
    internal static IReadOnlyDictionary<string, string> LockExemptions => _lockExemptions;

    /// <summary>Returns an actionable rule and byte location for each invalid source representation.</summary>
    internal static IReadOnlyList<string> Check(string path, byte[] content,
            IReadOnlySet<string>? finalNewlineExemptions = null)
    {
        var errors = new List<string>();
        CheckEncoding(path: path, content: content, errors: errors);
        AddFirst(path: path, content: content, value: (byte)'\r', rule: "R-03", diagnostic: "FORMAT_LF",
                errors: errors);
        AddFirst(path: path, content: content, value: (byte)'\t', rule: "R-06", diagnostic: "FORMAT_TAB",
                errors: errors);
        for (int index = 0; index < content.Length; index++)
        {
            if (content[index] is (byte)' ' or (byte)'\t'
                    && (index == content.Length - 1 || content[index + 1] == (byte)'\n'))
            {
                errors.Add(item: At(rule: "R-05", diagnostic: "FORMAT_TRAILING_WHITESPACE", path: path, index: index));
            }
        }

        if (content.Length > 0 && content[^1] != (byte)'\n' && !(finalNewlineExemptions?.Contains(item: path) ?? false))
        {
            long end = content.LongLength;
            errors.Add(item: At(rule: "R-04", diagnostic: "FORMAT_FINAL_NEWLINE", path: path, index: end));
        }

        return errors;
    }

    /// <summary>Reads only the five exact observed and registered final-newline permissions.</summary>
    internal static IReadOnlySet<string> ReadFinalNewlineExemptions()
    {
        JsonObject registry = RegisterValidator.Read(relative: "docs/code/exemptions.json");
        var paths = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? row in registry["local"]?.AsArray()
                ?? throw new InvalidDataException(message: "FORMAT_EXEMPTIONS_REQUIRED"))
        {
            if (!StringComparer.Ordinal.Equals(x: row?["rule"]?.GetValue<string>(), y: "R-04"))
            {
                continue;
            }

            string id = row?["id"]?.GetValue<string>() ?? "";
            string path = row?["path"]?.GetValue<string>() ?? "";
            if (!_lockExemptions.TryGetValue(key: id, value: out string? expected)
                    || !StringComparer.Ordinal.Equals(x: path, y: expected)
                    || !StringComparer.Ordinal.Equals(x: row?["sourceId"]?.GetValue<string>(), y: "EX-11")
                    || !StringComparer.Ordinal.Equals(x: row?["diagnostic"]?.GetValue<string>(),
                            y: "FORMAT_FINAL_NEWLINE") || !paths.Add(item: path))
            {
                throw new InvalidDataException(message: "FORMAT_EXEMPTION_UNOBSERVED: " + id);
            }
        }

        if (!paths.SetEquals(other: _lockExemptions.Values))
        {
            throw new InvalidDataException(message: "FORMAT_EXEMPTION_MISSING");
        }

        return paths;
    }

    /// <summary>Checks every tracked project text file, preserving installed-tool and binary boundaries.</summary>
    internal static IReadOnlyList<string> CheckRepository()
    {
        IReadOnlySet<string> exemptions = ReadFinalNewlineExemptions();
        var errors = new List<string>();
        foreach (string path in TrackedPaths())
        {
            if (!IsChecked(path: path))
            {
                continue;
            }

            string file = Path.Combine(path1: ProofProtocol.Root, path2: path);
            if (!File.Exists(path: file) || File.GetAttributes(path: file).HasFlag(flag: FileAttributes.ReparsePoint))
            {
                errors.Add(item: "R-02 FORMAT_TRACKED_FILE " + path);
                continue;
            }

            byte[] bytes = File.ReadAllBytes(path: file);
            if (IsText(content: bytes))
            {
                errors.AddRange(collection: Check(path: path, content: bytes, finalNewlineExemptions: exemptions));
            }
        }

        return errors;
    }

    /// <summary>Requires selected tracked paths to exist and checks unknown text extensions by content.</summary>
    internal static IReadOnlyList<string> CheckTracked(string path, byte[]? content)
    {
        if (!IsChecked(path: path))
        {
            return [];
        }

        if (content is null)
        {
            return ["R-02 FORMAT_TRACKED_FILE " + path];
        }

        return IsText(content: content) ? Check(path: path, content: content) : [];
    }

    /// <summary>Uses Git's first-8000-byte NUL rule, with an exact exclusive upper boundary.</summary>
    internal static bool IsText(byte[] content)
    {
        return !content.AsSpan(start: 0, length: Math.Min(val1: content.Length, val2: TextProbeLength))
                .Contains(value: (byte)0);
    }

    /// <summary>Checks new text extensions while excluding only installed tooling and known binary formats.</summary>
    internal static bool IsChecked(string path)
    {
        return !_skippedFolders.Any(predicate: folder => path.StartsWith(value: folder,
                comparisonType: StringComparison.Ordinal)) && !_binaryExtensions.Contains(item: Path.GetExtension(
                        path: path));
    }

    /// <summary>Requires every declared setting occurrence to retain its exact approved value.</summary>
    internal static void RequireEditorSetting(string source, string name, string expected)
    {
        Match[] declarations = [..Regex.Matches(input: source,
                pattern: @"^[ \t]*" + Regex.Escape(str: name) + @"[ \t]*=[ \t]*(?<value>[^\r\n]*)$",
                options: RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
                        | RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking,
                matchTimeout: TimeSpan.FromSeconds(value: 1)).Cast<Match>()];
        if (declarations.Length == 0 || declarations.Any(predicate: match => !StringComparer.OrdinalIgnoreCase
                .Equals(x: match.Groups[groupname: "value"].Value.Trim(), y: expected)))
        {
            throw new InvalidDataException(message: "FORMAT_SETTING .editorconfig " + name);
        }
    }

    /// <summary>Returns the exact tracked project inventory for required source-policy checks.</summary>
    internal static string[] ReadTrackedPaths()
    {
        return TrackedPaths();
    }

    private static readonly IReadOnlyDictionary<string, string> _lockExemptions = new Dictionary<string, string>(
            comparer: StringComparer.Ordinal)
    {
        ["KZ-EX-LOCK-01"] = "src/godot-game/tests/verification/packages.lock.json",
        ["KZ-EX-LOCK-CORE-01"] = "src/godot-game/core/packages.lock.json",
        ["KZ-EX-LOCK-GODOT-01"] = "src/godot-game/game/packages.lock.json",
        ["KZ-EX-LOCK-SERVICES-01"] = "src/godot-game/multiplayer-abstractions/packages.lock.json",
        ["KZ-EX-LOCK-ANALYZERS-01"] = "src/godot-game/tools/code-rules/packages.lock.json"
    };
    private static readonly string[] _skippedFolders = [".claude/", ".codex/", ".agents/", ".planning/"];
    private static readonly HashSet<string> _binaryExtensions = new(comparer: StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".webp", ".pdf", ".zip", ".gz", ".tgz", ".7z",
        ".jar", ".dll", ".exe", ".so", ".dylib", ".nupkg", ".snupkg", ".snk", ".pfx", ".woff", ".woff2",
        ".ttf", ".otf", ".eot", ".mp3", ".mp4", ".wav"
    };
    private static readonly byte[] _bom = [0xEF, 0xBB, 0xBF];
    private static readonly UTF8Encoding _strictUtf8 = new(encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true);
    private static string At(string rule, string diagnostic, string path, long index)
    {
        return rule + " " + diagnostic + " " + path + "(byte " + (index + 1).ToString(
                provider: CultureInfo.InvariantCulture) + ")";
    }

    private static void CheckEncoding(string path, byte[] content, List<string> errors)
    {
        if (content.AsSpan().StartsWith(value: _bom))
        {
            errors.Add(item: At(rule: "R-02", diagnostic: "FORMAT_BOM", path: path, index: 0));
        }

        try
        {
            _strictUtf8.GetCharCount(bytes: content);
        }
        catch (DecoderFallbackException error)
        {
            errors.Add(item: At(rule: "R-02", diagnostic: "FORMAT_UTF8", path: path, index: error.Index));
        }
    }

    private static void AddFirst(string path, byte[] content, byte value, string rule, string diagnostic,
            List<string> errors)
    {
        int index = Array.IndexOf(array: content, value: value);
        if (index >= 0)
        {
            errors.Add(item: At(rule: rule, diagnostic: diagnostic, path: path, index: index));
        }
    }

    private static string[] TrackedPaths()
    {
        var start = new ProcessStartInfo(fileName: "git")
        {
            WorkingDirectory = ProofProtocol.Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = _strictUtf8,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add(item: "ls-files");
        start.ArgumentList.Add(item: "-z");
        using Process child = Process.Start(startInfo: start)
                ?? throw new InvalidOperationException(message: "FORMAT_GIT_START");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 5000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "FORMAT_GIT_TIMEOUT");
        }

        if (child.ExitCode != 0)
        {
            throw new InvalidOperationException(message: "FORMAT_GIT_FAILED: " + error.GetAwaiter().GetResult());
        }

        string[] paths = output.GetAwaiter().GetResult().Split(separator: '\0', options: StringSplitOptions
                .RemoveEmptyEntries);
        if (paths.Length == 0 || paths.Any(predicate: path => path.StartsWith(value: '/') || path.Contains(
                value: '\\') || path.Contains(value: ':') || path.Split(separator: '/').Any(predicate: part =>
                        part is "" or "." or "..")))
        {
            throw new InvalidDataException(message: "FORMAT_GIT_PATHS_INVALID");
        }

        return paths;
    }
}
