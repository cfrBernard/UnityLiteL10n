namespace UnityLiteL10n.Audit.Rules
{
    using UnityLiteL10n.Core;

    public class MissingKeysRule : ILocalizationAuditRule
    {
        public void Evaluate(LocalizationStore store, string referenceLanguage, LocalizationAuditResult result)
        {
            if (!store.HasLanguage(referenceLanguage))
                return;

            var referenceKeys = store.AllTexts[referenceLanguage];
            result.ReferenceKeyCount = referenceKeys.Count;

            foreach (var (lang, dict) in store.AllTexts)
            {
                if (lang == referenceLanguage)
                    continue;

                var audit = new LanguageAudit
                {
                    Language = lang,
                    KeyCount = dict.Count,
                    ReferenceKeyCount = referenceKeys.Count
                };

                foreach (var key in referenceKeys.Keys)
                {
                    if (!dict.ContainsKey(key))
                        audit.MissingKeys.Add(key);
                }

                audit.Coverage =
                    referenceKeys.Count == 0
                        ? 1f
                        : 1f - (float)audit.MissingKeys.Count / referenceKeys.Count;

                result.Languages[lang] = audit;
            }
        }
    }
}
