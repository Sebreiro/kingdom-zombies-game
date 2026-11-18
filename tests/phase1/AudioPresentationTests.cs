using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves compiled audio ownership independently of eventual listening judgments.</summary>
public sealed class AudioPresentationTests
{
    /// <summary>Replacement owners cannot revive an earlier owner with the same local state.</summary>
    [Fact]
    public void ReplacementOwners_ReceiveStrictlyIncreasingEpochs()
    {
        long first = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        long second = (long)(Call(name: "NextEpoch", arguments: []) ?? throw new InvalidOperationException());
        second.ShouldBeGreaterThan(expected: first);
    }

    /// <summary>The actual generated food source resolves its declared deterministic provenance.</summary>
    [Fact]
    public void Sources_ResolveGeneratedFoodAuthority()
    {
        Source(id: "interface.food-crunch.v1").ShouldContain(expected: "seed190021");
    }

    /// <summary>The derived burst resolves its current shared source without approval extension.</summary>
    [Fact]
    public void Sources_ResolveCurrentSharedAction()
    {
        Source(id: "interface.bloater-burst.v1").ShouldContain(expected: "2790");
    }

    /// <summary>Every current inline, donor, generated and shared variant retains nonempty provenance.</summary>
    [Fact]
    public void Sources_ResolveEveryCurrentVariant()
    {
        int count = 0;
        JsonElement actions = Pack(file: "audio-actions");
        foreach (JsonElement pack in new[] { actions, Pack(file: "audio-interface-ambience") })
        {
            foreach (JsonElement sample in pack.GetProperty(propertyName: "samples").EnumerateArray())
            {
                ((string?)Call(name: "ResolveSource", arguments: [pack, sample, actions])).ShouldNotBeNullOrEmpty();
                count++;
            }
        }

        count.ShouldBe(expected: 71);
    }

    /// <summary>Source authorities reject rebindings and restore current positive inputs.</summary>
    [Theory]
    [InlineData("unknown", "sources")]
    [InlineData("donor-ambiguous", "sources")]
    [InlineData("donor-wrong-valid", "sourceId")]
    [InlineData("donor-owner", "source")]
    [InlineData("shared-target", "sharedSources")]
    [InlineData("shared-pack", "sharedSources.pack")]
    [InlineData("shared-hash", "sharedSources.wavSha256")]
    [InlineData("shared-current-hash", "sharedSources.wavSha256")]
    [InlineData("generated-authority", "donorGap")]
    [InlineData("generated-recipe", "recipes")]
    [InlineData("generated-recipe-hash", "recipes")]
    public void Sources_RejectWrongAuthoritiesAndRestore(string control, string field)
    {
        JsonElement original = Pack(file: "audio-interface-ambience");
        JsonObject pack = Object(json: original.GetRawText());
        JsonObject actions = Object(json: Pack(file: "audio-actions").GetRawText());
        string id = control.StartsWith(value: "shared", comparisonType: StringComparison.Ordinal)
                ? "interface.bloater-burst.v1" : control.StartsWith(value: "generated",
                        comparisonType: StringComparison.Ordinal) ? "interface.food-crunch.v1" : "interface.r3254.full";
        JsonElement current = original.GetProperty(propertyName: "samples").EnumerateArray()
                .Single(predicate: value => StringComparer.Ordinal.Equals(
                        x: value.GetProperty(propertyName: "id").GetString(), y: id));
        JsonObject sample = Object(json: current.GetRawText());
        Mutate(control: control, pack: pack, sample: sample, actions: actions);
        TargetInvocationException error = Should.Throw<TargetInvocationException>(actual: () => Call(
                name: "ResolveSource", arguments: [Element(node: pack), Element(node: sample),
                    Element(node: actions)]));
        error.InnerException.ShouldBeOfType<InvalidDataException>().Message.ShouldContain(
                expected: "content/packs/audio-interface-ambience.json, field " + field + ", value ");
        ((string?)Call(name: "ResolveSource", arguments: [original, current, Pack(file: "audio-actions")]))
                .ShouldNotBeNullOrEmpty();
    }

    private static readonly Type _subject = Subject();

    private static JsonObject Object(string json)
    {
        return JsonNode.Parse(json: json)?.AsObject() ?? throw new InvalidOperationException();
    }

    private static JsonNode Field(JsonObject owner, string field)
    {
        return owner[propertyName: field] ?? throw new InvalidOperationException(message: field);
    }

    private static JsonObject First(JsonObject owner, string field)
    {
        return Field(owner: owner, field: field).AsArray()[index: 0]?.AsObject()
                ?? throw new InvalidOperationException(message: field);
    }

    private static JsonElement Element(JsonObject node)
    {
        using var document = JsonDocument.Parse(json: node.ToJsonString());
        return document.RootElement.Clone();
    }

    private static void Mutate(string control, JsonObject pack, JsonObject sample, JsonObject actions)
    {
        switch (control)
        {
            case "unknown":
                sample[propertyName: "sourceId"] = "unregistered-current-source";
                break;
            case "donor-ambiguous":
                Field(owner: pack, field: "sources").AsArray().Add(item: First(owner: pack,
                        field: "sources").DeepClone());
                break;
            case "donor-wrong-valid":
                sample[propertyName: "sourceId"] = "r4864";
                break;
            case "donor-owner":
                Field(owner: First(owner: pack, field: "sources"), field: "source").AsObject()[propertyName: "owner"]
                        = "sharedassets9.assets";
                break;
            case "shared-target":
                First(owner: pack, field: "sharedSources")[propertyName: "id"] = "action.3294.full";
                break;
            case "shared-pack":
                First(owner: pack, field: "sharedSources")[propertyName: "pack"]
                        = "packs/audio-interface-ambience.json";
                break;
            case "shared-hash":
                First(owner: pack, field: "sharedSources")[propertyName: "wavSha256"] = Field(
                        owner: First(owner: actions, field: "samples"), field: "sha256").DeepClone();
                break;
            case "shared-current-hash":
                JsonObject target = Field(owner: actions, field: "samples").AsArray().Select(selector: value =>
                        value?.AsObject() ?? throw new InvalidOperationException()).Single(predicate: value =>
                                Field(owner: value, field: "id").GetValue<string>() is "action.2790.full");
                target[propertyName: "sha256"] = Field(owner: First(owner: actions, field: "samples"),
                        field: "sha256").DeepClone();
                break;
            case "generated-authority":
                pack.Remove(propertyName: "donorGap");
                break;
            case "generated-recipe":
                pack.Remove(propertyName: "recipes");
                break;
            case "generated-recipe-hash":
                JsonObject recipe = Field(owner: pack, field: "recipes").AsObject();
                recipe[propertyName: "assemblerSha256"] = Field(owner: recipe, field: "decoderSha256").DeepClone();
                break;
        }
    }

    private static string Source(string id)
    {
        JsonElement pack = Pack(file: "audio-interface-ambience");
        JsonElement sample = pack.GetProperty(propertyName: "samples").EnumerateArray()
                .Single(predicate: value => StringComparer.Ordinal.Equals(
                        x: value.GetProperty(propertyName: "id").GetString(), y: id));
        try
        {
            return (string)(Call(name: "ResolveSource", arguments: [pack, sample, Pack(file: "audio-actions")])
                    ?? throw new InvalidOperationException());
        }
        catch (TargetInvocationException error) when (error.InnerException is InvalidOperationException
                && error.InnerException.Message is "Sequence contains no matching element")
        {
            return "Production ordinary-source lookup rejected this valid sample";
        }
    }

    private static JsonElement Pack(string file)
    {
        using var document = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: Path.Combine(
                path1: ProofProtocol.Root, path2: "src/godot-game/content/packs/" + file + ".json")));
        return document.RootElement.Clone();
    }

    private static object? Call(string name, object[] arguments)
    {
        MethodInfo method = _subject.GetMethod(name: name, bindingAttr: BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException(message: name);
        return method.Invoke(obj: null, parameters: arguments);
    }

    private static Type Subject()
    {
        var assembly = Assembly.Load(rawAssembly: File.ReadAllBytes(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/.godot/mono/temp/bin/Debug/KingdomZombies.dll")));
        return assembly.GetType(name: "KingdomZombies.Inspection.AudioPreview", throwOnError: true)
                ?? throw new InvalidOperationException();
    }
}
