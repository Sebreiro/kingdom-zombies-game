using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Shouldly;
using Xunit;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using KingdomZombies.Phase1.Tests.Verification;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves actual evaluated compiler policy and permitted consumer composition.</summary>
public sealed class ConfigurationTests
{
    private static readonly string[] ProductNames = ["KingdomZombies.Core.csproj", "KingdomZombies.Multiplayer.Abstractions.csproj"];
    internal static readonly string[] Projects = ["src/core/KingdomZombies.Core.csproj", "src/multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj", "src/godot-game/KingdomZombies.csproj", "tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj", "tests/phase1/KingdomZombies.Phase1.Tests.csproj"];

    internal static (int Exit, string Output) Dotnet(params string[] arguments)
    {
        var start = new ProcessStartInfo("dotnet") { WorkingDirectory = ProofProtocol.Root, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var child = Process.Start(start) ?? throw new InvalidOperationException("dotnet did not start");
        var output = child.StandardOutput.ReadToEndAsync();
        var error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(30000)) { child.Kill(entireProcessTree: true); child.WaitForExit(); throw new TimeoutException("Compiler proof exceeded 30 seconds"); }
        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }

    /// <summary>Requires the actual game to compose exactly Core and service ports with an analyzer-only tooling edge.</summary>
    [Fact, Trait("Case", "pins.composition")]
    public void GameHasPermittedEvaluatedComposition()
    {
        var result = Dotnet("msbuild", Projects[2], "-nologo", "-getItem:ProjectReference");
        result.Exit.ShouldBe(0, result.Output);
        using var json = JsonDocument.Parse(result.Output);
        var references = json.RootElement.GetProperty("Items").GetProperty("ProjectReference").EnumerateArray().ToArray();
        var runtime = references.Where(r => !r.TryGetProperty("ReferenceOutputAssembly", out var value) || !string.Equals(value.GetString(), "false", StringComparison.OrdinalIgnoreCase)).ToArray();
        runtime.Length.ShouldBe(2, "Godot must compile with both permitted product references");
        runtime.Select(r => Path.GetFileName(r.GetProperty("FullPath").GetString())).Order(StringComparer.Ordinal).ShouldBe(ProductNames);
        var analyzer = references.Single(r => r.GetProperty("FullPath").GetString()!.EndsWith("Sebreiro.CodeRules.Analyzers.csproj", StringComparison.Ordinal));
        analyzer.GetProperty("OutputItemType").GetString().ShouldBe("Analyzer");
        analyzer.GetProperty("ReferenceOutputAssembly").GetString().ShouldBe("false");
        ProofProtocol.Record("pins.composition", "COMPOSITION_EXACT", Projects[2]);
    }

    /// <summary>Emits an actual Godot consumer against the built game, Core and service-port assemblies.</summary>
    [Fact, Trait("Case", "pins.consumer")]
    public async Task RealGodotConsumerCompiles()
    {
        var build = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false");
        build.Exit.ShouldBe(0, build.Output);
        var paths = new[] { "src/core/bin/Debug/net10.0/KingdomZombies.Core.dll", "src/multiplayer-abstractions/bin/Debug/net10.0/KingdomZombies.Multiplayer.Abstractions.dll", "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll", ".phase1/packages/godotsharp/4.7.2/lib/net8.0/GodotSharp.dll" };
        // Godot's actual package targets net8.0; Roslyn reports the precise framework-unification
        // warning that SDK builds already handle. Assert it explicitly instead of hiding diagnostics.
        var frameworkUnification = new AnalyzerVerifier.Expected("CS1701", DiagnosticSeverity.Warning, "", 1, 1, 1, 1);
        await AnalyzerVerifier.VerifyAsync("public sealed class Consumer { public Godot.Node? Engine { get; init; } }", [frameworkUnification], paths.Select(p => MetadataReference.CreateFromFile(Path.Combine(ProofProtocol.Root, p))));
        ProofProtocol.Record("pins.consumer", "CONSUMER_EMITTED", "Subject.cs with actual Godot/game/Core/ports references");
    }

    /// <summary>Accepts the exact compiler diagnostic, then rejects wrong identity, severity, location and additional errors.</summary>
    [Fact, Trait("Case", "pins.verifier")]
    public async Task VerifierRequiresExactCompilerDiagnostic()
    {
        var exact = new AnalyzerVerifier.Expected("CS0246", DiagnosticSeverity.Error, "Subject.cs", 1, 24, 1, 31);
        const string source = "public class Subject : Missing { }";
        await AnalyzerVerifier.VerifyAsync(source, [exact]);
        foreach (var changed in new[] { exact with { Id = "RS0030" }, exact with { Severity = DiagnosticSeverity.Warning }, exact with { Column = 25 }, exact with { File = "Other.cs" }, exact with { EndColumn = 32 } })
            (await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync(source, [changed]))).Message.ShouldContain("DIAGNOSTIC_MISMATCH");
        (await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync(source + " public class Other : Absent { }", [exact]))).Message.ShouldContain("CS0246");
        await Should.ThrowAsync<InvalidOperationException>(() => AnalyzerVerifier.VerifyAsync("public class Clean { }", [exact]));
        ProofProtocol.Record("pins.verifier", "DIAGNOSTIC_EXACT", "Subject.cs(1,24)-(1,31): CS0246");
    }

    /// <summary>Requires production and tests to consume different checked banned-symbol inputs.</summary>
    [Fact, Trait("Case", "pins.banned-inputs")]
    public void ConsumersSelectScopedBannedInputs()
    {
        foreach (var project in new[] { Projects[0], Projects[2], Projects[4] })
        {
            var result = Dotnet("msbuild", project, "-nologo", "-getItem:AdditionalFiles");
            result.Exit.ShouldBe(0, result.Output);
            using var json = JsonDocument.Parse(result.Output);
            var files = json.RootElement.GetProperty("Items").GetProperty("AdditionalFiles").EnumerateArray().Select(r => Path.GetFileName(r.GetProperty("FullPath").GetString())).ToArray();
            var isTest = string.Equals(project, Projects[4], StringComparison.Ordinal);
            files.Any(f => string.Equals(f, isTest ? "BannedSymbols.Tests.txt" : "BannedSymbols.txt", StringComparison.Ordinal)).ShouldBeTrue();
            files.Any(f => string.Equals(f, isTest ? "BannedSymbols.txt" : "BannedSymbols.Tests.txt", StringComparison.Ordinal)).ShouldBeFalse();
        }
        ProofProtocol.Record("pins.banned-inputs", "BANNED_INPUTS_SCOPED", "evaluated AdditionalFiles");
    }

    private static JsonDocument Evaluate(string project, params string[] overrides)
    {
        var result = Dotnet(["msbuild", project, "-nologo", "-getProperty:Nullable,ImplicitUsings,LangVersion,TreatWarningsAsErrors,MSBuildTreatWarningsAsErrors,EnforceCodeStyleInBuild,GenerateDocumentationFile,AnalysisLevel,DisableTransitiveProjectReferences,ManagePackageVersionsCentrally,CentralPackageTransitivePinningEnabled,RestorePackagesWithLockFile,DirectoryBuildPropsPath,DirectoryBuildTargetsPath,DirectoryPackagesPropsPath", "-getItem:ProjectReference,GlobalPackageReference,AdditionalFiles", .. overrides]);
        if (result.Exit != 0) throw new InvalidOperationException("EVALUATION_FAILED: " + result.Output);
        return JsonDocument.Parse(result.Output);
    }

    private static void ValidateConfiguration(JsonDocument document, bool analyzerProject)
    {
        var properties = document.RootElement.GetProperty("Properties");
        var required = new Dictionary<string, string>(StringComparer.Ordinal) { ["Nullable"] = "enable", ["ImplicitUsings"] = "disable", ["LangVersion"] = "14.0", ["TreatWarningsAsErrors"] = "true", ["MSBuildTreatWarningsAsErrors"] = "true", ["EnforceCodeStyleInBuild"] = "true", ["GenerateDocumentationFile"] = "true", ["AnalysisLevel"] = "10.0-recommended", ["DisableTransitiveProjectReferences"] = "true", ["ManagePackageVersionsCentrally"] = "true", ["CentralPackageTransitivePinningEnabled"] = "true", ["RestorePackagesWithLockFile"] = "true" };
        foreach (var (key, value) in required)
            if (!string.Equals(properties.GetProperty(key).GetString(), value, StringComparison.Ordinal)) throw new InvalidOperationException("POLICY_DRIFT: " + key);
        foreach (var key in new[] { "DirectoryBuildPropsPath", "DirectoryBuildTargetsPath", "DirectoryPackagesPropsPath" })
            if (string.IsNullOrEmpty(properties.GetProperty(key).GetString())) throw new InvalidOperationException("POLICY_DRIFT: " + key);
        var packages = document.RootElement.GetProperty("Items").GetProperty("GlobalPackageReference").EnumerateArray().ToArray();
        var expected = new Dictionary<string, string>(StringComparer.Ordinal) { ["Meziantou.Analyzer"] = "[3.0.312]", ["Roslynator.Analyzers"] = "[5.0.1]", ["Microsoft.CodeAnalysis.BannedApiAnalyzers"] = "[5.6.0]" };
        if (packages.Length != expected.Count) throw new InvalidOperationException("POLICY_DRIFT: global analyzer count");
        foreach (var package in packages)
            if (!expected.TryGetValue(package.GetProperty("Identity").GetString()!, out var version) || !string.Equals(package.GetProperty("Version").GetString(), version, StringComparison.Ordinal)) throw new InvalidOperationException("POLICY_DRIFT: analyzer version");
        if (analyzerProject) return;
        var custom = document.RootElement.GetProperty("Items").GetProperty("ProjectReference").EnumerateArray().Where(r => r.GetProperty("FullPath").GetString()!.EndsWith("Sebreiro.CodeRules.Analyzers.csproj", StringComparison.Ordinal)).ToArray();
        if (custom.Length != 1 || !string.Equals(custom[0].GetProperty("OutputItemType").GetString(), "Analyzer", StringComparison.Ordinal) || !string.Equals(custom[0].GetProperty("ReferenceOutputAssembly").GetString(), "false", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("POLICY_DRIFT: analyzer metadata");
    }

    /// <summary>Requires strict effective properties and exact global package versions for all five subjects.</summary>
    [Fact, Trait("Case", "pins.evaluated")]
    public void AllFiveProjectsInheritExactPolicy()
    {
        foreach (var project in Projects)
        {
            using var evaluated = Evaluate(project);
            ValidateConfiguration(evaluated, string.Equals(project, Projects[3], StringComparison.Ordinal));
        }
        ProofProtocol.Record("pins.evaluated", "EVALUATED_POLICY_EXACT", "all five actual MSBuild consumers");
    }

    /// <summary>Rejects actual evaluated severity, import and runtime-analyzer metadata mutations.</summary>
    [Fact, Trait("Case", "pins.configuration-drift")]
    public void EvaluatedPolicyRejectsWeakening()
    {
        foreach (var setting in new[] { "TreatWarningsAsErrors=false", "MSBuildTreatWarningsAsErrors=false", "Nullable=disable", "ImportDirectoryBuildProps=false", "ImportDirectoryBuildTargets=false", "ManagePackageVersionsCentrally=false" })
        {
            using var changed = Evaluate(Projects[0], "-p:" + setting);
            Should.Throw<InvalidOperationException>(() => ValidateConfiguration(changed, analyzerProject: false)).Message.ShouldContain("POLICY_DRIFT");
        }
        var directory = Path.Combine(ProofProtocol.Root, ".phase1/temp/configuration-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var targets = Path.Combine(directory, "Changed.targets");
        foreach (var metadata in new[] { "ReferenceOutputAssembly", "OutputItemType" })
        {
            File.WriteAllText(targets, "<Project><Import Project=\"" + Path.Combine(ProofProtocol.Root, "Directory.Build.targets") + "\"/><ItemGroup><ProjectReference Update=\"" + Path.Combine(ProofProtocol.Root, Projects[3]) + "\"><" + metadata + ">true</" + metadata + "></ProjectReference></ItemGroup></Project>");
            using var changed = Evaluate(Projects[0], "-p:DirectoryBuildTargetsPath=" + targets);
            Should.Throw<InvalidOperationException>(() => ValidateConfiguration(changed, analyzerProject: false)).Message.ShouldContain("analyzer metadata");
        }
        ProofProtocol.Record("pins.configuration-drift", "POLICY_DRIFT_REJECTED", "actual MSBuild overrides/import/metadata controls");
    }

    private static string Fixture(string source, bool tests = false)
    {
        var directory = Path.Combine(ProofProtocol.Root, ".phase1/temp/compiler-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "CompilerFixture.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework><IsTestProject>" + (tests ? "true" : "false") + "</IsTestProject>" + (tests ? "<OutputType>Exe</OutputType><UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>" : "") + "</PropertyGroup>" + (tests ? "<ItemGroup><PackageReference Include=\"xunit.v3\"/><PackageReference Include=\"Shouldly\"/></ItemGroup>" : "") + "</Project>");
        File.WriteAllText(Path.Combine(directory, "Subject.cs"), source);
        return Path.Combine(directory, "CompilerFixture.csproj");
    }

    private static string BuildFixture(string project, bool succeeds, string? diagnostic = null)
    {
        var restore = Dotnet("restore", project, "-p:NuGetAudit=false", "--ignore-failed-sources");
        restore.Exit.ShouldBe(0, "Infrastructure restore must succeed: " + restore.Output);
        var build = Dotnet("build", project, "--no-restore", "-p:NuGetAudit=false");
        if (succeeds) build.Exit.ShouldBe(0, build.Output);
        else { RequireDiagnostic(build, diagnostic!); build.Output.ShouldContain("Subject.cs("); }
        return build.Output;
    }

    /// <summary>Rejects uncontrolled wall-clock/randomness, service locators and Console in actual production builds.</summary>
    [Fact, Trait("Case", "pins.banned-production")]
    public void ProductionBannedApisFailRealCompilation()
    {
        var project = Fixture("namespace CompilerFixture; /// <summary>Negative production control.</summary>\npublic static class Subject { /// <summary>Reads forbidden ambient inputs.</summary>\npublic static object Read() { System.Console.WriteLine(System.DateTime.UtcNow); _ = System.Random.Shared.Next(); return typeof(System.IServiceProvider); } }");
        var output = BuildFixture(project, succeeds: false, "RS0030");
        foreach (var symbol in new[] { "Console", "UtcNow", "Shared", "IServiceProvider" }) output.ShouldContain(symbol);
        ProofProtocol.Record("pins.banned-production", "RS0030", "Subject.cs: actual production compiler");
    }

    /// <summary>Accepts Shouldly and an awaitable method without an Async suffix; rejects inherited Xunit.Assert usage.</summary>
    [Fact, Trait("Case", "pins.banned-tests")]
    public void TestsRetainShouldlyAndRejectAssert()
    {
        var project = Fixture("using Shouldly; namespace CompilerFixture; /// <summary>Permitted test APIs.</summary>\npublic static class Subject { /// <summary>No Async suffix is required.</summary>\npublic static System.Threading.Tasks.Task Read() { 1.ShouldBe(1); return System.Threading.Tasks.Task.CompletedTask; } }", tests: true);
        BuildFixture(project, succeeds: true);
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(project)!, "Subject.cs"), "namespace CompilerFixture; /// <summary>Forbidden assertion library.</summary>\npublic static class Subject { /// <summary>Rejects the inherited assertion ban.</summary>\npublic static void Read() { Xunit.Assert.True(true); } }");
        BuildFixture(project, succeeds: false, "RS0030").ShouldContain("Assert");
        ProofProtocol.Record("pins.banned-tests", "TEST_POLICY_EXACT", "Subject.cs: Shouldly/permission pass; Xunit.Assert RS0030");
    }

    /// <summary>Rejects severity weakening when it removes the intended subject diagnostic.</summary>
    [Fact, Trait("Case", "pins.banned-severity")]
    public void BannedSeverityWeakeningCannotSatisfyTheContract()
    {
        var project = Fixture("namespace CompilerFixture; /// <summary>Diagnostic severity control.</summary>\npublic static class Subject { /// <summary>Forbidden output.</summary>\npublic static void Read() => System.Console.WriteLine(\"forbidden\"); }");
        BuildFixture(project, succeeds: false, "RS0030");
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(project)!, ".editorconfig"), "root = true\n[*.cs]\ndotnet_diagnostic.RS0030.severity = none\n");
        var changed = Dotnet("build", project, "--no-restore", "-p:NuGetAudit=false");
        changed.Exit.ShouldBe(0, changed.Output);
        var rejected = Should.Throw<InvalidOperationException>(() => RequireDiagnostic(changed, "RS0030"));
        rejected.Message.ShouldContain("POLICY_BYPASS");
        ProofProtocol.Record("pins.banned-severity", "POLICY_BYPASS_REJECTED", "real compiler loses RS0030 only under deliberate severity weakening");
    }

    private static void RequireDiagnostic((int Exit, string Output) result, string diagnostic)
    {
        if (result.Exit == 0 || !result.Output.Contains("error " + diagnostic + ":", StringComparison.Ordinal)) throw new InvalidOperationException("POLICY_BYPASS: intended " + diagnostic + " absent");
    }

    /// <summary>Preserves permission for awaitable methods without an Async suffix and rejects a configuration that removes it.</summary>
    [Fact, Trait("Case", "pins.async-permission")]
    public void AsyncSuffixPermissionRejectsConfigurationDrift()
    {
        var project = Fixture("namespace CompilerFixture; /// <summary>Awaitable naming permission.</summary>\npublic static class Subject { /// <summary>Permitted suffix omission.</summary>\npublic static System.Threading.Tasks.Task Read() => System.Threading.Tasks.Task.CompletedTask; }");
        BuildFixture(project, succeeds: true);
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(project)!, ".editorconfig"), "root = true\n[*.cs]\ndotnet_diagnostic.MA0137.severity = error\n");
        BuildFixture(project, succeeds: false, "MA0137");
        ProofProtocol.Record("pins.async-permission", "ASYNC_PERMISSION_EXACT", "permitted Read passes; wrong naming requirement emits MA0137");
    }

    /// <summary>Requires five canonical locks and rejects a changed exact dependency under clean locked restore.</summary>
    [Fact, Trait("Case", "ac-01.complete")]
    public void FiveCanonicalLocksRejectDependencyDrift()
    {
        var hashes = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var project in Projects)
        {
            var path = Path.Combine(ProofProtocol.Root, Path.GetDirectoryName(project)!, "packages.lock.json");
            File.Exists(path).ShouldBeTrue(path);
            hashes.Add(path, Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))));
        }
        var cleanRoot = Path.Combine(ProofProtocol.Root, ".phase1/temp/clean-restore-" + Guid.NewGuid().ToString("N"));
        var copyFiles = new List<string> { "KingdomZombies.slnx", "Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "NuGet.config", "global.json" };
        foreach (var project in Projects) { copyFiles.Add(project); copyFiles.Add(Path.Combine(Path.GetDirectoryName(project)!, "packages.lock.json")); }
        foreach (var relative in copyFiles)
        {
            var destination = Path.Combine(cleanRoot, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(Path.Combine(ProofProtocol.Root, relative), destination);
        }
        var config = Path.Combine(cleanRoot, "NuGet.config");
        File.WriteAllText(config, File.ReadAllText(config).Replace("value=\".phase1/feed\"", "value=\"" + Path.Combine(ProofProtocol.Root, ".phase1/feed") + "\"", StringComparison.Ordinal));
        Directory.EnumerateFiles(cleanRoot, "project.assets.json", SearchOption.AllDirectories).ShouldBeEmpty();
        var restore = Dotnet("restore", Path.Combine(cleanRoot, "KingdomZombies.slnx"), "--locked-mode", "-p:NuGetAudit=false");
        restore.Exit.ShouldBe(0, restore.Output);
        Directory.EnumerateFiles(cleanRoot, "project.assets.json", SearchOption.AllDirectories).Count().ShouldBe(5);
        foreach (var (path, hash) in hashes) Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ShouldBe(hash);
        var fixture = Fixture("namespace CompilerFixture; /// <summary>Restore-only control.</summary>\npublic static class Subject { }");
        File.Copy(Path.Combine(ProofProtocol.Root, "src/core/packages.lock.json"), Path.Combine(Path.GetDirectoryName(fixture)!, "packages.lock.json"));
        File.WriteAllText(fixture, File.ReadAllText(fixture).Replace("</Project>", "<ItemGroup><PackageReference Include=\"Shouldly\"/></ItemGroup></Project>", StringComparison.Ordinal));
        var changed = Dotnet("restore", fixture, "--locked-mode", "-p:NuGetAudit=false");
        changed.Exit.ShouldNotBe(0);
        changed.Output.ShouldContain("NU1004");
        ProofProtocol.Record("ac-01.complete", "LOCKED_GRAPH_EXACT", "five canonical locks + NU1004 changed dependency");
    }

    /// <summary>Requires the pinned generator package and authentic generated method before accepting its report adapter.</summary>
    [Fact, Trait("Case", "pins.generated-report")]
    public void GeneratedReportAdapterHasPinnedOrigin()
    {
        var build = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false", "-t:Rebuild");
        build.Exit.ShouldBe(0, build.Output);
        var package = Path.Combine(ProofProtocol.Root, ".phase1/feed/Godot.SourceGenerators.4.7.2.nupkg");
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(package))).ShouldBe("ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e");
        var resolve = Dotnet("msbuild", Projects[2], "-nologo", "-target:ResolveReferences", "-getItem:Analyzer");
        resolve.Exit.ShouldBe(0, resolve.Output);
        using var analyzers = JsonDocument.Parse(resolve.Output);
        analyzers.RootElement.GetProperty("Items").GetProperty("Analyzer").EnumerateArray().Any(a => a.GetProperty("FullPath").GetString()!.Replace('\\', '/').EndsWith("godot.sourcegenerators/4.7.2/analyzers/dotnet/cs/Godot.SourceGenerators.dll", StringComparison.Ordinal)).ShouldBeTrue();
        var generated = Path.Combine(ProofProtocol.Root, "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot.SourceGenerators.GodotPluginsInitializerGenerator/GodotPlugins.Game.generated.cs");
        var source = File.ReadAllText(generated);
        source.ShouldContain("private static godot_bool InitializeFromGameProject(IntPtr godotDllHandle, IntPtr outManagedCallbacks,");
        source.ShouldContain("global::System.Console.Error.WriteLine(e);");
        using var registry = JsonDocument.Parse(File.ReadAllText(Path.Combine(ProofProtocol.Root, "docs/code/engine-adaptations.json")));
        var entry = registry.RootElement.GetProperty("local").EnumerateArray().Single();
        Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(generated))).ShouldBe(entry.GetProperty("generatedSourceSha256").GetString());
        entry.GetProperty("diagnostic").GetString().ShouldBe("RS0030");
        File.ReadAllText(Path.Combine(ProofProtocol.Root, "src/godot-game/GeneratedReportAdapter.cs")).ShouldContain(entry.GetProperty("target").GetString()!);
        ProofProtocol.Record("pins.generated-report", "GENERATOR_ORIGIN_EXACT", generated);
    }

    private static string BuildGameWithSource(string source)
    {
        var directory = Path.Combine(ProofProtocol.Root, ".phase1/temp/game-boundary-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var subject = Path.Combine(directory, "Main.cs");
        File.WriteAllText(subject, source);
        var targets = Path.Combine(directory, "Boundary.targets");
        File.WriteAllText(targets, "<Project><Import Project=\"" + Path.Combine(ProofProtocol.Root, "Directory.Build.targets") + "\"/><ItemGroup Condition=\"'$(MSBuildProjectName)' == 'KingdomZombies'\"><Compile Include=\"" + subject + "\"/></ItemGroup></Project>");
        var result = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false", "-p:DirectoryBuildTargetsPath=" + targets);
        result.Exit.ShouldNotBe(0, "Forbidden game input must not acquire generated permission");
        return result.Output;
    }

    /// <summary>Rejects handwritten adjacent Console use and a same-signature generated-method impostor.</summary>
    [Fact, Trait("Case", "pins.generated-report-boundary")]
    public void GeneratedReportPermissionCannotEscapeItsMethod()
    {
        var adjacent = BuildGameWithSource("namespace GodotPlugins.Game; internal static partial class Main { private static void Adjacent() => System.Console.WriteLine(\"forbidden\"); }");
        adjacent.ShouldContain("Main.cs(");
        adjacent.ShouldContain("error RS0030:");
        var impostor = BuildGameWithSource("namespace GodotPlugins.Game; internal static partial class Main { private static bool InitializeFromGameProject(System.IntPtr a, System.IntPtr b, System.IntPtr c, int d) { System.Console.WriteLine(\"impostor\"); return false; } }");
        impostor.ShouldContain("error CS0111:");
        impostor.ShouldContain("InitializeFromGameProject");
        var clean = Dotnet("build", Projects[2], "--no-restore", "-p:NuGetAudit=false");
        clean.Exit.ShouldBe(0, clean.Output);
        ProofProtocol.Record("pins.generated-report-boundary", "GENERATED_PERMISSION_BOUNDED", "Subject.cs RS0030 adjacent / CS0111 impostor");
    }
}
