using System;

namespace KingdomZombies.Inspection;

/// <summary>Projects inspection-only camp state without executing gameplay purchases.</summary>
internal sealed class CampLayouts
{
    /// <summary>Quotes the stash balance after selling selected stored food.</summary>
    internal static int SaleMoney(int stashStock, int selected, int unitPrice, int stashMoney, int carriedFood)
    {
        return stashMoney;
    }

    /// <summary>Returns capacity from the last completed main-building tier.</summary>
    internal static int Capacity(int completedTier, int pendingTier)
    {
        return 0;
    }

    /// <summary>Returns the exact reason an inspection assault action is unavailable.</summary>
    internal static string RallyReason(bool hasTarget, bool active, int ready, int personalCash)
    {
        return string.Empty;
    }
}
