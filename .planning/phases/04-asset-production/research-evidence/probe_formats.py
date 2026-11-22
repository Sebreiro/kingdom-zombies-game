"""Probe a constrained PCM16 FSB5 unwrap and runtime streamed curve decoding."""
import array
import hashlib
import io
import json
import math
import pathlib
import struct
import sys
import wave

ROOT=pathlib.Path(__file__).resolve().parent
DONOR=pathlib.Path('D:/Work/Games/KindomCrown')
sys.path.insert(0,str(DONOR/'.tools/resource-python'))
import UnityPy

result={'scope':'Format feasibility only; no listening or production acceptance.','pcm':[],'curves':[]}
env=UnityPy.load(str(DONOR/'recovered/resources/KingdomTwoCrowns_Data/resources.assets'))
owners={pathlib.Path(o.assets_file.name).name:o.assets_file for o in env.objects}
for name,path_id in [('resources.assets',3288),('resources.assets',5272),('sharedassets0.assets',449)]:
    raw=(ROOT/f'{name}-{path_id}.fsb').read_bytes()
    version,count,headers,names,size,codec=struct.unpack_from('<6I',raw,4)
    assert version==1 and count==1 and codec==2
    assert len(raw)==60+headers+names+size
    mode=struct.unpack_from('<Q',raw,60)[0]
    channels=[1,2,6,8][(mode>>5)&3]
    rate=[4000,8000,11000,11025,16000,22050,24000,32000,44100,48000,96000][(mode>>1)&15]
    frames=(mode>>34)&0x3fffffff
    offset=60+headers+names+(((mode>>7)&0x7ffffff)<<5)
    pcm=raw[offset:offset+frames*channels*2]
    assert len(pcm)==frames*channels*2
    with wave.open(str(ROOT/f'{name}-{path_id}-pcm.wav'),'wb') as wav:
        wav.setnchannels(channels)
        wav.setsampwidth(2)
        wav.setframerate(rate)
        wav.writeframes(pcm)
    values=array.array('h',pcm)
    result['pcm'].append({'owner':name,'id':path_id,'codec':codec,'frames':frames,'channels':channels,'rate':rate,
                          'seconds':frames/rate,'peak':max(abs(x) for x in values),'pcmSha256':hashlib.sha256(pcm).hexdigest()})
for clip_id in [1214,783,1245,2568,2573,2298,2193]:
    obj=owners['resources.assets'].objects[clip_id]
    tree=obj.read_typetree()
    muscle=tree['m_MuscleClip']
    clip=muscle['m_Clip']['data']
    words=clip['m_StreamedClip']['data']
    raw=struct.pack('<'+'I'*len(words),*words)
    cursor=0
    frames=[]
    while cursor<len(raw):
        time,count=struct.unpack_from('<fI',raw,cursor)
        cursor+=8
        keys=[]
        for _ in range(count):
            index,*coeff=struct.unpack_from('<i4f',raw,cursor)
            cursor+=20
            keys.append({'curve':index,'value':coeff[-1]})
        frames.append({'time':time if math.isfinite(time) else 'positive-infinity-sentinel','keys':keys})
    mapping=[]
    for pointer in obj.read().m_ClipBindingConstant.pptrCurveMapping:
        ref=pointer.read()
        mapping.append({'file':pathlib.Path(ref.object_reader.assets_file.name).name,'id':ref.object_reader.path_id,
                        'type':ref.object_reader.type.name,'name':getattr(ref,'m_Name',None)})
    result['curves'].append({'id':clip_id,'name':tree['m_Name'],'sampleRate':tree['m_SampleRate'],
                             'start':muscle['m_StartTime'],'stop':muscle['m_StopTime'],'loop':muscle['m_LoopTime'],
                             'frames':frames,'mapping':mapping,'events':tree['m_Events'],
                             'genericBindings':tree['m_ClipBindingConstant']['genericBindings'],
                             'dense':clip['m_DenseClip'],'constant':clip['m_ConstantClip']})
(ROOT/'format-probe.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps({'pcm':result['pcm'],'clips':[{'id':x['id'],'name':x['name'],'start':x['start'],'stop':x['stop'],
      'loop':x['loop'],'mapping':len(x['mapping']),'streamFrames':len(x['frames'])} for x in result['curves']]},indent=2))
