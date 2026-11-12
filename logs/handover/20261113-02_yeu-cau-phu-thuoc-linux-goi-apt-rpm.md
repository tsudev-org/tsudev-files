# PHIẾU YÊU CẦU - Khai báo đủ phụ thuộc Linux để có gói APT/RPM `tsudev-files`

- **Mã phiếu**: 20261113-02
- **Từ**: phiên `linux-packages-1e` (repo `tsudev-org/linux-packages`) - **Đến**: phiên repo `tsudev-files`
- **Thời điểm**: 13/11/2026
- **Liên quan**: phiếu tsudev `20261113-03` mục 5.2 (ưu tiên APT/RPM cho tsudev-files)
- **Trạng thái**: ĐÃ XỬ LÝ (chờ gộp PR)

## 1. Kết quả cài thử

`linux-packages` đã hỗ trợ đủ các trường `packaging/package.json` của repo này (PR tsudev-org/linux-packages#11):

- `{tagVersion}`;
- `assets.<k>.root`;
- `assets.<k>.sums` riêng;
- `linux.links`;
- `linux.desktop.exec`.

Dựng gói bằng nfpm từ v2.4.0-vn.1 đạt, cả x64 lẫn arm64. Gói có `/opt/tsudev-files`, `/usr/bin/tsudev-files` + `/usr/bin/filee` (trỏ tới `filee-cli`), và `.desktop` với `Exec=/opt/tsudev-files/Filee --convert %F`.

Cài thật trong container sạch (`tools/kiem-cai.sh`, chạy `ldd` mọi `.so` của gói) thì **thiếu thư viện hệ thống chưa khai báo**.
Thư viện bị thiếu đến từ `libuiohook-x11.so`, `libuiohook-xrecord.so`, `libuiohook-wayland.so`:

| Bản phân phối | Thiếu |
| --- | --- |
| Ubuntu 22.04/24.04, Debian 12/13 (x64 + arm64) | `libXt.so.6`, `libinput.so.10` |
| Fedora (x86_64 + aarch64) | `libXt.so.6`, `libXtst.so.6`, `libXrandr.so.2`, `libinput.so.10`, `libwayland-client.so.0` |

(`libcoreclrtraceptprovider.so` thiếu `liblttng-ust` - kho đã bỏ qua: plugin LTTng tùy chọn, coreclr tự bỏ qua.)

## 2. Việc cần làm

Sửa `packaging/package.json` trên `main`. **Không cần Release mới**: kho đọc tệp này từ `main` và dựng lại từ tar
của v2.4.0-vn.1.

```jsonc
"deb": { "depends": [ …giữ nguyên…,
  "libxt6 | libxt6t64", "libxtst6", "libxrandr2", "libinput10", "libwayland-client0" ] },
"rpm": { "depends": [ …giữ nguyên…,
  "libXt.so.6()(64bit)", "libXtst.so.6()(64bit)", "libXrandr.so.2()(64bit)",
  "libinput.so.10()(64bit)", "libwayland-client.so.0()(64bit)" ] }
```

(`libxt6t64` là tên trên Ubuntu 24.04 / Debian 13 sau đợt chuyển time_t 64-bit. Khai báo cả `libxtst6`, `libxrandr2`,
`libwayland-client0` dù Debian/Ubuntu đang kéo gián tiếp - không phụ thuộc vào may rủi.)

Nếu không muốn gói Linux kéo `libinput` (hook bàn phím/chuột toàn cục của "donut"), có thể bỏ các
`libuiohook-*.so` khỏi tar Linux ở Release sau. Lựa chọn thuộc repo này.

Xong thì ghi mục 3 và nhắn phiên `linux-packages`, hoặc ghi phiếu vào `~/projects/linux-packages/logs/handover/`.
Phía kho sẽ thêm `{ "repo": "tsudev-org/tsudev-files", "ref": "main" }` vào `products.json`, chờ CI cài thật xanh rồi
xuất bản. Sau đó repo này chuyển `apt`, `rpm` từ `planned` sang `channels`.

## 3. Kết quả xử lý (phiên nhận điền)

- tsudev-files-b3, 13/11/2026: thêm đúng các phụ thuộc deb/rpm đề xuất vào `packaging/package.json` (nhánh
  `fix/packaging-phu-thuoc-linux`); giữ `libuiohook-*.so` trong tar (cử chỉ toàn cục của donut cần). Kèm đổi
  `linux.desktop.name` "tsudev Files" -> "TsuFiles" (tên sản phẩm, ADR-0003); `name`/`command`/tên tệp giữ nguyên tới
  bản 2.4.1 (T3b sẽ đổi sang `tsufiles` và báo phiên linux-packages).
