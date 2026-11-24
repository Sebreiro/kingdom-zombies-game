using System;
using System.Collections.Immutable;
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
        RunSnapshotDocument document = JsonSerializer.Deserialize<RunSnapshotDocument>(utf8Json: data,
                options: _options) ?? throw new InvalidDataException(message: "RUN_SNAPSHOT_DOCUMENT");
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
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

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

        var saved = new RunSavedLayout(maximumContainerLevels: row.MaximumContainerLevels,
                items: layouts.ToImmutable());
        return new RunInventory(items: items.MoveToImmutable(), stashGrid: Restore(row: row.StashGrid),
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
            Defenses = [.. row.Defenses.Select(selector: Capture)],
            Blockers = [.. row.Blockers.Select(selector: Capture)],
            Hostiles = [.. row.Hostiles.Select(selector: Capture)],
            Anchors = [.. row.Anchors.Select(selector: Capture)],
        };
    }

    private static RunWorld Restore(WorldData row)
    {
        return new RunWorld(origin: Restore(row: row.Origin), groundContactSubunits: row.Ground,
                mainBuilding: Restore(row: row.MainBuilding), ranges: Restore(row: row.Ranges),
                defenses: [.. row.Defenses.Select(selector: Restore)],
                blockers: [.. row.Blockers.Select(selector: Restore)],
                hostiles: [.. row.Hostiles.Select(selector: Restore)],
                anchors: [.. row.Anchors.Select(selector: Restore)]);
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
            Survivors = [.. row.Survivors.Select(selector: Capture)],
            Leases = [.. row.Leases.Select(selector: Capture)],
            IssuedBindings = [.. row.IssuedBindings.Order(comparer: StringComparer.Ordinal)],
            LastControlGeneration = row.LastControlGeneration,
        };
    }

    private static RunPopulation Restore(PopulationData row)
    {
        return new RunPopulation(survivors: [.. row.Survivors.Select(selector: Restore)],
                leases: [.. row.Leases.Select(selector: Restore)],
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
            FatalHits = [.. row.FatalHits.Select(selector: Capture)],
            Arrows = [.. row.Arrows.Select(selector: Capture)],
            Impacts = [.. row.Impacts.Select(selector: Capture)],
            LastArrowId = row.LastArrowId,
        };
    }

    private static RunCombat.State Restore(CombatData row)
    {
        return new RunCombat.State(fatalHits: [.. row.FatalHits.Select(selector: Restore)],
                arrows: [.. row.Arrows.Select(selector: Restore)],
                impacts: [.. row.Impacts.Select(selector: Restore)], lastArrowId: row.LastArrowId);
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
            Waiting = [.. row.Waiting.Select(selector: Capture)],
            Seed = row.Seed,
            DrawPosition = row.DrawPosition,
        };
    }

    private static RunSuccession.State Restore(SuccessionData row)
    {
        return new RunSuccession.State(waiting: [.. row.Waiting.Select(selector: Restore)], seed: row.Seed,
                drawPosition: row.DrawPosition);
    }
}
