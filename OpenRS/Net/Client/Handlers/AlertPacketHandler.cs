using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class AlertPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int MaximumTeleBubbleCount => 50;
        private static int TeleBubbleTimeReset => 0;

        internal bool TryHandlePacket(
            ServerCommand command,
            int packetLength,
            sbyte[] packetData)
        {
            if (command == ServerCommand.PlaySound)
            {
                Client.PlaySound(DecodePacketString(packetData, 1, packetLength - 1));
                return true;
            }

            if (command == ServerCommand.TeleBubble)
            {
                HandleTeleBubble(packetData);
                return true;
            }

            if (command == ServerCommand.LoginScreen)
            {
                HandleLoginScreen(packetLength, packetData);
                return true;
            }

            if (command == ServerCommand.AlertSmall)
            {
                ShowServerMessage(packetLength, packetData, false);
                return true;
            }

            if (command == ServerCommand.AlertBig)
            {
                ShowServerMessage(packetLength, packetData, true);
                return true;
            }

            return false;
        }

        private void HandleTeleBubble(sbyte[] packetData)
        {
            if (Client.teleBubbleCount >= MaximumTeleBubbleCount)
            {
                return;
            }

            int type = packetData[1] & 0xff;
            int tileX = packetData[2] + Client.sectionX;
            int tileY = packetData[3] + Client.sectionY;
            Client.teleBubbleType[Client.teleBubbleCount] = type;
            Client.teleBubbleTime[Client.teleBubbleCount] = TeleBubbleTimeReset;
            Client.teleBubbleX[Client.teleBubbleCount] = tileX;
            Client.teleBubbleY[Client.teleBubbleCount] = tileY;
            Client.teleBubbleCount += 1;
        }

        private void HandleLoginScreen(int packetLength, sbyte[] packetData)
        {
            if (Client.loginScreenShown)
            {
                return;
            }

            Client.lastLoginDays = BinaryDataReader.GetShort(packetData, 1);
            Client.subDaysLeft = BinaryDataReader.GetShort(packetData, 3);
            Client.lastLoginAddress = DecodePacketString(packetData, 5, packetLength - 5);
            Client.showWelcomeBox = true;
            Client.loginScreenShown = true;
        }

        private void ShowServerMessage(
            int packetLength,
            sbyte[] packetData,
            bool showAtTop)
        {
            Client.serverMessage = DecodePacketString(packetData, 1, packetLength - 1);
            Client.showServerMessageBox = true;
            Client.serverMessageBoxTop = showAtTop;
        }
    }
}