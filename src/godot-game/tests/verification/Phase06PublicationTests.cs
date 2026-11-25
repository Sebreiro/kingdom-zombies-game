using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using KingdomZombies.Core;
using KingdomZombies.Foundation;
using Shouldly;
using Xunit;
using Model = KingdomZombies.Foundation.FoundationInventoryInteraction;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Verifies that normal composition and peer frames share the same run authority.</summary>
[Trait("Tier", "1")]
public sealed class Phase06PublicationTests
{
    /// <summary>A current connection can invoke the same scoped gameplay dispatcher as a local player.</summary>
    [Fact]
    public void Dispatcher_TrustedGuestAndSolo_UseOneAuthority()
    {
        CheckSolo();
        CheckContestedGuest();
    }

    /// <summary>The current compatibility epoch identifies the bounded recipient protocol.</summary>
    [Fact]
    public void GameplayVersion_CurrentEpoch_IdentifiesScopedPublicationFrames()
    {
        var hello = new FoundationEnvelope(kind: "hello", playerId: "peer", label: "Guest");
        FoundationProtocol.Decode(bytes: FoundationProtocol.Encode(envelope: hello)).ProtocolVersion
                .ShouldBe(expected: 2);
    }

    /// <summary>Real oversized wire parts keep roster and world paired while private death fencing proceeds.</summary>
    [Fact]
    public void PublicFrames_Oversized_InstallTogetherWithoutDelayingPrivateFencing()
    {
        var room = new Room(read: ReadLargeConfig, fixture: "wire");
        FoundationCoordinator guest = room.Join(sender: 2);
        guest.Submit(direction: 1).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 2000);
        guest.Submit(direction: 0).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        FoundationRunReadback previous = guest.ReadRunRender().ShouldNotBeNull();
        string victim = guest.LocalRunControl.ShouldNotBeNull().survivorId;
        byte[] oldOwn = LastScope(room: room, sender: 2, scope: "own");
        FoundationSession host = Session(coordinator: room.Host);
        JsonElement world = host.ReadRunPublic().ShouldNotBeNull().world;
        JsonSerializer.SerializeToUtf8Bytes(value: world).Length.ShouldBeGreaterThan(expected:
                FoundationProtocol.MaximumBytes);
        Phase06MeleeTests.Enemy(session: host, id: "wire-killer", x: -1500);
        host.TryUpdateVitals(survivorId: victim, health: 1, stamina: 100).ShouldBeTrue();
        host.AttackUncontrolled(survivorId: "wire-killer", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();

        room.Receive(sender: 2, elapsed: 16);
        byte[][] publicParts = TakePublicParts(room: room, sender: 2);
        publicParts.Length.ShouldBeGreaterThan(expected: 1);
        room.ReceivePart(sender: 2, guest: guest, data: publicParts[^1]);
        guest.ReadRunRender().ShouldNotBeNull().Roster.ShouldBeSameAs(expected: previous.Roster);
        room.Send(sender: 2, guest: guest);
        guest.LocalRunControl.ShouldNotBeNull().generation.ShouldBe(expected: 0);
        guest.ReadRunRender().ShouldNotBeNull().Views.ContainsKey(key: "own").ShouldBeFalse();
        room.ReceivePart(sender: 2, guest: guest, data: oldOwn);
        guest.ReadRunRender().ShouldNotBeNull().Views.ContainsKey(key: "own").ShouldBeFalse();
        guest.ReadRunRender().ShouldNotBeNull().Roster.Revision.ShouldBe(expected: previous.Roster.Revision);
        foreach (byte[] part in publicParts[..^1])
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        FoundationRunReadback after = guest.ReadRunRender().ShouldNotBeNull();
        after.Roster.Revision.ShouldBeGreaterThan(expected: previous.Roster.Revision);
        after.World.ShouldNotBeNull().GetProperty(propertyName: "Corpses").EnumerateArray().Any(predicate: row =>
                StringComparer.Ordinal.Equals(x: row.GetProperty(propertyName: "SurvivorId").GetString(), y: victim))
                .ShouldBeTrue();
        room.ReceivePart(sender: 2, guest: guest, data: publicParts[0]);
        guest.ReadRunRender().ShouldNotBeNull().Roster.ShouldBeSameAs(expected: after.Roster);
    }

    /// <summary>A legal vicinity above the old cap still advertises reachable targets.</summary>
    [Fact]
    public void NearbyList_Saturated_KeepsStashAndEveryBoundedPageAvailable()
    {
        const int Piles = 17000;
        RunWorld.Anchor[] anchors =
        [
            new RunWorld.Anchor(id: RunWorld.StashId, contact: new RunWorld.Point(x: -8000, y: 0),
                    kind: RunWorld.Access.Stash),
            .. Enumerable.Range(start: 0, count: Piles).Select(selector: index => new RunWorld.Anchor(
                    id: FormattableString.Invariant(formattable: $"local-pile-{index:D5}"),
                    contact: new RunWorld.Point(x: -16500 + index, y: 0), kind: RunWorld.Access.Pile)),
            new RunWorld.Anchor(id: "out-of-range", contact: new RunWorld.Point(x: 40000, y: 0),
                    kind: RunWorld.Access.Pile),
        ];
        var world = new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: new RunWorld.Ranges(refugeUnits: 12, dangerUnits: 12, localUnits: 9, stashUnits: 4),
                defenses: [], blockers: [], hostiles: [], anchors: anchors);
        var session = new FoundationSession(sessionId: "pages", playerId: "host", actorId: "actor", world: world);
        session.Step(elapsedMilliseconds: 0);

        FoundationRunFrame first = NearbyPage(session: session, page: 1);
        first.Body.ShouldNotBeNull().GetProperty(propertyName: "Count").GetInt32().ShouldBe(expected: Piles + 8);
        first.Rows[0].GetProperty(propertyName: "Id").GetString().ShouldBe(expected: RunWorld.StashId);
        FoundationRunFrame second = NearbyPage(session: session, page: 2);
        second.Rows.Length.ShouldBe(expected: FoundationSession.NearbyPageSize);
        second.Rows[0].GetProperty(propertyName: "Id").GetString().ShouldBe(expected: "local-pile-00024");
        FoundationRunFrame last = NearbyPage(session: session, page: int.MaxValue);
        last.Rows[^1].GetProperty(propertyName: "Id").GetString().ShouldBe(expected: "local-pile-16999");
        last.Rows.Length.ShouldBeInRange(from: 1, to: FoundationSession.NearbyPageSize);
    }

    /// <summary>A full nearby page fits even when every legal identity needs six wire bytes per character.</summary>
    [Fact]
    public void NearbyIds_MaximumEscapedLength_FitOneNativeFrame()
    {
        string prefix = "\u6800".PadLeft(totalWidth: 126, paddingChar: '\u6800');
        RunWorld.Anchor[] anchors = [.. Enumerable.Range(start: 0, count: 80).Select(selector: index =>
                new RunWorld.Anchor(id: prefix + (char)(0x6800 + index) + '\u682F',
                        contact: new RunWorld.Point(x: -8000 + index, y: 0), kind: RunWorld.Access.Pile))];
        var world = new RunWorld(origin: default, groundContactSubunits: 0, mainBuilding: default,
                ranges: new RunWorld.Ranges(refugeUnits: 12, dangerUnits: 12, localUnits: 9, stashUnits: 4),
                defenses: [], blockers: [], hostiles: [], anchors: anchors);
        var session = new FoundationSession(sessionId: "pages", playerId: "host", actorId: "actor", world: world);
        FoundationRunFrame page = NearbyPage(session: session, page: 2);

        page.Rows.Length.ShouldBe(expected: FoundationSession.NearbyPageSize);
        page.Rows.All(predicate: row => row.GetProperty(propertyName: "Id").GetString()?.Length == 128)
                .ShouldBeTrue();
        anchors.Count(predicate: anchor => page.Rows.Any(predicate: row => StringComparer.Ordinal.Equals(
                x: anchor.Id, y: row.GetProperty(propertyName: "Id").GetString())))
                .ShouldBe(expected: page.Rows.Length);
    }

    /// <summary>Recipients can page through thousands of loose items and act on original references.</summary>
    [Fact]
    public void LooseLootPages_ThousandsOfItems_RetainTokensAndExactItemReferences()
    {
        var room = new Room(read: ReadLargeConfig, fixture: "wire");
        FoundationCoordinator guest = room.Join(sender: 2);
        FoundationCoordinator other = room.Join(sender: 3);
        FoundationSession session = Session(coordinator: room.Host);
        string owner = guest.LocalRunControl.ShouldNotBeNull().survivorId;
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-23", slot: "backpack", owner: owner);
        (long sequence, string status) localSelection = room.Host.SubmitRunAction(kind: "select",
                request: Model.SelectTarget(target: ("Pile", RunCombatFixture.Remains)).Request)
                .ShouldNotBeNull();
        localSelection.status.ShouldBe(expected: "Ready");
        FoundationRunView local = ExternalView(coordinator: room.Host);
        local.Header.GetProperty(propertyName: "ItemCount").GetInt32().ShouldBe(expected: 4098);
        room.Host.RequestExternalItemsPage(token: local.Token + 1, page: 2).ShouldBeFalse();
        room.Host.RequestExternalItemsPage(token: local.Token, page: 2).ShouldBeTrue();
        FoundationRunView localSecond = ExternalView(coordinator: room.Host);
        localSecond.Token.ShouldBe(expected: local.Token);

        OpenGuestPile(room: room, guest: guest);
        FoundationRunView first = ExternalView(coordinator: guest);
        FoundationRunView own = guest.ReadRunRender().ShouldNotBeNull().Views["own"];
        guest.RequestExternalItemsPage(token: first.Token, page: 2).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        FoundationRunView second = ExternalView(coordinator: guest);
        second.Token.ShouldBe(expected: first.Token);
        second.Header.GetProperty(propertyName: "ItemPage").GetInt32().ShouldBe(expected: 2);
        Ids(view: second).ShouldBe(expected: Ids(view: localSecond));
        Ids(view: second).Intersect(second: Ids(view: first)).ShouldBeEmpty();
        guest.ReadRunRender().ShouldNotBeNull().Views["own"].ShouldBeSameAs(expected: own);
        room.Send(sender: 3, guest: other);
        other.ReadRunRender().ShouldNotBeNull().Views.ContainsKey(key: "external").ShouldBeFalse();
        AssertLoosePageActions(room: room, guest: guest, session: session, second: second);
    }

    /// <summary>An unchanged authorized bag tree remains usable through a delayed unrelated refresh.</summary>
    [Fact]
    public void OffPageWindows_RefreshThenTransfer_SurviveAndFenceOldParts()
    {
        var room = new Room(read: ReadLargeConfig, fixture: "wire");
        FoundationCoordinator guest = room.Join(sender: 2);
        FoundationSession session = Session(coordinator: room.Host);
        JsonElement bag = OpenGuestBagTree(room: room, guest: guest);
        FoundationRunReadback before = guest.ReadRunRender().ShouldNotBeNull();

        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "right-hand");
        room.Receive(sender: 2, elapsed: 16);
        byte[][] delayed = HoldExternalScopes(room: room, sender: 2);
        delayed.Length.ShouldBeGreaterThanOrEqualTo(expected: 3);
        room.Send(sender: 2, guest: guest);
        AssertContinuingExternalScopes(guest: guest, before: before);
        foreach (byte[] part in delayed.Reverse())
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        guest.ReadRunRender().ShouldNotBeNull().Views["external.2"].Revision.ShouldBeGreaterThan(expected:
                before.Views["external.2"].Revision);
        string owner = guest.LocalRunControl.ShouldNotBeNull().survivorId;
        _ = guest.SubmitRunAction(kind: "place", request: Equipment(source: bag, owner: owner, slot: "backpack",
                token: before.Views["external"].Token)).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Applied");
        foreach (byte[] part in delayed)
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        FoundationRunReadback moved = guest.ReadRunRender().ShouldNotBeNull();
        moved.Views.ContainsKey(key: "external.1").ShouldBeFalse();
        moved.Views.ContainsKey(key: "external.2").ShouldBeFalse();
        moved.Views["external"].Token.ShouldBe(expected: before.Views["external"].Token);
    }

    /// <summary>A full legal stash page pulse crosses the real wire and highlights all exact changed cells.</summary>
    [Fact]
    public void StashGridPulse_FullPage_FitsWireWithoutLosingCells()
    {
        var room = new Room(read: ReadFullFootprintConfig);
        FoundationCoordinator guest = room.Join(sender: 2);
        FoundationSession session = Session(coordinator: room.Host);
        string owner = guest.LocalRunControl.ShouldNotBeNull().survivorId;
        RunInventory.Item sword = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand",
                owner: owner);
        ReachStash(room: room, guest: guest, sender: 2);
        FoundationRunView external = ExternalView(coordinator: guest);
        guest.RequestExternalItemsPage(token: external.Token, page: 2).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "AccessLost");
        ExternalView(coordinator: guest).ShouldBeSameAs(expected: external);
        Model.Item item = guest.ReadRunRender().ShouldNotBeNull().Views["own"].Rows.Select(
                selector: Model.ReadItem).Single(predicate: row => row.Id == sword.Id);
        Model.Command deposit = Model.Quick(item: item, token: external.Token, deposit: true);
        long sequence = guest.SubmitRunAction(kind: deposit.Kind, request: deposit.Request).ShouldNotBeNull().sequence;
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Applied");

        FoundationRunView deposited = ExternalView(coordinator: guest);
        (ImmutableArray<(int x, int y)> cells, int more) effect = Model.DepositHighlight(header: deposited.Header,
                target: ("Stash", RunWorld.StashId), sequence: sequence).ShouldNotBeNull();
        effect.cells.ShouldBe(expected: Enumerable.Range(start: 0, count: 4096)
                .Select(selector: cell => (cell % 64, cell / 64)).ToImmutableArray());
        effect.more.ShouldBe(expected: 0);
        LastScope(room: room, sender: 2, scope: "external").Length.ShouldBeLessThan(expected:
                FoundationProtocol.MaximumBytes);
    }

    /// <summary>Four roles receive only their own private data and the permitted current correction.</summary>
    [Fact]
    public void P02_CleanControl_PreservesPrivateAndLocalAccess()
    {
        var room = new Room();
        FoundationCoordinator first = room.Join(sender: 2);
        FoundationCoordinator second = room.Join(sender: 3);
        CheckRecruitScopeRevoked(room: room, guest: first);
        FoundationCoordinator remote = room.Guest(sender: 4);
        Dictionary<int, long> secrets = SeedPrivateTrees(session: Session(coordinator: room.Host),
                senders: [0, 2, 3, 4]);
        CheckCachedPublicBaseline(room: room, guest: remote, secrets: secrets);
        CheckPrivateWindows(room: room, secrets: secrets);
        CheckStashCorrection(room: room, first: first, second: second);
        CheckRemoteDenial(room: room, guest: remote, secrets: secrets);
        room.History.Where(predicate: row => IsPublic(data: row.Payload)).All(predicate: row =>
                PrivacyHolds(recipient: row.Peer, data: row.Payload, secrets: secrets, stashAllowed: false))
                .ShouldBeTrue();
    }

    /// <summary>The same byte and permission assertion detects two deliberately inconsistent observations.</summary>
    [Fact]
    public void P02_ViolationControl_DetectsPrivateOrRemoteStashResult()
    {
        var room = new Room();
        FoundationCoordinator guest = room.Join(sender: 2);
        Dictionary<int, long> secrets = SeedPrivateTrees(session: Session(coordinator: room.Host), senders: [2]);
        FoundationRunView own = OpenDeepWindow(room: room, sender: 2, secret: secrets[2]);
        own.Rows.Single().GetProperty(propertyName: "Id").GetInt64().ShouldBe(expected: secrets[2]);
        byte[] privateBytes = LastScope(room: room, sender: 2, scope: "own");
        PrivacyHolds(recipient: 2, data: privateBytes, secrets: secrets, stashAllowed: false).ShouldBeTrue();
        PrivacyHolds(recipient: 3, data: privateBytes, secrets: secrets, stashAllowed: false).ShouldBeFalse();
        ReachStash(room: room, guest: guest, sender: 2);
        byte[] stashBytes = LastScope(room: room, sender: 2, scope: "external");
        PrivacyHolds(recipient: 2, data: stashBytes, secrets: secrets, stashAllowed: true).ShouldBeTrue();
        PrivacyHolds(recipient: 2, data: stashBytes, secrets: secrets, stashAllowed: false).ShouldBeFalse();
        ProofProtocol.Record(id: "phase06.publication.p02-violation",
                diagnostic: "MISROUTED_PRIVATE_BYTES_AND_UNAUTHORIZED_STASH_DETECTED");
    }

    /// <summary>Real page parts install together and are fenced after the guest loses stash access.</summary>
    [Fact]
    public void ScopedPage_LargeThenRevoked_InstallsAtomicallyAndFencesDelayedFrames()
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
        int applicationMax = parts.Max(selector: bytes => bytes.Length);
        int nativeMax = parts.Max(selector: bytes => FoundationWireFrame.Encode(payload: bytes).Length);
        applicationMax.ShouldBeLessThanOrEqualTo(expected: FoundationProtocol.MaximumBytes);
        nativeMax.ShouldBeLessThanOrEqualTo(expected: FoundationProtocol.MaximumBytes);
        room.Send(sender: 2, guest: guest);
        room.ReceivePart(sender: 2, guest: guest, data: parts[^1]);
        room.ReceivePart(sender: 2, guest: guest, data: parts[^1]);
        CheckMalformedPart(room: room, guest: guest, data: parts[^1]);
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
        room.Exchange(sender: 2, guest: guest, elapsed: 16);
        foreach (byte[] part in parts.SkipLast(count: 1).Reverse())
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        FoundationRunView open = guest.ReadRunRender().ShouldNotBeNull().Views["external"];
        open.Rows.Length.ShouldBe(expected: 4096);
        open.Rows.Select(selector: ItemId).Distinct().Count().ShouldBe(expected: 4096);
        CheckPartTimeout(room: room, guest: guest, page: page, token: open.Token);
        guest.Submit(direction: -1).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 1200);
        foreach (byte[] part in parts)
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
        guest.Ready.ShouldBeTrue();
        string evidence = FormattableString.Invariant(
                formattable: $"ROWS_4096_PARTS_{parts.Length}_APPLICATION_MAX_{applicationMax}_NATIVE_MAX_{nativeMax}");
        ProofProtocol.Record(id: "phase06.publication.large-page", diagnostic: evidence);
    }

    /// <summary>A ready guest can wait while dead, then use the same connection with a new survivor epoch.</summary>
    [Fact]
    public void Spectator_NewControl_RetainsConnectionAndRejectsOldEpoch()
    {
        var room = new Room();
        FoundationCoordinator guest = room.Join(sender: 2);
        _ = room.Join(sender: 3);
        FoundationCoordinator leaving = room.Join(sender: 4);
        ReachStash(room: room, guest: guest, sender: 2);
        RetireOtherCandidates(room: room);
        room.Send(sender: 2, guest: guest);
        (string survivorId, long generation, long _) = guest.LocalRunControl.ShouldNotBeNull();
        string actorToken = guest.LocalActor.ShouldNotBeNull().ActorId;
        byte[] stale = LastScope(room: room, sender: 2, scope: "own");
        KillGuest(room: room, guest: guest, survivor: survivorId);
        guest.Ready.ShouldBeTrue();
        guest.LocalRunControl.ShouldNotBeNull().generation.ShouldBe(expected: 0);
        guest.ReadRunRender()?.Views.ContainsKey(key: "own").ShouldBe(expected: false);
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
        guest.Update(elapsedMilliseconds: 500);
        room.GuestPort(sender: 2).Sent.Any(predicate: row => row.Envelope.Kind is "ping").ShouldBeTrue();
        _ = guest.SubmitRunAction(kind: "respawn", request: JsonSerializer.SerializeToElement(value: new object()))
                .ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Waiting");
        leaving.RequestClose();
        room.Exchange(sender: 4, guest: leaving);
        room.Send(sender: 2, guest: guest);
        guest.LocalRunControl.ShouldNotBeNull().survivorId.ShouldBe(expected: "survivor-04");
        guest.LocalRunControl.ShouldNotBeNull().generation.ShouldBeGreaterThan(expected: generation);
        guest.LocalActor.ShouldNotBeNull().ActorId.ShouldBe(expected: actorToken);
        room.ReceivePart(sender: 2, guest: guest, data: stale);
        guest.ReadRunRender().ShouldNotBeNull().Views["own"].Epoch
                .ShouldBe(expected: guest.LocalRunControl.ShouldNotBeNull().generation);
        CheckOldEpochCannotAdvanceSequence(room: room, guest: guest, survivor: survivorId, generation: generation);
        ProofProtocol.Record(id: "phase06.publication.spectator", diagnostic:
                "READY_HEARTBEAT_WAITING_REBOUND_SAME_TRANSPORT_REJECTED_OLD_EPOCH");
    }

    /// <summary>Typed peer requests route jump, combat and food into the same authority.</summary>
    [Fact]
    public void GuestRequests_MotionAttackAndFood_TraverseGameplayWire()
    {
        var room = new Room();
        FoundationCoordinator guest = room.Join(sender: 2);
        FoundationSession session = Session(coordinator: room.Host);
        string survivor = guest.LocalRunControl.ShouldNotBeNull().survivorId;
        guest.SubmitRunMotion(direction: 0, sprint: false, jump: true).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        Phase06MeleeTests.Survivor(session: session, id: survivor).JumpStartedAt.ShouldBe(expected: 0);
        Phase06MeleeTests.Enemy(session: session, id: "wire-target", x: -7500, health: 1);
        _ = guest.SubmitRunAction(kind: "attack", request: JsonSerializer.SerializeToElement(value:
                new { AimX = (double?)null, AimY = (double?)null })).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        Phase06DeathTests.Loot(session: session).Deaths.ContainsKey(key: "wire-target").ShouldBeTrue();
        CheckGuestFood(room: room, guest: guest, session: session, survivor: survivor);
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
                selector: scope => scope.scope is "nearby" ? [] : scope.rows)
                .Any(predicate: row => ItemId(item: row) == item);
    }

    private static JsonElement Backpack(JsonElement source, string owner, long token)
    {
        return Equipment(source: source, owner: owner, slot: "backpack", token: token);
    }

    private static JsonElement Equipment(JsonElement source, string owner, string slot, long token)
    {
        return JsonSerializer.SerializeToElement(value: new
        {
            Item = new { Id = ItemId(item: source), Stamp = source.GetProperty(propertyName: "Stamp").GetString() },
            Token = token,
            Destination = new
            {
                Kind = "equipment",
                TargetId = owner,
                Slot = slot,
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

    private static void CheckRecruitScopeRevoked(Room room, FoundationCoordinator guest)
    {
        _ = guest.SubmitRunAction(kind: "select", request: JsonSerializer.SerializeToElement(value:
                new { TargetKind = "Npc", TargetId = "survivor-04" })).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        byte[] recruit = LastScope(room: room, sender: 2, scope: "external");
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: true);
        _ = room.Join(sender: 4);
        room.Send(sender: 2, guest: guest);
        room.ReceivePart(sender: 2, guest: guest, data: recruit);
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
    }

    private static void CheckPrivateWindows(Room room, Dictionary<int, long> secrets)
    {
        room.Host.RequestRunWindows(ownGrid: secrets[0] - 1, externalGrid: null).ShouldBeTrue();
        room.Host.ReadRunRender().ShouldNotBeNull().Views["own"].Rows.Single()
                .GetProperty(propertyName: "Id").GetInt64().ShouldBe(expected: secrets[0]);
        foreach (int sender in new[] { 2, 3, 4 })
        {
            FoundationRunView own = OpenDeepWindow(room: room, sender: sender, secret: secrets[sender]);
            own.Rows.Single().GetProperty(propertyName: "Id").GetInt64().ShouldBe(expected: secrets[sender]);
            foreach (int recipient in new[] { 2, 3, 4 })
            {
                PrivacyHolds(recipient: recipient, data: LastScope(room: room, sender: sender, scope: "own"),
                        secrets: secrets, stashAllowed: false).ShouldBe(expected: recipient == sender);
            }
        }
    }

    private static void CheckCachedPublicBaseline(Room room, FoundationCoordinator guest,
            Dictionary<int, long> secrets)
    {
        byte[] initial = room.History.First(predicate: row => row.Peer == 4
                && FoundationProtocol.Decode(bytes: row.Payload).Kind is "baseline").Payload;
        FoundationEnvelope hello = room.GuestPort(sender: 4).Sent.First(predicate: row =>
                row.Envelope.Kind is "hello").Envelope;
        room.HostPort.Enqueue(sender: 4, envelope: hello);
        room.Exchange(sender: 4, guest: guest);
        byte[] replay = room.History.Last(predicate: row => row.Peer == 4
                && FoundationProtocol.Decode(bytes: row.Payload).Kind is "baseline").Payload;
        replay.ShouldBe(expected: initial);
        PrivacyHolds(recipient: 4, data: replay, secrets: secrets, stashAllowed: false).ShouldBeTrue();
        guest.Ready.ShouldBeTrue();
    }

    private static FoundationRunView OpenDeepWindow(Room room, int sender, long secret)
    {
        FoundationCoordinator guest = room.Guest(sender: sender);
        guest.RequestRunWindows(ownGrid: secret - 2, externalGrid: null).ShouldBeTrue();
        room.Exchange(sender: sender, guest: guest);
        guest.ReadRunRender().ShouldNotBeNull().Views["own"].Rows.Single().GetProperty(propertyName: "Id")
                .GetInt64().ShouldBe(expected: secret - 1);
        guest.RequestRunWindows(ownGrid: secret - 1, externalGrid: null).ShouldBeTrue();
        room.Exchange(sender: sender, guest: guest);
        FoundationRunReadback readback = guest.ReadRunRender().ShouldNotBeNull();
        FoundationRunView own = readback.Views["own"];
        own.Correlation.ShouldBe(expected: readback.Feedback.ShouldNotBeNull().sequence);
        own.Serial.ShouldBeGreaterThan(expected: 0);
        return own;
    }

    private static void CheckStashCorrection(Room room, FoundationCoordinator first, FoundationCoordinator second)
    {
        ReachStash(room: room, guest: first, sender: 2);
        ReachStash(room: room, guest: second, sender: 3);
        FoundationRunView open = first.ReadRunRender().ShouldNotBeNull().Views["external"];
        JsonElement club = open.Rows.First(predicate: row => row.GetProperty(propertyName: "DefinitionId")
                .GetString() is "IT-04");
        long otherToken = second.ReadRunRender().ShouldNotBeNull().Views["external"].Token;
        string firstOwner = first.LocalRunControl.ShouldNotBeNull().survivorId;
        string secondOwner = second.LocalRunControl.ShouldNotBeNull().survivorId;
        (long sequence, string status) winner = first.SubmitRunAction(kind: "place", request:
                Equipment(source: club, owner: firstOwner, slot: "right-hand", token: open.Token)).ShouldNotBeNull();
        byte[] retry = room.GuestPort(sender: 2).WireSent.Last().Payload;
        room.Exchange(sender: 2, guest: first);
        first.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Applied");
        room.HostPort.Raw(sender: 2, payload: retry);
        room.Exchange(sender: 2, guest: first);
        first.RunActionFeedback.ShouldNotBeNull().ShouldBe(expected: (winner.sequence, "Applied", true));
        (long sequence, string status) loser = second.SubmitRunAction(kind: "place", request:
                Equipment(source: club, owner: secondOwner, slot: "right-hand", token: otherToken)).ShouldNotBeNull();
        room.Exchange(sender: 3, guest: second);
        FoundationRunReadback corrected = second.ReadRunRender().ShouldNotBeNull();
        corrected.Feedback.ShouldBe(expected: (loser.sequence, "AccessLost", false));
        corrected.Views["external"].Correlation.ShouldBe(expected: loser.sequence);
        corrected.Views["external"].Rows.Any(predicate: row => ItemId(item: row) == ItemId(item: club)).ShouldBeFalse();
    }

    private static void CheckRemoteDenial(Room room, FoundationCoordinator guest, Dictionary<int, long> secrets)
    {
        (long sequence, string status) request = guest.SubmitRunAction(kind: "select", request: StashSelection())
                .ShouldNotBeNull();
        room.Exchange(sender: 4, guest: guest);
        FoundationRunReadback readback = guest.ReadRunRender().ShouldNotBeNull();
        readback.Feedback.ShouldBe(expected: (request.sequence, "AccessLost", false));
        readback.Views.ContainsKey(key: "external").ShouldBeFalse();
        room.History.Where(predicate: row => row.Peer == 4).All(predicate: row =>
                PrivacyHolds(recipient: 4, data: row.Payload, secrets: secrets, stashAllowed: false)).ShouldBeTrue();
        ProofProtocol.Record(id: "phase06.publication.p02-clean", diagnostic:
                "FOUR_ROLES_REMOTE_STASH_ACCESS_LOST_STALE_TRANSFER_ACCESS_LOST_PRIVATE_BYTES_SCOPED");
    }

    private static void ReachStash(Room room, FoundationCoordinator guest, int sender)
    {
        guest.Submit(direction: 1).ShouldBeTrue();
        room.Exchange(sender: sender, guest: guest, elapsed: 2000);
        guest.Submit(direction: 0).ShouldBeTrue();
        _ = guest.SubmitRunAction(kind: "select", request: StashSelection()).ShouldNotBeNull();
        room.Exchange(sender: sender, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Ready");
    }

    private static bool PrivacyHolds(int recipient, byte[] data, Dictionary<int, long> secrets, bool stashAllowed)
    {
        FoundationEnvelope envelope = FoundationProtocol.Decode(bytes: data);
        string text = Encoding.UTF8.GetString(bytes: data);
        if (envelope.Game is { Scope: "external", Body: { } header }
                && header.GetProperty(propertyName: "Kind").GetString() is "Stash" && !stashAllowed)
        {
            return false;
        }

        return secrets.All(predicate: entry => !text.Contains(value: entry.Value.ToString(
                provider: CultureInfo.InvariantCulture), comparisonType: StringComparison.Ordinal)
                || entry.Key == recipient && envelope.Game?.Scope is "own")
                && (!IsPublic(data: data) || !text.Contains(value: "\"Quantity\"",
                        comparisonType: StringComparison.Ordinal));
    }

    private static bool IsPublic(byte[] data)
    {
        return FoundationProtocol.Decode(bytes: data).Kind is "state" or "baseline" or "world-part" or "action-result"
                or "ping" or "end" or "reject";
    }

    private static byte[][] TakePublicParts(Room room, int sender)
    {
        (int Peer, byte[] Payload, bool Reliable)[] parts = [.. room.HostPort.WireSent.Where(predicate: entry =>
                entry.Peer == sender && FoundationProtocol.Decode(bytes: entry.Payload).Kind is "world-part")];
        foreach ((int peer, byte[] data, bool reliable) in parts)
        {
            reliable.ShouldBeTrue();
            data.Length.ShouldBeLessThan(expected: FoundationProtocol.MaximumBytes);
            PrivacyHolds(recipient: peer, data: data, secrets: [], stashAllowed: false).ShouldBeTrue();
            room.HostPort.WireSent.Remove(item: (peer, data, reliable)).ShouldBeTrue();
        }

        return [.. parts.Select(selector: entry => entry.Payload)];
    }

    private static FoundationRunFrame NearbyPage(FoundationSession session, int page)
    {
        (string scope, long generation, long token, JsonElement header, ImmutableArray<JsonElement> rows) nearby =
                session.ReadRunViews(identity: ("pages", "local", "host"), includeItems: false, nearbyPage: page)
                        .ShouldNotBeNull().views.Single(predicate: view => view.scope is "nearby");
        var frame = new FoundationRunFrame
        {
            Scope = nearby.scope,
            Epoch = nearby.generation,
            View = 1,
            Parts = 1,
            Body = nearby.header,
            Rows = nearby.rows,
        };
        byte[] bytes = FoundationProtocol.Encode(envelope: new FoundationEnvelope(kind: "view-part",
                sessionId: "pages", playerId: "host", actorId: "actor", bindingId: "local",
                revision: session.Snapshot.Revision, attemptId: "visit", game: frame));
        bytes.Length.ShouldBeLessThan(expected: FoundationProtocol.MaximumBytes);
        return FoundationProtocol.Decode(bytes: bytes).Game.ShouldNotBeNull();
    }

    private static byte[] LastScope(Room room, int sender, string scope)
    {
        return room.History.Last(predicate: row => row.Peer == sender
                && FoundationProtocol.Decode(bytes: row.Payload) is { Kind: "view-part", Game: { } game }
                && StringComparer.Ordinal.Equals(x: game.Scope, y: scope)).Payload;
    }

    private static Dictionary<int, long> SeedPrivateTrees(FoundationSession session, int[] senders)
    {
        Dictionary<int, long> markers = senders.ToDictionary(keySelector: sender => sender,
                elementSelector: sender => 6000000002L + 100 * sender);
        session.TryEditInventory(edit: (inventory, definitions, _) => inventory.WithItems(items:
                [.. inventory.Items, .. markers.SelectMany(selector: mark => Pack(definitions: definitions,
                        sender: mark.Key, secret: mark.Value))], nextItemId: markers.Values.Max() + 1)).ShouldBeTrue();
        return markers;
    }

    private static RunInventory.Item[] Pack(RunDefinitions definitions, int sender, long secret)
    {
        string owner = FormattableString.Invariant(formattable: $"survivor-{Math.Max(val1: sender, val2: 1):D2}");
        RunDefinitions.Item pack = definitions.Items["IT-23"];
        RunDefinitions.Item pouch = definitions.Items["IT-22"];
        RunDefinitions.Item coins = definitions.Items["IT-01"];
        return
        [
            new RunInventory.Item(id: secret - 2, definitionId: pack.Id, quantity: 1, condition: null,
                    location: new RunInventory.Equipment(survivorId: owner, slot: "backpack"),
                    storageGrid: pack.Storage),
            new RunInventory.Item(id: secret - 1, definitionId: pouch.Id, quantity: 1, condition: null,
                    location: RunItemTransfer.Container(definition: pouch, containerId: secret - 2, x: 0, y: 0),
                    storageGrid: pouch.Storage),
            new RunInventory.Item(id: secret, definitionId: coins.Id, quantity: 40 + sender, condition: null,
                    location: RunItemTransfer.Container(definition: coins, containerId: secret - 1, x: 0, y: 0),
                    storageGrid: null),
        ];
    }

    private static FoundationRunView ExternalView(FoundationCoordinator coordinator)
    {
        return coordinator.ReadRunRender().ShouldNotBeNull().Views["external"];
    }

    private static long[] Ids(FoundationRunView view)
    {
        return [.. view.Rows.Select(selector: row => row.GetProperty(propertyName: "Id").GetInt64())];
    }

    private static void OpenGuestPile(Room room, FoundationCoordinator guest)
    {
        Model.Command select = Model.SelectTarget(target: ("Pile", RunCombatFixture.Remains));
        _ = guest.SubmitRunAction(kind: select.Kind, request: select.Request).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Ready");
    }

    private static JsonElement OpenGuestBagTree(Room room, FoundationCoordinator guest)
    {
        OpenGuestPile(room: room, guest: guest);
        FoundationRunView selected = ExternalView(coordinator: guest);
        JsonElement bag = selected.Rows.Single(predicate: row =>
                row.GetProperty(propertyName: "DefinitionId").GetString() is "IT-23");
        long bagId = ItemId(item: bag);
        guest.RequestRunWorkspace(own: [], external: [bagId]).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        long pouch = ItemId(item: guest.ReadRunRender().ShouldNotBeNull().Views["external.1"].Rows.Single());
        guest.RequestRunWorkspace(own: [], external: [bagId, pouch]).ShouldBeTrue();
        guest.RequestExternalItemsPage(token: selected.Token, page: 2).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        FoundationRunReadback open = guest.ReadRunRender().ShouldNotBeNull();
        open.Views["external"].Header.GetProperty(propertyName: "ItemPage").GetInt32().ShouldBe(expected: 2);
        open.Views["external.2"].Rows.Length.ShouldBe(expected: 1);
        return bag;
    }

    private static void AssertContinuingExternalScopes(FoundationCoordinator guest, FoundationRunReadback before)
    {
        FoundationRunReadback retained = guest.ReadRunRender().ShouldNotBeNull();
        retained.Views["external"].ShouldBeSameAs(expected: before.Views["external"]);
        retained.Views["external.1"].ShouldBeSameAs(expected: before.Views["external.1"]);
        retained.Views["external.2"].ShouldBeSameAs(expected: before.Views["external.2"]);
    }

    private static byte[][] HoldExternalScopes(Room room, int sender)
    {
        Predicate<(int Peer, byte[] Payload, bool Reliable)> external = row => row.Peer == sender && row.Reliable
                && FoundationProtocol.Decode(bytes: row.Payload) is { Kind: "view-part", Game: { } game }
                && (game.Scope is "external" || FoundationProtocol.WindowScope(scope: game.Scope, external: true));
        byte[][] parts = [.. room.HostPort.WireSent.FindAll(match: external).Select(selector: row => row.Payload)];
        _ = room.HostPort.WireSent.RemoveAll(match: external);
        return parts;
    }

    private static void AssertLoosePageActions(Room room, FoundationCoordinator guest, FoundationSession session,
            FoundationRunView second)
    {
        Model.Item chosen = Model.ReadItem(row: second.Rows[0]);
        Model.Command withdraw = Model.Quick(item: chosen, token: second.Token, deposit: false);
        _ = guest.SubmitRunAction(kind: withdraw.Kind, request: withdraw.Request).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Applied");
        session.Inventory.Find(itemId: chosen.Id).ShouldNotBeNull().Location
                .ShouldBeOfType<RunInventory.ContainerCell>();
        guest.RequestExternalItemsPage(token: second.Token, page: int.MaxValue).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        ExternalView(coordinator: guest).Rows.Length.ShouldBe(expected: 1);

        OpenGuestPile(room: room, guest: guest);
        FoundationRunView reopened = ExternalView(coordinator: guest);
        reopened.Token.ShouldNotBe(expected: second.Token);
        reopened.Header.GetProperty(propertyName: "ItemPage").GetInt32().ShouldBe(expected: 1);
        guest.RequestExternalItemsPage(token: second.Token, page: 2).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "AccessLost");
        ExternalView(coordinator: guest).ShouldBeSameAs(expected: reopened);
    }

    private static byte[] ReadFullFootprintConfig(string path, int limit)
    {
        byte[] available = ReadLargeConfig(path: path, limit: limit);
        if (path is not "content/config/items.json")
        {
            return available;
        }

        JsonNode root = JsonNode.Parse(utf8Json: available).ShouldNotBeNull();
        JsonNode sword = root["definitions"].ShouldNotBeNull().AsArray().Single(predicate: row =>
                StringComparer.Ordinal.Equals(x: row?["id"]?.GetValue<string>(), y: "IT-03")).ShouldNotBeNull();
        sword["footprint"] = new JsonArray(items: [JsonValue.Create(value: 64), JsonValue.Create(value: 64)]);
        return JsonSerializer.SerializeToUtf8Bytes(value: root);
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

    private static void CheckPartTimeout(Room room, FoundationCoordinator guest, int page, long token)
    {
        _ = guest.SubmitRunAction(kind: "browse", request:
                JsonSerializer.SerializeToElement(value: new { Token = token, Page = page })).ShouldNotBeNull();
        room.Receive(sender: 2);
        byte[][] parts = HoldExternalPage(room: room, sender: 2);
        parts.Length.ShouldBeGreaterThan(expected: 1);
        room.Send(sender: 2, guest: guest);
        room.ReceivePart(sender: 2, guest: guest, data: parts[0]);
        room.Time += 5000;
        guest.Update(elapsedMilliseconds: 0);
        foreach (byte[] part in parts)
        {
            room.ReceivePart(sender: 2, guest: guest, data: part);
        }

        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
    }

    private static void CheckMalformedPart(Room room, FoundationCoordinator guest, byte[] data)
    {
        JsonNode root = JsonNode.Parse(utf8Json: data).ShouldNotBeNull();
        root["Game"].ShouldNotBeNull()["Parts"] = FoundationProtocol.MaximumViewParts + 1;
        byte[] invalid = JsonSerializer.SerializeToUtf8Bytes(value: root);
        long rejected = guest.RejectedPackets;
        room.ReceivePart(sender: 2, guest: guest, data: invalid);
        guest.RejectedPackets.ShouldBe(expected: rejected + 1);
        guest.ReadRunRender()?.Views.ContainsKey(key: "external").ShouldBe(expected: false);
    }

    private static void RetireOtherCandidates(Room room)
    {
        room.Host.Submit(direction: -1).ShouldBeTrue();
        foreach (int sender in new[] { 3, 4 })
        {
            room.Guest(sender: sender).Submit(direction: -1).ShouldBeTrue();
            room.Receive(sender: sender);
        }

        room.Receive(sender: 2, elapsed: 1000);
        FoundationSession session = Session(coordinator: room.Host);
        Phase06MeleeTests.Enemy(session: session, id: "wire-left", x: -7500);
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-03", slot: "left-hand", owner: "wire-left");
        _ = Phase06MeleeTests.Equip(session: session, id: "IT-04", slot: "right-hand", owner: "wire-left");
        foreach (RunPopulation.Survivor candidate in session.Population.Candidates())
        {
            session.TryUpdateVitals(survivorId: candidate.Id, health: 1, stamina: 100).ShouldBeTrue();
        }

        for (long sequence = 1; sequence <= 2; sequence++)
        {
            session.AttackUncontrolled(survivorId: "wire-left", sequence: sequence).Hits
                    .Count(predicate: hit => hit.Lethal).ShouldBe(expected: 2);
            room.Receive(sender: 2, elapsed: 1000);
        }

        session.Population.Candidates().ShouldBeEmpty();
    }

    private static void KillGuest(Room room, FoundationCoordinator guest, string survivor)
    {
        FoundationSession session = Session(coordinator: room.Host);
        Phase06MeleeTests.Enemy(session: session, id: "wire-killer", x: -1500);
        session.TryUpdateVitals(survivorId: survivor, health: 1, stamina: 100).ShouldBeTrue();
        session.AttackUncontrolled(survivorId: "wire-killer", sequence: 1).Hits.Single().Lethal.ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 1000);
    }

    private static void CheckOldEpochCannotAdvanceSequence(Room room, FoundationCoordinator guest, string survivor,
            long generation)
    {
        FoundationEnvelope previouslySent = room.GuestPort(sender: 2).Sent.Last(predicate: row =>
                row.Envelope.Kind is "action" && row.Envelope.Game?.Action is "move").Envelope;
        var stale = new FoundationEnvelope(kind: "action", sessionId: previouslySent.SessionId,
                playerId: previouslySent.PlayerId, actorId: previouslySent.ActorId,
                bindingId: previouslySent.BindingId, attemptId: previouslySent.AttemptId, sequence: 1000,
                game: previouslySent.Game.ShouldNotBeNull() with { Survivor = survivor, Epoch = generation });
        long accepted = room.Host.AcceptedIntents;
        room.HostPort.Enqueue(sender: 2, envelope: stale);
        room.Exchange(sender: 2, guest: guest);
        room.HostPort.Sent.Last(predicate: row => row.Envelope.Kind is "action-result").Envelope.Game?.Status
                .ShouldBe(expected: "AccessLost");
        room.Host.AcceptedIntents.ShouldBe(expected: accepted);
        guest.Submit(direction: 1).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest, elapsed: 100);
        room.Host.AcceptedIntents.ShouldBe(expected: accepted + 1);
    }

    private static void CheckGuestFood(Room room, FoundationCoordinator guest, FoundationSession session,
            string survivor)
    {
        RunInventory.Item pocket = session.Inventory.Equipped(survivorId: survivor, slot: "legs").ShouldNotBeNull();
        RunDefinitions.Item rule = session.Definitions.Items["IT-10"];
        long id = session.Inventory.NextItemId;
        var food = new RunInventory.Item(id: id, definitionId: rule.Id, quantity: 2, condition: null,
                location: RunItemTransfer.Container(definition: rule, containerId: pocket.Id, x: 0, y: 0),
                storageGrid: null);
        session.TryEditInventory(edit: (inventory, _, _) => inventory.WithItems(
                items: inventory.Items.Add(item: food), nextItemId: id + 1)).ShouldBeTrue();
        session.TryUpdateVitals(survivorId: survivor, health: 50, stamina: 100).ShouldBeTrue();
        guest.RequestRunWindows(ownGrid: pocket.Id, externalGrid: null).ShouldBeTrue();
        room.Exchange(sender: 2, guest: guest);
        JsonElement item = guest.ReadRunRender().ShouldNotBeNull().Views["own"].Rows.Single();
        _ = guest.SubmitRunAction(kind: "food", request: JsonSerializer.SerializeToElement(value: new
        {
            Item = new { Id = id, Stamp = item.GetProperty(propertyName: "Stamp").GetString() },
            QuickSlot = (int?)null,
        })).ShouldNotBeNull();
        room.Exchange(sender: 2, guest: guest);
        guest.RunActionFeedback.ShouldNotBeNull().status.ShouldBe(expected: "Consumed");
        session.Inventory.Find(itemId: id)?.Quantity.ShouldBe(expected: 1);
    }

    private sealed class Room
    {
        internal Room(Func<string, int, byte[]?>? read = null, string? fixture = null)
        {
            Host = new FoundationCoordinator(playerId: "host", label: "Host", connection: HostPort,
                    nowMilliseconds: () => Time);
            Host.Start(host: true, read: read, fixture: fixture);
        }

        internal FoundationHarnessConnection HostPort { get; } = new();
        internal FoundationCoordinator Host { get; }
        internal List<(int Peer, byte[] Payload, bool Reliable)> History { get; } = [];
        internal long Time { get; set; }

        internal FoundationCoordinator Join(int sender)
        {
            var port = new FoundationHarnessConnection();
            _ports.Add(key: sender, value: port);
            var guest = new FoundationCoordinator(playerId: "guest" + sender, label: "Guest", connection: port,
                    nowMilliseconds: () => Time);
            _guests.Add(key: sender, value: guest);
            guest.Join(address: "test-local", port: 24680);
            for (int pulse = 0; pulse < 3; pulse++)
            {
                Exchange(sender: sender, guest: guest);
            }

            guest.Ready.ShouldBeTrue();
            return guest;
        }

        internal FoundationCoordinator Guest(int sender)
        {
            return _guests[sender];
        }

        internal FoundationHarnessConnection GuestPort(int sender)
        {
            return _ports[sender];
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
            Time += elapsed;
            Host.Update(elapsedMilliseconds: elapsed);
        }

        internal void Send(int sender, FoundationCoordinator guest)
        {
            foreach ((int peer, byte[] data, bool reliable) in HostPort.WireSent.Where(
                    predicate: packet => packet.Peer == sender))
            {
                _ports[sender].Raw(sender: 1, payload: data);
                History.Add(item: (peer, data, reliable));
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
        private readonly Dictionary<int, FoundationCoordinator> _guests = [];
    }
}
