using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public GuiMinimap Minimap { get; set; }
        
        /// <summary>
        /// Loads the content.
        /// </summary>
        public override void LoadContent()
        {
            GuiManager.Instance.GuiElements.Add(GameClient);
            GuiManager.Instance.GuiElements.Add(Minimap);

            base.LoadContent();

            Minimap.AssociateGameClient(ref GameClient.gameClient);
        }

        /// <summary>
        /// Update the content.
        /// </summary>
        /// <returns>The update.</returns>
        /// <param name="gameTime">Game time.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the content on the specified spriteBatch.
        /// </summary>
        /// <returns>The draw.</returns>
        /// <param name="spriteBatch">Sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Minimap.Draw(spriteBatch);
        }
    }
}

