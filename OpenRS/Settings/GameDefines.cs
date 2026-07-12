namespace OpenRS.Settings
{
    /// <summary>
    /// Configuration constants class.
    /// </summary>
    public static class GameDefines
    {
        public static string ApplicationName => $"OpenRS v{ClientVersion}";

        public static int GuiTileSize => 32;

        /// <summary>
        /// The client version.
        /// </summary>
        public static int ClientVersion => 3;

        /// <summary>
        /// The IP address of the server.
        /// </summary>
        public static string ServerIp => "127.0.0.1";

        /// <summary>
        /// The port of the server.
        /// </summary>
        public static int ServerPort => 43594;

        /// <summary>
        /// The cache URL.
        /// </summary>
        public static string CacheUrl => "http://216.24.201.81/cache/";

        /// <summary>
        /// The crash URL.
        /// </summary>
        public static string CrashUrl => "http://216.24.201.81/crash.php";
    }
}
