using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OpenRS.Net.Client
{
    public static class GraphicsDeviceExtensions
    {
        private static readonly bool sbBegin;
        private static Color defaultColor { get; set; }
        private static SpriteFont defaultFont { get; set; }
        public static void FillRect(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            if (dummyTexture is null)
            {
                CreateDummyTexture(spriteBatch);
            }

            try
            {
                //   spriteBatch.BeginSafe();
                //spriteBatch.Draw(dummyTexture, rect, color);
                //  spriteBatch.EndSafe();
            }
            catch { }
        }

        public static void SetColor(this SpriteBatch spriteBatch, Color color)
        {
            defaultColor = color;
        }
        public static void SetFont(this SpriteBatch spriteBatch, SpriteFont font)
        {
            defaultFont = font;
        }

        public static bool BeginIsActive(this SpriteBatch spriteBatch)
        {
            return sbBegin;
        }

        public static void DrawString(this SpriteBatch spriteBatch, string text, int x, int y)
        {
            if (defaultFont is not null)
            {
                //  spriteBatch.BeginSafe();
                //    if (spriteBatch.BeginIsActive())
                // spriteBatch.DrawString(defaultFont, text, new Vector2(x, y), defaultColor);
                //  spriteBatch.EndSafe();
            }
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
        {
            if (dummyTexture is null)
            {
                CreateDummyTexture(spriteBatch);
            }

            float length = (end - start).Length();
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            // spriteBatch.BeginSafe();

            //  if (!spriteBatch.BeginIsActive()) return;

            // spriteBatch.Draw(dummyTexture, start, null, color, rotation, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
            // spriteBatch.EndSafe();
        }

        public static void FillRect(this SpriteBatch spriteBatch, int x, int y, int w, int h, Color color)
        {
            //FillRect(spriteBatch, x, y, w, h, color);
            if (dummyTexture is null)
            {
                CreateDummyTexture(spriteBatch);
            }

            spriteBatch.Draw(dummyTexture, new Rectangle(x, y, w, h), color);
        }

        public static void DrawGradient(this SpriteBatch spriteBatch, int x, int y, int x2, int y2, Color color, Color color2)
        {
            if (dummyTexture is null)
            {
                CreateDummyTexture(spriteBatch);
            }

            //  DrawLine(spriteBatch)
            //int stepX = x2 - x;
            int stepY = y2 - y;

            int stepR = (color2.R - color.R) / stepY;
            int stepG = (color2.G - color.G) / stepY;
            int stepB = (color2.B - color.B) / stepY;
            int stepA = (color2.A - color.A) / stepY;

          //  MathHelper.s
            MathHelper.Lerp(color2.PackedValue, color.PackedValue, 0);

            //if (stepY == stepX)
            {
                int sR=0, sG=0, sB=0, sA=0;
                for (int j = 0; j < stepY; j += 1)
                {
                    int currentY = y + j;
                    int currentX = x;

                    sR += stepR;
                    sG += stepG;
                    sB += stepB;
                    sA += stepA;

                    Color currentColor = new(sR, sG, sB, sA);
                    spriteBatch.DrawLine(new Vector2(currentX, currentY), new Vector2(x2, currentY), currentColor);
                }
            }
            //else
            //{
            //    throw new NotImplementedException("Only rectangular gradients implemented.");
            //}

        }

        public static void DrawRect(this SpriteBatch spriteBatch, int x, int y, int w, int h, Color color)
        {
            DrawRect(spriteBatch, new Rectangle(x, y, w, h), color);
        }
        public static void DrawRect(this SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            if (dummyTexture is null)
            {
                CreateDummyTexture(spriteBatch);
            }

            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, 1), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
            spriteBatch.Draw(dummyTexture, new Rectangle(rectangle.Right, rectangle.Top, 1, rectangle.Height + 1), color);
        }

        private static void CreateDummyTexture(SpriteBatch spriteBatch)
        {
            dummyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            dummyTexture.SetData([Color.White]);
        }

        private static Texture2D dummyTexture;
    }
}
