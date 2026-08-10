using System.Text.Json;

using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Loading
{
    internal static class ModelTextureMetadata
    {
        private static string FaceCountPropertyName => "faceCount";

        private static string TextureBackPropertyName => "textureBack";

        private static string TextureFrontPropertyName => "textureFront";

        private static string GouraudShadePropertyName => "gouraudShade";

        private static string FaceVertexCountsPropertyName => "faceVertexCounts";

        private static int NoTextureSentinel => GameObject.DefaultShadeValue;

        private static string ExtrasPropertyName => "extras";

        private static string EmbeddedTextureMetadataPropertyName => "modelTextureSidecar";

        public static bool TryApplyToModel(GameObject targetModel, JsonElement gltfRoot)
        {
            if (targetModel is null)
            {
                return false;
            }

            if (!gltfRoot.TryGetProperty(ExtrasPropertyName, out JsonElement extrasElement))
            {
                return false;
            }

            if (!extrasElement.TryGetProperty(EmbeddedTextureMetadataPropertyName, out JsonElement textureMetadataElement))
            {
                return false;
            }

            return TryApplyTextureMetadata(targetModel, textureMetadataElement);
        }

        private static bool TryApplyTextureMetadata(GameObject targetModel, JsonElement root)
        {
            if (!root.TryGetProperty(FaceCountPropertyName, out JsonElement faceCountElement))
            {
                return false;
            }

            int faceCount = faceCountElement.GetInt32();
            int[] textureBack = ReadIntArray(root, TextureBackPropertyName, faceCount);
            int[] textureFront = ReadIntArray(root, TextureFrontPropertyName, faceCount);
            int[] gouraudShade = ReadIntArray(root, GouraudShadePropertyName, faceCount);

            if (textureBack is null || textureFront is null || gouraudShade is null)
            {
                return false;
            }

            if (faceCount == targetModel.FaceCount)
            {
                ApplyDirectFaceMapping(targetModel, faceCount, textureBack, textureFront, gouraudShade);
                return true;
            }

            int[] faceVertexCounts = ReadIntArray(root, FaceVertexCountsPropertyName, faceCount);

            if (faceVertexCounts is null)
            {
                ApplyProportionalFallback(
                    targetModel,
                    textureBack,
                    textureFront,
                    gouraudShade);
                return true;
            }

            int triangulatedFaceCount = CalculateTriangulatedFaceCount(faceVertexCounts);

            if (triangulatedFaceCount != targetModel.FaceCount)
            {
                return false;
            }

            int targetFaceIndex = 0;

            for (int sourceFaceIndex = 0; sourceFaceIndex < faceCount; sourceFaceIndex += 1)
            {
                int sourceFaceVertexCount = faceVertexCounts[sourceFaceIndex];
                int triangleCountForFace = CalculateTriangleCountForFace(sourceFaceVertexCount);

                for (int triangleIndex = 0; triangleIndex < triangleCountForFace; triangleIndex += 1)
                {
                    ApplySingleFaceTexture(
                        targetModel,
                        targetFaceIndex,
                        textureBack[sourceFaceIndex],
                        textureFront[sourceFaceIndex],
                        gouraudShade[sourceFaceIndex]);
                    targetFaceIndex += 1;
                }
            }

            return true;
        }

        private static void ApplyProportionalFallback(
            GameObject targetModel,
            int[] sourceTextureBack,
            int[] sourceTextureFront,
            int[] sourceGouraudShade)
        {
            int sourceFaceCount = sourceTextureBack.Length;
            int targetFaceCount = targetModel.FaceCount;

            for (int targetFaceIndex = 0; targetFaceIndex < targetFaceCount; targetFaceIndex += 1)
            {
                int sourceFaceIndex = (int)((long)targetFaceIndex * sourceFaceCount / targetFaceCount);

                if (sourceFaceIndex < 0)
                {
                    sourceFaceIndex = 0;
                }
                else if (sourceFaceIndex >= sourceFaceCount)
                {
                    sourceFaceIndex = sourceFaceCount - 1;
                }

                ApplySingleFaceTexture(
                    targetModel,
                    targetFaceIndex,
                    sourceTextureBack[sourceFaceIndex],
                    sourceTextureFront[sourceFaceIndex],
                    sourceGouraudShade[sourceFaceIndex]);
            }
        }

        private static void ApplyDirectFaceMapping(
            GameObject targetModel,
            int faceCount,
            int[] textureBack,
            int[] textureFront,
            int[] gouraudShade)
        {
            for (int faceIndex = 0; faceIndex < faceCount; faceIndex += 1)
            {
                ApplySingleFaceTexture(
                    targetModel,
                    faceIndex,
                    textureBack[faceIndex],
                    textureFront[faceIndex],
                    gouraudShade[faceIndex]);
            }
        }

        private static void ApplySingleFaceTexture(
            GameObject targetModel,
            int faceIndex,
            int backTexture,
            int frontTexture,
            int gouraudShade)
        {
            if (frontTexture == NoTextureSentinel && backTexture != NoTextureSentinel)
            {
                frontTexture = backTexture;
            }

            if (backTexture == NoTextureSentinel && frontTexture != NoTextureSentinel)
            {
                backTexture = frontTexture;
            }

            targetModel.TextureBack[faceIndex] = backTexture;
            targetModel.TextureFront[faceIndex] = frontTexture;
            targetModel.GouraudShade[faceIndex] = gouraudShade;
        }

        private static int CalculateTriangulatedFaceCount(int[] faceVertexCounts)
        {
            int totalTriangles = 0;

            for (int faceIndex = 0; faceIndex < faceVertexCounts.Length; faceIndex += 1)
            {
                totalTriangles += CalculateTriangleCountForFace(faceVertexCounts[faceIndex]);
            }

            return totalTriangles;
        }

        private static int CalculateTriangleCountForFace(int faceVertexCount)
        {
            if (faceVertexCount >= 3)
            {
                return faceVertexCount - 2;
            }

            return 1;
        }

        private static int[] ReadIntArray(JsonElement root, string propertyName, int expectedLength)
        {
            if (!root.TryGetProperty(propertyName, out JsonElement arrayElement) ||
                arrayElement.ValueKind != JsonValueKind.Array ||
                arrayElement.GetArrayLength() != expectedLength)
            {
                return null;
            }

            int[] values = new int[expectedLength];

            for (int elementIndex = 0; elementIndex < expectedLength; elementIndex += 1)
            {
                values[elementIndex] = arrayElement[elementIndex].GetInt32();
            }

            return values;
        }
    }
}
