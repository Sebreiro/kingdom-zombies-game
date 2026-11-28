using System.Collections.Immutable;
using System.Text.Json;
using KingdomZombies.Core;

namespace KingdomZombies.Foundation;

/// <summary>Gives the native consumer one detached roster, render projection and permitted view read.</summary>
internal sealed class FoundationRunReadback
{
    internal FoundationRunReadback(FoundationSnapshot roster, JsonElement? world,
            ImmutableDictionary<string, FoundationRunView> views, (long sequence, string status, bool replayed)?
                    feedback)
    {
        Roster = roster;
        World = world;
        Views = views;
        Feedback = feedback;
    }

    internal FoundationSnapshot Roster { get; }
    internal JsonElement? World { get; }
    internal ImmutableDictionary<string, FoundationRunView> Views { get; }
    internal (long sequence, string status, bool replayed)? Feedback { get; }
}
