using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using Shouldly;
using Xunit;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies committed source identity, purely local previews and actual owned grid windows.</summary>
[Trait("Tier", "1")]
public sealed class Phase06InventoryInteractionTests
{
    /// <summary>Outside release and closing never produce a request or alter a real carried source.</summary>
    [Fact]
    public void Phase06_PreviewCancellationPreservesTheCommittedSource()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        RunInventory.Item equipped = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand");
        Model.Item item = Read(session: session, id: equipped.Id);
        RunInventory before = session.Inventory;
        var workspace = new Model();
        workspace.Show(external: false);
        workspace.Begin(item: item, external: false, token: 0, split: false).ShouldBeTrue();
        workspace.Release(placement: new Model.Placement(reason: "Outside")).ShouldBeNull();
        workspace.Drag.ShouldBeNull();
        workspace.Begin(item: item, external: false, token: 0, split: false).ShouldBeTrue();
        workspace.Close();
        workspace.Drag.ShouldBeNull();
        session.Inventory.ShouldBeSameAs(expected: before);
        session.Inventory.Find(itemId: equipped.Id).ShouldBeSameAs(expected: equipped);
    }

    /// <summary>Rotation uses the full authored footprint and an occupied hand never invites a swap.</summary>
    [Fact]
    public void Phase06_InventoryPreviewChecksRotatedCellsAndManualReplacement()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        Model.Item knife = Read(session: session, id: Phase06MeleeTests.Equip(
                session: session, id: "IT-03", slot: "left-hand").Id);
        Model.Item club = Read(session: session, id: Phase06MeleeTests.Equip(
                session: session, id: "IT-04", slot: "right-hand").Id);
        var workspace = new Model();
        workspace.Show(external: false);
        workspace.Begin(item: knife, external: false, token: 0, split: false).ShouldBeTrue();
        workspace.CheckSlot(slot: "right-hand", owner: Phase06MeleeTests.Owner, kind: "equipment",
                occupied: club, token: 0).Reason.ShouldBe(expected: "Occupied");
        var grid = new Model.Grid(kind: "stash", id: 0, width: 2, height: 1, container: null, token: 41,
                items: [], ancestors: []);
        workspace.CheckGrid(grid: grid, x: 0, y: 0).Action.ShouldBeNull();
        workspace.Rotate().ShouldBeTrue();
        Model.Placement preview = workspace.CheckGrid(grid: grid, x: 0, y: 0);
        preview.Area.ShouldBe(expected: new Model.Rect(x: 0, y: 0, width: 2, height: 1));
        Model.Command command = workspace.Release(placement: preview).ShouldNotBeNull();
        command.Kind.ShouldBe(expected: "place");
        command.Request.GetProperty(propertyName: "Destination").GetProperty(propertyName: "Rotated")
                .GetBoolean().ShouldBeTrue();
        command.Request.GetProperty(propertyName: "Token").GetInt64().ShouldBe(expected: 41);
    }

    /// <summary>The split preview validates whole quantities and preserves the exact source stamp.</summary>
    [Fact]
    public void Phase06_SplitPreviewBoundsAndQuantityUseTheCurrentStack()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bag, RunInventory.Item food) = WithFood(session: session);
        var workspace = new Model();
        workspace.Show(external: false);
        Model.Item carried = Read(session: session, id: food.Id, grid: bag.Id);
        workspace.Begin(item: carried, external: false, token: 0, split: true).ShouldBeTrue();
        workspace.Amount(text: "2.5").ShouldBeFalse();
        workspace.Amount(text: "5").ShouldBeFalse();
        workspace.Amount(text: "2").ShouldBeTrue();
        var grid = new Model.Grid(kind: "container", id: bag.Id, width: 4, height: 4,
                container: Read(session: session, id: bag.Id), token: 0, items: [carried], ancestors: [bag.Id]);
        Model.Command action = workspace.Release(placement: workspace.CheckGrid(grid: grid, x: 1, y: 0))
                .ShouldNotBeNull();
        action.Request.GetProperty(propertyName: "Amount").GetInt32().ShouldBe(expected: 2);
        action.Request.GetProperty(propertyName: "Item").GetProperty(propertyName: "Stamp").GetString()
                .ShouldBe(expected: carried.Stamp);
        session.Inventory.Find(itemId: food.Id)?.Quantity.ShouldBe(expected: 5);
    }

    /// <summary>Nested close cascades and reopen restores only the remembered position of the manual opening.</summary>
    [Fact]
    public void Phase06_BagWindowsCloseByParentAndRememberClampedPosition()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bag, RunInventory.Item food) = WithFood(session: session);
        Model.Item outer = Read(session: session, id: bag.Id);
        Model.Item inner = Read(session: session, id: food.Id, grid: bag.Id) with
        {
            StorageWidth = 2,
            StorageHeight = 2,
        };
        var workspace = new Model();
        workspace.Show(external: false);
        var bounds = new Model.Rect(x: 24, y: 112, width: 1232, height: 528);
        var origin = new Model.Rect(x: 384, y: 200, width: 128, height: 128);
        workspace.OpenBag(item: outer, parent: null, external: false, source: origin, bounds: bounds)
                .ShouldNotBeNull();
        workspace.OpenBag(item: inner, parent: outer.Id, external: false, source: origin, bounds: bounds)
                .ShouldNotBeNull();
        Model.Rect moved = workspace.MoveBag(id: outer.Id, x: 5000, y: -10, bounds: bounds).ShouldNotBeNull().Frame;
        moved.Y.ShouldBe(expected: 112);
        moved.X.ShouldBe(expected: bounds.X + bounds.Width - moved.Width);
        workspace.CloseBag(id: outer.Id);
        workspace.Windows.ShouldBeEmpty();
        workspace.Close();
        workspace.Show(external: false);
        workspace.Windows.ShouldBeEmpty();
        workspace.OpenBag(item: outer, parent: null, external: false, source: origin, bounds: bounds)
                .ShouldNotBeNull().Frame.ShouldBe(expected: moved);
    }

    /// <summary>Extra windows remain in the recipient scope and Assign changes only a carried reference.</summary>
    [Fact]
    public void Phase06_InventoryWindowsAndQuickAssignmentUseHostAuthority()
    {
        FoundationSession session = Phase06MeleeTests.Start();
        (RunInventory.Item bag, RunInventory.Item food) = WithFood(session: session);
        RunInventory.Item shirt = session.Inventory.Equipped(survivorId: Phase06MeleeTests.Owner,
                slot: "torso-clothing").ShouldNotBeNull();
        string survivor = Phase06MeleeTests.Owner;
        (string session, string binding, string player) identity = Identity(session: session);
        long opaque = long.MaxValue;
        (_, ImmutableArray<(string scope, long generation, long token, JsonElement header,
                ImmutableArray<JsonElement> rows)> views) = session.ReadRunViews(identity: identity,
                        ownWindows: [bag.Id, shirt.Id, opaque])
                        ?? throw new InvalidOperationException(message: "WORKSPACE");
        views.Select(selector: view => view.scope).ShouldBe(expected: ["status", "nearby", "own", "own.1", "own.2"]);
        Model.Item item = Model.ReadItem(row: views.Single(predicate: view => view.scope is "own.1")
                .rows.Single());
        Model.Command assign = Model.Assign(item: item, slot: 3);
        (string status, _, _) = session.DispatchRunAction(identity: identity, key: (survivor, 1, 1),
                kind: assign.Kind, request: assign.Request);
        status.ShouldBe(expected: "Applied");
        session.Inventory.Find(itemId: food.Id)?.Quantity.ShouldBe(expected: 5);
        session.Inventory.QuickReferences[(survivor, 3)].ShouldBe(expected: food.Id);
    }

    private static (string session, string binding, string player) Identity(FoundationSession session)
    {
        return (session.Snapshot.SessionId, "local", "host");
    }

    private static Model.Item Read(FoundationSession session, long id, long? grid = null)
    {
        (_, ImmutableArray<(string scope, long generation, long token, JsonElement header,
                ImmutableArray<JsonElement> rows)> views) = session.ReadRunViews(identity: Identity(session: session),
                        ownGrid: grid)
                        ?? throw new InvalidOperationException(message: "WORKSPACE");
        JsonElement row = views.Single(predicate: view => view.scope is "own").rows.Single(
                predicate: item => item.GetProperty(propertyName: "Id").GetInt64() == id);
        return Model.ReadItem(row: row);
    }

    private static (RunInventory.Item bag, RunInventory.Item food) WithFood(FoundationSession session)
    {
        RunInventory.Item bag = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack");
        var food = new RunInventory.Item(id: session.Inventory.NextItemId, definitionId: "IT-08", quantity: 5,
                condition: null, location: RunItemTransfer.Container(definition: session.Definitions.Items["IT-08"],
                        containerId: bag.Id, x: 0, y: 0), storageGrid: null);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(items: inventory.Items.Add(item: food),
                nextItemId: food.Id + 1)).ShouldBeTrue();
        return (bag, food);
    }
}
