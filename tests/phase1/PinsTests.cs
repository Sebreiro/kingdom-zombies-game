using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

public sealed class PinsTests
{
    private static void Reject(string id, string mutation, string diagnostic)
    {
        var script = "const b=require('./tools/phase1/bootstrap.cjs'),fs=require('node:fs'),cp=require('node:child_process'),p=require('node:path'); " +
            "const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json')); " +
            "try { " + mutation + "; console.log('ACCEPTED'); } catch(e) { console.log(e.diagnostic); process.exitCode=1; }";
        var result = ProofProtocol.Node(script);
        result.Exit.ShouldBe(1);
        result.Output.Trim().ShouldBe(diagnostic);
        ProofProtocol.Record(id, diagnostic, "exact pinned declarations/observed tools");
    }

    [Fact, Trait("Case", "pins.missing")]
    public void MissingPinIsRejected() => Reject("pins.missing", "delete m.sdk;b.validateManifest(m)", "PINS_MISSING");
    [Fact, Trait("Case", "pins.duplicate-identical")]
    public void DuplicateIdenticalDeclarationRequiresUniqueness() => Reject("pins.duplicate-identical", "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"10.0.404\"}')", "PINS_CONFLICT");
    [Fact, Trait("Case", "pins.duplicate-conflicting")]
    public void ConflictingDuplicateDeclarationIsRejected() => Reject("pins.duplicate-conflicting", "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"8.0.100\"}')", "PINS_CONFLICT");
    [Fact, Trait("Case", "pins.duplicate-candidate")]
    public void AmbiguousCandidatesCannotSelectAccidentally() => Reject("pins.duplicate-candidate", "m.godotPackages.push(m.godotPackages[0]);b.validateManifest(m)", "PINS_CONFLICT");
    [Fact, Trait("Case", "pins.generator")]
    public void IncompatibleGeneratorIsRejected() => Reject("pins.generator", "m.godotPackages.find(x=>x.id==='Godot.SourceGenerators').version='4.7.1';b.validateManifest(m)", "PINS_MISMATCH");
    [Fact, Trait("Case", "pins.observed-engine")]
    public void WrongActualEngineToolIsRejected() => Reject("pins.observed-engine", "const r=cp.spawnSync(process.execPath,['--version'],{encoding:'utf8'});if(r.status!==0)throw Error('prerequisite');b.observedIdentity('engine',r.stdout.trim(),m.engineFullVersion)", "PINS_MISMATCH");
    [Fact, Trait("Case", "pins.observed-compiler")]
    public void WrongActualCompilerIdentityIsRejected() => Reject("pins.observed-compiler", "const r=cp.spawnSync('dotnet',['--version'],{encoding:'utf8'});if(r.status!==0)throw Error('prerequisite');b.observedIdentity('compiler',r.stdout.trim(),m.compiler)", "PINS_MISMATCH");
    [Fact, Trait("Case", "pins.observed-generator")]
    public void WrongGeneratorPayloadIdentityIsRejected() => Reject("pins.observed-generator", "const r=fs.readFileSync('.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg');const h=require('node:crypto').createHash('sha256').update(r).digest('hex');b.observedIdentity('generator-payload',h,'0'.repeat(64))", "PINS_MISMATCH");

    [Fact, Trait("Case", "pins.permutation")]
    public void CandidateAndDeclarationOrderingPreservesSelection()
    {
        var result = ProofProtocol.Node("const b=require('./tools/phase1/bootstrap.cjs'),fs=require('node:fs');const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json'));const p=Object.fromEntries(Object.entries(m).reverse());p.godotPackages=[...m.godotPackages].reverse();if(b.validateManifest(m)!==b.validateManifest(p))process.exitCode=1;const i={global:fs.readFileSync('global.json','utf8'),tools:fs.readFileSync('.config/dotnet-tools.json','utf8'),project:fs.readFileSync('src/godot-game/KingdomZombies.csproj','utf8'),nuget:fs.readFileSync('NuGet.config','utf8')};b.declarations(m,i);i.global=JSON.stringify({sdk:{allowPrerelease:false,rollForward:'disable',version:m.sdk}});b.declarations(p,i);console.log('PERMUTATION_EQUAL')");
        result.Exit.ShouldBe(0);
        result.Output.ShouldContain("PERMUTATION_EQUAL");
        ProofProtocol.Record("pins.permutation", "PERMUTATION_EQUAL", "candidate/declaration permutations");
    }
}
