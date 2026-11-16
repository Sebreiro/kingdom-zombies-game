using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using KingdomZombies.Phase1.Tests.Verification;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves actual evaluated compiler policy and permitted consumer composition.</summary>
public sealed class ConfigurationTests
{
    /// <summary>Requires the actual game to compose exactly Core and service ports with an analyzer-only
    /// tooling edge.</summary>
    [Fact, Trait("Case", "pins.composition")]
    public void Evaluate_GameComposition_MatchesPermittedPolicy()
    {
        (int Exit, string Output) result = Dotnet("msbuild", Projects[2], "-nologo", "-getItem:ProjectReference");
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        using var json = JsonDocument.Parse(json: result.Output);
        JsonElement[] references = [..json.RootElement.GetProperty(propertyName: "Items").GetProperty(
                propertyName: "ProjectReference").EnumerateArray()];
        JsonElement[] runtime = [..references.Where(predicate: r => !r.TryGetProperty(
                propertyName: "ReferenceOutputAssembly", value: out JsonElement value) || !string.Equals(
                        a: value.GetString(), b: "false", comparisonType: StringComparison.OrdinalIgnoreCase))];
        runtime.Length.ShouldBe(expected: 2,
                customMessage: "Godot must compile with both permitted product references");
        runtime.Select(selector: r => Path.GetFileName(path: r.GetProperty(propertyName: "FullPath")
                .GetString())).Order(comparer: StringComparer.Ordinal).ShouldBe(expected: _productNames);
        JsonElement analyzer = references.Single(predicate: r => (r.GetProperty(propertyName: "FullPath")
                .GetString() ?? throw new System.InvalidOperationException(
                        message: "Required proof value is null")).EndsWith(
                                value: "Sebreiro.CodeRules.Analyzers.csproj", comparisonType: StringComparison
                                        .Ordinal));
        analyzer.GetProperty(propertyName: "OutputItemType").GetString().ShouldBe(expected: "Analyzer");
        analyzer.GetProperty(propertyName: "ReferenceOutputAssembly").GetString().ShouldBe(expected: "false");
        ProofProtocol.Record(id: "pins.composition", diagnostic: "COMPOSITION_EXACT", location: Projects[2]);
    }

    /// <summary>Emits an actual Godot consumer against the built game, Core and service-port assemblies.</summary>
    [Fact, Trait("Case", "pins.consumer")]
    public async Task Build_RealGodotConsumer_Compiles()
    {
        (int Exit, string Output) build = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        string[] paths = ["src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll",
            ("src/godot-game/multiplayer-abstractions/bin/Debug/net10.0/KingdomZ"
                    + "ombies.Multiplayer.Abstractions.dll"),
            "src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll",
            ".phase1/packages/godotsharp/4.7.2/lib/net8.0/GodotSharp.dll"];
        var frameworkUnification = new AnalyzerVerifier.Expected(id: "CS1701", severity: DiagnosticSeverity
                .Warning, file: "", line: 1, column: 1, endLine: 1, endColumn: 1);
        await AnalyzerVerifier.VerifyAsync(
                source: "public sealed class Consumer { public Godot.Node? Engine { get; init; } }", expected: [
                    frameworkUnification], additionalReferences: paths.Select(selector: p => MetadataReference
                            .CreateFromFile(path: Path.Combine(path1: ProofProtocol.Root, path2: p))));
        ProofProtocol.Record(id: "pins.consumer", diagnostic: "CONSUMER_EMITTED",
                location: "Subject.cs with actual Godot/game/Core/ports references");
    }

    /// <summary>Accepts the exact compiler diagnostic, then rejects wrong identity, severity, location and
    /// additional errors.</summary>
    [Fact, Trait("Case", "pins.verifier")]
    public async Task Verify_WrongCompilerDiagnostic_IsRejected()
    {
        var exact = new AnalyzerVerifier.Expected(id: "CS0246", severity: DiagnosticSeverity.Error,
                file: "Subject.cs", line: 1, column: 24, endLine: 1, endColumn: 31);
        const string Source = "public class Subject : Missing { }";
        await AnalyzerVerifier.VerifyAsync(source: Source, expected: [exact]);
        foreach (AnalyzerVerifier.Expected? changed in new[]
        {
            exact with
            {
                Id = "RS0030"
            },
            exact with
            {
                Severity = DiagnosticSeverity.Warning
            },
            exact with
            {
                Column = 25
            },
            exact with
            {
                File = "Other.cs"
            },
            exact with
            {
                EndColumn = 32
            }
        }

        )
        {
            (await Should.ThrowAsync<InvalidOperationException>(actual: () => AnalyzerVerifier.VerifyAsync(
                    source: Source, expected: [changed]))).Message.ShouldContain(expected: "DIAGNOSTIC_MISMATCH");
        }

        (await Should.ThrowAsync<InvalidOperationException>(actual: () => AnalyzerVerifier.VerifyAsync(
                source: Source + " public class Other : Absent { }", expected: [exact]))).Message.ShouldContain(
                        expected: "CS0246");
        await Should.ThrowAsync<InvalidOperationException>(actual: () => AnalyzerVerifier.VerifyAsync(
                source: "public class Clean { }", expected: [exact]));
        ProofProtocol.Record(id: "pins.verifier", diagnostic: "DIAGNOSTIC_EXACT",
                location: "Subject.cs(1,24)-(1,31): CS0246");
    }

    /// <summary>Requires production and tests to consume different checked banned-symbol inputs.</summary>
    [Fact, Trait("Case", "pins.banned-inputs")]
    public void Evaluate_ConsumerProjects_SelectScopedBannedInputs()
    {
        foreach (string? project in new[]
        {
            Projects[0],
            Projects[2],
            Projects[4]
        }

        )
        {
            (int Exit, string Output) result = Dotnet("msbuild", project, "-nologo", "-getItem:AdditionalFiles");
            result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
            using var json = JsonDocument.Parse(json: result.Output);
            string?[] files = [..json.RootElement.GetProperty(propertyName: "Items").GetProperty(
                    propertyName: "AdditionalFiles").EnumerateArray().Select(selector: r => Path.GetFileName(
                            path: r.GetProperty(propertyName: "FullPath").GetString()))];
            bool isTest = StringComparer.Ordinal.Equals(x: project, y: Projects[4]);
            files.Any(predicate: f => StringComparer.Ordinal.Equals(x: f, y: isTest ? "src/godot-game/build/BannedSymbols.Tests.txt"
                    : "src/godot-game/build/BannedSymbols.txt")).ShouldBeTrue();
            files.Any(predicate: f => StringComparer.Ordinal.Equals(x: f, y: isTest ? "src/godot-game/build/BannedSymbols.txt"
                    : "src/godot-game/build/BannedSymbols.Tests.txt")).ShouldBeFalse();
        }

        ProofProtocol.Record(id: "pins.banned-inputs", diagnostic: "BANNED_INPUTS_SCOPED",
                location: "evaluated AdditionalFiles");
    }

    /// <summary>Requires strict effective properties and exact global package versions for all five subjects.</summary>
    [Fact, Trait("Case", "pins.evaluated")]
    public void Evaluate_AllFiveProjects_InheritExactPolicy()
    {
        foreach (string project in Projects)
        {
            using JsonDocument evaluated = Evaluate(project: project);
            ValidateConfiguration(document: evaluated, analyzerProject: StringComparer.Ordinal.Equals(
                    x: project, y: Projects[3]));
        }

        ProofProtocol.Record(id: "pins.evaluated", diagnostic: "EVALUATED_POLICY_EXACT",
                location: "all five actual MSBuild consumers");
    }

    /// <summary>Rejects actual evaluated severity, import and runtime-analyzer metadata mutations.</summary>
    [Fact, Trait("Case", "pins.configuration-drift")]
    public void Evaluate_WeakenedPolicy_IsRejected()
    {
        foreach (string? setting in new[]
        {
            "TreatWarningsAsErrors=false",
            "MSBuildTreatWarningsAsErrors=false",
            "Nullable=disable",
            "ImportDirectoryBuildProps=false",
            "ImportDirectoryBuildTargets=false",
            "ManagePackageVersionsCentrally=false"
        }

        )
        {
            using JsonDocument changed = Evaluate(project: Projects[0], "-p:" + setting);
            Should.Throw<InvalidOperationException>(actual: () => ValidateConfiguration(document: changed,
                    analyzerProject: false)).Message.ShouldContain(expected: "POLICY_DRIFT");
        }

        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/configuration-" + Guid
                .NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        string targets = Path.Combine(path1: directory, path2: "Changed.targets");
        foreach (string? metadata in new[]
        {
            "ReferenceOutputAssembly",
            "OutputItemType"
        }

        )
        {
            File.WriteAllText(path: targets, contents: "<Project><Import Project=\"" + Path.Combine(
                    path1: ProofProtocol.Root, path2: "src/godot-game/Directory.Build.targets")
                    + "\"/><ItemGroup><ProjectReference Update=\"" + Path.Combine(path1: ProofProtocol.Root,
                            path2: Projects[3]) + "\"><" + metadata + ">true</" + metadata
                    + "></ProjectReference></ItemGroup></Project>");
            using JsonDocument changed = Evaluate(project: Projects[0], "-p:DirectoryBuildTargetsPath=" + targets);
            Should.Throw<InvalidOperationException>(actual: () => ValidateConfiguration(document: changed,
                    analyzerProject: false)).Message.ShouldContain(expected: "analyzer metadata");
        }

        ProofProtocol.Record(id: "pins.configuration-drift", diagnostic: "POLICY_DRIFT_REJECTED",
                location: "actual MSBuild overrides/import/metadata controls");
    }

    /// <summary>Rejects uncontrolled wall-clock/randomness, service locators and Console in actual production
    /// builds.</summary>
    [Fact, Trait("Case", "pins.banned-production")]
    public void Build_ProductionBannedApis_FailsAtExactLocations()
    {
        string project = Fixture(source: ("namespace CompilerFixture; /// <summary>Negative produc"
                + "tion control.</summary>\npublic static class Subject { /"
                + "// <summary>Reads forbidden ambient inputs.</summary>\np"
                + "ublic static object Read() { System.Console.WriteLine(S"
                + "ystem.DateTime.UtcNow); _ = System.Random.Shared.Next()"
                + "; return typeof(System.IServiceProvider); } }"));
        string output = BuildFixture(project: project, succeeds: false, diagnostic: "RS0030");
        foreach (string? symbol in new[]
        {
            "Console",
            "UtcNow",
            "Shared",
            "IServiceProvider"
        }

        )
        {
            output.ShouldContain(expected: symbol);
        }

        ProofProtocol.Record(id: "pins.banned-production", diagnostic: "RS0030",
                location: "Subject.cs: actual production compiler");
    }

    /// <summary>Accepts Shouldly and an awaitable method without an Async suffix; rejects inherited
    /// Xunit.Assert usage.</summary>
    [Fact, Trait("Case", "pins.banned-tests")]
    public void Build_TestAssertions_AcceptsShouldlyAndRejectsAssert()
    {
        string project = Fixture(source: ("using Shouldly; namespace CompilerFixture; /// <summary"
                + ">Permitted test APIs.</summary>\npublic static class Sub"
                + "ject { /// <summary>No Async suffix is required.</summa"
                + "ry>\npublic static System.Threading.Tasks.Task Read() { "
                + "1.ShouldBe(expected: 1); return System.Threading.Tasks." + "Task.CompletedTask; } }"), tests: true);
        BuildFixture(project: project, succeeds: true);
        File.WriteAllText(path: Path.Combine((Path.GetDirectoryName(path: project)
                ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                path2: "Subject.cs"), contents: ("namespace CompilerFixture; /// <summary>Forbidden asser"
                        + "tion library.</summary>\npublic static class Subject { /"
                        + "// <summary>Rejects the inherited assertion ban.</summa"
                        + "ry>\npublic static void Read() { Xunit.Assert.True(condi" + "tion: true); } }"));
        BuildFixture(project: project, succeeds: false, diagnostic: "RS0030").ShouldContain(expected: "Assert");
        ProofProtocol.Record(id: "pins.banned-tests", diagnostic: "TEST_POLICY_EXACT",
                location: "Subject.cs: Shouldly/permission pass; Xunit.Assert RS0030");
    }

    /// <summary>Rejects severity weakening when it removes the intended subject diagnostic.</summary>
    [Fact, Trait("Case", "pins.banned-severity")]
    public void Evaluate_WeakenedBannedSeverity_IsRejected()
    {
        string project = Fixture(source: ("namespace CompilerFixture; /// <summary>Diagnostic seve"
                + "rity control.</summary>\npublic static class Subject { /"
                + "// <summary>Forbidden output.</summary>\npublic static v"
                + "oid Read() => System.Console.WriteLine(value: \"forbidde" + "n\"); }"));
        BuildFixture(project: project, succeeds: false, diagnostic: "RS0030");
        File.WriteAllText(path: Path.Combine((Path.GetDirectoryName(path: project)
                ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                path2: ".editorconfig"), contents: "root = true\n[*.cs]\ndotnet_diagnostic.RS0030.severity = none\n");
        (int Exit, string Output) changed = Dotnet("build", project, "--no-restore", "-p:NuGetAudit=false");
        changed.Exit.ShouldBe(expected: 0, customMessage: changed.Output);
        InvalidOperationException rejected = Should.Throw<InvalidOperationException>(
                actual: () => RequireDiagnostic(result: changed, diagnostic: "RS0030"));
        rejected.Message.ShouldContain(expected: "POLICY_BYPASS");
        ProofProtocol.Record(id: "pins.banned-severity", diagnostic: "POLICY_BYPASS_REJECTED",
                location: "real compiler loses RS0030 only under deliberate severity weakening");
    }

    /// <summary>Preserves permission for awaitable methods without an Async suffix and rejects a
    /// configuration that removes it.</summary>
    [Fact, Trait("Case", "pins.async-permission")]
    public void Evaluate_AsyncSuffixPermission_RejectsConfigurationDrift()
    {
        string project = Fixture(source: ("namespace CompilerFixture; /// <summary>Awaitable namin"
                + "g permission.</summary>\npublic static class Subject { /"
                + "// <summary>Permitted suffix omission.</summary>\npublic"
                + " static System.Threading.Tasks.Task Read() { return System."
                + "Threading.Tasks.Task.CompletedTask; } }"));
        BuildFixture(project: project, succeeds: true);
        File.WriteAllText(path: Path.Combine((Path.GetDirectoryName(path: project)
                ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                path2: ".editorconfig"), contents: "root = true\n[*.cs]\ndotnet_diagnostic.MA0137.severity = error\n");
        BuildFixture(project: project, succeeds: false, diagnostic: "MA0137");
        ProofProtocol.Record(id: "pins.async-permission", diagnostic: "ASYNC_PERMISSION_EXACT",
                location: "permitted Read passes; wrong naming requirement emits MA0137");
    }

    /// <summary>Requires five canonical locks and rejects a changed exact dependency under clean locked
    /// restore.</summary>
    [Fact, Trait("Case", "ac-01.complete")]
    public void Restore_FiveCanonicalLocks_RejectsDependencyDrift()
    {
        var hashes = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach (string project in Projects)
        {
            string path = Path.Combine(path1: ProofProtocol.Root, (Path.GetDirectoryName(path: project)
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                    path3: "packages.lock.json");
            File.Exists(path: path).ShouldBeTrue(customMessage: path);
            hashes.Add(key: path, value: Convert.ToHexString(inArray: SHA256.HashData(source: File.ReadAllBytes(
                    path: path))));
        }

        string cleanRoot = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/clean-restore-" + Guid
                .NewGuid().ToString(format: "N"));
        var copyFiles = new List<string>
        {
            "src/godot-game/KingdomZombies.slnx",
            "src/godot-game/Directory.Build.props",
            "src/godot-game/Directory.Build.targets",
            "src/godot-game/Directory.Packages.props",
            "src/godot-game/NuGet.config",
            "src/godot-game/global.json"
        };
        foreach (string project in Projects)
        {
            copyFiles.Add(item: project);
            copyFiles.Add(item: Path.Combine((Path.GetDirectoryName(path: project)
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                    path2: "packages.lock.json"));
        }

        foreach (string relative in copyFiles)
        {
            string destination = Path.Combine(path1: cleanRoot, path2: relative);
            Directory.CreateDirectory((Path.GetDirectoryName(path: destination)
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null")));
            File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root, path2: relative),
                    destFileName: destination);
        }

        string config = Path.Combine(path1: cleanRoot, path2: "src/godot-game/NuGet.config");
        File.WriteAllText(path: config, contents: File.ReadAllText(path: config).Replace(
                oldValue: "value=\"../../.phase1/feed\"", newValue: "value=\"" + Path.Combine(path1: ProofProtocol
                        .Root, path2: ".phase1/feed") + "\"", comparisonType: StringComparison.Ordinal));
        Directory.EnumerateFiles(path: cleanRoot, searchPattern: "project.assets.json",
                searchOption: SearchOption.AllDirectories).ShouldBeEmpty();
        (int Exit, string Output) restore = Dotnet("restore", Path.Combine(path1: cleanRoot,
                path2: "src/godot-game/KingdomZombies.slnx"), "--locked-mode", "-p:NuGetAudit=false");
        restore.Exit.ShouldBe(expected: 0, customMessage: restore.Output);
        Directory.EnumerateFiles(path: cleanRoot, searchPattern: "project.assets.json",
                searchOption: SearchOption.AllDirectories).Count().ShouldBe(expected: 5);
        foreach ((string? path, string? hash) in hashes)
        {
            Convert.ToHexString(inArray: SHA256.HashData(source: File.ReadAllBytes(path: path))).ShouldBe(
                    expected: hash);
        }

        VerifyChangedDependencyLock();
        ProofProtocol.Record(id: "ac-01.complete", diagnostic: "LOCKED_GRAPH_EXACT",
                location: "five canonical locks + NU1004 changed dependency");
    }

    /// <summary>Requires the pinned generator package and authentic generated method before accepting its
    /// report adapter.</summary>
    [Fact, Trait("Case", "pins.generated-report")]
    public void Check_GeneratedReportAdapter_HasPinnedOrigin()
    {
        (int Exit, string Output) build = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false",
                "-t:Rebuild");
        build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        string package = Path.Combine(path1: ProofProtocol.Root,
                path2: ".phase1/feed/Godot.SourceGenerators.4.7.2.nupkg");
        Convert.ToHexStringLower(inArray: SHA256.HashData(source: File.ReadAllBytes(path: package))).ShouldBe(
                expected: "ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e");
        (int Exit, string Output) resolve = Dotnet("msbuild", Projects[2], "-nologo",
                "-target:ResolveReferences", "-getItem:Analyzer");
        resolve.Exit.ShouldBe(expected: 0, customMessage: resolve.Output);
        using var analyzers = JsonDocument.Parse(json: resolve.Output);
        const string GeneratorSuffix = "godot.sourcegenerators/4.7.2/analyzers/dotnet/cs/Godot.SourceGenerators.dll";
        analyzers.RootElement.GetProperty(propertyName: "Items").GetProperty(propertyName: "Analyzer")
                .EnumerateArray().Any(predicate: a => (a.GetProperty(propertyName: "FullPath").GetString()
                        ?? throw new System.InvalidOperationException(message: "Required proof value is null"))
                        .Replace(oldChar: '\\', newChar: '/').EndsWith(
                                value: GeneratorSuffix, comparisonType: StringComparison.Ordinal)).ShouldBeTrue();
        string generated = Path.Combine(path1: ProofProtocol.Root,
                path2: ("src/godot-game/game/.godot/mono/temp/obj/Debug/generated/God"
                        + "ot.SourceGenerators/Godot.SourceGenerators.GodotPlugins"
                        + "InitializerGenerator/GodotPlugins.Game.generated.cs"));
        string source = File.ReadAllText(path: generated);
        source.ShouldContain(expected: ("private static godot_bool InitializeFromGameProject(Int"
                + "Ptr godotDllHandle, IntPtr outManagedCallbacks,"));
        source.ShouldContain(expected: "global::System.Console.Error.WriteLine(e);");
        using var registry = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(path1: ProofProtocol
                .Root, path2: "docs/code/engine-adaptations.json")));
        JsonElement entry = registry.RootElement.GetProperty(propertyName: "local").EnumerateArray().Single();
        Convert.ToHexStringLower(inArray: SHA256.HashData(source: File.ReadAllBytes(path: generated))).ShouldBe(
                expected: entry.GetProperty(propertyName: "generatedSourceSha256").GetString());
        entry.GetProperty(propertyName: "diagnostic").GetString().ShouldBe(expected: "RS0030");
        AdapterTargets.Read(source: File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/GeneratedReportAdapter.cs"))).ShouldContain(expected: entry.GetProperty(
                        propertyName: "target").GetString() ?? throw new InvalidOperationException(
                                message: "Required target is missing"), comparer: StringComparer.Ordinal);
        ProofProtocol.Record(id: "pins.generated-report", diagnostic: "GENERATOR_ORIGIN_EXACT", location: generated);
    }

    /// <summary>Rejects handwritten adjacent Console use and a same-signature generated-method impostor.</summary>
    [Fact, Trait("Case", "pins.generated-report-boundary")]
    public void Analyze_GeneratedReportPermission_RejectsOutsideMethod()
    {
        string adjacent = BuildGameWithSource(
                source: ("namespace GodotPlugins.Game; internal static partial cl"
                        + "ass Main { private static void Adjacent() => System.Con"
                        + "sole.WriteLine(value: \"forbidden\"); }"));
        adjacent.ShouldContain(expected: "Main.cs(");
        adjacent.ShouldContain(expected: "error RS0030:");
        string impostor = BuildGameWithSource(
                source: ("namespace GodotPlugins.Game; internal static partial cl"
                        + "ass Main { private static bool InitializeFromGameProjec"
                        + "t(System.IntPtr a, System.IntPtr b, System.IntPtr c, in"
                        + "t d) { System.Console.WriteLine(\"impostor\"); return fal" + "se; } }"));
        impostor.ShouldContain(expected: "error CS0111:");
        impostor.ShouldContain(expected: "InitializeFromGameProject");
        (int Exit, string Output) clean = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(expected: 0, customMessage: clean.Output);
        ProofProtocol.Record(id: "pins.generated-report-boundary", diagnostic: "GENERATED_PERMISSION_BOUNDED",
                location: "Subject.cs RS0030 adjacent / CS0111 impostor");
    }

    internal static readonly string[] Projects = ["src/godot-game/core/KingdomZombies.Core.csproj",
        "src/godot-game/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj",
        "src/godot-game/game/KingdomZombies.csproj", "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj",
        "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj"];
    internal static (int Exit, string Output) Dotnet(params string[] arguments)
    {
        var start = new ProcessStartInfo(fileName: "dotnet")
        {
            WorkingDirectory = ProofProtocol.Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string argument in arguments)
        {
            start.ArgumentList.Add(item: argument);
        }

        using Process child = Process.Start(startInfo: DotnetLaunch.Configure(start: start)) ?? throw new InvalidOperationException(
                message: "dotnet did not start");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 30000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "Compiler proof exceeded 30 seconds");
        }

        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    internal static string BuildGameWithSource(string source)
    {
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/game-boundary-" + Guid
                .NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        string subject = Path.Combine(path1: directory, path2: "Main.cs");
        File.WriteAllText(path: subject, contents: source);
        string targets = Path.Combine(path1: directory, path2: "Boundary.targets");
        File.WriteAllText(path: targets, contents: "<Project><Import Project=\"" + Path.Combine(
                path1: ProofProtocol.Root, path2: "src/godot-game/Directory.Build.targets")
                + ("\"/><ItemGroup Condition=\"'$(MSBuildProjectName)' == 'Ki"
                        + "ngdomZombies'\"><Compile Include=\"") + subject + "\"/></ItemGroup></Project>");
        (int Exit, string Output) result = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false",
                "-p:DirectoryBuildTargetsPath=" + targets);
        result.Exit.ShouldNotBe(expected: 0,
                customMessage: "Forbidden game input must not acquire generated permission");
        return result.Output;
    }

    private static readonly string[] _productNames = ["KingdomZombies.Core.csproj",
        "KingdomZombies.Multiplayer.Abstractions.csproj"];
    private static JsonDocument Evaluate(string project, params string[] overrides)
    {
        (int Exit, string Output) result = Dotnet(arguments: ["msbuild", project, "-nologo",
            ("-getProperty:Nullable,ImplicitUsings,LangVersion,TreatW"
                    + "arningsAsErrors,MSBuildTreatWarningsAsErrors,EnforceCod"
                    + "eStyleInBuild,GenerateDocumentationFile,AnalysisLevel,D"
                    + "isableTransitiveProjectReferences,ManagePackageVersions"
                    + "Centrally,CentralPackageTransitivePinningEnabled,Restor"
                    + "ePackagesWithLockFile,DirectoryBuildPropsPath,Directory"
                    + "BuildTargetsPath,DirectoryPackagesPropsPath"),
            "-getItem:ProjectReference,GlobalPackageReference,AdditionalFiles", ..overrides]);
        if (result.Exit != 0)
        {
            throw new InvalidOperationException(message: "EVALUATION_FAILED: " + result.Output);
        }

        return JsonDocument.Parse(json: result.Output);
    }

    private static void ValidateConfiguration(JsonDocument document, bool analyzerProject)
    {
        ValidateEffectiveProperties(document: document);
        JsonElement[] packages = [..document.RootElement.GetProperty(propertyName: "Items").GetProperty(
                propertyName: "GlobalPackageReference").EnumerateArray()];
        var expected = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            ["Meziantou.Analyzer"] = "[3.0.312]",
            ["Roslynator.Analyzers"] = "[5.0.1]",
            ["Microsoft.CodeAnalysis.BannedApiAnalyzers"] = "[5.6.0]"
        };
        if (packages.Length != expected.Count)
        {
            throw new InvalidOperationException(message: "POLICY_DRIFT: global analyzer count");
        }

        foreach (JsonElement package in packages)
        {
            if (!expected.TryGetValue((package.GetProperty(propertyName: "Identity").GetString()
                    ?? throw new System.InvalidOperationException(message: "Required proof value is null")),
                    value: out string? version) || !StringComparer.Ordinal.Equals(x: package.GetProperty(
                            propertyName: "Version").GetString(), y: version))
            {
                throw new InvalidOperationException(message: "POLICY_DRIFT: analyzer version");
            }
        }

        if (analyzerProject)
        {
            return;
        }

        JsonElement[] custom = [..document.RootElement.GetProperty(propertyName: "Items").GetProperty(
                propertyName: "ProjectReference").EnumerateArray().Where(predicate: r => (r.GetProperty(
                        propertyName: "FullPath").GetString() ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null")).EndsWith(
                                        value: "Sebreiro.CodeRules.Analyzers.csproj",
                                        comparisonType: StringComparison.Ordinal))];
        if (custom.Length != 1 || !StringComparer.Ordinal.Equals(x: custom[0].GetProperty(
                propertyName: "OutputItemType").GetString(), y: "Analyzer") || !string.Equals(a: custom[0]
                        .GetProperty(propertyName: "ReferenceOutputAssembly").GetString(), b: "false",
                        comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(message: "POLICY_DRIFT: analyzer metadata");
        }
    }

    private static string Fixture(string source, bool tests = false)
    {
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/compiler-" + Guid
                .NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: directory);
        File.WriteAllText(path: Path.Combine(path1: directory, path2: "CompilerFixture.csproj"),
                contents: ("<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><Target"
                        + "Framework>net10.0</TargetFramework><IsTestProject>") + (tests ? "true" : "false")
                        + "</IsTestProject>" + (tests
                                ? ("<OutputType>Exe</OutputType><XunitAutoGeneratedEntryPoi"
                                        + "nt>false</XunitAutoGeneratedEntryPoint><GenerateSelfReg"
                                        + "isteredExtensions>false</GenerateSelfRegisteredExtensio" + "ns>")
                                : "") + "</PropertyGroup>" + (tests
                                        ? ("<ItemGroup><PackageReference Include=\"xunit.v3\"/><Packa"
                                                + "geReference Include=\"Shouldly\"/></ItemGroup>") : "")
                        + "</Project>");
        if (tests)
        {
            File.WriteAllText(path: Path.Combine(path1: directory, path2: "EntryPoint.cs"),
                    contents: ("namespace CompilerFixture; internal static class EntryP"
                            + "oint { private static void Main() { } }"));
        }

        File.WriteAllText(path: Path.Combine(path1: directory, path2: "Subject.cs"), contents: Microsoft
                .CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(text: source).GetRoot().NormalizeWhitespace()
                .ToFullString().Replace(oldValue: " => ", newValue: " =>\n        "));
        return Path.Combine(path1: directory, path2: "CompilerFixture.csproj");
    }

    private static string BuildFixture(string project, bool succeeds, string? diagnostic = null)
    {
        (int Exit, string Output) restore = Dotnet("restore", project, "-p:NuGetAudit=false",
                "--ignore-failed-sources");
        restore.Exit.ShouldBe(expected: 0, customMessage: "Infrastructure restore must succeed: " + restore.Output);
        (int Exit, string Output) build = Dotnet("build", project, "--no-restore", "-p:NuGetAudit=false");
        if (succeeds)
        {
            build.Exit.ShouldBe(expected: 0, customMessage: build.Output);
        }
        else
        {
            RequireDiagnostic(result: build, (diagnostic ?? throw new System.InvalidOperationException(
                    message: "Required proof value is null")));
            build.Output.ShouldContain(expected: "Subject.cs(");
        }

        return build.Output;
    }

    private static void RequireDiagnostic((int Exit, string Output) result, string diagnostic)
    {
        if (result.Exit == 0 || !result.Output.Contains(value: "error " + diagnostic + ":",
                comparisonType: StringComparison.Ordinal))
        {
            throw new InvalidOperationException(message: "POLICY_BYPASS: intended " + diagnostic + " absent");
        }
    }

    private static void VerifyChangedDependencyLock()
    {
        string fixture = Fixture(source: ("namespace CompilerFixture; /// <summary>Restore-only co"
                + "ntrol.</summary>\npublic static class Subject { }"));
        File.Copy(sourceFileName: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/core/packages.lock.json"), destFileName: Path.Combine((Path.GetDirectoryName(
                        path: fixture) ?? throw new System.InvalidOperationException(
                                message: "Required proof value is null")), path2: "packages.lock.json"));
        File.WriteAllText(path: fixture, contents: File.ReadAllText(path: fixture).Replace(
                oldValue: "</Project>",
                newValue: "<ItemGroup><PackageReference Include=\"Shouldly\"/></ItemGroup></Project>",
                comparisonType: StringComparison.Ordinal));
        (int Exit, string Output) changed = Dotnet("restore", fixture, "--locked-mode", "-p:NuGetAudit=false");
        changed.Exit.ShouldNotBe(expected: 0);
        changed.Output.ShouldContain(expected: "NU1004");
    }

    private static void ValidateEffectiveProperties(JsonDocument document)
    {
        JsonElement properties = document.RootElement.GetProperty(propertyName: "Properties");
        var required = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            ["Nullable"] = "enable",
            ["ImplicitUsings"] = "disable",
            ["LangVersion"] = "14.0",
            ["TreatWarningsAsErrors"] = "true",
            ["MSBuildTreatWarningsAsErrors"] = "true",
            ["EnforceCodeStyleInBuild"] = "true",
            ["GenerateDocumentationFile"] = "true",
            ["AnalysisLevel"] = "10.0-recommended",
            ["DisableTransitiveProjectReferences"] = "true",
            ["ManagePackageVersionsCentrally"] = "true",
            ["CentralPackageTransitivePinningEnabled"] = "true",
            ["RestorePackagesWithLockFile"] = "true"
        };
        foreach ((string? key, string? value) in required)
        {
            if (!StringComparer.Ordinal.Equals(x: properties.GetProperty(propertyName: key).GetString(), y: value))
            {
                throw new InvalidOperationException(message: "POLICY_DRIFT: " + key);
            }
        }

        foreach (string? key in new[]
        {
            "DirectoryBuildPropsPath",
            "DirectoryBuildTargetsPath",
            "DirectoryPackagesPropsPath"
        }

        )
        {
            if (string.IsNullOrEmpty(value: properties.GetProperty(propertyName: key).GetString()))
            {
                throw new InvalidOperationException(message: "POLICY_DRIFT: " + key);
            }
        }
    }
}
