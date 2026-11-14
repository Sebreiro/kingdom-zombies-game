# Development setup

**Current Phase 1 closure boundary (creator instruction, 2026-11-14):** The [one-time exception](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-EXCEPTION.md) accepts the retained complete Windows baseline plus actual focused Linux strict/native/rule/state/graph evidence without a host restart. The narrow GODOT_LINUXBSD test correction passed separately on both OSes. A complete Linux suite, its original graph capacity failure and fourteen individually unreviewed human components remain explicit [follow-ups](../../.planning/phases/01-code-rules-and-documentation/01-CLOSURE-FOLLOWUPS.md), not passes. The complete-other-OS-VM rule continues for later phases.

The exact baseline is [toolchain.json](../../tools/phase1/toolchain.json). The existing [Godot project](../../src/godot-game/project.godot) hosts a minimal C# proof scene. These setup references introduce no gameplay. Preserve the implemented runner and rules; Phase 1 requires its complete current scope on Windows and the local Linux VM for the same final implementation. Measured coverage and the eight explicit later-phase declarations remain Phase 5 obligations. [Verification](verification.md) owns commands and result interpretation under the creator's latest 2026-11-14 platform rule.

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

All child processes receive workspace-local `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `NUGET_HTTP_CACHE_PATH`, `TEMP` and `TMP` under ignored `.phase1/`. The native engine comes from the actual OS archive. The common managed feed uses the exact four packages from the verified Windows archive on both operating systems, explicitly selected by `managedPackageArchive` in the toolchain manifest. Linux therefore caches both official archives. Their inner SDK, generator and managed DLL contents match, but package relationship/core metadata and raw nupkg hashes differ. Linux archive packages must not replace the canonical bytes pinned by the five common locks and generated-source provenance. Bootstrap verifies both archive identities and the canonical package hashes before copying to `.phase1/feed`; it never executes the Windows binary on Linux. [NuGet.config](../../NuGet.config) maps those exact IDs to that feed and other pinned dependencies to NuGet.org. The archive packages are unsigned. Their provenance is the official archive checksum, package payload hashes and inspected publisher/repository metadata; no successful NuGet signature verdict is claimed. Each controlled child has a 60-second deadline and descendant cleanup. Windows extraction uses the installed .NET `ZipFile` implementation through PowerShell, avoiding a dependency on Archive-module autoload.

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

Tool restore uses only the declared official NuGet source. The bootstrap's initial engine slice needs only the four archive packages; additional phase tools are restored when their owning plans establish their proofs. After the native bootstrap command in the [verification reference](verification.md) has populated the verified engine feed, restore exactly the five committed project lock files from the checkout root on either platform:

```sh
dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
```

Use the same source checkout and pinned prerequisites on each operating system. A clean checkout must not need LangProj, recovered donor folders or a Downloads cache. Keep each checkout's CLI home, package/feed/cache/temp paths local and retain the entire platform Godot archive directory. Acquisition failure, missing prerequisites or failed locked restore blocks the dependent build/proofs. Do not substitute an unverified package name or silently update a lock to bypass failure. Bootstrap native subprocesses have bounded deadlines; the full case coordinator records each case's own deadline and cleanup. The prepared workspace Linux VM belongs to later platform execution and must not be terminated by unrelated proof cleanup.

Setup alone does not certify execution, coverage, exports or rendered gameplay. Under the creator's latest [platform rule](ai-collaboration.md#development-and-phase-end-testing), use the development OS for implementation checks, then execute the complete current phase scope on that OS and in a local VM running the other supported OS at phase end. Plan 01-17 owns fresh complete Phase 1 Windows/Linux-VM evidence after the review repairs; earlier 01-16/01-17 reports remain historical. Measured 90%/80% coverage, eight explicitly pending declarations and advanced certification belong to [Phase 5](../../.planning/phases/05-ci-and-repeatable-game-testing/05-PHASE1-HANDOFF.md). Actual commands and evidence rules are in [verification](verification.md).

## Local VM hardware acceleration

For the existing Windows-hosted Linux VM, QEMU's [WHPX backend](https://www.qemu.org/docs/master/system/whpx.html) uses Windows Hypervisor Platform. Enable **Windows Hypervisor Platform** in **Turn Windows features on or off**, then complete any requested Windows restart. The creator confirmed enabling this feature on 2026-11-14; live inspection still reported no active hypervisor before restart. No accelerated execution is inferred from the checkbox alone.

The owned runtime is `.phase1/tools/linux-vm/`. Its launcher `start-vm.cjs` now defaults to `whpx` and records the requested accelerator/CPU with the owned process identity; software `tcg` requires explicit selection. After restart, first confirm that the host hypervisor is active, start only the one owned VM, verify actual WHPX startup and genuine Linux access, then run the complete Phase 1 selection with unchanged deadlines. Preserve the existing disk, verified tools and caches; do not start a second process on the same live disk. Shut down the owned guest cleanly before a host restart. Hardware preparation supplies no test acceptance by itself.
