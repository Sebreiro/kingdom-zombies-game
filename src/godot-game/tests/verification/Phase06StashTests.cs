using System.Collections.Immutable;
using System.Linq;
using KingdomZombies.Core;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Proves the production stash's placement, navigation and transfer outcomes.</summary>
[Trait("Tier", "1")]
public sealed class Phase06StashTests
{
    /// <summary>Fresh setup chooses an earlier page that can fit the item legally rotated.</summary>
    [Fact]
    public void Phase06_E070_StartupUsesTheEarliestFittingOrientation()
    {
        FoundationSession session = Start(stacks:
                [Stack(id: "IT-01", quantity: 4, count: 9), Stack(id: "IT-07"),
                    Stack(id: "IT-04")]);
        RunInventory.Item sword = session.Inventory.Items.Single(predicate: item => item.DefinitionId is "IT-04");
        var cell = (RunInventory.StashCell)sword.Location;
        (cell.Page, cell.X, cell.Y, cell.Rotated).ShouldBe(expected: (0, 1, 2, true));
    }

    private static FoundationSession Start(ImmutableArray<RunDefinitions.StartingStack> stacks)
    {
        RunDefinitions baseline = RunDefinitions.Default;
        var settings = new RunDefinitions.InventorySettings(stashGrid: new RunDefinitions.Size(width: 4, height: 4),
                maximumContainerLevels: 3, quickSlotCount: 4, localUnits: 3, stashUnits: 4);
        var start = new RunDefinitions.Start(survivorCount: 8, playerLimit: 4, role: "unassigned",
                outfit: baseline.StartingOutfit, stash: stacks);
        var survivors = new RunDefinitions.SurvivorSettings(maximumHealth: 100, maximumStamina: 100,
                refugeUnits: 12, dangerUnits: 12);
        var definitions = new RunDefinitions(start: start, survivors: survivors, inventory: settings,
                items: baseline.Items);
        return new FoundationSession(sessionId: "stash-session", playerId: "host", actorId: "actor",
                world: RunWorld.Foundation, definitions: definitions);
    }

    private static RunDefinitions.StartingStack Stack(string id, int quantity = 1, int count = 1)
    {
        return new RunDefinitions.StartingStack(itemId: id, quantity: quantity, count: count);
    }
}
