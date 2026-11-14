# Ảnh chụp cho README

`tsufiles-<trang>-<ngôn ngữ>.png`: cửa sổ chính ở kiểu giao diện Cú đêm tối (mặc định của bản cài mới), sinh từ test
giao diện headless - không chụp tay, không sửa ảnh. Đổi giao diện thì sinh lại:

```bash
dotnet test tests/TsuFiles.App.Tests --filter "FullyQualifiedName~NightOwlThemeTests.Main_window_renders_in_night_owl_dark_for_the_readme"
for f in home toolbar; do for l in en vi; do
  cp tests/TsuFiles.App.Tests/bin/Debug/net10.0/screenshots/readme-$f-$l.png assets/screenshots/tsufiles-$f-$l.png
done; done
```

Ảnh của website (`website/public/anh/page-*-vi.png`) sinh theo `website/README.md`.
