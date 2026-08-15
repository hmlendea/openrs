using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageCharacterRenderer
    {
        private static int DefaultColour => 0xffffff;

        private readonly GameImageCharacterColourRenderer colourRenderer;
        private readonly GameImageFlippedSpriteRenderer flippedSpriteRenderer;
        private readonly GameImage gameImage;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageCharacterRenderer>();

        internal GameImageCharacterRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
            colourRenderer = new GameImageCharacterColourRenderer(gameImage);
            flippedSpriteRenderer = new GameImageFlippedSpriteRenderer(gameImage);
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
                flippedSpriteRenderer.Draw(
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
                flippedSpriteRenderer.Draw(
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

                colourRenderer.Draw(
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

    }
}
