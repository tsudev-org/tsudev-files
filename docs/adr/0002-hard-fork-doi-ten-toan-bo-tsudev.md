# ADR-0002: Tách hẳn khỏi upstream (hard fork) - đổi tên toàn bộ mã nguồn sang tsudev

- **Trạng thái**: ĐÃ CHẤP NHẬN (thay thế phương án C của [ADR-0001](0001-doi-ten-hien-thi-tsudev-files.md))
- **Ngày**: 11/11/2026
- **Người quyết định**: tsudev-tsudev (chủ dự án)
- **Liên quan**: `tools/doi-ten-tsudev.py`, `docs/vi/KIEN-TRUC-BAO-MAT.md`, `LICENSE`, `THIRD-PARTY-NOTICES.md`

## 1. Bối cảnh

tsudev-files là fork MIT của `KnifeLemon/Filee` (v2.4.0, `64890f1e`). ADR-0001 chỉ đổi tên hiển thị để gộp upstream
dễ. Từ 11/11/2026 sản phẩm phát triển theo hệ sinh thái tsudev; chủ dự án chọn đổi tên toàn bộ, chấp nhận không
gộp tự động từ upstream nữa.

- Còn khoảng 12.000 chỗ "Filee" trong 1.197 file: namespace, project, assembly, tệp chạy, thư mục dữ liệu, registry,
  COM, tiện ích GNOME, bundle macOS, biến môi trường, tài liệu.
- Bản `v2.4.0-vn.1` đã phát hành và đang được cài: trình cập nhật, thư mục dữ liệu, lối tắt, PATH phải được di trú.
- Giấy phép MIT bắt buộc giữ thông báo bản quyền của tác giả gốc trong mọi bản sao.

## 2. Các phương án đã cân nhắc

| Phương án | Ưu | Nhược |
| --- | --- | --- |
| A. Giữ ADR-0001 | Gộp upstream dễ | Mã nguồn vẫn mang tên sản phẩm khác |
| B. Tách định danh lúc chạy, giữ namespace | Cài song song Filee gốc; vẫn gộp được phần lớn | Mã vẫn ghi `Filee.*` |
| C. Hard fork: đổi cả mã nguồn lẫn định danh lúc chạy | Toàn bộ thuộc tsudev; cài song song Filee gốc | Không gộp tự động được upstream; phải di trú bản đã cài |

## 3. Quyết định

Chọn **C**, làm hai bước để không làm hỏng bản đã cài:

- **P5a - mã nguồn** (không đổi hành vi): `Filee.<X>` -> `Tsudev.Files.<X>` (namespace, project, assembly thư viện,
  `Filee.Setup` -> `Tsudev.Files.Setup`), `Filee.slnx` -> `tsudev-files.slnx`, định danh `Filee*` của C#/MSBuild ->
  `TsudevFiles*` (ví dụ `FileeUpdatePublicKey` -> `TsudevFilesUpdatePublicKey`). Bằng `tools/doi-ten-tsudev.py`.
- **P5b - định danh lúc chạy** (có di trú): `Filee.exe` -> `tsudev-files.exe`, `filee-cli` -> `tsudev-files-cli`
  (giữ lệnh `filee` làm bí danh cho script cũ), AppGuid/khóa gỡ cài đặt mới và Setup tiếp quản bản `2.4.x-vn` đã cài,
  thư mục dữ liệu `tsudev-files` (sao dữ liệu cũ khi chạy lần đầu), biến `TSUDEV_FILES_*` (vẫn đọc `FILEE_*`),
  ExplorerMenu/COM, tiện ích GNOME, bundle macOS, tiêu đề cửa sổ donut (luật KWin/Hyprland).

Giữ nguyên:

- `LICENSE`: thông báo bản quyền MIT của Myeonggu Kim (KnifeLemon) và Filee contributors - **bắt buộc** theo giấy
  phép. Thêm dòng bản quyền tsudev cho phần sửa đổi và mã mới.
- Định dạng JSON đầu ra của dòng lệnh (script bên ngoài phụ thuộc).
- Lịch sử (`CHANGELOG.md` các bản cũ, `logs/`, ADR-0001).

## 4. Hệ quả

- Không còn `git merge upstream/main`. Bản vá bảo mật của upstream được theo dõi và chép thủ công:
  `git format-patch` từ upstream, chạy `python3 -I tools/doi-ten-tsudev.py --tep <tệp>` rồi `git apply`.
- Quy tắc "đánh dấu `tsudev-files:` mỗi chỗ sửa mã upstream" (AGENTS.md B.4) và miễn trừ em-dash theo dòng
  (`.standards-upstream`) hết lý do tồn tại sau P5a; em-dash trong mã được sửa dần theo quy ước mục 7.1, trừ dữ liệu
  test cố ý chứa ký tự ngoài Latin-1.
- `ForkDefaults.Brand` (đổi tên hiển thị lúc chạy) còn cần cho tới khi các bản dịch hết chữ "Filee" ở P5b.
