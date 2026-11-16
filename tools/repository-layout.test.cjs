'use strict';
const test=require('node:test'),assert=require('node:assert/strict');
const fs=require('node:fs'),path=require('node:path'),os=require('node:os');
const {launch}=require('./dotnet.cjs');
const root=path.resolve(__dirname,'..'),workspace=path.join(root,'src/godot-game');

test('solution resolves exactly the five projects from its own directory',()=>{
  const expected=['core/KingdomZombies.Core.csproj','multiplayer-abstractions/KingdomZombies.Multiplayer.Abstractions.csproj',
    'game/KingdomZombies.csproj','tests/verification/KingdomZombies.Phase1.Tests.csproj',
    '../../tools/code-rules/Sebreiro.CodeRules.Analyzers.csproj'];
  const projects=[...fs.readFileSync(path.join(workspace,'KingdomZombies.slnx'),'utf8').matchAll(/<Project Path="([^"]+)"/g)].map(x=>x[1]);
  assert.deepEqual([...projects].sort(),[...expected].sort());
  for(const project of projects)assert.ok(fs.statSync(path.resolve(workspace,project)).isFile(),project);
  assert.ok(fs.existsSync(path.join(workspace,'game/project.godot')));
  for(const name of ['KingdomZombies.slnx','Directory.Build.props','Directory.Build.targets','Directory.Packages.props',
    'Directory.Xunit.targets','BannedSymbols.txt','BannedSymbols.Pure.txt','BannedSymbols.Tests.txt','global.json','NuGet.config'])
    assert.equal(fs.existsSync(path.join(root,name)),false,'root policy must stay in the workspace: '+name);
});

test('automated dotnet launches select workspace SDK policy without losing subject paths',()=>{
  const args=['build','src/godot-game/game/KingdomZombies.csproj','--no-restore','-p:UseSharedCompilation=false'];
  const configured=launch(root,'dotnet',args);
  assert.equal(configured.cwd,workspace);
  assert.equal(configured.args[1],path.join(workspace,'game/KingdomZombies.csproj'));
  assert.equal(configured.args[3],args[3]);
  assert.equal(args[1],'src/godot-game/game/KingdomZombies.csproj');
  assert.deepEqual(launch(root,'node',args),{cwd:root,args});
  assert.equal(launch(root,path.join('C:/sdk','dotnet.exe'),args).cwd,workspace);
});

test('fixture roots without workspace policy retain their own working directory',()=>{
  const fixture=fs.mkdtempSync(path.join(os.tmpdir(),'kz-layout-test-'));
  try{
    const args=['--version'];
    assert.deepEqual(launch(fixture,'dotnet',args),{cwd:fixture,args});
  }finally{fs.rmdirSync(fixture);}
});

test('authored C# and game resources stay in the workspace, with the reusable analyzer exception',()=>{
  const authored=[];
  function visit(directory,relative=''){
    for(const entry of fs.readdirSync(directory,{withFileTypes:true})){
      if(entry.name.startsWith('.')||['bin','obj','node_modules','docs','specs'].includes(entry.name))continue;
      const file=relative?relative+'/'+entry.name:entry.name;
      if(entry.isDirectory())visit(path.join(directory,entry.name),file);
      else if(entry.isFile()&&/\.(?:cs|csproj|slnx|tscn|tres|png|svg|wav|ogg)$/i.test(entry.name))authored.push(file);
    }
  }
  visit(root);
  assert.ok(authored.some(file=>file.startsWith('src/godot-game/references/')),'reference artwork is checked');
  for(const file of authored)assert.ok(file.startsWith('src/godot-game/')||file.startsWith('tools/code-rules/'),file);
});
