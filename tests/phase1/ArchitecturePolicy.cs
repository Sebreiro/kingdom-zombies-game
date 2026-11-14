using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Checks structural obligations; semantic design judgments remain separate review obligations.</summary>
internal static class ArchitecturePolicy
{
    /// <summary>Accepts only the exact independently declared public API and observed engine script surfaces.</summary>
    internal static IReadOnlyList<string> Visibility(string root, string source, string path)
    {
        JsonObject registry = JsonNode.Parse(json: File.ReadAllText(path: Path.Combine(path1: root,
                path2: "docs/code/visibility-exemptions.json")))?.AsObject()
                ?? throw new InvalidDataException(message: "VISIBILITY_REGISTRY: missing object");
        string[] expected = ["src/core/ProofValue.cs|KingdomZombies.Core.ProofValue",
            "src/multiplayer-abstractions/IProofService.cs|KingdomZombies.Multiplayer.Abstractions.IProofService",
            "src/godot-game/proofs/Phase1.cs|KingdomZombies.Proofs.Phase1"];
        JsonArray rows = registry["permissions"]?.AsArray()
                ?? throw new InvalidDataException(message: "VISIBILITY_REGISTRY: missing permissions");
        string[] permissions = [..rows.Select(selector: row => row?["path"]?.GetValue<string>() + "|"
                + row?["type"]?.GetValue<string>())];
        if (registry.Count != 5 || registry["schemaVersion"]?.GetValue<int>() != 1
                || registry["rule"]?.GetValue<string>() is not "R-34"
                || registry["positiveProof"]?.GetValue<string>() is not "R-34.positive"
                || registry["negativeProof"]?.GetValue<string>() is not "R-34.negative"
                || rows.Any(predicate: row => row?.AsObject().Count != 3 || string.IsNullOrWhiteSpace(
                        value: row?["reason"]?.GetValue<string>()))
                || permissions.Length != expected.Length || permissions.Distinct(comparer: StringComparer.Ordinal)
                        .Count() != permissions.Length || !permissions.Order(comparer: StringComparer.Ordinal)
                                .SequenceEqual(second: expected.Order(comparer: StringComparer.Ordinal),
                                        comparer: StringComparer.Ordinal))
        {
            return ["R-34: invalid or expanded exact visibility registry"];
        }

        return CSharpSyntaxTree.ParseText(text: source).GetRoot().DescendantNodes()
                .OfType<BaseTypeDeclarationSyntax>().Where(predicate: type => type.Modifiers.Any(predicate: modifier =>
                        modifier.IsKind(kind: SyntaxKind.PublicKeyword))).Where(predicate: type => !permissions
                                .Contains(
                                        value: path + "|" + FullName(type: type), comparer: StringComparer.Ordinal))
                .Select(selector: type => "R-34: undeclared public type " + FullName(type: type) + " in " + path)
                .ToArray();
    }

    /// <summary>Rejects any authored version declaration outside the single shared version authority.</summary>
    internal static IReadOnlyList<string> Version(string root)
    {
        var errors = new List<string>();
        string[] names = ["Version", "VersionPrefix", "VersionSuffix", "AssemblyVersion", "FileVersion"];
        string[] files = [.._patterns.SelectMany(selector: pattern =>
                ProjectEvaluator.DiscoverFiles(root: root, pattern: pattern))
                .Distinct(comparer: StringComparer.Ordinal)];
        int declarations = 0;
        foreach (string file in files)
        {
            foreach (XElement element in XDocument.Load(uri: Path.Combine(path1: root, path2: file)).Descendants()
                    .Where(predicate: element =>
                            names.Contains(value: element.Name.LocalName, comparer: StringComparer.Ordinal)))
            {
                declarations++;
                if (!StringComparer.Ordinal.Equals(x: file, y: "Directory.Build.props")
                        || !StringComparer.Ordinal.Equals(x: element.Name.LocalName,
                                y: "VersionPrefix") || !StringComparer.Ordinal.Equals(x: element.Value, y: "0.1.0"))
                {
                    errors.Add(item: "R-66: conflicting version " + file + ":" + element.Name.LocalName);
                }
            }
        }

        if (declarations != 1)
        {
            errors.Add(item: "R-66: exactly one version declaration required");
        }

        return errors;
    }

    /// <summary>Requires explicit concrete model selectors before checking constructors and private mutation.</summary>
    internal static IReadOnlyList<string> Model(string source, string? selector, string rule)
    {
        if (string.IsNullOrWhiteSpace(value: selector))
        {
            return ["MODEL_SELECTOR: " + rule];
        }

        SyntaxNode root = CSharpSyntaxTree.ParseText(text: source).GetRoot();
        BaseTypeDeclarationSyntax[] selected = [..root.DescendantNodes().OfType<BaseTypeDeclarationSyntax>()
                .Where(predicate: type => StringComparer.Ordinal.Equals(x: FullName(type: type), y: selector))];
        if (selected.Length != 1)
        {
            return ["MODEL_MISSING: " + selector];
        }

        if (selected[0] is not ClassDeclarationSyntax model || model.Modifiers.Any(predicate: modifier => modifier
                .IsKind(kind: SyntaxKind.StaticKeyword)))
        {
            return [rule + ": class required " + selector];
        }

        var errors = new List<string>();
        if (!model.Members.OfType<ConstructorDeclarationSyntax>().Any(predicate: constructor => constructor
                .ParameterList.Parameters.Count > 0))
        {
            errors.Add(item: rule + ": explicit initializing constructor required " + selector);
        }

        foreach (PropertyDeclarationSyntax property in model.Members.OfType<PropertyDeclarationSyntax>())
        {
            foreach (AccessorDeclarationSyntax accessor in property.AccessorList?.Accessors ?? [])
            {
                if (accessor.IsKind(kind: SyntaxKind.InitAccessorDeclaration) || (accessor.IsKind(
                        kind: SyntaxKind.SetAccessorDeclaration) && !accessor.Modifiers.Any(predicate:
                                modifier => modifier.IsKind(kind: SyntaxKind.PrivateKeyword))))
                {
                    errors.Add(item: rule + ": nonprivate mutation " + selector + "." + property.Identifier.ValueText);
                }
            }
        }

        if (model.Members.OfType<FieldDeclarationSyntax>().Any(predicate: field => !field.Modifiers.Any(
                predicate: modifier => modifier.IsKind(kind: SyntaxKind.PrivateKeyword))))
        {
            errors.Add(item: rule + ": exposed field " + selector);
        }

        return errors;
    }

    /// <summary>Requires explicit constructor mapping and rejects automatic mapper dependencies.</summary>
    internal static IReadOnlyList<string> Mapping(string source, string selector)
    {
        SyntaxNode root = CSharpSyntaxTree.ParseText(text: source).GetRoot();
        MethodDeclarationSyntax[] methods = [..root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(predicate: method => StringComparer.Ordinal.Equals(x: method.Identifier.ValueText,
                        y: selector))];
        bool automatic = root.DescendantNodes().OfType<NameSyntax>().Any(predicate: name => name.ToString()
                .Split(separator: '.').Any(predicate: part => part is "AutoMapper" or "Mapster" or "Mapperly"));
        bool constructor = methods.Length == 1 && methods[0].ParameterList.Parameters.Count > 0
                && methods[0].Body?.Statements.OfType<ReturnStatementSyntax>().Any(predicate: statement => statement
                        .Expression is ObjectCreationExpressionSyntax creation && creation.ArgumentList?.Arguments
                                .Count > 0 && StringComparer.Ordinal.Equals(x: creation.Type.ToString(),
                                        y: methods[0].ReturnType.ToString())) == true;
        return automatic || !constructor ? ["R-65: explicit constructor mapping required " + selector] : [];
    }

    /// <summary>Checks resolved service-locator uses and aliases against the sole composition identity.</summary>
    internal static IReadOnlyList<string> ServiceLocator(CSharpCompilation compilation)
    {
        var errors = new List<string>();
        foreach (SyntaxTree tree in compilation.SyntaxTrees)
        {
            SemanticModel model = compilation.GetSemanticModel(syntaxTree: tree);
            foreach (NameSyntax name in tree.GetRoot().DescendantNodes().OfType<NameSyntax>())
            {
                if (name.Ancestors().OfType<UsingDirectiveSyntax>().Any())
                {
                    continue;
                }

                if (model.GetSymbolInfo(node: name).Symbol is not INamedTypeSymbol symbol || !StringComparer.Ordinal
                        .Equals(x: symbol.ToDisplayString(), y: "System.IServiceProvider"))
                {
                    continue;
                }

                INamedTypeSymbol? owner = model.GetEnclosingSymbol(position: name.SpanStart)?.ContainingType;
                if (!StringComparer.Ordinal.Equals(x: owner?.ToDisplayString(), y: "KingdomZombies.CompositionRoot"))
                {
                    errors.Add(item: "R-60: service locator outside composition " + tree.FilePath + ":"
                            + (name.GetLocation().GetLineSpan().StartLinePosition.Line + 1)
                                    .ToString(provider: System.Globalization.CultureInfo.InvariantCulture));
                }
            }
        }

        return errors;
    }

    /// <summary>Checks zero sentinel, unique pinned names and additive evolution against an independent map.</summary>
    internal static IReadOnlyList<string> Enum(string source, string selector, string rule)
    {
        EnumDeclarationSyntax[] enums = [..CSharpSyntaxTree.ParseText(text: source).GetRoot().DescendantNodes()
                .OfType<EnumDeclarationSyntax>().Where(predicate: type => StringComparer.Ordinal.Equals(
                        x: FullName(type: type), y: selector))];
        if (enums.Length != 1 || enums[0].Members.Count == 0)
        {
            return ["ENUM_SELECTOR: " + selector];
        }

        var errors = new List<string>();
        var pinned = new Dictionary<string, string>(comparer: StringComparer.Ordinal);
        foreach (EnumMemberDeclarationSyntax member in enums[0].Members)
        {
            AttributeSyntax[] attributes = [..member.AttributeLists.SelectMany(selector: list => list.Attributes)
                    .Where(predicate: attribute => attribute.Name.ToString().EndsWith(value: "JsonStringEnumMemberName",
                            comparisonType: StringComparison.Ordinal))];
            string? name = attributes.Length == 1 ? (attributes[0].ArgumentList?.Arguments.FirstOrDefault()?.Expression
                    as LiteralExpressionSyntax)?.Token.ValueText : null;
            string number = member.EqualsValue?.Value.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(value: name) || !pinned.TryAdd(key: name, value: number))
            {
                errors.Add(item: "R-75: missing or duplicate pinned name " + selector + "."
                        + member.Identifier.ValueText);
            }
        }

        if (StringComparer.Ordinal.Equals(x: rule, y: "R-74") && !enums[0].Members.Any(predicate: member =>
                StringComparer.Ordinal.Equals(x: member.Identifier.ValueText, y: "Unknown")
                        && StringComparer.Ordinal.Equals(x: member.EqualsValue?.Value.ToString(), y: "0")))
        {
            errors.Add(item: "R-74: Unknown = 0 required " + selector);
        }

        if (StringComparer.Ordinal.Equals(x: rule, y: "R-77") && (!pinned.TryGetValue(key: "unknown", value:
                out string? unknown) || !StringComparer.Ordinal.Equals(x: unknown, y: "0") || !pinned.TryGetValue(
                        key: "ready", value: out string? ready) || !StringComparer.Ordinal.Equals(x: ready, y: "1")))
        {
            errors.Add(item: "R-77: removed, renamed or renumbered pinned value " + selector);
        }

        return errors;
    }

    private static readonly string[] _patterns = ["*.csproj", "*.props", "*.targets"];
    private static string FullName(BaseTypeDeclarationSyntax type)
    {
        IEnumerable<string> owners = type.Ancestors().Reverse().Select(selector: node => node switch
        {
            BaseNamespaceDeclarationSyntax space => space.Name.ToString(),
            BaseTypeDeclarationSyntax container => container.Identifier.ValueText,
            _ => ""
        }).Where(predicate: part => part.Length > 0);
        return string.Join(separator: ".", values: owners.Append(element: type.Identifier.ValueText));
    }
}
