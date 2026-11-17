'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const PYTHON='C:/Users/User/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe';
const MAP_FILES=['src/godot-game/assets/world/environment.png','src/godot-game/content/packs/world.json','src/godot-game/content/world/map.json','production/phase4/world.zip','tools/phase4/cases/world.cjs'];
const MAP_INPUTS=[...MAP_FILES,'production/phase4/common-selected.zip','src/godot-game/content/packs/common.json','tools/phase4/extract-media.py'];
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
 exact(map.plants.map(p=>[p.id,p.kind,p.x,p.capacity]),[['left.berry.1','berry',-10,12],['left.berry.2','berry',-50,12],['left.fruit.1','fruit',-16,8],['right.berry.1','berry',10,12],['right.berry.2','berry',50,12],['right.fruit.1','fruit',16,8]],'WORLD_PLANT_BASELINE');
 for(const p of map.plants){need(p.clearingRemoves===false&&p.refill==='daily-dawn-once'&&p.foodUnit==='one-item-per-stock','WORLD_PLANT_STOCK');need(parts.has(p.base)&&p.stockLayers.length===(p.kind==='berry'?4:2)&&p.stockLayers.every(x=>parts.has(x)),'WORLD_STOCK_REFERENCE');}
 exact(map.stockStates,[{id:'full',fraction:1},{id:'partial',fraction:0.5},{id:'empty',fraction:0},{id:'refilled',fraction:1}],'WORLD_STOCK_STATES');
 exact(map.clearingStates.map(s=>[s.id,s.stump,s.mark,s.rotation,s.duration]),[['intact',false,false,0,0],['marked-paid',false,true,0,0],['chopping',false,true,0,0.8],['falling-left',false,false,-Math.PI/2,0.65],['falling-right',false,false,Math.PI/2,0.65],['cleared-stump',true,false,0,0]],'WORLD_CLEARING_STATES');
 for(const zone of map.wildlifeZones){need(zone.width===15&&zone.retainExisting&&zone.visibleBorder===false,'WORLD_WILDLIFE');exact(zone.dailyBatch.map(a=>[a.kind,a.x]),[['rabbit',zone.center+(zone.side==='left'?3:-3)],['deer',zone.center+(zone.side==='left'?-3:3)]],'WORLD_PREY_PLACEMENT');need(new Set(zone.dailyBatch.map(a=>a.id)).size===2&&zone.dailyBatch.every(a=>parts.has(a.art)&&a.x>=-320&&a.x<=320),'WORLD_PREY_REFERENCE');}
 exact(map.rallies.map(r=>[r.id,r.fallbackBorder,r.offsetInward]),[['left.rally',-12,3],['right.rally',12,-3]],'WORLD_RALLY_ID');
 for(const r of map.rallies){exact(r.states,['idle','ready','launching','assault-active','recalling','returning','no-target','insufficient-squad'],'WORLD_RALLY_STATES');need(r.borderSource==='outermost-completed-living-wall'&&r.cloth.length===6&&r.cloth.every(x=>parts.has(x)),'WORLD_RALLY_BINDING');}
 exact(map.rallySamples,[{id:'refuge',livingWalls:[],borders:[-12,12],rallies:[-9,9]},{id:'expanded',livingWalls:['left.wall.2','right.wall.3'],borders:[-75,155],rallies:[-72,152]},{id:'wall-lost',livingWalls:['left.wall.1','right.wall.1'],borders:[-25,25],rallies:[-22,22]}],'WORLD_MOVING_RALLIES');
 exact(map.layers.map(r=>[r.id,r.parallax,r.z]),[['sky',0,-100],['hills',0.12,-80],['middle-forest',0.35,-60],['near-forest',0.7,-30],['ground',1,0],['actors',1,5],['foreground',1,12]],'WORLD_PARALLAX');
 need(map.lighting.length===5&&map.lighting[0].time===0&&map.lighting.at(-1).time===1,'WORLD_LIGHTING');for(const l of map.lighting){finite(l.time,0,1,'WORLD_LIGHT_TIME');color(l.sky,'WORLD_SKY');color(l.tint,'WORLD_TINT');}
 need(map.lighting.every((l,i,a)=>!i||l.time>a[i-1].time)&&map.firelight.cosmeticOnly===true&&map.firelight.energy<=1,'WORLD_LIGHT_ORDER');color(map.firelight.color,'WORLD_FIRE_COLOR');
 exact([map.reveal.radius,map.reveal.shared,map.reveal.preserveExplored,map.reveal.liveEnemies],[24,true,true,false],'WORLD_REVEAL');
 for(const sample of map.reveal.samples){ids(sample.players,'WORLD_PLAYER_DUPLICATE');need(sample.explored.length>0&&sample.explored.length<=16,'WORLD_EXPLORED');for(const range of sample.explored)need(range.length===2&&range.every(Number.isFinite)&&range[0]>=-320&&range[1]<=320&&range[0]<range[1],'WORLD_EXPLORED_RANGE');for(const p of sample.players){need(p.id==='P'+p.number&&p.number>=1&&p.number<=4&&typeof p.name==='string'&&p.name.length>0&&p.name.length<64,'WORLD_PLAYER_ID');finite(p.x,-320,320,'WORLD_PLAYER_X');color(p.color,'WORLD_PLAYER_COLOR');}for(const id of sample.knownDefenses)need([...map.walls,...map.towers].some(x=>x.id===id),'WORLD_KNOWN_DEFENSE');for(const id of sample.destroyedPortals)need(map.portals.some(x=>x.id===id),'WORLD_KNOWN_PORTAL');}
 exact(map.reveal.samples[0].players.map(p=>[p.id,p.number,p.shape]),[['P1',1,'diamond'],['P2',2,'square'],['P3',3,'triangle'],['P4',4,'circle']],'WORLD_FOUR_IDENTITIES');
 need(known(map,map.reveal.samples[0]).portals.length===0,'WORLD_UNKNOWN_PORTAL');exact(known(map,map.reveal.samples[1]).portals,['left.portal.1','left.portal.2','right.portal.1'],'WORLD_SHARED_REVEAL');
 exact(map.interaction.targetKinds,['character','corpse','site','plant','stash','rally'],'WORLD_TARGET_KINDS');need(map.interaction.selection.steady&&map.interaction.selection.outlinePixels===1&&map.interaction.selectorUsesStableId,'WORLD_SELECTION');ids(map.interaction.overlapSample,'WORLD_OVERLAP_ID');
 need(map.native.root==='AuthoredWorld'&&map.native.filter==='nearest'&&map.native.atlas===pack.atlas.path&&map.native.nodes.length===275,'WORLD_NATIVE');ids(map.native.nodes,'WORLD_NODE_IDS');
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
  check("sprites-"+filename,sprites==287,sprites)
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
  var expected_left=-72 if filename=="expanded" else (-22 if filename=="wall-lost" else -9)
  var expected_right=152 if filename=="expanded" else (22 if filename=="wall-lost" else 9)
  check("rally-"+filename,left==expected_left and right==expected_right,[left,right])
  check("navigation-"+filename,owner.get_node("WalkableNavigation").navigation_polygon.get_polygon_count()==1)
  if filename in ["falling-left","falling-right"]:
   check("fall-direction-"+filename,is_equal_approx(by_id["left.clearing.1.trunk.0"].rotation,(-PI/2 if filename=="falling-left" else PI/2)))
  if filename=="cleared-stump":check("stumps-food-independent",not by_id["left.clearing.1.trunk.0"].visible and owner.get_node("ClearState_left_1_0").visible and foodbases==6)
  if filename=="night":check("cool-night",by_id["ground_0"].modulate.b>by_id["ground_0"].modulate.r)
  root.remove_child(owner)
  owner.free()
 var failed=outcomes.any(func(x):return not x.pass)
 var file=FileAccess.open("res://native-results.json",FileAccess.WRITE)
 file.store_string(JSON.stringify({"pid":OS.get_process_id(),"nonce":OS.get_environment("WORLD_PROOF_NONCE"),"outcomes":outcomes,"failed":failed}))
 file.close()
 print("WORLD_MAP_NATIVE_COMPLETE "+OS.get_environment("WORLD_PROOF_NONCE")+" "+("FAIL" if failed else "PASS"))
 quit(1 if failed else 0)
`;
async function nativeMap(context,pack,map){
 const project=inside(context.directory,'native-world');fs.mkdirSync(project);write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="Authored world resource proof"\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 write(project,'assets/world/environment.png',bytes(ROOT,MAP_FILES[0]));write(project,'world.json',bytes(ROOT,MAP_FILES[1]));write(project,'map.json',bytes(ROOT,MAP_FILES[2]));
 const samples=[['full',{}],['partial',{stock:'partial'}],['empty',{stock:'empty'}],['refilled',{stock:'refilled'}],...['marked-paid','chopping','falling-left','falling-right','cleared-stump'].map(c=>[c,{clearing:c}]),['expanded',{rally:'expanded'}],['wall-lost',{rally:'wall-lost'}],['shared-exploration',{reveal:'shared-exploration'}],['night',{night:true}]];
 for(const [id,options]of samples)write(project,id+'.tscn',Buffer.from(scene(pack,map,options)));write(project,'probe.gd',Buffer.from(MAP_PROBE));
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'tools/phase1/toolchain.json').archives['win32-x64'];const archive=inside(ROOT,'.phase1/cache/'+pin.file);need(hash(fs.readFileSync(archive))===pin.sha256,'WORLD_ARCHIVE_PIN');
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe')),entry=pin.directory+'/'+path.basename(executable);
 const auth=await runner.command(context,'world-engine-auth',PYTHON,['-B','-c','import sys,zipfile,hashlib; z=zipfile.ZipFile(sys.argv[1]); rows=[r for r in z.infolist() if r.filename==sys.argv[2]]; assert len(rows)==1 and 0<rows[0].file_size<=268435456; print(hashlib.sha256(z.read(rows[0])).hexdigest())',archive,entry]);need(hash(fs.readFileSync(executable))===auth.stdout.trim(),'WORLD_ENGINE_PIN');
 write(context.directory,'engine.json',{...tool,consoleExecutable:tool.executable,executable,sha256:auth.stdout.trim(),archiveSha256:pin.sha256,archiveEntry:entry});
 const env={...process.env,WORLD_PROOF_NONCE:context.nonce};await runner.command(context,'world-map-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});const child=await runner.command(context,'world-map-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 const result=document(project,'native-results.json');write(context.directory,'native-observation.json',result);need(result.pid===child.pid&&result.nonce===context.nonce&&!result.failed&&result.outcomes.every(x=>x.pass),'WORLD_NATIVE_RESULT');need(!/ERROR:|SCRIPT ERROR:|FATAL/i.test(child.stdout+child.stderr)&&child.stderr.trim()===''&&child.stdout.includes('WORLD_MAP_NATIVE_COMPLETE '+context.nonce+' PASS'),'WORLD_NATIVE_CLEAN');return {pid:result.pid,outcomes:result.outcomes.length,samples:samples.length};
}
async function mapCase(context){
 const before=fingerprint(MAP_INPUTS);write(context.directory,'inputs.json',before);const pack=document(ROOT,MAP_FILES[1]),map=document(ROOT,MAP_FILES[2]);const result=validate(pack,map);const negative=controls(pack,map);write(context.directory,'negative-controls.json',negative);
 write(context.directory,'verify-world.py',Buffer.from(VERIFY_WORLD));const child=await runner.command(context,'world-map-source',PYTHON,['-B',inside(context.directory,'verify-world.py'),ROOT]);const source=JSON.parse(child.stdout);const native=await nativeMap(context,pack,map);need(equal(before,fingerprint(MAP_INPUTS)),'WORLD_SOURCE_CHANGED');return {...result,source,native,expectedRejected:negative.length,scope:'Source/replay and authenticated scriptless PackedScene resource inspection only; production WorldPreview,full hub/index,actual appearance/input acceptance remain separate'};
}
runner.registerCase('world.map',{group:'world',title:'Exact authored one-map composition,stock/clearing/reveal and moving rallies in actual native resources',importance:'Critical',rationale:'Wrong authored geometry,missing food,portal bypass or false exploration invalidates the entire world pack',needsGameWindow:false,runGroup:'G0-world',order:1,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-03','AC-08','AC-09','AC-24'],setup:'Tracked world/common bundles and atlas; pinned existing Python/Pillow and authenticated headless Godot; no donor directory',actions:['Replay42 source-bound art parts and212 retained members','Independently compare every mirrored baseline site and separate geometry/references','Reject23 deliberate semantic faults','Instantiate13 real PackedScenes with275 declared composition nodes and inspect native pixels,stock,clearing,food,reveal,identity and moving rally resources'],expected:'All exact world resources and independent malformed controls pass; real game windows and named judgment remain04-32/33',reset:'Fresh owned immutable invocation; each native owner removed and freed before next sample',evidence:['inputs.json','negative-controls.json','world-map-source.command.json','engine.json','native-observation.json','result.json']},mapCase);
module.exports={MAP_FILES,MAP_INPUTS,MAP_NEGATIVES,VERIFY_WORLD,MAP_PROBE,fingerprint,validate,known,controls,scene,nativeMap,mapCase};
