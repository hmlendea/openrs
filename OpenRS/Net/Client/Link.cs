using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public sealed class Link
    {
        public static sbyte[] StreamToSbyte(BinaryReader stream)
            => SignedByteStreamReader.ReadRemaining(stream);

        public static void AddFile(string filename, BinaryReader reader)
            => LinkFileCache.Add(filename, StreamToSbyte(reader));

        public static void AddFile(string fileName, sbyte[] data)
            => LinkFileCache.Add(fileName, data);

        public static bool LoadFile(string fileName)
        {
            try
            {
                FileInfo fileInfo = new(Path.Combine(Config.ConfigurationDirectory, fileName));

                if (fileInfo.Exists)
                {
                    AddFile(fileName, new BinaryReader(fileInfo.OpenRead()));

                    return true;
                }

                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static sbyte[] GetFile(string fileName)
        {
            int fileIndex = LinkFileCache.FindIndex(fileName);

            if (fileIndex >= 0)
            {
                return LinkFileCache.GetData(fileIndex);
            }

            if (LoadFile(fileName))
            {
                return GetFile(fileName);
            }

            return null;
        }

        public static TcpClient GetSocket(int port)
        {
            for (Link.port = port; Link.port != 0;)
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch (Exception) { }
            }

            return socket;
        }

        public static string GetAddress(string ip)
        {
            for (ipLookup = ip; ipLookup is not null;)
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch (Exception) { }
            }

            return address;
        }

        public static int userId;
        private static int port;
        private static readonly TcpClient socket = null;
        private static string ipLookup;
        private static readonly string address = null;
    }
}