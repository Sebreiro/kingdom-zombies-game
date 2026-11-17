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
 const ws=()=>{while(/[ \t\r\n]/.test(text[at]??'')&&at<text.length)at++;};
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
function schemaEvidence(schema,docs,text){check(hash(text)===schema.sourceSha256,'schema','/sourceSha256',schema.sourceSha256,'SOURCE_HASH');
 const units={weightKilograms:'kg/item',footprint:'cells',storage:'cells',pageGrid:'cells',maxStack:'items/stack',quantity:'items',stackQuantity:'items/stack',stacks:'stacks',daylightSeconds:'seconds',nightSeconds:'seconds',jumpAirborneSeconds:'seconds',staminaRegenerationDelaySeconds:'seconds',safeRestDelaySeconds:'seconds',foodCooldownSeconds:'seconds',intervalSeconds:'seconds',lifetimeSeconds:'seconds',emptyDelaySeconds:'seconds',accessSeconds:'seconds',softKilograms:'kg',severeKilograms:'kg',walkSpeedUnitsPerSecond:'u/second',sprintSpeedUnitsPerSecond:'u/second',speedUnitsPerSecond:'u/second',jumpHeightUnits:'u',safeEnemyDistanceUnits:'u',refugeDistanceUnits:'u',reachUnits:'u',targetRangeUnits:'u',mainBuildingUnits:'u',npcUnits:'u',corpseUnits:'u',siteUnits:'u',rallyUnits:'u',airWalkControlMultiplier:'ratio',minimumSpeedMultiplier:'ratio',maximumActionCostMultiplier:'ratio',reductionFraction:'ratio',rawDamageWearFraction:'ratio',maximumReductionFraction:'ratio',returnSoftLoadFraction:'ratio',friendlyDamageMultiplier:'ratio',slowdownFraction:'ratio'};
 for(const file of schema.files){const data=docs[path.basename(file.path,'.json')];for(const f of file.fields){let value=data;const parts=f.path.split('/').slice(1);for(const k of parts)value=value?.[k];check(equal(value,f.default),file.path,f.path,f.default,'DOCUMENTED_DEFAULT');check(section(text,f.source.section).includes(f.source.evidence),file.path,f.path,f.source,'SOURCE_CROSSWALK');const key=/^\d+$/.test(parts.at(-1))?parts.at(-2):parts.at(-1);if(units[key])check(f.unit===units[key],file.path,f.path,f.unit,'SOURCE_UNIT');}}
}
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
function inputs(names,schema){const paths=[...names.map(x=>BASE+'config/'+x+'.json'),BASE+'schema/'+schema+'.json','tools/phase4/cases/data-foundation.cjs','tools/phase4/check.cjs','tools/phase4/content.cjs','tools/phase4/case-discovery.cjs','tools/phase4/expected-obligations.json',...['items-01-10','items-11-24','bodies-a'].map(x=>BASE+'packs/'+x+'.json')];
 if(names.includes('items'))paths.push(BASE+'animation/fits-a.json',BASE+'config/survivors.json',...['items-01-10','items-11-24'].map(x=>'src/godot-game/'+read(BASE+'packs/'+x+'.json').atlas.path));
 return {node:process.version,sourceRevision:SOURCE_REVISION,files:paths.map(file=>({file,sha256:hash(content.bytes(ROOT,file))}))};}
const metadata={group:'data',title:'Complete run time survivor baseline and exhaustive source field inventory',importance:'Critical',rationale:'Invalid shared baseline or silent fallback would prevent a coherent run and lose source identity',needsGameWindow:false,runGroup:'G0-data',order:40,requirements:['ASSET-02','ASSET-04'],acceptance:['AC-03','AC-11','AC-13'],setup:'Pinned committed specification, actual item/body packs, authored separate configs and field inventory; no game window',actions:['Read strict bounded UTF-8 documents with exact ordinal paths','Walk every field and its meaning unit default range dependencies and source','Compare complete numerical source rows and prose-derived relationships','Reject independent malformed documents missing fields references bounds and incoherent relationships','Accept independent designer edits without a hidden fallback'],expected:'Exact baseline/source crosswalk and strict validation pass; actual gameplay and final resource inspection remain pending',reset:'Fresh immutable source invocation; isolated in-memory edits only; no process or window',evidence:['inputs.json','source-spec.md','source-crosswalk.json','fields.json','negative-controls.json','valid-edit.json','result.json'],backstops:[{id:'H24',observation:'authored-units-and-exact-source-baselines',renderedCheckpoint:'data-formatting'},{id:'UI-TEXT-STATUS/error',observation:'file-field-value-reason',renderedCheckpoint:'data-errors'}]};
runner.registerCase('data.foundation',metadata,foundation);
module.exports={BASE,SOURCE_REVISION,FOUNDATION,fault,check,strictParse,read,source,section,row,numbers,capture,roster,referenceSets,describeFields,validateDocument,validateFoundation,foundationSource,loadDomains,validateSet,schemaEvidence,negative,foundation,inputs,metadata};

const ITEMS_COMBAT=['items','combat','inventory'];
function legalLoadout(left,right,definitions){
 const file=BASE+'config/combat.json',map=new Map(definitions.map(x=>[x.id,x]));
 for(const [hand,id]of [['left',left],['right',right]])if(id!==null){check(map.has(id),file,'/loadout/'+hand,id,'ITEM_REFERENCE');check(map.get(id).slot==='either-logical-hand',file,'/loadout/'+hand,id,'SLOT_RELATIONSHIP');}
 const bow=definitions.find(x=>x.name==='Bow')?.id,arrows=definitions.find(x=>x.name==='Arrows')?.id;
 if(left===bow&&right!==null)check(right===arrows,file,'/loadout/right',right,'HAND_RELATIONSHIP');
 if(right===bow&&left!==null)check(left===arrows,file,'/loadout/left',left,'HAND_RELATIONSHIP');
 return {left,right,legal:true};
}
function itemReferences(item,index,refs,load=(file)=>content.bytes(ROOT,file)){
 const file=BASE+'config/items.json',p='/definitions/'+index,actual=refs.items.get(item.id);
 check(actual,file,p+'/id',item.id,'ITEM_REFERENCE');
 for(const key of ['nameKey','descriptionKey'])check(item[key]===actual[key],file,p+'/'+key,item[key],'TEXT_REFERENCE');
 const pack='content/packs/'+actual.pack.id+'.json';check(item.appearance.pack===pack,file,p+'/appearance/pack',item.appearance.pack,'RESOURCE_REFERENCE');
 check(item.appearance.item===item.id,file,p+'/appearance/item',item.appearance.item,'APPEARANCE_REFERENCE');
 check(item.appearance.canonicalIcon===actual.canonicalIcon&&actual.pack.parts.some(x=>x.key===item.appearance.canonicalIcon),file,p+'/appearance/canonicalIcon',item.appearance.canonicalIcon,'IMAGE_REFERENCE');
 const atlas='src/godot-game/'+actual.pack.atlas.path;let bytes;try{bytes=load(atlas);}catch{fault(file,p+'/appearance/canonicalIcon',atlas,'IMAGE_MISSING');}
 check(Buffer.isBuffer(bytes)&&hash(bytes)===actual.pack.atlas.sha256,file,p+'/appearance/canonicalIcon',atlas,'IMAGE_IDENTITY');
 const fits=refs.fits.items.some(x=>x.id===item.id);
 check(item.appearance.fitFile===(fits?'content/animation/fits-a.json':null),file,p+'/appearance/fitFile',item.appearance.fitFile,'FIT_REFERENCE');
 check(item.appearance.fitItem===(fits?item.id:null),file,p+'/appearance/fitItem',item.appearance.fitItem,'FIT_REFERENCE');
}
function validateItemsCombat(docs,refs){const {items,combat:c,inventory:i}=docs,file=BASE+'config/items.json',defs=items.definitions,ids=new Set();
 for(const [n,item]of defs.entries()){check(!ids.has(item.id),file,'/definitions/'+n+'/id',item.id,'DUPLICATE_ID');ids.add(item.id);}
 for(const [n,item]of defs.entries()){
  itemReferences(item,n,refs);
  check(item.footprint.every(x=>Number.isSafeInteger(x)&&x>0&&x<=256),file,'/definitions/'+n+'/footprint',item.footprint,'GRID_BOUNDS');
  const [w,h]=item.footprint,[pw,ph]=i.stash.pageGrid;
  check(w<=pw&&h<=ph||item.rotatable&&h<=pw&&w<=ph,file,'/definitions/'+n+'/footprint',item.footprint,'STASH_FOOTPRINT');
  if(item.storage!==null){check(item.storage.every(x=>Number.isSafeInteger(x)&&x>0&&x<=256),file,'/definitions/'+n+'/storage',item.storage,'GRID_BOUNDS');check(item.maxStack===1,file,'/definitions/'+n+'/maxStack',item.maxStack,'CONTAINER_STACK');}
  if(item.slot!==null)check(item.maxStack===1||item.id===c.handRules.bowOtherHand,file,'/definitions/'+n+'/maxStack',item.maxStack,'EQUIPMENT_STACK');
  if(item.conditionMaximum!==null)check(item.maxStack===1,file,'/definitions/'+n+'/maxStack',item.maxStack,'CONDITION_STACK');
 }
 const combatFile=BASE+'config/combat.json',projectileIds=new Set();
 for(const [n,x]of c.projectiles.entries()){check(typeof x.projectileId==='string'&&/^[a-z][a-z0-9.-]*$/.test(x.projectileId),combatFile,'/projectiles/'+n+'/projectileId',x.projectileId,'ORDINAL_ID');check(!projectileIds.has(x.projectileId),combatFile,'/projectiles/'+n+'/projectileId',x.projectileId,'DUPLICATE_ID');projectileIds.add(x.projectileId);
  if(x.targetRangeUnits!==null)check(x.targetRangeUnits<=x.speedUnitsPerSecond*x.lifetimeSeconds,combatFile,'/projectiles/'+n+'/targetRangeUnits',x.targetRangeUnits,'PROJECTILE_RELATIONSHIP');}
 const attacks=new Set();for(const [n,x]of c.attacks.entries()){check(typeof x.attackId==='string'&&/^[a-z][a-z0-9.-]*$/.test(x.attackId),combatFile,'/attacks/'+n+'/attackId',x.attackId,'ORDINAL_ID');check(!attacks.has(x.attackId),combatFile,'/attacks/'+n+'/attackId',x.attackId,'DUPLICATE_ID');attacks.add(x.attackId);
  if(x.projectile!==null)check(projectileIds.has(x.projectile),combatFile,'/attacks/'+n+'/projectile',x.projectile,'PROJECTILE_REFERENCE');
  if(x.itemRef!==null)check(defs.some(d=>d.id===x.itemRef&&d.conditionMaximum>0&&d.slot==='either-logical-hand'),combatFile,'/attacks/'+n+'/itemRef',x.itemRef,'WEAPON_RELATIONSHIP');}
 check(attacks.has(c.handRules.emptyHandsAttack),combatFile,'/handRules/emptyHandsAttack',c.handRules.emptyHandsAttack,'ATTACK_REFERENCE');
 const armorIds=new Set();for(const [n,a]of c.armor.entries()){check(!armorIds.has(a.itemRef),combatFile,'/armor/'+n+'/itemRef',a.itemRef,'DUPLICATE_ID');armorIds.add(a.itemRef);const item=defs.find(x=>x.id===a.itemRef);
  check(item&&['body-armor','headwear'].includes(item.slot)&&item.conditionMaximum===a.maximumDurability,combatFile,'/armor/'+n+'/maximumDurability',a.maximumDurability,'ARMOR_RELATIONSHIP');
  check(a.reductionFraction>=0&&a.reductionFraction<=1&&a.rawDamageWearFraction>=0&&a.rawDamageWearFraction<=1,combatFile,'/armor/'+n,a,'PROBABILITY_BOUNDS');}
 check(c.maximumReductionFraction>=0&&c.maximumReductionFraction<=1,combatFile,'/maximumReductionFraction',c.maximumReductionFraction,'PROBABILITY_BOUNDS');
 check(defs.find(x=>x.id===c.handRules.bowOtherHand)?.name==='Arrows',combatFile,'/handRules/bowOtherHand',c.handRules.bowOtherHand,'HAND_RELATIONSHIP');
 check(i.hauling.returnSoftLoadFraction>0&&i.hauling.returnSoftLoadFraction<=1,BASE+'config/inventory.json','/hauling/returnSoftLoadFraction',i.hauling.returnSoftLoadFraction,'LOAD_RELATIONSHIP');
 check(i.nesting.maximumLevels>=i.nesting.rootContainerLevel&&i.nesting.maximumLevels<=16,BASE+'config/inventory.json','/nesting/maximumLevels',i.nesting.maximumLevels,'NESTING_RELATIONSHIP');
 return {items:defs.length,attacks:attacks.size,projectiles:projectileIds.size,armor:armorIds.size,stashGrid:i.stash.pageGrid,maximumNesting:i.nesting.maximumLevels,haulKilograms:read(BASE+'config/survivors.json').load.softKilograms*i.hauling.returnSoftLoadFraction};
}
function itemsSource(docs,text){const expectedItems=roster(text),observations=[],{items,combat:c,inventory:i}=docs;
 const exact=(domain,p,value,expected,source)=>{check(equal(value,expected),BASE+'config/'+domain+'.json',p,value,'SOURCE_BASELINE');observations.push({domain,field:p,value,expected,source});};
 for(const [n,expected]of expectedItems.entries()){
  const item=items.definitions[n];for(const key of ['id','name','footprint','maxStack','weightKilograms'])exact('items','/definitions/'+n+'/'+key,item[key],expected[key],'5.3 source roster row '+expected.name);
  const storage=expected.purpose.includes('pocket')||expected.purpose.includes('grid')?numbers(expected.purpose):null;
  exact('items','/definitions/'+n+'/storage',item.storage,storage,'5.3 purpose/storage '+expected.purpose);
  exact('items','/definitions/'+n+'/rotatable',item.rotatable,section(text,'5.3').includes('Every footprint below may rotate.'),'5.3 all footprints may rotate');
  const purpose=expected.purpose.toLowerCase(),slot=purpose.includes('hand')||purpose.includes('melee')?'either-logical-hand':purpose.startsWith('torso')?'torso-clothing':purpose.startsWith('legs')?'legs':purpose.startsWith('headwear')?'headwear':purpose.startsWith('face')?'face':purpose.startsWith('gloves')?'gloves':purpose.startsWith('boots')?'boots':purpose.startsWith('body armor')?'body-armor':purpose.startsWith('belt')?'belt':purpose.startsWith('backpack')?'backpack':null;
  exact('items','/definitions/'+n+'/slot',item.slot,slot,'5.3 purpose '+expected.purpose+' and5.2 slot contract');
 }
 const attackRows=section(text,'4.3').split('\n').filter(x=>/^\| (Survivor fists|Knife|Wooden club|Machete|Metal bat|Bow|Spitter natural spit) \|/.test(x)).map(x=>x.split('|').slice(1,-1).map(x=>x.trim()));
 check(c.attacks.length===attackRows.length,BASE+'config/combat.json','/attacks',c.attacks.length,'SOURCE_BASELINE');
 for(const [n,r]of attackRows.entries()){const a=c.attacks[n],item=expectedItems.find(x=>x.name===r[0]),isBow=r[0]==='Bow',isSpit=r[0]==='Spitter natural spit';
  for(const [key,expected]of [['itemRef',item?.id??null],['damage',Number(r[1])],['intervalSeconds',numbers(r[2])[0]],['reachUnits',isBow?null:numbers(r[3])[0]],['staminaCost',Number(r[4])]])exact('combat','/attacks/'+n+'/'+key,a[key],expected,'4.3 '+r[0]);
  if(item)exact('items','/definitions/'+expectedItems.indexOf(item)+'/conditionMaximum',items.definitions[expectedItems.indexOf(item)].conditionMaximum,Number(r[5]),'4.3 weapon durability');
  if(isBow||isSpit){const projectile=c.projectiles.find(x=>x.projectileId===(isBow?'arrow':'spit')),values=numbers(r[3]);exact('combat','/projectiles/'+(isBow?0:1)+'/speedUnitsPerSecond',projectile.speedUnitsPerSecond,values[isBow?0:1],'4.3 projectile speed');exact('combat','/projectiles/'+(isBow?0:1)+'/lifetimeSeconds',projectile.lifetimeSeconds,values[isBow?1:2],'4.3 projectile lifetime');}
 }
 const armorRows=section(text,'4.3').split('\n').filter(x=>/^\| (Improvised body armor|Plated body armor|Protective helmet) \|/.test(x)).map(x=>x.split('|').slice(1,-1).map(x=>x.trim()));
 for(const [n,r]of armorRows.entries()){const armor=c.armor[n],item=expectedItems.find(x=>x.name===r[0]);for(const [key,value]of [['itemRef',item.id],['reductionFraction',numbers(r[1])[0]/100],['maximumDurability',Number(r[2])],['rawDamageWearFraction',numbers(r[3])[0]/100]])exact('combat','/armor/'+n+'/'+key,armor[key],value,'4.3 '+r[0]);exact('items','/definitions/'+expectedItems.indexOf(item)+'/conditionMaximum',items.definitions[expectedItems.indexOf(item)].conditionMaximum,Number(r[2]),'4.3 armor maximum');}
 const stashRow=row(text,'5.2','Stash page')[1],nestingRow=row(text,'5.2','Maximum nested containers')[1];
 exact('inventory','/stash/pageGrid',i.stash.pageGrid,numbers(stashRow).slice(0,2),'5.2 stash grid');
 const words=['zero','one','two','three','four','five','six'];exact('inventory','/nesting/maximumLevels',i.nesting.maximumLevels,words.indexOf(nestingRow.split(' ')[0].toLowerCase()),'5.2 nesting');exact('inventory','/nesting/rootContainerLevel',i.nesting.rootContainerLevel,numbers(nestingRow)[0],'5.2 root level');exact('inventory','/quickSlots/count',i.quickSlots.count,words.indexOf(row(text,'5.2','Quick slots')[1].split(' ')[0].toLowerCase()),'5.2 quick references');
 const ranges=numbers(row(text,'4.1','Interaction distance')[1]);for(const key of Object.keys(i.ranges))exact('inventory','/ranges/'+key,i.ranges[key],ranges[key==='mainBuildingUnits'?0:1],'4.1 interaction distances');
 exact('inventory','/hauling/returnSoftLoadFraction',i.hauling.returnSoftLoadFraction,capture(section(text,'5.5'),/weight reaches (\d+)%/)/100,'5.5 hauling');exact('inventory','/cleanup/emptyDelaySeconds',i.cleanup.emptyDelaySeconds,capture(section(text,'5.5'),/expires after (\d+) seconds/),'5.5 physical cleanup');
 exact('combat','/maximumReductionFraction',c.maximumReductionFraction,capture(section(text,'4.3'),/capped at (\d+)%/)/100,'4.3 armor cap');
 const contact=/At most (\w+) melee zombies can strike one structure and (\w+) can strike one survivor/.exec(section(text,'4.2'));need(contact,'SOURCE_CONTACT');exact('combat','/contactLimits/structure',c.contactLimits.structure,words.indexOf(contact[1]),'4.2 contact cap');exact('combat','/contactLimits/survivor',c.contactLimits.survivor,words.indexOf(contact[2]),'4.2 contact cap');
 exact('combat','/projectiles/1/targetRangeUnits',c.projectiles[1].targetRangeUnits,numbers(row(text,'4.3','Spitter natural spit')[3])[0],'4.3 spitter targeting range independent of flight lifetime');
 const policies=[['4.3','weapons cannot make weapon attacks'],['4.3','Ordinary clothes, bags, and belts do not wear out'],['4.2','A dual attack uses the slower weapon\'s interval and the sum of both stamina costs.'],['5.6','Preserve saved positions, orientation, old display footprints/grid boundaries'],['5.6','Missing item definitions or unreadable save data produce an explicit error'],['5.5','Nonempty corpses and loot piles never expire because of age.']];for(const [id,quote]of policies)need(section(text,id).includes(quote),'SOURCE_POLICY',quote);
 function policy(domain,p,value,expected,id,quote){need(section(text,id).includes(quote),'SOURCE_POLICY',quote);exact(domain,p,value,expected,id+' '+quote);}
 const checks=[
 ['combat','/durability',c.durability,{meleeLossPerSuccessfulStrike:1,bowLossPerShot:1,zeroWeaponUsable:false,zeroArmorProtects:false,retainBrokenItems:true,ordinaryClothingWears:false,repairAllowed:false},'4.3','Melee weapons lose one durability per successful weapon strike; bows lose one per shot.'],
 ['combat','/armorCombination',c.armorCombination,'multiplicative','4.3','Protection combines multiplicatively'],
 ['combat','/armorWearSource',c.armorWearSource,'same-raw-hit-no-extra-damage','4.3','Armor wear is calculated from the same raw hit, without increasing the damage dealt.'],
 ['combat','/armorAppliesPerHit',c.armorAppliesPerHit,true,'4.2','Armor applies to each hit separately.'],
 ['combat','/friendlyDamageMultiplier',c.friendlyDamageMultiplier,0,'4.2','Friendly attacks cause no allied damage.'],
 ['combat','/handRules',c.handRules,{emptyHandsAttack:'survivor.fists',dualOneHanded:true,dualInterval:'maximum-of-both',dualStamina:'sum-of-both',dualTargets:'independent-per-weapon-may-share-target',targetsPerWeaponSwing:1,bowOtherHand:'IT-02',playerAutoRefill:false,npcZombieAutoRefill:true,actualArrowsConsumed:true,fallback:'usable-carried-weapon-or-fists'},'4.2','With two one-handed weapons, one input swings both together.'],
 ['combat','/cosmeticAcid',c.cosmeticAcid,{damagePerSecond:0,durabilityLoss:0,slowdownFraction:0,blocksMovement:false,statusEffect:false},'4.2','It causes no damage, status effect, slowdown, durability loss, or movement obstruction.'],
 ['inventory','/stash/pageLimit',i.stash.pageLimit,'unlimited','5.4','unlimited aggregate space through new pages'],
 ['inventory','/stash/tierIndependent',i.stash.tierIndependent,true,'5.4','independent of building tier.'],
 ['inventory','/stash/remoteAccess',i.stash.remoteAccess,false,'5.4','There is no remote stash access.'],
 ['inventory','/stash/automaticPlacement',i.stash.automaticPlacement,['combine-compatible-stacks','fitting-empty-spaces','rotate-if-allowed','create-page'],'5.4','Automatic placement first combines compatible stacks'],
 ['inventory','/quickSlots/referencesOnly',i.quickSlots.referencesOnly,true,'5.2','no duplicate storage or independent supply'],
 ['inventory','/quickSlots/foodOnly',i.quickSlots.foodOnly,true,'5.2','references to food stacks in carried storage'],
 ['inventory','/quickSlots/invalidateWhenRemovedConsumedOrLost',i.quickSlots.invalidateWhenRemovedConsumedOrLost,true,'5.2','Removing it from the character, consuming it, or losing it invalidates the reference.'],
 ['inventory','/quickSlots/substituteSimilarStack',i.quickSlots.substituteSimilarStack,false,'5.2','Another similar stack is not silently substituted.'],
 ['inventory','/hauling/priority',i.hauling.priority,['money','ammunition','usable-combat-gear','food','containers','clothing','broken-gear'],'5.5','Loot priority is money, ammunition, usable combat gear, food, containers, clothing, then broken gear.'],
 ['inventory','/hauling/towerGuardsHaul',i.hauling.towerGuardsHaul,false,'5.5','Built-in tower guards never haul.'],
 ['inventory','/cleanup/nonemptyExpires',i.cleanup.nonemptyExpires,false,'5.5','Nonempty corpses and loot piles never expire because of age.'],
 ['inventory','/cleanup/addingItemCancels',i.cleanup.addingItemCancels,true,'5.5','Adding an item cancels that cleanup countdown.'],
 ['inventory','/cleanup/cosmeticsDeleteLoot',i.cleanup.cosmeticsDeleteLoot,false,'5.5','Visual fading cannot delete loot.'],
 ['inventory','/legacy',i.legacy,{preservePositions:true,preserveOrientation:true,preserveFootprints:true,preserveGridBounds:true,preserveContainment:true,markIncompatible:true,removalAllowed:true,invalidAdditionsAllowed:false,deeperInvalidNestingAllowed:false,destinationUsesCurrentDefinition:true,failedMoveRetainsSource:true,filledUnitExtraction:'legal-outer-placement-and-no-worse-invalid-internal-nesting',emptyAdoptsCurrentGrid:true,oversizeStackExtraction:'legal-substacks-no-truncation',illegalLoadoutCombatAllowed:false,persistThroughSaves:true,missingDefinition:'explicit-error-preserve-original-save'},'5.6','load saved layouts as they are and allow items to be taken out.'],
 ['inventory','/identityPolicy',i.identityPolicy,'one-owner-location-no-cycles','5.1','An item has exactly one owner/location.'],
 ['inventory','/accessSeconds',i.accessSeconds,0,'5.1','Transfers and equipping have no search delay or action timer.'],
 ['inventory','/privateControlledInventories',i.privateControlledInventories,true,'5.1','Other player-controlled inventories are private to their controllers.'],
 ['inventory','/dragPolicy',i.dragPolicy,'preview-until-valid-commit-preserve-on-close','5.1','Dragging previews a move; the actual transfer happens only when a valid destination accepts it.']];
 for(const args of checks)policy(...args);
 for(const item of items.definitions){if(!attackRows.some(x=>x[0]===item.name)&&!armorRows.some(x=>x[0]===item.name))exact('items','/definitions/'+items.definitions.indexOf(item)+'/conditionMaximum',item.conditionMaximum,null,'4.3 ordinary clothes bags belts do not wear;5.3 non-equipment');}
 // H24 compares both exact numeric identities and independent round-trip/display representations.
 const precision=expectedItems.map(x=>({id:x.id,authored:x.weightKilograms,serialized:JSON.stringify(x.weightKilograms),display:Number(x.weightKilograms).toFixed(2),footprint:x.footprint,maxStack:x.maxStack}));
 for(const x of precision)check(JSON.parse(x.serialized)===x.authored,'H24',x.id,x,'PRECISION');
 return {observations,precision};
}
async function itemsCombat(context){const before=inputs(ITEMS_COMBAT,'fields-items-combat'),{schema,docs}=loadDomains(ITEMS_COMBAT,'fields-items-combat'),refs=referenceSets();refs.fits=read(BASE+'animation/fits-a.json');const text=source();write(context.directory,'inputs.json',before);write(context.directory,'source-spec.md',text);
 const fields=validateSet(docs,schema,refs);schemaEvidence(schema,docs,text);const relationships=validateItemsCombat(docs,refs),sourceProof=itemsSource(docs,text),controls=[];
 function invalid(name,domain,p,value,reason,semantic=false){const copy=structuredClone(docs),keys=p.split('/').slice(1);let subject=copy[domain];for(const k of keys.slice(0,-1))subject=subject[k];subject[keys.at(-1)]=value;negative(controls,name,BASE+'config/'+domain+'.json',p,reason,()=>{validateSet(copy,schema,refs);if(semantic)validateItemsCombat(copy,refs);});}
 invalid('zero-weight','items','/definitions/0/weightKilograms',0,'BOUNDS');invalid('nonfinite-weight','items','/definitions/0/weightKilograms',NaN,'FINITE');invalid('fractional-footprint','items','/definitions/0/footprint/0',1.2,'INTEGER');invalid('negative-stack','items','/definitions/0/maxStack',-1,'BOUNDS');invalid('ordinal-item-id','items','/definitions/0/id','it-01','ITEM_REFERENCE');
 invalid('duplicate-item','items','/definitions/1/id','IT-01','DUPLICATE_ID',true);
 invalid('missing-icon','items','/definitions/0/appearance/canonicalIcon','missing','IMAGE_REFERENCE',true);invalid('missing-name-key','items','/definitions/0/nameKey','missing','TEXT_REFERENCE',true);invalid('missing-description-key','items','/definitions/0/descriptionKey','missing','TEXT_REFERENCE',true);
 invalid('missing-sound','items','/definitions/0/handlingSound','AU-99','CONTENT_REFERENCE');invalid('unknown-obligation','inventory','/inspectionLayoutObligations/0','UI-99','CONTENT_REFERENCE');
 invalid('missing-fit-item','items','/definitions/1/appearance/fitItem','IT-03','FIT_REFERENCE',true);
 invalid('missing-fit-resource','items','/definitions/1/appearance/fitFile','content/animation/missing.json','RESOURCE_REFERENCE');
 invalid('wrong-text-case','items','/definitions/0/nameKey','Item.IT-01.name','TEXT_REFERENCE',true);
 invalid('zero-attack-interval','combat','/attacks/0/intervalSeconds',0,'BOUNDS');invalid('nonfinite-projectile','combat','/projectiles/0/lifetimeSeconds',Infinity,'FINITE');invalid('duplicate-attack','combat','/attacks/1/attackId','survivor.fists','DUPLICATE_ID',true);invalid('duplicate-projectile','combat','/projectiles/1/projectileId','arrow','DUPLICATE_ID',true);
 invalid('uppercase-attack','combat','/attacks/0/attackId','Survivor.fists','ORDINAL_ID',true);
 invalid('armor-mismatch','combat','/armor/0/maximumDurability',99,'ARMOR_RELATIONSHIP',true);
 invalid('haul-over-soft-load','inventory','/hauling/returnSoftLoadFraction',1.1,'LOAD_RELATIONSHIP',true);
 invalid('excessive-nesting','inventory','/nesting/maximumLevels',17,'NESTING_RELATIONSHIP',true);
 invalid('unknown-field','inventory','/hiddenFallback',true,'UNKNOWN_FIELD');
 const missing=structuredClone(docs);delete missing.items.definitions[0].weightKilograms;negative(controls,'missing-item-weight',BASE+'config/items.json','/definitions/0/weightKilograms','MISSING_FIELD',()=>validateSet(missing,schema,refs));
 const grid=structuredClone(docs);grid.inventory.stash.pageGrid=[1,1];negative(controls,'stash-cannot-fit',BASE+'config/items.json','/definitions/1/footprint','STASH_FOOTPRINT',()=>{validateSet(grid,schema,refs);validateItemsCombat(grid,refs);});
 negative(controls,'missing-actual-atlas',BASE+'config/items.json','/definitions/0/appearance/canonicalIcon','IMAGE_MISSING',()=>itemReferences(docs.items.definitions[0],0,refs,()=>{throw Error('ENOENT');}));
 negative(controls,'tampered-actual-atlas',BASE+'config/items.json','/definitions/0/appearance/canonicalIcon','IMAGE_IDENTITY',()=>itemReferences(docs.items.definitions[0],0,refs,()=>Buffer.from('wrong-bytes')));
 const loadouts=[[null,null],['IT-03',null],['IT-03','IT-06'],['IT-07','IT-02'],['IT-02','IT-07']].map(([l,r])=>legalLoadout(l,r,docs.items.definitions));
 for(const [name,l,r,p,reason]of [['bow-knife','IT-07','IT-03','right','HAND_RELATIONSHIP'],['two-bows','IT-07','IT-07','right','HAND_RELATIONSHIP'],['wearable-in-hand','IT-17',null,'left','SLOT_RELATIONSHIP'],['unknown-hand-item','IT-99',null,'left','ITEM_REFERENCE']])negative(controls,name,BASE+'config/combat.json','/loadout/'+p,reason,()=>legalLoadout(l,r,docs.items.definitions));
 const edited=structuredClone(docs);edited.items.definitions[0].weightKilograms=0.002;edited.combat.attacks[0].intervalSeconds=0.8;edited.inventory.stash.pageGrid=[12,10];validateSet(edited,schema,refs);validateItemsCombat(edited,refs);
 const rounded=structuredClone(docs);rounded.items.definitions[0].weightKilograms=Number(rounded.items.definitions[0].weightKilograms.toFixed(2));negative(controls,'H24-rounded-default',BASE+'config/items.json','/definitions/0/weightKilograms','SOURCE_BASELINE',()=>itemsSource(rounded,text));
 const wrongUnit=structuredClone(schema),weight=wrongUnit.files[0].fields.find(x=>x.path==='/definitions/0/weightKilograms');weight.unit='g/item';negative(controls,'H24-unit-conversion',BASE+'config/items.json',weight.path,'SOURCE_UNIT',()=>schemaEvidence(wrongUnit,docs,text));
 write(context.directory,'fields.json',describeFields(schema));write(context.directory,'source-crosswalk.json',sourceProof.observations);write(context.directory,'H24-precision.json',sourceProof.precision);write(context.directory,'negative-controls.json',controls);write(context.directory,'legal-loadouts.json',loadouts);write(context.directory,'valid-edit.json',{moneyWeightKilograms:0.002,fistIntervalSeconds:0.8,stashGrid:[12,10],accepted:true});assert.deepEqual(inputs(ITEMS_COMBAT,'fields-items-combat'),before);
 return {fields,sourceAssertions:sourceProof.observations.length,negativeControls:controls.length,legalLoadouts:loadouts.length,relationships,scope:'DT-04/05/06 data; TXT/sound obligations declared only; no gameplay transactions or ASSET completion'};
}
runner.registerCase('data.items-combat',{...metadata,title:'All canonical items full combat and inventory baselines with independent H24 precision',order:41,actions:['Compare every24-item source row and actual icon/name/appearance/fit binding','Compare attack armor projectile contact storage timing and load baselines directly with source','Validate legal hand pairs stash geometry armor condition and reference relationships','Reject independent missing corrupt nonfinite ordinal duplicate bounds and precision-loss fixtures','Accept designer edits while keeping source defaults separately authenticated'],evidence:['inputs.json','source-spec.md','source-crosswalk.json','fields.json','H24-precision.json','negative-controls.json','legal-loadouts.json','valid-edit.json','result.json']},itemsCombat);
Object.assign(module.exports,{ITEMS_COMBAT,legalLoadout,itemReferences,validateItemsCombat,itemsSource,itemsCombat});
