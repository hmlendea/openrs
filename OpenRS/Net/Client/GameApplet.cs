using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client
{
    public class GameApplet// : java.applet.Applet
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
            Console.WriteLine("Started application");
            appletWidth = width;
            appletHeight = height;
            gameFrame = new GameFrame(this, width, height, title, resizable, false);
            gameLoadingScreen = 1;

            InitGameApplet();
        }


        public void SetRefreshRate(int rate)
        {
            refreshRate = 1000 / rate;
        }        public void ResetTimings()
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
            Console.WriteLine("Started applet");
            appletWidth = 512;
            appletHeight = 344;
            gameLoadingScreen = 1;
            DataOperations.CodeBase = GetCodeBase();
            // startThread(this);
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
                Console.WriteLine("2 seconds expired, forcing kill");
                CloseProgram();

                if (gameWindowThread is not null)
                {
                    gameWindowThread.Interrupt();
                    gameWindowThread = null;
                }
            }
        }



        public void CloseProgram()
        {
            runStatus = -2;
            Console.WriteLine("Closing program");
            Close();

            try
            {
                Thread.Sleep(1000);
            }
            catch (Exception) { }
        }

        //Component getGameComponent() {
        //    if(gameFrame is not null)
        //        return gameFrame;
        //    else
        //        return this;
        //}

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
        public bool DrawIsNecessary = false;

        public void OnDrawDone() => DrawIsNecessary = true;

        public int gameTimingArrayIndex = 0;
        public int gameTimingMultiplier = 256;
        public int gameThreadSleepTime = 1;
        public int gameLoopAccumulator = 0;

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
                timingMultiplier = (int)((long)(2560 * refreshRate) / (currentTime - timeArray[timingArrayIndex]));
            }

            if (timingMultiplier < 25)
            {
                timingMultiplier = 25;
            }

            if (timingMultiplier > 256)
            {
                timingMultiplier = 256;
                sleepTime = (int)((long)refreshRate - (currentTime - timeArray[timingArrayIndex]) / 10L);

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
            sbyte[] bytes = UnpackData("fonts.jag", "Game fonts", 0);
            GameImage.AddFont(DataOperations.LoadData("h11p.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h12b.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h12p.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h13b.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h14b.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h16b.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h20b.jf", 0, bytes));
            GameImage.AddFont(DataOperations.LoadData("h24b.jf", 0, bytes));
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

        //public void DrawString(String arg1, int arg3, int arg4, Color color)
        //{
        //    //Object obj;
        //    //if (gameFrame is null)
        //    //    obj = this;
        //    //else
        //    //    obj = gameFrame;
        //    //var fontmetrics = arg2.MeasureString(arg1);//((Component)(obj)).getFontMetrics(arg2);
        //    //fontmetrics.stringWidth(arg1);
        //    //arg0.SetFont(arg2);
        //    //arg0.DrawString(arg1, arg3 - fontmetrics.stringWidth(arg1) / 2, arg4 + fontmetrics.getHeight() / 4);

        //    //GameImage.stringsToDraw.Add(new StringDraw
        //    //{
        //    //    font = arg2,
        //    //    text = arg1,
        //    //    pos = new Vector2(arg3 - fontmetrics.X / 2, arg4 + fontmetrics.Y / 4),
        //    //    forecolor = color
        //    //});

        //    //spriteBatch.BeginSafe();
        //    //spriteBatch.DrawString(arg2, arg1, new Vector2(fontmetrics.X / 2, arg4 + fontmetrics.Y / 4), color);
        //    //spriteBatch.EndSafe();
        //}

        public virtual sbyte[] UnpackData(string filename, string fileTitle, int startPercentage)
        {
            Console.WriteLine("Using default load");
            int decompressedSize = 0;
            int compressedSize = 0;
            sbyte[] fileData = Link.GetFile(filename);

            if (fileData is null)
            {
                try
                {
                    Console.WriteLine("Loading " + fileTitle + " - 0%");
                    DrawLoadingBarText(startPercentage, "Loading " + fileTitle + " - 0%");
                    BinaryReader inputStream = new(DataOperations.OpenInputStream(filename));
                    sbyte[] headerBytes = [
                        inputStream.ReadSByte(), inputStream.ReadSByte(), inputStream.ReadSByte(),
                        inputStream.ReadSByte(), inputStream.ReadSByte(), inputStream.ReadSByte()
                    ];
                    decompressedSize = ((headerBytes[0] & 0xff) << 16) + ((headerBytes[1] & 0xff) << 8) + (headerBytes[2] & 0xff);
                    compressedSize = ((headerBytes[3] & 0xff) << 16) + ((headerBytes[4] & 0xff) << 8) + (headerBytes[5] & 0xff);

                    Console.WriteLine("Loading " + fileTitle + " - 5%");
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
                        Console.WriteLine("Loading " + fileTitle + " - " + (5 + bytesRead * 95 / compressedSize) + "%");
                        DrawLoadingBarText(startPercentage, "Loading " + fileTitle + " - " + (5 + bytesRead * 95 / compressedSize) + "%");
                    }

                    inputStream.Close();
                }
                catch (IOException) { }
            }

            Console.WriteLine("Unpacking " + fileTitle);
            DrawLoadingBarText(startPercentage, "Unpacking " + fileTitle);

            if (compressedSize != decompressedSize)
            {
                sbyte[] decompressedData = new sbyte[decompressedSize];
                DataFileDecrypter.UnpackData(decompressedData, decompressedSize, fileData, compressedSize, 0);

                return decompressedData;
            }
            else
            {
                // return UnpackData(filename, fileTitle, startPercentage); // fileData;
                return fileData;
            }
        }

        //public Texture2D createImage(int i, int k)
        //{
        //    //if (gameFrame is not null)
        //    //    return gameFrame.createImage(i, k);
        //    //else
        //    //    return super.createImage(i, k);

        //    return new Texture2D(this.graphics, i, k);
        //}

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
            Console.WriteLine("mouseWheel(" + begin + ", " + scrollAmount + ")");
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
            //gameLoadingFont = loadingFont;//new Font("TimesRoman", 0, 15);
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

        //GameApplet baseApplet;
        private int appletWidth;
        private int appletHeight;
        public Thread gameWindowThread;
        private int refreshRate;
        private int maxLoopCount;
        private long[] timeArray;
        public static GameFrame gameFrame = null;
        public int runStatus;
        public int loadingAnimationCounter;
        public int mouseYOffset = 0;
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

        public static int[][] bgPixels = null;
        public static Texture2D bgImage = null;

    }
}
