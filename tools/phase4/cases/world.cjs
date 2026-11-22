'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
const MAP_FILES=['src/godot-game/assets/world/environment.png','src/godot-game/content/packs/world.json','src/godot-game/content/world/map.json','production/phase4/world.zip','tools/phase4/cases/world.cjs'];
const MAP_INPUTS=[...MAP_FILES,'production/phase4/common-selected.zip','src/godot-game/content/packs/common.json','tools/phase4/extract-media.py','src/godot-game/content/config/food-wildlife.json'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
function exact(a,b,code){need(equal(a,b),code);}
function finite(value,min,max,code){need(typeof value==='number'&&Number.isFinite(value)&&value>=min&&value<=max,code);}
function ids(rows,code){need(Array.isArray(rows)&&rows.length<=512&&new Set(rows.map(r=>r.id)).size===rows.length,code);for(const r of rows)need(/^[a-zA-Z0-9_.-]{1,100}$/.test(r.id),code);}
function rectangle(r,code){need(Array.isArray(r)&&r.length===4&&r.every(Number.isFinite)&&r[2]>0&&r[3]>0&&r.every(x=>Math.abs(x)<=5120),code);}
function color(c,code){need(Array.isArray(c)&&c.length===4&&c.every(x=>typeof x==='number'&&x>=0&&x<=1),code);}
function validate(pack,map){
 exact([pack.schemaVersion,pack.id,pack.version,map.schemaVersion,map.id,map.version],[1,'world',1,1,'world-map',1],'WORLD_ID');
 exact(pack.obligations,['BL-15',...Array.from({length:11},(_,i)=>'EN-'+String(i+1).padStart(2,'0'))],'WORLD_OBLIGATIONS');
 exact([pack.logicalPixelsPerWorldPixel,pack.designUnitPixels,map.pixelsPerUnit,map.units],[1,8,8,'u'],'WORLD_DENSITY');
 exact(pack.atlas.size,[2048,1024],'WORLD_ATLAS_SIZE');need(pack.atlas.path==='res://assets/world/environment.png'&&/^[a-f0-9]{64}$/.test(pack.atlas.sha256),'WORLD_ATLAS_PATH');
 need(pack.parts.length===42&&new Set(pack.parts.map(p=>p.key)).size===42,'WORLD_PART_IDS');const parts=new Map(pack.parts.map(p=>[p.key,p]));
 for(const p of pack.parts){need(/^[a-z0-9-]{1,64}$/.test(p.key)&&p.member==='parts/'+p.key+'.png','WORLD_PART_ID');rectangle(p.atlasRect,'WORLD_PART_RECT');const [x,y,w,h]=p.atlasRect;
  need([x,y,w,h].every(Number.isInteger)&&x>=0&&y>=0&&x+w<=2048&&y+h<=1024&&equal([w,h],p.size),'WORLD_PART_BOUNDS');need(/^[a-f0-9]{64}$/.test(p.sha256)&&/^[a-f0-9]{64}$/.test(p.rgbaSha256)&&p.visiblePixels>0,'WORLD_PART_HASH');}
 exact(map.edges,[-320,320],'WORLD_EDGES');exact(map.groundY,0,'WORLD_GROUND');
 exact(map.terrain,[{id:'ground',from:-320,to:320,height:0,collisionPolygon:[[-320,0],[320,0],[320,6],[-320,6]],navigation:[[-320,0],[320,0]],art:'ground',artTileWidth:16}],'WORLD_TERRAIN');
 for(const group of ['portals','walls','towers','clearings','plants','wildlifeZones','rallies','dressing'])ids(map[group],'WORLD_DUPLICATE '+group);
 const baseline={portals:[100,190,280],walls:[25,75,155,245],towers:[21,71,151,241],clearings:[50,125,215],wildlifeZones:[45,130,230]};
 for(const [group,distances]of Object.entries(baseline)){
  const expected=['left','right'].flatMap((side,index)=>distances.map((x,i)=>({id:side+'.'+({portals:'portal',walls:'wall',towers:'tower',clearings:'clearing',wildlifeZones:'wildlife'}[group])+'.'+(i+1),side,x:(index?1:-1)*x})));
  exact(map[group].map(r=>({id:r.id,side:r.side,x:group==='wildlifeZones'?r.center:r.x})),expected,'WORLD_BASELINE '+group);
 }
 for(const p of map.portals){exact(p.art,['portal-back','portal-front'],'WORLD_PORTAL_ART');exact(p.blocker,{rect:[-1,-64,2,70],blocks:['survivor'],passes:['zombie'],extendsAllJumpHeights:true},'WORLD_PORTAL_BLOCKER');exact(p.projectileRect,[-1,-9,2,9],'WORLD_PROJECTILE_GEOMETRY');need(p.interactionRadius===3&&p.revealRadius===24&&p.sourceIndex>=1&&p.sourceIndex<=3,'WORLD_PORTAL_SOURCE');}
 for(const site of [...map.walls,...map.towers]){
  const index=Number(site.id.at(-1));exact(site.clearing,index===1?null:site.side+'.clearing.'+(index-1),'WORLD_CLEARING_REFERENCE');
  exact(site.requiresDestroyedPortals,[100,190,280].flatMap((x,i)=>x<Math.abs(site.x)?[site.side+'.portal.'+(i+1)]:[]),'WORLD_PORTAL_REFERENCE');
  const wall=site.id.includes('.wall.');exact(site.collisionRect,wall?[-2,-5,4,5]:[-1,-6,2,6],'WORLD_SITE_GEOMETRY');exact(site.factionBlocker,wall?'zombie':null,'WORLD_SITE_FACTION');need(site.artBoundsDoNotCollide&&site.interactionRadius===3,'WORLD_SITE_BOUNDS');
 }
 for(const c of map.clearings){exact(c.trees,[{offset:[-3,0],species:'oak'},{offset:[2,0],species:'birch'}],'WORLD_TREE_GEOMETRY');need(c.extent===6&&c.blocksWalking===false&&c.foodIndependent===true,'WORLD_FOOD_INDEPENDENCE');const i=Number(c.id.at(-1))+1;exact(c.unlocks,[c.side+'.wall.'+i,c.side+'.tower.'+i],'WORLD_CLEARING_UNLOCK');}
 exact(map.plants.map(p=>[p.id,p.kind,p.x,p.capacity]),[['left.berry.1','berry',-10,8],['left.berry.2','berry',-50,8],['left.fruit.1','fruit',-16,6],['right.berry.1','berry',10,8],['right.berry.2','berry',50,8],['right.fruit.1','fruit',16,6]],'WORLD_PLANT_BASELINE');
 for(const p of map.plants){need(p.clearingRemoves===false&&p.refill==='daily-dawn-once'&&p.foodUnit==='one-item-per-stock','WORLD_PLANT_STOCK');need(parts.has(p.base)&&p.stockLayers.length===(p.kind==='berry'?4:2)&&p.stockLayers.every(x=>parts.has(x)),'WORLD_STOCK_REFERENCE');}
 exact(map.stockStates,[{id:'full',fraction:1},{id:'partial',fraction:0.5},{id:'empty',fraction:0},{id:'refilled',fraction:1}],'WORLD_STOCK_STATES');
 exact(map.clearingStates.map(s=>[s.id,s.stump,s.mark,s.rotation,s.duration]),[['intact',false,false,0,0],['marked-paid',false,true,0,0],['chopping',false,true,0,0.8],['falling-left',false,false,-Math.PI/2,0.65],['falling-right',false,false,Math.PI/2,0.65],['cleared-stump',true,false,0,0]],'WORLD_CLEARING_STATES');
 for(const zone of map.wildlifeZones){need(zone.width===15&&zone.retainExisting&&zone.visibleBorder===false,'WORLD_WILDLIFE');exact(zone.dailyBatch.map(a=>[a.kind,a.x]),[['rabbit',zone.center+(zone.side==='left'?3:-3)],['deer',zone.center+(zone.side==='left'?-3:3)],['rabbit',zone.center]],'WORLD_PREY_PLACEMENT');need(new Set(zone.dailyBatch.map(a=>a.id)).size===3&&zone.dailyBatch.every(a=>parts.has(a.art)&&a.x>=-320&&a.x<=320),'WORLD_PREY_REFERENCE');}
 exact(map.rallies.map(r=>[r.id,r.fallbackBorder,r.offsetInward]),[['left.rally',-12,5],['right.rally',12,-5]],'WORLD_RALLY_ID');
 for(const r of map.rallies){exact(r.states,['idle','ready','launching','assault-active','recalling','returning','no-target','insufficient-squad'],'WORLD_RALLY_STATES');need(r.borderSource==='outermost-completed-living-wall'&&r.cloth.length===6&&r.cloth.every(x=>parts.has(x)),'WORLD_RALLY_BINDING');}
 exact(map.rallySamples,[{id:'refuge',livingWalls:[],borders:[-12,12],rallies:[-9,9]},{id:'expanded',livingWalls:['left.wall.2','right.wall.3'],borders:[-75,155],rallies:[-70,150]},{id:'wall-lost',livingWalls:['left.wall.1','right.wall.1'],borders:[-25,25],rallies:[-20,20]}],'WORLD_MOVING_RALLIES');
 exact(map.layers.map(r=>[r.id,r.parallax,r.z]),[['sky',0,-100],['hills',0.12,-80],['middle-forest',0.35,-60],['near-forest',0.7,-30],['ground',1,0],['actors',1,5],['foreground',1,12]],'WORLD_PARALLAX');
 need(map.lighting.length===5&&map.lighting[0].time===0&&map.lighting.at(-1).time===1,'WORLD_LIGHTING');for(const l of map.lighting){finite(l.time,0,1,'WORLD_LIGHT_TIME');color(l.sky,'WORLD_SKY');color(l.tint,'WORLD_TINT');}
 need(map.lighting.every((l,i,a)=>!i||l.time>a[i-1].time)&&map.firelight.cosmeticOnly===true&&map.firelight.energy<=1,'WORLD_LIGHT_ORDER');color(map.firelight.color,'WORLD_FIRE_COLOR');
 exact([map.reveal.radius,map.reveal.shared,map.reveal.preserveExplored,map.reveal.liveEnemies],[24,true,true,false],'WORLD_REVEAL');
 for(const sample of map.reveal.samples){ids(sample.players,'WORLD_PLAYER_DUPLICATE');need(sample.explored.length>0&&sample.explored.length<=16,'WORLD_EXPLORED');for(const range of sample.explored)need(range.length===2&&range.every(Number.isFinite)&&range[0]>=-320&&range[1]<=320&&range[0]<range[1],'WORLD_EXPLORED_RANGE');for(const p of sample.players){need(p.id==='P'+p.number&&p.number>=1&&p.number<=4&&typeof p.name==='string'&&p.name.length>0&&p.name.length<64,'WORLD_PLAYER_ID');finite(p.x,-320,320,'WORLD_PLAYER_X');color(p.color,'WORLD_PLAYER_COLOR');}for(const id of sample.knownDefenses)need([...map.walls,...map.towers].some(x=>x.id===id),'WORLD_KNOWN_DEFENSE');for(const id of sample.destroyedPortals)need(map.portals.some(x=>x.id===id),'WORLD_KNOWN_PORTAL');}
 exact(map.reveal.samples[0].players.map(p=>[p.id,p.number,p.shape]),[['P1',1,'diamond'],['P2',2,'square'],['P3',3,'triangle'],['P4',4,'circle']],'WORLD_FOUR_IDENTITIES');
 need(known(map,map.reveal.samples[0]).portals.length===0,'WORLD_UNKNOWN_PORTAL');exact(known(map,map.reveal.samples[1]).portals,['left.portal.1','left.portal.2','right.portal.1'],'WORLD_SHARED_REVEAL');
 exact(map.interaction.targetKinds,['character','corpse','site','plant','stash','rally'],'WORLD_TARGET_KINDS');need(map.interaction.selection.steady&&map.interaction.selection.outlinePixels===1&&map.interaction.selectorUsesStableId,'WORLD_SELECTION');ids(map.interaction.overlapSample,'WORLD_OVERLAP_ID');
 need(map.native.root==='AuthoredWorld'&&map.native.filter==='nearest'&&map.native.atlas===pack.atlas.path&&map.native.nodes.length===287,'WORLD_NATIVE');ids(map.native.nodes,'WORLD_NODE_IDS');
 for(const n of map.native.nodes){need(n.type==='Sprite2D'&&n.parent==='.'&&parts.has(n.part),'WORLD_NODE_REFERENCE');need(n.position.length===2&&n.position.every(Number.isFinite)&&Math.abs(n.position[0])<=6000&&Math.abs(n.position[1])<=500,'WORLD_NODE_POSITION');finite(n.z,-100,20,'WORLD_NODE_Z');finite(n.rotation,-Math.PI/2,Math.PI/2,'WORLD_NODE_ROTATION');color(n.color,'WORLD_NODE_COLOR');}
 const node=id=>map.native.nodes.find(n=>n.id===id);
 for(let i=0;i<40;i++)exact([node('ground_'+i)?.part,node('ground_'+i)?.position],['ground',[-2560+i*128,-4]],'WORLD_NATIVE_GROUND');
 for(const p of map.plants){const [w,h]=parts.get(p.base).size;exact([node(p.id+'.base')?.part,node(p.id+'.base')?.position],[p.base,[p.x*8-Math.floor(w/2),-h]],'WORLD_NATIVE_PLANT');for(let i=0;i<p.stockLayers.length;i++)need(node(p.id+'.stock.'+i)?.part===p.stockLayers[i],'WORLD_NATIVE_STOCK');}
 for(const p of map.portals)for(let i=0;i<2;i++){const [w,h]=parts.get(p.art[i]).size;exact([node(p.id+'.'+i)?.part,node(p.id+'.'+i)?.position],[p.art[i],[p.x*8-Math.floor(w/2),-h]],'WORLD_NATIVE_PORTAL');}
 for(const c of map.clearings)for(let i=0;i<2;i++){const t=c.trees[i],h=parts.get('trunk-'+t.species).size[1];exact([node(c.id+'.trunk.'+i)?.part,node(c.id+'.trunk.'+i)?.position],['trunk-'+t.species,[c.x*8+t.offset[0]*8,-h]],'WORLD_NATIVE_CLEARING');}
 exact(pack.acceptance,{visual:'pending',owner:'04-32/04-33',completedObligations:[]},'WORLD_FALSE_ACCEPTANCE');return {parts:parts.size,nodes:map.native.nodes.length,portals:6,walls:8,towers:8,plants:6,clearings:6,wildlifeZones:6};
}
function known(map,sample){return {portals:map.portals.filter(p=>sample.explored.some(([lo,hi])=>p.x>=lo&&p.x<=hi)).map(p=>p.id),players:sample.players.map(p=>p.id),defenses:[...sample.knownDefenses]};}
const MAP_NEGATIVES=[
 ['wrong-portal','map','portals/0/x',-99,'WORLD_BASELINE'],['one-side','map','towers/7/x',240,'WORLD_BASELINE'],['duplicate-site','map','walls/1/id','left.wall.1','WORLD_DUPLICATE'],['ground-gap','map','terrain/0/to',319,'WORLD_TERRAIN'],['alpha-blocker','map','portals/0/blocker/rect',[-24,-70,49,70],'WORLD_PORTAL_BLOCKER'],['jump-bypass','map','portals/0/blocker/extendsAllJumpHeights',false,'WORLD_PORTAL_BLOCKER'],['zombie-blocked','map','portals/0/blocker/passes',[],'WORLD_PORTAL_BLOCKER'],['site-food-removal','map','clearings/0/foodIndependent',false,'WORLD_FOOD_INDEPENDENCE'],['plant-removed','map','plants/0/clearingRemoves',true,'WORLD_PLANT_STOCK'],['stock-empty-refill','map','stockStates/3/fraction',0,'WORLD_STOCK_STATES'],['one-fall-direction','map','clearingStates/3/rotation',Math.PI/2,'WORLD_CLEARING_STATES'],['zero-zone','map','wildlifeZones/1/width',0,'WORLD_WILDLIFE'],['delete-old-prey','map','wildlifeZones/1/retainExisting',false,'WORLD_WILDLIFE'],['stationary-rally','map','rallySamples/1/rallies',[-9,9],'WORLD_MOVING_RALLIES'],['unknown-portal-reveal','map','reveal/samples/0/explored',[[-124,124]],'WORLD_UNKNOWN_PORTAL'],['merge-player','map','reveal/samples/0/players/1/id','P1','WORLD_PLAYER_DUPLICATE'],['wrong-defense','map','reveal/samples/0/knownDefenses/0','left.tower.99','WORLD_KNOWN_DEFENSE'],['live-enemies','map','reveal/liveEnemies',true,'WORLD_REVEAL'],['lost-stock-layer','map','native/nodes/145/part','no-such-part','WORLD_NODE_REFERENCE'],['crop-outside','pack','parts/0/atlasRect/0',2048,'WORLD_PART_BOUNDS'],['duplicate-part','pack','parts/1/key','trunk-oak','WORLD_PART_IDS'],['bad-time','map','lighting/1/time',-1,'WORLD_LIGHT_TIME'],['fake-pass','pack','acceptance/visual','approved','WORLD_FALSE_ACCEPTANCE']
].map(([id,subject,path,value,code])=>({id,subject,path,value,code}));
function controls(pack,map){return MAP_NEGATIVES.map(c=>{const p=structuredClone(pack),w=structuredClone(map);let target=c.subject==='pack'?p:w;const keys=c.path.split('/');for(const key of keys.slice(0,-1))target=target[key];target[keys.at(-1)]=structuredClone(c.value);try{validate(p,w);}catch(error){need(error.message.includes(c.code),'WORLD_CONTROL_REASON '+c.id+' '+error.message);return {id:c.id,rejected:true,diagnostic:error.message};}throw new Error('WORLD_CONTROL_FALSE_PASS '+c.id);});}
function validateIntegration(pack,map,food){validate(pack,map);need(food.wildlife.livingCap===24&&food.wildlife.remainingPersist&&food.wildlife.excess==='skip-additions-never-delete-existing'&&food.wildlife.dawnBatch==='once-only-no-load-or-join-replay','WORLD_INTEGRATION_CAP');
 need(equal(food.wildlife.batchPerZone,{rabbit:2,deer:1}),'WORLD_INTEGRATION_BATCH');
 for(const zone of map.wildlifeZones){need(zone.dailyBatch.filter(a=>a.kind==='rabbit').length===2&&zone.dailyBatch.filter(a=>a.kind==='deer').length===1,'WORLD_INTEGRATION_SPECIES');
  for(const animal of zone.dailyBatch){need(Math.abs(animal.x-zone.center)<=zone.width/2,'WORLD_INTEGRATION_ZONE');need(map.native.nodes.some(n=>n.id===animal.id&&n.part===animal.art)&&map.native.nodes.some(n=>n.id===animal.id+'.shadow'&&n.part==='contact-shadow'),'WORLD_INTEGRATION_NATIVE_PREY');}}
 need(map.wildlifeZones.flatMap(z=>z.dailyBatch).length===18&&18<=food.wildlife.livingCap,'WORLD_INTEGRATION_BASELINE_CAP');return {livingCap:24,potentialDawnAnimals:18,retainsExisting:true,excessPolicy:food.wildlife.excess};}
function integrationControls(pack,map,food){const result=[];for(const [id,change]of [
 ['old-berry-capacity',(_,w)=>w.plants[0].capacity=12],['old-fruit-capacity',(_,w)=>w.plants[2].capacity=8],
 ['missing-second-rabbit',(_,w)=>w.wildlifeZones[0].dailyBatch.pop()],['second-deer',(_,w)=>w.wildlifeZones[0].dailyBatch[2].kind='deer'],
 ['prey-outside-zone',(_,w)=>w.wildlifeZones[0].dailyBatch[2].x=-90],['duplicate-prey-identity',(_,w)=>w.wildlifeZones[0].dailyBatch[2].id=w.wildlifeZones[0].dailyBatch[0].id],
 ['missing-native-second-prey',(_,w)=>w.native.nodes.find(n=>n.id==='left.rabbit.1.second').id='unbound.second'],
 ['delete-existing-prey',(_,w)=>w.wildlifeZones[0].retainExisting=false],['wrong-living-cap',(_,w,f)=>f.wildlife.livingCap=23],
 ['remove-excess-retention',(_,w,f)=>f.wildlife.excess='delete-existing'],['wrong-species-cap',(_,w,f)=>f.wildlife.batchPerZone.rabbit=1],
 ['old-wall-offset',(_,w)=>w.rallies[0].offsetInward=3],['old-wall-position',(_,w)=>w.rallySamples[1].rallies=[-72,152]],
 ['wall-offset-used-for-refuge',(_,w)=>w.rallySamples[0].rallies=[-7,7]]]){const p=structuredClone(pack),w=structuredClone(map),f=structuredClone(food);change(p,w,f);let failure;try{validateIntegration(p,w,f);}catch(error){failure=error;}need(failure,'WORLD_INTEGRATION_FALSE_PASS',id);result.push({id,rejected:true,diagnostic:failure.message,reboundSubject:{mapSha256:hash(JSON.stringify(w)),foodSha256:hash(JSON.stringify(f))}});}
 result.push({id:'world-integration-restored-positive',passed:true,...validateIntegration(pack,map,food)});return result;}
const VERIFY_WORLD=String.raw`import pathlib,sys,io,json,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
d=m.read_archive(root/'production/phase4/world.zip');p=m.loads(d['world.json']);w=m.loads(d['map.json']);a=Image.open(io.BytesIO(d['environment.png'])).convert('RGBA')
for n,f in [('world.json','content/packs/world.json'),('map.json','content/world/map.json'),('environment.png','assets/world/environment.png')]:m.need(d[n]==(root/'src/godot-game'/f).read_bytes(),'WORLD_PUBLISHED_BYTES '+n)
rows=m.loads(d['members.json']);m.need(len(rows)+1==len(d) and len({r['path'] for r in rows})==len(rows),'WORLD_INVENTORY')
for r in rows:m.need(r['path'] in d and r['bytes']==len(d[r['path']]) and r['sha256']==m.sha(d[r['path']]),'WORLD_MEMBER_HASH')
m.need(p['atlas']['sha256']==m.sha(d['environment.png']) and p['atlas']['rgbaSha256']==m.sha(a.tobytes()),'WORLD_ATLAS_HASH')
common=m.loads(d['sources/common.json']);records={r['id']:r for r in common['resources']};commonzip=m.read_archive(root/'production/phase4/common-selected.zip')
m.need(p['sources']['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and d['sources/common.json']==(root/'src/godot-game/content/packs/common.json').read_bytes(),'WORLD_COMMON_LINEAGE')
for part in p['parts']:
 im=Image.open(io.BytesIO(d[part['member']])).convert('RGBA');x,y,ww,hh=part['atlasRect'];m.need(list(im.size)==[ww,hh] and a.crop((x,y,x+ww,y+hh)).tobytes()==im.tobytes(),'WORLD_CROP_PIXELS')
 m.need(part['sha256']==m.sha(d[part['member']]) and part['rgbaSha256']==m.sha(im.tobytes()) and set(im.getchannel('A').tobytes())<={0,255} and all(aa or (r,g,b)==(0,0,0) for r,g,b,aa in im.get_flattened_data()),'WORLD_PART_PIXELS')
 m.need(part['sourceSha256']==m.sha(d[part['sourceMember']]),'WORLD_SOURCE_HASH')
 if part['source'] in records:
  record=records[part['source']];m.need(d[part['sourceMember']]==commonzip[record['member']] and record['pngSha256']==m.sha(d[part['sourceMember']]),'WORLD_EXACT_DONOR')
  for source in [record['source'],*record.get('dependencies',[])]:
   if 'rawMember' in source:m.need(d[source['rawMember']]==commonzip[source['rawMember']] and m.sha(d[source['rawMember']])==source['rawSha256'],'WORLD_RAW_SOURCE')
gen=p['sources']['generated'];master=Image.open(io.BytesIO(d[gen['master']])).convert('RGBA');m.need(gen['sha256']==m.sha(d[gen['master']]) and gen['promptSha256']==m.sha(d[gen['prompt']]) and master.getchannel('A').getextrema()==(0,255) and b'D-13 LOW-RESOLUTION' in d[gen['prompt']],'WORLD_GENERATED')
m.need(p['recipe']['sha256']==m.sha(d['recipes/build-world.py']),'WORLD_RECIPE_HASH');ns={'__name__':'retained_recipe','__file__':str(root/'.phase4/04-13-authoring/build-world.py')};exec(compile(d['recipes/build-world.py'],'retained-world.py','exec'),ns);rp,rw,replayed=ns['build'](d)
m.need(rp==p and rw==w,'WORLD_REPLAY_DEFINITIONS')
for n in ['world.json','map.json','environment.png',*[part['member'] for part in p['parts']]]:m.need(replayed[n]==d[n],'WORLD_REPLAY_BYTES '+n)
print(json.dumps({'parts':len(p['parts']),'nodes':len(w['native']['nodes']),'members':len(d),'rawSources':len([n for n in d if n.startswith('raw/')]),'replayed':45,'alpha':'binary; zero RGB outside; source raw PNGs retained'}))
`;
function scene(pack,map,{stock='full',clearing='intact',rally='refuge',reveal='unknown-portals',night=false}={}){
 validate(pack,map);const q=JSON.stringify,parts=new Map(pack.parts.map(p=>[p.key,p])),nodes=structuredClone(map.native.nodes),state=map.clearingStates.find(s=>s.id===clearing),fraction=map.stockStates.find(s=>s.id===stock)?.fraction,rallySample=map.rallySamples.find(s=>s.id===rally),sample=map.reveal.samples.find(s=>s.id===reveal);
 need(state&&fraction!==undefined&&rallySample&&sample,'WORLD_SCENE_SAMPLE');
 const resources=['[ext_resource type="Texture2D" path="res://assets/world/environment.png" id="1_world"]'];
 for(const [i,p]of pack.parts.entries())resources.push(`[sub_resource type="AtlasTexture" id="Part_${i}"]\natlas = ExtResource("1_world")\nregion = Rect2(${p.atlasRect.join(', ')})\nfilter_clip = true`);
 resources.push('[sub_resource type="RectangleShape2D" id="GroundShape"]\nsize = Vector2(5120, 48)');resources.push('[sub_resource type="RectangleShape2D" id="PortalShape"]\nsize = Vector2(16, 560)');resources.push('[sub_resource type="RectangleShape2D" id="EdgeShape"]\nsize = Vector2(8, 1024)');
 resources.push('[sub_resource type="NavigationPolygon" id="Navigation"]\nvertices = PackedVector2Array(-2560,-8,2560,-8,2560,0,-2560,0)\npolygons = [PackedInt32Array(0,1,2,3)]\nagent_radius = 0.0');
 let out=`[gd_scene load_steps=${resources.length+1} format=3]\n\n`+resources.join('\n\n')+'\n\n[node name="AuthoredWorld" type="Node2D"]\ntexture_filter = 1\nmetadata/resource_id = "world-map"\n';
 const add=(name,type,parent,properties)=>{out+=`\n[node name=${q(name)} type=${q(type)} parent=${q(parent)}]\n${properties}\n`;};
 for(const [i,n]of nodes.entries()){
  let visible=true;const plant=map.plants.find(p=>n.id.startsWith(p.id+'.stock.'));
  if(plant){const layer=Number(n.id.at(-1));visible=layer<Math.ceil(plant.stockLayers.length*fraction);}
  const c=map.clearings.find(c=>n.id.startsWith(c.id+'.'));if(c){if(state.stump)visible=false;else if(state.rotation){const tree=c.trees[Number(n.id.at(-1))],pivot=c.x*8+tree.offset[0]*8,dx=n.position[0]-pivot,dy=n.position[1];n.position=[pivot+dx*Math.cos(state.rotation)-dy*Math.sin(state.rotation),dx*Math.sin(state.rotation)+dy*Math.cos(state.rotation)];n.rotation=state.rotation;}}
  const rallyIndex=map.rallies.findIndex(r=>n.id.startsWith(r.id+'.'));if(rallyIndex>=0)n.position[0]=rallySample.rallies[rallyIndex]*8;
  const p=parts.get(n.part),partIndex=pack.parts.indexOf(p);const tint=night?n.color.map((v,i)=>v*map.lighting[3].tint[i]):n.color;
  add('Sprite_'+i,'Sprite2D','.',`texture = SubResource("Part_${partIndex}")\ncentered = false\nposition = Vector2(${n.position.join(', ')})\nrotation = ${n.rotation}\nz_index = ${n.z}\nvisible = ${visible}\nmodulate = Color(${tint.join(', ')})\nmetadata/resource_id = ${q(n.id)}\nmetadata/part = ${q(n.part)}`);
 }
 for(const c of map.clearings)for(let i=0;i<2;i++){
  const part=parts.get(state.stump?'stump':'tree-mark'),partIndex=pack.parts.indexOf(part);add('ClearState_'+c.side+'_'+c.id.at(-1)+'_'+i,'Sprite2D','.',`texture = SubResource("Part_${partIndex}")\ncentered = false\nposition = Vector2(${c.x*8+c.trees[i].offset[0]*8}, ${state.stump?-20:-60})\nvisible = ${state.stump||state.mark}\nmetadata/resource_id = ${q(c.id+'.state.'+i)}`);
 }
 add('Ground','StaticBody2D','.', 'collision_layer = 1\ncollision_mask = 1');add('Collision','CollisionShape2D','Ground','shape = SubResource("GroundShape")\nposition = Vector2(0, 24)');
 add('WalkableNavigation','NavigationRegion2D','.', 'navigation_polygon = SubResource("Navigation")\nnavigation_layers = 1');
 for(const p of map.portals){const name=p.id.replaceAll('.','_');add(name,'StaticBody2D','.',`position = Vector2(${p.x*8}, 0)\ncollision_layer = 2\ncollision_mask = 1\nmetadata/resource_id = ${q(p.id)}\nmetadata/faction_passes = "zombie"`);add('Blocker','CollisionShape2D',name,'shape = SubResource("PortalShape")\nposition = Vector2(0, -232)');}
 for(const [i,x]of map.edges.entries()){add('Edge_'+i,'StaticBody2D','.',`position = Vector2(${x*8+(i?4:-4)}, 0)\ncollision_layer = 4\ncollision_mask = 1`);add('Collision','CollisionShape2D','Edge_'+i,'shape = SubResource("EdgeShape")');}
 add('Minimap','Node2D','.', 'position = Vector2(0, 160)');
 for(const id of known(map,sample).portals){const p=map.portals.find(p=>p.id===id),partIndex=pack.parts.indexOf(parts.get(sample.destroyedPortals.includes(id)?'portal-map-ruin':'portal-map'));add(id.replaceAll('.','_'),'Sprite2D','Minimap',`texture = SubResource("Part_${partIndex}")\nposition = Vector2(${p.x}, 0)\nmetadata/resource_id = ${q(id)}`);}
 for(const p of sample.players){const partIndex=pack.parts.indexOf(parts.get('pointer-map'));add(p.id,'Sprite2D','Minimap',`texture = SubResource("Part_${partIndex}")\nposition = Vector2(${p.x}, 0)\nmodulate = Color(${p.color.join(', ')})\nmetadata/resource_id = ${q(p.id)}\nmetadata/player_number = ${p.number}\nmetadata/player_shape = ${q(p.shape)}\nmetadata/player_name = ${q(p.name)}`);}
 for(const id of sample.knownDefenses){const p=[...map.walls,...map.towers].find(p=>p.id===id),partIndex=pack.parts.indexOf(parts.get('tree-mark'));add(id.replaceAll('.','_'),'Sprite2D','Minimap',`texture = SubResource("Part_${partIndex}")\nposition = Vector2(${p.x}, 0)\nmetadata/resource_id = ${q(id)}`);}
 return out;
}
const MAP_PROBE=String.raw`extends SceneTree
var outcomes=[]
func check(id,ok,observed=null):
 outcomes.append({"id":id,"pass":ok,"observed":observed})
func _initialize():
 var pack=JSON.parse_string(FileAccess.get_file_as_string("res://world.json"))
 var map=JSON.parse_string(FileAccess.get_file_as_string("res://map.json"))
 var atlas=load("res://assets/world/environment.png")
 var image=atlas.get_image()
 for part in pack.parts:
  var r=part.atlasRect
  var crop=image.get_region(Rect2i(int(r[0]),int(r[1]),int(r[2]),int(r[3])))
  crop.convert(Image.FORMAT_RGBA8)
  var data=crop.get_data()
  for i in range(0,data.size(),4):
   if data[i+3]==0:
    data[i]=0
    data[i+1]=0
    data[i+2]=0
  var digest=HashingContext.new()
  digest.start(HashingContext.HASH_SHA256)
  digest.update(data)
  check("native-pixels-"+part.key,digest.finish().hex_encode()==part.rgbaSha256)
 for filename in ["full","partial","empty","refilled","marked-paid","chopping","falling-left","falling-right","cleared-stump","expanded","wall-lost","shared-exploration","night"]:
  var packed=load("res://"+filename+".tscn")
  check("packed-"+filename,packed is PackedScene)
  var owner=packed.instantiate()
  root.add_child(owner)
  var by_id={}
  var sprites=0
  for child in owner.get_children():
   if child is Sprite2D:
    sprites+=1
    by_id[str(child.get_meta("resource_id"))]=child
  check("sprites-"+filename,sprites==287+6*2,sprites)
  check("filter-"+filename,owner.texture_filter==CanvasItem.TEXTURE_FILTER_NEAREST)
  for i in range(40):
   var n=by_id["ground_"+str(i)]
   check("ground-"+filename+"-"+str(i),n.position==Vector2(-2560+i*128,-4) and n.texture.get_width()==128)
  for portal in map.portals:
   var body=owner.get_node(str(portal.id).replace(".","_"))
   var collision=body.get_node("Blocker")
   check("portal-"+filename+"-"+portal.id,body.position.x==portal.x*8 and body.collision_layer==2 and body.collision_mask==1 and collision.shape.size==Vector2(16,560) and collision.position==Vector2(0,-232) and body.get_meta("faction_passes")=="zombie")
  var stock=0
  var foodbases=0
  for key in by_id:
   if ".stock." in key and by_id[key].visible:stock+=1
   if (".berry." in key or ".fruit." in key) and key.ends_with(".base") and by_id[key].visible:foodbases+=1
  check("food-bases-"+filename,foodbases==6)
  check("stock-"+filename,stock==(0 if filename=="empty" else (10 if filename=="partial" else 20)))
  var mini=owner.get_node("Minimap")
  check("minimap-unknown-"+filename,mini.get_child_count()==(8 if filename=="shared-exploration" else 6))
  if filename!="shared-exploration":
   for p in ["P1","P2","P3","P4"]:check("identity-"+filename+"-"+p,mini.get_node(p).get_meta("resource_id")==p)
  var left=by_id["left.rally.pole"].position.x/8
  var right=by_id["right.rally.pole"].position.x/8
  var expected_left=-70 if filename=="expanded" else (-20 if filename=="wall-lost" else -9)
  var expected_right=150 if filename=="expanded" else (20 if filename=="wall-lost" else 9)
  check("rally-"+filename,left==expected_left and right==expected_right,[left,right])
  check("navigation-"+filename,owner.get_node("WalkableNavigation").navigation_polygon.get_polygon_count()==1)
  if filename in ["falling-left","falling-right"]:
   check("fall-direction-"+filename,is_equal_approx(by_id["left.clearing.1.trunk.0"].rotation,(-PI/2 if filename=="falling-left" else PI/2)))
  if filename=="cleared-stump":check("stumps-food-independent",not by_id["left.clearing.1.trunk.0"].visible and owner.get_node("ClearState_left_1_0").visible and foodbases==6)
  if filename=="night":check("cool-night",by_id["ground_0"].modulate.b>by_id["ground_0"].modulate.r)
  root.remove_child(owner)
  owner.free()
 var integration=[]
 var fixture=load("res://full.tscn").instantiate()
 root.add_child(fixture)
 var current={}
 for node in fixture.get_children():
  if node is Sprite2D and node.has_meta("resource_id"):current[str(node.get_meta("resource_id"))]=node
 var food=JSON.parse_string(FileAccess.get_file_as_string("res://food-wildlife.json"))
 for zone in map.wildlifeZones:
  var rabbits=0
  var deer=0
  for animal in zone.dailyBatch:
   rabbits+=1 if animal.kind=="rabbit" else 0
   deer+=1 if animal.kind=="deer" else 0
   var present=current.has(animal.id) and current.has(animal.id+".shadow")
   var expected_part=pack.parts.filter(func(p):return p.key==animal.art)[0]
   var expected_x=animal.x*8-floor(expected_part.size[0]/2.0)
   integration.append({"id":"r1-prey-"+animal.id,"pass":present and current[animal.id].position.x==expected_x and abs(animal.x-zone.center)<=zone.width/2.0,"observed":{"x":animal.x,"nativeX":current[animal.id].position.x if present else null}})
  integration.append({"id":"r1-zone-"+zone.id,"pass":rabbits==2 and deer==1 and zone.retainExisting,"observed":[rabbits,deer]})
 integration.append({"id":"r1-authoritative-wildlife-cap","pass":food.wildlife.livingCap==24 and food.wildlife.remainingPersist and food.wildlife.excess=="skip-additions-never-delete-existing" and map.wildlifeZones.size()*3<=24,"observed":food.wildlife})
 for plant in map.plants:integration.append({"id":"r1-stock-"+plant.id,"pass":plant.capacity==(8 if plant.kind=="berry" else 6),"observed":plant.capacity})
 for sample in map.rallySamples:
  for i in range(2):
   var expected=([-9,9][i] if sample.id=="refuge" else sample.borders[i]+(5 if i==0 else -5))
   integration.append({"id":"r1-rally-"+sample.id+"-"+str(i),"pass":sample.rallies[i]==expected,"observed":sample.rallies[i]})
 root.remove_child(fixture)
 fixture.free()
 var failed=outcomes.any(func(x):return not x.pass) or integration.any(func(x):return not x.pass)
 var file=FileAccess.open("res://native-results.json",FileAccess.WRITE)
 file.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":OS.get_environment("WORLD_PROOF_NONCE"),"outcomes":outcomes,"integrationOutcomes":integration,"failed":failed}))
 file.close()
 print("WORLD_MAP_NATIVE_COMPLETE "+OS.get_environment("WORLD_PROOF_NONCE")+" "+("FAIL" if failed else "PASS"))
 quit(1 if failed else 0)
`;
async function nativeMap(context,pack,map){
 const project=inside(context.directory,'native-world');fs.mkdirSync(project);write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Authored world resource proof"\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 write(project,'assets/world/environment.png',bytes(ROOT,MAP_FILES[0]));write(project,'world.json',bytes(ROOT,MAP_FILES[1]));write(project,'map.json',bytes(ROOT,MAP_FILES[2]));write(project,'food-wildlife.json',bytes(ROOT,'src/godot-game/content/config/food-wildlife.json'));
 const samples=[['full',{}],['partial',{stock:'partial'}],['empty',{stock:'empty'}],['refilled',{stock:'refilled'}],...['marked-paid','chopping','falling-left','falling-right','cleared-stump'].map(c=>[c,{clearing:c}]),['expanded',{rally:'expanded'}],['wall-lost',{rally:'wall-lost'}],['shared-exploration',{reveal:'shared-exploration'}],['night',{night:true}]];
 for(const [id,options]of samples)write(project,id+'.tscn',Buffer.from(scene(pack,map,options)));write(project,'probe.gd',Buffer.from(MAP_PROBE));
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];const archive=inside(ROOT,'.phase1/cache/'+pin.file);need(hash(fs.readFileSync(archive))===pin.sha256,'WORLD_ARCHIVE_PIN');
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),entry=pin.directory+'/'+path.basename(executable);
 const auth=await runner.command(context,'world-engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); rows=[r for r in z.infolist() if r.filename==sys.argv[2]]; assert len(rows)==1 and 0<rows[0].file_size<=268435456; print(hashlib.sha256(z.read(rows[0])).hexdigest())',archive,entry]);need(hash(fs.readFileSync(executable))===auth.stdout.trim(),'WORLD_ENGINE_PIN');
 write(context.directory,'engine.json',{...tool,consoleExecutable:tool.executable,executable,sha256:auth.stdout.trim(),archiveSha256:pin.sha256,archiveEntry:entry});
 const env={...process.env,WORLD_PROOF_NONCE:context.nonce};await runner.command(context,'world-map-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});const child=await runner.command(context,'world-map-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 const result=document(project,'native-results.json');write(context.directory,'native-observation.json',result);need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.length===796&&result.outcomes.every(x=>x.pass)&&result.integrationOutcomes.length===37&&result.integrationOutcomes.every(x=>x.pass),'WORLD_NATIVE_RESULT');need(!/ERROR:|SCRIPT ERROR:|FATAL/i.test(child.stdout+child.stderr)&&child.stderr.trim()===''&&child.stdout.includes('WORLD_MAP_NATIVE_COMPLETE '+context.nonce+' PASS'),'WORLD_NATIVE_CLEAN');return {pid:result.pid,outcomes:result.outcomes.length,integrationOutcomes:result.integrationOutcomes.length,samples:samples.length};
}
async function mapCase(context){
 const before=fingerprint(MAP_INPUTS);write(context.directory,'inputs.json',before);const pack=document(ROOT,MAP_FILES[1]),map=document(ROOT,MAP_FILES[2]);const result=validate(pack,map);const negative=controls(pack,map);write(context.directory,'negative-controls.json',negative);write(context.directory,'integration-controls.json',integrationControls(pack,map,document(ROOT,'src/godot-game/content/config/food-wildlife.json')));
 write(context.directory,'verify-world.py',Buffer.from(VERIFY_WORLD));const child=await runner.command(context,'world-map-source',PYTHON,['-B',inside(context.directory,'verify-world.py'),ROOT]);const source=JSON.parse(child.stdout);const native=await nativeMap(context,pack,map);need(equal(before,fingerprint(MAP_INPUTS)),'WORLD_SOURCE_CHANGED');return {...result,source,native,expectedRejected:negative.length,scope:'Source/replay and authenticated scriptless PackedScene resource inspection only; production WorldPreview,full hub/index,actual appearance/input acceptance remain separate'};
}
runner.registerCase('world.map',{group:'world',title:'Exact authored one-map composition,stock/clearing/reveal and moving rallies in actual native resources',importance:'Critical',rationale:'Wrong authored geometry,missing food,portal bypass or false exploration invalidates the entire world pack',needsGameWindow:false,runGroup:'G0-world',order:1,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08','AC-09','AC-24'],setup:'Tracked world/common bundles and atlas; pinned existing Python/Pillow and authenticated headless Godot; no donor directory',actions:['Replay42 source-bound art parts and212 retained members','Independently compare every mirrored baseline site and separate geometry/references','Reject23 deliberate semantic faults','Instantiate13 real PackedScenes with275 declared composition nodes and inspect native pixels,stock,clearing,food,reveal,identity and moving rally resources'],expected:'All exact world resources and independent malformed controls pass; real game windows and named judgment remain04-32/33',reset:'Fresh owned immutable invocation; each native owner removed and freed before next sample',evidence:['inputs.json','negative-controls.json','world-map-source.command.json','engine.json','native-observation.json','result.json']},mapCase);
module.exports={MAP_FILES,MAP_INPUTS,MAP_NEGATIVES,VERIFY_WORLD,MAP_PROBE,fingerprint,validate,known,controls,scene,nativeMap,mapCase};

const EFFECT_FILES=['src/godot-game/assets/effects/effects.png','src/godot-game/content/packs/effects-portals.json','src/godot-game/content/world/portal-assemblies.json','src/godot-game/inspection/WorldPreview.cs'];
function files(){const source=['Directory.Build.props','Directory.Build.targets','Directory.Packages.props','global.json','BannedSymbols.txt','.editorconfig'];function walk(directory){for(const entry of fs.readdirSync(inside(ROOT,directory),{withFileTypes:true})){const file=directory+'/'+entry.name;if(entry.isDirectory()&&!['bin','obj','.godot'].includes(entry.name))walk(file);else if(entry.isFile()&&entry.name.endsWith('.cs'))source.push(file);}}for(const directory of ['src/core','src/multiplayer-abstractions','src/godot-game','tools/code-rules'])walk(directory);return [...new Set([...MAP_INPUTS,...EFFECT_FILES,...require('./defenses.cjs').stateFiles(),...source])];}
const EFFECT_IDS=['arrow.friendly','arrow.enemy','arrow.tower',...['wood','metal','ground','flesh','armor','portal'].map(x=>'arrow.contact.'+x),...['fists','knife','club','machete','bat','dual'].map(x=>'melee.'+x),'work.wood','work.metal','tree.chop','tree.fall','collapse.wood','collapse.metal','fire.loop','smoke.loop','embers.loop','portal.warning','portal.spawn','portal.hit','portal.destroy','spit.projectile','spit.splash','bloater.burst','bloater.stain','character.hit','character.death','recovery.health','recovery.food','recovery.stamina','selection','placement.valid','placement.invalid','placement.collision','placement.legacy','ownership','crown.control','condition.broken','warning.nightfall','warning.retaliation','warning.portal-loss','outcome.victory','outcome.defeat','warning.connection-loss'];
const EFFECT_AUDIO_CATEGORIES={"arrow.friendly":"AU-06","arrow.enemy":"AU-06","arrow.tower":"AU-06","arrow.contact.wood":"AU-07","arrow.contact.metal":"AU-07","arrow.contact.ground":"AU-07","arrow.contact.portal":"AU-07","melee.fists":"AU-03","melee.knife":"AU-03","melee.club":"AU-03","melee.machete":"AU-03","melee.bat":"AU-03","melee.dual":"AU-03","tree.chop":"AU-13","tree.fall":"AU-13","collapse.wood":"AU-15","collapse.metal":"AU-15","fire.loop":"AU-28","portal.warning":"AU-21","portal.spawn":"AU-21","spit.splash":"AU-08","bloater.burst":"AU-29","recovery.food":"AU-19","crown.control":"AU-10","warning.nightfall":"AU-20","warning.retaliation":"AU-20","warning.portal-loss":"AU-22","outcome.victory":"AU-23","outcome.defeat":"AU-23","warning.connection-loss":"AU-24"};
function validateEffectAudio(pack,events=document(ROOT,'src/godot-game/content/audio/events.json')){
 const routes=new Map(events.events.flatMap(event=>event.routes.map(route=>[route.id,{event,route}])));
 function binding(value){const found=routes.get(value.route);need(found&&value.event===found.event.id&&value.category===found.event.category&&value.bus===found.event.bus&&value.bus===found.route.bus&&equal(value.variants,found.route.variants),'EFFECT_CANONICAL_AUDIO');}
 for(const [id,category]of Object.entries(EFFECT_AUDIO_CATEGORIES)){const effect=pack.effects.find(x=>x.id===id);need(effect&&effect.soundObligation===category&&effect.sound.startsWith(category+':'),'EFFECT_AUDIO_CATEGORY',id);}
 for(const effect of pack.effects){if(effect.audioRoute)binding(effect.audioRoute);if(effect.soundByBody){need(effect.sound===null&&effect.soundObligation===null,'EFFECT_CONTEXT_DEFAULT');for(let n=1;n<=8;n++){const actor=effect.soundByBody['CH-'+String(n).padStart(2,'0')];binding(actor);need(actor.category===(n===1?'AU-10':'AU-09'),'EFFECT_ACTOR_CATEGORY');}}}
 for(const id of ['recovery.health','recovery.stamina','portal.hit'])need(pack.effects.find(x=>x.id===id).sound===null,'EFFECT_QUIET_CONTEXT',id);
 need(pack.effects.find(x=>x.id==='bloater.stain').duration===20,'EFFECT_STAIN_LIFETIME');return {auditedCategories:30,actorBodies:8,stainSeconds:20};}
function effectAudioControls(pack){const controls=[];validateEffectAudio(pack);for(const [id,edit]of [
 ['wrong-valid-category',p=>{p.effects[0].soundObligation='AU-03';p.effects[0].sound='AU-03:bow.release';}],
 ['wrong-valid-route',p=>p.effects[0].audioRoute.route='melee.swing.club-bat'],['wrong-bus',p=>p.effects[0].audioRoute.bus='Ambience'],
 ['borrowed-variant',p=>p.effects[0].audioRoute.variants=['action.2864.full']],['wrong-actor-category',p=>p.effects.find(x=>x.id==='character.hit').soundByBody['CH-01'].category='AU-09'],
 ['audible-natural-regeneration',p=>p.effects.find(x=>x.id==='recovery.health').sound='AU-19:recovery.health'],['old-eight-second-stain',p=>p.effects.find(x=>x.id==='bloater.stain').duration=8]]){
 const copy=structuredClone(pack);edit(copy);let failure;try{validateEffectAudio(copy);}catch(error){failure=error;}need(failure,'EFFECT_AUDIO_FALSE_PASS',id);controls.push({id,rejected:true,diagnostic:failure.message});}
 controls.push({id:'effect-audio-current-restored',passed:true,...validateEffectAudio(pack)});return controls;}
function validateEffects(p,a,common){
 validateEffectAudio(p);
 exact([p.schemaVersion,p.id,p.version,a.schemaVersion,a.id,a.version],[1,'effects-portals',1,1,'portal-assemblies',1],'EFFECT_ID');
 exact(p.obligations,['BL-16',...Array.from({length:12},(_,i)=>'FX-'+String(i+1).padStart(2,'0'))],'EFFECT_OBLIGATIONS');
 exact(p.effects.map(r=>r.id),EFFECT_IDS,'EFFECT_ID_SET');ids(p.effects,'EFFECT_DUPLICATE');ids(a.portals,'EFFECT_PORTAL_DUPLICATE');
 const parts=new Map(p.parts.map(r=>[r.key,r]));need(parts.size===176&&p.parts.length===176,'EFFECT_PART_SET');exact(p.atlas.size,[2048,2048],'EFFECT_ATLAS_SIZE');
 for(const r of p.parts){rectangle(r.atlasRect,'EFFECT_PART_RECT');const [x,y,w,h]=r.atlasRect;need([x,y,w,h].every(Number.isInteger)&&x>=0&&y>=0&&x+w<=2048&&y+h<=2048,'EFFECT_PART_BOUNDS');const source=common.resources.find(s=>s.id===r.key);need(source&&source.member===r.sourceMember&&(source.pngSha256??source.sha256)===r.sourceSha256&&equal(source.source,r.source)&&equal(source.registration??null,r.registration),'EFFECT_PART_SOURCE');}
 for(const r of p.effects){need(parts.has(r.part),'EFFECT_PART_REFERENCE');finite(r.duration,0.01,r.id==='bloater.stain'?20:10,'EFFECT_DURATION');if(r.id==='bloater.stain')need(r.duration===20,'EFFECT_STAIN_LIFETIME');need(Number.isInteger(r.amount)&&r.amount>0&&r.amount<=12&&r.maxConcurrent===4,'EFFECT_AMOUNT');finite(r.speedPixelsPerSecond,0,200,'EFFECT_SPEED');finite(r.radiusPixels,0.1,24,'EFFECT_RADIUS');finite(r.emissionSeconds,0.001,r.duration,'EFFECT_EMISSION');color(r.color,'EFFECT_COLOR');need(['projectile','particles','stain','marker','warning'].includes(r.kind),'EFFECT_KIND');for(const f of ['loot','damage','collision','status'])need(r[f]===false,'EFFECT_MECHANIC');need(r.sound===null||p.soundEvents.some(s=>s.id===r.sound&&/^AU-(0[1-9]|[12][0-9]|30):[a-z.-]+$/.test(s.id)&&s.obligation===r.soundObligation),'EFFECT_SOUND');need(r.textureFrames.length===2&&r.textureFrames.every(x=>Number.isInteger(x)&&x>=1&&x<=16),'EFFECT_FLIPBOOK');if(r.kind==='projectile')need(r.firstContactOnly===true,'EFFECT_FIRST_CONTACT');}
 exact(p.portalClips,common.clips.filter(c=>c.name.startsWith('portal_')),'EFFECT_EXACT_CLIPS');for(const c of p.portalClips){need(c.callbacksExecutable===false&&c.events.length===0,'EFFECT_CALLBACK');for(const l of c.layers)for(const k of l.keys)if(k.sprite)need(parts.has(k.sprite),'EFFECT_CLIP_PART');}
 exact(a.portals.map(r=>[r.id,r.x,r.side,r.sourceIndex]),[['left.portal.1',-100,'left',1],['left.portal.2',-190,'left',2],['left.portal.3',-280,'left',3],['right.portal.1',100,'right',1],['right.portal.2',190,'right',2],['right.portal.3',280,'right',3]],'EFFECT_PORTAL_BASELINE');
 for(const r of a.portals){need(r.front==='common.s0.8291'&&r.back==='common.s0.7865'&&r.debris.length===13&&r.debris.every(x=>parts.has(x)),'EFFECT_PORTAL_PARTS');need(r.ruinPassable&&r.ruinPermanent&&!r.ruinActive&&!r.ruinRegrows,'EFFECT_PERMANENT_RUIN');exact(r.blocker,{rect:[-1,-64,2,70],blocks:['survivor'],passes:['zombie'],extendsAllJumpHeights:true},'EFFECT_BLOCKER');}
 exact(a.states,['closed','idle','warning','spawning','damaged','destroying','ruins'],'EFFECT_PORTAL_STATES');need(a.samples.length===24,'EFFECT_SAMPLES');ids(a.samples,'EFFECT_SAMPLE_ID');
 exact(a.restoration,{silent:true,replayPastEvents:false,ruinsPermanent:true,currentLoopsOnly:true,revokeOldGenerations:true},'EFFECT_SILENT_RESTORE');
 exact(p.limits,{total:24,perEffect:4,particleAmount:12,bloodParticles:0,dismemberment:false,projectilesBecomeLoot:false,crownBecomesLoot:false,guardLoot:false,cosmeticStainOnly:true},'EFFECT_LIMITS');
 need(p.portalShader.includes('preview_time')&&!/\bTIME\b/.test(p.portalShader),'EFFECT_MANUAL_SHADER');need(!equal(p.portalColor,p.spitColor)&&!equal(p.portalColor,p.stainColor),'EFFECT_SEPARATE_PALETTES');
 exact(p.acceptance,{visual:'pending',listening:'pending',owner:'04-32/04-33',completedObligations:[]},'EFFECT_FALSE_ACCEPTANCE');return {parts:176,effects:51,clips:6,portals:6,samples:24};
}
const EFFECT_NEGATIVES=[
 ['part-outside','effects','parts/0/atlasRect/0',2048,'WORLD_PART_BOUNDS'],['part-negative','effects','parts/0/atlasRect/2',-1,'WORLD_PART_BOUNDS'],['part-duplicate','effects','parts/1/key','common.r.8356','WORLD_DUPLICATE_ID'],['source-member','effects','parts/0/sourceMember','images/elsewhere.png','WORLD_PART_SOURCE'],['source-hash','effects','parts/0/sourceSha256','0'.repeat(64),'WORLD_PART_SOURCE'],['source-raw','effects','parts/0/source/rawSha256','0'.repeat(64),'WORLD_PART_SOURCE'],['duration-negative','effects','effects/0/duration',-1,'WORLD_EFFECT_TIMING'],['amount-fraction','effects','effects/0/amount',0.5,'WORLD_EFFECT_AMOUNT'],['effect-reference','effects','effects/0/part','unknown','WORLD_REFERENCE'],['damaging-stain','effects','effects/31/damage',true,'WORLD_EFFECT_MECHANIC'],['loot-arrow','effects','effects/0/loot',true,'WORLD_EFFECT_MECHANIC'],['sound-undefined','effects','effects/0/sound','AU-06:missing','WORLD_REFERENCE'],['clip-time','effects','portalClips/0/stop',9,'WORLD_CLIP_SOURCE'],['callbacks','effects','portalClips/0/callbacksExecutable',true,'WORLD_CLIP_SOURCE'],['shader-code','effects','portalShader','shader_type canvas_item;','WORLD_SHADER_SOURCE'],['blood','effects','limits/bloodParticles',20,'WORLD_EFFECT_LIMITS'],['atlas-path','effects','atlas/path','res://../outside.png','WORLD_MEDIA_PATH'],['node-reference','map','native/nodes/0/part','missing','WORLD_REFERENCE'],['site-position','map','walls/0/x',-24,'WORLD_SITE_POSITION'],['plant-removal','map','plants/0/clearingRemoves',true,'WORLD_PLANT_STOCK'],['food-position','map','plants/0/x',-11,'WORLD_FOOD_POSITION'],['stock-refill','map','stockStates/3/fraction',0,'WORLD_STOCK_STATES'],['terrain-gap','map','terrain/0/to',319,'WORLD_CONTINUOUS_GROUND'],['portal-identity','assembly','portals/0/side','right','WORLD_PORTAL_IDENTITY'],['portal-active-ruin','assembly','portals/0/ruinActive',true,'WORLD_RUIN_STATE'],['portal-regrowth','assembly','portals/0/ruinRegrows',true,'WORLD_RUIN_STATE'],['portal-padding-collider','assembly','portals/0/blocker/rect',[-24,-70,49,70],'WORLD_PORTAL_GEOMETRY'],['portal-jump','assembly','portals/0/blocker/extendsAllJumpHeights',false,'WORLD_PORTAL_JUMP'],['restore-replay','assembly','restoration/replayPastEvents',true,'WORLD_RESTORATION'],['sample-reference','assembly','samples/0/stock','missing','WORLD_REFERENCE'],['binding-path','assembly','sourceBindings/0/file','../common.json','WORLD_BINDING_SET']
].map(([id,subject,path,value,code])=>({id,subject,path,value,code}));
const VERIFY_EFFECTS=String.raw`import pathlib,sys,io,json,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);spec=importlib.util.spec_from_file_location('media',root/'tools/phase4/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
p=m.loads((root/'src/godot-game/content/packs/effects-portals.json').read_bytes());a=m.loads((root/'src/godot-game/content/world/portal-assemblies.json').read_bytes());common=m.loads((root/'src/godot-game/content/packs/common.json').read_bytes());d=m.read_archive(root/'production/phase4/common-selected.zip');atlas=Image.open(root/'src/godot-game/assets/effects/effects.png').convert('RGBA');records={r['id']:r for r in common['resources']}
m.need(p['atlas']['sha256']==m.digest(root/'src/godot-game/assets/effects/effects.png') and p['atlas']['rgbaSha256']==m.sha(atlas.tobytes()),'EFFECT_ATLAS_HASH')
m.need(p['sources']['commonBundleSha256']==m.digest(root/'production/phase4/common-selected.zip') and p['sources']['recipeSha256']==m.sha(p['sources']['authoringRecipe'].encode()),'EFFECT_SOURCE_RECIPE')
for part in p['parts']:
 r=records[part['key']];raw=d[r['member']];m.need(m.sha(raw)==part['sourceSha256'],'EFFECT_SOURCE_BYTES');im=Image.open(io.BytesIO(raw)).convert('RGBA');im.putalpha(im.getchannel('A').point(lambda v:255 if v>=128 else 0));data=bytearray(im.tobytes())
 for j in range(0,len(data),4):
  if not data[j+3]:data[j:j+3]=b'\x00\x00\x00'
 im=Image.frombytes('RGBA',im.size,bytes(data));x,y,w,h=part['atlasRect'];m.need(list(im.size)==[w,h] and im.tobytes()==atlas.crop((x,y,x+w,y+h)).tobytes() and m.sha(im.tobytes())==part['rgbaSha256'],'EFFECT_EXACT_PIXELS')
 for source in [r['source'],*r.get('dependencies',[])]:
  if 'rawMember' in source:m.need(m.sha(d[source['rawMember']])==source['rawSha256'],'EFFECT_RAW_SOURCE')
m.need(p['portalClips']==[c for c in common['clips'] if c['name'].startswith('portal_')],'EFFECT_EXACT_TIMING')
for row in a['sourceBindings']:m.need(m.digest(root/'src/godot-game'/row['file'])==row['sha256'],'EFFECT_CURRENT_BINDING')
print(json.dumps({'parts':len(p['parts']),'clips':len(p['portalClips']),'effects':len(p['effects']),'source':'pinned retained common-selected.zip','replayedPixels':176,'donorCallbacksExecuted':0,'generatedMasters':0}))
`;
const EFFECT_HARNESS=String.raw`using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;
namespace WorldEffectsProof;
public sealed partial class WorldHarness : Node
{
 private readonly List<object> outcomes=new();private Assembly game=null!;private Type subject=null!;private bool failed;
 public override async void _Ready(){try{await Run();}catch(Exception e){Check("unhandled",false,e.ToString());}finally{System.IO.File.WriteAllText(ProjectSettings.GlobalizePath("res://native-results.json"),JsonSerializer.Serialize(new{pid=System.Environment.ProcessId,nonce=System.Environment.GetEnvironmentVariable("WORLD_EFFECT_NONCE"),failed,outcomes,gameSha256=Digest("KingdomZombies.dll"),coreSha256=Digest("KingdomZombies.Core.dll")}));GD.Print("WORLD_EFFECT_NATIVE_COMPLETE "+System.Environment.GetEnvironmentVariable("WORLD_EFFECT_NONCE")+" "+(failed?"FAIL":"PASS"));GetTree().Quit(failed?1:0);}}
 private static string Digest(string file)=>Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://subjects/"+file))));
 private void Check(string id,bool pass,object detail){outcomes.Add(new{id,pass,detail});failed|=!pass;}
 private object Call(object p,string name,params object[] args)=>subject.GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(p,args)!;
 private T Get<T>(object p,string name)=>(T)subject.GetProperty(name,BindingFlags.Instance|BindingFlags.NonPublic)!.GetValue(p)!;
 private static JsonElement Read(string file){using var d=JsonDocument.Parse(System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://"+file)));return d.RootElement.Clone();}
 private Dictionary<string,byte[]> Sources(){var rows=Read("content/world/portal-assemblies.json").GetProperty("sourceBindings").EnumerateArray();var files=rows.ToDictionary(x=>x.GetProperty("file").GetString()!,x=>System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://"+x.GetProperty("file").GetString()!)),StringComparer.Ordinal);files["content/world/portal-assemblies.json"]=System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://content/world/portal-assemblies.json"));return files;}
 private object Open(Node2D root,Dictionary<string,byte[]>? sources=null){var data=sources??Sources();object gateway=Activator.CreateInstance(game.GetType("KingdomZombies.Proofs.MainThreadGateway",true)!,true)!;Func<string,byte[]?> read=file=>data.TryGetValue(file,out var b)?b:null;return subject.GetMethod("Open",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{root,gateway,read})!;}
 private static Node[] Descendants(Node root){var all=new List<Node>();foreach(var n in root.GetChildren()){all.Add(n);all.AddRange(Descendants(n));}return all.ToArray();}
 private static Node? Id(Node root,string id)=>Descendants(root).FirstOrDefault(n=>n.HasMeta("resource_id")&&StringComparer.Ordinal.Equals(n.GetMeta("resource_id").AsString(),id));
 private async Task Run(){var alc=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!;alc.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.Core.dll"));game=alc.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));subject=game.GetType("KingdomZombies.Inspection.WorldPreview",true)!;Check("production-internal",!subject.IsPublic&&subject.Assembly==game,subject.FullName!);
  var root=new Node2D();var neighbour=new Node2D();AddChild(root);AddChild(neighbour);var p=Open(root);var other=Open(neighbour);Call(p,"Attach");Call(other,"Attach");long neighbourEpoch=Get<long>(other,"Epoch");
  foreach(var sample in Read("content/world/portal-assemblies.json").GetProperty("samples").EnumerateArray()){string id=sample.GetProperty("id").GetString()!;Call(p,"Restore",id);Check("silent-"+id,Get<int>(p,"EventCount")==0&&Get<int>(p,"TransientCount")==0,Get<long>(p,"Epoch"));Check("food-"+id,Descendants(root).OfType<Sprite2D>().Count(n=>n.GetMeta("resource_id").AsString().EndsWith(".base",StringComparison.Ordinal)&&n.Visible)==6,"six persistent food plants");if(StringComparer.Ordinal.Equals(id,"portal.ruins")){Check("ruins-passable",!Descendants(root).OfType<StaticBody2D>().Any(n=>n.CollisionLayer==2),"zero active blockers");Check("ruins-six",Descendants(root).Count(n=>n.HasMeta("passable")&&n.GetMeta("passable").AsBool())==78,"13 debris pieces at each of6 source/side identities");Call(p,"Advance",30d);Check("ruins-permanent",!Descendants(root).OfType<Polygon2D>().Any()&&Get<int>(p,"EventCount")==0,"no interior/regrowth/death replay");}if(StringComparer.Ordinal.Equals(id,"plants.empty"))Check("empty-stock",!Descendants(root).OfType<Sprite2D>().Any(n=>n.GetMeta("resource_id").AsString().Contains(".stock.",StringComparison.Ordinal)&&n.Visible),"no stock layers");if(StringComparer.Ordinal.Equals(id,"clearings.falling-left")||StringComparer.Ordinal.Equals(id,"clearings.falling-right")){Call(p,"Advance",0.325d);var n=(Sprite2D)Id(root,"left.clearing.1.trunk.0")!;Check("half-fall-"+id,Math.Abs(n.Rotation-(id.EndsWith("left",StringComparison.Ordinal)?-Math.PI/4:Math.PI/4))<0.00001,n.Rotation);}if(StringComparer.Ordinal.Equals(id,"world.night")){Check("current-three-loops",Descendants(root).OfType<CpuParticles2D>().Count()==3&&Descendants(root).OfType<PointLight2D>().Count()==1,"manual-clock particles and restrained native light");Call(p,"Restore",id);Check("loop-idempotency",Descendants(root).OfType<CpuParticles2D>().Count()==3&&Get<int>(p,"EventCount")==0,"exact current owners");}if(StringComparer.Ordinal.Equals(id,"map.unknown-portals"))Check("unknown-portals-hidden",!Descendants(root).Any(n=>n.HasMeta("resource_id")&&n.GetMeta("resource_id").AsString().StartsWith("minimap.",StringComparison.Ordinal)&&n.GetMeta("resource_id").AsString().Contains(".portal.",StringComparison.Ordinal)),"no unknown marker");await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
  foreach(var row in Read("content/packs/effects-portals.json").GetProperty("effects").EnumerateArray()){Call(p,"Restore","portal.closed");string id=row.GetProperty("id").GetString()!;long epoch=Get<long>(p,"Epoch");bool accepted=(bool)Call(p,"Trigger",id,"left.portal.2",0f,-18f,32f,-10f,epoch);Check("trigger-"+id,accepted&&Get<int>(p,"EventCount")==1&&Get<int>(p,"TransientCount")==1,"explicit action only");Call(p,"Advance",0.1d);Check("resource-"+id,Descendants(root).Any(n=>n.HasMeta("resource_id")&&n.GetMeta("resource_id").AsString().StartsWith("effect."+id,StringComparison.Ordinal)),row.GetProperty("kind").GetString()!);Call(p,"Advance",row.GetProperty("duration").GetDouble());Check("expiry-"+id,Get<int>(p,"TransientCount")==0,"bounded authored duration");}
  Call(p,"Restore","portal.closed");long first=Get<long>(p,"Epoch");Call(p,"Trigger","spit.projectile","spitter",5f,-21f,60f,-21f,first);Check("first-solid-contact",(bool)Call(p,"Contact",18f,-21f,"wood",first),"captured mouth origin");Check("second-contact-rejected",!(bool)Call(p,"Contact",24f,-21f,"metal",first),"one splash only");Check("spit-no-hazard",!Descendants(root).OfType<Area2D>().Any()&&Get<int>(p,"TransientCount")==1,"cosmetic impact only");Call(p,"Reset");Check("reset-all-local",Get<double>(p,"Time")==0&&Get<int>(p,"EventCount")==0&&Get<int>(p,"TransientCount")==0&&Get<long>(p,"Epoch")!=first,"time/events/effects/origin authority revoked");Check("stale-trigger",!(bool)Call(p,"Trigger","character.hit","friendly",0f,0f,0f,0f,first),"old generation rejected");Check("neighbour-epoch",!(bool)Call(p,"Trigger","character.hit","friendly",0f,0f,0f,0f,neighbourEpoch),"another owner cannot authorize event");
  Call(p,"Restore","portal.closed");long epochNow=Get<long>(p,"Epoch");for(int i=0;i<4;i++)Check("repeat-limit-"+i,(bool)Call(p,"Trigger","melee.fists","friendly",0f,0f,0f,0f,epochNow),i);Check("repeat-limit-reject",!(bool)Call(p,"Trigger","melee.fists","friendly",0f,0f,0f,0f,epochNow),Get<int>(p,"TransientCount"));
  await Task.Run(()=>{bool rejected=false;try{Call(p,"Advance",0.1d);}catch(TargetInvocationException e){rejected=e.InnerException is InvalidOperationException;}Check("worker-thread-rejected",rejected,"synchronous gateway before native access");});
  ObserveCurrent(p,root);ObserveEffects(p,root);ObserveContactsAndLimits(p,root);ConstructorControls();Pixels();Call(p,"Close");Check("owned-teardown",root.GetChildCount()==0&&neighbour.GetChildCount()==1&&Get<long>(other,"Epoch")==neighbourEpoch,"neighbour unchanged");bool late=false;try{Call(p,"Advance",0d);}catch(TargetInvocationException e){late=e.InnerException is InvalidDataException;}Check("closed-owner-reject",late,"closed fence");Call(p,"Close");Call(other,"Close");var clearingProof=Open(root);Call(clearingProof,"Attach");ObserveClearingIdentities(clearingProof,root);Call(clearingProof,"Close");RemoveChild(root);root.Free();RemoveChild(neighbour);neighbour.Free();await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
 }
 private static string Text(JsonElement row,string key)=>row.GetProperty(key).GetString()!;
 private static bool Same(string? a,string? b)=>StringComparer.Ordinal.Equals(a,b);
 private static bool Near(double a,double b)=>Math.Abs(a-b)<0.00001;
 private static string[] ClearingGroups()=>new[]{"left.clearing.1","left.clearing.2","left.clearing.3","right.clearing.1","right.clearing.2","right.clearing.3"};
 private static string[] ClearingStateIds()=>ClearingGroups().SelectMany(id=>new[]{id+".state.0",id+".state.1"}).ToArray();
 private void ObserveClearingIdentities(object p,Node2D root){var map=Read("content/world/map.json");string[] clearingIds=ClearingStateIds();string[] decorativeIds={"left.prop.4","left.prop.9","left.prop.14","right.prop.4","right.prop.9","right.prop.14"};var groups=map.GetProperty("clearings").EnumerateArray().ToArray();Check("clearing-identity-source-baseline",groups.Select(row=>Text(row,"id")).Order(StringComparer.Ordinal).SequenceEqual(ClearingGroups().Order(StringComparer.Ordinal),StringComparer.Ordinal)&&groups.All(row=>row.GetProperty("trees").GetArrayLength()==2)&&clearingIds.Length==12,"six independent clearing groups,exactly two state identities each");Check("decorative-stump-source-baseline",map.GetProperty("dressing").EnumerateArray().Where(row=>Same(Text(row,"part"),"stump")).Select(row=>Text(row,"id")).Order(StringComparer.Ordinal).SequenceEqual(decorativeIds.Order(StringComparer.Ordinal),StringComparer.Ordinal),"six independent permanent decorative identities");
  foreach(var sample in Read("content/world/portal-assemblies.json").GetProperty("samples").EnumerateArray()){string id=Text(sample,"id");Call(p,"Restore",id);var sprites=Descendants(root).OfType<Sprite2D>().ToArray();bool stump=Same(Text(sample,"clearing"),"cleared-stump");foreach(string decorativeId in decorativeIds){var matches=sprites.Where(n=>n.HasMeta("resource_id")&&Same(n.GetMeta("resource_id").AsString(),decorativeId)).ToArray();Check("decorative-stump-"+id+"-"+decorativeId,matches.Length==1&&matches[0].Visible&&matches[0].HasMeta("part")&&Same(matches[0].GetMeta("part").AsString(),"stump"),"one permanent visible stump at the exact independent decorative identity");}Check("clearing-state-identity-set-"+id,sprites.Where(n=>n.HasMeta("resource_id")&&clearingIds.Contains(n.GetMeta("resource_id").AsString(),StringComparer.Ordinal)).Select(n=>n.GetMeta("resource_id").AsString()).Order(StringComparer.Ordinal).SequenceEqual((stump||Same(Text(sample,"clearing"),"marked-paid")||Same(Text(sample,"clearing"),"chopping")?clearingIds:Array.Empty<string>()).Order(StringComparer.Ordinal),StringComparer.Ordinal),"exactly twelve state identities for marked/chopping/cleared samples,zero otherwise");var expected=stump?decorativeIds.Concat(clearingIds):decorativeIds;Check("visible-stump-complete-identity-set-"+id,sprites.Where(n=>n.Visible&&n.HasMeta("part")&&Same(n.GetMeta("part").AsString(),"stump")).Select(n=>n.HasMeta("resource_id")?n.GetMeta("resource_id").AsString():"<missing-resource-id>").Order(StringComparer.Ordinal).SequenceEqual(expected.Order(StringComparer.Ordinal),StringComparer.Ordinal),"complete visible stump multiset rejects unknown,missing and duplicate identities");}
 }
 private void ObserveCurrent(object p,Node2D root){var map=Read("content/world/map.json");var assembly=Read("content/world/portal-assemblies.json");var effects=Read("content/packs/effects-portals.json");
  foreach(var sample in assembly.GetProperty("samples").EnumerateArray()){string id=Text(sample,"id");Call(p,"Restore",id);var nodes=Descendants(root);string stock=Text(sample,"stock");int expectedStock=Same(stock,"empty")?0:Same(stock,"partial")?10:20;Check("observed-stock-"+id,nodes.OfType<Sprite2D>().Count(n=>n.HasMeta("resource_id")&&n.GetMeta("resource_id").AsString().Contains(".stock.",StringComparison.Ordinal)&&n.Visible)==expectedStock,expectedStock);
   var rally=map.GetProperty("rallySamples").EnumerateArray().Single(r=>Same(Text(r,"id"),Text(sample,"rally")));for(int i=0;i<2;i++){string anchor=Text(map.GetProperty("rallies")[i],"id");Check("observed-rally-"+id+"-"+i,nodes.OfType<Sprite2D>().Where(n=>n.GetMeta("resource_id").AsString().StartsWith(anchor+".",StringComparison.Ordinal)).All(n=>Near(n.Position.X,rally.GetProperty("rallies")[i].GetDouble()*8)),rally.GetProperty("rallies")[i].GetDouble());}
   var reveal=map.GetProperty("reveal").GetProperty("samples").EnumerateArray().Single(r=>Same(Text(r,"id"),Text(sample,"reveal")));foreach(var player in reveal.GetProperty("players").EnumerateArray()){var sprite=(Sprite2D)Id(root,"minimap."+Text(player,"id"))!;Check("observed-player-"+id+"-"+Text(player,"id"),sprite is not null&&Near(sprite.Position.X,player.GetProperty("x").GetDouble())&&sprite.GetMeta("player_number").AsInt32()==player.GetProperty("number").GetInt32()&&Same(sprite.GetMeta("player_name").AsString(),Text(player,"name"))&&Same(sprite.GetMeta("player_shape").AsString(),Text(player,"shape")),"distinct current identity");}
   foreach(var portal in map.GetProperty("portals").EnumerateArray()){double x=portal.GetProperty("x").GetDouble();bool known=reveal.GetProperty("explored").EnumerateArray().Any(r=>r[0].GetDouble()<=x&&r[1].GetDouble()>=x);Check("observed-reveal-"+id+"-"+Text(portal,"id"),(Id(root,"minimap."+Text(portal,"id")) is not null)==known,known);}
   bool stump=Same(Text(sample,"clearing"),"cleared-stump");Check("observed-clearing-"+id,nodes.OfType<Sprite2D>().Count(n=>n.HasMeta("resource_id")&&ClearingStateIds().Contains(n.GetMeta("resource_id").AsString(),StringComparer.Ordinal)&&n.HasMeta("part")&&Same(n.GetMeta("part").AsString(),"stump")&&n.Visible)==(stump?12:0),"twelve ground endpoints only in cleared state");
  }
  foreach(string state in new[]{"closed","idle","warning","spawning","damaged","destroying"}){Call(p,"Restore","portal."+state);Call(p,"Advance",0.125d);foreach(var portal in assembly.GetProperty("portals").EnumerateArray()){string id=Text(portal,"id");float x=portal.GetProperty("x").GetSingle()*8;var front=(Sprite2D)Id(root,id+".front")!;var back=(Sprite2D)Id(root,id+".back")!;var interior=(Polygon2D)Id(root,id+".interior")!;var material=interior.Material as ShaderMaterial;Check("portal-native-"+state+"-"+id,front is not null&&back is not null&&Near(front.Position.X,x-24)&&Near(back.Position.X,x-22)&&Same(front.GetMeta("part").AsString(),"common.s0.8291")&&Same(back.GetMeta("part").AsString(),"common.s0.7865")&&front.ZIndex>interior.ZIndex&&interior.ZIndex>back.ZIndex,"six source/side layers");Check("portal-material-"+state+"-"+id,material is not null&&Same(material.Shader.Code,Text(effects,"portalShader"))&&Near(material.GetShaderParameter("preview_time").AsDouble(),0.125)&&interior.Visible!=Same(state,"closed"),"native shader with explicit clock");
    string clipId=Text(assembly.GetProperty("clips"),state);var clip=effects.GetProperty("portalClips").EnumerateArray().Single(c=>Same(Text(c,"id"),clipId));var layers=clip.GetProperty("layers").EnumerateArray().Where(l=>l.GetProperty("keys").EnumerateArray().Any(k=>k.TryGetProperty("sprite",out var value)&&value.ValueKind==JsonValueKind.String)).ToArray();for(int i=0;i<layers.Length;i++){var keys=layers[i].GetProperty("keys").EnumerateArray().Where(k=>k.TryGetProperty("sprite",out var value)&&value.ValueKind==JsonValueKind.String).ToArray();var expected=keys.Last(k=>k.GetProperty("time").GetDouble()<=0.125001);var sprite=(Sprite2D)Id(root,id+".clip."+i)!;var animation=sprite.GetMeta("native_animation").AsGodotObject() as Animation;Check("portal-timeline-"+state+"-"+id+"-"+i,Same(sprite.GetMeta("part").AsString(),Text(expected,"sprite"))&&animation is not null&&animation.GetTrackCount()==1&&animation.TrackGetKeyCount(0)==keys.Length&&Near(animation.Length,clip.GetProperty("stop").GetDouble()),"exact source clip native animation");}
   }}
  Call(p,"Restore","portal.destroying");Call(p,"Advance",8d);Check("destroy-to-permanent-ruins",Descendants(root).OfType<Sprite2D>().Count(n=>n.HasMeta("permanent")&&n.GetMeta("permanent").AsBool()&&!n.GetMeta("active").AsBool()&&n.GetMeta("passable").AsBool())==78&&!Descendants(root).OfType<Polygon2D>().Any()&&!Descendants(root).OfType<StaticBody2D>().Any(n=>n.CollisionLayer==2),"six permanent inert passable ruins after natural destruction duration");Call(p,"Advance",60d);Check("destroyed-never-regrows",!Descendants(root).OfType<Polygon2D>().Any()&&Get<int>(p,"EventCount")==0,"no resurrection or replay");
 }
 private void ObserveEffects(object p,Node2D root){foreach(var row in Read("content/packs/effects-portals.json").GetProperty("effects").EnumerateArray()){string id=Text(row,"id"),kind=Text(row,"kind");Call(p,"Restore","portal.closed");Call(p,"Trigger",id,"left.portal.2",0f,-18f,32f,-10f,Get<long>(p,"Epoch"));Call(p,"Advance",0.1d);var nodes=Descendants(root);var owner=nodes.First(n=>n.HasMeta("resource_id")&&n.GetMeta("resource_id").AsString().StartsWith("effect."+id,StringComparison.Ordinal));
   if(Same(kind,"particles")){var particles=owner as CpuParticles2D;var material=particles?.Material as CanvasItemMaterial;Check("particle-native-properties-"+id,particles is not null&&material is not null&&particles.Amount==row.GetProperty("amount").GetInt32()&&Near(particles.Lifetime,row.GetProperty("duration").GetDouble())&&particles.OneShot!=row.GetProperty("loop").GetBoolean()&&Near(particles.SpeedScale,0)&&particles.UseFixedSeed&&particles.Seed==13&&particles.Texture is AtlasTexture&&particles.Position==new Vector2(0,-18)&&Near(particles.InitialVelocityMax,row.GetProperty("speedPixelsPerSecond").GetDouble())&&Near(particles.Gravity.Y,row.GetProperty("gravityPixelsPerSecondSquared").GetDouble()),"actual owned manual-clock particle resource");Check("particle-native-material-"+id,material is not null&&material.ParticlesAnimation&&material.ParticlesAnimHFrames==row.GetProperty("textureFrames")[0].GetInt32()&&material.ParticlesAnimVFrames==row.GetProperty("textureFrames")[1].GetInt32()&&material.ParticlesAnimLoop==row.GetProperty("loop").GetBoolean()&&Near(owner.GetMeta("preview_time").AsDouble(),0.1)&&Same(owner.GetMeta("source_id").AsString(),"left.portal.2"),"native flipbook/material and captured source");}
   if(Same(kind,"projectile")){var sprite=owner as Sprite2D;var delta=new Vector2(32,8);var expected=new Vector2(0,-18)+delta.Normalized()*(float)Math.Min(delta.Length(),0.1*row.GetProperty("speedPixelsPerSecond").GetDouble());Check("projectile-native-flight-"+id,sprite is not null&&sprite.Texture is AtlasTexture&&sprite.Position.DistanceTo(expected)<0.00001&&Near(sprite.Rotation,delta.Angle()),"authored speed from captured origin");}
   if(Same(kind,"marker"))Check("marker-native-outline-"+id,nodes.OfType<Line2D>().Any(n=>n.Closed&&Near(n.Width,1)&&n.Points.Length==4)&&nodes.OfType<Sprite2D>().Any(n=>n.HasMeta("resource_id")&&Same(n.GetMeta("resource_id").AsString(),"effect."+id+".marker")),"steady one-pixel native outline and marker");
   if(Same(kind,"warning"))Check("warning-native-label-"+id,owner is Label label&&label.Text.Contains("left.portal.2",StringComparison.Ordinal)&&label.GetParent() is CanvasLayer layer&&layer.Layer==100,"visible high-priority native warning");
   if(Same(kind,"stain")){double time=row.GetProperty("duration").GetDouble()-row.GetProperty("fadeSeconds").GetDouble()/2;Call(p,"Advance",time-0.1);var sprite=Descendants(root).OfType<Sprite2D>().Single(n=>n.HasMeta("cosmetic_only"));Check("stain-native-half-fade",Near(sprite.Modulate.A,row.GetProperty("color")[3].GetDouble()/2)&&sprite.GetMeta("cosmetic_only").AsBool()&&!Descendants(root).OfType<Area2D>().Any(),"independent cosmetic-only native fade");}
   Call(p,"Reset");Check("native-reset-cancels-"+id,!Descendants(root).Any(n=>n.HasMeta("resource_id")&&n.GetMeta("resource_id").AsString().StartsWith("effect.",StringComparison.Ordinal))&&Get<int>(p,"EventCount")==0&&Get<int>(p,"TransientCount")==0&&Get<double>(p,"Time")==0,"all native transients retired");
  }}
 private void ObserveContactsAndLimits(object p,Node2D root){foreach(string shot in new[]{"arrow.friendly","arrow.enemy","arrow.tower","spit.projectile"})foreach(string material in new[]{"wood","metal","ground","flesh","armor","portal"}){Call(p,"Restore","portal.closed");long epoch=Get<long>(p,"Epoch");Call(p,"Trigger",shot,"spitter",5f,-21f,60f,-21f,epoch);bool first=(bool)Call(p,"Contact",18f,-21f,material,epoch);string expected=Same(shot,"spit.projectile")?"spit.splash":"arrow.contact."+material;var impact=Id(root,"effect."+expected) as CpuParticles2D;Check("contact-native-"+shot+"-"+material,first&&impact is not null&&impact.Position==new Vector2(18,-21)&&Same(impact.GetMeta("source_id").AsString(),"spitter")&&Get<int>(p,"TransientCount")==1&&!(bool)Call(p,"Contact",24f,-21f,material,epoch),"first contact emits one material splash and retires projectile");Call(p,"Reset");Check("reset-stale-contact-"+shot+"-"+material,!(bool)Call(p,"Contact",24f,-21f,material,epoch)&&Get<int>(p,"TransientCount")==0,"old origin/contact authority cleared");}
  Call(p,"Restore","portal.closed");long current=Get<long>(p,"Epoch");foreach(string effect in new[]{"melee.fists","melee.knife","melee.club","melee.machete","melee.bat","melee.dual"})for(int i=0;i<4;i++)Check("total-limit-fill-"+effect+"-"+i,(bool)Call(p,"Trigger",effect,"friendly",0f,0f,0f,0f,current),"within24total/fourperfamily");Check("total-limit-reject",Get<int>(p,"TransientCount")==24&&!(bool)Call(p,"Trigger","character.hit","friendly",0f,0f,0f,0f,current),"twenty-fifth distinct-family effect rejected");Call(p,"Restore","portal.closed");Check("restore-cancel-old-generation",Get<long>(p,"Epoch")!=current&&Get<int>(p,"TransientCount")==0&&Get<double>(p,"Time")==0&&Get<int>(p,"EventCount")==0&&!(bool)Call(p,"Trigger","character.hit","friendly",0f,0f,0f,0f,current),"quiet restore revokes prior owner generation");
 }
 private void ConstructorControls(){foreach(var control in Read("controls.json").EnumerateArray()){var data=Sources();string subjectName=control.GetProperty("subject").GetString()!;string file=StringComparer.Ordinal.Equals(subjectName,"effects")?"content/packs/effects-portals.json":StringComparer.Ordinal.Equals(subjectName,"map")?"content/world/map.json":"content/world/portal-assemblies.json";var json=JsonNode.Parse(data[file])!;string[] keys=control.GetProperty("path").GetString()!.Split('/');JsonNode parent=json;foreach(string key in keys.SkipLast(1))parent=parent is JsonArray?parent[int.Parse(key,System.Globalization.CultureInfo.InvariantCulture) ]!:parent[key]!;var value=JsonNode.Parse(control.GetProperty("value").GetRawText());if(parent is JsonArray a)a[int.Parse(keys[^1],System.Globalization.CultureInfo.InvariantCulture)]=value;else parent[keys[^1]]=value;data[file]=JsonSerializer.SerializeToUtf8Bytes(json);if(!StringComparer.Ordinal.Equals(subjectName,"assembly")){var outer=JsonNode.Parse(data["content/world/portal-assemblies.json"])!;foreach(var binding in outer["sourceBindings"]!.AsArray())if(StringComparer.Ordinal.Equals(binding!["file"]!.GetValue<string>(),file))binding["sha256"]=Convert.ToHexStringLower(SHA256.HashData(data[file]));data["content/world/portal-assemblies.json"]=JsonSerializer.SerializeToUtf8Bytes(outer);}var root=new Node2D();AddChild(root);bool rejected=false;string diagnostic="";try{Open(root,data);}catch(TargetInvocationException e){diagnostic=e.InnerException?.Message??e.Message;rejected=diagnostic.Contains(control.GetProperty("code").GetString()!,StringComparison.Ordinal);}Check("constructor-"+control.GetProperty("id").GetString(),rejected&&root.GetChildCount()==0,diagnostic);RemoveChild(root);root.Free();}
  var wrong=Sources();wrong["content/packs/effects-portals.json"]=wrong["content/packs/effects-portals.json"].Concat(new byte[]{32}).ToArray();var owner=new Node2D();AddChild(owner);bool hashRejected=false;try{Open(owner,wrong);}catch(TargetInvocationException e){hashRejected=e.InnerException?.Message.Contains("WORLD_BINDING_HASH",StringComparison.Ordinal)==true;}Check("separate-wrong-hash",hashRejected&&owner.GetChildCount()==0,"not a semantic negative");var restored=Open(owner);Call(restored,"Attach");Check("restored-positive",owner.GetChildCount()==1,"original owned input restored");Call(restored,"Close");RemoveChild(owner);owner.Free();
 }
 private void Pixels(){using Image image=ResourceLoader.Load<Texture2D>("res://assets/effects/effects.png")!.GetImage();foreach(var part in Read("content/packs/effects-portals.json").GetProperty("parts").EnumerateArray()){var r=part.GetProperty("atlasRect");using Image crop=image.GetRegion(new Rect2I(r[0].GetInt32(),r[1].GetInt32(),r[2].GetInt32(),r[3].GetInt32()));crop.Convert(Image.Format.Rgba8);byte[] bytes=crop.GetData();for(int i=0;i<bytes.Length;i+=4)if(bytes[i+3]==0){bytes[i]=0;bytes[i+1]=0;bytes[i+2]=0;}Check("pixels-"+part.GetProperty("key").GetString(),StringComparer.Ordinal.Equals(Convert.ToHexStringLower(SHA256.HashData(bytes)),part.GetProperty("rgbaSha256").GetString()),new{width=crop.GetWidth(),height=crop.GetHeight()});}}
}
`;
async function nativeEffects(context){
 const env=require('./defenses.cjs').isolated(),project=inside(context.directory,'native-effects');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const built=await runner.command(context,'world-effects-strict','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(built.stdout),'WORLD_EFFECTS_STRICT');
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});const closure=fs.readdirSync(original).filter(f=>fs.statSync(path.join(original,f)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'production-assemblies.json',closure);
 write(project,'WorldProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>WorldProof</AssemblyName><Nullable>enable</Nullable><EnableDynamicLoading>true</EnableDynamicLoading><TreatWarningsAsErrors>true</TreatWarningsAsErrors></PropertyGroup></Project>'));
 write(project,'WorldHarness.cs',Buffer.from(EFFECT_HARNESS));write(project,'controls.json',EFFECT_NEGATIVES);write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="World effects proof"\n[dotnet]\nproject/assembly_name="WorldProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of [...MAP_FILES,...EFFECT_FILES,'src/godot-game/content/packs/common.json'].filter(f=>f.startsWith('src/godot-game/')&&!f.endsWith('.cs')))write(project,file.slice('src/godot-game/'.length),bytes(ROOT,file));write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n var proof=load("res://WorldHarness.cs").new()\n root.add_child(proof)\n'));
 const audioInputs=[];for(const file of ['content/audio/events.json','content/packs/audio-actions.json','content/packs/audio-interface-ambience.json']){const binding=document(ROOT,'src/godot-game/content/world/portal-assemblies.json').sourceBindings.filter(row=>row.file===file);need(binding.length===1,'WORLD_AUDIO_FIXTURE_BINDING',file);const value=bytes(ROOT,'src/godot-game/'+file);need(hash(value)===binding[0].sha256,'WORLD_AUDIO_FIXTURE_SOURCE',file);write(project,file,value);need(hash(bytes(project,file))===binding[0].sha256,'WORLD_AUDIO_FIXTURE_COPY',file);audioInputs.push({file,bytes:value.length,sha256:binding[0].sha256});}write(context.directory,'audio-fixture-bindings.json',audioInputs);
 const build=await runner.command(context,'world-harness-build','dotnet',['build',inside(project,'WorldProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'WORLD_HARNESS_CLEAN');
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'],archive=inside(ROOT,'.phase1/cache/'+pin.file);need(hash(fs.readFileSync(archive))===pin.sha256,'WORLD_EFFECT_ARCHIVE');const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),entry=pin.directory+'/'+path.basename(executable);const auth=await runner.command(context,'world-effects-engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); r=[x for x in z.infolist() if x.filename==sys.argv[2]]; assert len(r)==1 and 0<r[0].file_size<=268435456; print(hashlib.sha256(z.read(r[0])).hexdigest())',archive,entry]);need(hash(fs.readFileSync(executable))===auth.stdout.trim(),'WORLD_EFFECT_ENGINE');write(context.directory,'engine.json',{executable,sha256:auth.stdout.trim(),archiveSha256:pin.sha256,archiveEntry:entry,consoleExecutable:tool.executable});
 await runner.command(context,'world-effects-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});const child=await runner.command(context,'world-effects-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env:{...env,WORLD_EFFECT_NONCE:context.nonce}});const result=document(project,'native-results.json');write(context.directory,'native-observation.json',result);need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.every(r=>r.pass),'WORLD_EFFECT_NATIVE');need(result.gameSha256===closure.find(r=>r.file==='KingdomZombies.dll').sha256&&result.coreSha256===closure.find(r=>r.file==='KingdomZombies.Core.dll').sha256,'WORLD_EFFECT_DLL_ID');need(!/ERROR:|SCRIPT ERROR:|FATAL|leaked/i.test(child.stdout+child.stderr)&&child.stderr.trim()===''&&child.stdout.includes('WORLD_EFFECT_NATIVE_COMPLETE '+context.nonce+' PASS'),'WORLD_EFFECT_NATIVE_CLEAN');return {outcomes:result.outcomes.length,pid:result.pid,rejected:EFFECT_NEGATIVES.length,productionAssemblies:closure};
}
async function effectsCase(context){const before=fingerprint(files());write(context.directory,'audio-integration-controls.json',effectAudioControls(document(ROOT,'src/godot-game/content/packs/effects-portals.json')));write(context.directory,'inputs.json',before);const result=validateEffects(document(ROOT,EFFECT_FILES[1]),document(ROOT,EFFECT_FILES[2]),document(ROOT,'src/godot-game/content/packs/common.json'));write(context.directory,'verify-effects.py',Buffer.from(VERIFY_EFFECTS));const child=await runner.command(context,'world-effects-source',PYTHON,['-B',inside(context.directory,'verify-effects.py'),ROOT]);const source=JSON.parse(child.stdout);const native=await nativeEffects(context);need(equal(before,fingerprint(files())),'WORLD_EFFECT_SOURCE_CHANGED');return {...result,source,native,scope:'Actual reusable source/native resources only;Root/index/native-policy/current-producer adoption23/31,post-review actual input/appearance/listening32/33 remain pending'};}
runner.registerCase('world.effects',{group:'world',title:'Six permanent portal identities and all cosmetic effect families through actual owned native presenter',importance:'Critical',rationale:'Active ruins,misleading loot/hazard presentation or obsolete effects break required mechanic and lifetime integrity',needsGameWindow:false,runGroup:'G0-world',order:2,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08','AC-09','AC-21','AC-24'],setup:'Source-bound world/effects/common resources,current strict production DLL and authenticated headless Godot;no donor or game window',actions:['Independently authenticate176 exact donor parts,six clip timings and51 required effect IDs','Restore24 current world/portal/stock/clearing/loop samples silently','Trigger each effect explicitly and verify limits,expiry,first contact,Reset/epoch/thread/neighbor/closed boundaries','Reject31 parent-rebound malformed semantic subjects before native attachment,plus separate wrong hash and restored positive','Authenticate native imported alpha/nonzero RGB with deterministically disposed proof-owned Images'],expected:'Every source/native resource and rejection control passes; actual visual/blood/mechanic/listening judgments remain04-32/33',reset:'Close each owned presenter;retire nodes/resources and reject old generations synchronously',evidence:['inputs.json','world-effects-source.command.json','world-effects-strict.command.json','production-assemblies.json','engine.json','native-observation.json','result.json']},effectsCase);
Object.assign(module.exports,{EFFECT_FILES,EFFECT_IDS,EFFECT_NEGATIVES,VERIFY_EFFECTS,EFFECT_HARNESS,files,validateEffects,nativeEffects,effectsCase});

const INSPECTOR_DRIVER=String.raw`extends SceneTree
var hub: Node
var outcomes: Array = []
var failed := false
const BROWSER = "Shell/Workspace/Controls/Browser/"
const CURRENT = "Shell/Workspace/Preview/Surface/Current/WorldItems/"
const BODY = CURRENT + "InspectorScroll/Body/"
func _initialize() -> void:
 call_deferred("run")
func check_case(id: String, ok: bool, observed: Variant) -> void:
 outcomes.append({"id":id,"pass":ok,"observed":observed})
 if not ok:
  failed = true
 var report = FileAccess.open("res://native-results.json",FileAccess.WRITE)
 report.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":OS.get_environment("WORLD_INSPECTOR_NONCE"),"failed":failed,"outcomes":outcomes,"scope":"Incomplete until final marker; source/headless only"}))
 report.close()
func category(index: int) -> void:
 var control = hub.get_node("Shell/Workspace/Navigation/Category")
 control.select(index)
 control.item_selected.emit(index)
func choose(id: String) -> bool:
 var control = hub.get_node(BROWSER+"Resources")
 for i in control.item_count:
  if control.get_item_text(i).begins_with(id+" · "):
   control.select(i)
   control.item_selected.emit(i)
   return true
 return false
func current_text() -> String:
 return hub.get_node(BODY+"Identity").text if hub.has_node(BODY+"Identity") else "MISSING_INSPECTOR"
func state(index: int) -> void:
 var control = hub.get_node(BODY+"Choices/CurrentState")
 control.select(index)
 control.item_selected.emit(index)
func settle() -> void:
 for i in range(240):
  await process_frame
  if not hub.get_node("Summary/Status").text.begins_with("Loading"):
   return
func descendants(node: Node) -> Array:
 var rows: Array = []
 for child in node.get_children():
  rows.append(child)
  rows.append_array(descendants(child))
 return rows
func world_positions() -> Dictionary:
 var values := {}
 for child in descendants(hub.get_node(BODY+"Art")):
  if child is Sprite2D and child.has_meta("resource_id"):
   values[str(child.get_meta("resource_id"))] = [child.position.x,child.position.y]
 return values
func replace_bytes(file: String, value: PackedByteArray) -> void:
 var output = FileAccess.open("res://"+file,FileAccess.WRITE)
 output.store_buffer(value)
 output.close()
func reader_size(file: String) -> int:
 var result = hub.call("Read",file)
 return result.size() if result is PackedByteArray else 0
func reader_controls() -> void:
 var common = "content/packs/common.json"
 var other = "content/fixtures/world-states.json"
 var original_common = FileAccess.get_file_as_bytes("res://"+common)
 var original_other = FileAccess.get_file_as_bytes("res://"+other)
 for length in [2097152,2097153]:
  var value := PackedByteArray()
  value.resize(length)
  value.fill(32)
  replace_bytes(common,value)
  hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
  await settle()
  check_case("reader-common-"+str(length),reader_size(common)==(length if length==2097152 else 0),reader_size(common))
 replace_bytes(common,original_common)
 var oversized := PackedByteArray()
 oversized.resize(1048577)
 oversized.fill(32)
 replace_bytes(other,oversized)
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 check_case("reader-other-1MiB",reader_size(other)==0,reader_size(other))
 replace_bytes(other,original_other)
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(2)
 choose("world.assembled")
 var selected_text = current_text()
 var selected_art = hub.get_node(BODY+"Art/Content").get_instance_id()
 for file in ["inspection/itemsweaponspreview.tscn","inspection/ItemswearablesPreview.tscn","inspection/./ItemsWeaponsPreview.tscn","inspection/../inspection/ItemsWeaponsPreview.tscn","/inspection/ItemsWeaponsPreview.tscn","res://inspection/ItemsWeaponsPreview.tscn","C:/inspection/ItemsWeaponsPreview.tscn","content/packs/Common.json","content/fixtures/reader-absent.json"]:
  var rejected = hub.call("Read",file)
  var controlled_empty = rejected is PackedByteArray and rejected.is_empty()
  check_case("reader-path-"+file,controlled_empty,file)
  if file.ends_with(".tscn"):
   var failure = hub.get_node("Shell/Workspace/Preview/Failures")
   var expected = "res://"+file+", field bytes, value read — WORLD_INSPECTOR_SCENE_PATH "+file
   check_case("reader-controlled-diagnostic-"+file,failure.text.split("\n").has(expected) and failure.visible and not failure.editable and controlled_empty,failure.text)
   check_case("reader-preserved-selection-"+file,current_text()==selected_text and hub.get_node(BODY+"Art/Content").get_instance_id()==selected_art,current_text())
 for file in ["inspection/ItemsWeaponsPreview.tscn","inspection/ItemsWearablesPreview.tscn"]:
  check_case("reader-exact-"+file,reader_size(file)>0,file)
 category(2)
 choose("world.assembled")
 check_case("reader-restored-world",current_text().begins_with("world.assembled\nPopulated"),current_text())
 var fresh_file = "content/fixtures/world-states.json"
 var fresh_original = FileAccess.get_file_as_bytes("res://"+fresh_file)
 var fresh_changed = "{\"reader\":\"changed-current-bytes\"}".to_utf8_buffer()
 replace_bytes(fresh_file,fresh_changed)
 check_case("reader-same-path-changed",hub.call("Read",fresh_file)==fresh_changed,fresh_file)
 var malformed = "{\"duplicate\":1,\"duplicate\":2}".to_utf8_buffer()
 replace_bytes(fresh_file,malformed)
 check_case("reader-same-path-malformed-bytes",hub.call("Read",fresh_file)==malformed,fresh_file)
 DirAccess.remove_absolute(ProjectSettings.globalize_path("res://"+fresh_file))
 var absent = hub.call("Read",fresh_file)
 check_case("reader-same-path-missing",absent is PackedByteArray and absent.is_empty() and hub.get_node("Shell/Workspace/Preview/Failures").text.contains("res://"+fresh_file+", field bytes, value read — missing or oversized resource"),fresh_file)
 replace_bytes(fresh_file,fresh_original)
 check_case("reader-same-path-restored",hub.call("Read",fresh_file)==fresh_original,fresh_file)
 var fresh_world = FileAccess.get_file_as_bytes("res://assets/world/environment.png")
 var repeat_ok = true
 for i in range(300):
  repeat_ok = (hub.call("Read","assets/world/environment.png")==fresh_world) and repeat_ok
 check_case("reader-world-repeat-budget",repeat_ok,fresh_world.size()*300)
 choose("world.assembled")
 check_case("reader-repeat-valid-neighbor",current_text().begins_with("world.assembled\nPopulated"),current_text())
func diagnostic_controls() -> void:
 category(1)
 choose("IT-07")
 var failure = hub.get_node("Shell/Workspace/Preview/Failures")
 var obligation = "IT-07: res://content/expected-obligations.json, field acceptance, value IT-07"
 var absent_file = "content/fixtures/diagnostic-absent.json"
 var read_error = "res://"+absent_file+", field bytes, value read — missing or oversized resource"
 check_case("reader-selected-exact-obligation",failure.text.contains(obligation) and failure.visible,failure.text)
 hub.call("Read",absent_file)
 hub.call("Read",absent_file)
 check_case("reader-merged-exact-obligation",failure.text.contains(obligation) and failure.text.split("\n").has(read_error) and failure.visible and not failure.editable,failure.text)
 check_case("reader-current-error-once",failure.text.count(read_error)==1,failure.text)
 replace_bytes(absent_file,"{}".to_utf8_buffer())
 var restored = hub.call("Read",absent_file)
 check_case("reader-restored-clears-exact-error",restored=="{}".to_utf8_buffer() and not failure.text.contains(read_error) and failure.text.contains(obligation),failure.text)
 DirAccess.remove_absolute(ProjectSettings.globalize_path("res://"+absent_file))
 hub.call("Read",absent_file)
 category(2)
 choose("world.assembled")
 check_case("reader-selection-current-diagnostics",not failure.text.contains(read_error) and not failure.text.contains(obligation) and current_text().begins_with("world.assembled\nPopulated"),failure.text)
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(1)
 choose("IT-07")
 check_case("reader-recheck-current-diagnostics",failure.text.contains(obligation) and not failure.text.contains(read_error) and current_text().begins_with("IT-07\nPopulated"),failure.text)
func view_snapshot(id: String) -> Dictionary:
 var art = hub.get_node(BODY+"Art/Content") if not id.begins_with("IT-") else hub.get_node(BODY+"Art/"+("ItemsWeaponsPreview" if id=="IT-07" else "ItemsWearablesPreview")+"/Groups/"+id.replace("-","_")+"/ArtScroll/Canvas/Art")
 var rendered = art.get_node("ArtScroll/Content/Art") if id.begins_with("BL-") else art
 return {"id":id,"identity":current_text(),"state":hub.get_node(BODY+"Choices/CurrentState").text,"pan":hub.get_node(BODY+"Choices/Pan").value,"zoom":hub.get_node(BODY+"Choices/Zoom").selected,"light":hub.get_node(BODY+"Choices/Light").selected,"ownerPosition":str(art.position),"ownerScale":str(art.scale),"renderedPosition":str(rendered.position),"renderedScale":str(rendered.scale),"renderedLight":str(rendered.modulate),"positions":world_positions()}
func view_expected(id: String, stage: String, positions: Dictionary, source_state: String) -> void:
 var observed = view_snapshot(id)
 var prefix = "persisted-"+id+"-"+stage
 check_case(prefix+"-facade",observed.pan==160 and observed.zoom==1 and observed.light==1,observed)
 check_case(prefix+"-scale",observed.renderedScale=="(2.0, 2.0)",observed)
 check_case(prefix+"-light",observed.renderedLight=="(0.55, 0.65, 0.82, 1.0)" and observed.identity.contains("View light: Night"),observed)
 check_case(prefix+"-source-state",observed.state==source_state and observed.identity.contains(source_state),observed)
 check_case(prefix+"-authored-positions",observed.positions==positions,observed.positions)
 if id.begins_with("BL-"):
  check_case(prefix+"-art-only",observed.ownerScale=="(1.0, 1.0)" and observed.ownerPosition=="(0.0, 0.0)" and observed.renderedPosition=="(-220.0, 40.0)",observed)
  check_case(prefix+"-nested-controls",hub.get_node(BODY+"Art/Content/Controls/Zoom").selected==1 and hub.get_node(BODY+"Art/Content/Controls/Light").selected==1,observed)
 elif id.begins_with("IT-"):
  check_case(prefix+"-art-only",observed.renderedPosition=="(-320.0, 0.0)" and hub.get_node(BODY+"Art/"+("ItemsWeaponsPreview" if id=="IT-07" else "ItemsWearablesPreview")).scale==Vector2.ONE,observed)
 else:
  check_case(prefix+"-art-only",observed.ownerPosition=="(-40.0, 280.0)",observed)
  check_case(prefix+"-quiet",not descendants(hub.get_node(BODY+"Art")).any(func(node):return node.has_meta("resource_id") and str(node.get_meta("resource_id")).begins_with("effect.")),observed)
func view_baseline() -> void:
 var observations := []
 for id in ["world.assembled","world.effects","IT-07","IT-15","BL-01","BL-04"]:
  category(1 if id.begins_with("IT-") else 2)
  choose(id)
  state(0)
  hub.get_node(BODY+"Choices/Reset").pressed.emit()
  var positions = world_positions()
  var source_state = hub.get_node(BODY+"Choices/CurrentState").text
  observations.append({"stage":"initial","view":view_snapshot(id)})
  var light = hub.get_node(BODY+"Choices/Light")
  light.select(1)
  light.item_selected.emit(1)
  var zoom = hub.get_node(BODY+"Choices/Zoom")
  zoom.select(1)
  zoom.item_selected.emit(1)
  hub.get_node(BODY+"Choices/Pan").value = 160
  observations.append({"stage":"chosen","view":view_snapshot(id)})
  view_expected(id,"chosen",positions,source_state)
  hub.get_node(BODY+"Choices/Reset").pressed.emit()
  observations.append({"stage":"reset","view":view_snapshot(id)})
  view_expected(id,"reset",positions,source_state)
  choose("IT-08" if id.begins_with("IT-") else "world.wildlife")
  choose(id)
  observations.append({"stage":"reselected","view":view_snapshot(id)})
  view_expected(id,"reselected",positions,source_state)
 var report = FileAccess.open("res://view-baseline.json",FileAccess.WRITE)
 report.store_string(JSON.stringify(observations))
 report.close()
func run() -> void:
 root.size = Vector2i(1280,720)
 hub = load("res://inspection/Inspection.tscn").instantiate()
 root.add_child(hub)
 check_case("WORLD-ITEM-PREVIEW.loading",hub.get_node("Summary/Status").text.begins_with("Loading"),hub.get_node("Summary/Status").text)
 await settle()
 category(1)
 for n in range(1,25):
  var id = "IT-%02d" % n
  check_case("item-reachable-"+id,choose(id),id)
  await process_frame
  check_case("item-native-"+id,current_text().begins_with(id+"\nPopulated") and hub.has_node(BODY+"Art/"+("ItemsWeaponsPreview" if n<=10 else "ItemsWearablesPreview")),current_text())
  if hub.has_node(BODY+"Art/"+("ItemsWeaponsPreview" if n<=10 else "ItemsWearablesPreview")):
   var actual = hub.get_node(BODY+"Art/"+("ItemsWeaponsPreview" if n<=10 else "ItemsWearablesPreview"))
   check_case("item-current-group-"+id,actual.get_node("Groups/"+id.replace("-","_")).visible and actual.get_node("Groups").get_children().filter(func(node):return node.visible).size()==1,id)
 check_case("WORLD-ITEM-PREVIEW.populated",choose("IT-07") and current_text().begins_with("IT-07\nPopulated"),current_text())
 var query = hub.get_node(BROWSER+"Search")
 query.text = "independently-absent-world-item-27"
 query.text_changed.emit(query.text)
 check_case("WORLD-ITEM-PREVIEW.empty",hub.get_node(BROWSER+"Resources").item_count==0 and hub.get_node(BROWSER+"Count").text.contains("No resources match"),hub.get_node(BROWSER+"Count").text)
 query.text = ""
 query.text_changed.emit("")
 category(2)
 for n in range(1,10):
  var id = "BL-%02d" % n
  check_case("building-reachable-"+id,choose(id),id)
  await process_frame
  check_case("building-native-"+id,current_text().begins_with(id+"\nPopulated") and hub.has_node(BODY+"Art/Content/Controls/Tier"),current_text())
  var selector = hub.get_node(BODY+"Choices/CurrentState")
  for i in selector.item_count:
   state(i)
   check_case("building-state-"+id+"-"+str(i),hub.get_node(BODY+"Art/Content").get_meta("destruction_events")==0,selector.get_item_text(i))
 check_case("world-reachable",choose("world.assembled"),current_text())
 check_case("world-native-populated",current_text().begins_with("world.assembled\nPopulated"),current_text())
 if not hub.has_node(BODY+"Art/Content"):
  quit(1)
  return
 var selector = hub.get_node(BODY+"Choices/CurrentState")
 for i in selector.item_count:
  state(i)
  check_case("world-state-"+str(i),descendants(hub.get_node(BODY+"Art")).size()>200,selector.get_item_text(i))
 state(0)
 var positions = world_positions()
 var pan = hub.get_node(BODY+"Choices/Pan")
 var zoom = hub.get_node(BODY+"Choices/Zoom")
 for i in range(4):
  zoom.select(i)
  zoom.item_selected.emit(i)
  for x in [-2560,0,2560]:
   pan.value = x
   check_case("authored-transform-"+str(i)+"-"+str(x),world_positions()==positions,hub.get_node(BODY+"Art/Content").transform)
 for i in range(7):
  hub.get_node(BODY+"Choices/Portal").item_selected.emit(i)
  check_case("portal-selector-"+str(i),world_positions()==positions,i)
 for i in range(3):
  hub.get_node(BODY+"Choices/Light").item_selected.emit(i)
  check_case("light-selector-"+str(i),descendants(hub.get_node(BODY+"Art")).size()>200,i)
 check_case("effects-reachable",choose("world.effects"),current_text())
 selector = hub.get_node(BODY+"Choices/CurrentState")
 check_case("effects-complete",selector.item_count==51,selector.item_count)
 for i in selector.item_count:
  state(i)
  hub.get_node(BODY+"Choices/Step").pressed.emit()
  check_case("effect-state-"+str(i),descendants(hub.get_node(BODY+"Art")).any(func(node):return node.has_meta("resource_id") and str(node.get_meta("resource_id")).begins_with("effect.")),selector.get_item_text(i))
 hub.get_node(BODY+"Choices/Reset").pressed.emit()
 check_case("quiet-reset",not descendants(hub.get_node(BODY+"Art")).any(func(node):return node.has_meta("resource_id") and str(node.get_meta("resource_id")).begins_with("effect.")),current_text())
 var scroll = hub.get_node(CURRENT+"InspectorScroll")
 await process_frame
 check_case("WORLD-ITEM-PREVIEW.overflow",scroll.follow_focus and scroll.get_v_scroll_bar().max_value>scroll.get_v_scroll_bar().page,scroll.get_v_scroll_bar().max_value)
 await reader_controls()
 await diagnostic_controls()
 var media = "res://assets/equipment/items-01-10.png"
 var original = FileAccess.get_file_as_bytes(media)
 DirAccess.remove_absolute(ProjectSettings.globalize_path(media))
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(1)
 choose("IT-07")
 check_case("WORLD-ITEM-PREVIEW.error",current_text().contains("WORLD_INSPECTOR_MISSING assets/equipment/items-01-10.png"),current_text())
 category(2)
 choose("world.assembled")
 check_case("partial-valid-world",current_text().begins_with("world.assembled\nPopulated"),current_text())
 var restored = FileAccess.open(media,FileAccess.WRITE)
 restored.store_buffer(original)
 restored.close()
 var corrupted = original.duplicate()
 corrupted[0] = corrupted[0]^1
 replace_bytes("assets/equipment/items-01-10.png",corrupted)
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(1)
 choose("IT-07")
 check_case("item-wrong-hash",current_text().contains("WORLD_INSPECTOR_HASH assets/equipment/items-01-10.png"),current_text())
 replace_bytes("assets/equipment/items-01-10.png",original)
 var pack_path = "res://content/packs/items-01-10.json"
 var fixture_path = "res://content/fixtures/world-states.json"
 var pack_bytes = FileAccess.get_file_as_bytes(pack_path)
 var fixture_bytes = FileAccess.get_file_as_bytes(fixture_path)
 var pack = JSON.parse_string(pack_bytes.get_string_from_utf8())
 var long_id = "assets/equipment/"+"independent-long-diagnostic-".repeat(7)+".png"
 pack.atlas.path = long_id
 var changed = JSON.stringify(pack).to_utf8_buffer()
 var fixture = JSON.parse_string(fixture_bytes.get_string_from_utf8())
 for binding in fixture.bindings:
  if binding.file=="content/packs/items-01-10.json":
   var digest = HashingContext.new()
   digest.start(HashingContext.HASH_SHA256)
   digest.update(changed)
   binding.sha256 = digest.finish().hex_encode()
 for pair in [[pack_path,changed],[fixture_path,JSON.stringify(fixture).to_utf8_buffer()]]:
  var file = FileAccess.open(pair[0],FileAccess.WRITE)
  file.store_buffer(pair[1])
  file.close()
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(1)
 choose("IT-07")
 var diagnostic = hub.get_node(BODY+"Identity")
 check_case("WORLD-ITEM-PREVIEW.long-text",diagnostic.text.contains(long_id) and diagnostic.visible and not diagnostic.editable and diagnostic.wrap_mode==TextEdit.LINE_WRAPPING_BOUNDARY,diagnostic.text)
 for pair in [[pack_path,pack_bytes],[fixture_path,fixture_bytes]]:
  var file = FileAccess.open(pair[0],FileAccess.WRITE)
  file.store_buffer(pair[1])
  file.close()
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 choose("IT-07")
 check_case("restored-positive",current_text().begins_with("IT-07\nPopulated"),current_text())
 check_case("truthful-pack-status",hub.get_node("Summary/PackStatus").text.begins_with("0/180 obligations"),hub.get_node("Summary/PackStatus").text)
 await view_baseline()
 root.remove_child(hub)
 hub.free()
 await process_frame
 var result = {"pid":OS.get_process_id(),"nonce":OS.get_environment("WORLD_INSPECTOR_NONCE"),"failed":failed,"outcomes":outcomes,"scope":"Source/headless native production hub; actual input/draw/readability pending04-32"}
 var report = FileAccess.open("res://native-results.json",FileAccess.WRITE)
 report.store_string(JSON.stringify(result))
 report.close()
 print("WORLD_INSPECTOR_COMPLETE "+result.nonce+" "+("FAIL" if failed else "PASS"))
 quit(1 if failed else 0)
`;
function inspectorFiles(){
 const paths=[...files(),'tests/phase1/WorldInspectorTests.cs','src/godot-game/content/fixtures/world-states.json','src/godot-game/inspection/Inspection.tscn','src/godot-game/project.godot','src/godot-game/content/pack-index.json','src/godot-game/content/expected-obligations.json'];
 for(const directory of ['src/godot-game/assets','src/godot-game/content','src/godot-game/inspection']){
  function walk(dir){for(const entry of fs.readdirSync(inside(ROOT,dir),{withFileTypes:true})){const file=dir+'/'+entry.name;if(entry.isDirectory())walk(file);else if(entry.isFile()&&!file.endsWith('.import')&&!file.endsWith('.uid'))paths.push(file);}}
  walk(directory);
 }
 return [...new Set(paths)];
}
const REPLACEMENT_CONTROLS=String.raw`
func replacement_controls() -> void:
 category(2)
 choose("world.assembled")
 var old_owner = hub.get_node(CURRENT.trim_suffix("/")).get_instance_id()
 category(1)
 choose("IT-15")
 var selected = current_text()
 var late = Node2D.new()
 late.name = "LateWorldA"
 hub.call("AcceptPreview",1,late)
 check_case("replacement-late-A-after-B",current_text()==selected and late.is_queued_for_deletion(),current_text())
 await process_frame
 check_case("replacement-late-A-retired",not is_instance_valid(late) and hub.get_node(CURRENT).get_parent().get_child_count()==1,current_text())
 category(2)
 choose("world.assembled")
 check_case("replacement-new-A-owner",hub.get_node(CURRENT.trim_suffix("/")).get_instance_id()!=old_owner,current_text())
 var positions = world_positions()
 for i in range(12):
  choose("world.effects")
  hub.get_node(BODY+"Choices/Reset").pressed.emit()
  check_case("replacement-effects-quiet-"+str(i),not descendants(hub.get_node(BODY+"Art")).any(func(node):return node.has_meta("resource_id") and str(node.get_meta("resource_id")).begins_with("effect.")),current_text())
  choose("BL-07")
  hub.get_node(BODY+"Choices/Reset").pressed.emit()
  check_case("replacement-building-quiet-"+str(i),hub.get_node(BODY+"Art/Content").get_meta("destruction_events")==0,current_text())
  choose("world.assembled")
  hub.get_node(BODY+"Choices/Reset").pressed.emit()
  check_case("replacement-authored-"+str(i),world_positions()==positions and hub.get_node(CURRENT).get_parent().get_child_count()==1,current_text())
 var query = hub.get_node(BROWSER+"Search")
 query.text = "independent-replacement-unlisted-73"
 query.text_changed.emit(query.text)
 check_case("WORLD-ITEM-REPLACEMENT.empty",hub.get_node(BROWSER+"Resources").item_count==0 and hub.get_node(BROWSER+"Count").text.contains("No resources match"),hub.get_node(BROWSER+"Count").text)
 query.text = ""
 query.text_changed.emit("")
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 check_case("WORLD-ITEM-REPLACEMENT.loading",hub.get_node("Summary/Status").text.begins_with("Loading") and not hub.has_node(BODY),hub.get_node("Summary/Status").text)
 await settle()
 category(2)
 choose("BL-09")
 check_case("WORLD-ITEM-REPLACEMENT.populated",current_text().begins_with("BL-09\nPopulated") and hub.has_node(BODY+"Art/Content/Controls/Tier"),current_text())
 category(1)
 choose("IT-24")
 hub.get_node(BODY+"Choices/Zoom").item_selected.emit(3)
 await process_frame
 var scroll = hub.get_node(BODY+"Art/ItemsWearablesPreview/Groups/IT_24/ArtScroll")
 check_case("WORLD-ITEM-REPLACEMENT.overflow",scroll.get_v_scroll_bar().max_value>scroll.get_v_scroll_bar().page and scroll.clip_contents,scroll.get_v_scroll_bar().max_value)
 var media = "assets/equipment/items-11-24.png"
 var original_media = FileAccess.get_file_as_bytes("res://"+media)
 DirAccess.remove_absolute(ProjectSettings.globalize_path("res://"+media))
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 choose("IT-15")
 check_case("WORLD-ITEM-REPLACEMENT.error",current_text().contains("WORLD_INSPECTOR_MISSING "+media),current_text())
 category(2)
 choose("BL-07")
 check_case("replacement-invalid-neighbor-building",current_text().begins_with("BL-07\nPopulated"),current_text())
 replace_bytes(media,original_media)
 var pack_file = "content/packs/items-11-24.json"
 var fixture_file = "content/fixtures/world-states.json"
 var pack_bytes = FileAccess.get_file_as_bytes("res://"+pack_file)
 var fixture_bytes = FileAccess.get_file_as_bytes("res://"+fixture_file)
 var pack = JSON.parse_string(pack_bytes.get_string_from_utf8())
 var long_id = "assets/equipment/"+"independent-wearable-id-".repeat(9)+".png"
 pack.atlas.path = long_id
 var changed = JSON.stringify(pack).to_utf8_buffer()
 var fixture = JSON.parse_string(fixture_bytes.get_string_from_utf8())
 for binding in fixture.bindings:
  if binding.file==pack_file:
   var digest = HashingContext.new()
   digest.start(HashingContext.HASH_SHA256)
   digest.update(changed)
   binding.sha256 = digest.finish().hex_encode()
 replace_bytes(pack_file,changed)
 replace_bytes(fixture_file,JSON.stringify(fixture).to_utf8_buffer())
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 category(1)
 choose("IT-15")
 var diagnostic = hub.get_node(BODY+"Identity")
 check_case("WORLD-ITEM-REPLACEMENT.long-text",diagnostic.text.contains(long_id) and diagnostic.visible and not diagnostic.editable and diagnostic.wrap_mode==TextEdit.LINE_WRAPPING_BOUNDARY,diagnostic.text)
 replace_bytes(pack_file,pack_bytes)
 replace_bytes(fixture_file,fixture_bytes)
 hub.get_node("Shell/Toolbar/Recheck").pressed.emit()
 await settle()
 choose("IT-15")
 check_case("replacement-restored-wearable",current_text().begins_with("IT-15\nPopulated"),current_text())
`;
async function inspectorCase(context,replacement=false){
 const inputs=fingerprint(inspectorFiles());write(context.directory,'inputs.json',inputs);
 const env=require('./defenses.cjs').isolated(),project=inside(context.directory,'native-inspector');fs.mkdirSync(project);
 const built=await runner.command(context,'inspector-strict','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(built.stdout),'WORLD_INSPECTOR_STRICT');
 for(const file of ['project.godot','foundation','inspection','content','assets','.godot/mono/temp/bin/Debug'])fs.cpSync(inside(ROOT,'src/godot-game/'+file),inside(project,file),{recursive:true});
 const legacy=require('./provenance.cjs').legacyFixture(),inspection=fs.readFileSync(inside(project,'inspection/Inspection.tscn'));
 for(const output of legacy.index.outputs)for(const dependency of output.dependencies)if(dependency.path==='src/godot-game/inspection/Inspection.tscn'){dependency.bytes=inspection.length;dependency.sha256=hash(inspection);}
 fs.writeFileSync(inside(project,'content/pack-index.json'),JSON.stringify(legacy.index,null,2)+'\n');
 fs.writeFileSync(inside(project,'content/packs/tracer.json'),legacy.files.get('src/godot-game/content/packs/tracer.json'));
 write(context.directory,'legacy-partial-fixture.json',{scope:'Independent historical one-bow/28pending fixture; current actual scene/production assemblies; no actual current180 coverage',indexSha256:hash(fs.readFileSync(inside(project,'content/pack-index.json'))),scene:{bytes:inspection.length,sha256:hash(inspection)},originalDependency:'66702efb9bf9390f6454316ce0ec5bc7808cb5f439addf0430d46abccb589739'});
 const binaries=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug'),closure=fs.readdirSync(binaries).filter(f=>fs.statSync(path.join(binaries,f)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(binaries,file)))}));write(context.directory,'production-assemblies.json',closure);
 const driver=replacement?INSPECTOR_DRIVER.replace('func run() -> void:',REPLACEMENT_CONTROLS+'\nfunc run() -> void:').replace(' await view_baseline()',' await view_baseline()\n await replacement_controls()'):INSPECTOR_DRIVER;
 write(project,'probe.gd',Buffer.from(driver));
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'],archive=inside(ROOT,'.phase1/cache/'+pin.file);need(hash(fs.readFileSync(archive))===pin.sha256,'WORLD_INSPECTOR_ARCHIVE');
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),entry=pin.directory+'/'+path.basename(executable);
 const auth=await runner.command(context,'inspector-engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); r=[x for x in z.infolist() if x.filename==sys.argv[2]]; assert len(r)==1 and 0<r[0].file_size<=268435456; print(hashlib.sha256(z.read(r[0])).hexdigest())',archive,entry]);need(hash(fs.readFileSync(executable))===auth.stdout.trim(),'WORLD_INSPECTOR_ENGINE');
 write(context.directory,'engine.json',{executable,sha256:auth.stdout.trim(),archiveSha256:pin.sha256,archiveEntry:entry,consoleExecutable:tool.executable});
 const imported=await runner.command(context,'inspector-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 need(imported.stderr.trim()===''&&!/ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked/i.test(imported.stdout+imported.stderr),'WORLD_INSPECTOR_IMPORT_CLEAN');
 const child=await runner.command(context,'inspector-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','1600'],{env:{...env,WORLD_INSPECTOR_NONCE:context.nonce}});
 const result=document(project,'native-results.json');write(context.directory,'native-observation.json',result);
 need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.every(row=>row.pass),'WORLD_INSPECTOR_NATIVE');
 for(const state of ['empty','loading','error','populated','overflow','long-text'])need(result.outcomes.filter(row=>row.id==='WORLD-ITEM-PREVIEW.'+state&&row.pass).length===1,'WORLD_INSPECTOR_BACKSTOP '+state);
 need(!/ERROR:|SCRIPT ERROR:|WARNING:|FATAL|leaked/i.test(child.stdout+child.stderr)&&child.stderr.trim()===''&&child.stdout.includes('WORLD_INSPECTOR_COMPLETE '+context.nonce+' PASS'),'WORLD_INSPECTOR_NATIVE_CLEAN');
 if(replacement){
  for(const state of ['empty','loading','error','populated','overflow','long-text'])need(result.outcomes.filter(row=>row.id==='WORLD-ITEM-REPLACEMENT.'+state&&row.pass).length===1,'WORLD_REPLACEMENT_BACKSTOP '+state);
  const compiled=inside(context.directory,'compiled');fs.mkdirSync(compiled);
  await runner.command(context,'world-inspector-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-class','*WorldInspectorTests','--report-xunit-junit','--report-xunit-junit-filename','tests.xml','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',compiled,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:{...env,WORLD_INSPECTOR_REPORT:inside(context.directory,'native-observation.json')}});
  const report=document(compiled,'tests.json');need(report.results.summary.tests===10&&report.results.summary.passed===10&&report.results.summary.failed===0&&report.results.summary.skipped===0,'WORLD_INSPECTOR_COMPILED_RESULTS');
 }
 need(equal(inputs,fingerprint(inspectorFiles())),'WORLD_INSPECTOR_SOURCE_CHANGED');return {outcomes:result.outcomes.length,pid:result.pid,productionAssemblies:closure,scope:result.scope};
}
runner.registerCase('world.inspector',{group:'world',title:'Actual hub world/item selection and six independent preview backstops',importance:'Critical',rationale:'Unreachable authored content or one bad item hiding valid world breaks the inspection pack',needsGameWindow:false,runGroup:'G0-world',order:3,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-09','AC-16','AC-20'],setup:'Current strict production assembly, actual CSharp hub and authenticated headless Godot in owned isolated project',actions:['Select all24items and9structure tiers through real hub signals','Select every world/effect current state,pan edges and zoom without changing authored positions','Observe six independently named WORLD-ITEM-PREVIEW backstops','Restore damaged media and close owned native scene'],expected:'Actual production resource identities and every individual outcome pass; actual input/draw/readability remain04-32',reset:'Revoke and retire all previews; restore owned copied bytes only',evidence:['inputs.json','production-assemblies.json','engine.json','native-observation.json','result.json']},inspectorCase);
Object.assign(module.exports,{INSPECTOR_DRIVER,inspectorFiles,inspectorCase});
runner.registerCase('world.replacement',{group:'world',title:'World/item late replacement, authored positions and independent held-out states',importance:'Critical',rationale:'A stale owner or failed entry must not replace valid current authored content',needsGameWindow:false,runGroup:'G0-world',order:4,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-09','AC-16','AC-20'],setup:'Current strict production assembly, real native hub and six compiled production fence/backstop tests',actions:['Submit late A after selecting B through actual root acceptance fence','Rapidly select/reset effects,buildings and authored map','Independently observe six wearable/building replacement backstops','Restore valid media and retire owned native scene'],expected:'All native and compiled outcomes pass independently,with stable authored positions and no historical events',reset:'Revoke and retire every preview;restore owned copied media and definitions',evidence:['inputs.json','production-assemblies.json','native-observation.json','compiled/tests.xml','compiled/tests.json','result.json']},context=>inspectorCase(context,true));
