# FAQ

## Table of Contents

1. [General](#general)

   1. [What is UnityLiteL10n?](#what-is-unitylitel10n)
   2. [Is this tool intended for large projects?](#is-this-tool-intended-for-large-projects)
   3. [Can I use this in production?](#can-i-use-this-in-production)
2. [Localization Features](#localization-features)

   1. [Does UnityLiteL10n support variables or plurals?](#does-unitylitel10n-support-variables-or-plurals)
   2. [How are missing keys handled?](#how-are-missing-keys-handled)
   3. [Is there a fallback language system?](#is-there-a-fallback-language-system)
3. [Data & File Format](#data--file-format)

   1. [Why JSON instead of ScriptableObjects?](#why-json-instead-of-scriptableobjects)
   2. [Where should localization files be stored?](#where-should-localization-files-be-stored)
   3. [How are languages identified?](#how-are-languages-identified)
4. [Debugging & Validation](#debugging--validation)

   1. [What is strict mode?](#what-is-strict-mode)
   2. [How are duplicate keys handled?](#how-are-duplicate-keys-handled)
   3. [What does the full audit do?](#what-does-the-full-audit-do)
   4. [Can I control logging verbosity?](#can-i-control-logging-verbosity)
5. [Runtime & API](#runtime--api)

   1. [Can I change language at runtime?](#can-i-change-language-at-runtime)
   2. [Can localization data be reloaded without restarting the game?](#can-localization-data-be-reloaded-without-restarting-the-game)

---

## General

### What is UnityLiteL10n?

UnityLiteL10n is a lightweight localization system for Unity designed to load simple key/value text translations from JSON files. It focuses on simplicity, fast iteration, and minimal dependencies.

### Is this tool intended for large projects?

UnityLiteL10n is designed primarily for small to medium projects, game jams, and prototypes. While it can be used in larger projects, it does not yet include advanced features such as addressable-based loading, smart pluralization rules, or editor tooling.

### Can I use this in production?

Yes. The system is runtime-safe and supports fallback languages, audits, and configurable error handling. However, you should be aware that advanced localization features are not implemented yet.

---

## Localization Features

### Does UnityLiteL10n support variables or plurals?

No. The tool currently supports static text only. Variables, formatting helpers, and pluralization rules are planned for future versions but are not part of the current implementation.

### How are missing keys handled?

When a key is not found in the current language, UnityLiteL10n will attempt to retrieve it from the default language. If the key is missing in both, the system returns a formatted placeholder (by default `[key]`). This format can be customized via `missingKeyFormat`.

### Is there a fallback language system?

Yes. The `DefaultLanguage` is used as a fallback whenever a key is missing in the active language. Fallback usage is logged (depending on log level) to help detect incomplete translations.

---

## Data & File Format

### Why JSON instead of ScriptableObjects?

JSON allows easy manual editing, source control diffing, and runtime reloading without Unity editor dependencies. This makes it suitable for designers, translators, and external tools.

### Where should localization files be stored?

All localization files must be placed under `Resources/Localization/`. Each JSON file represents one language and is automatically loaded at startup.

### How are languages identified?

Languages are identified by the JSON file name. File names are normalized to uppercase (e.g. `en`, `EN`, and `En` will all resolve to `EN`).

---

## Debugging & Validation

### What is strict mode?

When `strictMode` is enabled, UnityLiteL10n throws exceptions instead of silently failing or logging warnings. This applies to missing keys, duplicate keys (depending on policy), invalid JSON files, and unknown languages.

### How are duplicate keys handled?

Duplicate keys within the same language file are handled according to `DuplicateKeyPolicy`:

* **Overwrite**: the last value wins (default)
* **KeepFirst**: subsequent duplicates are ignored
* **Error**: duplicates trigger an error (and an exception in strict mode)

### What does the full audit do?

When `performFullAudit` is enabled, all languages are checked against the default language after loading. The audit reports how many keys are missing per language, helping ensure translation completeness.

### Can I control logging verbosity?

Yes. The `LocalizationLogLevel` setting allows you to control how much information is logged:

* `None`
* `ErrorsOnly`
* `Warnings`
* `Verbose`

<br>

> ⚠️ **Full debugging features are documented** [here](DebugFeatures.md) 

---

## Runtime & API

### Can I change language at runtime?

Yes. Calling `SetLanguage()` changes the active language at runtime. When the language changes, the `OnLanguageChanged` event is invoked so UI systems can refresh displayed text.

### Can localization data be reloaded without restarting the game?

Yes. Calling `Reload()` clears cached missing keys and reloads all localization files from disk, then triggers `OnLanguageChanged`. This is useful during development or when updating files on the fly.

---
