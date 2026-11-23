---
phase: quick-261123-fsu
plan: 01
type: execute
wave: 1
depends_on: []
# Conflict-resolution scope is authorized only by the live merge index observed after
# `git merge --no-ff --no-commit origin/dev` (unmerged paths plus `git status` placements).
# Entries below are the deterministic edits plus the relocated trees as directories.
files_modified:
  - .planning/STATE.md
  - docs/architecture/repository-layout.md
  - docs/code/visibility-exemptions.json
  - docs/code/engine-generated-spans.json
  - docs/code/generated-callsite-adaptations.json
  - docs/code/generated-layout-adaptations.json
  - docs/content/README.md
  - docs/content/windows-inspection.md
  - docs/content/production.md
  - docs/workflow/cleanup.md
  - docs/workflow/engine-defects.md
  - docs/workflow/setup.md
  - docs/workflow/lan-foundation-checks.md
  - docs/workflow/verification.md
  - src/godot-game/tools/README.md
  - src/godot-game/tools/repository-layout.test.cjs
  - src/godot-game/tools/workspace-cleanup.cjs
  - src/godot-game/tools/code-rules/GeneratedLayout.cs
  - src/godot-game/tools/verification/check.cjs
  - src/godot-game/tools/verification/required-cases.json
  - src/godot-game/tools/gameplay-checks/
  - src/godot-game/tools/lan-checks/
  - src/godot-game/tools/asset-pipeline/
  - src/godot-game/production/assets/
  - src/godot-game/game/
  - src/godot-game/core/
  - src/godot-game/tests/verification/
autonomous: true
requirements: [QUICK-261123-FSU]

estimate:
  tokens: 700000
  raw_tokens: 700000
  tasks: 3
  confidence: low

must_haves:
  truths:
    - "The PR branch gains exactly one merge commit, parents c5be170 (PR head) and a305b8b (origin/dev), as its first new commit, followed only by linear fix commits; no rebase, stash, force, push or GitHub change (CONTEXT: Integration method)."
    - "Every Phase 4 and PR #7 file sits at its current layout location: no tracked path under a retired root, and the 206 dev-added files are byte-identical at their mapped paths in the merge commit (CONTEXT: Scope)."
    - "Each conflicted path in the live merge index keeps both intents (dev behaviour and rows, PR paths) and its resolution is recorded in the SUMMARY."
    - "Phase 4 records keep logical identities resolved by one asset-pipeline map (layout.cjs): pack-index IDs, paths and outputs, ZIP blobs, pinned extractors and fields.md equal dev; the layout policy states the rule (CONTEXT: Phase 4 path identities)."
    - "Live asset-pipeline tools derive the checkout root from their own location, and all four runners list their catalogs from an unrelated working directory: 686, 16, 32 and 75 cases (CONTEXT: Checkout root)."
    - "Every hash binding the move breaks is regenerated and statically consistent: 423 module bindings, projectGraphSha256, every pack-index row, every ac-08 source row (path set by the SourceFileFormat rule, kind, owners, and sha256 equal to the HEAD blob and the working-tree bytes, by a rule that reproduces dev's own block), repository, caller, producer and authority hashes, 9 staged plans accepted. These are reported as rebound, statically consistent and not executed."
    - "No validator, pin, exemption or expected count is weakened; Phase 1-4 records, other quick-task records and provenance are byte-identical to their source side; no committed line adds a machine-specific path (CONTEXT: No machine paths)."
    - "At the executor's final commit, the strict locked build has 0 warnings and 0 errors, the layout tests pass 6/6 and the documentation group passes 36/36 (CONTEXT: Verification)."
    - "STATE.md keeps dev's Phase 5 position, the 7 quick-task rows in the 6-column format, the ub4 link to its SUMMARY file and the PR #6 relocation renewal bullet, with no 261123-fsu row."
    - "The uncommitted SUMMARY has status complete, a results table, the expected-blocked and not-run rows and a How to reproduce section. No file in either untracked quick-task directory is committed."
  artifacts:
    - path: src/godot-game/tools/asset-pipeline/layout.cjs
      provides: "The single logical-to-physical path map (physical(), logical()) for Phase 4 record identities"
    - path: src/godot-game/tools/repository-layout.test.cjs
      provides: "Layout tests covering asset-pipeline, retired root phase4 and root workspace-cleanup"
    - path: src/godot-game/tools/verification/required-cases.json
      provides: "Regenerated ac-08 repositoryStages and caller, producer and authority hashes"
    - path: src/godot-game/game/content/pack-index.json
      provides: "Pack index with unchanged IDs and paths and current file hashes"
    - path: docs/architecture/repository-layout.md
      provides: "Asset-pipeline map rows and the logical-identity rule"
    - path: .planning/STATE.md
      provides: "Merged state with the Phase 5 renewal handoff"
  key_links:
    - from: src/godot-game/tools/asset-pipeline/content.cjs
      to: src/godot-game/tools/asset-pipeline/layout.cjs
      via: "inside() resolves logical record paths through physical()"
    - from: src/godot-game/tools/asset-pipeline/check.cjs
      to: src/godot-game/tools/asset-pipeline/layout.cjs
      via: "inside() plus a root derived from __dirname"
    - from: src/godot-game/tools/verification/required-cases.json
      to: "ac-08.complete repositoryStages"
      via: "callerStages.repositorySha256 (5 cases here, 1 in gameplay-checks)"
    - from: src/godot-game/tools/asset-pipeline/required-cases.json
      to: src/godot-game/tools/asset-pipeline/cases/
      via: "sourceBinding.module and moduleSha256"
    - from: src/godot-game/game/content/packs/delivery.json
      to: "asset catalog and the 3 embedded guides"
      via: "fixtureCatalog gzipBase64 and guides[].sha256 (text-data validateSupport)"
    - from: src/godot-game/tests/verification/EngineAccessPolicy.cs
      to: src/godot-game/tests/verification/ContentNativePermissionTests.cs
      via: "identical inspection tuples (evidence.cjs guardedHostTuples)"
---

<objective>
Integrate `origin/dev` (Phase 4 squash and PR #7) into PR #6 (`feature/quick-repository-layout`). The work is one merge commit that resolves every live conflict with both intents and moves every Phase 4 and PR #7 file to its current location, followed by fix commits. The fix commits rebind every path and hash binding the move breaks, keep Phase 4 logical identities through one map, and update live docs and STATE.md.

Purpose: let PR #6 merge cleanly into `dev` without losing Phase 4 or PR #7 work. Locked scope: CONTEXT "Scope", "Integration method", "Verification" and "Carried from 261122-uby".
Output: the merge commit plus fix commits on the PR worktree's branch (not pushed), and an uncommitted `261123-fsu-SUMMARY.md`.
</objective>

<execution_context>
@.claude/gsd-core/workflows/execute-plan.md
@.claude/gsd-core/templates/summary.md
</execution_context>

<context>
@.planning/STATE.md
@AGENTS.md
@.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-CONTEXT.md
@.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-RESEARCH.md
@docs/architecture/repository-layout.md
@docs/workflow/phase4-layout-integration.md

Line-level detail source, read only: `.planning/quick/261122-uby-resolve-pr-6-merge-conflicts-and-check-d/261122-uby-RESEARCH.md`, sections 2 to 4. This untracked file contains machine paths. Never copy them, never stage the folder, and do not execute its PLAN.
After the merge, dev's task-end rules are in `docs/workflow/cleanup.md`; before it, read them with `git show origin/dev:docs/workflow/cleanup.md`.
</context>

## Locations and shell rules

- **PR worktree**: the only checkout you edit, build and commit in. Its root is given in your prompt. **Main checkout**: read-only seed source, also given in your prompt. Every shell call starts with `cd` to the PR worktree root and then `export PATH="$PWD/.phase1/quick-261121-jmu-node:$PATH" MSYS_NO_PATHCONV=1`. Set `MAIN_CHECKOUT` from your prompt only in the call that seeds caches. Never write either real path into any file.
- Build and check in place in the PR worktree. The orchestrator chose this under the CONTEXT discretion, so no verification worktree is created.
- The commands below assume the current directory is the PR worktree root. Run `node --version` and expect `v24.22.1`; the runners reject any other version.
- Scratch scripts, logs and messages go only in the ignored folder `.phase1/quick-261123-fsu/`. Stage with explicit pathspecs only. Never use `git add -A`, `git add .` or `git commit -a`. The two untracked `.planning/quick/261122-uby-*` and `261123-fsu-*` folders are never staged or committed.
- Write repository files with the Edit or Write tools or with Node (LF, no BOM), never with PowerShell `Set-Content` or `Out-File`. Keep C# lines within the 120-column analyzer rule.
- No downloads or installs. `git fetch origin` is the only network step. Never push and never touch GitHub. Never delete the pinned node folder or the seeded caches; the orchestrator needs them for its final documentation run.
- Every commit message ends with the line `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. Use `git commit -F .phase1/quick-261123-fsu/NAME.txt`.
- Do not weaken, skip or broaden a validator, pin, exemption or expected count to get a pass. Rebind only what the move requires. No line you write may contain a machine-specific path. Where an edited line already has one, derive the value instead.

## Gate script

Session setup S3 extracts the block between the markers verbatim to `.phase1/quick-261123-fsu/gates.cjs`. Every `<automated>` check runs it as `node .phase1/quick-261123-fsu/gates.cjs GATE...` from the PR worktree root. Each gate prints `PASS ...` or exits 1 with `FAIL GATE: REASON`. Gates `merge`, `moves`, `history`, `lists`, `layout`, `bindings`, `state` and `records` fail on the current PR head by design. `newhost` is an invariant gate and already passes there.

<!-- gates.cjs begin -->
~~~js
'use strict';
// 261123-fsu gates. From the PR worktree root with the pinned node: node .phase1/quick-261123-fsu/gates.cjs <gate>...
const fs=require('fs'),path=require('path'),os=require('os'),cp=require('child_process'),crypto=require('crypto');
const PR='c5be170a4df039ffb54a5a2f903c5c9b305a56cf',DEV='a305b8b58a6d17722a1ddb3ba4561849d54a2a49',BASE='5428da86b6dc753d43472ea8460acb997d7e15dd';
const ROOT=process.cwd(),W='src/godot-game/',AP=W+'tools/asset-pipeline/',Q='.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/';
const git=(a,ok=[0])=>{const r=cp.spawnSync('git',a,{cwd:ROOT,encoding:'utf8',maxBuffer:1<<30});if(!ok.includes(r.status))throw Error('GIT '+a.join(' ')+' '+r.stderr);return r.stdout;};
const tree=rev=>new Map(git(['ls-tree','-r',rev]).split('\n').filter(Boolean).map(l=>{const[a,p]=l.split('\t');return[p,a.split(' ')[2]];}));
const sha=b=>crypto.createHash('sha256').update(b).digest('hex'),file=p=>fs.readFileSync(path.join(ROOT,p)),json=p=>JSON.parse(file(p));
const fail=(g,m)=>{console.error('FAIL '+g+': '+m);process.exit(1);};
const mergeCommit=()=>{const rows=git(['rev-list','--first-parent','--parents',PR+'..HEAD']).split('\n').filter(Boolean).map(l=>l.split(' '));
  if(!rows.length)fail('merge','no commits after the PR head');const m=rows[rows.length-1];
  if(m.length!==3||m[1]!==PR||m[2]!==DEV)fail('merge','oldest new commit is not the PR-head x origin/dev merge');
  const extra=rows.slice(0,-1).filter(r=>r.length!==2);if(extra.length)fail('merge','non-linear fix commits '+extra.map(r=>r[0]).join(' '));return m[0];};
const MAP=[['tools/phase4/',AP],['production/phase4/',W+'production/assets/'],['src/core/',W+'core/'],['tests/phase1/',W+'tests/verification/'],
  ['tools/workspace-cleanup',W+'tools/workspace-cleanup'],['src/godot-game/assets/',W+'game/assets/'],['src/godot-game/content/',W+'game/content/'],
  ['src/godot-game/inspection/',W+'game/inspection/'],['src/godot-game/export_presets.cfg',W+'game/export_presets.cfg']];
const RETIRED=['tools','production','src/core','src/multiplayer-abstractions','tests',W+'tools/phase4',W+'assets',W+'content',W+'inspection',W+'proofs',
  W+'foundation',W+'docs',W+'project.godot',W+'export_presets.cfg',W+'KingdomZombies.csproj'];
const phys=p=>{for(const[a,b]of[['tools/phase4/',AP],['production/phase4/',W+'production/assets/']])if(p.startsWith(a))return b+p.slice(a.length);
  return /^src\/godot-game\/(?:(?:assets|content|inspection|foundation|proofs|docs)\/|(?:project\.godot|export_presets\.cfg|KingdomZombies\.csproj|override\.cfg)$)/.test(p)?W+'game/'+p.slice(W.length):p;};
const HISTORY=/^(\.planning\/phases\/0[1-4]-|\.planning\/quick\/(?!261122-uby-|261123-fsu-)|docs\/code\/provenance\/)/;
const HOST=/(^|[^A-Za-z0-9_])[A-Za-z]:[\/\x5c][A-Za-z]|\/[a-z]\/(Users|Work)\b|App[D]ata|Users[\/\x5c]User\b/;
const hostLines=rev=>{const out=git(['grep','-I','-n','-E','[A-Za-z]:[/\x5c][A-Za-z]|/[a-z]/(Users|Work)|App[D]ata|Users[/\x5c]User',rev],[0,1]);
  return out.split('\n').filter(Boolean).map(l=>/^[^:]+:(.+?):(\d+):(.*)$/.exec(l)).filter(m=>m&&HOST.test(m[3])).map(m=>({at:m[1]+':'+m[2],text:m[3]}));};
const blobs=(rev,ps)=>{const r=cp.spawnSync('git',['cat-file','--batch'],{cwd:ROOT,input:ps.map(p=>rev+':'+p+'\n').join(''),maxBuffer:1<<30}),o=r.stdout,m=new Map();let i=0;
  if(r.status!==0)throw Error('GIT cat-file '+r.stderr);
  for(const p of ps){const e=o.indexOf(10,i),h=o.toString('latin1',i,e).split(' '),n=Number(h[2]);if(h[1]!=='blob')throw Error('GIT cat-file '+rev+':'+p+' '+h.join(' '));m.set(p,o.subarray(e+1,e+1+n));i=e+2+n;}
  return m;};
// ac-08 rows at rev by research 3c: SourceFileFormat path set, kind by extension, sha256 of the rev blob (null when outside-validator),
// owners = projects whose folder holds the file plus the test project for its linked Compile items, in projects order. Returns the bad rows.
const acRows=(rev,ac)=>{const T=(ac.projects||[]).find(p=>p.endsWith('/KingdomZombies.Phase1.Tests.csproj'));if(!T)return['test project'];
  const dir=p=>p.slice(0,p.lastIndexOf('/')+1),src=blobs(rev,[dir(T)+'SourceFileFormat.cs',T]);
  const bin=new Set([...src.get(dir(T)+'SourceFileFormat.cs').toString().match(/HashSet<string> _binaryExtensions[^{]*\{([^}]*)\}/)[1].matchAll(/"(\.[a-z0-9]+)"/g)].map(m=>m[1]));
  const linked=new Set([...src.get(T).toString().matchAll(/<Compile\s+Include="([^"]+)"/g)].map(m=>path.posix.normalize(dir(T)+m[1])));
  const ext=p=>{const b=p.split('/').pop(),i=b.lastIndexOf('.');return i<0?'':b.slice(i).toLowerCase();},kind=p=>ext(p)==='.cs'?'source':ext(p)==='.editorconfig'?'configuration':'outside-validator';
  const elig=git(['ls-tree','-r','-z','--name-only',rev]).split('\0').filter(Boolean).filter(p=>!/^\.(claude|codex|agents|planning)\//.test(p)&&!bin.has(ext(p))).sort();
  if(JSON.stringify(ac.sources.map(s=>s.path))!==JSON.stringify(elig))return['paths '+ac.sources.length+' declared, '+elig.length+' by the rule'];
  const b=blobs(rev,elig.filter(p=>kind(p)!=='outside-validator')),bad=[];
  for(const s of ac.sources){const k=kind(s.path),own=k==='source'?ac.projects.filter(x=>s.path.startsWith(dir(x))||(x===T&&linked.has(s.path))):[];
    if(s.kind!==k||s.sha256!==(k==='outside-validator'?null:sha(b.get(s.path)))||JSON.stringify(s.owners)!==JSON.stringify(own))bad.push(s.path);}
  return bad;};
const G={
  merge(){const m=mergeCommit();if(git(['ls-files','-u']).trim())fail('merge','unmerged index entries');
    const mk=git(['grep','-n','-I','-E','^(<{7}|>{7})( |$)','HEAD'],[0,1]).trim();if(mk)fail('merge','conflict markers '+mk.slice(0,400));return 'merge '+m.slice(0,9);},
  moves(){const m=mergeCommit(),B=tree(BASE),D=tree(DEV),M=tree(m),bad=[];let n=0;
    for(const[p,b]of D){if(B.has(p))continue;const r=MAP.find(([a])=>p.startsWith(a));if(!r)continue;n++;const q=r[1]+p.slice(r[0].length);if(M.get(q)!==b)bad.push(q);}
    if(n!==206||bad.length)fail('moves',n+' dev-added mapped files, '+bad.length+' not byte-identical in the merge commit: '+bad.slice(0,8).join(' '));
    const left=git(['ls-tree','-r','--name-only','HEAD','--',...RETIRED]).trim();if(left)fail('moves','retired paths at HEAD: '+left.split('\n').slice(0,8).join(' '));
    return 'moves 206';},
  history(){const B=tree(BASE),P=tree(PR),D=tree(DEV),H=tree('HEAD'),bad=[];const ps=new Set([...P.keys(),...D.keys(),...H.keys()].filter(p=>HISTORY.test(p)));
    for(const p of ps){const want=P.get(p)===B.get(p)?D.get(p):P.get(p);if(H.get(p)!==want)bad.push(p);}
    if(bad.length)fail('history',bad.length+' record(s) differ from their source side: '+bad.slice(0,8).join(' '));return 'history '+ps.size;},
  newhost(){const old=new Set([PR,DEV].flatMap(r=>hostLines(r).map(x=>x.text)));const add=hostLines('HEAD').filter(x=>!old.has(x.text));
    if(add.length)fail('newhost',add.length+' new machine-path line(s): '+add.slice(0,8).map(x=>x.at).join(' '));return 'newhost 0';},
  lists(){const want={'verification':686,'gameplay-checks':16,'lan-checks':32,'asset-pipeline':75},got=[];
    for(const[f,k]of Object.entries(want)){const r=cp.spawnSync(process.execPath,[path.join(ROOT,W,'tools',f,'check.cjs'),'--list'],{cwd:os.tmpdir(),encoding:'utf8',maxBuffer:1<<30});
      if(r.status!==0)fail('lists',f+' exit '+r.status+' '+r.stderr.slice(0,300));const n=JSON.parse(r.stdout).length;if(n!==k)fail('lists',f+' '+n+' expected '+k);got.push(f+'='+n);}
    return 'lists '+got.join(' ');},
  bindings(){const cat=json(AP+'required-cases.json'),bad=[];let n=0;
    (function walk(o){if(o&&typeof o==='object'){if(typeof o.module==='string'&&typeof o.moduleSha256==='string'){n++;
      if(!o.module.startsWith(AP)||!fs.existsSync(path.join(ROOT,o.module))||sha(file(o.module))!==o.moduleSha256)bad.push(o.module);}for(const k in o)walk(o[k]);}})(cat);
    if(n!==423||bad.length)fail('bindings',n+' module bindings, stale: '+[...new Set(bad)].slice(0,8).join(' '));
    const graph=sha(file(W+'tools/verification/projects.json'));
    if(json(W+'production/assets/export-profile.json').sourceContract.projectGraphSha256!==graph||!file(AP+'cases/export-profile.cjs').toString().includes(graph))fail('bindings','projectGraphSha256');
    const P1=json(W+'tools/verification/required-cases.json'),P2=json(W+'tools/gameplay-checks/required-cases.json'),ac=P1.cases.find(c=>c.id==='ac-08.complete').repositoryStages;
    const cal=acRows(DEV,JSON.parse(git(['show',DEV+':tools/phase1/required-cases.json'])).cases.find(c=>c.id==='ac-08.complete').repositoryStages);
    if(cal.length)fail('bindings','the ac-08 row rule does not reproduce dev: '+cal.slice(0,8).join(' '));
    const rows=acRows('HEAD',ac);if(rows.length)fail('bindings','ac-08 rows differ from paths, kinds, owners or HEAD sha256: '+rows.slice(0,8).join(' '));
    const disk=ac.sources.filter(s=>s.sha256!==null&&(!fs.existsSync(path.join(ROOT,s.path))||sha(file(s.path))!==s.sha256)).map(s=>s.path);
    if(disk.length)fail('bindings','ac-08 sha256 differs from the working tree: '+disk.slice(0,8).join(' '));
    if(sha(JSON.stringify(ac.sources))!==ac.sourceSetSha256||ac.keys.length!==42)fail('bindings','ac-08 sourceSetSha256 or key count');
    const proj=git(['ls-tree','-r','--name-only','HEAD','--',W]).split('\n').filter(p=>p.endsWith('.csproj')).sort();
    if(JSON.stringify([...ac.projects].sort())!==JSON.stringify(proj))fail('bindings','ac-08 projects '+JSON.stringify(ac.projects));
    const rs=sha(JSON.stringify(ac)),hashed=[...ac.bindings,P1.callerAuthority&&{path:P1.callerAuthority.registry,sha256:P1.callerAuthority.sha256}];
    for(const c of[...P1.cases,...P2.cases]){if(c.callerStages&&(c.callerStages.repositorySha256!==rs||!c.callerStages.catalog.startsWith(W)||!fs.existsSync(path.join(ROOT,c.callerStages.catalog))))bad.push('caller '+c.id);
      if(c.producerStages)for(const r of Object.values(c.producerStages.registries||{}))hashed.push(r);}
    for(const h of hashed)if(!h||sha(file(h.path))!==h.sha256)bad.push('hash '+(h&&h.path));
    const v=require(path.join(ROOT,W,'tools/verification/check.cjs'));let staged=0;
    for(const c of[...P1.cases,...P2.cases])if(c.repositoryStages||c.callerStages||c.producerStages){try{v.producerPlan(c,ROOT);staged++;}catch(e){bad.push('plan '+c.id+' '+(e.diagnostic||e.message));}}
    if(staged!==9)bad.push('staged '+staged);
    const r=require(path.join(ROOT,AP,'content.cjs')).validate(ROOT);if(!Array.isArray(r.valid)||r.valid.length!==180)bad.push('content.validate '+(r.valid&&r.valid.length));
    try{require(path.join(ROOT,AP,'cases/text-data.cjs')).validateSupport();}catch(e){bad.push('validateSupport '+e.message);}
    const ix=json(W+'game/content/pack-index.json');for(const f of[...ix.files,ix.expectedObligations]){const b=file(phys(f.path));if(b.length!==f.bytes||sha(b)!==f.sha256)bad.push('index '+f.path);}
    const zips=t=>[...t].filter(([p])=>p.endsWith('.zip')).map(([,b])=>b).sort().join(),D=tree(DEV),H=tree('HEAD');if(zips(D)!==zips(H))bad.push('zip blobs');
    for(const x of['extract-media.py','extract-audio.py'])if(D.get('tools/phase4/'+x)!==H.get(AP+x))bad.push('pinned '+x);
    if(D.get('docs/content/fields.md')!==H.get('docs/content/fields.md'))bad.push('fields.md');
    const ids=s=>JSON.stringify([s.files.map(f=>[f.id,f.path]).sort(),s.outputs]);
    if(ids(JSON.parse(git(['show',DEV+':src/godot-game/content/pack-index.json'])))!==ids(ix))bad.push('pack-index ids/paths/outputs');
    if(bad.length)fail('bindings',bad.slice(0,12).join(' | '));return 'bindings modules='+n+' staged='+staged+' content=180 index='+ix.files.length;},
  build(){const env={...process.env},log=n=>path.join(ROOT,'.phase1/quick-261123-fsu',n);
    for(const[k,d]of[['DOTNET_CLI_HOME','cli-home'],['NUGET_PACKAGES','packages'],['NUGET_HTTP_CACHE_PATH','http-cache'],['TEMP','temp'],['TMP','temp']]){env[k]=path.join(ROOT,'.phase1',d);fs.mkdirSync(env[k],{recursive:true});}
    const run=(args,n)=>{const r=cp.spawnSync('dotnet',args,{cwd:path.join(ROOT,W),env,encoding:'utf8',maxBuffer:1<<30,windowsHide:true}),out=(r.stdout||'')+(r.stderr||'');
      fs.writeFileSync(log(n),out);if(r.status!==0)fail('build',n+' exit '+r.status);return out;};
    if(/\bGET https?:/.test(run(['restore','KingdomZombies.slnx','--locked-mode','--disable-parallel','-p:NuGetAudit=false','-v:n','-tl:off'],'restore.log')))fail('build','restore used the network');
    const out=run(['build','KingdomZombies.slnx','--no-restore','--disable-build-servers','-p:UseSharedCompilation=false','-nodeReuse:false','-p:NuGetAudit=false','-tl:off'],'build.log');
    const w=/(\d+) Warning\(s\)/.exec(out),e=/(\d+) Error\(s\)/.exec(out);if(!w||!e||w[1]!=='0'||e[1]!=='0')fail('build','warnings/errors '+(w&&w[1])+'/'+(e&&e[1]));
    return 'build restore locked, 0 Warning(s), 0 Error(s)';},
  layout(){const t=file(W+'tools/repository-layout.test.cjs').toString();if(!t.includes("'asset-pipeline'")||!t.includes("'phase4'")||!t.includes("'workspace-cleanup.cjs'"))fail('layout','test does not cover asset-pipeline, root phase4 and root workspace-cleanup');
    const r=cp.spawnSync(process.execPath,['--test','--test-reporter=tap',path.join(ROOT,W,'tools/repository-layout.test.cjs')],{cwd:ROOT,encoding:'utf8',maxBuffer:1<<30});
    const p=/^# pass (\d+)$/m.exec(r.stdout||''),f=/^# fail (\d+)$/m.exec(r.stdout||'');if(r.status!==0||!p||p[1]!=='6'||!f||f[1]!=='0')fail('layout',(r.stdout||'').slice(-800));return 'layout 6/6';},
  docs(){const r=cp.spawnSync(process.execPath,[path.join(ROOT,W,'tools/verification/check.cjs'),'--group','docs'],{cwd:ROOT,encoding:'utf8',maxBuffer:1<<30});
    fs.writeFileSync(path.join(ROOT,'.phase1/quick-261123-fsu/docs.log'),(r.stdout||'')+(r.stderr||''));
    if(r.status!==0||!/^PASS 36\/36 selected obligations/m.test(r.stdout||''))fail('docs','exit '+r.status+'; see .phase1/quick-261123-fsu/docs.log');return 'docs PASS 36/36';},
  state(){const s=file('.planning/STATE.md').toString(),bad=[];
    if(!/^current_phase: 05$/m.test(s))bad.push('phase');if(!/^\| # \| Description \| Date \| Commit \| Status \| Directory \|$/m.test(s))bad.push('table header');
    for(const id of['261114-we2','261116-k3u','261116-m0w','261119-15c','261120-09v','261121-jmu','261122-ub4'])if((s.match(new RegExp('^\\| '+id+' \\|','gm'))||[]).length!==1)bad.push('row '+id);
    if(/^\| 26112[23]-(uby|fsu) /m.test(s))bad.push('uby/fsu row present');if(!s.includes('261122-ub4-SUMMARY.md)'))bad.push('ub4 link');
    if(!/^- \[PR #6 relocation\].*runner\.prelaunch.*Phase 5/m.test(s))bad.push('renewal handoff bullet');
    if(bad.length)fail('state',bad.join(', '));return 'state ok';},
  records(){const s=file(Q+'261123-fsu-SUMMARY.md').toString(),bad=[];if(!/^---\n[\s\S]*?^status: complete$[\s\S]*?^---$/m.test(s))bad.push('status: complete');
    if(!/^## How to reproduce$/m.test(s))bad.push('How to reproduce');
    for(const f of['261123-fsu-PLAN.md','261123-fsu-SUMMARY.md'])file(Q+f).toString().split('\n').forEach((t,i)=>{if(HOST.test(t))bad.push('machine path '+f+':'+(i+1));});
    if(git(['log','--format=%h',PR+'..HEAD','--',Q,'.planning/quick/261122-uby-resolve-pr-6-merge-conflicts-and-check-d/']).trim())bad.push('task records committed');
    if(!s.includes(git(['rev-parse','--short=7','HEAD']).trim()))bad.push('tested revision missing');
    const dirty=git(['status','--porcelain']).split('\n').filter(l=>l&&!/^\?\? \.planning\/quick\/26112(2-uby|3-fsu)-/.test(l));if(dirty.length)bad.push('working tree: '+dirty.slice(0,5).join(' '));
    if(bad.length)fail('records',bad.join(', '));return 'records ok';}
};
const names=process.argv.slice(2);if(!names.length||names.some(n=>!G[n]))fail('usage','gates: '+Object.keys(G).join(' '));
for(const n of names)console.log('PASS '+G[n]());
~~~
<!-- gates.cjs end -->

## Session setup (Task 1 runs these first)

S1 preflight. Stop and report without merging if any line fails, because this plan's fixed hashes and counts are bound to these revisions.

```bash
test "$(git rev-parse --abbrev-ref HEAD)" = feature/quick-repository-layout
test "$(git rev-parse HEAD)" = c5be170a4df039ffb54a5a2f903c5c9b305a56cf
test -z "$(git status --porcelain | grep -vE '^\?\? \.planning/quick/(261122-uby|261123-fsu)-')"
git fetch origin
test "$(git rev-parse origin/dev)" = a305b8b58a6d17722a1ddb3ba4561849d54a2a49
test "$(git rev-parse origin/feature/quick-repository-layout)" = c5be170a4df039ffb54a5a2f903c5c9b305a56cf
test "$(node --version)" = v24.22.1
```

S2: seed the ignored caches by copying, never moving or linking, from the main checkout (research section 5). Set `MAIN_CHECKOUT` in this call only.

```bash
mkdir -p .phase1/feed
for f in "$MAIN_CHECKOUT"/.phase1/feed/*.nupkg; do test -e ".phase1/feed/${f##*/}" || cp "$f" .phase1/feed/; cmp "$f" ".phase1/feed/${f##*/}" || exit 1; done
for p in godot.sourcegenerators godotsharp godotsharpeditor; do test -e ".phase1/packages/$p/4.7.2" || { mkdir -p ".phase1/packages/$p" && cp -r "$MAIN_CHECKOUT/.phase1/packages/$p/4.7.2" ".phase1/packages/$p/"; }; done
test "$(ls .phase1/feed/*.nupkg | wc -l)" = 4
```

S3: extract the gate script and confirm it is byte-exact.

```bash
node -e "const fs=require('fs'),s=fs.readFileSync('.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-PLAN.md','utf8'),b=s.indexOf('<!-- gates.cjs '+'begin -->'),e=s.indexOf('<!-- gates.cjs '+'end -->'),k=s.slice(b,e),o=k.indexOf('\n',k.indexOf('~~~js'))+1;fs.mkdirSync('.phase1/quick-261123-fsu',{recursive:true});fs.writeFileSync('.phase1/quick-261123-fsu/gates.cjs',k.slice(o,k.lastIndexOf('~~~')))"
sha256sum .phase1/quick-261123-fsu/gates.cjs   # must print GATES_SHA256 (see Verification)
node .phase1/quick-261123-fsu/gates.cjs newhost
```

<tasks>

<task type="tracer">
  <name>Task 1: Merge commit with both-intent resolutions and all 187 moves, then prove the path map end to end</name>
  <precondition>S1 passes: the PR worktree is on feature/quick-repository-layout at c5be170, clean apart from the two untracked quick-task folders, and origin/dev resolves to a305b8b after git fetch.</precondition>
  <reversibility rating="reversible">The merge and fix commits stay local until the orchestrator pushes; nothing is published here.</reversibility>
  <files>.planning/STATE.md, docs/code/visibility-exemptions.json, docs/workflow/lan-foundation-checks.md, docs/workflow/verification.md, src/godot-game/tests/verification/ (conflicted files and git-placed dev tests), src/godot-game/tools/verification/check.cjs, src/godot-game/tools/gameplay-checks/, src/godot-game/tools/lan-checks/, src/godot-game/core/, src/godot-game/tools/workspace-cleanup.cjs, src/godot-game/tools/workspace-cleanup.test.cjs, src/godot-game/tools/asset-pipeline/ (moved, plus new layout.cjs), src/godot-game/production/assets/, src/godot-game/game/{assets,content,inspection,export_presets.cfg}, src/godot-game/tools/repository-layout.test.cjs</files>
  <action>
1. Run Session setup S1, S2 and S3. S3 must print the GATES_SHA256 value from Verification, and `newhost` must pass.
2. Run `git merge --no-ff --no-commit origin/dev`; it reports conflicts. Save `git diff --name-only --diff-filter=U` to `.phase1/quick-261123-fsu/conflicts.txt` and `git status --porcelain` to `.phase1/quick-261123-fsu/merge-status.txt`. This live index is the only authority for conflict resolution (#3786). Research section 1 is guidance and expects 81 unmerged paths: 16 content conflicts and 65 file-location conflicts. The location conflicts are 46 under `src/godot-game/tools/phase4/`, 14 under `src/godot-game/tests/verification/`, 3 under `src/godot-game/core/` and 2 under `src/godot-game/tools/`. Record any difference for the SUMMARY, then resolve every live path with the rules below anyway.
3. For each content-conflicted path, keep dev's behaviour, structure and new rows, and the PR's relocated paths. Follow research table 1a row by row. These choices must hold:
   - `gameplay-checks/check.cjs`: `inputManifest()` returns exactly `src`, `.editorconfig` and `docs/code`. Appending dev's caller extras triggers `INPUT_DUPLICATE`.
   - `verification/check.cjs`: dev's 22-name export list with the PR's root depth `../../../..`.
   - `BoundaryFixture.cs`: dev's binary Reference-element approach with HintPath `../game/.godot/mono/temp/bin/Debug/KingdomZombies.dll`.
   - `EngineAccessPolicy.cs`: dev logic, with inspection tuples at `src/godot-game/game/inspection/…`.
   - `ExemptionTests.cs`: dev's companion refactor, the companion filter `src/godot-game/game/`, and fixture `src/godot-game/game/proofs/Phase1.cs`.
   - `ExemptionValidator.cs`: generated prefix `src/godot-game/game/.godot/mono/temp/obj/Debug/generated/` and InspectionRoot under `src/godot-game/game/inspection/`.
   - `visibility-exemptions.json`, `ArchitecturePolicy.cs` and `FoundationPermissionTests.cs`: the union, with dev's 4 types at `src/godot-game/core/…` and `src/godot-game/game/inspection/InspectionRoot.cs`.
   - `lan-foundation-checks.md`: the PR's commands plus dev's last-row wording. `verification.md`: the PR's paths plus dev's "recorded".
   - Rendered and LAN tools: dev logic with the `src/godot-game/game` project and the `src/godot-game/tools/…` drivers.

   For `.planning/STATE.md`, do the merge resolution only. Start from dev's file. Convert its Quick Tasks table to the PR's header `| # | Description | Date | Commit | Status | Directory |`, using `—` as Status for dev rows. Add the PR-only rows 261116-k3u, 261116-m0w and 261121-jmu, and order all rows by ID. Keep dev's frontmatter, position, blockers and continuity. Drop the PR's superseded "Stopped at" and "Next action: Review PR #4" lines. Keep the PR's two 2026-11-16 continuation paragraphs at the end of Session Continuity; Task 3 condenses them. Stage each resolved path with `git add` on that path.
4. Stage every file-location conflict with `git add` on that path exactly as git placed it, with dev bytes. Never edit these files inside the merge.
5. Moves (research section 2; same map as the gate script's MAP): write `.phase1/quick-261123-fsu/move.cjs`. It runs `git mv` once per tracked file under `tools/phase4/`, `production/phase4/`, `src/godot-game/tools/phase4/`, `src/godot-game/{assets,content,inspection}/` and `src/godot-game/export_presets.cfg`, creating each parent folder first and never moving a directory onto another. Destinations: `src/godot-game/tools/asset-pipeline/`, `src/godot-game/production/assets/` and `src/godot-game/game/…`. Expect 187 moves. Afterwards `git ls-files tools production src/core src/multiplayer-abstractions tests src/godot-game/tools/phase4 src/godot-game/assets src/godot-game/content src/godot-game/inspection src/godot-game/export_presets.cfg` prints nothing.
6. With no unmerged entries left, commit the merge. Subject: `Merge origin/dev (Phase 4 and PR #7) into feature/quick-repository-layout`. Body: one line per content conflict naming its resolution, the move count, and the co-author trailer. Make no other content edits in the merge commit.
7. Fix commit `fix(quick-261123-fsu): resolve asset-pipeline paths through one layout map`, implementing CONTEXT "Phase 4 path identities" and "Checkout root":
   - Add `src/godot-game/tools/asset-pipeline/layout.cjs`. It exports `physical(rel)` and its inverse `logical(rel)`, both idempotent, with exactly the map in research "Code Examples" and 261122-uby research section 3.
   - Make `inside()` in `content.cjs` and `check.cjs` resolve through `physical()`. Make helpers that emit record paths, such as `relative()` and `normalizeCurrent()`, emit `logical()`. Record identities do not change.
   - Replace the hard-coded checkout root in `check.cjs` and `cases/hub.cjs` with `path.resolve(__dirname, …)` at the right depth. Fix the depths in `content.cjs`, `cases/export-profile.cjs` and `cases/runner.cjs`. Make `PROJECT_ROOT_PIN` compare against the same derived expression (research section 4).
   - Strengthen `src/godot-game/tools/repository-layout.test.cjs` and keep it at 6 tests. Add `'asset-pipeline'` to the purpose-folder list and the unrelated-cwd `--list` loop, and `'phase4'` to the retired root `tools/phaseN` loop. Add `'workspace-cleanup.cjs'` and `'workspace-cleanup.test.cjs'` to the root-file absence loop. Assert that `src/godot-game/tools/phase4` does not exist.
8. Run this task's verify. Repair failures with further fix commits and never amend the merge. If the strict build fails only because a Task 2 step A item is still stale, such as `GeneratedLayout.cs`, do that item now.
  </action>
  <verify>
    <automated>node .phase1/quick-261123-fsu/gates.cjs merge moves history newhost lists layout build</automated>
  </verify>
  <done>The merge commit (parents c5be170, a305b8b) is followed only by linear fix commits. No unmerged entry or conflict marker remains. All 206 dev-added files are byte-identical at their mapped paths and no retired root remains. History is unchanged and no new machine-path line exists. The four runners list 686, 16, 32 and 75 cases from an unrelated working directory. Layout tests pass 6/6 and the strict locked build has 0 warnings and 0 errors.</done>
</task>

<task type="auto">
  <name>Task 2: Rebind remaining live code, registers and every hash binding in cascade order</name>
  <files>src/godot-game/tests/verification/ (path literals, pins, new AssetPipelinePaths.cs), src/godot-game/tools/code-rules/GeneratedLayout.cs, src/godot-game/tools/verification/check.cjs, src/godot-game/tools/verification/required-cases.json, src/godot-game/tools/gameplay-checks/check.cjs, src/godot-game/tools/gameplay-checks/required-cases.json, src/godot-game/tools/asset-pipeline/ (peripheral literals, required-cases.json), src/godot-game/tools/workspace-cleanup.cjs, docs/code/engine-generated-spans.json, docs/code/generated-callsite-adaptations.json, docs/code/generated-layout-adaptations.json, src/godot-game/production/assets/export-profile.json, docs/content/README.md, docs/content/windows-inspection.md, src/godot-game/game/docs/README.md, src/godot-game/game/content/packs/delivery.json, src/godot-game/game/content/pack-index.json</files>
  <action>
Follow the order below; each later step hashes earlier outputs (research 3b). Afterwards, a C#, `.editorconfig` or register edit means redoing step D. An asset-pipeline module or indexed-doc edit means redoing the tail of step C.

A. Commit `fix(quick-261123-fsu): rebind live code to the relocated tree`, covering research 3a and 261122-uby research sections 2 and 4.
   - C# tests: classify every path literal in the 14 moved and the conflicted tests. Physical literals (file reads, analyzer keys, tuples) take the new path. Logical literals stay, such as those passed to `ContentPack.ResourcePath` or compared with pack records. Paths read from pack JSON and then opened go through one internal helper, `src/godot-game/tests/verification/AssetPipelinePaths.cs`, which mirrors layout.cjs. Move the `ContentNativePermissionTests` tuples in step with `EngineAccessPolicy`. Apply the prefix-collision fixes so that code which enumerates game sources points at `src/godot-game/game`. The dev files in `src/godot-game/core/` keep their bytes.
   - `GeneratedLayout.cs`: the 6 export suffixes become `/src/godot-game/{core,game/.godot/mono/temp,multiplayer-abstractions}/obj/…`, wrapped to 120 columns.
   - Runner literals: `verification/check.cjs` caller and catalog comparisons use `src/godot-game/tools/verification/required-cases.json` and `src/godot-game/tools/gameplay-checks/required-cases.json`.
   - Asset-pipeline peripheral items:
     - hub assembly path;
     - PowerShell archive literals, built from `runner.inside(ROOT,'.phase1/cache/'+file)` with quote escaping;
     - evidence.cjs `SOURCE_ROOTS`, `TARGETS` and media regex;
     - `reviewed-source.targets` depth;
     - a live extractor entry point that sets the root for the pinned extractors, per 261122-uby section 4;
     - inline Python paths;
     - the export `git ls-files` pathspec, and a fail-closed NuGet feed rewrite of `src/godot-game/NuGet.config`;
     - the export-layout origin comparison, made through the map;
     - the `scenarios.cjs` historical catalog path, chosen by revision;
     - the `export-profile.cjs` path to `projects.json`;
     - the `tools/phase4/cases/` prefix checks in `cases/text-data.cjs` `validateSupport` and `scenarios.cjs`, which become `src/godot-game/tools/asset-pipeline/cases/`.
   - Keep the bytes of the captured recipe strings, `extract-media.py`, `extract-audio.py`, ZIPs, provenance, DONOR and the `python.json` executable.
   - In `workspace-cleanup.cjs`, the USAGE text names `src/godot-game/tools/workspace-cleanup.cjs`.
   - Before committing, run `gates.cjs build lists newhost`.
B. Commit `fix(quick-261123-fsu): rebind register rows and row-baseline pins`.
   - In `engine-generated-spans.json` and `generated-callsite-adaptations.json`, rebind only the `.local[]` path and surface values to `src/godot-game/game/.godot/…`. `producerInventories` and `producerSources` are captured provenance; keep their bytes.
   - In `generated-layout-adaptations.json`, rebind the live `exportSources[]` path, suffix and owner. The captured `surface` keeps its bytes.
   - Rebind the `phase1-baseline-mtp` cohort `sourceInputsSha256`, per 261122-uby section 3.
   - Then run `gates.cjs build`. Derive each research 3a row-baseline pin whose hashed rows changed, by running its owning test method from the built test DLL with the isolation env (research "Pin readout technique") or by recomputing the hashed rows. Edit only those pins, then rebuild green. Record pins as "read out", never as test passes.
C. Commit `fix(quick-261123-fsu): regenerate asset catalog, delivery and pack index`.
   1. Set `projectGraphSha256` in `src/godot-game/production/assets/export-profile.json` and `cases/export-profile.cjs` to sha256 of `src/godot-game/tools/verification/projects.json`.
   2. Fix the live links and commands of the indexed and embedded docs now: `docs/content/README.md`, `docs/content/windows-inspection.md` and `src/godot-game/game/docs/README.md`. These are the pack-index docs plus the delivery guides. Never hand-edit `docs/content/fields.md`.
   3. With every asset-pipeline module final, set each catalog `module` that carries a `moduleSha256` to its physical `src/godot-game/tools/asset-pipeline/…` path and recompute the raw-byte sha256. Fixture `resources[].path` and `delivery.fixtureCatalog.sourcePath` stay logical.
   4. Regenerate `delivery.json`: the gzip level 9 fixture catalog, the 3 guides and any changed binding. First prove your packer reproduces dev's current payload from dev's bytes.
   5. Regenerate `pack-index.json` with `content.normalizeCurrent(ROOT)`, in the committed file's exact format.
   6. Regenerate fields.md with text-data `authorDocumentation` only to confirm it is byte-unchanged.
D. Commit `fix(quick-261123-fsu): regenerate ac-08 repository stages and caller bindings`.
   - Write `.phase1/quick-261123-fsu/ac08.cjs` implementing research 3c. Prove it first: from `a305b8b` trees, its output must equal dev's `repositoryStages` (sha256 `810bd84a…` = dev `callerStages.repositorySha256`).
   - Run it on the current tree and replace `repositoryStages`. Use dev's 42 keys with the map applied, re-sorted ordinal. `projects` are the 5 workspace csproj paths in C# `ProjectEvaluator.Expected` order.
   - Set `callerStages.repositorySha256` for the 5 cases here and for gameplay `core.permissions`, and rebind the `callerStages.catalog` strings.
   - Recompute `callerAuthority.sha256` and every `producerStages.registries.*.sha256`. Rebind the catalog's other retired path literals consistently with step A.
   - Rerun ac08.cjs on the committed tree and require byte equality.
   - The `bindings` gate does not trust ac08.cjs. It first checks that its own row rule reproduces dev's ac-08 block at `a305b8b` exactly. It then re-derives every HEAD row: the path set, the kind by extension, the owners from project folders plus the test csproj's linked `Compile` items in `projects` order, and the sha256 of the HEAD blob, which must also equal the working-tree bytes the runner hashes. A row that is wrong but has a recomputed `sourceSetSha256` therefore still fails. Commit before running it.
E. Run this task's verify.
  </action>
  <verify>
    <automated>node .phase1/quick-261123-fsu/gates.cjs merge moves history newhost lists layout build bindings</automated>
  </verify>
  <done>Every gate prints PASS. The 423 module bindings and projectGraphSha256 are current. Every pack-index row hashes to its physical file, with IDs, paths and outputs equal to dev. content.validate reports 180 and validateSupport passes. Every ac-08 row matches HEAD: the path set by the SourceFileFormat rule, the kind, the owners, and a sha256 equal to both the HEAD blob and the working-tree bytes. The same rule reproduces dev's own block, and the 42 keys and 5 projects are correct. Caller, producer and authority hashes match and the 9 staged plans are accepted. ZIPs, pinned extractors and fields.md equal dev. The build is still 0/0.</done>
</task>

<task type="auto">
  <name>Task 3: Fix remaining live docs, finish STATE.md, run the final checks and write the SUMMARY</name>
  <files>docs/architecture/repository-layout.md, docs/workflow/cleanup.md, docs/workflow/engine-defects.md, docs/workflow/setup.md, docs/content/production.md, src/godot-game/tools/README.md, .planning/STATE.md, .planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-SUMMARY.md (written, never committed)</files>
  <action>
1. Commit `docs(quick-261123-fsu): point live docs at the relocated tree`. These are path-only fixes from the research 3a live-docs row; out-of-scope sweeps stay out (CONTEXT Scope).
   - `engine-defects.md`: links and the command.
   - `setup.md`: the link.
   - `cleanup.md`: commands only. Use `node src/godot-game/tools/workspace-cleanup.cjs …` and the gameplay-checks path. Move no rules.
   - `production.md`: links and commands.
   - `src/godot-game/tools/README.md`: the asset-pipeline sentence moves to the present tense.
   - `repository-layout.md`:
     - Drop "after integration" from the `tools/phase4/` and `production/phase4/` rows.
     - Add `tools/workspace-cleanup.cjs` and `.test.cjs` to the same-names row.
     - State the rule: Phase 4 content and pack records keep `src/godot-game/…`, `production/phase4/…` and `tools/phase4/…` as stable logical identities, `src/godot-game/tools/asset-pipeline/layout.cjs` resolves them, and runner bindings use physical paths.
     - Make the integration-guide sentence past tense.
   - Before editing a doc, confirm it is not a pack-index file or a delivery guide. If it is, do the edit in Task 2 step C and redo that tail.
2. Commit `docs(quick-261123-fsu): record the dev integration in STATE`, following research section 6:
   - Set `last_updated`, `last_activity` and `last_activity_desc`, and the "Last activity" line.
   - Change dev's ub4 link to `./quick/261122-ub4-prepare-for-merge-pr-7/261122-ub4-SUMMARY.md`.
   - Add this exact Blockers/Concerns bullet: "- [PR #6 relocation] After PR #6 integrated dev, the Phase 4 review-bound gates `runner.prelaunch`, `runner.selection`, `acceptance.judgments`, `acceptance.artifacts` and `--verify-completion` are expected-blocked against the relocated tree. Phase 4 records keep their bytes; renewing these gates is owned by Phase 5."
   - Add a matching clause to Next action.
   - Condense the PR's two 2026-11-16 paragraphs into one sentence saying PR #6 integrated dev (Phase 4 and PR #7).
   - Add no 261123-fsu row and no 261122-uby row. The orchestrator adds the fsu row.
3. Final checks at the final commit: run this task's verify without the trailing `records` gate first.
   - If `docs` fails, read `.phase1/quick-261123-fsu/docs.log` and the runner summary it links. Repair with a fix commit, following the cascade rules, and rerun the whole set.
   - The `docs` gate can take several minutes; run it in the background if a call would exceed the shell timeout.
   - Make no commit after the passing run. That HEAD is the tested revision.
4. Write the SUMMARY using the summary template, and do not stage it. It contains:
   - frontmatter `status: complete`;
   - the tested revision as a full SHA;
   - a per-path resolution table built from `conflicts.txt`;
   - a results table (command | tested revision | exit status | counts) for each gate, including build, layout, docs (36/36 and the report summary path it printed) and bindings;
   - "Rebound, statically consistent, not executed": ac-08, the producer and caller stages, every asset-pipeline case, export, the extractor entry point, C# behaviour of the rebound literals, and the pins;
   - "Not run": Windows export, editor-import self-test, headless launch, game windows and the 776-case selection;
   - "Expected-blocked": `runner.prelaunch`, `runner.selection`, `acceptance.*` and `--verify-completion`, with renewal handed to Phase 5;
   - "Blocked by PR #7 cleanup": `export.layout`, `export.profile`, `rules.export-layout-origin` and `rules.export-layout-boundary`, and the `ContentNativePermissionTests` model tests;
   - deviations from the research;
   - task-created ignored paths for the orchestrator's end-of-task cleanup: `.phase1/quick-261123-fsu/`, this task's `.phase1/runs/` reports, `.phase1/temp/`, `bin`, `obj` and `.godot`, plus the seeded `.phase1/feed/` and the 3 Godot package folders, which cleanup.md keeps. Note that the pinned node folder predates the task and is needed for the orchestrator's final docs run;
   - `## How to reproduce`, with commands from the checkout root: seed caches, pinned node, setup isolation, the strict build from verification.md, layout tests, the docs group, and the gate script from this plan.

   Use symbolic locations only.
5. Run the full verify, including `records`.
  </action>
  <verify>
    <automated>node .phase1/quick-261123-fsu/gates.cjs merge moves history newhost lists layout build bindings state docs records</automated>
  </verify>
  <done>Every gate prints PASS at the final commit: strict build 0/0, layout 6/6, docs 36/36, bindings, state and records. STATE.md holds dev's Phase 5 position, the 7 rows, the fixed ub4 link and the renewal bullet, with no fsu row. The SUMMARY exists uncommitted, with status complete, the results table, the blocked and not-run rows and How to reproduce. The working tree is clean apart from the two untracked quick-task folders.</done>
</task>

</tasks>

<threat_model>
## Trust Boundaries

| Boundary | Description |
|----------|-------------|
| origin/dev to PR branch | Incoming Phase 4 and PR #7 bytes, some carrying old paths and machine-specific roots |
| Executor edits to committed tree | Hash-bound validators, pins and records that a shortcut could weaken |
| Untracked inputs to commits | 261122-uby files hold machine paths and must never be committed |

## STRIDE Threat Register

| Threat ID | Category | Component | Severity | Disposition | Mitigation Plan |
|-----------|----------|-----------|----------|-------------|-----------------|
| T-fsu-01 | Tampering | validators, pins, expected counts | high | mitigate | Rebind values only. The gates fix the counts (206 moves, 686/16/32/75 lists, 423 modules, 42 keys, 9 staged plans, 180 outputs, 6 layout tests, 36 docs) and re-derive every ac-08 row (path, kind, owners and sha256 at HEAD and on disk) from SourceFileFormat.cs, the test csproj and the HEAD blobs, after proving that rule on dev's own block. |
| T-fsu-02 | Information disclosure | committed lines | high | mitigate | The `newhost` gate rejects any machine-path line absent from both parents. The `records` gate scans the PLAN and SUMMARY. Locations are named symbolically. |
| T-fsu-03 | Tampering | Phase 1-4 records, quick records, provenance | medium | mitigate | The `history` gate requires each record to equal its source side. |
| T-fsu-04 | Information disclosure | 261122-uby and 261123-fsu folders | medium | mitigate | Explicit pathspec staging only. The `records` gate fails if any commit after the PR head touches either folder. |
| T-fsu-05 | Tampering | ZIP blobs, pinned extractors, pack IDs | high | mitigate | The `bindings` gate compares them with dev. A logical-identity map is used instead of rewriting record paths. |
| T-fsu-06 | Repudiation | branch history | medium | mitigate | The `merge` gate requires one merge commit then linear commits. No rebase, stash, amend of the merge, force or push. |
| T-fsu-SC | Tampering | NuGet restore | low | accept | No npm, pip or cargo installs. NuGet restore is locked-mode from byte-copied verified caches, and the `build` gate fails on any HTTP GET. |
</threat_model>

<verification>
- GATES_SHA256 for `.phase1/quick-261123-fsu/gates.cjs` as extracted by S3: `daa62c2f34409450785496ee65ce8eb0c1ab1adbe257ef837fa60ca1f63e0892`.
- Final: `node .phase1/quick-261123-fsu/gates.cjs merge moves history newhost lists layout build bindings state docs records` prints PASS for every gate at the executor's final commit.
- Not verified here, and reported as such: export, editor-import self-test, headless launch, game windows, the 776-case selection and any executed Phase 4 or ac-08 case. The orchestrator re-runs `gates.cjs docs` after adding the fsu STATE row.
</verification>

<success_criteria>
- One merge commit plus linear fix commits on feature/quick-repository-layout. Nothing is pushed.
- All 206 dev-added files are final at their current locations: 187 explicit moves plus 19 git placements (3 Core, 14 tests, 2 PR #7 tools). Every conflict keeps both intents.
- The bindings are regenerated and statically consistent. The strict build is 0/0, layout tests pass 6/6 and the docs group passes 36/36 at the final commit.
- STATE.md is merged without the fsu row. The SUMMARY is written, uncommitted, with How to reproduce.
</success_criteria>

<output>
Write `.planning/quick/261123-fsu-resolve-conflict-https-github-com-sebrei/261123-fsu-SUMMARY.md` and leave it uncommitted. The orchestrator adds the STATE row, commits the quick-task artifacts, re-runs the docs gate and pushes.
</output>
