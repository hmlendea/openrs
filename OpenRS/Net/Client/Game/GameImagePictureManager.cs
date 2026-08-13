using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePictureManager
    {
        private readonly GameImage gameImage;
        private readonly GameImagePictureCapture pictureCapture;
        private readonly GameImagePictureDataDecoder pictureDataDecoder;
        private readonly GameImagePaletteConverter paletteConverter;
        private readonly GameImageSleepSpriteDecoder sleepSpriteDecoder;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImagePictureManager>();

        internal GameImagePictureManager(GameImage gameImage)
        {
            this.gameImage = gameImage;
            pictureCapture = new GameImagePictureCapture(gameImage);
            pictureDataDecoder = new GameImagePictureDataDecoder(gameImage);
            paletteConverter = new GameImagePaletteConverter(gameImage);
            sleepSpriteDecoder = new GameImageSleepSpriteDecoder(gameImage, logger);
        }

        internal void CleanUp()
        {
            for (int pictureIndex = 0; pictureIndex < gameImage.PictureColours.Length; pictureIndex += 1)
            {
                gameImage.PictureColours[pictureIndex] = null;
                gameImage.PictureWidth[pictureIndex] = 0;
                gameImage.PictureHeight[pictureIndex] = 0;
                gameImage.PictureColourIndexes[pictureIndex] = null;
                gameImage.PictureColour[pictureIndex] = null;
            }
        }

        internal void UnpackImageData(int startIndex, sbyte[] imageData, sbyte[] metaData, int count)
            => pictureDataDecoder.Decode(startIndex, imageData, metaData, count);

        internal void SetSleepSprite(int pictureIndex, sbyte[] spriteData)
            => sleepSpriteDecoder.Decode(pictureIndex, spriteData);

        internal void ApplyImage(int pictureIndex)
            => paletteConverter.ConvertToIndexed(pictureIndex);

        internal void LoadImage(int pictureIndex)
            => paletteConverter.ConvertToDirect(pictureIndex);

        internal void FillPicture(int pictureIndex, int x, int y, int width, int height)
            => pictureCapture.CaptureColumnMajor(pictureIndex, x, y, width, height);

        internal void DrawImage(int pictureIndex, int x, int y, int width, int height)
            => pictureCapture.CaptureRowMajor(pictureIndex, x, y, width, height);
    }
}
