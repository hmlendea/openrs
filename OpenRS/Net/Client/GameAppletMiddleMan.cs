using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Net;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class GameAppletMiddleMan : GameApplet
    {
        public static Random ran = new();
        private static bool isConnecting = false;
        private Thread connectionThread;
        public void connect(String user, String pass, bool reconnecting)
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

        private static readonly BigInteger RsaKey = BigInteger.Parse("1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335");
        private static readonly BigInteger RsaModulus = BigInteger.Parse("1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");

        private void DoConnect()
        {
            //username = user;
            string formattedUsername = DataOperations.formatString(username, 20);
            // password = pass;
            string formattedPassword = DataOperations.formatString(password, 20);
            if (formattedUsername.Trim().Length.Equals(0))
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
            try
            {
                streamClass = new StreamClass(makeSocket(Config.ServerIp, Config.ServerPort), this);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                loginScreenPrint("Unable to connect.", ex.Message);
                return;
            }
            streamClass.maxPacketReadCount = maxPacketReadCount;


            long l = DataOperations.nameToHash(formattedUsername);
            streamClass.createPacket(32);
            streamClass.addByte((int)(l >> 16 & 31L));
            streamClass.addString("Shinigami");// TODO not used server-side
            streamClass.flush();

            long sessionId;
            try
            {
                sessionId = streamClass.readLong();
            }
            catch (Exception ex)
            {
                loginScreenPrint("Unable to connect.", "Server timed out");
                Console.WriteLine(ex);
                streamClass.closeStream();
                return;
            }



            if (sessionId == 0L)
            {
                //     loginScreenPrint("Login server offline.", "Please try again in a few mins");
                //     return;
            }
            Console.WriteLine("Verb: Session id: " + sessionId);
            int[] sessionKeys =
            [
                (int)(Helper.Random.NextDouble() * 99999999D),
                (int)(Helper.Random.NextDouble() * 99999999D),
                (int)(sessionId >> 32),
                (int)sessionId,
            ];
            LoginEncryptor loginEncryptor = new(new byte[500]);
            loginEncryptor.addInt(sessionKeys[0]);
            loginEncryptor.addInt(sessionKeys[1]);
            loginEncryptor.addInt(sessionKeys[2]);
            loginEncryptor.addInt(sessionKeys[3]);
            loginEncryptor.addInt(0);
            loginEncryptor.addString(formattedUsername);
            loginEncryptor.addString(formattedPassword);
            loginEncryptor.encryptPacket(RsaKey, RsaModulus);
            streamClass.createPacket(0);
            if (reconnecting)
            {
                streamClass.addByte(1);
            }
            else
            {
                streamClass.addByte(0);
            }
            streamClass.addShort(Config.ClientVersion);
            streamClass.addBytes(loginEncryptor.packet, 0, loginEncryptor.offset);
            streamClass.flush();
            int loginCode;
            try
            {
                loginCode = streamClass.read();
            }
            catch (Exception)
            {
                loginScreenPrint("Unable to connect.", "Server timed out");
                streamClass.closeStream();
                return;
            }
            Console.WriteLine("login response:" + loginCode);

           // streamClass.MakeAsync();

            if (loginCode == 99)
            {
                reconnectTries = 0;
                initVars();
                return;
            }
            if (loginCode == 0)
            {
                reconnectTries = 0;
                initVars();
                return;
            }
            if (loginCode == 1)
            {
                reconnectTries = 0;
                return;
            }
            if (reconnecting)
            {
                username = "";
                password = "";
                resetIntVars();
                return;
            }
            if (loginCode == -1)
            {
                loginScreenPrint("Error unable to login.", "Server timed out");
                return;
            }
            if (loginCode == 2)
            {
                loginScreenPrint("Invalid username or password.", "Try again, or create a new account");
                return;
            }
            if (loginCode == 3)
            {
                loginScreenPrint("That username is already logged in.", "Wait 60 seconds then retry");
                return;
            }
            if (loginCode == 4)
            {
                loginScreenPrint("The client has been updated.", "Please restart the client");
                return;
            }
            if (loginCode == 5)
            {
                loginScreenPrint("Error unable to login.", "Please retry");
                return;
            }
            if (loginCode == 6)
            {
                loginScreenPrint("Account banned.", "Appeal on the forums, ASAP.");
                return;
            }
            if (loginCode == 7)
            {
                loginScreenPrint("Error - failed to decode profile.", "Contact an admin!");
                return;
            }
            if (loginCode == 8)
            {
                loginScreenPrint("Too many connections from your IP.", "Please try again later");
                return;
            }
            if (loginCode == 9)
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
            if (streamClass is not null)
            {
                try
                {
                    streamClass.createPacket(39);
                    streamClass.flush();
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

        protected void gameBoxPrint(String s1, String s2)
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
                streamClass.createPacket(5);
                streamClass.formatPacket();
            }
            try
            {
                streamClass.writePacket(20);
            }
            catch (IOException _ex)
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
                string displayText = Encoding.UTF8.GetString((byte[])(Array)packetData, 1, length - 1);
                //String s1 = new String(packetData, 1, length - 1);
                displayMessage(displayText);
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
                friendsCount = DataOperations.getByte((sbyte)packetData[1]);
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
                friendsCount += 1;
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
                String s = ChatMessage.bytesToString(packetData, 9, length - 9);
                displayMessage("@pri@" + DataOperations.hashToName(l1) + ": tells you " + s);
                return;
            }
            if (command == 211)
            {// TODO remove?
                streamClass.createPacket(69);
                streamClass.addByte(0);// scar.exe, etc
                streamClass.formatPacket();
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
            streamClass.createPacket(176);
            streamClass.addByte(blockChat);
            streamClass.addByte(blockPrivate);
            streamClass.addByte(blockTrade);
            streamClass.addByte(blockDuel);
            streamClass.formatPacket();
        }

        protected void addIgnore(String username)
        {
            long l = DataOperations.nameToHash(username);
            streamClass.createPacket(25);
            streamClass.addLong(l);
            streamClass.formatPacket();
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
            else
            {
                ignoresList[ignoresCount++] = l;
                return;
            }
        }

        protected void removeIgnore(long usernameHash)
        {
            streamClass.createPacket(108);
            streamClass.addLong(usernameHash);
            streamClass.formatPacket();
            for (int i = 0; i < ignoresCount; i++)
            {
                if (ignoresList[i] == usernameHash)
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

        protected void addFriend(String usernameHash)
        {
            streamClass.createPacket(168);
            streamClass.addLong(DataOperations.nameToHash(usernameHash));
            streamClass.formatPacket();
            long l = DataOperations.nameToHash(username);
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
                friendsCount += 1;
                return;
            }
        }

        protected void removeFriend(long usernameHash)
        {
            streamClass.createPacket(52);
            streamClass.addLong(usernameHash);
            streamClass.formatPacket();
            for (int i = 0; i < friendsCount; i++)
            {
                if (friendsList[i] != usernameHash)
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

            displayMessage("@pri@" + DataOperations.hashToName(usernameHash) + " has been removed from your friends list");
        }

        protected void sendPrivateMessage(long l, byte[] abyte0, int i)
        {
            streamClass.createPacket(254);
            streamClass.addLong(l);
            streamClass.addBytes(abyte0, 0, i);
            streamClass.formatPacket();
        }

        protected void sendChatMessage(byte[] abyte0, int i)
        {
            streamClass.createPacket(145);
            streamClass.addBytes(abyte0, 0, i);
            streamClass.formatPacket();
        }

        protected void sendCommand(String s1)
        {
            streamClass.createPacket(90);
            streamClass.addString(s1);
            streamClass.formatPacket();
        }

        public virtual void loginScreenPrint(String s1, String s2)
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

        public virtual void displayMessage(String s1)
        {
        }

        public GameAppletMiddleMan()
        {
            username = "";
            password = "";
            packetData = new sbyte[10000];
            friendsList = new long[40];
            friendsWorld = new int[400];
            ignoresList = new long[200];
        }

        public static int maxPacketReadCount;
        public String username;
        private String password;
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
