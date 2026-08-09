using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectLookupTables
    {
        internal static readonly int[] RotationSinCosTable;
        internal static readonly int[] FineRotationTable;
        internal static readonly int[] ScaleTable;
        internal static readonly int[] ShadeTable;

        static GameObjectLookupTables()
        {
            RotationSinCosTable = new int[512];
            FineRotationTable = new int[2048];
            ScaleTable = new int[64];
            ShadeTable = new int[256];

            for (int angleIndex = 0; angleIndex < 256; angleIndex += 1)
            {
                RotationSinCosTable[angleIndex] = (int)(Math.Sin(angleIndex * 0.02454369D) * 32768D);
                RotationSinCosTable[angleIndex + 256] = (int)(Math.Cos(angleIndex * 0.02454369D) * 32768D);
            }

            for (int fineAngleIndex = 0; fineAngleIndex < 1024; fineAngleIndex += 1)
            {
                FineRotationTable[fineAngleIndex] = (int)(Math.Sin(fineAngleIndex * 0.00613592315D) * 32768D);
                FineRotationTable[fineAngleIndex + 1024] = (int)(Math.Cos(fineAngleIndex * 0.00613592315D) * 32768D);
            }

            for (int digitIndex = 0; digitIndex < 10; digitIndex += 1)
            {
                ScaleTable[digitIndex] = (byte)(48 + digitIndex);
            }

            for (int upperCaseIndex = 0; upperCaseIndex < 26; upperCaseIndex += 1)
            {
                ScaleTable[upperCaseIndex + 10] = (byte)(65 + upperCaseIndex);
            }

            for (int lowerCaseIndex = 0; lowerCaseIndex < 26; lowerCaseIndex += 1)
            {
                ScaleTable[lowerCaseIndex + 36] = (byte)(97 + lowerCaseIndex);
            }

            ScaleTable[62] = -93;
            ScaleTable[63] = 36;

            for (int digitIndex = 0; digitIndex < 10; digitIndex += 1)
            {
                ShadeTable[48 + digitIndex] = digitIndex;
            }

            for (int upperCaseIndex = 0; upperCaseIndex < 26; upperCaseIndex += 1)
            {
                ShadeTable[65 + upperCaseIndex] = upperCaseIndex + 10;
            }

            for (int lowerCaseIndex = 0; lowerCaseIndex < 26; lowerCaseIndex += 1)
            {
                ShadeTable[97 + lowerCaseIndex] = lowerCaseIndex + 36;
            }

            ShadeTable[163] = 62;
            ShadeTable[36] = 63;
        }
    }
}
