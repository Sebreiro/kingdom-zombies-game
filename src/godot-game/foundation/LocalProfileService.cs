using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Publishes local identities only after exclusive atomic storage succeeds.</summary>
internal sealed class LocalProfileService : IIdentityService
{
    public LocalProfileService(string directory, Action<string, string>? publish = null)
    {
        _storage = new LocalProfileStorage(directory: directory, publish: publish);
    }

    public IReadOnlyList<PlayerProfile> Profiles => Array.AsReadOnly(array: _profiles);
    public PlayerProfile? Selected { get; private set; }
    public string FailureReason { get; private set; } = "";

    public bool Load()
    {
        return Execute(operation: "load", value: "");
    }

    public bool Create()
    {
        return Execute(operation: "create", value: "");
    }

    public bool Choose(string playerId)
    {
        return Execute(operation: "select", value: playerId);
    }

    public bool Rename(string displayName)
    {
        if (string.IsNullOrWhiteSpace(value: displayName))
        {
            FailureReason = "Enter a display name to continue.";
            return false;
        }

        return Execute(operation: "rename", value: displayName);
    }

    private readonly LocalProfileStorage _storage;
    private PlayerProfile[] _profiles = [];

    private bool Execute(string operation, string value)
    {
        try
        {
            using FileStream writer = _storage.Lock();
            (PlayerProfile[] profiles, string selected) = Read(operation: operation);
            if (operation is "create" || profiles.Length == 0)
            {
                int ordinal = profiles.Length == 0 ? 1 : checked(profiles.Max(selector: p => p.Ordinal) + 1);
                var created = new PlayerProfile(playerId: Guid.NewGuid().ToString(format: "N"),
                        displayName: "Player " + ordinal.ToString(provider: CultureInfo.InvariantCulture),
                        ordinal: ordinal);
                profiles = [..profiles, created];
                selected = created.PlayerId;
            }

            selected = operation is "select" ? value : selected;
            PlayerProfile current = profiles.Single(predicate: p => StringComparer.Ordinal.Equals(
                    x: p.PlayerId, y: selected));
            if (operation is "rename")
            {
                current = new PlayerProfile(playerId: current.PlayerId, displayName: value, ordinal: current.Ordinal);
                profiles = [..profiles.Select(selector: p => StringComparer.Ordinal.Equals(x: p.PlayerId, y: selected)
                        ? current : p)];
            }

            if (operation is not "load" || !_storage.Exists)
            {
                _storage.Write(profiles: profiles, selected: selected);
            }

            _profiles = profiles;
            Selected = current;
            FailureReason = "";
            return true;
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException or InvalidDataException
                or System.Text.Json.JsonException or InvalidOperationException or ArgumentException
                or KeyNotFoundException or FormatException)
        {
            FailureReason = operation is "load"
                    ? "Could not load this profile. Choose another profile or create a new profile."
                    : "Could not save this profile. Try again or choose another profile.";
            if (operation is "load")
            {
                Selected = null;
                _profiles = [];
            }

            return false;
        }
    }

    private (PlayerProfile[] Profiles, string Selected) Read(string operation)
    {
        try
        {
            return _storage.Read();
        }
        catch (Exception error) when (operation is "create" && error is (InvalidDataException
                or System.Text.Json.JsonException or KeyNotFoundException or InvalidOperationException
                or FormatException))
        {
            _storage.PreserveCorrupt();
            return ([], "");
        }
    }
}
