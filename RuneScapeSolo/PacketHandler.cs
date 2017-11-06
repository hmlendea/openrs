using System;
using System.Text;

using RuneScapeSolo.Enumerations;
using RuneScapeSolo.GameLogic.GameManagers;
using RuneScapeSolo.Lib;
using RuneScapeSolo.Lib.Data;
using RuneScapeSolo.Lib.Game;

namespace RuneScapeSolo
{
    public class PacketHandler
    {
        readonly GameClient client;

        public PacketHandler(GameClient client)
        {
            this.client = client;
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

                case ServerCommand.Command27:
                    HandleCommand27(data, length);
                    return true;

                case ServerCommand.Command53:
                    HandleCommand53(data);
                    return true;

                case ServerCommand.Command77:
                    HandleCommand77(data, length);
                    return true;

                case ServerCommand.Command114:
                    HandleCommand114(data);
                    return true;

                case ServerCommand.Command131:
                    HandleCommand131(data);
                    return true;

                case ServerCommand.Command145:
                    HandleCommand145(data, length);
                    return true;

                case ServerCommand.Command180:
                    HandleCommand180(data);
                    return true;

                case ServerCommand.CompletedTasks:
                    HandleCompletedTasks(data);
                    return true;

                case ServerCommand.Deaths:
                    HandleDeaths(data);
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

                case ServerCommand.EquipmentStatus:
                    HandleEquipmentStatus(data);
                    return true;

                case ServerCommand.ErnestTheChicken:
                    HandleErnestTheChicken(data);
                    return true;

                case ServerCommand.FatigueChange:
                    HandleFatigueChange(data);
                    return true;

                case ServerCommand.GameSettings:
                    HandleGameSettings(data);
                    return true;

                case ServerCommand.GuthixSpells:
                    HandleGuthixSpells(data);
                    return true;

                case ServerCommand.HideDuelBox:
                    HandleHideDuelBox();
                    return true;

                case ServerCommand.HideQuestionMenu:
                    HandleHideQuestionMenu();
                    return true;

                case ServerCommand.ImpCatcher:
                    HandleImpCatcher(data);
                    return true;

                case ServerCommand.InventoryItems:
                    HandleInventoryItems(data);
                    return true;

                case ServerCommand.KillingSpree:
                    HandleKillingSpree(data);
                    return true;

                case ServerCommand.Kills:
                    HandleKills(data);
                    return true;

                case ServerCommand.LoginScreen:
                    HandleLoginScreen(data, length);
                    return true;

                case ServerCommand.MoneyTask:
                    HandleMoneyTask(data, length);
                    return true;

                case ServerCommand.PirateTreasure:
                    HandlePirateTreasure(data);
                    return true;

                case ServerCommand.PlayerStats:
                    HandlePlayerStats(data, length);
                    return true;

                case ServerCommand.PvpTournamentTimer:
                    HandlePvpTournamentTimer(data);
                    return true;

                case ServerCommand.QuestPointsChange:
                    HandleQuestPointsChange(data);
                    return true;

                case ServerCommand.Remaining:
                    HandleRemaining(data);
                    return true;

                case ServerCommand.ResetPlayerAliveTimeout:
                    HandleResetPlayerAliveTimeout();
                    return true;

                case ServerCommand.RomeoAndJuliet:
                    HandleRomeoAndJuliet(data);
                    return true;

                case ServerCommand.SaradominSpells:
                    HandleSaradominSpells(data);
                    return true;

                case ServerCommand.ServerInfo:
                    HandleServerInfo(data, length);
                    return true;

                case ServerCommand.SheepShearer:
                    HandleSheepShearer(data);
                    return true;

                case ServerCommand.ShowAppearanceWindow:
                    HandleShowAppearanceWindow();
                    return true;

                case ServerCommand.ShowBankBox:
                    HandleShowBankBox();
                    return true;

                case ServerCommand.ShowShopBox:
                    HandleShowShopBox();
                    return true;

                case ServerCommand.TakeScreenshot:
                    HandleTakeScreenshot();
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

                case ServerCommand.WallObjects:
                    HandleWallObjects(data, length);
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

        public void HandleAwake()
        {
            client.IsSleeping = false;
        }

        void HandleCombatStyleChange(sbyte[] data)
        {
            client.CombatStyle = DataOperations.GetInt8(data[1]);
        }

        void HandleCookAssistant(sbyte[] data)
        {
            client.Quests.CookAssistant = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleCommand27(sbyte[] data, int length)
        {
            for (int offset = 1; offset < length;)
            {
                if (DataOperations.GetInt8(data[offset]) == 255)
                {
                    int newCount = 0;
                    int newSectionX = client.SectionX + data[offset + 1] >> 3;
                    int newSectionY = client.SectionY + data[offset + 2] >> 3;
                    offset += 3;

                    for (int obj = 0; obj < client.ObjectCount; obj++)
                    {
                        int newX = (client.ObjectX[obj] >> 3) - newSectionX;
                        int newY = (client.ObjectY[obj] >> 3) - newSectionY;

                        if (newX != 0 || newY != 0)
                        {
                            if (obj != newCount)
                            {
                                client.ObjectArray[newCount] = client.ObjectArray[obj];
                                client.ObjectArray[newCount].index = newCount;

                                client.ObjectX[newCount] = client.ObjectX[obj];
                                client.ObjectY[newCount] = client.ObjectY[obj];

                                client.ObjectType[newCount] = client.ObjectType[obj];
                                client.ObjectRotation[newCount] = client.ObjectRotation[obj];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.ObjectArray[obj]);

                            client.engineHandle.removeObject(
                                client.ObjectX[obj],
                                client.ObjectY[obj],
                                client.ObjectType[obj],
                                client.ObjectRotation[obj]);
                        }
                    }

                    client.ObjectCount = newCount;
                }
                else
                {
                    int index = DataOperations.GetUnsigned2Bytes(data, offset);
                    offset += 2;

                    int newSectionX = client.SectionX + data[offset++];
                    int newSectionY = client.SectionY + data[offset++];

                    int rotation = data[offset];
                    int newCount = 0;
                    offset += 1;

                    for (int obj = 0; obj < client.ObjectCount; obj++)
                    {
                        if (client.ObjectX[obj] != newSectionX ||
                            client.ObjectY[obj] != newSectionY ||
                            client.ObjectRotation[obj] != rotation)
                        {
                            if (obj != newCount)
                            {
                                client.ObjectArray[newCount] = client.ObjectArray[obj];
                                client.ObjectArray[newCount].index = newCount;

                                client.ObjectX[newCount] = client.ObjectX[obj];
                                client.ObjectY[newCount] = client.ObjectY[obj];

                                client.ObjectType[newCount] = client.ObjectType[obj];
                                client.ObjectRotation[newCount] = client.ObjectRotation[obj];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.ObjectArray[obj]);

                            client.engineHandle.removeObject(
                                client.ObjectX[obj],
                                client.ObjectY[obj],
                                client.ObjectType[obj],
                                client.ObjectRotation[obj]);
                        }
                    }

                    client.ObjectCount = newCount;

                    if (index != 60000)
                    {
                        client.engineHandle.registerObjectDir(newSectionX, newSectionY, rotation);

                        int width;
                        int height;

                        if (rotation == 0 || rotation == 4)
                        {
                            width = EntityManager.GetObject(index).Width;
                            height = EntityManager.GetObject(index).Height;
                        }
                        else
                        {
                            height = EntityManager.GetObject(index).Width;
                            width = EntityManager.GetObject(index).Height;
                        }

                        int l40 = ((newSectionX + newSectionX + width) * client.GridSize) / 2;
                        int k42 = ((newSectionY + newSectionY + height) * client.GridSize) / 2;
                        int model = EntityManager.GetObject(index).ModelId;
                        GameObject gameObjectModel = client.GameDataObjects[model];

                        if (gameObjectModel == null)
                        {
                            Console.WriteLine("Problem is here");
                        }

                        GameObject gameObject = gameObjectModel.CreateParent();

#warning object not being added to camera.
                        client.gameCamera.addModel(gameObject);

                        gameObject.index = client.ObjectCount;
                        gameObject.offsetMiniPosition(0, rotation * 32, 0);
                        gameObject.offsetPosition(l40, -client.engineHandle.getAveragedElevation(l40, k42), k42);
                        gameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                        client.engineHandle.createObject(newSectionX, newSectionY, index, rotation);

                        if (index == 74)
                        {
                            gameObject.offsetPosition(0, -480, 0);
                        }

                        client.ObjectX[client.ObjectCount] = newSectionX;
                        client.ObjectY[client.ObjectCount] = newSectionY;
                        client.ObjectType[client.ObjectCount] = index;
                        client.ObjectRotation[client.ObjectCount] = rotation;
                        client.ObjectArray[client.ObjectCount++] = gameObject;
                    }
                }
            }
        }

        void HandleCommand53(sbyte[] data)
        {
            int mobCount = DataOperations.GetUnsigned2Bytes(data, 1);
            int mobUpdateOffset = 3;

            for (int currentMob = 0; currentMob < mobCount; currentMob++)
            {
                int mobArrayIndex = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                mobUpdateOffset += 2;

                if (mobArrayIndex < 0 || mobArrayIndex > client.Mobs.Length)
                {
                    return;
                }

                Mob mob = client.Mobs[mobArrayIndex];

                if (mob == null)
                {
                    return;
                }

                byte mobUpdateType = (byte)data[mobUpdateOffset++];

                if (mobUpdateType == 0)
                {
                    int i30 = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    if (mob != null)
                    {
                        mob.PlayerSkullTimeout = 150;
                        mob.ItemAboveHeavId = i30;
                    }
                }
                else if (mobUpdateType == 1)
                {
                    // Player talking
                    byte byte7 = (byte)data[mobUpdateOffset++];

                    if (mob != null)
                    {
                        string s2 = DataConversions.byteToString(data, mobUpdateOffset, byte7);

                        mob.lastMessageTimeout = 150;
                        mob.lastMessage = s2;

                        if ((mob.Flag != null) && (mob.Flag != ""))
                        {
                            // TODO
                            //client.displayMessage("#f" + mob.Flag + "# " + ((mob.Clan.ToLower().Equals("null")) ? "" : "[@cya@" + mob.Clan + "@yel@] ") + mob.Name + ": " + mob.lastMessage, 2, mob.Admin);
                        }
                        else
                        {
                            // TODO
                            //client.displayMessage((mob.Clan.ToLower().Equals("null") ? "" : "[@cya@" + mob.Clan + "@yel@] ") + mob.Clan + ": " + mob.lastMessage, 2, mob.Admin);
                        }
                    }
                    mobUpdateOffset += byte7;
                }
                else if (mobUpdateType == 2)
                {
                    int lastDamageCount = DataOperations.GetInt8(data[mobUpdateOffset++]);
                    int hits = DataOperations.GetInt8(data[mobUpdateOffset++]);
                    int hitsBase = DataOperations.GetInt8(data[mobUpdateOffset++]);

                    if (mob != null)
                    {
                        mob.LastDamageCount = DataOperations.GetInt8(data[mobUpdateOffset++]); ;
                        mob.CurrentHitpoints = hits;
                        mob.BaseHitpoints = hitsBase;
                        mob.combatTimer = 200;

                        if (mob == client.CurrentPlayer)
                        {
                            client.PlayerStatCurrent[3] = hits;
                            client.PlayerStatBase[3] = hitsBase;
                            client.ShowWelcomeBox = false;
                            // showServerMessageBox = false;
                        }
                    }
                }
                else if (mobUpdateType == 3)
                {
                    // Projectile an NPC.
                    int k30 = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    int k34 = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    if (mob != null)
                    {
                        mob.ProjectileType = k30;
                        mob.AttackingNpcIndex = k34;
                        mob.AttackingPlayerIndex = -1;
                        mob.ProjectileDistance = client.ProjectileRange;
                    }
                }
                else if (mobUpdateType == 4)
                {
                    // Projectile another player.
                    int l30 = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    int l34 = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    if (mob != null)
                    {
                        mob.ProjectileType = l30;
                        mob.AttackingPlayerIndex = l34;
                        mob.AttackingNpcIndex = -1;
                        mob.ProjectileDistance = client.ProjectileRange;
                    }
                }
                else if (mobUpdateType == 5)
                {
                    // Apperance update
                    if (mob != null)
                    {
                        try
                        {
                            mob.ServerId = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                            mobUpdateOffset += 2;

                            mob.NameHash = DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset);
                            mobUpdateOffset += 8;

                            mob.Name = DataOperations.LongToString(mob.NameHash);
                            mob.Clan = DataOperations.LongToString(DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset));
                            mobUpdateOffset += 8;

                            int i31 = DataOperations.GetInt8(data[mobUpdateOffset]);
                            mobUpdateOffset++;

                            for (int i35 = 0; i35 < i31; i35++)
                            {
                                mob.AppearanceItems[i35] = DataOperations.GetInt8(data[mobUpdateOffset]);
                                mobUpdateOffset++;
                            }

                            for (int l37 = i31; l37 < 12; l37++)
                            {
                                mob.AppearanceItems[l37] = 0;
                            }

                            mob.HairColour = data[mobUpdateOffset++] & 0xff;
                            mob.TopColour = data[mobUpdateOffset++] & 0xff;
                            mob.BottomColour = data[mobUpdateOffset++] & 0xff;
                            mob.SkinColour = data[mobUpdateOffset++] & 0xff;
                            mob.CombatLevel = data[mobUpdateOffset++] & 0xff;
                            mob.PlayerSkulled = data[mobUpdateOffset++] & 0xff;
                            mob.Admin = data[mobUpdateOffset++] & 0xff;

                            string s = DataOperations.LongToString(DataOperations.GetUnsigned2Bytes(data, mobUpdateOffset));
                            mobUpdateOffset += 8;

                            if ((s != null) || (!s.Equals("--")))
                            {
                                mob.Flag = s.ToUpper();
                            }
                            else
                            {
                                mob.Flag = null;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        mobUpdateOffset += 14;
                        int j31 = DataOperations.GetInt8(data[mobUpdateOffset]);
                        mobUpdateOffset += j31 + 1;
                    }
                }
                else if (mobUpdateType == 6)
                {
                    // private player talking
                    byte byte8 = (byte)data[mobUpdateOffset];
                    mobUpdateOffset++;

                    if (mob != null)
                    {
                        string s3 = DataConversions.byteToString(data, mobUpdateOffset, byte8);
                        mob.lastMessageTimeout = 150;
                        mob.lastMessage = s3;

                        if (mob == client.CurrentPlayer)
                        {
                            // TODO
                            //client.displayMessage(mob.Name + ": " + mob.lastMessage, 5, mob.Admin);
                        }
                    }

                    mobUpdateOffset += byte8;
                }
            }
        }

        void HandleCommand77(sbyte[] data, int length)
        {
            client.LastNpcCount = client.NpcCount;
            client.NpcCount = 0;

            for (int i = 0; i < client.LastNpcCount; i++)
            {
                client.LastNpcs[i] = client.Npcs[i];
            }

            int newNpcOffset = 8;
            int newNpcCount = DataOperations.GetInt(data, newNpcOffset, 8);
            newNpcOffset += 8;

            for (int newNpcIndex = 0; newNpcIndex < newNpcCount; newNpcIndex++)
            {
                int serverIndex = DataOperations.GetInt(data, newNpcOffset, 16);
                Mob newNpc = client.GetLastNpc(serverIndex);
                newNpcOffset += 16;

                int needsUpdate = DataOperations.GetInt(data, newNpcOffset, 1);
                newNpcOffset++;

                if (needsUpdate != 0)
                {
                    int j32 = DataOperations.GetInt(data, newNpcOffset, 1);
                    newNpcOffset++;

                    if (j32 == 0)
                    {
                        int nextSprite = DataOperations.GetInt(data, newNpcOffset, 3);
                        newNpcOffset += 3;

                        int waypointCurrent = newNpc.WaypointCurrent;
                        int waypointX = newNpc.WaypointsX[waypointCurrent];
                        int waypointY = newNpc.WaypointsY[waypointCurrent];

                        if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
                        {
                            waypointX += client.GridSize;
                        }

                        if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
                        {
                            waypointX -= client.GridSize;
                        }

                        if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
                        {
                            waypointY += client.GridSize;
                        }

                        if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
                        {
                            waypointY -= client.GridSize;
                        }

                        newNpc.nextSprite = nextSprite;
                        newNpc.WaypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                        newNpc.WaypointsX[waypointCurrent] = waypointX;
                        newNpc.WaypointsY[waypointCurrent] = waypointY;
                    }
                    else
                    {
                        int nextSpriteOffset = DataOperations.GetInt(data, newNpcOffset, 4);
                        newNpcOffset += 4;

                        if ((nextSpriteOffset & 0xc) == 12)
                        {
                            continue;
                        }

                        newNpc.nextSprite = nextSpriteOffset;
                    }
                }

                client.Npcs[client.NpcCount] = newNpc;
                client.NpcCount += 1;
            }

            while (newNpcOffset + 34 < length * 8)
            {
                int mobIndex = DataOperations.GetInt(data, newNpcOffset, 16);
                newNpcOffset += 16;

                int areaMobX = DataOperations.GetInt(data, newNpcOffset, 5);
                newNpcOffset += 5;

                if (areaMobX > 15)
                {
                    areaMobX -= 32;
                }

                int areaMobY = DataOperations.GetInt(data, newNpcOffset, 5);
                newNpcOffset += 5;

                if (areaMobY > 15)
                {
                    areaMobY -= 32;
                }

                int mobSprite = DataOperations.GetInt(data, newNpcOffset, 4);
                newNpcOffset += 4;

                int mobX = (client.SectionX + areaMobX) * client.GridSize + 64;
                int mobY = (client.SectionY + areaMobY) * client.GridSize + 64;
                int addIndex = DataOperations.GetInt(data, newNpcOffset, 10);

                newNpcOffset += 10;

                if (addIndex >= EntityManager.NpcCount)
                {
                    addIndex = 24;
                }

                client.AddNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
            }
        }

        void HandleCommand114(sbyte[] data)
        {
            int off = 1;
            client.InventoryItemsCount = data[off++] & 0xff;

            for (int item = 0; item < client.InventoryItemsCount; item++)
            {
                int val = DataOperations.getShort(data, off);

                off += 2;
                client.InventoryItems[item] = val & 0x7fff;
                client.InventoryItemEquipped[item] = val / 32768;

                if (EntityManager.GetItem(val & 0x7fff).IsStackable == 0)
                {
                    client.InventoryItemCount[item] = DataOperations.getInt(data, off);
                    off += 4;
                }
                else
                {
                    client.InventoryItemCount[item] = 1;
                }
            }
        }

        void HandleCommand131(sbyte[] data)
        {
            client.ServerIndex = DataOperations.getShort(data, 1);

            client.WildX = DataOperations.getShort(data, 3);
            client.WildY = DataOperations.getShort(data, 5);

            client.LayerIndex = DataOperations.getShort(data, 7);
            client.LayerModifier = DataOperations.getShort(data, 9);

            client.WildY -= client.LayerIndex * client.LayerModifier;

            client.LoadArea = true;
            client.NeedsClear = true;
            client.HasWorldInfo = true;
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

            client.SectionX = DataOperations.GetInt(data, off, 11);
            off += 11;

            client.SectionY = DataOperations.GetInt(data, off, 13);
            off += 13;

            int sprite = DataOperations.GetInt(data, off, 4);
            off += 4;

            bool sectionLoaded = client.loadSection(client.SectionX, client.SectionY);

            client.SectionX -= client.AreaX;
            client.SectionY -= client.AreaY;

            int mapEnterX = client.SectionX * client.GridSize + 64;
            int mapEnterY = client.SectionY * client.GridSize + 64;

            if (sectionLoaded)
            {
                client.CurrentPlayer.WaypointCurrent = 0;
                client.CurrentPlayer.WaypointsEndSprite = 0;
                client.CurrentPlayer.currentX = client.CurrentPlayer.WaypointsX[0] = mapEnterX;
                client.CurrentPlayer.currentY = client.CurrentPlayer.WaypointsY[0] = mapEnterY;
            }

            client.PlayerCount = 0;
            client.CurrentPlayer = client.MakePlayer(client.ServerIndex, mapEnterX, mapEnterY, sprite);

            int newPlayerCount = DataOperations.GetInt(data, off, 8);
            off += 8;

            for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer++)
            {
                //Mob mob = client.LastPlayers[currentNewPlayer + 1];
                Mob mob = client.GetLastPlayer(DataOperations.GetInt(data, off, 16));
                off += 16;

                int playerAtTile = DataOperations.GetInt(data, off, 1);
                off++;

                if (playerAtTile != 0)
                {
                    int waypointsLeft = DataOperations.GetInt(data, off, 1);
                    off++;

                    if (waypointsLeft == 0)
                    {
                        int currentNextSprite = DataOperations.GetInt(data, off, 3);
                        off += 3;

                        int currentWaypoint = mob.WaypointCurrent;
                        int newWaypointX = mob.WaypointsX[currentWaypoint];
                        int newWaypointY = mob.WaypointsY[currentWaypoint];

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
                        mob.WaypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                        mob.WaypointsX[currentWaypoint] = newWaypointX;
                        mob.WaypointsY[currentWaypoint] = newWaypointY;
                    }
                    else
                    {
                        int needsNextSprite = DataOperations.GetInt(data, off, 4);
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
                int mobIndex = DataOperations.GetInt(data, off, 16);
                off += 16;

                int areaMobX = DataOperations.GetInt(data, off, 5);
                off += 5;

                if (areaMobX > 15)
                {
                    areaMobX -= 32;
                }

                int areaMobY = DataOperations.GetInt(data, off, 5);
                off += 5;

                if (areaMobY > 15)
                {
                    areaMobY -= 32;
                }

                int mobSprite = DataOperations.GetInt(data, off, 4);
                off += 4;

                int addIndex = DataOperations.GetInt(data, off, 1);
                off++;

                int mobX = (client.SectionX + areaMobX) * client.GridSize + 64;
                int mobY = (client.SectionY + areaMobY) * client.GridSize + 64;
                client.MakePlayer(mobIndex, mobX, mobY, mobSprite);

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
                    Mob dummyMob = client.Mobs[client.PlayersBufferIndexes[k40]];
                    client.StreamClass.AddInt16(dummyMob.ServerIndex);
                    client.StreamClass.AddInt16(dummyMob.ServerId);
                }

                client.StreamClass.FormatPacket();
                mobCount = 0;
            }
        }

        void HandleCommand180(sbyte[] data)
        {
            int offset = 1;

            for (int stat = 0; stat < 18; stat++)
            {
                client.PlayerStatCurrent[stat] = DataOperations.GetInt8(data[offset++]);
            }

            for (int stat = 0; stat < 18; stat++)
            {
                client.PlayerStatBase[stat] = DataOperations.GetInt8(data[offset++]);
            }

            for (int stat = 0; stat < 18; stat++)
            {
                client.PlayerStatExperience[stat] = DataOperations.getInt(data, offset);
                offset += 4;
            }
        }

        void HandleCompletedTasks(sbyte[] data)
        {
            client.CompletedTasks = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleDeaths(sbyte[] data)
        {
            client.Deaths = DataOperations.GetUnsigned2Bytes(data, 1);
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
            client.DropPartyTimer = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleDruidicRitual(sbyte[] data)
        {
            client.Quests.DruidicRitual = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleEquipmentStatus(sbyte[] data)
        {
            int offset = 1;

            for (int i = 0; i < 5; i++)
            {
                client.EquipmentStatus[i] = DataOperations.getShort2(data, offset);
                offset += 2;
            }
        }

        void HandleErnestTheChicken(sbyte[] data)
        {
            client.Quests.ErnestTheChicken = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleFatigueChange(sbyte[] data)
        {
            client.fatigue = DataOperations.getShort(data, 1);
        }

        void HandleGameSettings(sbyte[] data)
        {
            client.CameraAutoAngle = DataOperations.GetInt8(data[1]) == 1;
            client.OneMouseButton = DataOperations.GetInt8(data[2]) == 1;
            client.SoundOff = DataOperations.GetInt8(data[3]) == 1;
            client.ShowRoofs = DataOperations.GetInt8(data[4]) == 1;
            client.AutoScreenshot = DataOperations.GetInt8(data[5]) == 1;
            client.ShowCombatWindow = DataOperations.GetInt8(data[6]) == 1;
        }

        void HandleGuthixSpells(sbyte[] data)
        {
            client.GuthixSpells = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleHideDuelBox()
        {
            client.ShowDuelBox = false;
            client.ShowDuelConfirmBox = false;
        }

        void HandleHideQuestionMenu()
        {
            client.ShowQuestionMenu = false;
        }

        void HandleImpCatcher(sbyte[] data)
        {
            client.Quests.ImpCatcher = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleInventoryItems(sbyte[] data)
        {
            int offset = 1;
            int count = 1;

            int newCount = data[offset++] & 0xff;
            int val = DataOperations.getShort(data, offset);
            offset += 2;

            if (EntityManager.GetItem(val & 0x7fff).IsStackable == 0)
            {
                count = DataOperations.getInt(data, offset);
                offset += 4;
            }

            client.InventoryItems[newCount] = val & 0x7fff;
            client.InventoryItemEquipped[newCount] = val / 32768;
            client.InventoryItemCount[newCount] = count;

            if (newCount >= client.InventoryItemsCount)
            {
                client.InventoryItemsCount = newCount + 1;
            }
        }

        void HandleKillingSpree(sbyte[] data)
        {
            client.KillingSpree = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleKills(sbyte[] data)
        {
            client.Kills = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleLoginScreen(sbyte[] data, int length)
        {
            if (!client.LoginScreenShown)
            {
                client.LastLoginDays = DataOperations.getShort(data, 1);
                client.SubscriptionDaysLeft = DataOperations.getShort(data, 3);
                client.LastLoginAddress = Encoding.UTF8.GetString((byte[])(Array)data, 5, length - 5); // new string(data.Select(c => (char)c).ToArray(), 5, length - 5);
                client.ShowWelcomeBox = true;
                client.LoginScreenShown = true;
            }
        }

        void HandleMoneyTask(sbyte[] data, int length)
        {
            client.MoneyTask = Encoding.ASCII.GetString((byte[])(Array)data, 1, length);
        }

        void HandlePirateTreasure(sbyte[] data)
        {
            client.Quests.PirateTreasure = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandlePlayerStats(sbyte[] data, int length)
        {
            int offset = 1;
            int stat = data[offset++] & 0xff;

            client.PlayerStatCurrent[stat] = DataOperations.GetInt8(data[offset++]);
            client.PlayerStatBase[stat] = DataOperations.GetInt8(data[offset++]);
            client.PlayerStatExperience[stat] = DataOperations.getInt(data, offset);
        }

        void HandlePvpTournamentTimer(sbyte[] data)
        {
            client.PvpTournamentTimer = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleQuestPointsChange(sbyte[] data)
        {
            client.QuestPoints = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleRemaining(sbyte[] data)
        {
            client.Remaining = DataOperations.GetUnsigned2Bytes(data, 1);
        }

        void HandleResetPlayerAliveTimeout()
        {
            client.PlayerAliveTimeout = 250;
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

        void HandleShowAppearanceWindow()
        {
            client.ShowAppearanceWindow = true;
        }

        void HandleShowBankBox()
        {
            client.ShowBankBox = true;
        }

        void HandleShowShopBox()
        {
            client.ShowShopBox = true;
        }

        void HandleSystemUpdateTimer(sbyte[] data)
        {
            client.SystemUpdateTimer = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
        }

        void HandleTakeScreenshot()
        {
            client.takeScreenshot(false);
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

        void HandleWallObjects(sbyte[] data, int length)
        {
            for (int offset = 1; offset < length;)
            {
                if (DataOperations.GetInt8(data[offset]) == 255)
                {
                    int newCount = 0;
                    int newSectionX = client.SectionX + data[offset + 1] >> 3;
                    int newSectionY = client.SectionY + data[offset + 2] >> 3;

                    offset += 3;

                    for (int currentWallObject = 0; currentWallObject < client.WallObjectCount; currentWallObject++)
                    {
                        int newX = (client.WallObjectX[currentWallObject] >> 3) - newSectionX;
                        int newY = (client.WallObjectY[currentWallObject] >> 3) - newSectionY;

                        if (newX != 0 || newY != 0)
                        {
                            if (currentWallObject != newCount)
                            {
                                client.WallObjects[newCount] = client.WallObjects[currentWallObject];
                                client.WallObjects[newCount].index = newCount + 10000;

                                client.WallObjectX[newCount] = client.WallObjectX[currentWallObject];
                                client.WallObjectY[newCount] = client.WallObjectY[currentWallObject];

                                client.WallObjectDirection[newCount] = client.WallObjectDirection[currentWallObject];
                                client.WallObjectId[newCount] = client.WallObjectId[currentWallObject];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.WallObjects[currentWallObject]);

                            client.engineHandle.removeWallObject(
                                client.WallObjectX[currentWallObject],
                                client.WallObjectY[currentWallObject],
                                client.WallObjectDirection[currentWallObject],
                                client.WallObjectId[currentWallObject]);
                        }
                    }

                    client.WallObjectCount = newCount;
                }
                else
                {
                    int newId = DataOperations.getShort(data, offset);
                    offset += 2;

                    int newSectionX = client.SectionX + data[offset++];
                    int newSectionY = client.SectionY + data[offset++];

                    sbyte direction = data[offset++];
                    int newCount = 0;

                    for (int currentWallObject = 0; currentWallObject < client.WallObjectCount; currentWallObject++)
                    {
                        if (client.WallObjectX[currentWallObject] != newSectionX ||
                            client.WallObjectY[currentWallObject] != newSectionY ||
                            client.WallObjectDirection[currentWallObject] != direction)
                        {
                            if (currentWallObject != newCount)
                            {
                                client.WallObjects[newCount] = client.WallObjects[currentWallObject];
                                client.WallObjects[newCount].index = newCount + 10000;

                                client.WallObjectX[newCount] = client.WallObjectX[currentWallObject];
                                client.WallObjectY[newCount] = client.WallObjectY[currentWallObject];

                                client.WallObjectDirection[newCount] = client.WallObjectDirection[currentWallObject];
                                client.WallObjectId[newCount] = client.WallObjectId[currentWallObject];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.WallObjects[currentWallObject]);

                            client.engineHandle.removeWallObject(
                                client.WallObjectX[currentWallObject],
                                client.WallObjectY[currentWallObject],
                                client.WallObjectDirection[currentWallObject],
                                client.WallObjectId[currentWallObject]);
                        }
                    }

                    client.WallObjectCount = newCount;

                    if (newId != 60000)
                    {
                        client.engineHandle.createWall(newSectionX, newSectionY, direction, newId);

                        GameObject k35 = client.makeWallObject(newSectionX, newSectionY, direction, newId, client.WallObjectCount);
                        client.WallObjects[client.WallObjectCount] = k35;

                        client.WallObjectX[client.WallObjectCount] = newSectionX;
                        client.WallObjectY[client.WallObjectCount] = newSectionY;

                        client.WallObjectId[client.WallObjectCount] = newId;
                        client.WallObjectDirection[client.WallObjectCount++] = direction;
                    }
                }
            }
        }

        void HandleWildernessModeTimer(sbyte[] data)
        {
            client.WildernessModeTimer = DataOperations.GetUnsigned2Bytes(data, 1) * 32;
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
