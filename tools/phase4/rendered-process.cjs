'use strict';

const {spawn,spawnSync} = require('node:child_process');
const {performance} = require('node:perf_hooks');
const {setTimeout:delay} = require('node:timers/promises');
const {need} = require('./evidence.cjs');

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
    const inventory = spawnSync('powershell.exe',['-NoProfile','-NonInteractive','-Command',command],
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

module.exports = {timeBudget,observeChildren,command};
