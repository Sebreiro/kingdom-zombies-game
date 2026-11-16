'use strict';
const fs=require('node:fs'),path=require('node:path'),e=require('./evidence.cjs'),p=require('./processes.cjs');
const integrity=['engine.source-policy','engine.generated-origin','engine.generated-boundary','rules.godot-callsite-boundary',
  'graph.visibility-inventory','graph.allowed-composition','rules.source-format'];
function plan(root,selection){
  const phase1=e.read(path.join(root,'src/godot-game/tools/verification/required-cases.json'));
  const covered=new Set(),commands=[];
  for(const group of ['engine','graph','state','runner']){
    const ids=phase1.cases.filter(row=>row.group===group).map(row=>row.id);
    if(ids.length&&ids.every(id=>selection.selected.phase1.includes(id))){commands.push({family:'phase1',args:['--group',group],selected:ids,dependsOn:[]});for(const id of ids)covered.add(id);}
  }
  for(const id of selection.selected.phase1.filter(id=>!covered.has(id)))commands.push({family:'phase1',args:['--case',id],selected:[id],dependsOn:[]});
  const phase2=e.read(path.join(root,'src/godot-game/tools/gameplay-checks/required-cases.json'));
  const prerequisites=integrity.filter(id=>selection.selected.phase1.includes(id)).map(id=>'phase1/'+id);
  for(const group of ['core','runner','rendered']){
    const selected=phase2.cases.filter(row=>row.group===group&&selection.selected.phase2.includes(row.id)).map(row=>row.id);
    if(selected.length)commands.push({family:'phase2',args:['--group',group],selected,dependsOn:group==='rendered'?
      [...prerequisites,...phase2.cases.filter(row=>row.group==='core'&&selection.selected.phase2.includes(row.id)).map(row=>'phase2/'+row.id)]:[]});
  }
  return {commands,nativeDependsOn:[...prerequisites,...phase2.cases.filter(row=>row.group==='core'&&selection.selected.phase2.includes(row.id)).map(row=>'phase2/'+row.id)]};
}
async function execute(context,command,index){
  const startedAt=Date.now();let record,error;
  try{
    const script={phase1:'src/godot-game/tools/verification/check.cjs',phase2:'src/godot-game/tools/gameplay-checks/check.cjs'}[command.family];
    e.requireTrue(typeof script==='string','INHERITED_FAMILY_UNKNOWN');
    record=await p.run(process.execPath,[path.join(context.root,script),...command.args],
    {cwd:context.root,env:context.env,onOutput:bytes=>process.stdout.write(bytes)},900000);}
  catch(failure){error=failure;record=failure.record??{executable:process.execPath,args:command.args,code:null,stdout:'',stderr:'',error:failure.stack};}
  const finishedAt=Date.now(),prefix='inherited-'+index;
  const save=(name,value)=>{const file=path.join(context.directory,prefix+name);fs.writeFileSync(file,value,{flag:'wx'});return {path:path.relative(context.root,file).replaceAll('\\','/'),sha256:e.hash(fs.readFileSync(file))};};
  const processReference=save('.process.json',JSON.stringify({...record,startedAt,finishedAt,error:error?.stack},null,2));
  const logs={stdout:save('.stdout.log',record.stdout??''),stderr:save('.stderr.log',record.stderr??'')};
  const outcome={...command,executed:true,startedAt,finishedAt,process:processReference,logs,status:'blocked',reason:error?.message,caseOutcomes:command.selected.map(id=>({id,status:'blocked',message:'INHERITED_REPORT_UNAVAILABLE'}))};
  try{
    const matches=[...(record.stdout??'').matchAll(/(\.phase[12][\\/]runs[\\/][^\s;)\]]+[\\/]summary\.json)/g)];
    e.requireTrue(matches.length>0,'INHERITED_REPORT_NOT_FOUND');const relative=matches.at(-1)[1].replaceAll('\\','/');
    const reportFile=e.inside(context.root,relative),summary=e.read(reportFile);
    outcome.report={path:relative,sha256:e.hash(fs.readFileSync(reportFile)),process:processReference,startedAt,finishedAt};
    e.exact(command.selected,summary.selected,'inherited command selection');
    outcome.caseOutcomes=command.selected.map(id=>{
      const result=summary.results?.find(row=>row.id===id)??summary.failures?.find(row=>row.id===id);
      return {id,status:['pass','fail','blocked'].includes(result?.status)?result.status:'blocked',message:result?.message??result?.diagnostic};
    });
    const abnormal=record.timedOut||record.signal||record.cleanupError||!Number.isInteger(record.code);
    outcome.status=!error&&record.code===0&&summary.status==='pass'&&outcome.caseOutcomes.every(row=>row.status==='pass')?'pass':
      abnormal||outcome.caseOutcomes.some(row=>row.status==='fail')?'fail':'blocked';
    if(outcome.status!=='pass')outcome.reason=error?.message??'INHERITED_'+summary.status;
    if(abnormal||outcome.status!=='pass'&&outcome.caseOutcomes.every(row=>row.status==='pass')){
      outcome.status='fail';for(const row of outcome.caseOutcomes){row.status='fail';row.message='INHERITED_PROCESS_OR_AGGREGATE_FAILED';}
    }
  }catch(failure){outcome.reason=failure.message;}
  outcome.record=save('.outcome.json',JSON.stringify(outcome,null,2));return outcome;
}
async function run(context,selection){
  const references={phase1:[],phase2:[],commands:[],status:'pass'},outcomes=new Map();
  const {commands,nativeDependsOn}=plan(context.root,selection);
  for(let index=0;index<commands.length;index++){
    const command=commands[index];console.log('Inherited '+command.family+' '+command.args.join(' '));
    const blockers=command.dependsOn.filter(id=>outcomes.get(id)!=='pass');
    const outcome=blockers.length?{...command,executed:false,status:'blocked',reason:'PREREQUISITE_FAILED '+blockers.join(','),
      caseOutcomes:command.selected.map(id=>({id,status:'blocked'}))}:await execute(context,command,index);
    references.commands.push(outcome);if(outcome.report)references[command.family].push(outcome.report);
    for(const row of outcome.caseOutcomes)outcomes.set(command.family+'/'+row.id,row.status);
    if(outcome.status!=='pass')references.status='fail';
  }
  references.nativeBlockers=nativeDependsOn.filter(id=>outcomes.get(id)!=='pass');
  references.nativeAllowed=references.nativeBlockers.length===0;return references;
}
module.exports={run,plan};
