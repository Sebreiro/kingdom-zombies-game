'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
const {PYTHON}=require('../python.cjs');
function prior(){return require('./bodies-a.cjs');}
const profiles={b:['CH-04','CH-05','CH-08'],c:['CH-06','CH-07']};
const poses=['idle','walk-near','walk-far','run','windup','strike','recover','aim','hit','collapse-a','collapse-b','corpse','sit','gather','eat','carry'];
const owned=p=>['src/godot-game/assets/characters/bodies-'+p+'.png','src/godot-game/content/packs/bodies-'+p+'.json','src/godot-game/content/animation/fits-'+p+'.json','production/phase4/bodies-'+p+'.zip'];
function fingerprint(files){return files.map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
// Native observation volume is larger than a source document; source limits stay unchanged.
const REPORT_BYTES=2097152,REPORT_NODES=125000,REPORT_DEPTH=24;
function parseNativeReport(value,label='native-results.json'){
 need(Buffer.isBuffer(value)&&value.length>0&&value.length<=REPORT_BYTES,'BODY_REPORT_BYTES',label);
 let source;try{source=new (require('node:util').TextDecoder)('utf-8',{fatal:true,ignoreBOM:true}).decode(value);}catch{throw new Error('BODY_REPORT_UTF8 '+label);}
 need(!source.startsWith('\ufeff'),'UTF8_BOM',label);
 const stack=[];
 for(const token of source.match(/"(?:[^"\\]|\\.)*"|[{}\[\]:,]/g)??[]){
  if(token==='{')stack.push({keys:new Set(),key:true});else if(token==='[')stack.push({});
  else if(token==='}'||token===']')stack.pop();else if(token===','&&stack.at(-1)?.keys)stack.at(-1).key=true;
  else if(token===':'&&stack.at(-1)?.keys)stack.at(-1).key=false;
  else if(token.startsWith('"')&&stack.at(-1)?.key){let key;try{key=JSON.parse(token);}catch{throw new Error('BODY_REPORT_JSON '+label);}const frame=stack.at(-1);
   need(!frame.keys.has(key),'JSON_DUPLICATE_KEY',label+' '+key);frame.keys.add(key);}
  need(stack.length<=REPORT_DEPTH,'DOCUMENT_DEPTH',label);
 }
 let result;try{result=JSON.parse(source);}catch{throw new Error('BODY_REPORT_JSON '+label);}let count=0;
 function walk(item,depth){
  need(++count<=REPORT_NODES&&depth<=REPORT_DEPTH,'DOCUMENT_LIMIT',label);
  if(typeof item==='number')need(Number.isFinite(item),'DOCUMENT_NUMBER',label);
  if(typeof item==='string')need(item.length<=REPORT_BYTES&&item===item.normalize('NFC'),'DOCUMENT_ENCODING',label);
  if(item&&typeof item==='object')for(const [key,child]of Object.entries(item)){
   need(!['__proto__','constructor','prototype'].includes(key),'DOCUMENT_KEY',label);walk(child,depth+1);
  }
 }
 walk(result,0);return result;
}
function nativeReportShape(result){
 need(result&&typeof result==='object'&&!Array.isArray(result)&&Number.isSafeInteger(result.pid)&&result.pid>0&&
  typeof result.nonce==='string'&&/^[a-f0-9-]{36}$/.test(result.nonce)&&typeof result.assemblySha256==='string'&&/^[a-f0-9]{64}$/.test(result.assemblySha256)&&
  typeof result.failed==='boolean'&&Array.isArray(result.outcomes)&&result.outcomes.length>0&&result.outcomes.every(x=>x&&typeof x==='object'&&!Array.isArray(x)&&typeof x.id==='string'&&x.id.length>0&&typeof x.pass==='boolean')&&
  new Set(result.outcomes.map(x=>x.id)).size===result.outcomes.length&&result.matrix&&Number.isSafeInteger(result.matrix.tuples)&&result.matrix.tuples>0&&Number.isSafeInteger(result.matrix.failed)&&result.matrix.failed>=0&&
  result.h13&&Number.isSafeInteger(result.h13.passed)&&['logicalHandsPreserved','replacementRetired','workerRejected','detachedRejected'].every(k=>typeof result[k]==='boolean'),'BODY_REPORT_SHAPE');
 return result;
}
function readNativeReport(project){
 const file=inside(project,'native-results.json'),stat=fs.statSync(file);
 need(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=REPORT_BYTES,'BODY_REPORT_BYTES','native-results.json');
 const value=bytes(project,'native-results.json');need(value.length===stat.size,'FILE_CHANGED','native-results.json');
 return nativeReportShape(parseNativeReport(value));
}
function reportControls(){
 const rows=[],encode=s=>Buffer.from(s),valid={pid:1,nonce:'00000000-0000-0000-0000-000000000000',assemblySha256:'0'.repeat(64),outcomes:[{id:'control',pass:true}],failed:false,matrix:{tuples:1,failed:0},h13:{passed:12},logicalHandsPreserved:true,replacementRetired:true,workerRejected:true,detachedRejected:true};
 for(const [id,value,code,shape]of[
  ['malformed',encode('{"x":'),'BODY_REPORT_JSON'],['duplicate',encode('{"x":1,"\\u0078":2}'),'JSON_DUPLICATE_KEY'],
  ['bom',encode('\ufeff{}'),'UTF8_BOM'],['invalid-utf8',Buffer.from([0xc3,0x28]),'BODY_REPORT_UTF8'],
  ['forbidden',encode('{"__proto__":{}}'),'DOCUMENT_KEY'],['constructor',encode('{"constructor":0}'),'DOCUMENT_KEY'],['prototype',encode('{"prototype":0}'),'DOCUMENT_KEY'],
  ['nonfinite',encode('{"x":1e400}'),'DOCUMENT_NUMBER'],['nfc',encode('{"x":"e\u0301"}'),'DOCUMENT_ENCODING'],
  ['depth',encode('['.repeat(25)+'0'+']'.repeat(25)),'DOCUMENT_DEPTH'],['overbyte',Buffer.alloc(REPORT_BYTES+1,32),'BODY_REPORT_BYTES'],
  ['overnode',encode('['+'0,'.repeat(REPORT_NODES-1)+'0]'),'DOCUMENT_LIMIT'],
  ['wrong-pass-type',encode(JSON.stringify({...valid,outcomes:[{id:'control',pass:'true'}]})),'BODY_REPORT_SHAPE',true],
  ['wrong-failed-type',encode(JSON.stringify({...valid,failed:'false'})),'BODY_REPORT_SHAPE',true]
 ]){let diagnostic;try{const r=parseNativeReport(value);if(shape)nativeReportShape(r);}catch(e){diagnostic=e.message;}need(diagnostic===code+' '+(code==='BODY_REPORT_SHAPE'?'':'native-results.json'+(id==='duplicate'?' x':'')),'BODY_REPORT_CONTROL',id+':'+diagnostic);rows.push({id,diagnostic});}
 need(equal(nativeReportShape(parseNativeReport(encode(JSON.stringify(valid)))),valid),'BODY_REPORT_POSITIVE');
 return {byteLimit:REPORT_BYTES,nodeLimit:REPORT_NODES,depthLimit:REPORT_DEPTH,rejected:rows,positive:true};
}
function validate(profile){
 const p=document(ROOT,'src/godot-game/content/packs/bodies-'+profile+'.json'),f=document(ROOT,'src/godot-game/content/animation/fits-'+profile+'.json');
 need(p.id==='bodies-'+profile&&f.id==='fits-'+profile&&p.version===1&&f.version===1&&p.logicalPixelsPerWorldPixel===1&&f.logicalPixelsPerWorldPixel===1,'BODY_PROFILE_ID');
 need(equal(p.bodies.map(b=>b.id),profiles[profile])&&equal(f.bodies.map(b=>b.id),profiles[profile]),'BODY_EXACT_ROSTER');
 need(p.parts.length===profiles[profile].length*64&&new Set(p.parts.map(x=>x.key)).size===p.parts.length,'BODY_EXACT_PARTS');
 need(p.acceptance.visual==='pending'&&f.acceptance.visual==='pending'&&p.acceptance.humanApproval===null&&p.acceptance.completedObligations.length===0,'BODY_NO_FALSE_ACCEPTANCE');
 if(profile==='c')need(equal(f.applicability.map(x=>x.item),Array.from({length:24},(_,i)=>'IT-'+String(i+1).padStart(2,'0')))&&f.applicability.every(x=>equal(x.bodies,profiles.c)&&(['IT-01','IT-22'].includes(x.item)?x.presentation==='inventory-container-only'&&x.poses.length===0&&x.facings.length===0:x.presentation==='worn-held'&&equal(x.poses,poses)&&equal(x.facings,[-1,1]))),'FIT_EXPLICIT_24_APPLICABILITY');
 need(f.bindings.every(x=>hash(bytes(ROOT,'src/godot-game/'+x.path))===x.sha256),'FIT_CURRENT_BINDING');
 const inherited=document(ROOT,'src/godot-game/content/animation/fits-a.json');need(equal(f.slots,inherited.slots)&&equal(f.items,inherited.items)&&equal(f.layerRules,inherited.layerRules),'FIT_ACTUAL_GEAR_CONTRACT');
 const parts=new Set(p.parts.map(x=>x.key));
 for(const b of f.bodies){need(Number.isInteger(b.width)&&b.width>=2&&b.width<=16&&equal(b.poses.map(x=>x.id),poses),'FIT_BODY_GEOMETRY');
  const components=new Set(f.items.flatMap(i=>i.layers.map(l=>l.component)));
  need(Object.keys(b.componentWidths).length>0&&Object.entries(b.componentWidths).every(([k,w])=>components.has(k)&&Number.isInteger(w)&&w>=1&&w<=16),'FIT_COMPONENT_WIDTH');
  for(const p of b.poses){need(Object.values(p.bodyParts).every(k=>parts.has(k))&&p.torsoLength>0&&p.torsoLength<32&&p.persistent===(p.id==='corpse'),'FIT_POSE_BINDING');
   need(Object.values(p.anchors).every(v=>v.length===2&&v.every(x=>Number.isInteger(x)&&x>=0&&x<=48))&&Number.isFinite(p.unlinkedRotation)&&Math.abs(p.unlinkedRotation)<=Math.PI&&Math.abs(p.unlinkedRotation/(Math.PI/2)-Math.round(p.unlinkedRotation/(Math.PI/2)))<1e-6,'FIT_ANCHOR_ROTATION');
   if(b.id==='CH-06')need(equal(p.mouthRelease,p.anchors.face),'FIT_SPITTER_MOUTH');
  }
 }
 return {pack:p,fits:f};
}
function verifier(){return String.raw`import pathlib,sys,io,json,copy,importlib.util
sys.dont_write_bytecode=True
root=pathlib.Path(sys.argv[1]);profile=sys.argv[2]
spec=importlib.util.spec_from_file_location('media',root/'src/godot-game/tools/asset-pipeline/extract-media.py');m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m);Image=m.image_module()
data=m.read_archive(root/('production/phase4/bodies-'+profile+'.zip'));pack=m.loads(data['bodies-'+profile+'.json']);atlas=Image.open(io.BytesIO(data['bodies-'+profile+'.png'])).convert('RGBA')
`+prior().VERIFY.slice(prior().VERIFY.indexOf('replay_cache='),prior().VERIFY.indexOf("poses=['idle'"))+String.raw`
for name,path in [('bodies-'+profile+'.png','assets/characters/'),('bodies-'+profile+'.json','content/packs/'),('fits-'+profile+'.json','content/animation/')]:m.need((root/'src/godot-game/game'/path/name).read_bytes()==data[name],'BODY_PUBLICATION')
inventory=m.loads(data['members.json']);m.need(len(inventory)+1==len(data) and len({x['path'] for x in inventory})==len(inventory),'BODY_INVENTORY')
for row in inventory:m.need(row['path'] in data and row['sha256']==m.sha(data[row['path']]) and row['bytes']==len(data[row['path']]),'BODY_MEMBER_HASH')
m.need(pack['atlas']['sha256']==m.sha(data['bodies-'+profile+'.png']) and list(atlas.size)==pack['atlas']['size'],'BODY_ATLAS')
m.need(pack['recipe']['sha256']==m.sha(data[pack['recipe']['member']]),'BODY_RECIPE')
images={}
for part in pack['parts']:
 im=Image.open(io.BytesIO(data[part['member']])).convert('RGBA');m.need(im.getbbox() and part['sha256']==m.sha(data[part['member']]) and part['rgbaSha256']==m.sha(im.tobytes()),'BODY_COMPONENT')
 m.need(set(im.getchannel('A').tobytes())<={0,255} and all(a or(r,g,b)==(0,0,0) for r,g,b,a in im.get_flattened_data()),'BODY_ALPHA')
 m.need(im.tobytes()==replay(part,data).tobytes(),'BODY_REPRODUCTION');x,y,w,h=part['atlasRect'];m.need(atlas.crop((x,y,x+w,y+h)).tobytes()==im.tobytes(),'BODY_ATLAS_PIXELS');images[part['key']]=im
silhouettes=[]
for body in pack['bodies']:
 original=Image.open(io.BytesIO(data[body['master']])).convert('RGBA');m.need(original.getchannel('A').getextrema()==(0,255) and list(original.size)==body['sourceCanvas'],'BODY_ORIGINAL_ALPHA')
 m.need(body['masterSha256']==m.sha(data[body['master']]) and body['promptSha256']==m.sha(data[body['prompt']]) and b'D-13 LOW-RESOLUTION' in data[body['prompt']],'BODY_ORIGINAL_IDENTITY')
 m.need(body['bakedEquipment']==[] and len(body['poses'])==16,'BODY_BAKED_GEAR')
 for pose in body['poses']:
  combined=Image.new('RGBA',(48,40))
  for k in ['head','upper','lower']:combined.alpha_composite(images[pose['parts'][k]])
  m.need(combined.tobytes()==images[pose['parts']['full']].tobytes() and pose['authored'] and pose['bakedEquipment']==[] and pose['persistent']==(pose['id']=='corpse'),'BODY_PARTITION')
 m.need(len({images[p['parts']['full']].tobytes() for p in body['poses']})==16,'BODY_AUTHORED_POSES')
 silhouettes.append(images[body['poses'][0]['parts']['full']].getchannel('A').tobytes())
m.need(len(set(silhouettes))==len(silhouettes),'BODY_RECOLOR_ONLY')
if profile=='c':
 all_silhouettes={}
 for family in ['a','b','c']:
  other=m.read_archive(root/('production/phase4/bodies-'+family+'.zip'));definition=m.loads(other['bodies-'+family+'.json']);parts={p['key']:p for p in definition['parts']}
  for b in definition['bodies']:
   part=parts[b['poses'][0]['parts']['full']];all_silhouettes[b['id']]=Image.open(io.BytesIO(other[part['member']])).convert('RGBA').getchannel('A').tobytes()
 m.need(sorted(all_silhouettes)==['CH-0'+str(i) for i in range(1,9)] and len(set(all_silhouettes.values()))==8,'BODY_COMPLETE_DISTINCT_SILHOUETTES')
print(json.dumps({'bodies':len(pack['bodies']),'poses':len(pack['bodies'])*16,'parts':len(pack['parts']),'members':len(data),'replayedParts':len(images),'alpha':'binary-clean','visualAcceptance':'pending04-32/33'}))
`;}
async function art(context,profile){validate(profile);const name='verify-bodies-'+profile+'.py',script=inside(context.directory,name);write(context.directory,name,Buffer.from(verifier()));const result=await runner.command(context,'body-source-'+profile,PYTHON,[script,ROOT,profile]);return JSON.parse(result.stdout);}
async function authenticate(context){
 const tool=require('./items.cjs').engine(),pin=document(ROOT,'src/godot-game/tools/verification/toolchain.json').archives['win32-x64'];
 const archive=inside(ROOT,'.phase1/cache/'+pin.file);need(fs.statSync(archive).size<=268435456&&hash(fs.readFileSync(archive))===pin.sha256,'BODY_ARCHIVE_PIN');
 const result=await runner.command(context,'body-native-origin','powershell.exe',['-NoProfile','-NonInteractive','-Command',"Add-Type -AssemblyName System.IO.Compression.FileSystem; $zip=[IO.Compression.ZipFile]::OpenRead('"+inside(ROOT,'.phase1/cache/Godot_v4.7.2-stable_mono_win64.zip').replaceAll("'","''")+"'); try { $entry=$zip.Entries | Where-Object { $_.FullName -ceq 'Godot_v4.7.2-stable_mono_win64/Godot_v4.7.2-stable_mono_win64.exe' }; if(@($entry).Count -ne 1){throw 'ARCHIVE_ENTRY'}; $stream=$entry.Open(); try { $sha=[Security.Cryptography.SHA256]::Create(); [BitConverter]::ToString($sha.ComputeHash($stream)).Replace('-','').ToLowerInvariant() } finally { $stream.Dispose() } } finally { $zip.Dispose() }"]);
 const executable=path.join(path.dirname(tool.executable),pin.executable.replace('_console.exe','.exe'));need(hash(fs.readFileSync(executable))===result.stdout.trim()&&!executable.endsWith('_console.exe'),'BODY_NONCONSOLE_PIN');return executable;
}
function harness(ids){
 let source=prior().FIT_HARNESS;
 const begin=source.indexOf(' private object Create('),end=source.indexOf('\n private void Check(',begin);
 source=source.slice(0,begin)+` private object Create(Node2D root){var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;Func<string,byte[]?> read=file=>FileAccess.GetFileAsBytes("res://"+file);return _type.GetMethod("Open",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{root,gateway,read,"CH-01"})!;}\n`+source.slice(end);
 source=source.replace('for(int b=0;b<3;b++)','for(int b=0;b<'+ids.length+';b++)').replaceAll('i%3','i%'+ids.length).replace('"CH-0"+(i%'+ids.length+'+1)','new[]{'+ids.map(x=>'"'+x+'"').join(',')+'}[i%'+ids.length+']');
 source=source.replace('using System.Text.Json;','using System.Text.Json;\nusing System.Text.Json.Nodes;');
 source=source.replace(' private async Task Run(){',GEOMETRY+'\n private async Task Run(){');
 source=source.replace('var packs=new[]{Read(', 'ShapeControls();TupleControls();Check("owning-alc",ReferenceEquals(AssemblyLoadContext.GetLoadContext(_assembly),AssemblyLoadContext.GetLoadContext(GetType().Assembly)),"actual production DLL loaded in owning Godot context");\n  var packs=new[]{Read(');
 source=source.replace('var other=new Node2D', 'var bodyChoices=primary.GetNode<OptionButton>("Controls/Slots/Body/Select");Check("exact-body-choices",Enumerable.Range(0,bodyChoices.ItemCount).Select(i=>bodyChoices.GetItemText(i)).SequenceEqual(new[]{'+ids.map(x=>'"'+x+'"').join(',')+'},StringComparer.Ordinal),"actual ordinal body identities, no aliases");var other=new Node2D');
 source=source.replace('foreach(var slot in slots){foreach(var item in slot.Value)', 'Check("body-pixels-"+b+"-"+p+"-"+f,ExactBody(primary),primary.GetMeta("body").AsString()+"/"+primary.GetMeta("pose").AsString());foreach(var slot in slots){foreach(var item in slot.Value)');
 source=source.replace('if(StringComparer.Ordinal.Equals(slot.Key,"left")', 'exact&=Geometry(primary,sprites,slot.Key,item);if(StringComparer.Ordinal.Equals(slot.Key,"left")');
 source=source.replace('exact&=Geometry(primary,sprites,slot.Key,item);','exact&=Geometry(primary,sprites,slot.Key,item)&&Mouth(primary,sprites,slot.Key);');
 return source;
}
const GEOMETRY=String.raw`
 private void TupleControls(){foreach(var row in new[]{("b","CH-08","IT-07","absent",1),("b","CH-04","IT-99","idle",-1)}){bool rejected=false;try{_type.GetMethod("FitItem",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{Read("res://content/animation/fits-"+row.Item1+".json","fits-"+row.Item1),row.Item2,row.Item3,row.Item4,row.Item5});}catch(TargetInvocationException e){rejected=StringComparer.Ordinal.Equals(e.InnerException?.Message,"FIT_INCOMPLETE body="+row.Item2+" item="+row.Item3+" pose="+row.Item4+" facing="+row.Item5);}Check("exact-tuple-diagnostic-"+row.Item2,rejected,"no identity fallback");}}
 private bool Mouth(Node2D root,Sprite2D[] sprites,string slot){if(root.GetMeta("body").AsString() is not "CH-06"||slot is not "face")return true;var fits=Read("res://content/animation/fits-c.json","fits-c");var pose=fits.GetProperty("bodies")[0].GetProperty("poses").EnumerateArray().Single(p=>StringComparer.Ordinal.Equals(p.GetProperty("id").GetString(),root.GetMeta("pose").AsString()));var mouth=pose.GetProperty("mouthRelease");float x=mouth[0].GetSingle(),y=mouth[1].GetSingle();if(root.GetMeta("facing").AsInt32()==-1)x=48-x;return sprites.Length==1&&sprites[0].Position==new Vector2(x,y)&&root.GetNode<Sprite2D>("Art/Body_head").Visible&&pose.GetProperty("anchors").GetProperty("face")[0].GetSingle()==mouth[0].GetSingle()&&pose.GetProperty("anchors").GetProperty("face")[1].GetSingle()==y;}
 private bool SamePixels(Image actual,Image expected){if(actual.GetSize()!=expected.GetSize())return false;for(int y=0;y<actual.GetHeight();y++)for(int x=0;x<actual.GetWidth();x++){var a=actual.GetPixel(x,y);var b=expected.GetPixel(x,y);if(a.A!=b.A||(a.A!=0&&(a.R!=b.R||a.G!=b.G||a.B!=b.B)))return false;}return true;}
 private bool ExactBody(Node2D root){string id=root.GetMeta("body").AsString(),pose=root.GetMeta("pose").AsString();string family=id is "CH-01" or "CH-02" or "CH-03"?"a":id is "CH-04" or "CH-05" or "CH-08"?"b":"c";var pack=Read("res://content/packs/bodies-"+family+".json","bodies-"+family);using var image=new Image();if(image.LoadPngFromBuffer(FileAccess.GetFileAsBytes("res://assets/characters/bodies-"+family+".png"))!=Error.Ok)return false;int facing=root.GetMeta("facing").AsInt32();foreach(string component in new[]{"head","upper","lower"}){var sprite=root.GetNode<Sprite2D>("Art/Body_"+component);string key=id+"."+pose+"."+component;if(id is "CH-02"&&pose is "idle"&&component is "head")key="CH-02.local-head";var part=pack.GetProperty("parts").EnumerateArray().Single(p=>StringComparer.Ordinal.Equals(p.GetProperty("key").GetString(),key));var r=part.GetProperty("atlasRect");using var expected=image.GetRegion(new Rect2I(r[0].GetInt32(),r[1].GetInt32(),r[2].GetInt32(),r[3].GetInt32()));using var actual=sprite.Texture.GetImage();bool pass=SamePixels(actual,expected)&&sprite.Scale.X==facing&&sprite.Position.X==(facing==1?0:48)&&StringComparer.Ordinal.Equals(sprite.GetMeta("part").AsString(),key);if(!pass){Check("body-detail-"+id+"-"+pose+"-"+component+"-"+facing,false,new{actual=actual.GetSize().ToString(),expected=expected.GetSize().ToString(),format=actual.GetFormat().ToString(),expectedFormat=expected.GetFormat().ToString(),part=sprite.GetMeta("part").AsString(),key,scale=sprite.Scale.ToString(),position=sprite.Position.ToString()});return false;}}return true;}
 private bool Geometry(Node2D root,Sprite2D[] sprites,string slot,string item){string id=root.GetMeta("body").AsString(),pose=root.GetMeta("pose").AsString();int facing=root.GetMeta("facing").AsInt32();int width=id is "CH-02"?6:id is "CH-04"?9:id is "CH-08"?11:5;bool added=id is "CH-04" or "CH-05" or "CH-06" or "CH-07" or "CH-08";var original=Read("res://content/animation/fits-a.json","fits-a");var expectedItem=original.GetProperty("items").EnumerateArray().Single(i=>StringComparer.Ordinal.Equals(i.GetProperty("id").GetString(),item));foreach(var sprite in sprites){string part=sprite.GetMeta("part").AsString();var layer=expectedItem.GetProperty("layers").EnumerateArray().Single(l=>StringComparer.Ordinal.Equals(l.GetProperty("part").GetString(),part));string component=layer.GetProperty("component").GetString()!;int expected=layer.GetProperty("size")[0].GetInt32();if(added){if(component.StartsWith("sleeve",StringComparison.Ordinal))expected=id is "CH-04"?5:3;else if(component.StartsWith("leg-",StringComparison.Ordinal))expected=id is "CH-04" or "CH-08"?4:3;else if(component is "waist" or "hem"||item is "IT-21")expected=width+2;}if(layer.GetProperty("anchor").GetString() is "torso")expected=width+(slot is "armor"?2:0);if(sprite.Texture.GetWidth()!=expected)return false;if(slot is "left" or "right"&&sprite.Texture.GetHeight()!=layer.GetProperty("size")[1].GetInt32())return false;if(added&&pose is "corpse"&&layer.GetProperty("end").ValueKind is JsonValueKind.Null&&layer.GetProperty("anchor").GetString() is not "torso"&&slot is not("left" or "right")&&Math.Abs(sprite.Rotation-facing*MathF.PI/2)>.001)return false;}return true;}
 private void ShapeControls(){foreach(var example in new[]{("width-zero","CHARACTER_FIT_WIDTH"),("width-large","CHARACTER_FIT_WIDTH"),("width-string","CHARACTER_FIT_WIDTH"),("body-width","CHARACTER_FIT_WIDTH"),("rotation-oblique","CHARACTER_FIT_ROTATION"),("rotation-unbounded","CHARACTER_FIT_ROTATION")}){var json=JsonNode.Parse(FileAccess.GetFileAsString("res://content/animation/fits-b.json"))!;var body=json["bodies"]![0]!;if(example.Item1.StartsWith("width",StringComparison.Ordinal)){string key=body["componentWidths"]!.AsObject().First().Key;body["componentWidths"]![key]=example.Item1 is "width-zero"?JsonValue.Create(0):example.Item1 is "width-large"?JsonValue.Create(17):JsonValue.Create("bad");}else if(example.Item1 is "body-width")body["width"]=0;else body["poses"]![0]!["unlinkedRotation"]=example.Item1 is "rotation-oblique"?.1:Math.PI*3;bool rejected=false;try{_type.GetMethod("ReadDefinition",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{JsonSerializer.SerializeToUtf8Bytes(json),"fits-b"});}catch(TargetInvocationException e){rejected=e.InnerException is System.IO.InvalidDataException&&e.InnerException.Message.StartsWith(example.Item2,StringComparison.Ordinal);}Check(example.Item1,rejected,example.Item2);}}
`;
async function native(context,ids,selected){
 const project=inside(context.directory,'body-matrix');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(x=>fs.statSync(path.join(original,x)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'body-production-assemblies.json',closure);
 write(project,'CharacterProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>CharacterProof</AssemblyName><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors><EnableDynamicLoading>true</EnableDynamicLoading></PropertyGroup></Project>'));
 write(project,'CharacterHarness.cs',Buffer.from(harness(ids)));write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="BodyProof"\n[dotnet]\nproject/assembly_name="CharacterProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of selected.filter(x=>require('../layout.cjs').physical(x).startsWith('src/godot-game/game/')&&!x.endsWith('.cs')&&!x.endsWith('.tscn')))write(project,file.replace('src/godot-game/',''),bytes(ROOT,file));
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n var proof=load("res://CharacterHarness.cs").new()\n root.add_child(proof)\n'));
 const build=await runner.command(context,'body-harness-build','dotnet',['build',inside(project,'CharacterProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'BODY_HARNESS_CLEAN_BUILD');
 const executable=await authenticate(context),env={...prior().isolated(),CHARACTER_PROOF_NONCE:context.nonce};
 await runner.command(context,'body-matrix-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 const child=await runner.command(context,'body-matrix-native',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','1200'],{env});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('CHARACTER_MATRIX_COMPLETE '+context.nonce+' PASS'),'BODY_NATIVE_MARKER');
 const result=readNativeReport(project);write(context.directory,'body-matrix-observation.json',result);
 need(result.pid===child.pid&&result.nonce===context.nonce&&result.assemblySha256===closure.find(x=>x.file==='KingdomZombies.dll').sha256&&result.failed===false&&result.outcomes.every(x=>x.pass===true)&&result.matrix.tuples===ids.length*992&&result.matrix.failed===0&&result.h13.passed===12,'BODY_NATIVE_RESULT');
 return {pid:result.pid,tuples:result.matrix.tuples,outcomes:result.outcomes.length,h13:result.h13.passed,assembly:result.assemblySha256};
}
async function run(context,complete){
 write(context.directory,'body-report-controls.json',reportControls());
 const includeC=complete||fs.existsSync(inside(ROOT,'src/godot-game/content/packs/bodies-c.json'));
 const selected=[...new Set([...prior().FIT_FILES,'tools/phase4/cases/bodies-b.cjs',...owned('b'),...(includeC?owned('c'):[])])],before=fingerprint(selected);write(context.directory,'body-inputs.json',before);
 const source=[await art(context,'b')];if(complete)source.push(await art(context,'c'));
 const itemIds=['items-01-10','items-11-24'].flatMap(p=>document(ROOT,'src/godot-game/content/packs/'+p+'.json').items.map(x=>x.id)).sort();need(equal(itemIds,Array.from({length:24},(_,i)=>'IT-'+String(i+1).padStart(2,'0'))),'FIT_ALL_24_ACTUAL_MASTERS');
 const build=await runner.command(context,'body-strict-build','dotnet',['build',inside(ROOT,'src/godot-game/KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'BODY_PRODUCTION_CLEAN_BUILD');
 const ids=includeC?Array.from({length:8},(_,i)=>'CH-0'+(i+1)):['CH-01','CH-02','CH-03','CH-04','CH-05','CH-08'];const matrix=await native(context,ids,selected);
 need(equal(before,fingerprint(selected)),'BODY_INPUT_CHANGED');return {source,matrix,scope:'Source/headless actual DLL composition; final physical appearance/input and acceptance pending04-32/33'};
}
for(const [id,complete,order]of[['bodies.b-wide',false,28],['bodies.complete-fit',true,29]])runner.registerCase(id,{group:'characters',title:complete?'Eight actual humanoid bodies and all applicable equipment fitting':'Actual brute, armored zombie and bloater wide-body fitting',importance:'Critical',rationale:'Missing fits hides required equipped bodies and persistent corpse contents',needsGameWindow:false,runGroup:'G0',order,requirements:['ASSET-01','ASSET-02','ASSET-03'],acceptance:['AC-05','AC-18'],setup:'Pinned strict production DLL, source-replayed neutral atlas components and actual item definitions',actions:['Replay every retained body component and alpha/partition/source binding','Select actual bodies,16poses,bothfacings and every31 legal slot/item choice through production Controls','Remove and replace gear; retain logical hand identity and equipped corpse endpoints'],expected:'Every declared tuple has actual current item pixels; no recolored substitute or baked armor; final readability judgment remains04-32',reset:'Fresh owned authenticated headless process, immutable per-invocation reports; no game window',evidence:['body-inputs.json','body-matrix-observation.json','body-production-assemblies.json','body-strict-build.command.json','body-harness-build.command.json','result.json']},context=>run(context,complete));
module.exports={profiles,poses,owned,fingerprint,validate,verifier,art,authenticate,harness,GEOMETRY,native,run,parseNativeReport,nativeReportShape,readNativeReport,reportControls};
