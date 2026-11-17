'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
function engine(){return require('./items.cjs').engine();}
const ART_FILES=['src/godot-game/assets/characters/bodies-a.png','src/godot-game/content/packs/bodies-a.json','production/phase4/bodies-a.zip','tools/phase4/cases/bodies-a.cjs'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file,67108864))}));}
const VERIFY=String.raw`import pathlib,sys,io,json,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);artifact=pathlib.Path(sys.argv[2]) if len(sys.argv)>2 else root
spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
data=m.read_archive(artifact/'production/phase4/bodies-a.zip');pack=m.loads(data['bodies-a.json']);atlas=Image.open(io.BytesIO(data['bodies-a.png'])).convert('RGBA')
m.need((artifact/'src/godot-game/assets/characters/bodies-a.png').read_bytes()==data['bodies-a.png'] and (artifact/'src/godot-game/content/packs/bodies-a.json').read_bytes()==data['bodies-a.json'],'BODY_PUBLICATION')
inventory=m.loads(data['members.json']);m.need(len(inventory)+1==len(data) and len({x['path'] for x in inventory})==len(inventory),'BODY_INVENTORY')
for row in inventory:m.need(row['path'] in data and row['sha256']==m.sha(data[row['path']]) and row['bytes']==len(data[row['path']]),'BODY_MEMBER_HASH')
replay_cache={};decode_cache={}
def largest(im):
 pixels=im.load();seen=set();best=[]
 for y in range(im.height):
  for x in range(im.width):
   if (x,y) in seen or pixels[x,y][3]==0:continue
   group=[];queue=[(x,y)];seen.add((x,y))
   while queue:
    a,b=queue.pop();group.append((a,b))
    for dy in [-1,0,1]:
     for dx in [-1,0,1]:
      c,d=a+dx,b+dy
      if 0<=c<im.width and 0<=d<im.height and (c,d) not in seen and pixels[c,d][3]:seen.add((c,d));queue.append((c,d))
   if len(group)>len(best):best=group
 out=Image.new('RGBA',im.size);target=out.load()
 for x,y in best:target[x,y]=pixels[x,y]
 return out
def replay(part,d):
 raw=d[part['origin']];origin=m.sha(raw)
 if origin not in decode_cache:decode_cache[origin]=Image.open(io.BytesIO(raw)).convert('RGBA')
 im=decode_cache[origin].copy();prefix=[]
 for op in part['operations']:
  prefix.append(op);cache_key=origin+'|'+json.dumps(prefix,sort_keys=True)
  if cache_key in replay_cache:im=replay_cache[cache_key].copy();continue
  if op['op']=='mask':
   m.need(op['alphaAtLeast']==240 and op['removeRedMatte']==[180,2,2],'BODY_MASK');im.putdata([(r,g,b,255) if a>=240 and not(r>180 and r>g*2 and r>b*2) else (0,0,0,0) for r,g,b,a in im.get_flattened_data()])
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'BODY_CROP');im=im.crop(op['box'])
  elif op['op']=='nearest':m.need(all(type(x)==int and 0<x<=48 for x in op['size']),'BODY_SIZE');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='largest-component':m.need(op['connectivity']==8,'BODY_COMPONENT_MASK');im=largest(im)
  elif op['op']=='place':
   m.need(op['canvas']==[48,40] and 0<=op['at'][0]<=48-im.width and 0<=op['at'][1]<=40-im.height,'BODY_REGISTRATION');out=Image.new('RGBA',(48,40));out.alpha_composite(im,op['at']);im=out
  elif op['op']=='partition':
   x,y,r,b=op['bounds'];m.need(0<=x<r<=48 and 0<=y<b<=40,'BODY_PARTITION');out=Image.new('RGBA',(48,40));out.paste(im.crop(op['bounds']),(x,y));im=out
   for box in op.get('exclude',[]):im.paste((0,0,0,0),box)
  else:raise ValueError('BODY_OPERATION')
  replay_cache[cache_key]=im.copy()
 return im
poses=['idle','walk-near','walk-far','run','windup','strike','recover','aim','hit','collapse-a','collapse-b','corpse','sit','gather','eat','carry']
def verify(p,d):
 m.need(p['schemaVersion']==p['version']==1 and p['id']=='bodies-a' and p['logicalPixelsPerWorldPixel']==1 and p['scaleDecision']=='D-13-pilot-1' and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'BODY_ID_SCALE')
 m.need(p['acceptance']=={'visual':'pending','humanApproval':None,'owner':'04-32/04-33','completedObligations':[]},'BODY_FALSE_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['pilotBundleSha256']==m.digest(root/'production/phase4/scale-pilot.zip'),'BODY_LINEAGE')
 m.need(p['recipe']['sha256']==m.sha(d[p['recipe']['member']]) and p['recipe']['Pillow']=='13.0.0','BODY_RECIPE')
 m.need(p['atlas']['size']==list(atlas.size) and p['atlas']['sha256']==m.sha(d['bodies-a.png']) and p['atlas']['path']=='res://assets/characters/bodies-a.png','BODY_ATLAS')
 m.need([x['id'] for x in p['bodies']]==['CH-01','CH-02','CH-03'] and len({x['silhouette'] for x in p['bodies']})==3,'BODY_ROSTER')
 for b in p['bodies']:
  m.need(b['tool']=='built-in image_gen' and b['masterSha256']==m.sha(d[b['master']]) and b['promptSha256']==m.sha(d[b['prompt']]) and b'D-13 LOW-RESOLUTION' in d[b['prompt']],'BODY_GENERATION')
 m.need(len(p['parts'])==193 and len({x['key'] for x in p['parts']})==193,'BODY_PARTS');parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  m.need(part['origin'] in d and len(part['operations'])==(5 if part['key']=='CH-02.local-head' else 6),'BODY_SOURCE');im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA');m.need(im.getbbox() and part['sha256']==m.sha(d[part['member']]) and part['rgbaSha256']==m.sha(im.tobytes()),'BODY_PIXELS')
  m.need(part['density']==1 and part['worldSize']==[48,40] and part['pivot']==[24,38] and set(im.getchannel('A').tobytes())<={0,255} and all(a or(r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'BODY_ALPHA_REGISTRATION')
  x,y,w,h=part['atlasRect'];m.need([w,h]==[48,40] and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'BODY_ATLAS_PIXELS')
  replayed=replay(part,d);m.need(im.tobytes()==replayed.tobytes(),'BODY_REPRODUCTION');images[part['key']]=im
 for b in p['bodies']:
  source=Image.open(io.BytesIO(d[b['master']])).convert('RGBA');m.need(source.getchannel('A').getextrema()==(0,255) and list(source.size)==b['sourceCanvas'],'BODY_ORIGINAL_ALPHA')
  m.need([x['id'] for x in b['poses']]==poses and b['bakedEquipment']==[] and b['logicalStandingHeight'] in [28,30],'BODY_POSES')
  for pose in b['poses']:
   m.need(pose['authored'] and pose['bakedEquipment']==[] and pose['persistent']==(pose['id']=='corpse') and pose['duration']==(0 if pose['persistent'] else .12),'BODY_AUTHORED')
   m.need(list(pose['parts'])==['full','head','upper','lower'] and all(k in parts for k in pose['parts'].values()),'BODY_COMPONENTS')
   combined=Image.new('RGBA',(48,40))
   for label in ['head','upper','lower']:combined.alpha_composite(images[pose['parts'][label]])
   m.need(combined.tobytes()==images[pose['parts']['full']].tobytes(),'BODY_COMPONENT_COVERAGE')
  m.need(len({images[b['id']+'.'+pose+'.full'].tobytes() for pose in poses})==16,'BODY_DUPLICATED_POSE')
 m.need(len({images[b['id']+'.idle.full'].getchannel('A').tobytes() for b in p['bodies']})==3,'BODY_RECOLORED_SILHOUETTE')
 m.need(len(p['sourceSuitability'])==57 and all(x['suitability'] for x in p['sourceSuitability']) and all(x in d for x in ['history/rejected-safety.json','history/rejected-covered-survivor.png','history/runner-original-clipped.png']),'BODY_SUITABILITY')
 for id in ['common.s0.2361','common.s0.7052','common.s0.10365','common.s0.7060','common.s0.7246','common.s0.10669']:
  row=m.loads(d['references/'+id+'.json']);m.need(row['source']['rawSha256']==m.sha(d['references/'+id+'.bin']) and row['pngSha256']==m.sha(d['references/'+id+'.png']),'BODY_DONOR_IDENTITY')
 m.need(p['headAdaptation']['sha256']==m.sha(d['adapted/local-shambler-head.png']) and 'CH-02.local-head' in parts,'BODY_LOCAL_ADAPTATION')
 return {'bodies':3,'poses':48,'parts':193,'members':len(d),'generatedSelected':3,'visualAcceptance':'pending04-32/33'}
result=verify(pack,data);rejections=[]
for name,code in [('missing-pose','BODY_POSES'),('baked-equipment','BODY_POSES'),('wrong-density','BODY_ID_SCALE'),('false-acceptance','BODY_FALSE_ACCEPTANCE'),('missing-master','BODY_GENERATION'),('missing-component','BODY_COMPONENTS'),('recolored-silhouette','BODY_ROSTER'),('altered-transform','BODY_MASK'),('wrong-source-hash','BODY_DONOR_IDENTITY')]:
 p=copy.deepcopy(pack);d=dict(data)
 if name=='missing-pose':p['bodies'][1]['poses'].pop()
 elif name=='baked-equipment':p['bodies'][0]['bakedEquipment']=['IT-11']
 elif name=='wrong-density':p['logicalPixelsPerWorldPixel']=2
 elif name=='false-acceptance':p['acceptance']['visual']='approved'
 elif name=='missing-master':d[p['bodies'][0]['master']]=b'changed'
 elif name=='missing-component':p['bodies'][0]['poses'][0]['parts']['head']='absent'
 elif name=='recolored-silhouette':p['bodies'][2]['silhouette']=p['bodies'][1]['silhouette']
 elif name=='altered-transform':p['parts'][0]['operations'][0]['alphaAtLeast']=128
 elif name=='wrong-source-hash':d['references/common.s0.2361.bin']=b'changed'
 try:verify(p,d);raise AssertionError('accepted '+name)
 except ValueError as e:m.need(str(e)==code,'BODY_CONTROL_DIAGNOSTIC '+name+' '+str(e));rejections.append({'id':name,'diagnostic':str(e)})
print(json.dumps({'actual':result,'negativeControls':rejections}))
`;
async function art(context){
 const before=fingerprint(ART_FILES);write(context.directory,'inputs.json',before);write(context.directory,'verify-art.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'bodies-art',PYTHON,[inside(context.directory,'verify-art.py'),ROOT]);
 const result=JSON.parse(child.stdout.trim());need(result.actual.bodies===3&&result.actual.poses===48&&result.actual.parts===193&&result.negativeControls.length===9,'BODY_ART_RESULTS');
 need(equal(before,fingerprint(ART_FILES)),'BODY_ART_SOURCE_CHANGED');write(context.directory,'art-observation.json',result);return result;
}
runner.registerCase('bodies.a-art',{group:'characters',title:'Three source-reproducible neutral bodies and authored registered pose/component masters',importance:'Critical',rationale:'Missing or baked body poses prevent reusable character/equipment coverage',needsGameWindow:false,runGroup:'G0',order:26,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-05','AC-18'],setup:'Retained built-in generated originals, exact K14/K18/source head references, complete production atlas and strict bounded ZIP inventory',actions:['Reproduce193 parts from exact retained originals and masks/crops/nearest transforms','Assert all48 neutral poses, disjoint reusable component coverage and three distinct alpha silhouettes','Reject nine source/pose/baked/density/component/acceptance/transform faults'],expected:'Three actual neutral body families with true alpha and exact source identity; final appearance judgement remains04-32/33',reset:'Fresh immutable source-side invocation; no game window',evidence:['inputs.json','bodies-art.command.json','art-observation.json','result.json']},art);
module.exports={ART_FILES,VERIFY,fingerprint,art};

function isolated(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT};}
const DRIVER=String.raw`extends SceneTree
var hub: Node
func choose(control: OptionButton, value: String) -> bool:
 for i in control.item_count:
  if control.get_item_text(i).begins_with(value):
   control.select(i)
   control.item_selected.emit(i)
   return true
 return false
func _initialize() -> void:
 call_deferred("run")
func run() -> void:
 root.size = Vector2i(1280,720)
 hub=load("res://inspection/Inspection.tscn").instantiate()
 root.add_child(hub)
 for i in range(160):
  await process_frame
  if hub.get_node("Shell/Toolbar/Cancel").disabled: break
 choose(hub.get_node("Shell/Workspace/Navigation/Category"),"Characters")
 var rows=hub.get_node("Shell/Workspace/Controls/Browser/Resources")
 for i in rows.item_count:
  if rows.get_item_text(i).begins_with("CH-01 · "):
   rows.select(i)
   rows.item_selected.emit(i)
   break
 await process_frame
 var preview=hub.get_node_or_null("Shell/Workspace/Preview/Surface/Current/Character")
 var coat: Variant = null
 if preview != null:
  choose(preview.get_node("Controls/Slots/torso/Select"),"IT-13")
  choose(preview.get_node("Controls/Slots/armor/Select"),"IT-20")
  choose(preview.get_node("Controls/Slots/armor/Select"),"Empty")
  for sprite in preview.get_node("Art").get_children():
   if sprite.has_meta("slot") and sprite.get_meta("slot") == "torso" and sprite.visible:
    coat=sprite.get_meta("item")
 var digest=HashingContext.new()
 digest.start(HashingContext.HASH_SHA256)
 digest.update(FileAccess.get_file_as_bytes("res://.godot/mono/temp/bin/Debug/KingdomZombies.dll"))
 var result={"pid":OS.get_process_id(),"nonce":OS.get_environment("CHARACTER_PROOF_NONCE"),"script":hub.get_script().resource_path,"assemblySha256":digest.finish().hex_encode(),"coatAfterArmorRemoval":coat,"characterControlsPresent":preview != null}
 var file=FileAccess.open("res://native-results.json",FileAccess.WRITE)
 file.store_string(JSON.stringify(result,"\t"))
 file.close()
 print("CHARACTER_NATIVE_COMPLETE "+result.nonce)
 hub.queue_free()
 await process_frame
 quit()
`;
async function nativeFit(context){
 const project=inside(context.directory,'native-character');fs.mkdirSync(project);
 const game=inside(ROOT,'src/godot-game');
 for(const item of ['project.godot','inspection','content','assets','.godot/mono/temp/bin/Debug'])fs.cpSync(path.join(game,item),path.join(project,item),{recursive:true});
 write(project,'probe.gd',Buffer.from(DRIVER));
 const tool=engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
 const archive=inside(ROOT,'.phase1/cache/'+pin.file);need(fs.statSync(archive).size<=268435456&&hash(fs.readFileSync(archive))===pin.sha256,'CHARACTER_ARCHIVE_PIN');
 const authentication=await runner.command(context,'character-native-tool-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',
  "Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip=[IO.Compression.ZipFile]::OpenRead('D:/Work/Games/KingdomZombies/.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip'); try { $entry=$zip.Entries | Where-Object { $_.FullName -ceq 'Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe' }; if(@($entry).Count -ne 1){throw 'ARCHIVE_ENTRY'}; $stream=$entry.Open(); try { $sha=[Security.Cryptography.SHA256]::Create(); [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } finally { $stream.Dispose() } } finally { $zip.Dispose() }"]);
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe'));
 need(hash(fs.readFileSync(executable))===authentication.stdout.trim(),'CHARACTER_NATIVE_PIN');
 const env={...isolated(),...runner.environment(context.directory),CHARACTER_PROOF_NONCE:context.nonce};
 await runner.command(context,'character-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 const child=await runner.command(context,'character-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('CHARACTER_NATIVE_COMPLETE '+context.nonce),'CHARACTER_NATIVE_MARKER');
 const result=document(project,'native-results.json');need(result.pid===child.pid&&result.nonce===context.nonce&&result.script==='res://inspection/InspectionRoot.cs','CHARACTER_NATIVE_IDENTITY');
 need(result.assemblySha256===hash(fs.readFileSync(path.join(game,'.godot/mono/temp/bin/Debug/KingdomZombies.dll'))),'CHARACTER_NATIVE_ASSEMBLY');
 write(context.directory,'native-observation.json',result);return result;
}
async function fit(context){
 const inputs=FIT_FILES;
 const before=fingerprint(inputs);write(context.directory,'inputs.json',before);
 const controls=fitControls(document(ROOT,'src/godot-game/content/animation/fits-a.json'));write(context.directory,'negative-controls.json',controls);
 const build=await runner.command(context,'character-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:isolated()});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'CHARACTER_CLEAN_BUILD');
 const native=await nativeFit(context),matrix=await nativeMatrix(context),dir=inside(context.directory,'compiled');fs.mkdirSync(dir);
 write(context.directory,'fit-observation.json',{...native,matrix:matrix.matrix,h13:matrix.h13,logicalHandsPreserved:matrix.logicalHandsPreserved,replacementRetired:matrix.replacementRetired,workerRejected:matrix.workerRejected,detachedRejected:matrix.detachedRejected,matrixPid:matrix.pid});
 const assembly=inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
 const discovery=await runner.command(context,'character-discovery','dotnet',[assembly,'--list-tests','--filter-class','*CharacterFitTests','--no-ansi'],{env:isolated()});
 const names=[...discovery.stdout.matchAll(/CharacterFitTests\.([A-Za-z0-9_]+)/g)].map(x=>x[1]);need(names.length===10&&new Set(names).size===5,'CHARACTER_COMPILED_DISCOVERY');
 await runner.command(context,'character-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-class','*CharacterFitTests','--report-xunit-trx','--report-xunit-trx-filename','tests.trx','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',dir,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:{...isolated(),CHARACTER_NATIVE_REPORT:inside(context.directory,'fit-observation.json')}});
 const report=document(dir,'tests.json');need(report.results.summary.tests===10&&report.results.summary.passed===10&&report.results.summary.failed===0&&report.results.summary.skipped===0,'CHARACTER_COMPILED_RESULTS');
 need(equal(before,fingerprint(inputs)),'CHARACTER_SOURCE_CHANGED');return {hub:native,matrix:{pid:matrix.pid,tuples:matrix.matrix.tuples,observations:matrix.outcomes.length,h13:matrix.h13.passed},compiled:10,negativeControls:controls.length,scope:'Actual compiled source/native observations only; appearance and full-pack adoption remain04-32/33 and04-23'};
}
runner.registerCase('bodies.a-fit',{group:'characters',title:'Actual character gear fitting, selected coat restoration and logical hands',importance:'Critical',rationale:'Incorrect replacement loses selected equipment and character identity',needsGameWindow:false,runGroup:'G0',order:27,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-05','AC-18','AC-19'],setup:'Strict compiled production hub, real body/item atlases and saved fit definitions',actions:['Select CH-01 through actual category/resource controls','Select coat, cover with armor and remove armor','Assert actual exposed torso is the selected IT-13 coat'],expected:'Current composition retains selected equipment and clears replaced layers; actual appearance remains04-32/33',reset:'Fresh owned headless native hub and report; no game window',evidence:['inputs.json','native-observation.json','matrix-observation.json','fit-observation.json','character-strict-build.command.json','character-compiled.command.json','result.json']},fit);
Object.assign(module.exports,{isolated,DRIVER,nativeFit,fit});

const FIT_FILES=[...ART_FILES,"src/godot-game/content/animation/bow.json","src/godot-game/content/packs/common.json","src/godot-game/assets/common/donor-kit.png","src/godot-game/content/animation/locomotion-melee.json","src/godot-game/content/config/items.json","src/godot-game/content/config/combat.json","src/core/CharacterPresentation.cs",'src/godot-game/content/animation/fits-a.json','src/godot-game/inspection/CharacterPreview.cs','src/godot-game/inspection/InspectionRoot.cs','tests/phase1/CharacterFitTests.cs',...['items-01-10','items-11-24'].flatMap(x=>['src/godot-game/content/packs/'+x+'.json','src/godot-game/assets/equipment/'+x+'.png']),'src/godot-game/inspection/Inspection.tscn','tools/phase1/toolchain.json',...['b','c'].flatMap(x=>['src/godot-game/content/packs/bodies-'+x+'.json','src/godot-game/content/animation/fits-'+x+'.json','src/godot-game/assets/characters/bodies-'+x+'.png'])];
const FIT_HARNESS=String.raw`using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
namespace KingdomZombies.CharacterProof;
public sealed partial class CharacterHarness : Node
{
 private Assembly _assembly=null!; private Assembly _core=null!; private Type _type=null!; private readonly List<object> _observations=new();private bool _failed;private int _tuples;private int _tupleFailures;private int _h13;
 private object Invoke(object owner,string method,params object[] args){return _type.GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(owner,args)!;}
 private JsonElement Read(string file,string identity){return (JsonElement)_type.GetMethod("ReadDefinition",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{FileAccess.GetFileAsBytes(file),identity})!;}
 private object Create(Node2D root){var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;return Activator.CreateInstance(_type,BindingFlags.Instance|BindingFlags.NonPublic,null,new object[]{root,gateway,Read("res://content/animation/fits-a.json","fits-a"),Read("res://content/packs/bodies-a.json","bodies-a"),Read("res://content/packs/items-01-10.json","items-01-10"),Read("res://content/packs/items-11-24.json","items-11-24"),"CH-01"},null)!;}
 private void Check(string id,bool pass,object detail){_observations.Add(new{id,pass,detail});_failed|=!pass;}
 private static void Select(Node2D root,string id,int index){var c=root.GetNode<OptionButton>("Controls/Slots/"+id+"/Select");c.Select(index);c.EmitSignal(OptionButton.SignalName.ItemSelected,index);}
 private static void Item(Node2D root,string id,string value){var c=root.GetNode<OptionButton>("Controls/Slots/"+id+"/Select");for(int i=0;i<c.ItemCount;i++){if(c.GetItemText(i).StartsWith(value,StringComparison.Ordinal)){Select(root,id,i);return;}}throw new InvalidOperationException("CONTROL_ITEM "+id+" "+value);}
 private static Sprite2D[] Gear(Node2D root,string slot){return root.GetNode("Art").GetChildren().OfType<Sprite2D>().Where(s=>s.HasMeta("slot")&&StringComparer.Ordinal.Equals(s.GetMeta("slot").AsString(),slot)).ToArray();}
 private static string Signature(Node2D root){return string.Join("|",root.GetNode("Art").GetChildren().OfType<Sprite2D>().Select(s=>s.Name+":"+s.Position+":"+s.GetMeta("part").AsString()));}
 private static bool Nonempty(Sprite2D sprite){using var im=sprite.Texture.GetImage();for(int y=0;y<im.GetHeight();y++){for(int x=0;x<im.GetWidth();x++){if(im.GetPixel(x,y).A>0)return true;}}return false;}
 private static bool ExactPixels(Sprite2D sprite,JsonElement[] packs,string item){var pack=packs.Single(p=>p.GetProperty("items").EnumerateArray().Any(i=>StringComparer.Ordinal.Equals(i.GetProperty("id").GetString(),item)));string key=sprite.GetMeta("part").AsString();var part=pack.GetProperty("parts").EnumerateArray().Single(p=>StringComparer.Ordinal.Equals(p.GetProperty("key").GetString(),key));var r=part.GetProperty("atlasRect");using var source=new Image();if(source.LoadPngFromBuffer(FileAccess.GetFileAsBytes("res://"+pack.GetProperty("atlas").GetProperty("path").GetString()))!=Error.Ok)throw new InvalidOperationException("EXPECTED_ATLAS_DECODE");using var im=source.GetRegion(new Rect2I(r[0].GetInt32(),r[1].GetInt32(),r[2].GetInt32(),r[3].GetInt32()));using var actual=sprite.Texture.GetImage();im.Resize(actual.GetWidth(),actual.GetHeight(),Image.Interpolation.Nearest);for(int y=0;y<im.GetHeight();y++){for(int x=0;x<im.GetWidth();x++){var a=actual.GetPixel(x,y);var b=im.GetPixel(x,y);if(a.A!=b.A||(a.A!=0&&(a.R!=b.R||a.G!=b.G||a.B!=b.B)))return false;}}return true;}
 public override async void _Ready(){try{await Run();}catch(Exception e){Check("fixture-exception",false,e.ToString());}var result=new{pid=OS.GetProcessId(),nonce=OS.GetEnvironment("CHARACTER_PROOF_NONCE"),assemblySha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll")))),coreSha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(_core.Location))),coreOwningAlc=ReferenceEquals(AssemblyLoadContext.GetLoadContext(_core),AssemblyLoadContext.GetLoadContext(GetType().Assembly)),owningAlc=ReferenceEquals(AssemblyLoadContext.GetLoadContext(_assembly),AssemblyLoadContext.GetLoadContext(GetType().Assembly)),outcomes=_observations,failed=_failed,matrix=new{tuples=_tuples,failed=_tupleFailures},h13=new{passed=_h13},logicalHandsPreserved=_hands,replacementRetired=_replacement,workerRejected=_worker,detachedRejected=_detached};using(var file=FileAccess.Open("res://native-results.json",FileAccess.ModeFlags.Write)){file.StoreString(JsonSerializer.Serialize(result));}GD.Print("CHARACTER_MATRIX_COMPLETE "+result.nonce+" "+(_failed?"FAIL":"PASS"));GetTree().Quit(_failed?1:0);}
 private bool _hands;private bool _replacement;private bool _worker;private bool _detached;
 private async Task Run(){
  GetTree().Root.Size=new Vector2I(1280,720);var context=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!;_core=context.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.Core.dll"));_assembly=context.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));_type=_assembly.GetType("KingdomZombies.Inspection.CharacterPreview",true)!;Check("actual-internal-production-type",!_type.IsPublic&&StringComparer.Ordinal.Equals(_type.Assembly.GetName().Name,"KingdomZombies"),_type.FullName!);
  var primary=new Node2D{Name="Primary"};AddChild(primary);var preview=Create(primary);Invoke(preview,"Attach");var other=new Node2D{Name="Nearby",Position=new Vector2(18,0)};AddChild(other);var neighbor=Create(other);Invoke(neighbor,"Attach");
  var packs=new[]{Read("res://content/packs/items-01-10.json","items-01-10"),Read("res://content/packs/items-11-24.json","items-11-24")};
  var slots=new Dictionary<string,string[]>(StringComparer.Ordinal){{"headwear",new[]{"IT-14","IT-18"}},{"face",new[]{"IT-15"}},{"torso",new[]{"IT-11","IT-13"}},{"legs",new[]{"IT-12"}},{"armor",new[]{"IT-19","IT-20"}},{"gloves",new[]{"IT-16"}},{"boots",new[]{"IT-17"}},{"backpack",new[]{"IT-23","IT-24"}},{"belt",new[]{"IT-21"}},{"left",Enumerable.Range(2,9).Select(i=>"IT-"+i.ToString("00",System.Globalization.CultureInfo.InvariantCulture)).ToArray()},{"right",Enumerable.Range(2,9).Select(i=>"IT-"+i.ToString("00",System.Globalization.CultureInfo.InvariantCulture)).ToArray()}};
  Check("eleven-real-labeled-slots",slots.All(s=>primary.HasNode("Controls/Slots/"+s.Key+"/Label")&&primary.GetNode<OptionButton>("Controls/Slots/"+s.Key+"/Select").ItemCount==s.Value.Length+1),slots.Keys.ToArray());
  for(int b=0;b<3;b++){Select(primary,"Body",b);for(int p=0;p<16;p++){Select(primary,"Pose",p);for(int f=0;f<2;f++){Select(primary,"Facing",f);foreach(var slot in slots){foreach(var item in slot.Value){Item(primary,slot.Key,item);var sprites=Gear(primary,slot.Key);bool exact=sprites.Length>0&&sprites.All(s=>StringComparer.Ordinal.Equals(s.GetMeta("item").AsString(),item)&&s.TextureFilter==CanvasItem.TextureFilterEnum.Nearest&&Nonempty(s)&&ExactPixels(s,packs,item));if(StringComparer.Ordinal.Equals(slot.Key,"left")||StringComparer.Ordinal.Equals(slot.Key,"right"))exact&=sprites.Length==1&&StringComparer.Ordinal.Equals(sprites[0].GetMeta("logical_hand").AsString(),slot.Key);_tuples++;if(!exact)_tupleFailures++;Check("fit-"+b+"-"+p+"-"+f+"-"+slot.Key+"-"+item,exact,new{parts=sprites.Select(s=>s.GetMeta("part").AsString()).ToArray(),positions=sprites.Select(s=>s.Position.ToString()).ToArray()});Item(primary,slot.Key,"Empty");Check("empty-"+_tuples,Gear(primary,slot.Key).Length==0,"all previous children removed synchronously");} }await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}}}
  Item(primary,"left","IT-03");Item(primary,"right","IT-06");Select(primary,"Facing",0);float left=Gear(primary,"left")[0].Position.X;Select(primary,"Facing",1);_hands=StringComparer.Ordinal.Equals(Gear(primary,"left")[0].GetMeta("item").AsString(),"IT-03")&&StringComparer.Ordinal.Equals(Gear(primary,"right")[0].GetMeta("item").AsString(),"IT-06")&&Math.Abs(left+Gear(primary,"left")[0].Position.X-48)<0.01;Check("logical-hands-reflect-without-swapping",_hands,"left knife/right bat");
  Item(primary,"torso","IT-13");Item(primary,"armor","IT-20");Item(primary,"armor","Empty");_replacement=Gear(primary,"torso").All(s=>StringComparer.Ordinal.Equals(s.GetMeta("item").AsString(),"IT-13"))&&Gear(primary,"armor").Length==0;Item(primary,"torso","IT-11");_replacement&=Gear(primary,"torso").All(s=>StringComparer.Ordinal.Equals(s.GetMeta("item").AsString(),"IT-11"));Check("selected-coat-and-replacement",_replacement,"selected coat survives armor; shirt replacement removes every coat component");
  Item(other,"torso","IT-13");Item(other,"backpack","IT-24");string independent=Signature(other);
  for(int i=0;i<12;i++){Select(primary,"Body",i%3);Select(primary,"Pose",new[]{0,3,5,11}[i%4]);Select(primary,"Facing",i%2);Item(primary,"torso",i%2==0?"IT-13":"IT-11");Item(primary,"armor",i%2==0?"IT-19":"IT-20");Item(primary,"belt","IT-21");Item(primary,"backpack",i%2==0?"IT-24":"IT-23");Item(primary,"gloves","IT-16");Item(primary,"boots","IT-17");Item(primary,"headwear",i%2==0?"IT-14":"IT-18");var armor=Gear(primary,"armor");var torso=Gear(primary,"torso");bool pass=armor.Any()&&torso.Any()&&armor.Min(s=>s.ZIndex)>torso.Min(s=>s.ZIndex)&&slots.Where(s=>!StringComparer.Ordinal.Equals(s.Key,"face")&&!StringComparer.Ordinal.Equals(s.Key,"legs")).All(s=>Gear(primary,s.Key).Length>0)&&StringComparer.Ordinal.Equals(independent,Signature(other))&&StringComparer.Ordinal.Equals(primary.GetMeta("body").AsString(),"CH-0"+(i%3+1));foreach(var slot in slots.Keys)pass&=Gear(primary,slot).All(s=>Nonempty(s));Check("H13-independent-"+i,pass,new{body=primary.GetMeta("body").AsString(),pose=primary.GetMeta("pose").AsString(),nearby=other.GetMeta("body").AsString(),parts=primary.GetNode("Art").GetChildCount()});if(pass)_h13++;}
  Select(primary,"Condition",1);Check("real-broken-master",Gear(primary,"armor").Any(s=>s.GetMeta("part").AsString().Contains("broken",StringComparison.Ordinal)),Gear(primary,"armor").Select(s=>s.GetMeta("part").AsString()).ToArray());
  Item(primary,"right","Empty");Item(primary,"left","IT-07");var opposite=primary.GetNode<OptionButton>("Controls/Slots/right/Select");Check("bow-illegal-new-combinations-disabled",Enumerable.Range(1,opposite.ItemCount-1).All(i=>opposite.IsItemDisabled(i)==!StringComparer.Ordinal.Equals(opposite.GetItemText(i),"IT-02")),"only arrows or empty opposite bow");
  await Task.Run(()=>{try{Invoke(preview,"Changed",0L);}catch(TargetInvocationException e){_worker=StringComparer.Ordinal.Equals(e.InnerException?.Message,"THREAD_ACCESS");}});Check("worker-before-native-rejected",_worker,"captured gateway before any native work");Invoke(preview,"Detach");try{Invoke(preview,"Changed",0L);}catch(TargetInvocationException e){_detached=StringComparer.Ordinal.Equals(e.InnerException?.Message,"CHARACTER_DETACHED");}Check("detached-callback-rejected",_detached,"acceptance closed and signals disconnected");Invoke(neighbor,"Detach");primary.QueueFree();other.QueueFree();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
 }
}
`;
async function nativeMatrix(context){
 const project=inside(context.directory,'character-matrix');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(x=>fs.statSync(path.join(original,x)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'production-assemblies.json',closure);
 write(project,'CharacterProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>CharacterProof</AssemblyName><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors><EnableDynamicLoading>true</EnableDynamicLoading></PropertyGroup></Project>'));
 write(project,'CharacterHarness.cs',Buffer.from(FIT_HARNESS));write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="CharacterProof"\n[dotnet]\nproject/assembly_name="CharacterProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of FIT_FILES.filter(x=>x.startsWith('src/godot-game/')&&!x.endsWith('.cs')&&!x.endsWith('.tscn')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n var proof=load("res://CharacterHarness.cs").new()\n root.add_child(proof)\n'));
 const build=await runner.command(context,'character-harness-build','dotnet',['build',inside(project,'CharacterProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'CHARACTER_HARNESS_CLEAN_BUILD');
 const executable=path.join(path.dirname(engine().executable),document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'].executable.replace('_console.exe','.exe')),env={...isolated(),CHARACTER_PROOF_NONCE:context.nonce};
 await runner.command(context,'character-matrix-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});const child=await runner.command(context,'character-matrix-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('CHARACTER_MATRIX_COMPLETE '+context.nonce+' PASS'),'CHARACTER_MATRIX_MARKER');const result=document(project,'native-results.json');need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.every(x=>x.pass)&&result.matrix.tuples===2976&&result.matrix.failed===0&&result.h13.passed===12,'CHARACTER_MATRIX_RESULTS');need(result.assemblySha256===closure.find(x=>x.file==='KingdomZombies.dll').sha256&&result.coreSha256===closure.find(x=>x.file==='KingdomZombies.Core.dll').sha256&&result.owningAlc&&result.coreOwningAlc,'CHARACTER_MATRIX_ASSEMBLY');write(context.directory,'matrix-observation.json',result);return result;
}
Object.assign(module.exports,{FIT_FILES,FIT_HARNESS,nativeMatrix});

const SLOT_ITEMS=[['headwear',['IT-14','IT-18']],['face',['IT-15']],['torso',['IT-11','IT-13']],['legs',['IT-12']],['armor',['IT-19','IT-20']],['gloves',['IT-16']],['boots',['IT-17']],['backpack',['IT-23','IT-24']],['belt',['IT-21']],['left',Array.from({length:9},(_,i)=>'IT-'+String(i+2).padStart(2,'0'))],['right',Array.from({length:9},(_,i)=>'IT-'+String(i+2).padStart(2,'0'))]];
function validateFits(p){
 need(p.id==='fits-a'&&p.schemaVersion===1&&p.version===1&&p.logicalPixelsPerWorldPixel===1,'FIT_ID_DENSITY');
 need(equal(p.acceptance,{visual:'pending',owner:'04-32/04-33',humanApproval:null,completedObligations:[]}),'FIT_FALSE_ACCEPTANCE');
 need(equal(p.slots.map(x=>[x.id,x.items]),SLOT_ITEMS)&&p.slots.every(x=>typeof x.label==='string'&&x.label.length>0),'FIT_SLOTS');
 need(equal(p.bindings.map(x=>x.path),['content/packs/bodies-a.json','content/packs/items-01-10.json','content/packs/items-11-24.json'])&&p.bindings.every(x=>hash(bytes(ROOT,'src/godot-game/'+x.path))===x.sha256),'FIT_SOURCE_BINDING');
 need(equal(p.facings,[-1,1])&&p.layerRules.logicalHandsNeverSwap===true&&p.layerRules.selectedTorsoSurvivesArmor===true&&p.layerRules.clothSway===false&&p.layerRules.imageSampling==='nearest'&&p.layerRules.replacement==='retire-all-old-layers-before-project','FIT_LAYER_RULES');
 const poseIds=['idle','walk-near','walk-far','run','windup','strike','recover','aim','hit','collapse-a','collapse-b','corpse','sit','gather','eat','carry'];
 need(equal(p.bodies.map(x=>x.id),['CH-01','CH-02','CH-03'])&&p.bodies.every(x=>equal(x.poses.map(y=>y.id),poseIds)),'FIT_BODY_POSES');
 const anchors=['head','face','torso','hips','backpack','leftShoulder','rightShoulder','leftHand','rightHand','leftFoot','rightFoot'];
 for(const body of p.bodies)for(const pose of body.poses)need(equal(Object.keys(pose.anchors),anchors)&&Object.values(pose.anchors).every(a=>a.length===2&&a.every(n=>Number.isInteger(n)&&n>=0&&n<=48))&&pose.persistent===(pose.id==='corpse')&&pose.torsoLength>0&&pose.torsoLength<32,'FIT_ANCHORS');
 const expected=[...new Set(SLOT_ITEMS.flatMap(x=>x[1]))].sort();need(equal(p.items.map(x=>x.id).sort(),expected),'FIT_ITEM_ROSTER');
 for(const item of p.items){const pack=document(ROOT,'src/godot-game/content/packs/'+(Number(item.id.slice(3))<=10?'items-01-10':'items-11-24')+'.json'),parts=new Set(pack.parts.map(x=>x.key));
  need(item.layers.length>0&&item.layers.length<=7&&item.atlas===pack.atlas.path,'FIT_ITEM_LAYERS');
  for(const layer of item.layers)need(parts.has(layer.part)&&[...anchors,'hand'].includes(layer.anchor)&&(layer.end===null||anchors.includes(layer.end))&&layer.size.length===2&&layer.size.every(n=>Number.isInteger(n)&&n>0&&n<=32)&&['front','rear','both'].includes(layer.surface)&&Number.isInteger(layer.z)&&layer.z>=0&&layer.z<=100,'FIT_PART_TRANSFORM');
 }
 need(p.heldOutReview.id==='H13'&&p.heldOutReview.owner==='04-32'&&equal(p.heldOutReview.required,['overlap','nearby-body-identity','both-facing','replacement','corpse']),'FIT_H13');
}
function fitControls(pack){validateFits(pack);const rows=[];for(const [id,mutate,code]of[
 ['density',p=>p.logicalPixelsPerWorldPixel=2,'FIT_ID_DENSITY'],['false-acceptance',p=>p.acceptance.visual='approved','FIT_FALSE_ACCEPTANCE'],['slot-identity',p=>p.slots[0].id='hat','FIT_SLOTS'],['missing-source',p=>p.bindings[0].sha256='0'.repeat(64),'FIT_SOURCE_BINDING'],['logical-hand-swap',p=>p.layerRules.logicalHandsNeverSwap=false,'FIT_LAYER_RULES'],['default-shirt',p=>p.layerRules.selectedTorsoSurvivesArmor=false,'FIT_LAYER_RULES'],['sway',p=>p.layerRules.clothSway=true,'FIT_LAYER_RULES'],['linear-filter',p=>p.layerRules.imageSampling='linear','FIT_LAYER_RULES'],['missing-pose',p=>p.bodies[0].poses.pop(),'FIT_BODY_POSES'],['invalid-anchor',p=>p.bodies[1].poses[0].anchors.head=[999,2],'FIT_ANCHORS'],['missing-item',p=>p.items.pop(),'FIT_ITEM_ROSTER'],['empty-fit',p=>p.items[0].layers=[],'FIT_ITEM_LAYERS'],['wrong-part',p=>p.items[0].layers[0].part='absent','FIT_PART_TRANSFORM'],['missing-H13',p=>p.heldOutReview.required.pop(),'FIT_H13']]){const p=structuredClone(pack);mutate(p);let error;try{validateFits(p);}catch(e){error=e.message;}need(error?.startsWith(code),'FIT_NEGATIVE_DIAGNOSTIC',id+':'+error);rows.push({id,diagnostic:error});}return rows;}
Object.assign(module.exports,{validateFits,fitControls});
