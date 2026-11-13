using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises exact toolchain declarations and observed identities with deliberate incompatible controls.</summary>
public sealed class PinsTests
{
    private static void Reject(string id, string mutation, string diagnostic)
    {
        var script = "const b=require('./tools/phase1/bootstrap.cjs'),fs=require('node:fs'),cp=require('node:child_process'),p=require('node:path'); " +
            "const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json')); " +
            "try { " + mutation + "; console.log('ACCEPTED'); } catch(e) { console.log(e.diagnostic); process.exitCode=1; }";
        var result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 1);
        result.Output.Trim().ShouldBe(expected: diagnostic);
        ProofProtocol.Record(id: id, diagnostic: diagnostic, location: "exact pinned declarations/observed tools");
    }

    /// <summary>Requires a missing SDK pin to fail validation with PINS_MISSING.</summary>
    [Fact, Trait("Case", "pins.missing")]
    public void MissingPinIsRejected() => Reject(id: "pins.missing", mutation: "delete m.sdk;b.validateManifest(m)", diagnostic: "PINS_MISSING");
    /// <summary>Rejects repeated SDK keys even when both values are identical.</summary>
    [Fact, Trait("Case", "pins.duplicate-identical")]
    public void DuplicateIdenticalDeclarationRequiresUniqueness() => Reject(id: "pins.duplicate-identical", mutation: "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"10.0.404\"}')", diagnostic: "PINS_CONFLICT");
    /// <summary>Rejects conflicting SDK keys before a parser can select a value.</summary>
    [Fact, Trait("Case", "pins.duplicate-conflicting")]
    public void ConflictingDuplicateDeclarationIsRejected() => Reject(id: "pins.duplicate-conflicting", mutation: "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"8.0.100\"}')", diagnostic: "PINS_CONFLICT");
    /// <summary>Rejects duplicate Godot package candidates instead of selecting by order.</summary>
    [Fact, Trait("Case", "pins.duplicate-candidate")]
    public void AmbiguousCandidatesCannotSelectAccidentally() => Reject(id: "pins.duplicate-candidate", mutation: "m.godotPackages.push(m.godotPackages[0]);b.validateManifest(m)", diagnostic: "PINS_CONFLICT");
    /// <summary>Rejects a source generator version that differs from the selected engine packages.</summary>
    [Fact, Trait("Case", "pins.generator")]
    public void IncompatibleGeneratorIsRejected() => Reject(id: "pins.generator", mutation: "m.godotPackages.find(x=>x.id==='Godot.SourceGenerators').version='4.7.1';b.validateManifest(m)", diagnostic: "PINS_MISMATCH");
    /// <summary>Compares an actual foreign executable identity against the pinned engine.</summary>
    [Fact, Trait("Case", "pins.observed-engine")]
    public void WrongActualEngineToolIsRejected() => Reject(id: "pins.observed-engine", mutation: "const r=cp.spawnSync(process.execPath,['--version'],{encoding:'utf8'});if(r.status!==0)throw Error('prerequisite');b.observedIdentity('engine',r.stdout.trim(),m.engineFullVersion)", diagnostic: "PINS_MISMATCH");
    /// <summary>Rejects the SDK identity when presented as the required compiler identity.</summary>
    [Fact, Trait("Case", "pins.observed-compiler")]
    public void WrongActualCompilerIdentityIsRejected() => Reject(id: "pins.observed-compiler", mutation: "const r=cp.spawnSync('dotnet',['--version'],{encoding:'utf8'});if(r.status!==0)throw Error('prerequisite');b.observedIdentity('compiler',r.stdout.trim(),m.compiler)", diagnostic: "PINS_MISMATCH");
    /// <summary>Hashes the acquired generator package and rejects an intentionally different hash.</summary>
    [Fact, Trait("Case", "pins.observed-generator")]
    public void WrongGeneratorPayloadIdentityIsRejected() => Reject(id: "pins.observed-generator", mutation: "const r=fs.readFileSync('.phase1/feed/Godot.SourceGenerators.4.7.2.nupkg');const h=require('node:crypto').createHash('sha256').update(r).digest('hex');b.observedIdentity('generator-payload',h,'0'.repeat(64))", diagnostic: "PINS_MISMATCH");

    /// <summary>Proves equivalent declaration and candidate permutations select the same toolchain.</summary>
    [Fact, Trait("Case", "pins.permutation")]
    public void CandidateAndDeclarationOrderingPreservesSelection()
    {
        var closure = ProofProtocol.Node(script: "const b=require('./tools/phase1/bootstrap.cjs'),fs=require('node:fs');const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json'));const expected=[...m.godotPackages.filter(x=>x.id!=='Godot.NET.Sdk'),{id:'Meziantou.Analyzer',version:'3.0.312'},{id:'Roslynator.Analyzers',version:'5.0.1'},{id:'Microsoft.CodeAnalysis.BannedApiAnalyzers',version:'5.6.0'}];const lock=Object.fromEntries(expected.map(x=>[x.id,{resolved:x.version}]));for(const id of ['kingdomzombies.core','kingdomzombies.multiplayer.abstractions'])lock[id]={type:'Project'};b.validateEnginePackages(m,lock);b.validateEnginePackages(m,Object.fromEntries(Object.entries(lock).reverse()));for(const mutate of [l=>delete l['GodotSharp'],l=>l['unexpected']={resolved:'1.0.0'},l=>l['Meziantou.Analyzer'].resolved='3.0.311',l=>delete l['kingdomzombies.core'],l=>l['kingdomzombies.core'].type='Direct',l=>l['kingdomzombies.core'].dependencies={unexpected:'1.0.0'}]){const bad=structuredClone(lock);mutate(bad);let rejected=false;try{b.validateEnginePackages(m,bad)}catch(e){if(e.diagnostic!=='RESTORE_FAILED')throw e;rejected=true}if(!rejected)process.exitCode=1}console.log('CLOSURE_EXACT')");
        closure.Exit.ShouldBe(expected: 0);
        closure.Output.ShouldContain(expected: "CLOSURE_EXACT");
        var result = ProofProtocol.Node(script: "const b=require('./tools/phase1/bootstrap.cjs'),fs=require('node:fs');const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json'));const p=Object.fromEntries(Object.entries(m).reverse());p.godotPackages=[...m.godotPackages].reverse();if(b.validateManifest(m)!==b.validateManifest(p))process.exitCode=1;const i={global:fs.readFileSync('global.json','utf8'),tools:fs.readFileSync('.config/dotnet-tools.json','utf8'),project:fs.readFileSync('src/godot-game/KingdomZombies.csproj','utf8'),nuget:fs.readFileSync('NuGet.config','utf8')};b.declarations(m,i);i.global=JSON.stringify({sdk:{allowPrerelease:false,rollForward:'disable',version:m.sdk}});b.declarations(p,i);console.log('PERMUTATION_EQUAL')");
        result.Exit.ShouldBe(expected: 0);
        result.Output.ShouldContain(expected: "PERMUTATION_EQUAL");
        ProofProtocol.Record(id: "pins.permutation", diagnostic: "PERMUTATION_EQUAL", location: "candidate/declaration permutations");
    }
}
