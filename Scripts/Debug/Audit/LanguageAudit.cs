namespace UnityLiteL10n.Audit
{
    using System.Collections.Generic;

    public class LanguageAudit
    {
        public string Language;

        public int KeyCount;
        public int ReferenceKeyCount;

        public float Coverage;

        public List<string> MissingKeys = new();
        public List<string> ExtraKeys = new();
        public List<string> EmptyValues = new();

        public bool HasIssues =>
            MissingKeys.Count > 0 ||
            EmptyValues.Count > 0;
    }
}
