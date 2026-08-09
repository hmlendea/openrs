namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectTransformer
    {
        private static int FarBound => 0x98967f;

        private static int RotationShift => 15;

        private static int TransformShift => 8;

        private static int RotationCosineTableOffset => 256;

        private static int FineRotationCosineTableOffset => 1024;

        internal static void ApplyNullTransform(GameObject gameObject)
        {
            gameObject.ObjectState = 0;

            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] = gameObject.VertexCoordinatesX[vertexIndex];
                gameObject.WorldVertY[vertexIndex] = gameObject.VertexCoordinatesY[vertexIndex];
                gameObject.WorldVertZ[vertexIndex] = gameObject.VertexCoordinatesZ[vertexIndex];
            }

            gameObject.MaximumFaceSpan = FarBound;
            gameObject.BoundsMaxX = FarBound;
            gameObject.BoundsMaxY = FarBound;
            gameObject.BoundsMaxZ = FarBound;
            gameObject.BoundsMinX = -FarBound;
            gameObject.BoundsMinY = -FarBound;
            gameObject.BoundsMinZ = -FarBound;
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
                ScaleVerticesShear(
                    gameObject,
                    secondaryScaleX,
                    secondaryScaleY,
                    secondaryScaleZ,
                    tertiaryScaleX,
                    tertiaryScaleY,
                    tertiaryScaleZ);
            }

            if (transformType >= 1)
            {
                OffsetWorldVertices(gameObject, positionX, positionY, positionZ);
            }

            GameObjectShaderCalculator.CalculateBounds(gameObject);
            GameObjectShaderCalculator.CalculatePolygonNormals(gameObject);
        }

        internal static void ProjectVertices(
            GameObject gameObject,
            VertexProjectionContext context)
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

        private static void RotateVertices(
            GameObject gameObject,
            int rotationXAmount,
            int rotationYAmount,
            int rotationZAmount)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                RotateSingleVertex(
                    gameObject,
                    vertexIndex,
                    rotationXAmount,
                    rotationYAmount,
                    rotationZAmount);
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
                RotateVertexAroundZ(gameObject, vertexIndex, rotationZAmount);
            }

            if (rotationXAmount != 0)
            {
                RotateVertexAroundX(gameObject, vertexIndex, rotationXAmount);
            }

            if (rotationYAmount != 0)
            {
                RotateVertexAroundY(gameObject, vertexIndex, rotationYAmount);
            }
        }

        private static void RotateVertexAroundZ(
            GameObject gameObject,
            int vertexIndex,
            int rotationAmount)
        {
            int sine = GameObjectLookupTables.RotationSinCosTable[rotationAmount];
            int cosine = GameObjectLookupTables.RotationSinCosTable[
                rotationAmount + RotationCosineTableOffset];
            int rotatedX =
                (gameObject.WorldVertY[vertexIndex] * sine +
                gameObject.WorldVertX[vertexIndex] * cosine) >> RotationShift;
            gameObject.WorldVertY[vertexIndex] =
                (gameObject.WorldVertY[vertexIndex] * cosine -
                gameObject.WorldVertX[vertexIndex] * sine) >> RotationShift;
            gameObject.WorldVertX[vertexIndex] = rotatedX;
        }

        private static void RotateVertexAroundX(
            GameObject gameObject,
            int vertexIndex,
            int rotationAmount)
        {
            int sine = GameObjectLookupTables.RotationSinCosTable[rotationAmount];
            int cosine = GameObjectLookupTables.RotationSinCosTable[
                rotationAmount + RotationCosineTableOffset];
            int rotatedY =
                (gameObject.WorldVertY[vertexIndex] * cosine -
                gameObject.WorldVertZ[vertexIndex] * sine) >> RotationShift;
            gameObject.WorldVertZ[vertexIndex] =
                (gameObject.WorldVertY[vertexIndex] * sine +
                gameObject.WorldVertZ[vertexIndex] * cosine) >> RotationShift;
            gameObject.WorldVertY[vertexIndex] = rotatedY;
        }

        private static void RotateVertexAroundY(
            GameObject gameObject,
            int vertexIndex,
            int rotationAmount)
        {
            int sine = GameObjectLookupTables.RotationSinCosTable[rotationAmount];
            int cosine = GameObjectLookupTables.RotationSinCosTable[
                rotationAmount + RotationCosineTableOffset];
            int rotatedZ =
                (gameObject.WorldVertZ[vertexIndex] * sine +
                gameObject.WorldVertX[vertexIndex] * cosine) >> RotationShift;
            gameObject.WorldVertZ[vertexIndex] =
                (gameObject.WorldVertZ[vertexIndex] * cosine -
                gameObject.WorldVertX[vertexIndex] * sine) >> RotationShift;
            gameObject.WorldVertX[vertexIndex] = rotatedZ;
        }

        private static void ScaleVertices(GameObject gameObject, int x, int y, int z)
        {
            for (int vertexIndex = 0; vertexIndex < gameObject.VertexCount; vertexIndex += 1)
            {
                gameObject.WorldVertX[vertexIndex] =
                    gameObject.WorldVertX[vertexIndex] * x >> TransformShift;
                gameObject.WorldVertY[vertexIndex] =
                    gameObject.WorldVertY[vertexIndex] * y >> TransformShift;
                gameObject.WorldVertZ[vertexIndex] =
                    gameObject.WorldVertZ[vertexIndex] * z >> TransformShift;
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
                ApplyShearToVertex(
                    gameObject,
                    vertexIndex,
                    shearXY,
                    shearZY,
                    shearXZ,
                    shearYZ,
                    shearZX,
                    shearYX);
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
                gameObject.WorldVertX[vertexIndex] +=
                    gameObject.WorldVertY[vertexIndex] * shearXY >> TransformShift;
            }

            if (shearZY != 0)
            {
                gameObject.WorldVertZ[vertexIndex] +=
                    gameObject.WorldVertY[vertexIndex] * shearZY >> TransformShift;
            }

            if (shearXZ != 0)
            {
                gameObject.WorldVertX[vertexIndex] +=
                    gameObject.WorldVertZ[vertexIndex] * shearXZ >> TransformShift;
            }

            if (shearYZ != 0)
            {
                gameObject.WorldVertY[vertexIndex] +=
                    gameObject.WorldVertZ[vertexIndex] * shearYZ >> TransformShift;
            }

            if (shearZX != 0)
            {
                gameObject.WorldVertZ[vertexIndex] +=
                    gameObject.WorldVertX[vertexIndex] * shearZX >> TransformShift;
            }

            if (shearYX != 0)
            {
                gameObject.WorldVertY[vertexIndex] +=
                    gameObject.WorldVertX[vertexIndex] * shearYX >> TransformShift;
            }
        }

        private static void ProjectSingleVertex(
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

