using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using SyntaxPermission = (string Diagnostic, string Suffix, string Hash, int Line, int Column, int Length);

namespace Sebreiro.CodeRules.Analyzers;
/// <summary>Permits only observed native-fixture generated diagnostic spans with complete text identity.</summary>
internal static class EngineGeneratedSyntax
{
    internal static bool IsPermitted(string diagnostic, SyntaxTree tree, TextSpan span)
    {
        string path = tree.FilePath.Replace(oldChar: '\\', newChar: '/');
        SourceText text = tree.GetText();
        LinePosition position = text.Lines.GetLinePosition(position: span.Start);
        foreach (SyntaxPermission row in _spans)
        {
            if (!StringComparer.Ordinal.Equals(x: diagnostic, y: row.Diagnostic) || !path.EndsWith(
                    value: row.Suffix, comparisonType: StringComparison.Ordinal) || position.Line + 1 != row.Line
                            || position.Character + 1 != row.Column || span.Length != row.Length)
            {
                continue;
            }

            using var sha = SHA256.Create();
            string hash = BitConverter.ToString(value: sha.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: text
                    .ToString()))).Replace(oldValue: "-", newValue: "").ToLowerInvariant();
            return StringComparer.Ordinal.Equals(x: hash, y: row.Hash);
        }

        return false;
    }

    private static readonly SyntaxPermission[] _spans =
    [
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 47, 121, 190),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 48, 121, 186),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 49, 121, 365),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 50, 121, 189),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 51, 121, 183),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 52, 121, 364),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 58, 121, 9),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPathAttributeGenerator/"
                + "AssemblyScriptTypes.generated.cs",
            "28871bb36d1cd947eec16cac87d2577ef5a2435601220b86bb08483a773109aa", 1, 121, 46),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 20, 121, 30),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 27, 121, 362),
        ("SCR0008", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 44, 98, 18),
        ("SCR0008", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 24, 98, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 46, 100, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 47, 21, 288),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 48, 21, 284),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 49, 21, 463),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 50, 21, 287),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 51, 21, 281),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 52, 21, 462),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 71, 23, 67),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 71, 82, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 86, 22, 64),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 86, 78, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 90, 44, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 90, 52, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 90, 58, 7),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 13, 56, 18),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 21, 13, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "a6b590969a6911628f263b83eea32baffe858384f7884a047d47c22beaa2abaf", 114, 41, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 39, 20, 16),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 39, 38, 8),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 58, 41, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 12, 34, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 13, 37, 17),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 13, 56, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 19, 37, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 20, 110, 17),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "71123d3a9fc7f0258312579667767b7c73adedc9eb869fe53be40105ec0eadc5", 20, 129, 20),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 26, 100, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 27, 21, 460),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 46, 91, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 49, 45, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "f67cd222d46663404e13d154f0b4f88f34a28f8466e80f956b5f0c020900cb02", 49, 53, 4)
    ];
}
