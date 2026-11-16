'use strict';
const fs=require('node:fs'),path=require('node:path');
const ROOT=path.resolve(__dirname,'../../..');
function selfTest(){
  const {test}=require('node:test'),assert=require('node:assert/strict');
  test('inspection export feature selects its own entry while ordinary Foundation stays literal',()=>{
    const project=fs.readFileSync(path.join(ROOT,'src/godot-game/project.godot'),'utf8');
    const preset=fs.readFileSync(path.join(ROOT,'src/godot-game/export_presets.cfg'),'utf8');
    assert.match(project,/^run\/main_scene="res:\/\/foundation\/Foundation\.tscn"$/m);
    assert.match(project,/^run\/main_scene\.asset_inspection="res:\/\/inspection\/Inspection\.tscn"$/m,
      'the permanent feature-qualified inspection entry is absent');
    assert.match(preset,/^custom_features="asset_inspection"$/m);
    assert.ok(fs.existsSync(path.join(ROOT,'production/phase4/export-profile.json')));
  });
}
module.exports={selfTest};
if(require.main===module&&process.argv[2]==='--self-test')selfTest();
