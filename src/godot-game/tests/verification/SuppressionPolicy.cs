using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Closes non-C# suppression entry points while retaining exact existing source permissions.</summary>
internal static class SuppressionPolicy
{
    /// <summary>Checks tracked policy files alongside the semantic C# and editor configuration scan.</summary>
    internal static IReadOnlyList<string> CheckRepository()
    {
        var errors = new List<string>(collection: ExemptionValidator.CheckRepository());
        foreach (string path in SourceFileFormat.ReadTrackedPaths().Where(predicate: SourceFileFormat.IsChecked))
        {
            if (IsBuild(path: path) || path.EndsWith(value: ".rsp", comparisonType: StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(value: ".yml", comparisonType: StringComparison.OrdinalIgnoreCase)
                    || path.EndsWith(value: ".yaml", comparisonType: StringComparison.OrdinalIgnoreCase))
            {
                errors.AddRange(collection: CheckConfiguration(path: path, content: File.ReadAllText(
                        path: Path.Combine(path1: ProofProtocol.Root, path2: path))));
            }
        }

        return errors;
    }

    /// <summary>Parses MSBuild as XML; response/workflow commands cannot downgrade required policy.</summary>
    internal static IReadOnlyList<string> CheckConfiguration(string path, string content)
    {
        var errors = new List<string>();
        if (IsBuild(path: path))
        {
            var document = XDocument.Parse(text: content, options: LoadOptions.SetLineInfo);
            foreach (XElement element in document.Descendants())
            {
                string key = element.Name.LocalName;
                bool disabled = _enabled.Contains(value: key, comparer: StringComparer.OrdinalIgnoreCase)
                        && !StringComparer.OrdinalIgnoreCase.Equals(x: element.Value.Trim(), y: "true");
                bool suppressed = (key.Equals(value: "NoWarn", comparisonType: StringComparison.OrdinalIgnoreCase)
                        || key.Equals(value: "WarningsNotAsErrors", comparisonType: StringComparison.OrdinalIgnoreCase))
                        && !string.IsNullOrWhiteSpace(value: element.Value);
                bool removed = key.Equals(value: "Analyzer", comparisonType: StringComparison.OrdinalIgnoreCase)
                        && element.Attribute(name: "Remove") is not null;
                if (disabled || suppressed || removed)
                {
                    int line = ((System.Xml.IXmlLineInfo)element).LineNumber;
                    errors.Add(item: "SUPPRESSION_BUILD " + path + ":" + line.ToString(
                            provider: System.Globalization.CultureInfo.InvariantCulture) + " " + key);
                }
            }
        }
        else
        {
            string[] lines = content.Split(separator: '\n');
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index].Trim();
                if (!line.StartsWith(value: '#') && Regex.IsMatch(input: line,
                        pattern: @"(?:[/\-](?:nowarn|warnaserror-)|(?:NoWarn|WarningsNotAsErrors)\s*[:=]"
                                + @"|(?:RunAnalyzers|EnableNETAnalyzers|TreatWarningsAsErrors|EnforceCodeStyleInBuild)"
                                + @"\s*[:=]\s*(?:false|0))", options: RegexOptions.IgnoreCase
                                        | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                        matchTimeout: TimeSpan.FromSeconds(value: 1)))
                {
                    errors.Add(item: "SUPPRESSION_COMMAND " + path + ":" + (index + 1).ToString(
                            provider: System.Globalization.CultureInfo.InvariantCulture));
                }
            }
        }

        return errors;
    }

    private static readonly string[] _enabled = ["RunAnalyzers", "RunAnalyzersDuringBuild", "EnableNETAnalyzers",
        "TreatWarningsAsErrors", "MSBuildTreatWarningsAsErrors", "CodeAnalysisTreatWarningsAsErrors",
        "EnforceCodeStyleInBuild", "ImportDirectoryBuildProps", "ImportDirectoryBuildTargets"];
    private static bool IsBuild(string path)
    {
        return new[] { ".csproj", ".props", ".targets" }.Contains(value: Path.GetExtension(path: path),
                comparer: StringComparer.OrdinalIgnoreCase);
    }
}
