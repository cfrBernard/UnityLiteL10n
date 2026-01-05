namespace UnityLiteL10n.Audit.Rules
{
    using System;
    using UnityLiteL10n.Core;

    public class EmptyValuesRule : ILocalizationAuditRule
    {
        public void Evaluate(LocalizationStore store, string referenceLanguage, LocalizationAuditResult result)
        {
            foreach (var (lang, dict) in store.AllTexts)
            {
                if (lang == referenceLanguage)
                    continue;

                if (!result.Languages.TryGetValue(lang, out var audit))
                    continue;

                foreach (var (key, value) in dict)
                {
                    if (string.IsNullOrWhiteSpace(value) ||
                        string.Equals(key, value, StringComparison.Ordinal))
                    {
                        audit.EmptyValues.Add(key);
                    }
                }
            }
        }
    }
}
