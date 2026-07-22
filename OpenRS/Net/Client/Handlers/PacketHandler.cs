using System;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net;

namespace OpenRS.Net.Client.Handlers
{
    public sealed class PacketHandler(GameClient client) : IPacketHandler
    {
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<PacketHandler>();
        private readonly MovementRegionPacketHandler movementRegionPacketHandler = new(client);
        private readonly ObjectStatePacketHandler objectStatePacketHandler = new(client);
        private readonly MobUpdatePacketHandler mobUpdatePacketHandler = new(client);
        private readonly TradeShopPacketHandler tradeShopPacketHandler = new(client);
        private readonly DuelQuestionPacketHandler duelQuestionPacketHandler = new(client);
        private readonly PlayerStatePacketHandler playerStatePacketHandler = new(client);
        private readonly SystemPacketHandler systemPacketHandler = new(client);

        public void HandlePacket(int packetID, int packetLength, sbyte[] packetData)
        {
            try
            {
                ServerCommand command = (ServerCommand)packetID;

                if (TryHandlePacket(command, packetLength, packetData))
                {
                    return;
                }

                logger.Warn(
                    GameOperation.HandlePacket,
                    "Unhandled packet.",
                    new LogInfo(GameLogInfoKey.PacketId, packetID),
                    new LogInfo(GameLogInfoKey.PacketLength, packetLength));
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.HandlePacket,
                    "Packet handling has failed.",
                    exception);
            }
        }

        private bool TryHandlePacket(
            ServerCommand command,
            int packetLength,
            sbyte[] packetData)
        {
            if (movementRegionPacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (objectStatePacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (mobUpdatePacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (tradeShopPacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            if (duelQuestionPacketHandler.TryHandlePacket(command, packetData))
            {
                return true;
            }

            if (playerStatePacketHandler.TryHandlePacket(command, packetLength, packetData))
            {
                return true;
            }

            return systemPacketHandler.TryHandlePacket(command, packetLength, packetData);
        }
    }
}
