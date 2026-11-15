'use strict';
const fs=require('node:fs'),path=require('node:path'),e=require('./evidence.cjs'),p=require('./processes.cjs');
async function run(context,selection){
  const references={phase1:[],phase2:[]},phase1=e.read(path.join(context.root,'tools/phase1/required-cases.json'));
  const covered=new Set(),commands=[];
  for(const group of ['engine','graph','state','runner']){
    const ids=phase1.cases.filter(row=>row.group===group).map(row=>row.id);
    if(ids.every(id=>selection.selected.phase1.includes(id))){commands.push({family:'phase1',args:['--group',group]});for(const id of ids)covered.add(id);}
  }
  for(const id of selection.selected.phase1.filter(id=>!covered.has(id)))commands.push({family:'phase1',args:['--case',id]});
  commands.push({family:'phase2',args:['--all']});
  for(let index=0;index<commands.length;index++){
    const command=commands[index];console.log('Inherited '+command.family+' '+command.args.join(' '));
    const startedAt=Date.now();
    const record=await p.run(process.execPath,[path.join(context.root,'tools',command.family,'check.cjs'),...command.args],
      {cwd:context.root,env:context.env,onOutput:bytes=>process.stdout.write(bytes)},900000);
    const finishedAt=Date.now(),matches=[...record.stdout.matchAll(/(\.phase[12][\\/]runs[\\/][^\s;)\]]+[\\/]summary\.json)/g)];
    e.requireTrue(matches.length>0,'INHERITED_REPORT_NOT_FOUND');const relative=matches.at(-1)[1].replaceAll('\\','/');
    const reportFile=e.inside(context.root,relative),summary=e.read(reportFile);e.requireTrue(summary.status==='pass','INHERITED_FAILED');
    const processFile='inherited-'+index+'.process.json';fs.writeFileSync(path.join(context.directory,processFile),JSON.stringify({...record,startedAt,finishedAt},null,2),{flag:'wx'});
    references[command.family].push({path:relative,sha256:e.hash(fs.readFileSync(reportFile)),startedAt,finishedAt,
      process:{path:path.relative(context.root,path.join(context.directory,processFile)),sha256:e.hash(fs.readFileSync(path.join(context.directory,processFile)))}});
  }return references;
}
module.exports={run};
