using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using Shouldly;
using Xunit;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies external target, stash paging and access-loss presentation decisions.</summary>
[Trait("Tier", "1")]
public sealed class Phase06ExternalInteractionTests
{
    /// <summary>Page requests use the committed token, preserve identity and reject malformed input.</summary>
    [Fact]
    public void Phase06_ExternalPagerUsesCurrentTokenAndRejectsBadInput()
    {
        FoundationSession session = Start();
        RunInventory before = session.Inventory;
        int newPage = before.StashPageCount + 1;
        string pageText = newPage.ToString(provider: CultureInfo.InvariantCulture);
        session.TryEditInventory(edit: (current, _, _) => new RunInventory(items: current.Items,
                stashGrid: current.StashGrid, stashPageCount: newPage, nextItemId: current.NextItemId)).ShouldBeTrue();
        Dispatch(session: session, sequence: 1, command: Model.SelectTarget(target: _stash))
                .ShouldBe(expected: "Ready");
        JsonElement header = Scope(session: session, scope: "external").header;
        int page = header.GetProperty(propertyName: "Page").GetInt32();
        int total = header.GetProperty(propertyName: "PageCount").GetInt32();
        header.GetProperty(propertyName: "Width").GetInt32().ShouldBe(expected: 10);
        header.GetProperty(propertyName: "Height").GetInt32().ShouldBe(expected: 10);
        foreach (string invalid in new[] { "", "1", "0", "500", "-1", "2.0", " 2", "２",
                new string(c: '9', count: 400) })
        {
            Model.PageRequest(text: invalid, current: page, total: total, token: 1).ShouldBeNull();
        }

        Model.PageRequest(text: pageText, current: page, total: total, token: 0).ShouldBeNull();
        Model.Command next = Model.PageRequest(text: pageText, current: page, total: total, token: 1).ShouldNotBeNull();
        Dispatch(session: session, sequence: 2, command: next).ShouldBe(expected: "Ready");
        Scope(session: session, scope: "external").rows.ShouldBeEmpty();
        Dispatch(session: session, sequence: 3, command: Model.PageRequest(text: "1", current: newPage, total: total,
                token: 1).ShouldNotBeNull()).ShouldBe(expected: "AccessLost");
        Dispatch(session: session, sequence: 4, command: Model.PageRequest(text: "1", current: newPage, total: total,
                token: 2).ShouldNotBeNull()).ShouldBe(expected: "Ready");
        Scope(session: session, scope: "external").rows.Length.ShouldBeGreaterThan(expected: 0);
        session.Inventory.Items.ShouldBe(expected: before.Items);
    }

    /// <summary>Equal-label permitted targets stay distinct and a claimed survivor loses the old scope.</summary>
    [Fact]
    public void Phase06_NearbySelectionsUseStablePermittedIdentities()
    {
        FoundationSession session = Start();
        (string kind, string id) first = ("Npc", "survivor-02");
        (string kind, string id) second = ("Npc", "survivor-03");
        ImmutableArray<(string kind, string id)> nearby =
                Model.Nearby(rows: Scope(session: session, scope: "nearby").rows);
        nearby.ShouldContain(expected: first);
        nearby.ShouldContain(expected: second);
        nearby.ShouldNotContain(expected: ("Npc", Owner));
        Dispatch(session: session, sequence: 1, command: Model.SelectTarget(target: first)).ShouldBe(expected: "Ready");
        Model.ExternalTarget(header: Scope(session: session, scope: "external").header).ShouldBe(expected: first);
        session.AddParticipant(bindingId: "guest", playerId: "guest-player", actorId: "guest-actor", label: "Guest")
                .ShouldBeTrue();
        session.MarkReady(bindingId: "guest").ShouldBeTrue();
        Model.Nearby(rows: Scope(session: session, scope: "nearby").rows).ShouldNotContain(expected: first);
        Views(session: session).Any(predicate: view => view.scope is "external").ShouldBeFalse();
        Dispatch(session: session, sequence: 2, command: Model.SelectTarget(target: second))
                .ShouldBe(expected: "Ready");
        Model.ExternalTarget(header: Scope(session: session, scope: "external").header).ShouldBe(expected: second);
    }

    /// <summary>Access loss closes exterior descendants and cancels affected previews without moving items.</summary>
    [Fact]
    public void Phase06_ExternalRevocationKeepsIndependentOwnWindows()
    {
        FoundationSession session = Start();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        Model.Item personal = Model.ReadItem(row: Scope(session: session, scope: "own").rows.Single(predicate: row =>
                row.GetProperty(propertyName: "Id").GetInt64() == bag.Id));
        Dispatch(session: session, sequence: 1, command: Model.SelectTarget(target: _stash))
                .ShouldBe(expected: "Ready");
        Model.Item nearby = Scope(session: session, scope: "external").rows.Select(selector: Model.ReadItem)
                .First(predicate: item => item.StorageWidth.HasValue);
        RunInventory before = session.Inventory;
        var workspace = new Model();
        var bounds = new Model.Rect(x: 24, y: 112, width: 1232, height: 528);
        workspace.Show(external: true);
        workspace.OpenBag(item: personal, parent: null, external: false, source: bounds, bounds: bounds)
                .ShouldNotBeNull();
        workspace.OpenBag(item: nearby, parent: null, external: true, source: bounds, bounds: bounds).ShouldNotBeNull();
        workspace.Begin(item: personal, external: false, token: 0, split: false).ShouldBeTrue();
        workspace.AccessLost(keepExternal: true, aimedExternal: false);
        workspace.Drag.ShouldNotBeNull();
        workspace.AccessLost(keepExternal: false, aimedExternal: true);
        workspace.Drag.ShouldBeNull();
        workspace.Open.ShouldBeTrue();
        workspace.External.ShouldBeFalse();
        workspace.Windows.Single().Item.Id.ShouldBe(expected: personal.Id);
        session.Inventory.ShouldBeSameAs(expected: before);
    }

    /// <summary>Quick transfer projects an effect only for the matching action and current stash page.</summary>
    [Fact]
    public void Phase06_DepositEffectUsesTheSelectedStashAndExactAction()
    {
        FoundationSession session = Start();
        RunInventory.Item item = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand");
        Model.Item personal = Model.ReadItem(row: Scope(session: session, scope: "own").rows.Single(predicate: row =>
                row.GetProperty(propertyName: "Id").GetInt64() == item.Id));
        Dispatch(session: session, sequence: 1, command: Model.SelectTarget(target: _stash))
                .ShouldBe(expected: "Ready");
        Dispatch(session: session, sequence: 2, command: Model.Quick(item: personal, token: 1, deposit: true))
                .ShouldBe(expected: "Applied");
        JsonElement header = Scope(session: session, scope: "external").header;
        Model.DepositHighlight(header: header, target: _stash, sequence: 1).ShouldBeNull();
        Model.DepositHighlight(header: header, target: ("Stash", "other"), sequence: 2).ShouldBeNull();
        (ImmutableArray<(int x, int y)> cells, int more) effect = Model.DepositHighlight(
                header: header, target: _stash, sequence: 2).ShouldNotBeNull();
        RunInventory.StashCell location = session.Inventory.Find(itemId: item.Id).ShouldNotBeNull().Location
                .ShouldBeOfType<RunInventory.StashCell>();
        effect.cells.ShouldContain(expected: (location.X, location.Y));
        effect.more.ShouldBe(expected: 0);
    }

    /// <summary>Quick withdrawal follows the actual carried merge order and leaves both hands empty.</summary>
    [Theory]
    [InlineData("Stash", RunWorld.StashId)]
    [InlineData("Npc", "survivor-02")]
    [InlineData("Corpse", "nearby-corpse")]
    [InlineData("Pile", "nearby-pile")]
    public void Phase06_QuickWithdrawUsesActualCarryWithoutEquipping(string kind, string targetId)
    {
        FoundationSession session = Start();
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        RunDefinitions.Item definition = session.Definitions.Items["IT-08"];
        var carried = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: definition.Id,
                quantity: 1, condition: null, location: RunItemTransfer.Container(definition: definition,
                        containerId: bag.Id, x: 0, y: 0), storageGrid: null);
        session.TryEditInventory(edit: (inventory, _, _) =>
                inventory.WithItems(items: inventory.Items.Add(item: carried), nextItemId: carried.Id + 1))
                .ShouldBeTrue();
        long? container = SeedExternalBerries(session: session, kind: kind, targetId: targetId);
        Dispatch(session: session, sequence: 1, command: Model.SelectTarget(target: (kind, targetId)))
                .ShouldBe(expected: "Ready");
        Model.Item selected = Scope(session: session, scope: "external", container: container)
                .rows.Select(selector: Model.ReadItem)
                .First(predicate: entry => entry.Definition is "IT-08");
        RunInventory before = session.Inventory;
        Dispatch(session: session, sequence: 2, command: Model.Quick(item: selected, token: 400, deposit: false))
                .ShouldBe(expected: "AccessLost");
        session.Inventory.ShouldBeSameAs(expected: before);
        Dispatch(session: session, sequence: 3, command: Model.Quick(item: selected, token: 1, deposit: false))
                .ShouldBe(expected: "Applied");
        session.Inventory.Find(itemId: carried.Id)?.Quantity.ShouldBeGreaterThan(expected: 1);
        session.Inventory.Equipped(survivorId: Owner, slot: "left-hand").ShouldBeNull();
        session.Inventory.Equipped(survivorId: Owner, slot: "right-hand").ShouldBeNull();
    }

    private const string Owner = Phase06MeleeTests.Owner;
    private static readonly (string kind, string id) _stash = ("Stash", RunWorld.StashId);

    private static FoundationSession Start()
    {
        var stash = new RunWorld.Anchor(id: RunWorld.StashId, contact: new RunWorld.Point(x: -5500, y: 0),
                kind: RunWorld.Access.Stash);
        var corpse = new RunWorld.Anchor(id: "nearby-corpse", contact: new RunWorld.Point(x: -7900, y: 0),
                kind: RunWorld.Access.Corpse);
        var pile = new RunWorld.Anchor(id: "nearby-pile", contact: new RunWorld.Point(x: -7800, y: 0),
                kind: RunWorld.Access.Pile);
        var world = new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: RunWorld.DefaultRanges, defenses: [], blockers: [], hostiles: [],
                anchors: [stash, corpse, pile]);
        return Phase06MeleeTests.Start(world: world);
    }

    private static long? SeedExternalBerries(FoundationSession session, string kind, string targetId)
    {
        if (kind is "Stash")
        {
            return null;
        }

        RunInventory.Item from = session.Inventory.Items.First(predicate: item => item.DefinitionId is "IT-08"
                && item.Location is RunInventory.StashCell);
        RunInventory.ItemPath source = session.Inventory.PathTo(itemId: from.Id).ShouldNotBeNull();
        if (kind is "Npc")
        {
            RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack",
                    owner: targetId);
            RunInventory.ContainerCell cell = RunItemTransfer.Container(definition: session.Definitions.Items["IT-08"],
                    containerId: bag.Id, x: 0, y: 0);
            session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Move(inventory: inventory,
                    expected: source, destination: cell)).ShouldBeTrue();
            return bag.Id;
        }

        session.TryEditInventory(edit: (inventory, _, _) => RunItemTransfer.Drop(inventory: inventory,
                expected: source, pileId: targetId)).ShouldBeTrue();
        return null;
    }

    private static (JsonElement header, ImmutableArray<JsonElement> rows) Scope(FoundationSession session, string scope,
            long? container = null)
    {
        (_, _, _, JsonElement header, ImmutableArray<JsonElement> rows) = Views(session: session, container: container)
                .Single(predicate: entry => StringComparer.Ordinal.Equals(x: entry.scope, y: scope));
        return (header, rows);
    }

    private static ImmutableArray<(string scope, long generation, long token, JsonElement header,
            ImmutableArray<JsonElement> rows)> Views(FoundationSession session, long? container = null)
    {
        return session.ReadRunViews(identity: Identity(session: session), externalGrid: container)?.views
                ?? throw new InvalidOperationException(message: "WORKSPACE");
    }

    private static string Dispatch(FoundationSession session, long sequence, Model.Command command)
    {
        return session.DispatchRunAction(identity: Identity(session: session), key: (Owner, 1, sequence),
                kind: command.Kind, request: command.Request).status;
    }

    private static (string session, string binding, string player) Identity(FoundationSession session)
    {
        return (session.Snapshot.SessionId, "local", "host");
    }
}
