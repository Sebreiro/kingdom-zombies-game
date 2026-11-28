using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static KingdomZombies.Core.RunSnapshotDocument;

namespace KingdomZombies.Core;

/// <summary>Converts authoritative run state to and from detached, versioned data.</summary>
internal static class RunSnapshotCodec
{
    internal static byte[] Encode(RunSnapshotDocument document)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value: document, options: _options);
    }

    internal static (RunSnapshotDocument document, FoundationSession.RunPublication publication) Decode(
            ReadOnlySpan<byte> data, RunDefinitions definitions)
    {
        var reader = new Utf8JsonReader(jsonData: data);
        using var json = JsonDocument.ParseValue(reader: ref reader);
        if (reader.Read())
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_DOCUMENT");
        }

        ValidateArrayEntries(element: json.RootElement);
        RunSnapshotDocument document = json.RootElement.Deserialize<RunSnapshotDocument>(options: _options)
                ?? throw new InvalidDataException(message: "RUN_SNAPSHOT_DOCUMENT");
        if (document.Version != Version || document.Elapsed < 0)
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_VERSION_OR_CLOCK");
        }

        RunWorld world = Restore(row: document.World);
        world.Validate();
        RunPopulation population = Restore(row: document.Population);
        RunInventory inventory = RestoreInventory(row: document.Inventory, definitions: definitions);
        if (!RunInventoryRules.IsValid(inventory: inventory, definitions: definitions, population: population))
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_INVENTORY");
        }

        var snapshot = new FoundationSnapshot(sessionId: document.SessionId, revision: document.Revision,
                participants: document.Participants.OrderBy(keySelector: row => row.Slot)
                        .Select(selector: row => RestoreParticipant(row: row, sessionId: document.SessionId)));
        var root = new FoundationSession.RunPublication(snapshot: snapshot, world: world,
                elapsedMilliseconds: document.Elapsed, definitions: definitions, population: population,
                inventory: inventory, combat: Restore(row: document.Combat), loot: Restore(row: document.Loot),
                succession: Restore(row: document.Succession));
        ValidateRun(root: root, document: document);
        return (document, root);
    }

    internal static RunSnapshotDocument Capture(FoundationSession.RunPublication root, IntentData[] intents,
            FoodReceiptData[] foodReceipts, AttackReceiptData[] attackReceipts, RespawnReceiptData[] respawnReceipts,
            ItemReceiptData[] itemReceipts)
    {
        return new RunSnapshotDocument
        {
            Version = Version,
            SessionId = root.Snapshot.SessionId,
            Revision = root.Snapshot.Revision,
            Elapsed = root.ElapsedMilliseconds,
            Participants = [.. root.Snapshot.Participants.Select(selector: Capture)],
            World = Capture(row: root.World),
            Population = Capture(row: root.Population),
            Inventory = CaptureInventory(inventory: root.Inventory, definitions: root.Definitions),
            Combat = Capture(row: root.Combat),
            Loot = Capture(row: root.Loot),
            Succession = Capture(row: root.Succession),
            Intents = intents,
            FoodReceipts = foodReceipts,
            AttackReceipts = attackReceipts,
            RespawnReceipts = respawnReceipts,
            ItemReceipts = itemReceipts,
        };
    }

    private const int Version = 1;
    private static readonly JsonSerializerOptions _options = new()
    {
        AllowDuplicateProperties = false,
        RespectNullableAnnotations = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    private static void ValidateArrayEntries(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                ValidateArrayEntries(element: property.Value);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement entry in element.EnumerateArray())
            {
                Require(valid: entry.ValueKind != JsonValueKind.Null, problem: "NULL_RECORD");
                ValidateArrayEntries(element: entry);
            }
        }
    }

    private static void ValidateRun(FoundationSession.RunPublication root, RunSnapshotDocument document)
    {
        var survivors = root.Population.Survivors
                .ToDictionary(keySelector: survivor => survivor.Id, comparer: StringComparer.Ordinal);
        ValidatePopulation(root: root, survivors: survivors, issued: document.Population.IssuedBindings);
        ValidateLoot(root: root, survivors: survivors);
        ValidateCombat(root: root, survivors: survivors);
        ValidateSuccession(root: root, document: document);
        ValidateReceipts(root: root, document: document, survivors: survivors);
        Require(valid: ReferenceEquals(objA: root.Inventory, objB: RunQuickReferences.Invalidate(
                inventory: root.Inventory, definitions: root.Definitions)), problem: "QUICK_REFERENCES");
    }

    private static void ValidatePopulation(FoundationSession.RunPublication root,
            Dictionary<string, RunPopulation.Survivor> survivors, string[] issued)
    {
        RunPopulation population = root.Population;
        Require(valid: population.Survivors.Any(predicate: survivor => !survivor.Hostile)
                && population.LastControlGeneration >= 1
                && population.IssuedBindings.Count == issued.Length
                && issued.All(predicate: id => ValidId(id: id)), problem: "POPULATION");
        foreach (RunPopulation.Survivor survivor in survivors.Values)
        {
            Require(valid: ValidSurvivor(root: root, survivor: survivor), problem: "SURVIVOR");
        }

        var generations = new HashSet<long>();
        var occupied = new HashSet<string>(comparer: StringComparer.Ordinal);
        var bindings = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (RunPopulation.Lease lease in population.Leases)
        {
            Require(valid: ValidLease(root: root, lease: lease) && generations.Add(item: lease.Generation)
                    && occupied.Add(item: lease.SurvivorId) && bindings.Add(item: lease.BindingId)
                    && survivors.TryGetValue(key: lease.SurvivorId, value: out RunPopulation.Survivor? owner)
                    && owner.EligibleForControl && HasParticipant(root: root, lease: lease), problem: "LEASE");
        }

        foreach (RunDeathAndLoot.Death death in root.Loot.Deaths.Values)
        {
            if (death.Lease is { } retired)
            {
                Require(valid: ValidLease(root: root, lease: retired) && generations.Add(item: retired.Generation)
                        && StringComparer.Ordinal.Equals(x: retired.SurvivorId, y: death.SurvivorId),
                        problem: "RETIRED_LEASE");
            }
        }

        foreach (FoundationParticipantState participant in root.Snapshot.Participants)
        {
            bool death = root.Loot.Deaths.Values.Any(predicate: entry => entry.Lease is { } retired
                    && MatchesParticipant(participant: participant, lease: retired));
            Require(valid: population.IssuedBindings.Contains(item: participant.BindingId)
                    && participant.Actor.ElapsedMilliseconds <= root.ElapsedMilliseconds
                    && (bindings.Contains(item: participant.BindingId) || participant.Ready && death),
                    problem: "PARTICIPATION");
        }
    }

    private static bool ValidSurvivor(FoundationSession.RunPublication root, RunPopulation.Survivor survivor)
    {
        long now = root.ElapsedMilliseconds;
        return ValidId(id: survivor.Id) && ValidId(id: survivor.Role)
                && survivor.PositionSubunitsExact is >= -59000 and <= 59000
                && survivor.Facing is -1 or 1 && survivor.Health >= 0
                && survivor.Health <= root.Definitions.MaximumHealth && survivor.Stamina >= 0
                && survivor.Stamina <= root.Definitions.MaximumStamina && survivor.HeightSubunits >= 0
                && survivor.HeightSubunits <= root.Definitions.Locomotion.JumpHeight * RunWorld.SubunitsPerUnit
                && (survivor.JumpStartedAt.HasValue || survivor.HeightSubunits == 0)
                && ValidPast(time: survivor.LastStaminaSpend, now: now)
                && ValidPast(time: survivor.RestStartedAt, now: now)
                && ValidPast(time: survivor.JumpStartedAt, now: now)
                && ValidMotion(survivor: survivor, now: now)
                && survivor.FoodAvailableAt is not < 0 && survivor.AttackAvailableAt is not < 0
                && Enum.IsDefined(value: survivor.SprintBlock)
                && (survivor.SprintFence is not { } stamp || stamp.generation >= 1
                        && stamp.generation <= root.Population.LastControlGeneration && stamp.sequence >= 1)
                && (survivor.Health > 0 || root.Loot.Deaths.ContainsKey(key: survivor.Id));
    }

    private static bool ValidMotion(RunPopulation.Survivor survivor, long now)
    {
        return (survivor.MovementAt.HasValue ? survivor.MovementPose is "walk" or "faster"
                : survivor.MovementPose is "idle")
                && ValidPast(time: survivor.MovementAt, now: now)
                && ValidPast(time: survivor.StrikeAt, now: now)
                && ValidPast(time: survivor.FlinchAt, now: now)
                && (survivor.StrikeAt is not { } strike || survivor.AttackAvailableAt > strike)
                && (survivor.StrikeAim is null || survivor.StrikeAt.HasValue && survivor.StrikeAim.Valid);
    }

    private static bool ValidLease(FoundationSession.RunPublication root, RunPopulation.Lease lease)
    {
        return StringComparer.Ordinal.Equals(x: lease.SessionId, y: root.Snapshot.SessionId)
                && root.Population.IssuedBindings.Contains(item: lease.BindingId) && ValidId(id: lease.PlayerId)
                && lease.Slot is >= 1 and <= 4 && lease.Generation >= 1
                && lease.Generation <= root.Population.LastControlGeneration;
    }

    private static bool HasParticipant(FoundationSession.RunPublication root, RunPopulation.Lease lease)
    {
        return root.Snapshot.Participants.Any(predicate: participant => MatchesParticipant(participant: participant,
                lease: lease));
    }

    private static bool MatchesParticipant(FoundationParticipantState participant, RunPopulation.Lease lease)
    {
        return StringComparer.Ordinal.Equals(x: participant.BindingId, y: lease.BindingId)
                && StringComparer.Ordinal.Equals(x: participant.Actor.PlayerId, y: lease.PlayerId)
                && participant.Slot == lease.Slot;
    }

    private static void ValidateLoot(FoundationSession.RunPublication root,
            Dictionary<string, RunPopulation.Survivor> survivors)
    {
        RunDeathAndLoot.State loot = root.Loot;
        var corpses = new HashSet<string>(comparer: StringComparer.Ordinal);
        var serials = new Dictionary<long, string>();
        Require(valid: loot.LastTargetId >= 0, problem: "LOOT_COUNTER");
        foreach (RunDeathAndLoot.Death death in loot.Deaths.Values)
        {
            Require(valid: corpses.Add(item: death.CorpseId) && ValidPast(time: death.At, now: root.ElapsedMilliseconds)
                    && ValidPoint(point: death.Contact)
                    && survivors.TryGetValue(key: death.SurvivorId, value: out RunPopulation.Survivor? survivor)
                    && survivor.Health == 0 && (death.Lease is null || !survivor.Hostile), problem: "DEATH");
            CheckTargetSerial(id: death.CorpseId, kind: RunWorld.Access.Corpse, last: loot.LastTargetId,
                    serials: serials);
            if (loot.Targets.TryGetValue(key: death.CorpseId, value: out RunDeathAndLoot.Target? corpse))
            {
                Require(valid: corpse.Generated && corpse.Anchor.Kind == RunWorld.Access.Corpse
                        && corpse.Anchor.Contact == death.Contact
                        && (corpse.EmptySince is null || corpse.EmptySince >= death.At),
                        problem: "CORPSE_LINK");
            }
        }

        foreach (RunDeathAndLoot.Target target in loot.Targets.Values)
        {
            ValidateTarget(root: root, target: target, corpses: corpses, serials: serials);
        }

        foreach (RunInventory.Item item in root.Inventory.Items)
        {
            Require(valid: item.Location switch
            {
                RunInventory.Ground pile => HasTarget(loot: loot, id: pile.PileId, kind: RunWorld.Access.Pile),
                RunInventory.CorpseEquipment corpse => HasTarget(loot: loot, id: corpse.CorpseId,
                        kind: RunWorld.Access.Corpse),
                _ => true,
            }, problem: "LOOT_OWNERSHIP");
        }

        Require(valid: root.World.Anchors.All(predicate: anchor => anchor.Kind is not
                (RunWorld.Access.Corpse or RunWorld.Access.Pile) || HasTarget(loot: loot, id: anchor.Id,
                        kind: anchor.Kind)), problem: "LOOT_ANCHOR");
    }

    private static void ValidateTarget(FoundationSession.RunPublication root, RunDeathAndLoot.Target target,
            HashSet<string> corpses, Dictionary<long, string> serials)
    {
        RunWorld.Anchor anchor = target.Anchor;
        Require(valid: ValidId(id: anchor.Id) && ValidPoint(point: anchor.Contact)
                && anchor.Kind is RunWorld.Access.Corpse or RunWorld.Access.Pile
                && ValidPast(time: target.EmptySince, now: root.ElapsedMilliseconds)
                && root.Inventory.HasLoot(id: anchor.Id) == (target.EmptySince is null)
                && (target.EmptySince is not { } empty || root.ElapsedMilliseconds - empty
                        < root.Definitions.LootEmptyDelay), problem: "LOOT_TIMER");
        if (target.Generated)
        {
            Require(valid: root.World.Anchors.Contains(item: anchor)
                    && (anchor.Kind != RunWorld.Access.Corpse || corpses.Contains(item: anchor.Id)),
                    problem: "GENERATED_LOOT");
            CheckTargetSerial(id: anchor.Id, kind: anchor.Kind, last: root.Loot.LastTargetId, serials: serials);
        }
    }

    private static void CheckTargetSerial(string id, RunWorld.Access kind, long last,
            Dictionary<long, string> serials)
    {
        string prefix = kind == RunWorld.Access.Corpse ? "run.corpse." : "run.pile.";
        Require(valid: id.StartsWith(value: prefix, comparisonType: StringComparison.Ordinal)
                && long.TryParse(s: id.AsSpan(start: prefix.Length), style: NumberStyles.None,
                        provider: CultureInfo.InvariantCulture, result: out long serial)
                && serial > 0 && serial <= last && StringComparer.Ordinal.Equals(x: id,
                        y: prefix + serial.ToString(provider: CultureInfo.InvariantCulture))
                && (serials.TryAdd(key: serial, value: id) || StringComparer.Ordinal.Equals(x: id, y: serials[serial])),
                problem: "LOOT_ID");
    }

    private static bool HasTarget(RunDeathAndLoot.State loot, string id, RunWorld.Access kind)
    {
        return loot.Targets.TryGetValue(key: id, value: out RunDeathAndLoot.Target? target)
                && target.Anchor.Kind == kind;
    }

    private static void ValidateCombat(FoundationSession.RunPublication root,
            Dictionary<string, RunPopulation.Survivor> survivors)
    {
        RunCombat.State combat = root.Combat;
        Require(valid: combat.LastArrowId >= 0, problem: "ARROW_COUNTER");
        var ids = new HashSet<long>();
        foreach (RunProjectiles.Arrow arrow in combat.Arrows)
        {
            Require(valid: arrow.Id > 0 && arrow.Id <= combat.LastArrowId && ids.Add(item: arrow.Id)
                    && survivors.TryGetValue(key: arrow.SourceId, value: out RunPopulation.Survivor? shooter)
                    && shooter.Hostile == arrow.Hostile && arrow.Damage is >= 0 and <= 10000
                    && ValidPast(time: arrow.LaunchedAt, now: root.ElapsedMilliseconds)
                    && arrow.ExpiresAt > root.ElapsedMilliseconds && arrow.ExpiresAt <= arrow.LaunchedAt + 600000
                    && arrow.Aim.Valid && ValidVector(vector: arrow.Position) && ValidVector(vector: arrow.Velocity)
                    && arrow.Velocity != (0, 0), problem: "ARROW");
        }

        foreach (RunProjectiles.Impact impact in combat.Impacts)
        {
            Require(valid: impact.ArrowId > 0 && impact.ArrowId <= combat.LastArrowId && ids.Add(item: impact.ArrowId)
                    && ValidId(id: impact.TargetId) && Enum.IsDefined(value: impact.Kind)
                    && ValidPast(time: impact.At, now: root.ElapsedMilliseconds)
                    && ValidImpact(impact: impact, root: root, survivors: survivors), problem: "ARROW_IMPACT");
        }

        var fatalities = new HashSet<string>(comparer: StringComparer.Ordinal);
        foreach (RunDamage.Hit hit in combat.FatalHits)
        {
            Require(valid: ValidHit(hit: hit, root: root, survivors: survivors) && hit.Lethal
                    && fatalities.Add(item: hit.TargetId)
                    && root.Loot.Deaths.TryGetValue(key: hit.TargetId, value: out RunDeathAndLoot.Death? death)
                    && death.At == hit.At, problem: "FATAL_HIT");
        }

        Require(valid: fatalities.Count == root.Loot.Deaths.Count, problem: "FATAL_HIT_COUNT");
    }

    private static bool ValidImpact(RunProjectiles.Impact impact, FoundationSession.RunPublication root,
            Dictionary<string, RunPopulation.Survivor> survivors)
    {
        return impact.Kind switch
        {
            RunProjectiles.Kind.Survivor => impact.Hit is { } hit && hit.At == impact.At
                    && hit.Damage == impact.Damage && StringComparer.Ordinal.Equals(x: hit.TargetId, y: impact.TargetId)
                    && ValidHit(hit: hit, root: root, survivors: survivors)
                    && (!hit.Lethal || root.Combat.FatalHits.Contains(item: hit)),
            RunProjectiles.Kind.Terrain => impact.Hit is null && impact.Damage == 0,
            RunProjectiles.Kind.Defense => impact.Hit is null && impact.Damage is >= 0 and <= 10000,
            _ => false,
        };
    }

    private static bool ValidHit(RunDamage.Hit hit, FoundationSession.RunPublication root,
            Dictionary<string, RunPopulation.Survivor> survivors)
    {
        return survivors.TryGetValue(key: hit.SourceId, value: out RunPopulation.Survivor? source)
                && survivors.TryGetValue(key: hit.TargetId, value: out RunPopulation.Survivor? target)
                && source.Hostile != target.Hostile && hit.Raw is >= 0 and <= 10000
                && hit.Damage >= 0 && hit.Damage <= hit.Raw && hit.HealthBefore > 0
                && hit.HealthBefore <= root.Definitions.MaximumHealth
                && hit.HealthAfter == Math.Max(val1: 0, val2: hit.HealthBefore - hit.Damage)
                && ValidPast(time: hit.At, now: root.ElapsedMilliseconds);
    }

    private static void ValidateSuccession(FoundationSession.RunPublication root, RunSnapshotDocument document)
    {
        RunSuccession.State state = root.Succession;
        var waiting = new HashSet<string>(comparer: StringComparer.Ordinal);
        Require(valid: state.DrawPosition is >= 0 and <= ReplayRandom.MaxDrawPosition
                && StringComparer.Ordinal.Equals(x: document.Succession.RuntimeContract,
                        y: ReplayRandom.PinnedRuntimeContract), problem: "RANDOM_CONTINUATION");
        foreach (RunSuccession.Wait ticket in state.Waiting)
        {
            RunDeathAndLoot.Death? death = root.Loot.Deaths.Values.Where(predicate: previous =>
                    previous.Lease is { } lease && StringComparer.Ordinal.Equals(x: lease.BindingId,
                            y: ticket.BindingId)).MaxBy(keySelector: previous => previous.Lease?.Generation);
            Require(valid: waiting.Add(item: ticket.BindingId) && death?.Lease is { } retired
                    && retired.Generation == ticket.Epoch && retired.Slot == ticket.Slot
                    && ticket.At >= death.At + root.Definitions.RespawnDelay && ticket.At <= root.ElapsedMilliseconds
                    && root.Population.FindLease(bindingId: ticket.BindingId) is null
                    && root.Snapshot.Participants.Any(predicate: participant => participant.Ready
                            && MatchesParticipant(participant: participant, lease: retired))
                    && document.RespawnReceipts.Any(predicate: receipt => receipt.Epoch == ticket.Epoch
                            && receipt.Status == RunSuccession.Status.Waiting
                            && StringComparer.Ordinal.Equals(x: receipt.BindingId, y: ticket.BindingId)),
                    problem: "WAITING_SUCCESSOR");
        }
    }

    private static void ValidateReceipts(FoundationSession.RunPublication root, RunSnapshotDocument document,
            Dictionary<string, RunPopulation.Survivor> survivors)
    {
        RunPopulation population = root.Population;
        Require(valid: document.Intents.All(predicate: row => ValidFence(population: population,
                binding: row.BindingId, generation: row.Generation) && row.Sequence > 0
                && row.Direction is >= -1 and <= 1),
                problem: "MOTION_RECEIPT");
        Require(valid: document.FoodReceipts.All(predicate: row => ValidFence(population: population,
                binding: row.BindingId, generation: row.Generation) && row.Sequence > 0
                && Enum.IsDefined(value: row.Status)), problem: "FOOD_RECEIPT");
        Require(valid: document.AttackReceipts.All(predicate: row => survivors.ContainsKey(key: row.Id)
                && row.Generation >= 0 && row.Generation <= population.LastControlGeneration && row.Sequence > 0
                && Enum.IsDefined(value: row.Status)), problem: "ATTACK_RECEIPT");
        Require(valid: document.RespawnReceipts.All(predicate: row => row.Sequence > 0
                && Enum.IsDefined(value: row.Status) && root.Loot.Deaths.Values.Any(predicate: death =>
                        death.Lease is { } lease && lease.Generation == row.Epoch
                                && StringComparer.Ordinal.Equals(x: lease.BindingId, y: row.BindingId))),
                problem: "RESPAWN_RECEIPT");
        Require(valid: document.ItemReceipts.All(predicate: row => ValidItemReceipt(root: root, row: row)),
                problem: "ITEM_RECEIPT");
    }

    private static bool ValidItemReceipt(FoundationSession.RunPublication root, ItemReceiptData row)
    {
        RunDefinitions.Size stash = root.Inventory.StashGrid;
        return ValidFence(population: root.Population, binding: row.BindingId, generation: row.Generation)
                && row.Sequence >= 0 && Enum.IsDefined(value: row.Status)
                && (row.Selection is not { } selection || !row.SelectionLost && ValidId(id: selection.Id)
                        && Enum.IsDefined(value: selection.Kind) && selection.Token > 0
                        && selection.Token <= row.Sequence
                        && selection.PageIndex >= 0 && selection.PageIndex < root.Inventory.StashPageCount
                        && (selection.Kind == RunWorld.Access.Stash || selection.PageIndex == 0))
                && (row.StashEffect is not { } effect || effect.PageNumber >= 1
                        && effect.PageNumber <= root.Inventory.StashPageCount && effect.MorePages >= 0
                        && effect.ChangedCells.Select(selector: cell => (cell.X, cell.Y)).Distinct().Count()
                                == effect.ChangedCells.Length
                        && effect.ChangedCells.All(predicate: cell => cell.X >= 0 && cell.X < stash.Width
                                && cell.Y >= 0 && cell.Y < stash.Height));
    }

    private static bool ValidFence(RunPopulation population, string binding, long generation)
    {
        return population.IssuedBindings.Contains(item: binding) && generation >= 1
                && generation <= population.LastControlGeneration;
    }

    private static bool ValidId(string id)
    {
        return !string.IsNullOrWhiteSpace(value: id) && id.Length <= 128;
    }

    private static bool ValidPast(decimal? time, long now)
    {
        return time is null || time >= 0 && time <= now;
    }

    private static bool ValidPoint(RunWorld.Point point)
    {
        return point.X is >= -1000000000 and <= 1000000000 && point.Y is >= -1000000000 and <= 1000000000;
    }

    private static bool ValidVector((decimal x, decimal y) vector)
    {
        return vector.x is >= -1000100000 and <= 1000100000 && vector.y is >= -1000100000 and <= 1000100000;
    }

    private static void Require(bool valid, string problem)
    {
        if (!valid)
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_" + problem);
        }
    }

    private static ParticipantData Capture(FoundationParticipantState row)
    {
        return new ParticipantData
        {
            BindingId = row.BindingId,
            Label = row.Label,
            Slot = row.Slot,
            Ready = row.Ready,
            PlayerId = row.Actor.PlayerId,
            ActorId = row.Actor.ActorId,
            Position = row.Actor.PositionSubunits,
            Facing = row.Actor.Facing,
            Elapsed = row.Actor.ElapsedMilliseconds,
        };
    }

    private static FoundationParticipantState RestoreParticipant(ParticipantData row, string sessionId)
    {
        var actor = new FoundationActorState(sessionId: sessionId, playerId: row.PlayerId, actorId: row.ActorId,
                positionSubunits: row.Position, facing: row.Facing, elapsedMilliseconds: row.Elapsed);
        return new FoundationParticipantState(bindingId: row.BindingId, label: row.Label, slot: row.Slot,
                ready: row.Ready, actor: actor);
    }

    private static InventoryData CaptureInventory(RunInventory inventory, RunDefinitions definitions)
    {
        return new InventoryData
        {
            StashGrid = Capture(row: inventory.StashGrid),
            LegacyStash = inventory.SavedLayout?.LegacyStash == true,
            StashPageCount = inventory.StashPageCount,
            NextItemId = inventory.NextItemId,
            MaximumContainerLevels = inventory.SavedLayout?.MaximumContainerLevels
                    ?? definitions.MaximumContainerLevels,
            Items = [.. inventory.Items.OrderBy(keySelector: item => item.Id).Select(selector: item =>
                    CaptureItem(item: item, saved: inventory.SavedLayout, definitions: definitions))],
            QuickReferences = [.. inventory.QuickReferences.OrderBy(keySelector: entry => entry.Key.survivor,
                    comparer: StringComparer.Ordinal).ThenBy(keySelector: entry => entry.Key.slot)
                    .Select(selector: entry => new QuickReferenceData
                    {
                        SurvivorId = entry.Key.survivor, Slot = entry.Key.slot, ItemId = entry.Value,
                    })],
        };
    }

    private static RunInventory RestoreInventory(InventoryData row, RunDefinitions definitions)
    {
        ImmutableArray<RunInventory.Item>.Builder items =
                ImmutableArray.CreateBuilder<RunInventory.Item>(initialCapacity: row.Items.Length);
        ImmutableDictionary<long, (RunDefinitions.Item definition, bool legacy)>.Builder layouts =
                ImmutableDictionary.CreateBuilder<long, (RunDefinitions.Item definition, bool legacy)>();
        foreach (ItemData item in row.Items.OrderBy(keySelector: item => item.Id))
        {
            if (!definitions.Items.TryGetValue(key: item.DefinitionId, value: out RunDefinitions.Item? current))
            {
                throw new InvalidDataException(message: $"RUN_SNAPSHOT_DEFINITION {item.DefinitionId}");
            }

            RunDefinitions.Item original = RestoreLayout(row: item.Layout, current: current);
            RunSavedLayout.Validate(item: original);
            layouts.Add(key: item.Id, value: (original, item.Layout.Legacy
                    || RunSavedLayout.IsDifferent(saved: original, current: current)));
            items.Add(item: new RunInventory.Item(id: item.Id, definitionId: item.DefinitionId, quantity: item.Quantity,
                    condition: item.Condition, location: Restore(row: item.Location), storageGrid:
                    item.Storage is { } storage ? Restore(row: storage) : null));
        }

        if (row.MaximumContainerLevels <= 0)
        {
            throw new InvalidDataException(message: "RUN_SNAPSHOT_CONTAINER_LIMIT");
        }

        RunDefinitions.Size stash = Restore(row: row.StashGrid);
        var saved = new RunSavedLayout(maximumContainerLevels: row.MaximumContainerLevels,
                legacyStash: row.LegacyStash || stash != definitions.StashGrid,
                items: layouts.ToImmutable());
        return new RunInventory(items: items.MoveToImmutable(), stashGrid: stash,
                stashPageCount: row.StashPageCount, nextItemId: row.NextItemId, savedLayout: saved,
                quickReferences: row.QuickReferences.ToImmutableDictionary(
                        keySelector: reference => (reference.SurvivorId, reference.Slot),
                        elementSelector: reference => reference.ItemId));
    }

    private static ItemData CaptureItem(RunInventory.Item item, RunSavedLayout? saved, RunDefinitions definitions)
    {
        RunDefinitions.Item rule = definitions.Items[item.DefinitionId];
        bool legacy = false;
        if (saved is not null && saved.Items.TryGetValue(key: item.Id,
                value: out (RunDefinitions.Item definition, bool legacy) original))
        {
            (rule, legacy) = original;
        }

        return new ItemData
        {
            Id = item.Id,
            DefinitionId = item.DefinitionId,
            Quantity = item.Quantity,
            Condition = item.Condition,
            Storage = item.StorageGrid is { } storage ? Capture(row: storage) : null,
            Location = Capture(row: item.Location),
            Layout = CaptureLayout(rule: rule, legacy: legacy),
        };
    }

    private static ItemLayoutData CaptureLayout(RunDefinitions.Item rule, bool legacy)
    {
        return new ItemLayoutData
        {
            Footprint = Capture(row: rule.Footprint),
            Rotatable = rule.Rotatable,
            MaximumStack = rule.MaxStack,
            Storage = rule.Storage is { } storage ? Capture(row: storage) : null,
            MaximumCondition = rule.MaximumCondition,
            Slot = rule.Slot,
            Pack = rule.Appearance.Pack,
            Icon = rule.Appearance.CanonicalIcon,
            FitFile = rule.Appearance.FitFile,
            FitItem = rule.Appearance.FitItem,
            Legacy = legacy,
        };
    }

    private static RunDefinitions.Item RestoreLayout(ItemLayoutData row, RunDefinitions.Item current)
    {
        var appearance = new RunDefinitions.Appearance(pack: row.Pack, canonicalIcon: row.Icon,
                fitFile: row.FitFile, fitItem: row.FitItem);
        return new RunDefinitions.Item(id: current.Id, footprint: Restore(row: row.Footprint),
                maxStack: row.MaximumStack, weightKilograms: current.WeightKilograms, rotatable: row.Rotatable,
                slot: row.Slot, storage: row.Storage is { } storage ? Restore(row: storage) : null,
                maximumCondition: row.MaximumCondition, appearance: appearance);
    }

    private static LocationData Capture(RunInventory.Location row)
    {
        (LocationKind kind, string? target, string? slot) = row switch
        {
            RunInventory.Equipment held => (LocationKind.Equipment, held.SurvivorId, held.Slot),
            RunInventory.CorpseEquipment corpse => (LocationKind.CorpseEquipment, corpse.CorpseId, corpse.Slot),
            RunInventory.Ground pile => (LocationKind.Ground, pile.PileId, null),
            RunInventory.StashCell => (LocationKind.Stash, null, null),
            RunInventory.ContainerCell => (LocationKind.Container, null, null),
            _ => throw new InvalidDataException(message: "RUN_SNAPSHOT_LOCATION"),
        };
        var cell = row as RunInventory.GridCell;
        return new LocationData
        {
            Kind = kind,
            TargetId = target,
            Slot = slot,
            GridId = cell?.Grid.id,
            X = cell?.X,
            Y = cell?.Y,
            Rotated = cell?.Rotated,
            Footprint = cell is null ? null : Capture(row: cell.Footprint),
        };
    }

    private static RunInventory.Location Restore(LocationData row)
    {
        if (row.TargetId is null && row.Slot is null && row.GridId is { } grid && row.X is { } x
                && row.Y is { } y && row.Rotated is { } rotated && row.Footprint is { } footprint)
        {
            RunDefinitions.Size size = Restore(row: footprint);
            if (row.Kind is LocationKind.Stash && grid is >= 0 and <= int.MaxValue)
            {
                return new RunInventory.StashCell(page: (int)grid, x: x, y: y, rotated: rotated, footprint: size);
            }

            if (row.Kind is LocationKind.Container)
            {
                return new RunInventory.ContainerCell(containerId: grid, x: x, y: y, rotated: rotated, footprint: size);
            }
        }

        if (row is { TargetId: { } target, GridId: null, X: null, Y: null, Rotated: null, Footprint: null })
        {
            return row.Kind switch
            {
                LocationKind.Equipment when row.Slot is { } slot =>
                    new RunInventory.Equipment(survivorId: target, slot: slot),
                LocationKind.CorpseEquipment when row.Slot is { } slot =>
                    new RunInventory.CorpseEquipment(corpseId: target, slot: slot),
                LocationKind.Ground when row.Slot is null => new RunInventory.Ground(pileId: target),
                _ => throw new InvalidDataException(message: "RUN_SNAPSHOT_LOCATION"),
            };
        }

        throw new InvalidDataException(message: "RUN_SNAPSHOT_LOCATION");
    }

    private static LootData Capture(RunDeathAndLoot.State row)
    {
        return new LootData
        {
            Targets = [.. row.Targets.Values.OrderBy(keySelector: target => target.Anchor.Id,
                    comparer: StringComparer.Ordinal).Select(selector: Capture)],
            Deaths = [.. row.Deaths.Values.OrderBy(keySelector: death => death.SurvivorId,
                    comparer: StringComparer.Ordinal).Select(selector: Capture)],
            LastTargetId = row.LastTargetId,
        };
    }

    private static RunDeathAndLoot.State Restore(LootData row)
    {
        return new RunDeathAndLoot.State(targets: row.Targets.ToImmutableDictionary(
                keySelector: target => target.Anchor.Id, elementSelector: Restore, keyComparer: StringComparer.Ordinal),
                deaths: row.Deaths.ToImmutableDictionary(keySelector: death => death.SurvivorId,
                        elementSelector: Restore, keyComparer: StringComparer.Ordinal), lastTargetId: row.LastTargetId);
    }

    private static PointData Capture(RunWorld.Point row)
    {
        return new PointData
        {
            X = row.X,
            Y = row.Y,
        };
    }

    private static RunWorld.Point Restore(PointData row)
    {
        return new RunWorld.Point(x: row.X, y: row.Y);
    }

    private static VectorData Capture((decimal x, decimal y) row)
    {
        return new VectorData
        {
            X = row.x,
            Y = row.y,
        };
    }

    private static (decimal x, decimal y) Restore(VectorData row)
    {
        return (row.X, row.Y);
    }

    private static SizeData Capture(RunDefinitions.Size row)
    {
        return new SizeData
        {
            Width = row.Width,
            Height = row.Height,
        };
    }

    private static RunDefinitions.Size Restore(SizeData row)
    {
        return new RunDefinitions.Size(width: row.Width, height: row.Height);
    }

    private static RangesData Capture(RunWorld.Ranges row)
    {
        return new RangesData
        {
            RefugeUnits = row.RefugeUnits,
            DangerUnits = row.DangerUnits,
            LocalUnits = row.LocalUnits,
            StashUnits = row.StashUnits,
        };
    }

    private static RunWorld.Ranges Restore(RangesData row)
    {
        return new RunWorld.Ranges(refugeUnits: row.RefugeUnits, dangerUnits: row.DangerUnits,
                localUnits: row.LocalUnits, stashUnits: row.StashUnits);
    }

    private static WorldData Capture(RunWorld row)
    {
        return new WorldData
        {
            Origin = Capture(row: row.Origin),
            Ground = row.GroundContactSubunits,
            MainBuilding = Capture(row: row.MainBuilding),
            Ranges = Capture(row: row.Settings),
            Defenses = [.. row.Defenses.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                    .Select(selector: Capture)],
            Blockers = [.. row.Blockers.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                    .Select(selector: Capture)],
            Hostiles = [.. row.Hostiles.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                    .Select(selector: Capture)],
            Anchors = [.. row.Anchors.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                    .Select(selector: Capture)],
        };
    }

    private static RunWorld Restore(WorldData row)
    {
        return new RunWorld(origin: Restore(row: row.Origin), groundContactSubunits: row.Ground,
                mainBuilding: Restore(row: row.MainBuilding), ranges: Restore(row: row.Ranges),
                defenses: [.. row.Defenses.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                        .Select(selector: Restore)],
                blockers: [.. row.Blockers.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                        .Select(selector: Restore)],
                hostiles: [.. row.Hostiles.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                        .Select(selector: Restore)],
                anchors: [.. row.Anchors.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                        .Select(selector: Restore)]);
    }

    private static DefenseData Capture(RunWorld.Defense row)
    {
        return new DefenseData
        {
            Id = row.Id,
            Contact = Capture(row: row.Contact),
            Completed = row.Completed,
            Living = row.Living,
            IsWall = row.IsWall,
            Health = row.Health,
        };
    }

    private static RunWorld.Defense Restore(DefenseData row)
    {
        return new RunWorld.Defense(id: row.Id, contact: Restore(row: row.Contact), completed: row.Completed,
                living: row.Living, isWall: row.IsWall, health: row.Health);
    }

    private static BlockerData Capture(RunWorld.Blocker row)
    {
        return new BlockerData
        {
            Id = row.Id,
            Minimum = Capture(row: row.Minimum),
            Maximum = Capture(row: row.Maximum),
        };
    }

    private static RunWorld.Blocker Restore(BlockerData row)
    {
        return new RunWorld.Blocker(id: row.Id, minimum: Restore(row: row.Minimum),
                maximum: Restore(row: row.Maximum));
    }

    private static HostileData Capture(RunWorld.Hostile row)
    {
        return new HostileData
        {
            Id = row.Id,
            Contact = Capture(row: row.Contact),
        };
    }

    private static RunWorld.Hostile Restore(HostileData row)
    {
        return new RunWorld.Hostile(id: row.Id, contact: Restore(row: row.Contact));
    }

    private static AnchorData Capture(RunWorld.Anchor row)
    {
        return new AnchorData
        {
            Id = row.Id,
            Contact = Capture(row: row.Contact),
            Kind = row.Kind,
        };
    }

    private static RunWorld.Anchor Restore(AnchorData row)
    {
        return new RunWorld.Anchor(id: row.Id, contact: Restore(row: row.Contact), kind: row.Kind);
    }

    private static PopulationData Capture(RunPopulation row)
    {
        return new PopulationData
        {
            Survivors = [.. row.Survivors.OrderBy(keySelector: item => item.Id, comparer: StringComparer.Ordinal)
                    .Select(selector: Capture)],
            Leases = [.. row.Leases.OrderBy(keySelector: item => item.Slot)
                    .Select(selector: Capture)],
            IssuedBindings = [.. row.IssuedBindings.Order(comparer: StringComparer.Ordinal)],
            LastControlGeneration = row.LastControlGeneration,
        };
    }

    private static RunPopulation Restore(PopulationData row)
    {
        return new RunPopulation(survivors: [.. row.Survivors.OrderBy(keySelector: survivor => survivor.Id,
                comparer: StringComparer.Ordinal).Select(selector: Restore)],
                leases: [.. row.Leases.OrderBy(keySelector: lease => lease.Slot).Select(selector: Restore)],
                issuedBindings: row.IssuedBindings.ToImmutableHashSet(equalityComparer: StringComparer.Ordinal),
                lastControlGeneration: row.LastControlGeneration);
    }

    private static SurvivorData Capture(RunPopulation.Survivor row)
    {
        return new SurvivorData
        {
            Id = row.Id,
            Position = row.PositionSubunitsExact,
            Facing = row.Facing,
            Health = row.Health,
            Stamina = row.Stamina,
            Role = row.Role,
            Height = row.HeightSubunits,
            LastStaminaSpend = row.LastStaminaSpend,
            JumpStartedAt = row.JumpStartedAt,
            SprintFence = row.SprintFence is { } fence
                    ? new FenceData { Generation = fence.generation, Sequence = fence.sequence } : null,
            SprintBlock = row.SprintBlock,
            RestStartedAt = row.RestStartedAt,
            FoodAvailableAt = row.FoodAvailableAt,
            AttackAvailableAt = row.AttackAvailableAt,
            MovementPose = row.MovementPose,
            MovementAt = row.MovementAt,
            StrikeAt = row.StrikeAt,
            StrikeAim = row.StrikeAim is { } aim ? Capture(row: aim) : null,
            FlinchAt = row.FlinchAt,
            Hostile = row.Hostile,
        };
    }

    private static RunPopulation.Survivor Restore(SurvivorData row)
    {
        return new RunPopulation.Survivor(id: row.Id, positionSubunits: 0, facing: row.Facing,
                health: row.Health, stamina: row.Stamina, role: row.Role)
        {
            PositionSubunitsExact = row.Position,
            HeightSubunits = row.Height,
            LastStaminaSpend = row.LastStaminaSpend,
            JumpStartedAt = row.JumpStartedAt,
            SprintFence = row.SprintFence is { } fence ? (fence.Generation, fence.Sequence) : null,
            SprintBlock = row.SprintBlock,
            RestStartedAt = row.RestStartedAt,
            FoodAvailableAt = row.FoodAvailableAt,
            AttackAvailableAt = row.AttackAvailableAt,
            MovementPose = row.MovementPose,
            MovementAt = row.MovementAt,
            StrikeAt = row.StrikeAt,
            StrikeAim = row.StrikeAim is { } aim ? Restore(row: aim) : null,
            FlinchAt = row.FlinchAt,
            Hostile = row.Hostile,
        };
    }

    private static LeaseData Capture(RunPopulation.Lease row)
    {
        return new LeaseData
        {
            SessionId = row.SessionId,
            PlayerId = row.PlayerId,
            BindingId = row.BindingId,
            Slot = row.Slot,
            SurvivorId = row.SurvivorId,
            Generation = row.Generation,
            Reassigned = row.Reassigned,
        };
    }

    private static RunPopulation.Lease Restore(LeaseData row)
    {
        return new RunPopulation.Lease(sessionId: row.SessionId, playerId: row.PlayerId,
                bindingId: row.BindingId, slot: row.Slot, survivorId: row.SurvivorId, generation: row.Generation,
                reassigned: row.Reassigned);
    }

    private static CombatData Capture(RunCombat.State row)
    {
        return new CombatData
        {
            FatalHits = [.. row.FatalHits.OrderBy(keySelector: hit => hit.At).ThenBy(
                    keySelector: hit => hit.TargetId, comparer: StringComparer.Ordinal).Select(selector: Capture)],
            Arrows = [.. row.Arrows.OrderBy(keySelector: item => item.Id)
                    .Select(selector: Capture)],
            Impacts = [.. row.Impacts.OrderBy(keySelector: impact => (impact.At, impact.ArrowId))
                    .Select(selector: Capture)],
            LastArrowId = row.LastArrowId,
        };
    }

    private static RunCombat.State Restore(CombatData row)
    {
        return new RunCombat.State(fatalHits: [.. row.FatalHits.OrderBy(keySelector: hit => hit.At)
                .ThenBy(keySelector: hit => hit.TargetId, comparer: StringComparer.Ordinal)
                .Select(selector: Restore)],
                arrows: [.. row.Arrows.OrderBy(keySelector: arrow => arrow.Id).Select(selector: Restore)],
                impacts: [.. row.Impacts.OrderBy(keySelector: impact => (impact.At, impact.ArrowId))
                        .Select(selector: Restore)], lastArrowId: row.LastArrowId);
    }

    private static HitData Capture(RunDamage.Hit row)
    {
        return new HitData
        {
            SourceId = row.SourceId,
            TargetId = row.TargetId,
            Raw = row.Raw,
            Damage = row.Damage,
            HealthBefore = row.HealthBefore,
            HealthAfter = row.HealthAfter,
            At = row.At,
        };
    }

    private static RunDamage.Hit Restore(HitData row)
    {
        return new RunDamage.Hit(sourceId: row.SourceId, targetId: row.TargetId, raw: row.Raw,
                damage: row.Damage, healthBefore: row.HealthBefore, healthAfter: row.HealthAfter, at: row.At);
    }

    private static ArrowData Capture(RunProjectiles.Arrow row)
    {
        return new ArrowData
        {
            Id = row.Id,
            SourceId = row.SourceId,
            Hostile = row.Hostile,
            Damage = row.Damage,
            LaunchedAt = row.LaunchedAt,
            ExpiresAt = row.ExpiresAt,
            Position = Capture(row: row.Position),
            Velocity = Capture(row: row.Velocity),
            Aim = Capture(row: row.Aim),
        };
    }

    private static RunProjectiles.Arrow Restore(ArrowData row)
    {
        return new RunProjectiles.Arrow(id: row.Id, sourceId: row.SourceId, hostile: row.Hostile,
                damage: row.Damage, launchedAt: row.LaunchedAt, expiresAt: row.ExpiresAt,
                position: Restore(row: row.Position), velocity: Restore(row: row.Velocity),
                aim: Restore(row: row.Aim));
    }

    private static AimData Capture(RunCombat.Aim row)
    {
        return new AimData
        {
            X = row.X,
            Y = row.Y,
        };
    }

    private static RunCombat.Aim Restore(AimData row)
    {
        return new RunCombat.Aim(x: row.X, y: row.Y);
    }

    private static ImpactData Capture(RunProjectiles.Impact row)
    {
        return new ImpactData
        {
            ArrowId = row.ArrowId,
            TargetId = row.TargetId,
            Kind = row.Kind,
            At = row.At,
            Damage = row.Damage,
            Hit = row.Hit is { } valueHit ? Capture(row: valueHit) : null,
        };
    }

    private static RunProjectiles.Impact Restore(ImpactData row)
    {
        return new RunProjectiles.Impact(arrowId: row.ArrowId, targetId: row.TargetId, kind: row.Kind,
                at: row.At, damage: row.Damage, hit: row.Hit is { } valueHit ? Restore(row: valueHit) : null);
    }

    private static LootTargetData Capture(RunDeathAndLoot.Target row)
    {
        return new LootTargetData
        {
            Anchor = Capture(row: row.Anchor),
            Generated = row.Generated,
            EmptySince = row.EmptySince,
        };
    }

    private static RunDeathAndLoot.Target Restore(LootTargetData row)
    {
        return new RunDeathAndLoot.Target(anchor: Restore(row: row.Anchor), generated: row.Generated)
        {
            EmptySince = row.EmptySince,
        };
    }

    private static DeathData Capture(RunDeathAndLoot.Death row)
    {
        return new DeathData
        {
            SurvivorId = row.SurvivorId,
            CorpseId = row.CorpseId,
            Contact = Capture(row: row.Contact),
            At = row.At,
            Lease = row.Lease is { } valueLease ? Capture(row: valueLease) : null,
        };
    }

    private static RunDeathAndLoot.Death Restore(DeathData row)
    {
        return new RunDeathAndLoot.Death(survivorId: row.SurvivorId, corpseId: row.CorpseId,
                contact: Restore(row: row.Contact), at: row.At,
                lease: row.Lease is { } valueLease ? Restore(row: valueLease) : null);
    }

    private static WaitData Capture(RunSuccession.Wait row)
    {
        return new WaitData
        {
            BindingId = row.BindingId,
            Epoch = row.Epoch,
            Slot = row.Slot,
            At = row.At,
        };
    }

    private static RunSuccession.Wait Restore(WaitData row)
    {
        return new RunSuccession.Wait(bindingId: row.BindingId, epoch: row.Epoch, slot: row.Slot, at: row.At);
    }

    private static SuccessionData Capture(RunSuccession.State row)
    {
        return new SuccessionData
        {
            Waiting = [.. row.Waiting.OrderBy(keySelector: item => (item.At, item.Slot))
                    .Select(selector: Capture)],
            Seed = row.Seed,
            DrawPosition = row.DrawPosition,
            RuntimeContract = ReplayRandom.PinnedRuntimeContract,
        };
    }

    private static RunSuccession.State Restore(SuccessionData row)
    {
        return new RunSuccession.State(waiting: [.. row.Waiting.OrderBy(keySelector: ticket => (ticket.At, ticket.Slot))
                .Select(selector: Restore)], seed: row.Seed,
                drawPosition: row.DrawPosition);
    }
}
