using System;
using System.Text.Json;
using Godot;

namespace KingdomZombies.Proofs;

public partial class Phase1 : Node
{
    private readonly string _nonce;

    public Phase1()
    {
        _nonce = System.Environment.GetEnvironmentVariable(variable: "PHASE1_NONCE")
            ?? throw new InvalidOperationException(message: "ENGINE_NONCE_MISSING");
        WriteEvidence(assertion: "construction");
    }

    public override void _Ready()
    {
        WriteEvidence(assertion: "ready");
        GD.Print("PHASE1_IDENTITY " + JsonSerializer.Serialize(value: new
        {
            nonce = _nonce,
            runtime = System.Environment.Version.ToString(),
            os = OS.GetName(),
            display = DisplayServer.GetName(),
            renderer = RenderingServer.GetCurrentRenderingMethod(),
            engine = Engine.GetVersionInfo()["string"].AsString(),
        }));
        GetTree().Quit();
    }

    public override void _ExitTree()
    {
        WriteEvidence(assertion: "exit");
    }

    private void WriteEvidence(string assertion)
    {
        GD.Print("PHASE1_ASSERT " + JsonSerializer.Serialize(value: new
        {
            nonce = _nonce,
            assertion,
        }));
    }
}
