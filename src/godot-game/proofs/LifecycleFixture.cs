using Godot;

namespace KingdomZombies.Proofs;
/// <summary>Receives engine-owned callbacks and generated signal binding for each actual scene instance.</summary>
internal partial class LifecycleFixture : Node
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
    }

    /// <summary>Records readiness and explicitly subscribes the intended live parent handler.</summary>
    public override void _Ready()
    {
        ProofSequence.Gateway.RequireThread();
        _lifetime.Events.Add(item: Name + ":ready");
        if (Name == "Parent")
        {
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

    private readonly LifetimeToken _lifetime;

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
