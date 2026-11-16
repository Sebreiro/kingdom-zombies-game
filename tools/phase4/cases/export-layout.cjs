'use strict';
const fs=require('node:fs'),path=require('node:path');
const runner=require('../check.cjs');
const {ROOT,need,inside,bytes,document,write,hash}=require('../content.cjs');
const DOTNET='C:/Program Files/dotnet/dotnet.exe';
const REGISTRY='docs/code/generated-layout-adaptations.json',ORIGIN='production/phase4/export-layout-origin.json';
function snapshot(){
  return Object.fromEntries([REGISTRY,ORIGIN,'tools/code-rules/GeneratedLayout.cs','tools/phase4/check.cjs',
    'tools/phase4/cases/export-layout.cjs','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets',
    'tools/phase1/required-cases.json','tests/phase1/GeneratedExportLayoutTests.cs']
    .map(file=>[file,hash(bytes(ROOT,file))]));
}
function sourceFile(root,relative){
  need(typeof relative==='string'&&!relative.includes('\\')&&!path.isAbsolute(relative)&&
    relative.split('/').every(x=>x&&x!=='.'&&x!=='..'&&!x.includes(':')),'EXPORT_SOURCE_PATH',relative);
  let current=root;
  for(const segment of relative.split('/')){
    need(fs.readdirSync(current).includes(segment)&&!fs.lstatSync(current).isSymbolicLink(),'EXPORT_SOURCE_CASE',relative);
    current=path.join(current,segment);
  }
  need(!fs.lstatSync(current).isSymbolicLink()&&fs.statSync(current).isFile(),'EXPORT_SOURCE_REPARSE',relative);
  return current;
}
function authenticate(row,root,relative){
  need(row.platform==='win32-x64'&&row.sdk==='10.0.404'&&row.bytes===200&&row.line===4&&row.lineLength===135&&
    row.originSymbol==='GenerateTargetFrameworkMonikerAttribute'&&row.suffix==='/'+row.path,'EXPORT_SOURCE_IDENTITY',row.id);
  const data=fs.readFileSync(sourceFile(root,relative)),text=new TextDecoder('utf-8',{fatal:true,ignoreBOM:true}).decode(data);
  need(!text.startsWith('\ufeff')&&data.length===row.bytes&&hash(data)===row.sha256&&
    hash(Buffer.from(text,'utf8'))===row.textSha256&&text.split(/\r?\n/)[row.line-1].length===row.lineLength,
  'EXPORT_SOURCE_HASH',row.id);
  return {id:row.id,path:row.path,sha256:hash(data),bytes:data.length,line:row.line,lineLength:row.lineLength};
}
function policy(result,configuration){
  const value=JSON.parse(result.stdout),p=value.Properties;
  need(p.Configuration===configuration&&p.TargetFramework==='net10.0'&&p.RuntimeIdentifier==='win-x64'&&
    p.SelfContained==='true'&&p.EnableNETAnalyzers==='true'&&p.TreatWarningsAsErrors==='true'&&
    p.MSBuildTreatWarningsAsErrors==='true'&&p.WarningsNotAsErrors===''&&p.NoWarn==='1701;1702'&&
    [p.RunAnalyzers,p.RunAnalyzersDuringBuild,p.CodeAnalysisTreatWarningsAsErrors].every(x=>x===''||x==='true'),
  'EXPORT_ANALYZER_POLICY',JSON.stringify(p));
  need(value.Items.ProjectReference.some(x=>x.OutputItemType==='Analyzer'&&x.ReferenceOutputAssembly==='false'&&
    x.Identity.replaceAll('\\','/').endsWith('/tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj')),'EXPORT_ANALYZER_EDGE');
  return value;
}
function dependencies(directory){
  const names=fs.readdirSync(directory);
  for(const name of ['KingdomZombies.dll','KingdomZombies.Core.dll','KingdomZombies.Multiplayer.Abstractions.dll',
    'GodotSharp.dll','System.Private.CoreLib.dll','hostfxr.dll','KingdomZombies.deps.json','KingdomZombies.runtimeconfig.json'])
    need(names.includes(name)&&fs.statSync(path.join(directory,name)).size>0,'EXPORT_DEPENDENCY_MISSING',name);
  const config=document(directory,'KingdomZombies.runtimeconfig.json'),deps=document(directory,'KingdomZombies.deps.json');
  need(config.runtimeOptions.tfm==='net10.0'&&!config.runtimeOptions.framework&&!config.runtimeOptions.frameworks&&
    config.runtimeOptions.includedFrameworks?.some(x=>x.name==='Microsoft.NETCore.App'&&x.version==='10.0.15')&&
    deps.runtimeTarget.name.endsWith('/win-x64'),'EXPORT_SELF_CONTAINED');
  return {runtimeconfig:config,runtimeTarget:deps.runtimeTarget,
    files:names.filter(x=>fs.statSync(path.join(directory,x)).isFile()).map(name=>{
      const data=fs.readFileSync(path.join(directory,name));return {path:name,bytes:data.length,sha256:hash(data)};})};
}
async function compile(context){
  const initial=snapshot();write(context.directory,'inputs.json',initial);
  const tools=document(ROOT,'tools/phase1/toolchain.json'),record=document(ROOT,ORIGIN),registry=document(ROOT,REGISTRY);
  need(process.platform==='win32'&&process.arch==='x64'&&process.version==='v'+tools.node,'EXPORT_LAYOUT_PLATFORM');
  need(registry.sources.length===4&&registry.members.length===7&&registry.exportSources.length===6&&
    JSON.stringify(record.exportSources)===JSON.stringify(registry.exportSources),'EXPORT_SOURCE_REGISTRY');
  const sdk=await runner.command(context,'sdk-version',DOTNET,['--version']);need(sdk.stdout.trim()===tools.sdk,'SDK_PIN');
  need(record.producer.path==='C:/Program Files/dotnet/sdk/10.0.404/Microsoft.Common.CurrentVersion.targets'&&
    record.producer.sdk===tools.sdk&&record.producer.symbol==='GenerateTargetFrameworkMonikerAttribute','EXPORT_PRODUCER_IDENTITY');
  const producer=fs.readFileSync(record.producer.path);
  need(hash(producer)===record.producer.sha256&&hash(producer)===
    '428d17faaf0737fba66feb5063f9a8c0d06a2ece7c42e6cca4a8a65220f80fbe','EXPORT_PRODUCER_HASH');
  write(context.directory,'producer.targets',producer);
  const retained=registry.exportSources.map(row=>authenticate(row,ROOT,row.surface));write(context.directory,'retained-sources.json',retained);
  const tracked=await runner.command(context,'tracked-inputs','git',['-C',ROOT,'ls-files','--','src','tools/code-rules',
    'Directory.*','BannedSymbols*','.editorconfig','global.json','NuGet.config','KingdomZombies.slnx']);
  const results=[];
  for(const configuration of ['ExportDebug','ExportRelease']){
    const source=inside(context.directory,configuration+'/source');fs.mkdirSync(source,{recursive:true});
    const inputs={};for(const name of tracked.stdout.trim().split(/\r?\n/)){
      const data=bytes(ROOT,name);inputs[name]=hash(data);write(source,name,data);
    }
    write(context.directory,configuration+'/project-inputs.json',inputs);
    const nuget=inside(source,'NuGet.config');fs.writeFileSync(nuget,fs.readFileSync(nuget,'utf8')
      .replace('value=".phase1/feed"','value="'+ROOT+'/.phase1/feed"'));
    const project=inside(source,'src/godot-game/KingdomZombies.csproj'),publish=inside(context.directory,configuration+'/publish');
    const flags=['-p:NuGetAudit=false','-p:UseSharedCompilation=false','-nodeReuse:false',
      '-p:NuGetPackageRoot='+ROOT+'/.phase1/packages/','-p:RestorePackagesPath='+ROOT+'/.phase1/packages'];
    const effective=await runner.command(context,configuration+'-policy',DOTNET,['msbuild',project,'-nologo',
      '-p:Configuration='+configuration,'-p:RuntimeIdentifier=win-x64','-p:SelfContained=true',...flags,
      '-getProperty:Configuration,TargetFramework,RuntimeIdentifier,SelfContained,RunAnalyzers,RunAnalyzersDuringBuild,EnableNETAnalyzers,TreatWarningsAsErrors,MSBuildTreatWarningsAsErrors,CodeAnalysisTreatWarningsAsErrors,NoWarn,WarningsNotAsErrors',
      '-getItem:ProjectReference']);
    const evaluated=policy(effective,configuration);
    const published=await runner.command(context,configuration+'-publish',DOTNET,['publish',project,'-c',configuration,'-r','win-x64',
      '--self-contained','true','-o',publish,'--disable-build-servers','-p:RestoreLockedMode=false',...flags,'-v:normal']);
    need(published.stdout.includes('/analyzer:')&&published.stdout.includes('Sebreiro.CodeRules.Analyzers.dll')&&
      published.stdout.includes('/warnaserror+'),'EXPORT_COMPILER_ANALYZERS_REQUIRED',configuration);
    const observed=registry.exportSources.filter(row=>row.configuration===configuration).map(row=>authenticate(row,source,row.path));
    need(observed.length===3,'EXPORT_SOURCE_COUNT');
    const analyzer=inside(source,'tools/code-rules/bin/'+configuration+'/netstandard2.0/Sebreiro.CodeRules.Analyzers.dll');
    const closure=dependencies(publish);
    const result={configuration,status:'pass',policy:evaluated,sources:observed,analyzerSha256:hash(fs.readFileSync(analyzer)),
      command:configuration+'-publish.command.json',closure};
    write(context.directory,configuration+'/compile.json',result);results.push(result);
  }
  need(JSON.stringify(snapshot())===JSON.stringify(initial),'EXPORT_LAYOUT_INPUT_CHANGED');
  write(context.directory,'configurations.json',results);
  return {configurations:results.map(x=>({configuration:x.configuration,status:x.status,analyzerSha256:x.analyzerSha256})),
    scope:'source/headless Windows managed publish only',nativeExport:false,linuxExecution:false,acceptance:'pending-04-32/04-33'};
}
runner.registerCase('export.layout',{group:'export',title:'Exact SDK export layout with strict real managed compiles',importance:'Critical',
  rationale:'The actual exporter cannot resume until both real SDK configurations compile without weakened analyzers',
  needsGameWindow:false,runGroup:'G0',order:5,requirements:['ASSET-05'],acceptance:[],tags:['ASSET-05','prerequisite','source-only'],
  setup:'Authenticated Windows x64 SDK 10.0.404, six observed immutable sources, pinned local Godot feed/cache; no new packages',
  actions:['Authenticate producer and retained exact source bytes','Copy tracked current source into separate owned configurations',
    'Require enabled analyzer and warnings-as-errors properties plus exact tooling edge','Publish genuine self-contained net10.0/win-x64 ExportDebug and ExportRelease',
    'Authenticate fresh generated sources and managed runtime dependencies; preserve all compiler evidence'],
  expected:'Both genuine managed export configurations pass with original analyzers; no native export/content/Linux acceptance',
  reset:'Fresh owned copies and local home/temp/cache per invocation; each child bounded to 60 seconds with descendant cleanup; preserve logs',
  evidence:['inputs.json','sdk-version.command.json','producer.targets','retained-sources.json','ExportDebug-policy.command.json',
    'ExportRelease-policy.command.json','ExportDebug-publish.command.json','ExportRelease-publish.command.json','configurations.json','result.json']},compile);
function selfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('export.layout registers its bounded source-only compiler procedure',()=>{
    const item=runner.cases.get('export.layout');
    assert.ok(item,'export.layout must have an executable saved case before export can resume');
    assert.equal(item.needsGameWindow,false);assert.equal(item.runGroup,'G0');
    assert.equal(item.importance,'Critical');assert.equal(typeof item.run,'function');
  });
}
module.exports={snapshot,sourceFile,authenticate,policy,dependencies,compile,selfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
