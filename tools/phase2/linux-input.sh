#!/bin/sh
set -eu
owned_pid=$1
action=$2
x=${3:-0}
y=${4:-0}
key=${5:-0}
command -v xdotool >/dev/null
test -n "${DISPLAY:-}"
kill -0 "$owned_pid"
window=$(xdotool search --onlyvisible --pid "$owned_pid" | head -n 1)
test -n "$window"
test "$(xdotool getwindowpid "$window")" = "$owned_pid"
xdotool windowactivate --sync "$window"
test "$(xdotool getwindowfocus)" = "$window"
case "$action" in
  probe) ;;
  click) xdotool mousemove --window "$window" "$x" "$y" click 1 ;;
  down) xdotool keydown "$key" ;;
  up) xdotool keyup "$key" ;;
  tap) xdotool key "$key" ;;
  *) exit 2 ;;
esac
printf '{"pid":%s,"window":%s,"focused":true,"action":"%s"}\n' "$owned_pid" "$window" "$action"
