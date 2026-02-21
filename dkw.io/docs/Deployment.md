# dkw.io Deployment Runbook (Docker Compose)

This runbook covers first-time setup and repeat deployments for the `dkwio` stack.

## Prerequisites

- Docker Engine with Compose plugin (`docker compose`)
- Linux host with `sudo` access
- Repository checked out to `/home/docker/containers/dkwpiranha` (or equivalent)

## First-Time Setup (per server)

From repo root:

```bash
scripts/setup-dkwio-prereqs.sh
docker compose up -d --build
```

What this does:

- Ensures host prerequisites are available
- Creates and secures `dkw.io/App_Data`
- Applies ACL permissions so both host user and container app user can write SQLite files
- Builds and starts containers

## Repeat Deployment

From repo root after pulling latest changes:

```bash
docker compose up -d --build
```

If deployment user/group/UID changed, re-run:

```bash
scripts/setup-dkwio-prereqs.sh
```

## Verification

```bash
docker compose ps
docker logs dkwio --tail 120
ls -la dkw.io/App_Data
```

Expected:

- `dkwio` container status is `Up`
- App logs show startup listening on port `8080`
- SQLite files exist under `dkw.io/App_Data` (`piranha.db`, `piranha.db-shm`, `piranha.db-wal`)

## Troubleshooting

- `readonly database`: re-run `scripts/setup-dkwio-prereqs.sh`
- `dkw.io.dll does not exist`: ensure no bind mount overrides `/app`
- file upload permission errors under `/app/wwwroot/uploads`: rebuild image (`docker compose build dkwio`) and restart
