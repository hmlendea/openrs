namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraPolygonRasteriser(CameraSceneObjectTracker sceneObjectTracker)
    {
        public CameraVariable[] ScanlineVariables { get; private set; }

        public int MinVisibleScanline { get; set; }

        public int MaxVisibleScanline { get; set; }

        private int scanlineBufferCentre;
        private int screenCentreY;

        private static int NotSetSentinel => 0xbc614e;

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

            CheckPolygonMouseHit(gameObject, faceVertexIndex);
        }

        private void RenderTrianglePolygon(int[] polygonX, int[] polygonY, int[] shadeLevels)
        {
            int scanlineY0 = polygonY[0] + scanlineBufferCentre;
            int scanlineY1 = polygonY[1] + scanlineBufferCentre;
            int scanlineY2 = polygonY[2] + scanlineBufferCentre;
            int vertX0 = polygonX[0];
            int vertX1 = polygonX[1];
            int vertX2 = polygonX[2];
            int vertShade0 = shadeLevels[0];
            int vertShade1 = shadeLevels[1];
            int vertShade2 = shadeLevels[2];
            int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
            int edgeAX = 0;
            int edgeAXSlope = 0;
            int edgeAShade = 0;
            int edgeAShadeSlope = 0;
            int edgeAMinY = NotSetSentinel;
            int edgeAMaxY = -NotSetSentinel;

            if (scanlineY2 != scanlineY0)
            {
                edgeAXSlope = (vertX2 - vertX0 << 8) / (scanlineY2 - scanlineY0);
                edgeAShadeSlope = (vertShade2 - vertShade0 << 8) / (scanlineY2 - scanlineY0);

                if (scanlineY0 < scanlineY2)
                {
                    edgeAX = vertX0 << 8;
                    edgeAShade = vertShade0 << 8;
                    edgeAMinY = scanlineY0;
                    edgeAMaxY = scanlineY2;
                }
                else
                {
                    edgeAX = vertX2 << 8;
                    edgeAShade = vertShade2 << 8;
                    edgeAMinY = scanlineY2;
                    edgeAMaxY = scanlineY0;
                }

                if (edgeAMinY < 0)
                {
                    edgeAX -= edgeAXSlope * edgeAMinY;
                    edgeAShade -= edgeAShadeSlope * edgeAMinY;
                    edgeAMinY = 0;
                }

                if (edgeAMaxY > scanlineLimit)
                {
                    edgeAMaxY = scanlineLimit;
                }
            }

            int edgeBX = 0;
            int edgeBXSlope = 0;
            int edgeBShade = 0;
            int edgeBShadeSlope = 0;
            int edgeBMinY = NotSetSentinel;
            int edgeBMaxY = -NotSetSentinel;

            if (scanlineY1 != scanlineY0)
            {
                edgeBXSlope = (vertX1 - vertX0 << 8) / (scanlineY1 - scanlineY0);
                edgeBShadeSlope = (vertShade1 - vertShade0 << 8) / (scanlineY1 - scanlineY0);

                if (scanlineY0 < scanlineY1)
                {
                    edgeBX = vertX0 << 8;
                    edgeBShade = vertShade0 << 8;
                    edgeBMinY = scanlineY0;
                    edgeBMaxY = scanlineY1;
                }
                else
                {
                    edgeBX = vertX1 << 8;
                    edgeBShade = vertShade1 << 8;
                    edgeBMinY = scanlineY1;
                    edgeBMaxY = scanlineY0;
                }

                if (edgeBMinY < 0)
                {
                    edgeBX -= edgeBXSlope * edgeBMinY;
                    edgeBShade -= edgeBShadeSlope * edgeBMinY;
                    edgeBMinY = 0;
                }

                if (edgeBMaxY > scanlineLimit)
                {
                    edgeBMaxY = scanlineLimit;
                }
            }

            int edgeCX = 0;
            int edgeCXSlope = 0;
            int edgeCShade = 0;
            int edgeCShadeSlope = 0;
            int edgeCMinY = NotSetSentinel;
            int edgeCMaxY = -NotSetSentinel;

            if (scanlineY2 != scanlineY1)
            {
                edgeCXSlope = (vertX2 - vertX1 << 8) / (scanlineY2 - scanlineY1);
                edgeCShadeSlope = (vertShade2 - vertShade1 << 8) / (scanlineY2 - scanlineY1);

                if (scanlineY1 < scanlineY2)
                {
                    edgeCX = vertX1 << 8;
                    edgeCShade = vertShade1 << 8;
                    edgeCMinY = scanlineY1;
                    edgeCMaxY = scanlineY2;
                }
                else
                {
                    edgeCX = vertX2 << 8;
                    edgeCShade = vertShade2 << 8;
                    edgeCMinY = scanlineY2;
                    edgeCMaxY = scanlineY1;
                }

                if (edgeCMinY < 0)
                {
                    edgeCX -= edgeCXSlope * edgeCMinY;
                    edgeCShade -= edgeCShadeSlope * edgeCMinY;
                    edgeCMinY = 0;
                }

                if (edgeCMaxY > scanlineLimit)
                {
                    edgeCMaxY = scanlineLimit;
                }
            }

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
            int scanlineY0 = polygonY[0] + scanlineBufferCentre;
            int scanlineY1 = polygonY[1] + scanlineBufferCentre;
            int scanlineY2 = polygonY[2] + scanlineBufferCentre;
            int scanlineY3 = polygonY[3] + scanlineBufferCentre;
            int vertX0 = polygonX[0];
            int vertX1 = polygonX[1];
            int vertX2 = polygonX[2];
            int vertX3 = polygonX[3];
            int vertShade0 = shadeLevels[0];
            int vertShade1 = shadeLevels[1];
            int vertShade2 = shadeLevels[2];
            int vertShade3 = shadeLevels[3];
            int scanlineLimit = scanlineBufferCentre + screenCentreY - 1;
            int edgeAX = 0;
            int edgeAXSlope = 0;
            int edgeAShade = 0;
            int edgeAShadeSlope = 0;
            int edgeAMinY = NotSetSentinel;
            int edgeAMaxY = -NotSetSentinel;

            if (scanlineY3 != scanlineY0)
            {
                edgeAXSlope = (vertX3 - vertX0 << 8) / (scanlineY3 - scanlineY0);
                edgeAShadeSlope = (vertShade3 - vertShade0 << 8) / (scanlineY3 - scanlineY0);

                if (scanlineY0 < scanlineY3)
                {
                    edgeAX = vertX0 << 8;
                    edgeAShade = vertShade0 << 8;
                    edgeAMinY = scanlineY0;
                    edgeAMaxY = scanlineY3;
                }
                else
                {
                    edgeAX = vertX3 << 8;
                    edgeAShade = vertShade3 << 8;
                    edgeAMinY = scanlineY3;
                    edgeAMaxY = scanlineY0;
                }

                if (edgeAMinY < 0)
                {
                    edgeAX -= edgeAXSlope * edgeAMinY;
                    edgeAShade -= edgeAShadeSlope * edgeAMinY;
                    edgeAMinY = 0;
                }

                if (edgeAMaxY > scanlineLimit)
                {
                    edgeAMaxY = scanlineLimit;
                }
            }

            int edgeBX = 0;
            int edgeBXSlope = 0;
            int edgeBShade = 0;
            int edgeBShadeSlope = 0;
            int edgeBMinY = NotSetSentinel;
            int edgeBMaxY = -NotSetSentinel;

            if (scanlineY1 != scanlineY0)
            {
                edgeBXSlope = (vertX1 - vertX0 << 8) / (scanlineY1 - scanlineY0);
                edgeBShadeSlope = (vertShade1 - vertShade0 << 8) / (scanlineY1 - scanlineY0);

                if (scanlineY0 < scanlineY1)
                {
                    edgeBX = vertX0 << 8;
                    edgeBShade = vertShade0 << 8;
                    edgeBMinY = scanlineY0;
                    edgeBMaxY = scanlineY1;
                }
                else
                {
                    edgeBX = vertX1 << 8;
                    edgeBShade = vertShade1 << 8;
                    edgeBMinY = scanlineY1;
                    edgeBMaxY = scanlineY0;
                }

                if (edgeBMinY < 0)
                {
                    edgeBX -= edgeBXSlope * edgeBMinY;
                    edgeBShade -= edgeBShadeSlope * edgeBMinY;
                    edgeBMinY = 0;
                }

                if (edgeBMaxY > scanlineLimit)
                {
                    edgeBMaxY = scanlineLimit;
                }
            }

            int edgeCX = 0;
            int edgeCXSlope = 0;
            int edgeCShade = 0;
            int edgeCShadeSlope = 0;
            int edgeCMinY = NotSetSentinel;
            int edgeCMaxY = -NotSetSentinel;

            if (scanlineY2 != scanlineY1)
            {
                edgeCXSlope = (vertX2 - vertX1 << 8) / (scanlineY2 - scanlineY1);
                edgeCShadeSlope = (vertShade2 - vertShade1 << 8) / (scanlineY2 - scanlineY1);

                if (scanlineY1 < scanlineY2)
                {
                    edgeCX = vertX1 << 8;
                    edgeCShade = vertShade1 << 8;
                    edgeCMinY = scanlineY1;
                    edgeCMaxY = scanlineY2;
                }
                else
                {
                    edgeCX = vertX2 << 8;
                    edgeCShade = vertShade2 << 8;
                    edgeCMinY = scanlineY2;
                    edgeCMaxY = scanlineY1;
                }

                if (edgeCMinY < 0)
                {
                    edgeCX -= edgeCXSlope * edgeCMinY;
                    edgeCShade -= edgeCShadeSlope * edgeCMinY;
                    edgeCMinY = 0;
                }

                if (edgeCMaxY > scanlineLimit)
                {
                    edgeCMaxY = scanlineLimit;
                }
            }

            int edgeDX = 0;
            int edgeDXSlope = 0;
            int edgeDShade = 0;
            int edgeDShadeSlope = 0;
            int edgeDMinY = NotSetSentinel;
            int edgeDMaxY = -NotSetSentinel;

            if (scanlineY3 != scanlineY2)
            {
                edgeDXSlope = (vertX3 - vertX2 << 8) / (scanlineY3 - scanlineY2);
                edgeDShadeSlope = (vertShade3 - vertShade2 << 8) / (scanlineY3 - scanlineY2);

                if (scanlineY2 < scanlineY3)
                {
                    edgeDX = vertX2 << 8;
                    edgeDShade = vertShade2 << 8;
                    edgeDMinY = scanlineY2;
                    edgeDMaxY = scanlineY3;
                }
                else
                {
                    edgeDX = vertX3 << 8;
                    edgeDShade = vertShade3 << 8;
                    edgeDMinY = scanlineY3;
                    edgeDMaxY = scanlineY2;
                }

                if (edgeDMinY < 0)
                {
                    edgeDX -= edgeDXSlope * edgeDMinY;
                    edgeDShade -= edgeDShadeSlope * edgeDMinY;
                    edgeDMinY = 0;
                }

                if (edgeDMaxY > scanlineLimit)
                {
                    edgeDMaxY = scanlineLimit;
                }
            }

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

        private void CheckPolygonMouseHit(GameObject gameObject, int faceVertexIndex)
        {
            if (sceneObjectTracker.IsHitCandidate &&
                sceneObjectTracker.MouseAdjustedY >= MinVisibleScanline &&
                sceneObjectTracker.MouseAdjustedY < MaxVisibleScanline)
            {
                CameraVariable scanlineAtHitY = ScanlineVariables[sceneObjectTracker.MouseAdjustedY];
                bool isWithinScanlineX = sceneObjectTracker.MouseAdjustedX >= scanlineAtHitY.LeftX >> 8 &&
                                          sceneObjectTracker.MouseAdjustedX <= scanlineAtHitY.RightX >> 8;
                bool hasScanlineSpan = scanlineAtHitY.LeftX <= scanlineAtHitY.RightX;
                bool isHittablePolygon = !gameObject.shareEntityArrays && gameObject.polygonTypeData[faceVertexIndex] == 0;

                if (isWithinScanlineX && hasScanlineSpan && isHittablePolygon)
                {
                    sceneObjectTracker.RecordHit(gameObject, faceVertexIndex);
                }
            }
        }
    }
}
