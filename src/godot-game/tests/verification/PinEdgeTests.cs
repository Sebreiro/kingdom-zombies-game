using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Closes the five original toolchain runner-edge identities with independent actual subjects.</summary>
public sealed class PinEdgeTests
{
    /// <summary>Rejects adjacent identical and conflicting complete-manifest pins before value selection.</summary>
    [Fact, Trait("Case", "edge.rule-01.adjacency")]
    public void Pins_AdjacentCompleteDeclarations_RejectsAmbiguity()
    {
        string script = "const b=require('./src/godot-game/tools/verification/bootstrap.cjs'),fs=require('node:fs');"
                + "const raw=fs.readFileSync('src/godot-game/tools/verification/toolchain.json','utf8');"
                + "b.validateManifest(b.parseJson(raw));for(const value of ['10.0.404','8.0.100']){"
                + "const bad=raw.replace('{','{\"sdk\":\"'+value+'\",');let rejected=false;"
                + "try{b.validateManifest(b.parseJson(bad))}catch(e){if(e.diagnostic!=='PINS_CONFLICT')throw e;"
                + "rejected=true}if(!rejected)throw Error('duplicate accepted')}console.log('PINS_ADJACENCY')";
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.Trim().ShouldBe(expected: "PINS_ADJACENCY");
        ProofProtocol.Record(id: "edge.rule-01.adjacency", diagnostic: "PINS_ADJACENCY",
                location: "src/godot-game/tools/verification/"
                        + "toolchain.json: complete actual manifest plus adjacent duplicate sdk keys");
    }

    /// <summary>Runs an actual isolated coordinator with an empty required inventory and rejects discovery.</summary>
    [Fact, Trait("Case", "edge.rule-01.empty")]
    public void Pins_EmptyRequiredInventory_RejectsVacuousRun()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        string path = Path.Combine(path1: fixture.Root, path2: "src/godot-game/tools/verification/required-cases.json");
        JsonObject manifest = RegisterValidator.Parse(bytes: File.ReadAllBytes(path: path));
        manifest["cases"] = new JsonArray();
        File.WriteAllText(path: path, contents: manifest.ToJsonString());
        (int Exit, string Output) result = fixture.Run();
        result.Exit.ShouldBe(expected: 1, customMessage: result.Output);
        result.Output.ShouldContain(expected: "MANIFEST_EMPTY");
        Directory.Exists(path: Path.Combine(path1: fixture.Root, path2: ".phase1/runs")).ShouldBeFalse();
        ProofProtocol.Record(id: "edge.rule-01.empty", diagnostic: "MANIFEST_EMPTY", location: path);
    }

    /// <summary>Permutes actual declaration keys and package candidates and preserves the selected identity.</summary>
    [Fact, Trait("Case", "edge.rule-01.ordering")]
    public void Pins_ActualInputPermutation_PreservesSelection()
    {
        string script = "const b=require('./src/godot-game/tools/verification/bootstrap.cjs'),fs=require('node:fs');"
                + "const m=b.parseJson(fs.readFileSync('src/godot-game/tools/verification/toolchain.json','utf8'));"
                + "const p=Object.fromEntries(Object.entries(m).reverse());"
                + "p.godotPackages=[...m.godotPackages].reverse();"
                + "const a=b.validateManifest(m),c=b.validateManifest(p);if(a!==c)throw Error('selection drift');"
                + "const inputs={global:fs.readFileSync('src/godot-game/global.json','utf8'),"
                + "tools:fs.readFileSync('src/godot-game/.config/dotnet-tools.json','utf8'),"
                + "project:fs.readFileSync('src/godot-game/game/KingdomZombies.csproj','utf8'),"
                + "nuget:fs.readFileSync('src/godot-game/NuGet.config','utf8')};b.declarations(m,inputs);"
                + "b.declarations(p,Object.fromEntries(Object.entries(inputs).reverse()));"
                + "console.log('PINS_ORDERING '+a)";
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
        result.Output.ShouldStartWith(expected: "PINS_ORDERING ");
        ProofProtocol.Record(id: "edge.rule-01.ordering", diagnostic: "PINS_ORDERING",
                location: "src/godot-game/tools/verification/toolchain.json, global.json, tool/"
                        + "project/feed actual declarations");
    }

    /// <summary>Executes two real runs with unique invocation identities and unchanged declared input bytes.</summary>
    [Fact, Trait("Case", "edge.rule-01.idempotency")]
    public void Pins_RepeatedActualRuns_PreservesInputs()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "pass");
        string path = Path.Combine(path1: fixture.Root, path2: "src/godot-game/tools/verification/toolchain.json");
        string hash = RunnerProcessFixture.Hash(file: path);
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode first = fixture.Summary();
        fixture.Run().Exit.ShouldBe(expected: 0);
        JsonNode second = fixture.Summary();
        first["nonce"]?.GetValue<string>().ShouldNotBe(expected: second["nonce"]?.GetValue<string>(),
                comparer: StringComparer.Ordinal);
        first["input"]?.GetValue<string>().ShouldBe(expected: second["input"]?.GetValue<string>(),
                comparer: StringComparer.Ordinal);
        first["status"]?.GetValue<string>().ShouldBe(expected: "pass");
        second["status"]?.GetValue<string>().ShouldBe(expected: "pass");
        RunnerProcessFixture.Hash(file: path).ShouldBe(expected: hash);
        ProofProtocol.Record(id: "edge.rule-01.idempotency", diagnostic: "PINS_FRESH_RERUNS", location: path);
    }

    /// <summary>Starts two actual coordinators; the competitor cannot combine or replace the first run.</summary>
    [Fact, Trait("Case", "edge.rule-01.concurrency")]
    public void Pins_CompetingActualRuns_RejectsMixedPublication()
    {
        using var fixture = RunnerProcessFixture.Create(mode: "concurrent");
        using Process first = fixture.Start();
        string ready = Path.Combine(path1: fixture.Root, path2: "child.pid");
        var watch = Stopwatch.StartNew();
        while (!File.Exists(path: ready) && watch.ElapsedMilliseconds < 2500)
        {
            Thread.Sleep(millisecondsTimeout: 10);
        }

        File.Exists(path: ready).ShouldBeTrue();
        (int Exit, string Output) second = fixture.Run();
        second.Exit.ShouldBe(expected: 1, customMessage: second.Output);
        second.Output.ShouldContain(expected: "CHECK_LOCKED");
        RunnerProcessFixture.Finish(child: first).Exit.ShouldBe(expected: 0);
        fixture.Summary()["status"]?.GetValue<string>().ShouldBe(expected: "pass");
        Directory.GetFiles(path: Path.Combine(path1: fixture.Root, path2: ".phase1/runs"),
                searchPattern: "summary.json", searchOption: SearchOption.AllDirectories).Length.ShouldBe(expected: 1);
        ProofProtocol.Record(id: "edge.rule-01.concurrency", diagnostic: "PINS_CONCURRENT_ISOLATION",
                location: ready + ": competing coordinator CHECK_LOCKED, one complete fresh summary");
    }
}
