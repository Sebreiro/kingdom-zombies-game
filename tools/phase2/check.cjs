'use strict';
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const { spawnSync } = require('node:child_process');
const baseline = require('../phase1/check.cjs');
const { tracer, command } = require('./rendered.cjs');
const hash = bytes => crypto.createHash('sha256').update(bytes).digest('hex');
function requireTrue(value, message) { if (!value) throw new Error(message); }
function select(catalog, args) {
  requireTrue(Array.isArray(catalog.cases) && catalog.cases.length === 16, 'CATALOG_REQUIRED');
  const ids = new Set();
  for (const row of catalog.cases) {
    requireTrue(typeof row.id === 'string' && !ids.has(row.id) && ['core','rendered','runner'].includes(row.group) && ['pending','csharp','native'].includes(row.implementation), 'CATALOG_INVALID');
    ids.add(row.id);
  }
  if (args.length === 1 && args[0] === '--list') return { list: true, selected: catalog.cases };
  requireTrue(args.length === 1 && args[0] === '--all' || args.length === 2 && ['--case','--group'].includes(args[0]), 'USAGE: --case ID | --group GROUP | --all | --list');
  const selected = catalog.cases.filter(row => args[0] === '--all' || (args[0] === '--case' ? row.id === args[1] : row.group === args[1]));
  requireTrue(selected.length > 0, 'SELECTION_EMPTY');
  return { list: false, selected };
}
function reconcile(selected, results, context) {
  requireTrue(results.length > 0 && results.length === selected.length, 'RESULT_COUNT');
  const expected = new Map(selected.map(row => [row.id,row])), seen = new Set();
  for (const result of results) {
    requireTrue(expected.has(result.id) && !seen.has(result.id), 'RESULT_UNKNOWN_DUPLICATE'); seen.add(result.id);
    requireTrue(result.status === 'pass', 'RESULT_NOT_PASS ' + result.id);
    for (const key of ['nonce','input','tool','platform']) requireTrue(result[key] === context[key], 'RESULT_IDENTITY ' + key);
    for (const name of expected.get(result.id).images ?? []) requireTrue(result.images?.some(image => image.checkpoint === name), 'RESULT_IMAGE_MISSING ' + name);
  }
  return results;
}
async function main(root, args) {
  const catalog = JSON.parse(fs.readFileSync(path.join(root, 'tools/phase2/required-cases.json'), 'utf8'));
  const selection = select(catalog,args);
  if (selection.list) { console.log(JSON.stringify(selection.selected,null,2)); return; }
  const nonce = crypto.randomUUID(), platform = process.platform + '-' + process.arch;
  const directory = path.join(root,'.phase2/runs',platform,nonce);
  fs.mkdirSync(directory,{recursive:true});
  const started = Date.now(), results = [], failures = [];
  const manifest = { inputs: ['src','tests/phase1','tools/phase2','tools/phase1/toolchain.json','Directory.Build.props','Directory.Build.targets','Directory.Packages.props','.editorconfig','BannedSymbols.txt','BannedSymbols.Pure.txt','docs/code'] };
  const snapshot = baseline.snapshot(root,manifest);
  const pins = JSON.parse(fs.readFileSync(path.join(root,'tools/phase1/toolchain.json'),'utf8'));
  const archive = pins.archives[platform];
  requireTrue(archive, 'PLATFORM_UNSUPPORTED');
  const engine = path.join(root,'.phase1/tools',archive.directory,archive.executable);
  const env = { ...process.env, DOTNET_CLI_HOME:path.join(root,'.phase1/cli-home'), NUGET_PACKAGES:path.join(root,'.phase1/packages'), NUGET_HTTP_CACHE_PATH:path.join(root,'.phase1/http-cache'), TEMP:path.join(root,'.phase1/temp'), TMP:path.join(root,'.phase1/temp') };
  const nativeEngine = process.platform === 'win32' ? engine.replace('_console.exe','.exe') : engine;
  const context = { root,directory,nonce,platform,input:snapshot.sha256,tool:null,engine:nativeEngine,env };
  fs.writeFileSync(path.join(directory,'inputs.json'),JSON.stringify(snapshot,null,2));
  try {
    requireTrue(fs.existsSync(engine),'ENGINE_MISSING');
    const executableHash = hash(fs.readFileSync(engine));
    if (archive.executableSha256) requireTrue(executableHash === archive.executableSha256,'ENGINE_HASH');
    if (process.platform === 'win32') requireTrue(hash(fs.readFileSync(nativeEngine)) === '45336315eb6f1a52a8923bc4f2ce8079a03dc4939dcb7d531047890f1f7cdfab','NATIVE_ENGINE_HASH');
    const version = await command(engine,['--version'],{cwd:root,env});
    requireTrue(version.stdout.trim() === pins.engineFullVersion,'ENGINE_VERSION');
    context.tool = hash(JSON.stringify({engine:executableHash,nativeEngine:hash(fs.readFileSync(nativeEngine)),node:process.version,driver:hash(fs.readFileSync(path.join(root,process.platform === 'win32'?'tools/phase2/windows-input.ps1':'tools/phase2/linux-input.sh')))}));
    for (const item of selection.selected) {
      console.log('Checking ' + item.id);
      requireTrue(item.implementation !== 'pending','PENDING_OBLIGATION ' + item.id);
      let result;
      if (item.implementation === 'native') {
        const caseDirectory=path.join(directory,item.id);
        fs.mkdirSync(caseDirectory,{recursive:true});
        const native = await tracer({...context,directory:caseDirectory},item.id);
        result = {id:item.id,status:'pass',nonce,input:context.input,tool:context.tool,platform,...native};
        for (const checkpoint of item.images) {
          const image=result.images.find(image=>image.checkpoint===checkpoint);
          requireTrue(image && fs.existsSync(image.path),'IMAGE_MISSING');
          image.sha256=hash(fs.readFileSync(image.path));
        }
      } else {
        const testEnv = {...env, PHASE1_ROOT:root,PHASE1_RESULT_DIRECTORY:directory,PHASE1_NONCE:nonce,PHASE1_INPUT:context.input,PHASE1_TOOL:context.tool,PHASE1_PLATFORM:platform};
        const run = await command('dotnet',[path.join(root,'tests/phase1/bin/Debug/net10.0/KingdomZombies.Phase1.Tests.dll'),'--filter-method',item.test,'--report-xunit-ctrf','--report-xunit-ctrf-filename',item.id+'.ctrf.json','--results-directory',directory],{cwd:root,env:testEnv},180000);
        fs.writeFileSync(path.join(directory,item.id+'.console.log'),run.stdout+run.stderr);
        const report = JSON.parse(fs.readFileSync(path.join(directory,item.id+'.ctrf.json'),'utf8'));
        requireTrue(report.results.summary.tests > 0 && report.results.summary.failed === 0 && report.results.summary.passed === report.results.summary.tests,'TEST_EXECUTION_COUNT');
        result=JSON.parse(fs.readFileSync(path.join(directory,item.id+'.proof.json'),'utf8'));
      }
      fs.writeFileSync(path.join(directory,item.id+'.result.json'),JSON.stringify(result,null,2)); results.push(result);
    }
    reconcile(selection.selected,results,context);
    requireTrue(baseline.snapshot(root,manifest).sha256===context.input,'SOURCE_CHANGED');
  } catch (error) { failures.push({message:error.message,stack:error.stack}); }
  const status=failures.length?'fail':'pass';
  const summary={schemaVersion:1,nonce,platform,input:context.input,tool:context.tool,status,selected:selection.selected.map(row=>row.id),executed:results.length,results,failures,durationMilliseconds:Date.now()-started,revision:spawnSync('git',['rev-parse','HEAD'],{cwd:root,encoding:'utf8',windowsHide:true}).stdout.trim()};
  fs.writeFileSync(path.join(directory,'summary.json'),JSON.stringify(summary,null,2),{flag:'wx'});
  console.log(`${status.toUpperCase()} ${results.length}/${selection.selected.length} selected obligations; ${path.relative(root,path.join(directory,'summary.json'))}`);
  for(const failure of failures) console.error(failure.message);
  if(status!=='pass')process.exitCode=1;
  return summary;
}
module.exports={main,select,reconcile};
if(require.main===module)main(path.resolve(__dirname,'../..'),process.argv.slice(2)).catch(error=>{console.error(error.stack);process.exitCode=1;});
