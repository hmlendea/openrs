using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameImageMinimapRotationTable
    {
        internal static int CosineOffset => 256;

        private static int TableSize => 512;

        private static double AngleStep => 0.02454369;

        private static double TrigonometryScale => 32768.0;

        internal static void EnsureInitialised(GameImage gameImage)
        {
            if (gameImage.CharacterRotationTable is not null)
            {
                return;
            }

            gameImage.CharacterRotationTable = new int[TableSize];

            for (int index = 0; index < CosineOffset; index += 1)
            {
                gameImage.CharacterRotationTable[index] =
                    (int)(Math.Sin(index * AngleStep) * TrigonometryScale);
                gameImage.CharacterRotationTable[index + CosineOffset] =
                    (int)(Math.Cos(index * AngleStep) * TrigonometryScale);
            }
        }
    }
}