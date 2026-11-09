# tsudev-files

**Trình chuyển đổi tệp ngoại tuyến, miễn phí, mã nguồn mở - bản Việt hóa và gia cố bảo mật của [Filee](https://github.com/KnifeLemon/Filee).**

Một sản phẩm của [tsudev](https://tsudev.com).

**Hạng sản phẩm: C - Ngoại tuyến** (theo `.standards/docs/AUTH_AND_ACCOUNT.md` mục 1): mọi chức năng chuyển đổi chạy
trọn vẹn không cần internet và không cần tài khoản; kiểm tra cập nhật là tùy chọn.

Kéo tệp, thả vào một định dạng trên vòng "donut" mở ra ngay tại con trỏ chuột là xong. Hơn 260 định dạng: ảnh, PDF,
Word, Excel, PowerPoint, HWP/HWPX, video, âm thanh, sách điện tử, tệp nén, CAD, bản đồ, 3D... Mọi xử lý chạy **trên
máy của bạn**, không tải tệp lên bất kỳ máy chủ nào, không cần cài Microsoft Office.

- Tải bản phát hành: <https://github.com/tsudev-org/tsudev-files/releases/latest>
- Báo lỗi / đề xuất: <https://github.com/tsudev-org/tsudev-files/issues>
- Báo lỗ hổng bảo mật: xem [SECURITY.md](SECURITY.md) (KHÔNG mở issue công khai)
- README gốc (tiếng Anh) của upstream: [README.en.md](README.en.md)

## Khác biệt so với bản gốc

| Hạng mục | Bản gốc Filee | tsudev-files |
| --- | --- | --- |
| Ngôn ngữ mặc định | Theo ngôn ngữ hệ điều hành, dự phòng tiếng Anh | **Tiếng Việt** cho giao diện, trình cài đặt và dòng lệnh; đổi được trong Cài đặt (`auto`, `en`, ...) |
| Dòng lệnh `filee` | Chỉ tiếng Anh | Tiếng Việt mặc định, `FILEE_LANG=en` để dùng tiếng Anh; khóa JSON giữ nguyên cho script |
| Kiểm tra cập nhật | `KnifeLemon/Filee` + khóa ký của tác giả gốc | `tsudev-org/tsudev-files` + **khóa ký ECDSA P-256 riêng**; bản phát hành của repo khác bị từ chối |
| Tên hiển thị | Filee | **tsudev-files** (giao diện, trình cài đặt, lối tắt, tệp phát hành); tệp chạy, lệnh `filee` và thư mục dữ liệu giữ tên cũ - xem [ADR-0001](docs/adr/0001-doi-ten-hien-thi-tsudev-files.md) |
| Trang tải khi cập nhật lỗi | filee.sh | Trang phát hành của repo này |
| GitHub Actions | Ghim theo tag | **Ghim theo SHA commit** (chống tag bị sửa) |
| Cổng kiểm bảo mật | CI + test | Thêm CodeQL (security-extended), gitleaks, soát lỗ hổng NuGet, kiểm tra bản dịch, Dependabot |

Chi tiết kiến trúc bảo mật: [docs/vi/KIEN-TRUC-BAO-MAT.md](docs/vi/KIEN-TRUC-BAO-MAT.md).

## Phát triển

Yêu cầu: .NET SDK 10 (`global.json`), Git. Build được trên Windows, Linux và WSL; bộ cài Windows chỉ build trên Windows
(xem [docs/vi/PHAT-HANH.md](docs/vi/PHAT-HANH.md)).

Chuẩn bị một lần sau khi clone (giống CI): phông Noto và engine dùng trong test, đều ghim SHA-256, không vào git.

```bash
dotnet tool install --global PowerShell   # cần cho script build/*.ps1 trên Linux/WSL
pwsh -NoProfile -File build/fetch-fonts.ps1
tools/fetch-engines-linux.sh 7zip         # Windows: pwsh build/fetch-engines.ps1 -Only rhwp,pandoc,7zip,exiftool
```

```bash
dotnet build Filee.slnx
dotnet test tests/Filee.Core.Tests        # các bộ test khác: tests/Filee.*.Tests
# Bộ giao diện chạy riêng, có phát hiện treo (giống CI; khoảng 30-60 phút trên WSL):
dotnet test tests/Filee.App.Tests --no-build --blame-crash --blame-hang-timeout 5m
python3 -I tools/check_i18n_vi.py         # kiểm tra bản dịch tiếng Việt
bash scripts/check-standards.sh           # cổng kiểm quy ước tsudev
```

Chạy dòng lệnh từ mã nguồn:

```bash
dotnet run --project src/Filee.Cli -- help
FILEE_LANG=en dotnet run --project src/Filee.Cli -- help
```

Quy ước làm việc (cho người và agent AI): đọc [AGENTS.md](AGENTS.md) trước tiên.

## Đồng bộ với upstream

```bash
git fetch upstream
git merge upstream/main     # giải quyết xung đột, ưu tiên giữ ForkDefaults.cs và Directory.Build.props của fork
python3 -I tools/check_i18n_vi.py && dotnet test tests/Filee.App.Tests
```

Remote `upstream` đã bị khóa chiều đẩy (`DISABLED`) để không thể vô tình đẩy lên repo gốc.

## Ủng hộ tác giả gốc

Filee miễn phí và sẽ luôn miễn phí. Nếu muốn ủng hộ, hãy ủng hộ trực tiếp tác giả gốc (Myeonggu Kim - KnifeLemon);
nhà tài trợ được liệt kê tại đây và trong trang "Giới thiệu" của ứng dụng.

<!-- sponsors: src/Filee.App/Assets/Sponsors/sponsors.json -->

<p align="center">
  <a href="https://github.com/sponsors/KnifeLemon"><img src="https://img.shields.io/badge/GitHub%20Sponsors-EA4AAA?style=for-the-badge&logo=githubsponsors&logoColor=white" alt="GitHub Sponsors"></a>
  <a href="https://patreon.com/KnifeLemon"><img src="https://img.shields.io/badge/Patreon-000000?style=for-the-badge&logo=patreon&logoColor=white" alt="Patreon"></a>
  <a href="https://ko-fi.com/knifelemon"><img src="https://img.shields.io/badge/Ko--fi-FF6433?style=for-the-badge&logo=kofi&logoColor=white" alt="Ko-fi"></a>
  <a href="https://paypal.me/knifelemon"><img src="https://img.shields.io/badge/PayPal-002991?style=for-the-badge&logo=paypal&logoColor=white" alt="PayPal"></a>
</p>

## Giấy phép và ghi công

Phát hành theo giấy phép MIT, giữ nguyên [LICENSE](LICENSE) của các tác giả Filee ("Filee contributors").
Mọi công lao của phần mềm gốc thuộc về [KnifeLemon/Filee](https://github.com/KnifeLemon/Filee) và cộng đồng đóng góp;
nút "Ủng hộ" trong ứng dụng vẫn trỏ về tác giả gốc. Thành phần bên thứ ba: [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
