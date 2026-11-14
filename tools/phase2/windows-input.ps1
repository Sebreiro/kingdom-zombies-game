param([int]$OwnedPid, [ValidateSet('probe','click','down','up','tap')][string]$Action,
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
}
'@
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
if ([FoundationDesktop]::GetForegroundWindow() -ne $window) { throw 'OWNED_FOCUS_UNACKNOWLEDGED' }
[uint32]$verifiedPid = 0
[void][FoundationDesktop]::GetWindowThreadProcessId($window, [ref]$verifiedPid)
if ($verifiedPid -ne $OwnedPid -or $owned.HasExited) { throw 'OWNED_WINDOW_CHANGED' }
$rect = [FoundationDesktop+Rect]::new()
if (![FoundationDesktop]::GetClientRect($window, [ref]$rect)) { throw 'CLIENT_RECT_MISSING' }
switch ($Action) {
    'click' {
        $point = [FoundationDesktop+Point]::new()
        $point.X = [int]($X * ($rect.R - $rect.L) / 1280)
        $point.Y = [int]($Y * ($rect.B - $rect.T) / 720)
        if (![FoundationDesktop]::ClientToScreen($window, [ref]$point)) { throw 'CLIENT_POINT_MISSING' }
        if (![FoundationDesktop]::SetCursorPos($point.X, $point.Y)) { throw 'CURSOR_FAILED' }
        [FoundationDesktop]::mouse_event(2, 0, 0, 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [FoundationDesktop]::mouse_event(4, 0, 0, 0, [UIntPtr]::Zero)
    }
    'down' { [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 0, [UIntPtr]::Zero) }
    'up' { [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 2, [UIntPtr]::Zero) }
    'tap' {
        [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 0, [UIntPtr]::Zero)
        Start-Sleep -Milliseconds 80
        [FoundationDesktop]::keybd_event($Key, [byte][FoundationDesktop]::MapVirtualKey($Key, 0), 2, [UIntPtr]::Zero)
    }
}
@{ pid=$OwnedPid; window=$window.ToInt64(); focused=$true; action=$Action; key=$Key;
   width=$rect.R-$rect.L; height=$rect.B-$rect.T } | ConvertTo-Json -Compress
