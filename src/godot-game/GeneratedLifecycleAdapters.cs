using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1852", Scope = "type",
        Target = "~T:KingdomZombies.Proofs.LifecycleFixture",
        Justification = "Pinned signal generator introduces a protected helper, so this exact script cannot be sealed")]
