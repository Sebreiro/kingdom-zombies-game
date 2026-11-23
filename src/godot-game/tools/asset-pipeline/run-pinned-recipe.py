"""Run a retained recipe with its output guard bound to a new, owned directory."""

import pathlib
import sys


ABSOLUTE = b"pathlib.Path('D:/Work/Games/KingdomZombies/.phase4')"
CHECKOUT_RELATIVE = b"ROOT / '.phase4'"


def relocate(source, marker):
    if source.count(marker) != 1:
        raise ValueError('PINNED_OUTPUT_ROOT_GUARD')
    return source.replace(marker, b'_kz_owned_output_root')


def main():
    kind, script, owner, *args = sys.argv[1:]
    owner = pathlib.Path(owner).resolve(strict=True)
    script = pathlib.Path(script)
    if kind == 'decode' and len(args) == 3 and args[0] == '--child':
        marker, target = CHECKOUT_RELATIVE, pathlib.Path(args[2])
    elif kind == 'assemble' and len(args) == 1:
        marker, target = ABSOLUTE, pathlib.Path(args[0])
    else:
        raise ValueError('PINNED_RECIPE_ARGUMENTS')
    if script.parent != owner or not script.is_file() or script.is_symlink() or script.is_junction():
        raise ValueError('PINNED_RECIPE_SOURCE')
    target = target.absolute()
    if not target.is_relative_to(owner) or not target.resolve().is_relative_to(owner):
        raise ValueError('PINNED_RECIPE_OUTPUT')
    for component in [target, *target.parents]:
        if component.is_symlink() or component.is_junction():
            raise ValueError('PINNED_RECIPE_REPARSE')
        if component == owner:
            break
    sys.argv = [str(script), *args]
    code = compile(relocate(script.read_bytes(), marker), str(script), 'exec')
    exec(code, {'__name__': '__main__', '__file__': str(script), '_kz_owned_output_root': owner})


if __name__ == '__main__':
    main()
