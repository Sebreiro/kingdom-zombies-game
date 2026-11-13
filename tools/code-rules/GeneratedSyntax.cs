using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using SyntaxPermission = (string Diagnostic, string Suffix, string Hash, int Line, int Column, int Length);

namespace Sebreiro.CodeRules.Analyzers;
/// <summary>Permits only the exact two observed Godot 4.7.2 syntax spans, including immutable content
/// identity.</summary>
internal static class GeneratedSyntax
{
    internal static bool IsPermitted(string diagnostic, SyntaxTree tree, TextSpan span)
    {
        string path = tree.FilePath.Replace(oldChar: '\\', newChar: '/');
        SourceText text = tree.GetText();
        LinePosition position = text.Lines.GetLinePosition(position: span.Start);
        foreach ((string Diagnostic, string Suffix, string Hash, int Line, int Column, int Length) entry in _spans)
        {
            if (!StringComparer.Ordinal.Equals(x: diagnostic, y: entry.Diagnostic) || !path.EndsWith(
                    value: entry.Suffix, comparisonType: StringComparison.Ordinal) || position.Line + 1 != entry
                            .Line || position.Character + 1 != entry.Column || span.Length != entry.Length)
            {
                continue;
            }

            using var sha = SHA256.Create();
            string hash = BitConverter.ToString(value: sha.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: text
                    .ToString()))).Replace(oldValue: "-", newValue: "").ToLowerInvariant();
            return StringComparer.Ordinal.Equals(x: hash, y: entry.Hash);
        }

        return false;
    }

    private static readonly SyntaxPermission[] _spans =
    [
        ("SCR0009", "/Godot.SourceGenerators.GodotPluginsInitializerGenerator/GodotPlugins.Game.generated.cs",
            "50df9e26e6d80ac8d94e8881618c71803e1f98a565e3e8e20c03479911792c66", 12, 13, 6),
        ("SCR0011", "/Godot.SourceGenerators.ScriptPropertiesGenerator/"
                + "KingdomZombies.Proofs.Phase1_ScriptProperties.generated.cs",
            "0096a903e684ab047368a57a308d2274f4c39899574b0177d3363fee3147ab40", 23, 81, 12)
    ];
}
