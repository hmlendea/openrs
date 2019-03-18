using System;

using NuciExtensions;
using NuciXNA.Primitives;

namespace OpenRS.Net.Client.Game.Cameras
{
    public class Camera
    {
        public static Point3D NearLocation { get; set; }
        public static Point3D FarLocation { get; set; }

        public Point3D[] VertLocations { get; set; }

        static readonly Random Random = new Random();

        Point3D viewLocation;
        Point3D[] sceneObjectLocations;

        public Camera(GraphicsEngine gameimage, int maxObjects, int maxVisibleObjects, int maxSceneObjects)
        {
            NearLocation = new Point3D();
            FarLocation = new Point3D();

            VertLocations = new Point3D[40];

            viewLocation = Point3D.Empty;
            sceneObjectLocations = new Point3D[maxSceneObjects];

            bba = 50;
            bbb = new int[bba];
            bbc = new int[bba][];

            for (int j = 0; j < bba; j++)
            {
                bbc[j] = new int[256];
            }

            bbf = 5;
            zoom1 = 1000;
            zoom2 = 1000;
            zoom3 = 20;
            zoom4 = 10;
            bbm = false;
            bbn = 1.1000000000000001D;
            bca = 1;
            bcb = false;
            maxHighlightedObjects = 100;
            _highlightedObjects = new ObjectModel[maxHighlightedObjects];
            bch = new int[maxHighlightedObjects];
            bci = 512;
            bcj = 256;
            bck = 192;
            bcl = 256;
            bcm = 256;
            bcn = 8;
            bda = 4;
            bfl = new int[40];
            bfm = new int[40];
            bfn = new int[40];
            gameImage = gameimage;
            bcj = gameimage.GameSize.Width / 2;
            bck = gameimage.GameSize.Height / 2;
            bfh = gameimage.pixels;
            currentObjectCount = 0;
            totalModelCount = maxObjects;
            objectCache = new ObjectModel[totalModelCount];
            bdk = new int[totalModelCount];
            currentModelIndex = 0;
            visibleModels = new CameraModel[maxVisibleObjects];
            for (int k = 0; k < maxVisibleObjects; k++)
            {
                visibleModels[k] = new CameraModel();
            }

            bdn = 0;
            highlightedObject = new ObjectModel(maxSceneObjects * 2, maxSceneObjects);
            sceneObjectId = new int[maxSceneObjects];
            bee = new int[maxSceneObjects];
            bef = new int[maxSceneObjects];
            beg = new int[maxSceneObjects];
            if (bfe == null)
            {
                bfe = new sbyte[17691];
            }

            bde = 0;
            bdf = 0;
            bdg = 0;
            for (int i1 = 0; i1 < 256; i1++)
            {
                var val = (int)(Math.Sin(i1 * 0.02454369D) * 32768D);
                bbl[i1] = (int)(Math.Sin(i1 * 0.02454369D) * 32768D);
                bbl[i1 + 256] = (int)(Math.Cos(i1 * 0.02454369D) * 32768D);
            }

            for (int j1 = 0; j1 < 1024; j1++)
            {
                var val = (int)(Math.Sin(j1 * 0.00613592315D) * 32768D);
                bbk[j1] = (int)(Math.Sin(j1 * 0.00613592315D) * 32768D);
                bbk[j1 + 1024] = (int)(Math.Cos(j1 * 0.00613592315D) * 32768D);
            }

        }

        public void addModel(ObjectModel k)
        {
            if (k == null)
            {
                Console.WriteLine("Warning tried to add null object!");
            }

            if (currentObjectCount < totalModelCount)
            {
                bdk[currentObjectCount] = 0;
                objectCache[currentObjectCount++] = k;
            }
        }

        public void removeModel(ObjectModel arg0)
        {
            for (int k = 0; k < currentObjectCount; k++)
            {
                if (objectCache[k] == arg0)
                {
                    currentObjectCount--;
                    for (int i1 = k; i1 < currentObjectCount; i1++)
                    {
                        objectCache[i1] = objectCache[i1 + 1];
                        bdk[i1] = bdk[i1 + 1];
                    }

                }
            }
        }

        public void cleanUp()
        {
            bhb();
            for (int k = 0; k < currentObjectCount; k++)
            {
                objectCache[k] = null;
            }

            currentObjectCount = 0;
        }

        public void bhb()
        {
            bdn = 0;
            highlightedObject.resetObjectIndexes();
        }

        public void removeLastUpdates(int k)
        {
            bdn -= k;
            highlightedObject.cll(k, k * 2);
            if (bdn < 0)
            {
                bdn = 0;
            }
        }

        public int addSpriteToScene(int objectId, Point3D location, int width, int height, int j2)
        {
            sceneObjectId[bdn] = objectId;
            sceneObjectLocations[bdn] = location;
            bee[bdn] = width;
            bef[bdn] = height;
            beg[bdn] = 0;

            int k2 = highlightedObject.addVertex(location);
            int l2 = highlightedObject.addVertex(new Point3D(location.X, location.Y - height, location.Z));
            int[] ai = { k2, l2 };

            highlightedObject.addFaceVertices(2, ai, 0, 0);
            highlightedObject.entityType[bdn] = j2;
            highlightedObject.chm[bdn++] = 0;

            return bdn - 1;
        }

        public void bhe(int k)
        {
            highlightedObject.chm[k] = 1;
        }

        public void bhf(int k, int i1)
        {
            beg[k] = i1;
        }

        public void setMousePosition(int k, int i1)
        {
            bcc = k - bcl;
            bcd = i1;
            bce = 0;
            bcb = true;
        }

        public int getOptionCount()
        {
            return bce;
        }

        public int[] getHighlightedPlayers()
        {
            return bch;
        }

        public ObjectModel[] getHighlightedObjects()
        {
            return _highlightedObjects;
        }

        public void setCameraSize(int arg0, int arg1, int arg2, int arg3, int arg4, int arg5)
        {
            bcj = arg2;
            bck = arg3;
            bcl = arg0;
            bcm = arg1;
            bci = arg4;
            bcn = arg5;
            bfi = new CameraVariable[arg3 + arg1];
            for (int k = 0; k < arg3 + arg1; k++)
            {
                bfi[k] = new CameraVariable();
            }
        }

        void bhl(CameraModel[] arg0, int arg1, int arg2)
        {
            if (arg1 < arg2)
            {
                int k = arg1 - 1;
                int i1 = arg2 + 1;
                int j1 = (arg1 + arg2) / 2;
                CameraModel l1 = arg0[j1];
                arg0[j1] = arg0[arg1];
                arg0[arg1] = l1;
                int k1 = l1.Scale;
                while (k < i1)
                {
                    do
                    {
                        i1--;
                    }
                    while (arg0[i1].Scale < k1);
                    do
                    {
                        k++;
                    }
                    while (arg0[k].Scale > k1);
                    if (k < i1)
                    {
                        CameraModel l2 = arg0[k];
                        arg0[k] = arg0[i1];
                        arg0[i1] = l2;
                    }
                }
                bhl(arg0, arg1, i1);
                bhl(arg0, i1 + 1, arg2);
            }
        }

        public void bhm(int arg0, CameraModel[] models, int index)
        {
            for (int k = 0; k <= index; k++)
            {
                models[k].bld = false;
                models[k].ble = k;
                models[k].blf = -1;
            }

            int i1 = 0;
            do
            {
                while (models[i1].bld)
                {
                    i1++;
                }

                if (i1 == index)
                {
                    return;
                }

                CameraModel l1 = models[i1];
                l1.bld = true;
                int j1 = i1;
                int k1 = i1 + arg0;
                if (k1 >= index)
                {
                    k1 = index - 1;
                }

                for (int i2 = k1; i2 >= j1 + 1; i2--)
                {
                    CameraModel l2 = models[i2];
                    if (l1.bkd < l2.bkf && l2.bkd < l1.bkf && l1.bke < l2.bkg && l2.bke < l1.bkg && l1.ble != l2.blf && !bjc(l1, l2) && bjd(l2, l1))
                    {
                        bhn(models, j1, i2);
                        if (models[i2] != l2)
                        {
                            i2++;
                        }

                        j1 = bgk;
                        l2.blf = l1.ble;
                    }
                }

            } while (true);
        }

        public bool bhn(CameraModel[] models, int start, int stop)
        {
            do
            {
                CameraModel l1 = models[start];
                for (int k = start + 1; k <= stop; k++)
                {
                    CameraModel l2 = models[k];
                    if (!bjc(l2, l1))
                    {
                        break;
                    }

                    models[start] = l2;
                    models[k] = l1;
                    start = k;
                    if (start == stop)
                    {
                        bgk = start;
                        bgl = start - 1;
                        return true;
                    }
                }

                CameraModel l3 = models[stop];
                for (int i1 = stop - 1; i1 >= start; i1--)
                {
                    CameraModel l4 = models[i1];
                    if (!bjc(l3, l4))
                    {
                        break;
                    }

                    models[stop] = l4;
                    models[i1] = l3;
                    stop = i1;
                    if (start == stop)
                    {
                        bgk = stop + 1;
                        bgl = stop;
                        return true;
                    }
                }

                if (start + 1 >= stop)
                {
                    bgk = start;
                    bgl = stop;
                    return false;
                }
                if (!bhn(models, start + 1, stop))
                {
                    bgk = start;
                    return false;
                }
                stop = bgl;
            } while (true);
        }

        public void bia(int k, int i1, int j1)
        {
            int k1 = -bde + 1024 & 0x3ff;
            int l1 = -bdf + 1024 & 0x3ff;
            int i2 = -bdg + 1024 & 0x3ff;
            if (i2 != 0)
            {
                int j2 = bbk[i2];
                int i3 = bbk[i2 + 1024];
                int l3 = i1 * j2 + k * i3 >> 15;
                i1 = i1 * i3 - k * j2 >> 15;
                k = l3;
            }
            if (k1 != 0)
            {
                int k2 = bbk[k1];
                int j3 = bbk[k1 + 1024];
                int i4 = i1 * j3 - j1 * k2 >> 15;
                j1 = i1 * k2 + j1 * j3 >> 15;
                i1 = i4;
            }
            if (l1 != 0)
            {
                int l2 = bbk[l1];
                int k3 = bbk[l1 + 1024];
                int j4 = j1 * l2 + k * k3 >> 15;
                j1 = j1 * k3 - k * l2 >> 15;
                k = j4;
            }
            if (k < NearLocation.X)
            {
                NearLocation = new Point3D(k, NearLocation.Y, NearLocation.Z);
            }

            if (k > FarLocation.X)
            {
                FarLocation = new Point3D(k, FarLocation.Y, FarLocation.Z);
            }

            if (i1 < NearLocation.Y)
            {
                NearLocation = new Point3D(NearLocation.X, i1, NearLocation.Z);
            }

            if (i1 > FarLocation.Y)
            {
                FarLocation = new Point3D(FarLocation.X, i1, FarLocation.Z);
            }

            if (j1 < NearLocation.Z)
            {
                NearLocation = new Point3D(NearLocation.X, NearLocation.Y, j1);
            }

            if (j1 > FarLocation.Z)
            {
                FarLocation = new Point3D(FarLocation.X, FarLocation.Y, j1);
            }
        }

        public void finishCamera()
        {
            int k4 = bcj * zoom1 >> bcn;
            int l4 = bck * zoom1 >> bcn;

            NearLocation = Point3D.Empty;
            FarLocation = Point3D.Empty;

            bia(-k4, -l4, zoom1);
            bia(-k4, l4, zoom1);
            bia(k4, -l4, zoom1);
            bia(k4, l4, zoom1);
            bia(-bcj, -bck, 0);
            bia(-bcj, bck, 0);
            bia(bcj, -bck, 0);
            bia(bcj, bck, 0);

            NearLocation = new Point3D(
                NearLocation.X + viewLocation.X,
                NearLocation.Y + viewLocation.Y,
                NearLocation.Z + viewLocation.Z);

            FarLocation = new Point3D(
                FarLocation.X + viewLocation.X,
                FarLocation.Y + viewLocation.Y,
                FarLocation.Z + viewLocation.Z);

            objectCache[currentObjectCount] = highlightedObject;
            highlightedObject.objectState = 2;

            for (int k1 = 0; k1 < currentObjectCount; k1++)
            {
                if (objectCache[k1] != null)
                {
                    objectCache[k1].cnh(viewLocation, bde, bdf, bdg, bcn, bbf);
                }
            }

            int msSlept = 0;
            while (objectCache[currentObjectCount] == null)
            {
                System.Threading.Thread.Sleep(10);
                msSlept += 10;
                if (msSlept > 1000)
                {
                    return;
                }
            }

            objectCache[currentObjectCount].cnh(viewLocation, bde, bdf, bdg, bcn, bbf);
            currentModelIndex = 0;
            for (int i5 = 0; i5 < currentObjectCount; i5++)
            {
                ObjectModel k = objectCache[i5];
                if (k == null)
                {
                    continue;
                }

                if (k.visible)
                {
                    for (int l1 = 0; l1 < k.face_count; l1++)
                    {
                        int j5 = k.face_vertices_count[l1];
                        int[] ai1 = k.face_vertices[l1];
                        bool flag = false;
                        for (int i6 = 0; i6 < j5; i6++)
                        {
                            int k2 = k.vertZ[ai1[i6]];
                            if (k2 <= bbf || k2 >= zoom1)
                            {
                                continue;
                            }

                            flag = true;
                            break;
                        }

                        if (flag)
                        {
                            int j3 = 0;
                            for (int j7 = 0; j7 < j5; j7++)
                            {
                                int l2 = k.cfl[ai1[j7]];
                                if (l2 > -bcj)
                                {
                                    j3 |= 1;
                                }

                                if (l2 < bcj)
                                {
                                    j3 |= 2;
                                }

                                if (j3 == 3)
                                {
                                    break;
                                }
                            }

                            if (j3 == 3)
                            {
                                int k3 = 0;
                                for (int k8 = 0; k8 < j5; k8++)
                                {
                                    int i3 = k.cfm[ai1[k8]];
                                    if (i3 > -bck)
                                    {
                                        k3 |= 1;
                                    }

                                    if (i3 < bck)
                                    {
                                        k3 |= 2;
                                    }

                                    if (k3 == 3)
                                    {
                                        break;
                                    }
                                }

                                if (k3 == 3)
                                {
                                    CameraModel l9 = visibleModels[currentModelIndex];
                                    l9.Object = k;
                                    l9.faceVertCountIndex1 = l1;
                                    bja(currentModelIndex);
                                    int textureIndex;
                                    if (l9.blb < 0)
                                    {
                                        textureIndex = k.texture_back[l1];
                                    }
                                    else
                                    {
                                        textureIndex = k.texture_front[l1];
                                    }

                                    if (textureIndex != 0xbc614e)
                                    {
                                        int l3 = 0;
                                        for (int l11 = 0; l11 < j5; l11++)
                                        {
                                            l3 += k.vertZ[ai1[l11]];
                                        }

                                        l9.Scale = l3 / j5 + k.cgm;
                                        l9.currentTextureIndex = textureIndex;
                                        currentModelIndex++;
                                    }
                                }
                            }
                        }
                    }

                }
            }

            ObjectModel i1 = highlightedObject;
            if (i1.visible)
            {
                for (int i2 = 0; i2 < i1.face_count; i2++)
                {
                    int[] ai = i1.face_vertices[i2];
                    int l5 = ai[0];
                    int j6 = i1.cfl[l5];
                    int k7 = i1.cfm[l5];
                    int l8 = i1.vertZ[l5];
                    if (l8 > bbf && l8 < zoom2)
                    {
                        int i10 = (bee[i2] << bcn) / l8;
                        int i11 = (bef[i2] << bcn) / l8;
                        if (j6 - i10 / 2 <= bcj && j6 + i10 / 2 >= -bcj && k7 - i11 <= bck && k7 >= -bck)
                        {
                            CameraModel l12 = visibleModels[currentModelIndex];
                            l12.Object = i1;
                            l12.faceVertCountIndex1 = i2;
                            bjb(currentModelIndex);
                            l12.Scale = (l8 + i1.vertZ[ai[1]]) / 2;
                            currentModelIndex++;
                        }
                    }
                }

            }
            if (currentModelIndex == 0)
            {
                return;
            }

            bbe = currentModelIndex;
            bhl(visibleModels, 0, currentModelIndex - 1);
            bhm(100, visibleModels, currentModelIndex);
            for (int k5 = 0; k5 < currentModelIndex; k5++)
            {
                CameraModel l6 = visibleModels[k5];
                ObjectModel model = l6.Object;
                int j2 = l6.faceVertCountIndex1;
                if (model == highlightedObject)
                {
                    int[] ai2 = model.face_vertices[j2];
                    int l7 = ai2[0];
                    int i9 = model.cfl[l7];
                    int j10 = model.cfm[l7];
                    int j11 = model.vertZ[l7];
                    int i12 = (bee[j2] << bcn) / j11;
                    int k12 = (bef[j2] << bcn) / j11;
                    int j13 = j10 - model.cfm[ai2[1]];
                    int k13 = ((model.cfl[ai2[1]] - i9) * j13) / k12;
                    k13 = model.cfl[ai2[1]] - i9;
                    int i14 = i9 - i12 / 2;
                    int k14 = (bcm + j10) - k12;
                    gameImage.DrawVisibleEntity(i14 + bcl, k14, i12, k12, sceneObjectId[j2], k13, (256 << bcn) / j11);
                    if (bcb && bce < maxHighlightedObjects)
                    {
                        i14 += (beg[j2] << bcn) / j11;
                        if (bcd >= k14 && bcd <= k14 + k12 && bcc >= i14 && bcc <= i14 + i12 && !model.cic && model.chm[j2] == 0)
                        {
                            _highlightedObjects[bce] = model;
                            bch[bce] = j2;
                            bce++;
                        }
                    }
                }
                else
                {
                    int k10 = 0;
                    int j12 = 0;
                    int vertCount = model.face_vertices_count[j2];
                    int[] ai3 = model.face_vertices[j2];
                    if (model.gouraud_shade[j2] != 0xbc614e)
                    {
                        if (l6.blb < 0)
                        {
                            j12 = model.clf - model.gouraud_shade[j2];
                        }
                        else
                        {
                            j12 = model.clf + model.gouraud_shade[j2];
                        }
                    }

                    for (int l13 = 0; l13 < vertCount; l13++)
                    {
                        int i4 = ai3[l13];

                        VertLocations[l13] = new Point3D(
                            model.vertX[i4],
                            model.vertY[i4],
                            model.vertZ[i4]);

                        if (model.gouraud_shade[j2] == 0xbc614e)
                        {
                            if (l6.blb < 0)
                            {
                                j12 = (model.clf - model.cfn[i4]) + model.vertexColor[i4];
                            }
                            else
                            {
                                j12 = model.clf + model.cfn[i4] + model.vertexColor[i4];
                            }
                        }

                        if (model.vertZ[i4] >= bbf)
                        {
                            bfl[k10] = model.cfl[i4];
                            bfm[k10] = model.cfm[i4];
                            bfn[k10] = j12;
                            if (model.vertZ[i4] > zoom4)
                            {
                                bfn[k10] += (model.vertZ[i4] - zoom4) / zoom3;
                            }

                            k10++;
                        }
                        else
                        {
                            int k11;
                            if (l13 == 0)
                            {
                                k11 = ai3[vertCount - 1];
                            }
                            else
                            {
                                k11 = ai3[l13 - 1];
                            }

                            if (model.vertZ[k11] >= bbf)
                            {
                                int j9 = model.vertZ[i4] - model.vertZ[k11];
                                int k6 = model.vertX[i4] - ((model.vertX[i4] - model.vertX[k11]) * (model.vertZ[i4] - bbf)) / j9;
                                int i8 = model.vertY[i4] - ((model.vertY[i4] - model.vertY[k11]) * (model.vertZ[i4] - bbf)) / j9;
                                bfl[k10] = (k6 << bcn) / bbf;
                                bfm[k10] = (i8 << bcn) / bbf;
                                bfn[k10] = j12;
                                k10++;
                            }
                            if (l13 == vertCount - 1)
                            {
                                k11 = ai3[0];
                            }
                            else
                            {
                                k11 = ai3[l13 + 1];
                            }

                            if (model.vertZ[k11] >= bbf)
                            {
                                int k9 = model.vertZ[i4] - model.vertZ[k11];
                                int i7 = model.vertX[i4] - ((model.vertX[i4] - model.vertX[k11]) * (model.vertZ[i4] - bbf)) / k9;
                                int j8 = model.vertY[i4] - ((model.vertY[i4] - model.vertY[k11]) * (model.vertZ[i4] - bbf)) / k9;
                                bfl[k10] = (i7 << bcn) / bbf;
                                bfm[k10] = (j8 << bcn) / bbf;
                                bfn[k10] = j12;
                                k10++;
                            }
                        }
                    }

                    for (int j14 = 0; j14 < vertCount; j14++)
                    {
                        if (bfn[j14] < 0)
                        {
                            bfn[j14] = 0;
                        }
                        else
                            if (bfn[j14] > 255)
                        {
                            bfn[j14] = 255;
                        }

                        if (l6.currentTextureIndex >= 0)
                        {
                            if (bel[l6.currentTextureIndex] == 1)
                            {
                                bfn[j14] <<= 9;
                            }
                            else
                            {
                                bfn[j14] <<= 6;
                            }
                        }
                    }

                    bic(0, 0, 0, 0, k10, bfl, bfm, bfn, model, j2);
                    if (bfk > bfj)
                    {
                        renderModel(0, 0, vertCount, VertLocations, l6.currentTextureIndex, model);
                    }
                }
            }

            bcb = false;
        }

        void bic(int arg0, int arg1, int arg2, int arg3, int arg4, int[] arg5, int[] arg6,
                int[] arg7, ObjectModel arg8, int arg9)
        {
            if (arg4 == 3)
            {
                int k = arg6[0] + bcm;
                int l1 = arg6[1] + bcm;
                int l2 = arg6[2] + bcm;
                int l3 = arg5[0];
                int i5 = arg5[1];
                int k6 = arg5[2];
                int i8 = arg7[0];
                int k9 = arg7[1];
                int k10 = arg7[2];
                int k11 = (bcm + bck) - 1;
                int i12 = 0;
                int k12 = 0;
                int i13 = 0;
                int k13 = 0;
                int i14 = 0xbc614e;
                int k14 = -i14;//0xff439eb2;
                if (l2 != k)
                {
                    k12 = (k6 - l3 << 8) / (l2 - k);
                    k13 = (k10 - i8 << 8) / (l2 - k);
                    if (k < l2)
                    {
                        i12 = l3 << 8;
                        i13 = i8 << 8;
                        i14 = k;
                        k14 = l2;
                    }
                    else
                    {
                        i12 = k6 << 8;
                        i13 = k10 << 8;
                        i14 = l2;
                        k14 = k;
                    }
                    if (i14 < 0)
                    {
                        i12 -= k12 * i14;
                        i13 -= k13 * i14;
                        i14 = 0;
                    }
                    if (k14 > k11)
                    {
                        k14 = k11;
                    }
                }
                int i15 = 0;
                int k15 = 0;
                int i16 = 0;
                int k16 = 0;
                int i17 = 0xbc614e;
                int k17 = -i17;//0xff439eb2;
                if (l1 != k)
                {
                    k15 = (i5 - l3 << 8) / (l1 - k);
                    k16 = (k9 - i8 << 8) / (l1 - k);
                    if (k < l1)
                    {
                        i15 = l3 << 8;
                        i16 = i8 << 8;
                        i17 = k;
                        k17 = l1;
                    }
                    else
                    {
                        i15 = i5 << 8;
                        i16 = k9 << 8;
                        i17 = l1;
                        k17 = k;
                    }
                    if (i17 < 0)
                    {
                        i15 -= k15 * i17;
                        i16 -= k16 * i17;
                        i17 = 0;
                    }
                    if (k17 > k11)
                    {
                        k17 = k11;
                    }
                }
                int i18 = 0;
                int k18 = 0;
                int i19 = 0;
                int k19 = 0;
                int i20 = 0xbc614e;
                int k20 = -i20;//0xff439eb2;
                if (l2 != l1)
                {
                    k18 = (k6 - i5 << 8) / (l2 - l1);
                    k19 = (k10 - k9 << 8) / (l2 - l1);
                    if (l1 < l2)
                    {
                        i18 = i5 << 8;
                        i19 = k9 << 8;
                        i20 = l1;
                        k20 = l2;
                    }
                    else
                    {
                        i18 = k6 << 8;
                        i19 = k10 << 8;
                        i20 = l2;
                        k20 = l1;
                    }
                    if (i20 < 0)
                    {
                        i18 -= k18 * i20;
                        i19 -= k19 * i20;
                        i20 = 0;
                    }
                    if (k20 > k11)
                    {
                        k20 = k11;
                    }
                }
                bfj = i14;
                if (i17 < bfj)
                {
                    bfj = i17;
                }

                if (i20 < bfj)
                {
                    bfj = i20;
                }

                bfk = k14;
                if (k17 > bfk)
                {
                    bfk = k17;
                }

                if (k20 > bfk)
                {
                    bfk = k20;
                }

                int i21 = 0;
                for (arg2 = bfj; arg2 < bfk; arg2++)
                {
                    if (arg2 >= i14 && arg2 < k14)
                    {
                        arg0 = (arg1 = i12);
                        arg3 = i21 = i13;
                        i12 += k12;
                        i13 += k13;
                    }
                    else
                    {
                        arg0 = 0xa0000;
                        arg1 = unchecked((int)0xfff60000);
                    }
                    if (arg2 >= i17 && arg2 < k17)
                    {
                        if (i15 < arg0)
                        {
                            arg0 = i15;
                            arg3 = i16;
                        }
                        if (i15 > arg1)
                        {
                            arg1 = i15;
                            i21 = i16;
                        }
                        i15 += k15;
                        i16 += k16;
                    }
                    if (arg2 >= i20 && arg2 < k20)
                    {
                        if (i18 < arg0)
                        {
                            arg0 = i18;
                            arg3 = i19;
                        }
                        if (i18 > arg1)
                        {
                            arg1 = i18;
                            i21 = i19;
                        }
                        i18 += k18;
                        i19 += k19;
                    }
                    CameraVariable m7 = bfi[arg2];
                    m7.Unknown1 = arg0;
                    m7.Unknown2 = arg1;
                    m7.Unknown3 = arg3;
                    m7.Unknown4 = i21;
                }

                if (bfj < bcm - bck)
                {
                    bfj = bcm - bck;
                }
            }
            else
                if (arg4 == 4)
            {
                int i1 = arg6[0] + bcm;
                int i2 = arg6[1] + bcm;
                int i3 = arg6[2] + bcm;
                int i4 = arg6[3] + bcm;
                int j5 = arg5[0];
                int l6 = arg5[1];
                int j8 = arg5[2];
                int l9 = arg5[3];
                int l10 = arg7[0];
                int l11 = arg7[1];
                int j12 = arg7[2];
                int l12 = arg7[3];
                int j13 = (bcm + bck) - 1;
                int l13 = 0;
                int j14 = 0;
                int l14 = 0;
                int j15 = 0;
                int l15 = 0xbc614e;
                int j16 = -l15;// 0xff439eb2;
                if (i4 != i1)
                {
                    j14 = (l9 - j5 << 8) / (i4 - i1);
                    j15 = (l12 - l10 << 8) / (i4 - i1);
                    if (i1 < i4)
                    {
                        l13 = j5 << 8;
                        l14 = l10 << 8;
                        l15 = i1;
                        j16 = i4;
                    }
                    else
                    {
                        l13 = l9 << 8;
                        l14 = l12 << 8;
                        l15 = i4;
                        j16 = i1;
                    }
                    if (l15 < 0)
                    {
                        l13 -= j14 * l15;
                        l14 -= j15 * l15;
                        l15 = 0;
                    }
                    if (j16 > j13)
                    {
                        j16 = j13;
                    }
                }
                int l16 = 0;
                int j17 = 0;
                int l17 = 0;
                int j18 = 0;
                int l18 = 0xbc614e;
                int j19 = -l18;//0xff439eb2;
                if (i2 != i1)
                {
                    j17 = (l6 - j5 << 8) / (i2 - i1);
                    j18 = (l11 - l10 << 8) / (i2 - i1);
                    if (i1 < i2)
                    {
                        l16 = j5 << 8;
                        l17 = l10 << 8;
                        l18 = i1;
                        j19 = i2;
                    }
                    else
                    {
                        l16 = l6 << 8;
                        l17 = l11 << 8;
                        l18 = i2;
                        j19 = i1;
                    }
                    if (l18 < 0)
                    {
                        l16 -= j17 * l18;
                        l17 -= j18 * l18;
                        l18 = 0;
                    }
                    if (j19 > j13)
                    {
                        j19 = j13;
                    }
                }
                int l19 = 0;
                int j20 = 0;
                int l20 = 0;
                int j21 = 0;
                int k21 = 0xbc614e;
                int l21 = -k21;//0xff439eb2;
                if (i3 != i2)
                {
                    j20 = (j8 - l6 << 8) / (i3 - i2);
                    j21 = (j12 - l11 << 8) / (i3 - i2);
                    if (i2 < i3)
                    {
                        l19 = l6 << 8;
                        l20 = l11 << 8;
                        k21 = i2;
                        l21 = i3;
                    }
                    else
                    {
                        l19 = j8 << 8;
                        l20 = j12 << 8;
                        k21 = i3;
                        l21 = i2;
                    }
                    if (k21 < 0)
                    {
                        l19 -= j20 * k21;
                        l20 -= j21 * k21;
                        k21 = 0;
                    }
                    if (l21 > j13)
                    {
                        l21 = j13;
                    }
                }
                int i22 = 0;
                int j22 = 0;
                int k22 = 0;
                int l22 = 0;
                int i23 = 0xbc614e;
                int j23 = -i23;//0xff439eb2;
                if (i4 != i3)
                {
                    j22 = (l9 - j8 << 8) / (i4 - i3);
                    l22 = (l12 - j12 << 8) / (i4 - i3);
                    if (i3 < i4)
                    {
                        i22 = j8 << 8;
                        k22 = j12 << 8;
                        i23 = i3;
                        j23 = i4;
                    }
                    else
                    {
                        i22 = l9 << 8;
                        k22 = l12 << 8;
                        i23 = i4;
                        j23 = i3;
                    }
                    if (i23 < 0)
                    {
                        i22 -= j22 * i23;
                        k22 -= l22 * i23;
                        i23 = 0;
                    }
                    if (j23 > j13)
                    {
                        j23 = j13;
                    }
                }
                bfj = l15;
                if (l18 < bfj)
                {
                    bfj = l18;
                }

                if (k21 < bfj)
                {
                    bfj = k21;
                }

                if (i23 < bfj)
                {
                    bfj = i23;
                }

                bfk = j16;
                if (j19 > bfk)
                {
                    bfk = j19;
                }

                if (l21 > bfk)
                {
                    bfk = l21;
                }

                if (j23 > bfk)
                {
                    bfk = j23;
                }

                int k23 = 0;
                for (arg2 = bfj; arg2 < bfk; arg2++)
                {
                    if (arg2 >= l15 && arg2 < j16)
                    {
                        arg0 = arg1 = l13;
                        arg3 = k23 = l14;
                        l13 += j14;
                        l14 += j15;
                    }
                    else
                    {
                        arg0 = 0xa0000;
                        arg1 = unchecked((int)0xfff60000);//0xfff60000;
                    }
                    if (arg2 >= l18 && arg2 < j19)
                    {
                        if (l16 < arg0)
                        {
                            arg0 = l16;
                            arg3 = l17;
                        }
                        if (l16 > arg1)
                        {
                            arg1 = l16;
                            k23 = l17;
                        }
                        l16 += j17;
                        l17 += j18;
                    }
                    if (arg2 >= k21 && arg2 < l21)
                    {
                        if (l19 < arg0)
                        {
                            arg0 = l19;
                            arg3 = l20;
                        }
                        if (l19 > arg1)
                        {
                            arg1 = l19;
                            k23 = l20;
                        }
                        l19 += j20;
                        l20 += j21;
                    }
                    if (arg2 >= i23 && arg2 < j23)
                    {
                        if (i22 < arg0)
                        {
                            arg0 = i22;
                            arg3 = k22;
                        }
                        if (i22 > arg1)
                        {
                            arg1 = i22;
                            k23 = k22;
                        }
                        i22 += j22;
                        k22 += l22;
                    }
                    CameraVariable m8 = bfi[arg2];
                    m8.Unknown1 = arg0;
                    m8.Unknown2 = arg1;
                    m8.Unknown3 = arg3;
                    m8.Unknown4 = k23;
                }

                if (bfj < bcm - bck)
                {
                    bfj = bcm - bck;
                }
            }
            else
            {
                bfk = bfj = arg6[0] += bcm;
                for (arg2 = 1; arg2 < arg4; arg2++)
                {
                    int j1;
                    if ((j1 = arg6[arg2] += bcm) < bfj)
                    {
                        bfj = j1;
                    }
                    else
                        if (j1 > bfk)
                    {
                        bfk = j1;
                    }
                }

                if (bfj < bcm - bck)
                {
                    bfj = bcm - bck;
                }

                if (bfk >= bcm + bck)
                {
                    bfk = (bcm + bck) - 1;
                }

                if (bfj >= bfk)
                {
                    return;
                }

                for (arg2 = bfj; arg2 < bfk; arg2++)
                {
                    CameraVariable m1 = bfi[arg2];
                    m1.Unknown1 = 0xa0000;
                    m1.Unknown2 = unchecked((int)0xfff60000);//0xfff60000;
                }

                int k1 = arg4 - 1;
                int j2 = arg6[0];
                int j3 = arg6[k1];
                if (j2 < j3)
                {
                    int j4 = (arg5[0] << 8);
                    int k5 = ((arg5[k1] - arg5[0]) << 8) / (j3 - j2);
                    int i7 = (arg7[0] << 8);
                    int k8 = ((arg7[k1] - arg7[0]) << 8) / (j3 - j2);
                    if (j2 < 0)
                    {
                        j4 -= k5 * j2;
                        i7 -= k8 * j2;
                        j2 = 0;
                    }
                    if (j3 > bfk)
                    {
                        j3 = bfk;
                    }

                    for (arg2 = j2; arg2 <= j3; arg2++)
                    {
                        CameraVariable m3 = bfi[arg2];
                        m3.Unknown1 = m3.Unknown2 = j4;
                        m3.Unknown3 = m3.Unknown4 = i7;
                        j4 += k5;
                        i7 += k8;
                    }

                }
                else
                    if (j2 > j3)
                {
                    int k4 = arg5[k1] << 8;
                    int l5 = (arg5[0] - arg5[k1] << 8) / (j2 - j3);
                    int j7 = arg7[k1] << 8;
                    int l8 = (arg7[0] - arg7[k1] << 8) / (j2 - j3);
                    if (j3 < 0)
                    {
                        k4 -= l5 * j3;
                        j7 -= l8 * j3;
                        j3 = 0;
                    }
                    if (j2 > bfk)
                    {
                        j2 = bfk;
                    }

                    for (arg2 = j3; arg2 <= j2; arg2++)
                    {
                        CameraVariable m4 = bfi[arg2];
                        m4.Unknown1 = m4.Unknown2 = k4;
                        m4.Unknown3 = m4.Unknown4 = j7;
                        k4 += l5;
                        j7 += l8;
                    }

                }
                for (arg2 = 0; arg2 < k1; arg2++)
                {
                    int l4 = arg2 + 1;
                    int k2 = arg6[arg2];
                    int k3 = arg6[l4];
                    if (k2 < k3)
                    {
                        int i6 = arg5[arg2] << 8;
                        int k7 = (arg5[l4] - arg5[arg2] << 8) / (k3 - k2);
                        int i9 = arg7[arg2] << 8;
                        int i10 = (arg7[l4] - arg7[arg2] << 8) / (k3 - k2);
                        if (k2 < 0)
                        {
                            i6 -= k7 * k2;
                            i9 -= i10 * k2;
                            k2 = 0;
                        }
                        if (k3 > bfk)
                        {
                            k3 = bfk;
                        }

                        for (int i11 = k2; i11 <= k3; i11++)
                        {
                            CameraVariable m5 = bfi[i11];
                            if (i6 < m5.Unknown1)
                            {
                                m5.Unknown1 = i6;
                                m5.Unknown3 = i9;
                            }
                            if (i6 > m5.Unknown2)
                            {
                                m5.Unknown2 = i6;
                                m5.Unknown4 = i9;
                            }
                            i6 += k7;
                            i9 += i10;
                        }

                    }
                    else
                        if (k2 > k3)
                    {
                        int j6 = arg5[l4] << 8;
                        int l7 = (arg5[arg2] - arg5[l4] << 8) / (k2 - k3);
                        int j9 = arg7[l4] << 8;
                        int j10 = (arg7[arg2] - arg7[l4] << 8) / (k2 - k3);
                        if (k3 < 0)
                        {
                            j6 -= l7 * k3;
                            j9 -= j10 * k3;
                            k3 = 0;
                        }
                        if (k2 > bfk)
                        {
                            k2 = bfk;
                        }

                        for (int j11 = k3; j11 <= k2; j11++)
                        {
                            CameraVariable m6 = bfi[j11];
                            if (j6 < m6.Unknown1)
                            {
                                m6.Unknown1 = j6;
                                m6.Unknown3 = j9;
                            }
                            if (j6 > m6.Unknown2)
                            {
                                m6.Unknown2 = j6;
                                m6.Unknown4 = j9;
                            }
                            j6 += l7;
                            j9 += j10;
                        }

                    }
                }

                if (bfj < bcm - bck)
                {
                    bfj = bcm - bck;
                }
            }
            if (bcb && bce < maxHighlightedObjects && bcd >= bfj && bcd < bfk)
            {
                CameraVariable m2 = bfi[bcd];
                if (bcc >= m2.Unknown1 >> 8 && bcc <= m2.Unknown2 >> 8 && m2.Unknown1 <= m2.Unknown2 && !arg8.cic && arg8.chm[arg9] == 0)
                {
                    _highlightedObjects[bce] = arg8;
                    bch[bce] = arg9;
                    bce++;
                }
            }
        }

        void renderModel(int arg0, int arg1, int arg2, Point3D[] vertLocations, int textureIndex,
                ObjectModel arg7)
        {
            if (textureIndex == -2)
            {
                return;
            }

            if (textureIndex >= 0)
            {
                if (textureIndex >= bei)
                {
                    textureIndex = 0;
                }

                updateTextureSmoothing(textureIndex);
                int k = vertLocations[0].X;
                int j1 = vertLocations[0].Y;
                int i2 = vertLocations[0].Z;
                int l2 = k - vertLocations[1].X;
                int j3 = j1 - vertLocations[1].Y;
                int l3 = i2 - vertLocations[1].Z;
                arg2--;
                int l5 = vertLocations[arg2].X - k;
                int i7 = vertLocations[arg2].Y - j1;
                int j8 = vertLocations[arg2].Z - i2;

                if (bel[textureIndex] == 1)
                {
                    int k9 = l5 * j1 - i7 * k << 12;
                    int j10 = i7 * i2 - j8 * j1 << (5 - bcn) + 7 + 4;
                    int l10 = j8 * k - l5 * i2 << (5 - bcn) + 7;
                    int j11 = l2 * j1 - j3 * k << 12;
                    int l11 = j3 * i2 - l3 * j1 << (5 - bcn) + 7 + 4;
                    int j12 = l3 * k - l2 * i2 << (5 - bcn) + 7;
                    int l12 = j3 * l5 - l2 * i7 << 5;
                    int j13 = l3 * i7 - j3 * j8 << (5 - bcn) + 4;
                    int l13 = l2 * j8 - l3 * l5 >> bcn - 5;
                    int j14 = j10 >> 4;
                    int l14 = l11 >> 4;
                    int j15 = j13 >> 4;
                    int l15 = bfj - bcm;
                    int j16 = bci;
                    int l16 = bcl + bfj * j16;
                    byte byte1 = 1;
                    k9 += l10 * l15;
                    j11 += j12 * l15;
                    l12 += l13 * l15;

                    if (arg7.chi)
                    {
                        for (arg0 = bfj; arg0 < bfk; arg0 += byte1)
                        {
                            CameraVariable m4 = bfi[arg0];
                            arg1 = m4.Unknown1 >> 8;
                            int j17 = m4.Unknown2 >> 8;
                            int j20 = j17 - arg1;
                            if (j20 <= 0)
                            {
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                            else
                            {
                                int l21 = m4.Unknown3;
                                int j23 = (m4.Unknown4 - l21) / j20;
                                if (arg1 < -bcj)
                                {
                                    l21 += (-bcj - arg1) * j23;
                                    arg1 = -bcj;
                                    j20 = j17 - arg1;
                                }
                                if (j17 > bcj)
                                {
                                    int k17 = bcj;
                                    j20 = k17 - arg1;
                                }
                                bif(bfh, objectTexturePixels[textureIndex], 0, 0, k9 + j14 * arg1, j11 + l14 * arg1, l12 + j15 * arg1, j10, l11, j13, j20, l16 + arg1, l21, j23 << 2);
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                        }

                        return;
                    }
                    if (!bfa[textureIndex])
                    {
                        for (arg0 = bfj; arg0 < bfk; arg0 += byte1)
                        {
                            CameraVariable m5 = bfi[arg0];
                            arg1 = m5.Unknown1 >> 8;
                            int l17 = m5.Unknown2 >> 8;
                            int k20 = l17 - arg1;
                            if (k20 <= 0)
                            {
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                            else
                            {
                                int i22 = m5.Unknown3;
                                int k23 = (m5.Unknown4 - i22) / k20;
                                if (arg1 < -bcj)
                                {
                                    i22 += (-bcj - arg1) * k23;
                                    arg1 = -bcj;
                                    k20 = l17 - arg1;
                                }
                                if (l17 > bcj)
                                {
                                    int i18 = bcj;
                                    k20 = i18 - arg1;
                                }
                                bie(bfh, objectTexturePixels[textureIndex], 0, 0, k9 + j14 * arg1, j11 + l14 * arg1, l12 + j15 * arg1, j10, l11, j13, k20, l16 + arg1, i22, k23 << 2);
                                k9 += l10;
                                j11 += j12;
                                l12 += l13;
                                l16 += j16;
                            }
                        }

                        return;
                    }
                    for (arg0 = bfj; arg0 < bfk; arg0 += byte1)
                    {
                        CameraVariable m6 = bfi[arg0];
                        arg1 = m6.Unknown1 >> 8;
                        int j18 = (m6.Unknown2) >> 8;
                        int l20 = j18 - arg1;
                        if (l20 <= 0)
                        {
                            k9 += l10;
                            j11 += j12;
                            l12 += l13;
                            l16 += j16;
                        }
                        else
                        {
                            int j22 = m6.Unknown3;
                            int l23 = (m6.Unknown4 - j22) / l20;
                            if (arg1 < -bcj)
                            {
                                j22 += (-bcj - arg1) * l23;
                                arg1 = -bcj;
                                l20 = j18 - arg1;
                            }
                            if (j18 > bcj)
                            {
                                int k18 = bcj;
                                l20 = k18 - arg1;
                            }
                            big(bfh, 0, 0, 0, objectTexturePixels[textureIndex], k9 + j14 * arg1, j11 + l14 * arg1, l12 + j15 * arg1, j10, l11, j13, l20, l16 + arg1, j22, l23);
                            k9 += l10;
                            j11 += j12;
                            l12 += l13;
                            l16 += j16;
                        }
                    }

                    return;
                }
                int l9 = ((l5 * j1 - i7 * k) << 11);
                int k10 = ((i7 * i2 - j8 * j1) << (5 - bcn) + 6 + 4);
                int i11 = ((j8 * k - l5 * i2) << (5 - bcn) + 6);
                int k11 = ((l2 * j1 - j3 * k) << 11);
                int i12 = ((j3 * i2 - l3 * j1) << (5 - bcn) + 6 + 4);
                int k12 = ((l3 * k - l2 * i2) << (5 - bcn) + 6);
                int i13 = ((j3 * l5 - l2 * i7) << 5);
                int k13 = ((l3 * i7 - j3 * j8) << (5 - bcn) + 4);
                int i14 = ((l2 * j8 - l3 * l5) >> (bcn - 5));
                int k14 = (k10 >> 4);
                int i15 = (i12 >> 4);
                int k15 = (k13 >> 4);
                int i16 = bfj - bcm;
                int k16 = bci;
                int i17 = bcl + bfj * k16;
                byte byte2 = 1;
                l9 += i11 * i16;
                k11 += k12 * i16;
                i13 += i14 * i16;

                if (arg7.chi)
                {
                    for (arg0 = bfj; arg0 < bfk; arg0 += byte2)
                    {
                        CameraVariable m7 = bfi[arg0];
                        arg1 = m7.Unknown1 >> 8;
                        int l18 = m7.Unknown2 >> 8;
                        int i21 = l18 - arg1;
                        if (i21 <= 0)
                        {
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                        else
                        {
                            int k22 = m7.Unknown3;
                            int i24 = (m7.Unknown4 - k22) / i21;
                            if (arg1 < -bcj)
                            {
                                k22 += (-bcj - arg1) * i24;
                                arg1 = -bcj;
                                i21 = l18 - arg1;
                            }
                            if (l18 > bcj)
                            {
                                int i19 = bcj;
                                i21 = i19 - arg1;
                            }
                            bii(bfh, objectTexturePixels[textureIndex], 0, 0, l9 + k14 * arg1, k11 + i15 * arg1, i13 + k15 * arg1, k10, i12, k13, i21, i17 + arg1, k22, i24);
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                    }

                    return;
                }
                if (!bfa[textureIndex])
                {
                    for (arg0 = bfj; arg0 < bfk; arg0 += byte2)
                    {
                        CameraVariable m8 = bfi[arg0];
                        arg1 = m8.Unknown1 >> 8;
                        int j19 = m8.Unknown2 >> 8;
                        int j21 = j19 - arg1;
                        if (j21 <= 0)
                        {
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                        else
                        {
                            int l22 = m8.Unknown3;
                            int j24 = (m8.Unknown4 - l22) / j21;
                            if (arg1 < -bcj)
                            {
                                l22 += (-bcj - arg1) * j24;
                                arg1 = -bcj;
                                j21 = j19 - arg1;
                            }
                            if (j19 > bcj)
                            {
                                int k19 = bcj;
                                j21 = k19 - arg1;
                            }
                            bih(bfh, objectTexturePixels[textureIndex], 0, 0, l9 + k14 * arg1, k11 + i15 * arg1, i13 + k15 * arg1, k10, i12, k13, j21, i17 + arg1, l22, j24);
                            l9 += i11;
                            k11 += k12;
                            i13 += i14;
                            i17 += k16;
                        }
                    }

                    return;
                }
                for (arg0 = bfj; arg0 < bfk; arg0 += byte2)
                {
                    CameraVariable m9 = bfi[arg0];
                    arg1 = m9.Unknown1 >> 8;
                    int l19 = m9.Unknown2 >> 8;
                    int k21 = l19 - arg1;
                    if (k21 <= 0)
                    {
                        l9 += i11;
                        k11 += k12;
                        i13 += i14;
                        i17 += k16;
                    }
                    else
                    {
                        int i23 = m9.Unknown3;
                        int k24 = (m9.Unknown4 - i23) / k21;
                        if (arg1 < -bcj)
                        {
                            i23 += (-bcj - arg1) * k24;
                            arg1 = -bcj;
                            k21 = l19 - arg1;
                        }
                        if (l19 > bcj)
                        {
                            int i20 = bcj;
                            k21 = i20 - arg1;
                        }
                        bij(bfh, 0, 0, 0, objectTexturePixels[textureIndex], l9 + k14 * arg1, k11 + i15 * arg1, i13 + k15 * arg1, k10, i12, k13, k21, i17 + arg1, i23, k24);
                        l9 += i11;
                        k11 += k12;
                        i13 += i14;
                        i17 += k16;
                    }
                }

                return;
            }
            for (int i1 = 0; i1 < bba; i1++)
            {
                if (bbb[i1] == textureIndex)
                {
                    bbd = bbc[i1];
                    break;
                }
                if (i1 == bba - 1)
                {
                    var val = Random.NextDouble();
                    int k1 = (int)(val /*Math.random()*/ * bba);
                    if (k1 >= bbb.Length)
                    {
                        k1 = k1 - 1;
                    }

                    bbb[k1] = textureIndex;
                    textureIndex = -1 - textureIndex;
                    int j2 = (textureIndex >> 10 & 0x1f) * 8;
                    int i3 = (textureIndex >> 5 & 0x1f) * 8;
                    int k3 = (textureIndex & 0x1f) * 8;
                    for (int i4 = 0; i4 < 256; i4++)
                    {
                        int i6 = i4 * i4;
                        int j7 = (j2 * i6) / 0x10000;
                        int k8 = (i3 * i6) / 0x10000;
                        int i10 = (k3 * i6) / 0x10000;
                        bbc[k1][255 - i4] = (j7 << 16) + (k8 << 8) + i10;
                    }

                    bbd = bbc[k1];
                }
            }

            int l1 = bci;
            int k2 = bcl + bfj * l1;
            byte byte0 = 1;

            if (arg7.isGiantCrystal)
            {
                for (arg0 = bfj; arg0 < bfk; arg0 += byte0)
                {
                    CameraVariable m1 = bfi[arg0];
                    arg1 = m1.Unknown1 >> 8;
                    int j4 = m1.Unknown2 >> 8;
                    int j6 = j4 - arg1;
                    if (j6 <= 0)
                    {
                        k2 += l1;
                    }
                    else
                    {
                        int k7 = m1.Unknown3;
                        int l8 = (m1.Unknown4 - k7) / j6;
                        if (arg1 < -bcj)
                        {
                            k7 += (-bcj - arg1) * l8;
                            arg1 = -bcj;
                            j6 = j4 - arg1;
                        }
                        if (j4 > bcj)
                        {
                            int k4 = bcj;
                            j6 = k4 - arg1;
                        }
                        bil(bfh, -j6, k2 + arg1, 0, bbd, k7, l8);
                        k2 += l1;
                    }
                }

                return;
            }
            if (bbm)
            {
                for (arg0 = bfj; arg0 < bfk; arg0 += byte0)
                {
                    CameraVariable m2 = bfi[arg0];
                    arg1 = m2.Unknown1 >> 8;
                    int l4 = m2.Unknown2 >> 8;
                    int k6 = l4 - arg1;
                    if (k6 <= 0)
                    {
                        k2 += l1;
                    }
                    else
                    {
                        int l7 = m2.Unknown3;
                        int i9 = (m2.Unknown4 - l7) / k6;
                        if (arg1 < -bcj)
                        {
                            l7 += (-bcj - arg1) * i9;
                            arg1 = -bcj;
                            k6 = l4 - arg1;
                        }
                        if (l4 > bcj)
                        {
                            int i5 = bcj;
                            k6 = i5 - arg1;
                        }
                        bik(bfh, -k6, k2 + arg1, 0, bbd, l7, i9);
                        k2 += l1;
                    }
                }

                return;
            }
            for (arg0 = bfj; arg0 < bfk; arg0 += byte0)
            {
                CameraVariable m3 = bfi[arg0];
                arg1 = m3.Unknown1 >> 8;
                int j5 = m3.Unknown2 >> 8;
                int l6 = j5 - arg1;
                if (l6 <= 0)
                {
                    k2 += l1;
                }
                else
                {
                    int i8 = m3.Unknown3;
                    int j9 = (m3.Unknown4 - i8) / l6;
                    if (arg1 < -bcj)
                    {
                        i8 += (-bcj - arg1) * j9;
                        arg1 = -bcj;
                        l6 = j5 - arg1;
                    }
                    if (j5 > bcj)
                    {
                        int k5 = bcj;
                        l6 = k5 - arg1;
                    }
                    bim(bfh, -l6, k2 + arg1, 0, bbd, i8, j9);
                    k2 += l1;
                }
            }

        }

        static void bie(int[] arg0, int[] arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            int l1 = 0;
            if (arg6 != 0)
            {
                arg2 = arg4 / arg6 << 7;
                arg3 = arg5 / arg6 << 7;
            }
            if (arg2 < 0)
            {
                arg2 = 0;
            }
            else
                if (arg2 > 16256)
            {
                arg2 = 16256;
            }

            arg4 += arg7;
            arg5 += arg8;
            arg6 += arg9;
            if (arg6 != 0)
            {
                k = arg4 / arg6 << 7;
                i1 = arg5 / arg6 << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            int j1 = k - arg2 >> 4;
            int k1 = i1 - arg3 >> 4;
            for (int i2 = arg10 >> 4; i2 > 0; i2--)
            {
                arg2 += arg12 & 0x600000;
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = ((arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1);
                arg2 = k;
                arg3 = i1;
                arg4 += arg7;
                arg5 += arg8;
                arg6 += arg9;
                if (arg6 != 0)
                {
                    k = arg4 / arg6 << 7;
                    i1 = arg5 / arg6 << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                j1 = k - arg2 >> 4;
                k1 = i1 - arg3 >> 4;
            }

            for (int j2 = 0; j2 < (arg10 & 0xf); j2++)
            {
                if ((j2 & 3) == 0)
                {
                    arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                    l1 = arg12 >> 23;
                    arg12 += arg13;
                }
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)]) >> l1;
                arg2 += j1;
                arg3 += k1;
            }

        }

        static void bif(int[] arg0, int[] arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            int l1 = 0;
            if (arg6 != 0)
            {
                arg2 = arg4 / arg6 << 7;
                arg3 = arg5 / arg6 << 7;
            }
            if (arg2 < 0)
            {
                arg2 = 0;
            }
            else
                if (arg2 > 16256)
            {
                arg2 = 16256;
            }

            arg4 += arg7;
            arg5 += arg8;
            arg6 += arg9;
            if (arg6 != 0)
            {
                k = arg4 / arg6 << 7;
                i1 = arg5 / arg6 << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            int j1 = k - arg2 >> 4;
            int k1 = i1 - arg3 >> 4;
            for (int i2 = arg10 >> 4; i2 > 0; i2--)
            {
                arg2 += arg12 & 0x600000;
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                l1 = arg12 >> 23;
                arg12 += arg13;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 = k;
                arg3 = i1;
                arg4 += arg7;
                arg5 += arg8;
                arg6 += arg9;
                if (arg6 != 0)
                {
                    k = arg4 / arg6 << 7;
                    i1 = arg5 / arg6 << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                j1 = k - arg2 >> 4;
                k1 = i1 - arg3 >> 4;
            }

            for (int j2 = 0; j2 < (arg10 & 0xf); j2++)
            {
                if ((j2 & 3) == 0)
                {
                    arg2 = (arg2 & 0x3fff) + (arg12 & 0x600000);
                    l1 = arg12 >> 23;
                    arg12 += arg13;
                }
                arg0[arg11++] = (arg1[(arg3 & 0x3f80) + (arg2 >> 7)] >> l1) + (arg0[arg11] >> 1 & 0x7f7f7f);
                arg2 += j1;
                arg3 += k1;
            }

        }

        static void big(int[] arg0, int arg1, int arg2, int arg3, int[] arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14)
        {
            if (arg11 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg14 <<= 2;
            if (arg7 != 0)
            {
                k = arg5 / arg7 << 7;
                i1 = arg6 / arg7 << 7;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 16256)
            {
                k = 16256;
            }

            for (int l1 = arg11; l1 > 0; l1 -= 16)
            {
                arg5 += arg8;
                arg6 += arg9;
                arg7 += arg10;
                arg2 = k;
                arg3 = i1;
                if (arg7 != 0)
                {
                    k = arg5 / arg7 << 7;
                    i1 = arg6 / arg7 << 7;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 16256)
                {
                    k = 16256;
                }

                int j1 = k - arg2 >> 4;
                int k1 = i1 - arg3 >> 4;
                int i2 = arg13 >> 23;
                arg2 += arg13 & 0x600000;
                arg13 += arg14;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2++)
                    {
                        if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                        {
                            arg0[arg12] = arg1;
                        }

                        arg12++;
                        arg2 += j1;
                        arg3 += k1;
                        if ((j2 & 3) == 3)
                        {
                            arg2 = (arg2 & 0x3fff) + (arg13 & 0x600000);
                            i2 = arg13 >> 23;
                            arg13 += arg14;
                        }
                    }

                }
                else
                {
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0x3fff) + (arg13 & 0x600000);
                    i2 = arg13 >> 23;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0x3f80) + (arg2 >> 7)] >> i2)) != 0)
                    {
                        arg0[arg12] = arg1;
                    }

                    arg12++;
                }
            }

        }

        static void bih(int[] arg0, int[] arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg13 <<= 2;
            if (arg6 != 0)
            {
                k = arg4 / arg6 << 6;
                i1 = arg5 / arg6 << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = arg10; l1 > 0; l1 -= 16)
            {
                arg4 += arg7;
                arg5 += arg8;
                arg6 += arg9;
                arg2 = k;
                arg3 = i1;
                if (arg6 != 0)
                {
                    k = arg4 / arg6 << 6;
                    i1 = arg5 / arg6 << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - arg2 >> 4;
                int k1 = i1 - arg3 >> 4;
                int i2 = arg12 >> 20;
                arg2 += arg12 & 0xc0000;
                arg12 += arg13;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2++)
                    {
                        arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                        arg2 += j1;
                        arg3 += k1;
                        if ((j2 & 3) == 3)
                        {
                            arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                            i2 = arg12 >> 20;
                            arg12 += arg13;
                        }
                    }

                }
                else
                {
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2);
                }
            }

        }

        static void bii(int[] arg0, int[] arg1, int arg2, int arg3, int arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13)
        {
            if (arg10 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg13 <<= 2;
            if (arg6 != 0)
            {
                k = (arg4 / arg6) << 6;
                i1 = (arg5 / arg6) << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = arg10; l1 > 0; l1 -= 16)
            {
                arg4 += arg7;
                arg5 += arg8;
                arg6 += arg9;
                arg2 = k;
                arg3 = i1;
                if (arg6 != 0)
                {
                    k = arg4 / arg6 << 6;
                    i1 = arg5 / arg6 << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - arg2 >> 4;
                int k1 = i1 - arg3 >> 4;
                int i2 = arg12 >> 20;
                arg2 += arg12 & 0xc0000;
                arg12 += arg13;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2++)
                    {
                        arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                        arg2 += j1;
                        arg3 += k1;
                        if ((j2 & 3) == 3)
                        {
                            arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                            i2 = arg12 >> 20;
                            arg12 += arg13;
                        }
                    }

                }
                else
                {
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg12 & 0xc0000);
                    i2 = arg12 >> 20;
                    arg12 += arg13;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                    arg2 += j1;
                    arg3 += k1;
                    arg0[arg11++] = ((arg1[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) + (arg0[arg11] >> 1 & 0x7f7f7f);
                }
            }

        }

        static void bij(int[] pixels, int arg1, int arg2, int arg3, int[] arg4, int arg5, int arg6, int arg7,
                int arg8, int arg9, int arg10, int arg11, int arg12, int arg13, int arg14)
        {
            if (arg11 <= 0)
            {
                return;
            }

            int k = 0;
            int i1 = 0;
            arg14 <<= 2;
            if (arg7 != 0)
            {
                k = arg5 / arg7 << 6;
                i1 = arg6 / arg7 << 6;
            }
            if (k < 0)
            {
                k = 0;
            }
            else
                if (k > 4032)
            {
                k = 4032;
            }

            for (int l1 = arg11; l1 > 0; l1 -= 16)
            {
                arg5 += arg8;
                arg6 += arg9;
                arg7 += arg10;
                arg2 = k;
                arg3 = i1;
                if (arg7 != 0)
                {
                    k = arg5 / arg7 << 6;
                    i1 = arg6 / arg7 << 6;
                }
                if (k < 0)
                {
                    k = 0;
                }
                else
                    if (k > 4032)
                {
                    k = 4032;
                }

                int j1 = k - arg2 >> 4;
                int k1 = i1 - arg3 >> 4;
                int i2 = arg13 >> 20;
                arg2 += arg13 & 0xc0000;
                arg13 += arg14;
                if (l1 < 16)
                {
                    for (int j2 = 0; j2 < l1; j2++)
                    {
                        if ((arg1 = ((arg4[(arg3 & 0xfc0) + (arg2 >> 6)] >> i2))) != 0)
                        {
                            pixels[arg12] = arg1;
                        }

                        arg12++;
                        arg2 += j1;
                        arg3 += k1;
                        if ((j2 & 3) == 3)
                        {
                            arg2 = (arg2 & 0xfff) + (arg13 & 0xc0000);
                            i2 = arg13 >> 20;
                            arg13 += arg14;
                        }
                    }

                }
                else
                {
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    arg2 = (arg2 & 0xfff) + (arg13 & 0xc0000);
                    i2 = arg13 >> 20;
                    arg13 += arg14;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                    arg2 += j1;
                    arg3 += k1;
                    if ((arg1 = (arg4[(arg3 & 0xfc0) + (arg2 >> 6)]) >> i2) != 0)
                    {
                        pixels[arg12] = arg1;
                    }

                    arg12++;
                }
            }

        }

        static void bik(int[] pixels, int arg1, int arg2, int arg3, int[] arg4, int arg5, int arg6)
        {
            if (arg1 >= 0)
            {
                return;
            }

            arg6 <<= 1;
            arg3 = arg4[arg5 >> 8 & 0xff];
            arg5 += arg6;
            int k = arg1 / 8;
            for (int i1 = k; i1 < 0; i1++)
            {
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
            }

            k = -(arg1 % 8);
            for (int j1 = 0; j1 < k; j1++)
            {
                pixels[arg2++] = arg3;
                if ((j1 & 1) == 1)
                {
                    arg3 = arg4[arg5 >> 8 & 0xff];
                    arg5 += arg6;
                }
            }

        }

        static void bil(int[] pixels, int arg1, int arg2, int arg3, int[] arg4, int arg5, int arg6)
        {
            if (arg1 >= 0)
            {
                return;
            }

            arg6 <<= 2;
            arg3 = arg4[arg5 >> 8 & 0xff];
            arg5 += arg6;
            int k = arg1 / 16;
            for (int i1 = k; i1 < 0; i1++)
            {
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
            }

            k = -(arg1 % 16);
            for (int j1 = 0; j1 < k; j1++)
            {
                pixels[arg2++] = arg3 + (pixels[arg2] >> 1 & 0x7f7f7f);
                if ((j1 & 3) == 3)
                {
                    arg3 = arg4[arg5 >> 8 & 0xff];
                    arg5 += arg6;
                    arg5 += arg6;
                }
            }

        }

        static void bim(int[] pixels, int arg1, int arg2, int arg3, int[] arg4, int arg5, int arg6)
        {
            if (arg1 >= 0)
            {
                return;
            }

            arg6 <<= 2;
            arg3 = arg4[arg5 >> 8 & 0xff];
            arg5 += arg6;
            int k = arg1 / 16;
            for (int i1 = k; i1 < 0; i1++)
            {
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                pixels[arg2++] = arg3;
                arg3 = arg4[arg5 >> 8 & 0xff];
                arg5 += arg6;
            }

            k = (-(arg1 % 16));
            for (int j1 = 0; j1 < k; j1++)
            {
                pixels[arg2++] = arg3;
                if ((j1 & 3) == 3)
                {
                    arg3 = arg4[arg5 >> 8 & 0xff];
                    arg5 += arg6;
                }
            }

        }

        public void SetCameraTransform(Point3D location, int k1, int rotation, int i2, int distance)
        {
            k1 &= 0x3ff;
            rotation &= 0x3ff;
            i2 &= 0x3ff;
            bde = 1024 - k1 & 0x3ff;
            bdf = 1024 - rotation & 0x3ff;
            bdg = 1024 - i2 & 0x3ff;

            int xOffset = 0;
            int yOffset = 0;
            int zOffset = distance;

            if (k1 != 0)
            {
                int j3 = bbk[k1];
                int i4 = bbk[k1 + 1024];
                int l4 = yOffset * i4 - zOffset * j3 >> 15;
                zOffset = yOffset * j3 + zOffset * i4 >> 15;
                yOffset = l4;
            }
            if (rotation != 0)
            {
                int k3 = bbk[rotation];
                int j4 = bbk[rotation + 1024];
                int i5 = zOffset * k3 + xOffset * j4 >> 15;
                zOffset = zOffset * j4 - xOffset * k3 >> 15;
                xOffset = i5;
            }
            if (i2 != 0)
            {
                int l3 = bbk[i2];
                int k4 = bbk[i2 + 1024];
                int j5 = yOffset * l3 + xOffset * k4 >> 15;
                yOffset = yOffset * k4 - xOffset * l3 >> 15;
                xOffset = j5;
            }

            viewLocation = new Point3D(
                location.X - xOffset,
                location.Y - yOffset,
                location.Z - zOffset);
        }

        void bja(int arg0)
        {
            CameraModel l1 = visibleModels[arg0];
            ObjectModel k = l1.Object;
            int i1 = l1.faceVertCountIndex1;
            int[] ai = k.face_vertices[i1];
            int j1 = k.face_vertices_count[i1];
            int k1 = k.cgh[i1];
            int j2 = k.vertX[ai[0]];
            int k2 = k.vertY[ai[0]];
            int l2 = k.vertZ[ai[0]];
            int i3 = k.vertX[ai[1]] - j2;
            int j3 = k.vertY[ai[1]] - k2;
            int k3 = k.vertZ[ai[1]] - l2;
            int l3 = k.vertX[ai[2]] - j2;
            int i4 = k.vertY[ai[2]] - k2;
            int j4 = k.vertZ[ai[2]] - l2;
            int xDistance = j3 * j4 - i4 * k3;
            int yDistance = k3 * l3 - j4 * i3;
            int zDistance = i3 * i4 - l3 * j3;
            if (k1 == -1)
            {
                k1 = 0;
                for (; xDistance > 25000 || yDistance > 25000 || zDistance > 25000 || xDistance < -25000 || yDistance < -25000 || zDistance < -25000; zDistance >>= 1)
                {
                    k1++;
                    xDistance >>= 1;
                    yDistance >>= 1;
                }

                k.cgh[i1] = k1;
                k.cgg[i1] = (int)(bda * Math.Sqrt(xDistance * xDistance + yDistance * yDistance + zDistance * zDistance));
            }
            else
            {
                xDistance >>= k1;
                yDistance >>= k1;
                zDistance >>= k1;
            }
            l1.blb = j2 * xDistance + k2 * yDistance + l2 * zDistance;
            l1.bkm = xDistance;
            l1.bkn = yDistance;
            l1.bla = zDistance;
            int j5 = k.vertZ[ai[0]];
            int k5 = j5;
            int l5 = k.cfl[ai[0]];
            int i6 = l5;
            int j6 = k.cfm[ai[0]];
            int k6 = j6;
            for (int l6 = 1; l6 < j1; l6++)
            {
                int i2 = k.vertZ[ai[l6]];
                if (i2 > k5)
                {
                    k5 = i2;
                }
                else
                    if (i2 < j5)
                {
                    j5 = i2;
                }

                i2 = k.cfl[ai[l6]];
                if (i2 > i6)
                {
                    i6 = i2;
                }
                else
                    if (i2 < l5)
                {
                    l5 = i2;
                }

                i2 = k.cfm[ai[l6]];
                if (i2 > k6)
                {
                    k6 = i2;
                }
                else
                    if (i2 < j6)
                {
                    j6 = i2;
                }
            }

            l1.bkh = j5;
            l1.bki = k5;
            l1.bkd = l5;
            l1.bkf = i6;
            l1.bke = j6;
            l1.bkg = k6;
        }

        void bjb(int arg0)
        {
            CameraModel l1 = visibleModels[arg0];
            ObjectModel k = l1.Object;
            int i1 = l1.faceVertCountIndex1;
            int[] faceVertices = k.face_vertices[i1];
            int k1 = 0;
            int i2 = 0;
            int j2 = 1;
            int k2 = k.vertX[faceVertices[0]];
            int l2 = k.vertY[faceVertices[0]];
            int i3 = k.vertZ[faceVertices[0]];
            k.cgg[i1] = 1;
            k.cgh[i1] = 0;
            l1.blb = k2 * k1 + l2 * i2 + i3 * j2;
            l1.bkm = k1;
            l1.bkn = i2;
            l1.bla = j2;
            int j3 = k.vertZ[faceVertices[0]];
            int k3 = j3;
            int l3 = k.cfl[faceVertices[0]];
            int i4 = l3;
            if (k.cfl[faceVertices[1]] < l3)
            {
                l3 = k.cfl[faceVertices[1]];
            }
            else
            {
                i4 = k.cfl[faceVertices[1]];
            }

            int j4 = k.cfm[faceVertices[1]];
            int k4 = k.cfm[faceVertices[0]];
            int j1 = k.vertZ[faceVertices[1]];
            if (j1 > k3)
            {
                k3 = j1;
            }
            else
                if (j1 < j3)
            {
                j3 = j1;
            }

            j1 = k.cfl[faceVertices[1]];
            if (j1 > i4)
            {
                i4 = j1;
            }
            else
                if (j1 < l3)
            {
                l3 = j1;
            }

            j1 = k.cfm[faceVertices[1]];
            if (j1 > k4)
            {
                k4 = j1;
            }
            else
                if (j1 < j4)
            {
                j4 = j1;
            }

            l1.bkh = j3;
            l1.bki = k3;
            l1.bkd = l3 - 20;
            l1.bkf = i4 + 20;
            l1.bke = j4;
            l1.bkg = k4;
        }

        bool bjc(CameraModel arg0, CameraModel arg1)
        {
            if (arg0.bkd >= arg1.bkf)
            {
                return true;
            }

            if (arg1.bkd >= arg0.bkf)
            {
                return true;
            }

            if (arg0.bke >= arg1.bkg)
            {
                return true;
            }

            if (arg1.bke >= arg0.bkg)
            {
                return true;
            }

            if (arg0.bkh >= arg1.bki)
            {
                return true;
            }

            if (arg1.bkh > arg0.bki)
            {
                return false;
            }

            ObjectModel k = arg0.Object;
            ObjectModel i1 = arg1.Object;
            int j1 = arg0.faceVertCountIndex1;
            int k1 = arg1.faceVertCountIndex1;
            int[] ai = k.face_vertices[j1];
            int[] ai1 = i1.face_vertices[k1];
            int l1 = k.face_vertices_count[j1];
            int i2 = i1.face_vertices_count[k1];
            int l3 = i1.vertX[ai1[0]];
            int i4 = i1.vertY[ai1[0]];
            int j4 = i1.vertZ[ai1[0]];
            int k4 = arg1.bkm;
            int l4 = arg1.bkn;
            int i5 = arg1.bla;
            int j5 = i1.cgg[k1];
            int k5 = arg1.blb;
            bool flag = false;
            for (int l5 = 0; l5 < l1; l5++)
            {
                int j2 = ai[l5];
                int j3 = (l3 - k.vertX[j2]) * k4 + (i4 - k.vertY[j2]) * l4 + (j4 - k.vertZ[j2]) * i5;
                if ((j3 >= -j5 || k5 >= 0) && (j3 <= j5 || k5 <= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            l3 = k.vertX[ai[0]];
            i4 = k.vertY[ai[0]];
            j4 = k.vertZ[ai[0]];
            k4 = arg0.bkm;
            l4 = arg0.bkn;
            i5 = arg0.bla;
            j5 = k.cgg[j1];
            k5 = arg0.blb;
            flag = false;
            for (int i6 = 0; i6 < i2; i6++)
            {
                int k2 = ai1[i6];
                int k3 = (l3 - i1.vertX[k2]) * k4 + (i4 - i1.vertY[k2]) * l4 + (j4 - i1.vertZ[k2]) * i5;
                if ((k3 >= -j5 || k5 <= 0) && (k3 <= j5 || k5 >= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            int[] ai2;
            int[] ai3;
            if (l1 == 2)
            {
                ai2 = new int[4];
                ai3 = new int[4];
                int j6 = ai[0];
                int l2 = ai[1];
                ai2[0] = k.cfl[j6] - 20;
                ai2[1] = k.cfl[l2] - 20;
                ai2[2] = k.cfl[l2] + 20;
                ai2[3] = k.cfl[j6] + 20;
                ai3[0] = ai3[3] = k.cfm[j6];
                ai3[1] = ai3[2] = k.cfm[l2];
            }
            else
            {
                ai2 = new int[l1];
                ai3 = new int[l1];
                for (int k6 = 0; k6 < l1; k6++)
                {
                    int j7 = ai[k6];
                    ai2[k6] = k.cfl[j7];
                    ai3[k6] = k.cfm[j7];
                }

            }
            int[] ai4;
            int[] ai5;
            if (i2 == 2)
            {
                ai4 = new int[4];
                ai5 = new int[4];
                int l6 = ai1[0];
                int i3 = ai1[1];
                ai4[0] = i1.cfl[l6] - 20;
                ai4[1] = i1.cfl[i3] - 20;
                ai4[2] = i1.cfl[i3] + 20;
                ai4[3] = i1.cfl[l6] + 20;
                ai5[0] = ai5[3] = i1.cfm[l6];
                ai5[1] = ai5[2] = i1.cfm[i3];
            }
            else
            {
                ai4 = new int[i2];
                ai5 = new int[i2];
                for (int i7 = 0; i7 < i2; i7++)
                {
                    int k7 = ai1[i7];
                    ai4[i7] = i1.cfl[k7];
                    ai5[i7] = i1.cfm[k7];
                }

            }
            return !bkc(ai2, ai3, ai4, ai5);
        }

        bool bjd(CameraModel arg0, CameraModel arg1)
        {
            ObjectModel k = arg0.Object;
            ObjectModel i1 = arg1.Object;
            int j1 = arg0.faceVertCountIndex1;
            int k1 = arg1.faceVertCountIndex1;
            int[] ai = k.face_vertices[j1];
            int[] ai1 = i1.face_vertices[k1];
            int l1 = k.face_vertices_count[j1];
            int i2 = i1.face_vertices_count[k1];
            int j3 = i1.vertX[ai1[0]];
            int k3 = i1.vertY[ai1[0]];
            int l3 = i1.vertZ[ai1[0]];
            int i4 = arg1.bkm;
            int j4 = arg1.bkn;
            int k4 = arg1.bla;
            int l4 = i1.cgg[k1];
            int i5 = arg1.blb;
            bool flag = false;
            for (int j5 = 0; j5 < l1; j5++)
            {
                int j2 = ai[j5];
                int l2 = (j3 - k.vertX[j2]) * i4 + (k3 - k.vertY[j2]) * j4 + (l3 - k.vertZ[j2]) * k4;
                if ((l2 >= -l4 || i5 >= 0) && (l2 <= l4 || i5 <= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            if (!flag)
            {
                return true;
            }

            j3 = k.vertX[ai[0]];
            k3 = k.vertY[ai[0]];
            l3 = k.vertZ[ai[0]];
            i4 = arg0.bkm;
            j4 = arg0.bkn;
            k4 = arg0.bla;
            l4 = k.cgg[j1];
            i5 = arg0.blb;
            flag = false;
            for (int k5 = 0; k5 < i2; k5++)
            {
                int k2 = ai1[k5];
                int i3 = (j3 - i1.vertX[k2]) * i4 + (k3 - i1.vertY[k2]) * j4 + (l3 - i1.vertZ[k2]) * k4;
                if ((i3 >= -l4 || i5 <= 0) && (i3 <= l4 || i5 >= 0))
                {
                    continue;
                }

                flag = true;
                break;
            }

            return !flag;
        }

        public void createTexture(int k, int i1, int j1)
        {
            bei = k;
            texturePictureColorIndex = new sbyte[k][];
            texturePictureColorArray = new int[k][];
            bel = new int[k];
            bem = new long[k];
            bfa = new bool[k];
            objectTexturePixels = new int[k][];
            bfb = 0L;
            texturePixels = new int[i1][];
            bfd = new int[j1][];
        }

        public void setTexture(int k, sbyte[] abyte0, int[] ai, int i1)
        {
            texturePictureColorIndex[k] = abyte0;
            texturePictureColorArray[k] = ai;
            bel[k] = i1;
            bem[k] = 0L;
            bfa[k] = false;
            objectTexturePixels[k] = null;
            updateTextureSmoothing(k);
        }

        public void updateTextureSmoothing(int arg0)
        {
            if (arg0 < 0)
            {
                return;
            }

            bem[arg0] = bfb++;
            if (objectTexturePixels[arg0] != null)
            {
                return;
            }

            if (bel[arg0] == 0)
            {
                for (int k = 0; k < texturePixels.Length; k++)
                {
                    if (texturePixels[k] == null)
                    {
                        texturePixels[k] = new int[16384];
                        objectTexturePixels[arg0] = texturePixels[k];
                        applyTexture(arg0);
                        return;
                    }
                }

                long l1 = 1L << 30;
                int j1 = 0;
                for (int i2 = 0; i2 < bei; i2++)
                {
                    if (i2 != arg0 && bel[i2] == 0 && objectTexturePixels[i2] != null && bem[i2] < l1)
                    {
                        l1 = bem[i2];
                        j1 = i2;
                    }
                }

                objectTexturePixels[arg0] = objectTexturePixels[j1];
                objectTexturePixels[j1] = null;
                applyTexture(arg0);
                return;
            }
            for (int i1 = 0; i1 < bfd.Length; i1++)
            {
                if (bfd[i1] == null)
                {
                    bfd[i1] = new int[0x10000];
                    objectTexturePixels[arg0] = bfd[i1];
                    applyTexture(arg0);
                    return;
                }
            }

            long l2 = 1L << 30;
            int k1 = 0;
            for (int j2 = 0; j2 < bei; j2++)
            {
                if (j2 != arg0 && bel[j2] == 1 && objectTexturePixels[j2] != null && bem[j2] < l2)
                {
                    l2 = bem[j2];
                    k1 = j2;
                }
            }

            objectTexturePixels[arg0] = objectTexturePixels[k1];
            objectTexturePixels[k1] = null;
            applyTexture(arg0);
        }

        void applyTexture(int arg0)
        {
            int textureSize;
            if (bel[arg0] == 0)
            {
                textureSize = 64;
            }
            else
            {
                textureSize = 128;//'\200';
            }

            int[] texture = objectTexturePixels[arg0];
            int pixelCount = 0;
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    int pixel = texturePictureColorArray[arg0][texturePictureColorIndex[arg0][y + x * textureSize] & 0xff];
                    pixel &= 0xf8f8ff;
                    if (pixel == 0)
                    {
                        pixel = 1;
                    }
                    else if (pixel == 0xf800ff)
                    {
                        pixel = 0;
                        bfa[arg0] = true;
                    }
                    texture[pixelCount++] = pixel;
                }

            }
            // blend objects with correct lightning?
            for (int pixel = 0; pixel < pixelCount; pixel++)
            {
                int i2 = texture[pixel];
                texture[pixelCount + pixel] = i2 - (i2 >> 3) & 0xf8f8ff;
                texture[pixelCount * 2 + pixel] = i2 - (i2 >> 2) & 0xf8f8ff;
                texture[pixelCount * 3 + pixel] = i2 - (i2 >> 2) - (i2 >> 3) & 0xf8f8ff;
            }

        }

        /// <summary>
        /// ?????? No idea if the name convention for this one is correct...
        /// </summary>
        /// <param name="arg0"></param>
        public void updateLightning(int arg0)
        {
            if (objectTexturePixels[arg0] == null)
            {
                return;
            }

            int[] objLight = objectTexturePixels[arg0];
            for (int k = 0; k < 64; k++)
            {
                int i1 = k + 4032;
                int j1 = objLight[i1];
                for (int l1 = 0; l1 < 63; l1++)
                {
                    objLight[i1] = objLight[i1 - 64];
                    i1 -= 64;
                }

                objectTexturePixels[arg0][i1] = j1;
            }

            int c = 4096;
            for (int k1 = 0; k1 < c; k1++)
            {
                int i2 = objLight[k1];
                objLight[c + k1] = i2 - (i2 >> 3) & 0xf8f8ff;
                objLight[c * 2 + k1] = i2 - (i2 >> 2) & 0xf8f8ff;
                objLight[c * 3 + k1] = i2 - (i2 >> 2) - (i2 >> 3) & 0xf8f8ff;
            }

        }

        public int applyTextureSmoothing(int index)
        {
            if (index == 0xbc614e)
            {
                return 0;
            }

            updateTextureSmoothing(index);
            if (index >= 0)
            {
                return objectTexturePixels[index][0];
            }

            if (index < 0)
            {
                index = -(index + 1);
                int i1 = index >> 10 & 0x1f;
                int j1 = index >> 5 & 0x1f;
                int k1 = index & 0x1f;
                return (i1 << 19) + (j1 << 11) + (k1 << 3);
            }
            else
            {
                return 0;
            }
        }

        public void bjk(Point3D point)
        {
            if (point == Point3D.Empty)
            {
                point.X = 32;
            }

            for (int objectIndex = 0; objectIndex < currentObjectCount; objectIndex++)
            {
                objectCache[objectIndex].cmg(point);
            }
        }

        public void bjl(int arg0, int arg1, Point3D location)
        {
            if (location == Point3D.Empty)
            {
                location.X = 32;
            }

            for (int objectCacheIndex = 0; objectCacheIndex < currentObjectCount; objectCacheIndex++)
            {
                objectCache[objectCacheIndex].cmf(arg0, arg1, location);
            }
        }

        public static int getTextureColor(int r, int g, int b)
        {
            return -1 - (r / 8) * 1024 - (g / 8) * 32 - b / 8;
        }

        public int bjn(int k, int i1, int j1, int k1, int l1)
        {
            if (k1 == i1)
            {
                return k;
            }
            else
            {
                return k + ((j1 - k) * (l1 - i1)) / (k1 - i1);
            }
        }

        public bool bka(int k, int i1, int j1, int k1, bool flag)
        {
            if (flag && k <= j1 || k < j1)
            {
                if (k > k1)
                {
                    return true;
                }

                if (i1 > j1)
                {
                    return true;
                }

                if (i1 > k1)
                {
                    return true;
                }

                return !flag;
            }
            if (k < k1)
            {
                return true;
            }

            if (i1 < j1)
            {
                return true;
            }

            if (i1 < k1)
            {
                return true;
            }
            else
            {
                return flag;
            }
        }

        public bool bkb(int k, int i1, int j1, bool flag)
        {
            if (flag && k <= j1 || k < j1)
            {
                if (i1 > j1)
                {
                    return true;
                }

                return !flag;
            }
            if (i1 < j1)
            {
                return true;
            }
            else
            {
                return flag;
            }
        }

        public bool bkc(int[] arg0, int[] arg1, int[] arg2, int[] arg3)
        {
            int k = arg0.Length;
            int i1 = arg2.Length; ;
            byte byte0 = 0;
            int l20;
            int j21 = l20 = arg1[0];
            int j1 = 0;
            int i21;
            int k21 = i21 = arg3[0];
            int l1 = 0;
            for (int l21 = 1; l21 < k; l21++)
            {
                if (arg1[l21] < l20)
                {
                    l20 = arg1[l21];
                    j1 = l21;
                }
                else
                    if (arg1[l21] > j21)
                {
                    j21 = arg1[l21];
                }
            }

            for (int i22 = 1; i22 < i1; i22++)
            {
                if (arg3[i22] < i21)
                {
                    i21 = arg3[i22];
                    l1 = i22;
                }
                else
                    if (arg3[i22] > k21)
                {
                    k21 = arg3[i22];
                }
            }

            if (i21 >= j21)
            {
                return false;
            }

            if (l20 >= k21)
            {
                return false;
            }

            int k1;
            int i2;
            bool flag;
            if (arg1[j1] < arg3[l1])
            {
                for (k1 = j1; arg1[k1] < arg3[l1]; k1 = (k1 + 1) % k)
                {
                    ;
                }

                for (; arg1[j1] < arg3[l1]; j1 = ((j1 - 1) + k) % k)
                {
                    ;
                }

                int j2 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
                int j7 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
                int k11 = arg2[l1];
                flag = (j2 < k11) | (j7 < k11);
                if (bkb(j2, j7, k11, flag))
                {
                    return true;
                }

                i2 = (l1 + 1) % i1;
                l1 = ((l1 - 1) + i1) % i1;
                if (j1 == k1)
                {
                    byte0 = 1;
                }
            }
            else
            {
                for (i2 = l1; arg3[i2] < arg1[j1]; i2 = (i2 + 1) % i1)
                {
                    ;
                }

                for (; arg3[l1] < arg1[j1]; l1 = ((l1 - 1) + i1) % i1)
                {
                    ;
                }

                int k2 = arg0[j1];
                int l11 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[j1]);
                int k16 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[j1]);
                flag = (k2 < l11) | (k2 < k16);
                if (bkb(l11, k16, k2, !flag))
                {
                    return true;
                }

                k1 = (j1 + 1) % k;
                j1 = ((j1 - 1) + k) % k;
                if (l1 == i2)
                {
                    byte0 = 2;
                }
            }
            while (byte0 == 0)
            {
                if (arg1[j1] < arg1[k1])
                {
                    if (arg1[j1] < arg3[l1])
                    {
                        if (arg1[j1] < arg3[i2])
                        {
                            int l2 = arg0[j1];
                            int k7 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg1[j1]);
                            int i12 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[j1]);
                            int l16 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[j1]);
                            if (bka(l2, k7, i12, l16, flag))
                            {
                                return true;
                            }

                            j1 = ((j1 - 1) + k) % k;
                            if (j1 == k1)
                            {
                                byte0 = 1;
                            }
                        }
                        else
                        {
                            int i3 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                            int l7 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                            int j12 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                            int i17 = arg2[i2];
                            if (bka(i3, l7, j12, i17, flag))
                            {
                                return true;
                            }

                            i2 = (i2 + 1) % i1;
                            if (l1 == i2)
                            {
                                byte0 = 2;
                            }
                        }
                    }
                    else
                        if (arg3[l1] < arg3[i2])
                    {
                        int j3 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
                        int i8 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
                        int k12 = arg2[l1];
                        int j17 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg3[l1]);
                        if (bka(j3, i8, k12, j17, flag))
                        {
                            return true;
                        }

                        l1 = ((l1 - 1) + i1) % i1;
                        if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                    else
                    {
                        int k3 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                        int j8 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                        int l12 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                        int k17 = arg2[i2];
                        if (bka(k3, j8, l12, k17, flag))
                        {
                            return true;
                        }

                        i2 = (i2 + 1) % i1;
                        if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                }
                else
                    if (arg1[k1] < arg3[l1])
                {
                    if (arg1[k1] < arg3[i2])
                    {
                        int l3 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg1[k1]);
                        int k8 = arg0[k1];
                        int i13 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[k1]);
                        int l17 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[k1]);
                        if (bka(l3, k8, i13, l17, flag))
                        {
                            return true;
                        }

                        k1 = (k1 + 1) % k;
                        if (j1 == k1)
                        {
                            byte0 = 1;
                        }
                    }
                    else
                    {
                        int i4 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                        int l8 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                        int j13 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                        int i18 = arg2[i2];
                        if (bka(i4, l8, j13, i18, flag))
                        {
                            return true;
                        }

                        i2 = (i2 + 1) % i1;
                        if (l1 == i2)
                        {
                            byte0 = 2;
                        }
                    }
                }
                else
                        if (arg3[l1] < arg3[i2])
                {
                    int j4 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
                    int i9 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
                    int k13 = arg2[l1];
                    int j18 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg3[l1]);
                    if (bka(j4, i9, k13, j18, flag))
                    {
                        return true;
                    }

                    l1 = ((l1 - 1) + i1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 2;
                    }
                }
                else
                {
                    int k4 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                    int j9 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                    int l13 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                    int k18 = arg2[i2];
                    if (bka(k4, j9, l13, k18, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 2;
                    }
                }
            }

            while (byte0 == 1)
            {
                if (arg1[j1] < arg3[l1])
                {
                    if (arg1[j1] < arg3[i2])
                    {
                        int l4 = arg0[j1];
                        int i14 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[j1]);
                        int l18 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[j1]);
                        return bkb(i14, l18, l4, !flag);
                    }
                    int i5 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                    int k9 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                    int j14 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                    int i19 = arg2[i2];
                    if (bka(i5, k9, j14, i19, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
                else
                    if (arg3[l1] < arg3[i2])
                {
                    int j5 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
                    int l9 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
                    int k14 = arg2[l1];
                    int j19 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg3[l1]);
                    if (bka(j5, l9, k14, j19, flag))
                    {
                        return true;
                    }

                    l1 = ((l1 - 1) + i1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
                else
                {
                    int k5 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[i2]);
                    int i10 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[i2]);
                    int l14 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg3[i2]);
                    int k19 = arg2[i2];
                    if (bka(k5, i10, l14, k19, flag))
                    {
                        return true;
                    }

                    i2 = (i2 + 1) % i1;
                    if (l1 == i2)
                    {
                        byte0 = 0;
                    }
                }
            }

            while (byte0 == 2)
            {
                if (arg3[l1] < arg1[j1])
                {
                    if (arg3[l1] < arg1[k1])
                    {
                        int l5 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
                        int j10 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
                        int i15 = arg2[l1];
                        return bkb(l5, j10, i15, flag);
                    }
                    int i6 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg1[k1]);
                    int k10 = arg0[k1];
                    int j15 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[k1]);
                    int l19 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[k1]);
                    if (bka(i6, k10, j15, l19, flag))
                    {
                        return true;
                    }

                    k1 = (k1 + 1) % k;
                    if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
                else
                    if (arg1[j1] < arg1[k1])
                {
                    int j6 = arg0[j1];
                    int l10 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg1[j1]);
                    int k15 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[j1]);
                    int i20 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[j1]);
                    if (bka(j6, l10, k15, i20, flag))
                    {
                        return true;
                    }

                    j1 = ((j1 - 1) + k) % k;
                    if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
                else
                {
                    int k6 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg1[k1]);
                    int i11 = arg0[k1];
                    int l15 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[k1]);
                    int j20 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[k1]);
                    if (bka(k6, i11, l15, j20, flag))
                    {
                        return true;
                    }

                    k1 = (k1 + 1) % k;
                    if (j1 == k1)
                    {
                        byte0 = 0;
                    }
                }
            }

            if (arg1[j1] < arg3[l1])
            {
                int l6 = arg0[j1];
                int i16 = bjn(arg2[(l1 + 1) % i1], arg3[(l1 + 1) % i1], arg2[l1], arg3[l1], arg1[j1]);
                int k20 = bjn(arg2[((i2 - 1) + i1) % i1], arg3[((i2 - 1) + i1) % i1], arg2[i2], arg3[i2], arg1[j1]);
                return bkb(i16, k20, l6, !flag);
            }
            int i7 = bjn(arg0[(j1 + 1) % k], arg1[(j1 + 1) % k], arg0[j1], arg1[j1], arg3[l1]);
            int j11 = bjn(arg0[((k1 - 1) + k) % k], arg1[((k1 - 1) + k) % k], arg0[k1], arg1[k1], arg3[l1]);
            int j16 = arg2[l1];
            return bkb(i7, j11, j16, flag);
        }

        public int bba;
        public int[] bbb;
        public int[][] bbc;
        public int[] bbd;
        public int bbe;
        public int bbf;
        public int zoom1;
        public int zoom2;
        public int zoom3;
        public int zoom4;
        public static int[] bbk = new int[2048];
        static int[] bbl = new int[512];
        public bool bbm;
        public double bbn;
        public int bca;
        bool bcb;
        int bcc;
        int bcd;
        int bce;
        int maxHighlightedObjects;
        ObjectModel[] _highlightedObjects;
        int[] bch;
        int bci;
        int bcj;
        int bck;
        int bcl;
        int bcm;
        int bcn;
        int bda;
        int bde;
        int bdf;
        int bdg;
        public int currentObjectCount;
        public int totalModelCount;
        public ObjectModel[] objectCache;
        int[] bdk;
        int currentModelIndex;
        CameraModel[] visibleModels;
        int bdn;
        int[] sceneObjectId;
        int[] bee;
        int[] bef;
        int[] beg;
        public ObjectModel highlightedObject;
        public int bei;
        public sbyte[][] texturePictureColorIndex;
        public int[][] texturePictureColorArray;
        public int[] bel;
        public long[] bem;
        public int[][] objectTexturePixels;
        public bool[] bfa;
        static long bfb;
        public int[][] texturePixels;
        public int[][] bfd;
        static sbyte[] bfe;
        public GraphicsEngine gameImage;
        public int[] bfh;
        public CameraVariable[] bfi;
        public int bfj;
        public int bfk;
        public int[] bfl;
        public int[] bfm;
        public int[] bfn;
        public int bgk;
        public int bgl;
    }
}
