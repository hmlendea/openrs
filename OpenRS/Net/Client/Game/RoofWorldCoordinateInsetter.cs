namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofWorldCoordinateInsetter
    {
        private readonly EngineHandle engineHandle;

        private static int RoofEdgeInset => 16;

        internal RoofWorldCoordinateInsetter(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void ApplyEdgeInsets(
            RoofCornerCoordinates cornerCoordinates,
            RoofWorldCoordinates worldCoordinates)
        {
            ApplyTopLeftEdgeInsets(cornerCoordinates, worldCoordinates);
            ApplyTopRightEdgeInsets(cornerCoordinates, worldCoordinates);
            ApplyBottomRightEdgeInsets(cornerCoordinates, worldCoordinates);
            ApplyBottomLeftEdgeInsets(cornerCoordinates, worldCoordinates);
        }

        private void ApplyTopLeftEdgeInsets(
            RoofCornerCoordinates cornerCoordinates,
            RoofWorldCoordinates worldCoordinates)
        {
            worldCoordinates.TopLeftX = ApplyHorizontalEdgeInset(
                cornerCoordinates.TopLeftX,
                cornerCoordinates.TopLeftY,
                worldCoordinates.TopLeftX);
            worldCoordinates.TopLeftY = ApplyVerticalEdgeInset(
                cornerCoordinates.TopLeftX,
                cornerCoordinates.TopLeftY,
                worldCoordinates.TopLeftY);
        }

        private void ApplyTopRightEdgeInsets(
            RoofCornerCoordinates cornerCoordinates,
            RoofWorldCoordinates worldCoordinates)
        {
            worldCoordinates.TopRightX = ApplyHorizontalEdgeInset(
                cornerCoordinates.TopRightX,
                cornerCoordinates.TopRightY,
                worldCoordinates.TopRightX);
            worldCoordinates.TopRightY = ApplyVerticalEdgeInset(
                cornerCoordinates.TopRightX,
                cornerCoordinates.TopRightY,
                worldCoordinates.TopRightY);
        }

        private void ApplyBottomRightEdgeInsets(
            RoofCornerCoordinates cornerCoordinates,
            RoofWorldCoordinates worldCoordinates)
        {
            worldCoordinates.BottomRightX = ApplyHorizontalEdgeInset(
                cornerCoordinates.BottomRightX,
                cornerCoordinates.BottomRightY,
                worldCoordinates.BottomRightX);
            worldCoordinates.BottomRightY = ApplyVerticalEdgeInset(
                cornerCoordinates.BottomRightX,
                cornerCoordinates.BottomRightY,
                worldCoordinates.BottomRightY);
        }

        private void ApplyBottomLeftEdgeInsets(
            RoofCornerCoordinates cornerCoordinates,
            RoofWorldCoordinates worldCoordinates)
        {
            worldCoordinates.BottomLeftX = ApplyHorizontalEdgeInset(
                cornerCoordinates.BottomLeftX,
                cornerCoordinates.BottomLeftY,
                worldCoordinates.BottomLeftX);
            worldCoordinates.BottomLeftY = ApplyVerticalEdgeInset(
                cornerCoordinates.BottomLeftX,
                cornerCoordinates.BottomLeftY,
                worldCoordinates.BottomLeftY);
        }

        private int ApplyHorizontalEdgeInset(int cornerX, int cornerY, int coordinate)
        {
            int adjustedCoordinate = coordinate;

            if (!engineHandle.HasRoofTiles(cornerX - 1, cornerY))
            {
                adjustedCoordinate -= RoofEdgeInset;
            }

            if (!engineHandle.HasRoofTiles(cornerX + 1, cornerY))
            {
                adjustedCoordinate += RoofEdgeInset;
            }

            return adjustedCoordinate;
        }

        private int ApplyVerticalEdgeInset(int cornerX, int cornerY, int coordinate)
        {
            int adjustedCoordinate = coordinate;

            if (!engineHandle.HasRoofTiles(cornerX, cornerY - 1))
            {
                adjustedCoordinate -= RoofEdgeInset;
            }

            if (!engineHandle.HasRoofTiles(cornerX, cornerY + 1))
            {
                adjustedCoordinate += RoofEdgeInset;
            }

            return adjustedCoordinate;
        }
    }
}