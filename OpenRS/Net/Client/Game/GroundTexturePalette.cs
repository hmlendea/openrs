using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Net.Client.Game
{
    internal static class GroundTexturePalette
    {
        private static int SnowChannelMax => 255;
        private static int SnowRedBlueDecayFactor => 4;
        private static double SnowGreenDecayRate => 1.75;
        private static int GrassRedGrowthFactor => 3;
        private static int GrassGreenChannel => 144;
        private static int AutumnRedBase => 192;
        private static int AutumnGreenBase => 144;
        private static double AutumnDecayRate => 1.5;
        private static int DirtRedBase => 96;
        private static int DirtGreenBase => 48;
        private static double DirtGrowthRate => 1.5;

        internal static int[] Create()
        {
            int chunkCount = EngineHandle.ChunkCount;
            int[] palette = new int[chunkCount * 4];

            for (int textureIndex = 0; textureIndex < chunkCount; textureIndex += 1)
            {
                palette[textureIndex] = Camera.GetTextureColour(
                    SnowChannelMax - textureIndex * SnowRedBlueDecayFactor,
                    SnowChannelMax - (int)(textureIndex * SnowGreenDecayRate),
                    SnowChannelMax - textureIndex * SnowRedBlueDecayFactor);
            }

            for (int textureIndex = 0; textureIndex < chunkCount; textureIndex += 1)
            {
                palette[textureIndex + chunkCount] = Camera.GetTextureColour(
                    textureIndex * GrassRedGrowthFactor,
                    GrassGreenChannel,
                    0);
            }

            for (int textureIndex = 0; textureIndex < chunkCount; textureIndex += 1)
            {
                palette[textureIndex + chunkCount * 2] = Camera.GetTextureColour(
                    AutumnRedBase - (int)(textureIndex * AutumnDecayRate),
                    AutumnGreenBase - (int)(textureIndex * AutumnDecayRate),
                    0);
            }

            for (int textureIndex = 0; textureIndex < chunkCount; textureIndex += 1)
            {
                palette[textureIndex + chunkCount * 3] = Camera.GetTextureColour(
                    DirtRedBase - (int)(textureIndex * DirtGrowthRate),
                    DirtGreenBase + (int)(textureIndex * DirtGrowthRate),
                    0);
            }

            return palette;
        }
    }
}
