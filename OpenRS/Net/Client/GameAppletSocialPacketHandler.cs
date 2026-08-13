using System;

using OpenRS.Localisation;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client
{
    internal static class GameAppletSocialPacketHandler
    {
        internal static bool TryHandle(
            GameAppletMiddleMan applet,
            int command,
            int length)
        {
            if (command == (int)ServerCommand.FriendList)
            {
                HandleFriendList(applet);

                return true;
            }

            if (command == (int)ServerCommand.FriendUpdate)
            {
                HandleFriendUpdate(applet);

                return true;
            }

            if (command == (int)ServerCommand.IgnoreList)
            {
                HandleIgnoreList(applet, length);

                return true;
            }

            if (command == (int)ServerCommand.WontImplement158)
            {
                HandlePrivacySettings(applet);

                return true;
            }

            if (command == (int)ServerCommand.PrivateMessage)
            {
                HandlePrivateMessage(applet, length);

                return true;
            }

            return false;
        }

        private static void HandleFriendList(GameAppletMiddleMan applet)
        {
            applet.friendsCount = BinaryDataReader.GetByte(applet.packetData[1]);

            for (int friendIndex = 0;
                friendIndex < applet.friendsCount;
                friendIndex += 1)
            {
                applet.friendsList[friendIndex] = BinaryDataReader.GetLong(
                    applet.packetData,
                    2 + friendIndex * 9);
                applet.friendsWorld[friendIndex] = BinaryDataReader.GetByte(
                    applet.packetData[10 + friendIndex * 9]);
            }

            ReorderFriendsList(applet);
        }

        private static void HandleFriendUpdate(GameAppletMiddleMan applet)
        {
            long friend = BinaryDataReader.GetLong(applet.packetData, 1);
            int status = applet.packetData[9] & 0xff;

            for (int friendIndex = 0;
                friendIndex < applet.friendsCount;
                friendIndex += 1)
            {
                if (applet.friendsList[friendIndex] == friend)
                {
                    DisplayFriendStatusChange(
                        applet,
                        friend,
                        applet.friendsWorld[friendIndex],
                        status);
                    applet.friendsWorld[friendIndex] = status;
                    ReorderFriendsList(applet);

                    return;
                }
            }

            applet.friendsList[applet.friendsCount] = friend;
            applet.friendsWorld[applet.friendsCount] = status;
            applet.friendsCount += 1;
            ReorderFriendsList(applet);
        }

        private static void DisplayFriendStatusChange(
            GameAppletMiddleMan applet,
            long friend,
            int previousStatus,
            int status)
        {
            if (previousStatus == 0 && status != 0)
            {
                applet.DisplayMessage(string.Format(
                    LocalisationManager.GetString("social.friend_logged_in"),
                    PlayerNameEncoder.HashToName(friend)));
            }

            if (previousStatus != 0 && status == 0)
            {
                applet.DisplayMessage(string.Format(
                    LocalisationManager.GetString("social.friend_logged_out"),
                    PlayerNameEncoder.HashToName(friend)));
            }
        }

        private static void HandleIgnoreList(
            GameAppletMiddleMan applet,
            int length)
        {
            applet.ignoresCount = Math.Min(
                BinaryDataReader.GetByte(applet.packetData[1]),
                Math.Min(applet.ignoresList.Length, (length - 2) / 8));

            for (int ignoreIndex = 0;
                ignoreIndex < applet.ignoresCount;
                ignoreIndex += 1)
            {
                applet.ignoresList[ignoreIndex] = BinaryDataReader.GetLong(
                    applet.packetData,
                    2 + ignoreIndex * 8);
            }
        }

        private static void HandlePrivacySettings(GameAppletMiddleMan applet)
        {
            applet.blockChat = applet.packetData[1];
            applet.blockPrivate = applet.packetData[2];
            applet.blockTrade = applet.packetData[3];
            applet.blockDuel = applet.packetData[4];
        }

        private static void HandlePrivateMessage(
            GameAppletMiddleMan applet,
            int length)
        {
            long senderHash = BinaryDataReader.GetLong(applet.packetData, 1);
            string messageText = ChatMessage.BytesToString(
                applet.packetData,
                9,
                length - 9);
            applet.DisplayMessage(string.Format(
                LocalisationManager.GetString("social.private_message_received"),
                PlayerNameEncoder.HashToName(senderHash),
                messageText));
        }

        private static void ReorderFriendsList(GameAppletMiddleMan applet)
        {
            bool hasSwapped = true;

            while (hasSwapped)
            {
                hasSwapped = false;

                for (int friendIndex = 0;
                    friendIndex < applet.friendsCount - 1;
                    friendIndex += 1)
                {
                    if (applet.friendsWorld[friendIndex] <
                        applet.friendsWorld[friendIndex + 1])
                    {
                        int temporaryWorld = applet.friendsWorld[friendIndex];
                        applet.friendsWorld[friendIndex] =
                            applet.friendsWorld[friendIndex + 1];
                        applet.friendsWorld[friendIndex + 1] = temporaryWorld;
                        long temporaryFriend = applet.friendsList[friendIndex];
                        applet.friendsList[friendIndex] =
                            applet.friendsList[friendIndex + 1];
                        applet.friendsList[friendIndex + 1] = temporaryFriend;
                        hasSwapped = true;
                    }
                }
            }
        }
    }
}