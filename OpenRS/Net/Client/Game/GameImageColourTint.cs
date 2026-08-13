namespace OpenRS.Net.Client.Game
{
    internal readonly struct GameImageColourTint
    {
        private static int ByteMask => 0xff;

        private static int FullRedChannel => 255;

        private static int GreenShift => 8;

        private static int RedShift => 16;

        private int Blue { get; }

        private int Green { get; }

        private int Red { get; }

        internal GameImageColourTint(int colour)
        {
            Red = colour >> RedShift & ByteMask;
            Green = colour >> GreenShift & ByteMask;
            Blue = colour & ByteMask;
        }

        internal int ApplyPrimary(int colour)
        {
            int red = colour >> RedShift & ByteMask;
            int green = colour >> GreenShift & ByteMask;
            int blue = colour & ByteMask;

            if (red == green && green == blue)
            {
                return Apply(red, green, blue);
            }

            return colour;
        }

        internal int ApplyPrimaryAndSecondary(
            int colour,
            GameImageColourTint secondaryTint)
        {
            int red = colour >> RedShift & ByteMask;
            int green = colour >> GreenShift & ByteMask;
            int blue = colour & ByteMask;

            if (red == green && green == blue)
            {
                return Apply(red, green, blue);
            }

            if (red == FullRedChannel && green == blue)
            {
                return secondaryTint.Apply(red, green, blue);
            }

            return colour;
        }

        private int Apply(int red, int green, int blue) =>
            ((red * Red >> GreenShift) << RedShift) +
            ((green * Green >> GreenShift) << GreenShift) +
            (blue * Blue >> GreenShift);
    }
}