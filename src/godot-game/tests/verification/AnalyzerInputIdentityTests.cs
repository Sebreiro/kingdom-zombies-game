using System;
using System.IO;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Proves analyzer source participates in the independent input identity without changing approved
/// inputs.</summary>
public sealed class AnalyzerInputIdentityTests
{
    /// <summary>Mutates a copied real analyzer and rejects its old identity; cache output stays outside the
    /// declared scope.</summary>
    [Fact, Trait("Case", "rules.analyzer-input-identity")]
    public void Fingerprint_ChangedAnalyzerSource_InvalidatesInputIdentity()
    {
        string directory = WorkspacePaths.Temp(purpose: "analyzer-identity-"
                + Guid.NewGuid().ToString(format: "N"));
        Directory.CreateDirectory(path: Path.Combine(path1: directory, path2: "src/godot-game/tools/code-rules"));
        string source = Path.Combine(path1: ProofProtocol.Root, path2:
                "src/godot-game/tools/code-rules/NamedArgumentAnalyzer.cs");
        string copy = Path.Combine(path1: directory, path2: "src/godot-game/tools/code-rules/NamedArgumentAnalyzer.cs");
        File.Copy(sourceFileName: source, destFileName: copy);
        string script = """
            const fs = require('fs'), path = require('path');
            const c = require('./src/godot-game/tools/verification/check.cjs');
            const manifest = require('./src/godot-game/tools/verification/required-cases.json');
            const analyzer = 'src/godot-game/tools/code-rules';
            const input = analyzer + '/NamedArgumentAnalyzer.cs';
            const inputs = manifest.inputs.filter(scope => input.startsWith(scope + '/') || input === scope);
            if (inputs.length !== 1) throw Error('ANALYZER_SCOPE_MISSING_OR_AMBIGUOUS');
            const root = fs.realpathSync(process.env.PHASE1_IDENTITY_COPY);
            const scope = { inputs };
            const before = c.snapshot(root, scope);
            if (before.files.length !== 1 || before.files[0].path !== input) throw Error('ANALYZER_SOURCE_MISSING');
            fs.appendFileSync(path.join(root, input), '\n// isolated source mutation\n');
            const after = c.snapshot(root, scope);
            if (before.sha256 === after.sha256) throw Error('SOURCE_IDENTITY_UNCHANGED');
            let rejected = false;
            try {
              c.reconcile([{id:'source'}], [{id:'source',status:'pass',nonce:'nonce',input:before.sha256}],
                {nonce:'nonce',input:after.sha256});
            } catch (error) {
              if (error.diagnostic !== 'INPUT_MISMATCH') throw error;
              rejected = true;
            }
            if (!rejected) throw Error('STALE_SOURCE_ACCEPTED');
            fs.mkdirSync(path.join(root, analyzer, 'obj'), {recursive:true});
            fs.writeFileSync(path.join(root, analyzer, 'obj', 'cache.cs'), 'untrusted output');
            if (c.snapshot(root, scope).sha256 !== after.sha256) throw Error('CACHE_SCOPE_LEAK');
            console.log('ANALYZER_INPUT_CHANGED');
            """;
        byte[] original = File.ReadAllBytes(path: source);
        string? previous = Environment.GetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY");
        try
        {
            Environment.SetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY", value: directory);
            (int Exit, string Output) result = ProofProtocol.Node(script: script);
            result.Exit.ShouldBe(expected: 0, customMessage: result.Output);
            result.Output.ShouldContain(expected: "ANALYZER_INPUT_CHANGED");
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable: "PHASE1_IDENTITY_COPY", value: previous);
        }

        File.ReadAllBytes(path: source).ShouldBe(expected: original);
        ProofProtocol.Record(id: "rules.analyzer-input-identity", diagnostic: "ANALYZER_INPUT_CHANGED",
                location: ("isolated copied NamedArgumentAnalyzer.cs; approved sour" + "ce unchanged; obj excluded"));
    }
}
