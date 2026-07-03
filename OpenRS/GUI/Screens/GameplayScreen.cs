using System;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui;
using NuciXNA.Gui.Screens;
using NuciXNA.Primitives;

using OpenRS.Gui.Controls;
using OpenRS.Net.Client;
using OpenRS.Net.Client.Events;

namespace OpenRS.Gui.Screens
{
    /// <summary>
    /// Gameplay screen.
    /// </summary>
    public class GameplayScreen : Screen
    {
        GameClient gameClient;
        Thread gameThread;

        /// <summary>
        /// Gets or sets the minimap.
        /// </summary>
        /// <value>The minimap.</value>
        public GuiSideBar SideBar { get; set; }

        public GuiChatPanel ChatPanel { get; set; }

        readonly string username;
        readonly string password;

        /// <summary>
        /// Gets or sets the game client.
        /// </summary>
        /// <value>The game client.</value>
        public GuiGame GuiGame { get; set; }

        public GameplayScreen(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        protected override void DoLoadContent()
        {
            gameClient = GameClient.CreateGameClient(username, password, 640, 480);
            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();
            gameThread = new Thread(gameClient.run);
            gameThread.Start();

            SideBar = new GuiSideBar(gameClient);
            ChatPanel = new GuiChatPanel();
            GuiGame = new GuiGame(gameClient);

            GuiManager.Instance.RegisterControls(GuiGame, SideBar, ChatPanel);

            RegisterEvents();
            SetChildrenProperties();
        }

        /// <summary>
        /// Unloads the content.
        /// </summary>
        protected override void DoUnloadContent()
        {
            UnregisterEvents();
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <param name="gameTime">Game time.</param>
        protected override void DoUpdate(GameTime gameTime)
        {
            if (gameClient.loggedIn)
            {
                if (!SideBar.IsEnabled)
                {
                    SideBar.Enable();
                    SideBar.Show();
                }

                SetChildrenProperties();
            }
            else
            {
                SideBar.Disable();
                SideBar.Hide();
            }
        }

        /// <summary>
        /// Draw the content on the specified spriteBatch.
        /// </summary>
        /// <returns>The draw.</returns>
        /// <param name="spriteBatch">Sprite batch.</param>
        protected override void DoDraw(SpriteBatch spriteBatch)
        {

        }
        
        /// <summary>
        /// Registers the events.
        /// </summary>
        void RegisterEvents()
        {
            ContentLoaded += OnContentLoaded;
            gameClient.OnChatMessageReceived += OnGameClientChatMessageReceived;
        }

        /// <summary>
        /// Unregisters the events.
        /// </summary>
        void UnregisterEvents()
        {
            ContentLoaded -= OnContentLoaded;
            gameClient.OnChatMessageReceived -= OnGameClientChatMessageReceived;
        }

        void SetChildrenProperties()
        {
            SideBar.Size = new Size2D(240, ScreenManager.Instance.Size.Height);
            SideBar.Location = new Point2D(ScreenManager.Instance.Size.Width - SideBar.Size.Width, 0);

            ChatPanel.Size = new Size2D(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                (int)(ScreenManager.Instance.Size.Height * 0.25));
            ChatPanel.Location = new Point2D(0, ScreenManager.Instance.Size.Height - ChatPanel.Size.Height);

            GuiGame.Size = new Size2D(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                ScreenManager.Instance.Size.Height - ChatPanel.Size.Height);
        }

        void OnContentLoaded(object sender, EventArgs e)
        {
            SideBar.Disable();
            SideBar.Hide();
        }

        void OnGameClientChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            ChatPanel.AddMessage(e.Message);
        }
    }
}
