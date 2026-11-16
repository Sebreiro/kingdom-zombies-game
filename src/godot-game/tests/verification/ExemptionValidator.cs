using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Rejects undeclared suppression and generated-origin claims in owned source and effective policy.</summary>
internal static class ExemptionValidator
{
    /// <summary>Checks syntax and semantic suppression identities against exact observed member permissions.</summary>
    internal static IReadOnlyList<string> CheckSource(string path, string content,
            IReadOnlySet<string>? permissions = null, ConsumerCompilation? consumer = null)
    {
        consumer ??= ConsumerCompilation.Read(root: ProofProtocol.Root, project: SourceProject(path: path));
        SyntaxTree tree = consumer.Parse(source: content, path: path);
        SyntaxNode root = tree.GetRoot();
        var errors = new List<string>();
        SemanticModel model = CSharpCompilation.Create(assemblyName: "SuppressionScan", syntaxTrees: [tree],
                references: consumer.References).GetSemanticModel(syntaxTree: tree);
        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (SyntaxTrivia trivia in root.DescendantTrivia(descendIntoTrivia: true))
        {
            CheckTrivia(path: path, trivia: trivia, errors: errors);
        }

        foreach (AttributeSyntax attribute in root.DescendantNodes().OfType<AttributeSyntax>())
        {
            CheckAttribute(path: path, attribute: attribute, model: model, permissions: permissions,
                    seen: seen, errors: errors);
        }

        if (_generatedSuffixes.Any(predicate: suffix => path.EndsWith(value: suffix,
                comparisonType: StringComparison.OrdinalIgnoreCase)) || Path.GetFileName(path: path).StartsWith(
                        value: "TemporaryGeneratedFile_", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(item: "GENERATION_UNPROVEN " + path + "(1,1) generated filename");
        }

        return errors;
    }

    /// <summary>Checks every actual owned tracked source against the exact registered member permissions.</summary>
    internal static IReadOnlyList<string> CheckRepository()
    {
        IReadOnlyList<string> registryErrors = RegisterValidator.Validate(
                manifest: RegisterValidator.Read(relative: "docs/code/provenance/manifest.json"),
                register: RegisterValidator.Read(relative: "docs/code/register.json"),
                exemptions: RegisterValidator.Read(relative: "docs/code/exemptions.json"));
        Require(value: registryErrors.Count == 0, message: string.Join(separator: "\n", values: registryErrors));
        IReadOnlySet<string> permissions = ReadPermissions();
        var errors = new List<string>();
        ConsumerCompilation[] consumers = [..ProjectEvaluator.Expected.Select(selector: project =>
                ConsumerCompilation.Read(root: ProofProtocol.Root, project: project))];
        foreach (string path in SourceFileFormat.ReadTrackedPaths().Where(predicate: SourceFileFormat.IsChecked))
        {
            string content = File.ReadAllText(path: Path.Combine(path1: ProofProtocol.Root, path2: path));
            if (path.EndsWith(value: ".cs", comparisonType: StringComparison.Ordinal))
            {
                ConsumerCompilation[] owners = [..consumers.Where(predicate: consumer => consumer.AuthoredPaths
                        .Contains(value: path, comparer: StringComparer.Ordinal))];
                errors.AddRange(collection: CheckOwnedSource(path: path, content: content,
                        permissions: permissions, owners: owners));
            }
            else if (path.EndsWith(value: ".editorconfig", comparisonType: StringComparison.Ordinal)
                    || path.EndsWith(value: ".globalconfig", comparisonType: StringComparison.Ordinal))
            {
                errors.AddRange(collection: CheckEditorConfig(path: path, content: content,
                        requirePermissions: StringComparer.Ordinal.Equals(x: path, y: ".editorconfig")));
            }
        }

        return errors;
    }

    /// <summary>Checks shared authored bytes in every declared actual compiler context.</summary>
    internal static IReadOnlyList<string> CheckOwnedSource(string path, string content,
            IReadOnlySet<string>? permissions, IReadOnlyList<ConsumerCompilation> owners)
    {
        RequireOwners(path: path, projects: [.. owners.Select(selector: owner => owner.Project)]);
        return [..owners.SelectMany(selector: owner => CheckSource(path: path, content: content,
                permissions: permissions, consumer: owner))];
    }

    /// <summary>Rejects missing, duplicate or undeclared compiler ownership, including shared-source drift.</summary>
    internal static void RequireOwners(string path, IReadOnlyList<string> projects)
    {
        string[] expected = FoundationLinkedSources.Paths.Contains(value: path, comparer: StringComparer.Ordinal)
                ? [SourceProject(path: path), "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj"]
                : [SourceProject(path: path)];
        Require(value: projects.Count == expected.Length && projects.Order(comparer: StringComparer.Ordinal)
                .SequenceEqual(second: expected.Order(comparer: StringComparer.Ordinal),
                        comparer: StringComparer.Ordinal), message: "SUPPRESSION_SOURCE_OWNERSHIP " + path);
    }

    /// <summary>Matches each active donor counterpart to one exact local setting or project edge.</summary>
    internal static bool IsObservedLocalPermission(JsonNode entry, string id)
    {
        return _localPermissions.TryGetValue(key: id, value: out string[]? expected) && _localFields
                .Select(selector: field => Text(row: entry, name: field)).SequenceEqual(second: expected,
                        comparer: StringComparer.Ordinal);
    }

    /// <summary>Lists every exact active local scope, including the five observed lock paths.</summary>
    internal static IEnumerable<string> ExpectedLocalIds()
    {
        return _localPermissions.Keys.Concat(second: SourceFileFormat.LockExemptions.Keys);
    }

    /// <summary>Names independent actual controls for non-rule project bootstrap permissions.</summary>
    internal static (string Positive, string Negative) LocalProofs(string id, string rule)
    {
        return id switch
        {
            "KZ-EX-TEST-EXE-01" => ("rules.exemption-executable-positive", "rules.exemption-executable-negative"),
            "KZ-EX-ANALYZER-BOOTSTRAP-01" => ("rules.analyzer-source-consumer", "rules.analyzer-source-boundary"),
            _ => (rule + ".positive", rule + ".negative")
        };
    }

    /// <summary>Only the exact named proof project with IsTestProject true receives executable permission.</summary>
    internal static IReadOnlyList<string> CheckExecutableProperties(JsonObject properties, string project)
    {
        bool tests = StringComparer.Ordinal.Equals(x: project,
                y: "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj");
        string output = properties["OutputType"]?.GetValue<string>() ?? "";
        bool testShape = StringComparer.Ordinal.Equals(x: output, y: "Exe")
                && StringComparer.Ordinal.Equals(x: properties["IsTestProject"]?.GetValue<string>(), y: "true")
                && StringComparer.Ordinal.Equals(x: properties["MSBuildProjectName"]?.GetValue<string>(),
                        y: "KingdomZombies.Phase1.Tests");
        bool libraryShape = StringComparer.Ordinal.Equals(x: output, y: "Library")
                && !StringComparer.Ordinal.Equals(x: properties["IsTestProject"]?.GetValue<string>(), y: "true");
        return (tests ? testShape : libraryShape) ? [] : ["EXEMPTION_EXECUTABLE_SCOPE " + project];
    }

    /// <summary>Requires declared non-error severities exactly once, and rejects any additional weakening.</summary>
    internal static IReadOnlyList<string> CheckEditorConfig(string path, string content,
            bool requirePermissions = false)
    {
        var errors = new List<string>();
        var observed = new HashSet<string>(comparer: StringComparer.Ordinal);
        var settings = new HashSet<string>(comparer: StringComparer.OrdinalIgnoreCase);
        string section = "";
        string[] lines = content.Split(separator: '\n');
        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index].Trim();
            if (line.Length == 0 || line[0] is '#' or ';')
            {
                continue;
            }

            if (line[0] == '[' && line[^1] == ']')
            {
                section = line;
                continue;
            }

            int equals = line.IndexOf(value: '=');
            if (equals < 0)
            {
                continue;
            }

            string key = line[..equals].Trim().ToLowerInvariant();
            string value = line[(equals + 1)..].Trim();
            CheckSetting(path: path, line: index + 1, section: section, key: key, value: value,
                    settings: settings, observed: observed, errors: errors);
        }

        if (requirePermissions && !observed.SetEquals(other: _severityPermissions))
        {
            errors.Add(item: "SUPPRESSION_POLICY_MISSING " + path + " declared severity permissions");
        }

        return errors;
    }

    /// <summary>Checks effective SDK suppression properties after evaluation, including imported overrides.</summary>
    internal static IReadOnlyList<string> CheckEffectiveProperties(JsonObject properties, string project)
    {
        var errors = new List<string>();
        string[] warnings = (properties["NoWarn"]?.GetValue<string>() ?? "").Split(separator: ';',
                options: StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var unique = new HashSet<string>(collection: warnings, comparer: StringComparer.Ordinal);
        if (warnings.Length != unique.Count || !unique.SetEquals(other: _sdkNoWarn))
        {
            errors.Add(item: "SUPPRESSION_MSBUILD " + project + " NoWarn");
        }

        foreach (string name in _trueOrDefault)
        {
            string value = properties[name]?.GetValue<string>() ?? "missing";
            if (value is not ("" or "true"))
            {
                errors.Add(item: "SUPPRESSION_MSBUILD " + project + " " + name);
            }
        }

        if (!StringComparer.Ordinal.Equals(x: properties["WarningsNotAsErrors"]?.GetValue<string>(), y: ""))
        {
            errors.Add(item: "SUPPRESSION_MSBUILD " + project + " WarningsNotAsErrors");
        }

        if (!StringComparer.Ordinal.Equals(x: properties["EnableNETAnalyzers"]?.GetValue<string>(), y: "true"))
        {
            errors.Add(item: "SUPPRESSION_MSBUILD " + project + " EnableNETAnalyzers");
        }

        return errors;
    }

    /// <summary>Binds eighteen observed attributes to exact producer source and package bytes.</summary>
    internal static IReadOnlySet<string> ReadPermissions()
    {
        var permissions = new HashSet<string>(comparer: StringComparer.Ordinal);
        AddRecords(relative: "docs/code/generated-callsite-adaptations.json", collection: "local", count: 18,
                permissions: permissions);
        AddRecords(relative: "docs/code/generated-layout-adaptations.json", collection: "members", count: 7,
                permissions: permissions);
        AddRecords(relative: "docs/code/engine-adaptations.json", collection: "local", count: 1,
                permissions: permissions);
        Require(value: permissions.Count == 26, message: "SUPPRESSION_TOTAL_COUNT");
        return permissions;
    }

    private static readonly string[] _sdkNoWarn = ["1701", "1702"];
    private static readonly string[] _generatedSuffixes = [".g.cs", ".g.i.cs", ".designer.cs", ".generated.cs"];
    private static readonly string[] _trueOrDefault = ["RunAnalyzers", "RunAnalyzersDuringBuild",
        "CodeAnalysisTreatWarningsAsErrors"];
    private static readonly string[] _severityPermissions = ["[*.cs]|dotnet_diagnostic.ma0004.severity|none",
        "[*.cs]|dotnet_diagnostic.ma0051.severity|none", "[*.cs]|dotnet_diagnostic.ma0137.severity|none",
        "[*.cs]|dotnet_diagnostic.ma0156.severity|none",
        "[src/godot-game/tests/verification/**.cs]|dotnet_diagnostic.ca1707.severity|none"];
    private static readonly string[] _localFields = ["sourceId", "rule", "diagnostic", "path", "surface", "subject"];
    private static readonly Dictionary<string, string[]> _localPermissions = new(comparer: StringComparer.Ordinal)
    {
        ["KZ-EX-BODY-LENGTH-01"] = ["EX-01", "R-37", "MA0051", ".editorconfig", "exact editorconfig setting",
            "[*.cs] dotnet_diagnostic.MA0051.severity = none"],
        ["KZ-EX-ENGINE-AWAIT-01"] = ["EX-02", "R-45", "MA0004", ".editorconfig", "exact editorconfig setting",
            "[*.cs] dotnet_diagnostic.MA0004.severity = none; [src/{core,multiplayer-abstractions}/**.cs] error"],
        ["KZ-EX-TEST-NAMES-01"] = ["EX-03", "R-48", "CA1707", ".editorconfig", "exact editorconfig setting",
            "[src/godot-game/tests/verification/**.cs] dotnet_diagnostic.CA1707.severity = none"],
        ["KZ-EX-TEST-EXE-01"] = ["EX-13", "R-52", "PROJECT_OUTPUT_EXE",
            "src/godot-game/tests/verification/KingdomZombies.Phase1.Tests.csproj", "exact project executable permission",
            "KingdomZombies.Phase1.Tests; IsTestProject=true; OutputType=Exe"],
        ["KZ-EX-ANALYZER-BOOTSTRAP-01"] = ["EX-19", "R-54", "ANALYZER_BOOTSTRAP_EDGE", "src/godot-game/Directory.Build.targets",
            "exact analyzer bootstrap edge",
            "MSBuildProjectName != Sebreiro.CodeRules.Analyzers; OutputItemType=Analyzer; "
                    + "ReferenceOutputAssembly=false"]
    };

    private static string SourceProject(string path)
    {
        return ProjectEvaluator.Expected.FirstOrDefault(predicate: project => path.StartsWith(value:
                (Path.GetDirectoryName(path: project) ?? "").Replace(oldChar: '\\', newChar: '/') + "/",
                comparisonType: StringComparison.Ordinal)) ?? ProjectEvaluator.Expected[4];
    }

    private static void CheckTrivia(string path, SyntaxTrivia trivia, List<string> errors)
    {
        if (trivia.GetStructure() is PragmaWarningDirectiveTriviaSyntax pragma
                && pragma.DisableOrRestoreKeyword.IsKind(kind: SyntaxKind.DisableKeyword))
        {
            string codes = pragma.ErrorCodes.Count == 0 ? "all" : string.Join(separator: ",",
                    values: pragma.ErrorCodes.Select(selector: code => code.ToString()));
            errors.Add(item: At(path: path, location: pragma.GetLocation(), diagnostic: "SUPPRESSION_UNREGISTERED")
                    + " pragma " + codes);
        }

        if (trivia.GetStructure() is NullableDirectiveTriviaSyntax nullable
                && nullable.SettingToken.IsKind(kind: SyntaxKind.DisableKeyword))
        {
            errors.Add(item: At(path: path, location: nullable.GetLocation(), diagnostic: "SUPPRESSION_UNREGISTERED")
                    + " nullable disable");
        }

        if ((trivia.IsKind(kind: SyntaxKind.SingleLineCommentTrivia)
                || trivia.IsKind(kind: SyntaxKind.MultiLineCommentTrivia)) && trivia.ToString().Contains(
                        value: "auto-generated", comparisonType: StringComparison.OrdinalIgnoreCase)
                || (trivia.IsKind(kind: SyntaxKind.SingleLineCommentTrivia)
                        || trivia.IsKind(kind: SyntaxKind.MultiLineCommentTrivia)) && trivia.ToString().Contains(
                                value: "autogenerated", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(item: At(path: path, location: trivia.GetLocation(), diagnostic: "GENERATION_UNPROVEN")
                    + " generated marker");
        }
    }

    private static void CheckAttribute(string path, AttributeSyntax attribute, SemanticModel model,
            IReadOnlySet<string>? permissions, HashSet<string> seen, List<string> errors)
    {
        string type = (model.GetSymbolInfo(node: attribute).Symbol as IMethodSymbol)?.ContainingType
                .ToDisplayString() ?? throw new InvalidDataException(message: "SUPPRESSION_ATTRIBUTE_UNRESOLVED "
                        + path + " " + attribute.Name);
        if (type.EndsWith(value: "GeneratedCodeAttribute", comparisonType: StringComparison.Ordinal))
        {
            errors.Add(item: At(path: path, location: attribute.GetLocation(), diagnostic: "GENERATION_UNPROVEN")
                    + " generated attribute");
        }

        if (!type.EndsWith(value: "SuppressMessageAttribute", comparisonType: StringComparison.Ordinal)
                && !type.EndsWith(value: "SuppressMessage", comparisonType: StringComparison.Ordinal))
        {
            return;
        }

        string diagnostic = ReadArgument(attribute: attribute, model: model, index: 1);
        string target = ReadArgument(attribute: attribute, model: model, name: "Target");
        string key = Permission(path: path, diagnostic: diagnostic, target: target);
        bool valid = StringComparer.Ordinal.Equals(x: type,
                y: "System.Diagnostics.CodeAnalysis.SuppressMessageAttribute")
                && attribute.Parent is AttributeListSyntax { Target.Identifier.ValueText: "assembly" }
                && StringComparer.Ordinal.Equals(x: ReadArgument(attribute: attribute, model: model, name: "Scope"),
                        y: "member") && ReadArgument(attribute: attribute, model: model,
                                name: "Justification").Length > 0
                && StringComparer.Ordinal.Equals(x: ReadArgument(attribute: attribute, model: model, index: 0),
                        y: diagnostic.StartsWith(value: "SCR", comparisonType: StringComparison.Ordinal)
                                ? "Sebreiro.CodeRules" : "ApiDesign") && (permissions?.Contains(item: key) ?? false);
        string code = !seen.Add(item: key) ? "SUPPRESSION_DUPLICATE" : "SUPPRESSION_UNREGISTERED";
        if (!valid || StringComparer.Ordinal.Equals(x: code, y: "SUPPRESSION_DUPLICATE"))
        {
            errors.Add(item: At(path: path, location: attribute.GetLocation(), diagnostic: code) + " " + diagnostic);
        }
    }

    private static string ReadArgument(AttributeSyntax attribute, SemanticModel model, string name = "", int index = -1)
    {
        AttributeArgumentSyntax? argument = index >= 0 ? attribute.ArgumentList?.Arguments.ElementAtOrDefault(
                index: index) : attribute.ArgumentList?.Arguments.FirstOrDefault(predicate: item => StringComparer
                        .Ordinal.Equals(x: item.NameEquals?.Name.Identifier.ValueText, y: name));
        return argument is not null && model.GetConstantValue(node: argument.Expression) is { HasValue: true } constant
                && constant.Value is string value ? value : "";
    }

    private static void CheckSetting(string path, int line, string section, string key, string value,
            HashSet<string> settings, HashSet<string> observed, List<string> errors)
    {
        bool severity = key.EndsWith(value: ".severity", comparisonType: StringComparison.Ordinal);
        bool generated = StringComparer.Ordinal.Equals(x: key, y: "generated_code")
                && StringComparer.OrdinalIgnoreCase.Equals(x: value, y: "true");
        bool style = value.Contains(value: ':') && !value.EndsWith(value: ":error", comparisonType: StringComparison
                .OrdinalIgnoreCase);
        if (!severity && !generated && !style)
        {
            return;
        }

        string setting = section + "|" + key;
        string permission = setting + "|" + value;
        bool allowed = _severityPermissions.Contains(value: permission, comparer: StringComparer.Ordinal)
                && StringComparer.Ordinal.Equals(x: path, y: ".editorconfig");
        bool duplicate = !settings.Add(item: setting);
        if (duplicate || generated || style || severity && !StringComparer.OrdinalIgnoreCase.Equals(
                x: value, y: "error")
                && !allowed)
        {
            errors.Add(item: "SUPPRESSION_CONFIG " + path + "(" + line.ToString(provider: CultureInfo.InvariantCulture)
                    + ",1) " + setting);
        }

        if (allowed)
        {
            observed.Add(item: permission);
        }
    }

    private static void AddRecords(string relative, string collection, int count, HashSet<string> permissions)
    {
        JsonArray rows = RegisterValidator.Read(relative: relative)[collection]?.AsArray()
                ?? throw new InvalidDataException(message: "SUPPRESSION_RECORDS_REQUIRED " + relative);
        Require(value: rows.Count == count, message: "SUPPRESSION_RECORD_COUNT " + relative);
        foreach (JsonNode? row in rows)
        {
            string path = Text(row: row, name: "path");
            string diagnostic = Text(row: row, name: "diagnostic");
            string target = Text(row: row, name: "target");
            Require(value: target.StartsWith(value: "~M:", comparisonType: StringComparison.Ordinal)
                    && target.IndexOfAny(anyOf: ['*', '?']) < 0, message: "SUPPRESSION_MEMBER_REQUIRED " + target);
            Require(value: permissions.Add(item: Permission(path: path, diagnostic: diagnostic, target: target)),
                    message: "SUPPRESSION_RECORD_OVERLAP " + target);
            string surface = Text(row: row, name: "surface");
            bool engine = row?["generatedSourceSha256"] is not null;
            if (engine)
            {
                surface = "src/godot-game/game/.godot/mono/temp/obj/Debug/generated/" + surface;
            }

            string hashName = OperatingSystem.IsLinux() && row?["linuxSha256"] is not null ? "linuxSha256"
                    : engine ? "generatedSourceSha256" : "sha256";
            CheckHash(relative: surface, expected: Text(row: row, name: hashName));
            string package = engine ? ".phase1/feed/Godot.SourceGenerators.4.7.2.nupkg"
                    : Text(row: row, name: "package");
            CheckHash(relative: package, expected: Text(row: row, name: "packageSha256"));
            Require(value: Text(row: row, name: "reason").Length > 0
                    && Text(row: row, name: "positiveProof").Length > 0
                    && Text(row: row, name: "negativeProof").Length > 0,
                    message: "SUPPRESSION_PROOFS_REQUIRED " + target);
        }
    }

    private static void CheckHash(string relative, string expected)
    {
        string file = Path.GetFullPath(path: Path.Combine(path1: ProofProtocol.Root, path2: relative));
        string local = Path.GetRelativePath(relativeTo: ProofProtocol.Root, path: file);
        Require(value: !Path.IsPathRooted(path: local) && !local.Split(separator: Path.DirectorySeparatorChar)
                .Any(predicate: part => part is ".." or ".") && File.Exists(path: file),
                message: "SUPPRESSION_ORIGIN_PATH " + relative);
        string hash = Convert.ToHexStringLower(inArray: System.Security.Cryptography.SHA256.HashData(
                source: File.ReadAllBytes(path: file)));
        Require(value: StringComparer.Ordinal.Equals(x: hash, y: expected),
                message: "SUPPRESSION_ORIGIN_HASH " + relative);
    }

    private static string Text(JsonNode? row, string name)
    {
        return row?[name]?.GetValue<string>() ?? throw new InvalidDataException(message: "SUPPRESSION_FIELD " + name);
    }

    private static string Permission(string path, string diagnostic, string target)
    {
        return path + "|" + diagnostic + "|member|" + target;
    }

    private static string At(string path, Location location, string diagnostic)
    {
        Microsoft.CodeAnalysis.Text.LinePosition start = location.GetLineSpan().StartLinePosition;
        return diagnostic + " " + path + "(" + (start.Line + 1).ToString(provider: CultureInfo.InvariantCulture)
                + "," + (start.Character + 1).ToString(provider: CultureInfo.InvariantCulture) + ")";
    }

    private static void Require(bool value, string message)
    {
        if (!value)
        {
            throw new InvalidDataException(message: message);
        }
    }
}
