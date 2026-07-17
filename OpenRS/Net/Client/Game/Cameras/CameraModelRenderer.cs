namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraModelRenderer
    {
        public bool IsRenderingInterlaced { get; set; }

        public bool IsInterlaced { get; set; }

        private readonly CameraPolygonRasteriser rasteriser;
        private readonly CameraTextureManager textureManager;
        private readonly int[] screenPixels;
        private int screenCentreX;
        private int screenMouseOffsetX;
        private int defaultScreenHalfWidth;
        private int scanlineBufferCentre;
        private int screenProjectionShift;

        private static int SquaredIntensityDivisor => 0x10000;

        public CameraModelRenderer(
            CameraPolygonRasteriser rasteriser,
            CameraTextureManager textureManager,
            int[] screenPixels)
        {
            this.rasteriser = rasteriser;
            this.textureManager = textureManager;
            this.screenPixels = screenPixels;
        }

        public void Initialise(
            int newScreenCentreX,
            int newScreenMouseOffsetX,
            int newDefaultScreenHalfWidth,
            int newScanlineBufferCentre,
            int newScreenProjectionShift)
        {
            screenCentreX = newScreenCentreX;
            screenMouseOffsetX = newScreenMouseOffsetX;
            defaultScreenHalfWidth = newDefaultScreenHalfWidth;
            scanlineBufferCentre = newScanlineBufferCentre;
            screenProjectionShift = newScreenProjectionShift;
        }

        public void Render(
            int vertCount,
            int[] vertX,
            int[] vertY,
            int[] vertZ,
            int textureIndex,
            GameObject gameObject)
        {
            if (textureIndex == -2)
            {
                return;
            }

            if (textureIndex >= 0)
            {
                RenderTexturedModel(vertCount, vertX, vertY, vertZ, textureIndex, gameObject);
                return;
            }

            RenderColouredModel(textureIndex, gameObject);
        }

        private void RenderTexturedModel(
            int vertCount,
            int[] modelVertX,
            int[] modelVertY,
            int[] modelVertZ,
            int textureIndex,
            GameObject gameObject)
        {
            if (textureIndex >= textureManager.textureCount)
            {
                textureIndex = 0;
            }

            textureManager.UpdateTextureSmoothing(textureIndex);
            int vert0X = modelVertX[0];
            int vert0Y = modelVertY[0];
            int vert0Z = modelVertZ[0];
            int edge1X = vert0X - modelVertX[1];
            int edge1Y = vert0Y - modelVertY[1];
            int edge1Z = vert0Z - modelVertZ[1];
            vertCount -= 1;
            int edge2X = modelVertX[vertCount] - vert0X;
            int edge2Y = modelVertY[vertCount] - vert0Y;
            int edge2Z = modelVertZ[vertCount] - vert0Z;

            if (textureManager.textureLastAccessFrame[textureIndex] == 1)
            {
                RenderTexture128Scanlines(
                    textureIndex,
                    gameObject,
                    vert0X, vert0Y, vert0Z,
                    edge1X, edge1Y, edge1Z,
                    edge2X, edge2Y, edge2Z);
            }
            else
            {
                RenderTexture64Scanlines(
                    textureIndex,
                    gameObject,
                    vert0X, vert0Y, vert0Z,
                    edge1X, edge1Y, edge1Z,
                    edge2X, edge2Y, edge2Z);
            }
        }

        private void RenderTexture128Scanlines(
            int textureIndex,
            GameObject gameObject,
            int vert0X,
            int vert0Y,
            int vert0Z,
            int edge1X,
            int edge1Y,
            int edge1Z,
            int edge2X,
            int edge2Y,
            int edge2Z)
        {
            int texUOrigin = edge2X * vert0Y - edge2Y * vert0X << 12;
            int texURowStep = edge2Y * vert0Z - edge2Z * vert0Y << 5 - screenProjectionShift + 7 + 4;
            int texUColStep = edge2Z * vert0X - edge2X * vert0Z << 5 - screenProjectionShift + 7;
            int texVOrigin = edge1X * vert0Y - edge1Y * vert0X << 12;
            int texVRowStep = edge1Y * vert0Z - edge1Z * vert0Y << 5 - screenProjectionShift + 7 + 4;
            int texVColStep = edge1Z * vert0X - edge1X * vert0Z << 5 - screenProjectionShift + 7;
            int texDenomOrigin = edge1Y * edge2X - edge1X * edge2Y << 5;
            int texDenomRowStep = edge1Z * edge2Y - edge1Y * edge2Z << 5 - screenProjectionShift + 4;
            int texDenomColStep = edge1X * edge2Z - edge1Z * edge2X >> screenProjectionShift - 5;
            int texURowStepScaled = texURowStep >> 4;
            int texVRowStepScaled = texVRowStep >> 4;
            int texDenomRowStepScaled = texDenomRowStep >> 4;
            int scanlineOffset = rasteriser.MinVisibleScanline - scanlineBufferCentre;
            int rowStride = defaultScreenHalfWidth;
            int pixelOffset = screenMouseOffsetX + rasteriser.MinVisibleScanline * rowStride;
            byte scanlineStep = 1;
            texUOrigin += texUColStep * scanlineOffset;
            texVOrigin += texVColStep * scanlineOffset;
            texDenomOrigin += texDenomColStep * scanlineOffset;

            if (IsRenderingInterlaced)
            {
                if ((rasteriser.MinVisibleScanline & 1) == 1)
                {
                    rasteriser.MinVisibleScanline += 1;
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                texUColStep <<= 1;
                texVColStep <<= 1;
                texDenomColStep <<= 1;
                rowStride <<= 1;
                scanlineStep = 2;
            }

            if (gameObject.IsPerspectiveTextured)
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int scanlineMaxX = scanline.RightX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;

                    if (spanWidth <= 0)
                    {
                        texUOrigin += texUColStep;
                        texVOrigin += texVColStep;
                        texDenomOrigin += texDenomColStep;
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum = scanline.LeftShade;
                    int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                        scanlineX = -screenCentreX;
                        spanWidth = scanlineMaxX - scanlineX;
                    }

                    if (scanlineMaxX > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawShadedPolygon(
                        screenPixels,
                        textureManager.objectTexturePixels[textureIndex],
                        0,
                        0,
                        texUOrigin + texURowStepScaled * scanlineX,
                        texVOrigin + texVRowStepScaled * scanlineX,
                        texDenomOrigin + texDenomRowStepScaled * scanlineX,
                        texURowStep,
                        texVRowStep,
                        texDenomRowStep,
                        spanWidth,
                        pixelOffset + scanlineX,
                        shadeAccum,
                        shadeStep << 2);
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                return;
            }

            if (!textureManager.textureIsTransparent[textureIndex])
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int scanlineMaxX = scanline.RightX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;

                    if (spanWidth <= 0)
                    {
                        texUOrigin += texUColStep;
                        texVOrigin += texVColStep;
                        texDenomOrigin += texDenomColStep;
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum64 = scanline.LeftShade;
                    int shadeStep64 = (scanline.RightShade - shadeAccum64) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum64 += (-screenCentreX - scanlineX) * shadeStep64;
                        scanlineX = -screenCentreX;
                        spanWidth = scanlineMaxX - scanlineX;
                    }

                    if (scanlineMaxX > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawFlatPolygon(
                        screenPixels,
                        textureManager.objectTexturePixels[textureIndex],
                        0,
                        0,
                        texUOrigin + texURowStepScaled * scanlineX,
                        texVOrigin + texVRowStepScaled * scanlineX,
                        texDenomOrigin + texDenomRowStepScaled * scanlineX,
                        texURowStep,
                        texVRowStep,
                        texDenomRowStep,
                        spanWidth,
                        pixelOffset + scanlineX,
                        shadeAccum64,
                        shadeStep64 << 2);
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                return;
            }

            for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
            {
                CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                int scanlineX = scanline.LeftX >> 8;
                int scanlineMaxX = scanline.RightX >> 8;
                int spanWidth = scanlineMaxX - scanlineX;

                if (spanWidth <= 0)
                {
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                    continue;
                }

                int shadeAccum = scanline.LeftShade;
                int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                if (scanlineX < -screenCentreX)
                {
                    shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                    scanlineX = -screenCentreX;
                    spanWidth = scanlineMaxX - scanlineX;
                }

                if (scanlineMaxX > screenCentreX)
                {
                    int clampedX = screenCentreX;
                    spanWidth = clampedX - scanlineX;
                }

                CameraPolygonDrawer.DrawTexturedPolygon(
                    screenPixels,
                    0,
                    0,
                    0,
                    textureManager.objectTexturePixels[textureIndex],
                    texUOrigin + texURowStepScaled * scanlineX,
                    texVOrigin + texVRowStepScaled * scanlineX,
                    texDenomOrigin + texDenomRowStepScaled * scanlineX,
                    texURowStep,
                    texVRowStep,
                    texDenomRowStep,
                    spanWidth,
                    pixelOffset + scanlineX,
                    shadeAccum,
                    shadeStep);
                texUOrigin += texUColStep;
                texVOrigin += texVColStep;
                texDenomOrigin += texDenomColStep;
                pixelOffset += rowStride;
            }
        }

        private void RenderTexture64Scanlines(
            int textureIndex,
            GameObject gameObject,
            int vert0X,
            int vert0Y,
            int vert0Z,
            int edge1X,
            int edge1Y,
            int edge1Z,
            int edge2X,
            int edge2Y,
            int edge2Z)
        {
            int texUOrigin = (edge2X * vert0Y - edge2Y * vert0X) << 11;
            int texURowStep = (edge2Y * vert0Z - edge2Z * vert0Y) << 5 - screenProjectionShift + 6 + 4;
            int texUColStep = (edge2Z * vert0X - edge2X * vert0Z) << 5 - screenProjectionShift + 6;
            int texVOrigin = (edge1X * vert0Y - edge1Y * vert0X) << 11;
            int texVRowStep = (edge1Y * vert0Z - edge1Z * vert0Y) << 5 - screenProjectionShift + 6 + 4;
            int texVColStep = (edge1Z * vert0X - edge1X * vert0Z) << 5 - screenProjectionShift + 6;
            int texDenomOrigin = (edge1Y * edge2X - edge1X * edge2Y) << 5;
            int texDenomRowStep = (edge1Z * edge2Y - edge1Y * edge2Z) << 5 - screenProjectionShift + 4;
            int texDenomColStep = (edge1X * edge2Z - edge1Z * edge2X) >> (screenProjectionShift - 5);
            int texURowStepScaled = texURowStep >> 4;
            int texVRowStepScaled = texVRowStep >> 4;
            int texDenomRowStepScaled = texDenomRowStep >> 4;
            int scanlineOffset = rasteriser.MinVisibleScanline - scanlineBufferCentre;
            int rowStride = defaultScreenHalfWidth;
            int pixelOffset = screenMouseOffsetX + rasteriser.MinVisibleScanline * rowStride;
            byte scanlineStep = 1;
            texUOrigin += texUColStep * scanlineOffset;
            texVOrigin += texVColStep * scanlineOffset;
            texDenomOrigin += texDenomColStep * scanlineOffset;

            if (IsRenderingInterlaced)
            {
                if ((rasteriser.MinVisibleScanline & 1) == 1)
                {
                    rasteriser.MinVisibleScanline += 1;
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                texUColStep <<= 1;
                texVColStep <<= 1;
                texDenomColStep <<= 1;
                rowStride <<= 1;
                scanlineStep = 2;
            }

            if (gameObject.IsPerspectiveTextured)
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int clampedX64 = scanline.RightX >> 8;
                    int spanWidth = clampedX64 - scanlineX;

                    if (spanWidth <= 0)
                    {
                        texUOrigin += texUColStep;
                        texVOrigin += texVColStep;
                        texDenomOrigin += texDenomColStep;
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum = scanline.LeftShade;
                    int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                        scanlineX = -screenCentreX;
                        spanWidth = clampedX64 - scanlineX;
                    }

                    if (clampedX64 > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawMaskedPolygon(
                        screenPixels,
                        textureManager.objectTexturePixels[textureIndex],
                        0,
                        0,
                        texUOrigin + texURowStepScaled * scanlineX,
                        texVOrigin + texVRowStepScaled * scanlineX,
                        texDenomOrigin + texDenomRowStepScaled * scanlineX,
                        texURowStep,
                        texVRowStep,
                        texDenomRowStep,
                        spanWidth,
                        pixelOffset + scanlineX,
                        shadeAccum,
                        shadeStep);
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                return;
            }

            if (!textureManager.textureIsTransparent[textureIndex])
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int scanlineMaxX = scanline.RightX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;

                    if (spanWidth <= 0)
                    {
                        texUOrigin += texUColStep;
                        texVOrigin += texVColStep;
                        texDenomOrigin += texDenomColStep;
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum = scanline.LeftShade;
                    int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                        scanlineX = -screenCentreX;
                        spanWidth = scanlineMaxX - scanlineX;
                    }

                    if (scanlineMaxX > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawTransparentPolygon(
                        screenPixels,
                        textureManager.objectTexturePixels[textureIndex],
                        0,
                        0,
                        texUOrigin + texURowStepScaled * scanlineX,
                        texVOrigin + texVRowStepScaled * scanlineX,
                        texDenomOrigin + texDenomRowStepScaled * scanlineX,
                        texURowStep,
                        texVRowStep,
                        texDenomRowStep,
                        spanWidth,
                        pixelOffset + scanlineX,
                        shadeAccum,
                        shadeStep);
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                }

                return;
            }

            for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
            {
                CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                int scanlineX = scanline.LeftX >> 8;
                int scanlineMaxX = scanline.RightX >> 8;
                int spanWidth = scanlineMaxX - scanlineX;

                if (spanWidth <= 0)
                {
                    texUOrigin += texUColStep;
                    texVOrigin += texVColStep;
                    texDenomOrigin += texDenomColStep;
                    pixelOffset += rowStride;
                    continue;
                }

                int shadeAccum = scanline.LeftShade;
                int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                if (scanlineX < -screenCentreX)
                {
                    shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                    scanlineX = -screenCentreX;
                    spanWidth = scanlineMaxX - scanlineX;
                }

                if (scanlineMaxX > screenCentreX)
                {
                    int spanWidth64 = screenCentreX;
                    spanWidth = spanWidth64 - scanlineX;
                }

                CameraPolygonDrawer.DrawFlatTexturedPolygon(
                    screenPixels,
                    0,
                    0,
                    0,
                    textureManager.objectTexturePixels[textureIndex],
                    texUOrigin + texURowStepScaled * scanlineX,
                    texVOrigin + texVRowStepScaled * scanlineX,
                    texDenomOrigin + texDenomRowStepScaled * scanlineX,
                    texURowStep,
                    texVRowStep,
                    texDenomRowStep,
                    spanWidth,
                    pixelOffset + scanlineX,
                    shadeAccum,
                    shadeStep);
                texUOrigin += texUColStep;
                texVOrigin += texVColStep;
                texDenomOrigin += texDenomColStep;
                pixelOffset += rowStride;
            }
        }

        private void RenderColouredModel(int textureIndex, GameObject gameObject)
        {
            EnsureColourTable(textureIndex);

            int rowStride = defaultScreenHalfWidth;
            int pixelOffset = screenMouseOffsetX + rasteriser.MinVisibleScanline * rowStride;
            byte scanlineStep = 1;

            if (IsRenderingInterlaced)
            {
                if ((rasteriser.MinVisibleScanline & 1) == 1)
                {
                    rasteriser.MinVisibleScanline += 1;
                    pixelOffset += rowStride;
                }

                rowStride <<= 1;
                scanlineStep = 2;
            }

            if (gameObject.IsGiantCrystal)
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int scanlineMaxX = scanline.RightX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;

                    if (spanWidth <= 0)
                    {
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum = scanline.LeftShade;
                    int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                        scanlineX = -screenCentreX;
                        spanWidth = scanlineMaxX - scanlineX;
                    }

                    if (scanlineMaxX > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawShiftColourPolygon(
                        screenPixels,
                        -spanWidth,
                        pixelOffset + scanlineX,
                        0,
                        textureManager.textureClipSizes,
                        shadeAccum,
                        shadeStep);
                    pixelOffset += rowStride;
                }

                return;
            }

            if (IsInterlaced)
            {
                for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
                {
                    CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                    int scanlineX = scanline.LeftX >> 8;
                    int scanlineMaxX = scanline.RightX >> 8;
                    int spanWidth = scanlineMaxX - scanlineX;

                    if (spanWidth <= 0)
                    {
                        pixelOffset += rowStride;
                        continue;
                    }

                    int shadeAccum = scanline.LeftShade;
                    int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                    if (scanlineX < -screenCentreX)
                    {
                        shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                        scanlineX = -screenCentreX;
                        spanWidth = scanlineMaxX - scanlineX;
                    }

                    if (scanlineMaxX > screenCentreX)
                    {
                        int clampedX = screenCentreX;
                        spanWidth = clampedX - scanlineX;
                    }

                    CameraPolygonDrawer.DrawVertexColourPolygon(
                        screenPixels,
                        -spanWidth,
                        pixelOffset + scanlineX,
                        0,
                        textureManager.textureClipSizes,
                        shadeAccum,
                        shadeStep);
                    pixelOffset += rowStride;
                }

                return;
            }

            for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
            {
                CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                int scanlineX = scanline.LeftX >> 8;
                int scanlineMaxX = scanline.RightX >> 8;
                int spanWidth = scanlineMaxX - scanlineX;

                if (spanWidth <= 0)
                {
                    pixelOffset += rowStride;
                    continue;
                }

                int shadeAccum = scanline.LeftShade;
                int shadeStep = (scanline.RightShade - shadeAccum) / spanWidth;

                if (scanlineX < -screenCentreX)
                {
                    shadeAccum += (-screenCentreX - scanlineX) * shadeStep;
                    scanlineX = -screenCentreX;
                    spanWidth = scanlineMaxX - scanlineX;
                }

                if (scanlineMaxX > screenCentreX)
                {
                    int clampedX = screenCentreX;
                    spanWidth = clampedX - scanlineX;
                }

                CameraPolygonDrawer.DrawGradientPolygon(
                    screenPixels,
                    -spanWidth,
                    pixelOffset + scanlineX,
                    0,
                    textureManager.textureClipSizes,
                    shadeAccum,
                    shadeStep);
                pixelOffset += rowStride;
            }
        }

        private void EnsureColourTable(int textureIndex)
        {
            for (int clipIndex = 0; clipIndex < textureManager.maxTextureCount; clipIndex += 1)
            {
                if (textureManager.textureClipIds[clipIndex] == textureIndex)
                {
                    textureManager.textureClipSizes = textureManager.textureClipData[clipIndex];
                    return;
                }

                if (clipIndex != textureManager.maxTextureCount - 1)
                {
                    continue;
                }

                double randomValue = Helper.Random.NextDouble();
                int randomSlot = (int)(randomValue * textureManager.maxTextureCount);

                if (randomSlot >= textureManager.textureClipIds.Length)
                {
                    randomSlot -= 1;
                }

                textureManager.textureClipIds[randomSlot] = textureIndex;
                int encodedTextureIndex = -1 - textureIndex;
                int redChannel = (encodedTextureIndex >> 10 & 0x1f) * 8;
                int greenChannel = (encodedTextureIndex >> 5 & 0x1f) * 8;
                int blueChannel = (encodedTextureIndex & 0x1f) * 8;

                for (int colourTableIndex = 0; colourTableIndex < 256; colourTableIndex += 1)
                {
                    int squaredIntensity = colourTableIndex * colourTableIndex;
                    int redScaled = redChannel * squaredIntensity / SquaredIntensityDivisor;
                    int greenScaled = greenChannel * squaredIntensity / SquaredIntensityDivisor;
                    int blueScaled = blueChannel * squaredIntensity / SquaredIntensityDivisor;
                    textureManager.textureClipData[randomSlot][255 - colourTableIndex] = (redScaled << 16) + (greenScaled << 8) + blueScaled;
                }

                textureManager.textureClipSizes = textureManager.textureClipData[randomSlot];
            }
        }
    }
}
