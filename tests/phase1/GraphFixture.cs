using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Compiles real isolated graph controls with the unchanged inherited build policy.</summary>
internal sealed class GraphFixture
{
    /// <summary>Copies the real product projects, locks and authentic analyzer producer.</summary>
    internal GraphFixture()
    {
        var fixture = new BuildFixture(game: true);
        Root = fixture.DirectoryPath;
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN: " + clean.Output);
    }

    /// <summary>Gets the independently copied subject root.</summary>
    internal string Root { get; }

    /// <summary>Adds a compilable Core to engine reference without a dependency cycle.</summary>
    internal void AddDirectEngineEdge()
    {
        string game = Path.Combine(path1: Root, path2: "src/godot-game/KingdomZombies.csproj");
        string text = File.ReadAllText(path: game);
        File.WriteAllText(path: game, contents: text.Replace(oldValue:
                "    <ProjectReference Include=\"../core/KingdomZombies.Core.csproj\" />\n", newValue: "",
                comparisonType: StringComparison.Ordinal));
        string core = Path.Combine(path1: Root, path2: "src/core/KingdomZombies.Core.csproj");
        File.WriteAllText(path: core, contents: File.ReadAllText(path: core).Replace(oldValue: "</Project>",
                newValue: "<ItemGroup><ProjectReference Include=\"../godot-game/KingdomZombies.csproj\"/>"
                        + "</ItemGroup></Project>", comparisonType: StringComparison.Ordinal));
        Run("restore", core, "--force-evaluate", "-p:NuGetAudit=false").Exit.ShouldBe(expected: 0);
        Run("restore", core, "--locked-mode", "-p:NuGetAudit=false").Exit.ShouldBe(expected: 0);
        (int Exit, string Output) changed = Run("build", core, "--no-restore", "--disable-build-servers",
                "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        changed.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + changed.Output);
    }

    /// <summary>Runs only an owned child with captured logs and a bounded process-tree lifetime.</summary>
    internal (int Exit, string Output) Run(params string[] arguments)
    {
        return RunAt(root: Root, arguments: arguments);
    }

    /// <summary>Runs a bounded SDK or inventory query in an explicit root.</summary>
    internal static (int Exit, string Output) RunAt(string root, string[] arguments, string executable = "dotnet")
    {
        var start = new ProcessStartInfo(fileName: executable)
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string name in start.Environment.Keys.ToArray())
        {
            if (name.StartsWith(value: "MSBUILD", comparisonType: StringComparison.OrdinalIgnoreCase)
                    || name.StartsWith(value: "COR_", comparisonType: StringComparison.OrdinalIgnoreCase)
                    || name.StartsWith(value: "CORECLR_", comparisonType: StringComparison.OrdinalIgnoreCase))
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

        using Process child = Process.Start(startInfo: start)
                ?? throw new InvalidOperationException(message: "GRAPH_INFRASTRUCTURE: child failed to start");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 60000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "GRAPH_INFRASTRUCTURE: child exceeded 60 seconds");
        }

        string text = output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult();
        string logs = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp");
        Directory.CreateDirectory(path: logs);
        File.WriteAllText(path: Path.Combine(path1: logs, path2: "graph-" + Guid.NewGuid().ToString(format: "N")
                + ".log"), contents: text);
        return (child.ExitCode, text);
    }
}
