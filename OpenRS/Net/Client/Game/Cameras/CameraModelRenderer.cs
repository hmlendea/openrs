namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraModelRenderer(
        CameraPolygonRasteriser rasteriser,
        CameraTextureManager textureManager,
        int[] screenPixels)
    {
        public bool IsRenderingInterlaced { get; set; }

        public bool IsInterlaced { get; set; }

        private readonly CameraColourTableCache colourTableCache =
            new(textureManager);
        private readonly CameraPolygonRasteriser rasteriser = rasteriser;
        private readonly CameraTextureManager textureManager = textureManager;
        private readonly int[] screenPixels = screenPixels;
        private int screenCentreX;
        private int screenMouseOffsetX;
        private int defaultScreenHalfWidth;
        private int scanlineBufferCentre;
        private int screenProjectionShift;

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
            bool isTexture128 =
                textureManager.textureLastAccessFrame[textureIndex] == 1;
            int textureCoordinateShift = 6;

            if (isTexture128)
            {
                textureCoordinateShift = 7;
            }

            CameraTextureProjection textureProjection =
                CameraTextureProjection.Calculate(
                    vert0X,
                    vert0Y,
                    vert0Z,
                    edge1X,
                    edge1Y,
                    edge1Z,
                    edge2X,
                    edge2Y,
                    edge2Z,
                    screenProjectionShift,
                    textureCoordinateShift);
            CameraTextureScanlineState scanlineState = new(
                textureProjection,
                rasteriser,
                scanlineBufferCentre,
                defaultScreenHalfWidth,
                screenMouseOffsetX,
                IsRenderingInterlaced);
            CameraTextureRenderMode renderMode = ResolveTextureRenderMode(
                isTexture128,
                gameObject.IsPerspectiveTextured,
                textureManager.textureIsTransparent[textureIndex]);
            RenderTextureScanlines(textureIndex, renderMode, scanlineState);
        }

        private static CameraTextureRenderMode ResolveTextureRenderMode(
            bool isTexture128,
            bool isPerspectiveTextured,
            bool isTransparent)
        {
            if (isTexture128)
            {
                if (isPerspectiveTextured)
                {
                    return CameraTextureRenderMode.HalfBlended128;
                }

                if (!isTransparent)
                {
                    return CameraTextureRenderMode.Opaque128;
                }

                return CameraTextureRenderMode.Transparent128;
            }

            if (isPerspectiveTextured)
            {
                return CameraTextureRenderMode.HalfBlended64;
            }

            if (!isTransparent)
            {
                return CameraTextureRenderMode.Opaque64;
            }

            return CameraTextureRenderMode.Transparent64;
        }

        private void RenderTextureScanlines(
            int textureIndex,
            CameraTextureRenderMode renderMode,
            CameraTextureScanlineState scanlineState)
        {
            int textureUOrigin = scanlineState.UOrigin;
            int textureVOrigin = scanlineState.VOrigin;
            int denominatorOrigin = scanlineState.DenominatorOrigin;
            int textureURowStepScaled = scanlineState.URowStep >> 4;
            int textureVRowStepScaled = scanlineState.VRowStep >> 4;
            int denominatorRowStepScaled =
                scanlineState.DenominatorRowStep >> 4;
            int pixelOffset = scanlineState.PixelOffset;

            for (int scanlineY = rasteriser.MinVisibleScanline;
                scanlineY < rasteriser.MaxVisibleScanline;
                scanlineY += scanlineState.ScanlineStep)
            {
                CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                CameraRenderedScanline renderedScanline =
                    CameraRenderedScanline.Calculate(
                        scanline,
                        screenCentreX);

                if (renderedScanline.HasSourceSpan)
                {
                    int textureU =
                        textureUOrigin +
                        textureURowStepScaled * renderedScanline.StartX;
                    int textureV =
                        textureVOrigin +
                        textureVRowStepScaled * renderedScanline.StartX;
                    int denominator =
                        denominatorOrigin +
                        denominatorRowStepScaled * renderedScanline.StartX;
                    int destinationOffset =
                        pixelOffset + renderedScanline.StartX;
                    int[] texturePixels =
                        textureManager.objectTexturePixels[textureIndex];

                    switch (renderMode)
                    {
                        case CameraTextureRenderMode.HalfBlended128:
                            CameraPolygonDrawer.DrawShadedPolygon(
                                screenPixels,
                                texturePixels,
                                0,
                                0,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep << 2);
                            break;
                        case CameraTextureRenderMode.Opaque128:
                            CameraPolygonDrawer.DrawFlatPolygon(
                                screenPixels,
                                texturePixels,
                                0,
                                0,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep << 2);
                            break;
                        case CameraTextureRenderMode.Transparent128:
                            CameraPolygonDrawer.DrawTexturedPolygon(
                                screenPixels,
                                0,
                                0,
                                0,
                                texturePixels,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep);
                            break;
                        case CameraTextureRenderMode.HalfBlended64:
                            CameraPolygonDrawer.DrawMaskedPolygon(
                                screenPixels,
                                texturePixels,
                                0,
                                0,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep);
                            break;
                        case CameraTextureRenderMode.Opaque64:
                            CameraPolygonDrawer.DrawTransparentPolygon(
                                screenPixels,
                                texturePixels,
                                0,
                                0,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep);
                            break;
                        case CameraTextureRenderMode.Transparent64:
                            CameraPolygonDrawer.DrawFlatTexturedPolygon(
                                screenPixels,
                                0,
                                0,
                                0,
                                texturePixels,
                                textureU,
                                textureV,
                                denominator,
                                scanlineState.URowStep,
                                scanlineState.VRowStep,
                                scanlineState.DenominatorRowStep,
                                renderedScanline.Width,
                                destinationOffset,
                                renderedScanline.Shade,
                                renderedScanline.ShadeStep);
                            break;
                    }
                }

                textureUOrigin += scanlineState.UColumnStep;
                textureVOrigin += scanlineState.VColumnStep;
                denominatorOrigin += scanlineState.DenominatorColumnStep;
                pixelOffset += scanlineState.RowStride;
            }
        }

        private void RenderColouredModel(int textureIndex, GameObject gameObject)
        {
            colourTableCache.Ensure(textureIndex);

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

            CameraColourRenderMode renderMode = CameraColourRenderMode.Gradient;

            if (gameObject.IsGiantCrystal)
            {
                renderMode = CameraColourRenderMode.HalfBlended;
            }
            else if (IsInterlaced)
            {
                renderMode = CameraColourRenderMode.Vertex;
            }

            RenderColourScanlines(
                renderMode,
                rowStride,
                pixelOffset,
                scanlineStep);
        }

        private void RenderColourScanlines(
            CameraColourRenderMode renderMode,
            int rowStride,
            int pixelOffset,
            int scanlineStep)
        {
            for (int scanlineY = rasteriser.MinVisibleScanline; scanlineY < rasteriser.MaxVisibleScanline; scanlineY += scanlineStep)
            {
                CameraVariable scanline = rasteriser.ScanlineVariables[scanlineY];
                CameraRenderedScanline renderedScanline =
                    CameraRenderedScanline.Calculate(
                        scanline,
                        screenCentreX);

                if (!renderedScanline.HasSourceSpan)
                {
                    pixelOffset += rowStride;
                    continue;
                }

                int spanWidth = -renderedScanline.Width;
                int destinationOffset =
                    pixelOffset + renderedScanline.StartX;

                switch (renderMode)
                {
                    case CameraColourRenderMode.HalfBlended:
                        CameraPolygonDrawer.DrawShiftColourPolygon(
                            screenPixels,
                            spanWidth,
                            destinationOffset,
                            0,
                            textureManager.textureClipSizes,
                            renderedScanline.Shade,
                            renderedScanline.ShadeStep);
                        break;
                    case CameraColourRenderMode.Vertex:
                        CameraPolygonDrawer.DrawVertexColourPolygon(
                            screenPixels,
                            spanWidth,
                            destinationOffset,
                            0,
                            textureManager.textureClipSizes,
                            renderedScanline.Shade,
                            renderedScanline.ShadeStep);
                        break;
                    case CameraColourRenderMode.Gradient:
                        CameraPolygonDrawer.DrawGradientPolygon(
                            screenPixels,
                            spanWidth,
                            destinationOffset,
                            0,
                            textureManager.textureClipSizes,
                            renderedScanline.Shade,
                            renderedScanline.ShadeStep);
                        break;
                }

                pixelOffset += rowStride;
            }
        }

    }
}
