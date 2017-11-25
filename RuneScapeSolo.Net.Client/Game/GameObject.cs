using System;

using Microsoft.Xna.Framework;

using RuneScapeSolo.Net.Client.Data;
using RuneScapeSolo.Net.Client.Game.Cameras;
using RuneScapeSolo.Primitives;

namespace RuneScapeSolo.Net.Client.Game
{
    public class ObjectModel //: Model
    {
        public Point3D[] WorldVerticeLocations;
        public Point3D[] verticeLocations;
        Point3D[] minimumFaceBounds;
        Point3D[] maximumFaceBounds;
        Point3D[] normalLocations;
        Point3D location;
        Point3D rotation;
        Point3D minimumBounds;
        Point3D maximumBounds;
        Point3D shadingUnknown;

        static ObjectModel()
        {
            cie = new int[512];
            cif = new int[2048];
            cig = new int[64];
            cih = new int[256];
            for (int j = 0; j < 256; j++)
            {
                cie[j] = (int)(Math.Sin(j * 0.02454369D) * 32768D);
                cie[j + 256] = (int)(Math.Cos(j * 0.02454369D) * 32768D);
            }

            for (int k = 0; k < 1024; k++)
            {
                cif[k] = (int)(Math.Sin(k * 0.00613592315D) * 32768D);
                cif[k + 1024] = (int)(Math.Cos(k * 0.00613592315D) * 32768D);
            }

            for (int l = 0; l < 10; l++)
            {
                cig[l] = (byte)(48 + l);
            }

            for (int i1 = 0; i1 < 26; i1++)
            {
                cig[i1 + 10] = (byte)(65 + i1);
            }

            for (int j1 = 0; j1 < 26; j1++)
            {
                cig[j1 + 36] = (byte)(97 + j1);
            }

            cig[62] = -93;
            cig[63] = 36;
            for (int k1 = 0; k1 < 10; k1++)
            {
                cih[48 + k1] = k1;
            }

            for (int l1 = 0; l1 < 26; l1++)
            {
                cih[65 + l1] = l1 + 10;
            }

            for (int i2 = 0; i2 < 26; i2++)
            {
                cih[97 + i2] = i2 + 36;
            }

            cih[163] = 62;
            cih[36] = 63;
        }

        public ObjectModel(int vertCount, int polygonCount)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            InitializeObject(vertCount, polygonCount);
            cje = new int[polygonCount][];

            for (int j = 0; j < polygonCount; j++)
            {
                cje[j] = new int[1];
                cje[j][0] = j;
            }
        }

        public ObjectModel(int vertCount, int polyCount, bool flag, bool flag1, bool flag2, bool flag3, bool flag4)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            chn = flag;
            noCollider = flag1;
            dontRecieveShadows = flag2;
            cic = flag3;
            cid = flag4;
            InitializeObject(vertCount, polyCount);
        }

        // TODO: REMOVE ASAP
        public int getVertexIndex(int x, int y, int z)
        {
            Point3D point = new Point3D(x, y, z);

            return getVertexIndex(point);
        }

        // TODO: REMOVE ASAP
        public void offsetLocation(int x, int y, int z)
        {
            Point3D point = new Point3D(x, y, z);

            offsetLocation(point);
        }

        // TODO: REMOVE ASAP
        public void setRotation(int x, int y, int z)
        {
            Point3D point = new Point3D(x, y, z);

            setRotation(point);
        }

        void InitializeObject(int _vert_count, int polygonCount)
        {
            verticeLocations = new Point3D[_vert_count];

            _vertices = new Vector3[_vert_count];

#warning possibly texture coordinates.
            cfn = new int[_vert_count];
            vertexColor = new int[_vert_count];


            face_vertices_count = new int[polygonCount];
            face_vertices = new int[polygonCount][];
            texture_back = new int[polygonCount];
            texture_front = new int[polygonCount];
            gouraud_shade = new int[polygonCount];
            cgh = new int[polygonCount];
            cgg = new int[polygonCount];
            if (!cid)
            {
                vertX = new int[_vert_count];
                vertY = new int[_vert_count];
                vertZ = new int[_vert_count];
                cfl = new int[_vert_count];
                cfm = new int[_vert_count];
            }

            if (!cic)
            {
                chm = new int[polygonCount];
                entityType = new int[polygonCount];
            }

            if (chn)
            {
                WorldVerticeLocations = verticeLocations;
            }
            else
            {
                WorldVerticeLocations = new Point3D[_vert_count];
            }

            if (!dontRecieveShadows || !noCollider)
            {
                normalLocations = new Point3D[polygonCount];
            }

            if (!noCollider)
            {
                minimumFaceBounds = new Point3D[polygonCount];
                maximumFaceBounds = new Point3D[polygonCount];
            }

            face_count = 0;
            vert_count = 0;
            totalVerticeCount = _vert_count;
            totalFaceCount = polygonCount;
            location = Point3D.Empty;
            rotation = Point3D.Empty;
            ckd = cke = ckf = 256;
            ckg = ckh = cki = ckj = ckk = ckl = 256;
            ckm = 0;
        }

        public void clj()
        {
            vertX = new int[vert_count];
            vertY = new int[vert_count];
            vertZ = new int[vert_count];
            cfl = new int[vert_count];
            cfm = new int[vert_count];
        }

        public void resetObjectIndexes()
        {
            face_count = 0;
            vert_count = 0;
        }

        public void cll(int j, int k)
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

        public ObjectModel(sbyte[] data, int offset)
        //: base(_vert_count, polygonCount, z)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            int _vert_count = DataOperations.GetInt16(data, offset);
            offset += 2;
            int _face_count = DataOperations.GetInt16(data, offset);
            offset += 2;

            InitializeObject(_vert_count, _face_count);
            cje = new int[verticeLocations.Length][];

            for (int verticeIndex = 0; verticeIndex < _vert_count; verticeIndex++)
            {
                cje[verticeIndex] = new int[1];
                verticeLocations[verticeIndex].X = DataOperations.getShort2(data, offset);
                _vertices[verticeIndex] = new Vector3(verticeLocations[verticeIndex].X, _vertices[verticeIndex].Y, _vertices[verticeIndex].Z);
                offset += 2;
            }

            for (int verticeIndex = 0; verticeIndex < _vert_count; verticeIndex++)
            {
                verticeLocations[verticeIndex].Y = DataOperations.getShort2(data, offset);
                _vertices[verticeIndex] = new Vector3(_vertices[verticeIndex].X, verticeLocations[verticeIndex].Y, _vertices[verticeIndex].Z);
                offset += 2;
            }

            for (int verticeIndex = 0; verticeIndex < _vert_count; verticeIndex++)
            {
                verticeLocations[verticeIndex].Z = DataOperations.getShort2(data, offset);
                _vertices[verticeIndex] = new Vector3(_vertices[verticeIndex].X, _vertices[verticeIndex].Y, verticeLocations[verticeIndex].Z);
                offset += 2;
            }

            vert_count = _vert_count;
            for (int k1 = 0; k1 < _face_count; k1++)
            {
                face_vertices_count[k1] = data[offset++] & 0xff;
            }

            for (int l1 = 0; l1 < _face_count; l1++)
            {
                texture_back[l1] = DataOperations.getShort2(data, offset);
                offset += 2;
                if (texture_back[l1] == 32767)
                {
                    texture_back[l1] = shadeValue;
                }
            }

            for (int i2 = 0; i2 < _face_count; i2++)
            {
                texture_front[i2] = DataOperations.getShort2(data, offset);
                offset += 2;
                if (texture_front[i2] == 32767)
                {
                    texture_front[i2] = shadeValue;
                }
            }

            for (int j2 = 0; j2 < _face_count; j2++)
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

            for (int l2 = 0; l2 < _face_count; l2++)
            {
                face_vertices[l2] = new int[face_vertices_count[l2]];
                for (int i3 = 0; i3 < face_vertices_count[l2]; i3++)
                {
                    if (_vert_count < 256)
                    {
                        face_vertices[l2][i3] = data[offset++] & 0xff;
                    }
                    else
                    {
                        face_vertices[l2][i3] = DataOperations.GetInt16(data, offset);
                        offset += 2;
                    }
                }
            }

            face_count = _face_count;
            objectState = 1;
        }

        public ObjectModel(string fileName)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            byte[] abyte0 = null;
            try
            {
                var inputstream = DataOperations.openInputStream(fileName);
                //DataInputStream datainputstream = new DataInputStream(inputstream);
                abyte0 = new byte[3];
                clg = 0;
                for (int j = 0; j < 3; j += inputstream.Read(abyte0, j, 3 - j))
                {
                    ;
                }

                int l = getShadeValue((sbyte[])(Array)abyte0);
                abyte0 = new byte[l];
                clg = 0;
                for (int k = 0; k < l; k += inputstream.Read(abyte0, k, l - k))
                {
                    ;
                }

                inputstream.Close();
            }
            catch
            {
                Console.WriteLine($"An error has occured in {nameof(ObjectModel)}.cs");

                vert_count = 0;
                face_count = 0;
                return;
            }
            int i1 = getShadeValue((sbyte[])(Array)abyte0);
            int j1 = getShadeValue((sbyte[])(Array)abyte0);
            InitializeObject(i1, j1);
            cje = new int[j1][];

            for (int k3 = 0; k3 < i1; k3++)
            {
                Point3D point = new Point3D(
                    getShadeValue((sbyte[])(Array)abyte0),
                    getShadeValue((sbyte[])(Array)abyte0),
                    getShadeValue((sbyte[])(Array)abyte0));

                getVertexIndex(point);
            }

            for (int l3 = 0; l3 < j1; l3++)
            {
                int j2 = getShadeValue((sbyte[])(Array)abyte0);
                int k2 = getShadeValue((sbyte[])(Array)abyte0);
                int l2 = getShadeValue((sbyte[])(Array)abyte0);
                int i3 = getShadeValue((sbyte[])(Array)abyte0);
                cle = getShadeValue((sbyte[])(Array)abyte0);
                clf = getShadeValue((sbyte[])(Array)abyte0);
                int j3 = getShadeValue((sbyte[])(Array)abyte0);
                int[] ai = new int[j2];
                for (int i4 = 0; i4 < j2; i4++)
                {
                    ai[i4] = getShadeValue((sbyte[])(Array)abyte0);
                }

                int[] ai1 = new int[i3];
                for (int j4 = 0; j4 < i3; j4++)
                {
                    ai1[j4] = getShadeValue((sbyte[])(Array)abyte0);
                }

                int k4 = addFaceVertices(j2, ai, k2, l2);
                cje[l3] = ai1;
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

        public ObjectModel(ObjectModel[] childObjects, int objectCount, bool flag, bool flag1, bool flag2, bool flag3)
        //: base(childObjects, x, flag, flag1, flag2, flag3)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            chn = flag;
            noCollider = flag1;
            dontRecieveShadows = flag2;
            cic = flag3;
            BuildModel(childObjects, objectCount, false);
        }

        public ObjectModel(ObjectModel[] childObjects, int objectCount)
        //: base(childObjects, x)
        {
            objectState = 1;
            visible = true;
            chh = true;
            chi = false;
            isGiantCrystal = false;
            index = -1;
            chn = false;
            noCollider = false;
            dontRecieveShadows = false;
            cic = false;
            cid = false;
            shadeValue = 0xbc614e;
            distVar = 0xbc614e;
            shadingUnknown = new Point3D(180, 155, 95);
            cld = 256;
            cle = 512;
            clf = 32;
            BuildModel(childObjects, objectCount, true);
        }

        public void BuildModel(ObjectModel[] childObjects, int objectCount, bool arg2)
        {
            int j = 0;
            int k = 0;
            for (int l = 0; l < objectCount; l++)
            {
                j += childObjects[l].face_count;
                k += childObjects[l].vert_count;
            }

            InitializeObject(k, j);
            if (arg2)
            {
                cje = new int[j][];
            }

            for (int i1 = 0; i1 < objectCount; i1++)
            {
                ObjectModel j1 = childObjects[i1];
                j1.cni();
                clf = j1.clf;
                cle = j1.cle;
                shadingUnknown = j1.shadingUnknown;
                cld = j1.cld;
                for (int k1 = 0; k1 < j1.face_count; k1++)
                {
                    int[] ai = new int[j1.face_vertices_count[k1]];
                    int[] ai1 = j1.face_vertices[k1];

                    for (int faceVerticeIndex = 0; faceVerticeIndex < j1.face_vertices_count[k1]; faceVerticeIndex++)
                    {
                        ai[faceVerticeIndex] = getVertexIndex(j1.verticeLocations[ai1[faceVerticeIndex]]);
                    }

                    int i2 = addFaceVertices(j1.face_vertices_count[k1], ai, j1.texture_back[k1], j1.texture_front[k1]);
                    gouraud_shade[i2] = j1.gouraud_shade[k1];
                    cgh[i2] = j1.cgh[k1];
                    cgg[i2] = j1.cgg[k1];
                    if (arg2)
                    {
                        if (objectCount > 1)
                        {
                            cje[i2] = new int[j1.cje[k1].Length + 1];
                            cje[i2][0] = i1;
                            for (int j2 = 0; j2 < j1.cje[k1].Length; j2++)
                            {
                                cje[i2][j2 + 1] = j1.cje[k1][j2];
                            }
                        }
                        else
                        {
                            cje[i2] = new int[j1.cje[k1].Length];
                            for (int k2 = 0; k2 < j1.cje[k1].Length; k2++)
                            {
                                cje[i2][k2] = j1.cje[k1][k2];
                            }
                        }
                    }
                }
            }

            objectState = 1;
        }

        public int getVertexIndex(Point3D location)
        {
            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                if (verticeLocations[verticeIndex] == location)
                {
                    return verticeIndex;
                }
            }

            if (vert_count >= totalVerticeCount)
            {
                return -1;
            }

            verticeLocations[vert_count] = location;

            return vert_count++;
        }

        public int addVertex(Point3D location)
        {
            if (vert_count >= totalVerticeCount)
            {
                return -1;
            }

            verticeLocations[vert_count] = location;

            return vert_count++;
        }

        public int addFaceVertices(int vertexCount, int[] _faceVertices, int _faceBack, int _faceFront)
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
                return face_count++;
            }
        }

        public ObjectModel[] getObjectsWithinArea(int x, int y, int width, int height, int objectSize, int objectCount, int maxVertCount,
                bool arg7)
        {
            cni();
            int[] ai = new int[objectCount];
            int[] ai1 = new int[objectCount];
            for (int j = 0; j < objectCount; j++)
            {
                ai[j] = 0;
                ai1[j] = 0;
            }

            for (int k = 0; k < face_count; k++)
            {
                int l = 0;
                int i1 = 0;
                int k1 = face_vertices_count[k];
                int[] ai3 = face_vertices[k];

                for (int k2 = 0; k2 < k1; k2++)
                {
                    l += verticeLocations[ai3[k2]].X;
                    i1 += verticeLocations[ai3[k2]].Z;
                }

                int i3 = l / (k1 * width) + (i1 / (k1 * height)) * objectSize;
                ai[i3] += k1;
                ai1[i3]++;
            }

            ObjectModel[] ai2 = new ObjectModel[objectCount];
            for (int j1 = 0; j1 < objectCount; j1++)
            {
                if (ai[j1] > maxVertCount)
                {
                    ai[j1] = maxVertCount;
                }

                ai2[j1] = new ObjectModel(ai[j1], ai1[j1], true, true, true, arg7, true);
                ai2[j1].cle = cle;
                ai2[j1].clf = clf;
            }

            for (int l1 = 0; l1 < face_count; l1++)
            {
                int i2 = 0;
                int l2 = 0;
                int j3 = face_vertices_count[l1];
                int[] ai4 = face_vertices[l1];

                for (int k3 = 0; k3 < j3; k3++)
                {
                    i2 += verticeLocations[ai4[k3]].X;
                    l2 += verticeLocations[ai4[k3]].Z;
                }

                int l3 = i2 / (j3 * width) + (l2 / (j3 * height)) * objectSize;
                CopyModelData(ai2[l3], ai4, j3, l1);
            }

            for (int j2 = 0; j2 < objectCount; j2++)
            {
                ai2[j2].clj();
            }

            return ai2;
        }

        public void CopyModelData(ObjectModel arg0, int[] indices, int indexCount, int entityTypeIndex)
        {
            int[] ai = new int[indexCount];

            for (int j = 0; j < indexCount; j++)
            {
                int k = ai[j] = arg0.getVertexIndex(verticeLocations[indices[j]]);

                arg0.cfn[k] = cfn[indices[j]];
                arg0.vertexColor[k] = vertexColor[indices[j]];
            }

            int l = arg0.addFaceVertices(indexCount, ai, texture_back[entityTypeIndex], texture_front[entityTypeIndex]);
            if (!arg0.cic && !cic)
            {
                arg0.entityType[l] = entityType[entityTypeIndex];
            }

            arg0.gouraud_shade[l] = gouraud_shade[entityTypeIndex];
            arg0.cgh[l] = cgh[entityTypeIndex];
            arg0.cgg[l] = cgg[entityTypeIndex];
        }

        public void UpdateShading(bool setShadeValue, int arg1, int arg2, Point3D point)
        {
            clf = 256 - arg1 * 4;
            cle = (64 - arg2) * 16 + 128;

            if (dontRecieveShadows)
            {
                return;
            }

            for (int j = 0; j < face_count; j++)
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

            shadingUnknown = point;

            // Calculate magnitude (length) of input vector
            cld = (int)Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);
            normalize();
        }

        public void cmf(int j, int k, Point3D point)
        {
            clf = 256 - j * 4;
            cle = (64 - k) * 16 + 128;

            if (dontRecieveShadows)
            {
                return;
            }

            shadingUnknown = point;

            cld = (int)Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);
            normalize();

            return;
        }

        public void cmg(Point3D point)
        {
            if (dontRecieveShadows)
            {
                return;
            }

            shadingUnknown = point;

            // normalized value?
            cld = (int)Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);
            normalize();
            return;
        }

        public void SetVertexColor(int vertIndex, int value)
        {
            vertexColor[vertIndex] = value;
        }

        public void offsetMiniPosition(Point3D point)
        {
            rotation.X += point.X & 0xFF;
            rotation.Y += point.Y & 0xFF;
            rotation.Z += point.Z & 0xFF;

            cmm();
            objectState = 1;
        }

        public void setRotation(Point3D point)
        {
            rotation.X = point.X & 0xFF;
            rotation.Y = point.Y & 0xFF;
            rotation.Z = point.Z & 0xFF;

            cmm();
            objectState = 1;
        }

        public void offsetLocation(Point3D offset)
        {
            location += offset;

            cmm();
            objectState = 1;
        }

        public void setLocation(Point3D location)
        {
            this.location = location;

            cmm();
            objectState = 1;
        }

        void cmm()
        {
            if (ckg != 256 || ckh != 256 || cki != 256 || ckj != 256 || ckk != 256 || ckl != 256)
            {
                ckm = 4;
                return;
            }
            if (ckd != 256 || cke != 256 || ckf != 256)
            {
                ckm = 3;
                return;
            }
            if (rotation.X != 0 || rotation.Y != 0 || rotation.Z != 0)
            {
                ckm = 2;
                return;
            }
            if (location.X != 0 || location.Y != 0 || location.Z != 0)
            {
                ckm = 1;
                return;
            }

            ckm = 0;
        }

        void OffsetWorldVertices(Point3D offset)
        {
            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                WorldVerticeLocations[verticeIndex] += offset;
            }
        }

        void rotate(Point3D rotationPoint)
        {
            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                if (rotationPoint.Z != 0)
                {
                    int j = cie[rotationPoint.Z];
                    int i1 = cie[rotationPoint.Z + 256];
                    int l1 = WorldVerticeLocations[verticeIndex].Y * j + WorldVerticeLocations[verticeIndex].X * i1 >> 15;

                    WorldVerticeLocations[verticeIndex].Y =
                        WorldVerticeLocations[verticeIndex].Y * i1 -
                        WorldVerticeLocations[verticeIndex].X * j >> 15;
                    WorldVerticeLocations[verticeIndex].X = l1;
                }

                if (rotationPoint.X != 0)
                {
                    int k = cie[rotationPoint.X];
                    int j1 = cie[rotationPoint.X + 256];
                    int i2 =
                        WorldVerticeLocations[verticeIndex].Y * j1 -
                        WorldVerticeLocations[verticeIndex].Z * k >> 15;

                    WorldVerticeLocations[verticeIndex].Z =
                        WorldVerticeLocations[verticeIndex].Y * k +
                        WorldVerticeLocations[verticeIndex].Z * j1 >> 15;
                    WorldVerticeLocations[verticeIndex].Y = i2;
                }

                if (rotationPoint.Y != 0)
                {
                    int l = cie[rotationPoint.Y];
                    int k1 = cie[rotationPoint.Y + 256];
                    int j2 =
                        WorldVerticeLocations[verticeIndex].Z * l +
                        WorldVerticeLocations[verticeIndex].X * k1 >> 15;

                    WorldVerticeLocations[verticeIndex].Z =
                        WorldVerticeLocations[verticeIndex].Z * k1 -
                        WorldVerticeLocations[verticeIndex].X * l >> 15;
                    WorldVerticeLocations[verticeIndex].X = j2;
                }
            }
        }

        void scaleVertices(int x, int z, int x1, int y, int z1, int y1)
        {
            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                if (x != 0)
                {
                    WorldVerticeLocations[verticeIndex].X += WorldVerticeLocations[verticeIndex].Y * x >> 8;
                }

                if (z != 0)
                {
                    WorldVerticeLocations[verticeIndex].Z += WorldVerticeLocations[verticeIndex].Y * z >> 8;
                }

                if (x1 != 0)
                {
                    WorldVerticeLocations[verticeIndex].X += WorldVerticeLocations[verticeIndex].Z * x1 >> 8;
                }

                if (y != 0)
                {
                    WorldVerticeLocations[verticeIndex].Y += WorldVerticeLocations[verticeIndex].Z * y >> 8;
                }

                if (z1 != 0)
                {
                    WorldVerticeLocations[verticeIndex].Z += WorldVerticeLocations[verticeIndex].X * z1 >> 8;
                }

                if (y1 != 0)
                {
                    WorldVerticeLocations[verticeIndex].Y += WorldVerticeLocations[verticeIndex].X * y1 >> 8;
                }
            }
        }

        void ScaleVertices(Point3D scale)
        {
            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                WorldVerticeLocations[verticeIndex].X = WorldVerticeLocations[verticeIndex].X * scale.X >> 8;
                WorldVerticeLocations[verticeIndex].Y = WorldVerticeLocations[verticeIndex].Y * scale.Y >> 8;
                WorldVerticeLocations[verticeIndex].Z = WorldVerticeLocations[verticeIndex].Z * scale.Z >> 8;
            }
        }

        void calculateObjectBounds()
        {
            minimumBounds = new Point3D(0xf423f, 0xf423f, 0xf423f);
            distVar = -minimumBounds.X;
            maximumBounds = new Point3D(distVar, distVar, distVar);

            for (int faceIndex = 0; faceIndex < face_count; faceIndex++)
            {
                int[] ai = face_vertices[faceIndex];
                int l = ai[0];
                int j1 = face_vertices_count[faceIndex];
                int minX;
                int maxX = minX = WorldVerticeLocations[l].X;
                int minY;
                int maxY = minY = WorldVerticeLocations[l].Y;
                int minZ;
                int maxZ = minZ = WorldVerticeLocations[l].Z;

                for (int k = 0; k < j1; k++)
                {
                    int i1 = ai[k];

                    if (WorldVerticeLocations[i1].X < minX)
                    {
                        minX = WorldVerticeLocations[i1].X;
                    }
                    else if (WorldVerticeLocations[i1].X > maxX)
                    {
                        maxX = WorldVerticeLocations[i1].X;
                    }

                    if (WorldVerticeLocations[i1].Y < minY)
                    {
                        minY = WorldVerticeLocations[i1].Y;
                    }
                    else if (WorldVerticeLocations[i1].Y > maxY)
                    {
                        maxY = WorldVerticeLocations[i1].Y;
                    }

                    if (WorldVerticeLocations[i1].Z < minZ)
                    {
                        minZ = WorldVerticeLocations[i1].Z;
                    }
                    else if (WorldVerticeLocations[i1].Z > maxZ)
                    {
                        maxZ = WorldVerticeLocations[i1].Z;
                    }
                }

                if (!noCollider)
                {
                    minimumFaceBounds[faceIndex] = new Point3D(minX, minY, minZ);
                    maximumFaceBounds[faceIndex] = new Point3D(maxX, maxY, maxZ);
                }

                if (maxX - minX > distVar)
                {
                    distVar = (maxX - minX);
                }

                if (maxY - minY > distVar)
                {
                    distVar = (maxY - minY);
                }

                if (maxZ - minZ > distVar)
                {
                    distVar = (maxZ - minZ);
                }

                if (minX < minimumBounds.X)
                {
                    minimumBounds.X = minX;
                }

                if (maxX > maximumBounds.X)
                {
                    maximumBounds.X = maxX;
                }

                if (minY < minimumBounds.Y)
                {
                    minimumBounds.Y = minY;
                }

                if (maxY > maximumBounds.Y)
                {
                    maximumBounds.Y = maxY;
                }

                if (minZ < minimumBounds.Z)
                {
                    minimumBounds.Z = minZ;
                }

                if (maxZ > maximumBounds.Z)
                {
                    maximumBounds.Z = maxZ;
                }
            }

        }

        public void normalize()
        {
            if (dontRecieveShadows)
            {
                return;
            }

            int j = cle * cld >> 8;

            for (int faceIndex = 0; faceIndex < face_count; faceIndex++)
            {
                if (gouraud_shade[faceIndex] != shadeValue)
                {
                    gouraud_shade[faceIndex] =
                        (normalLocations[faceIndex].X * shadingUnknown.X +
                         normalLocations[faceIndex].Y * shadingUnknown.Y +
                         normalLocations[faceIndex].Z * shadingUnknown.Z) / j;
                }
            }

            Point3D[] unknownPoint = new Point3D[vert_count];
            int[] ai3 = new int[vert_count];

            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                unknownPoint[verticeIndex] = Point3D.Empty;
                ai3[verticeIndex] = 0;
            }

            for (int faceIndex = 0; faceIndex < face_count; faceIndex++)
            {
                if (gouraud_shade[faceIndex] != shadeValue)
                {
                    continue;
                }

                for (int faceVerticeIndex = 0; faceVerticeIndex < face_vertices_count[faceIndex]; faceVerticeIndex++)
                {
                    int l1 = face_vertices[faceIndex][faceVerticeIndex];

                    unknownPoint[l1] += normalLocations[faceIndex];
                    ai3[l1]++;
                }
            }

            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                if (ai3[verticeIndex] > 0)
                {
                    int a = (unknownPoint[verticeIndex].X * shadingUnknown.X +
                             unknownPoint[verticeIndex].Y * shadingUnknown.Y +
                             unknownPoint[verticeIndex].Z * shadingUnknown.Z);
                    int b = j * ai3[verticeIndex];

                    cfn[verticeIndex] = a / b;
                }
            }
        }

        public void calculateNormals()
        {
            if (dontRecieveShadows && noCollider)
            {
                return;
            }

            for (int j = 0; j < face_count; j++)
            {
                int[] ai = face_vertices[j];
                int k = WorldVerticeLocations[ai[0]].X;
                int l = WorldVerticeLocations[ai[0]].Y;
                int i1 = WorldVerticeLocations[ai[0]].Z;
                int j1 = WorldVerticeLocations[ai[1]].X - k;
                int k1 = WorldVerticeLocations[ai[1]].Y - l;
                int l1 = WorldVerticeLocations[ai[1]].Z - i1;
                int i2 = WorldVerticeLocations[ai[2]].X - k;
                int j2 = WorldVerticeLocations[ai[2]].Y - l;
                int k2 = WorldVerticeLocations[ai[2]].Z - i1;

                int xDistance = k1 * k2 - j2 * l1;
                int yDistance = l1 * i2 - k2 * j1;
                int j3;
                for (j3 = j1 * j2 - i2 * k1; xDistance > 8192 || yDistance > 8192 || j3 > 8192 || xDistance < -8192 || yDistance < -8192 || j3 < -8192; j3 >>= 1)
                {
                    xDistance >>= 1;
                    yDistance >>= 1;
                }

                // normalize
                int k3 = (int)(256D * Math.Sqrt(xDistance * xDistance + yDistance * yDistance + j3 * j3));
                if (k3 <= 0)
                {
                    k3 = 1;
                }

                normalLocations[j] = new Point3D(
                    (xDistance * 0x10000) / k3,
                    (yDistance * 0x10000) / k3,
                    (j3 * 65535) / k3);

                cgh[j] = -1;
            }

            normalize();
        }

        public void UpdateWorldTransformation()
        {
            if (objectState == 2)
            {
                objectState = 0;

                for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
                {
                    WorldVerticeLocations[verticeIndex] = verticeLocations[verticeIndex];
                }


                distVar = maximumBounds.X = maximumBounds.Y = maximumBounds.Z = 0x98967f;
                minimumBounds.X = minimumBounds.Y = minimumBounds.Z = -maximumBounds.Z/*unchecked((int)0xff676981)*/;
                return;
            }

            if (objectState == 1)
            {
                objectState = 0;

                for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
                {
                    WorldVerticeLocations[verticeIndex] = verticeLocations[verticeIndex];
                }

                if (ckm >= 2)
                {
                    rotate(rotation);
                }

                if (ckm >= 3)
                {
                    Point3D scale = new Point3D(ckd, cke, ckf);
                    ScaleVertices(scale);
                }

                if (ckm >= 4)
                {
                    scaleVertices(ckg, ckh, cki, ckj, ckk, ckl);
                }

                if (ckm >= 1)
                {
                    OffsetWorldVertices(location);
                }

                calculateObjectBounds();
                calculateNormals();
            }
        }

        public void cnh(Point3D loc, int arg3, int arg4, int arg5, int arg6, int arg7)
        {
            UpdateWorldTransformation();

            if (minimumBounds.X > Camera.FarLocation.X ||
                maximumBounds.X < Camera.NearLocation.X ||
                minimumBounds.Y > Camera.FarLocation.Y ||
                maximumBounds.Y < Camera.NearLocation.Y ||
                minimumBounds.Z > Camera.FarLocation.Z ||
                maximumBounds.Z < Camera.NearLocation.Z)
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

            if (arg5 != 0)
            {
                i1 = cif[arg5];
                j1 = cif[arg5 + 1024];
            }

            if (arg4 != 0)
            {
                i2 = cif[arg4];
                j2 = cif[arg4 + 1024];
            }

            if (arg3 != 0)
            {
                k1 = cif[arg3];
                l1 = cif[arg3 + 1024];
            }

            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                int l2 = WorldVerticeLocations[verticeIndex].X - loc.X;
                int i3 = WorldVerticeLocations[verticeIndex].Y - loc.Y;
                int j3 = WorldVerticeLocations[verticeIndex].Z - loc.Z;

                if (arg5 != 0)
                {
                    int j = i3 * i1 + l2 * j1 >> 15;
                    i3 = i3 * j1 - l2 * i1 >> 15;
                    l2 = j;
                }
                if (arg4 != 0)
                {
                    int k = j3 * i2 + l2 * j2 >> 15;
                    j3 = j3 * j2 - l2 * i2 >> 15;
                    l2 = k;
                }
                if (arg3 != 0)
                {
                    int l = i3 * l1 - j3 * k1 >> 15;
                    j3 = i3 * k1 + j3 * l1 >> 15;
                    i3 = l;
                }
                if (j3 >= arg7)
                {
                    cfl[verticeIndex] = (l2 << arg6) / j3;
                }
                else
                {
                    cfl[verticeIndex] = l2 << arg6;
                }

                if (j3 >= arg7)
                {
                    cfm[verticeIndex] = (i3 << arg6) / j3;
                }
                else
                {
                    cfm[verticeIndex] = i3 << arg6;
                }

                vertX[verticeIndex] = l2;
                vertY[verticeIndex] = i3;
                vertZ[verticeIndex] = j3;
            }
        }

        public void cni()
        {
            UpdateWorldTransformation();

            for (int verticeIndex = 0; verticeIndex < vert_count; verticeIndex++)
            {
                verticeLocations[verticeIndex] = WorldVerticeLocations[verticeIndex];
            }

            location.X = location.Y = location.Z = 0;
            rotation.X = rotation.Y = rotation.Z = 0;
            ckd = cke = ckf = 256;
            ckg = ckh = cki = ckj = ckk = ckl = 256;
            ckm = 0;
        }

        public ObjectModel CreateParent()
        {
            ObjectModel[] ai = new ObjectModel[1];
            ai[0] = this;
            ObjectModel j = new ObjectModel(ai, 1);
            j.cgm = cgm;
            j.isGiantCrystal = isGiantCrystal;
            return j;
        }

        public ObjectModel CreateParent(bool flag, bool flag1, bool flag2, bool flag3)
        {
            ObjectModel[] ai = new ObjectModel[1];
            ai[0] = this;
            ObjectModel j = new ObjectModel(ai, 1, flag, flag1, flag2, flag3);
            j.cgm = cgm;
            return j;
        }

        public void CopyTranslation(ObjectModel j)
        {
            rotation = j.rotation;
            location = j.location;

            cmm();
            objectState = 1;
        }

        public int getShadeValue(sbyte[] arg0)
        {
            for (; arg0[clg] == 10 || arg0[clg] == 13; clg++)
            {
                ;
            }

            int j = cih[arg0[clg++] & 0xff];
            int k = cih[arg0[clg++] & 0xff];
            int l = cih[arg0[clg++] & 0xff];
            int i1 = (j * 4096 + k * 64 + l) - 0x20000;
            if (i1 == 0x1e240)
            {
                i1 = shadeValue;
            }

            return i1;
        }

        public int vert_count;
        public int[] vertX;
        public int[] vertY;
        public int[] vertZ;
        public int[] cfl;
        public int[] cfm;
        public int[] cfn;
        public int[] vertexColor;
        public int face_count;
        public int[] face_vertices_count;
        public int[][] face_vertices;
        public int[] texture_back;
        public int[] texture_front;
        public int[] cgg;
        public int[] cgh;
        public int[] gouraud_shade;
        public int cgm;
        public int objectState;
        public bool visible;
        public bool chh;
        public bool chi;
        public bool isGiantCrystal;
        public int index;
        public int[] entityType;
        public int[] chm;
        bool chn;
        public bool noCollider;
        public bool dontRecieveShadows;
        public bool cic;
        public bool cid;
        static int[] cie;
        static int[] cif;
        static int[] cig;
        static int[] cih;
        int shadeValue;
        public int totalVerticeCount;

        public Vector3[] _vertices;

        int totalFaceCount;
        int[][] cje;
        int ckd;
        int cke;
        int ckf;
        int ckg;
        int ckh;
        int cki;
        int ckj;
        int ckk;
        int ckl;
        int ckm;
        int distVar;
        int cld;
        public int cle;
        public int clf;
        int clg;
    }
}
