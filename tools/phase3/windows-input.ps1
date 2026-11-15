param([Parameter(Mandatory=$true)][string]$OwnedPids,
      [Parameter(Mandatory=$true)][string]$Nonce)
$ErrorActionPreference = 'Stop'
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
    public static void Key(ushort key,bool pressed,bool unicode=false) {
        var item=new Event {Type=1};
        item.Data.Keyboard=new Keyboard {Scan=unicode?key:(ushort)MapVirtualKey(key,0),
            Flags=(unicode?4u:8u)|(pressed?0u:2u)|(!unicode&&key>=37&&key<=40?1u:0u)};
        if(SendInput(1,new[]{item},Marshal.SizeOf<Event>())!=1)throw new Exception("SENDINPUT_REJECTED");
    }
    public static void MouseButton(bool pressed) {
        var item=new Event {Type=0}; item.Data.Mouse=new Mouse {Flags=pressed?2u:4u};
        if(SendInput(1,new[]{item},Marshal.SizeOf<Event>())!=1)throw new Exception("SENDINPUT_MOUSE_REJECTED");
    }
}
'@
$allowed = @{}
foreach ($text in $OwnedPids.Split(',')) {
    $ownedId = [int]$text
    if ($allowed.ContainsKey($ownedId)) { throw 'DRIVER_DUPLICATE_PID' }
    $subject = Get-Process -Id $ownedId -ErrorAction Stop
    $allowed[$ownedId] = @{ process=$subject; started=$subject.StartTime.ToUniversalTime().Ticks; window=[IntPtr]::Zero }
}
if ($allowed.Count -eq 0 -or -not [Guid]::TryParseExact($Nonce,'D',[ref]([Guid]::Empty))) { throw 'DRIVER_SCOPE' }
$held = @{}
$sequence = 0
$sink = $null
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
function Focus-Owned([IntPtr]$Target) {
    [void][LanDesktop]::SetForegroundWindow($Target)
    if ([LanDesktop]::GetForegroundWindow() -ne $Target) {
        [uint32]$foregroundId=0
        $foregroundThread=[LanDesktop]::GetWindowThreadProcessId([LanDesktop]::GetForegroundWindow(),[ref]$foregroundId)
        $currentThread=[LanDesktop]::GetCurrentThreadId()
        if ([LanDesktop]::AttachThreadInput($currentThread,$foregroundThread,$true)) {
            try { [void][LanDesktop]::SetForegroundWindow($Target) }
            finally { [void][LanDesktop]::AttachThreadInput($currentThread,$foregroundThread,$false) }
        }
    }
    if ([LanDesktop]::GetForegroundWindow() -ne $Target) { throw 'DRIVER_FOREGROUND_UNACKNOWLEDGED' }
}
function Deliver-Key([int]$SubjectId,[IntPtr]$Target,[string]$Method,[int]$Code,[bool]$Pressed) {
    if ($Code -lt 1 -or $Code -gt 255) { throw 'DRIVER_KEY' }
    $token="$SubjectId/$Method/$Code"
    if ($Pressed) { $held[$token]=@{ subject=$SubjectId; target=$Target; method=$Method; code=$Code } }
    if ($Method -eq 'physical-SendInput') {
        if ([LanDesktop]::GetForegroundWindow() -ne $Target) { throw 'DRIVER_PHYSICAL_NOT_FOREGROUND' }
        [LanDesktop]::Key($Code,$Pressed)
    } elseif ($Method -eq 'window-message') {
        [long]$parameter=1 -bor ([long][LanDesktop]::MapVirtualKey($Code,0) -shl 16)
        if ($Code -ge 37 -and $Code -le 40) { $parameter=$parameter -bor (1L -shl 24) }
        if (-not $Pressed) { $parameter=$parameter -bor (1L -shl 30) -bor (1L -shl 31) }
        $message=if ($Pressed) { 0x100 } else { 0x101 }
        if (-not [LanDesktop]::PostMessage($Target,$message,[UIntPtr]::new($Code),[IntPtr]::new($parameter))) { throw 'DRIVER_POSTMESSAGE_REJECTED' }
    } else { throw 'DRIVER_METHOD' }
    if (-not $Pressed) { $held.Remove($token) }
}
try {
    [Console]::Out.WriteLine((@{ready=$true;nonce=$Nonce;pid=$PID;owned=@($allowed.Keys);methods=@('physical-SendInput','window-message')} | ConvertTo-Json -Compress))
    while ($null -ne ($line=[Console]::In.ReadLine())) {
        try {
            $request=$line | ConvertFrom-Json
            if ($request.nonce -cne $Nonce -or [int]$request.sequence -ne $sequence+1) { throw 'DRIVER_REQUEST_IDENTITY' }
            $sequence++
            if ($request.action -eq 'close') { break }
            $subjectId=[int]$request.pid
            $window=Get-OwnedWindow $subjectId
            $method=[string]$request.method
            switch ($request.action) {
                'probe' { }
                'focus' { Focus-Owned $window }
                'key' { Deliver-Key $subjectId $window $method ([int]$request.key) ([bool]$request.pressed) }
                'text' {
                    if ([LanDesktop]::GetForegroundWindow() -ne $window -or $method -ne 'physical-SendInput') { throw 'DRIVER_TEXT_FOCUS' }
                    foreach ($character in ([string]$request.text).ToCharArray()) { [LanDesktop]::Key([int]$character,$true,$true); [LanDesktop]::Key([int]$character,$false,$true) }
                }
                'click' {
                    if ([LanDesktop]::GetForegroundWindow() -ne $window) { throw 'DRIVER_MOUSE_FOCUS' }
                    $point=[LanDesktop+Point]::new();$point.X=[int]$request.x;$point.Y=[int]$request.y
                    $rect=[LanDesktop+Rect]::new()
                    if (-not [LanDesktop]::GetClientRect($window,[ref]$rect) -or $rect.R-ne1280 -or $rect.B-ne720 -or $point.X-lt0 -or $point.X-ge1280 -or $point.Y-lt0 -or $point.Y-ge720) { throw 'DRIVER_MOUSE_BOUNDS' }
                    if (-not [LanDesktop]::ClientToScreen($window,[ref]$point) -or -not [LanDesktop]::SetCursorPos($point.X,$point.Y)) { throw 'DRIVER_MOUSE_POINT' }
                    [LanDesktop]::MouseButton($true); [LanDesktop]::MouseButton($false)
                }
                'focus-sink' {
                    Add-Type -AssemblyName System.Windows.Forms
                    if ($null -eq $sink) { $sink=[System.Windows.Forms.Form]::new();$sink.Text='Owned LAN focus observation';$sink.TopMost=$true;$sink.Show() }
                    $sink.Activate();[System.Windows.Forms.Application]::DoEvents();Focus-Owned $sink.Handle
                }
                default { throw 'DRIVER_ACTION' }
            }
            [Console]::Out.WriteLine((@{ok=$true;nonce=$Nonce;sequence=$sequence;pid=$subjectId;driverPid=$PID;window=$window.ToInt64();focused=([LanDesktop]::GetForegroundWindow()-eq$window);action=$request.action;method=$method;key=$request.key;pressed=$request.pressed;deliveryOnly=$true} | ConvertTo-Json -Compress))
        } catch { [Console]::Out.WriteLine((@{ok=$false;nonce=$Nonce;sequence=$sequence;error=$_.Exception.Message} | ConvertTo-Json -Compress)) }
    }
} finally {
    foreach ($record in @($held.Values)) {
        try {
            if ($record.method -eq 'physical-SendInput') { [LanDesktop]::Key($record.code,$false) }
            else { Deliver-Key $record.subject $record.target $record.method $record.code $false }
        } catch { [Console]::Error.WriteLine('DRIVER_RELEASE_FAILED '+$_.Exception.Message) }
    }
    if ($null -ne $sink) { $sink.Close();$sink.Dispose() }
}
