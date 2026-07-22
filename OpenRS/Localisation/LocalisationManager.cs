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

            return value;
        }
    }
}
