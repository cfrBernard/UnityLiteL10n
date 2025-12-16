# Debug & Visualization Features

This document describes all debugging, logging, and validation features available in **UnityLiteL10n** as of **v0.4.0**, based solely on the runtime behavior of `LocalizationManager`.

The goal of these features is to help you:

* Detect missing or invalid localization data early
* Identify duplicate keys and broken language files
* Visualize localization issues directly in-game
* Choose the right balance between safety (strictness) and iteration speed

---

<br>

<p align="center">
  <img src="Img\LocManagerComponent_v0.4.0.PNG" alt="DemoGIF v0.3.0" />
</p>

> LocalizationManager inspector – all debug and validation settings are centralized here.

---

## Table of Contents

- [1. Logging System](#1-logging-system)
- [2. Missing Key Visualization](#2-missing-key-visualization)
- [3. Missing Key Deduplication](#3-missing-key-deduplication)
- [4. Duplicate Key Detection](#4-duplicate-key-detection)
- [5. Strict Mode (Fail-Fast Debugging)](#5-strict-mode-fail-fast-debugging)
- [6. Full Language Audit](#6-full-language-audit)
- [7. Language Validation & Switching](#7-language-validation--switching)
- [8. Reload & Hot Iteration](#8-reload--hot-iteration)
- [9. Recommended Debug Configurations](#9-recommended-debug-configurations)
- [10. Debug Philosophy](#10-debug-philosophy)

---

## 1. Logging System

UnityLiteL10n provides a configurable logging system through the `LocalizationLogLevel` enum.

### Log Levels

```csharp
public enum LocalizationLogLevel
{
    None,
    ErrorsOnly,
    Warnings,
    Verbose
}
```

| Level          | Behavior                                |
| -------------- | --------------------------------------- |
| **None**       | No logs at all                          |
| **ErrorsOnly** | Only critical errors are logged         |
| **Warnings**   | Errors + warnings (recommended default) |
| **Verbose**    | Full diagnostic output                  |

### What Gets Logged

* Missing localization files
* Invalid or empty JSON files
* Duplicate keys per language
* Missing keys at runtime
* Language loading summary
* Audit results (verbose only)

All logs are prefixed and color-coded:

* Cyan: general information
* Orange: warnings
* Red: errors

This makes localization issues easy to spot in the Unity Console.

---

## 2. Missing Key Visualization

When a localization key cannot be resolved, UnityLiteL10n provides **visual feedback directly in the UI**.

### Missing Key Format

```csharp
[SerializeField] private string missingKeyFormat = "[{0}]";
```

If a key is missing in both the current and default language, the returned string will be:

```
[MY_MISSING_KEY]
```

This makes missing entries immediately visible in-game without crashing.

### Fallback Behavior

Lookup order:

1. Current language
2. Default language
3. Missing key format

If the key exists only in the default language:

* The default value is returned
* A warning is logged once per key per language

---

## 3. Missing Key Deduplication

To avoid console spam, missing keys are logged **only once per language**.

Internally, UnityLiteL10n tracks missing keys using:

```csharp
lang:key
```

This ensures:

* Clean logs even in UI-heavy scenes
* Reliable detection of *new* missing keys

The cache is cleared when calling:

```csharp
LocalizationManager.Reload();
```

---

## 4. Duplicate Key Detection

Duplicate keys inside the same language file are detected during load.

### DuplicateKeyPolicy

```csharp
public enum DuplicateKeyPolicy
{
    Overwrite,
    KeepFirst,
    Error
}
```

| Policy        | Behavior                          |
| ------------- | --------------------------------- |
| **Overwrite** | Last value wins (warning logged)  |
| **KeepFirst** | First value kept (warning logged) |
| **Error**     | Error logged, optional exception  |

> Duplicate counts are summarized per language file after parsing.

---

## 5. Strict Mode (Fail-Fast Debugging)

```csharp
[SerializeField] private bool strictMode = false;
```

When **Strict Mode** is enabled, UnityLiteL10n throws exceptions instead of silently recovering.

### Strict Mode Triggers

Exceptions are thrown when:

* No localization files are found
* A JSON file is invalid or empty
* Duplicate keys are detected (with `DuplicateKeyPolicy.Error`)
* A language is missing or unknown
* A localization key is missing (with or without fallback)

This mode is ideal for:

* CI pipelines
* Automated tests
* Early production validation

> ⚠️ **Not recommended for shipped builds**.

---

## 6. Full Language Audit

```csharp
[SerializeField] private bool performFullAudit = true;
```

When enabled, UnityLiteL10n compares **all languages** against the **default language** after loading.

### Audit Rules

* The default language is treated as the reference
* Other languages are checked for missing keys
* Results are logged in **Verbose** mode only

Example output:

```
Audit 'FR': 120 keys, 5 missing compared to default
```

> This helps identify incomplete translations early.

---

## 7. Language Validation & Switching

When switching languages using:

```csharp
SetLanguage("FR");
```

### Debug Behaviors

* Unknown languages trigger warnings
* Strict mode throws an exception
* No reload occurs if the language is unchanged

The `OnLanguageChanged` event is fired after a successful change.

---

## 8. Reload & Hot Iteration

```csharp
LocalizationManager.Reload();
```

Reloading:

* Clears the missing key cache
* Reloads all JSON files from `Resources/Localization`
* Re-runs duplicate detection
* Re-runs the full audit (if enabled)
* Fires `OnLanguageChanged`

This is ideal for:

* Iterating on localization files
* Editor tools
* Debug menus

---

## 9. Recommended Debug Configurations

### Development (Safe & Informative)

```text
Log Level: Verbose
Strict Mode: false
Duplicate Policy: Overwrite
Full Audit: false
```

Good balance between visibility and iteration speed.

---

### QA / CI Validation

```text
Log Level: Verbose
Strict Mode: true
Duplicate Policy: Error
Full Audit: true
```

Fail-fast configuration to catch all localization issues.

---

### Production Build

```text
Log Level: ErrorsOnly
Strict Mode: false
Duplicate Policy: Overwrite
Full Audit: false
```

Minimal overhead, no user-facing crashes.

---

## 10. Debug Philosophy

UnityLiteL10n is designed to:

* **Fail loudly in development**
* **Fail gracefully in production**
* Provide **visual feedback instead of silent bugs**

> If you see brackets in your UI, something is wrong – and that's intentional.

---
