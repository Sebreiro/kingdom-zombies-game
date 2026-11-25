using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Shouldly;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Mutates the actual store in an isolated compiled copy and reruns the unchanged target assertion.</summary>
internal static class RestoreMutationProof
{
    /// <summary>Requires clean execution, then detects actual premature first-record publication.</summary>
    internal static void Verify()
    {
        var fixture = new BuildFixture(tests: true);
        fixture.Restore();
        (int Exit, string Output) restored = fixture.Build();
        restored.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN_BUILD: " + restored.Output);
        (int Exit, string Output) clean = Run(root: fixture.DirectoryPath, report: "clean.xml");
        clean.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_CLEAN_TARGET: " + clean.Output);
        string path = Path.Combine(path1: fixture.DirectoryPath, path2: "src/godot-game/core/ExampleStateStore.cs");
        string original = File.ReadAllText(path: path);
        const string Marker = "            beforeCommit?.Invoke(obj: candidate);";
        original.Split(separator: Marker, options: StringSplitOptions.None).Length.ShouldBe(expected: 2);
        string mutation = original.Replace(oldValue: Marker, newValue: EarlyPublication + Marker,
                comparisonType: StringComparison.Ordinal);
        File.WriteAllText(path: path, contents: mutation,
                encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        (int Exit, string Output) build = fixture.Build();
        build.Exit.ShouldBe(expected: 0, customMessage: "INFRASTRUCTURE_MUTATED_BUILD: " + build.Output);
        (int Exit, string Output) result = Run(root: fixture.DirectoryPath, report: "mutation.xml");
        result.Exit.ShouldNotBe(expected: 0, customMessage: "EARLY_RECORD_MUTATION_SURVIVED");
        var report = XDocument.Load(uri: Path.Combine(path1: fixture.DirectoryPath, path2: "mutation.xml"));
        XElement[] tests = [.. report.Descendants(name: "testcase")];
        tests.Length.ShouldBe(expected: 1);
        tests[0].Attribute(name: "name")?.Value.ShouldContain(
                expected: "Restore_InterruptedPreparation_PreservesExactRoot");
        XElement failure = tests[0].Element(name: "failure")
                ?? throw new InvalidOperationException(message: "MUTATION_ASSERTION_REQUIRED: " + result.Output);
        failure.ToString().ShouldContain(expected: "ATOMIC_ROOT_UNCHANGED");
        report.Descendants(name: "error").ShouldBeEmpty();
        report.Descendants(name: "skipped").ShouldBeEmpty();
    }

    private const string EarlyPublication = "            ExampleState prior = Read();\n"
            + "            var firstRecord = new System.Collections.Generic.Dictionary<StableIdentity, long>(\n"
            + "                    collection: prior.Records);\n"
            + "            StableIdentity first = candidate.Ordered[0];\n"
            + "            firstRecord[first] = candidate.Records[first];\n"
            + "            var partial = new ExampleState(elapsedMilliseconds: prior.ElapsedMilliseconds,\n"
            + "                    ordered: prior.Ordered, records: firstRecord, seed: prior.Seed,\n"
            + "                    drawPosition: prior.DrawPosition);\n"
            + "            Interlocked.Exchange(location1: ref _root, value: partial);\n";

    private static (int Exit, string Output) Run(string root, string report)
    {
        var start = new ProcessStartInfo(fileName: "dotnet")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string key in start.Environment.Keys.ToArray())
        {
            if (key.StartsWith(value: "PHASE1_", comparisonType: StringComparison.Ordinal))
            {
                start.Environment.Remove(key: key);
            }
        }

        string[] arguments = ["src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll",
            "--filter-method", "*RestoreAtomicityTests.Restore_InterruptedPreparation_PreservesExactRoot",
            "--report-xunit-junit", "--report-xunit-junit-filename", report, "--results-directory", root,
            "--no-ansi", "--parallel", "none"];
        foreach (string argument in arguments)
        {
            start.ArgumentList.Add(item: argument);
        }

        return Capture(start: start);
    }

    private static (int Exit, string Output) Capture(ProcessStartInfo start)
    {
        using Process child = Process.Start(startInfo: DotnetLaunch.Configure(start: start))
                ?? throw new InvalidOperationException(message: "MUTATION_CHILD_START");
        Task<string> output = child.StandardOutput.ReadToEndAsync();
        Task<string> error = child.StandardError.ReadToEndAsync();
        if (!child.WaitForExit(milliseconds: 20000))
        {
            child.Kill(entireProcessTree: true);
            child.WaitForExit();
            throw new TimeoutException(message: "MUTATION_CHILD_TIMEOUT");
        }

        return (child.ExitCode, output.GetAwaiter().GetResult() + error.GetAwaiter().GetResult());
    }
}
