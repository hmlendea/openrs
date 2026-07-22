using System;

namespace OpenRS.Net.Client.Game.Cameras
{
    public static class CameraTrigonometryTable
    {
        private static readonly int[] sinCosTable = new int[SinCosTableHalfSize * 2];

        public static readonly int[] Table = new int[TrigTableHalfSize * 2];

        public static int TrigTableHalfSize => 1024;
        public static int TrigTableMask => 0x3ff;

        private static int TrigFixedPointScale => 32768;
        private static int SinCosTableHalfSize => 256;
        private static double SinCosTableAngleStep => 0.02454369D;
        private static double TrigTableAngleStep => 0.00613592315D;

        static CameraTrigonometryTable()
        {
            for (int sinCosIndex = 0; sinCosIndex < SinCosTableHalfSize; sinCosIndex += 1)
            {
                sinCosTable[sinCosIndex] = (int)(Math.Sin(sinCosIndex * SinCosTableAngleStep) * TrigFixedPointScale);
                sinCosTable[sinCosIndex + SinCosTableHalfSize] = (int)(Math.Cos(sinCosIndex * SinCosTableAngleStep) * TrigFixedPointScale);
            }

            for (int trigIndex = 0; trigIndex < TrigTableHalfSize; trigIndex += 1)
            {
                Table[trigIndex] = (int)(Math.Sin(trigIndex * TrigTableAngleStep) * TrigFixedPointScale);
                Table[trigIndex + TrigTableHalfSize] = (int)(Math.Cos(trigIndex * TrigTableAngleStep) * TrigFixedPointScale);
            }
        }
    }
}
