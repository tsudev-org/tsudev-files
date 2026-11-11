#!/usr/bin/env bash
# tsudev-files: ký các tệp *SHA256SUMS*.txt bằng khóa cập nhật riêng của bản fork (ECDSA P-256 / SHA-256).
#
# Thay cho build/sign-checksums.ps1 của upstream khi làm việc trên Linux/WSL. Định dạng chữ ký giống hệt:
# tệp <tên>.sig chứa base64 của chữ ký 64 byte r||s (IEEE P1363), đúng như UpdateSignature.Verify mong đợi.
#
# Cách dùng:
#   tools/sign-checksums-vn.sh <khóa-bí-mật.pem> <tệp-SHA256SUMS>...
#   (mặc định khóa: $TSUDEV_FILES_UPDATE_KEY hoặc ~/.config/tsudev-files/keys/update-key.pem)
#
# An toàn:
#   - Từ chối khóa nằm trong bất kỳ git working tree nào (một commit là lộ khóa).
#   - Từ chối khóa có quyền đọc cho group/other.
#   - Đối chiếu khóa công khai với vòng khóa FileeUpdatePublicKey ("<chính>|<dự phòng>") trước khi ký.
#   - Khóa đã mã hóa (tools/khoa-ky-vn.sh ma-hoa): hỏi passphrase MỘT lần, truyền cho openssl qua fd (không qua
#     tham số/biến môi trường).
#   - Tự xác minh lại từng chữ ký sau khi ký.
set -euo pipefail

die() { echo "LỖI: $*" >&2; exit 1; }
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

KEY="${1:-}"
if [[ -z "$KEY" || "$KEY" != *.pem ]]; then
  KEY="${TSUDEV_FILES_UPDATE_KEY:-$HOME/.config/tsudev-files/keys/update-key.pem}"
else
  shift
fi
(( $# > 0 )) || die "thiếu tệp SHA256SUMS cần ký. Xem phần đầu script."
[[ -f "$KEY" ]] || die "không thấy khóa bí mật: $KEY"
command -v openssl >/dev/null || die "cần openssl"
command -v python3 >/dev/null || die "cần python3"

KEY_DIR="$(cd "$(dirname "$KEY")" && pwd)"
if git -C "$KEY_DIR" rev-parse --is-inside-work-tree >/dev/null 2>&1; then
  die "khóa nằm trong một git working tree ($KEY_DIR). Chuyển nó ra ngoài."
fi
perm="$(stat -c '%a' "$KEY")"
[[ "$perm" =~ ^[0-7]00$ ]] || die "khóa có quyền $perm; cần 600 (chmod 600 '$KEY')."

PASSIN=()
if grep -q "ENCRYPTED PRIVATE KEY" "$KEY"; then
  read -rsp "Passphrase của khóa ký: " KEY_PASS; echo >&2
  PASSIN=(-passin fd:3)
fi
ossl() { if (( ${#PASSIN[@]} )); then openssl "$1" "${PASSIN[@]}" "${@:2}" 3<<<"$KEY_PASS"; else openssl "$@"; fi; }
grep -q "ENCRYPTED PRIVATE KEY" "$KEY" || echo "CẢNH BÁO: khóa ký đang để thô - chạy tools/khoa-ky-vn.sh ma-hoa." >&2

own_pub="$(ossl pkey -in "$KEY" -pubout -outform DER | base64 -w0)"
build_pub="$(sed -n 's:.*<FileeUpdatePublicKey>\([^<]*\)</FileeUpdatePublicKey>.*:\1:p' "$ROOT/Directory.Build.props" | head -1)"
if [[ "${TSUDEV_FILES_SKIP_PROPS_CHECK:-0}" != 1 ]]; then
  [[ -n "$build_pub" ]] || die "Directory.Build.props không có FileeUpdatePublicKey."
  [[ "|${build_pub// /}|" == *"|$own_pub|"* ]] || die "khóa bí mật KHÔNG thuộc vòng khóa FileeUpdatePublicKey trong Directory.Build.props."
fi

tmp="$(mktemp -d)"; trap 'rm -rf "$tmp"' EXIT
ossl pkey -in "$KEY" -pubout -out "$tmp/pub.pem"

for f in "$@"; do
  [[ -f "$f" ]] || die "không thấy $f"
  ossl dgst -sha256 -sign "$KEY" -out "$tmp/sig.der" "$f"
  # DER (SEQUENCE{INTEGER r, INTEGER s}) -> r||s 64 byte, base64.
  python3 -I - "$tmp/sig.der" "$f.sig" <<'PY'
import base64, sys
der = open(sys.argv[1], "rb").read()
def read_int(b, i):
    assert b[i] == 0x02, "không phải INTEGER"
    n = b[i + 1]; v = b[i + 2:i + 2 + n]
    return int.from_bytes(v, "big"), i + 2 + n
assert der[0] == 0x30, "không phải SEQUENCE"
i = 2 if der[1] < 0x80 else 2 + (der[1] & 0x7F)
r, i = read_int(der, i); s, _ = read_int(der, i)
raw = r.to_bytes(32, "big") + s.to_bytes(32, "big")
open(sys.argv[2], "w", encoding="ascii").write(base64.b64encode(raw).decode() + "\n")
PY
  # Tự xác minh: đổi ngược r||s -> DER rồi kiểm bằng openssl.
  python3 -I - "$f.sig" "$tmp/check.der" <<'PY'
import base64, sys
raw = base64.b64decode(open(sys.argv[1]).read().strip()); assert len(raw) == 64
def enc(x):
    b = x.lstrip(b"\x00") or b"\x00"
    if b[0] & 0x80: b = b"\x00" + b
    return b"\x02" + bytes([len(b)]) + b
body = enc(raw[:32]) + enc(raw[32:])
open(sys.argv[2], "wb").write(b"\x30" + bytes([len(body)]) + body)
PY
  openssl dgst -sha256 -verify "$tmp/pub.pem" -signature "$tmp/check.der" "$f" >/dev/null \
    || die "chữ ký của $f không tự xác minh được."
  echo "đã ký: $f.sig"
done
