# Tài sản thương hiệu tsudev-files

Một ảnh gốc, một script, mọi biểu tượng/logo/favicon sinh ra từ đó (`.standards/docs/BRAND_ASSETS.md` mục 7).
**Không sửa tay tệp sinh ra** - lần chạy sau sẽ ghi đè.

| Tệp | Là gì |
| --- | --- |
| `source/tsufiles-1.webp` | **Ảnh gốc của icon**: ảnh bìa TsuFiles 1024x512; script cắt con cú trong `OWL_BOX`. SHA-256 ghi trong `tools/thuong-hieu/tao-tai-san.py` |
| `source/tsufiles-2.webp` | Ảnh giới thiệu TsuFiles 845x752 (WebP lossless, giống từng điểm ảnh bản PNG chủ dự án gửi). Chưa dùng trong tệp sinh ra |
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
cp <ảnh mới>.webp assets/brand/source/tsufiles-1.webp          # WebP/PNG lossless; sửa OWL_BOX nếu bố cục khác
sha256sum assets/brand/source/tsufiles-1.webp                  # chép vào SOURCE_SHA256 trong script
python3 -m venv /tmp/brand && /tmp/brand/bin/pip install -r tools/thuong-hieu/requirements.txt
/tmp/brand/bin/python -I tools/thuong-hieu/tao-tai-san.py      # sinh lại tất cả, cuối cùng tự kiểm (ĐẠT)
dotnet test tests/Tsudev.Files.App.Tests --filter "FullyQualifiedName~BrandAssetsTests"
```

Rồi xem bằng mắt: icon 16/24/32 px trên nền sáng và tối, `og-image.png`, đầu trang website (`cd website && npm run dev`).

## Cách script xử lý ảnh

- **Tách nền**: cắt `OWL_BOX`, ước lượng nền tím nhạt chuyển sắc bằng mặt phẳng khớp với mép khung, rồi color-to-alpha
  theo khoảng cách tới nền tại chỗ. Chỉ vùng nối với mép khung mới là nền, nên lòng trắng mắt con cú vẫn đục; mảnh
  lạc (chữ "T" của dòng TsuFiles bên cạnh) bị bỏ, quầng sáng tím giữ dạng bán trong suốt.
- **Đổi cỡ** bằng LANCZOS trên alpha nhân sẵn (không viền tối quanh hình). Con cú trong ảnh bìa chỉ cao khoảng 160 px,
  nên cỡ 192/256/512 là phóng to + làm nét: đủ dùng nhưng chưa nét tuyệt đối. Có ảnh con cú riêng >= 1024 px (hoặc SVG)
  thì thay ảnh gốc, sửa `OWL_BOX`, chạy lại.
- **Cỡ nhỏ (<= 48 px)** cắt sát phần hình đặc, bỏ quầng mờ và làm nét nhẹ, để 16 px vẫn nhận ra hình.
- `.ico` có đủ lớp 16/20/24/32/40/48/64/96/128/256 (Windows chọn đúng lớp ở mọi mức phóng 100-250 %).
- Ảnh gốc là ảnh bitmap có hiệu ứng quầng sáng và chuyển màu, nên không có bản SVG: vector hóa tự động sẽ làm mất
  chi tiết. Nếu sau này có bản vẽ vector, đặt vào `source/` và cho script đọc thay.
