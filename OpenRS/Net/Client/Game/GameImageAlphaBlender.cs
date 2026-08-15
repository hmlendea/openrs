namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageAlphaBlender
    {
        private static int AlphaScale => 256;

        private static int ByteMask => 0xff;

        private readonly int inverseAlpha;
        private readonly int scaledBlue;
        private readonly int scaledGreen;
        private readonly int scaledRed;

        internal GameImageAlphaBlender(int colour, int alpha)
        {
            inverseAlpha = AlphaScale - alpha;
            scaledRed = (colour >> 16 & ByteMask) * alpha;
            scaledGreen = (colour >> 8 & ByteMask) * alpha;
            scaledBlue = (colour & ByteMask) * alpha;
        }

        internal int Blend(int existingColour)
        {
            int existingRed =
                (existingColour >> 16 & ByteMask) * inverseAlpha;
            int existingGreen =
                (existingColour >> 8 & ByteMask) * inverseAlpha;
            int existingBlue =
                (existingColour & ByteMask) * inverseAlpha;

            return ((scaledRed + existingRed >> 8) << 16) +
                ((scaledGreen + existingGreen >> 8) << 8) +
                (scaledBlue + existingBlue >> 8);
        }
    }
}