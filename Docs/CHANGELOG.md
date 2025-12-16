## [v0.4.0] – 2025/12/16

### Added
- Key normalization (Trim) to avoid trailing/leading spaces
- Duplicate key handling with configurable behavior:
  - Overwrite existing
  - Keep first (ignore)
  - Throw error
- Optional post-load audit to report:
  - Missing keys
  - Fallback usage
- Colored logs according to log level for better readability in console

### Improved
- Moved LocalizationData DTO logic into LocalizationManager for better control and logging
- Updated code presentation:
  - Regions for logical separation
  - Tooltips for serializable fields
- Full documentation (README, FAQ, DebugFeatures)

### Notes
- No breaking changes to the core localization API
- Existing keys and behavior remain fully compatible
- EditorWindow planned for future version

---

## [v0.3.0] – 2025/12/15

### Added
- Added Japanese (JA) and Korean (KO) localization files for demo purposes
- Extended demo to showcase multi-language support beyond Latin alphabets

### Improved
- Updated demo UI with a cleaner and minimal layout
- Improved overall demo readability and presentation

### Notes
- No changes to the core localization API
- No behavior changes in loading, fallback, or logging

---

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