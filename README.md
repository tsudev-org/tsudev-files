<p align="center"><img src="assets/brand/tsufiles-mark-512.png" alt="" width="128" height="128"></p>

# TsuFiles

**Trình chuyển đổi tệp ngoại tuyến, miễn phí, mã nguồn mở - bản Việt hóa và gia cố bảo mật của [Filee](https://github.com/KnifeLemon/Filee).**

Một sản phẩm của [tsudev](https://tsudev.com).

**Hạng sản phẩm: C - Ngoại tuyến** (theo `.standards/docs/AUTH_AND_ACCOUNT.md` mục 1): mọi chức năng chuyển đổi chạy
trọn vẹn không cần internet và không cần tài khoản; kiểm tra cập nhật là tùy chọn.

Kéo tệp, thả vào một định dạng trên vòng "donut" mở ra ngay tại con trỏ chuột là xong. Hơn 260 định dạng: ảnh, PDF,
Word, Excel, PowerPoint, HWP/HWPX, video, âm thanh, sách điện tử, tệp nén, CAD, bản đồ, 3D... Mọi xử lý chạy **trên
máy của bạn**, không tải tệp lên bất kỳ máy chủ nào, không cần cài Microsoft Office.

- Tải bản phát hành: <https://github.com/tsudev-org/tsufiles/releases/latest>
- Báo lỗi / đề xuất: <https://github.com/tsudev-org/tsufiles/issues>
- Báo lỗ hổng bảo mật: xem [SECURITY.md](SECURITY.md) (KHÔNG mở issue công khai)
- Ủng hộ dự án: <https://files.tsudev.com/ung-ho/>
- English: [README.en.md](README.en.md)

## Khác biệt so với bản gốc

| Hạng mục | Bản gốc Filee | TsuFiles |
| --- | --- | --- |
| Ngôn ngữ mặc định | Theo ngôn ngữ hệ điều hành, dự phòng tiếng Anh | **Tiếng Việt** cho giao diện, trình cài đặt và dòng lệnh; đổi được trong Cài đặt (`auto`, `en`, ...) |
| Dòng lệnh `tsufiles` | Chỉ tiếng Anh | Tiếng Việt mặc định, `TSUFILES_LANG=en` để dùng tiếng Anh; khóa JSON giữ nguyên cho script |
| Kiểm tra cập nhật | `KnifeLemon/Filee` + khóa ký của tác giả gốc | `tsudev-org/tsufiles` + **khóa ký ECDSA P-256 riêng**; bản phát hành của repo khác bị từ chối |
| Tên sản phẩm | Filee | **TsuFiles** - logo con cú, kiểu giao diện Cú đêm ([ADR-0003](docs/adr/0003-ten-san-pham-tsufiles.md)); tệp chạy, lệnh `tsufiles`, thư mục dữ liệu, lối tắt và tệp phát hành đổi sang `tsufiles` ở bản 2.4.1 kèm di trú |
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
dotnet build TsuFiles.slnx
dotnet test tests/TsuFiles.Core.Tests    # các bộ test khác: tests/TsuFiles.*.Tests
# Bộ giao diện chạy riêng, có phát hiện treo (giống CI; khoảng 30-60 phút trên WSL):
dotnet test tests/TsuFiles.App.Tests --no-build --blame-crash --blame-hang-timeout 5m
python3 -I tools/check_i18n_vi.py         # kiểm tra bản dịch tiếng Việt
bash scripts/check-standards.sh           # cổng kiểm quy ước tsudev
```

Chạy dòng lệnh từ mã nguồn:

```bash
dotnet run --project src/TsuFiles.Cli -- help
TSUFILES_LANG=en dotnet run --project src/TsuFiles.Cli -- help
```

Quy ước làm việc (cho người và agent AI): đọc [AGENTS.md](AGENTS.md) trước tiên.

## Bản vá từ upstream

TsuFiles đã tách hẳn khỏi Filee ([ADR-0002](docs/adr/0002-hard-fork-doi-ten-toan-bo-tsudev.md)): không gộp
`upstream/main` nữa. Bản vá bảo mật của upstream được chép thủ công:

```bash
git format-patch -1 <commit> -o /tmp/va                       # từ remote upstream
python3 -I tools/doi-ten-tsudev.py --tep /tmp/va/*.patch       # đổi tên Filee.* -> TsuFiles.*
git apply /tmp/va/*.patch                                      # ghi nguồn (commit upstream) trong thông điệp commit
```

Remote `upstream` đã bị khóa chiều đẩy (`DISABLED`) để không thể vô tình đẩy lên repo gốc.

## Ủng hộ

TsuFiles miễn phí và sẽ luôn miễn phí. Mọi kênh ủng hộ chính thức chỉ được công bố tại
**<https://files.tsudev.com/ung-ho/>** (dữ liệu: [`website/src/data/ung-ho.json`](website/src/data/ung-ho.json)); nút
"Ủng hộ" trong ứng dụng và nút "Sponsor" của GitHub đều trỏ về trang đó. Nhà tài trợ được liệt kê tại đây, trên trang
Ủng hộ và trong trang "Giới thiệu" của ứng dụng.

<!-- sponsors: src/TsuFiles.App/Assets/Sponsors/sponsors.json -->

## Giấy phép và ghi công

Phát hành theo giấy phép MIT, giữ nguyên [LICENSE](LICENSE) của các tác giả Filee ("Filee contributors").
Mọi công lao của phần mềm gốc thuộc về [KnifeLemon/Filee](https://github.com/KnifeLemon/Filee) và cộng đồng đóng góp.
Thành phần bên thứ ba: [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
