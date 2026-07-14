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
    public sealed class GameplayScreen(string username, string password) : Screen
    {
        private GameClient gameClient;
        private Thread gameThread;

        public GuiSideBar SideBar { get; set; }

        public GuiChatPanel ChatPanel { get; set; }

        public GuiGame GuiGame { get; set; }

        protected override void DoLoadContent()
        {
            gameClient = GameClient.CreateGameClient(username, password, 512, 334);
            gameClient.gameMinThreadSleepTime = 10;
            gameClient.Start();
            gameThread = new Thread(gameClient.Run);
            gameThread.Start();

            SideBar = new GuiSideBar(gameClient)
            {
                Location = new(
                    ScreenManager.Instance.Size.Width - 240,
                    0),
            };
            ChatPanel = new GuiChatPanel();
            GuiGame = new GuiGame(gameClient);

            GuiManager.Instance.RegisterControls(GuiGame, SideBar, ChatPanel);

            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

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

        protected override void DoDraw(SpriteBatch spriteBatch)
        {
        }

        private void RegisterEvents()
        {
            ContentLoaded += OnContentLoaded;
            gameClient.OnChatMessageReceived += OnGameClientChatMessageReceived;
        }

        private void UnregisterEvents()
        {
            ContentLoaded -= OnContentLoaded;
            gameClient.OnChatMessageReceived -= OnGameClientChatMessageReceived;
        }

        private void SetChildrenProperties()
        {
            SideBar.Size = new(240, ScreenManager.Instance.Size.Height);
            SideBar.Location = new(ScreenManager.Instance.Size.Width - SideBar.Size.Width, 0);

            ChatPanel.Size = new(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                (int)(ScreenManager.Instance.Size.Height * 0.25));
            ChatPanel.Location = new(0, ScreenManager.Instance.Size.Height - ChatPanel.Size.Height);

            GuiGame.Size = new(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                ScreenManager.Instance.Size.Height - ChatPanel.Size.Height);
        }

        private void OnContentLoaded(object sender, EventArgs e)
        {
            SideBar.Disable();
            SideBar.Hide();
        }

        private void OnGameClientChatMessageReceived(object sender, ChatMessageEventArgs e) => ChatPanel.AddMessage(e.Message);
    }
}
