using OpenRS.Net;
using OpenRS.Net.Client.Data;

namespace OpenRS.Net.Client.Handlers
{
    internal sealed class PlayerStatsPacketHandler(GameClient client) : PacketHandlerBase(client)
    {
        private static int HealthStatIndex => 3;
        private static int PlayerDiedTimeout => 250;
        private static int EnabledFlag => 1;
        private static string PrayerOnSound => "prayeron";
        private static string PrayerOffSound => "prayeroff";

        internal bool TryHandlePacket(ServerCommand command, sbyte[] packetData)
        {
            if (command == ServerCommand.UserStats)
            {
                HandleUserStats(packetData);
                return true;
            }

            if (command == ServerCommand.EquipmentStats)
            {
                HandleEquipmentStats(packetData);
                return true;
            }

            if (command == ServerCommand.PlayerDied)
            {
                Client.playerAliveTimeout = PlayerDiedTimeout;
                return true;
            }

            if (command == ServerCommand.GameSettings)
            {
                HandleGameSettings(packetData);
                return true;
            }

            if (command == ServerCommand.Prayers)
            {
                HandlePrayers(packetData);
                return true;
            }

            if (command == ServerCommand.SkillExperience)
            {
                HandleSkillExperience(packetData);
                return true;
            }

            if (command == ServerCommand.UserStat)
            {
                HandleUserStat(packetData);
                return true;
            }

            if (command == ServerCommand.CombatStyleChange)
            {
                Client.combatStyle = BinaryDataReader.GetByte(packetData[1]);
                return true;
            }

            return false;
        }

        private void HandleUserStats(sbyte[] packetData)
        {
            PacketReadCursor cursor = new(1);
            ReadPlayerCurrentStats(packetData, cursor);
            ReadPlayerBaseStats(packetData, cursor);
            ReadPlayerExperienceStats(packetData, cursor.Index);
        }

        private void ReadPlayerCurrentStats(sbyte[] packetData, PacketReadCursor cursor)
        {
            for (int statIndex = 0; statIndex < 18; statIndex += 1)
            {
                Client.playerStatCurrent[statIndex] =
                    PacketCursorDataReader.ReadByte(packetData, cursor);
            }
        }

        private void ReadPlayerBaseStats(sbyte[] packetData, PacketReadCursor cursor)
        {
            for (int statIndex = 0; statIndex < 18; statIndex += 1)
            {
                Client.playerStatBase[statIndex] =
                    PacketCursorDataReader.ReadByte(packetData, cursor);
            }
        }

        private void ReadPlayerExperienceStats(sbyte[] packetData, int offset)
        {
            for (int statIndex = 0; statIndex < 18; statIndex += 1)
            {
                Client.playerStatExp[statIndex] = BinaryDataReader.GetInt(packetData, offset);
                offset += 4;
            }
        }

        private void HandleEquipmentStats(sbyte[] packetData)
        {
            int offset = 1;

            for (int statIndex = 0; statIndex < 5; statIndex += 1)
            {
                Client.equipmentStatus[statIndex] =
                    BinaryDataReader.GetSignedShort(packetData, offset);
                offset += 2;
            }
        }

        private void HandleGameSettings(sbyte[] packetData)
        {
            Client.configCameraAutoAngle = BinaryDataReader.GetByte(packetData[1]) == EnabledFlag;
            Client.configOneMouseButton = BinaryDataReader.GetByte(packetData[2]) == EnabledFlag;
            Client.configSoundOff = BinaryDataReader.GetByte(packetData[3]) == EnabledFlag;
            Client.showRoofs = BinaryDataReader.GetByte(packetData[4]) == EnabledFlag;
            Client.autoScreenshot = BinaryDataReader.GetByte(packetData[5]) == EnabledFlag;
        }

        private void HandlePrayers(sbyte[] packetData)
        {
            for (int prayerIndex = 0; prayerIndex < packetData.Length - 1; prayerIndex += 1)
            {
                bool isEnabled = packetData[prayerIndex + 1] == EnabledFlag;
                PlayPrayerSoundIfRequired(prayerIndex, isEnabled);
                Client.prayerOn[prayerIndex] = isEnabled;
            }
        }

        private void PlayPrayerSoundIfRequired(int prayerIndex, bool isEnabled)
        {
            if (!Client.prayerOn[prayerIndex] && isEnabled)
            {
                Client.PlaySound(PrayerOnSound);
            }

            if (Client.prayerOn[prayerIndex] && !isEnabled)
            {
                Client.PlaySound(PrayerOffSound);
            }
        }

        private void HandleSkillExperience(sbyte[] packetData)
        {
            int statIndex = packetData[1] & 0xff;
            Client.playerStatExp[statIndex] = BinaryDataReader.GetInt(packetData, 2);
        }

        private void HandleUserStat(sbyte[] packetData)
        {
            int offset = 1;
            int statIndex = packetData[offset++] & 0xff;
            Client.playerStatCurrent[statIndex] = BinaryDataReader.GetByte(packetData[offset++]);
            Client.playerStatBase[statIndex] = BinaryDataReader.GetByte(packetData[offset++]);
            Client.playerStatExp[statIndex] = BinaryDataReader.GetInt(packetData, offset);

            if (statIndex == HealthStatIndex)
            {
                Client.showWelcomeBox = false;
                Client.showServerMessageBox = false;
            }
        }
    }
}