using System;
using Godot;

namespace KingdomZombies.Proofs;
/// <summary>Receives engine-owned callbacks and generated signal binding for each actual scene instance.</summary>
internal sealed partial class LifecycleFixture : Node
{
    /// <summary>Records genuine parameterless construction by the engine scene loader.</summary>
    public LifecycleFixture()
    {
        _lifetime = ProofSequence.Current;
        _lifetime.Constructions++;
    }

    /// <summary>Records the actual engine tree-entry callback.</summary>
    public override void _EnterTree()
    {
        ProofSequence.Gateway.RequireThread();
        _lifetime.Events.Add(item: Name + ":enter");
        if (Name == "Parent")
        {
            _lifetime.Instance = GetInstanceId();
        }
    }

    /// <summary>Records readiness and explicitly subscribes the intended live parent handler.</summary>
    public override void _Ready()
    {
        ProofSequence.Gateway.RequireThread();
        _lifetime.Events.Add(item: Name + ":ready");
        if (Name == "Parent")
        {
            if (!ValidateExports())
            {
                return;
            }

            Pulse += Receive;
        }
    }

    /// <summary>Observes real frames without forwarding frame-time into authoritative Core state.</summary>
    public override void _Process(double delta)
    {
        ProofSequence.Gateway.RequireThread();
        if (Name == "Parent")
        {
            _lifetime.Processes++;
        }
    }

    /// <summary>Closes generation acceptance before cancellation and explicit signal disconnection.</summary>
    public override void _ExitTree()
    {
        ProofSequence.Gateway.RequireThread();
        _lifetime.Close();
        if (Name == "Parent")
        {
            Pulse -= Receive;
        }

        _lifetime.Events.Add(item: Name + ":exit");
    }

    /// <summary>Emits through the real Godot signal binding rather than invoking a delegate directly.</summary>
    internal void Use()
    {
        ProofSequence.Gateway.RequireThread();
        EmitSignal(signal: SignalName.Pulse, args: [7]);
    }

    /// <summary>Exercises distinct controlled missing-value and explicit-null pre-readiness inputs.</summary>
    internal void ApplyMode(string mode)
    {
        ProofSequence.Gateway.RequireThread();
        if (StringComparer.Ordinal.Equals(x: mode, y: "missing-value"))
        {
            RequiredValue = default;
        }
        else if (StringComparer.Ordinal.Equals(x: mode, y: "invalid-null"))
        {
            RequiredResource = null;
        }
    }

    private readonly LifetimeToken _lifetime;
    [Export]
    private ProofResource? RequiredResource { get; set; }
    [Export]
    private Variant RequiredValue { get; set; }
    [Export]
    private ProofResource? OptionalResource { get; set; }

    private bool ValidateExports()
    {
        try
        {
            (ProofResource resource, int value) = CheckedExports();
            if (resource.Read() != 13 || value != 37 || OptionalResource is not null)
            {
                throw new InvalidOperationException(message: "EXPORT_SERIALIZED_VALUE");
            }

            _lifetime.Uses++;
            return true;
        }
        catch (InvalidOperationException error)
        {
            ProofSequence.Reject(fixture: this, diagnostic: error.Message, uses: _lifetime.Uses);
            return false;
        }
    }

    private (ProofResource Resource, int Value) CheckedExports()
    {
        ProofSequence.Gateway.RequireThread();
        ProofResource resource = RequiredResource
                ?? throw new InvalidOperationException(message: "EXPORT_REQUIRED:RequiredResource");
        int? staged = RequiredValue.VariantType == Variant.Type.Int ? RequiredValue.AsInt32() : null;
        int value = staged ?? throw new InvalidOperationException(message: "EXPORT_REQUIRED:RequiredValue");
        return (resource, value);
    }

    private void Receive(int value)
    {
        ProofSequence.Gateway.RequireThread();
        _lifetime.Handlers++;
        _lifetime.Value += value;
    }

    /// <summary>Declares the real generated signal used by the fixture.</summary>
    [Signal]
    public delegate void PulseEventHandler(int value);
}
