namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectProjector
    {
        private static int RotationShift => 15;

        private static int FineRotationCosineTableOffset => 1024;

        internal static VertexProjectionContext BuildContext(
            int originX,
            int originY,
            int originZ,
            int rotationXAngle,
            int rotationYAngle,
            int rotationZAngle,
            int projectionScale,
            int nearPlane)
        {
            VertexProjectionContext context = new()
            {
                OriginX = originX,
                OriginY = originY,
                OriginZ = originZ,
                RotationXAngle = rotationXAngle,
                RotationYAngle = rotationYAngle,
                RotationZAngle = rotationZAngle,
                ProjectionScale = projectionScale,
                NearPlane = nearPlane
            };

            if (rotationZAngle != 0)
            {
                context.SinZ = GameObjectLookupTables.FineRotationTable[rotationZAngle];
                context.CosZ = GameObjectLookupTables.FineRotationTable[
                    rotationZAngle + FineRotationCosineTableOffset];
            }

            if (rotationYAngle != 0)
            {
                context.SinY = GameObjectLookupTables.FineRotationTable[rotationYAngle];
                context.CosY = GameObjectLookupTables.FineRotationTable[
                    rotationYAngle + FineRotationCosineTableOffset];
            }

            if (rotationXAngle != 0)
            {
                context.SinX = GameObjectLookupTables.FineRotationTable[rotationXAngle];
                context.CosX = GameObjectLookupTables.FineRotationTable[
                    rotationXAngle + FineRotationCosineTableOffset];
            }

            return context;
        }

        internal static void Project(
            GameObject gameObject,
            VertexProjectionContext context)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                ProjectVertex(gameObject, vertexIndex, context);
            }
        }

        private static void ProjectVertex(
            GameObject gameObject,
            int vertexIndex,
            VertexProjectionContext context)
        {
            int vertexX = gameObject.WorldVertX[vertexIndex] - context.OriginX;
            int vertexY = gameObject.WorldVertY[vertexIndex] - context.OriginY;
            int vertexZ = gameObject.WorldVertZ[vertexIndex] - context.OriginZ;

            if (context.RotationZAngle != 0)
            {
                int rotatedX =
                    (vertexY * context.SinZ + vertexX * context.CosZ) >> RotationShift;
                vertexY =
                    (vertexY * context.CosZ - vertexX * context.SinZ) >> RotationShift;
                vertexX = rotatedX;
            }

            if (context.RotationYAngle != 0)
            {
                int rotatedX =
                    (vertexZ * context.SinY + vertexX * context.CosY) >> RotationShift;
                vertexZ =
                    (vertexZ * context.CosY - vertexX * context.SinY) >> RotationShift;
                vertexX = rotatedX;
            }

            if (context.RotationXAngle != 0)
            {
                int rotatedY =
                    (vertexY * context.CosX - vertexZ * context.SinX) >> RotationShift;
                vertexZ =
                    (vertexY * context.SinX + vertexZ * context.CosX) >> RotationShift;
                vertexY = rotatedY;
            }

            if (vertexZ >= context.NearPlane)
            {
                gameObject.ProjectedU[vertexIndex] =
                    (vertexX << context.ProjectionScale) / vertexZ;
                gameObject.ProjectedV[vertexIndex] =
                    (vertexY << context.ProjectionScale) / vertexZ;
            }
            else
            {
                gameObject.ProjectedU[vertexIndex] = vertexX << context.ProjectionScale;
                gameObject.ProjectedV[vertexIndex] = vertexY << context.ProjectionScale;
            }

            gameObject.ProjectedX[vertexIndex] = vertexX;
            gameObject.ProjectedY[vertexIndex] = vertexY;
            gameObject.ProjectedDepth[vertexIndex] = vertexZ;
        }
    }
}