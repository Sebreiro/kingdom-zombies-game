using System;
using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Independently enumerates the exact engine-free production sources compiled by the proof project.</summary>
internal static class FoundationLinkedSources
{
    internal static IReadOnlyList<string> Paths { get; } = Array.AsReadOnly<string>(array:
    [
        "src/godot-game/game/foundation/FoundationDisplay.cs",
        "src/godot-game/game/foundation/FoundationPresentation.cs",
        "src/godot-game/game/foundation/FoundationActorNode.cs",
        "src/godot-game/game/foundation/FoundationEquipmentProjection.cs",
        "src/godot-game/game/foundation/FoundationCapturePublication.cs",
        "src/godot-game/game/foundation/FoundationDrawRequest.cs",
        "src/godot-game/game/foundation/FoundationObservationScope.cs",
        "src/godot-game/game/foundation/LocalProfileService.cs",
        "src/godot-game/game/foundation/LocalProfileStorage.cs",
        "src/godot-game/game/foundation/LanDiscoveryService.cs",
        "src/godot-game/game/foundation/DiscoveryCodec.cs",
        "src/godot-game/game/foundation/FoundationBrowser.cs",
        "src/godot-game/game/foundation/FoundationServices.cs",
        "src/godot-game/multiplayer-abstractions/IIdentityService.cs",
        "src/godot-game/multiplayer-abstractions/IDiscoveryService.cs",
        "src/godot-game/multiplayer-abstractions/PlayerProfile.cs",
        "src/godot-game/multiplayer-abstractions/SessionAdvertisement.cs",
        "src/godot-game/game/foundation/FoundationInput.cs",
        "src/godot-game/game/foundation/FoundationInventoryInteraction.cs",
        "src/godot-game/game/foundation/FoundationHudState.cs",
        "src/godot-game/game/foundation/FoundationSpectator.cs",
        "src/godot-game/game/foundation/FoundationCoordinator.cs",
        "src/godot-game/game/foundation/FoundationCoordinator.RunWire.cs",
        "src/godot-game/game/foundation/FoundationViewInbox.cs",
        "src/godot-game/game/foundation/FoundationPublicWorld.cs",
        "src/godot-game/game/foundation/FoundationRunFrame.cs",
        "src/godot-game/game/foundation/FoundationRunView.cs",
        "src/godot-game/game/foundation/FoundationRunReadback.cs",
        "src/godot-game/game/foundation/FoundationCoordinator.Admission.cs",
        "src/godot-game/game/foundation/FoundationCoordinator.Lifetime.cs",
        "src/godot-game/game/foundation/FoundationCoordinator.GuestPublication.cs",
        "src/godot-game/game/foundation/FoundationProtocol.cs",
        "src/godot-game/game/foundation/FoundationWireFrame.cs",
        "src/godot-game/game/foundation/FoundationEnvelope.cs",
        "src/godot-game/game/foundation/FoundationPeerBinding.cs",
        "src/godot-game/game/foundation/FoundationAdmissionHistory.cs",
        "src/godot-game/game/foundation/FoundationEndpoint.cs",
        "src/godot-game/multiplayer-abstractions/IConnectionService.cs",
        "src/godot-game/multiplayer-abstractions/ConnectionPacket.cs",
    ]);
}
