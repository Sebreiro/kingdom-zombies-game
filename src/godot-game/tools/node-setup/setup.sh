#!/bin/sh
# Installs the pinned Node (toolchain.json nodeArchives) once per repository into the shared .local/tools/node/
# and prints one sh PATH line on stdout, for example: eval "$(sh src/godot-game/tools/node-setup/setup.sh)"
# Usage: setup.sh [--archive <verified local archive>] [--check] [--platform linux-x64]
#   --archive   seed the shared downloads from a local copy instead of downloading; it must match the pin.
#   --check     verify the pinned archive and the installed Node; download and write nothing.
#   --platform  linux-x64 only: verify and unpack the Linux archive without running it (VM preparation).
# Linux uses curl or wget, sha256sum and tar. Git Bash verifies an existing install itself and otherwise delegates
# installation to setup.ps1. The archive SHA-256 is verified before extraction; downloads and extraction go to
# nonce-named siblings that are renamed into place under the shared .local/locks/toolchain.lock.
# Messages go to stderr. KZ_SHARED_LOCAL overrides the shared root; shared-root.sh holds the rule it must meet.
set -eu

fail() { printf '%s: %s\n' "$1" "$2" >&2; exit 1; }
say() { printf '%s\n' "$1" >&2; }

archive_arg='' check=0 platform=''
while [ $# -gt 0 ]; do
  case $1 in
    --check) check=1 ;;
    --archive) [ $# -ge 2 ] || fail USAGE '--archive needs a path'; archive_arg=$2; shift ;;
    --platform) [ $# -ge 2 ] || fail USAGE '--platform needs a value'; platform=$2; shift ;;
    *) fail USAGE "setup.sh [--archive <file>] [--check] [--platform linux-x64]; unknown argument $1" ;;
  esac
  shift
done
[ -z "$platform" ] || [ "$platform" = linux-x64 ] || fail USAGE '--platform accepts only linux-x64'

# Without Git the shared root cannot be found; never fall back to this checkout's own .local and download there.
command -v git >/dev/null 2>&1 || fail GIT_MISSING 'git is not on PATH, so the shared .local root cannot be resolved; put Git on PATH and rerun'
here=$(cd "$(dirname "$0")" && pwd)
# Git for Windows is a native program: with MSYS_NO_PATHCONV set it cannot read an MSYS /c/... path.
if command -v cygpath >/dev/null 2>&1; then here=$(cygpath -m "$here"); fi
top=$(git -C "$here" rev-parse --show-toplevel 2>/dev/null) || top=$(cd "$here/../../../.." && pwd)
if command -v cygpath >/dev/null 2>&1; then top=$(cygpath -m "$top"); fi
. "$here/shared-root.sh"
manifest=$top/src/godot-game/tools/verification/toolchain.json
[ -f "$manifest" ] || fail PINS_MISSING "toolchain.json not found under $top"
version=$(sed -n 's/^ *"node": *"\([0-9.]*\)".*/\1/p' "$manifest" | head -n 1)
[ -n "$version" ] || fail PINS_MISSING 'toolchain.json node'

native='' windows=0
case $(uname -s) in
  MINGW*|MSYS*|CYGWIN*) native=win32-x64 windows=1 ;;
  Linux) [ "$(uname -m)" = x86_64 ] && native=linux-x64 ;;
esac
target=${platform:-$native}
[ -n "$target" ] || fail PLATFORM_UNSUPPORTED "no pinned Node archive for $(uname -s) $(uname -m)"
case $target in
  win32-x64) file=node-v$version-win-x64.zip directory=node-v$version-win-x64 bin= exe=node.exe ;;
  linux-x64) file=node-v$version-linux-x64.tar.xz directory=node-v$version-linux-x64 bin=/bin exe=node ;;
esac

# The pin is the single toolchain line naming the archive file; exactly one 64-hex value must parse from it.
pinline=$(grep -F "\"file\": \"$file\"" "$manifest" || true)
[ -n "$pinline" ] && [ "$(printf '%s\n' "$pinline" | wc -l)" -eq 1 ] || fail PINS_MISSING "exactly one nodeArchives line must name $file"
sha=$(printf '%s\n' "$pinline" | grep -oE '[0-9a-f]{64}' || true)
[ -n "$sha" ] && [ "$(printf '%s\n' "$sha" | wc -l)" -eq 1 ] || fail PINS_MISSING "exactly one SHA-256 must parse from the $file pin"
url=$(printf '%s\n' "$pinline" | sed -n 's/.*"url": *"\(https:\/\/[^"]*\)".*/\1/p')
[ -n "$url" ] || fail PINS_MISMATCH "the $file pin needs an HTTPS url"
printf '%s\n' "$pinline" | grep -qF "\"directory\": \"$directory\"" || fail PINS_MISMATCH "the $file pin must name directory $directory"

if [ -n "${KZ_SHARED_LOCAL:-}" ]; then
  problem=$(kz_override_problem "$KZ_SHARED_LOCAL" "$top" "$windows")
  [ -z "$problem" ] || fail SHARED_ROOT_INVALID "KZ_SHARED_LOCAL $problem: $KZ_SHARED_LOCAL"
  shared=$KZ_SHARED_LOCAL
elif common=$(git -C "$top" rev-parse --path-format=absolute --git-common-dir 2>/dev/null); then
  [ "$(basename "$common")" = .git ] || fail SHARED_ROOT_UNRESOLVED "git common dir $common is not a .git folder; set KZ_SHARED_LOCAL"
  shared=$(dirname "$common")/.local
else
  shared=$top/.local
fi
if command -v cygpath >/dev/null 2>&1; then shared=$(cygpath -u "$shared"); fi
downloads=$shared/downloads/node
tools=$shared/tools/node
archive=$downloads/$file
install=$tools/$directory
executable=$install$bin/$exe
lock=$shared/locks/toolchain.lock

hash_of() { sha256sum "$1" | cut -c1-64; }
assert_archive() {
  actual=$(hash_of "$1")
  [ "$actual" = "$sha" ] || fail NODE_ARCHIVE_MISMATCH "$1 has SHA-256 $actual; the pin is $sha"
}
# Runs the binary only for the native platform; --platform verifies the layout without running it.
assert_install() {
  [ -f "$executable" ] || fail NODE_INSTALL_INVALID "$install exists without ${bin#/}${bin:+/}$exe; delete that folder and rerun"
  if [ "$target" = "$native" ] && [ -z "$platform" ]; then
    reported=$("$executable" --version 2>/dev/null || true)
    [ "$reported" = "v$version" ] || fail NODE_INSTALL_INVALID "$executable reports '$reported'; expected v$version"
  fi
}
path_line() {
  quoted=$(printf '%s' "$install$bin" | sed "s/'/'\\\\''/g")
  printf "export PATH='%s':\"\$PATH\"\n" "$quoted"
}

if [ "$check" = 1 ]; then
  [ -f "$archive" ] || fail NODE_ARCHIVE_MISSING "$archive is absent; run setup.sh"
  assert_archive "$archive"
  [ -d "$install" ] || fail NODE_INSTALL_MISSING "$install is absent; run setup.sh"
  assert_install
  say "Node $version ($target) verified in the shared tools."
  path_line
  exit 0
fi

if [ -d "$install" ]; then
  [ ! -f "$archive" ] || assert_archive "$archive"
  assert_install
  say "Node $version ($target) is already installed in the shared tools; nothing downloaded."
  path_line
  exit 0
fi

if [ "$target" = win32-x64 ]; then
  # Git Bash: Windows PowerShell owns the download and the zip extraction.
  command -v powershell.exe >/dev/null 2>&1 || fail PLATFORM_UNSUPPORTED 'powershell.exe is required on Windows'
  set -- -NoProfile -ExecutionPolicy Bypass -File "$(cygpath -w "$here/setup.ps1")"
  [ -z "$archive_arg" ] || set -- "$@" --archive "$(cygpath -w "$archive_arg")"
  if [ -n "${KZ_SHARED_LOCAL:-}" ]; then KZ_SHARED_LOCAL=$(cygpath -w "$KZ_SHARED_LOCAL"); export KZ_SHARED_LOCAL; fi
  powershell.exe "$@" >/dev/null || exit 1
  assert_archive "$archive"
  assert_install
  path_line
  exit 0
fi

command -v sha256sum >/dev/null 2>&1 || fail PLATFORM_UNSUPPORTED 'sha256sum is required'
command -v tar >/dev/null 2>&1 || fail PLATFORM_UNSUPPORTED 'tar with xz support is required'
mkdir -p "$downloads" "$tools" "$(dirname "$lock")"
nonce=$(date +%s)$$
part=$archive.$nonce.part
staging=$tools/.$directory.$nonce.tmp
( set -C; printf '{"nonce":"%s","pid":%s,"at":"%s"}' "$nonce" "$$" "$(date -u +%Y-%m-%dT%H:%M:%SZ)" > "$lock" ) 2>/dev/null \
  || fail TOOLCHAIN_LOCKED 'Another acquisition owns .local/locks/toolchain.lock in the shared root; interrupted locks require inspection'
cleanup() { rm -f "$part"; rm -rf "$staging"; rm -f "$lock"; }
trap cleanup EXIT
trap 'exit 1' HUP INT TERM

if [ -f "$archive" ]; then
  assert_archive "$archive"
  say 'Using the verified archive in the shared downloads.'
else
  if [ -n "$archive_arg" ]; then
    [ -f "$archive_arg" ] || fail NODE_ARCHIVE_MISSING "--archive $archive_arg does not exist"
    say "Seeding the shared downloads from $archive_arg."
    cp "$archive_arg" "$part"
  elif command -v curl >/dev/null 2>&1; then
    say "Downloading $url."
    curl --fail --silent --show-error --location --proto '=https' --tlsv1.2 --output "$part" "$url"
  elif command -v wget >/dev/null 2>&1; then
    say "Downloading $url."
    wget --quiet --https-only --output-document="$part" "$url"
  else
    fail PLATFORM_UNSUPPORTED 'curl or wget is required to download Node'
  fi
  assert_archive "$part"
  mv "$part" "$archive"
fi

mkdir "$staging"
tar -xJf "$archive" -C "$staging"
[ -f "$staging/$directory$bin/$exe" ] || fail NODE_ARCHIVE_LAYOUT "the archive does not contain $directory$bin/$exe"
mv "$staging/$directory" "$install"
assert_install
say "Installed Node $version ($target) into the shared tools."
path_line
