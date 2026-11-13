# Phase 1 Execution Environment

## Local Linux Runtime

The orchestrator prepared a workspace-contained Linux VM on 2026-11-13. Windows features and system installations were not changed. This is environment preparation, not a passed Linux acceptance report.

- Guest shell verified over the owned loopback SSH endpoint: Ubuntu 24.04.5 LTS, kernel `6.8.0-147-generic`, x86_64.
- QEMU reports `11.1.0 (v11.1.0-12130-ge470268ff4)`; TCG emulation, four virtual CPUs, 4096 MB RAM.
- Runtime root: `.phase1/tools/linux-vm/`, ignored by Git and excluded from proof input identity.
- VM process identities and start time: `processes.json` under that runtime root. Initial launcher PID 23576 and QEMU PID 21752; verify the executable and live record before operating on a PID.
- SSH: the existing Windows OpenSSH executable at `C:/Windows/System32/OpenSSH/ssh.exe`, user `phase1`, host `127.0.0.1`, port `23022`. Use the runtime's `id_ed25519` and `known_hosts` files. These are temporary local VM credentials, not production credentials.
- Startup helper: `start-vm.cjs`; start hidden from the runtime root. Do not start a second VM over the same active disk. Seed HTTP and QMP bind only to loopback.
- QEMU was extracted with installed 7-Zip, without running its installer. Guest disk is an owned 30 GB sparse overlay over the verified base image.
- SDK, Node, Godot, clean checkout and the full Linux suite still need preparation/execution by plans 01-16 and 01-17. A boot or `uname` result cannot satisfy AC-02.
- When the run finishes or stops, shut down only the owned VM and its helper; preserve the disk for a later verified restart. Do not stop unrelated processes.

## Source Identity

The [official QEMU download page](https://www.qemu.org/download/) directs Windows users to the maintainer's binaries. The [2026-08-11 installer](https://qemu.weilnetz.de/w64/2026/qemu-w64-setup-20260811.exe) matched its [official SHA-512 file](https://qemu.weilnetz.de/w64/2026/qemu-w64-setup-20260811.sha512):

`5bcf9eed634e8575a37b74f445af41a2fe4106da512d0c30c368301d4c105037fdfab40a5287367a28a957624cddebbc8c07e16c88ab6634f554cdf3d16bf543`

The [Ubuntu Minimal 24.04 release image](https://cloud-images.ubuntu.com/minimal/releases/noble/release/ubuntu-24.04-minimal-cloudimg-amd64.img), published in the observed 2026-11-05 release, matched its [official SHA-256 list](https://cloud-images.ubuntu.com/minimal/releases/noble/release/SHA256SUMS):

`406630c08b5e545fcc7209854b129d47e3af822db128c9f19336593b5a2d3fe0`

Local download records are under `downloads/verified.json`; record these exact identities when producing platform evidence. The moving release URL must never silently select replacement bytes.

The seed uses the documented [NoCloud discovery interface](https://docs.cloud-init.io/en/latest/reference/datasources/nocloud.html). The emulator uses documented [TCG system emulation](https://www.qemu.org/docs/master/system/introduction.html). These are development proof tools; this environment establishes no release performance, rendering, export, LAN or endurance result.
