# PHIẾU BÀN GIAO - Commit phần khởi tạo fork, Việt hóa installer

- **Mã phiếu**: 20261109-02
- **Từ**: claude - **Đến**: phiên sau / chủ dự án
- **Thời điểm**: 15:50 09/11/2026
- **Trạng thái**: HOÀN THÀNH (còn việc chờ chủ dự án ở mục 2)
- **Nhánh git**: `chore/khoi-tao-fork` (chưa push)

## 1. Việc đã làm xong

- Phiên trước tắt đột ngột khi toàn bộ phần khởi tạo fork còn staged trên `main`, chưa commit. Đã kiểm chứng rồi
  commit thành nhánh `chore/khoi-tao-fork`:
  - `e27c5998` chore(standards), `e3a77ad1` ci(security), `c20a29c7` feat(update), `edef234d` feat(i18n),
    `a0a31c20` docs (gồm `tools/fetch-engines-linux.sh` còn untracked của phiên trước).
- `52ca56f0` Việt hóa `installer/linux/{install,uninstall}.sh` (hàm `say "<vi>" "<en>"`, `FILEE_LANG=en` ra tiếng
  Anh) và tiêu đề dự phòng của `src/Filee.ExplorerMenu` ("Chuyển đổi bằng Filee"); test `ForkLinuxInstallerTests`.
- `44a0cb08` Việt hóa `installer/macos/*.sh`; thêm `installer/{linux,macos}/HUONG-DAN-CAI-DAT.txt`, đóng gói kèm trong
  `build/build-portable.ps1`.
- Đề xuất miễn trừ em-dash: phiếu `20261109-01`.

## 2. Việc dang dở + bước tiếp theo CỤ THỂ

- [ ] Chủ dự án: `git push -u origin chore/khoi-tao-fork` rồi mở PR vào `main` (squash hoặc rebase; agent chưa được
      phép push). CI Windows sẽ chạy `Filee.Setup.Tests` và `ExplorerMenuTests` (dựng DLL) mà máy Linux không chạy được.
- [ ] Các task còn lại trong `logs/STATE.md` đều cần chủ dự án: phát hành trên Windows, quyết định đổi tên, chứng
      thư Authenticode, mở đề xuất ở `tsudev-standards`.

## 3. File liên quan / đang khóa

| Đường dẫn | Lý do | Còn khóa? |
| --- | --- | --- |
| (không) | | Không |

## 4. Yêu cầu gửi agent đang giữ khóa (nếu có)

- Không có.

## 5. Cảnh báo và quyết định quan trọng

- Kết quả test trên WSL (Linux) 09/11/2026:
  - Đạt: Core 1182, Cli 99, Platform 122 (16 bỏ qua), App (lọc `Fork|ExplorerMenu`) 9 (6 bỏ qua). Chưa chạy trọn
    bộ App (30-60 phút).
  - `Filee.Setup.Tests`: 65 lỗi, **đúng bằng** khi chạy upstream gốc `64890f1e` trên Linux - bộ này chỉ chạy trên
    Windows trong CI. Test riêng của fork (`Every_language_says_the_download_is_incomplete`) đạt.
  - `Filee.Engines.Tests`: 2717 đạt, 2 lỗi (`AnnotationTests...pen_line`, `CadTests.Dxf_to_svg...`) do máy thiếu
    phông Noto/CJK (`build/fetch-fonts.ps1` cần `pwsh`, chưa cài). Fork không sửa gì trong `src/Filee.Engines`.
- `logs/LOCKS.md` từng có quyền 444 vì được chép từ mẫu chỉ-đọc trong `.standards/templates/`; đã đổi về 644 để ghi
  khóa được (git không lưu khác biệt này).
- Test upstream `LinuxInstallerTests` so khớp thông báo tiếng Anh nên đặt `FILEE_LANG=en` trong `RunInstaller`
  (có chú thích `tsudev-files:`). Khi gộp upstream mà thêm thông báo mới vào script cài, nhớ bọc bằng `say`;
  `ForkLinuxInstallerTests.Every_message_of_the_scripts_has_both_languages` sẽ đỏ nếu quên.

## 6. Trạng thái cổng kiểm

- [ ] `./scripts/check-standards.sh` đạt - CHƯA: chỉ còn em-dash kế thừa upstream (phiếu `20261109-01`)
- [x] Lint và test đạt (trừ các lỗi do môi trường ở mục 5, đã đối chứng với upstream)
- [x] `git status` sạch ngoài phạm vi bàn giao

## 7. Kết quả xử lý (agent nhận điền sau khi thực hiện)

-
