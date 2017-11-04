using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

using RuneScapeSolo.Enumerations;
using RuneScapeSolo.Lib.Data;
using RuneScapeSolo.Lib.Game;
using RuneScapeSolo.Lib.Net;

namespace RuneScapeSolo.Lib
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
            friendsList = new long[40];
            friendsWorld = new int[400];
            ignoresList = new long[200];
        }

        public void connect(string user, string pass, bool reconnecting)
        {
            if (isConnecting)
            {
                isConnecting = !reconnecting;
            }

            if (socketTimeout > 0)
            {
                loginScreenPrint("Please wait...", "Connecting to server");
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Exception ex) { }
                loginScreenPrint("Sorry! The server is currently full.", "Please try again later");
                return;
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
                Console.WriteLine(e.ToString());
                // e.printStackTrace();
            }

        }

        void DoConnect()
        {
            //username = user;
            var user = DataOperations.formatString(username, 20);
            // password = pass;
            var pass = DataOperations.formatString(password, 20);
            if (user.Trim().Length == 0)
            {
                loginScreenPrint("You must enter both a username", "and a password - Please try again");
                return;
            }
            if (reconnecting)
            {
                gameBoxPrint("Connection lost! Please wait...", "Attempting to re-establish");
            }
            else
            {
                loginScreenPrint("Please wait...", "Connecting to server");
            }

            TcpClient socket = MakeSocket(Configuration.SERVER_IP, Configuration.SERVER_PORT);
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
                //     loginScreenPrint("Login server offline.", "Please try again in a few mins");
                //     return;
            }
            Console.WriteLine($"Session ID: {sessionId}");

            int[] sessionRotationKeys = new int[4];
            sessionRotationKeys[0] = (int)(Helper.Random.NextDouble() * 99999999D);
            sessionRotationKeys[1] = (int)(Helper.Random.NextDouble() * 99999999D);
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

            StreamClass.AddInt16(Configuration.CLIENT_VERSION);
            StreamClass.AddBytes(encryptor.Packet, 0, encryptor.Offset);
            StreamClass.FinalisePacket();

            LoginCode loginResponse = (LoginCode)StreamClass.ReadInputStream();

            Console.WriteLine($"Login response: {(int)loginResponse} ({loginResponse})");

            // streamClass.MakeAsync();

            if (loginResponse == LoginCode.Code99)
            {
                reconnectTries = 0;
                initVars();
                return;
            }
            if (loginResponse == LoginCode.Code0)
            {
                reconnectTries = 0;
                initVars();
                return;
            }
            if (loginResponse == LoginCode.Code1)
            {
                reconnectTries = 0;
                return;
            }
            if (reconnecting)
            {
                user = "";
                pass = "";
                resetIntVars();
                return;
            }
            if (loginResponse == LoginCode.ServerTimeOut)
            {
                loginScreenPrint("Error unable to login.", "Server timed out");
                return;
            }
            if (loginResponse == LoginCode.InvalidCredentials)
            {
                loginScreenPrint("Invalid username or password.", "Try again, or create a new account");
                return;
            }
            if (loginResponse == LoginCode.UsernameAlreadyLoggedIn)
            {
                loginScreenPrint("That username is already logged in.", "Wait 60 seconds then retry");
                return;
            }
            if (loginResponse == LoginCode.ClientUpdated)
            {
                loginScreenPrint("The client has been updated.", "Please restart the client");
                return;
            }
            if (loginResponse == LoginCode.Code5)
            {
                loginScreenPrint("Error unable to login.", "Please retry");
                return;
            }
            if (loginResponse == LoginCode.AccountBanned)
            {
                loginScreenPrint("Account banned.", "Appeal on the forums, ASAP.");
                return;
            }
            if (loginResponse == LoginCode.ProfileDecodeFailure)
            {
                loginScreenPrint("Error - failed to decode profile.", "Contact an admin!");
                return;
            }
            if (loginResponse == LoginCode.TooManyConnections)
            {
                loginScreenPrint("Too many connections from your IP.", "Please try again later");
                return;
            }
            if (loginResponse == LoginCode.AccountAlreadyLoggedIn)
            {
                loginScreenPrint("Account already in use.", "You may only login to one character at a time");
                return;
            }
            else
            {
                loginScreenPrint("Error unable to login.", "Unrecognised response code");
                return;
            }

            if (reconnectTries > 0)
            {
                try
                {
                    Thread.Sleep(2500);
                }
                catch (Exception ex) { }
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
                loginScreenPrint("Sorry! Unable to connect.", "Check internet settings or try another world");
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
                catch (IOException ex) { }
            }

            username = "";
            password = "";
            resetIntVars();

            loginScreenPrint("Please enter your usename and password", "");
        }

        public virtual void lostConnection()
        {
            Console.WriteLine("Lost connection");
            //connect(username, password, true);
            loginScreenPrint("Please enter your usename and password", "");
        }

        protected void gameBoxPrint(string s1, string s2)
        {

            //Font font = new Font("Helvetica", 1, 15);
            char c = '\u0200';
            char c1 = '\u0158';
            // g.setColor(Color.Black);

            //g.fillRect(c / 2 - 140, c1 / 2 - 25, 280, 50, Color.Black);

            //g.setColor(Color.White);
            //g.drawRect(c / 2 - 140, c1 / 2 - 25, 280, 50, Color.White);
            //drawString(s1/*, font*/, c / 2, c1 / 2 - 10, Color.White);
            //drawString(s2/*, font*/, c / 2, c1 / 2 + 10, Color.White);
        }

        protected void sendPingPacket()
        {
            long l = CurrentTimeMillis();

            if (StreamClass.hasData())
            {
                lastPing = l;
            }

            if (l - lastPing > 5000L)
            {
                lastPing = l;
                StreamClass.CreatePacket(5);
                StreamClass.FormatPacket();
            }

            try
            {
                StreamClass.WritePacket(20);
            }
            catch (IOException ex)
            {
                lostConnection();
                return;
            }

            int length = StreamClass.readPacket(data);

            if (length > 0)
            {
                int commandId = data[0] & 0xff;
                ServerCommand command = (ServerCommand)commandId;

                //Console.WriteLine("PACKET:" + command + " LEN:" + length);
                HandlePacket(command, length);
            }
        }

        public virtual void HandlePacket(ServerCommand command, int length)
        {

            if (command == ServerCommand.ServerAnnouncement)
            {
                string message = Encoding.UTF8.GetString((byte[])(Array)data, 1, length);
                displayMessage(message);

                return;
            }
            if (command == ServerCommand.LogoutRequest)
            {
                requestLogout();
                return;
            }
            if (command == ServerCommand.LogoutCannot)
            {
                cantLogout();
                return;
            }
            if (command == ServerCommand.Command249)
            {
                friendsCount = DataOperations.GetInt8(data[1]);

                for (int i = 0; i < friendsCount; i++)
                {
                    friendsList[i] = DataOperations.getLong(data, 2 + i * 9);
                    friendsWorld[i] = DataOperations.GetInt8(data[10 + i * 9]);
                }

                reOrderFriendsList();

                return;
            }
            if (command == ServerCommand.Command25)
            {
                long friend = DataOperations.getLong(data, 1);
                int status = data[9] & 0xff;

                for (int j1 = 0; j1 < friendsCount; j1++)
                {
                    if (friendsList[j1] == friend)
                    {
                        if (friendsWorld[j1] == 0 && status != 0)
                        {
                            displayMessage("@pri@" + DataOperations.LongToString(friend) + " has logged in");
                        }

                        if (friendsWorld[j1] != 0 && status == 0)
                        {
                            displayMessage("@pri@" + DataOperations.LongToString(friend) + " has logged out");
                        }

                        friendsWorld[j1] = status;
                        length = 0;
                        reOrderFriendsList();

                        return;
                    }
                }

                friendsList[friendsCount] = friend;
                friendsWorld[friendsCount] = status;
                friendsCount++;

                reOrderFriendsList();

                return;
            }
            if (command == ServerCommand.Command2)
            {
                ignoresCount = DataOperations.GetInt8(data[1]);

                for (int j = 0; j < ignoresCount; j++)
                {
                    ignoresList[j] = DataOperations.getLong(data, 2 + j * 8);
                }

                return;
            }
            if (command == ServerCommand.Command158)
            {
                blockChat = data[1];
                blockPrivate = data[2];
                blockTrade = data[3];
                blockDuel = data[4];

                return;
            }
            if (command == ServerCommand.PrivateMessage)
            {
                long user = DataOperations.getLong(data, 1);
                string s = ChatMessage.bytesToString(data, 9, length - 9);
                displayMessage("@pri@" + DataOperations.LongToString(user) + ": tells you " + s);

                return;
            }

            HandlePacket(command, length, data);
        }

        void reOrderFriendsList()
        {
            bool flag = true;

            while (flag)
            {
                flag = false;

                for (int i = 0; i < friendsCount - 1; i++)
                {
                    if (friendsWorld[i] < friendsWorld[i + 1])
                    {
                        int j = friendsWorld[i];
                        friendsWorld[i] = friendsWorld[i + 1];
                        friendsWorld[i + 1] = j;
                        long l = friendsList[i];
                        friendsList[i] = friendsList[i + 1];
                        friendsList[i + 1] = l;
                        flag = true;
                    }
                }
            }
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

        protected void AddIgnore(string arg0)
        {
            long l = DataOperations.nameToHash(arg0);

            StreamClass.CreatePacket(25);
            StreamClass.AddInt64(l);
            StreamClass.FormatPacket();

            for (int i = 0; i < ignoresCount; i++)
            {
                if (ignoresList[i] == l)
                {
                    return;
                }
            }

            if (ignoresCount >= ignoresList.Length - 1)
            {
                return;
            }

            ignoresList[ignoresCount++] = l;
        }

        protected void removeIgnore(long arg0)
        {
            StreamClass.CreatePacket(108);
            StreamClass.AddInt64(arg0);
            StreamClass.FormatPacket();

            for (int i = 0; i < ignoresCount; i++)
            {
                if (ignoresList[i] == arg0)
                {
                    ignoresCount--;
                    for (int j = i; j < ignoresCount; j++)
                    {
                        ignoresList[j] = ignoresList[j + 1];
                    }

                    return;
                }
            }
        }

        protected void addFriend(string arg0)
        {
            StreamClass.CreatePacket(168);
            StreamClass.AddInt64(DataOperations.nameToHash(arg0));
            StreamClass.FormatPacket();
            long l = DataOperations.nameToHash(arg0);

            for (int i = 0; i < friendsCount; i++)
            {
                if (friendsList[i] == l)
                {
                    return;
                }
            }

            if (friendsCount >= friendsList.Length - 1)
            {
                return;
            }
            else
            {
                friendsList[friendsCount] = l;
                friendsWorld[friendsCount] = 0;
                friendsCount++;
                return;
            }
        }

        protected void removeFriend(long arg0)
        {
            StreamClass.CreatePacket(52);
            StreamClass.AddInt64(arg0);
            StreamClass.FormatPacket();

            for (int i = 0; i < friendsCount; i++)
            {
                if (friendsList[i] != arg0)
                {
                    continue;
                }

                friendsCount--;
                for (int j = i; j < friendsCount; j++)
                {
                    friendsList[j] = friendsList[j + 1];
                    friendsWorld[j] = friendsWorld[j + 1];
                }

                break;
            }

            displayMessage("@pri@" + DataOperations.LongToString(arg0) + " has been removed from your friends list");
        }

        protected void sendPrivateMessage(long l, byte[] abyte0, int i)
        {
            StreamClass.CreatePacket(254);
            StreamClass.AddInt64(l);
            StreamClass.AddBytes(abyte0, 0, i);
            StreamClass.FormatPacket();
        }

        protected void SendChatMessage(string message)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(message);

            StreamClass.CreatePacket(145);
            StreamClass.AddBytes(bytes);
            StreamClass.FormatPacket();
        }

        protected void SendCommand(string command)
        {
            StreamClass.CreatePacket(90);
            StreamClass.AddString(command);
            StreamClass.FormatPacket();
        }

        public virtual void loginScreenPrint(string s1, string s2)
        {
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

        public virtual void HandlePacket(ServerCommand command, int j, sbyte[] abyte0)
        {
        }

        public virtual void displayMessage(string s1)
        {
        }

        public static int maxPacketReadCount;
        public string username;
        string password;
        public sbyte[] data;
        public int reconnectTries;
        public long lastPing;
        public int friendsCount;
        public long[] friendsList;
        public int[] friendsWorld;
        public int ignoresCount;
        public long[] ignoresList;
        public int blockChat;
        public int blockPrivate;
        public int blockTrade;
        public int blockDuel;
        public long sessionId;
        public int socketTimeout;


        public bool reconnecting { get; set; }
    }
}
