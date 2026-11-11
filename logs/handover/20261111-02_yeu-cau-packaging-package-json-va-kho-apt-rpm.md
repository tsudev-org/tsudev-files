# Phiếu yêu cầu 20261111-02 - thêm `packaging/package.json`, đưa tsudev-files vào kho APT/RPM

- **Từ**: phiên đóng gói hệ tsudev (`~/projects`, 11/11/2026)
- **Gửi**: phiên làm việc ở tsudev-files
- **Mức**: thường - không chặn việc khác

## Bối cảnh

`tsudev-standards/docs/PACKAGING.md` mục 3 yêu cầu mỗi repo có phân phối có `packaging/package.json`. Đã thêm ở
trafficmonitor, lotus, swico, cwico, contact (xem `tsudev-org/tsudev-lotus/packaging/package.json` làm mẫu đầy đủ,
có khối `linux`). Kho APT/RPM `pkg.tsudev.com` đang dựng ở `tsudev-org/linux-packages`; bộ dựng đọc
`packaging/package.json` của từng repo trong `products.json`, đóng gói **chính tệp `linux-*.tar.gz` đã phát hành**
(không build lại) sau khi kiểm SHA-256.

tsudev-files đã phát hành `tsudev-files-{version}-linux-x64.tar.gz` và `-linux-arm64.tar.gz` nên đủ điều kiện.

## Việc cần làm (trong repo này)

1. Thêm `packaging/package.json` (schema 1). Gợi ý - kiểm lại tên tệp chạy, biểu tượng, thư viện phụ thuộc thật:

```json
{
  "schema": 1, "name": "tsudev-files", "winget": "tsudev.Files",
  "homepage": "https://files.tsudev.com", "license": "MIT",
  "description": { "vi": "...", "en": "..." },
  "release": { "repo": "tsudev-org/tsudev-files",
               "tagRegex": "^v(?<version>\\d+\\.\\d+\\.\\d+-vn\\.\\d+)$",
               "sums": "tsudev-files-{version}-SHA256SUMS.txt" },
  "assets": {
    "windows-x64":           { "file": "tsudev-files-{version}-win-Portable.zip", "kind": "portable", "bin": "Filee.exe" },
    "windows-x64-installer": { "file": "tsudev-files-{version}-win-Setup.exe", "kind": "installer" },
    "linux-x64":   { "file": "tsudev-files-{version}-linux-x64.tar.gz",   "kind": "tar", "bin": "<tệp chạy trong tar>" },
    "linux-arm64": { "file": "tsudev-files-{version}-linux-arm64.tar.gz", "kind": "tar", "bin": "<tệp chạy trong tar>" },
    "macos-arm64": { "file": "tsudev-files-{version}-osx-arm64.dmg", "kind": "dmg", "app": "<tên .app>" }
  },
  "linux": {
    "command": "tsudev-files",
    "icon": "<đường dẫn PNG trong tar>",
    "desktop": { "name": "tsudev Files", "genericName": { "vi": "Quản lý tệp", "en": "File manager" },
                 "categories": ["Utility", "FileManager"], "keywords": ["files"], "wmClass": "<nếu có>" },
    "deb": { "depends": ["<thư viện thật, vd libicu / libssl>"], "section": "utils" },
    "rpm": { "depends": ["<dạng soname, vd libssl.so.3()(64bit)>"], "group": "Applications/System" }
  },
  "channels": ["scoop", "brew"],
  "planned": ["winget", "apt", "rpm"]
}
```

   Lưu ý: tệp tổng kiểm của Release có tiền tố phiên bản (`tsudev-files-2.4.0-vn.1-SHA256SUMS.txt`) - bộ dựng
   hiện chỉ nhận tên cố định ở `release.sums`; nếu cần `{version}` trong `sums`, báo phiên đóng gói để sửa
   `tools/dung_kho.py` (một dòng `.replace("{version}", ...)`).
   Phiên bản `2.4.0-vn.1` chứa `-`: deb chấp nhận, rpm thì không - nên tách `version` = `2.4.0`, `revision` = `vn.1`
   bằng nhóm `(?<revision>...)` trong `tagRegex`.

2. PR → CI → merge, rồi nhắn phiên đóng gói (hoặc tự mở PR thêm `{ "repo": "tsudev-org/tsudev-files" }` vào
   `products.json` của `tsudev-org/linux-packages` - CI ở đó cài thử thật trên Ubuntu/Debian/Fedora).

## Kết quả

- 11/11/2026 - phiên tsudev-files-61: thêm `packaging/package.json` (nhánh `feat/packaging-package-json`). Đối chiếu
  Release `v2.4.0-vn.1`: mọi tên tệp khai báo đều có. `tagRegex` tách `version` = `2.4.0`, `revision` = `vn.1`.
- Bộ dựng hiện tại CHƯA đóng gói được (tên tệp chứa `2.4.0-vn.1`, tổng kiểm Linux ở tệp riêng theo kiến trúc, tar có
  thư mục gốc, lệnh `tsudev-files` là CLI còn trình đơn chạy GUI, cần thêm lệnh `filee`). Đã thêm các trường
  `{tagVersion}`, `assets.*.sums`, `assets.*.root`, `linux.desktop.exec`, `linux.links` và gửi phiếu
  `~/projects/logs/handover/20261111-03_yeu-cau-linux-packages-ho-tro-tsudev-files.md` cho phiên đóng gói
  (`projects-5d`). `apt`, `rpm` để ở `planned` cho tới khi CI cài thử của `linux-packages` đạt.
