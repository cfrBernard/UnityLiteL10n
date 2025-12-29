namespace UnityLiteL10n.Audit
{
    using UnityLiteL10n.Core;

    public class LocalizationAuditor
    {
        public LocalizationAuditResult Run(LocalizationStore store, string referenceLanguage)
        {
            var result = new LocalizationAuditResult
            {
                ReferenceLanguage = referenceLanguage
            };

            if (!store.HasLanguage(referenceLanguage))
                return result;

            var referenceKeys = store.AllTexts[referenceLanguage].Keys;
            result.ReferenceKeyCount = referenceKeys.Count;

            foreach (var (lang, dict) in store.AllTexts)
            {
                if (lang == referenceLanguage)
                    continue;

                var audit = new LanguageAudit
                {
                    Language = lang,
                    KeyCount = dict.Count
                };

                foreach (var key in referenceKeys)
                {
                    if (!dict.ContainsKey(key))
                        audit.MissingKeys.Add(key);
                }

                result.Languages[lang] = audit;
            }

            return result;
        }
    }
}
