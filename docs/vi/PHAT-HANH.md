# Quy trình phát hành tsudev-files

## 1. Khóa ký cập nhật

- Thuật toán: ECDSA P-256 / SHA-256, chữ ký dạng r||s 64 byte, base64, đặt trong `<tệp>.sig` cạnh mỗi tệp
  `*SHA256SUMS*.txt` của bản phát hành.
- Khóa công khai: `FileeUpdatePublicKey` trong `Directory.Build.props` (được nhúng vào ứng dụng lúc build).
- Khóa bí mật: `~/.config/tsudev-files/keys/update-key.pem` (quyền 600, **ngoài mọi git working tree**).
  - `MUST` sao lưu khóa này ra nơi chỉ chủ dự án truy cập (ví dụ: trình quản lý mật khẩu, USB mã hóa). Mất khóa thì mọi
    bản đã cài không tự cập nhật được nữa; người dùng phải tải tay bản mới mang khóa mới.
  - `MUST NOT` commit, gửi qua chat/email, hoặc đưa vào GitHub Secrets trừ khi chuyển sang ký trong CI có môi trường
    được bảo vệ (environment + người duyệt).
  - Nên mã hóa khóa bằng passphrase: `openssl pkey -in update-key.pem -aes256 -out update-key.enc.pem`
    (script ký sẽ hỏi passphrase).
- Khóa bị lộ: tạo khóa mới, cập nhật `FileeUpdatePublicKey`, phát hành bản mới ký bằng **khóa cũ** lần cuối (để bản
  đã cài nhận được khóa mới), rồi hủy khóa cũ. Ghi sự cố vào `logs/STATE.md` mục "Sự cố bảo mật".

## 2. Số phiên bản

- Trình cập nhật chỉ nhận bản có số phiên bản **lớn hơn** bản đang chạy (đọc từ tên tệp `Filee-<x.y.z>-...`).
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
   gh release download "v$V" -R tsudev-tsudev/tsudev-files -p '*SHA256SUMS*.txt' -D "$D"
   tools/sign-checksums-vn.sh "$D"/*SHA256SUMS*.txt
   gh release upload "v$V" -R tsudev-tsudev/tsudev-files "$D"/*.sig
   ```

   Script từ chối nếu khóa không khớp `FileeUpdatePublicKey`, nằm trong git working tree hoặc có quyền đọc rộng.
5. Kiểm tra bản nháp: mỗi `*SHA256SUMS*.txt` đều có `.sig`, rồi xuất bản:
   `gh release edit "v$V" -R tsudev-tsudev/tsudev-files --draft=false`.
6. Bản cài cũ sẽ nhận cập nhật: tải SHA256SUMS -> kiểm chữ ký ECDSA -> kiểm số phiên bản mới hơn -> tải bộ cài ->
   kiểm SHA-256 -> cài. Sai bất kỳ bước nào thì từ chối và mở trang phát hành.

## 4. Khi có chứng thư Authenticode

- Điền `FileeExpectedPublisher` / `FileeExpectedIssuer` (của **chính** chứng thư đó) trong `Directory.Build.props`.
- Đặt biến repo `TSUDEV_UNSIGNED_WINDOWS=false` và dùng `build/sign-release.ps1` của upstream (bỏ phần gương R2
  bằng `-NoMirror`).
- Lựa chọn 0 đồng cho dự án mã nguồn mở: SignPath Foundation (cần duyệt hồ sơ).
