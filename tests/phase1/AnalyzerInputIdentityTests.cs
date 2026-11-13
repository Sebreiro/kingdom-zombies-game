using System;
using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves analyzer source participates in the independent input identity without changing approved inputs.</summary>
public sealed class AnalyzerInputIdentityTests
{
    /// <summary>Mutates a copied real analyzer and rejects its old identity; cache output stays outside the declared scope.</summary>
    [Fact, Trait("Case", "rules.analyzer-input-identity")]
    public void AnalyzerSourceMutationInvalidatesInputIdentity()
    {
        var directory = Path.Combine(path1: ProofProtocol.Root, path2: ".phase1/temp/analyzer-identity-" + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: Path.Combine(path1: directory, path2: "tools/code-rules"));
        var source = Path.Combine(path1: ProofProtocol.Root, path2: "tools/code-rules/NamedArgumentAnalyzer.cs");
        var copy = Path.Combine(path1: directory, path2: "tools/code-rules/NamedArgumentAnalyzer.cs");
        File.Copy(sourceFileName: source, destFileName: copy);
        var script = "const fs=require('fs'),path=require('path'),c=require('./tools/phase1/check.cjs'),m=require('./tools/phase1/required-cases.json');if(!m.inputs.includes('tools/code-rules'))throw Error('ANALYZER_SCOPE_MISSING');const root=fs.realpathSync(process.env.PHASE1_IDENTITY_COPY),before=c.snapshot(root,{inputs:['tools/code-rules']}),file=path.join(root,'tools/code-rules/NamedArgumentAnalyzer.cs');fs.appendFileSync(file,'\\n// isolated source mutation\\n');const after=c.snapshot(root,{inputs:['tools/code-rules']});if(before.sha256===after.sha256)throw Error('SOURCE_IDENTITY_UNCHANGED');let rejected=false;try{c.reconcile([{id:'source'}],[{id:'source',status:'pass',nonce:'nonce',input:before.sha256}],{nonce:'nonce',input:after.sha256})}catch(e){if(e.diagnostic!=='INPUT_MISMATCH')throw e;rejected=true}if(!rejected)throw Error('STALE_SOURCE_ACCEPTED');fs.mkdirSync(path.join(root,'tools/code-rules/obj'),{recursive:true});fs.writeFileSync(path.join(root,'tools/code-rules/obj/cache.cs'),'untrusted output');if(c.snapshot(root,{inputs:['tools/code-rules']}).sha256!==after.sha256)throw Error('CACHE_SCOPE_LEAK');console.log('ANALYZER_INPUT_CHANGED')";
        var original = File.ReadAllBytes(path: source);
        var previous = Environment.GetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY");
        try
        {
            Environment.SetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY", value: directory);
            var result = ProofProtocol.Node(script: script);
            result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
            result.Output.ShouldContain(expected: "ANALYZER_INPUT_CHANGED");
        }
        finally { Environment.SetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY", value: previous); }
        File.ReadAllBytes(path: source).ShouldBe(expected: original);
        ProofProtocol.Record(id: "rules.analyzer-input-identity", diagnostic: "ANALYZER_INPUT_CHANGED", location: "isolated copied NamedArgumentAnalyzer.cs; approved source unchanged; obj excluded");
    }
}
