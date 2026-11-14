// Sinh dữ liệu khối "Cài bằng lệnh" của /tai-ve/ lúc build (phiếu 20261114-02, kế hoạch 20261114-01 việc C5).
//
// Nguồn sự thật là catalog của BA kho gói tsudev (cùng hợp đồng schema 1 mà tsudev.com/apps đọc qua
// tsudev/scripts/pkg-catalog-sync.js): APT/RPM pkg.tsudev.com, Scoop bucket, Homebrew tap. Lọc gói có
// homepage = trang này; bỏ gói chuyển tiếp (replacedBy). KHÔNG chép tay tên gói hay lệnh: kho đổi tên gói thì
// lần build sau trang tự đúng. Riêng winget đọc mã gói từ packaging/package.json (manifest nằm ở
// microsoft/winget-pkgs, không có catalog) và chỉ hiện khi "winget" đã ở "channels".
//
// Lỗi bất kỳ (mạng, schema, vân tay khóa, không tìm thấy gói nào) => thoát mã 1, build THẤT BẠI có thông báo -
// không lặng lẽ ra khối rỗng. Tệp sinh ra: src/data/cai-bang-lenh.generated.json (không commit, .gitignore).
import { mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const goc = dirname(fileURLToPath(import.meta.url));
const OUT = join(goc, '../src/data/cai-bang-lenh.generated.json');
const SITE = 'https://files.tsudev.com';
const NGUON = {
  pkg: 'https://pkg.tsudev.com/catalog.json',
  scoop: 'https://raw.githubusercontent.com/tsudev-org/scoop-bucket/main/catalog.json',
  brew: 'https://raw.githubusercontent.com/tsudev-org/homebrew-tap/main/catalog.json',
};
// Vân tay khóa ký kho APT/RPM: giá trị CỨNG, không tin vân tay tải về (khớp PKG_REPO của tsudev và keys/ của
// tsudev-org/linux-packages).
const VAN_TAY = '388310F47FA8224D76C0A9C79745B6AF3EA33C74';

function dung(thongBao) {
  console.error(`[cai-bang-lenh] LỖI: ${thongBao}`);
  console.error('[cai-bang-lenh] Không sinh được khối "Cài bằng lệnh" - build dừng (phiếu 20261114-02 mục 2).');
  process.exit(1);
}

const chuoi = (v, ten) => {
  if (typeof v !== 'string' || !v.trim() || /[\r\n]/.test(v)) dung(`${ten}: cần một dòng lệnh, nhận ${JSON.stringify(v)}`);
  return v.trim();
};

async function tai(loai, url) {
  let res;
  try {
    res = await fetch(url, { signal: AbortSignal.timeout(20000), headers: { accept: 'application/json' } });
  } catch (e) {
    dung(`${loai}: không tải được ${url} (${e.cause?.code ?? e.message})`);
  }
  if (!res.ok) dung(`${loai}: ${url} trả HTTP ${res.status}`);
  let cat;
  try {
    cat = await res.json();
  } catch {
    dung(`${loai}: ${url} không phải JSON`);
  }
  if (cat.schema !== 1) dung(`${loai}: schema ${cat.schema} - script chỉ hiểu schema 1`);
  if (loai === 'pkg' && cat.key?.fingerprint !== VAN_TAY) dung(`vân tay khóa kho ${cat.key?.fingerprint} KHÁC giá trị cứng ${VAN_TAY}`);
  if (!Array.isArray(cat.packages)) dung(`${loai}: thiếu mảng packages`);
  const cua = cat.packages.filter((p) => !p.replacedBy && String(p.homepage ?? '').replace(/\/$/, '') === SITE);
  if (cua.length > 1) dung(`${loai}: ${cua.length} gói cùng homepage ${SITE} (${cua.map((p) => p.name).join(', ')})`);
  const p = cua[0];
  if (p && !/^[a-z][a-z0-9-]+$/.test(p.name ?? '')) dung(`${loai}: tên gói không hợp lệ: ${p.name}`);
  return { cat, p };
}

const kenh = [];
const pk = JSON.parse(readFileSync(join(goc, '../../packaging/package.json'), 'utf8'));
if ((pk.channels ?? []).includes('winget')) {
  if (!/^[A-Za-z0-9-]+\.[A-Za-z0-9.-]+$/.test(pk.winget ?? '')) dung(`packaging/package.json: mã winget không hợp lệ: ${pk.winget}`);
  kenh.push({ he: 'windows', ten: 'winget', lenh: [`winget install --id ${pk.winget} -e`] });
}

const [pkg, scoop, brew] = await Promise.all(Object.entries(NGUON).map(([loai, url]) => tai(loai, url)));
if (scoop.p) kenh.push({ he: 'windows', ten: 'Scoop', goi: scoop.p.name, lenh: [chuoi(scoop.p.installFull, 'scoop.installFull')] });
if (brew.p) kenh.push({ he: 'macos', ten: 'Homebrew', goi: brew.p.name, lenh: [chuoi(brew.p.installFull, 'brew.installFull')] });
let lenhLinux = [];
if (pkg.p) {
  const setup = pkg.cat.setup ?? {};
  for (const [ma, ten] of [['apt', 'APT (Debian, Ubuntu, Linux Mint)'], ['dnf', 'DNF (Fedora, RHEL)']]) {
    if (!Array.isArray(setup[ma]) || !pkg.p.install?.[ma]) dung(`pkg: thiếu setup.${ma} hoặc install.${ma} của ${pkg.p.name}`);
    kenh.push({ he: 'linux', ten, goi: pkg.p.name, lenh: [...setup[ma].map((l, i) => chuoi(l, `setup.${ma}[${i}]`)), chuoi(pkg.p.install[ma], `install.${ma}`)] });
  }
  lenhLinux = (pkg.p.commands ?? []).filter((c) => /^[a-z][a-z0-9-]*$/.test(c));
}
if (kenh.length === 0) dung(`không kho nào có gói với homepage ${SITE}`);

const duLieu = {
  // Không ghi thời điểm tải: tệp chỉ đổi khi gói đổi.
  nguon: Object.values(NGUON),
  vanTay: VAN_TAY.replace(/(.{4})/g, '$1 ').trim(),
  lenhLinux,
  kenh,
};
mkdirSync(dirname(OUT), { recursive: true });
writeFileSync(OUT, JSON.stringify(duLieu, null, 2) + '\n');
console.log(`[cai-bang-lenh] ${kenh.map((k) => `${k.ten.split(' ')[0]}${k.goi ? `=${k.goi}` : ''}`).join(', ')}`);
