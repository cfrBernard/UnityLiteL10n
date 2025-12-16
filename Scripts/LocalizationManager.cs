namespace UnityLiteL10n
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class LocalizationData
    {
        public LocalizationEntry[] entries;
    }

    public enum LocalizationLogLevel
    {
        None,
        ErrorsOnly,
        Warnings,
        Verbose
    }

    public enum DuplicateKeyPolicy
    {
        Overwrite,
        KeepFirst,
        Error
    }

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Languages")]
        [Tooltip("Current active language")]
        public string CurrentLanguage = "EN";

        [Tooltip("Fallback language when key is missing")]
        public string DefaultLanguage = "EN";

        [Header("Debug")]
        [Tooltip("Format used when a key is missing")]
        [SerializeField] private string missingKeyFormat = "[{0}]";

        [Tooltip("Policy to handle duplicate keys within the same language file")]
        [SerializeField] private DuplicateKeyPolicy duplicateKeyPolicy = DuplicateKeyPolicy.Overwrite;

        [Tooltip("Level of logging for localization system")]
        [SerializeField] private LocalizationLogLevel logLevel = LocalizationLogLevel.Warnings;

        [Tooltip("If true, strict mode will throw exceptions on missing/duplicate keys")]
        [SerializeField] private bool strictMode = false;

        [Tooltip("If true, perform post-load audit of all languages against the default language")]
        [SerializeField] private bool performFullAudit = true;

        public event Action OnLanguageChanged;

        private readonly Dictionary<string, Dictionary<string, string>> _allTexts = new();
        private readonly HashSet<string> _missingKeys = new();

        #region Unity

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            CurrentLanguage = NormalizeLanguage(CurrentLanguage);
            DefaultLanguage = NormalizeLanguage(DefaultLanguage);

            LoadAllLanguages();
        }

        #endregion

        #region Loading

        private void LoadAllLanguages()
        {
            _allTexts.Clear();

            TextAsset[] files = Resources.LoadAll<TextAsset>("Localization");

            if (files.Length == 0)
            {
                LogWarning("No localization files found in Resources/Localization/");
                if (strictMode)
                    throw new Exception("[UnityLiteL10n] No localization files found.");
                return;
            }

            foreach (var file in files)
            {
                string lang = NormalizeLanguage(file.name);

                LocalizationData data = JsonUtility.FromJson<LocalizationData>(file.text);

                if (data == null || data.entries == null)
                {
                    LogError($"Failed to parse localization file '{file.name}'");
                    if (strictMode)
                        throw new Exception($"Invalid JSON in {file.name}");
                    continue;
                }

                var dict = BuildDictionary(data, lang);

                if (dict.Count == 0)
                {
                    LogWarning($"Language '{lang}' contains no entries");
                    if (strictMode)
                        throw new Exception($"Empty localization file: {lang}");
                }

                _allTexts[lang] = dict;
                Log($"Loaded language '{lang}' ({dict.Count} entries)", LocalizationLogLevel.Verbose);
            }

            if (performFullAudit)
                PerformAudit();

            Log($"Loaded {_allTexts.Count} languages total", LocalizationLogLevel.Verbose);
        }

        private Dictionary<string, string> BuildDictionary(LocalizationData data, string lang)
        {
            var dict = new Dictionary<string, string>();
            int duplicateCount = 0;

            foreach (var entry in data.entries)
            {
                string key = NormalizeKey(entry.key);

                if (string.IsNullOrEmpty(key))
                    continue;

                if (dict.ContainsKey(key))
                {
                    duplicateCount++;
                    HandleDuplicateKey(lang, key, entry.value, dict);
                    continue;
                }

                dict.Add(key, entry.value);
            }

            if (duplicateCount > 0)
            {
                Log($"Language '{lang}' contains {duplicateCount} duplicate keys", LocalizationLogLevel.Warnings);
            }

            return dict;
        }

        #endregion

        #region Public API

        public string Get(string rawKey)
        {
            string key = NormalizeKey(rawKey);

            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_allTexts.TryGetValue(CurrentLanguage, out var dict) &&
                dict.TryGetValue(key, out var value) &&
                !string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (_allTexts.TryGetValue(DefaultLanguage, out var fallbackDict) &&
                fallbackDict.TryGetValue(key, out var fallbackValue) &&
                !string.IsNullOrEmpty(fallbackValue))
            {
                LogMissingKey(key, CurrentLanguage, fallback: true);
                return fallbackValue;
            }

            LogMissingKey(key, CurrentLanguage, fallback: false);
            return string.Format(missingKeyFormat, key);
        }

        public void SetLanguage(string newLang)
        {
            newLang = NormalizeLanguage(newLang);

            if (CurrentLanguage == newLang)
                return;

            if (!_allTexts.ContainsKey(newLang))
            {
                LogWarning($"Trying to set unknown language '{newLang}'");
                if (strictMode)
                    throw new Exception($"Unknown language: {newLang}");
                return;
            }

            CurrentLanguage = newLang;
            OnLanguageChanged?.Invoke();
        }

        public void Reload()
        {
            _missingKeys.Clear();
            Log("Reloading localization data", LocalizationLogLevel.Verbose);
            LoadAllLanguages();
            OnLanguageChanged?.Invoke();
        }

        #endregion

        #region Audit

        private void PerformAudit()
        {
            if (!_allTexts.ContainsKey(DefaultLanguage))
            {
                LogWarning($"Default language '{DefaultLanguage}' not found, skipping audit");
                return;
            }

            var referenceKeys = _allTexts[DefaultLanguage].Keys;

            foreach (var (lang, dict) in _allTexts)
            {
                if (lang == DefaultLanguage) continue;

                int missingCount = 0;

                foreach (var key in referenceKeys)
                {
                    if (!dict.ContainsKey(key))
                        missingCount++;
                }

                Log($"Audit '{lang}': {dict.Count} keys, {missingCount} missing compared to default",
                    LocalizationLogLevel.Verbose);
            }
        }

        #endregion

        #region Normalization

        private string NormalizeLanguage(string lang)
        {
            return string.IsNullOrEmpty(lang)
                ? string.Empty
                : lang.Trim().ToUpperInvariant();
        }

        private string NormalizeKey(string key)
        {
            return string.IsNullOrEmpty(key) ? string.Empty : key.Trim();
        }

        #endregion

        #region Duplicate & Missing Keys

        private void HandleDuplicateKey(string lang, string key, string value, Dictionary<string, string> dict)
        {
            string msg = $"Duplicate key '{key}' in language '{lang}'";

            switch (duplicateKeyPolicy)
            {
                case DuplicateKeyPolicy.Overwrite:
                    dict[key] = value;
                    LogWarning(msg + " (overwritten)");
                    break;

                case DuplicateKeyPolicy.KeepFirst:
                    LogWarning(msg + " (ignored)");
                    break;

                case DuplicateKeyPolicy.Error:
                    LogError(msg);
                    if (strictMode)
                        throw new Exception(msg);
                    break;
            }
        }

        private void LogMissingKey(string key, string lang, bool fallback)
        {
            string id = $"{lang}:{key}";
            if (_missingKeys.Contains(id)) return;

            _missingKeys.Add(id);

            string msg = fallback
                ? $"Missing key '{key}' in {lang}, fallback used"
                : $"Missing key '{key}' in {lang} and default language";

            LogWarning(msg);

            if (strictMode)
                throw new Exception(msg);
        }

        #endregion

        #region Logging
        
        private void Log(string message, LocalizationLogLevel level)
        {
            if (logLevel < level) return;
            Debug.Log($"<color=cyan>[UnityLiteL10n]</color> {message}");
        }
        
        private void LogWarning(string message)
        {
            if (logLevel < LocalizationLogLevel.Warnings) return;
            Debug.LogWarning($"<color=orange>[UnityLiteL10n]</color> {message}");
        }
        
        private void LogError(string message)
        {
            if (logLevel < LocalizationLogLevel.ErrorsOnly) return;
            Debug.LogError($"<color=red>[UnityLiteL10n]</color> {message}");
        }
        
        #endregion
    }
}
