namespace OpenRS.Net.Client.Game
{
    internal sealed class RoofFaceBuilder
    {
        private readonly EngineHandle engineHandle;

        private static int DiagonalWallMaxIndex => 24000;

        private static int TriangleVertexCount => 3;

        private static int QuadVertexCount => 4;

        internal RoofFaceBuilder(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal RoofFaceContext CreateFaceContext(
            int tileX,
            int tileY,
            int roofColour,
            RoofCornerElevations cornerElevations,
            RoofWorldCoordinates worldCoordinates,
            GameObject sectionObject) => new()
        {
            TileX = tileX,
            TileY = tileY,
            RoofColour = roofColour,
            DiagonalWall = engineHandle.GetDiagonalWall(tileX, tileY),
            TopLeftElevation = cornerElevations.TopLeft,
            TopRightElevation = cornerElevations.TopRight,
            BottomRightElevation = cornerElevations.BottomRight,
            BottomLeftElevation = cornerElevations.BottomLeft,
            TopLeftVertexIndex = sectionObject.GetVertexIndex(
                worldCoordinates.TopLeftX,
                -cornerElevations.TopLeft,
                worldCoordinates.TopLeftY),
            TopRightVertexIndex = sectionObject.GetVertexIndex(
                worldCoordinates.TopRightX,
                -cornerElevations.TopRight,
                worldCoordinates.TopRightY),
            BottomRightVertexIndex = sectionObject.GetVertexIndex(
                worldCoordinates.BottomRightX,
                -cornerElevations.BottomRight,
                worldCoordinates.BottomRightY),
            BottomLeftVertexIndex = sectionObject.GetVertexIndex(
                worldCoordinates.BottomLeftX,
                -cornerElevations.BottomLeft,
                worldCoordinates.BottomLeftY),
        };

        internal void AddFaces(RoofFaceContext faceContext)
        {
            if (TryAddDiagonalFace(faceContext))
            {
                return;
            }

            if (TryAddQuadFace(faceContext))
            {
                return;
            }

            AddSplitTriangleFaces(faceContext);
        }

        private bool TryAddDiagonalFace(RoofFaceContext faceContext)
        {
            if (IsBackDiagonalNorthWestGap(faceContext))
            {
                AddTriangleFace(
                    faceContext.BottomRightVertexIndex,
                    faceContext.BottomLeftVertexIndex,
                    faceContext.TopRightVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            if (IsBackDiagonalSouthEastGap(faceContext))
            {
                AddTriangleFace(
                    faceContext.TopLeftVertexIndex,
                    faceContext.TopRightVertexIndex,
                    faceContext.BottomLeftVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            if (IsForwardDiagonalNorthEastGap(faceContext))
            {
                AddTriangleFace(
                    faceContext.BottomLeftVertexIndex,
                    faceContext.TopLeftVertexIndex,
                    faceContext.BottomRightVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            if (IsForwardDiagonalSouthWestGap(faceContext))
            {
                AddTriangleFace(
                    faceContext.TopRightVertexIndex,
                    faceContext.BottomRightVertexIndex,
                    faceContext.TopLeftVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            return false;
        }

        private bool IsBackDiagonalNorthWestGap(RoofFaceContext faceContext) =>
            faceContext.DiagonalWall > EngineHandle.DiagonalWallBackOffset &&
            faceContext.DiagonalWall < DiagonalWallMaxIndex &&
            engineHandle.GetTileRoofType(faceContext.TileX - 1, faceContext.TileY - 1) == 0;

        private bool IsBackDiagonalSouthEastGap(RoofFaceContext faceContext) =>
            faceContext.DiagonalWall > EngineHandle.DiagonalWallBackOffset &&
            faceContext.DiagonalWall < DiagonalWallMaxIndex &&
            engineHandle.GetTileRoofType(faceContext.TileX + 1, faceContext.TileY + 1) == 0;

        private bool IsForwardDiagonalNorthEastGap(RoofFaceContext faceContext) =>
            faceContext.DiagonalWall > 0 &&
            faceContext.DiagonalWall < EngineHandle.DiagonalWallBackOffset &&
            engineHandle.GetTileRoofType(faceContext.TileX + 1, faceContext.TileY - 1) == 0;

        private bool IsForwardDiagonalSouthWestGap(RoofFaceContext faceContext) =>
            faceContext.DiagonalWall > 0 &&
            faceContext.DiagonalWall < EngineHandle.DiagonalWallBackOffset &&
            engineHandle.GetTileRoofType(faceContext.TileX - 1, faceContext.TileY + 1) == 0;

        private bool TryAddQuadFace(RoofFaceContext faceContext)
        {
            if (HasHorizontalRoofLevels(faceContext))
            {
                AddQuadFace(
                    faceContext.TopLeftVertexIndex,
                    faceContext.TopRightVertexIndex,
                    faceContext.BottomRightVertexIndex,
                    faceContext.BottomLeftVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            if (HasVerticalRoofLevels(faceContext))
            {
                AddQuadFace(
                    faceContext.BottomLeftVertexIndex,
                    faceContext.TopLeftVertexIndex,
                    faceContext.TopRightVertexIndex,
                    faceContext.BottomRightVertexIndex,
                    faceContext.RoofColour);

                return true;
            }

            return false;
        }

        private bool HasHorizontalRoofLevels(RoofFaceContext faceContext) =>
            faceContext.TopLeftElevation == faceContext.TopRightElevation &&
            faceContext.BottomRightElevation == faceContext.BottomLeftElevation;

        private bool HasVerticalRoofLevels(RoofFaceContext faceContext) =>
            faceContext.TopLeftElevation == faceContext.BottomLeftElevation &&
            faceContext.TopRightElevation == faceContext.BottomRightElevation;

        private void AddSplitTriangleFaces(RoofFaceContext faceContext)
        {
            if (ShouldUseTopRightToBottomLeftSplit(faceContext))
            {
                AddTopRightToBottomLeftSplit(faceContext);

                return;
            }

            AddTopLeftToBottomRightSplit(faceContext);
        }

        private bool ShouldUseTopRightToBottomLeftSplit(RoofFaceContext faceContext) =>
            engineHandle.GetTileRoofType(faceContext.TileX - 1, faceContext.TileY - 1) <= 0 &&
            engineHandle.GetTileRoofType(faceContext.TileX + 1, faceContext.TileY + 1) <= 0;

        private void AddTopRightToBottomLeftSplit(RoofFaceContext faceContext)
        {
            AddTriangleFace(
                faceContext.TopLeftVertexIndex,
                faceContext.TopRightVertexIndex,
                faceContext.BottomLeftVertexIndex,
                faceContext.RoofColour);
            AddTriangleFace(
                faceContext.BottomRightVertexIndex,
                faceContext.BottomLeftVertexIndex,
                faceContext.TopRightVertexIndex,
                faceContext.RoofColour);
        }

        private void AddTopLeftToBottomRightSplit(RoofFaceContext faceContext)
        {
            AddTriangleFace(
                faceContext.TopRightVertexIndex,
                faceContext.BottomRightVertexIndex,
                faceContext.TopLeftVertexIndex,
                faceContext.RoofColour);
            AddTriangleFace(
                faceContext.BottomLeftVertexIndex,
                faceContext.TopLeftVertexIndex,
                faceContext.BottomRightVertexIndex,
                faceContext.RoofColour);
        }

        private void AddTriangleFace(
            int firstVertexIndex,
            int secondVertexIndex,
            int thirdVertexIndex,
            int roofColour)
        {
            int[] faceVertices =
            [
                firstVertexIndex,
                secondVertexIndex,
                thirdVertexIndex,
            ];
            engineHandle.CurrentSectionObject.AddFaceVertices(
                TriangleVertexCount,
                faceVertices,
                roofColour,
                EngineHandle.EmptyTileColour);
        }

        private void AddQuadFace(
            int firstVertexIndex,
            int secondVertexIndex,
            int thirdVertexIndex,
            int fourthVertexIndex,
            int roofColour)
        {
            int[] faceVertices =
            [
                firstVertexIndex,
                secondVertexIndex,
                thirdVertexIndex,
                fourthVertexIndex,
            ];
            engineHandle.CurrentSectionObject.AddFaceVertices(
                QuadVertexCount,
                faceVertices,
                roofColour,
                EngineHandle.EmptyTileColour);
        }
    }
}