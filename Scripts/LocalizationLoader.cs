using System;
using System.Collections.Generic;
using UnityEngine;

using UnityLiteL10n.Data;
using UnityLiteL10n.Logging;

namespace UnityLiteL10n.Core
{
    public class LocalizationLoader
    {
        public Dictionary<string, Dictionary<string, string>> LoadFromResources(
            string folder,
            Func<string, string> normalizeLang,
            Func<string, string> normalizeKey,
            DuplicateKeyPolicy duplicatePolicy,
            bool strictMode,
            LocalizationLogger logger
        )
        {
            var result = new Dictionary<string, Dictionary<string, string>>();

            var files = Resources.LoadAll<TextAsset>(folder);

            if (files.Length == 0)
            {
                logger?.Warning($"No localization files found in Resources/{folder}/");
                if (strictMode)
                    throw new Exception("No localization files found.");
                return result;
            }

            foreach (var file in files)
            {
                string lang = normalizeLang(file.name);
                var data = JsonUtility.FromJson<LocalizationData>(file.text);

                if (data?.entries == null)
                {
                    logger?.Error($"Invalid JSON in '{file.name}'");
                    if (strictMode)
                        throw new Exception($"Invalid JSON in {file.name}");
                    continue;
                }

                var dict = new Dictionary<string, string>();

                foreach (var entry in data.entries)
                {
                    string key = normalizeKey(entry.key);
                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (dict.ContainsKey(key))
                    {
                        HandleDuplicate(
                            dict, key, entry.value,
                            duplicatePolicy, logger, strictMode, lang
                        );
                        continue;
                    }

                    dict[key] = entry.value;
                }

                result[lang] = dict;
            }

            return result;
        }

        private void HandleDuplicate(
            Dictionary<string, string> dict,
            string key,
            string value,
            DuplicateKeyPolicy policy,
            LocalizationLogger logger,
            bool strict,
            string lang
        )
        {
            string msg = $"Duplicate key '{key}' in language '{lang}'";

            switch (policy)
            {
                case DuplicateKeyPolicy.Overwrite:
                    dict[key] = value;
                    logger?.Warning(msg + " (overwritten)");
                    break;

                case DuplicateKeyPolicy.KeepFirst:
                    logger?.Warning(msg + " (ignored)");
                    break;

                case DuplicateKeyPolicy.Error:
                    logger?.Error(msg);
                    if (strict)
                        throw new Exception(msg);
                    break;
            }
        }
    }
}
