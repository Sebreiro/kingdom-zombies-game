using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Reads one selected consumer's evaluated compiler inputs for a bounded source-policy scan.</summary>
internal sealed class ConsumerCompilation
{
    /// <summary>Gets the exact evaluated project identity used for source ownership.</summary>
    internal string Project { get; }

    /// <summary>Gets the evaluated assembly name, so friend-assembly access matches the real build.</summary>
    internal string AssemblyName => _assembly;

    /// <summary>Gets every evaluated Compile input; emitted generator outputs are added separately.</summary>
    internal IReadOnlyList<string> AuthoredPaths => _authored;

    /// <summary>Gets the selected compiler's complete symbol set and language version.</summary>
    internal CSharpParseOptions Options { get; }

    /// <summary>Gets resolved references from the selected consumer rather than the proof host runtime.</summary>
    internal IReadOnlyList<MetadataReference> References => _references;

    /// <summary>Evaluates once per scan; callers reuse the context while source bytes are read afresh.</summary>
    internal static ConsumerCompilation Read(string root, string project)
    {
        (int Exit, string Output) query = GraphFixture.RunAt(root: root, arguments: ["msbuild", project,
            "-nologo", "-target:ResolveReferences", "-p:BuildProjectReferences=false", "-p:Configuration=Debug",
            "-getItem:Compile,ReferencePath", "-getProperty:DefineConstants,LangVersion,AssemblyName,"
                    + "CompilerGeneratedFilesOutputPath"]);
        if (query.Exit != 0)
        {
            throw new InvalidDataException(message: "CONSUMER_INPUTS " + project + " " + query.Output);
        }

        JsonObject evaluation = JsonNode.Parse(json: query.Output)?.AsObject()
                ?? throw new InvalidDataException(message: "CONSUMER_INPUTS missing evaluation " + project);
        string language = Property(evaluation: evaluation, name: "LangVersion");
        if (!LanguageVersionFacts.TryParse(version: language, result: out LanguageVersion version)
                || string.IsNullOrWhiteSpace(value: language))
        {
            throw new InvalidDataException(message: "CONSUMER_LANGUAGE " + project);
        }

        string symbols = Property(evaluation: evaluation, name: "DefineConstants");
        if (string.IsNullOrWhiteSpace(value: symbols))
        {
            throw new InvalidDataException(message: "CONSUMER_SYMBOLS " + project);
        }

        var options = new CSharpParseOptions(languageVersion: version, preprocessorSymbols: symbols.Split(
                separator: [';', ','], options: StringSplitOptions.RemoveEmptyEntries
                        | StringSplitOptions.TrimEntries));
        return new ConsumerCompilation(root: root, project: project, options: options, evaluation: evaluation);
    }

    /// <summary>Parses actual and replacement inputs with the same selected consumer options.</summary>
    internal SyntaxTree Parse(string source, string path)
    {
        return CSharpSyntaxTree.ParseText(text: source, options: Options, path: path);
    }

    /// <summary>Builds a semantic view with resolved evaluated references and freshly read source inputs.</summary>
    internal CSharpCompilation Create(string? source = null, string? path = null)
    {
        IEnumerable<SyntaxTree> trees = _paths.Where(predicate: candidate => !StringComparer.Ordinal.Equals(
                x: candidate, y: path)).Select(selector: candidate => Parse(source: File.ReadAllText(
                        path: Path.Combine(path1: _root, path2: candidate)), path: candidate));
        if (source is not null && path is not null)
        {
            trees = trees.Append(element: Parse(source: source, path: path));
        }

        return CSharpCompilation.Create(assemblyName: _assembly, syntaxTrees: trees, references: _references,
                options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary,
                        nullableContextOptions: NullableContextOptions.Enable, allowUnsafe: true));
    }

    /// <summary>Incomplete semantic subjects fail closed before unresolved operations could be ignored.</summary>
    internal static void RequireValid(CSharpCompilation compilation)
    {
        Diagnostic[] errors = [..compilation.GetDiagnostics().Where(predicate: diagnostic =>
                diagnostic.Severity is DiagnosticSeverity.Error)];
        if (errors.Length > 0)
        {
            throw new InvalidDataException(message: "CONSUMER_COMPILATION " + string.Join(separator: "\n",
                    values: errors.Select(selector: diagnostic => diagnostic.ToString())));
        }
    }

    private readonly string _root;
    private readonly string _assembly;
    private readonly string[] _authored;
    private readonly string[] _paths;
    private readonly MetadataReference[] _references;

    private ConsumerCompilation(string root, string project, CSharpParseOptions options, JsonObject evaluation)
    {
        Project = project;
        _root = root;
        _assembly = Property(evaluation: evaluation, name: "AssemblyName");
        Options = options;
        string generated = Property(evaluation: evaluation, name: "CompilerGeneratedFilesOutputPath");
        string[] compile = Items(evaluation: evaluation, name: "Compile");
        _authored = [.. compile.Select(selector: file => Relative(root: root, file: file))];
        if (_authored.Length == 0 || string.IsNullOrWhiteSpace(value: _assembly))
        {
            throw new InvalidDataException(message: "CONSUMER_COMPILE_REQUIRED " + project);
        }

        IEnumerable<string> emitted = Directory.Exists(path: generated) ? Directory.EnumerateFiles(path: generated,
                searchPattern: "*.cs", searchOption: SearchOption.AllDirectories) : [];
        _paths = [.. compile.Concat(second: emitted).Select(selector: file => Relative(root: root, file: file))
                .Distinct(comparer: StringComparer.Ordinal)];
        string[] references = Items(evaluation: evaluation, name: "ReferencePath");
        if (references.Length == 0)
        {
            throw new InvalidDataException(message: "CONSUMER_REFERENCES_REQUIRED " + project);
        }

        _references = [.. references.Select(selector: file => MetadataReference.CreateFromFile(path: file))];
    }

    private static string Property(JsonObject evaluation, string name)
    {
        return evaluation["Properties"]?[name]?.GetValue<string>()
                ?? throw new InvalidDataException(message: "CONSUMER_PROPERTY " + name);
    }

    private static string[] Items(JsonObject evaluation, string name)
    {
        JsonArray items = evaluation["Items"]?[name]?.AsArray()
                ?? throw new InvalidDataException(message: "CONSUMER_ITEMS " + name);
        return [..items.Select(selector: item => item?["FullPath"]?.GetValue<string>()
                ?? throw new InvalidDataException(message: "CONSUMER_ITEM_PATH " + name))];
    }

    private static string Relative(string root, string file)
    {
        string path = Path.GetRelativePath(relativeTo: root, path: file).Replace(oldChar: '\\', newChar: '/');
        if (Path.IsPathRooted(path: path) || path.Split(separator: '/').Any(predicate: part => part is ".."))
        {
            throw new InvalidDataException(message: "CONSUMER_SOURCE_OWNERSHIP " + file);
        }

        return path;
    }
}
