'use strict';
const net=require('node:net'),path=require('node:path');
const {need,write}=require('./content.cjs');
const MAX=1048576;
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
  const rows=[];for(let i=0;i<data.length;i+=6)rows.push({children:data[i],name:data[i+1],type:data[i+2],id:data[i+3],scene:data[i+4],flags:data[i+5]});
  need(rows.every(x=>Number.isSafeInteger(x.children)&&x.children>=0&&typeof x.name==='string'&&typeof x.type==='string'&&Number.isSafeInteger(x.id)&&x.id>0),'DEBUG_TREE_FIELDS');return rows;
}
function validate(record,pid){
  need(record.reportedPid===pid&&Number.isSafeInteger(pid),'DEBUG_PID_MISMATCH');
  const nodes=tree(record.tree),inspection=nodes.find(x=>x.name==='Inspection');
  need(nodes.length===9&&new Set(nodes.map(x=>x.id)).size===9&&
    nodes.map(x=>x.name).join(',')==='root,Inspection,Bow,Summary,Title,ResourceId,ResourceName,Status,PackStatus'&&
    nodes.map(x=>x.children).join(',')==='1,2,0,5,0,0,0,0,0'&&
    inspection&&inspection.type==='Node2D'&&inspection.scene==='res://inspection/Inspection.tscn'&&
    nodes[0].name==='root'&&nodes[0].children===1&&nodes.every(x=>x.name!=='Foundation'),'PACKAGED_INSPECTION_ROOT');
  function object(name){const node=nodes.find(x=>x.name===name),value=record.objects.find(x=>x[0]===node?.id);
    need(value&&value.length===3&&value[1]===node.type&&Array.isArray(value[2])&&value[2].every(x=>Array.isArray(x)&&x.length===6),'DEBUG_OBJECT_MISSING',name);
    return Object.fromEntries(value[2].map(x=>[x[0],x[5]]));}
  const bow=object('Bow'),id=object('ResourceId'),status=object('Status');
  need(bow.texture==='res://assets/common/bow.png'&&bow.centered===false&&bow.offset?.values[0]===-6&&bow.offset?.values[1]===-9,
    'PACKAGED_NATIVE_TEXTURE');
  need(id.text==='common.bow'&&status.text==='Source/headless verified; visual review pending','PACKAGED_NATIVE_LABELS');
  return {pid,root:'Inspection',scene:inspection.scene,instanceId:inspection.id,texture:bow.texture,resourceId:id.text,
    nodes:nodes.length,scope:'actual natural exported headless startup; read-only native object observations; no rendered credit'};
}
async function observe(context,name,executable,env){
  const runner=require('./check.cjs'),packets=[],sent=[],objects=[],record={reportedPid:null,tree:null,objects};
  let socket,buffer=Buffer.alloc(0),failure,received=0,requested=false,timer;
  const server=net.createServer(peer=>{
    if(socket||peer.remoteAddress!=='127.0.0.1'){peer.destroy();failure='DEBUG_PEER';return;}socket=peer;
    function send(command,args){
      need(['scene:request_scene_tree','scene:inspect_objects'].includes(command),'DEBUG_WRITE_FORBIDDEN');
      const body=encode([command,record.threadId,args]);sent.push({command,threadId:record.threadId,args});peer.write(Buffer.concat([u32(body.length),body]));
    }
    timer=setInterval(()=>{if(record.reportedPid&&!record.tree)send('scene:request_scene_tree',[]);},150);
    peer.on('error',error=>{failure??=error.message;});
    peer.on('data',part=>{try{
      received+=part.length;need(received<=8*MAX,'DEBUG_TOTAL_LIMIT');buffer=Buffer.concat([buffer,part]);need(buffer.length<=2*MAX,'DEBUG_BUFFER_LIMIT');
      while(buffer.length>=4){const length=buffer.readUInt32LE(0);need(length>0&&length<=MAX,'DEBUG_PACKET_SIZE');if(buffer.length<length+4)break;
        const raw=buffer.subarray(4,length+4);buffer=buffer.subarray(length+4);need(packets.length<512,'DEBUG_MESSAGE_LIMIT');
        write(context.directory,name+'-packet-'+String(packets.length).padStart(3,'0')+'.bin',raw);
        const message=decode(raw);packets.push(message);
        need(Array.isArray(message)&&message.length===3&&typeof message[0]==='string','DEBUG_MESSAGE');
        const [command,,data]=message;
        if(command==='set_pid'){record.reportedPid=data[0];record.threadId=message[1];}
        if(command==='scene:scene_tree'){
          const nodes=tree(data);if(nodes.some(x=>x.name==='Inspection')){
            record.tree=data;
            if(!requested){requested=true;const ids=['Inspection','Bow','ResourceId','Status'].map(name=>nodes.find(x=>x.name===name)?.id);need(ids.every(Number.isSafeInteger),'DEBUG_REQUIRED_NODES');for(const id of ids)send('scene:inspect_objects',[[id],false]);}
          }
        }
        if(command==='scene:inspect_objects')for(const obj of data)if(!objects.some(x=>x[0]===obj[0]))objects.push(obj);
      }
    }catch(error){failure??=error.message;peer.destroy();}});
  });
  let command,error;
  try{
    await new Promise((resolve,reject)=>{server.once('error',reject);server.listen(0,'127.0.0.1',resolve);});
    const address=server.address();write(context.directory,name+'-endpoint.json',{host:'127.0.0.1',port:address.port,scope:'owned loopback read-only debugger'});
    command=await runner.command(context,name,executable,['--headless','--remote-debug','tcp://127.0.0.1:'+address.port,'--quit-after','180'],
      {timeout:15000,cwd:path.dirname(executable),env});
  }catch(caught){error=caught;}
  finally{clearInterval(timer);socket?.destroy();await new Promise(resolve=>server.close(resolve));
    write(context.directory,name+'-debugger.json',{...record,sent,packets,failure,error:error?.message});}
  if(error)throw error;need(!failure&&buffer.length===0,'DEBUG_PROTOCOL',failure);
  need(!packets.some(x=>['error','debug_enter'].includes(x[0])),'DEBUG_ENGINE_ERROR');
  need(!/ERROR:|SCRIPT ERROR:|Failed to load/i.test(command.stdout+command.stderr),'PACKAGED_STARTUP_ERROR');
  const result=validate(record,command.pid);write(context.directory,name+'-observation.json',result);return result;
}
function diagnostics(record,pid){
  const outcomes=[];
  for(const [name,code,action]of [
    ['truncated','DEBUG_TRUNCATED',()=>decode(Buffer.from([28,0,0,0]))],
    ['unsupported','DEBUG_VARIANT_UNSUPPORTED',()=>decode(u32(255))],
    ['forged-pid','DEBUG_PID_MISMATCH',()=>validate(record,pid+1)],
    ['missing-tree','DEBUG_TREE_SHAPE',()=>validate({...record,tree:[]},pid)],
    ['wrong-root','PACKAGED_INSPECTION_ROOT',()=>{const copy=structuredClone(record);copy.tree[7]='Foundation';validate(copy,pid);}],
    ['missing-properties','DEBUG_OBJECT_MISSING',()=>validate({...record,objects:[]},pid)]
  ]){let error;try{action();}catch(e){error=e.message;}need(error?.startsWith(code),'DEBUG_NEGATIVE_CONTROL',name+' '+error);outcomes.push({name,expected:code,observed:error});}
  return outcomes;
}
module.exports={encode,decode,tree,validate,observe,diagnostics};
