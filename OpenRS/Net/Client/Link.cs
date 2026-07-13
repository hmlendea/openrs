using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public sealed class Link
    {
        public static sbyte[] StreamToSbyte(BinaryReader stream)
        {
            List<sbyte> result = [];
            int byteIndex = 0;

            try
            {
                while (byteIndex < stream.BaseStream.Length)
                {
                    result.Add(stream.ReadSByte());
                    byteIndex += 1;
                }
            }
            catch (IOException) { }

            return result.ToArray();
        }

        public static void AddFile(string filename, BinaryReader reader)
        {
            fileNames[currentFile] = filename;
            fileData[currentFile] = StreamToSbyte(reader);
            currentFile += 1;
        }

        public static void AddFile(string fileName, sbyte[] data)
        {
            fileNames[currentFile] = fileName;
            fileData[currentFile] = data;
            currentFile += 1;
        }

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
            for (int fileIndex = 0; fileIndex < currentFile; fileIndex += 1)
            {
                if (string.Equals(fileNames[fileIndex], fileName))
                {
                    return fileData[fileIndex];
                }
            }

            if (LoadFile(fileName))
            {
                return GetFile(fileName);
            }

            return null;
        }

        public static TcpClient GetSocket(int port)
        {
            for (Link.port = port; Link.port != 0; )
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
            for (ipLookup = ip; ipLookup is not null; )
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
        private static readonly TcpClient socket;
        private static string ipLookup = null;
        private static readonly string address;
        private static int currentFile;
        private static readonly string[] fileNames = new string[50];
        private static readonly sbyte[][] fileData = new sbyte[50][];
    }
}