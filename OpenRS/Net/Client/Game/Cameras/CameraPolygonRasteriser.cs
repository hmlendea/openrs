namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraPolygonRasteriser(CameraSceneObjectTracker sceneObjectTracker)
    {
        private readonly CameraPolygonHitTester hitTester =
            new(sceneObjectTracker);

        public CameraVariable[] ScanlineVariables { get; private set; }

        public int MinVisibleScanline { get; set; }

        public int MaxVisibleScanline { get; set; }

        private int scanlineBufferCentre;
        private int screenCentreY;

        private static int ScanlineLeftXSentinel => 0xa0000;

        private static int ScanlineRightXSentinel => unchecked((int)0xfff60000);

        public void Initialise(int scanlineCount, int scanlineBufferCentre, int screenCentreY)
        {
            this.scanlineBufferCentre = scanlineBufferCentre;
            this.screenCentreY = screenCentreY;
            ScanlineVariables = new CameraVariable[scanlineCount];

            for (int index = 0; index < scanlineCount; index += 1)
            {
                ScanlineVariables[index] = new CameraVariable();
            }
        }

        public void Rasterise(
            int vertexCount,
            int[] polygonX,
            int[] polygonY,
            int[] shadeLevels,
            GameObject gameObject,
            int faceVertexIndex)
        {
            if (vertexCount == 3)
            {
                RenderTrianglePolygon(polygonX, polygonY, shadeLevels);
            }
            else if (vertexCount == 4)
            {
                RenderQuadPolygon(polygonX, polygonY, shadeLevels);
            }
            else if (!RenderGeneralPolygon(vertexCount, polygonX, polygonY, shadeLevels))
            {
                return;
            }

            hitTester.Check(
                gameObject,
                faceVertexIndex,
                ScanlineVariables,
                MinVisibleScanline,
                MaxVisibleScanline);
        }

        private void RenderTrianglePolygon(int[] polygonX, int[] polygonY, int[] shadeLevels)
        {
            int vertX0 = polygonX[0];
            int vertX1 = polygonX[1];
            int vertX2 = polygonX[2];
            int vertShade0 = shadeLevels[0];
            int vertShade1 = shadeLevels[1];
            int vertShade2 = shadeLevels[2];
            int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
            CameraPolygonEdge edgeA = CameraPolygonEdge.Calculate(
                vertX0,
                polygonY[0],
                vertShade0,
                vertX2,
                polygonY[2],
                vertShade2,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeAX = edgeA.CurrentX;
            int edgeAXSlope = edgeA.XSlope;
            int edgeAShade = edgeA.CurrentShade;
            int edgeAShadeSlope = edgeA.ShadeSlope;
            int edgeAMinY = edgeA.MinimumY;
            int edgeAMaxY = edgeA.MaximumY;
            CameraPolygonEdge edgeB = CameraPolygonEdge.Calculate(
                vertX0,
                polygonY[0],
                vertShade0,
                vertX1,
                polygonY[1],
                vertShade1,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeBX = edgeB.CurrentX;
            int edgeBXSlope = edgeB.XSlope;
            int edgeBShade = edgeB.CurrentShade;
            int edgeBShadeSlope = edgeB.ShadeSlope;
            int edgeBMinY = edgeB.MinimumY;
            int edgeBMaxY = edgeB.MaximumY;
            CameraPolygonEdge edgeC = CameraPolygonEdge.Calculate(
                vertX1,
                polygonY[1],
                vertShade1,
                vertX2,
                polygonY[2],
                vertShade2,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeCX = edgeC.CurrentX;
            int edgeCXSlope = edgeC.XSlope;
            int edgeCShade = edgeC.CurrentShade;
            int edgeCShadeSlope = edgeC.ShadeSlope;
            int edgeCMinY = edgeC.MinimumY;
            int edgeCMaxY = edgeC.MaximumY;

            MinVisibleScanline = edgeAMinY;

            if (edgeBMinY < MinVisibleScanline)
            {
                MinVisibleScanline = edgeBMinY;
            }

            if (edgeCMinY < MinVisibleScanline)
            {
                MinVisibleScanline = edgeCMinY;
            }

            MaxVisibleScanline = edgeAMaxY;

            if (edgeBMaxY > MaxVisibleScanline)
            {
                MaxVisibleScanline = edgeBMaxY;
            }

            if (edgeCMaxY > MaxVisibleScanline)
            {
                MaxVisibleScanline = edgeCMaxY;
            }

            int scanlineLeftX = 0;
            int scanlineRightX = 0;
            int scanlineLeftShade = 0;
            int scanlineRightShade = 0;

            for (int scanlineY = MinVisibleScanline; scanlineY < MaxVisibleScanline; scanlineY += 1)
            {
                if (scanlineY >= edgeAMinY && scanlineY < edgeAMaxY)
                {
                    scanlineLeftX = scanlineRightX = edgeAX;
                    scanlineLeftShade = scanlineRightShade = edgeAShade;
                    edgeAX += edgeAXSlope;
                    edgeAShade += edgeAShadeSlope;
                }
                else
                {
                    scanlineLeftX = ScanlineLeftXSentinel;
                    scanlineRightX = ScanlineRightXSentinel;
                }

                if (scanlineY >= edgeBMinY && scanlineY < edgeBMaxY)
                {
                    if (edgeBX < scanlineLeftX)
                    {
                        scanlineLeftX = edgeBX;
                        scanlineLeftShade = edgeBShade;
                    }

                    if (edgeBX > scanlineRightX)
                    {
                        scanlineRightX = edgeBX;
                        scanlineRightShade = edgeBShade;
                    }

                    edgeBX += edgeBXSlope;
                    edgeBShade += edgeBShadeSlope;
                }

                if (scanlineY >= edgeCMinY && scanlineY < edgeCMaxY)
                {
                    if (edgeCX < scanlineLeftX)
                    {
                        scanlineLeftX = edgeCX;
                        scanlineLeftShade = edgeCShade;
                    }

                    if (edgeCX > scanlineRightX)
                    {
                        scanlineRightX = edgeCX;
                        scanlineRightShade = edgeCShade;
                    }

                    edgeCX += edgeCXSlope;
                    edgeCShade += edgeCShadeSlope;
                }

                CameraVariable scanline = ScanlineVariables[scanlineY];
                scanline.LeftX = scanlineLeftX;
                scanline.RightX = scanlineRightX;
                scanline.LeftShade = scanlineLeftShade;
                scanline.RightShade = scanlineRightShade;
            }

            if (MinVisibleScanline < scanlineBufferCentre - screenCentreY)
            {
                MinVisibleScanline = scanlineBufferCentre - screenCentreY;
            }
        }

        private void RenderQuadPolygon(int[] polygonX, int[] polygonY, int[] shadeLevels)
        {
            int vertX0 = polygonX[0];
            int vertX1 = polygonX[1];
            int vertX2 = polygonX[2];
            int vertX3 = polygonX[3];
            int vertShade0 = shadeLevels[0];
            int vertShade1 = shadeLevels[1];
            int vertShade2 = shadeLevels[2];
            int vertShade3 = shadeLevels[3];
            int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
            CameraPolygonEdge edgeA = CameraPolygonEdge.Calculate(
                vertX0,
                polygonY[0],
                vertShade0,
                vertX3,
                polygonY[3],
                vertShade3,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeAX = edgeA.CurrentX;
            int edgeAXSlope = edgeA.XSlope;
            int edgeAShade = edgeA.CurrentShade;
            int edgeAShadeSlope = edgeA.ShadeSlope;
            int edgeAMinY = edgeA.MinimumY;
            int edgeAMaxY = edgeA.MaximumY;

            CameraPolygonEdge edgeB = CameraPolygonEdge.Calculate(
                vertX0,
                polygonY[0],
                vertShade0,
                vertX1,
                polygonY[1],
                vertShade1,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeBX = edgeB.CurrentX;
            int edgeBXSlope = edgeB.XSlope;
            int edgeBShade = edgeB.CurrentShade;
            int edgeBShadeSlope = edgeB.ShadeSlope;
            int edgeBMinY = edgeB.MinimumY;
            int edgeBMaxY = edgeB.MaximumY;

            CameraPolygonEdge edgeC = CameraPolygonEdge.Calculate(
                vertX1,
                polygonY[1],
                vertShade1,
                vertX2,
                polygonY[2],
                vertShade2,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeCX = edgeC.CurrentX;
            int edgeCXSlope = edgeC.XSlope;
            int edgeCShade = edgeC.CurrentShade;
            int edgeCShadeSlope = edgeC.ShadeSlope;
            int edgeCMinY = edgeC.MinimumY;
            int edgeCMaxY = edgeC.MaximumY;

            CameraPolygonEdge edgeD = CameraPolygonEdge.Calculate(
                vertX2,
                polygonY[2],
                vertShade2,
                vertX3,
                polygonY[3],
                vertShade3,
                scanlineBufferCentre,
                scanlineLimit);
            int edgeDX = edgeD.CurrentX;
            int edgeDXSlope = edgeD.XSlope;
            int edgeDShade = edgeD.CurrentShade;
            int edgeDShadeSlope = edgeD.ShadeSlope;
            int edgeDMinY = edgeD.MinimumY;
            int edgeDMaxY = edgeD.MaximumY;

            MinVisibleScanline = edgeAMinY;

            if (edgeBMinY < MinVisibleScanline)
            {
                MinVisibleScanline = edgeBMinY;
            }

            if (edgeCMinY < MinVisibleScanline)
            {
                MinVisibleScanline = edgeCMinY;
            }

            if (edgeDMinY < MinVisibleScanline)
            {
                MinVisibleScanline = edgeDMinY;
            }

            MaxVisibleScanline = edgeAMaxY;

            if (edgeBMaxY > MaxVisibleScanline)
            {
                MaxVisibleScanline = edgeBMaxY;
            }

            if (edgeCMaxY > MaxVisibleScanline)
            {
                MaxVisibleScanline = edgeCMaxY;
            }

            if (edgeDMaxY > MaxVisibleScanline)
            {
                MaxVisibleScanline = edgeDMaxY;
            }

            int scanlineLeftX = 0;
            int scanlineRightX = 0;
            int scanlineLeftShade = 0;
            int scanlineRightShade = 0;

            for (int scanlineY = MinVisibleScanline; scanlineY < MaxVisibleScanline; scanlineY += 1)
            {
                if (scanlineY >= edgeAMinY && scanlineY < edgeAMaxY)
                {
                    scanlineLeftX = scanlineRightX = edgeAX;
                    scanlineLeftShade = scanlineRightShade = edgeAShade;
                    edgeAX += edgeAXSlope;
                    edgeAShade += edgeAShadeSlope;
                }
                else
                {
                    scanlineLeftX = ScanlineLeftXSentinel;
                    scanlineRightX = ScanlineRightXSentinel;
                }

                if (scanlineY >= edgeBMinY && scanlineY < edgeBMaxY)
                {
                    if (edgeBX < scanlineLeftX)
                    {
                        scanlineLeftX = edgeBX;
                        scanlineLeftShade = edgeBShade;
                    }

                    if (edgeBX > scanlineRightX)
                    {
                        scanlineRightX = edgeBX;
                        scanlineRightShade = edgeBShade;
                    }

                    edgeBX += edgeBXSlope;
                    edgeBShade += edgeBShadeSlope;
                }

                if (scanlineY >= edgeCMinY && scanlineY < edgeCMaxY)
                {
                    if (edgeCX < scanlineLeftX)
                    {
                        scanlineLeftX = edgeCX;
                        scanlineLeftShade = edgeCShade;
                    }

                    if (edgeCX > scanlineRightX)
                    {
                        scanlineRightX = edgeCX;
                        scanlineRightShade = edgeCShade;
                    }

                    edgeCX += edgeCXSlope;
                    edgeCShade += edgeCShadeSlope;
                }

                if (scanlineY >= edgeDMinY && scanlineY < edgeDMaxY)
                {
                    if (edgeDX < scanlineLeftX)
                    {
                        scanlineLeftX = edgeDX;
                        scanlineLeftShade = edgeDShade;
                    }

                    if (edgeDX > scanlineRightX)
                    {
                        scanlineRightX = edgeDX;
                        scanlineRightShade = edgeDShade;
                    }

                    edgeDX += edgeDXSlope;
                    edgeDShade += edgeDShadeSlope;
                }

                CameraVariable scanline = ScanlineVariables[scanlineY];
                scanline.LeftX = scanlineLeftX;
                scanline.RightX = scanlineRightX;
                scanline.LeftShade = scanlineLeftShade;
                scanline.RightShade = scanlineRightShade;
            }

            if (MinVisibleScanline < scanlineBufferCentre - screenCentreY)
            {
                MinVisibleScanline = scanlineBufferCentre - screenCentreY;
            }
        }

        private bool RenderGeneralPolygon(
            int vertexCount,
            int[] polygonX,
            int[] polygonY,
            int[] shadeLevels)
        {
            if (!ComputePolygonYRange(vertexCount, polygonY))
            {
                return false;
            }

            FillScanlineSentinels();

            int lastVertIndex = vertexCount - 1;
            RasteriseClosingEdge(polygonX, polygonY, shadeLevels, lastVertIndex);
            RasteriseInnerEdges(polygonX, polygonY, shadeLevels, lastVertIndex);

            if (MinVisibleScanline < scanlineBufferCentre - screenCentreY)
            {
                MinVisibleScanline = scanlineBufferCentre - screenCentreY;
            }

            return true;
        }

        private bool ComputePolygonYRange(int vertexCount, int[] polygonY)
        {
            MaxVisibleScanline = MinVisibleScanline = polygonY[0] += scanlineBufferCentre;

            for (int vertexIndex = 1; vertexIndex < vertexCount; vertexIndex += 1)
            {
                int scanlineOffset;

                if ((scanlineOffset = polygonY[vertexIndex] += scanlineBufferCentre) < MinVisibleScanline)
                {
                    MinVisibleScanline = scanlineOffset;
                }
                else if (scanlineOffset > MaxVisibleScanline)
                {
                    MaxVisibleScanline = scanlineOffset;
                }
            }

            if (MinVisibleScanline < scanlineBufferCentre - screenCentreY)
            {
                MinVisibleScanline = scanlineBufferCentre - screenCentreY;
            }

            if (MaxVisibleScanline >= scanlineBufferCentre + screenCentreY)
            {
                MaxVisibleScanline = scanlineBufferCentre + screenCentreY - 1;
            }

            return MinVisibleScanline < MaxVisibleScanline;
        }

        private void FillScanlineSentinels()
        {
            for (int scanlineY = MinVisibleScanline; scanlineY < MaxVisibleScanline; scanlineY += 1)
            {
                CameraVariable scanline = ScanlineVariables[scanlineY];
                scanline.LeftX = ScanlineLeftXSentinel;
                scanline.RightX = ScanlineRightXSentinel;
            }
        }

        private void RasteriseClosingEdge(
            int[] polygonX,
            int[] polygonY,
            int[] shadeLevels,
            int lastVertIndex)
        {
            int firstScanlineY = polygonY[0];
            int lastScanlineY = polygonY[lastVertIndex];

            if (firstScanlineY < lastScanlineY)
            {
                int edgeXStart = polygonX[0] << 8;
                int edgeXSlope = ((polygonX[lastVertIndex] - polygonX[0]) << 8) / (lastScanlineY - firstScanlineY);
                int edgeShadeStart = shadeLevels[0] << 8;
                int edgeShadeSlope = ((shadeLevels[lastVertIndex] - shadeLevels[0]) << 8) / (lastScanlineY - firstScanlineY);

                if (firstScanlineY < 0)
                {
                    edgeXStart -= edgeXSlope * firstScanlineY;
                    edgeShadeStart -= edgeShadeSlope * firstScanlineY;
                    firstScanlineY = 0;
                }

                if (lastScanlineY > MaxVisibleScanline)
                {
                    lastScanlineY = MaxVisibleScanline;
                }

                for (int scanlineY = firstScanlineY; scanlineY <= lastScanlineY; scanlineY += 1)
                {
                    CameraVariable scanline = ScanlineVariables[scanlineY];
                    scanline.LeftX = scanline.RightX = edgeXStart;
                    scanline.LeftShade = scanline.RightShade = edgeShadeStart;
                    edgeXStart += edgeXSlope;
                    edgeShadeStart += edgeShadeSlope;
                }
            }
            else if (firstScanlineY > lastScanlineY)
            {
                int revEdgeXStart = polygonX[lastVertIndex] << 8;
                int revEdgeXSlope = (polygonX[0] - polygonX[lastVertIndex] << 8) / (firstScanlineY - lastScanlineY);
                int revEdgeShadeStart = shadeLevels[lastVertIndex] << 8;
                int revEdgeShadeSlope = (shadeLevels[0] - shadeLevels[lastVertIndex] << 8) / (firstScanlineY - lastScanlineY);

                if (lastScanlineY < 0)
                {
                    revEdgeXStart -= revEdgeXSlope * lastScanlineY;
                    revEdgeShadeStart -= revEdgeShadeSlope * lastScanlineY;
                    lastScanlineY = 0;
                }

                if (firstScanlineY > MaxVisibleScanline)
                {
                    firstScanlineY = MaxVisibleScanline;
                }

                for (int scanlineY = lastScanlineY; scanlineY <= firstScanlineY; scanlineY += 1)
                {
                    CameraVariable scanline = ScanlineVariables[scanlineY];
                    scanline.LeftX = scanline.RightX = revEdgeXStart;
                    scanline.LeftShade = scanline.RightShade = revEdgeShadeStart;
                    revEdgeXStart += revEdgeXSlope;
                    revEdgeShadeStart += revEdgeShadeSlope;
                }
            }
        }

        private void RasteriseInnerEdges(
            int[] polygonX,
            int[] polygonY,
            int[] shadeLevels,
            int lastVertIndex)
        {
            for (int edgeIndex = 0; edgeIndex < lastVertIndex; edgeIndex += 1)
            {
                int nextEdgeVertIndex = edgeIndex + 1;
                int edgeY0 = polygonY[edgeIndex];
                int edgeY1 = polygonY[nextEdgeVertIndex];

                if (edgeY0 < edgeY1)
                {
                    int edgeX = polygonX[edgeIndex] << 8;
                    int edgeXSlopeLocal = (polygonX[nextEdgeVertIndex] - polygonX[edgeIndex] << 8) / (edgeY1 - edgeY0);
                    int edgeShade = shadeLevels[edgeIndex] << 8;
                    int edgeShadeSlopeLocal = (shadeLevels[nextEdgeVertIndex] - shadeLevels[edgeIndex] << 8) / (edgeY1 - edgeY0);

                    if (edgeY0 < 0)
                    {
                        edgeX -= edgeXSlopeLocal * edgeY0;
                        edgeShade -= edgeShadeSlopeLocal * edgeY0;
                        edgeY0 = 0;
                    }

                    if (edgeY1 > MaxVisibleScanline)
                    {
                        edgeY1 = MaxVisibleScanline;
                    }

                    for (int scanlineY = edgeY0; scanlineY <= edgeY1; scanlineY += 1)
                    {
                        CameraVariable scanline = ScanlineVariables[scanlineY];

                        if (edgeX < scanline.LeftX)
                        {
                            scanline.LeftX = edgeX;
                            scanline.LeftShade = edgeShade;
                        }

                        if (edgeX > scanline.RightX)
                        {
                            scanline.RightX = edgeX;
                            scanline.RightShade = edgeShade;
                        }

                        edgeX += edgeXSlopeLocal;
                        edgeShade += edgeShadeSlopeLocal;
                    }
                }
                else if (edgeY0 > edgeY1)
                {
                    int revEdgeX = polygonX[nextEdgeVertIndex] << 8;
                    int revEdgeXSlopeLocal = (polygonX[edgeIndex] - polygonX[nextEdgeVertIndex] << 8) / (edgeY0 - edgeY1);
                    int revEdgeShade = shadeLevels[nextEdgeVertIndex] << 8;
                    int revEdgeShadeSlopeLocal = (shadeLevels[edgeIndex] - shadeLevels[nextEdgeVertIndex] << 8) / (edgeY0 - edgeY1);

                    if (edgeY1 < 0)
                    {
                        revEdgeX -= revEdgeXSlopeLocal * edgeY1;
                        revEdgeShade -= revEdgeShadeSlopeLocal * edgeY1;
                        edgeY1 = 0;
                    }

                    if (edgeY0 > MaxVisibleScanline)
                    {
                        edgeY0 = MaxVisibleScanline;
                    }

                    for (int scanlineY = edgeY1; scanlineY <= edgeY0; scanlineY += 1)
                    {
                        CameraVariable scanline = ScanlineVariables[scanlineY];

                        if (revEdgeX < scanline.LeftX)
                        {
                            scanline.LeftX = revEdgeX;
                            scanline.LeftShade = revEdgeShade;
                        }

                        if (revEdgeX > scanline.RightX)
                        {
                            scanline.RightX = revEdgeX;
                            scanline.RightShade = revEdgeShade;
                        }

                        revEdgeX += revEdgeXSlopeLocal;
                        revEdgeShade += revEdgeShadeSlopeLocal;
                    }
                }
            }
        }

    }
}
