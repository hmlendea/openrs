using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using RuneScapeSolo.Settings;

namespace RuneScapeSolo.Lib
{
    public class Link
    {
        public static sbyte[] streamToSbyte(BinaryReader stream)
        {
            List<sbyte> list = new List<sbyte>();
            {
                // int ch;
                int c = 0;
                try
                {
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
            }
            return list.ToArray();
        }

        public static void addFile(string filename, BinaryReader reader)
        {

            Link.fileName[currentFile] = filename;

            //     reader.Close();

            //  var f = Path.Combine(Config.CONF_DIR, filename);
            //var bytes = File.ReadAllBytes(f).Select(c => (char)c); ;
            //var sbytes = bytes.Select(c=>Convert.ToSByte(c)).ToArray();//c.t(sbyte[])(Array)bytes;
            Link.fileData[currentFile] = streamToSbyte(reader);

            currentFile++;
        }


        public static void addFile(string fileName, sbyte[] fileData)
        {
            Link.fileName[currentFile] = fileName;

            Link.fileData[currentFile] = fileData;//.Cast<byte>().ToArray();

            currentFile++;
        }

        public static bool loadFile(string fileName)
        {
            try
            {
                var f = new FileInfo(Path.Combine(ApplicationPaths.ConfigurationDirectory, fileName));
                if (f.Exists)
                {

                    addFile(fileName, new BinaryReader(f.OpenRead()));
                    return true;
                }
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error has occured in {nameof(Link)}.cs");
                Console.WriteLine(ex.Message);

                return false;
            }
        }

        public static sbyte[] getFile(string fileName)
        {
            for (int i = 0; i < currentFile; i++)
            {
                if (Link.fileName[i].Equals(fileName))
                {
                    return fileData[i];
                }
            }

            if (loadFile(fileName))
            {
                return getFile(fileName);
            }
            else
            {
                return null;
            }
        }


        public static TcpClient getSocket(int port)
        {
            for (Link.port = port; Link.port != 0;)
            {
                Thread.Sleep(100);
            }

            return socket;
        }

        public static string getAddress(string ip)
        {
            for (iplookup = ip; iplookup != null;)
            {
                Thread.Sleep(100);
            }

            return address;
        }

        public static int uid;
        static int port;
        static TcpClient socket;
        static string iplookup = null;
        static string address;
        static int currentFile;
        static string[] fileName = new string[50];
        static sbyte[][] fileData = new sbyte[50][];
    }
}
