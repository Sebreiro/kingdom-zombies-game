namespace KingdomZombies.Core;

/// <summary>Defines the detached, versioned run data independently from roster and recipient views.</summary>
internal sealed class RunSnapshotDocument
{
    public required int Version { get; init; }
    public required string SessionId { get; init; }
    public required long Revision { get; init; }
    public required long Elapsed { get; init; }
    public required ParticipantData[] Participants { get; init; }
    public required WorldData World { get; init; }
    public required PopulationData Population { get; init; }
    public required InventoryData Inventory { get; init; }
    public required CombatData Combat { get; init; }
    public required LootData Loot { get; init; }
    public required SuccessionData Succession { get; init; }
    public required IntentData[] Intents { get; init; }
    public required FoodReceiptData[] FoodReceipts { get; init; }
    public required AttackReceiptData[] AttackReceipts { get; init; }
    public required RespawnReceiptData[] RespawnReceipts { get; init; }
    public required ItemReceiptData[] ItemReceipts { get; init; }

    internal enum LocationKind { Equipment, CorpseEquipment, Ground, Stash, Container }

    internal sealed class ParticipantData
    {
        public required string BindingId { get; init; }
        public required string Label { get; init; }
        public required int Slot { get; init; }
        public required bool Ready { get; init; }
        public required string PlayerId { get; init; }
        public required string ActorId { get; init; }
        public required long Position { get; init; }
        public required int Facing { get; init; }
        public required long Elapsed { get; init; }
    }

    internal sealed class PointData
    {
        public required long X { get; init; }
        public required long Y { get; init; }
    }

    internal sealed class VectorData
    {
        public required decimal X { get; init; }
        public required decimal Y { get; init; }
    }

    internal sealed class SizeData
    {
        public required int Width { get; init; }
        public required int Height { get; init; }
    }

    internal sealed class RangesData
    {
        public required int RefugeUnits { get; init; }
        public required int DangerUnits { get; init; }
        public required int LocalUnits { get; init; }
        public required int StashUnits { get; init; }
    }

    internal sealed class WorldData
    {
        public required PointData Origin { get; init; }
        public required long Ground { get; init; }
        public required PointData MainBuilding { get; init; }
        public required RangesData Ranges { get; init; }
        public required DefenseData[] Defenses { get; init; }
        public required BlockerData[] Blockers { get; init; }
        public required HostileData[] Hostiles { get; init; }
        public required AnchorData[] Anchors { get; init; }
    }

    internal sealed class DefenseData
    {
        public required string Id { get; init; }
        public required PointData Contact { get; init; }
        public required bool Completed { get; init; }
        public required bool Living { get; init; }
        public required bool IsWall { get; init; }
        public required decimal Health { get; init; }
    }

    internal sealed class BlockerData
    {
        public required string Id { get; init; }
        public required PointData Minimum { get; init; }
        public required PointData Maximum { get; init; }
    }

    internal sealed class HostileData
    {
        public required string Id { get; init; }
        public required PointData Contact { get; init; }
    }

    internal sealed class AnchorData
    {
        public required string Id { get; init; }
        public required PointData Contact { get; init; }
        public required RunWorld.Access Kind { get; init; }
    }

    internal sealed class PopulationData
    {
        public required SurvivorData[] Survivors { get; init; }
        public required LeaseData[] Leases { get; init; }
        public required string[] IssuedBindings { get; init; }
        public required long LastControlGeneration { get; init; }
    }

    internal sealed class SurvivorData
    {
        public required string Id { get; init; }
        public required decimal Position { get; init; }
        public required decimal Height { get; init; }
        public required int Facing { get; init; }
        public required decimal Health { get; init; }
        public required decimal Stamina { get; init; }
        public required string Role { get; init; }
        public required decimal? LastStaminaSpend { get; init; }
        public required long? JumpStartedAt { get; init; }
        public required FenceData? SprintFence { get; init; }
        public required RunLocomotion.Block SprintBlock { get; init; }
        public required decimal? RestStartedAt { get; init; }
        public required decimal? FoodAvailableAt { get; init; }
        public required decimal? AttackAvailableAt { get; init; }
        public string MovementPose { get; init; } = "idle";
        public decimal? MovementAt { get; init; }
        public decimal? StrikeAt { get; init; }
        public AimData? StrikeAim { get; init; }
        public decimal? FlinchAt { get; init; }
        public required bool Hostile { get; init; }
    }

    internal sealed class FenceData
    {
        public required long Generation { get; init; }
        public required long Sequence { get; init; }
    }

    internal sealed class LeaseData
    {
        public required string SessionId { get; init; }
        public required string PlayerId { get; init; }
        public required string BindingId { get; init; }
        public required int Slot { get; init; }
        public required string SurvivorId { get; init; }
        public required long Generation { get; init; }
        public required bool Reassigned { get; init; }
    }

    internal sealed class InventoryData
    {
        public required SizeData StashGrid { get; init; }
        public required bool LegacyStash { get; init; }
        public required int StashPageCount { get; init; }
        public required long NextItemId { get; init; }
        public required int MaximumContainerLevels { get; init; }
        public required ItemData[] Items { get; init; }
        public required QuickReferenceData[] QuickReferences { get; init; }
    }

    internal sealed class ItemData
    {
        public required long Id { get; init; }
        public required string DefinitionId { get; init; }
        public required int Quantity { get; init; }
        public required decimal? Condition { get; init; }
        public required SizeData? Storage { get; init; }
        public required LocationData Location { get; init; }
        public required ItemLayoutData Layout { get; init; }
    }

    internal sealed class ItemLayoutData
    {
        public required SizeData Footprint { get; init; }
        public required bool Rotatable { get; init; }
        public required int MaximumStack { get; init; }
        public required SizeData? Storage { get; init; }
        public required decimal? MaximumCondition { get; init; }
        public required string? Slot { get; init; }
        public required string Pack { get; init; }
        public required string Icon { get; init; }
        public required string? FitFile { get; init; }
        public required string? FitItem { get; init; }
        public required bool Legacy { get; init; }
    }

    internal sealed class LocationData
    {
        public required LocationKind Kind { get; init; }
        public required string? TargetId { get; init; }
        public required string? Slot { get; init; }
        public required long? GridId { get; init; }
        public required int? X { get; init; }
        public required int? Y { get; init; }
        public required bool? Rotated { get; init; }
        public required SizeData? Footprint { get; init; }
    }

    internal sealed class QuickReferenceData
    {
        public required string SurvivorId { get; init; }
        public required int Slot { get; init; }
        public required long ItemId { get; init; }
    }

    internal sealed class CombatData
    {
        public required HitData[] FatalHits { get; init; }
        public required ArrowData[] Arrows { get; init; }
        public required ImpactData[] Impacts { get; init; }
        public required long LastArrowId { get; init; }
    }

    internal sealed class HitData
    {
        public required string SourceId { get; init; }
        public required string TargetId { get; init; }
        public required decimal Raw { get; init; }
        public required decimal Damage { get; init; }
        public required decimal HealthBefore { get; init; }
        public required decimal HealthAfter { get; init; }
        public required decimal At { get; init; }
    }

    internal sealed class ArrowData
    {
        public required long Id { get; init; }
        public required string SourceId { get; init; }
        public required bool Hostile { get; init; }
        public required decimal Damage { get; init; }
        public required decimal LaunchedAt { get; init; }
        public required decimal ExpiresAt { get; init; }
        public required VectorData Position { get; init; }
        public required VectorData Velocity { get; init; }
        public required AimData Aim { get; init; }
    }

    internal sealed class AimData
    {
        public required double X { get; init; }
        public required double Y { get; init; }
    }

    internal sealed class ImpactData
    {
        public required long ArrowId { get; init; }
        public required string TargetId { get; init; }
        public required RunProjectiles.Kind Kind { get; init; }
        public required decimal At { get; init; }
        public required decimal Damage { get; init; }
        public required HitData? Hit { get; init; }
    }

    internal sealed class LootData
    {
        public required LootTargetData[] Targets { get; init; }
        public required DeathData[] Deaths { get; init; }
        public required long LastTargetId { get; init; }
    }

    internal sealed class LootTargetData
    {
        public required AnchorData Anchor { get; init; }
        public required bool Generated { get; init; }
        public required decimal? EmptySince { get; init; }
    }

    internal sealed class DeathData
    {
        public required string SurvivorId { get; init; }
        public required string CorpseId { get; init; }
        public required PointData Contact { get; init; }
        public required decimal At { get; init; }
        public required LeaseData? Lease { get; init; }
    }

    internal sealed class SuccessionData
    {
        public required WaitData[] Waiting { get; init; }
        public required int Seed { get; init; }
        public required long DrawPosition { get; init; }
        public required string RuntimeContract { get; init; }
    }

    internal sealed class WaitData
    {
        public required string BindingId { get; init; }
        public required long Epoch { get; init; }
        public required int Slot { get; init; }
        public required long At { get; init; }
    }

    internal sealed class IntentData
    {
        public required string BindingId { get; init; }
        public required long Generation { get; init; }
        public required long Sequence { get; init; }
        public required int Direction { get; init; }
        public required bool Sprint { get; init; }
    }

    internal sealed class FoodReceiptData
    {
        public required string BindingId { get; init; }
        public required long Generation { get; init; }
        public required long Sequence { get; init; }
        public required RunFood.Status Status { get; init; }
    }

    internal sealed class AttackReceiptData
    {
        public required string Id { get; init; }
        public required long Generation { get; init; }
        public required long Sequence { get; init; }
        public required RunCombat.Status Status { get; init; }
    }

    internal sealed class RespawnReceiptData
    {
        public required string BindingId { get; init; }
        public required long Epoch { get; init; }
        public required long Sequence { get; init; }
        public required RunSuccession.Status Status { get; init; }
    }

    internal sealed class ItemReceiptData
    {
        public required string BindingId { get; init; }
        public required long Generation { get; init; }
        public required long Sequence { get; init; }
        public required RunItemActions.Status Status { get; init; }
        public required SelectionData? Selection { get; init; }
        public required bool SelectionLost { get; init; }
        public required PageEffectData? StashEffect { get; init; }
    }

    internal sealed class SelectionData
    {
        public required RunWorld.Access Kind { get; init; }
        public required string Id { get; init; }
        public required long Token { get; init; }
        public required int PageIndex { get; init; }
    }

    internal sealed class PageEffectData
    {
        public required int PageNumber { get; init; }
        public required CellData[] ChangedCells { get; init; }
        public required int MorePages { get; init; }
    }

    internal sealed class CellData
    {
        public required int X { get; init; }
        public required int Y { get; init; }
    }
}
