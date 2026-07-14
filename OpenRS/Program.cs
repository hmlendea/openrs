using System;

using Microsoft.Extensions.Configuration;

using OpenRS.Settings;

namespace OpenRS
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            NuciLoggerFactory.Initialise(configuration);

            using GameWindow game = new();
            game.Run();
        }
    }
}
