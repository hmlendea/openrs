namespace OpenRS.Net.Client.Game
{
    internal readonly struct SectorCoordinates
    {
        internal int Layer { get; }

        internal int X { get; }

        internal int Y { get; }

        private static int TopLeftLayer => 0;

        private static int TopRightLayer => 1;

        private static int BottomLeftLayer => 2;

        private static int BottomRightLayer => 3;

        private SectorCoordinates(int layer, int x, int y)
        {
            Layer = layer;
            X = x;
            Y = y;
        }

        internal static SectorCoordinates From(int x, int y)
        {
            bool isRightSector = IsRightSector(x);
            bool isBottomSector = IsBottomSector(y);

            if (isRightSector && !isBottomSector)
            {
                return CreateTopRightCoordinates(x, y);
            }

            if (!isRightSector && isBottomSector)
            {
                return CreateBottomLeftCoordinates(x, y);
            }

            if (isRightSector && isBottomSector)
            {
                return CreateBottomRightCoordinates(x, y);
            }

            return CreateTopLeftCoordinates(x, y);
        }

        private static bool IsRightSector(int x) => x >= EngineHandle.SectorSize;

        private static bool IsBottomSector(int y) => y >= EngineHandle.SectorSize;

        private static int NormaliseCoordinate(int coordinate) => coordinate - EngineHandle.SectorSize;

        private static SectorCoordinates CreateTopLeftCoordinates(int x, int y) =>
            new(TopLeftLayer, x, y);

        private static SectorCoordinates CreateTopRightCoordinates(int x, int y) =>
            new(TopRightLayer, NormaliseCoordinate(x), y);

        private static SectorCoordinates CreateBottomLeftCoordinates(int x, int y) =>
            new(BottomLeftLayer, x, NormaliseCoordinate(y));

        private static SectorCoordinates CreateBottomRightCoordinates(int x, int y) =>
            new(BottomRightLayer, NormaliseCoordinate(x), NormaliseCoordinate(y));
    }
}
