using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Exercises exact toolchain declarations and observed identities with deliberate incompatible
/// controls.</summary>
public sealed class PinsTests
{
    /// <summary>Requires a missing SDK pin to fail validation with PINS_MISSING.</summary>
    [Fact, Trait("Case", "pins.missing")]
    public void ValidatePins_MissingPin_Rejected()
    {
        Reject(id: "pins.missing", mutation: "delete m.sdk;b.validateManifest(m)", diagnostic: "PINS_MISSING");
    }

    /// <summary>Rejects repeated SDK keys even when both values are identical.</summary>
    [Fact, Trait("Case", "pins.duplicate-identical")]
    public void ValidatePins_IdenticalDuplicate_RejectsNonUniqueDeclaration()
    {
        Reject(id: "pins.duplicate-identical",
                mutation: "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"10.0.404\"}')", diagnostic: "PINS_CONFLICT");
    }

    /// <summary>Rejects conflicting SDK keys before a parser can select a value.</summary>
    [Fact, Trait("Case", "pins.duplicate-conflicting")]
    public void ValidatePins_ConflictingDuplicateDeclaration_Rejected()
    {
        Reject(id: "pins.duplicate-conflicting",
                mutation: "b.parseJson('{\"sdk\":\"10.0.404\",\"sdk\":\"8.0.100\"}')", diagnostic: "PINS_CONFLICT");
    }

    /// <summary>Rejects duplicate Godot package candidates instead of selecting by order.</summary>
    [Fact, Trait("Case", "pins.duplicate-candidate")]
    public void ValidatePins_AmbiguousCandidates_RejectsAccidentalSelection()
    {
        Reject(id: "pins.duplicate-candidate",
                mutation: "m.godotPackages.push(m.godotPackages[0]);b.validateManifest(m)",
                diagnostic: "PINS_CONFLICT");
    }

    /// <summary>Rejects a source generator version that differs from the selected engine packages.</summary>
    [Fact, Trait("Case", "pins.generator")]
    public void ValidatePins_IncompatibleGenerator_Rejected()
    {
        Reject(id: "pins.generator", mutation: ("m.godotPackages.find(x=>x.id==='Godot.SourceGenerators'"
                + ").version='4.7.1';b.validateManifest(m)"), diagnostic: "PINS_MISMATCH");
    }

    /// <summary>Compares an actual foreign executable identity against the pinned engine.</summary>
    [Fact, Trait("Case", "pins.observed-engine")]
    public void ValidatePins_WrongActualEngineTool_Rejected()
    {
        Reject(id: "pins.observed-engine", mutation: ("const r=cp.spawnSync(process.execPath,['--version'],{en"
                + "coding:'utf8'});if(r.status!==0)throw Error('prerequisi"
                + "te');b.observedIdentity('engine',r.stdout.trim(),m.engi" + "neFullVersion)"),
                diagnostic: "PINS_MISMATCH");
    }

    /// <summary>Rejects the SDK identity when presented as the required compiler identity.</summary>
    [Fact, Trait("Case", "pins.observed-compiler")]
    public void ValidatePins_WrongActualCompilerIdentity_Rejected()
    {
        Reject(id: "pins.observed-compiler",
                mutation: ("const r=cp.spawnSync('dotnet',['--version'],{encoding:'"
                        + "utf8'});if(r.status!==0)throw Error('prerequisite');b.o"
                        + "bservedIdentity('compiler',r.stdout.trim(),m.compiler)"), diagnostic: "PINS_MISMATCH");
    }

    /// <summary>Hashes the acquired generator package and rejects an intentionally different hash.</summary>
    [Fact, Trait("Case", "pins.observed-generator")]
    public void ValidatePins_WrongGeneratorPayloadIdentity_Rejected()
    {
        Reject(id: "pins.observed-generator",
                mutation: ("const r=fs.readFileSync('.phase1/feed/Godot.SourceGener"
                        + "ators.4.7.2.nupkg');const h=require('node:crypto').crea"
                        + "teHash('sha256').update(r).digest('hex');b.observedIden"
                        + "tity('generator-payload',h,'0'.repeat(64))"), diagnostic: "PINS_MISMATCH");
    }

    /// <summary>Proves equivalent declaration and candidate permutations select the same toolchain.</summary>
    [Fact, Trait("Case", "pins.permutation")]
    public void ValidatePins_PermutedCandidates_PreservesSelection()
    {
        (int Exit, string Output) closure = ProofProtocol.Node(
                script: ("const b=require('./tools/phase1/bootstrap.cjs'),fs=requ"
                        + "ire('node:fs');const m=JSON.parse(fs.readFileSync('tool"
                        + "s/phase1/toolchain.json'));const expected=[...m.godotPa"
                        + "ckages.filter(x=>x.id!=='Godot.NET.Sdk'),{id:'Meziantou"
                        + ".Analyzer',version:'3.0.312'},{id:'Roslynator.Analyzers"
                        + "',version:'5.0.1'},{id:'Microsoft.CodeAnalysis.BannedAp"
                        + "iAnalyzers',version:'5.6.0'}];const lock=Object.fromEnt"
                        + "ries(expected.map(x=>[x.id,{resolved:x.version}]));for("
                        + "const id of ['kingdomzombies.core','kingdomzombies.mult"
                        + "iplayer.abstractions'])lock[id]={type:'Project'};b.vali"
                        + "dateEnginePackages(m,lock);b.validateEnginePackages(m,O"
                        + "bject.fromEntries(Object.entries(lock).reverse()));for("
                        + "const mutate of [l=>delete l['GodotSharp'],l=>l['unexpe"
                        + "cted']={resolved:'1.0.0'},l=>l['Meziantou.Analyzer'].re"
                        + "solved='3.0.311',l=>delete l['kingdomzombies.core'],l=>"
                        + "l['kingdomzombies.core'].type='Direct',l=>l['kingdomzom"
                        + "bies.core'].dependencies={unexpected:'1.0.0'}]){const b"
                        + "ad=structuredClone(lock);mutate(bad);let rejected=false"
                        + ";try{b.validateEnginePackages(m,bad)}catch(e){if(e.diag"
                        + "nostic!=='RESTORE_FAILED')throw e;rejected=true}if(!rej"
                        + "ected)process.exitCode=1}console.log('CLOSURE_EXACT')"));
        closure.Exit.ShouldBe(expected: 0);
        closure.Output.ShouldContain(expected: "CLOSURE_EXACT");
        (int Exit, string Output) result = ProofProtocol.Node(
                script: ("const b=require('./tools/phase1/bootstrap.cjs'),fs=requ"
                        + "ire('node:fs');const m=JSON.parse(fs.readFileSync('tool"
                        + "s/phase1/toolchain.json'));const p=Object.fromEntries(O"
                        + "bject.entries(m).reverse());p.godotPackages=[...m.godot"
                        + "Packages].reverse();if(b.validateManifest(m)!==b.valida"
                        + "teManifest(p))process.exitCode=1;const i={global:fs.rea"
                        + "dFileSync('src/godot-game/global.json','utf8'),tools:fs.readFileSync('"
                        + ".config/dotnet-tools.json','utf8'),project:fs.readFileS"
                        + "ync('src/godot-game/game/KingdomZombies.csproj','utf8'),nuge"
                        + "t:fs.readFileSync('src/godot-game/NuGet.config','utf8')};b.declaration"
                        + "s(m,i);i.global=JSON.stringify({sdk:{allowPrerelease:fa"
                        + "lse,rollForward:'disable',version:m.sdk}});b.declaratio"
                        + "ns(p,i);console.log('PERMUTATION_EQUAL')"));
        result.Exit.ShouldBe(expected: 0);
        result.Output.ShouldContain(expected: "PERMUTATION_EQUAL");
        ProofProtocol.Record(id: "pins.permutation", diagnostic: "PERMUTATION_EQUAL",
                location: "candidate/declaration permutations");
    }

    private static void Reject(string id, string mutation, string diagnostic)
    {
        string script = ("const b=require('./tools/phase1/bootstrap.cjs'),fs=requ"
                + "ire('node:fs'),cp=require('node:child_process'),p=requi" + "re('node:path'); ")
                + "const m=JSON.parse(fs.readFileSync('tools/phase1/toolchain.json')); " + "try { " + mutation
                + ("; console.log('ACCEPTED'); } catch(e) { console.log(e.d" + "iagnostic); process.exitCode=1; }");
        (int Exit, string Output) result = ProofProtocol.Node(script: script);
        result.Exit.ShouldBe(expected: 1);
        result.Output.Trim().ShouldBe(expected: diagnostic);
        ProofProtocol.Record(id: id, diagnostic: diagnostic, location: "exact pinned declarations/observed tools");
    }
}
