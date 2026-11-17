'use strict';
const fs=require('node:fs'),path=require('node:path'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,document,write,hash,need}=require('../content.cjs');
function prior(){return require('./bodies-a.cjs');}
const INPUTS=['src/godot-game/inspection/CharacterPreview.cs','tests/phase1/CharacterPresentationTests.cs','tools/phase4/cases/animation.cjs',...['a','b','c'].flatMap(p=>['src/godot-game/content/packs/bodies-'+p+'.json','src/godot-game/content/animation/fits-'+p+'.json','src/godot-game/assets/characters/bodies-'+p+'.png']),...['items-01-10','items-11-24'].flatMap(p=>['src/godot-game/content/packs/'+p+'.json','src/godot-game/assets/equipment/'+p+'.png']),'src/godot-game/content/config/items.json','src/godot-game/content/config/combat.json','tools/phase1/toolchain.json'];
function files(){return [...INPUTS,...['src/core/CharacterPresentation.cs','src/godot-game/content/animation/locomotion-melee.json'].filter(p=>fs.existsSync(inside(ROOT,p)))];}
function fingerprint(){return files().map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
const HARNESS=String.raw`using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text.Json;
using Godot;
namespace KingdomZombies.AnimationProof;
public sealed partial class AnimationHarness : Node
{
 private Assembly _assembly=null!; private Type _type=null!;
 private object Invoke(object owner,string name,params object[] args){return _type.GetMethod(name,BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(owner,args)!;}
 private object Create(Node2D root){var gateway=Activator.CreateInstance(_assembly.GetType("KingdomZombies.Proofs.MainThreadGateway")!,true)!;Func<string,byte[]?> read=file=>FileAccess.GetFileAsBytes("res://"+file);return _type.GetMethod("Open",BindingFlags.Static|BindingFlags.NonPublic)!.Invoke(null,new object[]{root,gateway,read,"CH-01"})!;}
 private static void Select(Node2D root,string id,string value){var c=root.GetNode<OptionButton>("Controls/Slots/"+id+"/Select");for(int i=0;i<c.ItemCount;i++){if(StringComparer.Ordinal.Equals(c.GetItemText(i),value)){c.Select(i);c.EmitSignal(OptionButton.SignalName.ItemSelected,i);return;}}throw new InvalidOperationException("ANIMATION_CONTROL "+id+" "+value);}
 private static string Part(Node2D root,string part){return root.GetNode<Sprite2D>("Art/Body_"+part).GetMeta("part").AsString();}
 public override void _Ready(){try{Run();}catch(Exception error){GD.PrintErr(error.ToString());GetTree().Quit(1);}}
 private void Run(){
  _assembly=AssemblyLoadContext.GetLoadContext(GetType().Assembly)!.LoadFromAssemblyPath(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll"));_type=_assembly.GetType("KingdomZombies.Inspection.CharacterPreview",true)!;
  var root=new Node2D();AddChild(root);object preview=Create(root);Invoke(preview,"Attach");
  bool available=root.HasNode("Controls/Slots/Movement/Select");
  Select(root,"left","IT-04");Select(root,"right","IT-03");
  if(available){Select(root,"Movement","Walk");Select(root,"Action","Melee");}else Select(root,"Pose","windup");
  string before=Part(root,"lower");if(available)Invoke(preview,"Tick",0.13);string after=Part(root,"lower");
  bool stride=available&&!StringComparer.Ordinal.Equals(before,after)&&Part(root,"upper").Contains("windup",StringComparison.Ordinal);
  var result=new{pid=OS.GetProcessId(),nonce=OS.GetEnvironment("ANIMATION_PROOF_NONCE"),assemblySha256=Convert.ToHexStringLower(SHA256.HashData(System.IO.File.ReadAllBytes(ProjectSettings.GlobalizePath("res://subjects/KingdomZombies.dll")))),owningAlc=ReferenceEquals(AssemblyLoadContext.GetLoadContext(_assembly),AssemblyLoadContext.GetLoadContext(GetType().Assembly)),strideAdvancesDuringMelee=stride,before,after};
  Invoke(preview,"Detach");root.QueueFree();
  using(var file=FileAccess.Open("res://native-results.json",FileAccess.ModeFlags.Write)){file.StoreString(JsonSerializer.Serialize(result));}GD.Print("ANIMATION_NATIVE_COMPLETE "+result.nonce);GetTree().Quit();
 }
}
`;
async function native(context){
 const project=inside(context.directory,'animation-native');fs.mkdirSync(project);write(project,'Directory.Build.props',Buffer.from('<Project />'));write(project,'Directory.Build.targets',Buffer.from('<Project />'));
 const original=inside(ROOT,'src/godot-game/.godot/mono/temp/bin/Debug');fs.cpSync(original,inside(project,'subjects'),{recursive:true});
 const closure=fs.readdirSync(original).filter(n=>fs.statSync(path.join(original,n)).isFile()).map(file=>({file,sha256:hash(fs.readFileSync(path.join(original,file)))}));write(context.directory,'animation-assemblies.json',closure);
 write(project,'AnimationProof.csproj',Buffer.from('<Project Sdk="Godot.NET.Sdk/4.7.2"><PropertyGroup><TargetFramework>net10.0</TargetFramework><AssemblyName>AnimationProof</AssemblyName><Nullable>enable</Nullable><TreatWarningsAsErrors>true</TreatWarningsAsErrors><EnableDynamicLoading>true</EnableDynamicLoading></PropertyGroup></Project>'));
 write(project,'AnimationHarness.cs',Buffer.from(HARNESS));write(project,'project.godot',Buffer.from('config_version=5\n[application]\nconfig/name="AnimationProof"\n[dotnet]\nproject/assembly_name="AnimationProof"\n[display]\nwindow/size/viewport_width=1280\nwindow/size/viewport_height=720\n[rendering]\nrenderer/rendering_method="gl_compatibility"\n'));
 for(const file of files().filter(p=>p.startsWith('src/godot-game/')&&!p.endsWith('.cs')))write(project,file.slice('src/godot-game/'.length),bytes(ROOT,file));
 write(project,'probe.gd',Buffer.from('extends SceneTree\nfunc _initialize():\n root.add_child(load("res://AnimationHarness.cs").new())\n'));
 const build=await runner.command(context,'animation-harness-build','dotnet',['build',inside(project,'AnimationProof.csproj'),'--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-p:RestoreIgnoreFailedSources=true','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'ANIMATION_HARNESS_CLEAN');
 const executable=await require('./bodies-b.cjs').authenticate(context),env={...prior().isolated(),ANIMATION_PROOF_NONCE:context.nonce};
 await runner.command(context,'animation-native-import',executable,['--headless','--path',project,'--editor','--import','--quit'],{env});
 const child=await runner.command(context,'animation-native-run',executable,['--headless','--path',project,'--script','res://probe.gd','--quit-after','800'],{env});
 need(!/ERROR:|SCRIPT ERROR:/.test(child.stdout+child.stderr)&&child.stdout.includes('ANIMATION_NATIVE_COMPLETE '+context.nonce),'ANIMATION_NATIVE_MARKER');
 const result=document(project,'native-results.json');need(result.pid===child.pid&&result.nonce===context.nonce&&result.owningAlc&&result.assemblySha256===closure.find(x=>x.file==='KingdomZombies.dll').sha256,'ANIMATION_NATIVE_IDENTITY');write(context.directory,'animation-observation.json',result);return result;
}
async function run(context){
 const before=fingerprint();write(context.directory,'animation-inputs.json',before);
 const build=await runner.command(context,'animation-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:prior().isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'ANIMATION_CLEAN_BUILD');
 const observation=await native(context),compiled=inside(context.directory,'compiled');fs.mkdirSync(compiled);
 await runner.command(context,'animation-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-class','*CharacterPresentationTests','--report-xunit-junit','--report-xunit-junit-filename','tests.xml','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',compiled,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:{...prior().isolated(),ANIMATION_NATIVE_REPORT:inside(context.directory,'animation-observation.json')}});
 need(equal(before,fingerprint()),'ANIMATION_INPUT_CHANGED');return observation;
}
runner.registerCase('animation.melee',{group:'characters',title:'Concurrent authored lower locomotion and upper weighted melee',importance:'Critical',rationale:'Stopping stride or misidentifying equipment breaks required concurrent presentation',needsGameWindow:false,runGroup:'G0',order:50,requirements:['ASSET-02','ASSET-03'],acceptance:['AC-04','AC-06','AC-19'],setup:'Strict current production assemblies and exact eight-body/fits/item/configuration bytes in an authenticated owned headless engine',actions:['Select actual logical club and knife with walking and melee','Advance explicit deterministic elapsed time','Inspect actual lower and upper segments then execute compiled assertions'],expected:'Stride changes beneath the same weighted windup without swapping logical hands; no gameplay actions or damage are created',reset:'Detach current preview and retire owned process; no game window, sound or real input',evidence:['animation-inputs.json','animation-assemblies.json','animation-observation.json','animation-strict-build.command.json','animation-harness-build.command.json','animation-native-run.command.json','compiled/tests.xml','result.json']},run);
module.exports={INPUTS,files,fingerprint,HARNESS,native,run};
