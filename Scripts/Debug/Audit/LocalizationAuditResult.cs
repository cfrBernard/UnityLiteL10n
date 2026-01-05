namespace UnityLiteL10n.Audit
{
    using System.Collections.Generic;
    using System.Linq;

    public class LocalizationAuditResult
    {
        public string ReferenceLanguage;
        public int ReferenceKeyCount;

        public Dictionary<string, LanguageAudit> Languages = new();

        public bool HasIssues =>
            Languages.Values.Any(l => l.HasIssues);
    }
}
