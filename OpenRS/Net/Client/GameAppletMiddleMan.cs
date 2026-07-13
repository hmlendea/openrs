using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Net;
using OpenRS.Net.Enumerations;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class GameAppletMiddleMan : GameApplet
    {
        public static Random random = new();
        private static bool isConnecting = false;
        private Thread connectionThread;
        public void Connect(string username, string password, bool isReconnecting)
        {
            if (isConnecting)
            {
                isConnecting = !isReconnecting;
            }

            if (socketTimeout > 0)
            {
                LoginScreenPrint("Please wait...", "Connecting to server");
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Exception) { }
                LoginScreenPrint("Sorry! The server is currently full.", "Please try again later");
                return;
            }
            try
            {
                if (isConnecting)
                {
                    return;
                }

                isConnecting = true;

                this.username = username;
                this.password = password;

                connectionThread = new Thread(new ThreadStart(DoConnect));
                connectionThread.Start();

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

        }

        private static readonly BigInteger RsaKey = BigInteger.Parse("1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335");
        private static readonly BigInteger RsaModulus = BigInteger.Parse("1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");

        private void DoConnect()
        {
            string formattedUsername = DataOperations.FormatString(username, 20);
            string formattedPassword = DataOperations.FormatString(password, 20);

            if (formattedUsername.Trim().Length == 0)
            {
                LoginScreenPrint("You must enter both a username", "and a password - Please try again");
                return;
            }
            if (reconnecting)
            {
                GameBoxPrint("Connection lost! Please wait...", "Attempting to re-establish");
            }
            else
            {
                LoginScreenPrint("Please wait...", "Connecting to server");
            }
            try
            {
                streamClass = new StreamClass(MakeSocket(Config.ServerIp, Config.ServerPort), this);
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                LoginScreenPrint("Unable to Connect.", ex.Message);
                return;
            }
            streamClass.maxPacketReadCount = maxPacketReadCount;


            long nameHash = DataOperations.NameToHash(formattedUsername);
            streamClass.CreatePacket((int)ClientPacket.SessionNameHash);
            streamClass.AddByte((int)(nameHash >> 16 & 31L));
            streamClass.AddString("Shinigami"); // TODO: Remove unused server-side string.
            streamClass.Flush();

            long sessionId;
            try
            {
                sessionId = streamClass.ReadLong();
            }
            catch (Exception ex)
            {
                LoginScreenPrint("Unable to Connect.", "Server timed out");
                Console.WriteLine(ex);
                streamClass.CloseStream();
                return;
            }

            if (sessionId == 0L)
            {
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
            loginEncryptor.AddInt(sessionKeys[0]);
            loginEncryptor.AddInt(sessionKeys[1]);
            loginEncryptor.AddInt(sessionKeys[2]);
            loginEncryptor.AddInt(sessionKeys[3]);
            loginEncryptor.AddInt(0);
            loginEncryptor.AddString(formattedUsername);
            loginEncryptor.AddString(formattedPassword);
            loginEncryptor.EncryptPacket(RsaKey, RsaModulus);
            streamClass.CreatePacket((int)ClientPacket.Login);
            if (reconnecting)
            {
                streamClass.AddByte(1);
            }
            else
            {
                streamClass.AddByte(0);
            }
            streamClass.AddShort(Config.ClientVersion);
            streamClass.AddBytes(loginEncryptor.packet, 0, loginEncryptor.offset);
            streamClass.Flush();
            int loginCode;
            try
            {
                loginCode = streamClass.Read();
            }
            catch (Exception)
            {
                LoginScreenPrint("Unable to Connect.", "Server timed out");
                streamClass.CloseStream();
                return;
            }
            Console.WriteLine("login response:" + loginCode);

            if (loginCode == (int)LoginCode.Code99)
            {
                reconnectTries = 0;
                InitVars();
                return;
            }
            if (loginCode == (int)LoginCode.Code0)
            {
                reconnectTries = 0;
                InitVars();
                return;
            }
            if (loginCode == (int)LoginCode.Code1)
            {
                reconnectTries = 0;
                return;
            }
            if (reconnecting)
            {
                username = "";
                password = "";
                ResetIntVars();
                return;
            }
            if (loginCode == (int)LoginCode.ServerTimeOut)
            {
                LoginScreenPrint("Error unable to login.", "Server timed out");
                return;
            }
            if (loginCode == (int)LoginCode.InvalidCredentials)
            {
                LoginScreenPrint("Invalid username or password.", "Try again, or create a new account");
                return;
            }
            if (loginCode == (int)LoginCode.UsernameAlreadyLoggedIn)
            {
                LoginScreenPrint("That username is already logged in.", "Wait 60 seconds then retry");
                return;
            }
            if (loginCode == (int)LoginCode.ClientUpdated)
            {
                LoginScreenPrint("The client has been updated.", "Please restart the client");
                return;
            }
            if (loginCode == (int)LoginCode.Code5)
            {
                LoginScreenPrint("Error unable to login.", "Please retry");
                return;
            }
            if (loginCode == (int)LoginCode.AccountBanned)
            {
                LoginScreenPrint("Account banned.", "Appeal on the forums, ASAP.");
                return;
            }
            if (loginCode == (int)LoginCode.ProfileDecodeFailure)
            {
                LoginScreenPrint("Error - failed to decode profile.", "Contact an admin!");
                return;
            }
            if (loginCode == (int)LoginCode.TooManyConnections)
            {
                LoginScreenPrint("Too many connections from your IP.", "Please try again later");
                return;
            }
            if (loginCode == (int)LoginCode.AccountAlreadyLoggedIn)
            {
                LoginScreenPrint("Account already in use.", "You may only login to one character at a time");
                return;
            }

            LoginScreenPrint("Error unable to login.", "Unrecognised response code");
        }

        protected void RequestLogout()
        {
            if (streamClass is not null)
            {
                try
                {
                    streamClass.CreatePacket((int)ClientPacket.RequestLogout);
                    streamClass.Flush();
                }
                catch (IOException) { }
            }

            username = "";
            password = "";
            ResetIntVars();
            LoginScreenPrint("Please enter your usename and password", "");
        }

        public virtual void LostConnection()
        {
            Console.WriteLine("Lost connection");
            // Connect(username, password, true);
            LoginScreenPrint("Please enter your usename and password", "");
        }

        protected void GameBoxPrint(string firstLine, string secondLine)
        {
        }

        protected void SendPingPacket()
        {
            long currentTime = CurrentTimeMillis();

            if (streamClass.HasData())
            {
                lastPing = currentTime;
            }

            if (currentTime - lastPing > 5000L)
            {
                lastPing = currentTime;
                streamClass.CreatePacket((int)ClientPacket.SessionNameHash);
                streamClass.FormatPacket();
            }

            try
            {
                streamClass.WritePacket(20);
            }
            catch (IOException)
            {
                LostConnection();
                return;
            }

            int packetLength = streamClass.ReadPacket(packetData);

            if (packetLength > 0)
            {
                HandlePacket(packetData[0] & 0xff, packetLength);
            }
        }

        public virtual void HandlePacket(int command, int length)
        {
            if (command == (int)ServerCommand.ServerAnnouncement)
            {
                string displayText = Encoding.UTF8.GetString((byte[])(Array)packetData, 1, length - 1);
                //string s1 = new String(packetData, 1, length - 1);
                DisplayMessage(displayText);
                return;
            }
            if (command == (int)ServerCommand.LogoutRequest)
            {
                RequestLogout();
                return;
            }
            if (command == (int)ServerCommand.LogoutCannot)
            {
                CantLogout();
                return;
            }
            if (command == (int)ServerCommand.FriendList)
            {
                friendsCount = DataOperations.GetByte((sbyte)packetData[1]);
                for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
                {
                    friendsList[friendIndex] = DataOperations.GetLong(packetData, 2 + friendIndex * 9);
                    friendsWorld[friendIndex] = DataOperations.GetByte(packetData[10 + friendIndex * 9]);
                }

                ReOrderFriendsList();

                return;
            }

            if (command == (int)ServerCommand.FriendUpdate)
            {
                long friend = DataOperations.GetLong(packetData, 1);
                int status = packetData[9] & 0xff;
                for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
                {
                    if (friendsList[friendIndex] == friend)
                    {
                        if (friendsWorld[friendIndex] == 0 && status != 0)
                        {
                            DisplayMessage("@pri@" + DataOperations.HashToName(friend) + " has logged in");
                        }

                        if (friendsWorld[friendIndex] != 0 && status == 0)
                        {
                            DisplayMessage("@pri@" + DataOperations.HashToName(friend) + " has logged out");
                        }

                        friendsWorld[friendIndex] = status;
                        length = 0;
                        ReOrderFriendsList();

                        return;
                    }
                }

                friendsList[friendsCount] = friend;
                friendsWorld[friendsCount] = status;
                friendsCount += 1;
                ReOrderFriendsList();

                return;
            }

            if (command == (int)ServerCommand.IgnoreList)
            {
                ignoresCount = DataOperations.GetByte(packetData[1]);

                for (int ignoreIndex = 0; ignoreIndex < ignoresCount; ignoreIndex += 1)
                {
                    ignoresList[ignoreIndex] = DataOperations.GetLong(packetData, 2 + ignoreIndex * 8);
                }

                return;
            }
            if (command == (int)ServerCommand.WontImplement158)
            {
                blockChat = packetData[1];
                blockPrivate = packetData[2];
                blockTrade = packetData[3];
                blockDuel = packetData[4];

                return;
            }

            if (command == (int)ServerCommand.PrivateMessage)
            {
                long senderHash = DataOperations.GetLong(packetData, 1);
                string messageText = ChatMessage.BytesToString(packetData, 9, length - 9);
                DisplayMessage("@pri@" + DataOperations.HashToName(senderHash) + ": tells you " + messageText);

                return;
            }

            if (command == (int)ServerCommand.ServerInfo)
            { // TODO: Determine if this command can be removed.
                streamClass.CreatePacket((int)ClientPacket.ClientInfoReport);
                streamClass.AddByte(0); // scar.exe, etc.
                streamClass.FormatPacket();
                return;
            }

            if (command == (int)ServerCommand.Command1)
            { // TODO: Determine if this command can be removed.
                return;
            }
            HandlePacket(command, length, packetData);
        }

        private void ReOrderFriendsList()
        {
            bool hasSwapped = true;

            while (hasSwapped)
            {
                hasSwapped = false;

                for (int friendIndex = 0; friendIndex < friendsCount - 1; friendIndex += 1)
                {
                    if (friendsWorld[friendIndex] < friendsWorld[friendIndex + 1])
                    {
                        int tempWorld = friendsWorld[friendIndex];
                        friendsWorld[friendIndex] = friendsWorld[friendIndex + 1];
                        friendsWorld[friendIndex + 1] = tempWorld;
                        long tempFriend = friendsList[friendIndex];
                        friendsList[friendIndex] = friendsList[friendIndex + 1];
                        friendsList[friendIndex + 1] = tempFriend;
                        hasSwapped = true;
                    }
                }
            }
        }

        protected void SendUpdatedPrivacyInfo(int blockChat, int blockPrivate, int blockTrade, int blockDuel)
        {
            streamClass.CreatePacket((int)ClientPacket.UpdatePrivacySettings);
            streamClass.AddByte(blockChat);
            streamClass.AddByte(blockPrivate);
            streamClass.AddByte(blockTrade);
            streamClass.AddByte(blockDuel);
            streamClass.FormatPacket();
        }

        protected void AddIgnore(string username)
        {
            long usernameHash = DataOperations.NameToHash(username);
            streamClass.CreatePacket((int)ClientPacket.AddIgnore);
            streamClass.AddLong(usernameHash);
            streamClass.FormatPacket();

            for (int ignoreIndex = 0; ignoreIndex < ignoresCount; ignoreIndex += 1)
            {
                if (ignoresList[ignoreIndex] == usernameHash)
                {
                    return;
                }
            }

            if (ignoresCount >= ignoresList.Length - 1)
            {
                return;
            }

            ignoresList[ignoresCount] = usernameHash;
            ignoresCount += 1;
        }

        protected void RemoveIgnore(long usernameHash)
        {
            streamClass.CreatePacket((int)ClientPacket.RemoveIgnore);
            streamClass.AddLong(usernameHash);
            streamClass.FormatPacket();

            for (int ignoreIndex = 0; ignoreIndex < ignoresCount; ignoreIndex += 1)
            {
                if (ignoresList[ignoreIndex] == usernameHash)
                {
                    ignoresCount -= 1;

                    for (int shiftIndex = ignoreIndex; shiftIndex < ignoresCount; shiftIndex += 1)
                    {
                        ignoresList[shiftIndex] = ignoresList[shiftIndex + 1];
                    }

                    return;
                }
            }
        }

        protected void AddFriend(string friendUsername)
        {
            streamClass.CreatePacket((int)ClientPacket.AddFriend);
            streamClass.AddLong(DataOperations.NameToHash(friendUsername));
            streamClass.FormatPacket();
            long selfHash = DataOperations.NameToHash(username);

            for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
            {
                if (friendsList[friendIndex] == selfHash)
                {
                    return;
                }
            }

            if (friendsCount >= friendsList.Length - 1)
            {
                return;
            }

            friendsList[friendsCount] = selfHash;
            friendsWorld[friendsCount] = 0;
            friendsCount += 1;
        }

        protected void RemoveFriend(long usernameHash)
        {
            streamClass.CreatePacket((int)ClientPacket.RemoveFriend);
            streamClass.AddLong(usernameHash);
            streamClass.FormatPacket();

            for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
            {
                if (friendsList[friendIndex] != usernameHash)
                {
                    continue;
                }

                friendsCount -= 1;

                for (int shiftIndex = friendIndex; shiftIndex < friendsCount; shiftIndex += 1)
                {
                    friendsList[shiftIndex] = friendsList[shiftIndex + 1];
                    friendsWorld[shiftIndex] = friendsWorld[shiftIndex + 1];
                }

                break;
            }

            DisplayMessage("@pri@" + DataOperations.HashToName(usernameHash) + " has been removed from your friends list");
        }

        protected void SendPrivateMessage(long recipientHash, byte[] messageBytes, int messageLength)
        {
            streamClass.CreatePacket((int)ClientPacket.SendPrivateMessage);
            streamClass.AddLong(recipientHash);
            streamClass.AddBytes(messageBytes, 0, messageLength);
            streamClass.FormatPacket();
        }

        protected void SendChatMessage(byte[] messageBytes, int messageLength)
        {
            streamClass.CreatePacket((int)ClientPacket.SendChatMessage);
            streamClass.AddBytes(messageBytes, 0, messageLength);
            streamClass.FormatPacket();
        }

        protected void SendCommand(string commandText)
        {
            streamClass.CreatePacket((int)ClientPacket.SendCommand);
            streamClass.AddString(commandText);
            streamClass.FormatPacket();
        }

        public virtual void LoginScreenPrint(string firstLine, string secondLine)
        {
        }

        public virtual void InitVars()
        {
        }

        public virtual void ResetIntVars()
        {
        }

        public virtual void CantLogout()
        {
        }

        public virtual void HandlePacket(int command, int length, sbyte[] data)
        {
        }

        public virtual void DisplayMessage(string message)
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
        public string username;
        private string password;
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
