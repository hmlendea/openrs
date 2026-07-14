using System;
using System.IO;
using System.Reflection;

namespace OpenRS.Settings
{
    public static class ApplicationPaths
    {
        private static string rootDirectory;
        private static string localAppData;

        public static string ApplicationDirectory
        {
            get
            {
                rootDirectory ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                return rootDirectory;
            }
        }

        public static string UserDataDirectory
        {
            get
            {
                localAppData ??= Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                return Path.Combine(localAppData, "OpenRS");
            }
        }

        public static string ConfigurationDirectory => Path.Combine(ApplicationDirectory, "Data");

        public static string DataDirectory => Path.Combine(ApplicationDirectory, "Data");

        public static string AnimationsDirectory => Path.Combine(DataDirectory, "Animations");

        public static string ChatFilterDirectory => Path.Combine(DataDirectory, "ChatFilter");

        public static string FontsDirectory => Path.Combine(DataDirectory, "Fonts");

        public static string MediaDirectory => Path.Combine(DataDirectory, "Media");

        public static string MapsDirectory => Path.Combine(DataDirectory, "Maps");

        public static string TexturesDirectory => Path.Combine(DataDirectory, "Textures");

        public static string EntitiesDirectory => Path.Combine(DataDirectory, "Entities");

        public static string SettingsFile => Path.Combine(UserDataDirectory, "Settings.xml");
    }
}
