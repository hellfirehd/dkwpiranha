#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
APP_DATA_DIR="${ROOT_DIR}/dkw.io/App_Data"
APP_UID="1654"
APP_GID="1654"
DRY_RUN="false"

usage() {
  cat <<'EOF'
Usage: scripts/setup-dkwio-prereqs.sh [options]

Prepares host prerequisites for dkw.io docker deployment:
- Verifies docker and docker compose are available
- Installs acl package if setfacl/getfacl are missing (Debian/Ubuntu)
- Creates dkw.io/App_Data if needed
- Applies least-privilege permissions + ACLs for host user and container app user

Options:
  --data-dir <path>   Override App_Data directory
  --app-uid <uid>     Container runtime UID (default: 1654)
  --app-gid <gid>     Container runtime GID (default: 1654)
  --dry-run           Print actions without changing the system
  -h, --help          Show this help
EOF
}

log() {
  echo "[setup] $*"
}

run_cmd() {
  if [[ "$DRY_RUN" == "true" ]]; then
    echo "[dry-run] $*"
    return 0
  fi

  if [[ "$EUID" -eq 0 ]]; then
    "$@"
  else
    sudo "$@"
  fi
}

require_command() {
  local cmd="$1"
  if ! command -v "$cmd" >/dev/null 2>&1; then
    echo "Missing required command: $cmd" >&2
    exit 1
  fi
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --data-dir)
      APP_DATA_DIR="$2"
      shift 2
      ;;
    --app-uid)
      APP_UID="$2"
      shift 2
      ;;
    --app-gid)
      APP_GID="$2"
      shift 2
      ;;
    --dry-run)
      DRY_RUN="true"
      shift
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      usage
      exit 1
      ;;
  esac
done

HOST_USER="$(id -un)"
HOST_GROUP="$(id -gn)"

log "Checking docker prerequisites"
require_command docker
if ! docker compose version >/dev/null 2>&1; then
  echo "docker compose is required but not available" >&2
  exit 1
fi

if ! command -v setfacl >/dev/null 2>&1 || ! command -v getfacl >/dev/null 2>&1; then
  log "ACL tools not found; installing acl package"
  if command -v apt-get >/dev/null 2>&1; then
    run_cmd apt-get update
    run_cmd apt-get install -y acl
  else
    echo "ACL tools missing and automatic install is only implemented for apt-get hosts." >&2
    echo "Please install package 'acl' manually and rerun." >&2
    exit 1
  fi
fi

log "Preparing data directory: ${APP_DATA_DIR}"
run_cmd mkdir -p "${APP_DATA_DIR}"

# Restrictive base mode + host ownership for day-to-day operations.
run_cmd chown "${HOST_USER}:${HOST_GROUP}" "${APP_DATA_DIR}"
run_cmd chmod 0750 "${APP_DATA_DIR}"

# Ensure both host user and container runtime user can read/write/execute inside dir.
run_cmd setfacl -m "u:${HOST_USER}:rwx" "${APP_DATA_DIR}"
run_cmd setfacl -m "d:u:${HOST_USER}:rwx" "${APP_DATA_DIR}"
run_cmd setfacl -m "u:${APP_UID}:rwx" "${APP_DATA_DIR}"
run_cmd setfacl -m "d:u:${APP_UID}:rwx" "${APP_DATA_DIR}"

# Tighten default group/other ACL entries.
run_cmd setfacl -m g::r-x "${APP_DATA_DIR}"
run_cmd setfacl -m o::--- "${APP_DATA_DIR}"
run_cmd setfacl -m d:g::r-x "${APP_DATA_DIR}"
run_cmd setfacl -m d:o::--- "${APP_DATA_DIR}"

# If database files already exist, align ACL so both principals can read/write.
shopt -s nullglob
for db_file in "${APP_DATA_DIR}"/piranha.db*; do
  run_cmd setfacl -m "u:${HOST_USER}:rw,u:${APP_UID}:rw" "${db_file}"
  run_cmd chmod 0640 "${db_file}"
done
shopt -u nullglob

log "Done. Effective ACLs:"
if [[ "$DRY_RUN" == "true" ]]; then
  echo "[dry-run] getfacl -p ${APP_DATA_DIR}"
else
  getfacl -p "${APP_DATA_DIR}"
fi

log "You can now run: docker compose up -d --build"
