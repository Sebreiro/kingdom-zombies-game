"""Live entry point for the pinned Phase 4 extractors.

Recorded recipe hashes pin the bytes of extract-media.py and extract-audio.py, so both still derive ROOT from
their original tools/phase4/ depth. This entry point loads one of them from its file, points ROOT at the checkout
that contains this file, and then runs that extractor's own command-line entry.
"""
import importlib.util
import pathlib
import sys

sys.dont_write_bytecode = True
HERE = pathlib.Path(__file__).resolve().parent
CHECKOUT = HERE.parents[3]
EXTRACTORS = ('extract-media.py', 'extract-audio.py')


def load(name):
    assert name in EXTRACTORS, 'EXTRACTOR_UNKNOWN'
    spec = importlib.util.spec_from_file_location(name[:-3].replace('-', '_'), HERE / name)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    module.ROOT = CHECKOUT
    return module


def main():
    assert len(sys.argv) >= 2, 'USAGE run-extractor.py EXTRACTOR ARGUMENTS...'
    name = sys.argv[1]
    module = load(name)
    sys.argv = [str(HERE / name)] + sys.argv[2:]
    if name == 'extract-media.py':
        module.main()
    else:
        assert len(sys.argv) == 4 and sys.argv[1] == '--child', 'USAGE --child ID OWNED_NEW_DIRECTORY'
        module.decode(sys.argv[2], module.owned(sys.argv[3]))


if __name__ == '__main__':
    main()
