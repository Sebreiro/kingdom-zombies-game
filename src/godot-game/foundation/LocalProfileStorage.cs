using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using KingdomZombies.Multiplayer.Abstractions;

namespace KingdomZombies.Foundation;

/// <summary>Restricts all profile paths to owned fixed names and atomically replaces complete books.</summary>
internal sealed class LocalProfileStorage
{
    internal LocalProfileStorage(string directory, Action<string, string>? publish)
    {
        _directory = Path.GetFullPath(path: directory);
        _path = Path.Combine(path1: _directory, path2: "profiles.json");
        _publish = publish ?? Publish;
    }

    internal bool Exists => File.Exists(path: _path);

    internal FileStream Lock()
    {
        Directory.CreateDirectory(path: _directory);
        for (DirectoryInfo? item = new DirectoryInfo(path: _directory); item is not null; item = item.Parent)
        {
            if (item.Attributes.HasFlag(flag: FileAttributes.ReparsePoint))
            {
                throw new IOException(message: "PROFILE_ROOT_LINK");
            }
        }

        foreach (string path in new[] { _path, Path.Combine(path1: _directory, path2: "profiles.lock") })
        {
            if (File.Exists(path: path) && File.GetAttributes(path: path).HasFlag(flag: FileAttributes.ReparsePoint))
            {
                throw new IOException(message: "PROFILE_FILE_LINK");
            }
        }

        return new FileStream(path: Path.Combine(path1: _directory, path2: "profiles.lock"),
                mode: FileMode.OpenOrCreate,
                access: FileAccess.ReadWrite, share: FileShare.None);
    }

    internal (PlayerProfile[] Profiles, string Selected) Read()
    {
        if (!Exists)
        {
            return ([], "");
        }

        if (new FileInfo(fileName: _path).Length > 1048576)
        {
            throw new InvalidDataException(message: "PROFILE_SIZE");
        }

        using JsonDocument document = JsonDocument.Parse(utf8Json: File.ReadAllBytes(path: _path),
                options: new JsonDocumentOptions { MaxDepth = 8 });
        JsonElement root = document.RootElement;
        if (root.GetProperty(propertyName: "schema").GetInt32() != 1)
        {
            throw new InvalidDataException(message: "PROFILE_SCHEMA");
        }

        PlayerProfile[] profiles = [..root.GetProperty(propertyName: "profiles").EnumerateArray().Select(
                selector: p => new PlayerProfile(playerId: p.GetProperty(propertyName: "id").GetString() ?? "",
                        displayName: p.GetProperty(propertyName: "name").GetString() ?? "",
                        ordinal: p.GetProperty(propertyName: "ordinal").GetInt32()))];
        string selected = root.GetProperty(propertyName: "selected").GetString() ?? "";
        bool invalid = profiles.Length == 0 || profiles.Any(predicate: p => !Guid.TryParseExact(input: p.PlayerId,
                format: "N", result: out _) || string.IsNullOrWhiteSpace(value: p.DisplayName) || p.Ordinal < 1);
        if (invalid || profiles.Select(selector: p => p.PlayerId).Distinct(comparer: StringComparer.Ordinal).Count()
                != profiles.Length || profiles.Select(selector: p => p.Ordinal).Distinct().Count() != profiles.Length
                || !profiles.Any(predicate: p => StringComparer.Ordinal.Equals(x: p.PlayerId, y: selected)))
        {
            throw new InvalidDataException(message: "PROFILE_IDENTITY");
        }

        return (profiles, selected);
    }

    internal void Write(PlayerProfile[] profiles, string selected)
    {
        string temporary = Path.Combine(path1: _directory, path2: "profiles." + Guid.NewGuid().ToString(format: "N")
                + ".tmp");
        try
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(value: new
            {
                schema = 1, selected, profiles = profiles.Select(selector: p => new
                {
                    id = p.PlayerId, name = p.DisplayName, ordinal = p.Ordinal,
                }),
            });
            using (var stream = new FileStream(path: temporary, mode: FileMode.CreateNew, access: FileAccess.Write,
                    share: FileShare.None))
            {
                stream.Write(buffer: bytes);
                stream.Flush(flushToDisk: true);
            }

            _publish(arg1: temporary, arg2: _path);
        }
        finally
        {
            File.Delete(path: temporary);
        }
    }

    internal void PreserveCorrupt()
    {
        File.Copy(sourceFileName: _path, destFileName: Path.Combine(path1: _directory,
                path2: "profiles.corrupt." + Guid.NewGuid().ToString(format: "N") + ".json"));
    }

    private readonly string _directory;
    private readonly string _path;
    private readonly Action<string, string> _publish;

    private static void Publish(string temporary, string destination)
    {
        if (File.Exists(path: destination))
        {
            File.Replace(sourceFileName: temporary, destinationFileName: destination, destinationBackupFileName: null);
        }
        else
        {
            File.Move(sourceFileName: temporary, destFileName: destination);
        }
    }
}
