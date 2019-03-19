using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;

using NuciExtensions;

using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Net;
using OpenRS.Net.Enumerations;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class GameAppletMiddleMan : GameApplet
    {
        public static Random ran = new Random();
        static bool isConnecting = false;
        Thread connectionThread;

        public StreamClass StreamClass { get; set; }

        static BigInteger key = BigInteger.Parse("1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335");
        static BigInteger modulus = BigInteger.Parse("1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");

        public GameAppletMiddleMan()
        {
            username = "";
            password = "";
            data = new sbyte[10000];
        }

        public void connect(string user, string pass, bool reconnecting)
        {
            if (isConnecting)
            {
                isConnecting = !reconnecting;
            }

            if (socketTimeout > 0)
            {
                Thread.Sleep(2000);

                throw new LoginException("The server is currently full");
            }
            try
            {
                if (isConnecting)
                {
                    return;
                }

                isConnecting = true;

                username = user;
                password = pass;

                connectionThread = new Thread(new ThreadStart(DoConnect));
                connectionThread.Start();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void DoConnect()
        {
            var user = DataOperations.FormatString(username, 20);
            var pass = DataOperations.FormatString(password, 20);

            if (user.Trim().Length == 0)
            {
                return;
            }

            TcpClient socket = MakeSocket(GameDefines.SERVER_IP, GameDefines.SERVER_PORT);
            StreamClass = new StreamClass(socket, this);
            StreamClass.MaximumPacketReadCount = maxPacketReadCount;

            long l = DataOperations.nameToHash(user);
            StreamClass.CreatePacket(32);
            StreamClass.AddInt8((int)(l >> 16 & 31L));
            StreamClass.AddString("&%..."); // TODO: not used server-side
            StreamClass.FinalisePacket();

            long sessionId = StreamClass.ReadInt64();

            if (sessionId == 0L)
            {
                throw new LoginException("Login server offline");
            }

            Console.WriteLine($"Session ID: {sessionId}");

            int[] sessionRotationKeys = new int[4];
            sessionRotationKeys[0] = (int)(ran.NextDouble() * 99999999D);
            sessionRotationKeys[1] = (int)(ran.NextDouble() * 99999999D);
            sessionRotationKeys[2] = (int)(sessionId >> 32);
            sessionRotationKeys[3] = (int)sessionId;

            LoginEncryptor encryptor = new LoginEncryptor(new byte[500]);
            encryptor.AddInt32(sessionRotationKeys[0]);
            encryptor.AddInt32(sessionRotationKeys[1]);
            encryptor.AddInt32(sessionRotationKeys[2]);
            encryptor.AddInt32(sessionRotationKeys[3]);
            encryptor.AddInt32(0);
            encryptor.AddString(user);
            encryptor.AddString(pass);
            encryptor.EncryptPacket(key, modulus);

            StreamClass.CreatePacket(0);

            if (reconnecting)
            {
                StreamClass.AddInt8(1);
            }
            else
            {
                StreamClass.AddInt8(0);
            }

            StreamClass.AddInt16(GameDefines.CLIENT_VERSION);
            StreamClass.AddBytes(encryptor.Packet, 0, encryptor.Offset);
            StreamClass.FinalisePacket();

            LoginCode loginResponse = (LoginCode)StreamClass.ReadInputStream();

            Console.WriteLine($"Login response: {(int)loginResponse} ({loginResponse})");

            // streamClass.MakeAsync();

            switch (loginResponse)
            {
                case LoginCode.Code0:
                    reconnectTries = 0;
                    initVars();
                    return;

                case LoginCode.Code1:
                    reconnectTries = 0;
                    return;

                case LoginCode.Code5:
                    throw new LoginException("CODE 5");

                case LoginCode.Code99:
                    reconnectTries = 0;
                    initVars();
                    return;

                case LoginCode.AccountBanned:
                    throw new LoginException("Account banned");

                case LoginCode.AccountAlreadyLoggedIn:
                    throw new LoginException("Account already in use");

                case LoginCode.ClientUpdated:
                    throw new LoginException("The client has been updated");

                case LoginCode.InvalidCredentials:
                    throw new LoginException("Invalid credentials");

                case LoginCode.ProfileDecodeFailure:
                    throw new LoginException("Failed to decode the profile");

                case LoginCode.ServerTimeOut:
                    throw new LoginException("Server timed out");

                case LoginCode.TooManyConnections:
                    throw new LoginException("Too many connections from the same IP");

                case LoginCode.UsernameAlreadyLoggedIn:
                    throw new LoginException("Already logged in");

                default:
                    throw new LoginException();
            }

            if (reconnecting)
            {
                user = "";
                pass = "";
                resetIntVars();
                return;
            }

            if (reconnectTries > 0)
            {
                Thread.Sleep(2500);

                reconnectTries--;
                connect(username, password, reconnecting);
            }
            if (reconnecting)
            {
                username = "";
                password = "";
                resetIntVars();
            }
            else
            {
                throw new LoginException("Unable to connect");
            }
        }

        protected void requestLogout()
        {
            if (StreamClass != null)
            {
                try
                {
                    StreamClass.CreatePacket(39);
                    StreamClass.FinalisePacket();
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"An error has occured in {nameof(GameAppletMiddleMan)}.cs");
                }
            }

            username = "";
            password = "";
            resetIntVars();
        }

        public virtual void LostConnection()
        {
            Console.WriteLine("Lost connection");
        }

        protected void SendPing()
        {
            long time = DateTime.Now.GetCurrentTimeMilliseconds();

            if (StreamClass.HasData)
            {
                lastPing = time;
            }

            if (time - lastPing > 5000L)
            {
                lastPing = time;
                StreamClass.CreatePacket(5);
                StreamClass.FormatPacket();
            }

            try
            {
                StreamClass.WritePacket(20);
            }
            catch (IOException ex)
            {
                LostConnection();

                return;
            }

            int length = StreamClass.ReadPacket(data);

            if (length > 0)
            {
                int commandId = data[0] & 0xff;
                ServerCommand command = (ServerCommand)commandId;

                HandlePacket(command, length);
            }
        }

        public virtual void HandlePacket(ServerCommand command, int length)
        {
            HandlePacket(command, length, data);
        }

        protected void sendUpdatedPrivacyInfo(int blockChat, int blockPrivate, int blockTrade, int blockDuel)
        {
            StreamClass.CreatePacket(176);
            StreamClass.AddInt8(blockChat);
            StreamClass.AddInt8(blockPrivate);
            StreamClass.AddInt8(blockTrade);
            StreamClass.AddInt8(blockDuel);
            StreamClass.FormatPacket();
        }

        protected void SendCommand(string command)
        {
            StreamClass.CreatePacket(90);
            StreamClass.AddString(command);
            StreamClass.FormatPacket();
        }

        public virtual void initVars()
        {
        }

        public virtual void resetIntVars()
        {
        }

        public virtual void cantLogout()
        {
        }

        public virtual void HandlePacket(ServerCommand command, int length, sbyte[] data)
        {
        }

        public virtual void DisplayMessage(string message)
        {
        }

        public static int maxPacketReadCount;
        public string username;
        string password;
        public sbyte[] data;
        public int reconnectTries;
        public long lastPing;
        public long sessionId;
        public int socketTimeout;


        public bool reconnecting { get; set; }
    }
}
