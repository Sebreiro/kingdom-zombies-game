# Phase 1 Execution Environment

## Final repaired-source preparation and owned shutdown — 2026-11-14

The final candidate is `59f032cf23340058eec91f12c5d240f585dfabdc`, containing the four stabilized review repairs and current platform policy. The fresh Windows validation checkout is `.phase1/temp/17-windows-59f032c`; its 203-file frozen input is `fe2bf549f20329633fd62cfeab796ca8fb2c5ce02da432346e8ee4c314873426`. The matching Linux checkout is `/home/phase1/phase1-final-17`, prepared from `/home/phase1/phase1-tools/downloads/final-17.bundle`. SHA-256 of the exact bundle on both platforms is `334110bde21f2a983deb965d0d227e0872ed659309c8d170e3fd10e07f35f54b`. A bundle contains tracked source only; no donor, VM credentials, reports or old build outputs were transferred.

Independent guest inspection at `2026-11-14T18:42:47.105Z` verified the same source revision and 203-file input, clean Git status, all 41 raw NuGet payload hashes and their identities against the five current locks, both pinned official Godot archives, and all four retained official tool archives against their authenticated receipts. No `bin`, `obj` or `.godot` directories exist under the new checkout's source/tests/tools. Node `24.22.1`, SDK `10.0.404`, and runtimes `8.0.16` / `10.0.15` were observed through the existing guest tool environment. No Linux restore, build, bootstrap or acceptance case has run on this candidate. This preparation supplies no Linux acceptance credit.

Preparation receipts are retained under ignored `.phase1/temp/17-final-preparation/`: `frozen.json`, `frozen-inputs.json`, `linux-prepared.json`, `linux-prepared-inputs.json` and `linux-preparation.log`. The guest retains `.phase1/prepared-final-17.json` and `.phase1/prepared-final-17-inputs.json`. Official archives and the 41-package seed remain under `/home/phase1/phase1-tools/downloads/`; its pinned tools remain under `/home/phase1/phase1-tools/`. Old `/home/phase1/phase1-baseline-16` and historical results are preserved.

After matching the live QEMU executable, full owned disk/loopback command line, launcher parent and original start identities, the agent issued `sync; sudo -n poweroff` over the known-host-verified owned SSH connection at `2026-11-14T18:43:22Z`. Verification at `2026-11-14T18:43:33.5437334Z` found QEMU PID 21752 and launcher PID 23576 exited, no listeners on owned loopback SSH/QMP ports 23022/23023, and a successful exclusive read-open of `system.qcow2`. The ignored `processes.json` preserves executable/start details and last PIDs, records `shutdownConfirmed: true` / `stoppedAt`, and clears the live `qemu` / `launcher` values. `vm-before-shutdown.json`, `vm-shutdown.log`, its actual SSH exit receipt and `vm-stopped.json` retain the observations. No unrelated process was stopped. The disk, firmware, caches and keys are preserved.

The same host query still returned `HypervisorPresent: false`. The agent did not restart Windows or start another guest. After the creator's Windows restart, verify the active hypervisor and actual WHPX startup before running the complete Linux `--phase 1` selection from the frozen checkout. Source `/home/phase1/phase1-tools/env.sh`, establish the checkout-local isolation paths below, then run native bootstrap, clean locked five-project restore, strict whole-solution build and all 676 required cases with unchanged deadlines. Preserve the separate eight Phase 5 deferrals and fourteen pending human judgments. The [platform record](../../../docs/workflow/platform-proof.md) owns final executed outcomes.

## Hardware-acceleration handoff — 2026-11-14

The creator now requires complete phase scope on the development OS and a local VM running the other OS. The creator confirmed manually enabling Windows Hypervisor Platform. A subsequent live host query still returned `HypervisorPresent: false`, so accelerated Linux acceptance awaits the Windows restart. The earlier administrator-helper launch was canceled before execution; no successful automated feature installation is claimed.

The existing ignored launcher `.phase1/tools/linux-vm/start-vm.cjs` now defaults to WHPX and records its requested accelerator/CPU. It refuses to start while the recorded VM PID is live. `PHASE1_VM_ACCELERATOR=tcg` is an explicit diagnostic choice, not a silent fallback; default CPU is `max` with `PHASE1_VM_CPU` available for a concrete supported-runtime correction if actual startup requires it. This prepares configuration only. After restart, verify the live host capability, actual QEMU WHPX startup/logs and genuine guest access before running the complete 676-case Phase 1 selection. The [official backend guide](https://www.qemu.org/docs/master/system/whpx.html) documents the Windows feature and WHPX selection.

Preserve the existing VM disk, official tools and authenticated package caches. Before the host restarts, prepare the final same-source bundle/checkout and cleanly shut down only this owned guest. Its old TCG source/results remain historical; no old pass certifies the repaired implementation. The final checkpoint records the actual shutdown and new candidate rather than inferring them here.

## Local Linux Runtime

The orchestrator prepared a workspace-contained Linux VM on 2026-11-13. Windows features and system installations were not changed. This is environment preparation, not a passed Linux acceptance report.

- Guest shell verified over the owned loopback SSH endpoint: Ubuntu 24.04.5 LTS, kernel `6.8.0-147-generic`, x86_64.
- QEMU reports `11.1.0 (v11.1.0-12130-ge470268ff4)`; TCG emulation, four virtual CPUs, 4096 MB RAM.
- Runtime root: `.phase1/tools/linux-vm/`, ignored by Git and excluded from proof input identity.
- VM process identities and start time: `processes.json` under that runtime root. Initial launcher PID 23576 and QEMU PID 21752; verify the executable and live record before operating on a PID.
- SSH: the existing Windows OpenSSH executable at `C:/Windows/System32/OpenSSH/ssh.exe`, user `phase1`, host `127.0.0.1`, port `23022`. Use the runtime's `id_ed25519` and `known_hosts` files. These are temporary local VM credentials, not production credentials.
- Startup helper: `start-vm.cjs`; start hidden from the runtime root. Do not start a second VM over the same active disk. Seed HTTP and QMP bind only to loopback.
- QEMU was extracted with installed 7-Zip, without running its installer. Guest disk is an owned 30 GB sparse overlay over the verified base image.
- Plan 01-16 prepared Node `24.22.1`, SDK `10.0.404`, runtimes `8.0.16` and `10.0.15`, and Godot `4.7.2.stable.mono.official.ed1daf0bf`. The guest environment is `/home/phase1/phase1-tools/env.sh`; source it before commands. Tool archives were authenticated before extraction on Windows and again inside the guest. Preparation and exploratory setup/build feedback do not satisfy Linux acceptance.
- When the run finishes or stops, shut down only the owned VM and its helper; preserve the disk for a later verified restart. Do not stop unrelated processes.

## Source Identity

The [official QEMU download page](https://www.qemu.org/download/) directs Windows users to the maintainer's binaries. The [2026-08-11 installer](https://qemu.weilnetz.de/w64/2026/qemu-w64-setup-20260811.exe) matched its [official SHA-512 file](https://qemu.weilnetz.de/w64/2026/qemu-w64-setup-20260811.sha512):

`5bcf9eed634e8575a37b74f445af41a2fe4106da512d0c30c368301d4c105037fdfab40a5287367a28a957624cddebbc8c07e16c88ab6634f554cdf3d16bf543`

The [Ubuntu Minimal 24.04 release image](https://cloud-images.ubuntu.com/minimal/releases/noble/release/ubuntu-24.04-minimal-cloudimg-amd64.img), published in the observed 2026-11-05 release, matched its [official SHA-256 list](https://cloud-images.ubuntu.com/minimal/releases/noble/release/SHA256SUMS):

`406630c08b5e545fcc7209854b129d47e3af822db128c9f19336593b5a2d3fe0`

Local download records are under `downloads/verified.json`; record these exact identities when producing platform evidence. The moving release URL must never silently select replacement bytes.

The seed uses the documented [NoCloud discovery interface](https://docs.cloud-init.io/en/latest/reference/datasources/nocloud.html). The emulator uses documented [TCG system emulation](https://www.qemu.org/docs/master/system/introduction.html). These are development proof tools; this environment establishes no release performance, rendering, export, LAN or endurance result.

## Prepared Linux tools and acquisition

Guest tools/downloads are under `/home/phase1/phase1-tools/`. Windows download/provenance records are under ignored `.phase1/tools/linux-prep/`: `selected-downloads.json`, `verified-downloads.json`, Microsoft release metadata and Node's checksum list. These are local setup receipts, not acceptance reports.

| Archive | Exact hash |
| --- | --- |
| `dotnet-sdk-10.0.404-linux-x64.tar.gz` | SHA-512 `577d0eb17fec33d029104c5c63a1ab0accc3e28776274abef2218dbac5af18d2109b5406d26b31d76f5c245215e3ba0926a94c20246c672c41847c9e1eb1dc50` |
| `dotnet-runtime-8.0.16-linux-x64.tar.gz` | SHA-512 `e0924f88b6795d669e5c6278845a574b75fbd9b5aeb63a0ed638224fb8d3d48b91bb2a9afb272dfd5f40e8241d41a84792b9fcfe426e8fdcfa14116bc132cee2` |
| `node-v24.22.1-linux-x64.tar.xz` | SHA-256 `6c172a9ab6718f7b801fb5a7bd486af2ee5e32758086a04b84fcce2aa77095e6` |
| `Godot_v4.7.2-stable_mono_linux_x86_64.zip` | SHA-512 `1855960b27ee3ef5e66e5e228cced69d55637b24334a7411162687dcd077d8f9f645348cdb8eae984bec8135d49ed855a1e3a16476786b8bce60774fd8402d13` |

The SDK/runtime downloads and hashes come from Microsoft's official .NET 10/8 release metadata. Node comes from its official `v24.22.1` release and `SHASUMS256.txt`. Godot archives and common canonical managed payloads are pinned in `tools/phase1/toolchain.json`; Linux uses its native archive plus the Windows archive as the exact managed-package donor, without executing its Windows binary. The [setup reference](../../../docs/workflow/setup.md) explains the raw NuGet metadata difference and keeps the common locks/provenance unchanged.

The guest has `git`, `curl`, `unzip`, `tar`, `xz`, `ca-certificates`, `libicu74`, `libssl3t64`, `libgssapi-krb5-2`, fontconfig and fonts from signed Ubuntu apt sources. No global Windows setting or donor filesystem is required. Use the full Windows OpenSSH paths; a PATH `ssh.bat` is a deny shadow. Never copy private keys into source bundles or fixtures.

The [platform record](../../../docs/workflow/platform-proof.md) retains the earlier sixteen representative case outcomes as historical evidence. Current 01-17 acceptance requires the complete 676-case `node tools/phase1/check.cjs --phase 1` scope on Windows and the local Linux VM for the same final implementation; preparation supplies no pass.

Exploratory preflight uses `/home/phase1/phase1-preflight-16`, separate from the final acceptance checkout. Its first cold bootstrap restore hit the existing 60-second child deadline (nonce `e8b3cf84-b09b-487d-958f-78bc15092e5c`). The ordinary bounded locked whole-solution restore then succeeded for all five projects. The initial strict build rejected the SDK's actual Linux LF assembly attribute with SCR0007 at line 4, column 121. Its exact source hashes are `f79c1ad9a9fc76110b3655f5bfb5ff4e3168e9addceff05b937c0796f0ec34eb` (net10.0) and `7300a46b63ecd250f168f446b4864848453c4b655fb6f317d1530d7ca963ebc5` (netstandard2.0); the actual SDK `Microsoft.Common.CurrentVersion.targets` hashes to `879d9742497730b1c8bac7b90c3b14bcb3d4280a87c50b651a1c4430744207ba`. These observed identities are added narrowly to the existing layout provenance; common locks, native generated spans and exact member suppressions remain unchanged. Logs are `/home/phase1/preflight16-{bootstrap,restore,build,build2}.log` and the checkout's ordinary `.phase1/runs/`.

The subsequent ordinary single-node strict build (`-m:1`, otherwise the same strict flags) succeeded with zero warnings/errors in `00:03:03.63`; `/home/phase1/preflight16-build3.log` and `.exit` retain the actual result. Software-emulated cold setup is slow; the prior parallel build exhausted its bounded setup window without further source diagnostics. The combined existing-registry inventory checked 560 complete source/text/package/producer identities without mismatches in `/home/phase1/preflight16-provenance.json`. This also identified and authenticated the one testing-platform MSBuild-hook newline variant described in the platform record; all other recorded Godot/xUnit source identities remain unchanged. These are exploratory preflight results, not the final clean Linux baseline or representative acceptance.

After the exact member-source selection change, a second strict five-project build passed with zero warnings/errors in `00:02:55.88` (`preflight16-build4.log`). The existing actual permission-reader/duplicate-boundary control passed 1/1, zero failed/skipped, with explicit `PHASE1_ROOT` (`preflight16-permissions2.log` and `.phase1/temp/16-permissions2/permissions.json`). Its first raw invocation omitted the root that the canonical coordinator normally supplies and failed on an assembly-directory-relative path; that diagnostic run receives no acceptance credit. A bounded direct native Linux launch of the existing proof scene also exited zero and emitted its real lifecycle/resource/thread records (`preflight16-native.log`, nonce `16-exploratory-native`). These exploratory checks resolve setup/provenance risks before the final Windows run; 01-17 must still execute the declared clean Linux baseline and all sixteen canonical case selectors for the final frozen source.

## Historical pre-review same-source checkout procedure

The final acceptance checkout is `/home/phase1/phase1-baseline-16`; the retained Git bundle is `/home/phase1/phase1-tools/downloads/baseline-16.bundle`. Its exact source revision and matching clean Windows checkout are recorded in 01-16-SUMMARY.md after preparation. Clone the bundle, retain its pinned revision, and inspect `git status --short` before acceptance. No source repair is authored in these test checkouts. The bundle excludes VM credentials, donor files, old outputs and reports.

The declared cache seed is `/home/phase1/phase1-tools/downloads/packages.tar.gz`, with `cache.json` recording 41 exact locked package payloads. Before copying, Windows checked each raw archive against its `.nupkg.sha512` and its normalized NuGet metadata identity against the committed lock. Restore still uses the declared feeds and locked mode. The SDK package comes from bootstrap's authenticated canonical archive feed, not an unrelated warm SDK cache. The two Godot ZIPs and official checksum file are in the same downloads directory; bootstrap rechecks their exact hashes. Extract the package seed into the new checkout's `.phase1/`, and copy only those three archive/checksum files to `.phase1/cache/`. No old `bin`, `obj` or `.godot` output is copied.

From that clean checkout, use the existing commands:

```sh
. /home/phase1/phase1-tools/env.sh
cd /home/phase1/phase1-baseline-16
mkdir -p .phase1/cli-home .phase1/packages .phase1/http-cache .phase1/temp .phase1/cache
export DOTNET_CLI_HOME="$PWD/.phase1/cli-home" NUGET_PACKAGES="$PWD/.phase1/packages"
export NUGET_HTTP_CACHE_PATH="$PWD/.phase1/http-cache"
export TMPDIR="$PWD/.phase1/temp" TEMP="$PWD/.phase1/temp" TMP="$PWD/.phase1/temp"
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
PHASE1_IGNORE_DOWNLOADS=1 node tools/phase1/bootstrap.cjs --probe engine
dotnet restore KingdomZombies.slnx --locked-mode --disable-parallel -p:NuGetAudit=false
dotnet build KingdomZombies.slnx --no-restore --disable-build-servers -p:UseSharedCompilation=false -nodeReuse:false -p:NuGetAudit=false -v:q
node tools/phase1/check.cjs --case R-38.positive
```

The earlier procedure then ran each of the sixteen exact IDs in the platform record with the same `--case` command. Bootstrap's 60-second child deadline, the coordinator's deadlines and existing exact evidence checks remain in force. Cold TCG setup can exceed a deadline; retain that failure and record the concrete environment limitation instead of awarding a pass. Current final acceptance instead requires a new same-source checkout, clean setup and the complete 676-case `--phase 1` selection after actual hardware acceleration is verified. Complete Linux execution and the separate human review remain 01-17 work.
