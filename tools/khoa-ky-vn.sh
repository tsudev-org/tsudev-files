#!/usr/bin/env bash
# tsudev-files: quản lý khóa ký cập nhật (ECDSA P-256) - mã hóa khóa bằng passphrase và tạo khóa dự phòng.
#
# Cách dùng:
#   tools/khoa-ky-vn.sh ma-hoa [khóa.pem]     Mã hóa tại chỗ một khóa đang để thô (PKCS#8, scrypt + AES-256-CBC).
#                                             Mặc định: ~/.config/tsudev-files/keys/update-key.pem
#   tools/khoa-ky-vn.sh tao <khóa-mới.pem>    Tạo khóa MỚI đã mã hóa, in khóa công khai để thêm vào vòng khóa
#                                             FileeUpdatePublicKey ("<khóa chính>|<khóa mới>") trong Directory.Build.props.
#   tools/khoa-ky-vn.sh kiem [khóa.pem]       In khóa công khai và cho biết khóa có trong vòng khóa của repo không.
#
# An toàn:
#   - Passphrase nhập từ bàn phím (không qua tham số/biến môi trường, không vào lịch sử shell).
#   - scrypt (N=2^14, r=8, p=16: 16 MiB và ~16 lần công tính mỗi lần thử - mức trần của openssl pkcs8) làm chậm dò passphrase; khóa thô cũ bị ghi đè ngẫu nhiên rồi xóa (shred) sau khi mã hóa xong.
#   - Từ chối khóa nằm trong git working tree hoặc có quyền đọc cho group/other.
#   - Khóa dự phòng: cất NGOẠI TUYẾN (USB mã hóa/két), không để trên máy ký hằng ngày (docs/vi/PHAT-HANH.md).
set -euo pipefail

die() { echo "LỖI: $*" >&2; exit 1; }
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DEFAULT_KEY="$HOME/.config/tsudev-files/keys/update-key.pem"
command -v openssl >/dev/null || die "cần openssl"

ring() { sed -n 's:.*<FileeUpdatePublicKey>\([^<]*\)</FileeUpdatePublicKey>.*:\1:p' "$ROOT/Directory.Build.props" | head -1; }

outside_git() {
  local dir; dir="$(cd "$(dirname "$1")" && pwd)"
  if git -C "$dir" rev-parse --is-inside-work-tree >/dev/null 2>&1; then
    die "$1 nằm trong một git working tree ($dir). Chuyển ra ngoài."
  fi
}

read_new_pass() {
  local a b
  read -rsp "Passphrase mới (>= 16 ký tự): " a; echo >&2
  (( ${#a} >= 16 )) || die "passphrase quá ngắn."
  read -rsp "Nhập lại: " b; echo >&2
  [[ "$a" == "$b" ]] || die "hai lần nhập không khớp."
  PASS="$a"
}

encrypt_to() { # <khóa thô> <đích>
  ( umask 077; openssl pkcs8 -topk8 -in "$1" -out "$2" -v2 aes-256-cbc -scrypt -scrypt_N 16384 -scrypt_r 8 -scrypt_p 16 \
      -passout fd:3 3<<<"$PASS" )
}

pub_of() { # <khóa> -> base64 SPKI (hỏi passphrase nếu khóa đã mã hóa)
  if grep -q "ENCRYPTED PRIVATE KEY" "$1"; then
    [[ -n "${PASS:-}" ]] || { read -rsp "Passphrase của $1: " PASS; echo >&2; }
    openssl pkey -in "$1" -passin fd:3 -pubout -outform DER 3<<<"$PASS" | base64 -w0
  else
    openssl pkey -in "$1" -pubout -outform DER | base64 -w0
  fi
}

case "${1:-}" in
  ma-hoa)
    KEY="${2:-$DEFAULT_KEY}"
    [[ -f "$KEY" ]] || die "không thấy $KEY"
    outside_git "$KEY"
    grep -q "ENCRYPTED PRIVATE KEY" "$KEY" && { echo "$KEY đã được mã hóa."; exit 0; }
    before="$(pub_of "$KEY")"
    read_new_pass
    tmp="$KEY.enc.$$"; trap 'rm -f "$tmp"' EXIT
    encrypt_to "$KEY" "$tmp" || die "mã hóa thất bại; giữ nguyên khóa cũ."
    after="$(pub_of "$tmp")"
    [[ "$before" == "$after" ]] || { rm -f "$tmp"; die "kiểm lại sau mã hóa thất bại; giữ nguyên khóa cũ."; }
    shred -u "$KEY" 2>/dev/null || rm -f "$KEY"
    mv "$tmp" "$KEY"; chmod 600 "$KEY"
    echo "Đã mã hóa $KEY (khóa công khai không đổi: ${after:0:24}...)."
    echo "GHI passphrase vào trình quản lý mật khẩu NGAY - mất passphrase = mất khóa."
    ;;
  tao)
    KEY="${2:-}"
    [[ -n "$KEY" && "$KEY" == *.pem ]] || die "cần đường dẫn khóa mới (*.pem)."
    [[ ! -e "$KEY" ]] || die "$KEY đã tồn tại."
    mkdir -p "$(dirname "$KEY")"; outside_git "$KEY"
    read_new_pass
    raw="$(mktemp)"; enc="$KEY.tmp.$$"
    trap 'shred -u "$raw" 2>/dev/null || rm -f "$raw"; rm -f "$enc"' EXIT
    ( umask 077; openssl genpkey -algorithm EC -pkeyopt ec_paramgen_curve:P-256 -out "$raw" )
    encrypt_to "$raw" "$enc" || die "mã hóa thất bại."
    mv "$enc" "$KEY"; chmod 600 "$KEY"
    pub="$(pub_of "$KEY")"
    echo "Đã tạo $KEY (đã mã hóa). Khóa công khai:"
    echo "$pub"
    echo "Thêm vào Directory.Build.props: <FileeUpdatePublicKey>$(ring)|$pub</FileeUpdatePublicKey>"
    ;;
  kiem)
    KEY="${2:-$DEFAULT_KEY}"
    [[ -f "$KEY" ]] || die "không thấy $KEY"
    pub="$(pub_of "$KEY")"
    echo "$pub"
    if [[ "|$(ring | tr -d ' ')|" == *"|$pub|"* ]]; then echo "CÓ trong vòng khóa của repo."; else echo "KHÔNG có trong vòng khóa của repo."; exit 1; fi
    ;;
  *) sed -n '2,16p' "$0"; exit 2 ;;
esac
