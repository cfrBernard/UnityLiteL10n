## [v0.2.0] – 2025/12/14

### Added
- Logging system with configurable LogLevel (None, ErrorsOnly, Warnings, Verbose)
- `strictMode` toggle for development / QA / CI to throw on errors
- `Reload()` method + clears `_missingKeys` and reloads JSON files
- `missingKeyFormat` field for customizable missing key display (default: "[{0}]")
- Verbose logging for loaded languages and entry counts
- Warnings for invalid or empty JSON files

### Improved
- Robust handling of null or empty JSON entries
- Clear separation of fallback vs missing key warnings
- Ensured backward compatibility with v0.1.0 usage

### Notes
- Key normalization and duplicate detection postponed for v0.2.1
- Still supports static text only; no variables, plurals, or advanced formatting

---

## [v0.1.0] – 2025/12/14

### Added
- JSON-based localization system
- `LocalizationManager` for runtime language handling
- `LocalizedText` component for TextMeshPro
- Language fallback to default language
- Runtime language switching via event system
- Demo scene showcasing basic usage

### Notes
- Static text only
- No editor tooling yet

---