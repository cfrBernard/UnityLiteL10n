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

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (entries == null) return dict;

            foreach (var e in entries)
            {
                if (!string.IsNullOrEmpty(e.key))
                    dict[e.key] = e.value;
            }

            return dict;
        }
    }

    public enum LocalizationLogLevel
    {
        None,
        ErrorsOnly,
        Warnings,
        Verbose
    }

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        [Header("Languages")]
        public string CurrentLanguage = "EN";
        public string DefaultLanguage = "EN";

        [Header("Debug")]
        [SerializeField] private string missingKeyFormat = "[{0}]";
        [SerializeField] private LocalizationLogLevel logLevel = LocalizationLogLevel.Warnings;
        [SerializeField] private bool strictMode = false;

        public event Action OnLanguageChanged;

        private readonly Dictionary<string, Dictionary<string, string>> _allTexts = new();
        private readonly HashSet<string> _missingKeys = new();

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

        private void LoadAllLanguages()
        {
            _allTexts.Clear();

            TextAsset[] files = Resources.LoadAll<TextAsset>("Localization");

            if (files.Length == 0)
            {
                LogWarning("No localization files found in Resources/Localization/");
                if (strictMode)
                    throw new Exception("[UnityLiteL10n] No localization files found.");
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

                var dict = data.ToDictionary();

                if (dict.Count == 0)
                {
                    LogWarning($"Language '{lang}' contains no entries");
                    if (strictMode)
                        throw new Exception($"Empty localization file: {lang}");
                }

                _allTexts[lang] = dict;
                Log($"Loaded language '{lang}' ({dict.Count} entries)", LocalizationLogLevel.Verbose);
            }

            Log($"Loaded {_allTexts.Count} languages total", LocalizationLogLevel.Verbose);
        }

        public string Get(string key)
        {
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

        private string NormalizeLanguage(string lang)
        {
            return string.IsNullOrEmpty(lang)
                ? string.Empty
                : lang.Trim().ToUpperInvariant();
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

        private void Log(string message, LocalizationLogLevel level)
        {
            if (logLevel < level) return;
            Debug.Log($"[UnityLiteL10n] {message}");
        }

        private void LogWarning(string message)
        {
            if (logLevel < LocalizationLogLevel.Warnings) return;
            Debug.LogWarning($"[UnityLiteL10n] {message}");
        }

        private void LogError(string message)
        {
            if (logLevel < LocalizationLogLevel.ErrorsOnly) return;
            Debug.LogError($"[UnityLiteL10n] {message}");
        }
    }
}
