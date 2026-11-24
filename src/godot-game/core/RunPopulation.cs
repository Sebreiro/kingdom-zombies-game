using System;
using System.Collections.Immutable;
using System.Linq;

namespace KingdomZombies.Core;

/// <summary>Stores the ordinary recruits independently of transport participation.</summary>
internal sealed class RunPopulation
{
    internal const long StartingPosition = -8000;

    internal RunPopulation(ImmutableArray<Survivor> survivors)
    {
        Survivors = survivors;
    }

    internal ImmutableArray<Survivor> Survivors { get; }

    internal static RunPopulation Create(RunDefinitions definitions)
    {
        return new RunPopulation(survivors: [.. Enumerable.Range(start: 1, count: definitions.SurvivorCount)
                .Select(selector: ordinal => new Survivor(
                        id: FormattableString.Invariant(formattable: $"survivor-{ordinal:D2}"),
                        positionSubunits: StartingPosition, facing: 1, health: definitions.MaximumHealth,
                        stamina: definitions.MaximumStamina, role: definitions.InitialRole))]);
    }

    internal sealed record Survivor
    {
        internal Survivor(string id, long positionSubunits, int facing, decimal health, decimal stamina,
                string role)
        {
            Id = id;
            PositionSubunits = positionSubunits;
            Facing = facing;
            Health = health;
            Stamina = stamina;
            Role = role;
        }

        internal string Id { get; init; }
        internal long PositionSubunits { get; init; }
        internal int Facing { get; init; }
        internal decimal Health { get; init; }
        internal decimal Stamina { get; init; }
        internal string Role { get; init; }
    }
}
