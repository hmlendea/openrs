namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectTransformer
    {
        internal static void ApplyNullTransform(GameObject gameObject)
        {
            gameObject.ObjectState = 0;

            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] = gameObject.VertexCoordinatesX[vertexIndex];
                gameObject.WorldVertY[vertexIndex] = gameObject.VertexCoordinatesY[vertexIndex];
                gameObject.WorldVertZ[vertexIndex] = gameObject.VertexCoordinatesZ[vertexIndex];
            }

            int farBound = 0x98967f;
            gameObject.MaximumFaceSpan = gameObject.BoundsMaxX = gameObject.BoundsMaxY = gameObject.BoundsMaxZ = farBound;
            gameObject.BoundsMinX = gameObject.BoundsMinY = gameObject.BoundsMinZ = -farBound;
        }

        internal static void ApplyFullTransform(
            GameObject gameObject,
            int transformType,
            int rotationX,
            int rotationY,
            int rotationZ,
            int scaleX,
            int scaleY,
            int scaleZ,
            int secondaryScaleX,
            int secondaryScaleY,
            int secondaryScaleZ,
            int tertiaryScaleX,
            int tertiaryScaleY,
            int tertiaryScaleZ,
            int positionX,
            int positionY,
            int positionZ)
        {
            gameObject.ObjectState = 0;
            CopyLocalToWorldVerts(gameObject);

            if (transformType >= 2)
            {
                RotateVertices(gameObject, rotationX, rotationY, rotationZ);
            }

            if (transformType >= 3)
            {
                ScaleVertices(gameObject, scaleX, scaleY, scaleZ);
            }

            if (transformType >= 4)
            {
                ScaleVerticesShear(gameObject, secondaryScaleX, secondaryScaleY, secondaryScaleZ, tertiaryScaleX, tertiaryScaleY, tertiaryScaleZ);
            }

            if (transformType >= 1)
            {
                OffsetWorldVertices(gameObject, positionX, positionY, positionZ);
            }

            GameObjectShaderCalculator.CalculateBounds(gameObject);
            GameObjectShaderCalculator.CalculatePolygonNormals(gameObject);
        }

        internal static void ProjectVertices(GameObject gameObject, VertexProjectionContext context)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                ProjectSingleVertex(gameObject, vertexIndex, context);
            }
        }

        internal static VertexProjectionContext BuildProjectionContext(
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
                context.CosZ = GameObjectLookupTables.FineRotationTable[rotationZAngle + 1024];
            }

            if (rotationYAngle != 0)
            {
                context.SinY = GameObjectLookupTables.FineRotationTable[rotationYAngle];
                context.CosY = GameObjectLookupTables.FineRotationTable[rotationYAngle + 1024];
            }

            if (rotationXAngle != 0)
            {
                context.SinX = GameObjectLookupTables.FineRotationTable[rotationXAngle];
                context.CosX = GameObjectLookupTables.FineRotationTable[rotationXAngle + 1024];
            }

            return context;
        }

        private static void CopyLocalToWorldVerts(GameObject gameObject)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] = gameObject.VertexCoordinatesX[vertexIndex];
                gameObject.WorldVertY[vertexIndex] = gameObject.VertexCoordinatesY[vertexIndex];
                gameObject.WorldVertZ[vertexIndex] = gameObject.VertexCoordinatesZ[vertexIndex];
            }
        }

        private static void OffsetWorldVertices(GameObject gameObject, int x, int y, int z)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] += x;
                gameObject.WorldVertY[vertexIndex] += y;
                gameObject.WorldVertZ[vertexIndex] += z;
            }
        }

        private static void RotateVertices(GameObject gameObject, int rotationXAmount, int rotationYAmount, int rotationZAmount)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                RotateSingleVertex(gameObject, vertexIndex, rotationXAmount, rotationYAmount, rotationZAmount);
            }
        }

        private static void RotateSingleVertex(
            GameObject gameObject,
            int vertexIndex,
            int rotationXAmount,
            int rotationYAmount,
            int rotationZAmount)
        {
            if (rotationZAmount != 0)
            {
                int sinZ = GameObjectLookupTables.RotationSinCosTable[rotationZAmount];
                int cosZ = GameObjectLookupTables.RotationSinCosTable[rotationZAmount + 256];
                int rotatedX = gameObject.WorldVertY[vertexIndex] * sinZ + gameObject.WorldVertX[vertexIndex] * cosZ >> 15;
                gameObject.WorldVertY[vertexIndex] = gameObject.WorldVertY[vertexIndex] * cosZ - gameObject.WorldVertX[vertexIndex] * sinZ >> 15;
                gameObject.WorldVertX[vertexIndex] = rotatedX;
            }

            if (rotationXAmount != 0)
            {
                int sinX = GameObjectLookupTables.RotationSinCosTable[rotationXAmount];
                int cosX = GameObjectLookupTables.RotationSinCosTable[rotationXAmount + 256];
                int rotatedY = gameObject.WorldVertY[vertexIndex] * cosX - gameObject.WorldVertZ[vertexIndex] * sinX >> 15;
                gameObject.WorldVertZ[vertexIndex] = gameObject.WorldVertY[vertexIndex] * sinX + gameObject.WorldVertZ[vertexIndex] * cosX >> 15;
                gameObject.WorldVertY[vertexIndex] = rotatedY;
            }

            if (rotationYAmount != 0)
            {
                int sinY = GameObjectLookupTables.RotationSinCosTable[rotationYAmount];
                int cosY = GameObjectLookupTables.RotationSinCosTable[rotationYAmount + 256];
                int rotatedZ = gameObject.WorldVertZ[vertexIndex] * sinY + gameObject.WorldVertX[vertexIndex] * cosY >> 15;
                gameObject.WorldVertZ[vertexIndex] = gameObject.WorldVertZ[vertexIndex] * cosY - gameObject.WorldVertX[vertexIndex] * sinY >> 15;
                gameObject.WorldVertX[vertexIndex] = rotatedZ;
            }
        }

        private static void ScaleVertices(GameObject gameObject, int x, int y, int z)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] = gameObject.WorldVertX[vertexIndex] * x >> 8;
                gameObject.WorldVertY[vertexIndex] = gameObject.WorldVertY[vertexIndex] * y >> 8;
                gameObject.WorldVertZ[vertexIndex] = gameObject.WorldVertZ[vertexIndex] * z >> 8;
            }
        }

        private static void ScaleVerticesShear(
            GameObject gameObject,
            int shearXY,
            int shearZY,
            int shearXZ,
            int shearYZ,
            int shearZX,
            int shearYX)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                ApplyShearToVertex(gameObject, vertexIndex, shearXY, shearZY, shearXZ, shearYZ, shearZX, shearYX);
            }
        }

        private static void ApplyShearToVertex(
            GameObject gameObject,
            int vertexIndex,
            int shearXY,
            int shearZY,
            int shearXZ,
            int shearYZ,
            int shearZX,
            int shearYX)
        {
            if (shearXY != 0)
            {
                gameObject.WorldVertX[vertexIndex] += gameObject.WorldVertY[vertexIndex] * shearXY >> 8;
            }

            if (shearZY != 0)
            {
                gameObject.WorldVertZ[vertexIndex] += gameObject.WorldVertY[vertexIndex] * shearZY >> 8;
            }

            if (shearXZ != 0)
            {
                gameObject.WorldVertX[vertexIndex] += gameObject.WorldVertZ[vertexIndex] * shearXZ >> 8;
            }

            if (shearYZ != 0)
            {
                gameObject.WorldVertY[vertexIndex] += gameObject.WorldVertZ[vertexIndex] * shearYZ >> 8;
            }

            if (shearZX != 0)
            {
                gameObject.WorldVertZ[vertexIndex] += gameObject.WorldVertX[vertexIndex] * shearZX >> 8;
            }

            if (shearYX != 0)
            {
                gameObject.WorldVertY[vertexIndex] += gameObject.WorldVertX[vertexIndex] * shearYX >> 8;
            }
        }

        private static void ProjectSingleVertex(GameObject gameObject, int vertexIndex, VertexProjectionContext context)
        {
            int vertX = gameObject.WorldVertX[vertexIndex] - context.OriginX;
            int vertY = gameObject.WorldVertY[vertexIndex] - context.OriginY;
            int vertZ = gameObject.WorldVertZ[vertexIndex] - context.OriginZ;

            if (context.RotationZAngle != 0)
            {
                int rotated = vertY * context.SinZ + vertX * context.CosZ >> 15;
                vertY = vertY * context.CosZ - vertX * context.SinZ >> 15;
                vertX = rotated;
            }

            if (context.RotationYAngle != 0)
            {
                int rotated = vertZ * context.SinY + vertX * context.CosY >> 15;
                vertZ = vertZ * context.CosY - vertX * context.SinY >> 15;
                vertX = rotated;
            }

            if (context.RotationXAngle != 0)
            {
                int rotated = vertY * context.CosX - vertZ * context.SinX >> 15;
                vertZ = vertY * context.SinX + vertZ * context.CosX >> 15;
                vertY = rotated;
            }

            gameObject.ProjectedU[vertexIndex] = vertZ >= context.NearPlane
                ? (vertX << context.ProjectionScale) / vertZ
                : vertX << context.ProjectionScale;

            gameObject.ProjectedV[vertexIndex] = vertZ >= context.NearPlane
                ? (vertY << context.ProjectionScale) / vertZ
                : vertY << context.ProjectionScale;

            gameObject.ProjectedX[vertexIndex] = vertX;
            gameObject.ProjectedY[vertexIndex] = vertY;
            gameObject.ProjectedDepth[vertexIndex] = vertZ;
        }
    }
}
