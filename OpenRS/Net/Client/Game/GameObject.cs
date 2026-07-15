using System;
using System.IO;

using Microsoft.Xna.Framework;

using OpenRS.Net.Client.Data;
using OpenRS.Net.Client.Game.Cameras;

namespace OpenRS.Net.Client.Game
{

    public sealed class GameObject //: GameObject
    {

        public GameObject(int vertCount, int polygonCount)
        //: base(_vert_count, polygonCount)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            InitializeObject(vertCount, polygonCount);
            polygonGroupMapping = new int[polygonCount][];
            for (int j = 0; j < polygonCount; j += 1)
            {
                polygonGroupMapping[j] = new int[1];
                polygonGroupMapping[j][0] = j;
            }

        }

        public GameObject(int vertCount, int polyCount, bool flag, bool flag1, bool flag2, bool flag3, bool flag4)
        //: base(x, y, flag, flag1, flag2, flag3, flag4)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            sharesWorldVertices = flag;
            noCollider = flag1;
            dontRecieveShadows = flag2;
            shareEntityArrays = flag3;
            shareVertexArrays = flag4;
            InitializeObject(vertCount, polyCount);
        }

        private void InitializeObject(int _vert_count, int polygonCount)
        {
            vert_x = new int[_vert_count];
            vert_y = new int[_vert_count];
            vert_z = new int[_vert_count];

            _vertices = new Vector3[_vert_count];

            faceNormalComponent = new int[_vert_count];
            vertexColor = new int[_vert_count];

            face_vertices_count = new int[polygonCount];
            face_vertices = new int[polygonCount][];
            texture_back = new int[polygonCount];
            texture_front = new int[polygonCount];
            gouraud_shade = new int[polygonCount];
            faceRenderFlag = new int[polygonCount];
            faceVisibility = new int[polygonCount];
            if (!shareVertexArrays)
            {
                projectedX = new int[_vert_count];
                projectedY = new int[_vert_count];
                projectedDepth = new int[_vert_count];
                projectedU = new int[_vert_count];
                projectedV = new int[_vert_count];
            }
            if (!shareEntityArrays)
            {
                polygonTypeData = new int[polygonCount];
                entityType = new int[polygonCount];
            }
            if (sharesWorldVertices)
            {
                worldVertX = vert_x;
                worldVertY = vert_y;
                worldVertZ = vert_z;
            }
            else
            {
                worldVertX = new int[_vert_count];
                worldVertY = new int[_vert_count];
                worldVertZ = new int[_vert_count];
            }
            if (!dontRecieveShadows || !noCollider)
            {
                normalX = new int[polygonCount];
                normalY = new int[polygonCount];
                normalZ = new int[polygonCount];
            }
            if (!noCollider)
            {
                faceBoundsMinX = new int[polygonCount];
                faceBoundsMaxX = new int[polygonCount];
                faceBoundsMinY = new int[polygonCount];
                faceBoundsMaxY = new int[polygonCount];
                faceBoundsMinZ = new int[polygonCount];
                faceBoundsMaxZ = new int[polygonCount];
            }
            face_count = 0;
            vert_count = 0;
            totalVerticeCount = _vert_count;
            totalFaceCount = polygonCount;
            positionX = positionY = positionZ = 0;
            rotationX = rotationY = rotationZ = 0;
            scaleX = scaleY = scaleZ = 256;
            secondaryScaleX = secondaryScaleY = secondaryScaleZ = tertiaryScaleX = tertiaryScaleY = tertiaryScaleZ = 256;
            transformType = 0;
        }

        public void ResetVertexNormals()
        {
            projectedX = new int[vert_count];
            projectedY = new int[vert_count];
            projectedDepth = new int[vert_count];
            projectedU = new int[vert_count];
            projectedV = new int[vert_count];
        }

        public void ResetObjectIndexes()
        {
            face_count = 0;
            vert_count = 0;
        }

        public void AddPolygonToGroup(int j, int k)
        {
            face_count -= j;
            if (face_count < 0)
            {
                face_count = 0;
            }

            vert_count -= k;
            if (vert_count < 0)
            {
                vert_count = 0;
            }
        }

        public GameObject(sbyte[] data, int offset, bool loadFromData)
        //: base(_vert_count, polygonCount, z)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            int _vert_count = BinaryDataReader.GetShort(data, offset);
            offset += 2;
            int _face_count = BinaryDataReader.GetShort(data, offset);
            offset += 2;

            InitializeObject(_vert_count, _face_count);
            polygonGroupMapping = new int[vert_x.Length][];

            for (int l = 0; l < _vert_count; l += 1)
            {
                polygonGroupMapping[l] = new int[1];
                vert_x[l] = BinaryDataReader.GetSignedShort(data, offset);
                _vertices[l] = new Vector3(vert_x[l], _vertices[l].Y, _vertices[l].Z);
                offset += 2;
            }

            for (int i1 = 0; i1 < _vert_count; i1 += 1)
            {
                vert_y[i1] = BinaryDataReader.GetSignedShort(data, offset);
                _vertices[i1] = new Vector3(_vertices[i1].X, vert_y[i1], _vertices[i1].Z);
                offset += 2;
            }

            for (int j1 = 0; j1 < _vert_count; j1 += 1)
            {
                vert_z[j1] = BinaryDataReader.GetSignedShort(data, offset);
                _vertices[j1] = new Vector3(_vertices[j1].X, _vertices[j1].Y, vert_z[j1]);
                offset += 2;
            }

            vert_count = _vert_count;
            for (int k1 = 0; k1 < _face_count; k1 += 1)
            {
                face_vertices_count[k1] = data[offset++] & 0xff;
            }

            for (int l1 = 0; l1 < _face_count; l1 += 1)
            {
                texture_back[l1] = BinaryDataReader.GetSignedShort(data, offset);
                offset += 2;
                if (texture_back[l1] == 32767)
                {
                    texture_back[l1] = shadeValue;
                }
            }

            for (int i2 = 0; i2 < _face_count; i2 += 1)
            {
                texture_front[i2] = BinaryDataReader.GetSignedShort(data, offset);
                offset += 2;
                if (texture_front[i2] == 32767)
                {
                    texture_front[i2] = shadeValue;
                }
            }

            for (int j2 = 0; j2 < _face_count; j2 += 1)
            {
                int k2 = data[offset++] & 0xff;
                if (k2 == 0)
                {
                    gouraud_shade[j2] = 0;
                }
                else
                {
                    gouraud_shade[j2] = shadeValue;
                }
            }

            for (int l2 = 0; l2 < _face_count; l2 += 1)
            {
                face_vertices[l2] = new int[face_vertices_count[l2]];
                for (int i3 = 0; i3 < face_vertices_count[l2]; i3 += 1)
                {
                    if (_vert_count < 256)
                    {
                        face_vertices[l2][i3] = data[offset++] & 0xff;
                    }
                    else
                    {
                        face_vertices[l2][i3] = BinaryDataReader.GetShort(data, offset);
                        offset += 2;
                    }
                }
            }

            face_count = _face_count;
            objectState = 1;
        }

        public GameObject(string fileName)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            byte[] abyte0;
            try
            {
                MemoryStream inputStream = GameResourceLoader.OpenInputStream(fileName);
                //DataInputStream datainputstream = new DataInputStream(inputstream);
                abyte0 = new byte[3];
                shadeBufferIndex = 0;
                for (int j = 0; j < 3; j += inputStream.Read(abyte0, j, 3 - j))
                {
                    ;
                }

                int l = GetShadeValue((sbyte[])(Array)abyte0);
                abyte0 = new byte[l];
                shadeBufferIndex = 0;
                for (int k = 0; k < l; k += inputStream.Read(abyte0, k, l - k))
                {
                    ;
                }

                inputStream.Close();
            }
            catch
            {
                vert_count = 0;
                face_count = 0;
                return;
            }
            int i1 = GetShadeValue((sbyte[])(Array)abyte0);
            int j1 = GetShadeValue((sbyte[])(Array)abyte0);
            InitializeObject(i1, j1);
            polygonGroupMapping = new int[j1][];
            for (int k3 = 0; k3 < i1; k3 += 1)
            {
                int k1 = GetShadeValue((sbyte[])(Array)abyte0);
                int l1 = GetShadeValue((sbyte[])(Array)abyte0);
                int i2 = GetShadeValue((sbyte[])(Array)abyte0);
                GetVertexIndex(k1, l1, i2);
            }

            for (int l3 = 0; l3 < j1; l3 += 1)
            {
                int j2 = GetShadeValue((sbyte[])(Array)abyte0);
                int k2 = GetShadeValue((sbyte[])(Array)abyte0);
                int l2 = GetShadeValue((sbyte[])(Array)abyte0);
                int i3 = GetShadeValue((sbyte[])(Array)abyte0);
                ambientLightLevel = GetShadeValue((sbyte[])(Array)abyte0);
                baseShadeLevel = GetShadeValue((sbyte[])(Array)abyte0);
                int j3 = GetShadeValue((sbyte[])(Array)abyte0);
                int[] ai = new int[j2];
                for (int i4 = 0; i4 < j2; i4 += 1)
                {
                    ai[i4] = GetShadeValue((sbyte[])(Array)abyte0);
                }

                int[] ai1 = new int[i3];
                for (int j4 = 0; j4 < i3; j4 += 1)
                {
                    ai1[j4] = GetShadeValue((sbyte[])(Array)abyte0);
                }

                int k4 = AddFaceVertices(j2, ai, k2, l2);
                polygonGroupMapping[l3] = ai1;
                if (j3 == 0)
                {
                    gouraud_shade[k4] = 0;
                }
                else
                {
                    gouraud_shade[k4] = shadeValue;
                }
            }

            objectState = 1;
        }

        public GameObject(GameObject[] childObjects, int objectCount, bool flag, bool flag1, bool flag2, bool flag3)
        //: base(childObjects, x, flag, flag1, flag2, flag3)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            sharesWorldVertices = flag;
            noCollider = flag1;
            dontRecieveShadows = flag2;
            shareEntityArrays = flag3;
            BuildGameObject(childObjects, objectCount, false);
        }

        public GameObject(GameObject[] childObjects, int objectCount)
        //: base(childObjects, x)
        {
            objectState = 1;
            visible = true;
            isTranslucent = true;
            isPerspectiveTextured = false;
            isGiantCrystal = false;
            index = -1;
            sharesWorldVertices = false;
            noCollider = false;
            dontRecieveShadows = false;
            shareEntityArrays = false;
            shareVertexArrays = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            lightDirectionX = 180;
            lightDirectionY = 155;
            lightDirectionZ = 95;
            lightMagnitude = 256;
            ambientLightLevel = 512;
            baseShadeLevel = 32;
            BuildGameObject(childObjects, objectCount, true);
        }

        public void BuildGameObject(GameObject[] childObjects, int objectCount, bool applyLighting)
        {
            int j = 0;
            int k = 0;
            for (int l = 0; l < objectCount; l += 1)
            {
                j += childObjects[l].face_count;
                k += childObjects[l].vert_count;
            }

            InitializeObject(k, j);
            if (applyLighting)
            {
                polygonGroupMapping = new int[j][];
            }

            for (int i1 = 0; i1 < objectCount; i1 += 1)
            {
                GameObject j1 = childObjects[i1];
                j1.ResetWorldTransform();
                baseShadeLevel = j1.baseShadeLevel;
                ambientLightLevel = j1.ambientLightLevel;
                lightDirectionX = j1.lightDirectionX;
                lightDirectionY = j1.lightDirectionY;
                lightDirectionZ = j1.lightDirectionZ;
                lightMagnitude = j1.lightMagnitude;
                for (int k1 = 0; k1 < j1.face_count; k1 += 1)
                {
                    int[] ai = new int[j1.face_vertices_count[k1]];
                    int[] ai1 = j1.face_vertices[k1];
                    for (int l1 = 0; l1 < j1.face_vertices_count[k1]; l1 += 1)
                    {
                        ai[l1] = GetVertexIndex(j1.vert_x[ai1[l1]], j1.vert_y[ai1[l1]], j1.vert_z[ai1[l1]]);
                    }

                    int i2 = AddFaceVertices(j1.face_vertices_count[k1], ai, j1.texture_back[k1], j1.texture_front[k1]);
                    gouraud_shade[i2] = j1.gouraud_shade[k1];
                    faceRenderFlag[i2] = j1.faceRenderFlag[k1];
                    faceVisibility[i2] = j1.faceVisibility[k1];
                    if (applyLighting)
                    {
                        if (objectCount > 1)
                        {
                            polygonGroupMapping[i2] = new int[j1.polygonGroupMapping[k1].Length + 1];
                            polygonGroupMapping[i2][0] = i1;
                            for (int j2 = 0; j2 < j1.polygonGroupMapping[k1].Length; j2 += 1)
                            {
                                polygonGroupMapping[i2][j2 + 1] = j1.polygonGroupMapping[k1][j2];
                            }
                        }
                        else
                        {
                            polygonGroupMapping[i2] = new int[j1.polygonGroupMapping[k1].Length];
                            for (int k2 = 0; k2 < j1.polygonGroupMapping[k1].Length; k2 += 1)
                            {
                                polygonGroupMapping[i2][k2] = j1.polygonGroupMapping[k1][k2];
                            }
                        }
                    }
                }

            }

            objectState = 1;
        }

        public int GetVertexIndex(int x, int y, int z)
        {
            for (int j = 0; j < vert_count; j += 1)
            {
                if (vert_x[j] == x && vert_y[j] == y && vert_z[j] == z)
                {
                    return j;
                }
            }

            if (vert_count >= totalVerticeCount)
            {
                return -1;
            }
            else
            {
                vert_x[vert_count] = x;
                vert_y[vert_count] = y;
                vert_z[vert_count] = z;
                vert_count += 1;
                return vert_count - 1;
            }
        }

        public int AddVertex(int x, int y, int z)
        {
            if (vert_count >= totalVerticeCount)
            {
                return -1;
            }
            else
            {
                vert_x[vert_count] = x;
                vert_y[vert_count] = y;
                vert_z[vert_count] = z;
                vert_count += 1;
                return vert_count - 1;
            }
        }

        public int AddFaceVertices(int vertexCount, int[] _faceVertices, int _faceBack, int _faceFront)
        {
            if (face_count >= totalFaceCount)
            {
                return -1;
            }
            else
            {
                face_vertices_count[face_count] = vertexCount;
                face_vertices[face_count] = _faceVertices;
                texture_back[face_count] = _faceBack;
                texture_front[face_count] = _faceFront;
                objectState = 1;
                face_count += 1;
                return face_count - 1;
            }
        }

        public GameObject[] GetObjectsWithinArea(int x, int y, int width, int height, int objectSize, int objectCount, int maxVertCount,
                bool applyLighting)
        {
            ResetWorldTransform();
            int[] ai = new int[objectCount];
            int[] ai1 = new int[objectCount];
            for (int j = 0; j < objectCount; j += 1)
            {
                ai[j] = 0;
                ai1[j] = 0;
            }

            for (int k = 0; k < face_count; k += 1)
            {
                int l = 0;
                int i1 = 0;
                int k1 = face_vertices_count[k];
                int[] ai3 = face_vertices[k];
                for (int k2 = 0; k2 < k1; k2 += 1)
                {
                    l += vert_x[ai3[k2]];
                    i1 += vert_z[ai3[k2]];
                }

                int i3 = l / (k1 * width) + i1 / (k1 * height) * objectSize;
                ai[i3] += k1;
                ai1[i3] += 1;
            }

            GameObject[] ai2 = new GameObject[objectCount];
            for (int j1 = 0; j1 < objectCount; j1 += 1)
            {
                if (ai[j1] > maxVertCount)
                {
                    ai[j1] = maxVertCount;
                }

                ai2[j1] = new GameObject(ai[j1], ai1[j1], true, true, true, applyLighting, true)
                {
                    ambientLightLevel = ambientLightLevel,
                    baseShadeLevel = baseShadeLevel
                };
            }

            for (int l1 = 0; l1 < face_count; l1 += 1)
            {
                int i2 = 0;
                int l2 = 0;
                int j3 = face_vertices_count[l1];
                int[] ai4 = face_vertices[l1];
                for (int k3 = 0; k3 < j3; k3 += 1)
                {
                    i2 += vert_x[ai4[k3]];
                    l2 += vert_z[ai4[k3]];
                }

                int l3 = i2 / (j3 * width) + l2 / (j3 * height) * objectSize;
                CopyModelData(ai2[l3], ai4, j3, l1);
            }

            for (int j2 = 0; j2 < objectCount; j2 += 1)
            {
                ai2[j2].ResetVertexNormals();
            }

            return ai2;
        }

        public void CopyModelData(GameObject sourceModel, int[] indices, int indexCount, int entityTypeIndex)
        {
            int[] ai = new int[indexCount];
            for (int j = 0; j < indexCount; j += 1)
            {
                int k = ai[j] = sourceModel.GetVertexIndex(vert_x[indices[j]], vert_y[indices[j]], vert_z[indices[j]]);
                sourceModel.faceNormalComponent[k] = faceNormalComponent[indices[j]];
                sourceModel.vertexColor[k] = vertexColor[indices[j]];
            }

            int l = sourceModel.AddFaceVertices(indexCount, ai, texture_back[entityTypeIndex], texture_front[entityTypeIndex]);
            if (!sourceModel.shareEntityArrays && !shareEntityArrays)
            {
                sourceModel.entityType[l] = entityType[entityTypeIndex];
            }

            sourceModel.gouraud_shade[l] = gouraud_shade[entityTypeIndex];
            sourceModel.faceRenderFlag[l] = faceRenderFlag[entityTypeIndex];
            sourceModel.faceVisibility[l] = faceVisibility[entityTypeIndex];
        }

        public void UpdateShading(bool setShadeValue, int lightIntensity, int ambientLight, int x, int y, int z)
        {
            baseShadeLevel = 256 - lightIntensity * 4;
            ambientLightLevel = (64 - ambientLight) * 16 + 128;
            if (dontRecieveShadows)
            {
                return;
            }

            for (int j = 0; j < face_count; j += 1)
            {
                if (setShadeValue)
                {
                    gouraud_shade[j] = shadeValue;
                }
                else
                {
                    gouraud_shade[j] = 0;
                }
            }

            lightDirectionX = x;
            lightDirectionY = y;
            lightDirectionZ = z;
            // Calculate magnitude (length) of input vector
            lightMagnitude = (int)Math.Sqrt(x * x + y * y + z * z);
            Normalize();
        }

        public void SetModelColors(int j, int k, int x, int y, int z)
        {
            baseShadeLevel = 256 - j * 4;
            ambientLightLevel = (64 - k) * 16 + 128;
            if (dontRecieveShadows)
            {
                return;
            }
            else
            {
                lightDirectionX = x;
                lightDirectionY = y;
                lightDirectionZ = z;
                lightMagnitude = (int)Math.Sqrt(x * x + y * y + z * z);
                Normalize();
                return;
            }
        }

        public void OffsetModelColors(int x, int y, int z)
        {
            if (dontRecieveShadows)
            {
                return;
            }
            else
            {
                lightDirectionX = x;
                lightDirectionY = y;
                lightDirectionZ = z;
                // normalized value?
                lightMagnitude = (int)Math.Sqrt(x * x + y * y + z * z);
                Normalize();
                return;
            }
        }

        public void SetVertexColor(int vertIndex, int value)
        {
            vertexColor[vertIndex] = value;
        }

        public void OffsetMiniPosition(int x, int y, int z)
        {
            rotationX = rotationX + x & 0xff;
            rotationY = rotationY + y & 0xff;
            rotationZ = rotationZ + z & 0xff;
            CalculatePolygonNormals();
            objectState = 1;
        }

        public void SetRotation(int x, int y, int z)
        {
            rotationX = x & 0xff;
            rotationY = y & 0xff;
            rotationZ = z & 0xff;
            CalculatePolygonNormals();
            objectState = 1;
        }

        public void OffsetPosition(int xOffset, int yOffset, int zOffset)
        {
            positionX += xOffset;
            positionY += yOffset;
            positionZ += zOffset;
            CalculatePolygonNormals();
            objectState = 1;
        }

        public void SetPosition(int x, int y, int z)
        {
            positionX = x;
            positionY = y;
            positionZ = z;
            CalculatePolygonNormals();
            objectState = 1;
        }

        private void CalculatePolygonNormals()
        {
            if (secondaryScaleX != 256 || secondaryScaleY != 256 || secondaryScaleZ != 256 || tertiaryScaleX != 256 || tertiaryScaleY != 256 || tertiaryScaleZ != 256)
            {
                transformType = 4;
                return;
            }
            if (scaleX != 256 || scaleY != 256 || scaleZ != 256)
            {
                transformType = 3;
                return;
            }
            if (rotationX != 0 || rotationY != 0 || rotationZ != 0)
            {
                transformType = 2;
                return;
            }
            if (positionX != 0 || positionY != 0 || positionZ != 0)
            {
                transformType = 1;
                return;
            }
            else
            {
                transformType = 0;
                return;
            }
        }

        private void OffsetWorldVertices(int x, int y, int z)
        {
            for (int j = 0; j < vert_count; j += 1)
            {
                worldVertX[j] += x;
                worldVertY[j] += y;
                worldVertZ[j] += z;
            }

        }

        private void Rotate(int x, int y, int z)
        {
            for (int k2 = 0; k2 < vert_count; k2 += 1)
            {
                if (z != 0)
                {
                    int j = rotationSinCosTable[z];
                    int i1 = rotationSinCosTable[z + 256];
                    int l1 = worldVertY[k2] * j + worldVertX[k2] * i1 >> 15;
                    worldVertY[k2] = worldVertY[k2] * i1 - worldVertX[k2] * j >> 15;
                    worldVertX[k2] = l1;
                }
                if (x != 0)
                {
                    int k = rotationSinCosTable[x];
                    int j1 = rotationSinCosTable[x + 256];
                    int i2 = worldVertY[k2] * j1 - worldVertZ[k2] * k >> 15;
                    worldVertZ[k2] = worldVertY[k2] * k + worldVertZ[k2] * j1 >> 15;
                    worldVertY[k2] = i2;
                }
                if (y != 0)
                {
                    int l = rotationSinCosTable[y];
                    int k1 = rotationSinCosTable[y + 256];
                    int j2 = worldVertZ[k2] * l + worldVertX[k2] * k1 >> 15;
                    worldVertZ[k2] = worldVertZ[k2] * k1 - worldVertX[k2] * l >> 15;
                    worldVertX[k2] = j2;
                }
            }

        }

        private void ScaleVertices(int x, int z, int x1, int y, int z1, int y1)
        {
            for (int j = 0; j < vert_count; j += 1)
            {
                if (x != 0)
                {
                    worldVertX[j] += worldVertY[j] * x >> 8;
                }

                if (z != 0)
                {
                    worldVertZ[j] += worldVertY[j] * z >> 8;
                }

                if (x1 != 0)
                {
                    worldVertX[j] += worldVertZ[j] * x1 >> 8;
                }

                if (y != 0)
                {
                    worldVertY[j] += worldVertZ[j] * y >> 8;
                }

                if (z1 != 0)
                {
                    worldVertZ[j] += worldVertX[j] * z1 >> 8;
                }

                if (y1 != 0)
                {
                    worldVertY[j] += worldVertX[j] * y1 >> 8;
                }
            }

        }

        private void ScaleVertices(int x, int y, int z)
        {
            for (int j = 0; j < vert_count; j += 1)
            {
                worldVertX[j] = worldVertX[j] * x >> 8;
                worldVertY[j] = worldVertY[j] * y >> 8;
                worldVertZ[j] = worldVertZ[j] * z >> 8;
            }

        }

        private void CalculateObjectBounds()
        {
            boundsMinX = boundsMinY = boundsMinZ = 0xf423f;
            distVar = boundsMaxX = boundsMaxY = boundsMaxZ = -boundsMinX/*unchecked((int)0xfff0bdc1)*/;
            for (int j = 0; j < face_count; j += 1)
            {
                int[] ai = face_vertices[j];
                int l = ai[0];
                int j1 = face_vertices_count[j];
                int minX;
                int maxX = minX = worldVertX[l];
                int minY;
                int maxY = minY = worldVertY[l];
                int minZ;
                int maxZ = minZ = worldVertZ[l];
                for (int k = 0; k < j1; k += 1)
                {
                    int i1 = ai[k];
                    if (worldVertX[i1] < minX)
                    {
                        minX = worldVertX[i1];
                    }
                    else
                        if (worldVertX[i1] > maxX)
                    {
                        maxX = worldVertX[i1];
                    }

                    if (worldVertY[i1] < minY)
                    {
                        minY = worldVertY[i1];
                    }
                    else
                        if (worldVertY[i1] > maxY)
                    {
                        maxY = worldVertY[i1];
                    }

                    if (worldVertZ[i1] < minZ)
                    {
                        minZ = worldVertZ[i1];
                    }
                    else
                        if (worldVertZ[i1] > maxZ)
                    {
                        maxZ = worldVertZ[i1];
                    }
                }

                if (!noCollider)
                {
                    faceBoundsMinX[j] = minX;
                    faceBoundsMaxX[j] = maxX;
                    faceBoundsMinY[j] = minY;
                    faceBoundsMaxY[j] = maxY;
                    faceBoundsMinZ[j] = minZ;
                    faceBoundsMaxZ[j] = maxZ;
                }

                if (maxX - minX > distVar)
                {
                    distVar = maxX - minX;
                }

                if (maxY - minY > distVar)
                {
                    distVar = maxY - minY;
                }

                if (maxZ - minZ > distVar)
                {
                    distVar = maxZ - minZ;
                }

                if (minX < boundsMinX)
                {
                    boundsMinX = minX;
                }

                if (maxX > boundsMaxX)
                {
                    boundsMaxX = maxX;
                }

                if (minY < boundsMinY)
                {
                    boundsMinY = minY;
                }

                if (maxY > boundsMaxY)
                {
                    boundsMaxY = maxY;
                }

                if (minZ < boundsMinZ)
                {
                    boundsMinZ = minZ;
                }

                if (maxZ > boundsMaxZ)
                {
                    boundsMaxZ = maxZ;
                }
            }

        }

        public void Normalize()
        {
            if (dontRecieveShadows)
            {
                return;
            }

            int j = ambientLightLevel * lightMagnitude >> 8;
            for (int k = 0; k < face_count; k += 1)
            {
                if (gouraud_shade[k] != shadeValue)
                {
                    gouraud_shade[k] = (normalX[k] * lightDirectionX + normalY[k] * lightDirectionY + normalZ[k] * lightDirectionZ) / j;
                }
            }

            int[] ai = new int[vert_count];
            int[] ai1 = new int[vert_count];
            int[] ai2 = new int[vert_count];
            int[] ai3 = new int[vert_count];
            for (int l = 0; l < vert_count; l += 1)
            {
                ai[l] = 0;
                ai1[l] = 0;
                ai2[l] = 0;
                ai3[l] = 0;
            }

            for (int i1 = 0; i1 < face_count; i1 += 1)
            {
                if (gouraud_shade[i1] == shadeValue)
                {
                    for (int j1 = 0; j1 < face_vertices_count[i1]; j1 += 1)
                    {
                        int l1 = face_vertices[i1][j1];
                        ai[l1] += normalX[i1];
                        ai1[l1] += normalY[i1];
                        ai2[l1] += normalZ[i1];
                        ai3[l1] += 1;
                    }

                }
            }

            for (int k1 = 0; k1 < vert_count; k1 += 1)
            {
                if (ai3[k1] > 0)
                {
                    faceNormalComponent[k1] = (ai[k1] * lightDirectionX + ai1[k1] * lightDirectionY + ai2[k1] * lightDirectionZ) / (j * ai3[k1]);
                }
            }
        }

        public void CalculateNormals()
        {
            if (dontRecieveShadows && noCollider)
            {
                return;
            }

            for (int j = 0; j < face_count; j += 1)
            {
                int[] ai = face_vertices[j];
                int k = worldVertX[ai[0]];
                int l = worldVertY[ai[0]];
                int i1 = worldVertZ[ai[0]];
                int j1 = worldVertX[ai[1]] - k;
                int k1 = worldVertY[ai[1]] - l;
                int l1 = worldVertZ[ai[1]] - i1;
                int i2 = worldVertX[ai[2]] - k;
                int j2 = worldVertY[ai[2]] - l;
                int k2 = worldVertZ[ai[2]] - i1;

                int xDistance = k1 * k2 - j2 * l1;
                int yDistance = l1 * i2 - k2 * j1;
                int j3;
                for (j3 = j1 * j2 - i2 * k1; xDistance > 8192 || yDistance > 8192 || j3 > 8192 || xDistance < -8192 || yDistance < -8192 || j3 < -8192; j3 >>= 1)
                {
                    xDistance >>= 1;
                    yDistance >>= 1;
                }

                // Normalize
                int k3 = (int)(256D * Math.Sqrt(xDistance * xDistance + yDistance * yDistance + j3 * j3));
                if (k3 <= 0)
                {
                    k3 = 1;
                }

                normalX[j] = xDistance * 0x10000 / k3;
                normalY[j] = yDistance * 0x10000 / k3;
                normalZ[j] = j3 * 65535 / k3;
                faceRenderFlag[j] = -1;
            }

            Normalize();
        }

        public void UpdateWorldTransformation()
        {
            if (objectState == 2)
            {
                objectState = 0;
                for (int j = 0; j < vert_count; j += 1)
                {
                    worldVertX[j] = vert_x[j];
                    worldVertY[j] = vert_y[j];
                    worldVertZ[j] = vert_z[j];
                }

                distVar = boundsMaxX = boundsMaxY = boundsMaxZ = 0x98967f;
                boundsMinX = boundsMinY = boundsMinZ = -boundsMaxZ/*unchecked((int)0xff676981)*/;
                return;
            }
            if (objectState == 1)
            {
                objectState = 0;
                for (int k = 0; k < vert_count; k += 1)
                {
                    worldVertX[k] = vert_x[k];
                    worldVertY[k] = vert_y[k];
                    worldVertZ[k] = vert_z[k];
                }

                if (transformType >= 2)
                {
                    Rotate(rotationX, rotationY, rotationZ);
                }

                if (transformType >= 3)
                {
                    ScaleVertices(scaleX, scaleY, scaleZ);
                }

                if (transformType >= 4)
                {
                    ScaleVertices(secondaryScaleX, secondaryScaleY, secondaryScaleZ, tertiaryScaleX, tertiaryScaleY, tertiaryScaleZ);
                }

                if (transformType >= 1)
                {
                    OffsetWorldVertices(positionX, positionY, positionZ);
                }

                CalculateObjectBounds();
                CalculateNormals();
            }
        }

        public void ProjectWithRotation(int originX, int originY, int originZ, int rotationXAngle, int rotationYAngle, int rotationZAngle, int projectionScale,
                int nearPlane)
        {
            UpdateWorldTransformation();
            if (boundsMinZ > Camera.FarZ || boundsMaxZ < Camera.NearZ || boundsMinX > Camera.FarX || boundsMaxX < Camera.NearX || boundsMinY > Camera.FarY || boundsMaxY < Camera.NearY)
            {
                visible = false;
                return;
            }
            visible = true;
            int i1 = 0;
            int j1 = 0;
            int k1 = 0;
            int l1 = 0;
            int i2 = 0;
            int j2 = 0;
            if (rotationZAngle != 0)
            {
                i1 = fineRotationTable[rotationZAngle];
                j1 = fineRotationTable[rotationZAngle + 1024];
            }
            if (rotationYAngle != 0)
            {
                i2 = fineRotationTable[rotationYAngle];
                j2 = fineRotationTable[rotationYAngle + 1024];
            }
            if (rotationXAngle != 0)
            {
                k1 = fineRotationTable[rotationXAngle];
                l1 = fineRotationTable[rotationXAngle + 1024];
            }
            for (int k2 = 0; k2 < vert_count; k2 += 1)
            {
                int l2 = worldVertX[k2] - originX;
                int i3 = worldVertY[k2] - originY;
                int j3 = worldVertZ[k2] - originZ;
                if (rotationZAngle != 0)
                {
                    int j = i3 * i1 + l2 * j1 >> 15;
                    i3 = i3 * j1 - l2 * i1 >> 15;
                    l2 = j;
                }
                if (rotationYAngle != 0)
                {
                    int k = j3 * i2 + l2 * j2 >> 15;
                    j3 = j3 * j2 - l2 * i2 >> 15;
                    l2 = k;
                }
                if (rotationXAngle != 0)
                {
                    int l = i3 * l1 - j3 * k1 >> 15;
                    j3 = i3 * k1 + j3 * l1 >> 15;
                    i3 = l;
                }
                if (j3 >= nearPlane)
                {
                    projectedU[k2] = (l2 << projectionScale) / j3;
                }
                else
                {
                    projectedU[k2] = l2 << projectionScale;
                }

                if (j3 >= nearPlane)
                {
                    projectedV[k2] = (i3 << projectionScale) / j3;
                }
                else
                {
                    projectedV[k2] = i3 << projectionScale;
                }

                projectedX[k2] = l2;
                projectedY[k2] = i3;
                projectedDepth[k2] = j3;
            }

        }

        public void ResetWorldTransform()
        {
            UpdateWorldTransformation();
            for (int j = 0; j < vert_count; j += 1)
            {
                vert_x[j] = worldVertX[j];
                vert_y[j] = worldVertY[j];
                vert_z[j] = worldVertZ[j];
            }

            positionX = positionY = positionZ = 0;
            rotationX = rotationY = rotationZ = 0;
            scaleX = scaleY = scaleZ = 256;
            secondaryScaleX = secondaryScaleY = secondaryScaleZ = tertiaryScaleX = tertiaryScaleY = tertiaryScaleZ = 256;
            transformType = 0;
        }

        public GameObject CreateParent()
        {
            GameObject[] ai = [this];
            GameObject j = new(ai, 1)
            {
                scaleBias = scaleBias,
                isGiantCrystal = isGiantCrystal
            };
            return j;
        }

        public GameObject CreateParent(bool flag, bool flag1, bool flag2, bool flag3)
        {
            GameObject[] ai = [this];
            GameObject j = new(ai, 1, flag, flag1, flag2, flag3)
            {
                scaleBias = scaleBias
            };
            return j;
        }

        public void CopyTranslation(GameObject j)
        {
            rotationX = j.rotationX;
            rotationY = j.rotationY;
            rotationZ = j.rotationZ;
            positionX = j.positionX;
            positionY = j.positionY;
            positionZ = j.positionZ;
            CalculatePolygonNormals();
            objectState = 1;
        }

        //public int GetShadeValue(sbyte[] _vert_count)
        //{
        //    for (; _vert_count[shadeBufferIndex] == 10 || _vert_count[shadeBufferIndex] == 13; shadeBufferIndex += 1) ;
        //    int x = shadeTable[_vert_count[shadeBufferIndex++]];
        //    int y = shadeTable[_vert_count[shadeBufferIndex++]];
        //    int z = shadeTable[_vert_count[shadeBufferIndex++]];
        //    int y = (x * 4096 + y * 64 + z) - 0x20000;
        //    if (y == 0x1e240)
        //        y = shadeValue;
        //    return y;
        //}

        public int GetShadeValue(sbyte[] buffer)
        {
            for (; buffer[shadeBufferIndex] == 10 || buffer[shadeBufferIndex] == 13; shadeBufferIndex += 1)
            {
                ;
            }

            int j = shadeTable[buffer[shadeBufferIndex++] & 0xff];
            int k = shadeTable[buffer[shadeBufferIndex++] & 0xff];
            int l = shadeTable[buffer[shadeBufferIndex++] & 0xff];
            int i1 = j * 4096 + k * 64 + l - 0x20000;
            if (i1 == 0x1e240)
            {
                i1 = shadeValue;
            }

            return i1;
        }

        public int vert_count;
        public int[] projectedX;
        public int[] projectedY;
        public int[] projectedDepth;
        public int[] projectedU;
        public int[] projectedV;
        public int[] faceNormalComponent;
        public int[] vertexColor;
        public int face_count;
        public int[] face_vertices_count;
        public int[][] face_vertices;
        public int[] texture_back;
        public int[] texture_front;
        public int[] faceVisibility;
        public int[] faceRenderFlag;
        public int[] gouraud_shade;
        public int[] normalX;
        public int[] normalY;
        public int[] normalZ;
        public int scaleBias;
        public int objectState;
        public bool visible;
        public int boundsMinX;
        public int boundsMaxX;
        public int boundsMinY;
        public int boundsMaxY;
        public int boundsMinZ;
        public int boundsMaxZ;
        public bool isTranslucent;
        public bool isPerspectiveTextured;
        public bool isGiantCrystal;
        public int index;
        public int[] entityType;
        public int[] polygonTypeData;
        private readonly bool sharesWorldVertices;
        public bool noCollider;
        public bool dontRecieveShadows;
        public bool shareEntityArrays;
        public bool shareVertexArrays;
        private static readonly int[] rotationSinCosTable;
        private static readonly int[] fineRotationTable;
        private static readonly int[] scaleTable;
        private static readonly int[] shadeTable;
        private readonly int shadeValue;
        public int totalVerticeCount;
        public int[] vert_x;
        public int[] vert_y;
        public int[] vert_z;

        public Vector3[] _vertices;

        public int[] worldVertX;
        public int[] worldVertY;
        public int[] worldVertZ;
        private int totalFaceCount;
        private int[][] polygonGroupMapping;
        private int[] faceBoundsMinX;
        private int[] faceBoundsMaxX;
        private int[] faceBoundsMinY;
        private int[] faceBoundsMaxY;
        private int[] faceBoundsMinZ;
        private int[] faceBoundsMaxZ;
        private int positionX;
        private int positionY;
        private int positionZ;
        private int rotationX;
        private int rotationY;
        private int rotationZ;
        private int scaleX;
        private int scaleY;
        private int scaleZ;
        private int secondaryScaleX;
        private int secondaryScaleY;
        private int secondaryScaleZ;
        private int tertiaryScaleX;
        private int tertiaryScaleY;
        private int tertiaryScaleZ;
        private int transformType;
        private int distVar;
        private int lightDirectionX;
        private int lightDirectionY;
        private int lightDirectionZ;
        private int lightMagnitude;
        public int ambientLightLevel;
        public int baseShadeLevel;
        private int shadeBufferIndex;

        static GameObject()
        {
            rotationSinCosTable = new int[512];
            fineRotationTable = new int[2048];
            scaleTable = new int[64];
            shadeTable = new int[256];
            for (int j = 0; j < 256; j += 1)
            {
                rotationSinCosTable[j] = (int)(Math.Sin(j * 0.02454369D) * 32768D);
                rotationSinCosTable[j + 256] = (int)(Math.Cos(j * 0.02454369D) * 32768D);
            }

            for (int k = 0; k < 1024; k += 1)
            {
                fineRotationTable[k] = (int)(Math.Sin(k * 0.00613592315D) * 32768D);
                fineRotationTable[k + 1024] = (int)(Math.Cos(k * 0.00613592315D) * 32768D);
            }

            for (int l = 0; l < 10; l += 1)
            {
                scaleTable[l] = (byte)(48 + l);
            }

            for (int i1 = 0; i1 < 26; i1 += 1)
            {
                scaleTable[i1 + 10] = (byte)(65 + i1);
            }

            for (int j1 = 0; j1 < 26; j1 += 1)
            {
                scaleTable[j1 + 36] = (byte)(97 + j1);
            }

            scaleTable[62] = -93;
            scaleTable[63] = 36;
            for (int k1 = 0; k1 < 10; k1 += 1)
            {
                shadeTable[48 + k1] = k1;
            }

            for (int l1 = 0; l1 < 26; l1 += 1)
            {
                shadeTable[65 + l1] = l1 + 10;
            }

            for (int i2 = 0; i2 < 26; i2 += 1)
            {
                shadeTable[97 + i2] = i2 + 36;
            }

            shadeTable[163] = 62;
            shadeTable[36] = 63;
        }
    }
}
