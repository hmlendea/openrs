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

        TcpClient socket;
        bool socketClosing;
        byte[] buffer;
        int dataWritten;
        int offset;
        bool socketClosed;
        int lastWriteLen;

        public StreamClass(TcpClient socket, GameApplet applet)
        {
            socketClosing = false;
            socketClosed = true;
            this.socket = socket;

            netStream = socket.GetStream();

            socketClosed = false;

            applet.StartThread(run);
        }

        public bool IsConnected => socket != null && socket.Connected;

        void OnRead(IAsyncResult result)
        {
            try
            {
                var len = netStream.EndRead(result);

                if (len != 0)
                {

                }
            }
            catch { }

            try
            {
                netStream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnRead), netStream);
            }
            catch
            {
                CloseStream();
            }
        }

        public override void CloseStream()
        {
            base.CloseStream();
            socketClosing = true;

            try
            {
                netStream?.Close();
                netStream?.Close();
                socket?.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine("Error closing stream");
            }

            socketClosed = true;

            lock (syncLock)
            {
                Monitor.Pulse(syncLock);
            }

            //connection thread abort

            buffer = null;
        }

        public override int ReadInputStream()
        {
            if (socketClosing)
            {
                return 0;
            }

            return netStream.ReadByte();
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

            byte[] org = new byte[data.Length];
            int i = 0;

            while (i < length)
            {
                int j = netStream.Read(org, i + offset, length - i);

                if (j <= 0)
                {
                    throw new IOException("EOF");
                }

                i += j;
            }

            for (int k = 0; k < length; k++)
            {
                data[k] = (sbyte)org[k];
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
                    HasErrors = true;
                    ErrorMessage = "Twriter:" + ioexception1;
                }
            }
            catch { }
        }

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
    }
}

