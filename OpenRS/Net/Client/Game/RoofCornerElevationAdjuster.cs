namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofCornerElevationAdjuster
    {
        private readonly EngineHandle engineHandle;

        internal RoofCornerElevationAdjuster(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void RaiseAndNormaliseCornerElevations(
            RoofCornerCoordinates cornerCoordinates,
            RoofCornerElevations cornerElevations,
            int roofRaiseAmount)
        {
            RaiseCornerElevations(cornerCoordinates, cornerElevations, roofRaiseAmount);
            NormaliseCornerElevations(cornerElevations);
        }

        private void RaiseCornerElevations(
            RoofCornerCoordinates cornerCoordinates,
            RoofCornerElevations cornerElevations,
            int roofRaiseAmount)
        {
            cornerElevations.TopLeft = RaiseCornerElevation(
                cornerCoordinates.TopLeftX,
                cornerCoordinates.TopLeftY,
                cornerElevations.TopLeft,
                roofRaiseAmount);
            cornerElevations.TopRight = RaiseCornerElevation(
                cornerCoordinates.TopRightX,
                cornerCoordinates.TopRightY,
                cornerElevations.TopRight,
                roofRaiseAmount);
            cornerElevations.BottomRight = RaiseCornerElevation(
                cornerCoordinates.BottomRightX,
                cornerCoordinates.BottomRightY,
                cornerElevations.BottomRight,
                roofRaiseAmount);
            cornerElevations.BottomLeft = RaiseCornerElevation(
                cornerCoordinates.BottomLeftX,
                cornerCoordinates.BottomLeftY,
                cornerElevations.BottomLeft,
                roofRaiseAmount);
        }

        private int RaiseCornerElevation(
            int cornerX,
            int cornerY,
            int elevation,
            int roofRaiseAmount)
        {
            if (!engineHandle.IsRoofTile(cornerX, cornerY) ||
                elevation >= EngineHandle.RoofElevationFlag)
            {
                return elevation;
            }

            int raisedElevation =
                elevation +
                roofRaiseAmount +
                EngineHandle.RoofElevationFlag;
            engineHandle.RoofTiles[cornerX][cornerY] = raisedElevation;

            return raisedElevation;
        }

        private void NormaliseCornerElevations(RoofCornerElevations cornerElevations)
        {
            cornerElevations.TopLeft = NormaliseRaisedRoofElevation(cornerElevations.TopLeft);
            cornerElevations.TopRight = NormaliseRaisedRoofElevation(cornerElevations.TopRight);
            cornerElevations.BottomRight =
                NormaliseRaisedRoofElevation(cornerElevations.BottomRight);
            cornerElevations.BottomLeft = NormaliseRaisedRoofElevation(cornerElevations.BottomLeft);
        }

        private int NormaliseRaisedRoofElevation(int elevation)
        {
            if (elevation >= EngineHandle.RoofElevationFlag)
            {
                return elevation - EngineHandle.RoofElevationFlag;
            }

            return elevation;
        }
    }
}