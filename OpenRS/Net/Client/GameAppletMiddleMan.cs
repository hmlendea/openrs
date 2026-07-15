using System;
using System.IO;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

using NuciLog.Core;

using OpenRS.Localisation;
using OpenRS.Logging;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Net;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class GameAppletMiddleMan : GameApplet
    {
        private static bool isConnecting;
        private Thread connectionThread;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameAppletMiddleMan>();
        public void Connect(string username, string password, bool isReconnecting)
        {
            if (isConnecting)
            {
                isConnecting = !isReconnecting;
            }

            if (socketTimeout > 0)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.please_wait"),
                    LocalisationManager.GetString("login.connecting_to_server"));
                try
                {
                    Thread.Sleep(2000);
                }
                catch (Exception) { }
                LoginScreenPrint(
                    LocalisationManager.GetString("login.server_full"),
                    LocalisationManager.GetString("login.please_try_again_later"));
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
                logger.Error(
                    GameOperation.Connect,
                    "Failed to initiate the connection.",
                    exception);
            }

        }

        private static readonly BigInteger RsaKey = BigInteger.Parse("1370158896620336158431733257575682136836100155721926632321599369132092701295540721504104229217666225601026879393318399391095704223500673696914052239029335");
        private static readonly BigInteger RsaModulus = BigInteger.Parse("1549611057746979844352781944553705273443228154042066840514290174539588436243191882510185738846985723357723362764835928526260868977814405651690121789896823");

        private void DoConnect()
        {
            string formattedUsername = PlayerNameEncoder.FormatString(username, 20);
            string formattedPassword = PlayerNameEncoder.FormatString(password, 20);

            if (formattedUsername.Trim().Length == 0)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.empty_credentials_line1"),
                    LocalisationManager.GetString("login.empty_credentials_line2"));
                return;
            }
            if (reconnecting)
            {
                GameBoxPrint(
                    LocalisationManager.GetString("login.connection_lost_line1"),
                    LocalisationManager.GetString("login.connection_lost_line2"));
            }
            else
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.please_wait"),
                    LocalisationManager.GetString("login.connecting_to_server"));
            }
            try
            {
                streamClass = new StreamClass(MakeSocket(Config.ServerIp, Config.ServerPort), this);
            }
            catch (SocketException exception)
            {
                LoginScreenPrint(LocalisationManager.GetString("login.unable_to_connect"), exception.Message);
                return;
            }
            streamClass.maxPacketReadCount = maxPacketReadCount;

            long nameHash = PlayerNameEncoder.NameToHash(formattedUsername);
            sessionNameHashByte = (int)(nameHash >> 16 & 31L);
            streamClass.CreatePacket((int)ClientPacket.SessionNameHash);
            streamClass.AddByte(sessionNameHashByte);
            streamClass.Flush();

            long sessionId;
            try
            {
                sessionId = streamClass.ReadLong();
            }
            catch (Exception ex)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.unable_to_connect"),
                    LocalisationManager.GetString("login.server_timed_out"));
                logger.Error(
                    GameOperation.Authenticate,
                    "Failed to read the session ID.",
                    ex);
                streamClass.CloseStream();
                return;
            }

            if (sessionId == 0L)
            {
            }

            logger.Debug(
                GameOperation.Authenticate,
                "Session ID received.",
                new LogInfo(GameLogInfoKey.SessionId, sessionId));
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
                LoginScreenPrint(
                    LocalisationManager.GetString("login.unable_to_connect"),
                    LocalisationManager.GetString("login.server_timed_out"));
                streamClass.CloseStream();
                return;
            }
            logger.Debug(
                GameOperation.Authenticate,
                "Login response received.",
                new LogInfo(GameLogInfoKey.LoginResponseCode, loginCode));

            if (loginCode == (int)LoginCode.LoginComplete)
            {
                reconnectTries = 0;
                lastPing = CurrentTimeMillis();
                InitVars();
                return;
            }
            if (loginCode == (int)LoginCode.LoginSuccess)
            {
                reconnectTries = 0;
                lastPing = CurrentTimeMillis();
                InitVars();
                return;
            }
            if (loginCode == (int)LoginCode.ReconnectSuccess)
            {
                reconnectTries = 0;
                lastPing = CurrentTimeMillis();
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
                LoginScreenPrint(
                    LocalisationManager.GetString("login.error_unable_to_login"),
                    LocalisationManager.GetString("login.server_timed_out"));
                return;
            }
            if (loginCode == (int)LoginCode.InvalidCredentials)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.invalid_credentials_line1"),
                    LocalisationManager.GetString("login.invalid_credentials_line2"));
                return;
            }
            if (loginCode == (int)LoginCode.UsernameAlreadyLoggedIn)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.username_already_logged_in_line1"),
                    LocalisationManager.GetString("login.username_already_logged_in_line2"));
                return;
            }
            if (loginCode == (int)LoginCode.ClientUpdated)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.client_updated_line1"),
                    LocalisationManager.GetString("login.client_updated_line2"));
                return;
            }
            if (loginCode == (int)LoginCode.SessionRejected)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.error_unable_to_login"),
                    LocalisationManager.GetString("login.session_rejected"));
                return;
            }
            if (loginCode == (int)LoginCode.AccountBanned)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.account_banned_line1"),
                    LocalisationManager.GetString("login.account_banned_line2"));
                return;
            }
            if (loginCode == (int)LoginCode.ProfileDecodeFailure)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.profile_decode_failure_line1"),
                    LocalisationManager.GetString("login.profile_decode_failure_line2"));
                return;
            }
            if (loginCode == (int)LoginCode.TooManyConnections)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.too_many_connections"),
                    LocalisationManager.GetString("login.please_try_again_later"));
                return;
            }
            if (loginCode == (int)LoginCode.AccountAlreadyLoggedIn)
            {
                LoginScreenPrint(
                    LocalisationManager.GetString("login.account_already_in_use_line1"),
                    LocalisationManager.GetString("login.account_already_in_use_line2"));
                return;
            }

            LoginScreenPrint(
                LocalisationManager.GetString("login.error_unable_to_login"),
                LocalisationManager.GetString("login.unrecognised_response_code"));
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
            LoginScreenPrint(LocalisationManager.GetString("login.enter_credentials"), "");
        }

        public virtual void LostConnection()
        {
            logger.Warn(GameOperation.LostConnection);
            // Connect(username, password, true);
            LoginScreenPrint(LocalisationManager.GetString("login.enter_credentials"), "");
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
                streamClass.CreatePacket((int)ClientPacket.Ping);
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
                friendsCount = BinaryDataReader.GetByte(packetData[1]);
                for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
                {
                    friendsList[friendIndex] = BinaryDataReader.GetLong(packetData, 2 + friendIndex * 9);
                    friendsWorld[friendIndex] = BinaryDataReader.GetByte(packetData[10 + friendIndex * 9]);
                }

                ReOrderFriendsList();

                return;
            }

            if (command == (int)ServerCommand.FriendUpdate)
            {
                long friend = BinaryDataReader.GetLong(packetData, 1);
                int status = packetData[9] & 0xff;
                for (int friendIndex = 0; friendIndex < friendsCount; friendIndex += 1)
                {
                    if (friendsList[friendIndex] == friend)
                    {
                        if (friendsWorld[friendIndex] == 0 && status != 0)
                        {
                            DisplayMessage(string.Format(LocalisationManager.GetString("social.friend_logged_in"), PlayerNameEncoder.HashToName(friend)));
                        }

                        if (friendsWorld[friendIndex] != 0 && status == 0)
                        {
                            DisplayMessage(string.Format(LocalisationManager.GetString("social.friend_logged_out"), PlayerNameEncoder.HashToName(friend)));
                        }

                        friendsWorld[friendIndex] = status;
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
                ignoresCount = Math.Min(
                    BinaryDataReader.GetByte(packetData[1]),
                    Math.Min(ignoresList.Length, (length - 2) / 8));

                for (int ignoreIndex = 0; ignoreIndex < ignoresCount; ignoreIndex += 1)
                {
                    ignoresList[ignoreIndex] = BinaryDataReader.GetLong(packetData, 2 + ignoreIndex * 8);
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
                long senderHash = BinaryDataReader.GetLong(packetData, 1);
                string messageText = ChatMessage.BytesToString(packetData, 9, length - 9);
                DisplayMessage(string.Format(LocalisationManager.GetString("social.private_message_received"), PlayerNameEncoder.HashToName(senderHash), messageText));

                return;
            }

            if (command == (int)ServerCommand.ServerInfo)
            {
                streamClass.CreatePacket((int)ClientPacket.ClientInfoReport);
                streamClass.AddByte(0); // scar.exe, etc.
                streamClass.FormatPacket();
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
            long usernameHash = PlayerNameEncoder.NameToHash(username);
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
            streamClass.AddLong(PlayerNameEncoder.NameToHash(friendUsername));
            streamClass.FormatPacket();
            long selfHash = PlayerNameEncoder.NameToHash(username);

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

            DisplayMessage(string.Format(LocalisationManager.GetString("social.friend_removed"), PlayerNameEncoder.HashToName(usernameHash)));
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
        private int sessionNameHashByte;
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
