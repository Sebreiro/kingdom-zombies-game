# The KZ_SHARED_LOCAL rule on Windows, dot-sourced by setup.ps1 and session-path.ps1. workspace-paths.cjs,
# shared-root.sh and WorkspacePaths.cs hold the same rule with the same messages; session-path.test.cjs runs one case
# table through each script. The value is drive-qualified or UNC (a root-relative or MSYS /d/... form is refused,
# never converted). It has no '.' or '..' segment, is not a filesystem or drive root, and is neither the checkout nor
# inside it, except the checkout's own .local folder.

# A path for comparison only: backslashes become '/'; repeated separators collapse except a leading '//'; trailing
# separators are dropped.
function ConvertTo-KzNormal([string]$value) {
  $text = $value.Replace('\', '/')
  $unc = $text.StartsWith('//', [StringComparison]::Ordinal)
  $text = [regex]::Replace($text, '/{2,}', '/')
  if ($unc) { $text = '/' + $text }
  return $text.TrimEnd('/')
}

# Returns why the value is refused, or $null when it is accepted.
function Get-KzOverrideProblem([string]$value, [string]$checkout) {
  if ($value -cnotmatch '^([A-Za-z]:[\\/]|[\\/]{2}[^\\/]+[\\/][^\\/]+)') {
    return 'must be a drive-qualified or UNC path, not a root-relative or MSYS /d/... form'
  }
  $normal = ConvertTo-KzNormal $value
  if ($normal -cmatch '(^|/)\.\.?(/|$)') { return 'must not contain . or .. segments' }
  if ($normal -cmatch '^[A-Za-z]:$|^//[^/]+/[^/]+$') { return 'must not be a filesystem or drive root' }
  $target = $normal.ToLowerInvariant() + '/'
  $base = (ConvertTo-KzNormal $checkout).ToLowerInvariant() + '/'
  if ($target.StartsWith($base + '.local/', [StringComparison]::Ordinal)) { return $null }
  if ($target.StartsWith($base, [StringComparison]::Ordinal)) {
    return 'must not be the checkout or inside it, except its .local folder'
  }
  return $null
}

# Before anything writes the shared root: returns why Git does not ignore it in the checkout that holds it, or $null.
# A shared root outside every Git working tree needs no check. The same check is assertSharedIgnored in
# workspace-paths.cjs; nothing edits info/exclude.
function Get-KzIgnoreProblem([string]$shared) {
  $existing = $shared
  while (-not (Test-Path -LiteralPath $existing -PathType Container)) {
    $parent = Split-Path -Parent $existing
    if (-not $parent -or $parent -eq $existing) { break }
    $existing = $parent
  }
  $previous = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $top = [string](& git -C $existing rev-parse --show-toplevel 2>$null | Select-Object -First 1)
    if ($LASTEXITCODE -ne 0 -or -not $top) { return $null }
    & git -C $top check-ignore -q -- "$shared/" 2>$null
    if ($LASTEXITCODE -eq 0) { return $null }
  } finally { $ErrorActionPreference = $previous }
  return "$shared is not ignored by Git in $top; on a branch older than the .local layout, add the line /.local/ " +
    "to that checkout's .git/info/exclude (docs/workflow/setup.md)"
}
