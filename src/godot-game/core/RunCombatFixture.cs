#if DEBUG
using System.IO;
using System.Text.Json;

namespace KingdomZombies.Core;

/// <summary>Builds independent stationary combat runs for owned Phase 6 observation scenarios.</summary>
internal static class RunCombatFixture
{
    internal const string Owner = "survivor-01";
    internal const string First = "fixture.melee-a";
    internal const string Second = "fixture.melee-b";
    internal const string Bowman = "fixture.bow";
    internal const string Remains = "fixture.supplies";
    internal const string Empty = "fixture.empty";

    internal static FoundationSession Create(string mode, string sessionId, string playerId, string actorId,
            RunDefinitions definitions)
    {
        if (mode is not ("field" or "bow" or "life" or "far"))
        {
            throw new InvalidDataException(message: "FIXTURE_MODE");
        }

        var session = new FoundationSession(sessionId: sessionId, playerId: playerId, actorId: actorId,
                world: World(ranges: definitions.Ranges), definitions: definitions, fixtureMode: mode);
        Require(applied: session.TryAddHostile(id: First, position: -7110, facing: -1,
                health: definitions.MaximumHealth));
        Require(applied: session.TryAddHostile(id: Second, position: -7110, facing: -1,
                health: definitions.MaximumHealth));
        Require(applied: session.TryAddHostile(id: Bowman, position: 4500, facing: -1,
                health: definitions.MaximumHealth));
        Require(applied: session.TryEditInventory(edit: (inventory, _, _) => Kit(inventory: inventory,
                definitions: definitions, mode: mode)));
        if (mode is "life" or "far")
        {
            Require(applied: session.TryUpdateVitals(survivorId: Owner, health: 1,
                    stamina: definitions.MaximumStamina));
        }
        if (mode is "far")
        {
            Require(applied: session.PlaceFixtureSuccessors(position: 30000));
        }

        return session;
    }

    internal static (string status, bool replayed) Trigger(FoundationSession session,
            FoundationSession.RunPublication root, RunItemActions.Controller controller,
            long sequence, JsonElement request)
    {
        if (session.FixtureMode is null || controller.BindingId is not "local" || sequence <= 0)
        {
            return ("AccessLost", false);
        }

        string? name = request.GetProperty(propertyName: "Target").GetString();
        if (name is not (First or Second or Bowman))
        {
            return ("InvalidAction", false);
        }

        RunCombat.Aim? aim = name is Bowman ? Aim(root: root) : null;
        RunCombat.Outcome outcome = session.AttackUncontrolled(survivorId: name, sequence: sequence, aim: aim);
        return (outcome.Status.ToString(), outcome.Replayed);
    }

    private static RunWorld World(RunWorld.Ranges ranges)
    {
        return new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default, ranges: ranges,
                defenses:
                [
                    new RunWorld.Defense(id: "fixture.living-wall", contact: new RunWorld.Point(x: 2500, y: 0),
                            completed: true, living: true, isWall: true, health: 12),
                    new RunWorld.Defense(id: "fixture.broken-wall", contact: new RunWorld.Point(x: 1000, y: 0),
                            completed: true, living: false, isWall: true),
                ],
                blockers:
                [
                    new RunWorld.Blocker(id: "fixture.raised-terrain", minimum: new RunWorld.Point(x: -5550, y: 1200),
                            maximum: new RunWorld.Point(x: -5300, y: 3500)),
                ], hostiles: [], anchors:
                [
                    new RunWorld.Anchor(id: RunWorld.StashId, contact: new RunWorld.Point(x: -6100, y: 0),
                            kind: RunWorld.Access.Stash),
                    new RunWorld.Anchor(id: Remains, contact: new RunWorld.Point(x: -7450, y: 0),
                            kind: RunWorld.Access.Pile),
                    new RunWorld.Anchor(id: Empty, contact: new RunWorld.Point(x: -7750, y: 0),
                            kind: RunWorld.Access.Pile),
                ]);
    }

    private static RunInventory Kit(RunInventory inventory, RunDefinitions definitions, string mode)
    {
        bool bow = mode is "bow";
        Equip(inventory: ref inventory, definitions: definitions, owner: Owner, slot: "left-hand",
                id: bow ? "IT-07" : "IT-03", condition: mode is "field" ? 1 : null);
        if (bow)
        {
            Equip(inventory: ref inventory, definitions: definitions, owner: Owner, slot: "right-hand", id: "IT-02");
        }

        long bag = Equip(inventory: ref inventory, definitions: definitions, owner: Owner, slot: "backpack",
                id: "IT-23");
        _ = Equip(inventory: ref inventory, definitions: definitions, owner: Owner, slot: "body-armor", id: "IT-19");
        _ = Equip(inventory: ref inventory, definitions: definitions, owner: First, slot: "left-hand", id: "IT-04");
        _ = Equip(inventory: ref inventory, definitions: definitions, owner: Bowman, slot: "left-hand", id: "IT-07");
        _ = Equip(inventory: ref inventory, definitions: definitions, owner: Bowman, slot: "right-hand", id: "IT-02",
                quantity: 3);
        long pouch = Store(inventory: ref inventory, definitions: definitions, id: "IT-22", grid: bag, x: 0, y: 0);
        _ = Store(inventory: ref inventory, definitions: definitions, id: "IT-08", grid: pouch, x: 0, y: 0,
                quantity: 6);
        _ = Store(inventory: ref inventory, definitions: definitions, id: "IT-02", grid: bag, x: 1, y: 0,
                quantity: 9);
        _ = Store(inventory: ref inventory, definitions: definitions, id: "IT-07", grid: bag, x: 2, y: 0);
        _ = Add(inventory: ref inventory, definition: definitions.Items["IT-01"], quantity: 17, condition: null,
                location: new RunInventory.Ground(pileId: Remains));
        return inventory;
    }

    private static long Equip(ref RunInventory inventory, RunDefinitions definitions, string owner, string slot,
            string id, decimal? condition = null, int quantity = 1)
    {
        return Add(inventory: ref inventory, definition: definitions.Items[id], quantity: quantity,
                condition: condition, location: new RunInventory.Equipment(survivorId: owner, slot: slot));
    }

    private static long Store(ref RunInventory inventory, RunDefinitions definitions, string id, long grid, int x,
            int y, int quantity = 1)
    {
        RunDefinitions.Item definition = definitions.Items[id];
        return Add(inventory: ref inventory, definition: definition, quantity: quantity, condition: null,
                location: RunItemTransfer.Container(definition: definition, containerId: grid, x: x, y: y));
    }

    private static long Add(ref RunInventory inventory, RunDefinitions.Item definition, int quantity,
            decimal? condition, RunInventory.Location location)
    {
        long id = inventory.NextItemId;
        var item = new RunInventory.Item(id: id, definitionId: definition.Id, quantity: quantity,
                condition: condition ?? definition.MaximumCondition, location: location,
                storageGrid: definition.Storage);
        inventory = inventory.WithItems(items: inventory.Items.Add(item: item), nextItemId: id + 1);
        return id;
    }

    private static RunCombat.Aim? Aim(FoundationSession.RunPublication root)
    {
        if (root.Population.FindLivingControl(bindingId: "local") is not { } lease)
        {
            return null;
        }

        (decimal x, decimal y) = RunCombat.Contact(world: root.World, survivor: root.Population.ForLease(lease: lease));
        return new RunCombat.Aim(x: (double)x, y: (double)y + 500);
    }

    private static void Require(bool applied)
    {
        if (!applied)
        {
            throw new InvalidDataException(message: "FIXTURE_SETUP");
        }
    }
}
#endif
