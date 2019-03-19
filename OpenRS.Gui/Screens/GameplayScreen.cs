﻿using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui;
using NuciXNA.Gui.Screens;
using NuciXNA.Primitives;

using OpenRS.Gui.GuiElements;
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
        public override void LoadContent()
        {
            SideBar = new GuiSideBar();
            ChatPanel = new GuiChatPanel();
            GuiGame = new GuiGame();

            SideBar.Enabled = false;
            SideBar.Visible = false;

            GuiManager.Instance.GuiElements.Add(GuiGame);
            GuiManager.Instance.GuiElements.Add(SideBar);
            GuiManager.Instance.GuiElements.Add(ChatPanel);

            gameClient = GameClient.CreateGameClient(username, password, 640, 480);
            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();
            gameThread = new Thread(gameClient.run);
            gameThread.Start();

            GuiGame.AssociateGameClient(ref gameClient);

            base.LoadContent();

            SideBar.AssociateGameClient(ref gameClient);
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameClient.loggedIn)
            {
                SideBar.Enabled = true;
                SideBar.Visible = true;
            }
            else
            {
                SideBar.Enabled = false;
                SideBar.Visible = false;
            }
        }

        /// <summary>
        /// Draw the content on the specified spriteBatch.
        /// </summary>
        /// <returns>The draw.</returns>
        /// <param name="spriteBatch">Sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void SetChildrenProperties()
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

        protected override void RegisterEvents()
        {
            gameClient.OnChatMessageReceived += GameClient_OnChatMessageReceived;
        }

        protected override void UnregisterEvents()
        {
            gameClient.OnChatMessageReceived -= GameClient_OnChatMessageReceived;
        }

        void GameClient_OnChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            ChatPanel.AddMessage(e.Message);
        }
    }
}
