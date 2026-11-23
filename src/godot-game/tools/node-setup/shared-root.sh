# The KZ_SHARED_LOCAL rule, sourced by setup.sh and session-path.sh. workspace-paths.cjs, shared-root.ps1 and
# WorkspacePaths.cs hold the same rule with the same messages; session-path.test.cjs runs one case table through
# each script. On Windows the value is drive-qualified or UNC (a root-relative or MSYS /d/... form is refused, never
# converted); elsewhere it starts with '/'. It has no '.' or '..' segment, is not a filesystem or drive root, and is
# neither the checkout nor inside it, except the checkout's own .local folder.

# kz_normal VALUE WINDOWS: a path for comparison only. On Windows (WINDOWS=1) backslashes become '/'; repeated
# separators collapse except a leading '//'; trailing separators are dropped.
kz_normal() {
  if [ "$2" = 1 ]; then
    printf '%s' "$1" | sed -e 's#\\#/#g' -e 's#\(.\)//*#\1/#g' -e 's#/*$##'
  else
    printf '%s' "$1" | sed -e 's#\(.\)//*#\1/#g' -e 's#/*$##'
  fi
}

# kz_override_problem VALUE CHECKOUT WINDOWS: prints why VALUE is refused, or nothing when it is accepted.
kz_override_problem() {
  if [ "$3" = 1 ]; then
    if ! printf '%s' "$1" | grep -Eq '^([A-Za-z]:[\\/]|[\\/][\\/][^\\/]+[\\/][^\\/]+)'; then
      echo 'must be a drive-qualified or UNC path, not a root-relative or MSYS /d/... form'
      return 0
    fi
  else
    case $1 in
      /*) ;;
      *) echo 'must be an absolute path'; return 0 ;;
    esac
  fi
  kz_value=$(kz_normal "$1" "$3")
  if printf '%s' "$kz_value" | grep -Eq '(^|/)\.\.?(/|$)'; then
    echo 'must not contain . or .. segments'
    return 0
  fi
  if [ "$3" = 1 ]; then
    if printf '%s' "$kz_value" | grep -Eq '^[A-Za-z]:$|^//[^/]+/[^/]+$'; then
      echo 'must not be a filesystem or drive root'
      return 0
    fi
  elif [ -z "$kz_value" ]; then
    echo 'must not be a filesystem or drive root'
    return 0
  fi
  kz_base=$(kz_normal "$2" "$3")
  if [ "$3" = 1 ]; then
    kz_value=$(printf '%s' "$kz_value" | tr 'A-Z' 'a-z')
    kz_base=$(printf '%s' "$kz_base" | tr 'A-Z' 'a-z')
  fi
  case "$kz_value/" in
    "$kz_base/.local/"*) ;;
    "$kz_base/"*) echo 'must not be the checkout or inside it, except its .local folder' ;;
  esac
  return 0
}
