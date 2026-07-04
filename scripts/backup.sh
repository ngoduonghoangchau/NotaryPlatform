#!/usr/bin/env bash
# NotaryPlatform nightly backup — PostgreSQL dump + uploaded files, copied off-site.
#
# Install (as the deploy user), crontab -e:
#   0 2 * * * cd /home/app/notaryplatform && RCLONE_REMOTE=r2:np-backups ./scripts/backup.sh >> ~/np-backup.log 2>&1
#
# RCLONE_REMOTE is optional: configure `rclone` with a FREE object store
# (Cloudflare R2 / Backblaze B2 / Google Drive) to keep an off-box copy.
set -euo pipefail
umask 077                                   # backups readable only by their owner

PROJECT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_DIR"

BACKUP_DIR="${BACKUP_DIR:-$PROJECT_DIR/backups}"
RCLONE_REMOTE="${RCLONE_REMOTE:-}"          # empty = keep local copies only
KEEP="${KEEP:-7}"                           # how many of each backup to retain locally
STAMP="$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

echo "[$(date -Is)] backup start"

# 1. PostgreSQL logical dump — runs inside the container, so no password on the CLI.
docker compose exec -T postgres \
	pg_dump -U postgres -Fc NotaryDB \
	> "$BACKUP_DIR/np_$STAMP.pgdump"

# 2. Uploaded files (local storage volume). Skipped cleanly if empty/unused.
docker run --rm \
	-v notaryplatform_app-uploads:/data:ro \
	-v "$BACKUP_DIR":/backup \
	alpine tar czf "/backup/uploads_$STAMP.tgz" -C /data . 2>/dev/null || true

# 3. Off-site copy to free object storage (only if a remote is configured).
if [ -n "$RCLONE_REMOTE" ]; then
	rclone copy "$BACKUP_DIR" "$RCLONE_REMOTE" --max-age 25h --transfers 2
fi

# 4. Local retention: keep the newest $KEEP of each kind.
ls -1t "$BACKUP_DIR"/np_*.pgdump   2>/dev/null | tail -n +"$((KEEP + 1))" | xargs -r rm -f
ls -1t "$BACKUP_DIR"/uploads_*.tgz 2>/dev/null | tail -n +"$((KEEP + 1))" | xargs -r rm -f

echo "[$(date -Is)] backup done -> $BACKUP_DIR"
