# OpenSSF Best Practices - prepared answers for TsuFiles

Answers for the **passing** level of the [OpenSSF Best Practices badge](https://www.bestpractices.dev). The
maintainer registers the project at <https://www.bestpractices.dev/en/projects/new> (sign in with GitHub, repository
`https://github.com/tsudev-org/tsufiles`) and copies the answers below; each one links to the evidence. Prepared
14/11/2026 (ticket `logs/handover/20261113-03`); keep it in step with the repository when a fact changes.

After the badge is granted, add its badge line to the top of `README.md` and `README.en.md` next to the Scorecard
badge, and write the project id here: `<id>`.

## Basics

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `description_good` | Met | README.en.md first paragraph: offline file converter for 260+ formats |
| `interact` | Met | Issues, Discussions and security advisories linked from README.en.md |
| `contribution` | Met | [CONTRIBUTING.md](../CONTRIBUTING.md) |
| `contribution_requirements` | Met | CONTRIBUTING.md "Code style", "Tests", "Pull requests"; PR template |
| `floss_license` | Met | MIT, [LICENSE](../LICENSE) (keeps the upstream Filee copyright) |
| `floss_license_osi` | Met | MIT is OSI-approved |
| `license_location` | Met | `LICENSE` at the repository root |
| `documentation_basics` | Met | README.en.md, <https://files.tsudev.com/huong-dan/>, `docs/` |
| `documentation_interface` | Met | Command line reference <https://files.tsudev.com/dong-lenh/>, `tsufiles help` |
| `sites_https` | Met | github.com and files.tsudev.com are HTTPS only |
| `discussion` | Met | GitHub Issues + Discussions (searchable, URL-addressable, no proprietary client) |
| `english` | Met | README.en.md, code comments and issues accepted in English |
| `maintained` | Met | Releases 2.4.0-vn.1 (11/11/2026), 2.4.1-vn.1 (14/11/2026); Dependabot PRs merged |

## Change control

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `repo_public` | Met | <https://github.com/tsudev-org/tsufiles> |
| `repo_track` | Met | Git |
| `repo_interim` | Met | Every change lands through a pull request on `main` |
| `repo_distributed` | Met | Git |
| `version_unique` | Met | Tags `v<upstream>-vn.<n>`, e.g. `v2.4.1-vn.1` |
| `version_semver` | Met (suggested) | Upstream SemVer plus a `-vn.<n>` fork revision |
| `version_tags` | Met | Every release has a git tag |
| `release_notes` | Met | [CHANGELOG.md](../CHANGELOG.md) and GitHub release notes (English summary from 2.4.1-vn.1) |
| `release_notes_vulns` | Met | CHANGELOG lists security fixes; none with a CVE so far |

## Reporting

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `report_process` | Met | Issue templates in `.github/ISSUE_TEMPLATE/` |
| `report_tracker` | Met | GitHub Issues |
| `report_responses` | Met | Maintainer answers issues; repository is young (first public release 11/11/2026) |
| `enhancement_responses` | Met | Feature request template, labels `enhancement`, `help wanted` |
| `report_archive` | Met | GitHub Issues are public and archived |
| `vulnerability_report_process` | Met | [SECURITY.md](../SECURITY.md) |
| `vulnerability_report_private` | Met | GitHub private vulnerability reporting is enabled |
| `vulnerability_report_response` | Met | SECURITY.md: acknowledge in 3 working days, first assessment in 7 days |

## Quality

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `build` | Met | `dotnet build TsuFiles.slnx` (.NET SDK pinned by `global.json`) |
| `build_common_tools` | Met | .NET SDK, PowerShell for packaging |
| `build_floss_tools` | Met | .NET SDK and Avalonia are MIT-licensed |
| `test` | Met | xUnit suites `tests/TsuFiles.{Core,Cli,App,Engines,Platform,Setup}.Tests`, run with `dotnet test` |
| `test_invocation` | Met | `dotnet test` (standard for .NET) |
| `test_most` | Met | Converters, updater, signature checks, CLI, headless UI rendering, installer |
| `test_continuous_integration` | Met | `.github/workflows/ci.yml` on every push and pull request (Windows, Linux, macOS) |
| `test_policy` | Met | CONTRIBUTING.md "Pull requests": "make your change with tests" |
| `tests_are_added` | Met | Recent PRs add tests with each change (e.g. #57-#62, #68) |
| `tests_documented_added` | Met | PR template checklist and CONTRIBUTING.md |
| `warnings` | Met | Nullable reference types, .NET analyzers, `dotnet format --verify-no-changes` in CI |
| `warnings_fixed` | Met | CI fails on formatting/analyzer findings |
| `warnings_strict` | Met | `--severity warn` in the format gate |

## Security

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `know_secure_design` | Met | [docs/vi/KIEN-TRUC-BAO-MAT.md](vi/KIEN-TRUC-BAO-MAT.md) (threat model, defence layers) |
| `know_common_errors` | Met | Same document: path traversal/zip slip, untrusted file parsing, update chain |
| `crypto_published` | Met | ECDSA P-256 + SHA-256 (.NET `System.Security.Cryptography`) |
| `crypto_call` | Met | No home-grown cryptography; platform APIs only |
| `crypto_floss` | Met | .NET runtime cryptography |
| `crypto_keylength` | Met | P-256 (128-bit security) |
| `crypto_working` | Met | No MD5/SHA-1 for security decisions |
| `crypto_weaknesses` | Met | Same as above |
| `crypto_pfs` | N/A | No custom network protocol; HTTPS via the OS/.NET stack |
| `crypto_password_storage` | N/A | No user accounts or passwords |
| `crypto_random` | Met | `RandomNumberGenerator` where randomness matters |
| `delivery_mitm` | Met | Releases over HTTPS; the updater checks SHA-256 and an ECDSA signature of `SHA256SUMS` |
| `delivery_unsigned` | Met | Hashes are never fetched over plain HTTP; signed SHA256SUMS, build provenance attestations |
| `vulnerabilities_fixed_60_days` | Met | No known unpatched vulnerability |
| `vulnerabilities_critical_fixed` | Met | Same |
| `no_leaked_credentials` | Met | gitleaks in CI, GitHub secret scanning + push protection; signing key kept offline (`docs/vi/PHAT-HANH.md`) |

## Analysis

| Criterion | Answer | Evidence |
| --- | --- | --- |
| `static_analysis` | Met | CodeQL `security-extended` on every push/PR (`.github/workflows/security.yml`), OpenSSF Scorecard (`scorecard.yml`) |
| `static_analysis_common_vulnerabilities` | Met | CodeQL security-extended queries |
| `static_analysis_fixed` | Met | Alerts triaged; false positives documented in `logs/STATE.md` |
| `static_analysis_often` | Met | Every push and pull request, plus weekly |
| `dynamic_analysis` | Met (suggested) | Weekly fuzzing of the in-house parsers (`.github/workflows/fuzz.yml`) |
| `dynamic_analysis_unsafe` | N/A | Managed C# code; native engines are third-party binaries pinned by SHA-256 |
| `dynamic_analysis_enable_assertions` | Met | Test builds run with assertions and `--blame-crash` |
| `dynamic_analysis_fixed` | Met | Fuzz findings become regression tests |
