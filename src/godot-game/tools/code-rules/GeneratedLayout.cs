using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sebreiro.CodeRules.Analyzers;
/// <summary>Exact pinned assembly-attribute source lines; all adjacent or modified text is checked.</summary>
internal static class GeneratedLayout
{
    internal static bool IsPermitted(SyntaxTree tree, SourceText text, TextLine line)
    {
        string path = tree.FilePath.Replace(oldChar: '\\', newChar: '/').Replace(
                oldValue: "/buildTransitive/../", newValue: "/");
        foreach ((string Suffix, string Hash, int[] Lines) entry in _entries)
        {
            if (!path.EndsWith(value: entry.Suffix, comparisonType: StringComparison.Ordinal) || !entry.Lines
                    .Contains(value: line.LineNumber + 1))
            {
                continue;
            }

            using var sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(buffer: Encoding.UTF8.GetBytes(s: text.ToString()));
            string hash = BitConverter.ToString(value: bytes).Replace(oldValue: "-", newValue: "").ToLowerInvariant();
            if (StringComparer.Ordinal.Equals(x: hash, y: entry.Hash))
            {
                return true;
            }
        }

        return false;
    }

    private static readonly (string Suffix, string Hash, int[] Lines)[] _entries =
    [
        ("/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/obj/Debug/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/obj/Debug/netstandard2.0/.NETStandard,Version=v2.0.AssemblyAttributes.cs",
            "024d53b28a9ade66887280d5dc398a1e1b10c3172df3573f01c67fda2d7b673b", [4]),
        ("/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "f79c1ad9a9fc76110b3655f5bfb5ff4e3168e9addceff05b937c0796f0ec34eb", [4]),
        ("/obj/Debug/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "f79c1ad9a9fc76110b3655f5bfb5ff4e3168e9addceff05b937c0796f0ec34eb", [4]),
        ("/obj/Debug/netstandard2.0/.NETStandard,Version=v2.0.AssemblyAttributes.cs",
            "7300a46b63ecd250f168f446b4864848453c4b655fb6f317d1530d7ca963ebc5", [4]),
        ("/src/godot-game/core/obj/ExportDebug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/src/godot-game/game/.godot/mono/temp/obj/ExportDebug/win-x64/"
            + ".NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/src/godot-game/multiplayer-abstractions/obj/ExportDebug/net10.0/"
            + ".NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/src/godot-game/core/obj/ExportRelease/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/src/godot-game/game/.godot/mono/temp/obj/ExportRelease/win-x64/"
            + ".NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/src/godot-game/multiplayer-abstractions/obj/ExportRelease/net10.0/"
            + ".NETCoreApp,Version=v10.0.AssemblyAttributes.cs",
            "93d67476196ebfc80d1d7d8844b91146cfd678d105f5ce635eb900ca40610c9b", [4]),
        ("/xunit.v3.core.mtp-v2/4.0.2/_content/DefaultRunnerReporters.cs",
            "e110bb9fe21ca6c3120a00382c773a086d62a008a0b0d2b76d08416f27d92bf2", [4]),
        ("/xunit.v3.core.mtp-v2/4.0.2/_content/DefaultResultWriters.cs",
            "206a1dd2d92f05c8c977f2a86bc283fbbf519e69abf17808be1daee9a2642fe3", [3, 4, 5, 6, 7, 9, 10])
    ];
}
