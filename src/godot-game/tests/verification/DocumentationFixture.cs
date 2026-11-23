using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Creates isolated authored-document subjects and preserves exact byte identity observations.</summary>
internal static class DocumentationFixture
{
    /// <summary>Returns a real singleton index linked to one required nonempty document.</summary>
    internal static Dictionary<string, byte[]> Create(string link = "docs/subject.md#subject")
    {
        return new Dictionary<string, byte[]>(comparer: StringComparer.Ordinal)
        {
            ["AGENTS.md"] = Encoding.UTF8.GetBytes(s: "# Guide\n[Subject](" + link + ")\n"),
            ["docs/subject.md"] = Encoding.UTF8.GetBytes(s: "# Subject\nLive content.\n"),
        };
    }

    /// <summary>Reads the actual Git index bytes for non-mutation proof without staging or refreshing it.</summary>
    internal static byte[] IndexBytes()
    {
        (int Exit, string Output) index = GraphFixture.RunAt(root: ProofProtocol.Root, executable: "git",
                arguments: ["rev-parse", "--path-format=absolute", "--git-path", "index"]);
        if (index.Exit != 0)
        {
            throw new InvalidDataException(message: "DOC_INDEX_PATH: " + index.Output);
        }

        return File.ReadAllBytes(path: index.Output.Trim());
    }

    /// <summary>Computes independent exact source byte hashes in ordinal path order.</summary>
    internal static string Hash(IReadOnlyDictionary<string, byte[]> files)
    {
        return string.Join(separator: "\n", values: files.OrderBy(keySelector: row => row.Key,
                comparer: StringComparer.Ordinal).Select(selector: row => row.Key + ":" + Convert.ToHexString(
                        inArray: SHA256.HashData(source: row.Value))));
    }

    /// <summary>Materializes a subject under ignored workspace output for host-filesystem mutation checks.</summary>
    internal static string Write(IReadOnlyDictionary<string, byte[]> files)
    {
        string root = WorkspacePaths.Temp(purpose: "docs-" + Guid.NewGuid());
        foreach ((string path, byte[] content) in files)
        {
            string target = Path.Combine(path1: root, path2: path);
            Directory.CreateDirectory(path: Path.GetDirectoryName(path: target)
                    ?? throw new InvalidOperationException(message: "DOC_FIXTURE_PARENT"));
            File.WriteAllBytes(path: target, bytes: content);
        }

        return root;
    }
}
