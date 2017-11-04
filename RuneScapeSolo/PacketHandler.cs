using System;
using System.Text;

using RuneScapeSolo.Enumerations;
using RuneScapeSolo.Lib;
using RuneScapeSolo.Lib.Data;
using RuneScapeSolo.Lib.Game;

namespace RuneScapeSolo
{
    public class PacketHandler
    {
        mudclient client;

        public PacketHandler(mudclient client)
        {
            this.client = client;
        }

        public void HandleAwake()
        {
            client.IsSleeping = false;
        }

        public bool HandlePacket(ServerCommand command, sbyte[] data, int length)
        {
            switch (command)
            {
                case ServerCommand.Awake:
                    HandleAwake();
                    return true;

                case ServerCommand.CookAssistant:
                    HandleCookAssistant(data);
                    return true;

                case ServerCommand.CombatStyleChange:
                    HandleCombatStyleChange(data);
                    return true;

                case ServerCommand.Command145:
                    HandleCommand145(data, length);
                    return true;

                case ServerCommand.CompletedTasks:
                    HandleCompletedTasks(data);
                    return true;

                case ServerCommand.DemonsSlayer:
                    HandleDemonSlayer(data);
                    return true;

                case ServerCommand.DoricQuest:
                    HandleDoricQuest(data);
                    return true;

                case ServerCommand.DruidicRitual:
                    HandleDruidicRitual(data);
                    return true;

                case ServerCommand.DropPartyTimer:
                    HandleDropPartyTimer(data);
                    return true;

                case ServerCommand.ErnestTheChicken:
                    HandleErnestTheChicken(data);
                    return true;

                case ServerCommand.FatigueChange:
                    HandleFatigueChange(data);
                    return true;

                case ServerCommand.GuthixSpells:
                    HandleGuthixSpells(data);
                    return true;

                case ServerCommand.ImpCatcher:
                    HandleImpCatcher(data);
                    return true;

                case ServerCommand.KillingSpree:
                    HandleKillingSpree(data);
                    return true;

                case ServerCommand.MoneyTask:
                    HandleMoneyTask(data, length);
                    return true;

                case ServerCommand.PirateTreasure:
                    HandlePirateTreasure(data);
                    return true;

                case ServerCommand.PvpTournamentTimer:
                    HandlePvpTournamentTimer(data);
                    return true;

                case ServerCommand.QuestPointsChange:
                    HandleQuestPointsChange(data);
                    return true;

                case ServerCommand.RomeoAndJuliet:
                    HandleRomeoAndJuliet(data);
                    return true;

                case ServerCommand.SheepShearer:
                    HandleSheepShearer(data);
                    return true;

                case ServerCommand.SaradominSpells:
                    HandleSaradominSpells(data);
                    return true;

                case ServerCommand.ServerInfo:
                    HandleServerInfo(data, length);
                    return true;

                case ServerCommand.TaskCash:
                    HandleTaskCash(data);
                    return true;

                case ServerCommand.TaskExperience:
                    HandleTaskExperience(data);
                    return true;

                case ServerCommand.TaskItem:
                    HandleTaskItem(data);
                    return true;

                case ServerCommand.TaskPointsChange:
                    HandleTaskPointsChange(data);
                    return true;

                case ServerCommand.TaskStatus:
                    HandleTaskStatus(data);
                    return true;

                case ServerCommand.TheRuthlessGhost:
                    HandleTheRestlessGhost(data);
                    return true;

                case ServerCommand.TutorialChange:
                    HandleTutorialChange(data);
                    return true;

                case ServerCommand.WildernessModeTimer:
                    HandleWildernessModeTimer(data);
                    return true;

                case ServerCommand.WitchPotion:
                    HandleWitchPotion(data);
                    return true;

                case ServerCommand.ZamorakSpells:
                    HandleZamorakSpells(data);
                    return true;

                default:
                    return false;
            }
        }

        void HandleCombatStyleChange(sbyte[] data)
        {
            client.CombatStyle = DataOperations.getByte(data[1]);
        }

        void HandleCookAssistant(sbyte[] data)
        {
            client.Quests.CookAssistant = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleCommand145(sbyte[] data, int length)
        {
            if (!client.HasWorldInfo)
            {
                return;
            }

            client.LastPlayerCount = client.PlayerCount;
            for (int l = 0; l < client.LastPlayerCount; l++)
            {
                client.LastPlayers[l] = client.Players[l];
            }

            int off = 8;
            client.SectionX = DataOperations.getBits(data, off, 11);
            off += 11;
            client.SectionY = DataOperations.getBits(data, off, 13);
            off += 13;
            int sprite = DataOperations.getBits(data, off, 4);
            off += 4;

            bool sectionLoaded = client.loadSection(client.SectionX, client.SectionY);

            client.SectionX -= client.AreaX;
            client.SectionY -= client.AreaY;

            int mapEnterX = client.SectionX * client.GridSize + 64;
            int mapEnterY = client.SectionY * client.GridSize + 64;

            if (sectionLoaded)
            {
                client.CurrentPlayer.waypointCurrent = 0;
                client.CurrentPlayer.waypointsEndSprite = 0;
                client.CurrentPlayer.currentX = client.CurrentPlayer.waypointsX[0] = mapEnterX;
                client.CurrentPlayer.currentY = client.CurrentPlayer.waypointsY[0] = mapEnterY;
            }

            client.PlayerCount = 0;
            client.CurrentPlayer = client.makePlayer(client.ServerIndex, mapEnterX, mapEnterY, sprite);
            int newPlayerCount = DataOperations.getBits(data, off, 8);
            off += 8;

            for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer++)
            {
                //Mob mob = client.LastPlayers[currentNewPlayer + 1];
                Mob mob = client.getLastPlayer(DataOperations.getBits(data, off, 16));
                off += 16;
                int playerAtTile = DataOperations.getBits(data, off, 1);
                off++;
                if (playerAtTile != 0)
                {
                    int waypointsLeft = DataOperations.getBits(data, off, 1);
                    off++;
                    if (waypointsLeft == 0)
                    {
                        int currentNextSprite = DataOperations.getBits(data, off, 3);
                        off += 3;
                        int currentWaypoint = mob.waypointCurrent;
                        int newWaypointX = mob.waypointsX[currentWaypoint];
                        int newWaypointY = mob.waypointsY[currentWaypoint];
                        if (currentNextSprite == 2 || currentNextSprite == 1 || currentNextSprite == 3)
                        {
                            newWaypointX += client.GridSize;
                        }

                        if (currentNextSprite == 6 || currentNextSprite == 5 || currentNextSprite == 7)
                        {
                            newWaypointX -= client.GridSize;
                        }

                        if (currentNextSprite == 4 || currentNextSprite == 3 || currentNextSprite == 5)
                        {
                            newWaypointY += client.GridSize;
                        }

                        if (currentNextSprite == 0 || currentNextSprite == 1 || currentNextSprite == 7)
                        {
                            newWaypointY -= client.GridSize;
                        }

                        mob.nextSprite = currentNextSprite;
                        mob.waypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                        mob.waypointsX[currentWaypoint] = newWaypointX;
                        mob.waypointsY[currentWaypoint] = newWaypointY;
                    }
                    else
                    {
                        int needsNextSprite = DataOperations.getBits(data, off, 4);
                        off += 4;
                        if ((needsNextSprite & 0xc) == 12)
                        {
                            continue;
                        }
                        mob.nextSprite = needsNextSprite;
                    }
                }
                client.Players[client.PlayerCount++] = mob;
            }

            int mobCount = 0;

            while (off + 24 < length * 8)
            {
                int mobIndex = DataOperations.getBits(data, off, 16);
                off += 16;
                int areaMobX = DataOperations.getBits(data, off, 5);
                off += 5;
                if (areaMobX > 15)
                {
                    areaMobX -= 32;
                }

                int areaMobY = DataOperations.getBits(data, off, 5);
                off += 5;
                if (areaMobY > 15)
                {
                    areaMobY -= 32;
                }

                int mobSprite = DataOperations.getBits(data, off, 4);
                off += 4;
                int addIndex = DataOperations.getBits(data, off, 1);
                off++;
                int mobX = (client.SectionX + areaMobX) * client.GridSize + 64;
                int mobY = (client.SectionY + areaMobY) * client.GridSize + 64;
                client.makePlayer(mobIndex, mobX, mobY, mobSprite);
                if (addIndex == 0)
                {
                    client.PlayersBufferIndexes[mobCount++] = mobIndex;
                }
            }

            if (mobCount > 0)
            {
                client.StreamClass.CreatePacket(83);
                client.StreamClass.AddInt16(mobCount);
                for (int k40 = 0; k40 < mobCount; k40++)
                {
                    Mob f5 = client.PlayersBuffer[client.PlayersBufferIndexes[k40]];
                    client.StreamClass.AddInt16(f5.serverIndex);
                    client.StreamClass.AddInt16(f5.serverID);
                }

                client.StreamClass.FormatPacket();
                mobCount = 0;
            }
        }

        void HandleCompletedTasks(sbyte[] data)
        {
            client.CompletedTasks = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleDemonSlayer(sbyte[] data)
        {
            client.Quests.DemonSlayer = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleDoricQuest(sbyte[] data)
        {
            client.Quests.DoricQuest = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleDropPartyTimer(sbyte[] data)
        {
            client.DropPartyCountdown = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleDruidicRitual(sbyte[] data)
        {
            client.Quests.DruidicRitual = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleErnestTheChicken(sbyte[] data)
        {
            client.Quests.ErnestTheChicken = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleFatigueChange(sbyte[] data)
        {
            client.fatigue = DataOperations.getShort(data, 1);
        }

        void HandleGuthixSpells(sbyte[] data)
        {
            client.GuthixSpells = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleImpCatcher(sbyte[] data)
        {
            client.Quests.ImpCatcher = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleKillingSpree(sbyte[] data)
        {
            client.KillingSpree = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleMoneyTask(sbyte[] data, int length)
        {
            client.MoneyTask = Encoding.ASCII.GetString((byte[])(Array)data, 1, length);
        }

        void HandlePirateTreasure(sbyte[] data)
        {
            client.Quests.PirateTreasure = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandlePvpTournamentTimer(sbyte[] data)
        {
            client.PvpTournamentCountdown = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleQuestPointsChange(sbyte[] data)
        {
            client.QuestPoints = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleRemaining(sbyte[] data)
        {
            client.Remaining = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleRomeoAndJuliet(sbyte[] data)
        {
            client.Quests.RomeoAndJuliet = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleSaradominSpells(sbyte[] data)
        {
            client.SaradominSpells = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleServerInfo(sbyte[] data, int length)
        {
            client.ServerStartTime = DataOperations.GetUnsigned2Bytes(data, 1);
            client.ServerLocation = Encoding.ASCII.GetString((byte[])(Array)data, 9, length - 9);
        }

        void HandleSheepShearer(sbyte[] data)
        {
            client.Quests.SheepShearer = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTaskCash(sbyte[] data)
        {
            client.TaskCash = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTaskExperience(sbyte[] data)
        {
            client.TaskExperience = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTaskItem(sbyte[] data)
        {
            client.TaskItem = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTaskPointsChange(sbyte[] data)
        {
            client.TaskPoints = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTaskStatus(sbyte[] data)
        {
            client.TaskCash = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTheRestlessGhost(sbyte[] data)
        {
            client.Quests.TheRestlessGhost = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleTutorialChange(sbyte[] data)
        {
            client.Tutorial = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleWildernessModeTimer(sbyte[] data)
        {
            client.WildernessModeCountdown = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleWitchPotion(sbyte[] data)
        {
            client.Quests.WitchPotion = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleZamorakSpells(sbyte[] data)
        {
            client.ZamorakSpells = DataOperations.GetUnsigned2Bytes(data, 1);
        }
    }
}
