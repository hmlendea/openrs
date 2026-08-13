namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraTextureColourCodec
    {
        private static int ColourIndexRedMultiplier => 1024;

        private static int ColourIndexGreenMultiplier => 32;

        private static int ColourChannelQuantisationDivisor => 8;

        private static int ColourChannel5BitMask => 0x1f;

        internal static int Pack(int red, int green, int blue)
            => -1 - red / ColourChannelQuantisationDivisor *
                ColourIndexRedMultiplier -
                green / ColourChannelQuantisationDivisor *
                ColourIndexGreenMultiplier -
                blue / ColourChannelQuantisationDivisor;

        internal static int Expand(int packedColour)
        {
            int colourIndex = -(packedColour + 1);
            int redComponent = colourIndex >> 10 & ColourChannel5BitMask;
            int greenComponent = colourIndex >> 5 & ColourChannel5BitMask;
            int blueComponent = colourIndex & ColourChannel5BitMask;

            return (redComponent << 19) +
                (greenComponent << 11) +
                (blueComponent << 3);
        }
    }
}