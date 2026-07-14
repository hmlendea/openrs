using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;
using OpenRS.Settings;

namespace OpenRS.Net.Client
{
    public class GameApplet
    {
        public GameApplet()
        {
        }

        public virtual void LoadGame()
        {
        }

        public virtual void CheckInputs()
        {
        }

        public virtual void Close()
        {
        }

        public void CreateWindow(int width, int height, string title, bool resizable)
        {
            logger.Info(GameOperation.Startup, "The application has started.");
            appletWidth = width;
            appletHeight = height;
            gameFrame = new GameFrame(this, width, height, title, resizable, false);
            gameLoadingScreen = 1;

            InitGameApplet();
        }

        public void SetRefreshRate(int rate)
        {
            refreshRate = 1000 / rate;
        }

        public void ResetTimings()
        {
            for (int timeIndex = 0; timeIndex < 10; timeIndex += 1)
            {
                timeArray[timeIndex] = 0L;
            }
        }

        public void KeyTyped(EventArgs e)
        {
            // Ignore.
        }

        public void MouseClicked(EventArgs e)
        {
            // Ignore.
        }

        public void KeyPressed(Keys key)
        {
            char[] keyChars = Encoding.UTF8.GetChars([(byte)key]);
            KeyDown(key, keyChars[0]);
        }

        public void KeyReleased(Keys key)
        {
            char[] keyChars = Encoding.UTF8.GetChars([(byte)key]);
            KeyUp(key, keyChars[0]);
        }

        public void MouseEntered(MouseState mouseState)
        {
            MouseMove(mouseState.X, mouseState.Y);
        }

        public void MouseExited(MouseState mouseState)
        {
            MouseMove(mouseState.X, mouseState.Y);
        }

        public void MousePressed(MouseState mouseState)
        {
            MouseDown(mouseState.X, mouseState.Y, mouseState.RightButton == ButtonState.Pressed);
        }

        public void MouseReleased(MouseState mouseState)
        {
            MouseUp(mouseState.X, mouseState.Y);
        }

        public void MouseDragged(MouseState mouseState)
        {
            MouseDrag(mouseState.Y, mouseState.X, mouseState.RightButton == ButtonState.Pressed);
        }

        public void MouseMoved(MouseState mouseState)
        {
            MouseMove(mouseState.X, mouseState.Y);
        }

        public void KeyDown(Keys key, char character)
        {
            HandleKeyDown(key, character);
            if (key == Keys.Left)
            {
                keyLeftDown = true;
            }

            if (key == Keys.Right)
            {
                keyRightDown = true;
            }

            if (key == Keys.Up)
            {
                keyUpDown = true;
            }

            if (key == Keys.Down)
            {
                keyDownDown = true;
            }

            if (key == Keys.Space)
            {
                keySpaceDown = true;
            }

            if (key == Keys.N || key == Keys.M)
            {
                keyNMDown = true;
            }

            if (key == Keys.F1)
            {
                keyF1Toggle = !keyF1Toggle;
            }

            bool charIsAllowed = false;

            for (int charIndex = 0; charIndex < AllowedChars.Length; charIndex += 1)
            {
                if (character != AllowedChars[charIndex] && key != Keys.Left && key != Keys.Right && key != Keys.Up && key != Keys.Down)
                {
                    continue;
                }

                charIsAllowed = true;
                break;
            }

            if (charIsAllowed && inputText.Length < 20)
            {
                inputText += character;
            }

            if (charIsAllowed && pmText.Length < 80)
            {
                pmText += character;
            }

            if (key == Keys.Back && inputText.Length > 0)
            {
                inputText = inputText.Substring(0, inputText.Length - 1);
            }

            if (key == Keys.Back && pmText.Length > 0)
            {
                pmText = pmText.Substring(0, pmText.Length - 1);
            }

            if (key == Keys.Enter)
            {
                enteredInputText = inputText;
                enteredPMText = pmText;
            }
        }

        public virtual void HandleKeyDown(Keys key, char character)
        {
        }

        public void KeyUp(Keys key, char character)
        {
            if (key == Keys.Left)
            {
                keyLeftDown = false;
            }

            if (key == Keys.Right)
            {
                keyRightDown = false;
            }

            if (key == Keys.Up)
            {
                keyUpDown = false;
            }

            if (key == Keys.Down)
            {
                keyDownDown = false;
            }

            if (key == Keys.Space)
            {
                keySpaceDown = false;
            }

            if (key == Keys.N || key == Keys.M)
            {
                keyNMDown = false;
            }
        }

        public bool MouseMove(int x, int y)
        {
            mouseX = x;
            mouseY = y - mouseYOffset;
            mouseButton = 0;

            return true;
        }

        public bool MouseUp(int x, int y)
        {
            mouseX = x;
            mouseY = y - mouseYOffset;
            mouseButton = 0;

            return true;
        }

        public bool MouseDown(int x, int y, bool isMetaDown)
        {
            mouseX = x;
            mouseY = y - mouseYOffset;

            if (isMetaDown)
            {
                mouseButton = 2;
            }
            else
            {
                mouseButton = 1;
            }

            lastMouseButton = mouseButton;
            HandleMouseDown(mouseButton, x, y);

            return true;
        }

        public virtual void HandleMouseDown(int pressedMouseButton, int mouseXPosition, int mouseYPosition)
        {
        }

        public bool MouseDrag(int x, int y, bool isMetaDown)
        {
            mouseX = x;
            mouseY = y - mouseYOffset;

            if (isMetaDown)
            {
                mouseButton = 2;
            }
            else
            {
                mouseButton = 1;
            }

            return true;
        }

        public void Init()
        {
            logger.Info(GameOperation.Startup, "The applet has started.");
            appletWidth = 512;
            appletHeight = 344;
            gameLoadingScreen = 1;
            DataOperations.CodeBase = GetCodeBase();
        }

        public void Start()
        {
            if (runStatus >= 0)
            {
                runStatus = 0;
            }
        }

        public void Stop()
        {
            if (runStatus >= 0)
            {
                runStatus = 4000 / refreshRate;
            }
        }

        public void Destroy()
        {
            runStatus = -1;

            try
            {
                Thread.Sleep(2000);
            }
            catch (Exception) { }

            if (runStatus == -1)
            {
                logger.Warn(GameOperation.ForceShutdown, "The 2-second timeout has expired, forcing kill.");
                CloseProgram();
                gameWindowThread?.Interrupt();
                gameWindowThread = null;
            }
        }

        public void CloseProgram()
        {
            runStatus = -2;
            logger.Info(GameOperation.Shutdown, "The program is closing.");
            Close();

            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception) { }
        }

        public void LoadApp()
        {
        }

        public void Run()
        {
            if (gameLoadingScreen == 1)
            {
                gameLoadingScreen = 2;
                LoadLoadingScreen();
                DrawLoadingScreen(0, "Loading...");
                LoadGame();
                gameLoadingScreen = 0;
            }

            for (int timeIndex = 0; timeIndex < 10; timeIndex += 1)
            {
                timeArray[timeIndex] = CurrentTimeMillis();
            }

            while (runStatus >= 0)
            {
                UpdateGame(gameTimingArrayIndex, gameTimingMultiplier, gameThreadSleepTime, gameLoopAccumulator);
                OnDrawDone();
            }

            if (runStatus == -1)
            {
                CloseProgram();
                gameWindowThread = null;
            }
        }

        public bool DrawIsNecessary;

        public void OnDrawDone() => DrawIsNecessary = true;

        public int gameTimingArrayIndex;
        public int gameTimingMultiplier = 256;
        public int gameThreadSleepTime = 1;
        public int gameLoopAccumulator;

        public void UpdateGame(int timingArrayIndex, int timingMultiplier, int sleepTime, int loopAccumulator)
        {
            if (runStatus > 0)
            {
                runStatus -= 1;

                if (runStatus == 0)
                {
                    CloseProgram();
                    gameWindowThread = null;

                    return;
                }
            }

            int savedTimingMultiplier = timingMultiplier;
            int savedSleepTime = sleepTime;
            timingMultiplier = 300;
            sleepTime = 1;
            long currentTime = CurrentTimeMillis();

            if (timeArray[timingArrayIndex] == 0L)
            {
                timingMultiplier = savedTimingMultiplier;
                sleepTime = savedSleepTime;
            }
            else if (currentTime > timeArray[timingArrayIndex])
            {
                timingMultiplier = (int)(2560 * refreshRate / (currentTime - timeArray[timingArrayIndex]));
            }

            if (timingMultiplier < 25)
            {
                timingMultiplier = 25;
            }

            if (timingMultiplier > 256)
            {
                timingMultiplier = 256;
                sleepTime = (int)(refreshRate - (currentTime - timeArray[timingArrayIndex]) / 10L);

                if (sleepTime < gameMinThreadSleepTime)
                {
                    sleepTime = gameMinThreadSleepTime;
                }
            }

            try
            {
                Thread.Sleep(sleepTime);
            }
            catch (Exception) { }

            timeArray[timingArrayIndex] = currentTime;
            timingArrayIndex = (timingArrayIndex + 1) % 10;

            if (sleepTime > 1)
            {
                for (int timeIndex = 0; timeIndex < 10; timeIndex += 1)
                {
                    if (timeArray[timeIndex] != 0L)
                    {
                        timeArray[timeIndex] += sleepTime;
                    }
                }
            }

            int loopCount = 0;

            while (loopAccumulator < 256)
            {
                CheckInputs();
                loopAccumulator += timingMultiplier;

                loopCount += 1;

                if (loopCount > maxLoopCount)
                {
                    loopAccumulator = 0;
                    loadingAnimationCounter += 6;

                    if (loadingAnimationCounter > 25)
                    {
                        loadingAnimationCounter = 0;
                        keyF1Toggle = true;
                    }

                    break;
                }
            }

            loadingAnimationCounter -= 1;
        }

        public virtual void DrawWindow()
        {
        }

        public void Paint(GraphicsDevice graphicsDevice)
        {
            if (gameLoadingScreen == 2)
            {
                DrawLoadingScreen(gameLoadingPercentage, gameLoadingFileTitle);

                return;
            }
        }

        private void LoadLoadingScreen()
        {
            GameImage.AddFont(LoadFontFile("h11p.jf"));
            GameImage.AddFont(LoadFontFile("h12b.jf"));
            GameImage.AddFont(LoadFontFile("h12p.jf"));
            GameImage.AddFont(LoadFontFile("h13b.jf"));
            GameImage.AddFont(LoadFontFile("h14b.jf"));
            GameImage.AddFont(LoadFontFile("h16b.jf"));
            GameImage.AddFont(LoadFontFile("h20b.jf"));
            GameImage.AddFont(LoadFontFile("h24b.jf"));
        }

        private static sbyte[] LoadFontFile(string fileName)
        {
            string filePath = System.IO.Path.Combine(ApplicationPaths.FontsDirectory, fileName);
            return (sbyte[])(Array)System.IO.File.ReadAllBytes(filePath);
        }

        private void DrawLoadingScreen(int percentage, string fileTitle)
        {
            try
            {
                int xOffset = (appletWidth - 281) / 2;
                int yOffset = (appletHeight - 148) / 2;
                xOffset += 2;
                yOffset += 90;
                gameLoadingPercentage = percentage;
                gameLoadingFileTitle = fileTitle;
            }
            catch (Exception) { }
        }

        public void DrawLoadingBarText(int percentage, string statusText)
        {
            try
            {
                int xOffset = (appletWidth - 281) / 2;
                int yOffset = (appletHeight - 148) / 2;
                xOffset += 2;
                yOffset += 90;
                gameLoadingPercentage = percentage;
                gameLoadingFileTitle = statusText;
                int progressWidth = 277 * percentage / 100;
            }
            catch (Exception)
            {
            }
        }

        public virtual sbyte[] UnpackData(string filename, string fileTitle, int startPercentage)
        {
            logger.Debug(GameOperation.UnpackData, "Using default load.");
            int decompressedSize = 0;
            int compressedSize = 0;
            sbyte[] fileData = Link.GetFile(filename);

            if (fileData is null)
            {
                try
                {
                    logger.Debug(
                        GameOperation.UnpackData,
                        "Loading file.",
                        new LogInfo(GameLogInfoKey.FileName, fileTitle),
                        new LogInfo(GameLogInfoKey.LoadProgress, 0));
                    DrawLoadingBarText(startPercentage, "Loading " + fileTitle + " - 0%");
                    BinaryReader inputStream = new(DataOperations.OpenInputStream(filename));
                    sbyte[] headerBytes = [
                        inputStream.ReadSByte(), inputStream.ReadSByte(), inputStream.ReadSByte(),
                        inputStream.ReadSByte(), inputStream.ReadSByte(), inputStream.ReadSByte()
                    ];
                    decompressedSize = ((headerBytes[0] & 0xff) << 16) + ((headerBytes[1] & 0xff) << 8) + (headerBytes[2] & 0xff);
                    compressedSize = ((headerBytes[3] & 0xff) << 16) + ((headerBytes[4] & 0xff) << 8) + (headerBytes[5] & 0xff);

                    logger.Debug(
                        GameOperation.UnpackData,
                        "Loading file.",
                        new LogInfo(GameLogInfoKey.FileName, fileTitle),
                        new LogInfo(GameLogInfoKey.LoadProgress, 5));
                    DrawLoadingBarText(startPercentage, "Loading " + fileTitle + " - 5%");
#warning this could break stuff
                    int bytesRead = 6;
                    fileData = new sbyte[compressedSize];

                    while (bytesRead < compressedSize)
                    {
                        int chunkSize = compressedSize - bytesRead;

                        if (chunkSize > 1000)
                        {
                            chunkSize = 1000;
                        }

                        for (int chunkIndex = 0; chunkIndex < chunkSize; chunkIndex += 1)
                        {
                            fileData[bytesRead + chunkIndex] = inputStream.ReadSByte();
                        }

                        bytesRead += chunkSize;
                        logger.Debug(
                            GameOperation.UnpackData,
                            "Loading file.",
                            new LogInfo(GameLogInfoKey.FileName, fileTitle),
                            new LogInfo(GameLogInfoKey.LoadProgress, 5 + bytesRead * 95 / compressedSize));
                        DrawLoadingBarText(startPercentage, "Loading " + fileTitle + " - " + (5 + bytesRead * 95 / compressedSize) + "%");
                    }

                    inputStream.Close();
                }
                catch (IOException) { }
            }

            logger.Debug(
                GameOperation.UnpackData,
                "Unpacking file.",
                new LogInfo(GameLogInfoKey.FileName, fileTitle));
            DrawLoadingBarText(startPercentage, "Unpacking " + fileTitle);

            if (compressedSize != decompressedSize)
            {
                sbyte[] decompressedData = new sbyte[decompressedSize];
                DataFileDecrypter.UnpackData(decompressedData, decompressedSize, fileData, compressedSize, 0);

                return decompressedData;
            }
            else
            {
                return fileData;
            }
        }

        public Uri GetCodeBase() => default;

        public Uri GetDocumentBase() => default;

        public string GetParameter(string parameterName) => "";

        public TcpClient MakeSocket(string address, int port)
        {
            TcpClient socket = new(address, port)
            {
                SendTimeout = 30000,
                ReceiveTimeout = 30000,
                NoDelay = true
            };

            return socket;
        }

        public void MouseScroll(bool begin, int scrollAmount)
        {
            logger.Verbose(
                GameOperation.ProcessInput,
                "Mouse scroll event.",
                new LogInfo(GameLogInfoKey.ScrollBegin, begin),
                new LogInfo(GameLogInfoKey.ScrollAmount, scrollAmount));
        }

        public void InitGameApplet()
        {
            appletWidth = 512;
            appletHeight = 384;
            refreshRate = 60;
            maxLoopCount = 1000;
            timeArray = new long[10];
            gameLoadingScreen = 1;
            gameLoadingFileTitle = "Loading";
            keyLeftDown = false;
            keyRightDown = false;
            keyUpDown = false;
            keyDownDown = false;
            keySpaceDown = false;
            keyNMDown = false;
            gameMinThreadSleepTime = 1;
            keyF1Toggle = false;
            inputText = "";
            enteredInputText = "";
            pmText = "";
            enteredPMText = "";
        }

        public GameApplet(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            InitGameApplet();
        }

        private static readonly DateTime Jan1st1970 = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis() => (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;

        private int appletWidth;
        private int appletHeight;
        public Thread gameWindowThread;
        private int refreshRate;
        private int maxLoopCount;
        private long[] timeArray;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameApplet>();
        public static GameFrame gameFrame;
        public int runStatus;
        public int loadingAnimationCounter;
        public int mouseYOffset;
        public int gameLoadingScreen;
        public int gameLoadingPercentage;
        public string gameLoadingFileTitle;
        public static string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
        public bool keyLeftDown;
        public bool keyRightDown;
        public bool keyUpDown;
        public bool keyDownDown;
        public bool keySpaceDown;
        public bool keyNMDown;
        public int gameMinThreadSleepTime;
        public int mouseX;
        public int mouseY;
        public int mouseButton;
        public int lastMouseButton;
        public bool keyF1Toggle;
        public string inputText;
        public string enteredInputText;
        public string pmText;
        public string enteredPMText;

        public static int[][] bgPixels;
        public static Texture2D bgImage;

    }
}
