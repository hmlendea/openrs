using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageMinimapRenderer
    {
        private readonly GameImage gameImage;
        private readonly GameImageMinimapRasteriser rasteriser;
        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageMinimapRenderer>();

        internal GameImageMinimapRenderer(GameImage gameImage)
        {
            this.gameImage = gameImage;
            rasteriser = new GameImageMinimapRasteriser(gameImage);
        }

        internal void DrawMinimapPic(int centreX, int centreY, int pictureIndex, int rotation, int scale)
        {
            try
            {
                DrawMinimapPicInternal(centreX, centreY, pictureIndex, rotation, scale);
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderMinimap, "Error drawing minimap picture.", exception);
                throw;
            }
        }

        private void DrawMinimapPicInternal(
            int centreX,
            int centreY,
            int pictureIndex,
            int rotation,
            int scale)
        {
            GameImageMinimapProjection projection = GameImageMinimapProjection.Calculate(
                gameImage,
                centreX,
                centreY,
                pictureIndex,
                rotation,
                scale);

            GameImageMinimapDrawCounter.Update(scale, projection.Rotation);

            int scanlineMinY = projection.MinimumY;
            int scanlineMaxY = projection.MaximumY;

            if (scanlineMinY < gameImage.ImageY)
            {
                scanlineMinY = gameImage.ImageY;
            }

            if (scanlineMaxY > gameImage.ImageHeight)
            {
                scanlineMaxY = gameImage.ImageHeight;
            }

            rasteriser.Rasterise(projection, pictureIndex, scanlineMinY, scanlineMaxY);
        }
    }
}
