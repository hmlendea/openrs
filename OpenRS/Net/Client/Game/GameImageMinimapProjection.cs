using System;

namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageMinimapProjection
    {
        private static int RotationMask => 0xff;

        private static int ProjectionShift => 22;

        internal int BottomLeftX { get; }

        internal int BottomLeftY { get; }

        internal int BottomRightX { get; }

        internal int BottomRightY { get; }

        internal int MaximumY { get; }

        internal int MinimumY { get; }

        internal int Rotation { get; }

        internal int TopLeftX { get; }

        internal int TopLeftY { get; }

        internal int TopRightX { get; }

        internal int TopRightY { get; }

        private GameImageMinimapProjection(
            int bottomLeftX,
            int bottomLeftY,
            int bottomRightX,
            int bottomRightY,
            int maximumY,
            int minimumY,
            int rotation,
            int topLeftX,
            int topLeftY,
            int topRightX,
            int topRightY)
        {
            BottomLeftX = bottomLeftX;
            BottomLeftY = bottomLeftY;
            BottomRightX = bottomRightX;
            BottomRightY = bottomRightY;
            MaximumY = maximumY;
            MinimumY = minimumY;
            Rotation = rotation;
            TopLeftX = topLeftX;
            TopLeftY = topLeftY;
            TopRightX = topRightX;
            TopRightY = topRightY;
        }

        internal static GameImageMinimapProjection Calculate(
            GameImage gameImage,
            int centreX,
            int centreY,
            int pictureIndex,
            int rotation,
            int scale)
        {
            GameImageMinimapRotationTable.EnsureInitialised(gameImage);
            int pictureLeft = -gameImage.PictureAssumedWidth[pictureIndex] / 2;
            int pictureTop = -gameImage.PictureAssumedHeight[pictureIndex] / 2;

            if (gameImage.HasTransparentBackground[pictureIndex])
            {
                pictureLeft += gameImage.PictureOffsetX[pictureIndex];
                pictureTop += gameImage.PictureOffsetY[pictureIndex];
            }

            int pictureRight = pictureLeft + gameImage.PictureWidth[pictureIndex];
            int pictureBottom = pictureTop + gameImage.PictureHeight[pictureIndex];
            rotation &= RotationMask;
            int scaledSine = gameImage.CharacterRotationTable[rotation] * scale;
            int scaledCosine =
                gameImage.CharacterRotationTable[
                    rotation + GameImageMinimapRotationTable.CosineOffset] *
                scale;
            int topLeftX = centreX +
                (pictureTop * scaledSine + pictureLeft * scaledCosine >> ProjectionShift);
            int topLeftY = centreY +
                (pictureTop * scaledCosine - pictureLeft * scaledSine >> ProjectionShift);
            int topRightX = centreX +
                (pictureTop * scaledSine + pictureRight * scaledCosine >> ProjectionShift);
            int topRightY = centreY +
                (pictureTop * scaledCosine - pictureRight * scaledSine >> ProjectionShift);
            int bottomRightX = centreX +
                (pictureBottom * scaledSine + pictureRight * scaledCosine >> ProjectionShift);
            int bottomRightY = centreY +
                (pictureBottom * scaledCosine - pictureRight * scaledSine >> ProjectionShift);
            int bottomLeftX = centreX +
                (pictureBottom * scaledSine + pictureLeft * scaledCosine >> ProjectionShift);
            int bottomLeftY = centreY +
                (pictureBottom * scaledCosine - pictureLeft * scaledSine >> ProjectionShift);
            int minimumY = topLeftY;
            int maximumY = topLeftY;
            minimumY = Math.Min(minimumY, topRightY);
            maximumY = Math.Max(maximumY, topRightY);
            minimumY = Math.Min(minimumY, bottomRightY);
            maximumY = Math.Max(maximumY, bottomRightY);
            minimumY = Math.Min(minimumY, bottomLeftY);
            maximumY = Math.Max(maximumY, bottomLeftY);

            return new GameImageMinimapProjection(
                bottomLeftX,
                bottomLeftY,
                bottomRightX,
                bottomRightY,
                maximumY,
                minimumY,
                rotation,
                topLeftX,
                topLeftY,
                topRightX,
                topRightY);
        }
    }
}