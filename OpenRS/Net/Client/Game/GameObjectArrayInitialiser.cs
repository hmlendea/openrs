using Microsoft.Xna.Framework;

namespace OpenRS.Net.Client.Game
{
    internal static class GameObjectArrayInitialiser
    {
        internal static void Initialise(
            GameObject gameObject,
            int vertexCapacity,
            int faceCapacity)
        {
            AllocateRequiredArrays(gameObject, vertexCapacity, faceCapacity);
            AllocateOptionalArrays(gameObject, vertexCapacity, faceCapacity);
            ResetState(gameObject, vertexCapacity, faceCapacity);
        }

        private static void AllocateRequiredArrays(
            GameObject gameObject,
            int vertexCapacity,
            int faceCapacity)
        {
            gameObject.VertexCoordinatesX = new int[vertexCapacity];
            gameObject.VertexCoordinatesY = new int[vertexCapacity];
            gameObject.VertexCoordinatesZ = new int[vertexCapacity];
            gameObject.VertexVectors = new Vector3[vertexCapacity];
            gameObject.FaceNormalComponent = new int[vertexCapacity];
            gameObject.VertexColour = new int[vertexCapacity];
            gameObject.FaceVertexCounts = new int[faceCapacity];
            gameObject.FaceVertexIndices = new int[faceCapacity][];
            gameObject.TextureBack = new int[faceCapacity];
            gameObject.TextureFront = new int[faceCapacity];
            gameObject.GouraudShade = new int[faceCapacity];
            gameObject.FaceRenderFlag = new int[faceCapacity];
            gameObject.FaceVisibility = new int[faceCapacity];
        }

        private static void AllocateOptionalArrays(
            GameObject gameObject,
            int vertexCapacity,
            int faceCapacity)
        {
            AllocateProjectionArrays(gameObject, vertexCapacity);
            AllocateEntityArrays(gameObject, faceCapacity);
            AllocateWorldVertexArrays(gameObject, vertexCapacity);
            AllocateNormalArrays(gameObject, faceCapacity);
            AllocateFaceBounds(gameObject, faceCapacity);
        }

        private static void AllocateProjectionArrays(GameObject gameObject, int vertexCapacity)
        {
            if (gameObject.DoesShareVertexArrays)
            {
                return;
            }

            gameObject.ProjectedX = new int[vertexCapacity];
            gameObject.ProjectedY = new int[vertexCapacity];
            gameObject.ProjectedDepth = new int[vertexCapacity];
            gameObject.ProjectedU = new int[vertexCapacity];
            gameObject.ProjectedV = new int[vertexCapacity];
        }

        private static void AllocateEntityArrays(GameObject gameObject, int faceCapacity)
        {
            if (gameObject.DoesShareEntityArrays)
            {
                return;
            }

            gameObject.PolygonTypeData = new int[faceCapacity];
            gameObject.EntityType = new int[faceCapacity];
        }

        private static void AllocateWorldVertexArrays(GameObject gameObject, int vertexCapacity)
        {
            if (gameObject.DoesShareWorldVertices)
            {
                gameObject.WorldVertX = gameObject.VertexCoordinatesX;
                gameObject.WorldVertY = gameObject.VertexCoordinatesY;
                gameObject.WorldVertZ = gameObject.VertexCoordinatesZ;
                return;
            }

            gameObject.WorldVertX = new int[vertexCapacity];
            gameObject.WorldVertY = new int[vertexCapacity];
            gameObject.WorldVertZ = new int[vertexCapacity];
        }

        private static void AllocateNormalArrays(GameObject gameObject, int faceCapacity)
        {
            if (gameObject.DoesNotReceiveShadows && gameObject.HasNoCollider)
            {
                return;
            }

            gameObject.NormalX = new int[faceCapacity];
            gameObject.NormalY = new int[faceCapacity];
            gameObject.NormalZ = new int[faceCapacity];
        }

        private static void AllocateFaceBounds(GameObject gameObject, int faceCapacity)
        {
            if (gameObject.HasNoCollider)
            {
                return;
            }

            gameObject.faceBoundsMinX = new int[faceCapacity];
            gameObject.faceBoundsMaxX = new int[faceCapacity];
            gameObject.faceBoundsMinY = new int[faceCapacity];
            gameObject.faceBoundsMaxY = new int[faceCapacity];
            gameObject.faceBoundsMinZ = new int[faceCapacity];
            gameObject.faceBoundsMaxZ = new int[faceCapacity];
        }

        private static void ResetState(
            GameObject gameObject,
            int vertexCapacity,
            int faceCapacity)
        {
            gameObject.FaceCount = 0;
            gameObject.VertexCount = 0;
            gameObject.TotalVertexCapacity = vertexCapacity;
            gameObject.TotalFaceCapacity = faceCapacity;
            gameObject.positionX = 0;
            gameObject.positionY = 0;
            gameObject.positionZ = 0;
            gameObject.rotationX = 0;
            gameObject.rotationY = 0;
            gameObject.rotationZ = 0;
            gameObject.scaleX = GameObject.DefaultScale;
            gameObject.scaleY = GameObject.DefaultScale;
            gameObject.scaleZ = GameObject.DefaultScale;
            gameObject.secondaryScaleX = GameObject.DefaultScale;
            gameObject.secondaryScaleY = GameObject.DefaultScale;
            gameObject.secondaryScaleZ = GameObject.DefaultScale;
            gameObject.tertiaryScaleX = GameObject.DefaultScale;
            gameObject.tertiaryScaleY = GameObject.DefaultScale;
            gameObject.tertiaryScaleZ = GameObject.DefaultScale;
            gameObject.transformType = 0;
        }
    }
}