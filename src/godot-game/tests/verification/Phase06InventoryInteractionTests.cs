using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    public void PreviewCancellation_OutsideReleaseOrClose_PreservesCommittedSource()
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
    public void InventoryPreview_RotatedOrOccupied_ChecksCellsAndManualReplacement()
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
    public void SplitPreview_CurrentStack_BoundsQuantity()
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
    public void BagWindows_ParentClosed_CloseAndRememberClampedPosition()
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
    public void InventoryWindows_QuickAssignment_UseHostAuthority()
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

    /// <summary>
    /// Models the rendered reopen contract on a real solo coordinator: the closed workspace drew its windows, the
    /// reopened one draws before they return, and the returning windows are byte-identical to the earlier drawing yet
    /// still require a new projection. The test records each draw itself; the workspace wiring is bound separately.
    /// </summary>
    [Fact]
    public void ProjectionVersion_ReopenedBeforeIdenticalWindowsReturn_IsStale()
    {
        var solo = new FoundationCoordinator(playerId: "p1", label: "Host");
        solo.Start(host: false, fixture: "field");
        solo.Update(elapsedMilliseconds: 0);
        ImmutableArray<long> storage = [.. solo.ReadRunRender().ShouldNotBeNull().Views["own"].Rows
                .Select(selector: Model.ReadItem).Where(predicate: item => item.StorageWidth.HasValue
                        && item.Location.Kind is "equipment").Select(selector: item => item.Id)];
        storage.ShouldNotBeEmpty();
        var workspace = new Model();
        workspace.Show(external: false);
        solo.RequestRunWorkspace(own: storage, external: []).ShouldBeTrue();
        string drawn = Model.Version(readback: solo.ReadRunRender());
        workspace.Projected(version: drawn);
        workspace.Stale(version: drawn).ShouldBeFalse();
        workspace.Close();
        solo.RequestRunWorkspace(own: [], external: []).ShouldBeTrue();
        string closed = Model.Version(readback: solo.ReadRunRender());
        StringComparer.Ordinal.Equals(x: closed, y: drawn).ShouldBeFalse();
        workspace.Show(external: false);
        workspace.Projected(version: closed);
        solo.RequestRunWorkspace(own: storage, external: []).ShouldBeTrue();
        string returned = Model.Version(readback: solo.ReadRunRender());
        returned.ShouldBe(expected: drawn);
        workspace.Stale(version: returned).ShouldBeTrue();
        workspace.Projected(version: returned);
        workspace.Stale(version: returned).ShouldBeFalse();
    }

    /// <summary>Closing or resetting forgets the drawn version, so the first draw after reopening projects.</summary>
    [Fact]
    public void ProjectionVersion_ClosedOrReset_ForgetsDrawnVersion()
    {
        var workspace = new Model();
        workspace.Stale(version: "own0{}").ShouldBeTrue();
        workspace.Show(external: false);
        workspace.Projected(version: "own0{}");
        workspace.Stale(version: "own0{}").ShouldBeFalse();
        workspace.Close();
        workspace.Stale(version: "own0{}").ShouldBeTrue();
        workspace.Show(external: false);
        workspace.Projected(version: "own0{}");
        workspace.Reset();
        workspace.Stale(version: "own0{}").ShouldBeTrue();
    }

    /// <summary>
    /// Binds the workspace wiring the projection tests model: only Draw records a version, as its last projection
    /// step, and Observe reprojects dirty or stale readback. Solo, host and guest readback all reach the workspace
    /// through this one Observe, so the binding covers each of them without an engine.
    /// </summary>
    [Fact]
    public void ProjectionWiring_WorkspaceSource_RecordsEachDrawAndReprojectsStaleReadback()
    {
        MethodDeclarationSyntax[] methods = [.. Directory.GetFiles(path: Path.Combine(path1: ProofProtocol.Root,
                path2: "src/godot-game/game/foundation"), searchPattern: "FoundationInventoryWorkspace*.cs")
                .SelectMany(selector: file => CSharpSyntaxTree.ParseText(text: File.ReadAllText(path: file),
                        cancellationToken: TestContext.Current.CancellationToken).GetRoot(
                                cancellationToken: TestContext.Current.CancellationToken).DescendantNodes()
                        .OfType<MethodDeclarationSyntax>())];
        methods.Where(predicate: method => method.DescendantNodes().OfType<InvocationExpressionSyntax>().Any(
                predicate: call => call.Expression.ToString() is "_model.Projected")).Select(
                        selector: method => method.Identifier.ValueText).ShouldBe(expected: ["Draw"]);
        methods.Single(predicate: method => method.Identifier.ValueText is "Draw").Body.ShouldNotBeNull().Statements
                .TakeLast(count: 2).Select(selector: statement => statement.ToString()).ShouldBe(expected:
                        ["_model.Projected(version: Model.Version(readback: _readback));", "_dirty = false;"]);
        IfStatementSyntax reproject = methods.Single(predicate: method => method.Identifier.ValueText is "Observe")
                .Body.ShouldNotBeNull().Statements.OfType<IfStatementSyntax>().Last();
        reproject.Condition.ToString().ShouldBe(expected: "_dirty || _model.Stale(version: Model.Version("
                + "readback: _readback))");
        reproject.Statement.DescendantNodes().OfType<ExpressionStatementSyntax>().Select(
                selector: statement => statement.ToString()).ShouldBe(expected: ["Project();"]);
    }

    /// <summary>
    /// A Ctrl or Shift key held across a physical click counts when the platform click carries no modifier flag, as
    /// in the rendered Ctrl-click withdrawal whose acknowledged Ctrl press submitted no quick request.
    /// </summary>
    [Fact]
    public void Modifier_HeldWithoutEventFlag_IsActive()
    {
        Model.Modifier(flagged: false, held: true).ShouldBeTrue();
        Model.Modifier(flagged: true, held: false).ShouldBeTrue();
        Model.Modifier(flagged: true, held: true).ShouldBeTrue();
        Model.Modifier(flagged: false, held: false).ShouldBeFalse();
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
