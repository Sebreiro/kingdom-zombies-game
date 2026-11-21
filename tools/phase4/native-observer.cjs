'use strict';
const net=require('node:net'),path=require('node:path'),crypto=require('node:crypto');
const {isDeepStrictEqual:equal}=require('node:util');
const {need,write}=require('./content.cjs');
const MAX=1048576;
const HUB='/root/Inspection',SUMMARY=HUB+'/Summary',TOOLBAR=HUB+'/Shell/Toolbar';
const WORKSPACE=HUB+'/Shell/Workspace',NAVIGATION=WORKSPACE+'/Navigation';
const BROWSER=WORKSPACE+'/Controls/Browser',PREVIEW=WORKSPACE+'/Preview';
const SCRIPT='res://inspection/InspectionRoot.cs',SCENE='res://inspection/Inspection.tscn';
const READY='Validated; visual/listening acceptance pending';
const ROLES=new Map([
  ['/root','Window'],[HUB,SCRIPT],[HUB+'/Background','ColorRect'],[HUB+'/Bow','Sprite2D'],
  [SUMMARY,'VBoxContainer'],...['Title','ResourceId','ResourceName','Status','PackStatus'].map(x=>[SUMMARY+'/'+x,'Label']),
  [HUB+'/Shell','VBoxContainer'],[TOOLBAR,'HBoxContainer'],[TOOLBAR+'/Recheck','Button'],
  [TOOLBAR+'/Cancel','Button'],[TOOLBAR+'/Operation','Label'],[WORKSPACE,'BoxContainer'],
  [NAVIGATION,'VBoxContainer'],[NAVIGATION+'/Category','OptionButton'],[NAVIGATION+'/Scope','Label'],
  [WORKSPACE+'/Controls','ScrollContainer'],[BROWSER,'VBoxContainer'],[BROWSER+'/Search','LineEdit'],
  [BROWSER+'/Status','OptionButton'],[BROWSER+'/Count','Label'],[BROWSER+'/Resources','ItemList'],
  [PREVIEW,'VBoxContainer'],[PREVIEW+'/Title','Label'],[PREVIEW+'/Surface','PanelContainer'],
  [PREVIEW+'/Surface/Current','Node2D'],[PREVIEW+'/Metadata','TextEdit'],[PREVIEW+'/Failures','TextEdit'],
  [PREVIEW+'/Actions','HBoxContainer'],[PREVIEW+'/Actions/Details','Button'],[PREVIEW+'/Actions/Copy','Button'],
  [PREVIEW+'/Transport','HFlowContainer'],[PREVIEW+'/Transport/Play','Button'],
  [PREVIEW+'/Transport/Step','Button'],[PREVIEW+'/Transport/Reset','Button'],[PREVIEW+'/Transport/Speed','OptionButton']
]);
const INSPECT=[HUB,HUB+'/Bow',...['ResourceId','ResourceName','Status','PackStatus'].map(x=>SUMMARY+'/'+x),
  TOOLBAR+'/Cancel',NAVIGATION+'/Category',BROWSER+'/Status',BROWSER+'/Count',BROWSER+'/Resources',
  PREVIEW+'/Metadata',PREVIEW+'/Failures'];
function u32(value){const out=Buffer.alloc(4);out.writeUInt32LE(value);return out;}
function encode(value){
  if(typeof value==='string'){const data=Buffer.from(value),pad=Buffer.alloc((4-data.length%4)%4);return Buffer.concat([u32(4),u32(data.length),data,pad]);}
  if(typeof value==='boolean')return Buffer.concat([u32(1),u32(Number(value))]);
  if(Number.isSafeInteger(value)){const b=Buffer.alloc(8);b.writeBigInt64LE(BigInt(value));return Buffer.concat([u32(65538),b]);}
  if(Array.isArray(value)){need(value.length<4096,'DEBUG_ENCODE_COUNT');return Buffer.concat([u32(28),u32(value.length),...value.map(encode)]);}
  throw new Error('DEBUG_ENCODE_TYPE');
}
function decode(buffer){
  let position=0,count=0;
  function take(size){need(Number.isSafeInteger(size)&&size>=0&&position+size<=buffer.length,'DEBUG_TRUNCATED');const value=buffer.subarray(position,position+size);position+=size;return value;}
  const number=()=>take(4).readUInt32LE(0);
  function string(){const length=number();need(length<=65536,'DEBUG_STRING_LIMIT');const text=new TextDecoder('utf-8',{fatal:true}).decode(take(length));take((4-length%4)%4);return text;}
  function read(depth=0){
    need(depth<=24&&++count<=100000,'DEBUG_VARIANT_LIMIT');const header=number(),type=header&255,wide=Boolean(header&65536);
    if(type===0)return null;
    if(type===1)return number()!==0;
    if(type===2){const value=wide?Number(take(8).readBigInt64LE(0)):take(4).readInt32LE(0);need(Number.isSafeInteger(value),'DEBUG_INTEGER');return value;}
    if(type===3){const value=wide?take(8).readDoubleLE(0):take(4).readFloatLE(0);need(Number.isFinite(value),'DEBUG_FLOAT');return value;}
    if(type===4||type===21)return string();
    const dimensions={5:2,6:2,7:4,8:4,9:3,10:3,11:6,12:4,13:4,14:4,15:4,16:6,17:9,18:12,19:16,20:4};
    if(dimensions[type]){
      const integer=[6,8,10,13].includes(type),values=[];
      for(let i=0;i<dimensions[type];i++)values.push(integer?take(4).readInt32LE(0):(wide&&type!==20?take(8).readDoubleLE(0):take(4).readFloatLE(0)));
      need(values.every(Number.isFinite),'DEBUG_FLOAT');return {type,values};
    }
    if(type===22){const names=number();need(names&2147483648,'DEBUG_NODEPATH_FORMAT');const length=names&2147483647,sub=number(),flags=number();need(length+sub<256,'DEBUG_NODEPATH_COUNT');const values=Array.from({length:length+sub},string);return {type,path:(flags&1?'/':'')+values.slice(0,length).join('/')+(sub?':'+values.slice(length).join(':'):'')};}
    if(type===23||type===24){need(type!==24||wide,'DEBUG_OBJECT_FORBIDDEN');return {type,id:take(8).readBigUInt64LE(0).toString()};}
    if(type===25)return {type};
    if(type===27||type===28){
      const containerType=kind=>{if(kind===1)need(number()<39,'DEBUG_CONTAINER_TYPE');else if(kind===2||kind===3)string();};
      containerType((header>>>16)&3);if(type===27)containerType((header>>>18)&3);
      const length=number()&2147483647;need(length<=8192,'DEBUG_CONTAINER_COUNT');
      if(type===28)return Array.from({length},()=>read(depth+1));
      return {type,entries:Array.from({length},()=>[read(depth+1),read(depth+1)])};
    }
    if(type>=29&&type<=38){
      const length=number();need(length<=65536,'DEBUG_PACKED_COUNT');
      if(type===29){const value=take(length).toString('base64');take((4-length%4)%4);return {type,length,base64:value};}
      if(type===34)return {type,values:Array.from({length},string)};
      const dimensions2={30:1,31:1,32:1,33:1,35:2,36:3,37:4,38:4},values=[];
      for(let i=0;i<length*dimensions2[type];i++)values.push(type===30?take(4).readInt32LE(0):type===31?Number(take(8).readBigInt64LE(0)):type===33?take(8).readDoubleLE(0):take(4).readFloatLE(0));
      need(values.every(Number.isFinite),'DEBUG_PACKED_NUMBER');return {type,values};
    }
    throw new Error('DEBUG_VARIANT_UNSUPPORTED '+type+' '+header);
  }
  const result=read();need(position===buffer.length,'DEBUG_TRAILING_BYTES');return result;
}
function tree(data){
  need(Array.isArray(data)&&data.length>0&&data.length%6===0&&data.length<=1536,'DEBUG_TREE_SHAPE');
  const rows=new Map(),ids=new Set(),ancestors=[];
  for(let i=0;i<data.length;i+=6){
    const [children,name,type,id,scene,flags]=data.slice(i,i+6);
    need(Number.isSafeInteger(children)&&children>=0&&children<=(data.length-i-6)/6&&
      typeof name==='string'&&name.length<=128&&/^[A-Za-z0-9_@-]+$/.test(name)&&
      typeof type==='string'&&type.length>0&&type.length<=128&&Number.isSafeInteger(id)&&id>0&&
      typeof scene==='string'&&scene.length<=220&&Number.isInteger(flags)&&flags>=0,'DEBUG_TREE_FIELDS',String(i/6));
    while(ancestors.at(-1)?.left===0)ancestors.pop();
    need((i===0||ancestors.length>0)&&ancestors.length<24,'DEBUG_TREE_CHILDREN',name);
    const parent=ancestors.at(-1);
    const full=(parent?.full??'')+'/'+name;
    if(parent)parent.left--;
    need(!ids.has(id)&&!rows.has(full),'DEBUG_TREE_DUPLICATE',full);
    rows.set(full,{children,name,type,id,scene,flags});ids.add(id);
    ancestors.push({full,left:children});
  }
  need(ancestors.every(x=>x.left===0),'DEBUG_TREE_CHILDREN');
  return rows;
}
function hub(data){
  const nodes=tree(data),root=nodes.get('/root'),inspection=nodes.get(HUB);
  need(root?.children===1&&inspection?.scene===SCENE,'PACKAGED_INSPECTION_ROOT');
  for(const [full,type] of ROLES)need(nodes.get(full)?.type===type,'DEBUG_REQUIRED_NODE',full+' '+type);
  return nodes;
}
function properties(value,node,full){
  need(Array.isArray(value)&&value.length===3&&value[0]===node.id&&value[1]===(full===HUB?'Node2D':node.type)&&
    Array.isArray(value[2])&&value[2].length<=1024&&value[2].every(x=>Array.isArray(x)&&x.length===6&&typeof x[0]==='string'),
    'DEBUG_OBJECT_MISSING',full);
  const result=new Map(value[2].map(x=>[x[0],x[5]]));
  need(result.size===value[2].length,'DEBUG_PROPERTY_DUPLICATE',full);
  return Object.fromEntries(result);
}
function validate(record,launch,contract){
  need(launch&&record.nonce===launch.nonce&&record.launchId===launch.launchId&&
    record.packageSha256===launch.packageSha256&&record.executable===launch.executable,'DEBUG_LAUNCH_IDENTITY');
  const {pid}=launch;
  need(record.reportedPid===pid&&Number.isSafeInteger(pid)&&pid>0,'DEBUG_PID_MISMATCH');
  const nodes=hub(record.tree),inspection=nodes.get(HUB);
  const expectedObjects=[TOOLBAR+'/Operation',...INSPECT];
  need(Array.isArray(record.objects)&&record.objects.length===expectedObjects.length&&
    new Set(record.objects.map(x=>x[0])).size===record.objects.length,'DEBUG_OBJECT_SET');
  const observed=Object.fromEntries(expectedObjects.map(full=>{
    const node=nodes.get(full);
    return [full,properties(record.objects.find(x=>x[0]===node.id),node,full)];
  }));
  const bow=observed[HUB+'/Bow'],op=observed[TOOLBAR+'/Operation'];
  need(observed[HUB].script===SCRIPT&&op.text===READY&&op.visible===true&&
    observed[TOOLBAR+'/Cancel'].disabled===true,'PACKAGED_NATIVE_READY');
  need(bow.texture===contract.texture&&bow.centered===false&&bow.texture_filter===1&&
    equal(bow.offset,{type:5,values:[-6,-9]})&&equal(bow.scale,{type:5,values:[1,1]})&&
    bow.visible===contract.found,'PACKAGED_NATIVE_TEXTURE');
  const label=role=>observed[SUMMARY+'/'+role];
  need(label('ResourceId').text===contract.resourceId&&label('ResourceName').text===contract.resourceName&&
    label('Status').text===contract.status&&['ResourceId','ResourceName','Status','PackStatus'].every(role=>label(role).visible===true),
    'PACKAGED_NATIVE_LABELS',JSON.stringify(['ResourceId','ResourceName','Status'].map(x=>label(x).text)));
  need(label('PackStatus').text===contract.packStatus,'PACKAGED_NATIVE_REGISTER',label('PackStatus').text);
  const category=observed[NAVIGATION+'/Category'],filter=observed[BROWSER+'/Status'];
  function options(value,field,items,selected){
    need(value.visible===true&&value.selected===selected&&items.every((text,index)=>value['popup/item_'+index+'/text']===text)&&
      Object.keys(value).filter(key=>/^popup\/item_\d+\/text$/.test(key)).length===items.length,field);
  }
  options(category,'PACKAGED_NATIVE_CATEGORY',contract.categories,1);
  options(filter,'PACKAGED_NATIVE_FILTER',contract.filters,0);
  const resources=observed[BROWSER+'/Resources'],count=observed[BROWSER+'/Count'];
  const rows=Object.entries(resources).filter(([key])=>/^item_\d+\/text$/.test(key))
    .sort(([a],[b])=>Number(a.slice(5,-5))-Number(b.slice(5,-5))).map(([,value])=>value);
  need(resources.visible===true&&count.text===contract.items.length+' results'&&equal(rows,contract.items),
    'PACKAGED_NATIVE_ITEM_REGISTER',JSON.stringify({count:count.text,rows}));
  const metadata=observed[PREVIEW+'/Metadata'],failures=observed[PREVIEW+'/Failures'];
  need(metadata.visible===false&&metadata.text===contract.metadata&&failures.visible===true&&
    failures.text===contract.diagnostics,'PACKAGED_NATIVE_DIAGNOSTICS',String(failures.text).slice(0,4096));
  return {pid,launchId:launch.launchId,nonce:launch.nonce,packageSha256:launch.packageSha256,root:inspection.name,
    scene:inspection.scene,instanceId:inspection.id,texture:bow.texture,textureVisible:bow.visible,
    resourceId:label('ResourceId').text,resourceName:label('ResourceName').text,status:label('Status').text,
    packStatus:label('PackStatus').text,items:rows,categories:contract.categories,diagnostics:failures.text,
    nodes:nodes.size,roles:[...ROLES.keys()],
    scope:'Natural exported headless startup and clean exit using read-only engine observations'};
}
async function observe(context,name,executable,env,contract,packageSha256){
  const runner=require('./check.cjs'),packets=[],sent=[],objects=[];
  const launch={nonce:context.nonce,launchId:crypto.randomUUID(),packageSha256,executable};
  const record={...launch,reportedPid:null,tree:null,objects};
  let socket,buffer=Buffer.alloc(0),failure,received=0,pending,timer;
  const server=net.createServer(peer=>{
    if(socket||peer.remoteAddress!=='127.0.0.1'){peer.destroy();failure='DEBUG_PEER';return;}socket=peer;
    function send(command,args){
      need(['scene:request_scene_tree','scene:inspect_objects'].includes(command),'DEBUG_WRITE_FORBIDDEN');
      const body=encode([command,record.threadId,args]);sent.push({command,threadId:record.threadId,args});peer.write(Buffer.concat([u32(body.length),body]));
    }
    function request(command,args,response){
      need(!pending,'DEBUG_OVERLAPPING_REQUEST');
      pending=response;
      send(command,args);
    }
    function inspect(nodes,data,paths){
      // The engine's default selection limit is one, even when selection updates are disabled.
      request('scene:inspect_objects',[[nodes.get(paths[0]).id],false],{command:'scene:inspect_objects',nodes,data,paths});
    }
    timer=setInterval(()=>{
      if(record.reportedPid&&!record.tree&&!pending&&!peer.destroyed)
        request('scene:request_scene_tree',[],{command:'scene:scene_tree'});
    },150);
    peer.on('error',error=>{failure??=error.message;});
    peer.on('data',part=>{try{
      received+=part.length;need(received<=8*MAX,'DEBUG_TOTAL_LIMIT');buffer=Buffer.concat([buffer,part]);need(buffer.length<=2*MAX,'DEBUG_BUFFER_LIMIT');
      while(buffer.length>=4){const length=buffer.readUInt32LE(0);need(length>0&&length<=MAX,'DEBUG_PACKET_SIZE');if(buffer.length<length+4)break;
        const raw=buffer.subarray(4,length+4);buffer=buffer.subarray(length+4);need(packets.length<512,'DEBUG_MESSAGE_LIMIT');
        write(context.directory,name+'-packet-'+String(packets.length).padStart(3,'0')+'.bin',raw);
        const message=decode(raw);packets.push(message);
        need(Array.isArray(message)&&message.length===3&&typeof message[0]==='string','DEBUG_MESSAGE');
        const [command,threadId,data]=message;
        if(command==='set_pid'){
          need(record.reportedPid===null&&Array.isArray(data)&&data.length===1&&Number.isSafeInteger(data[0])&&
            data[0]>0&&Number.isSafeInteger(threadId)&&threadId>0,'DEBUG_PID_PACKET');
          record.reportedPid=data[0];record.threadId=threadId;
        }
        if(command!=='scene:scene_tree'&&command!=='scene:inspect_objects')continue;
        // Godot may publish an unsolicited scene tree while the main scene is still loading.
        if(command==='scene:scene_tree'&&pending?.command!==command)continue;
        need(pending?.command===command&&threadId===record.threadId,'DEBUG_UNSOLICITED_RESPONSE',command);
        const response=pending;pending=undefined;
        if(command==='scene:scene_tree'){
          const nodes=tree(data);
          if(!nodes.has(HUB))continue;
          const full=TOOLBAR+'/Operation';
          need(nodes.get(full)?.type==='Label','DEBUG_REQUIRED_NODE',full);
          inspect(nodes,data,[full]);
          continue;
        }
        need(Array.isArray(data)&&data.length===1,'DEBUG_OBJECT_SET');
        const [full,...remaining]=response.paths;
        const object=properties(data[0],response.nodes.get(full),full);
        if(full===TOOLBAR+'/Operation'){
          const op=object;
          if(op.text!==READY)continue;
          hub(response.data);
          record.tree=response.data;objects.push(data[0]);
          inspect(response.nodes,response.data,INSPECT);
        }else{
          objects.push(data[0]);
          if(remaining.length)inspect(response.nodes,response.data,remaining);
        }
      }
    }catch(error){failure??=error.message;peer.destroy();}});
  });
  let command,error;
  try{
    await new Promise((resolve,reject)=>{server.once('error',reject);server.listen(0,'127.0.0.1',resolve);});
    const address=server.address();write(context.directory,name+'-endpoint.json',{host:'127.0.0.1',port:address.port,scope:'owned loopback read-only debugger'});
    command=await runner.command(context,name,executable,['--headless','--remote-debug','tcp://127.0.0.1:'+address.port,'--quit-after','360'],
      {timeout:15000,cwd:path.dirname(executable),env});
  }catch(caught){error=caught;}
  finally{clearInterval(timer);socket?.destroy();await new Promise(resolve=>server.close(resolve));
    write(context.directory,name+'-debugger.json',{...record,sent,packets,failure,error:error?.message});}
  if(error)throw error;need(!failure&&buffer.length===0,'DEBUG_PROTOCOL',failure);
  need(!packets.some(x=>['error','debug_enter'].includes(x[0])),'DEBUG_ENGINE_ERROR');
  need(!/ERROR:|SCRIPT ERROR:|Failed to load/i.test(command.stdout+command.stderr),'PACKAGED_STARTUP_ERROR');
  const result=validate(record,{...launch,pid:command.pid},contract);
  const controls=diagnostics(record,{...launch,pid:command.pid},contract);
  write(context.directory,name+'-observation.json',result);
  return {result,controls};
}
function diagnostics(record,launch,contract){
  const roles=hub(record.tree);
  const row=full=>[...roles.keys()].indexOf(full)*6;
  function altered(edit,changedLaunch=launch){
    const copy=structuredClone(record);edit(copy);return validate(copy,changedLaunch,contract);
  }
  function property(copy,full,key,value){
    copy.objects.find(object=>object[0]===roles.get(full).id)[2].find(x=>x[0]===key)[5]=value;
  }
  const outcomes=[];
  for(const [name,code,action]of [
    ['truncated','DEBUG_TRUNCATED',()=>decode(Buffer.from([28,0,0,0]))],
    ['unsupported','DEBUG_VARIANT_UNSUPPORTED',()=>decode(u32(255))],
    ['forged-pid','DEBUG_PID_MISMATCH',()=>altered(()=>{},{...launch,pid:launch.pid+1})],
    ['borrowed-launch','DEBUG_LAUNCH_IDENTITY',()=>altered(()=>{},{...launch,launchId:crypto.randomUUID()})],
    ['borrowed-package','DEBUG_LAUNCH_IDENTITY',()=>altered(copy=>{copy.packageSha256='0'.repeat(64);})],
    ['missing-tree','DEBUG_TREE_SHAPE',()=>altered(copy=>{copy.tree=[];})],
    ['malformed-tree','DEBUG_TREE_CHILDREN',()=>altered(copy=>{copy.tree[0]=0;})],
    ['old-tracer-tree','DEBUG_REQUIRED_NODE',()=>altered(copy=>{
      const original=copy.tree;
      copy.tree=['/root',HUB,HUB+'/Bow',SUMMARY,...['Title','ResourceId','ResourceName','Status','PackStatus']
        .map(name=>SUMMARY+'/'+name)].flatMap(full=>original.slice(row(full),row(full)+6));
      copy.tree[6]=2;copy.tree[8]='Node2D';copy.tree[18]=5;
    })],
    ['wrong-root','PACKAGED_INSPECTION_ROOT',()=>altered(copy=>{copy.tree[7]='Foundation';})],
    ['aliased-path','DEBUG_REQUIRED_NODE',()=>altered(copy=>{copy.tree[row(SUMMARY+'/ResourceId')+1]='ResourceID';})],
    ['wrong-type','DEBUG_REQUIRED_NODE',()=>altered(copy=>{copy.tree[row(NAVIGATION+'/Category')+2]='Button';})],
    ['duplicate-node-id','DEBUG_TREE_DUPLICATE',()=>altered(copy=>{copy.tree[row(SUMMARY+'/ResourceId')+3]=copy.tree[row(SUMMARY+'/Status')+3];})],
    ['duplicate-path','DEBUG_TREE_DUPLICATE',()=>altered(copy=>{copy.tree[row(SUMMARY+'/ResourceId')+1]='Status';})],
    ['missing-properties','DEBUG_OBJECT_SET',()=>altered(copy=>{copy.objects=[];})],
    ['wrong-resource','PACKAGED_NATIVE_LABELS',()=>altered(copy=>property(copy,SUMMARY+'/ResourceId','text','IT-07'))],
    ['wrong-name','PACKAGED_NATIVE_LABELS',()=>altered(copy=>property(copy,SUMMARY+'/ResourceName','text','Other bow'))],
    ['wrong-texture','PACKAGED_NATIVE_TEXTURE',()=>altered(copy=>property(copy,HUB+'/Bow','texture','res://assets/common/alias.png'))],
    ['wrong-status','PACKAGED_NATIVE_LABELS',()=>altered(copy=>property(copy,SUMMARY+'/Status','text','Accepted'))],
    ['wrong-register','PACKAGED_NATIVE_REGISTER',()=>altered(copy=>property(copy,SUMMARY+'/PackStatus','text','179/180 source-ready'))],
    ['wrong-category','PACKAGED_NATIVE_CATEGORY',()=>altered(copy=>property(copy,NAVIGATION+'/Category','popup/item_6/text','Unrelated'))],
    ['wrong-item-register','PACKAGED_NATIVE_ITEM_REGISTER',()=>altered(copy=>property(copy,BROWSER+'/Resources','item_0/text','IT-00'))]
  ]){let error;try{action();}catch(e){error=e.message;}need(error?.startsWith(code),'DEBUG_NEGATIVE_CONTROL',name+' '+error);outcomes.push({name,expected:code,observed:error});}
  return outcomes;
}
module.exports={encode,decode,tree,validate,observe,diagnostics};
