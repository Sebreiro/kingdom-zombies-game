param([int]$OwnedPid, [ValidateSet('probe','click','down','up','tap','focus-cycle','move','mouse-down','mouse-up')][string]$Action,
      [int]$X = 0, [int]$Y = 0, [byte]$Key = 0)
$ErrorActionPreference = 'Stop'
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class FoundationDesktop {
    public delegate bool WindowVisitor(IntPtr window, IntPtr data);
    [StructLayout(LayoutKind.Sequential)] public struct Point { public int X; public int Y; }
    [StructLayout(LayoutKind.Sequential)] public struct Rect { public int L; public int T; public int R; public int B; }
    [DllImport("user32.dll")] public static extern bool EnumWindows(WindowVisitor callback, IntPtr data);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr window, out uint pid);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr window, out Rect rect);
    [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr window, ref Point point);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x, int y);
    [DllImport("user32.dll")] public static extern void mouse_event(uint flags, uint x, uint y, uint data, UIntPtr extra);
    [DllImport("user32.dll")] public static extern void keybd_event(byte key, byte scan, uint flags, UIntPtr extra);
    [DllImport("user32.dll")] public static extern uint MapVirtualKey(uint key, uint mapType);
    [DllImport("kernel32.dll")] public static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] public static extern bool AttachThreadInput(uint from, uint to, bool attach);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr window, out Rect rect);
    [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(Point point);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr window, uint flags);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetWindowText(IntPtr window, System.Text.StringBuilder text, int count);
    [DllImport("user32.dll")] public static extern IntPtr GetWindowLongPtr(IntPtr window, int index);
}
'@
# Names the root window at a screen point, so a point-ownership failure identifies what covers the owned window.
function Describe-PointWindow([FoundationDesktop+Point]$Point) {
    $root = [FoundationDesktop]::GetAncestor([FoundationDesktop]::WindowFromPoint($Point), 2)
    [uint32]$ownerId = 0
    [void][FoundationDesktop]::GetWindowThreadProcessId($root, [ref]$ownerId)
    $title = [System.Text.StringBuilder]::new(256)
    [void][FoundationDesktop]::GetWindowText($root, $title, $title.Capacity)
    $rect = [FoundationDesktop+Rect]::new()
    [void][FoundationDesktop]::GetWindowRect($root, [ref]$rect)
    $name = (Get-Process -Id $ownerId -ErrorAction SilentlyContinue).ProcessName
    $topmost = ([FoundationDesktop]::GetWindowLongPtr($root, -20).ToInt64() -band 8) -ne 0
    return "point=$($Point.X),$($Point.Y) covering window=$root pid=$ownerId process=$name " +
        "title='$($title.ToString())' rect=$($rect.L),$($rect.T),$($rect.R),$($rect.B) topmost=$topmost"
}
$owned = Get-Process -Id $OwnedPid -ErrorAction Stop
$script:window = [IntPtr]::Zero
[FoundationDesktop]::EnumWindows({ param($candidate, $data)
    [uint32]$candidatePid = 0
    [void][FoundationDesktop]::GetWindowThreadProcessId($candidate, [ref]$candidatePid)
    if ($candidatePid -eq $OwnedPid -and [FoundationDesktop]::IsWindowVisible($candidate)) {
        $script:window = $candidate
        return $false
    }
    return $true
}, [IntPtr]::Zero) | Out-Null
if ($window -eq [IntPtr]::Zero) { throw 'OWNED_WINDOW_MISSING' }
[void][FoundationDesktop]::SetForegroundWindow($window)
Start-Sleep -Milliseconds 100
if ([FoundationDesktop]::GetForegroundWindow() -ne $window) {
    [uint32]$foregroundPid = 0
    $foregroundThread = [FoundationDesktop]::GetWindowThreadProcessId([FoundationDesktop]::GetForegroundWindow(), [ref]$foregroundPid)
    $currentThread = [FoundationDesktop]::GetCurrentThreadId()
    if ($foregroundThread -ne $currentThread -and [FoundationDesktop]::AttachThreadInput($currentThread, $foregroundThread, $true)) {
        try { [void][FoundationDesktop]::SetForegroundWindow($window) }
        finally { [void][FoundationDesktop]::AttachThreadInput($currentThread, $foregroundThread, $false) }
    }
    Start-Sleep -Milliseconds 100
}
if ([FoundationDesktop]::GetForegroundWindow() -ne $window) {
    $outer = [FoundationDesktop+Rect]::new()
    if (![FoundationDesktop]::GetWindowRect($window, [ref]$outer)) { throw 'OWNED_WINDOW_RECT_MISSING' }
    $titlePoint = [FoundationDesktop+Point]::new()
    $titlePoint.X = $outer.L + 50
    $titlePoint.Y = $outer.T + 10
    $pointWindow = [FoundationDesktop]::WindowFromPoint($titlePoint)
    if ([FoundationDesktop]::GetAncestor($pointWindow, 2) -eq $window) {
        [void][FoundationDesktop]::SetCursorPos($titlePoint.X, $titlePoint.Y)
        [FoundationDesktop]::mouse_event(2, 0, 0, 0, [UIntPtr]::Zero)
        [FoundationDesktop]::mouse_event(4, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 150
    }
}
if ([FoundationDesktop]::GetForegroundWindow() -ne $window) {
    [uint32]$actualPid = 0
    $actualWindow = [FoundationDesktop]::GetForegroundWindow()
    [void][FoundationDesktop]::GetWindowThreadProcessId($actualWindow, [ref]$actualPid)
    throw "OWNED_FOCUS_UNACKNOWLEDGED expectedWindow=$window expectedPid=$OwnedPid actualWindow=$actualWindow actualPid=$actualPid"
}
[uint32]$verifiedPid = 0
[void][FoundationDesktop]::GetWindowThreadProcessId($window, [ref]$verifiedPid)
if ($verifiedPid -ne $OwnedPid -or $owned.HasExited) { throw 'OWNED_WINDOW_CHANGED' }
$rect = [FoundationDesktop+Rect]::new()
if (![FoundationDesktop]::GetClientRect($window, [ref]$rect)) { throw 'CLIENT_RECT_MISSING' }
switch ($Action) {
    { $_ -in 'click','move','mouse-down','mouse-up' } {
        $point = [FoundationDesktop+Point]::new()
        $point.X = [int]($X * ($rect.R - $rect.L) / 1280)
        $point.Y = [int]($Y * ($rect.B - $rect.T) / 720)
        if (![FoundationDesktop]::ClientToScreen($window, [ref]$point)) { throw 'CLIENT_POINT_MISSING' }
        if (![FoundationDesktop]::SetCursorPos($point.X, $point.Y)) { throw 'CURSOR_FAILED' }
        if ($Action -in 'click','mouse-down') { [FoundationDesktop]::mouse_event(2, 0, 0, 0, [UIntPtr]::Zero) }
        if ($Action -eq 'click') { Start-Sleep -Milliseconds 80 }
        if ($Action -in 'click','mouse-up') { [FoundationDesktop]::mouse_event(4, 0, 0, 0, [UIntPtr]::Zero) }
    }
    'down' { [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), [uint32]([int]($Key -ge 37 -and $Key -le 40)), [UIntPtr]::Zero) }
    'up' { [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), [uint32](2 + [int]($Key -ge 37 -and $Key -le 40)), [UIntPtr]::Zero) }
    'tap' {
        [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 2, [UIntPtr]::Zero)
    }
    'focus-cycle' {
        Add-Type -AssemblyName System.Windows.Forms
        $sink = [System.Windows.Forms.Form]::new()
        $sink.Text = 'Owned foundation focus check'
        $sink.TopMost = $true
        try {
            $sink.Show()
            $sink.Activate()
            [System.Windows.Forms.Application]::DoEvents()
            if ([FoundationDesktop]::GetForegroundWindow() -ne $sink.Handle) {
                $sinkRect = [FoundationDesktop+Rect]::new()
                if (![FoundationDesktop]::GetWindowRect($sink.Handle, [ref]$sinkRect)) { throw 'SINK_RECT_MISSING' }
                $sinkPoint = [FoundationDesktop+Point]::new()
                $sinkPoint.X = $sinkRect.L + 50
                $sinkPoint.Y = $sinkRect.T + 10
                if ([FoundationDesktop]::GetAncestor([FoundationDesktop]::WindowFromPoint($sinkPoint), 2) -ne $sink.Handle) {
                    throw "SINK_POINT_NOT_OWNED sink=$($sink.Handle) $(Describe-PointWindow $sinkPoint)"
                }
                [void][FoundationDesktop]::SetCursorPos($sinkPoint.X, $sinkPoint.Y)
                [FoundationDesktop]::mouse_event(2, 0, 0, 0, [UIntPtr]::Zero)
                [FoundationDesktop]::mouse_event(4, 0, 0, 0, [UIntPtr]::Zero)
                [System.Windows.Forms.Application]::DoEvents()
            }
            Start-Sleep -Milliseconds 500
            if ([FoundationDesktop]::GetForegroundWindow() -ne $sink.Handle) { throw 'FOCUS_DID_NOT_REACH_OWNED_SINK' }
        } finally { $sink.Close(); $sink.Dispose() }
        [void][FoundationDesktop]::SetForegroundWindow($window)
        Start-Sleep -Milliseconds 150
        if ([FoundationDesktop]::GetForegroundWindow() -ne $window) { throw 'FOCUS_DID_NOT_RETURN' }
    }
}
@{ pid=$OwnedPid; window=$window.ToInt64(); focused=$true; action=$Action; key=$Key;
   width=$rect.R-$rect.L; height=$rect.B-$rect.T } | ConvertTo-Json -Compress
