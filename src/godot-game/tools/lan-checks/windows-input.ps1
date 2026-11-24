param([Parameter(Mandatory=$true)][string]$OwnedPids,
      [Parameter(Mandatory=$true)][string]$Nonce)
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;
public static class LanDesktop {
    public delegate bool Visitor(IntPtr window, IntPtr data);
    [StructLayout(LayoutKind.Sequential)] public struct Point { public int X; public int Y; }
    [StructLayout(LayoutKind.Sequential)] public struct Rect { public int L; public int T; public int R; public int B; }
    [StructLayout(LayoutKind.Sequential)] public struct Mouse { public int X,Y; public uint Data,Flags,Time; public UIntPtr Extra; }
    [StructLayout(LayoutKind.Sequential)] public struct Keyboard { public ushort Key,Scan; public uint Flags,Time; public UIntPtr Extra; }
    [StructLayout(LayoutKind.Explicit)] public struct Payload {
        [FieldOffset(0)] public Mouse Mouse; [FieldOffset(0)] public Keyboard Keyboard;
    }
    [StructLayout(LayoutKind.Sequential)] public struct Event { public uint Type; public Payload Data; }
    [DllImport("user32.dll")] public static extern bool EnumWindows(Visitor visit,IntPtr data);
    [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr window,out uint pid);
    [DllImport("user32.dll")] public static extern bool IsWindowVisible(IntPtr window);
    [DllImport("user32.dll")] public static extern bool IsWindow(IntPtr window);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr window);
    [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")] public static extern bool AttachThreadInput(uint from,uint to,bool attach);
    [DllImport("kernel32.dll")] public static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] public static extern uint MapVirtualKey(uint key,uint map);
    [DllImport("user32.dll",SetLastError=true)] public static extern uint SendInput(uint count,Event[] events,int size);
    [DllImport("user32.dll",SetLastError=true)] public static extern bool PostMessage(IntPtr window,uint message,UIntPtr key,IntPtr parameter);
    [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr window,out Rect rect);
    [DllImport("user32.dll")] public static extern bool ClientToScreen(IntPtr window,ref Point point);
    [DllImport("user32.dll")] public static extern bool SetCursorPos(int x,int y);
    [DllImport("user32.dll")] public static extern IntPtr WindowFromPoint(Point point);
    [DllImport("user32.dll")] public static extern IntPtr GetAncestor(IntPtr window,uint flags);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr window,out Rect rect);
    [DllImport("user32.dll")] public static extern bool SetWindowPos(IntPtr window,IntPtr after,int x,int y,int width,int height,uint flags);
    [DllImport("user32.dll")] public static extern IntPtr GetWindowLongPtr(IntPtr window,int index);
    [DllImport("user32.dll",SetLastError=true)] public static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
    [DllImport("user32.dll")] public static extern IntPtr GetThreadDpiAwarenessContext();
    [DllImport("user32.dll")] public static extern bool AreDpiAwarenessContextsEqual(IntPtr first,IntPtr second);
    public static void PhysicalCoordinates() {
        var context=new IntPtr(-4);
        if(SetThreadDpiAwarenessContext(context)==IntPtr.Zero ||
            !AreDpiAwarenessContextsEqual(GetThreadDpiAwarenessContext(),context))throw new Exception("DRIVER_DPI_CONTEXT");
    }
    public static void Key(ushort key,bool pressed,bool unicode=false) {
        var item=new Event {Type=1};
        item.Data.Keyboard=new Keyboard {Scan=unicode?key:(ushort)MapVirtualKey(key,0),
            Flags=(unicode?4u:8u)|(pressed?0u:2u)|(!unicode&&((key>=33&&key<=40)||key==45||key==46)?1u:0u)};
        if(SendInput(1,new[]{item},Marshal.SizeOf<Event>())!=1)throw new Exception("SENDINPUT_REJECTED");
    }
    public static void MouseButton(bool pressed) { MouseButton(pressed,false); }
    public static void MouseButton(bool pressed,bool right) {
        var item=new Event {Type=0}; item.Data.Mouse=new Mouse {Flags=right?(pressed?8u:16u):(pressed?2u:4u)};
        if(SendInput(1,new[]{item},Marshal.SizeOf<Event>())!=1)throw new Exception("SENDINPUT_MOUSE_REJECTED");
    }
}
'@
[LanDesktop]::PhysicalCoordinates()
$allowed = @{}
foreach ($text in $OwnedPids.Split(',')) {
    $ownedId = [int]$text
    if ($allowed.ContainsKey($ownedId)) { throw 'DRIVER_DUPLICATE_PID' }
    $subject = Get-Process -Id $ownedId -ErrorAction Stop
    $allowed[$ownedId] = @{ process=$subject; started=$subject.StartTime.ToUniversalTime().Ticks; window=[IntPtr]::Zero }
}
if ($allowed.Count -eq 0 -or -not [Guid]::TryParseExact($Nonce,'D',[ref]([Guid]::Empty))) { throw 'DRIVER_SCOPE' }
$held = @{}
$mouseHeld = $false
$rightHeld = $false
$sequence = 0
Add-Type -AssemblyName System.Windows.Forms
$sink = [System.Windows.Forms.Form]::new()
$sink.Text = 'Owned LAN focus observation'
$sink.TopMost = $true
[void]$sink.Handle
function Get-OwnedWindow([int]$SubjectId) {
    if (-not $allowed.ContainsKey($SubjectId)) { throw 'DRIVER_FOREIGN_PID' }
    $record = $allowed[$SubjectId]
    $record.process.Refresh()
    if ($record.process.HasExited -or $record.process.StartTime.ToUniversalTime().Ticks -ne $record.started) { throw 'DRIVER_PID_RECYCLED' }
    if ($record.window -eq [IntPtr]::Zero) {
        $script:foundWindow = [IntPtr]::Zero
        [void][LanDesktop]::EnumWindows({param($candidate,$data)
            [uint32]$candidateId = 0
            [void][LanDesktop]::GetWindowThreadProcessId($candidate,[ref]$candidateId)
            if ($candidateId -eq $SubjectId -and [LanDesktop]::IsWindowVisible($candidate)) { $script:foundWindow=$candidate; return $false }
            return $true
        },[IntPtr]::Zero)
        $record.window=$script:foundWindow
    }
    [uint32]$actualId = 0
    [void][LanDesktop]::GetWindowThreadProcessId($record.window,[ref]$actualId)
    if ($record.window -eq [IntPtr]::Zero -or -not [LanDesktop]::IsWindow($record.window) -or $actualId -ne $SubjectId) { throw 'DRIVER_WINDOW_OWNERSHIP' }
    return $record.window
}
function Wait-Foreground([IntPtr]$Target,[int]$Milliseconds) {
    $elapsed=[Diagnostics.Stopwatch]::StartNew()
    do {
        if ([LanDesktop]::GetForegroundWindow() -eq $Target) { return $true }
        Start-Sleep -Milliseconds 10
    } while ($elapsed.ElapsedMilliseconds -lt $Milliseconds)
    return ([LanDesktop]::GetForegroundWindow() -eq $Target)
}
function Focus-Owned([IntPtr]$Target) {
    if ([LanDesktop]::GetForegroundWindow() -eq $Target) { return }
    $wasTopmost=([LanDesktop]::GetWindowLongPtr($Target,-20).ToInt64() -band 8) -ne 0
    try { Focus-Reachable $Target }
    finally {
        if (-not $wasTopmost -and ([LanDesktop]::GetWindowLongPtr($Target,-20).ToInt64() -band 8) -ne 0) {
            if (-not [LanDesktop]::SetWindowPos($Target,[IntPtr]::new(-2),0,0,0,0,0x13)) { throw 'DRIVER_FOCUS_ORDER_RESTORE' }
        }
    }
}
function Focus-Reachable([IntPtr]$Target) {
    if ([LanDesktop]::GetForegroundWindow() -eq $Target) { return }
    [void][LanDesktop]::SetForegroundWindow($Target)
    if (Wait-Foreground $Target 100) { return }
    if (-not [LanDesktop]::SetWindowPos($Target,[IntPtr]::new(-1),0,0,0,0,0x53)) { throw 'DRIVER_FOCUS_RAISE' }
    if ([LanDesktop]::GetForegroundWindow() -ne $Target) {
        [uint32]$foregroundId=0
        $foregroundThread=[LanDesktop]::GetWindowThreadProcessId([LanDesktop]::GetForegroundWindow(),[ref]$foregroundId)
        $currentThread=[LanDesktop]::GetCurrentThreadId()
        if ($foregroundThread -ne 0 -and $foregroundThread -ne $currentThread -and [LanDesktop]::AttachThreadInput($currentThread,$foregroundThread,$true)) {
            try { [void][LanDesktop]::SetForegroundWindow($Target) }
            finally { [void][LanDesktop]::AttachThreadInput($currentThread,$foregroundThread,$false) }
        }
    }
    if (Wait-Foreground $Target 100) { return }
    $outer=[LanDesktop+Rect]::new()
    if (-not [LanDesktop]::GetWindowRect($Target,[ref]$outer)) { throw 'DRIVER_FOCUS_RECT' }
    $clientOrigin=[LanDesktop+Point]::new()
    if (-not [LanDesktop]::ClientToScreen($Target,[ref]$clientOrigin) -or $clientOrigin.Y -le $outer.T) { throw 'DRIVER_FOCUS_CAPTION' }
    $captionHeight=$clientOrigin.Y-$outer.T
    $captionOffset=[Math]::Min(($outer.R-$outer.L)/3,[Math]::Max(50,$captionHeight*2))
    $titlePoint=[LanDesktop+Point]::new();$titlePoint.X=$clientOrigin.X+[int]$captionOffset;$titlePoint.Y=$outer.T+[int]($captionHeight/2)
    $pointRoot=[LanDesktop]::GetAncestor([LanDesktop]::WindowFromPoint($titlePoint),2)
    if ($pointRoot -ne $Target) { throw "DRIVER_FOCUS_POINT_NOT_OWNED target=$Target actual=$pointRoot point=$($titlePoint.X),$($titlePoint.Y)" }
    if (-not [LanDesktop]::SetCursorPos($titlePoint.X,$titlePoint.Y)) { throw 'DRIVER_FOCUS_CURSOR' }
    $script:mouseHeld=$true
    [LanDesktop]::MouseButton($true);[LanDesktop]::MouseButton($false)
    $script:mouseHeld=$false
    if (Wait-Foreground $Target 150) { return }
    if ([LanDesktop]::GetForegroundWindow() -ne $Target) { throw 'DRIVER_FOREGROUND_UNACKNOWLEDGED' }
}
function Deliver-Key([int]$SubjectId,[IntPtr]$Target,[string]$Method,[int]$Code,[bool]$Pressed) {
    if ($Code -lt 1 -or $Code -gt 255) { throw 'DRIVER_KEY' }
    $token="$SubjectId/$Method/$Code"
    $ownedRelease=(-not $Pressed -and $held.ContainsKey($token))
    if ($Pressed) { $held[$token]=@{ subject=$SubjectId; target=$Target; method=$Method; code=$Code } }
    if ($Method -eq 'physical-SendInput') {
        if ([LanDesktop]::GetForegroundWindow() -ne $Target -and -not $ownedRelease) { throw 'DRIVER_PHYSICAL_NOT_FOREGROUND' }
        [LanDesktop]::Key($Code,$Pressed)
    } elseif ($Method -eq 'window-message') {
        [long]$parameter=1 -bor ([long][LanDesktop]::MapVirtualKey($Code,0) -shl 16)
        if (($Code -ge 33 -and $Code -le 40) -or $Code -eq 45 -or $Code -eq 46) { $parameter=$parameter -bor (1L -shl 24) }
        if (-not $Pressed) { $parameter=$parameter -bor (1L -shl 30) -bor (1L -shl 31) }
        $message=if ($Pressed) { 0x100 } else { 0x101 }
        if (-not [LanDesktop]::PostMessage($Target,$message,[UIntPtr]::new($Code),[IntPtr]::new($parameter))) { throw 'DRIVER_POSTMESSAGE_REJECTED' }
    } else { throw 'DRIVER_METHOD' }
    if (-not $Pressed) { $held.Remove($token) }
}
function Move-OwnedPointer([int]$SubjectId,[IntPtr]$Target,[int]$X,[int]$Y) {
    if ([LanDesktop]::GetForegroundWindow() -ne $Target) { throw 'DRIVER_MOUSE_FOCUS' }
    if ($X -lt 0 -or $X -ge 1280 -or $Y -lt 0 -or $Y -ge 720) { throw 'DRIVER_MOUSE_BOUNDS' }
    $point=[LanDesktop+Point]::new(); $point.X=$X; $point.Y=$Y
    if (-not [LanDesktop]::ClientToScreen($Target,[ref]$point)) { throw 'DRIVER_MOUSE_POINT' }
    [uint32]$pointOwner=0
    [void][LanDesktop]::GetWindowThreadProcessId([LanDesktop]::GetAncestor([LanDesktop]::WindowFromPoint($point),2),[ref]$pointOwner)
    if ($pointOwner -ne $SubjectId) { throw 'DRIVER_MOUSE_POINT_NOT_OWNED' }
    if (-not [LanDesktop]::SetCursorPos($point.X,$point.Y)) { throw 'DRIVER_MOUSE_POINT' }
}
try {
    [Console]::Out.WriteLine((@{ready=$true;nonce=$Nonce;pid=$PID;owned=@($allowed.Keys);methods=@('physical-SendInput','window-message')} | ConvertTo-Json -Compress))
    while ($null -ne ($line=[Console]::In.ReadLine())) {
        try {
            $request=$line | ConvertFrom-Json
            if ($request.nonce -cne $Nonce -or [int]$request.sequence -ne $sequence+1) { throw 'DRIVER_REQUEST_IDENTITY' }
            $sequence++
            if ($request.action -eq 'close') { break }
            [LanDesktop]::PhysicalCoordinates()
            $subjectId=[int]$request.pid
            $window=Get-OwnedWindow $subjectId
            $client=[LanDesktop+Rect]::new()
            if (-not [LanDesktop]::GetClientRect($window,[ref]$client) -or $client.R-$client.L-ne1280 -or $client.B-$client.T-ne720) { throw 'DRIVER_PHYSICAL_CLIENT_RESOLUTION' }
            $method=[string]$request.method
            switch ($request.action) {
                'probe' { }
                'focus' { if ($null -ne $sink) { $sink.Hide() }; Focus-Owned $window }
                'key' { Deliver-Key $subjectId $window $method ([int]$request.key) ([bool]$request.pressed) }
                'tap' {
                    if ($method -ne 'physical-SendInput') { throw 'DRIVER_TAP_METHOD' }
                    $code=[int]$request.key
                    Deliver-Key $subjectId $window $method $code $true
                    Start-Sleep -Milliseconds 30
                    Deliver-Key $subjectId $window $method $code $false
                }
                'reversal' {
                    if ($method -ne 'physical-SendInput' -or [LanDesktop]::GetForegroundWindow() -ne $window) { throw 'DRIVER_REVERSAL_FOCUS' }
                    Deliver-Key $subjectId $window $method ([int]$request.releaseKey) $false
                    Deliver-Key $subjectId $window $method ([int]$request.key) $true
                }
                'text' {
                    if ([LanDesktop]::GetForegroundWindow() -ne $window -or $method -ne 'physical-SendInput') { throw 'DRIVER_TEXT_FOCUS' }
                    $typed=0
                    foreach ($character in ([string]$request.text).ToCharArray()) {
                        if ([LanDesktop]::GetForegroundWindow() -ne $window) { throw 'DRIVER_TEXT_FOCUS_CHANGED' }
                        [LanDesktop]::Key([int]$character,$true,$true); [LanDesktop]::Key([int]$character,$false,$true)
                        $typed++
                        if ($typed % 4 -eq 0) { Start-Sleep -Milliseconds 16 }
                    }
                }
                'click' {
                    if ([LanDesktop]::GetForegroundWindow() -ne $window) { throw 'DRIVER_MOUSE_FOCUS' }
                    $point=[LanDesktop+Point]::new();$point.X=[int]$request.x;$point.Y=[int]$request.y
                    $rect=[LanDesktop+Rect]::new()
                    if (-not [LanDesktop]::GetClientRect($window,[ref]$rect) -or $rect.R-ne1280 -or $rect.B-ne720 -or $point.X-lt0 -or $point.X-ge1280 -or $point.Y-lt0 -or $point.Y-ge720) { throw 'DRIVER_MOUSE_BOUNDS' }
                    if (-not [LanDesktop]::ClientToScreen($window,[ref]$point) -or -not [LanDesktop]::SetCursorPos($point.X,$point.Y)) { throw 'DRIVER_MOUSE_POINT' }
                    [uint32]$pointOwner=0
                    [void][LanDesktop]::GetWindowThreadProcessId([LanDesktop]::GetAncestor([LanDesktop]::WindowFromPoint($point),2),[ref]$pointOwner)
                    if ($pointOwner -ne $subjectId) { throw 'DRIVER_MOUSE_POINT_NOT_OWNED' }
                    $mouseHeld=$true
                    [LanDesktop]::MouseButton($true); [LanDesktop]::MouseButton($false)
                    $mouseHeld=$false
                }
                'pointer' {
                    if ($method -ne 'physical-SendInput' -or -not ($request.x -is [ValueType]) -or
                        -not ($request.y -is [ValueType])) { throw 'DRIVER_POINTER_METHOD' }
                    Move-OwnedPointer $subjectId $window ([int]$request.x) ([int]$request.y)
                    if ($request.button -in 'left','right' -and $request.pressed -is [bool]) {
                        $pressed=[bool]$request.pressed
                        $isRight=$request.button -eq 'right'
                        if ($isRight -and $pressed -eq $rightHeld -or -not $isRight -and $pressed -eq $mouseHeld) {
                            throw 'DRIVER_MOUSE_UNBALANCED'
                        }
                        [LanDesktop]::MouseButton($pressed,$isRight)
                        if ($isRight) { $rightHeld=$pressed } else { $mouseHeld=$pressed }
                    } elseif ($null -ne $request.button -or $null -ne $request.pressed) { throw 'DRIVER_MOUSE_BUTTON' }
                }
                'focus-sink' {
                    $sink.Show()
                    $sink.Activate();[System.Windows.Forms.Application]::DoEvents();Focus-Owned $sink.Handle
                }
                default { throw 'DRIVER_ACTION' }
            }
            [Console]::Out.WriteLine((@{ok=$true;nonce=$Nonce;sequence=$sequence;pid=$subjectId;driverPid=$PID;window=$window.ToInt64();focused=([LanDesktop]::GetForegroundWindow()-eq$window);action=$request.action;method=$method;key=$request.key;pressed=$request.pressed;releaseKey=$request.releaseKey;x=$request.x;y=$request.y;button=$request.button;deliveryOnly=$true;coordinateSpace='physical-pixels';dpiAwareness='per-monitor-v2';clientWidth=$client.R-$client.L;clientHeight=$client.B-$client.T} | ConvertTo-Json -Compress))
        } catch { [Console]::Out.WriteLine((@{ok=$false;nonce=$Nonce;sequence=$sequence;error=$_.Exception.Message} | ConvertTo-Json -Compress)) }
    }
} finally {
    if ($mouseHeld) {
        try { [LanDesktop]::MouseButton($false) }
        catch { [Console]::Error.WriteLine('DRIVER_MOUSE_RELEASE_FAILED '+$_.Exception.Message) }
    }
    if ($rightHeld) {
        try { [LanDesktop]::MouseButton($false,$true) }
        catch { [Console]::Error.WriteLine('DRIVER_MOUSE_RELEASE_FAILED '+$_.Exception.Message) }
    }
    foreach ($record in @($held.Values)) {
        try {
            if ($record.method -eq 'physical-SendInput') { [LanDesktop]::Key($record.code,$false) }
            else { Deliver-Key $record.subject $record.target $record.method $record.code $false }
        } catch { [Console]::Error.WriteLine('DRIVER_RELEASE_FAILED '+$_.Exception.Message) }
    }
    if ($null -ne $sink) { $sink.Close();$sink.Dispose() }
}
