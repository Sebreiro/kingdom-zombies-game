"""Bound audio native decoder per subprocess, retaining independent failures."""
import io
import json
import pathlib
import subprocess
import sys
import wave

ROOT=pathlib.Path(__file__).resolve().parent
if len(sys.argv)>1:
    sys.path.insert(0,'D:/Work/Games/KindomCrown/.tools/resource-python')
    import UnityPy
    owner=sys.argv[1]
    path_id=int(sys.argv[2])
    env=UnityPy.load('D:/Work/Games/KindomCrown/recovered/resources/KingdomTwoCrowns_Data/'+owner)
    obj=next(o for o in env.objects if pathlib.Path(o.assets_file.name).name==owner and o.path_id==path_id)
    samples=obj.read().samples
    result=[]
    for name,payload in samples.items():
        with wave.open(io.BytesIO(payload)) as wav:
            result.append({'name':name,'bytes':len(payload),'frames':wav.getnframes(),'rate':wav.getframerate(),
                           'channels':wav.getnchannels(),'sampleWidth':wav.getsampwidth(),
                           'seconds':wav.getnframes()/wav.getframerate()})
    print(json.dumps(result))
else:
    result=[]
    for owner,path_id in [('resources.assets',3288),('resources.assets',5272),('resources.assets',4441),
                           ('sharedassets0.assets',449),('sharedassets2.assets',125)]:
        child=subprocess.run([sys.executable,'-B',__file__,owner,str(path_id)],capture_output=True,text=True,timeout=40)
        result.append({'owner':owner,'id':path_id,'exit':child.returncode,'stdout':child.stdout,'stderr':child.stderr})
    (ROOT/'audio-isolation-probe.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
    print(json.dumps(result,indent=2))
