'use strict';
const fs=require('node:fs'),path=require('node:path'),assert=require('node:assert/strict');
const {TextDecoder,isDeepStrictEqual:equal}=require('node:util');
const {spawnSync}=require('node:child_process');
const runner=require('../check.cjs'),content=require('../content.cjs');
const {ROOT,hash,write,inside,need}=content;
const BASE='src/godot-game/content/',SOURCE_REVISION='0c6e9590c66ad2f3fe975bb36ce09d24ea72adca';
const FOUNDATION=['run','time','survivors'];
function fault(file,field,value,reason){
 const diagnostic={file,field,value:Number.isFinite(value)||typeof value!=='number'?value:String(value),reason};
 const error=new Error('DATA_INVALID '+JSON.stringify(diagnostic));error.diagnostic=diagnostic;throw error;
}
function check(ok,file,field,value,reason){if(!ok)fault(file,field,value,reason);}
function strictParse(raw,file){
 check(Buffer.isBuffer(raw)&&raw.length>0&&raw.length<=1048576,file,'',raw?.length,'SIZE');
 let text;try{text=new TextDecoder('utf-8',{fatal:true,ignoreBOM:true}).decode(raw);}catch{fault(file,'','invalid UTF-8','ENCODING');}
 check(!text.startsWith('\ufeff'),file,'','BOM','ENCODING');let at=0,count=0;
 const ws=()=>{while(/\s/.test(text[at]??'')&&at<text.length)at++;};
 function string(p){const match=/^"(?:[^"\\\u0000-\u001f]|\\(?:["\\/bfnrt]|u[0-9a-fA-F]{4}))*"/.exec(text.slice(at));check(match,file,p,text.slice(at,at+64),'JSON_STRING');at+=match[0].length;
  const value=JSON.parse(match[0]);check(value===value.normalize('NFC')&&!/[\ud800-\udfff]/u.test(value),file,p,value,'ENCODING');return value;}
 function value(p,depth){ws();check(++count<=100000&&depth<=16,file,p,count,'LIMIT');const ch=text[at];
  if(ch==='"')return string(p);
  if(ch==='{'||ch==='['){const array=ch==='[',result=array?[]:{},end=array?']':'}',keys=new Set();at++;ws();if(text[at]===end){at++;return result;}
   while(true){ws();let key=array?String(result.length):string(p);check(!keys.has(key),file,p+'/'+key,key,'DUPLICATE_KEY');keys.add(key);
    check(!['__proto__','prototype','constructor'].includes(key),file,p+'/'+key,key,'KEY');if(!array){ws();check(text[at++]===':',file,p+'/'+key,'','JSON_COLON');}
    result[key]=value(p+'/'+key,depth+1);ws();if(text[at]===end){at++;return result;}check(text[at++ ]===',',file,p,'','JSON_SEPARATOR');}
  }
  for(const [literal,result]of [['true',true],['false',false],['null',null]])if(text.startsWith(literal,at)){at+=literal.length;return result;}
  const match=/^-?(?:0|[1-9]\d*)(?:\.\d+)?(?:[eE][+-]?\d+)?/.exec(text.slice(at));check(match,file,p,text.slice(at,at+64),'JSON_VALUE');at+=match[0].length;const result=Number(match[0]);check(Number.isFinite(result),file,p,match[0],'FINITE');return result;
 }
 const result=value('',0);ws();check(at===text.length,file,'',text.slice(at,at+64),'JSON_TRAILING');return result;
}
function read(file){const name=inside(ROOT,file),stat=fs.statSync(name);check(stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=1048576,file,'',stat.size,'SIZE');return strictParse(content.bytes(ROOT,file),file);}
function source(){const child=spawnSync('git',['show',SOURCE_REVISION+':specs/0.1/initial-spec.md'],{cwd:ROOT,encoding:'utf8',timeout:10000,maxBuffer:1048576,windowsHide:true});
 need(!child.error&&child.status===0,'SOURCE_READ',child.error?.message??child.stderr);return child.stdout;}
function section(text,id){const start=text.indexOf('### '+id+' ');need(start>=0,'SOURCE_SECTION',id);return text.slice(start).split(/\n(?=###? )/)[0];}
function row(text,id,label){const found=section(text,id).split('\n').filter(x=>x.startsWith('| '+label+' |'));need(found.length===1,'SOURCE_ROW',id+' '+label);return found[0].split('|').slice(1,-1).map(x=>x.trim());}
function numbers(text){return [...text.replaceAll(',','').matchAll(/\d+(?:\.\d+)?/g)].map(x=>Number(x[0]));}
function capture(text,pattern){const match=pattern.exec(text);need(match,'SOURCE_PARAGRAPH',String(pattern));return Number(match[1]);}
function roster(text){const table=section(text,'5.3').split('\n').filter(x=>/^\| [^|]+ \| \d+×\d+ \|/.test(x));need(table.length===24,'SOURCE_ITEM_ROSTER');return table.map((line,i)=>{const c=line.split('|').slice(1,-1).map(x=>x.trim());return {id:'IT-'+String(i+1).padStart(2,'0'),name:c[0],footprint:numbers(c[1]),maxStack:Number(c[2]),weightKilograms:numbers(c[3])[0],purpose:c[4]};});}
function referenceSets(){const packs=['items-01-10','items-11-24'].map(x=>read(BASE+'packs/'+x+'.json'));
 const bodies=read(BASE+'packs/bodies-a.json');const obligations=read('tools/phase4/expected-obligations.json').obligations;
 return {sourceItems:roster(source()),items:new Map(packs.flatMap(p=>p.items.map(x=>[x.id,{...x,pack:p}]))),bodies:new Set(bodies.bodies.map(x=>x.id)),obligations:new Set(obligations.map(x=>x.id))};}
function describeFields(schema){return schema.files.flatMap(file=>file.fields.map(field=>({file:file.path,...field})));}
function validateDocument(file,data,definition,refs){
 const fields=new Map();check(Array.isArray(definition.fields)&&definition.fields.length<=5000,file,'schema',definition.fields?.length,'SCHEMA');
 for(const field of definition.fields){check(!fields.has(field.path),file,field.path,field.path,'DUPLICATE_FIELD');fields.set(field.path,field);
  for(const key of ['id','path','type','meaning','unit','default','allowed','dependencies','source','referenceKind'])check(Object.hasOwn(field,key),file,field.path,key,'FIELD_METADATA');
  check(typeof field.meaning==='string'&&field.meaning.length>10&&typeof field.unit==='string'&&field.unit.length>0&&Array.isArray(field.dependencies)&&field.dependencies.length>0,file,field.path,field,'FIELD_METADATA');
  check(field.source.document==='specs/0.1/initial-spec.md'&&/^\d+\.\d+$/.test(field.source.section)&&typeof field.source.evidence==='string'&&field.source.evidence.length>0,file,field.path,field.source,'FIELD_SOURCE');
 }
 const seen=new Set();let count=0;
 function walk(v,p,depth){check(++count<=10000&&depth<=16,file,p,count,'LIMIT');const f=fields.get(p);check(f,file,p,v,'UNKNOWN_FIELD');seen.add(p);const type=v===null?'null':Array.isArray(v)?'array':typeof v;
  check(type===f.type,file,p,v,'TYPE');const a=f.allowed;
  if(type==='number'){check(Number.isFinite(v),file,p,v,'FINITE');check(!a.integer||Number.isSafeInteger(v),file,p,v,'INTEGER');check(v>=a.minimum&&v<=a.maximum,file,p,v,'BOUNDS');}
  if(type==='string')check(v.length>0&&v.length<=4096&&v===v.normalize('NFC')&&!/[\u0000-\u001f\ud800-\udfff]/u.test(v),file,p,v,'ENCODING');
  if(a.choices)check(a.choices.some(x=>equal(x,v)),file,p,v,'CHOICE');
  if(type==='array')check(v.length>=a.minItems&&v.length<=a.maxItems,file,p,v.length,'COUNT');
  if(f.referenceKind==='item')check(refs.items.has(v),file,p,v,'ITEM_REFERENCE');
  else if(f.referenceKind==='body')check(refs.bodies.has(v),file,p,v,'BODY_REFERENCE');
  else if(f.referenceKind==='content-obligation'||f.referenceKind==='sound-obligation')check(refs.obligations.has(v),file,p,v,'CONTENT_REFERENCE');
  else if(f.referenceKind==='resource'){try{read('src/godot-game/'+v);}catch{fault(file,p,v,'RESOURCE_REFERENCE');}}
  else check(f.referenceKind==='none'||['image','text','appearance'].includes(f.referenceKind),file,p,f.referenceKind,'REFERENCE_KIND');
  if(v&&typeof v==='object')for(const [key,child]of Object.entries(v))walk(child,p+'/'+key,depth+1);
 }walk(data,'',0);
 for(const p of fields.keys())check(seen.has(p),file,p,'<missing>','MISSING_FIELD');return seen.size;
}
function validateFoundation(docs,refs){const {run:r,time:t,survivors:s}=docs;
 const fail=(domain,p,v,why)=>check(false,BASE+'config/'+domain+'.json',p,v,why);
 if(r.playerLimit>4||r.startingSurvivors<r.playerLimit||r.startingSurvivors>r.recruitedSurvivorCap)fail('run','/startingSurvivors',r.startingSurvivors,'POPULATION_RELATIONSHIP');
 const ids=new Set();for(const [i,x]of r.initialStash.entries()){if(ids.has(x.item))fail('run','/initialStash/'+i+'/item',x.item,'DUPLICATE_ID');ids.add(x.item);const it=refs.items.get(x.item);
  const max=refs.sourceItems.find(y=>y.id===x.item)?.maxStack;
  if(x.quantity!==x.stackQuantity*x.stacks||x.stackQuantity>max)fail('run','/initialStash/'+i+'/stackQuantity',x.stackQuantity,'STACK_RELATIONSHIP');
  if(x.condition==='full'&&!['IT-03','IT-04','IT-05','IT-06','IT-07','IT-18','IT-19','IT-20'].includes(it.id))fail('run','/initialStash/'+i+'/condition',x.condition,'CONDITION_RELATIONSHIP');
 }
 if(t.firstScheduledNight!==t.openingWaveFreeDays+1||t.stageOriginDay!==r.initialDay)fail('time','/firstScheduledNight',t.firstScheduledNight,'TIME_RELATIONSHIP');
 if(s.sprintSpeedUnitsPerSecond<s.walkSpeedUnitsPerSecond)fail('survivors','/sprintSpeedUnitsPerSecond',s.sprintSpeedUnitsPerSecond,'SPEED_RELATIONSHIP');
 if(s.load.softKilograms>=s.load.severeKilograms||s.load.minimumSpeedMultiplier>1||s.load.maximumActionCostMultiplier<1)fail('survivors','/load',s.load,'LOAD_RELATIONSHIP');
 check(equal(r.startingOutfit,s.appearance.outfitRefs),BASE+'config/survivors.json','/appearance/outfitRefs',s.appearance.outfitRefs,'OUTFIT_RELATIONSHIP');
 const slots=r.startingOutfit.map(x=>refs.items.get(x)?.slot);check(new Set(slots).size===slots.length&&slots.every(Boolean),BASE+'config/run.json','/startingOutfit',r.startingOutfit,'LOADOUT_RELATIONSHIP');
 return {initialStacks:r.initialStash.reduce((n,x)=>n+x.stacks,0),stageDays:[1,4,7,10].map(day=>({day,stage:t.stageOriginIndex+Math.floor((day-t.stageOriginDay)/t.escalationIntervalDays)})),loadEndpoints:[s.load.softKilograms,s.load.severeKilograms]};
}
function foundationSource(docs,text){const {run:r,time:t,survivors:s}=docs,observations=[];
 function exact(domain,p,value,expected,source){check(equal(value,expected),BASE+'config/'+domain+'.json',p,value,'SOURCE_BASELINE');observations.push({domain,field:p,value,expected,source});}
 const eq=(domain,key,expected,source)=>exact(domain,'/'+key,docs[domain][key],expected,source);
 const starting=section(text,'2.1'),words=['zero','one','two','three','four','five','six','seven','eight','nine','ten'],match=/defaults:\*\* (\w+) recruited/.exec(starting);need(match&&words.includes(match[1]),'SOURCE_START_COUNT');eq('run','startingSurvivors',words.indexOf(match[1]),'2.1 confirmed defaults');
 eq('run','initialDay',capture(starting,/dawn on day (\d+)/),'2.1');eq('run','initialMainTier',capture(starting,/tier-(\d+) main/),'2.1');
 eq('run','recruitedSurvivorCap',numbers(row(text,'6.4','Total recruited-survivor cap')[1])[0],'6.4');eq('run','livingWildlifeCap',numbers(row(text,'6.3','Total living wildlife cap')[1])[0],'6.3');
 eq('run','activeZombieCap',capture(text,/proposed active-zombie cap is (\d+)/),'9.6');
 const items=roster(text),stash=starting.split('\n').filter(x=>/^\| [^|]+ \| \d+ \|/.test(x)).map(line=>{const cells=line.split('|').slice(1,-1).map(x=>x.trim()),it=items.find(x=>x.name===cells[0]);need(it,'SOURCE_STASH_ITEM',cells[0]);const quantity=Number(cells[1]),stackQuantity=Math.min(quantity,it.maxStack);return {item:it.id,quantity,stackQuantity,stacks:quantity/stackQuantity,condition:cells[2].includes('Full durability')?'full':'not-applicable',contents:'empty'};});exact('run','/initialStash',r.initialStash,stash,'2.1 initial stash table plus5.3 legal stack maxima');
 exact('run','/startingOutfit',r.startingOutfit,['Basic shirt','Trousers','Boots'].map(name=>items.find(x=>x.name===name).id),'2.1 starter outfit paragraph;5.3 IDs');
 for(const [key,label]of [['daylightSeconds','Daylight'],['nightSeconds','Night'],['openingWaveFreeDays','Opening scheduled-wave-free days'],['escalationIntervalDays','Escalation interval']])eq('time',key,numbers(row(text,'3.2',label)[1])[0],'3.2 '+label);
 const rules=[['maximumHealth','Survivor maximum health / stamina',0],['maximumStamina','Survivor maximum health / stamina',1],['walkSpeedUnitsPerSecond','Walk / sprint speed',0],['sprintSpeedUnitsPerSecond','Walk / sprint speed',1],['jumpHeightUnits','Jump',0],['jumpAirborneSeconds','Jump',1],['airWalkControlMultiplier','Air movement',0,100],['sprintStaminaPerSecond','Sprint stamina cost',0],['jumpStaminaCost','Jump stamina cost',0],['staminaRegenerationPerSecond','Natural stamina regeneration',0],['staminaRegenerationDelaySeconds','Natural stamina regeneration',1],['safeHealthRegenerationPerSecond','Safe-camp health regeneration',0],['safeRestDelaySeconds','Safe-camp health regeneration',1],['safeEnemyDistanceUnits','Safe-rest conditions',0],['foodCooldownSeconds','Food-use cooldown',0]];
 for(const [key,label,index,divisor=1]of rules)eq('survivors',key,numbers(row(text,'4.1',label)[1])[index]/divisor,'4.1 '+label);
 const thresholds=numbers(row(text,'4.1','Soft / severe carried-load thresholds')[1]),effects=numbers(row(text,'4.1','Load effect')[1]);
 for(const [key,value]of [['softKilograms',thresholds[0]],['severeKilograms',thresholds[1]],['minimumSpeedMultiplier',effects[0]/100],['maximumActionCostMultiplier',effects[1]/100]])exact('survivors','/load/'+key,s.load[key],value,'4.1 load table');
 eq('survivors','refugeDistanceUnits',capture(section(text,'3.3'),/refuge extends (\d+) u/),'3.3');
 eq('run','livingPortals',words.indexOf(/and (\w+) living portals/.exec(starting)?.[1]),'2.1 living portal count');
 eq('run','playerLimit',capture(text,/Solo or 1–(\d+) cooperative/),'1 supported player scope');
 eq('time','firstScheduledNight',capture(section(text,'3.2'),/first scheduled attack is night (\d+)/),'3.2');
 eq('time','stageOriginDay',r.initialDay,'2.1 dawn on day1;3.2 stage boundaries');
 eq('time','stageOriginIndex',capture(section(text,'3.2'),/days 1–3 are stage (\d+)/),'3.2');
 eq('time','regularWavesPerLivingPortalPerNight',words.indexOf(row(text,'3.2','Regular waves')[1].split(' ')[0].toLowerCase()),'3.2 regular wave table');
 exact('run','/succession/delaySeconds',r.succession.delaySeconds,capture(section(text,'2.2'),/transfer takes (\d+) second/),'2.2 succession delay');
 function policy(domain,p,value,expected,id,quote){need(section(text,id).includes(quote),'SOURCE_POLICY',quote);exact(domain,p,value,expected,id+' '+quote);}
 const policies=[
 ['run','/defaultMap',r.defaultMap,'EN-01','3.1','The terrain, sites, plants, and portal positions are authored content.'],
 ['run','/initialPhase',r.initialPhase,'dawn','2.1','starts at dawn'],
 ['run','/initialSiteState',r.initialSiteState,'unbuilt','2.1','an empty construction landscape at the marked sites'],
 ['run','/initialUncontrolledRole',r.initialUncontrolledRole,'unassigned','2.1','the rest are unassigned.'],
 ['run','/startingPersonalSupplies',r.startingPersonalSupplies,'outfit-only','2.1','They carry no personal money, food, weapon, ammunition, armor, or backpack'],
 ['run','/stashPerRunNotPerPlayer',r.stashPerRunNotPerPlayer,true,'2.1','Starting players do not add survivors or duplicate supplies.'],
 ['run','/recruitsReplenishStash',r.recruitsReplenishStash,false,'2.1','Purchased recruits do not replenish the initial stash.'],
 ['time','/activeGamePauses',t.activeGamePauses,false,'3.2','The world never pauses during active play'],
 ['time','/graceSuppressesProvocation',t.graceSuppressesProvocation,false,'3.2','Opening grace never suppresses provoked defenders or retaliation.'],
 ['time','/graceSuppressesRetaliation',t.graceSuppressesRetaliation,false,'3.2','Opening grace never suppresses provoked defenders or retaliation.'],
 ['time','/stageBoundary',t.stageBoundary,'dawn','3.2','Stage changes occur at dawn.'],
 ['survivors','/walkStaminaCost',s.walkStaminaCost,0,'4.1','walking never requires stamina.'],
 ['survivors','/unsafeHealthRegenerationPerSecond',s.unsafeHealthRegenerationPerSecond,0,'4.1','Health does not naturally regenerate outside the safe-camp conditions.'],
 ['survivors','/staminaRecoveryDuringCombat',s.staminaRecoveryDuringCombat,true,'4.1','Stamina can recover outside camp and during fights'],
 ['survivors','/spendingRestartsRegenerationDelay',s.spendingRestartsRegenerationDelay,true,'4.1','A spending action interrupts regeneration'],
 ['survivors','/requireSufficientStamina',s.requireSufficientStamina,true,'4.1','Actions require sufficient stamina'],
 ['survivors','/load/severeSprintAllowed',s.load.severeSprintAllowed,false,'4.1','No sprint or jump'],
 ['survivors','/load/severeJumpAllowed',s.load.severeJumpAllowed,false,'4.1','No sprint or jump'],
 ['survivors','/load/walkingAlwaysAllowed',s.load.walkingAlwaysAllowed,true,'4.1','Extremely heavy characters can still walk.'],
 ['survivors','/load/countNestedAndEquippedExactlyOnce',s.load.countNestedAndEquippedExactlyOnce,true,'4.1','Weight includes equipped items and every nested item exactly once.'],
 ['survivors','/safeRest',s.safeRest,{insideCamp:true,sprinting:false,jumping:false,attacking:false,ordinaryWalkingAllowed:true},'4.1','Inside camp, no enemy within 12 u, and not sprinting, jumping, or attacking; ordinary walking is allowed'],
 ['survivors','/controls/scope',s.controls.scope,'local-preferences','13.2','saved locally.'],
 ['survivors','/controls/remappable',s.controls.remappable,true,'13.2','All gameplay bindings must be remappable'],
 ['survivors','/controls/resetToDefaults',s.controls.resetToDefaults,true,'13.2','provide reset-to-default.']];
 for(const args of policies)policy(...args);
 const bindings=[['moveLeft','Move',['A','Left']],['moveRight','Move',['D','Right']],['sprint','Sprint','Shift'],['jump','Jump','Space'],['attack','Attack / manually fire bow','MouseLeft'],['aim','Aim bow','mouse-position'],['interact','Interact with nearby target','E'],['inventory','Own inventory','I'],['rotate','Rotate a dragged item','R'],['quickTransfer','Quick transfer between open inventories','Ctrl+click'],['splitStack','Split a stack','Shift+drag-choose-quantity'],['quickFood','Use quick-slot food',['1','2','3','4']],['map','Expand/minimize map view','Tab'],['closePanelOrMenu','Close panel / in-run menu','Escape']];
 for(const [key,label,value]of bindings){const sourceDefault=row(text,'13.2',label)[1];exact('survivors','/controls/'+key,s.controls[key],value,'13.2 '+label+' = '+sourceDefault);}
 const proofs=[['2.1','Starting players do not add survivors or duplicate supplies.'],['2.1','Purchased recruits do not replenish the initial stash.'],['3.2','The world never pauses during active play'],['3.2','Opening grace never suppresses provoked defenders or retaliation.'],['4.1','walking never requires stamina.'],['4.1','Stamina can recover outside camp and during fights'],['4.1','Extremely heavy characters can still walk.']];
 for(const [id,quote]of proofs)need(section(text,id).includes(quote),'SOURCE_POLICY',quote);
 return observations;
}
function loadDomains(names,schemaName){const schema=read(BASE+'schema/'+schemaName+'.json'),docs=Object.fromEntries(names.map(name=>[name,read(BASE+'config/'+name+'.json')]));
 check(schema.sourceRevision===SOURCE_REVISION,BASE+'schema/'+schemaName+'.json','/sourceRevision',schema.sourceRevision,'SOURCE_REVISION');return {schema,docs};}
function validateSet(docs,schema,refs){let fields=0;for(const file of schema.files){const name=path.basename(file.path,'.json');check(Object.hasOwn(docs,name),file.path,'',name,'MISSING_DOMAIN');fields+=validateDocument(file.path,docs[name],file,refs);}return fields;}
function schemaEvidence(schema,docs,text){check(hash(text)===schema.sourceSha256,'schema','/sourceSha256',schema.sourceSha256,'SOURCE_HASH');for(const file of schema.files){const data=docs[path.basename(file.path,'.json')];for(const f of file.fields){let value=data;for(const k of f.path.split('/').slice(1))value=value?.[k];check(equal(value,f.default),file.path,f.path,f.default,'DOCUMENTED_DEFAULT');check(section(text,f.source.section).includes(f.source.evidence),file.path,f.path,f.source,'SOURCE_CROSSWALK');}}}
function negative(controls,name,file,field,reason,action){let caught;try{action();}catch(error){caught=error;}check(caught?.diagnostic?.file===file&&caught.diagnostic.field===field&&caught.diagnostic.reason===reason,'negative-controls',name,caught?.message??'<accepted>','CONTROL_ORACLE');controls.push({name,status:'rejected',...caught.diagnostic});}
async function foundation(context){const before=inputs(FOUNDATION,'fields-foundation'),{schema,docs}=loadDomains(FOUNDATION,'fields-foundation'),refs=referenceSets(),text=source();write(context.directory,'inputs.json',before);write(context.directory,'source-spec.md',text);
 const fields=validateSet(docs,schema,refs);schemaEvidence(schema,docs,text);const crosswalk=foundationSource(docs,text),relationships=validateFoundation(docs,refs),controls=[];
 const file=BASE+'config/time.json',bad=(name,p,v,reason)=>{const copy=structuredClone(docs),parts=p.split('/').slice(1);let subject=copy.time;for(const k of parts.slice(0,-1))subject=subject[k];subject[parts.at(-1)]=v;negative(controls,name,file,p,reason,()=>validateSet(copy,schema,refs));};
 bad('zero-duration','/daylightSeconds',0,'BOUNDS');bad('negative-duration','/nightSeconds',-1,'BOUNDS');bad('wrong-type','/nightSeconds','300','TYPE');bad('nonfinite','/nightSeconds',Infinity,'FINITE');bad('fractional-grace','/openingWaveFreeDays',1.5,'INTEGER');bad('unknown-field','/hiddenDefault',1,'UNKNOWN_FIELD');
 const missing=structuredClone(docs);delete missing.survivors.maximumHealth;negative(controls,'missing-health',BASE+'config/survivors.json','/maximumHealth','MISSING_FIELD',()=>validateSet(missing,schema,refs));
 const ref=structuredClone(docs);ref.run.startingOutfit[0]='it-11';negative(controls,'ordinal-item-id',BASE+'config/run.json','/startingOutfit/0','ITEM_REFERENCE',()=>validateSet(ref,schema,refs));
 const body=structuredClone(docs);body.survivors.appearance.bodyIds[0]='CH-99';negative(controls,'unknown-body',BASE+'config/survivors.json','/appearance/bodyIds/0','BODY_REFERENCE',()=>validateSet(body,schema,refs));
 const map=structuredClone(docs);map.run.defaultMap='EN-99';negative(controls,'unknown-map',BASE+'config/run.json','/defaultMap','CONTENT_REFERENCE',()=>validateSet(map,schema,refs));
 const resource=structuredClone(docs);resource.survivors.appearance.pack='../outside.json';negative(controls,'unsafe-resource',BASE+'config/survivors.json','/appearance/pack','RESOURCE_REFERENCE',()=>validateSet(resource,schema,refs));
 for(const [name,edit,domain,p,reason]of [
  ['reversed-load',x=>{x.survivors.load.severeKilograms=10;},'survivors','/load','LOAD_RELATIONSHIP'],
  ['invalid-cap',x=>{x.run.recruitedSurvivorCap=2;},'run','/startingSurvivors','POPULATION_RELATIONSHIP'],
  ['invalid-stacks',x=>{x.run.initialStash[0].stackQuantity=101;},'run','/initialStash/0/stackQuantity','STACK_RELATIONSHIP'],
  ['duplicate-supply',x=>{x.run.initialStash[1].item=x.run.initialStash[0].item;},'run','/initialStash/1/item','DUPLICATE_ID'],
  ['invalid-first-night',x=>{x.time.firstScheduledNight=4;},'time','/firstScheduledNight','TIME_RELATIONSHIP']]){const copy=structuredClone(docs);edit(copy);negative(controls,name,BASE+'config/'+domain+'.json',p,reason,()=>{validateSet(copy,schema,refs);validateFoundation(copy,refs);});}
 negative(controls,'duplicate-json-key','fixture.json','/a','DUPLICATE_KEY',()=>strictParse(Buffer.from('{"a":1,"a":2}'),'fixture.json'));
 negative(controls,'overflow-json-number','fixture.json','/n','FINITE',()=>strictParse(Buffer.from('{"n":1e999}'),'fixture.json'));
 negative(controls,'invalid-utf8','fixture.json','','ENCODING',()=>strictParse(Buffer.from([0xff]),'fixture.json'));
 const edited=structuredClone(docs);edited.time.daylightSeconds=720;edited.survivors.load.softKilograms=22;validateSet(edited,schema,refs);validateFoundation(edited,refs);
 write(context.directory,'source-crosswalk.json',crosswalk);write(context.directory,'fields.json',describeFields(schema));write(context.directory,'negative-controls.json',controls);write(context.directory,'valid-edit.json',{daylightSeconds:720,softKilograms:22,accepted:true});assert.deepEqual(inputs(FOUNDATION,'fields-foundation'),before);
 return {fields,sourceAssertions:crosswalk.length,negativeControls:controls.length,relationships,validEdits:2,scope:'DT-01/02/03 data only; map obligation declared, resolved map and final hub/appearance pending'};
}
function inputs(names,schema){const paths=[...names.map(x=>BASE+'config/'+x+'.json'),BASE+'schema/'+schema+'.json','tools/phase4/cases/data-foundation.cjs','tools/phase4/check.cjs','tools/phase4/content.cjs','tools/phase4/case-discovery.cjs','tools/phase4/expected-obligations.json',...['items-01-10','items-11-24','bodies-a'].map(x=>BASE+'packs/'+x+'.json')];return {node:process.version,sourceRevision:SOURCE_REVISION,files:paths.map(file=>({file,sha256:hash(content.bytes(ROOT,file))}))};}
const metadata={group:'data',title:'Complete run time survivor baseline and exhaustive source field inventory',importance:'Critical',rationale:'Invalid shared baseline or silent fallback would prevent a coherent run and lose source identity',needsGameWindow:false,runGroup:'G0-data',order:40,requirements:['ASSET-02','ASSET-04'],acceptance:['AC-03','AC-11','AC-13'],setup:'Pinned committed specification, actual item/body packs, authored separate configs and field inventory; no game window',actions:['Read strict bounded UTF-8 documents with exact ordinal paths','Walk every field and its meaning unit default range dependencies and source','Compare complete numerical source rows and prose-derived relationships','Reject independent malformed documents missing fields references bounds and incoherent relationships','Accept independent designer edits without a hidden fallback'],expected:'Exact baseline/source crosswalk and strict validation pass; actual gameplay and final resource inspection remain pending',reset:'Fresh immutable source invocation; isolated in-memory edits only; no process or window',evidence:['inputs.json','source-spec.md','source-crosswalk.json','fields.json','negative-controls.json','valid-edit.json','result.json'],backstops:[{id:'H24',observation:'authored-units-and-exact-source-baselines',renderedCheckpoint:'data-formatting'},{id:'UI-TEXT-STATUS/error',observation:'file-field-value-reason',renderedCheckpoint:'data-errors'}]};
runner.registerCase('data.foundation',metadata,foundation);
module.exports={BASE,SOURCE_REVISION,FOUNDATION,fault,check,strictParse,read,source,section,row,numbers,capture,roster,referenceSets,describeFields,validateDocument,validateFoundation,foundationSource,loadDomains,validateSet,schemaEvidence,negative,foundation,inputs,metadata};
