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
    /// <summary>Checks the semantic engine access boundary without constructing native engine objects.</summary>
    internal static IReadOnlyList<string> EngineAccess(string source, string path)
    {
        return EngineAccessPolicy.Check(source: source, path: path);
    }

    /// <summary>Checks independent concrete subjects in the actual compiled pure and reusable projects.</summary>
    internal static IReadOnlyList<string> ValidateRepository(string root)
    {
        var errors = new List<string>();
        errors.AddRange(collection: EngineAccessPolicy.Repository());
        errors.AddRange(collection: Validate(assembly: Path.Combine(path1: root,
                path2: "src/godot-game/core/bin/Debug/net10.0/KingdomZombies.Core.dll"),
                expectedType: "KingdomZombies.Core.ProofValue", boundary: "core"));
        errors.AddRange(collection: Validate(assembly: Path.Combine(path1: root,
                path2: "src/godot-game/multiplayer-abstractions/bin/Debug/net10.0/"
                        + "KingdomZombies.Multiplayer.Abstractions.dll"),
                expectedType: "KingdomZombies.Multiplayer.Abstractions.IProofService", boundary: "ports"));
        errors.AddRange(collection: Validate(assembly: Path.Combine(path1: root,
                path2: "tools/code-rules/bin/Debug/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll"),
                expectedType: "Sebreiro.CodeRules.Analyzers.NamedArgumentAnalyzer", boundary: "analyzer"));
        errors.AddRange(collection: RequireType(assembly: Path.Combine(path1: root,
                path2: "src/godot-game/game/.godot/mono/temp/bin/Debug/KingdomZombies.dll"),
                expectedType: "KingdomZombies.Proofs.Phase1"));
        errors.AddRange(collection: RequireType(assembly: Path.Combine(path1: root,
                path2: "src/godot-game/tests/verification/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll"),
                expectedType: "KingdomZombies.Phase1.Tests.GraphTests"));
        return errors;
    }

    /// <summary>Requires concrete engine and proof subjects without imposing production purity.</summary>
    internal static IReadOnlyList<string> RequireType(string assembly, string expectedType)
    {
        if (string.IsNullOrWhiteSpace(value: expectedType) || !File.Exists(path: assembly))
        {
            return ["TYPE_MISSING: " + expectedType + " in " + assembly];
        }

        using FileStream stream = File.OpenRead(path: assembly);
        using var pe = new PEReader(peStream: stream);
        MetadataReader reader = pe.GetMetadataReader();
        bool present = reader.TypeDefinitions.Select(selector: handle => reader.GetTypeDefinition(handle: handle))
                .Any(predicate: type => StringComparer.Ordinal.Equals(x: reader.GetString(handle: type.Namespace)
                        + "." + reader.GetString(handle: type.Name), y: expectedType));
        return present ? [] : ["TYPE_MISSING: " + expectedType + " in " + assembly];
    }

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

        InspectReferences(reader: reader, boundary: boundary, assembly: assembly, errors: errors);
        return errors.Order(comparer: StringComparer.Ordinal).ToArray();
    }

    private static void InspectReferences(MetadataReader reader, string boundary, string assembly, List<string> errors)
    {
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
            string owner = ReferenceAssembly(reader: reader, handle: type.ResolutionScope);
            if (owner.Length > 0 && !AllowedAssembly(name: owner, boundary: boundary))
            {
                errors.Add(item: "TYPE_FORBIDDEN: " + name + " from " + owner + " in " + assembly);
            }

            if (StringComparer.Ordinal.Equals(x: name, y: "System.IServiceProvider"))
            {
                errors.Add(item: "TYPE_SERVICE_LOCATOR: " + name + " in " + assembly);
            }
        }

    }

    private static string ReferenceAssembly(MetadataReader reader, EntityHandle handle)
    {
        return handle.Kind switch
        {
            HandleKind.AssemblyReference => reader.GetString(handle: reader.GetAssemblyReference(
                    handle: (AssemblyReferenceHandle)handle).Name),
            HandleKind.TypeReference => ReferenceAssembly(reader: reader, handle: reader.GetTypeReference(
                    handle: (TypeReferenceHandle)handle).ResolutionScope),
            _ => ""
        };
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
