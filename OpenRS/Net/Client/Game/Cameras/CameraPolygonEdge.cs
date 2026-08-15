namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraPolygonEdge
    {
        private static int NotSetSentinel => 0xbc614e;

        internal int CurrentShade { get; }

        internal int CurrentX { get; }

        internal int MaximumY { get; }

        internal int MinimumY { get; }

        internal int ShadeSlope { get; }

        internal int XSlope { get; }

        private CameraPolygonEdge(
            int currentShade,
            int currentX,
            int maximumY,
            int minimumY,
            int shadeSlope,
            int xSlope)
        {
            CurrentShade = currentShade;
            CurrentX = currentX;
            MaximumY = maximumY;
            MinimumY = minimumY;
            ShadeSlope = shadeSlope;
            XSlope = xSlope;
        }

        internal static CameraPolygonEdge Calculate(
            int startX,
            int startY,
            int startShade,
            int endX,
            int endY,
            int endShade,
            int scanlineBufferCentre,
            int scanlineLimit)
        {
            int adjustedStartY = startY + scanlineBufferCentre;
            int adjustedEndY = endY + scanlineBufferCentre;
            int currentX = 0;
            int xSlope = 0;
            int currentShade = 0;
            int shadeSlope = 0;
            int minimumY = NotSetSentinel;
            int maximumY = -NotSetSentinel;

            if (adjustedEndY != adjustedStartY)
            {
                xSlope = (endX - startX << 8) /
                    (adjustedEndY - adjustedStartY);
                shadeSlope = (endShade - startShade << 8) /
                    (adjustedEndY - adjustedStartY);

                if (adjustedStartY < adjustedEndY)
                {
                    currentX = startX << 8;
                    currentShade = startShade << 8;
                    minimumY = adjustedStartY;
                    maximumY = adjustedEndY;
                }
                else
                {
                    currentX = endX << 8;
                    currentShade = endShade << 8;
                    minimumY = adjustedEndY;
                    maximumY = adjustedStartY;
                }

                if (minimumY < 0)
                {
                    currentX -= xSlope * minimumY;
                    currentShade -= shadeSlope * minimumY;
                    minimumY = 0;
                }

                if (maximumY > scanlineLimit)
                {
                    maximumY = scanlineLimit;
                }
            }

            return new CameraPolygonEdge(
                currentShade,
                currentX,
                maximumY,
                minimumY,
                shadeSlope,
                xSlope);
        }
    }
}