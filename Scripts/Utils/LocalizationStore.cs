namespace UnityLiteL10n.Core
{
    using System.Collections.Generic;

    public class LocalizationStore
    {
        public Dictionary<string, Dictionary<string, string>> AllTexts { get; } = new();
        public HashSet<string> MissingKeys { get; } = new();

        public void Clear()
        {
            AllTexts.Clear();
            MissingKeys.Clear();
        }

        public void SetAll(Dictionary<string, Dictionary<string, string>> data)
        {
            Clear();
            foreach (var kvp in data)
                AllTexts[kvp.Key] = kvp.Value;
        }

        public bool HasLanguage(string lang) => AllTexts.ContainsKey(lang);

        public bool TryGetValue(string lang, string key, out string value)
        {
            value = null;
            return AllTexts.TryGetValue(lang, out var dict) &&
                   dict.TryGetValue(key, out value);
        }
    }
}
