"""Read-only exact Unity AudioClip selection; one native decoder owner per child."""
import base64
import hashlib
import importlib.metadata
import io
import json
import pathlib
import sys
import wave

sys.dont_write_bytecode = True
ROOT = pathlib.Path(__file__).resolve().parents[2]
DONOR = pathlib.Path('D:/Work/Games/KindomCrown/recovered/resources')
DEPENDENCIES = pathlib.Path('D:/Work/Games/KindomCrown/.tools/resource-python')
PYTHON_VERSION = '3.12.14'
SELECTIONS = {
    'feasibility.action': ('resources.assets', 3288, 'arrow_shoot', 3016),
    'feasibility.confirmation': ('sharedassets0.assets', 449, 'ui_select', 9242),
    'feasibility.ambience': ('resources.assets', 4441, 'env_homefire', 570452),
}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def file_hash(file):
    with file.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


def owned(value):
    target = pathlib.Path(value).resolve()
    assert target.is_relative_to((ROOT / '.phase4').resolve()), 'AUDIO_OUTPUT_ROOT'
    assert not target.exists(), 'AUDIO_OUTPUT_EXISTS'
    for parent in target.parents:
        assert not parent.is_symlink(), 'AUDIO_OUTPUT_REPARSE'
    target.mkdir(parents=True)
    return target


def decode(identity, destination):
    assert sys.version.split()[0] == PYTHON_VERSION, 'PYTHON_PIN'
    assert identity in SELECTIONS, 'AUDIO_SELECTION'
    sys.path.insert(0, str(DEPENDENCIES))
    import UnityPy
    from PIL import Image
    from UnityPy.helpers.ResourceReader import get_resource_data
    assert UnityPy.__version__ == '1.23.0' and Image.__version__ == '13.0.0', 'DECODER_PIN'
    assert importlib.metadata.version('pyfmodex') == '0.7.2', 'PYFMODEX_PIN'
    owner_name, object_id, expected_name, expected_frames = SELECTIONS[identity]
    data_root = DONOR / 'KingdomTwoCrowns_Data'
    manifest_bytes = (DONOR / 'manifest.json').read_bytes()
    manifest = json.loads(manifest_bytes)
    original_rows = []

    def preserved(name):
        assert pathlib.PurePath(name).name == name, 'AUDIO_SOURCE_PATH'
        expected = next(row for row in manifest['files'] if row['path'] == name)
        file = data_root / name
        assert file.stat().st_size == expected['bytes'], 'ORIGINAL_SIZE'
        assert file_hash(file) == expected['sha256'], 'ORIGINAL_HASH'
        original_rows.append(expected)

    preserved(owner_name)
    environment = UnityPy.load(str(data_root / owner_name))
    owner = next(file for file in environment.files.values() if pathlib.Path(file.name).name == owner_name)
    obj = owner.objects[object_id]
    assert obj.type.name == 'AudioClip', 'AUDIO_TYPE'
    clip = obj.read()
    assert clip.m_Name == expected_name, 'AUDIO_NAME'
    stream = clip.m_Resource
    resource_name = pathlib.PurePosixPath(stream.m_Source.replace('\\', '/')).name
    preserved(resource_name)
    assert 0 <= stream.m_Offset and 0 < stream.m_Size <= 32 * 1024 * 1024, 'AUDIO_STREAM_BOUNDS'
    raw = get_resource_data(stream.m_Source, obj.assets_file, stream.m_Offset, stream.m_Size)
    assert len(raw) == stream.m_Size, 'AUDIO_STREAM_TRUNCATED'
    samples = clip.samples
    assert len(samples) == 1 and expected_name + '.wav' in samples, 'AUDIO_SAMPLE_COUNT'
    wav_bytes = samples[expected_name + '.wav']
    with wave.open(io.BytesIO(wav_bytes), 'rb') as wav:
        frames, channels, rate, width = wav.getnframes(), wav.getnchannels(), wav.getframerate(), wav.getsampwidth()
        pcm = wav.readframes(frames + 1)
        assert frames == expected_frames and channels == 2 and rate == 44100 and width == 2, 'AUDIO_FORMAT'
        assert len(pcm) == frames * channels * width and wav.getcomptype() == 'NONE', 'AUDIO_PCM_TRUNCATED'
    for expected in original_rows:
        assert file_hash(data_root / expected['path']) == expected['sha256'], 'ORIGINAL_MUTATION'
    from UnityPy.export import AudioClipConverter
    AudioClipConverter.import_pyfmodex()
    fmod = pathlib.Path(AudioClipConverter.pyfmodex.__file__)
    native = DEPENDENCIES / 'UnityPy/lib/FMOD/Windows/x64/fmod.dll'
    result = {
        'id': identity, 'version': 1, 'role': identity.split('.')[-1],
        'source': {'owner': owner_name, 'type': 'AudioClip', 'objectId': object_id, 'name': expected_name,
                   'stream': {'path': resource_name, 'offset': stream.m_Offset, 'bytes': stream.m_Size,
                              'sha256': sha(raw)}, 'rawObjectSha256': sha(obj.get_raw_data())},
        'preservation': {'manifestSha256': sha(manifest_bytes), 'originalsBefore': original_rows,
                         'originalsAfter': original_rows, 'verifiedAfter': True},
        'tools': {'python': PYTHON_VERSION, 'pythonExeSha256': file_hash(pathlib.Path(sys.executable)),
                  'UnityPy': UnityPy.__version__, 'Pillow': Image.__version__,
                  'pyfmodex': importlib.metadata.version('pyfmodex'),
                  'converterSha256': file_hash(pathlib.Path(AudioClipConverter.__file__)),
                  'pyfmodexEntrySha256': file_hash(fmod), 'fmodDllSha256': file_hash(native)},
        'output': {'name': expected_name + '.wav', 'bytes': len(wav_bytes), 'sha256': sha(wav_bytes),
                   'pcmSha256': sha(pcm), 'pcmBytes': len(pcm), 'frames': frames, 'channels': channels,
                   'rate': rate, 'sampleWidth': width, 'seconds': frames / rate, 'trimFrames': [0, frames],
                   'loop': 'not-selected-or-auditioned', 'gainDb': 0},
        'bundle': {'originalPayloadBase64': base64.b64encode(raw).decode('ascii'),
                   'decodedWavBase64': base64.b64encode(wav_bytes).decode('ascii'),
                   'rawAudioClipBase64': base64.b64encode(obj.get_raw_data()).decode('ascii')},
        'status': {'decode': 'verified', 'listening': 'blocked-needs-capable-human',
                   'productionSelection': 'candidate-only', 'acceptanceOwner': '04-32'},
    }
    (destination / result['output']['name']).write_bytes(wav_bytes)
    (destination / 'sample.json').write_text(json.dumps(result, indent=2, allow_nan=False) + '\n', encoding='utf-8')
    print(json.dumps({'id': identity, 'version': 1, 'sha256': sha(wav_bytes), 'frames': frames, 'complete': True}))


if __name__ == '__main__':
    assert len(sys.argv) == 4 and sys.argv[1] == '--child', 'USAGE --child ID OWNED_NEW_DIRECTORY'
    decode(sys.argv[2], owned(sys.argv[3]))
