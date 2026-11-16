using System;
using System.Collections.Generic;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Independently enumerates the exact engine-free production sources compiled by the proof project.</summary>
internal static class FoundationLinkedSources
{
    internal static IReadOnlyList<string> Paths { get; } = Array.AsReadOnly<string>(array:
    [
        "src/godot-game/foundation/FoundationDisplay.cs",
        "src/godot-game/foundation/FoundationPresentation.cs",
        "src/godot-game/foundation/FoundationActorNode.cs",
        "src/godot-game/foundation/FoundationCapturePublication.cs",
        "src/godot-game/foundation/FoundationDrawRequest.cs",
        "src/godot-game/foundation/FoundationObservationScope.cs",
        "src/godot-game/foundation/LocalProfileService.cs",
        "src/godot-game/foundation/LocalProfileStorage.cs",
        "src/godot-game/foundation/LanDiscoveryService.cs",
        "src/godot-game/foundation/DiscoveryCodec.cs",
        "src/godot-game/foundation/FoundationBrowser.cs",
        "src/godot-game/foundation/FoundationServices.cs",
        "src/multiplayer-abstractions/IIdentityService.cs",
        "src/multiplayer-abstractions/IDiscoveryService.cs",
        "src/multiplayer-abstractions/PlayerProfile.cs",
        "src/multiplayer-abstractions/SessionAdvertisement.cs",
        "src/godot-game/foundation/FoundationInput.cs",
        "src/godot-game/foundation/FoundationCoordinator.cs",
        "src/godot-game/foundation/FoundationCoordinator.Admission.cs",
        "src/godot-game/foundation/FoundationCoordinator.Lifetime.cs",
        "src/godot-game/foundation/FoundationCoordinator.GuestPublication.cs",
        "src/godot-game/foundation/FoundationProtocol.cs",
        "src/godot-game/foundation/FoundationEnvelope.cs",
        "src/godot-game/foundation/FoundationPeerBinding.cs",
        "src/godot-game/foundation/FoundationAdmissionHistory.cs",
        "src/godot-game/foundation/FoundationEndpoint.cs",
        "src/multiplayer-abstractions/IConnectionService.cs",
        "src/multiplayer-abstractions/ConnectionPacket.cs",
    ]);
}
