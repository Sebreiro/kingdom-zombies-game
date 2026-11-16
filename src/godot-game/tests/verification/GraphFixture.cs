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
        Root = Path.GetFullPath(path: fixture.DirectoryPath);
        CopyInventory();
        fixture.Restore();
        (int Exit, string Output) clean = fixture.Build();
        clean.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN: " + clean.Output);
    }

    /// <summary>Gets the independently copied subject root.</summary>
    internal string Root { get; }

    /// <summary>Creates and compiles an evaluated edge from the requested real import or item form.</summary>
    internal void AddBoundaryMutation(string mutation)
    {
        RetainCleanCoreConsumer();
        string item = "<ItemGroup><ProjectReference Include=\"../game/KingdomZombies.csproj\"/>"
                + "</ItemGroup>";
        if (mutation is "props" or "targets")
        {
            Write(relative: "src/godot-game/core/boundary." + mutation, content: "<Project>" + item + "</Project>\n");
            Append(project: ProjectEvaluator.Expected[0], xml: "<Import Project=\"boundary." + mutation + "\"/>");
        }
        else if (mutation is "transitive")
        {
            RetainCleanPortsConsumer();
            Append(project: ProjectEvaluator.Expected[0], xml: "<ItemGroup><ProjectReference Include=\""
                    + "../multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj\"/></ItemGroup>");
            Append(project: ProjectEvaluator.Expected[1], xml: item);
        }
        else if (mutation is "undeclared")
        {
            Write(relative: "src/godot-game/integration/Hidden.csproj", content: "<Project Sdk=\"Microsoft.NET.Sdk\">"
                    + "<PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>\n");
            Append(project: ProjectEvaluator.Expected[0], xml: "<ItemGroup><ProjectReference Include=\""
                    + "../integration/Hidden.csproj\"/></ItemGroup>");
        }
        else
        {
            Append(project: ProjectEvaluator.Expected[0], xml: EdgeItem(mutation: mutation, item: item));
        }

        Compile(project: ProjectEvaluator.Expected[0], restore: true);
    }

    /// <summary>Appends only the controlled XML to a copied project.</summary>
    internal void Append(string project, string xml)
    {
        string path = Path.Combine(path1: Root, path2: project);
        Write(relative: project, content: File.ReadAllText(path: path).Replace(oldValue: "</Project>",
                newValue: xml + "</Project>", comparisonType: StringComparison.Ordinal));
    }

    /// <summary>Writes only into the isolated fixture root.</summary>
    internal void Write(string relative, string content)
    {
        string path = Path.GetFullPath(path: Path.Combine(path1: Root, path2: relative));
        if (!path.StartsWith(value: Root + Path.DirectorySeparatorChar, comparisonType: StringComparison.Ordinal))
        {
            throw new InvalidDataException(message: "GRAPH_FIXTURE_PATH: " + relative);
        }

        Directory.CreateDirectory(path: Path.GetDirectoryName(path: path)
                ?? throw new InvalidDataException(message: "Missing fixture parent"));
        File.WriteAllText(path: path, contents: content);
    }

    /// <summary>Requires successful subject compilation after optional changed-graph lock regeneration.</summary>
    internal void Compile(string project, bool restore = false)
    {
        if (restore)
        {
            Run("restore", project, "--force-evaluate", "-p:NuGetAudit=false").Exit.ShouldBe(expected: 0);
            Run("restore", project, "--locked-mode", "-p:NuGetAudit=false").Exit.ShouldBe(expected: 0);
        }

        (int Exit, string Output) changed = Run("build", project, "--no-restore", "--disable-build-servers",
                "-p:UseSharedCompilation=false", "-nodeReuse:false", "-p:NuGetAudit=false");
        changed.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_SUBJECT: " + changed.Output);
    }

    /// <summary>Adds a compilable Core to engine reference without a dependency cycle.</summary>
    internal void AddDirectEngineEdge()
    {
        RetainCleanCoreConsumer();
        Append(project: ProjectEvaluator.Expected[0], xml:
                "<ItemGroup><ProjectReference Include=\"../game/KingdomZombies.csproj\"/></ItemGroup>");
        Compile(project: ProjectEvaluator.Expected[0], restore: true);
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

        using Process child = Process.Start(startInfo: DotnetLaunch.Configure(start: start))
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

    private void CopyInventory()
    {
        foreach (string relative in new[] { "src/godot-game/KingdomZombies.slnx", "tools/phase1/projects.json",
            "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj" })
        {
            string target = Path.Combine(path1: Root, path2: relative);
            Directory.CreateDirectory(path: Path.GetDirectoryName(path: target)
                    ?? throw new InvalidDataException(message: "Missing fixture parent"));
            File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: relative),
                    destFileName: target);
        }
    }

    private void RemoveReference(string project, string target)
    {
        string path = Path.Combine(path1: Root, path2: project);
        Write(relative: project, content: File.ReadAllText(path: path).Replace(oldValue:
                "    <ProjectReference Include=\"" + target + "\" />\n", newValue: "",
                comparisonType: StringComparison.Ordinal));
    }

    private void RetainCleanCoreConsumer()
    {
        string baseline = Path.Combine(path1: Root, path2: ".graph-baseline");
        Directory.CreateDirectory(path: baseline);
        File.Copy(sourceFileName: Path.Combine(path1: Root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                destFileName: Path.Combine(path1: baseline, path2: "KingdomZombies.Core.dll"));
        RemoveReference(project: ProjectEvaluator.Expected[2], target: "../core/KingdomZombies.Core.csproj");
        Append(project: ProjectEvaluator.Expected[2], xml:
                "<ItemGroup><Reference Include=\"KingdomZombies.Core\">"
                        + "<HintPath>../../../.graph-baseline/KingdomZombies.Core.dll</HintPath>"
                        + "<Private>false</Private></Reference></ItemGroup>");
    }

    private void RetainCleanPortsConsumer()
    {
        File.Copy(sourceFileName: Path.Combine(path1: Root, path2:
                "src/godot-game/multiplayer-abstractions/bin/Debug/net10.0/"
                        + "KingdomZombies.Multiplayer.Abstractions.dll"),
                destFileName: Path.Combine(path1: Root,
                        path2: ".graph-baseline/KingdomZombies.Multiplayer.Abstractions.dll"));
        RemoveReference(project: ProjectEvaluator.Expected[2], target:
                "../multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj");
        Append(project: ProjectEvaluator.Expected[2], xml:
                "<ItemGroup><Reference Include=\"KingdomZombies.Multiplayer.Abstractions\">"
                        + "<HintPath>../../../.graph-baseline/KingdomZombies.Multiplayer.Abstractions.dll</HintPath>"
                        + "<Private>false</Private></Reference></ItemGroup>");
    }

    private static string EdgeItem(string mutation, string item)
    {
        string game = "../game/KingdomZombies.csproj";
        string ports = "../multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj";
        return mutation switch
        {
            "wildcard" => "<ItemGroup><ProjectReference Include=\"../gam*/*.csproj\"/></ItemGroup>",
            "semicolon" => "<ItemGroup><ProjectReference Include=\"" + game + ";" + ports + "\"/></ItemGroup>",
            "reversed" => "<ItemGroup><ProjectReference Include=\"" + ports + ";" + game + "\"/></ItemGroup>",
            "duplicate" => item + item,
            "direct" => item,
            _ => throw new ArgumentException(message: "Unknown graph mutation", paramName: nameof(mutation))
        };
    }
}
