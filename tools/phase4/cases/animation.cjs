'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
function prior(){return require('./bodies-a.cjs');}
const INPUTS=['src/godot-game/inspection/CharacterPreview.cs','tests/phase1/CharacterPresentationTests.cs','tools/phase4/cases/animation.cjs',...['a','b','c'].flatMap(p=>['src/godot-game/content/packs/bodies-'+p+'.json','src/godot-game/content/animation/fits-'+p+'.json','src/godot-game/assets/characters/bodies-'+p+'.png']),...['items-01-10','items-11-24'].flatMap(p=>['src/godot-game/content/packs/'+p+'.json','src/godot-game/assets/equipment/'+p+'.png']),'src/godot-game/content/config/items.json','src/godot-game/content/config/combat.json','tools/phase1/toolchain.json'];
function files(){return [...INPUTS,...['src/core/CharacterPresentation.cs','src/godot-game/content/animation/locomotion-melee.json'].filter(p=>fs.existsSync(inside(ROOT,p)))];}
function fingerprint(){return files().map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
const HARNESS=String.raw`using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Godot;
namespace KingdomZombies.AnimationProof;
public sealed partial class AnimationHarness : Node
{
 private Assembly _assembly=null!; private Type _type=null!;
 private object Invoke(object owner,string name,params object[] args){return _type.GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(owner,args)!;}
 private object Create(Node2D root){var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;Func<string,byte[]?> read=file=>FileAccess.GetFileAsBytes("res://"+file);return _type.GetMethod("Open",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{root,gateway,read,"CH-01"})!;}
 private static void Select(Node2D root,string id,string value){var c=root.GetNode<OptionButton>("Controls/Slots/"+id+"/Select");for(int i=0;i<c.ItemCount;i++){if(StringComparer.Ordinal.Equals(c.GetItemText(i),value)){c.Select(i);c.EmitSignal(OptionButton.SignalName.ItemSelected,i);return;}}throw new InvalidOperationException("ANIMATION_CONTROL "+id+" "+value);}
 private static string Part(Node2D root,string part){return root.GetNode<Sprite2D>("Art/Body_"+part).GetMeta("part").AsString();}
 private readonly List<object> _samples=new();
 private readonly List<object> _negatives=new();
 private bool _restoredBindingAccepted;
 private static JsonElement Sample(Node2D root){using var doc=JsonDocument.Parse(root.GetMeta("sample").AsString());return doc.RootElement.Clone();}
 private static JsonElement Read(string path){using var doc=JsonDocument.Parse(FileAccess.GetFileAsBytes("res://"+path));return doc.RootElement.Clone();}
 private static void Check(bool pass,string id){if(!pass)throw new InvalidOperationException("ANIMATION_OBSERVATION "+id);}
 private static Sprite2D[] Gear(Node2D root,string slot){return root.GetNode("Art").GetChildren().OfType<Sprite2D>().Where(s=>s.HasMeta("slot")&&StringComparer.Ordinal.Equals(s.GetMeta("slot").AsString(),slot)).ToArray();}
 private void Negatives(){
  foreach(string defect in new[]{"missing","hash","binding","duplicate","traversal"}){var root=new Node2D();AddChild(root);var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;var animation=JsonNode.Parse(FileAccess.GetFileAsString("res://content/animation/locomotion-melee.json"))!;if(StringComparer.Ordinal.Equals(defect,"hash"))animation["bindings"]![0]!["sha256"]=new string('0',64);if(StringComparer.Ordinal.Equals(defect,"binding"))animation["bindings"]!.AsArray().RemoveAt(0);if(StringComparer.Ordinal.Equals(defect,"traversal"))animation["bindings"]![0]!["path"]="src/godot-game/../escape.json";Func<string,byte[]?> read=file=>StringComparer.Ordinal.Equals(file,"content/animation/locomotion-melee.json")?(StringComparer.Ordinal.Equals(defect,"missing")?null:StringComparer.Ordinal.Equals(defect,"duplicate")?System.Text.Encoding.UTF8.GetBytes("{\"id\":\"locomotion-melee\",\"id\":\"locomotion-melee\"}"):JsonSerializer.SerializeToUtf8Bytes(animation)):FileAccess.GetFileAsBytes("res://"+file);bool rejected=false;string exceptionType=string.Empty,message=string.Empty;try{_type.GetMethod("Open",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{root,gateway,read,"CH-01"});}catch(TargetInvocationException e){exceptionType=e.InnerException?.GetType().FullName??string.Empty;message=e.InnerException?.Message??string.Empty;rejected=StringComparer.Ordinal.Equals(defect,"traversal")?e.InnerException?.GetType()==typeof(FormatException)&&StringComparer.Ordinal.Equals(message,"canonical relative path required: src/godot-game/../escape.json"):e.InnerException is System.IO.InvalidDataException;}_negatives.Add(new{defect,rejected,exceptionType,message});Check(rejected,"strict-native-source-"+defect);root.QueueFree();}
  var restoredRoot=new Node2D();AddChild(restoredRoot);object restored=Create(restoredRoot);Invoke(restored,"Attach");_restoredBindingAccepted=true;Invoke(restored,"Detach");restoredRoot.QueueFree();
 }
 private void Jump(Node2D root,object preview){Select(root,"Action","None");Select(root,"Body","CH-01");Select(root,"left","Empty");Select(root,"right","Empty");Select(root,"Movement","Idle");Select(root,"Movement","Jump");double[] deltas={0,.081,.16,.16};string[] poses={"run","walk-far","walk-near","run"};string[] stages={"takeoff","airborne-up","airborne-down","landing"};for(int i=0;i<4;i++){Invoke(preview,"Tick",deltas[i]);var sample=Sample(root);Check(StringComparer.Ordinal.Equals(sample.GetProperty("stage").GetString(),stages[i]),"native-jump-"+stages[i]);Body(root,"CH-01",poses[i],poses[i],root.GetMeta("facing").AsInt32());}}
 private static bool Pixels(Sprite2D sprite,JsonElement pack,string atlas,string key){
  var part=pack.GetProperty("parts").EnumerateArray().Single(p=>StringComparer.Ordinal.Equals(p.GetProperty("key").GetString(),key));var r=part.GetProperty("atlasRect");using var source=new Image();Check(source.LoadPngFromBuffer(FileAccess.GetFileAsBytes("res://"+atlas))==Error.Ok,"source-image");using var expected=source.GetRegion(new Rect2I(r[0].GetInt32(),r[1].GetInt32(),r[2].GetInt32(),r[3].GetInt32()));using var actual=sprite.Texture.GetImage();if(actual.GetSize()!=expected.GetSize())return false;for(int y=0;y<actual.GetHeight();y++)for(int x=0;x<actual.GetWidth();x++){var a=actual.GetPixel(x,y);var b=expected.GetPixel(x,y);if(a.A!=b.A||(a.A!=0&&(a.R!=b.R||a.G!=b.G||a.B!=b.B)))return false;}return true;
 }
 private static void Body(Node2D root,string body,string lower,string upper,int facing){
  string family=body is "CH-01" or "CH-02" or "CH-03"?"a":body is "CH-04" or "CH-05" or "CH-08"?"b":"c";var pack=Read("content/packs/bodies-"+family+".json");foreach(string component in new[]{"lower","upper","head"}){string pose=StringComparer.Ordinal.Equals(component,"lower")?lower:upper;string key=body+"."+pose+"."+component;if(StringComparer.Ordinal.Equals(body,"CH-02")&&StringComparer.Ordinal.Equals(pose,"idle")&&StringComparer.Ordinal.Equals(component,"head"))key="CH-02.local-head";var sprite=root.GetNode<Sprite2D>("Art/Body_"+component);Check(StringComparer.Ordinal.Equals(Part(root,component),key)&&sprite.Scale.X==facing&&sprite.TextureFilter==CanvasItem.TextureFilterEnum.Nearest&&Pixels(sprite,pack,"assets/characters/bodies-"+family+".png",key),"registered-body-"+key);}
 }
 private static void Grips(Node2D root,string body,string pose,string left,string right,int facing,double[] angles){
  string family=body is "CH-01" or "CH-02" or "CH-03"?"a":body is "CH-04" or "CH-05" or "CH-08"?"b":"c";var fit=Read("content/animation/fits-"+family+".json");var anchors=fit.GetProperty("bodies").EnumerateArray().Single(b=>StringComparer.Ordinal.Equals(b.GetProperty("id").GetString(),body)).GetProperty("poses").EnumerateArray().Single(p=>StringComparer.Ordinal.Equals(p.GetProperty("id").GetString(),pose)).GetProperty("anchors");var sample=Sample(root);var lower=sample.GetProperty("lowerOffset");var upper=sample.GetProperty("upperOffset");int hand=0;foreach(var pair in new[]{("left",left),("right",right)}){var sprites=Gear(root,pair.Item1);if(StringComparer.Ordinal.Equals(pair.Item2,"Empty")){Check(sprites.Length==0,"empty-fist-hand");hand++;continue;}Check(sprites.Length==1,"one-hand-layer");var sprite=sprites[0];var at=anchors.GetProperty(pair.Item1+"Hand");float x=at[0].GetSingle()+lower[0].GetSingle()+upper[0].GetSingle()+sample.GetProperty("flinchOffset").GetSingle(),y=at[1].GetSingle()+lower[1].GetSingle()+upper[1].GetSingle()+sample.GetProperty("transitionOffset").GetSingle();Check(StringComparer.Ordinal.Equals(sprite.GetMeta("item").AsString(),pair.Item2)&&StringComparer.Ordinal.Equals(sprite.GetMeta("logical_hand").AsString(),pair.Item1)&&Math.Abs(sprite.Position.X-(facing==1?x:48-x))<.001&&Math.Abs(sprite.Position.Y-y)<.001&&Math.Abs(sprite.Rotation-facing*angles[hand])<.001,"registered-grip-"+body+"-"+pose+"-"+pair.Item1);hand++;}
 }
 private async Task Matrix(Node2D root,object preview){
  var rows=new[]{("Empty","Empty",.7), ("IT-03","Empty",.5),("IT-04","Empty",.9),("IT-05","Empty",.75),("IT-06","Empty",1.1),("Empty","IT-03",.5),("Empty","IT-04",.9),("Empty","IT-05",.75),("Empty","IT-06",1.1),("IT-03","IT-04",.9),("IT-06","IT-05",1.1)};
  var angles=new Dictionary<string,double[]>(StringComparer.Ordinal){{"Empty",new[]{-.25,.5,.1,0}},{"IT-03",new[]{-.4,1.1,.5,0}},{"IT-04",new[]{-1.15,1.4,1,.2}},{"IT-05",new[]{-.8,1.6,.8,.1}},{"IT-06",new[]{-1.3,1.5,1.1,.25}}};
  foreach(string body in Enumerable.Range(1,8).Select(i=>"CH-"+i.ToString("00",System.Globalization.CultureInfo.InvariantCulture))){foreach(int facing in new[]{1,-1})foreach(var row in rows){Select(root,"Action","None");Select(root,"Movement","Idle");Select(root,"Body",body);Select(root,"Facing",facing==1?"Right":"Left");Select(root,"left",row.Item1);Select(root,"right",row.Item2);Select(root,"Movement","Walk");Select(root,"Action","Melee");string before=Part(root,"lower");Invoke(preview,"Tick",.13);var sample=Sample(root);Check(!StringComparer.Ordinal.Equals(before,Part(root,"lower"))&&sample.GetProperty("intervalSeconds").GetDouble()==row.Item3,"native-stride-interval");
   double elapsed=.13;string[] poses={"windup","strike","recover","idle"};double[] times={.13,row.Item3*.34,row.Item3*.55,row.Item3*.85};for(int stage=0;stage<4;stage++){if(stage>0)Invoke(preview,"Tick",times[stage]-elapsed);elapsed=times[stage];sample=Sample(root);string lower=sample.GetProperty("lowerPose").GetString()!;Body(root,body,lower,poses[stage],facing);Grips(root,body,poses[stage],row.Item1,row.Item2,facing,new[]{angles[row.Item1][stage],angles[row.Item2][stage]});Check(sample.GetProperty("gameplayEvents").GetInt32()==0&&StringComparer.Ordinal.Equals(sample.GetProperty("releaseSound").GetString(),"AU-03")&&StringComparer.Ordinal.Equals(sample.GetProperty("impactSound").GetString(),"AU-04"),"no-gameplay-config-events");if(stage==1){var contact=root.GetNode<Polygon2D>("Art/Contact");Check(contact.Polygon.Length==4&&contact.Polygon.Max(p=>p.X)-contact.Polygon.Min(p=>p.X)==2&&contact.Polygon.Max(p=>p.Y)-contact.Polygon.Min(p=>p.Y)==2,"minimal-native-contact");Invoke(preview,"Tick",0.0);Check(root.HasNode("Art/Contact"),"one-shot-contact-not-retriggered");}if(stage>=2)Check(!root.HasNode("Art/Contact"),"contact-expired");_samples.Add(new{body,left=row.Item1,right=row.Item2,facing,stage=poses[stage],lower,interval=row.Item3,gameplayEvents=0});}
  }await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);}
 }
 public override async void _Ready(){try{await Run();}catch(Exception error){GD.PrintErr(error.ToString());GetTree().Quit(1);}}
 private async Task Run(){
  var context=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!;var core=context.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.Core.dll"));_assembly=context.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));_type=_assembly.GetType("KingdomZombies.Inspection.CharacterPreview",true)!;
  var root=new Node2D();AddChild(root);object preview=Create(root);Invoke(preview,"Attach");
  bool available=root.HasNode("Controls/Slots/Movement/Select");
  Select(root,"left","IT-04");Select(root,"right","IT-03");
  if(available){Select(root,"Movement","Walk");Select(root,"Action","Melee");}else Select(root,"Pose","windup");
  string before=Part(root,"lower");if(available)Invoke(preview,"Tick",0.13);string after=Part(root,"lower");
  bool stride=available&&!StringComparer.Ordinal.Equals(before,after)&&Part(root,"upper").Contains("windup",StringComparison.Ordinal);
  await Matrix(root,preview);
  Jump(root,preview);Negatives();
  var result=new{pid=OS.GetProcessId(),nonce=OS.GetEnvironment("ANIMATION_PROOF_NONCE"),assemblySha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(_assembly.Location))),coreSha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(core.Location))),coreOwningAlc=ReferenceEquals(AssemblyLoadContext.GetLoadContext(core),context),owningAlc=ReferenceEquals(AssemblyLoadContext.GetLoadContext(_assembly),context),strideAdvancesDuringMelee=stride,before,after,samples=_samples,negatives=_negatives,restoredBindingAccepted=_restoredBindingAccepted};
  Invoke(preview,"Detach");root.QueueFree();
  using(var file=FileAccess.Open("res://native-results.json",FileAccess.ModeFlags.Write)){file.StoreString(JsonSerializer.Serialize(result));}GD.Print("ANIMATION_NATIVE_COMPLETE "+result.nonce);GetTree().Quit();
 }
}
`;
const REPLACEMENT_METHOD=String.raw`
 private object? _replacementError;
 private async Task Replacement(){
  var hubType=_assembly.GetType("KingdomZombies.Inspection.InspectionRoot",true)!;
  var skeleton=ResourceLoader.Load<PackedScene>("res://inspection-fixture.tscn").Instantiate<Node2D>();
  var hub=(Node2D)Activator.CreateInstance(hubType)!;
  foreach(Node child in skeleton.GetChildren()){skeleton.RemoveChild(child);hub.AddChild(child);}
  skeleton.QueueFree();AddChild(hub);
  for(int frame=0;frame<200&&!hub.GetNode<Button>("Shell/Toolbar/Cancel").Disabled;frame++)await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
  Check(hub.GetNode<Button>("Shell/Toolbar/Cancel").Disabled,"replacement-hub-ready");
  var selected=hubType.GetField("_selected",BindingFlags.Instance|BindingFlags.NonPublic)!;
  var category=hubType.GetField("_category",BindingFlags.Instance|BindingFlags.NonPublic)!;
  var selection=hubType.GetMethod("Selection",BindingFlags.Instance|BindingFlags.NonPublic)!;
  selected.SetValue(hub,"CH-06");category.SetValue(hub,"Characters");selection.Invoke(hub,null);
  Check(hub.HasNode("Shell/Workspace/Preview/Surface/Current/Character/Controls/Slots/Movement/Select"),"replacement-valid-character-control");
  string file=ProjectSettings.GlobalizePath("res://content/animation/locomotion-melee.json");
  byte[] original=System.IO.File.ReadAllBytes(file);string exceptionType=string.Empty,message=string.Empty;
  try{
   System.IO.File.WriteAllText(file,"{\"id\":\"locomotion-melee\",\"id\":\"locomotion-melee\"}");
   try{selection.Invoke(hub,null);}catch(TargetInvocationException error) when(error.InnerException is System.IO.InvalidDataException){exceptionType=error.InnerException.GetType().FullName!;message=error.InnerException.Message;}
   string failures=hub.GetNode<TextEdit>("Shell/Workspace/Preview/Failures").Text;
   bool errorVisible=hub.GetNode<TextEdit>("Shell/Workspace/Preview/Failures").Visible&&failures.Contains("content/animation/locomotion-melee.json",StringComparison.Ordinal)&&failures.Contains("CHARACTER_DUPLICATE_KEY",StringComparison.Ordinal);
   bool transportUsable=hub.GetNode<Button>("Shell/Toolbar/Recheck").Visible&&!hub.GetNode<Button>("Shell/Toolbar/Recheck").Disabled;
   _replacementError=new{body="CH-06",fixture="duplicate-animation-definition",exceptionType,message,failures,errorVisible,transportUsable};
  }finally{System.IO.File.WriteAllBytes(file,original);hub.QueueFree();}
  await ToSignal(GetTree(),SceneTree.SignalName.ProcessFrame);
 }
`;
const REPLACEMENT_HARNESS=HARNESS.replace(' public override async void _Ready()',REPLACEMENT_METHOD+' public override async void _Ready()').replace('Jump(root,preview);Negatives();','Jump(root,preview);Negatives();await Replacement();').replace('restoredBindingAccepted=_restoredBindingAccepted};','restoredBindingAccepted=_restoredBindingAccepted,replacementError=_replacementError};');
async function native(context,harness=HARNESS){
 const project=inside(context.directory,'animation-native');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(n=>fs.statSync(path.join(original,n)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'animation-assemblies.json',closure);
 write(project,'AnimationProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>AnimationProof</AssemblyName><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors><EnableDynamicLoading>true</EnableDynamicLoading></PropertyGroup></Project>'));
 write(project,'AnimationHarness.cs',Buffer.from(harness));write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="AnimationProof"\n[dotnet]\nproject/assembly_name="AnimationProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of files().filter(p=>p.startsWith('src/godot-game/')&&!p.endsWith('.cs')))write(project,file.slice('src/godot-game/'.length),bytes(ROOT,file));
 if(harness===REPLACEMENT_HARNESS){for(const folder of ['content','assets'])fs.cpSync(inside(ROOT,'src/godot-game/'+folder),inside(project,folder),{recursive:true});const scene=bytes(ROOT,'src/godot-game/inspection/Inspection.tscn').toString('utf8').replace('[gd_scene load_steps=4 format=3]','[gd_scene load_steps=3 format=3]').replace('[ext_resource type="Script" path="res://inspection/InspectionRoot.cs" id="2_root"]','').replace('script = ExtResource("2_root")','');write(project,'inspection-fixture.tscn',Buffer.from(scene));}
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n root.add_child(load("res://AnimationHarness.cs").new())\n'));
 const build=await runner.command(context,'animation-harness-build','dotnet',['build',inside(project,'AnimationProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'ANIMATION_HARNESS_CLEAN');
 const executable=await require('./bodies-b.cjs').authenticate(context),env={...prior().isolated(),ANIMATION_PROOF_NONCE:context.nonce};
 await runner.command(context,'animation-native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 const child=await runner.command(context,'animation-native-run',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('ANIMATION_NATIVE_COMPLETE '+context.nonce),'ANIMATION_NATIVE_MARKER');
 const result=document(project,'native-results.json');need(result.pid===child.pid&&result.nonce===context.nonce&&result.owningAlc&&result.coreOwningAlc&&result.coreSha256===closure.find(x=>x.file==='KingdomZombies.Core.dll').sha256&&result.assemblySha256===closure.find(x=>x.file==='KingdomZombies.dll').sha256,'ANIMATION_NATIVE_IDENTITY');
 need(equal(result.negatives.map(n=>n.defect),['missing','hash','binding','duplicate','traversal'])&&result.negatives.every(n=>n.rejected===true&&typeof n.message==='string'&&n.message.length>0&&n.exceptionType===(n.defect==='traversal'?'System.FormatException':'System.IO.InvalidDataException'))&&result.negatives[4].message==='canonical relative path required: src/godot-game/../escape.json'&&result.restoredBindingAccepted===true,'ANIMATION_NATIVE_SOURCE_CONTROLS');write(context.directory,'animation-observation.json',result);return result;
}
async function run(context,replacement=false){
 const before=fingerprint();write(context.directory,'animation-inputs.json',before);
 const build=await runner.command(context,'animation-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'ANIMATION_CLEAN_BUILD');
 const observation=await native(context,replacement?REPLACEMENT_HARNESS:HARNESS),compiled=inside(context.directory,'compiled');fs.mkdirSync(compiled);
 await runner.command(context,'animation-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-class','*CharacterPresentationTests','--report-xunit-junit','--report-xunit-junit-filename','tests.xml','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',compiled,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:{...prior().isolated(),ANIMATION_NATIVE_REPORT:inside(context.directory,'animation-observation.json')}});
 need(equal(before,fingerprint()),'ANIMATION_INPUT_CHANGED');return observation;
}
runner.registerCase('animation.melee',{group:'characters',title:'Concurrent authored lower locomotion and upper weighted melee',importance:'Critical',rationale:'Stopping stride or misidentifying equipment breaks required concurrent presentation',needsGameWindow:false,runGroup:'G0',order:50,requirements:['ASSET-02','ASSET-03'],acceptance:['AC-04','AC-06','AC-19'],setup:'Strict current production assemblies and exact eight-body/fits/item/configuration bytes in an authenticated owned headless engine',actions:['Select actual logical club and knife with walking and melee','Advance explicit deterministic elapsed time','Inspect actual lower and upper segments then execute compiled assertions'],expected:'Stride changes beneath the same weighted windup without swapping logical hands; no gameplay actions or damage are created',reset:'Detach current preview and retire owned process; no game window, sound or real input',evidence:['animation-inputs.json','animation-assemblies.json','animation-observation.json','animation-strict-build.command.json','animation-harness-build.command.json','animation-native-run.command.json','compiled/tests.xml','result.json']},run);
runner.registerCase('animation.replacement',{group:'characters',title:'Independent current character replacement and visible definition errors',importance:'Critical',rationale:'Obsolete selections or invisible required-resource failures break live inspection integrity',needsGameWindow:false,runGroup:'G0',order:51,requirements:['ASSET-02','ASSET-03'],acceptance:['AC-04','AC-06','AC-16','AC-19'],setup:'Exact current production assemblies, full owned resource copy and actual native inspection root using its unchanged scene children',actions:['Prove actual CH-06 controls load before the negative fixture','Duplicate a required animation key only in the owned native project','Invoke actual production selection and inspect visible exact diagnostics and recheck transport','Restore original valid definition bytes and retain authenticated native and compiled outcomes'],expected:'The current character failure remains visible with exact resource identity and cause; unaffected controls remain usable',reset:'Restore the exact owned definition in finally and close actual root and native process; no game window',evidence:['animation-inputs.json','animation-observation.json','animation-native-run.command.json','compiled/tests.xml','result.json']},context=>run(context,true));
module.exports={INPUTS,files,fingerprint,HARNESS,REPLACEMENT_HARNESS,native,run};
