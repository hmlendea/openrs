using Microsoft.Extensions.Configuration;

using NuciLog;
using NuciLog.Configuration;
using NuciLog.Core;

namespace OpenRS.Logging
{
    internal static class NuciLoggerFactory
    {
        private static readonly NuciLoggerSettings settings = new();

        internal static void Initialise(IConfiguration configuration)
            => configuration.GetSection(nameof(NuciLoggerSettings)).Bind(settings);

        internal static ILogger CreateLogger<T>()
        {
            ILogger logger = new NuciLogger(settings);
            logger.SetSourceContext<T>();

            return logger;
        }
    }
}
