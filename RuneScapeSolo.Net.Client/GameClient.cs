using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RuneScapeSolo.GameLogic.GameManagers;
using RuneScapeSolo.Input;
using RuneScapeSolo.Net.Client.Data;
using RuneScapeSolo.Net.Client.Enumerations;
using RuneScapeSolo.Net.Client.Events;
using RuneScapeSolo.Net.Client.Game;
using RuneScapeSolo.Net.Client.Game.Cameras;
using RuneScapeSolo.Settings;

using ObjectModel = RuneScapeSolo.Net.Client.Game.ObjectModel;

namespace RuneScapeSolo.Net.Client
{
    public class GameClient : GameAppletMiddleMan
    {
        public event EventHandler OnContentLoadedCompleted;
        public event EventHandler<ContentLoadedEventArgs> OnContentLoaded;

        public static GameClient CreateGameClient(string title = "RuneScape Solo", int width = 512, int height = 346)
        {
            //spriteBatch = sb;

            GameClient mud = new GameClient();
            //graphics = gfx;
            mud.windowWidth = width;
            mud.windowHeight = height;
            mud.CreateWindow(mud.windowWidth, mud.windowHeight + 11, title, false);
            mud.gameMinThreadSleepTime = 10;
            return mud;
        }

        readonly PacketHandler packetHandler;

        List<Keys> lastPressedKeys = new List<Keys>();
        int lastMouseX = 0;
        int lastMouseY = 0;
        TimeSpan timeLapse = TimeSpan.Zero;

        public ObjectModel[] GameDataObjects { get; set; }
        public ObjectModel[] WallObjects { get; set; }
        public Mob CurrentPlayer { get; set; }
        public Mob[] LastNpcs { get; set; }
        public Mob[] LastPlayers { get; set; }
        public Mob[] Mobs { get; set; }
        public Mob[] Npcs { get; set; }
        public Mob[] NpcAttackingArray { get; set; }
        public Mob[] Players { get; set; }
        public Quests Quests { get; set; }
        public int AreaX { get; set; }
        public int AreaY { get; set; }
        public int CompletedTasks { get; set; }
        public int Deaths { get; set; }
        public int DropPartyTimer;
        public int GridSize { get; set; }
        public int GroundItemCount { get; set; }
        public int GuthixSpells { get; set; }
        public int PvpTournamentTimer;
        public int WildernessModeTimer;
        public int Tutorial { get; set; }
        public int TaskCash { get; set; }
        public int TaskExperience { get; set; }
        public int TaskItem { get; set; }
        public int TaskPoints { get; set; }
        public int TaskStatus { get; set; }
        public int InventoryItemsCount { get; set; }
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
        public int ProjectileRange { get; set; }
        public int QuestPoints { get; set; }
        public int QuestionMenuCount { get; set; }
        public int Remaining { get; set; }
        public int SaradominSpells { get; set; }
        public int SectionX { get; set; }
        public int SectionY { get; set; }
        public int ServerStartTime { get; set; }
        public int ServerIndex { get; set; }
        public int SubscriptionDaysLeft { get; set; }
        public int SystemUpdateTimer { get; set; }
        public int WallObjectCount { get; set; }
        public int WildX { get; set; }
        public int WildY { get; set; }
        public int ZamorakSpells { get; set; }
        public int[] EquipmentStatus { get; set; }
        public int[] GroundItemId { get; set; }
        public int[] GroundItemObjectVar { get; set; }
        public int[] GroundItemX { get; set; }
        public int[] GroundItemY { get; set; }
        public int[] InventoryItems { get; set; }
        public int[] InventoryItemCount { get; set; }
        public int[] InventoryItemEquipped { get; set; }
        public int[] ObjectRotation { get; set; }
        public int[] ObjectType { get; set; }
        public int[] ObjectX { get; set; }
        public int[] ObjectY { get; set; }
        public int[] PlayerStatBase { get; set; }
        public int[] PlayerStatCurrent { get; set; }
        public int[] PlayerStatExperience { get; set; }
        public int[] PlayersBufferIndexes { get; set; }
        public int[] WallObjectDirection { get; set; }
        public int[] WallObjectId { get; set; }
        public int[] WallObjectX { get; set; }
        public int[] WallObjectY { get; set; }
        public bool AutoScreenshot { get; set; }
        public bool CameraAutoAngle { get; set; }
        public bool HasWorldInfo { get; set; }
        public bool LoadArea { get; set; } // Not in wilderness
        public bool LoginScreenShown { get; set; }
        public bool NeedsClear { get; set; }
        public bool OneMouseButton { get; set; }
        public bool ShowAppearanceWindow { get; set; }
        public bool ShowBankBox { get; set; }
        public bool ShowCombatWindow { get; set; }
        public bool ShowDuelBox { get; set; }
        public bool ShowDuelConfirmBox { get; set; }
        public bool ShowQuestionMenu { get; set; }
        public bool ShowRoofs { get; set; }
        public bool ShowShopBox { get; set; }
        public bool ShowWelcomeBox { get; set; }
        public bool SoundOff { get; set; }
        public bool[] WallObjectAlreadyInMenu { get; set; }
        public string LastLoginAddress { get; set; }
        public string MoneyTask { get; set; }
        public string ServerLocation { get; set; }
        
        public char TranslateOemKeys(Keys k)
        {
            //   if (k == Keys.1)
            //  { }
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

        public void Update(GameTime gt)
        {
            var lastUpdate = gt.ElapsedGameTime;

            var keyboardState = Keyboard.GetState();

            var mouseState = Mouse.GetState();
            List<Keys> keysPressedDown = new List<Keys>();
            keysPressedDown.AddRange(keyboardState.GetPressedKeys());

            foreach (var k in keysPressedDown)
            {
                //   if (timeLapse > TimeSpan.FromMilliseconds(100))                
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
                //handleKeyDown(k, c[0]);
            }

            foreach (var lk in lastPressedKeys)
            {
                if (!keysPressedDown.Contains(lk))
                {
                    KeyUp(lk, TranslateOemKeys(lk));
                }
            }

            lastPressedKeys.Clear();
            lastPressedKeys.AddRange(keyboardState.GetPressedKeys());

            timeLapse += lastUpdate;

            //mouseEntered(mouseState);
            if (mouseState.X != lastMouseX || mouseState.Y != lastMouseY)
            {
                MouseMove(mouseState.X, mouseState.Y);
                lastMouseX = mouseState.X;
                lastMouseY = mouseState.Y;
                //mouseButtonClick = 0;
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
                MouseUp(mouseState.X, mouseState.Y);
            }

            if (mouseState.LeftButton == ButtonState.Released)
            {
                MouseUp(mouseState.X, mouseState.Y);
            }
        }

        public GameClient()
        {
            packetHandler = new PacketHandler(this);

            Quests = new Quests();

            tradeOtherName = "";

            windowWidth = 512;
            windowHeight = 334;

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
            duelMyItems = new int[8];
            duelMyItemsCount = new int[8];
            Players = new Mob[500];
            selectedShopItemIndex = -1;
            selectedShopItemType = -2;
            menuText1 = new string[250];
            IsSleeping = false;
            tradeItemsOther = new int[14];
            tradeItemOtherCount = new int[14];
            tradeOtherAccepted = false;
            tradeWeAccepted = false;
            itemAboveHeadScale = new int[50];
            itemAboveHeadID = new int[50];
            PlayerStatCurrent = new int[18];
            menuActionX = new int[250];
            menuActionY = new int[250];
            menuActions = new MenuAction[250];
            showTradeBox = false;
            Npcs = new Mob[500];
            duelNoRetreating = false;
            duelNoMagic = false;
            duelNoPrayer = false;
            duelNoWeapons = false;
            Mobs = new Mob[4000];
            serverMessage = "";
            duelOpponentAccepted = false;
            duelMyAccepted = false;
            WallObjectX = new int[500];
            WallObjectY = new int[500];
            serverMessageBoxTop = false;
            cameraRotationYIncrement = 2;
            WallObjects = new ObjectModel[500];
            messagesArray = new string[5];
            objectAlreadyInMenu = new bool[1500];
            ObjectArray = new ObjectModel[1500];
            selectedSpell = -1;
            cameraAutoAngleDebug = false;
            CurrentPlayer = new Mob();
            ServerIndex = -1;
            tradeItemsOur = new int[14];
            tradeItemOurCount = new int[14];
            ShowWelcomeBox = false;
            menuActionType = new int[250];
            menuActionVar1 = new int[250];
            menuActionVar2 = new int[250];
            sleepWordDelay = true;
            CameraAutoAngle = true;
            cameraRotation = 128;
            SoundOff = false;
            menuShow = false;
            duelOpponentItems = new int[8];
            duelOpponentItemsCount = new int[8];
            ShowBankBox = false;
            PlayerStatBase = new int[18];
            serverBankItems = new int[256];
            serverBankItemCount = new int[256];
            ShowShopBox = false;
            GroundItemX = new int[5000];
            GroundItemY = new int[5000];
            GroundItemId = new int[5000];
            GroundItemObjectVar = new int[5000];
            maxBankItems = 48;
            tradeConfirmOtherItems = new int[14];
            tradeConfirmOtherItemsCount = new int[14];
            LayerIndex = -1;
            walkArrayX = new int[8000];
            walkArrayY = new int[8000];
            cameraDistance = 550;
            receivedMessageX = new int[50];
            receivedMessageY = new int[50];
            receivedMessageMidPoint = new int[50];
            receivedMessageHeight = new int[50];
            WallObjectAlreadyInMenu = new bool[500];
            lastLayerIndex = -1;
            bankItems = new int[256];
            bankItemCount = new int[256];
            maxInventoryItems = 30;
            errorLoading = false;
            itemAboveHeadX = new int[50];
            itemAboveHeadY = new int[50];
            showServerMessageBox = false;
            PlayersBufferIndexes = new int[500];
            tradeConfirmItems = new int[14];
            tradeConfigItemsCount = new int[14];
            selectedBankItem = -1;
            selectedBankItemType = -2;
            ShowDuelConfirmBox = false;
            duelConfirmOurAccepted = false;
            WallObjectDirection = new int[500];
            WallObjectId = new int[500];
            GameDataObjects = new ObjectModel[1000];
            LastNpcs = new Mob[500];
            InventoryItems = new int[35];
            InventoryItemCount = new int[35];
            InventoryItemEquipped = new int[35];
            selectedItem = -1;
            selectedItemName = "";
            LastPlayers = new Mob[500];
            showTradeConfirmBox = false;
            tradeConfirmAccepted = false;
            PlayerStatExperience = new int[18];
            mouseTrailX = new int[8192];
            mouseTrailY = new int[8192];
            OneMouseButton = false;
            prayerOn = new bool[50];
            shopItems = new int[256];
            shopItemCount = new int[256];
            shopItemBasePriceModifier = new int[256];
            duelOpponentStakeItem = new int[8];
            duelOutStakeItemCount = new int[8];
            EquipmentStatus = new int[5];
            receivedMessages = new string[50];
            cameraRotationXIncrement = 2;
            teleBubbleTime = new int[50];
            GridSize = 128;
            questStage = new int[questName.Length];
            teleBubbleType = new int[50];
            experienceList = new int[99];
            lastModelFireLightningSpellNumber = -1;
            lastModelTorchNumber = -1;
            lastModelClawSpellNumber = -1;
            messagesTimeout = new int[5];
            ProjectileRange = 40;
            memoryError = false;
            duelOurStakeItem = new int[8];
            duelOurStakeItemCount = new int[8];
            menuText2 = new string[250];
            loginUsername = "";
            loginPassword = "";
            duelOpponent = "";
            healthBarX = new int[50];
            healthBarY = new int[50];
            healthBarMissing = new int[50];
            ObjectX = new int[1500];
            ObjectY = new int[1500];
            ObjectType = new int[1500];
            ObjectRotation = new int[1500];
            ShowDuelBox = false;
            NpcAttackingArray = new Mob[5000];
            teleBubbleY = new int[50];
            cameraAutoAngle = 1;
            LoadArea = false;
            teleBubbleX = new int[50];
            ShowAppearanceWindow = false;
            cameraZoom = false;

            fogOfWar = true;
            ShowCombatWindow = false;
            ShowRoofs = true;
            AutoScreenshot = false;
            useChatFilter = true;
            usedQuestName = new string[0];
            SubscriptionDaysLeft = 0;
            shopItemSellPrice = new int[256];
            shopItemBuyPrice = new int[256];
            captchaPixels = new int[0][];
            captchaWidth = 0;
            captchaHeight = 0;
            NeedsClear = false;
            HasWorldInfo = false;
            //ImageIO.setCacheDirectory(new File(Config.CONF_DIR));
        }

        //public void Draw(GameTime gt)
        //{
        //    if (gameGraphics != null)
        //    {
        //        try
        //        {
        //            //   gameGraphics.UpdateGameImage();

        //            //  drawWindow();

        //            gameGraphics.drawImage(spriteBatch, 0, 0);

        //            //    //mudclient.spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);
        //            //    foreach (var str in GameImage.stringsToDraw)
        //            //    {

        //            //        //mudclient.gameFont12
        //            //        if (!mudclient.spriteBatch.BeginIsActive()) return;
        //            //        //var color = new Color(startColor >> 0x0000ff, startColor >> 0x00ff00, startColor >> 0xff0000, 255);

        //            //        Color clr = str.forecolor;
        //            //        SpriteFont font = mudclient.gameFont12;

        //            //        //if (clr.A == 0 || clr.A < 255)
        //            //        //    clr = new Color(255, 255, 255, 255);

        //            //        if (str.font != null)
        //            //        {
        //            //            font = str.font;
        //            //        }
        //            //        var textToRender = str.text;
        //            //        //textToRender = textToRender.Replace("@gre@", "");
        //            //        //textToRender = textToRender.Replace("@yel@", "");
        //            //        //textToRender = textToRender.Replace("@whi@", "");
        //            //        //textToRender = textToRender.Replace("@bla@", "");
        //            //        //textToRender = textToRender.Replace("@ran@", "");
        //            //        //textToRender = textToRender.Replace("@red@", "");

        //            //        mudclient.spriteBatch.DrawString(font, textToRender, str.pos - new Vector2(0f, (float)gameFrame.yOffset / 2.5f), clr);


        //            //    }
        //        }
        //        catch { }

        //        ////mudclient.spriteBatch.End();

        //        //GameImage.stringsToDraw.Clear();
        //    }
        //}


        public void menuClick(int actionId)
        {
            int actionX = menuActionX[actionId];
            int actionY = menuActionY[actionId];
            int actionType = menuActionType[actionId];
            int actionVar1 = menuActionVar1[actionId];
            int actionVar2 = menuActionVar2[actionId];

            MenuAction action = menuActions[actionId];

            if (action == MenuAction.CastSpellOnGroundItem)
            {
                walkToGroundItem(SectionX, SectionY, actionX, actionY, true);
                StreamClass.CreatePacket(104);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (action == MenuAction.UseItemWithGroundItem)
            {
                walkToGroundItem(SectionX, SectionY, actionX, actionY, true);
                StreamClass.CreatePacket(34);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }
            if (action == MenuAction.TakeItem)
            {
                walkToGroundItem(SectionX, SectionY, actionX, actionY, true);
                StreamClass.CreatePacket(245);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.ExamineGroundItem)
            {
                displayMessage(EntityManager.GetItem(actionType).Description, 3);
            }

            if (action == MenuAction.CastSpellOnWallObject)
            {
                walkToWallObject(actionX, actionY, actionType);
                StreamClass.CreatePacket(67);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (action == MenuAction.UseItemWithWallObject)
            {
                walkToWallObject(actionX, actionY, actionType);
                StreamClass.CreatePacket(36);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt8(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }
            if (action == MenuAction.Command1OnWallObject)
            {
                walkToWallObject(actionX, actionY, actionType);
                StreamClass.CreatePacket(126);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.Command2OnWallObject)
            {
                walkToWallObject(actionX, actionY, actionType);
                StreamClass.CreatePacket(235);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt8(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.ExamineWallObject)
            {
                displayMessage(EntityManager.GetWallObject(actionType).Description, 3);
            }

            if (action == MenuAction.CastSpellOnModel)
            {
                walkToObject(actionX, actionY, actionType, actionVar1);
                StreamClass.CreatePacket(17);
                StreamClass.AddInt16(actionVar2);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);

                StreamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (action == MenuAction.UseItemWithModel)
            {
                walkToObject(actionX, actionY, actionType, actionVar1);
                StreamClass.CreatePacket(94);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.AddInt16(actionVar2);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }
            if (action == MenuAction.Command1OnModel)
            {
                walkToObject(actionX, actionY, actionType, actionVar1);
                StreamClass.CreatePacket(51);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.Command2OnModel)
            {
                walkToObject(actionX, actionY, actionType, actionVar1);
                StreamClass.CreatePacket(40);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.ExamineModel)
            {
                displayMessage(EntityManager.GetWorldObject(actionType).Description, 3);
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
                selectedItemName = EntityManager.GetItem(InventoryItems[selectedItem]).Name;
            }
            if (action == MenuAction.UseItem)
            {
                StreamClass.CreatePacket(147);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedItem = -1;
                drawMenuTab = 0;
                displayMessage("Dropping " + EntityManager.GetItem(InventoryItems[actionType]).Name, 4);
            }
            if (action == MenuAction.ExamineItem)
            {
                displayMessage(EntityManager.GetItem(actionType).Description, 3);
            }

            if (action == MenuAction.CastSpellOnNpc)
            {
                int k2 = (actionX - 64) / GridSize;
                int k4 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, k2, k4, true);
                StreamClass.CreatePacket(71);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (action == MenuAction.UseItemWithNpc)
            {
                int l2 = (actionX - 64) / GridSize;
                int l4 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, l2, l4, true);
                StreamClass.CreatePacket(142);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }
            if (action == MenuAction.TalkToNpc)
            {
                int i3 = (actionX - 64) / GridSize;
                int i5 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, i3, i5, true);
                StreamClass.CreatePacket(177);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.CommandOnNpc)
            {
                int j3 = (actionX - 64) / GridSize;
                int j5 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, j3, j5, true);
                StreamClass.CreatePacket(74);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.AttackNpc || action == MenuAction.AttackNpc2)
            {
                int k3 = (actionX - 64) / GridSize;
                int k5 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, k3, k5, true);
                StreamClass.CreatePacket(73);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }

            if (action == MenuAction.ExamineNpc)
            {
                displayMessage(EntityManager.GetNpc(actionType).Description, 3);
            }

            if (action == MenuAction.CastSpellOnPlayer)
            {
                int l3 = (actionX - 64) / GridSize;
                int l5 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, l3, l5, true);
                StreamClass.CreatePacket(55);
                StreamClass.AddInt16(actionVar1);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }

            if (action == MenuAction.UseItemWithPlayer)
            {
                int i4 = (actionX - 64) / GridSize;
                int i6 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, i4, i6, true);
                StreamClass.CreatePacket(16);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionVar1);
                StreamClass.FormatPacket();
                selectedItem = -1;
            }

            if (action == MenuAction.AttackPlayer || action == MenuAction.AttackPlayer2)
            {
                int j4 = (actionX - 64) / GridSize;
                int j6 = (actionY - 64) / GridSize;
                walkTo1Tile(SectionX, SectionY, j4, j6, true);
                StreamClass.CreatePacket(57);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.DuelWithPlayer)
            {
                StreamClass.CreatePacket(222);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.TradeWithPlayer)
            {
                StreamClass.CreatePacket(166);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.FollowPlayer)
            {
                StreamClass.CreatePacket(68);
                StreamClass.AddInt16(actionType);
                StreamClass.FormatPacket();
            }
            if (action == MenuAction.CastSpellOnGround)
            {
                walkTo1Tile(SectionX, SectionY, actionX, actionY, true);
                StreamClass.CreatePacket(232);
                StreamClass.AddInt16(actionType);
                StreamClass.AddInt16(actionX + AreaX);
                StreamClass.AddInt16(actionY + AreaY);
                StreamClass.FormatPacket();
                selectedSpell = -1;
            }
            if (action == MenuAction.WalkHere)
            {
                walkTo1Tile(SectionX, SectionY, actionX, actionY, false);
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
            SystemUpdateTimer = 0;

            WildernessModeTimer = 0;
            PvpTournamentTimer = 0;
            DropPartyTimer = 0;

            loginScreenNumber = 0;
            loggedIn = false;
            logoutTimer = 0;
        }

        public void drawReportAbuseBox1()
        {
            reportAbuseOptionSelected = 0;
            int yOffset = 135;

            for (int option = 0; option < 12; option++)
            {
                if (InputManager.Instance.MouseLocation.X > 66 && InputManager.Instance.MouseLocation.X < 446 && InputManager.Instance.MouseLocation.Y >= yOffset - 12 && InputManager.Instance.MouseLocation.Y < yOffset + 3)
                {
                    reportAbuseOptionSelected = option + 1;
                }

                yOffset += 14;
            }

            if (mouseButtonClick != 0 && reportAbuseOptionSelected != 0)
            {
                mouseButtonClick = 0;
                showAbuseBox = 2;
                inputText = "";
                enteredInputText = "";
                return;
            }
            yOffset += 15;
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                if (InputManager.Instance.MouseLocation.X < 56 || InputManager.Instance.MouseLocation.Y < 35 || InputManager.Instance.MouseLocation.X > 456 || InputManager.Instance.MouseLocation.Y > 325)
                {
                    showAbuseBox = 0;
                    return;
                }
                if (InputManager.Instance.MouseLocation.X > 66 && InputManager.Instance.MouseLocation.X < 446 && InputManager.Instance.MouseLocation.Y >= yOffset - 15 && InputManager.Instance.MouseLocation.Y < yOffset + 5)
                {
                    showAbuseBox = 0;
                    return;
                }
            }
            gameGraphics.drawBox(56, 35, 400, 290, 0);
            gameGraphics.drawBoxEdge(56, 35, 400, 290, 0xffffff);
            yOffset = 50;
            gameGraphics.drawText("This form is for reporting players who are breaking our rules", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            gameGraphics.drawText("Using it sends a snapshot of the last 60 secs of activity to us", 256, yOffset, 1, 0xffffff);
            yOffset += 15;
            gameGraphics.drawText("If you misuse this form, you will be banned.", 256, yOffset, 1, 0xff8000);
            yOffset += 15;
            yOffset += 10;
            gameGraphics.drawText("First indicate which of our 12 rules is being broken. For a detailed", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            gameGraphics.drawText("explanation of each rule please read the manual on our website.", 256, yOffset, 1, 0xffff00);
            yOffset += 15;
            int j1;
            if (reportAbuseOptionSelected == 1)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("1: Offensive language", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 2)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("2: Item scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 3)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("3: Password scamming", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 4)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("4: Bug abuse", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 5)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("5: Jagex Staff impersonation", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 6)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("6: Account sharing/trading", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 7)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("7: Macroing", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 8)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("8: Mutiple logging in", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 9)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("9: Encouraging others to break rules", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 10)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("10: Misuse of customer support", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 11)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("11: Advertising / website", 256, yOffset, 1, j1);
            yOffset += 14;
            if (reportAbuseOptionSelected == 12)
            {
                gameGraphics.drawBoxEdge(66, yOffset - 12, 380, 15, 0xffffff);
                j1 = 0xff8000;
            }
            else
            {
                j1 = 0xffffff;
            }
            gameGraphics.drawText("12: Real world item trading", 256, yOffset, 1, j1);
            yOffset += 14;
            yOffset += 15;
            j1 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > 196 && InputManager.Instance.MouseLocation.X < 316 && InputManager.Instance.MouseLocation.Y > yOffset - 15 && InputManager.Instance.MouseLocation.Y < yOffset + 5)
            {
                j1 = 0xffff00;
            }

            gameGraphics.drawText("Click here to cancel", 256, yOffset, 1, j1);
        }

        public void loadMap()
        {
            engineHandle.mapsFree = unpackData("maps.jag", "map", 70);
            engineHandle.mapsMembers = unpackData("maps.mem", "members map", 75);
            engineHandle.landscapeFree = unpackData("land.jag", "landscape", 80);
            engineHandle.landscapeMembers = unpackData("land.mem", "members landscape", 85);
        }

        public void drawModel(int l, string s1)
        {
            int i1 = ObjectX[l];
            int j1 = ObjectY[l];
            int k1 = i1 - CurrentPlayer.currentX / 128;
            int l1 = j1 - CurrentPlayer.currentY / 128;
            byte byte0 = 7;
            if (i1 >= 0 && j1 >= 0 && i1 < 96 && j1 < 96 && k1 > -byte0 && k1 < byte0 && l1 > -byte0 && l1 < byte0)
            {
                gameCamera.removeModel(ObjectArray[l]);
                int i2 = EntityManager.GetModelIndex(s1);
                ObjectModel j2 = GameDataObjects[i2].CreateParent();
                gameCamera.addModel(j2);
                j2.UpdateShading(true, 48, 48, -50, -10, -50);
                j2.CopyTranslation(ObjectArray[l]);
                j2.index = l;
                ObjectArray[l] = j2;
            }
        }

        public void DrawPlayer(int x, int y, int width, int height, int playerIndex, int arg5, int arg6)
        {
            Mob f1 = Players[playerIndex];
            if (f1.BottomColour == 255)// TODO this checks if the player is an invisible moderator
            {
                return;
            }

            int direction = f1.currentSprite + (cameraRotation + 16) / 32 & 7;
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
            int j1 = direction2 * 3 + walkModel[(f1.stepCount / 6) % 4];
            if (f1.currentSprite == 8)
            {
                direction2 = 5;
                direction = 2;
                flag = false;
                x -= (5 * arg6) / 100;
                j1 = direction2 * 3 + combatModelArray1[(tick / 5) % 8];
            }
            else
                if (f1.currentSprite == 9)
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
                int l2 = f1.AppearanceItems[l1] - 1;
                if (l2 > EntityManager.AnimationCount - 1)
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
                        if (EntityManager.GetAnimation(l2).HasF == 1)
                        {
                            j4 += 15;
                        }
                        else if (l1 == 4 && direction2 == 1)
                        {
                            k3 = -22;
                            i4 = -3;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = -8;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 4 && direction2 == 3)
                        {
                            k3 = 26;
                            i4 = -5;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 1)
                        {
                            k3 = 22;
                            i4 = 3;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 2)
                        {
                            k3 = 0;
                            i4 = 8;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                        else if (l1 == 3 && direction2 == 3)
                        {
                            k3 = -26;
                            i4 = 5;
                            j4 = direction2 * 3 + walkModel[(2 + f1.stepCount / 6) % 4];
                        }
                    }

                    if (direction2 != 5 || EntityManager.GetAnimation(l2).HasA == 1)
                    {
                        int k4 = j4 + EntityManager.GetAnimation(l2).Number;
                        k3 = (k3 * width) / gameGraphics.pictureAssumedWidth[k4];
                        i4 = (i4 * height) / gameGraphics.pictureAssumedHeight[k4];
                        int l4 = (width * gameGraphics.pictureAssumedWidth[k4]) / gameGraphics.pictureAssumedWidth[EntityManager.GetAnimation(l2).Number];
                        k3 -= (l4 - width) / 2;
                        int i5 = EntityManager.GetAnimation(l2).CharacterColour;
                        int j5 = appearanceSkinColours[f1.SkinColour];

                        if (i5 == 1)
                        {
                            i5 = appearanceHairColours[f1.HairColour];
                        }
                        else
                            if (i5 == 2)
                        {
                            i5 = appearanceTopBottomColours[f1.TopColour];
                        }
                        else
                                if (i5 == 3)
                        {
                            i5 = appearanceTopBottomColours[f1.BottomColour];
                        }

                        gameGraphics.drawImage(x + k3, y + i4, l4, height, k4, i5, j5, arg5, flag);
                    }
                }
            }

            if (f1.lastMessageTimeout > 0)
            {
                receivedMessageMidPoint[receivedMessagesCount] = gameGraphics.textWidth(f1.lastMessage, 1) / 2;
                if (receivedMessageMidPoint[receivedMessagesCount] > 150)
                {
                    receivedMessageMidPoint[receivedMessagesCount] = 150;
                }

                receivedMessageHeight[receivedMessagesCount] = (gameGraphics.textWidth(f1.lastMessage, 1) / 300) * gameGraphics.textHeightNumber(1);
                receivedMessageX[receivedMessagesCount] = x + width / 2;
                receivedMessageY[receivedMessagesCount] = y;
                receivedMessages[receivedMessagesCount++] = f1.lastMessage;
            }
            if (f1.PlayerSkullTimeout > 0)
            {
                itemAboveHeadX[itemsAboveHeadCount] = x + width / 2;
                itemAboveHeadY[itemsAboveHeadCount] = y;
                itemAboveHeadScale[itemsAboveHeadCount] = arg6;
                itemAboveHeadID[itemsAboveHeadCount++] = f1.ItemAboveHeavId;
            }
            if (f1.currentSprite == 8 || f1.currentSprite == 9 || f1.combatTimer != 0)
            {
                if (f1.combatTimer > 0)
                {
                    int i2 = x;
                    if (f1.currentSprite == 8)
                    {
                        i2 -= (20 * arg6) / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        i2 += (20 * arg6) / 100;
                    }

                    int i3 = (f1.CurrentHitpoints * 30) / f1.BaseHitpoints;
                    healthBarX[healthBarVisibleCount] = i2 + width / 2;
                    healthBarY[healthBarVisibleCount] = y;
                    healthBarMissing[healthBarVisibleCount++] = i3;
                }
                if (f1.combatTimer > 150)
                {
                    int j2 = x;
                    if (f1.currentSprite == 8)
                    {
                        j2 -= (10 * arg6) / 100;
                    }
                    else
                        if (f1.currentSprite == 9)
                    {
                        j2 += (10 * arg6) / 100;
                    }

                    gameGraphics.drawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 11);
                    gameGraphics.drawText(f1.LastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
            if (f1.PlayerSkulled == 1 && f1.PlayerSkullTimeout == 0)
            {
                int k2 = arg5 + x + width / 2;
                if (f1.currentSprite == 8)
                {
                    k2 -= (20 * arg6) / 100;
                }
                else
                    if (f1.currentSprite == 9)
                {
                    k2 += (20 * arg6) / 100;
                }

                int j3 = (16 * arg6) / 100;
                int l3 = (16 * arg6) / 100;
                gameGraphics.DrawEntity(k2 - j3 / 2, y - l3 / 2 - (10 * arg6) / 100, j3, l3, baseInventoryPic + 13);
            }
        }

        public void walkToWallObject(int x, int y, int direction)
        {
            if (direction == 0)
            {
                WalkTo(SectionX, SectionY, x, y - 1, x, y, false, true);
                return;
            }
            if (direction == 1)
            {
                WalkTo(SectionX, SectionY, x - 1, y, x, y, false, true);
                return;
            }
            else
            {
                WalkTo(SectionX, SectionY, x, y, x, y, true, true);
                return;
            }
        }

        public void drawDuelConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.drawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            gameGraphics.drawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            gameGraphics.drawText("Please confirm your duel with @yel@" + DataOperations.LongToString(duelOpponentHash), byte0 + 234, byte1 + 12, 1, 0xffffff);
            gameGraphics.drawText("Your stake:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < duelOurStakeCount; i1++)
            {
                string s1 = EntityManager.GetItem(duelOurStakeItem[i1]).Name;
                if (EntityManager.GetItem(duelOurStakeItem[i1]).IsStackable == 0)
                {
                    s1 = s1 + " x " + formatItemCount(duelOurStakeItemCount[i1]);
                }

                gameGraphics.drawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (duelOurStakeCount == 0)
            {
                gameGraphics.drawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.drawText("Your opponent's stake:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < duelOpponentStakeCount; j1++)
            {
                string s2 = EntityManager.GetItem(duelOpponentStakeItem[j1]).Name;
                if (EntityManager.GetItem(duelOpponentStakeItem[j1]).IsStackable == 0)
                {
                    s2 = s2 + " x " + formatItemCount(duelOutStakeItemCount[j1]);
                }

                gameGraphics.drawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (duelOpponentStakeCount == 0)
            {
                gameGraphics.drawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            if (duelRetreat == 0)
            {
                gameGraphics.drawText("You can retreat from this duel", byte0 + 234, byte1 + 180, 1, 65280);
            }
            else
            {
                gameGraphics.drawText("No retreat is possible!", byte0 + 234, byte1 + 180, 1, 0xff0000);
            }

            if (duelMagic == 0)
            {
                gameGraphics.drawText("Magic may be used", byte0 + 234, byte1 + 192, 1, 65280);
            }
            else
            {
                gameGraphics.drawText("Magic cannot be used", byte0 + 234, byte1 + 192, 1, 0xff0000);
            }

            if (duelPrayer == 0)
            {
                gameGraphics.drawText("Prayer may be used", byte0 + 234, byte1 + 204, 1, 65280);
            }
            else
            {
                gameGraphics.drawText("Prayer cannot be used", byte0 + 234, byte1 + 204, 1, 0xff0000);
            }

            if (duelWeapons == 0)
            {
                gameGraphics.drawText("Weapons may be used", byte0 + 234, byte1 + 216, 1, 65280);
            }
            else
            {
                gameGraphics.drawText("Weapons cannot be used", byte0 + 234, byte1 + 216, 1, 0xff0000);
            }

            gameGraphics.drawText("If you are sure click 'Accept' to begin the duel", byte0 + 234, byte1 + 230, 1, 0xffffff);
            if (!duelConfirmOurAccepted)
            {
                gameGraphics.drawPicture((byte0 + 118) - 35, byte1 + 238, baseInventoryPic + 25);
                gameGraphics.drawPicture((byte0 + 352) - 35, byte1 + 238, baseInventoryPic + 26);
            }
            else
            {
                gameGraphics.drawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (mouseButtonClick == 1)
            {
                if (InputManager.Instance.MouseLocation.X < byte0 || InputManager.Instance.MouseLocation.Y < byte1 || InputManager.Instance.MouseLocation.X > byte0 + 468 || InputManager.Instance.MouseLocation.Y > byte1 + 262)
                {
                    ShowDuelConfirmBox = false;
                    StreamClass.CreatePacket(35);
                    StreamClass.FormatPacket();
                }
                if (InputManager.Instance.MouseLocation.X >= (byte0 + 118) - 35 && InputManager.Instance.MouseLocation.X <= byte0 + 118 + 70 && InputManager.Instance.MouseLocation.Y >= byte1 + 238 && InputManager.Instance.MouseLocation.Y <= byte1 + 238 + 21)
                {
                    duelConfirmOurAccepted = true;
                    StreamClass.CreatePacket(87);
                    StreamClass.FormatPacket();
                }
                if (InputManager.Instance.MouseLocation.X >= (byte0 + 352) - 35 && InputManager.Instance.MouseLocation.X <= byte0 + 353 + 70 && InputManager.Instance.MouseLocation.Y >= byte1 + 238 && InputManager.Instance.MouseLocation.Y <= byte1 + 238 + 21)
                {
                    ShowDuelConfirmBox = false;
                    StreamClass.CreatePacket(35);
                    StreamClass.FormatPacket();
                }
                mouseButtonClick = 0;
            }
        }

        public void setLoginVars()
        {
            loggedIn = false;
            loginScreenNumber = 0;
            loginUsername = "";
            loginPassword = "";
            /*dja = "Please enter a username:";
            djb = "*" + loginUsername + "*";*/
            PlayerCount = 0;
            NpcCount = 0;
        }

        public override void Close()
        {
            requestLogout();
            cleanUp();
            if (audioPlayer != null)
            {
                audioPlayer.Stop();
            }
        }

        //protected TcpClient makeSocket(string address, int port) {

        //    if(link.gameApplet != null) {
        //        Socket socket = link.getSocket(port);
        //        if(socket == null)
        //            throw new IOException();
        //        else
        //            return socket;
        //    }
        //    Socket socket1 = new Socket(InetAddress.getByName(address), port);
        //    socket1.setSoTimeout(30000);
        //    socket1.setTcpNoDelay(true);
        //    return socket1;
        //}

        public void drawInventoryMenu(bool canRightClick)
        {
            int l = gameGraphics.gameWidth - 248;
            gameGraphics.drawPicture(l, 3, baseInventoryPic + 1);
            for (int i1 = 0; i1 < maxInventoryItems; i1++)
            {
                int j1 = l + (i1 % 5) * 49;
                int l1 = 36 + (i1 / 5) * 34;
                if (i1 < InventoryItemsCount && InventoryItemEquipped[i1] == 1)
                {
                    gameGraphics.drawBoxAlpha(j1, l1, 49, 34, 0xff0000, 128);
                }
                else
                {
                    gameGraphics.drawBoxAlpha(j1, l1, 49, 34, GameImage.RgbToInt(181, 181, 181), 128);
                }

                if (i1 < InventoryItemsCount)
                {
                    gameGraphics.drawImage(j1, l1, 48, 32, baseItemPicture + EntityManager.GetItem(InventoryItems[i1]).InventoryPicture, EntityManager.GetItem(InventoryItems[i1]).PictureMask, 0, 0, false);
                    if (EntityManager.GetItem(InventoryItems[i1]).IsStackable == 0)
                    {
                        gameGraphics.drawString(InventoryItemCount[i1].ToString(), j1 + 1, l1 + 10, 1, 0xffff00);
                    }
                }
            }

            for (int k1 = 1; k1 <= 4; k1++)
            {
                gameGraphics.drawLineY(l + k1 * 49, 36, (maxInventoryItems / 5) * 34, 0);
            }

            for (int i2 = 1; i2 <= maxInventoryItems / 5 - 1; i2++)
            {
                gameGraphics.drawLineX(l, 36 + i2 * 34, 245, 0);
            }

            if (!canRightClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 248);
            int j2 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 0 && j2 >= 0 && l < 248 && j2 < (maxInventoryItems / 5) * 34)
            {
                int k2 = l / 49 + (j2 / 34) * 5;
                if (k2 < InventoryItemsCount)
                {
                    int l2 = InventoryItems[k2];
                    if (selectedSpell >= 0)
                    {
                        if (EntityManager.GetSpell(selectedSpell).Type == 3)
                        {
                            menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                            menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                            menuActions[menuOptionsCount] = MenuAction.CastSpellOnItem;
                            menuActionType[menuOptionsCount] = k2;
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
                            menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                            menuActions[menuOptionsCount] = MenuAction.UseItemWithItem;
                            menuActionType[menuOptionsCount] = k2;
                            menuActionVar1[menuOptionsCount] = selectedItem;
                            menuOptionsCount++;
                            return;
                        }
                        if (InventoryItemEquipped[k2] == 1)
                        {
                            menuText1[menuOptionsCount] = "Remove";
                            menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                            menuActions[menuOptionsCount] = MenuAction.RemoveItem;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount++;
                        }
                        else
                            if (EntityManager.GetItem(l2).IsEquipable != 0)
                        {
                            if ((EntityManager.GetItem(l2).IsEquipable & 0x18) != 0)
                            {
                                menuText1[menuOptionsCount] = "Wield";
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Wear";
                            }

                            menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                            menuActions[menuOptionsCount] = MenuAction.EquipItem;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount++;
                        }
                        if (!EntityManager.GetItem(l2).Command.Equals(""))
                        {
                            menuText1[menuOptionsCount] = EntityManager.GetItem(l2).Command;
                            menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                            menuActions[menuOptionsCount] = MenuAction.CommandOnItem;
                            menuActionType[menuOptionsCount] = k2;
                            menuOptionsCount++;
                        }
                        menuText1[menuOptionsCount] = "Use";
                        menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                        menuActions[menuOptionsCount] = MenuAction.UseItem;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount++;
                        menuText1[menuOptionsCount] = "Drop";
                        menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                        menuActions[menuOptionsCount] = MenuAction.DropItem;
                        menuActionType[menuOptionsCount] = k2;
                        menuOptionsCount++;
                        menuText1[menuOptionsCount] = "Examine";
                        menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(l2).Name;
                        menuActions[menuOptionsCount] = MenuAction.ExamineItem;
                        menuActionType[menuOptionsCount] = l2;
                        menuOptionsCount++;
                    }
                }
            }
        }
        public bool DoNotDrawLogo { get; set; }
        public void createLoginScreenBackgrounds()
        {
            int _bgScreenWidth = windowWidth;
            OnLoadingSection?.Invoke(this, new EventArgs());
            int l = 0;
            sbyte byte0 = 50;
            sbyte byte1 = 50;

            engineHandle.loadSection(byte0 * 48 + 23, byte1 * 48 + 23, l);
            engineHandle.addObjects(GameDataObjects);

            //char c1 = '\u2600';
            //char c2 = '\u1900';
            //char c3 = '\u044C';
            //char c4 = '\u0378';

            int cameraX = 9728;
            int cameraY = 6400;
            int cameraDistance = 1100;
            int cameraRotation = 888;

            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.screenFadeToBlack();
            gameGraphics.screenFadeToBlack();



            gameGraphics.drawBox(0, 0, _bgScreenWidth, 6, 0x000000); //_bgScreenWidth=512
            for (int i1 = 6; i1 >= 1; i1--)
            {
                gameGraphics.drawTransparentLine(0, i1, 0, i1, _bgScreenWidth, 8);
            }

            gameGraphics.drawBox(0, 194, _bgScreenWidth, 20, 0x000000);

            for (int j1 = 6; j1 >= 1; j1--)
            {
                gameGraphics.drawTransparentLine(0, j1, 0, 194 - j1, _bgScreenWidth, 8);
            }


#warning draws logo

            if (!DoNotDrawLogo)
            {
                if (bgPixels == null)
                {
                    gameGraphics.drawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.drawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }


            gameGraphics.drawImage(baseLoginScreenBackgroundPic, 0, 0, _bgScreenWidth, 200);
            gameGraphics.applyImage(baseLoginScreenBackgroundPic);

            cameraX = 9216;
            cameraY = 9216;
            cameraDistance = 1100;
            cameraRotation = 888;
            gameCamera.zoom1 = 4100;
            gameCamera.zoom2 = 4100;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 4000;
            gameCamera.SetCameraTransform(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.screenFadeToBlack();
            gameGraphics.screenFadeToBlack();



            gameGraphics.drawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int k1 = 6; k1 >= 1; k1--)
            {
                gameGraphics.drawTransparentLine(0, k1, 0, k1, _bgScreenWidth, 8);
            }

            gameGraphics.drawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int l1 = 6; l1 >= 1; l1--)
            {
                gameGraphics.drawTransparentLine(0, l1, 0, 194 - l1, _bgScreenWidth, 8);
            }

            if (!DoNotDrawLogo)
            {
                if (bgPixels == null)
                {
                    gameGraphics.drawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.drawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }

            gameGraphics.drawImage(baseLoginScreenBackgroundPic + 1, 0, 0, _bgScreenWidth, 200);
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
            gameCamera.SetCameraTransform(cameraX, -engineHandle.getAveragedElevation(cameraX, cameraY), cameraY, 912, cameraRotation, 0, cameraDistance * 2);
            gameCamera.finishCamera();
            gameGraphics.screenFadeToBlack();
            gameGraphics.screenFadeToBlack();



            gameGraphics.drawBox(0, 0, _bgScreenWidth, 6, 0);
            for (int j2 = 6; j2 >= 1; j2--)
            {
                gameGraphics.drawTransparentLine(0, j2, 0, j2, _bgScreenWidth, 8);
            }

            gameGraphics.drawBox(0, 194, _bgScreenWidth, 20, 0);
            for (int k2 = 6; k2 >= 1; k2--)
            {
                gameGraphics.drawTransparentLine(0, k2, 0, 194, _bgScreenWidth, 8);
            }

            if (!DoNotDrawLogo)
            {
                if (bgPixels == null)
                {
                    gameGraphics.drawPicture(15, 15, baseInventoryPic + 10);
                }
                else
                {
                    gameGraphics.drawPixels(bgPixels, 0, 0, bgPixels.Length, bgPixels[0].Length);
                }
            }

            gameGraphics.drawImage(baseInventoryPic + 10, 0, 0, _bgScreenWidth, 200);
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
                        int k17 = SectionX + DataOperations.getShort2(data, 1 + i11 * 4) >> 3;
                        int i22 = SectionY + DataOperations.getShort2(data, 3 + i11 * 4) >> 3;
                        int j25 = 0;
                        for (int l28 = 0; l28 < GroundItemCount; l28++)
                        {
                            int j33 = (GroundItemX[l28] >> 3) - k17;
                            int l36 = (GroundItemY[l28] >> 3) - i22;
                            if (j33 != 0 || l36 != 0)
                            {
                                if (l28 != j25)
                                {
                                    GroundItemX[j25] = GroundItemX[l28];
                                    GroundItemY[j25] = GroundItemY[l28];
                                    GroundItemId[j25] = GroundItemId[l28];
                                    GroundItemObjectVar[j25] = GroundItemObjectVar[l28];
                                }
                                j25++;
                            }
                        }

                        GroundItemCount = j25;
                        j25 = 0;
                        for (int k33 = 0; k33 < ObjectCount; k33++)
                        {
                            int i37 = (ObjectX[k33] >> 3) - k17;
                            int j39 = (ObjectY[k33] >> 3) - i22;
                            if (i37 != 0 || j39 != 0)
                            {
                                if (k33 != j25)
                                {
                                    ObjectArray[j25] = ObjectArray[k33];
                                    ObjectArray[j25].index = j25;
                                    ObjectX[j25] = ObjectX[k33];
                                    ObjectY[j25] = ObjectY[k33];
                                    ObjectType[j25] = ObjectType[k33];
                                    ObjectRotation[j25] = ObjectRotation[k33];
                                }
                                j25++;
                            }
                            else
                            {
                                gameCamera.removeModel(ObjectArray[k33]);
                                engineHandle.removeObject(ObjectX[k33], ObjectY[k33], ObjectType[k33], ObjectRotation[k33]);
                            }
                        }

                        ObjectCount = j25;
                        j25 = 0;
                        for (int j37 = 0; j37 < WallObjectCount; j37++)
                        {
                            int k39 = (WallObjectX[j37] >> 3) - k17;
                            int l41 = (WallObjectY[j37] >> 3) - i22;
                            if (k39 != 0 || l41 != 0)
                            {
                                if (j37 != j25)
                                {
                                    WallObjects[j25] = WallObjects[j37];
                                    WallObjects[j25].index = j25 + 10000;
                                    WallObjectX[j25] = WallObjectX[j37];
                                    WallObjectY[j25] = WallObjectY[j37];
                                    WallObjectDirection[j25] = WallObjectDirection[j37];
                                    WallObjectId[j25] = WallObjectId[j37];
                                }
                                j25++;
                            }
                            else
                            {
                                gameCamera.removeModel(WallObjects[j37]);
                                engineHandle.removeWallObject(WallObjectX[j37], WallObjectY[j37], WallObjectDirection[j37], WallObjectId[j37]);
                            }
                        }

                        WallObjectCount = j25;
                    }

                    return;
                }
                if (command == ServerCommand.TradeBegins)
                {
                    int tradeOther = DataOperations.GetInt16(data, 1);
                    if (Mobs[tradeOther] != null)
                    {
                        tradeOtherName = Mobs[tradeOther].username;
                    }

                    showTradeBox = true;
                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    tradeItemsOurCount = 0;
                    tradeItemsOtherCount = 0;
                    return;
                }
                if (command == ServerCommand.TradeEnds)
                {
                    showTradeBox = false;
                    showTradeConfirmBox = false;
                    return;
                }
                if (command == ServerCommand.Command250)
                {
                    tradeItemsOtherCount = data[1] & 0xff;
                    int i4 = 2;
                    for (int j11 = 0; j11 < tradeItemsOtherCount; j11++)
                    {
                        tradeItemsOther[j11] = DataOperations.GetInt16(data, i4);
                        i4 += 2;
                        tradeItemOtherCount[j11] = DataOperations.GetInt32(data, i4);
                        i4 += 4;
                    }

                    tradeOtherAccepted = false;
                    tradeWeAccepted = false;
                    return;
                }
                if (command == ServerCommand.TradeAcceptedByOther)
                {
                    sbyte byte0 = data[1];
                    if (byte0 == 1)
                    {
                        tradeOtherAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeOtherAccepted = false;
                        return;
                    }
                }
                if (command == ServerCommand.Command253)
                {
                    ShowShopBox = true;
                    int off = 1;
                    int newShopItemCount = data[off++] & 0xff;
                    sbyte isGeneralShop = data[off++];
                    shopItemSellPriceModifier = data[off++] & 0xff;
                    shopItemBuyPriceModifier = data[off++] & 0xff;
                    for (int j22 = 0; j22 < 40; j22++)
                    {
                        shopItems[j22] = -1;
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
                        for (int l33 = 0; l33 < InventoryItemsCount; l33++)
                        {
                            if (i29 < newShopItemCount)
                            {
                                break;
                            }

                            bool flag2 = false;
                            for (int l39 = 0; l39 < 40; l39++)
                            {
                                if (shopItems[l39] != InventoryItems[l33])
                                {
                                    continue;
                                }

                                flag2 = true;
                                break;
                            }

                            if (InventoryItems[l33] == 10)
                            {
                                flag2 = true;
                            }

                            if (!flag2)
                            {
                                shopItems[i29] = InventoryItems[l33] & 0x7fff;
                                shopItemCount[i29] = 0;
                                shopItemSellPrice[i29] = EntityManager.GetItem(shopItems[i29]).BasePrice - (int)(EntityManager.GetItem(shopItems[i29]).BasePrice / 2.5);
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
                if (command == ServerCommand.TradeAcceptedBySelf)
                {
                    sbyte byte1 = data[1];
                    if (byte1 == 1)
                    {
                        tradeWeAccepted = true;
                        return;
                    }
                    else
                    {
                        tradeWeAccepted = false;
                        return;
                    }
                }
                if (command == ServerCommand.Command209)
                {
                    for (int k4 = 0; k4 < length - 1; k4++)
                    {
                        bool flag = data[k4 + 1] == 1;
                        if (!prayerOn[k4] && flag)
                        {
                            playSound("prayeron");
                        }

                        if (prayerOn[k4] && !flag)
                        {
                            playSound("prayeroff");
                        }

                        prayerOn[k4] = flag;
                    }

                    return;
                }
                if (command == ServerCommand.Command93)
                {
                    ShowBankBox = true;
                    int off = 1;
                    serverBankItemsCount = data[off++] & 0xff;
                    maxBankItems = data[off++] & 0xff;
                    for (int l11 = 0; l11 < serverBankItemsCount; l11++)
                    {
                        serverBankItems[l11] = DataOperations.GetInt16(data, off);
                        off += 2;
                        serverBankItemCount[l11] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    updateBankItems();
                    return;
                }
                if (command == ServerCommand.SkillExperience)
                {
                    int j5 = data[1] & 0xff;
                    PlayerStatExperience[j5] = DataOperations.GetInt32(data, 2);
                    return;
                }
                if (command == ServerCommand.Command229)
                {
                    int k5 = DataOperations.GetInt16(data, 1);
                    if (Mobs[k5] != null)
                    {
                        duelOpponent = Mobs[k5].username;
                    }

                    ShowDuelBox = true;
                    duelMyItemCount = 0;
                    duelOpponentItemCount = 0;
                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    duelNoRetreating = false;
                    duelNoMagic = false;
                    duelNoPrayer = false;
                    duelNoWeapons = false;
                    return;
                }

#warning have not fixed the following yet....
                Console.WriteLine($"Unfixed command? {command}");

                if (command == ServerCommand.Command251)
                {
                    showTradeConfirmBox = true;
                    tradeConfirmAccepted = false;
                    showTradeBox = false;
                    int off = 1;
                    tradeConfirmOtherNameLong = DataOperations.GetLong(data, off);
                    off += 8;
                    tradeConfirmOtherItemCount = data[off++] & 0xff;
                    for (int i12 = 0; i12 < tradeConfirmOtherItemCount; i12++)
                    {
                        tradeConfirmOtherItems[i12] = DataOperations.GetInt16(data, off);
                        off += 2;
                        tradeConfirmOtherItemsCount[i12] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    tradeConfigItemCount = data[off++] & 0xff;
                    for (int l17 = 0; l17 < tradeConfigItemCount; l17++)
                    {
                        tradeConfirmItems[l17] = DataOperations.GetInt16(data, off);
                        off += 2;
                        tradeConfigItemsCount[l17] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    return;
                }
                if (command == ServerCommand.Command63)
                {
                    duelOpponentItemCount = data[1] & 0xff;
                    int off = 2;
                    for (int j12 = 0; j12 < duelOpponentItemCount; j12++)
                    {
                        duelOpponentItems[j12] = DataOperations.GetInt16(data, off);
                        off += 2;
                        duelOpponentItemsCount[j12] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (command == ServerCommand.Command198)
                {
                    if (data[1] == 1)
                    {
                        duelNoRetreating = true;
                    }
                    else
                    {
                        duelNoRetreating = false;
                    }

                    if (data[2] == 1)
                    {
                        duelNoMagic = true;
                    }
                    else
                    {
                        duelNoMagic = false;
                    }

                    if (data[3] == 1)
                    {
                        duelNoPrayer = true;
                    }
                    else
                    {
                        duelNoPrayer = false;
                    }

                    if (data[4] == 1)
                    {
                        duelNoWeapons = true;
                    }
                    else
                    {
                        duelNoWeapons = false;
                    }

                    duelOpponentAccepted = false;
                    duelMyAccepted = false;
                    return;
                }
                if (command == ServerCommand.Command139)
                {
                    int off = 1;
                    int itemSlot = data[off++] & 0xff;
                    int itemID = DataOperations.GetInt16(data, off);
                    off += 2;
                    int itemCount = DataOperations.GetInt32(data, off);
                    off += 4;
                    if (itemCount == 0)
                    {
                        serverBankItemsCount--;
                        for (int l25 = itemSlot; l25 < serverBankItemsCount; l25++)
                        {
                            serverBankItems[l25] = serverBankItems[l25 + 1];
                            serverBankItemCount[l25] = serverBankItemCount[l25 + 1];
                        }

                    }
                    else
                    {
                        serverBankItems[itemSlot] = itemID;
                        serverBankItemCount[itemSlot] = itemCount;
                        if (itemSlot >= serverBankItemsCount)
                        {
                            serverBankItemsCount = itemSlot + 1;
                        }
                    }
                    updateBankItems();
                    return;
                }
                if (command == ServerCommand.Command191)
                {
                    int l6 = data[1] & 0xff;
                    InventoryItemsCount--;
                    for (int i13 = l6; i13 < InventoryItemsCount; i13++)
                    {
                        InventoryItems[i13] = InventoryItems[i13 + 1];
                        InventoryItemCount[i13] = InventoryItemCount[i13 + 1];
                        InventoryItemEquipped[i13] = InventoryItemEquipped[i13 + 1];
                    }

                    return;
                }
                if (command == ServerCommand.DuelAcceptedByOther)
                {
                    sbyte byte2 = data[1];
                    if (byte2 == 1)
                    {
                        duelOpponentAccepted = true;
                        return;
                    }
                    else
                    {
                        duelOpponentAccepted = false;
                        return;
                    }
                }
                if (command == ServerCommand.DuelAcceptedBySelf)
                {
                    sbyte byte3 = data[1];
                    if (byte3 == 1)
                    {
                        duelMyAccepted = true;
                        return;
                    }
                    else
                    {
                        duelMyAccepted = false;
                        return;
                    }
                }
                if (command == ServerCommand.Command147)
                {
                    ShowDuelConfirmBox = true;
                    duelConfirmOurAccepted = false;
                    ShowDuelBox = false;
                    int off = 1;
                    duelOpponentHash = DataOperations.GetLong(data, off);
                    off += 8;
                    duelOpponentStakeCount = data[off++] & 0xff;
                    for (int k13 = 0; k13 < duelOpponentStakeCount; k13++)
                    {
                        duelOpponentStakeItem[k13] = DataOperations.GetInt16(data, off);
                        off += 2;
                        duelOutStakeItemCount[k13] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    duelOurStakeCount = data[off++] & 0xff;
                    for (int k18 = 0; k18 < duelOurStakeCount; k18++)
                    {
                        duelOurStakeItem[k18] = DataOperations.GetInt16(data, off);
                        off += 2;
                        duelOurStakeItemCount[k18] = DataOperations.GetInt32(data, off);
                        off += 4;
                    }

                    duelRetreat = data[off++] & 0xff;
                    duelMagic = data[off++] & 0xff;
                    duelPrayer = data[off++] & 0xff;
                    duelWeapons = data[off++] & 0xff;
                    return;
                }
                if (command == ServerCommand.PlaySound)
                {
                    string s1 = new string(data.Select(c => (char)c).ToArray(), 1, length - 1);
                    playSound(s1);
                    return;
                }
                if (command == ServerCommand.Command23)
                {
                    if (teleBubbleCount < 50)
                    {
                        int type = data[1] & 0xff;
                        int x = data[2] + SectionX;
                        int y = data[3] + SectionY;
                        teleBubbleType[teleBubbleCount] = type;
                        teleBubbleTime[teleBubbleCount] = 0;
                        teleBubbleX[teleBubbleCount] = x;
                        teleBubbleY[teleBubbleCount] = y;
                        teleBubbleCount++;
                    }
                    return;
                }
                if (command == ServerCommand.Command148)
                {
                    serverMessage = new string(data.Select(c => (char)c).ToArray(), 1, length - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = false;
                    return;
                }
                if (command == ServerCommand.Command64)
                {
                    serverMessage = new string(data.Select(c => (char)c).ToArray(), 1, length - 1);
                    showServerMessageBox = true;
                    serverMessageBoxTop = true;
                    return;
                }
                if (command == ServerCommand.Command225)
                {
                    sleepingStatusText = "Incorrect - Please wait...";
                    return;
                }
                if (command == ServerCommand.Command172)
                {
                    SystemUpdateTimer = DataOperations.GetInt16(data, 1) * 32;
                    return;
                }
                if (command == ServerCommand.Command182)
                {
                    int off = 1;
                    QuestPoints = DataOperations.GetInt16(data, off);
                    off += 2;
                    for (int l4 = 0; l4 < questName.Length; l4++)
                    {
                        questStage[l4] = data[l4 + 1];
                    }

                    return;
                }
                if (command == ServerCommand.Command233)
                {
                    QuestPoints = DataOperations.GetInt8(data[1]);
                    int count = DataOperations.GetInt8(data[2]);
                    int off = 3;
                    string[] newQuestNames = new string[count];
                    int[] newQuestStage = new int[count];
                    for (int i = 0; i < count; i++)
                    {
                        newQuestNames[i] = questName[DataOperations.GetInt8(data[off++])];
                        newQuestStage[i] = DataOperations.GetInt8(data[off++]);
                    }
                    usedQuestName = newQuestNames;
                    questStage = newQuestStage;
                    return;
                }

                Console.WriteLine("UNHANDLED PACKET:" + command + " LEN:" + length + " @#!#@#!#@#!#@#!#@#!#@");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //protected void startThread(Runnable runnable) {
        //    if(link.gameApplet != null) {
        //        link.thread(runnable);
        //        return;
        //    } else {
        //        Thread thread = new Thread(runnable);
        //        thread.setDaemon(true);
        //        thread.start();
        //        return;
        //    }
        //}

        public override void initVars()
        {
            DropPartyTimer = 0;
            PvpTournamentTimer = 0;
            SystemUpdateTimer = 0;
            WildernessModeTimer = 0;

            CombatStyle = 0;
            logoutTimer = 0;
            loginScreenNumber = 0;
            loggedIn = true;

            ResetPrivateMessages();
            gameGraphics.ClearScreen();
            // gameGraphics.UpdateGameImage();
            //gameGraphics.drawImage(spriteBatch, 0, 0);
            OnDrawDone();

            for (int i = 0; i < ObjectCount; i++)
            {
                gameCamera.removeModel(ObjectArray[i]);
                engineHandle.removeObject(ObjectX[i], ObjectY[i], ObjectType[i], ObjectRotation[i]);
            }

            for (int i = 0; i < WallObjectCount; i++)
            {
                gameCamera.removeModel(WallObjects[i]);
                engineHandle.removeWallObject(WallObjectX[i], WallObjectY[i], WallObjectDirection[i], WallObjectId[i]);
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
            friendsCount = 0;
        }

        public void drawMinimapMenu(bool canClick)
        {
            int l = gameGraphics.gameWidth - 199;
            int c1 = 156;//'æ';//(char)234;//'\u234';
            int c3 = 152;// '~';//(char)230;//'\u230';
            gameGraphics.drawPicture(l - 49, 3, baseInventoryPic + 2);
            l += 40;
            gameGraphics.drawBox(l, 36, c1, c3, 0);
            gameGraphics.setDimensions(l, 36, l + c1, 36 + c3);
            int j1 = 192 + minimapRandomRotationY;
            int l1 = cameraRotation + minimapRandomRotationX & 0xff;
            int j2 = ((CurrentPlayer.currentX - 6040) * 3 * j1) / 2048;
            int l3 = ((CurrentPlayer.currentY - 6040) * 3 * j1) / 2048;
            int j5 = Camera.bbk[1024 - l1 * 4 & 0x3ff];
            int l5 = Camera.bbk[(1024 - l1 * 4 & 0x3ff) + 1024];
            int j6 = l3 * j5 + j2 * l5 >> 18;
            l3 = l3 * l5 - j2 * j5 >> 18;
            j2 = j6;
            gameGraphics.drawMinimapPic((l + c1 / 2) - j2, 36 + c3 / 2 + l3, baseInventoryPic - 1, l1 + 64 & 0xff, j1);

            /*
            for (int l7 = 0; l7 < ObjectCount; l7++)
            {
                int k2 = (((ObjectX[l7] * GridSize + 64) - CurrentPlayer.currentX) * 3 * j1) / 2048;
                int i4 = (((ObjectY[l7] * GridSize + 64) - CurrentPlayer.currentY) * 3 * j1) / 2048;
                int k6 = i4 * j5 + k2 * l5 >> 18;
                i4 = i4 * l5 - k2 * j5 >> 18;
                k2 = k6;
                drawMinimapObject(l + c1 / 2 + k2, (36 + c3 / 2) - i4, 65535);
            }
            */

            for (int i8 = 0; i8 < GroundItemCount; i8++)
            {
                int l2 = (((GroundItemX[i8] * GridSize + 64) - CurrentPlayer.currentX) * 3 * j1) / 2048;
                int j4 = (((GroundItemY[i8] * GridSize + 64) - CurrentPlayer.currentY) * 3 * j1) / 2048;
                int l6 = j4 * j5 + l2 * l5 >> 18;
                j4 = j4 * l5 - l2 * j5 >> 18;
                l2 = l6;
                drawMinimapObject(l + c1 / 2 + l2, (36 + c3 / 2) - j4, 0xff0000);
            }

            for (int j8 = 0; j8 < NpcCount; j8++)
            {
                Mob f1 = Npcs[j8];
                int i3 = ((f1.currentX - CurrentPlayer.currentX) * 3 * j1) / 2048;
                int k4 = ((f1.currentY - CurrentPlayer.currentY) * 3 * j1) / 2048;
                int i7 = k4 * j5 + i3 * l5 >> 18;
                k4 = k4 * l5 - i3 * j5 >> 18;
                i3 = i7;
                drawMinimapObject(l + c1 / 2 + i3, (36 + c3 / 2) - k4, 0xffff00);
            }

            for (int k8 = 0; k8 < PlayerCount; k8++)
            {
                Mob f2 = Players[k8];
                int j3 = ((f2.currentX - CurrentPlayer.currentX) * 3 * j1) / 2048;
                int l4 = ((f2.currentY - CurrentPlayer.currentY) * 3 * j1) / 2048;
                int j7 = l4 * j5 + j3 * l5 >> 18;
                l4 = l4 * l5 - j3 * j5 >> 18;
                j3 = j7;
                int i9 = 0xffffff;
                for (int j9 = 0; j9 < friendsCount; j9++)
                {
                    if (f2.NameHash != friendsList[j9] || friendsWorld[j9] != 99)
                    {
                        continue;
                    }

                    i9 = 65280;
                    break;
                }

                drawMinimapObject(l + c1 / 2 + j3, (36 + c3 / 2) - l4, i9);
            }

            // compass
            gameGraphics.drawCircle(l + c1 / 2, 36 + c3 / 2, 2, 0xffffff, 255);
            gameGraphics.drawMinimapPic(l + 19, 55, baseInventoryPic + 24, cameraRotation + 128 & 0xff, 128);
            gameGraphics.setDimensions(0, 0, windowWidth, windowHeight + 12);
            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 199);
            int l8 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 40 && l8 >= 0 && l < 196 && l8 < 152)
            {
                int c2 = 156;//'\u234';
                int c4 = 152;//'\u230';
                int k1 = 192 + minimapRandomRotationY;
                int i2 = cameraRotation + minimapRandomRotationX & 0xff;
                int i1 = gameGraphics.gameWidth - 199;
                i1 += 40;
                int k3 = ((InputManager.Instance.MouseLocation.X - (i1 + c2 / 2)) * 16384) / (3 * k1);
                int i5 = ((InputManager.Instance.MouseLocation.Y - (36 + c4 / 2)) * 16384) / (3 * k1);
                int k5 = Camera.bbk[1024 - i2 * 4 & 0x3ff];
                int i6 = Camera.bbk[(1024 - i2 * 4 & 0x3ff) + 1024];
                int k7 = i5 * k5 + k3 * i6 >> 15;
                i5 = i5 * i6 - k3 * k5 >> 15;
                k3 = k7;
                k3 += CurrentPlayer.currentX;
                i5 = CurrentPlayer.currentY - i5;
                if (mouseButtonClick == 1)
                {
                    walkTo1Tile(SectionX, SectionY, k3 / 128, i5 / 128, false);
                }

                mouseButtonClick = 0;
            }
        }

        public bool validCameraAngle(int arg0)
        {
            int l = CurrentPlayer.currentX / 128;
            int i1 = CurrentPlayer.currentY / 128;
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

        public void loadSounds()
        {
            try
            {
                soundData = unpackData("sounds.mem", "Sound effects", 90);
                audioPlayer = new AudioReader();
                return;
            }
            catch (Exception throwable)
            {
                Console.WriteLine("Unable to init sounds:" + throwable);
            }
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

            GameAppletMiddleMan.maxPacketReadCount = 500;
            baseInventoryPic = 2000;
            baseScrollPic = baseInventoryPic + 100;
            baseItemPicture = baseScrollPic + 50;
            baseLoginScreenBackgroundPic = baseItemPicture + 1000;
            baseProjectilePic = baseLoginScreenBackgroundPic + 10;
            baseTexturePic = baseProjectilePic + 50;
            subTexturePic = baseTexturePic + 10;
            graphics = getGraphics();
            SetRefreshRate(50);
            gameGraphics = new GameImageMiddleMan(windowWidth, windowHeight + 12, 4000);
            gameGraphics.gameReference = this;
            gameGraphics.setDimensions(0, 0, windowWidth, windowHeight + 12);
            Menu.gdh = false;
            Menu.baseScrollPic = baseScrollPic;
            spellMenu = new Menu(gameGraphics, 5);
            int k1 = gameGraphics.gameWidth - 199;
            sbyte byte0 = 36;
            spellMenuHandle = spellMenu.createList(k1, byte0 + 24, 196, 90, 1, 500, true);
            friendsMenu = new Menu(gameGraphics, 5);
            friendsMenuHandle = friendsMenu.createList(k1, byte0 + 40, 196, 126, 1, 500, true);
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

            gameCamera.setCameraSize(windowWidth / 2, windowHeight / 2, windowWidth / 2, windowHeight / 2, windowWidth, cameraFieldOfView);
            gameCamera.zoom1 = 2400;
            gameCamera.zoom2 = 2400;
            gameCamera.zoom3 = 1;
            gameCamera.zoom4 = 2300;
            gameCamera.bjk(-50, -10, -50);
            engineHandle = new EngineHandle(gameCamera, gameGraphics);
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

            loadMap();
            if (errorLoading)
            {
                return;
            }

            loadSounds();
            if (!errorLoading)
            {
                OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Starting game...", 100));
                drawLoadingBarText(100, "Starting game...");
                createChatInputMenu();
                createLoginMenus();
                createAppearanceWindow();
                setLoginVars();

                OnContentLoadedCompleted?.Invoke(this, new EventArgs());

                createLoginScreenBackgrounds();
                return;
            }
        }

        public void createLoginMenus()
        {
            loginMenuFirst = new Menu(gameGraphics, 50);

            int l = 40;
            if (!GameDefines.PREMIUM_FEATURES)
            {
                loginMenuFirst.drawText(256, 200 + l, "Click on an option", 5, true);
                loginMenuFirst.drawButton(156, 240 + l, 120, 35);
                loginMenuFirst.drawButton(356, 240 + l, 120, 35);
                loginMenuFirst.drawText(156, 240 + l, "New User", 5, false);
                loginMenuFirst.drawText(356, 240 + l, "Existing User", 5, false);
                loginButtonNewUser = loginMenuFirst.createButton(156, 240 + l, 120, 35);
                loginMenuLoginButton = loginMenuFirst.createButton(356, 240 + l, 120, 35);
            }
            else
            {
                loginMenuFirst.drawText(256, 200 + l, "Welcome to RuneScape", 4, true);
                loginMenuFirst.drawText(256, 215 + l, "You need a member account to use this server", 4, true);
                loginMenuFirst.drawButton(256, 250 + l, 200, 35);
                loginMenuFirst.drawText(256, 250 + l, "Click here to login", 5, false);
                loginMenuLoginButton = loginMenuFirst.createButton(256, 250 + l, 200, 35);
            }
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

        public override void LostConnection()
        {
            SystemUpdateTimer = 0;

            PvpTournamentTimer = 0;
            WildernessModeTimer = 0;
            DropPartyTimer = 0;

            if (logoutTimer != 0)
            {
                resetIntVars();
                return;
            }
            else
            {
                base.LostConnection();
                return;
            }
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
            gameGraphics.unpackImageData(baseProjectilePic, DataOperations.loadData("projectile.dat", 0, media), abyte1, EntityManager.SpellProjectileCount);
            int l = EntityManager.HighestLoadedPicture;
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

            for (int l1 = 0; l1 < EntityManager.SpellProjectileCount; l1++)
            {
                gameGraphics.loadImage(baseProjectilePic + l1);
            }

            for (int i2 = 0; i2 < EntityManager.HighestLoadedPicture; i2++)
            {
                gameGraphics.loadImage(baseProjectilePic + i2);
                //var w = ((GameImage)(gameGraphics)).pictureWidth[baseProjectilePic + i2];
                //var h = ((GameImage)(gameGraphics)).pictureHeight[baseProjectilePic + i2];
                //var texture = GameImage.UnpackedImages[baseProjectilePic + i2];
                //if (texture != null)
                //    texture.SaveAsJpeg(System.IO.File.OpenWrite("c:/jpg/" + baseProjectilePic + i2 + ".jpg"), w, h);
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

                if (chatTabAllMsgFlash > 0)
                {
                    chatTabAllMsgFlash--;
                }

                if (chatTabHistoryFlash > 0)
                {
                    chatTabHistoryFlash--;
                }

                if (chatTabQuestFlash > 0)
                {
                    chatTabQuestFlash--;
                }

                if (chatTabPrivateFlash > 0)
                {
                    chatTabPrivateFlash--;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex.Message);

                cleanUp();
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
            for (int i1 = 0; i1 < EntityManager.AnimationCount; i1++)
            {
                //   label4:
                bool breakThis = false;
                string s1 = EntityManager.GetAnimation(i1).Name;
                for (int j1 = 0; j1 < i1; j1++)
                {
                    if (!EntityManager.GetAnimation(j1).Name.ToLower().Equals(s1))
                    {
                        continue;
                    }

                    EntityManager.GetAnimation(i1).Number = EntityManager.GetAnimation(j1).Number;

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
                        if (EntityManager.GetAnimation(i1).HasA == 1)
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
                        if (EntityManager.GetAnimation(i1).HasF == 1)
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
                        if (EntityManager.GetAnimation(i1).GenderModel != 0)
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
                EntityManager.GetAnimation(i1).Number = animationNumber;
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
                    appearanceHeadType = ((appearanceHeadType - 1) + EntityManager.AnimationCount) % EntityManager.AnimationCount;
                }
                while ((EntityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 ||
                       (EntityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0);
            }

            if (appearanceMenu.isClicked(appearanceHeadRightArrow))
            {
                do
                {
                    appearanceHeadType = (appearanceHeadType + 1) % EntityManager.AnimationCount;
                }
                while ((EntityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 ||
                       (EntityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0);
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
                for (appearanceHeadGender = 3 - appearanceHeadGender; (EntityManager.GetAnimation(appearanceHeadType).GenderModel & 3) != 1 || (EntityManager.GetAnimation(appearanceHeadType).GenderModel & 4 * appearanceHeadGender) == 0; appearanceHeadType = (appearanceHeadType + 1) % EntityManager.AnimationCount)
                {
                    ;
                }

                for (; (EntityManager.GetAnimation(appearanceBodyGender).GenderModel & 3) != 2 || (EntityManager.GetAnimation(appearanceBodyGender).GenderModel & 4 * appearanceHeadGender) == 0; appearanceBodyGender = (appearanceBodyGender + 1) % EntityManager.AnimationCount)
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

        public void drawWelcomeBox()
        {
            int l = 65;
            if (!LastLoginAddress.Equals("0.0.0.0"))
            {
                l += 30;
            }

            if (SubscriptionDaysLeft > 0)
            {
                l += 15;
            }

            if (LastLoginDays >= 0)
            {
                l += 15;
            }

            int i1 = 167 - l / 2;
            gameGraphics.drawBox(56, 167 - l / 2, 400, l, 0);
            gameGraphics.drawBoxEdge(56, 167 - l / 2, 400, l, 0xffffff);
            i1 += 20;
            gameGraphics.drawText("Welcome to RuneScape " + loginUsername, 256, i1, 4, 0xffff00);
            i1 += 30;
            string s1;
            // lastLoginDays    subDaysLeft    lastLoginAddress
            if (LastLoginDays == 0)
            {
                s1 = "earlier today";
            }
            else
                if (LastLoginDays == 1)
            {
                s1 = "yesterday";
            }
            else
            {
                s1 = LastLoginDays + " days ago";
            }

            if (!LastLoginAddress.Equals("0.0.0.0"))
            {
                gameGraphics.drawText("You last logged in " + s1, 256, i1, 1, 0xffffff);
                i1 += 15;
                gameGraphics.drawText("from: " + LastLoginAddress, 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            if (SubscriptionDaysLeft > 0)
            {
                gameGraphics.drawText("Subscription left: " + SubscriptionDaysLeft + " days", 256, i1, 1, 0xffffff);
                i1 += 15;
            }
            /*if(unreadMessages > 0) {
                int j1 = 0xffffff;
                gameGraphics.drawText("Jagex staff will NEVER email you. We use the", 256, i1, 1, j1);
                i1 += 15;
                gameGraphics.drawText("message-centre on this website instead.", 256, i1, 1, j1);
                i1 += 15;
                if(unreadMessages == 1)
                    gameGraphics.drawText("You have @yel@0@whi@ unread messages in your message-centre", 256, i1, 1, 0xffffff);
                else
                    gameGraphics.drawText("You have @gre@" + (unreadMessages - 1) + " unread messages @whi@in your message-centre", 256, i1, 1, 0xffffff);
                i1 += 15;
                i1 += 15;
            }
            if(lastChangedRecoveryDays != 201) {
                if(lastChangedRecoveryDays == 200) {
                    gameGraphics.drawText("You have not yet set any password recovery questions.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.drawText("We strongly recommend you do so now to secure your account.", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.drawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
                    i1 += 15;
                } else {
                    string s2;
                    if(lastChangedRecoveryDays == 0)
                        s2 = "Earlier today";
                    else
                    if(lastChangedRecoveryDays == 1)
                        s2 = "Yesterday";
                    else
                        s2 = lastChangedRecoveryDays + " days ago";
                    gameGraphics.drawText(s2 + " you changed your recovery questions", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.drawText("If you do not remember making this change then cancel it immediately", 256, i1, 1, 0xff8000);
                    i1 += 15;
                    gameGraphics.drawText("Do this from the 'account management' area on our front webpage", 256, i1, 1, 0xff8000);
                    i1 += 15;
                }
                i1 += 15;
            }*/
            int k1 = 0xffffff;
            if (InputManager.Instance.MouseLocation.Y > i1 - 12 && InputManager.Instance.MouseLocation.Y <= i1 && InputManager.Instance.MouseLocation.X > 106 && InputManager.Instance.MouseLocation.X < 406)
            {
                k1 = 0xff0000;
            }

            gameGraphics.drawText("Click here to close window", 256, i1, 1, k1);
            if (mouseButtonClick == 1)
            {
                if (k1 == 0xff0000)
                {
                    ShowWelcomeBox = false;
                }

                if ((InputManager.Instance.MouseLocation.X < 86 || InputManager.Instance.MouseLocation.X > 426) && (InputManager.Instance.MouseLocation.Y < 167 - l / 2 || InputManager.Instance.MouseLocation.Y > 167 + l / 2))
                {
                    ShowWelcomeBox = false;
                }
            }
            mouseButtonClick = 0;
        }

        public int getInventoryItemTotalCount(int arg0)
        {
            int l = 0;
            for (int i1 = 0; i1 < InventoryItemsCount; i1++)
            {
                if (InventoryItems[i1] == arg0)
                {
                    if (EntityManager.GetItem(arg0).IsStackable == 1)
                    {
                        l++;
                    }
                    else
                    {
                        l += InventoryItemCount[i1];
                    }
                }
            }

            return l;
        }

        public void sendLogout()
        {
            if (!loggedIn)
            {
                return;
            }

            if (combatTimeout > 450)
            {
                displayMessage("@cya@You can't logout during combat!", 3);
                return;
            }
            if (combatTimeout > 0)
            {
                displayMessage("@cya@You can't logout for 10 seconds after combat", 3);
                return;
            }
            else
            {
                StreamClass.CreatePacket(129);
                StreamClass.FormatPacket();
                logoutTimer = 1000;

                StreamClass.CloseStream();
                return;
            }
        }



        public bool WalkTo(int startX, int startY, int destBottomX, int destBottomY,
                           int destTopX, int destTopY, bool checkForObjects, bool walkToACommand)
        {
            int stepCount = engineHandle.generatePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, walkArrayX, walkArrayY, checkForObjects);

            if (stepCount == -1)
            {
                if (walkToACommand)
                {
                    stepCount = 1;
                    walkArrayX[0] = destBottomX;
                    walkArrayY[0] = destBottomY;
                }
                else
                {
                    return false;
                }
            }

            stepCount--;
            startX = walkArrayX[stepCount];
            startY = walkArrayY[stepCount];
            stepCount--;

            if (walkToACommand)
            {
                StreamClass.CreatePacket(246);
            }
            else
            {
                StreamClass.CreatePacket(132);
            }

            StreamClass.AddInt16(startX + AreaX);
            StreamClass.AddInt16(startY + AreaY);

            if (walkToACommand && stepCount == -1 && (startX + AreaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int currentStep = stepCount; currentStep >= 0 && currentStep > stepCount - 25; currentStep--)
            {
                StreamClass.AddInt8(walkArrayX[currentStep] - startX);
                StreamClass.AddInt8(walkArrayY[currentStep] - startY);
            }

            StreamClass.FormatPacket();

            actionPictureType = -24;
            walkMouseX = InputManager.Instance.MouseLocation.X;
            walkMouseY = InputManager.Instance.MouseLocation.Y;

            return true;
        }

        public bool walkTo2(int startX, int startY, int destBottomX, int destBottomY, int destTopX, int destTopY, bool unknownDifferent,
                bool walkToACommand)
        {
            int stepCount = engineHandle.generatePath(startX, startY, destBottomX, destBottomY, destTopX, destTopY, walkArrayX, walkArrayY, unknownDifferent);
            if (stepCount == -1)
            {
                return false;
            }

            stepCount--;
            startX = walkArrayX[stepCount];
            startY = walkArrayY[stepCount];
            stepCount--;
            if (walkToACommand)
            {
                StreamClass.CreatePacket(246);
            }
            else
            {
                StreamClass.CreatePacket(132);
            }

            StreamClass.AddInt16(startX + AreaX);
            StreamClass.AddInt16(startY + AreaY);
            if (walkToACommand && stepCount == -1 && (startX + AreaX) % 5 == 0)
            {
                stepCount = 0;
            }

            for (int i1 = stepCount; i1 >= 0 && i1 > stepCount - 25; i1--)
            {
                StreamClass.AddInt8(walkArrayX[i1] - startX);
                StreamClass.AddInt8(walkArrayY[i1] - startY);
            }

            StreamClass.FormatPacket();
            actionPictureType = -24;
            walkMouseX = InputManager.Instance.MouseLocation.X;
            walkMouseY = InputManager.Instance.MouseLocation.Y;
            return true;
        }

        public void drawOptionsMenu(bool canClick)
        {
            int l = gameGraphics.gameWidth - 199;
            int i1 = 36;
            gameGraphics.drawPicture(l - 49, 3, baseInventoryPic + 6);
            int c1 = 196;
            gameGraphics.drawBoxAlpha(l, 36, c1, 62, GameImage.RgbToInt(181, 181, 181), 160);
            gameGraphics.drawBoxAlpha(l, 98, c1, 92, GameImage.RgbToInt(201, 201, 201), 160);
            gameGraphics.drawBoxAlpha(l, 190, c1, 90, GameImage.RgbToInt(181, 181, 181), 160);
            gameGraphics.drawBoxAlpha(l, 280, c1, 40, GameImage.RgbToInt(201, 201, 201), 160);
            int j1 = l + 3;
            int l1 = i1 + 15;
            gameGraphics.drawString("Game options - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (CameraAutoAngle)
            {
                gameGraphics.drawString("Camera angle mode - @gre@Auto", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Camera angle mode - @red@Manual", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (OneMouseButton)
            {
                gameGraphics.drawString("Mouse buttons - @red@One", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Mouse buttons - @gre@Two", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (GameDefines.PREMIUM_FEATURES)
            {
                if (SoundOff)
                {
                    gameGraphics.drawString("Sound effects - @red@off", j1, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.drawString("Sound effects - @gre@on", j1, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            gameGraphics.drawString("Client assists - click to toggle", j1, l1, 1, 0);
            l1 += 15;
            if (ShowRoofs)
            {
                gameGraphics.drawString("Roofs - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Roofs - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (ShowCombatWindow)
            {
                gameGraphics.drawString("Fight mode window - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Fight mode window - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (fogOfWar)
            {
                gameGraphics.drawString("Fog of war - @gre@show", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Fog of war - @red@hide", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (AutoScreenshot)
            {
                gameGraphics.drawString("Automatic screenshots - @gre@on", j1, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Automatic screenshots - @red@off", j1, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (useChatFilter)
            {
                gameGraphics.drawString("Chat filter: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Chat filter: @red@<off>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            gameGraphics.drawString("Privacy settings. Will be applied to", j1, l1, 1, 0);
            l1 += 15;
            gameGraphics.drawString("all people not on your friends list", j1, l1, 1, 0);
            l1 += 15;
            if (blockChat == 0)
            {
                gameGraphics.drawString("Block chat messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Block chat messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (blockPrivate == 0)
            {
                gameGraphics.drawString("Block public messages: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Block public messages: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (blockTrade == 0)
            {
                gameGraphics.drawString("Block trade requests: @red@<off>", l + 3, l1, 1, 0xffffff);
            }
            else
            {
                gameGraphics.drawString("Block trade requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
            }

            l1 += 15;
            if (GameDefines.PREMIUM_FEATURES)
            {
                if (blockDuel == 0)
                {
                    gameGraphics.drawString("Block duel requests: @red@<off>", l + 3, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.drawString("Block duel requests: @gre@<on>", l + 3, l1, 1, 0xffffff);
                }
            }

            l1 += 15;
            l1 += 5;
            gameGraphics.drawString("Always logout when you finish", j1, l1, 1, 0);
            l1 += 15;
            int j2 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > j1 && InputManager.Instance.MouseLocation.X < j1 + c1 && InputManager.Instance.MouseLocation.Y > l1 - 12 && InputManager.Instance.MouseLocation.Y < l1 + 4)
            {
                j2 = 0xffff00;
            }

            gameGraphics.drawString("Click here to logout", l + 3, l1, 1, j2);
            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 199);
            i1 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 280)
            {
                int k2 = gameGraphics.gameWidth - 199;
                sbyte byte0 = 36;
                int c2 = 196;
                int k1 = k2 + 3;
                int i2 = byte0 + 30;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    CameraAutoAngle = !CameraAutoAngle;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(0);
                    StreamClass.AddInt8(CameraAutoAngle ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    OneMouseButton = !OneMouseButton;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(2);
                    StreamClass.AddInt8(OneMouseButton ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                i2 += 15;
                if (GameDefines.PREMIUM_FEATURES && InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    SoundOff = !SoundOff;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(3);
                    StreamClass.AddInt8(SoundOff ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                i2 += 15;
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    ShowRoofs = !ShowRoofs;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(4);
                    StreamClass.AddInt8(ShowRoofs ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    ShowCombatWindow = !ShowCombatWindow;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(6);
                    StreamClass.AddInt8(ShowCombatWindow ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    fogOfWar = !fogOfWar;
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    AutoScreenshot = !AutoScreenshot;
                    StreamClass.CreatePacket(157);
                    StreamClass.AddInt8(5);
                    StreamClass.AddInt8(AutoScreenshot ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                bool flag = false;
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    useChatFilter = !useChatFilter;
                }
                i2 += 15;
                i2 += 15;
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    blockChat = 1 - blockChat;
                    flag = true;
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    blockPrivate = 1 - blockPrivate;
                    flag = true;
                }
                i2 += 15;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    blockTrade = 1 - blockTrade;
                    flag = true;
                }
                i2 += 15;
                if (GameDefines.PREMIUM_FEATURES && InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    blockDuel = 1 - blockDuel;
                    flag = true;
                }
                i2 += 15;
                if (flag)
                {
                    sendUpdatedPrivacyInfo(blockChat, blockPrivate, blockTrade, blockDuel);
                }

                i2 += 20;
                if (InputManager.Instance.MouseLocation.X > k1 && InputManager.Instance.MouseLocation.X < k1 + c2 && InputManager.Instance.MouseLocation.Y > i2 - 12 && InputManager.Instance.MouseLocation.Y < i2 + 4 && mouseButtonClick == 1)
                {
                    sendLogout();
                }

                mouseButtonClick = 0;
            }
        }

        public void walkToObject(int arg0, int arg1, int arg2, int arg3)
        {
            int l;
            int i1;

            if (arg2 == 0 || arg2 == 4)
            {
                l = EntityManager.GetWorldObject(arg3).Width;
                i1 = EntityManager.GetWorldObject(arg3).Height;
            }
            else
            {
                i1 = EntityManager.GetWorldObject(arg3).Width;
                l = EntityManager.GetWorldObject(arg3).Height;
            }

            if (EntityManager.GetWorldObject(arg3).Type == 2 || EntityManager.GetWorldObject(arg3).Type == 3)
            {
                if (arg2 == 0)
                {
                    arg0--;
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
                    arg1--;
                    i1++;
                }

                WalkTo(SectionX, SectionY, arg0, arg1, (arg0 + l) - 1, (arg1 + i1) - 1, false, true);
                return;
            }
            else
            {
                WalkTo(SectionX, SectionY, arg0, arg1, (arg0 + l) - 1, (arg1 + i1) - 1, true, true);
                return;
            }
        }

        public void createChatInputMenu()
        {
            chatInputMenu = new Menu(gameGraphics, 10);
            messagesHandleType2 = chatInputMenu.gfh(5, 269, 502, 56, 1, 20, true);
            chatInputBox = chatInputMenu.gfi(7, 324, 498, 14, 1, 80, false, true);
            messagesHandleType5 = chatInputMenu.gfh(5, 269, 502, 56, 1, 20, true);
            messagesHandleType6 = chatInputMenu.gfh(5, 269, 502, 56, 1, 20, true);
            chatInputMenu.setFocus(chatInputBox);
        }

        public void drawCombatStyleBox()
        {
            sbyte byte0 = 7;
            sbyte byte1 = 15;
            int c1 = 175; ;//'\u257';

            if (mouseButtonClick != 0)
            {
                for (int l = 0; l < 5; l++)
                {
                    if (l <= 0 || InputManager.Instance.MouseLocation.X <= byte0 || InputManager.Instance.MouseLocation.X >= byte0 + c1 || InputManager.Instance.MouseLocation.Y <= byte1 + l * 20 || InputManager.Instance.MouseLocation.Y >= byte1 + l * 20 + 20)
                    {
                        continue;
                    }

                    CombatStyle = l - 1;
                    mouseButtonClick = 0;
                    StreamClass.CreatePacket(42);
                    StreamClass.AddInt8(CombatStyle);
                    StreamClass.FormatPacket();
                    break;
                }
            }

            for (int i1 = 0; i1 < 5; i1++)
            {
                if (i1 == CombatStyle + 1)
                {
                    gameGraphics.drawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(255, 0, 0), 128);
                }
                else
                {
                    gameGraphics.drawBoxAlpha(byte0, byte1 + i1 * 20, c1, 20, GameImage.RgbToInt(190, 190, 190), 128);
                }

                gameGraphics.drawLineX(byte0, byte1 + i1 * 20, c1, 0);
                gameGraphics.drawLineX(byte0, byte1 + i1 * 20 + 20, c1, 0);
            }

            gameGraphics.drawText("Select combat style", byte0 + c1 / 2, byte1 + 16, 3, 0xffffff);
            gameGraphics.drawText("Controlled (+1 of each)", byte0 + c1 / 2, byte1 + 36, 3, 0);
            gameGraphics.drawText("Aggressive (+3 strength)", byte0 + c1 / 2, byte1 + 56, 3, 0);
            gameGraphics.drawText("Accurate   (+3 attack)", byte0 + c1 / 2, byte1 + 76, 3, 0);
            gameGraphics.drawText("Defensive  (+3 defense)", byte0 + c1 / 2, byte1 + 96, 3, 0);
        }

        public void drawTradeBox()
        {
            if (mouseButtonClick != 0)
            {
                int mx = InputManager.Instance.MouseLocation.X - 22;
                int my = InputManager.Instance.MouseLocation.Y - 36;
                if (mx >= 0 && my >= 30 && mx < 462 && my < 262)
                {
                    if (mx > 216 && my > 30 && mx < 462 && my < 235)
                    {
                        int curItem = (mx - 217) / 49 + ((my - 31) / 34) * 5;
                        if (curItem >= 0 && curItem < InventoryItemsCount)
                        {
                            int item = InventoryItems[curItem];
                            mouseClickedHeldInTradeDuelBox = 1;
                            bool ourTradeItemsChanged = false;
                            int someInt = 0;
                            for (int tradeItem = 0; tradeItem < tradeItemsOurCount; tradeItem++)
                            {
                                if (tradeItemsOur[tradeItem] == item)
                                {
                                    if (EntityManager.GetItem(item).IsStackable == 0)
                                    {
                                        for (int i = 0; i < mouseClickedHeldInTradeDuelBox; i++)
                                        {
                                            if (tradeItemOurCount[tradeItem] < InventoryItemCount[curItem])
                                            {
                                                tradeItemOurCount[tradeItem]++;
                                            }

                                            ourTradeItemsChanged = true;
                                        }
                                    }
                                    else
                                    {
                                        someInt++;
                                    }
                                }
                            }

                            if (getInventoryItemTotalCount(item) <= someInt)
                            {
                                ourTradeItemsChanged = true;
                            }

                            if (EntityManager.GetItem(item).IsSpecial == 1)
                            {
                                displayMessage("This object cannot be traded with other players", 3);
                                ourTradeItemsChanged = true;
                            }

                            if (!ourTradeItemsChanged && tradeItemsOurCount < 12)
                            {
                                tradeItemsOur[tradeItemsOurCount] = item;
                                tradeItemOurCount[tradeItemsOurCount] = 1;
                                tradeItemsOurCount++;
                                ourTradeItemsChanged = true;
                            }

                            if (ourTradeItemsChanged)
                            {
                                StreamClass.CreatePacket(70);
                                StreamClass.AddInt8(tradeItemsOurCount);
                                for (int i = 0; i < tradeItemsOurCount; i++)
                                {
                                    StreamClass.AddInt16(tradeItemsOur[i]);
                                    StreamClass.AddInt32(tradeItemOurCount[i]);
                                }
                                StreamClass.FormatPacket();
                                tradeOtherAccepted = false;
                                tradeWeAccepted = false;
                            }
                        }
                    }
                    else if (mx > 8 && my > 30 && mx < 205 && my < 133)
                    {
                        int curItem = (mx - 9) / 49 + ((my - 31) / 34) * 4;
                        if (curItem >= 0 && curItem < tradeItemsOurCount)
                        {
                            int item = tradeItemsOur[curItem];
                            for (int i = 0; i < mouseClickedHeldInTradeDuelBox; i++)
                            {
                                if (EntityManager.GetItem(item).IsStackable == 0 && tradeItemOurCount[curItem] > 1)
                                {
                                    tradeItemOurCount[curItem]--;
                                    continue;
                                }
                                tradeItemsOurCount--;
                                mouseButtonHeldTime = 0;
                                for (int j = curItem; j < tradeItemsOurCount; j++)
                                {
                                    tradeItemsOur[j] = tradeItemsOur[j + 1];
                                    tradeItemOurCount[j] = tradeItemOurCount[j + 1];
                                }
                                break;
                            }
                            StreamClass.CreatePacket(70);
                            StreamClass.AddInt8(tradeItemsOurCount);
                            for (int i = 0; i < tradeItemsOurCount; i++)
                            {
                                StreamClass.AddInt16(tradeItemsOur[i]);
                                StreamClass.AddInt32(tradeItemOurCount[i]);
                            }
                            StreamClass.FormatPacket();
                            tradeOtherAccepted = false;
                            tradeWeAccepted = false;
                        }
                    }
                    if (mx >= 217 && my >= 238 && mx <= 286 && my <= 259)
                    {
                        tradeWeAccepted = true;
                        StreamClass.CreatePacket(211);
                        StreamClass.FormatPacket();
                    }
                    if (mx >= 394 && my >= 238 && mx < 463 && my < 259)
                    {
                        showTradeBox = false;
                        StreamClass.CreatePacket(216);
                        StreamClass.FormatPacket();
                    }
                }
                else
                {
                    //showTradeBox = false;
                    //base.streamClass.createPacket(216);
                    //base.streamClass.formatPacket();
                }
                mouseButtonClick = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!showTradeBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.drawBox(byte0, byte1, 468, 12, 192);
            int l1 = 0x989898;
            gameGraphics.drawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            gameGraphics.drawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 133, 197, 22, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 30, 197, 103, j2, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 155, 197, 103, j2, 160);
            gameGraphics.drawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 4; i3++)
            {
                gameGraphics.drawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 4; i4++)
            {
                gameGraphics.drawLineX(byte0 + 8, byte1 + 155 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4++)
            {
                gameGraphics.drawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5++)
            {
                if (j5 < 5)
                {
                    gameGraphics.drawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 103, 0);
                }

                if (j5 < 5)
                {
                    gameGraphics.drawLineY(byte0 + 8 + j5 * 49, byte1 + 155, 103, 0);
                }

                gameGraphics.drawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            gameGraphics.drawString("Trading with: " + tradeOtherName, byte0 + 1, byte1 + 10, 1, 0xffffff);
            gameGraphics.drawString("Your Offer", byte0 + 9, byte1 + 27, 4, 0xffffff);
            gameGraphics.drawString("Opponent's Offer", byte0 + 9, byte1 + 152, 4, 0xffffff);
            gameGraphics.drawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            if (!tradeWeAccepted)
            {
                gameGraphics.drawPicture(byte0 + 217, byte1 + 238, baseInventoryPic + 25);
            }

            gameGraphics.drawPicture(byte0 + 394, byte1 + 238, baseInventoryPic + 26);
            if (tradeOtherAccepted)
            {
                gameGraphics.drawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                gameGraphics.drawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (tradeWeAccepted)
            {
                gameGraphics.drawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                gameGraphics.drawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < InventoryItemsCount; k5++)
            {
                int l5 = 217 + byte0 + (k5 % 5) * 49;
                int j6 = 31 + byte1 + (k5 / 5) * 34;
                gameGraphics.drawImage(l5, j6, 48, 32, baseItemPicture + EntityManager.GetItem(InventoryItems[k5]).InventoryPicture, EntityManager.GetItem(InventoryItems[k5]).PictureMask, 0, 0, false);

                if (EntityManager.GetItem(InventoryItems[k5]).IsStackable == 0)
                {
                    gameGraphics.drawString(InventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < tradeItemsOurCount; i6++)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.drawImage(k6, i7, 48, 32, baseItemPicture + EntityManager.GetItem(tradeItemsOur[i6]).InventoryPicture, EntityManager.GetItem(tradeItemsOur[i6]).PictureMask, 0, 0, false);
                if (EntityManager.GetItem(tradeItemsOur[i6]).IsStackable == 0)
                {
                    gameGraphics.drawString(tradeItemOurCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (InputManager.Instance.MouseLocation.X > k6 && InputManager.Instance.MouseLocation.X < k6 + 48 && InputManager.Instance.MouseLocation.Y > i7 && InputManager.Instance.MouseLocation.Y < i7 + 32)
                {
                    gameGraphics.drawString(EntityManager.GetItem(tradeItemsOur[i6]).Name + ": @whi@" + EntityManager.GetItem(tradeItemsOur[i6]).Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < tradeItemsOtherCount; l6++)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 156 + byte1 + (l6 / 4) * 34;
                gameGraphics.drawImage(j7, k7, 48, 32, baseItemPicture + EntityManager.GetItem(tradeItemsOther[l6]).InventoryPicture, EntityManager.GetItem(tradeItemsOther[l6]).PictureMask, 0, 0, false);
                if (EntityManager.GetItem(tradeItemsOther[l6]).IsStackable == 0)
                {
                    gameGraphics.drawString(tradeItemOtherCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (InputManager.Instance.MouseLocation.X > j7 && InputManager.Instance.MouseLocation.X < j7 + 48 && InputManager.Instance.MouseLocation.Y > k7 && InputManager.Instance.MouseLocation.Y < k7 + 32)
                {
                    gameGraphics.drawString(EntityManager.GetItem(tradeItemsOther[l6]).Name + ": @whi@" + EntityManager.GetItem(tradeItemsOther[l6]).Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
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
                    return;
                }
                if (validCameraAngle(cameraAutoAngle + 7 & 7))
                {
                    cameraAutoAngle = cameraAutoAngle + 7 & 7;
                }

                return;
            }
            else
            {
                return;
            }
        }

        //public string getParameter(string s1) {
        //    if(link.gameApplet != null)
        //        return link.gameApplet.getParameter(s1);
        //    else
        //        return base.getParameter(s1);
        //}

        public void drawLogoutBox()
        {
            gameGraphics.drawBox(126, 137, 260, 60, 0);
            gameGraphics.drawBoxEdge(126, 137, 260, 60, 0xffffff);
            gameGraphics.drawText("Logging out...", 256, 173, 5, 0xffffff);
        }

        public void walkToGroundItem(int l, int i1, int j1, int k1, bool flag)
        {
            if (walkTo2(l, i1, j1, k1, j1, k1, false, flag))
            {
                return;
            }
            else
            {
                WalkTo(l, i1, j1, k1, j1, k1, true, flag);
                return;
            }
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

        public void DrawTeleBubble(int x, int y, int j1, int k1, int l1, int i2, int j2)
        {
            int type = teleBubbleType[l1];
            int time = teleBubbleTime[l1];
            if (type == 0)
            {
                int i3 = 255 + time * 5 * 256;
                gameGraphics.drawCircle(x + j1 / 2, y + k1 / 2, 20 + time * 2, i3, 255 - time * 5);
            }
            if (type == 1)
            {
                int j3 = 0xff0000 + time * 5 * 256;
                gameGraphics.drawCircle(x + j1 / 2, y + k1 / 2, 10 + time, j3, 255 - time * 5);
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
            else
                if (loginScreenNumber == 1)
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
            else
                    if (loginScreenNumber == 2)
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

        public bool isItemEquipped(int arg0)
        {
            for (int l = 0; l < InventoryItemsCount; l++)
            {
                if (InventoryItems[l] == arg0 && InventoryItemEquipped[l] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public override void DrawWindow()
        {

            paint(graphics);

            if (errorLoading)
            {
#warning add error loading event
                //var g1 = spriteBatch;//getGraphics();
                ////g1.setColor();
                //// g1.fillRect(0, 0, 512, 356, Color.Black);

                //// g1.setFont(gameFont16);
                //g1.setColor(Color.Yellow);
                //int l = 35;
                //g1.drawString("Sorry, an error has occured whilst loading", 30, l);
                //l += 50;
                //g1.setColor(Color.White);
                //g1.drawString("To fix this try the following (in order):", 30, l);
                //l += 50;
                //g1.setColor(Color.White);
                ////g1.setFont(gameFont12);
                //g1.drawString("1: Try closing ALL open web-browser windows, and reloading", 30, l);
                //l += 30;
                //g1.drawString("2: Try clearing your web-browsers cache from tools->internet options", 30, l);
                //l += 30;
                //g1.drawString("3: Try using a different game-world", 30, l);
                //l += 30;
                //g1.drawString("4: Try rebooting your computer", 30, l);
                //l += 30;
                //g1.drawString("5: Try selecting a different version of Java from the play-game menu", 30, l);
                SetRefreshRate(1);
                return;
            }
            if (memoryError)
            {
#warning add memory exception event
                //var g3 = spriteBatch;//getGraphics();
                ////g3.setColor(Color.Black);
                ////g3.fillRect(0, 0, 512, 356, Color.Black);
                ////g3.setFont(gameFont20);
                //g3.setColor(Color.White);
                //g3.drawString("Error - out of memory!", 50, 50);
                //g3.drawString("Close ALL unnecessary programs", 50, 100);
                //g3.drawString("and windows before loading the game", 50, 150);
                //g3.drawString("this game needs about 48meg of spare RAM", 50, 200);
                //setRefreshRate(1);
                return;
            }
            try
            {
                if (loggedIn)
                {
                    gameGraphics.loggedIn = true;
                    drawGame();

                    return;
                }
                else
                {
                    gameGraphics.loggedIn = false;
                    drawLoginScreens();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex);

                cleanUp();
                memoryError = true;
            }
        }


        public void cleanUp()
        {
            try
            {
                if (gameGraphics != null)
                {
                    gameGraphics.cleanUp();
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
                //System.gc();
                System.GC.Collect();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex.Message);

                return;
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

                gameGraphics.drawString(questionMenuAnswer[i1], 6, 12 + i1 * 12, 1, j1);
            }

        }

        public void drawTradeConfirmBox()
        {
            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.drawBox(byte0, byte1, 468, 16, 192);
            int l = 0x989898;
            gameGraphics.drawBoxAlpha(byte0, byte1 + 16, 468, 246, l, 160);
            gameGraphics.drawText("Please confirm your trade with @yel@" + DataOperations.LongToString(tradeConfirmOtherNameLong), byte0 + 234, byte1 + 12, 1, 0xffffff);
            gameGraphics.drawText("You are about to give:", byte0 + 117, byte1 + 30, 1, 0xffff00);
            for (int i1 = 0; i1 < tradeConfigItemCount; i1++)
            {
                string s1 = EntityManager.GetItem(tradeConfirmItems[i1]).Name;
                if (EntityManager.GetItem(tradeConfirmItems[i1]).IsStackable == 0)
                {
                    s1 = s1 + " x " + formatItemCount(tradeConfigItemsCount[i1]);
                }

                gameGraphics.drawText(s1, byte0 + 117, byte1 + 42 + i1 * 12, 1, 0xffffff);
            }

            if (tradeConfigItemCount == 0)
            {
                gameGraphics.drawText("Nothing!", byte0 + 117, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.drawText("In return you will receive:", byte0 + 351, byte1 + 30, 1, 0xffff00);
            for (int j1 = 0; j1 < tradeConfirmOtherItemCount; j1++)
            {
                string s2 = EntityManager.GetItem(tradeConfirmOtherItems[j1]).Name;

                if (EntityManager.GetItem(tradeConfirmOtherItems[j1]).IsStackable == 0)
                {
                    s2 = s2 + " x " + formatItemCount(tradeConfirmOtherItemsCount[j1]);
                }

                gameGraphics.drawText(s2, byte0 + 351, byte1 + 42 + j1 * 12, 1, 0xffffff);
            }

            if (tradeConfirmOtherItemCount == 0)
            {
                gameGraphics.drawText("Nothing!", byte0 + 351, byte1 + 42, 1, 0xffffff);
            }

            gameGraphics.drawText("Are you sure you want to do this?", byte0 + 234, byte1 + 200, 4, 65535);
            gameGraphics.drawText("There is NO WAY to reverse a trade if you change your mind.", byte0 + 234, byte1 + 215, 1, 0xffffff);
            gameGraphics.drawText("Remember that not all players are trustworthy", byte0 + 234, byte1 + 230, 1, 0xffffff);

            if (!tradeConfirmAccepted)
            {
                gameGraphics.drawPicture((byte0 + 118) - 35, byte1 + 238, baseInventoryPic + 25);
                gameGraphics.drawPicture((byte0 + 352) - 35, byte1 + 238, baseInventoryPic + 26);
            }
            else
            {
                gameGraphics.drawText("Waiting for other player...", byte0 + 234, byte1 + 250, 1, 0xffff00);
            }
            if (mouseButtonClick == 1)
            {
                if (InputManager.Instance.MouseLocation.X < byte0 || InputManager.Instance.MouseLocation.Y < byte1 || InputManager.Instance.MouseLocation.X > byte0 + 468 || InputManager.Instance.MouseLocation.Y > byte1 + 262)
                {
                    //showTradeConfirmBox = false;
                    //base.streamClass.createPacket(216);
                    //base.streamClass.formatPacket();
                }
                if (InputManager.Instance.MouseLocation.X >= (byte0 + 118) - 35 && InputManager.Instance.MouseLocation.X <= byte0 + 118 + 70 && InputManager.Instance.MouseLocation.Y >= byte1 + 238 && InputManager.Instance.MouseLocation.Y <= byte1 + 238 + 21)
                {
                    tradeConfirmAccepted = true;
                    StreamClass.CreatePacket(53);
                    StreamClass.FormatPacket();
                }
                if (InputManager.Instance.MouseLocation.X >= (byte0 + 352) - 35 && InputManager.Instance.MouseLocation.X <= byte0 + 353 + 70 && InputManager.Instance.MouseLocation.Y >= byte1 + 238 && InputManager.Instance.MouseLocation.Y <= byte1 + 238 + 21)
                {
                    showTradeConfirmBox = false;
                    StreamClass.CreatePacket(216);
                    StreamClass.FormatPacket();
                }
                mouseButtonClick = 0;
            }
        }

        public virtual void drawLoginScreens()
        {
            LoginScreenShown = false;
            if (gameGraphics == null)
            {
                return;
            }

            gameGraphics.interlacingEnabled = false;
            gameGraphics.ClearScreen();
            if (loginScreenNumber == 0 || loginScreenNumber == 1 || loginScreenNumber == 2 || loginScreenNumber == 3)
            {
                int l = (tick * 2) % 3072;
                if (l < 1024)
                {
                    gameGraphics.drawPicture(0, 10, baseLoginScreenBackgroundPic);
                    if (l > 768)
                    {
                        gameGraphics.drawPicture(0, 10, baseLoginScreenBackgroundPic + 1, l - 768);
                    }
                }
                else if (l < 2048)
                {
                    gameGraphics.drawPicture(0, 10, baseLoginScreenBackgroundPic + 1);
                    if (l > 1792)
                    {
                        gameGraphics.drawPicture(0, 10, baseInventoryPic + 10, l - 1792);
                    }
                }
                else
                {
                    gameGraphics.drawPicture(0, 10, baseInventoryPic + 10);
                    if (l > 2816)
                    {
                        gameGraphics.drawPicture(0, 10, baseLoginScreenBackgroundPic, l - 2816);
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

            gameGraphics.drawPicture(0, windowHeight, baseInventoryPic + 22);



            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.drawImage(spriteBatch, 0, 0);
        }

        public void DrawItem(int x, int y, int width, int height, int itemID, int i2, int j2)
        {
            int picture = EntityManager.GetItem(itemID).InventoryPicture + baseItemPicture;
            int mask = EntityManager.GetItem(itemID).PictureMask;
            gameGraphics.drawImage(x, y, width, height, picture, mask, 0, 0, false);
        }

        public Mob MakePlayer(int serverIndex, int x, int y, int sprite)
        {
            if (Mobs[serverIndex] == null)
            {
                Mobs[serverIndex] = new Mob();
                Mobs[serverIndex].ServerIndex = serverIndex;
                Mobs[serverIndex].ServerId = 0;
            }

            Mob existingPlayer = Mobs[serverIndex];

            bool flag = LastPlayers
                .Where(player => player != null) // TODO: Remove this check once it is safe
                .Any(player => player.ServerIndex == serverIndex);

            if (flag)
            {
                existingPlayer.nextSprite = sprite;

                int i1 = existingPlayer.WaypointCurrent;
                if (x != existingPlayer.WaypointsX[i1] || y != existingPlayer.WaypointsY[i1])
                {
                    existingPlayer.WaypointCurrent = i1 = (i1 + 1) % 10;
                    existingPlayer.WaypointsX[i1] = x;
                    existingPlayer.WaypointsY[i1] = y;
                }
            }
            else
            {
                existingPlayer.ServerIndex = serverIndex;
                existingPlayer.WaypointsEndSprite = 0;
                existingPlayer.WaypointCurrent = 0;
                existingPlayer.WaypointsX[0] = existingPlayer.currentX = x;
                existingPlayer.WaypointsY[0] = existingPlayer.currentY = y;
                existingPlayer.nextSprite = existingPlayer.currentSprite = sprite;
                existingPlayer.stepCount = 0;
            }

            Players[PlayerCount++] = existingPlayer;

            return existingPlayer;
        }

        public void walkTo1Tile(int l, int i1, int j1, int k1, bool flag)
        {
            WalkTo(l, i1, j1, k1, j1, k1, false, flag);
        }

        public void LoadConfig()
        {
            EntityManager.Load();

            sbyte[] abyte1 = unpackData("filter.jag", "Chat system", 15);
            if (abyte1 == null)
            {
                errorLoading = true;
                return;
            }
            else
            {
                sbyte[] abyte2 = DataOperations.loadData("fragmentsenc.txt", 0, abyte1);
                sbyte[] abyte3 = DataOperations.loadData("badenc.txt", 0, abyte1);
                sbyte[] abyte4 = DataOperations.loadData("hostenc.txt", 0, abyte1);
                sbyte[] abyte5 = DataOperations.loadData("tldlist.txt", 0, abyte1);
                //ChatFilter.addFilterData(new DataEncryption(abyte2), new DataEncryption(abyte3), new DataEncryption(abyte4), new DataEncryption(abyte5));

                return;
            }
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

                        if (j1 == l - 1 && flag && combatTimeout == 0 && logoutTimer == 0)
                        {
                            sendLogout();
                            return;
                        }
                    }

                }
            }

        }

        public void drawFriendsMenu(bool canClick)
        {
            int l = gameGraphics.gameWidth - 199;
            int i1 = 36;
            gameGraphics.drawPicture(l - 49, 3, baseInventoryPic + 5);
            int c1 = 196;//(char)304;//'\u304';
            int c2 = 182;//(char)266;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (friendsIgnoreMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.drawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.drawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.drawLineX(l, i1 + 24, c1, 0);
            gameGraphics.drawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.drawLineX(l, (i1 + c2) - 16, c1, 0);
            gameGraphics.drawText("Friends", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.drawText("Ignore", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            friendsMenu.clearList(friendsMenuHandle);
            if (friendsIgnoreMenuSelected == 0)
            {
                for (int l1 = 0; l1 < friendsCount; l1++)
                {
                    string s1;
                    if (friendsWorld[l1] == 99)
                    {
                        s1 = "@gre@";
                    }
                    else
                        if (friendsWorld[l1] > 0)
                    {
                        s1 = "@yel@";
                    }
                    else
                    {
                        s1 = "@red@";
                    }

                    friendsMenu.addListItem(friendsMenuHandle, l1, s1 + DataOperations.LongToString(friendsList[l1]) + "~439~@whi@Remove         WWWWWWWWWW");
                }

            }
            if (friendsIgnoreMenuSelected == 1)
            {
                for (int i2 = 0; i2 < ignoresCount; i2++)
                {
                    friendsMenu.addListItem(friendsMenuHandle, i2, "@yel@" + DataOperations.LongToString(ignoresList[i2]) + "~439~@whi@Remove         WWWWWWWWWW");
                }
            }
            friendsMenu.drawMenu();
            if (friendsIgnoreMenuSelected == 0)
            {
                int j2 = friendsMenu.getEntryHighlighted(friendsMenuHandle);
                if (j2 >= 0 && InputManager.Instance.MouseLocation.X < 489)
                {
                    if (InputManager.Instance.MouseLocation.X > 429)
                    {
                        gameGraphics.drawText("Click to remove " + DataOperations.LongToString(friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                        if (friendsWorld[j2] == 99)
                    {
                        gameGraphics.drawText("Click to message " + DataOperations.LongToString(friendsList[j2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                            if (friendsWorld[j2] > 0)
                    {
                        gameGraphics.drawText(DataOperations.LongToString(friendsList[j2]) + " is on world " + friendsWorld[j2], l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                    else
                    {
                        gameGraphics.drawText(DataOperations.LongToString(friendsList[j2]) + " is offline", l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.drawText("Click a name to send a message", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int j3;
                if (InputManager.Instance.MouseLocation.X > l && InputManager.Instance.MouseLocation.X < l + c1 && InputManager.Instance.MouseLocation.Y > (i1 + c2) - 16 && InputManager.Instance.MouseLocation.Y < i1 + c2)
                {
                    j3 = 0xffff00;
                }
                else
                {
                    j3 = 0xffffff;
                }

                gameGraphics.drawText("Click here to add a friend", l + c1 / 2, (i1 + c2) - 3, 1, j3);
            }
            if (friendsIgnoreMenuSelected == 1)
            {
                int k2 = friendsMenu.getEntryHighlighted(friendsMenuHandle);
                if (k2 >= 0 && InputManager.Instance.MouseLocation.X < 489 && InputManager.Instance.MouseLocation.X > 429)
                {
                    if (InputManager.Instance.MouseLocation.X > 429)
                    {
                        gameGraphics.drawText("Click to remove " + DataOperations.LongToString(ignoresList[k2]), l + c1 / 2, i1 + 35, 1, 0xffffff);
                    }
                }
                else
                {
                    gameGraphics.drawText("Blocking messages from:", l + c1 / 2, i1 + 35, 1, 0xffffff);
                }
                int k3;
                if (InputManager.Instance.MouseLocation.X > l && InputManager.Instance.MouseLocation.X < l + c1 && InputManager.Instance.MouseLocation.Y > (i1 + c2) - 16 && InputManager.Instance.MouseLocation.Y < i1 + c2)
                {
                    k3 = 0xffff00;
                }
                else
                {
                    k3 = 0xffffff;
                }

                gameGraphics.drawText("Click here to add a name", l + c1 / 2, (i1 + c2) - 3, 1, k3);
            }
            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 199);
            i1 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                friendsMenu.mouseClick(l + (gameGraphics.gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98 && friendsIgnoreMenuSelected == 1)
                    {
                        friendsIgnoreMenuSelected = 0;
                        friendsMenu.switchList(friendsMenuHandle);
                    }
                    else
                        if (l > 98 && friendsIgnoreMenuSelected == 0)
                    {
                        friendsIgnoreMenuSelected = 1;
                        friendsMenu.switchList(friendsMenuHandle);
                    }
                }

                if (mouseButtonClick == 1 && friendsIgnoreMenuSelected == 0)
                {
                    int l2 = friendsMenu.getEntryHighlighted(friendsMenuHandle);

                    if (l2 >= 0 && InputManager.Instance.MouseLocation.X < 489)
                    {
                        if (InputManager.Instance.MouseLocation.X > 429)
                        {
                            removeFriend(friendsList[l2]);
                        }
                        else if (friendsWorld[l2] != 0)
                        {
                            showFriendsBox = 2;
                            pmTarget = friendsList[l2];
                            privateMessageText = "";
                            enteredPrivateMessageText = "";
                        }
                    }
                }
                if (mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    int i3 = friendsMenu.getEntryHighlighted(friendsMenuHandle);

                    if (i3 >= 0 && InputManager.Instance.MouseLocation.X < 489 && InputManager.Instance.MouseLocation.X > 429)
                    {
                        removeIgnore(ignoresList[i3]);
                    }
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 0)
                {
                    showFriendsBox = 1;
                    inputText = "";
                    enteredInputText = "";
                }
                if (i1 > 166 && mouseButtonClick == 1 && friendsIgnoreMenuSelected == 1)
                {
                    showFriendsBox = 3;
                    inputText = "";
                    enteredInputText = "";
                }
                mouseButtonClick = 0;
            }
        }

        public void drawPrayerMagicMenu(bool canClick)
        {
            int l = gameGraphics.gameWidth - 199;
            int i1 = 36;
            gameGraphics.drawPicture(l - 49, 3, baseInventoryPic + 4);
            int c1 = 196;//'\u304';
            int c2 = 182;//'\u266';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);

            if (menuMagicPrayersSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.drawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.drawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24, c1, 90, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24 + 90, c1, c2 - 90 - 24, GameImage.RgbToInt(160, 160, 160), 128);
            gameGraphics.drawLineX(l, i1 + 24, c1, 0);
            gameGraphics.drawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.drawLineX(l, i1 + 113, c1, 0);
            gameGraphics.drawText("Magic", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.drawText("Prayers", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);

            if (menuMagicPrayersSelected == 0)
            {
                spellMenu.clearList(spellMenuHandle);
                int l1 = 0;

                for (int l2 = 0; l2 < EntityManager.SpellCount; l2++)
                {
                    string s1 = "@yel@";

                    for (int k4 = 0; k4 < EntityManager.GetSpell(l2).RuneCount; k4++)
                    {
                        int j5 = EntityManager.GetSpell(l2).RequiredRunesIds[k4];
                        if (hasRequiredRunes(j5, EntityManager.GetSpell(l2).RequiredRunesCounts[k4]))
                        {
                            continue;
                        }

                        s1 = "@whi@";
                        break;
                    }

                    int k5 = PlayerStatCurrent[6];

                    if (EntityManager.GetSpell(l2).RequiredLevel > k5)
                    {
                        s1 = "@bla@";
                    }

                    spellMenu.addListItem(spellMenuHandle, l1++, s1 + "Level " + EntityManager.GetSpell(l2).RequiredLevel + ": " + EntityManager.GetSpell(l2).Name);
                }

                spellMenu.drawMenu();
                int l3 = spellMenu.getEntryHighlighted(spellMenuHandle);

                if (l3 != -1)
                {
                    gameGraphics.drawString("Level " + EntityManager.GetSpell(l3).RequiredLevel + ": " + EntityManager.GetSpell(l3).Name, l + 2, i1 + 124, 1, 0xffff00);
                    gameGraphics.drawString(EntityManager.GetSpell(l3).Description, l + 2, i1 + 136, 0, 0xffffff);

                    for (int l4 = 0; l4 < EntityManager.GetSpell(l3).RuneCount; l4++)
                    {
                        int l5 = EntityManager.GetSpell(l3).RequiredRunesIds[l4];
                        gameGraphics.drawPicture(l + 2 + l4 * 44, i1 + 150, baseItemPicture + EntityManager.GetItem(l5).InventoryPicture);
                        int i6 = getInventoryItemTotalCount(l5);
                        int j6 = EntityManager.GetSpell(l3).RequiredRunesCounts[l4];
                        string s3 = "@red@";

                        if (hasRequiredRunes(l5, j6))
                        {
                            s3 = "@gre@";
                        }

                        gameGraphics.drawString(s3 + i6 + "/" + j6, l + 2 + l4 * 44, i1 + 150, 1, 0xffffff);
                    }

                }
                else
                {
                    gameGraphics.drawString("Point at a spell for a description", l + 2, i1 + 124, 1, 0);
                }
            }
            if (menuMagicPrayersSelected == 1)
            {
                spellMenu.clearList(spellMenuHandle);
                int i2 = 0;

                for (int i3 = 0; i3 < EntityManager.PrayerCount; i3++)
                {
                    string s2 = "@whi@";
                    if (EntityManager.GetPrayer(i3).RequiredLevel > PlayerStatBase[5])
                    {
                        s2 = "@bla@";
                    }

                    if (prayerOn[i3])
                    {
                        s2 = "@gre@";
                    }

                    spellMenu.addListItem(spellMenuHandle, i2++, s2 + "Level " + EntityManager.GetPrayer(i3).RequiredLevel + ": " + EntityManager.GetPrayer(i3).Name);
                }

                spellMenu.drawMenu();
                int i4 = spellMenu.getEntryHighlighted(spellMenuHandle);

                if (i4 != -1)
                {
                    gameGraphics.drawText("Level " + EntityManager.GetPrayer(i4).RequiredLevel + ": " + EntityManager.GetPrayer(i4).Name, l + c1 / 2, i1 + 130, 1, 0xffff00);
                    gameGraphics.drawText(EntityManager.GetPrayer(i4).Description, l + c1 / 2, i1 + 145, 0, 0xffffff);
                    gameGraphics.drawText("Drain rate: " + EntityManager.GetPrayer(i4).DrainRate, l + c1 / 2, i1 + 160, 1, 0);
                }
                else
                {
                    gameGraphics.drawString("Point at a prayer for a description", l + 2, i1 + 124, 1, 0);
                }
            }

            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 199);
            i1 = InputManager.Instance.MouseLocation.Y - 36;

            if (l >= 0 && i1 >= 0 && l < 196 && i1 < 182)
            {
                spellMenu.mouseClick(l + (gameGraphics.gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98 && menuMagicPrayersSelected == 1)
                    {
                        menuMagicPrayersSelected = 0;
                        spellMenu.switchList(spellMenuHandle);
                    }
                    else
                        if (l > 98 && menuMagicPrayersSelected == 0)
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
                        int j3 = PlayerStatCurrent[6];
                        if (EntityManager.GetSpell(j2).RequiredLevel > j3)
                        {
                            displayMessage("Your magic ability is not high enough for this spell", 3);
                        }
                        else
                        {
                            int j4;
                            for (j4 = 0; j4 < EntityManager.GetSpell(j2).RuneCount; j4++)
                            {
                                int i5 = EntityManager.GetSpell(j2).RequiredRunesIds[j4];
                                if (hasRequiredRunes(i5, EntityManager.GetSpell(j2).RequiredRunesCounts[j4]))
                                {
                                    continue;
                                }

                                displayMessage("You don't have all the reagents you need for this spell", 3);
                                j4 = -1;
                                break;
                            }

                            if (j4 == EntityManager.GetSpell(j2).RuneCount)
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
                        int k3 = PlayerStatBase[5];

                        if (EntityManager.GetPrayer(k2).RequiredLevel > k3)
                        {
                            displayMessage("Your prayer ability is not high enough for this prayer", 3);
                        }
                        else if (PlayerStatCurrent[5] == 0)
                        {
                            displayMessage("You have run out of prayer points. Return to a church to recharge", 3);
                        }
                        else if (prayerOn[k2])
                        {
                            StreamClass.CreatePacket(248);
                            StreamClass.AddInt8(k2);
                            StreamClass.FormatPacket();
                            prayerOn[k2] = false;
                            playSound("prayeroff");
                        }
                        else
                        {
                            StreamClass.CreatePacket(56);
                            StreamClass.AddInt8(k2);
                            StreamClass.FormatPacket();
                            prayerOn[k2] = true;
                            playSound("prayeron");
                        }
                    }
                }

                mouseButtonClick = 0;
            }
        }

        public override sbyte[] unpackData(string arg0, string arg1, int arg2)
        {
            sbyte[] abyte0 = Link.getFile(arg0);

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
                else
                {
                    OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Unpacking " + arg1, arg2));
                    return abyte1;
                }
            }
            else
            {
                OnContentLoaded?.Invoke(this, new ContentLoadedEventArgs("Unpacking " + arg1, arg2));
                return base.unpackData(arg0, arg1, arg2);
            }
        }

        public void drawChatMessageTabs()
        {
            gameGraphics.drawPicture(0, windowHeight - 4, baseInventoryPic + 23);
            int l = GameImage.RgbToInt(200, 200, 255);

            if (messagesTab == 0)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabAllMsgFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.drawText("All messages", 54, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 1)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabHistoryFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.drawText("Chat history", 155, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 2)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabQuestFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.drawText("Quest history", 255, windowHeight + 6, 0, l);
            l = GameImage.RgbToInt(200, 200, 255);
            if (messagesTab == 3)
            {
                l = GameImage.RgbToInt(255, 200, 50);
            }

            if (chatTabPrivateFlash % 30 > 15)
            {
                l = GameImage.RgbToInt(255, 50, 50);
            }

            gameGraphics.drawText("Private history", 355, windowHeight + 6, 0, l);
            gameGraphics.drawText("Report abuse", 457, windowHeight + 6, 0, 0xffffff);
        }

        //public URL getDocumentBase() {
        //    if(link.gameApplet != null)
        //        return link.gameApplet.getDocumentBase();
        //    else
        //        return base.getDocumentBase();
        //}

        delegate void SendPingPacketDelegate();
        readonly object _sync = new object();
        public static bool sendingPing = false;
        public void sendPingPacketAsync()
        {
            SendPingPacketDelegate worker = new SendPingPacketDelegate(SendPing);
            AsyncCallback completedCallback = new AsyncCallback(sendPingPacketCompletedCallback);

            lock (_sync)
            {
                if (sendingPing)
                {
                    return;//throw new InvalidOperationException("The control is currently busy.");
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
            if (SystemUpdateTimer > 1)
            {
                SystemUpdateTimer--;
            }

            if (WildernessModeTimer >= 1)
            {
                WildernessModeTimer -= 1;
            }

            if (PvpTournamentTimer >= 1)
            {
                PvpTournamentTimer -= 1;
            }

            if (DropPartyTimer >= 1)
            {
                DropPartyTimer -= 1;
            }

            sendPingPacketAsync();




            if (logoutTimer > 0)
            {
                logoutTimer--;
            }

            if (CurrentPlayer.currentSprite == 8 || CurrentPlayer.currentSprite == 9)
            {
                combatTimeout = 500;
            }

            if (combatTimeout > 0)
            {
                combatTimeout--;
            }

            if (ShowAppearanceWindow)
            {
                updateAppearanceWindow();
                return;
            }
            for (int l = 0; l < PlayerCount; l++)
            {
                Mob player = Players[l];
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

                    if (player.WaypointsX[targetSprite] - player.currentX > GridSize * 3 || player.WaypointsY[targetSprite] - player.currentY > GridSize * 3 || player.WaypointsX[targetSprite] - player.currentX < -GridSize * 3 || player.WaypointsY[targetSprite] - player.currentY < -GridSize * 3 || i5 > 8)
                    {
                        player.currentX = player.WaypointsX[targetSprite];
                        player.currentY = player.WaypointsY[targetSprite];
                    }
                    else
                    {
                        if (player.currentX < player.WaypointsX[targetSprite])
                        {
                            player.currentX += i6;
                            player.stepCount++;
                            direction = 2;
                        }
                        else
                            if (player.currentX > player.WaypointsX[targetSprite])
                        {
                            player.currentX -= i6;
                            player.stepCount++;
                            direction = 6;
                        }
                        if (player.currentX - player.WaypointsX[targetSprite] < i6 && player.currentX - player.WaypointsX[targetSprite] > -i6)
                        {
                            player.currentX = player.WaypointsX[targetSprite];
                        }

                        if (player.currentY < player.WaypointsY[targetSprite])
                        {
                            player.currentY += i6;
                            player.stepCount++;
                            if (direction == -1)
                            {
                                direction = 4;
                            }
                            else
                                if (direction == 2)
                            {
                                direction = 3;
                            }
                            else
                            {
                                direction = 5;
                            }
                        }
                        else
                            if (player.currentY > player.WaypointsY[targetSprite])
                        {
                            player.currentY -= i6;
                            player.stepCount++;
                            if (direction == -1)
                            {
                                direction = 0;
                            }
                            else
                                if (direction == 2)
                            {
                                direction = 1;
                            }
                            else
                            {
                                direction = 7;
                            }
                        }
                        if (player.currentY - player.WaypointsY[targetSprite] < i6 && player.currentY - player.WaypointsY[targetSprite] > -i6)
                        {
                            player.currentY = player.WaypointsY[targetSprite];
                        }
                    }
                    if (direction != -1)
                    {
                        player.currentSprite = direction;
                    }

                    if (player.currentX == player.WaypointsX[targetSprite] && player.currentY == player.WaypointsY[targetSprite])
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
                        displayMessage("You have been granted another life. Be more careful this time!", 3);
                    }

                    if (PlayerAliveTimeout == 0)
                    {
                        displayMessage("You retain your skills. Your objects land where you died", 3);
                    }
                }
            }

            for (int i1 = 0; i1 < NpcCount; i1++)
            {
                Mob f2 = Npcs[i1];
                int i2 = (f2.WaypointCurrent + 1) % 10;
                if (f2.WaypointsEndSprite != i2)
                {
                    int l3 = -1;
                    int j5 = f2.WaypointsEndSprite;
                    int j6;
                    if (j5 < i2)
                    {
                        j6 = i2 - j5;
                    }
                    else
                    {
                        j6 = (10 + i2) - j5;
                    }

                    int k6 = 4;
                    if (j6 > 2)
                    {
                        k6 = (j6 - 1) * 4;
                    }

                    if (f2.WaypointsX[j5] - f2.currentX > GridSize * 3 ||
                        f2.WaypointsY[j5] - f2.currentY > GridSize * 3 ||
                        f2.WaypointsX[j5] - f2.currentX < -GridSize * 3 ||
                        f2.WaypointsY[j5] - f2.currentY < -GridSize * 3 || j6 > 8)
                    {
                        f2.currentX = f2.WaypointsX[j5];
                        f2.currentY = f2.WaypointsY[j5];
                    }
                    else
                    {
                        if (f2.currentX < f2.WaypointsX[j5])
                        {
                            f2.currentX += k6;
                            f2.stepCount++;
                            l3 = 2;
                        }
                        else
                            if (f2.currentX > f2.WaypointsX[j5])
                        {
                            f2.currentX -= k6;
                            f2.stepCount++;
                            l3 = 6;
                        }
                        if (f2.currentX - f2.WaypointsX[j5] < k6 && f2.currentX - f2.WaypointsX[j5] > -k6)
                        {
                            f2.currentX = f2.WaypointsX[j5];
                        }

                        if (f2.currentY < f2.WaypointsY[j5])
                        {
                            f2.currentY += k6;
                            f2.stepCount++;
                            if (l3 == -1)
                            {
                                l3 = 4;
                            }
                            else
                                if (l3 == 2)
                            {
                                l3 = 3;
                            }
                            else
                            {
                                l3 = 5;
                            }
                        }
                        else
                            if (f2.currentY > f2.WaypointsY[j5])
                        {
                            f2.currentY -= k6;
                            f2.stepCount++;
                            if (l3 == -1)
                            {
                                l3 = 0;
                            }
                            else
                                if (l3 == 2)
                            {
                                l3 = 1;
                            }
                            else
                            {
                                l3 = 7;
                            }
                        }
                        if (f2.currentY - f2.WaypointsY[j5] < k6 && f2.currentY - f2.WaypointsY[j5] > -k6)
                        {
                            f2.currentY = f2.WaypointsY[j5];
                        }
                    }
                    if (l3 != -1)
                    {
                        f2.currentSprite = l3;
                    }

                    if (f2.currentX == f2.WaypointsX[j5] && f2.currentY == f2.WaypointsY[j5])
                    {
                        f2.WaypointsEndSprite = (j5 + 1) % 10;
                    }
                }
                else
                {
                    f2.currentSprite = f2.nextSprite;
                    if (f2.npcId == 43)
                    {
                        f2.stepCount++;
                    }
                }
                if (f2.lastMessageTimeout > 0)
                {
                    f2.lastMessageTimeout--;
                }

                if (f2.PlayerSkullTimeout > 0)
                {
                    f2.PlayerSkullTimeout--;
                }

                if (f2.combatTimer > 0)
                {
                    f2.combatTimer--;
                }
            }

            if (drawMenuTab != 2)
            {
                if (GameImage.bnn > 0)
                {
                    sleepWordDelayTimer++;
                }

                if (GameImage.caa > 0)
                {
                    sleepWordDelayTimer = 0;
                }

                GameImage.bnn = 0;
                GameImage.caa = 0;
            }
            for (int k1 = 0; k1 < PlayerCount; k1++)
            {
                Mob f3 = Players[k1];
                if (f3.ProjectileDistance > 0)
                {
                    f3.ProjectileDistance--;
                }
            }

            if (cameraAutoAngleDebug)
            {
                if (cameraAutoRotatePlayerX - CurrentPlayer.currentX < -500 || cameraAutoRotatePlayerX - CurrentPlayer.currentX > 500 || cameraAutoRotatePlayerY - CurrentPlayer.currentY < -500 || cameraAutoRotatePlayerY - CurrentPlayer.currentY > 500)
                {
                    cameraAutoRotatePlayerX = CurrentPlayer.currentX;
                    cameraAutoRotatePlayerY = CurrentPlayer.currentY;
                }
            }
            else
            {
                if (cameraAutoRotatePlayerX - CurrentPlayer.currentX < -500 || cameraAutoRotatePlayerX - CurrentPlayer.currentX > 500 || cameraAutoRotatePlayerY - CurrentPlayer.currentY < -500 || cameraAutoRotatePlayerY - CurrentPlayer.currentY > 500)
                {
                    cameraAutoRotatePlayerX = CurrentPlayer.currentX;
                    cameraAutoRotatePlayerY = CurrentPlayer.currentY;
                }
                if (cameraAutoRotatePlayerX != CurrentPlayer.currentX)
                {
                    cameraAutoRotatePlayerX += (CurrentPlayer.currentX - cameraAutoRotatePlayerX) / (16 + (cameraDistance - 500) / 15);
                }

                if (cameraAutoRotatePlayerY != CurrentPlayer.currentY)
                {
                    cameraAutoRotatePlayerY += (CurrentPlayer.currentY - cameraAutoRotatePlayerY) / (16 + (cameraDistance - 500) / 15);
                }

                if (CameraAutoAngle)
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
                        else
                            if (i4 > 0)
                        {
                            byte0 = 1;
                        }
                        else
                                if (i4 < -128)
                        {
                            byte0 = 1;
                            i4 = 256 + i4;
                        }
                        else
                                    if (i4 < 0)
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
                if (enteredInputText.Length > 0)
                {
                    if (enteredInputText.ToLower().Equals("::lostcon"))
                    {
                        StreamClass.CloseStream();
                    }
                    else
                        if (enteredInputText.ToLower().Equals("::closecon"))
                    {
                        requestLogout();
                    }
                    else
                    {
                        StreamClass.CreatePacket(200);
                        StreamClass.AddString(enteredInputText);
                        if (!sleepWordDelay)
                        {
                            StreamClass.AddInt8(0);
                            sleepWordDelay = true;
                        }
                        StreamClass.FormatPacket();
                        inputText = "";
                        enteredInputText = "";
                        sleepingStatusText = "Please wait...";
                    }
                }

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
                    inputText = "";
                    enteredInputText = "";
                    sleepingStatusText = "Please wait...";
                }
                lastMouseButton = 0;
                return;
            }
            if (InputManager.Instance.MouseLocation.Y > windowHeight - 4)
            {
                if (InputManager.Instance.MouseLocation.X > 15 && InputManager.Instance.MouseLocation.X < 96 && lastMouseButton == 1)
                {
                    messagesTab = 0;
                }

                if (InputManager.Instance.MouseLocation.X > 110 && InputManager.Instance.MouseLocation.X < 194 && lastMouseButton == 1)
                {
                    messagesTab = 1;
                    chatInputMenu.listShownEntries[messagesHandleType2] = 0xf423f;
                }
                if (InputManager.Instance.MouseLocation.X > 215 && InputManager.Instance.MouseLocation.X < 295 && lastMouseButton == 1)
                {
                    messagesTab = 2;
                    chatInputMenu.listShownEntries[messagesHandleType5] = 0xf423f;
                }
                if (InputManager.Instance.MouseLocation.X > 315 && InputManager.Instance.MouseLocation.X < 395 && lastMouseButton == 1)
                {
                    messagesTab = 3;
                    chatInputMenu.listShownEntries[messagesHandleType6] = 0xf423f;
                }
                if (InputManager.Instance.MouseLocation.X > 417 && InputManager.Instance.MouseLocation.X < 497 && lastMouseButton == 1)
                {
                    showAbuseBox = 1;
                    reportAbuseOptionSelected = 0;
                    inputText = "";
                    enteredInputText = "";
                }
                lastMouseButton = 0;
                mouseButton = 0;
            }
            chatInputMenu.mouseClick(InputManager.Instance.MouseLocation.X, InputManager.Instance.MouseLocation.Y, lastMouseButton, mouseButton);

            if (messagesTab > 0 && InputManager.Instance.MouseLocation.X >= 494 && InputManager.Instance.MouseLocation.Y >= windowHeight - 66)
            {
                lastMouseButton = 0;
            }

            if (chatInputMenu.isClicked(chatInputBox))
            {
                string input = chatInputMenu.getText(chatInputBox);
                chatInputMenu.updateText(chatInputBox, "");

                if (input.StartsWith("::"))
                {
                    if (!handleCommand(input.Substring(2)))
                    {
                        SendCommand(input.Substring(2));
                    }
                }
                else
                {
                    int len = ChatMessage.stringToBytes(input);
                    input = ChatMessage.bytesToString(ChatMessage.lastChat, 0, len);
                    //SendChatMessage(ChatMessage.lastChat, len);
                    SendChatMessage(input);
                    //if (useChatFilter)
                    //input = ChatFilter.filterChat(input);
                    CurrentPlayer.lastMessageTimeout = 150;
                    CurrentPlayer.lastMessage = input;
                    displayMessage(CurrentPlayer.username + ": " + input, 2);
                }
            }
            if (messagesTab == 0)
            {
                for (int k2 = 0; k2 < 5; k2++)
                {
                    if (messagesTimeout[k2] > 0)
                    {
                        messagesTimeout[k2]--;
                    }
                }
            }
            if (PlayerAliveTimeout != 0)
            {
                lastMouseButton = 0;
            }

            if (showTradeBox || ShowDuelBox)
            {
                if (mouseButton != 0)
                {
                    mouseButtonHeldTime++;
                }
                else
                {
                    mouseButtonHeldTime = 0;
                }

                if (mouseButtonHeldTime > 500)
                {
                    mouseClickedHeldInTradeDuelBox += 100000;
                }
                else if (mouseButtonHeldTime > 350)
                {
                    mouseClickedHeldInTradeDuelBox += 10000;
                }
                else if (mouseButtonHeldTime > 250)
                {
                    mouseClickedHeldInTradeDuelBox += 1000;
                }
                else if (mouseButtonHeldTime > 150)
                {
                    mouseClickedHeldInTradeDuelBox += 100;
                }
                else if (mouseButtonHeldTime > 100)
                {
                    mouseClickedHeldInTradeDuelBox += 10;
                }
                else if (mouseButtonHeldTime > 50)
                {
                    mouseClickedHeldInTradeDuelBox++;
                }
                else if (mouseButtonHeldTime > 20 && (mouseButtonHeldTime & 5) == 0)
                {
                    mouseClickedHeldInTradeDuelBox++;
                }
            }
            else
            {
                mouseButtonHeldTime = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
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
            if (CameraAutoAngle)
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

            if (fogOfWar)
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
            else
                if (actionPictureType < 0)
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
            for (int j3 = 0; j3 < ObjectCount; j3++)
            {
                int k4 = ObjectX[j3];
                int k5 = ObjectY[j3];
                if (k4 >= 0 && k5 >= 0 && k4 < 96 && k5 < 96 && ObjectType[j3] == 74)
                {
                    ObjectArray[j3].offsetMiniPosition(1, 0, 0);
                }
            }

            for (int l4 = 0; l4 < teleBubbleCount; l4++)
            {
                teleBubbleTime[l4]++;
                if (teleBubbleTime[l4] > 50)
                {
                    teleBubbleCount--;
                    for (int l5 = l4; l5 < teleBubbleCount; l5++)
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
                if (key == Keys.F12)
                {
                    takeScreenshot(true);
                }
                else if (ShowAppearanceWindow && appearanceMenu != null)
                {
                    appearanceMenu.keyPress(key, c);
                }
                else if (showFriendsBox == 0 && showAbuseBox == 0 && !IsSleeping && chatInputMenu != null)
                {
                    chatInputMenu.keyPress(key, c);
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
            int l = 2203 - (SectionY + WildY + AreaY);
            if (SectionX + WildX + AreaX >= 2640)
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
                        if (type == 1)
                        {
                            string s1 = "";
                            int k4 = 0;
                            if (CurrentPlayer.CombatLevel > 0 && Players[index].CombatLevel > 0)
                            {
                                k4 = CurrentPlayer.CombatLevel - Players[index].CombatLevel;
                            }

                            if (k4 < 0)
                            {
                                s1 = "@or1@";
                            }

                            if (k4 < -3)
                            {
                                s1 = "@or2@";
                            }

                            if (k4 < -6)
                            {
                                s1 = "@or3@";
                            }

                            if (k4 < -9)
                            {
                                s1 = "@red@";
                            }

                            if (k4 > 0)
                            {
                                s1 = "@gr1@";
                            }

                            if (k4 > 3)
                            {
                                s1 = "@gr2@";
                            }

                            if (k4 > 6)
                            {
                                s1 = "@gr3@";
                            }

                            if (k4 > 9)
                            {
                                s1 = "@gre@";
                            }

                            s1 = " " + s1 + "(level-" + Players[index].CombatLevel + ")";
                            if (selectedSpell >= 0)
                            {
                                if (EntityManager.GetSpell(selectedSpell).Type == 1 || EntityManager.GetSpell(selectedSpell).Type == 2)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnPlayer;
                                    menuActionX[menuOptionsCount] = Players[index].currentX;
                                    menuActionY[menuOptionsCount] = Players[index].currentY;
                                    menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithPlayer;
                                menuActionX[menuOptionsCount] = Players[index].currentX;
                                menuActionY[menuOptionsCount] = Players[index].currentY;
                                menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (l > 0 && (Players[index].currentY - 64) / GridSize + WildY + AreaY < 2203)
                                {
                                    menuText1[menuOptionsCount] = "Attack";
                                    menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                    if (k4 >= 0 && k4 < 5)
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackPlayer;
                                    }
                                    else
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackPlayer2;
                                    }

                                    menuActionX[menuOptionsCount] = Players[index].currentX;
                                    menuActionY[menuOptionsCount] = Players[index].currentY;
                                    menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                    menuOptionsCount++;
                                }
                                else if (GameDefines.PREMIUM_FEATURES)
                                {
                                    menuText1[menuOptionsCount] = "Duel with";
                                    menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                    menuActionX[menuOptionsCount] = Players[index].currentX;
                                    menuActionY[menuOptionsCount] = Players[index].currentY;
                                    menuActions[menuOptionsCount] = MenuAction.DuelWithPlayer;
                                    menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Trade with";
                                menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                menuActions[menuOptionsCount] = MenuAction.TradeWithPlayer;
                                menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                menuOptionsCount++;
                                menuText1[menuOptionsCount] = "Follow";
                                menuText2[menuOptionsCount] = "@whi@" + Players[index].username + s1;
                                menuActions[menuOptionsCount] = MenuAction.FollowPlayer;
                                menuActionType[menuOptionsCount] = Players[index].ServerIndex;
                                menuOptionsCount++;
                            }
                        }
                        else
                            if (type == 2)
                        {
                            if (selectedSpell >= 0)
                            {
                                if (EntityManager.GetSpell(selectedSpell).Type == 3)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(GroundItemId[index]).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnGroundItem;
                                    menuActionX[menuOptionsCount] = GroundItemX[index];
                                    menuActionY[menuOptionsCount] = GroundItemY[index];
                                    menuActionType[menuOptionsCount] = GroundItemId[index];
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithGroundItem;
                                menuActionX[menuOptionsCount] = GroundItemX[index];
                                menuActionY[menuOptionsCount] = GroundItemY[index];
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                menuText1[menuOptionsCount] = "Take";
                                menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.TakeItem;
                                menuActionX[menuOptionsCount] = GroundItemX[index];
                                menuActionY[menuOptionsCount] = GroundItemY[index];
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuOptionsCount++;
                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@lre@" + EntityManager.GetItem(GroundItemId[index]).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineGroundItem;
                                menuActionType[menuOptionsCount] = GroundItemId[index];
                                menuOptionsCount++;
                            }
                        }
                        else
                                if (type == 3)
                        {
                            string s2 = "";
                            int l4 = -1;
                            int id = Npcs[index].npcId;
                            if (EntityManager.GetNpc(id).IsAttackable > 0)
                            {
                                int j5 = (EntityManager.GetNpc(id).AttackLevel + EntityManager.GetNpc(id).DefenceLevel + EntityManager.GetNpc(id).StrengthLevel + EntityManager.GetNpc(id).HealthLevel) / 4;
                                int k5 = (PlayerStatBase[0] + PlayerStatBase[1] + PlayerStatBase[2] + PlayerStatBase[3] + 27) / 4;
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
                                if (EntityManager.GetSpell(selectedSpell).Type == 2)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnNpc;
                                    menuActionX[menuOptionsCount] = Npcs[index].currentX;
                                    menuActionY[menuOptionsCount] = Npcs[index].currentY;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithNpc;
                                menuActionX[menuOptionsCount] = Npcs[index].currentX;
                                menuActionY[menuOptionsCount] = Npcs[index].currentY;
                                menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (EntityManager.GetNpc(id).IsAttackable > 0)
                                {
                                    menuText1[menuOptionsCount] = "Attack";
                                    menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name + s2;
                                    if (l4 >= 0)
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackNpc;
                                    }
                                    else
                                    {
                                        menuActions[menuOptionsCount] = MenuAction.AttackNpc2;
                                    }

                                    menuActionX[menuOptionsCount] = Npcs[index].currentX;
                                    menuActionY[menuOptionsCount] = Npcs[index].currentY;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuOptionsCount++;
                                }
                                menuText1[menuOptionsCount] = "Talk-to";
                                menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.TalkToNpc;
                                menuActionX[menuOptionsCount] = Npcs[index].currentX;
                                menuActionY[menuOptionsCount] = Npcs[index].currentY;
                                menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                menuOptionsCount++;
                                if (!EntityManager.GetNpc(id).Command.Equals(""))
                                {
                                    menuText1[menuOptionsCount] = EntityManager.GetNpc(id).Command;
                                    menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CommandOnNpc;
                                    menuActionX[menuOptionsCount] = Npcs[index].currentX;
                                    menuActionY[menuOptionsCount] = Npcs[index].currentY;
                                    menuActionType[menuOptionsCount] = Npcs[index].ServerIndex;
                                    menuOptionsCount++;
                                }
                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@yel@" + EntityManager.GetNpc(Npcs[index].npcId).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineNpc;
                                menuActionType[menuOptionsCount] = Npcs[index].npcId;
                                menuOptionsCount++;
                            }
                        }
                    }
                    else if (_obj != null && _obj.index >= 10000)
                    {
                        int j3 = _obj.index - 10000;
                        int i4 = WallObjectId[j3];

                        if (!WallObjectAlreadyInMenu[j3])
                        {
                            if (selectedSpell >= 0)
                            {
                                if (EntityManager.GetSpell(selectedSpell).Type == 4)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnWallObject;
                                    menuActionX[menuOptionsCount] = WallObjectX[j3];
                                    menuActionY[menuOptionsCount] = WallObjectY[j3];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[j3];
                                    menuActionVar1[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWallObject(i4).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithWallObject;
                                menuActionX[menuOptionsCount] = WallObjectX[j3];
                                menuActionY[menuOptionsCount] = WallObjectY[j3];
                                menuActionType[menuOptionsCount] = WallObjectDirection[j3];
                                menuActionVar1[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (!EntityManager.GetWallObject(i4).Command1.ToLower().Equals("WalkTo"))
                                {
                                    menuText1[menuOptionsCount] = EntityManager.GetWallObject(i4).Command1;
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command1OnWallObject;
                                    menuActionX[menuOptionsCount] = WallObjectX[j3];
                                    menuActionY[menuOptionsCount] = WallObjectY[j3];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[j3];
                                    menuOptionsCount++;
                                }
                                if (!EntityManager.GetWallObject(i4).Command2.ToLower().Equals("Examine"))
                                {
                                    menuText1[menuOptionsCount] = EntityManager.GetWallObject(i4).Command2;
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWallObject(i4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command2OnWallObject;
                                    menuActionX[menuOptionsCount] = WallObjectX[j3];
                                    menuActionY[menuOptionsCount] = WallObjectY[j3];
                                    menuActionType[menuOptionsCount] = WallObjectDirection[j3];
                                    menuOptionsCount++;
                                }

                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWallObject(i4).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineWallObject;
                                menuActionType[menuOptionsCount] = i4;
                                menuOptionsCount++;
                            }
                            WallObjectAlreadyInMenu[j3] = true;
                        }
                    }
                    else if (_obj != null && _obj.index >= 0)
                    {
                        int k3 = _obj.index;
                        int j4 = ObjectType[k3];

                        if (!objectAlreadyInMenu[k3])
                        {
                            if (selectedSpell >= 0)
                            {
                                if (EntityManager.GetSpell(selectedSpell).Type == 5)
                                {
                                    menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on";
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.CastSpellOnModel;
                                    menuActionX[menuOptionsCount] = ObjectX[k3];
                                    menuActionY[menuOptionsCount] = ObjectY[k3];
                                    menuActionType[menuOptionsCount] = ObjectRotation[k3];
                                    menuActionVar1[menuOptionsCount] = ObjectType[k3];
                                    menuActionVar2[menuOptionsCount] = selectedSpell;
                                    menuOptionsCount++;
                                }
                            }
                            else
                                if (selectedItem >= 0)
                            {
                                menuText1[menuOptionsCount] = "Use " + selectedItemName + " with";
                                menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWorldObject(j4).Name;
                                menuActions[menuOptionsCount] = MenuAction.UseItemWithModel;
                                menuActionX[menuOptionsCount] = ObjectX[k3];
                                menuActionY[menuOptionsCount] = ObjectY[k3];
                                menuActionType[menuOptionsCount] = ObjectRotation[k3];
                                menuActionVar1[menuOptionsCount] = ObjectType[k3];
                                menuActionVar2[menuOptionsCount] = selectedItem;
                                menuOptionsCount++;
                            }
                            else
                            {
                                if (!EntityManager.GetWorldObject(j4).Command1.ToLower().Equals("WalkTo"))
                                {
                                    menuText1[menuOptionsCount] = EntityManager.GetWorldObject(j4).Command1;
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command1OnModel;
                                    menuActionX[menuOptionsCount] = ObjectX[k3];
                                    menuActionY[menuOptionsCount] = ObjectY[k3];
                                    menuActionType[menuOptionsCount] = ObjectRotation[k3];
                                    menuActionVar1[menuOptionsCount] = ObjectType[k3];
                                    menuOptionsCount++;
                                }
                                if (!EntityManager.GetWorldObject(j4).Command2.ToLower().Equals("Examine"))
                                {
                                    menuText1[menuOptionsCount] = EntityManager.GetWorldObject(j4).Command2;
                                    menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWorldObject(j4).Name;
                                    menuActions[menuOptionsCount] = MenuAction.Command2OnModel;
                                    menuActionX[menuOptionsCount] = ObjectX[k3];
                                    menuActionY[menuOptionsCount] = ObjectY[k3];
                                    menuActionType[menuOptionsCount] = ObjectRotation[k3];
                                    menuActionVar1[menuOptionsCount] = ObjectType[k3];
                                    menuOptionsCount++;
                                }
                                menuText1[menuOptionsCount] = "Examine";
                                menuText2[menuOptionsCount] = "@cya@" + EntityManager.GetWorldObject(j4).Name;
                                menuActions[menuOptionsCount] = MenuAction.ExamineModel;
                                menuActionType[menuOptionsCount] = j4;
                                menuOptionsCount++;
                            }
                            objectAlreadyInMenu[k3] = true;
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

            if (selectedSpell >= 0 && EntityManager.GetSpell(selectedSpell).Type <= 1)
            {
                menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on self";
                menuText2[menuOptionsCount] = "";
                menuActions[menuOptionsCount] = MenuAction.CastSpellOnSelf;
                menuActionType[menuOptionsCount] = selectedSpell;
                menuOptionsCount++;
            }
            if (ground != -1)
            {
                if (selectedSpell >= 0)
                {
                    if (EntityManager.GetSpell(selectedSpell).Type == 6)
                    {
                        menuText1[menuOptionsCount] = "Cast " + EntityManager.GetSpell(selectedSpell).Name + " on ground";
                        menuText2[menuOptionsCount] = "";
                        menuActions[menuOptionsCount] = MenuAction.CastSpellOnGround;
                        menuActionX[menuOptionsCount] = engineHandle.selectedX[ground];
                        menuActionY[menuOptionsCount] = engineHandle.selectedY[ground];
                        menuActionType[menuOptionsCount] = selectedSpell;
                        menuOptionsCount++;
                        return;
                    }
                }
                else
                    if (selectedItem < 0)
                {
                    menuText1[menuOptionsCount] = "Walk here";
                    menuText2[menuOptionsCount] = "";
                    menuActions[menuOptionsCount] = MenuAction.WalkHere;
                    menuActionX[menuOptionsCount] = engineHandle.selectedX[ground];
                    menuActionY[menuOptionsCount] = engineHandle.selectedY[ground];
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
                        int i3 = shopItems[selectedShopItemIndex];
                        if (i3 != -1)
                        {
                            if (shopItemCount[selectedShopItemIndex] > 0 && l > 298 && i1 >= 204 && l < 408 && i1 <= 215)
                            {
                                StreamClass.CreatePacket(128);
                                StreamClass.AddInt16(shopItems[selectedShopItemIndex]);
                                StreamClass.AddInt32(shopItemBuyPrice[selectedShopItemIndex]);
                                StreamClass.FormatPacket();
                            }
                            if (getInventoryItemTotalCount(i3) > 0 && l > 2 && i1 >= 229 && l < 112 && i1 <= 240)
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
            gameGraphics.drawBox(_offsetX, _offsetY, 408, 12, 192);
            int k1 = 0x989898;
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 12, 408, 17, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 29, 8, 170, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX + 399, _offsetY + 29, 9, 170, k1, 160);
            gameGraphics.drawBoxAlpha(_offsetX, _offsetY + 199, 408, 47, k1, 160);
            gameGraphics.drawString("Buying and selling items", _offsetX + 1, _offsetY + 10, 1, 0xffffff);
            int i2 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > _offsetX + 320 && InputManager.Instance.MouseLocation.Y >= _offsetY && InputManager.Instance.MouseLocation.X < _offsetX + 408 && InputManager.Instance.MouseLocation.Y < _offsetY + 12)
            {
                i2 = 0xff0000;
            }

            gameGraphics.drawLabel("Close window", _offsetX + 406, _offsetY + 10, 1, i2);
            gameGraphics.drawString("Shops stock in green", _offsetX + 2, _offsetY + 24, 1, 65280);
            gameGraphics.drawString("Number you own in blue", _offsetX + 135, _offsetY + 24, 1, 65535);
            gameGraphics.drawString("Your money: " + getInventoryItemTotalCount(10) + "gp", _offsetX + 280, _offsetY + 24, 1, 0xffff00);
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

                    gameGraphics.drawBoxEdge(i6, l6, 50, 35, 0);
                    if (shopItems[j4] != -1)
                    {
                        gameGraphics.drawImage(i6, l6, 48, 32, baseItemPicture + EntityManager.GetItem(shopItems[j4]).InventoryPicture, EntityManager.GetItem(shopItems[j4]).PictureMask, 0, 0, false);
                        gameGraphics.drawString(shopItemCount[j4].ToString(), i6 + 1, l6 + 10, 1, 65280);
                        gameGraphics.drawLabel(getInventoryItemTotalCount(shopItems[j4]).ToString(), i6 + 47, l6 + 10, 1, 65535);
                    }
                    j4++;
                }

            }

            gameGraphics.drawLineX(_offsetX + 5, _offsetY + 222, 398, 0);
            if (selectedShopItemIndex == -1)
            {
                gameGraphics.drawText("Select an object to buy or sell", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                return;
            }
            int l5 = shopItems[selectedShopItemIndex];
            if (l5 != -1)
            {
                if (shopItemCount[selectedShopItemIndex] > 0)
                {
                    int j6 = shopItemBuyPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (j6 < 10)
                    {
                        j6 = 10;
                    }

                    int i7 = (j6 * EntityManager.GetItem(l5).BasePrice) / 100;
                    gameGraphics.drawString("Buy a new " + EntityManager.GetItem(l5).Name + " for " + i7 + "gp", _offsetX + 2, _offsetY + 214, 1, 0xffff00);
                    int j2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X > _offsetX + 298 && InputManager.Instance.MouseLocation.Y >= _offsetY + 204 && InputManager.Instance.MouseLocation.X < _offsetX + 408 && InputManager.Instance.MouseLocation.Y <= _offsetY + 215)
                    {
                        j2 = 0xff0000;
                    }

                    gameGraphics.drawLabel("Click here to buy", _offsetX + 405, _offsetY + 214, 3, j2);
                }
                else
                {
                    gameGraphics.drawText("This item is not currently available to buy", _offsetX + 204, _offsetY + 214, 3, 0xffff00);
                }
                if (getInventoryItemTotalCount(l5) > 0)
                {
                    int k6 = shopItemSellPriceModifier + shopItemBasePriceModifier[selectedShopItemIndex];
                    if (k6 < 10)
                    {
                        k6 = 10;
                    }

                    int j7 = (k6 * EntityManager.GetItem(l5).BasePrice) / 100;
                    gameGraphics.drawLabel("Sell your " + EntityManager.GetItem(l5).Name + " for " + j7 + "gp", _offsetX + 405, _offsetY + 239, 1, 0xffff00);
                    int k2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X > _offsetX + 2 && InputManager.Instance.MouseLocation.Y >= _offsetY + 229 && InputManager.Instance.MouseLocation.X < _offsetX + 112 && InputManager.Instance.MouseLocation.Y <= _offsetY + 240)
                    {
                        k2 = 0xff0000;
                    }

                    gameGraphics.drawString("Click here to sell", _offsetX + 2, _offsetY + 239, 3, k2);
                    return;
                }
                gameGraphics.drawText("You do not have any of this item to sell", _offsetX + 204, _offsetY + 239, 3, 0xffff00);
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
            gameCamera.createTexture(EntityManager.TextureCount, 7, 11);
            for (int l = 0; l < EntityManager.TextureCount; l++)
            {
                string s1 = EntityManager.GetTexture(l).Name;
                sbyte[] abyte2 = DataOperations.loadData(s1 + ".dat", 0, abyte0);
                gameGraphics.unpackImageData(baseTexturePic, abyte2, abyte1, 1);
                gameGraphics.drawBox(0, 0, 128, 128, 0xff00ff);
                gameGraphics.drawPicture(0, 0, baseTexturePic);
                int i1 = gameGraphics.pictureAssumedWidth[baseTexturePic];
                string s2 = EntityManager.GetTexture(l).SubName;
                if (s2 != null && s2.Length > 0)
                {
                    sbyte[] abyte3 = DataOperations.loadData(s2 + ".dat", 0, abyte0);
                    gameGraphics.unpackImageData(baseTexturePic, abyte3, abyte1, 1);
                    gameGraphics.drawPicture(0, 0, baseTexturePic);
                }
                gameGraphics.drawImage(subTexturePic + l, 0, 0, i1, i1);
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
            gameGraphics.interlacingEnabled = false;
            gameGraphics.ClearScreen();
            appearanceMenu.drawMenu();
            int l = 140;
            int i1 = 50;
            l += 116;
            i1 -= 25;
            gameGraphics.drawCharacterLegs(l - 32 - 55, i1, 64, 102, EntityManager.GetAnimation(appearance2Colour).Number, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.drawImage(l - 32 - 55, i1, 64, 102, EntityManager.GetAnimation(appearanceBodyGender).Number, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawImage(l - 32 - 55, i1, 64, 102, EntityManager.GetAnimation(appearanceHeadType).Number, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawCharacterLegs(l - 32, i1, 64, 102, EntityManager.GetAnimation(appearance2Colour).Number + 6, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.drawImage(l - 32, i1, 64, 102, EntityManager.GetAnimation(appearanceBodyGender).Number + 6, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawImage(l - 32, i1, 64, 102, EntityManager.GetAnimation(appearanceHeadType).Number + 6, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawCharacterLegs((l - 32) + 55, i1, 64, 102, EntityManager.GetAnimation(appearance2Colour).Number + 12, appearanceTopBottomColours[appearanceBottomColour]);
            gameGraphics.drawImage((l - 32) + 55, i1, 64, 102, EntityManager.GetAnimation(appearanceBodyGender).Number + 12, appearanceTopBottomColours[appearanceTopColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawImage((l - 32) + 55, i1, 64, 102, EntityManager.GetAnimation(appearanceHeadType).Number + 12, appearanceHairColours[appearanceHairColour], appearanceSkinColours[appearanceSkinColour], 0, false);
            gameGraphics.drawPicture(0, windowHeight, baseInventoryPic + 22);
            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.drawImage(spriteBatch, 0, 0);
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
                    gameGraphics.drawString(s1, 6, 14, 1, 0xffff00);
                }

                if (!OneMouseButton && mouseButtonClick == 1 || OneMouseButton && mouseButtonClick == 1 && menuOptionsCount == 1)
                {
                    menuClick(menuIndexes[0]);
                    mouseButtonClick = 0;
                    return;
                }
                if (!OneMouseButton && mouseButtonClick == 2 || OneMouseButton && mouseButtonClick == 1)
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
                gameGraphics.screenFadeToBlack();
                gameGraphics.drawText("Oh dear! You are dead...", windowWidth / 2, windowHeight / 2, 7, 0xff0000);
                drawChatMessageTabs();
                //gameGraphics.UpdateGameImage();
                OnDrawDone();//gameGraphics.drawImage(spriteBatch, 0, 0);
                return;
            }
            if (ShowAppearanceWindow)
            {
                drawAppearanceWindow();
                return;
            }
            if (IsSleeping)
            {
                gameGraphics.screenFadeToBlack();
                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    gameGraphics.drawText("ZZZ", (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                if (Helper.Random.NextDouble() < 0.14999999999999999D)
                {
                    gameGraphics.drawText("ZZZ", 512 - (int)(Helper.Random.NextDouble() * 80D), (int)(Helper.Random.NextDouble() * 334D), 5, (int)(Helper.Random.NextDouble() * 16777215D));
                }

                gameGraphics.drawBox(windowWidth / 2 - 100, 160, 200, 40, 0);
                gameGraphics.drawText("You are sleeping", windowWidth / 2, 50, 7, 0xffff00);
                gameGraphics.drawText("Fatigue: " + (fatigue * 100) / 750 + "%", windowWidth / 2, 90, 7, 0xffff00);
                gameGraphics.drawText("When you want to wake up just use your", windowWidth / 2, 140, 5, 0xffffff);
                gameGraphics.drawText("keyboard to type the word in the box below", windowWidth / 2, 160, 5, 0xffffff);
                gameGraphics.drawText(inputText + "*", windowWidth / 2, 180, 5, 65535);
                if (sleepingStatusText == null)
                {
                    gameGraphics.drawPixels(captchaPixels, windowWidth / 2 - 127, 230, captchaWidth, captchaHeight);
                }
                else
                {
                    gameGraphics.drawText(sleepingStatusText, windowWidth / 2, 260, 5, 0xff0000);
                }

                gameGraphics.drawBoxEdge(windowWidth / 2 - 128, 229, 257, 42, 0xffffff);
                drawChatMessageTabs();
                gameGraphics.drawText("If you can't read the word", windowWidth / 2, 290, 1, 0xffffff);
                gameGraphics.drawText("@yel@click here@whi@ to get a different one", windowWidth / 2, 305, 1, 0xffffff);

                //gameGraphics.UpdateGameImage();
                OnDrawDone();//gameGraphics.drawImage(spriteBatch, 0, 0);
                return;
            }
            if (!engineHandle.playerIsAlive)
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
                if (lastLayerIndex == 0 && (engineHandle.tiles[CurrentPlayer.currentX / 128][CurrentPlayer.currentY / 128] & 0x80) == 0)
                {
                    if (ShowRoofs)
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
                for (int i1 = 0; i1 < ObjectCount; i1++)
                {
                    if (ObjectType[i1] == 97)
                    {
                        drawModel(i1, "firea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[i1] == 274)
                    {
                        drawModel(i1, "fireplacea" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[i1] == 1031)
                    {
                        drawModel(i1, "lightning" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[i1] == 1036)
                    {
                        drawModel(i1, "firespell" + (modelFireLightningSpellNumber + 1));
                    }

                    if (ObjectType[i1] == 1147)
                    {
                        drawModel(i1, "spellcharge" + (modelFireLightningSpellNumber + 1));
                    }
                }

            }
            if (modelTorchNumber != lastModelTorchNumber)
            {
                lastModelTorchNumber = modelTorchNumber;
                for (int j1 = 0; j1 < ObjectCount; j1++)
                {
                    if (ObjectType[j1] == 51)
                    {
                        drawModel(j1, "torcha" + (modelTorchNumber + 1));
                    }

                    if (ObjectType[j1] == 143)
                    {
                        drawModel(j1, "skulltorcha" + (modelTorchNumber + 1));
                    }
                }

            }
            if (modelClawSpellNumber != lastModelClawSpellNumber)
            {
                lastModelClawSpellNumber = modelClawSpellNumber;
                for (int k1 = 0; k1 < ObjectCount; k1++)
                {
                    if (ObjectType[k1] == 1142)
                    {
                        drawModel(k1, "clawspell" + (modelClawSpellNumber + 1));
                    }
                }
            }
            gameCamera.removeLastUpdates(drawUpdatesPerformed);
            drawUpdatesPerformed = 0;
            for (int l1 = 0; l1 < PlayerCount; l1++)
            {
                Mob player = Players[l1];
                if (player.BottomColour != 255)
                {
                    int j2 = player.currentX;
                    int l2 = player.currentY;
                    int j3 = -engineHandle.getAveragedElevation(j2, l2);
                    int k4 = gameCamera.addSpriteToScene(5000 + l1, j2, j3, l2, 145, 220, l1 + 10000);
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

            for (int i2 = 0; i2 < PlayerCount; i2++)
            {
                Mob player = Players[i2];
                if (player.ProjectileDistance > 0)
                {
                    Mob targetMob = null;
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
                        int k3 = player.currentX;
                        int l4 = player.currentY;
                        int k7 = -engineHandle.getAveragedElevation(k3, l4) - 110;
                        int k9 = targetMob.currentX;
                        int j10 = targetMob.currentY;
                        int k10 = -engineHandle.getAveragedElevation(k9, j10) - EntityManager.GetNpc(targetMob.npcId).Camera2 / 2;
                        int l10 = (k3 * player.ProjectileDistance + k9 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;
                        int i11 = (k7 * player.ProjectileDistance + k10 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;
                        int j11 = (l4 * player.ProjectileDistance + j10 * (ProjectileRange - player.ProjectileDistance)) / ProjectileRange;
                        gameCamera.addSpriteToScene(baseProjectilePic + player.ProjectileType, l10, i11, j11, 32, 32, 0);
                        drawUpdatesPerformed++;
                    }
                }
            }

            for (int k2 = 0; k2 < NpcCount; k2++)
            {
                Mob npc = Npcs[k2];
                int x1 = npc.currentX;
                int z1 = npc.currentY;
                int y1 = -engineHandle.getAveragedElevation(x1, z1);
                int l9 = gameCamera.addSpriteToScene(20000 + k2, x1, y1, z1, EntityManager.GetNpc(npc.npcId).Camera1, EntityManager.GetNpc(npc.npcId).Camera2, k2 + 30000);
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

            for (int i3 = 0; i3 < GroundItemCount; i3++)
            {
                int x = GroundItemX[i3] * GridSize + 64;
                int y = GroundItemY[i3] * GridSize + 64;
                gameCamera.addSpriteToScene(40000 + GroundItemId[i3], x, -engineHandle.getAveragedElevation(x, y) - GroundItemObjectVar[i3], y, 96, 64, i3 + 20000);
                drawUpdatesPerformed++;
            }

            for (int j4 = 0; j4 < teleBubbleCount; j4++)
            {
                int k5 = teleBubbleX[j4] * GridSize + 64;
                int i8 = teleBubbleY[j4] * GridSize + 64;
                int i10 = teleBubbleType[j4];
                if (i10 == 0)
                {
                    gameCamera.addSpriteToScene(50000 + j4, k5, -engineHandle.getAveragedElevation(k5, i8), i8, 128, 256, j4 + 50000);
                    drawUpdatesPerformed++;
                }
                if (i10 == 1)
                {
                    gameCamera.addSpriteToScene(50000 + j4, k5, -engineHandle.getAveragedElevation(k5, i8), i8, 128, 64, j4 + 50000);
                    drawUpdatesPerformed++;
                }
            }

            gameGraphics.interlacingEnabled = false;
            gameGraphics.ClearScreen();
            gameGraphics.interlacingEnabled = SettingsManager.Instance.GraphicsSettings.Interlacing;

            if (lastLayerIndex == 3)
            {
                int l5 = 40 + (int)(Helper.Random.NextDouble() * 3D);
                int j8 = 40 + (int)(Helper.Random.NextDouble() * 7D);
                gameCamera.bjl(l5, j8, -50, -10, -50);
            }

            itemsAboveHeadCount = 0;
            receivedMessagesCount = 0;
            healthBarVisibleCount = 0;

            if (cameraAutoAngleDebug)
            {
                if (CameraAutoAngle && !cameraZoom)
                {
                    int i6 = cameraAutoAngle;
                    autoRotateCamera();
                    if (cameraAutoAngle != i6)
                    {
                        cameraAutoRotatePlayerX = CurrentPlayer.currentX;
                        cameraAutoRotatePlayerY = CurrentPlayer.currentY;
                    }
                }
                if (fogOfWar)
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
                cameraRotation = cameraAutoAngle * 32;
                int newCameraPosX = cameraAutoRotatePlayerX + cameraRotationXAmount;
                int newCameraPosY = cameraAutoRotatePlayerY + cameraRotationYAmount;
                gameCamera.SetCameraTransform(newCameraPosX, -engineHandle.getAveragedElevation(newCameraPosX, newCameraPosY), newCameraPosY, 912, cameraRotation * 4, 0, 2000);
            }
            else
            {
                if (CameraAutoAngle && !cameraZoom)
                {
                    autoRotateCamera();
                }

                if (fogOfWar)
                {
                    if (!SettingsManager.Instance.GraphicsSettings.Interlacing)
                    {
                        gameCamera.zoom1 = 2400;
                        gameCamera.zoom2 = 2400;
                        gameCamera.zoom3 = 1;
                        gameCamera.zoom4 = 2300;
                    }
                    else
                    {
                        gameCamera.zoom1 = 2200;
                        gameCamera.zoom2 = 2200;
                        gameCamera.zoom3 = 1;
                        gameCamera.zoom4 = 2100;
                    }
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
                gameCamera.SetCameraTransform(k6, -engineHandle.getAveragedElevation(k6, l8), l8, 912, cameraRotation * 4, 0, cameraDistance * 2);
            }
            gameCamera.finishCamera();
            drawAboveHeadThings();
            if (actionPictureType > 0)
            {
                gameGraphics.drawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 14 + (24 - actionPictureType) / 6);
            }

            if (actionPictureType < 0)
            {
                gameGraphics.drawPicture(walkMouseX - 8, walkMouseY - 8, baseInventoryPic + 18 + (24 + actionPictureType) / 6);
            }

            if (SystemUpdateTimer != 0)
            {
                int seconds = SystemUpdateTimer / 50;
                int minutes = seconds / 60;
                seconds %= 60;
                if (seconds < 10)
                {
                    gameGraphics.drawText("System update in: " + minutes + ":0" + seconds, 256, windowHeight - 7, 1, 0xffff00);
                }
                else
                {
                    gameGraphics.drawText("System update in: " + minutes + ":" + seconds, 256, windowHeight - 7, 1, 0xffff00);
                }
            }

            if (WildernessModeTimer != 0)
            {
                DrawWildernessTypeAnnouncement();
            }

            if (PvpTournamentTimer != 0)
            {
                DrawPvpTournamentAnnouncement();
            }

            if (DropPartyTimer != 0)
            {
                DrawDropPartyAnnouncement();
            }

            if (!LoadArea)
            {
                int i7 = 2203 - (SectionY + WildY + AreaY);
                if (SectionX + WildX + AreaX >= 2640)
                {
                    i7 = -50;
                }

                if (i7 > 0)
                {
                    int j9 = 1 + i7 / 6;
                    gameGraphics.drawPicture(453, windowHeight - 56, baseInventoryPic + 13);
                    gameGraphics.drawText("Wilderness", 465, windowHeight - 20, 1, 0xffff00);
                    gameGraphics.drawText("Level: " + j9, 465, windowHeight - 7, 1, 0xffff00);
                    if (wildType == 0)
                    {
                        wildType = 2;
                    }
                }
                if (wildType == 0 && i7 > -10 && i7 <= 0)
                {
                    wildType = 1;
                }
            }
            if (messagesTab == 0)
            {
                for (int j7 = 0; j7 < 5; j7++)
                {
                    if (messagesTimeout[j7] > 0)
                    {
                        string s1 = messagesArray[j7];
                        gameGraphics.drawString(s1, 7, windowHeight - 18 - j7 * 12, 1, 0xffff00);
                    }
                }
            }
            chatInputMenu.disableInput(messagesHandleType2);
            chatInputMenu.disableInput(messagesHandleType5);
            chatInputMenu.disableInput(messagesHandleType6);
            if (messagesTab == 1)
            {
                chatInputMenu.enableInput(messagesHandleType2);
            }
            else if (messagesTab == 2)
            {
                chatInputMenu.enableInput(messagesHandleType5);
            }
            else if (messagesTab == 3)
            {
                chatInputMenu.enableInput(messagesHandleType6);
            }

            Menu.chatMenuTextHeightMod = 2;
            chatInputMenu.drawMenu();
            Menu.chatMenuTextHeightMod = 0;
            gameGraphics.drawPicture(gameGraphics.gameWidth - 3 - 197, 3, baseInventoryPic, 128);

#warning play with this! Create a new menu of choice :)


            drawMenus();

            gameGraphics.loggedIn = false;
            drawChatMessageTabs();


            string text = "Coordinates: ( " + (SectionX + AreaX) + "," + (SectionY + AreaY) + " ) Section: (" + SectionX + "," + SectionY + ") Area: (" + AreaX + "," + AreaY + ")";
            // Text shadow
            gameGraphics.drawString(text, 10 + 11, 10 + 11, 1, 0x000000);
            gameGraphics.drawString(text, 10 + 10, 10 + 10, 1, 0xffffff);

            //gameGraphics.UpdateGameImage();
            OnDrawDone();//gameGraphics.drawImage(spriteBatch, 0, 0);
        }

        void DrawWildernessTypeAnnouncement()
        {
            int i6 = WildernessModeTimer / 50;
            int j8 = i6 / 60;

            i6 %= 60;

            if (i6 < 10)
            {
                gameGraphics.drawText($"Wilderness type will change in: {j8}:0{i6}", 256, windowHeight - 7, 1, 0xFFFF00);
            }
            else
            {
                gameGraphics.drawText($"Wilderness type will change in: {j8}:{i6}", 256, windowHeight - 7, 1, 0xFFFF00);
            }
        }

        void DrawPvpTournamentAnnouncement()
        {
            int i6 = PvpTournamentTimer / 50;
            int j8 = i6 / 60;

            i6 %= 60;

            if (i6 < 10)
            {
                gameGraphics.drawText("Drop party starting in: " + j8 + ":0" + i6, 256, windowHeight - 7, 1, 0xFFFF00);
            }
            else
            {
                gameGraphics.drawText("Drop party starting in: " + j8 + ":" + i6, 256, windowHeight - 7, 1, 0xFFFF00);
            }
        }

        void DrawDropPartyAnnouncement()
        {
            int i6 = DropPartyTimer / 50;
            int j8 = i6 / 60;

            i6 %= 60;

            if (i6 < 10)
            {
                gameGraphics.drawText("Drop party starting in: " + j8 + ":0" + i6, 256, windowHeight - 7, 1, 0xFFFF00);
            }
            else
            {
                gameGraphics.drawText("Drop party starting in: " + j8 + ":" + i6, 256, windowHeight - 7, 1, 0xFFFF00);
            }
        }

        //  public bool DrawCustomMenus { get; set; }
        //    public event EventHandler OnDrawMenus;

        public void drawReportAbuseBox2()
        {
            if (enteredInputText.Length > 0)
            {
                string s1 = enteredInputText.Trim();
                inputText = "";
                enteredInputText = "";
                if (s1.Length > 0)
                {
                    long l1 = DataOperations.nameToHash(s1);
                    StreamClass.CreatePacket(7);
                    StreamClass.AddInt64(l1);
                    StreamClass.AddInt8(reportAbuseOptionSelected);
                    //base.streamClass.addByte(dia ? 1 : 0);
                    StreamClass.FormatPacket();
                }
                showAbuseBox = 0;
                return;
            }
            gameGraphics.drawBox(56, 130, 400, 100, 0);
            gameGraphics.drawBoxEdge(56, 130, 400, 100, 0xffffff);
            int l = 160;
            gameGraphics.drawText("Now type the name of the offending player, and press enter", 256, l, 1, 0xffff00);
            l += 18;
            gameGraphics.drawText("Name: " + inputText + "*", 256, l, 4, 0xffffff);
            l = 222;
            int i1 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > 196 && InputManager.Instance.MouseLocation.X < 316 && InputManager.Instance.MouseLocation.Y > l - 13 && InputManager.Instance.MouseLocation.Y < l + 2)
            {
                i1 = 0xffff00;
                if (mouseButtonClick == 1)
                {
                    mouseButtonClick = 0;
                    showAbuseBox = 0;
                }
            }
            gameGraphics.drawText("Click here to cancel", 256, l, 1, i1);
            if (mouseButtonClick == 1 && (InputManager.Instance.MouseLocation.X < 56 || InputManager.Instance.MouseLocation.X > 456 || InputManager.Instance.MouseLocation.Y < 130 || InputManager.Instance.MouseLocation.Y > 230))
            {
                mouseButtonClick = 0;
                showAbuseBox = 0;
            }
        }

        public void drawMenus()
        {
            if (logoutTimer != 0)
            {
                drawLogoutBox();
            }
            else if (ShowWelcomeBox)
            {
                drawWelcomeBox();
            }
            else if (showServerMessageBox)
            {
                drawServerMessageBox();
            }
            else if (wildType == 1)
            {
                drawWildernessAlertBox();
            }
            else if (ShowBankBox && combatTimeout == 0)
            {
                drawBankBox();
            }
            else if (ShowShopBox && combatTimeout == 0)
            {
                drawShopBox();
            }
            else if (showTradeConfirmBox)
            {
                drawTradeConfirmBox();
            }
            else if (showTradeBox)
            {
                drawTradeBox();
            }
            else if (ShowDuelConfirmBox)
            {
                drawDuelConfirmBox();
            }
            else if (ShowDuelBox)
            {
                drawDuelBox();
            }
            else if (showAbuseBox == 1)
            {
                drawReportAbuseBox1();
            }
            else if (showAbuseBox == 2)
            {
                drawReportAbuseBox2();
            }
            else if (showFriendsBox != 0)
            {
                drawFriendsBox();
            }
            else
            {
                if (ShowQuestionMenu)
                {
                    drawQuestionMenu();
                }

                if (ShowCombatWindow || CurrentPlayer.currentSprite == 8 || CurrentPlayer.currentSprite == 9)
                {
                    drawCombatStyleBox();
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

                if (drawMenuTab == 3)
                {
                    drawStatsQuestsMenu(flag);
                }

                if (drawMenuTab == 4)
                {
                    drawPrayerMagicMenu(flag);
                }

                if (drawMenuTab == 5)
                {
                    drawFriendsMenu(flag);
                }

                if (drawMenuTab == 6)
                {
                    drawOptionsMenu(flag);
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
            EntityManager.GetModelIndex("torcha2");
            EntityManager.GetModelIndex("torcha3");
            EntityManager.GetModelIndex("torcha4");
            EntityManager.GetModelIndex("skulltorcha2");
            EntityManager.GetModelIndex("skulltorcha3");
            EntityManager.GetModelIndex("skulltorcha4");
            EntityManager.GetModelIndex("firea2");
            EntityManager.GetModelIndex("firea3");
            EntityManager.GetModelIndex("fireplacea2");
            EntityManager.GetModelIndex("fireplacea3");
            EntityManager.GetModelIndex("firespell2");
            EntityManager.GetModelIndex("firespell3");
            EntityManager.GetModelIndex("lightning2");
            EntityManager.GetModelIndex("lightning3");
            EntityManager.GetModelIndex("clawspell2");
            EntityManager.GetModelIndex("clawspell3");
            EntityManager.GetModelIndex("clawspell4");
            EntityManager.GetModelIndex("clawspell5");
            EntityManager.GetModelIndex("spellcharge2");
            EntityManager.GetModelIndex("spellcharge3");

            sbyte[] models = unpackData("models.jag", "3d models", 60);

            if (models == null)
            {
                errorLoading = true;
                return;
            }

            for (int i1 = 0; i1 < EntityManager.ObjectModelCount; i1++)
            {
                try
                {
                    long j1 = DataOperations.getObjectOffset(EntityManager.GetObjectModelName(i1) + ".ob3", models);

                    if (j1 != 0)
                    {
                        GameDataObjects[i1] = new ObjectModel(models, (int)j1, true);
                    }
                    else
                    {
                        GameDataObjects[i1] = new ObjectModel(1, 1);
                    }

                    if (EntityManager.GetObjectModelName(i1).Equals("giantcrystal"))
                    {
                        GameDataObjects[i1].isGiantCrystal = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                    Console.WriteLine(ex);
                }
            }
        }

        public void drawDuelBox()
        {
            if (mouseButtonClick != 0 && mouseClickedHeldInTradeDuelBox == 0)
            {
                mouseClickedHeldInTradeDuelBox = 1;
            }

            if (mouseClickedHeldInTradeDuelBox > 0)
            {
                int l = InputManager.Instance.MouseLocation.X - 22;
                int i1 = InputManager.Instance.MouseLocation.Y - 36;
                if (l >= 0 && i1 >= 0 && l < 468 && i1 < 262)
                {
                    if (l > 216 && i1 > 30 && l < 462 && i1 < 235)
                    {
                        int j1 = (l - 217) / 49 + ((i1 - 31) / 34) * 5;
                        if (j1 >= 0 && j1 < InventoryItemsCount)
                        {
                            bool flag1 = false;
                            int k2 = 0;
                            int j3 = InventoryItems[j1];
                            for (int j4 = 0; j4 < duelMyItemCount; j4++)
                            {
                                if (duelMyItems[j4] == j3)
                                {
                                    if (EntityManager.GetItem(j3).IsStackable == 0)
                                    {
                                        for (int l4 = 0; l4 < mouseClickedHeldInTradeDuelBox; l4++)
                                        {
                                            if (duelMyItemsCount[j4] < InventoryItemCount[j1])
                                            {
                                                duelMyItemsCount[j4]++;
                                            }

                                            flag1 = true;
                                        }

                                    }
                                    else
                                    {
                                        k2++;
                                    }
                                }
                            }

                            if (getInventoryItemTotalCount(j3) <= k2)
                            {
                                flag1 = true;
                            }

                            if (EntityManager.GetItem(j3).IsSpecial == 1)
                            {
                                displayMessage("This object cannot be added to a duel offer", 3);
                                flag1 = true;
                            }
                            if (!flag1 && duelMyItemCount < 8)
                            {
                                duelMyItems[duelMyItemCount] = j3;
                                duelMyItemsCount[duelMyItemCount] = 1;
                                duelMyItemCount++;
                                flag1 = true;
                            }
                            if (flag1)
                            {
                                StreamClass.CreatePacket(123);
                                StreamClass.AddInt8(duelMyItemCount);
                                for (int i5 = 0; i5 < duelMyItemCount; i5++)
                                {
                                    StreamClass.AddInt16(duelMyItems[i5]);
                                    StreamClass.AddInt32(duelMyItemsCount[i5]);
                                }

                                StreamClass.FormatPacket();
                                duelOpponentAccepted = false;
                                duelMyAccepted = false;
                            }
                        }
                    }
                    if (l > 8 && i1 > 30 && l < 205 && i1 < 129)
                    {
                        int k1 = (l - 9) / 49 + ((i1 - 31) / 34) * 4;
                        if (k1 >= 0 && k1 < duelMyItemCount)
                        {
                            int i2 = duelMyItems[k1];
                            for (int l2 = 0; l2 < mouseClickedHeldInTradeDuelBox; l2++)
                            {
                                if (EntityManager.GetItem(i2).IsStackable == 0 && duelMyItemsCount[k1] > 1)
                                {
                                    duelMyItemsCount[k1]--;
                                    continue;
                                }
                                duelMyItemCount--;
                                mouseButtonHeldTime = 0;
                                for (int k3 = k1; k3 < duelMyItemCount; k3++)
                                {
                                    duelMyItems[k3] = duelMyItems[k3 + 1];
                                    duelMyItemsCount[k3] = duelMyItemsCount[k3 + 1];
                                }

                                break;
                            }

                            StreamClass.CreatePacket(123);
                            StreamClass.AddInt8(duelMyItemCount);
                            for (int l3 = 0; l3 < duelMyItemCount; l3++)
                            {
                                StreamClass.AddInt16(duelMyItems[l3]);
                                StreamClass.AddInt32(duelMyItemsCount[l3]);
                            }

                            StreamClass.FormatPacket();
                            duelOpponentAccepted = false;
                            duelMyAccepted = false;
                        }
                    }
                    bool flag = false;
                    if (l >= 93 && i1 >= 221 && l <= 104 && i1 <= 232)
                    {
                        duelNoRetreating = !duelNoRetreating;
                        flag = true;
                    }
                    if (l >= 93 && i1 >= 240 && l <= 104 && i1 <= 251)
                    {
                        duelNoMagic = !duelNoMagic;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 221 && l <= 202 && i1 <= 232)
                    {
                        duelNoPrayer = !duelNoPrayer;
                        flag = true;
                    }
                    if (l >= 191 && i1 >= 240 && l <= 202 && i1 <= 251)
                    {
                        duelNoWeapons = !duelNoWeapons;
                        flag = true;
                    }
                    if (flag)
                    {
                        StreamClass.CreatePacket(225);
                        StreamClass.AddInt8(duelNoRetreating ? 1 : 0);
                        StreamClass.AddInt8(duelNoMagic ? 1 : 0);
                        StreamClass.AddInt8(duelNoPrayer ? 1 : 0);
                        StreamClass.AddInt8(duelNoWeapons ? 1 : 0);
                        StreamClass.FormatPacket();
                        duelOpponentAccepted = false;
                        duelMyAccepted = false;
                    }
                    if (l >= 217 && i1 >= 238 && l <= 286 && i1 <= 259)
                    {
                        duelMyAccepted = true;
                        StreamClass.CreatePacket(252);
                        StreamClass.FormatPacket();
                    }
                    if (l >= 394 && i1 >= 238 && l < 463 && i1 < 259)
                    {
                        ShowDuelBox = false;
                        StreamClass.CreatePacket(35);
                        StreamClass.FormatPacket();
                    }
                }
                else
                    if (mouseButtonClick != 0)
                {
                    ShowDuelBox = false;
                    StreamClass.CreatePacket(35);
                    StreamClass.FormatPacket();
                }
                mouseButtonClick = 0;
                mouseClickedHeldInTradeDuelBox = 0;
            }
            if (!ShowDuelBox)
            {
                return;
            }

            sbyte byte0 = 22;
            sbyte byte1 = 36;
            gameGraphics.drawBox(byte0, byte1, 468, 12, 0xc90b1d);
            int l1 = 0x989898;
            gameGraphics.drawBoxAlpha(byte0, byte1 + 12, 468, 18, l1, 160);
            gameGraphics.drawBoxAlpha(byte0, byte1 + 30, 8, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 205, byte1 + 30, 11, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 462, byte1 + 30, 6, 248, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 99, 197, 24, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 192, 197, 23, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 258, 197, 20, l1, 160);
            gameGraphics.drawBoxAlpha(byte0 + 216, byte1 + 235, 246, 43, l1, 160);
            int j2 = 0xd0d0d0;
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 30, 197, 69, j2, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 123, 197, 69, j2, 160);
            gameGraphics.drawBoxAlpha(byte0 + 8, byte1 + 215, 197, 43, j2, 160);
            gameGraphics.drawBoxAlpha(byte0 + 216, byte1 + 30, 246, 205, j2, 160);
            for (int i3 = 0; i3 < 3; i3++)
            {
                gameGraphics.drawLineX(byte0 + 8, byte1 + 30 + i3 * 34, 197, 0);
            }

            for (int i4 = 0; i4 < 3; i4++)
            {
                gameGraphics.drawLineX(byte0 + 8, byte1 + 123 + i4 * 34, 197, 0);
            }

            for (int k4 = 0; k4 < 7; k4++)
            {
                gameGraphics.drawLineX(byte0 + 216, byte1 + 30 + k4 * 34, 246, 0);
            }

            for (int j5 = 0; j5 < 6; j5++)
            {
                if (j5 < 5)
                {
                    gameGraphics.drawLineY(byte0 + 8 + j5 * 49, byte1 + 30, 69, 0);
                }

                if (j5 < 5)
                {
                    gameGraphics.drawLineY(byte0 + 8 + j5 * 49, byte1 + 123, 69, 0);
                }

                gameGraphics.drawLineY(byte0 + 216 + j5 * 49, byte1 + 30, 205, 0);
            }

            gameGraphics.drawLineX(byte0 + 8, byte1 + 215, 197, 0);
            gameGraphics.drawLineX(byte0 + 8, byte1 + 257, 197, 0);
            gameGraphics.drawLineY(byte0 + 8, byte1 + 215, 43, 0);
            gameGraphics.drawLineY(byte0 + 204, byte1 + 215, 43, 0);
            gameGraphics.drawString("Preparing to duel with: " + duelOpponent, byte0 + 1, byte1 + 10, 1, 0xffffff);
            gameGraphics.drawString("Your Stake", byte0 + 9, byte1 + 27, 4, 0xffffff);
            gameGraphics.drawString("Opponent's Stake", byte0 + 9, byte1 + 120, 4, 0xffffff);
            gameGraphics.drawString("Duel Options", byte0 + 9, byte1 + 212, 4, 0xffffff);
            gameGraphics.drawString("Your Inventory", byte0 + 216, byte1 + 27, 4, 0xffffff);
            gameGraphics.drawString("No retreating", byte0 + 8 + 1, byte1 + 215 + 16, 3, 0xffff00);
            gameGraphics.drawString("No magic", byte0 + 8 + 1, byte1 + 215 + 35, 3, 0xffff00);
            gameGraphics.drawString("No prayer", byte0 + 8 + 102, byte1 + 215 + 16, 3, 0xffff00);
            gameGraphics.drawString("No weapons", byte0 + 8 + 102, byte1 + 215 + 35, 3, 0xffff00);
            gameGraphics.drawBoxEdge(byte0 + 93, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (duelNoRetreating)
            {
                gameGraphics.drawBox(byte0 + 95, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            gameGraphics.drawBoxEdge(byte0 + 93, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (duelNoMagic)
            {
                gameGraphics.drawBox(byte0 + 95, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            gameGraphics.drawBoxEdge(byte0 + 191, byte1 + 215 + 6, 11, 11, 0xffff00);
            if (duelNoPrayer)
            {
                gameGraphics.drawBox(byte0 + 193, byte1 + 215 + 8, 7, 7, 0xffff00);
            }

            gameGraphics.drawBoxEdge(byte0 + 191, byte1 + 215 + 25, 11, 11, 0xffff00);
            if (duelNoWeapons)
            {
                gameGraphics.drawBox(byte0 + 193, byte1 + 215 + 27, 7, 7, 0xffff00);
            }

            if (!duelMyAccepted)
            {
                gameGraphics.drawPicture(byte0 + 217, byte1 + 238, baseInventoryPic + 25);
            }

            gameGraphics.drawPicture(byte0 + 394, byte1 + 238, baseInventoryPic + 26);
            if (duelOpponentAccepted)
            {
                gameGraphics.drawText("Other player", byte0 + 341, byte1 + 246, 1, 0xffffff);
                gameGraphics.drawText("has accepted", byte0 + 341, byte1 + 256, 1, 0xffffff);
            }
            if (duelMyAccepted)
            {
                gameGraphics.drawText("Waiting for", byte0 + 217 + 35, byte1 + 246, 1, 0xffffff);
                gameGraphics.drawText("other player", byte0 + 217 + 35, byte1 + 256, 1, 0xffffff);
            }
            for (int k5 = 0; k5 < InventoryItemsCount; k5++)
            {
                int l5 = 217 + byte0 + (k5 % 5) * 49;
                int j6 = 31 + byte1 + (k5 / 5) * 34;
                gameGraphics.drawImage(l5, j6, 48, 32, baseItemPicture + EntityManager.GetItem(InventoryItems[k5]).InventoryPicture, EntityManager.GetItem(InventoryItems[k5]).PictureMask, 0, 0, false);
                if (EntityManager.GetItem(InventoryItems[k5]).IsStackable == 0)
                {
                    gameGraphics.drawString(InventoryItemCount[k5].ToString(), l5 + 1, j6 + 10, 1, 0xffff00);
                }
            }

            for (int i6 = 0; i6 < duelMyItemCount; i6++)
            {
                int k6 = 9 + byte0 + (i6 % 4) * 49;
                int i7 = 31 + byte1 + (i6 / 4) * 34;
                gameGraphics.drawImage(k6, i7, 48, 32, baseItemPicture + EntityManager.GetItem(duelMyItems[i6]).InventoryPicture, EntityManager.GetItem(duelMyItems[i6]).PictureMask, 0, 0, false);
                if (EntityManager.GetItem(duelMyItems[i6]).IsStackable == 0)
                {
                    gameGraphics.drawString(duelMyItemsCount[i6].ToString(), k6 + 1, i7 + 10, 1, 0xffff00);
                }

                if (InputManager.Instance.MouseLocation.X > k6 && InputManager.Instance.MouseLocation.X < k6 + 48 && InputManager.Instance.MouseLocation.Y > i7 && InputManager.Instance.MouseLocation.Y < i7 + 32)
                {
                    gameGraphics.drawString(EntityManager.GetItem(duelMyItems[i6]).Name + ": @whi@" + EntityManager.GetItem(duelMyItems[i6]).Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

            for (int l6 = 0; l6 < duelOpponentItemCount; l6++)
            {
                int j7 = 9 + byte0 + (l6 % 4) * 49;
                int k7 = 124 + byte1 + (l6 / 4) * 34;
                gameGraphics.drawImage(j7, k7, 48, 32, baseItemPicture + EntityManager.GetItem(duelMyItems[l6]).InventoryPicture, EntityManager.GetItem(duelMyItems[l6]).PictureMask, 0, 0, false);

                if (EntityManager.GetItem(duelMyItems[l6]).IsStackable == 0)
                {
                    gameGraphics.drawString(duelOpponentItemsCount[l6].ToString(), j7 + 1, k7 + 10, 1, 0xffff00);
                }

                if (InputManager.Instance.MouseLocation.X > j7 && InputManager.Instance.MouseLocation.X < j7 + 48 && InputManager.Instance.MouseLocation.Y > k7 && InputManager.Instance.MouseLocation.Y < k7 + 32)
                {
                    gameGraphics.drawString(EntityManager.GetItem(duelMyItems[l6]).Name + ": @whi@" + EntityManager.GetItem(duelMyItems[l6]).Description, byte0 + 8, byte1 + 273, 1, 0xffff00);
                }
            }

        }

        public void drawWildernessAlertBox()
        {
            int l = 97;
            gameGraphics.drawBox(86, 77, 340, 180, 0);
            gameGraphics.drawBoxEdge(86, 77, 340, 180, 0xffffff);
            gameGraphics.drawText("Warning! Proceed with caution", 256, l, 4, 0xff0000);
            l += 26;
            gameGraphics.drawText("If you go much further north you will enter the", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.drawText("wilderness. This a very dangerous area where", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.drawText("other players can attack you!", 256, l, 1, 0xffffff);
            l += 22;
            gameGraphics.drawText("The further north you go the more dangerous it", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.drawText("becomes, but the more treasure you will find.", 256, l, 1, 0xffffff);
            l += 22;
            gameGraphics.drawText("In the wilderness an indicator at the bottom-right", 256, l, 1, 0xffffff);
            l += 13;
            gameGraphics.drawText("of the screen will show the current level of danger", 256, l, 1, 0xffffff);
            l += 22;
            int i1 = 0xffffff;

            if (InputManager.Instance.MouseLocation.Y > l - 12 && InputManager.Instance.MouseLocation.Y <= l && InputManager.Instance.MouseLocation.X > 181 && InputManager.Instance.MouseLocation.X < 331)
            {
                i1 = 0xff0000;
            }

            gameGraphics.drawText("Click here to close window", 256, l, 1, i1);

            if (mouseButtonClick != 0)
            {
                if (InputManager.Instance.MouseLocation.Y > l - 12 && InputManager.Instance.MouseLocation.Y <= l && InputManager.Instance.MouseLocation.X > 181 && InputManager.Instance.MouseLocation.X < 331)
                {
                    wildType = 2;
                }

                if (InputManager.Instance.MouseLocation.X < 86 || InputManager.Instance.MouseLocation.X > 426 || InputManager.Instance.MouseLocation.Y < 77 || InputManager.Instance.MouseLocation.Y > 257)
                {
                    wildType = 2;
                }

                mouseButtonClick = 0;
            }
        }

        public void DrawNpc(int x, int y, int width, int height, int index, int unknown1, int unknown2)
        {
            Mob npc = Npcs[index];
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
            int j1 = newFrameIndex * 3 + walkModel[(npc.stepCount / EntityManager.GetNpc(npc.npcId).WalkModel) % 4];
            if (npc.currentSprite == 8)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = false;
                x -= (EntityManager.GetNpc(npc.npcId).CombatSprite * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray1[(tick / (EntityManager.GetNpc(npc.npcId).CombatModel - 1)) % 8];
            }
            else
                if (npc.currentSprite == 9)
            {
                newFrameIndex = 5;
                frameIndex = 2;
                flag = true;
                x += (EntityManager.GetNpc(npc.npcId).CombatSprite * unknown2) / 100;
                j1 = newFrameIndex * 3 + combatModelArray2[(tick / EntityManager.GetNpc(npc.npcId).CombatModel) % 8];
            }

            for (int k1 = 0; k1 < 12; k1++)
            {
                int l1 = animationModelArray[frameIndex][k1];
                int k2 = EntityManager.GetNpc(npc.npcId).Sprites[l1];

                if (k2 >= 0)
                {
                    int i3 = 0;
                    int j3 = 0;
                    int k3 = j1;

                    if (flag && newFrameIndex >= 1 && newFrameIndex <= 3 && EntityManager.GetAnimation(k2).HasF == 1)
                    {
                        k3 += 15;
                    }

                    if (newFrameIndex != 5 || EntityManager.GetAnimation(k2).HasA == 1)
                    {
                        int l3 = k3 + EntityManager.GetAnimation(k2).Number;
                        i3 = (i3 * width) / gameGraphics.pictureAssumedWidth[l3];
                        j3 = (j3 * height) / gameGraphics.pictureAssumedHeight[l3];
                        int i4 = (width * gameGraphics.pictureAssumedWidth[l3]) / gameGraphics.pictureAssumedWidth[EntityManager.GetAnimation(k2).Number];
                        i3 -= (i4 - width) / 2;
                        int j4 = EntityManager.GetAnimation(k2).CharacterColour;
                        int k4 = 0;

                        if (j4 == 1)
                        {
                            j4 = EntityManager.GetNpc(npc.npcId).HairColour;
                            k4 = EntityManager.GetNpc(npc.npcId).SkinColour;
                        }
                        else if (j4 == 2)
                        {
                            j4 = EntityManager.GetNpc(npc.npcId).TopColour;
                            k4 = EntityManager.GetNpc(npc.npcId).SkinColour;
                        }
                        else if (j4 == 3)
                        {
                            j4 = EntityManager.GetNpc(npc.npcId).BottomColour;
                            k4 = EntityManager.GetNpc(npc.npcId).SkinColour;
                        }

                        gameGraphics.drawImage(x + i3, y + j3, i4, height, l3, j4, k4, unknown1, flag);
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
                    else
                        if (npc.currentSprite == 9)
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

                    gameGraphics.drawPicture((j2 + width / 2) - 12, (y + height / 2) - 12, baseInventoryPic + 12);
                    gameGraphics.drawText(npc.LastDamageCount.ToString(), (j2 + width / 2) - 1, y + height / 2 + 5, 3, 0xffffff);
                }
            }
        }

        public override void displayMessage(string s1)
        {
            if (s1.StartsWith("@bor@"))
            {
                displayMessage(s1, 4);
                return;
            }
            if (s1.StartsWith("@que@"))
            {
                displayMessage("@whi@" + s1, 5);
                return;
            }
            if (s1.StartsWith("@pri@"))
            {
                displayMessage(s1, 6);
                return;
            }
            else
            {
                displayMessage(s1, 3);
                return;
            }
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
                gameGraphics.drawFloatingText(receivedMessages[l], x, y, 1, 0xffff00, 300);
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
                gameGraphics.drawTransparentImage(x - width / 2, j5, width, height, baseInventoryPic + 9, 85);
                int k5 = (36 * scale) / 100;
                int l5 = (24 * scale) / 100;
                gameGraphics.drawImage(x - k5 / 2, (j5 + height / 2) - l5 / 2, k5, l5, EntityManager.GetItem(id).InventoryPicture + baseItemPicture, EntityManager.GetItem(id).PictureMask, 0, 0, false);
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

        public override void cantLogout()
        {
            logoutTimer = 0;
            displayMessage("@cya@Sorry, you can't logout at the moment", 3);
        }

        public void drawBankBox()
        {
            char c1 = '\u0198';
            char c2 = '\u014E';
            if (bankPage > 0 && bankItemsCount <= 48)
            {
                bankPage = 0;
            }

            if (bankPage > 1 && bankItemsCount <= 96)
            {
                bankPage = 1;
            }

            if (bankPage > 2 && bankItemsCount <= 144)
            {
                bankPage = 2;
            }

            if (selectedBankItem >= bankItemsCount || selectedBankItem < 0)
            {
                selectedBankItem = -1;
            }

            if (selectedBankItem != -1 && bankItems[selectedBankItem] != selectedBankItemType)
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
                    int l1 = bankPage * 48;
                    for (int k2 = 0; k2 < 6; k2++)
                    {
                        for (int i3 = 0; i3 < 8; i3++)
                        {
                            int k7 = 7 + i3 * 49;
                            int i8 = 28 + k2 * 34;
                            if (l > k7 && l < k7 + 49 && j1 > i8 && j1 < i8 + 34 && l1 < bankItemsCount && bankItems[l1] != -1)
                            {
                                selectedBankItemType = bankItems[l1];
                                selectedBankItem = l1;
                            }
                            l1++;
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
                        id = bankItems[selectedBankItem];
                    }

                    if (id != -1)
                    {
                        int count = bankItemCount[selectedBankItem];
                        if (EntityManager.GetItem(id).IsStackable == 1 && count > 1)
                        {
                            count = 1;
                        }

                        if (count >= 1 && InputManager.Instance.MouseLocation.X >= l + 220 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 250 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(1);
                            StreamClass.FormatPacket();
                        }
                        if (count >= 5 && InputManager.Instance.MouseLocation.X >= l + 250 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 280 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(5);
                            StreamClass.FormatPacket();
                        }
                        if (count >= 25 && InputManager.Instance.MouseLocation.X >= l + 280 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 305 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(25);
                            StreamClass.FormatPacket();
                        }
                        if (count >= 100 && InputManager.Instance.MouseLocation.X >= l + 305 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 335 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(100);
                            StreamClass.FormatPacket();
                        }
                        if (count >= 500 && InputManager.Instance.MouseLocation.X >= l + 335 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 368 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(500);
                            StreamClass.FormatPacket();
                        }
                        if (count >= 2500 && InputManager.Instance.MouseLocation.X >= l + 370 && InputManager.Instance.MouseLocation.Y >= j1 + 238 && InputManager.Instance.MouseLocation.X < l + 400 && InputManager.Instance.MouseLocation.Y <= j1 + 249)
                        {
                            StreamClass.CreatePacket(183);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(2500);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 1 && InputManager.Instance.MouseLocation.X >= l + 220 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 250 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(1);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 5 && InputManager.Instance.MouseLocation.X >= l + 250 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 280 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(5);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 25 && InputManager.Instance.MouseLocation.X >= l + 280 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 305 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(25);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 100 && InputManager.Instance.MouseLocation.X >= l + 305 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 335 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(100);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 500 && InputManager.Instance.MouseLocation.X >= l + 335 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 368 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(500);
                            StreamClass.FormatPacket();
                        }
                        if (getInventoryItemTotalCount(id) >= 2500 && InputManager.Instance.MouseLocation.X >= l + 370 && InputManager.Instance.MouseLocation.Y >= j1 + 263 && InputManager.Instance.MouseLocation.X < l + 400 && InputManager.Instance.MouseLocation.Y <= j1 + 274)
                        {
                            StreamClass.CreatePacket(198);
                            StreamClass.AddInt16(id);
                            StreamClass.AddInt32(2500);
                            StreamClass.FormatPacket();
                        }
                    }
                }
                else
                    if (bankItemsCount > 48 && l >= 50 && l <= 115 && j1 <= 12)
                {
                    bankPage = 0;
                }
                else
                        if (bankItemsCount > 48 && l >= 115 && l <= 180 && j1 <= 12)
                {
                    bankPage = 1;
                }
                else
                            if (bankItemsCount > 96 && l >= 180 && l <= 245 && j1 <= 12)
                {
                    bankPage = 2;
                }
                else
                                if (bankItemsCount > 144 && l >= 245 && l <= 310 && j1 <= 12)
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
            gameGraphics.drawBox(i1, k1, 408, 12, 192);
            int j2 = 0x989898;
            gameGraphics.drawBoxAlpha(i1, k1 + 12, 408, 17, j2, 160);
            gameGraphics.drawBoxAlpha(i1, k1 + 29, 8, 204, j2, 160);
            gameGraphics.drawBoxAlpha(i1 + 399, k1 + 29, 9, 204, j2, 160);
            gameGraphics.drawBoxAlpha(i1, k1 + 233, 408, 47, j2, 160);
            gameGraphics.drawString("Bank", i1 + 1, k1 + 10, 1, 0xffffff);
            int l2 = 50;
            if (bankItemsCount > 48)
            {
                int k3 = 0xffffff;
                if (bankPage == 0)
                {
                    k3 = 0xff0000;
                }
                else
                    if (InputManager.Instance.MouseLocation.X > i1 + l2 && InputManager.Instance.MouseLocation.Y >= k1 && InputManager.Instance.MouseLocation.X < i1 + l2 + 65 && InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.drawString("<page 1>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
                k3 = 0xffffff;
                if (bankPage == 1)
                {
                    k3 = 0xff0000;
                }
                else
                    if (InputManager.Instance.MouseLocation.X > i1 + l2 && InputManager.Instance.MouseLocation.Y >= k1 && InputManager.Instance.MouseLocation.X < i1 + l2 + 65 && InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    k3 = 0xffff00;
                }

                gameGraphics.drawString("<page 2>", i1 + l2, k1 + 10, 1, k3);
                l2 += 65;
            }
            if (bankItemsCount > 96)
            {
                int l3 = 0xffffff;
                if (bankPage == 2)
                {
                    l3 = 0xff0000;
                }
                else
                    if (InputManager.Instance.MouseLocation.X > i1 + l2 && InputManager.Instance.MouseLocation.Y >= k1 && InputManager.Instance.MouseLocation.X < i1 + l2 + 65 && InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    l3 = 0xffff00;
                }

                gameGraphics.drawString("<page 3>", i1 + l2, k1 + 10, 1, l3);
                l2 += 65;
            }
            if (bankItemsCount > 144)
            {
                int i4 = 0xffffff;
                if (bankPage == 3)
                {
                    i4 = 0xff0000;
                }
                else
                    if (InputManager.Instance.MouseLocation.X > i1 + l2 && InputManager.Instance.MouseLocation.Y >= k1 && InputManager.Instance.MouseLocation.X < i1 + l2 + 65 && InputManager.Instance.MouseLocation.Y < k1 + 12)
                {
                    i4 = 0xffff00;
                }

                gameGraphics.drawString("<page 4>", i1 + l2, k1 + 10, 1, i4);
                l2 += 65;
            }
            int j4 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > i1 + 320 && InputManager.Instance.MouseLocation.Y >= k1 && InputManager.Instance.MouseLocation.X < i1 + 408 && InputManager.Instance.MouseLocation.Y < k1 + 12)
            {
                j4 = 0xff0000;
            }

            gameGraphics.drawLabel("Close window", i1 + 406, k1 + 10, 1, j4);
            gameGraphics.drawString("Number in bank in green", i1 + 7, k1 + 24, 1, 65280);
            gameGraphics.drawString("Number held in blue", i1 + 289, k1 + 24, 1, 65535);
            int l7 = 0xd0d0d0;
            int j8 = bankPage * 48;
            for (int l8 = 0; l8 < 6; l8++)
            {
                for (int i9 = 0; i9 < 8; i9++)
                {
                    int k9 = i1 + 7 + i9 * 49;
                    int l9 = k1 + 28 + l8 * 34;
                    if (selectedBankItem == j8)
                    {
                        gameGraphics.drawBoxAlpha(k9, l9, 49, 34, 0xff0000, 160);
                    }
                    else
                    {
                        gameGraphics.drawBoxAlpha(k9, l9, 49, 34, l7, 160);
                    }

                    gameGraphics.drawBoxEdge(k9, l9, 50, 35, 0);
                    if (j8 < bankItemsCount && bankItems[j8] != -1)
                    {
                        gameGraphics.drawImage(k9, l9, 48, 32, baseItemPicture + EntityManager.GetItem(bankItems[j8]).InventoryPicture, EntityManager.GetItem(bankItems[j8]).PictureMask, 0, 0, false);
                        gameGraphics.drawString(bankItemCount[j8].ToString(), k9 + 1, l9 + 10, 1, 65280);
                        gameGraphics.drawLabel(getInventoryItemTotalCount(bankItems[j8]).ToString(), k9 + 47, l9 + 29, 1, 65535);
                    }
                    j8++;
                }

            }

            gameGraphics.drawLineX(i1 + 5, k1 + 256, 398, 0);
            if (selectedBankItem == -1)
            {
                gameGraphics.drawText("Select an object to withdraw or deposit", i1 + 204, k1 + 248, 3, 0xffff00);
                return;
            }
            int j9;
            if (selectedBankItem < 0)
            {
                j9 = -1;
            }
            else
            {
                j9 = bankItems[selectedBankItem];
            }

            if (j9 != -1)
            {
                int k8 = bankItemCount[selectedBankItem];

                if (EntityManager.GetItem(j9).IsStackable == 1 && k8 > 1)
                {
                    k8 = 1;
                }

                if (k8 > 0)
                {
                    gameGraphics.drawString("Withdraw " + EntityManager.GetItem(j9).Name, i1 + 2, k1 + 248, 1, 0xffffff);
                    int k4 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X >= i1 + 220 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 250 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                    {
                        k4 = 0xff0000;
                    }

                    gameGraphics.drawString("One", i1 + 222, k1 + 248, 1, k4);
                    if (k8 >= 5)
                    {
                        int l4 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 250 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 280 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            l4 = 0xff0000;
                        }

                        gameGraphics.drawString("Five", i1 + 252, k1 + 248, 1, l4);
                    }
                    if (k8 >= 25)
                    {
                        int i5 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 280 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 305 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            i5 = 0xff0000;
                        }

                        gameGraphics.drawString("25", i1 + 282, k1 + 248, 1, i5);
                    }
                    if (k8 >= 100)
                    {
                        int j5 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 305 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 335 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            j5 = 0xff0000;
                        }

                        gameGraphics.drawString("100", i1 + 307, k1 + 248, 1, j5);
                    }
                    if (k8 >= 500)
                    {
                        int k5 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 335 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 368 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            k5 = 0xff0000;
                        }

                        gameGraphics.drawString("500", i1 + 337, k1 + 248, 1, k5);
                    }
                    if (k8 >= 2500)
                    {
                        int l5 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 370 && InputManager.Instance.MouseLocation.Y >= k1 + 238 && InputManager.Instance.MouseLocation.X < i1 + 400 && InputManager.Instance.MouseLocation.Y <= k1 + 249)
                        {
                            l5 = 0xff0000;
                        }

                        gameGraphics.drawString("2500", i1 + 370, k1 + 248, 1, l5);
                    }
                }
                if (getInventoryItemTotalCount(j9) > 0)
                {
                    gameGraphics.drawString("Deposit " + EntityManager.GetItem(j9).Name, i1 + 2, k1 + 273, 1, 0xffffff);
                    int i6 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X >= i1 + 220 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 250 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                    {
                        i6 = 0xff0000;
                    }

                    gameGraphics.drawString("One", i1 + 222, k1 + 273, 1, i6);
                    if (getInventoryItemTotalCount(j9) >= 5)
                    {
                        int j6 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 250 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 280 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            j6 = 0xff0000;
                        }

                        gameGraphics.drawString("Five", i1 + 252, k1 + 273, 1, j6);
                    }
                    if (getInventoryItemTotalCount(j9) >= 25)
                    {
                        int k6 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 280 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 305 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            k6 = 0xff0000;
                        }

                        gameGraphics.drawString("25", i1 + 282, k1 + 273, 1, k6);
                    }
                    if (getInventoryItemTotalCount(j9) >= 100)
                    {
                        int l6 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 305 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 335 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            l6 = 0xff0000;
                        }

                        gameGraphics.drawString("100", i1 + 307, k1 + 273, 1, l6);
                    }
                    if (getInventoryItemTotalCount(j9) >= 500)
                    {
                        int i7 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 335 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 368 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            i7 = 0xff0000;
                        }

                        gameGraphics.drawString("500", i1 + 337, k1 + 273, 1, i7);
                    }
                    if (getInventoryItemTotalCount(j9) >= 2500)
                    {
                        int j7 = 0xffffff;
                        if (InputManager.Instance.MouseLocation.X >= i1 + 370 && InputManager.Instance.MouseLocation.Y >= k1 + 263 && InputManager.Instance.MouseLocation.X < i1 + 400 && InputManager.Instance.MouseLocation.Y <= k1 + 274)
                        {
                            j7 = 0xff0000;
                        }

                        gameGraphics.drawString("2500", i1 + 370, k1 + 273, 1, j7);
                    }
                }
            }
        }

        public GraphicsDevice getGraphics()
        {
            //if(GameApplet.gameFrame != null)
            //    return GameApplet.gameFrame.getGraphics();
            //if(link.gameApplet != null)
            //    return link.gameApplet.getGraphics();
            //else
            //    return base.getGraphics();
            return graphics;
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
            x += WildX;
            y += WildY;
            if (lastLayerIndex == LayerIndex && x > sectionWidth && x < sectionPosX && y > sectionHeight && y < sectionPosY)
            {
                engineHandle.playerIsAlive = true;
                return false;
            }
            OnLoadingSection?.Invoke(this, new EventArgs());
            gameGraphics.drawText("Loading... Please wait", 256, 192, 1, 0xffffff);
            drawChatMessageTabs();


            //gameGraphics.drawImage(spriteBatch, 0, 0);
            int l = AreaX;
            int i1 = AreaY;
            int xBase = (x + 24) / 48;
            int yBase = (y + 24) / 48;
            lastLayerIndex = LayerIndex;
            AreaX = xBase * 48 - 48;
            AreaY = yBase * 48 - 48;
            sectionWidth = xBase * 48 - 32;
            sectionHeight = yBase * 48 - 32;
            sectionPosX = xBase * 48 + 32;
            sectionPosY = yBase * 48 + 32;
            engineHandle.loadSection(x, y, lastLayerIndex);


            AreaX -= WildX;
            AreaY -= WildY;
            int offsetX = AreaX - l;
            int offsetY = AreaY - i1;
            for (int j2 = 0; j2 < ObjectCount; j2++)
            {
                ObjectX[j2] -= offsetX;
                ObjectY[j2] -= offsetY;
                int objX = ObjectX[j2];
                int objY = ObjectY[j2];
                int objType = ObjectType[j2];
                ObjectModel _obj = ObjectArray[j2];
                try
                {
                    int objDir = ObjectRotation[j2];
                    int objWidth;
                    int objHeight;
                    if (objDir == 0 || objDir == 4)
                    {
                        objWidth = EntityManager.GetWorldObject(objType).Width;
                        objHeight = EntityManager.GetWorldObject(objType).Height;
                    }
                    else
                    {
                        objHeight = EntityManager.GetWorldObject(objType).Width;
                        objWidth = EntityManager.GetWorldObject(objType).Height;
                    }
                    int flatObjX = ((objX + objX + objWidth) * GridSize) / 2;
                    int flatObjY = ((objY + objY + objHeight) * GridSize) / 2;
                    if (objX >= 0 && objY >= 0 && objX < 96 && objY < 96)
                    {
                        gameCamera.addModel(_obj);
                        _obj.setPosition(flatObjX, -engineHandle.getAveragedElevation(flatObjX, flatObjY), flatObjY);
                        engineHandle.createObject(objX, objY, objType, objDir);
                        if (objType == 74)
                        {
                            _obj.offsetPosition(0, -480, 0);
                        }
                    }
                }
                catch (Exception runtimeexception)
                {
                    Console.WriteLine("Loc Error: " + runtimeexception.ToString());
                    Console.WriteLine("x:" + j2 + " obj:" + _obj);
                    //runtimeexception.printStackTrace();
                }
            }


            for (int wallIndex = 0; wallIndex < WallObjectCount; wallIndex++)
            {
                WallObjectX[wallIndex] -= offsetX;
                WallObjectY[wallIndex] -= offsetY;
                int wallX = WallObjectX[wallIndex];
                int wallY = WallObjectY[wallIndex];
                int wallId = WallObjectId[wallIndex];
                int wallDir = WallObjectDirection[wallIndex];
                try
                {
                    engineHandle.createWall(wallX, wallY, wallDir, wallId);
                    ObjectModel wallObject = makeWallObject(wallX, wallY, wallDir, wallId, wallIndex);
                    WallObjects[wallIndex] = wallObject;
                }
                catch (Exception runtimeexception1)
                {
                    Console.WriteLine("Bound Error: " + runtimeexception1.ToString());
                    //runtimeexception1.printStackTrace();
                }
            }

            for (int k3 = 0; k3 < GroundItemCount; k3++)
            {
                GroundItemX[k3] -= offsetX;
                GroundItemY[k3] -= offsetY;
            }

            for (int j4 = 0; j4 < PlayerCount; j4++)
            {
                Mob f1 = Players[j4];
                f1.currentX -= offsetX * GridSize;
                f1.currentY -= offsetY * GridSize;
                for (int l5 = 0; l5 <= f1.WaypointCurrent; l5++)
                {
                    f1.WaypointsX[l5] -= offsetX * GridSize;
                    f1.WaypointsY[l5] -= offsetY * GridSize;
                }

            }

            for (int i5 = 0; i5 < NpcCount; i5++)
            {
                Mob f2 = Npcs[i5];
                f2.currentX -= offsetX * GridSize;
                f2.currentY -= offsetY * GridSize;
                for (int k6 = 0; k6 <= f2.WaypointCurrent; k6++)
                {
                    f2.WaypointsX[k6] -= offsetX * GridSize;
                    f2.WaypointsY[k6] -= offsetY * GridSize;
                }

            }

            engineHandle.playerIsAlive = true;
            OnLoadingSectionCompleted?.Invoke(this, new EventArgs());


            OnDrawDone();


            return true;
        }

        public static string formatItemCount(int arg0)
        {
            string s1 = arg0.ToString();
            for (int l = s1.Length - 3; l > 0; l -= 3)
            {
                s1 = s1.Substring(0, l) + "," + s1.Substring(l);
            }

            if (s1.Length > 8)
            {
                s1 = "@gre@" + s1.Substring(0, s1.Length - 8) + " million @whi@(" + s1 + ")";
            }
            else
                if (s1.Length > 4)
            {
                s1 = "@cya@" + s1.Substring(0, s1.Length - 4) + "K @whi@(" + s1 + ")";
            }

            return s1;
        }

        public bool hasRequiredRunes(int l, int i1)
        {
            if (l == 31 && (isItemEquipped(197) || isItemEquipped(615) || isItemEquipped(682)))
            {
                return true;
            }

            if (l == 32 && (isItemEquipped(102) || isItemEquipped(616) || isItemEquipped(683)))
            {
                return true;
            }

            if (l == 33 && (isItemEquipped(101) || isItemEquipped(617) || isItemEquipped(684)))
            {
                return true;
            }

            if (l == 34 && (isItemEquipped(103) || isItemEquipped(618) || isItemEquipped(685)))
            {
                return true;
            }

            return getInventoryItemTotalCount(l) >= i1;
        }

        public void displayMessage(string message, int type)
        {
            if (type == 2 || type == 4 || type == 6)
            {
                for (; message.Length > 5 && message[0] == '@' && message[4] == '@'; message = message.Substring(5))
                {
                    ;
                }

                int l = message.IndexOf(":");
                if (l != -1)
                {
                    string s1 = message.Substring(0, l);
                    long l1 = DataOperations.nameToHash(s1);
                    for (int j1 = 0; j1 < ignoresCount; j1++)
                    {
                        if (ignoresList[j1] == l1)
                        {
                            return;
                        }
                    }
                }
            }
            if (type == 2)
            {
                message = "@yel@" + message;
            }

            if (type == 3 || type == 4)
            {
                message = "@whi@" + message;
            }

            if (type == 6)
            {
                message = "@cya@" + message;
            }

            if (messagesTab != 0)
            {
                if (type == 4 || type == 3)
                {
                    chatTabAllMsgFlash = 200;
                }

                if (type == 2 && messagesTab != 1)
                {
                    chatTabHistoryFlash = 200;
                }

                if (type == 5 && messagesTab != 2)
                {
                    chatTabQuestFlash = 200;
                }

                if (type == 6 && messagesTab != 3)
                {
                    chatTabPrivateFlash = 200;
                }

                if (type == 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }

                if (type == 6 && messagesTab != 3 && messagesTab != 0)
                {
                    messagesTab = 0;
                }
            }
            for (int i1 = 4; i1 > 0; i1--)
            {
                messagesArray[i1] = messagesArray[i1 - 1];
                messagesTimeout[i1] = messagesTimeout[i1 - 1];
            }

            messagesArray[0] = message;
            messagesTimeout[0] = 300;
            if (type == 2)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType2] == chatInputMenu.listLength[messagesHandleType2] - 4)
                {
                    chatInputMenu.addMessage(messagesHandleType2, message, true);
                }
                else
                {
                    chatInputMenu.addMessage(messagesHandleType2, message, false);
                }
            }

            if (type == 5)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType5] == chatInputMenu.listLength[messagesHandleType5] - 4)
                {
                    chatInputMenu.addMessage(messagesHandleType5, message, true);
                }
                else
                {
                    chatInputMenu.addMessage(messagesHandleType5, message, false);
                }
            }

            if (type == 6)
            {
                if (chatInputMenu.listShownEntries[messagesHandleType6] == chatInputMenu.listLength[messagesHandleType6] - 4)
                {
                    chatInputMenu.addMessage(messagesHandleType6, message, true);
                    return;
                }
                chatInputMenu.addMessage(messagesHandleType6, message, false);
            }
        }

        /*
        void displayMessage(String message, int type, int status)
        {
            if (type == 2 || type == 4 || type == 6)
            {
                for (; message.Length > 5 && message[0] == '@' && message[4] == '@'; message = message.Substring(5))
                    ;
            }

            message = message.Replace("\\#pmd\\#", "");
            message = message.Replace("\\#mod\\#", "");
            message = message.Replace("\\#adm\\#", "");
            message = message.Replace("\\#evt\\#", "");
            message = message.Replace("\\#dev\\#", "");
            message = message.Replace("\\#orn\\#", "");

            switch (type)
            {
                case 2:
                    message = "@yel@" + message;
                    break;

                case 3:
                case 4:
                    message = "@whi@" + message;
                    break;

                case 6:
                    message = "@cya@" + message;
                    break;

                case 7:
                    message = "@whi@" + message;
                    break;
            }

            switch (status)
            {
                case 1:
                    message = "#pmd#" + message;
                    break;

                case 2:
                    message = "#mod#" + message;
                    break;

                case 3:
                    message = "#adm#" + message;
                    break;

                case 4:
                    message = "#evt#" + message;
                    break;

                case 5:
                    message = "#dev#" + message;
                    break;
            }

            //if (status == 6)
            //    message = "[ #orn#] " + message; // Clan Leader

            if (message.Substring(0, 8).Equals("@whi@%r-"))
            {
                killQueue.addKill(new KillThing("@whi@" + message.substring(8)));
                return;
            }

            if (messagesTab != 0)
            {
                if (type == 4 || type == 3)
                    anInt952 = 200;
                if (type == 2 && messagesTab != 1)
                    anInt953 = 200;
                if (type == 5 && messagesTab != 2)
                    anInt954 = 200;
                if (type == 6 && messagesTab != 3)
                    anInt955 = 200;
                if (type == 7 && messagesTab != 4)
                    anInt956 = 200;
                if (type == 3 && messagesTab != 0)
                    messagesTab = 0;
                if (type == 6 && messagesTab != 4 && messagesTab != 3 && messagesTab != 0)
                    messagesTab = 0;
            }
            for (int k = 4; k > 0; k--)
            {
                messagesArray[k] = messagesArray[k - 1];
                messagesTimeout[k] = messagesTimeout[k - 1];
            }

            messagesArray[0] = message;
            messagesTimeout[0] = 300;

            if (type == 2)
                if (gameMenu.anIntArray187[messagesHandleType2] == gameMenu.menuListTextCount[messagesHandleType2] - 4)
                    gameMenu.addString(messagesHandleType2, message, true);
                else
                    gameMenu.addString(messagesHandleType2, message, false);
            if (type == 5)
                if (gameMenu.anIntArray187[messagesHandleType5] == gameMenu.menuListTextCount[messagesHandleType5] - 4)
                    gameMenu.addString(messagesHandleType5, message, true);
                else
                    gameMenu.addString(messagesHandleType5, message, false);
            if (type == 7)
                if (gameMenu.anIntArray187[messagesHandleType7] == gameMenu.menuListTextCount[messagesHandleType7] - 4)
                    gameMenu.addString(messagesHandleType7, message, true);
                else
                    gameMenu.addString(messagesHandleType7, message, false);
            if (type == 6)
            {
                if (gameMenu.anIntArray187[messagesHandleType6] == gameMenu.menuListTextCount[messagesHandleType6] - 4)
                {
                    gameMenu.addString(messagesHandleType6, message, true);
                    return;
                }
                gameMenu.addString(messagesHandleType6, message, false);
            }
        }
        */

        public void drawMinimapObject(int x, int y, int color)
        {
            gameGraphics.drawMinimapPixel(x, y, color);
            gameGraphics.drawMinimapPixel(x - 1, y, color);
            gameGraphics.drawMinimapPixel(x + 1, y, color);
            gameGraphics.drawMinimapPixel(x, y - 1, color);
            gameGraphics.drawMinimapPixel(x, y + 1, color);
        }

        public void drawServerMessageBox()
        {
            char c1 = '\u0190';
            char c2 = 'd';
            if (serverMessageBoxTop)
            {
                c2 = '\u01C2';
                c2 = '\u012C';
            }
            gameGraphics.drawBox(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0);
            gameGraphics.drawBoxEdge(256 - c1 / 2, 167 - c2 / 2, c1, c2, 0xffffff);
            gameGraphics.drawFloatingText(serverMessage, 256, (167 - c2 / 2) + 20, 1, 0xffffff, c1 - 40);
            int l = 157 + c2 / 2;
            int i1 = 0xffffff;
            if (InputManager.Instance.MouseLocation.Y > l - 12 && InputManager.Instance.MouseLocation.Y <= l && InputManager.Instance.MouseLocation.X > 106 && InputManager.Instance.MouseLocation.X < 406)
            {
                i1 = 0xff0000;
            }

            gameGraphics.drawText("Click here to close window", 256, l, 1, i1);
            if (mouseButtonClick == 1)
            {
                if (i1 == 0xff0000)
                {
                    showServerMessageBox = false;
                }

                if ((InputManager.Instance.MouseLocation.X < 256 - c1 / 2 || InputManager.Instance.MouseLocation.X > 256 + c1 / 2) && (InputManager.Instance.MouseLocation.Y < 167 - c2 / 2 || InputManager.Instance.MouseLocation.Y > 167 + c2 / 2))
                {
                    showServerMessageBox = false;
                }
            }
            mouseButtonClick = 0;
        }

        //public Texture2D createImage(int l, int i1)
        //{
        //    //if(GameApplet.gameFrame != null)
        //    //    return GameApplet.gameFrame.createImage(l, i1);
        //    //if(link.gameApplet != null)
        //    //    return link.gameApplet.createImage(l, i1);
        //    //else
        //    return base.createImage(l, i1);
        //}

        public ObjectModel makeWallObject(int x, int y, int dir, int type, int totalCount)
        {

            int tileX = x;
            int tileY = y;
            int destTileX = x;
            int destTileY = y;
            int textureBack = EntityManager.GetWallObject(type).ModelFaceBack;
            int textureFront = EntityManager.GetWallObject(type).ModelFaceFront;
            int wallHeight = EntityManager.GetWallObject(type).ModelHeight;
            ObjectModel wallModel = new ObjectModel(4, 1);

            //      
            //    _ _ _ _
            //
            //       
            if (dir == 0)
            {
                destTileX = x + 1;
            }

            //    |  
            //    | 
            //    | 
            //    |
            if (dir == 1)
            {
                destTileY = y + 1;
            }

            //       /
            //      /
            //     /
            //    /
            if (dir == 2)
            {
                tileX = x + 1;
                destTileY = y + 1;
            }

            //    \  
            //     \ 
            //      \
            //       \
            if (dir == 3)
            {
                destTileX = x + 1;
                destTileY = y + 1;
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
            int[] faceVertices = {
            bLeft, tLeft, tRight, bRight
        };
            wallModel.addFaceVertices(4, faceVertices, textureBack, textureFront);
            wallModel.UpdateShading(false, 60, 24, -50, -10, -50);
            if (x >= 0 && y >= 0 && x < 96 && y < 96)
            {
                gameCamera.addModel(wallModel);
            }

            wallModel.index = totalCount + 10000;
            return wallModel;
        }

        public void ResetPrivateMessages()
        {
            privateMessageText = "";
            enteredPrivateMessageText = "";
        }

        public Mob AddNpc(int serverIndex, int x, int y, int sprite, int id)
        {
            if (NpcAttackingArray[serverIndex] == null)
            {
                NpcAttackingArray[serverIndex] = new Mob();
                NpcAttackingArray[serverIndex].ServerIndex = serverIndex;
            }

            Mob mob = NpcAttackingArray[serverIndex];

            bool alreadyExists = LastNpcs
                .Take(LastNpcCount)
                .Any(lastNpc => lastNpc.ServerIndex == serverIndex);

            if (alreadyExists)
            {
                mob.npcId = id;
                mob.nextSprite = sprite;

                int waypointCurrent = mob.WaypointCurrent;

                if (x != mob.WaypointsX[waypointCurrent] ||
                    y != mob.WaypointsY[waypointCurrent])
                {
                    mob.WaypointCurrent = waypointCurrent = (waypointCurrent + 1) % 10;
                    mob.WaypointsX[waypointCurrent] = x;
                    mob.WaypointsY[waypointCurrent] = y;
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
                mob.WaypointsX[0] = mob.currentX = x;
                mob.WaypointsY[0] = mob.currentY = y;
            }

            Npcs[NpcCount] = mob;
            NpcCount += 1;

            return mob;
        }

        public void updateBankItems()
        {
            bankItemsCount = serverBankItemsCount;
            for (int l = 0; l < serverBankItemsCount; l++)
            {
                bankItems[l] = serverBankItems[l];
                bankItemCount[l] = serverBankItemCount[l];
            }

            for (int i1 = 0; i1 < InventoryItemsCount; i1++)
            {
                if (bankItemsCount >= maxBankItems)
                {
                    break;
                }

                int j1 = InventoryItems[i1];
                bool flag = false;
                for (int k1 = 0; k1 < bankItemsCount; k1++)
                {
                    if (bankItems[k1] != j1)
                    {
                        continue;
                    }

                    flag = true;
                    break;
                }

                if (!flag)
                {
                    bankItems[bankItemsCount] = j1;
                    bankItemCount[bankItemsCount] = 0;
                    bankItemsCount++;
                }
            }

        }

        public void drawStatsQuestsMenu(bool canClick)
        {
            int l = gameGraphics.gameWidth - 199; //199
            int i1 = 36;
            gameGraphics.drawPicture(l - 49, 3, baseInventoryPic + 3);
            int c1 = 196;//'\u304';
            int c2 = 275;//113;//'\u0113';
            int k1;
            int j1 = k1 = GameImage.RgbToInt(160, 160, 160);
            if (questMenuSelected == 0)
            {
                j1 = GameImage.RgbToInt(220, 220, 220);
            }
            else
            {
                k1 = GameImage.RgbToInt(220, 220, 220);
            }

            gameGraphics.drawBoxAlpha(l, i1, c1 / 2, 24, j1, 128);
            gameGraphics.drawBoxAlpha(l + c1 / 2, i1, c1 / 2, 24, k1, 128);
            gameGraphics.drawBoxAlpha(l, i1 + 24, c1, c2 - 24, GameImage.RgbToInt(220, 220, 220), 128);
            gameGraphics.drawLineX(l, i1 + 24, c1, 0);
            gameGraphics.drawLineY(l + c1 / 2, i1, 24, 0);
            gameGraphics.drawText("Stats", l + c1 / 4, i1 + 16, 4, 0);
            gameGraphics.drawText("Quests", l + c1 / 4 + c1 / 2, i1 + 16, 4, 0);
            if (questMenuSelected == 0)
            {
                int l1 = 72;
                int j2 = -1;
                gameGraphics.drawString("Skills", l + 5, l1, 3, 0xffff00);
                l1 += 13;
                for (int k2 = 0; k2 < 9; k2++)
                {
                    int l2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X > l + 3 && InputManager.Instance.MouseLocation.Y >= l1 - 11 && InputManager.Instance.MouseLocation.Y < l1 + 2 && InputManager.Instance.MouseLocation.X < l + 90)
                    {
                        l2 = 0xff0000;
                        j2 = k2;
                    }
                    gameGraphics.drawString(skillName[k2] + ":@yel@" + PlayerStatCurrent[k2] + "/" + PlayerStatBase[k2], l + 5, l1, 1, l2);
                    l2 = 0xffffff;
                    if (InputManager.Instance.MouseLocation.X >= l + 90 && InputManager.Instance.MouseLocation.Y >= l1 - 13 - 11 && InputManager.Instance.MouseLocation.Y < (l1 - 13) + 2 && InputManager.Instance.MouseLocation.X < l + 196)
                    {
                        l2 = 0xff0000;
                        j2 = k2 + 9;
                    }
                    gameGraphics.drawString(skillName[k2 + 9] + ":@yel@" + PlayerStatCurrent[k2 + 9] + "/" + PlayerStatBase[k2 + 9], (l + c1 / 2) - 5, l1 - 13, 1, l2);
                    l1 += 13;
                }

                gameGraphics.drawString("Quest Points:@yel@" + QuestPoints, (l + c1 / 2) - 5, l1 - 13, 1, 0xffffff);
                l1 += 12;
                gameGraphics.drawString("Fatigue: @yel@" + (fatigue * 100) / 750 + "%", l + 5, l1 - 13, 1, 0xffffff);
                l1 += 8;
                gameGraphics.drawString("Equipment Status", l + 5, l1, 3, 0xffff00);
                l1 += 12;
                for (int i3 = 0; i3 < 3; i3++)
                {
                    gameGraphics.drawString(gearStats[i3] + ":@yel@" + EquipmentStatus[i3], l + 5, l1, 1, 0xffffff);
                    if (i3 < 2)
                    {
                        gameGraphics.drawString(gearStats[i3 + 3] + ":@yel@" + EquipmentStatus[i3 + 3], l + c1 / 2 + 25, l1, 1, 0xffffff);
                    }

                    l1 += 13;
                }

                l1 += 6;
                gameGraphics.drawLineX(l, l1 - 15, c1, 0);
                if (j2 != -1)
                {
                    gameGraphics.drawString(skillNameVerb[j2] + " skill", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int j3 = experienceList[0];
                    for (int l3 = 0; l3 < 98; l3++)
                    {
                        if (PlayerStatExperience[j2] >= experienceList[l3])
                        {
                            j3 = experienceList[l3 + 1];
                        }
                    }

                    gameGraphics.drawString("Total xp: " + PlayerStatExperience[j2], l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    gameGraphics.drawString("Next level at: " + j3, l + 5, l1, 1, 0xffffff);
                }
                else
                {
                    gameGraphics.drawString("Overall levels", l + 5, l1, 1, 0xffff00);
                    l1 += 12;
                    int k3 = 0;
                    for (int i4 = 0; i4 < 18; i4++)
                    {
                        k3 += PlayerStatBase[i4];
                    }

                    gameGraphics.drawString("Skill total: " + k3, l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                    gameGraphics.drawString("Combat level: " + CurrentPlayer.CombatLevel, l + 5, l1, 1, 0xffffff);
                    l1 += 12;
                }
            }
            if (questMenuSelected == 1)
            {
                questMenu.clearList(questMenuHandle);
                questMenu.addListItem(questMenuHandle, 0, "@whi@Quest-list (green=completed)");
                for (int i2 = 0; i2 < usedQuestName.Length; i2++)
                {
                    questMenu.addListItem(questMenuHandle, i2 + 1, (questStage[i2] == 0 ? "@red@" : questStage[i2] == 1 ? "@yel@" : "@gre@") + usedQuestName[i2]);
                }

                questMenu.drawMenu();
            }
            if (!canClick)
            {
                return;
            }

            l = InputManager.Instance.MouseLocation.X - (gameGraphics.gameWidth - 199);
            i1 = InputManager.Instance.MouseLocation.Y - 36;
            if (l >= 0 && i1 >= 0 && l < c1 && i1 < c2)
            {
                if (questMenuSelected == 1)
                {
                    questMenu.mouseClick(l + (gameGraphics.gameWidth - 199), i1 + 36, lastMouseButton, mouseButton);
                }

                if (i1 <= 24 && mouseButtonClick == 1)
                {
                    if (l < 98)
                    {
                        questMenuSelected = 0;
                        return;
                    }
                    if (l > 98)
                    {
                        questMenuSelected = 1;
                    }
                }
            }
        }

        public void drawFriendsBox()
        {
            if (mouseButtonClick != 0)
            {
                mouseButtonClick = 0;
                if (showFriendsBox == 1 && (InputManager.Instance.MouseLocation.X < 106 || InputManager.Instance.MouseLocation.Y < 145 || InputManager.Instance.MouseLocation.X > 406 || InputManager.Instance.MouseLocation.Y > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 2 && (InputManager.Instance.MouseLocation.X < 6 || InputManager.Instance.MouseLocation.Y < 145 || InputManager.Instance.MouseLocation.X > 506 || InputManager.Instance.MouseLocation.Y > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (showFriendsBox == 3 && (InputManager.Instance.MouseLocation.X < 106 || InputManager.Instance.MouseLocation.Y < 145 || InputManager.Instance.MouseLocation.X > 406 || InputManager.Instance.MouseLocation.Y > 215))
                {
                    showFriendsBox = 0;
                    return;
                }
                if (InputManager.Instance.MouseLocation.X > 236 && InputManager.Instance.MouseLocation.X < 276 && InputManager.Instance.MouseLocation.Y > 193 && InputManager.Instance.MouseLocation.Y < 213)
                {
                    showFriendsBox = 0;
                    return;
                }
            }
            int l = 145;
            if (showFriendsBox == 1)
            {
                gameGraphics.drawBox(106, l, 300, 70, 0);
                gameGraphics.drawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                gameGraphics.drawText("Enter name to add to friends list", 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.drawText(inputText + "*", 256, l, 4, 0xffffff);
                if (enteredInputText.Length > 0)
                {
                    string s1 = enteredInputText.Trim();
                    inputText = "";
                    enteredInputText = "";
                    showFriendsBox = 0;
                    if (s1.Length > 0 && DataOperations.nameToHash(s1) != CurrentPlayer.NameHash)
                    {
                        addFriend(s1);
                    }
                }
            }
            if (showFriendsBox == 2)
            {
                gameGraphics.drawBox(6, l, 500, 70, 0);
                gameGraphics.drawBoxEdge(6, l, 500, 70, 0xffffff);
                l += 20;
                gameGraphics.drawText("Enter message to send to " + DataOperations.LongToString(pmTarget), 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.drawText(privateMessageText + "*", 256, l, 4, 0xffffff);

                if (enteredPrivateMessageText.Length > 0)
                {
                    string s2 = enteredPrivateMessageText;
                    privateMessageText = "";
                    enteredPrivateMessageText = "";
                    showFriendsBox = 0;
                    int j1 = ChatMessage.stringToBytes(s2);
                    sendPrivateMessage(pmTarget, ChatMessage.lastChat, j1);
                    s2 = ChatMessage.bytesToString(ChatMessage.lastChat, 0, j1);
                    //if (useChatFilter)
                    // s2 = ChatFilter.filterChat(s2);
                    displayMessage("@pri@You tell " + DataOperations.LongToString(pmTarget) + ": " + s2);
                }
            }
            if (showFriendsBox == 3)
            {
                gameGraphics.drawBox(106, l, 300, 70, 0);
                gameGraphics.drawBoxEdge(106, l, 300, 70, 0xffffff);
                l += 20;
                gameGraphics.drawText("Enter name to add to ignore list", 256, l, 4, 0xffffff);
                l += 20;
                gameGraphics.drawText(inputText + "*", 256, l, 4, 0xffffff);
                if (enteredInputText.Length > 0)
                {
                    string s3 = enteredInputText.Trim();
                    inputText = "";
                    enteredInputText = "";
                    showFriendsBox = 0;
                    if (s3.Length > 0 && DataOperations.nameToHash(s3) != CurrentPlayer.NameHash)
                    {
                        AddIgnore(s3);
                    }
                }
            }
            int i1 = 0xffffff;
            if (InputManager.Instance.MouseLocation.X > 236 && InputManager.Instance.MouseLocation.X < 276 && InputManager.Instance.MouseLocation.Y > 193 && InputManager.Instance.MouseLocation.Y < 213)
            {
                i1 = 0xffff00;
            }

            gameGraphics.drawText("Cancel", 256, 208, 1, i1);
        }

        public void playSound(string s1)
        {
            if (audioPlayer == null || !GameDefines.PREMIUM_FEATURES)
            {
                return;
            }

            if (!SoundOff)
            {
                int off = (int)DataOperations.getObjectOffset(s1 + ".pcm", soundData);
                int len = DataOperations.getSoundLength(s1 + ".pcm", soundData);
                audioPlayer.Play(soundData, off, len);
            }
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
            gameGraphics.drawString("Choose option", menuX + 2, menuY + 12, 1, 65535);
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
                gameGraphics.drawString(menuText1[menuIndexes[i1]] + " " + menuText2[menuIndexes[i1]], k1, i2, 1, j2);
            }

        }

        public void getMenuHighlighted()
        {
            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 33 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 33 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 66 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 66 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 99 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 99 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 132 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 132 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab == 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 165 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 165 && InputManager.Instance.MouseLocation.Y < 35)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 1;
            }

            if (drawMenuTab != 0 && drawMenuTab != 2 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 33 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 33 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 2;
                minimapRandomRotationX = (int)(Helper.Random.NextDouble() * 13D) - 6;
                minimapRandomRotationY = (int)(Helper.Random.NextDouble() * 23D) - 11;
            }
            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 66 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 66 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 3;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 99 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 99 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 4;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 132 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 132 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 5;
            }

            if (drawMenuTab != 0 && InputManager.Instance.MouseLocation.X >= gameGraphics.gameWidth - 35 - 165 && InputManager.Instance.MouseLocation.Y >= 3 && InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 3 - 165 && InputManager.Instance.MouseLocation.Y < 26)
            {
                drawMenuTab = 6;
            }

            if (drawMenuTab == 1 && (InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 248 || InputManager.Instance.MouseLocation.Y > 36 + (maxInventoryItems / 5) * 34))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 3 && (InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 199 || InputManager.Instance.MouseLocation.Y > 316))
            {
                drawMenuTab = 0;
            }

            if ((drawMenuTab == 2 || drawMenuTab == 4 || drawMenuTab == 5) && (InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 199 || InputManager.Instance.MouseLocation.Y > 240))
            {
                drawMenuTab = 0;
            }

            if (drawMenuTab == 6 && (InputManager.Instance.MouseLocation.X < gameGraphics.gameWidth - 199 || InputManager.Instance.MouseLocation.Y > 326))
            {
                drawMenuTab = 0;
            }
        }

        protected int getUID()
        {
            return Link.uid;
        }

        public bool takeScreenshot(bool verb)
        {
            //try
            //{
            //    string charName = DataOperations.hashToName(DataOperations.nameToHash(username));
            //    File dir = new File(Config.MEDIA_DIR + "/" + charName);
            //    if (!dir.exists() || !dir.isDirectory())
            //        dir.mkdir();
            //    string folder = dir.getPath() + "/";
            //    File file = null;
            //    for (int count = 0; file == null || file.exists(); count++)
            //        file = new File(folder + "screenshot" + count + ".png");
            //    BufferedImage bi = new BufferedImage(windowWidth, windowHeight + 11, BufferedImage.TYPE_INT_RGB);
            //    Graphics2D g2d = bi.createGraphics();
            //    g2d.drawImage(gameGraphics.image, 0, 0, this);
            //    g2d.dispose();
            //    ImageIO.write(bi, "png", file);
            //    if (verb)
            //        displayMessage("Screenshot saved as " + file.getName());
            //    return true;
            //}
            //catch (IOException ioe)
            //{
            //    if (verb)
            //        displayMessage("Error saving screenshot");
            //    return false;
            //}
            return true;
        }

        public Mob GetLastPlayer(int serverIndex)
        {
            return LastPlayers
                .Where(x => x != null) // TODO: Remove this check once it is safe
                .FirstOrDefault(x => x.ServerIndex == serverIndex);
        }

        public Mob GetLastNpc(int serverIndex)
        {
            return LastNpcs
                .Where(x => x != null) // TODO: Remove this check once it is safe
                .FirstOrDefault(x => x.ServerIndex == serverIndex);
        }

        public bool handleCommand(string command)
        {
            try
            {
                int firstSpace = command.IndexOf(' ');
                string cmd = command;
                string[] args = new string[0];
                if (firstSpace != -1)
                {
                    cmd = command.Substring(0, firstSpace).Trim();
                    args = command.Substring(firstSpace).Trim().Split(' ');
                }
                if (cmd.Equals("closecon"))
                {
                    StreamClass.CloseStream();
                    return true;
                }
                if (cmd.Equals("logout"))
                {
                    sendLogout();
                    return true;
                }
                if (cmd.Equals("lostcon"))
                {
                    LostConnection();
                    return true;
                }
                if (cmd.Equals("tell"))
                {
                    long recipient = DataOperations.nameToHash(args[0]);
                    string message = joinString(args, " ", 1).Trim();
                    if (message.Equals(""))
                    {
                        return true;
                    }

                    int len = ChatMessage.stringToBytes(message);
                    sendPrivateMessage(recipient, ChatMessage.lastChat, len);
                    message = ChatMessage.bytesToString(ChatMessage.lastChat, 0, len);
                    //  if (useChatFilter)
                    //      message = ChatFilter.filterChat(message);
                    displayMessage("@pri@You tell " + DataOperations.LongToString(recipient) + ": " + message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured in {nameof(GameClient)}.cs");
                Console.WriteLine(ex.Message);
            }
            return false;
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

        public string tradeOtherName;
        public int windowWidth;
        public int windowHeight;
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
        public Menu chatInputMenu;
        int messagesHandleType2;
        int chatInputBox;
        int messagesHandleType5;
        int messagesHandleType6;
        int messagesTab;
        public int[] menuIndexes;
        public int duelMyItemCount;
        public int[] duelMyItems;
        public int[] duelMyItemsCount;
        public string[] questName = {// TODO really?... needs to be done better imho
            "Cook's Assistant", "Sheep Shearer", "Black knight's fortress", "Imp catcher", "Vampire slayer",
            "Romeo & Juliet", "The restless ghost", "Doric's quest", "The knight's sword", "Witch's potion",
            "Goblin diplomacy", "Ernest the chicken", "Demon Slayer", "Pirate's treasure", "Prince Ali Rescue",
            "Shield of Arrav", "Dragon Slayer"
        /*"Black knight's fortress", "Cook's assistant", "Demon slayer", "Doric's quest", "The restless ghost", "Goblin diplomacy", "Ernest the chicken",
        "Imp catcher", "Pirate's treasure", "Prince Ali rescue", 
        "Romeo & Juliet", "Sheep shearer", "Shield of Arrav", "The knight's sword", "Vampire slayer", "Witch's potion", "Dragon slayer", "Witch's house (members)",
        "Lost city (members)", "Hero's quest (members)", 
        "Druidic ritual (members)", "Merlin's crystal (members)", "Scorpion catcher (members)", "Family crest (members)", "Tribal totem (members)",
        "Fishing contest (members)", "Monk's friend (members)", "Temple of Ikov (members)", "Clock tower (members)", "The Holy Grail (members)", 
        "Fight Arena (members)", "Tree Gnome Village (members)", "The Hazeel Cult (members)", "Sheep Herder (members)", "Plague City (members)",
        "Sea Slug (members)", "Waterfall quest (members)", "Biohazard (members)", "Jungle potion (members)", "Grand tree (members)", 
        "Shilo village (members)", "Underground pass (members)", "Observatory quest (members)", "Tourist trap (members)", "Watchtower (members)",
        "Dwarf Cannon (members)", "Murder Mystery (members)", "Digsite (members)", "Gertrude's Cat (members)", "Legend's Quest (members)"*/
    };
        public int selectedShopItemIndex;
        public int selectedShopItemType;
        public string sleepingStatusText;
        public string[] menuText1;
        public bool IsSleeping;
        public int modelFireLightningSpellNumber;
        public int modelTorchNumber;
        public int modelClawSpellNumber;
        public int tradeItemsOtherCount;
        public int[] tradeItemsOther;
        public int[] tradeItemOtherCount;
        public bool tradeOtherAccepted;
        public bool tradeWeAccepted;
        public int[] itemAboveHeadScale;
        public int[] itemAboveHeadID;
        public int[] menuActionX;
        public int[] menuActionY;
        public string[] skillNameVerb = new string[] {
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcutting", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    };
        public MenuAction[] menuActions;
        public int cameraAutoRotatePlayerX;
        public int cameraAutoRotatePlayerY;
        public bool showTradeBox;
        public bool duelNoRetreating;
        public bool duelNoMagic;
        public bool duelNoPrayer;
        public bool duelNoWeapons;
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
        public bool duelOpponentAccepted;
        public bool duelMyAccepted;
        public bool serverMessageBoxTop;
        public int fatigue;
        public int cameraRotationYAmount;
        public int cameraRotationYIncrement;
        public int[] walkModel = {
        0, 1, 2, 1
    };
        public int itemsAboveHeadCount;
        public AudioReader audioPlayer;
        public string[] messagesArray;
        public long duelOpponentHash;
        public Menu questMenu;
        int questMenuHandle;
        int questMenuSelected;
        public bool[] objectAlreadyInMenu;
        public ObjectModel[] ObjectArray;
        public int selectedSpell;
        public bool cameraAutoAngleDebug;
        public int tradeItemsOurCount;
        public int[] tradeItemsOur;
        public int[] tradeItemOurCount;
        public int[] menuActionType;
        public int[] menuActionVar1;
        public int[] menuActionVar2;
        public bool sleepWordDelay;
        public int minimapRandomRotationX;
        public int minimapRandomRotationY;
        public int loginMenuOkButton;
        public int cameraRotation;
        public int CombatStyle;
        public int[] appearanceSkinColours = {
        0xecded0, 0xccb366, 0xb38c40, 0x997326, 0x906020
    };
        public bool menuShow;
        public int duelOpponentItemCount;
        public int[] duelOpponentItems;
        public int[] duelOpponentItemsCount;
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
        public sbyte[] soundData;
        public int shopItemSellPriceModifier;
        public int shopItemBuyPriceModifier;
        public int wildType;
        public long tradeConfirmOtherNameLong;
        public int showAbuseBox;
        public int[] serverBankItems;
        public int[] serverBankItemCount;
        public GameImageMiddleMan gameGraphics;
        public int maxBankItems;
        public int tradeConfirmOtherItemCount;
        public int[] tradeConfirmOtherItems;
        public int[] tradeConfirmOtherItemsCount;
        public int tick;
        public EngineHandle engineHandle;
        public int mouseButtonClick;
        public Menu loginNewUser;
        public int[] walkArrayX;
        public int[] walkArrayY;
        public int[] combatModelArray2 = {
        0, 0, 0, 0, 0, 1, 2, 1
    };
        public int cameraDistance;
        public int[] receivedMessageX;
        public int[] receivedMessageY;
        public int[] receivedMessageMidPoint;
        public int[] receivedMessageHeight;
        public int lastLayerIndex;
        public int[] bankItems;
        public int[] bankItemCount;
        public string[] skillName = {
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcut", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    };
        public int combatTimeout;
        public int maxInventoryItems;
        public static GraphicsDevice graphics;
        public bool errorLoading;
        public int animationNumber;
        public int[] itemAboveHeadX;
        public int[] itemAboveHeadY;
        public int duelRetreat;
        public int duelMagic;
        public int duelPrayer;
        public int duelWeapons;
        public bool showServerMessageBox;
        public int loginScreenNumber;
        public int tradeConfigItemCount;
        public int[] tradeConfirmItems;
        public int[] tradeConfigItemsCount;
        public int selectedBankItem;
        public int selectedBankItemType;
        public bool duelConfirmOurAccepted;
        public int modelUpdatingTimer;
        public int selectedItem;
        string selectedItemName;
        public bool showTradeConfirmBox;
        public bool tradeConfirmAccepted;
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
        public int duelOpponentStakeCount;
        public int[] duelOpponentStakeItem;
        public int[] duelOutStakeItemCount;
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
        public int logoutTimer;
        public bool loggedIn;
        public int[] questStage;
        public int[] teleBubbleType;
        public int[] experienceList;
        public int lastModelFireLightningSpellNumber;
        public int lastModelTorchNumber;
        public int lastModelClawSpellNumber;
        public int chatTabAllMsgFlash;
        public int chatTabHistoryFlash;
        public int chatTabQuestFlash;
        public int chatTabPrivateFlash;
        public int[] messagesTimeout;
        public int[] appearanceTopBottomColours = {
        0xff0000, 0xff8000, 0xffe000, 0xa0e000, 57344, 32768, 41088, 45311, 33023, 12528,
        0xe000e0, 0x303030, 0x604000, 0x805000, 0xffffff
    };
        public int showFriendsBox;
        public int teleBubbleCount;
        public bool memoryError;
        public int[] appearanceHairColours = {
        0xffc030, 0xffa040, 0x805030, 0x604020, 0x303030, 0xff6020, 0xff4000, 0xffffff, 65280, 65535
    };
        public Menu spellMenu;
        int spellMenuHandle;
        int menuMagicPrayersSelected;
        public int duelOurStakeCount;
        public int[] duelOurStakeItem;
        public int[] duelOurStakeItemCount;
        public int menuX;
        public int menuY;
        public int menuWidth;
        public int menuHeight;
        public int menuOptionsCount;
        public Camera gameCamera;
        public Menu friendsMenu;
        int friendsMenuHandle;
        int friendsIgnoreMenuSelected;
        long pmTarget;
        public int healthBarVisibleCount;
        public string[] menuText2;
        public int sleepWordDelayTimer;
        public int mouseButtonHeldTime;
        public int mouseClickedHeldInTradeDuelBox;
        public string loginUsername;
        public string loginPassword;
        public string duelOpponent;
        public int bankPage;
        public Menu loginMenuFirst;
        public int[] healthBarX;
        public int[] healthBarY;
        public int[] healthBarMissing;
        public int reportAbuseOptionSelected;
        public int serverBankItemsCount;
        public int[] teleBubbleY;
        public int cameraAutoAngle;
        public int cameraAutoRotationAmount;
        public int[] teleBubbleX;
        public int bankItemsCount;
        public int actionPictureType;
        int walkMouseX;
        int walkMouseY;
        public int[] combatModelArray1 = {
        0, 1, 2, 1, 0, 0, 0, 0
    };
        public bool cameraZoom;

        public bool fogOfWar;
        public bool useChatFilter;
        public string[] usedQuestName;
        public int[] shopItemSellPrice;
        public int[] shopItemBuyPrice;
        public int[][] captchaPixels;
        public int captchaWidth;
        public int captchaHeight;
    }
}