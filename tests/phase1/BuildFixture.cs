using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Copies the real Core consumer and owned analyzer inputs for isolated compiler controls.</summary>
internal sealed class BuildFixture
{
    /// <summary>Returns a compliant handwritten source for clean controls.</summary>
    internal const string CleanSource = "namespace KingdomZombies.Core;\n\n"
            + ("/// <summary>Isolated real consumer.</summary>\ninternal" + " static class Subject\n{\n")
            + "    internal static int Read()\n    {\n        return 1;\n    }\n}\n";
    /// <summary>Creates a scratch consumer with the real project, policy and canonical dependency locks.</summary>
    internal BuildFixture()
    {
        _directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/build-" + Guid.NewGuid()
                .ToString(format: "N"));
        Directory.CreateDirectory(path: _directory);
        foreach (string file in _sharedFiles)
        {
            File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: file), destFileName: Path
                    .Combine(path1: _directory, path2: file));
        }

        CopyTree(relative: "src/core");
        CopyTree(relative: "tools/code-rules");
        _project = Path.Combine(path1: _directory, path2: "src/core/KingdomZombies.Core.csproj");
        Write(source: CleanSource);
    }

    /// <summary>Gets the scratch root for controlled policy mutations.</summary>
    internal string DirectoryPath => _directory;

    /// <summary>Writes the exact subject bytes without normalizing away a violating source control.</summary>
    internal void Write(string source)
    {
        File.WriteAllText(path: Path.Combine(path1: _directory, path2: "src/core/Subject.cs"),
                contents: source, encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    /// <summary>Requires successful canonical locked restore before any diagnostic assertion.</summary>
    internal void Restore()
    {
        (int Exit, string Output) result = Run("restore", _project, "--locked-mode", "--disable-parallel",
                "-p:NuGetAudit=false");
        result.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_RESTORE: " + result.Output);
    }

    /// <summary>Builds the exact copied consumer without shared compiler or reusable MSBuild nodes.</summary>
    internal (int Exit, string Output) Build(params string[] overrides)
    {
        return Run(arguments: ["build", _project, "--no-restore", "--disable-build-servers",
            "-nodeReuse:false", "-p:UseSharedCompilation=false", "-p:NuGetAudit=false", "-v:minimal", ..overrides]);
    }

    /// <summary>Checks identity, subject, severity and exact start coordinates after infrastructure succeeds.</summary>
    internal static bool HasDiagnostic((int Exit, string Output) result, string id, int line, int column)
    {
        string subject = "Subject.cs(" + line.ToString(provider: System.Globalization.CultureInfo
                .InvariantCulture) + "," + column.ToString(provider: System.Globalization.CultureInfo
                        .InvariantCulture) + "): error " + id + ":";
        return result.Exit != 0 && result.Output.Contains(value: subject, comparisonType: StringComparison
                .Ordinal) && !Regex.IsMatch(input: result.Output,
                        pattern: @"error (NU\d+|MSB\d+|CS8032|AD0001):", options: RegexOptions.CultureInvariant
                                | RegexOptions.ExplicitCapture, matchTimeout: TimeSpan.FromSeconds(value: 1));
    }

    private readonly string _directory;
    private readonly string _project;
    private static readonly string[] _sharedFiles = ["Directory.Build.props", "Directory.Build.targets",
        "Directory.Packages.props", ".editorconfig", "global.json", "NuGet.config", "BannedSymbols.txt",
        "BannedSymbols.Tests.txt", "BannedSymbols.Pure.txt"];
    private void CopyTree(string relative)
    {
        string source = Path.Combine(path1: ProofProtocol.Root, path2: relative);
        foreach (string file in Directory.EnumerateFiles(path: source, searchPattern: "*",
                searchOption: SearchOption.AllDirectories))
        {
            string local = Path.GetRelativePath(relativeTo: source, path: file);
            if (local.Split(separator: Path.DirectorySeparatorChar).Any(predicate: part => part is "bin" or "obj"))
            {
                continue;
            }

            string target = Path.Combine(path1: _directory, path2: relative, path3: local);
            Directory.CreateDirectory(path: Path.GetDirectoryName(path: target)
                    ?? throw new InvalidOperationException(message: "Missing fixture parent"));
            File.Copy(sourceFileName: file, destFileName: target);
        }
    }

    private (int Exit, string Output) Run(params string[] arguments)
    {
        var start = new ProcessStartInfo(fileName: "dotnet")
        {
            WorkingDirectory = _directory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string name in start.Environment.Keys.ToArray())
        {
            if (name.StartsWith(value: "MSBUILD", comparisonType: StringComparison.OrdinalIgnoreCase) || name
                    .StartsWith(value: "COR_", comparisonType: StringComparison.OrdinalIgnoreCase) || name
                            .StartsWith(value: "CORECLR_", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                start.Environment.Remove(key: name);
            }
        }

        start.Environment[key: "DOTNET_CLI_HOME"] = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/cli-home");
        start.Environment[key: "NUGET_PACKAGES"] = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/packages");
        start.Environment[key: "MSBUILDDISABLENODEREUSE"] = "1";
        foreach (string argument in arguments)
        {
            start.ArgumentList.Add(item: argument);
        }

        using Process child = Process.Start(startInfo: start) ?? throw new InvalidOperationException(
                message: "Compiler failed to start");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 60000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "Isolated compiler exceeded 60 seconds");
        }

        string text = output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult();
        File.WriteAllText(path: Path.Combine(path1: _directory, path2: Guid.NewGuid().ToString(format: "N")
                + ".process.log"), contents: text);
        return (child.ExitCode, text);
    }
}
