namespace OpenRS.Net.Client.Game
{
    internal sealed class SectionSceneBuilder
    {
        private readonly GroundSectionBuilder groundBuilder;

        private readonly WallSectionBuilder wallBuilder;

        internal SectionSceneBuilder(EngineHandle engineHandle)
        {
            groundBuilder = new GroundSectionBuilder(engineHandle);
            wallBuilder = new WallSectionBuilder(engineHandle);
        }

        internal void BuildSection(int height, bool freshLoad)
        {
            if (freshLoad)
            {
                groundBuilder.InitialiseGroundVertices(height);
                groundBuilder.BuildGroundTileGeometry(height);
                groundBuilder.BuildWaterBorderGeometry(height);
                groundBuilder.FinaliseGroundSection();
            }

            wallBuilder.BuildWalls(height, freshLoad);
        }
    }
}
