namespace KingdomZombies.Phase1.Tests.Models;
/// <summary>Supplies explicit constructor mapping between the declared structural proof models.</summary>
internal static class BoundaryMapper
{
    /// <summary>Copies state and translates the validated entity kind to the independent wire name.</summary>
    internal static BoundarySnapshotDto Snapshot(BoundaryEntity entity)
    {
        return new BoundarySnapshotDto(value: entity.Value, kind: "ready");
    }
}
