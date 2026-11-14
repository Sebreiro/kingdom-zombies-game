using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Finds every Fact/Theory method by syntax, including explicit and wrapped attributes.</summary>
internal static class TestSourceIndex
{
    /// <summary>Lists fully qualified test methods in source order, without reading sample string contents.</summary>
    internal static IReadOnlyList<string> TestMethods(string content)
    {
        SyntaxNode root = CSharpSyntaxTree.ParseText(text: content).GetRoot();
        return root.DescendantNodes().OfType<MethodDeclarationSyntax>().Where(predicate: IsTestMethod)
                .Select(selector: QualifiedName).ToArray();
    }

    /// <summary>Checks exactly three uppercase-or-digit alphanumeric name parts.</summary>
    internal static bool FollowsMethodStateExpected(string methodName)
    {
        return Regex.IsMatch(input: methodName, pattern: "^[A-Z0-9][A-Za-z0-9]*(_[A-Z0-9][A-Za-z0-9]*){2}$",
                options: RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant,
                matchTimeout: TimeSpan.FromSeconds(value: 1));
    }

    /// <summary>Reads the owned test project source, retaining nested, generic and explicit controls.</summary>
    internal static IReadOnlyList<string> ProductionTestMethods()
    {
        string directory = Path.Combine(path1: ProofProtocol.Root, path2: "tests/phase1");
        return Directory.EnumerateFiles(path: directory, searchPattern: "*.cs",
                searchOption: SearchOption.AllDirectories).Where(predicate: path => !IsBuildOutput(path: path))
                .SelectMany(selector: path => TestMethods(content: File.ReadAllText(path: path)))
                .Order(comparer: StringComparer.Ordinal).ToArray();
    }

    private static bool IsTestMethod(MethodDeclarationSyntax method)
    {
        return method.AttributeLists.SelectMany(selector: list => list.Attributes).Any(predicate: attribute =>
                IsTestAttribute(name: attribute.Name));
    }

    private static bool IsTestAttribute(NameSyntax name)
    {
        string simple = name switch
        {
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            AliasQualifiedNameSyntax alias => alias.Name.Identifier.ValueText,
            SimpleNameSyntax plain => plain.Identifier.ValueText,
            _ => name.ToString()
        };
        if (simple.EndsWith(value: "Attribute", comparisonType: StringComparison.Ordinal))
        {
            simple = simple[..^"Attribute".Length];
        }

        return simple.EndsWith(value: "Fact", comparisonType: StringComparison.Ordinal)
                || simple.EndsWith(value: "Theory", comparisonType: StringComparison.Ordinal);
    }

    private static string QualifiedName(MethodDeclarationSyntax method)
    {
        IEnumerable<string> containers = method.Ancestors().Reverse().Select(selector: node => node switch
        {
            BaseNamespaceDeclarationSyntax space => space.Name.ToString(),
            BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
            _ => string.Empty
        }).Where(predicate: name => name.Length > 0);
        return string.Join(separator: ".", values: containers.Append(element: method.Identifier.ValueText));
    }

    private static bool IsBuildOutput(string path)
    {
        return path.Split(separator: [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar])
                .Any(predicate: part => part is "bin" or "obj");
    }
}
