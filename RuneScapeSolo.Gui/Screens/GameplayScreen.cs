using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RuneScapeSolo.Graphics.Primitives;
using RuneScapeSolo.Gui.GuiElements;

namespace RuneScapeSolo.Gui.Screens
{
    /// <summary>
    /// Gameplay screen.
    /// </summary>
    public class GameplayScreen : Screen
    {
        /// <summary>
        /// Gets or sets the game client.
        /// </summary>
        /// <value>The game client.</value>
        public GuiGame GameClient { get; set; }

        /// <summary>
        /// Gets or sets the minimap.
        /// </summary>
        /// <value>The minimap.</value>
        public GuiSideBar SideBar { get; set; }

        public GuiChatPanel ChatPanel { get; set; }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public override void LoadContent()
        {
            ChatPanel = new GuiChatPanel();

            SideBar.Enabled = false;
            SideBar.Visible = false;

            GuiManager.Instance.GuiElements.Add(GameClient);
            GuiManager.Instance.GuiElements.Add(SideBar);
            GuiManager.Instance.GuiElements.Add(ChatPanel);

            SetChildrenProperties();
            base.LoadContent();

            SideBar.AssociateGameClient(ref GameClient.gameClient);
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (GameClient.gameClient.loggedIn)
            {
                SideBar.Enabled = true;
                SideBar.Visible = true;
            }
            else
            {
                SideBar.Enabled = false;
                SideBar.Visible = false;
            }

            SetChildrenProperties();
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

        protected void SetChildrenProperties()
        {
            GameClient.Size = new Size2D(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                (int)(ScreenManager.Instance.Size.Height * 0.8));

            SideBar.Size = new Size2D(SideBar.Size.Width, ScreenManager.Instance.Size.Height);
            SideBar.Location = new Point2D(ScreenManager.Instance.Size.Width - SideBar.Size.Width, 0);

            ChatPanel.Size = new Size2D(
                ScreenManager.Instance.Size.Width - SideBar.Size.Width,
                ScreenManager.Instance.Size.Height - GameClient.Size.Height);
            ChatPanel.Location = new Point2D(0, ScreenManager.Instance.Size.Height - ChatPanel.Size.Height);
        }
    }
}

