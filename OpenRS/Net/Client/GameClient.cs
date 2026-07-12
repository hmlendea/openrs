using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Net.Client.Game.Cameras;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using OpenRS.Net.Client.Events;
using OpenRS.Settings;
using System.Threading;

namespace OpenRS.Net.Client
{

    public sealed partial class GameClient : GameAppletMiddleMan
    {
        public int killingSpree;
        public event EventHandler OnContentLoadedCompleted;
        public event EventHandler<ContentLoadedEventArgs> OnContentLoaded;

        public static Microsoft.Xna.Framework.GameWindow GameWindow;

        public event ChatMessageEventHandler OnChatMessageReceived;

        public static GameClient CreateMudclient(string title, int width, int height)
        {
            GameClient mud = new()
            {
                windowWidth = width,
                windowHeight = height
            };
            mud.CreateWindow(mud.windowWidth, mud.windowHeight + 11, title, false);
            mud.gameMinThreadSleepTime = 10;

            return mud;
        }

        public static GameClient CreateMudclient(string title)
        {
            return CreateMudclient(title, 512, 346);
        }

        public static GameClient CreateMudclient()
        {
            return CreateMudclient("RuneScape", 512, 346);
        }

        public static GameClient CreateGameClient(string username, string password, int width, int height)
        {
            return CreateMudclient("RuneScape", width, height);
        }

        public static GameClient CreateGameClient(string username, string password)
        {
            return CreateMudclient("RuneScape", 512, 346);
        }

        public new void Dispose()
        {
            Destroy();
        }

        public void Paint() { }

        public void UnloadContent() { }


        public void SetCombatStyle(Models.Enumerations.CombatStyle style)
        {
            combatStyle = (int)style;
        }

        public ClientMob CurrentPlayer
        {
            get
            {
                if (playerArray is null || playerArray.Length == 0)
                {
                    return null;
                }

                return playerArray[0];
            }
        }

        public ClientMob[] Players => playerArray;

        public ClientMob[] Npcs => npcArray;

        public int GridSize => gridSize;

        public int GroundItemCount => groundItemCount;

        public int PlayerFatigue => fatigue;

        public NuciXNA.Primitives.Point2D[] GroundItemLocations
        {
            get
            {
                NuciXNA.Primitives.Point2D[] locs = new NuciXNA.Primitives.Point2D[groundItemCount];
                for (int i = 0; i < groundItemCount; i += 1)
                {
                    locs[i] = new NuciXNA.Primitives.Point2D(groundItemX[i], groundItemY[i]);
                }
                return locs;
            }
        }

        public Models.Skill[] Skills
        {
            get
            {
                if (playerStatBase is null)
                {
                    return [];
                }

                Models.Skill[] skills = new Models.Skill[playerStatBase.Length];

                for (int i = 0; i < playerStatBase.Length; i += 1)
                {
                    int currentLevel = 0;

                    if (playerStatCurrent is not null)
                    {
                        currentLevel = playerStatCurrent[i];
                    }

                    int experience = 0;

                    if (playerStatExp is not null)
                    {
                        experience = playerStatExp[i];
                    }

                    string name = string.Empty;

                    if (skillName is not null && i < skillName.Length)
                    {
                        name = skillName[i];
                    }

                    skills[i] = new Models.Skill
                    {
                        BaseLevel = playerStatBase[i],
                        CurrentLevel = currentLevel,
                        Experience = experience,
                        Name = name
                    };
                }
                return skills;
            }
        }

        public int GameDisplayOffsetX { get; set; }

        public int GameDisplayOffsetY { get; set; }

        public float GameDisplayScaleX { get; set; } = 1.0f;

        public float GameDisplayScaleY { get; set; } = 1.0f;

        public GameLogic.GameManagers.EntityManager entityManager;

        public GameLogic.GameManagers.InventoryManager inventoryManager;




        public override void ResetIntVars()
        {
            systemUpdate = 0;
            loginScreen = 0;
            loggedIn = false;
            logoutTimer = 0;
        }









        //protected TcpClient makeSocket(string address, int port) {

        //    if(Link.gameApplet is not null) {
        //        Socket socket = Link.getSocket(port);
        //        if(socket is null)
        //            throw new IOException();
        //        else
        //            return socket;
        //    }
        //    Socket socket1 = new Socket(InetAddress.getByName(address), port);
        //    socket1.setSoTimeout(30000);
        //    socket1.setTcpNoDelay(true);
        //    return socket1;
        //}

        public bool DoNotDrawLogo { get; set; }


        //protected void startThread(Runnable runnable) {
        //    if(Link.gameApplet is not null) {
        //        Link.thread(runnable);
        //        return;
        //    } else {
        //        Thread thread = new Thread(runnable);
        //        thread.setDaemon(true);
        //        thread.start();
        //        return;
        //    }
        //}















        //public Uri getCodeBase() {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getCodeBase();
        //    else
        //        return base.getCodeBase();
        //}









        //public string getParameter(string s1) {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getParameter(s1);
        //    else
        //        return base.getParameter(s1);
        //}






















        //public URL getDocumentBase() {
        //    if(Link.gameApplet is not null)
        //        return Link.gameApplet.getDocumentBase();
        //    else
        //        return base.getDocumentBase();
        //}













        //	public bool DrawCustomMenus { get; set; }
        //    public event EventHandler OnDrawMenus;

















        //public Texture2D createImage(int l, int i1)
        //{
        //    //if(GameApplet.gameFrame is not null)
        //    //    return GameApplet.gameFrame.createImage(l, i1);
        //    //if(Link.gameApplet is not null)
        //    //    return Link.gameApplet.createImage(l, i1);
        //    //else
        //    return base.createImage(l, i1);
        //}

















        public GameClient()
        {
            tradeOtherName = "";


            windowWidth = 512;
            windowHeight = 334;


            cameraFieldOfView = 9;
            showQuestionMenu = false;
            loginScreenShown = false;
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
            playerArray = new ClientMob[500];
            selectedShopItemIndex = -1;
            selectedShopItemType = -2;
            menuText1 = new string[250];
            isSleeping = false;
            tradeItemsOther = new int[14];
            tradeItemOtherCount = new int[14];
            tradeOtherAccepted = false;
            tradeWeAccepted = false;
            itemAboveHeadScale = new int[50];
            itemAboveHeadID = new int[50];
            playerStatCurrent = new int[18];
            menuActionX = new int[250];
            menuActionY = new int[250];
            menuActionID = new int[250];
            showTradeBox = false;
            npcArray = new ClientMob[500];
            duelNoRetreating = false;
            duelNoMagic = false;
            duelNoPrayer = false;
            duelNoWeapons = false;
            playerBufferArray = new ClientMob[4000];
            serverMessage = "";
            duelOpponentAccepted = false;
            duelMyAccepted = false;
            wallObjectX = new int[500];
            wallObjectY = new int[500];
            serverMessageBoxTop = false;
            cameraRotationYIncrement = 2;
            wallObjectArray = new GameObject[500];
            messagesArray = new string[5];
            objectAlreadyInMenu = new bool[1500];
            objectArray = new GameObject[1500];
            selectedSpell = -1;
            cameraAutoAngleDebug = false;
            ourPlayer = new ClientMob();
            serverIndex = -1;
            tradeItemsOur = new int[14];
            tradeItemOurCount = new int[14];
            showWelcomeBox = false;
            menuActionType = new int[250];
            menuActionVar1 = new int[250];
            menuActionVar2 = new int[250];
            sleepWordDelay = true;
            configCameraAutoAngle = true;
            cameraRotation = 128;
            configSoundOff = false;
            menuShow = false;
            duelOpponentItems = new int[8];
            duelOpponentItemsCount = new int[8];
            showBankBox = false;
            playerStatBase = new int[18];
            serverBankItems = new int[256];
            serverBankItemCount = new int[256];
            showShopBox = false;
            groundItemX = new int[5000];
            groundItemY = new int[5000];
            groundItemID = new int[5000];
            groundItemObjectVar = new int[5000];
            maxBankItems = 48;
            tradeConfirmOtherItems = new int[14];
            tradeConfirmOtherItemsCount = new int[14];
            layerIndex = -1;
            walkArrayX = new int[8000];
            walkArrayY = new int[8000];
            cameraDistance = 550;
            receivedMessageX = new int[50];
            receivedMessageY = new int[50];
            receivedMessageMidPoint = new int[50];
            receivedMessageHeight = new int[50];
            wallObjectAlreadyInMenu = new bool[500];
            lastLayerIndex = -1;
            bankItems = new int[256];
            bankItemCount = new int[256];
            maxInventoryItems = 30;
            errorLoading = false;
            itemAboveHeadX = new int[50];
            itemAboveHeadY = new int[50];
            showServerMessageBox = false;
            playerBufferArrayIndexes = new int[500];
            tradeConfirmItems = new int[14];
            tradeConfigItemsCount = new int[14];
            selectedBankItem = -1;
            selectedBankItemType = -2;
            showDuelConfirmBox = false;
            duelConfirmOurAccepted = false;
            wallObjectDirection = new int[500];
            wallObjectID = new int[500];
            gameDataObjects = new GameObject[1000];
            lastNpcArray = new ClientMob[500];
            inventoryItems = new int[35];
            inventoryItemCount = new int[35];
            inventoryItemEquipped = new int[35];
            selectedItem = -1;
            selectedItemName = "";
            lastPlayerArray = new ClientMob[500];
            showTradeConfirmBox = false;
            tradeConfirmAccepted = false;
            playerStatExp = new int[18];
            mouseTrailX = new int[8192];
            mouseTrailY = new int[8192];
            configOneMouseButton = false;
            prayerOn = new bool[50];
            shopItems = new int[256];
            shopItemCount = new int[256];
            shopItemBasePriceModifier = new int[256];
            duelOpponentStakeItem = new int[8];
            duelOutStakeItemCount = new int[8];
            equipmentStatus = new int[5];
            receivedMessages = new string[50];
            cameraRotationXIncrement = 2;
            teleBubbleTime = new int[50];
            gridSize = 128;
            questStage = new int[questName.Length];
            teleBubbleType = new int[50];
            experienceList = new int[99];
            lastModelFireLightningSpellNumber = -1;
            lastModelTorchNumber = -1;
            lastModelClawSpellNumber = -1;
            messagesTimeout = new int[5];
            projectileRange = 40;
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
            objectX = new int[1500];
            objectY = new int[1500];
            objectType = new int[1500];
            objectRotation = new int[1500];
            showDuelBox = false;
            npcAttackingArray = new ClientMob[5000];
            teleBubbleY = new int[50];
            cameraAutoAngle = 1;
            loadArea = false;
            teleBubbleX = new int[50];
            showAppearanceWindow = false;
            cameraZoom = false;

            fogOfWar = true;
            showCombatWindow = false;
            showRoofs = true;
            autoScreenshot = false;
            useChatFilter = true;
            usedQuestName = [];
            subDaysLeft = 0;
            shopItemSellPrice = new int[256];
            shopItemBuyPrice = new int[256];
            captchaPixels = [];
            captchaWidth = 0;
            captchaHeight = 0;
            needsClear = false;
            hasWorldInfo = false;
            //ImageIO.setCacheDirectory(new File(Config.CONF_DIR));
        }

        public string tradeOtherName;
        public int windowWidth;
        public int windowHeight;
        public int cameraFieldOfView;
        public bool showQuestionMenu;
        public bool loginScreenShown;
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
        private int messagesHandleType2;
        private int chatInputBox;
        private int messagesHandleType5;
        private int messagesHandleType6;
        private int messagesTab;
        public int[] menuIndexes;
        public int duelMyItemCount;
        public int[] duelMyItems;
        public int[] duelMyItemsCount;
        public int systemUpdate;
        public ClientMob[] playerArray;
        public string[] questName = [// TODO really?... needs to be done better imho
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
    ];
        public int selectedShopItemIndex;
        public int selectedShopItemType;
        public string sleepingStatusText;
        public string[] menuText1;
        public bool isSleeping;
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
        public int[] playerStatCurrent;
        public int[] menuActionX;
        public int[] menuActionY;
        public string[] skillNameVerb = [
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcutting", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    ];
        public int[] menuActionID;
        public int playerAliveTimeout;
        public int cameraAutoRotatePlayerX;
        public int cameraAutoRotatePlayerY;
        public bool showTradeBox;
        public ClientMob[] npcArray;
        public bool duelNoRetreating;
        public bool duelNoMagic;
        public bool duelNoPrayer;
        public bool duelNoWeapons;
        public Menu appearanceMenu;
        public int[][] animationModelArray =
        [ [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        ], [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            3, 4
        ], [
            11, 3, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 4
        ], [
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            3, 4, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            4, 3, 2, 9, 7, 1, 6, 10, 8, 11,
            0, 5
        ], [
            11, 4, 2, 9, 7, 1, 6, 10, 0, 5,
            8, 3
        ], [
            11, 2, 9, 7, 1, 6, 10, 0, 5, 8,
            4, 3
        ]
    ];
        public int playerCount;
        public int lastPlayerCount;
        public int drawUpdatesPerformed;
        public ClientMob[] playerBufferArray;
        public string serverMessage;
        public int groundItemCount;
        public bool duelOpponentAccepted;
        public bool duelMyAccepted;
        public int[] wallObjectX;
        public int[] wallObjectY;
        public bool serverMessageBoxTop;
        public int fatigue;
        public int cameraRotationYAmount;
        public int cameraRotationYIncrement;
        public int[] walkModel = [
        0, 1, 2, 1
    ];
        public int itemsAboveHeadCount;
        public AudioReader audioPlayer;
        public GameObject[] wallObjectArray;
        public string[] messagesArray;
        public long duelOpponentHash;
        public Menu questMenu;
        private int questMenuHandle;
        private int questMenuSelected;
        public bool[] objectAlreadyInMenu;
        public GameObject[] objectArray;
        public int selectedSpell;
        public bool cameraAutoAngleDebug;
        public string lastLoginAddress;
        public ClientMob ourPlayer;
        private int sectionX;
        private int sectionY;
        private int serverIndex;
        public int tradeItemsOurCount;
        public int[] tradeItemsOur;
        public int[] tradeItemOurCount;
        public bool showWelcomeBox;
        public int[] menuActionType;
        public int[] menuActionVar1;
        public int[] menuActionVar2;
        public bool sleepWordDelay;
        public bool configCameraAutoAngle;
        public int minimapRandomRotationX;
        public int minimapRandomRotationY;
        public int loginMenuOkButton;
        public int cameraRotation;
        public int combatStyle;
        public int[] appearanceSkinColours = [
        0xecded0, 0xccb366, 0xb38c40, 0x997326, 0x906020
    ];
        public bool configSoundOff;
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
        public bool showBankBox;
        public int shopItemSellPriceModifier;
        public int shopItemBuyPriceModifier;
        public int wildType;
        public int[] playerStatBase;
        public long tradeConfirmOtherNameLong;
        public int showAbuseBox;
        public int[] serverBankItems;
        public int[] serverBankItemCount;
        public bool showShopBox;
        public int[] groundItemX;
        public int[] groundItemY;
        public int[] groundItemID;
        public int[] groundItemObjectVar;
        public GameImageMiddleMan gameGraphics;
        public int maxBankItems;
        public int tradeConfirmOtherItemCount;
        public int[] tradeConfirmOtherItems;
        public int[] tradeConfirmOtherItemsCount;
        public int tick;
        public EngineHandle engineHandle;
        public int areaX;
        public int areaY;
        public int layerIndex;
        public int mouseButtonClick;
        public Menu loginNewUser;
        public int[] walkArrayX;
        public int[] walkArrayY;
        public int[] combatModelArray2 = [
        0, 0, 0, 0, 0, 1, 2, 1
    ];
        public int cameraDistance;
        public int[] receivedMessageX;
        public int[] receivedMessageY;
        public int[] receivedMessageMidPoint;
        public int[] receivedMessageHeight;
        public bool[] wallObjectAlreadyInMenu;
        public int wildX;
        public int wildY;
        public int layerModifier;
        public int lastLayerIndex;
        public int[] bankItems;
        public int[] bankItemCount;
        public string[] skillName = [
        "Attack", "Defense", "Strength", "Hits", "Ranged", "Prayer", "Magic", "Cooking", "Woodcut", "Fletching",
        "Fishing", "Firemaking", "Crafting", "Smithing", "Mining", "Herblaw", "Agility", "Thieving"
    ];
        public int npcCount;
        public int lastNpcCount;
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
        public int[] playerBufferArrayIndexes;
        public int loginScreen;
        public int tradeConfigItemCount;
        public int[] tradeConfirmItems;
        public int[] tradeConfigItemsCount;
        public int selectedBankItem;
        public int selectedBankItemType;
        public bool showDuelConfirmBox;
        public bool duelConfirmOurAccepted;
        public int[] wallObjectDirection;
        public int[] wallObjectID;
        public GameObject[] gameDataObjects;
        public ClientMob[] lastNpcArray;
        public int modelUpdatingTimer;
        public int inventoryItemsCount;
        public int[] inventoryItems;
        public int[] inventoryItemCount;
        public int[] inventoryItemEquipped;
        public int selectedItem;
        private string selectedItemName;
        public ClientMob[] lastPlayerArray;
        public bool showTradeConfirmBox;
        public bool tradeConfirmAccepted;
        public int[] playerStatExp;
        public int loginButtonNewUser;
        public int loginMenuLoginButton;
        public int mouseTrailIndex;
        private int[] mouseTrailX;
        private int[] mouseTrailY;
        public bool configOneMouseButton;
        public bool[] prayerOn;
        public int lastLoginDays;
        public int loginMenuStatusText;
        public int loginMenuUserText;
        public int loginMenuPasswordText;
        public int loginMenuOkLoginButton;
        public int loginMenuCancelButton;
        public int[] shopItems;
        public int[] shopItemCount;
        public int[] shopItemBasePriceModifier;
        public int objectCount;
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
        public int[] equipmentStatus;
        public int drawMenuTab;
        public int receivedMessagesCount;
        private string[] receivedMessages;
        public int cameraRotateTime;
        public int questionMenuCount;
        public int cameraRotationXAmount;
        public int cameraRotationXIncrement;
        public int[] teleBubbleTime;
        public string[] gearStats = [
        "Armour", "WeaponAim", "WeaponPower", "Magic", "Prayer"
    ];
        public int logoutTimer;
        public int wallObjectCount;
        public int gridSize;
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
        public int projectileRange;
        public int[] appearanceTopBottomColours = [
        0xff0000, 0xff8000, 0xffe000, 0xa0e000, 57344, 32768, 41088, 45311, 33023, 12528,
        0xe000e0, 0x303030, 0x604000, 0x805000, 0xffffff
    ];
        public int showFriendsBox;
        public int teleBubbleCount;
        public bool memoryError;
        public int[] appearanceHairColours = [
        0xffc030, 0xffa040, 0x805030, 0x604020, 0x303030, 0xff6020, 0xff4000, 0xffffff, 65280, 65535
    ];
        public Menu spellMenu;
        private int spellMenuHandle;
        private int menuMagicPrayersSelected;
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
        private int friendsMenuHandle;
        private int friendsIgnoreMenuSelected;
        private long pmTarget;
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
        public int[] objectX;
        public int[] objectY;
        public int[] objectType;
        public int[] objectRotation;
        public int reportAbuseOptionSelected;
        public bool showDuelBox;
        public ClientMob[] npcAttackingArray;
        public int serverBankItemsCount;
        public int[] teleBubbleY;
        public int cameraAutoAngle;
        public int cameraAutoRotationAmount;
        public bool loadArea;
        public int[] teleBubbleX;
        public int bankItemsCount;
        public bool showAppearanceWindow;
        public int questPoints;
        public int actionPictureType;
        private int walkMouseX;
        private int walkMouseY;
        public int[] combatModelArray1 = [
        0, 1, 2, 1, 0, 0, 0, 0
    ];
        public bool cameraZoom;

        public bool fogOfWar;
        public bool showCombatWindow;
        public bool showRoofs;
        public bool autoScreenshot;
        public bool useChatFilter;
        public string[] usedQuestName;
        public int subDaysLeft;
        public int[] shopItemSellPrice;
        public int[] shopItemBuyPrice;
        public int[][] captchaPixels;
        public int captchaWidth;
        public int captchaHeight;
        public bool needsClear;
        public bool hasWorldInfo;

        //public void LoadContent()
        //{
        //    throw new NotImplementedException();
        //}
    }

}
