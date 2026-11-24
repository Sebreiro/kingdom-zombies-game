using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
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

    /// <summary>The current compatibility epoch identifies the bounded recipient protocol.</summary>
    [Fact]
    public void GameplayVersionIdentifiesScopedPublicationFrames()
    {
        var hello = new FoundationEnvelope(kind: "hello", playerId: "peer", label: "Guest");
        FoundationProtocol.Decode(bytes: FoundationProtocol.Encode(envelope: hello)).ProtocolVersion
                .ShouldBe(expected: 2);
    }

    /// <summary>The recipient can bind a corrected private readback to its original action.</summary>
    [Fact]
    public void P02_CleanControl_PreservesPrivateAndLocalAccess()
    {
        var room = new Room();
        FoundationCoordinator guest = room.Join(sender: 2);
        FoundationRunView status = guest.ReadRunRender().ShouldNotBeNull().Views["status"];
        status.GetType().GetProperty(name: "Correlation", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)
                .ShouldNotBeNull();
    }

    /// <summary>Real page parts install together and are fenced after the guest loses stash access.</summary>
    [Fact]
    public void LargeScopedPageInstallsAtomicallyAndRevocationFencesDelayedFrames()
    {
        var room = new Room(read: ReadLargeConfig);
        FoundationCoordinator guest = room.Join(sender: 2);
        int page = FillLargePage(session: Session(coordinator: room.Host));
        guest.Submit(direction: 1).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 2000);
        guest.Submit(direction: 0).ShouldBeTrue();
        _ = guest.SubmitRunAction(kind: "select", request: StashSelection()).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        long token = guest.ReadRunRender().ShouldNotBeNull().Views["external"].Token;
        _ = guest.SubmitRunAction(kind: "browse", request:
                JsonSerializer.SerializeToElement(value: new { Token = token, Page = page })).ShouldNotBeNull();
        room.Receive(sender: 2);
        byte[][] parts = HoldExternalPage(room: room, sender: 2);
        parts.Length.ShouldBeGreaterThan(expected: 1);
        parts.Max(selector: bytes => bytes.Length).ShouldBeLessThanOrEqualTo(expected: FoundationProtocol.MaximumBytes);
        room.Send(sender: 2, guest: guest);
        room.ReceivePart(sender: 2, guest: guest, data: parts[^1]);
        room.ReceivePart(sender: 2, guest: guest, data: parts[^1]);
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
        room.Exchange(sender: 2, guest: guest, elapsed: 16);
        foreach (byte[] part in parts.SkipLast(count: 1).Reverse())
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        FoundationRunView open = guest.ReadRunRender().ShouldNotBeNull().Views["external"];
        open.Rows.Length.ShouldBe(expected: 4096);
        open.Rows.Select(selector: ItemId).Distinct().Count().ShouldBe(expected: 4096);
        guest.Submit(direction: -1).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 1200);
        foreach (byte[] part in parts)
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
        guest.Ready.ShouldBeTrue();
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

    private static JsonElement StashSelection()
    {
        return JsonSerializer.SerializeToElement(value: new { TargetKind = "Stash", TargetId = "foundation.stash" });
    }

    private static FoundationSession Session(FoundationCoordinator coordinator)
    {
        FieldInfo field = typeof(FoundationCoordinator).GetField(name: "_session",
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance).ShouldNotBeNull();
        return field.GetValue(obj: coordinator).ShouldBeOfType<FoundationSession>();
    }

    private static byte[] ReadLargeConfig(string path, int limit)
    {
        using Stream stream = typeof(FoundationSession).Assembly.GetManifestResourceStream(name: path)
                ?? throw new InvalidOperationException(message: path);
        using var output = new MemoryStream();
        stream.CopyTo(destination: output);
        byte[] bytes = output.ToArray();
        if (path is not "content/config/inventory.json")
        {
            return bytes;
        }

        JsonNode root = JsonNode.Parse(utf8Json: bytes).ShouldNotBeNull();
        root["stash"].ShouldNotBeNull()["pageGrid"] = new JsonArray(items: [JsonValue.Create(value: 64),
            JsonValue.Create(value: 64)]);
        return JsonSerializer.SerializeToUtf8Bytes(value: root);
    }

    private static int FillLargePage(FoundationSession session)
    {
        RunInventory before = session.Inventory;
        RunDefinitions.Item rule = session.Definitions.Items["IT-01"];
        int index = before.StashPageCount;
        ImmutableArray<RunInventory.Item> filled = [.. before.Items, .. Enumerable.Range(start: 0, count: 4096)
                .Select(selector: cell => new RunInventory.Item(id: before.NextItemId + cell,
                        definitionId: rule.Id, quantity: 1, condition: null, location: RunItemTransfer.Stash(
                                definition: rule, page: index, x: cell % 64, y: cell / 64), storageGrid: null))];
        session.TryEditInventory(edit: (inventory, _, _) => new RunInventory(items: filled,
                stashGrid: inventory.StashGrid, stashPageCount: index + 1, nextItemId: inventory.NextItemId + 4096))
                .ShouldBeTrue();
        return index + 1;
    }

    private static byte[][] HoldExternalPage(Room room, int sender)
    {
        byte[][] parts = [.. room.HostPort.WireSent.Where(predicate: row => row.Peer == sender && row.Reliable
                && IsExternalPart(data: row.Payload)).Select(selector: row => row.Payload)];
        _ = room.HostPort.WireSent.RemoveAll(match: row => row.Peer == sender && IsExternalPart(data: row.Payload));
        return parts;
    }

    private static bool IsExternalPart(byte[] data)
    {
        FoundationEnvelope envelope = FoundationProtocol.Decode(bytes: data);
        return envelope.Kind is "view-part" && envelope.Game?.Scope is "external";
    }

    private sealed class Room
    {
        internal Room(Func<string, int, byte[]?>? read = null)
        {
            Host = new FoundationCoordinator(playerId: "host", label: "Host", connection: HostPort);
            Host.Start(host: true, read: read);
        }

        internal FoundationHarnessConnection HostPort { get; } = new();
        internal FoundationCoordinator Host { get; }

        internal FoundationCoordinator Join(int sender)
        {
            var port = new FoundationHarnessConnection();
            _ports.Add(key: sender, value: port);
            var guest = new FoundationCoordinator(playerId: "guest" + sender, label: "Guest", connection: port);
            guest.Join(address: "test-local", port: 24680);
            for (int pulse = 0; pulse < 3; pulse++)
            {
                Exchange(sender: sender, guest: guest);
            }

            guest.Ready.ShouldBeTrue();
            return guest;
        }

        internal void Exchange(int sender, FoundationCoordinator guest, long elapsed = 0)
        {
            guest.Update(elapsedMilliseconds: 0);
            Receive(sender: sender, elapsed: elapsed);
            Send(sender: sender, guest: guest);
        }

        internal void Receive(int sender, long elapsed = 0)
        {
            FoundationHarnessConnection port = _ports[sender];
            foreach ((int _, byte[] data, bool _) in port.WireSent)
            {
                HostPort.Raw(sender: sender, payload: data);
            }

            port.WireSent.Clear();
            Host.Update(elapsedMilliseconds: elapsed);
        }

        internal void Send(int sender, FoundationCoordinator guest)
        {
            foreach ((int _, byte[] data, bool _) in HostPort.WireSent.Where(
                    predicate: packet => packet.Peer == sender))
            {
                _ports[sender].Raw(sender: 1, payload: data);
            }

            _ = HostPort.WireSent.RemoveAll(match: packet => packet.Peer == sender);
            guest.Update(elapsedMilliseconds: 0);
        }

        internal void ReceivePart(int sender, FoundationCoordinator guest, byte[] data)
        {
            _ports[sender].Raw(sender: 1, payload: data);
            guest.Update(elapsedMilliseconds: 0);
        }

        private readonly Dictionary<int, FoundationHarnessConnection> _ports = [];
    }
}
