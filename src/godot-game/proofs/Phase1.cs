using System;
using System.Text.Json;
using Godot;

namespace KingdomZombies.Proofs;

/// <summary>Provides the minimal actual-engine construction, ready and teardown bootstrap evidence.</summary>
public partial class Phase1 : Node
{

    /// <summary>Requires the invocation nonce and emits construction evidence for this engine instance.</summary>
    public Phase1()
    {
        _nonce = System.Environment.GetEnvironmentVariable(variable: "PHASE1_NONCE")
                ?? throw new InvalidOperationException(message: "ENGINE_NONCE_MISSING");
        ProofSequence.Gateway.RequireThread();
        WriteEvidence(assertion: "construction");
    }

    /// <summary>Emits ready and observed engine/runtime identities, then requests a clean tree shutdown.</summary>
    public override void _Ready()
    {
        ProofSequence.Gateway.RequireThread();
        WriteEvidence(assertion: "ready");
        GD.Print(what: ["PHASE1_IDENTITY " + JsonSerializer.Serialize(value: new
        {
            nonce = _nonce,
            runtime = System.Environment.Version.ToString(),
            os = OS.GetName(),
            display = DisplayServer.GetName(),
            renderer = RenderingServer.GetCurrentRenderingMethod(),
            engine = Engine.GetVersionInfo()["string"].AsString(),
        })]);
    }

    /// <summary>Advances the native proof through actual engine frames.</summary>
    public override void _Process(double delta)
    {
        ProofSequence.Tick(owner: this, nonce: _nonce);
    }

    /// <summary>Emits teardown evidence carrying the same invocation nonce as construction and ready.</summary>
    public override void _ExitTree()
    {
        WriteEvidence(assertion: "exit");
    }
    private readonly string _nonce;

    private void WriteEvidence(string assertion)
    {
        GD.Print(what: ["PHASE1_ASSERT " + JsonSerializer.Serialize(value: new
        {
            nonce = _nonce,
            assertion,
        })]);
    }
}
