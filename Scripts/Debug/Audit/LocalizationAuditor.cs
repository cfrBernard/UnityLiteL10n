namespace UnityLiteL10n.Audit
{
    using System.Collections.Generic;
    using UnityLiteL10n.Audit.Rules;
    using UnityLiteL10n.Core;

    public class LocalizationAuditor
    {
        private readonly List<ILocalizationAuditRule> _rules = new()
        {
            new MissingKeysRule(),
            new ExtraKeysRule(),
            new EmptyValuesRule()
        };

        public LocalizationAuditResult Run(
            LocalizationStore store,
            string referenceLanguage)
        {
            var result = new LocalizationAuditResult
            {
                ReferenceLanguage = referenceLanguage
            };

            foreach (var rule in _rules)
                rule.Evaluate(store, referenceLanguage, result);

            return result;
        }
    }
}
