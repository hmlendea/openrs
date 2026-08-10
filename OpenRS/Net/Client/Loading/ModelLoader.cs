using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Buffers.Binary;

using Microsoft.Xna.Framework;

using OpenRS.GameLogic.GameManagers;
using OpenRS.Net.Client.Game;

namespace OpenRS.Net.Client.Loading
{
    internal static class ModelLoader
    {
        private static uint GlbMagic => 0x46546C67;

        private static int GlbHeaderLength => 12;

        private static int ChunkHeaderLength => 8;

        private static int UnsignedShortComponentType => 5123;

        private static int UnsignedIntComponentType => 5125;

        private static int UnsignedByteComponentType => 5121;

        private static int FloatComponentType => 5126;

        private static int PositionComponents => 3;

        private static int FloatSizeInBytes => 4;

        private static int LargeScaleFactor => 256;

        private static int NativeScaleFactor => 1;

        private static float SmallCoordinateThreshold => 8f;

        private static int DefaultFaceColourIndex => -25369;

        private static int TrianglePrimitiveMode => 4;

        private static int MissingPrimitiveToken => -1;

        private static int MeshIndexShift => 16;

        private static int PrimitiveIndexMask => 0xffff;

        private static int MatrixElementCount => 16;

        private static int MatrixAxisCount => 4;

        private static int PrimitiveScoreNormalWeight => 100000000;

        private static int PrimitiveScoreUvWeight => 1000000;

        private static int PrimitiveScoreMaterialWeight => 10000;

        private static char NormalisedNameSeparator => '_';

        public static GameObject Load(string path)
            => Load(path, null);

        public static GameObject Load(string path, EntityManager entityManager)
        {
            byte[] data = File.ReadAllBytes(path);

            // Basic GLB header validation
            if (data.Length < GlbHeaderLength)
            {
                throw new InvalidDataException("GLB file too small");
            }

            uint magic = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(0, 4));
            if (magic != GlbMagic) // 'glTF'
            {
                throw new InvalidDataException("Not a GLB file");
            }

            // skip version and length
            int offset = GlbHeaderLength;

            // read JSON chunk
            if (offset + ChunkHeaderLength > data.Length)
            {
                throw new InvalidDataException("Invalid GLB");
            }

            int jsonChunkLength = (int)BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
            string jsonChunkType = Encoding.ASCII.GetString(data, offset + 4, 4);
            offset += ChunkHeaderLength;

            if (jsonChunkType != "JSON")
            {
                throw new InvalidDataException("First GLB chunk is not JSON");
            }

            string jsonText = Encoding.UTF8.GetString(data, offset, jsonChunkLength);
            offset += jsonChunkLength;

            // align
            if ((offset & 3) != 0)
            {
                offset += 4 - (offset & 3);
            }

            // next chunk should be BIN
            if (offset + ChunkHeaderLength > data.Length)
            {
                throw new InvalidDataException("Missing BIN chunk");
            }

            int binChunkLength = (int)BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
            string binChunkType = Encoding.ASCII.GetString(data, offset + 4, 4);
            offset += ChunkHeaderLength;

            if (binChunkType != "BIN\0")
            {
                throw new InvalidDataException("Second GLB chunk is not BIN");
            }

            byte[] bin = new byte[binChunkLength];
            Array.Copy(data, offset, bin, 0, Math.Min(binChunkLength, data.Length - offset));

            using JsonDocument doc = JsonDocument.Parse(jsonText);
            JsonElement root = doc.RootElement;

            // Locate the most representative primitive.
            if (!root.TryGetProperty("meshes", out JsonElement meshes) || meshes.GetArrayLength() == 0)
            {
                throw new InvalidDataException("No meshes in GLB");
            }

            if (!root.TryGetProperty("accessors", out JsonElement accessors))
            {
                throw new InvalidDataException("No accessors in GLB");
            }

            if (!root.TryGetProperty("bufferViews", out JsonElement bufferViews))
            {
                throw new InvalidDataException("No bufferViews in GLB");
            }

            int primitiveToken = FindBestPrimitiveToken(meshes, accessors);
            if (primitiveToken == MissingPrimitiveToken)
            {
                throw new InvalidDataException("No suitable primitives in GLB");
            }

            int selectedMeshIndex = primitiveToken >> MeshIndexShift;
            int selectedPrimitiveIndex = primitiveToken & PrimitiveIndexMask;
            JsonElement selectedMesh = meshes[selectedMeshIndex];
            JsonElement selectedPrimitive = selectedMesh.GetProperty("primitives")[selectedPrimitiveIndex];
            float[] meshTransform = FindMeshTransform(root, selectedMeshIndex);
            int materialTextureIndex = ResolvePrimitiveTextureIndex(root, selectedPrimitive, entityManager);

            int positionsAccessor = -1;
            int indicesAccessor = -1;

            if (selectedPrimitive.TryGetProperty("attributes", out JsonElement attributes))
            {
                if (attributes.TryGetProperty("POSITION", out JsonElement posEl))
                {
                    positionsAccessor = posEl.GetInt32();
                }
            }

            if (selectedPrimitive.TryGetProperty("indices", out JsonElement idxEl))
            {
                indicesAccessor = idxEl.GetInt32();
            }

            if (positionsAccessor < 0 || indicesAccessor < 0)
            {
                throw new InvalidDataException("POSITION or indices accessor missing");
            }

            JsonElement posAccessor = accessors[positionsAccessor];
            JsonElement idxAccessor = accessors[indicesAccessor];

            int posCount = posAccessor.GetProperty("count").GetInt32();
            int idxCount = idxAccessor.GetProperty("count").GetInt32();

            int posBufferView = posAccessor.GetProperty("bufferView").GetInt32();
            int idxBufferView = idxAccessor.GetProperty("bufferView").GetInt32();

            JsonElement posBv = bufferViews[posBufferView];
            JsonElement idxBv = bufferViews[idxBufferView];

            int posBufferOffset = posBv.TryGetProperty("byteOffset", out JsonElement bo)
                ? bo.GetInt32()
                : 0;
            int idxBufferOffset = idxBv.TryGetProperty("byteOffset", out JsonElement ibo)
                ? ibo.GetInt32()
                : 0;

            int posAccessorOffset = posAccessor.TryGetProperty("byteOffset", out JsonElement posAo)
                ? posAo.GetInt32()
                : 0;
            int idxAccessorOffset = idxAccessor.TryGetProperty("byteOffset", out JsonElement idxAo)
                ? idxAo.GetInt32()
                : 0;

            int posOffset = posBufferOffset + posAccessorOffset;
            int idxOffset = idxBufferOffset + idxAccessorOffset;

            int posByteStride = posBv.TryGetProperty("byteStride", out JsonElement strideEl)
                ? strideEl.GetInt32()
                : PositionComponents * FloatSizeInBytes;

            // component types
            int idxComponentType = idxAccessor.GetProperty("componentType").GetInt32();
            int posComponentType = posAccessor.GetProperty("componentType").GetInt32();
            int idxComponentSize = GetIndexComponentSize(idxComponentType);
            int idxByteStride = idxBv.TryGetProperty("byteStride", out JsonElement idxStrideEl)
                ? idxStrideEl.GetInt32()
                : idxComponentSize;

            if (posComponentType != FloatComponentType)
            {
                throw new InvalidDataException("Unsupported position component type");
            }

            // read positions (FLOAT32)
            float[] positions = new float[posCount * PositionComponents];

            for (int vertexIndex = 0; vertexIndex < posCount; vertexIndex += 1)
            {
                int vertexByteOffset = posOffset + vertexIndex * posByteStride;
                positions[vertexIndex * PositionComponents + 0] =
                    BinaryPrimitives.ReadSingleLittleEndian(bin.AsSpan(vertexByteOffset + 0, FloatSizeInBytes));
                positions[vertexIndex * PositionComponents + 1] =
                    BinaryPrimitives.ReadSingleLittleEndian(bin.AsSpan(vertexByteOffset + FloatSizeInBytes, FloatSizeInBytes));
                positions[vertexIndex * PositionComponents + 2] =
                    BinaryPrimitives.ReadSingleLittleEndian(bin.AsSpan(vertexByteOffset + FloatSizeInBytes * 2, FloatSizeInBytes));
            }

            // read indices
            int[] indices;

            if (idxComponentType == UnsignedByteComponentType) // UNSIGNED_BYTE
            {
                indices = new int[idxCount];
                for (int i = 0; i < idxCount; i += 1)
                {
                    indices[i] = bin[idxOffset + i * idxByteStride] & 0xff;
                }
            }
            else if (idxComponentType == UnsignedShortComponentType) // UNSIGNED_SHORT
            {
                indices = new int[idxCount];
                for (int i = 0; i < idxCount; i += 1)
                {
                    indices[i] = BinaryPrimitives.ReadUInt16LittleEndian(
                        bin.AsSpan(idxOffset + i * idxByteStride, 2));
                }
            }
            else if (idxComponentType == UnsignedIntComponentType) // UNSIGNED_INT
            {
                indices = new int[idxCount];
                for (int i = 0; i < idxCount; i += 1)
                {
                    indices[i] = (int)BinaryPrimitives.ReadUInt32LittleEndian(
                        bin.AsSpan(idxOffset + i * idxByteStride, 4));
                }
            }
            else
            {
                throw new InvalidDataException("Unsupported index component type");
            }

            // build GameObject
            int vertexCount = posCount;
            int triangleCount = indices.Length / 3;

            GameObject go = new GameObject(vertexCount, triangleCount);

            Vector3[] transformedPositions = new Vector3[vertexCount];
            float largestAbsoluteCoordinate = 0f;

            for (int vertexIndex = 0; vertexIndex < vertexCount; vertexIndex += 1)
            {
                float sourceX = positions[vertexIndex * PositionComponents + 0];
                float sourceY = positions[vertexIndex * PositionComponents + 1];
                float sourceZ = positions[vertexIndex * PositionComponents + 2];

                Vector3 transformedPosition =
                    TransformPosition(meshTransform, sourceX, sourceY, sourceZ);

                transformedPositions[vertexIndex] = transformedPosition;

                float absoluteX = Math.Abs(transformedPosition.X);
                float absoluteY = Math.Abs(transformedPosition.Y);
                float absoluteZ = Math.Abs(transformedPosition.Z);

                largestAbsoluteCoordinate = Math.Max(largestAbsoluteCoordinate, absoluteX);
                largestAbsoluteCoordinate = Math.Max(largestAbsoluteCoordinate, absoluteY);
                largestAbsoluteCoordinate = Math.Max(largestAbsoluteCoordinate, absoluteZ);
            }

            float scale = SelectVertexScale(largestAbsoluteCoordinate);

            // fill vertices using AddVertex so counts are correct
            for (int i = 0; i < vertexCount; i++)
            {
                float x = transformedPositions[i].X;
                float y = transformedPositions[i].Y;
                float z = transformedPositions[i].Z;

                int xi = (int)Math.Round(x * scale);
                int yi = (int)Math.Round(y * scale);
                int zi = (int)Math.Round(z * scale);

                go.AddVertex(xi, yi, zi);
                go.VertexVectors[i] = new Vector3(xi, yi, zi);
            }

            // fill faces using AddFaceVertices
            for (int t = 0; t < triangleCount; t++)
            {
                int a = indices[t * 3 + 0];
                int b = indices[t * 3 + 1];
                int c = indices[t * 3 + 2];

                int[] faceVerts = [a, b, c];
                int faceTextureIndex = materialTextureIndex;

                if (faceTextureIndex < 0)
                {
                    faceTextureIndex = DefaultFaceColourIndex;
                }

                go.AddFaceVertices(3, faceVerts, faceTextureIndex, faceTextureIndex);
            }

            ModelTextureMetadata.TryApplyToModel(go, root);

            go.ObjectState = 1;

            return go;
        }

        private static int ResolvePrimitiveTextureIndex(
            JsonElement root,
            JsonElement primitive,
            EntityManager entityManager)
        {
            if (entityManager is null)
            {
                return -1;
            }

            if (!primitive.TryGetProperty("material", out JsonElement materialIndexElement))
            {
                return -1;
            }

            if (!root.TryGetProperty("materials", out JsonElement materials) ||
                materials.GetArrayLength() == 0)
            {
                return -1;
            }

            int materialIndex = materialIndexElement.GetInt32();

            if (materialIndex < 0 || materialIndex >= materials.GetArrayLength())
            {
                return -1;
            }

            JsonElement material = materials[materialIndex];
            List<string> candidateNames = BuildMaterialCandidateNames(root, material);

            return ResolveTextureIndexFromCandidates(entityManager, candidateNames);
        }

        private static List<string> BuildMaterialCandidateNames(JsonElement root, JsonElement material)
        {
            List<string> candidates = new();

            if (material.TryGetProperty("name", out JsonElement materialNameElement))
            {
                string materialName = materialNameElement.GetString();

                if (!string.IsNullOrWhiteSpace(materialName))
                {
                    candidates.Add(materialName);
                }
            }

            string imageName = ResolveMaterialImageName(root, material);

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                candidates.Add(imageName);
            }

            return candidates;
        }

        private static string ResolveMaterialImageName(JsonElement root, JsonElement material)
        {
            if (!material.TryGetProperty("pbrMetallicRoughness", out JsonElement pbrElement))
            {
                return string.Empty;
            }

            if (!pbrElement.TryGetProperty("baseColorTexture", out JsonElement baseColorTextureElement))
            {
                return string.Empty;
            }

            if (!baseColorTextureElement.TryGetProperty("index", out JsonElement textureIndexElement))
            {
                return string.Empty;
            }

            if (!root.TryGetProperty("textures", out JsonElement textures) ||
                !root.TryGetProperty("images", out JsonElement images))
            {
                return string.Empty;
            }

            int textureIndex = textureIndexElement.GetInt32();

            if (textureIndex < 0 || textureIndex >= textures.GetArrayLength())
            {
                return string.Empty;
            }

            JsonElement textureElement = textures[textureIndex];

            if (!textureElement.TryGetProperty("source", out JsonElement sourceElement))
            {
                return string.Empty;
            }

            int imageIndex = sourceElement.GetInt32();

            if (imageIndex < 0 || imageIndex >= images.GetArrayLength())
            {
                return string.Empty;
            }

            JsonElement imageElement = images[imageIndex];

            if (imageElement.TryGetProperty("name", out JsonElement imageNameElement))
            {
                string imageName = imageNameElement.GetString();

                if (!string.IsNullOrWhiteSpace(imageName))
                {
                    return imageName;
                }
            }

            if (imageElement.TryGetProperty("uri", out JsonElement imageUriElement))
            {
                string imageUri = imageUriElement.GetString();

                if (!string.IsNullOrWhiteSpace(imageUri))
                {
                    string fileName = Path.GetFileNameWithoutExtension(imageUri);

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        return fileName;
                    }
                }
            }

            return string.Empty;
        }

        private static int ResolveTextureIndexFromCandidates(EntityManager entityManager, List<string> candidates)
        {
            HashSet<string> expandedCandidates = new(StringComparer.OrdinalIgnoreCase);

            for (int candidateIndex = 0; candidateIndex < candidates.Count; candidateIndex += 1)
            {
                string candidate = candidates[candidateIndex];

                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                string normalisedCandidate = NormaliseToken(candidate);
                expandedCandidates.Add(normalisedCandidate);

                string[] splitCandidates = normalisedCandidate.Split(
                    [NormalisedNameSeparator],
                    StringSplitOptions.RemoveEmptyEntries);

                for (int splitIndex = 0; splitIndex < splitCandidates.Length; splitIndex += 1)
                {
                    expandedCandidates.Add(splitCandidates[splitIndex]);
                }
            }

            for (int textureIndex = 0; textureIndex < entityManager.TextureCount; textureIndex += 1)
            {
                OpenRS.Models.GameTexture texture = entityManager.GetTexture(textureIndex);
                string name = texture.Name ?? string.Empty;
                string subName = texture.SubName ?? string.Empty;
                string normalisedName = NormaliseToken(name);
                string normalisedSubName = NormaliseToken(subName);
                string normalisedCombined = normalisedName;

                if (normalisedSubName.Length > 0)
                {
                    normalisedCombined = normalisedName + NormalisedNameSeparator + normalisedSubName;
                }

                if (expandedCandidates.Contains(normalisedCombined) ||
                    expandedCandidates.Contains(normalisedName) ||
                    (normalisedSubName.Length > 0 && expandedCandidates.Contains(normalisedSubName)))
                {
                    return textureIndex;
                }
            }

            return -1;
        }

        private static string NormaliseToken(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            StringBuilder builder = new();

            for (int charIndex = 0; charIndex < value.Length; charIndex += 1)
            {
                char currentCharacter = char.ToLowerInvariant(value[charIndex]);

                if ((currentCharacter >= 'a' && currentCharacter <= 'z') ||
                    (currentCharacter >= '0' && currentCharacter <= '9'))
                {
                    builder.Append(currentCharacter);
                    continue;
                }

                if (builder.Length == 0 || builder[builder.Length - 1] == NormalisedNameSeparator)
                {
                    continue;
                }

                builder.Append(NormalisedNameSeparator);
            }

            while (builder.Length > 0 && builder[builder.Length - 1] == NormalisedNameSeparator)
            {
                builder.Length -= 1;
            }

            return builder.ToString();
        }

        private static float SelectVertexScale(float largestAbsoluteCoordinate)
        {
            if (largestAbsoluteCoordinate <= SmallCoordinateThreshold)
            {
                return LargeScaleFactor;
            }

            return NativeScaleFactor;
        }

        private static int FindBestPrimitiveToken(JsonElement meshes, JsonElement accessors)
        {
            int bestPrimitiveScore = -1;
            int bestPrimitiveToken = MissingPrimitiveToken;

            for (int meshIndex = 0; meshIndex < meshes.GetArrayLength(); meshIndex += 1)
            {
                JsonElement mesh = meshes[meshIndex];

                if (!mesh.TryGetProperty("primitives", out JsonElement primitives))
                {
                    continue;
                }

                for (int primitiveIndex = 0; primitiveIndex < primitives.GetArrayLength(); primitiveIndex += 1)
                {
                    JsonElement primitive = primitives[primitiveIndex];

                    if (primitive.TryGetProperty("mode", out JsonElement modeElement) &&
                        modeElement.GetInt32() != TrianglePrimitiveMode)
                    {
                        continue;
                    }

                    if (!primitive.TryGetProperty("attributes", out JsonElement attributes) ||
                        !attributes.TryGetProperty("POSITION", out JsonElement _))
                    {
                        continue;
                    }

                    if (!primitive.TryGetProperty("indices", out JsonElement indexElement))
                    {
                        continue;
                    }

                    int accessorIndex = indexElement.GetInt32();

                    if (accessorIndex < 0 || accessorIndex >= accessors.GetArrayLength())
                    {
                        continue;
                    }

                    JsonElement indexAccessor = accessors[accessorIndex];
                    int indexCount = indexAccessor.GetProperty("count").GetInt32();
                    int triangleCount = indexCount / 3;
                    int primitiveScore = triangleCount;

                    if (attributes.TryGetProperty("NORMAL", out JsonElement _))
                    {
                        primitiveScore += PrimitiveScoreNormalWeight;
                    }

                    if (attributes.TryGetProperty("TEXCOORD_0", out JsonElement _))
                    {
                        primitiveScore += PrimitiveScoreUvWeight;
                    }

                    if (primitive.TryGetProperty("material", out JsonElement _))
                    {
                        primitiveScore += PrimitiveScoreMaterialWeight;
                    }

                    if (primitiveScore > bestPrimitiveScore)
                    {
                        bestPrimitiveScore = primitiveScore;
                        bestPrimitiveToken = (meshIndex << MeshIndexShift) | primitiveIndex;
                    }
                }
            }

            return bestPrimitiveToken;
        }

        private static float[] FindMeshTransform(JsonElement root, int meshIndex)
        {
            float[] identity = CreateIdentityMatrix();

            if (!root.TryGetProperty("nodes", out JsonElement nodes) ||
                !root.TryGetProperty("scenes", out JsonElement scenes) ||
                scenes.GetArrayLength() == 0)
            {
                return identity;
            }

            int sceneIndex = 0;

            if (root.TryGetProperty("scene", out JsonElement sceneElement))
            {
                sceneIndex = sceneElement.GetInt32();
            }

            if (sceneIndex < 0 || sceneIndex >= scenes.GetArrayLength())
            {
                sceneIndex = 0;
            }

            JsonElement scene = scenes[sceneIndex];

            if (!scene.TryGetProperty("nodes", out JsonElement rootNodeIndices))
            {
                return identity;
            }

            for (int nodeArrayIndex = 0; nodeArrayIndex < rootNodeIndices.GetArrayLength(); nodeArrayIndex += 1)
            {
                int rootNodeIndex = rootNodeIndices[nodeArrayIndex].GetInt32();
                float[] discoveredTransform =
                    FindMeshTransformInNode(nodes, rootNodeIndex, meshIndex, identity);

                if (discoveredTransform is not null)
                {
                    return discoveredTransform;
                }
            }

            return identity;
        }

        private static float[] FindMeshTransformInNode(
            JsonElement nodes,
            int nodeIndex,
            int meshIndex,
            float[] parentTransform)
        {
            if (nodeIndex < 0 || nodeIndex >= nodes.GetArrayLength())
            {
                return null;
            }

            JsonElement node = nodes[nodeIndex];
            float[] localTransform = CreateNodeTransform(node);
            float[] combinedTransform = MultiplyMatrices(parentTransform, localTransform);

            if (node.TryGetProperty("mesh", out JsonElement nodeMeshElement) &&
                nodeMeshElement.GetInt32() == meshIndex)
            {
                return combinedTransform;
            }

            if (!node.TryGetProperty("children", out JsonElement children))
            {
                return null;
            }

            for (int childArrayIndex = 0; childArrayIndex < children.GetArrayLength(); childArrayIndex += 1)
            {
                int childNodeIndex = children[childArrayIndex].GetInt32();
                float[] discoveredTransform =
                    FindMeshTransformInNode(nodes, childNodeIndex, meshIndex, combinedTransform);

                if (discoveredTransform is not null)
                {
                    return discoveredTransform;
                }
            }

            return null;
        }

        private static float[] CreateNodeTransform(JsonElement node)
        {
            if (node.TryGetProperty("matrix", out JsonElement matrixElement) &&
                matrixElement.ValueKind == JsonValueKind.Array &&
                matrixElement.GetArrayLength() == MatrixElementCount)
            {
                return ParseMatrix(matrixElement);
            }

            float tx = 0f;
            float ty = 0f;
            float tz = 0f;
            float rx = 0f;
            float ry = 0f;
            float rz = 0f;
            float rw = 1f;
            float sx = 1f;
            float sy = 1f;
            float sz = 1f;

            if (node.TryGetProperty("translation", out JsonElement translationElement) &&
                translationElement.ValueKind == JsonValueKind.Array &&
                translationElement.GetArrayLength() == 3)
            {
                tx = (float)translationElement[0].GetDouble();
                ty = (float)translationElement[1].GetDouble();
                tz = (float)translationElement[2].GetDouble();
            }

            if (node.TryGetProperty("rotation", out JsonElement rotationElement) &&
                rotationElement.ValueKind == JsonValueKind.Array &&
                rotationElement.GetArrayLength() == 4)
            {
                rx = (float)rotationElement[0].GetDouble();
                ry = (float)rotationElement[1].GetDouble();
                rz = (float)rotationElement[2].GetDouble();
                rw = (float)rotationElement[3].GetDouble();
            }

            if (node.TryGetProperty("scale", out JsonElement scaleElement) &&
                scaleElement.ValueKind == JsonValueKind.Array &&
                scaleElement.GetArrayLength() == 3)
            {
                sx = (float)scaleElement[0].GetDouble();
                sy = (float)scaleElement[1].GetDouble();
                sz = (float)scaleElement[2].GetDouble();
            }

            float[] translationMatrix = CreateTranslationMatrix(tx, ty, tz);
            float[] rotationMatrix = CreateQuaternionRotationMatrix(rx, ry, rz, rw);
            float[] scaleMatrix = CreateScaleMatrix(sx, sy, sz);

            return MultiplyMatrices(MultiplyMatrices(translationMatrix, rotationMatrix), scaleMatrix);
        }

        private static float[] ParseMatrix(JsonElement matrixElement)
        {
            float[] matrix = new float[MatrixElementCount];

            for (int matrixIndex = 0; matrixIndex < MatrixElementCount; matrixIndex += 1)
            {
                matrix[matrixIndex] = (float)matrixElement[matrixIndex].GetDouble();
            }

            return matrix;
        }

        private static float[] CreateIdentityMatrix()
        {
            float[] matrix = new float[MatrixElementCount];
            matrix[0] = 1f;
            matrix[5] = 1f;
            matrix[10] = 1f;
            matrix[15] = 1f;

            return matrix;
        }

        private static float[] CreateScaleMatrix(float x, float y, float z)
        {
            float[] matrix = CreateIdentityMatrix();
            matrix[0] = x;
            matrix[5] = y;
            matrix[10] = z;

            return matrix;
        }

        private static float[] CreateTranslationMatrix(float x, float y, float z)
        {
            float[] matrix = CreateIdentityMatrix();
            matrix[12] = x;
            matrix[13] = y;
            matrix[14] = z;

            return matrix;
        }

        private static float[] CreateQuaternionRotationMatrix(float x, float y, float z, float w)
        {
            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;
            float wx = w * x;
            float wy = w * y;
            float wz = w * z;

            float[] matrix = CreateIdentityMatrix();
            matrix[0] = 1f - 2f * (yy + zz);
            matrix[1] = 2f * (xy + wz);
            matrix[2] = 2f * (xz - wy);

            matrix[4] = 2f * (xy - wz);
            matrix[5] = 1f - 2f * (xx + zz);
            matrix[6] = 2f * (yz + wx);

            matrix[8] = 2f * (xz + wy);
            matrix[9] = 2f * (yz - wx);
            matrix[10] = 1f - 2f * (xx + yy);

            return matrix;
        }

        private static float[] MultiplyMatrices(float[] first, float[] second)
        {
            float[] result = new float[MatrixElementCount];

            for (int columnIndex = 0; columnIndex < MatrixAxisCount; columnIndex += 1)
            {
                for (int rowIndex = 0; rowIndex < MatrixAxisCount; rowIndex += 1)
                {
                    result[columnIndex * MatrixAxisCount + rowIndex] =
                        first[0 * MatrixAxisCount + rowIndex] * second[columnIndex * MatrixAxisCount + 0] +
                        first[1 * MatrixAxisCount + rowIndex] * second[columnIndex * MatrixAxisCount + 1] +
                        first[2 * MatrixAxisCount + rowIndex] * second[columnIndex * MatrixAxisCount + 2] +
                        first[3 * MatrixAxisCount + rowIndex] * second[columnIndex * MatrixAxisCount + 3];
                }
            }

            return result;
        }

        private static Vector3 TransformPosition(
            float[] matrix,
            float x,
            float y,
            float z)
        {
            float transformedX = matrix[0] * x + matrix[4] * y + matrix[8] * z + matrix[12];
            float transformedY = matrix[1] * x + matrix[5] * y + matrix[9] * z + matrix[13];
            float transformedZ = matrix[2] * x + matrix[6] * y + matrix[10] * z + matrix[14];

            return new Vector3(transformedX, transformedY, transformedZ);
        }

        private static int GetIndexComponentSize(int componentType)
        {
            if (componentType == UnsignedByteComponentType)
            {
                return 1;
            }

            if (componentType == UnsignedShortComponentType)
            {
                return 2;
            }

            if (componentType == UnsignedIntComponentType)
            {
                return 4;
            }

            throw new InvalidDataException("Unsupported index component type");
        }
    }
}
