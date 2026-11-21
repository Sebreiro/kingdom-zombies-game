using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using OriginPair = (string Old, string Target, string Surface, string Raw, string Text);

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
        CheckRepositoryRegister();
        IReadOnlySet<string> permissions = ReadPermissions();
        var errors = new List<string>();
        ConsumerCompilation[] consumers = [..ProjectEvaluator.Expected.Select(selector: project =>
                ConsumerCompilation.Read(root: ProofProtocol.Root, project: project))];
        foreach (string path in SourceFileFormat.ReadTrackedPaths().Where(predicate: SourceFileFormat.IsChecked))
        {
            errors.AddRange(collection: CheckRepositoryPath(path: path, permissions: permissions,
                    consumers: consumers));
        }

        return errors;
    }

    /// <summary>Retains the original repository register conjunction for whole and bounded execution.</summary>
    internal static void CheckRepositoryRegister()
    {
        IReadOnlyList<string> registryErrors = RegisterValidator.Validate(
                manifest: RegisterValidator.Read(relative: "docs/code/provenance/manifest.json"),
                register: RegisterValidator.Read(relative: "docs/code/register.json"),
                exemptions: RegisterValidator.Read(relative: "docs/code/exemptions.json"));
        Require(value: registryErrors.Count == 0, message: string.Join(separator: "\n", values: registryErrors));
    }

    /// <summary>Runs the original per-path source/configuration checks in every actual owner.</summary>
    internal static IReadOnlyList<string> CheckRepositoryPath(string path, IReadOnlySet<string> permissions,
            ConsumerCompilation[] consumers)
    {
        var errors = new List<string>();
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
                ? [SourceProject(path: path), "tests/phase1/KingdomZombies.Phase1.Tests.csproj"]
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
                y: "tests/phase1/KingdomZombies.Phase1.Tests.csproj");
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
        EngineGeneratedTests.VerifyFixedRows(relative: "docs/code/generated-layout-adaptations.json",
                collection: "members", expected: "a83746c3be9932338ceb5ef54a20205bb93367be16e4da9eb008f411e9eb84ff");
        EngineGeneratedTests.VerifyFixedRows(relative: "docs/code/engine-adaptations.json",
                collection: "local", expected: "8cce39bc8891a37af0c98ebcf43d5e547c093ba70fa408f4d7feaa996cd16587");
        using var callsites = JsonDocument.Parse(json: File.ReadAllText(path: Path.Combine(
                path1: ProofProtocol.Root, path2: "docs/code/generated-callsite-adaptations.json")));
        EngineGeneratedTests.VerifyInventories(registry: callsites.RootElement, baselineCount: 18,
                baselineHash: "ef8ea1e0be872146bcf1f81a2079733bb37be5c2b9718fbf0faa47b164c08d8f");
        int added = callsites.RootElement.GetProperty(propertyName: "local").GetArrayLength() - 18;
        int origins = AddRecords(relative: "docs/code/generated-callsite-adaptations.json", collection: "local",
                count: 18 + added,
                permissions: permissions, authenticatedRegistry: callsites.RootElement);
        AddRecords(relative: "docs/code/generated-layout-adaptations.json", collection: "members", count: 7,
                permissions: permissions);
        AddRecords(relative: "docs/code/engine-adaptations.json", collection: "local", count: 1,
                permissions: permissions);
        Require(value: origins == 8 && permissions.Count == 42 && permissions.Count == 26 + added - origins,
                message: "SUPPRESSION_TOTAL_COUNT");
        return permissions;
    }

    private static readonly JsonSerializerOptions _originJson = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    private static readonly Dictionary<string, OriginPair> _originPairs = new(comparer: StringComparer.Ordinal)
    {
        ["KZ-EX-INSPECTION23-CALLS-1"] = (
                "KZ-EX-INSPECTION26-CALLS-1",
                "~M:KingdomZombies.Inspection.InspectionRoot.GetGodotMethodList",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Inspection.InspectionRoo"
                        + "t_ScriptMethods.generated.cs",
                "90670ddae4552b2c26393d581cf4c529a81df89bff8700563891f3320089776d",
                "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697"),
        ["KZ-EX-INSPECTION23-CALLS-3"] = (
                "KZ-EX-INSPECTION26-CALLS-2",
                "~M:KingdomZombies.Inspection.InspectionRoot.HasGodotClassMethod(Godot.NativeInte"
                        + "rop.godot_string_name@)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Inspection.InspectionRoo"
                        + "t_ScriptMethods.generated.cs",
                "90670ddae4552b2c26393d581cf4c529a81df89bff8700563891f3320089776d",
                "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697"),
        ["KZ-EX-INSPECTION23-CALLS-2"] = (
                "KZ-EX-INSPECTION26-CALLS-3",
                "~M:KingdomZombies.Inspection.InspectionRoot.InvokeGodotClassMethod(Godot.NativeI"
                        + "nterop.godot_string_name@,Godot.NativeInterop.NativeVariantPtrArgs,Godot.NativeI"
                        + "nterop.godot_variant@)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptMethodsGenerator/KingdomZombies.Inspection.InspectionRoo"
                        + "t_ScriptMethods.generated.cs",
                "90670ddae4552b2c26393d581cf4c529a81df89bff8700563891f3320089776d",
                "62b731cc194517856ced1cd294886332555f4bf224fb4863cd174f33d015b697"),
        ["KZ-EX-INSPECTION23-CALLS-4"] = (
                "KZ-EX-INSPECTION26-CALLS-4",
                "~M:KingdomZombies.Inspection.InspectionRoot.SetGodotClassPropertyValue(Godot.Nat"
                        + "iveInterop.godot_string_name@,Godot.NativeInterop.godot_variant@)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Inspection.Inspection"
                        + "Root_ScriptProperties.generated.cs",
                "49987c74fe6d557f5b88d57aa9d8f78d849a85e364a1d771d0b387c75990b902",
                "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b"),
        ["KZ-EX-INSPECTION23-CALLS-5"] = (
                "KZ-EX-INSPECTION26-CALLS-5",
                "~M:KingdomZombies.Inspection.InspectionRoot.GetGodotClassPropertyValue(Godot.Nat"
                        + "iveInterop.godot_string_name@,Godot.NativeInterop.godot_variant@)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Inspection.Inspection"
                        + "Root_ScriptProperties.generated.cs",
                "49987c74fe6d557f5b88d57aa9d8f78d849a85e364a1d771d0b387c75990b902",
                "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b"),
        ["KZ-EX-INSPECTION23-CALLS-6"] = (
                "KZ-EX-INSPECTION26-CALLS-6",
                "~M:KingdomZombies.Inspection.InspectionRoot.GetGodotPropertyList",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptPropertiesGenerator/KingdomZombies.Inspection.Inspection"
                        + "Root_ScriptProperties.generated.cs",
                "49987c74fe6d557f5b88d57aa9d8f78d849a85e364a1d771d0b387c75990b902",
                "c3c7164eb02e57f1d8bf9fbad0fc2c4d5ab79cbbcc1899f8cb99adaf78fc9c7b"),
        ["KZ-EX-INSPECTION23-CALLS-7"] = (
                "KZ-EX-INSPECTION26-CALLS-7",
                "~M:KingdomZombies.Inspection.InspectionRoot.SaveGodotObjectData(Godot.Bridge.God"
                        + "otSerializationInfo)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Inspection.Inspect"
                        + "ionRoot_ScriptSerialization.generated.cs",
                "b1ae41f931f3ddd1d83ca13f9d46ba35e310d1f0ab2eb6b8c4470c7106e0bbd6",
                "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d"),
        ["KZ-EX-INSPECTION23-CALLS-8"] = (
                "KZ-EX-INSPECTION26-CALLS-8",
                "~M:KingdomZombies.Inspection.InspectionRoot.RestoreGodotObjectData(Godot.Bridge."
                        + "GodotSerializationInfo)",
                "src/godot-game/.godot/mono/temp/obj/Debug/generated/Godot.SourceGenerators/Godot"
                        + ".SourceGenerators.ScriptSerializationGenerator/KingdomZombies.Inspection.Inspect"
                        + "ionRoot_ScriptSerialization.generated.cs",
                "b1ae41f931f3ddd1d83ca13f9d46ba35e310d1f0ab2eb6b8c4470c7106e0bbd6",
                "def430bc84dfca85080c110acb092b543607684f0d246aa339ea85d900f8596d"),
    };

    private static readonly string[] _sdkNoWarn = ["1701", "1702"];
    private static readonly string[] _generatedSuffixes = [".g.cs", ".g.i.cs", ".designer.cs", ".generated.cs"];
    private static readonly string[] _trueOrDefault = ["RunAnalyzers", "RunAnalyzersDuringBuild",
        "CodeAnalysisTreatWarningsAsErrors"];
    private static readonly string[] _severityPermissions = ["[*.cs]|dotnet_diagnostic.ma0004.severity|none",
        "[*.cs]|dotnet_diagnostic.ma0051.severity|none", "[*.cs]|dotnet_diagnostic.ma0137.severity|none",
        "[*.cs]|dotnet_diagnostic.ma0156.severity|none",
        "[tests/phase1/**.cs]|dotnet_diagnostic.ca1707.severity|none"];
    private static readonly string[] _localFields = ["sourceId", "rule", "diagnostic", "path", "surface", "subject"];
    private static readonly Dictionary<string, string[]> _localPermissions = new(comparer: StringComparer.Ordinal)
    {
        ["KZ-EX-BODY-LENGTH-01"] = ["EX-01", "R-37", "MA0051", ".editorconfig", "exact editorconfig setting",
            "[*.cs] dotnet_diagnostic.MA0051.severity = none"],
        ["KZ-EX-ENGINE-AWAIT-01"] = ["EX-02", "R-45", "MA0004", ".editorconfig", "exact editorconfig setting",
            "[*.cs] dotnet_diagnostic.MA0004.severity = none; [src/{core,multiplayer-abstractions}/**.cs] error"],
        ["KZ-EX-TEST-NAMES-01"] = ["EX-03", "R-48", "CA1707", ".editorconfig", "exact editorconfig setting",
            "[tests/phase1/**.cs] dotnet_diagnostic.CA1707.severity = none"],
        ["KZ-EX-TEST-EXE-01"] = ["EX-13", "R-52", "PROJECT_OUTPUT_EXE",
            "tests/phase1/KingdomZombies.Phase1.Tests.csproj", "exact project executable permission",
            "KingdomZombies.Phase1.Tests; IsTestProject=true; OutputType=Exe"],
        ["KZ-EX-ANALYZER-BOOTSTRAP-01"] = ["EX-19", "R-54", "ANALYZER_BOOTSTRAP_EDGE", "Directory.Build.targets",
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

    private static int AddRecords(string relative, string collection, int count, HashSet<string> permissions,
            JsonElement? authenticatedRegistry = null)
    {
        JsonArray rows = RegisterValidator.Read(relative: relative)[collection]?.AsArray()
                ?? throw new InvalidDataException(message: "SUPPRESSION_RECORDS_REQUIRED " + relative);
        Require(value: rows.Count == count, message: "SUPPRESSION_RECORD_COUNT " + relative);
        var owners = new Dictionary<string, JsonNode>(comparer: StringComparer.Ordinal);
        var pairs = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (JsonNode? row in rows)
        {
            AuthenticateRecord(relative: relative, collection: collection, row: row,
                    authenticatedRegistry: authenticatedRegistry);
            MergeOrigin(row: row ?? throw new InvalidDataException(message: "SUPPRESSION_RECORD_REQUIRED"),
                    authenticatedRegistry: authenticatedRegistry, permissions: permissions, owners: owners,
                    pairs: pairs);
        }

        if (authenticatedRegistry is not null)
        {
            CompleteOrigins(pairs: pairs);
        }

        return pairs.Count;
    }

    private static void AuthenticateRecord(string relative, string collection, JsonNode? row,
            JsonElement? authenticatedRegistry)
    {
        string path = Text(row: row, name: "path");
        string target = Text(row: row, name: "target");
        Require(value: target.StartsWith(value: "~M:", comparisonType: StringComparison.Ordinal)
                && target.IndexOfAny(anyOf: ['*', '?']) < 0, message: "SUPPRESSION_MEMBER_REQUIRED " + target);
        string surface = Text(row: row, name: "surface");
        bool engine = row?["generatedSourceSha256"] is not null;
        if (authenticatedRegistry is JsonElement registry)
        {
            Require(value: relative is "docs/code/generated-callsite-adaptations.json" && collection is "local",
                    message: "SUPPRESSION_AUTHENTICATED_SCOPE " + relative);
            string id = Text(row: row, name: "id");
            JsonElement authenticated = registry.GetProperty(propertyName: "local").EnumerateArray()
                    .Single(predicate: candidate => StringComparer.Ordinal.Equals(x: candidate.GetProperty(
                            propertyName: "id").GetString(), y: id));
            Require(value: JsonNode.DeepEquals(node1: row, node2: JsonNode.Parse(json:
                    authenticated.GetRawText())), message: "SUPPRESSION_AUTHENTICATED_ROW " + id);
            surface = EngineGeneratedTests.Surface(row: authenticated, registry: registry);
        }
        else if (engine)
        {
            surface = "src/godot-game/.godot/mono/temp/obj/Debug/generated/" + surface;
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

    private static void MergeOrigin(JsonNode row, JsonElement? authenticatedRegistry, HashSet<string> permissions,
            Dictionary<string, JsonNode> owners, HashSet<string> pairs)
    {
        ValidateKnownOrigin(row: row, registry: authenticatedRegistry);
        string key = Permission(path: Text(row: row, name: "path"), diagnostic: Text(row: row, name: "diagnostic"),
                target: Text(row: row, name: "target"));
        if (permissions.Add(item: key))
        {
            owners.Add(key: key, value: row);
            return;
        }

        Require(value: authenticatedRegistry is not null && owners.ContainsKey(key: key),
                message: "SUPPRESSION_RECORD_OVERLAP " + Text(row: row, name: "target"));
        string id = ExactOriginPair(first: owners[key], second: row);
        Require(value: pairs.Add(item: id), message: "SUPPRESSION_PROVENANCE_DUPLICATE " + id);
    }

    private static string ExactOriginPair(JsonNode first, JsonNode second)
    {
        string left = Text(row: first, name: "id");
        string right = Text(row: second, name: "id");
        foreach (KeyValuePair<string, OriginPair> pair in _originPairs)
        {
            bool forward = StringComparer.Ordinal.Equals(x: pair.Key, y: left)
                    && StringComparer.Ordinal.Equals(x: pair.Value.Old, y: right);
            bool reverse = StringComparer.Ordinal.Equals(x: pair.Key, y: right)
                    && StringComparer.Ordinal.Equals(x: pair.Value.Old, y: left);
            if (forward || reverse)
            {
                return pair.Key;
            }
        }

        throw new InvalidDataException(message: "SUPPRESSION_RECORD_OVERLAP " + right);
    }

    private static void CompleteOrigins(HashSet<string> pairs)
    {
        Require(value: _originPairs.Count == 8 && pairs.Count == 8 && pairs.SetEquals(other: _originPairs.Keys),
                message: "SUPPRESSION_PROVENANCE_COVERAGE");
    }

    private static void ValidateKnownOrigin(JsonNode row, JsonElement? registry)
    {
        string id = Text(row: row, name: "id");
        KeyValuePair<string, OriginPair>[] matches = [.. _originPairs.Where(predicate: pair =>
                StringComparer.Ordinal.Equals(x: pair.Key, y: id)
                        || StringComparer.Ordinal.Equals(x: pair.Value.Old, y: id))];
        if (matches.Length == 0)
        {
            return;
        }

        JsonElement originRegistry = registry
                ?? throw new InvalidDataException(message: "SUPPRESSION_PROVENANCE_REGISTRY " + id);
        KeyValuePair<string, OriginPair> pair = matches.Single();
        JsonElement authenticated = originRegistry.GetProperty(propertyName: "local").EnumerateArray().Single(
                predicate: item => StringComparer.Ordinal.Equals(x: item.GetProperty(propertyName: "id")
                        .GetString(), y: id));
        Require(value: JsonNode.DeepEquals(node1: row, node2: JsonNode.Parse(json: authenticated.GetRawText())),
                message: "SUPPRESSION_AUTHENTICATED_ROW " + id);
        ValidateOriginCells(row: row, target: pair.Value.Target, surface: pair.Value.Surface,
                raw: pair.Value.Raw, text: pair.Value.Text);
        ValidateOriginCohort(registry: originRegistry, id: id,
                current: StringComparer.Ordinal.Equals(x: pair.Key, y: id));
    }

    private static void ValidateOriginCells(JsonNode row, string target, string surface, string raw, string text)
    {
        var expected = new Dictionary<string, string>(comparer: StringComparer.Ordinal)
        {
            ["path"] = "src/godot-game/inspection/InspectionRoot.cs",
            ["diagnostic"] = "SCR0001",
            ["target"] = target,
            ["surface"] = surface,
            ["sha256"] = raw,
            ["textSha256"] = text,
            ["producer"] = "Godot.SourceGenerators/4.7.2",
            ["package"] = ".phase1/feed/Godot.SourceGenerators.4.7.2.nupkg",
            ["packageSha256"] = "ce2beb0b75bfb81b94982bdaf7234fc681edd4cf024606c6baf6e40625e0ca4e",
            ["positiveProof"] = "rules.generated-callsite-origin",
            ["negativeProof"] = "rules.godot-callsite-boundary",
        };
        foreach ((string key, string value) in expected)
        {
            Require(value: StringComparer.Ordinal.Equals(x: Text(row: row, name: key), y: value),
                    message: "SUPPRESSION_PROVENANCE_CELL " + key);
        }
    }

    private static void ValidateOriginCohort(JsonElement registry, string id, bool current)
    {
        JsonElement owner = registry.GetProperty(propertyName: "producerInventories").EnumerateArray().Single(
                predicate: cohort => cohort.GetProperty(propertyName: "rowIds").EnumerateArray().Any(
                        predicate: row => StringComparer.Ordinal.Equals(x: row.GetString(), y: id)));
        string cohortId = current ? "inspection23-final-5943a86f6eb2" : "inspection26-lifecycle";
        string inputs = current ? "5943a86f6eb2c875c8a990db1b1dedd884133c0fba5fbe5e1358e22f9dd369da"
                : "1dac6bc39f6d0fb7b4f84564fa75f2ac35460dd4160bad8fcab01d25c8c16621";
        string observations = current ? "c7bb484774af361c9f5e2429d5a8354f59be1d73625c9fb1e9fbbf2532e9741d"
                : "ae4e2c92aa344cc9536b143e5fc5a942db411a420d34112f2cee94d2ad2a99cb";
        Require(value: StringComparer.Ordinal.Equals(x: owner.GetProperty(propertyName: "id").GetString(),
                y: cohortId), message: "SUPPRESSION_PROVENANCE_COHORT " + id);
        OriginHash(owner: owner, field: "sourceInputs", hashField: "sourceInputsSha256", expected: inputs);
        OriginHash(owner: owner, field: "observations", hashField: "observationsSha256", expected: observations);
    }

    private static void OriginHash(JsonElement owner, string field, string hashField, string expected)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value: owner.GetProperty(propertyName: field),
                options: _originJson);
        Require(value: StringComparer.Ordinal.Equals(x: owner.GetProperty(propertyName: hashField).GetString(),
                y: expected) && StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray:
                        SHA256.HashData(source: bytes)), y: expected), message: "SUPPRESSION_PROVENANCE_HASH " + field);
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
