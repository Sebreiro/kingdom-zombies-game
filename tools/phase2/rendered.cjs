'use strict';
const fs = require('node:fs');
const path = require('node:path');
const zlib = require('node:zlib');
const { spawn } = require('node:child_process');
const delay = milliseconds => new Promise(resolve => setTimeout(resolve, milliseconds));
function requireTrue(value, message) { if (!value) throw new Error(message); }
function decodePng(file, width = 1280, height = 720) {
  const bytes = fs.readFileSync(file);
  requireTrue(bytes.subarray(0, 8).equals(Buffer.from([137,80,78,71,13,10,26,10])), 'PNG_SIGNATURE');
  let offset = 8, actualWidth, actualHeight, depth, color, end = false;
  const data = [];
  while (offset + 12 <= bytes.length) {
    const size = bytes.readUInt32BE(offset), type = bytes.toString('ascii', offset + 4, offset + 8);
    requireTrue(offset + size + 12 <= bytes.length, 'PNG_TRUNCATED');
    const chunk = bytes.subarray(offset + 8, offset + 8 + size);
    if (type === 'IHDR') { actualWidth = chunk.readUInt32BE(0); actualHeight = chunk.readUInt32BE(4); depth = chunk[8]; color = chunk[9]; }
    if (type === 'IDAT') data.push(chunk);
    if (type === 'IEND') { end = true; break; }
    offset += size + 12;
  }
  requireTrue(end && actualWidth === width && actualHeight === height && depth === 8 && [2,6].includes(color), 'PNG_DIMENSIONS_FORMAT');
  const channels = color === 6 ? 4 : 3, stride = width * channels;
  const raw = zlib.inflateSync(Buffer.concat(data)), pixels = Buffer.alloc(height * stride);
  requireTrue(raw.length === height * (stride + 1), 'PNG_DECODE_LENGTH');
  for (let y = 0; y < height; y++) {
    const filter = raw[y * (stride + 1)];
    requireTrue(filter <= 4, 'PNG_FILTER');
    for (let x = 0; x < stride; x++) {
      const a = x >= channels ? pixels[y * stride + x - channels] : 0;
      const b = y ? pixels[(y - 1) * stride + x] : 0;
      const c = y && x >= channels ? pixels[(y - 1) * stride + x - channels] : 0;
      const p = a + b - c, pa = Math.abs(p - a), pb = Math.abs(p - b), pc = Math.abs(p - c);
      const predictor = filter === 1 ? a : filter === 2 ? b : filter === 3 ? Math.floor((a+b)/2) : filter === 4 ? pa <= pb && pa <= pc ? a : pb <= pc ? b : c : 0;
      pixels[y * stride + x] = (raw[y * (stride + 1) + 1 + x] + predictor) & 255;
    }
  }
  const colors = new Set();
  for (let i = 0; i < pixels.length; i += channels) colors.add(pixels.subarray(i, i + 3).toString('hex'));
  requireTrue(colors.size > 3, 'PNG_EMPTY_CONTENT');
  return { width, height, colors: colors.size, decodedBytes: pixels.length };
}
async function command(executable, args, options = {}, timeoutMilliseconds = 12000) {
  return new Promise((resolve, reject) => {
    const child = spawn(executable, args, { ...options, shell: false, windowsHide: true });
    let stdout = '', stderr = '';
    child.stdout?.on('data', chunk => { stdout += chunk; });
    child.stderr?.on('data', chunk => { stderr += chunk; });
    const timer = setTimeout(() => { child.kill(); reject(new Error('INPUT_DEADLINE')); }, timeoutMilliseconds);
    child.on('error', error => { clearTimeout(timer); reject(error); });
    child.on('exit', code => { clearTimeout(timer); code === 0 ? resolve({ stdout, stderr }) : reject(new Error(`INPUT_EXIT ${code}: ${stdout}${stderr}`)); });
  });
}
async function tracer(context, scenario = 'rendered.tracer', width = 1280, height = 720) {
  const { root, directory, nonce, engine, env } = context;
  const game = spawn(engine, ['--path', path.join(root, 'src/godot-game'), '--rendering-method', 'gl_compatibility', '--resolution', `${width}x${height}`], {
    cwd: root, env: { ...env, PHASE2_NONCE: nonce, PHASE2_OUTPUT: directory }, shell: false, windowsHide: false,
  });
  let log = '', exited = false, exitCode;
  game.stdout.on('data', chunk => { log += chunk; }); game.stderr.on('data', chunk => { log += chunk; });
  game.on('error', error => { log += error.stack; });
  game.on('exit', code => { exited = true; exitCode = code; });
  const inputs = [], images = [], observations = [];
  async function input(action, x = 0, y = 0, key = 0) {
    requireTrue(!exited, 'GAME_EXITED_BEFORE_INPUT');
    const windows = process.platform === 'win32';
    const result = await command(windows ? 'powershell.exe' : '/bin/sh', windows
      ? ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', path.join(root, 'tools/phase2/windows-input.ps1'), '-OwnedPid', String(game.pid), '-Action', action, '-X', String(x), '-Y', String(y), '-Key', String(key)]
      : [path.join(root, 'tools/phase2/linux-input.sh'), String(game.pid), action, String(x), String(y), String(key)], { cwd: root, env });
    const acknowledgement = JSON.parse(result.stdout.trim());
    requireTrue(acknowledgement.pid === game.pid && acknowledgement.focused === true && acknowledgement.action === action, 'INPUT_ACKNOWLEDGEMENT');
    inputs.push(acknowledgement);
  }
  async function observe(checkpoint, targetElapsed = null) {
    fs.writeFileSync(path.join(directory, 'observe.request.tmp'), checkpoint + (targetElapsed === null ? '' : '@' + targetElapsed), { flag: 'wx' });
    fs.renameSync(path.join(directory, 'observe.request.tmp'), path.join(directory, 'observe.request'));
    const target = path.join(directory, checkpoint + '.json'), deadline = Date.now() + 12000;
    while (!fs.existsSync(target)) {
      requireTrue(!exited && Date.now() < deadline, 'OBSERVATION_DEADLINE ' + checkpoint + ': ' + log);
      await delay(50);
    }
    const state = JSON.parse(fs.readFileSync(target, 'utf8'));
    requireTrue(state.nonce === nonce && state.checkpoint === checkpoint && state.focused, 'OBSERVATION_IDENTITY_FOCUS');
    images.push({ checkpoint, path: path.join(directory, checkpoint + '.png'), ...decodePng(path.join(directory, checkpoint + '.png'), width, height) }); observations.push(state);
    return state;
  }
  try {
    await delay(1500); await input('probe');
    const menu = await observe('launch-menu'); requireTrue(menu.actors === 0, 'LAUNCH_MENU');
    await input('click', 640, 328);
    const initial = await observe('initial-scene'); requireTrue(initial.actors === 1 && initial.actor.PositionSubunits === -8000, 'START_ONE_ACTOR');
    const key = name => process.platform === 'win32' ? { a:65,d:68,Left:37,Right:39,Escape:27 }[name] : name;
    const send = (action,name) => input(action,0,0,key(name));
    const intervals = [];
    if (['rendered.directions','rendered.distance','rendered.world'].includes(scenario)) {
      for (const [index,keys,direction] of [[0,['a'],-1],[1,['d'],1],[2,['Left'],-1],
        [3,['Right'],1],[4,['a','Left'],-1],[5,['d','Right'],1],
        [6,['a','Right'],0],[7,['Left','d'],0]]) {
        for (const name of keys) await send('down',name);
        const start = await observe('walk-start-' + index);
        const end = await observe(direction < 0 ? 'left-facing-' + index : 'right-facing-' + index, start.actor.ElapsedMilliseconds + 1000);
        const elapsed = end.actor.ElapsedMilliseconds - start.actor.ElapsedMilliseconds;
        const distance = (end.actor.PositionSubunits - start.actor.PositionSubunits)/1000;
        requireTrue(elapsed >= 1000 && elapsed <= 1050, 'SIMULATION_INTERVAL ' + elapsed);
        requireTrue(direction === 0 ? distance === 0 : distance * direction >= 2.85 && distance * direction <= 3.15, 'WALK_DISTANCE ' + distance);
        requireTrue(end.actor.ActorId === initial.actor.ActorId && end.actor.SessionId === initial.actor.SessionId && end.actor.GroundSubunits === 0, 'WALK_IDENTITY_GROUND');
        intervals.push({keys,direction,elapsed,distance,start:start.checkpoint,end:end.checkpoint});
        for (const name of keys) await send('up',name);
        const release = await observe('release-' + index);
        const stopped = await observe('stopped-' + index,release.actor.ElapsedMilliseconds + 100);
        requireTrue(stopped.direction === 0 && stopped.actor.PositionSubunits === release.actor.PositionSubunits, 'RELEASE_STOP');
      }
      await send('down','d'); await send('down','d');
      const repeat = await observe('repeat-held');
      const repeatEnd = await observe('repeat-end',repeat.actor.ElapsedMilliseconds + 1000);
      requireTrue(repeatEnd.actor.PositionSubunits-repeat.actor.PositionSubunits >= 2850 && repeatEnd.actor.PositionSubunits-repeat.actor.PositionSubunits <= 3150,'REPEAT_SPEED');
      await send('up','d');
      if (scenario === 'rendered.world') {
        await send('down','d');
        const deadline = Date.now()+65000;
        let state = await observe('world-start'), sample = 0;
        if (state.actor.PositionSubunits < 0) {
          state = await observe('building-overlap',state.actor.ElapsedMilliseconds+Math.ceil(-state.actor.PositionSubunits/3));
          requireTrue(Math.abs(state.actor.PositionSubunits)<150,'BUILDING_OVERLAP');
        }
        while (state.actor.PositionSubunits < 59000) {
          requireTrue(Date.now()<deadline,'RIGHT_BOUND_DEADLINE');
          state = await observe('world-right-' + sample++,Math.min(state.actor.ElapsedMilliseconds+4000,state.actor.ElapsedMilliseconds+Math.ceil((59000-state.actor.PositionSubunits)/3)));
          requireTrue(state.actor.ActorId===initial.actor.ActorId,'TRAVERSE_IDENTITY');
        }
        await send('up','d'); const right = await observe('right-bound');
        requireTrue(right.actor.PositionSubunits===59000 && right.actor.Facing===1,'RIGHT_CLAMP');
        await send('down','a');
        let left = await observe('inward-right');
        requireTrue(left.actor.PositionSubunits<59000,'RIGHT_INWARD');
        const leftDeadline=Date.now()+65000;
        while(left.actor.PositionSubunits > -59000) {
          requireTrue(Date.now()<leftDeadline,'LEFT_BOUND_DEADLINE');
          left=await observe('world-left-'+sample++,left.actor.ElapsedMilliseconds+Math.min(4000,Math.ceil((left.actor.PositionSubunits+59000)/3)));
        }
        await send('up','a'); const bound=await observe('left-bound');
        requireTrue(bound.actor.PositionSubunits===-59000 && bound.actor.Facing===-1,'LEFT_CLAMP');
        await send('down','d'); const inward=await observe('inward-left');
        requireTrue(inward.actor.PositionSubunits>-59000,'LEFT_INWARD'); await send('up','d');
      }
    }
    const beforeRight=observations.at(-1);
    await input('down', 0, 0, process.platform === 'win32' ? 68 : 'd'); await delay(1100);
    const right = await observe('right-facing');
    await input('up', 0, 0, process.platform === 'win32' ? 68 : 'd');
    requireTrue(right.actors === 1 && right.actor.ActorId === initial.actor.ActorId && right.actor.PositionSubunits > beforeRight.actor.PositionSubunits && right.actor.Facing === 1 && right.inputEvents > initial.inputEvents && right.actor.ElapsedMilliseconds > initial.actor.ElapsedMilliseconds, 'RIGHTWARD_NATIVE_COMMAND');
    await input('tap', 0, 0, process.platform === 'win32' ? 27 : 'Escape');
    const returned = await observe('returned-menu'); requireTrue(returned.actors === 0 && returned.actor === null, 'RETURNED_MENU');
    await input('click', 640, 392);
    const deadline = Date.now() + 8000;
    while (!exited && Date.now() < deadline) await delay(50);
    requireTrue(exited && exitCode === 0, 'QUIT_CLEAN_EXIT');
    requireTrue(!/ERROR:|WARNING:|Unhandled exception/.test(log), 'NATIVE_DIAGNOSTIC ' + log);
    return { inputs, observations, images, intervals, pid: game.pid, exitCode, cleanExit: true };
  } finally {
    fs.writeFileSync(path.join(directory, 'game.log'), log);
    fs.writeFileSync(path.join(directory, 'native-input.json'), JSON.stringify(inputs, null, 2));
    if (!exited) {
      if (process.platform === 'win32') await command('taskkill.exe', ['/PID', String(game.pid), '/T', '/F']).catch(() => {});
      else game.kill('SIGKILL');
    }
  }
}
module.exports = { tracer, decodePng, command };
