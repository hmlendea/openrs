using System.IO;
using System.Reflection;

namespace RuneScapeSolo.Settings
{
    /// <summary>
    /// Application paths.
    /// </summary>
    public static class ApplicationPaths
    {
        static string rootDirectory;

        /// <summary>
        /// The application directory.
        /// </summary>
        public static string ApplicationDirectory
        {
            get
            {
                if (rootDirectory == null)
                {
                    rootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }

                return rootDirectory;
            }
        }

        /// <summary>
        /// The data directory.
        /// </summary>
        public static string DataDirectory => Path.Combine(ApplicationDirectory, "data");

        /// <summary>
        /// The configuration directory.
        /// </summary>
        public static string ConfigurationDirectory => Path.Combine(ApplicationDirectory, "data");

        /// <summary>
        /// The entities directory.
        /// </summary>
        public static string EntitiesDirectory => Path.Combine(DataDirectory, "Entities");
    }
}
