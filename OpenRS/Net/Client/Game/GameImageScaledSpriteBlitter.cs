using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageScaledSpriteBlitter
    {
        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageScaledSpriteBlitter>();

        internal static void Draw<TColourSource, TPixelComposer>(
            int[] pixels,
            TColourSource colourSource,
            int srcX,
            int srcY,
            int dstOffset,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int xPosFixed,
            int xPosStep,
            int interlaceFlag,
            int imageX,
            int imageWidth,
            int gameWidth,
            TPixelComposer pixelComposer)
            where TColourSource : struct, IGameImageSpriteColourSource
            where TPixelComposer : struct, IGameImageSampledPixelComposer
        {
            try
            {
                GameImageScaledTintedSpriteBlitter.Draw(
                    pixels,
                    colourSource,
                    srcX,
                    srcY,
                    dstOffset,
                    width,
                    height,
                    xStep,
                    yStep,
                    srcWidth,
                    xPosFixed,
                    xPosStep,
                    interlaceFlag,
                    imageX,
                    imageWidth,
                    gameWidth,
                    pixelComposer);
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.",
                    exception);

                throw;
            }
        }
    }
}
