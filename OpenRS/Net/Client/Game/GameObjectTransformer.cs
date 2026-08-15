namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectTransformer
    {
        private static int FarBound => 0x98967f;

        private static int RotationShift => 15;

        private static int TransformShift => 8;

        private static int RotationCosineTableOffset => 256;

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

            GameObjectBoundsCalculator.Calculate(gameObject);
            GameObjectNormalCalculator.CalculatePolygonNormals(gameObject);
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

    }
}
