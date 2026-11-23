'use strict';
const fs = require('node:fs');
const path = require('node:path');
const http = require('node:http');
const { spawn } = require('node:child_process');
const root = require('../../workspace-paths.cjs').locations().tools('linux-vm');
const accelerator = process.env.PHASE1_VM_ACCELERATOR || 'whpx';
if (!['whpx', 'tcg'].includes(accelerator)) throw new Error('Unsupported owned VM accelerator');
const cpu = process.env.PHASE1_VM_CPU || 'max';
const processRecord = path.join(root, 'processes.json');
if (fs.existsSync(processRecord)) {
  const previous = JSON.parse(fs.readFileSync(processRecord, 'utf8'));
  if (Number.isInteger(previous.qemu)) {
    try { process.kill(previous.qemu, 0); throw new Error('Recorded VM PID is live; verify it before starting another VM'); }
    catch (error) { if (error.code !== 'ESRCH') throw error; }
  }
}
const publicKey = fs.readFileSync(path.join(root, 'id_ed25519.pub'), 'utf8').trim();
const userData = `#cloud-config
hostname: kingdomzombies-proof
manage_etc_hosts: true
ssh_pwauth: false
disable_root: true
users:
  - default
  - name: phase1
    shell: /bin/bash
    groups: [adm, sudo]
    sudo: ALL=(ALL) NOPASSWD:ALL
    lock_passwd: true
    ssh_authorized_keys:
      - ${publicKey}
runcmd:
  - [sh, -c, 'echo PHASE1_LOCAL_LINUX_READY; uname -a']
`;
const seed = new Map([
  ['/user-data', userData],
  ['/meta-data', 'instance-id: kingdomzombies-phase1-20261113\nlocal-hostname: kingdomzombies-proof\n'],
  ['/vendor-data', '#cloud-config\n'],
  ['/network-config', 'version: 2\nethernets:\n  all:\n    match:\n      name: "en*"\n    dhcp4: true\n']
]);
const server = http.createServer((req, res) => {
  const value = seed.get(req.url);
  if (value === undefined) { res.writeHead(404); res.end(); return; }
  fs.appendFileSync(path.join(root, 'seed-access.log'), `${new Date().toISOString()} ${req.url}\n`);
  res.writeHead(200, { 'Content-Type': 'text/plain' }); res.end(value);
});
server.listen(28880, '127.0.0.1', () => {
  const serial = path.join(root, 'serial.log').replaceAll('\\', '/');
  const qemu = path.join(root, 'qemu', 'qemu-system-x86_64.exe');
  const args = ['-machine', 'q35', '-accel', accelerator === 'tcg' ? 'tcg,thread=multi' : 'whpx', '-cpu', cpu, '-smp', '4', '-m', '4096',
    '-drive', `if=pflash,format=raw,unit=0,readonly=on,file=${path.join(root, 'qemu', 'share', 'edk2-x86_64-code.fd')}`,
    '-drive', `if=pflash,format=raw,unit=1,file=${path.join(root, 'vars.fd')}`,
    '-drive', `file=${path.join(root, 'system.qcow2')},if=virtio,format=qcow2`,
    '-netdev', 'user,id=n0,hostfwd=tcp:127.0.0.1:23022-:22', '-device', 'virtio-net-pci,netdev=n0',
    '-smbios', 'type=1,serial=ds=nocloud;s=http://10.0.2.2:28880/',
    '-display', 'none', '-serial', `file:${serial}`, '-monitor', 'none',
    '-qmp', 'tcp:127.0.0.1:23023,server=on,wait=off'];
  const out = fs.openSync(path.join(root, 'qemu.stdout.log'), 'a');
  const err = fs.openSync(path.join(root, 'qemu.stderr.log'), 'a');
  const child = spawn(qemu, args, { cwd: root, windowsHide: true, stdio: ['ignore', out, err] });
  fs.writeFileSync(processRecord, JSON.stringify({ launcher: process.pid, qemu: child.pid, exe: qemu, started: new Date().toISOString(), sshPort: 23022, requestedAccelerator: accelerator, cpu }, null, 2));
  child.on('error', error => { fs.appendFileSync(path.join(root, 'qemu.stderr.log'), String(error)); server.close(); process.exitCode = 1; });
  child.on('exit', code => { server.close(); process.exitCode = code ?? 1; });
  setTimeout(() => server.close(), 30 * 60 * 1000).unref();
});
