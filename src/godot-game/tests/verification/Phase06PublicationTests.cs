using System;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies that normal composition and peer frames share the same run authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06PublicationTests
{
    /// <summary>A current connection can invoke the same scoped gameplay dispatcher as a local player.</summary>
    [Fact]
    public void TrustedGuestAndSoloUseOneDispatcher()
    {
        CheckSolo();
        CheckContestedGuest();
    }

    private static void CheckSolo()
    {
        var solo = new FoundationCoordinator(playerId: "p1", label: "Host");
        solo.Start(host: false);
        Move(coordinator: solo, sender: 0, sequence: 1, direction: 1);
        solo.Update(elapsedMilliseconds: 2000);
        SelectStash(coordinator: solo, sender: 0, sequence: 2);
        JsonElement source = StashBackpack(coordinator: solo, sender: 0);
        string owner = solo.ReadBoundRunControl(sender: 0).ShouldNotBeNull().survivorId;
        Dispatch(coordinator: solo, sender: 0, sequence: 3, kind: "place",
                request: Backpack(source: source, owner: owner, token: 2)).ShouldBe(expected: "Applied");
        HasItem(coordinator: solo, sender: 0, item: ItemId(item: source)).ShouldBeTrue();
    }

    private static void CheckContestedGuest()
    {
        var port = new FoundationHarnessConnection();
        var host = new FoundationCoordinator(playerId: "host", label: "Host", connection: port);
        host.Start(host: true);
        FoundationEnvelope baseline = Admit(host: host, port: port, sender: 2);
        Move(coordinator: host, sender: 0, sequence: 1, direction: 1);
        Move(coordinator: host, sender: 2, sequence: 1, direction: 1);
        host.Update(elapsedMilliseconds: 2000);
        SelectStash(coordinator: host, sender: 0, sequence: 2);
        SelectStash(coordinator: host, sender: 2, sequence: 2);
        JsonElement source = StashBackpack(coordinator: host, sender: 2);
        string guest = host.ReadBoundRunControl(sender: 2).ShouldNotBeNull().survivorId;
        string owner = host.ReadBoundRunControl(sender: 0).ShouldNotBeNull().survivorId;
        Dispatch(coordinator: host, sender: 2, sequence: 3, kind: "place",
                request: Backpack(source: source, owner: guest, token: 2)).ShouldBe(expected: "Applied");
        Dispatch(coordinator: host, sender: 0, sequence: 3, kind: "place",
                request: Backpack(source: source, owner: owner, token: 2)).ShouldBe(expected: "AccessLost");
        long item = ItemId(item: source);
        HasItem(coordinator: host, sender: 2, item: item).ShouldBeTrue();
        HasItem(coordinator: host, sender: 0, item: item).ShouldBeFalse();
        host.ReadRunPublic().ShouldNotBeNull().world.GetRawText().ShouldNotContain(expected: "Quantity");
        port.PeerGenerations[key: 2] = "other";
        host.ReadBoundRunViews(sender: 2).ShouldBeNull();
        host.ReadBoundRunControl(sender: 2).ShouldBeNull();
        host.Snapshot?.Participants.First(predicate: p => StringComparer.Ordinal.Equals(
                x: p.BindingId, y: baseline.BindingId)).Ready.ShouldBe(expected: true);
    }

    private static FoundationEnvelope Admit(FoundationCoordinator host, FoundationHarnessConnection port, int sender)
    {
        port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "hello", playerId: "p" + sender,
                label: "Guest", attemptId: "attempt" + sender));
        host.Update(elapsedMilliseconds: 0);
        FoundationEnvelope baseline = port.Sent.Last(predicate: row => row.Peer == sender
                && row.Envelope.Kind is "baseline").Envelope;
        port.Enqueue(sender: sender, envelope: new FoundationEnvelope(kind: "ack", sessionId: baseline.SessionId,
                playerId: baseline.PlayerId, actorId: baseline.ActorId, bindingId: baseline.BindingId,
                revision: baseline.Revision, attemptId: baseline.AttemptId));
        host.Update(elapsedMilliseconds: 0);
        return baseline;
    }

    private static void Move(FoundationCoordinator coordinator, int sender, long sequence, int direction)
    {
        Dispatch(coordinator: coordinator, sender: sender, sequence: sequence, kind: "move",
                request: JsonSerializer.SerializeToElement(value: new
                {
                    Direction = direction,
                    Sprint = false,
                    Jump = false
                })).ShouldBe(expected: "Accepted");
    }

    private static void SelectStash(FoundationCoordinator coordinator, int sender, long sequence)
    {
        Dispatch(coordinator: coordinator, sender: sender, sequence: sequence, kind: "select",
                request: JsonSerializer.SerializeToElement(value: new
                {
                    TargetKind = "Stash",
                    TargetId = "foundation.stash"
                })).ShouldBe(expected: "Ready");
    }

    private static string Dispatch(FoundationCoordinator coordinator, int sender, long sequence, string kind,
            JsonElement request)
    {
        (string actor, long generation, long _) = coordinator.ReadBoundRunControl(sender: sender).ShouldNotBeNull();
        return coordinator.DispatchBoundRunAction(sender: sender, key: (actor, generation, sequence), kind: kind,
                request: request).status;
    }

    private static JsonElement StashBackpack(FoundationCoordinator coordinator, int sender)
    {
        return coordinator.ReadBoundRunViews(sender: sender).ShouldNotBeNull().views
                .Single(predicate: scope => scope.scope is "external").rows.First(predicate: row =>
                        row.GetProperty(propertyName: "DefinitionId").GetString() is "IT-23");
    }

    private static bool HasItem(FoundationCoordinator coordinator, int sender, long item)
    {
        return coordinator.ReadBoundRunViews(sender: sender).ShouldNotBeNull().views.SelectMany(
                selector: scope => scope.rows).Any(predicate: row => ItemId(item: row) == item);
    }

    private static JsonElement Backpack(JsonElement source, string owner, long token)
    {
        return JsonSerializer.SerializeToElement(value: new
        {
            Item = new { Id = ItemId(item: source), Stamp = source.GetProperty(propertyName: "Stamp").GetString() },
            Token = token,
            Destination = new
            {
                Kind = "equipment",
                TargetId = owner,
                Slot = "backpack",
                GridId = (long?)null,
                X = (int?)null,
                Y = (int?)null,
                Rotated = (bool?)null,
            },
            Container = (object?)null,
            Amount = (decimal?)null,
        });
    }

    private static long ItemId(JsonElement item)
    {
        return item.GetProperty(propertyName: "Id").GetInt64();
    }
}
