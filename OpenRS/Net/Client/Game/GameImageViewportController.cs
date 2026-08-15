namespace OpenRS.Net.Client.Game
{
    internal static class GameImageViewportController
    {
        internal static void Set(
            GameImage image,
            int x,
            int y,
            int width,
            int height)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (width > image.GameWidth)
            {
                width = image.GameWidth;
            }

            if (height > image.GameHeight)
            {
                height = image.GameHeight;
            }

            image.ImageX = x;
            image.ImageY = y;
            image.ImageWidth = width;
            image.ImageHeight = height;
        }

        internal static void Reset(GameImage image)
        {
            image.ImageX = 0;
            image.ImageY = 0;
            image.ImageWidth = image.GameWidth;
            image.ImageHeight = image.GameHeight;
        }
    }
}