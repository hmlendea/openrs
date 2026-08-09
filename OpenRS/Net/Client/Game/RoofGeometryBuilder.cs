using OpenRS.Models;

namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofGeometryBuilder
    {
        private readonly EngineHandle engineHandle;

        private readonly RoofCornerElevationAdjuster roofCornerElevationAdjuster;

        private readonly RoofFaceBuilder roofFaceBuilder;

        private readonly RoofTileContextFactory roofTileContextFactory;

        private readonly RoofWorldCoordinateInsetter roofWorldCoordinateInsetter;

        internal RoofGeometryBuilder(
            EngineHandle engineHandle,
            RoofTileContextFactory roofTileContextFactory)
        {
            this.engineHandle = engineHandle;
            this.roofTileContextFactory = roofTileContextFactory;
            roofCornerElevationAdjuster = new RoofCornerElevationAdjuster(engineHandle);
            roofFaceBuilder = new RoofFaceBuilder(engineHandle);
            roofWorldCoordinateInsetter = new RoofWorldCoordinateInsetter(engineHandle);
        }

        internal void BuildRoofGeometry()
        {
            GameObject sectionObject = engineHandle.CurrentSectionObject;
            sectionObject.ResetObjectIndexes();

            for (int tileX = 1; tileX < EngineHandle.GridSize - 1; tileX += 1)
            {
                for (int tileY = 1; tileY < EngineHandle.GridSize - 1; tileY += 1)
                {
                    BuildRoofTileGeometry(tileX, tileY, sectionObject);
                }
            }
        }

        private void BuildRoofTileGeometry(int tileX, int tileY, GameObject sectionObject)
        {
            int roofType = engineHandle.GetTileRoofType(tileX, tileY);

            if (roofType <= 0)
            {
                return;
            }

            RoofCornerCoordinates cornerCoordinates =
                roofTileContextFactory.CreateCornerCoordinates(tileX, tileY);
            RoofWorldCoordinates worldCoordinates =
                roofTileContextFactory.CreateWorldCoordinates(tileX, tileY);
            RoofCornerElevations cornerElevations =
                roofTileContextFactory.LoadCornerElevations(cornerCoordinates);
            Elevation elevation = engineHandle.entityManager.GetElevation(roofType - 1);
            roofCornerElevationAdjuster.RaiseAndNormaliseCornerElevations(
                cornerCoordinates,
                cornerElevations,
                elevation.Roof);
            roofWorldCoordinateInsetter.ApplyEdgeInsets(cornerCoordinates, worldCoordinates);

            RoofFaceContext faceContext = roofFaceBuilder.CreateFaceContext(
                tileX,
                tileY,
                elevation.Colour,
                cornerElevations,
                worldCoordinates,
                sectionObject);
            roofFaceBuilder.AddFaces(faceContext);
        }
    }
}