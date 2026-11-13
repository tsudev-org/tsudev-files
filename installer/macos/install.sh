#!/bin/sh
set -eu

# TsuFiles: thông báo tiếng Việt mặc định; TSUFILES_LANG=en để dùng tiếng Anh (giống dòng lệnh). say "<vi>" "<en>".
say() {
    case "$(printf '%s' "${TSUFILES_LANG:-}" | tr '[:upper:]' '[:lower:]')" in
        en|en-us|english) shift ;;
    esac
    printf '%s\n' "$1"
}

source_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd -P)
install_dir="$HOME/Applications/Filee.app"
bin_dir="$HOME/.local/bin"
if [ -e "$install_dir" ] || [ -L "$install_dir" ]; then
    say "Đã có bản cài tại $install_dir. Hãy chạy uninstall.sh trước." "An installation already exists at $install_dir. Run uninstall.sh first."
    exit 1
fi
# A filee link left by an earlier Filee whose app is gone (moved to the Trash instead of uninstall.sh) is replaced.
# Anything else at that path isn't ours and stays.
stale_link=false
if [ -L "$bin_dir/filee" ] && [ ! -e "$bin_dir/filee" ]; then
    case $(readlink "$bin_dir/filee") in
        */Filee.app/Contents/MacOS/filee-cli) stale_link=true ;;
    esac
fi
if [ "$stale_link" = false ] && { [ -e "$bin_dir/filee" ] || [ -L "$bin_dir/filee" ]; }; then
    say "$bin_dir/filee đã tồn tại. Hãy xóa hoặc đổi tên nó trước khi cài TsuFiles." "$bin_dir/filee already exists. Remove or rename it before installing TsuFiles."
    exit 1
fi
test -x "$source_dir/Filee.app/Contents/MacOS/Filee"
mkdir -p "$HOME/Applications" "$bin_dir"
/usr/bin/ditto "$source_dir/Filee.app" "$install_dir"
# ditto keeps the download's quarantine flag: macOS would then start Filee from a translocated copy and could stop
# the filee command without a word (#316).
/usr/bin/xattr -dr com.apple.quarantine "$install_dir" 2>/dev/null || true
if [ "$stale_link" = true ]; then
    rm -f -- "$bin_dir/filee"
    say "Đã thay liên kết filee còn sót từ bản cài trước." "Replaced the filee link left by an earlier installation."
fi
ln -s "$install_dir/Contents/MacOS/filee-cli" "$bin_dir/filee"
say "Đã cài $install_dir" "Installed $install_dir"
say "Dòng lệnh: $bin_dir/filee (thêm $bin_dir vào PATH nếu cần)." "CLI: $bin_dir/filee (add $bin_dir to PATH if needed)."
say 'Mở TsuFiles từ Applications. Cấp quyền Accessibility và Finder Automation khi được hỏi.' 'Open TsuFiles from Applications. Grant Accessibility and Finder Automation access when requested.'
say 'Nếu TsuFiles đã có sẵn và đang bật trong Accessibility, hãy xóa nó bằng nút - rồi thêm lại bằng nút +.' 'If TsuFiles is already listed and switched on under Accessibility, remove it with the - button and add it again with +.'
