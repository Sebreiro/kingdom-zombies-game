"""Bounded read-only donor conversion and portable retained-media verification.

Authoring writes a new generation beneath .phase4 only. Publication is a separate
reviewed caller operation; this process never writes donors or invokes callbacks.
"""
import base64
import hashlib
import io
import json
import math
import pathlib
import re
import struct
import sys
import unicodedata
import warnings
import zipfile

sys.dont_write_bytecode = True
ROOT = pathlib.Path(__file__).resolve().parents[2]
DONOR = pathlib.Path('D:/Work/Games/KindomCrown/recovered')
DEPS = pathlib.Path('D:/Work/Games/KindomCrown/.tools/resource-python')
PYTHON = '3.12.14'
MAX_MEMBER = 128 * 1024 * 1024
MAX_TOTAL = 512 * 1024 * 1024
R = 'resources.assets'
S = 'sharedassets0.assets'
G = 'globalgamemanagers.assets'
# These identities originate in the approved donor register, independently of output.
ANCHORS = {
    'K01': (R, [10674, 9896, 10716]), 'K02': (R, [7982, 11002, 11491, 8949]),
    'K03': (R, [8604, 10266, 411]), 'K04': (R, [9480, 10311, 9893]),
    'K05': (R, [11222, 9204, 9468, 8203]), 'K06': (R, [7977, 9734, 8356, 10329]),
    'K07': (S, [8280, 7551]), 'K08': (R, [8144, 9758, 8847, 8053]),
    'K09': (R, [8279, 7865, 8711, 8027, 7808]), 'K10': (R, [9956, 10804]),
    'K11': (S, [7865, 8291]), 'K12': (R, [8557, 7981]),
    'K13': (R, [9243, 9577, 10160]), 'K14': (S, [2361]),
    'K15': (R, [6011, 6056, 6095]), 'K16': (S, [4214, 4399, 7986, 6853]),
    'K17': (S, [3879, 2939]), 'K18': (S, [7060, 7246]),
    'K20': (S, [7856, 7008, 6885]), 'K21': (S, [7985, 6994, 7295]),
    'K22': (R, [10173, 8376]), 'K23': (R, [217, 292, 269, 453]),
    'K24': (S, [7837, 7900, 6925, 8173, 8269, 7867, 8151]),
    'K25': (S, [6949, 7692, 7907, 7540]), 'K26': (G, [3]),
    'K27': (R, [6003, 6004]), 'K30': (R, [10583, 8704, 9819, 8043]),
}
CLIPS = [777, 780, 783, 786, 804, 819, 823, 903, 906, 912, 915, 918,
         1204, 1207, 1214, 1245, 2193, 2197, 2201, 2278, 2282, 2286,
         2290, 2294, 2298, 2451, 2455, 2459, 2463, 2568, 2569, 2570, 2571, 2572, 2573]
FAMILIES = {
    'K01': r'trees_(?:(?:bg_)?(?:trunk|leaves)(?:_birch)?_\d+(?:_EXTENDED)?(?:_\d+)?|stump(?:_birch)?_\d+)',
    'K02': r'(?:hills_\w+|ground_object_(?:moss|roots)\w*)',
    'K03': r'(?:clouds_\d+|clouds_(?:highlight|stratus)\w*)',
    'K04': r'berrybush_berries_0[1-4]', 'K06': r'fire_(?:mid|small)_burn_\d+',
    'K07': r'chest_(?:open|close|empty)_\d+',
    'K08': r'wall_wreck_[12]_s\d+_\d+', 'K10': r'scaffolds_(?:fill_)?0[1-5]',
    'K11': r'portal_(?:back|front)_s\d+_\d+',
    'K12': r'deer_(?:walk|run|eat|look|die)_\d+',
    'K13': r'bunny_(?:stand|dead|walk_\d+)',
    'K14': r'peasant_(?:idle|walk|run)_\d+',
    'K15': r'archer_(?:idle|walk|run|shoot|shoot_prep)_\d+',
    'K16': r'worker_(?:hammer|chop|work)_\d+',
    'K17': r'farmer_forage_(?:idle|putaway|walk|work|work_loop)_\d+',
    'K18': r'beggar_(?:eat|die)_\d+', 'K20': r'coin_spin_\d+',
    'K22': r'border_banner_wave_\d+', 'K23': r'splash_mid_\d+',
    'K26': r'input_glyph_keyboard_(?:sides|[a-z]+)_\d+',
}
APPROVED_EVENTS = {'OnAnimHammerHit': 'work.contact'}
ANCHOR_NAMES = {
    'K01': 'trees_trunk_02 trees_leaves_02_0 trees_stump_01',
    'K02': 'backdrop_hills cliff_stones_ground_EXTENDED grass_0 ground_object_rocks_01',
    'K03': 'clouds_01 moon_new_recoloured star', 'K04': 'berrybush berrybush_berries_01 berrybush_basket',
    'K05': 'athena_apple_orchard_back athena_apple_orchard_apples_back athena_apple_orchard_front athena_apple_orchard_apples_front',
    'K06': 'campfire_01 campfire fire_mid_burn_0 fire_small_burn_0', 'K07': 'chest_open_0 chest_empty_1',
    'K08': 'wall_1 wall_2 wall_0 wall_wreck_1_s028_040', 'K09': 'tower_1a tower_1b tower_2 tower_0a tower_0b',
    'K10': 'scaffolds_01 scaffolds_fill_01', 'K11': 'portal_back portal_front',
    'K12': 'deer_walk_0 deer_die_0', 'K13': 'bunny_stand bunny_dead bunny_walk_0', 'K14': 'peasant_idle_0',
    'K15': 'archer_idle_1 archer_walk_0 archer_shoot_4', 'K16': 'worker_hammer_0 worker_chop_2 worker_work_0 tools_hammer',
    'K17': 'farmer_forage_work_0 farmer_forage_work_loop_3', 'K18': 'beggar_eat_0 beggar_die_0',
    'K20': 'coin_spin_0 tools_bow arrow', 'K21': 'crown crown_indicator crown_glow',
    'K22': 'banner_1 border_portal_icon', 'K23': 'fire_mid_particle fire_small_particle sparks smoke_small',
    'K25': 'map_icon_portal_active map_icon_portal_destroyed map_campfire map_pointer_0',
    'K26': 'ui_cursor', 'K27': 'PerfectDOSVGA437 LiberationSans',
    'K30': 'castle_02_front castle_02_back castle_02_back_left castle_02_back_right',
}


def need(ok, code):
    if not ok:
        raise ValueError(code)


def sha(value):
    return hashlib.sha256(value).hexdigest()


def digest(file):
    with file.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def relative(value):
    need(isinstance(value, str) and 0 < len(value) < 220 and
         re.fullmatch(r'[A-Za-z0-9._/-]+', value) and
         all(x not in ('', '.', '..') and not x.endswith('.') and
             not re.match(r'(?i)^(con|prn|aux|nul|com[0-9]|lpt[0-9])(?:\.|$)', x)
             for x in value.split('/')), 'PATH_INVALID')
    return value


def contained(root, value):
    relative(value)
    target = root / value
    need(target.resolve().is_relative_to(root.resolve()), 'PATH_ESCAPE')
    for ancestor in [target, *target.parents]:
        need(not ancestor.is_symlink() and not ancestor.is_junction(), 'PATH_REPARSE')
        if ancestor == root:
            break
    return target


def owned(value):
    target = pathlib.Path(value).absolute()
    base = ROOT / '.phase4'
    need(target.is_relative_to(base) and target != base, 'OUTPUT_ROOT')
    contained(ROOT, target.relative_to(ROOT).as_posix())
    need(not target.exists(), 'OUTPUT_EXISTS')
    target.mkdir(parents=True)
    return target


def portable(value, depth=0, budget=None):
    budget = [0] if budget is None else budget
    budget[0] += 1
    need(budget[0] <= 1000000, 'JSON_NODES')
    need(depth <= 24, 'JSON_DEPTH')
    if isinstance(value, bytes):
        return {'base64': base64.b64encode(value).decode('ascii')}
    if isinstance(value, dict):
        need(len(value) <= 16384, 'JSON_COUNT')
        return {str(k): portable(v, depth + 1, budget) for k, v in value.items()}
    if isinstance(value, (list, tuple)):
        need(len(value) <= 100000, 'JSON_COUNT')
        return [portable(x, depth + 1, budget) for x in value]
    if isinstance(value, str):
        need(len(value) <= MAX_MEMBER and unicodedata.normalize('NFC', value) == value, 'JSON_IDENTITY')
    if isinstance(value, float):
        need(math.isfinite(value), 'JSON_NUMBER')
    return value


def json_bytes(value):
    return (json.dumps(portable(value), ensure_ascii=False, allow_nan=False,
                       separators=(',', ':')) + '\n').encode('utf-8')


def loads(value):
    need(len(value) <= MAX_MEMBER, 'JSON_SIZE')
    def pairs(items):
        result = {}
        for key, item in items:
            need(key not in result and key not in ('__proto__', 'constructor', 'prototype'), 'JSON_KEY')
            result[key] = item
        return result
    return portable(json.loads(value.decode('utf-8', errors='strict'), object_pairs_hook=pairs,
                               parse_constant=lambda _: need(False, 'JSON_NUMBER')))


def image_module():
    need(sys.version.split()[0] == PYTHON, 'PYTHON_PIN')
    sys.path.insert(0, str(DEPS))
    from PIL import Image
    need(Image.__version__ == '13.0.0', 'PILLOW_PIN')
    Image.MAX_IMAGE_PIXELS = 16777216
    warnings.simplefilter('error', Image.DecompressionBombWarning)
    return Image


def png(image):
    stream = io.BytesIO()
    image.save(stream, format='PNG', compress_level=9)
    return stream.getvalue()


def image_info(image):
    rgba = image.convert('RGBA')
    alpha = rgba.getchannel('A')
    return {'width': rgba.width, 'height': rgba.height, 'rgbaSha256': sha(rgba.tobytes()),
            'alphaBBox': list(alpha.getbbox()) if alpha.getbbox() else None, 'alphaValues': sorted(set(alpha.tobytes())),
            'visiblePixels': sum(x > 0 for x in alpha.tobytes())}


def streamed(words, count, start, stop):
    need(len(words) <= 1000000, 'STREAM_LIMIT')
    raw = struct.pack('<' + 'I' * len(words), *words)
    cursor, prior, tracks, terminated = 0, -math.inf, [[] for _ in range(count)], False
    while cursor < len(raw):
        need(cursor + 8 <= len(raw) and not terminated, 'STREAM_TRUNCATED')
        time, size = struct.unpack_from('<fI', raw, cursor)
        cursor += 8
        need(size <= count and cursor + size * 20 <= len(raw), 'STREAM_COUNT')
        if math.isinf(time):
            need(time > 0 and size == 0 and cursor == len(raw), 'STREAM_SENTINEL')
            terminated = True
            continue
        need(math.isfinite(time) and time >= prior, 'STREAM_TIME')
        prior = time
        seen = set()
        for _ in range(size):
            index, *coefficients = struct.unpack_from('<i4f', raw, cursor)
            cursor += 20
            need(0 <= index < count and index not in seen and all(math.isfinite(x) for x in coefficients), 'STREAM_KEY')
            seen.add(index)
            effective = start if time < -1e30 else time
            need(start <= effective <= stop + 1e-6, 'STREAM_RANGE')
            tracks[index].append({'time': min(effective, stop), 'value': coefficients[-1],
                                  'coefficients': coefficients, 'sourceTime': 'initial' if time < -1e30 else time})
    need(not words or terminated, 'STREAM_TERMINATOR')
    return tracks


def convert_clip(tree, mapping):
    muscle = tree['m_MuscleClip']
    start, stop = muscle['m_StartTime'], muscle['m_StopTime']
    need(all(math.isfinite(x) for x in [start, stop]) and 0 <= start <= stop <= 3600, 'CLIP_RANGE')
    data = muscle['m_Clip']['data']
    stream, dense, constant = data['m_StreamedClip'], data['m_DenseClip'], data['m_ConstantClip']['data']
    count = stream['curveCount'] + stream.get('discreteCurveCount', 0)
    need(0 <= count <= 1024 and 0 <= dense['m_CurveCount'] <= 1024 and len(constant) <= 1024, 'CURVE_LIMIT')
    tracks = streamed(stream['data'], count, start, stop)
    dense_count, frames = dense['m_CurveCount'], dense['m_FrameCount']
    need(0 <= frames <= 100000 and len(dense['m_SampleArray']) == dense_count * frames, 'DENSE_COUNT')
    if dense_count:
        rate, begin = dense['m_SampleRate'], dense['m_BeginTime']
        need(math.isfinite(rate) and rate > 0 and math.isfinite(begin), 'DENSE_RATE')
        for curve in range(dense_count):
            keys = [{'time': begin + frame / rate, 'value': dense['m_SampleArray'][frame * dense_count + curve]}
                    for frame in range(frames) if start <= begin + frame / rate <= stop]
            need(keys and all(math.isfinite(k['value']) for k in keys), 'DENSE_KEY')
            tracks.append(keys)
    for value in constant:
        need(math.isfinite(value), 'CONSTANT_NUMBER')
        tracks.append([{'time': start, 'value': value}])
    bindings = tree['m_ClipBindingConstant']['genericBindings']
    need(0 < len(bindings) <= 1024, 'BINDING_COUNT')
    layers, cursor = [], 0
    for binding in bindings:
        width = 4 if binding['typeID'] == 4 and binding['attribute'] == 2 else (
            3 if binding['typeID'] == 4 and binding['attribute'] in [1, 3, 4] else 1)
        need(cursor + width <= len(tracks), 'BINDING_TRACK_MISSING')
        for component in range(width):
            keys = tracks[cursor + component]
            need(keys and keys[0]['time'] <= start + 1e-6, 'INITIAL_KEY_MISSING')
            keys = [dict(key) for key in keys]
            if binding['isPPtrCurve']:
                for key in keys:
                    value = key['value']
                    need(value == int(value) and 0 <= int(value) < len(mapping), 'POINTER_MAPPING')
                    key['sprite'] = mapping[int(value)]
            layers.append({'binding': binding, 'component': component, 'curve': cursor + component,
                           'interpolation': 'discrete' if binding['isPPtrCurve'] or binding.get('isIntCurve') else (
                               'cubic' if cursor + component < count else 'linear' if cursor + component < count + dense_count else 'constant'),
                           'keys': keys, 'endpoint': keys[-1].get('sprite', keys[-1]['value'])})
        cursor += width
    need(cursor == len(tracks), 'UNBOUND_CURVES')
    approved, excluded = [], []
    for event in tree['m_Events']:
        need(start <= event['time'] <= stop, 'EVENT_TIME')
        name = event['functionName']
        if name in APPROVED_EVENTS:
            approved.append({'time': event['time'], 'id': APPROVED_EVENTS[name], 'sourceFunction': name,
                             'execution': 'presentation-data-only'})
        else:
            excluded.append(event)
    return {'name': tree['m_Name'], 'start': start, 'stop': stop, 'loop': muscle['m_LoopTime'],
            'sampleRate': tree['m_SampleRate'], 'layers': layers, 'mapping': mapping,
            'events': approved, 'excludedEvents': excluded, 'callbacksExecutable': False,
            'endpointPolicy': 'explicit-loop' if muscle['m_LoopTime'] else 'hold-final',
            'curveStorage': {'streamed': count, 'dense': dense_count, 'constant': len(constant)}}


def archive(file, members):
    need(0 < len(members) <= 16384 and sum(len(x) for x in members.values()) <= MAX_TOTAL, 'BUNDLE_LIMIT')
    with zipfile.ZipFile(file, 'x', compression=zipfile.ZIP_DEFLATED, compresslevel=9) as output:
        for name, value in sorted(members.items()):
            relative(name)
            need(0 < len(value) <= MAX_MEMBER, 'BUNDLE_MEMBER_SIZE')
            info = zipfile.ZipInfo(name, (2026, 11, 16, 0, 0, 0))
            info.compress_type = zipfile.ZIP_DEFLATED
            info.external_attr = 0o100644 << 16
            output.writestr(info, value, compresslevel=9)


def read_archive(file):
    need(file.stat().st_size <= MAX_TOTAL, 'BUNDLE_SIZE')
    result, folded, size = {}, set(), 0
    with zipfile.ZipFile(file) as source:
        need(0 < len(source.infolist()) <= 16384, 'BUNDLE_COUNT')
        for item in source.infolist():
            relative(item.filename)
            need(item.filename.casefold() not in folded and item.file_size <= MAX_MEMBER and item.file_size > 0 and
                 item.external_attr >> 28 != 10 and not item.flag_bits & 1, 'BUNDLE_MEMBER')
            folded.add(item.filename.casefold())
            size += item.file_size
            need(size <= MAX_TOTAL, 'BUNDLE_TOTAL')
            result[item.filename] = source.read(item)
    return result


def extract(destination):
    Image = image_module()
    import UnityPy
    from UnityPy.helpers.ResourceReader import get_resource_data
    from UnityPy.export import SpriteHelper
    need(UnityPy.__version__ == '1.23.0', 'UNITYPY_PIN')
    manifest_bytes = (DONOR / 'resources/manifest.json').read_bytes()
    manifest = loads(manifest_bytes)
    original_rows, members, objects, images, resources, clips = {}, {}, {}, {}, [], []
    data_root = DONOR / 'resources/KingdomTwoCrowns_Data'

    def preserve(name):
        relative(name)
        if name not in original_rows:
            row = next(x for x in manifest['files'] if x['path'] == name)
            file = contained(data_root, name)
            need(file.stat().st_size == row['bytes'] and digest(file) == row['sha256'], 'ORIGINAL_IDENTITY')
            original_rows[name] = row

    for name in [R, S, G]:
        preserve(name)
    environment = UnityPy.load(*(str(data_root / name) for name in [R, S, G]))
    owners = {pathlib.Path(obj.assets_file.name).name: obj.assets_file for obj in environment.objects}

    def identity(obj):
        owner = pathlib.Path(obj.assets_file.name).name
        preserve(owner)
        return {'owner': owner, 'objectId': obj.path_id, 'type': obj.type.name,
                'ownerSha256': original_rows[owner]['sha256'], 'rawSha256': sha(obj.get_raw_data())}

    def retain(obj):
        key = (pathlib.Path(obj.assets_file.name).name, obj.path_id)
        if key in objects:
            return objects[key]
        record = identity(obj)
        record['rawMember'] = f'raw/{key[0]}/{key[1]}.bin'
        raw = obj.get_raw_data()
        need(0 < len(raw) <= MAX_MEMBER, 'OBJECT_SIZE')
        members[record['rawMember']] = raw
        objects[key] = record
        return record

    def texture(pointer):
        obj = pointer.deref()
        record = retain(obj)
        if 'stream' in record:
            return record
        value = obj.read()
        record['name'] = value.m_Name
        stream = value.m_StreamData
        if stream and stream.size:
            name = pathlib.PurePosixPath(stream.path.replace('\\', '/')).name
            preserve(name)
            need(0 <= stream.offset and 0 < stream.size <= MAX_MEMBER and stream.offset + stream.size <= (data_root / name).stat().st_size, 'TEXTURE_STREAM_RANGE')
            payload = get_resource_data(stream.path, obj.assets_file, stream.offset, stream.size)
            need(len(payload) == stream.size, 'TEXTURE_STREAM_TRUNCATED')
            member = f'streams/{name}/{stream.offset}-{stream.size}.bin'
            members[member] = payload
            record['stream'] = {'path': name, 'offset': stream.offset, 'bytes': stream.size, 'sha256': sha(payload), 'member': member}
        else:
            record['stream'] = None
        members[f'trees/{record["owner"]}/{record["objectId"]}.json'] = json_bytes(obj.read_typetree())
        return record

    def sprite(obj, family):
        key = (pathlib.Path(obj.assets_file.name).name, obj.path_id)
        if key in images:
            images[key][1]['families'] = sorted(set(images[key][1]['families'] + [family]))
            return images[key][1]
        need(len(images) < 2048, 'SPRITE_COUNT')
        record, value, tree = retain(obj), obj.read(), obj.read_typetree()
        record['name'] = value.m_Name
        render = value.m_RD
        dependencies = []
        if value.m_SpriteAtlas:
            atlas = value.m_SpriteAtlas.read()
            dependencies.append(retain(atlas.object_reader))
            render = next(v for k, v in atlas.m_RenderDataMap if k == value.m_RenderDataKey)
        elif value.m_AtlasTags:
            need(False, 'ATLAS_POINTER_MISSING')
        dependencies.append(texture(render.texture))
        if render.alphaTexture:
            dependencies.append(texture(render.alphaTexture))
        # UnityPy resolves tight mesh, atlas packing rotation and split alpha.
        image = value.image.convert('RGBA')
        need(0 < image.width <= 4096 and 0 < image.height <= 4096 and image.width * image.height <= 16777216, 'SPRITE_SIZE')
        info = image_info(image)
        # A source-authored empty frame hides a layer; retaining it is essential
        # to portal endpoints. It is never used as a completed visible asset.
        logical = tree['m_Rect']
        offset = render.textureRectOffset
        trim = [offset.x, offset.y]
        pivot = tree['m_Pivot']
        registration = {'logicalRect': logical, 'pivotNormalizedBottomLeft': pivot,
                        'sourcePixelsPerUnit': value.m_PixelsToUnits, 'sourceOffset': tree['m_Offset'],
                        'trimOffsetBottomLeft': trim, 'atlasTextureRect': {key: getattr(render.textureRect, key) for key in ['x', 'y', 'width', 'height']},
                        'atlasSettingsRaw': render.settingsRaw, 'packingRotation': (render.settingsRaw >> 2) & 15,
                        'alphaTextureId': render.alphaTexture.path_id,
                        'offsetTopLeftExact': [trim[0] - pivot['x'] * logical['width'],
                                               (1 - pivot['y']) * logical['height'] - trim[1] - image.height],
                        'filter': 'nearest', 'resampled': False}
        member = f'images/{key[0]}/{key[1]}.png'
        members[member] = png(image)
        members[f'trees/{key[0]}/{key[1]}.json'] = json_bytes(tree)
        resource = {'id': f'common.{"r" if key[0] == R else "s0"}.{key[1]}', 'version': 1,
                    'name': value.m_Name, 'families': [family], 'source': record,
                    'dependencies': [dict(x) for x in dependencies], 'registration': registration,
                    'image': info, 'authoredEmptyFrame': info['visiblePixels'] == 0,
                    'member': member, 'pngSha256': sha(members[member]),
                    'sourceEvidence': 'V/M/C are historical candidate observations; not production approval',
                    'validation': {'source': 'decoded', 'visual': 'pending-04-32', 'obligationStatus': 'production-input'}}
        images[key] = (image, resource)
        return resource

    selected = {}
    for family, (owner, ids) in ANCHORS.items():
        for index, object_id in enumerate(ids):
            if family in ANCHOR_NAMES:
                need(owners[owner].objects[object_id].read().m_Name == ANCHOR_NAMES[family].split()[index], 'ANCHOR_NAME_' + family)
            selected[(owner, object_id)] = family
    selected.update({(S, 18126): 'K22', (S, 7705): 'K22', (S, 134): 'K23', (S, 6967): 'K23',
                     (S, 7592): 'K26', (S, 558): 'K27', (S, 564): 'K27'})
    family_audit = {family: {'selected': [], 'rejected': [], 'suitability': 'Reusable source pixels/motion; game fitting and actual judgment pending'}
                    for family in list(ANCHORS) + ['K28', 'K29']}
    for obj in environment.objects:
        if obj.type.name != 'Sprite' or pathlib.Path(obj.assets_file.name).name not in [R, S]:
            continue
        tree = obj.read_typetree()
        name = tree['m_Name']
        for family, pattern in FAMILIES.items():
            if re.fullmatch(pattern, name):
                atlas = tree['m_AtlasTags']
                if any(re.search(r'bamboo|deadlands|norselands|greece|shogun', x, re.I) for x in atlas):
                    family_audit[family]['rejected'].append({'owner': pathlib.Path(obj.assets_file.name).name, 'objectId': obj.path_id,
                                                              'name': name, 'reason': 'alternate biome excluded from ordinary common kit'})
                else:
                    selected.setdefault((pathlib.Path(obj.assets_file.name).name, obj.path_id), family)
    for clip_id in CLIPS:
        obj = owners[R].objects[clip_id]
        need(obj.type.name == 'AnimationClip', 'CLIP_TYPE')
        tree, value, mapping = obj.read_typetree(), obj.read(), []
        record = retain(obj)
        record['name'] = tree['m_Name']
        for pointer in value.m_ClipBindingConstant.pptrCurveMapping:
            if pointer.path_id == 0:
                mapping.append(None)
            else:
                target = pointer.deref()
                need(target.type.name == 'Sprite', 'CLIP_POINTER_TYPE')
                source = sprite(target, 'animation-dependency')
                mapping.append(source['id'])
        # Sentinel floats are represented explicitly in the retained source tree.
        member = f'clips/{clip_id}.json'
        members[member] = json_bytes(tree)
        converted = convert_clip(tree, mapping)
        converted.update({'id': f'common.clip.{clip_id}', 'source': record, 'sourceTree': member})
        clips.append(converted)
    for (owner, object_id), family in sorted(selected.items()):
        obj = owners[owner].objects[object_id]
        need(obj.type.name in ['Sprite', 'Texture2D', 'Font'], 'SELECTED_TYPE')
        if obj.type.name == 'Sprite':
            item = sprite(obj, family)
            family_audit[family]['selected'].append(item['id'])
        elif obj.type.name == 'Font':
            record, font = retain(obj), obj.read()
            record['name'] = font.m_Name
            payload = bytes(font.m_FontData)
            need(payload[:4] == b'\x00\x01\x00\x00' and 0 < len(payload) < 2 * 1024 * 1024, 'FONT_BYTES')
            member = f'fonts/{owner}/{object_id}.ttf'
            members[member] = payload
            resources.append({'id': f'common.font.{object_id}', 'name': font.m_Name, 'version': 1, 'source': record,
                              'member': member, 'sha256': sha(payload), 'bytes': len(payload), 'family': family,
                              'validation': 'bytes verified; glyph/readability judgment pending-04-32'})
            family_audit[family]['selected'].append(resources[-1]['id'])
        else:
            value = obj.read()
            record = retain(obj)
            record['name'] = value.m_Name
            # Resolve explicit texture records through the existing pointer class.
            from UnityPy.classes import PPtr
            record = texture(PPtr(m_FileID=0, m_PathID=obj.path_id, assetsfile=obj.assets_file))
            image = value.image.convert('RGBA')
            member = f'textures/{owner}/{object_id}.png'
            members[member] = png(image)
            resources.append({'id': f'common.texture.{object_id}', 'version': 1, 'name': value.m_Name, 'source': record,
                              'family': family, 'member': member, 'image': image_info(image), 'sha256': sha(members[member])})
            family_audit[family]['selected'].append(resources[-1]['id'])
    # K29 decoded records must match the exact original raw object, never just names.
    config_file = DONOR / 'resources/gameplay-configs/resources.assets.jsonl'
    need(config_file.stat().st_size <= MAX_MEMBER, 'CONFIG_SIZE')
    configs = [loads(line) for line in config_file.read_bytes().splitlines()]
    for object_id in [73492, 73486]:
        row = next(x for x in configs if x['path_id'] == object_id)
        record = retain(owners[R].objects[object_id])
        need(record['rawSha256'] == row['sha256'] and len(members[record['rawMember']]) == row['bytes'] and row['name'] == 'OakAndBirch', 'K29_RAW_IDENTITY')
        member = f'palettes/{object_id}.json'
        members[member] = json_bytes(row)
        family_audit['K29']['selected'].append(member)
    biome = next(x for x in configs if x['path_id'] == 73492)['fields']
    for key in ['deerColorA', 'deerColorB']:
        need(biome[key]['m_NumColorKeys'] == 2, 'DEER_GRADIENT')
    for name in ['Deer.cs', 'BaseSpriteFX.cs']:
        payload = (DONOR / 'source/Assembly-CSharp' / name).read_bytes()
        need(len(payload) <= MAX_MEMBER, 'RECOLOR_SOURCE_SIZE')
        members['reference-code/' + name] = payload
    # Explicit source-input records preserve assembly relationships without importing gameplay.
    upgrades = (DONOR / 'resources/building-upgrades.json').read_bytes()
    need(len(upgrades) <= MAX_MEMBER, 'UPGRADE_SIZE')
    members['assembly/building-upgrades.json'] = upgrades
    save_file = data_root / 'StreamingAssets/SaveIcon.png'
    save = save_file.read_bytes()
    need(len(save) <= MAX_MEMBER, 'SAVE_SIZE')
    save_image = Image.open(io.BytesIO(save)).convert('RGBA')
    members['loose/SaveIcon.png'] = save
    resources.append({'id': 'common.save-indicator', 'version': 1, 'name': 'Save indicator', 'family': 'K28',
                      'source': {'path': 'KingdomTwoCrowns_Data/StreamingAssets/SaveIcon.png', 'sha256': sha(save)},
                      'member': 'loose/SaveIcon.png', 'image': image_info(save_image)})
    family_audit['K28']['selected'].append('common.save-indicator')
    # Atlas packing preserves each decoded trimmed canvas; registration is separate.
    rows, x, y, row_height, width = [], 1, 1, 0, 2048
    for key, (image, item) in sorted(images.items()):
        need(image.width + 2 <= width, 'ATLAS_WIDTH')
        if x + image.width + 1 > width:
            x, y, row_height = 1, y + row_height + 2, 0
        item['atlasRect'] = [x, y, image.width, image.height]
        rows.append((image, x, y))
        x += image.width + 2
        row_height = max(row_height, image.height)
    height = y + row_height + 1
    need(width * height <= 16777216, 'ATLAS_AREA')
    atlas = Image.new('RGBA', (width, height))
    for image, x, y in rows:
        atlas.paste(image, (x, y))
    atlas_bytes = png(atlas)
    members['donor-kit.png'] = atlas_bytes
    recipe = pathlib.Path(__file__).read_bytes()
    members['recipes/extract-media.py'] = recipe
    for name, row in original_rows.items():
        need(digest(data_root / name) == row['sha256'], 'ORIGINAL_MUTATION')
    pack = {'schemaVersion': 1, 'id': 'common', 'version': 1, 'status': 'production-inputs-acceptance-pending',
            'resources': [item for _, item in sorted(images.values(), key=lambda x: x[1]['id'])] + resources,
            'atlas': {'path': 'assets/common/donor-kit.png', 'sha256': sha(atlas_bytes), **image_info(atlas)},
            'clips': clips, 'families': family_audit,
            'deerRecolor': {'gradients': {key: biome[key] for key in ['deerColorA', 'deerColorB']},
                           'source': 'palettes/73492.json', 'selectors': ['_RecolorA', '_RecolorB'],
                           'policy': 'Retain source alpha and mask channels; deterministic palette production belongs to wildlife fit slice',
                           'referenceCode': ['reference-code/Deer.cs', 'reference-code/BaseSpriteFX.cs']},
            'recipe': {'path': 'recipes/extract-media.py', 'sha256': sha(recipe), 'python': PYTHON,
                       'UnityPy': UnityPy.__version__, 'Pillow': Image.__version__,
                       'spriteHelperSha256': digest(pathlib.Path(SpriteHelper.__file__)),
                       'transformations': ['Exact selected Sprite.image atlas/mesh/rotation/alpha decoding',
                                           'Lossless RGBA trimmed canvases, original logical registration retained',
                                           'All runtime curve stores and binding components; original key order and repeated values',
                                           'Only work.contact presentation event; excluded donor callbacks remain inert data',
                                           'Deterministic shelf atlas with two transparent separator pixels; no scaling']},
            'preservation': {'manifestSha256': sha(manifest_bytes), 'originals': sorted(original_rows.values(), key=lambda x: x['path']),
                             'verifiedAfter': True},
            'acceptance': {'owner': '04-32/04-33', 'visual': 'pending', 'listening': 'not-applicable', 'completedObligations': []}}
    members['common.json'] = json_bytes(pack)
    members['preserved-manifest.json'] = manifest_bytes
    members['members.json'] = json_bytes([{'path': name, 'bytes': len(value), 'sha256': sha(value)} for name, value in sorted(members.items())])
    archive(destination / 'common-selected.zip', members)
    (destination / 'donor-kit.png').write_bytes(atlas_bytes)
    (destination / 'common.json').write_bytes(members['common.json'])
    print(json.dumps({'complete': True, 'sprites': len(images), 'clips': len(clips), 'members': len(members),
                      'atlas': [width, height], 'bundleSha256': digest(destination / 'common-selected.zip')}))


def verify_common(bundle, pack_file, atlas_file, supplied_members=None):
    Image = image_module()
    members = read_archive(bundle) if supplied_members is None else supplied_members
    pack = loads(members['common.json'])
    need(pack_file.read_bytes() == members['common.json'] and atlas_file.read_bytes() == members['donor-kit.png'], 'PUBLISHED_IDENTITY')
    need(sha(members['recipes/extract-media.py']) == pack['recipe']['sha256'], 'RECIPE_IDENTITY')
    inventory = loads(members['members.json'])
    need(len(inventory) + 1 == len(members), 'MEMBER_COUNT')
    for row in inventory:
        payload = members.get(row['path'])
        need(payload is not None and len(payload) == row['bytes'] and sha(payload) == row['sha256'], 'MEMBER_HASH')
    atlas = Image.open(io.BytesIO(members['donor-kit.png'])).convert('RGBA')
    need(image_info(atlas) == {key: pack['atlas'][key] for key in image_info(atlas)}, 'ATLAS_IMAGE')
    expected = set(ANCHORS) | {'K28', 'K29'}
    need(set(pack['families']) == expected and all(row['selected'] for row in pack['families'].values()), 'FAMILY_MISSING')
    images = {item['id']: item for item in pack['resources'] if 'atlasRect' in item}
    need(len(images) == sum('atlasRect' in item for item in pack['resources']), 'IMAGE_DUPLICATE')
    for item in images.values():
        image = Image.open(io.BytesIO(members[item['member']])).convert('RGBA')
        need(image_info(image) == item['image'] and sha(members[item['member']]) == item['pngSha256'], 'IMAGE_METADATA')
        x, y, width, height = item['atlasRect']
        need(width == image.width and height == image.height and x >= 1 and y >= 1 and
             x + width < atlas.width and y + height < atlas.height, 'ATLAS_RECT')
        need(atlas.crop((x, y, x + width, y + height)).tobytes() == image.tobytes(), 'ATLAS_PIXELS')
        reg, tree = item['registration'], loads(members[f'trees/{item["source"]["owner"]}/{item["source"]["objectId"]}.json'])
        need(reg['logicalRect'] == tree['m_Rect'] and reg['pivotNormalizedBottomLeft'] == tree['m_Pivot'] and
             reg['sourcePixelsPerUnit'] == tree['m_PixelsToUnits'] and reg['filter'] == 'nearest' and reg['resampled'] is False, 'REGISTRATION')
        pivot, logical, trim = reg['pivotNormalizedBottomLeft'], reg['logicalRect'], reg['trimOffsetBottomLeft']
        need(reg['offsetTopLeftExact'] == [trim[0] - pivot['x'] * logical['width'],
                                          (1 - pivot['y']) * logical['height'] - trim[1] - height], 'PIVOT_OFFSET')
        need(sha(members[item['source']['rawMember']]) == item['source']['rawSha256'], 'SOURCE_RAW_HASH')
        for dependency in item['dependencies']:
            need(sha(members[dependency['rawMember']]) == dependency['rawSha256'], 'DEPENDENCY_RAW_HASH')
            if dependency.get('stream'):
                stream = dependency['stream']
                need(len(members[stream['member']]) == stream['bytes'] and sha(members[stream['member']]) == stream['sha256'], 'STREAM_HASH')
    need([int(clip['id'].split('.')[-1]) for clip in pack['clips']] == CLIPS, 'CLIP_SELECTION')
    for clip in pack['clips']:
        converted = convert_clip(loads(members[clip['sourceTree']]), clip['mapping'])
        need(all(clip[key] == value for key, value in converted.items()), 'CLIP_CONVERSION')
        need(all(sprite is None or sprite in images for sprite in clip['mapping']), 'CLIP_IMAGE_MISSING')
    need(pack['acceptance']['completedObligations'] == [] and pack['acceptance']['visual'] == 'pending', 'FALSE_ACCEPTANCE')
    return {'sprites': len(images), 'clips': len(pack['clips']), 'members': len(members), 'portable': True,
            'atlasSha256': sha(members['donor-kit.png']), 'acceptance': pack['acceptance']}


def conversion_controls(bundle, destination):
    """Held-out two-layer irregular/repeated stream plus dense/constant tracks.

    This fixture's times, values and geometry are independent of research samples.
    Corrupt production records are tested after rebuilding their outer ZIP hashes,
    so a generic checksum failure cannot masquerade as a semantic rejection.
    """
    import copy
    words = []
    for time, keys in [(-3.4028234663852886e38, [(0, 2), (1, 1)]),
                       (0.1875, [(0, 0)]), (0.4375, [(0, 0), (1, 2)]), (0.8125, [(0, 1)]),
                       (math.inf, [])]:
        payload = struct.pack('<fI', time, len(keys))
        for index, value in keys:
            payload += struct.pack('<i4f', index, 0, 0, 0, value)
        words += list(struct.unpack('<' + 'I' * (len(payload) // 4), payload))
    binding = lambda path: {'path': path, 'typeID': 212, 'attribute': 0, 'isPPtrCurve': 1, 'isIntCurve': 0}
    fixture = {'m_Name': 'held-out.h16', 'm_SampleRate': 4,
               'm_MuscleClip': {'m_StartTime': 0, 'm_StopTime': 1, 'm_LoopTime': False,
                               'm_Clip': {'data': {'m_StreamedClip': {'data': words, 'curveCount': 0, 'discreteCurveCount': 2},
                                                  'm_DenseClip': {'m_CurveCount': 3, 'm_FrameCount': 5, 'm_SampleRate': 4,
                                                                  'm_BeginTime': 0, 'm_SampleArray': [0, 10, 20, 1, 11, 21, 2, 12, 22, 3, 13, 23, 4, 14, 24]},
                                                  'm_ConstantClip': {'data': [0.625]}}}},
               'm_ClipBindingConstant': {'genericBindings': [binding(53), binding(71),
                   {'path': 83, 'typeID': 4, 'attribute': 1, 'isPPtrCurve': 0, 'isIntCurve': 0},
                   {'path': 97, 'typeID': 212, 'attribute': 144, 'isPPtrCurve': 0, 'isIntCurve': 0}]},
               'm_Events': [{'time': 0.4375, 'functionName': 'OnAnimHammerHit'},
                            {'time': 0.8125, 'functionName': 'SpitOutHermits'}]}
    result = convert_clip(fixture, ['sprite.z', 'sprite.a', 'sprite.q'])
    need([[key['time'], key['sprite']] for key in result['layers'][0]['keys']] ==
         [[0, 'sprite.q'], [0.1875, 'sprite.z'], [0.4375, 'sprite.z'], [0.8125, 'sprite.a']], 'H16_IRREGULAR_REPEATED')
    need([[key['time'], key['sprite']] for key in result['layers'][1]['keys']] == [[0, 'sprite.a'], [0.4375, 'sprite.q']], 'H16_LAYER_HOLD')
    need([key['value'] for key in result['layers'][3]['keys']] == [10, 11, 12, 13, 14] and
         result['layers'][5]['keys'] == [{'time': 0, 'value': 0.625}], 'H16_DENSE_CONSTANT')
    need(result['layers'][0]['endpoint'] == 'sprite.a' and result['endpointPolicy'] == 'hold-final' and
         result['events'][0]['id'] == 'work.contact' and result['callbacksExecutable'] is False and
         len(result['excludedEvents']) == 1, 'H16_ENDPOINT_EVENT')
    looping = copy.deepcopy(fixture)
    looping['m_MuscleClip']['m_LoopTime'] = True
    need(convert_clip(looping, ['sprite.z', 'sprite.a', 'sprite.q'])['endpointPolicy'] == 'explicit-loop', 'H16_LOOP')
    outcomes = []
    for name, code, edit in [
        ('dense-count', 'DENSE_COUNT', lambda x: x['m_MuscleClip']['m_Clip']['data']['m_DenseClip']['m_SampleArray'].pop()),
        ('dense-rate', 'DENSE_RATE', lambda x: x['m_MuscleClip']['m_Clip']['data']['m_DenseClip'].__setitem__('m_SampleRate', 0)),
        ('constant-nan', 'CONSTANT_NUMBER', lambda x: x['m_MuscleClip']['m_Clip']['data']['m_ConstantClip']['data'].__setitem__(0, math.nan)),
        ('binding-layer', 'UNBOUND_CURVES', lambda x: x['m_ClipBindingConstant']['genericBindings'].pop()),
        ('truncated-stream', 'STREAM_COUNT', lambda x: x['m_MuscleClip']['m_Clip']['data']['m_StreamedClip']['data'].__setitem__(1, 3000)),
        ('event-outside', 'EVENT_TIME', lambda x: x['m_Events'][0].__setitem__('time', 1.5)),
    ]:
        subject = copy.deepcopy(fixture)
        edit(subject)
        failure = None
        try:
            convert_clip(subject, ['sprite.z', 'sprite.a', 'sprite.q'])
        except ValueError as error:
            failure = str(error)
        need(failure == code, 'CONTROL_NOT_REJECTED_' + name + '_' + str(failure))
        outcomes.append({'name': name, 'expectedRejectionObserved': True, 'diagnostic': failure})
    original = read_archive(bundle)
    for name, code in [('missing-layer', 'CLIP_CONVERSION'), ('wrong-pivot', 'REGISTRATION'),
                       ('lost-held-frame', 'CLIP_CONVERSION'), ('alpha-metadata', 'IMAGE_METADATA'),
                       ('callback-enabled', 'CLIP_CONVERSION'), ('loop-endpoint', 'CLIP_CONVERSION'),
                       ('missing-dependency', 'DEPENDENCY_RAW_HASH')]:
        members = dict(original)
        pack = loads(members['common.json'])
        image = next(x for x in pack['resources'] if 'atlasRect' in x)
        clip = next(x for x in pack['clips'] if len(x['layers']) > 1)
        if name == 'missing-layer':
            clip['layers'].pop()
        elif name == 'wrong-pivot':
            image['registration']['pivotNormalizedBottomLeft']['x'] += 0.25
        elif name == 'lost-held-frame':
            target = next(x for x in pack['clips'] if len(x['layers'][0]['keys']) > 2)
            target['layers'][0]['keys'].pop(1)
        elif name == 'alpha-metadata':
            image['image']['visiblePixels'] += 1
        elif name == 'callback-enabled':
            clip['callbacksExecutable'] = True
        elif name == 'loop-endpoint':
            clip['loop'] = not clip['loop']
        elif name == 'missing-dependency':
            dep = image['dependencies'][0]
            members[dep['rawMember']] = b'incorrect retained dependency'
        members['common.json'] = json_bytes(pack)
        members['members.json'] = json_bytes([{'path': member, 'bytes': len(payload), 'sha256': sha(payload)}
                                               for member, payload in sorted(members.items()) if member != 'members.json'])
        target = destination / name
        target.mkdir()
        # Keep only the changed records; the immutable original ZIP is shared.
        # Recompressing every multi-megabyte stream per fault wastes minutes.
        (target / 'mutation.json').write_bytes(json_bytes({'name': name, 'expected': code,
            'changedMembers': [key for key in members if members[key] != original.get(key)]}))
        (target / 'common.json').write_bytes(members['common.json'])
        (target / 'atlas.png').write_bytes(members['donor-kit.png'])
        failure = None
        try:
            verify_common(bundle, target / 'common.json', target / 'atlas.png', members)
        except ValueError as error:
            failure = str(error)
        need(failure == code, 'CONTROL_NOT_REJECTED_' + name + '_' + str(failure))
        outcomes.append({'name': name, 'expectedRejectionObserved': True, 'diagnostic': failure})
    (destination / 'held-out.json').write_bytes(json_bytes({'fixture': fixture, 'converted': result, 'expectedSource': 'independent literal oracle'}))
    (destination / 'negative-controls.json').write_bytes(json_bytes(outcomes))
    return {'heldOut': 'irregular repeated pointer keys; second held layer; three dense components; constant; explicit loop/hold endpoints',
            'expectedRejected': len(outcomes), 'controls': outcomes}


def main():
    args = sys.argv[1:]
    if len(args) == 2 and args[0] == '--extract':
        extract(owned(args[1]))
    elif len(args) == 4 and args[0] == '--verify-common':
        print(json.dumps(verify_common(*(pathlib.Path(x) for x in args[1:]))))
    elif len(args) == 3 and args[0] == '--conversion-controls':
        print(json.dumps(conversion_controls(pathlib.Path(args[1]), owned(args[2]))))
    else:
        raise ValueError('USAGE --extract NEW_OWNED_DIRECTORY | --verify-common BUNDLE PACK ATLAS')


if __name__ == '__main__':
    main()
