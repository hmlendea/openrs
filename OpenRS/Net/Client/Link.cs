using System;
using System.Collections.Generic;
using System.IO;

using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class Link
    {
        public static int uid;
        static string iplookup;
        static int currentFile;
        static string[] fileName = new string[50];
        static sbyte[][] fileData = new sbyte[50][];

        public static sbyte[] streamToSbyte(BinaryReader stream)
        {
            List<sbyte> list = new List<sbyte>();

            try
            {
                int c = 0;

                while (c < stream.BaseStream.Length)
                {
                    list.Add(stream.ReadSByte());
                    c++;
                }
            }
            catch
            {
                Console.WriteLine($"An error has occured in {nameof(Link)}.cs");
            }

            return list.ToArray();
        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <returns><c>true</c>, if file was loaded, <c>false</c> otherwise.</returns>
        /// <param name="fileName">File name.</param>
        public static bool LoadFile(string fileName)
        {
            string path = Path.Combine(ApplicationPaths.ConfigurationDirectory, fileName);

            try
            {
                FileInfo f = new FileInfo(path);

                if (f.Exists)
                {
                    AddFile(fileName, new BinaryReader(f.OpenRead()));

                    return true;
                }

                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error has occured in {nameof(Link)}.cs");
                Console.WriteLine(ex);

                return false;
            }
        }

        public static sbyte[] GetFile(string fileName)
        {
            for (int i = 0; i < currentFile; i++)
            {
                if (Link.fileName[i].Equals(fileName))
                {
                    return fileData[i];
                }
            }

            if (LoadFile(fileName))
            {
                return GetFile(fileName);
            }

            return null;
        }

        static void AddFile(string filename, BinaryReader reader)
        {
            fileName[currentFile] = filename;
            fileData[currentFile] = streamToSbyte(reader);

            currentFile += 1;
        }
    }
}
