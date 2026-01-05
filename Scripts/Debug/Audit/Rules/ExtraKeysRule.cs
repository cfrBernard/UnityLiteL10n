namespace UnityLiteL10n.Audit.Rules
{
    using UnityLiteL10n.Core;
    using System.Linq;

    public class ExtraKeysRule : ILocalizationAuditRule
    {
        public void Evaluate(LocalizationStore store, string referenceLanguage, LocalizationAuditResult result)
        {
            if (!store.HasLanguage(referenceLanguage))
                return;

            var referenceKeys = store.AllTexts[referenceLanguage].Keys;

            foreach (var (lang, dict) in store.AllTexts)
            {
                if (lang == referenceLanguage)
                    continue;

                if (!result.Languages.TryGetValue(lang, out var audit))
                    continue;

                foreach (var key in dict.Keys)
                {
                    if (!referenceKeys.Contains(key))
                        audit.ExtraKeys.Add(key);
                }
            }
        }
    }
}
