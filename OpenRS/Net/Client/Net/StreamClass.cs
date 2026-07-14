using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Net
{
    public sealed class StreamClass : PacketConstruction
    {

        private readonly Thread connectionThread;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<StreamClass>();

        public StreamClass(/*Socket*/ TcpClient socket, GameApplet a1 = null)
        {
            socketClosing = false;
            socketClosed = true;
            this.socket = socket;

            inputStream = new BinaryReader(socket.GetStream()); // socket.getInputStream();
            outputStream = new BinaryWriter(socket.GetStream()); // socket.getOutputStream();
            socketClosed = false;

            connectionThread = new Thread(new ThreadStart(this.Run));
            connectionThread.Start();
            //   a1.startThread(this);
        }

        public bool Connected
        {
            get
            {
                return this.socket is not null && this.socket.Connected;
            }
        }

        private void OnRead(IAsyncResult iar)
        {
            try
            {
                int bytesRead = inputStream.BaseStream.EndRead(iar);
                if (bytesRead != 0)
                {

                }
            }
            catch { }
            try { inputStream.BaseStream.BeginRead(this.buffer, 0, this.buffer.Length, new AsyncCallback(OnRead), inputStream); }
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
                inputStream?.Close();

                outputStream?.Close();

                socket?.Close();
            }
            catch (IOException)
            {
                logger.Error(GameOperation.NetworkDisconnect, "Failed to close the stream.");
            }
            socketClosed = true;
            // synchronized {
            //lock (syncLock)
            //{
            //    Monitor.Pulse(syncLock);
            //}
            //}

            buffer = null;
        }

        public override int Read()
        {
            if (socketClosing)
            {
                return 0;
            }
            else
            {
                return inputStream.ReadByte();
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

        public override void ReadInputStream(int byteCount, int bufferOffset, sbyte[] outputBuffer)
        {
            if (socketClosing)
            {
                return;
            }

            int i = 0;
            int j;
            try
            {
                byte[] org = new byte[outputBuffer.Length];
                for (; i < byteCount; i += j)
                {
                    if (socketClosing)
                    {
                        return;
                    }

                    if (!socket.Connected)
                    {
                        return;
                    }

                    if ((j = inputStream.Read(org, i + bufferOffset, byteCount - i)) <= 0)
                    {
                        ;
                    }
                    //throw new IOException("EOF");

                    for (int k = 0; k < outputBuffer.Length; k += 1)
                    {
                        outputBuffer[k] = (sbyte)org[k];
                    }

                }
            }
            catch
            {
            }

        }

        private readonly object syncLock = new();

        // [MethodImpl(MethodImplOptions.Synchronized)]
        public override void WriteToBuffer(byte[] sourceData, int sourceOffset, int byteCount)
        {
            if (socketClosing)
            {
                return;
            }

            if (buffer is null)
            {
                buffer = new byte[5000];
            }
            // lock (syncLock)
            {
                for (int i = 0; i < byteCount; i += 1)
                {
                    buffer[offset] = sourceData[i + sourceOffset];
                    offset = (offset + 1) % 5000;
                    if (offset == (dataWritten + 4900) % 5000)
                    {
                        throw new IOException("buffer overflow");
                    }
                }
                //     Monitor.PulseAll(syncLock);
                //Monitor.Pulse(connectionThread);
            }
        }

        private int lastWriteLen;
        public void Run()
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

                        outputStream.Write(buffer, j, i);
                    }
                    catch (IOException ioexception)
                    {
                        error = true;
                        errorText = "Twriter:" + ioexception;
                    }
                    lastWriteLen = i;

                    {
                        dataWritten = (dataWritten + i) % 5000;
                        try
                        {
                            if (offset == dataWritten)
                            {
                                outputStream.Flush();
                            }
                        }
                        catch (IOException ioexception1)
                        {
                            error = true;
                            errorText = "Twriter:" + ioexception1;
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        private readonly BinaryReader /*InputStream*/ inputStream;
        private readonly BinaryWriter /*OutputStream*/ outputStream;
        private readonly TcpClient /*Socket*/ socket;
        private bool socketClosing;
        private byte[] buffer;
        private int dataWritten;
        private int offset;
        private bool socketClosed;

    }
}
