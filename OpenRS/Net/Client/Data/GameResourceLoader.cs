using System;
using System.IO;
using System.Net.Http;

using OpenRS.Settings;

namespace OpenRS.Net.Client.Data
{
    internal sealed class GameResourceLoader
    {
        private static int StreamBufferSize => 2048;

        internal static Uri CodeBase { get; set; }

        internal static MemoryStream OpenInputStream(string fileName)
        {
            Stream inputStream;

            if (CodeBase is null)
            {
                inputStream = File.OpenRead(Path.Combine(Config.ConfigurationDirectory, fileName));
            }
            else
            {
                Uri url = new(CodeBase, fileName);
                using HttpClient httpClient = new();
                inputStream = httpClient.GetStreamAsync(url).GetAwaiter().GetResult();
            }

            using (inputStream)
            {
                MemoryStream memory = new();
                byte[] buffer = new byte[StreamBufferSize];
                int bytesRead;

                while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memory.Write(buffer, 0, bytesRead);
                }

                return memory;
            }
        }
    }
}
