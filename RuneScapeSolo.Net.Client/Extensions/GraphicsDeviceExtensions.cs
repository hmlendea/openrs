using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RuneScapeSolo.Net.Client.Extensions
{
    public static class GraphicsDeviceExtensions
    {
        public static void fillRect(this SpriteBatch spriteBatch, int x, int y, int w, int h, Color color)
        {
            if (dummyTexture == null)
            {
                createDummyTexture(spriteBatch);
            }

            spriteBatch.Draw(dummyTexture, new Rectangle(x, y, w, h), color);
        }

        public static void drawRect(this SpriteBatch spriteBatch, int x, int y, int w, int h, Color color)
        {
            drawRect(spriteBatch, new Rectangle(x, y, w, h), color);
        }

        /// <summary>
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="color">The draw color.</param>
        public static void drawRect(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            if (dummyTexture == null)
            {
                createDummyTexture(spriteBatch);
            }

            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height + 1), color);
        }

        static void createDummyTexture(SpriteBatch spriteBatch)
        {
            dummyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            dummyTexture.SetData(new[] { Color.White });
        }

        static Texture2D dummyTexture;
    }
}
