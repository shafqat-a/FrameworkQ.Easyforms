#!/usr/bin/env bash
set -euo pipefail

if ! command -v lsof >/dev/null 2>&1; then
  echo "Error: lsof is required." >&2
  exit 1
fi

ports=("$@")
if [[ ${#ports[@]} -eq 0 ]]; then
  ports=(4000 5002)
fi

kill_port(){
  local p="$1";
  local pids;
  pids=$(lsof -nP -iTCP:"$p" -sTCP:LISTEN -t || true)
  if [[ -z "$pids" ]]; then
    echo "No process listening on port $p"
    return 0
  fi
  echo "Killing processes on port $p: $pids"
  while read -r pid; do
    [[ -z "$pid" ]] && continue
    kill -TERM "$pid" 2>/dev/null || true
  done <<< "$pids"
  sleep 1
  # Force kill if needed
  pids=$(lsof -nP -iTCP:"$p" -sTCP:LISTEN -t || true)
  if [[ -n "$pids" ]]; then
    while read -r pid; do
      [[ -z "$pid" ]] && continue
      kill -KILL "$pid" 2>/dev/null || true
    done <<< "$pids"
  fi
  echo "Port $p cleared."
}

for port in "${ports[@]}"; do
  kill_port "$port"
done
