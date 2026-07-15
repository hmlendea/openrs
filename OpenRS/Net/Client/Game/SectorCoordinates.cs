namespace OpenRS.Net.Client.Game
{
    internal readonly struct SectorCoordinates
    {
        internal int Layer { get; }
        internal int X { get; }
        internal int Y { get; }

        private SectorCoordinates(int layer, int x, int y)
        {
            Layer = layer;
            X = x;
            Y = y;
        }

        internal static SectorCoordinates From(int x, int y)
        {
            if (x >= EngineHandle.SectorSize && y < EngineHandle.SectorSize)
            {
                return new SectorCoordinates(1, x - EngineHandle.SectorSize, y);
            }

            if (x < EngineHandle.SectorSize && y >= EngineHandle.SectorSize)
            {
                return new SectorCoordinates(2, x, y - EngineHandle.SectorSize);
            }

            if (x >= EngineHandle.SectorSize && y >= EngineHandle.SectorSize)
            {
                return new SectorCoordinates(3, x - EngineHandle.SectorSize, y - EngineHandle.SectorSize);
            }

            return new SectorCoordinates(0, x, y);
        }
    }
}
