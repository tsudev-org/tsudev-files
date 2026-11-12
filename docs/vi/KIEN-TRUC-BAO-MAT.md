# Kiến trúc bảo mật tsudev-files

> Phạm vi: ứng dụng desktop chuyển đổi tệp ngoại tuyến (Windows/Linux/macOS), dòng lệnh `filee`, trình cài đặt và
> trình cập nhật. Không có máy chủ, không có tài khoản người dùng.

## 1. Phân loại dữ liệu (SECURITY_BASELINE mục 2)

| Dữ liệu | Mức | Ghi chú |
| --- | --- | --- |
| Tệp người dùng đưa vào chuyển đổi | **D2-D3** (có thể chứa CCCD, hợp đồng, tài chính) | Chỉ xử lý cục bộ, không gửi đi đâu |
| Cài đặt, preset, lịch sử chuyển đổi | D1 | Thư mục dữ liệu người dùng |
| Khóa bí mật ký cập nhật | **D3** | Chỉ trên máy chủ dự án, ngoài repo (`docs/vi/PHAT-HANH.md`) |
| Mã nguồn, bản dịch, tài liệu | D0 | Công khai (MIT) |

## 2. Mô hình mối đe dọa

| Mối đe dọa | Vector | Lớp phòng thủ |
| --- | --- | --- |
| Tệp độc hại chiếm quyền máy | PDF/Office/HWP/ảnh/email/tệp nén được dựng sẵn | Bộ đọc thuần .NET (managed) cho phần lớn định dạng; XML tắt DTD hoặc không có resolver và giới hạn thực thể (13 vị trí); engine ngoài chạy tiến trình riêng |
| Ghi tệp ra ngoài thư mục đích | "zip slip", liên kết tượng trưng trong tệp nén | `Archives/ArchiveSafety.cs`: chặn đường dẫn thoát thư mục, symlink, nội dung mã hóa, "entry bomb", kiểm dung lượng trống |
| Rò rỉ dữ liệu/SSRF khi chuyển đổi | Tài liệu nhúng liên kết tới URL/tệp ngoài | `Office/LinkedContent.cs`, `Web/LocalNetwork.cs`; MsgReader 6.2 không đọc tệp đính kèm theo tham chiếu |
| Từ chối dịch vụ | Tệp "bom" giải nén, ảnh khổng lồ | Giới hạn số mục, dung lượng; tiến trình có thể hủy |
| Cập nhật giả mạo | Chiếm repo/CDN, MITM | HTTPS + SHA-256 + **chữ ký ECDSA P-256 riêng của fork** trên SHA256SUMS + chống hạ cấp (chỉ nhận số phiên bản lớn hơn) |
| Engine tải về bị tráo | Chiếm nguồn tải FFmpeg/LibreOffice/... | SHA-256 ghim sẵn trong mã (`EngineDownloads`/`EngineInstaller`), sai thì hủy |
| Chuỗi cung ứng CI | Tag action bị sửa, workflow bị lạm dụng | Action ghim SHA commit, `permissions` tối thiểu, `persist-credentials: false` ở workflow mới, `pull_request_target` không checkout mã PR |
| Lộ bí mật trong repo | Commit nhầm khóa/token | gitleaks trong CI, `.gitignore` chuẩn tsudev, cổng kiểm `scripts/check-standards.sh` |
| Phụ thuộc có lỗ hổng | Gói NuGet | Quản lý phiên bản tập trung (`Directory.Packages.props`), Dependabot hằng tuần, CI chặn gói High/Critical |
| Đầu vào dòng lệnh | Biến môi trường `FILEE_LANG` | Danh sách trắng `vi`/`en`, giá trị lạ quay về mặc định |

## 3. Cổng kiểm tự động

| Workflow | Nội dung | Chặn merge |
| --- | --- | --- |
| `ci.yml` (upstream) | Build + toàn bộ test trên Windows/Linux/ARM | Có |
| `security.yml` | CodeQL `security-extended` (C#), gitleaks, `dotnet list package --vulnerable`, `tools/check_i18n_vi.py` | Có (khi bật branch protection) |
| Dependabot | NuGet + GitHub Actions, thứ Hai hằng tuần | - |

## 4. Nguyên tắc khi sửa mã

1. Mọi khác biệt của fork gom vào `src/Tsudev.Files.Core/ForkDefaults.cs` (giá trị lúc chạy) và `Directory.Build.props`
   (giá trị lúc build). Không rải URL/khóa khắp mã.
2. Không thêm bất kỳ lệnh gọi mạng nào trong luồng chuyển đổi. Tính năng cần mạng (cập nhật, tải engine, Ollama cục
   bộ) phải có kiểm tra toàn vẹn và mặc định an toàn.
3. Bộ đọc XML mới: `DtdProcessing.Prohibit` (hoặc `Parse` + `XmlResolver = null` + `MaxCharactersFromEntities`).
4. Giải nén mới: bắt buộc đi qua `ArchiveSafety`.
5. Chuỗi giao diện mới: thêm khóa vào **cả** `en.json` và `vi.json`; `tools/check_i18n_vi.py` phải xanh.

## 5. Kết quả rà soát ban đầu (09/11/2026, upstream `64890f1e`)

- Không có tệp nhị phân thực thi trong repo; không có mã bị làm rối, base64 lớn, `Invoke-Expression`, hay tải-rồi-chạy.
- Không có telemetry/analytics. Kết nối mạng duy nhất: GitHub API (cập nhật), nguồn tải engine (có SHA-256),
  Ollama tại `127.0.0.1:11434` (tùy chọn, cục bộ).
- Duy nhất một chỗ `curl | sh` trong CI (`portable.yml`, cài rustup chính thức qua HTTPS/TLS1.2) - chấp nhận được, chỉ
  chạy trên runner GitHub.
- Kết luận: **an toàn để fork và phát triển**.

## 6. Lộ trình gia cố tiếp theo

- [ ] Chứng thư Authenticode (SignPath OSS) để hết cảnh báo SmartScreen và bật ghim nhà phát hành.
- [x] SBOM (CycloneDX 1.7) + GitHub artifact attestation (nguồn gốc build + SBOM) cho mỗi bản phát hành (11/11/2026).
- [ ] Bật branch protection cho `main`: yêu cầu PR + các check `ci` và `Bảo mật`.
- [x] Fuzz đột biến cho bộ đọc tự viết (MOBI/PalmDOC/HUFF-CDIC, ICNS, OFD) + kiểm thuộc tính ArchiveSafety: 200 vòng mỗi CI, 20.000 vòng hằng tuần (`fuzz.yml`). Lượt đầu 5.000 vòng/mục tiêu: không lỗi (11/11/2026). Còn HWP/HWPX.
- [x] Đổi tên hiển thị "Filee" -> "tsudev-files" (09/11/2026, [ADR-0001](../adr/0001-doi-ten-hien-thi-tsudev-files.md)).
- [ ] Cài song song với Filee gốc: hiện dùng chung `AppGuid`, khóa registry và thư mục dữ liệu (ADR-0001 mục 4).
