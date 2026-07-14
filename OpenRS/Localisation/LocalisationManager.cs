using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using OpenRS.Settings;

namespace OpenRS.Localisation
{
    public static class LocalisationManager
    {
        private static Dictionary<string, string> strings = [];

        public static void Load(Language language)
        {
            string filePath = Path.Combine(
                ApplicationPaths.DataDirectory,
                "Localisation",
                $"{language.Code}.json");

            string json = File.ReadAllText(filePath);

            strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? [];
        }

        public static string GetString(string key)
        {
            if (!strings.TryGetValue(key, out string value))
            {
                return key;
            }

            return ResolveReference(value);
        }

        private static string ResolveReference(string value)
        {
            const string ReferencePrefix = "@ref:";
            const int MaxResolutionDepth = 5;

            string current = value;

            for (int depth = 0; depth < MaxResolutionDepth; depth++)
            {
                if (!current.StartsWith(ReferencePrefix))
                {
                    return current;
                }

                string referencedKey = current[ReferencePrefix.Length..];

                if (!strings.TryGetValue(referencedKey, out string referencedValue))
                {
                    return referencedKey;
                }

                current = referencedValue;
            }

            return current;
        }
    }
}
