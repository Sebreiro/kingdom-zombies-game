#!/bin/sh
# SessionStart hook: put the repository's shared Node on PATH for this agent session.
# Claude Code: appends one export line to $CLAUDE_ENV_FILE. Codex (--codex): prints it as session context.
# Fail-open: any problem exits 0 and changes nothing.
mode=${1:-claude}
project=${CLAUDE_PROJECT_DIR:-.}
# Git for Windows is a native program: with MSYS_NO_PATHCONV set it cannot read an MSYS /c/... path.
if command -v cygpath >/dev/null 2>&1; then project=$(cygpath -m "$project"); fi
top=$(git -C "$project" rev-parse --show-toplevel 2>/dev/null) || exit 0
version=$(sed -n 's/^ *"node": *"\([0-9.]*\)".*/\1/p' "$top/src/godot-game/tools/verification/toolchain.json" 2>/dev/null | head -n 1)
[ -n "$version" ] || exit 0
if [ -n "${KZ_SHARED_LOCAL:-}" ]; then
  shared=$KZ_SHARED_LOCAL
else
  common=$(git -C "$top" rev-parse --path-format=absolute --git-common-dir 2>/dev/null) || exit 0
  shared=$(dirname "$common")/.local
fi
case $(uname -s) in
  MINGW*|MSYS*|CYGWIN*) dir=$shared/tools/node/node-v$version-win-x64 exe=node.exe ;;
  Linux) dir=$shared/tools/node/node-v$version-linux-x64/bin exe=node ;;
  *) exit 0 ;;
esac
[ -f "$dir/$exe" ] || exit 0
if command -v cygpath >/dev/null 2>&1; then dir=$(cygpath -u "$dir"); fi
quoted=$(printf '%s' "$dir" | sed "s/'/'\\\\''/g")
line="export PATH='$quoted':\"\$PATH\""
if [ "$mode" = --codex ]; then
  printf 'Shared Node %s for this repository. In each shell command run first: %s\n' "$version" "$line"
  exit 0
fi
[ -n "${CLAUDE_ENV_FILE:-}" ] || exit 0
grep -qxF "$line" "$CLAUDE_ENV_FILE" 2>/dev/null || printf '%s\n' "$line" >> "$CLAUDE_ENV_FILE"
exit 0
