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

        private static BigInteger key = BigInteger.Parse("1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335");
        private static BigInteger modulus = BigInteger.Parse("1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");
        
        public GameAppletMiddleMan()
        {
            username = "";
            password = "";
            packetData = new sbyte[10000];
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
                catch (Exception _ex) { }
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

        private void DoConnect()
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
            streamClass = new StreamClass(socket, this);
            streamClass.maxPacketReadCount = maxPacketReadCount;
            
            long l = DataOperations.nameToHash(user);
            streamClass.CreatePacket(32);
            streamClass.AddInt8((int)(l >> 16 & 31L));
            streamClass.AddString("&%..."); // TODO: not used server-side
            streamClass.FinalisePacket();

            long sessionId = streamClass.ReadInt64();
            
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
            
            streamClass.CreatePacket(0);

            if (reconnecting)
            {
                streamClass.AddInt8(1);
            }
            else
            {
                streamClass.AddInt8(0);
            }

            streamClass.AddInt16(Configuration.CLIENT_VERSION);
            streamClass.AddBytes(encryptor.Packet, 0, encryptor.Offset);
            streamClass.FinalisePacket();

            LoginCode loginResponse = (LoginCode)streamClass.ReadInputStream();
            
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
                catch (Exception _ex) { }
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
            if (streamClass != null)
            {
                try
                {
                    streamClass.CreatePacket(39);
                    streamClass.FinalisePacket();
                }
                catch (IOException _ex) { }
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
            if (streamClass.hasData())
            {
                lastPing = l;
            }

            if (l - lastPing > 5000L)
            {
                lastPing = l;
                streamClass.CreatePacket(5);
                streamClass.FormatPacket();
            }
            try
            {
                streamClass.WritePacket(20);
            }
            catch (IOException ex)
            {
                lostConnection();
                return;
            }
            int packetLength = streamClass.readPacket(packetData);
            if (packetLength > 0)
            {
                handlePacket(packetData[0] & 0xff, packetLength);
            }
        }

        public virtual void handlePacket(int command, int length)
        {
            if (command == 48)
            {
                var s1 = Encoding.UTF8.GetString((byte[])(Array)packetData, 1, length - 1);
                //string s1 = new string(packetData, 1, length - 1);
                displayMessage(s1);
                return;
            }
            if (command == 222)
            {
                requestLogout();
                return;
            }
            if (command == 136)
            {
                cantLogout();
                return;
            }
            if (command == 249)
            {
                friendsCount = DataOperations.getByte(packetData[1]);
                for (int i = 0; i < friendsCount; i++)
                {
                    friendsList[i] = DataOperations.getLong(packetData, 2 + i * 9);
                    friendsWorld[i] = DataOperations.getByte(packetData[10 + i * 9]);
                }

                reOrderFriendsList();
                return;
            }
            if (command == 25)
            {
                long friend = DataOperations.getLong(packetData, 1);
                int status = packetData[9] & 0xff;
                for (int j1 = 0; j1 < friendsCount; j1++)
                {
                    if (friendsList[j1] == friend)
                    {
                        if (friendsWorld[j1] == 0 && status != 0)
                        {
                            displayMessage("@pri@" + DataOperations.hashToName(friend) + " has logged in");
                        }

                        if (friendsWorld[j1] != 0 && status == 0)
                        {
                            displayMessage("@pri@" + DataOperations.hashToName(friend) + " has logged out");
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
            if (command == 2)
            {
                ignoresCount = DataOperations.getByte(packetData[1]);
                for (int j = 0; j < ignoresCount; j++)
                {
                    ignoresList[j] = DataOperations.getLong(packetData, 2 + j * 8);
                }

                return;
            }
            if (command == 158)
            {
                blockChat = packetData[1];
                blockPrivate = packetData[2];
                blockTrade = packetData[3];
                blockDuel = packetData[4];
                return;
            }
            if (command == 170)
            {
                long l1 = DataOperations.getLong(packetData, 1);
                string s = ChatMessage.bytesToString(packetData, 9, length - 9);
                displayMessage("@pri@" + DataOperations.hashToName(l1) + ": tells you " + s);
                return;
            }
            if (command == 211)
            {// TODO remove?
                streamClass.CreatePacket(69);
                streamClass.AddInt8(0);// scar.exe, etc
                streamClass.FormatPacket();
                return;
            }
            if (command == 1)
            {// TODO remove?
                //bluePoints
                //redPoints
                return;
            }
            handlePacket(command, length, packetData);
        }

        private void reOrderFriendsList()
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
            streamClass.CreatePacket(176);
            streamClass.AddInt8(blockChat);
            streamClass.AddInt8(blockPrivate);
            streamClass.AddInt8(blockTrade);
            streamClass.AddInt8(blockDuel);
            streamClass.FormatPacket();
        }

        protected void AddIgnore(string arg0)
        {
            long l = DataOperations.nameToHash(arg0);

            streamClass.CreatePacket(25);
            streamClass.AddInt64(l);
            streamClass.FormatPacket();

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
            streamClass.CreatePacket(108);
            streamClass.AddInt64(arg0);
            streamClass.FormatPacket();

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
            streamClass.CreatePacket(168);
            streamClass.AddInt64(DataOperations.nameToHash(arg0));
            streamClass.FormatPacket();
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
            streamClass.CreatePacket(52);
            streamClass.AddInt64(arg0);
            streamClass.FormatPacket();
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

            displayMessage("@pri@" + DataOperations.hashToName(arg0) + " has been removed from your friends list");
        }

        protected void sendPrivateMessage(long l, byte[] abyte0, int i)
        {
            streamClass.CreatePacket(254);
            streamClass.AddInt64(l);
            streamClass.AddBytes(abyte0, 0, i);
            streamClass.FormatPacket();
        }

        protected void sendChatMessage(byte[] abyte0, int i)
        {
            streamClass.CreatePacket(145);
            streamClass.AddBytes(abyte0, 0, i);
            streamClass.FormatPacket();
        }

        protected void sendCommand(string s1)
        {
            streamClass.CreatePacket(90);
            streamClass.AddString(s1);
            streamClass.FormatPacket();
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

        public virtual void handlePacket(int i, int j, sbyte[] abyte0)
        {
        }

        public virtual void displayMessage(string s1)
        {
        }

        public static int maxPacketReadCount;
        public string username;
        string password;
        public StreamClass streamClass;
        public sbyte[] packetData;
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
