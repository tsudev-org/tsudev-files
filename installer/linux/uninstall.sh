#!/bin/sh
set -eu

# TsuFiles: thông báo tiếng Việt mặc định; TSUFILES_LANG=en để dùng tiếng Anh (giống dòng lệnh). say "<vi>" "<en>".
say() {
    case "$(printf '%s' "${TSUFILES_LANG:-}" | tr '[:upper:]' '[:lower:]')" in
        en|en-us|english) shift ;;
    esac
    printf '%s\n' "$1"
}

data_dir=${XDG_DATA_HOME:-"$HOME/.local/share"}
install_dir="$data_dir/filee/application"
desktop_file="$data_dir/applications/com.filee.app.desktop"
gnome_extension_id="filee-donut@filee.sh"
gnome_extension_dir="$data_dir/gnome-shell/extensions/$gnome_extension_id"
python3 - "$data_dir" "$install_dir" "${TSUFILES_LANG:-}" <<'PY'
import pathlib
import sys

# TsuFiles: tiếng Việt mặc định, TSUFILES_LANG=en để dùng tiếng Anh.
EN = sys.argv[3].strip().lower() in ('en', 'en-us', 'english')

root, target = map(pathlib.Path, sys.argv[1:3])
expected = root.resolve() / 'filee' / 'application'
if target.is_symlink() or target.resolve() != expected:
    raise SystemExit('Refusing to remove an installation outside its expected directory.' if EN else 'Từ chối gỡ: bản cài nằm ngoài thư mục dự kiến.')
if (target / '.filee-package').read_text(encoding='utf-8').strip() != 'com.filee.app':
    raise SystemExit('The directory is not an installation created by TsuFiles.' if EN else 'Thư mục này không phải bản cài do TsuFiles tạo.')
PY
# TsuFiles: an installation of 2.4.0-vn.* has Filee only.
if [ -x "$install_dir/TsuFiles" ]; then "$install_dir/TsuFiles" --uninstall-cleanup; else "$install_dir/Filee" --uninstall-cleanup; fi
if [ -f "$gnome_extension_dir/.filee-package" ] && [ "$(cat "$gnome_extension_dir/.filee-package")" = 'com.filee.app' ]; then
    if command -v gnome-extensions >/dev/null 2>&1; then
        gnome-extensions disable "$gnome_extension_id" >/dev/null 2>&1 || true
    fi
    rm -rf -- "$gnome_extension_dir"
fi
if [ -L "$HOME/.local/bin/tsufiles" ] && [ "$(readlink "$HOME/.local/bin/tsufiles")" = "$install_dir/tsufiles-cli" ]; then
    rm -- "$HOME/.local/bin/tsufiles"
fi
# TsuFiles 2.4.0-vn.*: the command was "filee".
if [ -L "$HOME/.local/bin/filee" ] && [ "$(readlink "$HOME/.local/bin/filee")" = "$install_dir/filee-cli" ]; then
    rm -- "$HOME/.local/bin/filee"
fi
if [ -f "$desktop_file" ] && grep -q '^X-Filee-Managed=true$' "$desktop_file"; then
    rm -- "$desktop_file"
fi
rm -rf -- "$install_dir"
say 'Đã gỡ TsuFiles. Cài đặt và các tệp đã chuyển đổi của bạn vẫn được giữ.' 'TsuFiles was removed. Your settings and conversion results were kept.'
