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
geometry=$(xdotool getwindowgeometry --shell "$window")
width=$(printf '%s\n' "$geometry" | sed -n 's/^WIDTH=//p')
height=$(printf '%s\n' "$geometry" | sed -n 's/^HEIGHT=//p')
scaled_x=$((x * width / 1280))
scaled_y=$((y * height / 720))
case "$action" in
  probe) ;;
  click) xdotool mousemove --window "$window" "$scaled_x" "$scaled_y" click 1 ;;
  move) xdotool mousemove --window "$window" "$scaled_x" "$scaled_y" ;;
  mouse-down) xdotool mousemove --window "$window" "$scaled_x" "$scaled_y" mousedown 1 ;;
  mouse-up) xdotool mousemove --window "$window" "$scaled_x" "$scaled_y" mouseup 1 ;;
  down) xdotool keydown "$key" ;;
  up) xdotool keyup "$key" ;;
  tap) xdotool key "$key" ;;
  focus-cycle)
    xdotool windowminimize "$window"
    sleep 0.5
    test "$(xdotool getwindowfocus)" != "$window"
    xdotool windowmap "$window"
    xdotool windowactivate --sync "$window"
    test "$(xdotool getwindowfocus)" = "$window"
    ;;
  *) exit 2 ;;
esac
printf '{"pid":%s,"window":%s,"focused":true,"action":"%s","width":%s,"height":%s}\n' "$owned_pid" "$window" "$action" "$width" "$height"
