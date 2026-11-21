'use strict';

const {spawn,spawnSync} = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const {performance} = require('node:perf_hooks');
const {setTimeout:delay} = require('node:timers/promises');
const {need} = require('./evidence.cjs');

let systemPowerShell;
function windowsPowerShell() {
  if (systemPowerShell) return systemPowerShell;
  const root=process.env.SystemRoot;
  need(process.platform==='win32' && typeof root==='string' && /^[A-Z]:\\/i.test(root),'RENDERED_WINDOWS_ROOT');
  const executable=path.resolve(root,'System32','WindowsPowerShell','v1.0','powershell.exe');
  need(fs.realpathSync.native(executable).toLowerCase()===executable.toLowerCase() &&
    fs.lstatSync(executable).isFile(),'RENDERED_POWERSHELL_PATH');
  const script=[
    "$ErrorActionPreference='Stop'",
    "$ProgressPreference='SilentlyContinue'",
    "Import-Module ([IO.Path]::Combine($PSHOME,'Modules','Microsoft.PowerShell.Security','Microsoft.PowerShell.Security.psd1'))",
    "$binary=(Get-Process -Id $PID).Path",
    "$signature=Get-AuthenticodeSignature -LiteralPath $binary",
    "[pscustomobject]@{image=$binary;status=[string]$signature.Status;publisher=$signature.SignerCertificate.Subject}|ConvertTo-Json -Compress"
  ].join(';');
  const result=spawnSync(executable,['-NoProfile','-NonInteractive','-EncodedCommand',Buffer.from(script,'utf16le').toString('base64')],
    {encoding:'utf8',windowsHide:true,timeout:10000,maxBuffer:65536});
  need(!result.error && result.status===0,'RENDERED_POWERSHELL_AUTHENTICATION',
    result.error?.message??String(result.stderr).slice(0,512));
  const proof=JSON.parse(result.stdout);
  need(proof.image?.toLowerCase()===executable.toLowerCase() && proof.status==='Valid' &&
    /^CN=Microsoft Windows, O=Microsoft Corporation,/.test(proof.publisher),'RENDERED_POWERSHELL_PUBLISHER');
  systemPowerShell=executable;
  return systemPowerShell;
}

function managedMvid(data) {
  const u16=at=>data.readUInt16LE(at), u32=at=>data.readUInt32LE(at);
  const pe=u32(0x3c), optional=pe+24, format=u16(optional);
  need(u16(0)===0x5a4d && u32(pe)===0x4550 && [0x10b,0x20b].includes(format),'RENDERED_MANAGED_PE');
  const sections=optional+u16(pe+20), count=u16(pe+6);
  function fileOffset(rva) {
    for (let index=0;index<count;index++) {
      const at=sections+index*40, address=u32(at+12), bytes=u32(at+16);
      if (rva>=address && rva<address+bytes) return u32(at+20)+rva-address;
    }
    throw new Error('RENDERED_MANAGED_RVA');
  }
  const directories=optional+(format===0x10b?96:112), cli=fileOffset(u32(directories+14*8));
  const metadata=fileOffset(u32(cli+8)), streams=metadata+16+u32(metadata+12);
  need(u32(metadata)===0x424a5342 && u16(streams+2)<=32,'RENDERED_MANAGED_METADATA');
  let cursor=streams+4;
  const entries=new Map();
  for (let index=0;index<u16(streams+2);index++) {
    const nameAt=cursor+8, end=data.indexOf(0,nameAt);
    need(end>=nameAt && end-nameAt<32,'RENDERED_MANAGED_STREAM');
    entries.set(data.toString('ascii',nameAt,end),{at:metadata+u32(cursor),size:u32(cursor+4)});
    cursor=nameAt+Math.ceil((end-nameAt+1)/4)*4;
  }
  const tables=entries.get('#~')??entries.get('#-'), guids=entries.get('#GUID');
  need(tables && guids,'RENDERED_MANAGED_TABLES');
  const flags=data[tables.at+6], mask=data.readBigUInt64LE(tables.at+8);
  need(Boolean(mask&1n) && u32(tables.at+24)===1,'RENDERED_MANAGED_MODULE');
  let tableCount=0;
  for (let valid=mask;valid;valid>>=1n) if (valid&1n) tableCount++;
  const indexAt=tables.at+24+4*tableCount+2+(flags&1?4:2), index=flags&2?u32(indexAt):u16(indexAt);
  const guid=guids.at+(index-1)*16;
  need(index>0 && index*16<=guids.size && guid+16<=data.length,'RENDERED_MANAGED_MVID');
  return [u32(guid).toString(16).padStart(8,'0'),u16(guid+4).toString(16).padStart(4,'0'),
    u16(guid+6).toString(16).padStart(4,'0'),data.toString('hex',guid+8,guid+10),data.toString('hex',guid+10,guid+16)].join('-');
}

function timeBudget(milliseconds, signal, now = () => performance.now()) {
  const end = now() + milliseconds;
  return (limit = milliseconds) => {
    if (signal?.aborted) throw signal.reason;
    const remaining = Math.floor(end - now());
    need(remaining > 0,'RENDERED_GROUP_DEADLINE');
    return Math.min(remaining,limit);
  };
}

function observeChildren() {
  const owned = new Map();
  function track(child) {
    if (!child.pid) return child;
    owned.set(child.pid,child);
    return child;
  }
  function terminate(child) {
    if (child.exitCode !== null || child.signalCode !== null) return;
    spawnSync('taskkill.exe',['/PID',String(child.pid),'/T','/F'],{windowsHide:true,timeout:3000});
  }
  async function remaining() {
    if (!owned.size) return [];
    // Windows retains the creating PID even if its parent has already exited.
    const command = 'Get-CimInstance Win32_Process | ForEach-Object { "$($_.ProcessId) $($_.ParentProcessId)" }';
    const inventory = spawnSync(windowsPowerShell(),['-NoProfile','-NonInteractive','-Command',command],
      {windowsHide:true,encoding:'utf8',timeout:5000,maxBuffer:1024*1024});
    need(!inventory.error && inventory.status === 0,'RENDERED_PROCESS_INVENTORY');
    const rows = inventory.stdout.trim().split(/\r?\n/).map(row => {
      need(/^\d+ \d+$/.test(row),'RENDERED_PROCESS_INVENTORY_ROW');
      return row.split(' ').map(Number);
    });
    const found = new Set(owned.keys());
    // Ignore this query and its process tree. They are not children of an owned launch.
    let count;
    do {
      count = found.size;
      for (const [pid,parent] of rows) if (found.has(parent)) found.add(pid);
    } while (count !== found.size);
    return rows.filter(([pid]) => found.has(pid)).map(([pid]) => pid);
  }
  async function cleanup() {
    for (const child of owned.values()) terminate(child);
    let survivors = await remaining();
    if (survivors.length) {
      for (const pid of survivors) {
        const child = owned.get(pid);
        // A closed root can have survived only as a recycled PID. Leave it alone.
        if (!child || (child.exitCode === null && child.signalCode === null))
          spawnSync('taskkill.exe',['/PID',String(pid),'/T','/F'],{windowsHide:true,timeout:3000});
      }
      await delay(100);
      survivors = await remaining();
    }
    return survivors;
  }
  return {track,cleanup,remaining,pids:() => [...owned.keys()]};
}

async function command(owner, executable, args, options, timeout, signal) {
  if (signal?.aborted) throw signal.reason;
  const child = owner.track(spawn(executable,args,{...options,shell:false,windowsHide:true}));
  return new Promise((resolve,reject) => {
    let stdout = '', stderr = '', failure;
    const cancel = () => {
      failure ??= signal?.reason ?? new Error('RENDERED_HELPER_DEADLINE');
      if (child.pid && child.exitCode === null && child.signalCode === null)
        spawnSync('taskkill.exe',['/PID',String(child.pid),'/T','/F'],{windowsHide:true,timeout:3000});
    };
    const timer = setTimeout(cancel,timeout);
    signal?.addEventListener('abort',cancel,{once:true});
    child.stdout.on('data',data => {
      stdout += data;
      if (stdout.length > 1024*1024) {failure = new Error('RENDERED_HELPER_OUTPUT');cancel();}
    });
    child.stderr.on('data',data => {
      stderr += data;
      if (stderr.length > 1024*1024) {failure = new Error('RENDERED_HELPER_OUTPUT');cancel();}
    });
    child.on('error',error => {failure ??= error;});
    child.once('close',code => {
      clearTimeout(timer);
      signal?.removeEventListener('abort',cancel);
      if (failure || code !== 0) reject(failure ?? new Error('RENDERED_HELPER_EXIT '+code+' '+stderr.slice(0,2000)));
      else resolve({stdout,stderr});
    });
  });
}

module.exports = {managedMvid,timeBudget,observeChildren,command,windowsPowerShell};
