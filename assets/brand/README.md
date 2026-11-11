# Tài sản thương hiệu tsudev-files

Một ảnh gốc, một script, mọi biểu tượng/logo/favicon sinh ra từ đó (`.standards/docs/BRAND_ASSETS.md` mục 7).
**Không sửa tay tệp sinh ra** - lần chạy sau sẽ ghi đè.

| Tệp | Là gì |
| --- | --- |
| `source/logo-tsudev-files.png` | **Ảnh gốc** (976x878, nền trắng). SHA-256 ghi trong `tools/thuong-hieu/tao-tai-san.py` |
| `tsudev-files-mark.{png,webp,avif}` | Bản chính đã tách nền, vuông, nền trong suốt (813x813) - dùng cho tài liệu, bài viết, ảnh ghép |
| `tsudev-files-mark-512.png` | Bản 512 px cho README |

Script còn sinh (cùng nguồn):

| Tệp | Dùng cho |
| --- | --- |
| `src/Tsudev.Files.App/Assets/Icons/filee.png` (512) | Trang Giới thiệu, đầu cửa sổ chính, hộp thoại, logo động của Setup (`AppLogo`), icon gói Linux, icon macOS (`build/build-portable.ps1` dựng `.icns`), logo gói menu Explorer |
| `src/Tsudev.Files.App/Assets/Icons/filee.ico` | Icon cửa sổ, thanh tác vụ, khay hệ thống; icon `.exe` của ứng dụng, Setup, CLI |
| `website/public/favicon.ico`, `favicon-{16,32,96}.png` | Favicon files.tsudev.com |
| `website/public/apple-touch-icon.png` | iOS (180, nền `#0A0A0A`) |
| `website/public/android-chrome-*.png` | PWA 192/512, bản `maskable` có nền và vùng an toàn |
| `website/public/brand/tsudev-files-mark.{avif,webp,png}` | Logo đầu trang website (AVIF -> WebP -> PNG qua `<picture>`) |
| `website/public/og-image.png` | Ảnh chia sẻ 1200x630 (màu theo token chế độ Tối) |

`website/public/brand/logo-mark.png` là **con cú tsudev** (dây chuyền của repo `tsudev-org/tsudev`), chỉ dùng ở chân
trang cho liên kết về hệ sinh thái tsudev.com - script này không đụng tới nó.

## Đổi logo

```bash
cp <ảnh mới>.png assets/brand/source/logo-tsudev-files.png      # PNG, nền trắng hoặc trong suốt, cạnh ngắn >= 512
sha256sum assets/brand/source/logo-tsudev-files.png            # chép vào SOURCE_SHA256 trong script
python3 -m venv /tmp/brand && /tmp/brand/bin/pip install -r tools/thuong-hieu/requirements.txt
/tmp/brand/bin/python -I tools/thuong-hieu/tao-tai-san.py      # sinh lại tất cả, cuối cùng tự kiểm (ĐẠT)
dotnet test tests/Tsudev.Files.App.Tests --filter "FullyQualifiedName~BrandAssetsTests"
```

Rồi xem bằng mắt: icon 16/24/32 px trên nền sáng và tối, `og-image.png`, đầu trang website (`cd website && npm run dev`).

## Cách script xử lý ảnh

- **Tách nền trắng** bằng color-to-alpha có ngưỡng: điểm càng xa màu trắng càng đục, màu được gỡ trộn khỏi nền nên
  quầng sáng thành bán trong suốt đúng sắc (đặt lên nền tối vẫn tự nhiên); vùng hình đặc giữ nguyên từng điểm ảnh,
  nhiễu khối JPEG ở vùng gần trắng được làm mịn. Ghép lại lên nền trắng lệch trung bình khoảng 1/255 so với ảnh gốc.
- **Thu nhỏ** bằng LANCZOS trên alpha nhân sẵn (không viền tối quanh hình). Không phóng to.
- **Cỡ nhỏ (<= 48 px)** cắt sát phần hình đặc, bỏ quầng mờ và làm nét nhẹ, để 16 px vẫn nhận ra hình.
- `.ico` có đủ lớp 16/20/24/32/40/48/64/96/128/256 (Windows chọn đúng lớp ở mọi mức phóng 100-250 %).
- Ảnh gốc là ảnh bitmap có hiệu ứng quầng sáng và chuyển màu, nên không có bản SVG: vector hóa tự động sẽ làm mất
  chi tiết. Nếu sau này có bản vẽ vector, đặt vào `source/` và cho script đọc thay.
