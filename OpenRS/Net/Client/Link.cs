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
        public static sbyte[] streamToSbyte(BinaryReader stream)
        {
            List<sbyte> list = [];
            {
                // int ch;
                int c = 0;
                try
                {
                    while (c < stream.BaseStream.Length)
                    {
                        list.Add(stream.ReadSByte());
                        c += 1;
                    }
                }
                catch { }
            }
            return list.ToArray();
        }

        public static void addFile(String filename, BinaryReader reader)
        {

            Link.fileName[currentFile] = filename;

       //     reader.Close();

          //  var f = Path.Combine(Config.CONF_DIR, filename);
            //var bytes = File.ReadAllBytes(f).Select(c => (char)c); ;
            //var sbytes = bytes.Select(c=>Convert.ToSByte(c)).ToArray();//c.t(sbyte[])(Array)bytes;
            Link.fileData[currentFile] = streamToSbyte(reader);

            currentFile += 1;
        }


        public static void addFile(String fileName, sbyte[] fileData)
        {
            Link.fileName[currentFile] = fileName;

            Link.fileData[currentFile] = fileData;//.Cast<byte>().ToArray();

            currentFile += 1;
        }

        public static bool loadFile(String fileName)
        {
            try
            {
                FileInfo fileInfo = new(Path.Combine(Config.ConfigurationDirectory, fileName));
                if (fileInfo.Exists)
                {

                    addFile(fileName, new BinaryReader(fileInfo.OpenRead()));
                    return true;
                }
                return false;
            }
            catch (IOException ioe)
            {
                // ioe.printStackTrace();
                return false;
            }
        }

        public static sbyte[] getFile(String fileName)
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
            for (Link.port = port; Link.port != 0; )
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch (Exception _ex) { }
            }

            return socket;
        }

        //public static void thread(Runnable runnable) {
        //    for(thread = runnable; thread is not null;)
        //        try {
        //            Thread.Sleep(100);
        //        }
        //        catch(Exception _ex) { }

        //}

        public static String getAddress(String ip)
        {
            for (iplookup = ip; iplookup is not null; )
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch (Exception _ex) { }
            }

            return address;
        }

        //public static Applet gameApplet;
        public static int uid;
        private static int port;
        private static TcpClient socket;
        //  static Runnable thread = null;
        private static String iplookup = null;
        private static String address;
        private static int currentFile;
        private static String[] fileName = new String[50];
        private static sbyte[][] fileData = new sbyte[50][];

    }
}