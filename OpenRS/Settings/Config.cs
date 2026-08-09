using System;
using System.IO;

namespace OpenRS.Settings
{
    public static class Config
    {
        public static string ConfigurationDirectory => Path.Combine(AppContext.BaseDirectory, "Data") + Path.DirectorySeparatorChar;

        public static int ClientVersion => 3;

        public static bool MembersFeatures => true;

        public static string ServerIp => "127.0.0.1";

        public static int ServerPort => 43594;
    }
}
