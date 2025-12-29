namespace UnityLiteL10n.Logging
{
    using UnityEngine;

    public class LocalizationLogger
    {
        private readonly LocalizationLogLevel _level;

        public LocalizationLogger(LocalizationLogLevel level)
        {
            _level = level;
        }

        public void Log(string msg, LocalizationLogLevel level)
        {
            if (_level < level) return;
            Debug.Log($"<color=cyan>[UnityLiteL10n]</color> {msg}");
        }

        public void Warning(string msg)
        {
            if (_level < LocalizationLogLevel.Warnings) return;
            Debug.LogWarning($"<color=orange>[UnityLiteL10n]</color> {msg}");
        }

        public void Error(string msg)
        {
            if (_level < LocalizationLogLevel.ErrorsOnly) return;
            Debug.LogError($"<color=red>[UnityLiteL10n]</color> {msg}");
        }
    }
}
