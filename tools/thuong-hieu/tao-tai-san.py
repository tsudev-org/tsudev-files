#!/usr/bin/env python3
"""TsuFiles: sinh MỌI biểu tượng/logo/favicon của repo từ một ảnh gốc duy nhất.

Ảnh gốc: assets/brand/source/tsufiles-1.webp (ảnh bìa TsuFiles; con cú cầm thư mục nằm trong OWL_BOX). Không sửa tay
tệp sinh ra; đổi logo thì thay ảnh gốc, cập nhật SOURCE_SHA256 (và OWL_BOX) rồi chạy lại script này và commit kết quả.
Hướng dẫn: assets/brand/README.md.

  python3 -m venv /tmp/brand && /tmp/brand/bin/pip install -r tools/thuong-hieu/requirements.txt
  /tmp/brand/bin/python -I tools/thuong-hieu/tao-tai-san.py          sinh lại toàn bộ
  /tmp/brand/bin/python -I tools/thuong-hieu/tao-tai-san.py --kiem   chỉ kiểm (thoát 1 nếu thiếu/sai cỡ)

Các bước:
1. Cắt OWL_BOX, ước lượng nền chuyển sắc bằng mặt phẳng khớp với mép khung, tách nền bằng color-to-alpha theo khoảng
   cách tới nền tại chỗ. Chỉ vùng NỐI với mép mới là nền, nên lòng trắng mắt (gần màu nền) vẫn đục. Bỏ mảnh lạc
   (chữ "T" cạnh con cú), giữ khối lớn nhất cùng quầng sáng.
2. Cắt sát, đặt vào khung vuông trong suốt (bản chính). Thu nhỏ bằng LANCZOS trên alpha nhân sẵn (không viền đen).
3. Cỡ <= 48 px: cắt sát hơn (bỏ quầng mờ) và làm nét nhẹ để hình còn đọc được ở 16 px.
4. Con cú trong ảnh bìa chỉ cao khoảng 160 px: cỡ lớn hơn bản chính được phóng LANCZOS + làm nét. Có ảnh con cú riêng
   độ phân giải cao (>= 1024 px, hoặc SVG) thì thay ảnh gốc để icon 256/512 nét hẳn.
"""
from __future__ import annotations

import hashlib
import io
import struct
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "assets/brand/source/tsufiles-1.webp"   # ảnh bìa TsuFiles (1024x512), con cú ở góc trái
SOURCE_SHA256 = "33974eb00c9b72fb7899027bb155662697700f51fc95b743adf9eb32f77bccee"
OWL_BOX = (20, 20, 212, 240)                            # khung chứa con cú trong ảnh bìa (trái, trên, phải, dưới)

BRAND = ROOT / "assets/brand"
APP_ICONS = ROOT / "src/TsuFiles.App/Assets/Icons"
WEB = ROOT / "website/public"
WEB_FONTS = WEB / "fonts"

ICO_SIZES = [16, 20, 24, 32, 40, 48, 64, 96, 128, 256]  # gồm đủ 16/24/32/48/64/128/256 (BRAND_ASSETS mục 6, 7)
SMALL = 48
WEB_DARK = (10, 10, 10)  # --bg-base của chế độ Tối (website/src/styles/palette.css), cũng là theme_color của manifest

# Tệp sinh ra -> (rộng, cao). --kiem đối chiếu đúng bảng này.
OUTPUTS: dict[Path, tuple[int, int] | None] = {
    BRAND / "tsufiles-mark.png": None,
    BRAND / "tsufiles-mark.webp": None,
    BRAND / "tsufiles-mark.avif": None,
    BRAND / "tsufiles-mark-512.png": (512, 512),
    APP_ICONS / "filee.png": (512, 512),
    APP_ICONS / "filee.ico": (256, 256),
    WEB / "favicon.ico": (256, 256),
    WEB / "favicon-16x16.png": (16, 16),
    WEB / "favicon-32x32.png": (32, 32),
    WEB / "favicon-96x96.png": (96, 96),
    WEB / "apple-touch-icon.png": (180, 180),
    WEB / "android-chrome-192x192.png": (192, 192),
    WEB / "android-chrome-512x512.png": (512, 512),
    WEB / "android-chrome-maskable-192x192.png": (192, 192),
    WEB / "android-chrome-maskable-512x512.png": (512, 512),
    WEB / "brand/tsufiles-mark.png": None,
    WEB / "brand/tsufiles-mark.webp": None,
    WEB / "brand/tsufiles-mark.avif": None,
    WEB / "og-image.png": (1200, 630),
}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def png_size(data: bytes) -> tuple[int, int]:
    return struct.unpack(">II", data[16:24])


def ico_sizes(data: bytes) -> list[int]:
    _, kind, count = struct.unpack("<HHH", data[:6])
    if kind != 1:
        raise ValueError("không phải ICO")
    return sorted((data[6 + 16 * i] or 256) for i in range(count))


def check() -> int:
    errors: list[str] = []
    if not SOURCE.is_file() or sha256(SOURCE) != SOURCE_SHA256:
        errors.append(f"ảnh gốc thiếu hoặc khác SOURCE_SHA256: {SOURCE.relative_to(ROOT)}")
    for path, size in OUTPUTS.items():
        rel = path.relative_to(ROOT)
        if not path.is_file():
            errors.append(f"thiếu {rel}")
            continue
        data = path.read_bytes()
        if path.suffix == ".ico":
            if ico_sizes(data) != ICO_SIZES:
                errors.append(f"{rel}: lớp {ico_sizes(data)}, cần {ICO_SIZES}")
        elif path.suffix == ".png" and size and png_size(data) != size:
            errors.append(f"{rel}: cỡ {png_size(data)}, cần {size}")
    for e in errors:
        print("LỖI", e)
    print("ĐẠT" if not errors else f"{len(errors)} lỗi")
    return 1 if errors else 0


# ---------------------------------------------------------------------------------------------------------------- sinh

def build() -> None:
    import numpy as np
    from PIL import Image, ImageDraw, ImageFilter
    from scipy import ndimage

    if sha256(SOURCE) != SOURCE_SHA256:
        sys.exit("Ảnh gốc khác SOURCE_SHA256: đổi logo thì cập nhật chuỗi băm trong script này (có chủ đích).")

    # 1. Cắt con cú khỏi ảnh gốc, tách nền tím nhạt (chuyển sắc) của ảnh
    src = Image.open(SOURCE).convert("RGB").crop(OWL_BOX)
    c = np.asarray(src).astype(np.float64)
    h, w, _ = c.shape
    ys, xs = np.mgrid[0:h, 0:w]
    edge = np.zeros((h, w), bool)
    edge[:4], edge[-4:], edge[:, :4], edge[:, -4:] = True, True, True, True
    plane = np.c_[xs[edge], ys[edge], np.ones(edge.sum())]
    grid = np.c_[xs.ravel(), ys.ravel(), np.ones(h * w)]
    bg = np.stack([(grid @ np.linalg.lstsq(plane, c[..., k][edge], rcond=None)[0]).reshape(h, w) for k in range(3)], -1)
    dist = np.sqrt(((c - bg) ** 2).sum(axis=2))        # khoảng cách tới nền tại chỗ
    lo, hi = 8, 40
    near, _ = ndimage.label(dist < hi)                  # nền = vùng gần màu nền NỐI với mép ảnh
    outer = np.unique(np.r_[near[0], near[-1], near[:, 0], near[:, -1]])
    background = np.isin(near, outer[outer > 0])        # (lòng trắng mắt cú gần màu nền nhưng không nối mép -> giữ)
    alpha = np.where(background, np.clip((dist - lo) / (hi - lo), 0, 1), 1.0)
    rgb = np.clip((c - bg * (1 - alpha[..., None])) / np.maximum(alpha, 1e-6)[..., None], 0, 255)
    solid = alpha > 0.3                                 # bỏ mảnh lạc (chữ bên cạnh): giữ khối lớn nhất + quầng 6 px
    parts, n = ndimage.label(solid)
    main = 1 + int(np.argmax(ndimage.sum(solid, parts, range(1, n + 1))))
    alpha = np.where(ndimage.binary_dilation(parts == main, iterations=6), alpha, 0)
    rgba = np.dstack([rgb, alpha * 255]).round().astype(np.uint8)
    rgba[alpha * 255 < 1.5] = 0
    cut = Image.fromarray(rgba, "RGBA")

    def bbox(img: Image.Image, threshold: int) -> tuple[int, int, int, int]:
        return img.getchannel("A").point(lambda v: 255 if v > threshold else 0).getbbox()

    def square(img: Image.Image, margin: float) -> Image.Image:
        side = max(img.size)
        pad = round(side * margin)
        out = Image.new("RGBA", (side + 2 * pad,) * 2, (0, 0, 0, 0))
        out.alpha_composite(img, ((out.width - img.width) // 2, (out.height - img.height) // 2))
        return out

    def resize(img: Image.Image, size: tuple[int, int]) -> Image.Image:
        return img.convert("RGBa").resize(size, Image.LANCZOS).convert("RGBA")

    tight = cut.crop(bbox(cut, 2))                      # hình + quầng, sát mép
    master = square(tight, 0.03)                        # bản chính vuông
    core = square(cut.crop(bbox(cut, 96)), 0.0)         # chỉ phần đặc, cho cỡ nhỏ

    def icon(size: int) -> Image.Image:
        if size <= SMALL:
            img = resize(core, (size, size))
            return img.filter(ImageFilter.UnsharpMask(radius=0.6, percent=60, threshold=0))
        img = resize(master, (size, size))
        if size > master.width:                         # phóng to (xem bước 4)
            img = img.filter(ImageFilter.UnsharpMask(radius=1.0 * size / master.width, percent=45, threshold=1))
        return img

    def on(bg: tuple[int, int, int], size: int, scale: float) -> Image.Image:
        out = Image.new("RGBA", (size, size), bg + (255,))
        inner = round(size * scale)
        out.alpha_composite(icon(inner), ((size - inner) // 2, (size - inner) // 2))
        return out.convert("RGB")

    def save(img: Image.Image, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        if path.suffix == ".png":
            img.save(path, optimize=True)
        elif path.suffix == ".webp":
            img.save(path, quality=90, alpha_quality=95, method=6)
        elif path.suffix == ".avif":
            img.save(path, quality=80, speed=2)
        print(f"  {path.relative_to(ROOT)} {img.size[0]}x{img.size[1]} {path.stat().st_size // 1024} KiB")

    def save_ico(path: Path) -> None:
        frames = [icon(s) for s in ICO_SIZES]
        path.parent.mkdir(parents=True, exist_ok=True)
        frames[-1].save(path, format="ICO", sizes=[(s, s) for s in ICO_SIZES], append_images=frames[:-1])
        print(f"  {path.relative_to(ROOT)} lớp {ico_sizes(path.read_bytes())}")

    print("Bản chính:", master.size)
    save(master, BRAND / "tsufiles-mark.png")
    save(master, BRAND / "tsufiles-mark.webp")
    save(master, BRAND / "tsufiles-mark.avif")
    save(icon(512), BRAND / "tsufiles-mark-512.png")

    # Ứng dụng desktop, Setup, CLI, gói Linux/macOS, gói menu Explorer (đều đọc hai tệp này)
    save(icon(512), APP_ICONS / "filee.png")
    save_ico(APP_ICONS / "filee.ico")

    # Website files.tsudev.com
    save_ico(WEB / "favicon.ico")
    for s in (16, 32, 96):
        save(icon(s), WEB / f"favicon-{s}x{s}.png")
    save(on(WEB_DARK, 180, 0.82), WEB / "apple-touch-icon.png")
    for s in (192, 512):
        save(icon(s), WEB / f"android-chrome-{s}x{s}.png")
        save(on(WEB_DARK, s, 0.62), WEB / f"android-chrome-maskable-{s}x{s}.png")  # trong vùng an toàn 80 %
    header = resize(tight, (round(tight.width * 168 / tight.height), 168))         # đầu trang: cao 42 px, 4x
    for ext in ("png", "webp", "avif"):
        save(header, WEB / f"brand/tsufiles-mark.{ext}")
    save(og_image(tight, resize, Image, ImageDraw), WEB / "og-image.png")


def og_image(tight, resize, Image, ImageDraw):
    """Ảnh chia sẻ 1200x630, nền và chữ theo token chế độ Tối của website."""
    from fontTools.ttLib import TTFont
    from PIL import ImageFont

    W, H = 1200, 630
    # website/src/styles/palette.css, [data-theme="dark"]
    BG, SURFACE, BORDER = (0x0A, 0x0A, 0x0A), (0x11, 0x11, 0x11), (0x2E, 0x2E, 0x2E)
    TEXT, TEXT2, PRIMARY, ON_PRIMARY = (0xED, 0xED, 0xED), (0xB4, 0xB4, 0xB4), (0xA9, 0x70, 0xFF), (0x0A, 0x0A, 0x0A)

    class Font:
        """Be Vietnam Pro tự host chia theo bộ ký tự: chọn tệp có glyph cho từng ký tự."""

        def __init__(self, weight: int, size: int):
            self.parts = []
            for subset in ("latin", "vietnamese", "latin-ext"):
                path = WEB_FONTS / f"be-vietnam-pro-{weight}-{subset}.woff2"
                cmap = TTFont(path).getBestCmap()
                self.parts.append((set(cmap), ImageFont.truetype(str(path), size)))

        def pick(self, ch: str):
            return next((f for cmap, f in self.parts if ord(ch) in cmap), self.parts[0][1])

        def width(self, text: str) -> float:
            return sum(self.pick(ch).getlength(ch) for ch in text)

        def draw(self, d, xy, text: str, fill) -> None:
            x, y = xy
            for ch in text:
                f = self.pick(ch)
                d.text((x, y), ch, font=f, fill=fill, anchor="ls")
                x += f.getlength(ch)

    og = Image.new("RGB", (W, H), BG)
    d = ImageDraw.Draw(og)
    d.rounded_rectangle((48, 48, W - 48, H - 48), radius=32, fill=SURFACE, outline=BORDER, width=2)
    mh = 340
    mark = resize(tight, (round(tight.width * mh / tight.height), mh))
    og.paste(mark, (120, (H - mh) // 2), mark)
    x = 120 + mark.width + 56
    Font(700, 64).draw(d, (x, 250), "TsuFiles", TEXT)
    reg = Font(400, 30)
    reg.draw(d, (x, 312), "Chuyển đổi tệp ngoại tuyến, kéo thả là xong", TEXT2)
    reg.draw(d, (x, 356), "Hơn 260 định dạng - tiếng Việt, mã nguồn mở", TEXT2)
    semi = Font(600, 22)
    label = "files.tsudev.com"
    bw = semi.width(label) + 56
    d.rounded_rectangle((x, 400, x + bw, 456), radius=28, fill=PRIMARY)
    semi.draw(d, (x + 28, 436), label, ON_PRIMARY)
    Font(600, 30).draw(d, (W - 110 - Font(600, 30).width("tsudev"), H - 100), "tsudev", TEXT)
    return og


if __name__ == "__main__":
    if "--kiem" in sys.argv[1:]:
        sys.exit(check())
    build()
    sys.exit(check())
