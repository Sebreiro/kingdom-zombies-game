# Installs the pinned Node (toolchain.json nodeArchives.win32-x64) once per repository into the shared
# .local/tools/node/ and prints one PowerShell PATH line on stdout. Windows PowerShell 5.1 or later.
# Usage: setup.ps1 [--archive <verified local zip>] [--check]
#   --archive  seed the shared downloads from a local copy instead of downloading; it must match the pin.
#   --check    verify the pinned archive and the installed Node; download and write nothing.
# The archive SHA-256 is verified before extraction. Downloads and extraction go to nonce-named siblings that are
# renamed into place, under the shared .local/locks/toolchain.lock. A second run downloads nothing.
# Messages go to stderr; stdout carries only the PATH line. KZ_SHARED_LOCAL overrides the shared root;
# shared-root.ps1 holds the rule it must meet.
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version 2.0

function Fail([string]$diagnostic, [string]$message) {
  [Console]::Error.WriteLine("${diagnostic}: $message")
  exit 1
}
function Say([string]$message) { [Console]::Error.WriteLine($message) }

$archiveArgument = $null
$check = $false
for ($i = 0; $i -lt $args.Count; $i++) {
  switch ($args[$i]) {
    '--check' { $check = $true }
    '--archive' {
      if ($i + 1 -ge $args.Count) { Fail 'USAGE' '--archive needs a path' }
      $i++; $archiveArgument = [string]$args[$i]
    }
    default { Fail 'USAGE' "setup.ps1 [--archive <zip>] [--check]; unknown argument $($args[$i])" }
  }
}

function Sha256([string]$file) {
  $stream = [System.IO.File]::OpenRead($file)
  try {
    $hasher = [System.Security.Cryptography.SHA256]::Create()
    try { return (($hasher.ComputeHash($stream) | ForEach-Object { $_.ToString('x2') }) -join '') }
    finally { $hasher.Dispose() }
  } finally { $stream.Dispose() }
}

function Read-Git([string[]]$arguments) {
  $previous = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $output = & git @arguments 2>$null
    if ($LASTEXITCODE -ne 0) { return $null }
    return ([string]($output | Select-Object -First 1)).Trim()
  } catch { return $null } finally { $ErrorActionPreference = $previous }
}

$here = Split-Path -Parent $MyInvocation.MyCommand.Path
# Without Git the shared root cannot be found; never fall back to this checkout's own .local and download there.
if (-not (Get-Command git -CommandType Application -ErrorAction SilentlyContinue)) {
  Fail 'GIT_MISSING' 'git is not on PATH, so the shared .local root cannot be resolved; put Git on PATH and rerun'
}
$top = Read-Git @('-C', $here, 'rev-parse', '--show-toplevel')
if (-not $top) { $top = [System.IO.Path]::GetFullPath((Join-Path $here '../../../..')) }
$manifestPath = Join-Path $top 'src/godot-game/tools/verification/toolchain.json'
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) { Fail 'PINS_MISSING' "toolchain.json not found under $top" }
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$version = [string]$manifest.node
$pin = $manifest.nodeArchives.'win32-x64'
if (-not $version -or -not $pin) { Fail 'PINS_MISSING' 'node and nodeArchives.win32-x64 are required in toolchain.json' }
if ($pin.directory -ne "node-v$version-win-x64" -or $pin.file -ne "node-v$version-win-x64.zip") {
  Fail 'PINS_MISMATCH' "nodeArchives.win32-x64 does not name Node $version"
}
if ($pin.sha256 -notmatch '^[0-9a-f]{64}$') { Fail 'PINS_MISSING' 'nodeArchives.win32-x64 sha256' }
if (-not $pin.url.StartsWith('https://')) { Fail 'PINS_MISMATCH' 'the Node archive URL must use HTTPS' }

. (Join-Path $here 'shared-root.ps1')
$shared = $env:KZ_SHARED_LOCAL
if ($shared) {
  $problem = Get-KzOverrideProblem $shared $top
  if ($problem) { Fail 'SHARED_ROOT_INVALID' "KZ_SHARED_LOCAL ${problem}: $shared" }
} else {
  $common = Read-Git @('-C', $top, 'rev-parse', '--path-format=absolute', '--git-common-dir')
  if ($common) {
    if ((Split-Path -Leaf $common) -ne '.git') { Fail 'SHARED_ROOT_UNRESOLVED' "git common dir $common is not a .git folder; set KZ_SHARED_LOCAL" }
    $shared = Join-Path (Split-Path -Parent $common) '.local'
  } else { $shared = Join-Path $top '.local' }
}
$shared = [System.IO.Path]::GetFullPath($shared)
$downloads = Join-Path $shared 'downloads/node'
$toolsRoot = Join-Path $shared 'tools/node'
$archive = Join-Path $downloads $pin.file
$install = Join-Path $toolsRoot $pin.directory
$executable = Join-Path $install $pin.executable
$lockPath = Join-Path $shared 'locks/toolchain.lock'

function Assert-Archive([string]$file) {
  $actual = Sha256 $file
  if ($actual -ne $pin.sha256) { Fail 'NODE_ARCHIVE_MISMATCH' "$file has SHA-256 $actual; the pin is $($pin.sha256)" }
}
function Assert-Install {
  if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) {
    Fail 'NODE_INSTALL_INVALID' "$install exists without $($pin.executable); delete that folder and rerun"
  }
  $previous = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try { $reported = ([string](& $executable --version 2>$null)).Trim() } finally { $ErrorActionPreference = $previous }
  if ($reported -ne "v$version") { Fail 'NODE_INSTALL_INVALID' "$executable reports '$reported'; expected v$version" }
}
function Write-PathLine {
  Write-Output "`$env:Path = '$($install.Replace("'", "''"))' + [IO.Path]::PathSeparator + `$env:Path"
}

if ($check) {
  if (-not (Test-Path -LiteralPath $archive -PathType Leaf)) { Fail 'NODE_ARCHIVE_MISSING' "$archive is absent; run setup.ps1" }
  Assert-Archive $archive
  if (-not (Test-Path -LiteralPath $install -PathType Container)) { Fail 'NODE_INSTALL_MISSING' "$install is absent; run setup.ps1" }
  Assert-Install
  Say "Node $version verified in the shared tools."
  Write-PathLine
  exit 0
}

if (Test-Path -LiteralPath $install -PathType Container) {
  if (Test-Path -LiteralPath $archive -PathType Leaf) { Assert-Archive $archive }
  Assert-Install
  Say "Node $version is already installed in the shared tools; nothing downloaded."
  Write-PathLine
  exit 0
}

# Nothing writes the shared root unless Git ignores it in the checkout that holds it.
$ignoreProblem = Get-KzIgnoreProblem $shared
if ($ignoreProblem) { Fail 'SHARED_ROOT_NOT_IGNORED' $ignoreProblem }
New-Item -ItemType Directory -Force -Path $downloads, $toolsRoot, (Split-Path -Parent $lockPath) | Out-Null
try { $lock = [System.IO.File]::Open($lockPath, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write) }
catch { Fail 'TOOLCHAIN_LOCKED' 'Another acquisition owns .local/locks/toolchain.lock in the shared root; interrupted locks require inspection' }
$nonce = [guid]::NewGuid().ToString('N')
$staging = Join-Path $toolsRoot ".$($pin.directory).$nonce.tmp"
$part = "$archive.$nonce.part"
try {
  $record = [System.Text.Encoding]::UTF8.GetBytes((@{ nonce = $nonce; pid = $PID; at = (Get-Date).ToUniversalTime().ToString('o') } | ConvertTo-Json -Compress))
  $lock.Write($record, 0, $record.Length); $lock.Flush()

  if (Test-Path -LiteralPath $archive -PathType Leaf) {
    Assert-Archive $archive
    Say "Using the verified archive in the shared downloads."
  } else {
    if ($archiveArgument) {
      $source = [System.IO.Path]::GetFullPath($archiveArgument)
      if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { Fail 'NODE_ARCHIVE_MISSING' "--archive $source does not exist" }
      Say "Seeding the shared downloads from $source."
      Copy-Item -LiteralPath $source -Destination $part
    } else {
      [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
      Say "Downloading $($pin.url)."
      $progress = $ProgressPreference
      $ProgressPreference = 'SilentlyContinue'
      try { Invoke-WebRequest -UseBasicParsing -Uri $pin.url -OutFile $part } finally { $ProgressPreference = $progress }
    }
    Assert-Archive $part
    Move-Item -LiteralPath $part -Destination $archive
  }

  Add-Type -AssemblyName System.IO.Compression.FileSystem
  New-Item -ItemType Directory -Path $staging | Out-Null
  [System.IO.Compression.ZipFile]::ExtractToDirectory($archive, $staging)
  $extracted = Join-Path $staging $pin.directory
  if (-not (Test-Path -LiteralPath (Join-Path $extracted $pin.executable) -PathType Leaf)) {
    Fail 'NODE_ARCHIVE_LAYOUT' "the archive does not contain $($pin.directory)/$($pin.executable)"
  }
  Move-Item -LiteralPath $extracted -Destination $install
  Assert-Install
  Say "Installed Node $version into the shared tools."
} finally {
  if (Test-Path -LiteralPath $part) { Remove-Item -LiteralPath $part -Force }
  if (Test-Path -LiteralPath $staging) { Remove-Item -LiteralPath $staging -Recurse -Force }
  $lock.Dispose()
  Remove-Item -LiteralPath $lockPath -Force
}
Write-PathLine
exit 0
