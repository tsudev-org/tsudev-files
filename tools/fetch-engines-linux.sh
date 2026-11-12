#!/usr/bin/env bash
# TsuFiles: tải engine tùy chọn cho Linux/WSL vào engines/<id>/ ở gốc repo, để chạy test giống CI.
# Tương đương build/fetch-engines.ps1 (chỉ dành cho Windows) của upstream.
#
# Nguồn tải, phiên bản và SHA-256 lấy từ src/TsuFiles.Engines/Infrastructure/engines.json (cùng danh sách ứng dụng dùng
# khi tải engine). Sai SHA-256 thì hủy, không giải nén. Giải nén từ chối đường dẫn tuyệt đối và "..".
#
# Cách dùng: tools/fetch-engines-linux.sh 7zip [pandoc rhwp ...]   (mặc định: 7zip)
set -euo pipefail
die() { echo "LỖI: $*" >&2; exit 1; }
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
JSON="$ROOT/src/TsuFiles.Engines/Infrastructure/engines.json"
case "$(uname -m)" in x86_64) RID=linux-x64 ;; aarch64) RID=linux-arm64 ;; *) die "kiến trúc $(uname -m) chưa hỗ trợ" ;; esac
(( $# > 0 )) || set -- 7zip
tmp="$(mktemp -d)"; trap 'rm -rf "$tmp"' EXIT

for id in "$@"; do
  [[ "$id" =~ ^[a-z0-9-]+$ ]] || die "tên engine không hợp lệ: $id"
  read -r url sha kind < <(python3 -I - "$JSON" "$RID" "$id" <<'PY'
import json, sys
c = json.load(open(sys.argv[1], encoding="utf-8"))["platforms"][sys.argv[2]]["components"].get(sys.argv[3])
if not c: sys.exit(f"không có engine {sys.argv[3]} cho {sys.argv[2]}")
print(c["url"], c["sha256"].lower(), c["kind"])
PY
)
  [[ "$url" == https://* ]] || die "$id: chỉ nhận URL https"
  target="$ROOT/engines/$id"
  if [[ -f "$target/.sha256" && "$(cat "$target/.sha256")" == "$sha" ]]; then echo "có sẵn $id"; continue; fi
  echo "tải  $id"
  curl -fsSL --proto '=https' --tlsv1.2 --retry 3 -o "$tmp/$id.pkg" "$url"
  echo "$sha  $tmp/$id.pkg" | sha256sum --check --quiet || die "$id: SHA-256 không khớp, đã hủy"
  case "$kind" in tar.xz|tar.gz) ;; *) die "$id: kiểu gói $kind chưa hỗ trợ" ;; esac
  if tar -tf "$tmp/$id.pkg" | grep -qE '(^/|(^|/)\.\.(/|$))'; then die "$id: gói chứa đường dẫn không an toàn"; fi
  rm -rf "$target"; mkdir -p "$target"
  tar -xf "$tmp/$id.pkg" -C "$target" --no-same-owner --no-same-permissions
  # Gói chỉ có một thư mục con thì đưa nội dung lên (giống Expand-Flat của upstream).
  mapfile -t top < <(find "$target" -mindepth 1 -maxdepth 1)
  if (( ${#top[@]} == 1 )) && [[ -d "${top[0]}" ]]; then
    shopt -s dotglob; mv "${top[0]}"/* "$target"/; rmdir "${top[0]}"; shopt -u dotglob
  fi
  echo "$sha" > "$target/.sha256"
  echo "xong $id -> engines/$id"
done
