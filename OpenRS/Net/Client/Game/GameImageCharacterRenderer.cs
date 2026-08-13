using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageCharacterRenderer
    {
        private static int DefaultColour => 0xffffff;

        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageCharacterRenderer>();

        internal GameImageCharacterRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
        }

        internal void DrawTransparentImage(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint)
        {
            try
            {
                DrawFlippedSpriteInternal(
                    x,
                    y,
                    drawWidth,
                    drawHeight,
                    pictureIndex,
                    colourTint,
                    false);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        internal void DrawCharacterLegs(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint)
        {
            try
            {
                DrawFlippedSpriteInternal(
                    x,
                    y,
                    drawWidth,
                    drawHeight,
                    pictureIndex,
                    colourTint,
                    true);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderCharacter, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        internal void DrawImage(
            int x,
            int y,
            int width,
            int height,
            int pictureIndex,
            int primaryColour,
            int secondaryColour,
            int shearFactor,
            bool isFlipped)
        {
            try
            {
                if (primaryColour == 0)
                {
                    primaryColour = DefaultColour;
                }

                if (secondaryColour == 0)
                {
                    secondaryColour = DefaultColour;
                }

                GameImageCharacterClip characterClip = GameImageCharacterClip.Calculate(
                    gameImage,
                    x,
                    y,
                    width,
                    height,
                    pictureIndex,
                    shearFactor,
                    isFlipped);

                if (secondaryColour == DefaultColour)
                {
                    DrawPrimaryColour(pictureIndex, primaryColour, characterClip);
                    return;
                }

                DrawPrimaryAndSecondaryColours(
                    pictureIndex,
                    primaryColour,
                    secondaryColour,
                    characterClip);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.", exception);
                throw;
            }
        }

        private void DrawPrimaryColour(
            int pictureIndex,
            int primaryColour,
            GameImageCharacterClip characterClip)
        {
            if (gameImage.PictureColours[pictureIndex] is not null)
            {
                GameImageScaledSpriteBlitter.DrawSpriteFlatShaded(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    0,
                    characterClip.SourceX,
                    characterClip.SourceY,
                    characterClip.DestinationOffset,
                    characterClip.Width,
                    characterClip.Height,
                    characterClip.ScaleX,
                    characterClip.ScaleY,
                    characterClip.SourceWidth,
                    primaryColour,
                    characterClip.XPosition,
                    characterClip.XStep,
                    characterClip.ScanlineMode,
                    gameImage.ImageX,
                    gameImage.ImageWidth,
                    gameImage.GameWidth);

                return;
            }

            GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTextured(
                gameImage.Pixels,
                gameImage.PictureColourIndexes[pictureIndex],
                gameImage.PictureColour[pictureIndex],
                0,
                characterClip.SourceX,
                characterClip.SourceY,
                characterClip.DestinationOffset,
                characterClip.Width,
                characterClip.Height,
                characterClip.ScaleX,
                characterClip.ScaleY,
                characterClip.SourceWidth,
                primaryColour,
                characterClip.XPosition,
                characterClip.XStep,
                characterClip.ScanlineMode,
                gameImage.ImageX,
                gameImage.ImageWidth,
                gameImage.GameWidth);
        }

        private void DrawPrimaryAndSecondaryColours(
            int pictureIndex,
            int primaryColour,
            int secondaryColour,
            GameImageCharacterClip characterClip)
        {
            if (gameImage.PictureColours[pictureIndex] is not null)
            {
                GameImageScaledSpriteBlitter.DrawSpriteFlatShadedAlt(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    0,
                    characterClip.SourceX,
                    characterClip.SourceY,
                    characterClip.DestinationOffset,
                    characterClip.Width,
                    characterClip.Height,
                    characterClip.ScaleX,
                    characterClip.ScaleY,
                    characterClip.SourceWidth,
                    primaryColour,
                    secondaryColour,
                    characterClip.XPosition,
                    characterClip.XStep,
                    characterClip.ScanlineMode,
                    gameImage.ImageX,
                    gameImage.ImageWidth,
                    gameImage.GameWidth);

                return;
            }

            GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTexturedAlt(
                gameImage.Pixels,
                gameImage.PictureColourIndexes[pictureIndex],
                gameImage.PictureColour[pictureIndex],
                0,
                characterClip.SourceX,
                characterClip.SourceY,
                characterClip.DestinationOffset,
                characterClip.Width,
                characterClip.Height,
                characterClip.ScaleX,
                characterClip.ScaleY,
                characterClip.SourceWidth,
                primaryColour,
                secondaryColour,
                characterClip.XPosition,
                characterClip.XStep,
                characterClip.ScanlineMode,
                gameImage.ImageX,
                gameImage.ImageWidth,
                gameImage.GameWidth);
        }

        private void DrawFlippedSpriteInternal(
            int x,
            int y,
            int drawWidth,
            int drawHeight,
            int pictureIndex,
            int colourTint,
            bool isColourShifted)
        {
            GameImageEntityClip entityClip = GameImageEntityClip.Calculate(
                gameImage,
                x,
                y,
                drawWidth,
                drawHeight,
                pictureIndex);

            if (isColourShifted)
            {
                GameImageSpriteBlitter.DrawSpriteFlippedColorShifted(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    entityClip.SourceX,
                    entityClip.SourceY,
                    entityClip.DestinationOffset,
                    entityClip.DestinationRowStride,
                    entityClip.Width,
                    entityClip.Height,
                    entityClip.ScaleX,
                    entityClip.ScaleY,
                    entityClip.SourceWidth,
                    entityClip.RowStep,
                    colourTint);

                return;
            }

            GameImageSpriteBlitter.DrawSpriteFlipped(
                gameImage.Pixels,
                gameImage.PictureColours[pictureIndex],
                entityClip.SourceX,
                entityClip.SourceY,
                entityClip.DestinationOffset,
                entityClip.DestinationRowStride,
                entityClip.Width,
                entityClip.Height,
                entityClip.ScaleX,
                entityClip.ScaleY,
                entityClip.SourceWidth,
                entityClip.RowStep,
                colourTint);
        }
    }
}
