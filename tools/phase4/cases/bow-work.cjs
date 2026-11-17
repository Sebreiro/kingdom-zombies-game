'use strict';
const fs=require('node:fs'),equal=require('node:util').isDeepStrictEqual;
const runner=require('../check.cjs'),{ROOT,inside,bytes,write,hash,need}=require('../content.cjs');
const animation=require('./animation.cjs'),bodies=require('./bodies-a.cjs');
function files(){return [...animation.files(),'tools/phase4/cases/bow-work.cjs','tests/phase1/CharacterBowTests.cs'];}
function fingerprint(){return files().map(file=>({file,sha256:hash(bytes(ROOT,file))}));}
const BOW_METHOD=String.raw`
 private object _bow=null!;
 private void Bow(Node2D root,object preview){
  Select(root,"Action","None");Select(root,"left","IT-07");Select(root,"right","IT-02");Select(root,"Condition","Usable");
  var options=root.GetNode<OptionButton>("Controls/Slots/Action/Select");var values=Enumerable.Range(0,options.ItemCount).Select(i=>options.GetItemText(i)).ToArray();
  _bow=new{offered=values.Contains("Bow",StringComparer.Ordinal),options=values,leftItem=Gear(root,"left").Single().GetMeta("item").AsString(),rightItem=Gear(root,"right").Single().GetMeta("item").AsString(),condition=root.GetNode<OptionButton>("Controls/Slots/Condition/Select").GetItemText(0)};
 }
`;
function harness(){return animation.HARNESS.replace(' public override async void _Ready()',BOW_METHOD+' public override async void _Ready()').replace('await Matrix(root,preview);','Bow(root,preview);await Matrix(root,preview);').replace('restoredBindingAccepted=_restoredBindingAccepted};','restoredBindingAccepted=_restoredBindingAccepted,bow=_bow};');}
async function run(context){
 const before=fingerprint();write(context.directory,'bow-inputs.json',before);
 const build=await runner.command(context,'bow-strict-build','dotnet',['build',inside(ROOT,'KingdomZombies.slnx'),'--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-v:q'],{env:bodies.isolated()});need(/0 Warning\(s\)[\s\S]*0 Error\(s\)/.test(build.stdout),'BOW_CLEAN_BUILD');
 const observation=await animation.native(context,harness());
 const compiled=inside(context.directory,'compiled');fs.mkdirSync(compiled);
 await runner.command(context,'bow-compiled','dotnet',[inside(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-class','*CharacterBowTests','--report-xunit-junit','--report-xunit-junit-filename','tests.xml','--report-xunit-ctrf','--report-xunit-ctrf-filename','tests.json','--results-directory',compiled,'--no-ansi','--progress','off','--parallel','none','--timeout','45s'],{env:{...bodies.isolated(),BOW_NATIVE_REPORT:inside(context.directory,'animation-observation.json')}});
 need(equal(before,fingerprint()),'BOW_INPUT_CHANGED');return observation;
}
runner.registerCase('animation.bow',{group:'characters',title:'Actual walking bow aim, ammunition and fallback',importance:'Critical',rationale:'Invented ammunition or wrong logical hands breaks required combat presentation',needsGameWindow:false,runGroup:'G0',order:52,requirements:['ASSET-02','ASSET-03'],acceptance:['AC-04','AC-06','AC-24'],setup:'Actual production preview and exact current bodies, equipment and configuration in authenticated owned headless engine',actions:['Equip actual usable bow and opposite arrow hand','Observe real live action selectors and composed output','Execute compiled behavior assertions'],expected:'Bow action is available on the actual usable bow preview with exact logical hands',reset:'Detach owned preview and close owned headless process; no actual game window',evidence:['bow-inputs.json','animation-assemblies.json','animation-observation.json','compiled/tests.xml','result.json']},run);
module.exports={files,fingerprint,harness,run};
