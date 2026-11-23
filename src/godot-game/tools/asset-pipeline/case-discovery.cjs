'use strict';
const fs=require('node:fs'),path=require('node:path'),crypto=require('node:crypto');
const ordinal=(a,b)=>a<b?-1:a>b?1:0;
function need(ok,code,detail=''){if(!ok)throw new Error(code+' '+detail);}
const stable=value=>typeof value==='string'&&value.length<=128&&/^[a-z][a-z0-9]*(?:[.-][a-z0-9]+)*$/.test(value);
const label=value=>typeof value==='string'&&value.trim().length>0&&value.length<=4096&&
  value===value.normalize('NFC')&&!/[\u0000-\u001f\ud800-\udfff]/u.test(value);
function createCaseRegistry(){
  const cases=new Map(),positions=new Set();
  function registerCase(id,definition,run){
    need(stable(id),'CASE_ID_INVALID',String(id));need(!cases.has(id),'CASE_ID_DUPLICATE',id);
    need(typeof run==='function','CASE_RUN',id);
    need(definition&&typeof definition==='object'&&!Array.isArray(definition)&&
      !Object.hasOwn(definition,'id')&&!Object.hasOwn(definition,'run'),'CASE_METADATA',id+' reserved');
    for(const field of ['title','rationale','setup','expected','reset'])need(label(definition[field]),'CASE_METADATA',id+' '+field);
    for(const field of ['actions','evidence'])need(Array.isArray(definition[field])&&definition[field].length>0&&
      definition[field].length<=128&&definition[field].every(label),'CASE_METADATA',id+' '+field);
    need(['Critical','High','Normal','Low'].includes(definition.importance)&&typeof definition.needsGameWindow==='boolean',
      'CASE_METADATA',id+' importance/window');
    need(stable(definition.group)&&typeof definition.runGroup==='string'&&/^G[0-9A-Za-z]*(?:-[a-z0-9]+)*$/.test(definition.runGroup)&&
      definition.runGroup.length<=128&&Number.isSafeInteger(definition.order),'CASE_METADATA',id+' group/order');
    need(Array.isArray(definition.requirements)&&definition.requirements.length>0&&definition.requirements.length<=5&&
      new Set(definition.requirements).size===definition.requirements.length&&definition.requirements.every(x=>/^ASSET-0[1-5]$/.test(x)),
      'CASE_METADATA',id+' requirements');
    need(Array.isArray(definition.acceptance)&&definition.acceptance.length<=24&&
      new Set(definition.acceptance).size===definition.acceptance.length&&definition.acceptance.every(x=>/^AC-(0[1-9]|1[0-9]|2[0-4])$/.test(x)),
      'CASE_METADATA',id+' acceptance');
    const position=JSON.stringify([definition.runGroup,definition.order]);need(!positions.has(position),'CASE_ORDER_DUPLICATE',id+' '+position);
    const item=structuredClone(definition);cases.set(id,{...item,id,run});positions.add(position);
  }
  return {cases,registerCase};
}
function canonicalDirectory(directory){
  need(typeof directory==='string'&&path.isAbsolute(directory),'CASE_DIRECTORY_INVALID',String(directory));
  const absolute=path.resolve(directory);let current=path.parse(absolute).root;
  for(const segment of path.relative(current,absolute).split(path.sep)){
    need(fs.existsSync(current),'CASE_DIRECTORY_MISSING',directory);
    need(!fs.lstatSync(current).isSymbolicLink(),'CASE_PATH_REPARSE',current);
    need(fs.readdirSync(current).includes(segment),'CASE_DIRECTORY_MISSING',directory);
    current=path.join(current,segment);need(fs.existsSync(current),'CASE_DIRECTORY_MISSING',directory);
    need(!fs.lstatSync(current).isSymbolicLink(),'CASE_PATH_REPARSE',current);
  }
  need(fs.statSync(absolute).isDirectory(),'CASE_DIRECTORY_INVALID',directory);return absolute;
}
function loadCaseModules(directory,registry,loadModule=(file)=>require(file)){
  const root=canonicalDirectory(directory);
  need(registry?.cases instanceof Map&&typeof registry.registerCase==='function'&&typeof loadModule==='function','CASE_REGISTRY_INVALID');
  const names=fs.readdirSync(root);need(names.length>0,'CASE_MODULES_EMPTY',root);need(names.length<=128,'CASE_MODULE_LIMIT',root);
  const folded=new Set(),identities=new Set();
  const descriptors=names.sort(ordinal).map(name=>{
    need(/^[a-z][a-z0-9]*(?:-[a-z0-9]+)*\.cjs$/.test(name),'CASE_MODULE_NAME',name);
    need(!folded.has(name.toLowerCase()),'CASE_MODULE_CONFLICT',name);folded.add(name.toLowerCase());
    const file=path.join(root,name),stat=fs.lstatSync(file);
    need(!stat.isSymbolicLink()&&stat.isFile()&&stat.nlink===1&&stat.size>0&&stat.size<=1048576,'CASE_MODULE_FILE',file);
    const identity=stat.dev+':'+stat.ino;need(!identities.has(identity),'CASE_MODULE_CONFLICT',file);identities.add(identity);
    const value=fs.readFileSync(file);need(value.length===stat.size,'CASE_MODULE_CHANGED',file);
    return {name,path:file,bytes:value.length,sha256:crypto.createHash('sha256').update(value).digest('hex'),caseIds:[]};
  });
  for(const descriptor of descriptors){
    const before=new Map(registry.cases);
    try{loadModule(descriptor.path,registry);}catch(error){throw new Error('CASE_MODULE_LOAD '+descriptor.path+' '+error.message,{cause:error});}
    need([...before].every(([id,item])=>registry.cases.get(id)===item),'CASE_REGISTRY_CHANGED',descriptor.path);
    descriptor.caseIds=[...registry.cases.keys()].filter(id=>!before.has(id)).sort(ordinal);
    need(descriptor.caseIds.length>0,'CASE_MODULE_EMPTY',descriptor.path);
    const after=fs.lstatSync(descriptor.path),value=fs.readFileSync(descriptor.path);
    need(!after.isSymbolicLink()&&after.isFile()&&after.nlink===1&&value.length===descriptor.bytes&&
      crypto.createHash('sha256').update(value).digest('hex')===descriptor.sha256,'CASE_MODULE_CHANGED',descriptor.path);
  }
  return descriptors;
}
module.exports={createCaseRegistry,loadCaseModules};
