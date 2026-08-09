namespace OpenRS.Net.Client
{
    public readonly struct ItemSpriteDrawCall
    {
        public int PixelX { get; init; }
        public int PixelY { get; init; }
        public int PixelWidth { get; init; }
        public int PixelHeight { get; init; }
        public string SpriteName { get; init; }
    }
}
