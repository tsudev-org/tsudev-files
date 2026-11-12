#!/usr/bin/env python3
"""TsuFiles: kiểm tra bản dịch tiếng Việt (chạy trong CI, workflow "Bảo mật").

Đạt khi:
  1. vi.json và en.json có đúng cùng tập khóa (không thiếu, không thừa).
  2. Mọi chuỗi giữ nguyên tham số định dạng của bản tiếng Anh ({0}, {1:N0}, …) - sai tham số làm
     string.Format ném ngoại lệ lúc chạy.
  3. Không có chuỗi rỗng.
  4. Không còn trỏ người dùng về trang tải của repo gốc (filee.sh) - bản fork chỉ phát hành tại repo riêng.
  5. Số chuỗi tiếng Việt còn trùng y nguyên tiếng Anh không vượt danh sách cho phép (tên riêng, mã kỹ thuật).

Chạy: python3 -I tools/check_i18n_vi.py
"""
import json
import pathlib
import re
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent
I18N = ROOT / "src" / "TsuFiles.App" / "Assets" / "i18n"
PLACEHOLDER = re.compile(r"\{\d+(?:[,:][^}]*)?\}")
# Chuỗi được phép giống tiếng Anh: tên sản phẩm, đơn vị, chuẩn giấy, codec, ... (không cần dịch).
ALLOWED_SAME = 30


def load(name: str) -> dict:
    with open(I18N / f"{name}.json", encoding="utf-8") as f:
        data = json.load(f)
    return {k: v for k, v in data.items() if isinstance(v, str)}


def main() -> int:
    en, vi = load("en"), load("vi")
    errors = []
    missing = sorted(set(en) - set(vi))
    extra = sorted(set(vi) - set(en))
    errors += [f"thiếu khóa: {k}" for k in missing]
    errors += [f"khóa thừa: {k}" for k in extra]
    for key in sorted(set(en) & set(vi)):
        if sorted(PLACEHOLDER.findall(en[key])) != sorted(PLACEHOLDER.findall(vi[key])):
            errors.append(f"sai tham số định dạng: {key}: en={PLACEHOLDER.findall(en[key])} vi={PLACEHOLDER.findall(vi[key])}")
        if en[key].strip() and not vi[key].strip():
            errors.append(f"chuỗi rỗng: {key}")
        if "filee.sh" in vi[key]:
            errors.append(f"còn trỏ về filee.sh: {key}")
    same = [k for k in en if k in vi and vi[k] == en[k] and re.search(r"[A-Za-z]{3,}", en[k])]
    if len(same) > ALLOWED_SAME:
        errors.append(f"{len(same)} chuỗi chưa dịch (trùng tiếng Anh), cho phép tối đa {ALLOWED_SAME}")
    for e in errors:
        print(f"LỖI: {e}")
    print(f"vi.json: {len(vi)}/{len(en)} khóa, {len(same)} chuỗi giữ nguyên tiếng Anh (cho phép {ALLOWED_SAME}).")
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
