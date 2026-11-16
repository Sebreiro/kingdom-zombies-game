'use strict';
const fs=require('node:fs');
const {decodePng}=require('../gameplay-checks/rendered.cjs');
function crc(bytes){let value=0xffffffff;for(const byte of bytes){value^=byte;for(let bit=0;bit<8;bit++)value=(value>>>1)^((value&1)?0xedb88320:0);}return (value^0xffffffff)>>>0;}
function validate(file){
  const bytes=fs.readFileSync(file);let offset=8,header=0,data=0,end=0;
  while(offset+12<=bytes.length){
    const size=bytes.readUInt32BE(offset),kind=bytes.toString('ascii',offset+4,offset+8);
    if(offset+size+12>bytes.length)throw new Error('PNG_TRUNCATED');
    if(crc(bytes.subarray(offset+4,offset+8+size))!==bytes.readUInt32BE(offset+8+size))throw new Error('PNG_CRC');
    if(kind==='IHDR'){if(offset!==8||size!==13||++header!==1)throw new Error('PNG_HEADER');}
    if(kind==='IDAT')data++;
    offset+=size+12;
    if(kind==='IEND'){if(size!==0||offset!==bytes.length)throw new Error('PNG_TRAILING');end++;break;}
  }
  if(header!==1||data===0||end!==1)throw new Error('PNG_STRUCTURE');return decodePng(file,1280,720);
}
module.exports={crc,validate};
