namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofTileContextFactory
    {
        private readonly EngineHandle engineHandle;

        internal RoofTileContextFactory(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal RoofCornerCoordinates CreateCornerCoordinates(int tileX, int tileY) => new()
        {
            TopLeftX = tileX,
            TopLeftY = tileY,
            TopRightX = tileX + 1,
            TopRightY = tileY,
            BottomRightX = tileX + 1,
            BottomRightY = tileY + 1,
            BottomLeftX = tileX,
            BottomLeftY = tileY + 1,
        };

        internal RoofWorldCoordinates CreateWorldCoordinates(int tileX, int tileY) => new()
        {
            TopLeftX = tileX * EngineHandle.TileWorldSize,
            TopLeftY = tileY * EngineHandle.TileWorldSize,
            TopRightX = (tileX + 1) * EngineHandle.TileWorldSize,
            TopRightY = tileY * EngineHandle.TileWorldSize,
            BottomRightX = (tileX + 1) * EngineHandle.TileWorldSize,
            BottomRightY = (tileY + 1) * EngineHandle.TileWorldSize,
            BottomLeftX = tileX * EngineHandle.TileWorldSize,
            BottomLeftY = (tileY + 1) * EngineHandle.TileWorldSize,
        };

        internal RoofCornerElevations LoadCornerElevations(
            RoofCornerCoordinates cornerCoordinates) => new()
        {
            TopLeft = engineHandle.RoofTiles[cornerCoordinates.TopLeftX][cornerCoordinates.TopLeftY],
            TopRight = engineHandle.RoofTiles[cornerCoordinates.TopRightX][cornerCoordinates.TopRightY],
            BottomRight =
                engineHandle.RoofTiles[cornerCoordinates.BottomRightX][cornerCoordinates.BottomRightY],
            BottomLeft =
                engineHandle.RoofTiles[cornerCoordinates.BottomLeftX][cornerCoordinates.BottomLeftY],
        };
    }
}