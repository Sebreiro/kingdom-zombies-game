'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {PYTHON,engine}=require('./items.cjs');
const ART_FILES=['src/godot-game/assets/buildings/main-tiers.png','src/godot-game/content/packs/main-buildings.json','production/phase4/main-buildings.zip','tools/phase4/cases/main-buildings.cjs'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file,67108864))}));}
const VERIFY=String.raw`import sys,pathlib,json,zipfile,io,hashlib,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
data=m.read_archive(root/'production/phase4/main-buildings.zip');pack=m.loads(data['main-buildings.json']);atlas=Image.open(io.BytesIO(data['main-tiers.png'])).convert('RGBA')
m.need(data['main-buildings.json']==(root/'src/godot-game/content/packs/main-buildings.json').read_bytes(),'BUILDING_PACK_BYTES');m.need(data['main-tiers.png']==(root/'src/godot-game/assets/buildings/main-tiers.png').read_bytes(),'BUILDING_ATLAS_BYTES')
inventory=m.loads(data['members.json']);m.need(len(inventory)+1==len(data) and len({x['path'] for x in inventory})==len(inventory),'BUILDING_INVENTORY')
for row in inventory:m.need(row['path'] in data and row['bytes']==len(data[row['path']]) and row['sha256']==m.sha(data[row['path']]),'BUILDING_MEMBER_HASH')
def binary(im):
 im=im.convert('RGBA');im.putalpha(im.getchannel('A').point(lambda a:255 if a>=128 else 0));im.putdata([(r,g,b,a) if a else (0,0,0,0) for r,g,b,a in im.get_flattened_data()]);return im
def replay(part,d):
 m.need(part['origin'] in d,'BUILDING_ORIGIN');im=Image.open(io.BytesIO(d[part['origin']])).convert('RGBA')
 for op in part['operations']:
  if op['op']=='alpha-threshold':m.need(op['value']==128,'BUILDING_THRESHOLD');im=binary(im)
  elif op['op']=='crop':
   x,y,r,b=op['box'];m.need(0<=x<r<=im.width and 0<=y<b<=im.height,'BUILDING_CROP');im=im.crop((x,y,r,b))
  elif op['op']=='nearest':m.need(len(op['size'])==2 and all(type(x)==int and 0<x<=256 for x in op['size']),'BUILDING_NEAREST');im=im.resize(op['size'],Image.Resampling.NEAREST)
  elif op['op']=='damage':
   m.need(len(op['cuts'])<=8 and len(op['bends'])<=2,'BUILDING_DAMAGE_LIMIT')
   for box in op['cuts']:m.need(0<=box[0]<box[2]<=im.width and 0<=box[1]<box[3]<=im.height,'BUILDING_DAMAGE_BOUNDS');im.paste((0,0,0,0),box)
   for box,at in op['bends']:patch=im.crop(box);im.paste((0,0,0,0),box);im.alpha_composite(patch,at)
  elif op['op']=='rubble':
   original=im;m.need(op['size']==[im.width,16] and len(op['pieces'])==3,'BUILDING_RUBBLE');im=Image.new('RGBA',tuple(op['size']))
   for box,at in op['pieces']:im.alpha_composite(original.crop(box),at)
  else:raise ValueError('BUILDING_OPERATION')
 return im
def verify(p,d):
 m.need(p['id']=='main-buildings' and p['schemaVersion']==1 and p['version']==1,'BUILDING_ID')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'BUILDING_STYLE')
 m.need(p['acceptance']=={'visual':'pending','owner':'04-32/04-33','humanApproval':None,'completedObligations':[]},'BUILDING_ACCEPTANCE')
 m.need(p['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['pilotBundleSha256']==m.digest(root/'production/phase4/scale-pilot.zip'),'BUILDING_LINEAGE')
 m.need(p['atlas']['sha256']==m.sha(d['main-tiers.png']) and p['atlas']['size']==list(atlas.size),'BUILDING_ATLAS')
 m.need([x['tier'] for x in p['tiers']]==[1,2,3] and [x['id'] for x in p['tiers']]==['BL-01','BL-02','BL-03'],'BUILDING_TIERS')
 m.need([x['material'] for x in p['tiers']]==['campfire/rough-boxes','wood','metal-reinforced-wood'],'BUILDING_MATERIALS')
 m.need(len(p['parts'])==47 and len({x['key'] for x in p['parts']})==47,'BUILDING_PARTS');parts={x['key']:x for x in p['parts']};images={}
 for part in p['parts']:
  m.need(part['member'] in d and part['sha256']==m.sha(d[part['member']]),'BUILDING_PART_HASH');im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA')
  m.need(im.getbbox() and part['density']==1 and part['worldSize']==list(im.size) and part['image']['rgbaSha256']==m.sha(im.tobytes()),'BUILDING_PART_IMAGE')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'BUILDING_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=atlas.width-w and 0<=y<=atlas.height-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'BUILDING_ATLAS_PIXELS')
  m.need(replay(part,d).tobytes()==im.tobytes(),'BUILDING_REPLAY');images[part['key']]=im
 for tier in p['tiers']:
  m.need(tier['bakedGuardCount']==0 and tier['repairOverlay']=='scaffold' and tier['upgradeOverlay']=='scaffold','BUILDING_COMPONENTS')
  m.need(list(tier['states'])==['intact','damage-light','damage-heavy','defeated'] and all(k in parts for k in tier['states'].values()),'BUILDING_STATES')
  material=['boxes','wood','metal'][tier['tier']-1];m.need(tier['sourceComposition']['facade']==material,'BUILDING_SILHOUETTE')
  for state,key in tier['states'].items():
   canvas=Image.new('RGBA',(160,88));asset=material if state=='intact' else material+'-'+('rubble' if state=='defeated' else state);pos=(4,68) if tier['tier']==1 else (16,16)
   if state=='defeated':pos=(16,72)
   canvas.alpha_composite(images[asset],pos)
   if state=='defeated':canvas.alpha_composite(images['wood-fragment'],(65,65))
   else:
    canvas.alpha_composite(images['fire'],(63,72));canvas.alpha_composite(images['chest-closed'],(130,68))
    if tier['tier']>1:canvas.alpha_composite(images['boxes'],(2,68))
   m.need(canvas.tobytes()==images[key].tobytes(),'BUILDING_COMPOSITION')
  a=images[material];b=images[material+'-damage-light'];c=images[material+'-damage-heavy'];m.need(a.tobytes()!=b.tobytes()!=c.tobytes() and sum(bool(x) for x in a.getchannel('A').tobytes())>sum(bool(x) for x in b.getchannel('A').tobytes())>sum(bool(x) for x in c.getchannel('A').tobytes()),'BUILDING_DAMAGE_PROGRESSION')
  m.need(tier['fragments']==['wood-fragment']+(['metal-fragment'] if tier['tier']==3 else []),'BUILDING_FRAGMENTS')
 m.need(p['recipe']['sha256']==m.sha(d['recipes/build-main.py']),'BUILDING_RECIPE')
 for generated in p['sources']['generated']:
  m.need(generated['tool']=='built-in image_gen' and generated['master'] in d and generated['sha256']==m.sha(d[generated['master']]) and generated['promptSha256']==m.sha(d[generated['prompt']]),'BUILDING_GENERATION')
  m.need(b'D-13 LOW-RESOLUTION' in d[generated['prompt']] and b'transparent' in d[generated['prompt']],'BUILDING_PROMPT')
  original=Image.open(io.BytesIO(d[generated['master']])).convert('RGBA');m.need(original.getchannel('A').getextrema()==(0,255),'BUILDING_MASTER_ALPHA')
 guard=m.loads(d['sources/guard-shoot.json']);m.need(guard['id']=='common.clip.783' and [x['time'] for x in guard['layers'][0]['keys']]==[0,0.125,0.25,0.375,0.5] and guard['stop']==0.625,'BUILDING_GUARD_TIMING')
 return {'parts':len(parts),'tiers':3,'members':len(d),'guardFrames':len(guard['mapping'])}
result=verify(pack,data);controls=[]
def reject(name,mutate,code):
 candidate=copy.deepcopy(pack);d=dict(data);mutate(candidate,d)
 try:verify(candidate,d)
 except (ValueError,KeyError,AssertionError) as error:m.need(code in str(error),'BUILDING_CONTROL_REASON '+name+' '+str(error));controls.append({'id':name,'rejected':True,'diagnostic':str(error)})
 else:raise ValueError('BUILDING_CONTROL_FALSE_PASS '+name)
reject('first-tier-house',lambda p,d:p['tiers'][0].update(material='wood'),'BUILDING_MATERIALS')
reject('third-tier-stone',lambda p,d:p['tiers'][2].update(material='stone'),'BUILDING_MATERIALS')
reject('baked-guard',lambda p,d:p['tiers'][2].update(bakedGuardCount=2),'BUILDING_COMPONENTS')
reject('missing-defeat',lambda p,d:p['tiers'][1]['states'].pop('defeated'),'BUILDING_STATES')
reject('missing-metal-fragment',lambda p,d:p['tiers'][2].update(fragments=['wood-fragment']),'BUILDING_FRAGMENTS')
reject('premature-house-silhouette',lambda p,d:p['tiers'][0]['sourceComposition'].update(facade='wood'),'BUILDING_SILHOUETTE')
reject('missing-master',lambda p,d:d.pop('masters/metal.png'),'BUILDING_ORIGIN')
reject('fake-generation',lambda p,d:p['sources']['generated'][0].update(tool='programmatic stand-in'),'BUILDING_GENERATION')
reject('bad-density',lambda p,d:p.update(logicalPixelsPerWorldPixel=2),'BUILDING_STYLE')
reject('false-approval',lambda p,d:p['acceptance'].update(visual='approved'),'BUILDING_ACCEPTANCE')
reject('atlas-shift',lambda p,d:p['parts'][0]['atlasRect'].__setitem__(0,p['parts'][0]['atlasRect'][0]+1),'BUILDING_ATLAS_PIXELS')
reject('smoothing-operation',lambda p,d:p['parts'][0]['operations'][-1].update(op='bilinear'),'BUILDING_OPERATION')
print(json.dumps({**result,'expectedRejected':len(controls),'controls':controls}))
`;
async function art(context){
 const before=fingerprint(ART_FILES);write(context.directory,'inputs.json',before);write(context.directory,'verify-buildings.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'buildings-art',PYTHON,['-B',inside(context.directory,'verify-buildings.py'),ROOT]);const result=JSON.parse(child.stdout);
 need(result.tiers===3&&result.parts===47&&result.expectedRejected===12,'BUILDING_ART_RESULT');write(context.directory,'negative-controls.json',result.controls);
 need(equal(before,fingerprint(ART_FILES)),'BUILDING_SOURCE_CHANGED');return {...result,scope:'Source art/provenance only; guarded native state in buildings.main-state, hub adoption04-23, actual appearance04-32/33 pending'};
}
runner.registerCase('buildings.main-art',{group:'buildings',title:'Three actual campfire, wooden and metal main tiers with retained lifecycle art',importance:'Critical',
 rationale:'Wrong main silhouettes, missing lifecycle material or false source lineage invalidates camp resource production',needsGameWindow:false,runGroup:'G0',order:24,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08'],setup:'Tracked portable main-building ZIP/atlas/shard, installed pinned Python/Pillow; recovered roots are unnecessary',
 actions:['Authenticate exact bundle/member/project bytes','Reproduce47 actual material/lifecycle/guard inputs from retained selected/generated art','Independently reconstruct three main silhouettes and12 lifecycle compositions','Reject12 independent semantic faults'],
 expected:'Three exact materials, no baked guards, binary alpha, localized increasing damage, reusable repair/upgrade/ruin inputs and12 rejected controls',reset:'Fresh owned immutable source invocation; no real game window',
 evidence:['inputs.json','buildings-art.command.json','negative-controls.json','result.json']},art);
const STATE_FILES=[...ART_FILES,'src/godot-game/inspection/BuildingPreview.cs','src/godot-game/content/world/main-assemblies.json','src/godot-game/content/packs/camp-access.json','tests/phase1/BuildingPresentationTests.cs'];
function validateState(a,s,p){
 need(a.schemaVersion===1&&a.id==='main-assemblies'&&a.sampleDataOnly===true&&s.id==='camp-access','BUILDING_STATE_ID');
 need(equal(a.tierDefinitions.map(x=>x.repairPrice),[50,150,400]),'BUILDING_FIXED_REPAIR_FEES');
 need(equal(a.tierDefinitions.map(x=>[x.tier,x.health,x.price,x.upgradePrice,x.workSeconds,x.seats]),[[1,600,0,200,0,8],[2,1400,200,500,90,12],[3,3200,500,0,150,16]]),'BUILDING_BASELINE');
 need(equal(a.tierDefinitions.map(x=>x.attachments.length),[0,0,2])&&equal(a.tierDefinitions[2].attachments.map(x=>x.id),['guard-left','guard-right']),'BUILDING_GUARD_SOCKETS');
 need(equal(a.tierDefinitions[2].attachments.map(x=>x.position),[[-30,40],[146,40]])&&equal(a.tierDefinitions[2].attachments.map(x=>x.phase),[0,0.7]),'BUILDING_GUARD_INDEPENDENT');
 need(a.tierDefinitions[2].attachments.every(x=>x.guard==='main.single-guard'&&x.health==='main-instance'),'BUILDING_SHARED_HEALTH');
 const g=a.guard;need(g.id==='main.single-guard'&&g.count===1&&g.presentationTier===1&&g.sourceClip==='common.clip.783'&&g.healthOwner==='main-instance'&&g.repair===false&&g.destruction===false&&equal(g.loot,[])&&g.populationUse===0,'BUILDING_GUARD_OWNERSHIP');
 need(equal(g.attackProfile,{tier:1,archers:1,rangeUnits:20,arrowDamage:14,intervalSeconds:1.4,targets:['zombie'],inventory:false,leaderEligible:false,bowDurability:false}),'BUILDING_GUARD_PROFILE');
 need(equal(g.frames.map(x=>[x.time,x.part]),Array.from({length:5},(_,i)=>[i*0.125,'guard-shoot-'+i]))&&g.stop===0.625&&g.sampleRate===8&&g.equipment.bodyPixelsIncludeDonorBow&&g.equipment.neverDoubleDrawBow&&g.equipment.arrowPart==='arrow','BUILDING_GUARD_ART');
 need(a.restoration.replayDestruction===false&&a.restoration.replayAttack===false&&a.restoration.preservePaidWork===true&&a.restoration.functionalTier==='completed-until-progress-is-one','BUILDING_RESTORE');
 need(a.resourceBindings.atlas===p.atlas.path&&a.resourceBindings.atlasSha256===p.atlas.sha256&&a.presenter.type==='KingdomZombies.Inspection.BuildingPreview'&&a.presenter.method==='Attach'&&a.presenter.hubAdoptionOwner==='04-23','BUILDING_PRESENTER_BINDING');
 const ids=['intact','damage-light','damage-heavy','repair-paid','repair-complete','upgrade-paid','upgrade-complete','defeated','restore-defeated','restore-upgrade','stash-open','stash-selected'];
 need(equal(a.samples.map(x=>x.id),ids)&&a.samples.every(x=>Number.isFinite(x.progress)&&x.progress>=0&&x.progress<=1&&Number.isFinite(x.hpRatio)&&x.hpRatio>=0&&x.hpRatio<=1),'BUILDING_SAMPLE_SET');
 need(a.samples[5].progress===0.5&&a.samples[6].progress===1&&a.samples[8].quiet&&a.samples[9].quiet,'BUILDING_WORK_VISIBILITY');
 need(s.stash.id==='shared-camp-stash'&&s.stash.physicalLidExclusive===false&&equal(s.stash.accessTiers,[1,2,3])&&equal(s.stash.sampleConcurrentViewers,['sample.actor-a','sample.actor-b'])&&s.stash.marker==='STASH'&&s.stash.closedPart==='chest-closed'&&s.stash.openPart==='chest-open','BUILDING_STASH_SHARED');
 need(equal(s.layouts.map(x=>[x.tier,x.capacity,x.seats.length]),[[1,8,8],[2,12,12],[3,16,16]])&&s.layouts.every(x=>x.stashId===s.stash.id&&x.overflow.remainsValid&&x.overflow.reserved===false&&x.seats.every(y=>y.occupied===false)),'BUILDING_SEAT_LAYOUT');
 for(const layout of s.layouts)need(new Set(layout.seats.map(x=>JSON.stringify(x.position))).size===layout.capacity&&new Set(layout.seats.map(x=>x.id)).size===layout.capacity,'BUILDING_SEAT_DISTINCT');
}
function stateControls(a,s,p){
 const controls=[];validateState(a,s,p);
 for(const [id,mutate,code] of [
  ['early-new-seats',(x,y)=>x.tierDefinitions[1].seats=16,'BUILDING_BASELINE'],
  ['three-main-guards',x=>x.tierDefinitions[2].attachments.push({...x.tierDefinitions[2].attachments[1],id:'guard-third'}),'BUILDING_GUARD_SOCKETS'],
  ['same-clock',x=>x.tierDefinitions[2].attachments[1].phase=0,'BUILDING_GUARD_INDEPENDENT'],
  ['attachment-health',x=>x.tierDefinitions[2].attachments[0].health='own-health','BUILDING_SHARED_HEALTH'],
  ['attachment-repair',x=>x.guard.repair=true,'BUILDING_GUARD_OWNERSHIP'],
  ['attachment-loot',x=>x.guard.loot=['bow'],'BUILDING_GUARD_OWNERSHIP'],
  ['tier3-attack-profile',x=>x.guard.attackProfile.tier=3,'BUILDING_GUARD_PROFILE'],
  ['fake-stick-guard',x=>x.guard.frames[0].part='stick','BUILDING_GUARD_ART'],
  ['replay-destruction',x=>x.restoration.replayDestruction=true,'BUILDING_RESTORE'],
  ['exclusive-stash',(x,y)=>y.stash.physicalLidExclusive=true,'BUILDING_STASH_SHARED'],
  ['missing-stash-tier',(x,y)=>y.stash.accessTiers.pop(),'BUILDING_STASH_SHARED'],
  ['missing-seat',(x,y)=>y.layouts[1].seats.pop(),'BUILDING_SEAT_LAYOUT'],
  ['overlapping-seats',(x,y)=>y.layouts[1].seats[1].position=y.layouts[1].seats[0].position,'BUILDING_SEAT_DISTINCT'],
  ['false-hub-adoption',x=>x.presenter.hubAdoptionOwner='complete','BUILDING_PRESENTER_BINDING']]){
   const x=structuredClone(a),y=structuredClone(s);mutate(x,y);let error;try{validateState(x,y,p);}catch(e){error=e.message;}
   need(error?.includes(code),'BUILDING_STATE_CONTROL',id+':'+error);controls.push({id,rejected:true,diagnostic:error});
 }
 return controls;
}
const HARNESS=String.raw`using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
namespace KingdomZombies.BuildingProof;
public sealed partial class BuildingHarness : Node
{
 private readonly List<object> _outcomes=new(); private bool _failed; private Type _subject=null!;private Assembly _assembly=null!;
 private object Invoke(object instance,string method,params object[] args){return _subject.GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(instance,args)!;}
 private void Check(string id,bool pass,object detail){_outcomes.Add(new{id,pass,detail});_failed|=!pass;}
 private object Read(string file,string id){return _subject.GetMethod("ReadDefinition",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{FileAccess.GetFileAsString(file),id})!;}
 private object Create(Node2D root,string identity){var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;return Activator.CreateInstance(_subject,BindingFlags.Instance|BindingFlags.NonPublic,null,new[]{root,gateway,Read("res://content/packs/main-buildings.json","main-buildings"),Read("res://content/world/main-assemblies.json","main-assemblies"),Read("res://content/packs/camp-access.json","camp-access"),identity},null)!;}
 private static void Select(Node2D root,string control,int index){var value=root.GetNode<OptionButton>("Controls/"+control);value.Select(index);value.EmitSignal(OptionButton.SignalName.ItemSelected,index);}
 private static Rect2 Bounds(Control content,Sprite2D sprite){var transform=content.GetGlobalTransform().AffineInverse()*sprite.GetGlobalTransform();var rect=sprite.GetRect();var result=new Rect2(transform*rect.Position,Vector2.Zero);return result.Expand(transform*rect.End).Expand(transform*new Vector2(rect.End.X,rect.Position.Y)).Expand(transform*new Vector2(rect.Position.X,rect.End.Y));}
 private static bool Inside(Rect2 rect,Vector2 size){return rect.Position.X>=-0.01&&rect.Position.Y>=-0.01&&rect.End.X<=size.X+0.01&&rect.End.Y<=size.Y+0.01;}
 private static bool Reachable(Control content,Sprite2D sprite){return !sprite.IsVisibleInTree()||Inside(Bounds(content,sprite),content.Size);}
 public override async void _Ready(){try{await Run();}catch(Exception e){Check("fixture-exception",false,e.ToString());}var result=new{pid=OS.GetProcessId(),nonce=OS.GetEnvironment("BUILDING_PROOF_NONCE"),outcomes=_outcomes,failed=_failed,assemblySha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"))))};using(var f=FileAccess.Open("res://native-results.json",FileAccess.ModeFlags.Write)){f.StoreString(JsonSerializer.Serialize(result));}GD.Print("BUILDING_NATIVE_COMPLETE "+result.nonce+" "+(_failed?"FAIL":"PASS"));GetTree().Quit(_failed?1:0);}
 private async Task Run(){
  GetTree().Root.Size=new Vector2I(1280,720);Check("actual-root-viewport",GetTree().Root.Size==new Vector2I(1280,720),GetTree().Root.Size.ToString());
  _assembly=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));_subject=_assembly.GetType("KingdomZombies.Inspection.BuildingPreview",true)!;
  Check("actual-production-type",!_subject.IsPublic&&StringComparer.Ordinal.Equals(_subject.Assembly.GetName().Name,"KingdomZombies"),_subject.FullName!);
  var root=new Node2D{Name="First"};AddChild(root);var preview=Create(root,"main.first");Invoke(preview,"Attach");
  var second=new Node2D{Name="Second",Position=new Vector2(0,450)};AddChild(second);var other=Create(second,"main.second");Invoke(other,"Attach");
  Check("real-native-controls",root.GetNode<OptionButton>("Controls/Sample").ItemCount==12&&root.GetNode<OptionButton>("Controls/Tier").ItemCount==3,"native OptionButton signals invoke actual production handlers");
  string[] ids={"intact","damage-light","damage-heavy","repair-paid","repair-complete","upgrade-paid","upgrade-complete","defeated","restore-defeated","restore-upgrade","stash-open","stash-selected"};
  for(int tier=1;tier<=3;tier++){Select(root,"Tier",tier-1);for(int sample=0;sample<12;sample++){
   Select(root,"Sample",sample);var art=root.GetNode<Node2D>("ArtScroll/Content/Art");bool dead=sample==7||sample==8;int functional=sample==6?Math.Min(3,tier+1):tier;int count=dead?0:new[]{8,12,16}[functional-1];
   Check("functional-"+tier+"-"+ids[sample],root.GetMeta("functional_tier").AsInt32()==functional&&(!dead?art.GetNode("Seats").GetChildCount()==count:!art.HasNode("Seats")),new{tier,functional,count});
   string visual=dead?"defeated":sample==1?"damage-light":sample==2||sample==3?"damage-heavy":"intact";
   Check("main-silhouette-"+tier+"-"+ids[sample],StringComparer.Ordinal.Equals(art.GetNode<Sprite2D>("Main").GetMeta("part_key").AsString(),"tier-"+functional+"-"+visual),art.GetNode<Sprite2D>("Main").GetMeta("part_key").AsString());
   bool guards=!dead&&functional==3;Check("guard-count-"+tier+"-"+ids[sample],art.HasNode("guard-left")==guards&&art.HasNode("guard-right")==guards,guards?2:0);
   if(guards){foreach(string name in new[]{"guard-left","guard-right"}){var guard=art.GetNode(name);Check("shared-health-"+tier+"-"+sample+"-"+name,StringComparer.Ordinal.Equals(guard.GetMeta("health_identity").AsString(),"main.first")&&guard.GetMeta("guard_count").AsInt32()==1&&!guard.GetMeta("independent_repair").AsBool()&&!guard.GetMeta("independent_destruction").AsBool()&&guard.GetMeta("loot_count").AsInt32()==0,guard.GetChildCount());}}
   if(!dead){Check("shared-stash-"+tier+"-"+sample,StringComparer.Ordinal.Equals(art.GetNode("Stash").GetMeta("stash_identity").AsString(),"shared-camp-stash")&&art.GetNode("Stash").GetMeta("concurrent_viewers").AsInt32()==(sample>=10?2:0),art.GetNode<Label>("StashMarker").Text);}
   Check("paid-overlay-"+tier+"-"+sample,art.HasNode("PaidWork")==(!dead&&(sample==3||sample==5||sample==9)),art.HasNode("PaidWork"));
   if(sample==7){int before=root.GetMeta("destruction_events").AsInt32();Invoke(preview,"Restore",tier,tier,0.0,true);Check("quiet-defeat-"+tier,root.GetMeta("destruction_events").AsInt32()==before&&root.GetNode<Node2D>("ArtScroll/Content/Art").GetChildCount()==1,before);}
  }}
  Invoke(preview,"Restore",2,3,0.5,false);var current=root.GetNode<Node2D>("ArtScroll/Content/Art");Check("restored-paid-upgrade",root.GetMeta("functional_tier").AsInt32()==2&&current.GetNode("Seats").GetChildCount()==12&&!current.HasNode("guard-left")&&current.HasNode("PaidWork"),root.GetNode<Label>("Status").Text);
  Check("instance-state-independent",second.GetMeta("functional_tier").AsInt32()==1&&StringComparer.Ordinal.Equals(second.GetMeta("health_identity").AsString(),"main.second"),second.GetNode<Label>("Status").Text);
  Invoke(preview,"Restore",2,3,1.0,false);current=root.GetNode<Node2D>("ArtScroll/Content/Art");Invoke(preview,"Tick",0.15);var left=current.GetNode<Sprite2D>("guard-left/Guard");var right=current.GetNode<Sprite2D>("guard-right/Guard");Check("independent-actual-guard-frames",StringComparer.Ordinal.Equals(left.GetMeta("part_key").AsString(),"guard-shoot-1")&&StringComparer.Ordinal.Equals(right.GetMeta("part_key").AsString(),"guard-shoot-0"),new{left=left.GetMeta("part_key").AsString(),right=right.GetMeta("part_key").AsString()});
  Invoke(preview,"Tick",0.15);Check("actual-arrow-release",current.GetNode<Sprite2D>("guard-left/Arrow").Visible&&!current.GetNode<Sprite2D>("guard-right/Arrow").Visible,current.GetNode<Sprite2D>("guard-left/Arrow").Position.ToString());
  var texture=(AtlasTexture)left.Texture;var image=texture.GetImage();var png=new Image();png.LoadPngFromBuffer(FileAccess.GetFileAsBytes("res://assets/buildings/main-tiers.png"));var region=(Rect2I)texture.Region;bool exact=true;for(int y=0;y<image.GetHeight();y++){for(int x=0;x<image.GetWidth();x++){Color a=image.GetPixel(x,y),b=png.GetPixel(region.Position.X+x,region.Position.Y+y);exact&=a.A==b.A&&(a.A==0||(a.R==b.R&&a.G==b.G&&a.B==b.B));}}Check("native-guard-pixels",exact&&image.GetWidth()>10&&image.GetHeight()>20,new{width=image.GetWidth(),height=image.GetHeight()});
  for(int zoom=0;zoom<4;zoom++){Select(root,"Zoom",zoom);for(int i=0;i<3;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);var scroll=root.GetNode<ScrollContainer>("ArtScroll");var content=scroll.GetNode<Control>("Content");bool reachable=true;int observed=0;var sprites=new List<object>();foreach(Node node in current.FindChildren("*","Sprite2D",true,false)){var sprite=(Sprite2D)node;var rect=sprite.GetRect();Vector2 tl=content.GetGlobalTransform().AffineInverse()*sprite.GetGlobalTransform()*rect.Position;Vector2 br=content.GetGlobalTransform().AffineInverse()*sprite.GetGlobalTransform()*rect.End;bool inside=tl.X>=0&&tl.Y>=0&&br.X<=content.Size.X+0.01&&br.Y<=content.Size.Y+0.01;reachable&=Reachable(content,sprite);observed++;sprites.Add(new{path=current.GetPathTo(sprite).ToString(),part=sprite.GetMeta("part_key").AsString(),visible=sprite.Visible,visibleInTree=sprite.IsVisibleInTree(),rect=rect.ToString(),position=sprite.Position.ToString(),transform=sprite.GetGlobalTransform().ToString(),tl=tl.ToString(),br=br.ToString(),inside,sampleTime=sprite.GetParent().HasMeta("sample_time")?sprite.GetParent().GetMeta("sample_time").AsDouble():-1});}scroll.ScrollHorizontal=int.MaxValue;scroll.ScrollVertical=int.MaxValue;await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);Check("zoom-reachability-"+(1<<zoom),reachable&&observed>=9&&(zoom<2||scroll.GetHScrollBar().MaxValue>scroll.GetHScrollBar().Page),new{observed,content=content.Size.ToString(),maxX=scroll.GetHScrollBar().MaxValue,pageX=scroll.GetHScrollBar().Page,maxY=scroll.GetVScrollBar().MaxValue,pageY=scroll.GetVScrollBar().Page,scrollX=scroll.ScrollHorizontal,scrollY=scroll.ScrollVertical,sprites});scroll.ScrollHorizontal=0;scroll.ScrollVertical=0;}
  // Sample both independent clocks from release through the final visible instant, then stop.
  foreach(string socket in new[]{"guard-left","guard-right"}){foreach(double localTime in new[]{0.25,0.375,0.5,0.624,0.625}){
   Invoke(preview,"Restore",3,3,0.0,false);double elapsed=StringComparer.Ordinal.Equals(socket,"guard-left")?localTime:localTime+0.7;Invoke(preview,"Tick",elapsed);current=root.GetNode<Node2D>("ArtScroll/Content/Art");var arrow=current.GetNode<Sprite2D>(socket+"/Arrow");bool active=localTime<0.625;
   for(int zoom=0;zoom<4;zoom++){Select(root,"Zoom",zoom);for(int i=0;i<3;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);var scroll=root.GetNode<ScrollContainer>("ArtScroll");var content=scroll.GetNode<Control>("Content");var bounds=Bounds(content,arrow);double maxX=Math.Max(0,scroll.GetHScrollBar().MaxValue-scroll.GetHScrollBar().Page),maxY=Math.Max(0,scroll.GetVScrollBar().MaxValue-scroll.GetVScrollBar().Page);scroll.ScrollHorizontal=(int)Math.Clamp(bounds.Position.X,0,maxX);scroll.ScrollVertical=(int)Math.Clamp(bounds.Position.Y,0,maxY);await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
    var viewport=Bounds(scroll,arrow);bool validScroll=scroll.ScrollHorizontal>=0&&scroll.ScrollHorizontal<=maxX&&scroll.ScrollVertical>=0&&scroll.ScrollVertical<=maxY;bool visibleFit=Inside(viewport,new Vector2((float)scroll.GetHScrollBar().Page,(float)scroll.GetVScrollBar().Page));
    Check("arrow-lifetime-"+socket+"-"+localTime+"-zoom-"+(1<<zoom),arrow.IsVisibleInTree()==active&&Reachable(content,arrow)&&validScroll&&(!active||visibleFit),new{socket,elapsed,localTime,actualTime=arrow.GetParent().GetMeta("sample_time").AsDouble(),active,visible=arrow.IsVisibleInTree(),rect=arrow.GetRect().ToString(),bounds=bounds.ToString(),viewport=viewport.ToString(),content=content.Size.ToString(),maxX,maxY,pageX=scroll.GetHScrollBar().Page,pageY=scroll.GetVScrollBar().Page,scrollX=scroll.ScrollHorizontal,scrollY=scroll.ScrollVertical});scroll.ScrollHorizontal=0;scroll.ScrollVertical=0;
   }
  }}
  Invoke(preview,"Restore",3,3,0.0,false);Invoke(preview,"Tick",0.3);current=root.GetNode<Node2D>("ArtScroll/Content/Art");Select(root,"Zoom",0);for(int i=0;i<3;i++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);var area=root.GetNode<Control>("ArtScroll/Content");var controlArrow=current.GetNode<Sprite2D>("guard-left/Arrow");var savedPosition=controlArrow.Position;controlArrow.Position=new Vector2(1000,-14);
  Check("visible-outside-rejected",controlArrow.IsVisibleInTree()&&!Reachable(area,controlArrow)&&!Inside(Bounds(area,controlArrow),area.Size),new{visible=controlArrow.IsVisibleInTree(),bounds=Bounds(area,controlArrow).ToString(),content=area.Size.ToString()});controlArrow.Visible=false;
  Check("hidden-outside-ignored",!controlArrow.IsVisibleInTree()&&Reachable(area,controlArrow)&&!Inside(Bounds(area,controlArrow),area.Size),new{visible=controlArrow.IsVisibleInTree(),bounds=Bounds(area,controlArrow).ToString()});controlArrow.Position=savedPosition;controlArrow.Visible=true;
  Check("visible-inside-restored",controlArrow.IsVisibleInTree()&&Reachable(area,controlArrow),Bounds(area,controlArrow).ToString());
  Select(root,"Light",1);Check("night-palette",current.Modulate==new Color(0.55f,0.65f,0.82f),current.Modulate.ToString());Select(root,"Light",0);Check("day-palette",current.Modulate==Colors.White,current.Modulate.ToString());
  int nativeCalls=0;Exception? workerError=null;await Task.Run(()=>{try{Invoke(preview,"Tick",0.1);nativeCalls++;}catch(TargetInvocationException e){workerError=e.InnerException;}});Check("worker-before-native-rejected",nativeCalls==0&&StringComparer.Ordinal.Equals(workerError?.Message,"THREAD_ACCESS"),workerError?.Message??"none");
  var previous=current.GetChildCount();int events=root.GetMeta("destruction_events").AsInt32();Invoke(preview,"Restore",3,3,0.0,true);Invoke(preview,"Restore",3,3,0.0,true);Check("quiet-repeat-no-replay",events==root.GetMeta("destruction_events").AsInt32(),events);
  Invoke(preview,"Detach");Check("detach-retires-native-owners",root.GetChildCount()==0&&second.GetChildCount()>0,previous);bool rejected=false;try{Invoke(preview,"Tick",0.1);}catch(TargetInvocationException e){rejected=StringComparer.Ordinal.Equals(e.InnerException?.Message,"BUILDING_DETACHED");}Check("detached-result-rejected",rejected,"closed owner rejects tick");Invoke(other,"Detach");root.QueueFree();second.QueueFree();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
 }
}
`;
function isolated(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT};}
async function compiled(context){
 const assembly=inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
 const discovery=await runner.command(context,'buildings-discovery','dotnet',[assembly,'--list-tests','--filter-class','*BuildingPresentationTests','--no-ansi'],{env:isolated()});
 const names=[...discovery.stdout.matchAll(/BuildingPresentationTests\.([A-Za-z0-9_]+)/g)].map(x=>x[1]);
 const expected={Pending_Upgrade_RetainsCompletedTier:1,Work_UpgradeProgress_ProjectsCapacityAndGuardsTogether:8,Work_InvalidState_IsRejected:7,Definition_ValidAndAmbiguousInputs_DetachesAndRejects:1};
 need(names.length===17&&equal(Object.keys(expected).sort(),[...new Set(names)].sort()),'BUILDING_DISCOVERY');
 const dir=inside(context.directory,'compiled');fs.mkdirSync(dir);await runner.command(context,'buildings-compiled','dotnet',[assembly,'--filter-class','*BuildingPresentationTests','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',dir,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:isolated()});
 const report=document(dir,'tests.json');need(report.results.summary.tests===17&&report.results.summary.passed===17&&report.results.summary.failed===0&&report.results.summary.skipped===0&&report.results.tests.length===17,'BUILDING_COMPILED_RESULTS');
 for(const [name,count]of Object.entries(expected))need(report.results.tests.filter(x=>x.name.includes('BuildingPresentationTests.'+name)).length===count,'BUILDING_COMPILED_SUBJECT',name);
 return {discovered:names.length,passed:report.results.summary.passed};
}
async function nativeState(context){
 const project=inside(context.directory,'native-building');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(x=>fs.statSync(path.join(original,x)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'production-assemblies.json',closure);
 const proj='<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>MainBuildingProof</AssemblyName><Nullable>enable</Nullable><EnableDynamicLoading>true</EnableDynamicLoading></PropertyGroup></Project>';
 write(project,'MainBuildingProof.csproj',Buffer.from(proj));write(project,'BuildingHarness.cs',Buffer.from(HARNESS));
 write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="MainBuildingProof"\n[dotnet]\nproject/assembly_name="MainBuildingProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of STATE_FILES.filter(x=>x.startsWith('src/godot-game/')&&!x.endsWith('.cs')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n var proof=load("res://BuildingHarness.cs").new()\n root.add_child(proof)\n'));
 const harnessBuild=await runner.command(context,'buildings-harness-build','dotnet',['build',inside(project,'MainBuildingProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:isolated()});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(harnessBuild.stdout),'BUILDING_HARNESS_CLEAN_BUILD');
 const tool=engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
 const archive=inside(ROOT,'.phase1/cache/'+pin.file),name=pin.executable.replace('_console.exe','.exe');
 need(fs.statSync(archive).size<=268435456&&hash(fs.readFileSync(archive))===pin.sha256,'BUILDING_ARCHIVE_PIN');
 const authentication=await runner.command(context,'buildings-native-tool-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',
  "Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip=[IO.Compression.ZipFile]::OpenRead('D:/Work/Games/KingdomZombies/.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip'); try { $entry=$zip.Entries | Where-Object { $_.FullName -ceq 'Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe' }; if(@($entry).Count -ne 1 -or $entry.Length -le 0 -or $entry.Length -gt 268435456){throw 'ARCHIVE_ENTRY'}; $stream=$entry.Open(); try { $sha=[Security.Cryptography.SHA256]::Create(); [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } finally { $stream.Dispose() } } finally { $zip.Dispose() }"]);
 const executable=path.join(path.dirname(tool.executable),name),nativeHash=hash(fs.readFileSync(executable));need(/^[a-f0-9]{64}$/.test(authentication.stdout.trim())&&nativeHash===authentication.stdout.trim(),'BUILDING_NATIVE_ENTRY_PIN');
 write(context.directory,'engine.json',{...tool,consoleExecutable:tool.executable,executable,sha256:nativeHash,archiveSha256:pin.sha256,archiveEntry:pin.directory+'/'+name});
 const version=await runner.command(context,'buildings-engine-version',executable,['--headless','--version']);need(version.stdout.trim()===tool.version,'BUILDING_ENGINE_VERSION');
 const nativeEnv={...isolated(),BUILDING_PROOF_NONCE:context.nonce};
 await runner.command(context,'buildings-native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env:nativeEnv});
 const child=await runner.command(context,'buildings-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env:nativeEnv});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('BUILDING_NATIVE_COMPLETE '+context.nonce+' PASS'),'BUILDING_NATIVE_MARKER');const result=document(project,'native-results.json');
 need(result.pid===child.pid&&result.nonce===context.nonce&&result.failed===false&&result.outcomes.length===260&&result.outcomes.every(x=>x.pass),'BUILDING_NATIVE_RESULTS');
 need(result.assemblySha256===closure.find(x=>x.file==='KingdomZombies.dll').sha256,'BUILDING_ACTUAL_ASSEMBLY');write(context.directory,'native-observation.json',result);return {pid:result.pid,observations:result.outcomes.length,assemblySha256:result.assemblySha256};
}
async function state(context){
 const before=fingerprint(STATE_FILES);write(context.directory,'inputs.json',before);
 const assembly=document(ROOT,'src/godot-game/content/world/main-assemblies.json'),access=document(ROOT,'src/godot-game/content/packs/camp-access.json'),pack=document(ROOT,'src/godot-game/content/packs/main-buildings.json');
 const controls=stateControls(assembly,access,pack);write(context.directory,'negative-controls.json',controls);
 await runner.command(context,'buildings-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:isolated()});
 const tests=await compiled(context),native=await nativeState(context);need(equal(before,fingerprint(STATE_FILES)),'BUILDING_SOURCE_CHANGED');return {samples:12,layouts:[8,12,16],expectedRejected:controls.length,compiled:tests,native,scope:'Actual compiled source and native headless behavior only; current hub adoption04-23 and rendered04-32/33 acceptance remain pending'};
}
runner.registerCase('buildings.main-state',{group:'buildings',title:'Actual guarded main lifecycle, shared access, capacity and independent one-guard attachments',importance:'Critical',rationale:'Premature upgrade functions, independent attachment ownership or replayed defeat corrupt required building presentation',needsGameWindow:false,runGroup:'G0',order:25,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-08','AC-09'],setup:'Strict current production assembly,17 compiled controls, retained art definitions and owned native proof project using actual production DLL',
 actions:['Validate exact baseline/resources and14 semantic negative controls','Discover and run all17 BuildingPresentationTests with zero skips','Load the byte-identical actual production presenter in native Godot and drive36 tier/sample selections','Verify old functional tiers, seats, shared concurrent stash, quiet restore, actual guard pixels and independent attack/arrow clocks','Exercise1/2/4/8x native scroll reachability, two independent owners, captured thread guard and teardown'],
 expected:'All declared compiled/native observations pass; no projected labels or substitute implementation; final current-hub adoption and appearance remain pending',reset:'Fresh owned headless project and bounded child processes; no real game window',evidence:['inputs.json','negative-controls.json','buildings-strict-build.command.json','buildings-discovery.command.json','buildings-compiled.command.json','production-assemblies.json','native-observation.json','result.json']},state);
module.exports={ART_FILES,STATE_FILES,VERIFY,HARNESS,fingerprint,art,validateState,stateControls,compiled,nativeState,state};
