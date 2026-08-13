using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageSpriteRenderer
    {
        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageSpriteRenderer>();

        internal GameImageSpriteRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawPicture(int x, int y, int pictureIndex)
        {
            GameImageSpriteClip spriteClip = GameImageSpriteClip.Calculate(
                gameImage,
                x,
                y,
                pictureIndex);

            if (!spriteClip.IsVisible)
            {
                return;
            }

            if (gameImage.PictureColours[pictureIndex] is null)
            {
                GameImageSpriteBlitter.DrawSpriteTextured(
                    gameImage.Pixels,
                    gameImage.PictureColourIndexes[pictureIndex],
                    gameImage.PictureColour[pictureIndex],
                    spriteClip.SourceOffset,
                    spriteClip.DestinationOffset,
                    spriteClip.Width,
                    spriteClip.Height,
                    spriteClip.DestinationRowStride,
                    spriteClip.SourceRowStride,
                    spriteClip.RowStep);
            }
            else
            {
                GameImageSpriteBlitter.DrawSpriteOpaque(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    spriteClip.SourceOffset,
                    spriteClip.DestinationOffset,
                    spriteClip.Width,
                    spriteClip.Height,
                    spriteClip.DestinationRowStride,
                    spriteClip.SourceRowStride,
                    spriteClip.RowStep);
            }
        }

        internal void DrawPicture(int x, int y, int pictureIndex, int blendFactor)
        {
            GameImageSpriteClip spriteClip = GameImageSpriteClip.Calculate(
                gameImage,
                x,
                y,
                pictureIndex);

            if (!spriteClip.IsVisible)
            {
                return;
            }

            if (gameImage.PictureColours[pictureIndex] is null)
            {
                GameImageSpriteBlitter.DrawSpriteColorShiftedTextured(
                    gameImage.Pixels,
                    gameImage.PictureColourIndexes[pictureIndex],
                    gameImage.PictureColour[pictureIndex],
                    spriteClip.SourceOffset,
                    spriteClip.DestinationOffset,
                    spriteClip.Width,
                    spriteClip.Height,
                    spriteClip.DestinationRowStride,
                    spriteClip.SourceRowStride,
                    spriteClip.RowStep,
                    blendFactor);
            }
            else
            {
                GameImageSpriteBlitter.DrawSpriteColorShifted(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    spriteClip.SourceOffset,
                    spriteClip.DestinationOffset,
                    spriteClip.Width,
                    spriteClip.Height,
                    spriteClip.DestinationRowStride,
                    spriteClip.SourceRowStride,
                    spriteClip.RowStep,
                    blendFactor);
            }
        }

        internal void DrawEntity(int x, int y, int width, int height, int index)
        {
            try
            {
                GameImageEntityClip entityClip = GameImageEntityClip.Calculate(
                    gameImage,
                    x,
                    y,
                    width,
                    height,
                    index);
                GameImageSpriteBlitter.DrawSpriteTransparent(
                    gameImage.Pixels,
                    gameImage.PictureColours[index],
                    entityClip.SourceX,
                    entityClip.SourceY,
                    entityClip.DestinationOffset,
                    entityClip.DestinationRowStride,
                    entityClip.Width,
                    entityClip.Height,
                    entityClip.ScaleX,
                    entityClip.ScaleY,
                    entityClip.SourceWidth,
                    entityClip.RowStep);
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderEntity, "Error in sprite clipping routine.");
            }
        }
    }
}
