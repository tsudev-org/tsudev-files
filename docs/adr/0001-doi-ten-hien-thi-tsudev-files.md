# ADR-0001: Tên hiển thị của ứng dụng là "tsudev-files", định danh nội bộ giữ "Filee"

- **Trạng thái**: ĐÃ CHẤP NHẬN
- **Ngày**: 09/11/2026
- **Người quyết định**: tsudev-tsudev (chủ dự án)
- **Liên quan**: `logs/STATE.md` (hàng đợi task), `docs/vi/KIEN-TRUC-BAO-MAT.md` mục 6, `src/Filee.Core/ForkDefaults.cs`

## 1. Bối cảnh

Bản fork mang tên `tsudev-files` nhưng giao diện, trình cài đặt và tệp phát hành vẫn ghi "Filee". Chữ này có trong
khoảng 1.200 file: namespace, tên tệp chạy, thư mục dữ liệu, khóa registry, 22 tệp dịch, tài liệu.

- Ràng buộc kỹ thuật: upstream `KnifeLemon/Filee` thay đổi hằng ngày; mỗi chỗ sửa trong mã upstream là một điểm xung
  đột khi gộp. Luật cửa sổ KWin/Hyprland nhận cửa sổ donut theo tiêu đề "Filee Donut" và lớp cửa sổ "filee".
- Ràng buộc 0 đồng: không phát sinh chi phí.
- Mức dữ liệu: D3 (khóa ký cập nhật) - không đổi; tên tệp phát hành liên quan trình cập nhật nên phải kiểm kỹ.

## 2. Các phương án đã cân nhắc

| Phương án | Ưu | Nhược | Chi phí |
| --- | --- | --- | --- |
| A. Đổi toàn bộ (namespace, tệp chạy, thư mục dữ liệu, registry) | Tên thống nhất tuyệt đối | Xung đột gần như mọi lần gộp; phải di trú dữ liệu và registry; dễ hỏng cập nhật | Rất cao |
| B. Sửa chuỗi trong 22 tệp dịch + tiêu đề từng cửa sổ | Đơn giản, dễ hiểu | Xung đột ở mọi dòng dịch upstream sửa; chuỗi mới của upstream lại ghi "Filee" | Trung bình, kéo dài |
| C. Đổi tên lúc chạy bằng một hàm (`ForkDefaults.Brand`) + sửa có chọn lọc chỗ hiển thị ngoài bản dịch | Gần như không xung đột; chuỗi mới tự đổi; test kiểm tự động | Phải giữ danh sách loại trừ định danh | Thấp |
| D. Không làm gì | 0 | Người dùng thấy tên khác với sản phẩm | 0 |

## 3. Quyết định

Chọn phương án **C**.

Đổi (những gì người dùng nhìn thấy):

- Mọi chuỗi dịch của ứng dụng, dòng lệnh, trình cài Windows: `ForkDefaults.Brand` chạy lúc nạp tệp dịch
  (`LocalizationService.LoadStrings`, `CliTexts`, `CliLanguage.T`, `Help.Text`, `SetupTexts.Load`).
- Tiêu đề mọi cửa sổ: `ForkWindowTitles` (một class handler trên `Window.TitleProperty`), tooltip khay hệ thống.
- Windows: tên trong Apps & Features, nhà phát hành `tsudev`, lối tắt `tsudev-files.lnk` (dọn `Filee.lnk` cũ khi
  cài/gỡ), `ProductName` trong thuộc tính tệp, tiêu đề dự phòng của menu Explorer.
- Linux: `Name=` của lối tắt ứng dụng và mục tự khởi động, tên script Nautilus (dọn script "Filee" cũ), tên/mô tả
  extension GNOME và KWin, thông báo của `install.sh`/`uninstall.sh`, `INSTALL.txt`, `HUONG-DAN-CAI-DAT.txt`.
- macOS: `CFBundleName`, `CFBundleDisplayName`, lời giải thích quyền, bản dịch trong `*.lproj`.
- Tệp phát hành: `tsudev-files-<phiên bản>-win-Setup.exe`, `...-Portable.zip`, `...-SHA256SUMS.txt`,
  `tsudev-files-<phiên bản>-linux-x64.tar.gz`, ... Regex `VersionInName` nhận cả tiền tố cũ lẫn mới.

Giữ nguyên (định danh, đổi là hỏng tương thích hoặc gây xung đột lớn):

- Namespace và assembly `Filee.*`, tệp chạy `Filee.exe`/`Filee`/`Filee.app`, lệnh `filee`/`filee-cli`.
- Thư mục dữ liệu `%APPDATA%\Filee`, `~/.config/Filee`, `~/.local/share/filee`, bộ nhớ đệm, tệp tạm `Filee-*`.
- Khóa registry, `AppGuid` của Inno Setup, tên giá trị Run, bundle id `com.filee.app`, `NSPortName`, uuid extension
  GNOME, tên gói MSIX của menu Explorer (`ExplorerMenuPackage`), `Application.Name` (lớp cửa sổ "filee").
- Tiêu đề "Filee Donut" (luật KWin/Hyprland dựa vào), khóa dịch macOS "Convert with Filee".
- Chuỗi tiếng Anh viết cứng trong `src/Filee.Engines` (ví dụ "being installed by another Filee", siêu dữ liệu
  `Creator`/`Producer` của tệp xuất ra): giữ để không đụng mã engine của upstream.
- Ghi công: README, `LICENSE`, trợ giúp dòng lệnh nêu rõ đây là bản Việt hóa của `KnifeLemon/Filee` (MIT).

## 4. Hệ quả

**Tích cực:**

- Gộp upstream không xung đột ở tệp dịch; chuỗi mới của upstream tự mang tên mới.
- `tests/Filee.Core.Tests/ForkBrandingTests.cs` quét mọi chuỗi của 22 ngôn ngữ, gộp upstream mà lọt chữ "Filee" ở dạng
  lạ (ví dụ đi kèm ký tự mới) thì test đỏ.

**Tiêu cực và chi phí phải trả:**

- Bản fork và Filee gốc dùng chung thư mục dữ liệu, `AppGuid` và khóa registry: cài cả hai trên một máy Windows thì
  bản cài sau ghi đè bản trước (đã có từ khi khởi tạo fork, ADR này không làm xấu thêm). Muốn cài song song cần ADR
  riêng (đổi `AppGuid`, thư mục dữ liệu, di trú).
- Vài chỗ ít gặp vẫn hiện "Filee": thông báo lỗi của engine, siêu dữ liệu tệp xuất ra, nhật ký kỹ thuật.
- Tiếng Anh viết "tsudev-files's" ở dạng sở hữu - chấp nhận được, ưu tiên không sửa từng câu.

**Việc phát sinh cần làm:**

- [ ] Khi gộp upstream: chạy `dotnet test tests/Filee.Core.Tests --filter ForkBranding` và test của các bộ cài.
- [ ] Nếu upstream thêm chỗ hiển thị tên ngoài bản dịch (tiêu đề, metadata gói), bổ sung theo danh sách "Đổi" ở trên.

## 5. Đường lùi

Đặt `ForkDefaults.DisplayName = "Filee"` thì toàn bộ chuỗi dịch, tiêu đề, tên lối tắt và Setup trở về như cũ. Tiền tố
tệp phát hành ở `build/*.ps1` và `.github/workflows/release.yml` sửa lại bằng tay; trình cập nhật nhận cả hai tiền tố
nên không cần phát hành chuyển tiếp.
