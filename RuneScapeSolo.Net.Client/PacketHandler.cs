using System;
using System.Text;

using RuneScapeSolo.GameLogic.GameManagers;
using RuneScapeSolo.Models;
using RuneScapeSolo.Models.Enumerations;
using RuneScapeSolo.Net.Client.Data;
using RuneScapeSolo.Net.Client.Enumerations;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Primitives;

namespace RuneScapeSolo.Net.Client
{
    public class PacketHandler
    {
        readonly GameClient client;
        readonly EntityManager entityManager;
        readonly InventoryManager inventoryManager;
        readonly QuestManager questManager;

        public PacketHandler(
            GameClient client,
            EntityManager entityManager,
            InventoryManager inventoryManager,
            QuestManager questManager)
        {
            this.client = client;
            this.entityManager = entityManager;
            this.inventoryManager = inventoryManager;
            this.questManager = questManager;
        }

        public bool HandlePacket(ServerCommand command, sbyte[] data, int length)
        {
            switch (command)
            {
                case ServerCommand.Awake:
                    HandleAwake();
                    return true;

                case ServerCommand.BankItem:
                    HandleBankItem(data);
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

                case ServerCommand.Inventory:
                    HandleCommand114(data);
                    return true;

                case ServerCommand.WorldInfo:
                    HandleCommand131(data);
                    return true;

                case ServerCommand.Command145:
                    HandleCommand145(data, length);
                    return true;

                case ServerCommand.UserStats:
                    HandleCommand180(data);
                    return true;

                case ServerCommand.Command190:
                    HandleCommand190(data);
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

                case ServerCommand.EquipmentStats:
                    HandleEquipmentStatus(data);
                    return true;

                case ServerCommand.ErnestTheChicken:
                    HandleErnestTheChicken(data);
                    return true;

                case ServerCommand.FatigueChange:
                    HandleFatigueChange(data);
                    return true;

                case ServerCommand.GroundItems:
                    HandleGroundItems(data, length);
                    return true;

                case ServerCommand.GuthixSpells:
                    HandleGuthixSpells(data);
                    return true;

                case ServerCommand.CloseQuestionMenu:
                    HandleHideQuestionMenu();
                    return true;

                case ServerCommand.ImpCatcher:
                    HandleImpCatcher(data);
                    return true;

                case ServerCommand.UpdateItem:
                    HandleInventoryItems(data);
                    return true;

                case ServerCommand.KillingSpree:
                    HandleKillingSpree(data);
                    return true;

                case ServerCommand.Kills:
                    HandleKills(data);
                    return true;

                case ServerCommand.MoneyTask:
                    HandleMoneyTask(data, length);
                    return true;

                case ServerCommand.PirateTreasure:
                    HandlePirateTreasure(data);
                    return true;

                case ServerCommand.PlayerDied:
                    HandleResetPlayerAliveTimeout();
                    return true;

                case ServerCommand.Prayers:
                    HandlePrayers(data, length);
                    return true;

                case ServerCommand.QuestPointsChange:
                    HandleQuestPointsChange(data);
                    return true;

                case ServerCommand.OpenQuestionMenu:
                    HandleQuestionMenu(data);
                    return true;

                case ServerCommand.Remaining:
                    HandleRemaining(data);
                    return true;

                case ServerCommand.RemoveItem:
                    HandleRemoveItem(data);
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

                case ServerCommand.CloseBankWindow:
                    HandleShowBankBox();
                    return true;

                case ServerCommand.CloseShopWindow:
                    HandleShowShopBox();
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

                case ServerCommand.UserStat:
                    HandlePlayerStats(data);
                    return true;

                case ServerCommand.WallObjects:
                    HandleWallObjects(data, length);
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

        void HandleAwake()
        {
            client.IsSleeping = false;
        }

        void HandleBankItem(sbyte[] data)
        {
            int off = 1;
            int itemSlot = data[off++] & 0xff;
            int itemId = DataOperations.GetInt16(data, off);
            off += 2;
            int itemCount = DataOperations.GetInt32(data, off);
            off += 4;

            inventoryManager.BankItem(itemId, itemSlot, itemCount);
        }

        void HandleCombatStyleChange(sbyte[] data)
        {
            client.CombatStyle = (CombatStyle)DataOperations.GetInt8(data[1]);
        }

        void HandleCookAssistant(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("0", stage);
        }

        void HandleCommand27(sbyte[] data, int length)
        {
            for (int offset = 1; offset < length;)
            {
                if (DataOperations.GetInt8(data[offset]) == 255)
                {
                    int newCount = 0;
                    Point2D newSectionLocation = new Point2D(
                        client.SectionLocation.X + data[offset + 1] >> 3,
                        client.SectionLocation.Y + data[offset + 2] >> 3);

                    offset += 3;

                    for (int objectIndex = 0; objectIndex < client.ObjectCount; objectIndex++)
                    {
                        Point2D newLocation = new Point2D(
                            (client.ObjectLocations[objectIndex].X >> 3) - newSectionLocation.X,
                            (client.ObjectLocations[objectIndex].Y >> 3) - newSectionLocation.Y);

                        if (newLocation != Point2D.Empty)
                        {
                            if (objectIndex != newCount)
                            {
                                client.ObjectArray[newCount] = client.ObjectArray[objectIndex];
                                client.ObjectArray[newCount].index = newCount;
                                client.ObjectLocations[newCount] = client.ObjectLocations[objectIndex];
                                client.ObjectType[newCount] = client.ObjectType[objectIndex];
                                client.ObjectRotation[newCount] = client.ObjectRotation[objectIndex];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.ObjectArray[objectIndex]);

                            client.engineHandle.removeObject(
                                client.ObjectLocations[objectIndex],
                                client.ObjectType[objectIndex],
                                client.ObjectRotation[objectIndex]);
                        }
                    }

                    client.ObjectCount = newCount;
                }
                else
                {
                    int index = DataOperations.GetInt16(data, offset);
                    offset += 2;

                    Point2D newSectionLocation = new Point2D(
                        client.SectionLocation.X + data[offset++],
                        client.SectionLocation.Y + data[offset++]);

                    int rotation = data[offset];
                    int newCount = 0;
                    offset += 1;

                    for (int objectIndex = 0; objectIndex < client.ObjectCount; objectIndex++)
                    {
                        if (client.ObjectLocations[objectIndex].X != newSectionLocation.X ||
                            client.ObjectLocations[objectIndex].Y != newSectionLocation.Y ||
                            client.ObjectRotation[objectIndex] != rotation)
                        {
                            if (objectIndex != newCount)
                            {
                                client.ObjectArray[newCount] = client.ObjectArray[objectIndex];
                                client.ObjectArray[newCount].index = newCount;
                                client.ObjectLocations[newCount] = client.ObjectLocations[objectIndex];
                                client.ObjectType[newCount] = client.ObjectType[objectIndex];
                                client.ObjectRotation[newCount] = client.ObjectRotation[objectIndex];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.ObjectArray[objectIndex]);

                            client.engineHandle.removeObject(
                                client.ObjectLocations[objectIndex],
                                client.ObjectType[objectIndex],
                                client.ObjectRotation[objectIndex]);
                        }
                    }

                    client.ObjectCount = newCount;

                    if (index != 60000)
                    {
                        client.engineHandle.registerObjectDir(newSectionLocation.X, newSectionLocation.Y, rotation);

                        int width = 0;
                        int height = 0;

#warning this sometimes returns null (index = WorldObjectCount)
                        WorldObject worldObject = entityManager.GetWorldObject(index);

                        if (rotation == 0 || rotation == 4)
                        {
                            width = worldObject.Width;
                            height = worldObject.Height;
                        }
                        else
                        {
                            height = worldObject.Width;
                            width = worldObject.Height;
                        }

                        int l40 = ((newSectionLocation.X * 2 + width) * client.GridSize) / 2;
                        int k42 = ((newSectionLocation.Y * 2 + height) * client.GridSize) / 2;
                        ObjectModel gameObjectModel = client.GameDataObjects[worldObject.ModelId];
                        ObjectModel gameObject = gameObjectModel.CreateParent();

#warning object not being added to camera.
                        client.gameCamera.addModel(gameObject);

                        gameObject.index = client.ObjectCount;
                        gameObject.offsetMiniPosition(0, rotation * 32, 0);
                        gameObject.offsetPosition(l40, -client.engineHandle.getAveragedElevation(l40, k42), k42);
                        gameObject.UpdateShading(true, 48, 48, -50, -10, -50);
                        client.engineHandle.createObject(newSectionLocation.X, newSectionLocation.Y, index, rotation);

                        if (index == 74)
                        {
                            gameObject.offsetPosition(0, -480, 0);
                        }

                        client.ObjectLocations[client.ObjectCount] = newSectionLocation;
                        client.ObjectType[client.ObjectCount] = index;
                        client.ObjectRotation[client.ObjectCount] = rotation;
                        client.ObjectArray[client.ObjectCount++] = gameObject;
                    }
                }
            }
        }

        void HandleCommand53(sbyte[] data)
        {
            int mobCount = DataOperations.GetInt16(data, 1);
            int mobUpdateOffset = 3;

            for (int currentMob = 0; currentMob < mobCount; currentMob++)
            {
                int mobArrayIndex = DataOperations.GetInt16(data, mobUpdateOffset);
                mobUpdateOffset += 2;

                if (mobArrayIndex < 0 || mobArrayIndex > client.Mobs.Length)
                {
                    return;
                }

                ClientMob mob = client.Mobs[mobArrayIndex];

                if (mob == null)
                {
                    return;
                }

                byte mobUpdateType = (byte)data[mobUpdateOffset++];

                if (mobUpdateType == 0)
                {
                    int i30 = DataOperations.GetInt16(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    if (mob != null)
                    {
                        mob.PlayerSkullTimeout = 150;
                        mob.ItemAboveHeadId = i30;
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
                            client.Skills[3].CurrentLevel = hits;
                            client.Skills[3].BaseLevel = hitsBase;
                            // showServerMessageBox = false;
                        }
                    }
                }
                else if (mobUpdateType == 3)
                {
                    // Projectile an NPC.
                    int k30 = DataOperations.GetInt16(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    int k34 = DataOperations.GetInt16(data, mobUpdateOffset);
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
                    int l30 = DataOperations.GetInt16(data, mobUpdateOffset);
                    mobUpdateOffset += 2;

                    int l34 = DataOperations.GetInt16(data, mobUpdateOffset);
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
                            mob.ServerId = DataOperations.GetInt16(data, mobUpdateOffset);
                            mobUpdateOffset += 2;

                            mob.NameHash = DataOperations.GetInt16(data, mobUpdateOffset);
                            mobUpdateOffset += 8;

                            mob.Name = DataOperations.LongToString(mob.NameHash);
                            DataOperations.LongToString(DataOperations.GetInt16(data, mobUpdateOffset)); // Dummy for clan
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

                            mob.Appearance.HairColour = data[mobUpdateOffset++] & 0xff;
                            mob.Appearance.TopColour = data[mobUpdateOffset++] & 0xff;
                            mob.Appearance.TrousersColour = data[mobUpdateOffset++] & 0xff;
                            mob.Appearance.SkinColour = data[mobUpdateOffset++] & 0xff;
                            mob.CombatLevel = data[mobUpdateOffset++] & 0xff;
                            mob.PlayerSkulled = data[mobUpdateOffset++] & 0xff;
                            mob.Admin = data[mobUpdateOffset++] & 0xff;

                            string s = DataOperations.LongToString(DataOperations.GetInt16(data, mobUpdateOffset));
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
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
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
                ClientMob newNpc = client.GetLastNpc(serverIndex);
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
                        Point2D waypoint = new Point2D(
                            newNpc.Waypoints[waypointCurrent].X,
                            newNpc.Waypoints[waypointCurrent].Y);

                        if (nextSprite == 2 || nextSprite == 1 || nextSprite == 3)
                        {
                            waypoint.X += client.GridSize;
                        }

                        if (nextSprite == 6 || nextSprite == 5 || nextSprite == 7)
                        {
                            waypoint.X -= client.GridSize;
                        }

                        if (nextSprite == 4 || nextSprite == 3 || nextSprite == 5)
                        {
                            waypoint.Y += client.GridSize;
                        }

                        if (nextSprite == 0 || nextSprite == 1 || nextSprite == 7)
                        {
                            waypoint.Y -= client.GridSize;
                        }

                        newNpc.nextSprite = nextSprite;
                        newNpc.WaypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                        newNpc.Waypoints[waypointCurrent] = waypoint;
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

                int mobX = (client.SectionLocation.X + areaMobX) * client.GridSize + 64;
                int mobY = (client.SectionLocation.Y + areaMobY) * client.GridSize + 64;
                int addIndex = DataOperations.GetInt(data, newNpcOffset, 10);

                newNpcOffset += 10;

                if (addIndex >= entityManager.NpcCount)
                {
                    addIndex = 24;
                }

                client.AddNpc(mobIndex, mobX, mobY, mobSprite, addIndex);
            }
        }

        void HandleCommand114(sbyte[] data)
        {
            int off = 1;
            inventoryManager.InventoryItemsCount = data[off++] & 0xff;

            for (int item = 0; item < inventoryManager.InventoryItemsCount; item++)
            {
                int val = DataOperations.GetInt16(data, off);

                off += 2;
                inventoryManager.SetItem(item, val & 0x7fff);
                inventoryManager.SetItemEquippedStatus(item, val / 32768 == 1);

                if (entityManager.GetItem(val & 0x7fff).IsStackable == 0)
                {
                    int count = DataOperations.GetInt32(data, off);
                    inventoryManager.SetItemCount(item, count);
                    off += 4;
                }
                else
                {
                    inventoryManager.SetItemCount(item, 1);
                }
            }
        }

        void HandleCommand131(sbyte[] data)
        {
            client.ServerIndex = DataOperations.GetInt16(data, 1);

            client.WildLocation = new Point2D(
                DataOperations.GetInt16(data, 3),
                DataOperations.GetInt16(data, 5));

            client.LayerIndex = DataOperations.GetInt16(data, 7);
            client.LayerModifier = DataOperations.GetInt16(data, 9);
            
            client.WildLocation = new Point2D(
                client.WildLocation.X,
                client.WildLocation.Y - client.LayerIndex * client.LayerModifier);

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

            client.SectionLocation = new Point2D(
                DataOperations.GetInt(data, off, 11),
                DataOperations.GetInt(data, off + 11, 13));
            off += 24;

            int sprite = DataOperations.GetInt(data, off, 4);
            off += 4;

            bool sectionLoaded = client.loadSection(client.SectionLocation.X, client.SectionLocation.Y);

            client.SectionLocation = new Point2D(
                client.SectionLocation.X - client.AreaLocation.X,
                client.SectionLocation.Y - client.AreaLocation.Y);

            int mapEnterX = client.SectionLocation.X * client.GridSize + 64;
            int mapEnterY = client.SectionLocation.Y * client.GridSize + 64;

            if (sectionLoaded)
            {
                client.CurrentPlayer.WaypointCurrent = 0;
                client.CurrentPlayer.WaypointsEndSprite = 0;
                client.CurrentPlayer.Location = new Point2D(mapEnterX, mapEnterY);
                client.CurrentPlayer.Waypoints[0] = client.CurrentPlayer.Location;
            }

            client.PlayerCount = 0;
            client.CurrentPlayer = client.MakePlayer(client.ServerIndex, mapEnterX, mapEnterY, sprite);

            int newPlayerCount = DataOperations.GetInt(data, off, 8);
            off += 8;

            for (int currentNewPlayer = 0; currentNewPlayer < newPlayerCount; currentNewPlayer++)
            {
                //Mob mob = client.LastPlayers[currentNewPlayer + 1];
                ClientMob mob = client.GetLastPlayer(DataOperations.GetInt(data, off, 16));
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
                        Point2D newWaypoint = new Point2D(
                            mob.Waypoints[currentWaypoint].X,
                            mob.Waypoints[currentWaypoint].Y);

                        if (currentNextSprite == 2 || currentNextSprite == 1 || currentNextSprite == 3)
                        {
                            newWaypoint.X += client.GridSize;
                        }

                        if (currentNextSprite == 6 || currentNextSprite == 5 || currentNextSprite == 7)
                        {
                            newWaypoint.X -= client.GridSize;
                        }

                        if (currentNextSprite == 4 || currentNextSprite == 3 || currentNextSprite == 5)
                        {
                            newWaypoint.Y += client.GridSize;
                        }

                        if (currentNextSprite == 0 || currentNextSprite == 1 || currentNextSprite == 7)
                        {
                            newWaypoint.Y -= client.GridSize;
                        }

                        mob.nextSprite = currentNextSprite;
                        mob.WaypointCurrent = currentWaypoint = (currentWaypoint + 1) % 10;
                        mob.Waypoints[currentWaypoint] = newWaypoint;
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

                int mobX = (client.SectionLocation.X + areaMobX) * client.GridSize + 64;
                int mobY = (client.SectionLocation.Y + areaMobY) * client.GridSize + 64;
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
                    ClientMob dummyMob = client.Mobs[client.PlayersBufferIndexes[k40]];
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
                client.Skills[stat].CurrentLevel = DataOperations.GetInt8(data[offset++]);
            }

            for (int skillId = 0; skillId < 18; skillId++)
            {
                client.Skills[skillId].BaseLevel = DataOperations.GetInt8(data[offset++]);
            }

            for (int stat = 0; stat < 18; stat++)
            {
                client.Skills[stat].Experience = DataOperations.GetInt32(data, offset);
                offset += 4;
            }
        }

        void HandleCommand190(sbyte[] data)
        {
            int newCount = DataOperations.GetInt16(data, 1);
            int offset = 3;

            for (int i = 0; i < newCount; i++)
            {
                int npcIndex = DataOperations.GetInt16(data, offset);
                offset += 2;

                ClientMob mob = client.NpcAttackingArray[npcIndex];
                int updateType = DataOperations.GetInt8(data[offset]);
                offset++;

                if (updateType == 1)
                {
                    int playerIndex = DataOperations.GetInt16(data, offset);
                    offset += 2;

                    sbyte messageLength = data[offset];
                    offset++;

                    if (mob != null)
                    {
                        string str = ChatMessage.bytesToString(data, offset, messageLength);
                        mob.lastMessageTimeout = 150;
                        mob.lastMessage = str;

                        if (playerIndex == client.CurrentPlayer.ServerIndex)
                        {
                            // TODO: Is this retrieving the name correctly?
                            client.DisplayMessage(entityManager.GetNpc(mob.npcId).Name + ": " + mob.lastMessage);
                        }
                    }

                    offset += messageLength;
                }
                else if (updateType == 2)
                {
                    int lastDamageCount = DataOperations.GetInt8(data[offset]);
                    offset++;

                    int currentHits = DataOperations.GetInt8(data[offset]);
                    offset++;

                    int baseHits = DataOperations.GetInt8(data[offset]);
                    offset++;

                    if (mob != null)
                    {
                        mob.LastDamageCount = lastDamageCount;
                        mob.CurrentHitpoints = currentHits;
                        mob.BaseHitpoints = baseHits;
                        mob.combatTimer = 200;
                    }
                }
            }
        }

        void HandleCompletedTasks(sbyte[] data)
        {
            client.CompletedTasks = DataOperations.GetInt16(data, 1);
        }

        void HandleDeaths(sbyte[] data)
        {
            client.Deaths = DataOperations.GetInt16(data, 1);
        }

        void HandleDemonSlayer(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("12", stage);
        }

        void HandleDoricQuest(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("7", stage);
        }

        void HandleDruidicRitual(sbyte[] data)
        {
            throw new NotImplementedException();
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
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("11", stage);
        }

        void HandleFatigueChange(sbyte[] data)
        {
            client.PlayerFatigue = DataOperations.GetInt16(data, 1);
        }

        void HandleGroundItems(sbyte[] data, int length)
        {
            /*
            if (client.NeedsClear)
            {
                for (int i = 0; i < client.GroundItemId.Length; i++)
                {
                    client.GroundItemX[i] = -1;
                    client.GroundItemY[i] = -1;
                    client.GroundItemId[i] = -1;
                    client.GroundItemObjectVar[i] = -1;
                }

                client.GroundItemCount = 0;
                client.NeedsClear = false;
            }
            */

            for (int offset = 1; offset < length;)
            {
                if (DataOperations.GetInt8(data[offset]) == 255)
                {
                    int newCount = 0;
                    int newSectionX = client.SectionLocation.X + data[offset + 1] >> 3;
                    int newSectionY = client.SectionLocation.Y + data[offset + 2] >> 3;
                    offset += 3;

                    for (int groundItem = 0; groundItem < client.GroundItemCount; groundItem++)
                    {
                        int newX = (client.GroundItemLocations[groundItem].X >> 3) - newSectionX;
                        int newY = (client.GroundItemLocations[groundItem].Y >> 3) - newSectionY;

                        if (newX != 0 || newY != 0)
                        {
                            if (groundItem != newCount)
                            {
                                client.GroundItemLocations[newCount] = client.GroundItemLocations[groundItem];
                                client.GroundItemId[newCount] = client.GroundItemId[groundItem];
                                client.GroundItemObjectVar[newCount] = client.GroundItemObjectVar[groundItem];
                            }

                            newCount += 1;
                        }
                    }

                    client.GroundItemCount = newCount;
                }
                else
                {
                    int newId = DataOperations.GetInt16(data, offset);
                    offset += 2;

                    int newX = client.SectionLocation.X + data[offset++];
                    int newY = client.SectionLocation.Y + data[offset++];

                    // True if it is new, False if it is known;
                    bool itemIsNew = (newId & 0x8000) == 0;

                    if (itemIsNew)
                    {
                        client.GroundItemLocations[client.GroundItemCount] = new Point2D(newX, newY);
                        client.GroundItemId[client.GroundItemCount] = newId;
                        client.GroundItemObjectVar[client.GroundItemCount] = 0;

                        for (int objectIndex = 0; objectIndex < client.ObjectCount; objectIndex++)
                        {
                            if (client.ObjectLocations[objectIndex].X != newX ||
                                client.ObjectLocations[objectIndex].Y != newY)
                            {
                                continue;
                            }

                            client.GroundItemObjectVar[client.GroundItemCount] = entityManager.GetWorldObject(client.ObjectType[objectIndex]).GroundItemVar;
                            break;
                        }

                        client.GroundItemCount++;
                    }
                    else
                    {
                        newId &= 0x7fff;
                        int updateIndex = 0;

                        for (int groundItemIndex = 0; groundItemIndex < client.GroundItemCount; groundItemIndex++)
                        {
                            if (client.GroundItemLocations[groundItemIndex].X != newX ||
                                client.GroundItemLocations[groundItemIndex].Y != newY ||
                                client.GroundItemId[groundItemIndex] != newId)
                            {
                                if (groundItemIndex != updateIndex)
                                {
                                    client.GroundItemLocations[updateIndex] = client.GroundItemLocations[groundItemIndex];
                                    client.GroundItemId[updateIndex] = client.GroundItemId[groundItemIndex];
                                    client.GroundItemObjectVar[updateIndex] = client.GroundItemObjectVar[groundItemIndex];
                                }

                                updateIndex += 1;
                            }
                            else
                            {
                                newId = -123;
                            }
                        }

                        client.GroundItemCount = updateIndex;
                    }
                }
            }
        }

        void HandleGuthixSpells(sbyte[] data)
        {
            client.GuthixSpells = DataOperations.GetInt16(data, 1);
        }

        void HandleHideQuestionMenu()
        {
            client.ShowQuestionMenu = false;
        }

        void HandleImpCatcher(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("3", stage);
        }

        void HandleInventoryItems(sbyte[] data)
        {
            int offset = 1;
            int count = 1;

            int newCount = data[offset++] & 0xff;
            int val = DataOperations.GetInt16(data, offset);
            offset += 2;

            if (entityManager.GetItem(val & 0x7fff).IsStackable == 0)
            {
                count = DataOperations.GetInt32(data, offset);
                offset += 4;
            }

            inventoryManager.SetItem(newCount, val & 0x7fff);
            inventoryManager.SetItemCount(newCount, count);
            inventoryManager.SetItemEquippedStatus(newCount, val / 32768 == 1);

            if (newCount >= inventoryManager.InventoryItemsCount)
            {
                inventoryManager.InventoryItemsCount = newCount + 1;
            }
        }

        void HandleKillingSpree(sbyte[] data)
        {
            client.KillingSpree = DataOperations.GetInt16(data, 1);
        }

        void HandleKills(sbyte[] data)
        {
            client.Kills = DataOperations.GetInt16(data, 1);
        }

        void HandleMoneyTask(sbyte[] data, int length)
        {
            client.MoneyTask = Encoding.ASCII.GetString((byte[])(Array)data, 1, length);
        }

        void HandlePirateTreasure(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("13", stage);
        }

        void HandlePlayerStats(sbyte[] data)
        {
            int offset = 1;
            int skillId = data[offset++] & 0xff;

            client.Skills[skillId].CurrentLevel = DataOperations.GetInt8(data[offset++]);
            client.Skills[skillId].BaseLevel = DataOperations.GetInt8(data[offset++]);
            client.Skills[skillId].Experience = DataOperations.GetInt32(data, offset);
        }

        void HandlePrayers(sbyte[] data, int length)
        {
            for (int i = 0; i < length - 1; i++)
            {
                client.prayerOn[i] = data[i + 1] == 1;
            }
        }

        void HandleQuestPointsChange(sbyte[] data)
        {
            questManager.QuestPoints = DataOperations.GetInt16(data, 1);
        }

        void HandleQuestionMenu(sbyte[] data)
        {
            client.ShowQuestionMenu = true;
            client.QuestionMenuCount = DataOperations.GetInt8(data[1]);

            int offset = 2;
            for (int index = 0; index < client.QuestionMenuCount; index++)
            {
                int answerLength = DataOperations.GetInt8(data[offset]);
                offset++;

                client.questionMenuAnswer[index] = Encoding.ASCII.GetString((byte[])(Array)data, offset, answerLength);
                offset += answerLength;
            }
        }

        void HandleRemaining(sbyte[] data)
        {
            client.Remaining = DataOperations.GetInt16(data, 1);
        }

        void HandleRemoveItem(sbyte[] data)
        {
            int itemSlot = data[1] & 0xff;

            inventoryManager.RemoveItem(itemSlot);
        }

        void HandleResetPlayerAliveTimeout()
        {
            client.PlayerAliveTimeout = 250;
        }

        void HandleRomeoAndJuliet(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("5", stage);
        }

        void HandleSaradominSpells(sbyte[] data)
        {
            client.SaradominSpells = DataOperations.GetInt16(data, 1);
        }

        void HandleServerInfo(sbyte[] data, int length)
        {
            client.ServerStartTime = DataOperations.GetInt16(data, 1);
            client.ServerLocation = Encoding.ASCII.GetString((byte[])(Array)data, 9, length - 9);
        }

        void HandleSheepShearer(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("1", stage);
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

        void HandleTaskCash(sbyte[] data)
        {
            client.TaskCash = DataOperations.GetInt16(data, 1);
        }

        void HandleTaskExperience(sbyte[] data)
        {
            client.TaskExperience = DataOperations.GetInt16(data, 1);
        }

        void HandleTaskItem(sbyte[] data)
        {
            client.TaskItem = DataOperations.GetInt16(data, 1);
        }

        void HandleTaskPointsChange(sbyte[] data)
        {
            client.TaskPoints = DataOperations.GetInt16(data, 1);
        }

        void HandleTaskStatus(sbyte[] data)
        {
            client.TaskCash = DataOperations.GetInt16(data, 1);
        }

        void HandleTheRestlessGhost(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("6", stage);
        }

        void HandleTutorialChange(sbyte[] data)
        {
            client.Tutorial = DataOperations.GetInt16(data, 1);
        }

        void HandleWallObjects(sbyte[] data, int length)
        {
            for (int offset = 1; offset < length;)
            {
                if (DataOperations.GetInt8(data[offset]) == 255)
                {
                    int newCount = 0;
                    Point2D newSectionLocation = new Point2D(
                        client.SectionLocation.X + data[offset + 1] >> 3,
                        client.SectionLocation.Y + data[offset + 2] >> 3);

                    offset += 3;

                    for (int currentWallObject = 0; currentWallObject < client.WallObjectCount; currentWallObject++)
                    {
                        int newX = (client.WallObjectLocations[currentWallObject].X >> 3) - newSectionLocation.X;
                        int newY = (client.WallObjectLocations[currentWallObject].Y >> 3) - newSectionLocation.Y;

                        if (newX != 0 || newY != 0)
                        {
                            if (currentWallObject != newCount)
                            {
                                client.WallObjects[newCount] = client.WallObjects[currentWallObject];
                                client.WallObjects[newCount].index = newCount + 10000;
                                client.WallObjectLocations[newCount] = client.WallObjectLocations[currentWallObject];
                                client.WallObjectDirection[newCount] = client.WallObjectDirection[currentWallObject];
                                client.WallObjectId[newCount] = client.WallObjectId[currentWallObject];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.WallObjects[currentWallObject]);

                            client.engineHandle.removeWallObject(
                                client.WallObjectLocations[currentWallObject],
                                client.WallObjectDirection[currentWallObject],
                                client.WallObjectId[currentWallObject]);
                        }
                    }

                    client.WallObjectCount = newCount;
                }
                else
                {
                    int newId = DataOperations.GetInt16(data, offset);
                    offset += 2;

                    Point2D newSectionLocation = new Point2D(
                        client.SectionLocation.X + data[offset++],
                        client.SectionLocation.Y + data[offset++]);
                
                    sbyte direction = data[offset++];
                    int newCount = 0;

                    for (int currentWallObject = 0; currentWallObject < client.WallObjectCount; currentWallObject++)
                    {
                        if (client.WallObjectLocations[currentWallObject] != newSectionLocation ||
                            client.WallObjectDirection[currentWallObject] != direction)
                        {
                            if (currentWallObject != newCount)
                            {
                                client.WallObjects[newCount] = client.WallObjects[currentWallObject];
                                client.WallObjects[newCount].index = newCount + 10000;
                                client.WallObjectLocations[newCount] = client.WallObjectLocations[currentWallObject];
                                client.WallObjectDirection[newCount] = client.WallObjectDirection[currentWallObject];
                                client.WallObjectId[newCount] = client.WallObjectId[currentWallObject];
                            }

                            newCount += 1;
                        }
                        else
                        {
                            client.gameCamera.removeModel(client.WallObjects[currentWallObject]);

                            client.engineHandle.removeWallObject(
                                client.WallObjectLocations[currentWallObject],
                                client.WallObjectDirection[currentWallObject],
                                client.WallObjectId[currentWallObject]);
                        }
                    }

                    client.WallObjectCount = newCount;

                    if (newId != 60000)
                    {
                        client.engineHandle.createWall(newSectionLocation, direction, newId);

                        ObjectModel k35 = client.makeWallObject(newSectionLocation, direction, newId, client.WallObjectCount);
                        client.WallObjects[client.WallObjectCount] = k35;
                        client.WallObjectLocations[client.WallObjectCount] = newSectionLocation;
                        client.WallObjectId[client.WallObjectCount] = newId;
                        client.WallObjectDirection[client.WallObjectCount++] = direction;
                    }
                }
            }
        }

        void HandleWitchPotion(sbyte[] data)
        {
            int stage = DataOperations.GetInt16(data, 1);

            // TODO: Ditch numerical identifiers
            questManager.SetStage("9", stage);
        }

        void HandleZamorakSpells(sbyte[] data)
        {
            client.ZamorakSpells = DataOperations.GetInt16(data, 1);
        }
    }
}
