'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs');
const ROOT='D:/Work/Games/KingdomZombies';
const assembly=path.join(ROOT,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll');
function env(){const base=path.join(ROOT,'.phase1');return {...process.env,DOTNET_CLI_HOME:path.join(base,'cli-home'),
  NUGET_PACKAGES:path.join(base,'packages'),NUGET_HTTP_CACHE_PATH:path.join(base,'http-cache'),
  TEMP:path.join(base,'temp'),TMP:path.join(base,'temp'),PHASE1_ROOT:ROOT,DOTNET_NOLOGO:'1'};}
function compiled(){return spawnSync('dotnet',[assembly,'--filter-class','*ContentInspectorTests'],
  {cwd:ROOT,env:env(),encoding:'utf8',windowsHide:true,timeout:60000});}
runner.registerCase('hub.source',{group:'hub',title:'Independent portable register and valid partial hub resources',importance:'Critical',
  rationale:'One invalid resource must not hide valid content or erase required obligations',needsGameWindow:false,
  runGroup:'G0-hub',order:1,requirements:['ASSET-02','ASSET-03','ASSET-04','ASSET-05'],acceptance:['AC-16','AC-20'],
  setup:'Compiled production Core and saved compiled loader tests; actual bow/index/independent register',
  actions:['Run compiled loader behavior and boundary controls','Observe the exact independent180 census and bow'],
  expected:'Nonzero test discovery; valid bow survives independent failures; invalid paths and definitions fail visibly',
  reset:'Pure detached loader snapshots; no game window',evidence:['compiled.command.json','result.json']},async context=>{
    const result=compiled();fs.writeFileSync(path.join(context.directory,'compiled.command.json'),JSON.stringify(result,null,2));
    assert.equal(result.status,0,result.stdout+'\n'+result.stderr);assert.match(result.stdout,/summary: Passed!/);
    const discovered=/total:\s*(\d+)/.exec(result.stdout);assert.ok(discovered&&Number(discovered[1])>=26,'NONZERO_REQUIRED_DISCOVERY');
    const inputs=Object.fromEntries(['src/core/ContentPack.cs','src/godot-game/inspection/InspectionRoot.cs',
      'src/godot-game/inspection/Inspection.tscn','src/godot-game/content/pack-index.json',
      'src/godot-game/content/expected-obligations.json','src/godot-game/content/packs/tracer.json',
      'src/godot-game/assets/common/bow.png','tests/phase1/ContentInspectorTests.cs',
      'tools/phase4/cases/hub.cjs'].map(file=>[file,runner.hash(fs.readFileSync(path.join(ROOT,file)))]));
    fs.writeFileSync(path.join(context.directory,'inputs.json'),JSON.stringify(inputs,null,2));
    return {compiled:true,discovered:Number(discovered[1]),scope:'pure loader; actual native hub authorization follows Task2'};
  });
if(require.main===module&&process.argv.includes('--self-test')){
  require('node:test').test('valid bow survives incomplete independent register',()=>{
    const result=compiled();assert.equal(result.status,0,result.stdout+'\n'+result.stderr);
  });
}
