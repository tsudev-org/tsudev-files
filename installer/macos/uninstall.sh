#!/bin/sh
set -eu

# TsuFiles: thông báo tiếng Việt mặc định; FILEE_LANG=en để dùng tiếng Anh (giống dòng lệnh). say "<vi>" "<en>".
say() {
    case "$(printf '%s' "${FILEE_LANG:-}" | tr '[:upper:]' '[:lower:]')" in
        en|en-us|english) shift ;;
    esac
    printf '%s\n' "$1"
}

install_dir="$HOME/Applications/Filee.app"
if [ -L "$install_dir" ] || [ ! -d "$install_dir" ]; then
    say "Không tìm thấy gói ứng dụng TsuFiles thông thường tại $install_dir." "No ordinary TsuFiles app bundle was found at $install_dir."
    exit 1
fi
resolved_parent=$(CDPATH= cd -- "$HOME/Applications" && pwd -P)
resolved_app=$(CDPATH= cd -- "$install_dir" && pwd -P)
if [ "$resolved_app" != "$resolved_parent/Filee.app" ]; then
    say 'Từ chối gỡ: ứng dụng nằm ngoài thư mục Applications dự kiến.' 'Refusing to remove an app outside the expected Applications directory.'
    exit 1
fi
bundle_id=$(/usr/libexec/PlistBuddy -c 'Print :CFBundleIdentifier' "$install_dir/Contents/Info.plist")
if [ "$bundle_id" != 'com.filee.app' ]; then
    say 'Gói này không phải TsuFiles.' 'The bundle is not TsuFiles.'
    exit 1
fi
"$install_dir/Contents/MacOS/Filee" --uninstall-cleanup
if [ -L "$HOME/.local/bin/filee" ] && [ "$(readlink "$HOME/.local/bin/filee")" = "$install_dir/Contents/MacOS/filee-cli" ]; then
    rm -- "$HOME/.local/bin/filee"
fi
rm -rf -- "$install_dir"
say 'Đã gỡ TsuFiles. Cài đặt và các tệp đã chuyển đổi của bạn vẫn được giữ.' 'TsuFiles was removed. Your settings and conversion results were kept.'
