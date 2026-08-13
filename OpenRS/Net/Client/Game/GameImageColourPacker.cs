namespace OpenRS.Net.Client.Game
{
    internal static class GameImageColourPacker
    {
        private static int ByteMask => 0xff;

        private static int GreenChannelShift => 8;

        private static int BlueChannelShift => 16;

        private static int AlphaChannelShift => 24;

        internal static uint PackRgba(int red, int green, int blue, int alpha)
        {
            if (((red | green | blue | alpha) & -256) != 0)
            {
                red = ClampToByte(red);
                green = ClampToByte(green);
                blue = ClampToByte(blue);
                alpha = ClampToByte(alpha);
            }

            return (uint)(
                red |
                green << GreenChannelShift |
                blue << BlueChannelShift |
                alpha << AlphaChannelShift);
        }

        internal static int PackRgb(int red, int green, int blue) =>
            red << BlueChannelShift |
            green << GreenChannelShift |
            blue;

        private static int ClampToByte(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > ByteMask)
            {
                return ByteMask;
            }

            return value;
        }
    }
}