namespace UnityLiteL10n.Audit
{
    using UnityLiteL10n.Core;

    public interface ILocalizationAuditRule
    {
        void Evaluate(
            LocalizationStore store,
            string referenceLanguage,
            LocalizationAuditResult result
        );
    }
}
