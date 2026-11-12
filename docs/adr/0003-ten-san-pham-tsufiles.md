# ADR-0003: Tên sản phẩm TsuFiles, logo con cú, lệnh `tsufiles`

- **Trạng thái**: Đã chấp nhận (12/11/2026, chủ dự án)
- **Thay thế một phần**: ADR-0001 (tên hiển thị `tsudev-files`), ADR-0002 mục 3 (tên đích `Tsudev.Files.*`,
  `tsudev-files.exe`, giữ bí danh `filee`)

## 1. Bối cảnh

Chủ dự án (12/11/2026) chốt tên sản phẩm mới **TsuFiles** cho thống nhất với hệ sinh thái tsudev, kèm bộ logo con cú
(`assets/brand/source/tsufiles-1.webp`, `tsufiles-2.webp`). Mọi tên cũ (`Filee`, `filee`, `tsudev-files`,
`Tsudev.Files`) phải biến mất khỏi sản phẩm, trừ chỗ giấy phép hoặc lịch sử bắt buộc giữ.

## 2. Quyết định

| Hạng mục | Trước | Sau |
| --- | --- | --- |
| Tên hiển thị | `tsudev-files` | **TsuFiles** |
| Namespace, project, assembly thư viện | `Tsudev.Files.*` | `TsuFiles.*` |
| Định danh C#/MSBuild | `TsudevFiles*`, `Filee*` | `TsuFiles*` |
| Tệp chạy | `Filee.exe`, `filee-cli` | `TsuFiles.exe`, `tsufiles-cli` |
| Lệnh dòng lệnh | `filee` | `tsufiles` - **không** giữ bí danh `filee` (chủ dự án chọn, chấp nhận script cũ gãy) |
| Biến môi trường | `FILEE_*` | `TSUFILES_*` |
| Thư mục dữ liệu, khóa registry, AppGuid | `Filee` | `TsuFiles` + di trú một lần từ chỗ cũ |
| Tệp phát hành | `tsudev-files-<ver>-...` | `tsufiles-<ver>-...` (trình cập nhật bản mới nhận cả hai tiền tố) |
| Repo GitHub | `tsudev-org/tsudev-files` | `tsudev-org/tsufiles` - đổi ở phase cuối |
| Website | `files.tsudev.com` | giữ nguyên tên miền; Worker đổi tên `tsufiles` |
| Giao diện | `Soft`, `Material You` | thêm kiểu **TsuFiles Cú đêm** (đen + tím theo logo), mặc định cho cài mới |

Giữ nguyên: thông báo bản quyền MIT của Myeonggu Kim (KnifeLemon) và Filee contributors trong `LICENSE`,
`THIRD-PARTY-NOTICES.md` (bắt buộc theo giấy phép); khóa JSON đầu ra của dòng lệnh; lịch sử (`CHANGELOG.md` bản cũ,
`logs/`, ADR cũ).

## 3. Thứ tự thực hiện (mỗi phase một PR)

1. **T1 - logo**: con cú thay logo dải lụa trong dây chuyền `tools/thuong-hieu/tao-tai-san.py` (gộp cùng PR P6 #47).
2. **T2 - mã nguồn + hiển thị**: `tools/doi-ten-tsudev.py` thêm bảng `Tsudev.Files` -> `TsuFiles`, chạy `dotnet format`;
   chuỗi giao diện/tài liệu/website sang TsuFiles; kiểu giao diện Cú đêm.
3. **T3 - định danh lúc chạy** (bản 2.4.1, thay P5b): exe, lệnh, biến môi trường, thư mục dữ liệu + di trú, Setup tiếp
   quản bản `2.4.0-vn.*` đã cài và thay lối tắt/icon cũ, ExplorerMenu/COM, GNOME, bundle macOS.
4. **T4 - repo**: đổi tên repo `tsudev-org/tsufiles`, URL trong `ForkDefaults`/`Directory.Build.props`, `git remote`,
   thư mục cục bộ, Worker. Tên `tsudev-files` vào danh sách cấm tạo lại (chuyển hướng GitHub mất ngay khi tên cũ bị
   chiếm, bản 2.4.0-vn.1 đã cài sẽ không cập nhật được).

## 4. Hệ quả

- Bản 2.4.0-vn.1 đã cài cập nhật lên 2.4.1 qua URL cũ (GitHub chuyển hướng), Setup 2.4.1 đổi lối tắt, icon, thư mục.
- Script gọi `filee` phải sửa thành `tsufiles` (ghi rõ trong CHANGELOG 2.4.1).
- Ảnh bìa `tsufiles-1.webp` có chữ "tsufiles.io" và "for the web" không đúng sự thật (sản phẩm là ứng dụng desktop,
  tên miền là files.tsudev.com): chỉ dùng phần con cú, không đăng nguyên ảnh.
