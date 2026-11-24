using System.Collections.Immutable;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Gives the native consumer one detached roster, render projection and permitted view read.</summary>
internal sealed class FoundationRunReadback
{
    internal FoundationRunReadback(FoundationSnapshot roster, JsonElement? world,
            ImmutableDictionary<string, FoundationRunView> views)
    {
        Roster = roster;
        World = world;
        Views = views;
    }

    internal FoundationSnapshot Roster { get; }
    internal JsonElement? World { get; }
    internal ImmutableDictionary<string, FoundationRunView> Views { get; }
}
