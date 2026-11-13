# Development setup

The exact baseline is [toolchain.json](../../tools/phase1/toolchain.json). The existing [Godot project](../../src/godot-game/project.godot) hosts a minimal C# proof scene. These setup references introduce no gameplay. Phase 1 acceptance and coverage remain pending the later trusted proof runner and actual Linux execution.

| Prerequisite | Exact version | Source |
| --- | --- | --- |
| Node.js, x64 | 24.22.1 | [Official release](https://nodejs.org/download/release/v24.22.1/) and its `SHASUMS256.txt` |
| .NET SDK, x64 | 10.0.404 | [Microsoft .NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0); use the platform archive/installer and the published checksum |
| .NET application runtime, x64 | 10.0.15 | Same Microsoft download page; supplied by the pinned SDK |
| .NET editor runtime, x64 | 8.0.16 | [Microsoft .NET 8 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/8.0); required alongside SDK 10 for Godot editor tools targeting `net8.0` |
| Godot .NET | 4.7.2 stable | Platform archives below; retain the entire `GodotSharp` directory |
| C# / target | 14.0 / net10.0 | Explicit project declarations; compiler `5.9.0-1.26529.119 (2ab2854867733112c84ac12c4273d7970ac3afdf)` comes from SDK 10.0.404 |
| Local coverage tool | 18.12.0 | [Microsoft dotnet-coverage](https://www.nuget.org/packages/dotnet-coverage/18.12.0), pinned in the repository tool manifest; collection is proved in a later plan |

The installed SDK is a prerequisite. [global.json](../../global.json) disables SDK roll-forward and prerelease selection. The bootstrap checks actual Node, SDK, compiler, engine and managed runtime identities instead of treating declarations as proof. Roslyn NuGet development packages have separate version numbers; they are not the SDK compiler version.

| OS / architecture | Exact official archive | SHA-512 |
| --- | --- | --- |
| Windows x64 | [Godot_v4.7.2-stable_mono_win64.zip](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/Godot_v4.7.2-stable_mono_win64.zip) | `79229fd112b0c9cbeab82363a4ef7be18ea70f1caf86bf912789335b136fbe7e01db0053a33461438c5da1c680c17bbb10040bd09bedc51221cd4423d0367757` |
| Linux x64 | [Godot_v4.7.2-stable_mono_linux_x86_64.zip](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/Godot_v4.7.2-stable_mono_linux_x86_64.zip) | `1855960b27ee3ef5e66e5e228cced69d55637b24334a7411162687dcd077d8f9f645348cdb8eae984bec8135d49ed855a1e3a16476786b8bce60774fd8402d13` |

Both SHA-512 values come from the [official release checksum file](https://github.com/godotengine/godot-builds/releases/download/4.7.2-stable/SHA512-SUMS.txt). Windows also has pinned SHA-256 `a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`. Linux archive availability is preparation; instructions and cross-builds do not count as a Linux execution result.

Bootstrap acquisition happens as part of the later proof commands. It checks `.phase1/cache` first, may seed that cache from the matching archive in Downloads, then downloads the exact official release if absent. Downloads and the LangProj/recovered donor directories are never required. To require official acquisition, set `PHASE1_IGNORE_DOWNLOADS=1` and `PHASE1_FORCE_DOWNLOAD=1` for that invocation. An optional `PHASE1_ARCHIVE` names a local archive cache seed; the same pinned hashes apply. Node HTTPS uses a task-local direct connection when inherited proxy settings are broken. No machine-wide proxy or NuGet configuration changes are made.

All child processes receive workspace-local `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP` and `TMP` under ignored `.phase1/`. Exact Godot packages are copied from the verified archive to `.phase1/feed`; [NuGet.config](../../NuGet.config) maps their exact IDs to that feed and other pinned dependencies to NuGet.org. The archive packages are unsigned. Their provenance is the official archive checksum, package payload hashes and inspected publisher/repository metadata; no successful NuGet signature verdict is claimed. The first restore creates an isolated lock file, verifies the complete initial package closure, then repeats in locked mode before building. Each controlled child has a 60-second deadline and descendant cleanup.

For direct CLI setup commands outside the bootstrap, first establish the same isolation. PowerShell:

```powershell
$phase1Root = (Get-Location).Path
foreach ($phase1Dir in 'cli-home', 'packages', 'http-cache', 'temp') {
    New-Item -ItemType Directory -Force (Join-Path $phase1Root ".phase1/$phase1Dir") | Out-Null
}
$env:DOTNET_CLI_HOME = Join-Path $phase1Root '.phase1/cli-home'
$env:NUGET_PACKAGES = Join-Path $phase1Root '.phase1/packages'
$env:NUGET_HTTP_CACHE_PATH = Join-Path $phase1Root '.phase1/http-cache'
$env:TEMP = Join-Path $phase1Root '.phase1/temp'
$env:TMP = $env:TEMP
dotnet tool restore --configfile NuGet.config
```

Linux, from the checkout root (requires `unzip` and the x64 prerequisites):

```sh
mkdir -p .phase1/cli-home .phase1/packages .phase1/http-cache .phase1/temp
export DOTNET_CLI_HOME="$PWD/.phase1/cli-home"
export NUGET_PACKAGES="$PWD/.phase1/packages"
export NUGET_HTTP_CACHE_PATH="$PWD/.phase1/http-cache"
export TMPDIR="$PWD/.phase1/temp" TEMP="$PWD/.phase1/temp" TMP="$PWD/.phase1/temp"
dotnet tool restore --configfile NuGet.config
```

Tool restore uses only the declared official NuGet source. The bootstrap's initial engine slice needs only the four archive packages; additional phase tools are restored when their owning plans establish their proofs. Setup does not certify coverage, exports, rendered gameplay or a second operating system. Verification commands belong to the phase's later verification reference.
