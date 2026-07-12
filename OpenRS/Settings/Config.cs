using System;
using System.IO;

namespace OpenRS.Settings
{
    public class Config
    {
        public static string CONF_DIR = Path.Combine(AppContext.BaseDirectory, "Data") + Path.DirectorySeparatorChar;
        public static string MEDIA_DIR = Path.Combine(AppContext.BaseDirectory, "Data") + Path.DirectorySeparatorChar;

        public static int CLIENT_VERSION = 3;
        public static bool MEMBERS_FEATURES = true;

        public static String SERVER_IP = "127.0.0.1";//83.248.5.67
        public static int SERVER_PORT = 43594;

        public static String CACHE_URL = "http://216.24.201.81/cache/";
        public static String CRASH_URL = "http://216.24.201.81/crash.php";
    }
}
