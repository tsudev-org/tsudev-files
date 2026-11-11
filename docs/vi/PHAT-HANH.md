# Quy trình phát hành tsudev-files

## 1. Khóa ký cập nhật

- Thuật toán: ECDSA P-256 / SHA-256, chữ ký dạng r||s 64 byte, base64, đặt trong `<tệp>.sig` cạnh mỗi tệp
  `*SHA256SUMS*.txt` của bản phát hành.
- **Vòng khóa** (từ 2.4.1): `FileeUpdatePublicKey` trong `Directory.Build.props` chứa 2-4 khóa công khai ngăn bởi `|`
  (`<khóa chính>|<khóa dự phòng>`), được nhúng vào ứng dụng lúc build. Chữ ký hợp lệ với bất kỳ khóa nào trong vòng
  thì đạt (`UpdateSignature.Verify`); vòng có hơn 4 khóa bị coi là bị chèn và từ chối hết.
- Khóa chính: `~/.config/tsudev-files/keys/update-key.pem` - ký mọi bản phát hành.
- Khóa dự phòng: tạo bằng `tools/khoa-ky-vn.sh tao <đường dẫn>.pem`, cất **ngoại tuyến** (USB mã hóa/két, không để
  trên máy ký hằng ngày). Chỉ dùng khi khóa chính lộ hoặc mất.
- Cả hai khóa `MUST` mã hóa bằng passphrase (`tools/khoa-ky-vn.sh ma-hoa <khóa>.pem`: PKCS#8, scrypt + AES-256,
  ghi đè khóa thô bằng `shred`). `tools/sign-checksums-vn.sh` hỏi passphrase một lần, truyền cho openssl qua fd và
  cảnh báo nếu khóa còn để thô. Passphrase cất trong trình quản lý mật khẩu, tách nơi cất tệp khóa.
- Mọi khóa: quyền 600, **ngoài mọi git working tree**; `MUST NOT` commit, gửi qua chat/email, hoặc đưa vào GitHub
  Secrets trừ khi chuyển sang ký trong CI có môi trường được bảo vệ (environment + người duyệt).
- Kiểm một khóa có trong vòng không: `tools/khoa-ky-vn.sh kiem <khóa>.pem`.

### 1.1. Thay khóa ký

1. **Khóa chính lộ** (người khác có thể ký): ký bản vá kế tiếp bằng **khóa dự phòng**; trong bản đó bỏ khóa chính cũ
   khỏi vòng và thêm một khóa dự phòng MỚI (`tools/khoa-ky-vn.sh tao`). Bản đã cài nhận bản vá nhờ khóa dự phòng,
   từ đó không còn tin khóa bị lộ. Khóa dự phòng cũ trở thành khóa chính.
2. **Khóa chính mất** (không lộ): làm như trên, không gấp.
3. **Thay định kỳ** (khuyến nghị 2 năm/lần): thêm khóa mới vào vòng ở một bản, ký bằng khóa mới từ bản sau, rồi bỏ
   khóa cũ ở bản sau nữa.
4. Mọi lần thay khóa: ghi vào `logs/STATE.md` mục "Sự cố bảo mật" (nếu lộ) và "Quyết định quan trọng".

## 2. Số phiên bản

- Trình cập nhật chỉ nhận bản có số phiên bản **lớn hơn** bản đang chạy (đọc từ tên tệp `tsudev-files-<x.y.z>-...`).
- `Version` trong `Directory.Build.props`, nhãn git `v<x.y.z>`; mục `## <x.y.z>` trong `CHANGELOG.md` thành ghi chú
  phát hành.
- Khi gộp upstream lên phiên bản mới, lấy theo số của upstream; bản sửa riêng của fork giữa hai lần gộp tăng số vá
  (ví dụ 2.4.0 -> 2.4.1).

## 3. Các bước phát hành (chưa có chứng thư Authenticode)

1. Cập nhật `Version`, `CHANGELOG.md`; chạy đủ kiểm tra cục bộ:
   `dotnet test`, `python3 -I tools/check_i18n_vi.py`.
2. Mở PR vào `main`, chờ CI + workflow "Bảo mật" xanh, merge.
3. Gắn nhãn và đẩy: `git tag v2.4.1 && git push origin v2.4.1`.
   Workflow `release.yml` build, test, đóng gói Windows/Linux/macOS và tạo **bản nháp** (draft) kèm các tệp
   `*SHA256SUMS*.txt`. Bản Windows chưa ký Authenticode (SmartScreen sẽ cảnh báo lần đầu).
4. Trên WSL, ký các tệp checksum và tải chữ ký lên bản nháp:

   ```bash
   V=2.4.1; D=$(mktemp -d)
   gh release download "v$V" -R tsudev-org/tsudev-files -p '*SHA256SUMS*.txt' -D "$D"
   tools/sign-checksums-vn.sh "$D"/*SHA256SUMS*.txt
   gh release upload "v$V" -R tsudev-org/tsudev-files "$D"/*.sig
   ```

   Script từ chối nếu khóa không khớp `FileeUpdatePublicKey`, nằm trong git working tree hoặc có quyền đọc rộng.
5. Kiểm tra bản nháp: mỗi `*SHA256SUMS*.txt` đều có `.sig`; mỗi gói có chứng thực nguồn gốc build (từ 2.4.1,
   bước "Attest build provenance" của `release.yml`):
   `gh release download "v$V" -R tsudev-org/tsudev-files -p '*linux-x64.tar.gz' -D "$D" &&
   gh attestation verify "$D"/*linux-x64.tar.gz -R tsudev-org/tsudev-files`. Rồi xuất bản:
   `gh release edit "v$V" -R tsudev-org/tsudev-files --draft=false --latest`.
6. Bản cài cũ sẽ nhận cập nhật: tải SHA256SUMS -> kiểm chữ ký ECDSA -> kiểm số phiên bản mới hơn -> tải bộ cài ->
   kiểm SHA-256 -> cài. Sai bất kỳ bước nào thì từ chối và mở trang phát hành.

## 4. Khi có chứng thư Authenticode

- Điền `FileeExpectedPublisher` / `FileeExpectedIssuer` (của **chính** chứng thư đó) trong `Directory.Build.props`.
- Đặt biến repo `TSUDEV_UNSIGNED_WINDOWS=false` và dùng `build/sign-release.ps1` của upstream (bỏ phần gương R2
  bằng `-NoMirror`).
- Lựa chọn 0 đồng cho dự án mã nguồn mở: SignPath Foundation (cần duyệt hồ sơ).
