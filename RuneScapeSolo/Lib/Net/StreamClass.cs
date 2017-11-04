using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace RuneScapeSolo.Lib.Net
{
    public class StreamClass : PacketConstruction
    {
        NetworkStream netStream;

        readonly object syncLock = new object();

        public StreamClass(TcpClient socket, GameApplet applet)
        {
            socketClosing = false;
            socketClosed = true;
            this.socket = socket;

            netStream = socket.GetStream();

            socketClosed = false;

            applet.StartThread(run);
        }

        public bool Connected
        {
            get
            {
                return socket != null && socket.Connected;
            }
        }


        void OnRead(IAsyncResult iar)
        {
            try
            {
                var len = netStream.EndRead(iar);
                if (len != 0)
                {

                }
            }
            catch { }
            try { netStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), netStream); }
            catch
            {
                // We have been disconnected :<
                CloseStream();
            }
        }

        public override void CloseStream()
        {
            base.CloseStream();
            socketClosing = true;
            try
            {
                if (netStream != null)
                {
                    netStream.Close();
                }

                if (netStream != null)
                {
                    netStream.Close();
                }

                if (socket != null)
                {
                    socket.Close();
                }
            }
            catch (IOException ex)
            {
                System.Console.WriteLine("Error closing stream");
            }
            socketClosed = true;
            // synchronized {
            //lock (syncLock)
            //{
            //    Monitor.Pulse(syncLock);
            //}
            //}

            // connectionThread.Abort();

            buffer = null;
        }

        public override int ReadInputStream()
        {
            if (socketClosing)
            {
                return 0;
            }
            else
            {
                int val = netStream.ReadByte();

                return val;
            }
        }

        // We dont like this in C#
        //public override int available()
        //{
        //    if (socketClosing)
        //        return 0;
        //    else
        //    {
        //        //   return (int)inputStream.BaseStream.Length;//Available();
        //        return 2;
        //    }
        //}

        public override void ReadInputStream(int length, int offset, sbyte[] data)
        {
            if (socketClosing)
            {
                return;
            }

            int i = 0;
            int j;

            byte[] org = new byte[data.Length];

            for (; i < length; i += j)
            {
                if ((j = netStream.Read(org, i + offset, length - i)) <= 0)
                {
                    throw new IOException("EOF");
                }

                for (int k = 0; k < data.Length; k++)
                {
                    data[k] = (sbyte)org[k];
                }
            }
        }

        // [MethodImpl(MethodImplOptions.Synchronized)]
        public override void WriteToBuffer(byte[] abyte0, int i, int j)
        {
            if (socketClosed)
            {
                return;
            }

            if (buffer == null)
            {
                buffer = new byte[5000];
            }

            lock (syncLock) // WARNING: synchronized(this)
            {
                for (int k = 0; k < j; k++)
                {
                    buffer[offset] = abyte0[k + i];
                    offset = (offset + 1) % 5000;

                    if (offset == (dataWritten + 4900) % 5000)
                    {
                        throw new IOException("buffer overflow");
                    }
                }

                Monitor.Pulse(syncLock); // WARNING: notify();
            }
        }

        void OnWrite(IAsyncResult iar)
        {
            try
            {
                netStream.EndWrite(iar);
                dataWritten = (dataWritten + lastWriteLen) % 5000;
                try
                {
                    if (offset == dataWritten)
                    {
                        netStream.Flush();
                    }
                }
                catch (IOException ioexception1)
                {
                    base.HasErrors = true;
                    base.ErrorMessage = "Twriter:" + ioexception1;
                }
            }
            catch { }
        }
        int lastWriteLen = 0;
        public void run()
        {
            while (!socketClosed) //  && connectionThread.ThreadState != ThreadState.AbortRequested && connectionThread.ThreadState != ThreadState.Aborted
            {
                int i;
                int j;
                // lock (syncLock)
                {
                    if (offset == dataWritten)
                    {
                        try
                        {
                            //  wait();
                            //Monitor.Wait(syncLock);
                            // System.Threading.Thread.Sleep(10);
                        }
                        catch { }
                    }

                    if (socketClosed)
                    {
                        return;
                    }

                    j = dataWritten;
                    if (offset >= dataWritten)
                    {
                        i = offset - dataWritten;
                    }
                    else
                    {
                        i = 5000 - dataWritten;
                    }
                }
                if (i > 0)
                {
                    try
                    {


                        netStream.Write(buffer, j, i);
                    }
                    catch (IOException ioexception)
                    {
                        base.HasErrors = true;
                        base.ErrorMessage = "Twriter:" + ioexception;
                    }
                    lastWriteLen = i;

                    {
                        dataWritten = (dataWritten + i) % 5000;
                        try
                        {
                            if (offset == dataWritten)
                            {
                                netStream.Flush();
                            }
                        }
                        catch (IOException ioexception1)
                        {
                            base.HasErrors = true;
                            base.ErrorMessage = "Twriter:" + ioexception1;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        TcpClient /*Socket*/ socket;
        bool socketClosing;
        byte[] buffer;
        int dataWritten;
        int offset;
        bool socketClosed;

    }
}

