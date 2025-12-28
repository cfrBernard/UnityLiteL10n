namespace UnityLiteL10n
{
    using System;
    using UnityEngine;
    
    using UnityLiteL10n.Core;
    using UnityLiteL10n.Logging;

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

        // Core services
        private LocalizationStore _store;
        private LocalizationLoader _loader;
        private LocalizationLogger _logger;

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

            _store = new LocalizationStore();
            _loader = new LocalizationLoader();
            _logger = new LocalizationLogger(logLevel);

            Reload();
        }

        #endregion

        #region Public API

        public string Get(string rawKey)
        {
            string key = NormalizeKey(rawKey);
            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_store.TryGetValue(CurrentLanguage, key, out var value) &&
                !string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (_store.TryGetValue(DefaultLanguage, key, out var fallback) &&
                !string.IsNullOrEmpty(fallback))
            {
                LogMissingKey(key, CurrentLanguage, fallback: true);
                return fallback;
            }

            LogMissingKey(key, CurrentLanguage, fallback: false);
            return string.Format(missingKeyFormat, key);
        }

        public void SetLanguage(string newLang)
        {
            newLang = NormalizeLanguage(newLang);

            if (CurrentLanguage == newLang)
                return;

            if (!_store.HasLanguage(newLang))
            {
                _logger.Warning($"Trying to set unknown language '{newLang}'");
                if (strictMode)
                    throw new Exception($"Unknown language: {newLang}");
                return;
            }

            CurrentLanguage = newLang;
            OnLanguageChanged?.Invoke();
        }

        public void Reload()
        {
            _store.Clear();
            _logger.Log("Reloading localization data", LocalizationLogLevel.Verbose);

            var data = _loader.LoadFromResources(
                folder: "Localization",
                normalizeLang: NormalizeLanguage,
                normalizeKey: NormalizeKey,
                duplicatePolicy: duplicateKeyPolicy,
                strictMode: strictMode,
                logger: _logger
            );
            
            _store.SetAll(data);

            if (performFullAudit)
                PerformAudit();

            OnLanguageChanged?.Invoke();
        }

        #endregion

        #region Audit

        private void PerformAudit()
        {
            if (!_store.HasLanguage(DefaultLanguage))
            {
                _logger.Warning($"Default language '{DefaultLanguage}' not found, skipping audit");
                return;
            }

            var referenceKeys = _store.AllTexts[DefaultLanguage].Keys;

            foreach (var (lang, dict) in _store.AllTexts)
            {
                if (lang == DefaultLanguage) continue;

                int missingCount = 0;
                foreach (var key in referenceKeys)
                {
                    if (!dict.ContainsKey(key))
                        missingCount++;
                }

                _logger.Log(
                    $"Audit '{lang}': {dict.Count} keys, {missingCount} missing compared to default",
                    LocalizationLogLevel.Verbose
                );
            }
        }

        #endregion

        #region Helpers

        private string NormalizeLanguage(string lang)
        {
            return string.IsNullOrEmpty(lang)
                ? string.Empty
                : lang.Trim().ToUpperInvariant();
        }

        private string NormalizeKey(string key)
        {
            return string.IsNullOrEmpty(key)
                ? string.Empty
                : key.Trim();
        }

        private void LogMissingKey(string key, string lang, bool fallback)
        {
            string id = $"{lang}:{key}";
            if (_store.MissingKeys.Contains(id))
                return;

            _store.MissingKeys.Add(id);

            string msg = fallback
                ? $"Missing key '{key}' in {lang}, fallback used"
                : $"Missing key '{key}' in {lang} and default language";

            _logger.Warning(msg);

            if (strictMode)
                throw new Exception(msg);
        }

        #endregion
    }
}
