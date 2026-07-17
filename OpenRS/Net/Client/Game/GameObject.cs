using Microsoft.Xna.Framework;

using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Net.Client.Game
{
    public sealed class GameObject
    {
        public int VertexCount;
        public int[] ProjectedX;
        public int[] ProjectedY;
        public int[] ProjectedDepth;
        public int[] ProjectedU;
        public int[] ProjectedV;
        public int[] FaceNormalComponent;
        public int[] VertexColour;
        public int FaceCount;
        public int[] FaceVertexCounts;
        public int[][] FaceVertexIndices;
        public int[] TextureBack;
        public int[] TextureFront;
        public int[] FaceVisibility;
        public int[] FaceRenderFlag;
        public int[] GouraudShade;
        public int[] NormalX;
        public int[] NormalY;
        public int[] NormalZ;
        public int ScaleBias;
        public int ObjectState;
        public bool IsVisible;
        public int BoundsMinX;
        public int BoundsMaxX;
        public int BoundsMinY;
        public int BoundsMaxY;
        public int BoundsMinZ;
        public int BoundsMaxZ;
        public bool IsTranslucent;
        public bool IsPerspectiveTextured;
        public bool IsGiantCrystal;
        public int Index;
        public int[] EntityType;
        public int[] PolygonTypeData;
        public bool DoesNotReceiveShadows;
        public bool DoesShareEntityArrays;
        public bool DoesShareVertexArrays;
        public int TotalVertexCapacity;
        public int[] VertexCoordinatesX;
        public int[] VertexCoordinatesY;
        public int[] VertexCoordinatesZ;
        public Vector3[] VertexVectors;
        public int[] WorldVertX;
        public int[] WorldVertY;
        public int[] WorldVertZ;
        public int MaximumFaceSpan;
        public int AmbientLightLevel;
        public int BaseShadeLevel;
        public int LightDirectionX;
        public int LightDirectionY;
        public int LightDirectionZ;
        public int LightMagnitude;
        public bool DoesShareWorldVertices;
        public bool HasNoCollider;

        internal int[][] polygonGroupMapping;
        internal int[] faceBoundsMinX;
        internal int[] faceBoundsMaxX;
        internal int[] faceBoundsMinY;
        internal int[] faceBoundsMaxY;
        internal int[] faceBoundsMinZ;
        internal int[] faceBoundsMaxZ;
        internal int positionX;
        internal int positionY;
        internal int positionZ;
        internal int rotationX;
        internal int rotationY;
        internal int rotationZ;
        internal int scaleX;
        internal int scaleY;
        internal int scaleZ;
        internal int secondaryScaleX;
        internal int secondaryScaleY;
        internal int secondaryScaleZ;
        internal int tertiaryScaleX;
        internal int tertiaryScaleY;
        internal int tertiaryScaleZ;
        internal int transformType;

        private int totalFaceCapacity;
        private int shadeBufferIndex;

        internal static int DefaultShadeValue => 0xbc614e;

        internal static int DefaultLightDirectionX => 180;

        internal static int DefaultLightDirectionY => 155;

        internal static int DefaultLightDirectionZ => 95;

        internal static int DefaultLightMagnitude => 256;

        internal static int DefaultAmbientLightLevel => 512;

        internal static int DefaultBaseShadeLevel => 32;

        internal static int DefaultScale => 256;

        internal static int TextureShadeMaxValue => 32767;

        internal static int ObjectStateRequiresTransform => 1;

        internal static int ObjectStateNullTransform => 2;

        internal int ShadeBufferIndex
        {
            get => shadeBufferIndex;
            set => shadeBufferIndex = value;
        }

        private static int AsciiLineFeed => 10;

        private static int AsciiCarriageReturn => 13;

        private static int ByteMaskValue => 0xff;

        private static int ShadeHighByteMultiplier => 4096;

        private static int ShadeMidByteMultiplier => 64;

        private static int ShadeValueDecodingOffset => 0x20000;

        private static int ShadeSpecialEncodedValue => 0x1e240;

        private static int NoTransformType => 0;

        public GameObject(string fileName)
        {
            GameObjectDataLoader.Initialise(this);
            sbyte[] fileData = GameObjectDataLoader.LoadFromFile(this, fileName);

            if (fileData is null)
            {
                return;
            }

            int loadedVertCount = GetShadeValue(fileData);
            int loadedFaceCount = GetShadeValue(fileData);

            InitialiseArrays(loadedVertCount, loadedFaceCount);
            polygonGroupMapping = new int[loadedFaceCount][];

            GameObjectDataLoader.ReadVerticesFromShadeBuffer(this, fileData, loadedVertCount);
            GameObjectDataLoader.ReadFacesFromShadeBuffer(this, fileData, loadedFaceCount);

            ObjectState = ObjectStateRequiresTransform;
        }

        public GameObject(int vertexCount, int polygonCount)
        {
            GameObjectDataLoader.Initialise(this);
            InitialiseArrays(vertexCount, polygonCount);
            polygonGroupMapping = new int[polygonCount][];

            for (int polygonIndex = 0; polygonIndex < polygonCount; polygonIndex += 1)
            {
                polygonGroupMapping[polygonIndex] = [polygonIndex];
            }
        }

        public GameObject(GameObject[] childObjects, int childCount)
        {
            GameObjectDataLoader.Initialise(this);
            GameObjectComposer.BuildComposite(this, childObjects, childCount, true);
        }

        public GameObject(sbyte[] data, int offset)
        {
            GameObjectDataLoader.Initialise(this);
            int readVertCount = BinaryDataReader.GetShort(data, offset);
            offset += 2;
            int readFaceCount = BinaryDataReader.GetShort(data, offset);
            offset += 2;

            InitialiseArrays(readVertCount, readFaceCount);
            polygonGroupMapping = new int[VertexCoordinatesX.Length][];

            GameObjectDataLoader.ReadBinaryData(this, data, offset, readVertCount, readFaceCount);

            FaceCount = readFaceCount;
            ObjectState = ObjectStateRequiresTransform;
        }

        public GameObject(
            GameObject[] childObjects,
            int childCount,
            bool sharesWorldVertices,
            bool isCollisionless,
            bool doesNotReceiveShadow,
            bool sharesEntityArrays)
        {
            GameObjectDataLoader.Initialise(this);
            DoesShareWorldVertices = sharesWorldVertices;
            HasNoCollider = isCollisionless;
            DoesNotReceiveShadows = doesNotReceiveShadow;
            DoesShareEntityArrays = sharesEntityArrays;
            GameObjectComposer.BuildComposite(this, childObjects, childCount, false);
        }

        public GameObject(
            int vertexCount,
            int polygonCount,
            bool sharesWorldVertices,
            bool isCollisionless,
            bool doesNotReceiveShadow,
            bool sharesEntityArrays,
            bool sharesVertexArrays)
        {
            GameObjectDataLoader.Initialise(this);
            DoesShareWorldVertices = sharesWorldVertices;
            HasNoCollider = isCollisionless;
            DoesNotReceiveShadows = doesNotReceiveShadow;
            DoesShareEntityArrays = sharesEntityArrays;
            DoesShareVertexArrays = sharesVertexArrays;
            InitialiseArrays(vertexCount, polygonCount);
        }

        public void InitialiseArrays(int vertCapacity, int faceCapacity)
        {
            VertexCoordinatesX = new int[vertCapacity];
            VertexCoordinatesY = new int[vertCapacity];
            VertexCoordinatesZ = new int[vertCapacity];
            VertexVectors = new Vector3[vertCapacity];
            FaceNormalComponent = new int[vertCapacity];
            VertexColour = new int[vertCapacity];
            FaceVertexCounts = new int[faceCapacity];
            FaceVertexIndices = new int[faceCapacity][];
            TextureBack = new int[faceCapacity];
            TextureFront = new int[faceCapacity];
            GouraudShade = new int[faceCapacity];
            FaceRenderFlag = new int[faceCapacity];
            FaceVisibility = new int[faceCapacity];

            GameObjectDataLoader.AllocateOptionalArrays(this, vertCapacity, faceCapacity);

            FaceCount = 0;
            VertexCount = 0;
            TotalVertexCapacity = vertCapacity;
            totalFaceCapacity = faceCapacity;
            positionX = positionY = positionZ = 0;
            rotationX = rotationY = rotationZ = 0;
            scaleX = scaleY = scaleZ = DefaultScale;
            secondaryScaleX = secondaryScaleY = secondaryScaleZ = DefaultScale;
            tertiaryScaleX = tertiaryScaleY = tertiaryScaleZ = DefaultScale;
            transformType = NoTransformType;
        }

        public void ResetVertexNormals()
        {
            ProjectedX = new int[VertexCount];
            ProjectedY = new int[VertexCount];
            ProjectedDepth = new int[VertexCount];
            ProjectedU = new int[VertexCount];
            ProjectedV = new int[VertexCount];
        }

        public void ResetObjectIndexes()
        {
            FaceCount = 0;
            VertexCount = 0;
        }

        public void AddPolygonToGroup(int faceDecrement, int vertexDecrement)
        {
            FaceCount -= faceDecrement;

            if (FaceCount < 0)
            {
                FaceCount = 0;
            }

            VertexCount -= vertexDecrement;

            if (VertexCount < 0)
            {
                VertexCount = 0;
            }
        }

        public int GetVertexIndex(int x, int y, int z)
        {
            for (int vertexIndex = 0; vertexIndex < VertexCount; vertexIndex += 1)
            {
                if (VertexCoordinatesX[vertexIndex] == x &&
                    VertexCoordinatesY[vertexIndex] == y &&
                    VertexCoordinatesZ[vertexIndex] == z)
                {
                    return vertexIndex;
                }
            }

            if (VertexCount >= TotalVertexCapacity)
            {
                return -1;
            }

            VertexCoordinatesX[VertexCount] = x;
            VertexCoordinatesY[VertexCount] = y;
            VertexCoordinatesZ[VertexCount] = z;
            VertexCount += 1;

            return VertexCount - 1;
        }

        public int AddVertex(int x, int y, int z)
        {
            if (VertexCount >= TotalVertexCapacity)
            {
                return -1;
            }

            VertexCoordinatesX[VertexCount] = x;
            VertexCoordinatesY[VertexCount] = y;
            VertexCoordinatesZ[VertexCount] = z;
            VertexCount += 1;

            return VertexCount - 1;
        }

        public int AddFaceVertices(int vertexCountForFace, int[] faceVertices, int faceBack, int faceFront)
        {
            if (FaceCount >= totalFaceCapacity)
            {
                return -1;
            }

            FaceVertexCounts[FaceCount] = vertexCountForFace;
            FaceVertexIndices[FaceCount] = faceVertices;
            TextureBack[FaceCount] = faceBack;
            TextureFront[FaceCount] = faceFront;
            ObjectState = ObjectStateRequiresTransform;
            FaceCount += 1;

            return FaceCount - 1;
        }

        public void SetVertexColour(int vertexIndex, int value)
            => VertexColour[vertexIndex] = value;

        public void UpdateShading(
            bool applyShadeValue,
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
            => GameObjectShaderCalculator.ApplyShading(
                this,
                applyShadeValue,
                lightIntensity,
                ambientLight,
                directionX,
                directionY,
                directionZ);

        public void SetModelColours(
            int lightIntensity,
            int ambientLight,
            int directionX,
            int directionY,
            int directionZ)
            => GameObjectShaderCalculator.SetLightingColours(
                this,
                lightIntensity,
                ambientLight,
                directionX,
                directionY,
                directionZ);

        public void OffsetModelColours(int directionX, int directionY, int directionZ)
            => GameObjectShaderCalculator.OffsetLightingColours(
                this,
                directionX,
                directionY,
                directionZ);

        public void OffsetMiniPosition(int deltaX, int deltaY, int deltaZ)
            => GameObjectTransformController.OffsetMiniPosition(this, deltaX, deltaY, deltaZ);

        public void SetRotation(int x, int y, int z)
            => GameObjectTransformController.SetRotation(this, x, y, z);

        public void OffsetPosition(int xOffset, int yOffset, int zOffset)
            => GameObjectTransformController.OffsetPosition(this, xOffset, yOffset, zOffset);

        public void SetPosition(int x, int y, int z)
            => GameObjectTransformController.SetPosition(this, x, y, z);

        public void CopyTranslation(GameObject source)
            => GameObjectTransformController.CopyTranslation(this, source);

        public void CopyModelData(
            GameObject destinationModel,
            int[] vertexIndices,
            int vertexCountForFace,
            int polygonIndex)
            => GameObjectComposer.CopyPolygonData(
                destinationModel,
                this,
                vertexIndices,
                vertexCountForFace,
                polygonIndex);

        public GameObject[] GetObjectsWithinArea(
            int x,
            int y,
            int width,
            int height,
            int chunkSize,
            int chunkCount,
            int maximumVertexCount,
            bool applyLighting)
            => GameObjectComposer.SplitByArea(
                this,
                x,
                y,
                width,
                height,
                chunkSize,
                chunkCount,
                maximumVertexCount,
                applyLighting);

        public void BuildGameObject(GameObject[] childObjects, int childCount, bool applyLighting)
            => GameObjectComposer.BuildComposite(this, childObjects, childCount, applyLighting);

        public void Normalise()
            => GameObjectShaderCalculator.RecalculateNormals(this);

        public void CalculateNormals()
            => GameObjectShaderCalculator.CalculatePolygonNormals(this);

        public void ResetWorldTransform()
        {
            UpdateWorldTransformation();

            for (int vertexIndex = 0; vertexIndex < VertexCount; vertexIndex += 1)
            {
                VertexCoordinatesX[vertexIndex] = WorldVertX[vertexIndex];
                VertexCoordinatesY[vertexIndex] = WorldVertY[vertexIndex];
                VertexCoordinatesZ[vertexIndex] = WorldVertZ[vertexIndex];
            }

            positionX = positionY = positionZ = 0;
            rotationX = rotationY = rotationZ = 0;
            scaleX = scaleY = scaleZ = DefaultScale;
            secondaryScaleX = secondaryScaleY = secondaryScaleZ = DefaultScale;
            tertiaryScaleX = tertiaryScaleY = tertiaryScaleZ = DefaultScale;
            transformType = NoTransformType;
        }

        public void UpdateWorldTransformation()
        {
            if (ObjectState == ObjectStateNullTransform)
            {
                ApplyNullWorldTransform();

                return;
            }

            if (ObjectState == ObjectStateRequiresTransform)
            {
                ApplyFullWorldTransform();
            }
        }

        public void ProjectWithRotation(
            int originX,
            int originY,
            int originZ,
            int rotationXAngle,
            int rotationYAngle,
            int rotationZAngle,
            int projectionScale,
            int nearPlane)
        {
            UpdateWorldTransformation();

            if (IsOutsideFrustum())
            {
                IsVisible = false;

                return;
            }

            IsVisible = true;
            VertexProjectionContext context = BuildProjectionContext(
                originX,
                originY,
                originZ,
                rotationXAngle,
                rotationYAngle,
                rotationZAngle,
                projectionScale,
                nearPlane);
            ProjectAllVertices(context);
        }

        public GameObject CreateParent()
        {
            GameObject[] childArray = [this];
            GameObject parentObject = new(childArray, 1)
            {
                ScaleBias = ScaleBias,
                IsGiantCrystal = IsGiantCrystal
            };

            return parentObject;
        }

        public GameObject CreateParent(
            bool sharesWorldVertices,
            bool isCollisionless,
            bool doesNotReceiveShadow,
            bool sharesEntityArrays)
        {
            GameObject[] childArray = [this];
            GameObject parentObject = new(
                childArray,
                1,
                sharesWorldVertices,
                isCollisionless,
                doesNotReceiveShadow,
                sharesEntityArrays)
            {
                ScaleBias = ScaleBias
            };

            return parentObject;
        }

        public int GetShadeValue(sbyte[] buffer)
        {
            SkipLineBreaks(buffer);

            int highByte = ReadNextShadeTableEntry(buffer);
            int midByte = ReadNextShadeTableEntry(buffer);
            int lowByte = ReadNextShadeTableEntry(buffer);
            int decodedValue =
                highByte * ShadeHighByteMultiplier +
                midByte * ShadeMidByteMultiplier +
                lowByte -
                ShadeValueDecodingOffset;

            if (decodedValue == ShadeSpecialEncodedValue)
            {
                decodedValue = DefaultShadeValue;
            }

            return decodedValue;
        }

        private void ApplyNullWorldTransform()
            => GameObjectTransformer.ApplyNullTransform(this);

        private void ApplyFullWorldTransform()
            => GameObjectTransformer.ApplyFullTransform(
                this,
                transformType,
                rotationX,
                rotationY,
                rotationZ,
                scaleX,
                scaleY,
                scaleZ,
                secondaryScaleX,
                secondaryScaleY,
                secondaryScaleZ,
                tertiaryScaleX,
                tertiaryScaleY,
                tertiaryScaleZ,
                positionX,
                positionY,
                positionZ);

        private bool IsOutsideFrustum() =>
            BoundsMinZ > Camera.FarZ ||
            BoundsMaxZ < Camera.NearZ ||
            BoundsMinX > Camera.FarX ||
            BoundsMaxX < Camera.NearX ||
            BoundsMinY > Camera.FarY ||
            BoundsMaxY < Camera.NearY;

        private void ProjectAllVertices(VertexProjectionContext context)
            => GameObjectTransformer.ProjectVertices(this, context);

        private static VertexProjectionContext BuildProjectionContext(
            int originX,
            int originY,
            int originZ,
            int rotationXAngle,
            int rotationYAngle,
            int rotationZAngle,
            int projectionScale,
            int nearPlane)
            => GameObjectTransformer.BuildProjectionContext(
                originX,
                originY,
                originZ,
                rotationXAngle,
                rotationYAngle,
                rotationZAngle,
                projectionScale,
                nearPlane);

        private void SkipLineBreaks(sbyte[] buffer)
        {
            while (buffer[ShadeBufferIndex] == AsciiLineFeed ||
                buffer[ShadeBufferIndex] == AsciiCarriageReturn)
            {
                ShadeBufferIndex += 1;
            }
        }

        private int ReadNextShadeTableEntry(sbyte[] buffer)
        {
            int value = GameObjectLookupTables.ShadeTable[buffer[ShadeBufferIndex] & ByteMaskValue];
            ShadeBufferIndex += 1;

            return value;
        }
    }
}
