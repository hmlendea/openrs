namespace RuneScapeSolo.Settings
{
    /// <summary>
    /// Configuration constants class.
    /// </summary>
    public static class GameDefines
    {
        public static string ApplicationName => $"RuneScape Solo v{CLIENT_VERSION}";

        /// <summary>
        /// The client version.
        /// </summary>
        public const int CLIENT_VERSION = 3;

        /// <summary>
        /// Flag indicating whether the premium features are enabled.
        /// </summary>
        public const bool PREMIUM_FEATURES = true;

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        public const string SERVER_IP = "127.0.0.1";

        /// <summary>
        /// The port of the server.
        /// </summary>
        public const int SERVER_PORT = 43594;

        /// <summary>
        /// The cache URL.
        /// </summary>
        public const string CACHE_URL = "http://216.24.201.81/cache/";

        /// <summary>
        /// The crash URL.
        /// </summary>
        public const string CRASH_URL = "http://216.24.201.81/crash.php";
    }
}
