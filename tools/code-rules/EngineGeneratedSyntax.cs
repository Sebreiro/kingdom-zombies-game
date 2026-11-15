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
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 63, 121, 186),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 64, 121, 419),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 65, 121, 365),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 66, 121, 189),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 67, 121, 185),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 68, 121, 192),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 69, 121, 184),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 70, 121, 189),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 71, 121, 187),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 72, 121, 187),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 78, 121, 9),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 80, 121, 1),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 83, 121, 95),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 84, 121, 96),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 85, 121, 94),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 86, 121, 92),
        ("SCR0008", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptMethods.generated.cs",
            "a1c1f954f367fc450c19f93e5d7bdac4b05c1777959ded816ccbd9947d6800bc", 60, 98, 18),
        ("SCR0008", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 32, 29, 26),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 35, 13, 17),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 39, 13, 18),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 43, 13, 16),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 47, 13, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 57, 79, 17),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 61, 79, 18),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 65, 81, 16),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptProperties.generated.cs",
            "fee3e8579886b224823e25c3f5be675dc7ac7169a6781869d82ce62a2a208b5b", 69, 81, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 13, 86, 17),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 14, 87, 18),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 15, 87, 16),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 16, 85, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 24, 13, 17),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 26, 13, 18),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 28, 13, 16),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Foundation.FoundationRoot_ScriptSerialization.generated.cs",
            "5ead67f658245d7689a16ed1486d37b69337d245f55223910d005443c5c25f54", 30, 13, 14),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 55, 121, 190),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 56, 121, 186),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 57, 121, 365),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 58, 121, 189),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 59, 121, 183),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 60, 121, 365),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 61, 121, 195),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 62, 121, 364),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 68, 121, 9),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 27, 121, 184),
        ("SCR0007", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 33, 121, 9),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPathAttributeGenerator/"
                + "AssemblyScriptTypes.generated.cs",
            "34e0126fbdedd5d9fd26a9768b2d0903161fb87d5149e7be0ed0cbe786661884", 1, 121, 157),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 121, 20),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 121, 20),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 121, 21),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 121, 21),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 68, 121, 1),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 71, 121, 109),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 72, 121, 98),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 73, 121, 109),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 56, 121, 1),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 59, 121, 91),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 60, 121, 89),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 14, 121, 30),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 16, 121, 3),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 121, 38),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 121, 10),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 121, 38),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 14, 121, 30),
        ("SCR0007", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 16, 121, 3),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 121, 34),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 121, 6),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 121, 34),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 121, 30),
        ("SCR0008", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 24, 98, 18),
        ("SCR0008", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 52, 98, 18),
        ("SCR0008", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 28, 29, 26),
        ("SCR0008", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 24, 29, 26),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 26, 100, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 27, 21, 282),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 27, 13, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 31, 13, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 49, 41, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 37, 76, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 44, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 52, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptMethods.generated.cs",
            "295feec981d5667ee1b8da9679f773291027e19402592e9c039d820b20cc6891", 40, 58, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 27, 89, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 31, 84, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 34, 48, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 34, 54, 5),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 41, 81, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 45, 78, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 41, 81, 14),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 45, 78, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 48, 48, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 48, 54, 9),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 59, 24, 186),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptProperties.generated.cs",
            "87c6e236b588682e3913571028bd0beb693bd237c495f812892fda0d90ff7ee1", 60, 24, 184),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 16, 121, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 20, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 44, 60),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 85, 14),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 18, 79, 24),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 20, 20),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 42, 55),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 80, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptPropertyDefVal.generated.cs",
            "7887db50a80663e5d7277602e84394d6ed2e82d8370362dd781db79afe310c4b", 20, 74, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 22, 13, 14),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 24, 13, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 12, 34, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 26, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 50, 50),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 13, 85, 14),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 26, 20),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 48, 45),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 14, 80, 12),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 20, 37, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 21, 33, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 21, 57, 23),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 23, 33, 20),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.ProofResource_ScriptSerialization.generated.cs",
            "64ccc9f46f71c1de2f0027840c39cc718a408877b4327198e39e42ee0ff51e1b", 23, 55, 21),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 13, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 118, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 35, 13, 19),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 53, 96, 19),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 13, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 118, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 140, 41, 6),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 130, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 105, 19),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 130, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 56, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 71, 24, 204),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 72, 24, 193),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 73, 24, 204),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 16, 121, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 20, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 52, 105),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 18, 124, 32),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 20, 27),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 49, 80),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 20, 99, 29),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 20, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 52, 105),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertyDefValGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptPropertyDefVal.generated.cs",
            "bbd1e0627cb698c6339ea36934a2c9f1670fecd8f6f5e19c4da8d983500d4bfc", 22, 124, 32),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 81, 23, 67),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 81, 82, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 96, 24, 67),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 96, 83, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 102, 77, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 106, 22, 64),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 106, 78, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 44, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 52, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 110, 58, 7),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 24, 13, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 26, 13, 19),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 28, 13, 22),
        ("SCR0011", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 30, 13, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 12, 34, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 31, 134, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 26, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 58, 95),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 13, 130, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 35, 109, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 26, 27),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 55, 70),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 39, 134, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 49, 118, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 14, 105, 19),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 42, 48, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 26, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 58, 95),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 42, 54, 5),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 53, 96, 19),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 15, 130, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 37, 17),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 57, 118, 22),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 16, 56, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 60, 48, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptProperties.generated.cs",
            "e8950dc0bebcf0bfc25ec2075fb0eaf3ece62bd69216d433800a890e45b775e5", 60, 54, 9),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 22, 37, 4),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 23, 33, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 23, 65, 31),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 25, 33, 27),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 25, 62, 28),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 27, 33, 30),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 27, 65, 31),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 110, 17),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSerializationGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSerialization.generated.cs",
            "5a6533d898bdcce42cdd192e90bb4bfe90d60d7ae0d63b8bdab1ab6bf0484f64", 29, 129, 20),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 54, 100, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 55, 21, 288),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 56, 21, 284),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 57, 21, 463),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 58, 21, 287),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 59, 21, 281),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 60, 21, 463),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 61, 21, 293),
        ("SCR0001", "/Godot.SourceGenerators.ScriptMethodsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptMethods.generated.cs",
            "5bb4d4f1584b5d6bf9366ef6aa87df086899e54beefce1181bf58f7ef1ec245b", 62, 21, 462),
        ("SCR0007", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 27, 121, 362),
        ("SCR0008", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 24, 98, 18),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 26, 100, 1),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 27, 21, 460),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 39, 20, 16),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 39, 38, 8),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 58, 41, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 46, 91, 7),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 49, 45, 6),
        ("SCR0001", "/Godot.SourceGenerators.ScriptSignalsGenerator/"
                + "KingdomZombies.Proofs.LifecycleFixture_ScriptSignals.generated.cs",
            "16d37d09d65b6fea642fad98c45c0d7812454064932d800f79c28e5841ec3d7d", 49, 53, 4)
    ];
}
