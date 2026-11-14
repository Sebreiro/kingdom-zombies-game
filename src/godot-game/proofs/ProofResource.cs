using System;
using Godot;

namespace KingdomZombies.Proofs;
/// <summary>Represents the actual serialized resource loaded through the proof scene export.</summary>
internal sealed partial class ProofResource : Resource
{
    /// <summary>Records real parameterless resource construction by the native loader.</summary>
    public ProofResource()
    {
        ProofSequence.Gateway.RequireThread();
        ProofSequence.Current.Resources++;
    }

    /// <summary>Returns checked serialized resource data after engine injection.</summary>
    internal int Read()
    {
        ProofSequence.Gateway.RequireThread();
        string identity = Identity ?? throw new InvalidOperationException(message: "RESOURCE_ID_REQUIRED");
        if (!StringComparer.Ordinal.Equals(x: identity, y: "native-proof") || Amount != 13)
        {
            throw new InvalidOperationException(message: "RESOURCE_SERIALIZED_VALUE");
        }

        return Amount;
    }

    [Export]
    private string? Identity { get; set; }
    [Export]
    private int Amount { get; set; }
}
