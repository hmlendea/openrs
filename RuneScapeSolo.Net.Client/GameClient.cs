using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using RuneScapeSolo.GameLogic.GameManagers;
using RuneScapeSolo.Infrastructure;
using RuneScapeSolo.Input;
using RuneScapeSolo.Models;
using RuneScapeSolo.Models.Enumerations;
using RuneScapeSolo.Net.Client.Data;
using RuneScapeSolo.Net.Client.Events;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Net.Client.Game.Cameras;
using RuneScapeSolo.Net.Enumerations;
using RuneScapeSolo.Primitives;
using RuneScapeSolo.Primitives.Mapping;
using RuneScapeSolo.Settings;

using ObjectModel = RuneScapeSolo.Net.Client.Game.ObjectModel;

namespace RuneScapeSolo.Net.Client
{
    public class GameClient : GameAppletMiddleMan
    {
        public event EventHandler OnContentLoadedCompleted;
        public event EventHandler<ContentLoadedEventArgs> OnContentLoaded;

        public event ChatMessageEventHandler OnChatMessageReceived;

        public static GameClient CreateGameClient(int width = 512, int height = 346)
        {
            GameClient client = new GameClient();

            client.WindowSize = new Size2D(width, height);
            client.CreateWindow(client.WindowSize.Width, client.WindowSize.Height + 11);
            client.gameMinThreadSleepTime = 10;
            return client;
        }

        readonly PacketHandler packetHandler;

        List<Keys> lastPressedKeys = new List<Keys>();
        int lastMouseX;
        int lastMouseY;
        TimeSpan timeLapse = TimeSpan.Zero;

        readonly CombatManager combatManager;
        public readonly EntityManager entityManager; // TODO: Shouldn't be public
        public readonly InventoryManager inventoryManager; // TODO: Shouldn't be public
        readonly QuestManager questManager;

        public ObjectModel[] GameDataObjects { get; set; }
        public ObjectModel[] WallObjects { get; set; }
        public ClientMob CurrentPlayer { get; set; }
        public ClientMob[] LastNpcs { get; set; }
        public ClientMob[] LastPlayers { get; set; }
        public ClientMob[] Mobs { get; set; }
        public ClientMob[] Npcs { get; set; }
        public ClientMob[] NpcAttackingArray { get; set; }
        public ClientMob[] Players { get; set; }
        public Point2D AreaLocation { get; set; }
        public Point2D SectionLocation { get; set; }
        public Point2D WildLocation { get; set; }
        public Point2D[] GroundItemLocations { get; set; }
        public Point2D[] MenuActionLocations { get; set; }
        public Point2D[] ObjectLocations { get; set; }
        public Point2D[] WalkArrayLocations { get; set; }
        public Point2D[] WallObjectLocations { get; set; }
        public Size2D WindowSize { get; set; }
        public Skill[] Skills { get; set; }
        public CombatStyle CombatStyle { get; set; }
        public int CompletedTasks { get; set; }
        public int Deaths { get; set; }
        public int GridSize { get; set; }
        public int GroundItemCount { get; set; }
        public int GuthixSpells { get; set; }
        public int Tutorial { get; set; }
        public int TaskCash { get; set; }
        public int TaskExperience { get; set; }
        public int TaskItem { get; set; }
        public int TaskPoints { get; set; }
        public int TaskStatus { get; set; }
        public int KillingSpree { get; set; }
        public int Kills { get; set; }
        public int LastLoginDays { get; set; }
        public int LastNpcCount { get; set; }
        public int LastPlayerCount { get; set; }
        public int LayerIndex { get; set; }
        public int LayerModifier { get; set; }
        public int NpcCount { get; set; }
        public int ObjectCount { get; set; }
        public int PlayerAliveTimeout { get; set; }
        public int PlayerCount { get; set; }
        public int PlayerFatigue { get; set; }
        public int ProjectileRange { get; set; }
        public int QuestionMenuCount { get; set; }
        public int Remaining { get; set; }
        public int SaradominSpells { get; set; }
        public int ServerStartTime { get; set; }
        public int ServerIndex { get; set; }
        public int SubscriptionDaysLeft { get; set; }
        public int WallObjectCount { get; set; }
        public int ZamorakSpells { get; set; }
        public int[] EquipmentStatus { get; set; }
        public int[] GroundItemId { get; set; }
        public int[] GroundItemObjectVar { get; set; }
        public int[] ObjectRotation { get; set; }
        public int[] ObjectType { get; set; }
        public int[] PlayersBufferIndexes { get; set; }
        public int[] WallObjectDirection { get; set; }
        public int[] WallObjectId { get; set; }
        public bool HasWorldInfo { get; set; }
        public bool LoadArea { get; set; } // Not in wilderness
        public bool LoginScreenShown { get; set; }
        public bool NeedsClear { get; set; }
        public bool ShowAppearanceWindow { get; set; }
        public bool ShowBankBox { get; set; }
        public bool ShowQuestionMenu { get; set; }
        public bool ShowShopBox { get; set; }
        public bool[] WallObjectAlreadyInMenu { get; set; }
        public string LastLoginAddress { get; set; }
        public string MoneyTask { get; set; }
        public string ServerLocation { get; set; }

        public char TranslateOemKeys(Keys k)
        {
            if (k == Keys.OemPeriod)
            {
                return '.';
            }
            else if (InputManager.Instance.IsAnyKeyDown(Keys.LeftShift, Keys.RightShift))
            {
                if (k == Keys.NumPad1 || k == Keys.D1)
                {
                    return '!';
                }
                else if (k == Keys.NumPad2 || k == Keys.D2)
                {
                    return '"';
                }
                else if (k == Keys.NumPad3 || k == Keys.D3)
                {
                    return '#';
                }
                else if (k == Keys.NumPad4 || k == Keys.D4)
                {
                    return '¤';
                }
                else if (k == Keys.NumPad5 || k == Keys.D5)
                {
                    return '%';
                }
                else if (k == Keys.NumPad6 || k == Keys.D6)
                {
                    return '&';
                }
                else if (k == Keys.NumPad7 || k == Keys.D7)
                {
                    return '/';
                }
                else if (k == Keys.NumPad8 || k == Keys.D8)
                {
                    return '(';
                }
                else if (k == Keys.NumPad9 || k == Keys.D9)
                {
                    return ')';
                }
                else if (k == Keys.NumPad0 || k == Keys.D0)
                {
                    return '=';
                }
                else if (k == Keys.OemPlus)
                {
                    return '?';
                }

                return (char)k;
            }
            else if (InputManager.Instance.IsAnyKeyDown(Keys.LeftAlt, Keys.RightAlt) &&
                     InputManager.Instance.IsAnyKeyDown(Keys.LeftControl, Keys.RightControl))
            {
                if (k == Keys.NumPad2 || k == Keys.D2)
                {
                    return '@';
                }
                else if (k == Keys.NumPad3 || k == Keys.D3)
                {
                    return '£';
                }
                else if (k == Keys.NumPad4 || k == Keys.D4)
                {
                    return '$';
                }
                else if (k == Keys.NumPad7 || k == Keys.D7)
                {
                    return '{';
                }
                else if (k == Keys.NumPad8 || k == Keys.D8)
                {
                    return '[';
                }
                else if (k == Keys.NumPad9 || k == Keys.D9)
                {
                    return ']';
                }
                else if (k == Keys.NumPad0 || k == Keys.D0)
                {
                    return '}';
                }
                else if (k == Keys.OemPlus)
                {
                    return '\\';
                }
            }
            else
            {
                return ((char)k + "").ToLower()[0];
            }
            return (char)k;
        }

        public GameClient()
        {
            entityManager = new EntityManager();
            inventoryManager = new InventoryManager(entityManager);
            combatManager = new CombatManager(inventoryManager);
            questManager = new QuestManager();

            packetHandler = new PacketHandler(this, entityManager, inventoryManager, questManager);

            WindowSize = new Size2D(512, 334);

            Skills = new Skill[18];

            Skills[0] = new Skill { Name = "Attack" };
            Skills[1] = new Skill { Name = "Defence" };
            Skills[2] = new Skill { Name = "Strength" };
            Skills[3] = new Skill { Name = "Health" };
            Skills[4] = new Skill { Name = "Ranged" };
            Skills[5] = new Skill { Name = "Prayer" };
            Skills[6] = new Skill { Name = "Magic" };
            Skills[7] = new Skill { Name = "Cooking" };
            Skills[8] = new Skill { Name = "Woodcutting" };
            Skills[9] = new Skill { Name = "Fletching" };
            Skills[10] = new Skill { Name = "Fishing" };
            Skills[11] = new Skill { Name = "Firemaking" };
            Skills[12] = new Skill { Name = "Crafting" };
            Skills[13] = new Skill { Name = "Smithing" };
            Skills[14] = new Skill { Name = "Mining" };
            Skills[15] = new Skill { Name = "Herblore" };
            Skills[16] = new Skill { Name = "Agility" };
            Skills[17] = new Skill { Name = "Thieving" };

            MenuActionLocations = new Point2D[250];
            GroundItemLocations = new Point2D[5000];
            ObjectLocations = new Point2D[1500];
            WalkArrayLocations = new Point2D[8000];
            WallObjectLocations = new Point2D[500];

            cameraFieldOfView = 9;
            ShowQuestionMenu = false;
            LoginScreenShown = false;
            questionMenuAnswer = new string[10];
            appearanceBodyGender = 1;
            appearance2Colour = 2;
            appearanceHairColour = 2;
            appearanceTopColour = 8;
            appearanceBottomColour = 14;
            appearanceHeadGender = 1;
            menuIndexes = new int[250];
            Players = new ClientMob[500];
            selectedShopItemIndex = -1;
            selectedShopItemType = -2;
            menuText1 = new string[250];
            IsSleeping = false;
            itemAboveHeadScale = new int[50];
            itemAboveHeadID = new int[50];
            menuActions = new MenuAction[250];
            Npcs = new ClientMob[500];
            Mobs = new ClientMob[4000];
            serverMessage = "";
            serverMessageBoxTop = false;
            cameraRotationYIncrement = 2;
            WallObjects = new ObjectModel[500];
            messagesArray = new string[5];
            objectAlreadyInMenu = new bool[1500];
            ObjectArray = new ObjectModel[1500];
            selectedSpell = -1;
            cameraAutoAngleDebug = false;
            CurrentPlayer = new ClientMob();
            ServerIndex = -1;
            menuActionType = new int[250];
            menuActionVar1 = new int[250];
            menuActionVar2 = new int[250];
            sleepWordDelay = true;
            cameraRotation = 128;
            menuShow = false;
            ShowBankBox = false;
            ShowShopBox = false;
            GroundItemId = new int[5000];
            GroundItemObjectVar = new int[5000];
            LayerIndex = -1;
            cameraDistance = 550;
            receivedMessageX = new int[50];
            receivedMessageY = new int[50];
            receivedMessageMidPoint = new int[50];
            receivedMessageHeight = new int[50];
            WallObjectAlreadyInMenu = new bool[500];
            lastLayerIndex = -1;
            errorLoading = false;
            itemAboveHeadX = new int[50];
            itemAboveHeadY = new int[50];
            PlayersBufferIndexes = new int[500];
            selectedBankItem = -1;
            selectedBankItemType = -2;
            WallObjectDirection = new int[500];
            WallObjectId = new int[500];
            GameDataObjects = new ObjectModel[1000];
            LastNpcs = new ClientMob[500];
            selectedItem = -1;
            selectedItemName = "";
            LastPlayers = new ClientMob[500];
            mouseTrailX = new int[8192];
            mouseTrailY = new int[8192];
            prayerOn = new bool[50];
            shopItems = new int[256];
            shopItemCount = new int[256];
            shopItemBasePriceModifier = new int[256];
            EquipmentStatus = new int[5];
            receivedMessages = new string[50];
            cameraRotationXIncrement = 2;
            teleBubbleTime = new int[50];
            GridSize = 128;
            teleBubbleType = new int[50];
            experienceList = new int[99];
            lastModelFireLightningSpellNumber = -1;
            lastModelTorchNumber = -1;
            lastModelClawSpellNumber = -1;
            messagesTimeout = new int[5];
            ProjectileRange = 40;
            memoryError = false;
            menuText2 = new string[250];
            loginUsername = "";
            loginPassword = "";
            healthBarX = new int[50];
            healthBarY = new int[50];
            healthBarMissing = new int[50];
            ObjectType = new int[1500];
            ObjectRotation = new int[1500];
            NpcAttackingArray = new ClientMob[5000];
            teleBubbleY = new int[50];
            cameraAutoAngle = 1;
            LoadArea = false;
            teleBubbleX = new int[50];
            ShowAppearanceWindow = false;
            cameraZoom = false;

            SubscriptionDaysLeft = 0;
            shopItemSellPrice = new int[256];
            shopItemBuyPrice = new int[256];
            captchaPixels = new int[0][];
            captchaWidth = 0;
            captchaHeight = 0;
            NeedsClear = false;
            HasWorldInfo = false;

            LoadContent(); // TODO: Call this from outside
        }

        public void LoadContent()
        {
            entityManager.LoadContent();
            inventoryManager.LoadContent();
        }

        public override void UnloadContent()
        {
            try
            {
                if (gameGraphics != null)
                {
                    gameGraphics.UnloadContent();
                    gameGraphics.pixels = null;
                    gameGraphics = null;
                }

                if (gameCamera != null)
                {
                    gameCamera.cleanUp();
                    gameCamera = null;
                }

                GameDataObjects = null;
                ObjectArray = null;
                WallObjects = null;
                Mobs = null;
                Players = null;
                NpcAttackingArray = null;
                Npcs = null;
                CurrentPlayer = null;

                if (engineHandle != null)
                {
                    engineHandle.TileChunks = null;
                    engineHandle.wallObject = null;
                    engineHandle.roofObject = null;
                    engineHandle.currentSectionObject = null;
                    engineHandle = null;
                }

                GC.Collect();

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex.Message);

                return;
            }
        }

        public void Update(GameTime gameTime)
        {
            var lastUpdate = gameTime.ElapsedGameTime;

            var keyboardState = Keyboard.GetState();

            var mouseState = Mouse.GetState();
            List<Keys> keysPressedDown = new List<Keys>();
            keysPressedDown.AddRange(keyboardState.GetPressedKeys());

            foreach (var k in keysPressedDown)
            {
                if (!lastPressedKeys.Contains(k))
                {
                    KeyDown(k, TranslateOemKeys(k));
                    timeLapse = TimeSpan.Zero;
                }
                else if (timeLapse > TimeSpan.FromMilliseconds(150))
                {
                    KeyDown(k, TranslateOemKeys(k));
                    timeLapse = TimeSpan.Zero;
                }
            }

            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(keyboardState.GetPressedKeys());

            timeLapse += lastUpdate;

            if (mouseState.X != lastMouseX || mouseState.Y != lastMouseY)
            {
                MouseMove();

                lastMouseX = mouseState.X;
                lastMouseY = mouseState.Y;
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                mouseDown(mouseState.X, mouseState.Y, mouseState.LeftButton == ButtonState.Pressed);
                MousePressed(mouseState);
            }

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                mouseDown(mouseState.X, mouseState.Y, mouseState.LeftButton != ButtonState.Pressed);
                MousePressed(mouseState);
            }

            if (mouseState.RightButton == ButtonState.Released)
            {
                MouseUp();
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                MouseUp();
            }
        }

        public void menuClick(int actionId)
        {
            int actionType = menuActionType[actionId];
            int actionVar1 = menuActionVar1[actionId];
            int actionVar2 = menuActionVar2[actionId];

            MenuAction action = menuActions[actionId];

            if (action == MenuAction.CastSpellOnGroundItem)
            {
                walkToGroundItem(SectionLocation, MenuActionLocations[actionId], true);
                StreamClass.CreatePacket(104);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithGroundItem)
            {
                walkToGroundItem(SectionLocation, MenuActionLocations[actionId], true);
                StreamClass.CreatePacket(34);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.TakeItem)
            {
                walkToGroundItem(SectionLocation, MenuActionLocations[actionId], true);
                StreamClass.CreatePacket(245);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.ExamineGroundItem)
            {
                DisplayMessage(entityManager.GetItem(actionType).Description);
            }

            if (action == MenuAction.CastSpellOnWallObject)
            {
                walkToWallObject(MenuActionLocations[actionId], actionType);
                StreamClass.CreatePacket(67);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithWallObject)
            {
                walkToWallObject(MenuActionLocations[actionId], actionType);
                StreamClass.CreatePacket(36);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt8(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.Command1OnWallObject)
            {
                walkToWallObject(MenuActionLocations[actionId], actionType);
                StreamClass.CreatePacket(126);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.Command2OnWallObject)
            {
                walkToWallObject(MenuActionLocations[actionId], actionType);
                StreamClass.CreatePacket(235);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.ExamineWallObject)
            {
                DisplayMessage(entityManager.GetWallObject(actionType).Description);
            }

            if (action == MenuAction.CastSpellOnModel)
            {
                walkToObject(MenuActionLocations[actionId], actionType, actionVar1);
                StreamClass.CreatePacket(17);
                StreamClass.AddInt16(actionVar2);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);

                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithModel)
            {
                walkToObject(MenuActionLocations[actionId], actionType, actionVar1);
                StreamClass.CreatePacket(94);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.AddInt16(actionVar2);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.Command1OnModel)
            {
                walkToObject(MenuActionLocations[actionId], actionType, actionVar1);
                StreamClass.CreatePacket(51);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.Command2OnModel)
            {
                walkToObject(MenuActionLocations[actionId], actionType, actionVar1);
                StreamClass.CreatePacket(40);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.ExamineModel)
            {
                DisplayMessage(entityManager.GetWorldObject(actionType).Description);
            }

            if (action == MenuAction.CastSpellOnItem)
            {
                StreamClass.CreatePacket(49);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithItem)
            {
                StreamClass.CreatePacket(27);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.RemoveItem)
            {
                StreamClass.CreatePacket(92);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.EquipItem)
            {
                StreamClass.CreatePacket(181);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.CommandOnItem)
            {
                StreamClass.CreatePacket(89);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.UseItem)
            {
                selectedItem = actionType;
                drawMenuTab = 0;

                int itemId = inventoryManager.GetItem(selectedItem).Index;
                selectedItemName = entityManager.GetItem(itemId).Name;
            }

            if (action == MenuAction.UseItem)
            {
                StreamClass.CreatePacket(147);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedItem = -1;
                drawMenuTab = 0;

                int itemIndex = inventoryManager.GetItem(actionType).Index;

                DisplayMessage("Dropping " + entityManager.GetItem(itemIndex).Name);
            }

            if (action == MenuAction.ExamineItem)
            {
                DisplayMessage(entityManager.GetItem(actionType).Description);
            }

            if (action == MenuAction.CastSpellOnNpc)
            {
                Point2D destination = new Point2D(
                    (MenuActionLocations[actionId].X - 64) / GridSize,
                    (MenuActionLocations[actionId].Y - 64) / GridSize);

                walkTo1Tile(SectionLocation, destination, true);
                StreamClass.CreatePacket(71);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithNpc)
            {
                Point2D destination = new Point2D(
                    (MenuActionLocations[actionId].X - 64) / GridSize,
                    (MenuActionLocations[actionId].Y - 64) / GridSize);

                walkTo1Tile(SectionLocation, destination, true);
                StreamClass.CreatePacket(142);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.TalkToNpc)
            {
                Point2D destination = new Point2D(
                    (MenuActionLocations[actionId].X - 64) / GridSize,
                    (MenuActionLocations[actionId].Y - 64) / GridSize);

                walkTo1Tile(SectionLocation, destination, true);
                StreamClass.CreatePacket(177);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.CommandOnNpc)
            {
                Point2D destination = new Point2D(
                    (MenuActionLocations[actionId].X - 64) / GridSize,
                    (MenuActionLocations[actionId].Y - 64) / GridSize);

                walkTo1Tile(SectionLocation, destination, true);
                StreamClass.CreatePacket(74);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.AttackNpc || action == MenuAction.AttackNpc2)
            {
                Point2D destination = new Point2D(
                    (MenuActionLocations[actionId].X - 64) / GridSize,
                    (MenuActionLocations[actionId].Y - 64) / GridSize);

                walkTo1Tile(SectionLocation, destination, true);
                StreamClass.CreatePacket(73);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.ExamineNpc)
            {
                DisplayMessage(entityManager.GetNpc(actionType).Description);
            }

            if (action == MenuAction.CastSpellOnGround)
            {
                walkTo1Tile(SectionLocation, MenuActionLocations[actionId], true);
                StreamClass.CreatePacket(232);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(MenuActionLocations[actionId].X + AreaLocation.X);
                StreamClass.AddInt16(MenuActionLocations[actionId].Y + AreaLocation.Y);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.WalkHere)
            {
                walkTo1Tile(SectionLocation, MenuActionLocations[actionId], false);

                if (actionPictureType == -24)
                {
                    actionPictureType = 24;
                }
            }

            if (action == MenuAction.CastSpellOnSelf)
            {
                StreamClass.CreatePacket(206);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.Cancel)
            {
                selectedItem = -1;
                selectedSpell = -1;
            }
        }

        public override void resetIntVars()
        {
            loginScreenNumber = 0;
            loggedIn = false;
        }

        public void LoadMap()
        {
            engineHandle.mapsFree = unpackData("maps.jag", "map", 70);
            engineHandle.mapsMembers = unpackData("maps.mem", "members map", 75);
            engineHandle.landscapeFree = unpackData("land.jag", "landscape", 80);
            engineHandle.landscapeMembers = unpackData("land.mem", "members landscape", 85);
        }

        public void drawModel(int objectIndex, string modelName)
        {
            int k1 = ObjectLocations[objectIndex].X - CurrentPlayer.Location.X / 128;
            int l1 = ObjectLocations[objectIndex].Y - CurrentPlayer.Location.Y / 128;

            byte byte0 = 7;

            if (ObjectLocations[objectIndex].X >= 0 &&
                ObjectLocations[objectIndex].Y >= 0 &&
                ObjectLocations[objectIndex].X < 96 &&
                ObjectLocations[objectIndex].Y < 96 &&
                k1 > -byte0 &&
                k1 < byte0 &&
                l1 > -byte0 &&
                l1 < byte0)
            {
                gameCamera.removeModel(ObjectArray[objectIndex]);

                int i2 = entityManager.GetModelIndex(modelName);
                ObjectModel j2 = GameDataObjects[i2].CreateParent();
                Point3D shadingPoint = new Point3D(-50, -10, -50);

                gameCamera.addModel(j2);
                j2.UpdateShading(true, 48, 48, shadingPoint);
                j2.CopyTranslation(ObjectArray[objectIndex]);
                j2.index = objectIndex;
                ObjectArray[objectIndex] = j2;
            }
        }

        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int arg5, int arg6)
        {
            ClientMob player = Players[playerIndex];

            if (player.Appearance.TrousersColour == 255)// TODO this checks if the player is an invisible moderator
            {
                return;
            }

            int direction = player.currentSprite + (cameraRotation + 16) / 32 & 7;
            bool flag = false;
            int direction2 = direction;

            if (direction2 == 5)
            {
                direction2 = 3;
                flag = true;
            }
            else if (direction2 == 6)
            {
                direction2 = 2;
                flag = true;
            }
            else if (direction2 == 7)
            {
                direction2 = 1;
                flag = true;
            }

            int j1 = direction2 * 3 + walkModel[(player.stepCount / 6) % 4];

            if (player.currentSprite == 8)
            {
                direction2 = 5;
                direction = 2;
                flag = false;
                x -= (5 * arg6) / 100;
                j1 = direction2 * 3 + combatModelArray1[(tick / 5) % 8];
            }
            else if (player.currentSprite == 9)
            {
                direction2 = 5;
                direction = 2;
                flag = true;
                x += (5 * arg6) / 100;
                j1 = direction2 * 3 + combatModelArray2[(tick / 6) % 8];
            }

            for (int k1 = 0; k1 < 12; k1++)
            {
                int l1 = animationModelArray[direction][k1];
                int l2 = player.AppearanceItems[l1] - 1;
                if (l2 > entityManager.AnimationCount - 1)
                {
                    continue;
                }

                if (l2 >= 0)
                {
                    int k3 = 0;
                    int i4 = 0;
                    int j4 = j1;
                    if (flag && direction2 >= 1 && direction2 <= 3)
                    {
                        if (entityManager.GetAnimation(l2).HasF == 1)
                        {
                            j4 += 15;
                        }
                        else if (l1 == 4 && direction2 == 1)
                        {
                            k3 = -22;
                            i4 = -3;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = -8;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 3)
                        {
                            k3 = 26;
                            i4 = -5;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 1)
                        {
                            k3 = 22;
                            i4 = 3;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = 8;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 3)
                        {
                            k3 = -26;
                            i4 = 5;
                            j4 = direction2 * 3 + walkModel[(2 + player.stepCount / 6) % 4];
                        }
                    }

                    if (direction2 != 5 || entityManager.GetAnimation(l2).HasA == 1)
                    {
                        int k4 = j4 + entityManager.GetAnimation(l2).Number;
                        k3 = (k3 * width) / gameGraphics.pictureAssumedWidth[k4];
                        i4 = (i4 * height) / gameGraphics.pictureAssumedHeight[k4];
                        int l4 = (width * gameGraphics.pictureAssumedWidth[k4]) / gameGraphics.pictureAssumedWidth[entityManager.GetAnimation(l2).Number];
                        k3 -= (l4 - width) / 2;
                        int i5 = entityManager.GetAnimation(l2).CharacterColour;
                        int j5 = appearanceSkinColours[player.Appearance.SkinColour];

                        if (i5 == 1)
                        {
                            i5 = appearanceHairColours[player.Appearance.HairColour];
                        }
                        else
                            if (i5 == 2)
                        {
                            i5 = appearanceTopBottomColours[player.Appearance.TopColour];
                        }
                        else
                                if (i5 == 3)
                        {
                            i5 = appearanceTopBottomColours[player.Appearance.TrousersColour];
                        }

                        gameGraphics.DrawImage(x + k3, y + i4, l4, height, k4, i5, j5, arg5, flag);
                    }
                }
            }

            if (player.lastMessageTimeout > 0)
            {
                receivedMessageMidPoint[receivedMessagesCount] = gameGraphics.textWidth(player.lastMessage, 1) / 2;
                if (receivedMessageMidPoint[receivedMessagesCount] > 150)
                {
                    receivedMessageMidPoint[receivedMessagesCount] = 150;
                }

                receivedMessageHeight[receivedMessagesCount] = (gameGraphics.textWidth(player.lastMessage, 1) / 300) * gameGraphics.textHeightNumber(1);
                receivedMessageX[receivedMessagesCount] = x + width / 2;
                receivedMessageY[receivedMessagesCount] = y;
                receivedMessages[receivedMessagesCount++] = player.lastMessage;
            }

            if (player.PlayerSkullTimeout > 0)
            {
                itemAboveHeadX[itemsAboveHeadCount] = x + width / 2;
                itemAboveHeadY[itemsAboveHeadCount] = y;
                itemAboveHeadScale[itemsAboveHeadCount] = arg6;
                itemAboveHeadID[itemsAboveHeadCount++] = player.ItemAboveHeadId;
            }

            if (player.currentSprite == 8 || player.currentSprite == 9 || player.combatTimer != 0)
            {
                if (player.combatTimer > 0)
                {
                    int i2 = x;
                    if (player.currentSprite == 8)
                    {
                        i2 -= (20 * arg6) / 100;
                    }
                    else
                        if (player.currentSprite == 9)
                    {
                        i2 += (20 * arg6) / 100;
                    }

                    int i3 = (player.CurrentHitpoints * 30) / player.BaseHitpoints;
                    healthBarX[healthBarVisibleCount] = i2 + width / 2;
                    healthBarY[healthBarVisibleCount] = y;
                    healthBarMissing[healthBarVisibleCount++] = i3;
                }
                if (player.combatTimer > 150)
                {
                    int j2 = x;
                    if (player.currentSprite == 8)
                    {
                        j2 -= (10 * arg6) / 100;
                    }
                    else
                        if (player.currentSprite == 9)
                    {
                        j2 += (10 * arg6) / 100;
                    }

                    gameGraphics.DrawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 11);
                    gameGraphics.DrawText(player.LastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }

            if (player.PlayerSkulled == 1 && player.PlayerSkullTimeout == 0)
            {
                int k2 = arg5 + x + width / 2;
                if (player.currentSprite == 8)
                {
                    k2 -= (20 * arg6) / 100;
                }
                else
                    if (player.currentSprite == 9)
                {
                    k2 += (20 * arg6) / 100;
                }

                int j3 = (16 * arg6) / 100;
                int l3 = (16 * arg6) / 100;
                gameGraphics.DrawEntity(k2 - j3 / 2, y - l3 / 2 - (10 * arg6) / 100, j3, l3, baseInventoryPic + 13);
            }
        }

        public void walkToWallObject(Point2D location, int direction)
        {
            if (direction == 0)
            {
                Point2D locationBottom = new Point2D(location.X, location.Y - 1);
                WalkTo(SectionLocation, locationBottom, location, false, true);
                return;
            }

            if (direction == 1)
            {
                Point2D locationBottom = new Point2D(location.X - 1, location.Y);
                WalkTo(SectionLocation, locationBottom, location, false, true);
                return;
            }

            WalkTo(SectionLocation, location, location, true, true);
            return;
        }

        public void InitialiseLoginVars()
        {
            loggedIn = false;
            loginScreenNumber = 0;

            loginUsername = string.Empty;
            loginPassword = string.Empty;

            PlayerCount = 0;
            NpcCount = 0;
        }

        public void drawInventoryMenu(bool canRightClick)
        {
            int l = gameGraphics.GameSize.Width - 248;
            gameGraphics.DrawPicture(l, 3, baseInventoryPic + 1);

            for (int itemSlot = 0; itemSlot < InventoryManager.MaximumInventorySize; itemSlot++)
            {
                int j1 = l + (itemSlot % 5) * 49;
                int l1 = 36 + (itemSlot / 5) * 34;


                if (itemSlot < inventoryManager.InventoryItemsCount &&
                    inventoryManager.GetItem(itemSlot).IsEquipped)
                {
                    gameGraphics.drawBoxAlpha(j1, l1, 49, 34, 0xff0000, 128);
                }
                else
                {
                    int argb = ColourTranslator.ToArgb(181, 181, 181);

                    gameGraphics.drawBoxAlpha(j1, l1, 49, 34, argb, 128);
                }

                if (itemSlot < inventoryManager.InventoryItemsCount)
                {
                    InventoryItem inventoryItem = inventoryManager.GetItem(itemSlot);
                    Item item = entityManager.GetItem(inventoryItem.Index);

                    int pic = item.InventoryPicture;
                    int picMask = item.PictureMask;

                    gameGraphics.DrawImage(j1, l1, 48, 32, baseItemPicture + pic, picMask, 0, 0, false);

                    if (item.IsStackable == 0)
                    {
                        gameGraphics.DrawString(inventoryItem.Quantity.ToString(), j1 + 1, l1 + 10, 1, 0xffff00);
                    }
                }
            }

            for (int k1 = 1; k1 <= 4; k1++)
            {
                gameGraphics.DrawVerticalLine(l + k1 * 49, 36, (InventoryManager.MaximumInventorySize / 5) * 34, 0);
            }

            for (int i2 = 1; i2 <= InventoryManager.MaximumInventorySize / 5 - 1; i2++)
            {
                gameGraphics.DrawHorizontalLine(l, 36 + i2 * 34, 245, 0);
            }

            if (!canRightClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.GameSize.Width - 248);
            int j2 = InputManager.Instance.MouseLocation.Y - 36;

            if (l >= 0 && j2 >= 0 && l < 248 && j2 < (InventoryManager.MaximumInventorySize / 5) * 34)
            {
                int itemSlot = l / 49 + (j2 / 34) * 5;

                if (itemSlot < inventoryManager.InventoryItemsCount)
                {
                    InventoryItem inventoryItem = inventoryManager.GetItem(itemSlot);
                    Item item = entityManager.GetItem(inventoryItem.Index);

                    if (selectedSpell >= 0)
                    {
                        if (entityManager.GetSpell(selectedSpell).Type == 3)
                        {
                            menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on";
                            menuText2[menuOptionsCount] = "@lre@" + item.Name;
                            menuActions[menuOptionsCount] = MenuAction.CastSpellOnItem;
                            menuActionType[menuOptionsCount] = itemSlot;
                            menuActionVar1[menuOptionsCount] = selectedSpell;
                            menuOptionsCount++;
                            return;
                        }
                    }
                    else
                    {
                        if (selectedItem >= 0)
                        {
                            menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                            menuText2[menuOptionsCount] = "@lre@" + item.Name;
                            menuActions[menuOptionsCount] = MenuAction.UseItemWithItem;
                            menuActionType[menuOptionsCount] = itemSlot;
                            menuActionVar1[menuOptionsCount] = selectedItem;
                            menuOptionsCount++;
                            return;
                        }

                        if (inventoryItem.IsEquipped)
                        {
                            menuText1[menuOptionsCount] = "Remove";
                            menuText2[menuOptionsCount] = "@lre@" + item.Name;
                            menuActions[menuOptionsCount] = MenuAction.RemoveItem;
                            menuActionType[menuOptionsCount] = itemSlot;
                            menuOptionsCount++;
                        }
                        else if (item.IsEquipable != 0)
                        {
                            if ((item.IsEquipable & 0x18) != 0)
                            {
                                menuText1[menuOptionsCount] = "Wield";
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Wear";
                            }

                            menuText2[menuOptionsCount] = "@lre@" + item.Name;
                            menuActions[menuOptionsCount] = MenuAction.EquipItem;
                            menuActionType[menuOptionsCount] = itemSlot;
                            menuOptionsCount++;
                        }

                        if (!item.Command.Equals(""))
                        {
                            menuText1[menuOptionsCount] = item.Command;
                            menuText2[menuOptionsCount] = "@lre@" + item.Name;
                            menuActions[menuOptionsCount] = MenuAction.CommandOnItem;
                            menuActionType[menuOptionsCount] = itemSlot;
                            menuOptionsCount++;
                        }

                        menuText1[menuOptionsCount] = "Use";
                        menuText2[menuOptionsCount] = "@lre@" + item.Name;
                        menuActions[menuOptionsCount] = MenuAction.UseItem;
                        menuActionType[menuOptionsCount] = itemSlot;
                        menuOptionsCount++;

                        menuText1[menuOptionsCount] = "Drop";
                        menuText2[menuOptionsCount] = "@lre@" + item.Name;
                        menuActions[menuOptionsCount] = MenuAction.DropItem;
                        menuActionType[menuOptionsCount] = itemSlot;
                        menuOptionsCount++;

                        menuText1[menuOptionsCount] = "Examine";
                        menuText2[menuOptionsCount] = "@lre@" + item.Name;
                        menuActions[menuOptionsCount] = MenuAction.ExamineItem;
                        menuActionType[menuOptionsCount] = inventoryItem.Index;
                        menuOptionsCount++;
                    }
                }
            }
        }

        public void createLoginScreenBackgrounds()
        {
            int _bgScreenWidth = WindowSize.Width;
            OnLoadingSection?.Invoke(this, new EventArgs());
            int l = 0;
            sbyte byte0 = 50;
            sbyte byte1 = 50;

            engineHandle.loadSection(byte0 * 48 + 23, byte1 * 48 + 23, l);
            engineHandle.addObjects(GameDataObjects);

            int cameraX = 9728;
            int cameraY = 6400;
            int cameraDistance = 1100;
            int cameraRotation = 888;

            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(new Point3D(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY), 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.FadeScreenToBlack();
            gameGraphics.FadeScreenToBlack();

            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0x000000); //_bgScreenWidth=512
            for (int i1 = 6; i1 >= 1; i1--)
            {
                gameGraphics.DrawTransparentLine(0, i1, 0, i1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0x000000);

            for (int j1 = 6; j1 >= 1; j1--)
            {
                gameGraphics.DrawTransparentLine(0, j1, 0, 194 - j1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawImage(baseLoginScreenBackgroundPic, 0, 0, _bgScreenWidth, 200);
            gameGraphics.applyImage(baseLoginScreenBackgroundPic);

            cameraX = 9216;
            cameraY = 9216;
            cameraDistance = 1100;
            cameraRotation = 888;
            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(new Point3D(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY), 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.FadeScreenToBlack();
            gameGraphics.FadeScreenToBlack();

            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int k1 = 6; k1 >= 1; k1--)
            {
                gameGraphics.DrawTransparentLine(0, k1, 0, k1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int l1 = 6; l1 >= 1; l1--)
            {
                gameGraphics.DrawTransparentLine(0, l1, 0, 194 - l1, _bgScreenWidth, 8);
            }

            gameGraphics.DrawImage(baseLoginScreenBackgroundPic + 1, 0, 0, _bgScreenWidth, 200);
            gameGraphics.applyImage(baseLoginScreenBackgroundPic + 1);

            // Remove buildings
            for (int i2 = 0; i2 < 64; i2++)
            {

                gameCamera.removeModel(engineHandle.roofObject[0][i2]);
                gameCamera.removeModel(engineHandle.wallObject[0][i2]);
                gameCamera.removeModel(engineHandle.wallObject[1][i2]);
                gameCamera.removeModel(engineHandle.roofObject[1][i2]);
                gameCamera.removeModel(engineHandle.wallObject[2][i2]);
                gameCamera.removeModel(engineHandle.roofObject[2][i2]);
            }

            cameraX = 11136;//'\u2B80';
            cameraY = 10368;//'\u2880';
            cameraDistance = 500;//'\u01F4';
            cameraRotation = 376;//'\u0178';
            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(new Point3D(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY), 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.FadeScreenToBlack();
            gameGraphics.FadeScreenToBlack();

            gameGraphics.DrawBox(0, 0, _bgScreenWidth, 6, 0);

            for (int j2 = 6; j2 >= 1; j2--)
            {
                gameGraphics.DrawTransparentLine(0, j2, 0, j2, _bgScreenWidth, 8);
            }

            gameGraphics.DrawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int k2 = 6; k2 >= 1; k2--)
            {
                gameGraphics.DrawTransparentLine(0, k2, 0, 194, _bgScreenWidth, 8);
            }

            gameGraphics.DrawImage(baseInventoryPic + 10, 0, 0, _bgScreenWidth, 200);
            gameGraphics.applyImage(baseInventoryPic + 10);

            OnLoadingSectionCompleted?.Invoke(this, new EventArgs());
        }

        public override void HandlePacket(ServerCommand command, int length, sbyte[] data)
        {
            try
            {
                // TODO: Remove this check after all commands are properly handled
                bool properlyHandled = packetHandler.HandlePacket(command, data, length);

                if (properlyHandled)
                {
                    return;
                }

                Console.WriteLine($"Command unproperly handled: {command} (length={length})");

                if (command == ServerCommand.Command115)
                {
                    int k3 = (length - 1) / 4;

                    for (int i11 = 0; i11 < k3; i11++)
                    {
                        int k17 = SectionLocation.X + DataOperations.getShort2(data, 1 + i11 * 4) >> 3;
                        int i22 = SectionLocation.Y + DataOperations.getShort2(data, 3 + i11 * 4) >> 3;
                        int j25 = 0;

                        for (int groundItemIndex = 0; groundItemIndex < GroundItemCount; groundItemIndex++)
                        {
                            int j33 = (GroundItemLocations[groundItemIndex].X >> 3) - k17;
                            int l36 = (GroundItemLocations[groundItemIndex].Y >> 3) - i22;

                            if (j33 != 0 || l36 != 0)
                            {
                                if (groundItemIndex != j25)
                                {
                                    GroundItemLocations[j25] = GroundItemLocations[groundItemIndex];
                                    GroundItemId[j25] = GroundItemId[groundItemIndex];
                                    GroundItemObjectVar[j25] = GroundItemObjectVar[groundItemIndex];
                                }

                                j25++;
                            }
                        }

                        GroundItemCount = j25;
                        j25 = 0;

                        for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
                        {
                            int i37 = (ObjectLocations[objectIndex].X >> 3) - k17;
                            int j39 = (ObjectLocations[objectIndex].Y >> 3) - i22;

                            if (i37 != 0 || j39 != 0)
                            {
                                if (objectIndex != j25)
                                {
                                    ObjectArray[j25] = ObjectArray[objectIndex];
                                    ObjectArray[j25].index = j25;
                                    ObjectLocations[j25] = ObjectLocations[objectIndex];
                                    ObjectType[j25] = ObjectType[objectIndex];
                                    ObjectRotation[j25] = ObjectRotation[objectIndex];
                                }
                                j25++;
                            }
                            else
                            {
                                gameCamera.removeModel(ObjectArray[objectIndex]);
                                engineHandle.removeObject(ObjectLocations[objectIndex], ObjectType[objectIndex], ObjectRotation[objectIndex]);
                            }
                        }

                        ObjectCount = j25;
                        j25 = 0;

                        for (int wallObjectIndex = 0; wallObjectIndex < WallObjectCount; wallObjectIndex++)
                        {
                            int k39 = (WallObjectLocations[wallObjectIndex].X >> 3) - k17;
                            int l41 = (WallObjectLocations[wallObjectIndex].Y >> 3) - i22;

                            if (k39 != 0 || l41 != 0)
                            {
                                if (wallObjectIndex != j25)
                                {
                                    WallObjects[j25] = WallObjects[wallObjectIndex];
                                    WallObjects[j25].index = j25 + 10000;
                                    WallObjectLocations[j25] = WallObjectLocations[wallObjectIndex];
                                    WallObjectDirection[j25] = WallObjectDirection[wallObjectIndex];
                                    WallObjectId[j25] = WallObjectId[wallObjectIndex];
                                }

                                j25++;
                            }
                            else
                            {
                                gameCamera.removeModel(WallObjects[wallObjectIndex]);
                                engineHandle.removeWallObject(WallObjectLocations[wallObjectIndex], WallObjectDirection[wallObjectIndex], WallObjectId[wallObjectIndex]);
                            }
                        }

                        WallObjectCount = j25;
                    }

                    return;
                }

                if (command == ServerCommand.OpenShopWindow)
                {
                    ShowShopBox = true;
                    int off = 1;
                    int newShopItemCount = data[off++] & 0xff;
                    sbyte isGeneralShop = data[off++];
                    shopItemSellPriceModifier = data[off++] & 0xff;
                    shopItemBuyPriceModifier = data[off++] & 0xff;

                    for (int shopItemIndex = 0; shopItemIndex < 40; shopItemIndex++)
                    {
                        shopItems[shopItemIndex] = -1;
                    }

                    for (int item = 0; item < newShopItemCount; item++)
                    {
                        shopItems[item] = DataOperations.GetInt16(data, off);
                        off += 2;
                        shopItemCount[item] = DataOperations.GetInt16(data, off);
                        off += 2;
                        shopItemBuyPrice[item] = DataOperations.GetInt32(data, off);
                        off += 4;
                        shopItemSellPrice[item] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    if (isGeneralShop == 1)
                    {
                        int i29 = 39;
                        for (int itemSlot = 0; itemSlot < inventoryManager.InventoryItemsCount; itemSlot++)
                        {
                            InventoryItem inventoryItem = inventoryManager.GetItem(itemSlot);

                            if (i29 < newShopItemCount)
                            {
                                break;
                            }

                            bool flag2 = false;
                            for (int l39 = 0; l39 < 40; l39++)
                            {
                                if (shopItems[l39] != inventoryItem.Index)
                                {
                                    continue;
                                }

                                flag2 = true;
                                break;
                            }

                            if (inventoryItem.Index == 10)
                            {
                                flag2 = true;
                            }

                            if (!flag2)
                            {
                                shopItems[i29] = inventoryItem.Index & 0x7fff;
                                shopItemCount[i29] = 0;
                                shopItemSellPrice[i29] = entityManager.GetItem(shopItems[i29]).BasePrice - (int)(entityManager.GetItem(shopItems[i29]).BasePrice / 2.5);
                                shopItemSellPrice[i29] -= (int)(shopItemSellPrice[i29] * 0.10);
                                i29--;
                            }
                        }

                    }
                    if (selectedShopItemIndex >= 0 && selectedShopItemIndex < 40 && shopItems[selectedShopItemIndex] != selectedShopItemType)
                    {
                        selectedShopItemIndex = -1;
                        selectedShopItemType = -2;
                    }
                    return;
                }

                if (command == ServerCommand.OpenBankWindow)
                {
                    ShowBankBox = true;
                    int off = 1;

                    inventoryManager.ServerBankItemsCount = data[off++] & 0xff;
                    InventoryManager.MaximumBankSize = data[off++] & 0xff;

                    for (int bankSlot = 0; bankSlot < inventoryManager.ServerBankItemsCount; bankSlot++)
                    {
                        InventoryItem serverBankItem = inventoryManager.GetServerBankItem(bankSlot);

                        // TODO: Does the value update persist?
                        serverBankItem.Index = DataOperations.GetInt16(data, off);
                        off += 2;

                        // TODO: Does the value update persist?
                        serverBankItem.Quantity = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    inventoryManager.UpdateBankItems();
                    return;
                }

                if (command == ServerCommand.SkillExperience)
                {
                    int j5 = data[1] & 0xff;
                    Skills[j5].Experience = DataOperations.GetInt32(data, 2);
                    return;
                }

#warning have not fixed the following yet....
                Console.WriteLine($"Unfixed command? {command}");

                if (command == ServerCommand.TeleBubble)
                {
                    if (teleBubbleCount < 50)
                    {
                        int type = data[1] & 0xff;
                        int x = data[2] + SectionLocation.X;
                        int y = data[3] + SectionLocation.Y;

                        teleBubbleType[teleBubbleCount] = type;
                        teleBubbleTime[teleBubbleCount] = 0;
                        teleBubbleX[teleBubbleCount] = x;
                        teleBubbleY[teleBubbleCount] = y;
                        teleBubbleCount++;
                    }
                    return;
                }

                if (command == ServerCommand.Command225)
                {
                    sleepingStatusText = "Incorrect - Please wait...";
                    return;
                }

                if (command == ServerCommand.Command182)
                {
                    int off = 1;
                    questManager.QuestPoints = DataOperations.GetInt16(data, off);
                    off += 2;

                    for (int i = 0; i < questManager.QuestsCount; i++)
                    {
                        // TODO: Ditch numerical identifiers
                        questManager.SetStage(i.ToString(), data[i + 1]);
                    }

                    return;
                }

                if (command == ServerCommand.Command233)
                {
                    questManager.QuestPoints = DataOperations.GetInt8(data[1]);
                    int count = DataOperations.GetInt8(data[2]);
                    int off = 3;
                    string[] newQuestNames = new string[count];
                    int[] newQuestStage = new int[count];

                    for (int i = 0; i < count; i++)
                    {
                        // TODO: Ditch numerical identifiers
                        int id = DataOperations.GetInt8(data[off++]);
                        int newStage = DataOperations.GetInt8(data[off++]);

                        questManager.SetStage(id.ToString(), newStage);
                    }

                    return;
                }

                Console.WriteLine("UNHANDLED PACKET:" + command + " LEN:" + length + " @#!#@#!#@#!#@#!#@#!#@");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void initVars()
        {
            CombatStyle = 0;
            loginScreenNumber = 0;
            loggedIn = true;

            gameGraphics.ClearScreen();
            OnDrawDone();

            for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
            {
                gameCamera.removeModel(ObjectArray[objectIndex]);
                engineHandle.removeObject(ObjectLocations[objectIndex], ObjectType[objectIndex], ObjectRotation[objectIndex]);
            }

            for (int wallObjectIndex = 0; wallObjectIndex < WallObjectCount; wallObjectIndex++)
            {
                gameCamera.removeModel(WallObjects[wallObjectIndex]);
                engineHandle.removeWallObject(WallObjectLocations[wallObjectIndex], WallObjectDirection[wallObjectIndex], WallObjectId[wallObjectIndex]);
            }

            ObjectCount = 0;
            WallObjectCount = 0;
            GroundItemCount = 0;
            PlayerCount = 0;
            NpcCount = 0;

            for (int i = 0; i < 4000; i++)
            {
                Mobs[i] = null;
                NpcAttackingArray[i] = null;
            }

            for (int i = 0; i < 500; i++)
            {
                Players[i] = null;
                Npcs[i] = null;
            }

            for (int i = 0; i < 50; i++)
            {
                prayerOn[i] = false;
            }

            mouseButtonClick = 0;
            lastMouseButton = 0;
            mouseButton = 0;
            ShowShopBox = false;
            ShowBankBox = false;
            IsSleeping = false;
        }

        public void drawMinimapMenu(bool canClick)
        {
            int l = gameGraphics.GameSize.Width - 199;
            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';

            l += 40;
            gameGraphics.SetDimensions(l, 36, l + c1, 36 + c3);

            int j1 = 192 + minimapRandomRotationY;
            int l1 = cameraRotation + minimapRandomRotationX & 0xff;
            int j2 = ((CurrentPlayer.Location.X - 6040) * 3 * j1) / 2048;
            int l3 = ((CurrentPlayer.Location.Y - 6040) * 3 * j1) / 2048;
            int j5 = Camera.bbk[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.bbk[(1024 - l1 * 4 & 0x3ff) + 1024];
            int j6 = l3 * j5 + j2 * l5 >> 18;
            l3 = l3 * l5 - j2 * j5 >> 18;

            gameGraphics.drawMinimapPic((l + c1 / 2) - j6, 36 + c3 / 2 + l3, baseInventoryPic - 1, l1 + 64 & 0xff, j1);
            gameGraphics.SetDimensions(0, 0, WindowSize.Width, WindowSize.Height + 12);

            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.GameSize.Width - 199);
            int l8 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + minimapRandomRotationY;
                int i2 = cameraRotation + minimapRandomRotationX & 0xff;
                int i1 = gameGraphics.GameSize.Width - 199;
                i1 += 40;
                int k3 = ((InputManager.Instance.MouseLocation.X - (i1 + c2 / 2)) * 16384) / (3 * k1);
                int i5 = ((InputManager.Instance.MouseLocation.Y - (36 + c4 / 2)) * 16384) / (3 * k1);
                int k5 = Camera.bbk[1024 - i2 * 4 & 0x3ff];
                int i6 = Camera.bbk[(1024 - i2 * 4 & 0x3ff) + 1024];
                int k7 = i5 * k5 + k3 * i6 >> 15;
                i5 = i5 * i6 - k3 * k5 >> 15;
                k3 = k7;
                k3 += CurrentPlayer.Location.X;
                i5 = CurrentPlayer.Location.Y - i5;

                Point2D destination = new Point2D(k3 / 128, i5 / 128);

                if (mouseButtonClick == 1)
                {
                    walkTo1Tile(SectionLocation, destination, false);
                }

                mouseButtonClick = 0;
            }
        }

        public bool validCameraAngle(int arg0)
        {
            int l = CurrentPlayer.Location.X / 128;
            int i1 = CurrentPlayer.Location.Y / 128;
            for (int j1 = 2; j1 >= 1; j1--)
            {
                if (arg0 == 1 && ((engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (arg0 == 3 && ((engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1] & 0x80) == 128 || (engineHandle.tiles[l - j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (arg0 == 5 && ((engineHandle.tiles[l][i1 + j1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1 + j1] & 0x80) == 128))
                {
                    return false;
                }

                if (arg0 == 7 && ((engineHandle.tiles[l][i1 - j1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1] & 0x80) == 128 || (engineHandle.tiles[l + j1][i1 - j1] & 0x80) == 128))
                {
                    return false;
                }

                if (arg0 == 0 && (engineHandle.tiles[l][i1 - j1] & 0x80) == 128)
                {
                    return false;
                }

                if (arg0 == 2 && (engineHandle.tiles[l - j1][i1] & 0x80) == 128)
                {
                    return false;
                }

                if (arg0 == 4 && (engineHandle.tiles[l][i1 + j1] & 0x80) == 128)
                {
                    return false;
                }

                if (arg0 == 6 && (engineHandle.tiles[l + j1][i1] & 0x80) == 128)
                {
                    return false;
                }
            }

            return true;
        }

        public override void LoadGame()
        {
            int l = 0;
            for (int i1 = 0; i1 < 99; i1++)
            {
                int j1 = i1 + 1;
                int l1 = (int)(j1 + 300D * Math.Pow(2D, j1 / 7D));
                l += l1;
                experienceList[i1] = (l & 0xffffffc) / 4;
            }
            LoadConfig();
            if (errorLoading)
            {
                return;
            }

            maxPacketReadCount = 500;
            baseInventoryPic = 2000;
            baseScrollPic = baseInventoryPic + 100;
            baseItemPicture = baseScrollPic + 50;
            baseLoginScreenBackgroundPic = baseItemPicture + 1000;
            baseProjectilePic = baseLoginScreenBackgroundPic + 10;
            baseTexturePic = baseProjectilePic + 50;
            subTexturePic = baseTexturePic + 10;
            SetRefreshRate(50);
            gameGraphics = new GameImageMiddleMan(WindowSize.Width, WindowSize.Height + 12, 4000);
            gameGraphics.gameReference = this;
            gameGraphics.SetDimensions(0, 0, WindowSize.Width, WindowSize.Height + 12);
            Menu.gdh = false;
            Menu.baseScrollPic = baseScrollPic;
            spellMenu = new Menu(gameGraphics, 5);
            int k1 = gameGraphics.GameSize.Width - 199;
            sbyte byte0 = 36;
            spellMenuHandle = spellMenu.createList(k1, byte0 + 24, 196, 90, 1, 500, true);
            questMenu = new Menu(gameGraphics, 5);
            questMenuHandle = questMenu.createList(k1, byte0 + 24, 196, 251, 1, 500, true);
            loadMedia();

            if (errorLoading)
            {
                return;
            }

            loadAnimations();

            if (errorLoading)
            {
                return;
            }

            gameCamera = new Camera(gameGraphics, 15000, 15000, 1000);
            Point3D shadingPoint = new Point3D(-50, -10, -50);

            gameCamera.setCameraSize(WindowSize.Width / 2, WindowSize.Height / 2, WindowSize.Width / 2, WindowSize.Height / 2, WindowSize.Width, cameraFieldOfView);
            gameCamera.zoom1 = 2400;
            gameCamera.zoom2 = 2400;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 2300;
            gameCamera.bjk(shadingPoint);
            engineHandle = new EngineHandle(entityManager, gameCamera, gameGraphics);
            engineHandle.baseInventoryPic = baseInventoryPic;
            loadTextures();

            if (errorLoading)
            {
                return;
            }

            LoadModels();

            if (errorLoading)
            {
                return;
            }

            LoadMap();

            if (errorLoading)
            {
                return;
            }

            if (!errorLoading)
            {
                OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Starting game...", 100));
                drawLoadingBarText(100, "Starting game...");
                createLoginMenus();
                createAppearanceWindow();
                InitialiseLoginVars();

                OnContentLoadedCompleted?.Invoke(this, new EventArgs());

                //createLoginScreenBackgrounds();
                return;
            }
        }

        public void createLoginMenus()
        {
            loginMenuFirst = new Menu(gameGraphics, 50);

            int l = 40;
            loginMenuFirst.drawText(256, 200 + l, "Welcome to RuneScape", 4, true);
            loginMenuFirst.drawText(256, 215 + l, "You need a member account to use this server", 4, true);
            loginMenuFirst.drawButton(256, 250 + l, 200, 35);
            loginMenuFirst.drawText(256, 250 + l, "Click here to login", 5, false);
            loginMenuLoginButton = loginMenuFirst.createButton(256, 250 + l, 200, 35);
            loginNewUser = new Menu(gameGraphics, 50);
            l = 230;
            loginNewUser.drawText(256, l + 8, "To create an account please go back to the", 4, true);
            l += 20;
            loginNewUser.drawText(256, l + 8, "www.runescape.com front page, and choose 'create account'", 4, true);
            l += 30;
            loginNewUser.drawButton(256, l + 17, 150, 34);
            loginNewUser.drawText(256, l + 17, "Ok", 5, false);
            loginMenuOkButton = loginNewUser.createButton(256, l + 17, 150, 34);
            loginMenuLogin = new Menu(gameGraphics, 50);
            l = 230;
            loginMenuStatusText = loginMenuLogin.drawText(256, l - 10, "Please enter your username and password", 4, true);
            l += 28;
            loginMenuLogin.drawButton(140, l, 200, 40);
            loginMenuLogin.drawText(140, l - 10, "Username:", 4, false);
            loginMenuUserText = loginMenuLogin.createInput(140, l + 10, 200, 40, 4, 12, false, false);
            l += 47;
            loginMenuLogin.drawButton(190, l, 200, 40);
            loginMenuLogin.drawText(190, l - 10, "Password:", 4, false);
            loginMenuPasswordText = loginMenuLogin.createInput(190, l + 10, 200, 40, 4, 20, true, false);
            l -= 55;
            loginMenuLogin.drawButton(410, l, 120, 25);
            loginMenuLogin.drawText(410, l, "Ok", 4, false);
            loginMenuOkLoginButton = loginMenuLogin.createButton(410, l, 120, 25);
            l += 30;
            loginMenuLogin.drawButton(410, l, 120, 25);
            loginMenuLogin.drawText(410, l, "Cancel", 4, false);
            loginMenuCancelButton = loginMenuLogin.createButton(410, l, 120, 25);
            l += 30;
            loginMenuLogin.setFocus(loginMenuUserText);
        }

        public void loadMedia()
        {
            sbyte[] media = unpackData("media.jag", "2d graphics", 20);
            if (media == null)
            {
                errorLoading = true;
                return;
            }
            sbyte[] abyte1 = DataOperations.loadData("index.dat", 0, media);
            gameGraphics.unpackImageData(baseInventoryPic, DataOperations.loadData("inv1.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 1, DataOperations.loadData("inv2.dat", 0, media), abyte1, 6);
            gameGraphics.unpackImageData(baseInventoryPic + 9, DataOperations.loadData("bubble.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 10, DataOperations.loadData("runescape.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 11, DataOperations.loadData("splat.dat", 0, media), abyte1, 3);
            gameGraphics.unpackImageData(baseInventoryPic + 14, DataOperations.loadData("icon.dat", 0, media), abyte1, 8);
            gameGraphics.unpackImageData(baseInventoryPic + 22, DataOperations.loadData("hbar.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 23, DataOperations.loadData("hbar2.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 24, DataOperations.loadData("compass.dat", 0, media), abyte1, 1);
            gameGraphics.unpackImageData(baseInventoryPic + 25, DataOperations.loadData("buttons.dat", 0, media), abyte1, 2);
            gameGraphics.unpackImageData(baseScrollPic, DataOperations.loadData("scrollbar.dat", 0, media), abyte1, 2);
            gameGraphics.unpackImageData(baseScrollPic + 2, DataOperations.loadData("corners.dat", 0, media), abyte1, 4);
            gameGraphics.unpackImageData(baseScrollPic + 6, DataOperations.loadData("arrows.dat", 0, media), abyte1, 2);
            gameGraphics.unpackImageData(baseProjectilePic, DataOperations.loadData("projectile.dat", 0, media), abyte1, entityManager.SpellProjectileCount);
            int l = entityManager.HighestLoadedPicture;
            for (int i1 = 1; l > 0; i1++)
            {
                int j1 = l;
                l -= 30;
                if (j1 > 30)
                {
                    j1 = 30;
                }

                gameGraphics.unpackImageData(baseItemPicture + (i1 - 1) * 30, DataOperations.loadData("objects" + i1 + ".dat", 0, media), abyte1, j1);
            }
            //gameGraphics.UpdateGameImage();
            gameGraphics.loadImage(baseInventoryPic);
            gameGraphics.loadImage(baseInventoryPic + 9);
            for (int k1 = 11; k1 <= 26; k1++)
            {
                gameGraphics.loadImage(baseInventoryPic + k1);
            }

            for (int l1 = 0; l1 < entityManager.SpellProjectileCount; l1++)
            {
                gameGraphics.loadImage(baseProjectilePic + l1);
            }

            for (int i2 = 0; i2 < entityManager.HighestLoadedPicture; i2++)
            {
                gameGraphics.loadImage(baseProjectilePic + i2);
            }


        }

        public override void CheckInputs()
        {
            if (memoryError)
            {
                return;
            }

            if (errorLoading)
            {
                return;
            }

            try
            {
                tick++;

                if (loggedIn)
                {
                    checkGameInputs();
                }
                else
                {
                    checkLoginScreenInputs();
                }

                lastMouseButton = 0;
                cameraRotateTime++;
                if (cameraRotateTime > 500)
                {
                    cameraRotateTime = 0;
                    int l = (int)(Helper.Random.NextDouble() * 4D);
                    if ((l & 1) == 1)
                    {
                        cameraRotationXAmount += cameraRotationXIncrement;
                    }

                    if ((l & 2) == 2)
                    {
                        cameraRotationYAmount += cameraRotationYIncrement;
                    }
                }
                if (cameraRotationXAmount < -50)
                {
                    cameraRotationXIncrement = 2;
                }

                if (cameraRotationXAmount > 50)
                {
                    cameraRotationXIncrement = -2;
                }

                if (cameraRotationYAmount < -50)
                {
                    cameraRotationYIncrement = 2;
                }

                if (cameraRotationYAmount > 50)
                {
                    cameraRotationYIncrement = -2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex.Message);

                UnloadContent();
                memoryError = true;
            }
        }

        public void loadAnimations()
        {
            StringBuilder sb = new StringBuilder();
            sbyte[] abyte0 = null;
            sbyte[] abyte1 = null;
            abyte0 = unpackData("entity.jag", "people and monsters", 30);
            if (abyte0 == null)
            {
                errorLoading = true;
                return;
            }
            abyte1 = DataOperations.loadData("index.dat", 0, abyte0);
            sbyte[] abyte2 = null;
            sbyte[] abyte3 = null;
            abyte2 = unpackData("entity.mem", "member graphics", 45);
            if (abyte2 == null)
            {
                errorLoading = true;
                return;
            }
            abyte3 = DataOperations.loadData("index.dat", 0, abyte2);
            int l = 0;
            animationNumber = 0;
            //label0:
            for (int i1 = 0; i1 < entityManager.AnimationCount; i1++)
            {
                //   label4:
                bool breakThis = false;
                string s1 = entityManager.GetAnimation(i1).Name;
                for (int j1 = 0; j1 < i1; j1++)
                {
                    if (!entityManager.GetAnimation(j1).Name.ToLower().Equals(s1))
                    {
                        continue;
                    }

                    entityManager.GetAnimation(i1).Number = entityManager.GetAnimation(j1).Number;

                    // i1++;
                    // goto label0;
                    //break;
                    breakThis = true;
                    break;
                }
                if (breakThis)
                {
                    continue;
                }

                //label4:
                sbyte[] abyte7 = DataOperations.loadData(s1 + ".dat", 0, abyte0);
                sbyte[] abyte4 = abyte1;
                if (abyte7 == null)
                {
                    abyte7 = DataOperations.loadData(s1 + ".dat", 0, abyte2);
                    abyte4 = abyte3;
                }
                if (abyte7 != null)
                {
                    try
                    {
                        gameGraphics.unpackImageData(animationNumber, abyte7, abyte4, 15);
                        l += 15;
                        if (entityManager.GetAnimation(i1).HasA == 1)
                        {
                            sbyte[] abyte8 = DataOperations.loadData(s1 + "a.dat", 0, abyte0);
                            sbyte[] abyte5 = abyte1;
                            if (abyte8 == null)
                            {
                                abyte8 = DataOperations.loadData(s1 + "a.dat", 0, abyte2);
                                abyte5 = abyte3;
                            }
                            gameGraphics.unpackImageData(animationNumber + 15, abyte8, abyte5, 3);
                            l += 3;
                        }
                        if (entityManager.GetAnimation(i1).HasF == 1)
                        {
                            sbyte[] abyte9 = DataOperations.loadData(s1 + "f.dat", 0, abyte0);
                            sbyte[] abyte6 = abyte1;
                            if (abyte9 == null)
                            {
                                abyte9 = DataOperations.loadData(s1 + "f.dat", 0, abyte2);
                                abyte6 = abyte3;
                            }
                            gameGraphics.unpackImageData(animationNumber + 18, abyte9, abyte6, 9);
                            l += 9;
                        }
                        if (entityManager.GetAnimation(i1).GenderModel != 0)
                        {
                            for (int k1 = animationNumber; k1 < animationNumber + 27; k1++)
                            {
                                gameGraphics.loadImage(k1);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                    }
                }
                entityManager.GetAnimation(i1).Number = animationNumber;
                animationNumber += 27;




                //if (File.Exists("animations-loaded.txt")) File.Delete("animations-loaded.txt");
                //if (!File.Exists("animations-loaded.txt")) File.Create("animations-loaded.txt").Close();
                sb.AppendLine("Loaded: " + l + " frames of animation");

#warning ugly fix for forcing animation count to 1143.
                if (l == 1143)
                {
                    break;
                }
            }
            var str = sb.ToString();
            Console.WriteLine("Loaded: " + l + " frames of animation");
        }

        public void updateAppearanceWindow()
        {
            appearanceMenu.mouseClick(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y, lastMouseButton, mouseButton);
            if (appearanceMenu.isClicked(appearanceHeadLeftArrow))
            {
                do
                {
                    appearanceHeadType = ((appearanceHeadType - 1) + entityManager.AnimationCount) % entityManager.AnimationCount;
                }
                while ((entityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 ||
                       (entityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.isClicked(appearanceHeadRightArrow))
            {
                do
                {
                    appearanceHeadType = (appearanceHeadType + 1) % entityManager.AnimationCount;
                }
                while ((entityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 ||
                       (entityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.isClicked(appearanceHairLeftArrow))
            {
                appearanceHairColour = ((appearanceHairColour - 1) + appearanceHairColours.Length) % appearanceHairColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceHairRightArrow))
            {
                appearanceHairColour = (appearanceHairColour + 1) % appearanceHairColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceGenderLeftArrow) || appearanceMenu.isClicked(appearanceGenderRightArrow))
            {
                for (appearanceHeadGender = 3 - appearanceHeadGender; (entityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 || (entityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0; appearanceHeadType = (appearanceHeadType + 1) % entityManager.AnimationCount)
                {
                    ;
                }

                for (; (entityManager.GetAnimation(appearanceBodyGender).GenderModel & 3) != 2 || (entityManager.GetAnimation(appearanceBodyGender).GenderModel & 4 * appearanceHeadGender) == 0; appearanceBodyGender = (appearanceBodyGender + 1) % entityManager.AnimationCount)
                {
                    ;
                }
            }

            if (appearanceMenu.isClicked(appearanceTopLeftArrow))
            {
                appearanceTopColour = ((appearanceTopColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceTopRightArrow))
            {
                appearanceTopColour = (appearanceTopColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceSkinLeftArrow))
            {
                appearanceSkinColour = ((appearanceSkinColour - 1) + appearanceSkinColours.Length) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceSkingRightArrow))
            {
                appearanceSkinColour = (appearanceSkinColour + 1) % appearanceSkinColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceBottomLeftArrow))
            {
                appearanceBottomColour = ((appearanceBottomColour - 1) + appearanceTopBottomColours.Length) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceBottomRightArrow))
            {
                appearanceBottomColour = (appearanceBottomColour + 1) % appearanceTopBottomColours.Length;
            }

            if (appearanceMenu.isClicked(appearanceAcceptButton))
            {
                StreamClass.CreatePacket(218);
                StreamClass.AddInt8(appearanceHeadGender);
                StreamClass.AddInt8(appearanceHeadType);
                StreamClass.AddInt8(appearanceBodyGender);
                StreamClass.AddInt8(appearance2Colour);
                StreamClass.AddInt8(appearanceHairColour);
                StreamClass.AddInt8(appearanceTopColour);
                StreamClass.AddInt8(appearanceBottomColour);
                StreamClass.AddInt8(appearanceSkinColour);
                StreamClass.FormatPacket();
                gameGraphics.ClearScreen();
                ShowAppearanceWindow = false;
            }
        }

        public bool WalkTo(Point2D startLocation, Point2D destinationBottom, Point2D destinationTop, bool checkForObjects, bool walkToACommand)
        {
            int stepCount = engineHandle.generatePath(startLocation, destinationBottom, destinationTop, WalkArrayLocations, checkForObjects);

            if (stepCount == -1)
            {
                if (walkToACommand)
                {
                    stepCount = 1;
                    WalkArrayLocations[0] = destinationBottom;
                }
                else
                {
                    return false;
                }
            }

            stepCount--;
            startLocation = WalkArrayLocations[stepCount];
            stepCount--;

            if (walkToACommand)
            {
                StreamClass.CreatePacket(246);
            }
            else
            {
                StreamClass.CreatePacket(132);
            }

            StreamClass.AddInt16(startLocation.X + AreaLocation.X);
            StreamClass.AddInt16(startLocation.Y + AreaLocation.Y);

            if (walkToACommand && stepCount == -1 && (startLocation.X + AreaLocation.X) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int currentStep = stepCount; currentStep >= 0 && currentStep > stepCount - 25; currentStep--)
            {
                StreamClass.AddInt8(WalkArrayLocations[currentStep].X - startLocation.X);
                StreamClass.AddInt8(WalkArrayLocations[currentStep].Y - startLocation.Y);
            }

            StreamClass.FormatPacket();

            actionPictureType = -24;
            walkMouseX = InputManager.Instance.MouseLocation.X;
            walkMouseY = InputManager.Instance.MouseLocation.Y;

            return true;
        }

        public bool walkTo2(Point2D startLocation, Point2D destinationBottom, Point2D destinationTop, bool unknownDifferent, bool walkToACommand)
        {
            int stepCount = engineHandle.generatePath(startLocation, destinationBottom, destinationTop, WalkArrayLocations, unknownDifferent);

            if (stepCount == -1)
            {
                return false;
            }

            stepCount--;
            startLocation = WalkArrayLocations[stepCount];
            stepCount--;

            if (walkToACommand)
            {
                StreamClass.CreatePacket(246);
            }
            else
            {
                StreamClass.CreatePacket(132);
            }

            StreamClass.AddInt16(startLocation.X + AreaLocation.X);
            StreamClass.AddInt16(startLocation.Y + AreaLocation.Y);

            if (walkToACommand && stepCount == -1 && (startLocation.X + AreaLocation.X) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int step = stepCount; step >= 0 && step > stepCount - 25; step--)
            {
                StreamClass.AddInt8(WalkArrayLocations[step].X - startLocation.X);
                StreamClass.AddInt8(WalkArrayLocations[step].Y - startLocation.Y);
            }

            StreamClass.FormatPacket();
            actionPictureType = -24;
            walkMouseX = InputManager.Instance.MouseLocation.X;
            walkMouseY = InputManager.Instance.MouseLocation.Y;

            return true;
        }

        public void SetCombatStyle(CombatStyle style)
        {
            if (CombatStyle == style)
            {
                return;
            }

            CombatStyle = style;
            StreamClass.CreatePacket(42);
            StreamClass.AddInt8((int)CombatStyle);
            StreamClass.FormatPacket();
        }

        public void walkToObject(Point2D location, int arg2, int arg3)
        {
            int l;
            int i1;

            if (arg2 == 0 || arg2 == 4)
            {
                l = entityManager.GetWorldObject(arg3).Width;
                i1 = entityManager.GetWorldObject(arg3).Height;
            }
            else
            {
                i1 = entityManager.GetWorldObject(arg3).Width;
                l = entityManager.GetWorldObject(arg3).Height;
            }

            Point2D loc = new Point2D(location.X + l - 1, location.Y + i1 - 1);

            if (entityManager.GetWorldObject(arg3).Type == 2 || entityManager.GetWorldObject(arg3).Type == 3)
            {
                if (arg2 == 0)
                {
                    location = new Point2D(location.X - 1, location.Y);
                    l++;
                }

                if (arg2 == 2)
                {
                    i1++;
                }

                if (arg2 == 4)
                {
                    l++;
                }

                if (arg2 == 6)
                {
                    location = new Point2D(location.X, location.Y - 1);
                    i1++;
                }

                WalkTo(SectionLocation, location, loc, false, true);
                return;
            }
            else
            {
                WalkTo(SectionLocation, location, loc, true, true);
                return;
            }
        }

        public void autoRotateCamera()
        {
            if ((cameraAutoAngle & 1) == 1 && validCameraAngle(cameraAutoAngle))
            {
                return;
            }

            if ((cameraAutoAngle & 1) == 0 && validCameraAngle(cameraAutoAngle))
            {
                if (validCameraAngle(cameraAutoAngle + 1 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 1 & 7;
                    return;
                }
                if (validCameraAngle(cameraAutoAngle + 7 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 7 & 7;
                }

                return;
            }
            int[] ai = {
            1, -1, 2, -2, 3, -3, 4
        };
            for (int l = 0; l < 7; l++)
            {
                if (!validCameraAngle(cameraAutoAngle + ai[l] + 8 & 7))
                {
                    continue;
                }

                cameraAutoAngle = cameraAutoAngle + ai[l] + 8 & 7;
                break;
            }

            if ((cameraAutoAngle & 1) == 0 && validCameraAngle(cameraAutoAngle))
            {
                if (validCameraAngle(cameraAutoAngle + 1 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 1 & 7;
                }
                else if (validCameraAngle(cameraAutoAngle + 7 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 7 & 7;
                }
            }
        }

        public void walkToGroundItem(Point2D location, Point2D destination, bool flag)
        {
            if (walkTo2(location, destination, destination, false, flag))
            {
                return;
            }

            WalkTo(location, destination, destination, true, flag);
        }

        public override void loginScreenPrint(string s1, string s2)
        {
            if (loginScreenNumber == 2 && loginMenuLogin != null)
            {
                loginMenuLogin.updateText(loginMenuStatusText, s1 + " " + s2);
            }

            drawLoginScreens();
            ResetTimings();
        }

        public void DrawTeleBubble(int x, int y, int j1, int k1, int l1)
        {
            int type = teleBubbleType[l1];
            int time = teleBubbleTime[l1];

            if (type == 0)
            {
                int i3 = 255 + time * 5 * 256;
                gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 20 + time * 2, i3, 255 - time * 5);
            }

            if (type == 1)
            {
                int j3 = 0xff0000 + time * 5 * 256;
                gameGraphics.DrawCircle(x + j1 / 2, y + k1 / 2, 10 + time, j3, 255 - time * 5);
            }
        }

        public void checkLoginScreenInputs()
        {
            if (socketTimeout > 0)
            {
                socketTimeout--;
            }

            if (loginScreenNumber == 0)
            {
                if (loginMenuFirst == null)
                {
                    return;
                }

                loginMenuFirst.mouseClick(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y, lastMouseButton, mouseButton);
                if (loginMenuFirst.isClicked(loginButtonNewUser))
                {
                    loginScreenNumber = 1;
                }

                if (loginMenuFirst.isClicked(loginMenuLoginButton))
                {
                    loginScreenNumber = 2;
                    loginMenuLogin.updateText(loginMenuStatusText, "Please enter your username and password");
                    loginMenuLogin.updateText(loginMenuUserText, "");
                    loginMenuLogin.updateText(loginMenuPasswordText, "");
                    loginMenuLogin.setFocus(loginMenuUserText);
                    return;
                }
            }
            else if (loginScreenNumber == 1)
            {
                if (loginNewUser == null)
                {
                    return;
                }

                loginNewUser.mouseClick(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y, lastMouseButton, mouseButton);
                if (loginNewUser.isClicked(loginMenuOkButton))
                {
                    loginScreenNumber = 0;
                    return;
                }
            }
            else if (loginScreenNumber == 2)
            {
                loginMenuLogin.mouseClick(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y, lastMouseButton, mouseButton);
                if (loginMenuLogin.isClicked(loginMenuCancelButton))
                {
                    loginScreenNumber = 0;
                }

                if (loginMenuLogin.isClicked(loginMenuUserText))
                {
                    loginMenuLogin.setFocus(loginMenuPasswordText);
                }

                if (loginMenuLogin.isClicked(loginMenuPasswordText) || loginMenuLogin.isClicked(loginMenuOkLoginButton))
                {
                    loginUsername = loginMenuLogin.getText(loginMenuUserText);
                    loginPassword = loginMenuLogin.getText(loginMenuPasswordText);
                    connect(loginUsername, loginPassword, false);
                }
            }
        }

        public override void DrawWindow()
        {
            paint();

            if (errorLoading)
            {
                SetRefreshRate(1);
                return;
            }

            if (memoryError)
            {
                return;
            }

            try
            {
                if (loggedIn)
                {
                    gameGraphics.IsLoggedIn = true;
                    drawGame();

                    return;
                }

                gameGraphics.IsLoggedIn = false;
                drawLoginScreens();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex);

                UnloadContent();
                memoryError = true;
            }
        }

        public void drawQuestionMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < QuestionMenuCount; l++)
                {
                    if (InputManager.Instance.MouseLocation.X >= gameGraphics.textWidth(questionMenuAnswer[l], 1) || InputManager.Instance.MouseLocation.Y <= l * 12 || InputManager.Instance.MouseLocation.Y >= 12 + l * 12)
                    {
                        continue;
                    }

                    StreamClass.CreatePacket(154);
                    StreamClass.AddInt8(l);
                    StreamClass.FormatPacket();
                    break;
                }

                mouseButtonClick = 0;
                ShowQuestionMenu = false;
                return;
            }

            for (int i1 = 0; i1 < QuestionMenuCount; i1++)
            {
                int j1 = 65535;
                if (InputManager.Instance.MouseLocation.X < gameGraphics.textWidth(questionMenuAnswer[i1], 1) && InputManager.Instance.MouseLocation.Y > i1 * 12 && InputManager.Instance.MouseLocation.Y < 12 + i1 * 12)
                {
                    j1 = 0xff0000;
                }

                gameGraphics.DrawString(questionMenuAnswer[i1], 6, 12 + i1 * 12, 1, j1);
            }
        }

        public virtual void drawLoginScreens()
        {
            LoginScreenShown = false;

            if (gameGraphics == null)
            {
                return;
            }

            gameGraphics.ClearScreen();
            if (loginScreenNumber == 0 || loginScreenNumber == 1 || loginScreenNumber == 2 || loginScreenNumber == 3)
            {
                int l = (tick * 2) % 3072;
                if (l < 1024)
                {
                    gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic);
                    if (l > 768)
                    {
                        gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic + 1, l - 768);
                    }
                }
                else if (l < 2048)
                {
                    gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic + 1);
                    if (l > 1792)
                    {
                        gameGraphics.DrawPicture(0, 10, baseInventoryPic + 10, l - 1792);
                    }
                }
                else
                {
                    gameGraphics.DrawPicture(0, 10, baseInventoryPic + 10);
                    if (l > 2816)
                    {
                        gameGraphics.DrawPicture(0, 10, baseLoginScreenBackgroundPic, l - 2816);
                    }
                }
            }
            if (loginMenuFirst == null)
            {
                return;
            }

            if (loginScreenNumber == 0)
            {
                loginMenuFirst.drawMenu();
            }

            if (loginScreenNumber == 1)
            {
                loginNewUser.drawMenu();
            }

            if (loginScreenNumber == 2)
            {
                loginMenuLogin.drawMenu();
            }

            gameGraphics.DrawPicture(0, WindowSize.Height, baseInventoryPic + 22);


            OnDrawDone();
        }

        public void DrawItem(int x, int y, int width, int height, int itemID)
        {
            int picture = entityManager.GetItem(itemID).InventoryPicture + baseItemPicture;
            int mask = entityManager.GetItem(itemID).PictureMask;

            gameGraphics.DrawImage(x, y, width, height, picture, mask, 0, 0, false);
        }

        public ClientMob MakePlayer(int serverIndex, int x, int y, int sprite)
        {
            if (Mobs[serverIndex] == null)
            {
                Mobs[serverIndex] = new ClientMob();
                Mobs[serverIndex].ServerIndex = serverIndex;
                Mobs[serverIndex].ServerId = 0;
            }

            ClientMob existingPlayer = Mobs[serverIndex];

            bool flag = LastPlayers
                .Where(player => player != null) // TODO: Remove this check once it is safe
                .Any(player => player.ServerIndex == serverIndex);

            if (flag)
            {
                existingPlayer.nextSprite = sprite;

                int waypointIndex = existingPlayer.WaypointCurrent;

                if (x != existingPlayer.Waypoints[waypointIndex].X || y != existingPlayer.Waypoints[waypointIndex].Y)
                {
                    existingPlayer.WaypointCurrent = waypointIndex = (waypointIndex + 1) % 10;
                    existingPlayer.Waypoints[waypointIndex].X = x;
                    existingPlayer.Waypoints[waypointIndex].Y = y;
                }
            }
            else
            {
                existingPlayer.ServerIndex = serverIndex;
                existingPlayer.WaypointsEndSprite = 0;
                existingPlayer.WaypointCurrent = 0;
                existingPlayer.Location = new Point2D(x, y);
                existingPlayer.Waypoints[0].X = x;
                existingPlayer.Waypoints[0].Y = y;
                existingPlayer.nextSprite = existingPlayer.currentSprite = sprite;
                existingPlayer.stepCount = 0;
            }

            Players[PlayerCount++] = existingPlayer;

            return existingPlayer;
        }

        public void walkTo1Tile(Point2D location, Point2D destination, bool flag)
        {
            WalkTo(location, destination, destination, false, flag);
        }

        public void LoadConfig()
        {
            entityManager.LoadContent();

            sbyte[] abyte1 = unpackData("filter.jag", "Chat system", 15);
            if (abyte1 == null)
            {
                errorLoading = true;
                return;
            }

            sbyte[] abyte2 = DataOperations.loadData("fragmentsenc.txt", 0, abyte1);
            sbyte[] abyte3 = DataOperations.loadData("badenc.txt", 0, abyte1);
            sbyte[] abyte4 = DataOperations.loadData("hostenc.txt", 0, abyte1);
            sbyte[] abyte5 = DataOperations.loadData("tldlist.txt", 0, abyte1);

            return;
        }

        public override void handleMouseDown(int arg0, int arg1, int arg2)
        {
            mouseTrailX[mouseTrailIndex] = arg1;
            mouseTrailY[mouseTrailIndex] = arg2;
            mouseTrailIndex = mouseTrailIndex + 1 & 0x1fff;
            for (int l = 10; l < 4000; l++)
            {
                int lastMouseTrailIndex = mouseTrailIndex - l & 0x1fff;
                if (mouseTrailX[lastMouseTrailIndex] == arg1 && mouseTrailY[lastMouseTrailIndex] == arg2)
                {
                    bool flag = false;
                    for (int j1 = 1; j1 < l; j1++)
                    {
                        int mouseNew = mouseTrailIndex - j1 & 0x1fff;
                        int mouseOld = lastMouseTrailIndex - j1 & 0x1fff;
                        if (mouseTrailX[mouseOld] != arg1 || mouseTrailY[mouseOld] != arg2)
                        {
                            flag = true;
                        }

                        if (mouseTrailX[mouseNew] != mouseTrailX[mouseOld] || mouseTrailY[mouseNew] != mouseTrailY[mouseOld])
                        {
                            break;
                        }
                    }
                }
            }

        }

        public void drawPrayerMagicMenu(bool canClick)
        {
            int l = gameGraphics.GameSize.Width - 199;
            int i1 = 36;
            gameGraphics.DrawPicture(l - 49, 3, baseInventoryPic + 4);
            int c1 = 196;//'\u304';
            int c2 = 182;//'\u266';
            int k1;
            int j1 = k1 = ColourTranslator.ToArgb(160, 160, 160);

            if (menuMagicPrayersSelected == 0)
            {
                j1 = ColourTranslator.ToArgb(220, 220, 220);
            }
            else
            {
                k1 = ColourTranslator.ToArgb(220, 220, 220);
            }

            gameGraphics.drawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.drawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24, c1, 90, ColourTranslator.ToArgb(220, 220, 220), 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24 + 90, c1, c2 - 90 - 24, ColourTranslator.ToArgb(160, 160, 160), 128);
            gameGraphics.DrawHorizontalLine(l, i1 + 24, c1, 0);
            gameGraphics.DrawVerticalLine(l + c1 / 2, i1, 24, 0);
            gameGraphics.DrawHorizontalLine(l, i1 + 113, c1, 0);
            gameGraphics.DrawText("Magic", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.DrawText("Prayers", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);

            if (menuMagicPrayersSelected == 0)
            {
                spellMenu.clearList(spellMenuHandle);
                int l1 = 0;

                for (int l2 = 0; l2 < entityManager.SpellCount; l2++)
                {
                    string s1 = "@yel@";

                    for (int k4 = 0; k4 < entityManager.GetSpell(l2).RuneCount; k4++)
                    {
                        int runeItemId = entityManager.GetSpell(l2).RequiredRunesIds[k4];
                        int runeCount = entityManager.GetSpell(l2).RequiredRunesCounts[k4];

                        if (combatManager.HasRequiredRunes(runeItemId, runeCount))
                        {
                            continue;
                        }

                        s1 = "@whi@";
                        break;
                    }

                    int k5 = Skills[6].CurrentLevel;

                    if (entityManager.GetSpell(l2).RequiredLevel > k5)
                    {
                        s1 = "@bla@";
                    }

                    spellMenu.addListItem(spellMenuHandle, l1++, s1 + "Level " + entityManager.GetSpell(l2).RequiredLevel + ": " + entityManager.GetSpell(l2).Name);
                }

                spellMenu.drawMenu();
                int l3 = spellMenu.getEntryHighlighted(spellMenuHandle);

                if (l3 != -1)
                {
                    gameGraphics.DrawString("Level " + entityManager.GetSpell(l3).RequiredLevel + ": " + entityManager.GetSpell(l3).Name, l + 2, i1 + 124, 1, 0xffff00);
                    gameGraphics.DrawString(entityManager.GetSpell(l3).Description, l + 2, i1 + 136, 0, 0xffffff);

                    for (int l4 = 0; l4 < entityManager.GetSpell(l3).RuneCount; l4++)
                    {
                        int runeItemId = entityManager.GetSpell(l3).RequiredRunesIds[l4];
                        gameGraphics.DrawPicture(l + 2 + l4 * 44, i1 + 150, baseItemPicture + entityManager.GetItem(runeItemId).InventoryPicture);
                        int i6 = inventoryManager.GetItemTotalCount(runeItemId);
                        int runeCount = entityManager.GetSpell(l3).RequiredRunesCounts[l4];
                        string s3 = "@red@";

                        if (combatManager.HasRequiredRunes(runeItemId, runeCount))
                        {
                            s3 = "@gre@";
                        }

                        gameGraphics.DrawString(s3 + i6 + "/" + runeCount, l + 2 + l4 * 44, i1 + 150, 1, 0xffffff);
                    }

                }
                else
                {
                    gameGraphics.DrawString("Point at a spell for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (menuMagicPrayersSelected == 1)
            {
                spellMenu.clearList(spellMenuHandle);
                int i2 = 0;

                int prayerIndex = 0;

                for (prayerIndex = 0; prayerIndex < entityManager.PrayerCount; prayerIndex++)
                {
                    string s2 = "@whi@";
                    if (entityManager.GetPrayer(prayerIndex).RequiredLevel > Skills[5].BaseLevel)
                    {
                        s2 = "@bla@";
                    }

                    if (prayerOn[prayerIndex])
                    {
                        s2 = "@gre@";
                    }

                    Prayer prayer = entityManager.GetPrayer(prayerIndex);

                    spellMenu.addListItem(spellMenuHandle, i2++, s2 + "Level " + prayer.RequiredLevel + ": " + prayer.Name);
                }

                spellMenu.drawMenu();
                prayerIndex = spellMenu.getEntryHighlighted(spellMenuHandle);

                if (prayerIndex != -1)
                {
                    Prayer prayer = entityManager.GetPrayer(prayerIndex);

                    gameGraphics.DrawText("Level " + prayer.RequiredLevel + ": " + prayer.Name, l + c1 / 2, i1 + 130, 1, 0xffff00);
                    gameGraphics.DrawText(prayer.Description, l + c1 / 2, i1 + 145, 0, 0xffffff);
                    gameGraphics.DrawText("Drain rate: " + prayer.DrainRate, l + c1 / 2, i1 + 160, 1, 0);
                }
                else
                {
                    gameGraphics.DrawString("Point at a prayer for a description", l + 2, i1 + 124, 1, 0);
                }
            }

            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.GameSize.Width - 199);
            i1 = InputManager.Instance.MouseLocation.Y - 36;

            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                spellMenu.mouseClick(l + (gameGraphics.GameSize.Width - 199), i1 + 36, lastMouseButton, mouseButton);

                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98 && menuMagicPrayersSelected == 1)
                    {
                        menuMagicPrayersSelected = 0;
                        spellMenu.switchList(spellMenuHandle);
                    }
                    else if (l > 98 && menuMagicPrayersSelected == 0)
                    {
                        menuMagicPrayersSelected = 1;
                        spellMenu.switchList(spellMenuHandle);
                    }
                }

                if (mouseButtonClick == 1 && menuMagicPrayersSelected == 0)
                {
                    int j2 = spellMenu.getEntryHighlighted(spellMenuHandle);
                    if (j2 != -1)
                    {
                        int magicLevel = Skills[6].CurrentLevel;

                        if (entityManager.GetSpell(j2).RequiredLevel > magicLevel)
                        {
                            DisplayMessage("Your magic ability is not high enough for this spell");
                        }
                        else
                        {
                            int j4;
                            for (j4 = 0; j4 < entityManager.GetSpell(j2).RuneCount; j4++)
                            {
                                int runeItemId = entityManager.GetSpell(j2).RequiredRunesIds[j4];
                                int runeCount = entityManager.GetSpell(j2).RequiredRunesCounts[j4];

                                if (combatManager.HasRequiredRunes(runeItemId, runeCount))
                                {
                                    continue;
                                }

                                DisplayMessage("You don't have all the reagents you need for this spell");
                                j4 = -1;
                                break;
                            }

                            if (j4 == entityManager.GetSpell(j2).RuneCount)
                            {
                                selectedSpell = j2;
                                selectedItem = -1;
                            }
                        }
                    }
                }

                if (mouseButtonClick == 1 && menuMagicPrayersSelected == 1)
                {
                    int k2 = spellMenu.getEntryHighlighted(spellMenuHandle);

                    if (k2 != -1)
                    {
                        int prayerLevel = Skills[5].BaseLevel;

                        if (entityManager.GetPrayer(k2).RequiredLevel > prayerLevel)
                        {
                            DisplayMessage("Your prayer ability is not high enough for this prayer");
                        }
                        else if (Skills[5].CurrentLevel == 0)
                        {
                            DisplayMessage("You have run out of prayer points. Return to a church to recharge");
                        }
                        else if (prayerOn[k2])
                        {
                            StreamClass.CreatePacket(248);
                            StreamClass.AddInt8(k2);
                            StreamClass.FormatPacket();
                            prayerOn[k2] = false;
                        }
                        else
                        {
                            StreamClass.CreatePacket(56);
                            StreamClass.AddInt8(k2);
                            StreamClass.FormatPacket();
                            prayerOn[k2] = true;
                        }
                    }
                }

                mouseButtonClick = 0;
            }
        }

        public override sbyte[] unpackData(string arg0, string arg1, int arg2)
        {
            sbyte[] abyte0 = Link.GetFile(arg0);

            if (abyte0 != null)
            {
                int l = ((abyte0[0] & 0xff) << 16) + ((abyte0[1] & 0xff) << 8) + (abyte0[2] & 0xff);
                int i1 = ((abyte0[3] & 0xff) << 16) + ((abyte0[4] & 0xff) << 8) + (abyte0[5] & 0xff);

                sbyte[] abyte1 = new sbyte[abyte0.Length - 6];
                for (int j1 = 0; j1 < abyte0.Length - 6; j1++)
                {
                    abyte1[j1] = abyte0[j1 + 6];
                }

                drawLoadingBarText(arg2, "Unpacking " + arg1);

                if (i1 != l)
                {
                    sbyte[] abyte2 = new sbyte[l];
                    DataFileDecrypter.unpackData(abyte2, l, abyte1, i1, 0);
                    OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Unpacking " + arg1, arg2));
                    return abyte2;
                }

                OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Unpacking " + arg1, arg2));
                return abyte1;
            }

            OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Unpacking " + arg1, arg2));
            return base.unpackData(arg0, arg1, arg2);
        }

        delegate void SendPingPacketDelegate();
        readonly object _sync = new object();
        public static bool sendingPing;

        public void sendPingPacketAsync()
        {
            SendPingPacketDelegate worker = new SendPingPacketDelegate(SendPing);
            AsyncCallback completedCallback = new AsyncCallback(sendPingPacketCompletedCallback);

            lock (_sync)
            {
                if (sendingPing)
                {
                    return;
                }

                AsyncOperation async1 = AsyncOperationManager.CreateOperation(null);
                worker.BeginInvoke(completedCallback, async1);
                sendingPing = true;
            }
        }

        public event AsyncCompletedEventHandler MyTaskCompleted;

        protected virtual void OnMyTaskCompleted(AsyncCompletedEventArgs e)
        {
            MyTaskCompleted?.Invoke(this, e);
        }

        void sendPingPacketCompletedCallback(IAsyncResult ar)
        {
            // get the original worker delegate and the AsyncOperation instance
            SendPingPacketDelegate worker =
              (SendPingPacketDelegate)((AsyncResult)ar).AsyncDelegate;
            AsyncOperation async1 = (AsyncOperation)ar.AsyncState;

            // finish the asynchronous operation
            worker.EndInvoke(ar);

            // clear the running task flag
            lock (_sync)
            {
                sendingPing = false;
            }

            // raise the completed event
            AsyncCompletedEventArgs completedArgs = new AsyncCompletedEventArgs(null,
              false, null);
            async1.PostOperationCompleted(
              delegate (object e) { OnMyTaskCompleted((AsyncCompletedEventArgs)e); },
              completedArgs);
        }

        public void checkGameInputs()
        {
            sendPingPacketAsync();

            if (ShowAppearanceWindow)
            {
                updateAppearanceWindow();
                return;
            }

            for (int playerIndex = 0; playerIndex < PlayerCount; playerIndex++)
            {
                ClientMob player = Players[playerIndex];
                int j1 = (player.WaypointCurrent + 1) % 10;

                if (player.WaypointsEndSprite != j1)
                {
                    int direction = -1;
                    int targetSprite = player.WaypointsEndSprite;
                    int i5;

                    if (targetSprite < j1)
                    {
                        i5 = j1 - targetSprite;
                    }
                    else
                    {
                        i5 = (10 + j1) - targetSprite;
                    }

                    int i6 = 4;
                    if (i5 > 2)
                    {
                        i6 = (i5 - 1) * 4;
                    }

                    if (player.Waypoints[targetSprite].X - player.Location.X > GridSize * 3 ||
                        player.Waypoints[targetSprite].Y - player.Location.Y > GridSize * 3 ||
                        player.Waypoints[targetSprite].X - player.Location.X < -GridSize * 3 ||
                        player.Waypoints[targetSprite].Y - player.Location.Y < -GridSize * 3 ||
                        i5 > 8)
                    {
                        player.Location = new Point2D(
                            player.Waypoints[targetSprite].X,
                            player.Waypoints[targetSprite].Y);
                    }
                    else
                    {
                        if (player.Location.X < player.Waypoints[targetSprite].X)
                        {
                            player.Location = new Point2D(player.Location.X + i6, player.Location.Y);
                            player.stepCount++;
                            direction = 2;
                        }
                        else if (player.Location.X > player.Waypoints[targetSprite].X)
                        {
                            player.Location = new Point2D(player.Location.X - i6, player.Location.Y);
                            player.stepCount++;
                            direction = 6;
                        }

                        if (player.Location.X - player.Waypoints[targetSprite].X < i6 &&
                            player.Location.X - player.Waypoints[targetSprite].X > -i6)
                        {
                            player.Location = new Point2D(player.Waypoints[targetSprite].X, player.Location.Y);
                        }

                        if (player.Location.Y < player.Waypoints[targetSprite].Y)
                        {
                            player.Location = new Point2D(player.Location.X, player.Location.Y + i6);
                            player.stepCount++;

                            if (direction == -1)
                            {
                                direction = 4;
                            }
                            else if (direction == 2)
                            {
                                direction = 3;
                            }
                            else
                            {
                                direction = 5;
                            }
                        }
                        else if (player.Location.Y > player.Waypoints[targetSprite].Y)
                        {
                            player.Location = new Point2D(player.Location.X, player.Location.Y - i6);
                            player.stepCount++;

                            if (direction == -1)
                            {
                                direction = 0;
                            }
                            else if (direction == 2)
                            {
                                direction = 1;
                            }
                            else
                            {
                                direction = 7;
                            }
                        }
                        if (player.Location.Y - player.Waypoints[targetSprite].Y < i6 &&
                            player.Location.Y - player.Waypoints[targetSprite].Y > -i6)
                        {
                            player.Location = new Point2D(player.Location.X, player.Waypoints[targetSprite].Y);
                        }
                    }

                    if (direction != -1)
                    {
                        player.currentSprite = direction;
                    }

                    if (player.Location.X == player.Waypoints[targetSprite].X &&
                        player.Location.Y == player.Waypoints[targetSprite].Y)
                    {
                        player.WaypointsEndSprite = (targetSprite + 1) % 10;
                    }
                }
                else
                {
                    player.currentSprite = player.nextSprite;
                }

                if (player.lastMessageTimeout > 0)
                {
                    player.lastMessageTimeout--;
                }

                if (player.PlayerSkullTimeout > 0)
                {
                    player.PlayerSkullTimeout--;
                }

                if (player.combatTimer > 0)
                {
                    player.combatTimer--;
                }

                if (PlayerAliveTimeout > 0)
                {
                    PlayerAliveTimeout--;

                    if (PlayerAliveTimeout == 0)
                    {
                        DisplayMessage("You have been granted another life. Be more careful this time!");
                    }

                    if (PlayerAliveTimeout == 0)
                    {
                        DisplayMessage("You retain your skills. Your objects land where you died");
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < NpcCount; npcIndex++)
            {
                ClientMob npc = Npcs[npcIndex];
                int i2 = (npc.WaypointCurrent + 1) % 10;

                if (npc.WaypointsEndSprite != i2)
                {
                    int l3 = -1;
                    int waypointIndex = npc.WaypointsEndSprite;
                    int j6;

                    if (waypointIndex < i2)
                    {
                        j6 = i2 - waypointIndex;
                    }
                    else
                    {
                        j6 = (10 + i2) - waypointIndex;
                    }

                    int k6 = 4;

                    if (j6 > 2)
                    {
                        k6 = (j6 - 1) * 4;
                    }

                    if (npc.Waypoints[waypointIndex].X - npc.Location.X > GridSize * 3 ||
                        npc.Waypoints[waypointIndex].Y - npc.Location.Y > GridSize * 3 ||
                        npc.Waypoints[waypointIndex].X - npc.Location.X < -GridSize * 3 ||
                        npc.Waypoints[waypointIndex].Y - npc.Location.Y < -GridSize * 3 || j6 > 8)
                    {
                        npc.Location = new Point2D(
                            npc.Waypoints[waypointIndex].X,
                            npc.Waypoints[waypointIndex].Y);
                    }
                    else
                    {
                        if (npc.Location.X < npc.Waypoints[waypointIndex].X)
                        {
                            npc.Location = new Point2D(npc.Location.X + k6, npc.Location.Y);
                            npc.stepCount++;
                            l3 = 2;
                        }
                        else if (npc.Location.X > npc.Waypoints[waypointIndex].X)
                        {
                            npc.Location = new Point2D(npc.Location.X - k6, npc.Location.Y);
                            npc.stepCount++;
                            l3 = 6;
                        }

                        if (npc.Location.X - npc.Waypoints[waypointIndex].X < k6 &&
                            npc.Location.X - npc.Waypoints[waypointIndex].X > -k6)
                        {
                            npc.Location = new Point2D(npc.Waypoints[waypointIndex].X, npc.Location.Y);
                        }

                        if (npc.Location.Y < npc.Waypoints[waypointIndex].Y)
                        {
                            npc.Location = new Point2D(npc.Location.X, npc.Location.Y + k6);
                            npc.stepCount++;

                            if (l3 == -1)
                            {
                                l3 = 4;
                            }
                            else if (l3 == 2)
                            {
                                l3 = 3;
                            }
                            else
                            {
                                l3 = 5;
                            }
                        }
                        else if (npc.Location.Y > npc.Waypoints[waypointIndex].Y)
                        {
                            npc.Location = new Point2D(npc.Location.X, npc.Location.Y - k6);
                            npc.stepCount++;

                            if (l3 == -1)
                            {
                                l3 = 0;
                            }
                            else if (l3 == 2)
                            {
                                l3 = 1;
                            }
                            else
                            {
                                l3 = 7;
                            }
                        }

                        if (npc.Location.Y - npc.Waypoints[waypointIndex].Y < k6 &&
                            npc.Location.Y - npc.Waypoints[waypointIndex].Y > -k6)
                        {
                            npc.Location = new Point2D(npc.Location.X, npc.Waypoints[waypointIndex].Y);
                        }
                    }
                    if (l3 != -1)
                    {
                        npc.currentSprite = l3;
                    }

                    if (npc.Location.X == npc.Waypoints[waypointIndex].X &&
                        npc.Location.Y == npc.Waypoints[waypointIndex].Y)
                    {
                        npc.WaypointsEndSprite = (waypointIndex + 1) % 10;
                    }
                }
                else
                {
                    npc.currentSprite = npc.nextSprite;
                    if (npc.npcId == 43)
                    {
                        npc.stepCount++;
                    }
                }
                if (npc.lastMessageTimeout > 0)
                {
                    npc.lastMessageTimeout--;
                }

                if (npc.PlayerSkullTimeout > 0)
                {
                    npc.PlayerSkullTimeout--;
                }

                if (npc.combatTimer > 0)
                {
                    npc.combatTimer--;
                }
            }

            if (drawMenuTab != 2)
            {
                if (GraphicsEngine.bnn > 0)
                {
                    sleepWordDelayTimer++;
                }

                if (GraphicsEngine.caa > 0)
                {
                    sleepWordDelayTimer = 0;
                }

                GraphicsEngine.bnn = 0;
                GraphicsEngine.caa = 0;
            }
            for (int k1 = 0; k1 < PlayerCount; k1++)
            {
                ClientMob f3 = Players[k1];
                if (f3.ProjectileDistance > 0)
                {
                    f3.ProjectileDistance--;
                }
            }

            if (cameraAutoAngleDebug)
            {
                if (cameraAutoRotatePlayerX - CurrentPlayer.Location.X < -500 ||
                    cameraAutoRotatePlayerX - CurrentPlayer.Location.X > 500 ||
                    cameraAutoRotatePlayerY - CurrentPlayer.Location.Y < -500 ||
                    cameraAutoRotatePlayerY - CurrentPlayer.Location.Y > 500)
                {
                    cameraAutoRotatePlayerX = CurrentPlayer.Location.X;
                    cameraAutoRotatePlayerY = CurrentPlayer.Location.Y;
                }
            }
            else
            {
                if (cameraAutoRotatePlayerX - CurrentPlayer.Location.X < -500 ||
                    cameraAutoRotatePlayerX - CurrentPlayer.Location.X > 500 ||
                    cameraAutoRotatePlayerY - CurrentPlayer.Location.Y < -500 ||
                    cameraAutoRotatePlayerY - CurrentPlayer.Location.Y > 500)
                {
                    cameraAutoRotatePlayerX = CurrentPlayer.Location.X;
                    cameraAutoRotatePlayerY = CurrentPlayer.Location.Y;
                }

                if (cameraAutoRotatePlayerX != CurrentPlayer.Location.X)
                {
                    cameraAutoRotatePlayerX += (CurrentPlayer.Location.X - cameraAutoRotatePlayerX) / (16 + (cameraDistance - 500) / 15);
                }

                if (cameraAutoRotatePlayerY != CurrentPlayer.Location.Y)
                {
                    cameraAutoRotatePlayerY += (CurrentPlayer.Location.Y - cameraAutoRotatePlayerY) / (16 + (cameraDistance - 500) / 15);
                }

                if (SettingsManager.Instance.CameraAutoAngle)
                {
                    int j2 = cameraAutoAngle * 32;
                    int i4 = j2 - cameraRotation;
                    int byte0 = 1;

                    if (i4 != 0)
                    {
                        cameraAutoRotationAmount++;
                        if (i4 > 128)
                        {
                            byte0 = -1;
                            i4 = 256 - i4;
                        }
                        else if (i4 > 0)
                        {
                            byte0 = 1;
                        }
                        else if (i4 < -128)
                        {
                            byte0 = 1;
                            i4 = 256 + i4;
                        }
                        else if (i4 < 0)
                        {
                            byte0 = -1;
                            i4 = -i4;
                        }

                        cameraRotation += ((cameraAutoRotationAmount * i4 + 255) / 256) * byte0;
                        cameraRotation &= 0xff;
                    }
                    else
                    {
                        cameraAutoRotationAmount = 0;
                    }
                }
            }

            if (sleepWordDelayTimer > 20)
            {
                sleepWordDelay = false;
                sleepWordDelayTimer = 0;
            }

            if (IsSleeping)
            {
                if (lastMouseButton == 1 && InputManager.Instance.MouseLocation.Y > 275 && InputManager.Instance.MouseLocation.Y < 310 && InputManager.Instance.MouseLocation.X > 56 && InputManager.Instance.MouseLocation.X < 456)
                {
                    StreamClass.CreatePacket(200);
                    StreamClass.AddString("-null-");

                    if (!sleepWordDelay)
                    {
                        StreamClass.AddInt8(0);
                        sleepWordDelay = true;
                    }

                    StreamClass.FormatPacket();
                    sleepingStatusText = "Please wait...";
                }

                lastMouseButton = 0;
                return;
            }

            if (PlayerAliveTimeout != 0)
            {
                lastMouseButton = 0;
            }

            mouseButtonHeldTime = 0; ;

            if (lastMouseButton == 1)
            {
                mouseButtonClick = 1;
            }
            else if (lastMouseButton == 2)
            {
                mouseButtonClick = 2;
            }

            gameCamera.setMousePosition(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y);
            lastMouseButton = 0;

            if (SettingsManager.Instance.CameraAutoAngle)
            {
                if (cameraAutoRotationAmount == 0 || cameraAutoAngleDebug)
                {
                    if (InputManager.Instance.IsKeyDown(Keys.Left))
                    {
                        cameraAutoAngle = cameraAutoAngle + 1 & 7;

                        if (!cameraZoom)
                        {
                            if ((cameraAutoAngle & 1) == 0)
                            {
                                cameraAutoAngle = cameraAutoAngle + 1 & 7;
                            }

                            for (int l2 = 0; l2 < 8; l2++)
                            {
                                if (validCameraAngle(cameraAutoAngle))
                                {
                                    break;
                                }

                                cameraAutoAngle = cameraAutoAngle + 1 & 7;
                            }
                        }
                    }
                    if (InputManager.Instance.IsKeyDown(Keys.Right))
                    {
                        cameraAutoAngle = cameraAutoAngle + 7 & 7;

                        if (!cameraZoom)
                        {
                            if ((cameraAutoAngle & 1) == 0)
                            {
                                cameraAutoAngle = cameraAutoAngle + 7 & 7;
                            }

                            for (int i3 = 0; i3 < 8; i3++)
                            {
                                if (validCameraAngle(cameraAutoAngle))
                                {
                                    break;
                                }

                                cameraAutoAngle = cameraAutoAngle + 7 & 7;
                            }
                        }
                    }
                }
            }
            else if (InputManager.Instance.IsKeyDown(Keys.Left))
            {
                cameraRotation = cameraRotation + 2 & 0xff;
            }
            else if (InputManager.Instance.IsKeyDown(Keys.Right))
            {
                cameraRotation = cameraRotation - 2 & 0xff;
            }

            if (InputManager.Instance.IsKeyDown(Keys.Up) && cameraDistance > 550)
            {
                cameraDistance -= 4;
            }
            else if (InputManager.Instance.IsKeyDown(Keys.Down) && cameraDistance < 1250)
            {
                cameraDistance += 4;
            }

            if (SettingsManager.Instance.GraphicsSettings.FogOfWar)
            {
                if ((cameraZoom && cameraDistance > 550) || cameraDistance > 750)
                {
                    cameraDistance -= 4;
                }

                if (!cameraZoom && cameraDistance < 750)
                {
                    cameraDistance += 4;
                }
            }

            if (actionPictureType > 0)
            {
                actionPictureType--;
            }
            else if (actionPictureType < 0)
            {
                actionPictureType++;
            }

            gameCamera.updateLightning(17);
            modelUpdatingTimer++;

            if (modelUpdatingTimer > 5)
            {
                modelUpdatingTimer = 0;
                modelFireLightningSpellNumber = (modelFireLightningSpellNumber + 1) % 3;
                modelTorchNumber = (modelTorchNumber + 1) % 4;
                modelClawSpellNumber = (modelClawSpellNumber + 1) % 5;
            }

            for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
            {
                if (ObjectLocations[objectIndex].X >= 0 &&
                    ObjectLocations[objectIndex].Y >= 0 &&
                    ObjectLocations[objectIndex].X < 96 &&
                    ObjectLocations[objectIndex].Y < 96 &&
                    ObjectType[objectIndex] == 74)
                {
                    Point3D off = new Point3D(1, 0, 0);
                    ObjectArray[objectIndex].offsetMiniPosition(off);
                }
            }

            for (int teleBubbleIndex = 0; teleBubbleIndex < teleBubbleCount; teleBubbleIndex++)
            {
                teleBubbleTime[teleBubbleIndex]++;

                if (teleBubbleTime[teleBubbleIndex] > 50)
                {
                    teleBubbleCount--;

                    for (int l5 = teleBubbleIndex; l5 < teleBubbleCount; l5++)
                    {
                        teleBubbleX[l5] = teleBubbleX[l5 + 1];
                        teleBubbleY[l5] = teleBubbleY[l5 + 1];
                        teleBubbleTime[l5] = teleBubbleTime[l5 + 1];
                        teleBubbleType[l5] = teleBubbleType[l5 + 1];
                    }
                }
            }
        }

        public void createAppearanceWindow()
        {
            appearanceMenu = new Menu(gameGraphics, 100);
            appearanceMenu.drawText(256, 10, "Please design Your Character", 4, true);
            int l = 140;
            int i1 = 34;
            l += 116;
            i1 -= 10;
            appearanceMenu.drawText(l - 55, i1 + 110, "Front", 3, true);
            appearanceMenu.drawText(l, i1 + 110, "Side", 3, true);
            appearanceMenu.drawText(l + 55, i1 + 110, "Back", 3, true);
            sbyte byte0 = 54;
            i1 += 145;
            appearanceMenu.drawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.drawText(l - byte0, i1 - 8, "Head", 1, true);
            appearanceMenu.drawText(l - byte0, i1 + 8, "Type", 1, true);
            appearanceMenu.drawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceHeadLeftArrow = appearanceMenu.createButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.drawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceHeadRightArrow = appearanceMenu.createButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.drawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.drawText(l + byte0, i1 - 8, "Hair", 1, true);
            appearanceMenu.drawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.drawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceHairLeftArrow = appearanceMenu.createButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.drawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceHairRightArrow = appearanceMenu.createButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            appearanceMenu.drawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.drawText(l - byte0, i1, "Gender", 1, true);
            appearanceMenu.drawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceGenderLeftArrow = appearanceMenu.createButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.drawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceGenderRightArrow = appearanceMenu.createButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.drawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.drawText(l + byte0, i1 - 8, "Top", 1, true);
            appearanceMenu.drawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.drawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceTopLeftArrow = appearanceMenu.createButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.drawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceTopRightArrow = appearanceMenu.createButton(l + byte0 + 40, i1, 20, 20);
            i1 += 50;
            appearanceMenu.drawCurvedBox(l - byte0, i1, 53, 41);
            appearanceMenu.drawText(l - byte0, i1 - 8, "Skin", 1, true);
            appearanceMenu.drawText(l - byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.drawArrow(l - byte0 - 40, i1, Menu.baseScrollPic + 7);
            appearanceSkinLeftArrow = appearanceMenu.createButton(l - byte0 - 40, i1, 20, 20);
            appearanceMenu.drawArrow((l - byte0) + 40, i1, Menu.baseScrollPic + 6);
            appearanceSkingRightArrow = appearanceMenu.createButton((l - byte0) + 40, i1, 20, 20);
            appearanceMenu.drawCurvedBox(l + byte0, i1, 53, 41);
            appearanceMenu.drawText(l + byte0, i1 - 8, "Bottom", 1, true);
            appearanceMenu.drawText(l + byte0, i1 + 8, "Color", 1, true);
            appearanceMenu.drawArrow((l + byte0) - 40, i1, Menu.baseScrollPic + 7);
            appearanceBottomLeftArrow = appearanceMenu.createButton((l + byte0) - 40, i1, 20, 20);
            appearanceMenu.drawArrow(l + byte0 + 40, i1, Menu.baseScrollPic + 6);
            appearanceBottomRightArrow = appearanceMenu.createButton(l + byte0 + 40, i1, 20, 20);
            i1 += 82;
            i1 -= 35;
            appearanceMenu.drawButton(l, i1, 200, 30);
            appearanceMenu.drawText(l, i1, "Accept", 4, false);
            appearanceAcceptButton = appearanceMenu.createButton(l, i1, 200, 30);
        }

        public override void HandleKeyDown(Keys key, char c)
        {
            if (key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down)
            {
                return;
            }

            if (loggedIn)
            {
                if (ShowAppearanceWindow && appearanceMenu != null)
                {
                    appearanceMenu.keyPress(key, c);
                }
            }
            else
            {
                if (loginScreenNumber == 0 && loginMenuFirst != null)
                {
                    loginMenuFirst.keyPress(key, c);
                }

                if (loginScreenNumber == 1 && loginNewUser != null)
                {
                    loginNewUser.keyPress(key, c);
                }

                if (loginScreenNumber == 2 && loginMenuLogin != null)
                {
                    loginMenuLogin.keyPress(key, c);
                }
            }
        }

        public void generateWorldRightClickMenu()
        {
            int l = 2203 - (SectionLocation.Y + WildLocation.Y + AreaLocation.Y);
            if (SectionLocation.X + WildLocation.X + AreaLocation.X >= 2640)
            {
                l = -50;
            }

            int ground = -1;
            for (int j1 = 0; j1 < ObjectCount; j1++)
            {
                objectAlreadyInMenu[j1] = false;
            }

            for (int k1 = 0; k1 < WallObjectCount; k1++)
            {
                WallObjectAlreadyInMenu[k1] = false;
            }

            int optionCount = gameCamera.getOptionCount();
            ObjectModel[] objects = gameCamera.getHighlightedObjects();
            int[] players = gameCamera.getHighlightedPlayers();
            for (int i2 = 0; i2 < optionCount; i2++)
            {
                if (menuOptionsCount > 200)
                {
                    break;
                }

                int player = players[i2];
                ObjectModel _obj = objects[i2];

                if (_obj.entityType[player] <= 65535 || _obj.entityType[player] >= 0x30d40 && _obj.entityType[player] <= 0x493e0)
                {
                    if (_obj == gameCamera.highlightedObject)
                    {
                        int index = _obj.entityType[player] % 10000;
                        int type = _obj.entityType[player] / 10000;

                        if (type == 2)
                        {
                            if (selectedSpell >= 0)
                            {
                                if (entityManager.GetSpell(selectedSpell).Type == 3)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@lre@" + entityManager.GetItem(GroundItemId[index]).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnGroundItem;
                                    MenuActionLocations[menuOptionsCount] = GroundItemLocations[index];
                                    menuActionType[menuOptionsCount] = GroundItemId[index];
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@lre@" + entityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithGroundItem;
                                MenuActionLocations[menuOptionsCount] = GroundItemLocations[index];
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Take";
                                menuText2[menuOptionsCount] = "@lre@" + entityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.TakeItem;
                                MenuActionLocations[menuOptionsCount] = GroundItemLocations[index];
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuOptionsCount++;
                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@lre@" + entityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineGroundItem;
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuOptionsCount++;
                            }
                        }
                        else if (type == 3)
                        {
                            string s2 = "";
                            int l4 = -1;
                            int id = Npcs[index].npcId;
                            if (entityManager.GetNpc(id).IsAttackable > 0)
                            {
                                int j5 = (entityManager.GetNpc(id).AttackLevel + entityManager.GetNpc(id).DefenceLevel + entityManager.GetNpc(id).StrengthLevel + entityManager.GetNpc(id).HealthLevel) / 4;
                                int k5 = (Skills[0].BaseLevel + Skills[1].BaseLevel + Skills[2].BaseLevel + Skills[3].BaseLevel + 27) / 4;
                                l4 = k5 - j5;
                                s2 = "@yel@";
                                if (l4 < 0)
                                {
                                    s2 = "@or1@";
                                }

                                if (l4 < -3)
                                {
                                    s2 = "@or2@";
                                }

                                if (l4 < -6)
                                {
                                    s2 = "@or3@";
                                }

                                if (l4 < -9)
                                {
                                    s2 = "@red@";
                                }

                                if (l4 > 0)
                                {
                                    s2 = "@gr1@";
                                }

                                if (l4 > 3)
                                {
                                    s2 = "@gr2@";
                                }

                                if (l4 > 6)
                                {
                                    s2 = "@gr3@";
                                }

                                if (l4 > 9)
                                {
                                    s2 = "@gre@";
                                }

                                s2 = " " + s2 + "(level-" + j5 + ")";
                            }

                            if (selectedSpell >= 0)
                            {
                                if (entityManager.GetSpell(selectedSpell).Type == 2)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnNpc;
                                    MenuActionLocations[menuOptionsCount] = Npcs[index].Location;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithNpc;
                                MenuActionLocations[menuOptionsCount] = Npcs[index].Location;
                                menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (entityManager.GetNpc(id).IsAttackable > 0)
                                {
                                    menuText1[menuOptionsCount] = "Attack";
                                    menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name + s2;
                                    if (l4 >= 0)
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackNpc;
                                    }
                                    else
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackNpc2;
                                    }

                                    MenuActionLocations[menuOptionsCount] = Npcs[index].Location;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Talk-to";
                                menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.TalkToNpc;
                                MenuActionLocations[menuOptionsCount] = Npcs[index].Location;
                                menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                menuOptionsCount++;

                                if (!entityManager.GetNpc(id).Command.Equals(""))
                                {
                                    menuText1[menuOptionsCount] = entityManager.GetNpc(id).Command;
                                    menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CommandOnNpc;
                                    MenuActionLocations[menuOptionsCount] = Npcs[index].Location;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@yel@" + entityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineNpc;
                                menuActionType[menuOptionsCount] = Npcs[index].npcId;
                                menuOptionsCount++;
                            }
                        }
                    }
                    else if (_obj != null && _obj.index >= 10000)
                    {
                        int wallObjectIndex = _obj.index - 10000;
                        int i4 = WallObjectId[wallObjectIndex];

                        if (!WallObjectAlreadyInMenu[wallObjectIndex])
                        {
                            if (selectedSpell >= 0)
                            {
                                if (entityManager.GetSpell(selectedSpell).Type == 4)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnWallObject;
                                    MenuActionLocations[menuOptionsCount] = WallObjectLocations[wallObjectIndex];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[wallObjectIndex];
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWallObject(i4).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithWallObject;
                                MenuActionLocations[menuOptionsCount] = WallObjectLocations[wallObjectIndex];
                                menuActionType[menuOptionsCount] = WallObjectDirection[wallObjectIndex];
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (!entityManager.GetWallObject(i4).Command1.ToLower().Equals("WalkTo"))
                                {
                                    menuText1[menuOptionsCount] = entityManager.GetWallObject(i4).Command1;
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command1OnWallObject;
                                    MenuActionLocations[menuOptionsCount] = WallObjectLocations[wallObjectIndex];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[wallObjectIndex];
                                    menuOptionsCount++;
                                }

                                if (!entityManager.GetWallObject(i4).Command2.ToLower().Equals("Examine"))
                                {
                                    menuText1[menuOptionsCount] = entityManager.GetWallObject(i4).Command2;
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command2OnWallObject;
                                    MenuActionLocations[menuOptionsCount] = WallObjectLocations[wallObjectIndex];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[wallObjectIndex];
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWallObject(i4).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineWallObject;
                                menuActionType[menuOptionsCount] = i4;
                                menuOptionsCount++;
                            }
                            WallObjectAlreadyInMenu[wallObjectIndex] = true;
                        }
                    }
                    else if (_obj != null && _obj.index >= 0)
                    {
                        int objectLocation = _obj.index;
                        int j4 = ObjectType[objectLocation];

                        if (!objectAlreadyInMenu[objectLocation])
                        {
                            if (selectedSpell >= 0)
                            {
                                if (entityManager.GetSpell(selectedSpell).Type == 5)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnModel;
                                    MenuActionLocations[menuOptionsCount] = ObjectLocations[objectLocation];
                                    menuActionType[menuOptionsCount] = ObjectRotation[objectLocation];
                                    menuActionVar1[menuOptionsCount] = ObjectType[objectLocation];
                                    menuActionVar2[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWorldObject(j4).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithModel;
                                MenuActionLocations[menuOptionsCount] = ObjectLocations[objectLocation];
                                menuActionType[menuOptionsCount] = ObjectRotation[objectLocation];
                                menuActionVar1[menuOptionsCount] = ObjectType[objectLocation];
                                menuActionVar2[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (!entityManager.GetWorldObject(j4).Command1.ToLower().Equals("WalkTo"))
                                {
                                    menuText1[menuOptionsCount] = entityManager.GetWorldObject(j4).Command1;
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command1OnModel;
                                    MenuActionLocations[menuOptionsCount] = ObjectLocations[objectLocation];
                                    menuActionType[menuOptionsCount] = ObjectRotation[objectLocation];
                                    menuActionVar1[menuOptionsCount] = ObjectType[objectLocation];
                                    menuOptionsCount++;
                                }

                                if (!entityManager.GetWorldObject(j4).Command2.ToLower().Equals("Examine"))
                                {
                                    menuText1[menuOptionsCount] = entityManager.GetWorldObject(j4).Command2;
                                    menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command2OnModel;
                                    MenuActionLocations[menuOptionsCount] = ObjectLocations[objectLocation];
                                    menuActionType[menuOptionsCount] = ObjectRotation[objectLocation];
                                    menuActionVar1[menuOptionsCount] = ObjectType[objectLocation];
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@cya@" + entityManager.GetWorldObject(j4).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineModel;
                                menuActionType[menuOptionsCount] = j4;
                                menuOptionsCount++;
                            }

                            objectAlreadyInMenu[objectLocation] = true;
                        }
                    }
                    else
                    {
                        if (player >= 0)
                        {
                            player = _obj.entityType[player] - 0x30d40;
                        }

                        if (player >= 0)
                        {
                            ground = player;
                        }
                    }
                }
            }

            if (selectedSpell >= 0 && entityManager.GetSpell(selectedSpell).Type <= 1)
            {
                menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on self";
                menuText2[menuOptionsCount] = "";
                menuActions[menuOptionsCount] = MenuAction.CastSpellOnSelf;
                menuActionType[menuOptionsCount] = selectedSpell;
                menuOptionsCount++;
            }

            if (ground != -1)
            {
                if (selectedSpell >= 0)
                {
                    if (entityManager.GetSpell(selectedSpell).Type == 6)
                    {
                        menuText1[menuOptionsCount] = "Cast " + entityManager.GetSpell(selectedSpell).Name + " on ground";
                        menuText2[menuOptionsCount] = "";
                        menuActions[menuOptionsCount] = MenuAction.CastSpellOnGround;
                        MenuActionLocations[menuOptionsCount] = new Point2D(engineHandle.selectedX[ground], engineHandle.selectedY[ground]);
                        menuActionType[menuOptionsCount] = selectedSpell;
                        menuOptionsCount++;
                        return;
                    }
                }
                else if (selectedItem < 0)
                {
                    menuText1[menuOptionsCount] = "Walk here";
                    menuText2[menuOptionsCount] = "";
                    menuActions[menuOptionsCount] = MenuAction.WalkHere;
                    MenuActionLocations[menuOptionsCount] = new Point2D(engineHandle.selectedX[ground], engineHandle.selectedY[ground]);
                    menuOptionsCount++;
                }
            }
        }

        public void drawShopBox()
        {
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                int l = InputManager.Instance.MouseLocation.X - 52;
                int i1 = InputManager.Instance.MouseLocation.Y - 44;
                if (l >= 0 && i1 >= 12 && l < 408 && i1 < 246)
                {
                    int j1 = 0;
                    for (int l1 = 0; l1 < 5; l1++)
                    {
                        for (int l2 = 0; l2 < 8; l2++)
                        {
                            int k3 = 7 + l2 * 49;
                            int k4 = 28 + l1 * 34;
                            if (l > k3 && l < k3 + 49 && i1 > k4 && i1 < k4 + 34 && shopItems[j1] != -1)
                            {
                                selectedShopItemIndex = j1;
                                selectedShopItemType = shopItems[j1];
                            }
                            j1++;
                        }

                    }

                    if (selectedShopItemIndex >= 0)
                    {
                        int itemIde = shopItems[selectedShopItemIndex];
                        if (itemIde != -1)
                        {
                            if (shopItemCount[selectedShopItemIndex] > 0 && l > 298 && i1 >= 204 && l < 408 && i1 <= 215)
                            {
                                StreamClass.CreatePacket(128);
                                StreamClass.AddInt16(shopItems[selectedShopItemIndex]);
                                StreamClass.AddInt32(shopItemBuyPrice[selectedShopItemIndex]);
                                StreamClass.FormatPacket();
                            }
                            if (inventoryManager.GetItemTotalCount(itemIde) > 0 && l > 2 && i1 >= 229 && l < 112 && i1 <= 240)
                            {
                                StreamClass.CreatePacket(255);
                                StreamClass.AddInt16(shopItems[selectedShopItemIndex]);
                                StreamClass.AddInt32(shopItemSellPrice[selectedShopItemIndex]);
                                StreamClass.FormatPacket();
                            }
                        }
                    }
                }
                else
                {
                    StreamClass.CreatePacket(253);
                    StreamClass.FormatPacket();
                    ShowShopBox = false;
                    return;
                }
            }
            sbyte _offsetX = 52;
            sbyte _offsetY = 44;
            gameGraphics.DrawBox(_offsetX, _offsetY, 408, 12, 192);
            int k1 = 0x989898;
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 12, 408, 17, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 29, 8, 170, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX + 399, _offsetY + 29, 9, 170, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 199, 408, 47, k1, 160);
            gameGraphics.DrawString("Buying and selling items", _offsetX + 1, _offsetY + 10, 1, 0xffffff);
            int i2 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > _offsetX + 320 && InputManager.Instance.MouseLocation.Y >= _offsetY && InputManager.Instance.MouseLocation.X < _offsetX + 408 && InputManager.Instance.MouseLocation.Y < _offsetY + 12)
            {
                i2 = 0xff0000;
            }

            gameGraphics.DrawLabel("Close window", _offsetX + 406, _offsetY + 10, 1, i2);
            gameGraphics.DrawString("Shops stock in green", _offsetX + 2, _offsetY + 24, 1, 65280);
            gameGraphics.DrawString("Number you own in blue", _offsetX + 135, _offsetY + 24, 1, 65535);
            gameGraphics.DrawString("Your money: " + inventoryManager.GetItemTotalCount(10) + "gp", _offsetX + 280, _offsetY + 24, 1, 0xffff00);
            int j3 = 0xd0d0d0;
            int j4 = 0;
            for (int boxRow = 0; boxRow < 5; boxRow++)
            {
                for (int boxRowColumn = 0; boxRowColumn < 8; boxRowColumn++)
                {
                    int i6 = _offsetX + 7 + boxRowColumn * 49;
                    int l6 = _offsetY + 28 + boxRow * 34;
                    if (selectedShopItemIndex == j4)
                    {
                        gameGraphics.drawBoxAlpha(i6, l6, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        gameGraphics.drawBoxAlpha(i6, l6, 49, 34, j3, 160);
                    }

                    gameGraphics.DrawBoxEdge(i6, l6, 50, 35, 0);
                    if (shopItems[j4] != -1)
                    {
                        gameGraphics.DrawImage(i6, l6, 48, 32, baseItemPicture + entityManager.GetItem(shopItems[j4]).InventoryPicture, entityManager.GetItem(shopItems[j4]).PictureMask, 0, 0, false);
                        gameGraphics.DrawString(shopItemCount[j4].ToString(), i6 + 1, l6 + 10, 1, 65280);
                        gameGraphics.DrawLabel(inventoryManager.GetItemTotalCount(shopItems[j4]).ToString(), i6 + 47, l6 + 10, 1, 65535);
                    }
                    j4++;
                }

            }

            gameGraphics.DrawHorizontalLine(_offsetX + 5, _offsetY + 222, 398, 0);
            if (selectedShopItemIndex == -1)
            {
                gameGraphics.DrawText("Select an object to buy or sell", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                return;
            }
            int itemId = shopItems[selectedShopItemIndex];
            if (itemId != -1)
            {
                if (shopItemCount[selectedShopItemIndex] > 0)
                {
                    int j6 = shopItemBuyPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (j6 < 10)
                    {
                        j6 = 10;
                    }

                    int i7 = (j6 * entityManager.GetItem(itemId).BasePrice) / 100;
                    gameGraphics.DrawString("Buy a new " + entityManager.GetItem(itemId).Name + " for " + i7 + "gp", _offsetX + 2, _offsetY + 214, 1, 0xffff00);
                    int j2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X > _offsetX + 298 && InputManager.Instance.MouseLocation.Y >= _offsetY + 204 && InputManager.Instance.MouseLocation.X < _offsetX + 408 && InputManager.Instance.MouseLocation.Y <= _offsetY + 215)
                    {
                        j2 = 0xff0000;
                    }

                    gameGraphics.DrawLabel("Click here to buy", _offsetX + 405, _offsetY + 214, 3, j2);
                }
                else
                {
                    gameGraphics.DrawText("This item is not currently available to buy", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                }

                if (inventoryManager.GetItemTotalCount(itemId) > 0)
                {
                    int k6 = shopItemSellPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (k6 < 10)
                    {
                        k6 = 10;
                    }

                    int j7 = (k6 * entityManager.GetItem(itemId).BasePrice) / 100;
                    gameGraphics.DrawLabel("Sell your " + entityManager.GetItem(itemId).Name + " for " + j7 + "gp", _offsetX + 405, _offsetY + 239, 1, 0xffff00);
                    int k2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X > _offsetX + 2 && InputManager.Instance.MouseLocation.Y >= _offsetY + 229 && InputManager.Instance.MouseLocation.X < _offsetX + 112 && InputManager.Instance.MouseLocation.Y <= _offsetY + 240)
                    {
                        k2 = 0xff0000;
                    }

                    gameGraphics.DrawString("Click here to sell", _offsetX + 2, _offsetY + 239, 3, k2);
                    return;
                }
                gameGraphics.DrawText("You do not have any of this item to sell", _offsetX + 204, _offsetY + 239, 3, 0xffff00);
            }
        }

        public void loadTextures()
        {
            sbyte[] abyte0 = unpackData("textures.jag", "Textures", 50);
            if (abyte0 == null)
            {
                errorLoading = true;
                return;
            }
            sbyte[] abyte1 = DataOperations.loadData("index.dat", 0, abyte0);
            gameCamera.createTexture(entityManager.TextureCount, 7, 11);
            for (int l = 0; l < entityManager.TextureCount; l++)
            {
                string s1 = entityManager.GetTexture(l).Name;
                sbyte[] abyte2 = DataOperations.loadData(s1 + ".dat", 0, abyte0);
                gameGraphics.unpackImageData(baseTexturePic, abyte2, abyte1, 1);
                gameGraphics.DrawBox(0, 0, 128, 128, 0xff00ff);
                gameGraphics.DrawPicture(0, 0, baseTexturePic);
                int i1 = gameGraphics.pictureAssumedWidth[baseTexturePic];
                string s2 = entityManager.GetTexture(l).SubName;
                if (s2 != null && s2.Length > 0)
                {
                    sbyte[] abyte3 = DataOperations.loadData(s2 + ".dat", 0, abyte0);
                    gameGraphics.unpackImageData(baseTexturePic, abyte3, abyte1, 1);
                    gameGraphics.DrawPicture(0, 0, baseTexturePic);
                }
                gameGraphics.DrawImage(subTexturePic + l, 0, 0, i1, i1);
                int j1 = i1 * i1;
                for (int k1 = 0; k1 < j1; k1++)
                {
                    if (gameGraphics.pictureColors[subTexturePic + l][k1] == 65280)
                    {
                        gameGraphics.pictureColors[subTexturePic + l][k1] = 0xff00ff;
                    }
                }

                gameGraphics.applyImage(subTexturePic + l);
                gameCamera.setTexture(l, gameGraphics.pictureColorIndexes[subTexturePic + l], gameGraphics.pictureColor[subTexturePic + l], i1 / 64 - 1);
            }
        }

        public void drawAppearanceWindow()
        {
            gameGraphics.ClearScreen();
            appearanceMenu.drawMenu();
            int l = 140;
            int i1 = 50;
            l += 116;
            i1 -= 25;
            gameGraphics.drawCharacterLegs(l - 32 - 55, i1, 64, 102, entityManager.GetAnimation(appearance2Colour).Number, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, entityManager.GetAnimation(appearanceBodyGender).Number, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32 - 55, i1, 64, 102, entityManager.GetAnimation(appearanceHeadType).Number, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawCharacterLegs(l - 32, i1, 64, 102, entityManager.GetAnimation(appearance2Colour).Number + 6, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, entityManager.GetAnimation(appearanceBodyGender).Number + 6, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage(l - 32, i1, 64, 102, entityManager.GetAnimation(appearanceHeadType).Number + 6, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawCharacterLegs((l - 32) + 55, i1, 64, 102, entityManager.GetAnimation(appearance2Colour).Number + 12, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, entityManager.GetAnimation(appearanceBodyGender).Number + 12, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawImage((l - 32) + 55, i1, 64, 102, entityManager.GetAnimation(appearanceHeadType).Number + 12, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.DrawPicture(0, WindowSize.Height, baseInventoryPic + 22);

            OnDrawDone();
        }

        public void checkMouseStatus()
        {
            if (selectedSpell >= 0 || selectedItem >= 0)
            {
                menuText1[menuOptionsCount] = "Cancel";
                menuText2[menuOptionsCount] = "";
                menuActions[menuOptionsCount] = MenuAction.Cancel; ;
                menuOptionsCount++;
            }
            for (int l = 0; l < menuOptionsCount; l++)
            {
                menuIndexes[l] = l;
            }

            for (bool flag = false; !flag;)
            {
                flag = true;
                for (int i1 = 0; i1 < menuOptionsCount - 1; i1++)
                {
                    int k1 = menuIndexes[i1];
                    int i2 = menuIndexes[i1 + 1];
                    if (menuActions[k1] > menuActions[i2])
                    {
                        menuIndexes[i1] = i2;
                        menuIndexes[i1 + 1] = k1;
                        flag = false;
                    }
                }

            }

            if (menuOptionsCount > 20)
            {
                menuOptionsCount = 20;
            }

            if (menuOptionsCount > 0)
            {
                int j1 = -1;
                for (int l1 = 0; l1 < menuOptionsCount; l1++)
                {
                    if (menuText2[menuIndexes[l1]] == null || menuText2[menuIndexes[l1]].Length <= 0)
                    {
                        continue;
                    }

                    j1 = l1;
                    break;
                }

                string s1 = null;
                if ((selectedItem >= 0 || selectedSpell >= 0) && menuOptionsCount == 1)
                {
                    s1 = "Choose a target";
                }
                else
                    if ((selectedItem >= 0 || selectedSpell >= 0) && menuOptionsCount > 1)
                {
                    s1 = "@whi@" + menuText1[menuIndexes[0]] + " " + menuText2[menuIndexes[0]];
                }
                else
                        if (j1 != -1)
                {
                    s1 = menuText2[menuIndexes[j1]] + ": @whi@" + menuText1[menuIndexes[0]];
                }

                if (menuOptionsCount == 2 && s1 != null)
                {
                    s1 = s1 + "@whi@ / 1 more option";
                }

                if (menuOptionsCount > 2 && s1 != null)
                {
                    s1 = s1 + "@whi@ / " + (menuOptionsCount - 1) + " more options";
                }

                if (s1 != null)
                {
                    gameGraphics.DrawString(s1, 6, 14, 1, 0xffff00);
                }

                if (mouseButtonClick == 1)
                {
                    menuClick(menuIndexes[0]);
                    mouseButtonClick = 0;
                    return;
                }
                if (mouseButtonClick == 2)
                {
                    menuHeight = (menuOptionsCount + 1) * 15;
                    menuWidth = gameGraphics.textWidth("Choose option", 1) + 5;
                    for (int j2 = 0; j2 < menuOptionsCount; j2++)
                    {
                        int k2 = gameGraphics.textWidth(menuText1[j2] + " " + menuText2[j2], 1) + 5;
                        if (k2 > menuWidth)
                        {
                            menuWidth = k2;
                        }
                    }

                    menuX = InputManager.Instance.MouseLocation.X - menuWidth / 2;
                    menuY = InputManager.Instance.MouseLocation.Y - 7;
                    menuShow = true;

                    if (menuX < 0)
                    {
                        menuX = 0;
                    }

                    if (menuY < 0)
                    {
                        menuY = 0;
                    }

                    if (menuX + menuWidth > 510)
                    {
                        menuX = 510 - menuWidth;
                    }

                    if (menuY + menuHeight > 315)
                    {
                        menuY = 315 - menuHeight;
                    }

                    mouseButtonClick = 0;
                }
            }
        }

        public void drawGame()
        {
            if (PlayerAliveTimeout != 0)
            {
                DrawDead();
            }
            else if (ShowAppearanceWindow)
            {
                drawAppearanceWindow();
            }
            else if (IsSleeping)
            {
                DrawSleeping();
            }
            else if (!engineHandle.playerIsAlive)
            {
                return;
            }

            for (int l = 0; l < 64; l++)
            {
                gameCamera.removeModel(engineHandle.roofObject[lastLayerIndex][l]);

                if (lastLayerIndex == 0)
                {
                    gameCamera.removeModel(engineHandle.wallObject[1][l]);
                    gameCamera.removeModel(engineHandle.roofObject[1][l]);
                    gameCamera.removeModel(engineHandle.wallObject[2][l]);
                    gameCamera.removeModel(engineHandle.roofObject[2][l]);
                }

                cameraZoom = true;

                if (lastLayerIndex == 0 && (engineHandle.tiles[CurrentPlayer.Location.X / 128][CurrentPlayer.Location.Y / 128] & 0x80) == 0)
                {
                    if (SettingsManager.Instance.GraphicsSettings.ShowRoofs)
                    {
                        gameCamera.addModel(engineHandle.roofObject[lastLayerIndex][l]);
                        if (lastLayerIndex == 0)
                        {
                            // draw wall object at lv 1 / second layer
                            gameCamera.addModel(engineHandle.wallObject[1][l]);
                            // draw roof object at lv 1 / second layer
                            var roof1 = engineHandle.roofObject[1][l];
                            gameCamera.addModel(roof1);


                            // draw wall object at lv 2 / third layer
                            gameCamera.addModel(engineHandle.wallObject[2][l]);

                            // draw roof object at lv 2 / third layer
                            var roof2 = engineHandle.roofObject[2][l];
                            gameCamera.addModel(engineHandle.roofObject[2][l]);
                        }
                    }
                    cameraZoom = false;
                }
            }

            if (modelFireLightningSpellNumber != lastModelFireLightningSpellNumber)
            {
                lastModelFireLightningSpellNumber = modelFireLightningSpellNumber;

                for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
                {
                    if (ObjectType[objectIndex] == 97)
                    {
                        drawModel(objectIndex, "firea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[objectIndex] == 274)
                    {
                        drawModel(objectIndex, "fireplacea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[objectIndex] == 1031)
                    {
                        drawModel(objectIndex, "lightning" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[objectIndex] == 1036)
                    {
                        drawModel(objectIndex, "firespell" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[objectIndex] == 1147)
                    {
                        drawModel(objectIndex, "spellcharge" + (modelFireLightningSpellNumber + 1));
                    }
                }
            }

            if (modelTorchNumber != lastModelTorchNumber)
            {
                lastModelTorchNumber = modelTorchNumber;

                for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
                {
                    if (ObjectType[objectIndex] == 51)
                    {
                        drawModel(objectIndex, "torcha" + (modelTorchNumber + 1));
                    }

                    if (ObjectType[objectIndex] == 143)
                    {
                        drawModel(objectIndex, "skulltorcha" + (modelTorchNumber + 1));
                    }
                }

            }

            if (modelClawSpellNumber != lastModelClawSpellNumber)
            {
                lastModelClawSpellNumber = modelClawSpellNumber;

                for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
                {
                    if (ObjectType[objectIndex] == 1142)
                    {
                        drawModel(objectIndex, "clawspell" + (modelClawSpellNumber + 1));
                    }
                }
            }
            gameCamera.removeLastUpdates(drawUpdatesPerformed);
            drawUpdatesPerformed = 0;

            for (int playerIndex = 0; playerIndex < PlayerCount; playerIndex++)
            {
                ClientMob player = Players[playerIndex];

                if (player.Appearance.TrousersColour != 255)
                {
                    int j2 = player.Location.X;
                    int l2 = player.Location.Y;
                    int j3 = -engineHandle.getAveragedElevation(j2, l2);

                    Point3D loc = new Point3D(j2, j3, l2);
                    int k4 = gameCamera.addSpriteToScene(5000 + playerIndex, loc, 145, 220, playerIndex + 10000);

                    drawUpdatesPerformed++;

                    if (player == CurrentPlayer)
                    {
                        gameCamera.bhe(k4);
                    }

                    if (player.currentSprite == 8)
                    {
                        gameCamera.bhf(k4, -30);
                    }

                    if (player.currentSprite == 9)
                    {
                        gameCamera.bhf(k4, 30);
                    }
                }
            }

            for (int playerIndex = 0; playerIndex < PlayerCount; playerIndex++)
            {
                ClientMob player = Players[playerIndex];

                if (player.ProjectileDistance > 0)
                {
                    ClientMob targetMob = null;
                    if (player.AttackingNpcIndex != -1)
                    {
                        targetMob = NpcAttackingArray[player.AttackingNpcIndex];
                    }
                    else if (player.AttackingPlayerIndex != -1)
                    {
                        targetMob = Mobs[player.AttackingPlayerIndex];
                    }

                    if (targetMob != null)
                    {
                        int k3 = player.Location.X;
                        int l4 = player.Location.Y;
                        int k7 = -engineHandle.getAveragedElevation(k3, l4) - 110;
                        int k9 = targetMob.Location.X;
                        int j10 = targetMob.Location.Y;
                        int k10 = -engineHandle.getAveragedElevation(k9, j10) - entityManager.GetNpc(targetMob.npcId).Camera2 / 2;
                        int l10 = (k3 * player.ProjectileDistance + k9 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;
                        int i11 = (k7 * player.ProjectileDistance + k10 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;
                        int j11 = (l4 * player.ProjectileDistance + j10 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;

                        Point3D loc = new Point3D(l10, i11, j11);
                        gameCamera.addSpriteToScene(baseProjectilePic + player.ProjectileType, loc, 32, 32, 0);
                        drawUpdatesPerformed++;
                    }
                }
            }

            for (int npcIndex = 0; npcIndex < NpcCount; npcIndex++)
            {
                ClientMob npc = Npcs[npcIndex];

                int x1 = npc.Location.X;
                int z1 = npc.Location.Y;
                int y1 = -engineHandle.getAveragedElevation(x1, z1);

                Point3D loc = new Point3D(x1, y1, z1);
                int l9 = gameCamera.addSpriteToScene(20000 + npcIndex, loc, entityManager.GetNpc(npc.npcId).Camera1, entityManager.GetNpc(npc.npcId).Camera2, npcIndex + 30000);

                drawUpdatesPerformed++;

                if (npc.currentSprite == 8)
                {
                    gameCamera.bhf(l9, -30);
                }

                if (npc.currentSprite == 9)
                {
                    gameCamera.bhf(l9, 30);
                }
            }

            for (int groundITemIndex = 0; groundITemIndex < GroundItemCount; groundITemIndex++)
            {
                int x = GroundItemLocations[groundITemIndex].X * GridSize + 64;
                int y = GroundItemLocations[groundITemIndex].Y * GridSize + 64;

                Point3D loc = new Point3D(x, -engineHandle.getAveragedElevation(x, y) - GroundItemObjectVar[groundITemIndex], y);
                gameCamera.addSpriteToScene(40000 + GroundItemId[groundITemIndex], loc, 96, 64, groundITemIndex + 20000);
                drawUpdatesPerformed++;
            }

            for (int teleBubbleIndex = 0; teleBubbleIndex < teleBubbleCount; teleBubbleIndex++)
            {
                int k5 = teleBubbleX[teleBubbleIndex] * GridSize + 64;
                int i8 = teleBubbleY[teleBubbleIndex] * GridSize + 64;
                int i10 = teleBubbleType[teleBubbleIndex];

                if (i10 == 0)
                {
                    Point3D loc = new Point3D(k5, -engineHandle.getAveragedElevation(k5, i8), i8);
                    gameCamera.addSpriteToScene(50000 + teleBubbleIndex, loc, 128, 256, teleBubbleIndex + 50000);
                    drawUpdatesPerformed++;
                }

                if (i10 == 1)
                {
                    Point3D loc = new Point3D(k5, -engineHandle.getAveragedElevation(k5, i8), i8);
                    gameCamera.addSpriteToScene(50000 + teleBubbleIndex, loc, 128, 64, teleBubbleIndex + 50000);
                    drawUpdatesPerformed++;
                }
            }

            gameGraphics.ClearScreen();

            if (lastLayerIndex == 3)
            {
                int l5 = 40 + (int)(Helper.Random.NextDouble() * 3D);
                int j8 = 40 + (int)(Helper.Random.NextDouble() * 7D);

                Point3D loc = new Point3D(-50, -10, -50);
                gameCamera.bjl(l5, j8, loc);
            }

            itemsAboveHeadCount = 0;
            receivedMessagesCount = 0;
            healthBarVisibleCount = 0;

            if (cameraAutoAngleDebug)
            {
                if (SettingsManager.Instance.CameraAutoAngle && !cameraZoom)
                {
                    int i6 = cameraAutoAngle;
                    autoRotateCamera();

                    if (cameraAutoAngle != i6)
                    {
                        cameraAutoRotatePlayerX = CurrentPlayer.Location.X;
                        cameraAutoRotatePlayerY = CurrentPlayer.Location.Y;
                    }
                }

                if (SettingsManager.Instance.GraphicsSettings.FogOfWar)
                {
                    gameCamera.zoom1 = 3000;
                    gameCamera.zoom2 = 3000;
                    gameCamera.zoom3 = 1;
                    gameCamera.zoom4 = 2800;
                }
                else
                {
                    gameCamera.zoom1 = 40000;
                    gameCamera.zoom2 = 40000;
                    gameCamera.zoom3 = 40000;
                    gameCamera.zoom4 = 40000;
                }

                int newCameraPosX = cameraAutoRotatePlayerX + cameraRotationXAmount;
                int newCameraPosY = cameraAutoRotatePlayerY + cameraRotationYAmount;

                cameraRotation = cameraAutoAngle * 32;
                Point3D loc = new Point3D(newCameraPosX, -engineHandle.getAveragedElevation(newCameraPosX, newCameraPosY), newCameraPosY);
                gameCamera.SetCameraTransform(loc, 912, cameraRotation * 4, 0, 2000);
            }
            else
            {
                if (SettingsManager.Instance.CameraAutoAngle && !cameraZoom)
                {
                    autoRotateCamera();
                }

                if (SettingsManager.Instance.GraphicsSettings.FogOfWar)
                {
                    gameCamera.zoom1 = 2400;
                    gameCamera.zoom2 = 2400;
                    gameCamera.zoom3 = 1;
                    gameCamera.zoom4 = 2300;
                }
                else
                {
                    gameCamera.zoom1 = 40000;
                    gameCamera.zoom2 = 40000;
                    gameCamera.zoom3 = 40000;
                    gameCamera.zoom4 = 40000;
                }

                int k6 = cameraAutoRotatePlayerX + cameraRotationXAmount;
                int l8 = cameraAutoRotatePlayerY + cameraRotationYAmount;
                Point3D loc = new Point3D(k6, -engineHandle.getAveragedElevation(k6, l8), l8);
                gameCamera.SetCameraTransform(loc, 912, cameraRotation * 4, 0, cameraDistance * 2);
            }

            gameCamera.finishCamera();
            drawAboveHeadThings();

            if (actionPictureType > 0)
            {
                gameGraphics.DrawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 14 + (24 - actionPictureType) / 6);
            }

            if (actionPictureType < 0)
            {
                gameGraphics.DrawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 18 + (24 + actionPictureType) / 6);
            }

            gameGraphics.DrawPicture(gameGraphics.GameSize.Width - 3 - 197, 3, baseInventoryPic, 128);

            drawMenus();

            gameGraphics.IsLoggedIn = false;


            string text =
                "Coordinates: ( " + (SectionLocation.X + AreaLocation.X) + "," + (SectionLocation.Y + AreaLocation.Y) + " ) " +
                "Section: (" + SectionLocation.X + "," + SectionLocation.Y + ") " +
                "Area: (" + AreaLocation.X + "," + AreaLocation.Y + ")";

            // Text shadow
            gameGraphics.DrawString(text, 10 + 11, 10 + 11, 1, 0x000000);
            gameGraphics.DrawString(text, 10 + 10, 10 + 10, 1, 0xffffff);

            OnDrawDone();
        }

        void DrawDead()
        {
            gameGraphics.FadeScreenToBlack();
            gameGraphics.DrawText("Oh dear! You are dead...", WindowSize.Width / 2, WindowSize.Height / 2, 7, 0xff0000);

            OnDrawDone();
        }

        void DrawSleeping()
        {
            gameGraphics.FadeScreenToBlack();

            if (Helper.Random.NextDouble() < 0.14999999999999999D)
            {
                gameGraphics.DrawText("ZZZ", (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
            }

            if (Helper.Random.NextDouble() < 0.14999999999999999D)
            {
                gameGraphics.DrawText("ZZZ", 512 - (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
            }

            gameGraphics.DrawBox(WindowSize.Width / 2 - 100, 160, 200, 40, 0);
            gameGraphics.DrawText("You are sleeping", WindowSize.Width / 2, 50, 7, 0xffff00);
            gameGraphics.DrawText("Fatigue: " + (PlayerFatigue * 100) / 750 + "%", WindowSize.Width / 2, 90, 7, 0xffff00);
            gameGraphics.DrawText("When you want to wake up just use your", WindowSize.Width / 2, 140, 5, 0xffffff);
            gameGraphics.DrawText("keyboard to type the word in the box below", WindowSize.Width / 2, 160, 5, 0xffffff);

            if (sleepingStatusText == null)
            {
                gameGraphics.drawPixels(captchaPixels, WindowSize.Width / 2 - 127, 230, captchaWidth, captchaHeight);
            }
            else
            {
                gameGraphics.DrawText(sleepingStatusText, WindowSize.Width / 2, 260, 5, 0xff0000);
            }

            gameGraphics.DrawBoxEdge(WindowSize.Width / 2 - 128, 229, 257, 42, 0xffffff);
            gameGraphics.DrawText("If you can't read the word", WindowSize.Width / 2, 290, 1, 0xffffff);
            gameGraphics.DrawText("@yel@click here@whi@ to get a different one", WindowSize.Width / 2, 305, 1, 0xffffff);

            OnDrawDone();
        }

        public void drawMenus()
        {
            if (ShowBankBox)
            {
                drawBankBox();
            }
            else if (ShowShopBox)
            {
                drawShopBox();
            }
            else
            {
                if (ShowQuestionMenu)
                {
                    drawQuestionMenu();
                }

                getMenuHighlighted();
                bool flag = !ShowQuestionMenu && !menuShow;
                if (flag)
                {
                    menuOptionsCount = 0;
                }

                if (drawMenuTab == 0 && flag)
                {
                    generateWorldRightClickMenu();
                }

                if (drawMenuTab == 1)
                {
                    drawInventoryMenu(flag);
                }

                if (drawMenuTab == 2)
                {
                    drawMinimapMenu(flag);
                }

                if (drawMenuTab == 4)
                {
                    drawPrayerMagicMenu(flag);
                }

                if (!menuShow && !ShowQuestionMenu)
                {
                    checkMouseStatus();
                }

                if (menuShow && !ShowQuestionMenu)
                {
                    drawRightClickMenu();
                }
            }
            mouseButtonClick = 0;
        }

        public void LoadModels()
        {
            entityManager.GetModelIndex("torcha2");
            entityManager.GetModelIndex("torcha3");
            entityManager.GetModelIndex("torcha4");
            entityManager.GetModelIndex("skulltorcha2");
            entityManager.GetModelIndex("skulltorcha3");
            entityManager.GetModelIndex("skulltorcha4");
            entityManager.GetModelIndex("firea2");
            entityManager.GetModelIndex("firea3");
            entityManager.GetModelIndex("fireplacea2");
            entityManager.GetModelIndex("fireplacea3");
            entityManager.GetModelIndex("firespell2");
            entityManager.GetModelIndex("firespell3");
            entityManager.GetModelIndex("lightning2");
            entityManager.GetModelIndex("lightning3");
            entityManager.GetModelIndex("clawspell2");
            entityManager.GetModelIndex("clawspell3");
            entityManager.GetModelIndex("clawspell4");
            entityManager.GetModelIndex("clawspell5");
            entityManager.GetModelIndex("spellcharge2");
            entityManager.GetModelIndex("spellcharge3");

            sbyte[] models = unpackData("models.jag", "3d models", 60);

            if (models == null)
            {
                errorLoading = true;
                return;
            }

            for (int objectModelIndex = 0; objectModelIndex < entityManager.ObjectModelCount; objectModelIndex++)
            {
                try
                {
                    long objectOffset = DataOperations.getObjectOffset(entityManager.GetObjectModelName(objectModelIndex) + ".ob3", models);

                    if (objectOffset != 0)
                    {
                        GameDataObjects[objectModelIndex] = new ObjectModel(models, (int)objectOffset);
                    }
                    else
                    {
                        GameDataObjects[objectModelIndex] = new ObjectModel(1, 1);
                    }

                    if (entityManager.GetObjectModelName(objectModelIndex).Equals("giantcrystal"))
                    {
                        GameDataObjects[objectModelIndex].isGiantCrystal = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                    Console.WriteLine(ex);
                }
            }
        }

        public void DrawNpc(int x, int y, int width, int height, int index, int unknown1, int unknown2)
        {
            ClientMob npc = Npcs[index];
            int frameIndex = npc.currentSprite + (cameraRotation + 16) / 32 & 7;
            bool flag = false;
            int newFrameIndex = frameIndex;
            if (newFrameIndex == 5)
            {
                newFrameIndex = 3;
                flag = true;
            }
            else if (newFrameIndex == 6)
            {
                newFrameIndex = 2;
                flag = true;
            }
            else if (newFrameIndex == 7)
            {
                newFrameIndex = 1;
                flag = true;
            }
            int j1 = newFrameIndex * 3 + walkModel[(npc.stepCount / entityManager.GetNpc(npc.npcId).WalkModel) % 4];
            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = false;
                x -= (entityManager.GetNpc(npc.npcId).CombatSprite * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray1[(tick / (entityManager.GetNpc(npc.npcId).CombatModel - 1)) % 8];
            }
            else
                if (npc.currentSprite == 9)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = true;
                x += (entityManager.GetNpc(npc.npcId).CombatSprite * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray2[(tick / entityManager.GetNpc(npc.npcId).CombatModel) % 8];
            }

            for (int k1 = 0; k1 < 12; k1++)
            {
                int l1 = animationModelArray[frameIndex][k1];
                int k2 = entityManager.GetNpc(npc.npcId).Sprites[l1];

                if (k2 >= 0)
                {
                    int i3 = 0;
                    int j3 = 0;
                    int k3 = j1;

                    if (flag && newFrameIndex >= 1 && newFrameIndex <= 3 && entityManager.GetAnimation(k2).HasF == 1)
                    {
                        k3 += 15;
                    }

                    if (newFrameIndex != 5 || entityManager.GetAnimation(k2).HasA == 1)
                    {
                        int l3 = k3 + entityManager.GetAnimation(k2).Number;
                        i3 = (i3 * width) / gameGraphics.pictureAssumedWidth[l3];
                        j3 = (j3 * height) / gameGraphics.pictureAssumedHeight[l3];
                        int i4 = (width * gameGraphics.pictureAssumedWidth[l3]) / gameGraphics.pictureAssumedWidth[entityManager.GetAnimation(k2).Number];
                        i3 -= (i4 - width) / 2;
                        int j4 = entityManager.GetAnimation(k2).CharacterColour;
                        int k4 = 0;

                        if (j4 == 1)
                        {
                            j4 = entityManager.GetNpc(npc.npcId).Appearance.HairColour;
                            k4 = entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }
                        else if (j4 == 2)
                        {
                            j4 = entityManager.GetNpc(npc.npcId).Appearance.TopColour;
                            k4 = entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }
                        else if (j4 == 3)
                        {
                            j4 = entityManager.GetNpc(npc.npcId).Appearance.TrousersColour;
                            k4 = entityManager.GetNpc(npc.npcId).Appearance.SkinColour;
                        }

                        gameGraphics.DrawImage(x + i3, y + j3, i4, height, l3, j4, k4, unknown1, flag);
                    }
                }
            }

            if (npc.lastMessageTimeout > 0)
            {
                receivedMessageMidPoint[receivedMessagesCount] = gameGraphics.textWidth(npc.lastMessage, 1) / 2;
                if (receivedMessageMidPoint[receivedMessagesCount] > 150)
                {
                    receivedMessageMidPoint[receivedMessagesCount] = 150;
                }

                receivedMessageHeight[receivedMessagesCount] = (gameGraphics.textWidth(npc.lastMessage, 1) / 300) * gameGraphics.textHeightNumber(1);
                receivedMessageX[receivedMessagesCount] = x + width / 2;
                receivedMessageY[receivedMessagesCount] = y;
                receivedMessages[receivedMessagesCount++] = npc.lastMessage;
            }
            if (npc.currentSprite == 8 || npc.currentSprite == 9 || npc.combatTimer != 0)
            {
                if (npc.combatTimer > 0)
                {
                    int i2 = x;

                    if (npc.currentSprite == 8)
                    {
                        i2 -= (20 * unknown2) / 100;
                    }
                    else if (npc.currentSprite == 9)
                    {
                        i2 += (20 * unknown2) / 100;
                    }

                    int l2 = (npc.CurrentHitpoints * 30) / npc.BaseHitpoints;
                    healthBarX[healthBarVisibleCount] = i2 + width / 2;
                    healthBarY[healthBarVisibleCount] = y;
                    healthBarMissing[healthBarVisibleCount++] = l2;
                }

                if (npc.combatTimer > 150)
                {
                    int j2 = x;
                    if (npc.currentSprite == 8)
                    {
                        j2 -= (10 * unknown2) / 100;
                    }
                    else
                        if (npc.currentSprite == 9)
                    {
                        j2 += (10 * unknown2) / 100;
                    }

                    gameGraphics.DrawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 12);
                    gameGraphics.DrawText(npc.LastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
        }

        public override void DisplayMessage(string message)
        {
            OnChatMessageReceived?.Invoke(this, new ChatMessageEventArgs(message));
        }

        public void drawAboveHeadThings()
        {
            for (int l = 0; l < receivedMessagesCount; l++)
            {
                int height = gameGraphics.textHeightNumber(1);
                int x = receivedMessageX[l];
                int y = receivedMessageY[l];
                int midpoint = receivedMessageMidPoint[l];
                int l3 = receivedMessageHeight[l];
                bool flag = true;
                while (flag)
                {
                    flag = false;
                    for (int l4 = 0; l4 < l; l4++)
                    {
                        if (y + l3 > receivedMessageY[l4] - height && y - height < receivedMessageY[l4] + receivedMessageHeight[l4] && x - midpoint < receivedMessageX[l4] + receivedMessageMidPoint[l4] && x + midpoint > receivedMessageX[l4] - receivedMessageMidPoint[l4] && receivedMessageY[l4] - height - l3 < y)
                        {
                            y = receivedMessageY[l4] - height - l3;
                            flag = true;
                        }
                    }
                }
                receivedMessageY[l] = y;
                gameGraphics.DrawFloatingText(receivedMessages[l], x, y, 1, 0xffff00, 300);
            }

            for (int j1 = 0; j1 < itemsAboveHeadCount; j1++)
            {
                int x = itemAboveHeadX[j1];
                int y = itemAboveHeadY[j1];
                int scale = itemAboveHeadScale[j1];
                int id = itemAboveHeadID[j1];
                int width = (39 * scale) / 100;
                int height = (27 * scale) / 100;
                int j5 = y - height;
                gameGraphics.DrawImageTransparent(x - width / 2, j5, width, height, baseInventoryPic + 9, 85);
                int k5 = (36 * scale) / 100;
                int l5 = (24 * scale) / 100;
                gameGraphics.DrawImage(x - k5 / 2, (j5 + height / 2) - l5 / 2, k5, l5, entityManager.GetItem(id).InventoryPicture + baseItemPicture, entityManager.GetItem(id).PictureMask, 0, 0, false);
            }

            for (int i2 = 0; i2 < healthBarVisibleCount; i2++)
            {
                int x = healthBarX[i2];
                int y = healthBarY[i2];
                int missing = healthBarMissing[i2];
                gameGraphics.drawBoxAlpha(x - 15, y - 3, missing, 5, 65280, 192);
                gameGraphics.drawBoxAlpha((x - 15) + missing, y - 3, 30 - missing, 5, 0xff0000, 192);
            }

        }

        public void drawBankBox()
        {
            char c1 = '\u0198';
            char c2 = '\u014E';

            InventoryItem bankItem = inventoryManager.GetBankItem(selectedBankItem);

            if (bankPage > 0 && inventoryManager.BankItemsCount <= 48)
            {
                bankPage = 0;
            }

            if (bankPage > 1 && inventoryManager.BankItemsCount <= 96)
            {
                bankPage = 1;
            }

            if (bankPage > 2 && inventoryManager.BankItemsCount <= 144)
            {
                bankPage = 2;
            }

            if (selectedBankItem >= inventoryManager.BankItemsCount || selectedBankItem < 0)
            {
                selectedBankItem = -1;
            }

            if (selectedBankItem != -1 && bankItem.Index != selectedBankItemType)
            {
                selectedBankItem = -1;
                selectedBankItemType = -2;
            }

            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                int l = InputManager.Instance.MouseLocation.X - (256 - c1 / 2);
                int j1 = InputManager.Instance.MouseLocation.Y - (170 - c2 / 2);

                if (l >= 0 && j1 >= 12 && l < 408 && j1 < 280)
                {
                    int slot = bankPage * 48;

                    for (int k2 = 0; k2 < 6; k2++)
                    {
                        for (int i3 = 0; i3 < 8; i3++)
                        {
                            int k7 = 7 + i3 * 49;
                            int i8 = 28 + k2 * 34;

                            if (l > k7 && l < k7 + 49 && j1 > i8 && j1 < i8 + 34 && slot < inventoryManager.BankItemsCount && inventoryManager.GetItem(slot).Index != -1)
                            {
                                selectedBankItemType = bankItem.Index;
                                selectedBankItem = slot;
                            }

                            slot++;
                        }
                    }

                    l = 256 - c1 / 2;
                    j1 = 170 - c2 / 2;
                    int id;

                    if (selectedBankItem < 0)
                    {
                        id = -1;
                    }
                    else
                    {
                        id = bankItem.Index;
                    }

                    if (id != -1)
                    {
                        int count = bankItem.Quantity;

                        if (entityManager.GetItem(id).IsStackable == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 &&
                            InputManager.Instance.MouseLocation.X >= l + 220 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 250 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(1);
                            StreamClass.FormatPacket();
                        }

                        if (count >= 5 &&
                            InputManager.Instance.MouseLocation.X >= l + 250 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 280 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(5);
                            StreamClass.FormatPacket();
                        }

                        if (count >= 25 &&
                            InputManager.Instance.MouseLocation.X >= l + 280 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 305 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(25);
                            StreamClass.FormatPacket();
                        }

                        if (count >= 100 &&
                            InputManager.Instance.MouseLocation.X >= l + 305 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 335 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(100);
                            StreamClass.FormatPacket();
                        }

                        if (count >= 500 &&
                            InputManager.Instance.MouseLocation.X >= l + 335 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 368 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(500);
                            StreamClass.FormatPacket();
                        }

                        if (count >= 2500 &&
                            InputManager.Instance.MouseLocation.X >= l + 370 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 238 &&
                            InputManager.Instance.MouseLocation.X < l + 400 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(2500);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 1 &&
                            InputManager.Instance.MouseLocation.X >= l + 220 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 250 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(1);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 5 &&
                            InputManager.Instance.MouseLocation.X >= l + 250 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 280 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(5);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 25 &&
                            InputManager.Instance.MouseLocation.X >= l + 280 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 305 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(25);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 100 &&
                            InputManager.Instance.MouseLocation.X >= l + 305 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 335 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(100);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 500 &&
                            InputManager.Instance.MouseLocation.X >= l + 335 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 368 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(500);
                            StreamClass.FormatPacket();
                        }

                        if (inventoryManager.GetItemTotalCount(id) >= 2500 &&
                            InputManager.Instance.MouseLocation.X >= l + 370 &&
                            InputManager.Instance.MouseLocation.Y >= j1 + 263 &&
                            InputManager.Instance.MouseLocation.X < l + 400 &&
                            InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(2500);
                            StreamClass.FormatPacket();
                        }
                    }
                }
                else if (inventoryManager.BankItemsCount > 48 && l >= 50 && l <= 115 && j1 <= 12)
                {
                    bankPage = 0;
                }
                else if (inventoryManager.BankItemsCount > 48 && l >= 115 && l <= 180 && j1 <= 12)
                {
                    bankPage = 1;
                }
                else if (inventoryManager.BankItemsCount > 96 && l >= 180 && l <= 245 && j1 <= 12)
                {
                    bankPage = 2;
                }
                else if (inventoryManager.BankItemsCount > 144 && l >= 245 && l <= 310 && j1 <= 12)
                {
                    bankPage = 3;
                }
                else
                {
                    StreamClass.CreatePacket(48);
                    StreamClass.FormatPacket();
                    ShowBankBox = false;
                    return;
                }
            }

            int i1 = 256 - c1 / 2;
            int k1 = 170 - c2 / 2;
            gameGraphics.DrawBox(i1, k1, 408, 12, 192);
            int j2 = 0x989898;
            gameGraphics.drawBoxAlpha(i1, k1 + 12, 408, 17, j2, 160);
            gameGraphics.drawBoxAlpha(i1, k1 + 29, 8, 204, j2, 160);
            gameGraphics.drawBoxAlpha(i1 + 399, k1 + 29, 9, 204, j2, 160);
            gameGraphics.drawBoxAlpha(i1, k1 + 233, 408, 47, j2, 160);
            gameGraphics.DrawString("Bank", i1 + 1, k1 + 10, 1, 0xffffff);
            int l2 = 50;

            if (inventoryManager.BankItemsCount > 48)
            {
                int k3 = 0xffffff;
                if (bankPage == 0)
                {
                    k3 = 0xff0000;
                }
                else if (InputManager.Instance.MouseLocation.X > i1 + l2 &&
                         InputManager.Instance.MouseLocation.Y >= k1 &&
                         InputManager.Instance.MouseLocation.X < i1 + l2 + 65 &&
                         InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 1>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
                k3 = 0xffffff;

                if (bankPage == 1)
                {
                    k3 = 0xff0000;
                }
                else if (InputManager.Instance.MouseLocation.X > i1 + l2 &&
                         InputManager.Instance.MouseLocation.Y >= k1 &&
                         InputManager.Instance.MouseLocation.X < i1 + l2 + 65 &&
                         InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 2>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
            }

            if (inventoryManager.BankItemsCount > 96)
            {
                int l3 = 0xffffff;

                if (bankPage == 2)
                {
                    l3 = 0xff0000;
                }
                else if (InputManager.Instance.MouseLocation.X > i1 + l2 &&
                         InputManager.Instance.MouseLocation.Y >= k1 &&
                         InputManager.Instance.MouseLocation.X < i1 + l2 + 65 &&
                         InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    l3 = 0xffff00;
                }

                gameGraphics.DrawString("<page 3>", i1 + l2, k1 + 10, 1, l3);
                l2 += 65;
            }

            if (inventoryManager.BankItemsCount > 144)
            {
                int i4 = 0xffffff;
                if (bankPage == 3)
                {
                    i4 = 0xff0000;
                }
                else if (InputManager.Instance.MouseLocation.X > i1 + l2 &&
                         InputManager.Instance.MouseLocation.Y >= k1 &&
                         InputManager.Instance.MouseLocation.X < i1 + l2 + 65 &&
                         InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    i4 = 0xffff00;
                }

                gameGraphics.DrawString("<page 4>", i1 + l2, k1 + 10, 1, i4);
                l2 += 65;
            }

            int j4 = 0xffffff;

            if (InputManager.Instance.MouseLocation.X > i1 + 320 &&
                InputManager.Instance.MouseLocation.Y >= k1 &&
                InputManager.Instance.MouseLocation.X < i1 + 408 &&
                InputManager.Instance.MouseLocation.Y < k1 + 12)
            {
                j4 = 0xff0000;
            }

            gameGraphics.DrawLabel("Close window", i1 + 406, k1 + 10, 1, j4);
            gameGraphics.DrawString("Number in bank in green", i1 + 7, k1 + 24, 1, 65280);
            gameGraphics.DrawString("Number held in blue", i1 + 289, k1 + 24, 1, 65535);

            int l7 = 0xd0d0d0;
            int bankSlot = bankPage * 48;

            for (int l8 = 0; l8 < 6; l8++)
            {
                for (int i9 = 0; i9 < 8; i9++)
                {
                    int k9 = i1 + 7 + i9 * 49;
                    int l9 = k1 + 28 + l8 * 34;

                    InventoryItem bankItem2 = inventoryManager.GetBankItem(bankSlot);
                    Item item = entityManager.GetItem(bankItem2.Index);

                    if (selectedBankItem == bankSlot)
                    {
                        gameGraphics.drawBoxAlpha(k9, l9, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        gameGraphics.drawBoxAlpha(k9, l9, 49, 34, l7, 160);
                    }

                    gameGraphics.DrawBoxEdge(k9, l9, 50, 35, 0);

                    if (bankSlot < inventoryManager.BankItemsCount && bankItem2.Index != -1)
                    {
                        gameGraphics.DrawImage(k9, l9, 48, 32, baseItemPicture + item.InventoryPicture, item.PictureMask, 0, 0, false);
                        gameGraphics.DrawString(bankItem2.Quantity.ToString(), k9 + 1, l9 + 10, 1, 65280);
                        gameGraphics.DrawLabel(inventoryManager.GetItemTotalCount(bankItem2.Quantity).ToString(), k9 + 47, l9 + 29, 1, 65535);
                    }

                    bankSlot++;
                }
            }

            gameGraphics.DrawHorizontalLine(i1 + 5, k1 + 256, 398, 0);

            if (selectedBankItem == -1)
            {
                gameGraphics.DrawText("Select an object to withdraw or deposit", i1 + 204, k1 + 248, 3, 0xffff00);
                return;
            }

            int itemId;

            if (selectedBankItem < 0)
            {
                itemId = -1;
            }
            else
            {
                itemId = inventoryManager.GetBankItem(selectedBankItem).Index;
            }

            if (itemId != -1)
            {
                int quantity = inventoryManager.GetBankItem(selectedBankItem).Quantity;

                if (entityManager.GetItem(itemId).IsStackable == 1 && quantity > 1)
                {
                    quantity = 1;
                }

                if (quantity > 0)
                {
                    gameGraphics.DrawString("Withdraw " + entityManager.GetItem(itemId).Name, i1 + 2, k1 + 248, 1, 0xffffff);

                    int k4 = 0xffffff;

                    if (InputManager.Instance.MouseLocation.X >= i1 + 220 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 250 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                    {
                        k4 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 248, 1, k4);

                    if (quantity >= 5)
                    {
                        int l4 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 250 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 280 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            l4 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 248, 1, l4);
                    }

                    if (quantity >= 25)
                    {
                        int i5 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 280 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 305 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            i5 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 248, 1, i5);
                    }

                    if (quantity >= 100)
                    {
                        int j5 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 305 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 335 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            j5 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 248, 1, j5);
                    }

                    if (quantity >= 500)
                    {
                        int k5 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 335 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 368 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            k5 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 248, 1, k5);
                    }

                    if (quantity >= 2500)
                    {
                        int l5 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 370 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 400 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            l5 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 248, 1, l5);
                    }
                }

                if (inventoryManager.GetItemTotalCount(itemId) > 0)
                {
                    gameGraphics.DrawString("Deposit " + entityManager.GetItem(itemId).Name, i1 + 2, k1 + 273, 1, 0xffffff);

                    int i6 = 0xffffff;

                    if (InputManager.Instance.MouseLocation.X >= i1 + 220 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 250 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                    {
                        i6 = 0xff0000;
                    }

                    gameGraphics.DrawString("One", i1 + 222, k1 + 273, 1, i6);

                    if (inventoryManager.GetItemTotalCount(itemId) >= 5)
                    {
                        int j6 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 250 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 280 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            j6 = 0xff0000;
                        }

                        gameGraphics.DrawString("Five", i1 + 252, k1 + 273, 1, j6);
                    }

                    if (inventoryManager.GetItemTotalCount(itemId) >= 25)
                    {
                        int k6 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 280 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 305 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            k6 = 0xff0000;
                        }

                        gameGraphics.DrawString("25", i1 + 282, k1 + 273, 1, k6);
                    }

                    if (inventoryManager.GetItemTotalCount(itemId) >= 100)
                    {
                        int l6 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 305 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 335 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            l6 = 0xff0000;
                        }

                        gameGraphics.DrawString("100", i1 + 307, k1 + 273, 1, l6);
                    }

                    if (inventoryManager.GetItemTotalCount(itemId) >= 500)
                    {
                        int i7 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 335 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 368 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            i7 = 0xff0000;
                        }

                        gameGraphics.DrawString("500", i1 + 337, k1 + 273, 1, i7);
                    }

                    if (inventoryManager.GetItemTotalCount(itemId) >= 2500)
                    {
                        int j7 = 0xffffff;

                        if (InputManager.Instance.MouseLocation.X >= i1 + 370 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 400 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            j7 = 0xff0000;
                        }

                        gameGraphics.DrawString("2500", i1 + 370, k1 + 273, 1, j7);
                    }
                }
            }
        }

        public event EventHandler OnLoadingSection;
        public event EventHandler OnLoadingSectionCompleted;

        public bool loadSection(int x, int y)
        {
            if (PlayerAliveTimeout != 0)
            {
                engineHandle.playerIsAlive = false;
                return false;
            }
            LoadArea = false;
            x += WildLocation.X;
            y += WildLocation.Y;

            if (lastLayerIndex == LayerIndex && x > sectionWidth && x < sectionPosX && y > sectionHeight && y < sectionPosY)
            {
                engineHandle.playerIsAlive = true;
                return false;
            }

            OnLoadingSection?.Invoke(this, new EventArgs());
            gameGraphics.DrawText("Loading... Please wait", 256, 192, 1, 0xffffff);

            int l = AreaLocation.X;
            int i1 = AreaLocation.Y;
            int xBase = (x + 24) / 48;
            int yBase = (y + 24) / 48;
            lastLayerIndex = LayerIndex;
            AreaLocation = new Point2D(xBase * 48 - 48, yBase * 48 - 48);
            sectionWidth = xBase * 48 - 32;
            sectionHeight = yBase * 48 - 32;
            sectionPosX = xBase * 48 + 32;
            sectionPosY = yBase * 48 + 32;
            engineHandle.loadSection(x, y, lastLayerIndex);

            AreaLocation = new Point2D(AreaLocation.X - WildLocation.X, AreaLocation.Y - WildLocation.Y);

            int offsetX = AreaLocation.X - l;
            int offsetY = AreaLocation.Y - i1;

            for (int objectIndex = 0; objectIndex < ObjectCount; objectIndex++)
            {
                ObjectLocations[objectIndex] = new Point2D(
                    ObjectLocations[objectIndex].X - offsetX,
                    ObjectLocations[objectIndex].Y - offsetY);

                int objX = ObjectLocations[objectIndex].X;
                int objY = ObjectLocations[objectIndex].Y;
                int objType = ObjectType[objectIndex];
                ObjectModel _obj = ObjectArray[objectIndex];

                try
                {
                    int objDir = ObjectRotation[objectIndex];
                    int objWidth;
                    int objHeight;

                    if (objDir == 0 || objDir == 4)
                    {
                        objWidth = entityManager.GetWorldObject(objType).Width;
                        objHeight = entityManager.GetWorldObject(objType).Height;
                    }
                    else
                    {
                        objHeight = entityManager.GetWorldObject(objType).Width;
                        objWidth = entityManager.GetWorldObject(objType).Height;
                    }

                    int flatObjX = ((objX + objX + objWidth) * GridSize) / 2;
                    int flatObjY = ((objY + objY + objHeight) * GridSize) / 2;

                    if (objX >= 0 && objY >= 0 && objX < 96 && objY < 96)
                    {
                        gameCamera.addModel(_obj);

                        Point3D location = new Point3D(flatObjX, -engineHandle.getAveragedElevation(flatObjX, flatObjY), flatObjY);

                        _obj.setLocation(location);
                        engineHandle.createObject(objX, objY, objType, objDir);

                        if (objType == 74)
                        {
                            location = new Point3D(0, -480, 0);
                            _obj.setLocation(location);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Loc Error: " + ex);
                    Console.WriteLine("x:" + objectIndex + " obj:" + _obj);
                }
            }

            for (int wallIndex = 0; wallIndex < WallObjectCount; wallIndex++)
            {
                WallObjectLocations[wallIndex] = new Point2D(
                    WallObjectLocations[wallIndex].X - offsetX,
                    WallObjectLocations[wallIndex].Y - offsetY);

                int wallId = WallObjectId[wallIndex];
                int wallDir = WallObjectDirection[wallIndex];

                try
                {
                    engineHandle.createWall(WallObjectLocations[wallIndex], wallDir, wallId);
                    ObjectModel wallObject = makeWallObject(WallObjectLocations[wallIndex], wallDir, wallId, wallIndex);
                    WallObjects[wallIndex] = wallObject;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Bound Error: " + ex);
                }
            }

            for (int groundItemIndex = 0; groundItemIndex < GroundItemCount; groundItemIndex++)
            {
                GroundItemLocations[groundItemIndex] = new Point2D(
                    GroundItemLocations[groundItemIndex].X - offsetX,
                    GroundItemLocations[groundItemIndex].Y - offsetY);
            }

            for (int playerIndex = 0; playerIndex < PlayerCount; playerIndex++)
            {
                ClientMob player = Players[playerIndex];
                player.Location = new Point2D(
                    player.Location.X - offsetX * GridSize,
                    player.Location.Y - offsetY * GridSize);

                for (int waypointIndex = 0; waypointIndex <= player.WaypointCurrent; waypointIndex++)
                {
                    player.Waypoints[waypointIndex] = new Point2D(
                        player.Waypoints[waypointIndex].X - offsetX * GridSize,
                        player.Waypoints[waypointIndex].Y - offsetY * GridSize);
                }
            }

            for (int npcIndex = 0; npcIndex < NpcCount; npcIndex++)
            {
                ClientMob npc = Npcs[npcIndex];
                npc.Location = new Point2D(
                    npc.Location.X - offsetX * GridSize,
                    npc.Location.Y - offsetY * GridSize);

                for (int waypointIndex = 0; waypointIndex <= npc.WaypointCurrent; waypointIndex++)
                {
                    npc.Waypoints[waypointIndex] = new Point2D(
                        npc.Waypoints[waypointIndex].X - offsetX * GridSize,
                        npc.Waypoints[waypointIndex].Y - offsetY * GridSize);
                }
            }

            engineHandle.playerIsAlive = true;
            OnLoadingSectionCompleted?.Invoke(this, new EventArgs());
            OnDrawDone();

            return true;
        }

        public ObjectModel makeWallObject(Point2D location, int dir, int type, int totalCount)
        {
            int tileX = location.X;
            int tileY = location.Y;
            int destTileX = location.X;
            int destTileY = location.Y;
            int textureBack = entityManager.GetWallObject(type).ModelFaceBack;
            int textureFront = entityManager.GetWallObject(type).ModelFaceFront;
            int wallHeight = entityManager.GetWallObject(type).ModelHeight;
            ObjectModel wallModel = new ObjectModel(4, 1);

            // -
            if (dir == 0)
            {
                destTileX = location.X + 1;
            }

            // |
            if (dir == 1)
            {
                destTileY = location.Y + 1;
            }

            // /
            if (dir == 2)
            {
                tileX = location.X + 1;
                destTileY = location.Y + 1;
            }

            // \
            if (dir == 3)
            {
                destTileX = location.X + 1;
                destTileY = location.Y + 1;
            }

            tileX *= GridSize;
            tileY *= GridSize;
            destTileX *= GridSize;
            destTileY *= GridSize;

            // add vertex index bottomLeft
            int bLeft = wallModel.getVertexIndex(tileX, -engineHandle.getAveragedElevation(tileX, tileY), tileY);

            // add vertex index topLeft
            int tLeft = wallModel.getVertexIndex(tileX, -engineHandle.getAveragedElevation(tileX, tileY) - wallHeight, tileY);

            // add vertex index topRight
            int tRight = wallModel.getVertexIndex(destTileX, -engineHandle.getAveragedElevation(destTileX, destTileY) - wallHeight, destTileY);

            // vertex index bottomRight
            int bRight = wallModel.getVertexIndex(destTileX, -engineHandle.getAveragedElevation(destTileX, destTileY), destTileY);
            int[] faceVertices = { bLeft, tLeft, tRight, bRight };
            Point3D shadingPoint = new Point3D(-50, -10, -50);

            wallModel.addFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, shadingPoint);

            if (location.X >= 0 &&
                location.Y >= 0 &&
                location.X < 96 &&
                location.Y < 96)
            {
                gameCamera.addModel(wallModel);
            }

            wallModel.index = totalCount + 10000;

            return wallModel;
        }

        public ClientMob AddNpc(int serverIndex, int x, int y, int sprite, int id)
        {
            if (NpcAttackingArray[serverIndex] == null)
            {
                NpcAttackingArray[serverIndex] = new ClientMob();
                NpcAttackingArray[serverIndex].ServerIndex = serverIndex;
            }

            ClientMob mob = NpcAttackingArray[serverIndex];

            bool alreadyExists = LastNpcs
                .Take(LastNpcCount)
                .Any(lastNpc => lastNpc.ServerIndex == serverIndex);

            if (alreadyExists)
            {
                mob.npcId = id;
                mob.nextSprite = sprite;

                int waypointCurrent = mob.WaypointCurrent;

                if (x != mob.Waypoints[waypointCurrent].X ||
                    y != mob.Waypoints[waypointCurrent].Y)
                {
                    mob.WaypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                    mob.Waypoints[waypointCurrent] = new Point2D(x, y);
                }
            }
            else
            {
                mob.ServerIndex = serverIndex;
                mob.npcId = id;
                mob.nextSprite = mob.currentSprite = sprite;
                mob.stepCount = 0;
                mob.WaypointsEndSprite = 0;
                mob.WaypointCurrent = 0;
                mob.Location = new Point2D(x, y);
                mob.Waypoints[0] = mob.Location;
            }

            Npcs[NpcCount] = mob;
            NpcCount += 1;

            return mob;
        }

        public void drawRightClickMenu()
        {
            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < menuOptionsCount; l++)
                {
                    int j1 = menuX + 2;
                    int l1 = menuY + 27 + l * 15;
                    if (InputManager.Instance.MouseLocation.X <= j1 - 2 || InputManager.Instance.MouseLocation.Y <= l1 - 12 || InputManager.Instance.MouseLocation.Y >= l1 + 4 || InputManager.Instance.MouseLocation.X >= (j1 - 3) + menuWidth)
                    {
                        continue;
                    }

                    menuClick(menuIndexes[l]);
                    break;
                }

                mouseButtonClick = 0;
                menuShow = false;
                return;
            }
            if (InputManager.Instance.MouseLocation.X < menuX - 10 || InputManager.Instance.MouseLocation.Y < menuY - 10 || InputManager.Instance.MouseLocation.X > menuX + menuWidth + 10 || InputManager.Instance.MouseLocation.Y > menuY + menuHeight + 10)
            {
                menuShow = false;
                return;
            }
            gameGraphics.drawBoxAlpha(menuX, menuY, menuWidth, menuHeight, 0xd0d0d0, 160);
            gameGraphics.DrawString("Choose option", menuX + 2, menuY + 12, 1, 65535);
            for (int i1 = 0; i1 < menuOptionsCount; i1++)
            {
                int k1 = menuX + 2;
                int i2 = menuY + 27 + i1 * 15;
                int j2 = 0xffffff;
                if (InputManager.Instance.MouseLocation.X > k1 - 2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && InputManager.Instance.MouseLocation.X < (k1 - 3) + menuWidth)
                {
                    j2 = 0xffff00;
                }

                var t2 = menuText2[menuIndexes[i1]];
                gameGraphics.DrawString(menuText1[menuIndexes[i1]] + " " + menuText2[menuIndexes[i1]], k1, i2, 1, j2);
            }

        }

        public void getMenuHighlighted()
        {
            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 33 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 33 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 66 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 66 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 99 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 99 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 132 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 132 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 165 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 165 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab != 0 && drawMenuTab != 2 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 33 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 33 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 66 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 66 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 99 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 99 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 132 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 132 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.GameSize.Width - 35 - 165 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 3 - 165 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab == 1 && (InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 248 || InputManager.Instance.MouseLocation.Y > 36 + (InventoryManager.MaximumInventorySize / 5) * 34))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 3 && (InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 199 || InputManager.Instance.MouseLocation.Y > 316))
            {
                drawMenuTab = 0;
            }

            if ((drawMenuTab == 2 || drawMenuTab == 4 || drawMenuTab == 5) && (InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 199 || InputManager.Instance.MouseLocation.Y > 240))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 6 && (InputManager.Instance.MouseLocation.X < gameGraphics.GameSize.Width - 199 || InputManager.Instance.MouseLocation.Y > 326))
            {
                drawMenuTab = 0;
            }
        }

        public ClientMob GetLastPlayer(int serverIndex)
        {
            return LastPlayers
                .Where(x => x != null) // TODO: Remove this check once it is safe
                .FirstOrDefault(x => x.ServerIndex == serverIndex);
        }

        public ClientMob GetLastNpc(int serverIndex)
        {
            return LastNpcs
                .Where(x => x != null) // TODO: Remove this check once it is safe
                .FirstOrDefault(x => x.ServerIndex == serverIndex);
        }

        public string joinString(string[] hay, string glue, int start)
        {
            string ret = "";
            for (int i = start; i < hay.Length; i++)
            {
                ret += hay[i] + (i != hay.Length - 1 ? glue : "");
            }

            return ret;
        }

        public string joinString(string[] hay, string glue)
        {
            return joinString(hay, glue, 0);
        }

        public int cameraFieldOfView;
        public string[] questionMenuAnswer;
        public int appearanceHeadType;
        public int appearanceBodyGender;
        public int appearance2Colour;
        public int appearanceHairColour;
        public int appearanceTopColour;
        public int appearanceBottomColour;
        public int appearanceSkinColour;
        public int appearanceHeadGender;

        public int[] menuIndexes;
        public int selectedShopItemIndex;
        public int selectedShopItemType;
        public string sleepingStatusText;
        public string[] menuText1;
        public bool IsSleeping;
        public int modelFireLightningSpellNumber;
        public int modelTorchNumber;
        public int modelClawSpellNumber;
        public int[] itemAboveHeadScale;
        public int[] itemAboveHeadID;
        public MenuAction[] menuActions;
        public int cameraAutoRotatePlayerX;
        public int cameraAutoRotatePlayerY;
        public Menu appearanceMenu;
        public int[][] animationModelArray = new int[][]
        { new int[]{
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        }, new int[]{
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        }, new int[]{
            11, 3, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 4
        }, new int[]{
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        }, new int[]{
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        }, new int[]{
            4, 3, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        }, new int[]{
            11, 4, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 3
        }, new int[]{
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            4, 3
        }
    };

        public int drawUpdatesPerformed;
        public string serverMessage;
        public bool serverMessageBoxTop;
        public int cameraRotationYAmount;
        public int cameraRotationYIncrement;
        public int[] walkModel = {
        0, 1, 2, 1
    };
        public int itemsAboveHeadCount;
        public string[] messagesArray;
        public Menu questMenu;
        int questMenuHandle;
        int questMenuSelected;
        public bool[] objectAlreadyInMenu;
        public ObjectModel[] ObjectArray;
        public int selectedSpell;
        public bool cameraAutoAngleDebug;
        public int[] menuActionType;
        public int[] menuActionVar1;
        public int[] menuActionVar2;
        public bool sleepWordDelay;
        public int minimapRandomRotationX;
        public int minimapRandomRotationY;
        public int loginMenuOkButton;
        public int cameraRotation;
        public int[] appearanceSkinColours = {
        0xecded0, 0xccb366, 0xb38c40, 0x997326, 0x906020
    };
        public bool menuShow;
        public Menu loginMenuLogin;
        public int appearanceHeadLeftArrow;
        public int appearanceHeadRightArrow;
        public int appearanceHairLeftArrow;
        public int appearanceHairRightArrow;
        public int appearanceGenderLeftArrow;
        public int appearanceGenderRightArrow;
        public int appearanceTopLeftArrow;
        public int appearanceTopRightArrow;
        public int appearanceSkinLeftArrow;
        public int appearanceSkingRightArrow;
        public int appearanceBottomLeftArrow;
        public int appearanceBottomRightArrow;
        public int appearanceAcceptButton;
        public int shopItemSellPriceModifier;
        public int shopItemBuyPriceModifier;
        public GameImageMiddleMan gameGraphics;
        public int tick;
        public EngineHandle engineHandle;
        public int mouseButtonClick;
        public Menu loginNewUser;
        public int[] combatModelArray2 = {
        0, 0, 0, 0, 0, 1, 2, 1
    };
        public int cameraDistance;
        public int[] receivedMessageX;
        public int[] receivedMessageY;
        public int[] receivedMessageMidPoint;
        public int[] receivedMessageHeight;
        public int lastLayerIndex;
        public bool errorLoading;
        public int animationNumber;
        public int[] itemAboveHeadX;
        public int[] itemAboveHeadY;
        public int loginScreenNumber;
        public int tradeConfigItemCount;
        public int selectedBankItem;
        public int selectedBankItemType;
        public int modelUpdatingTimer;
        public int selectedItem;
        string selectedItemName;
        public int loginButtonNewUser;
        public int loginMenuLoginButton;
        public int mouseTrailIndex;
        int[] mouseTrailX;
        int[] mouseTrailY;
        public bool[] prayerOn;
        public int loginMenuStatusText;
        public int loginMenuUserText;
        public int loginMenuPasswordText;
        public int loginMenuOkLoginButton;
        public int loginMenuCancelButton;
        public int[] shopItems;
        public int[] shopItemCount;
        public int[] shopItemBasePriceModifier;
        public int baseInventoryPic;
        public int baseScrollPic;
        public int baseItemPicture;
        public int baseProjectilePic;
        public int baseTexturePic;
        public int subTexturePic;
        public int baseLoginScreenBackgroundPic;
        public int sectionWidth;
        public int sectionHeight;
        public int sectionPosX;
        public int sectionPosY;
        public int drawMenuTab;
        public int receivedMessagesCount;
        string[] receivedMessages;
        public int cameraRotateTime;
        public int cameraRotationXAmount;
        public int cameraRotationXIncrement;
        public int[] teleBubbleTime;
        public string[] gearStats = {
        "Armour", "WeaponAim", "WeaponPower", "Magic", "Prayer"
    };
        public bool loggedIn;
        public int[] teleBubbleType;
        public int[] experienceList;
        public int lastModelFireLightningSpellNumber;
        public int lastModelTorchNumber;
        public int lastModelClawSpellNumber;
        public int[] messagesTimeout;
        public int[] appearanceTopBottomColours = {
        0xff0000, 0xff8000, 0xffe000, 0xa0e000, 57344, 32768, 41088, 45311, 33023, 12528,
        0xe000e0, 0x303030, 0x604000, 0x805000, 0xffffff
    };
        public int teleBubbleCount;
        public bool memoryError;
        public int[] appearanceHairColours = {
        0xffc030, 0xffa040, 0x805030, 0x604020, 0x303030, 0xff6020, 0xff4000, 0xffffff, 65280, 65535
    };
        public Menu spellMenu;
        int spellMenuHandle;
        int menuMagicPrayersSelected;
        public int menuX;
        public int menuY;
        public int menuWidth;
        public int menuHeight;
        public int menuOptionsCount;
        public Camera gameCamera;
        public int healthBarVisibleCount;
        public string[] menuText2;
        public int sleepWordDelayTimer;
        public int mouseButtonHeldTime;
        public string loginUsername;
        public string loginPassword;
        public int bankPage;
        public Menu loginMenuFirst;
        public int[] healthBarX;
        public int[] healthBarY;
        public int[] healthBarMissing;
        public int[] teleBubbleY;
        public int cameraAutoAngle;
        public int cameraAutoRotationAmount;
        public int[] teleBubbleX;
        public int actionPictureType;
        int walkMouseX;
        int walkMouseY;
        public int[] combatModelArray1 = {
        0, 1, 2, 1, 0, 0, 0, 0
    };
        public bool cameraZoom;

        public int[] shopItemSellPrice;
        public int[] shopItemBuyPrice;
        public int[][] captchaPixels;
        public int captchaWidth;
        public int captchaHeight;
    }
}
