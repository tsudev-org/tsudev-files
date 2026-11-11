#!/usr/bin/env python3
"""tsudev-files: đổi tên mã nguồn từ "Filee" sang tsudev (ADR-0002, hard fork) - lặp lại an toàn (idempotent).

Bước P5a: namespace, project, solution, assembly thư viện, định danh C#/MSBuild. KHÔNG đổi định danh lúc chạy (tệp
chạy Filee.exe/filee-cli, AppGuid, thư mục dữ liệu, biến FILEE_*, COM/Explorer, GNOME, bundle macOS) - các thứ đó cần
di trú cho bản đã cài, làm ở bước P5b.

Dùng:
  python3 -I tools/doi-ten-tsudev.py            đổi tên cả repo (git mv + sửa nội dung)
  python3 -I tools/doi-ten-tsudev.py --kiem     chỉ liệt kê chỗ còn tên cũ (thoát 1 nếu còn)
  python3 -I tools/doi-ten-tsudev.py --tep A B  chỉ sửa nội dung các tệp chỉ định (bản vá lấy từ upstream)
"""

import pathlib
import re
import subprocess
import sys

ROOT = pathlib.Path(__file__).resolve().parent.parent

# Dự án .NET: Filee.<X> -> Tsudev.Files.<X>
PROJECTS = ("App", "Cli", "Core", "Engines", "Platform", "Setup")
NS = re.compile(r"\bFilee\.(" + "|".join(PROJECTS) + r")\b")

# Định danh C#/MSBuild mang tiền tố Filee (không phải định danh lúc chạy).
IDENTIFIERS = (
    "FileeJsonContext", "FileeUpdatePublicKey", "FileeRelease", "FileeExpectedPublisher", "FileeExpectedIssuer",
    "FileeExpectedSignerKeys", "FileeCheckExpectedPublisher", "FileeRepositoryUrl", "FileeFontCollectionTests",
    "FileeFontCollection", "FileeSetupPublish", "FileeNativeWayland", "FileeServicesProvider",
    "FileeRemoveNativeSymbols", "FileeRaiseTopmostTestWindow", "FileeMoveToActiveSpaceTestWindow",
    "FileeKeyWindowTestWindow", "FileeOnDemand", "FileeGlyphless", "FileeCmyk", "FileeTests",
)
IDENT = re.compile(r"\b(" + "|".join(sorted(IDENTIFIERS, key=len, reverse=True)) + r")\b")

SOLUTION_OLD, SOLUTION_NEW = "Filee.slnx", "tsudev-files.slnx"

# Lịch sử và tài liệu bản quyền giữ nguyên chữ cũ.
SKIP_PREFIXES = (".standards/", "logs/", "docs/adr/", "CHANGELOG.md", "LICENSE", "THIRD-PARTY-NOTICES.md",
                 "tools/doi-ten-tsudev.py", ".standards-upstream", ".gitleaksignore")
TEXT_SUFFIXES = {".cs", ".csproj", ".props", ".targets", ".slnx", ".axaml", ".xaml", ".json", ".jsonc", ".xml", ".yml",
                 ".yaml", ".ps1", ".psm1", ".sh", ".py", ".md", ".txt", ".plist", ".resx", ".manifest", ".js", ".mjs",
                 ".ts", ".astro", ".toml", ".editorconfig", ".runsettings", ".c", ".h", ".rc", ".def", ".iss", ".cmd",
                 ".bat", ".desktop", ".css", ".html", ".svg", ".config", ".in", ""}


def git(*args: str) -> str:
    return subprocess.run(["git", *args], cwd=ROOT, check=True, capture_output=True, text=True).stdout


def tracked() -> list[str]:
    return [p for p in git("ls-files", "-z").split("\0") if p]


def eligible(path: str) -> bool:
    if path.startswith(SKIP_PREFIXES):
        return False
    p = pathlib.Path(path)
    return p.suffix.lower() in TEXT_SUFFIXES or p.name in (".editorconfig", ".gitattributes", "CODEOWNERS")


def rewrite(text: str) -> str:
    text = NS.sub(lambda m: "Tsudev.Files." + m.group(1), text)
    text = IDENT.sub(lambda m: "TsudevFiles" + m.group(1)[len("Filee"):], text)
    return text.replace(SOLUTION_OLD, SOLUTION_NEW)


def new_path(path: str) -> str:
    return NS.sub(lambda m: "Tsudev.Files." + m.group(1), path).replace(SOLUTION_OLD, SOLUTION_NEW).replace(
        "FileeFontCollectionTests", "TsudevFilesFontCollectionTests")


def fix_content(paths: list[str]) -> int:
    changed = 0
    for path in paths:
        f = ROOT / path
        if not f.is_file() or not eligible(path):
            continue
        raw = f.read_bytes()
        try:
            text = raw.decode("utf-8")
        except UnicodeDecodeError:
            continue
        new = rewrite(text)
        if new != text:
            f.write_bytes(new.encode("utf-8"))
            changed += 1
    return changed


def main() -> int:
    args = sys.argv[1:]
    if args[:1] == ["--kiem"]:
        left = [p for p in tracked() if eligible(p) and (ROOT / p).is_file()
                and (NS.search(p) or (rewrite(t := (ROOT / p).read_text("utf-8", errors="ignore")) != t))]
        for p in left:
            print(p)
        return 1 if left else 0
    if args[:1] == ["--tep"]:
        print(f"đã sửa {fix_content(args[1:])} tệp")
        return 0

    moves = [(p, new_path(p)) for p in tracked() if new_path(p) != p]
    for old, new in moves:
        (ROOT / new).parent.mkdir(parents=True, exist_ok=True)
        git("mv", old, new)
    print(f"đã chuyển {len(moves)} tệp")
    print(f"đã sửa nội dung {fix_content(tracked())} tệp")
    return 0


if __name__ == "__main__":
    sys.exit(main())
