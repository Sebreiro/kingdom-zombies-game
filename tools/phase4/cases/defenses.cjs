'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
const ART_FILES=['src/godot-game/assets/buildings/defenses.png','src/godot-game/content/packs/defenses.json','production/phase4/defenses.zip','tools/phase4/cases/defenses.cjs'];
const ART_INPUTS=[...ART_FILES,'production/phase4/common-selected.zip','production/phase4/main-buildings.zip','src/godot-game/content/packs/common.json','src/godot-game/content/packs/main-buildings.json'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
const VERIFY=String.raw`import sys,pathlib,json,io,hashlib,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
d=m.read_archive(root/'production/phase4/defenses.zip');p=m.loads(d['defenses.json']);atlas=Image.open(io.BytesIO(d['defenses.png'])).convert('RGBA')
m.need(d['defenses.json']==(root/'src/godot-game/content/packs/defenses.json').read_bytes(),'DEFENSE_PACK_BYTES');m.need(d['defenses.png']==(root/'src/godot-game/assets/buildings/defenses.png').read_bytes(),'DEFENSE_ATLAS_BYTES')
inventory=m.loads(d['members.json']);m.need(len(inventory)+1==len(d) and len({r['path'] for r in inventory})==len(inventory),'DEFENSE_INVENTORY')
for r in inventory:m.need(r['path'] in d and r['bytes']==len(d[r['path']]) and r['sha256']==m.sha(d[r['path']]),'DEFENSE_MEMBER_HASH')
def verify(p,d):
 m.need(p['schemaVersion']==1 and p['version']==1 and p['id']=='defenses','DEFENSE_ID')
 m.need(p['logicalPixelsPerWorldPixel']==1 and 'D-13 LOW-RESOLUTION OVERRIDES' in p['stylePrecedence'],'DEFENSE_DENSITY')
 m.need(p['acceptance']=={'visual':'pending','owner':'04-32/04-33','humanApproval':None,'completedObligations':[]},'DEFENSE_FALSE_ACCEPTANCE')
 m.need(p['atlas']['path']=='res://assets/buildings/defenses.png' and p['atlas']['size']==[1024,512] and p['atlas']['sha256']==m.sha(d['defenses.png']) and p['atlas']['rgbaSha256']==m.sha(atlas.tobytes()),'DEFENSE_ATLAS')
 expected=[('BL-04','wall',1,'wood'),('BL-05','wall',2,'reinforced-wood'),('BL-06','wall',3,'metal-reinforced'),('BL-07','tower',1,'wood'),('BL-08','tower',2,'reinforced-wood'),('BL-09','tower',3,'metal-reinforced')]
 m.need([(r['id'],r['kind'],r['tier'],r['material']) for r in p['tiers']]==expected,'DEFENSE_TIERS')
 m.need(len(p['parts'])==46 and len({r['key'] for r in p['parts']})==46,'DEFENSE_PARTS');parts={r['key']:r for r in p['parts']};images={}
 for part in p['parts']:
  m.need(part['origin'] in d and part['member'] in d and part['sha256']==m.sha(d[part['member']]),'DEFENSE_ORIGIN')
  im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA');images[part['key']]=im
  m.need(im.getbbox() and part['density']==1 and part['worldSize']==list(im.size) and part['rgbaSha256']==m.sha(im.tobytes()) and part['visiblePixels']==sum(bool(a) for a in im.getchannel('A').tobytes()),'DEFENSE_IMAGE')
  m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or (r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'DEFENSE_ALPHA')
  x,y,w,h=part['atlasRect'];m.need([w,h]==list(im.size) and 0<=x<=1024-w and 0<=y<=512-h and atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'DEFENSE_ATLAS_PIXELS')
 for r in p['tiers']:
  m.need(r['bakedGuardCount']==0 and r['previousTierRuin'] is True and list(r['states'])==['intact','damage-light','damage-heavy','destroyed'] and all(k in parts for k in r['states'].values()),'DEFENSE_LIFECYCLE')
  key=r['kind']+'-'+str(r['tier']);m.need(r['states']['intact']==key and r['states']['destroyed']==key+'-ruin' and r['footprint']==list(images[key].size),'DEFENSE_PREVIOUS_TIER')
  base=images[key];light=images[key+'-damage-light'];heavy=images[key+'-damage-heavy'];m.need(base.tobytes()!=light.tobytes()!=heavy.tobytes(),'DEFENSE_DAMAGE')
  retained=lambda other:sum(a==b and a[3]>0 for a,b in zip(base.get_flattened_data(),other.get_flattened_data()))/sum(bool(a) for a in base.getchannel('A').tobytes())
  m.need(retained(light)>retained(heavy)>0.70,'DEFENSE_LOCALIZED_DAMAGE')
  m.need(r['fragments']==['metal-fragment' if r['tier']==3 else 'wood-fragment'],'DEFENSE_MATCHED_RUBBLE')
  m.need(images[key+'-ruin'].getbbox() and images[key+'-ruin'].tobytes()!=base.tobytes(),'DEFENSE_RUIN')
 expectedFrames=[('main-'+str(i),[160,88]) for i in range(1,4)]+[(r['kind']+'-'+str(r['tier']),r['footprint']) for r in p['tiers']]
 m.need([(r['structure'],r['footprint']) for r in p['frames']]==expectedFrames,'DEFENSE_NINE_FOOTPRINTS')
 for r in p['frames']:
  a=images[r['frame']];b=images[r['damagedFrame']];m.need(list(a.size)==r['footprint'] and b.size==a.size and a.tobytes()!=b.tobytes() and r['tileScale']==1,'DEFENSE_FRAME')
  coverage=sum(bool(a) for a in a.getchannel('A').tobytes())/(a.width*a.height);m.need(coverage<=p['visibility']['workOverlayMaxCoverage']<=0.48,'DEFENSE_OVERLAY_READABILITY')
 m.need(p['siteStates']==['unbuilt','available','tree-blocked','portal-blocked','main-tier-blocked','unaffordable','new-frame','damaged-unfinished','intact','damage-light','damage-heavy','upgrade-paid','upgrade-complete','repair-paid','repair-complete','unsafe-waiting','no-worker-waiting','destroyed','rebuild-paid','rebuild-complete','restore-destroyed','restore-upgrade'],'DEFENSE_SITE_STATES')
 m.need(p['visibility']=={'intactAbove':0.75,'lightDamageAbove':0.4,'heavyDamageAbove':0,'newFrameMaxHealthFraction':0.25,'workOverlayMaxCoverage':0.48,'oldFunctionalTierUntilProgress':1,'rubblePreservesCompletedTier':True,'noGuardLoot':True,'frameTileScale':1},'DEFENSE_VISIBILITY')
 m.need(p['sources']['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['sources']['mainBundleSha256']==m.digest(root/'production/phase4/main-buildings.zip'),'DEFENSE_LINEAGE')
 m.need(len(p['sources']['generated'])==3,'DEFENSE_GENERATED_COUNT')
 for r in p['sources']['generated']:
  m.need(r['tool']=='built-in image_gen' and r['master'] in d and r['sha256']==m.sha(d[r['master']]) and r['promptSha256']==m.sha(d[r['prompt']]),'DEFENSE_MASTER')
  master=Image.open(io.BytesIO(d[r['master']])).convert('RGBA');low,high=master.getchannel('A').getextrema();m.need(low==0 and high>=128 and b'D-13 LOW-RESOLUTION' in d[r['prompt']] and b'transparent' in d[r['prompt']],'DEFENSE_MASTER_ALPHA')
 m.need(p['recipe']['sha256']==m.sha(d['recipes/build-defenses.py']),'DEFENSE_RECIPE')
 return {'parts':46,'tiers':6,'frames':9,'members':len(d),'generatedMasters':3,'sourceSelected':p['sources']['selectedCount']}
result=verify(p,d)
# Re-execute exact retained recipe using only retained ZIP inputs; no recovery directory.
ns={'__name__':'retained_recipe'};exec(compile(d['recipes/build-defenses.py'],'retained-build-defenses.py','exec'),ns);replayed,outputs=ns['build'](d);m.need(replayed==p,'DEFENSE_REPLAY_PACK')
for n,b in outputs.items():m.need(b==d[n],'DEFENSE_REPLAY_BYTES '+n)
controls=[]
def reject(name,mutate,code):
 candidate=copy.deepcopy(p);data=dict(d);mutate(candidate,data)
 try:verify(candidate,data)
 except (KeyError,ValueError,AssertionError) as e:m.need(code in str(e),'DEFENSE_CONTROL_REASON '+name+' '+str(e));controls.append({'id':name,'rejected':True,'diagnostic':str(e)})
 else:raise ValueError('DEFENSE_CONTROL_FALSE_PASS '+name)
reject('stone-metal-substitute',lambda p,d:p['tiers'][2].update(material='stone'),'DEFENSE_TIERS')
reject('missing-wide-tower',lambda p,d:p['tiers'].pop(),'DEFENSE_TIERS')
reject('baked-guards',lambda p,d:p['tiers'][5].update(bakedGuardCount=3),'DEFENSE_LIFECYCLE')
reject('lost-previous-tier',lambda p,d:p['tiers'][1].update(previousTierRuin=False),'DEFENSE_LIFECYCLE')
reject('missing-ruin',lambda p,d:p['tiers'][2]['states'].pop('destroyed'),'DEFENSE_LIFECYCLE')
reject('wood-metal-rubble',lambda p,d:p['tiers'][5].update(fragments=['wood-fragment']),'DEFENSE_MATCHED_RUBBLE')
reject('one-arbitrary-frame',lambda p,d:p['frames'].__setitem__(0,{**p['frames'][0],'footprint':[32,16]}),'DEFENSE_NINE_FOOTPRINTS')
reject('scaled-frame',lambda p,d:p['frames'][5].update(tileScale=2),'DEFENSE_FRAME')
reject('missing-waiting',lambda p,d:p['siteStates'].remove('unsafe-waiting'),'DEFENSE_SITE_STATES')
reject('opaque-work',lambda p,d:p['visibility'].update(workOverlayMaxCoverage=1),'DEFENSE_OVERLAY_READABILITY')
reject('no-frame-damage',lambda p,d:p['frames'][3].update(damagedFrame=p['frames'][3]['frame']),'DEFENSE_FRAME')
reject('missing-master',lambda p,d:d.pop('masters/wall-3.png'),'DEFENSE_ORIGIN')
reject('fabricated-generation',lambda p,d:p['sources']['generated'][0].update(tool='code-generated stand-in'),'DEFENSE_MASTER')
reject('wrong-world-density',lambda p,d:p.update(logicalPixelsPerWorldPixel=2),'DEFENSE_DENSITY')
reject('false-approval',lambda p,d:p['acceptance'].update(visual='approved'),'DEFENSE_FALSE_ACCEPTANCE')
reject('shifted-atlas',lambda p,d:p['parts'][2]['atlasRect'].__setitem__(0,p['parts'][2]['atlasRect'][0]+1),'DEFENSE_ATLAS_PIXELS')
print(json.dumps({**result,'replayedParts':len(outputs)-1,'expectedRejected':len(controls),'controls':controls}))
`;
async function art(context){
 const before=fingerprint(ART_INPUTS);write(context.directory,'inputs.json',before);write(context.directory,'verify-defenses.py',Buffer.from(VERIFY));
 const child=await runner.command(context,'defenses-art',PYTHON,['-B',inside(context.directory,'verify-defenses.py'),ROOT]);const result=JSON.parse(child.stdout);
 need(result.tiers===6&&result.frames===9&&result.parts===46&&result.expectedRejected===16&&result.replayedParts===46,'DEFENSE_ART_RESULT');write(context.directory,'negative-controls.json',result.controls);
 need(equal(before,fingerprint(ART_INPUTS)),'DEFENSE_SOURCE_CHANGED');return {...result,scope:'Source art/replay only; native defense states are buildings.defense-state; full hub/index owner04-23; actual named appearance/input pending04-32/33'};
}
runner.registerCase('buildings.defense-art',{group:'buildings',title:'Six material-correct defense tiers, nine fitted work frames and persistent lifecycle source art',importance:'Critical',
 rationale:'Missing defense materials, previous tiers or traceable lifecycle art invalidates required production',needsGameWindow:false,runGroup:'G0-defenses',order:1,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08','AC-09','AC-24'],setup:'Tracked defenses ZIP/atlas/shard and pinned existing Python/Pillow; no donor folder or game window',
 actions:['Authenticate complete retained members and selected donor lineage','Replay46 actual art parts from retained3 generated masters and selected source pixels','Independently validate six tiers, localized damage, matched ruins and nine exact frames','Reject16 material/lifecycle/source/alpha/frame/acceptance faults'],
 expected:'Six actual wood/reinforced-wood/metal tiers and BL10–12 components pass exact source replay and all16 independent controls',reset:'Fresh immutable owned source invocation; no real game window',
 evidence:['inputs.json','defenses-art.command.json','negative-controls.json','result.json']},art);
module.exports={ART_FILES,ART_INPUTS,VERIFY,fingerprint,art};

const STATE_OWN=['src/godot-game/assets/characters/structural-guard.png','src/godot-game/content/packs/guards.json','src/godot-game/content/world/defense-assemblies.json','src/godot-game/inspection/BuildingPreview.cs'];
function stateFiles(){return [...new Set([...ART_INPUTS,...STATE_OWN,...require('./animation.cjs').files(),...require('./main-buildings.cjs').STATE_FILES])];}
const NEGATIVES=[
 ['part-outside','art','parts/0/atlasRect/0',1024,'PART_BOUNDS'],
 ['part-negative','art','parts/0/atlasRect/2',-1,'PART_BOUNDS'],
 ['part-duplicate','art','parts/1/key','wood-fragment','PART_ID'],
 ['atlas-size','art','atlas/size/0',2048,'ATLAS_SIZE'],
 ['footprint','art','tiers/0/footprint/0',99,'FOOTPRINT'],
 ['missing-state','art','tiers/1/states/destroyed','missing','PART_REFERENCE missing'],
 ['stone-tier','art','tiers/2/material','stone','TIER_ID'],
 ['guard-outside','guard','parts/0/atlasRect/1',64,'PART_BOUNDS'],
 ['guard-anchor','guard','frames/0/anchor/1',80,'GUARD_ANCHOR'],
 ['guard-shoulder','guard','frames/0/shoulder/1',80,'GUARD_SHOULDER'],
 ['guard-frame','guard','frames/1/arms','missing','GUARD_FRAME_BINDING'],
 ['guard-time','guard','frames/2/time',0.7,'GUARD_FRAME_BINDING'],
 ['guard-release','guard','timing/release',10,'GUARD_TIMING'],
 ['guard-interval','guard','timing/interval',0,'GUARD_TIMING'],
 ['guard-lifetime','guard','timing/arrowLifetime',600,'GUARD_TIMING'],
 ['finite-guard-ammo','guard','ownership/ammo','finite','GUARD_OWNERSHIP'],
 ['guard-inventory','guard','ownership/inventory',true,'GUARD_OWNERSHIP'],
 ['guard-population','guard','ownership/populationUse',1,'GUARD_OWNERSHIP'],
 ['guard-corpse','guard','ownership/corpse',true,'GUARD_OWNERSHIP'],
 ['guard-loot','guard','ownership/loot',true,'GUARD_OWNERSHIP'],
 ['extra-bow','guard','aim/extraBow',true,'GUARD_AIM'],
 ['aim-range','guard','aim/maximum',20,'GUARD_AIM'],
 ['collision','assembly','tierDefinitions/0/collision/rect/2',-4,'COLLISION'],
 ['faction','assembly','tierDefinitions/0/collision/factionBlocker','everyone','FACTION_BLOCKER'],
 ['work-anchor','assembly','tierDefinitions/1/workAnchor/0',900,'WORK_ANCHOR'],
 ['socket-outside','assembly','tierDefinitions/5/sockets/0/position/0',1000,'SOCKET_POSITION'],
 ['socket-duplicate','assembly','tierDefinitions/5/sockets/1/position',[18,25],'SOCKET_POSITION'],
 ['socket-phase','assembly','tierDefinitions/5/sockets/1/phase',0,'SOCKET_ID'],
 ['socket-count','assembly','tierDefinitions/5/sockets',[],'GUARD_COUNT'],
 ['projectile-origin','assembly','tierDefinitions/5/sockets/0/projectileOrigin/0',99,'PROJECTILE_ORIGIN'],
 ['range','assembly','tierDefinitions/5/range',10000,'ATTACK_PROFILE'],
 ['hp-negative','assembly','samples/9/hpRatio',-1,'SAMPLE_NUMBER'],
 ['progress-negative','assembly','samples/11/progress',-1,'SAMPLE_NUMBER'],
 ['frame-functional','assembly','samples/6/newConstruction',false,'SAMPLE_STATE'],
 ['restore-replay','assembly','restoration/replayAttack',true,'RESTORATION']
].map(([id,pack,path,value,code])=>({id,pack,path,value,code}));
function isolated(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT};}
async function nativeState(context){
 const project=inside(context.directory,'native-defense');fs.mkdirSync(project);
 write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(file=>fs.statSync(path.join(original,file)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'production-assemblies.json',closure);
 write(project,'DefenseProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>DefenseProof</AssemblyName><Nullable>enable</Nullable><EnableDynamicLoading>true</EnableDynamicLoading><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup></Project>'));
 write(project,'DefenseHarness.cs',Buffer.from(HARNESS));write(project,'controls.json',NEGATIVES);
 write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="DefenseProof"\n[dotnet]\nproject/assembly_name="DefenseProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of [...ART_FILES,...STATE_OWN].filter(file=>file.startsWith('src/godot-game/')&&!file.endsWith('.cs')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n var proof=load("res://DefenseHarness.cs").new()\n root.add_child(proof)\n'));
 const build=await runner.command(context,'defenses-harness-build','dotnet',['build',inside(project,'DefenseProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:isolated()});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'DEFENSE_HARNESS_CLEAN');
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];
 const archive=inside(ROOT,'.phase1/cache/'+pin.file);need(fs.statSync(archive).size<=268435456&&hash(fs.readFileSync(archive))===pin.sha256,'DEFENSE_ARCHIVE_PIN');
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),entry=pin.directory+'/'+path.basename(executable);
 const auth=await runner.command(context,'defenses-engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); n=sys.argv[2]; rows=[r for r in z.infolist() if r.filename==n]; assert len(rows)==1 and 0<rows[0].file_size<=268435456; print(hashlib.sha256(z.read(n)).hexdigest())',archive,entry]);
 const nativeHash=hash(fs.readFileSync(executable));need(nativeHash===auth.stdout.trim(),'DEFENSE_NATIVE_ENTRY_PIN');
 write(context.directory,'engine.json',{...tool,consoleExecutable:tool.executable,executable,sha256:nativeHash,archiveSha256:pin.sha256,archiveEntry:entry});
 const version=await runner.command(context,'defenses-engine-version',executable,['--headless','--version']);need(version.stdout.trim()===tool.version,'DEFENSE_ENGINE_VERSION');
 const env={...isolated(),DEFENSE_PROOF_NONCE:context.nonce};
 await runner.command(context,'defenses-native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 const child=await runner.command(context,'defenses-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 const result=document(project,'native-results.json');write(context.directory,'native-observation.json',result);
 need(!/ERROR:|SCRIPT ERROR:|FATAL/i.test(child.stdout+child.stderr)&&child.stderr.trim()===''&&child.stdout.includes('DEFENSE_NATIVE_COMPLETE '+context.nonce+' PASS'),'DEFENSE_NATIVE_CLEAN');
 need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.every(row=>row.pass),'DEFENSE_NATIVE_RESULTS');
 need(new Set(result.outcomes.map(row=>row.id)).size===result.outcomes.length,'DEFENSE_NATIVE_IDENTITIES');
 need(result.gameSha256===closure.find(row=>row.file==='KingdomZombies.dll').sha256&&result.coreSha256===closure.find(row=>row.file==='KingdomZombies.Core.dll').sha256,'DEFENSE_NATIVE_ASSEMBLIES');
 need(result.outcomes.filter(row=>row.id.startsWith('constructor-')).length===NEGATIVES.length&&result.outcomes.filter(row=>row.id.startsWith('media-')).length===6,'DEFENSE_NATIVE_CONTROLS');
 need(result.outcomes.filter(row=>row.id.startsWith('functional-')).length===132&&result.outcomes.filter(row=>row.id.startsWith('source-part-')).length===132&&result.outcomes.filter(row=>row.id.startsWith('H14-intrinsic-')).length===128,'DEFENSE_NATIVE_MATRIX');
 return {pid:result.pid,observations:result.outcomes.length,semanticControls:NEGATIVES.length,mediaControls:6,gameSha256:result.gameSha256,coreSha256:result.coreSha256};
}
async function state(context){
 const before=fingerprint(stateFiles());write(context.directory,'inputs.json',before);write(context.directory,'verify-guards.py',Buffer.from(GUARD_VERIFY));
 const source=await runner.command(context,'defenses-guard-source',PYTHON,['-B',inside(context.directory,'verify-guards.py'),ROOT]);const guardSource=JSON.parse(source.stdout);
 const build=await runner.command(context,'defenses-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:isolated()});
 need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'DEFENSE_PRODUCTION_CLEAN');
 const native=await nativeState(context);need(equal(before,fingerprint(stateFiles())),'DEFENSE_SOURCE_CHANGED');
 return {guardSource,native,scope:'Actual production/native headless defense contract only; main original regression remains buildings.main-state; whole-phase reviewed actual input/appearance pending04-32/33'};
}
runner.registerCase('buildings.defense-state',{group:'buildings',title:'Actual defense lifecycle, independent structural guards and strict constructor boundaries',importance:'Critical',rationale:'Premature guards, wrong rebuild tier, invented structural loot or unsafe definitions invalidate required production',needsGameWindow:false,runGroup:'G0-defenses',order:2,
 requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-07','AC-08','AC-09','AC-19','AC-24'],setup:'Current strict production DLL, exact portable defense/guard definitions and authenticated official headless Godot; no real game window',
 actions:['Reconstruct five exact donor guard poses from disjoint torso/arm-bow layers and unchanged arrow','Build actual production and emitted native harness without warnings','Drive132 independent native tier/site/work/damage/ruin selections and compare imported actual pixels','Aim/fire three guards independently, retain captured arrows and exercise128 sustained per-facing intrinsic cycles','Independently exercise H12 tier/guard/rebuild and H14 empty/no-loot transitions','Reject35 semantic constructor faults separately from6 missing/hash/dimension media faults; verify owner/thread/late fences'],
 expected:'All actual native/semantic/source outcomes pass; original main two-guard profile remains separately verified; actual appearance/input/named judgment remain pending',
 reset:'Fresh owned headless project; ResetDefense cancels current arrows, ordinary selectors reset samples, Detach closes acceptance before late results',
 evidence:['inputs.json','defenses-guard-source.command.json','defenses-strict-build.command.json','production-assemblies.json','engine.json','native-observation.json','result.json']},state);


const HARNESS=String.raw`using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;

namespace KingdomZombies.DefenseProof;

public sealed partial class DefenseHarness : Node
{
    public override async void _Ready()
    {
        try { await Run(); }
        catch (Exception error) { Check(id: "fixture-exception", pass: false, detail: error.ToString()); }
        var result = new { pid = OS.GetProcessId(), nonce = OS.GetEnvironment(variable: "DEFENSE_PROOF_NONCE"),
                outcomes = _outcomes, pixels = PixelObservations, failed = _failed, gameSha256 = Digest(name: "KingdomZombies.dll"),
                coreSha256 = Digest(name: "KingdomZombies.Core.dll") };
        using (var file = Godot.FileAccess.Open(path: "res://native-results.json", flags: Godot.FileAccess.ModeFlags.Write))
        {
            file.StoreString(@string: JsonSerializer.Serialize(value: result));
        }
        GD.Print(what: "DEFENSE_NATIVE_COMPLETE " + result.nonce + " " + (_failed ? "FAIL" : "PASS"));
        GetTree().Quit(exitCode: _failed ? 1 : 0);
    }

    private readonly List<object> _outcomes = new();
    private bool _failed;
    private Assembly _assembly = null!;
    private Type _subject = null!;
    private static readonly List<object> PixelObservations = new();
    private static readonly Dictionary<string, (byte[] Imported, byte[] Source)> PixelFixtures = new(comparer: StringComparer.Ordinal);

    private static string Digest(string name)
    {
        return Convert.ToHexStringLower(inArray: SHA256.HashData(source: System.IO.File.ReadAllBytes(path:
                ProjectSettings.GlobalizePath(path: "res://subjects/" + name))));
    }

    private void Check(string id, bool pass, object detail)
    {
        _outcomes.Add(item: new { id, pass, detail });
        _failed |= !pass;
    }

    private object Invoke(object instance, string method, params object[] args)
    {
        return _subject.GetMethod(name: method, bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(obj: instance, parameters: args)!;
    }

    private static JsonElement Read(string file)
    {
        using var document = JsonDocument.Parse(json: Godot.FileAccess.GetFileAsString(path: "res://" + file));
        return document.RootElement.Clone();
    }

    private object Create(Node2D root, string identity, JsonElement? art = null, JsonElement? assembly = null,
            JsonElement? guards = null, Func<string, byte[]?>? read = null)
    {
        object gateway = Activator.CreateInstance(type: _assembly.GetType(name:
                "KingdomZombies.Proofs.MainThreadGateway", throwOnError: true)!, nonPublic: true)!;
        read ??= file => Godot.FileAccess.GetFileAsBytes(path: "res://" + file);
        return Activator.CreateInstance(type: _subject, bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null, args: new object[] { root, gateway,
                    art ?? Read(file: "content/packs/defenses.json"),
                    assembly ?? Read(file: "content/world/defense-assemblies.json"),
                    guards ?? Read(file: "content/packs/guards.json"), identity, read }, culture: CultureInfo.InvariantCulture)!;
    }

    private static void Select(Node2D root, string control, int index)
    {
        OptionButton selector = root.GetNode<OptionButton>(path: "Controls/" + control);
        selector.Select(idx: index);
        selector.EmitSignal(signal: OptionButton.SignalName.ItemSelected, args: new Variant[] { index });
    }

    private static string Key(Node2D root)
    {
        return root.GetNode<Sprite2D>(path: "ArtScroll/Content/Art/Structure").GetMeta(name: "part_key").AsString();
    }

    private async Task Run()
    {
        GetTree().Root.Size = new Vector2I(x: 1280, y: 720);
        AssemblyLoadContext context = AssemblyLoadContext.GetLoadContext(assembly: GetType().Assembly)!;
        Assembly core = context.LoadFromAssemblyPath(assemblyPath: ProjectSettings.GlobalizePath(path:
                "res://subjects/KingdomZombies.Core.dll"));
        _assembly = context.LoadFromAssemblyPath(assemblyPath: ProjectSettings.GlobalizePath(path:
                "res://subjects/KingdomZombies.dll"));
        _subject = _assembly.GetType(name: "KingdomZombies.Inspection.BuildingPreview", throwOnError: true)!;
        Check(id: "owning-alc", pass: AssemblyLoadContext.GetLoadContext(assembly: core) == context
                && AssemblyLoadContext.GetLoadContext(assembly: _assembly) == context, detail: context.ToString());
        Check(id: "actual-production-type", pass: !_subject.IsPublic && StringComparer.Ordinal.Equals(
                x: _subject.Assembly.GetName().Name, y: "KingdomZombies"), detail: _subject.FullName!);
        var root = new Node2D { Name = "First" };
        var neighbour = new Node2D { Name = "Neighbour" };
        AddChild(node: root);
        AddChild(node: neighbour);
        object preview = Create(root: root, identity: "defense.first");
        object other = Create(root: neighbour, identity: "defense.neighbour");
        Invoke(instance: preview, method: "Attach");
        Invoke(instance: other, method: "Attach");
        Matrix(root: root);
        Guards(root: root, preview: preview);
        Boundaries(root: root, preview: preview);
        ConstructorControls(root: root);
        string neighbourKey = Key(root: neighbour);
        await Task.Run(action: () =>
        {
            bool rejected = false;
            try { Invoke(instance: preview, method: "Tick", args: new object[] { 0.1d }); }
            catch (TargetInvocationException error) { rejected = error.InnerException is InvalidOperationException; }
            Check(id: "thread-guard", pass: rejected, detail: "actual captured owner thread");
        });
        Invoke(instance: preview, method: "Detach");
        Check(id: "independent-owner", pass: root.GetChildCount() == 0 && StringComparer.Ordinal.Equals(
                x: Key(root: neighbour), y: neighbourKey), detail: neighbour.GetChildCount());
        bool late = false;
        try { Invoke(instance: preview, method: "Tick", args: new object[] { 0.1d }); }
        catch (TargetInvocationException error) { late = StringComparer.Ordinal.Equals(x:
                error.InnerException?.Message, y: "BUILDING_DETACHED"); }
        Check(id: "late-result-fence", pass: late, detail: "closed owner rejects results");
        Invoke(instance: other, method: "Detach");
        root.QueueFree();
        neighbour.QueueFree();
        PixelControls();
        await ToSignal(source: GetTree(), signal: SceneTree.SignalName.ProcessFrame);
    }

    private void Matrix(Node2D root)
    {
        Check(id: "native-controls", pass: root.GetNode<OptionButton>(path: "Controls/Tier").ItemCount == 6
                && root.GetNode<OptionButton>(path: "Controls/Sample").ItemCount == 22, detail: "actual native signals");
        JsonElement artPack = Read(file: "content/packs/defenses.json");
        for (int index = 0; index < 6; index++)
        {
            for (int state = 0; state < 22; state++)
            {
                Select(root: root, control: "Tier", index: index);
                Select(root: root, control: "Sample", index: state);
                bool dead = state is 17 or 20;
                bool frame = state is 6 or 7 or 18;
                bool site = state < 6;
                int tier = index % 3 + 1;
                int functional = site || dead || frame ? 0 : state == 12 ? Math.Min(val1: 3, val2: tier + 1) : tier;
                int guards = index < 3 ? 0 : functional;
                string prefix = (index < 3 ? "wall-" : "tower-") + (functional == 0 ? tier : functional)
                        .ToString(provider: CultureInfo.InvariantCulture);
                string expected = site ? index < 3 ? "site-wall" : "site-tower" : frame
                        ? prefix + (state == 7 ? "-frame-damaged" : "-frame") : dead ? prefix + "-ruin"
                                : state == 9 ? prefix + "-damage-light" : state is 10 or 13 or 15 or 16
                                        ? prefix + "-damage-heavy" : prefix;
                string id = index.ToString(provider: CultureInfo.InvariantCulture) + "-" + state.ToString(
                        provider: CultureInfo.InvariantCulture);
                Node2D art = root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
                Check(id: "functional-" + id, pass: root.GetMeta(name: "functional_tier").AsInt32() == functional,
                        detail: functional);
                Check(id: "source-part-" + id, pass: StringComparer.Ordinal.Equals(x: Key(root: root), y: expected)
                        && Pixels(sprite: art.GetNode<Sprite2D>(path: "Structure"), pack: artPack, key: expected), detail: expected);
                Check(id: "guards-" + id, pass: root.GetMeta(name: "guard_count").AsInt32() == guards
                        && art.GetChildren().OfType<Node2D>().Count(predicate: node => node.HasMeta(name: "ammo"))
                                == guards, detail: guards);
                Check(id: "overlay-" + id, pass: art.HasNode(path: "PaidWork") == (!site && !dead && !frame
                        && state is 11 or 13 or 15 or 16 or 21), detail: art.HasNode(path: "PaidWork"));
                Check(id: "geometry-" + id, pass: art.GetNode<StaticBody2D>(path: "FactionBlocker").CollisionLayer
                        == (functional > 0 && index < 3 ? 2u : 0u) && art.HasNode(path: "WorkAnchor"),
                        detail: "independent collider/blocker/work anchors");
            }
        }
    }

    private static bool Pixels(Sprite2D sprite, JsonElement pack, string key)
    {
        JsonElement part = pack.GetProperty(propertyName: "parts").EnumerateArray().Single(predicate: row =>
                StringComparer.Ordinal.Equals(x: row.GetProperty(propertyName: "key").GetString(), y: key));
        JsonElement rect = part.GetProperty(propertyName: "atlasRect");
        if (sprite.Texture is not AtlasTexture texture || texture.Region != new Rect2(x: rect[0].GetSingle(),
                y: rect[1].GetSingle(), width: rect[2].GetSingle(), height: rect[3].GetSingle())) { return false; }
        using Image image = sprite.Texture.GetImage();
        JsonElement atlas = pack.GetProperty(propertyName: "atlas");
        string sourcePath = atlas.GetProperty(propertyName: "path").GetString()!;
        byte[] png = Godot.FileAccess.GetFileAsBytes(path: sourcePath);
        using var source = new Image();
        Error loaded = source.LoadPngFromBuffer(buffer: png);
        byte[] raw = source.GetData();
        int width = rect[2].GetInt32(), height = rect[3].GetInt32();
        byte[] crop = new byte[width * height * 4];
        for (int y = 0; y < height; y++)
        {
            Array.Copy(sourceArray: raw, sourceIndex: ((rect[1].GetInt32() + y) * source.GetWidth()
                    + rect[0].GetInt32()) * 4, destinationArray: crop, destinationIndex: y * width * 4,
                    length: width * 4);
        }
        bool authenticated = loaded == Error.Ok && source.GetFormat() == Image.Format.Rgba8
                && source.GetWidth() == atlas.GetProperty(propertyName: "size")[0].GetInt32()
                && source.GetHeight() == atlas.GetProperty(propertyName: "size")[1].GetInt32()
                && StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: png)),
                        y: atlas.GetProperty(propertyName: "sha256").GetString())
                && StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: raw)),
                        y: atlas.GetProperty(propertyName: "rgbaSha256").GetString())
                && StringComparer.Ordinal.Equals(x: Convert.ToHexStringLower(inArray: SHA256.HashData(source: crop)),
                        y: part.GetProperty(propertyName: "rgbaSha256").GetString());
        int alphaDifferences = 0, visibleRgbDifferences = 0, invisibleRgbDifferences = 0;
        bool dimensions = image.GetWidth() == width && image.GetHeight() == height;
        if (dimensions)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color a = image.GetPixel(x: x, y: y), b = source.GetPixel(
                            x: rect[0].GetInt32() + x, y: rect[1].GetInt32() + y);
                    if (a.A != b.A) { alphaDifferences++; }
                    bool rgb = a.R != b.R || a.G != b.G || a.B != b.B;
                    if (rgb && (a.A != 0 || b.A != 0)) { visibleRgbDifferences++; }
                    if (rgb && a.A == 0 && b.A == 0) { invisibleRgbDifferences++; }
                }
            }
        }
        PixelObservations.Add(item: new { key, sourcePath, authenticated, dimensions, width, height,
                importedWidth = image.GetWidth(), importedHeight = image.GetHeight(),
                sourceFormat = source.GetFormat().ToString(), importedFormat = image.GetFormat().ToString(),
                alphaDifferences, visibleRgbDifferences, invisibleRgbDifferences,
                sourcePartSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: crop)),
                importedSha256 = Convert.ToHexStringLower(inArray: SHA256.HashData(source: image.GetData())) });
        PixelFixtures.TryAdd(key: sourcePath, value: (image.GetData(), crop));
        return authenticated && dimensions && image.GetFormat() == Image.Format.Rgba8
                && ExactPixels(actual: image.GetData(), expected: crop);
    }

    private static bool ExactPixels(byte[] actual, byte[] expected)
    {
        if (actual.Length != expected.Length || expected.Length == 0 || expected.Length % 4 != 0) { return false; }
        for (int index = 0; index < expected.Length; index += 4)
        {
            if (actual[index + 3] != expected[index + 3]) { return false; }
            if (expected[index + 3] != 0 && (actual[index] != expected[index]
                    || actual[index + 1] != expected[index + 1] || actual[index + 2] != expected[index + 2]))
            {
                return false;
            }
        }
        return true;
    }

    private void PixelControls()
    {
        foreach (var fixture in PixelFixtures)
        {
            byte[] original = fixture.Value.Imported, source = fixture.Value.Source;
            int visible = Enumerable.Range(start: 0, count: source.Length / 4)
                    .First(predicate: index => source[index * 4 + 3] != 0) * 4;
            int invisible = Enumerable.Range(start: 0, count: source.Length / 4)
                    .First(predicate: index => source[index * 4 + 3] == 0) * 4;
            string id = fixture.Key.Contains(value: "buildings", comparisonType: StringComparison.Ordinal)
                    ? "structure" : "guard";
            byte[] alpha = (byte[])original.Clone(), rgb = (byte[])original.Clone(), hidden = (byte[])original.Clone();
            alpha[visible + 3] ^= 1;
            rgb[visible] ^= 1;
            hidden[invisible] ^= 1;
            Check(id: "pixel-control-" + id + "-alpha", pass: ExactPixels(actual: original, expected: source)
                    && !ExactPixels(actual: alpha, expected: source), detail: "one exact alpha byte changed");
            Check(id: "pixel-control-" + id + "-visible-rgb", pass: ExactPixels(actual: original, expected: source)
                    && !ExactPixels(actual: rgb, expected: source), detail: "one exact visible RGB byte changed");
            Check(id: "pixel-control-" + id + "-invisible-rgb", pass: original[invisible + 3] == 0
                    && hidden[invisible + 3] == 0 && hidden[invisible] != original[invisible]
                    && ExactPixels(actual: hidden, expected: source), detail: "RGB-only alpha-zero change is invisible");
        }
    }

    private void Guards(Node2D root, object preview)
    {
        Select(root: root, control: "Tier", index: 5);
        Select(root: root, control: "Sample", index: 8);
        Invoke(instance: preview, method: "ResetDefense");
        Node2D art = root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        JsonElement pack = Read(file: "content/packs/guards.json");
        for (int i = 0; i < 3; i++)
        {
            Invoke(instance: preview, method: "AimGuard", args: new object[] { i, i == 1 ? -30f : 30f,
                    (i - 1) * 12f, true });
            Node2D guard = art.GetNode<Node2D>(path: "guard-" + i.ToString(provider: CultureInfo.InvariantCulture));
            Check(id: "independent-facing-" + i.ToString(provider: CultureInfo.InvariantCulture), pass:
                    guard.GetMeta(name: "facing").AsInt32() == (i == 1 ? -1 : 1)
                    && guard.GetNode<Node2D>(path: "Facing").Rotation == 0
                    && guard.HasNode(path: "Facing/Aim/ReleaseOrigin"), detail: guard.GetMeta(name: "aim_angle").AsDouble());
        }
        foreach (double delta in new[] { 0d, 0.125, 0.125, 0.125, 0.125, 0.125 })
        {
            Invoke(instance: preview, method: "Tick", args: new object[] { delta });
            foreach (int i in new[] { 0, 1, 2 })
            {
                Node2D guard = art.GetNode<Node2D>(path: "guard-" + i.ToString(provider: CultureInfo.InvariantCulture));
                foreach (string path in new[] { "Facing/Torso", "Facing/Aim/ArmBow", "Arrow" })
                {
                    Sprite2D sprite = guard.GetNode<Sprite2D>(path: path);
                    Check(id: "guard-source-" + _outcomes.Count.ToString(provider: CultureInfo.InvariantCulture),
                            pass: Pixels(sprite: sprite, pack: pack, key: sprite.GetMeta(name: "part_key").AsString()),
                            detail: path);
                }
            }
        }
        Invoke(instance: preview, method: "ResetDefense");
        Invoke(instance: preview, method: "Tick", args: new object[] { 0.3d });
        art = root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
        Sprite2D arrow = art.GetNode<Sprite2D>(path: "guard-0/Arrow");
        Vector2 origin = arrow.Position;
        Invoke(instance: preview, method: "AimGuard", args: new object[] { 0, -30f, 20f, true });
        Invoke(instance: preview, method: "Tick", args: new object[] { 0.1d });
        Check(id: "captured-arrow-origin", pass: arrow.Visible && arrow.Position.X > origin.X
                && Math.Abs(value: arrow.Position.Y - origin.Y) < 0.001f, detail: arrow.Position);
        Invoke(instance: preview, method: "Tick", args: new object[] { 0.3d });
        Check(id: "arrow-expiry", pass: !arrow.Visible, detail: "0.375 lifetime, no recoverable arrow");
        Invoke(instance: preview, method: "AimGuard", args: new object[] { 0, 30f, 0f, false });
        Check(id: "idle-aim-cancels-arrow", pass: !arrow.Visible, detail: "stable torso; shoulder-led arm/bow");
    }

    private void Boundaries(Node2D root, object preview)
    {
        Select(root: root, control: "Tier", index: 4);
        Invoke(instance: preview, method: "Restore", args: new object[] { 2, 3, 0.999d, false });
        Check(id: "H12-old-guards-0.999", pass: root.GetMeta(name: "guard_count").AsInt32() == 2
                && StringComparer.Ordinal.Equals(x: Key(root: root), y: "tower-2"), detail: "old functional tier");
        Invoke(instance: preview, method: "Restore", args: new object[] { 2, 3, 1d, false });
        Check(id: "H12-new-guards-only-complete", pass: root.GetMeta(name: "guard_count").AsInt32() == 3
                && StringComparer.Ordinal.Equals(x: Key(root: root), y: "tower-3"), detail: "new completed tier");
        Invoke(instance: preview, method: "Restore", args: new object[] { 2, 3, 0.5d, true });
        for (int i = 0; i < 5; i++) { Invoke(instance: preview, method: "Tick", args: new object[] { 60d }); }
        Check(id: "H12-permanent-previous-tier", pass: root.GetMeta(name: "previous_completed_tier").AsInt32() == 2
                && StringComparer.Ordinal.Equals(x: Key(root: root), y: "tower-2-ruin")
                && root.GetMeta(name: "guard_count").AsInt32() == 0, detail: "300seconds, persistent ruin");
        long events = root.GetMeta(name: "destruction_events").AsInt64();
        Invoke(instance: preview, method: "Restore", args: new object[] { 2, 3, 0.5d, true });
        Check(id: "H12-quiet-restore", pass: root.GetMeta(name: "destruction_events").AsInt64() == events,
                detail: "no past destruction/attack events");
        Select(root: root, control: "Sample", index: 18);
        Check(id: "H12-rebuild-frame-empty", pass: root.GetMeta(name: "guard_count").AsInt32() == 0
                && root.GetMeta(name: "previous_completed_tier").AsInt32() == 2, detail: Key(root: root));
        Select(root: root, control: "Sample", index: 19);
        Check(id: "H12-rebuild-previous-tier", pass: root.GetMeta(name: "guard_count").AsInt32() == 2,
                detail: Key(root: root));
        Empty(root: root, preview: preview);
        Reachability(root: root);
    }

    private void Empty(Node2D root, object preview)
    {
        Select(root: root, control: "Tier", index: 5);
        foreach (int sample in new[] { 0, 17, 18 })
        {
            Select(root: root, control: "Sample", index: sample);
            Check(id: "H14-empty-no-loot-" + sample.ToString(provider: CultureInfo.InvariantCulture), pass:
                    root.GetMeta(name: "guard_count").AsInt32() == 0 && root.GetMeta(name: "corpse_count").AsInt32() == 0
                    && root.GetMeta(name: "loot_count").AsInt32() == 0 && root.GetMeta(name: "inventory_count").AsInt32() == 0
                    && root.GetMeta(name: "population_use").AsInt32() == 0, detail: Key(root: root));
        }
        Select(root: root, control: "Sample", index: 19);
        foreach (int facing in new[] { -1, 1 })
        {
            Invoke(instance: preview, method: "ResetDefense");
            for (int i = 0; i < 3; i++) { Invoke(instance: preview, method: "AimGuard", args:
                    new object[] { i, facing * 30f, i * 3f, true }); }
            for (int cycle = 0; cycle < 64; cycle++)
            {
                Invoke(instance: preview, method: "Tick", args: new object[] { 1.4d });
                Node2D art = root.GetNode<Node2D>(path: "ArtScroll/Content/Art");
                Check(id: "H14-intrinsic-" + facing.ToString(provider: CultureInfo.InvariantCulture) + "-"
                        + cycle.ToString(provider: CultureInfo.InvariantCulture), pass:
                        Enumerable.Range(start: 0, count: 3).All(predicate: i => art.GetNode(
                                path: "guard-" + i.ToString(provider: CultureInfo.InvariantCulture))
                                        .GetMeta(name: "release_count").AsInt64() == cycle + 1),
                        detail: "three independent guards, unlimited intrinsic arrows, no inventory/refill/reload");
            }
        }
        Invoke(instance: preview, method: "Restore", args: new object[] { 3, 3, 0d, false });
        Check(id: "H14-quiet-arrow-restore", pass: !root.GetNode<Sprite2D>(
                path: "ArtScroll/Content/Art/guard-0/Arrow").Visible, detail: "no prior flight replay");
    }

    private void Reachability(Node2D root)
    {
        for (int tier = 0; tier < 6; tier++)
        {
            Select(root: root, control: "Tier", index: tier);
            Select(root: root, control: "Sample", index: 8);
            for (int zoom = 0; zoom < 4; zoom++)
            {
                Select(root: root, control: "Zoom", index: zoom);
                Control content = root.GetNode<Control>(path: "ArtScroll/Content");
                bool inside = root.GetNode<Node2D>(path: "ArtScroll/Content/Art")
                        .FindChildren(pattern: "*", type: "Sprite2D", recursive: true, owned: false)
                        .OfType<Sprite2D>().Where(predicate: sprite => sprite.Visible).All(predicate: sprite =>
                        new Rect2(position: Vector2.Zero, size: content.CustomMinimumSize).Encloses(
                                b: content.GetGlobalTransform().AffineInverse() * sprite.GetGlobalTransform() * sprite.GetRect()));
                Check(id: "zoom-reachable-" + tier.ToString(provider: CultureInfo.InvariantCulture) + "-"
                        + zoom.ToString(provider: CultureInfo.InvariantCulture), pass: inside, detail: content.CustomMinimumSize);
            }
        }
        Select(root: root, control: "Zoom", index: 0);
    }

    private void ConstructorControls(Node2D root)
    {
        JsonElement controls = Read(file: "controls.json");
        foreach (JsonElement row in controls.EnumerateArray())
        {
            JsonNode art = JsonNode.Parse(json: Read(file: "content/packs/defenses.json").GetRawText())!;
            JsonNode assembly = JsonNode.Parse(json: Read(file: "content/world/defense-assemblies.json").GetRawText())!;
            JsonNode guard = JsonNode.Parse(json: Read(file: "content/packs/guards.json").GetRawText())!;
            string pack = row.GetProperty(propertyName: "pack").GetString()!;
            JsonNode node = StringComparer.Ordinal.Equals(x: pack, y: "art") ? art
                    : StringComparer.Ordinal.Equals(x: pack, y: "guard") ? guard : assembly;
            string[] segments = row.GetProperty(propertyName: "path").GetString()!.Split(separator: '/');
            foreach (string segment in segments[..^1])
            {
                node = node is JsonArray ? node[int.Parse(s: segment, provider: CultureInfo.InvariantCulture)]!
                        : node[segment]!;
            }
            JsonNode? value = JsonNode.Parse(json: row.GetProperty(propertyName: "value").GetRawText());
            if (node is JsonArray array)
            {
                array[int.Parse(s: segments[^1], provider: CultureInfo.InvariantCulture)] = value;
            }
            else { node[segments[^1]] = value; }
            bool rejected = false;
            string detail = "accepted";
            try { Create(root: root, identity: "negative", art: JsonSerializer.SerializeToElement(value: art),
                    assembly: JsonSerializer.SerializeToElement(value: assembly),
                    guards: JsonSerializer.SerializeToElement(value: guard)); }
            catch (TargetInvocationException error)
            {
                detail = error.InnerException?.Message ?? "missing error";
                rejected = StringComparer.Ordinal.Equals(x: detail, y: "DEFENSE_"
                        + row.GetProperty(propertyName: "code").GetString());
                if (StringComparer.Ordinal.Equals(x: row.GetProperty(propertyName: "id").GetString(), y: "missing-state"))
                {
                    rejected &= error.InnerException is System.IO.InvalidDataException;
                }
            }
            Check(id: "constructor-" + row.GetProperty(propertyName: "id").GetString(), pass: rejected, detail: detail);
        }
        foreach (string media in new[] { "assets/buildings/defenses.png", "assets/characters/structural-guard.png" })
        {
            foreach (string fault in new[] { "missing", "hash", "dimensions" })
            {
                bool rejected = false;
                try { Create(root: root, identity: "media-negative", read: file =>
                {
                    byte[] value = Godot.FileAccess.GetFileAsBytes(path: "res://" + file);
                    if (!StringComparer.Ordinal.Equals(x: file, y: media)) { return value; }
                    if (StringComparer.Ordinal.Equals(x: fault, y: "missing")) { return null; }
                    value[StringComparer.Ordinal.Equals(x: fault, y: "dimensions") ? 16 : value.Length - 1] ^= 1;
                    return value;
                }); }
                catch (TargetInvocationException error) { rejected = error.InnerException is System.IO.InvalidDataException; }
                Check(id: "media-" + media.Replace(oldChar: '/', newChar: '-') + "-" + fault,
                        pass: rejected, detail: fault);
            }
        }
    }
}

`;
const GUARD_VERIFY=String.raw`import sys,pathlib,json,zipfile,io,hashlib
from PIL import Image
root=pathlib.Path(sys.argv[1]);p=json.loads((root/'src/godot-game/content/packs/guards.json').read_bytes());b=(root/'src/godot-game/assets/characters/structural-guard.png').read_bytes();sha=lambda b:hashlib.sha256(b).hexdigest()
assert p['id']=='guards' and p['atlas']['sha256']==sha(b),'GUARD_ATLAS'
atlas=Image.open(io.BytesIO(b)).convert('RGBA');assert list(atlas.size)==[256,64] and p['atlas']['rgbaSha256']==sha(atlas.tobytes()),'GUARD_PIXELS'
assert set(atlas.getchannel('A').tobytes())<={0,255},'GUARD_ALPHA'
assert p['sourceBundle']['path']=='production/phase4/main-buildings.zip' and p['sourceBundle']['sha256']==sha((root/p['sourceBundle']['path']).read_bytes()),'GUARD_LINEAGE'
parts={r['key']:r for r in p['parts']};assert len(parts)==11,'GUARD_PARTS'
def crop(k):
 r=parts[k];x,y,w,h=r['atlasRect'];assert 0<=x<=256-w and 0<=y<=64-h and w>0 and h>0,'GUARD_BOUNDS';im=atlas.crop((x,y,x+w,y+h));assert sha(im.tobytes())==r['rgbaSha256'],'GUARD_PART_RGBA';return im
with zipfile.ZipFile(root/p['sourceBundle']['path']) as z:
 for i,source in enumerate(p['sourceBundle']['frames']):
  original=z.read(source['member']);assert sha(original)==source['sha256'],'GUARD_SOURCE_BYTES';im=Image.open(io.BytesIO(original)).convert('RGBA');body=crop('torso-'+str(i));arms=crop('arms-'+str(i));assert body.size==im.size==arms.size and body.getbbox() and arms.getbbox(),'GUARD_SPLIT'
  assert not any(a[3] and b[3] for a,b in zip(body.get_flattened_data(),arms.get_flattened_data())),'GUARD_DOUBLE_BOW'
  combined=body.copy();combined.alpha_composite(arms);assert combined.tobytes()==im.tobytes(),'GUARD_RECONSTRUCTION'
  anchor=source['anchor'][0]
  for yy in range(im.height):
   for xx in range(im.width):
    r,g,b,a=im.getpixel((xx,yy));arm=6<=yy<=15 and (xx<anchor-2 or xx>anchor+1 or r>b+15);assert arms.getpixel((xx,yy))==((r,g,b,a) if arm else (0,0,0,0)),'GUARD_MASK'
  assert source['time']==i*0.125,'GUARD_TIME'
 arrow=Image.open(io.BytesIO(z.read('parts/arrow.png'))).convert('RGBA');assert arrow.tobytes()==crop('arrow').tobytes(),'GUARD_ARROW'
assert p['timing']=={'start':0,'stop':0.625,'sampleRate':8,'interval':1.4,'release':0.25,'arrowLifetime':0.375,'arrowSpeed':72},'GUARD_TIMING'
assert sha(p['recipe']['code'].encode())==p['recipe']['sha256'],'GUARD_RECIPE'
assert p['ownership']['mainAttachments']=={'tier':3,'count':2,'profileTier':1,'healthOwner':'main','phase':[0,0.7]},'GUARD_MAIN_TWO'
a=json.loads((root/'src/godot-game/content/world/defense-assemblies.json').read_bytes())
assert [(r['id'],len(r['sockets'])) for r in a['tierDefinitions']]==[('wall-1',0),('wall-2',0),('wall-3',0),('tower-1',1),('tower-2',2),('tower-3',3)],'DEFENSE_GUARDS'
assert [r['id'] for r in a['scenarios']]==['H12-defense-boundaries','H14-structural-empty'] and all(r['status']=='actual input/appearance pending04-32' for r in a['scenarios']),'DEFENSE_ACTUAL_PENDING'
print(json.dumps({'parts':11,'splitFrames':5,'reconstructedFrames':5,'arrowReused':True,'samples':len(a['samples']),'heldOut':['H12-defense-boundaries','H14-structural-empty']}))

`;
Object.assign(module.exports,{STATE_OWN,stateFiles,NEGATIVES,GUARD_VERIFY,HARNESS,isolated,nativeState,state});

