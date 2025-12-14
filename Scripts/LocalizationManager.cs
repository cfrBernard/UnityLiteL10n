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
            foreach (var e in entries)
                dict[e.key] = e.value;
            return dict;
        }
    }

    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }
        public string CurrentLanguage = "EN";
        public string DefaultLanguage = "EN";
        public event Action OnLanguageChanged;

        private Dictionary<string, Dictionary<string, string>> _allTexts
            = new Dictionary<string, Dictionary<string, string>>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllLanguages();
        }

        private void LoadAllLanguages()
        {
            TextAsset[] files = Resources.LoadAll<TextAsset>("Localization");
            foreach (var file in files)
            {
                string lang = file.name.ToUpper();
                var dict = JsonUtility.FromJson<LocalizationData>(file.text).ToDictionary();
                _allTexts[lang] = dict;
            }
            Debug.Log($"Loaded {_allTexts.Count} languages from Resources/Localization/");
        }

        public string Get(string key)
        {
            if (_allTexts.TryGetValue(CurrentLanguage, out var dict) && dict.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value))
                return value;

            if (_allTexts.TryGetValue(DefaultLanguage, out var defaultDict) && defaultDict.TryGetValue(key, out var fallbackValue) && !string.IsNullOrEmpty(fallbackValue))
                return fallbackValue;

            return key;
        }

        public void SetLanguage(string newLang)
        {
            if (CurrentLanguage == newLang) return;
            CurrentLanguage = newLang;
            OnLanguageChanged?.Invoke();
        }
    }
}
