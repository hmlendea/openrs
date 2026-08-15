namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageSpriteSample
    {
        internal int Colour { get; }

        internal bool IsVisible { get; }

        internal GameImageSpriteSample(int colour, bool isVisible)
        {
            Colour = colour;
            IsVisible = isVisible;
        }
    }
}