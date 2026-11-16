using System;
using System.Diagnostics;
using System.IO;

namespace KingdomZombies.Phase1.Tests;
/// <summary>Selects workspace SDK policy while retaining checkout-relative fixture arguments.</summary>
internal static class DotnetLaunch
{
    /// <summary>Roots existing subject paths before selecting the workspace as the .NET CLI directory.</summary>
    internal static ProcessStartInfo Configure(ProcessStartInfo start)
    {
        if (!StringComparer.OrdinalIgnoreCase.Equals(x: Path.GetFileNameWithoutExtension(path: start.FileName),
                y: "dotnet"))
        {
            return start;
        }

        string root = start.WorkingDirectory;
        string workspace = Path.Combine(path1: root, path2: "src/godot-game");
        if (!File.Exists(path: Path.Combine(path1: workspace, path2: "global.json")))
        {
            return start;
        }

        for (int index = 0; index < start.ArgumentList.Count; index++)
        {
            string argument = start.ArgumentList[index];
            if (!Path.IsPathRooted(path: argument) && Path.HasExtension(path: argument)
                    && (File.Exists(path: Path.Combine(path1: root,
                            path2: argument)) || Directory.Exists(path: Path.Combine(path1: root, path2: argument))))
            {
                start.ArgumentList[index] = Path.GetFullPath(path: Path.Combine(path1: root, path2: argument));
            }
        }

        start.WorkingDirectory = workspace;
        return start;
    }
}
