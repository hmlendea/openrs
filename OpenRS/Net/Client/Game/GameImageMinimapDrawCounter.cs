namespace OpenRS.Net.Client.Game
{
    internal static class GameImageMinimapDrawCounter
    {
        private static int AlignmentMask => 0x3f;

        private static int SpiralScale => 192;

        private static int StandardScale => 128;

        internal static void Update(int scale, int rotation)
        {
            bool isAlignedWithLastRotation =
                (rotation & AlignmentMask) ==
                (GameImage.LastCharacterRotation & AlignmentMask);

            if (scale == SpiralScale && isAlignedWithLastRotation)
            {
                GameImage.SpiralDrawCount += 1;
            }
            else if (scale == StandardScale)
            {
                GameImage.LastCharacterRotation = rotation;
            }
            else
            {
                GameImage.CharacterDrawCount += 1;
            }
        }
    }
}