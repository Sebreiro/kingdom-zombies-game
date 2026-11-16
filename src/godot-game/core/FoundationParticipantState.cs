namespace KingdomZombies.Core;

/// <summary>Publishes one immutable participant, readiness and assigned actor.</summary>
public sealed record FoundationParticipantState
{
    /// <summary>Maps explicitly supplied authority values into a detached publication.</summary>
    public FoundationParticipantState(string bindingId, string label, int slot, bool ready,
            FoundationActorState actor)
    {
        BindingId = bindingId;
        Label = label;
        Slot = slot;
        Ready = ready;
        Actor = actor;
    }

    /// <summary>Gets the opaque authority binding.</summary>
    public string BindingId { get; }
    /// <summary>Gets presentation text, independent of identity.</summary>
    public string Label { get; }
    /// <summary>Gets the stable in-session presentation number.</summary>
    public int Slot { get; }
    /// <summary>Gets whether this participant installed its baseline.</summary>
    public bool Ready { get; }
    /// <summary>Gets its distinct owned actor.</summary>
    public FoundationActorState Actor { get; }
}
