#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DO_PULL="true"

usage() {
  cat <<'EOF'
Usage: scripts/deploy-dkwio.sh [options]

Minimal-downtime deploy for dkwio:
1) (optional) git pull
2) docker compose build dkwio
3) docker compose up -d --no-deps dkwio

Options:
  --no-pull     Skip git pull
  -h, --help    Show help
EOF
}

log() {
  echo "[deploy] $*"
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
    --no-pull)
      DO_PULL="false"
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

require_command git
require_command docker
if ! docker compose version >/dev/null 2>&1; then
  echo "docker compose is required but not available" >&2
  exit 1
fi

cd "$ROOT_DIR"

if [[ "$DO_PULL" == "true" ]]; then
  log "Pulling latest code"
  git pull --ff-only
fi

log "Building dkwio image"
docker compose build dkwio

log "Recreating dkwio container without restarting dependencies"
docker compose up -d --no-deps dkwio

log "Deployment complete"
docker compose ps dkwio
