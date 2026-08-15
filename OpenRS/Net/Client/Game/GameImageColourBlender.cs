namespace OpenRS.Net.Client.Game
{
    internal static class GameImageColourBlender
    {
        private static int RedBlueChannelMask => 0xff00ff;

        private static uint RedBlueBlendMask => 0xff00ff00;

        private static int GreenChannelMask => 0xff00;

        private static int GreenBlendMask => 0xff0000;

        private static int BlendShift => 8;

        internal static int Blend(
            int foregroundColour,
            int backgroundColour,
            int blendFactor,
            int blendComplement)
        {
            int blendedRedBlue =
                (int)((foregroundColour & RedBlueChannelMask) * blendFactor +
                (backgroundColour & RedBlueChannelMask) * blendComplement &
                RedBlueBlendMask);
            int blendedGreen =
                (foregroundColour & GreenChannelMask) * blendFactor +
                (backgroundColour & GreenChannelMask) * blendComplement &
                GreenBlendMask;

            return (blendedRedBlue + blendedGreen) >> BlendShift;
        }
    }
}