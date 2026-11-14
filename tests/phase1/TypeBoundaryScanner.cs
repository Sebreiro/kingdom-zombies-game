using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Inspects compiled metadata without loading or constructing engine objects.</summary>
internal static class TypeBoundaryScanner
{
    /// <summary>Checks expected compiled subjects and referenced types, including generated bodies.</summary>
    internal static IReadOnlyList<string> Validate(string assembly, string expectedType, string boundary)
    {
        return Validate(assembly: assembly, expectedTypes: [expectedType], boundary: boundary);
    }

    /// <summary>Rejects absent or duplicate subjects before inspecting the complete compiled token inventory.</summary>
    internal static IReadOnlyList<string> Validate(string assembly, IReadOnlyList<string>? expectedTypes,
            string boundary)
    {
        var errors = new List<string>();
        if (expectedTypes is null || expectedTypes.Count == 0 || expectedTypes.Any(predicate: string.IsNullOrWhiteSpace)
                || expectedTypes.Distinct(comparer: StringComparer.Ordinal).Count() != expectedTypes.Count)
        {
            return ["TYPE_SELECTOR: missing, empty or duplicate selector " + assembly];
        }

        if (!File.Exists(path: assembly))
        {
            return ["TYPE_MISSING: assembly " + assembly];
        }

        if (boundary is not ("core" or "ports" or "analyzer" or "provider"))
        {
            return ["TYPE_SELECTOR: unknown boundary " + boundary];
        }

        using FileStream stream = File.OpenRead(path: assembly);
        using var pe = new PEReader(peStream: stream);
        MetadataReader reader = pe.GetMetadataReader();
        IEnumerable<TypeDefinition> definitions = reader.TypeDefinitions.Select(selector: handle => reader
                .GetTypeDefinition(handle: handle));
        string[] defined = [..definitions.Select(selector: type => reader.GetString(handle: type.Namespace) + "."
                + reader.GetString(handle: type.Name))];
        foreach (string expected in expectedTypes)
        {
            if (!defined.Contains(value: expected, comparer: StringComparer.Ordinal))
            {
                errors.Add(item: "TYPE_MISSING: " + expected + " in " + assembly);
            }
        }

        foreach (AssemblyReferenceHandle handle in reader.AssemblyReferences)
        {
            string dependency = reader.GetString(handle: reader.GetAssemblyReference(handle: handle).Name);
            if (!AllowedAssembly(name: dependency, boundary: boundary))
            {
                errors.Add(item: "TYPE_FORBIDDEN: " + boundary + " -> " + dependency + " in " + assembly);
            }
        }

        foreach (TypeReferenceHandle handle in reader.TypeReferences)
        {
            TypeReference type = reader.GetTypeReference(handle: handle);
            string name = reader.GetString(handle: type.Namespace) + "." + reader.GetString(handle: type.Name);
            if (StringComparer.Ordinal.Equals(x: name, y: "System.IServiceProvider"))
            {
                errors.Add(item: "TYPE_SERVICE_LOCATOR: " + name + " in " + assembly);
            }
        }

        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    private static bool AllowedAssembly(string name, string boundary)
    {
        bool framework = name is "System" or "mscorlib" or "netstandard" || name.StartsWith(value: "System.",
                comparisonType: StringComparison.Ordinal);
        bool compiler = StringComparer.Ordinal.Equals(x: boundary, y: "analyzer")
                && name.StartsWith(value: "Microsoft.CodeAnalysis", comparisonType: StringComparison.Ordinal);
        bool ports = StringComparer.Ordinal.Equals(x: boundary, y: "provider")
                && StringComparer.Ordinal.Equals(x: name, y: "KingdomZombies.Multiplayer.Abstractions");
        return framework || compiler || ports;
    }
}
