# Codex SessionStart hook on Windows: prints the shared Node folder as session context. Fail-open.
$ErrorActionPreference = 'SilentlyContinue'
$top = git rev-parse --show-toplevel 2>$null
if (-not $top) { exit 0 }
$version = (Get-Content -Raw (Join-Path $top 'src/godot-game/tools/verification/toolchain.json') | ConvertFrom-Json).node
if (-not $version) { exit 0 }
$shared = $env:KZ_SHARED_LOCAL
if (-not $shared) {
  $common = git -C $top rev-parse --path-format=absolute --git-common-dir 2>$null
  if (-not $common) { exit 0 }
  $shared = Join-Path (Split-Path -Parent $common) '.local'
}
$dir = Join-Path $shared "tools/node/node-v$version-win-x64"
if (-not (Test-Path -LiteralPath (Join-Path $dir 'node.exe') -PathType Leaf)) { exit 0 }
$dir = [System.IO.Path]::GetFullPath($dir)
Write-Output "Shared Node $version for this repository. In each PowerShell command run first: `$env:Path = '$($dir.Replace("'", "''"))' + [IO.Path]::PathSeparator + `$env:Path"
exit 0
