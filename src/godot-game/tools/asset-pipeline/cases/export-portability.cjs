'use strict';

const fs=require('node:fs'),path=require('node:path');
const {isDeepStrictEqual:equal}=require('node:util');
const {registerCase,cases,command}=require('../check.cjs');
const tool=require('../export.cjs');
const {ROOT,need,write,inside,parse,hash,document}=require('../content.cjs');
const PACK='KingdomZombies.pck',INDEX='content/pack-index.json',AUTHORITY='content/expected-obligations.json';
const MISSING='content/packs/items-11-24.json',MISSING_ID='IT-11',SIBLING_ID='IT-07';

async function hubConsumer(context){
  const built=await tool.buildPackage(context);
  const {clean,engine,contract}=built;
  const pck=path.join(clean,'KingdomZombies.pck');
  write(context.directory,'hub-source-contract.json',{
    ...contract,index:undefined,expected:undefined,tracer:undefined,
    expectedObligations:contract.index.expectedObligations,
    independentIds:contract.expected.obligations.map(row=>row.id).sort(),
    retainedBow:{id:contract.tracer.id,name:contract.tracer.name,path:contract.tracer.path,
      sha256:contract.tracer.sha256,image:contract.tracer.image,registration:contract.tracer.registration},
  });
  const {result,controls,runtimeResolution}=await tool.portable(context,'hub-native',clean,contract);
  write(context.directory,'observer-controls.json',controls);
  const resources=await tool.packagedProbe(context,'hub-package-resource',engine,pck,contract);
  tool.verifyInputs(built);
  need(result.packageSha256===resources.pckSha256,'HUB_PACKAGE_IDENTITY');
  const evidence={portable:clean,files:built.closure.rows,native:result,runtimeResolution,
    editorSupport:resources,observerControls:controls.length};
  write(context.directory,'hub-package.json',evidence);
  return evidence;
}

registerCase('export.hub-consumer',{
  title:'Current packaged inspection hub and consumer',
  rationale:'The actual packaged hub must independently load and diagnose current content before acceptance',
  group:'export',importance:'Critical',needsGameWindow:false,runGroup:'G0-export-hub',order:310,
  requirements:['ASSET-05'],acceptance:['AC-15','AC-16','AC-17'],
  setup:'Pinned official Windows debug export template, production scene and C# projects, runtime source and independent obligation register',
  actions:['Publish and export the current production project','Check runtime and PCK member closures',
    'Start the exported executable naturally with only read-only debugger observations',
    'Verify exact hub paths, resource and register bindings, and independent rejection controls',
    'Check the same exported PCK and assemblies with the separate editor-mounted resource probe'],
  expected:'Clean exported headless exit with exact current hub, selected bow binding, register and controls; edited records fail independently',
  reset:'Use fresh owned package and profile directories and a bounded loopback observer; retain child and packet receipts',
  evidence:['project-inputs.json','pck-runtime-closure.json','hub-source-contract.json','hub-native.command.json',
    'hub-native-debugger.json','hub-native-observation.json','observer-controls.json','hub-package-resource.command.json',
    'hub-package.json','result.json']
},hubConsumer);

function itemProbe(pck,nonce){
  return `extends SceneTree
func _initialize():
    call_deferred("verify_items")
func select_item(list, id):
    for i in range(list.item_count):
        if list.get_item_text(i).split(" · ")[0] == id:
            list.select(i)
            list.item_selected.emit(i)
            return true
    return false
func verify_items():
    if not ProjectSettings.load_resource_pack(${JSON.stringify(pck.replaceAll('\\','/'))}):
        quit(11)
        return
    var scene = load("res://inspection/Inspection.tscn").instantiate()
    root.add_child(scene)
    var operation = scene.get_node("Shell/Toolbar/Operation")
    while operation.text != "Validated; visual/listening acceptance pending":
        await process_frame
    var list = scene.get_node("Shell/Workspace/Controls/Browser/Resources")
    var status = scene.get_node("Summary/Status")
    var id = scene.get_node("Summary/ResourceId")
    var failures = scene.get_node("Shell/Workspace/Preview/Failures")
    if not select_item(list, "${MISSING_ID}"):
        quit(12)
        return
    await process_frame
    var missing = {"id":id.text, "status":status.text, "diagnostics":failures.text}
    if not select_item(list, "${SIBLING_ID}"):
        quit(13)
        return
    await process_frame
    var nodes = scene.find_children("Selection", "AnimationPlayer", true, false)
    var sibling = {"id":id.text, "status":status.text, "name":scene.get_node("Summary/ResourceName").text,
        "nativeClip":str(nodes[0].assigned_animation) if nodes.size() == 1 else ""}
    print("PORTABLE_ITEMS " + JSON.stringify({"pid":OS.get_process_id(), "nonce":"${nonce}", "missing":missing, "sibling":sibling}))
    scene.free()
    quit(0)
`;
}
async function fixtureEngine(context){
  const pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
  const name=pin.executable.replace('_console.exe','.exe'),entry=pin.directory+'/'+name;
  const archive=inside(ROOT,'.phase1/cache/'+pin.file);
  need(await tool.digest(archive)===pin.sha256,'PORTABLE_FIXTURE_ENGINE_ARCHIVE');
  const executable=inside(ROOT,'.phase1/tools/'+entry);
  const program="Add-Type -AssemblyName System.IO.Compression.FileSystem; "+
    "$zip=[IO.Compression.ZipFile]::OpenRead('"+archive.replaceAll("'","''")+"'); "+
    "try { $rows=@($zip.Entries | Where-Object { $_.FullName -ceq '"+entry+"' }); "+
    "if($rows.Count -ne 1 -or $rows[0].Length -lt 1 -or $rows[0].Length -gt 268435456){throw 'EDITOR_ENTRY'}; "+
    "$stream=$rows[0].Open(); $sha=[Security.Cryptography.SHA256]::Create(); "+
    "try { [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } "+
    "finally { $sha.Dispose(); $stream.Dispose() } } finally { $zip.Dispose() }";
  const origin=await command(context,'fixture-engine-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',program]);
  const digest=await tool.digest(executable);
  need(origin.stdout.trim()===digest,'PORTABLE_FIXTURE_ENGINE_IDENTITY',entry);
  write(context.directory,'fixture-engine.json',{executable,entry,archiveSha256:pin.sha256,sha256:digest});
  return executable;
}
async function inspectItems(context,engine,name,pck,reason){
  const mounted=await tool.mountPackage(context,name,engine,pck,itemProbe(pck,context.nonce));
  const lines=mounted.command.stdout.split(/\r?\n/).filter(line=>line.startsWith('PORTABLE_ITEMS '));
  need(lines.length===1,'PORTABLE_ITEMS_RECEIPT',name);
  const observed=JSON.parse(lines[0].slice('PORTABLE_ITEMS '.length));
  const {missing,sibling}=observed;
  need(observed.pid===mounted.command.pid&&observed.nonce===context.nonce,'PORTABLE_ITEMS_IDENTITY',name);
  need(missing.id===MISSING_ID&&missing.status.startsWith('Error — '+MISSING_ID+' — '+reason+' '+MISSING)&&
    missing.diagnostics.includes(MISSING_ID+': res://'+INDEX+', field outputs, value ')&&
    missing.diagnostics.includes('actual named completed definition unavailable')&&
    (reason==='WORLD_INSPECTOR_HASH'||missing.diagnostics.includes('res://'+MISSING+', field bytes, value read — ')),
    'PORTABLE_EXACT_MISSING_ENTRY',JSON.stringify(missing));
  need(sibling.id===SIBLING_ID&&sibling.name==='Authored world/item view · '+SIBLING_ID&&
    sibling.status==='Partial — validated bytes; native/visual/listening acceptance pending'&&sibling.nativeClip===SIBLING_ID,
    'PORTABLE_VALID_SIBLING',JSON.stringify(sibling));
  const evidence={...observed,pckSha256:mounted.pckSha256,scope:mounted.scope};
  write(context.directory,name+'-item-evidence.json',evidence);
  return evidence;
}
function validateDelivery(built){
  const {files}=built.packageData;
  const document=parse(files.get('content/packs/delivery.json').data,'packaged delivery');
  const ids=['DT-19','DT-20','DT-21','DT-22','DT-23','DT-26','DT-27'];
  need(equal(Object.keys(document.supportCrosswalk),ids)&&equal(document.obligations,ids)&&
    document.acceptedObligations.length===0,'PORTABLE_DELIVERY_CROSSWALK');
  for(const row of Object.values(document.supportCrosswalk)){
    need(row.runtime.length>0&&row.runtime.every(file=>files.has(file))&&
      row.cases.length>0&&row.cases.every(id=>cases.has(id)),'PORTABLE_DELIVERY_BINDING',row.runtime.join(' '));
  }
  for(const entry of document.guides){
    need(Buffer.byteLength(entry.text)===entry.bytes&&hash(Buffer.from(entry.text))===entry.sha256,
      'PORTABLE_GUIDE_IDENTITY',entry.path);
  }
  need(files.get('docs/README.md').sha256===document.guides.find(guide=>guide.path==='src/godot-game/docs/README.md')?.sha256,
    'PORTABLE_PACKAGED_README');
  return ids;
}
async function portability(context){
  const built=await tool.buildPackage(context);
  const {clean,closure,contract}=built;
  const delivered=validateDelivery(built);
  const pck=inside(clean,PACK),positive=fs.readFileSync(pck);
  const initial=await tool.portable(context,'clean-start',clean,contract);
  const editor=await fixtureEngine(context);
  const directory=tool.cloneOutput(context,'negative-package',clean,closure.rows.filter(x=>x.path!==PACK));
  const current=inside(directory,PACK),controls=[];
  const baseline=new Map([...built.packageData.files].map(([name,row])=>[name,row.data]));
  function variant(name,change){
    const members=new Map(baseline);
    change(members);
    const candidate=tool.packBytes(positive,members),file=inside(directory,name+'.pck');
    write(directory,name+'.pck',candidate);
    return {file,sha256:hash(candidate)};
  }
  async function reject(name,code,action,detail=''){
    let error;try{await action();}catch(failure){error=failure.message;}
    need(error&&(typeof code==='string'?error.startsWith(code):code.test(error))&&error.includes(detail),
      'PORTABLE_NEGATIVE',name+' '+error);
    need(!fs.existsSync(inside(context.directory,name+'-observation.json')),'PORTABLE_NEGATIVE_RECEIPT',name);
    const row={name,expected:String(code),observed:error,subjectStatus:'rejected'};
    controls.push(row);write(context.directory,name+'-rejection.json',row);
  }
  function mountedVariant(name,change,reason){
    const candidate=variant(name,change);
    return inspectItems(context,editor,name,candidate.file,reason);
  }
  function remove(name,alias){
    return members=>{
      if(alias)members.set(alias,members.get(name));
      need(members.delete(name),'PORTABLE_CONTROL_SUBJECT',name);
    };
  }
  function changeIndex(members,edit){
    const index=parse(members.get(INDEX),INDEX);
    edit(index);members.set(INDEX,Buffer.from(JSON.stringify(index,null,2)+'\n'));
  }
  const partial=variant('missing-entry',remove(MISSING));
  fs.copyFileSync(partial.file,current);
  await reject('missing-entry-native','PACKAGED_NATIVE_REGISTER',
    ()=>tool.portable(context,'missing-entry-native',directory,contract),'source-ready');
  const missing=await inspectItems(context,editor,'missing-entry',partial.file,'WORLD_INSPECTOR_MISSING');
  const aliases=[];
  for(const [name,alias] of [['nearby-file','content/packs/items-11-24-copy.json'],['wrong-case','content/packs/Items-11-24.json']]){
    aliases.push(await mountedVariant(name,remove(MISSING,alias),'WORLD_INSPECTOR_MISSING'));
  }
  const renderedFixtures=Object.fromEntries(Object.keys(tool.RENDERED_FIXTURES).map((id,index)=>{
    const {data,...identity}=tool.renderedFixture(positive,id);
    need(identity.sha256===[aliases[0],missing][index].pckSha256,'PORTABLE_RENDERED_FIXTURE_SUBJECT',id);
    tool.verifyRenderedFixture(positive,id,identity);
    return [id,identity];
  }));
  const reordered=await mountedVariant('reordered-missing-entry',members=>{
    remove(MISSING)(members);changeIndex(members,index=>{index.files.reverse();index.outputs.reverse();});
  },'WORLD_INSPECTOR_MISSING');
  need(reordered.missing.status===missing.missing.status&&equal(reordered.sibling,missing.sibling),
    'PORTABLE_REORDERED_SIBLING');
  await reject('conflicting-case','PCK_PATH_CONFLICT',()=>variant('duplicate-case',members=>{
    members.set('content/packs/Items-11-24.json',members.get(MISSING));
  }));
  await reject('encoded-alias','PATH_INVALID',()=>variant('encoded',remove(MISSING,'content/packs/items-%31%31-24.json')));
  const changed=await mountedVariant('changed-dependency',members=>{
    members.set(MISSING,Buffer.concat([members.get(MISSING),Buffer.from(' ')]));
  },'WORLD_INSPECTOR_HASH');
  for(const [name,edit,code,detail] of [
    ['empty-discovery',members=>changeIndex(members,index=>{index.files=[];index.outputs=[];}),
      'PACKAGED_NATIVE_REGISTER','0 inspectable outputs'],
    ['missing-register',remove(AUTHORITY),/^PACKAGED_NATIVE_(REGISTER|DIAGNOSTICS) /,'field $, value independent180'],
    ['tampered-register',members=>{members.set(AUTHORITY,Buffer.concat([members.get(AUTHORITY),Buffer.from(' ')]));},
      /^PACKAGED_NATIVE_(REGISTER|DIAGNOSTICS) /,'approved independent register bytes/header/hash required']
  ]){
    const candidate=variant(name,edit);fs.copyFileSync(candidate.file,current);
    if(typeof code==='string'){
      await reject(name,code,()=>tool.portable(context,name,directory,contract),detail);
    }else{
      await reject(name,code,()=>tool.portable(context,name,directory,contract));
      const raw=parse(fs.readFileSync(inside(context.directory,name+'-debugger.json')),name);
      const roles=require('../native-observer.cjs').tree(raw.tree);
      const failures=roles.get('/root/Inspection/Shell/Workspace/Preview/Failures');
      const properties=raw.objects.find(row=>row[0]===failures.id)?.[2];
      const diagnostic=properties?.find(row=>row[0]==='text')?.[5];
      need(diagnostic?.startsWith('expected: res://'+AUTHORITY+', field $, value independent180 — ')&&
        diagnostic.includes(detail),'PORTABLE_AUTHORITY_DIAGNOSTIC',String(diagnostic));
      write(context.directory,name+'-authority.json',{pid:raw.reportedPid,pckSha256:candidate.sha256,diagnostic});
    }
  }
  const badLaunch=variant('failed-launch',remove('inspection/Inspection.tscn.remap'));
  fs.copyFileSync(badLaunch.file,current);
  await reject('failed-current-launch','CHILD_FAILED',()=>tool.portable(context,'failed-current-launch',directory,contract));
  const failed=document(context.directory,'failed-current-launch.command.json');
  need(failed.code===1&&failed.streamsClosed&&!failed.reason&&
    failed.stderr.includes('Failed loading scene: res://inspection/Inspection.tscn.'),
    'PORTABLE_FAILED_LAUNCH_IDENTITY');
  const interrupted=inside(context.directory,'interrupted-import');fs.mkdirSync(interrupted);
  write(interrupted,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Owned interruption fixture"\n'));
  const font='assets/ui/LiberationSans.ttf';
  write(interrupted,font,fs.readFileSync(inside(built.project,font)));
  const marker='Started Project initialization';
  await reject('interrupt-editor-import','LOADER_INTERRUPTED',()=>command(context,'interrupt-editor-import',editor,
    ['--headless','--path',interrupted,'--editor','--import'],{interruptMarker:marker,env:built.authoring}));
  const stopped=document(context.directory,'interrupt-editor-import.command.json');
  need(stopped.stdout.includes(marker)&&!stopped.stdout.includes('Finalizing Asset Import')&&
    stopped.streamsClosed&&!stopped.cleanupError&&stopped.code!==0,
    'PORTABLE_INTERRUPTION_IDENTITY');
  const relaunch=await tool.portable(context,'clean-relaunch',clean,contract);
  need(initial.result.pid!==relaunch.result.pid&&initial.result.launchId!==relaunch.result.launchId&&
    initial.result.packageSha256===relaunch.result.packageSha256&&equal(tool.inventory(clean),closure.rows),
    'PORTABLE_RELAUNCH_IDENTITY');
  tool.verifyInputs(built);
  const receipt={revision:context.revision,nonce:context.nonce,portable:clean,
    executable:inside(clean,'KingdomZombies.exe'),pck,pckSha256:relaunch.result.packageSha256,files:closure.rows,delivered,
    launches:[initial.result,relaunch.result],observerControls:relaunch.controls.length,
    runtime:[initial.runtimeResolution,relaunch.runtimeResolution],
    fixtureScope:'Pin-matched editor observations with genuine PCK variants and exported assemblies',
    variants:{missing,aliases,reordered,changed},renderedFixtures,controls,interruptedPid:stopped.pid,
    scope:'Windows portable delivery and natural headless launch, close and relaunch'};
  write(context.directory,'portable-delivery.json',receipt);
  return receipt;
}

registerCase('export.portability',{
  title:'Portable Windows package launch, close, relaunch and failure isolation',
  rationale:'An external dependency, false successful startup or a failure hiding a valid sibling blocks inspection delivery',
  group:'export',importance:'Critical',needsGameWindow:false,runGroup:'G0-export-hub',order:320,
  requirements:['ASSET-05'],acceptance:['AC-15','AC-16','AC-17','AC-19'],
  setup:'Current genuine Windows inspection export, exact authored delivery crosswalk, canonical PCK and independent 180-obligation register',
  actions:['Build and launch the genuine package with a fresh runtime profile and local dependency resolution',
    'Exercise independent missing, changed, nearby and wrong-case resources while selecting a valid sibling',
    'Reject encoded and conflicting paths, empty discovery, invalid authority, a failed launch and an interrupted import',
    'Relaunch the same unmodified package in a second clean profile and match all retained hashes'],
  expected:'Two clean natural headless lifetimes of one package; exact resource and diagnostic bindings; valid sibling remains inspectable',
  reset:'Keep each altered PCK separately, use bounded children and isolated profiles, and preserve all process and packet receipts',
  evidence:['portable-delivery.json','pck-runtime-closure.json','clean-start-observation.json','clean-relaunch-observation.json',
    'missing-entry-item-evidence.json','interrupt-editor-import.command.json','result.json']
},portability);
