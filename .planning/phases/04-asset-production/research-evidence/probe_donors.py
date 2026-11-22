"""Read-only donor feasibility probes. Writes only this phase's evidence folder."""
import hashlib
import io
import json
import os
import pathlib
import sys
import wave

ROOT = pathlib.Path(__file__).resolve().parent
DONOR = pathlib.Path('D:/Work/Games/KindomCrown')
DATA = DONOR / 'recovered/resources/KingdomTwoCrowns_Data'
sys.path.insert(0, str(DONOR / '.tools/resource-python'))
import UnityPy
from PIL import Image

result = {'python': sys.version, 'UnityPy': UnityPy.__version__, 'Pillow': Image.__version__,
          'scope': 'Feasibility only. No production acceptance, rendered checks or audio listening.',
          'originals': [], 'sprites': [], 'fonts': [], 'audio': [], 'clips': [], 'errors': []}
preservation = json.loads((DONOR / 'recovered/resources/manifest.json').read_text())
for row in preservation['files']:
    source = DATA / row['path']
    with source.open('rb') as stream:
        actual = hashlib.file_digest(stream, 'sha256').hexdigest()
    result['originals'].append({'path': row['path'], 'bytes': source.stat().st_size,
                                'matches': actual == row['sha256'], 'sha256': actual})
env = UnityPy.load(*(str(DATA / x) for x in ['resources.assets', 'sharedassets0.assets', 'sharedassets2.assets']))
owners = {pathlib.Path(obj.assets_file.name).name: obj.assets_file for obj in env.objects}
for owner, path_id in [('resources.assets',9758), ('resources.assets',8711), ('sharedassets0.assets',2361),
                       ('resources.assets',8557), ('sharedassets0.assets',7837), ('sharedassets0.assets',7865)]:
    try:
        obj = owners[owner].objects[path_id]
        sprite = obj.read()
        image = sprite.image.convert('RGBA')
        output = ROOT / f'{owner}-{path_id}.png'
        image.save(output)
        tree = obj.read_typetree()
        (ROOT / f'{owner}-{path_id}-sprite.json').write_text(json.dumps(tree,indent=2,default=lambda x: {'binaryBytes':len(x)}), encoding='utf-8')
        result['sprites'].append({'owner':owner, 'id':path_id,'name':sprite.m_Name,'type':obj.type.name,
                                  'size':image.size,'bbox':image.getbbox(),'pixelsToUnits':sprite.m_PixelsToUnits,
                                  'rect':tree.get('m_Rect'),'pivot':tree.get('m_Pivot'),
                                  'textureRectOffset':tree.get('m_RD',{}).get('textureRectOffset'),
                                  'path': output.name})
    except Exception as error:
        result['errors'].append({'kind':'sprite','id':path_id,'error':repr(error)})
for owner, path_id in [('resources.assets',6003),('resources.assets',6004),('sharedassets0.assets',558),('sharedassets0.assets',564)]:
    try:
        font=owners[owner].objects[path_id].read()
        payload=bytes(font.m_FontData)
        result['fonts'].append({'owner':owner,'id':path_id,'name':font.m_Name,'bytes':len(payload),
                               'magic':payload[:4].hex(),'sha256':hashlib.sha256(payload).hexdigest()})
    except Exception as error:
        result['errors'].append({'kind':'font','id':path_id,'error':repr(error)})
for owner, path_id in [('resources.assets',3288),('resources.assets',5272),('resources.assets',4441),
                       ('sharedassets0.assets',449),('sharedassets2.assets',125)]:
    try:
        obj = owners[owner].objects[path_id]
        clip = obj.read()
        tree = obj.read_typetree()
        resource=tree.get('m_Resource')
        if resource:
            with (DATA / resource['m_Source']).open('rb') as stream:
                stream.seek(resource['m_Offset'])
                raw=stream.read(resource['m_Size'])
            (ROOT / f'{owner}-{path_id}.fsb').write_bytes(raw)
            print('AUDIO-RAW',owner,path_id,raw[:64].hex(),flush=True)
        samples=clip.samples
        decoded=[]
        for name, payload in samples.items():
            item={'name':name, 'bytes':len(payload), 'magic':payload[:8].hex()}
            if payload[:4] == b'RIFF':
                with wave.open(io.BytesIO(payload)) as wav:
                    item.update(channels=wav.getnchannels(),rate=wav.getframerate(),frames=wav.getnframes(),
                                seconds=wav.getnframes()/wav.getframerate())
            decoded.append(item)
        result['audio'].append({'owner':owner,'id':path_id,'name':clip.m_Name,
                               'resource':tree.get('m_Resource'),'seconds':clip.m_Length,'samples':decoded})
    except Exception as error:
        result['errors'].append({'kind':'audio','id':path_id,'error':repr(error)})
for path_id in [1214,783,1245,2568,2573,2298,2193]:
    try:
        obj=owners['resources.assets'].objects[path_id]
        tree=obj.read_typetree()
        (ROOT / f'clip-{path_id}.json').write_text(json.dumps(tree,indent=2), encoding='utf-8')
        result['clips'].append({'id':path_id,'name':tree.get('m_Name'),'sampleRate':tree.get('m_SampleRate'),
                               'spriteCurves':len(tree.get('m_PPtrCurves',[])),
                               'events':tree.get('m_Events'), 'settings':tree.get('m_AnimationClipSettings'),
                               'keys':list(tree)})
    except Exception as error:
        result['errors'].append({'kind':'clip','id':path_id,'error':repr(error)})
(ROOT / 'donor-probe.json').write_text(json.dumps(result,indent=2),encoding='utf-8')
print(json.dumps(result,indent=2))
