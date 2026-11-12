# Bắt buộc cập nhật

Chuẩn chung của hệ sinh thái tsudev (phiếu `~/projects/logs/handover/20261112-04`, mẫu `tsudev-org/lotus-share#39`):
mở bản quá cũ thì hiện hộp thoại yêu cầu cập nhật, không dùng tiếp được. Có từ bản 2.4.1 (số build 2); bản 2.4.0-vn.1
đã phát hành trước nên không tự hiện hộp thoại - chỉ hướng dẫn được trên website.

## 1. Tệp phiên bản

`https://files.tsudev.com/phien-ban.json` (nguồn: `website/public/phien-ban.json`, cache 5 phút qua `_headers`):

```json
{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}
```

| Trường | Ý nghĩa |
| --- | --- |
| `phienBan` | Phiên bản mới nhất `x.y.z`, chỉ để hiển thị trong hộp thoại |
| `moiNhat` | Số build của bản mới nhất |
| `toiThieu` | Số build thấp nhất còn dùng được; bản thấp hơn bị chặn |

Build website kiểm `1 <= toiThieu <= moiNhat <= ForkDefaults.BuildNumber` (`website/scripts/check-dist.mjs`).

## 2. Hành vi của ứng dụng

Mã: `src/TsuFiles.Core/Updates/VersionGate.cs` (đọc, quyết định, tải) và `src/TsuFiles.App/Services/ForcedUpdateService.cs`
(hộp thoại). Test: `VersionGateTests`, `ForcedUpdateTests`.

- Khi mở: nếu `toiThieu` đã lưu (`settings.json` -> `updateMinimumBuild`, chỉ tăng) lớn hơn số build đang chạy thì chặn
  ngay, kể cả khi mất mạng hoặc người dùng tắt kiểm tra cập nhật.
- Có bật "Kiểm tra cập nhật": tải tệp với URL cố định trong mã, không theo chuyển hướng, tối đa 16 KB, quá 8 giây thì
  bỏ. Mã khác 200, quá cỡ, sai khuôn: coi như không có tệp, ứng dụng chạy bình thường.
- Bị chặn: dừng trình kích hoạt (cử chỉ kéo, phím tắt) và thư mục theo dõi; hộp thoại không đóng được bằng X/Alt+F4,
  chỉ có "Tải bản mới" (mở `files.tsudev.com/tai-ve/`) và "Thoát".
- Cũ hơn `moiNhat` nhưng chưa dưới `toiThieu`: không thêm hộp thoại nào - thẻ cập nhật sẵn có (bản phát hành trên
  GitHub, nhắc một lần mỗi phiên bản, có "Để sau") đã nhắc.

## 3. Quy trình

1. Mỗi bản phát hành tăng `ForkDefaults.BuildNumber` thêm 1 ([PHAT-HANH.md](PHAT-HANH.md) mục 3).
2. **Sau khi** bản phát hành đã công khai trên GitHub: sửa `phienBan`, `moiNhat` trong `website/public/phien-ban.json`,
   PR, gộp, chủ dự án deploy website. Deploy trước khi có bản phát hành sẽ chặn người dùng mà không có gì để tải.
3. Nâng `toiThieu` (chặn bản cũ: lỗ hổng bảo mật, định dạng dữ liệu không còn tương thích) **cần chủ dự án duyệt**, ghi
   lý do trong PR và `CHANGELOG.md`. Không bao giờ hạ `toiThieu` để "mở khóa": ứng dụng đã lưu mức cao nhất từng thấy.
