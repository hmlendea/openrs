namespace OpenRS.Net.Client.Game
{
    internal static class GameImageStorageInitialiser
    {
        internal static void Initialise(
            GameImage image,
            int width,
            int height,
            int pictureCapacity)
        {
            image.ImageHeight = height;
            image.ImageWidth = width;
            image.Width = width;
            image.GameWidth = width;
            image.Height = height;
            image.GameHeight = height;
            image.Area = width * height;
            image.Pixels = new int[image.Area];
            image.PictureColours = new int[pictureCapacity][];
            image.HasTransparentBackground = new bool[pictureCapacity];
            image.PictureColourIndexes = new sbyte[pictureCapacity][];
            image.PictureColour = new int[pictureCapacity][];
            image.PictureWidth = new int[pictureCapacity];
            image.PictureHeight = new int[pictureCapacity];
            image.PictureAssumedWidth = new int[pictureCapacity];
            image.PictureAssumedHeight = new int[pictureCapacity];
            image.PictureOffsetX = new int[pictureCapacity];
            image.PictureOffsetY = new int[pictureCapacity];
        }
    }
}