using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Requires the complete unique fresh set of actual native surface assertions after cleanup.</summary>
internal static class EngineEvidencePolicy
{
    /// <summary>Rejects zero-exit evidence with any missing, duplicate, stale or unknown surface.</summary>
    internal static void Validate(string output, string nonce)
    {
        if (output.Length > 1048576 || string.IsNullOrEmpty(value: nonce))
        {
            throw new InvalidOperationException(message: "ENGINE_EVIDENCE_BOUND");
        }

        var expected = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (int cycle in new[] { 1, 2 })
        {
            foreach (string surface in _surfaces)
            {
                expected.Add(item: Key(surface: surface, cycle: cycle));
            }
        }

        expected.UnionWith(other: ["late-work:1", "worker-rejected:1", "main-thread:2"]);
        var seen = new HashSet<string>(comparer: StringComparer.Ordinal);
        var instances = new Dictionary<int, ulong>();
        var threads = new HashSet<int>();
        foreach (string line in output.Split(separator: '\n').Where(predicate: line => line.StartsWith(
                value: "PHASE1_SURFACE ", comparisonType: StringComparison.Ordinal)))
        {
            (string key, int cycle, ulong instance, int thread) = Read(line: line, nonce: nonce);
            threads.Add(item: thread);
            if (!instances.TryAdd(key: cycle, value: instance) && instances[cycle] != instance)
            {
                throw new InvalidOperationException(message: "ENGINE_INSTANCE_MISMATCH");
            }

            if (!expected.Contains(item: key) || !seen.Add(item: key))
            {
                throw new InvalidOperationException(message: "ENGINE_SURFACE_INVALID: " + key);
            }
        }

        if (!seen.SetEquals(other: expected) || threads.Count != 1 || threads.Single() <= 0
                || instances.Count != 2 || instances[1] == instances[2])
        {
            throw new InvalidOperationException(message: "ENGINE_SURFACE_MISSING: " + string.Join(separator: ",",
                    values: expected.Except(second: seen, comparer: StringComparer.Ordinal)));
        }
    }

    private static readonly string[] _surfaces = ["construction", "callbacks", "process", "signals", "teardown",
        "resource", "exports", "null-validation", "binding"];

    private static (string Key, int Cycle, ulong Instance, int Thread) Read(string line, string nonce)
    {
        using var document = JsonDocument.Parse(json: line[15..]);
        JsonElement row = document.RootElement;
        int cycle = row.GetProperty(propertyName: "cycle").GetInt32();
        ulong instance = row.GetProperty(propertyName: "instance").GetUInt64();
        if (row.EnumerateObject().Count() != 5 || instance == 0 || !StringComparer.Ordinal.Equals(
                x: row.GetProperty(propertyName: "nonce").GetString(), y: nonce))
        {
            throw new InvalidOperationException(message: "ENGINE_SURFACE_INVALID");
        }

        return (Key(surface: row.GetProperty(propertyName: "surface").GetString()
                ?? throw new InvalidOperationException(message: "ENGINE_SURFACE_REQUIRED"), cycle: cycle),
                cycle, instance, row.GetProperty(propertyName: "thread").GetInt32());
    }

    private static string Key(string surface, int cycle)
    {
        return surface + ":" + cycle.ToString(provider: CultureInfo.InvariantCulture);
    }
}
