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
            if (StringComparer.Ordinal.Equals(x: hash, y: row.Hash))
            {
                return true;
            }
        }

        return false;
    }

    private static readonly SyntaxPermission[] _spans =
    [
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 55, 121, 190),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 56, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 57, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 58, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 59, 121, 183),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 60, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 61, 121, 195),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 62, 121, 364),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 68, 121, 9),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 27, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 33, 121, 9),
        ("SCR0007", "Godot.SourceGenerators.ScriptPathAttributeGenerator/"
                + "AssemblyScriptTypes.generated.cs",
            "34e0126fbdedd5d9fd26a9768b2d0903161fb87d5149e7be0ed0cbe786661884", 1, 121, 157),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 121, 20),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 121, 20),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 121, 21),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 121, 21),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 68, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 71, 121, 109),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 72, 121, 98),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 73, 121, 109),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 56, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 59, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 60, 121, 89),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 14, 121, 30),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 16, 121, 3),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 121, 38),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 121, 10),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 121, 38),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 14, 121, 30),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 16, 121, 3),
        ("SCR0007", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 121, 34),
        ("SCR0007", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 121, 6),
        ("SCR0007", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 121, 34),
        ("SCR0007", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 121, 30),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 24, 98, 18),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 52, 98, 18),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 28, 29, 26),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 24, 29, 26),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 26, 100, 1),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 27, 21, 282),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 27, 13, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 31, 13, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 49, 41, 6),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 37, 76, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 44, 6),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 52, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 58, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 27, 89, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 31, 84, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 34, 48, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 34, 54, 5),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 41, 81, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 45, 78, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 41, 81, 14),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 45, 78, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 48, 48, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 48, 54, 9),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 59, 24, 186),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 60, 24, 184),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 16, 121, 1),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 20, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 44, 60),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 85, 14),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 79, 24),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 20, 20),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 42, 55),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 80, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 74, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 22, 13, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 24, 13, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 12, 34, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 26, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 50, 50),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 85, 14),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 26, 20),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 48, 45),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 80, 12),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 20, 37, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 21, 33, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 21, 57, 23),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 23, 33, 20),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 23, 55, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 13, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 118, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 35, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 53, 96, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 13, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 118, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 140, 41, 6),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 130, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 105, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 130, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 56, 18),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 71, 24, 204),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 72, 24, 193),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 73, 24, 204),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 16, 121, 1),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 20, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 52, 105),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 124, 32),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 20, 27),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 49, 80),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 99, 29),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 20, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 52, 105),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 124, 32),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 81, 23, 67),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 81, 82, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 96, 24, 67),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 96, 83, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 102, 77, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 106, 22, 64),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 106, 78, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 44, 6),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 52, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 58, 7),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 24, 13, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 26, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 28, 13, 22),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 30, 13, 18),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 12, 34, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 134, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 26, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 58, 95),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 130, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 35, 109, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 26, 27),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 55, 70),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 134, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 118, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 105, 19),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 42, 48, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 26, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 58, 95),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 42, 54, 5),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 53, 96, 19),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 130, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 37, 17),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 118, 22),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 56, 18),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 60, 48, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 60, 54, 9),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 22, 37, 4),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 23, 33, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 23, 65, 31),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 25, 33, 27),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 25, 62, 28),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 27, 33, 30),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 27, 65, 31),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 110, 17),
        ("SCR0001", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 129, 20),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 54, 100, 1),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 55, 21, 288),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 56, 21, 284),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 57, 21, 463),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 58, 21, 287),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 59, 21, 281),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 60, 21, 463),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 61, 21, 293),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 62, 21, 462),
        ("SCR0007", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 27, 121, 362),
        ("SCR0008", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 24, 98, 18),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 26, 100, 1),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 27, 21, 460),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 39, 20, 16),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 39, 38, 8),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 58, 41, 6),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 46, 91, 7),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 49, 45, 6),
        ("SCR0001", "Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 49, 53, 4),
        ("SCR0006", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 118, 29, 22),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 95, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 96, 121, 419),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 97, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 98, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 99, 121, 185),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 100, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 101, 121, 361),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 102, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 103, 121, 191),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 104, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 105, 121, 188),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 106, 121, 195),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 107, 121, 185),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 108, 121, 192),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 109, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 110, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 111, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 112, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 118, 121, 9),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 214, 121, 16),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 92, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 95, 121, 98),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 96, 121, 95),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 97, 121, 96),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 98, 121, 94),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 99, 121, 92),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 92, 98, 18),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 36, 29, 26),
        ("SCR0001", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "41f2cfc74eb1073c1b0ac63fa9c12eef6e9d91db5dacd90b19f28a3e72e502ad", 218, 76, 7),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 65, 78, 20),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 39, 13, 20),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 69, 79, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 43, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 73, 79, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 47, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 77, 81, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 51, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 81, 81, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "b22fdfa6ae405da6d54b0db581e44af3fc871265c7345d387a6ae35c2a97d93c", 55, 13, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 13, 88, 20),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 14, 86, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 15, 87, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 16, 87, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 17, 85, 14),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 25, 13, 20),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 27, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 29, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 31, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "191ec98af11d741f95b4830c94ec4ebdc9a501c8f39b06329e69b21bc1852256", 33, 13, 14),
        ("SCR0006", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 103, 29, 22),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 83, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 84, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 85, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 86, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 87, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 88, 121, 361),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 89, 121, 371),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 90, 121, 369),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 91, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 92, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 93, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 94, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 95, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 96, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 97, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 103, 121, 9),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "aa9c0782127d727a8f2614eaf12cfd786781385d92b55c84999e650a49ce4e59", 80, 98, 18),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 52, 29, 26),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 140, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 143, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 144, 121, 98),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 145, 121, 90),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 146, 121, 95),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 147, 121, 96),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 148, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 149, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 150, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 151, 121, 90),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 55, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 59, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 63, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 67, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 71, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 75, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 79, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 83, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 87, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 97, 81, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 101, 81, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 105, 83, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 109, 78, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 113, 79, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 117, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 121, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 125, 81, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "d55732652d9284086f786ab1d62c7213979e70983d47ca7c334ba148f609889d", 129, 81, 12),
        ("SCR0007", "Godot.SourceGenerators.ScriptPathAttributeGenerator/"
                + "AssemblyScriptTypes.generated.cs",
            "621c1e81abf2b4d2384f78c56e3c865ad780b952b814bab7ca6a56b896009846", 1, 121, 215),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 13, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 14, 90, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 15, 84, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 16, 85, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 17, 87, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 18, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 19, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 20, 84, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 21, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 29, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 31, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 33, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 35, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 37, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 39, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 41, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 43, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "41a61b43070a6ff619a475d3f9ace40a340b66c4f6b2a8f6c463d2cbd86cf615", 45, 13, 12),
        ("SCR0006", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 113, 29, 22),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 91, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 92, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 93, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 94, 121, 419),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 95, 121, 598),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 96, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 97, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 98, 121, 361),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 99, 121, 371),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 100, 121, 369),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 101, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 102, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 103, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 104, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 105, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 106, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 107, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 113, 121, 9),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 136, 121, 54),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 88, 98, 18),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 164, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 167, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 168, 121, 98),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 169, 121, 90),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 170, 121, 95),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 171, 121, 99),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 172, 121, 94),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 173, 121, 96),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 174, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 175, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 176, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 177, 121, 90),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 60, 29, 26),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 63, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 67, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 71, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 75, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 79, 13, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 83, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 87, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 91, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 95, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 99, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 103, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 113, 81, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 117, 81, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 121, 83, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 125, 78, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 129, 78, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 133, 79, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 137, 79, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 141, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 145, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 149, 81, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 153, 81, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 13, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 14, 90, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 15, 84, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 16, 85, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 17, 89, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 18, 85, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 19, 87, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 20, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 21, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 22, 84, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 23, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 31, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 33, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 35, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 37, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 39, 13, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 41, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 43, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 45, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 47, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 49, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 51, 13, 12),
        ("SCR0008", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 88, 98, 18),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 91, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 92, 121, 365),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 93, 121, 189),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 94, 121, 419),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 95, 121, 598),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 96, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 97, 121, 186),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 98, 121, 361),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 99, 121, 371),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 100, 121, 369),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 101, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 102, 121, 363),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 103, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 104, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 105, 121, 187),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 106, 121, 184),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 107, 121, 189),
        ("SCR0006", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 113, 29, 22),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 113, 121, 9),
        ("SCR0007", "Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptMethods.generated.cs",
            "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697", 136, 121, 54),
        ("SCR0008", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 60, 29, 26),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 63, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 67, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 71, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 75, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 79, 13, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 83, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 87, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 91, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 95, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 99, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 103, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 113, 81, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 117, 81, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 121, 83, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 125, 78, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 129, 78, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 133, 79, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 137, 79, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 141, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 145, 81, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 149, 81, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 153, 81, 12),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 164, 121, 1),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 167, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 168, 121, 98),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 169, 121, 90),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 170, 121, 95),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 171, 121, 99),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 172, 121, 94),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 173, 121, 96),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 174, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 175, 121, 93),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 176, 121, 91),
        ("SCR0007", "Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptProperties.generated.cs",
            "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b", 177, 121, 90),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 13, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 14, 90, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 15, 84, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 16, 85, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 17, 89, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 18, 85, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 19, 87, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 20, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 21, 86, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 22, 84, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 23, 83, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 31, 13, 12),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 33, 13, 19),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 35, 13, 11),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 37, 13, 17),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 39, 13, 21),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 41, 13, 16),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 43, 13, 18),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 45, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 47, 13, 15),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 49, 13, 13),
        ("SCR0011", "Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Inspection.InspectionRoot_ScriptSerialization.generated.cs",
            "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d", 51, 13, 12),
    ];
}
