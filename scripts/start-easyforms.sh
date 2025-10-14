#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<EOF
Start FrameworkQ.Easyforms API and Web UI.

Usage: $(basename "$0") [--build] [--force] [--api-port=N] [--web-port=N]

Options:
  --build        Build the solution before running (default: no)
  --force        Kill any processes on the target ports before starting
  --api-port     API port (default: 4000 or env PORT_API)
  --web-port     Web UI port (default: 5002 or env PORT_WEB)
  -h, --help     Show this help

Environment variables:
  PORT_API, PORT_WEB  Override default ports
EOF
}

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT="$(cd "$DIR/.." && pwd)"
PID_DIR="$ROOT/.pids"
LOG_DIR="$ROOT/.runlogs"
API_DIR="$ROOT/backend/src/FrameworkQ.Easyforms.Api"
WEB_DIR="$ROOT/backend/src/FrameworkQ.Easyforms.Web"

PORT_API="${PORT_API:-4000}"
PORT_WEB="${PORT_WEB:-5002}"
BUILD=0
FORCE=0

for arg in "$@"; do
  case "$arg" in
    --build) BUILD=1 ;;
    --force) FORCE=1 ;;
    --api-port=*) PORT_API="${arg#*=}" ;;
    --web-port=*) PORT_WEB="${arg#*=}" ;;
    -h|--help) usage; exit 0 ;;
    *) echo "Unknown option: $arg"; usage; exit 1 ;;
  esac
done

require() { command -v "$1" >/dev/null 2>&1 || { echo "Error: $1 is required." >&2; exit 1; }; }
require dotnet
require lsof

mkdir -p "$PID_DIR" "$LOG_DIR"

is_port_in_use() { local p="$1"; lsof -nP -iTCP:"$p" -sTCP:LISTEN -t >/dev/null 2>&1; }

kill_port() {
  local p="$1"
  local pids
  pids=$(lsof -nP -iTCP:"$p" -sTCP:LISTEN -t || true)
  if [[ -z "$pids" ]]; then return 0; fi
  echo "Killing processes on port $p: $pids"
  # Try graceful
  while read -r pid; do
    [[ -z "$pid" ]] && continue
    kill -TERM "$pid" 2>/dev/null || true
  done <<< "$pids"
  sleep 1
  # Force if still alive
  pids=$(lsof -nP -iTCP:"$p" -sTCP:LISTEN -t || true)
  if [[ -n "$pids" ]]; then
    while read -r pid; do
      [[ -z "$pid" ]] && continue
      kill -KILL "$pid" 2>/dev/null || true
    done <<< "$pids"
  fi
}

if [[ $FORCE -eq 1 ]]; then
  is_port_in_use "$PORT_API" && kill_port "$PORT_API" || true
  is_port_in_use "$PORT_WEB" && kill_port "$PORT_WEB" || true
else
  if is_port_in_use "$PORT_API"; then
    echo "Error: port $PORT_API is in use. Use --force or run scripts/kill-ports.sh $PORT_API" >&2
    exit 1
  fi
  if is_port_in_use "$PORT_WEB"; then
    echo "Error: port $PORT_WEB is in use. Use --force or run scripts/kill-ports.sh $PORT_WEB" >&2
    exit 1
  fi
fi

if [[ $BUILD -eq 1 ]]; then
  (cd "$ROOT/backend" && dotnet build FrameworkQ.Easyforms.sln -v minimal)
fi

echo "Starting API on http://localhost:$PORT_API ..."
(
  cd "$API_DIR"
  ASPNETCORE_URLS="http://localhost:$PORT_API" nohup dotnet run --no-build \
    > "$LOG_DIR/api.out.log" 2>&1 & echo $! > "$PID_DIR/api.pid"
)
sleep 1

echo "Starting Web UI on http://localhost:$PORT_WEB ..."
(
  cd "$WEB_DIR"
  ASPNETCORE_URLS="http://localhost:$PORT_WEB" nohup dotnet run --no-build \
    > "$LOG_DIR/web.out.log" 2>&1 & echo $! > "$PID_DIR/web.pid"
)
sleep 2

API_PID=$(cat "$PID_DIR/api.pid" 2>/dev/null || echo "?")
WEB_PID=$(cat "$PID_DIR/web.pid" 2>/dev/null || echo "?")

echo "API PID: $API_PID | Logs: $LOG_DIR/api.out.log"
echo "WEB PID: $WEB_PID | Logs: $LOG_DIR/web.out.log"
echo "Open:"
echo "  API Health:   http://localhost:$PORT_API/health"
echo "  Web UI Home:  http://localhost:$PORT_WEB"
