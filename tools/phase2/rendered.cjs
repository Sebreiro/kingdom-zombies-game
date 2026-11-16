'use strict';
const fs = require('node:fs');
const path = require('node:path');
const zlib = require('node:zlib');
const crypto = require('node:crypto');
const { spawn, spawnSync } = require('node:child_process');
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
  const raw = zlib.inflateSync(Buffer.concat(data),{maxOutputLength:height*(stride+1)}), pixels = Buffer.alloc(height * stride);
  requireTrue(raw.length === height * (stride + 1), 'PNG_DECODE_LENGTH');
  for (let y = 0; y < height; y++) {
    const filter = raw[y * (stride + 1)];
    requireTrue(filter <= 4, 'PNG_FILTER');
    for (let x = 0; x < stride; x++) {
      const a = x >= channels ? pixels[y * stride + x - channels] : 0;
      const b = y ? pixels[(y - 1) * stride + x] : 0;
      const c = y && x >= channels ? pixels[(y - 1) * stride + x - channels] : 0;
      let predictor = filter === 1 ? a : filter === 2 ? b : filter === 3 ? Math.floor((a+b)/2) : 0;
      if (filter === 4) {
        const p = a + b - c, pa = Math.abs(p - a), pb = Math.abs(p - b), pc = Math.abs(p - c);
        predictor = pa <= pb && pa <= pc ? a : pb <= pc ? b : c;
      }
      pixels[y * stride + x] = (raw[y * (stride + 1) + 1 + x] + predictor) & 255;
    }
  }
  const colors = new Set();
  for (let i = 0; i < pixels.length; i += channels) colors.add((pixels[i] << 16) | (pixels[i + 1] << 8) | pixels[i + 2]);
  requireTrue(colors.size > 3, 'PNG_EMPTY_CONTENT');
  return { width, height, colors: colors.size, decodedBytes: pixels.length };
}
async function command(executable, args, options = {}, timeoutMilliseconds = 12000) {
  return new Promise((resolve, reject) => {
    const child = spawn(executable, args, { ...options, shell: false, windowsHide: true, detached: process.platform !== 'win32' });
    let stdout = '', stderr = '', timedOut = false;
    child.stdout?.on('data', chunk => { stdout += chunk; });
    child.stderr?.on('data', chunk => { stderr += chunk; });
    const timer = setTimeout(() => { timedOut = true; killTree(child); }, timeoutMilliseconds);
    child.on('error', error => { clearTimeout(timer); reject(error); });
    child.on('close', code => { clearTimeout(timer); timedOut ? reject(new Error('INPUT_DEADLINE')) : code === 0 ? resolve({ stdout, stderr }) : reject(new Error(`INPUT_EXIT ${code}: ${stdout}${stderr}`)); });
  });
}
function killTree(child) {
  if (!child?.pid || child.exitCode !== null || child.signalCode !== null) return;
  if (process.platform === 'win32') spawnSync('taskkill.exe', ['/PID',String(child.pid),'/T','/F'], {windowsHide:true,timeout:5000});
  else { try { process.kill(-child.pid, 'SIGKILL'); } catch {} }
}
function ownedInput(dispatch) {
  const heldKeys=new Set();
  let heldMouse=null;
  async function input(action,x=0,y=0,key=0) {
    // Dispatch can inject a press before its acknowledgement fails.
    if(action==='down' || action==='tap')heldKeys.add(key);
    if(action==='mouse-down' || action==='click')heldMouse={x,y};
    await dispatch(action,x,y,key);
    if(action==='up' || action==='tap')heldKeys.delete(key);
    if(action==='mouse-up' || action==='click')heldMouse=null;
  }
  async function release() {
    const errors=[];
    for(const key of [...heldKeys])try {await input('up',0,0,key);}catch(error){errors.push(error);}
    if(heldMouse)try {await input('mouse-up',heldMouse.x,heldMouse.y);}catch(error){errors.push(error);}
    if(errors.length)throw new AggregateError(errors,'OWNED_INPUT_RELEASE_FAILED');
  }
  return {input,release};
}
function createSession(context) {
  const { root, directory, nonce, engine, env } = context;
  const width = 1280, height = 720;
  const game = spawn(engine, ['--path', path.join(root, 'src/godot-game/game'), '--rendering-method', 'gl_compatibility', '--resolution', `${width}x${height}`], {
    cwd: root, env: { ...env, PHASE2_NONCE: nonce, PHASE2_OUTPUT: directory,
      PHASE3_PROFILE_ROOT:path.join(directory,'profiles'),
      ...(context.includeMenu?{PHASE2_FAIL_ENTRY_ONCE:nonce}:{}) }, shell: false, windowsHide: false, detached:process.platform !== 'win32',
  });
  let log = '', exited = false, exitCode;
  game.stdout.on('data', chunk => { log += chunk; }); game.stderr.on('data', chunk => { log += chunk; });
  game.on('error', error => { log += error.stack; exited = true; });
  game.on('exit', code => { exited = true; exitCode = code; });
  const inputs = [];
  const {input,release}=ownedInput(async (action,x,y,key) => {
    requireTrue(!exited, 'GAME_EXITED_BEFORE_INPUT');
    const windows = process.platform === 'win32';
    const result = await command(windows ? 'powershell.exe' : '/bin/sh', windows
      ? ['-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', path.join(root, 'tools/phase2/windows-input.ps1'), '-OwnedPid', String(game.pid), '-Action', action, '-X', String(x), '-Y', String(y), '-Key', String(key)]
      : [path.join(root, 'tools/phase2/linux-input.sh'), String(game.pid), action, String(x), String(y), String(key)], { cwd: root, env });
    const acknowledgement = JSON.parse(result.stdout.trim());
    requireTrue(acknowledgement.pid === game.pid && acknowledgement.focused === true && acknowledgement.action === action, 'INPUT_ACKNOWLEDGEMENT');
    inputs.push(acknowledgement);
  });
  let sequence = 0;
  async function capture(checkpoint, targetElapsed = null) {
    const requestId = 'capture-' + (++sequence);
    const request = requestId + (targetElapsed === null ? '' : '@' + targetElapsed);
    requireTrue(!fs.existsSync(path.join(directory,'observe.request')), 'OBSERVATION_ALREADY_PENDING');
    fs.appendFileSync(path.join(directory,'requests.jsonl'),JSON.stringify({checkpoint,request,at:new Date().toISOString()})+'\n');
    fs.writeFileSync(path.join(directory, 'observe.request.tmp'), request, { flag: 'wx' });
    fs.renameSync(path.join(directory, 'observe.request.tmp'), path.join(directory, 'observe.request'));
    const target = path.join(directory, requestId + '.json'), deadline = Date.now() + 12000;
    while (!fs.existsSync(target)) {
      requireTrue(!exited && Date.now() < deadline, 'OBSERVATION_DEADLINE ' + checkpoint + ': ' + log);
      await delay(50);
    }
    const state = JSON.parse(fs.readFileSync(target, 'utf8'));
    requireTrue(state.nonce === nonce && state.checkpoint === requestId && state.focused, 'OBSERVATION_IDENTITY_FOCUS');
    return {state:{...state,checkpoint,captureId:requestId}, image:{checkpoint,path:requestId+'.png',
      statePath:requestId+'.json',stateSha256:crypto.createHash('sha256').update(fs.readFileSync(target)).digest('hex'),
      sha256:crypto.createHash('sha256').update(fs.readFileSync(path.join(directory,requestId+'.png'))).digest('hex'),...decodePng(path.join(directory,requestId+'.png'),width,height)}};
  }
  async function reset() {
    requireTrue(!exited,'GAME_PROCESS_LOST');
    await release();
    fs.rmSync(path.join(directory,'observe.request'),{force:true});
    await input('tap',0,0,process.platform==='win32'?27:'Escape');
    requireTrue((await capture('reset-menu')).state.actors === 0,'RESET_MENU');
  }
  async function close() {
    try {await release();}
    finally {
      killTree(game);
      fs.writeFileSync(path.join(directory,'game.log'),log);
      fs.writeFileSync(path.join(directory,'native-input.json'),JSON.stringify(inputs,null,2));
    }
  }
  return {input,capture,reset,close,game,inputs,get exited(){return exited;},get exitCode(){return exitCode;},get log(){return log;}};
}
async function tracer(context, scenario, session, logStart = session.log.length) {
  const { input } = session, width = 1280, height = 720;
  const images = [], observations = [];
  async function observe(checkpoint,targetElapsed=null) {
    const captured = await session.capture(checkpoint,targetElapsed);
    images.push(captured.image); observations.push(captured.state);
    return captured.state;
  }
  try {
    await input('probe');
    const menu = await observe('launch-menu'); requireTrue(menu.actors === 0 && menu.startFocused, 'LAUNCH_MENU');
    const key = name => process.platform === 'win32' ? { a:65,d:68,Left:37,Right:39,Escape:27,Enter:13,space:32,Tab:9,Shift_L:16,Up:38,Down:40 }[name] : name;
    const send = (action,name) => input(action,0,0,key(name));
    if (scenario === 'rendered.menu') {
      await input('move',menu.startX,menu.startY); await observe('start-hover-focus');
      await require('./menu-navigation.cjs').check(send,observe,requireTrue);
      await input('mouse-down',menu.startX,menu.startY); await observe('start-pressed'); await input('mouse-up',menu.startX,menu.startY);
      const error=await observe('recoverable-error');
      requireTrue(error.actors===0 && error.errorVisible && error.startFocused && error.menuVisible,'RECOVERABLE_MENU');
    }
    if (scenario === 'rendered.menu') await send('tap','Enter'); else await input('click', menu.startX, menu.startY);
    const initial = await observe('initial-scene'); requireTrue(initial.actors === 1 && initial.actor.PositionSubunits === -8000, 'START_ONE_ACTOR');
    requireTrue(!initial.errorVisible,'RETRY_ERROR_CLEARED');
    const intervals = [];
    if (scenario === 'rendered.focus') {
      await send('down','d'); await send('down','Right'); const held=await observe('before-focus-loss');
      await input('focus-cycle'); const regained=await observe('focus-regained');
      const stopped=await observe('focus-stopped',regained.actor.ElapsedMilliseconds+500);
      requireTrue(stopped.actor.PositionSubunits===regained.actor.PositionSubunits && stopped.direction===0,'FOCUS_HELD_BARRIER');
      await send('up','d');
      const alternate=await observe('alternate-still-held');
      const alternateStopped=await observe('alternate-still-stopped',alternate.actor.ElapsedMilliseconds+300);
      requireTrue(alternateStopped.actor.PositionSubunits===alternate.actor.PositionSubunits && alternateStopped.direction===0,'ALTERNATE_HELD_BARRIER');
      await send('up','Right'); await send('down','Right');
      const fresh=await observe('fresh-focus-press');
      requireTrue(fresh.actor.PositionSubunits>stopped.actor.PositionSubunits && fresh.actor.ActorId===held.actor.ActorId,'FOCUS_FRESH_PRESS');
      await send('up','Right');
    }
    if (scenario === 'rendered.lifecycle') {
      await send('down','d'); await send('down','Right'); await send('down','Escape');
      const returned=await observe('held-return'); requireTrue(returned.actors===0,'HELD_ESCAPE_RETURN');
      await send('down','Escape'); requireTrue((await observe('repeat-escape-menu')).actors===0,'REPEATED_ESCAPE');
      await send('down','Enter'); const restart=await observe('restart');
      requireTrue(restart.actors===1 && restart.actor.PositionSubunits===-8000 && restart.direction===0
        && restart.actor.ActorId!==initial.actor.ActorId && restart.actor.SessionId!==initial.actor.SessionId
        && restart.actor.PlayerId===initial.actor.PlayerId,'FRESH_RESTART');
      await send('down','Enter'); const still=await observe('repeat-start',restart.actor.ElapsedMilliseconds+200);
      requireTrue(still.actors===1 && still.actor.ActorId===restart.actor.ActorId && still.actor.PositionSubunits===-8000,'REPEAT_START');
      await send('up','Escape'); await send('tap','Escape');
      const again=await observe('return-with-held-enter'); requireTrue(again.actors===0,'RETURN_AGAIN');
      await send('down','Enter'); requireTrue((await observe('held-enter-menu')).actors===0,'HELD_ENTER_NO_START');
      await send('up','Enter'); await send('tap','space'); const fresh=await observe('space-restart');
      requireTrue(fresh.actors===1 && fresh.direction===0 && fresh.actor.PositionSubunits===-8000,'SPACE_FRESH_RESTART');
      await send('up','d'); await send('up','Right');
      for(let index=0;index<3;index++) {
        await send('tap','Escape'); requireTrue((await observe('rapid-return-'+index)).actors===0,'RAPID_ZERO');
        await send('tap','Enter'); requireTrue((await observe('rapid-start-'+index)).actors===1,'RAPID_ONE');
      }
    }
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
        const approach=await observe('building-approach');
        if(approach.actor.PositionSubunits>-6000) {
          await send('down','a');
          const retreat=await observe('building-retreat');
          await observe('building-ready',retreat.actor.ElapsedMilliseconds+Math.max(0,Math.ceil((retreat.actor.PositionSubunits+6000)/3)));
          await send('up','a');
        }
        await send('down','d');
        const deadline = Date.now()+65000;
        let state = await observe('world-start'), sample = 0;
        if (state.actor.PositionSubunits < 0) {
          state = await observe('building-overlap',state.actor.ElapsedMilliseconds+Math.ceil(-state.actor.PositionSubunits/3));
          requireTrue(Math.abs(state.actor.PositionSubunits)<3000,'BUILDING_OVERLAP');
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
    requireTrue(right.actors === 1 && right.actor.ActorId === beforeRight.actor.ActorId && right.actor.PositionSubunits > beforeRight.actor.PositionSubunits && right.actor.Facing === 1 && right.inputEvents > initial.inputEvents && right.actor.ElapsedMilliseconds > beforeRight.actor.ElapsedMilliseconds, 'RIGHTWARD_NATIVE_COMMAND');
    await send('down','a');
    const leftFacing=await observe('left-facing');
    requireTrue(leftFacing.actor.Facing===-1 && leftFacing.actor.ActorId===right.actor.ActorId,'LEFT_FACING_NATIVE_COMMAND');
    await send('up','a');
    await input('tap', 0, 0, process.platform === 'win32' ? 27 : 'Escape');
    const returned = await observe('returned-menu'); requireTrue(returned.actors === 0 && returned.actor === null, 'RETURNED_MENU');
    if (scenario === 'rendered.resources') {
      const ids=observations.find(row=>row.actor)?.actor;
      requireTrue(ids && ['FixtureId','GroundId','BuildingId','PlayerContentId'].every(key=>ids[key].startsWith('foundation.')),'RESOURCE_CONTENT_IDENTITIES');
      requireTrue(!/ERROR:|WARNING:|Unhandled exception/.test(session.log.slice(logStart)),'NATIVE_DIAGNOSTIC '+session.log.slice(logStart));
      return {...validateResources(context),contentIds:[ids.FixtureId,ids.GroundId,ids.BuildingId,ids.PlayerContentId],observations,images,intervals,pid:session.game.pid,width,height};
    }
    if (scenario === 'rendered.quit') {
      for(const control of ['Host','Join','Quit']){
        await send('tap','Tab');requireTrue((await observe('quit-keyboard-'+control.toLowerCase())).focusedControl===control,'QUIT_KEYBOARD_FOCUS '+control);
      }
      await send('tap','space');
      const deadline = Date.now() + 8000;
      while (!session.exited && Date.now() < deadline) await delay(50);
      requireTrue(session.exited && session.exitCode === 0, 'QUIT_CLEAN_EXIT');
    }
    requireTrue(!/ERROR:|WARNING:|Unhandled exception/.test(session.log.slice(logStart)), 'NATIVE_DIAGNOSTIC ' + session.log.slice(logStart));
    return { observations, images, intervals, pid: session.game.pid,width,height,exitCode:session.exitCode,
      ...(scenario==='rendered.quit'?{quitActivation:'keyboard-space'}:{}),cleanExit:scenario==='rendered.quit' };
  } catch(error) {
    error.partial={observations,images,pid:session.game.pid,width,height};
    throw error;
  } finally {
    fs.writeFileSync(path.join(context.directory,scenario+'.observations.json'),JSON.stringify({observations,images},null,2));
  }
}
function validateResources(context) {
  const isolated=path.join(context.directory,'isolated-input');
  fs.mkdirSync(isolated,{recursive:true});
  const inputs=['src/godot-game/game/project.godot','src/godot-game/game/icon.svg',
    ...fs.readdirSync(path.join(context.root,'src/godot-game/game/foundation')).filter(name=>/\.(cs|tscn)$/.test(name)).map(name=>'src/godot-game/game/foundation/'+name),
    'tools/phase2/windows-input.ps1','tools/phase2/linux-input.sh'];
  const resources=[];
  for(const file of inputs) {
    const bytes=fs.readFileSync(path.join(context.root,file));
    requireTrue(!/LangProj|Downloads|[A-Za-z]:\\|\/home\//.test(bytes.toString('utf8')),'ABSOLUTE_OR_DONOR_RESOURCE '+file);
    fs.mkdirSync(path.dirname(path.join(isolated,file)),{recursive:true});
    fs.writeFileSync(path.join(isolated,file),bytes);
    resources.push({path:file,origin:'repository-authored',sha256:crypto.createHash('sha256').update(bytes).digest('hex')});
  }
  const declared = new Set(inputs);
  for (const resource of resources) {
    const source = fs.readFileSync(path.join(isolated,resource.path),'utf8');
    for (const match of source.matchAll(/res:\/\/([^"\s]+)/g)) requireTrue(declared.has('src/godot-game/game/'+match[1]),'UNDECLARED_RESOURCE '+match[1]);
    requireTrue(crypto.createHash('sha256').update(fs.readFileSync(path.join(isolated,resource.path))).digest('hex')===resource.sha256,'RESOURCE_COPY_HASH');
  }
  return {isolatedInputRoot:isolated,resources,donorInputs:[],validation:'declared-resource-closure-and-byte-identical-copy; shared live scene supplies runtime evidence'};
}
async function rendered(context,items) {
  const session=createSession({...context,includeMenu:items.some(row=>row.id==='rendered.menu')});
  const interrupted=()=>{killTree(session.game);};
  process.on('SIGINT',interrupted); process.on('SIGTERM',interrupted);
  let results;
  try {
    await delay(1500);
    results=await runScenarios(context,items,session);
    return results;
  } finally {
    try {await closeSession(session,results);}
    finally {process.removeListener('SIGINT',interrupted);process.removeListener('SIGTERM',interrupted);}
  }
}
async function closeSession(session,results) {
  try {await session.close();}
  catch(error) {
    if(!results?.length)throw error;
    const affected=results.findLast(row=>row.status!=='blocked')??results.at(-1);
    affected.scenarioStatus=affected.status;
    affected.cleanupError={message:error.message,causes:(error.errors??[]).map(cause=>cause.message)};
    if(affected.status==='pass')affected.status='fail';
    affected.message=(affected.message?affected.message+'; ':'')+'SESSION_CLEANUP '+error.message;
  }
}
async function runScenarios(context,items,session) {
  const ordered=[...items].sort((a,b)=>(a.id==='rendered.quit'?100:a.id==='rendered.menu'?-1:0)-(b.id==='rendered.quit'?100:b.id==='rendered.menu'?-1:0));
  const results=[];
  let blocked;
  for (const item of ordered) {
      const identity={id:item.id,nonce:context.nonce,input:context.input,tool:context.tool,platform:context.platform};
      if(blocked || session.exited) {results.push({...identity,status:'blocked',message:blocked??'GAME_PROCESS_LOST'});continue;}
      // The first scenario owns startup output; later scenarios start after recovery.
      const logStart=results.length===0?0:session.log.length;
      try { results.push({...identity,status:'pass',...await tracer(context,item.id,session,logStart),nativeLog:{start:logStart,end:session.log.length,includesStartup:results.length===0}}); }
      catch(error) {
        results.push({...identity,...error.partial,status:'fail',message:error.message,nativeLog:{start:logStart,end:session.log.length,includesStartup:results.length===0}});
        if(!session.exited) try {await session.reset();} catch(resetError){blocked='SESSION_UNUSABLE '+resetError.message;}
        else blocked='GAME_PROCESS_LOST';
      }
  }
  return results;
}
module.exports = { tracer, rendered, runScenarios, closeSession, ownedInput, decodePng, command, killTree, validateResources };
